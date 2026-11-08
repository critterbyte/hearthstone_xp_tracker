# Hearthstone XP Tracker

A plugin for [Hearthstone Deck Tracker](https://hsreplay.net/downloads/) (HDT) that shows how much reward track XP the current game is worth.

> **Status:** early development. Nothing is released yet.

## Planned features

- **Live display** during a game, placed above the Hearthstone settings button:
  - XP time (the play time the game counts towards XP)
  - expected XP if you win
  - expected XP if you lose
- **End-of-game summary:** XP time, total play time including the mulligan, and the XP earned for the actual result.
- Correct handling of AFK turns: a turn without any action and without ending the turn manually does not count, and neither does the following opponent turn.

## How it works

The plugin reads Hearthstone's `Power.log` (the same log HDT reads) on a background thread and uses its timestamps for the mulligan, every turn change, the AFK marker and the end of the game. If HDT is started in the middle of a game, the plugin reads the current game from its start. The log folder is taken from HDT's Hearthstone folder setting, with the running game as a fallback.

Reading costs about 40 ms once when the plugin starts and well under a millisecond per update afterwards.

Every finished game is added to a local history file, `%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`, which a planned dashboard will use. The file stays on your computer; nothing is uploaded.

## Update check

When HDT starts, the plugin asks GitHub once whether a newer release exists (`api.github.com/repos/critterbyte/hearthstone_xp_tracker/releases`). The request sends only a User-Agent with the plugin name and version, for example `HearthstoneXpTracker/0.1.0`; as with any web request, GitHub sees your IP address. No game data is sent.

If a newer version exists, the overlay shows "Update available:" with the version number while no game is running, and the dashboard shows a link to the download page. Nothing is downloaded or installed automatically.

To turn the check off, clear "Check for updates when HDT starts" in the dashboard. The setting is stored in `%APPDATA%\HearthstoneDeckTracker\XpTracker\settings.ini`.

## How XP is calculated

Hearthstone does not log reward track XP, so the formula was measured game by game. In short:

```
XP = round(xpSeconds × rate)
```

| Mode | Win | Loss |
|---|---:|---:|
| Ranked (Wild and Standard) | 0.1308 XP/s | 0.0932 XP/s |
| Casual and Tavern Brawl | 0.0932 XP/s | 0.0666 XP/s |

See [docs/FORMULA.md](docs/FORMULA.md) for the full rules and the measurements behind them.

## Building

Requirements:

- Windows
- [.NET SDK 10](https://dotnet.microsoft.com/download) (pinned in `global.json`)
- Hearthstone Deck Tracker installed (the plugin is compiled against your local copy)

```powershell
.\build.ps1
```

The script finds the newest HDT installation under `%LOCALAPPDATA%\HearthstoneDeckTracker`, builds the solution and runs the tests. Use `-HdtDir <path>` to point it to another HDT folder.

The calculation engine and its tests also build without HDT:

```powershell
dotnet test tests\XpTracker.Core.Tests
```

## Project layout

| Path | Content |
|---|---|
| `src/XpTracker.Core` | Calculation engine: XP time, rates, AFK rule. No HDT dependency. |
| `src/XpTracker.Plugin` | HDT plugin: overlay and end-of-game summary. |
| `tests/XpTracker.Core.Tests` | Unit tests for the calculation engine. |
| `tools/XpTracker.LogCheck` | Replays `Power.log` files and prints the times per game. |
| `docs/` | Documentation. |

## Versioning

The project follows [Semantic Versioning](https://semver.org/). The version is set in `Directory.Build.props` only. Releases are tagged `vX.Y.Z` and listed in [CHANGELOG.md](CHANGELOG.md).

## Disclaimer

This is an unofficial third-party plugin. It is not affiliated with or endorsed by HearthSim (the makers of Hearthstone Deck Tracker) or Blizzard Entertainment. Hearthstone Deck Tracker is © HearthSim, all rights reserved; this repository and its releases contain no HDT files. Hearthstone is a trademark of Blizzard Entertainment.

## License

[MIT](LICENSE)
