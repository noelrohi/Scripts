/*
name: Castle of Glass Story
description: This will complete the Castle of Glass story.
tags: story, quest, castle-of-glass
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class CastleOfGlass
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
        if (Core.isCompletedBefore(5356))
            return;

        Story.PreLoad(this);

        //Where’s Tiffany? 5339
        Story.MapItemQuest(5339, "castleofglass", 4698);

        //Trapped! 5340
        if (!Story.QuestProgression(5340))
        {
            Core.EnsureAccept(5340);
            Core.GetMapItems(new[] { (4699, 1), (4710, 1) }, "castleofglass");
            Core.HuntMonster("castleofglass", "Glass Wyvern", "Glass Wyvern Slain", 3);
            Core.EnsureComplete(5340);
        }

        //If At First You Don’t Succeed... 5341
        if (!Story.QuestProgression(5341))
        {
            Core.EnsureAccept(5341);
            Core.GetMapItems(new[] { (4700, 1), (4711, 1) }, "castleofglass");
            Core.HuntMonster("castleofglass", "Glass Panther", "Glass Panther Slain", 2);
            Core.EnsureComplete(5341);
        }

        //Try, Try Again 5342
        if (!Story.QuestProgression(5342))
        {
            Core.EnsureAccept(5342);
            Core.GetMapItems(new[] { (4701, 1), (4712, 1) }, "castleofglass");
            Core.HuntMonster("castleofglass", "Mirror Knight", "Mirror Knight Slain", 1);
            Core.EnsureComplete(5342);
        }

        //Escape! 5343
        Story.MapItemQuest(5343, "castleofglass", 4702);

        //Smash It Up 5344
        Story.KillQuest(5344, "castleofglass", "Glass Golem");

        //Find the Pieces 5345
        Story.KillQuest(5345, "castleofglass", "Glass Wyvern");

        //Finish the First Mosaic 5346
        Story.MapItemQuest(5346, "castleofglass", 4703);

        //Find More Pieces 5347
        if (!Story.QuestProgression(5347))
        {
            Core.EnsureAccept(5347);
            Core.HuntMonster("castleofglass", "Glass Golem", "Green Mosaic Piece", 4);
            Core.HuntMonster("castleofglass", "Glass Panther", "Blue Mosaic Piece", 2);
            Core.EnsureComplete(5347);
        }

        //Finish the Second Mosaic 5348
        Story.MapItemQuest(5348, "castleofglass", 4704);

        //Even  More Pieces 5349
        if (!Story.QuestProgression(5349))
        {
            Core.EnsureAccept(5349);
            Core.HuntMonster("castleofglass", "Glass Panther", "Blue Mosaic Piece", 4);
            Core.HuntMonster("castleofglass", "Glass Golem", "Green Mosaic Piece", 2);
            Core.HuntMonster("castleofglass", "Glass Wyvern", "Red Mosaic Piece", 6);
            Core.EnsureComplete(5349);
        }

        //Finish the Third Mosaic 5350
        Story.MapItemQuest(5350, "castleofglass", 4705);

        //Cut and Run 5351
        Story.MapItemQuest(5351, "castleofglass", 4706);

        //The Shard Golem 5352
        Story.KillQuest(5352, "castleofglass", "Shard Golem");

        //The Shardy Boys 5353
        if (!Story.QuestProgression(5353))
        {
            Core.EnsureAccept(5353);
            Core.GetMapItem(4707, 5, "castleofglass");
            Core.HuntMonster("castleofglass", "Glass Golem ", "Golem Interrogated", 5);
            Core.EnsureComplete(5353);
        }

        //Cold Iron 5354
        Story.MapItemQuest(5354, "castleofglass", 4708);

        //Smash It Up Again 5355
        Story.MapItemQuest(5355, "castleofglass", 4709, 7);

        //What The Heck Is This Thing?! 5356
        Story.KillQuest(5356, "castleofglass", "Chihuly");
    }
}
