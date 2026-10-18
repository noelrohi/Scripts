/*
name: Orb Hunt (Extra)
description: This will finish the Orb Hunt quest.
tags: story, quest, queen-of-monsters, orb-hunt, extra
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class OrbHunt
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

        OrbHuntSaga();

        Core.SetOptions(false);
    }

    public void OrbHuntSaga()
    {
        if (Core.isCompletedBefore(8349))
            return;

        Story.PreLoad(this);

        // 8302|Fallen Nopperabo
        Story.KillQuest(8302, "queenreign", "Samurai Nopperabo");

        // 8303|Samurai of Jaaku
        Story.KillQuest(8303, "queenreign", "Shadow Samurai");

        // 8304|Constructing the Portal
        if (!Story.QuestProgression(8304))
        {
            Core.EnsureAccept(8304);
            Core.HuntMonster("queenreign", "Samurai Nopperabo", "Yokai Energy", 3);
            Core.HuntMonster("queenreign", "Shadow Samurai", "Shadow Energy", 3);
            Story.MapItemQuest(8304, "queenreign", 9120);
        }
        // 8305|Into the Yokai Realm
        Story.KillQuest(8305, "queenreign", "Tsukumo-Gami");

        // 8306|Jaaku's Shadow
        Story.KillQuest(8306, "queenreign", "Jaaku's Shadow");

        // 8307|The Forces of Jaaku
        Story.KillQuest(8307, "queenreign", "Tsukumo-Gami");

        // 8308|The Wind Orb
        Story.KillQuest(8308, "queenreign", "Jaaku");

        // 8309|Super Spreaders
        Story.KillQuest(8309, "queenreign", "Plague Spreader");

        // 8310|Sample Size
        Story.MapItemQuest(8310, "queenreign", 9121, 4);

        // 8311|Moss You Be This Nasty
        Story.KillQuest(8311, "queenreign", "Plaguemoss");

        // 8312|Infected With The Cure
        if (!Story.QuestProgression(8312))
        {
            Core.EnsureAccept(8312);
            Core.GetMapItem(9122, 4, "queenreign");
            Core.HuntMonster("queenreign", "Plaguemoss", "Cure Tested", 4);
            Core.EnsureComplete(8312);
        }

        // 8313|Hurtful Healing
        Story.KillQuest(8313, "queenreign", "Plaguemoss");

        // 8314|The Energy Orb
        Story.KillQuest(8314, "queenreign", "Extriki");

        // 8315|Amethite, Am I Right?
        Story.KillQuest(8315, "queenreign", "Calcified Amethite");

        // 8316|Reinforcements Deployed
        Story.MapItemQuest(8316, "queenreign", 9123, 4);

        // 8317|Wyrms Below
        Story.KillQuest(8317, "queenreign", "Calcified Wyrm");

        // 8318|Lair Located
        if (!Story.QuestProgression(8318))
        {
            Core.EnsureAccept(8318);
            Core.GetMapItem(9124, 1, "queenreign");
            Core.HuntMonster("queenreign", "Calcified Remains", "Calcified Remains Defeated", 6);
            Core.EnsureComplete(8318);
        }

        // 8319|The Earth Orb
        Story.KillQuest(8319, "queenreign", "Grou'luu");

        // 8320|Goblin Down Water
        Story.KillQuest(8320, "queenreign", "Water Goblin");

        // 8321|Spawn of the Salt Sower
        Story.KillQuest(8321, "queenreign", "Sa-Laatan Spawn");

        // 8322|Faerie in Danger
        Story.KillQuest(8322, "queenreign", "Water Goblin");

        // 8323|Fertility Ward
        Story.MapItemQuest(8323, "queenreign", 9125, 5);

        // 8324|Path to Sa-Laatan
        Story.KillQuest(8324, "queenreign", "Water Goblin");

        // 8325|The Water Orb
        Story.KillQuest(8325, "queenreign", "Sa-Laatan");

        // 8326|Water. Earth. Energy? Air.
        if (!Story.QuestProgression(8326))
        {
            Core.EnsureAccept(8326);
            Core.HuntMonster("queenreign", "Sa-Laatan", "Sa-Lataan Defeated", 1);
            Core.HuntMonster("queenreign", "Grou'luu", "Grou'luu Defeated", 1);
            Core.HuntMonster("queenreign", "Extriki", "Extriki Defeated", 1);
            Core.HuntMonster("queenreign", "Jaaku", "Jaaku Defeated", 1);
            Core.EnsureComplete(8326);
        }

        // 8328|Plant Stalk-ers
        Story.KillQuest(8328, "orbhunt", "Seed Stalker");

        // 8329|Rage Against the Mach-Wing
        Story.KillQuest(8329, "orbhunt", "Ragewing");

        // 8330|Tower Wards
        Story.MapItemQuest(8330, "orbhunt", 9156, 4);
        Story.MapItemQuest(8330, "orbhunt", 9157, 4);

        // 8331|The Final Barrier
        Story.KillQuest(8331, "orbhunt", "Seed Stalker");

        // 8332|The Fire Orb
        Story.KillQuest(8332, "orbhunt", "Chamat");

        // 8333|Lotus Be Done With This
        Story.KillQuest(8333, "orbhunt", "Lotus Spider");

        // 8334|Lightless Light
        Story.KillQuest(8334, "orbhunt", "Suffocated Light");

        // 8335|Suffocation Investigation
        Story.MapItemQuest(8335, "orbhunt", 9158, 4);

        // 8336|Gone With the Djinn
        if (!Story.QuestProgression(8336))
        {
            Core.EnsureAccept(8336);
            Core.GetMapItem(9159, 1, "orbhunt");
            Core.HuntMonster("orbhunt", "Lotus Spider", "Djinn Energy Restored", 10);
            Core.EnsureComplete(8336);
        }

        // 8337|The Light Orb
        Story.KillQuest(8337, "orbhunt", "Sek Duat I");

        // 8338|The Light Orb, Part II
        Story.KillQuest(8338, "orbhunt", "Horothotep");

        // 8339|Nax-ty Beasts
        Story.KillQuest(8339, "orbhunt", "Nax Beast");

        // 8340|Children of the Night
        if (!Story.QuestProgression(8340))
        {
            Core.EnsureAccept(8340);
            Core.GetMapItem(9160, 4, "orbhunt");
            Core.HuntMonster("orbhunt", "Hive", "Battles Overturned", 8);
            Core.EnsureComplete(8340);
        }

        // 8341|We've Woken The Hive
        Story.KillQuest(8341, "orbhunt", "Hive");

        // 8342|Ritual Disruption
        if (!Story.QuestProgression(8342))
        {
            Core.EnsureAccept(8342);
            Core.GetMapItem(9161, 4, "orbhunt");
            Core.GetMapItem(9162, 1, "orbhunt");
            Core.HuntMonster("orbhunt", "Hive", "Minion Defeated", 10);
            Core.EnsureComplete(8342);
        }

        // 8343|The Darkness Orb
        Story.KillQuest(8343, "orbhunt", "Kolyaban");

        // 8344|The Infernal Cold
        Story.KillQuest(8344, "orbhunt", "Ice Infernal");

        // 8345|Light a Fire
        Story.MapItemQuest(8345, "orbhunt", 9163, 4);

        // 8346|We Ani-must Win
        Story.KillQuest(8346, "orbhunt", "Animus of Ice");

        // 8347|Bones For Stones
        if (!Story.QuestProgression(8347))
        {
            Core.EnsureAccept(8347);
            Core.HuntMonster("orbhunt", "Lotus Spider", "Bones of Light", 4);
            Core.HuntMonster("orbhunt", "Hive", "Bones of Monster", 4);
            Core.HuntMonster("orbhunt", "Ice Infernal", "Bones of Ice", 4);
            Core.HuntMonster("orbhunt", "Seed Stalker", "Bones of Mana", 4);
            Core.EnsureComplete(8347);
        }

        // 8348|The Ice Orb
        Story.KillQuest(8348, "orbhunt", "Quetzal");

        // 8349|A Hymn of Ice and Fire, and Light and Dark
        if (!Story.QuestProgression(8349))
        {
            Core.EnsureAccept(8349);
            Core.HuntMonster("orbhunt", "Chamat", "Chamat Defeated", 1);
            Core.HuntMonster("orbhunt", "Horothotep", "Horothotep Defeated", 1);
            Core.HuntMonster("orbhunt", "Kolyaban", "Kolyaban Defeated", 1);
            Core.HuntMonster("orbhunt", "Quetzal", "Quetzal Defeated", 1);
            Core.EnsureComplete(8349);
        }
    }
}
