/*
name: Dragon Pirate Story
description: This will finish the Dragon Pirate Story Line.
tags: story, quest, dragon, pirate
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class DragonPirateStory
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

        DragonPirate();

        Core.SetOptions(false);
    }

    public void DragonPirate()
    {
        if (!Core.isSeasonalMapActive("dragonpirate"))
            return;
        if (Core.isCompletedBefore(8275))
            return;

        Story.PreLoad(this);

        // 8268 They Be Draconian
        Story.KillQuest(8268, "dragonpirate", "Pirate Draconian");

        // 8269 Pirate Pals
        Story.KillQuest(8269, "dragonpirate", "Pirate Wyvern");

        // 8270 Commandeer The Vessel
        if (!Story.QuestProgression(8270))
        {
            Core.EnsureAccept(8270);
            Core.GetMapItem(8930, 1, "dragonpirate");
            Core.HuntMonster("dragonpirate", "Pirate Draconian", "Enemies Cleared", 6);
            Core.EnsureComplete(8270);
        }

        // 8271 There Be Dragon
        Story.KillQuest(8271, "dragonpirate", "Dragon Pirate");

        // 8272 Man the Cannon
        Story.KillQuest(8272, "dragonpirate", "Dragon Gunner");

        // 8273 Boarding Party
        if (!Story.QuestProgression(8273))
        {
            Core.EnsureAccept(8273);
            Core.GetMapItem(8931, 1, "dragonpirate");
            Core.HuntMonster("dragonpirate", "Dragon Pirate", "Boarding Planks Collected", 5);
            Core.EnsureComplete(8273);
        }

        // 8274 The Mates First
        Story.KillQuest(8274, "dragonpirate", "Pirate Draconian");

        // 8275 Captain Scalebeard
        Story.KillQuest(8275, "dragonpirate", "Scalebeard");
    }
}
