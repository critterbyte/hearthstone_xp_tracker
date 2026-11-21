# Hearthstone XP Tracker

**Install guide in your language:** [Deutsch](docs/de/INSTALL.md) · [Español (AL)](docs/es-MX/INSTALL.md) · [Español (EU)](docs/es-ES/INSTALL.md) · [Français](docs/fr/INSTALL.md) · [Italiano](docs/it/INSTALL.md) · [日本語](docs/ja/INSTALL.md) · [Polski](docs/pl/INSTALL.md) · [Português](docs/pt-BR/INSTALL.md) · [Русский](docs/ru/INSTALL.md) · [ภาษาไทย](docs/th/INSTALL.md) · [한국어](docs/ko/INSTALL.md) · [繁體中文](docs/zh-TW/INSTALL.md) · [简体中文](docs/zh-CN/INSTALL.md)

A small Windows program that shows how much reward track XP your current Hearthstone game is worth. Along the way, it also tracks the damage you deal.

> **Formerly an HDT plugin.** Up to version 0.1.2 the tracker was a plugin for Hearthstone Deck Tracker (HDT). HDT stops supporting plugins on 30 November 2026, so from version 0.2.0 on it is a program of its own, with the same overlay and dashboard. It takes over your plugin data automatically; see [Coming from the HDT plugin](docs/INSTALL.md#coming-from-the-hdt-plugin).

[![build](https://github.com/critterbyte/hearthstone_xp_tracker/actions/workflows/build.yml/badge.svg)](https://github.com/critterbyte/hearthstone_xp_tracker/actions/workflows/build.yml)

> **Status:** beta. XP is calculated for Ranked, Casual, Tavern Brawl and Arena. Other modes, such as Battlegrounds, are not measured yet.

![The XP Tracker overlay at the bottom right of a Ranked game](docs/images/overlay-in-game.jpg)

## Features

- **Live overlay** at the bottom right above the Hearthstone settings button, or at the bottom left above your portrait:
  - XP time: the part of the game that counts for XP
  - the XP you get if you win, and if you lose
- **Event XP tracking:** during Hearthstone events, the overlay also shows the event XP of the game (one per minute of XP time), live and in the summary, and the dashboard adds them up. Switch on "Event running" in the dashboard.
- **Summary after each game:** XP time, total time including the mulligan, and the XP earned.
- **Damage tracker:** along the way, the overlay counts the damage your side deals to the opponent, in total and to the hero, live and in the summary after each game. You can switch it off in the dashboard.
- **AFK rule:** a turn that runs out without any action does not count, and neither does the opponent turn after it. The overlay shows this live.
- **Dashboard:** XP, XP per hour, win rate, lost XP and charts for today, this week, this season or all time.
- **Tavern Pass:** enter your rewards track level, and the XP include the pass boost of 10%, 15% or 20%. The boost is calculated from public sources, not measured.
- **Game history:** every game is saved to a local CSV file that you can open in a spreadsheet.
- **Passive and light:** the tracker only reads Hearthstone's log file. It does not read the game's memory, does not change game files and needs no administrator rights.
- **Update hint:** the tracker tells you when a new version is available.
- **Languages:** overlay and dashboard in all 14 languages of the Hearthstone client, selectable in the dashboard: Deutsch, English (default), Español (AL), Español (EU), Français, Italiano, 日本語, Polski, Português, Русский, ภาษาไทย, 한국어, 繁體中文, 简体中文. Game terms follow each client. German and Polish are checked by a native speaker; the other translations are machine-assisted, and [corrections are welcome](CONTRIBUTING.md#translations).

## Screenshots

| Live overlay | Dashboard |
|---|---|
| <img src="docs/images/overlay.png" alt="Overlay with XP time, XP for a win and a loss, event XP and damage" width="260"> | <img src="docs/images/dashboard.png" alt="Dashboard overview with XP, XP time, games, win rate and event XP for today" width="560"> |
| XP time, XP for a win and a loss, event XP and damage, updated live. | XP, XP per hour, win rate and more for the session, today, this week, this season or all time. |

## Installation

1. Download the ZIP file from the [Releases page](https://github.com/critterbyte/hearthstone_xp_tracker/releases).
2. Right-click the ZIP, choose *Properties* and select *Unblock* if it is shown.
3. Extract the ZIP into a folder of your choice, for example *Documents*. This creates the folder `HearthstoneXpTracker`.
4. Start `HearthstoneXpTracker.exe`. If Windows shows "Windows protected your PC", choose *More info → Run anyway*; the program is not signed yet.
5. The dashboard opens, and a gold "XP" icon in the notification area opens it again later. If Hearthstone was already running, restart it once so that it writes its game log.

See [docs/INSTALL.md](docs/INSTALL.md) for details, updates and troubleshooting.

## How XP is calculated

Hearthstone does not log reward track XP, so the formula was measured game by game:

```
XP = round(xpSeconds × rate)
```

| Mode | Win | Loss |
|---|---:|---:|
| Ranked (Standard and Wild) | 0.130515 XP/s (470 XP/h) | 0.0932 XP/s (336 XP/h) |
| Casual, Tavern Brawl and Arena | 0.0932 XP/s (336 XP/h) | 0.0666 XP/s (240 XP/h) |

Games with less than 60 seconds of XP time give no XP. The rates were measured without a Tavern Pass. With the pass, the tracker adds its boost before rounding; this is calculated, not measured, because the project follows a strict free-to-play policy and buys no pass. See [docs/FORMULA.md](docs/FORMULA.md) for all rules, the measurements and the sources.

## Documentation

| Page | Content |
|---|---|
| [INSTALL.md](docs/INSTALL.md) | Installation, update, uninstall, troubleshooting |
| [USAGE.md](docs/USAGE.md) | Overlay, dashboard, history file, privacy |
| [FORMULA.md](docs/FORMULA.md) | The XP formula: what was measured and what comes from sources |
| [FARMING.md](docs/FARMING.md) | Fair tips for more XP per hour |
| [CONTRIBUTING.md](CONTRIBUTING.md) | Building, code, measurements, pull requests |
| [CHANGELOG.md](CHANGELOG.md) | Changes per version |

## Privacy

- Your game history and settings stay on your computer in `%APPDATA%\HearthstoneXpTracker`. Nothing is uploaded.
- When the tracker starts, it asks GitHub once whether a newer release exists (`api.github.com/repos/critterbyte/hearthstone_xp_tracker/releases`). The request sends only the program name and version, for example `HearthstoneXpTracker/0.2.0`. As with any web request, GitHub sees your IP address. No game data is sent.
- If a newer version exists, the overlay shows "Update available:" while no game is running, and the dashboard shows a link to the download page. Nothing is downloaded or installed automatically.
- To turn the check off, clear "Check for updates when the tracker starts" in the dashboard.

## How it works

The tracker reads Hearthstone's `Power.log` (the same log other trackers read) on a background thread. It uses the log's timestamps for the mulligan, every turn change, the AFK marker and the end of the game. If the tracker is started in the middle of a game, it reads the current game from its start. It finds the log folder through the running game, the Battle.net uninstall entry or the default installation folder. At its first start it switches the log on in Hearthstone's `log.config` if needed; nothing else in that file is changed.

The overlay is a transparent window that lies over Hearthstone's game area. It lets every click through, never takes the focus and is shown only while Hearthstone is the active window.

Reading costs about 40 ms once when the tracker starts and well under a millisecond per update afterwards.

## Building

```powershell
.\build.ps1
```

You need Windows and the .NET SDK 10. See [CONTRIBUTING.md](CONTRIBUTING.md#building).

## Versioning

The project follows [Semantic Versioning](https://semver.org/). Releases are tagged `vX.Y.Z` and listed in [CHANGELOG.md](CHANGELOG.md).

## Disclaimer

This is an unofficial third-party program. It is not affiliated with or endorsed by Blizzard Entertainment. Hearthstone is a trademark of Blizzard Entertainment.

## License

[MIT](LICENSE)
