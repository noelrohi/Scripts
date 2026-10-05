/*
name: UltraSpeakerv3
description: Ultra First Speaker v3 — runs the Comp picked by the DoAllUltras "Ultra Speaker comp" option. default: chat-driven taunts (ArchPaladin on the listen phase, Lord of Order, StoneCrusher and Verus DoomKnight in turn on the truth phase) with position management and stasis handling.
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
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraComp.cs
//cs_include Scripts/Ultrasv3/DependenciesUltras/UltraAttempt.cs
//cs_include Scripts/CoreBots.cs
//cs_include Scripts/CoreAdvanced.cs

using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Skua.Core.Interfaces;

public class UltraSpeakerv3
{
    // The DoAllUltras boss key: the Comp and layout options are UltraSpeakerComp and UltraSpeakerLayout.
    public const string Boss = "UltraSpeaker";

    // Roles: what each class of the chosen Comp does in the fight. Everyone hits the Speaker;
    // the Roles differ in which chat line they taunt on.
    private const string ListenTaunter = "ListenTaunter"; // taunts on every "You shall listen."
    private const string TruthTaunter1 = "TruthTaunter1"; // taunts on the 3rd, 6th, 9th... "I will make you see the truth."
    private const string TruthTaunter2 = "TruthTaunter2"; // on the 1st, 4th, 7th...
    private const string TruthTaunter3 = "TruthTaunter3"; // on the 2nd, 5th, 8th...

    // Its 4th skill, Decay, removes Scintillation: it casts it on "All stand equal beneath the eyes of the Eternal."
    private const string DecayClass = "Verus DoomKnight";

    private const string ScrollOfEnrage = "Scroll of Enrage";

    /// <summary>The Speaker's Comps. The DoAllUltras "Ultra Speaker comp" option picks one; blank runs default.</summary>
    public static readonly UltraComp[] Comps =
    {
        new(UltraComp.Default,
            new UltraCompEntry
            {
                Class = "ArchPaladin",
                Role = ListenTaunter,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Lacerate },
                    Potions = new[] { "Body Tonic", "Potent Destruction Elixir" },
                    Scroll = ScrollOfEnrage,
                },
                Taunt = UltraTaunt.ByRole("on every \"You shall listen.\""),
            },
            new UltraCompEntry
            {
                Class = "Lord of Order",
                Role = TruthTaunter1,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Valiance },
                    Potions = new[] { "Body Tonic", "Unstable Divine Elixir" },
                    Scroll = ScrollOfEnrage,
                },
                Taunt = UltraTaunt.ByRole("on the 3rd, 6th, 9th... \"I will make you see the truth.\""),
            },
            new UltraCompEntry
            {
                Class = "StoneCrusher",
                Role = TruthTaunter2,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Valiance },
                    Cape = new[] { CapeSpecial.Absolution },
                    Potions = new[] { "Body Tonic", "Unstable Divine Elixir" },
                    Scroll = ScrollOfEnrage,
                },
                Taunt = UltraTaunt.ByRole("on the 1st, 4th, 7th... \"I will make you see the truth.\""),
            },
            new UltraCompEntry
            {
                Class = "Verus DoomKnight",
                Role = TruthTaunter3,
                Loadout = new UltraLoadout
                {
                    Enhancement = EnhancementType.Fighter,
                    Weapon = new[] { WeaponSpecial.Praxis },
                    Cape = new[] { CapeSpecial.Penitence },
                    Potions = new[] { "Body Tonic", "Potent Destruction Elixir" },
                    Scroll = ScrollOfEnrage,
                },
                Taunt = UltraTaunt.ByRole("on the 2nd, 5th, 8th... \"I will make you see the truth.\""),
            }),
    };

    private static IScriptInterface Bot => IScriptInterface.Instance;
    private static CoreBots C => CoreBots.Instance;
    private static CoreEnginev3 Engine => CoreEnginev3.Instance;
    private static CoreUltrav3 Ultra => _Ultra ??= new CoreUltrav3();
    private static CoreUltrav3 _Ultra;

    private UltraPartyLayout _party = null!;
    private UltraComp _comp = null!;
    private UltraCompEntry? _entry;
    private UltraAttempt? _attempt;

    // Chat-listener state
    private int truthTauntTurn;
    private int lastTruthTauntActedTurn;
    private const string TruthTurnSyncFile = "UltraSpeakerTruthTurn.sync";
    private int listenTauntTurn;
    private int lastListenTauntActedTurn;
    private const string ListenTurnSyncFile = "UltraSpeakerListenTurn.sync";
    private int equalizeCount;
    private bool _movingToEqualize;
    private bool _positioned;

    private const int EqualizeX = 200;
    private const int EqualizeY = 380;
    private static string _fbsMuteFile = "";

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
        Engine.Boot(); // Use CoreEngine auto-rotation

        Bot.Events.ExtensionPacketReceived += SpeakerMessageListener;
        Bot.Flash.FlashCall += SpeakerFlashListener;
        Bot.Events.ScriptStopping += StopAttemptEvent;
        try
        {
            if (Prep())
                Fight();
        }
        finally
        {
            // An Attempt still open here ended without a kill. The Speaker has no Wipe detection.
            _attempt?.End(UltraAttempt.Outcome.Stopped);
            Bot.Events.ScriptStopping -= StopAttemptEvent;
            Bot.Events.ExtensionPacketReceived -= SpeakerMessageListener;
            Bot.Flash.FlashCall -= SpeakerFlashListener;
            try { if (File.Exists(_fbsMuteFile)) File.Delete(_fbsMuteFile); } catch { }
            Engine.DisableSkills();
            C.SetOptions(false);
        }
    }

    private bool StopAttemptEvent(Exception? e)
    {
        _attempt?.End(UltraAttempt.Outcome.Stopped);
        return true;
    }

    private bool IsListenTaunter() => _entry?.Role == ListenTaunter;

    private int MyTruthIndex() => _entry?.Role switch
    {
        TruthTaunter1 => 0,
        TruthTaunter2 => 1,
        TruthTaunter3 => 2,
        _ => -1
    };

    private int MyListenIndex() => IsListenTaunter() ? 0 : -1;

    /// <summary>Picks the Comp, equips this account's class from it and enhances it as its Loadout says.</summary>
    private bool Prep()
    {
        UltraComp? comp = UltraComp.Read(Boss, Comps);
        if (comp == null)
            return false;
        _comp = comp;
        _party = UltraPartyLayout.Read(Boss);

        UltraGeneral.EquipWarriorClass();
        Bot.Sleep(2000);

        C.Logger($"[UltraSpeaker-v3] Equipping the {_comp.Name} Comp's classes for army size 4.");
        UltraCompEntry? entry = _comp.EquipClass(_party, Ultra, 4, "ultra_speaker_class-v3.sync");
        if (entry == null)
            return false;
        _entry = entry;

        _entry.Loadout.Enhance();
        return true;
    }

    private void Fight()
    {
        const string map = "ultraspeaker";
        const string boss = "The First Speaker";
        const string bossDefeatedTemp = "The First Speaker Silenced";

        const string waitSyncFile = "ultra_speaker.sync";
        const string completionSyncFile = "UltraSpeakerCompletion.sync";
        int armySize = 4;

        const int questId = 9173;

        if (!UltraGeneral.IsQuestGreen(Bot, questId))
            UltraGeneral.EnsureAcceptOnce(Bot, questId);

        if (!Bot.Quests.IsUnlocked(questId))
            Bot.Quests.UpdateQuest(9125);

        Bot.Options.DisableCollisions = true;
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(TruthTurnSyncFile));
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(ListenTurnSyncFile));
        Ultra.ClearSyncFile(Ultra.ResolveSyncPath(completionSyncFile));

        // Buying potion reagents can swap to a farm class; the Comp's class goes back on.
        _party.EnsureClass();
        _entry!.Loadout.Stock();
        _party.EnsureClass();

        C.Join("Whitemap");
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: false);

        _entry.Loadout.Use();

        _party.EnsureClass();
        Engine.Join(map);
        Bot.Sleep(2500);
        UltraWaitForArmy.Instance.NewWaitForArmy(armySize - 1, waitSyncFile, useSkill: true);

        Engine.ChooseBestCell(boss);
        Bot.Player.SetSpawnPoint();

        string? _username = Bot.Player.Username;
        string? _className = Bot.Player.CurrentClass?.Name;
        if (!string.IsNullOrWhiteSpace(_username) && !string.IsNullOrWhiteSpace(_className))
        {
            string _myKey = $"{_username}|{_className}".Replace(":", "-");
            Ultra.UpdateEntry(Ultra.ResolveSyncPath(completionSyncFile), _myKey, "0");
        }

        DateTime fightStartTime = DateTime.UtcNow;
        UltraAttempt attempt = _attempt = UltraAttempt.Begin(Boss, _comp.Name, _entry.Class, _entry.Role, map, m => m.Name == boss);

        while (!Bot.ShouldExit)
        {
            // Refresh mute file so FBS plugin stays muted during the fight
            try { File.WriteAllText(_fbsMuteFile, DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString()); } catch { }

            if (Bot.Player?.Alive != true)
            {
                // The boss's HP is still noted while dead.
                Bot.Wait.ForTrue(() => Bot.Player?.Alive == true, attempt.SeeBoss, 20);
                // Just respawned — re-sync truth taunt turn counter from sync file
                try
                {
                    string syncFile = Ultra.ResolveSyncPath(TruthTurnSyncFile);
                    string[] lines = Ultra.ReadLines(syncFile);
                    int maxTurn = 0;
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length >= 3 && int.TryParse(parts[1], out int turn) && turn > maxTurn)
                            maxTurn = turn;
                    }
                    if (maxTurn > truthTauntTurn)
                    {
                        truthTauntTurn = maxTurn;
                        lastTruthTauntActedTurn = maxTurn;
                        C.Logger($"[UltraSpeaker-v3] Re-synced truth taunt turn to {truthTauntTurn} after respawn.");
                    }
                }
                catch { }
                // Re-sync listen taunt turn counter from sync file
                try
                {
                    string syncFile = Ultra.ResolveSyncPath(ListenTurnSyncFile);
                    string[] lines = Ultra.ReadLines(syncFile);
                    int maxTurn = 0;
                    foreach (string line in lines)
                    {
                        string[] parts = line.Split(':');
                        if (parts.Length >= 3 && int.TryParse(parts[1], out int turn) && turn > maxTurn)
                            maxTurn = turn;
                    }
                    if (maxTurn > listenTauntTurn)
                    {
                        listenTauntTurn = maxTurn;
                        lastListenTauntActedTurn = maxTurn;
                        C.Logger($"[UltraSpeaker-v3] Re-synced listen taunt turn to {listenTauntTurn} after respawn.");
                    }
                }
                catch { }
                // Reset position flag so we walk to safe box again after respawn
                _positioned = false;
                continue;
            }

            // Position management — walk to safe box once on first enter or after death
            if (!_positioned && !_movingToEqualize && Bot.Player?.Cell == "Boss")
            {
                const int minX = 0, maxX = 100;
                const int minY = 485, maxY = 500;

                int randomX = Random.Shared.Next(minX, maxX + 1);
                int randomY = Random.Shared.Next(minY, maxY + 1);
                Bot.Player.WalkTo(randomX, randomY);
                Bot.Wait.ForTrue(() => Math.Abs(Bot.Player.X - randomX) < 40, 10);
                _positioned = true;
            }

            attempt.SeeBoss();

            if (Bot.Inventory.Contains(bossDefeatedTemp, 1))
                attempt.End(UltraAttempt.Outcome.Kill);

            if (Ultra.CheckArmyProgressBool(() => Bot.Inventory.Contains(bossDefeatedTemp, 1), completionSyncFile))
            {
                C.Logger("The First Speaker defeated. Finishing quest.");
                Engine.DisableSkills();
                Engine.Join(map);
                Ultra.PersistentJoinHouse();
                UltraGeneral.CompleteQuest(Bot, questId);
                Bot.Sleep(3000);
                break;
            }

            Bot.Combat.Attack(boss);
            _entry.Loadout.ActivatePotion();

            Bot.Sleep(500);
        }
    }

    private void SpeakerMessageListener(dynamic packet)
    {
        try
        {
            string type = packet["params"].type;
            if (type is not "json")
                return;

            if (!Bot.Player.Alive)
                return;

            dynamic data = packet["params"].dataObj;
            string cmd = data.cmd.ToString();

            if (cmd != "ct")
                return;

            if (data.anims is null)
                return;

            foreach (dynamic anim in data.anims)
            {
                if (anim is null || anim.msg is null)
                    continue;

                string message = (string)anim.msg;

                // Energy Draw — ListenTaunters (cascade 1-2-1-2-1-2)
                if (message.Contains("You shall listen.", StringComparison.OrdinalIgnoreCase))
                {
                    listenTauntTurn++;
                    C.Logger($"Detected: 'You shall listen.' (turn {listenTauntTurn})");
                    if (listenTauntTurn % 2 == MyListenIndex())
                    {
                        try
                        {
                            string myKey = $"{Bot.Player.Username}|{Bot.Player.CurrentClass?.Name ?? "Unknown"}".Replace(":", "-");
                            Ultra.UpdateEntry(Ultra.ResolveSyncPath(ListenTurnSyncFile), myKey, listenTauntTurn.ToString());
                        }
                        catch { }
                    }
                    _ = ListenTauntAsync();
                }

                // Magia Draw — TruthTaunters (cascade 1-2-1-2-1-2)
                if (message.Contains("I will make you see the truth.", StringComparison.OrdinalIgnoreCase))
                {
                    truthTauntTurn++;
                    // Only write to sync file when it's our turn — 1 writer per truth taunt
                    if (truthTauntTurn % 3 == MyTruthIndex())
                    {
                        try
                        {
                            string myKey = $"{Bot.Player.Username}|{Bot.Player.CurrentClass?.Name ?? "Unknown"}".Replace(":", "-");
                            Ultra.UpdateEntry(Ultra.ResolveSyncPath(TruthTurnSyncFile), myKey, truthTauntTurn.ToString());
                        }
                        catch { }
                    }
                    // Fire immediately — no Fight loop latency
                    _ = TruthTauntAsync();
                }
            }
        }
        catch { }
    }

    private void SpeakerFlashListener(string name, object[] args)
    {
        try
        {
            if (name != "packetFromServer")
                return;

            dynamic? data = null;
            var packet = JsonConvert.DeserializeObject<dynamic>((string)args[0])!;
            data = packet?["b"]?["o"];

            if (data == null || data!["cmd"]?.ToString() != "ct")
                return;

            // FlashCall version: check anim messages
            if (data!["anims"] != null)
            {
                foreach (var anim in data["anims"])
                {
                    if (anim?.msg != null)
                    {
                        string msg = (string)anim.msg;
                        if (msg.Contains("All stand equal beneath the eyes of the Eternal.", StringComparison.OrdinalIgnoreCase))
                        {
                            equalizeCount++;
                            C.Logger($"[UltraSpeaker-v3] Detected 'All stand equal beneath the eyes of the Eternal.' (count {equalizeCount})");

                            // Fire Decay for VDK (removes Scintillation)
                            _ = DecayAsync();

                            // Check auras synchronously before spawning background task
                            if (!Bot.Player.Alive)
                                return;

                            float health = Bot.Player.Health;
                            float corruptionStacks = Engine.GetAuraStacksFloat("Corruption", true);
                            float somberStacks = Engine.GetAuraStacksFloat("Somber", true);
                            bool hasMagiaBurn = Engine.HasAura("Magia Burn", true);
                            bool hasStasis = Engine.HasAura("Stasis", true);
                            bool hasSanctity = Engine.HasAura("Sanctity", true);
                            bool hasLowHp = health < 5100f;

                            if (!((corruptionStacks > 0f || somberStacks > 0f)
                                && !hasMagiaBurn
                                && !hasStasis
                                && !hasSanctity
                                && !hasLowHp))
                            {
                                C.Logger($"[UltraSpeaker-v3] Skipping equalize zone: hp={health:F0} corruption={corruptionStacks:F2} somber={somberStacks:F2} magiaBurn={hasMagiaBurn} stasis={hasStasis} sanctity={hasSanctity}");
                                return;
                            }

                            C.Logger($"[UltraSpeaker-v3] Stepping into equalize zone: corruption={corruptionStacks:F2} somber={somberStacks:F2}");
                            _movingToEqualize = true;

                            // Same pattern as UltraDageListener — Task.Run keeps blocking WalkTo/Wait/Sleep off the game thread
                            _ = Task.Run(() =>
                            {
                                Bot.Player.WalkTo(EqualizeX, EqualizeY);
                                Bot.Wait.ForTrue(() => Math.Abs(Bot.Player.X - EqualizeX) < 40, 10);

                                // Wait for Equalize to hit (3s charge + buffer)
                                Bot.Sleep(6000);

                                // Walk back to bottom-left safe box
                                C.Logger("[UltraSpeaker-v3] Returning to bottom-left safe box after Equalize.");
                                int homeX = Random.Shared.Next(0, 100);
                                int homeY = Random.Shared.Next(485, 500);
                                Bot.Player.WalkTo(homeX, homeY);
                                Bot.Wait.ForTrue(() => Math.Abs(Bot.Player.X - homeX) < 40, 10);
                                _movingToEqualize = false;
                            });
                        }
                    }
                }
            }
        }
        catch { }
    }

    private async Task ListenTauntAsync()
    {
        if (!Bot.Player.Alive || !Bot.Player.HasTarget || !IsListenTaunter())
            return;

        // Guard: only act once per turn (single listen taunter always acts)
        if (listenTauntTurn <= lastListenTauntActedTurn)
            return;

        lastListenTauntActedTurn = listenTauntTurn;

        for (int i = 0; i < 60; i++)
        {
            Engine.Cast(5);
            await Task.Delay(50);
        }
    }

    private async Task TruthTauntAsync()
    {
        int myIndex = MyTruthIndex();
        if (myIndex < 0 || !Bot.Player.Alive || !Bot.Player.HasTarget)
            return;

        // Guard: only act if this turn is ours (3-way rotation) and we haven't acted yet
        if (truthTauntTurn % 3 != myIndex || truthTauntTurn <= lastTruthTauntActedTurn)
            return;

        lastTruthTauntActedTurn = truthTauntTurn;

        for (int i = 0; i < 60; i++)
        {
            Engine.Cast(5);
            await Task.Delay(50);
        }
    }

    private async Task DecayAsync()
    {
        if (Bot.Player.CurrentClass?.Name != DecayClass || !Bot.Player.Alive)
            return;

        for (int i = 0; i < 60; i++)
        {
            Engine.Cast(4);
            await Task.Delay(50);
        }
    }

}