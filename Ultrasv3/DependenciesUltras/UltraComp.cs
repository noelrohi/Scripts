/*
name: null
description: null
tags: null
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPotions.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/GetScrolls.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraAsync.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPartyLayout.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraWaitForArmy.cs

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

/// <summary>
/// A Comp: a named strategy for one Ultra. It holds 4 entries, one per class, each with the
/// class's Role, Loadout and taunt timing. A boss script declares its Comps as static data
/// at its top; the fight code switches on the Role and reads the taunt timing from the entry
/// for the class this account plays, and names no class itself.
/// The <c>&lt;Boss&gt; comp</c> DoAllUltras option picks the Comp; blank means <see cref="Default"/>.
/// </summary>
public class UltraComp
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;
    private static CoreEnginev3 Engine => CoreEnginev3.Instance;

    /// <summary>The Comp a blank <c>&lt;Boss&gt; comp</c> option runs: the boss's behaviour before Comps.</summary>
    public const string Default = "default";

    /// <summary>Lowercase and hyphenated after its classes in Role order, e.g. <c>dot-lr-ap-loo</c>.</summary>
    public string Name { get; }

    /// <summary>One entry per class, in Role order. The automatic assignment fills them in this order.</summary>
    public IReadOnlyList<UltraCompEntry> Entries { get; }

    public IEnumerable<string> Classes => Entries.Select(e => e.Class);

    // Skua instantiates every included Script's class; Comps are declared by the boss scripts.
    public UltraComp() : this(string.Empty) { }

    public UltraComp(string name, params UltraCompEntry[] entries)
    {
        Name = name;
        Entries = entries;
    }

    /// <summary>The entry for <paramref name="className"/>, or null when the class isn't in this Comp.</summary>
    public UltraCompEntry? EntryFor(string? className) =>
        Entries.FirstOrDefault(e => e.Class.Equals(className, StringComparison.OrdinalIgnoreCase));

    public static string OptionName(string boss) => $"{boss}Comp";

    public static Option<string> Option(string boss, string displayName, IEnumerable<UltraComp> comps) =>
        new(OptionName(boss), $"{displayName} comp",
            $"Which Comp runs. Blank: {Default}. Comps: {string.Join("; ", comps.Select(c => $"{c.Name} ({string.Join(", ", c.Classes)})"))}.", "");

    /// <summary>
    /// Reads the boss's <c>&lt;Boss&gt; comp</c> option the way <see cref="UltraPartyLayout.Read"/> reads
    /// its layout, and returns that Comp. An unknown name stops the bot, listing the boss's Comps, and returns null.
    /// </summary>
    public static UltraComp? Read(string boss, IEnumerable<UltraComp> comps)
    {
        string name = UltraPartyLayout.ReadOption(OptionName(boss)).Trim();
        if (name.Length == 0)
            name = Default;

        UltraComp? comp = comps.FirstOrDefault(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase));
        if (comp == null)
        {
            Stop(boss, $"No Comp named \"{name}\". {boss} Comps: {string.Join(", ", comps.Select(c => c.Name))}.");
            return null;
        }

        C.Logger($"[Comp:{boss}] {comp.Name}: {string.Join("; ", comp.Entries.Select(e => $"{e.Class} as {e.Role}"))}.");
        return comp;
    }

    /// <summary>
    /// A boss script's Prep: reads the boss's Comp and Party Layout, equips this account's class from
    /// the Comp, enhances it as its Loadout says and raises its max HP to the entry's
    /// <see cref="UltraCompEntry.MinMaxHealth"/>. Returns null after stopping the bot.
    /// </summary>
    public static (UltraComp Comp, UltraPartyLayout Party, UltraCompEntry Entry)? Prep(
        string boss, IEnumerable<UltraComp> comps, string logTag, CoreUltrav3 ultra, int armySize, string classSyncFileName)
    {
        UltraComp? comp = Read(boss, comps);
        if (comp == null)
            return null;
        UltraPartyLayout party = UltraPartyLayout.Read(boss);

        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);

        C.Logger($"[{logTag}] Equipping the {comp.Name} Comp's classes for army size {armySize}.");
        UltraCompEntry? entry = comp.EquipClass(party, ultra, armySize, classSyncFileName);
        if (entry == null)
            return null;

        entry.Loadout.Enhance();
        RaiseMaxHealth(boss, entry);
        return (comp, party, entry);
    }

    /// <summary>
    /// When the class has less max HP than <paramref name="entry"/>'s <see cref="UltraCompEntry.MinMaxHealth"/>,
    /// raises it with gear (<see cref="UltraLoadout.RaiseMaxHealth"/>), and warns with what it tried when it stays short.
    /// </summary>
    private static void RaiseMaxHealth(string boss, UltraCompEntry entry)
    {
        int min = entry.MinMaxHealth;
        int before = Bot.Player.MaxHealth;
        if (min <= 0 || before >= min)
            return;

        C.Logger($"[Comp:{boss}] {entry.Class} has {before} max HP, under the {min} the {entry.Role} needs: raising it with gear.");
        List<string> tried = entry.Loadout.RaiseMaxHealth(min);
        string what = tried.Count > 0 ? string.Join("; ", tried) : "nothing: no gear to equip or enhance";
        int after = Bot.Player.MaxHealth;
        if (after >= min)
            C.Logger($"[Comp:{boss}] {entry.Class} has {after} max HP now. {what}.");
        else
            C.Logger($"[Comp:{boss}] {entry.Class} still has {after} max HP (was {before}), under the {min} the {entry.Role} needs. Tried: {what}. Expect it to die; raise it with gear.", "Warning");
    }

    /// <summary>
    /// Readies this account for an Attempt: stocks <paramref name="entry"/>'s Loadout, waits for the
    /// party on Whitemap, uses the Loadout and joins <paramref name="map"/>. Buying potion reagents can
    /// swap to a farm class, so the Comp's class goes back on after stocking and before joining.
    /// Warns when the class still has less max HP than the entry's <see cref="UltraCompEntry.MinMaxHealth"/>.
    /// </summary>
    public static void ReadyForAttempt(UltraPartyLayout party, UltraCompEntry entry, int armySize, string waitSyncFileName, string map)
    {
        party.EnsureClass();
        entry.Loadout.Stock();
        party.EnsureClass();

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFileName, useSkill: false);

        entry.Loadout.Use();

        party.EnsureClass();
        if (entry.MinMaxHealth > 0 && Bot.Player.MaxHealth < entry.MinMaxHealth)
            C.Logger($"[Comp:{party.Boss}] {entry.Class} has {Bot.Player.MaxHealth} max HP, under the {entry.MinMaxHealth} the {entry.Role} needs: expect it to die. Raise it with gear.", "Warning");
        Engine.Join(map);
    }

    /// <summary>
    /// Equips this account's class from this Comp and returns its entry, or returns null after
    /// stopping the bot. The Party Layout may name only this Comp's classes; a blank layout
    /// hands out this Comp's classes automatically from what the accounts own.
    /// </summary>
    public UltraCompEntry? EquipClass(UltraPartyLayout party, CoreUltrav3 ultra, int armySize, string syncFileName)
    {
        string? outside = party.ClassByUser.Values.FirstOrDefault(cls => EntryFor(cls) == null);
        if (outside != null)
        {
            Stop(party.Boss, $"{outside} is not in the Comp {Name}. Its classes: {string.Join(", ", Classes)}. " +
                $"Change the {UltraPartyLayout.OptionName(party.Boss)} or {OptionName(party.Boss)} option.");
            return null;
        }

        string[][] classesByRole = Entries.Select(e => new[] { e.Class }).ToArray();
        string cls = party.EquipClass(ultra, classesByRole, armySize, syncFileName);
        if (string.IsNullOrEmpty(cls) || !party.EnsureClass())
            return null;

        UltraCompEntry entry = EntryFor(cls)!;
        C.Logger($"[Comp:{party.Boss}] {entry.Class} plays {entry.Role} in {Name}. Loadout: {entry.Loadout}. Taunts: {entry.Taunt}.");
        return entry;
    }

    /// <summary>
    /// Starts <paramref name="entry"/>'s timed taunts on a background loop and returns the fight
    /// start they count from. The Comp's earliest timed taunter writes the start to
    /// <paramref name="fightTimeSyncPath"/>; the other timed taunters read it. A class that never
    /// taunts, or whose Role carries its taunts, starts no loop and gets now.
    /// </summary>
    public DateTime StartTaunts(UltraCompEntry entry, CoreUltrav3 ultra, string fightTimeSyncPath, CancellationToken cancellationToken)
    {
        UltraTaunt taunt = entry.Taunt;
        if (!taunt.IsTimed)
            return DateTime.UtcNow;

        UltraCompEntry fightTimeWriter = Entries.Where(e => e.Taunt.IsTimed).OrderBy(e => e.Taunt.AtSec).First();
        DateTime fightStart = entry == fightTimeWriter
            ? UltraAsync.SetFightTime(C, fightTimeSyncPath)
            : UltraAsync.GetFightTime(ultra, C, fightTimeSyncPath);

        // The taunt loop fires on pulses: every pulse whose number modulo the count is the index.
        int pulseSec = Gcd(taunt.AtSec, taunt.CycleSec);
        string? aura = taunt.SkipWhileAura;
        Func<bool>? skip = aura == null ? null : () => Engine.HasAura(aura, true);
        UltraAsync.StartTauntLoop(Bot, C, Engine, fightStart, taunt.AtSec / pulseSec, taunt.CycleSec / pulseSec, skip, pulseSec, cancellationToken);
        return fightStart;
    }

    private static int Gcd(int a, int b) => b == 0 ? a : Gcd(b, a % b);

    private static void Stop(string boss, string message) =>
        C.Logger($"[Comp:{boss}] {message}", "Error", messageBox: true, stopBot: true);
}

/// <summary>One class in a <see cref="UltraComp"/>: its Role, Loadout and taunt timing.</summary>
public class UltraCompEntry
{
    public string Class { get; init; } = string.Empty;

    /// <summary>The job the class has, as an identifier the boss's fight code switches on.</summary>
    public string Role { get; init; } = string.Empty;

    public UltraLoadout Loadout { get; init; } = new();

    public UltraTaunt Taunt { get; init; } = UltraTaunt.Never;

    /// <summary>
    /// The max HP the class needs, e.g. a guide's minimum. 0: none. Prep raises max HP to it with gear
    /// (<see cref="UltraLoadout.RaiseMaxHealth"/>); still less is warned about then and before each Attempt.
    /// </summary>
    public int MinMaxHealth { get; init; }
}

/// <summary>
/// A class's Loadout in a Comp: its enhancements, potions and scroll. The class itself, also part
/// of the Loadout, is the entry's <see cref="UltraCompEntry.Class"/>.
/// Applied before the fight: <see cref="Enhance"/> once, then <see cref="Stock"/> and <see cref="Use"/>
/// before each Attempt.
/// </summary>
public class UltraLoadout
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;
    private static CoreEnginev3 Engine => CoreEnginev3.Instance;
    private static CoreAdvanced Adv => _Adv ??= new CoreAdvanced();
    private static CoreAdvanced _Adv;
    private static UltraPotions Pots => _Pots ??= new UltraPotions();
    private static UltraPotions _Pots;
    private static GetScrolls Scrolls => _Scrolls ??= new GetScrolls();
    private static GetScrolls _Scrolls;

    /// <summary>The enhancement type. Null: the class keeps its own gear and nothing is enhanced.</summary>
    public EnhancementType? Enhancement { get; init; }

    // Each special lists choices in order: the first one unlocked is used, else the last.
    public WeaponSpecial[] Weapon { get; init; } = Array.Empty<WeaponSpecial>();
    public CapeSpecial[] Cape { get; init; } = Array.Empty<CapeSpecial>();
    public HelmSpecial[] Helm { get; init; } = Array.Empty<HelmSpecial>();

    /// <summary>Enhance even when the CoreBots option DisableAutoEnhance is on.</summary>
    public bool EnhanceWhenAutoEnhanceIsOff { get; init; }

    /// <summary>Equipped and drunk in order before each Attempt. Empty: no potions.</summary>
    public string[] Potions { get; init; } = Array.Empty<string>();

    /// <summary>Click the equipped potion again during the fight (<see cref="ActivatePotion"/>). False: the potions are drunk only before each Attempt.</summary>
    public bool ClickPotionInFight { get; init; } = true;

    public const string ScrollOfEnrage = "Scroll of Enrage";
    public const string ScrollOfDecay = "Scroll of Decay";

    /// <summary>The scroll equipped in the consumable slot after the potions, e.g. <see cref="ScrollOfEnrage"/>. Null: none.</summary>
    public string? Scroll { get; init; }

    /// <summary>
    /// Never overwrite a cape on Vainglory or a helm on Pneuma, which other Comps use: <see cref="Enhance"/>
    /// and <see cref="EnhanceWeapon"/> put the Loadout's cape or helm enhancement on another cape or helm instead.
    /// </summary>
    public bool KeepVaingloryAndPneuma { get; init; }

    public void Enhance()
    {
        if (Enhancement == null)
        {
            C.Logger($"[Loadout] {Bot.Player.CurrentClass?.Name} keeps its own gear.");
            return;
        }

        CapeSpecial cape = Pick(Cape, CapeUnlocked, CapeSpecial.None);
        HelmSpecial helm = Pick(Helm, HelmUnlocked, HelmSpecial.None);
        WeaponSpecial weapon = Pick(Weapon, WeaponUnlocked, WeaponSpecial.None);
        if (KeepVaingloryAndPneuma)
        {
            SpareVaingloryOrPneuma(Slot.Cape, cape, helm, weapon, CapeUnlocked(cape));
            SpareVaingloryOrPneuma(Slot.Helm, cape, helm, weapon, HelmUnlocked(helm));
        }
        EnhanceEquipped(cape, helm, weapon, KeepVaingloryAndPneuma);
    }

    /// <summary>
    /// Enhances <paramref name="weapon"/>, a weapon that isn't equipped, with the Loadout's enhancement and
    /// <paramref name="special"/>, e.g. a bait weapon, leaving the equipped gear as it is. Whether it has them now.
    /// </summary>
    public bool EnhanceSpareWeapon(string weapon, WeaponSpecial special)
    {
        if (Enhancement == null || !CanEnhance)
            return false;

        EnhanceInInventory(weapon, CapeSpecial.None, HelmSpecial.None, special);
        return Bot.Inventory.Items.FirstOrDefault(i => i != null && i.Name == weapon) is { } w
            && Has(w, CapeSpecial.None, HelmSpecial.None, special);
    }

    /// <summary>
    /// Raises max HP to <paramref name="minMaxHealth"/> with the Loadout's enhancement, one gear slot at a
    /// time (class, helm, cape, weapon: the ones enhancements put stats on), the emptiest first: no item,
    /// unenhanced, under full level, then another enhancement. A slot whose item already has the Loadout's
    /// enhancement at full level is left alone. Otherwise it equips an item from the inventory or bank that
    /// already has it, else enhances a spare, unenhanced ones first. A special that isn't unlocked can't be put
    /// on, so that slot gets the plain enhancement. Never touches a cape on Vainglory or a helm on Pneuma, which
    /// other Comps use. Checks max HP after each change, puts the old item back when it fell, and stops once
    /// it's met. Returns what it did, for the log.
    /// </summary>
    public List<string> RaiseMaxHealth(int minMaxHealth)
    {
        List<string> done = new();
        if (Enhancement == null)
        {
            done.Add("nothing: the class keeps its own gear");
            return done;
        }

        CapeSpecial cape = Pick(Cape, CapeUnlocked, CapeSpecial.None);
        HelmSpecial helm = Pick(Helm, HelmUnlocked, HelmSpecial.None);
        WeaponSpecial weapon = Pick(Weapon, WeaponUnlocked, WeaponSpecial.None);
        CapeSpecial capeOn = PickUnlocked(Cape, CapeUnlocked, CapeSpecial.None);
        HelmSpecial helmOn = PickUnlocked(Helm, HelmUnlocked, HelmSpecial.None);
        WeaponSpecial weaponOn = PickUnlocked(Weapon, WeaponCanGoOn, WeaponSpecial.None);

        Slot[] slots = { Slot.Class, Slot.Helm, Slot.Cape, Slot.Weapon };
        foreach (Slot slot in slots.OrderBy(Need).ToArray())
        {
            int before = Bot.Player.MaxHealth;
            if (before >= minMaxHealth || Bot.ShouldExit)
                break;

            // The slot being raised gets what can go on it; the others what Enhance gave them.
            CapeSpecial c = slot == Slot.Cape ? capeOn : cape;
            HelmSpecial h = slot == Slot.Helm ? helmOn : helm;
            WeaponSpecial w = slot == Slot.Weapon ? weaponOn : weapon;
            InventoryItem? old = Equipped(slot);
            if (old != null && Has(old, c, h, w))
                continue;

            string? change = Refit(slot, c, h, w);
            if (change == null)
            {
                done.Add($"{slot}: {(old == null ? "none" : $"{old.Name} ({Describe(old)})")}, no other to equip or enhance");
                continue;
            }

            Bot.Wait.ForTrue(() => Bot.Player.MaxHealth != before, 10);
            int after = Bot.Player.MaxHealth;
            if (after < before && old != null && Wear(old))
            {
                done.Add($"{change}, but max HP fell {before} to {after}: {old.Name} back on");
                continue;
            }
            done.Add($"{change}: max HP {before} to {after}");
        }
        return done;
    }

    private enum Slot { Class, Helm, Cape, Weapon }

    /// <summary>Enhances the equipped gear; <paramref name="keep"/>: an equipped Vainglory cape or Pneuma helm keeps its special.</summary>
    private void EnhanceEquipped(CapeSpecial cape, HelmSpecial helm, WeaponSpecial weapon, bool keep)
    {
        if (keep && Equipped(Slot.Cape) is { } c && IsVaingloryOrPneuma(c))
            cape = CapeSpecial.Vainglory;
        if (keep && Equipped(Slot.Helm) is { } h && IsVaingloryOrPneuma(h))
            helm = HelmSpecial.Pneuma;
        Adv.EnhanceEquipped(Enhancement!.Value, cape, helm, weapon, EnhanceWhenAutoEnhanceIsOff);
    }

    /// <summary>
    /// When the equipped cape or helm is on Vainglory or Pneuma and the Loadout would put another enhancement
    /// on it, puts that enhancement on another cape or helm (<see cref="Refit"/>); without one, keeps it as it is.
    /// </summary>
    private void SpareVaingloryOrPneuma(Slot slot, CapeSpecial cape, HelmSpecial helm, WeaponSpecial weapon, bool unlocked)
    {
        if (Equipped(slot) is not { } kept || !IsVaingloryOrPneuma(kept) || kept.EnhancementPatternID == Pattern(slot, cape, helm)
            || !unlocked || !CanEnhance)
            return;

        string? change = Refit(slot, cape, helm, weapon);
        C.Logger(change != null
            ? $"[Loadout] Keeps {kept.Name} on {Describe(kept)} for other Comps: {change}."
            : $"[Loadout] Keeps {kept.Name} on {Describe(kept)} for other Comps, with no other {slot.ToString().ToLower()} to enhance.");
    }

    /// <summary>
    /// Puts the enhancement <paramref name="cape"/>, <paramref name="helm"/> and <paramref name="weapon"/> name
    /// for <paramref name="slot"/> on: equips an owned item that already has it at full level, else equips a spare
    /// that isn't on Vainglory or Pneuma (unenhanced first, then the equipped one, then plain enhancements, lowest
    /// first) and enhances it, putting the old item back when that fails. Returns what it did, or null when it had
    /// nothing to try.
    /// </summary>
    private string? Refit(Slot slot, CapeSpecial cape, HelmSpecial helm, WeaponSpecial weapon)
    {
        InventoryItem? old = Equipped(slot);
        List<InventoryItem> owned = Owned(slot);

        InventoryItem? ready = owned.FirstOrDefault(i => !i.Equipped && Has(i, cape, helm, weapon));
        if (ready != null && Wear(ready))
            return $"equipped {ready.Name}, already {Describe(ready)}";

        if (!CanEnhance)
            return null;
        InventoryItem? spare = owned.Where(i => !IsVaingloryOrPneuma(i))
            .OrderBy(i => i.EnhancementLevel == 0 ? 0 : i.Equipped ? 1 : HasSpecial(i) ? 3 : 2)
            .ThenBy(i => !i.Equipped)
            .ThenBy(i => i.EnhancementLevel)
            .FirstOrDefault();
        if (spare == null)
            return null;
        string was = Describe(spare);
        if (spare.EnhancementLevel == 0 && !spare.Equipped)
            EnhanceInInventory(spare.Name, cape, helm, weapon);
        if (!Wear(spare))
            return $"couldn't equip {spare.Name}";

        EnhanceEquipped(cape, helm, weapon, keep: true);
        if (Equipped(slot) is { } now && Has(now, cape, helm, weapon))
            return $"enhanced {now.Name} ({was}) to {Describe(now)}";
        if (old != null && old.ID != spare.ID)
            Wear(old);
        return $"couldn't enhance {spare.Name} ({was}){(old != null && old.ID != spare.ID ? $", {old.Name} back on" : "")}";
    }

    /// <summary>
    /// Enhances an item that isn't equipped, unbanking it first. The game won't equip an unenhanced item,
    /// so a spare has to be enhanced where it lies before it can go on. CoreAdvanced.EnhanceItem always
    /// follows the CoreBots option DisableAutoEnhance, even with <see cref="EnhanceWhenAutoEnhanceIsOff"/>.
    /// </summary>
    private void EnhanceInInventory(string item, CapeSpecial cape, HelmSpecial helm, WeaponSpecial weapon)
    {
        if (C.CheckInventory(item))
            Adv.EnhanceItem(item, Enhancement!.Value, cape, helm, weapon);
    }

    private bool CanEnhance =>
        EnhanceWhenAutoEnhanceIsOff || !(C.CBOBool("DisableAutoEnhance", out bool off) && off);

    /// <summary>The enhancement pattern the Loadout puts on a cape or helm: its special, else the enhancement type.</summary>
    private int Pattern(Slot slot, CapeSpecial cape, HelmSpecial helm) => slot switch
    {
        Slot.Cape when cape != CapeSpecial.None => (int)cape,
        Slot.Helm when helm != HelmSpecial.None => (int)helm,
        _ => (int)Enhancement!.Value,
    };

    /// <summary>Whether <paramref name="item"/> has the Loadout's enhancement and the special for its slot at full level, as CoreAdvanced checks it.</summary>
    private bool Has(InventoryItem item, CapeSpecial cape, HelmSpecial helm, WeaponSpecial weapon)
    {
        if (Enhancement is not { } type || item.EnhancementLevel <= 0 || item.EnhancementLevel != Bot.Player.Level)
            return false;
        int pattern = item.EnhancementPatternID;
        if (item.Category == ItemCategory.Cape)
            return pattern == Pattern(Slot.Cape, cape, helm);
        if (item.Category == ItemCategory.Helm)
            return pattern == Pattern(Slot.Helm, cape, helm);
        if (item.Category == ItemCategory.Class || weapon == WeaponSpecial.None)
            return pattern == (int)type;
        if ((int)weapon <= 6)
            return pattern == (int)type && item.ProcID == (int)weapon;
        return item.ProcID == (int)weapon;
    }

    private static bool IsVaingloryOrPneuma(InventoryItem item) =>
        item.EnhancementLevel > 0
        && ((item.Category == ItemCategory.Cape && item.EnhancementPatternID == (int)CapeSpecial.Vainglory)
            || (item.Category == ItemCategory.Helm && item.EnhancementPatternID == (int)HelmSpecial.Pneuma));

    private static bool HasSpecial(InventoryItem item) =>
        item.ProcID != 0 || !Enum.IsDefined(typeof(EnhancementType), item.EnhancementPatternID);

    private static bool InSlot(InventoryItem item, Slot slot) => slot switch
    {
        Slot.Class => item.Category == ItemCategory.Class,
        Slot.Helm => item.Category == ItemCategory.Helm,
        Slot.Cape => item.Category == ItemCategory.Cape,
        _ => Adv.WeaponCatagories.Contains(item.Category),
    };

    private static InventoryItem? Equipped(Slot slot) =>
        Bot.Inventory.Items.FirstOrDefault(i => i != null && i.Equipped && InSlot(i, slot));

    /// <summary>The items for <paramref name="slot"/> this account can wear, inventory before bank. The class: only the equipped one.</summary>
    private static List<InventoryItem> Owned(Slot slot)
    {
        if (slot == Slot.Class)
            return Equipped(slot) is { } cls ? new List<InventoryItem> { cls } : new List<InventoryItem>();
        return Bot.Inventory.Items.Concat(Bot.Bank.Items)
            .Where(i => i != null && InSlot(i, slot) && (Bot.Player.IsMember || !i.Upgrade))
            .ToList();
    }

    /// <summary>Emptiest slot first: no item, unenhanced, under full level, then another enhancement.</summary>
    private static int Need(Slot slot) => Equipped(slot) switch
    {
        null => 0,
        { EnhancementLevel: 0 } => 1,
        { } i when i.EnhancementLevel < Bot.Player.Level => 2,
        _ => 3,
    };

    /// <summary>Equips <paramref name="item"/>, unbanking it first. Whether it's on.</summary>
    private static bool Wear(InventoryItem item)
    {
        if (!item.Equipped)
            C.Equip(item.ID);
        return Bot.Inventory.IsEquipped(item.ID);
    }

    private static string Describe(InventoryItem item)
    {
        if (item.EnhancementLevel <= 0)
            return "unenhanced";
        int pattern = item.EnhancementPatternID;
        string name = Enum.IsDefined(typeof(EnhancementType), pattern) ? ((EnhancementType)pattern).ToString()
            : pattern == 10 ? "Forge"
            : item.Category == ItemCategory.Cape && Enum.IsDefined(typeof(CapeSpecial), pattern) ? ((CapeSpecial)pattern).ToString()
            : item.Category == ItemCategory.Helm && Enum.IsDefined(typeof(HelmSpecial), pattern) ? ((HelmSpecial)pattern).ToString()
            : $"pattern {pattern}";
        if (item.ProcID != 0 && Enum.IsDefined(typeof(WeaponSpecial), item.ProcID))
            name += $"/{(WeaponSpecial)item.ProcID}";
        return $"{name} level {item.EnhancementLevel}";
    }

    /// <summary>Buys the potions and gets the scroll. Either can swap the class: put it back afterwards.</summary>
    public void Stock()
    {
        Pots.EnsurePotions(Potions);

        switch (Scroll)
        {
            case null:
                break;
            case ScrollOfEnrage:
                Scrolls.GetScrollOfEnrage();
                break;
            case ScrollOfDecay:
                Scrolls.GetScrollOfDecay();
                break;
            default:
                if (!C.CheckInventory(Scroll))
                    C.Logger($"[Loadout] No {Scroll}; get it yourself.", "Warning");
                break;
        }
    }

    /// <summary>Drinks the potions, then equips the scroll. Logs a potion or scroll it couldn't equip by name.</summary>
    public void Use()
    {
        Pots.UsePotions(Potions);

        if (Scroll == null)
            return;

        Engine.EquipConsumable(Scroll);
        if (!Bot.Inventory.IsEquipped(Scroll))
            C.Logger($"[Loadout] {Scroll} is not equipped, this class cannot taunt.", "Warning");
    }

    /// <summary>Clicks an equipped clickable potion during the fight, when the Loadout has potions and clicks them in the fight.</summary>
    public void ActivatePotion()
    {
        if (ClickPotionInFight && Potions.Length > 0)
            Pots.ActivateEquippedPotion();
    }

    public override string ToString()
    {
        List<string> parts = new();
        if (Enhancement == null)
            parts.Add("own gear");
        else
        {
            List<string> specials = new();
            if (Weapon.Length > 0)
                specials.Add($"weapon {string.Join(" else ", Weapon)}");
            if (Cape.Length > 0)
                specials.Add($"cape {string.Join(" else ", Cape)}");
            if (Helm.Length > 0)
                specials.Add($"helm {string.Join(" else ", Helm)}");
            parts.Add(specials.Count > 0 ? $"{Enhancement}, {string.Join(", ", specials)}" : $"{Enhancement}");
        }
        parts.Add(Potions.Length > 0 ? string.Join(", ", Potions) : "no potions");
        parts.Add(Scroll ?? "no scroll");
        return string.Join("; ", parts);
    }

    private static T Pick<T>(T[] choices, Func<T, bool> unlocked, T none)
    {
        foreach (T choice in choices)
        {
            if (unlocked(choice))
                return choice;
        }
        return choices.Length == 0 ? none : choices[^1];
    }

    /// <summary>The first choice that's unlocked, else <paramref name="none"/>: unlike <see cref="Pick"/>, never a locked one.</summary>
    private static T PickUnlocked<T>(T[] choices, Func<T, bool> unlocked, T none) =>
        choices.Where(unlocked).DefaultIfEmpty(none).First();

    /// <summary>Whether CoreAdvanced can put <paramref name="s"/> on a weapon: Awe specials need Awe enhancements.</summary>
    private static bool WeaponCanGoOn(WeaponSpecial s) =>
        WeaponUnlocked(s) && (s == WeaponSpecial.Forge || (int)s > 6 || Adv.uAwe());

    private static bool WeaponUnlocked(WeaponSpecial s) => s switch
    {
        WeaponSpecial.Forge => Adv.uForgeWeapon(),
        WeaponSpecial.Awe_Blast => Adv.uAwe(),
        WeaponSpecial.Lacerate => Adv.uLacerate(),
        WeaponSpecial.Smite => Adv.uSmite(),
        WeaponSpecial.Valiance => Adv.uValiance(),
        WeaponSpecial.Arcanas_Concerto => Adv.uArcanasConcerto(),
        WeaponSpecial.Acheron => Adv.uAcheron(),
        WeaponSpecial.Elysium => Adv.uElysium(),
        WeaponSpecial.Praxis => Adv.uPraxis(),
        WeaponSpecial.Dauntless => Adv.uDauntless(),
        WeaponSpecial.Ravenous => Adv.uRavenous(),
        _ => true
    };

    private static bool CapeUnlocked(CapeSpecial s) => s switch
    {
        CapeSpecial.Forge => Adv.uForgeCape(),
        CapeSpecial.Absolution => Adv.uAbsolution(),
        CapeSpecial.Avarice => Adv.uAvarice(),
        CapeSpecial.Vainglory => Adv.uVainglory(),
        CapeSpecial.Penitence => Adv.uPenitence(),
        CapeSpecial.Lament => Adv.uLament(),
        _ => true
    };

    private static bool HelmUnlocked(HelmSpecial s) => s switch
    {
        HelmSpecial.Forge => Adv.uForgeHelm(),
        HelmSpecial.Vim => Adv.uVim(),
        HelmSpecial.Examen => Adv.uExamen(),
        HelmSpecial.Anima => Adv.uAnima(),
        HelmSpecial.Pneuma => Adv.uPneuma(),
        HelmSpecial.Hearty => Adv.uHearty(),
        _ => true
    };
}

/// <summary>
/// When a class in a Comp taunts: never, at a fixed second of a repeating cycle counted from
/// the fight start, or as its Role says (for example on a boss's chat line, or while an orb is up).
/// </summary>
public class UltraTaunt
{
    public static readonly UltraTaunt Never = new(false, 0, 0, null, "never");

    /// <summary>Taunts at <paramref name="atSec"/> of every <paramref name="cycleSec"/>, optionally not while this class has <paramref name="skipWhileAura"/>.</summary>
    public static UltraTaunt Every(int cycleSec, int atSec, string? skipWhileAura = null) =>
        new(true, cycleSec, atSec, skipWhileAura,
            $"at {atSec} s of every {cycleSec} s{(skipWhileAura == null ? "" : $", not while it has {skipWhileAura}")}");

    /// <summary>The Role carries the taunts; <paramref name="when"/> says when, for people reading the Comp.</summary>
    public static UltraTaunt ByRole(string when) => new(false, 0, 0, null, when);

    public bool IsTimed { get; }
    public int CycleSec { get; }
    public int AtSec { get; }
    public string? SkipWhileAura { get; }
    private readonly string _description;

    private UltraTaunt(bool isTimed, int cycleSec, int atSec, string? skipWhileAura, string description)
    {
        IsTimed = isTimed;
        CycleSec = cycleSec;
        AtSec = atSec;
        SkipWhileAura = skipWhileAura;
        _description = description;
    }

    public override string ToString() => _description;
}
