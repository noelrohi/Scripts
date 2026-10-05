/*
name: UltraEngineerv3
description: Ultra Engineer v3 — runs the Comp picked by the DoAllUltras "Ultra Engineer comp" option. Everyone kills the Defense Drone, then the Attack Drone, then Engineer. default: Verus DoomKnight, StoneCrusher, Lord of Order, King's Echo; loo-lr-sc-csh: Lord of Order first casts its 5th skill on each new Attack Drone.
tags: null
*/
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPotions.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraCustomClassSync.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPartyLayout.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraWaitForArmy.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/GetScrolls.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraComp.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraAttempt.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs

using System;
using System.IO;
using System.Linq;
using Skua.Core.Interfaces;

public class UltraEngineerv3
{
    // The DoAllUltras boss key: the Comp and layout options are UltraEngineerComp and UltraEngineerLayout.
    public const string Boss = "UltraEngineer";

    // Roles: what each class of the chosen Comp does in the fight. Engineer can't be hit while a Drone is up.
    private const string DroneKiller = "DroneKiller";                 // the Defense Drone, then the Attack Drone, then Engineer
    private const string AttackDroneDebuffer = "AttackDroneDebuffer"; // casts its 5th skill (Cast(4)) on each new Attack Drone, then plays DroneKiller

    /// <summary>Engineer's Comps. The DoAllUltras "Ultra Engineer comp" option picks one; blank runs default.</summary>
    public static readonly UltraComp[] Comps =
    {
        // The four DPS classes and the class presets the script used before Comps.
        new(UltraComp.Default,
            new UltraCompEntry
            {
                Class = "Verus DoomKnight",
                Role = DroneKiller,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Lacerate },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Forge },
                    Potions = new[] { "Body Tonic", "Potent Destruction Elixir", UltraPotions.HonorOrMalice },
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "StoneCrusher",
                Role = DroneKiller,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Valiance },
                    Cape = new[] { CapeSpecial.Absolution },
                    Helm = new[] { HelmSpecial.Anima },
                    Potions = new[] { "Body Tonic", "Unstable Divine Elixir", UltraPotions.HonorOrMalice },
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "Lord of Order",
                Role = DroneKiller,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Awe_Blast },
                    Cape = new[] { CapeSpecial.Absolution },
                    Potions = new[] { "Body Tonic", "Unstable Divine Elixir", UltraPotions.HonorOrMalice },
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "King's Echo",
                Role = DroneKiller,
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

        // The Recommended Group of the community "Simplified bosses guide", without Forge helms.
        new("loo-lr-sc-csh",
            new UltraCompEntry
            {
                Class = "Lord of Order",
                Role = AttackDroneDebuffer,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Lucky,
                    Weapon = new[] { WeaponSpecial.Awe_Blast },
                    Cape = new[] { CapeSpecial.Penitence },
                    Helm = new[] { HelmSpecial.Examen },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "Legion Revenant",
                Role = DroneKiller,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Wizard,
                    Weapon = new[] { WeaponSpecial.Ravenous },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Pneuma },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "StoneCrusher",
                Role = DroneKiller,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Lacerate },
                    Cape = new[] { CapeSpecial.Absolution },
                    Helm = new[] { HelmSpecial.Anima },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "Chrono ShadowHunter",
                Role = DroneKiller,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Lucky,
                    Weapon = new[] { WeaponSpecial.Valiance },
                    Cape = new[] { CapeSpecial.Vainglory },
                    Helm = new[] { HelmSpecial.Examen },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            }),
    };

    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;
    private static CoreEnginev3 Engine => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;
    private static string _fbsMuteFile = "";

    // Monster MapIDs in ultraengineer.
    private const int AttackDrone = 1;
    private const int DefenseDrone = 2;
    private const int Engineer = 3;

    // The debuffer's skill, counted from 0 as Cast counts them (0 is the auto attack): the guide's
    // skill 5, Lord of Order's Order.
    private const int DebuffSkill = 4;
    private const int DebuffTryMs = 3000; // how long to keep trying it on a new Attack Drone

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
        _fbsMuteFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Skua", "fbs_mute.sync"
        );
        try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }
        Engine.Boot();

        Bot.Events.ScriptStopping += StopAttemptEvent;
        try
        {
            if (Prep())
                Fight();
        }
        finally
        {
            // An Attempt still open here ended without a kill. Engineer has no Wipe detection.
            _attempt?.End(UltraAttempt.Outcome.Stopped);
            Bot.Events.ScriptStopping -= StopAttemptEvent;
            try { if (File.Exists(_fbsMuteFile)) File.Delete(_fbsMuteFile); } catch { }
            Engine.DisableSkills();
            C.SetOptions(false);
        }
    }

    private bool StopAttemptEvent(Exception? e)
    {
        _attempt?.End(UltraAttempt.Outcome.Stopped);
        return true;
    }

    /// <summary>Picks the Comp, equips this account's class from it and enhances it as its Loadout says.</summary>
    private bool Prep()
    {
        if (UltraComp.Prep(Boss, Comps, "UltraEngineer-v3", Ultra, 4, "ultra_engineer_class-v3.sync") is not { } prep)
            return false;
        (_comp, _party, _entry) = prep;
        return true;
    }

    private void Fight()
    {
        const string map = "ultraengineer";
        const string boss = "Ultra Engineer";
        const string bossDefeatedTemp = "Ultra Engineer Defeated";
        const string priority1 = "Defense Drone";
        const string priority2 = "Attack Drone";

        const string waitSyncFile = "ultra_engineer.sync";
        const string completionSyncFile = "UltraEngineerCompletion.sync";
        int armySize = 4;

        const int questId = 8154;

        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        UltraComp.ReadyForAttempt(_party, _entry, armySize, waitSyncFile, map);
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        Engine.ChooseBestCell(boss);
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

        UltraAttempt attempt = _attempt = UltraAttempt.Begin(Boss, _comp.Name, _entry.Class, _entry.Role, map, m => m.MapID == Engineer);

        bool attackDroneDebuffed = false;

        while (!Bot.ShouldExit)
        {
            // Refresh mute file so FBS plugin stays muted during the fight
            try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }

            if (!Bot.Player.Alive)
            {
                // The boss's HP is still noted while dead.
                Bot.Wait.ForTrue(() => Bot.Player.Alive, attempt.SeeBoss, 20);
                continue;
            }

            attempt.SeeBoss();

            if (Bot.TempInv.Contains(bossDefeatedTemp, 1))
                attempt.End(UltraAttempt.Outcome.Kill);

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains(bossDefeatedTemp, 1), completionSyncFile))
            {
                C.Logger("Ultra Engineer defeated. Finishing quest.");
                Engine.DisableSkills();
                Engine.Join(map);
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                break;
            }

            if (_entry.Role == AttackDroneDebuffer)
            {
                // Once on each new Attack Drone, then on with the drones and Engineer.
                bool attackDroneUp = IsAlive(AttackDrone);
                if (!attackDroneUp)
                    attackDroneDebuffed = false;
                else if (!attackDroneDebuffed)
                {
                    DebuffAttackDrone();
                    attackDroneDebuffed = true;
                }
            }

            Ultra.KillWithPriority(boss, Engineer, priority1, DefenseDrone, priority2, AttackDrone);
            _entry.Loadout.ActivatePotion();
            Bot.Sleep(500);
        }
    }

    /// <summary>Casts Order on the Attack Drone as soon as it is ready, for up to <see cref="DebuffTryMs"/>.</summary>
    private static void DebuffAttackDrone()
    {
        DateTime giveUp = DateTime.UtcNow.AddMilliseconds(DebuffTryMs);
        while (DateTime.UtcNow < giveUp && !Bot.ShouldExit && Bot.Player.Alive && IsAlive(AttackDrone))
        {
            if (Bot.Player.Target?.MapID != AttackDrone)
                Bot.Combat.Attack(AttackDrone);
            if (Bot.Player.Target?.MapID == AttackDrone && Engine.Cast(DebuffSkill))
            {
                C.Logger("[Engineer] AttackDroneDebuffer cast Order on the Attack Drone.");
                return;
            }
            Bot.Sleep(100);
        }
        C.Logger($"[Engineer] AttackDroneDebuffer could not cast Order on the Attack Drone within {DebuffTryMs / 1000} s.", "Warning");
    }

    private static bool IsAlive(int mapId) =>
        Bot.Monsters.MapMonsters.Any(m => m != null && m.MapID == mapId && m.HP > 0);
}
