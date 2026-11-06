/*
name: Hunters Reward Merge
description: Farms the Hunters Reward Merge [2781] in /shadowslayerwar.
tags: shadowslayerwar, merge, hunters, reward, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Mogloween/CoreMogloween.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class HuntersReward
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
    private static CoreMogloween CoreMogloween
    {
        get => _CoreMogloween ??= new CoreMogloween();
        set => _CoreMogloween = value;
    }
    private static CoreMogloween _CoreMogloween;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Darkovian Nobility Hair",
            "Darkovian Nobility Locks",
            "Darkovian Pup Hair",
            "Darkovian Pup Locks",
            "Silver Stake",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
        CoreMogloween.BirgittaMogloween();
		CoreMogloween.ShadowSlayerWar();
        Adv.StartBuyAllMerge("shadowslayerwar", 2781, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Darkovian Nobility Hair":
                case "Darkovian Nobility Locks":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("shadowslayerwar", "Enthralled Hunter", req.Name, quant, req.Temp);
                    break;
                case "Darkovian Pup Hair":
                case "Darkovian Pup Locks":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("shadowslayerwar", "Darkovia Hunter", req.Name, quant, req.Temp);
                    break;
                case "Silver Stake":
                    Core.EquipClass(ClassType.Farm);
					Core.RegisterQuests(10908, 10909);
					while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
						Core.KillMonster("shadowslayerwar", "r11", "Left", "*");
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
        new Option<bool>("101382", "Neo ShadowSlayer", "Mode: [select] only\nShould the bot buy \"Neo ShadowSlayer\" ?", false),
        new Option<bool>("104615", "Sanguine Endura ShadowSlayer", "Mode: [select] only\nShould the bot buy \"Sanguine Endura ShadowSlayer\" ?", false),
        new Option<bool>("101385", "ShadowSlayer's Duster", "Mode: [select] only\nShould the bot buy \"ShadowSlayer's Duster\" ?", false),
        new Option<bool>("101386", "Shadowslayer's Trench Coat", "Mode: [select] only\nShould the bot buy \"Shadowslayer's Trench Coat\" ?", false),
        new Option<bool>("104618", "Back-up Painkiller Mk.II", "Mode: [select] only\nShould the bot buy \"Back-up Painkiller Mk.II\" ?", false),
        new Option<bool>("104620", "Dual Moonspring Katana", "Mode: [select] only\nShould the bot buy \"Dual Moonspring Katana\" ?", false),
        new Option<bool>("104623", "Thorn Catharsis Chainblades", "Mode: [select] only\nShould the bot buy \"Thorn Catharsis Chainblades\" ?", false),
        new Option<bool>("103351", "Darkovian Nobility Morph", "Mode: [select] only\nShould the bot buy \"Darkovian Nobility Morph\" ?", false),
        new Option<bool>("103352", "Darkovian Nobility Visage", "Mode: [select] only\nShould the bot buy \"Darkovian Nobility Visage\" ?", false),
        new Option<bool>("103356", "Darkovian Pup Morph", "Mode: [select] only\nShould the bot buy \"Darkovian Pup Morph\" ?", false),
        new Option<bool>("103358", "Darkovian Pup Visage", "Mode: [select] only\nShould the bot buy \"Darkovian Pup Visage\" ?", false),
        new Option<bool>("104616", "Sanguine Endura Hat", "Mode: [select] only\nShould the bot buy \"Sanguine Endura Hat\" ?", false),
        new Option<bool>("104617", "Sanguine Endura Cap", "Mode: [select] only\nShould the bot buy \"Sanguine Endura Cap\" ?", false),
        new Option<bool>("101383", "Contemporary Hunter's Fedora", "Mode: [select] only\nShould the bot buy \"Contemporary Hunter's Fedora\" ?", false),
        new Option<bool>("101384", "Contemporary Hunter's Cap", "Mode: [select] only\nShould the bot buy \"Contemporary Hunter's Cap\" ?", false),
        new Option<bool>("104621", "BloodRain Royale", "Mode: [select] only\nShould the bot buy \"BloodRain Royale\" ?", false),
        new Option<bool>("104619", "Moonspring Katana", "Mode: [select] only\nShould the bot buy \"Moonspring Katana\" ?", false),
        new Option<bool>("104622", "Thorn Catharsis Chainblade", "Mode: [select] only\nShould the bot buy \"Thorn Catharsis Chainblade\" ?", false),
    ];
}
