/*
name: UltraNulgathv3
description: Ultra Nulgath v3 — runs the Comp picked by the DoAllUltras "Ultra Nulgath comp" option. default: 3 taunters + 1 Blade attacker; dot-lr-ap-loo: the Dragon of Time hits each new Overfiend Blade then Nulgath while Legion Revenant and ArchPaladin taunt Nulgath on a 10s cycle.
tags: null
*/
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPotions.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraCustomClassSync.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraWaitForArmy.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/GetScrolls.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraAsync.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraDeath.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPartyLayout.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraComp.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraAttempt.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs

using System;
using System.IO;
using System.Linq;
using System.Threading;
using Skua.Core.Interfaces;

public class UltraNulgathv3
{
    // The DoAllUltras boss key: the Comp and layout options are UltraNulgathComp and UltraNulgathLayout.
    public const string Boss = "UltraNulgath";

    // Roles: what each class of the chosen Comp does in the fight.
    private const string BladeHitter = "BladeHitter";         // hits each new Overfiend Blade for about 1.5 s, otherwise Nulgath
    private const string NulgathTaunter = "NulgathTaunter";   // stays on Nulgath, so its taunts land on Nulgath
    private const string NulgathAttacker = "NulgathAttacker"; // stays on Nulgath
    private const string BladeTaunter = "BladeTaunter";       // hits the Blade, and Nulgath around its own taunt
    private const string BladeAttacker = "BladeAttacker";     // hits the Blade while it is up, otherwise Nulgath

    private const string ScrollOfEnrage = "Scroll of Enrage";

    /// <summary>Nulgath's Comps. The DoAllUltras "Ultra Nulgath comp" option picks one; blank runs default.</summary>
    public static readonly UltraComp[] Comps =
    {
        // Three taunters 5 s apart on a 15 s cycle, one of them also on the Blade, and a Blade attacker.
        // It wiped repeatedly in testing; prefer dot-lr-ap-loo.
        new(UltraComp.Default,
            new UltraCompEntry
            {
                Class = "Lord of Order",
                Role = NulgathTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Valiance },
                    Cape = new[] { CapeSpecial.Absolution },
                    Potions = new[] { "Body Tonic", "Unstable Divine Elixir" },
                    Scroll = ScrollOfEnrage,
                },
                Taunt = UltraTaunt.Every(15, atSec: 0, skipWhileAura: "Contract of Despair"),
            },
            new UltraCompEntry
            {
                Class = "StoneCrusher",
                Role = NulgathTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Valiance },
                    Cape = new[] { CapeSpecial.Absolution },
                    Potions = new[] { "Body Tonic", "Unstable Divine Elixir" },
                    Scroll = ScrollOfEnrage,
                },
                Taunt = UltraTaunt.Every(15, atSec: 5, skipWhileAura: "Contract of Despair"),
            },
            new UltraCompEntry
            {
                Class = "Verus DoomKnight",
                Role = BladeTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Lacerate },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Forge },
                    Potions = new[] { "Body Tonic", "Potent Destruction Elixir" },
                    Scroll = ScrollOfEnrage,
                },
                Taunt = UltraTaunt.Every(15, atSec: 10, skipWhileAura: "Contract of Despair"),
            },
            new UltraCompEntry
            {
                Class = "King's Echo",
                Role = BladeAttacker,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Healer,
                    Weapon = new[] { WeaponSpecial.Elysium, WeaponSpecial.Mana_Vamp },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Examen },
                    Potions = new[] { "Body Tonic", "Potent Destruction Elixir", UltraPotions.HonorOrMalice },
                },
                Taunt = UltraTaunt.Never,
            }),

        // Beat Nulgath in about 2.5 minutes with no deaths. Only the Dragon of Time is enhanced
        // and drinks; the others keep their own gear.
        new("dot-lr-ap-loo",
            new UltraCompEntry
            {
                Class = "Dragon of Time",
                Role = BladeHitter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Wizard,
                    Weapon = new[] { WeaponSpecial.Elysium },
                    Cape = new[] { CapeSpecial.Vainglory },
                    Helm = new[] { HelmSpecial.Pneuma },
                    EnhanceWhenAutoEnhanceIsOff = true,
                    // The potion buyer can't make Unstable Malevolence Elixir: keep some on hand.
                    Potions = new[] { "Unstable Malevolence Elixir", "Sage Tonic", "Potent Honor Potion" },
                    // Drunk before the fight only, as when this Comp beat Nulgath.
                    ClickPotionInFight = false,
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "Legion Revenant",
                Role = NulgathTaunter,
                Loadout = new UltraLoadout { Scroll = ScrollOfEnrage },
                Taunt = UltraTaunt.Every(10, atSec: 0),
            },
            new UltraCompEntry
            {
                Class = "ArchPaladin",
                Role = NulgathTaunter,
                Loadout = new UltraLoadout { Scroll = ScrollOfEnrage },
                Taunt = UltraTaunt.Every(10, atSec: 5),
            },
            new UltraCompEntry
            {
                Class = "Lord of Order",
                Role = NulgathAttacker,
                Taunt = UltraTaunt.Never,
            }),
    };

    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;
    private static CoreEnginev3 Engine => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;
    private static string _fbsMuteFile = "";

    // Monster MapIDs in ultranulgath.
    private const int Blade = 1;
    private const int Nulgath = 2;

    // How long the Blade hitter stays on each new Blade before going back to Nulgath.
    private const int BladeHitMs = 1500;

    private CancellationTokenSource _tauntCts = new();
    private CancellationTokenSource _wipeCts = new();
    private ManualResetEvent _retreatComplete = new(false);
    private UltraDeath.RetryCounter _deathRetries = new();
    private const int MaxDeathRetries = 10;
    private UltraPartyLayout _party = null!;
    private UltraComp _comp = null!;
    private UltraCompEntry _entry = null!;
    private UltraAttempt? _attempt;
    private int _tickMs = 500;

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
            // An Attempt still open here ended without a kill or a Wipe.
            _attempt?.End(UltraAttempt.Outcome.Stopped);
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
        _attempt?.End(UltraAttempt.Outcome.Stopped);
        _tauntCts.Cancel();
        return true;
    }

    /// <summary>
    /// Picks the Comp, equips this account's class from it once and enhances it as its Loadout says.
    /// The Attempts after a Wipe keep the class and Role.
    /// </summary>
    private bool Prep()
    {
        UltraComp? comp = UltraComp.Read(Boss, Comps);
        if (comp == null)
            return false;
        _comp = comp;
        _party = UltraPartyLayout.Read(Boss);

        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);

        C.Logger($"[UltraNulgath-v3] Equipping the {_comp.Name} Comp's classes for army size 4.");
        UltraCompEntry? entry = _comp.EquipClass(_party, Ultra, 4, "ultra_nulgath_class-v3.sync");
        if (entry == null)
            return false;
        _entry = entry;

        // A Blade hitter needs a fast loop to catch each new Blade; the whole party ticks with it,
        // as in the Attempt that beat Nulgath.
        _tickMs = _comp.Entries.Any(e => e.Role == BladeHitter) ? 100 : 500;

        _entry.Loadout.Enhance();
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

        // Buying potion reagents can swap to a farm class; the Comp's class goes back on.
        _party.EnsureClass();
        _entry.Loadout.Stock();
        _party.EnsureClass();

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        _entry.Loadout.Use();

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

        UltraAttempt attempt = _attempt = UltraAttempt.Begin(Boss, _comp.Name, _entry.Class, _entry.Role, map, m => m.MapID == Nulgath);
        // The wipe monitor cancels this token the moment the whole party is dead.
        using CancellationTokenRegistration onWipe = _wipeCts.Token.Register(() => attempt.End(UltraAttempt.Outcome.Wipe));

        DateTime fightStart = _comp.StartTaunts(_entry, Ultra, Ultra.ResolveSyncPath(fightTimeSyncFile), _tauntCts.Token);

        bool bladeHit = false;

        while (!Bot.ShouldExit && !_wipeCts.IsCancellationRequested)
        {
            // Refresh mute file so FBS plugin stays muted during the fight
            try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }

            if (!Bot.Player.Alive)
            {
                // Death is signaled by the background wipe monitor. The boss's HP is still noted while dead.
                Bot.Wait.ForTrue(() => Bot.Player.Alive || _wipeCts.IsCancellationRequested, attempt.SeeBoss, 20);
                continue;
            }

            attempt.SeeBoss();

            if (Bot.TempInv.Contains(bossDefeatedTemp, 1))
                attempt.End(UltraAttempt.Outcome.Kill);

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

            switch (_entry.Role)
            {
                case BladeHitter:
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
                    else
                        AttackNulgath();
                    break;

                case BladeTaunter:
                    // On Nulgath while its taunts are skipped and around its own taunt, otherwise the Blade.
                    if (SkippingTaunts() || AroundOwnTaunt(fightStart))
                        AttackNulgath();
                    else
                        AttackBladeOrNulgath();
                    break;

                case BladeAttacker:
                    AttackBladeOrNulgath();
                    break;

                default:
                    // NulgathTaunter, NulgathAttacker: the taunt loop presses the scroll on whatever is targeted.
                    AttackNulgath();
                    break;
            }

            _entry.Loadout.ActivatePotion();

            Bot.Sleep(_tickMs);
        }

        // The taunt loop must not keep pressing during a retreat.
        _tauntCts.Cancel();

        // If retreat is still in progress (background), wait for it
        if (_wipeCts.IsCancellationRequested)
            _retreatComplete.WaitOne(TimeSpan.FromSeconds(120));
    }

    private static void AttackNulgath()
    {
        if (Bot.Player.Target?.MapID != Nulgath)
            Bot.Combat.Attack(Nulgath);
    }

    private static void AttackBladeOrNulgath()
    {
        if (Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.MapID == Blade && x.HP > 0))
        {
            if (Bot.Player.Target?.MapID != Blade)
                Bot.Combat.Attack(Blade);
        }
        else
            AttackNulgath();
    }

    private bool SkippingTaunts() =>
        _entry.Taunt.SkipWhileAura is string aura && Engine.HasAura(aura, true);

    /// <summary>From 1 s before this class's taunt until its ~3 s of presses end.</summary>
    private bool AroundOwnTaunt(DateTime fightStart)
    {
        UltraTaunt taunt = _entry.Taunt;
        if (!taunt.IsTimed)
            return false;

        double sinceTaunt = ((DateTime.UtcNow - fightStart).TotalSeconds - taunt.AtSec) % taunt.CycleSec;
        if (sinceTaunt < 0)
            sinceTaunt += taunt.CycleSec;
        return sinceTaunt >= taunt.CycleSec - 1 || sinceTaunt <= 3;
    }
}
