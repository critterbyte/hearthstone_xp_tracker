using System;
using System.Globalization;

namespace XpTracker.Core;

/// <summary>
/// One finished game as stored in the game history.
/// </summary>
public sealed class GameRecord
{
    public string Session { get; set; } = string.Empty;

    /// <summary>Start of the first turn (local time).</summary>
    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    public GameMode Mode { get; set; }

    public string Format { get; set; } = string.Empty;

    public GameResult Result { get; set; }

    public double XpSeconds { get; set; }

    public double PlaySeconds { get; set; }

    public double WithMulliganSeconds { get; set; }

    public int AfkTurns { get; set; }

    /// <summary>Calculated XP; null for modes or results without a measured rate.</summary>
    public int? Xp { get; set; }

    public string Deck { get; set; } = string.Empty;

    public string PlayerClass { get; set; } = string.Empty;

    public string OpponentClass { get; set; } = string.Empty;

    /// <summary>Identifies a game across repeated reads of the same log.</summary>
    public string Key => Session + "|" + Start.ToString("yyyy-MM-ddTHH:mm:ss", CultureInfo.InvariantCulture);

    public static GameRecord From(FinishedGame game)
    {
        var snapshot = game.Clock.GetSnapshot(game.End);
        var rate = XpRates.RateFor(game.Mode, game.Clock.Result);
        return new GameRecord
        {
            Session = game.Session,
            Start = game.FirstTurn,
            End = game.End,
            Mode = game.Mode,
            Format = game.Format,
            Result = game.Clock.Result,
            XpSeconds = snapshot.XpSeconds,
            PlaySeconds = snapshot.PlaySeconds,
            WithMulliganSeconds = snapshot.WithMulliganSeconds,
            AfkTurns = game.Clock.AfkTurns,
            Xp = rate is null ? null : snapshot.ExpectedXp(rate.Value),
        };
    }

    /// <summary>Start time of the Hearthstone session, read from a folder name like Hearthstone_2026_11_08_09_10_51.</summary>
    public static DateTime? SessionStart(string session) =>
        session.Length >= 31 && DateTime.TryParseExact(session.Substring(12, 19), "yyyy_MM_dd_HH_mm_ss", CultureInfo.InvariantCulture, DateTimeStyles.None, out var start)
            ? start
            : null;
}
