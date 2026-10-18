/*
name: Ravenscar Story
description: This will finish the Ravenscar Story quests.
tags: story, quest, legion, ravenscar, raven scar, raven, scar, dage
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class Ravenscar
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

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(2907))
            return;

        Story.PreLoad(this);

        Core.EquipClass(ClassType.Farm);

        // Hunt for the Amulet (2901)
        Story.MapItemQuest(2901, "ravenscar", 1767);

        // Slay the Necromancer's Soldiers (2902)
        if (!Story.QuestProgression(2902))
        {
            Core.EnsureAccept(2902);
            Core.HuntMonster("ravenscar", "Restless Undead", "Crumpled Book Page", 1);
            Core.HuntMonster("ravenscar", "Restless Undead", "Undead Slain", 6);
            Core.EnsureComplete(2902);
        }

        // The Book of Portals (2903)
        Story.MapItemQuest(2903, "ravenscar", 1768);

        // Battle for a Lost Love (2904)
        Story.KillQuest(2904, "ravenscar", "Restless Undead");

        // Into the Blight (2905)
        Story.KillQuest(2905, "ravenscar", "Undead Soldier");

        // Gateway Guardian (2906)
        Story.KillQuest(2906, "ravenscar", "Shadowman");

        // Blightbringer's Doom (2907)
        Story.MapItemQuest(2907, "ravenscar", 1769);
    }
}
