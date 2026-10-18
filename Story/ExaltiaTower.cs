/*
name: Exaltia Tower Story
description: This will finish the Exaltia Tower story.
tags: story, quest, exaltia, exaltia tower, timeinn, dot
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class ExaltiaTower
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
        if (Core.isCompletedBefore(8151))
            return;

        Story.PreLoad(this);

        Core.AddDrop(
            "Exalted Node",
            "Exalted Forgemetal",
            "Exalted Relic Piece",
            "Exalted Artillery Shard"
        );

        //Unlocking the Antechamber 8146
        if (!Story.QuestProgression(8146))
        {
            Core.EnsureAccept(8146);
            Core.HuntMonster("timeinn", "Ice Elemental", "Key Ice Essence", 3);
            Core.HuntMonster("timeinn", "Fire Elemental", "Key Fire Essence", 3);
            Core.EnsureComplete(8146);
        }

        //Ezrajal 8147
        Story.KillQuest(8147, "timeinn", "Ezrajal");

        //Unlocking the Reliquary 8148
        if (!Story.QuestProgression(8148))
        {
            Core.EnsureAccept(8148);
            Core.HuntMonster("timeinn", "Nature Elemental", "Key Nature Essence", 3);
            Core.HuntMonster("timeinn", "Wind Elemental", "Key Wind Essence", 3);
            Core.EnsureComplete(8148);
        }

        //The Warden 8149
        Story.KillQuest(8149, "timeinn", "The Warden");

        //Unlocking the Apex 8150
        if (!Story.QuestProgression(8150))
        {
            Core.EnsureAccept(8150);
            Core.HuntMonster("timeinn", "Energy Elemental", "Key Energy Essence", 3);
            Core.HuntMonster("timeinn", "Water Elemental", "Key Water Essence", 3);
            Core.EnsureComplete(8150);
        }

        //The Engineer 8151
        Story.KillQuest(8151, "timeinn", "The Engineer");
    }
}
