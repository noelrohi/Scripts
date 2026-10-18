/*
name: Meateor Hunt Story
description: This will finish the Meateor Hunt storyline.
tags: meateor-hunt, seasonal, april-fools
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class MeateorHunt
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

        StoryLine();

        Core.SetOptions(false);
    }

    public void StoryLine()
    {
        if (!Core.isSeasonalMapActive("MeateorTown"))
            return;
        if (Core.isCompletedBefore(8628))
        {
            Core.Logger("StoryLine Complet: Meateor Hunt");
            return;
        }

        Story.PreLoad(this);

        //Defeat the Giant ChickenCow
        Story.KillQuest(8612, "MeateorTown", "Giant ChickenCow");

        //Chick Your Surroundings
        if (!Story.QuestProgression(8614))
        {
            Core.EnsureAccept(8614);
            Core.HuntMonster("EarthStorm", "Blue Chick", "Tiny Blue Freggment");
            Core.HuntMonster("Mythsong", "Pink Chick", "Tiny Pink Freggment");
            Core.HuntMonster("WaterStorm", "Purple Chick", "Tiny Purple Freggment");
            Core.HuntMonster("GreenguardWest", "Green Chick", "Tiny Green Freggment");
            Core.HuntMonster("DarkoviaGrave", "Rainbow Chick", "Tiny Rainbow Freggment");
            Core.EnsureComplete(8614);
        }

        //PainFowl Recollection
        Story.KillQuest(8615, "Battlefowl", "Chicken");

        //Empty Nest Syndrome
        if (!Story.QuestProgression(8616))
        {
            Core.EnsureAccept(8616);
            Core.HuntMonsterMapID("Uppercity", 8, "Nested Freggment", 8);
            Core.EnsureComplete(8616);
        }

        //Comet-ted to Memory
        if (!Story.QuestProgression(8617))
        {
            Core.EnsureAccept(8617);
            Core.HuntMonster("comet", "Vaderix", "Slimy Freggments", 5);
            Core.HuntMonster("comet", "Vaderix", "Even Slimier Freggment", 1);
            Core.EnsureComplete(8617);
        }

        //Right Under Your Nose
        Story.KillQuest(8618, "BattleFowl", "ChickenCow");

        //Don't Chicken Out!
        if (!Story.QuestProgression(8619))
        {
            Core.EquipClass(ClassType.Farm);
            Core.EnsureAccept(8619);
            Core.HuntMonster("BattleFowl", "Sabertooth Chicken", "Pointy Freggment", 8);
            Core.HuntMonster("MeateorTown", "Red Chicken", "Red Freggment", 10);
            Core.HuntMonster("DfLesson", "Chaotic Chicken", "Chaotic Freggment", 6);
            Core.EnsureComplete(8619);
        }
        //Ashville Hot
        if (!Story.QuestProgression(8620))
        {
            Core.EnsureAccept(8620);
            Core.HuntMonster("ashfallcamp", "Blackrawk", "Black Rocky Freggment", 1);
            Core.HuntMonster("ashfallcamp", "Smoldur", "Smoldering Freggment", 1);
            Core.HuntMonster("ashfallcamp", "Infernus", "Infernal Freggment", 1);
            Core.EnsureComplete(8620);
        }

        //Holy (Chicken)Cow!
        if (!Story.QuestProgression(8621))
        {
            Core.EnsureAccept(8621);
            Core.HuntMonster("crashruins", "Unlucky Explorer", "Ruined Freggment", 8);
            Core.HuntMonster("crashruins", "CluckMoo Idol", "Idolized Freggment", 1);
            Core.EnsureComplete(8621);
        }

        //Chilling Tales of Chicken Cows
        if (!Story.QuestProgression(8622))
        {
            Core.EnsureAccept(8622);
            Core.HuntMonster("northlands", "Ice Symbiote", "Frosty Freggment", 15);
            Core.HuntMonster("northlands", "Ice Symbiote", "25 Below Freggment", 1);
            Core.EnsureComplete(8622);
        }

        //Cowissa
        Bot.Quests.UpdateQuest(8000);
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(8623, "astravia", "The Moon");

        //Collection Dejection
        if (!Story.QuestProgression(8624))
        {
            Core.EnsureAccept(8624);
            Core.HuntMonster("future", "The Collector", "Collected Freggment", 10);
            Core.HuntMonster("future", "The Collector", "Private Collection Freggment", 3);
            Core.EnsureComplete(8624);
        }

        //Poached Eggs
        if (!Story.QuestProgression(8625))
        {
            Core.EnsureAccept(8625);
            Core.HuntMonster("byrodax", "Byro-Dax Monstrosity", "Poached Freggment", 1);
            Core.HuntMonster("byrodax", "Space Goop", "Goopy Freggment", 6);
            Core.HuntMonster("byrodax", "Mutated Critter", "Mutated Freggment", 12);
            Core.EnsureComplete(8625);
        }

        //Dumpster Diving
        if (!Story.QuestProgression(8626))
        {
            Core.EnsureAccept(8626);
            Core.HuntMonster("junkhoard", "Magpie Junk Heap", "Heaping Freggment", 1);
            Core.HuntMonster("junkhoard", "Junk Golem", "Trashy Freggment", 10);
            Core.HuntMonster("junkhoard", "Magpie", "Stolen Freggment", 8);
            Core.EnsureComplete(8626);
        }

        //A Dreadful Dinner
        Story.KillQuest(8627, "Dreadspace", "Troblor");

        //Succeed or Fry Trying
        if (!Story.QuestProgression(8628))
        {
            Core.EnsureAccept(8628);
            Core.HuntMonster("thirdspell", "Great Solar Elemental", "Fried Freggment", 3);
            Core.HuntMonster("thirdspell", "Sun Flare", "Boiled Freggment", 12);
            Core.HuntMonster("thirdspell", "Solar Incarnation ", "Sun-Cooked Freggment", 1);
            Core.EnsureComplete(8628);
        }
    }
}
