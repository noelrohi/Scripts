/*
name: Aranx Quests
description: This script will do all quests of aranx in /darkdimension, /icedimension, /Ivoliss, /Pocketdimension and /sanddimension
tags: aranx, darkdimension, icedimension, ivoliss, pocketdimension, sanddimension
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class AranxQuests
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

        StoryLine();
        Core.SetOptions(false);
    }

    public void StoryLine()
    {
        if (Core.isCompletedBefore(6508))
            return;

        Story.PreLoad(this);
        //Find the Resonance 6496
        Story.KillQuest(6496, "pocketdimension", "Nothing");

        //Tune it up! 6497
        Story.MapItemQuest(6497, "pocketdimension", 5988, 3);

        //Explore the Ice Dimension 6498
        if (!Story.QuestProgression(6498))
        {
            Core.EnsureAccept(6498);
            Core.GetMapItem(5989, 1, "icedimension");
            Core.HuntMonster("icedimension", "Ice Spitter", "Enemies Defeated", 10);
            Core.EnsureComplete(6498);
        }

        //Open the Next Dimension 6499
        if (!Story.QuestProgression(6499))
        {
            Core.EnsureAccept(6499);
            Core.GetMapItem(5990, 5, "icedimension");
            Core.HuntMonster("icedimension", "Ice Elemental", "Frigid Resonance", 5);
            Core.EnsureComplete(6499);
        }

        //Explore the Sand Dimension 6500
        if (!Story.QuestProgression(6500))
        {
            Core.EnsureAccept(6500);
            Core.GetMapItem(5991, 1, "sanddimension");
            Core.HuntMonster("sanddimension", "Lotus Spider", "Enemies Defeated", 10);
            Core.EnsureComplete(6500);
        }

        //Open the 3rd Dimension 6501
        if (!Story.QuestProgression(6501))
        {
            Core.EnsureAccept(6501);
            Core.GetMapItem(5992, 5, "sanddimension");
            Core.HuntMonster("sanddimension", "Lotus Spider", "Arid Resonance", 5);
            Core.EnsureComplete(6501);
        }

        //Explore the Dark Dimension 6502
        if (!Story.QuestProgression(6502))
        {
            Core.EnsureAccept(6502);
            Core.GetMapItem(5993, 1, "darkdimension");
            Core.HuntMonster("darkdimension", "Void Phoenix", "Enemies Defeated", 10);
            Core.EnsureComplete(6502);
        }

        //Open the 4th Dimension 6503
        if (!Story.QuestProgression(6503))
        {
            Core.EnsureAccept(6503);
            Core.GetMapItem(5994, 5, "darkdimension");
            Core.HuntMonster("darkdimension", "Void Phoenix", "Dark Resonance", 5);
            Core.EnsureComplete(6503);
        }

        //Find Arthelyn 6504
        Story.MapItemQuest(6504, "ivoliss", 5995);

        //Get the Key 6505
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(6505, "ivoliss", "ivoliss");

        //Free Arthelyn 6506
        Story.MapItemQuest(6506, "ivoliss", 5996);
        Story.MapItemQuest(6506, "ivoliss", 5998);

        //Reach Arthelyn 6507
        Story.MapItemQuest(6507, "ivoliss", 5997);

        //Defeat Arthelyn 6508
        Story.KillQuest(6508, "ivoliss", "Arthelyn");
    }
}
