/*
name: Blazing Beach Story
description: This will finish the Blazing Beach Story Line.
tags: story, quest, blazing-beach, summer-break
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
using Skua.Core.Interfaces;

public class BlazingBeachStory
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

        StoryLine();

        Core.SetOptions(false);
    }

    public void StoryLine()
    {
        if (!Core.isSeasonalMapActive("blazingbeach"))
            return;

        if (Core.isCompletedBefore(8708))
            return;

        Story.PreLoad(this);

        // (Volca)No Trespassing
        Story.KillQuest(8702, "blazingbeach", "Burning Bombadier");

        // Piracy for Pyromancers
        if (!Story.QuestProgression(8703))
        {
            Core.EnsureAccept(8703);
            Core.HuntMonster("blazingbeach", "Magma Pirate", "Superheated Blade", 7);
            Core.HuntMonster("blazingbeach", "Scalding Shooter", "Burning Blunderbuss", 5);
            Core.HuntMonster("blazingbeach", "Magma Pirate", "Stolen Supplies", 10);
            Core.EnsureComplete(8703);
        }

        // Canned Heat
        if (!Story.QuestProgression(8704))
        {
            Core.EnsureAccept(8704);
            Core.GetMapItem(10252, 1, "blazingbeach");
            Core.HuntMonster("blazingbeach", "Burning Bombadier", "Packed Firebomb", 4);
            Core.EnsureComplete(8704);
        }

        // Dau Go
        if (!Story.QuestProgression(8705))
        {
            Core.EnsureAccept(8705);
            Core.HuntMonster("blazingbeach", "Dao Treeant", "Cavern Wood", 12, log: false);
            Core.KillMonster("blazingbeach", "r2", "Right", "Burning Bombadier", "Redistributed Loot", 12, log: false);
            Core.HuntMonster("burningbeach", "Water Goblin", "Goblin Canteen", 5, log: false);
            Core.EnsureComplete(8705);
        }
        // Bãi Cháy
        if (!Story.QuestProgression(8706))
        {
            Core.EnsureAccept(8706);
            Core.HuntMonster("burningbeach", "Lavazard", "Lizard Lava", 5, log: false);
            Core.HuntMonster("burningbeach", "Lava Guardian", "Mage Magma", 5, log: false);
            Core.HuntMonster("blazingbeach", "Ruby Golem", "Flame Ruby", 3, log: false);

            Core.EnsureComplete(8706);
        }

        // Sung Sot
        if (!Story.QuestProgression(8707))
        {
            Core.EnsureAccept(8707);
            Core.GetMapItem(10253, 1, "blazingbeach");
            Core.HuntMonster("burningbeach", "Maladrite", "Maladrite's Storm Core", 1);
            Core.HuntMonster("burningbeach", "Shark", "Shark Skin", 5);
            Core.EnsureComplete(8707);
        }

        // Ha Long
        Story.KillQuest(8708, "blazingbeach", "Magma Blazebeard");
    }
}
