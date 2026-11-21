# Contributing

Thank you for helping! You can contribute in several ways:

- **Report a bug** or **suggest a feature**: [open an issue](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new/choose).
- **Contribute measurements**: the formula comes from measured games. Modes such as Battlegrounds still need data, and so does the Tavern Pass boost. See [Contributing measurements](#contributing-measurements).
- **Fix or improve a translation** of the tracker or the user guides: see [Translations](#translations).
- **Contribute code or documentation**: see below.

Everyone taking part is expected to follow the [Code of Conduct](CODE_OF_CONDUCT.md). Security problems are reported privately, not as an issue: see [SECURITY.md](SECURITY.md).

## Privacy first

Hearthstone's log files contain BattleTags and Battle.net account ids (`GameAccountId=[hi=… lo=…]`) of you and your opponents. Please never post raw logs or screenshots with other players' names in issues or pull requests. Remove the names first, or share only the numbers.

Test data uses placeholders only: players such as "Alice" and "Bob", and account ids such as `[hi=1 lo=1]` (`[hi=0 lo=0]` is the computer). Never copy a log line into a test unchanged. `PrivacyTests` fails the build when a file in the repository contains a BattleTag or a real-looking account id.

## Building

Requirements:

- Windows
- [.NET SDK 10](https://dotnet.microsoft.com/download) (the version is pinned in `global.json`)

Build and test:

```powershell
.\build.ps1
```

The script builds the solution in Release mode and runs the tests. The tracker is built into `src\XpTracker.App\bin\Release\`; start `HearthstoneXpTracker.exe` there to try it. Exit a running tracker first (right-click the gold "XP" icon → "Exit"), otherwise the build cannot replace the EXE.

The calculation engine and its tests also build without Windows, for example on Linux or in CI:

```powershell
dotnet test tests\XpTracker.Core.Tests
```

Before a release, run the manual checks in [docs/TESTING.md](docs/TESTING.md).

## Project layout

| Path | Content |
|---|---|
| `src/XpTracker.Core` | Calculation engine: XP time, rates, AFK rule, log reader, game history, dashboard numbers, update check, `log.config`, Hearthstone folder. No Windows dependency. |
| `src/XpTracker.App` | Windows app (WPF, .NET Framework 4.7.2): start, notification area icon, overlay window, dashboard window, update check request. |
| `tests/XpTracker.Core.Tests` | Unit tests for the calculation engine. |
| `tools/XpTracker.LogCheck` | Replays `Power.log` files and prints the times per game. |
| `docs/` | Documentation. |

## Code

- Keep the Windows-specific code in `XpTracker.App` and everything else in `XpTracker.Core`, with tests.
- Warnings are errors, and nullable reference types are on. The build must stay free of warnings.
- Follow `.editorconfig`.
- Identifiers, comments, commit messages and documentation are in English.
- The tracker stays passive: it reads Hearthstone's log files and window position only. It never reads the game's memory, hooks into the game or changes game files. The only file outside its data folder it writes is the `[Power]` section of `log.config`.
- New libraries need a license that allows shipping them in the ZIP; add their notice to `THIRD-PARTY-NOTICES.txt`.

## Pull requests

1. Create a branch from `main`, for example `feat/arena-rates` or `fix/overlay-position`.
2. Keep the change small and focused on one topic.
3. Add or update tests.
4. Add a line to `CHANGELOG.md` under *Unreleased*.
5. Use [Conventional Commits](https://www.conventionalcommits.org/) for commit messages, for example `feat: add Arena rates` or `fix: keep the overlay above the settings button`.
6. Open a pull request against `main`. The CI build must pass.

## Contributing measurements

The game does not log reward track XP, so every data point is a game where someone wrote down the XP by hand.

1. Note the XP shown on the result screen **right after each game**. Write it down before the next game.
2. Note the mode, the result, and anything unusual: AFK turns, a concede, a disconnect, a Rewind card.
3. Get the XP time of the game:
   - from the tracker's history file (`XpSeconds` column, see [USAGE.md](docs/USAGE.md#game-history-file)), or
   - with the LogCheck tool from a copy of `Power.log` (see below).
4. Report the games with the [XP measurement](https://github.com/critterbyte/hearthstone_xp_tracker/issues/new?template=measurement.md) issue template. Say whether a Tavern Pass or another XP boost was active.

Tips:

- Hearthstone writes the log to `<Hearthstone folder>\Logs\Hearthstone_<date and time>\Power.log`. It **empties this file when you close the game**, so copy it before you quit.
- Games of different lengths help most. A short game and a long game with the same result pin the rate down much better than two games of the same length.

### Tavern Pass measurements wanted

The project follows a strict free-to-play policy and spends no money on the game, so the Tavern Pass boost has not been measured (see [FORMULA.md](docs/FORMULA.md#tavern-pass)). If you own the pass, your games are especially useful:

- Note your rewards track level with each game. The boost changes at level 20 and level 70.
- Report the XP the game showed, not the XP the tracker showed.
- Games close to a rounding step help most: they show whether the game rounds before or after adding the boost.

### LogCheck

```powershell
dotnet run --project tools\XpTracker.LogCheck -c Release -- "<path to Power.log, a .zip with a Power.log, or a folder>"
```

It prints one tab-separated line per game: date, first turn, mode, result, XP seconds, play seconds, seconds including the mulligan, AFK turns, damage and hero damage.

## Translations

The overlay and the dashboard come in the languages of the Hearthstone client. Each language is one text file in `src/XpTracker.Core/Localization/`, named after its code (`de.txt`, `es-MX.txt`, `pt-BR.txt`, …). The files are embedded in `XpTracker.Core.dll`.

To fix or improve a translation:

1. Edit the line in the language file. Each line is `key=value`; lines starting with `#` are comments.
2. Keep placeholders such as `{0}` and `{1}`; they are filled in with numbers or names.
3. Use the game's own terms of that language, as the Hearthstone client shows them: game modes, the rewards track, the Tavern Pass, class names.
4. Run `.\build.ps1`. The tests check that every language has exactly the keys of `en.txt`, the same placeholders and no empty texts.

`en.txt` is the source. A new text is added to `en.txt` first and then to every other language file. A text missing in a language falls back to English.

The user guides `INSTALL.md`, `USAGE.md` and `FARMING.md` are translated in `docs/<code>/`, with the same codes as the language files. The English files in `docs/` are the source; `FORMULA.md`, this file and the changelog stay English only. When an English guide changes, update the translations in the same pull request, or say in the pull request which translations are behind. Quoted overlay and dashboard texts must match the language file, and every heading keeps an `<a id="…"></a>` anchor with the English name, so links between the guides work in every language.

## Releases

- The version is set in `Directory.Build.props` only.
- Releases are tagged `vX.Y.Z`. The update check only recognizes tags in exactly this form.
- Until 1.0, every release is published as a pre-release.

Releases are built on GitHub, not on a developer's computer. The ZIP of every release can be traced back to the commit it was built from.

Steps:

1. Set the version in `Directory.Build.props`, move the *Unreleased* entries in `CHANGELOG.md` under a new heading `## [X.Y.Z] - YYYY-MM-DD` and add its link at the end of the file. Merge this into `main`.
2. Run the manual checks in [docs/TESTING.md](docs/TESTING.md). The CI job `app` of the pull request offers the ZIP as an artifact; `.\release.ps1` builds the same ZIP locally.
3. Tag the merged commit on `main` and push the tag:

   ```powershell
   git switch main
   git pull
   git tag vX.Y.Z
   git push origin vX.Y.Z
   ```

4. The workflow `release` (`.github/workflows/release.yml`) starts. It stops unless the tag matches the version in `Directory.Build.props`, the commit is on `main` and `CHANGELOG.md` has a section for the version. Then it runs `release.ps1`, which builds, tests, checks that the EXE and `XpTracker.Core.dll` carry the version, and creates the ZIP with only the folder `HearthstoneXpTracker` (the app, its libraries, `INSTALL.txt`, `LICENSE.txt` and `THIRD-PARTY-NOTICES.txt`) and its `.sha256` file. It records the build provenance of the ZIP with GitHub's artifact attestations and creates a **draft** release with the ZIP, the checksum file and the changelog section as notes, followed by the checksum and the commit.
5. Open the draft on the Releases page, add what users need to know (for example how to update) and publish it. Until 1.0, the draft is marked as a pre-release.

If the workflow fails, delete the tag (`git push --delete origin vX.Y.Z` and `git tag -d vX.Y.Z`), fix the cause in a pull request and tag again.

## License

By contributing, you agree that your contributions are licensed under the [MIT License](LICENSE).
