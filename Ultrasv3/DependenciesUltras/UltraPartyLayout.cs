/*
name: null
description: null
tags: null
*/

//cs_include Scripts/CoreBots.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraCustomClassSync.cs

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models;
using Skua.Core.Options;

/// <summary>
/// The party layout of one Ultras-v3 boss: which account plays which class.
/// A layout is a DoAllUltras option such as
/// <c>alt1=Dragon of Time; alt2=Legion Revenant; alt3=ArchPaladin; alt4=Lord of Order</c>.
/// Each account equips the class the layout gives its login name; the boss script
/// derives the role from that class. A blank layout keeps the automatic
/// <see cref="UltraCustomClassSync"/> assignment.
/// </summary>
public class UltraPartyLayout
{
    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;

    // The layouts are DoAllUltras options; a single boss script reads them from its saved options file.
    private const string OptionsStorage = "DoAllUltras";

    public const string FormatHint = "Account=Class pairs separated by ';', e.g. alt1=Dragon of Time; alt2=Legion Revenant; alt3=ArchPaladin; alt4=Lord of Order. Blank: assign classes automatically from what the accounts own.";

    /// <summary>The DoAllUltras boss key, e.g. "UltraNulgath".</summary>
    public string Boss { get; }

    /// <summary>Class by login name. Empty when no layout is set.</summary>
    public IReadOnlyDictionary<string, string> ClassByUser { get; }

    public bool IsSet => ClassByUser.Count > 0;

    /// <summary>The class this account plays this run, set by <see cref="EquipClass"/>.</summary>
    public string Class { get; private set; } = string.Empty;

    private UltraPartyLayout(string boss, Dictionary<string, string> classByUser)
    {
        Boss = boss;
        ClassByUser = classByUser;
    }

    public static string OptionName(string boss) => $"{boss}Layout";

    public static Option<string> Option(string boss, string displayName, string classes) =>
        new(OptionName(boss), $"{displayName} layout", $"{FormatHint} Classes: {classes}.", "");

    /// <summary>
    /// Reads the boss's layout once: from the running script's options if it has the
    /// option (DoAllUltras), otherwise from the saved DoAllUltras options.
    /// </summary>
    public static UltraPartyLayout Read(string boss)
    {
        string raw = ReadRaw(OptionName(boss));
        UltraPartyLayout layout = new(boss, Parse(boss, raw));
        if (layout.IsSet)
            C.Logger($"[PartyLayout:{boss}] {string.Join("; ", layout.ClassByUser.Select(kv => $"{kv.Key}={kv.Value}"))}");
        return layout;
    }

    private static string ReadRaw(string optionName)
    {
        if (Bot.Config?.Options.Any(o => o.Name == optionName) == true)
            return Bot.Config!.Get<string>(optionName) ?? string.Empty;

        try
        {
            string file = Path.Combine(ClientFileSources.SkuaOptionsDIR, $"{OptionsStorage}.cfg");
            if (!File.Exists(file))
                return string.Empty;

            string prefix = $"Options:{optionName}=";
            string? line = File.ReadLines(file).FirstOrDefault(l => l.StartsWith(prefix, StringComparison.Ordinal));
            return line?.Substring(prefix.Length) ?? string.Empty;
        }
        catch (Exception ex)
        {
            C.Logger($"[PartyLayout] Could not read {OptionsStorage} options: {ex.Message}", "Warning");
            return string.Empty;
        }
    }

    private static Dictionary<string, string> Parse(string boss, string raw)
    {
        Dictionary<string, string> classByUser = new(StringComparer.OrdinalIgnoreCase);
        foreach (string entry in raw.Split(new[] { ';', ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            string[] parts = entry.Split(new[] { '=' }, 2);
            string user = parts[0].Trim();
            string cls = parts.Length == 2 ? parts[1].Trim() : string.Empty;
            if (user.Length == 0 || cls.Length == 0)
            {
                C.Logger($"[PartyLayout:{boss}] Ignoring \"{entry.Trim()}\", expected Account=Class.", "Warning");
                continue;
            }
            classByUser[user] = cls;
        }
        return classByUser;
    }

    /// <summary>True when every class in the layout is one of <paramref name="classes"/>.</summary>
    public bool Uses(IEnumerable<string> classes) =>
        IsSet && ClassByUser.Values.All(cls => classes.Contains(cls, StringComparer.OrdinalIgnoreCase));

    /// <summary>
    /// Equips this account's class and returns it, or returns "" after stopping the bot.
    /// With a layout: the class the layout names for this login, which must be one of
    /// <paramref name="classSlots"/> unless <paramref name="anyClass"/>. Without one:
    /// the automatic <see cref="UltraCustomClassSync"/> assignment over <paramref name="classSlots"/>.
    /// </summary>
    public string EquipClass(dynamic ultra, string[][] classSlots, int armySize, string syncFilePath, bool allowDuplicates = false, bool anyClass = false)
    {
        if (!IsSet)
        {
            Class = UltraCustomClassSync.CustomClassSync(ultra, Bot, classSlots, armySize, syncFilePath, allowDuplicates);
            return Class;
        }

        string user = Bot.Player.Username ?? string.Empty;
        if (!ClassByUser.TryGetValue(user, out string? wanted))
            return Stop($"{user} has no class in the {Boss} layout. Add \"{user}=Class\" to the {OptionName(Boss)} option or clear it.");

        string? cls = classSlots.SelectMany(s => s).FirstOrDefault(c => c.Equals(wanted, StringComparison.OrdinalIgnoreCase));
        if (cls == null)
        {
            if (!anyClass)
                return Stop($"{wanted} has no role in {Boss}. Roles take: {string.Join(", ", classSlots.SelectMany(s => s).Distinct())}.");
            cls = wanted;
        }

        if (!C.CheckInventory(cls, toInv: true))
            return Stop($"{user} does not own {cls}.");

        Class = cls;
        C.Logger($"[PartyLayout:{Boss}] {user} plays {Class}.");
        return EnsureClass() ? Class : string.Empty;
    }

    /// <summary>
    /// Puts <see cref="Class"/> back on if something swapped it, e.g. potion-reagent farming
    /// equipping the farm class. Call it right before joining the boss map.
    /// </summary>
    public bool EnsureClass()
    {
        if (string.IsNullOrEmpty(Class))
            return false;

        for (int i = 0; i < 5 && !IsOnClass() && !Bot.ShouldExit; i++)
        {
            C.Logger($"[PartyLayout:{Boss}] On {Bot.Player.CurrentClass?.Name ?? "no class"}, equipping {Class}.");
            if (Bot.Map.Name?.Equals("house", StringComparison.OrdinalIgnoreCase) == true)
                C.Join("whitemap");
            C.Equip(Class);
            Bot.Wait.ForTrue(IsOnClass, 10);
        }

        if (IsOnClass())
            return true;

        Stop($"Could not equip {Class}.");
        return false;
    }

    private bool IsOnClass() =>
        string.Equals(Bot.Player.CurrentClass?.Name, Class, StringComparison.OrdinalIgnoreCase);

    private string Stop(string message)
    {
        C.Logger($"[PartyLayout:{Boss}] {message}", "Error", messageBox: true, stopBot: true);
        return string.Empty;
    }
}
