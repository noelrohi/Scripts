/*
name: Lost Villa
description: This script will complete the questline in /lostvilla.
tags: lostvilla, diabolical, story, hikari, ira
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Story/Banished.cs
//cs_include Scripts/Story/QueenofMonsters/CoreQOM.cs
using Skua.Core.Interfaces;

public class LostVilla
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static Banished Ban
    {
        get => _Ban ??= new Banished();
        set => _Ban = value;
    }
    private static Banished _Ban;
    private static CoreQOM CoreQOM
    {
        get => _CoreQOM ??= new CoreQOM();
        set => _CoreQOM = value;
    }
    private static CoreQOM _CoreQOM;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(9570))
            return;

        CoreQOM.TheBook();
        Ban.HimisQuests();

        Story.PreLoad(this);

        // Scout Ahead (9561)
        if (!Story.QuestProgression(9561))
        {
            Core.EnsureAccept(9561);
            Core.GetMapItems(new[] { (12649, 1), (12650, 1) }, "lostvilla");
            Core.HuntMonster("lostvilla", "Eldritch Parasite", "Parasite Exterminated", 8);
            Core.EnsureComplete(9561);
        }

        // Gluttonous Slithering (9562)
        Story.KillQuest(9562, "lostvilla", "Gorewyrm");

        // Relics of Bygone Days (9563)
        if (!Story.QuestProgression(9563))
        {
            Core.EnsureAccept(9563);
            Core.GetMapItem(12651, 1, "lostvilla");
            Core.HuntMonster("lostvilla", "Eldritch Parasite", "Armory Guards Eradicated", 8);
            Core.EnsureComplete(9563);
        }

        // Rapacious Desires (9564)
        if (!Story.QuestProgression(9564))
        {
            Core.EnsureAccept(9564);
            Core.GetMapItem(12652, 3, "lostvilla");
            Core.HuntMonster("lostvilla", "Diabolical Hoard", "Hoard's Treasures", 10);
            Core.EnsureComplete(9564);
        }

        // Uninvited Guests (9565)
        if (!Story.QuestProgression(9565))
        {
            Core.EnsureAccept(9565);
            Core.HuntMonster("lostvilla", "Gorewyrm", "Gorewyrm Gored", 4);
            Core.HuntMonster("lostvilla", "Eldritch Parasite", "Parasite Exoskeleton", 6);
            Core.EnsureComplete(9565);
        }

        // Key of Life (9566)
        if (!Story.QuestProgression(9566))
        {
            Core.EnsureAccept(9566);
            Core.GetMapItem(12653, 1, "lostvilla");
            Core.HuntMonster("lostvilla", "Diabolical Hoard", "Mimic's Coin", 8);
            Core.EnsureComplete(9566);
        }

        // Proper Procedures (9567)
        if (!Story.QuestProgression(9567))
        {
            Core.EnsureAccept(9567);
            Core.GetMapItem(12654, 1, "lostvilla");
            Core.HuntMonster("noxustower", "Lightguard Caster", "Necrotic Tome", 1);
            Core.EnsureComplete(9567);
        }

        // A Peculiar Feast (9568)
        Story.KillQuest(9568, "lostvilla", "Mutilated Atrocity");

        // Clingy Creatures (9569)
        Story.KillQuest(9569, "lostvilla", "Eldritch Parasite");

        // Disfigured Aristrocrat (9570)
        Story.KillQuest(9570, "lostvilla", "Covetous Disgrace");
    }
}
