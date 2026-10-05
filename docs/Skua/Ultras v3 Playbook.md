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
Avatar Tyndarius, Ultra Nulgath and Ultra Speaker) have a `<Boss> comp` option in
`DoAllUltras`, next to `<Boss> layout`. Set it to a Comp's name; leave it blank to run the
boss's `default` Comp. An unknown name stops the bot and lists the boss's Comps. Like the
layout, a single boss script reads the saved option.

Each class's Loadout is applied before the fight: the enhancements once, the potions bought,
equipped and drunk before each Attempt, then the scroll equipped. Where a special lists a
fallback ("else"), the first one unlocked is used.

Each Attempt, from engaging the boss to its kill, a Wipe or the bot stopping, ends with an
`ultra.attempt` Script Report on every account: the boss, Comp, class and Role, the start
and end, the outcome (`kill`, `wipe` or `stopped`), the boss's HP at the end, and that
account's deaths in seconds since the start. Ultra Speaker has no Wipe detection, so its
Attempts end in `kill` or `stopped`.

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
  classes (any class for Ezrajal and Engineer). For a boss with Comps, every class in the
  layout must be one of the chosen Comp's 4 classes. If not, the bot stops and says what's
  wrong.
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
| Ezrajal | 4 × DPS (Verus DoomKnight, StoneCrusher, Lord of Order, King's Echo). Any class works with a layout. Everyone stops attacking during Counter Attack. |
| Warden | Taunter 1 (Verus DoomKnight), Taunter 2 (Lord of Order), DPS (King's Echo), DPS (StoneCrusher). Taunters alternate every 5 s. |
| Engineer | 4 × DPS (Verus DoomKnight, StoneCrusher, Lord of Order, King's Echo), drones first. Any class works with a layout. |
| Tyndarius | One Comp, see below. |
| Drakath | Taunters 1–3 (ArchPaladin, Lord of Order, Shaman), 4 s apart, DPS (StoneCrusher). |
| Nulgath | Two Comps, see below. |
| Drago | Taunter 1 and Taunter 2 (Lord of Order, Verus DoomKnight) hit the right summon and taunt the left one, 2 × DPS (StoneCrusher, King's Echo). |
| Darkon | Taunter 1 (Verus DoomKnight), Taunter 2 (Lord of Order), 2 × DPS (StoneCrusher, King's Echo). |
| Dage | Taunter 1 (Verus DoomKnight), Taunter 2 (ArchPaladin), Decay (Lord of Order, Scroll of Decay on Legionnaire), DPS (King's Echo). |
| Speaker | One Comp, see below. |
| Gramiel | Crystal taunters: left T1 (StoneCrusher), right T1 (ArchPaladin), left T2 (Lord of Order), right T2 (ArchFiend); all four taunt Gramiel in turn. |

### Ultra Avatar Tyndarius

#### Comps

The `Ultra Avatar Tyndarius comp` option picks one; blank runs `default`. The ArchPaladin's
taunts count from the fight start it shares.

**`default`** (won first try with the layout under [Recommended layouts](#recommended-layouts)):

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| King's Echo | Hits the right orb while it is up, otherwise Tyndarius | Healer; Elysium else Mana Vamp weapon, Lament cape, Examen helm; Body Tonic, Potent Destruction Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |
| Legion Revenant | Taunts the left orb while it is up, otherwise hits Tyndarius | Wizard; Arcana's Concerto else Health Vamp weapon, Penitence else Vainglory cape, Pneuma else no helm special; Body Tonic, Potent Destruction Elixir; Scroll of Enrage | whenever the left orb is up |
| ArchPaladin | Taunts Tyndarius | Fighter; Valiance weapon, Absolution cape, Forge helm; Body Tonic, Potent Destruction Elixir; Scroll of Enrage | at 0 s of every 12 s |
| Lord of Order | Hits Tyndarius | Fighter; Arcana's Concerto else Awe Blast weapon, Absolution cape; Body Tonic, Unstable Divine Elixir, Potent Malice Potion (if more than 30) else Potent Honor Potion; no scroll | never |

The Legion Revenant takes Health Vamp without Arcana's Concerto because the left orb kept
killing it.

#### Results

One row per Attempt, from the four accounts' `ultra.attempt` reports. Accounts are
`alt1`…`alt4`; deaths are `Role @ s` since the Attempt started.

| Date | Comp | Party Layout (`alt1=Class; …`) | Outcome | Duration | Deaths (Role @ s) | Note |
|---|---|---|---|---|---|---|

### Ultra Nulgath

#### Comps

The `Ultra Nulgath comp` option picks one; blank runs `default`. Taunts count from the fight
start the earliest taunter shares.

**`dot-lr-ap-loo`** (recommended; beat Nulgath in about 2.5 minutes with no deaths):

| Class | Role | Loadout | Taunts |
|---|---|---|---|
| Dragon of Time | Hits each new Overfiend Blade for about 1.5 s, otherwise Nulgath | Wizard; Elysium weapon, Vainglory cape, Pneuma helm, even with DisableAutoEnhance on; Unstable Malevolence Elixir, Sage Tonic, Potent Honor Potion; no scroll | never |
| Legion Revenant | Taunts Nulgath | own gear; no potions; Scroll of Enrage | at 0 s of every 10 s |
| ArchPaladin | Taunts Nulgath | own gear; no potions; Scroll of Enrage | at 5 s of every 10 s |
| Lord of Order | Hits Nulgath | own gear; no potions; no scroll | never |

The potion buyer can't make Unstable Malevolence Elixir; keep some on the Dragon of Time.

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

These two won first try:

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
