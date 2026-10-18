/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class CoreDoomwood
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
        Core.RunCore();
    }

    public void CompleteDoomwood()
    {
        DoomwoodPart1();
        DoomwoodPart2();
        DoomwoodPart3();
        AQWZombies();
    }

    public void DoomwoodPart1()
    {
        DoomwoodForest();
        ChoppingMaul();
        NecroTower();
        NecroU();
        TempleOfTheLight();
        MadScientistsLab();
    }

    public void DoomwoodPart2()
    {
        NecropolisDungeon();
        NecropolisCavern();
    }

    public void DoomwoodPart3()
    {
        Thornsgarde();
        Stonewood();
        TechDungeon();
        StonewoodForest();
        TechFortress();
    }

    #region Part 1

    public void DoomwoodForest()
    {
        if (Core.isCompletedBefore(1089))
            return;

        Story.PreLoad(this);

        //1080    Lightguard Keep Found
        Story.ChainQuest(1080);

        //1064    Bony Battalion
        if (!Story.QuestProgression(1064))
        {
            Core.EnsureAccept(1064);
            Core.HuntMonster("doomwood", "Doomwood Ectomancer", "Ball of Ectoplasm", 3);
            Core.HuntMonster("doomwood", "Doomwood Bonemuncher", "Bonespike Collar", 4);
            Core.KillMonster("doomwood", "r8", "Right", "Doomwood Soldier", "Distal Fingerbone", 4);
            Core.EnsureComplete(1064);
        }

        //1065    Warrior Rez-queue
        Story.MapItemQuest(1065, "doomwood", 423, 5);

        //1066    Bone-tired Backup
        if (!Story.QuestProgression(1066))
        {
            Core.EnsureAccept(1066);
            Core.KillMonster("doomwood", "r6", "Right", "*", "Warrior Reinforced");
            Core.EnsureComplete(1066);
        }

        //1067    Reconnaissance Route
        Story.MapItemQuest(1067, "doomwood", 422);

        //1068    Fight Against Shadowed Light
        Story.KillQuest(1068, "doomwood", "Undead Paladin");

        //1069    Camouflage: Skelly-Style
        if (!Story.QuestProgression(1069))
        {
            Core.EnsureAccept(1069);
            Core.HuntMonster("doomwood", "Doomwood Bonemuncher", "Muncher Mandibles", 3);
            Core.HuntMonster("doomwood", "Doomwood Bonemuncher", "Stained Skulls", 3);
            Core.HuntMonster("doomwood", "Doomwood Ectomancer", "Ecto-Coated Cloth Scraps", 3);
            Core.HuntMonster("doomwood", "Doomwood Soldier", "Fractured Tibia", 3);
            Core.EnsureComplete(1069);
        }

        //1070    De(ad)ception
        if (!Story.QuestProgression(1070))
        {
            Core.EnsureAccept(1070);
            Core.GetMapItem(427, map: "doomundead");
            Core.KillMonster("doomundead", "r3", "Right", "*", "Light Knight Lifeforce", 5);
            Core.EnsureComplete(1070);
        }

        //1089    Artix Stopped!
        Story.ChainQuest(1089);
    }

    public void ChoppingMaul()
    {
        if (Core.isCompletedBefore(1085))
            return;

        DoomwoodForest();

        Story.PreLoad(this);

        //1081    Zorbak's Hideout
        Story.MapItemQuest(1081, "maul", 435);

        //1082    Stink-tuary
        Story.MapItemQuest(1082, "maul", 434, 13);

        //1083    The Infected
        if (!Story.QuestProgression(1083))
        {
            Core.EnsureAccept(1083);
            Core.HuntMonster("maul", "Personal Chopper", "Chopper Quarantined", 2);
            Core.HuntMonster("maul", "Slimeskull", "Slimeskull Quarantined", 6);
            Core.EnsureComplete(1083);
        }

        //1084    Chopping Spree
        if (!Story.QuestProgression(1084))
        {
            Core.EnsureAccept(1084);
            Core.KillMonster("maul", "r2", "Left", "*", "Body Part Donation", 10);
            Core.GetMapItem(436, 2, "maul");
            Core.EnsureComplete(1084);
        }

        //1085    GraveStop the Creature
        Story.KillQuest(1085, "maul", "Creature Creation");
    }

    public void NecroTower()
    {
        if (Core.isCompletedBefore(1101))
            return;

        ChoppingMaul();

        Story.PreLoad(this);

        //1087    ID What You Did There
        if (!Story.QuestProgression(1087))
        {
            Core.EnsureAccept(1087);
            Core.KillMonster("necrotower", "r2", "Left", "Doomwood Treeant", "Pain-per", 5);
            Core.KillMonster("necrotower", "r2", "Left", "Slimeskull", "Toxic Goo", 5);
            Core.EnsureComplete(1087);
        }

        //1088    The Ego and the ID
        if (!Story.QuestProgression(1088))
        {
            Core.EnsureAccept(1088);
            Core.HuntMonster("doomwood", "Doomwood Soldier", "Tombstone-Tuff (tm) Case", 1);
            Core.HuntMonster("doomwood", "Doomwood Soldier", "Nekron CruelPix Camera", 1);
            Core.EnsureComplete(1088);
        }

        //1090    An IDeal Seal
        if (!Story.QuestProgression(1090))
        {
            Core.EnsureAccept(1090);
            Core.HuntMonsterMapID("necrotower", 4, "Seal Components");
            Core.EnsureComplete(1090);
        }

        //1091    Need for Speed (Reading)!
        Story.MapItemQuest(1091, "necrotower", 438);

        //Necro Tower Elevator Minigame
        for (int i = 1092; i <= 1101; i++)
            Story.ChainQuest(i);
    }

    public void NecroU()
    {
        if (Core.isCompletedBefore(1154))
            return;

        NecroTower();

        Story.PreLoad(this);

        //1112    Silence is Ghoulden
        Story.KillQuest(1112, "necroU", "Ghoul");

        //1113    Ghouls with Gall
        Story.MapItemQuest(1113, "necroU", 450, 5);

        //1114    Goals for Ghouls and Other Undead
        if (!Story.QuestProgression(1114))
        {
            Core.EnsureAccept(1114);
            Core.HuntMonster("necroU", "Ghoul", "Undead Wishes");
            Core.HuntMonster("necroU", "Doomwood Soldier", "Undead Dreams");
            Core.HuntMonster("necroU", "Doomwood Soldier", "Undead Ambitions");
            Core.EnsureComplete(1114);
        }

        //1115    Knee Bone's Connected to the Thigh Bone
        Story.MapItemQuest(1115, "necroU", 449, 6);

        //1116    Hip Bone's Connected to the Back Bone
        if (!Story.QuestProgression(1116))
        {
            Core.EnsureAccept(1116);
            Core.HuntMonster("necrou", "Doomwood Soldier", "Back Bone", 3);
            Core.HuntMonster("necrou", "Doomwood Soldier", "Hip Bone", 3);
            Core.EnsureComplete(1116);
        }

        //1117    Back Bone's Connected to the Neck Bone
        if (!Story.QuestProgression(1117))
        {
            Core.EnsureAccept(1117);
            Core.GetMapItem(451, 3, "necroU");
            Core.HuntMonster("necrou", "Doomwood Treeant", "Splint-wood", 7);
            Core.EnsureComplete(1117);
        }

        //1118   Slip 'n Slimes
        if (!Story.QuestProgression(1118))
        {
            Core.EnsureAccept(1118);
            Core.GetMapItem(452, 5, "necroU");
            Core.HuntMonster("necrou", "Slimeskull", "Sheet of Slime", 5);
            Core.EnsureComplete(1118);
        }

        //1119   Sl-eye-me
        Story.MapItemQuest(1119, "necroU", 453, 5);

        //1120   Bones Over Brawn
        Story.KillQuest(1120, "necroU", "Doomwood Soldier");

        //1121   Skullaton Shells
        if (!Story.QuestProgression(1121))
        {
            Core.EnsureAccept(1121);
            Core.GetMapItem(454, 3, "necroU");
            Core.GetMapItem(455, 3, "necroU");
            Core.HuntMonster("necrou", "Shelleton", "Shelleton Bale Culled", 3);
            Core.EnsureComplete(1121);
        }

        //1154   Noxious Noxus
        Story.ChainQuest(1154);

        //1170   Vordred Boss!
        Story.KillQuest(1170, "vordredboss", "Ultra Vordred");
    }

    public void TempleOfTheLight()
    {
        if (Core.isCompletedBefore(1148))
            return;

        Story.PreLoad(this);

        Core.EquipClass(ClassType.Farm);
        //1123    Level 1
        if (!Story.QuestProgression(1123))
        {
            Core.EnsureAccept(1123);
            Core.KillMonster("temple", "r2", "Up", "SlimeSkull", "Slimeskull Trophy", 3);
            Core.KillMonster("temple", "r2", "Up", "Doomwood Bonemuncher", "Munched Boneshard", 3);
            Core.KillMonster("temple", "r2", "Up", "Shelleton", "Shelleton Shrapnel", 3);
            Core.EnsureComplete(1123);
        }

        //1124    Level 2
        if (!Story.QuestProgression(1124))
        {
            Core.EnsureAccept(1124);
            Core.GetMapItem(456, 6, "temple");
            Core.KillMonster("temple", "r3", "Up", "Undead Mage", "Necrotic Rune", 3);
            Core.KillMonster("temple", "r3", "Up", "Doomwood Ectomancer", "Ecto-Covered Rune", 3);
            Core.EnsureComplete(1124);
        }

        //1125    Level 3
        if (!Story.QuestProgression(1125))
        {
            Core.EnsureAccept(1125);
            Core.KillMonster("temple", "r4", "Up", "Ghoul", "Ghoulish Gear");
            Core.KillMonster("temple", "r4", "Up", "Lich", "Haunted Habiliment");
            Core.EnsureComplete(1125);
        }

        //1126    Level 4
        if (!Story.QuestProgression(1126))
        {
            Core.EnsureAccept(1126);
            Core.GetMapItem(457, 8, "temple");
            Core.KillMonster("temple", "r5", "Up", "Skeletal Fire Mage", "Flame Extinguished", 6);
            Core.EnsureComplete(1126);
        }

        //1127    Level 5
        Story.KillQuest(1127, "temple", "Doomwood Ectomancer");

        //1128    Level 6
        if (!Story.QuestProgression(1128))
        {
            Core.EnsureAccept(1128);
            Core.KillMonster("temple", "r2", "Up", "Doomwood Bonemuncher", "Masticated Mandible", 2);
            Core.KillMonster("temple", "r7", "Up", "Sanguine Souleater", "Freed Soul", 4);
            Core.EnsureComplete(1128);
        }

        //1129    Level 7
        Story.MapItemQuest(1129, "temple", 458, 12);

        //1130    Level 8
        if (!Story.QuestProgression(1130))
        {
            Core.EnsureAccept(1130);
            Core.HuntMonster("temple", "Doomwood Ectomancer", "Ectoplasmic Orb", 1);
            Core.HuntMonster("temple", "Skeletal Fire Mage", "Conflagration Orb", 1);
            Core.EnsureComplete(1130);
        }

        //1131    Level 9
        if (!Story.QuestProgression(1131))
        {
            Core.EnsureAccept(1131);
            Core.GetMapItem(459, 8, "temple");
            Core.HuntMonster("temple", "Ghoul", "Ghouls Gone", 10);
            Core.EnsureComplete(1131);
        }

        //1132    Level 10
        Story.KillQuest(1132, "temple", "Doomwood Ectomancer");

        //1133    Level 11
        if (!Story.QuestProgression(1133))
        {
            Core.EnsureAccept(1133);
            Core.GetMapItem(460, 10, "temple");
            Core.HuntMonster("temple", "Sanguine Souleater", "Souls Soothed", 10);
            Core.EnsureComplete(1133);
        }

        //1134    Level 12
        if (!Story.QuestProgression(1134))
        {
            Core.EnsureAccept(1134);
            Core.HuntMonster("temple", "Slimeskull", "Slime-Soaked Skull", 1);
            Core.HuntMonster("temple", "Doomwood Ectomancer", "Ecto-Covered Core", 1);
            Core.EnsureComplete(1134);
        }

        //1135    Level 13
        Story.MapItemQuest(1135, "temple", 461, 12);

        //1137    Level 14
        Story.KillQuest(1137, "temple", "Doomwood Soldier");

        //1138    Level 15
        Story.KillQuest(1138, "temple", "Ghoul");

        //1139    Level 16
        Story.MapItemQuest(1139, "temple", 462, 10);

        //1140    Level 17
        if (!Story.QuestProgression(1140))
        {
            Core.EnsureAccept(1140);
            Core.HuntMonster("temple", "Doomwood Bonemuncher", "Spiky Collar", 1);
            Core.HuntMonster("temple", "Skeleton", "Vertebrae Trophy", 1);
            Core.EnsureComplete(1140);
        }

        //1141    Level 18
        if (!Story.QuestProgression(1141))
        {
            Core.EnsureAccept(1141);
            Core.HuntMonster("temple", "Undead Mage", "Tattered Necronomicon", 5);
            Core.HuntMonster("temple", "Skeletal Fire Mage", "Scorched Necronomicon", 5);
            Core.EnsureComplete(1141);
        }

        //1142    Level 19
        if (!Story.QuestProgression(1142))
        {
            Core.EnsureAccept(1142);
            Core.GetMapItem(463, 6, "temple");
            Core.HuntMonster("temple", "Shelleton", "Screaming Shell-skull", 10);
            Core.HuntMonster("temple", "Skeleton", "Grinning Jawbone", 3);
            Core.EnsureComplete(1142);
        }

        //1143    Level 20
        Story.KillQuest(1143, "temple", "Lich");

        //1144    Defy the Dracolich
        Story.KillQuest(1144, "temple", "Dracolich");

        //1145    Restore the Tome
        Story.MapItemQuest(1145, "temple", 464, 10);

        //1146    Recover the Pages
        Story.KillQuest(1146, "temple", "Doomwood Bonemuncher");

        //1147    Reconstruct the Codex
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(1147, "temple", "Cryptkeeper Lich");

        //1148 Galvanize the Guardian
        if (!Story.QuestProgression(1148))
        {
            Core.EnsureAccept(1148);
            //wiki says these 2 arent temps... but they are :hmst:
            Core.KillMonster("temple", "r2", "Up", "Shelleton", "Basilisk's Scale");
            Core.KillMonster("temple", "r2", "Up", "Shelleton", "Scroll of Magic Inversion");
            Core.JumpWait();
            Core.Jump("Enter", "Spawn");
            Core.BuyItem("temple", 287, "Scroll of Cure Petrification");
            Core.EnsureComplete(1148);
        }
    }

    public void MadScientistsLab()
    {
        if (Core.isCompletedBefore(1169))
            return;

        Story.PreLoad(this);

        //1155    CastleMania: Disharmony and Despair
        Story.MapItemQuest(1155, "lab", 488);

        //1156    Stringing Your Enemies Along
        Story.KillQuest(1156, "lab", "Ant Giant");

        //1157    Out of Tune(ing Knobs)
        if (!Story.QuestProgression(1157))
        {
            Core.EnsureAccept(1157);
            Core.GetMapItem(489, 3, "lab");
            Core.HuntMonster("lab", "Ant Giant", "Tuning Knob", 3);
            Core.EnsureComplete(1157);
        }

        //1158    Code of Conduct-or
        Story.KillQuest(1158, "lab", "Ant Giant");

        //1159    Sending Out an SOS to the World
        Story.KillQuest(1159, "lab", "Ant Giant");

        //1160    This is FINAL ZAP!
        Story.MapItemQuest(1160, "lab", 490, 6);

        //1161   You Doing This Again?
        if (!Story.QuestProgression(1161))
        {
            Core.EnsureAccept(1161);
            Core.GetMapItem(491, 7, "lab");
            Core.HuntMonster("lab", "Giant Scorpion", "Spool of Blue Wire", 10);
            Core.EnsureComplete(1161);
        }

        //1162    Die, All of You!
        Story.KillQuest(1162, "lab", "Ant Giant");

        //1163    Ample Amps Required
        Story.MapItemQuest(1163, "lab", 492, 10);

        //1164    Upward Over the Mountain
        Story.MapItemQuest(1164, "mountain", 493);

        //1169    Charging Up!
        Story.MapItemQuest(1169, "mountain", 494);
    }

    #endregion

    #region Part 2

    public void NecropolisDungeon()
    {
        if (Core.isCompletedBefore(2061))
            return;

        Story.PreLoad(this);

        //2044    Descent into Darkness
        if (!Story.QuestProgression(2044))
        {
            Core.EnsureAccept(2044);
            Core.JumpWait();
            Core.KillMonster("necrodungeon", "r6", "Down", "*", "1 Floor Descended", 10);
            Core.EnsureComplete(2044);
        }

        //2045    Retrieve the Past, Room 1
        if (!Story.QuestProgression(2045))
        {
            Core.EnsureAccept(2045);
            Core.GetMapItem(1001, 1, "necrodungeon");
            Core.HuntMonster("necrodungeon", "Necropolis Soldier", "Necropolis Soldier Slain", 7);
            Core.EnsureComplete(2045);
        }

        //2046    Retrieve the Past, Room 2
        Story.MapItemQuest(2046, "necrodungeon", 1015, 5);
        Story.MapItemQuest(2046, "necrodungeon", 1002);

        //2047    Retrieve the Past, Room 3
        if (!Story.QuestProgression(2047))
        {
            Core.EnsureAccept(2047);
            Core.GetMapItem(1003, 1, "necrodungeon");
            Core.HuntMonster("necrodungeon", "Slimeskull", "Slime Sentry Squished", 8);
            Core.EnsureComplete(2047);
        }

        //2048    Retrieve the Past, Room 4
        if (!Story.QuestProgression(2048))
        {
            Core.EnsureAccept(2048);
            Core.GetMapItem(1004, 1, "necrodungeon");
            Core.HuntMonster("necrodungeon", "Necropolis Soldier", "Slimed Elevator Key", 1);
            Core.HuntMonster("necrodungeon", "Ghoul", "Ghoul Guardian Slain", 4);
            Core.EnsureComplete(2048);
        }

        //2049    Deeper into Darkness
        if (!Story.QuestProgression(2049))
        {
            Core.EnsureAccept(2049);
            Core.KillMonster("necrodungeon", "r11", "Down", "*", "1 Floor Descended", 10);
            Core.JumpWait();
            Core.EnsureComplete(2049);
        }

        //2050    Retrieve the Past, Room 5
        if (!Story.QuestProgression(2050))
        {
            Core.EnsureAccept(2050);
            Core.GetMapItem(1005, 1, "necrodungeon");
            Core.GetMapItem(1016, 3, "necrodungeon");
            Core.HuntMonster("necrodungeon", "Slimeskull", "Suspicious Slime Bribed", 3);
            Core.HuntMonster("necrodungeon", "Necropolis Soldier", "Suspicious Soldier Bribed", 4);
            Core.EnsureComplete(2050);
        }

        //2051    Retrieve the Past, Room 6
        Story.MapItemQuest(2051, "necrodungeon", 1017, 5);
        Story.MapItemQuest(2051, "necrodungeon", 1006);

        //2052    Retrieve the Past, Room 7
        if (!Story.QuestProgression(2052))
        {
            Core.EnsureAccept(2052);
            Core.GetMapItem(1007, 1, "necrodungeon");
            Core.HuntMonster("necrodungeon", "Slimeskull", "Vial of Slime Scent", 9);
            Core.EnsureComplete(2052);
        }

        //2053    Monster Subway Ahead!
        if (!Story.QuestProgression(2053))
        {
            Core.EnsureAccept(2053);
            Core.GetMapItem(1008, 1, "necrodungeon");
            Core.HuntMonster("necrodungeon", "Doom Crawler", "Doomcrawler Destroyed", 1);
            Core.EnsureComplete(2053);
        }

        //2054    Underground RailRoad... to DOOM!
        if (!Story.QuestProgression(2054))
        {
            Core.EnsureAccept(2054);
            Core.GetMapItem(1009, 1, "necrodungeon");
            Core.GetMapItem(1018, 8, "necrodungeon");
            Core.HuntMonster("necrodungeon", "Ghoul", "Chunk of Crawler Food", 8);
            Core.EnsureComplete(2054);
        }

        //2055    Retrieve the Past, Room 10
        if (!Story.QuestProgression(2055))
        {
            Core.EnsureAccept(2055);
            Core.GetMapItem(1010, 1, "necrodungeon");
            Core.HuntMonster("necrodungeon", "Necropolis Soldier", "Bony Elevator Key", 1);
            Core.EnsureComplete(2055);
        }

        //2056    The Deepest Descent
        if (!Story.QuestProgression(2056))
        {
            Core.EnsureAccept(2056);
            Core.JumpWait();
            Core.KillMonster("necrodungeon", "r18", "Down", "*", "1 Floor Descended", 10);
            Core.EnsureComplete(2056);
        }

        //2057    Retrieve the Past, Room 11
        Story.MapItemQuest(2057, "necrodungeon", 1016, 5);
        Story.MapItemQuest(2057, "necrodungeon", 1011);

        //2058    Retrieve the Past, Room 12
        if (!Story.QuestProgression(2058))
        {
            Core.EnsureAccept(2058);
            Core.GetMapItem(1012, 1, "necrodungeon");
            Core.HuntMonster("necrodungeon", "Necropolis Soldier", "Soldier Swayed", 7);
            Core.EnsureComplete(2058);
        }

        //2059    Retrieve the Past, Room 13
        Story.MapItemQuest(2059, "necrodungeon", 1019, 5);
        Story.MapItemQuest(2059, "necrodungeon", 1013);

        //2060    Fives Times the Fury
        if (!Bot.Quests.IsUnlocked(2061))
        {
            Core.EnsureAccept(2060);
            Core.HuntMonsterMapID("necrodungeon", 49, "Dracolich Head 1 Defeated!");
            Core.HuntMonsterMapID("necrodungeon", 50, "Dracolich Head 2 Defeated!");
            Core.HuntMonsterMapID("necrodungeon", 46, "Dracolich Head 3 Defeated!");
            Core.HuntMonsterMapID("necrodungeon", 47, "Dracolich Head 4 Defeated!");
            Core.HuntMonsterMapID("necrodungeon", 48, "Dracolich Head 5 Defeated!");
            Core.EnsureComplete(2060);
        }

        //2061    The Past Will Haunt You
        Story.MapItemQuest(2061, "necrodungeon", 1020);
    }

    public void NecropolisCavern()
    {
        if (Core.isCompletedBefore(2077))
            return;

        NecropolisDungeon();

        Story.PreLoad(this);

        //2070    Thou Shalt Not Pass
        Story.KillQuest(2070, "necrocavern", "Shadowstone Elemental");

        //2071    Blinded by the Darkness
        if (!Story.QuestProgression(2071))
        {
            Core.EnsureAccept(2071);
            Core.GetMapItem(1042, 6, "necrocavern");
            Core.HuntMonster("necrocavern", "Shadow Imp", "Shadow Imp Slain", 6);
            Core.EnsureComplete(2071);
        }

        //2072    The Tale Never Dies
        if (!Story.QuestProgression(2072))
        {
            Core.EnsureAccept(2072);
            Core.GetMapItem(1044, 1, "necrocavern");
            Core.GetMapItem(1045, 3, "necrocavern");
            Core.HuntMonster("necrocavern", "Shadow Imp", "Shadow Ink Obtained", 5);
            Core.HuntMonster("necrocavern", "Shadowstone Elemental", "Inkstone Obtained", 1);
            Core.EnsureComplete(2072);
        }

        //2073    Doom Outside the Dome
        Story.KillQuest(2073, "necrocavern", "Shadowstone Elemental");

        //2074    Last Bastion of Light
        if (!Story.QuestProgression(2074))
        {
            Core.EnsureAccept(2074);
            Core.HuntMonster("necrocavern", "Shadowstone Elemental", "Rock Intruder Shattered", 3);
            Core.HuntMonster("necrocavern", "Shadow Imp", "Imp Invader Slain", 4);
            Core.EnsureComplete(2074);
        }

        //2075    Shadowy Corruption
        Story.MapItemQuest(2075, "necrocavern", 1043, 5);

        //2076    Strength of the Darkness
        Story.KillQuest(2076, "necrocavern", "Shadow Dragon");

        //2077    Bring Down the Necropolis
        Story.KillQuest(2077, "necrocavern", "Shadowstone Support");
    }

    #endregion

    #region Part 3

    public void Thornsgarde()
    {
        if (Core.isCompletedBefore(7653))
            return;

        Story.PreLoad(this);

        //7589    Bullies
        if (!Story.QuestProgression(7589))
        {
            Core.EnsureAccept(7589);
            Core.HuntMonster("thorngarde", "CryptHacker", "CryptHacker Slain", 4);
            Core.HuntMonster("thorngarde", "NecroDrone", "NecroDrone Slain", 1);
            Core.HuntMonster("thorngarde", "NecroMech", "NecroMech Slain", 1);
            Core.EnsureComplete(7589);
        }

        //7590    Free the People
        if (!Story.QuestProgression(7590))
        {
            Core.EnsureAccept(7590);
            Core.GetMapItem(7485, 5, "thorngarde");
            Core.HuntMonster("thorngarde", "NecroMech", "Deadtech Key", 5);
            Core.EnsureComplete(7590);
        }

        //7591    Sample It
        Story.KillQuest(7591, "thorngarde", "NecroMech");

        //7592    Swat the Flies
        Story.KillQuest(7592, "thorngarde", "CryptHacker");

        //7593    Reclaim the Supplies
        if (!Story.QuestProgression(7593))
        {
            Core.EnsureAccept(7593);
            Core.HuntMonster("thorngarde", "CryptHacker", "Cured Meat", 3);
            Core.HuntMonster("thorngarde", "NecroMech", "Bag of Grain", 3);
            Core.HuntMonster("thorngarde", "NecroDrone", "Jug of Water", 5);
            Core.EnsureComplete(7593);
        }

        //7594    Who's That?
        Story.MapItemQuest(7594, "thorngarde", 7486);

        //7595    Find my Stuff
        if (!Story.QuestProgression(7595))
        {
            Core.EnsureAccept(7595);
            Core.GetMapItems(new[] { (7487, 1), (7488, 1) }, "thorngarde");
            Core.HuntMonster("thorngarde", "NecroDrone", "Kyger's Bunny", 1);
            Core.EnsureComplete(7595);
        }

        //7596    Save my Friends
        if (!Story.QuestProgression(7596))
        {
            Core.EnsureAccept(7596);
            Core.GetMapItems(new[] { (7489, 1), (7490, 1), (7491, 1) }, "thorngarde");
            Core.HuntMonster("thorngarde", "NecroMech", "NecroMech Slain", 3);
            Core.EnsureComplete(7596);
        }

        //7597    Pull the Wires
        Story.KillQuest(7597, "thorngarde", "CryptHacker");

        //7598    Dress-up Time!
        if (!Story.QuestProgression(7598))
        {
            Core.EnsureAccept(7598);
            Core.HuntMonster("thorngarde", "CryptHacker", "CryptHacker Headgear", 1);
            Core.HuntMonster("thorngarde", "NecroDrone", "Deadtech Pieces", 5);
            Core.EnsureComplete(7598);
        }

        //7599    I Need a Distraction
        if (!Story.QuestProgression(7599))
        {
            Core.EnsureAccept(7599);
            Core.HuntMonster("thorngarde", "NecroMech", "NecroMech Slain", 5);
            Core.HuntMonster("thorngarde", "CryptHacker", "CryptHacker Slain", 5);
            Core.HuntMonster("thorngarde", "NecroDrone", "NecroDrone Slain", 5);
            Core.EnsureComplete(7599);
        }

        //7600    Zyrus is Lost
        Story.KillQuest(7600, "thorngarde", "Zyrus the BioKnight");

        //7601    Investigate the Tech
        if (!Story.QuestProgression(7601))
        {
            Core.EnsureAccept(7601);
            Core.HuntMonster("thorngarde", "NecroDrone", "Deadtech Power Core", 7);
            Core.HuntMonster("thorngarde", "NecroMech", "NecroMech Targeting Systems", 5);
            Core.HuntMonster("thorngarde", "CryptHacker", "CryptHacker Circuitry", 15);
            Core.HuntMonster("thorngarde", "Zyrus the BioKnight", "BioKnight Engine", 3);
            Core.EnsureComplete(7601);
        }
    }

    public void Stonewood()
    {
        if (Core.isCompletedBefore(7653))
            return;

        Thornsgarde();

        Story.PreLoad(this);

        //7603    Defeat the Deadtech
        Story.KillQuest(7603, "stonewood", "NecroDrone");

        //7604    Gather Spell Components
        if (!Story.QuestProgression(7604))
        {
            Core.EnsureAccept(7604);
            Core.GetMapItem(7510, 6, "stonewood");
            Core.HuntMonster("stonewood", "Doomwood Treeant", "Treeant Pollen", 8);
            Core.EnsureComplete(7604);
        }

        //7605    A Few Things More
        if (!Story.QuestProgression(7605))
        {
            Core.EnsureAccept(7605);
            Core.HuntMonster("stonewood", "NecroDrone", "Fresh Marrow", 8);
            Core.HuntMonster("stonewood", "NecroMech", "Pristine Wishbone", 1);
            Core.EnsureComplete(7605);
        }

        //7606    Search for the Axe
        Story.MapItemQuest(7606, "stonewood", 7511, 4);
        Story.MapItemQuest(7606, "stonewood", 7512);

        //7607    Defeat the BioKnight
        Story.KillQuest(7607, "stonewood", "BioKnight");

        //7608    Take the Axe!
        Story.MapItemQuest(7608, "stonewood", 7513);
    }

    public void TechDungeon()
    {
        if (Core.isCompletedBefore(7636))
            return;

        Stonewood();

        Story.PreLoad(this);

        //7609    Find the Keys
        Story.KillQuest(7609, "techdungeon", "NecroMech");

        //7610    Ew, Rats
        if (!Story.QuestProgression(7610))
        {
            Core.EnsureAccept(7610);
            Core.GetMapItem(7515, 6, "techdungeon");
            Core.HuntMonster("techdungeon", "Rotting Rat", "Rat Slain", 10);
            Core.EnsureComplete(7610);
        }

        //7611    Grab Their Gear
        if (!Story.QuestProgression(7611))
        {
            Core.EnsureAccept(7611);
            Core.KillMonster("techdungeon", "r4", "Left", "DoomBorg Guard", "Guard Armor Piece", 5);
            Core.KillMonster("techdungeon", "r4", "Left", "DoomBorg Guard", "Guard Helm");
            Core.KillMonster("techdungeon", "r4", "Left", "DoomBorg Guard", "Deadtech Polearm");
            Core.EnsureComplete(7611);
        }

        //7612    Talk to the Guard
        Story.MapItemQuest(7612, "techdungeon", 7514);

        //7613    Let Them Out!
        Story.MapItemQuest(7613, "techdungeon", 7516);

        //7614    Head on Out
        if (!Story.QuestProgression(7614))
        {
            Core.EnsureAccept(7614);
            Core.GetMapItem(7517, 1, "techdungeon");
            Core.HuntMonster("techdungeon", "DoomBorg Guard", "Deadtech Guard Slain", 12);
            Core.EnsureComplete(7614);
        }

        //7615    Take Him Down
        Story.KillQuest(7615, "techdungeon", "Kalron the Cryptborg");
    }

    public void StonewoodForest()
    {
        if (Core.isCompletedBefore(7636))
            return;

        TechDungeon();

        Story.PreLoad(this);

        //7623    Cat Ears?
        if (!Story.QuestProgression(7623))
        {
            Core.EnsureAccept(7623);
            Core.GetMapItem(7528, 1, "stonewooddeep");
            Core.EnsureComplete(7623);
            Core.Logger("Cutscene > Wait > Jump > Continue");
            Bot.Wait.ForCellChange("Cut2");
            Core.Sleep(2500);
            Core.JumpWait();
        }

        //7624    EN GARDE!
        if (!Story.QuestProgression(7624))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(7624);
            // Core.Join("stonewooddeep", "r2", "Left");
            Core.KillMonster("stonewooddeep", "r2", "Right", "Asherion", "Defeat Asherion");
            Core.EnsureComplete(7624);
        }

        //7625    The Light Of Destiny
        if (!Story.QuestProgression(7625))
        {
            Core.EquipClass(ClassType.Farm);
            Core.EnsureAccept(7625);
            Core.GetMapItem(7529, 1, "stonewooddeep");
            Core.EnsureComplete(7625);
            Core.Logger("Cutscene > Wait > Jump > Continue");
            Bot.Wait.ForCellChange("Cut2");
            Core.Sleep(2500);
            Core.JumpWait();
        }

        //7626    Stuff for Dummies
        if (!Story.QuestProgression(7626))
        {
            Core.EnsureAccept(7626);
            Core.KillMonster("stonewooddeep", "r2", "Right", "Doomwood Treeant", "Sturdy Wood", 8);
            Story.MapItemQuest(7626, "stonewooddeep", 7530, 8);
        }

        //7627    Build the Dummies
        if (!Story.QuestProgression(7627))
        {
            Core.EnsureAccept(7627);
            Core.KillMonster("stonewooddeep", "r3", "Right", "Doomwood Treeant", "Area Cleared", 10);
            Core.GetMapItem(7531, 6, "stonewooddeep");
            Core.EnsureComplete(7627);
            Core.Jump("r2");
            Core.Sleep(2500);
            Core.Jump("r3");
        }

        //7628    Battle the Dummies
        if (!Story.QuestProgression(7628))
        {
            // Core.Logger("Updatin the map because dummys are dumb");
            // Core.Join("whitemap");
            Core.EnsureAccept(7628);
            Core.KillMonster("stonewooddeep", "r3", "Left", "Target Dummy", "Target Dummy Slain", 6);
            Core.EnsureComplete(7628);
        }

        //7629    Lesson 1: Bravery
        if (!Story.QuestProgression(7629))
        {
            Core.EnsureAccept(7629);
            Core.KillMonster("stonewooddeep", "r3", "Right", "Doomwood Slime", "Slime Slain", 10);
            Core.EnsureComplete(7629);
        }

        //7630    Lesson 2: Armor
        if (!Story.QuestProgression(7630))
        {
            Core.EnsureAccept(7630);
            Core.KillMonster("stonewooddeep", "r3", "Right", "Doomwood Slime", "Stolen Armor", 7);
            Core.EnsureComplete(7630);
        }

        //7631    Lesson 3: Protection
        if (!Story.QuestProgression(7631))
        {
            Core.EnsureAccept(7631);
            Core.KillMonster("stonewooddeep", "r7", "Left", "CryptHacker", "Crypthacker Slain", 10);
            Core.KillMonster("stonewooddeep", "r7", "Left", "CryptHacker", "Unidentified Clue");
            Core.EnsureComplete(7631);
        }

        //7632    Lesson 4: Gather Intelligence
        Story.KillQuest(7632, "stonewooddeep", "CryptHacker");

        //7633    Lesson 5: Demolition
        if (!Story.QuestProgression(7633))
        {
            Core.EnsureAccept(7633);
            Core.KillMonster("stonewooddeep", "r3", "Right", "Doomwood Slime", "Acid Ooze", 6);
            Core.KillMonster("stonewooddeep", "r7", "Right", "NecroDrone", "Explosive Tech", 6);
            Core.EnsureComplete(7633);
        }

        //7634    Lesson 6: Searching
        Story.MapItemQuest(7634, "stonewooddeep", 7532);

        //7635    Get the Axe
        if (!Story.QuestProgression(7635))
        {
            Story.MapItemQuest(7635, "stonewooddeep", 7533);
            Core.Logger("Cutscene > Wait > Jump > Continue");
            Bot.Wait.ForCellChange("Cut2");
            Core.Sleep(2500);
            Core.JumpWait();
        }

        //7636    Never Give Up
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(7636, "stonewooddeep", "Sir Kut");
    }

    public void TechFortress()
    {
        if (Core.isCompletedBefore(7653))
            return;

        StonewoodForest();

        Story.PreLoad(this);

        //7637    I CPU U!
        Story.KillQuest(7637, "techfortress", "CryptHacker");

        //7638    Keep Droning On
        Story.KillQuest(7638, "techfortress", "NecroDrone");

        //7639    Gathering Data
        Story.KillQuest(7639, "techfortress", "CryptHacker");

        //7640    Signal Flare
        if (!Story.QuestProgression(7640))
        {
            Core.EnsureAccept(7640);
            Core.GetMapItem(7561, 1, "techfortress");
            Core.HuntMonster("techfortress", "NecroDrone", "Flare", 1);
            Core.EnsureComplete(7640);
        }

        //7641    Medal Time
        if (!Story.QuestProgression(7641))
        {
            Core.EnsureAccept(7641);
            Core.HuntMonster("techfortress", "NecroDrone", "NecroMedals", 5);
            Core.EnsureComplete(7641);
        }

        //7642    Hidden Data
        Story.KillQuest(7642, "techfortress", "NecroMech");

        //7643    Pass the CPU
        Story.KillQuest(7643, "techfortress", "CPU");

        //7644    The Guards
        Story.KillQuest(7644, "techfortress", "DoomBorg Guard");

        //7645    Circui-try This
        Story.KillQuest(7645, "techfortress", "DoomBorg Guard");

        //7646    The Thing
        Story.KillQuest(7646, "techfortress", "Vortrix");

        //7653    The Final Form
        Story.KillQuest(7653, "techfortress", "MechaVortrix");
    }

    #endregion

    public void AQWZombies()
    {
        if (Core.isCompletedBefore(2128))
            return;

        TechFortress();

        Story.PreLoad(this);

        //2093    Undead Assault
        Story.KillQuest(2093, "doomhaven", "Skeletal Soldier", AutoCompleteQuest: false);
        //insurance
        if (Bot.Quests.CanComplete(2093))
            Core.EnsureComplete(2093);

        //2094    Skull Crusher Mountain
        Story.KillQuest(2094, "doomhaven", "Skeletal Ice Mage");

        //2095    The Undead Giant
        Story.KillQuest(2095, "doomhaven", "Angry Undead Giant");

        //2096    Talk to the Knights
        Story.MapItemQuest(2096, "doomhaven", 1174, 5);

        //2097    Defend the Throne Room
        Story.KillQuest(2097, "doomhaven", "Skeletal Viking");

        foreach (
            int Quest in new[]
            {
                2117, // Rolith Defeated
                2120, // Lair
                2121, // Mythsong
                2122, // Arcangrove
                2123, // Willowshire
                2119, // Battleon
            }
        )
        {
            Story.ChainQuest(Quest);
            Bot.Wait.ForQuestComplete(Quest);
        }

        //2124    Keep the Area Clear
        Story.KillQuest(2124, "doomwar", "Angry Zombie");

        //2125    Defeat Zombie Dragons
        Story.KillQuest(2125, "doomwar", "Zombie Dragon");

        //2126    Defeat Your Fallen Friends
        if (!Story.QuestProgression(2126))
        {
            Core.EquipClass(ClassType.Farm);
            Core.EnsureAccept(2126);
            Core.KillMonster("doomwar", "r5", "Left", "*", "Fallen Friend Defeated", 4);
            Core.EnsureComplete(2126);
        }

        //2127    Long Unlive the King!
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(2127, "doomwar", "Zombie King Alteon");

        //2128    Dark Sepulchure Must be Slain!
        Story.KillQuest(2128, "sepulchure", "Dark Sepulchure");
    }
}
