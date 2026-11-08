# Manual test checklist

The calculation engine is covered by unit tests. The points below need a real game with Hearthstone Deck Tracker and are checked before every release.

## Installation

- [ ] The plugin appears under *Options → Tracker → Plugins* and can be enabled.
- [ ] Disabling the plugin removes the overlay immediately; enabling it shows it again.

## Live overlay

- [ ] Nothing is shown in the menu before the first game and during the mulligan.
- [ ] The overlay appears with the first turn, above the Hearthstone settings button, and does not cover any game controls.
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

## End of game

- [ ] After the game the summary shows "Last game:" (Win/Loss), "XP time:", "Total time:" (including the mulligan) and "Earned:" (+N with the scroll icon). It is hidden on the victory/defeat screen by HDT and appears in the menu.
- [ ] The earned XP matches the XP shown by the game (win, loss, concede, opponent concedes).
- [ ] The summary stays visible in the menu and disappears when the next mulligan starts.

## Game history

- [ ] After each game a row is added to `%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv` with the right mode, format, result, times, XP, deck and classes.
- [ ] Restarting HDT right after a game does not add the same game twice.
- [ ] Installing a new plugin version keeps the history file.

## Dashboard

- [ ] "Plugins → XP dashboard" and the plugin button open the window; opening it again brings the open window to the front.
- [ ] The window is titled "HS XP Tracker" and shows the gold title with the scroll icon; colors are dark wood, gold and parchment, also for buttons, tabs, the check box and selected rows.
- [ ] Today / This week / This season / Last 30 days / All time change all tabs; the selected period is highlighted in gold.
- [ ] Charts: "XP per hour" (blue, one point per game for Today, one point per day otherwise) and below it "XP over play time" (gold, starts at 0, always rising, time axis in h:mm); both tooltips are dark with light text.
- [ ] Overview values match the game list (XP, XP time, play time, games, wins, losses); online time counts up while Hearthstone runs.
- [ ] Decks, Lost XP, Games and Charts show data; the game list sorts by every column.
- [ ] A finished game appears in an open dashboard without pressing Refresh.
- [ ] Export CSV saves the history; Open data folder opens it in Explorer.

## Modes

- [ ] Ranked (Wild and Standard), Casual and Tavern Brawl show values with the matching rates.
- [ ] Arena, Battlegrounds and other modes show nothing.
