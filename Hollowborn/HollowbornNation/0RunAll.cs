/*
name: Hollowborn Archfiend - Run All
description: does all of Hollowborn Archfiend
tags: hollowborn, hollowborn archfiend, hollowbornarchfiend, hollowborn nation, do all, run all
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
//cs_include Scripts/CoreDailies.cs
//cs_include Scripts/CoreStory.cs
//cs_include Scripts/Nation/CoreNation.cs
//cs_include Scripts/Nation/Various/JuggernautItems.cs
//cs_include Scripts/Hollowborn/CoreHollowborn.cs
//cs_include Scripts/Hollowborn/Materials/HollowSoul.cs
//cs_include Scripts/Hollowborn/HollowbornOblivionBlade.cs
//cs_include Scripts/Nation/Various/ShadowLegacyofNulgath.cs
//cs_include Scripts/Nation/AFDL/WillpowerExtraction.cs
//cs_include Scripts/Nation/Various/GoldenHanzoVoid.cs
//cs_include Scripts/Nation/AFDL/NulgathDemandsWork.cs
//cs_include Scripts/Nation/VHL/CoreVHL.cs
//cs_include Scripts/Good/BLOD/CoreBLOD.cs
//cs_include Scripts/Story/BattleUnder.cs
//cs_include Scripts/Nation/Various/EnchantedNulgathNationHouse.cs
//cs_include Scripts/Hollowborn/HollowbornNation/1InTheFiendsShadow.cs
//cs_include Scripts/Hollowborn/HollowbornNation/2FiendsPurgatory.cs
//cs_include Scripts/Hollowborn/HollowbornNation/3MuddledPast.cs
//cs_include Scripts/Hollowborn/HollowbornNation/4TugandPull.cs
//cs_include Scripts/Hollowborn/HollowbornNation/5RolesUnreversed.cs
using Skua.Core.Interfaces;
using Skua.Core.Options;

public class HBA0
{
    private IScriptInterface Bot => IScriptInterface.Instance;
    private CoreBots Core => CoreBots.Instance;
    private static InTheFiendsShadow ITFS
    {
        get => _ITFS ??= new InTheFiendsShadow();
        set => _ITFS = value;
    }
    private static InTheFiendsShadow _ITFS;
    private static FiendsPurgatory FP
    {
        get => _FP ??= new FiendsPurgatory();
        set => _FP = value;
    }
    private static FiendsPurgatory _FP;
    private static MuddledPast MP
    {
        get => _MP ??= new MuddledPast();
        set => _MP = value;
    }
    private static MuddledPast _MP;
    private static TugandPull TaP
    {
        get => _TaP ??= new TugandPull();
        set => _TaP = value;
    }
    private static TugandPull _TaP;
    private static RolesUnreversed RU
    {
        get => _RU ??= new RolesUnreversed();
        set => _RU = value;
    }
    private static RolesUnreversed _RU;

    public string OptionsStorage = "HollowbornArchfiend";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        new Option<bool>(
            "getAll",
            "Get all items",
            "If true, each quest with selectable rewards will be completed until all rewards are collected. "
                + "If false, use the reward selection for each quest.",
            true
        ),
        CoreBots.Instance.SkipOptions,
        new Option<InTheFiendsShadow.Rewards>(
            "In The Fiend's Shadow",
            "In The Fiend's Shadow Reward",
            "Reward selection for In The Fiend's Shadow",
            InTheFiendsShadow.Rewards.All
        ),
        new Option<FiendsPurgatory.Rewards>(
            "Fiend's Purgatory",
            "Fiend's Purgatory Reward",
            "Reward selection for Fiend's Purgatory",
            FiendsPurgatory.Rewards.All
        ),
        new Option<RolesUnreversed.Rewards>(
            "Roles Unreversed",
            "Roles Unreversed Reward",
            "Reward selection for Roles Unreversed",
            RolesUnreversed.Rewards.All
        ),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        GetAll();

        Core.SetOptions(false);
    }

    public void GetAll()
    {
        bool getAllDrops = Bot.Config!.Get<bool>("getAll");
        var fiendsShadowReward = getAllDrops
            ? InTheFiendsShadow.Rewards.All
            : Bot.Config.Get<InTheFiendsShadow.Rewards>("In The Fiend's Shadow");
        var fiendsPurgatoryReward = getAllDrops
            ? FiendsPurgatory.Rewards.All
            : Bot.Config.Get<FiendsPurgatory.Rewards>("Fiend's Purgatory");
        var rolesUnreversedReward = getAllDrops
            ? RolesUnreversed.Rewards.All
            : Bot.Config.Get<RolesUnreversed.Rewards>("Roles Unreversed");

        if (fiendsShadowReward != InTheFiendsShadow.Rewards.None)
            GetFiendsShadowRewards(fiendsShadowReward);
        if (!Bot.ShouldExit && fiendsPurgatoryReward != FiendsPurgatory.Rewards.None)
            FP.Purgatory(fiendsPurgatoryReward);
        if (!Bot.ShouldExit)
            MP.DoMuddledPast();
        if (!Bot.ShouldExit)
            TaP.DoTugandPull();
        if (!Bot.ShouldExit && rolesUnreversedReward != RolesUnreversed.Rewards.None)
            GetRolesUnreversedRewards(rolesUnreversedReward);
    }

    private void GetFiendsShadowRewards(InTheFiendsShadow.Rewards reward)
    {
        if (reward == InTheFiendsShadow.Rewards.All)
        {
            foreach (InTheFiendsShadow.Rewards selectedReward in Enum.GetValues<InTheFiendsShadow.Rewards>())
            {
                if (Bot.ShouldExit)
                    return;
                if (selectedReward != InTheFiendsShadow.Rewards.All && selectedReward != InTheFiendsShadow.Rewards.None)
                    ITFS.FiendsShadow(selectedReward);
            }
            return;
        }

        ITFS.FiendsShadow(reward);
    }

    private void GetRolesUnreversedRewards(RolesUnreversed.Rewards reward)
    {
        if (reward == RolesUnreversed.Rewards.All)
        {
            foreach (RolesUnreversed.Rewards selectedReward in Enum.GetValues<RolesUnreversed.Rewards>())
            {
                if (Bot.ShouldExit)
                    return;
                if (selectedReward != RolesUnreversed.Rewards.All && selectedReward != RolesUnreversed.Rewards.None)
                    RU.DoRolesUnreversed(selectedReward);
            }
            return;
        }

        RU.DoRolesUnreversed(reward);
    }
}
