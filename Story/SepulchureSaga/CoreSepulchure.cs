/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;

public class CoreSepulchure
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

    public void CompleteSS()
    {
        Alden();
        Lynaria();
        SepulchuresRise();
        ShadowfallRise();
    }

    #region Alden Quests
    public void Alden()
    {
        if (Core.isCompletedBefore(6342))
            return;

        Story.PreLoad(this);

        // Who goes there? 6332
        Core.EquipClass(ClassType.Farm);
        Story.MapItemQuest(6332, "scarsgarde", 5860);

        // The Taint Spreads 6333
        if (!Story.QuestProgression(6333))
        {
            Core.EnsureAccept(6333);
            Core.GetMapItem(5864, 6, "scarsgarde");
            Core.HuntMonster("scarsgarde", "VenomWing", "VenomWing Slain", 6);
            Core.EnsureComplete(6333);
        }

        // Beauty Twisted 6334
        Story.KillQuest(6334, "scarsgarde", "Garde Grif");

        // Element of Surprise 6335
        if (!Story.QuestProgression(6335))
        {
            Core.EnsureAccept(6335);
            Core.GetMapItem(5865, 5, "scarsgarde");
            Core.HuntMonster("scarsgarde", "Tree", "Tree Branch Obtained", 10);
            Core.EnsureComplete(6335);
        }

        // (Take the) Watch Out 6336
        if (!Story.QuestProgression(6336))
        {
            Core.EnsureAccept(6336);
            Core.HuntMonster("scarsgarde", "Garde Watch", "Watch Slain", 5);
            Core.HuntMonster("scarsgarde", "Garde Pikeman", "Pike Slain", 5);
            Core.EnsureComplete(6336);
        }

        // False Hoods 6337
        if (!Story.QuestProgression(6337))
        {
            Quest? dothis = Core.InitializeWithRetries(() => Bot.Quests.EnsureLoad(6337));
            if (dothis != null)
            {
                Core.EnsureAccept(dothis.ID);
                foreach (ItemBase Item in dothis.Requirements)
                    Core.HuntMonster("scarsgarde", "Garde Watch", Item.Name, Item.Quantity, Item.Temp);
                Core.EnsureComplete(dothis.ID);
            }
            else
            {
                Core.Logger("Quest 6337 not found, try restarting the bot.");
                return;
            }
        }

        // Pass for Real 6338
        if (!Story.QuestProgression(6338))
        {
            Core.EnsureAccept(6338);
            //this cell is required for `Passcode A`, but everything can be goten here.. unlike elsewhere
            Core.KillMonster("scarsgarde", "r35", "Left", "Garde Knight", "Passcode A");
            Core.KillMonster("scarsgarde", "r35", "Left", "Garde Knight", "Passcode C");
            Core.KillMonster("scarsgarde", "r35", "Left", "Garde Pikeman", "Passcode B");

            Story.MapItemQuest(6338, "scarsgarde", new[] { 5866, 5867 });
        }

        // Hidden in Plain Sight 6339
        Story.MapItemQuest(6339, "scarsgarde", 5868, 8);
        Story.MapItemQuest(6339, "scarsgarde", 5869);

        // Stay Strong Keep Steady 6340
        if (!Story.QuestProgression(6340))
        {
            Core.EnsureAccept(6340);
            Core.HuntMonster("scarsgarde", "Garde Knight", "Knight Slain", 10);
            Core.HuntMonster("scarsgarde", "Garde Pikeman", "Pike Slain", 5);
            Core.EnsureComplete(6340);
        }

        // The Final Fight 6341
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(6341, "scarsgarde", "Garde Captain");

        // Arm the Army 6342
        if (!Story.QuestProgression(6342))
        {
            Core.EnsureAccept(6342);
            Core.HuntMonster("scarsgarde", "Garde Knight", "Knight Slain", 3);
            Core.HuntMonster("scarsgarde", "Garde Watch", "Watch Slain", 3);
            Core.HuntMonster("scarsgarde", "Garde Pikeman", "Pike Slain", 3);
            Core.EnsureComplete(6342);
        }
    }
    #endregion

    #region Lynaria Quests
    public void Lynaria()
    {
        if (Core.isCompletedBefore(6353))
            return;

        Story.PreLoad(this);

        // Who goes there? 6343
        Core.EquipClass(ClassType.Farm);
        Story.MapItemQuest(6343, "scarsgarde", 5861);

        // The Taint Spreads 6344
        if (!Story.QuestProgression(6344))
        {
            Core.EnsureAccept(6344);
            Core.GetMapItem(5864, 6, "scarsgarde");
            Core.HuntMonster("scarsgarde", "VenomWing", "VenomWing Slain", 6);
            Core.EnsureComplete(6344);
        }

        // Beauty Twisted 6345
        Story.KillQuest(6345, "scarsgarde", "Garde Grif");

        // Element of Surprise 6346
        if (!Story.QuestProgression(6346))
        {
            Core.EnsureAccept(6346);
            Core.GetMapItem(5865, 5, "scarsgarde");
            Core.HuntMonster("scarsgarde", "Tree", "Tree Branch Obtained", 10);
            Core.EnsureComplete(6346);
        }

        // (Take the) Watch Out 6347
        if (!Story.QuestProgression(6347))
        {
            Core.EnsureAccept(6347);
            Core.HuntMonster("scarsgarde", "Garde Watch", "Watch Slain", 5);
            Core.HuntMonster("scarsgarde", "Garde Pikeman", "Pike Slain", 5);
            Core.EnsureComplete(6347);
        }

        // False Hoods 6348
        if (!Story.QuestProgression(6348))
        {
            Core.EnsureAccept(6348);
            Core.HuntMonster("scarsgarde", "Garde Watch", "Watch Hood", 1);
            Core.HuntMonster("scarsgarde", "Garde Watch", "Watch Scarf", 1);
            Core.HuntMonster("scarsgarde", "Garde Watch", "Watch Tunic", 1);
            Core.HuntMonster("scarsgarde", "Garde Watch", "Watch Leggings", 1);
            Core.HuntMonster("scarsgarde", "Garde Watch", "Watch Boots", 1);
            Core.HuntMonster("scarsgarde", "Garde Watch", "Watch Dagger", 1);
            Core.HuntMonster("scarsgarde", "Garde Watch", "Watch Badge", 1);
            Core.EnsureComplete(6348);
        }

        // Pass for Real 6349
        if (!Story.QuestProgression(6349))
        {
            Core.EnsureAccept(6349);
            //this cell is required for `Passcode A`, but everything can be goten here.. unlike elsewhere
            Core.KillMonster("scarsgarde", "r35", "Left", "Garde Knight", "Passcode A");
            Core.KillMonster("scarsgarde", "r35", "Left", "Garde Knight", "Passcode C");
            Core.KillMonster("scarsgarde", "r35", "Left", "Garde Pikeman", "Passcode B");

            Story.MapItemQuest(6349, "scarsgarde", new[] { 5866, 5867 });
        }

        // Hidden in Plain Sight 6350
        Story.MapItemQuest(6350, "scarsgarde", 5868, 8);
        Story.MapItemQuest(6350, "scarsgarde", 5869);

        // Stay Strong Keep Steady 6351
        if (!Story.QuestProgression(6351))
        {
            Core.EnsureAccept(6351);
            Core.HuntMonster("scarsgarde", "Garde Knight", "Knight Slain", 10);
            Core.HuntMonster("scarsgarde", "Garde Pikeman", "Pike Slain", 5);
            Core.EnsureComplete(6351);
        }

        // The Final Fight 6352
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(6352, "scarsgarde", "Garde Captain");

        // Arm the Army 6353
        if (!Story.QuestProgression(6353))
        {
            Core.EnsureAccept(6353);
            Core.HuntMonster("scarsgarde", "Garde Knight", "Knight Slain", 3);
            Core.HuntMonster("scarsgarde", "Garde Watch", "Watch Slain", 3);
            Core.HuntMonster("scarsgarde", "Garde Pikeman", "Pike Slain", 3);
            Core.EnsureComplete(6353);
        }
    }
    #endregion

    #region Sepulchure's Rise
    public void SepulchuresRise()
    {
        if (Core.isCompletedBefore(6408))
            return;

        Story.PreLoad(this);

        // Come On Get Ready! 6356
        Core.EquipClass(ClassType.Farm);
        Story.BuyQuest(6356, "valleyofdoom", 1599, "Valen Gear", 1);

        // Scout the Valley 6357
        Story.KillQuest(6357, "valleyofdoom", "Shadow Imp");

        // Gather Energy 6358
        Story.KillQuest(6358, "valleyofdoom", "Shadow Imp");

        // Do I Have To Spell It Out? 6359
        Story.MapItemQuest(6359, "valleyofdoom", 5873, 8);

        // Get Through the Shadows 6360
        if (!Story.QuestProgression(6360))
        {
            Core.EnsureAccept(6360);
            Core.HuntMonster("valleyofdoom", "Shadow Beast", "Shadow Beasts Slain", 8);
            Core.HuntMonster("valleyofdoom", "Shadow Person", "Shadow People Slain", 8);
            Core.EnsureComplete(6360);
        }

        // Get the Key 6361
        Story.KillQuest(6361, "valleyofdoom", "Doom Guardian");

        // Confront the Champion of Darkness 6362
        Story.MapItemQuest(6362, "valleyofdoom", 5872);

        // Destroy the Arsenal 6363
        if (!Story.QuestProgression(6363))
        {
            Core.EnsureAccept(6363);
            Core.KillMonster("valleyofdoom", "r7", "Left", "Doom Star", "Doomstar Destroyed");
            Core.KillMonster("valleyofdoom", "r7", "Left", "Doom Scythe", "Doomscythe Destroyed");
            Core.KillMonster("valleyofdoom", "r7", "Left", "Doom Axe", "Doomaxe Destroyed");
            Core.KillMonster("valleyofdoom", "r8", "Left", "Doom Blade", "Doom Blade Destroyed");
            Core.KillMonster("valleyofdoom", "r8", "Left", "Doom Knight Armor", "Doom Knight Armor Destroyed");
            Core.EnsureComplete(6363);
        }

        // Defeat the Armor 6364
        Core.EquipClass(ClassType.Solo);
        if (!Story.QuestProgression(6364))
        {
            Core.EnsureAccept(6364);
            Core.HuntMonsterMapID("valleyofdoom", 25, "Doomknight Armor Defeated");
            Core.EnsureComplete(6364);
        }

        // Secure the Chest 6365
        Story.MapItemQuest(6365, "guardiantower", 5871);

        // Get Some Goo 6366
        Story.KillQuest(6366, "guardiantower", "Slime");

        // Retrieve the Wand 6367
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(6367, "guardiantower", "Yargol Magebane");

        // The Wedding 6368
        Core.Logger("Autocompleted");

        // Gather the Shadows 6369
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(6369, "valleyofdoom", "Shadow Imp");

        // Choose Your Path 6370
        Story.MapItemQuest(6370, "ebonslate", 5895);

        // Mind Control 6371
        Story.KillQuest(6371, "ebonslate", "Mind Con-Troll");

        // Sp-Eye on You 6372
        Story.KillQuest(6372, "ebonslate", "Sp-Eye");

        // Lower the Drawbridge 6373
        Story.MapItemQuest(6373, "ebonslate", 5896);
        Story.KillQuest(6373, "ebonslate", "Ebonslate Guard");

        // Get the Key 6374
        Story.MapItemQuest(6374, "ebonslate", 5897);
        Story.KillQuest(6374, "ebonslate", new[] { "Lycan Brute", "Lycan Brute" });

        // Break Down the Door 6375
        Story.MapItemQuest(6375, "ebonslate", 5898);

        // Question the Knights 6376
        Story.KillQuest(6376, "ebonslate", "Ebonslate Knight");

        // Troll the Trolls 6377
        Story.KillQuest(6377, "ebonslate", "Mind Con-Troll");

        // Get Through the Barrier 6378
        Story.MapItemQuest(6378, "ebonslate", 5899);

        // Disarm the Knights 6379
        Story.KillQuest(6379, "ebonslate", "Ebonslate Knight");

        // Bruise the Bruiser 6380
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(6380, "ebonslate", "Ebonslate Bruiser");

        // Take Down Dethrix 6381
        if (Story.QuestProgression(6381))
        {
            Core.EnsureAccept(6381);
            Core.Join("ebonslate", "r11", "Left");
            Bot.Kill.Monster("Dethrix");
            Core.Sleep(10000);
        }

        // Please Fix Me 6382
        Story.KillQuest(6382, "guardiantowerb", "Slime");

        // Get the Slime 6383
        Story.KillQuest(6383, "guardiantowerb", "Slime");

        // Reach for the Heart 6384
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(6384, "guardiantowerb", "Yargol Magebane");

        // Get More Shadows 6385
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(6385, "guardiantowerb", "Slime");

        // Garen's Key 6386
        if (!Story.QuestProgression(6386))
        {
            Core.EnsureAccept(6386);
            Core.HuntMonster("guardiantowerb", "Guardian Selby", "Guardians Slain", 3);
            Core.HuntMonster("guardiantowerb", "Guardian Garen", "Guardian Garen's Key", 1);
            Core.EnsureComplete(6386);
        }

        // Selby's Key 6387
        if (!Story.QuestProgression(6387))
        {
            Core.EnsureAccept(6387);
            Core.HuntMonster("guardiantowerb", "Guardian Garen", "Guardians Slain", 3);
            Core.HuntMonster("guardiantowerb", "Guardian Selby", "Guardian Selby's Key", 1);
            Core.EnsureComplete(6387);
        }

        // Get Rid of Bolton 6388
        if (!Story.QuestProgression(6388))
        {
            Core.EnsureAccept(6388);
            Core.GetMapItem(5901, 1, "guardiantowerb");
            Core.HuntMonster("guardiantowerb", "Guardian Bolton", "Guardian Bolton Slain", 1);
            Core.EnsureComplete(6388);
        }

        // Get Rid of the Imps 6389
        Story.KillQuest(6389, "ebondungeon", "Ebonslate Imp");

        // Get Into the Dungeon 6390
        Story.MapItemQuest(6390, "ebondungeon", 5902);

        // Destroy the Guards 6391
        Story.KillQuest(6391, "ebondungeon", "Ebon Dungeon Guard");

        // Get the Key 6392
        Story.KillQuest(6392, "ebondungeon", "Elite Dungeon Guard");

        // Find Lynaria 6393
        Story.MapItemQuest(6393, "ebondungeon", new[] { 5903, 5904 });

        // Destroy Dethrix 6394
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(6394, "ebondungeon", "Dethrix");

        // Siphon the Power (Farming) 6395
        Story.KillQuest(6395, "ebondungeon", "Ebon Dungeon Guard");

        // He's Not Your Friend 6399
        Story.KillQuest(6399, "alteonfight", "King Alteon");

        // Defeat the Dragon 6400
        Story.MapItemQuest(6400, "alteonfight", 5910);

        // Power Boost 6401
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(6401, "darkplane", "Shadow Beast");

        // Take His Knights 6402
        Story.KillQuest(6402, "darkplane", "Guardian Knight");

        // Destroy the Light 6403
        Story.KillQuest(6403, "darkplane", "Light Spirit");

        // More Power! 6404
        Story.KillQuest(6404, "darkplane", "Shadow Beast");

        // Destroy the Force Field 6405
        Story.MapItemQuest(6405, "darkplane", 5911);

        // Defeat the Battalion 6406
        Story.KillQuest(6406, "darkplane", "Guardian Knight");

        // Destroy the Beast 6407
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(6407, "darkplane", "Victorious");

        // Learn from the Past 6408
        Story.KillQuest(6408, "darkplane", "Shadow Beast");
    }
    #endregion

    #region Shadowfall Rise
    public void ShadowfallRise()
    {
        if (Core.isCompletedBefore(6592))
            return;

        Story.PreLoad(this);

        // Fluid Dynamics 6539
        Core.EquipClass(ClassType.Farm);
        Story.KillQuest(6539, "noxustower", "Slimeskull");

        // M-ossification 6540
        Story.KillQuest(6540, "noxustower", "Doomwood Treeant");

        // A-gore-able 6541
        Story.KillQuest(6541, "noxustower", "Sanguine Souleater");

        // Raw-r 6542
        Story.KillQuest(6542, "noxustower", "Lightguard Caster");

        // Light 'em Up 6543
        Story.KillQuest(6543, "noxustower", "Lightguard Caster");

        // Spellbound 6544
        if (!Story.QuestProgression(6544))
        {
            Core.EnsureAccept(6544);
            Core.GetMapItem(6018, 4, "noxustower");
            Core.HuntMonster("noxustower", "Lightguard Caster", "Lightguard Scroll", 4);
            Core.EnsureComplete(6544);
        }

        // Lupine Resurrection 6545
        Story.KillQuest(6545, "noxustower", "Lightguard Wolf");

        // Spying on the Light 6546
        Story.KillQuest(6546, "noxustower", "Lightguard Paladin");

        // Illusory Disguise 6547
        if (!Story.QuestProgression(6547))
        {
            Core.EnsureAccept(6547);
            Core.HuntMonster("noxustower", "Slimeskull", "Necrotic Ooze", 3);
            Core.HuntMonster("noxustower", "Lightguard Caster", "Lightguard Armor", 4);
            Core.HuntMonster("noxustower", "Doomwood Treeant", "Arcane Energy", 3);
            Core.EnsureComplete(6547);
        }

        // Test the Disguise 6548
        Story.MapItemQuest(6548, "noxustower", 6019);

        // Get Alteon 6549
        Story.MapItemQuest(6549, "noxustower", 6020);

        // Hammer Time 6550
        Story.KillQuest(6550, "noxustower", "General Goldhammer");

        // Lightguard Medals 6560
        Story.KillQuest(6560, "lightguardwar", "Citadel Crusader");

        // Mega Lightguard Medals 6561
        Story.KillQuest(6561, "lightguardwar", "Citadel Crusader");

        // Bone Marrow 6562
        Story.KillQuest(6562, "lightguardwar", "Citadel Crusader");

        // Ooze it up 6563
        Story.KillQuest(6563, "lightguardwar", "Slimeskull");

        // Seize the Seige (Engine) 6564
        Story.KillQuest(6564, "lightguardwar", "Lightguard Engine");

        // Darken the Light 6565
        Story.KillQuest(6565, "lightguardwar", "Scorching Flame");

        // Get the Powder 6566
        Story.KillQuest(6566, "lightguardwar", "Citadel Crusader");

        // Destroy the Shield 6567
        Story.KillQuest(6567, "lightguardwar", "Sigrid Sunshield");

        // Such a Mess 6581
        if (!Story.QuestProgression(6581))
        {
            Core.EnsureAccept(6581);
            Core.GetMapItem(6098, 8, "lumafortress");
            Core.HuntMonster("lumafortress", "Invasive Shadow", "Shadows Defeated", 8);
            Core.EnsureComplete(6581);
        }

        // Herbs Needed 6582
        if (!Story.QuestProgression(6582))
        {
            Core.EnsureAccept(6582);
            Core.GetMapItem(6099, 7, "lumafortress");
            Core.HuntMonster("lumafortress", "Light Treeant", "Glowmoss", 6);
            Core.EnsureComplete(6582);
        }

        // Shine a Light 6583
        Story.KillQuest(6583, "lumafortress", "Light Elemental");

        // Hapless? 6584
        Story.KillQuest(6584, "lumafortress", "Hapless Skeleton");

        // How 'bout them Apples 6585
        Story.KillQuest(6585, "lumafortress", "Light Treeant");

        // Make an Offering 6586
        if (!Story.QuestProgression(6586))
        {
            Core.EnsureAccept(6586);
            Core.GetMapItem(6100, 1, "lumafortress");
            Core.HuntMonster("lumafortress", "Lightwing", "Brilliant Feather", 1);
            Core.EnsureComplete(6586);
        }

        // Test the Spell Again! 6587
        Story.MapItemQuest(6587, "lumafortress", 6101);

        // Gather the Light 6588
        Story.KillQuest(6588, "lumafortress", "Light Elemental");

        // Banish the Shadows 6589
        Story.KillQuest(6589, "lumafortress", "Living Shadow");

        // So Many Minions 6590
        Story.KillQuest(6590, "lumafortress", "Skeleton Minion");

        // Corrupted Light 6591
        if (!Story.QuestProgression(6591))
        {
            Core.EnsureAccept(6591);
            Core.EquipClass(ClassType.Solo);
            Core.KillMonster("lumafortress", "r4", "Left", "Corrupted Luma", "Luma Defeated");
            Core.EnsureComplete(6591);
        }

        // Star Light Star Bright 6592
        Story.KillQuest(6592, "lumafortress", "Light Elemental");
    }
    #endregion
}
