using System;
using System.Collections.Generic;
using System.Globalization;

namespace XpTracker.Core;

/// <summary>
/// The lines shown on the overlay: live during a game, a summary of the last game after it.
/// </summary>
public static class XpDisplayText
{
    public static IReadOnlyList<string> For(GameObserver observer, DateTime now)
    {
        var clock = observer.Clock;
        var winRate = XpRates.RateFor(observer.Mode, GameResult.Win);
        var lossRate = XpRates.RateFor(observer.Mode, GameResult.Loss);
        if (clock is null || !clock.HasStarted || winRate is null || lossRate is null)
        {
            return Array.Empty<string>();
        }

        var snapshot = clock.GetSnapshot(now);
        var xpTime = "XP time " + TimeFormat.MinutesSeconds(Math.Min(snapshot.XpSeconds, XpRates.MaxXpSeconds)) + Suffix(snapshot, clock.IsOver);

        if (!clock.IsOver)
        {
            return new[]
            {
                xpTime,
                Format("If win: +{0} XP", snapshot.ExpectedXp(winRate.Value)),
                Format("If loss: +{0} XP", snapshot.ExpectedXp(lossRate.Value)),
            };
        }

        var rate = XpRates.RateFor(observer.Mode, clock.Result);
        return new[]
        {
            clock.Result is GameResult.Unknown ? "Last game" : "Last game: " + clock.Result,
            xpTime,
            "Total time " + TimeFormat.MinutesSeconds(snapshot.WithMulliganSeconds),
            rate is null ? "XP unknown" : Format("+{0} XP", snapshot.ExpectedXp(rate.Value)),
        };
    }

    private static string Suffix(XpSnapshot snapshot, bool isOver)
    {
        if (snapshot.XpSeconds >= XpRates.MaxXpSeconds)
        {
            return " (max)";
        }

        return !isOver && snapshot.IsPaused ? " (paused)" : string.Empty;
    }

    private static string Format(string format, int value) => string.Format(CultureInfo.InvariantCulture, format, value);
}
