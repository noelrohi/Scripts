/*
name: Divorares Hoard Merge
description: Farms the Divorares Hoard Merge [2777] in /divorarepass.
tags: divorarepass, merge, divorares, hoard, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class DivoraresHoard
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Confiscated Harp String",
            "Divorare Horn",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Adv.StartBuyAllMerge("divorarepass", 2777, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Confiscated Harp String":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10881);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Farm);
                        Core.HuntMonster("divorarepass", "Nazarene's Artist", "Moon Dust Pigment", 12);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Divorare Horn":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(10882);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("divorarepass", "Divorare", "Divorare Blood Sample", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                default:
                    bool shouldStop = !Adv.matsOnly || !dontStopMissingIng;
                    Core.Logger($"The bot hasn't been taught how to get {req.Name}.", messageBox: shouldStop, stopBot: shouldStop);
                    break;
            }
        }
    }

    public List<IOption> Select =
    [
        new Option<bool>("102129", "Divorare ShadowSlayer (Armor)", "Mode: [select] only\nShould the bot buy \"Divorare ShadowSlayer (Armor)\" ?", false),
        new Option<bool>("102128", "Chosen Divorare ShadowSlayer", "Mode: [select] only\nShould the bot buy \"Chosen Divorare ShadowSlayer\" ?", false),
        new Option<bool>("102142", "Dual Divorare's Hunger", "Mode: [select] only\nShould the bot buy \"Dual Divorare's Hunger\" ?", false),
        new Option<bool>("102143", "Divorare's Pseudo-Talons", "Mode: [select] only\nShould the bot buy \"Divorare's Pseudo-Talons\" ?", false),
        new Option<bool>("102144", "Divorare's Pseudo-Claws", "Mode: [select] only\nShould the bot buy \"Divorare's Pseudo-Claws\" ?", false),
        new Option<bool>("102130", "Divorare ShadowSlayer Morph", "Mode: [select] only\nShould the bot buy \"Divorare ShadowSlayer Morph\" ?", false),
        new Option<bool>("102131", "Divorare ShadowSlayer Visage", "Mode: [select] only\nShould the bot buy \"Divorare ShadowSlayer Visage\" ?", false),
        new Option<bool>("102132", "Divorare ShadowSlayer Scarf", "Mode: [select] only\nShould the bot buy \"Divorare ShadowSlayer Scarf\" ?", false),
        new Option<bool>("102133", "Divorare ShadowSlayer Sash", "Mode: [select] only\nShould the bot buy \"Divorare ShadowSlayer Sash\" ?", false),
        new Option<bool>("102134", "Divorare ShadowSlayer Hat", "Mode: [select] only\nShould the bot buy \"Divorare ShadowSlayer Hat\" ?", false),
        new Option<bool>("102135", "Divorare ShadowSlayer (Helm)", "Mode: [select] only\nShould the bot buy \"Divorare ShadowSlayer (Helm)\" ?", false),
        new Option<bool>("102141", "Divorare's Hunger", "Mode: [select] only\nShould the bot buy \"Divorare's Hunger\" ?", false),
    ];
}
