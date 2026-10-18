/*
name: Rangda Story
description: This will finish the Rangda storyline.
tags: rangda-story, rangda, seasonal
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class RangdaSeasonal
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
        if (!Core.isSeasonalMapActive("rangda"))
            return;
        if (Core.isCompletedBefore(7622))
            return;

        Story.PreLoad(this);

        // 7617 Gather Flowers
        if (!Story.QuestProgression(7617))
        {
            Core.EnsureAccept(7617);
            Core.HuntMonster("rangda", "Flowering Bush", "Red Rose");
            Core.HuntMonster("rangda", "Flowering Bush", "White Rose");
            Core.HuntMonster("rangda", "Flowering Bush", "Kanthil Flower");
            Core.HuntMonster("rangda", "Flowering Bush", "Telon Flower");
            Core.HuntMonster("rangda", "Flowering Bush", "Orchid");
            Core.HuntMonster("rangda", "Flowering Bush", "Jasmine");
            Core.HuntMonster("rangda", "Flowering Bush", "Kenanga Flower");
            Core.EnsureComplete(7617);
        }

        // 7618 Defeat the Minions
        Story.KillQuest(7618, "rangda", "Leyak");

        // 7619 Create a Barrier
        if (!Story.QuestProgression(7619))
        {
            Core.EnsureAccept(7619);
            Core.GetMapItem(7526, 4, "rangda");
            Core.HuntMonster("rangda", "Leyak", "Blood of the Leyak", 4);
            Core.EnsureComplete(7619);
        }

        // 7620 Purify the Tuyul
        Story.KillQuest(7620, "rangda", "Tuyul");

        // 7621 Give an Offering
        if (!Story.QuestProgression(7621))
        {
            Core.EnsureAccept(7621);
            Core.GetMapItem(7527, 1, "rangda");
            Core.HuntMonster("rangda", "Leyak", "Evil Spirits Slain", 10);
            Core.EnsureComplete(7621);
        }

        // 7622 Defeat Rangda
        Story.KillQuest(7622, "rangda", "Rangda");

        Core.Logger("Rangda Storyline Complete");
    }
}
