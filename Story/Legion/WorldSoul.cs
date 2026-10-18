/*
name: World Soul
description: This will finish the World Soul quest.
tags: story, quest, legion, world-soul
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class WorldSoul
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

        WorldSoulQuests();

        Core.SetOptions(false);
    }

    public void WorldSoulQuests()
    {
        if (Core.isCompletedBefore(6245))
            return;

        Story.PreLoad(this);

        if (!Core.isCompletedBefore(6245))
        {
            Core.EnsureAccept(6238);
            Core.HuntMonster("worldsoul", "Dwakel Infiltrator", "Void Cortex");
            Core.HuntMonster("worldsoul", "Dwakel Infiltrator", "Paradox Processor");
            Core.HuntMonster("worldsoul", "Dwakel Infiltrator", "Thermal Vent");
            Core.HuntMonster("worldsoul", "Dwakel Infiltrator", "Dwakel Defeated", 6);
            Core.EnsureComplete(6238);
        }
        Story.KillQuest(6239, "worldsoul", "Divine Water Elemental");

        Story.KillQuest(6240, "worldsoul", "Divine Fire Elemental");

        if (!Story.QuestProgression(6241))
        {
            Core.EnsureAccept(6241);
            Core.GetMapItem(5681, 3, "worldsoul");
            Core.HuntMonster("worldsoul", "Skeletal Squatter", "Iron Lightblade", 6);
            Core.EnsureComplete(6241);
        }

        Story.KillQuest(6242, "worldsoul", "Radioactive Hydra");

        if (!Story.QuestProgression(6243))
        {
            Core.EnsureAccept(6243);
            Core.GetMapItem(5680, 1, "worldsoul");
            Core.HuntMonster("worldsoul", "Legion Dreadmarch", "Legion Soldiers Defeated", 20);
            Core.EnsureComplete(6243);
        }

        if (!Story.QuestProgression(6244))
        {
            Core.EnsureAccept(6244);
            Core.GetMapItem(5682, 1, "worldsoul");
            Core.HuntMonster("worldsoul", "Legion Dreadmarch", "Legion Amulets", 3);
            Core.EnsureComplete(6244);
        }

        Story.KillQuest(6245, "worldsoul", "Core Guardian");
    }
}
