# Installation

## Requirements

- Windows
- [Hearthstone Deck Tracker](https://hsreplay.net/downloads/) (HDT). Tested with HDT 1.60.1.
- Hearthstone

The plugin reads Hearthstone's `Power.log`. HDT turns this log on by itself, so you do not need to change any Hearthstone settings.

## Install

1. Download the ZIP file of the newest release from the [Releases page](https://github.com/critterbyte/hearthstone_xp_tracker/releases), for example `HearthstoneXpTracker-0.1.0.zip`.
2. Close HDT.
3. Right-click the ZIP file, choose *Properties*, and select *Unblock* if it is shown. Otherwise Windows may block the plugin.
4. Open the HDT plugins folder. You can find it in two ways:
   - In HDT: *Settings* (the gear icon on the right of the top bar) *→ Tracker → Plugins → Plugins Folder*
   - In Windows Explorer: type `%APPDATA%\HearthstoneDeckTracker\Plugins` into the address bar
5. Extract the ZIP into the plugins folder. This creates the folder `HearthstoneXpTracker` with these files:
   - `HearthstoneXpTracker.dll`
   - `XpTracker.Core.dll`
   - `INSTALL.txt` and `LICENSE.txt`
6. Start HDT.
7. Open *Settings → Tracker → Plugins* and enable **Hearthstone XP Tracker**.

The overlay appears with the first turn of your next game. See [USAGE.md](USAGE.md) for what it shows.

Both DLL files must be in the same folder. Without `XpTracker.Core.dll` the plugin does not start.

Each release lists the SHA-256 checksum of the ZIP. To check your download, run `Get-FileHash HearthstoneXpTracker-0.1.0.zip` in PowerShell and compare the result.

## Update

When a newer version exists, the plugin tells you (see [USAGE.md](USAGE.md#update-hint)). To update:

1. Download the ZIP file of the new release and unblock it (see above).
2. Close HDT.
3. Extract the ZIP into the plugins folder again and replace the old files.
4. Start HDT.

Your game history and settings are kept. They are stored in a different folder (see below).

## Uninstall

1. In HDT, open *Settings → Tracker → Plugins* and disable the plugin.
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

- Check that both DLL files are in `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`, not one folder deeper (for example `...\HearthstoneXpTracker\HearthstoneXpTracker`).
- Windows can block files downloaded from the internet. Right-click each DLL, choose *Properties*, and if there is an *Unblock* check box, select it and click *OK*. Then restart HDT.

**The overlay does not appear.**

- The overlay is shown only in Ranked, Casual, Tavern Brawl and Arena. Other modes are not measured yet.
- During the mulligan the overlay is hidden. It appears with the first turn.
- Check that HDT's own overlay works. The plugin draws on the HDT overlay.

**After installing an older version, HDT still runs the newer one.**

- HDT does not load the plugin from `%APPDATA%\HearthstoneDeckTracker\Plugins`. It loads a copy in its own program folder, `%LOCALAPPDATA%\HearthstoneDeckTracker\app-<HDT version>\Plugins`, and replaces a file there only when the file in `%APPDATA%` is newer. The files of an older release are older, so HDT keeps the newer copy.
- To go back to an older version: close HDT, install the older version as described above, delete the folder `HearthstoneXpTracker` in `%LOCALAPPDATA%\HearthstoneDeckTracker\app-<HDT version>\Plugins`, and start HDT again. HDT then copies the plugin again.
- From version 0.1.2 on, the dashboard shows the running version in the top right corner.
- Updates to a newer version are not affected.

**Something else is wrong.**

Please [open an issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) and include your HDT version and what you expected to see.

## Build from source

See [CONTRIBUTING.md](../CONTRIBUTING.md#building).
