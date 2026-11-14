# Hearthstone XP Tracker

A plugin for [Hearthstone Deck Tracker](https://hsreplay.net/downloads/) (HDT) that shows how much reward track XP your current game is worth.

[![build](https://github.com/critterbyte/hearthstone_xp_tracker/actions/workflows/build.yml/badge.svg)](https://github.com/critterbyte/hearthstone_xp_tracker/actions/workflows/build.yml)

> **Status:** beta. XP is calculated for Ranked, Casual and Tavern Brawl. Other modes are not measured yet.

## Features

- **Live overlay** above the Hearthstone settings button:
  - XP time: the part of the game that counts for XP
  - the XP you get if you win, and if you lose
  - the damage your side dealt to the opponent, in total and to the hero (can be switched off)
- **Summary after each game:** XP time, total time including the mulligan, and the XP earned.
- **AFK rule:** a turn that runs out without any action does not count, and neither does the opponent turn after it. The overlay shows this live.
- **Dashboard:** XP, XP per hour, win rate, decks, lost XP and charts for today, this week, this season or all time.
- **Tavern Pass:** enter your rewards track level, and the XP include the pass boost of 10%, 15% or 20%. The boost is calculated from public sources, not measured.
- **Game history:** every game is saved to a local CSV file that you can open in a spreadsheet.
- **Update hint:** the plugin tells you when a new version is available.

## Installation

1. Download the ZIP file from the [Releases page](https://github.com/critterbyte/hearthstone_xp_tracker/releases).
2. Close HDT.
3. Extract the ZIP into `%APPDATA%\HearthstoneDeckTracker\Plugins`. This creates the folder `HearthstoneXpTracker`.
4. Start HDT and enable the plugin under *Settings* (the gear icon on the right of the top bar) *→ Tracker → Plugins*.

See [docs/INSTALL.md](docs/INSTALL.md) for details, updates and troubleshooting.

## How XP is calculated

Hearthstone does not log reward track XP, so the formula was measured game by game:

```
XP = round(xpSeconds × rate)
```

| Mode | Win | Loss |
|---|---:|---:|
| Ranked (Standard and Wild) | 0.13054 XP/s (470 XP/h) | 0.0932 XP/s (336 XP/h) |
| Casual and Tavern Brawl | 0.0932 XP/s (336 XP/h) | 0.0666 XP/s (240 XP/h) |

Games with less than 60 seconds of XP time give no XP. The rates were measured without a Tavern Pass. With the pass, the plugin adds its boost before rounding; this is calculated, not measured, because the project follows a strict free-to-play policy and buys no pass. See [docs/FORMULA.md](docs/FORMULA.md) for all rules, the measurements and the sources.

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

- Your game history and settings stay on your computer in `%APPDATA%\HearthstoneDeckTracker\XpTracker`. Nothing is uploaded.
- When HDT starts, the plugin asks GitHub once whether a newer release exists (`api.github.com/repos/critterbyte/hearthstone_xp_tracker/releases`). The request sends only the plugin name and version, for example `HearthstoneXpTracker/0.1.0`. As with any web request, GitHub sees your IP address. No game data is sent.
- If a newer version exists, the overlay shows "Update available:" while no game is running, and the dashboard shows a link to the download page. Nothing is downloaded or installed automatically.
- To turn the check off, clear "Check for updates when HDT starts" in the dashboard.

## How it works

The plugin reads Hearthstone's `Power.log` (the same log HDT reads) on a background thread. It uses the log's timestamps for the mulligan, every turn change, the AFK marker and the end of the game. If HDT is started in the middle of a game, the plugin reads the current game from its start. The log folder comes from HDT's Hearthstone folder setting, with the running game as a fallback.

Reading costs about 40 ms once when the plugin starts and well under a millisecond per update afterwards.

## Building

```powershell
.\build.ps1
```

You need Windows, the .NET SDK 10 and a local HDT installation. See [CONTRIBUTING.md](CONTRIBUTING.md#building).

## Versioning

The project follows [Semantic Versioning](https://semver.org/). Releases are tagged `vX.Y.Z` and listed in [CHANGELOG.md](CHANGELOG.md).

## Disclaimer

This is an unofficial third-party plugin. It is not affiliated with or endorsed by HearthSim (the makers of Hearthstone Deck Tracker) or Blizzard Entertainment. Hearthstone Deck Tracker is © HearthSim, all rights reserved; this repository and its releases contain no HDT files. Hearthstone is a trademark of Blizzard Entertainment.

## License

[MIT](LICENSE)
