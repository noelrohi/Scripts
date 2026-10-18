/*
name: Celestial Past (Extra)
description: This will finish the Celestial Past quest.
tags: story, quest, queen-of-monsters, celestial-past, extra
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class CelestialPast
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

        CompleteCeletialPast();

        Core.SetOptions(false);
    }

    public void CompleteCeletialPast()
    {
        if (Core.isCompletedBefore(7681))
            return;

        Story.PreLoad(this);

        //Wayfarer Potion 7674
        if (!Story.QuestProgression(7674))
        {
            Core.EnsureAccept(7674);
            Core.HuntMonster("celestialpast", "Blessed Deer", "Leaf of Visibility", 1);
            Core.HuntMonster("celestialpast", "Blessed Bear", "Root of Clarity", 1);
            Core.HuntMonster("celestialpast", "Blessed Centaur", "Spice of Direction", 1);
            Core.HuntMonster("celestialpast", "Blessed Hydra", "Broth", 1);
            Core.EnsureComplete(7674);
        }

        //The Path Revealed 7675
        if (!Story.QuestProgression(7675))
        {
            Core.EnsureAccept(7675);
            Core.GetMapItem(7592, 1, "CelestialPast");
            Core.HuntMonster("celestialpast", "Blessed Deer", "Beasts Cleared", 5);
            Core.EnsureComplete(7675);
        }

        //Well of Knowledge 7676
        if (!Story.QuestProgression(7676))
        {
            Core.EnsureAccept(7676);
            Core.GetMapItem(7593, 1, "CelestialPast");
            Core.HuntMonster("celestialpast", "Blessed Deer", "Beasts Defeated", 7);
            Core.EnsureComplete(7676);
        }

        //Gather the Artifacts 7677
        if (!Story.QuestProgression(7677))
        {
            Core.EnsureAccept(7677);
            Core.GetMapItem(7594, 1, "CelestialPast");
            Core.HuntMonster("celestialpast", "Blessed Deer", "Horn of Blessed Gold", 1);
            Core.HuntMonster("celestialpast", "Blessed Centaur", "Tail of Blessed Centaur", 1);
            Core.HuntMonster("celestialpast", "Blessed Hydra", "Blessed Green Water", 1);
            Core.HuntMonster("celestialpast", "Blessed Bear", "Blessed Bear Claw", 1);
            Core.EnsureComplete(7677);
        }

        //Oh Well, Oh Well Guardian 7678
        if (!Story.QuestProgression(7678))
        {
            Core.EnsureAccept(7678);
            Core.GetMapItem(7595, 1, "CelestialPast");
            Core.HuntMonster("celestialpast", "Well Guardian", "Well Guardian Defeated", 1);
            Core.EnsureComplete(7678);
        }

        //Infernal Commands 7679
        Story.KillQuest(7679, "CelestialPast", "Infernal Soldier");

        //Forward Unto Azalith 7680
        Story.KillQuest(7680, "CelestialPast", "Infernal Soldier");

        //Azalith Faced 7681    [Confront Quest] - You must die here to complete it
        if (!Story.QuestProgression(7681))
        {
            Core.EnsureAccept(7681);

            while (!Bot.ShouldExit && !Bot.TempInv.Contains(56077 /* Azalith Faced */))
            {
                if (Bot.Map.Name != "CelestialPast")
                    Core.Join("CelestialPast");
                if (Bot.Player.Cell != "r11a")
                    Core.Jump("r11a", "Left");

                if (!Bot.Player.InCombat)
                    Bot.Combat.Attack("*");

                Bot.Sleep(500);
                if ( Bot.TempInv.Contains(56077 /* Azalith Faced */))
                {
                    Core.JumpWait();
                    break;
                }
            }

            Core.EnsureComplete(7681);
        }
    }
}
