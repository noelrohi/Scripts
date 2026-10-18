/*
name: Quibble Hunt Story
description: This will finish the Quibble Hunt Story.
tags: story, quest, quibble-hunt
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class QuibbleHunt
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

        StoryLine();

        Core.SetOptions(false);
    }

    public void StoryLine()
    {
        if (Core.isCompletedBefore(5891))
            return;

        Story.PreLoad(this);

        //Follow Those Fairies! 5878
        Story.KillQuest(5878, "quibblehunt", "Void Larva");

        //Aerodu Assault 5879
        if (!Story.QuestProgression(5879))
        {
            Core.EnsureAccept(5879);
            Core.HuntMonster("quibblehunt", "Aerodu Defense Machine", "Aerodux Crate", 6);
            Core.HuntMonster("quibblehunt", "Nimbo", "Ausome Package", 3);
            Core.EnsureComplete(5879);
        }

        //Void Travel Battle 5880
        Story.KillQuest(5880, "quibblehunt", "Void Elemental");

        //Lost to Entropy? 5881
        Story.KillQuest(5881, "quibblehunt", "Entropy Dragon");

        //Void Travel Battle 5882
        Story.KillQuest(5882, "quibblehunt", "Void Elemental");

        //The Master Quest 5883
        if (!Story.QuestProgression(5883))
        {
            Core.EnsureAccept(5883);
            Core.HuntMonster("quibblehunt", "Jimmy the Eye-Pod", "iChest", 3);
            Core.HuntMonster("quibblehunt", "Master Twang", "Combat Diploma", 1);
            Core.EnsureComplete(5883);
        }

        //Void Travel Battle 5884
        Story.KillQuest(5884, "quibblehunt", "Void Elemental");

        //Crystallize This 5885
        Story.KillQuest(5885, "quibblehunt", "RogueFiend");

        //Void Travel Battle 5886
        Story.KillQuest(5886, "quibblehunt", "Void Elemental");

        //Nauts and Double-crosses 5887
        if (!Story.QuestProgression(5887))
        {
            Core.EnsureAccept(5887);
            Core.HuntMonster("quibblehunt", "Braken Tentacle", "Soggy Note", 1);
            Core.HuntMonster("quibblehunt", "Jellyfish", "Nautican ShellCase", 8);
            Core.HuntMonster("quibblehunt", "Braken Tentacle", "Coral Capsule", 5);
            Core.EnsureComplete(5887);
        }

        //Void Travel Battle 5888
        Story.KillQuest(5888, "quibblehunt", "Void Elemental");

        //Key to the Mystery 5889
        Story.MapItemQuest(5889, "quibblehunt", 5311);

        //Get the Key 5890
        Story.KillQuest(5890, "quibblehunt", "Clawg");

        //A Void the Dricken 5891
        Story.KillQuest(5891, "quibblehunt", "Dricken");
    }
}
