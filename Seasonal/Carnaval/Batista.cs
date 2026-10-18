/*
name: Batista's Quests
description: This script does Batista's Quests in carnaval.
tags: batista, carnaval, seasonal, story
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class Batista
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();
        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(2697) || !Core.isSeasonalMapActive("carnaval"))
            return;

        Story.PreLoad(this);

        //The Land of Monsters (2692)
        if (!Story.QuestProgression(2692))
        {
            Core.EnsureAccept(2692);
            Core.GetMapItem(1674, 1, "carnaval");
            Core.HuntMonster("carnaval", "Mapinguari", "Mapinguari Foot", 1);
            Core.EnsureComplete(2692);
        }

        //Battle and the Beasts (2693)
        if (!Story.QuestProgression(2693))
        {
            Core.EnsureAccept(2693);
            Core.HuntMonster("carnaval", "Mula Sem Cabeça", "Mula Sem Cabeça Defeated", 2);
            Core.HuntMonster("carnaval", "Mapinguari", "Mapinguari Defeated", 2);
            Core.HuntMonster("carnaval", "Cuco", "Cocu Defeated", 2);
            Core.HuntMonster("carnaval", "Boiuna", "Boiúna Defeated", 2);
            Core.HuntMonster("carnaval", "Mulher de Branco", "Mulher de Branco Defeated", 2);
            Core.EnsureComplete(2693);
        }

        //Festooned in Feathers (2694)
        Story.KillQuest(2694, "carnaval", "Boiuna");

        //Mask your Emotions (2695)
        Story.KillQuest(2695, "carnaval", "Boiuna");

        //Crafting the Carranca (2696)
        if (!Story.QuestProgression(2696))
        {
            Core.EnsureAccept(2696);
            Core.GetMapItem(1677, 5, "carnaval");
            Core.HuntMonster("carnaval", "Mulher de Branco", "Mulher de Branco's Hair", 1);
            Core.HuntMonster("carnaval", "Mula Sem Cabe\u00e7a", "Horse Tail Paint Brush", 1);
            Core.HuntMonster("carnaval", "Boiuna", "Boi\u00fana Fangs", 2);
            Core.HuntMonster("carnaval", "Cuco", "Cuco's Claw", 1);
            Core.EnsureComplete(2696);
        }

        //Party Like a BOSS!
        Story.KillQuest(2697, "carnaval", "Lobisomem");
    }
}
