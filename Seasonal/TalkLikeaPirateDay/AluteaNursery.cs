/*
name: Alutea Nursery
description: This will finish the Alutea Nursery Quest.
tags: story, quest, alutea-nursery, pirate
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/TalkLikeaPirateDay/LowTideStory.cs

using Skua.Core.Interfaces;
using Skua.Core.Models.Items;

public class AluteaNursery
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static LowTideStory LowTideStory
    {
        get => _LowTideStory ??= new LowTideStory();
        set => _LowTideStory = value;
    }
    private static LowTideStory _LowTideStory;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoAll();

        Core.SetOptions(false);
    }

    public void DoAll()
    {
        if (!Core.isSeasonalMapActive("AluteaNursery"))
            return;

        AluteaNurseryStory();
        AluteaNurseryRewards();
    }

    public void AluteaNurseryStory()
    {
        LowTideStory.Storyline();

        if (Core.isCompletedBefore(8857))
            return;

        Story.PreLoad(this);

        //Carefree Drifters 8848
        Story.KillQuest(8848, "AluteaNursery", "Spectral Jellyfish");

        //Bone Bites 8849
        Story.KillQuest(8849, "AluteaNursery", "Bone Crustacean");

        //The Lost Generation 8850
        if (!Story.QuestProgression(8850))
        {
            Core.EnsureAccept(8850);
            Core.GetMapItem(10557, 5, "AluteaNursery");
            Core.HuntMonster("aluteanursery", "Bone Crustacean", "Crustacean Caught", 8);
            Core.EnsureComplete(8850);
        }

        //Tender Temple 8851
        if (!Story.QuestProgression(8851))
        {
            Core.EnsureAccept(8851);
            Core.GetMapItem(10558, 1, "AluteaNursery");
            Core.HuntMonster("aluteanursery", "Spectral Jellyfish", "Cold Stingers", 8);
            Core.EnsureComplete(8851);
        }

        //Cast Away Buds 8852
        if (!Story.QuestProgression(8852))
        {
            Core.EnsureAccept(8852);
            Core.GetMapItem(10559, 2, "AluteaNursery");
            Core.HuntMonster("aluteanursery", "Spectral Jellyfish", "Jelly Tormentors", 8);
            Core.EnsureComplete(8852);
        }

        //Current Catching 8853
        Story.MapItemQuest(8853, "AluteaNursery", 10563, 5);

        //Silent Sea 8854
        if (!Story.QuestProgression(8854))
        {
            Core.EnsureAccept(8854);
            Core.GetMapItems(new[] { (10560, 1), (10561, 1) }, "AluteaNursery");
            Core.HuntMonster("aluteanursery", "Stagnant Water", "Stagnant Water Cleared", 8);
            Core.EnsureComplete(8854);
        }

        //Hay! Bale Already! 8855
        if (!Story.QuestProgression(8855))
        {
            Core.EnsureAccept(8855);
            Core.GetMapItem(10562, 2, "AluteaNursery");
            Core.HuntMonster("aluteanursery", "Bone Crustacean", "Shellfish Meat", 8);
            Core.HuntMonster("aluteanursery", "Spectral Jellyfish", "Jelly Filling", 8);
            Core.EnsureComplete(8855);
        }

        //Abyss to Sun 8856
        if (!Story.QuestProgression(8856))
        {
            Core.EnsureAccept(8856);
            Core.GetMapItem(10564, 1, "AluteaNursery");
            Core.HuntMonster("aluteanursery", "Stagnant Water", "Elemental Defeated", 8);
            Core.EnsureComplete(8856);
        }

        //Ebbing Grudges 8857
        Story.KillQuest(8857, "AluteaNursery", "Last Alutian");
    }

    public void AluteaNurseryRewards()
    {
        List<ItemBase> RewardOptions = Core.EnsureLoad(8858).Rewards;
        List<string> RewardsList = new();
        List<string> RewardList = RewardOptions.Select(x => x.Name).ToList();

        string[] Rewards = RewardsList.ToArray();

        if (Core.CheckInventory(Rewards, toInv: false))
            return;

        int count = 0;
        Core.CheckSpaces(ref count, Rewards);

        Core.AddDrop(Rewards);
        Core.RegisterQuests(8858);
        while (!Bot.ShouldExit && !Core.CheckInventory(Rewards, toInv: false))
        {
            //Beach Cleanse 8858
            Core.EquipClass(ClassType.Solo);
            Core.HuntMonster("aluteanursery", "Last Alutian", "Angler Antena", 1, log: false);
            Core.EquipClass(ClassType.Farm);
            Core.HuntMonster("aluteanursery", "Stagnant Water", "Stale Seawater", 6, log: false);
            Core.HuntMonster("aluteanursery", "Bone Crustacean", "Pale Shell", 6, log: false);
        }
        Core.CancelRegisteredQuests();
        Core.ToBank(Rewards);
    }
}
