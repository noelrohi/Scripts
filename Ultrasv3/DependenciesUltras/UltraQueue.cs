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
    public static IEnumerable<string> GetSharedBossQueue(dynamic ultra, IScriptInterface bot, dynamic C, IEnumerable<string> bosses, string bossSyncFile, string participantSyncFile, Func<string, bool> isBossComplete)
    {
        if (bot == null)
            return bosses;

        // Ultras-v3 are designed for a fixed 4-player army queue.
        const int armySize = 4;
        string syncPath = ultra.ResolveSyncPath(bossSyncFile);
        string user = bot.Player?.Username ?? Guid.NewGuid().ToString();

        string payload = string.Join(",", bosses.Select(b => $"{b}={(isBossComplete(b) ? "true" : "false")}"));
        ultra.UpdateEntry(syncPath, user, payload);

        string participantPath = ultra.ResolveSyncPath(participantSyncFile);
        if (ShouldClearParticipantSync(participantPath))
            ultra.ClearSyncFile(participantPath);
        else
            CleanupStaleParticipants(ultra, C, participantPath);

        ultra.UpdateEntry(participantPath, user, "1");

        int lastCount = -1;
        const int staleThreshold = 600;
        DateTime waitStarted = DateTime.UtcNow;
        const int timeoutMs = 60000;

        while (!bot.ShouldExit)
        {
            int count = 0;
            foreach (string line in ultra.ReadLines(participantPath))
            {
                string[] parts = line.Split(':');
                if (parts.Length < 3)
                    continue;

                if (!long.TryParse(parts[^1], out long ts))
                    continue;

                if (DateTimeOffset.UtcNow.ToUnixTimeSeconds() - ts > staleThreshold)
                    continue;

                count++;
            }

            if (count != lastCount)
            {
                lastCount = count;
                C.Logger($"[AllUltras-v3] Registered participants: {count}/{armySize}", "Info");
            }

            if (count >= armySize)
                break;

            if ((DateTime.UtcNow - waitStarted).TotalMilliseconds >= timeoutMs && count > 0)
            {
                C.Logger($"[AllUltras-v3] Wait timeout reached — proceeding with {count}/{armySize} registered clients.", "Warning");
                break;
            }

            ultra.UpdateEntry(syncPath, user, payload);
            ultra.UpdateEntry(participantPath, user, "1");
            bot.Sleep(250);
        }

        if (bot.ShouldExit)
            return bosses;

        // Ensure all accounts have written their latest payload before reading
        ultra.UpdateEntry(syncPath, user, payload);
        bot.Sleep(3000);

        // A boss runs when any participant hasn't turned its quest in this period; it is
        // skipped only when every participant has. All accounts read the same file, so they
        // agree on the queue.
        long now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        HashSet<string> participants = FreshKeys((string[])ultra.ReadLines(participantPath), now, staleThreshold);
        var pendingFor = bosses.ToDictionary(b => b, b => new List<string>(), StringComparer.OrdinalIgnoreCase);
        var reported = new List<string>();

        foreach (string line in ultra.ReadLines(syncPath))
        {
            string[] parts = line.Split(':');
            if (parts.Length < 3)
                continue;

            if (!long.TryParse(parts[^1], out long ts))
                continue;

            if (now - ts > staleThreshold)
                continue;

            string account = parts[0];
            if (participants.Count > 0 && !participants.Contains(account))
                continue;

            reported.Add(account);
            string payloadLine = parts[1];
            var statuses = payloadLine.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Split(new[] { '=' }, 2))
                .Where(pair => pair.Length == 2)
                .ToDictionary(pair => pair[0].Trim(), pair => pair[1].Trim(), StringComparer.OrdinalIgnoreCase);

            foreach (string boss in bosses)
            {
                if (!statuses.TryGetValue(boss, out string? rawValue)
                    || !bool.TryParse(rawValue, out bool complete)
                    || !complete)
                {
                    pendingFor[boss].Add(account);
                }
            }
        }

        // A participant that never published its statuses can't be assumed done.
        foreach (string missing in participants.Where(p => !reported.Contains(p, StringComparer.OrdinalIgnoreCase)))
        {
            foreach (string boss in bosses)
                pendingFor[boss].Add(missing);
        }

        var queue = bosses.Where(b => pendingFor[b].Count > 0).ToList();
        var skipped = bosses.Where(b => pendingFor[b].Count == 0).ToList();
        if (skipped.Count > 0)
            C.Logger($"[AllUltras-v3] Skipping {string.Join(", ", skipped)}: all {reported.Count} participant(s) turned the quest in this period.", "Info");
        foreach (string boss in queue)
            C.Logger($"[AllUltras-v3] Running {boss}: not turned in this period by {string.Join(", ", pendingFor[boss])}.", "Info");

        return queue;
    }

    private static HashSet<string> FreshKeys(string[] lines, long now, int staleThreshold)
    {
        var keys = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (string line in lines)
        {
            string[] parts = line.Split(':');
            if (parts.Length >= 3 && long.TryParse(parts[^1], out long ts) && now - ts <= staleThreshold)
                keys.Add(parts[0]);
        }
        return keys;
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

    private static void CleanupStaleParticipants(dynamic ultra, dynamic C, string path)
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
                C.Logger($"[AllUltras-v3] Cleaned stale participant entries; {freshLines.Count} remain.", "Info");
            }
        }
        catch (Exception ex)
        {
            C.Logger($"[AllUltras-v3] Failed to cleanup participant sync: {ex.Message}", "Warning");
        }
    }
}
