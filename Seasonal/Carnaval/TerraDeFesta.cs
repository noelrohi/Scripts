/*
name: Terra de Festa
description: Completes Frevo's Quests in terradefesta.
tags: freva, terra de festa, seasonal, carnaval, story
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Carnaval/Batista.cs
using Skua.Core.Interfaces;

public class TerraDeFesta
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static Batista Bat
    {
        get => _Bat ??= new Batista();
        set => _Bat = value;
    }
    private static Batista _Bat;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();
        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(3392) || !Core.isSeasonalMapActive("terradefesta"))
            return;

        Bat.Storyline();

        Story.PreLoad(this);

        //Frevo's Dancers Need Feathers! (3398)
        Story.KillQuest(3398, "carnaval", "Cuco");

        //Dancers Need Feathers (3382)
        Story.KillQuest(3382, "carnaval", "Cuco");

        //And Dolls Need Heads (3383)
        Story.KillQuest(3383, "terradefesta", "GreenRat");

        //A Head Needs A Body (3384)
        if (!Story.QuestProgression(3384))
        {
            Core.EnsureAccept(3384);
            Core.HuntMonster("terradefesta", "Crow", "Scrap of Fabric", 4);
            Core.HuntMonster("terradefesta", "Crow", "Bundle of Straw", 4);
            Core.EnsureComplete(3384);
        }

        //Bind It All Together (3385)
        Story.KillQuest(3385, "carnaval", "Mulher de Branco");

        //Take 'Em Apart (3386)
        if (!Story.QuestProgression(3386))
        {
            Core.EnsureAccept(3386);
            Core.HuntMonster("terradefesta", "Voodoo Doll", "Feathered Topknot", 4);
            Core.HuntMonster("terradefesta", "Voodoo Doll", "Little Cloth Bodies", 4);
            Core.HuntMonster("terradefesta", "Voodoo Doll", "Straw Stuffing", 4);
            Core.HuntMonster("terradefesta", "Voodoo Doll", "Skully Head", 4);
            Core.EnsureComplete(3386);
        }

        //Who Could It Be? (3387)
        if (!Story.QuestProgression(3387))
        {
            Core.EnsureAccept(3387);
            Core.HuntMonster("terradefesta", "Party Zombie", "Seriously Bad Mojo", 1);
            Core.HuntMonster("terradefesta", "Party Zombie", "A Dramatic Flair", 1);
            Core.HuntMonster("terradefesta", "Party Zombie", "The Smell Of Death", 1);
            Core.HuntMonster("terradefesta", "Party Zombie", "Distinctive Talisman", 1);
            Core.EnsureComplete(3387);
        }

        //A Bag For A Talisman (3388)
        Story.KillQuest(3388, "terradefesta", "GreenRat");

        //Their... third eyes? (3389)
        Story.KillQuest(3389, "terradefesta", "Crow");

        //Hexy Ladies (3390)
        Story.KillQuest(3390, "terradefesta", "Dancing Zombie");

        //Tracking Down A Baron (3391)
        if (!Story.QuestProgression(3391))
        {
            Core.EnsureAccept(3391);
            Core.HuntMonster("terradefesta", "GreenRat", "Rat Guts", 4);
            Core.HuntMonster("terradefesta", "Crow", "Crow Entrails", 4);
            Core.EnsureComplete(3391);
        }

        //Defeat Baron Sunday (3392)
        Story.KillQuest(3392, "terradefesta", "Baron Sunday");
    }
}
