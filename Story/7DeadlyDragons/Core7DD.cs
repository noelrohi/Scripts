/*
name: null
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class Core7DD
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
        Core.RunCore();
    }

    public void Complete7DD()
    {
        Gluttony();
        Pride();
        Greed();
        Sloth();
        Lust();
        Envy();
        Wrath();
    }

    #region Gluttony
    public void Gluttony()
    {
        if (Core.isCompletedBefore(5915))
            return;
        Core.Logger("Hunting the Fire Dragon of Gluttony!");

        Story.PreLoad(this);

        // Guts for Glutes 5903
        Story.KillQuest(5903, "Gluttony", "Glutus");

        // Mucus be Joking 5904
        Story.KillQuest(5904, "Gluttony", "Mucus");

        // Build a Bone-a-fied Ladder 5905]
        if (!Story.QuestProgression(5905))
        {
            Core.EnsureAccept(5905);
            Core.GetMapItem(5346, 2, "Gluttony");
            Core.HuntMonster("gluttony", "Skeletal Slayer", "Nice Long Bones", 10);
            Core.EnsureComplete(5905);
        }

        // Talk About Reflux 5906
        Story.MapItemQuest(5906, "Gluttony", 5344);

        // How'd HE get here? 5907
        Story.MapItemQuest(5907, "Gluttony", 5345);

        // Glowworms not Glowsticks 5908
        if (!Story.QuestProgression(5908))
        {
            Core.EnsureAccept(5908);
            Core.GetMapItem(5347, 10, "Gluttony");
            Core.KillMonster("Gluttony", "r7", "Left", "Bowel Worm", "Agitated Bowel Worm", 10);
            Core.EnsureComplete(5908);
        }

        // Ossification Needed 5909
        if (!Story.QuestProgression(5909))
        {
            Core.EnsureAccept(5909);
            Core.KillMonster("Gluttony", "r6", "Left", "Bile", "Falgar's Bones", 9);
            Core.EnsureComplete(5909);
        }

        // Eye Need Bones 5910
        if (!Story.QuestProgression(5910))
        {
            Core.EnsureAccept(5910);
            Core.GetMapItem(5348, 5, "Gluttony");
            Core.HuntMonster("gluttony", "Skeletal Slayer", "Replacement Eyes", 2);
            Core.EnsureComplete(5910);
        }

        // Bile Burns 5911
        if (!Story.QuestProgression(5911))
        {
            Core.EnsureAccept(5911);
            Core.HuntMonster("gluttony", "Bowel Worm", "Bowel Worm Slain", 5);
            Core.HuntMonster("gluttony", "Bile", "Bile Slain", 5);
            Core.EnsureComplete(5911);
        }

        // Find the Chest 5912
        Story.MapItemQuest(5912, "Gluttony", 5349);

        // We Need the Key 5913
        Story.KillQuest(5913, "Gluttony", "Giant Tapeworm");

        // Cha Cha Cha 5914
        Story.MapItemQuest(5914, "Gluttony", 5350);

        // Glutus, Take 2 5915
        Story.KillQuest(5915, "Gluttony", "Deflated Glutus");
    }
    #endregion Gluttony

    #region Pride
    public void Pride()
    {
        if (Core.isCompletedBefore(5926))
            return;

        Core.Logger("Hunting the Energy Dragon of Pride!");

        Story.PreLoad(this);

        // Defeat the Drakel 5917
        Story.KillQuest(5917, "Pride", "Storm Drakel");

        // Don’t Get Zapped! 5918
        Story.KillQuest(5918, "Pride", "Ball Lightning");

        // Get Grounded 5919
        Story.KillQuest(5919, "Pride", "Rubber Treeant");

        // Get the Key 5920
        Story.KillQuest(5920, "Pride", "Cellar Guard");

        // Get the Boots 5921
        Story.MapItemQuest(5921, "Pride", 5351);
        Story.MapItemQuest(5921, "Pride", 5352, 6);

        // Make Lightning Rods 5922
        if (!Story.QuestProgression(5922))
        {
            Core.EnsureAccept(5922);
            Core.GetMapItem(5353, 8, "Pride");
            Core.HuntMonster("pride", "Storm Drakel", "Electric Prod", 8);
            Core.EnsureComplete(5922);
        }

        // Free the Villagers 5923
        if (!Story.QuestProgression(5923))
        {
            Core.EnsureAccept(5923);
            Core.GetMapItem(5354, 4, "Pride");
            Core.HuntMonster("pride", "Drakel Guard", "Drakel Key", 4);
            Core.EnsureComplete(5923);
        }

        // Open the Gate 5924
        if (!Story.QuestProgression(5924))
        {
            Core.EnsureAccept(5924);
            Core.GetMapItem(5355, 1, "Pride");
            Core.HuntMonster("pride", "Elite Guard", "Gate Key", 1);
            Core.EnsureComplete(5924);
        }

        // Fight Your Way Throug 5925
        if (!Story.QuestProgression(5925))
        {
            Core.EnsureAccept(5925);
            Core.GetMapItem(5356, 1, "Pride");
            Core.HuntMonster("pride", "Storm Drakel", "Storm Drakel Slain", 12);
            Core.EnsureComplete(5925);
        }

        // Defeat Valsarian 5926
        Story.KillQuest(5926, "Pride", "Valsarian");
    }
    #endregion Pride

    #region Greed
    public void Greed()
    {
        if (Core.isCompletedBefore(5943))
            return;

        Core.Logger("Hunting the Ice Dragon of Greed!");

        Story.PreLoad(this);

        // Looting is for Sneevils 5934
        Story.KillQuest(5934, "Greed", "Sneevil Looter");

        // Jumping in Puddles 5935
        Story.MapItemQuest(5935, "Greed", 5372, AutoCompleteQuest: false);

        // Pick the Right Chest 5936
        Story.MapItemQuest(5936, "Greed", 5373);

        // Gelatin is Nasty 5937
        Story.KillQuest(5937, "Greed", "Jelly-Like Cube");

        // Explore the Cave 5938
        Story.MapItemQuest(5938, "Greed", 5374, 5);

        // Disarm the Trap 5939
        Story.MapItemQuest(5939, "Greed", new[] { 5377, 5378 });

        // Crystal-eyes 5940
        if (!Story.QuestProgression(5940))
        {
            Core.EnsureAccept(5940);
            Core.HuntMonster("greed", "Ice Crystal", "Ice Crystal Slain", 5);
            Core.HuntMonster("greed", "Glacial Horror", "Glacial Horror Slain", 10);
            Core.EnsureComplete(5940);
        }

        // Go-bolds? 5941
        if (!Story.QuestProgression(5941))
        {
            Core.EnsureAccept(5941);
            Core.GetMapItem(5375, 5, "Greed");
            Core.HuntMonster("greed", "Kobold", "Recovered Gold", 10);
            Core.EnsureComplete(5941);
        }

        // Follow the Trail 5942
        Story.MapItemQuest(5942, "Greed", 5376, 3);

        // Get Rid of Greed 5943
        Story.KillQuest(5943, "Greed", "Goregold");
    }
    #endregion Greed

    #region Sloth
    public void Sloth()
    {
        if (Core.isCompletedBefore(5959))
            return;

        Core.Logger("Hunting the Slime Dragon of Sloth!");

        Story.PreLoad(this);

        HazMatSuit();

        // Are There Any Survivors? 5945
        if (!Story.QuestProgression(5945))
        {
            Core.EnsureAccept(5945);
            Core.GetMapItem(5382, 1, "Sloth");
            Core.HuntMonster("sloth", "Plague Zombie", "Plague Zombie Defeated", 10);
            Core.EnsureComplete(5945);
        }

        // Gathering Samples 5946
        if (!Story.QuestProgression(5946))
        {
            Core.EnsureAccept(5946);
            Core.HuntMonster("sloth", "SnotGoblin", "Snot Sample", 6);
            Core.HuntMonster("sloth", "Wandering Plague", "Plague Sample", 6);
            Core.EnsureComplete(5946);
        }

        // Find a ‘Volunteer’ 5947
        Story.KillQuest(5947, "Sloth", "Plague Zombie");

        // Cure the Volunteer 5948
        Story.MapItemQuest(5948, "Sloth", 5387);

        // Herbal Help 5949
        Story.KillQuest(5949, "Sloth", "Marsh Thing");

        // Re-heal 5950
        Story.MapItemQuest(5950, "Sloth", 5383, 8);

        // Let's Try That Again 5951
        Story.MapItemQuest(5951, "Sloth", 5389);

        // One More Time 5952
        if (!Story.QuestProgression(5952))
        {
            Core.EnsureAccept(5952);
            Core.BuyItem("Dragonhame", 865, "Airther Vitae");
            Core.BuyItem("embersea", 1100, 1749);
            Core.EnsureComplete(5952);
        }

        // Who’s Up for Round 3 5953
        Story.MapItemQuest(5953, "Sloth", 5391);

        // Cure the Villagers 5954
        Story.KillQuest(5954, "Sloth", "Plague Zombie");

        // Gotta Clean Up 5955
        Story.MapItemQuest(5955, "Sloth", 5384, 10);

        // Clear the Castellum 5956
        if (!Story.QuestProgression(5956))
        {
            Core.EnsureAccept(5956);
            Core.GetMapItem(5385, 1, "Sloth");
            Core.HuntMonster("sloth", "SnotGoblin Prime", "Snotgoblin Prime Defeated", 1);
            Core.EnsureComplete(5956);
        }

        // Get rid of Phlegnn 5957
        Story.KillQuest(5957, "Sloth", "Phlegnn");

        // Cured is NOT GOOD 5958
        Story.KillQuest(5958, "Sloth", "Cured Phlegnn");

        // Actual Sloth Dragon 5960
        Story.KillQuest(5960, "Sloth", "Actual Sloth Dragon");

        // Mutated Plague 5959
        Story.KillQuest(5959, "Sloth", "Mutated Plague");
    }
    #endregion Sloth

    #region Lust
    public void Lust()
    {
        if (Core.isCompletedBefore(5976))
            return;

        Core.Logger("Hunting the Dragon of Lust!");

        Story.PreLoad(this);

        // Hopelessly Devoted 5961
        Story.KillQuest(5961, "Lust", "Devoted Admirer");

        // Fighting the Feeling 5962
        Story.MapItemQuest(5962, "Lust", new[] { 5405, 5406, 5407, 5408, 5409 });

        // Love Potion #9.1 5963
        Story.KillQuest(5963, "Lust", "Golden Vase");

        // Leather is Better 5964
        Story.KillQuest(5964, "Lust", "Enamored Guard");

        // Gird your Loins 5965
        if (!Story.QuestProgression(5965))
        {
            Core.EnsureAccept(5965);
            Core.GetMapItems(new[] { (5410, 1), (5411, 1) }, "Lust");
            Core.HuntMonster("lust", "Devoted Admirer", "Spell Broken", 8);
            Core.EnsureComplete(5965);
        }

        // Get the Keys 5966
        Story.KillQuest(5966, "Lust", "Enamored Guard");

        // Open the Cages 5967
        Story.MapItemQuest(5967, "Lust", 5412, 5);

        // Get Past the Guards 5968
        if (!Story.QuestProgression(5968))
        {
            Core.EnsureAccept(5968);
            Core.GetMapItem(5413, 1, "Lust");
            Core.HuntMonster("lust", "Elite Guard", "Elite Guards Defeated", 9);
            Core.EnsureComplete(5968);
        }

        // Viscyra’lly Yours 5969
        if (!Story.QuestProgression(5969))
        {
            Core.EnsureAccept(5969);
            Core.GetMapItem(5414, 1, "Lust");
            Core.HuntMonster("lust", "Viscyra", "Viscyra's Key", 1);
            Core.EnsureComplete(5969);
        }

        // Lascivia’sness 5970
        Story.KillQuest(5970, "Lust", "Lascivia");

        // Talk to the Guards 5971
        Story.KillQuest(5971, "Lust", "Elite Guard");

        // Lascivia Stunk Pretty 5972
        if (!Story.QuestProgression(5972))
        {
            Core.EnsureAccept(5972);
            Core.HuntMonster("lust", "Golden Vase", "Gold Vase", 1);
            Core.HuntMonster("lust", "Golden Vase", "Lascivia's \"Smell-Good\"", 5);
            Core.EnsureComplete(5972);
        }

        // You Broke It, You Fix It 5973
        Story.KillQuest(5973, "Lust", "Devoted Admirer");

        // Elite Energy 5974
        if (!Story.QuestProgression(5974))
        {
            Core.EnsureAccept(5974);
            Core.GetMapItem(5415, 8, "Lust");
            Core.HuntMonster("lust", "Elite Guard", "Special Energy Drink", 3);
            Core.EnsureComplete(5974);
        }

        // No Pillow Unturned 5975
        Story.MapItemQuest(5975, "Lust", new[] { 5416, 5417 });

        // Take Down Killek 5976
        Story.KillQuest(5976, "Lust", "Killek Deadchewer");
    }
    #endregion Lust

    #region Envy
    public void Envy()
    {
        if (Core.isCompletedBefore(6005))
            return;

        Core.Logger("Hunting the Darkness Dragon of Envy!");

        Story.PreLoad(this);

        // Talk to the Guard 5983
        Story.MapItemQuest(5983, "DragonCrown", 5420);

        // Talk to the Villager 5984
        Story.MapItemQuest(5984, "DragonCrown", 5421);

        // Fire and Ice 5985
        if (!Story.QuestProgression(5985))
        {
            Core.EnsureAccept(5985);
            Core.GetMapItem(5422, 6, "DragonCrown");
            Core.HuntMonster("dragoncrown", "Fire Sprite", "Fire Sprites Slain", 6);
            Core.EnsureComplete(5985);
        }

        // Round ‘em Up 5986
        if (!Story.QuestProgression(5986))
        {
            Core.EnsureAccept(5986);
            Core.HuntMonster("dragoncrown", "Llama", "Llamas Rustled", 5);
            Core.HuntMonster("dragoncrown", "Rampaging Boar", "Boars Rustled", 5);
            Core.EnsureComplete(5986);
        }

        // Get Rockin’ 5987
        if (!Story.QuestProgression(5987))
        {
            Core.EnsureAccept(5987);
            Core.HuntMonster("dragoncrown", "Rock Elemental", "Strong Rocks", 10);
            Core.HuntMonster("dragoncrown", "Earth Elemental", "Sticky Mud", 5);
            Core.EnsureComplete(5987);
        }

        // Stop Fightin’ 5988
        Story.MapItemQuest(5988, "DragonCrown", 5423);

        // Find Riddrug 5989
        Story.MapItemQuest(5989, "DragonCrown", 5424);

        // Defeat The Red Champion 5990
        Story.KillQuest(5990, "DragonCrown", "Torgat");

        // Defeat The Ice Champion 5991
        Story.KillQuest(5991, "DragonCrown", "Fressa");

        // Defeat The Undead Champion 5992
        Story.KillQuest(5992, "DragonCrown", "Radroth");

        // Defeat The Water Champion 5993
        Story.KillQuest(5993, "DragonCrown", "Nizex");

        // Defeat The Wind Champion 5994
        Story.KillQuest(5994, "DragonCrown", "Tathu");

        // Defeat The Yokai Champion 5995
        Story.KillQuest(5995, "DragonCrown", "Lanshen");

        // Defeat The Green Champion 5996
        Story.KillQuest(5996, "DragonCrown", "Ashax");

        // Defeat The Faerie Champion 5997
        Story.KillQuest(5997, "DragonCrown", "Letori");

        // Defeat The Chaos Champion 5998
        Story.KillQuest(5998, "DragonCrown", "Nayzol");

        // Defeat the Void Champion 5999
        Story.KillQuest(5999, "DragonCrown", "Zathas");

        // Argo’s Not Stopping Us! 6000
        Bot.Log("please wait whilst it hunts out of the cutscene cell...");
        Story.KillQuest(6000, "DragonCrown", "Argo");

        // Defeat Ukki, The 1st Sentinel 6001
        Story.KillQuest(6001, "Maloth", "Ukki");

        // Defeat Kagan, The 2nd Guardian 6002
        Story.KillQuest(6002, "Maloth", "Kagan");

        // Defeat Golgar, The 3rd Guardian 6003
        Story.KillQuest(6003, "Maloth", "Golgar");

        // Hand over the Entry Code 6004
        if (!Story.QuestProgression(6004))
        {
            Core.EnsureAccept(6004);
            Core.HuntMonster("maloth", "Golgar", "Final Word", 1);
            Core.HuntMonster("maloth", "Castle Guard", "First Word", 1);
            Core.HuntMonster("maloth", "Scroll Keeper", "Second Word", 1);
            Core.HuntMonster("maloth", "Nervous Serf", "Third Word", 1);
            Core.HuntMonster("maloth", "Locked Chest", "Fourth Word", 1);
            Core.EnsureComplete(6004);
        }

        // Confront the Dragon King 6005
        Story.KillQuest(6005, "Maloth", "Maloth");
    }
    #endregion Envy

    #region Wrath
    public void Wrath()
    {
        if (Core.isCompletedBefore(6170))
            return;

        Core.Logger("Hunting the Dracolich of Wrath!");

        Story.PreLoad(this);

        // Decimate the Horde 6110
        if (!Story.QuestProgression(6110))
        {
            Core.EnsureAccept(6110);
            Core.HuntMonster("wrath", "Fishbones", "Fishbones Slain", 5);
            Core.HuntMonster("wrath", "Bone Terror", "Bone Terror Slain", 5);
            Core.EnsureComplete(6110);
        }

        // Douse with Flames 6111
        Story.KillQuest(6111, "Wrath", "Dark Fire");

        // Grenades of AWE 6112
        if (!Story.QuestProgression(6112))
        {
            Core.EnsureAccept(6112);
            Core.GetMapItem(5541, 6, "Wrath");
            Core.HuntMonster("wrath", "Bone Terror", "Bone Shrapnel", 6);
            Core.EnsureComplete(6112);
        }

        // The Final Ingredient 6113
        if (!Story.QuestProgression(6113))
        {
            if (!Core.CheckInventory("Holy Wasabi"))
            {
                Core.AddDrop("Holy Wasabi");
                Core.EnsureAccept(1075);

                Core.EquipClass(ClassType.Farm);
                Core.HuntMonster("doomwood", "Doomwood Ectomancer", "Dried Wasabi Powder", 4);
                Core.GetMapItem(428, 1, "lightguard");
                Core.Join("lightguard");

                Core.EnsureComplete(1075);
                Bot.Wait.ForPickup("Holy Wasabi");
            }
            Core.EnsureComplete(6113);
        }

        // Count Not to Four 6114
        Story.MapItemQuest(6114, "Wrath", 5542, 6);

        // Talk to Captain Rhubarb 6115
        Story.MapItemQuest(6115, "Wrath", 5540);

        // Find the Manifest 6116
        if (!Story.QuestProgression(6116))
        {
            Core.EnsureAccept(6116);
            Core.GetMapItem(5544, 1, "Wrath");
            Core.HuntMonster("wrath", "Undead Pirate", "Undead Pirates Slain", 8);
            Core.EnsureComplete(6116);
        }

        // Get the Cargo Hold Key 6117
        if (!Story.QuestProgression(6117))
        {
            Core.EnsureAccept(6117);
            Core.HuntMonster("wrath", "Mutineer", "Mutineers Slain", 8);
            Core.HuntMonster("wrath", "Mutineer", "Cargo Hold Key", 1);
            Core.EnsureComplete(6117);
        }

        // Find the Jewel 6118
        Story.MapItemQuest(6118, "Wrath", 5545);

        // Defeat Droghor the Screecher! 6119
        Story.KillQuest(6119, "Wrath", "Droghor");

        // Defeat Gorhorath! 6120
        Story.KillQuest(6120, "Wrath", "Gorgorath");

        // Navigate the Maze 6163
        if (!Story.QuestProgression(6163))
        {
            Core.EnsureAccept(6163);
            Core.GetMapItem(5587, 1, "Dragonbone");
            Core.HuntMonster("dragonbone", "Bone Dragonling", "Bone Dragonlings Slain", 5);
            Core.HuntMonster("dragonbone", "Dark Fire", "Dark Fire Extinguished", 5);
            Core.EnsureComplete(6163);
        }

        // Get a Clue 6164
        Story.KillQuest(6164, "Dragonbone", "Bone Terror");

        // Heart of a Dracolich 6165
        Story.KillQuest(6165, "Dragonbone", "Bone Wyvern");

        // Open the Door 6166
        Story.MapItemQuest(6166, "Dragonbone", 5588);

        // Find the Jewel 6167
        Story.MapItemQuest(6167, "Dragonbone", 5589);

        // Get to the Lair 6168
        Story.MapItemQuest(6168, "Dragonbone", 5590);

        // Unlock the Door 6169
        if (!Story.QuestProgression(6169))
        {
            Core.EnsureAccept(6169);
            Core.GetMapItem(5591, 1, "Dragonbone");
            Core.HuntMonster("dragonbone", "DragonShade", "Key to the Lair", 1);
            Core.EnsureComplete(6169);
        }

        // Slay Gorgorath 6170
        Story.KillQuest(6170, "Dragonbone", "Gorgorath");
    }
    #endregion Wrath

    #region Hazmat Suit for Sloth
    public void HazMatSuit()
    {
        if (!Bot.TempInv.Contains(40710))
        {
            Core.Logger("Suit not found");
            Core.EnsureAccept(5944);
            Core.GetMapItems(new[] { (5381, 1), (5380, 1) }, "Sloth");
            Core.EnsureComplete(5944);
            Bot.Wait.ForDrop("Hazmat Suit (Temp)");
            Bot.Wait.ForPickup("Hazmat Suit (Temp)");
            Bot.Sleep(2500);
        }
        else
        {
            Core.Logger("Suit found in Temp Inventory");
            Bot.Send.Packet($"%xt%zm%equipItem%{Bot.Map.RoomID}%40710%");
            Bot.Sleep(2500);
        }
    }
    #endregion Hazmat Suit for Sloth
}
