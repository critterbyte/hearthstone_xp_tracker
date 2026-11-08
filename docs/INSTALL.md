# Installation

## Requirements

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/) (HDT). Tested with HDT 1.59.10.
- Hearthstone

The plugin reads Hearthstone's `Power.log`. HDT turns this log on by itself, so you do not need to change any Hearthstone settings.

## Install

1. Download the ZIP file of the newest release from the [Releases page](https://github.com/critterbyte/hearthstone_xp_tracker/releases).
2. Close HDT.
3. Open the HDT plugins folder. You can find it in two ways:
   - In HDT: *Options → Tracker → Plugins → Plugins Folder*
   - In Windows Explorer: type `%APPDATA%\HearthstoneDeckTracker\Plugins` into the address bar
4. In the plugins folder, create a folder named `HearthstoneXpTracker`.
5. Copy the two files from the ZIP into this folder:
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
6. Start HDT.
7. Open *Options → Tracker → Plugins* and enable **Hearthstone XP Tracker**.

The overlay appears with the first turn of your next game. See [USAGE.md](USAGE.md) for what it shows.

Both files must be in the same folder. Without `XpTracker.Core.dll` the plugin does not start.

## Update

When a newer version exists, the plugin tells you (see [USAGE.md](USAGE.md#update-hint)). To update:

1. Download the ZIP file of the new release.
2. Close HDT.
3. Replace the two files in `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.
4. Start HDT.

Your game history and settings are kept. They are stored in a different folder (see below).

## Uninstall

1. In HDT, open *Options → Tracker → Plugins* and disable the plugin.
2. Close HDT.
3. Delete the folder `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.

The plugin keeps your data in `%APPDATA%\HearthstoneDeckTracker\XpTracker`:

| File | Content |
|---|---|
| `hs_games.csv` | Game history |
| `settings.ini` | Plugin settings |

Delete this folder too if you do not want to keep the data.

## Troubleshooting

**The plugin is not in the list, or HDT shows an error when loading it.**

- Check that both DLL files are in the same folder inside the plugins folder.
- Windows can block files downloaded from the internet. Right-click each DLL, choose *Properties*, and if there is an *Unblock* check box, select it and click *OK*. Then restart HDT.

**The overlay does not appear.**

- The overlay is shown only in Ranked, Casual and Tavern Brawl. Other modes are not measured yet.
- During the mulligan the overlay is hidden. It appears with the first turn.
- Check that HDT's own overlay works. The plugin draws on the HDT overlay.

**Something else is wrong.**

Please [open an issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) and include your HDT version and what you expected to see.

## Build from source

See [CONTRIBUTING.md](../CONTRIBUTING.md#building).
