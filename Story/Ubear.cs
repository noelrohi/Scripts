/*
name: Ubear Story
description: This will finish the Ubear Story.
tags: story, quest, ubear
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class Ubear
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
        if (Core.isCompletedBefore(7442))
            return;

        Story.PreLoad(this);

        //Ubear Token 7430
        Story.BuyQuest(7430, "ubear", 1851, "Ubear Token");

        //WHY ARE THERE BEES?! 7431
        Story.KillQuest(7431, "ubear", "Giant Bee");

        //That’s How We’ll Get Ants 7432
        if (!Story.QuestProgression(7432))
        {
            Core.EnsureAccept(7432);
            Core.GetMapItem(7199, 5, "ubear");
            Core.HuntMonster("ubear", "Honey Glob", "Honey Cleaned", 10);
            Core.EnsureComplete(7432);
        }

        //Door Jam 7433
        if (!Story.QuestProgression(7433))
        {
            Core.EnsureAccept(7433);
            Core.GetMapItem(7200, 1, "ubear");
            Core.HuntMonster("ubear", "Giant Bee", "Strong Stinger", 8);
            Core.EnsureComplete(7433);
        }

        //Oh... gross 7434
        Story.KillQuest(7434, "ubear", "Lil' Poot");

        //Blockage 7435
        Story.KillQuest(7435, "ubear", "Cornholio");

        //LIMft Token 7436
        Story.BuyQuest(7436, "limft", 1852, "LIMFT Token");

        //FIRE! 7437
        Story.KillQuest(7437, "limft", "Electrical Fire");

        //Smoke! 7438
        if (!Story.QuestProgression(7438))
        {
            Core.EnsureAccept(7438);
            Core.GetMapItem(7201, 6, "limft");
            Core.HuntMonster("limft", "Smoke Elemental", "Smoke Elemental Slain", 10);
            Core.EnsureComplete(7438);
        }

        //Scavengers 7439
        if (!Story.QuestProgression(7439))
        {
            Core.EnsureAccept(7439);
            Core.GetMapItem(7202, 5, "limft");
            Core.HuntMonster("limft", "Smoke Elemental", "Nuts", 5);
            Core.HuntMonster("limft", "Smoke Elemental", "Bolts", 5);
            Core.EnsureComplete(7439);
        }

        //Repair Time 7440
        Story.MapItemQuest(7440, "limft", 7203, 6);

        //Get Proof 7441
        Story.KillQuest(7441, "limft", "Sock Golem");

        //Stop the (U)bear! 7442
        Story.KillQuest(7442, "limft", "Ubear");
    }
}
