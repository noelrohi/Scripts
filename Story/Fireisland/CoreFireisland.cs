/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class CoreFireIsland
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

        Core.RunCore();

        Core.SetOptions(false);
    }

    public void CompleteFireIsland()
    {
        if (Core.isCompletedBefore(Core.IsMember ? 4157 : 4235))
            return;

        Embersea();
        Pyrewatch();
        Feverfew();
        Phoenixrise();
        Fireforge();
        Lavarun();
        Brimstone();
        Nightmare();
    }

    public void Embersea()
    {
        if (Core.isCompletedBefore(4055))
            return;

        Story.PreLoad(this);

        //Heat of Battle 4054
        if (!Story.QuestProgression(4054))
        {
            Core.EnsureAccept(4054);
            Core.HuntMonster("embersea", "Flame Soldier", "Flame Soldier slain", 3);
            Core.HuntMonster("embersea", "Storm Scout", "Storm Scout slain", 3);
            Core.EnsureComplete(4054);
        }

        //Light the Flame 4055
        if (!Story.QuestProgression(4055))
        {
            Core.EnsureAccept(4055);
            Core.GetMapItem(3153, 22, "Embersea");
            Core.HuntMonster("embersea", "Living Lava", "Living Lava slain", 4);
            Core.EnsureComplete(4055);
        }

        //Kill It With Fire [Member] 4056
        if (Core.IsMember)
        {
            if (!Story.QuestProgression(4056))
            {
                Core.EnsureAccept(4056);
                Core.HuntMonster("embersea", "Coal Creeper", "Coal Creeper slain", 5);
                Core.HuntMonster("embersea", "Pyradon", "Pyradon slain", 2);
                Core.HuntMonster("embersea", "Fyresyn", "Fyresyn slain", 1);
                Core.EnsureComplete(4056);
            }
        }
    }

    public void Pyrewatch()
    {
        if (Core.isCompletedBefore(4081))
            return;

        Embersea();

        Story.PreLoad(this);

        //Flee the Flames
        if (!Story.QuestProgression(4070))
        {
            Core.EnsureAccept(4070);
            Core.HuntMonster("pyrewatch", "Lavazard", "Lavazard Slain", 3);
            Core.HuntMonster("pyrewatch", "Fyreborn Tiger", "Tiger Slain", 5);
            Core.HuntMonster("pyrewatch", "Caustocrush", "Caustocrush Slain", 3);
            Core.EnsureComplete(4070);
        }

        //Taste of their own Medicine
        Story.KillQuest(4071, "pyrewatch", "Fire Pikeman");

        //A Jarring Discovery
        Story.MapItemQuest(4072, "pyrewatch", 3159, 12);

        //Push on to Pyrewatch
        if (!Story.QuestProgression(4073))
        {
            Core.EnsureAccept(4073);
            Core.HuntMonster("pyrewatch", "Firestorm Knight", "Battle Plans", 1);
            Core.HuntMonster("pyrewatch", "Firestorm Knight", "Peak Defended", 10);
            Core.EnsureComplete(4073);
        }

        //Friends of Pyrewatch Peak
        if (!Story.QuestProgression(4074))
        {
            Core.EnsureAccept(4074);
            Core.HuntMonster("Pyrewatch", "Fire Pikeman", "Pikeman Slain", 3);
            Core.HuntMonster("Pyrewatch", "Fire Pikeman", "Firestorm Helm", 6);
            Core.HuntMonster("Pyrewatch", "Flame Soldier", "Flame Soldier Slain", 3);
            Core.HuntMonster("Pyrewatch", "Caustocrush", "Caustocrush Slain", 3);
            Core.EnsureComplete(4074);
        }

        //A Salve to Soothe
        if (!Story.QuestProgression(4075))
        {
            Core.EnsureAccept(4075);
            Core.GetMapItem(3160, 5, "pyrewatch");
            Core.HuntMonster("pyrewatch", "Lavazard", "Glowing Embers", 5);
            Core.EnsureComplete(4075);
        }

        //Protect the Plague Sufferers 4076
        if (!Story.QuestProgression(4076))
        {
            Core.EnsureAccept(4076);
            Core.HuntMonster("Pyrewatch", "Lavazard", "Kindling", 3);
            Core.HuntMonster("Pyrewatch", "Caustocrush", "Flint and Steel");

            //multiple items with name "coal" will wig out otherwise
            while (!Bot.ShouldExit && !Core.CheckInventory(28565, 6))
                Core.HuntMonster("Pyrewatch", "Coal Creeper", "Coal", 6);

            Core.EnsureComplete(4076);
        }

        //Ease the Ill 4077
        if (!Story.QuestProgression(4077))
        {
            Core.EnsureAccept(4077);
            Core.HuntMonster("Pyrewatch", "Lavazard", "Wickskin Root", 3);
            Core.HuntMonster("Pyrewatch", "Lavazard", "Zard Marrow", 3);
            Core.HuntMonster("Pyrewatch", "Living Lava", "Living Lava Blood", 2);
            Story.MapItemQuest(4077, "Pyrewatch", 3161, 5);
        }

        //Defend Pyrewatch Peak 4078
        Story.KillQuest(4078, "Pyrewatch", "Storm Scout");

        //Signal Fire 4079
        if (!Story.QuestProgression(4079))
        {
            Core.EnsureAccept(4079);
            Core.HuntMonster("Pyrewatch", "Storm Scout", "Polish");
            Core.HuntMonster("Pyrewatch", "Flame Soldier", "Cloth");
            Core.HuntMonster("Pyrewatch", "Flame Soldier", "Stand Legs", 8);
            Core.HuntMonster("Pyrewatch", "Fyreborn Tiger", "Reflectors", 4);
            Core.EnsureComplete(4079);
        }

        //Spreading Like Wildfire 4080
        Story.MapItemQuest(4080, "Pyrewatch", 3162, 4);

        //Pyrewatch Defender Badge 4081
        Story.KillQuest(4081, "Pyrewatch", "Storm Scout");
    }

    public void Feverfew()
    {
        if (Core.isCompletedBefore(4142))
            return;

        Pyrewatch();

        Story.PreLoad(this);

        //map aggro from pyrewatch
        while (!Bot.ShouldExit && Bot.Player.InCombat)
        {
            Core.JumpWait();
            Core.Sleep();
            if (!Bot.Player.InCombat)
                break;
        }

        //Quench the Flames
        Story.KillQuest(4128, "feverfew", "Firestorm Knight");

        //Through the Fog and Flame
        Story.KillQuest(4129, "feverfew", "Locked Chest");

        //Restore the Lady of Waters
        if (!Story.QuestProgression(4130))
        {
            Story.MapItemQuest(4130, "feverfew", new[] { 3246, 3247 });
            Core.HuntMonster("feverfew", "Coral Creeper", "Statue Torso");
            Core.HuntMonster("feverfew", "Twisted Undine", "Statue Base");
            Core.HuntMonster("feverfew", "Salamander", "Statue Core");
            Core.HuntMonster("feverfew", "Firestorm Knight", "Statue Plaque");
            Core.EnsureComplete(4130);
        }

        //Rumors and Smoke
        if (!Story.QuestProgression(4131))
        {
            Core.EnsureAccept(4131);
            Core.GetMapItem(3245, 1, "feverfew");
            Core.HuntMonster("feverfew", "Firestorm Knight", "Intent Rumor", 3);
            Core.HuntMonster("feverfew", "Firestorm Major", "Location Rumor", 2);
            Core.HuntMonster("feverfew", "Firestorm Knight", "Fate Rumor", 2);
            Core.HuntMonster("feverfew", "Firestorm Major", "Identity Rumor", 1);
            Core.EnsureComplete(4131);
        }

        //Dam the Flood
        Story.MapItemQuest(4132, "feverfew", 3244, 5);

        //Salvage Mission
        if (!Story.QuestProgression(4133))
        {
            Core.EnsureAccept(4133);
            Core.GetMapItem(3243, 5, "feverfew");
            Core.HuntMonster("feverfew", "Twisted Undine", "Locatrix Animus Found", 1);
            Core.EnsureComplete(4133);
        }

        //Fear the Fog
        if (!Story.QuestProgression(4134))
        {
            Core.EnsureAccept(4134);

            Core.HuntMonster("feverfew", "Salamander", "Salamander Tongue", 3);
            Core.HuntMonster("feverfew", "Feverfew Vase", "Adderoot Powder", 2);
            Core.HuntMonster("feverfew", "Twisted Undine", "Shadowbane Brine", 4);
            Core.HuntMonster("feverfew", "Coral Creeper", "Charred Claw", 2);
            Core.HuntMonster("feverfew", "Firestorm Knight", "Whispered Regret");

            Core.EnsureComplete(4134);
        }

        //When There's Smoke...
        Story.MapItemQuest(4135, "feverfew", 3248);

        //Firin' This Guy
        Story.KillQuest(4136, "feverfew", "Blazebinder");

        //Blessings of the Lady
        Story.MapItemQuest(4137, "feverfew", 3242, 10);

        //Parting the Waters
        if (!Story.QuestProgression(4138))
        {
            Core.Logger("Quest needs to be abandoned before it can be completed and reaccepted.");
            Core.EnsureAccept(4138);
            Core.AbandonQuest(4138);

            Core.EnsureAccept(4138);

            Core.HuntMonster("feverfew", "Salamander", "Burning Ember", 3);
            Core.HuntMonster("feverfew", "Coral Creeper", "Stoneskin Shard", 3);
            Core.HuntMonster("feverfew", "Firestorm Knight", "Last Breath", 3);
            Core.HuntMonster("feverfew", "Twisted Undine", "Undine's Tear", 3);

            Core.Jump("Cut2", "Left");
            Core.EnsureComplete(4138);
        }

        //The Power to Heal
        if (!Story.QuestProgression(4139))
        {
            Core.EnsureAccept(4139);
            Core.HuntMonster("feverfew", "Feverfew Vase", "Saltwater Powder", 3);
            Core.HuntMonster("feverfew", "Twisted Undine", "Undine Effluvia", 2);
            Core.HuntMonster("feverfew", "Locked Chest", "Herbalist Manual", 1);
            Core.EnsureComplete(4139);
        }

        //The Deadsea Caverns
        if (!Story.QuestProgression(4140))
        {
            Core.EnsureAccept(4140);
            Core.HuntMonster("feverfew", "Coral Creeper", "Coral Creeper Slain", 3);
            Core.HuntMonster("feverfew", "Twisted Undine", "Twisted Undine Slain", 3);
            Core.HuntMonster("feverfew", "Salamander", "Salamander Slain", 3);
            Core.EnsureComplete(4140);
        }

        //Open the Floodgates
        Story.MapItemQuest(4141, "feverfew", 3241);

        //Tiger, Tiger Burning Bright
        Story.KillQuest(4142, "feverfew", "Major Thermas");
    }

    public void Phoenixrise()
    {
        if (Core.isCompletedBefore(4213))
            return;

        Feverfew();

        Story.PreLoad(this);

        //Stonecold Defense
        if (!Story.QuestProgression(4201))
        {
            Core.EnsureAccept(4201);
            Core.HuntMonster("phoenixrise", "Lava Troll", "Lava Tonic", 5);
            Core.HuntMonster("phoenixrise", "Infernal Goblin ", "Leather Wing", 3);
            Core.EnsureComplete(4201);
        }

        //Preying for a Good Offense
        if (!Story.QuestProgression(4202))
        {
            Core.EnsureAccept(4202);
            Core.HuntMonster("phoenixrise", "Infernal Goblin ", "Infernal Goblin Slain", 3);
            Core.HuntMonster("phoenixrise", "Lava Troll", "Lava Troll Slain", 3);
            Core.HuntMonster("phoenixrise", "Gargrowl", "Gargrowl Slain", 3);
            Core.EnsureComplete(4202);
        }

        //Red Alert
        Story.MapItemQuest(4203, "phoenixrise", 3283, 4);

        //Disguise Fur a Good Cause
        if (!Story.QuestProgression(4204))
        {
            Core.EnsureAccept(4204);
            Core.HuntMonster("phoenixrise", "Lava Troll", "MeltRite Glue", 1);
            Core.HuntMonster("phoenixrise", "Firestorm Tiger", "Whole Tigerskin", 2);
            Core.HuntMonster("phoenixrise", "Infernal Goblin ", "Binding Leather", 3);
            Core.EnsureComplete(4204);
        }

        //Hunt for the Stolen
        Story.MapItemQuest(4205, "phoenixrise", 3285);

        //Rescue Run
        Story.MapItemQuest(4206, "phoenixrise", 3282, 6);

        //Recover the Remainder
        Story.KillQuest(4207, "phoenixrise", "Pyrric Ursus");

        //Rune Chances of a Backstab
        if (!Story.QuestProgression(4208))
        {
            Core.EnsureAccept(4208);
            Core.GetMapItem(3284, 7, "phoenixrise");
            Core.HuntMonster("phoenixrise", "Infernal Goblin ", "Infernal Goblin Slain", 6);
            Core.EnsureComplete(4208);
        }

        //Clear out the Caverns
        if (!Story.QuestProgression(4209))
        {
            Core.EnsureAccept(4209);
            Core.HuntMonster("phoenixrise", "Lava Troll", "Lava Troll Disarmed", 3);
            Core.HuntMonster("phoenixrise", "Infernal Goblin", "Goblin Claws", 5);
            Core.HuntMonster("phoenixrise", "Firestorm Tiger", "Tiger Fang", 4);
            Core.HuntMonster("phoenixrise", "Pyrric Ursus", "Ursus Shard", 2);
            Core.EnsureComplete(4209);
        }

        //Strengthen the Survivors
        if (!Story.QuestProgression(4210))
        {
            Core.EnsureAccept(4210);
            Core.HuntMonster("phoenixrise", "Lava Troll", "Sacred Flame");
            Core.HuntMonster("phoenixrise", "Infernal Goblin", "Lava Rune", 3);
            Core.HuntMonster("phoenixrise", "Firestorm Tiger", "Tiger Claw", 4);
            Core.HuntMonster("phoenixrise", "Pyrric Ursus", "Pyrric Gem");
            Core.EnsureComplete(4210);
        }

        //Bridge to Salvation
        Story.KillQuest(4211, "phoenixrise", "Lava Troll");

        //Growling Pains
        Story.KillQuest(4212, "phoenixrise", "Gargrowl");

        //Defeat Cinderclaw
        Story.KillQuest(4213, "phoenixrise", "Cinderclaw");
    }

    public void Fireforge()
    {
        if (Core.isCompletedBefore(4226))
            return;

        Phoenixrise();

        Story.PreLoad(this);

        //Round 1: Firestorm Scouts 4216
        Story.KillQuest(4216, "Fireforge", "Firestorm Scout");

        //Round 2: Firestorm Soldiers 4217
        Story.KillQuest(4217, "Fireforge", "Firestorm Soldier");

        //Round 3: Firestorm Pikemen 4218
        Story.KillQuest(4218, "Fireforge", "Fire Pikeman");

        //Round 4: Firestorm Knights 4219
        Story.KillQuest(4219, "Fireforge", "Firestorm Knight");

        //Round 5: Firestorm Corporals 4220
        Story.KillQuest(4220, "Fireforge", "Firestorm Corporal");

        //Round 6: Firestorm Tigers 4221
        Story.KillQuest(4221, "Fireforge", "Firestorm Tiger");

        //Round 7: Firestorm Cavalry 4222
        Story.KillQuest(4222, "Fireforge", "Tiger Cavalry");

        //Round 8: Firestorm Majors 4223
        Story.KillQuest(4223, "Fireforge", "Firestorm Major");

        //Round 9: Firestorm Blazebinders 4224
        Story.KillQuest(4224, "Fireforge", "Blazebinder");

        //Round 10: Firestorm General 4225
        Story.KillQuest(4225, "Fireforge", "Firestorm General");

        //Bonus Round: Flamewing the Bright 4226
        Story.KillQuest(4226, "Fireforge", "Flamewing");

        //Final Round: Tyndarius Tigermaster 4230
        if (!Story.QuestProgression(4230))
        {
            Core.EnsureAccept(4230);
            Core.HuntMonsterMapID("Fireforge", 24, "Tyndarius Defeated?");
            Core.EnsureComplete(4230);
        }
    }

    public void Lavarun()
    {
        if (Core.isCompletedBefore(4235))
            return;

        Fireforge();

        Story.PreLoad(this);

        //Defeat Phedra 4231
        Story.KillQuest(4231, "Lavarun", "Phedra");

        //Defeat Mega Tyndarius 4232
        Story.KillQuest(4232, "Lavarun", "Mega Tyndarius");

        //The Onslaught Fights On 4235
        Story.KillQuest(4235, "Lavarun", "Firestorm Soldier");
    }

    public void Brimstone()
    {
        if (Core.isCompletedBefore(4115) || !Core.IsMember)
        {
            Core.Logger(
                !Core.IsMember
                    ? "You need to be a member to complete /brimestone questline."
                    : "Brimstone Story Complete"
            );
            return;
        }

        // The Hard Way 4107
        if (!Story.QuestProgression(4107))
        {
            Core.EnsureAccept(4107);
            Core.HuntMonster("Brimstone", "Brimstone Marauder", "Pieces of Gossip", 3);
            Core.HuntMonster("Brimstone", "Brimstone Looter", "Bits of Hearsay", 3);
            Core.HuntMonster("Brimstone", "Brimstone Bandit", "Overheard Conversations", 3);
            Core.EnsureComplete(4107);
        }

        //Encrypt Keepers 4108
        if (!Story.QuestProgression(4108))
        {
            Core.EnsureAccept(4108);
            Core.HuntMonster("Brimstone", "Brimstone Marauder", "Cryptic Key", 4);
            Core.HuntMonster("Brimstone", "Brimstone Looter", "Partial Cipher", 2);
            Core.HuntMonster("Brimstone", "Brimstone Bandit", "Reliable(?) Translation", 2);
            Core.EnsureComplete(4108);
        }

        //Catching Couriers  4109
        if (!Story.QuestProgression(4109))
        {
            Core.EnsureAccept(4109);
            Core.HuntMonster("Brimstone", "Brimstone Marauder", "Burned Letter");
            Core.HuntMonster("Brimstone", "Brimstone Looter", "Smudged Letter");
            Core.HuntMonster("Brimstone", "Brimstone Bandit", "Barely Legible Letter");
            Core.EnsureComplete(4109);
        }

        //Steppe Quickly 4110
        if (!Story.QuestProgression(4110))
        {
            Core.EnsureAccept(4110);
            Core.HuntMonster("Brimstone", "Brimstone Marauder", "Marauders slain", 3);
            Core.HuntMonster("Brimstone", "Fyreborn Tiger", "Fyreborn Tigers slain", 5);
            Core.HuntMonster("Brimstone", "Fyresyn", "Fyresyn slain", 5);
            Core.HuntMonster("Brimstone", "Brimstone Looter", "Looters slain", 3);
            Core.HuntMonster("Brimstone", "Brimstone Bandit", "Bandits slain", 3);
            Core.EnsureComplete(4110);
        }

        //Reclamation 4111
        if (!Story.QuestProgression(4111))
        {
            Core.EnsureAccept(4111);
            Core.HuntMonster("Brimstone", "Brimstone Bandit", "Bags of Mercantile Goods", 3);
            Core.HuntMonster("Brimstone", "Brimstone Bandit", "Boxes of Raw Materials", 3);
            Core.EnsureComplete(4111);
        }

        //Bad Press 4112
        if (!Story.QuestProgression(4112))
        {
            Core.EnsureAccept(4112);
            Core.HuntMonster("Brimstone", "Brimstone Bandit", "Talmin's Propoganda", 6);
            Core.HuntMonster("Brimstone", "Brimstone Bandit", "Talmin's Doctrine", 3);
            Core.EnsureComplete(4112);
        }

        //Good Hunting 4113
        if (!Story.QuestProgression(4113))
        {
            Core.EnsureAccept(4113);
            Core.HuntMonster("Brimstone", "Fyresyn", "Fyresyns Put Down", 5);
            Core.HuntMonster("Brimstone", "Fyreborn Tiger", "Fyreborn Tigers Put Down", 6);
            Core.HuntMonster("Brimstone", "Pyradon", "Pyradons Put Down", 5);
            Core.EnsureComplete(4113);
        }

        //One Last Push 4114
        if (!Story.QuestProgression(4114))
        {
            Core.EnsureAccept(4114);
            Core.HuntMonster("Brimstone", "Brimstone Bandit", "Bandit's Ear", 3);
            Core.HuntMonster("Brimstone", "Brimstone Marauder", "Marauder's Finger", 3);
            Core.HuntMonster("Brimstone", "Brimstone Looter", "Looter's Tooth", 3);
            Core.EnsureComplete(4114);
        }

        //Redemption 4115
        Story.KillQuest(4115, "Brimstone", "Chief Talmin");
    }

    public void Nightmare()
    {
        if (Core.isCompletedBefore(4157))
            return;

        if (!Core.IsMember)
        {
            Core.Logger("You need to be a member for complete the /nightmare questline.");
            return;
        }

        Story.PreLoad(this);

        //Fear of Clowns? 4143
        Story.KillQuest(4143, "Nightmare", "Bobble Clown");

        //FEAR OF CLOWNS!!! 4144
        Story.KillQuest(4144, "Nightmare", "Crazy Clown");

        //Fear of Spiders 4145
        Story.KillQuest(4145, "Nightmare", "Castle Spider");

        //Fear of Snakes? 4146
        if (!Story.QuestProgression(4146))
        {
            Core.EnsureAccept(4146);
            Core.HuntMonster("Nightmare", "Wrasp", "Grappling Hook");
            Core.HuntMonster("Nightmare", "Wrasp", "Rope");
            Core.EnsureComplete(4146);
        }

        //Fear of Falling? 4147
        Story.MapItemQuest(4147, "Nightmare", 3262);

        //Fear of Germs? 4148
        Story.KillQuest(4148, "Nightmare", "Germs");

        //Fear of Needles? 4149
        Story.KillQuest(4149, "Nightmare", "Needle");

        //Fear of Dolls? 4150
        Story.KillQuest(4150, "Nightmare", "Broken Toy");

        //Fear of Being Buried Alive? 4151
        if (!Story.QuestProgression(4151))
        {
            Core.EnsureAccept(4151);
            Core.HuntMonster("Nightmare", "Unearthed Skeleton", "Skeleton Deboned", 8);
            Core.HuntMonster("Nightmare", "Rotfeeder Worm", "Rotfeeder squished", 4);
            Core.EnsureComplete(4151);
        }

        //Fear of Burning? 4152
        Story.KillQuest(4152, "Nightmare", "Fire Imp");

        //FEAR OF BURNING!!! 4153
        Story.KillQuest(4153, "Nightmare", "Flame Elemental");

        //Fear of Drowning? 4154
        if (!Story.QuestProgression(4154))
        {
            Core.EnsureAccept(4154);
            Core.HuntMonster("Nightmare", "Anglerfish", "Anglerfish Defeated", 6);
            Core.HuntMonster("Nightmare", "Merdraconian", "Merdraconian Defeated", 6);
            Core.HuntMonster("Nightmare", "Deep Dweller", "Deep Dweller defeated");
            Core.EnsureComplete(4154);
        }

        //Fear of Inadequacy? 4155
        Story.KillQuest(4155, "Nightmare", "Unearthed Skeleton");

        //Fear of Loneliness? 4156
        Story.KillQuest(4156, "Nightmare", "Nothing");

        //Fear of Failure? 4157
        Story.KillQuest(4157, "Nightmare", "Devourax");
    }
}
