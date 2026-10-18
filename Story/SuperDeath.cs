/*
name: Super Death Story
description: This will finish the Super Death Story.
tags: story, quest, superdeath
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class SuperDeath
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
        if (Core.isCompletedBefore(8032))
            return;

        Story.PreLoad(this);

        //SMASH a Path 8015
        Story.KillQuest(8015, "SuperDeath", "Shadow Cave Yeti");

        //Cold Open the Portal 8016
        if (!Story.QuestProgression(8016))
        {
            Core.EnsureAccept(8016);
            Core.GetMapItem(8330, 1, "SuperDeath");
            Core.HuntMonster("superdeath", "Shadow Cave Yeti", "HeroSmash Essences", 4);
            Core.HuntMonster("superdeath", "Shadow Lava Crab", "Volcano Essences", 2);
            Core.HuntMonster("superdeath", "Shadow Collector", "Mall Essence", 1);
            Core.HuntMonster("superdeath", "Shadow Cave Yeti", "Ingiru Essence", 1);
            Core.EnsureComplete(8016);
        }

        //VolcaNO Thank You 8017
        Story.KillQuest(8017, "SuperDeath", "Igneous Lava Crab");

        //Return to Cinder 8018
        Story.KillQuest(8018, "SuperDeath", "Cinder Bender");

        //Hottica 8019
        Story.KillQuest(8019, "SuperDeath", "Hottica");

        //Liquidate Liberate 8020
        Story.KillQuest(8020, "SuperDeath", "Slime Collector");

        //Lord of the Mall 8021
        Story.KillQuest(8021, "SuperDeath", "Slime Lord");

        //Electina 8022
        Story.KillQuest(8022, "SuperDeath", "Electina");

        //Carpool Lane 8023
        Story.KillQuest(8023, "SuperDeath", "Rider");

        //The BlasterMaster 8024
        Story.KillQuest(8024, "SuperDeath", "Blaster Master");

        //General Smash 8025
        Story.KillQuest(8025, "SuperDeath", "General Smash");

        //Left In-giRuins 8026
        Story.KillQuest(8026, "SuperDeath", "Cave Yeti");

        //Rock Me Khali May-eus 8027
        Story.KillQuest(8027, "SuperDeath", "Khali May");

        //Charries 8028
        Story.KillQuest(8028, "SuperDeath", "Charries");

        //Shadow Realm Cleansing 8029
        Story.KillQuest(8029, "SuperDeath", "Shadow Cave Bandit");

        //Forging the Shadow Key 8030
        if (!Story.QuestProgression(8030))
        {
            Core.EnsureAccept(8030);
            Core.HuntMonster("superdeath", "Shadow Goo Pup", "Key Fragment #4", 1);
            Core.HuntMonster("superdeath", "Shadow Mutant", "Key Fragment #1", 1);
            Core.HuntMonster("superdeath", "Shadow Scorpion", "Key Fragment #2", 1);
            Core.HuntMonster("superdeath", "Shadow Cave Bandit", "Key Fragment #3", 1);
            Core.EnsureComplete(8030);
        }

        //Charge the Key 8031
        if (!Story.QuestProgression(8031))
        {
            Core.EnsureAccept(8031);
            Core.HuntMonster("superdeath", "Hottica", "Hottica Key Charge", 1);
            Core.HuntMonster("superdeath", "Electina", "Electina Key Charge", 1);
            Core.HuntMonster("superdeath", "General Smash", "General Smash Key Charge", 1);
            Core.HuntMonster("superdeath", "Charries", "Charries Key Charge", 1);
            Core.EnsureComplete(8031);
        }

        //Super Death 8032
        Story.KillQuest(8032, "SuperDeath", "Super Death");
    }
}
