/*
name: UltraEzrajalv3
description: Ultra Ezrajal v3 — runs the Comp picked by the DoAllUltras "Ultra Ezrajal comp" option. Everyone hits Ezrajal and stops attacking during Counter Attack. default: Verus DoomKnight, StoneCrusher, Lord of Order, King's Echo; loo-lr-ap-csh: Lord of Order, Legion Revenant, ArchPaladin, Chrono ShadowHunter.
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

public class UltraEzrajalv3
{
    // The DoAllUltras boss key: the Comp and layout options are UltraEzrajalComp and UltraEzrajalLayout.
    public const string Boss = "UltraEzrajal";

    // Roles: what each class of the chosen Comp does in the fight.
    private const string EzrajalAttacker = "EzrajalAttacker"; // hits Ezrajal, and stops attacking during his Counter Attack

    /// <summary>Ezrajal's Comps. The DoAllUltras "Ultra Ezrajal comp" option picks one; blank runs default.</summary>
    public static readonly UltraComp[] Comps =
    {
        // The four DPS classes and the class presets the script used before Comps.
        new(UltraComp.Default,
            new UltraCompEntry
            {
                Class = "Verus DoomKnight",
                Role = EzrajalAttacker,
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
                Role = EzrajalAttacker,
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
                Role = EzrajalAttacker,
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
                Role = EzrajalAttacker,
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

        // The Recommended Group of the community "Simplified bosses guide" (minimum 3175 HP).
        // Its Lock trick, swapping off a Mana Vamp weapon once Ezrajal locks it, is not played:
        // everyone fights on the weapon below.
        new("loo-lr-ap-csh",
            new UltraCompEntry
            {
                Class = "Lord of Order",
                Role = EzrajalAttacker,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Lucky,
                    Weapon = new[] { WeaponSpecial.Awe_Blast },
                    Cape = new[] { CapeSpecial.Penitence },
                    Helm = new[] { HelmSpecial.Forge },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "Legion Revenant",
                Role = EzrajalAttacker,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Wizard,
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.None },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "ArchPaladin",
                Role = EzrajalAttacker,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Lucky,
                    Weapon = new[] { WeaponSpecial.Praxis },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Forge },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "Chrono ShadowHunter",
                Role = EzrajalAttacker,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Lucky,
                    Weapon = new[] { WeaponSpecial.Valiance },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Forge },
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

    // Ezrajal's MapID in ultraezrajal.
    private const int Ezrajal = 1;

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

        Bot.UltraBossHelper.EnableCounterAttack();

        Bot.Events.ScriptStopping += StopAttemptEvent;
        try
        {
            if (Prep())
                Fight();
        }
        finally
        {
            // An Attempt still open here ended without a kill. Ezrajal has no Wipe detection.
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
        if (UltraComp.Prep(Boss, Comps, "UltraEzrajal-v3", Ultra, 4, "ultra_ezrajal_class-v3.sync") is not { } prep)
            return false;
        (_comp, _party, _entry) = prep;
        return true;
    }

    private void Fight()
    {
        const string map = "ultraezrajal";
        const string boss = "Ultra Ezrajal";
        const string bossDefeatedTemp = "Ultra Ezrajal Defeated";

        const string waitSyncFile = "ultra_ezrajal.sync";
        const string completionSyncFile = "UltraEzrajalCompletion.sync";
        int armySize = 4;

        const int questId = 8152;

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

        UltraAttempt attempt = _attempt = UltraAttempt.Begin(Boss, _comp.Name, _entry.Class, _entry.Role, map, m => m.MapID == Ezrajal);

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
                C.Logger("Ultra Ezrajal defeated. Finishing quest.");
                Engine.DisableSkills();
                Engine.Join(map);
                Bot.UltraBossHelper.DisableCounterAttack();
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                break;
            }

            // EzrajalAttacker, the only Role: stop attacking while Ezrajal reflects.
            if (Bot.Player.HasTarget
                && Bot.Target?.Auras?.Any(a => a != null && a.Name == "Counter Attack") == true)
            {
                Bot.Combat.CancelAutoAttack();
                Bot.Sleep(6300);
            }
            else
            {
                Bot.Combat.Attack(boss);
            }

            _entry.Loadout.ActivatePotion();
            Bot.Sleep(500);
        }
    }
}
