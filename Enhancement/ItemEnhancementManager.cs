/*
name: Item Enhancement Manager
description: Unbanks and enhances selected weapons, helms, and capes, leaving them in inventory. Bypasses Disable AutoEnhance.
tags: enhancement, unbank, weapon, helm, cape, inventory
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
using System;
using System.Collections.Generic;
using System.Linq;
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

#nullable enable

public class ItemEnhancementManager
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private CoreAdvanced Adv => CoreAdvanced.Instance;

    // Retain the prototype storage key so existing item selections are preserved.
    public string OptionsStorage = "ItemEnhancementPrototype";
    public bool DontPreconfigure = true;
    public string[] MultiOptions = { "Weapon", "Helm", "Cape", "Skip_Options" };

    public List<IOption> Skip_Options = new()
    {
        CoreBots.Instance.SkipOptions,
    };

    public List<IOption> Weapon = new()
    {
        new Option<string>("WeaponItem1", "Item 1", "Weapon name. Leave blank to skip.", string.Empty),
        new Option<WeaponEnhancement>("WeaponEnhancement1", "Enhancement 1", "Enhancement for Item 1.", WeaponEnhancement.None),
        new Option<string>("WeaponItem2", "Item 2", "Weapon name. Leave blank to skip.", string.Empty),
        new Option<WeaponEnhancement>("WeaponEnhancement2", "Enhancement 2", "Enhancement for Item 2.", WeaponEnhancement.None),
        new Option<string>("WeaponItem3", "Item 3", "Weapon name. Leave blank to skip.", string.Empty),
        new Option<WeaponEnhancement>("WeaponEnhancement3", "Enhancement 3", "Enhancement for Item 3.", WeaponEnhancement.None),
        new Option<string>("WeaponItem4", "Item 4", "Weapon name. Leave blank to skip.", string.Empty),
        new Option<WeaponEnhancement>("WeaponEnhancement4", "Enhancement 4", "Enhancement for Item 4.", WeaponEnhancement.None),
        new Option<string>("WeaponItem5", "Item 5", "Weapon name. Leave blank to skip.", string.Empty),
        new Option<WeaponEnhancement>("WeaponEnhancement5", "Enhancement 5", "Enhancement for Item 5.", WeaponEnhancement.None),
    };

    public List<IOption> Helm = new()
    {
        new Option<string>("HelmItem1", "Item 1", "Helm name. Leave blank to skip.", string.Empty),
        new Option<HelmEnhancement>("HelmEnhancement1", "Enhancement 1", "Enhancement for Item 1.", HelmEnhancement.None),
        new Option<string>("HelmItem2", "Item 2", "Helm name. Leave blank to skip.", string.Empty),
        new Option<HelmEnhancement>("HelmEnhancement2", "Enhancement 2", "Enhancement for Item 2.", HelmEnhancement.None),
        new Option<string>("HelmItem3", "Item 3", "Helm name. Leave blank to skip.", string.Empty),
        new Option<HelmEnhancement>("HelmEnhancement3", "Enhancement 3", "Enhancement for Item 3.", HelmEnhancement.None),
        new Option<string>("HelmItem4", "Item 4", "Helm name. Leave blank to skip.", string.Empty),
        new Option<HelmEnhancement>("HelmEnhancement4", "Enhancement 4", "Enhancement for Item 4.", HelmEnhancement.None),
        new Option<string>("HelmItem5", "Item 5", "Helm name. Leave blank to skip.", string.Empty),
        new Option<HelmEnhancement>("HelmEnhancement5", "Enhancement 5", "Enhancement for Item 5.", HelmEnhancement.None),
    };

    public List<IOption> Cape = new()
    {
        new Option<string>("CapeItem1", "Item 1", "Cape name. Leave blank to skip.", string.Empty),
        new Option<CapeEnhancement>("CapeEnhancement1", "Enhancement 1", "Enhancement for Item 1.", CapeEnhancement.None),
        new Option<string>("CapeItem2", "Item 2", "Cape name. Leave blank to skip.", string.Empty),
        new Option<CapeEnhancement>("CapeEnhancement2", "Enhancement 2", "Enhancement for Item 2.", CapeEnhancement.None),
        new Option<string>("CapeItem3", "Item 3", "Cape name. Leave blank to skip.", string.Empty),
        new Option<CapeEnhancement>("CapeEnhancement3", "Enhancement 3", "Enhancement for Item 3.", CapeEnhancement.None),
        new Option<string>("CapeItem4", "Item 4", "Cape name. Leave blank to skip.", string.Empty),
        new Option<CapeEnhancement>("CapeEnhancement4", "Enhancement 4", "Enhancement for Item 4.", CapeEnhancement.None),
        new Option<string>("CapeItem5", "Item 5", "Cape name. Leave blank to skip.", string.Empty),
        new Option<CapeEnhancement>("CapeEnhancement5", "Enhancement 5", "Enhancement for Item 5.", CapeEnhancement.None),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        if (!bot.Config!.Get<bool>(Core.SkipOptions))
            bot.Config.Configure();
        if (Bot.ShouldExit)
            return;

        List<string> itemNames = Weapon.Concat(Helm).Concat(Cape)
            .Where(option => option.Type == typeof(string))
            .Select(option => (Bot.Config.Get<string>(option) ?? string.Empty).Trim())
            .Where(name => name.Length > 0)
            .ToList();
        Core.BankingBlackList.AddRange(itemNames);
        Core.BankingBlackList.AddRange(Bot.Inventory.Items
            .Where(item => itemNames.Any(name => string.Equals(item.Name?.Trim(), name, StringComparison.OrdinalIgnoreCase)))
            .Select(item => item.Name));

        Core.SetOptions(disableClassSwap: true);
        try
        {
            Run();
        }
        finally
        {
            Core.SetOptions(false);
        }
    }

    private void Run()
    {
        foreach (string category in MultiOptions)
        {
            if (category == "Skip_Options")
                continue;

            for (int index = 1; index <= 5; index++)
            {
                if (Bot.ShouldExit)
                    return;

                string itemName = (Bot.Config!.Get<string>(category, category + "Item" + index) ?? string.Empty).Trim();
                if (itemName.Length == 0)
                    continue;

                string selection;
                if (category == "Weapon")
                    selection = Bot.Config.Get<WeaponEnhancement>(category, category + "Enhancement" + index).ToString();
                else if (category == "Helm")
                    selection = Bot.Config.Get<HelmEnhancement>(category, category + "Enhancement" + index).ToString();
                else
                    selection = Bot.Config.Get<CapeEnhancement>(category, category + "Enhancement" + index).ToString();

                if (selection == "None")
                    continue;

                InventoryItem? item = Bot.Inventory.Items.Concat(Bot.Bank.Items).FirstOrDefault(candidate =>
                    candidate != null && string.Equals(candidate.Name?.Trim(), itemName, StringComparison.OrdinalIgnoreCase));
                string logName = new string((item?.Name ?? itemName)
                    .Where(character => character >= ' ' && character <= '~' && "%<>&\"'".IndexOf(character) < 0)
                    .ToArray());
                if (item == null)
                {
                    Core.Logger($"{category} item {index} - {logName} was not found in inventory or bank. Check the configured name.");
                    continue;
                }

                if ((category == "Weapon" && item.ItemGroup != "Weapon")
                    || (category == "Helm" && item.Category != ItemCategory.Helm)
                    || (category == "Cape" && item.Category != ItemCategory.Cape))
                {
                    Core.Logger($"{category} item {index} - {logName} [ID {item.ID}] is in the wrong equipment section. Skipping.");
                    continue;
                }

                bool regular = Enum.TryParse(selection, out EnhancementType type)
                    && Enum.IsDefined(typeof(EnhancementType), type);
                WeaponSpecial weapon = WeaponSpecial.None;
                HelmSpecial helm = HelmSpecial.None;
                CapeSpecial cape = CapeSpecial.None;
                if (!regular)
                {
                    type = EnhancementType.Lucky;
                    if (category == "Weapon")
                    {
                        if (!Enum.TryParse(selection, out weapon))
                            continue;
                        if (weapon == WeaponSpecial.Forge)
                        {
                            Core.Logger($"Weapon item {index} - {logName} [ID {item.ID}]: Forge is not supported by the current Core enhancement route. Skipping.");
                            continue;
                        }
                        if ((int)weapon >= 2 && (int)weapon <= 6 && !Adv.uAwe())
                        {
                            Core.Logger($"Weapon item {index} - {logName} [ID {item.ID}] requires unlocked Awe enhancements. Skipping.");
                            continue;
                        }
                        if (Enum.IsDefined(typeof(EnhancementType), item.EnhancementPatternID))
                            type = (EnhancementType)item.EnhancementPatternID;
                    }
                    else if (category == "Helm")
                    {
                        if (!Enum.TryParse(selection, out helm))
                            continue;
                    }
                    else if (!Enum.TryParse(selection, out cape))
                        continue;
                }

                Core.BankingBlackList.Add(item.Name);
                int itemID = item.ID;
                Core.Unbank(itemID);
                item = Bot.Inventory.Items.FirstOrDefault(candidate => candidate.ID == itemID);
                if (item == null)
                {
                    Core.Logger($"{category} item {index} - {logName} [ID {itemID}] could not be moved into inventory. Skipping.");
                    continue;
                }

                Core.Logger($"Applying {selection.Replace('_', ' ')} to {category} item {index} - {logName} [ID {itemID}].");
                Adv.EnhanceItem(item.Name, type, cape, helm, weapon, logging: true, bypassDisableAutoEnhance: true);
                bool applied = Bot.Wait.ForTrue(() =>
                {
                    InventoryItem? updated = Bot.Inventory.Items.FirstOrDefault(candidate => candidate.ID == itemID);
                    if (updated == null || updated.EnhancementLevel <= 0)
                        return false;
                    if (weapon != WeaponSpecial.None)
                        return Core.GetItemProperty<int>(updated, "ProcID") == (int)weapon;
                    if (helm != HelmSpecial.None)
                        return updated.EnhancementPatternID == (int)helm;
                    if (cape != CapeSpecial.None)
                        return updated.EnhancementPatternID == (int)cape;
                    return updated.EnhancementPatternID == (int)type;
                }, 10);

                if (applied)
                    Core.Logger($"Verified {category} item {index} - {logName} [ID {itemID}]. The item remains in inventory.");
                else
                    Core.Logger($"Could not verify the selected enhancement for {category} item {index} - {logName} [ID {itemID}]. Check the Core enhancement logs.");
            }
        }

        Core.Logger("Item enhancement pass finished.");
    }

    public enum WeaponEnhancement
    {
        None,
        Forge,
        Lacerate,
        Smite,
        Valiance,
        Arcanas_Concerto,
        Acheron,
        Elysium,
        Praxis,
        Dauntless,
        Ravenous,
        Spiral_Carve,
        Awe_Blast,
        Health_Vamp,
        Mana_Vamp,
        Powerword_Die,
        Fighter,
        Thief,
        Hybrid,
        Wizard,
        Healer,
        SpellBreaker,
        Lucky,
    }

    public enum HelmEnhancement
    {
        None,
        Forge,
        Vim,
        Examen,
        Anima,
        Pneuma,
        Hearty,
        Fighter,
        Thief,
        Hybrid,
        Wizard,
        Healer,
        SpellBreaker,
        Lucky,
    }

    public enum CapeEnhancement
    {
        None,
        Forge,
        Absolution,
        Avarice,
        Vainglory,
        Penitence,
        Lament,
        Fighter,
        Thief,
        Hybrid,
        Wizard,
        Healer,
        SpellBreaker,
        Lucky,
    }
}
