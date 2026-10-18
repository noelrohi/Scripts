/*
name: Tournament Story
description: This will finish the Tournament Story.
tags: story, quest, tournament
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class Tournament
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
        if (Core.isCompletedBefore(2717))
            return;

        Story.PreLoad(this);

        //Guard Swordhaven 2708
        if (!Story.QuestProgression(2708))
        {
            Core.EnsureAccept(2708);
            Core.GetMapItem(1682, 5, "tournament");
            Core.HuntMonster("tournament", "Suspicious Spy Bot ", "Bot Disabled", 5);
            Core.EnsureComplete(2708);
        }

        //Stalkers and Mercenaries 2709
        if (!Story.QuestProgression(2709))
        {
            Core.EnsureAccept(2709);
            Core.HuntMonster("tournament", "Mercenary", "Mercenary Slain", 4);
            Core.HuntMonster("tournament", "SandStalker", "Intruder Slain", 3);
            Core.EnsureComplete(2709);
        }

        //Brave the Bandits 2710
        Story.KillQuest(2710, "tournament", "Greenguard Bandit");

        //Find the Princesses! 2711
        if (!Story.QuestProgression(2711))
        {
            Core.EnsureAccept(2711);
            Core.GetMapItem(1683, 1, "tournament");
            Core.HuntMonster("tournament", "Princess Tara", "Tara is Safe", 1);
            Core.EnsureComplete(2711);
        }

        //Find Princess Brittany! 2712
        Story.MapItemQuest(2712, "tournament", 1684);

        //Battle Khai Kaldun 2713
        Story.KillQuest(2713, "tournament", "Khai Khaldun");

        //Battle Johann Wryce 2714
        Story.KillQuest(2714, "tournament", "Johann Wryce");

        //Battle the Knight of Thorns 2715
        Story.KillQuest(2715, "tournament", "Knight of Thorns");

        //Battle Roderick 2716
        Story.KillQuest(2716, "tournament", "Roderick");

        //Battle Lord Brentan 2717
        Story.KillQuest(2717, "tournament", "Lord Brentan");
    }
}
