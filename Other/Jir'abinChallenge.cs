/*
name: Jirabin Challenge Quests
description: Does the Jirabin Challenge Quests
tags: jirabin challenge, runedwoods, dethertombs, xiex, voidbattle, jir'abin
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class JirabinChallenge
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public readonly string[] Drops =
    {
        "Blood of the Void Blade",
        "Blood of the Void Daggers",
        "Phantasmagoric Vengeance",
        "Purified Void Blade",
        "Purified Void Daggers",
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoAll();

        Core.SetOptions(false);
    }

    public void DoAll()
    {
        RunedWoods();
        DetherTombs();
        VoidBattle();
    }

    public void RunedWoods()
    {
        if (Core.isCompletedBefore(3987))
            return;

        Story.PreLoad(this);

        //Void Dragon Invasion
        Story.ChainQuest(3976);

        //Meet the Za'narians 3977
        Story.MapItemQuest(3977, "runedwoods", new[] { 3104, 3105, 3106, 3107, 3108, 3109 });

        //Ruining the Dragonwood 3978
        if (!Story.QuestProgression(3978))
        {
            Core.EnsureAccept(3978);
            Core.HuntMonster("runedwoods", "Void Warrior", "Slay Void Warriors", 3);
            Core.HuntMonster("runedwoods", "Frask", "Slay Frasks", 5);
            Core.HuntMonster("runedwoods", "Jies", "Slay Jies", 4);
            Core.EnsureComplete(3978);
        }

        //Catch the Ki'lks! 3979
        if (!Story.QuestProgression(3979))
        {
            Core.EnsureAccept(3979);
            Core.HuntMonster("northlandlight", "Ice Ki'lk", "Ice Ki'lk Slain", 5);
            Core.HuntMonster("northlands", "Fire Ki'lk", "Fire Ki'lk Slain", 5);
            Core.HuntMonster("northlands", "Energy Ki'lk", "Energy Ki'lk Slain", 5);
            Core.EnsureComplete(3979);
        }

        //Discover the Darkness 3980
        if (!Story.QuestProgression(3980))
        {
            Core.EnsureAccept(3980);
            Core.HuntMonster("runedwoods", "Frask", "Frask Scale", 3);
            Core.HuntMonster("doomwood", "Doomwood Bonemuncher", "Bonemuncher Fang", 8);
            Core.HuntMonster("doomwood", "Doomwood Ectomancer", "Ectomancer Orb");
            Core.EnsureComplete(3980);
        }

        //Shoot for Victory 3981
        if (!Story.QuestProgression(3981))
        {
            Core.EnsureAccept(3981);
            Core.HuntMonster("uppercity", "Drow Assassin", "Assassin Bowstring", 6);
            Core.HuntMonster("runedwoods", "Frask", "Frask Legbone", 6);
            Core.EnsureComplete(3981);
        }

        //Find the Frasks! 3982
        if (!Story.QuestProgression(3982))
        {
            Core.EnsureAccept(3982);
            Core.HuntMonster("sandsea", "Sandsea Frask", "Sandsea Frask Cleared", 4);
            Core.HuntMonster("pines", "Dwarfhold Frask", "Dwarfhold Frask Cleared", 4);
            Core.HuntMonster("hachiko", "Yokai Frask ", "Yokai Frask Cleared", 4);
            Core.EnsureComplete(3982);
        }

        //Analogues for Rath 3983
        if (!Story.QuestProgression(3983))
        {
            Core.EnsureAccept(3983);
            Core.HuntMonster("runedwoods", "Jies", "Jiess Rubble", 5);
            Core.HuntMonster("cloister", "Karasu", "Karasu Wing", 4);
            Core.HuntMonster("cloister", "Wendigo", "Wendigo Fur");
            Core.EnsureComplete(3983);
        }

        //The Shades Grow Darker 3984
        if (!Story.QuestProgression(3984))
        {
            Core.EnsureAccept(3984);
            Core.HuntMonster("northlands", "Northlands Shade", "Northlands Shades Cleared", 3);
            Core.HuntMonster("mythsong", "Mythsong Shade", "Mythsong Shades Cleared", 3);
            Core.HuntMonster("pines", "Dwarfhold Shade", "Dwarfhold Shades Cleared", 3);
            Core.HuntMonster("darkoviagrave", "Darkovia Shade", "Darkovia Shades Cleared", 3);
            Core.EnsureComplete(3984);
        }

        //Discover the Void 3985
        if (!Story.QuestProgression(3985))
        {
            Core.EnsureAccept(3985);
            Core.HuntMonster("void", "Void Elemental", "Void Elemental Heart", 8);
            Core.HuntMonster("void", "Void Bear", "Void Bear Tusk", 2);
            Core.EnsureComplete(3985);
        }

        //Confront Jir'abin 3986
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(3986, "runedwoods", "Jir'abin");

        // The Void Expands 3987
        Story.ChainQuest(3987);
    }

    public void DetherTombs()
    {
        if (Core.isCompletedBefore(4005))
            return;

        RunedWoods();

        Story.PreLoad(this);

        // Za'nar Week 2 - 1 3995
        Story.ChainQuest(3995);

        //Enter the Tomb 3996
        Story.KillQuest(3996, "dethertombs", "Sand Frask");

        //The Relic of Light Tome 3997
        Story.KillQuest(3997, "dethertombs", "De'ther Shade");

        //Disarm the Traps 3998
        Story.MapItemQuest(3998, "dethertombs", 3132, 12);

        //Find the Missing Pages 3999
        if (!Story.QuestProgression(3999))
        {
            Core.EnsureAccept(3999);
            Core.HuntMonster("dethertombs", "De'ther Shade", "Tome Page", 10);
            Core.HuntMonster("dethertombs", "De'ther Shade", "Device Diagram", 1);
            Core.EnsureComplete(3999);
        }

        //Find the Resonator Components 4000
        if (!Story.QuestProgression(4000))
        {
            Core.EnsureAccept(4000);
            Core.HuntMonster("dethertombs", "Tomb Drifter", "Tomb Drifter Scale", 10);
            Core.HuntMonster("dethertombs", "Sand Frask", "Frask Feather", 12);
            Core.EnsureComplete(4000);
        }

        //More Components needed 4001
        if (!Story.QuestProgression(4001))
        {
            Core.EnsureAccept(4001);
            Core.HuntMonster("dethertombs", "Darkfang", "Darkfang Claw", 15);
            Core.HuntMonster("dethertombs", "Tomb Shark", "Tomb Shark Hide", 1);
            Core.EnsureComplete(4001);
        }

        //Final Components 4002
        if (!Story.QuestProgression(4002))
        {
            Core.EnsureAccept(4002);
            Core.HuntMonster("dethertombs", "Jies", "Diamond Tooth", 6);
            Core.HuntMonster("dethertombs", "De'ther Vase", "Bottle of Oil", 1);
            Core.EnsureComplete(4002);
        }
        // if (!Story.QuestProgression(4002))
        // {
        //     Core.EnsureAccept(4002);
        //     Core.HuntMonster("dethertombs", "De'ther Vase", "Bottle Of Oil");
        //     Core.HuntMonster("dethertombs", "Jies", "Diamond Tooth", 6);
        //     Core.EnsureComplete(4002);
        // } //Keep //-ed incase something is wrong here

        //Gain Entrance to the Vault 4003
        Story.MapItemQuest(4003, "dethertombs", 3134);

        //Defeat the Spirit 4004
        Story.KillQuest(4004, "dethertombs", "Spirit of De'ther");

        //Battle Jir'abin 4005
        Story.KillQuest(4005, "xiex", "Jir'abin");
    }

    public void VoidBattle()
    {
        if (Core.CheckInventory(Drops, toInv: false))
            return;
        DetherTombs();
        Core.AddDrop(Drops);

        Core.Logger($"Hunting Jir'abin Challenge for drop items.");
        while (!Bot.ShouldExit && !Core.CheckInventory(Drops, toInv: false))
        {
            Core.HuntMonster("voidbattle", "Jir'abin Challenge", log: false);
            Core.ToBank(Drops);
        }
        Core.Logger($"All Drops Acquired.");
    }
}
