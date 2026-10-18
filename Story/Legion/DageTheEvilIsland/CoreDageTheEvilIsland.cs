/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class CoreDageTheEvilIsland
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

        Core.RunCore();

        Core.SetOptions(false);
    }

    public string[] EnvyItems =
    {
        "Envy Token V",
        "Envy Token IV",
        "Envy Token III",
        "Envy Token II",
        "Envy Token I",
    };

    public void CompleteDageTheEvilIslandStory()
    {
        if (Core.isCompletedBefore(5658))
        {
            Core.Logger("Saga Complete: Dage  the Evil Island");
            return;
        }
        DarkFortress();
        Seraph();
        LegionCrypt();
        EnvyMap();
        Laken();
    }

    public void DarkFortress()
    {
        if (Core.isCompletedBefore(4092))
        {
            Core.Logger("StoryLine Complete:   DarkFortress storyline");
            return;
        }
        Story.PreLoad(this);

        Core.AddDrop("Ultra Dark Mystery Stone Of Evil Animosity");

        //Test 1: Proving Ground 4084
        if (!Story.QuestProgression(4084))
        {
            Core.EnsureAccept(4084);
            Core.HuntMonster("darkfortress", "Dark Makai", "Dark Makai Slain", 13);
            Core.HuntMonster("darkfortress", "Dark Makai", "Cavern Dweller Slain", 13);
            Core.EnsureComplete(4084);
        }

        //Test 2: Nulgath's Disdain 4085
        if (!Story.QuestProgression(4085))
        {
            Core.EnsureAccept(4085);
            Core.HuntMonster("DarkFortress", "Ninja Spy", "Spy Slain", 6);
            Core.HuntMonster("DarkFortress", "Cloaked Fiend", "Elemental Slain", 6);
            Core.HuntMonster("DarkFortress", "Cloaked Fiend", "Cloaked Fiend Slain", 6);
            Core.EnsureComplete(4085);
        }

        //Test 3: Bonesaw Break 4086
        if (
            !Story.QuestProgression(4086)
            || !Core.CheckInventory("Ultra Dark Mystery Stone Of Evil Animosity")
        )
        {
            Core.AddDrop("Ultra Dark Mystery Stone Of Evil Animosity");
            Core.EnsureAccept(4086);
            Core.HuntMonster("DarkFortress", "Infernalfiend", "Ultra Dark Mystery Stone");
            Core.EnsureComplete(4086);
            Bot.Wait.ForPickup("Ultra Dark Mystery Stone Of Evil Animosity");
        }

        //Test 4: Dark Battleground 4088
        if (!Story.QuestProgression(4088))
        {
            Core.EnsureAccept(4088);
            Core.HuntMonster("darkfortress", "Cloaked Fiend", "Guard Slain", 13);
            Core.EnsureComplete(4088);
        }

        //Test 5: Shadows Within Shadows 4089
        Story.KillQuest(4089, "DarkFortress", "Dark Elemental");

        //Test 6: Runed Fiends 4090
        if (!Story.QuestProgression(4090))
        {
            Core.EnsureAccept(4090);
            Core.HuntMonster("darkfortress", "Dark Elemental", "Rune Scroll", 1);
            Core.HuntMonster("darkfortress", "Cloaked Fiend", "Fiend Scroll", 1);
            Core.EnsureComplete(4090);
        }

        //Surprise Test: Truth Revealed 4091
        Story.KillQuest(4091, "DarkFortress", "Wilhelm");

        //Final Exam: Your Own Worst Enemy 4092
        Story.KillQuest(4092, "DarkFortress", "Dage The Evil");
    }

    public void Seraph()
    {
        if (Bot.Quests.IsUnlocked(4186))
        {
            Core.Logger("StoryLine Complete:   Seraph storyline");
            return;
        }
        Story.PreLoad(this);

        //Explore the Order's Headquarters 4181
        Story.MapItemQuest(4181, "Seraph", 3280);

        //Good Secrets are Hard to Find 4182
        Story.KillQuest(4182, "Seraph", "Seraphic Recruit");

        //Whispers Rumors and Seraphic Scrolls 4183
        if (!Story.QuestProgression(4183))
        {
            Core.EnsureAccept(4183);
            Core.HuntMonster("seraph", "Legion Infiltrator", "Spy Rumors", 5);
            Core.HuntMonster("seraph", "Legion Infiltrator", "Seraphic Scrolls", 3);
            Core.HuntMonster("seraph", "Legion Infiltrator", "Undead Legion Sigil", 7);
            Core.EnsureComplete(4183);
        }
        //Me? A Spy? 4184
        Story.KillQuest(4184, "Seraph", "Legion Augur");

        //It's War! 4185
        if (!Story.QuestProgression(4185))
        {
            Core.EnsureAccept(4185);
            Core.KillMonster("Seraph", "r6", "Left", "Legion Infiltrator", "Legion Infiltrators Defeated", 10, log: false);
            Core.EnsureComplete(4185);
        }

        Core.Logger(
            "if the quest \"Finders Keepers\" [4186] is not Unlocked, the Bot will then do the previous Quest \"It's War!\" 10x"
        );

        while (!Bot.ShouldExit && !Bot.Quests.IsUnlocked(4186))
        {
            Core.EnsureAccept(4185);
            Core.KillMonster("Seraph", "r6", "Left", "Legion Infiltrator", "Legion Infiltrators Defeated", 10, log: false);
            Core.EnsureComplete(4185);
        }
    }

    public void LegionCrypt()
    {
        if (Core.isCompletedBefore(4195))
        {
            Core.Logger("StoryLine Complete:   LegionCrypt storyline");
            return;
        }
        Story.PreLoad(this);

        //Finders Keepers 4186
        Story.KillQuest(4186, "LegionCrypt", "Gravedigger");

        //Infiltrate the Infantry 4187
        Story.KillQuest(4187, "LegionCrypt", "Undead Infantry");

        //You’re Doomed 4188
        Story.KillQuest(4188, "LegionCrypt", "Legion Doomknight");

        //Eyes on the Prize 4189
        if (!Story.QuestProgression(4189))
        {
            Core.EnsureAccept(4189);
            Core.GetMapItem(3295, 5, "LegionCrypt");
            Core.HuntMonster("legioncrypt", "Gravedigger", "Gravedigger Gun", 5);
            Core.EnsureComplete(4189);
        }

        //Filler Quest 4190
        if (!Story.QuestProgression(4190))
        {
            Core.EnsureAccept(4190);
            Core.GetMapItem(3296, 5, "LegionCrypt");
            Core.HuntMonster("legioncrypt", "Legion DoomKnight", "Dead Doomknight", 6);
            Core.EnsureComplete(4190);
        }

        //Insider Information 4191
        Story.MapItemQuest(4191, "LegionCrypt", 3297);

        //Spiky Situation 4192
        if (!Story.QuestProgression(4192))
        {
            Core.EnsureAccept(4192);
            Core.HuntMonster("legioncrypt", "Gravedigger", "Gravedigger Knife", 6);
            Core.HuntMonster("legioncrypt", "Legion DoomKnight", "Legion Weapons", 7);
            Core.EnsureComplete(4192);
        }
        //Run it Back Again 4193
        Story.KillQuest(4193, "LegionCrypt", "Legion Doomknight");

        //Defeat Vincenzo! 4194
        Story.KillQuest(4194, "LegionCrypt", "Vincenzo");

        //Defeat Brutus! 4195
        Story.KillQuest(4195, "LegionCrypt", "Brutus");
    }

    public void EnvyMap()
    {
        if (Core.isCompletedBefore(4893))
        {
            Core.Logger("StoryLine Complete:   Envy storyline");
            return;
        }
        Story.PreLoad(this);

        Story.LegacyQuestManager(QuestLogic, Core.FromTo(4884, 4886));

        //Squee for Envy 4887
        if (!Story.QuestProgression(4887))
        {
            Core.EnsureAccept(4887);
            Core.HuntMonster("Envy", "Fawning Sycophant", "Clue");
            Core.HuntMonster("Envy", "Fawning Sycophant", "Secret");
            Core.HuntMonster("Envy", "Fawning Sycophant", "Gossip");
            Core.HuntMonster("Envy", "Fawning Sycophant", "Sincere but Unhelpful Praise");
            Core.EnsureComplete(4887);
        }

        Story.LegacyQuestManager(QuestLogic2, Core.FromTo(4888, 4889));

        void QuestLogic()
        {
            switch (Story.LegacyQuestID)
            {
                case 4884:
                    //An Enviable Army 4884 - Envy Token I
                    Core.HuntMonster("Envy", "Legion Defector", "Legion Defectors Beaten", 3);
                    Core.HuntMonster("Envy", "Fawning Sycophant", "Fawning Sycophants Beaten", 4);
                    break;

                case 4885:
                    //Where’s Darkon? 4885 - Envy Token II
                    Core.GetMapItem(4278, 4, "Envy");
                    break;

                case 4886:
                    //Her Biggest Fans 4886 - Envy Token III
                    Core.HuntMonster("Envy", "Fawning Sycophant", "Sycophant Hood");
                    Core.HuntMonster("Envy", "Fawning Sycophant", "Sycophant Tunic");
                    Core.HuntMonster("Envy", "Fawning Sycophant", "Sycophant Medallion");
                    Core.HuntMonster("Envy", "Fawning Sycophant", "Sycophant Boots");
                    break;
            }
        }
        void QuestLogic2()
        {
            switch (Story.LegacyQuestID)
            {
                case 4888:
                    //Higher Up The Totem Pole 4888 - Envy Token IV
                    Core.HuntMonster("Envy", "Disciple of Envy", "Disciple Helm");
                    Core.HuntMonster("Envy", "Disciple of Envy", "Disciple Chestplate");
                    Core.HuntMonster("Envy", "Disciple of Envy", "Disciple Leggings");
                    Core.HuntMonster("Envy", "Disciple of Envy", "Disciple Blade");
                    break;

                case 4889:
                    //Fight for Envy 4889 - Envy Token V
                    Core.HuntMonster("Envy", "Legion Spy", "Legion Spies Defeated", 8);
                    break;
            }
        }

        //Where’s That Employee Lounge Again? 4890
        if (!Story.QuestProgression(4890))
        {
            Core.EnsureAccept(4890);
            Core.HuntMonster("Envy", "Disciple of Envy", "Semi-Useful Information");
            Core.HuntMonster("Envy", "Disciple of Envy", "Cult Propaganda");
            Core.HuntMonster("Envy", "Disciple of Envy", "Another Clue");
            Core.HuntMonster("Envy", "Disciple of Envy", "Bad Puns");
            Core.EnsureComplete(4890);
        }

        //Group Effort 4891
        if (!Story.QuestProgression(4891))
        {
            Core.EnsureAccept(4891);
            Core.HuntMonster("envy", "Legion Defector", "Part 1", 1);
            Core.HuntMonster("envy", "Fawning Sycophant", "Part 2", 1);
            Core.HuntMonster("envy", "Disciple of Envy", "Part 3", 1);
            Core.EnsureComplete(4891);
        }

        //Defeat Envy 4892
        Story.KillQuest(4892, "Envy", "Envy");

        //The Queen of Envy 4893
        Story.KillQuest(4893, "Envy", "Queen of Envy");
    }

    public void Laken()
    {
        if (Core.isCompletedBefore(5658))
        {
            Core.Logger("StoryLine Complete:   Laken storyline");
            return;
        }
        Story.PreLoad(this);

        //Get Rid of the Guards 5648
        if (!Story.QuestProgression(5648))
        {
            Core.EnsureAccept(5648);
            Core.HuntMonster("laken", "Cyborg Dog", "Cyborg Dogs Slain", 4);
            Core.HuntMonster("laken", "Augmented Guard", "Augmented Guards Slain", 4);
            Core.EnsureComplete(5648);
        }

        //Get the Data 5649
        Story.KillQuest(5649, "Laken", "Mad Scientist");

        //Find the Door Code 5650
        Story.KillQuest(5650, "Laken", "Mad Scientist");

        //Find the Lab 5651
        Story.MapItemQuest(5651, "Laken", 5123);

        //Take out Dr. Eisenbacke 5652
        if (!Story.QuestProgression(5652))
        {
            Story.KillQuest(5652, "Laken", "Dr Eisenbacke");
            //Wait and Exit Cut Scene cell
            Bot.Wait.ForCellChange("Cut2");
            Core.JumpWait();
        }

        //Find Ada 5653
        if (!Story.QuestProgression(5653))
        {
            Core.EnsureAccept(5653);
            Core.GetMapItem(5124, 1, "Laken");
            Core.HuntMonster("laken", "Dustbunny", "Dustbunny Slain", 5);
            Core.EnsureComplete(5653);
        }

        //Keep Looking 5654
        if (!Story.QuestProgression(5654))
        {
            Core.EnsureAccept(5654);
            Core.GetMapItem(5125, 1, "Laken");
            Core.HuntMonster("laken", "Closet Moth", "Closet Moth Slain", 5);
            Core.EnsureComplete(5654);
        }

        //Gotcha! 5655
        if (!Story.QuestProgression(5655))
        {
            Story.MapItemQuest(5655, "Laken", 5126);
            //Wait and Exit Cut Scene cell
            Bot.Wait.ForCellChange("Cut3");
            Core.JumpWait();
        }

        //Combat Practice 5656
        if (!Story.QuestProgression(5656))
        {
            Core.EnsureAccept(5656);
            if (Bot.Player.Cell != "r10a")
                Core.Jump("r10a");
            Core.KillMonster("Laken", "r10a", "Left", "Ada", "Spar With Ada");
            Bot.Wait.ForQuestComplete(5656);
            // Core.EnsureComplete(5656);
            //Wait and Exit Cut Scene cell
            Core.JumpWait();
        }

        //Round 2 5657
        if (!Story.QuestProgression(5657))
        {
            Core.EnsureAccept(5657);
            if (Bot.Player.Cell != "r11")
                Core.Jump("r11");
            Core.KillMonster("Laken", "r11", "Left", "Ada", "Spar Again");
            Bot.Wait.ForQuestComplete(5657);
            //Wait and Exit Cut Scene cell
            Core.JumpWait();
            Core.EnsureComplete(5657);
        }

        //Third Time's A Charm 5658
        if (!Story.QuestProgression(5658))
        {
            Core.EnsureAccept(5658);
            if (Bot.Player.Cell != "r12")
                Core.Jump("r12");
            Core.KillMonster("Laken", "r12", "Left", "Ada", "Spar One More Time");
            Bot.Wait.ForQuestComplete(5658);
            //Wait and Exit Cut Scene cell
            Bot.Wait.ForCellChange("Cut4");
            Core.JumpWait();
        }
    }
}
