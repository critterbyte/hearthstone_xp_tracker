# XP formula

Hearthstone does not write reward track XP to any log file, and Blizzard has not published the formula. This page describes the formula the plugin uses and how it was found.

Each statement is marked:

- **Measured:** found by this project in our own games.
- **According to [n]:** taken from a source listed at the end of this page. Not checked by this project.

## How it was measured

- Games were played on 7 and 8 November 2026, during the *Reign of the Black Empire* expansion, without a Tavern Pass.
- After every game, the XP shown by the game was written down at once.
- The game times were read from Hearthstone's `Power.log`, to a hundredth of a second.
- 75 games have the XP noted: 67 Ranked, 3 Casual, 5 Tavern Brawl.
  - 64 of them determine the rates.
  - 11 short games with 0 XP determine the minimum game length.

## Formula

```
XP = round(xpSeconds × rate)
```

- **Measured:** rounding is half away from zero.
- **Measured:** XP grows linearly with every second. There is no base amount per game, no rounding to blocks of time and no slowdown in long games. The longest measured game, 24:28, still matched the rate.
- **Measured:** `xpSeconds` starts with the first turn, when the mulligan ends. It stops when the game ends. AFK turns are taken out (see [Which time counts](#which-time-counts)).
- **Measured:** a game with less than **60 seconds** of XP time gives **no XP at all**. From 60 seconds on, it gives the full amount.
  - Examples: 59.89 s gave 0 XP, and 60.78 s gave 6 XP.
  - The limit uses the XP time after AFK turns are taken out. A game of 2:34 with only 59.23 s of XP time gave 0 XP.
  - It does not matter how many turns were played or who conceded. The opponent conceding after 34 s gave 0 XP.
- **According to [1]:** XP time is capped at about **30 minutes** per game. Not measured yet. With this cap, a Ranked win is worth at most about 235 XP.

## Rates

| Mode | Win | Loss |
|---|---:|---:|
| Ranked (Standard and Wild) | 0.1305 XP/s (470 XP/h) | 0.0932 XP/s (336 XP/h) |
| Casual and Tavern Brawl | 0.0932 XP/s (336 XP/h) | 0.0666 XP/s (240 XP/h) |

All values in this table are **measured**.

- A win pays 1.40 times a loss.
- Casual and Tavern Brawl pay 0.71 times Ranked.
- Standard and Wild pay the same.

These rates are the middle of ranges that fit every measured game:

| Case | Range (XP/s) | Games |
|---|---|---:|
| Ranked win | 0.13047 to 0.13058 | 27 |
| Ranked loss | 0.09312 to 0.09332 | 29 |
| Casual or Tavern Brawl win | 0.09306 to 0.09351 | 5 |
| Casual or Tavern Brawl loss | 0.06649 to 0.06837 | 3 |

The sources agree in general:

- **According to [2]:** "XP is time-based", "winning matters", and Ranked Standard and Wild give "around 400/Hour". The measured win and loss rates average about 400 XP per hour.
- **According to [3]:** Battlegrounds gives about 300 XP per hour. Battlegrounds is not measured by this project.

## Which time counts

| Situation | Counts? |
|---|---|
| Own turn in which you play at least one card, attack or hero power, even if the turn timer runs out | yes |
| Own turn without any action, ended by clicking "End Turn" | yes |
| Own turn without any action, not ended by "End Turn" (the timer runs out) | **no**, and the opponent turn that follows does not count either |
| Own turn without any action in which the game ends (for example the opponent concedes) | yes |
| Opponent turns, including turns the opponent lets run out, with or without actions | yes |
| Mulligan | no |

All rows are **measured**. Other clicks, such as picking up a card or using an emote, do not make a turn count.

**According to [4]:** since patch 29.6 (2024), "extended periods of inactivity will now reward less XP". Normal actions, "or even just hitting the end turn button", are not affected. This matches the measured rule.

**Measured:** when a turn ends by the timer without any action, the server lowers that player's next turn limit to 10 seconds. The `Power.log` shows this as `TAG_CHANGE ... tag=TIMEOUT value=10`. The plugin uses this marker to find AFK turns. If the next own turn is also idle, it is excluded too. As soon as you play a card again, the turn counts fully, even under the 10-second limit.

## What does not matter

All **measured**:

- Conceding, in your own turn or in the opponent's turn
- Dying during your own turn
- The opponent conceding or disconnecting
- The opponent being AFK: their idle turns still count fully for you, even several in a row
- A Rewind card resetting the game

## Log markers used

| Moment | `Power.log` entry |
|---|---|
| Mulligan starts | `TAG_CHANGE Entity=GameEntity tag=STEP value=BEGIN_MULLIGAN` |
| First turn starts (XP time starts) | `TAG_CHANGE Entity=GameEntity tag=STEP value=MAIN_READY` |
| Turn changes | `TAG_CHANGE Entity=<player> tag=CURRENT_PLAYER value=1` |
| Turn limit of a player | `TAG_CHANGE Entity=<player> tag=TIMEOUT value=45/55/75` (10 after an AFK turn) |
| Game ends | `TAG_CHANGE Entity=GameEntity tag=STATE value=COMPLETE` |

Timestamps in the log have seven decimal places (100-nanosecond ticks). Whole seconds are not precise enough to tell the rates apart.

## Not covered yet

- **Arena, Battlegrounds, Twist and solo adventures** are not measured. The plugin shows nothing in these modes.
  - **According to [5]:** Casual, Ranked, Arena, Battlegrounds, Tavern Brawls and Adventures give XP.
  - **According to [5]:** Friendly Challenges do not give XP.
- **Tavern Pass:** **according to [6]**, the Tavern Pass gives an XP boost of 10%, rising to 15% and 20% on the track. **According to [7]**, the boost applies to XP from any source. It is not measured whether the boost multiplies these rates. All measurements were made without it.
- **30-minute cap:** not measured, see above.
- **Event XP:** event reward tracks have their own XP. Not measured.

## Sources

1. August Dean Ayala (Hearthstone game director), tweet, 11 November 2020: "There is an XP cap on each game around 30 min." <https://twitter.com/IksarHS/status/1326615786878767105>
2. August Dean Ayala, tweet, 11 November 2020: "There are different values for different modes, winning matters. XP is time-based. Ranked Wild/Standard is around 400/Hour." <https://twitter.com/IksarHS/status/1326613836531597314>
3. Chadd Nervig (Hearthstone developer), tweet, 11 November 2020: "Battlegrounds is around ~300xp/hr." <https://twitter.com/Celestalon/status/1326626763766550528>
4. Blizzard, *29.6 Patch Notes*, 2024. <https://news.blizzard.com/en-us/hearthstone/24108513/29-6-patch-notes>
5. Blizzard, *19.0 Patch Notes*, 11 November 2020. <https://hearthstone.blizzard.com/en-us/news/23557139>
6. Blizzard, *Reign of the Black Empire Rewards Track Refresh*, 2026. <https://news.blizzard.com/en-us/article/24309582/>
7. Hearthstone Wiki, *Rewards Track*. <https://hearthstone.wiki.gg/wiki/Rewards_Track>

Sources 1 to 3 and 5 are from the launch of the reward track in 2020. No newer source restates or changes these numbers. Source 4 is the only later official change to game XP that was found.
