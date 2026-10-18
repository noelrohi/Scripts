/*
name: Titan Attack Story
description: This will finish the Titan Attack Story.
tags: story, quest, titan-attack
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;

public class TitanAttackStory
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        DoAll();

        Core.SetOptions(false);
    }

    public void DoAll()
    {
        if (Core.isCompletedBefore(8777))
            return;

        Story.PreLoad(this);

        // Those Who Remain 8759
        Story.MapItemQuest(8759, "titanattack", new[] { 10345, 10346, 10347, 10348, 10349 });

        // Who Helps the Healer? 8760
        if (!Story.QuestProgression(8760))
        {
            Core.EnsureAccept(8760);
            Core.HuntMonster("titanattack", "Corrosive Crawler", "Crawler Chitin", 5);
            Core.HuntMonster("titanattack", "Chaorrupted Bandit", "Pilfered Potion", 5);
            Core.EnsureComplete(8760);
        }

        // Supply in Demand 8761
        Story.KillQuest(8761, "titanattack", "Supply Caravan");

        // Creature Comforts 8762
        if (!Story.QuestProgression(8762))
        {
            Core.EnsureAccept(8762);
            Core.HuntMonster("titanattack", "Chaos Wyvern", "Game Ration", 10);
            Core.HuntMonster("titanattack", "Corrosive Crawler", "Insect Ration", 10);
            Core.EnsureComplete(8762);
        }

        // Infernal Investigation 8763
        if (!Story.QuestProgression(8763))
        {
            Core.EnsureAccept(8763);
            Core.HuntMonster("titanattack", "Chaorrupted Bandit", "Bandit Beaten", 6);
            Core.HuntMonster("titanattack", "AntiTitan Corps", "Corps Questioned", 6);
            Core.EnsureComplete(8763);
        }

        // Another Brick in the Wall 8764
        if (!Story.QuestProgression(8764))
        {
            Story.MapItemQuest(8764, "titanattack", 10350, 3);
            Core.HuntMonster("titanattack", "Supply Caravan", "Building Brick", 15);
            Core.HuntMonster("titanattack", "Corrosive Crawler", "Chitin Coating", 8);
            Core.HuntMonster("titanattack", "Chaos Dweller", "Adhesive Saliva", 7);
            Core.EnsureComplete(8764);
        }

        // Rotten to the Corps 8765
        Story.KillQuest(8765, "titanattack", "AntiTitan Corps");

        // A Fighting Chance 8766
        if (!Story.QuestProgression(8766))
        {
            Core.EnsureAccept(8766);
            Core.GetMapItem(10351, 8, "titanattack");
            Core.HuntMonster("titanattack", "Corrosive Crawler", "Sharp Stinger", 8);
            Core.HuntMonster("titanattack", "Supply Caravan", "Battle Wire", 8);
            Core.EnsureComplete(8766);
        }

        // Flash of the Titans 8767
        if (!Story.QuestProgression(8767))
        {
            Core.EnsureAccept(8767);
            Core.HuntMonster("titanattack", "Supply Caravan", "Shine Powder", 5);
            Core.HuntMonster("titanattack", "Supply Caravan", "Explosive Casing", 10);
            Core.HuntMonster("titanattack", "Chaotic Beholder", "Smoke Sac", 5);
            Core.EnsureComplete(8767);
        }

        // Vein Ambition 8768
        Story.KillQuest(8768, "titanattack", "Corrosive Crawler");

        // Asset Acquisition 8769
        if (!Story.QuestProgression(8769))
        {
            Core.EnsureAccept(8769);
            Core.HuntMonster("titanattack", "Supply Caravan", "Food Crate", 5);
            Core.HuntMonster("titanattack", "Supply Caravan", "Medicine Vial", 15);
            Core.HuntMonster("titanattack", "Chaorrupted Bandit", "Bandit Blade", 15);
            Core.EnsureComplete(8769);
        }

        // Enemy at the Gate 8770
        Story.MapItemQuest(8770, "titanattack", 10352);

        // Titanic Terror 8771
        Story.KillQuest(8771, "titanattack", "Titanic Vindicator");

        // Titanic Terror Times Two 8772
        if (!Story.QuestProgression(8772))
        {
            Core.EnsureAccept(8772);
            Core.HuntMonster("titanattack", "Titanic Paladin", "Titanic Paladin Felled", 1);
            Core.HuntMonster("titanattack", "Titanic DoomKnight", "Titanic DoomKnight Felled", 1);
            Core.EnsureComplete(8772);
        }

        // A Little Light... 8773
        Story.KillQuest(8773, "titanstrike", "Titanic Paladin");

        // A Dose of Darkness... 8774
        Story.KillQuest(8774, "titanstrike", "Titanic DoomKnight");

        // And A Cup of Chaos 8775
        Story.KillQuest(8775, "titanstrike", "Titanic Destroyer");

        // Topple Some Titans! 8776
        Core.AddDrop("Heroic Titan's Greatsword");
        if (!Story.QuestProgression(8776))
        {
            Core.EnsureAccept(8776);
            Core.HuntMonster("titanstrike", "Titanic Paladin", "Paladin Punished", 1);
            Core.HuntMonster("titanstrike", "Titanic DoomKnight", "Doomknight Decimated", 1);
            Core.HuntMonster("titanstrike", "Titanic Destroyer", "Destroyer Destroyed", 1);
            Core.EnsureComplete(8776);
        }

        // The BIG Finish 8777
        Adv.EnhanceItem(
            "Heroic Titan's Greatsword",
            EnhancementType.Lucky,
            CapeSpecial.None,
            HelmSpecial.None,
            WeaponSpecial.Spiral_Carve
        );

        Story.KillQuest(8777, "titandrakath", "Titan Drakath");
    }
}
