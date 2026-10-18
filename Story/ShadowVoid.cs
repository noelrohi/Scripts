/*
name: Shadow Void Story
description: This will finish the ShadowVoid Story.
tags: story, quest, shadow-void
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs

using Skua.Core.Interfaces;

public class ShadowVoid
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

        ShadowVoidQuests();

        Core.SetOptions(false);
    }

    public void ShadowVoidQuests()
    {
        if (!Bot.Quests.IsUnlocked(7123))
        {
            Core.Logger("Quests are locked You Should Finish Lords Of Chaos story script first");
            return;
        }
        if (Core.isCompletedBefore(7131))
        {
            Core.Logger("StoryLine Complete:   Shadowvoid");
            return;
        }

        Story.PreLoad(this);

        //Pieces of Light 7123
        Story.KillQuest(7123, "ShadowVoid", "Light Matter");

        //Cloak of Silk 7124
        Story.KillQuest(7124, "ShadowVoid", "Shadowsprite");

        //Bridge of Shadows 7125
        Story.KillQuest(7125, "ShadowVoid", "Light Matter");

        //Clear the Shadowfiends 7126
        Story.KillQuest(7126, "ShadowVoid", "Shadowfiend");

        //Feed the Doomblade 7127
        Story.KillQuest(7127, "ShadowVoid", "Shadowfiend");

        //Build Another Bridge 7128
        if (!Story.QuestProgression(7128))
        {
            Core.EnsureAccept(7128);
            Core.HuntMonster("shadowvoid", "Shadowfiend", "Shadow Matter", 6);
            Core.HuntMonster("shadowvoid", "Light Matter", "Bright Oil", 5);
            Core.EnsureComplete(7128);
        }

        //Find Safiria 7129
        Story.MapItemQuest(7129, "ShadowVoid", 6765);

        //Dissolve the Chains 7130
        if (!Story.QuestProgression(7130))
        {
            Core.EnsureAccept(7130);
            Core.GetMapItem(6766, 1, "ShadowVoid");
            Core.HuntMonster("shadowvoid", "Light Matter", "Bright Acid", 3);
            Core.EnsureComplete(7130);
        }

        //Fragment of Doom 7131
        Story.KillQuest(7131, "ShadowVoid", "Fragment of Doom");
    }
}
