/*
name: Loves Curse
description: This will complete the Loves Curse story quest.
tags: story, quest, hero, heart, seasonal, loves, curse
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class LovesCurse
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

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (!Core.isSeasonalMapActive("lovescurse") || Core.isCompletedBefore(1671))
            return;

        Story.PreLoad(this);

        // Intro To Love (1665)
        Story.ChainQuest(1665);

        //Find the Rest of J6 (1656)
        if (!Story.QuestProgression(1656))
        {
            Core.EnsureAccept(1656);
            Core.HuntMonster("curseshore", "Escaped Ghostly Zardman", "Cyborg Arm", 2);
            Core.HuntMonster("curseshore", "Escaped Ghostly Zardman", "Cyborg Leg", 2);
            Core.HuntMonster("curseshore", "Escaped Ghostly Zardman", "Cyborg Chassis", 1);
            Core.EnsureComplete(1656);
        }

        //Mer'Angel Hunting (1657)
        Story.MapItemQuest(1657, "curseshore", 865);

        //Into the Underworld (1666)
        Story.ChainQuest(1666);

        //Search for the Cursed Lovers (1658)
        Story.MapItemQuest(1658, "curseblue", 866);

        //Love Ghosts (1667)
        Story.ChainQuest(1667);

        //Sing the Blues (1659)
        Story.KillQuest(1659, "curseblue", "Ghostly Aracara");

        //Land of the Red (1660)
        Story.MapItemQuest(1660, "cursered", 868);

        //Help Moore (1668)
        Story.ChainQuest(1668);

        //Find the Source of the Curse (1661)
        if (!Story.QuestProgression(1661))
        {
            Core.EnsureAccept(1661);
            Core.HuntMonster("cursered", "Cyclops Warlord Ghost", "Key of True Love", 1);
            Core.HuntMonster("cursered", "Cyclops Warlord Ghost", "Key of Eternity", 1);
            Core.EnsureComplete(1661);
        }

        //Defeat the CurseMaker (1662)
        Story.KillQuest(1662, "mercutio", "*");

        //Mercutio (1669)
        Story.ChainQuest(1669);

        //One Moore Time (1663)
        Story.MapItemQuest(1663, "cursered", 869);

        //Out of Time (1670)
        Story.ChainQuest(1670);

        //You Only Get One Shot at True Love (1664)
        Story.MapItemQuest(1664, "curseblue", 867);

        //Happy Ending (1671)
        Story.ChainQuest(1671);
    }
}
