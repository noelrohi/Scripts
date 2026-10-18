/*
name: Void Chasm Story
description: This script will complete the storyline in /voidchasm.
tags: voidchasm, chasm, nation, nulgath, story, ana di carcano, gravelyn, jadzia
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/Nation/VoidRefuge.cs
using Skua.Core.Interfaces;

public class VoidChasm
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    private static VoidRefuge VR
    {
        get => _VR ??= new VoidRefuge();
        set => _VR = value;
    }
    private static VoidRefuge _VR;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(9552))
            return;

        VR.Storyline();

        Story.PreLoad(this);

        // Light Headed 9543
        Story.KillQuest(9543, "voidchasm", "Paladin Ascendant");

        // Forgiveness of Debt 9544
        Story.KillQuest(9544, "voidchasm", "Nation Outrider");

        // Hyperspace-Plasia 9545
        if (!Story.QuestProgression(9545))
        {
            Core.EnsureAccept(9545);
            Core.GetMapItem(12619, 6, "voidchasm");
            Core.HuntMonster("voidchasm", "Paladin Ascendant", "Alberic Seal", 3);
            Core.EnsureComplete(9545);
        }

        // Tangled Aortas 9546
        Story.MapItemQuest(9546, "voidchasm", new[] { 12620, 12621 });

        // Nation Code 9547
        if (!Story.QuestProgression(9547))
        {
            Core.EnsureAccept(9547);
            Core.GetMapItem(12622, 1, "voidchasm");
            Core.HuntMonster("voidchasm", "Nation Outrider", "Outrider's Keepsake", 4);
            Core.EnsureComplete(9547);
        }

        // Snake Ears 9548
        Story.KillQuest(9548, "voidchasm", "Void Fang");

        // Faustian Leftovers 9549
        if (!Story.QuestProgression(9549))
        {
            Core.EnsureAccept(9549);
            Core.GetMapItems(new[] { (12623, 1), (12627, 1), (12628, 1) }, "voidchasm");
            Core.HuntMonster("voidchasm", "The Hushed", "Phantom's Sigh", 5);
            Core.EnsureComplete(9549);
        }

        // Broken Lives 9550
        if (!Story.QuestProgression(9550))
        {
            Core.EnsureAccept(9550);
            Core.GetMapItem(12624, 4, "voidchasm");
            Core.HuntMonster("voidchasm", "The Hushed", "Blank Contract", 3);
            Core.HuntMonster("voidchasm", "Void Fang", "Fiendish Sinew", 6);
            Core.EnsureComplete(9550);
        }

        // Call of the Fiend 9551
        Core.EquipClass(ClassType.Solo);
        if (!Story.QuestProgression(9551))
        {
            Core.EnsureAccept(9551);
            Core.GetMapItems(new[] { (12625, 1), (12626, 1) }, "voidchasm");
            Core.HuntMonster("voidchasm", "Carnage", "Voided Blood Sample", 1);
            Core.EnsureComplete(9551);
        }

        // Famiglia di Carcano 9552
        Story.KillQuest(9552, "voidchasm", "Carcano");
    }
}
