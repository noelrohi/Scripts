/*
name: Shadow Vault Story
description: This will finish the Shadow Vault Story.
tags: story, quest, shadow-vault
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class ShadowVault
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
        if (Core.isCompletedBefore(6794))
            return;

        Story.PreLoad(this);

        //Find the Ruins 6781
        Story.MapItemQuest(6781, "ShadowVault", 6310);

        //Calm the Shadows 6782
        Story.KillQuest(6782, "ShadowVault", "Fallen Adventurer");

        //A Clue! A Clue! 6783
        if (!Story.QuestProgression(6783))
        {
            Core.EnsureAccept(6783);
            Core.HuntMonster("shadowvault", "Shadowscythe Minion", "Aliens \"Talked\" With", 8);
            Core.HuntMonster("shadowvault", "Fallen Adventurer", "A Clue", 1);
            Core.EnsureComplete(6783);
        }

        //Un-Cloaky! 6784
        if (!Story.QuestProgression(6784))
        {
            Core.EnsureAccept(6784);
            Core.GetMapItem(6311, 1, "ShadowVault");
            Core.KillMonster("shadowvault", "r3a", "Left", "Shadowscythe Guard", "Shadowscythe Guard Slain");
            Story.KillQuest(6784, "ShadowVault", "Shadowscythe Guard");
            Core.EnsureComplete(6784);
        }

        //Bound Seams Hide a Key 6787
        Story.MapItemQuest(6787, "ShadowVault", 6312);

        //Hole in the Wall 6788
        if (!Story.QuestProgression(6788))
        {
            Core.EnsureAccept(6788);
            Core.GetMapItem(6313, 1, "ShadowVault");
            Core.HuntMonster("shadowvault", "Spiderscythe", "Spider Smashed", 5);
            Core.HuntMonster("shadowvault", "Shadowscythe Minion", "Minion Gored", 5);
            Core.EnsureComplete(6788);
        }

        //An Actual Skeleton... key? 6789
        if (!Story.QuestProgression(6789))
        {
            Core.EnsureAccept(6789);
            Core.HuntMonster("shadowvault", "Fallen Adventurer", "Random Bones", 7);
            Core.HuntMonster("shadowvault", "Spiderscythe", "Webbing", 5);
            Core.HuntMonster("shadowvault", "Shadowscythe Minion", "Super Hard Claw", 1);
            Core.EnsureComplete(6789);
        }

        //Open Sesame 6790
        if (!Story.QuestProgression(6790))
        {
            Core.EnsureAccept(6790);
            Core.GetMapItem(6314, 1, "ShadowVault");
            Core.HuntMonster("shadowvault", "Shadowstryke", "Stryke Squashed", 6);
            Core.EnsureComplete(6790);
        }

        //Magic the Door Open 6791
        if (!Story.QuestProgression(6791))
        {
            Core.EnsureAccept(6791);
            Core.GetMapItem(6315, 5, "ShadowVault");
            Core.HuntMonster("shadowvault", "Darkness", "Dark Magic", 8);
            Core.EnsureComplete(6791);
        }

        //Un-Stucky! 6792
        Story.MapItemQuest(6792, "ShadowVault", 6316);

        //BOSS FIGHT! 6793
        Story.KillQuest(6793, "ShadowVault", "Ancient Doomknight");

        //… Did we forget some guards? 6794
        if (!Story.QuestProgression(6794))
        {
            Core.EquipClass(ClassType.Farm);
            Core.EnsureAccept(6794);
            Core.HuntMonsterMapID("ShadowVault", 22, "Guard Slain", 100);
            Core.EnsureComplete(6794);
            Core.JumpWait();
        }
    }
}
