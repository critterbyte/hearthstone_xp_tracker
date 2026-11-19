# Installation

## Requirements

- Windows 10 or 11. The tracker needs .NET Framework 4.7.2, which is part of both.
- Hearthstone

Hearthstone Deck Tracker (HDT) is not needed. The tracker runs on its own, or next to HDT.

The tracker reads Hearthstone's `Power.log`. Hearthstone writes this log only when it is switched on in its own settings file, `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. The tracker switches it on at its first start: it adds the `[Power]` section and changes nothing else in the file. Before that change it keeps a copy of the old file as `log.config.bak`. If HDT is installed, the log is already on.

## Install

1. Download the ZIP file of the newest release from the [Releases page](https://github.com/critterbyte/hearthstone_xp_tracker/releases), for example `HearthstoneXpTracker-0.2.0.zip`.
2. Right-click the ZIP file, choose *Properties*, and select *Unblock* if it is shown.
3. Extract the ZIP into a folder of your choice, for example *Documents*. This creates the folder `HearthstoneXpTracker` with these files:
   - `HearthstoneXpTracker.exe`: the tracker
   - `HearthstoneXpTracker.exe.config`, `XpTracker.Core.dll`, `LiveCharts.dll`, `LiveCharts.Wpf.dll` and `Newtonsoft.Json.dll`: needed by the tracker
   - `INSTALL.txt`, `LICENSE.txt` and `THIRD-PARTY-NOTICES.txt`

   Choose a folder you can write to. In *Program Files*, every update needs administrator rights.
4. Start `HearthstoneXpTracker.exe`. If Windows shows "Windows protected your PC", choose *More info* and then *Run anyway*. The tracker is not signed yet, so Windows does not know it.
5. The dashboard opens, and a gold "XP" icon appears in the notification area at the bottom right of the taskbar. It may be hidden behind the small arrow (^).

If Hearthstone was already running at the first start, restart it once. Hearthstone reads `log.config` only when it starts.

The overlay appears with the first turn of your next game. See [USAGE.md](USAGE.md) for what it shows.

All files must stay together in one folder. To start the tracker more easily, right-click `HearthstoneXpTracker.exe` and choose *Pin to Start* or *Send to → Desktop (create shortcut)*.

Each release lists the SHA-256 checksum of the ZIP. To check your download, run `Get-FileHash HearthstoneXpTracker-0.2.0.zip` in PowerShell and compare the result.

## Coming from the HDT plugin

Up to version 0.1.2, the tracker was a plugin for HDT. HDT stops supporting plugins on 30 November 2026, so the tracker is now a program of its own with the same overlay and dashboard.

- **Your data comes along.** At every start, the tracker adds the games from the plugin's history (`%APPDATA%\HearthstoneDeckTracker\XpTracker\hs_games.csv`) that it does not have yet. It also takes over the plugin's settings once, if it has none of its own. The plugin's files are only read, never changed.
- **Disable the plugin.** In HDT, open *Settings* (the gear icon on the right of the top bar) *→ Tracker → Plugins* and disable **Hearthstone XP Tracker**. Otherwise two overlays show the same numbers. You can also delete the folder `%APPDATA%\HearthstoneDeckTracker\Plugins\HearthstoneXpTracker`.
- **No deck names for now.** HDT provided the deck names, so the *Decks* tab and the deck column are gone for now. Older games keep their deck names in the history file.

## Update

When a newer version exists, the tracker tells you (see [USAGE.md](USAGE.md#update-hint)). To update:

1. Download the ZIP file of the new release and unblock it (see above).
2. Exit the tracker: right-click the gold "XP" icon and choose "Exit".
3. Extract the ZIP into the same folder again and replace the old files.
4. Start `HearthstoneXpTracker.exe`.

Your game history and settings are kept. They are stored in a different folder (see below).

To go back to an older version, do the same with its ZIP.

## Uninstall

1. Exit the tracker: right-click the gold "XP" icon and choose "Exit".
2. Delete the folder `HearthstoneXpTracker` you extracted.

The tracker keeps your data in `%APPDATA%\HearthstoneXpTracker`:

| File | Content |
|---|---|
| `hs_games.csv` | Game history |
| `settings.ini` | Settings |
| `xptracker.log` | Unexpected errors, if any |

Delete this folder too if you do not want to keep the data.

The `[Power]` section stays in `log.config`, because other tools such as HDT use it too. It only makes Hearthstone write its log. To restore the file as it was before, replace `log.config` with `log.config.bak`.

## Troubleshooting

**Windows shows "Windows protected your PC".**

- Choose *More info* and then *Run anyway*. Windows shows this for programs it does not know yet. The tracker is open source; you can read the code on GitHub and check the SHA-256 checksum of the ZIP.

**Nothing happens when I start the tracker.**

- It may already be running. Starting it again opens the dashboard of the running tracker. Look for the gold "XP" icon in the notification area, also behind the small arrow (^).
- Check that all files of the ZIP are in the same folder, and that the ZIP was unblocked (see [Install](#install)).

**The overlay does not appear.**

- The overlay is shown only in Ranked, Casual, Tavern Brawl and Arena. Other modes are not measured yet.
- During the mulligan the overlay is hidden. It appears with the first turn.
- The overlay is shown only while Hearthstone is the active window. Click into the game once.
- Look at the status line in the dashboard, below the period buttons. It should say "Game log (Power.log): on". If it asks you to restart Hearthstone, do that.

**The dashboard says the game log could not be switched on.**

- The tracker could not write `%LOCALAPPDATA%\Blizzard\Hearthstone\log.config`. Close Hearthstone, open the file in Notepad (create it if it is missing), add these lines at the end, and save it:

  ```ini
  [Power]
  LogLevel=1
  FilePrinting=True
  ConsolePrinting=False
  ScreenPrinting=False
  Verbose=True
  ```

**Something else is wrong.**

Please [open an issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose) and include the tracker version (the dashboard shows it in the top right corner), your Windows version and what you expected to see. If `%APPDATA%\HearthstoneXpTracker\xptracker.log` exists, its last lines help too.

## Build from source

See [CONTRIBUTING.md](../CONTRIBUTING.md#building).
