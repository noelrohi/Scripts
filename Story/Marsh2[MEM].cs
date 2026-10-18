/*
name: Marsh 2 Story (Member)
description: This will finish the Marsh 2 Story.
tags: story, quest, marsh2, member
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class Marsh2
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
        if (!Core.IsMember)
        {
            Core.Logger("Marsh 2 Storyline Is Member Only. Skipping this Script");
            return;
        }

        if (Core.isCompletedBefore(1481))
            return;

        Story.PreLoad(this);

        //Clueless 1474
        Story.MapItemQuest(1474, "marsh2", 720, 8);

        //Filling In The Puzzle 1475
        Story.KillQuest(1475, "marsh2", "Undead Berserker");

        //The Eyes (and Teeth) Have It 1476
        if (!Story.QuestProgression(1476))
        {
            Core.EnsureAccept(1476);
            Core.HuntMonster("marsh2", "Ghoul", "Ghoul Tooth", 10);
            Core.HuntMonster("marsh2", "Marsh Lurker", "Marsh Lurker's Eye", 12);
            Core.EnsureComplete(1476);
        }

        //Halfway There 1477
        if (!Story.QuestProgression(1477))
        {
            Core.EnsureAccept(1477);
            Core.HuntMonster("marsh2", "Lich", "Mossy Bone", 15);
            Core.HuntMonster("marsh2", "Lesser Shadow Serpent", "Shadow Serpent Venom", 1);
            Core.EnsureComplete(1477);
        }

        //Charge It 1478
        if (!Story.QuestProgression(1478))
        {
            Core.EnsureAccept(1478);
            Core.HuntMonster("marsh2", "Thrax Ironhide", "Positive Charge", 1);
            Core.HuntMonster("marsh2", "Lesser Groglurk", "Negative Charge", 1);
            Core.EnsureComplete(1478);
        }

        //What Was That?! 1479
        Story.MapItemQuest(1479, "marsh2", 723);

        //The Mudluk Vault 1480
        Story.KillQuest(1480, "marsh2", "Marsh Vault Guardian");

        //Recovering Mudluk History 1481
        Story.MapItemQuest(1481, "marsh2", 721);
    }
}
