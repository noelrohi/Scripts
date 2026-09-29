/*
name: Yulgars Burgers Merge
description: Farms the Yulgars Burgers Merge [1884] in /borgars.
tags: borgars, merge, yulgars, burgers, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class YulgarsBurgers
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
            "Burger Buns",
            "Cowbell",
            "Fish Oil",
            "Sugar Cubes",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        // FILL_QUEST_UNLOCK: Add the story call that completes "An Aspiring Burgermonger" [7521] to unlock "Burglinster's Revenge" [7522].
        Adv.StartBuyAllMerge("borgars", 1884, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Burger Buns":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(7522);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.EquipClass(ClassType.Solo);
                        Core.HuntMonster("borgars", "Burglinster", "Burglinster Cured", 1);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Cowbell":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(7511);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.Logger("Cannot automate Heavy Cream [54722] for Creamy! [7511] from this map.", messageBox: true, stopBot: true);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Fish Oil":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(7514);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.Logger("Cannot automate Fishwing Filet [54725] for Filet o' Fishwing [7514] from this map.", messageBox: true, stopBot: true);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Sugar Cubes":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(7512);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
                        Core.Logger("Cannot automate Sugar [54723] for Sacchar-Imp [7512] from this map.", messageBox: true, stopBot: true);
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
        new Option<bool>("54738", "ChickencowMancer", "Mode: [select] only\nShould the bot buy \"ChickencowMancer\" ?", false),
        new Option<bool>("54646", "BorgarMonger", "Mode: [select] only\nShould the bot buy \"BorgarMonger\" ?", false),
        new Option<bool>("54651", "Dual Borgars", "Mode: [select] only\nShould the bot buy \"Dual Borgars\" ?", false),
        new Option<bool>("54780", "Burger and Spatula", "Mode: [select] only\nShould the bot buy \"Burger and Spatula\" ?", false),
        new Option<bool>("54647", "BorgarMonger Hair", "Mode: [select] only\nShould the bot buy \"BorgarMonger Hair\" ?", false),
        new Option<bool>("54652", "BorgarMonger Morph Locks", "Mode: [select] only\nShould the bot buy \"BorgarMonger Morph Locks\" ?", false),
        new Option<bool>("54740", "Chickencowl + Locks", "Mode: [select] only\nShould the bot buy \"Chickencowl + Locks\" ?", false),
        new Option<bool>("54741", "ChickenCowl", "Mode: [select] only\nShould the bot buy \"ChickenCowl\" ?", false),
        new Option<bool>("54648", "BorgarMonger Hat", "Mode: [select] only\nShould the bot buy \"BorgarMonger Hat\" ?", false),
        new Option<bool>("54649", "Triple BorgarMonger Hat", "Mode: [select] only\nShould the bot buy \"Triple BorgarMonger Hat\" ?", false),
        new Option<bool>("54782", "Bowl of Salad", "Mode: [select] only\nShould the bot buy \"Bowl of Salad\" ?", false),
        new Option<bool>("54758", "Chicken Fingers", "Mode: [select] only\nShould the bot buy \"Chicken Fingers\" ?", false),
        new Option<bool>("54650", "Borgar", "Mode: [select] only\nShould the bot buy \"Borgar\" ?", false),
        new Option<bool>("54781", "Slice of Cake", "Mode: [select] only\nShould the bot buy \"Slice of Cake\" ?", false),
        new Option<bool>("54744", "ChickencowMancer's Staff", "Mode: [select] only\nShould the bot buy \"ChickencowMancer's Staff\" ?", false),
        new Option<bool>("54745", "ChickencowMancer's Blade", "Mode: [select] only\nShould the bot buy \"ChickencowMancer's Blade\" ?", false),
    ];
}
