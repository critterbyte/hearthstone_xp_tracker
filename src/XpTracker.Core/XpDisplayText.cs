using System;
using System.Collections.Generic;
using System.Globalization;

namespace XpTracker.Core;

/// <summary>
/// One overlay row: a label and a value; XP values are shown with an XP icon, event XP with a crystal,
/// and the update hint with a highlighted label.
/// </summary>
public sealed class XpLine
{
    public XpLine(string label, string value, bool isXp = false, bool isEvent = false, bool isUpdate = false)
    {
        Label = label;
        Value = value;
        IsXp = isXp;
        IsEvent = isEvent;
        IsUpdate = isUpdate;
    }

    public string Label { get; }

    public string Value { get; }

    public bool IsXp { get; }

    public bool IsEvent { get; }

    /// <summary>The "Update available:" row; the overlay shows its label in mana blue so it catches the eye.</summary>
    public bool IsUpdate { get; }

    public override string ToString() => Label + " " + Value;
}

/// <summary>
/// The rows shown on the overlay: live during a game, a summary of the last game after it.
/// </summary>
public static class XpDisplayText
{
    /// <param name="eventActive">A Hearthstone event is running: adds an "Event XP:" row with the event XP.</param>
    /// <param name="damageDealt">Damage dealt to the opponent's side so far: adds a "Total Damage:" row; null hides the damage rows.</param>
    /// <param name="xpBonus">Tavern Pass XP boost in percent for "If win:", "If loss:" and "Earned:"; event XP get no boost.</param>
    /// <param name="heroDamageDealt">The part of the damage that hit the opponent's hero: adds a "Hero Damage:" row below "Total Damage:".</param>
    public static IReadOnlyList<XpLine> For(XpClock? clock, GameMode mode, DateTime now, bool eventActive = false, int? damageDealt = null, int xpBonus = 0, int? heroDamageDealt = null)
    {
        var winRate = XpRates.RateFor(mode, GameResult.Win);
        var lossRate = XpRates.RateFor(mode, GameResult.Loss);
        if (clock is null || !clock.HasStarted || winRate is null || lossRate is null)
        {
            return Array.Empty<XpLine>();
        }

        var snapshot = clock.GetSnapshot(now);
        var xpTime = new XpLine(Localizer.Text("overlay.xpTime"), TimeFormat.MinutesSeconds(Math.Min(snapshot.XpSeconds, XpRates.MaxXpSeconds)) + Suffix(snapshot, clock.IsOver));

        List<XpLine> lines;
        if (!clock.IsOver)
        {
            lines = new List<XpLine>
            {
                xpTime,
                new XpLine(Localizer.Text("overlay.ifWin"), Plus(snapshot.ExpectedXp(winRate.Value, xpBonus)), isXp: true),
                new XpLine(Localizer.Text("overlay.ifLoss"), Plus(snapshot.ExpectedXp(lossRate.Value, xpBonus)), isXp: true),
            };
        }
        else
        {
            var rate = XpRates.RateFor(mode, clock.Result);
            lines = new List<XpLine>
            {
                new XpLine(Localizer.Text("overlay.lastGame"), clock.Result is GameResult.Unknown ? "?" : GameText.Result(clock.Result)),
                xpTime,
                new XpLine(Localizer.Text("overlay.totalTime"), TimeFormat.MinutesSeconds(snapshot.WithMulliganSeconds)),
                rate is null ? new XpLine(Localizer.Text("overlay.earned"), Localizer.Text("overlay.unknown")) : new XpLine(Localizer.Text("overlay.earned"), Plus(snapshot.ExpectedXp(rate.Value, xpBonus)), isXp: true),
            };
        }

        // Event XP does not depend on the result, so one row is enough, live and after the game.
        if (eventActive)
        {
            lines.Add(new XpLine(Localizer.Text("overlay.eventXp"), Plus(XpRates.EventXpFor(mode, snapshot.XpSeconds) ?? 0), isEvent: true));
        }

        if (damageDealt is { } damage)
        {
            lines.Add(new XpLine(Localizer.Text("overlay.totalDamage"), damage.ToString(CultureInfo.InvariantCulture)));
            if (heroDamageDealt is { } heroDamage)
            {
                lines.Add(new XpLine(Localizer.Text("overlay.heroDamage"), heroDamage.ToString(CultureInfo.InvariantCulture)));
            }
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

        return new List<XpLine>(lines) { new XpLine(Localizer.Text("overlay.updateAvailable"), newVersion!, isUpdate: true) };
    }

    private static string Suffix(XpSnapshot snapshot, bool isOver)
    {
        if (snapshot.XpSeconds >= XpRates.MaxXpSeconds)
        {
            return " " + Localizer.Text("overlay.max");
        }

        return !isOver && snapshot.IsPaused ? " " + Localizer.Text("overlay.paused") : string.Empty;
    }

    private static string Plus(int value) => "+" + value.ToString(CultureInfo.InvariantCulture);
}
