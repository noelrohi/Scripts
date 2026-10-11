/*
name: Restore Ultra Loadout
description: Puts back the gear and class enhancement an Ultra Loadout changed on an account with auto enhance off, after a run stopped before it could.
tags: ultra, loadout, restore, enhancement, gear
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPotions.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/GetScrolls.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraAsync.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPartyLayout.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraWaitForArmy.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraComp.cs
using Skua.Core.Interfaces;

public class RestoreLoadout
{
    private CoreBots Core => CoreBots.Instance;

    public void ScriptMain(IScriptInterface Bot)
    {
        Core.SetOptions();
        Bot.Bank.Load();
        UltraLoadout.Restore();
        Core.SetOptions(false);
    }
}
