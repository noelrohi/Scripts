/*
name: Ghost Ship
description: Completes the Ghost Ship storyline after Merrow Trench.
tags: ghost ship, story, seasonal, friday the 13th
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class GhostShip
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story { get => _Story ??= new CoreStory(); set => _Story = value; }
    private static CoreStory _Story;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();
        Storyline();
        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(7101))
            return;

        if (!Core.isCompletedBefore(7084))
        {
            Core.Logger("Complete the Merrow Trench storyline first.", messageBox: true, stopBot: true);
            return;
        }

        Story.PreLoad(this);

        Story.KillQuest(7089, "ghostship", "Spectro-Plasm");
        Story.KillQuest(7090, "ghostship", "Spooky Sailor");
        Story.KillQuest(7091, "ghostship", "Spectro-Plasm");
        Story.KillQuest(7092, "ghostship", "Ghostly Fishwing");
        Story.MapItemQuest(7093, "ghostship", 6695);
        Story.KillQuest(7094, "ghostship", "Ghostly Fishwing");
        Story.MapItemQuest(7095, "ghostship", 6696);
        Story.KillQuest(7096, "ghostship", "Ghostly Fishwing");
        Story.MapItemQuest(7097, "ghostship", 6697);
        Story.KillQuest(7098, "ghostship", "Sea Wraith");

        if (!Story.QuestProgression(7099))
        {
            Core.EnsureAccept(7099);
            Core.HuntMonster("ghostship", "Ghostly Eel", "Monster Slain", 9);
            Core.Jump("r9", "Left");
            Bot.Wait.ForPickup("Reach the Door");
            Core.EnsureComplete(7099);
        }

        if (!Story.QuestProgression(7100))
        {
            Core.EnsureAccept(7100);
            Core.GetMapItem(6698, 1, "ghostship");
            Core.HuntMonster("ghostship", "Ghostly Eel", "Ghostly Electricity", 8);
            Core.EnsureComplete(7100);
        }
        Story.KillQuest(7101, "ghostship", "Cursed Cecaelia");
    }
}
