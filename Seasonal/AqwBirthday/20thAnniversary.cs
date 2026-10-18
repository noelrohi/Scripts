/*
name: AQW 20th Anniversary Story
description: This will finish the storyline of 20th anniversary.
tags: 20th-anniv, free-acs, seasonal
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class AnniversaryofDoom
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

        Anniversary();

        Core.SetOptions(false);
    }

    public void Anniversary()
    {
        if (Core.isCompletedBefore(8898) && !Core.isSeasonalMapActive("yulgarparty"))
            return;

        Story.PreLoad(this);

        Yulgarparty();
        Zombae();
        Mermaidsushi();
        FREE500ACSPLEASE();
        Spacepwny();
        Deathofgames();
    }

    public void Yulgarparty()
    {
        if (Core.isCompletedBefore(8876))
            return;

        // 8874 Friends by the Fireplace
        Story.MapItemQuest(
            8874,
            "yulgarparty",
            new[] { 10675, 10676, 10677, 10678, 10679, 10680, 10681, 10682, 10683 }
        );

        // 8875 Allies at the Entrance,
        Story.MapItemQuest(
            8875,
            "yulgarparty",
            new[] { 10684, 10685, 10686, 10687, 10688, 10689, 10690 }
        );

        // 8876 Mates near the Music,
        Story.MapItemQuest(
            8876,
            "yulgarparty",
            new[] { 10691, 10692, 10693, 10694, 10695, 10696, 10697, 10698, 10699 }
        );
    }

    public void Zombae()
    {
        if (Core.isCompletedBefore(8883))
            return;

        Core.EquipClass(ClassType.Farm);

        // 8877 O-(De)kay Cupid,
        Story.KillQuest(8877, "zombae", "Bachelor");

        // 8878 Ghosting my Ex-Goulfiends,
        Story.KillQuest(8878, "zombae", "Bachelorette");

        // 8879 You're Dead to Me,
        if (!Story.QuestProgression(8879))
        {
            Core.EnsureAccept(8879);
            Core.GetMapItem(10700, 1, "zombae");
            Core.HuntMonster("zombae", "Zombette", "Girl With Brains", 1);
            Core.EnsureComplete(8879);
        }

        // 8880 Swipe 4 Rich lich
        if (!Story.QuestProgression(8880))
        {
            Core.EnsureAccept(8880);
            Core.GetMapItem(10701, 1, "zombae");
            Core.HuntMonster("zombae", "Zombro", "Richest Zombie", 1);
            Core.EnsureComplete(8880);
        }

        // 8881 A Real Grave Digger
        if (!Story.QuestProgression(8881))
        {
            Core.EnsureAccept(8881);
            Core.GetMapItem(10702, 1, "zombae");
            Core.HuntMonster("zombae", "Zombroski", "GRAAAAAINS", 1);
            Core.EnsureComplete(8881);
        }

        // 8882 The Love of your Unlife
        if (!Story.QuestProgression(8882))
        {
            Core.EnsureAccept(8882);
            Core.GetMapItem(10703, 1, "zombae");
            Core.HuntMonster("zombae", "Zombetter", "Fiend With Benefits", 1);
            Core.EnsureComplete(8882);
        }

        Core.EquipClass(ClassType.Solo);
        // 8883 The Ultimate Ghoul Fiend
        Story.KillQuest(8883, "zombae", "Ghoul Fiend");
    }

    public void Mermaidsushi()
    {
        if (Core.isCompletedBefore(8892))
            return;

        Core.EquipClass(ClassType.Farm);
        // 8884 S'KARIN WANT MOGLIN POPPERS,
        Story.KillQuest(8884, "mermaidsushi", "Undead Moglin");

        // 8885 S'KARIN WANT HAMSTEAK,
        Story.KillQuest(8885, "mermaidsushi", "Haunted Hamster");

        // 8886 S'KARIN WANT A SKELLO MOLD,
        Story.KillQuest(8886, "mermaidsushi", "SkeleKitten");

        // 8887 S'KARIN WANT OFFAL BIRD FOOD,
        Story.KillQuest(8887, "mermaidsushi", "Undead Turkey");

        Core.EquipClass(ClassType.Solo);
        // 8888 S'KARIN WANT WINGS,
        Story.KillQuest(8888, "mermaidsushi", "Dedbull");

        // 8889 S'KARIN WANT MERMAID SUSHI
        Story.MapItemQuest(8889, "mermaidsushi", 10719);

        // 8890 Mer-Angel Fish Food?!
        Story.KillQuest(8890, "mermaidsushi", "Necro Minion");

        // 8891 S'KARIN NO WANT GO VIRAL
        Story.KillQuest(8891, "mermaidsushi", "Customer S'karin");

        // 8892 Gore-Made for This
        Story.KillQuest(8892, "mermaidsushi", "Die Fieri");
    }

    public void FREE500ACSPLEASE()
    {
        if (Core.isCompletedBefore(8893))
            return;

        Bot.Handlers.Remove("AFK Handler");

        // 8893 AFK Quest
        if (!Story.QuestProgression(8893))
        {
            Core.Join("afkquest");
            Bot.Send.Packet("%xt%zm%afk%0%false%");
            Core.Logger(
                $"**DO NOT CLICK THE GAME SCREEN** this Will Take ~5minutes, Go touch some grass 👍"
            );
            Core.Sleep(360000);
            Core.Logger("Game Complete. You're Welcome for the Acs");
        }
    }

    public void Spacepwny()
    {
        if (Core.isCompletedBefore(8898))
            return;

        Bot.Options.RestPackets = true;

        Core.EquipClass(ClassType.Farm);
        // 8894 Plague of Zom-Bays
        if (!Story.QuestProgression(8894))
        {
            Core.EnsureAccept(8894);
            Core.KillMonster("spacepwny", "Enter", "Spawn", "Zom-Bay", "Zom-Bay Defeated", 10);
            Core.EnsureComplete(8894);
        }

        // 8895 Foal of (Thorough) Dread
        if (!Story.QuestProgression(8895))
        {
            Core.EnsureAccept(8895);
            Core.KillMonster("spacepwny", "r2", "Left", "Thoroughdred", "Thoroughdred Defeated", 10);
            Core.EnsureComplete(8895);
        }

        // 8896 Create a Stable Base
        if (!Story.QuestProgression(8896))
        {
            Core.EnsureAccept(8896);
            Core.KillMonster("spacepwny", "r2", "Left", "NecroPrancer", "NecroPrancer Defeated", 10);
            Core.EnsureComplete(8896);
        }

        // 8897 Bonies & Bonies & Bonies
        if (!Story.QuestProgression(8897))
        {
            Core.EnsureAccept(8897);
            Core.KillMonster("spacepwny", "r3", "Left", "*", "Mare Carnage", 100);
            Core.EnsureComplete(8897);
        }

        Core.EquipClass(ClassType.Solo);
        // 8898 Defeat Mr DED
        Story.KillQuest(8898, "spacepwny", "Mr DED");
    }

    public void Deathofgames()
    {
        if (Core.isCompletedBefore(8924))
            return;

        Story.PreLoad(this);

        //Dungeons & DOOMFights 8899
        if (!Story.QuestProgression(8899))
        {
            Core.EnsureAccept(8899);
            Core.HuntMonster("deathofgames", "8-Bit Skelly", "8-Bit Skeletons Destroyed", 8);
            Core.HuntMonster("deathofgames", "8-Bit Sepulchure", "8-bit Sepulchure Deboned", 1);
            Core.EnsureComplete(8899);
        }

        //AQ3Destruction 8900
        if (!Story.QuestProgression(8900))
        {
            Core.EnsureAccept(8900);
            Core.HuntMonster("deathofgames", "3D Flying Eye", "Flying Eye De-Sighted", 5);
            Core.HuntMonster("deathofgames", "Trolluk", "Trolluk Destroyed", 1);
            Core.HuntMonster("deathofgames", "Clawg", "Clawg De-clawed", 3);
            Core.EnsureComplete(8900);
        }

        //OverSoul Under Attack 8901
        if (!Story.QuestProgression(8901))
        {
            Core.EnsureAccept(8901);
            Core.HuntMonster("deathofgames", "Vampire Knight", "Vampire Knight Defeated", 6);
            Core.HuntMonster("deathofgames", "Black Dragon", "Black Dragon Defeated", 1);
            Core.EnsureComplete(8901);
        }

        //HeroSmashed 8902
        if (!Story.QuestProgression(8902))
        {
            Core.EnsureAccept(8902);
            Core.HuntMonster("deathofgames", "Rider", "Riders Run Off", 5);
            Core.HuntMonster("deathofgames", "Blaster Master", "Blaster Master Blasted", 1);
            Core.HuntMonster("deathofgames", "Super Death", "Super Death... Deaded", 1);
            Core.EnsureComplete(8902);
        }

        //EpicDuel vs DoG 8907
        if (!Story.QuestProgression(8907))
        {
            Core.EnsureAccept(8907);
            Core.HuntMonster("deathofgames", "God of War", "God of War Defeated", 1);
            Core.HuntMonster("deathofgames", "Cyber Hunter", "Cyber Hunters Defeated", 6);
            Core.EnsureComplete(8907);
        }

        //AQWorlds At Risk 8920
        if (!Story.QuestProgression(8920))
        {
            Core.EnsureAccept(8920);
            Core.HuntMonster("deathofgames", "Skeletal Fire Mage", "Skeletal Enemies Defeated", 8);
            Core.HuntMonster("deathofgames", "Drakath", "Drakath Defeated", 1);
            Core.EnsureComplete(8920);
        }

        //MechQuest For Glory 8921
        if (!Story.QuestProgression(8921))
        {
            Core.EnsureAccept(8921);
            Core.HuntMonster("deathofgames", "Newbatron Prime", "Newbatron Primes Defeated", 5);
            Core.HuntMonster("deathofgames", "ShadowScythe Mecha", "ShadowScythe Mecha Defeated", 3);
            Core.HuntMonster("deathofgames", "SkullCrusher Mecha", "Skull Crusher Mecha Defeated", 1);
            Core.EnsureComplete(8921);
        }

        //DragonFables &amp; Lore 8922
        if (!Story.QuestProgression(8922))
        {
            Core.EnsureAccept(8922);
            Core.HuntMonster("deathofgames", "Titan Fluffy", "Titan Fluffy Defeated", 1);
            Core.HuntMonster("deathofgames", "Fire Elemental", "Fire Elementals Snuffed", 5);
            Core.HuntMonster("deathofgames", "Xan", "Xan Defeated", 1);
            Core.EnsureComplete(8922);
        }

        //AdventureQuest for Victory 8923
        if (!Story.QuestProgression(8923))
        {
            Core.EnsureAccept(8923);
            Core.HuntMonster("deathofgames", "Halenro the Paladin", "Halenro Run Off", 1);
            Core.HuntMonster("deathofgames", "Moglin Ghost", "Moglin Ghosts Banished", 5);
            Core.HuntMonster("deathofgames", "Mysterious Stranger", "Mysterious Stranger Defeated", 1);
            Core.EnsureComplete(8923);
        }

        //Battle On... FOREVER! 8924
        Story.KillQuest(8924, "deathofgames", "Death of Games");
    }
}
