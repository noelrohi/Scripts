/*
name: Walpurgisnacht
description: Completes the quest chain in /walpurgisnacht.
tags: walpurgisnacht, story, quests, walpurgisnacht
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Seasonal/Mogloween/CoreMogloween.cs
using Skua.Core.Interfaces;

public class Walpurgisnacht
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static CoreMogloween CoreMogloween
    {
        get => _CoreMogloween ??= new CoreMogloween();
        set => _CoreMogloween = value;
    }
    private static CoreMogloween _CoreMogloween;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();
        if (!Core.isCompletedBefore(10905)){
			CoreMogloween.BirgittaMogloween();
		}
		
		if (!Core.isCompletedBefore(10916)){
        CoreMogloween.ShadowSlayerWar();
		}
		
		CoreMogloween.Walpurgisnacht();
		
        Core.SetOptions(false);
    }
}
