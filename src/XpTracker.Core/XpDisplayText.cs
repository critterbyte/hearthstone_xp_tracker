using System;
using System.Collections.Generic;
using System.Globalization;

namespace XpTracker.Core;

/// <summary>
/// One overlay row: a label and a value; XP values are shown with an XP icon.
/// </summary>
public sealed class XpLine
{
    public XpLine(string label, string value, bool isXp = false)
    {
        Label = label;
        Value = value;
        IsXp = isXp;
    }

    public string Label { get; }

    public string Value { get; }

    public bool IsXp { get; }

    public override string ToString() => Label + " " + Value;
}

/// <summary>
/// The rows shown on the overlay: live during a game, a summary of the last game after it.
/// </summary>
public static class XpDisplayText
{
    public static IReadOnlyList<XpLine> For(XpClock? clock, GameMode mode, DateTime now)
    {
        var winRate = XpRates.RateFor(mode, GameResult.Win);
        var lossRate = XpRates.RateFor(mode, GameResult.Loss);
        if (clock is null || !clock.HasStarted || winRate is null || lossRate is null)
        {
            return Array.Empty<XpLine>();
        }

        var snapshot = clock.GetSnapshot(now);
        var xpTime = new XpLine("XP time:", TimeFormat.MinutesSeconds(Math.Min(snapshot.XpSeconds, XpRates.MaxXpSeconds)) + Suffix(snapshot, clock.IsOver));

        if (!clock.IsOver)
        {
            return new[]
            {
                xpTime,
                new XpLine("If win:", Plus(snapshot.ExpectedXp(winRate.Value)), isXp: true),
                new XpLine("If loss:", Plus(snapshot.ExpectedXp(lossRate.Value)), isXp: true),
            };
        }

        var rate = XpRates.RateFor(mode, clock.Result);
        return new[]
        {
            new XpLine("Last game:", clock.Result is GameResult.Unknown ? "?" : clock.Result.ToString()),
            xpTime,
            new XpLine("Total time:", TimeFormat.MinutesSeconds(snapshot.WithMulliganSeconds)),
            rate is null ? new XpLine("Earned:", "unknown") : new XpLine("Earned:", Plus(snapshot.ExpectedXp(rate.Value)), isXp: true),
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

    private static string Plus(int value) => "+" + value.ToString(CultureInfo.InvariantCulture);
}
