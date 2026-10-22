/*
name: Drakath Armor
description: Gets the Drakath Armor / Original Drakath Armor
tags: drakath, drakath armor, original drakath armor
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Good/BLoD/CoreBLOD.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Story/LordsofChaos/Core13LoC.cs
using Skua.Core.Interfaces;

public class DrakathArmorBot
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreDailies Daily
    {
        get => _Daily ??= new CoreDailies();
        set => _Daily = value;
    }
    private static CoreDailies _Daily;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;
    private static CoreBLOD BLOD
    {
        get => _BLOD ??= new CoreBLOD();
        set => _BLOD = value;
    }
    private static CoreBLOD _BLOD;
    private static Core13LoC LOC
    {
        get => _LOC ??= new Core13LoC();
        set => _LOC = value;
    }
    private static Core13LoC _LOC;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetBoth();

        Core.SetOptions(false);
    }

    public void GetBoth()
    {
        DrakathOriginalArmor();
        if (!Core.CheckInventory("Original Drakath Armor"))
            return;

        DrakathArmor();
    }

    public void DrakathArmor()
    {
        if (Core.CheckInventory("Drakath Armor"))
            return;

        LOC.Complete13LOC();
        DrakathArmorQuest();
        if (!Core.CheckInventory(60796))
            return;

        Core.Join("confrontation");
        if (Bot.ShouldExit)
            return;

        Bot.Shops.Load(994);
        if (!Bot.Shops.IsLoaded || Bot.Shops.ID != 994)
        {
            Core.Logger("Could not load Drakath Armor shop 994. The quest reward token is already owned.");
            return;
        }

        // The reward token confirms the turn-in even when the quest history is stale.
        Bot.Shops.BuyItem("Drakath Armor", 1);
        Bot.Wait.ForTrue(() => Bot.Inventory.Contains("Drakath Armor"), 20);
        if (!Core.CheckInventory("Drakath Armor"))
            Core.Logger("Drakath Armor purchase failed with reward token 60796 present. Check the shop unlock and inventory space.");
    }

    public void DrakathOriginalArmor()
    {
        if (Core.CheckInventory("Original Drakath Armor"))
            return;

        LOC.Complete13LOC();
        DrakathArmorQuest();
        if (!Core.CheckInventory(60796))
            return;

        Core.Join("confrontation");
        if (Bot.ShouldExit)
            return;

        Bot.Shops.Load(994);
        if (!Bot.Shops.IsLoaded || Bot.Shops.ID != 994)
        {
            Core.Logger("Could not load Drakath Armor shop 994. The quest reward token is already owned.");
            return;
        }

        // The reward token confirms the turn-in even when the quest history is stale.
        Bot.Shops.BuyItem("Original Drakath Armor", 1);
        Bot.Wait.ForTrue(() => Bot.Inventory.Contains("Original Drakath Armor"), 20);
        if (!Core.CheckInventory("Original Drakath Armor"))
            Core.Logger("Original Drakath Armor purchase failed with reward token 60796 present. Check the shop unlock and inventory space.");
    }

    public void DrakathArmorQuest()
    {
        if (
            Core.CheckInventory(
                60796 /* Get Your Original Drakath's Armor */
            )
        )
            return;

        Core.AddDrop(
            "Dage's Scroll Fragment",
            "Treasure Chest",
            "Face of Chaos",
            "Get Your Original Drakath's Armor"
        );
        Core.EnsureAccept(3882);
        Core.Logger("Getting Quest Accept Requirement: Blinding Light of Destiny");
        BLOD.BlindingLightOfDestiny(BLODMethod.Fewest_Hours);
        if (!Core.CheckInventory("Blinding Light of Destiny"))
        {
            Core.Logger(
                "You do not own \"Blinding Light of Destiny\". We'll Farm the materials, but you still need top run this again to get BLoD."
            );
        }

        // Get Accept Requirement: Blind Light of Destiny
        Farm.ChaosREP();

        Farm.BladeofAweREP(6, farmBoA: true);
        Nation.FarmUni13(3);
        Adv.BuyItem("hyperspace", 194, "Le Chocolat");
        Core.EquipClass(ClassType.Farm);
        Core.KillMonster("swordhavenundead", "Left", "Right", "*", "Treasure Chest", 100, false);
        Core.EquipClass(ClassType.Solo);
        Core.KillMonster("ultradrakath", "r1", "Left", "Champion of Chaos", "Face of Chaos", isTemp: false, publicRoom: true);

        if (!Core.CheckInventory("Dage's Scroll Fragment", 13))
        {
            Daily.DagesScrollFragment();
            if (!Core.CheckInventory("Dage's Scroll Fragment", 13))
            {
                Core.Logger("Build Drakath Armor needs 13 Dage scroll fragments before turn-in. The daily did not provide enough. Run again when more are available.");
                return;
            }
        }

        if (
            Core.CheckInventory("Dage's Scroll Fragment", 13)
            && Core.CheckInventory("Blinding Light of Destiny")
        )
        {
            Core.EnsureAccept(3882);
            if (!Bot.Quests.IsInProgress(3882))
            {
                Core.Logger("Build Drakath Armor quest 3882 could not be accepted. The armor purchase was skipped.");
                return;
            }

            Core.EnsureComplete(3882);
            Bot.Wait.ForPickup(60796);
            if (!Core.CheckInventory(60796))
                Core.Logger($"Build Drakath Armor turn-in did not yield reward token 60796. Current Dage scroll fragments: {Bot.Inventory.GetQuantity("Dage's Scroll Fragment")}. Check the quest turn-in and reward pickup before farming more fragments.");
        }
    }
}
