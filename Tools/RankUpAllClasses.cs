/*
name: RankUpAllClasses
description: rank up all classes
tags: rank, up, class, rank class, rank up class, rank up all class, class rank, class points
*/
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreFarms.cs
//cs_include Scripts/CoreAdvanced.cs
using Skua.Core.Interfaces;
using Skua.Core.Models.Items;
using Skua.Core.Options;

public class RankUpAll
{
    public IScriptInterface Bot => IScriptInterface.Instance;
    public CoreBots Core => CoreBots.Instance;
    private static CoreFarms Farm
    {
        get => _Farm ??= new CoreFarms();
        set => _Farm = value;
    }
    private static CoreFarms _Farm;
    private static CoreAdvanced Adv
    {
        get => _Adv ??= new CoreAdvanced();
        set => _Adv = value;
    }
    private static CoreAdvanced _Adv;

    public string OptionsStorage = "RankUpAll";
    public bool DontPreconfigure = true;
    public List<IOption> Options = new()
    {
        CoreBots.Instance.SkipOptions,
    };

    public void ScriptMain(IScriptInterface bot)
    {
        Core.SetOptions();

        RankUpAllClasses();

        Core.SetOptions(false);
    }

    public void RankUpAllClasses()
    {
        // Define the classes to exclude
        string InitialClass = Bot.Player!.CurrentClass!.Name;
        List<string> excludedClasses = new()
        {
            "Hobo Highlord",
            "No Class",
            "Obsidian No Class",
            InitialClass,
        };
        // Populate SelectedClasses from inventory, excluding specific classes
        List<string> SelectedClasses = Bot
            .Inventory.Items.Where(c =>
                c.Category == ItemCategory.Class
                && c.Quantity < 302500
                && !excludedClasses.Contains(c.Name) // Exclude specific classes
                && (Core.IsMember || !c.Upgrade) // Remove upgrade classes if not a member
                && c.EnhancementLevel > 0
            ) // Ensure enhancement level is greater than 0
            .Select(x => x.Name)
            .ToList();

        // Optional: Log the updated SelectedClasses
        Core.Logger("Classes to Rank: " + string.Join(", ", SelectedClasses));

        // Rank up classes
        foreach (string Class in SelectedClasses.Except(InitialClass))
        {
            if (Core.CheckInventory(Class))
            {
                Adv.RankUpClass(Class, false);

                //Equip class we started with
                Core.Equip(InitialClass);
            }
        }
    }
}
