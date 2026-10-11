/*
name: null
description: null
tags: null
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs

using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class UltraQueue
{
    /// <summary>
    /// The bosses the party still has to run: each account publishes which bosses it has
    /// turned in this period, waits until every participant has published during this call,
    /// and queues a boss when any of them hasn't turned it in.
    /// </summary>
    public static IEnumerable<string> GetSharedBossQueue(dynamic ultra, IScriptInterface bot, dynamic C, IEnumerable<string> bosses, string bossSyncFile, string participantSyncFile, Func<string, bool> isBossComplete, string logPrefix = "[AllUltras-v3]")
    {
        if (bot == null)
            return bosses;

        // Ultras-v3 are designed for a fixed 4-player army queue.
        const int armySize = 4;
        const int staleThreshold = 600;
        const int timeoutMs = 60000;

        // Statuses stamped before this call are from an earlier pass, which an account may still
        // be replacing. The slack covers the sync files' whole-second stamps.
        long since = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - 2;

        string syncPath = ultra.ResolveSyncPath(bossSyncFile);
        string user = bot.Player?.Username ?? Guid.NewGuid().ToString();

        string payload = string.Join(",", bosses.Select(b => $"{b}={(isBossComplete(b) ? "true" : "false")}"));
        ultra.UpdateEntry(syncPath, user, payload);

        string participantPath = ultra.ResolveSyncPath(participantSyncFile);
        if (ShouldClearParticipantSync(participantPath))
            ultra.ClearSyncFile(participantPath);
        else
            CleanupStaleParticipants(ultra, C, participantPath, logPrefix);

        ultra.UpdateEntry(participantPath, user, "1");

        Dictionary<string, string> published = new(StringComparer.OrdinalIgnoreCase);
        int lastCount = -1;
        DateTime waitStarted = DateTime.UtcNow;

        while (!bot.ShouldExit)
        {
            published = EntriesSince((string[])ultra.ReadLines(syncPath), since);

            if (published.Count != lastCount)
            {
                lastCount = published.Count;
                C.Logger($"{logPrefix} Participants with this pass's statuses: {published.Count}/{armySize}", "Info");
            }

            if (published.Count >= armySize)
                break;

            if ((DateTime.UtcNow - waitStarted).TotalMilliseconds >= timeoutMs && published.Count > 0)
            {
                long recent = DateTimeOffset.UtcNow.ToUnixTimeSeconds() - staleThreshold;
                var missing = EntriesSince((string[])ultra.ReadLines(participantPath), recent).Keys
                    .Where(account => !published.ContainsKey(account))
                    .ToList();
                string who = missing.Count > 0 ? string.Join(", ", missing) : "accounts that haven't registered";
                C.Logger($"{logPrefix} Wait timeout reached, proceeding with {published.Count}/{armySize}. No statuses this pass from {who}.", "Warning");
                break;
            }

            ultra.UpdateEntry(syncPath, user, payload);
            ultra.UpdateEntry(participantPath, user, "1");
            bot.Sleep(250);
        }

        if (bot.ShouldExit)
            return bosses;

        // Restamp so an account still waiting, whose call began after this one, counts this
        // account's statuses as this pass's.
        ultra.UpdateEntry(syncPath, user, payload);

        // Every account decides from the same pass's statuses of the same participants, so the
        // party agrees on the queue (unless one publishes just as another's wait times out).
        var pendingFor = bosses.ToDictionary(b => b, b => new List<string>(), StringComparer.OrdinalIgnoreCase);
        foreach (KeyValuePair<string, string> entry in published)
        {
            var statuses = entry.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split(new[] { '=' }, 2))
                .Where(pair => pair.Length == 2)
                .ToDictionary(pair => pair[0].Trim(), pair => pair[1].Trim(), StringComparer.OrdinalIgnoreCase);

            foreach (string boss in bosses)
            {
                if (!statuses.TryGetValue(boss, out string? rawValue)
                    || !bool.TryParse(rawValue, out bool complete)
                    || !complete)
                {
                    pendingFor[boss].Add(entry.Key);
                }
            }
        }

        var queue = bosses.Where(b => pendingFor[b].Count > 0).ToList();
        var skipped = bosses.Where(b => pendingFor[b].Count == 0).ToList();
        if (skipped.Count > 0)
            C.Logger($"{logPrefix} Skipping {string.Join(", ", skipped)}: all {published.Count} participant(s) turned the quest in this period.", "Info");
        foreach (string boss in queue)
            C.Logger($"{logPrefix} Running {boss}: not turned in this period by {string.Join(", ", pendingFor[boss])}.", "Info");

        return queue;
    }

    /// <summary>Each account's payload from the sync lines (key:payload:stamp) stamped at or after <paramref name="since"/>.</summary>
    private static Dictionary<string, string> EntriesSince(string[] lines, long since)
    {
        var entries = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length >= 3 && long.TryParse(parts[^1], out long ts) && ts >= since)
                entries[parts[0]] = parts[1];
        }
        return entries;
    }

    private static bool ShouldClearParticipantSync(string path)
    {
        try
        {
            if (!System.IO.File.Exists(path))
                return true;

            return (DateTime.UtcNow - System.IO.File.GetLastWriteTimeUtc(path)).TotalMinutes > 10;
        }
        catch
        {
            return false;
        }
    }

    private static void CleanupStaleParticipants(dynamic ultra, dynamic C, string path, string logPrefix)
    {
        try
        {
            string[] lines = ultra.ReadLines(path);
            if (lines.Length == 0)
                return;

            long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            const int staleThreshold = 600;
            var freshLines = new List<string>();

            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                    continue;

                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;

                if (!long.TryParse(parts[^1], out long ts))
                    continue;

                if (now - ts <= staleThreshold)
                    freshLines.Add(line);
            }

            if (freshLines.Count == 0)
            {
                ultra.ClearSyncFile(path);
                return;
            }

            if (freshLines.Count != lines.Length)
            {
                System.IO.File.WriteAllText(path, string.Join(Environment.NewLine, freshLines));
                C.Logger($"{logPrefix} Cleaned stale participant entries; {freshLines.Count} remain.", "Info");
            }
        }
        catch (Exception ex)
        {
            C.Logger($"{logPrefix} Failed to cleanup participant sync: {ex.Message}", "Warning");
        }
    }
}
