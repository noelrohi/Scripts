/*
name: WatchTower Story
description: This will finish the WatchTower Story.
tags: story, quest, watch-tower
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class WatchTower
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
        // if (Core.isCompletedBefore(2610))
        //     return;

        Story.PreLoad(this);

        //Chaos Investigation 2602
        if (!Story.QuestProgression(2602))
        {
            Core.EnsureAccept(2602);
            Core.GetMapItem(1605, 6, "watchtower");
            Core.HuntMonster("watchtower", "Chaorrupted Wolf", "Chaos Wolf Slain", 3);
            Core.HuntMonster("watchtower", "Chaotic Gorillaphant", "Gorillaphant Slain", 3);
            Core.EnsureComplete(2602);
        }

        //Hunt for Answers 2603
        if (!Story.QuestProgression(2603))
        {
            Core.EnsureAccept(2603);
            Core.HuntMonster("watchtower", "Chaos Spider", "Vermin Cleared", 5);
            Core.HuntMonster("watchtower", "StorageBox", "Tattered Page 1", 1);
            Core.EnsureComplete(2603);
        }

        //Decryption Hunt 2604
        if (!Story.QuestProgression(2604))
        {
            Core.EnsureAccept(2604);
            Core.GetMapItem(1606, 1, "watchtower");
            Core.HuntMonster("watchtower", "Chaorrupted Knight", "Intruder Removed", 6);
            Core.EnsureComplete(2604);
        }

        //Prisoner Breakout! 2605
        Story.KillQuest(2605, "watchtower", "Chaorrupted Prisoner");

        //Hidden Secrets 2606
        if (!Story.QuestProgression(2606))
        {
            Core.EnsureAccept(2606);
            Core.GetMapItems(new[] { (1607, 1), (1608, 1) }, "watchtower");
            Core.HuntMonster("watchtower", "Chaos Gorillaphant", "Secret Guardian Defeated", 1);
            Core.EnsureComplete(2606);
        }

        //Mage Mystery 2607
        if (!Story.QuestProgression(2607))
        {
            Core.EnsureAccept(2607);
            Core.GetMapItem(1609, 4, "watchtower");
            Core.HuntMonster("watchtower", "Chaos Sp-eye", "Sp-Eye Blinded", 13);
            Core.EnsureComplete(2607);
        }

        //Take Back Remains 2608
        Story.KillQuest(2608, "watchtower", "Chaorrupted Knight");

        //Activate Teleport Rune 2609
        if (!Story.QuestProgression(2609))
        {
            Core.EnsureAccept(2609);
            Core.GetMapItem(1610, 1, "watchtower");
            Core.HuntMonster("watchtower", "Chaorrupted Good Soldier", "Chaorrupted Blood", 1);
            Core.EnsureComplete(2609);
        }

        //Chaos Knight Attacks 2610
        Story.KillQuest(2610, "watchtower", "Chaos Knight");
    }
}
