/*
name: UltraAvatarTyndariusv3
description: Ultra Avatar Tyndarius v3 — King's Echo kills the right orb, Legion Revenant taunts the left orb, ArchPaladin taunts Tyndarius on a timer, Lord of Order hits Tyndarius.
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
using System.Linq;
using System.Threading;
using Skua.Core.Interfaces;

public class UltraAvatarTyndariusv3
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

    // Monster MapIDs in ultratyndarius, cell Boss.
    private const int LeftOrb = 1;
    private const int Tyndarius = 2;
    private const int RightOrb = 3;

    // One class per role. The class an account ends up on (from the party layout,
    // or the class sync without one) IS its role for the whole run.
    private const string RightOrbKiller = "King's Echo";
    private const string LeftOrbTaunter = "Legion Revenant";
    private const string TyndariusTaunter = "ArchPaladin";
    private const string TyndariusAttacker = "Lord of Order";

    private static readonly string[][] UltraClassesByRole =
    {
        new[] { RightOrbKiller },
        new[] { LeftOrbTaunter },
        new[] { TyndariusTaunter },
        new[] { TyndariusAttacker }
    };

    // Seconds between Tyndarius taunts. The ArchPaladin taunted every 12s in
    // the v2 layout that beat the boss.
    private const int TyndariusTauntIntervalSec = 12;

    private CancellationTokenSource _tauntCts = new();
    private CancellationTokenSource _wipeCts = new();
    private ManualResetEvent _retreatComplete = new(false);
    private UltraDeath.RetryCounter _deathRetries = new();
    private const int MaxDeathRetries = 10;
    private UltraPartyLayout _party = null!;
    private string _roleClass = "";

    public void ScriptMain(IScriptInterface bot)
    {
        RunBoss();
        Bot.StopSync();
    }

    public void RunBoss()
    {
        C.SetOptions(true);

        try
        {
            Engine.Boot();
            if (!FixRole())
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
                    () => UltraDeath.PerformRetreat(C, 4, MaxDeathRetries, _deathRetries, "UltraAvatarTyndariusRetreat.sync")
                );

                Fight();
            }
        }
        finally
        {
            Bot.Events.ScriptStopping -= StopTauntEvent;
            _tauntCts.Cancel();
            _wipeCts.Cancel();
            Engine.DisableSkills();
            C.SetOptions(false);
        }
    }

    private bool StopTauntEvent(Exception? e)
    {
        _tauntCts.Cancel();
        return true;
    }

    private bool IsTaunter() => _roleClass == LeftOrbTaunter || _roleClass == TyndariusTaunter;

    /// <summary>
    /// Equips this account's class once (party layout, else the class sync) and fixes its
    /// role from the class it equipped. Retries after a wipe keep the role; nothing re-runs it.
    /// </summary>
    private bool FixRole()
    {
        _party = UltraPartyLayout.Read("UltraAvatarTyndarius");

        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);

        C.Logger("[UltraAvatarTyndarius-v3] Assigning role classes for army size 4.");
        string assigned = _party.EquipClass(Ultra, UltraClassesByRole, 4, "ultra_tyndarius_class-v3.sync");
        if (string.IsNullOrEmpty(assigned) || !_party.EnsureClass())
            return false;

        string? className = Bot.Player.CurrentClass?.Name;
        _roleClass = UltraClassesByRole.Select(r => r[0]).First(r => r.Equals(className, StringComparison.OrdinalIgnoreCase));
        C.Logger($"[UltraAvatarTyndarius-v3] Role fixed: {RoleName()} ({_roleClass})");

        Enh.ApplyTyndarius();
        return true;
    }

    private string RoleName() => _roleClass switch
    {
        RightOrbKiller => "RightOrbKiller",
        LeftOrbTaunter => "LeftOrbTaunter",
        TyndariusTaunter => "TyndariusTaunter",
        _ => "TyndariusAttacker"
    };

    private void Fight()
    {
        const string map = "ultratyndarius";
        const string boss = "Ultra Avatar Tyndarius";
        const string bossDefeatedTemp = "Ultra Avatar Tyndarius Defeated";

        const string waitSyncFile = "ultra_tyndarius.sync";
        const string fightTimeSyncFile = "UltraAvatarTyndariusFightTime.sync";
        const string completionSyncFile = "UltraAvatarTyndariusCompletion.sync";
        int armySize = 4;

        const int questId = 8245;

        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(fightTimeSyncFile));
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        // Potions are picked from the equipped class, so it has to be the role's class.
        bool skipThird = IsTaunter();
        _party.EnsureClass();
        Pots.EnsureRecommendedPotions(skipThird: skipThird);
        if (IsTaunter())
            Scrolls.GetScrollOfEnrage();
        _party.EnsureClass();

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        Pots.UseRecommendedPotions(skipThird: skipThird, ensureStock: false);

        if (IsTaunter())
        {
            C.Logger("[UltraAvatarTyndarius-v3] Taunter, equipping Scroll of Enrage.");
            Engine.EquipEnrage();
            if (!Bot.Inventory.IsEquipped("Scroll of Enrage"))
                C.Logger("[UltraAvatarTyndarius-v3] Scroll of Enrage is not equipped, this taunter cannot taunt.", "Warning");
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

        if (_roleClass == TyndariusTaunter)
        {
            DateTime fightStartTime = UltraAsync.SetFightTime(C, Ultra.ResolveSyncPath(fightTimeSyncFile));
            UltraAsync.StartTauntLoop(Bot, C, Engine, fightStartTime, 0, 1, pulseIntervalSec: TyndariusTauntIntervalSec, cancellationToken: _tauntCts.Token);
        }

        while (!Bot.ShouldExit && !_wipeCts.IsCancellationRequested)
        {
            if (!Bot.Player.Alive)
            {
                // Death is signaled by the background wipe monitor
                // Wait for respawn and keep fighting
                Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                continue;
            }

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains(bossDefeatedTemp, 1), completionSyncFile))
            {
                C.Logger("Ultra Avatar Tyndarius defeated. Finishing quest.");
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

            switch (_roleClass)
            {
                case RightOrbKiller:
                    AttackTarget(IsAlive(RightOrb) ? RightOrb : Tyndarius);
                    break;

                case LeftOrbTaunter:
                    // Orbs respawn: whenever the left orb is up, it is this role's.
                    if (IsAlive(LeftOrb))
                    {
                        AttackTarget(LeftOrb);
                        if (Bot.Player.Target?.MapID == LeftOrb)
                            Engine.Cast(5);
                    }
                    else
                        AttackTarget(Tyndarius);
                    break;

                case TyndariusTaunter:
                    // The taunt loop presses the scroll on whatever is targeted, so stay on Tyndarius.
                    AttackTarget(Tyndarius);
                    break;

                default:
                    AttackTarget(Tyndarius);
                    break;
            }

            // No-op for the taunters: their consumable slot holds the scroll.
            Pots.ActivateEquippedPotion();

            Bot.Sleep(250);
        }

        // The taunt loop must not keep pressing during a retreat.
        _tauntCts.Cancel();

        // If retreat is still in progress (background), wait for it
        if (_wipeCts.IsCancellationRequested)
            _retreatComplete.WaitOne(TimeSpan.FromSeconds(120));
    }

    private static bool IsAlive(int mapId) =>
        Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.Alive && x.MapID == mapId);

    private static void AttackTarget(int mapId)
    {
        if (!Bot.Player.HasTarget || Bot.Player.Target == null || !Bot.Player.Target.Alive || Bot.Player.Target.MapID != mapId)
            Bot.Combat.Attack(mapId);
    }
}
