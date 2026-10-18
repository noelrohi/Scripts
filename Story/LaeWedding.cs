/*
name: Lae Wedding Story
description: This will finish the Lae Wedding Story.
tags: story, quest, lae-wedding,they,lae,wedding,laewed
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class LaeWedding
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreStory Story
    {
        get => _Story ??= new CoreStory();
        set => _Story = value;
    }
    private static CoreStory _Story;
    public string[] UseableMonsters { get; private set; }

    public LaeWedding()
    {
        UseableMonsters = new[]
        {
            "Possessed Rat", // UseableMonsters[0],
            "Possessed Spider", // UseableMonsters[1],
            "Vengeful Spirit", // UseableMonsters[2],
            "Wrathful Spirit", // UseableMonsters[3],
            "Haunted Closet", // UseableMonsters[4],
            "Blood Witch", // UseableMonsters[5]
        };
    }

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        Storyline();

        Core.SetOptions(false);
    }

    public void Storyline()
    {
        if (Core.isCompletedBefore(9953))
            return;

        Story.PreLoad(this);

        // 9944 | Wedding Reception
        Story.MapItemQuest(9944, "laewed", new[] { 13769, 13770 });

        // 9945 | Rats in the Walls
        if (!Story.QuestProgression(9945))
        {
            Core.HuntMonsterQuest(9945, ("laewed", UseableMonsters[0], ClassType.Farm));
        }

        // 9946 | Blissfully Unaware
        Story.MapItemQuest(9946, "laewed", new[] { 13771, 13772 });

        // 9947 | Caspar's Web
        if (!Story.QuestProgression(9947))
        {
            Core.HuntMonsterQuest(9947, ("laewed", UseableMonsters[1], ClassType.Farm));
        }

        // 9948 | Terrorized Tailors
        Story.MapItemQuest(9948, "laewed", new[] { 13773, 13774 });

        // 9949 | Occult Objection
        if (!Story.QuestProgression(9949))
        {
            Core.EnsureAccept(9949);
            Core.GetMapItem(13775, 1, "laewed");
            Core.HuntMonster("laewed", "Vengeful Spirit", "Vengeful Ectoplasm", 8);
            Core.EnsureComplete(9949);
        }

        // 9950 | Wraithful
        if (!Story.QuestProgression(9950))
        {
            Core.EnsureAccept(9950);
            Core.GetMapItem(13776, 1, "laewed");
            Core.HuntMonster("laewed", "Wrathful Spirit", "Wrathful Ectoplasm", 6);
            Core.EnsureComplete(9950);
        }

        // 9951 | Old, Blue, and…?
        if (!Story.QuestProgression(9951))
        {
            Core.HuntMonsterQuest(9951, ("laewed", UseableMonsters[4], ClassType.Solo));
        }

        // 9952 | Aisle Rampage
        if (!Story.QuestProgression(9952))
        {
            Core.HuntMonsterQuest(
                9952,
                ("laewed", UseableMonsters[2], ClassType.Farm),
                ("laewed", UseableMonsters[3], ClassType.Farm)
            );
        }

        // 9953 | Blood-Soaked Witchery
        if (!Story.QuestProgression(9953))
        {
            Core.HuntMonsterQuest(9953, ("laewed", UseableMonsters[5], ClassType.Solo));
        }
    }
}
