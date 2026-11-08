using System;

namespace XpTracker.Core;

/// <summary>
/// Reward track XP per second of XP time, measured in November 2026. See docs/FORMULA.md.
/// </summary>
public static class XpRates
{
    public const double RankedWin = 0.1308;
    public const double RankedLoss = 0.0932;

    // Casual and Tavern Brawl pay the same rates.
    public const double CasualWin = 0.0932;
    public const double CasualLoss = 0.0666;

    // XP time is capped at 30 minutes per game (reported by the community, not yet verified).
    public const double MaxXpSeconds = 1800;

    public static int ExpectedXp(double xpSeconds, double rate)
    {
        var seconds = Math.Min(Math.Max(xpSeconds, 0), MaxXpSeconds);
        return (int)Math.Round(seconds * rate, MidpointRounding.AwayFromZero);
    }
}
