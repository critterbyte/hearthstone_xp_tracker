using System;

namespace XpTracker.Core;

/// <summary>
/// Reward track XP per second of XP time, measured in November 2026. See docs/FORMULA.md.
/// </summary>
public static class XpRates
{
    // Measured range 0.13047 to 0.13058; 0.1308 was used until a 417.37 s win gave 54 XP instead of 55.
    public const double RankedWin = 0.1305;
    public const double RankedLoss = 0.0932;

    // Casual and Tavern Brawl pay the same rates.
    public const double CasualWin = 0.0932;
    public const double CasualLoss = 0.0666;

    // XP time is capped at about 30 minutes per game (stated by a Blizzard developer in 2020, not measured yet).
    public const double MaxXpSeconds = 1800;

    // Games with less XP time give no XP at all (measured for own concedes: 59.89 s gave 0, 60.78 s gave 6).
    public const double MinXpSeconds = 60;

    /// <summary>
    /// Returns the rate for a mode and result, or null if it has not been measured (other modes, ties).
    /// </summary>
    public static double? RateFor(GameMode mode, GameResult result)
    {
        if (result != GameResult.Win && result != GameResult.Loss)
        {
            return null;
        }

        var win = result == GameResult.Win;
        return mode switch
        {
            GameMode.Ranked => win ? RankedWin : RankedLoss,
            GameMode.Casual or GameMode.TavernBrawl => win ? CasualWin : CasualLoss,
            _ => null,
        };
    }

    /// <summary>XP by the formula alone, without the minimum and the cap; used to show what was lost.</summary>
    public static int FormulaXp(double xpSeconds, double rate) =>
        (int)Math.Round(Math.Max(xpSeconds, 0) * rate, MidpointRounding.AwayFromZero);

    /// <summary>
    /// Event XP during a Hearthstone event: one per minute of XP time, rounded half up (30 s and more count as a minute).
    /// Measured in November 2026 in Ranked and Casual: no 60-second minimum, no win bonus, same in both modes.
    /// Whether event XP is capped at 30 minutes is not known yet, so no cap is applied.
    /// </summary>
    public static int EventXp(double xpSeconds) =>
        (int)Math.Round(Math.Max(xpSeconds, 0) / 60, MidpointRounding.AwayFromZero);

    public static int ExpectedXp(double xpSeconds, double rate)
    {
        if (xpSeconds < MinXpSeconds)
        {
            return 0;
        }

        var seconds = Math.Min(xpSeconds, MaxXpSeconds);
        return (int)Math.Round(seconds * rate, MidpointRounding.AwayFromZero);
    }
}
