/*
name: Fortress Delve Story
description: This will complete the Fortress Delve story.
tags: story, quest, fortress, delve, legion, staff, birthday
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleSiege.cs
//cs_include Scripts/Seasonal/StaffBirthdays/Nulgath/TempleDelve.cs
using Skua.Core.Interfaces;

public class FortressDelve
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static TempleDelve TD
    {
        get => _TD ??= new TempleDelve();
        set => _TD = value;
    }
    private static TempleDelve _TD;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        DoStory();
        Core.SetOptions(false);
    }

    public void DoStory()
    {
        TD.Storyline();
        if (Core.isCompletedBefore(9169))
            return;
        if (!Core.isSeasonalMapActive("fortressdelve"))
            return;

        Story.PreLoad(this);

        //Budding Scolex 9160
        Story.KillQuest(9160, "fortressdelve", "Doom Worm");

        //Scuffling Shadows 9161
        Story.MapItemQuest(9161, "fortressdelve", 11345);
        Story.MapItemQuest(9161, "fortressdelve", 11346, 2);
        Story.MapItemQuest(9161, "fortressdelve", 11347, 3);

        //Humans DNI 9162
        if (!Story.QuestProgression(9162))
        {
            Core.EnsureAccept(9162);
            Core.GetMapItem(11348, 1, "fortressdelve");
            Core.HuntMonster("fortressdelve", "Doomed Beast", "Beast's Vein", 7);
            Core.EnsureComplete(9162);
        }

        //Scion's Privilege 9163
        if (!Story.QuestProgression(9163))
        {
            Core.EnsureAccept(9163);
            Core.GetMapItem(11349, 1, "fortressdelve");
            Core.HuntMonster("fortressdelve", "Doom Worm", "Worm Disintegrated", 6);
            Core.EnsureComplete(9163);
        }

        //A Man-Cave and a Box of Scraps 9164
        if (!Story.QuestProgression(9164))
        {
            Core.EnsureAccept(9164);
            Core.GetMapItem(11350, 1, "fortressdelve");
            Core.HuntMonster("fortressdelve", "Legion Dreadmarch", "Dreadmarch Armor Scraps", 6);
            Core.EnsureComplete(9164);
        }

        //The Glare of the Light... 9165
        if (!Story.QuestProgression(9165))
        {
            Core.EnsureAccept(9165);
            Core.GetMapItem(11351, 1, "fortressdelve");
            Core.HuntMonster("fortressdelve", "Delirious Elemental", "Elemental Rune", 7);
            Core.EnsureComplete(9165);
        }

        //...Pierces the Deepest Shadowsl 9166
        if (!Story.QuestProgression(9166))
        {
            Core.EnsureAccept(9166);
            Core.GetMapItem(11352, 1, "fortressdelve");
            Core.HuntMonster("fortressdelve", "Enlightened Shadow", "Glowing Armor Scrap", 7);
            Core.EnsureComplete(9166);
        }

        //Lonely Supremacy 9167
        if (!Story.QuestProgression(9167))
        {
            Core.EnsureAccept(9167);
            Core.GetMapItem(11353, 1, "fortressdelve");
            Core.HuntMonster("fortressdelve", "Legion Dreadmarch", "Dreadmarch Badge", 8);
            Core.EnsureComplete(9167);
        }

        //The Condemner 9168
        if (!Story.QuestProgression(9168))
        {
            Core.EnsureAccept(9168);
            Core.HuntMonster("fortressdelve", "Enlightened Shadow", "Light Vein", 6);
            Core.HuntMonster("fortressdelve", "Delirious Elemental", "Elemental Shard", 6);
            Core.EnsureComplete(9168);
        }

        //Lost Light, Astero 9169
        Story.KillQuest(9169, "fortressdelve", "Astero");
    }
}
