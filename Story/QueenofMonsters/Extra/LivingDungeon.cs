/*
name: Living Dungeon (Extra)
description: This will finish the Living Dungeon quest.
tags: story, quest, queen-of-monsters, living-dungeon, extra
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class LivingDungeon
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        LivingDungeonStory();

        Core.SetOptions(false);
    }

    public void LivingDungeonStory()
    {
        if (Core.isCompletedBefore(4384))
            return;

        Story.PreLoad(this);

        // Titan Hollow
        Story.ChainQuest(4348);

        // Roots of all Evil
        Story.KillQuest(4349, "livingdungeon", "Root of Evil");

        // Venus Hero Trap
        Story.KillQuest(4350, "livingdungeon", "Seed Spitter");

        // Bark is worse than its bite
        if (!Story.QuestProgression(4351))
        {
            Core.EnsureAccept(4351);
            Core.HuntMonster("livingdungeon", "Evil Plant Horror", "It's a CLUE!", 10);
            Core.EnsureComplete(4351);
        }

        // Knot what you expected
        Story.KillQuest(4352, "livingdungeon", "Weeping Widowmaker");

        // Cha Cha Cha Chia!
        Story.KillQuest(4353, "livingdungeon", "Chia Warrior");

        // Leaf me alone!
        if (!Story.QuestProgression(4354))
        {
            Core.EnsureAccept(4354);
            Core.HuntMonster("livingdungeon", "Titan Decay", "Titan Decay defeated", 5);
            Core.HuntMonster("livingdungeon", "Seed Spitter", "Seed Spitter de-seeded", 5);
            Core.HuntMonster("livingdungeon", "Evil Plant Horror", "Evil Plant Horror defeated", 5);
            Core.EnsureComplete(4354);
        }

        // Evil Faerie Ambush!
        Story.KillQuest(4355, "livingdungeon", "Evil Tree Faerie");

        // Check the Trunk
        Story.KillQuest(4356, "livingdungeon", "Vulchurion");

        // Committing Tree-son
        if (!Story.QuestProgression(4357))
        {
            Core.EnsureAccept(4357);
            Core.HuntMonster("livingdungeon", "Evil Plant Horror", "Magic Tree Bark", 10);
            Core.EnsureComplete(4357);
        }

        // Heartwood
        if (!Story.QuestProgression(4358))
        {
            Core.EnsureAccept(4358);
            Core.HuntMonster("livingdungeon", "Evil Plant Horror", "Titan's Heart", 1);
            Core.EnsureComplete(4358);
        }

        // Drayko BOSS FIGHT!
        Story.KillQuest(4359, "livingdungeon", "Drayko");

        // Foilaged again!
        if (!Story.QuestProgression(4360))
        {
            Core.EnsureAccept(4360);
            Core.HuntMonster("livingdungeon", "Evil Plant Horror", "Monsters defeated", 15);
            Core.EnsureComplete(4360);
        }

        // Mind Games
        if (!Story.QuestProgression(4361))
        {
            Core.EnsureAccept(4361);
            Core.HuntMonster("livingdungeon", "Evil Plant Horror", "Tree Titan Brain Chunk", 10);
            Core.EnsureComplete(4361);
        }

        // DRAGON vs TITAN
        Story.KillQuest(4362, "treetitanbattle", "Dakka the Dire Dragon");

        // Smells like trouble!
        Story.KillQuest(4363, "livingdungeon", "Lil' Poot");

        // EPIC DROP!
        Story.KillQuest(4364, "livingdungeon", "Epic Drop");

        Core.AddDrop(
            "Wooden Ring",
            "Salad!",
            "Weeping Widowmaker Bone",
            "Chia in a pot!",
            "Fairy Phone",
            "Vulchurion Quill",
            "Drayko's Medallion",
            "Giant Dakka Fang"
        );
        // ------------------------------------------
        // Drayko Battle!
        if (!Core.CheckInventory("Drayko's Medallion"))
        {
            Core.Logger("Drayko's Medallion not found, finding it for you");
            Core.EnsureAccept(4383);

            // Vulchurions
            if (!Core.CheckInventory("Vulchurion Quill"))
            {
                Core.Logger("Vulchurion Quill not found, finding it for you");
                Core.EnsureAccept(4382);

                // Evil Tree Faeries
                if (!Core.CheckInventory("Fairy Phone"))
                {
                    Core.Logger("Fairy Phone not found, finding it for you");
                    Core.EnsureAccept(4381);

                    // Chia Warriors
                    if (!Core.CheckInventory("Chia in a pot!"))
                    {
                        Core.Logger("Chia in a pot! not found, finding it for you");
                        Core.EnsureAccept(4380);

                        if (!Core.CheckInventory("Weeping Widowmaker Bone"))
                        {
                            Core.Logger("Weeping Widowmaker Bone not found, finding it for you");
                            Core.EnsureAccept(4379);

                            if (!Core.CheckInventory("Salad!"))
                            {
                                Core.Logger("Salad! not found, finding it for you");
                                Core.EnsureAccept(4378);

                                if (!Core.CheckInventory("Wooden Ring"))
                                {
                                    Core.Logger("Wooden Ring not found, finding it for you");
                                    Core.EnsureAccept(4377);
                                    Core.HuntMonster("livingdungeon", "Root of Evil", "Wooden Ring Piece", 5);
                                    Core.EnsureComplete(4377);
                                    Bot.Wait.ForPickup("Wooden Ring");
                                    Core.Sleep(1000);
                                }
                                Core.HuntMonster("livingdungeon", "Evil Plant Horror", "Evil Plant Horror Leaf", 6);
                                Core.EnsureComplete(4378);
                                Bot.Wait.ForPickup("Salad!");
                                Core.Sleep(1000);
                            }
                            Core.HuntMonster("livingdungeon", "Weeping Widowmaker", "Widowmaker deboned", 5);
                            Core.EnsureComplete(4379);
                            Bot.Wait.ForPickup("Weeping Widowmaker Bone");
                            Core.Sleep(1000);
                        }
                        Core.HuntMonster("livingdungeon", "Chia Warrior", "Chia Warrior defeated", 3);
                        Core.EnsureComplete(4380);
                        Bot.Wait.ForPickup("Chia in a pot!");
                        Core.Sleep(1000);
                    }
                    Core.HuntMonster("livingdungeon", "Evil Tree Faerie", "Fairy Purse", 5);
                    Core.EnsureComplete(4381);
                    Bot.Wait.ForPickup("Fairy Phone");
                    Core.Sleep(1000);
                }
                Core.HuntMonster("livingdungeon", "Vulchurion", "Vulchurion Feather", 3);
                Core.EnsureComplete(4382);
                Bot.Wait.ForPickup("Vulchurion Quill");
                Core.Sleep(1000);
            }
            Core.HuntMonster("livingdungeon", "Drayko", "Drayko Defeated... again");
            Core.EnsureComplete(4383);
            Bot.Wait.ForPickup("Drarko's Medalion");
            Core.Sleep(1000);
        }
        // DRAGON vs TITAN Rematch! - 4384
        if (!Story.QuestProgression(4384))
        {
            Core.Logger("Giant Dakka Fang not found, finding it for you");
            Core.EnsureAccept(4384);
            Core.HuntMonster("treetitanbattle", "Dakka the Dire Dragon", "Dakka Defeated... again");
            Core.EnsureComplete(4384);
            // Bot.Wait.ForPickup("Giant Dakka Fang");
        }
    }
}
