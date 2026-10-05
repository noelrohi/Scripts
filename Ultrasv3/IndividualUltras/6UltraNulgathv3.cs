/*
name: UltraNulgathv3
description: Ultra Nulgath v3 — default 3 taunters + 1 Blade attacker; with a party layout of Dragon of Time / Legion Revenant / ArchPaladin / Lord of Order, the DoT hits each new Overfiend Blade then Nulgath while LR and AP taunt Nulgath on a 10s cycle.
tags: null
*/
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraEnhancements.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPotions.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraCustomClassSync.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraWaitForArmy.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/GetScrolls.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraAsync.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraDeath.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPartyLayout.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs

using System;
using System.IO;
using System.Linq;
using System.Threading;
using Skua.Core.Interfaces;

public class UltraNulgathv3
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;
    private static CoreEnginev3 Engine => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;
    private static UltraEnhancements Enh => _Enh ??= new UltraEnhancements();
    private static UltraEnhancements _Enh;
    private static UltraPotions Pots => _Pots ??= new UltraPotions();
    private static UltraPotions _Pots;
    private static GetScrolls Scrolls => _Scrolls ??= new GetScrolls();
    private static GetScrolls _Scrolls;
    private static string _fbsMuteFile = "";

    // Monster MapIDs in ultranulgath.
    private const int Blade = 1;
    private const int Nulgath = 2;

    // Default role set, used when no party layout is set.
    private const string Taunter1 = "Lord of Order";
    private const string Taunter2 = "StoneCrusher";
    private const string Taunter3AttackBlade = "Verus DoomKnight";
    private const string DPSAttackBlade = "King's Echo";

    private static readonly string[][] UltraClassesByRole =
    {
        new[] { Taunter1 },
        new[] { Taunter2 },
        new[] { Taunter3AttackBlade },
        new[] { DPSAttackBlade }
    };

    // Party role set, used when the layout names only these classes. It beat Nulgath
    // first try with no deaths: the Dragon of Time hits each new Overfiend Blade briefly
    // and otherwise Nulgath, Legion Revenant and ArchPaladin taunt Nulgath 5s apart on a
    // 10s cycle, Lord of Order hits Nulgath.
    private const string BladeHitter = "Dragon of Time";
    private const string FirstTaunter = "Legion Revenant";
    private const string SecondTaunter = "ArchPaladin";
    private const string NulgathAttacker = "Lord of Order";

    private static readonly string[][] PartyClassesByRole =
    {
        new[] { BladeHitter },
        new[] { FirstTaunter },
        new[] { SecondTaunter },
        new[] { NulgathAttacker }
    };

    // How long the Dragon of Time stays on each new Blade before going back to Nulgath.
    private const int BladeHitMs = 1500;

    private CancellationTokenSource _tauntCts = new();
    private CancellationTokenSource _wipeCts = new();
    private ManualResetEvent _retreatComplete = new(false);
    private UltraDeath.RetryCounter _deathRetries = new();
    private const int MaxDeathRetries = 10;
    private DateTime fightStartTime = DateTime.MinValue;
    private UltraPartyLayout _party = null!;
    private bool _partyRoles;
    private string _role = "";

    public void ScriptMain(IScriptInterface bot)
    {
        RunBoss();
        Bot.StopSync();
    }

    public void RunBoss()
    {
        C.SetOptions(true);
        _fbsMuteFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Skua", "fbs_mute.sync"
        );
        try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }

        try
        {
            Engine.Boot();
            if (!Prep())
                return;

            while (_deathRetries.Value < MaxDeathRetries && !Bot.ShouldExit)
            {
                Engine.Boot();
                _tauntCts?.Cancel();
                _tauntCts = new();
                _wipeCts = new();
                _retreatComplete.Reset();
                Bot.Events.ScriptStopping -= StopTauntEvent;
                Bot.Events.ScriptStopping += StopTauntEvent;

                // Start background wipe monitor (also handles individual death signaling)
                UltraDeath.StartWipeMonitor(
                    C, 4, _wipeCts, _retreatComplete,
                    () => UltraDeath.PerformRetreat(C, 4, MaxDeathRetries, _deathRetries, "UltraNulgathRetreat.sync")
                );

                Fight();
            }
        }
        finally
        {
            Bot.Events.ScriptStopping -= StopTauntEvent;
            _tauntCts.Cancel();
            _wipeCts.Cancel();
            try { if (File.Exists(_fbsMuteFile)) File.Delete(_fbsMuteFile); } catch { }
            Engine.DisableSkills();
            C.SetOptions(false);
        }
    }

    private bool StopTauntEvent(Exception? e)
    {
        _tauntCts.Cancel();
        return true;
    }

    private bool IsTaunter() => _partyRoles
        ? _role == "FirstTaunter" || _role == "SecondTaunter"
        : _role != "DPSAttackBlade";

    /// <summary>
    /// Equips this account's class once and fixes its role from the class it ended up on.
    /// Retries after a wipe keep the role.
    /// </summary>
    private bool Prep()
    {
        _party = UltraPartyLayout.Read("UltraNulgath");
        _partyRoles = _party.Uses(PartyClassesByRole.SelectMany(r => r));
        if (_party.IsSet && !_partyRoles && !_party.Uses(UltraClassesByRole.SelectMany(r => r)))
        {
            C.Logger("[UltraNulgath-v3] The layout mixes role sets. Use either Dragon of Time / Legion Revenant / ArchPaladin / Lord of Order, " +
                "or Lord of Order / StoneCrusher / Verus DoomKnight / King's Echo.", "Error", messageBox: true, stopBot: true);
            return false;
        }

        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);

        C.Logger($"[UltraNulgath-v3] Equipping {(_partyRoles ? "party" : "default")} role classes for army size 4.");
        string assigned = _partyRoles
            ? _party.EquipClass(Ultra, PartyClassesByRole, 4, "ultra_nulgath_class-v3.sync")
            : _party.EquipClass(Ultra, UltraClassesByRole, 4, "ultra_nulgath_class-v3.sync", allowDuplicates: true);
        if (string.IsNullOrEmpty(assigned) || !_party.EnsureClass())
            return false;

        string className = Bot.Player.CurrentClass?.Name ?? string.Empty;
        _role = _partyRoles
            ? className switch
            {
                BladeHitter => "BladeHitter",
                FirstTaunter => "FirstTaunter",
                SecondTaunter => "SecondTaunter",
                _ => "NulgathAttacker"
            }
            : className switch
            {
                Taunter1 => "Taunter1",
                Taunter2 => "Taunter2",
                Taunter3AttackBlade => "Taunter3AttackBlade",
                _ => "DPSAttackBlade"
            };

        if (_partyRoles)
            Enh.ApplyNulgathParty();
        else
            Enh.ApplyNulgath();

        C.Logger($"[UltraNulgath-v3] Role: {_role} ({className})");
        return true;
    }

    private void Fight()
    {
        const string map = "ultranulgath";
        const string boss = "Nulgath the Archfiend";
        const string bossDefeatedTemp = "Nulgath the Archfiend Defeated?";

        const string waitSyncFile = "ultra_nulgath.sync";
        const string fightTimeSyncFile = "UltraNulgathFightTime.sync";
        const string completionSyncFile = "UltraNulgathCompletion.sync";
        int armySize = 4;

        const int questId = 8692;

        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(fightTimeSyncFile));
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        // Potions are picked from the equipped class, so it has to be the role's class.
        // Party roles: only the Dragon of Time drinks.
        string potionContext = _partyRoles ? "NulgathParty" : "";
        bool skipThird = !_partyRoles && IsTaunter();
        _party.EnsureClass();
        Pots.EnsureRecommendedPotions(skipThird: skipThird, context: potionContext);
        if (!_partyRoles || IsTaunter())
            Scrolls.GetScrollOfEnrage();
        _party.EnsureClass();

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        Pots.UseRecommendedPotions(skipThird: skipThird, ensureStock: false, context: potionContext);

        if (IsTaunter())
        {
            C.Logger("[UltraNulgath-v3] Taunter detected, equipping Scroll of Enrage.");
            Engine.EquipEnrage();
            if (!Bot.Inventory.IsEquipped("Scroll of Enrage"))
                C.Logger("[UltraNulgath-v3] Scroll of Enrage is not equipped, this taunter cannot taunt.", "Warning");
        }

        _party.EnsureClass();
        Engine.Join(map);
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        var (bestCell, bestPad) = Engine.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(2000);

        // Pre-seed completion sync file so all 4 entries exist before the loop starts.
        string? _username = Bot.Player.Username;
        string? _className = Bot.Player.CurrentClass?.Name;
        if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_className))
        {
            string _myKey = $"{_username}|{_className}".Replace(":", "-");
            Ultra.UpdateEntry(Ultra.ResolveSyncPath(completionSyncFile), _myKey, "0");
        }

        string fightTimeSyncPath = Ultra.ResolveSyncPath(fightTimeSyncFile);

        Func<bool> shouldSkipTaunt = () => Engine.HasAura("Contract of Despair", true);

        // Set or retrieve fight start time, then launch the appropriate taunter loop
        if (_role == "Taunter1")
        {
            C.Logger("[UltraNulgath-v3] Taunter1 (Primary) — setting fight start time.");
            fightStartTime = UltraAsync.SetFightTime(C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 0, 3, shouldSkipTaunt, cancellationToken: _tauntCts.Token);
        }
        else if (_role == "Taunter2")
        {
            C.Logger("[UltraNulgath-v3] Taunter2 — reading fight start time.");
            fightStartTime = UltraAsync.GetFightTime(Ultra, C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 1, 3, shouldSkipTaunt, cancellationToken: _tauntCts.Token);
        }
        else if (_role == "Taunter3AttackBlade")
        {
            C.Logger("[UltraNulgath-v3] Taunter3AttackBlade — reading fight start time.");
            fightStartTime = UltraAsync.GetFightTime(Ultra, C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 2, 3, shouldSkipTaunt, cancellationToken: _tauntCts.Token);
        }
        else if (_role == "FirstTaunter")
        {
            // Taunts at 0s of every 10s cycle.
            C.Logger("[UltraNulgath-v3] FirstTaunter (Primary) — setting fight start time.");
            fightStartTime = UltraAsync.SetFightTime(C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 0, 2, cancellationToken: _tauntCts.Token);
        }
        else if (_role == "SecondTaunter")
        {
            // Taunts at 5s of every 10s cycle.
            C.Logger("[UltraNulgath-v3] SecondTaunter — reading fight start time.");
            fightStartTime = UltraAsync.GetFightTime(Ultra, C, fightTimeSyncPath);
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 1, 2, cancellationToken: _tauntCts.Token);
        }

        bool bladeHit = false;

        while (!Bot.ShouldExit && !_wipeCts.IsCancellationRequested)
        {
            // Refresh mute file so FBS plugin stays muted during the fight
            try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }

            if (!Bot.Player.Alive)
            {
                // Death is signaled by the background wipe monitor
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains(bossDefeatedTemp, 1), completionSyncFile))
            {
                C.Logger("Nulgath the Archfiend defeated. Finishing quest.");
                Bot.Events.ScriptStopping -= StopTauntEvent;
                _tauntCts.Cancel();
                Engine.DisableSkills();
                Engine.Join(map);
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                _deathRetries.Value = MaxDeathRetries;
                break;
            }

            // Back to the fight if a respawn landed somewhere else.
            if (!string.IsNullOrEmpty(bestCell) && Bot.Player.Cell != bestCell)
            {
                C.Jump(bestCell, bestPad ?? "Left");
                Bot.Wait.ForCellChange(bestCell);
                continue;
            }

            // ── Dynamic targeting ──
            if (_role == "BladeHitter")
            {
                // A short hit on each new Blade, then back to Nulgath.
                bool bladeUp = Bot.Monsters.MapMonsters.Any(m => m != null && m.MapID == Blade && m.HP > 0);
                if (!bladeUp)
                    bladeHit = false;

                if (bladeUp && !bladeHit)
                {
                    Bot.Combat.Attack(Blade);
                    Bot.Sleep(BladeHitMs);
                    bladeHit = true;
                    Bot.Combat.Attack(Nulgath);
                }
                else if (Bot.Player.Target?.MapID != Nulgath)
                    Bot.Combat.Attack(Nulgath);
            }
            else if (_partyRoles)
            {
                // FirstTaunter, SecondTaunter, NulgathAttacker — always on Nulgath; the taunt
                // loop presses the scroll on whatever is targeted.
                if (Bot.Player.Target?.MapID != Nulgath)
                    Bot.Combat.Attack(Nulgath);
            }
            // Taunter1/Taunter2: always attack Nulgath; DPSAttackBlade: attack Blade
            // Taunter3: attack Blade normally, switch to Nulgath during taunt pulse;
            //           if Blade is dead, fall back to Nulgath
            else if (_role == "Taunter3AttackBlade" && !shouldSkipTaunt())
            {
                double elapsed = (DateTime.UtcNow - fightStartTime).TotalSeconds;
                double timeInCycle = elapsed % 15;
                // Taunter3 fires at t=10,25,40... (index 2, 5s interval, 15s cycle)
                // Pre-switch to Nulgath at 9s (1s early), stay until 13s (taunt ~3s)
                bool targetNulgath = timeInCycle >= 9 && timeInCycle <= 13;

                if (targetNulgath)
                {
                    if (Bot.Player.Target?.MapID != Nulgath)
                        Bot.Combat.Attack(Nulgath);
                }
                else
                {
                    // Attack Blade if alive, else fall back to Nulgath
                    if (Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.MapID == Blade && x.HP > 0))
                    {
                        if (Bot.Player.Target?.MapID != Blade)
                            Bot.Combat.Attack(Blade);
                    }
                    else if (Bot.Player.Target?.MapID != Nulgath)
                    {
                        Bot.Combat.Attack(Nulgath);
                    }
                }
            }
            else if (_role == "DPSAttackBlade")
            {
                // DPSAttackBlade — attack Blade if alive, else Nulgath
                if (Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.MapID == Blade && x.HP > 0))
                {
                    if (Bot.Player.Target?.MapID != Blade)
                        Bot.Combat.Attack(Blade);
                }
                else if (Bot.Player.Target?.MapID != Nulgath)
                {
                    Bot.Combat.Attack(Nulgath);
                }
            }
            else
            {
                // Taunter1, Taunter2 — always on Nulgath
                if (Bot.Player.Target?.Name != boss)
                    Bot.Combat.Attack(boss);
            }

            // Party taunters and Lord of Order use no potions.
            if (!_partyRoles || _role == "BladeHitter")
                Pots.ActivateEquippedPotion();

            Bot.Sleep(_partyRoles ? 100 : 500);
        }

        // The taunt loop must not keep pressing during a retreat.
        _tauntCts.Cancel();

        // If retreat is still in progress (background), wait for it
        if (_wipeCts.IsCancellationRequested)
            _retreatComplete.WaitOne(TimeSpan.FromSeconds(120));
    }
}
