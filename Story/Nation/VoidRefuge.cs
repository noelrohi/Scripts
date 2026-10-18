/*
name: Void Refuge Story
description: This script will complete the storyline in /voidrefuge
tags: voidrefuge, refuge, nation, nulgath, story, ana di carcano, gravelyn
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class VoidRefuge
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
        if (Core.isCompletedBefore(9531))
            return;

        Story.PreLoad(this);

        // Newbie Boom (9522)
        Story.KillQuest(9522, "voidrefuge", "Lightguard Paladin");

        // Tears Like Honey (9523)
        Story.KillQuest(9523, "voidrefuge", "Paladin Ascendant");

        // Work Fiends (9524)
        if (!Story.QuestProgression(9524))
        {
            Core.EnsureAccept(9524);
            Core.GetMapItem(12572, 1, "voidrefuge");
            Core.HuntMonster("voidrefuge", "Nation Caster", "Void Engraving", 5);
            Core.EnsureComplete(9524);
        }

        // Rewritten Signatures (9525)
        Story.KillQuest(9525, "voidrefuge", "Nation Outrider");

        // Where the Heart is (9526)
        if (!Story.QuestProgression(9526))
        {
            Core.EnsureAccept(9526);
            Core.GetMapItem(12573, 1, "voidrefuge");
            Core.HuntMonster("voidrefuge", "Lightguard Paladin", "Paladin's Iudex Oath", 6);
            Core.HuntMonster("voidrefuge", "Paladin Ascendant", "Ascendant's Iudex Oath", 3);
            Core.EnsureComplete(9526);
        }

        // Rip Space (9527)
        if (!Story.QuestProgression(9527))
        {
            Core.EnsureAccept(9527);
            Core.GetMapItem(12574, 5, "voidrefuge");
            Core.HuntMonster("voidrefuge", "Nation Caster", "Void Tattoo", 3);
            Core.EnsureComplete(9527);
        }

        // Permanent Record (9528)
        Story.KillQuest(9528, "voidrefuge", "Nation Outrider");

        // Blood and Water (9529)
        Story.MapItemQuest(9529, "voidrefuge", new[] { 12575, 12576, 12577 });

        // Family Feud (9530)
        if (!Story.QuestProgression(9530))
        {
            Core.EnsureAccept(9530);
            Core.HuntMonster("voidrefuge", "Paladin Ascendant", "Shimmering Blood", 4);
            Core.HuntMonster("voidrefuge", "Nation Outrider", "Caustic Blood", 8);
            Core.EnsureComplete(9530);
        }

        // Twisted Helix (9531)
        Story.KillQuest(9531, "voidrefuge", "Carnage");
    }
}
