/*
name: Kathool Adept Class Prerequisites
description: This script will farm for Kathool Adept Prerequisites.
tags: ageofruin, kathool adept, fishing, somnia
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Story/AgeOfRuin/CoreAOR.cs
//cs_include Scripts/Story/ShadowsOfWar/CoreSoW.cs
//cs_include Scripts/Other/MergeShops/UndineCommissaryMerge.cs
//cs_include Scripts/Other/MergeShops/SeaviewSouvenirsMerge.cs
//cs_include Scripts/Other/MergeShops/TwilightZoneMerge.cs
//cs_include Scripts/Other/MergeShops/AbyssalZoneMerge.cs
//cs_include Scripts/Other/MergeShops/TrenchObserveMerge.cs
//cs_include Scripts/Other/MergeShops/SeaVoiceMerge.cs
//cs_include Scripts/Other/MergeShops/MidnightZoneMerge.cs
//cs_include Scripts/Farm/REP/FishingREP.cs
//cs_include Scripts/Farm/REP/SomniaREP.cs
using Skua.Core.Interfaces;

public class KathoolAdeptPrerequisites
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreAOR AOR { get => _AOR ??= new CoreAOR(); set => _AOR = value; }
    private static CoreAOR _AOR;
    private static CoreFarms Farm { get => _Farm ??= new CoreFarms(); set => _Farm = value; }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv { get => _Adv ??= new CoreAdvanced(); set => _Adv = value; }
    private static CoreAdvanced _Adv;
    private static CoreAdvanced sAdv { get => _sAdv ??= new CoreAdvanced(); set => _sAdv = value; }
    private static CoreAdvanced _sAdv;
	private static UndineCommissaryMerge UCM { get => _UCM ??= new UndineCommissaryMerge(); set => _UCM = value; }
    private static UndineCommissaryMerge _UCM;
    private static SeaviewSouvenirsMerge SSM { get => _SSM ??= new SeaviewSouvenirsMerge(); set => _SSM = value; }
    private static SeaviewSouvenirsMerge _SSM;
    private static TwilightZoneMerge TZM { get => _TZM ??= new TwilightZoneMerge(); set => _TZM = value; }
    private static TwilightZoneMerge _TZM;
	private static MidnightZoneMerge MZM { get => _MZM ??= new MidnightZoneMerge(); set => _MZM = value; }
    private static MidnightZoneMerge _MZM;
    private static AbyssalZoneMerge AZM { get => _AZM ??= new AbyssalZoneMerge(); set => _AZM = value; }
    private static AbyssalZoneMerge _AZM;
    private static TrenchObserveMerge TOM { get => _TOM ??= new TrenchObserveMerge(); set => _TOM = value; }
    private static TrenchObserveMerge _TOM;
    private static SeaVoiceMerge SVM { get => _SVM ??= new SeaVoiceMerge(); set => _SVM = value; }
    private static SeaVoiceMerge _SVM;

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();
		
		FarmPreReqs();
		Farm.FishingREP();
		Farm.SomniaREP();
        
        Core.SetOptions(false);
    }
	
	public void FarmPreReqs(bool BuyItems = false)
    {
        if (Core.CheckInventory("Adeptus Kathooli")){
			Core.Logger("Prerequisites already in inventory (Adeptus Kathooli)");
			return;
		}
		
		if (Core.CheckInventory("Ashray Villager")&&Core.CheckInventory("Undine Defence Director")&&Core.CheckInventory("Evacuation Protocol Suit")&&Core.CheckInventory("Elven Heritage Guardian")&&Core.CheckInventory("Ashray Elf Warden")&&Core.CheckInventory("DeepWater Drow")&&Core.CheckInventory("Midnight Glaucus Sage")&&Core.CheckInventory("Kathool Acolyte")){
			Core.Logger("Prerequisites already in inventory (Method 1)");
			return;
		}
		
		Core.Logger("Farming Prerequisites");
		
        SSM.BuyAllMerge("Ashray Villager");
		UCM.BuyAllMerge("Undine Defence Director");
		TZM.BuyAllMerge("Evacuation Protocol Suit");
		MZM.BuyAllMerge("Elven Heritage Guardian");
		AZM.BuyAllMerge("Ashray Elf Warden");
		TOM.BuyAllMerge("DeepWater Drow");
		
		Core.AddDrop("Glaucus Mystic");
		SVM.BuyAllMerge("Midnight Glaucus Sage");
		if (!Core.CheckInventory("Midnight Glaucus Sage")){
			SVM.BuyAllMerge("Midnight Glaucus Sage");
		}
		
        Core.EquipClass(ClassType.Solo);
		Core.HuntMonster("deepchaos", "Kathool", "Kathool Acolyte", 1, false, false);
    }
}