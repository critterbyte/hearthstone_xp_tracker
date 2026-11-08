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

## End of game

- [ ] After the game the summary shows XP time, game time including the mulligan and the earned XP.
- [ ] The earned XP matches the XP shown by the game (win, loss, concede, opponent concedes).
- [ ] The summary stays visible in the menu and disappears when the next mulligan starts.

## Modes

- [ ] Ranked (Wild and Standard), Casual and Tavern Brawl show values with the matching rates.
- [ ] Arena, Battlegrounds and other modes show nothing.
