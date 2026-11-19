# Usage

The tracker runs in the background, with a gold "XP" icon in the notification area at the bottom right of the taskbar. It has three parts:

- the **overlay** in the game
- the **dashboard** window
- the **game history** file

The texts in quotes, such as "XP time:", are the texts the tracker shows.

## Overlay

The overlay is a small box at the bottom right, above the Hearthstone settings button (the gear in the bottom right corner). In the dashboard you can move it to the bottom left, above your portrait ("You" with your rank), instead. Labels are on the left. Values are on the right, in blue bars.

The overlay is shown only while Hearthstone is the active window. It never takes the focus, and every click goes through it to the game.

The overlay is shown only in **Ranked, Casual, Tavern Brawl and Arena**. Other modes are not measured yet, so the tracker shows nothing there. A Tavern Brawl against the computer (PvE, for example against the Innkeeper) gives no XP, so the overlay shows "+0" there.

### During a game

The rows appear with the first turn. During the mulligan the overlay is hidden.

| Row | Meaning |
|---|---|
| "XP time:" | The time that counts for XP so far, as minutes and seconds |
| "If win:" | The XP you get if you win now |
| "If loss:" | The XP you get if you lose or concede now |

The XP time can have a note:

| Note | Meaning |
|---|---|
| "(paused)" | You let your turn run out without any action. That turn and the opponent's next turn do not count, so the XP time jumped back and waits. It runs again in your next turn. |
| "(max)" | The XP time reached 30 minutes. More time gives no more XP. |

During a Hearthstone event, switch on "Event running (show event XP)" in the dashboard. The overlay then shows one more row, live and in the summary after the game:

| Row | Meaning |
|---|---|
| "Event XP:" | The event XP so far, with a red crystal: one per minute of XP time (see [FORMULA.md](FORMULA.md#event-xp)). It does not depend on the result. |

Switch it off when the event ends.

With "Tavern Pass (XP boost)" switched on in the dashboard, "If win:", "If loss:" and "Earned:" include the pass boost for your rewards track level. Event XP get no boost.

The last two rows show the damage, live and in the summary after the game. Switch them off with "Show damage on the overlay" in the dashboard.

| Row | Meaning |
|---|---|
| "Total Damage:" | All damage your side dealt to the opponent's hero and minions so far: your attacks, your minions' attacks, spells, weapons, hero power and other effects. Each hit counts with the amount the game shows, also when it is more than the target's health or hits armor. Damage to your own hero or minions does not count, and neither does damage without a source, such as fatigue. A hit on a Divine Shield is 0. |
| "Hero Damage:" | The part of "Total Damage:" that hit the opponent's hero, including the part that armor absorbed |

The damage is read when the server confirms an action, so it can rise a moment before the animation on the board.

While the XP time is below 60 seconds, both XP values show "+0". Games under one minute give no XP. See [FORMULA.md](FORMULA.md) for the rules.

If you start the tracker in the middle of a game, it still reads the game from its start.

### After a game

After the game, the overlay shows a summary until the mulligan of your next game:

| Row | Meaning |
|---|---|
| "Last game:" | Win or Loss |
| "XP time:" | The time that counted for XP |
| "Total time:" | The whole game including the mulligan |
| "Earned:" | The XP the game gave |

### Update hint

When a newer version of the tracker exists, the overlay shows one more row while no game is running:

| Row | Meaning |
|---|---|
| "Update available:" | The number of the new version, for example 0.2.0 |

The dashboard shows a link to the download page. See [INSTALL.md](INSTALL.md#update) for how to update.

## Dashboard

The dashboard window is called "HS XP Tracker". It opens when the tracker starts. To open it again:

- click the gold "XP" icon in the notification area, or
- right-click the icon and choose "XP dashboard", or
- start `HearthstoneXpTracker.exe` again.

Closing the dashboard keeps the tracker and the overlay running. To end the tracker, right-click the icon and choose "Exit".

### Toolbar

| Control | Meaning |
|---|---|
| "Session" | Games of the current Hearthstone session, from the start of the game until it is closed. After closing Hearthstone, the last session stays visible until the next start. This is the period shown when the dashboard opens. |
| "Today" | Games since midnight |
| "This week" | Games since Monday, 00:00 |
| "This season" | Games since the 1st of the month. Ranked seasons in Hearthstone run for a calendar month. |
| "Last 30 days" | Games of the last 30 days, including today |
| "All time" | All games in the history |
| "Refresh" | Read the history again |
| "Open data folder" | Open the folder with the history file in Windows Explorer |
| "Language" | Language of the overlay, the dashboard and the icon's menu. English is the default. The change applies at once; the dashboard opens again in the new language. In other languages the label keeps the English word, for example "Sprache (Language)". |
| "Overlay position" | Where the overlay sits: "Right, above the settings button" (the default) or "Left, above your portrait", which keeps the right side free, for example for the deck list of Hearthstone Deck Tracker. The overlay moves at once. |
| "Event running (show event XP)" | Switch on during a Hearthstone event: the overlay shows the event XP, and new games store it. The tracker cannot tell when an event runs, so this is off by default; while it is off, a short hint below it reminds you. |
| "Tavern Pass (XP boost)" | Switch on if you own the Tavern Pass of the current expansion. The boost is calculated, not measured: with the pass, the shown XP can be 1 XP off per game (see [FORMULA.md](FORMULA.md#tavern-pass)). |
| "Level" with "−", a number and "+" | Your rewards track level. Click "−" or "+", or type the number. The line below shows the boost and the next step, for example "10% XP boost, 2 levels to 15%". The boost is 10%, from level 20 on 15% and from level 70 on 20% (see [FORMULA.md](FORMULA.md#tavern-pass)). Update the level when you pass level 20 or 70. |
| "Show damage on the overlay" | Show or hide the rows "Total Damage:" and "Hero Damage:". On by default. |
| "Check for updates when the tracker starts" | Turn the update check on or off. See [Privacy](#privacy). |

A finished game appears in an open dashboard on its own. You do not need to click "Refresh".

For "Session", a line below the toolbar names the session, for example "Session since Tue 2026-11-10, 19:56 (running)" or "(ended)". A session can run past midnight.

Below it, a status line shows whether Hearthstone writes the game log the tracker reads: "Game log (Power.log): on". If the tracker had to switch the log on while Hearthstone was running, the line asks in red to restart Hearthstone once. If it could not switch the log on, it says so in red and names the file (see [INSTALL.md](INSTALL.md#troubleshooting)).

### Tabs

**Overview**

| Tile | Meaning |
|---|---|
| "XP" | XP earned in the period |
| "XP time" | Time that counted for XP |
| "Play time incl. mulligan" | Time in games, including the mulligan |
| "Online time" | Time Hearthstone was running. See below. |
| "Games" | Number of games, with wins and losses |
| "XP per hour" | XP per hour of XP time |
| "XP per online hour" | XP per hour of online time, so including menus and the queue |
| "Win rate" | Share of games won |
| "Event XP" | Event XP of the period; only shown when games were played during an event |

Online time is counted per Hearthstone session: from the start of Hearthstone to the end of the last game. While Hearthstone is running, it counts up to now. Only sessions with at least one game in the period count.

Games in modes without measured rates, for example Battlegrounds, are stored without XP. They are listed under "Games" and count in the play time and the online time, but not in the other tiles or the charts.

**Lost XP**

| Tile | Meaning |
|---|---|
| "AFK turns" | XP you lost because of AFK turns: turns without any action that ran out without "End Turn", and the opponent turns after them |
| "Games under 1 minute" | XP the formula would give for games under 60 seconds, which the game does not pay |
| "Time outside games" | Online time spent outside games (menus, queue, breaks), valued at your XP per hour in the period |

**Games**

All games of the period, newest first: date, start, mode, result, XP time, total time, XP, event XP, AFK turns and opponent class. Click a column header to sort.

**Charts**

Two curves on the same time axis. The axis always covers the whole period, so hours or days without games stay visible:

| Period | Time axis |
|---|---|
| "Session" | every hour from the start of the session to its end, or to now while it runs |
| "Today" | the 24 hours of the day |
| "This week" | Monday to Sunday |
| "This season" | every day of the month |
| "Last 30 days" | the last 30 days, including today |
| "All time" | every day since the first game in the history |

| Curve | Meaning |
|---|---|
| "XP earned" (gold) | XP of the games that started in each hour or on each day. Steps without games are at 0. |
| "XP per hour" (blue) | Speed in each hour or on each day: XP divided by XP time. Only steps with games have a point. The dashed line is the average of the whole period. |

A game counts in the hour or on the day in which it started.

## Game history file

Every finished game is added to a CSV file:

```
%APPDATA%\HearthstoneXpTracker\hs_games.csv
```

The file uses commas between fields and a dot as the decimal sign. Times are local time.

| Column | Content |
|---|---|
| `Session` | Name of Hearthstone's log folder for the session, for example `Hearthstone_2026_11_08_09_10_51` |
| `Start` | Start of the first turn, `yyyy-MM-ddTHH:mm:ss.fff` |
| `End` | End of the game, same format |
| `Mode` | Game mode, for example `Ranked` |
| `Format` | Format, for example Standard or Wild; empty if the mode has none |
| `Result` | For example `Win` or `Loss` |
| `XpSeconds` | Time that counted for XP |
| `PlaySeconds` | Time from the first turn to the end, AFK turns included |
| `WithMulliganSeconds` | Time including the mulligan |
| `AfkTurns` | Number of own AFK turns |
| `Xp` | XP calculated by the tracker; empty for modes without measured rates |
| `Deck` | Deck name, only for games recorded by the former HDT plugin; empty for newer games |
| `PlayerClass` | Your class |
| `OpponentClass` | The opponent's class |
| `EventXp` | Event XP calculated by the tracker; empty for games outside events. Files from older versions without this column are still read, and the column is added with the next game. |
| `XpBonus` | Tavern Pass XP boost in percent that applied when the game was played, for example `15`; `0` without the pass. Older files without this column are read as `0`. |

The `Xp` column is calculated by the tracker from the XP time. The game itself does not log XP. When the dashboard opens, the tracker calculates the XP of all stored games again with the current rates, so a corrected rate also fixes older games. Each game keeps its own `XpBonus`, so changing the pass or the level later does not change older games.

A game read twice from the same log is stored only once. Installing a new version keeps the file. At every start, the tracker also adds the games from the history of the former HDT plugin that it does not have yet (see [INSTALL.md](INSTALL.md#coming-from-the-hdt-plugin)).

## Privacy

- The game history and the settings stay on your computer. The tracker uploads nothing.
- The only network request is the update check. When the tracker starts, it asks GitHub once whether a newer release exists. The request sends only the program name and version, for example `HearthstoneXpTracker/0.2.0`. As with any web request, GitHub sees your IP address.
- To turn the update check off, clear "Check for updates when the tracker starts" in the dashboard. The setting is stored in `%APPDATA%\HearthstoneXpTracker\settings.ini`.
- The tracker only reads Hearthstone's log. It does not read the game's memory and does not change game files. It writes only to its own data folder and, once, the `[Power]` section of `log.config` (see [INSTALL.md](INSTALL.md#requirements)).
