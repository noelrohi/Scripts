/*
name: Walpurga Witch Wares Merge
description: Farms the Walpurga Witch Wares Merge [2780] in /birgittamogloween.
tags: birgittamogloween, merge, walpurga, witch, wares, merge
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Seasonal/Mogloween/CoreMogloween.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Monsters;
using Skua.Core.Options;

public class WalpurgaWitchWares
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
	private static CoreMogloween Mog{ get => _CoreMogloween ??= new CoreMogloween(); set => _CoreMogloween = value; }
    private static CoreMogloween _CoreMogloween;

    public bool DontPreconfigure = true;
    public List<IOption> Generic = sAdv.MergeOptions;
    public string[] MultiOptions = { "Generic", "Select" };
    public string OptionsStorage = sAdv.OptionsStorage;
    private bool dontStopMissingIng = false;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.BankingBlackList.AddRange([
            "Gothic Guest Hair",
            "Gothic Guest Locks",
            "Hollyhock Brew",
            "Owl's Dark Guise",
            "Poltergeistcorn Snack",
            "Poltergeistcorn Snacks",
            "Roosting Owl",
            "Rufous Wisdom Staff",
            "Sinema Snack",
            "Sinema Snacks",
            "Stikini Witch Visage",
            "Strikini Witch Morph",
            "Tiny Pink Princess Cupcake",
            "Tiny Pink Princess Cupcakes",
            "Upsizer",
        ]);
        Core.SetOptions();
        BuyAllMerge();
        Core.SetOptions(false);
    }

    public void BuyAllMerge(string? buyOnlyThis = null, mergeOptionsEnum? buyMode = null)
    {
		Mog.BirgittaMogloween();
        Adv.StartBuyAllMerge("birgittamogloween", 2780, findIngredients, buyOnlyThis, buyMode: buyMode);

        void findIngredients()
        {
            ItemBase req = Adv.externalItem;
            int quant = Adv.externalQuant;
			int currentQuant = req.Temp
                ? Bot.TempInv.GetQuantity(req.Name)
                : Bot.Inventory.GetQuantity(req.Name);
            if (req == null)
                return;

            switch (req.Name)
            {
                case "Gothic Guest Hair":
                case "Gothic Guest Locks":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("birgittamogloween", "ShadowSlayer", req.Name, quant, req.Temp);
                    break;
                case "Hollyhock Brew":
                    Core.FarmingLogger(req.Name, quant);
					Core.EquipClass(ClassType.Solo);
					Monster? ensis;
                    Monster? grigor;
                    Core.RegisterQuests(Core.IsMember ? 10907 : 10906);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.ID, quant))
                    {
						if (Bot.Map.Name != "birgittamogloween")
                            Core.Join("birgittamogloween");

                        if (Bot.Player.Cell != "r6")
                            Core.Jump("r6", "Left");

                        ensis = Bot.Monsters.CurrentAvailableMonsters?.FirstOrDefault(m => m?.Alive == true && m.MapID == 16);
                        grigor = Bot.Monsters.CurrentAvailableMonsters?.FirstOrDefault(m => m?.Alive == true && m.MapID == 17);
						
						if (Bot.TempInv.Contains("Cor Mortis Cosage") && ensis != null)
                            Bot.Combat.Attack(16);

                        else if (Bot.TempInv.Contains("Grigor's Silver Tooth") && grigor != null)
                            Bot.Combat.Attack(17);

                        // If both drops owned OR priority mob dead → kill the other for respawn sync
                        else if (ensis != null)
                            Bot.Combat.Attack(16);

                        else if (grigor != null)
                            Bot.Combat.Attack(17);
						Bot.Sleep(200);
						Bot.Wait.ForQuestComplete(Core.IsMember ? 10907 : 10906);
                        Bot.Wait.ForPickup(req.Name);
                    }
                    Core.CancelRegisteredQuests();
                    break;
                case "Owl's Dark Guise":
                case "Roosting Owl":
                case "Rufous Wisdom Staff":
                case "Stikini Witch Visage":
                case "Strikini Witch Morph":
					Core.FarmingLogger(req.Name, quant);
                    Core.EquipClass(ClassType.Solo);
                    while (!Bot.ShouldExit && !Core.CheckInventory(req.Name, quant))
                    {
                        if (Bot.Map.Name != "birgittamogloween")
                            Core.Join("birgittamogloween");

                        if (Bot.Player.Cell != "r6")
                            Core.Jump("r6", "Left");

                        ensis = Bot.Monsters.CurrentAvailableMonsters?.FirstOrDefault(m => m?.Alive == true && m.MapID == 16);
                        grigor = Bot.Monsters.CurrentAvailableMonsters?.FirstOrDefault(m => m?.Alive == true && m.MapID == 17);

                        if (ensis != null)
                            Bot.Combat.Attack(16);

                        else if (grigor != null)
                            Bot.Combat.Attack(17);

                        Bot.Sleep(200);
                    }
                    break;
                case "Poltergeistcorn Snack":
                case "Poltergeistcorn Snacks":
                case "Sinema Snack":
                case "Sinema Snacks":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("birgittamogloween", "Nazarene Artist", req.Name, quant, req.Temp);
                    break;
                case "Tiny Pink Princess Cupcake":
                case "Tiny Pink Princess Cupcakes":
                    Core.EquipClass(ClassType.Farm);
                    Core.HuntMonster("birgittamogloween", "Vampiric Guest", req.Name, quant, req.Temp);
                    break;
                case "Upsizer":
                    Core.EquipClass(ClassType.Solo);
                    Core.HuntMonster("birgittamogloween", "Pumpkin Pixie", req.Name, quant, req.Temp);
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
        new Option<bool>("104360", "Spooky Sleepover PJs", "Mode: [select] only\nShould the bot buy \"Spooky Sleepover PJs\" ?", false),
        new Option<bool>("103697", "Grande Owl Witch", "Mode: [select] only\nShould the bot buy \"Grande Owl Witch\" ?", false),
        new Option<bool>("104365", "Cineghouls", "Mode: [select] only\nShould the bot buy \"Cineghouls\" ?", false),
        new Option<bool>("104367", "Poltergeistcorn Buckets", "Mode: [select] only\nShould the bot buy \"Poltergeistcorn Buckets\" ?", false),
        new Option<bool>("104369", "Double Sinema Popcorn", "Mode: [select] only\nShould the bot buy \"Double Sinema Popcorn\" ?", false),
        new Option<bool>("104379", "Pink Princess Cupcakes", "Mode: [select] only\nShould the bot buy \"Pink Princess Cupcakes\" ?", false),
        new Option<bool>("103713", "Owl's Earthly Guise", "Mode: [select] only\nShould the bot buy \"Owl's Earthly Guise\" ?", false),
        new Option<bool>("103714", "Owl's Mystic Guise", "Mode: [select] only\nShould the bot buy \"Owl's Mystic Guise\" ?", false),
        new Option<bool>("103698", "Stikini Witch Hat", "Mode: [select] only\nShould the bot buy \"Stikini Witch Hat\" ?", false),
        new Option<bool>("103699", "Stikini Witch Cap", "Mode: [select] only\nShould the bot buy \"Stikini Witch Cap\" ?", false),
        new Option<bool>("103704", "Strikini Familiar Morph", "Mode: [select] only\nShould the bot buy \"Strikini Familiar Morph\" ?", false),
        new Option<bool>("103705", "Strikini Familiar Visage", "Mode: [select] only\nShould the bot buy \"Strikini Familiar Visage\" ?", false),
        new Option<bool>("104361", "Gothic Guest Visage", "Mode: [select] only\nShould the bot buy \"Gothic Guest Visage\" ?", false),
        new Option<bool>("104363", "Gothic Guest Morph", "Mode: [select] only\nShould the bot buy \"Gothic Guest Morph\" ?", false),
        new Option<bool>("104366", "Poltergeistcorn Bucket", "Mode: [select] only\nShould the bot buy \"Poltergeistcorn Bucket\" ?", false),
        new Option<bool>("104378", "Pink Princess Cupcake", "Mode: [select] only\nShould the bot buy \"Pink Princess Cupcake\" ?", false),
        new Option<bool>("104368", "Sinema Popcorn", "Mode: [select] only\nShould the bot buy \"Sinema Popcorn\" ?", false),
    ];
}
