/*
name: Firebird
description: Completes Fausto's Quests in firebird.
tags: carnaval, firebird, fausto, seasonal, story
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Carnaval/Batista.cs
//cs_include Scripts/Seasonal/Carnaval/TerraDeFesta.cs
using Skua.Core.Interfaces;

public class Firebird
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static TerraDeFesta TDF
    {
        get => _TDF ??= new TerraDeFesta();
        set => _TDF = value;
    }
    private static TerraDeFesta _TDF;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();
        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(5721) || !Core.isSeasonalMapActive("firebird"))
            return;

        TDF.Storyline();

        Story.PreLoad(this);

        //Turn Off the Heat (5711)
        Story.KillQuest(5711, "firebird", "Flaming Dancer");

        //One Man's Trash... (5712)
        Story.KillQuest(5712, "firebird", "Lantern");

        //Some Fiddling and the Right Spells (5713)
        if (!Story.QuestProgression(5713))
        {
            Core.EnsureAccept(5713);
            Core.HuntMonster("terradefesta", "GreenRat", "Green Fur", 8);
            Core.HuntMonster("terradefesta", "Crow", "Crow Egg", 5);
            Core.HuntMonster("terradefesta", "Baron Sunday", "Voodoo Dust", 1);
            Core.EnsureComplete(5713);
        }

        //I Need More Time (5714)
        if (!Story.QuestProgression(5714))
        {
            Core.EnsureAccept(5714);
            Core.HuntMonster("firebird", "Fire Elemental", "Elemental Out", 6);
            Core.HuntMonster("firebird", "Phoenix", "Phoenix Out", 6);
            Core.EnsureComplete(5714);
        }

        //Trapped Flames (5715)
        Story.MapItemQuest(5715, "firebird", 5156, 10);

        //Get Inside! (5716)
        Story.KillQuest(5716, "firebird", "Flame Imp");

        //Where There is Smoke... (5717)
        Story.MapItemQuest(5717, "firebird", 5157);

        //Through the Flames (5718)
        if (!Story.QuestProgression(5718))
        {
            Core.EnsureAccept(5718);
            Core.HuntMonster("carnaval", "Mulher de Branco", "Fireproof Hide", 5);
            Core.HuntMonster("carnaval", "Boiuna", "Black Scales", 7);
            Core.EnsureComplete(5718);
        }

        //Battling the Inferno (5719)
        Story.KillQuest(5719, "firebird", "Belo Passaro");

        //Ultra Belo Battle (5721)
        Core.EquipClass(ClassType.Solo);
        Story.KillQuest(5721, "firebird", "Ultra Belo");
    }
}
