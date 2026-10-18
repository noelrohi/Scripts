/*
name: Heart of the Sea Story
description: This will finish the Heart of the Sea Story Quest.
tags: story, quest, heart-of-the-sea, pirate
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class HeartOfTheSeaStory
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

        HeartOfTheSea();

        Core.SetOptions(false);
    }

    public void HeartOfTheSea()
    {
        if (!Core.isSeasonalMapActive("HeartOfTheSea"))
            return;
        if (Core.isCompletedBefore(6520))
            return;

        Story.PreLoad(this);

        // Puff Those Fish 6511
        Story.KillQuest(6511, "HeartOfTheSea", "Pufferfish");

        // Question the Merdraconians 6512
        Story.KillQuest(6512, "HeartOfTheSea", "Merdraconian");

        // Reveal the Ship 6513
        if (!Story.QuestProgression(6513))
        {
            Core.EnsureAccept(6513);
            Core.GetMapItem(5999, 5, "HeartOfTheSea");
            Core.HuntMonster("heartofthesea", "Jellyfish", "Jellyfish Stingers", 8);
            Core.EnsureComplete(6513);
        }

        // Find the Way In 6514
        Story.MapItemQuest(6514, "HeartOfTheSea", 6000);

        // Take Some Treasure 6515
        Story.MapItemQuest(6515, "HeartOfTheSea", 6001, 5);

        // Get Rid of the Grislytooth Pirates 6516
        Story.KillQuest(6516, "HeartOfTheSea", "Grislytooth Pirate");

        // Interrogate the Pirates 6517
        if (!Story.QuestProgression(6517))
        {
            Core.EnsureAccept(6517);
            Core.HuntMonster("heartofthesea", "First Mate Blackfang", "First Mate Interrogated", 1);
            Core.HuntMonster("heartofthesea", "Quartermaster Greenfin", "Quartermaster Interrogated", 1);
            Core.HuntMonster("heartofthesea", "Boatswain Rotbelly", "Boatswain Interrogated", 1);
            Core.EnsureComplete(6517);
        }

        // Find the Hold 6518
        if (!Story.QuestProgression(6518))
        {
            Core.EnsureAccept(6518);
            Core.GetMapItem(6002, 5, "HeartOfTheSea");
            Core.HuntMonster("heartofthesea", "Jellyfish", "BIG Stingers", 5);
            Core.EnsureComplete(6518);
        }

        // Get that Treasure!  6519
        Story.MapItemQuest(6519, "HeartOfTheSea", 6003);

        // Take out Scurvyfins McKrill 6520
        Story.KillQuest(6520, "HeartOfTheSea", "Scurvyfins McKrill");
    }
}
