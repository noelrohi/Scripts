/*
name: Cysero Merge
description: Farms the Cysero Merge [668] in /battleontown.
tags: battleontown, merge, cysero, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Other/MergeShops/YulgarsBurgers.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class CyseroMergeShop
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static YulgarsBurgers Burgers { get => _Burgers ??= new YulgarsBurgers(); set => _Burgers = value; }
    private static YulgarsBurgers _Burgers;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Barrel",
            "Basic Wooden Stake",
            "Black Knight's Nail",
            "Borgar",
            "Glass Bottle",
            "Glowing Sock",
            "New Home Weapon Display",
            "Real Rubber Ducky",
            "Staked Steak",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        Adv.StartBuyAllMerge("battleontown", 668, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Black Knight's Nail":
                    Core.HuntMonster("greenguardwest", "Black Knight", req.Name, req.Quantity, req.Temp);
                    break;

                case "Borgar":
                    Burgers.BuyAllMerge(req.Name);
                    break;
                case "Barrel":
                    Adv.BuyItem("artixpointe", 999, req.Name, req.Quantity);
                    Bot.Wait.ForPickup(req.Name);
                    break;
                case "Glass Bottle":
                    if (!Core.IsMember)
                    {
                        Core.Logger($"{req.Name} Requires membership");
                        return;
                    }
                    Adv.BuyItem("buyhouse", 1366, req.Name, req.Quantity);
                    Bot.Wait.ForPickup(req.Name);
                    break;
                case "New Home Weapon Display":
                    Core.KillMonster("cyserowed", "r1", "Down", "*", req.Name);
                    Bot.Wait.ForPickup(req.Name);
                    break;
                case "Staked Steak":
                case "Basic Wooden Stake":
                    Adv.BuyItem("darkoviaforest", 138, req.Name, req.Quantity);
                    break;
                case "Real Rubber Ducky":
                    if (!Core.IsMember)
                    {
                        Core.Logger($"{req.Name} Requires membership");
                        return;
                    }
                    Core.AddDrop(req.ID);
                    Core.EnsureAccept(2776);
                    Core.HuntMonster("river", "River Fishman", "Yellow Rubber Duck", 3);
                    Core.HuntMonster("marsh", "Dark Witch", "Red Rubber Duck", 3);
                    Core.HuntMonster("shallow", "Water Elemental", "Blue Rubber Duck", 3);
                    Core.EnsureComplete(2776);
                    Bot.Wait.ForPickup(req.ID);
                    break;
                case "Glowing Sock":
                    Core.FarmingLogger(req.Name, quant);
                    Core.RegisterQuests(2777);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        Core.HuntMonster("greenguardwest", "Slime", "Slimy Lost Sock", 5, true, false);
                        Core.HuntMonster("greenguardeast", "Wolf", "Furry Lost Sock", 2, true, false);
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
        new Option<bool>("18396", "Mad Magic Manawalker", "Mode: [select] only\nShould the bot buy \"Mad Magic Manawalker\" ?", false),
        new Option<bool>("18434", "Living Yogurt Warrior", "Mode: [select] only\nShould the bot buy \"Living Yogurt Warrior\" ?", false),
        new Option<bool>("44109", "Gilded Rainbow Wrap", "Mode: [select] only\nShould the bot buy \"Gilded Rainbow Wrap\" ?", false),
        new Option<bool>("18439", "Berry Tasty Helm", "Mode: [select] only\nShould the bot buy \"Berry Tasty Helm\" ?", false),
        new Option<bool>("18397", "Mad Magic Mana Helm", "Mode: [select] only\nShould the bot buy \"Mad Magic Mana Helm\" ?", false),
        new Option<bool>("80008", "Cysero's Design Dais", "Mode: [select] only\nShould the bot buy \"Cysero's Design Dais\" ?", false),
        new Option<bool>("80009", "Cysero's Showcase Stage", "Mode: [select] only\nShould the bot buy \"Cysero's Showcase Stage\" ?", false),
        new Option<bool>("18421", "Green Sockatana", "Mode: [select] only\nShould the bot buy \"Green Sockatana\" ?", false),
    ];
}
