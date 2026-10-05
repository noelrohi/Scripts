/*
name: UltraEzrajalv3
description: Ultra Ezrajal v3 — runs the Comp picked by the DoAllUltras "Ultra Ezrajal comp" option. Everyone hits Ezrajal and stops attacking during Counter Attack. default: Verus DoomKnight, StoneCrusher, Lord of Order, King's Echo; loo-lr-ap-csh: Lord of Order, Legion Revenant, ArchPaladin, Chrono ShadowHunter; ke-lr-ap-loo: King's Echo, Legion Revenant, ArchPaladin, Lord of Order, who first get Ezrajal to lock a Mana Vamp bait weapon, then restart the fight together on their real weapons.
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
using Skua.Core.Models.Items;

public class UltraEzrajalv3
{
    // The DoAllUltras boss key: the Comp and layout options are UltraEzrajalComp and UltraEzrajalLayout.
    public const string Boss = "UltraEzrajal";

    // Roles: what each class of the chosen Comp does in the fight.
    private const string EzrajalAttacker = "EzrajalAttacker"; // hits Ezrajal, and stops attacking during his Counter Attack
    // Gets Ezrajal to lock a Mana Vamp bait weapon at the start (the guide's lock trick), then plays EzrajalAttacker.
    private const string LockBaiter = "LockBaiter";

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
        // Its Lock trick, swapping off a Mana Vamp weapon once Ezrajal locks it, is not played
        // here (ke-lr-ap-loo plays it): everyone fights on the weapon below.
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

        // The guide's lock trick, without Vainglory capes. Legion Revenant needs the guide's minimum HP.
        new("ke-lr-ap-loo",
            new UltraCompEntry
            {
                Class = "King's Echo",
                Role = LockBaiter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Lucky,
                    Weapon = new[] { WeaponSpecial.Ravenous },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Forge },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "Legion Revenant",
                Role = LockBaiter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Wizard,
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.None },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
                MinMaxHealth = 3175,
            },
            new UltraCompEntry
            {
                Class = "ArchPaladin",
                Role = LockBaiter,
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
                Class = "Lord of Order",
                Role = LockBaiter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Lucky,
                    Weapon = new[] { WeaponSpecial.Awe_Blast },
                    Cape = new[] { CapeSpecial.Penitence },
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

    // The lock trick. Ezrajal locks one thing of a player's for 45 s with a "Skill Locked" aura whose
    // val and msgOn name it, e.g. val "Mana Vampire", msgOn "@Mana Vampire has been locked!".
    private const string SkillLocked = "Skill Locked";
    private const string ManaVampLock = "Mana Vamp";
    private const int LockTimeoutSec = 20; // stop baiting after this, locked or not, so the lock still has most of its 45 s
    private const string LockSyncFile = "ultra_ezrajal_lock.sync";
    private static CoreAdvanced Adv => _Adv ??= new CoreAdvanced();
    private static CoreAdvanced _Adv;

    private UltraPartyLayout _party = null!;
    private UltraComp _comp = null!;
    private UltraCompEntry _entry = null!;
    private UltraAttempt? _attempt;
    // LockBaiter: the Loadout's weapon, and the Mana Vamp bait weapon (null: this account skips the trick).
    private string? _mainWeapon;
    private string? _baitWeapon;
    private volatile bool _baiting;
    private volatile bool _baitLocked;

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
            Bot.Events.ExtensionPacketReceived -= LockListener;
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
        if (_entry.Role == LockBaiter)
            PrepareBait();
        return true;
    }

    /// <summary>
    /// Readies a bait weapon without overwriting any weapon's enhancement: a spare weapon (not the
    /// Loadout's) already on Mana Vamp, used as it is; else a spare weapon with no enhancement, enhanced
    /// with the Loadout's enhancement and Mana Vamp. Without either, or when the unenhanced one can't get
    /// Mana Vamp (Awe enhancements locked, or the enhancing failed), this account skips the lock trick.
    /// </summary>
    private void PrepareBait()
    {
        string who = Bot.Player.Username;
        _mainWeapon = EquippedWeapon()?.Name;
        _baitWeapon = null;
        if (_mainWeapon == null)
        {
            C.Logger($"[Lock] {who} has no weapon equipped; it skips the lock trick.", "Warning");
            return;
        }

        InventoryItem? bait = PickBaitWeapon(_mainWeapon);
        if (bait == null)
        {
            C.Logger($"[Lock] {who} has no spare weapon on Mana Vamp and no unenhanced spare weapon in its inventory or bank, " +
                "and won't overwrite another weapon's enhancement; it skips the lock trick.", "Warning");
            return;
        }

        if (IsManaVamp(bait))
        {
            _baitWeapon = bait.Name;
            C.Logger($"[Lock] {who} baits Ezrajal's lock with {bait.Name}, already on Mana Vamp, then fights with {_mainWeapon}.");
            return;
        }

        if (!Adv.uAwe())
        {
            C.Logger($"[Lock] {who} hasn't unlocked Awe enhancements, so it can't put Mana Vamp on the unenhanced {bait.Name}; it skips the lock trick.", "Warning");
            return;
        }

        C.Equip(bait.Name);
        if (Bot.Inventory.IsEquipped(bait.Name))
            _entry.Loadout.EnhanceWeapon(WeaponSpecial.Mana_Vamp);
        bool manaVamp = EquippedWeapon() is { } w && w.Name == bait.Name && IsManaVamp(w);
        C.Equip(_mainWeapon);
        if (!manaVamp)
        {
            C.Logger($"[Lock] {who} couldn't put Mana Vamp on the unenhanced {bait.Name}; it skips the lock trick.", "Warning");
            return;
        }

        _baitWeapon = bait.Name;
        C.Logger($"[Lock] {who} put Mana Vamp on the unenhanced {bait.Name} and baits Ezrajal's lock with it, then fights with {_mainWeapon}.");
    }

    /// <summary>
    /// A spare weapon for the bait, from the inventory before the bank: one already on Mana Vamp, else one
    /// with no enhancement. Never one carrying another enhancement. Unbanks it. Null when there is none.
    /// </summary>
    private static InventoryItem? PickBaitWeapon(string mainWeapon)
    {
        bool Spare(InventoryItem i) =>
            i != null && i.ItemGroup == "Weapon" && !i.Equipped && Adv.WeaponCatagories.Contains(i.Category)
            && !i.Name.Equals(mainWeapon, StringComparison.OrdinalIgnoreCase)
            && (Bot.Player.IsMember || !i.Upgrade);

        InventoryItem[] spares = Bot.Inventory.Items.Where(Spare).Concat(Bot.Bank.Items.Where(Spare)).ToArray();
        InventoryItem? pick = spares.FirstOrDefault(IsManaVamp) ?? spares.FirstOrDefault(i => i.EnhancementLevel == 0);
        return pick != null && C.CheckInventory(pick.Name) ? pick : null;
    }

    private static bool IsManaVamp(InventoryItem weapon) =>
        weapon.EnhancementLevel > 0 && weapon.ProcID == (int)WeaponSpecial.Mana_Vamp;

    private static InventoryItem? EquippedWeapon() =>
        Bot.Inventory.Items.FirstOrDefault(i => i != null && i.Equipped && i.ItemGroup == "Weapon");

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

        if (_entry.Role == LockBaiter)
            PlayLockTrick(boss, armySize);

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

            // Every Role: stop attacking while Ezrajal reflects.
            if (CounterAttackUp())
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

    /// <summary>
    /// The guide's lock trick: hits Ezrajal on auto attacks with the bait weapon until he locks its
    /// Mana Vamp, or for <see cref="LockTimeoutSec"/>; then steps out of his reach, puts the Loadout's
    /// weapon back on and waits for the whole party, so everyone restarts the fight together while
    /// the locks last. An account without a bait weapon only steps out and waits.
    /// </summary>
    private void PlayLockTrick(string boss, int armySize)
    {
        if (_baitWeapon != null && C.CheckInventory(_baitWeapon))
        {
            Engine.DisableSkills(); // auto attacks only, so the last thing Ezrajal can lock is the bait's Mana Vamp
            C.Equip(_baitWeapon);
            Engine.ChooseBestCell(boss);
            BaitLock(boss);
        }
        else
            C.Logger($"[Lock] {Bot.Player.Username} has no bait weapon; waiting out of Ezrajal's reach for the party's locks.");

        Bot.Combat.CancelAutoAttack();
        Bot.Combat.CancelTarget();
        C.JumpWait();
        if (_mainWeapon != null)
        {
            C.Equip(_mainWeapon);
            if (!Bot.Inventory.IsEquipped(_mainWeapon))
                C.Logger($"[Lock] {_mainWeapon} is not equipped again.", "Warning");
        }

        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, LockSyncFile);
        C.Logger("[Lock] The whole party is back on its weapons; restarting the fight together.");
        Engine.ChooseBestCell(boss);
        Engine.EnableSkills();
    }

    /// <summary>Hits Ezrajal until his "Skill Locked" aura on this player names Mana Vamp, or <see cref="LockTimeoutSec"/> passes.</summary>
    private void BaitLock(string boss)
    {
        _baitLocked = false;
        _baiting = true;
        Bot.Events.ExtensionPacketReceived += LockListener;
        try
        {
            C.Logger($"[Lock] Hitting Ezrajal with {_baitWeapon} until he locks its Mana Vamp, for up to {LockTimeoutSec} s.");
            DateTime giveUp = DateTime.UtcNow.AddSeconds(LockTimeoutSec);
            while (!_baitLocked && DateTime.UtcNow < giveUp && !Bot.ShouldExit)
            {
                if (!Bot.Player.Alive)
                    Bot.Sleep(500);
                else if (CounterAttackUp())
                {
                    Bot.Combat.CancelAutoAttack();
                    Bot.Sleep(500);
                }
                else
                {
                    Bot.Combat.Attack(boss);
                    Bot.Sleep(200);
                }
            }
        }
        finally
        {
            _baiting = false;
            Bot.Events.ExtensionPacketReceived -= LockListener;
        }

        if (_baitLocked)
            C.Logger("[Lock] Ezrajal locked the bait's Mana Vamp; swapping to the main weapon.");
        else
            C.Logger($"[Lock] Ezrajal didn't lock the bait's Mana Vamp within {LockTimeoutSec} s; swapping to the main weapon anyway.", "Warning");
    }

    /// <summary>
    /// Ezrajal's locks arrive in "ct" packets as <c>{ cmd: "aura+", tInf: "p:&lt;player ID&gt;",
    /// auras: [{ nam: "Skill Locked", val: "Mana Vampire", msgOn: "@Mana Vampire has been locked!" }] }</c>.
    /// </summary>
    private void LockListener(dynamic packet)
    {
        try
        {
            if (!_baiting)
                return;

            string type = packet["params"].type;
            if (type is not "json")
                return;

            dynamic data = packet["params"].dataObj;
            if (data["cmd"]?.ToString() != "ct" || data["a"] == null)
                return;

            string me = $"p:{Bot.Player.ID}";
            foreach (dynamic action in data["a"])
            {
                if (action?["cmd"]?.ToString() != "aura+" || action["tInf"]?.ToString() != me || action["auras"] == null)
                    continue;

                foreach (dynamic aura in action["auras"])
                {
                    if (aura?["nam"]?.ToString() != SkillLocked)
                        continue;

                    string val = aura["val"]?.ToString() ?? "";
                    string msgOn = aura["msgOn"]?.ToString() ?? "";
                    if (val.Contains(ManaVampLock, StringComparison.OrdinalIgnoreCase)
                        || msgOn.Contains(ManaVampLock, StringComparison.OrdinalIgnoreCase))
                        _baitLocked = true;
                    else
                        C.Logger($"[Lock] Ezrajal locked {val}, not the bait's Mana Vamp.");
                }
            }
        }
        catch { }
    }

    private static bool CounterAttackUp() =>
        Bot.Player.HasTarget && Bot.Target?.Auras?.Any(a => a != null && a.Name == "Counter Attack") == true;
}
