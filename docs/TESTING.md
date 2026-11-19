# Manual test checklist

The calculation engine is covered by unit tests. The points below need Windows and a real game and are checked before every release.

## Installation and start

- [ ] The release ZIP contains only the folder `HearthstoneXpTracker` with the app, its libraries, `INSTALL.txt`, `LICENSE.txt` and `THIRD-PARTY-NOTICES.txt`.
- [ ] Extracted into a new folder, `HearthstoneXpTracker.exe` starts; the dashboard opens and the gold "XP" icon appears in the notification area. The EXE and the dashboard window show the same gold icon.
- [ ] Starting the EXE a second time starts no second tracker; the running one brings its dashboard to the front.
- [ ] A left click on the icon opens the dashboard; its menu shows "XP dashboard" (bold) and "Exit"; "Exit" ends the tracker and removes the icon and the overlay.
- [ ] Closing the dashboard keeps the tracker and the overlay running.

## Game log

- [ ] Without a `[Power]` section in `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config` (rename the file to test), the first start adds it and keeps the other sections; `log.config.bak` holds the old file.
- [ ] If Hearthstone was running at that moment, a notice next to the icon and a red status line in the dashboard ask to restart Hearthstone; after the restart the status line says "Game log (Power.log): on".
- [ ] With the log already on, the file is not changed and no `log.config.bak` is created.
- [ ] With Hearthstone not running, the tracker still finds the Logs folder (Battle.net uninstall entry or default folder) and reads the next session.

## Taking over the HDT plugin's data

- [ ] With an existing `%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`, the first start adds all its games to `%APPDATA%\HearthstoneXpTracker\hs_games.csv`; `xptracker.log` names the number. The plugin's file is unchanged.
- [ ] Games recorded by both the plugin and the tracker appear once; deck names of older games are kept in the file.
- [ ] Without an own `settings.ini`, the plugin's settings are taken over (language, event, Tavern Pass, damage, overlay position).

## Live overlay

- [ ] Nothing is shown in the menu before the first game and during the mulligan.
- [ ] The overlay appears with the first turn at the bottom right, above the Hearthstone settings button, and covers no game controls, also in long languages such as Polski or ภาษาไทย.
- [ ] It follows the game window when it is moved or resized, in windowed and full screen mode, also with Windows display scaling above 100%.
- [ ] Clicks on the overlay reach the game (play a card or attack through it); the overlay never takes the focus and is not in the taskbar or Alt+Tab.
- [ ] It disappears when another window is active (Alt+Tab, the dashboard) and comes back with Hearthstone.
- [ ] Next to Hearthstone Deck Tracker, both overlays are visible and the XP Tracker stays on top of the game.
- [ ] XP time counts up during own and opponent turns.
- [ ] "Win" and "Loss" XP values grow with the XP time and match `docs/FORMULA.md`.
- [ ] **AFK turn:** let an own turn run out without playing anything and without clicking "End Turn". When the opponent's turn starts, the XP time jumps back by the length of the AFK turn, shows "(paused)" and stays paused for the whole opponent turn. It runs again with the next own turn.
- [ ] **Follow-up AFK turn:** idle through the next own turn as well (10-second limit). It is excluded too, and so is the opponent turn after it.
- [ ] Playing a card in the turn after an AFK turn makes the clock run normally again.
- [ ] A turn without any played card but ended by clicking "End Turn" counts normally (no jump, no pause).
- [ ] **Rewind:** when either player uses a Rewind card, the XP time keeps running without a reset.
- [ ] **Tracker started during a game:** the overlay shows the full XP time since the first turn, not only the time since the start.
- [ ] No stutter in Hearthstone while the tracker runs.
- [ ] Live rows: labels "XP time:", "If win:", "If loss:" in plain white text; values in blue tubes ("m:ss", "+N", "+N"); the XP values show the scroll icon.
- [ ] "Total Damage:" and "Hero Damage:" are the last rows and count up with your hits (an area spell counts every target); damage to your own side and fatigue do not count. The value can rise a moment before the animation.

## End of game

- [ ] After the game the summary shows "Last game:" (Win/Loss), "XP time:", "Total time:" (including the mulligan) and "Earned:" (+N with the scroll icon).
- [ ] The earned XP matches the XP shown by the game (win, loss, concede, opponent concedes).
- [ ] The summary stays visible in the menu and disappears when the next mulligan starts.

## Event XP

- [ ] With "Event running" switched on, the overlay shows "Event XP:" with the red crystal under "If loss:", live and in the summary; the value matches the event XP shown by the game.
- [ ] With the switch off, the row disappears at once, new games have no event XP, and the hint below the check box is shown.

## Game history

- [ ] After each game a row is added to `%APPDATA%\HearthstoneXpTracker\hs_games.csv` with the right mode, format, result, times, XP and both classes; the deck column stays empty.
- [ ] Restarting the tracker right after a game does not add the same game twice.
- [ ] Installing a new version keeps the history file.

## Dashboard

- [ ] The window is titled "HS XP Tracker" and shows the gold title, the version in the top right corner and the scroll icon; colors are dark wood, gold and parchment, also for buttons, tabs, the check box and selected rows.
- [ ] Session / Today / This week / This season / Last 30 days / All time change all tabs; the selected period is highlighted in blue. The dashboard opens on "Session".
- [ ] Session: while Hearthstone runs, the line says "(running)" and the online time counts up; after closing Hearthstone it says "(ended)", the values stay and the online time stops; after starting Hearthstone again, an empty new session is shown (after a game or "Refresh").
- [ ] The tabs are Overview, Lost XP, Games and Charts; there is no Decks tab and no deck column.
- [ ] Charts: "XP earned" (gold curve) above "XP per hour" (blue curve with the dashed period average), on the same complete time axis: 24 hours for Today, Monday to Sunday, every day of the month, the last 30 days, every day since the first game. Values match the game list; tooltips are dark and name the XP or XP/h and the number of games.
- [ ] Overview values match the game list (XP, XP time, play time, games, wins, losses); online time counts up while Hearthstone runs.
- [ ] Lost XP, Games and Charts show data; the game list sorts by every column and shows the opponent class.
- [ ] A finished game appears in an open dashboard without pressing Refresh.
- [ ] "Open data folder" opens `%APPDATA%\HearthstoneXpTracker` in Explorer. The window opens without a scroll bar.

## Dashboard options

- [ ] "Language": every language switches the overlay, the dashboard (it opens again in the new language) and the icon's menu at once; texts fit, with no missing characters (check at least Русский, 日本語, 简体中文 and ภาษาไทย).
- [ ] "Overlay position": "Right, above the settings button" (the default) and "Left, above your portrait" move the overlay at once, also during a game. On the left it covers neither the portrait ("You" with the rank) nor HDT's card lists.
- [ ] "Event running" and "Show damage on the overlay" show and hide their rows at once.
- [ ] "Tavern Pass": switched on, the level changes the boost text and the overlay values; switch it off again before the next game (games store the boost they were played with).
- [ ] "Check for updates when the tracker starts" can be switched off and on; check marks and selected values are mana blue.

## Update hint

- [ ] A build with a lower version (for example `-p:Version=0.1.0`) shows "Update available:" in mana blue on the overlay in the menu and the update bar in the dashboard; the link opens the release page.

## Modes

- [ ] Ranked (Wild and Standard), Casual, Tavern Brawl and Arena show values with the matching rates.
- [ ] A Tavern Brawl against the computer shows "+0" for win, loss and event XP, and the game list names it "Tavern Brawl (PvE)".
- [ ] Battlegrounds and other modes show nothing.
