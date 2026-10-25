/*
name: SeaVoice Merge
description: This bot will farm the items belonging to the selected mode for the SeaVoice Merge [2320] in /seavoice
tags: seavoice, merge, seavoice, midnight, glaucus, sage, mystic, morph, companion, abyssal, atlanticus, trident
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
using System.Dynamic;
using Newtonsoft.Json;
using Skua.Core.Interfaces;
using Skua.Core.Models.Auras;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Models.Skills;
using Skua.Core.Options;

public class SeaVoiceMerge
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private readonly CoreAOR AOR = new();
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv
    {
        get => _sAdv ??= new CoreAdvanced();
        set => _sAdv = value;
    }
    private static CoreAdvanced _sAdv;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;

    // [Can Change] This should only be changed by the author.
    //              If true, it will not stop the script if the default case triggers and the user chose to only get mats
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        ProtectRequiredItems();
        Core.SetOptions();

        BuyAllMerge();
        Core.SetOptions(false);
    }

    private void ProtectRequiredItems()
    {
        string[] items =
        {
            "Algal Bloom",
            "Bioluminessence",
            "Dark Elf Pearl",
            "Glaucus Mystic",
            "Water Elf Pearl",
            "Water Elf Antler",
            "Glaucus Companion",
            "Sundered Tentacle",
            "Calamity Atlanticus Trident",
            "Glaucus Sage",
            "Midnight Glaucus Mystic",
            "Midnight Glaucus Sage",
        };

        foreach (string item in items)
        {
            if (!Core.BankingBlackList.Contains(item))
                Core.BankingBlackList.Add(item);
        }
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        ProtectRequiredItems();
        //Only edit the map and shopID here
        Adv.StartBuyAllMerge("seavoice", 2320, findIngredients, buyOnlyThis, buyMode: buyMode);

        #region Dont edit this part
        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            int currentQuant = req.Temp
                ? Bot.TempInv.GetQuantity(req.Name)
                : Bot.Inventory.GetQuantity(req.Name);
            if (req == null)
            {
                Core.Logger("req is NULL");
                return;
            }

            switch (req.Name)
            {
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger(
                        $"The bot hasn't been taught how to get {req.Name}."
                            + (shouldStop ? " Please report the issue." : " Skipping"),
                        messageBox: shouldStop,
                        stopBot: shouldStop
                    );
                    break;
        #endregion

                case "Bioluminessence":
                case "Calamity Atlanticus Trident":
                case "Glaucus Mystic":
                case "Glaucus Companion":
                    Core.FarmingLogger(req.Name, req.Quantity);
                    AttackVoiceInTheSea(req.Name, quant);
                    break;

                case "Dark Elf Pearl":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9339);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("trenchobserve", "Lady Noelle", "Noelle's Brooch", log: false);
                        Core.HuntMonster("trenchobserve", "Sea Spirit", "Green Sea Jelly", 2, log: false);
                        Core.HuntMonster("trenchobserve", "Necro Adipocere", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Water Elf Pearl":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9302);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("midnightzone", "Shadow Viscera", "Fleshy Shadows", 8, log: false);
                        Core.HuntMonster("midnightzone", "Venerated Wraith", "Wraith Memento", 8, log: false);
                        Core.HuntMonster("midnightzone", "Sparagmos", "Memory Card", log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Water Elf Antler":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(9316);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("abyssalzone", "The Ashray", "Ashray Artifacts", log: false);
                        Core.HuntMonster("abyssalzone", "Necro Adipocere", "Adipocere Antler", 3, log: false);
                        Core.HuntMonster("abyssalzone", "Foam Scavenger", "Scavenger Foam", 6, log: false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;

                case "Sundered Tentacle":
                    Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Farm);
                    Core.RegisterQuests(9269);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster("twilightzone", "Leviathan", "Leviathan Tentacle", 1, true, false);
                        Core.HuntMonster("twilightzone", "Decay Spirit", "Decay Essence", 8, true, false);
                        Core.HuntMonster("twilightzone", "Ice Guardian", "Tarnished Icicle", 8, true, false);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
            }
        }
    }

    public void AttackVoiceInTheSea(string itemName, int quant)
    {
        string[] PossibleSoloClasses = new[]
        {
            "ArchPaladin",
        };

        bool bestGearDisabled = Core.CBOBool("DisableBestGear", out bool disableBestGear) && disableBestGear;
        if (!bestGearDisabled && !Core.CheckInventory(PossibleSoloClasses, any: true))
            Core.Logger("ArchPaladin not owned. Continuing with configured Solo class.");

        Core.RegisterQuests(9349);
        Core.EquipClass(ClassType.Solo);
        Core.AddDrop("Algal Bloom", itemName);
        Core.Unbank("Algal Bloom", itemName);
        Adv.GearStore(EnhAfter: true);

        string? selectedClass = Bot.Player.CurrentClass?.Name;
        if (!bestGearDisabled)
        {
            selectedClass = PossibleSoloClasses.FirstOrDefault(className =>
                Bot.Inventory.Items.Any(item => item.Name == className)
                || Bot.Bank.Items.Any(item => item.Name == className)
            ) ?? selectedClass;
        }
        if (string.IsNullOrWhiteSpace(selectedClass))
        {
            Core.Logger("No soloing class found and nothing equipped; aborting SeaVoice.");
            return;
        }

        Core.Logger($"Fighting Voice in the Sea with {selectedClass}");
        Adv.SmartEnhance(selectedClass);
        Core.Equip(selectedClass);
        AOR.FightVoiceInTheSea(itemName, quant);
        Adv.GearStore(true, EnhAfter: true);
        Core.CancelRegisteredQuests();
    }

    public List<IOption> Select = new()
    {
        new Option<bool>(
            "79161",
            "Midnight Glaucus Sage",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Sage\" ?",
            false
        ),
        new Option<bool>(
            "79160",
            "Midnight Glaucus Mystic",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Mystic\" ?",
            false
        ),
        new Option<bool>(
            "79154",
            "Glaucus Sage",
            "Mode: [select] only\nShould the bot buy \"Glaucus Sage\" ?",
            false
        ),
        new Option<bool>(
            "79165",
            "Midnight Glaucus Locks",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Locks\" ?",
            false
        ),
        new Option<bool>(
            "79164",
            "Midnight Glaucus Hair",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Hair\" ?",
            false
        ),
        new Option<bool>(
            "79163",
            "Midnight Glaucus Visage",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Visage\" ?",
            false
        ),
        new Option<bool>(
            "79162",
            "Midnight Glaucus Morph",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Morph\" ?",
            false
        ),
        new Option<bool>(
            "79156",
            "Glaucus Visage",
            "Mode: [select] only\nShould the bot buy \"Glaucus Visage\" ?",
            false
        ),
        new Option<bool>(
            "79155",
            "Glaucus Morph",
            "Mode: [select] only\nShould the bot buy \"Glaucus Morph\" ?",
            false
        ),
        new Option<bool>(
            "79166",
            "Midnight Glaucus Companion",
            "Mode: [select] only\nShould the bot buy \"Midnight Glaucus Companion\" ?",
            false
        ),
        new Option<bool>(
            "79167",
            "Abyssal Atlanticus Trident",
            "Mode: [select] only\nShould the bot buy \"Abyssal Atlanticus Trident\" ?",
            false
        ),
    };
}
