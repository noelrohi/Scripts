/*
name: SambasFlag Story
description: This will finish the SambasFlag Story.
tags: story, quest, sambasflag
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class SambasFlag
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

        StoryLine();

        Core.SetOptions(false);
    }

    public void StoryLine()
    {
        if (Core.isCompletedBefore(9115) || !Core.isSeasonalMapActive("Sambaflag"))
            return;

        Story.PreLoad(this);

        // Bald Spots 9110
        if (!Story.QuestProgression(9110))
        {
            Core.EnsureAccept(9110);
            Core.HuntMonster("bloodtusk", "Rhison", "Rhison Fur", 8);
            Core.HuntMonster("bloodtusk", "Jungle Vulture", "Vulture Feathers", 8);
            Core.EnsureComplete(9110);
        }

        // Work Out Glow 9111
        if (!Story.QuestProgression(9111))
        {
            Core.EnsureAccept(9111);
            Core.HuntMonster("dwarfhold", "Gemrald", "Gem Sequin", 8);
            Core.HuntMonster("dwarfhold", "Glow Worm", "Glowing Residue", 8);
            Core.EnsureComplete(9111);
        }

        // Burned Forevermore 9112
        if (!Story.QuestProgression(9112))
        {
            Core.EnsureAccept(9112);
            Core.HuntMonster("fotia", "Femme Cult Worshiper", "Cult Veil", 8);
            Core.HuntMonster("fotia", "Fotia Spirit", "Fotia Spirit Dress", 8);
            Core.EnsureComplete(9112);
        }

        // Calm Sea and Prosperous Voyage 9113
        if (!Story.QuestProgression(9113))
        {
            Core.EnsureAccept(9113);
            Core.HuntMonster("shipwreck", "Gilded Merdraconian", "Gilded Draconia Scale", 8);
            Core.HuntMonster("shipwreck", "Gilded Crystal Undead", "Gold Crystals", 8);
            Core.EnsureComplete(9113);
        }

        // Chaos Cleanse 9114
        if (!Story.QuestProgression(9114))
        {
            Core.EnsureAccept(9114);
            Core.HuntMonster("falguard", "Chaonslaught Cavalry", "Chaos Cavalry Defeated", 8);
            Core.HuntMonster("falguard", "Chaonslaught Caster", "Chaos Caster Defeated", 8);
            Core.EnsureComplete(9114);
        }

        // Fly the Standard 9115
        if (!Story.QuestProgression(9115))
        {
            Core.EnsureAccept(9115);
            Core.HuntMonster("sambaflag", "Flag Bearer", "Flag Standard", 1);
            Core.HuntMonster("sambaflag", "Master of Ceremonies", "Ceremony Feather", 1);
            Core.EnsureComplete(9115);
        }
    }
}
