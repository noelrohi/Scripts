/*
name: DarkDungeon
description: This script finish all dark dungeon quests storyline in /darkdungeon
tags: dark, dungeon, story, line, mirror, drakath
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class DarkDungeon
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();
        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(3543))
            return;

        Story.PreLoad(this);
        //Quiz the Locals 3538
        Story.MapItemQuest(3538, "darkdungeon", new[] { 2671, 2672, 2673 });

        //Slay the Minions 3539
        Story.KillQuest(3539, "darkdungeon", "Dungeon Minion");

        //Free the Fallen 3540
        Story.KillQuest(3540, "darkdungeon", "Dungeon Paladin");

        //Shining a Light 3541
        if (!Story.QuestProgression(3541))
        {
            Core.EnsureAccept(3541);
            Core.GetMapItem(2674, 4, "darkdungeon");
            Core.HuntMonster("darkdungeon", "Shadow Imp", "Shadow Dispelled", 10);
            Core.EnsureComplete(3541);
        }

        //Break the Doors Down 3542
        Story.MapItemQuest(3542, "darkdungeon", 2679, 5);

        //The Best vs The Beast 3543
        Story.KillQuest(3543, "darkdungeon", "Cockatrice");
    }
}
