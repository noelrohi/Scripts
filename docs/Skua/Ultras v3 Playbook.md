# Ultras v3 Playbook

How to run the Ultras v3 bosses (`Ultrasv3/`) with a party of four accounts: what each boss
needs, which account should play what, and how to tell the scripts.

Examples name the accounts `alt1`…`alt4`. Use your own login names in the option.

## Daily and weekly bosses

`DoAllUltras` runs every boss whose quest some account still hasn't turned in this period.

| Boss | Quest | Resets |
|---|---|---|
| Ultra Ezrajal | 8152 | daily |
| Ultra Warden | 8153 | daily |
| Ultra Engineer | 8154 | daily |
| Ultra Avatar Tyndarius | 8245 | daily |
| Champion Drakath | 8300 | weekly |
| Ultra Nulgath | 8692 | weekly |
| Ultra Drago | 8397 | weekly |
| Ultra Dage | 8547 | weekly |
| Ultra Darkon | 8746 | weekly |
| Ultra Speaker | 9173 | weekly |
| Ultra Gramiel | 10301 | weekly |

A boss counts as done for an account only once its quest is turned in this period (the
game's daily `id` or weekly `iw` completion flag). An account that still holds the boss's
defeat item but never turned the quest in retries the turn-in before the next pass; it is
not skipped and the party does not refight for it.

## Picking a Comp

A Comp is a boss's named strategy: its 4 classes, each class's Loadout (enhancements,
potions, scroll) and Role, and when each class taunts. Bosses with Comps (so far Ultra
Ezrajal, Ultra Warden, Ultra Engineer, Ultra Avatar Tyndarius, Ultra Nulgath and Ultra
Speaker) have a `<Boss> comp` option in
`DoAllUltras`, next to `<Boss> layout`. Set it to a Comp's name; leave it blank to run the
boss's `default` Comp. An unknown name stops the bot and lists the boss's Comps. Like the
layout, a single boss script reads the saved option.

Each class's Loadout is applied before the fight: the enhancements once, the potions bought,
equipped and drunk before each Attempt, then the scroll equipped. During the fight an equipped
clickable potion, such as Potent Honor Potion, is clicked again whenever it is ready, unless
the Loadout says the potions are drunk before the fight only. Where a special lists a
fallback ("else"), the first one unlocked is used.

Each Attempt, from engaging the boss to its kill, a Wipe or the bot stopping, ends with an
`ultra.attempt` Script Report on every account: the boss, Comp, class and Role, the start
and end, the outcome (`kill`, `wipe` or `stopped`), the boss's HP at the end, and that
account's deaths in seconds since the start. Ultra Ezrajal, Ultra Warden, Ultra Engineer and
Ultra Speaker have no Wipe detection, so their Attempts end in `kill` or `stopped`.

## Setting the party layout

Open the `DoAllUltras` options. Every boss has a `<Boss> layout` option. Set it to
`Account=Class` pairs separated by `;`:

```
alt1=Dragon of Time; alt2=Legion Revenant; alt3=ArchPaladin; alt4=Lord of Order
```

- Each account equips the class the layout gives its login name. Its role comes from the
  class it actually equipped.
- The class is checked again after potions and scrolls are bought and right before joining
  the boss map. Buying potion reagents can swap to a farm class; the role class goes back on.
- Names and classes are case-insensitive. `,` also works as a separator.
- Every account in the party must be listed, and its class must be one of the boss's role
  classes. For a boss with Comps, every class in the layout must be one of the chosen Comp's
  4 classes. If not, the bot stops and says what's wrong.
- Leave a layout blank to keep the automatic assignment: the accounts share which role
  classes they own and the script hands the roles out (for a boss with Comps, the chosen
  Comp's classes).

The layouts are saved with the `DoAllUltras` options and read once per boss run. Running a
single boss script (for example `6UltraNulgathv3.cs`) uses the same saved layout, so set it
once in `DoAllUltras` and leave it.

## Bosses and roles

Default classes are what the automatic assignment hands out. Taunters equip Scroll of Enrage.

| Boss | Roles (default class) |
|---|---|
| Ezrajal | Three Comps, see below. |
| Warden | Five Comps, see below. |
| Engineer | Three Comps, see below. |
| Tyndarius | One Comp, see below. |
| Drakath | Taunters 1–3 (ArchPaladin, Lord of Order, Shaman), 4 s apart, DPS (StoneCrusher). |
| Nulgath | Three Comps, see below. |
| Drago | Taunter 1 and Taunter 2 (Lord of Order, Verus DoomKnight) hit the right summon and taunt the left one, 2 × DPS (StoneCrusher, King's Echo). |
| Darkon | Taunter 1 (Verus DoomKnight), Taunter 2 (Lord of Order), 2 × DPS (StoneCrusher, King's Echo). |
| Dage | Taunter 1 (Verus DoomKnight), Taunter 2 (ArchPaladin), Decay (Lord of Order, Scroll of Decay on Legionnaire), DPS (King's Echo). |
| Speaker | One Comp, see below. |
| Gramiel | Crystal taunters: left T1 (StoneCrusher), right T1 (ArchPaladin), left T2 (Lord of Order), right T2 (ArchFiend); all four taunt Gramiel in turn. |

### Ultra Ezrajal

#### Comps

The `Ultra Ezrajal comp` option picks one; blank runs `default`. Everyone hits Ezrajal and
stops attacking for about 6 s whenever he has Counter Attack. No one taunts.

**`default`**:

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| Verus DoomKnight | Hits Ezrajal | Fighter; Lacerate weapon, Lament cape, Forge helm; Body Tonic, Potent Destruction Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |
| StoneCrusher | Hits Ezrajal | Fighter; Valiance weapon, Absolution cape, Anima helm; Body Tonic, Unstable Divine Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |
| Lord of Order | Hits Ezrajal | Fighter; Arcana's Concerto else Awe Blast weapon, Absolution cape; Body Tonic, Unstable Divine Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |
| King's Echo | Hits Ezrajal | Healer; Elysium else Mana Vamp weapon, Lament cape, Examen helm; Body Tonic, Potent Destruction Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |

**`loo-lr-ap-csh`** (the Recommended Group of the community "Simplified bosses guide";
the guide asks for at least 3175 HP):

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| Lord of Order | Hits Ezrajal | Lucky; Awe Blast weapon, Penitence cape, Forge helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| Legion Revenant | Hits Ezrajal | Wizard; Arcana's Concerto weapon, Lament cape, no helm special, even with DisableAutoEnhance on; no potions; no scroll | never |
| ArchPaladin | Hits Ezrajal | Lucky; Praxis weapon, Lament cape, Forge helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| Chrono ShadowHunter | Hits Ezrajal | Lucky; Valiance weapon, Lament cape, Forge helm, even with DisableAutoEnhance on; no potions; no scroll | never |

The guide's Lock trick is not played in this Comp (`ke-lr-ap-loo` plays it). Ezrajal locks a
player's weapon special for 45 s (a "Skill Locked" aura naming it); the guide hits him with a
Mana Vamp weapon until that is locked, then swaps to the real weapon and restarts the fight
together. The script fights on the weapon above throughout.

**`ke-lr-ap-loo`** (the guide's Lock trick, without Vainglory capes):

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| King's Echo | Lock baiter, then hits Ezrajal | Lucky; Ravenous weapon, Lament cape, Forge helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| Legion Revenant | Lock baiter, then hits Ezrajal; needs at least 3175 max HP | Wizard; Arcana's Concerto weapon, Lament cape, no helm special, even with DisableAutoEnhance on; no potions; no scroll | never |
| ArchPaladin | Lock baiter, then hits Ezrajal | Lucky; Praxis weapon, Lament cape, Forge helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| Lord of Order | Lock baiter, then hits Ezrajal | Lucky; Awe Blast weapon, Penitence cape, Forge helm, even with DisableAutoEnhance on; no potions; no scroll | never |

The Lock trick, played by each account:

1. Before the first Attempt, it picks a bait weapon: a spare weapon from its inventory, else
   its bank (one already on Mana Vamp first, then one without a weapon special, then any), and
   enhances it with its Loadout's enhancement and Mana Vamp. Without a spare weapon or Awe
   enhancements it logs that and skips the trick, but still waits with the party in step 3.
2. In the fight, with its class skills off, it hits Ezrajal on auto attacks with the bait until
   his "Skill Locked" aura on it names Mana Vampire, for up to 20 s.
3. It steps out of Ezrajal's cell, puts its Loadout weapon back on and waits until all four
   have done so, then all four go back in and fight on their skills.

A Legion Revenant under 3175 max HP is warned about before each Attempt.

#### Results

One row per Attempt, from the four accounts' `ultra.attempt` reports. Accounts are
`alt1`…`alt4`; deaths are `Role @ s` since the Attempt started.

| Date | Comp | Party Layout (`alt1=Class; …`) | Outcome | Duration | Deaths (Role @ s) | Note |
|---|---|---|---|---|---|---|
| 2026-10-05 | ke-lr-ap-loo | alt1=King's Echo; alt2=Lord of Order; alt3=Legion Revenant; alt4=ArchPaladin | kill | 41 s | none | Rerun after `01a0ec853`, on an already turned-in daily: all four LockBaiters got their bait's Mana Vamp locked, three on a spare enhanced for it. The LockBaiter on Legion Revenant had 2,970 max HP, under the 3,175 its entry asks for (Lament not unlocked), and still took no deaths. |
| 2026-10-05 | ke-lr-ap-loo | alt1=King's Echo; alt2=Lord of Order; alt3=Legion Revenant; alt4=ArchPaladin | kill | 36 s | none | Only alt1 baited the lock (a spare already on Mana Vamp). The other three skipped it: the game won't equip an unenhanced weapon, so their spares never got Mana Vamp (fixed in `01a0ec853`). |

### Ultra Warden

#### Comps

The `Ultra Warden comp` option picks one; blank runs `default`. Everyone stays on Warden.

**`default`**:

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| Verus DoomKnight | Taunts Warden | Fighter; Lacerate weapon, Lament cape, Forge helm; Body Tonic, Potent Destruction Elixir; Scroll of Enrage | at 0 s of every 10 s |
| Lord of Order | Taunts Warden | Fighter; Arcana's Concerto else Awe Blast weapon, Absolution cape; Body Tonic, Unstable Divine Elixir; Scroll of Enrage | at 5 s of every 10 s |
| King's Echo | Hits Warden | Healer; Elysium else Mana Vamp weapon, Lament cape, Examen helm; Body Tonic, Potent Destruction Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |
| StoneCrusher | Hits Warden | Fighter; Valiance weapon, Absolution cape, Anima helm; Body Tonic, Unstable Divine Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |

The taunts count from the fight start the Verus DoomKnight shares.

**`loo-lr-sc-csh`** (the Recommended Group of the community "Simplified bosses guide", with
Forge helms):

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| Lord of Order | Berserk healer: when Warden goes berserk, casts its 5th skill (Order) on him to negate his damage, then its heal whenever it is ready until he dies; otherwise hits Warden | Lucky; Awe Blast weapon, Penitence cape, Forge helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| Legion Revenant | Berserk taunter: taunts Warden when he goes berserk; otherwise hits him | Wizard; Ravenous weapon, Lament cape, Pneuma helm, even with DisableAutoEnhance on; no potions; Scroll of Enrage | when Warden goes berserk |
| StoneCrusher | Hits Warden | Fighter; Lacerate weapon, Absolution cape, Anima helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| Chrono ShadowHunter | Hits Warden | Lucky; Valiance weapon, Vainglory cape, Examen else Forge helm, even with DisableAutoEnhance on; no potions; no scroll | never |

Warden going berserk is his server message "Ultra Warden goes berserk!!  Kill it quickly!!",
seen at about a fifth of his HP. The two berserk Roles act on it, in any case. Until then no
one taunts.

**`loo-lr-vdk-ke`** (`loo-lr-sc-csh`'s berserk Roles, with Verus DoomKnight and King's Echo
hitting Warden, and Order held for the berserk):

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| Lord of Order | Berserk healer holding Order: from 30% of Warden's HP its rotation stops casting its 5th skill (Order), so Order is ready for the berserk; then as the berserk healer above | Lucky; Awe Blast weapon, Penitence cape, Forge helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| Legion Revenant | Berserk taunter: taunts Warden when he goes berserk; otherwise hits him | Wizard; Ravenous weapon, Lament cape, Pneuma helm, even with DisableAutoEnhance on; no potions; Scroll of Enrage | when Warden goes berserk |
| Verus DoomKnight | Hits Warden | Lucky; Ravenous weapon, Vainglory cape, Forge helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| King's Echo | Hits Warden | Lucky; Ravenous weapon, Vainglory cape, Examen helm, even with DisableAutoEnhance on; no potions; no scroll | never |

The Lord of Order casts Order again in its rotation once it has cast it on the berserk, or
given up after 6 s. A Warden back above 30% (a new fight) gets Order held again.

**`ke-lr-ap-loo`** and **`ke-lr-sc-loo`** (two variants of one Comp, differing in the third
class; `loo-lr-vdk-ke`'s Lord of Order, and Legion Revenant keeping Warden taunted):

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| King's Echo | Hits Warden | Lucky; Ravenous weapon, Vainglory cape, Examen helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| Legion Revenant | Taunt spammer: taunts Warden whenever its scroll is ready, the whole fight | Wizard; Ravenous weapon, Lament cape, Pneuma helm, even with DisableAutoEnhance on; no potions; Scroll of Enrage | whenever its scroll is ready |
| ArchPaladin (`ke-lr-ap-loo`) | Hits Warden | Lucky; Praxis weapon, Lament cape, Forge helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| StoneCrusher (`ke-lr-sc-loo`) | Hits Warden | Fighter; Lacerate weapon, Absolution cape, Anima helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| Lord of Order | Berserk healer holding Order, as in `loo-lr-vdk-ke` | Lucky; Awe Blast weapon, Penitence cape, Forge helm, even with DisableAutoEnhance on; no potions; no scroll | never |

#### Results

One row per Attempt, from the four accounts' `ultra.attempt` reports. Accounts are
`alt1`…`alt4`; deaths are `Role @ s` since the Attempt started.

| Date | Comp | Party Layout (`alt1=Class; …`) | Outcome | Duration | Deaths (Role @ s) | Note |
|---|---|---|---|---|---|---|
| 2026-10-05 | loo-lr-sc-csh | alt1=Chrono ShadowHunter; alt2=Lord of Order; alt3=Legion Revenant; alt4=StoneCrusher | kill | 103 s (alt2, alt3); 234 s (alt1, alt4) | BerserkTaunter @ 6.9, BerserkTaunter @ 91, WardenAttacker @ 100.5, WardenAttacker @ 102.7 | The berserk cue came at about 85 s. The BerserkHealer cast Order 5 s after it; the BerserkTaunter died 6 s after it, and both WardenAttackers died as Warden died, so they fought a second Warden; at its berserk the BerserkHealer could not cast Order within 6 s. Suggest: cast Order at once on the cue (or just before the berserk HP), and potions for the WardenAttackers. |

### Ultra Engineer

#### Comps

The `Ultra Engineer comp` option picks one; blank runs `default`. Engineer can't be hit while
a Drone is up, so everyone kills the Defense Drone, then the Attack Drone, then Engineer. No
one taunts.

**`default`**:

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| Verus DoomKnight | Drone killer | Fighter; Lacerate weapon, Lament cape, Forge helm; Body Tonic, Potent Destruction Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |
| StoneCrusher | Drone killer | Fighter; Valiance weapon, Absolution cape, Anima helm; Body Tonic, Unstable Divine Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |
| Lord of Order | Drone killer | Fighter; Arcana's Concerto else Awe Blast weapon, Absolution cape; Body Tonic, Unstable Divine Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |
| King's Echo | Drone killer | Healer; Elysium else Mana Vamp weapon, Lament cape, Examen helm; Body Tonic, Potent Destruction Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |

**`loo-lr-sc-csh`** (the Recommended Group of the community "Simplified bosses guide",
without Forge helms):

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| Lord of Order | Casts its 5th skill (Order) on each new Attack Drone to debuff it, then plays drone killer | Lucky; Awe Blast weapon, Penitence cape, Examen helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| Legion Revenant | Drone killer | Wizard; Ravenous weapon, Lament cape, Pneuma helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| StoneCrusher | Drone killer | Fighter; Lacerate weapon, Absolution cape, Anima helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| Chrono ShadowHunter | Drone killer | Lucky; Valiance weapon, Vainglory cape, Examen helm, even with DisableAutoEnhance on; no potions; no scroll | never |

**`loo-lr-sc-ke`** (`default`'s fight with Legion Revenant, and the guide's Loadouts without
Forge helms):

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| Lord of Order | Drone killer | Lucky; Awe Blast weapon, Penitence cape, Examen helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| Legion Revenant | Drone killer | Wizard; Ravenous weapon, Lament cape, Pneuma helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| StoneCrusher | Drone killer | Fighter; Lacerate weapon, Absolution cape, Anima helm, even with DisableAutoEnhance on; no potions; no scroll | never |
| King's Echo | Drone killer | Lucky; Ravenous weapon, Vainglory cape, Examen helm, even with DisableAutoEnhance on; Body Tonic, Potent Destruction Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |

#### Results

One row per Attempt, from the four accounts' `ultra.attempt` reports. Accounts are
`alt1`…`alt4`; deaths are `Role @ s` since the Attempt started.

| Date | Comp | Party Layout (`alt1=Class; …`) | Outcome | Duration | Deaths (Role @ s) | Note |
|---|---|---|---|---|---|---|
| 2026-10-05 | loo-lr-sc-ke | alt1=King's Echo; alt2=Legion Revenant; alt3=StoneCrusher; alt4=Lord of Order | kill | 17 s | none | First Attempt. Only King's Echo drank potions (`c48931b4f`). |
| 2026-10-05 | loo-lr-sc-csh | alt1=Chrono ShadowHunter; alt2=Lord of Order; alt3=Legion Revenant; alt4=StoneCrusher | stopped | 116–123 s | DroneKiller @ 44.9, AttackDroneDebuffer @ 45.6, DroneKiller @ 53.6, then 13 more (AttackDroneDebuffer ×5, DroneKiller ×8) | Stopped by the developer: the party never got past the drones and Engineer stayed at full HP (1,000,000). The Lord of Order and Legion Revenant died about every 12 s. Developer suggests King's Echo over Chrono ShadowHunter. |

### Ultra Avatar Tyndarius

#### Comps

The `Ultra Avatar Tyndarius comp` option picks one; blank runs `default`. The ArchPaladin's
taunts count from the fight start it shares.

**`default`** (won its first Attempt with the layout under [Recommended layouts](#recommended-layouts)):

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| King's Echo | Hits the right orb while it is up, otherwise Tyndarius | Lucky; Ravenous weapon, Vainglory cape, Examen helm; Body Tonic, Potent Destruction Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |
| Legion Revenant | Taunts the left orb while it is up, otherwise hits Tyndarius | Wizard; Arcana's Concerto else Health Vamp weapon, Penitence else Vainglory cape, Pneuma else no helm special; no potions; Scroll of Enrage | whenever the left orb is up |
| ArchPaladin | Taunts Tyndarius; skills 1 to 3 only, never its ultimate, which breaks Righteous Seal | Fighter; Valiance weapon, Absolution cape, Forge helm; no potions; Scroll of Enrage | at 0 s of every 12 s, Righteous Seal up first |
| Lord of Order | Hits Tyndarius | Fighter; Arcana's Concerto else Awe Blast weapon, Absolution cape; no potions; no scroll | never |

The Legion Revenant takes Health Vamp without Arcana's Concerto because the left orb kept
killing it.

#### Results

One row per Attempt, from the four accounts' `ultra.attempt` reports. Accounts are
`alt1`…`alt4`; deaths are `Role @ s` since the Attempt started.

**`ke-lr-ap-loo-loop`** (`default` with the guide's loop taunt and no potions). A taunt's Focus holds
Tyndarius for only 6 s, and `default`'s 12 s taunt left him free from 6 s to 12 s, when the party died.
Same classes, Roles and Loadouts as `default`, without potions, except:

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| Lord of Order | Hits Tyndarius | Fighter; Arcana's Concerto else Awe Blast weapon, Absolution cape; no potions; Scroll of Enrage | at 6 s of every 12 s |

**`ke-lr-ap-loo-orbs`** (`ke-lr-ap-loo-loop` with the guide's orb taunts). Both orbs put Melting on the
whole party every 6 s, and it stacks. Same as `ke-lr-ap-loo-loop`, except:

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| King's Echo | Taunts and hits the right orb while it is up, then the left orb, then Tyndarius | Lucky; Ravenous weapon, Vainglory cape, Examen helm; no potions; Scroll of Enrage | whenever the right orb is up |

**`ke-lr-ap-loo-orbpairs`** (all four scrolls on the orbs). Each orb puts Melting on the whole party about
every 6 s unless it's taunted, when only its taunter gets it. A taunt lasts 6 s and a scroll takes about 12 s
to come back, so two taunters share each orb, 6 s apart. Nobody taunts Tyndarius. Loadouts as
`ke-lr-ap-loo-orbs`.

| Class | Role | Taunts |
|---|---|---|
| King's Echo | Hits the right orb, then the left orb, then Tyndarius | at 0 s of every 12 s, on its target |
| Legion Revenant | Hits the left orb, then the right orb, then Tyndarius | at 0 s of every 12 s, on its target |
| ArchPaladin | Hits the left orb, then the right orb, then Tyndarius; skills 1 to 3 only | at 6 s of every 12 s, on its target, Righteous Seal up first |
| Lord of Order | Hits the right orb, then the left orb, then Tyndarius | at 6 s of every 12 s, on its target |

**`ke-lr-ap-loo-burst`** (`ke-lr-ap-loo-orbs` with more damage on the right orb). Untaunted, Tyndarius killed
the party in about 10 s, and one taunter per orb covers only every other Melting. Same as `ke-lr-ap-loo-orbs`,
except:

| Class | Role | Taunts |
|---|---|---|
| Lord of Order | Hits the right orb, then the left orb, then Tyndarius; turns to Tyndarius for its taunt's presses | at 6 s of every 12 s, on Tyndarius |
| King's Echo | as `ke-lr-ap-loo-orbs` | Lucky; Ravenous weapon, Vainglory cape, Examen helm; no potions; Scroll of Enrage |

| Date | Comp | Party Layout (`alt1=Class; …`) | Outcome | Duration | Deaths (Role @ s) | Note |
|---|---|---|---|---|---|---|
| 2026-10-05 | ke-lr-ap-loo-burst | alt1=King's Echo; alt2=ArchPaladin; alt3=Legion Revenant; alt4=Lord of Order | stopped | 2 Attempts | first deaths at 23.6 s, then 9.8 s | After `b24929245` (King's Echo in Lucky / Ravenous gear). Closest yet: the orbs got down to 27k (right) and 119k (left) by 26 s, but at 12.4 s both were untaunted and Melting hit all four. Out of tries. |
| 2026-10-05 | ke-lr-ap-loo-burst | alt1=King's Echo; alt2=ArchPaladin; alt3=Legion Revenant; alt4=Lord of Order | stopped | 1 Attempt (a 2nd cut short) | ArchPaladin 2.1 s, Legion Revenant 4.6 s, Lord of Order 11.8 s, King's Echo 23.8 s | King's Echo still on Healer gear: the right orb only got to 100k by 26 s, the left one barely moved. |
| 2026-10-05 | ke-lr-ap-loo-orbpairs | alt1=King's Echo; alt2=ArchPaladin; alt3=Legion Revenant; alt4=Lord of Order | stopped | 1 Attempt (a 2nd cut short) | Lord of Order 9.7 s, ArchPaladin 10.8 s, Legion Revenant 11.1 s, King's Echo 27.7 s | Furthest yet: the orbs got down to 97k (left) and 33k (right). Untaunted, Tyndarius killed three by 11 s, so their 6 s taunts never came and Melting got through. |
| 2026-10-05 | ke-lr-ap-loo-orbs | alt1=King's Echo; alt2=ArchPaladin; alt3=Legion Revenant; alt4=Lord of Order | stopped | 1 Attempt | first death at 13.7 s, then the whole party | At 6.5 s both orbs were taunted and Melting hit only their taunters; their taunts ran out at 9–10 s and the scrolls weren't back, so at 12.7 s Melting hit all four. |
| 2026-10-05 | ke-lr-ap-loo-loop | alt1=King's Echo; alt2=ArchPaladin; alt3=Legion Revenant; alt4=Lord of Order | stopped | 1 Attempt (a 2nd cut short) | first death at 13.9 s, then the whole party | Tyndarius stayed taunted throughout, yet the party died: both orbs' Melting stacked (cast at 6.6 s and 12.6 s). The left orb barely took damage; the right one was at about half. |
| 2026-10-05 | default | alt1=King's Echo; alt2=ArchPaladin; alt3=Legion Revenant; alt4=Lord of Order | stopped | 5 Attempts | first deaths at 14, 15.9, 13.8, 23.4, 16 s, then the whole party | After `4d22e513b` (ArchPaladin seals and holds its ultimate; Loadouts apply with DisableAutoEnhance on). No potions but King's Echo's. Tyndarius got no lower than 84%. |
| 2026-10-05 | default | alt1=King's Echo; alt2=Legion Revenant; alt3=ArchPaladin; alt4=Lord of Order | stopped | 5 Attempts, about 7 min | first death each Attempt at 12–15 s (once 25 s), then the whole party | Five Wipes, stopped. Only King's Echo drank potions (`c48931b4f`); the earlier win had Body Tonic on all four, so the lost max HP is the likely cause. Tyndarius got no lower than 68%. |

### Ultra Nulgath

#### Comps

The `Ultra Nulgath comp` option picks one; blank runs `default`. Timed taunts count from the
fight start the earliest taunter shares.

**`dot-lr-ap-loo`** (recommended; beat Nulgath in about 2.5 minutes with no deaths):

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| Dragon of Time | Hits each new Overfiend Blade for about 1.5 s, otherwise Nulgath | Wizard; Elysium weapon, Vainglory cape, Pneuma helm, even with DisableAutoEnhance on; Unstable Malevolence Elixir, Sage Tonic, Potent Honor Potion, drunk before the fight only; no scroll | never |
| Legion Revenant | Taunts Nulgath | own gear; no potions; Scroll of Enrage | at 0 s of every 10 s |
| ArchPaladin | Taunts Nulgath | own gear; no potions; Scroll of Enrage | at 5 s of every 10 s |
| Lord of Order | Hits Nulgath | own gear; no potions; no scroll | never |

The potion buyer can't make Unstable Malevolence Elixir; keep some on the Dragon of Time.

**`loo-lr-sc-dot`** (the Recommended Group of the community "Simplified bosses guide";
killed Nulgath in about 63 s on its first Attempt):

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| Lord of Order | Lead taunter: taunts Nulgath | Lucky; Awe Blast weapon, Penitence cape, Forge helm, even with DisableAutoEnhance on; no potions; Scroll of Enrage | about 1.5 s into the fight, never at 0 s; then each time the cue taunter's taunt on Nulgath ends |
| Legion Revenant | Cue taunter: taunts Nulgath | Wizard; Arcana's Concerto weapon, Lament cape, Pneuma helm, even with DisableAutoEnhance on; no potions; Scroll of Enrage | on every "Behold the power of the Abyss!" |
| StoneCrusher | Hits Nulgath | Fighter; Lacerate weapon, Absolution cape, Anima helm, even with DisableAutoEnhance on; Sage Tonic, Potent Honor Potion, drunk before the fight only; no scroll | never |
| Dragon of Time | Hits each new Overfiend Blade for about 1.5 s, otherwise Nulgath | as in `dot-lr-ap-loo` | never |

The two taunters follow Nulgath, not a timer. The cue taunter taunts when Nulgath says a line
containing "Behold the power of the Abyss", in any case. The lead taunter sees that taunt as a
new Focus aura on Nulgath and taunts again once the Focus is gone. If no new Focus shows up
within 5 s of the line, it taunts then.

**`default`** (it wiped repeatedly in testing; prefer `dot-lr-ap-loo`):

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| Lord of Order | Taunts Nulgath | Fighter; Arcana's Concerto else Valiance weapon, Absolution cape; Body Tonic, Unstable Divine Elixir; Scroll of Enrage | at 0 s of every 15 s, not while it has Contract of Despair |
| StoneCrusher | Taunts Nulgath | Fighter; Valiance weapon, Absolution cape; Body Tonic, Unstable Divine Elixir; Scroll of Enrage | at 5 s of every 15 s, not while it has Contract of Despair |
| Verus DoomKnight | Hits the Blade, and Nulgath from 1 s before its taunt until it ends | Fighter; Lacerate weapon, Lament cape, Forge helm; Body Tonic, Potent Destruction Elixir; Scroll of Enrage | at 10 s of every 15 s, not while it has Contract of Despair |
| King's Echo | Hits the Blade while it is up, otherwise Nulgath | Healer; Elysium else Mana Vamp weapon, Lament cape, Examen helm; Body Tonic, Potent Destruction Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |

#### Results

One row per Attempt, from the four accounts' `ultra.attempt` reports. Accounts are
`alt1`…`alt4`; deaths are `Role @ s` since the Attempt started.

| Date | Comp | Party Layout (`alt1=Class; …`) | Outcome | Duration | Deaths (Role @ s) | Note |
|---|---|---|---|---|---|---|
| 2026-10-05 | loo-lr-sc-dot | alt1=Dragon of Time; alt2=Lord of Order; alt3=Legion Revenant; alt4=StoneCrusher | kill | 63 s | CueTaunter @ 51.5 | The cue line came every 15 s (4 cues) and the Legion Revenant taunted on each. The Lord of Order never saw a Focus aura on Nulgath, so every re-taunt came from the 5 s fallback after the cue. Suggest: find the aura a Scroll of Enrage taunt really shows, or make the 5 s timing explicit. The Legion Revenant died mid-taunt at 51.5 s; no Wipe. |

### Ultra Speaker

#### Comps

The `Ultra Speaker comp` option picks one; blank runs `default`. Everyone hits the Speaker
from the bottom-left safe box, and steps into the equalize zone when it has Corruption or
Somber. The taunts follow the Speaker's chat lines, not a timer.

**`default`**:

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| ArchPaladin | Listen taunter | Fighter; Lacerate weapon; Body Tonic, Potent Destruction Elixir; Scroll of Enrage | on every "You shall listen." |
| Lord of Order | Truth taunter 1 | Fighter; Valiance weapon; Body Tonic, Unstable Divine Elixir; Scroll of Enrage | on the 3rd, 6th, 9th… "I will make you see the truth." |
| StoneCrusher | Truth taunter 2 | Fighter; Valiance weapon, Absolution cape; Body Tonic, Unstable Divine Elixir; Scroll of Enrage | on the 1st, 4th, 7th… "I will make you see the truth." |
| Verus DoomKnight | Truth taunter 3; also casts Decay on "All stand equal beneath the eyes of the Eternal." | Fighter; Praxis weapon, Penitence cape; Body Tonic, Potent Destruction Elixir; Scroll of Enrage | on the 2nd, 5th, 8th… "I will make you see the truth." |

#### Results

One row per Attempt, from the four accounts' `ultra.attempt` reports. Accounts are
`alt1`…`alt4`; deaths are `Role @ s` since the Attempt started.

| Date | Comp | Party Layout (`alt1=Class; …`) | Outcome | Duration | Deaths (Role @ s) | Note |
|---|---|---|---|---|---|---|

## Recommended layouts

Each of these won its first Attempt:

```
Tyndarius layout: alt1=King's Echo; alt2=Legion Revenant; alt3=ArchPaladin; alt4=Lord of Order
Nulgath comp:     dot-lr-ap-loo
Nulgath layout:   alt1=Dragon of Time; alt2=Legion Revenant; alt3=ArchPaladin; alt4=Lord of Order
```

For the other bosses, put each account on the class it is best geared for from the role
table above. If an account doesn't own a role's class, either list a party where every role
is covered or leave the layout blank and let the automatic assignment try.

## Wipes

Tyndarius, Nulgath, Darkon and Gramiel retreat when the whole party dies within 15
seconds, wait for everyone, and refight, up to 10 times. Roles and classes stay the same.
