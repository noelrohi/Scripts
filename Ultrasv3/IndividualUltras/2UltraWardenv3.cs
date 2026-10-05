/*
name: UltraWardenv3
description: Ultra Warden v3 — runs the Comp picked by the DoAllUltras "Ultra Warden comp" option. default: Verus DoomKnight and Lord of Order taunt Warden 5s apart on a 10s cycle, King's Echo and StoneCrusher hit him; loo-lr-sc-csh: when Warden goes berserk, Legion Revenant taunts him and Lord of Order casts its 5th skill on him, then spams its heal.
tags: null
*/
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreEnginev3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/CoreUltrav3.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPotions.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraGeneral.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraCustomClassSync.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraPartyLayout.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraWaitForArmy.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/GetScrolls.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraAsync.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraComp.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraAttempt.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Skua.Core.Interfaces;

public class UltraWardenv3
{
    // The DoAllUltras boss key: the Comp and layout options are UltraWardenComp and UltraWardenLayout.
    public const string Boss = "UltraWarden";

    // Roles: what each class of the chosen Comp does in the fight.
    private const string WardenTaunter = "WardenTaunter";   // stays on Warden, so its timed taunts land on Warden
    private const string WardenAttacker = "WardenAttacker"; // stays on Warden
    // The berserk Roles act on Warden's "goes berserk" message, and otherwise hit Warden.
    private const string BerserkTaunter = "BerserkTaunter"; // taunts Warden
    private const string BerserkHealer = "BerserkHealer";   // casts its 5th skill (Cast(4)) on Warden to negate his damage, then spams its heal (Cast(2))

    /// <summary>Warden's Comps. The DoAllUltras "Ultra Warden comp" option picks one; blank runs default.</summary>
    public static readonly UltraComp[] Comps =
    {
        // The two timed taunters and two DPS classes, with the class presets the script used before Comps.
        new(UltraComp.Default,
            new UltraCompEntry
            {
                Class = "Verus DoomKnight",
                Role = WardenTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Lacerate },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Forge },
                    Potions = new[] { "Body Tonic", "Potent Destruction Elixir" },
                    Scroll = UltraLoadout.ScrollOfEnrage,
                },
                Taunt = UltraTaunt.Every(10, atSec: 0),
            },
            new UltraCompEntry
            {
                Class = "Lord of Order",
                Role = WardenTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Arcanas_Concerto, WeaponSpecial.Awe_Blast },
                    Cape = new[] { CapeSpecial.Absolution },
                    Potions = new[] { "Body Tonic", "Unstable Divine Elixir" },
                    Scroll = UltraLoadout.ScrollOfEnrage,
                },
                Taunt = UltraTaunt.Every(10, atSec: 5),
            },
            new UltraCompEntry
            {
                Class = "King's Echo",
                Role = WardenAttacker,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Healer,
                    Weapon = new[] { WeaponSpecial.Elysium, WeaponSpecial.Mana_Vamp },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Examen },
                    Potions = new[] { "Body Tonic", "Potent Destruction Elixir", UltraPotions.HonorOrMalice },
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "StoneCrusher",
                Role = WardenAttacker,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Valiance },
                    Cape = new[] { CapeSpecial.Absolution },
                    Helm = new[] { HelmSpecial.Anima },
                    Potions = new[] { "Body Tonic", "Unstable Divine Elixir", UltraPotions.HonorOrMalice },
                },
                Taunt = UltraTaunt.Never,
            }),

        // The Recommended Group of the community "Simplified bosses guide" (with Forge helms):
        // only Warden's berserk is handled, cued by his "goes berserk" message rather than a timer.
        new("loo-lr-sc-csh",
            new UltraCompEntry
            {
                Class = "Lord of Order",
                Role = BerserkHealer,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Lucky,
                    Weapon = new[] { WeaponSpecial.Awe_Blast },
                    Cape = new[] { CapeSpecial.Penitence },
                    Helm = new[] { HelmSpecial.Forge },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "Legion Revenant",
                Role = BerserkTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Wizard,
                    Weapon = new[] { WeaponSpecial.Ravenous },
                    Cape = new[] { CapeSpecial.Lament },
                    Helm = new[] { HelmSpecial.Pneuma },
                    EnhanceWhenAutoEnhanceIsOff = true,
                    Scroll = UltraLoadout.ScrollOfEnrage,
                },
                Taunt = UltraTaunt.ByRole($"when Warden says \"{BerserkCue}\""),
            },
            new UltraCompEntry
            {
                Class = "StoneCrusher",
                Role = WardenAttacker,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Lacerate },
                    Cape = new[] { CapeSpecial.Absolution },
                    Helm = new[] { HelmSpecial.Anima },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            },
            new UltraCompEntry
            {
                Class = "Chrono ShadowHunter",
                Role = WardenAttacker,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Lucky,
                    Weapon = new[] { WeaponSpecial.Valiance },
                    Cape = new[] { CapeSpecial.Vainglory },
                    Helm = new[] { HelmSpecial.Examen, HelmSpecial.Forge },
                    EnhanceWhenAutoEnhanceIsOff = true,
                },
                Taunt = UltraTaunt.Never,
            }),
    };

    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;
    private static CoreEnginev3 Engine => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;
    private static string _fbsMuteFile = "";

    // Warden's MapID in ultrawarden.
    private const int Warden = 1;

    // Warden's server message (a "umsg" packet) when he goes berserk, at about a fifth of his HP:
    // "Ultra Warden goes berserk!!  Kill it quickly!!". Matched case-insensitively.
    private const string BerserkCue = "goes berserk";

    // The berserk healer's skills, counted from 0 as Cast counts them (0 is the auto attack):
    // the guide's skill 5 is Order, which debuffs its target; Resurgence heals the party.
    private const int OrderSkill = 4;
    private const int HealSkill = 2;
    private const int OrderTryMs = 6000; // how long to keep trying Order while it is on cooldown

    private CancellationTokenSource _tauntCts = new();
    private UltraPartyLayout _party = null!;
    private UltraComp _comp = null!;
    private UltraCompEntry _entry = null!;
    private UltraAttempt? _attempt;
    private volatile bool _inFight;
    private DateTime _lastBerserkAt = DateTime.MinValue;

    public void ScriptMain(IScriptInterface bot)
    {
        RunBoss();
        Bot.StopSync();
    }

    public void RunBoss()
    {
        C.SetOptions(true);
        _fbsMuteFile = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "Skua", "fbs_mute.sync"
        );
        try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }
        Engine.Boot();
        _tauntCts = new();
        Bot.Events.ScriptStopping -= StopTauntEvent;
        Bot.Events.ScriptStopping += StopTauntEvent;

        try
        {
            if (!Prep())
                return;

            if (_entry.Role is BerserkTaunter or BerserkHealer)
                Bot.Events.ExtensionPacketReceived += BerserkListener;

            Fight();
        }
        finally
        {
            // An Attempt still open here ended without a kill. Warden has no Wipe detection.
            _attempt?.End(UltraAttempt.Outcome.Stopped);
            _inFight = false;
            Bot.Events.ExtensionPacketReceived -= BerserkListener;
            Bot.Events.ScriptStopping -= StopTauntEvent;
            _tauntCts.Cancel();
            try { if (File.Exists(_fbsMuteFile)) File.Delete(_fbsMuteFile); } catch { }
            Engine.DisableSkills();
            C.SetOptions(false);
        }
    }

    private bool StopTauntEvent(Exception? e)
    {
        _attempt?.End(UltraAttempt.Outcome.Stopped);
        _tauntCts.Cancel();
        return true;
    }

    /// <summary>Picks the Comp, equips this account's class from it and enhances it as its Loadout says.</summary>
    private bool Prep()
    {
        if (UltraComp.Prep(Boss, Comps, "UltraWarden-v3", Ultra, 4, "ultra_warden_class-v3.sync") is not { } prep)
            return false;
        (_comp, _party, _entry) = prep;
        return true;
    }

    private void Fight()
    {
        const string map = "ultrawarden";
        const string boss = "Ultra Warden";
        const string bossDefeatedTemp = "Ultra Warden Defeated";

        const string waitSyncFile = "ultra_warden.sync";
        const string fightTimeSyncFile = "UltraWardenFightTime.sync";
        const string completionSyncFile = "UltraWardenCompletion.sync";
        int armySize = 4;

        const int questId = 8153;

        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(fightTimeSyncFile));
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        UltraComp.ReadyForAttempt(_party, _entry, armySize, waitSyncFile, map);
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        Engine.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();
        Bot.Sleep(2000);

        // Pre-seed completion sync file so all 4 entries exist before the loop starts.
        string? _username = Bot.Player.Username;
        string? _className = Bot.Player.CurrentClass?.Name;
        if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_className))
        {
            string _myKey = $"{_username}|{_className}".Replace(":", "-");
            Ultra.UpdateEntry(Ultra.ResolveSyncPath(completionSyncFile), _myKey, "0");
        }

        UltraAttempt attempt = _attempt = UltraAttempt.Begin(Boss, _comp.Name, _entry.Class, _entry.Role, map, m => m.MapID == Warden);

        _comp.StartTaunts(_entry, Ultra, Ultra.ResolveSyncPath(fightTimeSyncFile), _tauntCts.Token);
        _inFight = true;

        while (!Bot.ShouldExit)
        {
            // Refresh mute file so FBS plugin stays muted during the fight
            try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }

            if (!Bot.Player.Alive)
            {
                // The boss's HP is still noted while dead.
                Bot.Wait.ForTrue(() => Bot.Player.Alive, attempt.SeeBoss, 20);
                continue;
            }

            attempt.SeeBoss();

            if (Bot.TempInv.Contains(bossDefeatedTemp, 1))
                attempt.End(UltraAttempt.Outcome.Kill);

            if (Ultra.CheckArmyProgressBool(() => Bot.TempInv.Contains(bossDefeatedTemp, 1), completionSyncFile))
            {
                C.Logger("Ultra Warden defeated. Finishing quest.");
                _inFight = false;
                Bot.Events.ScriptStopping -= StopTauntEvent;
                _tauntCts.Cancel();
                Engine.DisableSkills();
                Engine.Join(map);
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                break;
            }

            // Every Role stays on Warden: the taunts and Order land on whatever is targeted.
            if (Bot.Player.Target?.Name != boss)
                Bot.Combat.Attack(boss);

            _entry.Loadout.ActivatePotion();

            Bot.Sleep(500);
        }
    }

    /// <summary>Warden's server messages arrive as "umsg" packets: <c>{ cmd: "umsg", s: "&lt;text&gt;" }</c>.</summary>
    private void BerserkListener(dynamic packet)
    {
        try
        {
            if (!_inFight)
                return;

            string type = packet["params"].type;
            if (type is not "json")
                return;

            dynamic data = packet["params"].dataObj;
            if (data.cmd.ToString() != "umsg" || data.s is null)
                return;

            string message = (string)data.s;
            if (!message.Contains(BerserkCue, StringComparison.OrdinalIgnoreCase))
                return;

            // One message can come in more than one packet.
            DateTime now = DateTime.UtcNow;
            if (now - _lastBerserkAt < TimeSpan.FromSeconds(2))
                return;
            _lastBerserkAt = now;

            C.Logger($"[Berserk] Warden: \"{message.Trim()}\"");
            CancellationToken token = _tauntCts.Token;
            if (_entry.Role == BerserkTaunter)
                _ = Task.Run(() => TauntWarden(token));
            else if (_entry.Role == BerserkHealer)
                _ = Task.Run(() => NegateThenHeal(token));
        }
        catch { }
    }

    /// <summary>Presses the scroll on Warden.</summary>
    private static void TauntWarden(CancellationToken token)
    {
        if (!Bot.Player.Alive || token.IsCancellationRequested)
            return;
        AttackWarden();
        C.Logger("[Berserk] BerserkTaunter taunts Warden.");
        UltraAsync.TauntPresses(Bot, C, Engine, token);
    }

    /// <summary>
    /// Casts Order on Warden as soon as it is ready, then casts the heal whenever it is ready
    /// until Warden dies or the fight ends.
    /// </summary>
    private static void NegateThenHeal(CancellationToken token)
    {
        DateTime giveUp = DateTime.UtcNow.AddMilliseconds(OrderTryMs);
        bool order = false;
        while (!order && DateTime.UtcNow < giveUp && !token.IsCancellationRequested && !Bot.ShouldExit && WardenAlive())
        {
            if (Bot.Player.Alive)
            {
                AttackWarden();
                order = Bot.Player.Target?.MapID == Warden && Engine.Cast(OrderSkill);
            }
            if (!order)
                Thread.Sleep(100);
        }
        C.Logger(order
            ? "[Berserk] BerserkHealer cast Order on Warden; spamming its heal."
            : $"[Berserk] BerserkHealer could not cast Order on Warden within {OrderTryMs / 1000} s; spamming its heal.");

        while (!token.IsCancellationRequested && !Bot.ShouldExit && WardenAlive())
        {
            if (Bot.Player.Alive)
                Engine.Cast(HealSkill);
            Thread.Sleep(100);
        }
    }

    private static void AttackWarden()
    {
        if (Bot.Player.Target?.MapID != Warden)
            Bot.Combat.Attack(Warden);
    }

    private static bool WardenAlive() =>
        Bot.Monsters.MapMonsters.Any(m => m != null && m.MapID == Warden && m.HP > 0);
}
