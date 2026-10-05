/*
name: UltraAvatarTyndariusv3
description: Ultra Avatar Tyndarius v3 — runs the Comp picked by the DoAllUltras "Ultra Avatar Tyndarius comp" option. default: King's Echo kills the right orb, Legion Revenant taunts the left orb, ArchPaladin taunts Tyndarius every 12s, Lord of Order hits Tyndarius. ke-lr-ap-loo-loop: default without potions, and Lord of Order also taunts Tyndarius 6 s after each ArchPaladin taunt. ke-lr-ap-loo-orbs: ke-lr-ap-loo-loop, and King's Echo taunts the right orb. ke-lr-ap-loo-orbpairs: two taunters per orb, 6 s apart, and nobody taunts Tyndarius. ke-lr-ap-loo-burst: ke-lr-ap-loo-orbs with Lord of Order on the right orb, turning to Tyndarius only to taunt.
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
using System.Linq;
using System.Threading;
using Skua.Core.Interfaces;

public class UltraAvatarTyndariusv3
{
    // The DoAllUltras boss key: the Comp and layout options are UltraAvatarTyndariusComp and UltraAvatarTyndariusLayout.
    public const string Boss = "UltraAvatarTyndarius";

    // Roles: what each class of the chosen Comp does in the fight.
    private const string RightOrbKiller = "RightOrbKiller";       // hits the right orb while it is up, otherwise Tyndarius
    private const string LeftOrbTaunter = "LeftOrbTaunter";       // taunts the left orb while it is up, otherwise hits Tyndarius
    private const string RightOrbTaunter = "RightOrbTaunter";     // taunts and kills the right orb, then the left orb, then Tyndarius
    private const string LeftOrbHitter = "LeftOrbHitter";         // the left orb, then the right orb, then Tyndarius; its timed taunts land on its target
    private const string RightOrbHitter = "RightOrbHitter";       // the right orb, then the left orb, then Tyndarius; its timed taunts land on its target
    private const string RightOrbBurster = "RightOrbBurster";     // the right orb, then the left orb, then Tyndarius, but on Tyndarius while it taunts him
    private const string TyndariusTaunter = "TyndariusTaunter";   // stays on Tyndarius, so its taunts land on Tyndarius
    private const string TyndariusAttacker = "TyndariusAttacker"; // stays on Tyndarius

    /// <summary>Tyndarius's Comps. The DoAllUltras "Ultra Avatar Tyndarius comp" option picks one; blank runs default.</summary>
    public static readonly UltraComp[] Comps =
    {
        // Won its first Attempt with a layout. The ArchPaladin taunted every 12 s in the v2 layout that beat the boss.
        new(UltraComp.Default,
            new UltraCompEntry
            {
                Class = "King's Echo",
                Role = RightOrbKiller,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Healer,
                    Weapon = new[] { WeaponSpecial.Elysium, WeaponSpecial.Mana_Vamp },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Examen },
                    Potions = new[] { "Body Tonic", "Potent Destruction Elixir", UltraPotions.HonorOrMalice },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "Legion Revenant",
                Role = LeftOrbTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Wizard,
                    // Health Vamp without Arcana's Concerto: the left orb kept killing it.
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Health_Vamp },
                    Cape = new[] { CapeSpecial.Penitence, CapeSpecial.Vainglory },
                    Helm = new[] { HelmSpecial.Pneuma, HelmSpecial.None },
                    // Body Tonic for the HP: it dies at its base HP taunting the left orb.
                    Scroll = UltraLoadout.ScrollOfEnrage,
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.ByRole("whenever the left orb is up"),
            },
            new UltraCompEntry
            {
                Class = "ArchPaladin",
                Role = TyndariusTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Valiance },
                    Cape = new[] { CapeSpecial.Absolution },
                    Helm = new[] { HelmSpecial.Forge },
                    Scroll = UltraLoadout.ScrollOfEnrage,
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Every(12, atSec: 0, beforeTaunt: SealUp, beforeTauntDescription: "Righteous Seal up"),
            },
            new UltraCompEntry
            {
                Class = "Lord of Order",
                Role = TyndariusAttacker,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Awe_Blast },
                    Cape = new[] { CapeSpecial.Absolution },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            }),
        // default with the guide's loop taunt: a taunt's Focus holds Tyndarius for 6 s, so the Lord of Order
        // taunts at 6 s of the ArchPaladin's 12 s cycle and Tyndarius is never left untaunted. No potions.
        new("ke-lr-ap-loo-loop",
            new UltraCompEntry
            {
                Class = "King's Echo",
                Role = RightOrbKiller,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Healer,
                    Weapon = new[] { WeaponSpecial.Elysium, WeaponSpecial.Mana_Vamp },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Examen },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "Legion Revenant",
                Role = LeftOrbTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Wizard,
                    // Health Vamp without Arcana's Concerto: the left orb kept killing it.
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Health_Vamp },
                    Cape = new[] { CapeSpecial.Penitence, CapeSpecial.Vainglory },
                    Helm = new[] { HelmSpecial.Pneuma, HelmSpecial.None },
                    Scroll = UltraLoadout.ScrollOfEnrage,
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.ByRole("whenever the left orb is up"),
            },
            new UltraCompEntry
            {
                Class = "ArchPaladin",
                Role = TyndariusTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Valiance },
                    Cape = new[] { CapeSpecial.Absolution },
                    Helm = new[] { HelmSpecial.Forge },
                    Scroll = UltraLoadout.ScrollOfEnrage,
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Every(12, atSec: 0, beforeTaunt: SealUp, beforeTauntDescription: "Righteous Seal up"),
            },
            new UltraCompEntry
            {
                Class = "Lord of Order",
                Role = TyndariusAttacker,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Awe_Blast },
                    Cape = new[] { CapeSpecial.Absolution },
                    EnhanceWhenAutoEnhanceIsOff = true,
                    Scroll = UltraLoadout.ScrollOfEnrage,
                },
                Taunt = UltraTaunt.Every(12, atSec: 6),
            }),
        // ke-lr-ap-loo-loop with the guide's orb taunts: both orbs put Melting on the whole party every 6 s and
        // it stacks, so King's Echo taunts the right orb as Legion Revenant taunts the left, and kills it first.
        new("ke-lr-ap-loo-orbs",
            new UltraCompEntry
            {
                Class = "King's Echo",
                Role = RightOrbTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Healer,
                    Weapon = new[] { WeaponSpecial.Elysium, WeaponSpecial.Mana_Vamp },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Examen },
                    EnhanceWhenAutoEnhanceIsOff = true,
                    Scroll = UltraLoadout.ScrollOfEnrage,
                },
                Taunt = UltraTaunt.ByRole("whenever the right orb is up"),
            },
            new UltraCompEntry
            {
                Class = "Legion Revenant",
                Role = LeftOrbTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Wizard,
                    // Health Vamp without Arcana's Concerto: the left orb kept killing it.
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Health_Vamp },
                    Cape = new[] { CapeSpecial.Penitence, CapeSpecial.Vainglory },
                    Helm = new[] { HelmSpecial.Pneuma, HelmSpecial.None },
                    Scroll = UltraLoadout.ScrollOfEnrage,
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.ByRole("whenever the left orb is up"),
            },
            new UltraCompEntry
            {
                Class = "ArchPaladin",
                Role = TyndariusTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Valiance },
                    Cape = new[] { CapeSpecial.Absolution },
                    Helm = new[] { HelmSpecial.Forge },
                    Scroll = UltraLoadout.ScrollOfEnrage,
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Every(12, atSec: 0, beforeTaunt: SealUp, beforeTauntDescription: "Righteous Seal up"),
            },
            new UltraCompEntry
            {
                Class = "Lord of Order",
                Role = TyndariusAttacker,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Awe_Blast },
                    Cape = new[] { CapeSpecial.Absolution },
                    EnhanceWhenAutoEnhanceIsOff = true,
                    Scroll = UltraLoadout.ScrollOfEnrage,
                },
                Taunt = UltraTaunt.Every(12, atSec: 6),
            }),
        // ke-lr-ap-loo-orbs with the Lord of Order on the right orb: it turns to Tyndarius only for its loop taunt,
        // so the right orb dies before its second Melting, which would hit the whole party.
        new("ke-lr-ap-loo-burst",
            new UltraCompEntry
            {
                Class = "King's Echo",
                Role = RightOrbTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Healer,
                    Weapon = new[] { WeaponSpecial.Elysium, WeaponSpecial.Mana_Vamp },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Examen },
                    EnhanceWhenAutoEnhanceIsOff = true,
                    Scroll = UltraLoadout.ScrollOfEnrage,
                },
                Taunt = UltraTaunt.ByRole("whenever the right orb is up"),
            },
            new UltraCompEntry
            {
                Class = "Legion Revenant",
                Role = LeftOrbTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Wizard,
                    // Health Vamp without Arcana's Concerto: the left orb kept killing it.
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Health_Vamp },
                    Cape = new[] { CapeSpecial.Penitence, CapeSpecial.Vainglory },
                    Helm = new[] { HelmSpecial.Pneuma, HelmSpecial.None },
                    Scroll = UltraLoadout.ScrollOfEnrage,
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.ByRole("whenever the left orb is up"),
            },
            new UltraCompEntry
            {
                Class = "ArchPaladin",
                Role = TyndariusTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Valiance },
                    Cape = new[] { CapeSpecial.Absolution },
                    Helm = new[] { HelmSpecial.Forge },
                    Scroll = UltraLoadout.ScrollOfEnrage,
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Every(12, atSec: 0, beforeTaunt: SealUp, beforeTauntDescription: "Righteous Seal up"),
            },
            new UltraCompEntry
            {
                Class = "Lord of Order",
                Role = RightOrbBurster,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Awe_Blast },
                    Cape = new[] { CapeSpecial.Absolution },
                    EnhanceWhenAutoEnhanceIsOff = true,
                    Scroll = UltraLoadout.ScrollOfEnrage,
                },
                Taunt = UltraTaunt.Every(12, atSec: 6, beforeTaunt: OnTyndariusForTaunt, beforeTauntDescription: "turned to Tyndarius"),
            }),
        // Each orb puts Melting on the whole party every ~6 s unless it's taunted, when only its taunter gets it.
        // A taunt lasts 6 s and a scroll takes ~12 s to come back, so two taunters share each orb, 6 s apart,
        // and hit it. Nobody taunts Tyndarius: the guide calls that optional when the orbs die fast.
        new("ke-lr-ap-loo-orbpairs",
            new UltraCompEntry
            {
                Class = "King's Echo",
                Role = RightOrbHitter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Healer,
                    Weapon = new[] { WeaponSpecial.Elysium, WeaponSpecial.Mana_Vamp },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Examen },
                    EnhanceWhenAutoEnhanceIsOff = true,
                    Scroll = UltraLoadout.ScrollOfEnrage,
                },
                Taunt = UltraTaunt.Every(12, atSec: 0),
            },
            new UltraCompEntry
            {
                Class = "Legion Revenant",
                Role = LeftOrbHitter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Wizard,
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Health_Vamp },
                    Cape = new[] { CapeSpecial.Penitence, CapeSpecial.Vainglory },
                    Helm = new[] { HelmSpecial.Pneuma, HelmSpecial.None },
                    Scroll = UltraLoadout.ScrollOfEnrage,
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Every(12, atSec: 0),
            },
            new UltraCompEntry
            {
                Class = "ArchPaladin",
                Role = LeftOrbHitter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Valiance },
                    Cape = new[] { CapeSpecial.Absolution },
                    Helm = new[] { HelmSpecial.Forge },
                    Scroll = UltraLoadout.ScrollOfEnrage,
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Every(12, atSec: 6, beforeTaunt: SealUp, beforeTauntDescription: "Righteous Seal up"),
            },
            new UltraCompEntry
            {
                Class = "Lord of Order",
                Role = RightOrbHitter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Awe_Blast },
                    Cape = new[] { CapeSpecial.Absolution },
                    EnhanceWhenAutoEnhanceIsOff = true,
                    Scroll = UltraLoadout.ScrollOfEnrage,
                },
                Taunt = UltraTaunt.Every(12, atSec: 6),
            }),
    };

    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;
    private static CoreEnginev3 Engine => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;

    // Monster MapIDs in ultratyndarius, cell Boss.
    private const int LeftOrb = 1;
    private const int Tyndarius = 2;
    private const int RightOrb = 3;

    private CancellationTokenSource _tauntCts = new();
    private CancellationTokenSource _wipeCts = new();
    private ManualResetEvent _retreatComplete = new(false);
    private UltraDeath.RetryCounter _deathRetries = new();
    private const int MaxDeathRetries = 10;
    private UltraPartyLayout _party = null!;
    private UltraComp _comp = null!;
    private UltraCompEntry _entry = null!;
    private UltraAttempt? _attempt;

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
                    () => UltraDeath.PerformRetreat(C, 4, MaxDeathRetries, _deathRetries, "UltraAvatarTyndariusRetreat.sync")
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
            Engine.DisableSkills();
            Engine.ArchPaladinHoldsUltimate = false;
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
        if (UltraComp.Prep(Boss, Comps, "UltraAvatarTyndarius-v3", Ultra, 4, "ultra_tyndarius_class-v3.sync") is not { } prep)
            return false;
        (_comp, _party, _entry) = prep;
        return true;
    }

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

        UltraComp.ReadyForAttempt(_party, _entry, armySize, waitSyncFile, map);
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

        UltraAttempt attempt = _attempt = UltraAttempt.Begin(Boss, _comp.Name, _entry.Class, _entry.Role, map, m => m.MapID == Tyndarius);
        // The wipe monitor cancels this token the moment the whole party is dead.
        using CancellationTokenRegistration onWipe = _wipeCts.Token.Register(() => attempt.End(UltraAttempt.Outcome.Wipe));

        // The ArchPaladin's ultimate breaks Righteous Seal ("Broken Seal" for 25 s), so it plays skills 1 to 3 only.
        Engine.ArchPaladinHoldsUltimate = _entry.Class == "ArchPaladin";
        _comp.StartTaunts(_entry, Ultra, Ultra.ResolveSyncPath(fightTimeSyncFile), _tauntCts.Token);

        while (!Bot.ShouldExit && !_wipeCts.IsCancellationRequested)
        {
            if (!Bot.Player.Alive)
            {
                // Death is signaled by the background wipe monitor
                // Wait for respawn and keep fighting, still noting the boss's HP
                Bot.Wait.ForTrue(() => Bot.Player.Alive, attempt.SeeBoss, 20);
                continue;
            }

            attempt.SeeBoss();

            if (Bot.TempInv.Contains(bossDefeatedTemp, 1))
                attempt.End(UltraAttempt.Outcome.Kill);

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

            switch (_entry.Role)
            {
                case RightOrbKiller:
                    AttackTarget(IsAlive(RightOrb) ? RightOrb : Tyndarius);
                    break;

                case RightOrbBurster:
                    if (DateTime.UtcNow < _onTyndariusUntil)
                        AttackTarget(Tyndarius);
                    else
                        AttackTarget(IsAlive(RightOrb) ? RightOrb : IsAlive(LeftOrb) ? LeftOrb : Tyndarius);
                    break;

                case LeftOrbHitter:
                    AttackTarget(IsAlive(LeftOrb) ? LeftOrb : IsAlive(RightOrb) ? RightOrb : Tyndarius);
                    break;

                case RightOrbHitter:
                    AttackTarget(IsAlive(RightOrb) ? RightOrb : IsAlive(LeftOrb) ? LeftOrb : Tyndarius);
                    break;

                case RightOrbTaunter:
                    // Each orb's Melting stacks on the whole party; a taunted orb's goes to its taunter.
                    if (IsAlive(RightOrb))
                    {
                        AttackTarget(RightOrb);
                        if (Bot.Player.Target?.MapID == RightOrb)
                            Engine.Cast(5);
                    }
                    else
                        AttackTarget(IsAlive(LeftOrb) ? LeftOrb : Tyndarius);
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
            _entry.Loadout.ActivatePotion();

            Bot.Sleep(250);
        }

        // The taunt loop must not keep pressing during a retreat.
        _tauntCts.Cancel();

        // If retreat is still in progress (background), wait for it
        if (_wipeCts.IsCancellationRequested)
            _retreatComplete.WaitOne(TimeSpan.FromSeconds(120));
    }

    // RightOrbBurster: until when it stays on Tyndarius, so its taunt's presses all land on him.
    private static DateTime _onTyndariusUntil = DateTime.MinValue;

    /// <summary>Turns to Tyndarius for the taunt's presses (about 3 s), waiting up to 1.5 s for him to be the target.</summary>
    private static void OnTyndariusForTaunt()
    {
        _onTyndariusUntil = DateTime.UtcNow.AddSeconds(3.5);
        Bot.Combat.Attack(Tyndarius);
        Bot.Wait.ForTrue(() => Bot.Player.Target?.MapID == Tyndarius, 6);
    }

    /// <summary>Puts Righteous Seal (ArchPaladin's skill 4, Cast(3)) up before a taunt, waiting up to 1.5 s for it.</summary>
    private static void SealUp()
    {
        if (Engine.HasAura("Righteous Seal"))
            return;
        Engine.Cast(3);
        Bot.Wait.ForTrue(() => Engine.HasAura("Righteous Seal"), 6);
    }

    private static bool IsAlive(int mapId) =>
        Bot.Monsters.CurrentAvailableMonsters.Any(x => x != null && x.Alive && x.MapID == mapId);

    private static void AttackTarget(int mapId)
    {
        if (!Bot.Player.HasTarget || Bot.Player.Target == null || !Bot.Player.Target.Alive || Bot.Player.Target.MapID != mapId)
            Bot.Combat.Attack(mapId);
    }
}
