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
  classes (any class for Ezrajal and Engineer). If not, the bot stops and says what's wrong.
- Leave a layout blank to keep the automatic assignment: the accounts share which role
  classes they own and the script hands the roles out.

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
| Tyndarius | Right orb then Tyndarius (King's Echo), left orb taunter (Legion Revenant: Wizard, Arcana's Concerto weapon, Penitence cape, Pneuma helm), Tyndarius taunter every 12 s (ArchPaladin), Tyndarius attacker (Lord of Order). |
| Drakath | Taunters 1–3 (ArchPaladin, Lord of Order, Shaman), 4 s apart, DPS (StoneCrusher). |
| Nulgath | Two role sets, see below. |
| Drago | Taunter 1 and Taunter 2 (Lord of Order, Verus DoomKnight) hit the right summon and taunt the left one, 2 × DPS (StoneCrusher, King's Echo). |
| Darkon | Taunter 1 (Verus DoomKnight), Taunter 2 (Lord of Order), 2 × DPS (StoneCrusher, King's Echo). |
| Dage | Taunter 1 (Verus DoomKnight), Taunter 2 (ArchPaladin), Decay (Lord of Order, Scroll of Decay on Legionnaire), DPS (King's Echo). |
| Speaker | Listen taunter (ArchPaladin), truth taunters 1–3 (Lord of Order, StoneCrusher, Verus DoomKnight). |
| Gramiel | Crystal taunters: left T1 (StoneCrusher), right T1 (ArchPaladin), left T2 (Lord of Order), right T2 (ArchFiend); all four taunt Gramiel in turn. |

### Ultra Nulgath role sets

The layout picks the set: a layout that names only Dragon of Time, Legion Revenant,
ArchPaladin and Lord of Order plays the party set. Blank, or the default classes, plays the
default set. Mixing the two stops the bot.

**Party set** (recommended; beat Nulgath in about 2.5 minutes with no deaths):

| Class | Role | Gear and potions |
|---|---|---|
| Dragon of Time | Hits each new Overfiend Blade for about 1.5 s, otherwise Nulgath | Wizard, Elysium weapon, Vainglory cape, Pneuma helm; Unstable Malevolence Elixir, Sage Tonic, Potent Honor Potion |
| Legion Revenant | Taunts Nulgath at 0 s of every 10 s | Scroll of Enrage, own gear, no potions |
| ArchPaladin | Taunts Nulgath at 5 s of every 10 s | Scroll of Enrage, own gear, no potions |
| Lord of Order | Hits Nulgath | Own gear, no potions |

The potion buyer can't make Unstable Malevolence Elixir; keep some on the Dragon of Time.

**Default set:** Taunter 1 (Lord of Order), Taunter 2 (StoneCrusher), Taunter 3 that also
hits the Blade (Verus DoomKnight), Blade attacker (King's Echo). It wiped repeatedly in
testing; prefer the party set.

## Recommended layouts

These two won first try:

```
Tyndarius layout: alt1=King's Echo; alt2=Legion Revenant; alt3=ArchPaladin; alt4=Lord of Order
Nulgath layout:   alt1=Dragon of Time; alt2=Legion Revenant; alt3=ArchPaladin; alt4=Lord of Order
```

For the other bosses, put each account on the class it is best geared for from the role
table above. If an account doesn't own a role's class, either list a party where every role
is covered or leave the layout blank and let the automatic assignment try.

## Wipes

Tyndarius, Nulgath, Darkon and Gramiel retreat when the whole party dies within 15
seconds, wait for everyone, and refight, up to 10 times. Roles and classes stay the same.
