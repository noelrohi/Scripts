/*
name: Dread Forest Story
description: This will finish the Dread Forest Story.
tags: story, quest, dread-forest
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class DreadForest
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(8722))
            return;

        Story.PreLoad(this);

        // Empty Fealty 8711
        Story.KillQuest(8711, "dreadforest", "Treacherous Bandit");

        // Bitter Sweet-Talker 8712
        if (!Story.QuestProgression(8712))
        {
            Core.EnsureAccept(8712);
            Core.GetMapItem(10267, 1, "dreadforest");
            Core.HuntMonster("dreadforest", "Noble\u2019s Servant", "Stolen Amenities", 7);
            Core.EnsureComplete(8712);
        }

        // Fake It Till You Make It 8713
        Story.KillQuest(8713, "dreadforest", "Noble's Knight");

        // Decimation 8714
        Story.KillQuest(8714, "dreadforest", "Treacherous Bandit");

        // Warning Beacon 8715
        if (!Story.QuestProgression(8715))
        {
            Core.EnsureAccept(8715);
            Core.GetMapItem(10268, 1, "dreadforest");
            Core.HuntMonster("dreadforest", "Noble's Knight", "Knight's Emblem", 7);
            Core.EnsureComplete(8715);
        }

        // Strategy and Utility 8716
        Story.KillQuest(8716, "dreadforest", "Reignolds' Knight");

        // Runaway 8717
        Story.MapItemQuest(8717, "dreadforest", new[] { 10269, 10270, 10271 });

        // Make - Up Artist 8718
        Story.KillQuest(8718, "dreadforest", "Taxidermied Servant");

        // Piercing the Veil 8719
        if (!Story.QuestProgression(8719))
        {
            Core.EnsureAccept(8719);
            Core.HuntMonster("dreadforest", "Taxidermied Servant", "Servants Eliminated", 7);
            Core.HuntMonster("dreadforest", "Reignolds' Knight", "Knights Eliminated", 7);
            Core.EnsureComplete(8719);
        }

        // Any Means Necessary 8720
        Story.KillQuest(8720, "dreadforest", "Lord Reignolds");

        // Out of Sight, Out of Mind 8721
        Story.MapItemQuest(8721, "dreadforest", 10272);

        // Vulture Pickings 8722
        if (!Story.QuestProgression(8722))
        {
            Core.EnsureAccept(8722);
            Core.HuntMonster("dreadforest", "Reignolds' Knight", "Valuable Metals", 8);
            Core.HuntMonster("dreadforest", "Taxidermied Servant", "Gold Pouches", 8);
            Core.HuntMonster("dreadforest", "Lord Reignolds", "Reignolds' Brooch", 1);
            Core.EnsureComplete(8722);
        }
    }
}
