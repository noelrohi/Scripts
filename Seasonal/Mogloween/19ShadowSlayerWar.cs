/*
name: Shadowslayerwar
description: Completes the quest chain in /shadowslayerwar.
tags: shadowslayerwar, story, quests, shadowslayerwar
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Mogloween/CoreMogloween.cs
using Skua.Core.Interfaces;

public class ShadowSlayerWar
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
		
		if (!Core.isCompletedBefore(10905)){
			CoreMogloween.BirgittaMogloween();
		}
		
        CoreMogloween.ShadowSlayerWar();

        Core.SetOptions(false);
    }
}
