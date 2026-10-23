/*
name: TendurrrTheAssistantQuests
description: null
tags: null
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/Nation/CoreNation.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Models.Quests;
using Skua.Core.Options;

public class TendurrrTheAssistantQuests
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public static CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreNation Nation
    {
        get => _Nation ??= new CoreNation();
        set => _Nation = value;
    }
    private static CoreNation _Nation;

    public string OptionsStorage = "Reward Select";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
        new Option<RewardsSelection>(
            "RewardsSelection",
            "Select Your Quest Reward",
            "Select Your Quest Reward for The Tendurr items of Nulgath quest.",
            RewardsSelection.Tendurrr_on_your_Back
        ),
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.BankingBlackList.AddRange(Nation.bagDrops);
        Core.BankingBlackList.AddRange(Rewards);
        Core.SetOptions();

        TendurrItems(Bot.Config!.Get<RewardsSelection>("RewardsSelection"));

        Core.SetOptions(false);
    }


    /// <summary>
    /// Farms the selected Tendurrr The Assistant quest reward or all available rewards.
    /// </summary>
    /// <param name="reward">The reward to obtain, or All to obtain every reward.</param>
    public void TendurrItems(RewardsSelection reward = RewardsSelection.All)
    {
        Core.AddDrop(Nation.bagDrops);
        Core.AddDrop(Rewards);

        if (!Core.CheckInventory("Tendurrr The Assistant"))
            Core.KillMonster("tercessuinotlim", "m2", "Left", "*", "Tendurrr The Assistant", isTemp: false);

        Nation.FarmUni13(1);

        if (reward != RewardsSelection.All && (int)reward >= Rewards.Length)
        {
            Core.Logger($"Invalid Tendurrr reward selection: {reward}");
            return;
        }

        List<ItemBase> questRewards = Core.EnsureLoad(5814).Rewards;

        ItemBase? selectedReward = reward == RewardsSelection.All
            ? null
            : questRewards.Find(x => x.Name == Rewards[(int)reward]);

        if (reward != RewardsSelection.All && selectedReward == null)
        {
            Core.Logger($"Could not find the quest reward for {reward}.");
            return;
        }

        int iteration = 1;

        while (
            !Bot.ShouldExit
            && (
                reward == RewardsSelection.All
                    ? !Core.CheckInventory(Rewards, toInv: false)
                    : !Core.CheckInventory(selectedReward!.ID, toInv: false)
            )
        )
        {
            if (reward == RewardsSelection.All)
                Core.Logger($"Farming Tendurrr rewards (attempt {iteration++})");
            else
                Core.Logger($"Farming {selectedReward!.Name}");

            Core.EnsureAccept(5814);

            Nation.ApprovalAndFavor(500, 500);
            Nation.EssenceofNulgath(20);
            Nation.FarmGemofNulgath(30);
            Nation.FarmDarkCrystalShard(30);
            Nation.FarmTaintedGem(30);
            Nation.FarmDiamondofNulgath(30);
            Nation.FarmVoucher(member: true);
            Nation.FarmBloodGem(10);
            Nation.FarmTotemofNulgath(3);

            if (Bot.ShouldExit)
                break;

            if (reward == RewardsSelection.All)
            {
                if (!Core.EnsureCompleteChoose(5814, Rewards))
                {
                    Core.Logger("Failed to collect a Tendurrr reward. Stopping to prevent an endless farming loop.");
                    return;
                }
            }
            else
            {
                Core.EnsureComplete(5814, selectedReward!.ID);
            }
        }

        if (!Bot.ShouldExit && Core.CheckInventory(Rewards, toInv: false))
            Core.Logger("All Tendurrr rewards are owned.");
    }


    public readonly string[] Rewards =
    {
        "Tendurrr on your Back",
        "Asuka's Flaming Stick",
        "Tendurrr's Flaming Stick",
        "Tendurrr Morph",
    };

    public enum RewardsSelection
    {
        Tendurrr_on_your_Back = 0,
        Asuka_Flaming_Stick = 1,
        Tendurrr_Flaming_Stick = 2,
        Tendurrr_Morph = 3,
        All,
    };
}
