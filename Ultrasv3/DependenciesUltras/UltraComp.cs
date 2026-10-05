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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Skua.Core.Interfaces;
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

    public void Enhance()
    {
        if (Enhancement == null)
        {
            C.Logger($"[Loadout] {Bot.Player.CurrentClass?.Name} keeps its own gear.");
            return;
        }

        Adv.EnhanceEquipped(
            Enhancement.Value,
            Pick(Cape, CapeUnlocked, CapeSpecial.None),
            Pick(Helm, HelmUnlocked, HelmSpecial.None),
            Pick(Weapon, WeaponUnlocked, WeaponSpecial.None),
            EnhanceWhenAutoEnhanceIsOff
        );
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
