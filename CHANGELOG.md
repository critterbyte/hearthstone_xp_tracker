# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Project skeleton: solution, calculation engine (`XpTracker.Core`), plugin entry point (`XpTracker.Plugin`), unit tests.
- XP rates for Ranked, Casual and Tavern Brawl with tests against measured games.
- Documentation of the XP formula (`docs/FORMULA.md`), with every statement marked as measured or taken from a listed source.
- Documentation for users and contributors: `docs/INSTALL.md`, `docs/USAGE.md`, `docs/FARMING.md` (fair tips for more XP per hour), `CONTRIBUTING.md`, and issue templates for bugs, feature requests and XP measurements.
- Local build script (`build.ps1`) and CI workflow for the calculation engine.
- Release script (`release.ps1`): builds and tests, checks the version, and creates the release ZIP with the two DLLs, `INSTALL.txt` and `LICENSE.txt` in a folder named `HearthstoneXpTracker`, plus a SHA-256 checksum. It refuses uncommitted changes and any unexpected file in the package.
- `XpClock`: tracks XP time, play time and time including the mulligan from turn events, live and after the game. AFK turns and the opponent turn that follows them are excluded.
- `XpRates.RateFor`: rate lookup by game mode and result.
- Game history: every finished game is appended to `%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv` (session, start, end, mode, format, result, XP time, play time, time including the mulligan, AFK turns, calculated XP, deck, player and opponent class). Games read twice from the same log are stored once. Basis for the planned dashboard.
- XP dashboard window, opened from HDT's Plugins menu ("XP dashboard") or the plugin's button: period selection (today, this week, this season, all time); overview with XP, XP time, play time including the mulligan, online time, games, XP per hour and win rate; deck table with XP per hour; lost XP from AFK turns, games under one minute and time outside games; game list; charts of XP and XP per hour per day; CSV export.
- Deck names from HDT's statistics: games stored without a deck (for example imported from logs) get deck and classes when the dashboard opens, matched by the game end time; new games take the deck of the game HDT is tracking. Games HDT did not record show "No deck recorded".
- Update hint: when HDT starts, the plugin asks GitHub once for a newer release (pre-releases included, drafts ignored). If there is one, the overlay shows "Update available:" while no game is running and the dashboard shows a link to the download page. Can be turned off in the dashboard ("Check for updates when HDT starts"); stored in `settings.ini` next to the game history. Nothing is downloaded automatically.
- Minimum XP time of 60 seconds: shorter games give no XP, so the overlay shows "+0" until the XP time reaches one minute.
- `PowerLogTracker` and `PowerLogTail`: read game times directly from Hearthstone's `Power.log` with the log timestamps, on a background thread. When HDT is started in the middle of a game, the current game is read from its start. AFK turns are detected from the player's turn limit (10 seconds).
- `tools/XpTracker.LogCheck`: replays `Power.log` files or archives and prints the times per game; `--tail` measures the plugin's read times.
- Live overlay above the Hearthstone settings button: "XP time: m:ss", "If win: +N XP", "If loss: +N XP"; after the game a "Last game" summary with XP time, total time including the mulligan and the XP earned, shown until the next mulligan.

### Changed

- Dashboard redesigned in Hearthstone-like colors (dark wood, gold, parchment, mana blue) with the title "HS XP Tracker". New period "Last 30 days". Values in mana blue; the selected period is highlighted in blue with a soft glow on hover. The two old charts are replaced by the running XP per hour (per game for today, per day for longer periods) and the XP earned over play time.

- Overlay layout: plain white labels on a dark box, values in blue tubes in the style of the reward track XP bar, XP values with a drawn scroll icon. The summary row for the earned XP is labeled "Earned:".

### Fixed

- Ranked win rate lowered from 0.1308 to 0.1305 XP/s. A 417.37-second win gave 54 XP, but the old rate calculated 55. The measured range is now 0.13047 to 0.13058 XP/s. When the dashboard opens, the XP of games already in the history is calculated again with the current rates.

- A Rewind card (game reset) no longer restarts the XP time on the overlay.
- Starting HDT during a game no longer loses the time before the start.
