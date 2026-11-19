# XP formula

Hearthstone does not write reward track XP to any log file, and Blizzard has not published the formula. This page describes the formula the tracker uses and how it was found.

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
- Ranked games until 12 November 2026 narrowed the Ranked ranges further: 55 wins and 51 losses with exact log times fit the ranges below.
- Arena was measured on 14 November 2026 in one run: 3 wins and 1 loss, without a Tavern Pass and paid with in-game gold.

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
| Ranked (Standard and Wild) | 0.13054 XP/s (470 XP/h) | 0.0932 XP/s (336 XP/h) |
| Casual, Tavern Brawl and Arena | 0.0932 XP/s (336 XP/h) | 0.0666 XP/s (240 XP/h) |

All values in this table are **measured**.

- A win pays 1.40 times a loss.
- Casual, Tavern Brawl and Arena pay 0.71 times Ranked.
- **Measured:** Arena pays exactly like Casual. Wins of 771.81 s, 1101.72 s and 712.97 s gave 72, 103 and 66 XP (0.0932 gives 71.93, 102.68 and 66.45), a loss of 1163.04 s gave 77 XP (0.0666 gives 77.46). With the Ranked rates it would have been 101, 144, 93 and 108 XP. Event XP follow the same formula as in the other modes.
- **Measured:** a Tavern Brawl against the computer (PvE) gives nothing. Two losses of 1:39 and 4:29 against the Innkeeper gave 0 XP and 0 event XP. Wins against the computer are not measured yet. The tracker recognizes such games by the opponent's missing account in the log (`GameAccountId=[hi=0 lo=0]`).
- Standard and Wild pay the same.

These rates are the middle of ranges that fit every measured game:

| Case | Range (XP/s) | Games |
|---|---|---:|
| Ranked win | 0.130505 to 0.13058 | 55 |
| Ranked loss | 0.09317 to 0.09322 | 51 |
| Casual, Tavern Brawl or Arena win | 0.09306 to 0.093272 | 8 |
| Casual, Tavern Brawl or Arena loss | 0.06649 to 0.066636 | 4 |

- **Measured:** the Arena games narrowed the shared ranges: a win of 66 XP / 712.97 s allows at most 0.093272 (before: 0.09351), only 0.05 XP from the rounding step; a loss of 77 XP / 1163.04 s allows at most 0.066636 (before: 0.06837). The win range overlaps the Ranked loss range (0.09317 to 0.09322), as expected if both rates are the same.

- **Measured:** the lower limit of the Ranked win rate comes from a win of 425.27 s that gave 56 XP. With 0.1305 it would have been 55.498, so 55. The game was only 0.02 seconds short of the rounding limit. If the server counts a game a few hundredths of a second longer than the log timestamps show, a slightly lower rate would fit too.
- With these ranges, a win pays 1.40 times a loss only if the loss rate is at least 0.093218.

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

**Measured:** when a turn ends by the timer without any action, the server lowers that player's next turn limit to 10 seconds. The `Power.log` shows this as `TAG_CHANGE ... tag=TIMEOUT value=10`. The tracker uses this marker to find AFK turns. If the next own turn is also idle, it is excluded too. As soon as you play a card again, the turn counts fully, even under the 10-second limit.

## What does not matter

All **measured**:

- Conceding, in your own turn or in the opponent's turn
- Dying during your own turn
- The opponent conceding or disconnecting
- The opponent being AFK: their idle turns still count fully for you, even several in a row
- A Rewind card resetting the game

## Event XP

During a Hearthstone event, games also give event XP for the event's own reward track. All statements are **measured** in the event *30 Years of Terror* (November 2026), in 9 games in Ranked and Casual:

```
eventXp = round(xpSeconds / 60)
```

- One event XP per minute of XP time. 30 seconds and more count as a full minute: 12:29.71 gave 12, 33.21 s gave 1, 27.14 s gave 0.
- The XP time is the same as for reward track XP: from the first turn, without the mulligan, without AFK turns and the opponent turns after them. In three games the time including the mulligan or the AFK turns would have given one more.
- There is no 60-second minimum: a concede after 36.52 s gave 0 reward track XP but 1 event XP.
- A win gives the same as a loss, and Casual gives the same as Ranked.
- Not measured yet: whether event XP is capped at 30 minutes, and other modes.

**According to [8]**, event XP "can also be earned by just playing regularly", and event quests are the fastest way to complete the event track.

## Tavern Pass

All rates above were measured **without** a Tavern Pass. They are the base rates.

> **The Tavern Pass boost is calculated, not measured.** The tracker applies the boost from the sources below by calculation only. No game with a Tavern Pass has been measured. It is therefore not known:
>
> - whether the boost applies to the XP from game time at all
> - whether the game rounds before or after adding the boost
> - whether the boost changes the 60-second minimum, the 30-minute cap or event XP
>
> Measuring this needs a Tavern Pass, which costs real money. This project follows a strict **free-to-play policy**: it spends no money on the game, so it does not buy a pass for measurements. Players who own a pass can help: see [Contributing measurements](../CONTRIBUTING.md#contributing-measurements).

- **According to [6]:** the Tavern Pass gives an XP boost of 10%, rising to 15% and 20% on the track. This official source does not name the levels.
- **According to [7] and [9]:** the boost grows with the rewards track level. In *Reign of the Black Empire* and the three expansions before it:

  | Rewards track level | XP boost |
  |---|---:|
  | 1 to 19 (from the purchase) | 10% |
  | 20 to 69 | 15% |
  | 70 and higher | 20% |

  Buying the pass later unlocks all paid rewards up to the current level, so the higher boost applies at once. Older guides list the 20% step at level 35.
- **According to [7]:** the boost applies to XP from any source, achievements included.
- **Assumed, not measured:** the boost also applies to the XP from game time, and it is applied before rounding:

  ```
  XP = round(xpSeconds × rate × (1 + boost))
  ```

  Example: a 378.43 s Ranked win gives 49.40 XP without the pass, 49.40 × 1.15 = 56.81 → **57** with 15%. Rounding first would give 56.
- **Assumed:** the boost does not change event XP. Not measured.
- The 60-second minimum and the 30-minute cap stay the same.

The tracker asks for the pass and the rewards track level in the dashboard and stores the boost with each game. With a pass, the XP the tracker shows can differ by about 1 XP per game from the XP the game gives, if the game rounds in another order. A few measured games with the pass would show whether these assumptions hold.

## Log markers used

| Moment | `Power.log` entry |
|---|---|
| Mulligan starts | `TAG_CHANGE Entity=GameEntity tag=STEP value=BEGIN_MULLIGAN` |
| First turn starts (XP time starts) | `TAG_CHANGE Entity=GameEntity tag=STEP value=MAIN_READY` |
| Turn changes | `TAG_CHANGE Entity=<player> tag=CURRENT_PLAYER value=1` |
| Turn limit of a player | `TAG_CHANGE Entity=<player> tag=TIMEOUT value=45/55/75` (10 after an AFK turn) |
| Game ends | `TAG_CHANGE Entity=GameEntity tag=STATE value=COMPLETE` |

Timestamps in the log have seven decimal places (100-nanosecond ticks). Whole seconds are not precise enough to tell the rates apart.

The timestamps carry only the time of day, not the date. The tracker takes the date from the system clock: a line belongs to the latest day on which its time is not in the future. This is right for every game of the last 24 hours, also when Hearthstone runs past midnight or stays open overnight. Known limit: if the tracker starts more than 24 hours after the last game of a Hearthstone session that is still open, that one game is read with a later date.

## Not covered yet

- **Battlegrounds, Twist and solo adventures** are not measured. The tracker shows nothing in these modes. Ranked and Casual count as measured in the formats Standard and Wild only; a game in another format, such as Twist, is stored without XP.
  - **According to [5]:** Casual, Ranked, Arena, Battlegrounds, Tavern Brawls and Adventures give XP.
  - **According to [5]:** Friendly Challenges do not give XP.
- **Tavern Pass:** not measured, because of the project's free-to-play policy. The tracker calculates the boost as described in [Tavern Pass](#tavern-pass); the rounding with the boost is assumed.
- **30-minute cap:** not measured, see above.

## Sources

1. August Dean Ayala (Hearthstone game director), tweet, 11 November 2020: "There is an XP cap on each game around 30 min." <https://twitter.com/IksarHS/status/1326615786878767105>
2. August Dean Ayala, tweet, 11 November 2020: "There are different values for different modes, winning matters. XP is time-based. Ranked Wild/Standard is around 400/Hour." <https://twitter.com/IksarHS/status/1326613836531597314>
3. Chadd Nervig (Hearthstone developer), tweet, 11 November 2020: "Battlegrounds is around ~300xp/hr." <https://twitter.com/Celestalon/status/1326626763766550528>
4. Blizzard, *29.6 Patch Notes*, 2024. <https://news.blizzard.com/en-us/hearthstone/24108513/29-6-patch-notes>
5. Blizzard, *19.0 Patch Notes*, 11 November 2020. <https://hearthstone.blizzard.com/en-us/news/23557139>
6. Blizzard, *Reign of the Black Empire Rewards Track Refresh*, 2026. <https://news.blizzard.com/en-us/article/24309582/>
7. Hearthstone Wiki, *Rewards Track*. <https://hearthstone.wiki.gg/wiki/Rewards_Track>
8. Blizzard, *24.6 Patch Notes*, 1 November 2022: "Event XP can also be earned by just playing regularly". <https://hearthstone.blizzard.com/en-us/news/23852694/24-6-patch-notes>
9. senseos.gg, *Hearthstone Rewards Track: XP per Level, Gold and Reset*, updated 7 November 2026: "The paid row of all four tables also lists a 15% XP Boost at level 20 and a 20% XP Boost at level 70." <https://senseos.gg/guides/hearthstone/how-rewards-track-works/>

Sources 1 to 3 and 5 are from the launch of the reward track in 2020. No newer source restates or changes these numbers. Source 4 is the only later official change to game XP that was found.
