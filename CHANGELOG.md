# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

## [0.2.0] - 2026-11-21

The tracker becomes a stand-alone Windows program. Hearthstone Deck Tracker (HDT) stops supporting plugins on 30 November 2026; the tracker keeps the same overlay, dashboard and formula and no longer needs HDT.

### Added

- `HearthstoneXpTracker.exe`: a program of its own (WPF, .NET Framework 4.7.2, part of Windows 10 and 11). Only one instance runs; starting it again opens the dashboard of the running one.
- A gold "XP" icon in the notification area: a click opens the dashboard, its menu has "XP dashboard" and "Exit". The EXE and the dashboard window use the same icon.
- An overlay window of its own: it lies over Hearthstone's game area, follows the game window, lets every click through, never takes the focus and is shown only while Hearthstone is the active window.
- The tracker switches on Hearthstone's game log itself: it adds the `[Power]` section to `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config` if it is missing, changes nothing else in the file and keeps a copy as `log.config.bak`. If Hearthstone was running, a notice asks to restart it once. A status line in the dashboard shows whether the log is on.
- The Hearthstone folder is found through the running game, the Battle.net uninstall entry or the default folder.
- The classes of both heroes are read from the `Power.log`, stored in the same form as HDT stored them.
- The history and the settings of the HDT plugin are taken over: at every start, the games the tracker does not have yet are added from `%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`, and the settings are copied once. The plugin's files are only read.
- A hint below "Event running (show event XP)" while it is off, because the tracker cannot tell when an event runs.
- "Start with Windows" in the dashboard: the tracker starts at sign-in with its icon only, without opening the dashboard. It uses an entry for the current user (no administrator rights) that the Task Manager lists under *Startup apps*.
- "Shortcut in the Start menu" in the dashboard, and a one-time question at the first start whether to add the tracker to the Start menu. If the tracker's folder was moved, the next start points the shortcut and the autostart entry to the new place.
- `THIRD-PARTY-NOTICES.txt` with the licenses of LiveCharts and Newtonsoft.Json, which the ZIP now contains.
- A CI job on Windows that builds the app, runs the tests and builds the release ZIP for every pull request.
- Releases are built on GitHub: pushing a tag `vX.Y.Z` starts the workflow `release`, which checks the version and the changelog, builds the ZIP, records its build provenance and creates a draft release. `gh attestation verify` shows that a ZIP was built from the tagged commit (see `SECURITY.md`).
- Community files: `CODE_OF_CONDUCT.md` (Contributor Covenant 2.1), `SECURITY.md` (private vulnerability reports through GitHub, no public issues) and a pull request template with the checklist from `CONTRIBUTING.md`. Reports go through GitHub's private reporting, so no e-mail address is published.

### Changed

- Ranked win rate changed from 0.13054 to 0.130515 XP/s. A 440.53-second win gave 57 XP, but 0.13054 calculated 58. The measured range is now 0.130505 to 0.130525 XP/s (103 wins), and the Ranked loss range 0.09317 to 0.093216 XP/s (111 losses). When the dashboard opens, the XP of games already in the history is calculated again with the current rate.
- Game history, settings and a small error log are stored in `%APPDATA%\HearthstoneXpTracker`.
- The update check runs when the tracker starts; the option is called "Check for updates when the tracker starts".
- The release ZIP contains the app and its libraries and can be extracted into any folder. `release.ps1` and `build.ps1` no longer need an HDT installation.
- README, install and usage guides in all 14 languages, `CONTRIBUTING.md`, `SECURITY.md`, the test checklist and the issue templates describe the app instead of the plugin. HDT is mentioned only where the switch from the plugin is described.

### Removed

- The HDT plugin. Version 0.1.2 stays available on the Releases page until HDT ends plugin support.
- The *Decks* tab and the deck column of the game list: deck names came from HDT. The history file keeps its `Deck` column, so older games keep their deck names.
- The code that matched games with HDT's deck records and the deck statistics behind the former *Decks* tab.

## [0.1.2] - 2026-11-15

A more visible update hint and the installed version in the dashboard.

### Added

- The dashboard shows the installed version in the top right corner of its title bar, in mana blue (for example "v0.1.2").

### Changed

- The overlay's "Update available:" label is mana blue instead of white, so the hint catches the eye.
- The dashboard's "Language" and "Overlay position" drop-downs are equally wide and line up on the right, with their labels right-aligned, in every language.
- HDT's plugin list shows the version with three parts, as on GitHub (0.1.2 instead of 0.1.2.0).

## [0.1.1] - 2026-11-15

Bug fixes after the first public beta, found in a review of the whole project.

### Changed

- Dashboard: games in modes without measured rates (for example Battlegrounds) no longer count in "Games", "XP time", "XP per hour", "Win rate", the deck table and the charts. They are stored without XP, so they lowered the XP per hour before. They still appear in the game list and count in the play time and the online time.

### Fixed

- Lost XP: a game that AFK turns pushed below the 60-second minimum is no longer counted in both "AFK turns" and "Games under 1 minute"; the two tiles now add up to the XP the game would have given. A Tavern Brawl against the computer no longer counts as a game under 1 minute, because it gives no XP anyway.
- The overlay no longer shows "Update available:" during the mulligan. It appears only while no game is running, as documented.
- Ranked and Casual games in formats other than Standard and Wild (for example Twist) no longer get the Ranked or Casual rates, which were never measured there. They are stored without XP, and the overlay shows nothing, as `docs/FORMULA.md` says.
- Documentation: `CONTRIBUTING.md` no longer calls translations planned or Arena unmeasured. `FARMING.md` and its translations name both sources for the Tavern Pass boost: the official post [6] for the percentages and the Hearthstone Wiki [7] for the levels.
- A row of `hs_games.csv` that cannot be read no longer stops the dashboard from opening or new games from being stored, for example after the file was saved by a spreadsheet with other date or number formats. Such rows are skipped and kept unchanged at the end of the file when the plugin rewrites it.
- Games after midnight get the right date when Hearthstone was started on an earlier day. Before, starting HDT after midnight in a session from the evening made the overlay show "30:00 (max)" during the game, and a game after a break of 12 to 24 hours in the same session was stored on the previous day. The date now comes from the system clock instead of being counted up from the session's start date.
- A game that ends while `hs_games.csv` is open in another program is no longer lost: the plugin keeps it and writes it with the next game or the next refresh of the dashboard, as long as HDT keeps running.
- An unexpected error while reading `Power.log` no longer stops the overlay until HDT is restarted. The line is skipped, reading goes on, and the error is written to HDT's log once.

## [0.1.0] - 2026-11-15

First public beta: XP for Ranked, Casual, Tavern Brawl and Arena, measured game by game.

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
- XP dashboard window, opened from HDT's Plugins menu ("XP dashboard") or the plugin's button: period selection (today, this week, this season, all time); overview with XP, XP time, play time including the mulligan, online time, games, XP per hour and win rate; deck table with XP per hour; lost XP from AFK turns, games under one minute and time outside games; game list; charts of XP and XP per hour per day. "Open data folder" opens the folder with the history file, which is already a CSV file, so there is no separate export.
- Deck names from HDT's statistics: games stored without a deck (for example imported from logs) get deck and classes when the dashboard opens, matched by the game end time; new games take the deck of the game HDT is tracking. Games HDT did not record show "No deck recorded".
- Update hint: when HDT starts, the plugin asks GitHub once for a newer release (pre-releases included, drafts ignored). If there is one, the overlay shows "Update available:" while no game is running and the dashboard shows a link to the download page. Can be turned off in the dashboard ("Check for updates when HDT starts"); stored in `settings.ini` next to the game history. Nothing is downloaded automatically.
- Event XP: measured formula (one per minute of XP time, rounded half up, no 60-second minimum, no win bonus). With "Event running" switched on in the dashboard, the overlay shows an "Event XP:" row with a red crystal, games store the event XP in the new `EventXp` column of the history, and the dashboard shows it in Overview and the game list. Older history files are still read and get the new column with the next game.
- Damage counter: the overlay's last two rows, live and in the summary after the game. "Total Damage:" adds up all damage the player's side dealt to the opponent's hero and minions; "Hero Damage:" is the part that hit the opponent's hero (entities with `CARDTYPE` `HERO`, including hero cards played later). The source of each hit is read from the target's `LAST_AFFECTED_BY` tag in `Power.log`; damage to the own side and damage without a source (fatigue, paying health) are left out. Checked against 61 games from 10 to 12 November 2026. "Show damage on the overlay" in the dashboard hides both rows (on by default, stored in `settings.ini`). `tools/XpTracker.LogCheck` prints the damage per game in the new `Damage` and `HeroDamage` columns.
- Tavern Pass mode: the dashboard has a "Tavern Pass (XP boost)" check box and the rewards track level with "−", a number field and "+". The boost follows from the level (10%, 15% from level 20, 20% from level 70, according to the Hearthstone Wiki; not measured yet) and is applied before rounding: `round(xpSeconds × rate × (1 + boost))`. The overlay and new games use it; event XP get no boost. Each game stores its boost in the new `XpBonus` column, so recalculating the history never adds the boost to older games. Older history files are read as games without the pass.
- Localization groundwork: all texts of the overlay, the dashboard and the HDT plugin entry come from a language file (`src/XpTracker.Core/Localization/en.txt`) embedded in `XpTracker.Core.dll`, so the release keeps two DLLs. Missing texts fall back to English. Numbers, dates and day names follow the language. Stored data (`hs_games.csv`, `settings.ini`) stays in English. The game list shows the classes "Death Knight" and "Demon Hunter" with their proper names.
- Language selection: "Language" in the dashboard switches the overlay, the dashboard and the plugin's menu entry at once (stored as `Language` in `settings.ini`, English by default). The list follows the Hearthstone client's languages and order. First translation: German (`de.txt`), with the German client's terms (EP, Belohnungsweg, Gasthauspass, Gewertet, Kartenchaos).
- Twelve more languages, so all 14 client languages are available: Español (AL), Español (EU), Français, Italiano, 日本語, Polski, Português (Brazil), Русский, ภาษาไทย, 한국어, 繁體中文, 简体中文. Game terms (modes, rewards track, Tavern Pass, XP abbreviation, classes, the End Turn button) come from the client's own string tables (HearthSim `hsdata`, build 37.0). Dates keep the Gregorian year in every language.
- Translated user guides: `INSTALL.md`, `USAGE.md` and `FARMING.md` in all 13 other client languages under `docs/<code>/`, machine-assisted and marked as such, with the English version as the reference. The README starts with links to the install guide in each language, and `INSTALL.txt` in the release ZIP lists them as well.
- Minimum XP time of 60 seconds: shorter games give no XP, so the overlay shows "+0" until the XP time reaches one minute.
- `PowerLogTracker` and `PowerLogTail`: read game times directly from Hearthstone's `Power.log` with the log timestamps, on a background thread. When HDT is started in the middle of a game, the current game is read from its start. AFK turns are detected from the player's turn limit (10 seconds).
- `tools/XpTracker.LogCheck`: replays `Power.log` files or archives and prints the times per game; `--tail` measures the plugin's read times.
- Arena: measured on 14 November 2026 (3 wins, 1 loss) and supported. Arena pays the Casual rates (win 0.0932, loss 0.0666 XP/s) and event XP as in the other modes. The plugin recognizes Arena games (`GT_ARENA`), the overlay shows their values, the history stores them as `Arena`, and the dashboard names the mode in every language. The Arena games narrowed the shared Casual, Tavern Brawl and Arena ranges to 0.09306 to 0.093272 (win) and 0.06649 to 0.066636 XP/s (loss).
- Overlay position: "Overlay position" in the dashboard moves the overlay from its default place above the Hearthstone settings button (bottom right) to the bottom left above the player's portrait, so it does not cover the end of HDT's deck list on the right (long lists, for example in singleton decks). Stored as `OverlayPosition` in `settings.ini`.
- Live overlay above the Hearthstone settings button: "XP time: m:ss", "If win: +N XP", "If loss: +N XP"; after the game a "Last game" summary with XP time, total time including the mulligan and the XP earned, shown until the next mulligan.

### Changed

- Dashboard redesigned in Hearthstone-like colors (dark wood, gold, parchment, mana blue) with the title "HS XP Tracker". New period "Last 30 days". Values in mana blue; the selected period is highlighted in blue with a soft glow on hover. The charts show two curves on a complete time axis (24 hours for today, Monday to Sunday, every day of the month, the last 30 days, every day since the first game): the XP earned in each step and the XP per hour, with the period average as a dashed line. "Last 30 days" now means the last 30 calendar days including today. New period "Session" (the current or last Hearthstone session, hourly chart axis across midnight), selected when the dashboard opens. The window opens 1180 × 800 px (at most the screen's work area), so the options column fits without a scroll bar. Check marks, the selected language and overlay position, and the Tavern Pass level are shown in mana blue. The overview shows "Wins: 3, losses: 2" instead of a count with a fixed plural.

- Overlay layout: plain white labels on a dark box, values in blue tubes in the style of the reward track XP bar, XP values with a drawn scroll icon. The summary row for the earned XP is labeled "Earned:".

### Fixed

- Tavern Brawls against the computer (PvE) no longer show and store XP: they give none. The plugin recognizes them by the opponent without an account and names them "Tavern Brawl (PvE)".

- The dashboard's online time no longer keeps counting after Hearthstone was closed.
- After a restart of Hearthstone, the dashboard shows the new session at once (within 5 seconds) instead of the previous one, and the online time starts with the new session. Hearthstone creates the new log folder at start but the `Power.log` only with the first game; the plugin now follows the newest session folder and reads the log as soon as it appears.
- The dashboard charts label each point with its own hour or day. With few points, such as a week or a session, the labels were shifted half a step to the left and showed the previous day.

- Ranked win rate changed from 0.1308 to 0.13054 XP/s. A 417.37-second win gave 54 XP, but 0.1308 calculated 55; a 425.27-second win gave 56 XP, but 0.1305 calculated 55. The measured range is now 0.130505 to 0.13058 XP/s, and the Ranked loss range 0.09317 to 0.09322 XP/s. When the dashboard opens, the XP of games already in the history is calculated again with the current rates.

- A Rewind card (game reset) no longer restarts the XP time on the overlay.
- Starting HDT during a game no longer loses the time before the start.

[Unreleased]: https://github.com/critterbyte/hearthstone_xp_tracker/compare/v0.2.0...HEAD
[0.2.0]: https://github.com/critterbyte/hearthstone_xp_tracker/releases/tag/v0.2.0
[0.1.2]: https://github.com/critterbyte/hearthstone_xp_tracker/releases/tag/v0.1.2
[0.1.1]: https://github.com/critterbyte/hearthstone_xp_tracker/releases/tag/v0.1.1
[0.1.0]: https://github.com/critterbyte/hearthstone_xp_tracker/releases/tag/v0.1.0
