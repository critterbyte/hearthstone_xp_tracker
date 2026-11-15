# Manual test checklist

The calculation engine is covered by unit tests. The points below need a real game with Hearthstone Deck Tracker and are checked before every release.

## Installation

- [ ] The plugin appears under *Settings → Tracker → Plugins* and can be enabled.
- [ ] Disabling the plugin removes the overlay immediately; enabling it shows it again.

## Live overlay

- [ ] Nothing is shown in the menu before the first game and during the mulligan.
- [ ] The overlay appears with the first turn at the bottom right, above the Hearthstone settings button, and covers no game controls, also in long languages such as Polski or ภาษาไทย.
- [ ] Text size and position fit at different window sizes (windowed and full screen).
- [ ] XP time counts up during own and opponent turns.
- [ ] "Win" and "Loss" XP values grow with the XP time and match `docs/FORMULA.md`.
- [ ] **AFK turn:** let an own turn run out without playing anything and without clicking "End Turn". When the opponent's turn starts, the XP time jumps back by the length of the AFK turn, shows "(paused)" and stays paused for the whole opponent turn. It runs again with the next own turn.
- [ ] **Follow-up AFK turn:** idle through the next own turn as well (10-second limit). It is excluded too, and so is the opponent turn after it.
- [ ] Playing a card in the turn after an AFK turn makes the clock run normally again.
- [ ] A turn without any played card but ended by clicking "End Turn" counts normally (no jump, no pause).
- [ ] **Rewind:** when either player uses a Rewind card, the XP time keeps running without a reset.
- [ ] **HDT started during a game:** the overlay shows the full XP time since the first turn, not only the time since HDT started.
- [ ] HDT stays responsive; no stutter in Hearthstone while the plugin is enabled.
- [ ] Live rows: labels "XP time:", "If win:", "If loss:" in plain white text; values in blue tubes ("m:ss", "+N", "+N"); the XP values show the scroll icon.
- [ ] "Total Damage:" and "Hero Damage:" are the last rows and count up with your hits (an area spell counts every target); damage to your own side and fatigue do not count. The value can rise a moment before the animation.

## End of game

- [ ] After the game the summary shows "Last game:" (Win/Loss), "XP time:", "Total time:" (including the mulligan) and "Earned:" (+N with the scroll icon). It is hidden on the victory/defeat screen by HDT and appears in the menu.
- [ ] The earned XP matches the XP shown by the game (win, loss, concede, opponent concedes).
- [ ] The summary stays visible in the menu and disappears when the next mulligan starts.

## Event XP

- [ ] With "Event running" switched on, the overlay shows "Event XP:" with the red crystal under "If loss:", live and in the summary; the value matches the event XP shown by the game.
- [ ] With the switch off, the row disappears at once and new games have no event XP.

## Game history

- [ ] After each game a row is added to `%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv` with the right mode, format, result, times, XP, deck and classes.
- [ ] Restarting HDT right after a game does not add the same game twice.
- [ ] Installing a new plugin version keeps the history file.

## Dashboard

- [ ] "Plugins → XP dashboard" and the plugin button open the window; opening it again brings the open window to the front.
- [ ] The window is titled "HS XP Tracker" and shows the gold title with the scroll icon; colors are dark wood, gold and parchment, also for buttons, tabs, the check box and selected rows.
- [ ] Session / Today / This week / This season / Last 30 days / All time change all tabs; the selected period is highlighted in blue. The dashboard opens on "Session".
- [ ] Session: while Hearthstone runs, the line says "(running)" and the online time counts up; after closing Hearthstone it says "(ended)", the values stay and the online time stops; after starting Hearthstone again, an empty new session is shown (after a game or "Refresh").
- [ ] Charts: "XP earned" (gold curve) above "XP per hour" (blue curve with the dashed period average), on the same complete time axis: 24 hours for Today, Monday to Sunday, every day of the month, the last 30 days, every day since the first game. Values match the game list; tooltips are dark and name the XP or XP/h and the number of games.
- [ ] Overview values match the game list (XP, XP time, play time, games, wins, losses); online time counts up while Hearthstone runs.
- [ ] Decks, Lost XP, Games and Charts show data; the game list sorts by every column.
- [ ] A finished game appears in an open dashboard without pressing Refresh.
- [ ] "Open data folder" opens the folder with `hs_games.csv` in Explorer. The window opens without a scroll bar.

## Dashboard options

- [ ] "Language": every language switches the overlay, the dashboard (it opens again in the new language) and the menu entry at once; texts fit, with no missing characters (check at least Русский, 日本語, 简体中文 and ภาษาไทย).
- [ ] "Overlay position": "Right, above the settings button" (the default) and "Left, above your portrait" move the overlay at once, also during a game. On the left it covers neither the portrait ("You" with the rank) nor HDT's card lists.
- [ ] "Event running" and "Show damage on the overlay" show and hide their rows at once.
- [ ] "Tavern Pass": switched on, the level changes the boost text and the overlay values; switch it off again before the next game (games store the boost they were played with).
- [ ] "Check for updates when HDT starts" can be switched off and on; check marks and selected values are mana blue.

## Modes

- [ ] Ranked (Wild and Standard), Casual, Tavern Brawl and Arena show values with the matching rates.
- [ ] A Tavern Brawl against the computer shows "+0" for win, loss and event XP, and the game list names it "Tavern Brawl (PvE)".
- [ ] Battlegrounds and other modes show nothing.
