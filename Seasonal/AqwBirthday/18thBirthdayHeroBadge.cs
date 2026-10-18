/*
name: 18th Birthday Hero Character Page Badge
description: This script will get the "Birthday Hero of DOOM" Character Page Badge.
tags: birthday, badge, anniversary, gifts, quest, seasonal
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
using Skua.Core.Interfaces;

public class BirthdayHeroBadge
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

        Badge();
        Core.SetOptions(false);
    }

    public void Badge()
    {
        if (Core.HasWebBadge(badge))
        {
            Core.Logger($"Already have the {badge} badge");
            return;
        }

        Core.EquipClass(ClassType.Solo);

        //18th Anniversary Gifts
        if (!Story.QuestProgression(6554))
        {
            Core.EnsureAccept(6554);
            Core.HuntMonster("birthday", "Birthday Cake", "Top Cherry", 1);
            Core.HuntMonster("birthday", "Birthday Cake", "Copper Knife", 1);
            Core.HuntMonster("birthday", "Birthday Cake", "Platinum Fork", 1);
            Core.HuntMonster("birthday", "Birthday Cake", "Gold Spoon", 1);
            Core.EnsureComplete(6554);
        }
    }

    private readonly string badge = "Birthday Hero of DOOM";
}
