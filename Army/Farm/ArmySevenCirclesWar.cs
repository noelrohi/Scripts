/*
name: Army Seven Circles War
description: Levels an army in /sevencircleswar while farming War Medals and Mega War Medals.
tags: army, leveling, seven-circles, war-medals, mega-war-medals
*/

//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreEngine.cs
//cs_include Scripts/Ultrasv3/Entwined Eclipse/CoreUltra.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Army/CoreArmyLite.cs
//cs_include Scripts/Story/Legion/SevenCircles(War).cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class ArmySevenCirclesWar
{
    private CoreBots C => CoreBots.Instance;
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreEngine Core = new();
    public CoreUltra Ultra = new();
    public CoreFarms Farm = new();

    private static CoreArmyLite Army
    {
        get => _Army ??= new CoreArmyLite();
        set => _Army = value;
    }
    private static CoreArmyLite? _Army;

    private static SevenCircles SC
    {
        get => _SC ??= new SevenCircles();
        set => _SC = value;
    }
    private static SevenCircles? _SC;

    public string OptionsStorage = "ArmySevenCirclesWar";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>("Solo", "Solo Farm", "Use just 1 account"),
        new Option<bool>("Endless", "Endless Farm", "Ignore the level check and continue until manually stopped."),
        new Option<ClassType>("ClassType", "ClassType to use", "Class to equip while farming."),
        Army.player1,
        Army.player2,
        Army.player3,
        Army.player4,
        Army.player5,
        Army.player6,
        Army.player7,
        Army.packetDelay,
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        C.SetOptions(disableClassSwap: true);
        Bot.Lite.ReacceptQuest = true;

        try
        {
            SC.Circles();
            if (!Bot.ShouldExit)
                Leveling();
        }
        finally
        {
            Bot.Options.AggroMonsters = false;
            Bot.Lite.ReacceptQuest = false;
            C.CancelRegisteredQuests();
            C.SetOptions(false);
        }
    }

    /// <summary>
    /// Levels the configured army in Seven Circles War while farming War Medals and Mega War Medals.
    /// </summary>
    public void Leveling()
    {
        bool solo = Bot.Config?.Get<bool>("Solo") ?? false;
        bool endless = Bot.Config?.Get<bool>("Endless") ?? false;
        ClassType classType = Bot.Config?.Get<ClassType>("ClassType") ?? ClassType.Farm;
        string[] players = Army.Players();

        if (!solo && players.Length == 0)
        {
            C.Logger("Players empty. Add account names in the script options or enable Solo Farm.");
            return;
        }

        const string map = "sevencircleswar";
        const string cell = "Enter";
        const string pad = "Spawn";
        string syncPath = Ultra.ResolveSyncPath("ArmyBool.sync");
        Ultra.ClearSyncFile(syncPath);

        C.PrivateRooms = true;
        C.PrivateRoomNumber = Army.getRoomNr();
        Farm.ToggleBoost(BoostType.Experience);
        Farm.ToggleBoost(BoostType.Gold);
        C.EquipClass(classType);
        C.RegisterQuests(7980, 7981);

        Bot.Sleep(2500);
        C.Logger("Players in Current Army: " + (solo ? "Yourself" : players.Length.ToString()));

        Core.Join(map);
        C.Jump(cell, pad);

        if (!solo && players.Length > 1)
            Ultra.WaitForArmy(players.Length - 1, "ArmySevenCirclesWar.sync");

        Bot.Player.SetSpawnPoint();
        Bot.Sleep(1500);
        Bot.Options.AggroMonsters = true;

        try
        {
            while (!Bot.ShouldExit)
            {
                if (!endless && ((solo && Bot.Player.Level >= 100)
                    || Ultra.CheckArmyProgressBool(() => Bot.Player.Level >= 100, syncPath)))
                {
                    Bot.Options.AggroMonsters = false;
                    C.JumpWait();
                    C.Logger("All players finished farm.");
                    break;
                }

                if (!Bot.Player.Alive)
                {
                    Bot.Wait.ForTrue(() => Bot.Player.Alive, 20);
                    continue;
                }

                if (Bot.Player.Cell != cell)
                {
                    Bot.Map.Jump(cell, pad);
                    Bot.Wait.ForCellChange(cell);
                }

                Bot.Combat.Attack("*");
                Bot.Sleep(500);
            }
        }
        finally
        {
            Bot.Options.AggroMonsters = false;
            C.CancelRegisteredQuests();
            Farm.ToggleBoost(BoostType.Experience, false);
            Farm.ToggleBoost(BoostType.Gold, false);
            C.JumpWait();
            Army.WaitForParty("whitemap");
        }
    }
}