# XP formula

Hearthstone does not write reward track XP to any log file. The formula below was measured in November 2026: after every game the XP shown in the client was noted, and the game duration was read from Hearthstone's `Power.log`. The data covers more than 40 games in Ranked (Wild and Standard), Casual and Tavern Brawl.

## Formula

```
XP = round(xpSeconds × rate)
```

- Rounding is half away from zero.
- `xpSeconds` starts with the first turn (end of the mulligan) and ends when the game ends, minus excluded turns (see below).
- XP grows linearly per second. There is no base amount, no rounding to blocks of time, and no slowdown: a Ranked loss of 15:33 and a Tavern Brawl win of 17:17 still matched the same rates.
- XP time is reported to be capped at 30 minutes per game. This has not been verified yet. With the cap, a Ranked win is worth at most about 235 XP.

## Rates

| Mode | Win | Loss |
|---|---:|---:|
| Ranked (Wild and Standard) | 0.1308 XP/s (471 XP/h) | 0.0932 XP/s (336 XP/h) |
| Casual and Tavern Brawl | 0.0932 XP/s (336 XP/h) | 0.0666 XP/s (240 XP/h) |

- A win pays 1.40 times a loss.
- Casual and Tavern Brawl pay 0.71 times Ranked.
- Wild and Standard pay the same.

Ranges that fit every measured game:

| Case | Range (XP/s) |
|---|---|
| Ranked win | 0.13047 to 0.13106 |
| Ranked loss | 0.09312 to 0.09332 |
| Casual / Tavern Brawl win | 0.09306 to 0.09351 |
| Casual / Tavern Brawl loss | 0.06649 to 0.06837 |

## Which time counts

| Situation | Counts? |
|---|---|
| Own turn with at least one played option (card, attack, hero power), even if the turn timer runs out | yes |
| Own turn without any option, ended by clicking "End Turn" | yes |
| Own turn without any option and without clicking "End Turn" (timer runs out); other clicks such as touching a card do not make the turn active | **no**, and the following opponent turn does not count either |
| Own turn without any option in which the game ends (for example the opponent concedes) | yes |
| Opponent turns, including turns the opponent lets run out with or without actions | yes |
| Mulligan | no |

When a turn ends by timeout without any action, the server sets that player's next turn limit to 10 seconds (`TAG_CHANGE ... tag=TIMEOUT value=10`). This marker identifies the excluded turns without having to know about clicks.

## What does not matter

- Conceding (own turn or opponent turn)
- Dying during your own turn
- The opponent conceding or disconnecting
- The opponent idling: a 48-second opponent turn without any action (marked AFK by the server) still counted fully; the game matched 15 XP only with that time included
- A Rewind card resetting the game

## Log markers used

| Moment | `Power.log` entry |
|---|---|
| Mulligan starts | `TAG_CHANGE Entity=GameEntity tag=STEP value=BEGIN_MULLIGAN` |
| First turn starts (XP time starts) | `TAG_CHANGE Entity=GameEntity tag=STEP value=MAIN_READY` |
| Turn changes | `TAG_CHANGE Entity=<player> tag=CURRENT_PLAYER value=1` |
| Options played in a turn | `TAG_CHANGE Entity=<player> tag=NUM_OPTIONS_PLAYED_THIS_TURN` |
| Turn limit of a player | `TAG_CHANGE Entity=<player> tag=TIMEOUT value=45/55/75` (10 after an AFK turn) |
| Game ends | `TAG_CHANGE Entity=GameEntity tag=STATE value=COMPLETE` |

Timestamps in the log have seven decimal places (100-nanosecond ticks). Truncating them to whole seconds is not precise enough to tell the rates apart.

## Open questions

- Is XP time really capped at 30 minutes, and does the cap include the mulligan?
- Is there a minimum game length for XP? A game that ended after 23 seconds without an own turn showed no XP, a game of 1:55 with one own turn did.
- Does the Tavern Pass XP boost multiply the rates?
- Arena and Battlegrounds have not been measured.
