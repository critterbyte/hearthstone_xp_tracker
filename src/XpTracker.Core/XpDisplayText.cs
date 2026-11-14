using System;
using System.Collections.Generic;
using System.Globalization;

namespace XpTracker.Core;

/// <summary>
/// One overlay row: a label and a value; XP values are shown with an XP icon, event XP with a crystal.
/// </summary>
public sealed class XpLine
{
    public XpLine(string label, string value, bool isXp = false, bool isEvent = false)
    {
        Label = label;
        Value = value;
        IsXp = isXp;
        IsEvent = isEvent;
    }

    public string Label { get; }

    public string Value { get; }

    public bool IsXp { get; }

    public bool IsEvent { get; }

    public override string ToString() => Label + " " + Value;
}

/// <summary>
/// The rows shown on the overlay: live during a game, a summary of the last game after it.
/// </summary>
public static class XpDisplayText
{
    /// <param name="eventActive">A Hearthstone event is running: adds an "Event XP:" row with the event XP.</param>
    /// <param name="damageDealt">Damage dealt to the opponent's side so far: adds a "Damage:" row as the last row.</param>
    public static IReadOnlyList<XpLine> For(XpClock? clock, GameMode mode, DateTime now, bool eventActive = false, int? damageDealt = null)
    {
        var winRate = XpRates.RateFor(mode, GameResult.Win);
        var lossRate = XpRates.RateFor(mode, GameResult.Loss);
        if (clock is null || !clock.HasStarted || winRate is null || lossRate is null)
        {
            return Array.Empty<XpLine>();
        }

        var snapshot = clock.GetSnapshot(now);
        var xpTime = new XpLine("XP time:", TimeFormat.MinutesSeconds(Math.Min(snapshot.XpSeconds, XpRates.MaxXpSeconds)) + Suffix(snapshot, clock.IsOver));

        List<XpLine> lines;
        if (!clock.IsOver)
        {
            lines = new List<XpLine>
            {
                xpTime,
                new XpLine("If win:", Plus(snapshot.ExpectedXp(winRate.Value)), isXp: true),
                new XpLine("If loss:", Plus(snapshot.ExpectedXp(lossRate.Value)), isXp: true),
            };
        }
        else
        {
            var rate = XpRates.RateFor(mode, clock.Result);
            lines = new List<XpLine>
            {
                new XpLine("Last game:", clock.Result is GameResult.Unknown ? "?" : clock.Result.ToString()),
                xpTime,
                new XpLine("Total time:", TimeFormat.MinutesSeconds(snapshot.WithMulliganSeconds)),
                rate is null ? new XpLine("Earned:", "unknown") : new XpLine("Earned:", Plus(snapshot.ExpectedXp(rate.Value)), isXp: true),
            };
        }

        // Event XP does not depend on the result, so one row is enough, live and after the game.
        if (eventActive)
        {
            lines.Add(new XpLine("Event XP:", Plus(XpRates.EventXpFor(mode, snapshot.XpSeconds) ?? 0), isEvent: true));
        }

        if (damageDealt is { } damage)
        {
            lines.Add(new XpLine("Damage:", damage.ToString(CultureInfo.InvariantCulture)));
        }

        return lines;
    }

    /// <summary>
    /// Adds an "Update available:" row when a newer version exists and no game is running
    /// (in the menu or under the last game summary); never during a game.
    /// </summary>
    public static IReadOnlyList<XpLine> WithUpdate(IReadOnlyList<XpLine> lines, bool gameRunning, string? newVersion)
    {
        if (gameRunning || string.IsNullOrEmpty(newVersion))
        {
            return lines;
        }

        return new List<XpLine>(lines) { new XpLine("Update available:", newVersion!) };
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
