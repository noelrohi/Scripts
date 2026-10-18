/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class CoreDOY
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
        Core.RunCore();
    }

    public void DoAll()
    {
        YokaiPirate();
        YokaiTreasure();
        HakuVillage();
        HakuWar();
        YokaiPortal();
        YokaiRealm();
        NovaShrine();
    }

    public void YokaiPirate()
    {
        if (Core.isCompletedBefore(9387))
            return;

        Story.PreLoad(this);

        Core.EquipClass(ClassType.Farm);

        // Wokou 9378
        Story.MapItemQuest(9378, "yokaipirate", new[] { 12133, 12134, 12135 });

        // Shanty Sha-N-Ti 9379
        Story.KillQuest(9379, "yokaipirate", "Disguised Pirate");

        // Yokai's Ark 9380
        if (!Story.QuestProgression(9380))
        {
            Core.EnsureAccept(9380);
            Core.HuntMonster("yokaipirate", "Serpent Warrior", "Serpent Badge", 7);
            Story.MapItemQuest(9380, "yokaipirate", new[] { 12136, 12137 });
        }

        // Fashion Fathoms 9381
        Story.KillQuest(9381, "yokaipirate", "Disguised Pirate");

        // Hoo Wants a Cracker 9382
        Story.KillQuest(9382, "yokaipirate", "Noble Owl");

        // Papers Please 9383
        Story.MapItemQuest(9383, "yokaipirate", 12138, 7);

        Core.EquipClass(ClassType.Farm);
        // King and Coral Snakes 9384
        if (!Story.QuestProgression(9384))
        {
            Core.EnsureAccept(9384);
            Core.HuntMonster("yokaipirate", "Disguised Pirate", "Pirate Interrogated", 6);
            Core.HuntMonster("yokaipirate", "Serpent Warrior", "Warrior Interrogated", 6);
            Story.MapItemQuest(9384, "yokaipirate", 12139);
        }

        // Horizon's Green Flash 9385
        Story.KillQuest(9385, "yokaipirate", "Noble Owl");

        // Highly Buoyant Metal Armor 9386
        if (!Story.QuestProgression(9386))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(9386);
            Core.HuntMonsterMapID("yokaipirate", 1, "Knight Captured", 8);
            Core.EnsureComplete(9386);
        }

        // Salty Roots 9387
        if (!Story.QuestProgression(9387))
        {
            Core.EquipClass(ClassType.Solo);
            Core.EnsureAccept(9387);
            Core.HuntMonsterMapID("yokaipirate", 11, "Neverglades Lord Dueled");
            Core.EnsureComplete(9387);
        }
    }

    public void YokaiTreasure()
    {
        if (Core.isCompletedBefore(9405))
            return;

        YokaiPirate();

        Story.PreLoad(this);

        Core.EquipClass(ClassType.Farm);

        // All Hands Report (9396)
        Story.MapItemQuest(9396, "yokaitreasure", new[] { 12162, 12163, 12164 });

        // Starving Ghosts (9397)
        Story.KillQuest(9397, "yokaitreasure", "Needle Mouth");

        // Sudden Striker (9398)
        Story.KillQuest(9398, "yokaitreasure", "Quicksilver");

        // Onmyoji Maiden (9399)
        Story.MapItemQuest(9399, "yokaitreasure", new[] { 12165, 12166 });

        // Proper Parley (9400)
        Story.KillQuest(9400, "yokaitreasure", "Imperial Warrior");

        // Hunger Pains (9401)
        Story.KillQuest(9401, "yokaitreasure", "Needle Mouth");

        // Terror-cotta Warriors (9402)
        Story.KillQuest(9402, "yokaitreasure", "Imperial Warrior");

        // Indoor Fireworks (9403)
        Story.MapItemQuest(9403, "yokaitreasure", 12167, 4);

        // Sons of Biscuit Eaters (9404)
        if (!Story.QuestProgression(9404))
        {
            Core.EnsureAccept(9404);
            Core.HuntMonster("yokaitreasure", "Needle Mouth", "Criminal Marker", 6);
            Core.HuntMonster("yokaitreasure", "Imperial Warrior", "Imperial Scale", 6);
            Core.EnsureComplete(9404);
        }

        // Pearl of my Heart (9405)
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(9405, "yokaitreasure", "Admiral Zheng");
        Core.EquipClass(ClassType.Farm);
    }

    public void HakuVillage()
    {
        if (Core.isCompletedBefore(9599))
            return;

        YokaiTreasure();

        Story.PreLoad(this);

        Core.EquipClass(ClassType.Farm);

        // 99th Birthday (9590)
        Story.KillQuest(9590, "hakuvillage", "Tsukumogami");

        // Red and Blue (9591)
        if (!Story.QuestProgression(9591))
        {
            Core.EnsureAccept(9591);
            Core.GetMapItem(12706, 1, "hakuvillage");
            Core.HuntMonster("hakuvillage", "Mountain Oni", "Clay Gourd", 10);
            Core.EnsureComplete(9591);
        }

        // Cold Sunshower (9592)
        if (!Story.QuestProgression(9592))
        {
            Core.EnsureAccept(9592);
            Core.GetMapItem(12707, 1, "hakuvillage");
            Core.HuntMonster("hakuvillage", "Tsukumogami", "Spirit Lights", 10);
            Core.EnsureComplete(9592);
        }

        // Inari Boom (9593)
        if (!Story.QuestProgression(9593))
        {
            Core.EnsureAccept(9593);
            Core.GetMapItems(new[] { (12708, 1), (12709, 1) }, "hakuvillage");
            Core.HuntMonster("hakuvillage", "Kitsune Spy", "Fried Tofu Skins", 12);
            Core.EnsureComplete(9593);
        }

        // Cui Niao (9594)
        if (!Story.QuestProgression(9594))
        {
            Core.EnsureAccept(9594);
            Core.GetMapItem(12710, 1, "hakuvillage");
            Core.HuntMonster("hakuvillage", "Nagami", "Mermeat", 12);
            Core.EnsureComplete(9594);
        }

        // Huli Jing (9595)
        Story.KillQuest(9595, "hakuvillage", "Kitsune Spy");

        // Aka Oni Ao Oni (9596)
        Story.KillQuest(9596, "hakuvillage", "Mountain Oni");

        // Stone Shangyuan Fest (9597)
        Story.KillQuest(9597, "hakuvillage", "Tsukumogami");

        // Nagami Sashimi (9598)
        Story.KillQuest(9598, "hakuvillage", "Nagami");

        // Divine Derision (9599)
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(9599, "hakuvillage", "Dai Tengu");
    }

    public void HakuWar()
    {
        if (Core.isCompletedBefore(9607))
            return;

        HakuVillage();

        Story.PreLoad(this);

        Core.EquipClass(ClassType.Farm);

        // Haku Medal (9601)
        Story.KillQuest(9601, "hakuwar", "Dark Zmey");

        // Tall Tails (9603)
        Story.KillQuest(9603, "hakuwar", "Kitsune Spy");

        // Flightless (9604)
        Story.KillQuest(9604, "hakuwar", "Dark Zmey");

        // NorthEast Ushitora (9605)
        Story.KillQuest(9605, "hakuwar", "Mountain Oni");

        // Zmey Gorynich (9606)
        Story.KillQuest(9606, "hakuwar", "Zmey Warrior");

        // Head Kukol'nyy (9607)
        if (!Core.isCompletedBefore(9607))
        {
            Core.EquipClass(ClassType.Solo);
            Story.KillQuest(9607, "hakuwar", "Zakhvatchik");
        }
    }

    public void YokaiPortal()
    {
        if (Core.isCompletedBefore(9676))
            return;

        HakuWar();

        Story.PreLoad(this);

        Core.EquipClass(ClassType.Farm);

        // Tense Reunion (9667)
        if (!Story.QuestProgression(9667))
        {
            Core.EnsureAccept(9667);
            Core.GetMapItems(new[] { (12982, 1), (12983, 1), (12984, 1) }, "yokaiportal");
            Core.EnsureComplete(9667);
        }

        // Kimon (9668)
        Story.KillQuest(9668, "yokaiportal", "Oni Spirits");

        // Shrine of Love (9669)
        Story.MapItemQuest(9669, "yokaiportal", new[] { 12985, 12986 });

        // Hoshi no Tama (9670)
        Story.KillQuest(9670, "yokaiportal", "Kitsune Spirits");

        // Childish Nostalgia (9671)
        Story.MapItemQuest(9671, "yokaiportal", new[] { 12987, 12988 });

        // Simple Wishes (9672)
        if (!Story.QuestProgression(9672))
        {
            Core.EnsureAccept(9672);
            Core.GetMapItems(new[] { (12989, 1), (12995, 1), (12990, 1) }, "yokaiportal");
            Core.HuntMonster("yokaiportal", "Snake Shikigami", "Quicksilver Venom", 21);
            Core.EnsureComplete(9672);
        }

        // Left Behind (9673)
        Story.KillQuest(9673, "yokaiportal", "Puppeted Dragonling");

        // Our Love (9674)
        Story.MapItemQuest(9674, "yokaiportal", new[] { 12991, 12992, 12993 });

        // Amano Iwato (9675)
        if (!Story.QuestProgression(9675))
        {
            Core.EnsureAccept(9675);
            Core.GetMapItem(12994, 1, "yokaiportal");
            Core.HuntMonster("yokaiportal", "Kitsune Spirits", "Fox Soul Pearl", 15);
            Core.HuntMonster("yokaiportal", "Puppeted Dragonling", "Seeded Dragon Heart", 15);
            Core.EnsureComplete(9675);
        }

        // Corrupted Protector (9676)
        if (!Core.isCompletedBefore(9676))
        {
            Core.EquipClass(ClassType.Solo);
            Story.KillQuest(9676, "yokaiportal", "Kitsune Kukol'nyy");
        }
    }

    public void YokaiRealm()
    {
        if (Core.isCompletedBefore(9689))
            return;

        YokaiPortal();

        Story.PreLoad(this);

        Core.EquipClass(ClassType.Farm);

        // Tail End (9680)
        if (!Story.QuestProgression(9680))
        {
            Core.EnsureAccept(9680);
            Core.GetMapItems(new[] { (13036, 1), (13037, 1) }, "yokairealm");
            Core.HuntMonster("yokairealm", "Snake Shikigami", "Tame Snake Shikigami", 18);
            Core.EnsureComplete(9680);
        }

        // A Thousand Hills (9681)
        if (!Story.QuestProgression(9681))
        {
            Core.EnsureAccept(9681);
            Core.GetMapItems(new[] { (13038, 1), (13039, 1) }, "yokairealm");
            Core.HuntMonster("yokairealm", "Snake Shikigami", "Snake Quicksilver", 18);
            Core.EnsureComplete(9681);
        }

        // Mount the Land and Skies (9682)
        if (!Story.QuestProgression(9682))
        {
            Core.EnsureAccept(9682);
            Core.GetMapItems(new[] { (13040, 1), (13041, 1) }, "yokairealm");
            Core.HuntMonster("yokairealm", "Puppeted Dragonling", "Eggshell Shard", 80);
            Core.EnsureComplete(9682);
        }

        // Distant Shores (9683)
        if (!Story.QuestProgression(9683))
        {
            Core.EnsureAccept(9683);
            Core.GetMapItems(new[] { (13042, 1), (13043, 1) }, "yokairealm");
            Core.HuntMonster("yokairealm", "Dark Zmey", "Tarnished Gold", 21);
            Core.EnsureComplete(9683);
        }

        // Taizi (9684)
        if (!Story.QuestProgression(9684))
        {
            Core.EnsureAccept(9684);
            Core.GetMapItem(13044, 1, "yokairealm");
            Core.HuntMonster("yokairealm", "Puppeted Dragonling", "Dragonling Bone", 15);
            Core.HuntMonster("yokairealm", "Dark Zmey", "Zmey Bone", 15);
            Core.EnsureComplete(9684);
        }

        // Begrudged Oni (9685)
        if (!Story.QuestProgression(9685))
        {
            Core.EnsureAccept(9685);
            Core.GetMapItems(new[] { (13045, 1), (13046, 1) }, "yokairealm");
            Core.HuntMonster("yokairealm", "Oni Spirits", "Dried Oni Blood", 1);
            Core.EnsureComplete(9685);
        }

        // Lingering Discord (9686)
        if (!Story.QuestProgression(9686))
        {
            Core.EnsureAccept(9686);
            Core.GetMapItem(13047, 1, "yokairealm");
            Core.HuntMonster("yokairealm", "Kitsune Spirits", "Chaotic Kitsunebi", 21);
            Core.EnsureComplete(9686);
        }

        // Yokai's Cycle (9687)
        if (!Story.QuestProgression(9687))
        {
            Core.EnsureAccept(9687);
            Core.HuntMonster("yokairealm", "Oni Spirits", "Oni Pacified", 15);
            Core.HuntMonster("yokairealm", "Kitsune Spirits", "Kitsune Pacified", 15);
            Core.EnsureComplete(9687);
        }

        // Kojutsu (9688)
        if (!Story.QuestProgression(9688))
        {
            Core.EquipClass(ClassType.Solo);
            Story.KillQuest(9688, "yokairealm", "Inugami");
        }

        // Ame no Murakumo (9689)
        if (!Story.QuestProgression(9689))
        {
            Core.EquipClass(ClassType.Solo);
            Story.KillQuest(9689, "yokairealm", "Mikoto Kukol'nyy");
        }
    }

    public void NovaShrine()
    {
        if (Core.isCompletedBefore(9802))
            return;

        Story.PreLoad(this);

        Core.EquipClass(ClassType.Farm);

        // Byakko 9798
        Story.KillQuest(9798, "lavarun", "Firestorm Tiger");

        // Suzaku 9799
        Story.KillQuest(9799, "dreampalace", "Flaming Harpy");

        // Genbu 9800
        if (!Story.QuestProgression(9800))
        {
            Core.EnsureAccept(9800);
            Core.KillMonster("yokaitreasure", "r3", "Left", "Quicksilver", "Silver Scales", 15);
            Core.KillMonster("hakuvillage", "r4", "Left", "Nagami", "Nagami Scales", 15);
            Core.EnsureComplete(9800);
        }

        // Seiryu 9801
        if (!Story.QuestProgression(9801))
        {
            Bot.Quests.UpdateQuest(9607);
            if (!Story.QuestProgression(9801))
            {
                Core.EnsureAccept(9801);
                Core.HuntMonster("hakuwar", "Zakhvatchik", "Zakhvatchik Fang", 1);
                Core.HuntMonster("hakuwar", "Zmey Warrior", "Zmey Fang", 15);
                Core.EnsureComplete(9801);
            }
        }
        // Hoshiyoru 9802
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(9802, "novashrine", "Nova Empyrean");
    }
}
