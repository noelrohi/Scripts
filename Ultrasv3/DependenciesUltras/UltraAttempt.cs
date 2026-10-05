/*
name: null
description: null
tags: null
*/

using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Monsters;

/// <summary>
/// One Attempt at an Ultra on this account, from engaging the boss to its kill, a Wipe, or the
/// bot stopping. It notes this account's deaths and the boss's HP as the fight goes, and its
/// end is recorded once as an <c>ultra.attempt</c> Script Report:
/// <c>{ boss, comp, class, role, startedAt, endedAt, outcome, bossHp, bossMaxHp, deaths: [ { atSec } ] }</c>.
/// The report names no account; the agent knows the account from the Engine it came from.
/// </summary>
public class UltraAttempt
{
    private static IScriptInterface Bot => IScriptInterface.Instance;

    public const string ReportName = "ultra.attempt";

    /// <summary>How an Attempt ended; reported as <c>kill</c>, <c>wipe</c> or <c>stopped</c>.</summary>
    public enum Outcome { Kill, Wipe, Stopped }

    // The game can say the player died more than once for one death.
    private const double SameDeathSec = 5;

    private readonly object _lock = new();
    private readonly string _boss = string.Empty;
    private readonly string _comp = string.Empty;
    private readonly string _class = string.Empty;
    private readonly string _role = string.Empty;
    private readonly string _map = string.Empty;
    private readonly Func<Monster, bool> _isBoss = _ => false;
    private readonly DateTime _startedAt;
    private readonly List<double> _deaths = new();
    private int? _bossHp;
    private int? _bossMaxHp;
    private bool _ended = true;

    // Skua instantiates every included Script's class; Attempts come from Begin.
    public UltraAttempt() { }

    private UltraAttempt(string boss, string comp, string className, string role, string map, Func<Monster, bool> isBoss)
    {
        _boss = boss;
        _comp = comp;
        _class = className;
        _role = role;
        _map = map;
        _isBoss = isBoss;
        _startedAt = DateTime.UtcNow;
        _ended = false;
    }

    /// <summary>
    /// Starts an Attempt now. <paramref name="boss"/> is the DoAllUltras boss key, e.g. UltraNulgath;
    /// <paramref name="map"/> is the boss's map and <paramref name="isBoss"/> picks the boss among its monsters.
    /// </summary>
    public static UltraAttempt Begin(string boss, string comp, string className, string role, string map, Func<Monster, bool> isBoss)
    {
        UltraAttempt attempt = new(boss, comp, className, role, map, isBoss);
        Bot.Events.PlayerDeath += attempt.OnDeath;
        return attempt;
    }

    /// <summary>
    /// Notes the boss's HP while this account is in the boss's map, alive or dead. Call it as the
    /// fight goes; a Wipe notes it once more as it ends the Attempt, before the retreat leaves the map.
    /// </summary>
    public void SeeBoss()
    {
        if (!string.Equals(Bot.Map.Name, _map, StringComparison.OrdinalIgnoreCase))
            return;
        Monster? boss = Bot.Monsters.MapMonsters.FirstOrDefault(m => m != null && _isBoss(m));
        if (boss == null || boss.MaxHP <= 0)
            return;

        lock (_lock)
        {
            _bossHp = boss.HP;
            _bossMaxHp = boss.MaxHP;
        }
    }

    /// <summary>
    /// Ends the Attempt with <paramref name="outcome"/> and reports it. Only the first call reports,
    /// so the stop and cleanup paths can call it with <see cref="Outcome.Stopped"/> unconditionally.
    /// </summary>
    public void End(Outcome outcome)
    {
        if (outcome == Outcome.Wipe)
        {
            try { SeeBoss(); } catch { }
        }

        object report;
        lock (_lock)
        {
            if (_ended)
                return;
            _ended = true;

            if (outcome == Outcome.Kill)
                _bossHp = 0;

            report = new
            {
                boss = _boss,
                comp = _comp,
                @class = _class,
                role = _role,
                startedAt = _startedAt.ToString("o"),
                endedAt = DateTime.UtcNow.ToString("o"),
                outcome = outcome switch
                {
                    Outcome.Kill => "kill",
                    Outcome.Wipe => "wipe",
                    _ => "stopped",
                },
                bossHp = _bossHp,
                bossMaxHp = _bossMaxHp,
                deaths = _deaths.Select(atSec => new { atSec }).ToArray(),
            };
        }

        // Report records even while the script is stopping, so nothing may log before it here.
        Bot.Report(ReportName, report);
        try { Bot.Events.PlayerDeath -= OnDeath; } catch { }
    }

    private void OnDeath()
    {
        lock (_lock)
        {
            if (_ended)
                return;

            double atSec = Math.Round((DateTime.UtcNow - _startedAt).TotalSeconds, 1);
            if (_deaths.Count > 0 && atSec - _deaths[^1] < SameDeathSec)
                return;
            _deaths.Add(atSec);
        }
    }
}
