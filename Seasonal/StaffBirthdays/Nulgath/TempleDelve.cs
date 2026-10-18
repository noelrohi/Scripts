/*
name: Temple Delve
description: Completes the Temple Delve storyline.
tags: story, quest, temple, delve, siegefortress, nulgath, seasonal, staff, birthday
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
using Skua.Core.Interfaces;

public class TempleDelve
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static TempleSiege TS
    {
        get => _TS ??= new TempleSiege();
        set => _TS = value;
    }
    private static TempleSiege _TS;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();
        Core.SetOptions(false);
    }

    public void Storyline()
    {
        // Core.isCompletedBefore(9143) aways returns true...? evne if none of these are complete
        if (!Core.isSeasonalMapActive("templedelve"))
            return;

        TS.CompleteTempleSiege();

        Story.PreLoad(this);

        //Mindless Waste (9080)
        Story.KillQuest(9080, "templedelve", "Overdriven Paladin");

        //Stone Proxy (9081)
        if (!Story.QuestProgression(9081))
        {
            Core.EnsureAccept(9081);
            Core.GetMapItems(new[] { (11160, 2), (11161, 2) }, "templedelve");
            Core.HuntMonster("templedelve", "Doomed Troll", "Infested Troll Hide", 7);
            Core.EnsureComplete(9081);
        }

        //The Deepness of Shadows... (9082)
        if (!Story.QuestProgression(9082))
        {
            Core.EnsureAccept(9082);
            Core.GetMapItem(11162, 4, "templedelve");
            Core.HuntMonster("templedelve", "Doomed Beast", "Beasts Freed", 6);
            Core.EnsureComplete(9082);
        }

        //...Begets the Glare of the Light (9083)
        if (!Core.isCompletedBefore(9083))
        {
            Core.EnsureAccept(9083);
            Core.HuntMonster("templedelve", "Delirious Elemental", "Fractured Light Shard", 4);
            Core.HuntMonster("templedelve", "Infested Nation", "Nation Eliminated", 4);
            Story.MapItemQuest(9083, "templedelve", 11163);
        }

        //Death Over Defeat (9084)
        Story.KillQuest(9084, "templedelve", "Infested Nation");

        //For a Brighter Tomorrow (9085)
        Story.KillQuest(9085, "templedelve", "Delirious Elemental");

        //Minions Anonymous (9086)
        Story.MapItemQuest(9086, "templedelve", 11164, 3);
        Story.MapItemQuest(9086, "templedelve", 11165);

        //Prephosphorus (9087)
        if (!Story.QuestProgression(9087))
        {
            Core.EnsureAccept(9087);
            Core.GetMapItems(new[] { (11166, 1), (11167, 1) }, "templedelve");
            Core.HuntMonster("templedelve", "Delirious Elemental", "Elemental Vanquished", 6);
            Core.EnsureComplete(9087);
        }

        //Sunspotty (9088)
        if (!Story.QuestProgression(9088))
        {
            Core.EnsureAccept(9088);
            Core.HuntMonster("templedelve", "Delirious Elemental", "Frenzied Essence", 6);
            Core.HuntMonster("templedelve", "Infested Nation", "Nation Essence", 6);
            Core.EnsureComplete(9088);
        }

        //Feeding Time (9089)
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(9089, "templedelve", "Doomed Fiend");

        // Vile Vanguard 9134
        Story.KillQuest(9134, "siegefortress", "Legion Dreadmarch");

        // Fell Fealty 9135
        Story.KillQuest(9135, "siegefortress", "Legion Dread Knight");

        // Infestation Signs 9136
        Story.MapItemQuest(9136, "siegefortress", 11312, 4);
        Story.MapItemQuest(9136, "siegefortress", 11313);

        // Banal Barage 9137
        if (!Story.QuestProgression(9137))
        {
            Core.EnsureAccept(9137);
            Core.HuntMonster("siegefortress", "Legion Dread Knight", "Knight Demolished", 6);
            Core.HuntMonster("siegefortress", "Legion Dreadmarch", "Dreadmarch Demolished", 6);
            Core.EnsureComplete(9137);
        }

        // Bluebirds 9138
        Story.KillQuest(9138, "siegefortress", "Shadow Traitor");

        // Anatomical Example 9139
        if (!Story.QuestProgression(9139))
        {
            Core.EnsureAccept(9139);
            Core.GetMapItem(11314, 4, "siegefortress");
            Core.HuntMonster("siegefortress", "Shadow Traitor", "Traitor Subdued", 6);
            Core.EnsureComplete(9139);
        }

        // Bloodless Daisy 9140
        if (!Story.QuestProgression(9140))
        {
            Core.EnsureAccept(9140);
            Core.GetMapItem(11315, 1, "siegefortress");
            Core.HuntMonster("siegefortress", "Enslaved Elemental", "Light Rune", 8);
            Core.EnsureComplete(9140);
        }

        // Shadows of Espionage 9141
        if (!Story.QuestProgression(9141))
        {
            Core.EnsureAccept(9141);
            Core.GetMapItem(11316, 2, "siegefortress");
            Core.HuntMonster("siegefortress", "Shadow Traitor", "Traitor Interrogated", 5);
            Core.HuntMonster("siegefortress", "Enslaved Elemental", "Light Strangled", 5);
            Core.EnsureComplete(9141);
        }

        // Foul Duke of Light 9142
        Story.KillQuest(9142, "siegefortress", "Enslaved Astero");

        // Evil of Humanity 9143
        Story.KillQuest(9143, "siegefortress", "Dage the Evil");
    }
}
