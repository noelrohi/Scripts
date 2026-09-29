/*
name: GenerateQuestFilesv3
description: Lean quest data generator — fetch quests in throttled batches and save.
tags: debug, quest, data, generation, v3
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreFarms.cs

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using CommunityToolkit.Mvvm.DependencyInjection;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;
using Skua.Core.Scripts;

public class QuestFileUpdaterV3
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private IQuestDataLoaderService? service;

    public string OptionsStorage = "QuestFileUpdaterV3";
    public bool DontPreconfigure = true;

    public List<IOption> Options =
    [
        // Hard ceiling: requesting any ID past AE's last one (10882 as of now) disconnects the player. Bump this only when AE adds quests.
        new Option<int>("TargetQuestID", "Target Quest ID (hard cap)", "Never request IDs above this. AE's last ID is 10882; going past it DCs you.", 10882),
        new Option<string>("QuestRange", "Quest ID Range (start,end)", "Force-regenerate a specific range. Empty = continue from last ID + 1.", ""),
        new Option<int>("BatchSize", "Batch Size", "Quest IDs per request.", 30),
        new Option<string>("SkipRange", "Skip Quest Range (start,end)", "Skip this range entirely.", ""),
        // v3.1: throttling options — too many back-to-back requests is what gets the client kicked.
        new Option<int>("RequestDelay", "Request Delay (ms)", "Delay before every request. Raise this if you still get disconnected.", 1500),
        new Option<int>("MaxEmptyBatches", "Max Empty Batches", "Stop after this many consecutive empty batches (end of data).", 10),
        // Opt-in: AE gives no ID list and disconnects you on the first ID past their last one, so the only way to find new quests is to probe.
        new Option<bool>("ProbePastCap", "Probe Past Cap (DCs you)", "After finishing, probe IDs above the cap one at a time to find new quests. Ends with a disconnect at AE's real last ID (data is saved first, and the DC ID is remembered).", false),
        CoreBots.Instance.SkipOptions,
    ];

    // v3.1: after this many requests, take a longer break so the server never sees a constant stream.
    private const int CooldownEvery = 20;
    private const int CooldownMs = 5000;
    // v3.1: log a progress line only every N successful batches instead of every batch.
    private const int LogEveryBatches = 10;

    private int requestDelay = 1500;
    private int requestCount;
    private bool aborted;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        try
        {
            string clientPath = Path.Combine(ClientFileSources.SkuaDIR, "QuestData.json");
            string scriptsPath = Path.Combine(ClientFileSources.SkuaScriptsDIR, "QuestData.json");

            service ??= Ioc.Default.GetRequiredService<IQuestDataLoaderService>();

            int targetMaxId = Bot.Config!.Get<int>("TargetQuestID");
            string range = Bot.Config!.Get<string>("QuestRange") ?? "";
            int batchSize = Bot.Config!.Get<int>("BatchSize");
            string skipRaw = Bot.Config!.Get<string>("SkipRange") ?? "";
            int maxEmpty = Bot.Config!.Get<int>("MaxEmptyBatches");
            requestDelay = Bot.Config!.Get<int>("RequestDelay");
            bool probePast = Bot.Config!.Get<bool>("ProbePastCap");

            // A previous probe run records the ID that disconnected us; the real ceiling is one below it.
            // This overrides TargetQuestID, which is only the fallback until a probe has found the ceiling.
            string ceilingPath = Path.Combine(ClientFileSources.SkuaDIR, "QuestCeiling.txt");
            if (File.Exists(ceilingPath) && int.TryParse(File.ReadAllText(ceilingPath).Trim(), out int knownBad))
            {
                targetMaxId = knownBad - 1;
                Core.Logger($"Known ceiling: {targetMaxId} (ID {knownBad} disconnects).");
                // Keep the saved option in sync (Set() persists it automatically for non-transient options).
                if (Bot.Config!.Get<int>("TargetQuestID") != targetMaxId)
                    Bot.Config!.Set("TargetQuestID", targetMaxId);
            }

            batchSize = Math.Clamp(batchSize, 1, 500);
            maxEmpty = Math.Max(maxEmpty, 1);
            // v3.1: never allow a delay so small that it defeats the throttle.
            requestDelay = Math.Max(requestDelay, 250);

            // Parse skip range
            int skipStart = 0, skipEnd = 0;
            if (!string.IsNullOrWhiteSpace(skipRaw))
            {
                string[] parts = skipRaw.Split(',');
                if (parts.Length >= 2 && int.TryParse(parts[0], out int ss) && int.TryParse(parts[1], out int se))
                {
                    skipStart = ss;
                    skipEnd = se;
                    Core.Logger($"Skipping quest range {skipStart} to {skipEnd}.");
                }
            }

            // Ensure target directories exist
            Directory.CreateDirectory(Path.GetDirectoryName(clientPath)!);
            Directory.CreateDirectory(Path.GetDirectoryName(scriptsPath)!);

            // Load existing data
            List<QuestData> existingData = File.Exists(clientPath)
                ? service.GetFromFileAsync(clientPath).GetAwaiter().GetResult()
                : [];

            Dictionary<int, QuestData> map = new(existingData.Count);
            foreach (QuestData q in existingData)
                map[q.ID] = q;

            // Determine range to fetch
            int fetchStart, fetchEnd;

            if (!string.IsNullOrWhiteSpace(range))
            {
                string[] parts = range.Split(',');
                fetchStart = parts.Length > 0 && int.TryParse(parts[0], out int ps) ? ps : 1;
                // Clamp to the hard cap so a manual range can never request past AE's last ID (that DCs the player).
                fetchEnd = Math.Min(parts.Length > 1 && int.TryParse(parts[1], out int pe) ? pe : targetMaxId, targetMaxId);
                Core.Logger($"Range mode: {fetchStart} to {fetchEnd}");

                // Remove existing entries in range
                existingData.RemoveAll(q => q.ID >= fetchStart && q.ID <= fetchEnd);
                foreach (int id in Enumerable.Range(fetchStart, fetchEnd - fetchStart + 1))
                    map.Remove(id);
            }
            else
            {
                fetchStart = existingData.Count > 0 ? existingData.Max(q => q.ID) + 1 : 1;
                fetchEnd = targetMaxId;
                Core.Logger($"Delta mode: {fetchStart} to {fetchEnd}");
            }

            // No early return: when already up to date we still want the opt-in probe below to run.
            if (fetchStart > fetchEnd)
                Core.Logger("Nothing to fetch in the known range.");

            // Fetch in batches
            int added = 0, updated = 0, emptyInARow = 0, batchCount = 0;
            HashSet<int> seenThisRun = [];

            int s = fetchStart;
            // v3.1: `aborted` is set when the player drops, so we stop instead of hammering a dead session.
            while (s <= fetchEnd && !Bot.ShouldExit && !aborted)
            {
                int e = Math.Min(s + batchSize - 1, fetchEnd);

                // Check skip range — if any ID in [s, e] falls within [skipStart, skipEnd]
                if (skipEnd > 0 && e >= skipStart && s <= skipEnd)
                {
                    // Process the portion before the skip range first (logging removed: the skip range is already announced once above)
                    if (s < skipStart)
                        FetchProbe(service, clientPath, s, skipStart - 1, existingData, map, seenThisRun, ref added, ref updated);

                    s = skipEnd + 1;
                    continue;
                }

                // Skip if all IDs in this batch are already in the map
                if (existingData.Count > 0 && Enumerable.Range(s, e - s + 1).All(map.ContainsKey))
                {
                    s += batchSize;
                    continue;
                }

                int foundInBatch = FetchProbe(service, clientPath, s, e, existingData, map, seenThisRun, ref added, ref updated);
                if (foundInBatch == 0)
                {
                    // v3.1: empty batches are silent now; only the stop condition is logged.
                    if (++emptyInARow >= maxEmpty)
                    {
                        Core.Logger($"{maxEmpty} consecutive empty batches, stopping at quest {s}.");
                        break;
                    }
                    s += batchSize;
                    continue;
                }

                emptyInARow = 0;
                batchCount++;

                // v3.1: one progress line every LogEveryBatches batches instead of two lines per batch.
                if (batchCount % LogEveryBatches == 0)
                    Core.Logger($"Progress: quest {e}/{fetchEnd} | Added: {added} | Updated: {updated}");

                // Auto-save every 10 batches so progress isn't lost (quiet save, no log)
                if (batchCount % 10 == 0)
                    SaveFiles(existingData, clientPath, scriptsPath, false);

                s += batchSize;
            }

            SaveFiles(existingData, clientPath, scriptsPath);

            // Opt-in: look for new quests past the cap. Only runs if the whole known range finished cleanly.
            if (probePast && !aborted && !Bot.ShouldExit && s > fetchEnd)
                ProbePastCeiling(service, clientPath, scriptsPath, fetchEnd + 1, ceilingPath, maxEmpty, existingData, map, seenThisRun, ref added, ref updated);

            Core.Logger($"{(aborted ? "Stopped early (disconnected). " : "")}Done. Total: {existingData.Count} | Added: {added} | Updated: {updated}");
        }
        catch (System.Exception ex)
        {
            Core.Logger("Error: " + ex.ToString());
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void SaveFiles(List<QuestData> data, string clientPath, string scriptsPath, bool log = true)
    {
        string json = JsonConvert.SerializeObject(data, Formatting.Indented);
        File.WriteAllText(clientPath, json);
        try
        {
            File.Copy(clientPath, scriptsPath, true);
        }
        catch (System.Exception ex)
        {
            Core.Logger($"Warning: Failed to copy quest data to scripts path: {ex.Message}");
        }
        if (log)
            Core.Logger($"Saved {data.Count} quests.");
    }

    private static bool QuestChanged(QuestData a, QuestData b)
    {
        if (a.ID != b.ID) return true;
        if (a.Slot != b.Slot) return true;
        if (a.Value != b.Value) return true;
        if (a.Name != b.Name) return true;
        if (a.Once != b.Once) return true;
        if (a.Field != b.Field) return true;
        if (a.Index != b.Index) return true;
        if (a.Upgrade != b.Upgrade) return true;
        if (a.Level != b.Level) return true;
        if (a.RequiredClassID != b.RequiredClassID) return true;
        if (a.RequiredClassPoints != b.RequiredClassPoints) return true;
        if (a.RequiredFactionId != b.RequiredFactionId) return true;
        if (a.RequiredFactionRep != b.RequiredFactionRep) return true;
        if (a.Gold != b.Gold) return true;
        if (a.XP != b.XP) return true;

        if ((a.AcceptRequirements == null) != (b.AcceptRequirements == null)) return true;
        if ((a.Requirements == null) != (b.Requirements == null)) return true;
        if ((a.Rewards == null) != (b.Rewards == null)) return true;
        if ((a.SimpleRewards == null) != (b.SimpleRewards == null)) return true;

        if (!ItemBaseListsEqual(a.AcceptRequirements, b.AcceptRequirements)) return true;
        if (!ItemBaseListsEqual(a.Requirements, b.Requirements)) return true;
        if (!ItemBaseListsEqual(a.Rewards, b.Rewards)) return true;
        if (!SimpleRewardListsEqual(a.SimpleRewards, b.SimpleRewards)) return true;

        return false;
    }

    private static bool ItemBaseListsEqual(List<ItemBase>? a, List<ItemBase>? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return JsonConvert.SerializeObject(a) == JsonConvert.SerializeObject(b);
    }

    private static bool SimpleRewardListsEqual(List<SimpleReward>? a, List<SimpleReward>? b)
    {
        if (a == null && b == null) return true;
        if (a == null || b == null) return false;
        return JsonConvert.SerializeObject(a) == JsonConvert.SerializeObject(b);
    }

    private void ProcessBatch(
        List<QuestData> batch,
        List<QuestData> existingData,
        Dictionary<int, QuestData> map,
        HashSet<int> seenThisRun,
        ref int added,
        ref int updated)
    {
        foreach (QuestData quest in batch)
        {
            if (!seenThisRun.Add(quest.ID))
                continue;

            if (!map.TryGetValue(quest.ID, out QuestData? old))
            {
                existingData.Add(quest);
                map[quest.ID] = quest;
                added++;
            }
            else if (QuestChanged(old, quest))
            {
                int idx = existingData.FindIndex(x => x.ID == quest.ID);
                if (idx >= 0) existingData[idx] = quest;
                map[quest.ID] = quest;
                updated++;
            }
        }
    }

    /// <summary>
    /// v3.1: single choke point for every loader request. Adds the delay, a periodic cooldown,
    /// a connection check, and error handling so nothing else can spam the server.
    /// Returns null when the request failed, was empty, or the script should stop.
    /// </summary>
    private List<QuestData>? Request(IQuestDataLoaderService loader, string filePath, int start, int end)
    {
        if (Bot.ShouldExit || aborted)
            return null;

        // Stop cleanly if the player dropped instead of firing requests into a dead session.
        if (Bot.Player?.LoggedIn != true)
        {
            Core.Logger("Player is no longer logged in, stopping. Saved progress is kept.");
            aborted = true;
            return null;
        }

        Bot.Sleep(requestDelay);

        // Longer breather every CooldownEvery requests.
        if (++requestCount % CooldownEvery == 0)
            Bot.Sleep(CooldownMs);

        try
        {
            return loader.UpdateRangeAsync(filePath, start, end, null, CancellationToken.None).GetAwaiter().GetResult();
        }
        catch (System.Exception ex)
        {
            // Back off hard on errors rather than immediately retrying the next range.
            Core.Logger($"Request {start}-{end} failed: {ex.Message}");
            Bot.Sleep(5000);
            return null;
        }
    }

    /// <summary>
    /// Opt-in discovery of new quests past the known cap, one ID at a time.
    /// AE disconnects the player on the first ID past their real last one, so the ID is written to a marker file
    /// BEFORE each request: if we get kicked, the file survives and future runs cap themselves just below it.
    /// If we survive, the marker is cleared and we move on. Found quests are saved immediately.
    /// </summary>
    private void ProbePastCeiling(
        IQuestDataLoaderService loader,
        string clientPath,
        string scriptsPath,
        int firstId,
        string ceilingPath,
        int maxEmpty,
        List<QuestData> existingData,
        Dictionary<int, QuestData> map,
        HashSet<int> seenThisRun,
        ref int added,
        ref int updated)
    {
        Core.Logger($"Probing past the cap from {firstId}, one ID at a time. Hitting AE's real last ID will disconnect you (data is already saved).");
        int emptyInARow = 0;

        for (int id = firstId; !Bot.ShouldExit && !aborted && emptyInARow < maxEmpty; id++)
        {
            // Already offline before we even ask: not this ID's fault, so don't record it.
            if (Bot.Player?.LoggedIn != true)
            {
                aborted = true;
                return;
            }

            File.WriteAllText(ceilingPath, id.ToString());
            List<QuestData>? found = Request(loader, clientPath, id, id);

            // Logged in before, offline now: this ID kicked us. Leave the marker in place.
            if (Bot.Player?.LoggedIn != true)
            {
                Core.Logger($"Disconnected at ID {id}, recorded as the ceiling. Future runs will stop at {id - 1}.");
                aborted = true;
                return;
            }

            // Survived, so this ID is safe: clear the marker and raise the saved TargetQuestID to it.
            // Done per ID (not at the end) so the option is already correct if the next probe disconnects us.
            File.Delete(ceilingPath);
            Bot.Config!.Set("TargetQuestID", id);

            if (found?.Count > 0)
            {
                ProcessBatch(found, existingData, map, seenThisRun, ref added, ref updated);
                SaveFiles(existingData, clientPath, scriptsPath, false);
                emptyInARow = 0;
                Core.Logger($"Found new quest {id}.");
            }
            else
                emptyInARow++;
        }
    }

    /// <summary>
    /// Tries to fetch a range of quests. If the batch returns empty (possible undefined IDs poisoning the request),
    /// falls back to sub-batches, then individual IDs. Every request goes through <see cref="Request"/>.
    /// Returns the number of quests found.
    /// </summary>
    private int FetchProbe(
        IQuestDataLoaderService loader,
        string filePath,
        int start, int end,
        List<QuestData> existingData,
        Dictionary<int, QuestData> map,
        HashSet<int> seenThisRun,
        ref int added,
        ref int updated)
    {
        // Fast path: try the full range first
        List<QuestData>? batch = Request(loader, filePath, start, end);
        if (batch?.Count > 0)
        {
            ProcessBatch(batch, existingData, map, seenThisRun, ref added, ref updated);
            return batch.Count;
        }

        // Full batch returned empty — split into smaller sub-batches before going 1-by-1.
        int found = 0;
        const int subBatchSize = 5;

        for (int subStart = start; subStart <= end && !Bot.ShouldExit && !aborted; subStart += subBatchSize)
        {
            int subEnd = Math.Min(subStart + subBatchSize - 1, end);
            List<QuestData>? sub = Request(loader, filePath, subStart, subEnd);

            if (sub?.Count > 0)
            {
                ProcessBatch(sub, existingData, map, seenThisRun, ref added, ref updated);
                found += sub.Count;
                continue;
            }

            // Sub-batch still empty — probe each ID individually
            for (int probe = subStart; probe <= subEnd && !Bot.ShouldExit && !aborted; probe++)
            {
                List<QuestData>? single = Request(loader, filePath, probe, probe);
                if (single?.Count > 0)
                {
                    ProcessBatch(single, existingData, map, seenThisRun, ref added, ref updated);
                    found += single.Count;
                }
            }
        }

        return found;
    }
}