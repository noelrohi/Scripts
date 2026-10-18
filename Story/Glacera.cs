/*
name: Glacera Story
description: This will finish the Glacera Story.
tags: story, quest, glacera
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class GlaceraStory
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

        DoAll();

        Core.SetOptions(false);
    }

    public void DoAll(bool Cryomancer = false)
    {
        FrozenTower(Cryomancer);
        FrozenRuins();
        Glacera();
        Glacera1();
        FrozenRuins2();
        Northstar();
        FrostRuins();
        IceWindPass();
        IceDungeon();
    }

    public void FrozenTower(bool Cryomancer = false)
    {
        if (Core.isCompletedBefore(3937))
            return;

        Story.PreLoad(this);

        // Seek the Tower
        Story.MapItemQuest(3907, "frozentower", 3022);

        // A n-Ice Beginning
        Story.KillQuest(3908, "frozentower", "Polar Elemental");

        // Search for Syrrus
        Story.MapItemQuest(3909, "frozentower", 3019);

        // Building the Base
        Story.MapItemQuest(3910, "frozentower", 3004, 13);

        // Refugee Roundup
        if (!Story.QuestProgression(3911))
        {
            Core.EnsureAccept(3911);
            Core.HuntMonster("frozentower", "Frostwyrm", "Refugee Saved", 7);
            Core.EnsureComplete(3911);
        }

        // Retrieve the Water Starstone
        Story.KillQuest(3912, "frozentower", "FrostDeep Dweller");

        // Magical Attraction
        Story.MapItemQuest(3913, "frozentower", 3005, 13);

        // Frozen Blood
        Story.KillQuest(3914, "frozentower", "Twisted Ice");

        // Retrieve the Fire Starstone
        Story.MapItemQuest(3915, "frozentower", 3006);

        // Defend the Tower!
        Story.KillQuest(3916, "frozentower", "Polar Elemental");

        // Refugee Rescue Run
        Story.MapItemQuest(3917, "frozentower", 3007, 6);

        // Retrieve the Earth Starston
        Story.MapItemQuest(3918, "frozentower", 3013);

        // Polar Penetration and Progress
        if (!Story.QuestProgression(3919))
        {
            Core.EnsureAccept(3919);
            Core.GetMapItem(3008, 6, "frozentower");
            Core.HuntMonster("frozentower", "Polar Elemental", "Ice Sample", 6);
            Core.EnsureComplete(3919);
        }

        // Save the Astronomer Apprentice
        if (!Story.QuestProgression(3920))
        {
            Core.EnsureAccept(3920);
            Core.GetMapItem(3020, 1, "frozentower");
            Core.HuntMonster("frozentower", "Polar Elemental", "Polar Elemental Guard Defeated", 4);
            Core.HuntMonster("frozentower", "Ice Wolf", "Ice Wolf Guard Defeated", 5);
            Core.EnsureComplete(3920);
        }

        // Glacial Elixir
        if (!Story.QuestProgression(3921))
        {
            Core.EnsureAccept(3921);
            Core.GetMapItem(3017, 6, "frozentower");
            Core.HuntMonster("frozentower", "FrostDeep Dweller", "FrostDeep Venom", 1);
            Core.EnsureComplete(3921);
        }

        // Retrieve the Energy Starstone
        Story.KillQuest(3922, "frozentower", "Polar Elemental");

        // Marking the Future
        Story.KillQuest(3923, "frozentower", "Frostwyrm");

        //Glacial Shift
        Story.MapItemQuest(3924, "frozentower", 3009, 6);

        // Divination Draft
        if (!Story.QuestProgression(3925))
        {
            Story.MapItemQuest(3925, "frozentower", 3012, 4);
            Story.MapItemQuest(3925, "frozentower", 3011, 4);

            Core.HuntMonster("frozentower", "Frostwyrm", "Frozen Tail", 5);
            Core.HuntMonster("frozentower", "Arctic Eel", "Frozen Scale", 5);
            Core.EnsureComplete(3925);
        }

        // Retrieve the Light StarStone
        Story.MapItemQuest(3926, "frozentower", 3021);

        // The Future is Bright
        Story.MapItemQuest(3927, "frozentower", 3014, 7);

        // Bled Bone Dry
        Story.KillQuest(3928, "frozentower", "Arctic Eel");

        // Chill of Fear
        Story.KillQuest(3929, "frozentower", "Polar Elemental");

        // Retrieve the Darkness Starstone
        Story.KillQuest(3930, "frozentower", "Twisted Ice");

        // Web of Fear
        Story.KillQuest(3931, "frozentower", "Frostwyrm");

        // Frozen in Time
        Story.MapItemQuest(3932, "frozentower", 3016, 13);

        // Heart of the Matter
        Story.KillQuest(3933, "frozentower", "Ice Wolf");

        // Retrieve the Wind Starstone
        Story.KillQuest(3934, "frozentower", "Rotten Ice");

        // Create the Gate
        if (!Story.QuestProgression(3935))
        {
            Core.EnsureAccept(3935);
            Core.GetMapItem(3018, 13, "frozentower");
            Core.HuntMonster("frozentower", "Ice Wolf", "Blood of the Ice Wolf", 1);
            Core.EnsureComplete(3935);
        }

        // Drive Back the Invaders
        if (!Story.QuestProgression(3936))
        {
            Core.EnsureAccept(3936);
            Core.HuntMonster("frozentower", "Frost Invader", "FrostSpawn Invader defeated", 10);
            Core.EnsureComplete(3936);
        }

        // Defeat the FrostSpawn Invaders
        if (!Story.QuestProgression(3937))
        {
            Core.EnsureAccept(3937);
            Core.HuntMonster("frozentower", "Frost Fangbeast", "Fangbeasts defeated", 15);
            Core.EnsureComplete(3937);
        }

        if (Cryomancer)
            return;
    }

    public void FrozenRuins()
    {
        if (Core.isCompletedBefore(3946))
            return;

        FrozenTower();

        Story.PreLoad(this);
        // FrozenRuins

        // FangBeast Bash-up
        Story.KillQuest(3941, "frozenruins", "Frost Fangbeast");

        // Ravage the Reapers
        Story.KillQuest(3942, "frozenruins", "Frost Reaper");

        // Oh the Humanity
        Story.KillQuest(3943, "frozenruins", "Frost Reaper");

        // Close the Gate
        Story.KillQuest(3944, "frozenruins", "Frost Reaper");

        // Form the Lock
        if (!Story.QuestProgression(3945))
        {
            Core.EnsureAccept(3945);
            Core.GetMapItem(3050, 10, "frozenruins");
            Core.HuntMonster("frozenruins", "Frozen Moglinster", "Frostbreath Weed", 5);
            Core.EnsureComplete(3945);
        }

        // Glacera
        if (!Story.QuestProgression(3946))
        {
            Core.EnsureAccept(3946);
            if (!Core.CheckInventory(27357))
                Core.HuntMonster("frozenruins", "Frost Reaper", "Mercury");
            Core.EnsureComplete(3946);
        }
    }

    public void Glacera()
    {
        if (Core.isCompletedBefore(3950))
            return;

        FrozenRuins();
        // Glaera

        //A Frosty Welcome
        if (!Story.QuestProgression(3947))
        {
            Core.EnsureAccept(3947);
            Core.Join("glacera");
            Bot.Wait.ForMapLoad("glacera");
            Core.GetMapItem(3048, 1, "glacera");
            Core.EnsureComplete(3947);
            // Ensure we're out of a potent cutscene/move cell
            Core.Jump("Enter", "Spawn");
        }

        // Key to the Fortress
        Story.KillQuest(3948, "glacera", "Frost Invader");
    }

    public void Glacera1()
    {
        if (Core.isCompletedBefore(3950))
            return;

        Glacera();

        // Breaking Boulders
        Story.MapItemQuest(3949, "glacera", 3049, 6);

        // The Scythe of Vengeance
        Story.MapItemQuest(3950, "Glacera", 3047);
    }

    public void FrozenRuins2()
    {
        if (Core.isCompletedBefore(3954))
            return;

        Glacera1();

        // FrozenRuins encore

        // // Rescue the Refugees
        if (!Story.QuestProgression(3951))
        {
            Core.EnsureAccept(3951);
            Core.HuntMonster("frozenruins", "Frost Invader", "Ice Beast Destroyed", 10);
            Core.EnsureComplete(3951);
        }

        // Defeat the Fangbeasts
        Story.KillQuest(3952, "frozenruins", "Frost Fangbeast");

        // Destroy the Frost Reapers
        Story.KillQuest(3953, "frozenruins", "Frost Reaper");

        // FrostSpawn General Takedown
        Story.KillQuest(3954, "frozenruins", "Frost General");
    }

    public void Northstar()
    {
        if (Core.isCompletedBefore(3971))
            return;

        FrozenRuins2();

        // Northstar

        // From Refugee to Enemy
        if (!Story.QuestProgression(3958))
        {
            Core.EnsureAccept(3958);
            Core.HuntMonster("northstar", "Monstrous Refugee", "Monstrous Refugee defeated", 3);
            Core.HuntMonster("northstar", "Frost Invader", "Frost Invader defeated", 5);
            Core.EnsureComplete(3958);
        }

        // Fangs and Blades
        if (!Story.QuestProgression(3959))
        {
            Core.EnsureAccept(3959);
            Core.HuntMonster("northstar", "Frost Fangbeast", "FrostSpawn defeated", 7);
            Core.EnsureComplete(3959);
        }

        // Reaping the Refugees
        Story.KillQuest(3960, "northstar", "Frost Reaper");

        // Saving Syrrus' Spirit
        if (!Story.QuestProgression(3961))
        {
            Core.EnsureAccept(3961);
            Core.GetMapItems(new[] { (3060, 5), (3061, 7), (3073, 5) }, "northstar");
            Core.HuntMonster("northstar", "Frost Reaper", "Lodestone Piece", 10);
            Core.EnsureComplete(3961);
        }

        // It's a Trap!
        Story.MapItemQuest(3972, "northstar", 3063, 10);

        // Feast or Famine
        if (!Story.QuestProgression(3973))
        {
            Core.EnsureAccept(3973);
            Core.HuntMonster("northstar", "Frost Fangbeast", "Frozen Dinners", 10);
            Core.EnsureComplete(3973);
        }

        // Decipher the Freezing
        if (!Story.QuestProgression(3974))
        {
            Core.EnsureAccept(3974);
            Core.HuntMonster("northstar", "Frost Fangbeast", "FrostSpawn Spine", 10);
            Core.HuntMonster("northstar", "Frost Fangbeast", "FrostSpawn Scale", 10);
            Core.EnsureComplete(3974);
        }

        // A New Frost Monster
        Story.KillQuest(3970, "northstar", "The Queen's Gift");

        // Defeat Karok!
        Story.KillQuest(3971, "northstar", "Karok The Fallen");
    }

    public void FrostRuins()
    {
        if (Core.isCompletedBefore(3954))
            return;
        Glacera1();

        // Rescue the Refugees
        Story.KillQuest(3951, "frozenruins", "Frost Invader");

        // Defeat the Fangbeasts
        Story.KillQuest(3952, "frozenruins", "Frost Fangbeast");

        // Destroy the Frost Reapers
        Story.KillQuest(3953, "frozenruins", "Frost Reaper");

        // FrostSpawn General Takedown
        Story.KillQuest(3954, "frozenruins", "Frost General");
    }

    public void IceWindPass()
    {
        if (Core.isCompletedBefore(5601))
            return;
        FrostRuins();

        // Where is Karok?
        Story.MapItemQuest(5587, "IceWindPass", 5074, 5);

        // Cloaking Spell
        Story.KillQuest(5588, "IceWindPass", "Glacial Elemental");

        // Splattered Mana
        if (!Story.QuestProgression(5589))
        {
            Core.EnsureAccept(5589);
            Core.GetMapItem(5075, 5, "IceWindPass");
            Core.HuntMonster("icewindpass", "Glacial Elemental", "Iced Mana", 9);
            Core.EnsureComplete(5589);
        }

        // Dispell the Spell
        Story.KillQuest(5590, "IceWindPass", "Polar Golem");

        // Catch Up to Karok
        Story.KillQuest(5591, "IceWindPass", "Frost Invader");

        // Blast the Frostspawn Symbiote
        Story.KillQuest(5592, "IceWindPass", "Frostspawn Symbiote");

        // Keep Going!
        Story.KillQuest(5593, "IceWindPass", "Frost Invader");

        // Take it Down
        Story.KillQuest(5594, "IceWindPass", "Frostspawn Horror");

        // Keep the Frostspawn Away!
        if (!Story.QuestProgression(5595))
        {
            Core.EnsureAccept(5595);
            Core.HuntMonster("icewindpass", "Frost Invader", "Frost Invader Slain", 5);
            Core.HuntMonster("icewindpass", "Frostspawn Troll", "Frostspawn Troll Slain", 5);
            Core.EnsureComplete(5595);
        }

        // Take a Break from Fighting
        if (!Story.QuestProgression(5596))
        {
            Core.EnsureAccept(5596);
            Core.HuntMonster("icewindpass", "Polar Golem", "Living Snow", 3);
            Core.HuntMonster("icewindpass", "Glacial Elemental", "Elemental Icicles", 3);
            Core.EnsureComplete(5596);
        }

        // As these quests are pre-unlocked.. it wont do them if we put them under a quest check. annoyingly.

        //Fight For Kezeroth! && (Mega) Fight For Kezeroth! - 5597, 5598
        //Fight For Karok! && (Mega) Fight For Karok! - 5599, 5600
        Core.EnsureAcceptmultiple(new[] { 5597, 5598, 5599, 5600 });
        Core.EquipClass(ClassType.Farm);
        Core.HuntMonster("icewindwar", "Glaceran Defender", "FrostSpawn Medal", 10);
        Core.HuntMonster("icewindwar", "Glaceran Defender", "Mega Frostspawn Medal", 5);

        Core.HuntMonster("icewindwar", "Frost Invader", "World Ender Medal", 10);
        Core.HuntMonster("icewindwar", "Frost Invader", "Mega World Ender Medal", 5);
        Core.EnsureComplete(new[] { 5597, 5598, 5599, 5600 });

        // What is THAT?
        Story.KillQuest(5601, "icewindwar", "Soricomorpha");
    }

    public void IceDungeon()
    {
        if (Core.isCompletedBefore(7841))
            return;
        IceWindPass();

        // Cold Open
        if (!Story.QuestProgression(7832))
        {
            Core.EnsureAccept(7832);
            Core.HuntMonster("icedungeon", "Ice Symbiote", "Ice Symbiote Shattered", 5);
            Core.HuntMonster("icedungeon", "Frosted Banshee", "Frosted Banshee Cloth Scrap", 5);
            Core.HuntMonster("icedungeon", "Frozen Undead", "Frozen Undead Laid to Rest", 5);
            Core.EnsureComplete(7832);
        }

        // Frost Fight
        if (!Story.QuestProgression(7833))
        {
            Core.EnsureAccept(7833);
            Core.HuntMonster("icedungeon", "Spirit of Ice", "Spirit of Ice Captured", 5);
            Core.HuntMonster("icedungeon", "Ice Crystal", "Ice Crystal Collected", 5);
            Core.HuntMonster("icedungeon", "Frigid Spirit", "Frigid Spirit Banished", 3);
            Core.EnsureComplete(7833);
        }

        // Freeze Frame
        if (!Story.QuestProgression(7834))
        {
            Core.EnsureAccept(7834);
            Core.HuntMonster("icedungeon", "Living Ice", "Living Ice Shattered", 5);
            Core.HuntMonster("icedungeon", "Crystallized Elemental", "Crystalized Elemental Defeated", 5);
            Core.HuntMonster("icedungeon", "Frozen Demon", "Frozen Demon Banished", 3);
            Core.EnsureComplete(7834);
        }

        // Glace's Trial
        Story.KillQuest(7835, "IceDungeon", "Image of Glace");

        // Abel's Trial
        Story.KillQuest(7836, "IceDungeon", "Abel");

        // The Final Trial
        Story.KillQuest(7837, "IceDungeon", "Shade of Kyanos");

        // Basic Ingredients
        if (!Story.QuestProgression(7838))
        {
            Core.EnsureAccept(7838);
            Core.HuntMonster("icedungeon", "Frosted Banshee", "Frosted Banshee Defeated", 10);
            Core.HuntMonster("icedungeon", "Frozen Undead", "Frozen Undead Defeated", 10);
            Core.HuntMonster("icedungeon", "Ice Symbiote", "Ice Symbiote Defeated", 10);
            Core.EnsureComplete(7838);
        }

        // Cool Flavor
        if (!Story.QuestProgression(7839))
        {
            Core.EnsureAccept(7839);
            Core.HuntMonster("icedungeon", "Spirit of Ice", "Spirit of Ice Defeated", 10);
            Core.HuntMonster("icedungeon", "Ice Crystal", "Ice Crystal Defeated", 10);
            Core.HuntMonster("icedungeon", "Frigid Spirit", "Frigid Spirit Defeated", 10);
            Core.EnsureComplete(7839);
        }

        // Chilled to Perfection
        if (!Story.QuestProgression(7840))
        {
            Core.EnsureAccept(7840);
            Core.HuntMonster("icedungeon", "Living Ice", "Living Ice Defeated", 5);
            Core.HuntMonster("icedungeon", "Crystallized Elemental", "Crystallized Elemental Defeated", 5);
            Core.HuntMonster("icedungeon", "Frozen Demon", "Frozen Demon Defeated", 5);
            Core.EnsureComplete(7840);
        }

        // Icing on the Cake
        if (!Story.QuestProgression(7841))
        {
            Core.EnsureAccept(7841);
            Core.HuntMonster("icedungeon", "Image of Glace", "Glace's Approval", 1);
            Core.HuntMonster("icedungeon", "Abel", "Abel's Approval", 1);
            Core.HuntMonster("icedungeon", "Shade of Kyanos", "Kyanos' Approval", 1);
            Core.EnsureComplete(7841);
        }
    }
}
