/*
name: Draco Con Story
description: This will finish the Draco Con story.
tags: story, quest, dracocon, draco, con
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class DracoCon
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
        if (Core.isCompletedBefore(5372))
            return;

        Story.PreLoad(this);

        Core.EquipClass(ClassType.Farm);

        // Zorbak's Legion: Chapter 1 (5357)
        if (!Story.QuestProgression(5357))
        {
            Core.EnsureAccept(5357);
            Core.HuntMonster("dracocon", "Zombie Tog", "Zombie Tog Slain", 7);
            Core.HuntMonster("dracocon", "Zombie Dravir", "Zombie Dravir Slain", 6);
            Core.EnsureComplete(5357);
        }

        // Zorbak's Legion: Eggstreme Plan (5358)
        Story.KillQuest(5358, "dracocon", "Zombie Dravir");

        // Zorbak's Legion: Drac Attack (5359)
        if (!Story.QuestProgression(5359))
        {
            Core.EnsureAccept(5359);
            Core.HuntMonster("dracocon", "Red Spirit", "Angry Spirit", 5);
            Core.HuntMonster("dracocon", "Angry Wisp", "Kill O' Whisp", 5);
            Core.EnsureComplete(5359);
        }

        // Zorbak's Legion: Undead vs Undead (5360)
        if (!Story.QuestProgression(5360))
        {
            Core.EnsureAccept(5360);
            Core.HuntMonster("dracocon", "Undead Soldier", "Undead Soldier Slain", 5);
            Core.HuntMonster("dracocon", "Zombie Warrior", "Zombie Warrior Slain", 5);
            Core.EnsureComplete(5360);
        }

        // Zorbak's Legion: Tiny Terrors (5361)
        if (!Story.QuestProgression(5361))
        {
            Core.EnsureAccept(5361);
            Core.GetMapItem(4723, 5, "dracocon");
            Core.HuntMonster("dracocon", "Villager", "Villager Convinced", 10);
            Core.EnsureComplete(5361);
        }

        // Eggcelent Hunt (5362)
        Story.MapItemQuest(5362, "dracocon", 4724, 13);

        // "Fresh" Dragon Chow (5363)
        Story.KillQuest(5363, "dracocon", "Zombie Warrior");

        // Bling, Baby, Bling (5364)
        if (!Story.QuestProgression(5364))
        {
            Core.EnsureAccept(5364);
            Core.GetMapItem(4725, 5, "dracocon");
            Core.HuntMonster("dracocon", "Battle Gem", "Battle Gems", 10);
            Core.EnsureComplete(5364);
        }

        // Class 101: Slaying for Dummies (5365)
        Story.KillQuest(5365, "dracocon", "Dragon Training Dummy");

        // Class 102: Breath of Fresh Fire (5366)
        Story.KillQuest(5366, "dracocon", "Angry Wisp");

        // Class 103: Tog Eat Tog World (5367)
        Story.KillQuest(5367, "dracocon", "Zombie Tog");

        // Final Exam Demonstration (5368)
        if (!Story.QuestProgression(5368))
        {
            Core.EquipClass(ClassType.Solo);
            Story.KillQuest(5368, "dracocon", "Strong Drag");
            Core.EquipClass(ClassType.Farm);
        }

        // Smack The Sheep! (5369)
        Story.MapItemQuest(5369, "dracocon", 4726);

        // Hear No Evil (5370)
        Story.MapItemQuest(5370, "dracocon", 4727, 4);

        // Safe and (No) Sound (5371)
        Story.MapItemQuest(5371, "dracocon", 4728, 6);

        // Through the Fire and Bones (5372)
        if (!Story.QuestProgression(5372))
        {
            Core.EquipClass(ClassType.Solo);
            if (!Story.QuestProgression(5372))
            {
                Core.EnsureAccept(5372);
                Core.HuntMonster("dracocon", "Drummer", "Drummer Defeated", 1);
                Core.HuntMonster("dracocon", "Guitarist", "Guitarist Defeated", 1);
                Core.HuntMonster("dracocon", "Keyboardist", "Keyboard Player Defeated", 1);
                Core.HuntMonster("dracocon", "Singer", "Singer Defeated", 1);
                Core.EnsureComplete(5372);
            }
            Core.EquipClass(ClassType.Farm);
        }
    }
}
