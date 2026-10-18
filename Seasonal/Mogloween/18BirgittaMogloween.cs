/*
name: BirgittaMogloween
description: Completes the quest chain in /birgittamogloween.
tags: birgittamogloween, story, quests, birgittamogloween
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Mogloween/CoreMogloween.cs
using Skua.Core.Interfaces;

public class BirgittaMogloween
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
	private static CoreMogloween CoreMogloween
    {
        get => _CoreMogloween ??= new CoreMogloween();
        set => _CoreMogloween = value;
    }
    private static CoreMogloween _CoreMogloween;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        CoreMogloween.BirgittaMogloween();

        Core.SetOptions(false);
    }
}
