/*
name: Willow Creek Story
description: This will finish the Willow Creek Story.
tags: story, quest, willow-creek
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class WillowCreek
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
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
        Story.PreLoad(this);

        // Willow Creek 27
        if (!Story.QuestProgression(27))
        {
            Core.EnsureAccept(27);
            Core.HuntMonster("forest", "Zardman Grunt", "Hideous Zard Tail", 5);
            Core.HuntMonster("forest", "Boss Zardman", "Chieftain's Head", 1);
            Core.EnsureComplete(27);
        }

        // Can you do it? 59
        if (!Story.QuestProgression(59))
        {
            Core.EnsureAccept(59);
            Core.HuntMonster("willowcreek", "Mosquito", "Mosquito Wing", 5);
            Core.HuntMonster("willowcreek", "Frogdrake", "Frogdrake's Feet", 5);
            Core.EnsureComplete(59);
        }

        // Backyard Zards 22
        Story.KillQuest(22, "willowcreek", "Frogzard");

        // Attic Spiders 24
        Story.KillQuest(24, "willowcreek", "Spider");

        // Storage Speyeders 58
        Story.KillQuest(58, "willowcreek", "Speyeder");

        // Garden Snails 23
        Story.KillQuest(23, "willowcreek", "Snail");

        // Dwakel Info 1219
        Story.MapItemQuest(1219, "willowcreek", 14);

        // Found Clue in Farmer's House 1599
        Story.MapItemQuest(1599, "willowcreek", 15);

        // Tempestuous Times 26
        Story.MapItemQuest(26, "willowcreek", 16);

        // The Bone? 25
        Story.KillQuest(25, "willowcreek", "Werewolf");
    }
}
