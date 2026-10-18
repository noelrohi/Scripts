/*
name: Dream Palace Story
description: This will finish the Dream Palace Story.
tags: story, quest, dream-palace
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class DreamPalace
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
        if (Core.isCompletedBefore(7874))
            return;

        Story.PreLoad(this);

        // Potent Ruby 7869
        if (!Story.QuestProgression(7869))
        {
            Core.EnsureAccept(7869);
            Core.HuntMonster("dreampalace", "Flaming Harpy", "Monster Slain", 6);
            Core.HuntMonster("dreampalace", "Golmoth", "Potent Ruby", 1);
            Core.EnsureComplete(7869);
        }

        // Mystic Sapphire 7870
        if (!Story.QuestProgression(7870))
        {
            Core.EnsureAccept(7870);
            Core.HuntMonster("dreampalace", "Zelkur", "Mystic Sapphire", 1);
            Core.HuntMonster("dreampalace", "Lotus Spider", "Monster Slain", 6);
            Core.EnsureComplete(7870);
        }

        // Living Emerald 7871
        if (!Story.QuestProgression(7871))
        {
            Core.EnsureAccept(7871);
            Core.HuntMonster("dreampalace", "Palace Hound", "Monster Slain", 6);
            Core.HuntMonster("dreampalace", "Gazeroth", "Living Emerald", 1);
            Core.EnsureComplete(7871);
        }

        // Ethereal Diamond 7872
        if (!Story.QuestProgression(7872))
        {
            Core.EnsureAccept(7872);
            Core.HuntMonster("dreampalace", "Zal", "Ethereal Diamond", 1);
            Core.HuntMonster("dreampalace", "Ethereal Harpy", "Monster Slain", 6);
            Core.EnsureComplete(7872);
        }

        // Open the Door 7873
        Story.MapItemQuest(7873, "dreampalace", 7944);

        // Zahad 7874
        if (!Story.QuestProgression(7874))
        {
            Core.EnsureAccept(7874);
            Core.HuntMonster("dreampalace", "Guardian Hound", "Monster Slain", 6);
            Core.HuntMonster("dreampalace", "Zahad", "Zahad's Golden Orb", 1);
            Core.EnsureComplete(7874);
        }
    }
}
