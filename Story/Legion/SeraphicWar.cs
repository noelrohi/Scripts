/*
name: Seraphic War
description: This will finish the Seraphic War quest.
tags: story, quest, legion, seraphic-war
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class SeraphicWar_Story
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

        SeraphicWar_Questline();

        Core.SetOptions(false);
    }

    public void SeraphicWar_Questline()
    {
        if (Core.isCompletedBefore(6245))
            return;

        Story.PreLoad(this);

        //Get the Parts
        if (!Story.QuestProgression(6238))
        {
            Core.EnsureAccept(6238);
            Core.HuntMonster("worldsoul", "Dwakel Infiltrator", "Dwakel Defeated", 6);
            Core.HuntMonster("worldsoul", "Dwakel Infiltrator", "Void Cortex", 1);
            Core.HuntMonster("worldsoul", "Dwakel Infiltrator", "Paradox Processor", 1);
            Core.HuntMonster("worldsoul", "Dwakel Infiltrator", "Thermal Vent", 1);
            Core.EnsureComplete(6238);
        }
        //Get the Water
        Story.KillQuest(6239, "worldsoul", "Divine Water Elemental");
        //Get the Fire
        Story.KillQuest(6240, "worldsoul", "Divine Fire Elemental");
        //Gather Armaments
        if (!Story.QuestProgression(6241))
        {
            Core.EnsureAccept(6241);
            Core.GetMapItem(5681, 3, "worldsoul");
            Core.HuntMonster("worldsoul", "Skeletal Squatter", "Iron Lightblade", 6);
            Core.EnsureComplete(6241);
        }
        //Plutonium For Power
        Story.KillQuest(6242, "worldsoul", "Radioactive Hydra");
        //Defeat the Undead
        if (!Story.QuestProgression(6243))
        {
            Core.EnsureAccept(6243);
            Core.GetMapItem(5680, 1, "worldsoul");
            Core.HuntMonster("worldsoul", "Legion Dreadmarch", "Legion Soldiers Defeated", 20);
            Core.EnsureComplete(6243);
        }
        //Remove the Ward
        if (!Story.QuestProgression(6244))
        {
            Core.EnsureAccept(6244);
            Core.HuntMonster("worldsoul", "Legion Dreadmarch", "Legion Amulets", 3);
            Story.MapItemQuest(6244, "worldsoul", 5682);
        }
        //Defeat the Guardian
        Story.KillQuest(6245, "worldsoul", "Core Guardian");
    }
}
