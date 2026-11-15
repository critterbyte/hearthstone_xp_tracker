using System;

namespace XpTracker.Core;

/// <summary>
/// Reward track XP per second of XP time, measured in November 2026. See docs/FORMULA.md.
/// </summary>
public static class XpRates
{
    // Measured range 0.130505 to 0.13058, this value is the middle.
    // 0.1308 was used until a 417.37 s win gave 54 XP instead of 55,
    // 0.1305 until a 425.27 s win gave 56 XP instead of 55.
    public const double RankedWin = 0.13054;

    // Measured range 0.09317 to 0.09322.
    public const double RankedLoss = 0.0932;

    // Casual, Tavern Brawl and Arena pay the same rates (Arena measured in 4 games in November 2026).
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
            GameMode.Casual or GameMode.TavernBrawl or GameMode.Arena => win ? CasualWin : CasualLoss,

            // Measured: losses against the computer in a Tavern Brawl give nothing. Wins are assumed to give nothing too.
            GameMode.TavernBrawlPvE => 0,
            _ => null,
        };
    }

    /// <summary>
    /// True for modes with measured rates. Games of other modes (for example Battlegrounds) are stored without XP
    /// and do not count in the dashboard's XP time, XP per hour, win rate, decks and charts.
    /// </summary>
    public static bool IsMeasured(GameMode mode) => RateFor(mode, GameResult.Win) is not null;

    /// <summary>Event XP of a game in this mode, or null if not known for the mode (for example Battlegrounds).</summary>
    public static int? EventXpFor(GameMode mode, double xpSeconds) => mode switch
    {
        GameMode.Ranked or GameMode.Casual or GameMode.TavernBrawl or GameMode.Arena => EventXp(xpSeconds),
        GameMode.TavernBrawlPvE => 0,
        _ => null,
    };

    /// <summary>XP by the formula alone, without the minimum and the cap; used to show what was lost.</summary>
    /// <param name="bonusPercent">Tavern Pass XP boost in percent, applied before rounding (see <see cref="TavernPassBonus"/>).</param>
    public static int FormulaXp(double xpSeconds, double rate, int bonusPercent = 0) =>
        Round(Math.Max(xpSeconds, 0) * rate, bonusPercent);

    /// <summary>
    /// Event XP during a Hearthstone event: one per minute of XP time, rounded half up (30 s and more count as a minute).
    /// Measured in November 2026 in Ranked and Casual: no 60-second minimum, no win bonus, same in both modes.
    /// Whether event XP is capped at 30 minutes is not known yet, so no cap is applied.
    /// </summary>
    public static int EventXp(double xpSeconds) =>
        (int)Math.Round(Math.Max(xpSeconds, 0) / 60, MidpointRounding.AwayFromZero);

    /// <param name="bonusPercent">Tavern Pass XP boost in percent, applied before rounding (see <see cref="TavernPassBonus"/>).</param>
    public static int ExpectedXp(double xpSeconds, double rate, int bonusPercent = 0)
    {
        if (xpSeconds < MinXpSeconds)
        {
            return 0;
        }

        var seconds = Math.Min(xpSeconds, MaxXpSeconds);
        return Round(seconds * rate, bonusPercent);
    }

    // The boost is applied before rounding: round(seconds × rate × (1 + boost)). Not measured yet, see docs/FORMULA.md.
    // Without a boost the XP is exactly round(seconds × rate) as before.
    private static int Round(double baseXp, int bonusPercent)
    {
        var xp = bonusPercent == 0 ? baseXp : baseXp * (100 + bonusPercent) / 100;
        return (int)Math.Round(xp, MidpointRounding.AwayFromZero);
    }
}
