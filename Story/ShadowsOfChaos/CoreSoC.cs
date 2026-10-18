/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
using Skua.Core.Interfaces;

public class CoreSoC
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreSoW SoW
    {
        get => _SoW ??= new CoreSoW();
        set => _SoW = value;
    }
    private static CoreSoW _SoW;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.RunCore();
    }

    public void CompleteCoreSoC()
    {
        if (Core.isCompletedBefore(7768))
        {
            Core.Logger("Saga Complete: Shadows Of Chaos");
            return;
        }

        SoW.ShadowWar();
        DualPlane();
        ChaosAmulet();
        LagunaBeach();
        Laguna();
        ShadowOff();
        BrightShadow();
        BrightChaos();
        BrightForest();
        BrightForestPast();
        BrightForestExtra();
    }

    public void DualPlane()
    {
        if (Core.isCompletedBefore(7571))
        {
            Core.Logger("StoryLine Complete:  Dualplane storyline");
            return;
        }

        Story.PreLoad(this);

        //Check their Teeth 7561
        Story.KillQuest(7561, "dualplane", "Terrarsite");

        //More Investigation Needed 7562
        Story.KillQuest(7562, "dualplane", "Droognax");

        //Find Xiang 7563
        Story.MapItemQuest(7563, "dualplane", 7459);

        //Destroy her Creatures 7564
        Story.KillQuest(7564, "dualplane", "Terrarsite");

        //Time to un-decorate 7565
        if (!Story.QuestProgression(7565))
        {
            Core.EnsureAccept(7565);
            Core.HuntMonster("dualplane", "SpiderWing", "Batwings", 4);
            Core.HuntMonster("dualplane", "Terrarsite", "Horns", 4);
            Core.EnsureComplete(7565);
        }

        //Interrogation Time 7566
        Story.KillQuest(7566, "dualplane", "Droognax");

        //Get rid of it! 7567
        Story.KillQuest(7567, "dualplane", "Netherpit Lackey");

        //"Convince" the Bruiser 7568
        Story.KillQuest(7568, "dualplane", "Netherpit Bruiser");

        //They Never Learn 7569
        if (!Story.QuestProgression(7569))
        {
            Core.EnsureAccept(7569);
            Core.HuntMonster("dualplane", "Terrarsite", "Monsters Defeated", 10);
            Core.HuntMonster("dualplane", "Netherpit Lackey", "Willing Lackey", 1);
            Core.EnsureComplete(7569);
        }

        //Get the Mirror! 7570
        Story.MapItemQuest(7570, "dualplane", 7460);

        //Get Her! 7571
        Story.KillQuest(7571, "dualplane", "Xiang");
    }

    public void ChaosAmulet()
    {
        if (Core.isCompletedBefore(7688))
        {
            Core.Logger("StoryLine Complete:  Chaos Amulet storyline");
            return;
        }

        DualPlane();

        Story.PreLoad(this);

        //Mega Shadow Medals
        if (!Bot.Quests.IsUnlocked(7687))
        {
            // Shadow Medals 7685
            if (!Bot.Quests.IsUnlocked(7686))
            {
                Core.EnsureAccept(7685);
                Core.HuntMonster("chaosamulet", "Shadowflame Scout", "Shadow Medal", 5);
                Core.EnsureComplete(7685);
            }
            Core.EnsureAccept(7686);
            Core.HuntMonster("chaosamulet", "Shadowflame Scout", "Mega Shadow Medal", 3);
            Core.EnsureComplete(7686);
        }

        //Defeat Goldun 7687
        if (!Story.QuestProgression(7687))
        {
            Core.EnsureAccept(7687);
            Core.HuntMonster("chaosamulet", "Goldun", "Goldun Defeated");
            Core.EnsureComplete(7687);
        }

        //Goldun Wants Revenge 7688
        if (!Story.QuestProgression(7688))
        {
            Core.EnsureAccept(7688);
            Core.HuntMonster("chaosamulet", "Goldun", "Goldun Defeated", 1);
            Core.HuntMonster("chaosamulet", "Shadowflame Berserker", "Minion Quelled", 30);
            Core.EnsureComplete(7688);
        }
    }

    public void LagunaBeach()
    {
        if (Core.isCompletedBefore(7700))
        {
            Core.Logger("StoryLine Complete:  Laguna Beach storyline");
            return;
        }
        Story.PreLoad(this);

        ChaosAmulet();

        //Eyes on You 7690
        Story.KillQuest(7690, "lagunabeach", "Flying Fisheye");

        //Behind the Tentacles 7691
        Story.MapItemQuest(7691, "lagunabeach", 7636);

        //Overwhelming Minions 7692
        Story.KillQuest(7692, "lagunabeach", "ShadowChaos Brigand");

        //Ground Kelp? 7693
        if (!Story.QuestProgression(7693))
        {
            Core.EnsureAccept(7693);
            Core.GetMapItem(7637, 5, "lagunabeach");
            Core.HuntMonster("lagunabeach", "Chaos Kelp", "Kelp Sample", 8);
            Core.EnsureComplete(7693);
        }

        //Slug 'em! 7694
        Story.KillQuest(7694, "lagunabeach", "ShadowChaos Brigand");

        //Shadows and Chaos 7695
        Story.KillQuest(7695, "lagunabeach", "Flying Fisheye");

        //Follow the Trail 7696
        Story.MapItemQuest(7696, "lagunabeach", 7638, 3);
        Story.MapItemQuest(7696, "lagunabeach", 7639);

        //Clear the Tunnel 7697
        Story.KillQuest(7697, "lagunabeach", "ShadowChaos Brigand");

        //Blow it up! 7698
        if (!Story.QuestProgression(7698))
        {
            Core.EnsureAccept(7698);
            Core.GetMapItem(7640, 1, "lagunabeach");
            Core.HuntMonster("lagunabeach", "ShadowChaos Gunner", "Grenade", 5);
            Core.EnsureComplete(7698);
        }

        //The Heart of the Matter 7699
        Story.KillQuest(7699, "lagunabeach", "Heart of Chaos");

        // A Closer Look 7700
        Story.KillQuest(7700, "lagunabeach", "Flying Fisheye");
    }

    public void Laguna()
    {
        if (Core.isCompletedBefore(7712))
        {
            Core.Logger("StoryLine Complete:  Laguna storyline");
            return;
        }

        LagunaBeach();

        Story.PreLoad(this);

        //Fight the Crew 7702
        Story.KillQuest(7702, "laguna", "ShadowChaos Brigand");

        //Reach the Ship 7703
        Story.MapItemQuest(7703, "laguna", 7675);

        //Get the Bombs 7704
        Story.KillQuest(7704, "laguna", "ShadowChaos Gunner");

        //Blow 'er Up! 7705
        if (!Story.QuestProgression(7705))
        {
            Core.EnsureAccept(7705);
            Core.GetMapItem(7676, 1, "laguna");
            Core.HuntMonster("laguna", "ShadowChaos Gunner", "Matchstick", 4);
            Core.EnsureComplete(7705);
        }

        //Defeat the Captain 7706
        Story.KillQuest(7706, "laguna", "Captain Laguna");

        //More Pirates 7707
        Story.KillQuest(7707, "laguna", "ShadowChaos Brigand");

        //Steal for Scouts 7708
        Story.KillQuest(7708, "laguna", "Chaos Roe");

        //Make them Work 7709
        Story.KillQuest(7709, "laguna", "Chaos Burrower");

        //Find the Amulet 7710
        if (!Story.QuestProgression(7710))
        {
            Core.EnsureAccept(7710);
            Core.GetMapItem(7678, 1, "laguna");
            Core.HuntMonster("laguna", "ShadowChaos Brigand", "Monster Slain", 9);
            Core.EnsureComplete(7710);
        }

        //Retrieve the Amulet 7711
        Story.KillQuest(7711, "laguna", "Writhing Chaos");

        //Snack Time 7712
        Story.KillQuest(7712, "laguna", "Chaos Roe");
    }

    public void ShadowOff()
    {
        if (Core.isCompletedBefore(7732))
        {
            Core.Logger("StoryLine Complete:  Shadowoff storyline");
            return;
        }

        Laguna();

        Story.PreLoad(this);

        // Invasion!
        if (!Story.QuestProgression(7728))
        {
            Core.EnsureAccept(7728);
            Core.HuntMonster("shadowoff", "Shadowflame Militia", "Invader Slain", 10);
            Core.EnsureComplete(7728);
        }

        //Rescue Needed
        if (!Story.QuestProgression(7729))
        {
            Core.EnsureAccept(7729);
            Core.GetMapItem(7699, 6, "Shadowoff");
            Core.HuntMonster("shadowoff", "Shadowflame Sorcerer", "Sorcerer Slain", 6);
            Core.EnsureComplete(7729);
        }

        //Get their Intel
        Story.KillQuest(7730, "Shadowoff", "Shadowflame Militia");

        //Clue me in
        if (!Story.QuestProgression(7731))
        {
            Core.EnsureAccept(7731);
            Core.HuntMonster("shadowoff", "Shadowflame Militia", "Militia Clue", 2);
            Core.HuntMonster("shadowoff", "Shadowflame Scout", "Scout Clue", 2);
            Core.HuntMonster("shadowoff", "Shadowflame Sorcerer", "Sorcerer Clue", 2);
            Core.EnsureComplete(7731);
        }

        //So Familiar
        Story.KillQuest(7732, "Shadowoff", "Shadowflame Paladin");
    }

    public void BrightShadow()
    {
        if (Core.isCompletedBefore(7738))
        {
            Core.Logger("StoryLine Complete:  BrightShadow storyline");
            return;
        }

        ShadowOff();

        Story.PreLoad(this);

        //Protect the Light 7733
        Story.KillQuest(7733, "BrightShadow", "BrightFall light");

        //Fend Them Off 7734
        if (!Story.QuestProgression(7734))
        {
            Core.EnsureAccept(7734);
            Core.HuntMonster("brightshadow", "Brightfall Guard", "Invader Slain", 10);
            Core.EnsureComplete(7734);
        }

        //Get The Keys 7735
        Story.KillQuest(7735, "BrightShadow", "BrightFall Guard");

        //Unlock The Door 7736
        Story.MapItemQuest(7736, "BrightShadow", 7701);

        //Defeat Gravelyn 7737
        Story.KillQuest(7737, "BrightShadow", "Gravelyn the Good", AutoCompleteQuest: false);

        //Restoring Order 7738
        if (!Story.QuestProgression(7738))
        {
            Core.EnsureAccept(7738);
            Core.HuntMonster("brightshadow", "Brightfall Guard", "BrightFall Dark", 10);
            Core.HuntMonster("brightshadow", "Brightfall Light", "BrightFall Light", 5);
            Core.EnsureComplete(7738);
        }
    }

    public void BrightChaos()
    {
        if (Core.isCompletedBefore(7750))
        {
            Core.Logger("StoryLine Complete:  BrightChaos storyline");
            return;
        }
        BrightShadow();
        Story.PreLoad(this);

        //The Situation 7740
        Story.KillQuest(7740, "BrightChaos", "Shadowflame Militia");

        //A Study In Shadow 7741
        Story.KillQuest(7741, "BrightChaos", "Shadowflame Sorcerer");

        //Percision Strike 7742
        if (!Story.QuestProgression(7742))
        {
            Core.EnsureAccept(7742);
            Core.HuntMonster("brightchaos", "Shadowflame Militia", "Captain's Tags", 5);
            Core.EnsureComplete(7742);
        }

        //Pants On Fire 7743
        if (!Story.QuestProgression(7743))
        {
            Core.EnsureAccept(7743);
            Core.GetMapItem(7731, 6, "BrightChaos");
            Core.HuntMonster("brightchaos", "Shadow Flame", "Shadow Flame Slain", 10);
            Core.EnsureComplete(7743);
        }

        //Scared of the Dark 7744
        if (!Story.QuestProgression(7744))
        {
            Core.EnsureAccept(7744);
            Core.GetMapItem(7732, 5, "BrightChaos");
            Core.HuntMonster("brightchaos", "Hidden Monster", "Hidden Monster Slain", 10);
            Core.EnsureComplete(7744);
        }

        //Go Bye-Bye 7745
        Story.KillQuest(7745, "BrightChaos", "Shadow Trap");

        //Essential Oils 7746
        Story.KillQuest(7746, "BrightChaos", "ShadowBeast");

        //What A Mess 7747
        Story.MapItemQuest(7747, "BrightChaos", 7733, 6);

        //Proscuitto! 7748
        Story.KillQuest(7748, "BrightChaos", "ShadowBeast");

        //A Blight On Us All 7749
        Story.KillQuest(7749, "BrightChaos", "Blight");

        //Unending Blight 7750
        Story.KillQuest(7750, "BrightChaos", "Blight");
    }

    public void BrightForest()
    {
        if (Core.isCompletedBefore(7758))
        {
            Core.Logger("StoryLine Complete:  BrightForest storyline");
            return;
        }
        BrightChaos();
        Story.PreLoad(this);
        //Blood And Ashes 7756
        if (!Story.QuestProgression(7756))
        {
            Core.EnsureAccept(7756);
            Core.HuntMonster("brightforest", "Shadow Flame", "Ashes of Death", 5);
            Core.HuntMonster("brightforest", "Shadowflame Scout", "Blood of Life", 5);
            Core.EnsureComplete(7756);
        }

        //Drink and Fight 7757
        if (!Story.QuestProgression(7757))
        {
            Core.EnsureAccept(7757);
            Core.HuntMonster("brightforest", "Shadowflame Scout", "Essence of Time", 10);
            Core.EnsureComplete(7757);
        }

        //Oooommm 7758
        Story.MapItemQuest(7758, "BrightForest", 7754, 2);
        Story.MapItemQuest(7758, "BrightForest", 7755, 1);
    }

    public void BrightForestPast()
    {
        if (Core.isCompletedBefore(7766))
        {
            Core.Logger("StoryLine Complete:  BrightForestPast storyline");
            return;
        }
        BrightForest();
        Story.PreLoad(this);

        //Slay the Wraiths 7759
        Story.KillQuest(7759, "BrightForestPast", "Time Wraith");

        //Spacetime 7760
        if (!Story.QuestProgression(7760))
        {
            Core.EnsureAccept(7760);
            Core.GetMapItem(7753, 1, "BrightForestPast");
            Core.HuntMonster("brightforestpast", "Spacetime Energy", "Spacetime Gathered", 6);
            Core.EnsureComplete(7760);
        }

        //Find Khasaanda 7761
        Story.MapItemQuest(7761, "BrightForestPast", 7756);

        //Find the Heart 7762
        Story.KillQuest(7762, "BrightForestPast", "Undead Minion");

        //Ooh, That Smell 7763
        if (!Story.QuestProgression(7763))
        {
            Core.EnsureAccept(7763);
            Core.HuntMonster("brightforestpast", "Undead Minion", "Elemental Evil", 8);
            Core.EnsureComplete(7763);
        }

        //So Twisted 7764
        if (!Story.QuestProgression(7764))
        {
            Core.EnsureAccept(7764);
            Core.KillMonster("BrightForestPast", "r8", "Left", "Twisted Treeant", "Twisted Treeant Slain", 10);
            Core.EnsureComplete(7764);
        }

        //Purifying Flames 7765
        if (!Story.QuestProgression(7765))
        {
            Core.EnsureAccept(7765);
            Core.GetMapItem(7757, 1, "BrightForestPast");
            Core.KillMonster("BrightForestPast", "r8", "Left", "Treeant", "Purified Oil", 8);
            Core.EnsureComplete(7765);
        }

        //Return to Iadoa 7766
        Story.MapItemQuest(7766, "BrightForestPast", 7758);
    }

    public void BrightForestExtra()
    {
        if (Core.isCompletedBefore(7768))
        {
            Core.Logger("StoryLine Complete:  BrightForest Extra storyline");
            return;
        }

        BrightForestPast();

        Story.PreLoad(this);

        //The Dragon 7767
        Story.KillQuest(7767, "BrightForest", "ShadowFlame Dragon");

        //The Shadows Recede 7768
        Story.KillQuest(7768, "BrightForest", "ShadowFlame Dragon");
        Story.KillQuest(7768, "BrightForest", new[] { "Shadowflame Scout", "Shadowflame Warrior" });
    }
}
