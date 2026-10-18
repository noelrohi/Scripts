/*
name: Bright Crystal Story
description: This will complete the Bright Crystal story.
tags: story, quest, bright-crystal
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class BrightCrystalStory
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        CrystalBrightQuests();

        Core.SetOptions(false);
    }

    public void CrystalBrightQuests()
    {
        if (Core.isCompletedBefore(4967))
            return;

        Story.PreLoad(this);

        //Street Team 4953
        Story.MapItemQuest(4953, "dreamforest", 4326, 5);

        //De-Pesting 4954
        Core.EquipClass(ClassType.Farm);
        if (!Story.QuestProgression(4954))
        {
            Core.EnsureAccept(4954);
            Core.HuntMonster("dreamforest", "Crow", "Crows Shooed", 4);
            Core.HuntMonster("dreamforest", "Green Rat", "Rats Shooed", 4);
            Core.EnsureComplete(4954);
        }

        //Earning That Guest Pass 4955
        Story.MapItemQuest(4955, "dreamforest", 4327, 4);
        Story.MapItemQuest(4955, "dreamforest", 4328, 3);
        Story.MapItemQuest(4955, "dreamforest", 4329);

        //Low On Caffeine 4956
        Story.MapItemQuest(4956, "northpointe", 4330);

        //Feels like a Dream 4957
        Story.MapItemQuest(4957, "dreamforest", 4331, 5);

        //Give me a Ticket 4958
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(4958, "dreamforest", "Crow");

        //For an Elephant Ride? 4959
        Story.MapItemQuest(4959, "dreamforest", 4332);

        //Trainers 4960
        Story.MapItemQuest(4960, "dreamforest", 4333, 6);
        Story.MapItemQuest(4960, "dreamforest", 4334);

        //Tumblers 4961
        if (!Story.QuestProgression(4961))
        {
            Core.EnsureAccept(4961);
            Core.HuntMonster("dreamforest", "Acrobat", "Acrobat Defeated", 4);
            Core.HuntMonster("dreamforest", "Acrobat", "Midway Ticket", 1);
            Core.EnsureComplete(4961);
        }

        //Head to the Midway 4962
        if (!Story.QuestProgression(4962))
        {
            Core.EnsureAccept(4962);
            Core.HuntMonster("dreamforest", "Acrobat", "Acrobats Know Nothing", 3);
            Core.HuntMonster("dreamforest", "Fire Dancer", "Fire Dancers Seem Confused", 3);
            Core.HuntMonster("dreamforest", "Elephant Trainer", "Trainers Haven't Seen Him", 3);
            Core.EnsureComplete(4962);
        }

        //Balloons 4963
        if (!Story.QuestProgression(4963))
        {
            Core.EnsureAccept(4963);
            Core.HuntMonsterMapID("dreamforest", 19, "Prize Ticket", 10);
            Core.EnsureComplete(4963);
        }

        //Out of the Shadows 4964
        if (!Story.QuestProgression(4964))
        {
            Core.EnsureAccept(4964);
            Core.HuntMonster("dreamforest", "Living Shadow", "Shadows", 4);
            Core.HuntMonsterMapID("dreamforest", 9, "Darkness", 4);
            Core.EnsureComplete(4964);
        }

        //Follow the Footprints 4965
        Story.MapItemQuest(4965, "dreamforest", 4335, 10);

        //Help Miranda 4967
        if (!Story.QuestProgression(4967))
        {
            Core.EnsureAccept(4967);
            Core.HuntMonsterMapID("dreamforest", 23, "Geist Defeated");
            Core.EnsureComplete(4967);
        }
    }
}
