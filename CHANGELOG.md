# Changelog

All notable changes to this project are documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.1.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [Unreleased]

### Added

- Project skeleton: solution, calculation engine (`XpTracker.Core`), plugin entry point (`XpTracker.Plugin`), unit tests.
- XP rates for Ranked, Casual and Tavern Brawl with tests against measured games.
- Documentation of the XP formula (`docs/FORMULA.md`).
- Local build script (`build.ps1`) and CI workflow for the calculation engine.
- `XpClock`: tracks XP time, play time and time including the mulligan from turn events, live and after the game. AFK turns and the opponent turn that follows them are excluded.
- `XpRates.RateFor`: rate lookup by game mode and result.
