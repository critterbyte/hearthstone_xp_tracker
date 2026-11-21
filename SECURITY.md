# Security policy

## Supported versions

Only the newest release gets security fixes. The tracker tells you when a newer version is available; please update before reporting.

| Version | Supported |
|---|---|
| Newest release on the [Releases page](https://github.com/critterbyte/hearthstone_xp_tracker/releases) | Yes |
| Older releases | No |

## Reporting a vulnerability

Please **do not open a public issue** for a security problem.

Report it privately instead: on the repository's **Security** tab, choose **Report a vulnerability**. This opens a private advisory that only you and the maintainer can see. See GitHub's guide on [privately reporting a security vulnerability](https://docs.github.com/en/code-security/security-advisories/guidance-on-reporting-and-writing-information-about-vulnerabilities/privately-reporting-a-security-vulnerability).

Please include:

- the tracker version and your Windows version,
- what the problem is and what an attacker could do with it,
- steps to reproduce it.

Do not include logs with BattleTags or account ids; remove them first.

This is a hobby project maintained by one person, so the answer may take a few days. Once the problem is fixed and a new release is out, the advisory is published with credit to you, unless you prefer to stay anonymous.

## Scope

Examples of what counts as a security problem:

- The tracker reads or sends data it should not, for example anything beyond the program name and version in the update check.
- The update hint can be made to link to a site outside this repository.
- A crafted `Power.log`, `hs_games.csv`, `settings.ini` or `log.config` makes the tracker run code or write files outside its data folder and `log.config`.

Not in scope: bugs in Hearthstone itself (please report those to Blizzard), and wrong XP values (please open a normal issue).

## Checking a release

From version 0.2.0 on, every release is built on GitHub by the workflow `release` from a tagged commit on `main`. The release notes name that commit. To check that a downloaded ZIP was built there and not changed since, install the [GitHub CLI](https://cli.github.com/) and run:

```
gh attestation verify HearthstoneXpTracker-X.Y.Z.zip -R critterbyte/hearthstone_xp_tracker
```

The SHA-256 checksum in the release notes and in the `.sha256` file only shows that the download is complete and unchanged.
