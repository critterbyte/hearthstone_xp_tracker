using System;
using System.Collections.Generic;
using System.Linq;

namespace XpTracker.Core;

public enum DashboardPeriod
{
    Today,
    Week,

    /// <summary>The ranked season, which runs for a calendar month.</summary>
    Season,
    All,
}

public sealed class SummaryStats
{
    public int Games { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public int Xp { get; set; }

    public double XpSeconds { get; set; }

    /// <summary>Game time including the mulligan.</summary>
    public double PlaySeconds { get; set; }

    /// <summary>Time Hearthstone was running in the sessions of the period.</summary>
    public double OnlineSeconds { get; set; }

    public double XpPerHour => XpSeconds > 0 ? Xp / (XpSeconds / 3600) : 0;

    public double XpPerOnlineHour => OnlineSeconds > 0 ? Xp / (OnlineSeconds / 3600) : 0;
}

public sealed class DeckStats
{
    public string Deck { get; set; } = string.Empty;

    public int Games { get; set; }

    public int Wins { get; set; }

    public int Xp { get; set; }

    public double XpSeconds { get; set; }

    public double PlaySeconds { get; set; }

    public double WinRate => Games > 0 ? (double)Wins / Games : 0;

    public double XpPerHour => XpSeconds > 0 ? Xp / (XpSeconds / 3600) : 0;

    public double AverageGameSeconds => Games > 0 ? PlaySeconds / Games : 0;
}

public sealed class LostXpStats
{
    public int AfkTurns { get; set; }

    /// <summary>XP the AFK turns and the opponent turns after them would have been worth.</summary>
    public int AfkXp { get; set; }

    public int ShortGames { get; set; }

    /// <summary>XP the formula gives for games below the minimum XP time, which the game does not award.</summary>
    public int ShortGamesXp { get; set; }

    /// <summary>Online time outside games (menus, queue, deck building).</summary>
    public double OutsideGamesSeconds { get; set; }

    /// <summary>XP the time outside games would have been worth at the period's XP per hour.</summary>
    public int OutsideGamesXp { get; set; }
}

public sealed class DayStats
{
    public DateTime Date { get; set; }

    public int Xp { get; set; }

    public double XpSeconds { get; set; }

    public double XpPerHour => XpSeconds > 0 ? Xp / (XpSeconds / 3600) : 0;
}

/// <summary>
/// Aggregates the game history for the dashboard.
/// </summary>
public static class Dashboard
{
    public static DateTime PeriodStart(DashboardPeriod period, DateTime now) => period switch
    {
        DashboardPeriod.Today => now.Date,
        DashboardPeriod.Week => now.Date.AddDays(-(((int)now.DayOfWeek + 6) % 7)),
        DashboardPeriod.Season => new DateTime(now.Year, now.Month, 1),
        _ => DateTime.MinValue,
    };

    public static IReadOnlyList<GameRecord> InPeriod(IEnumerable<GameRecord> games, DashboardPeriod period, DateTime now)
    {
        var start = PeriodStart(period, now);
        return games.Where(g => g.Start >= start).OrderBy(g => g.Start).ToList();
    }

    /// <param name="currentSession">Session that is still running; its online time counts up to <paramref name="now"/>.</param>
    public static SummaryStats Summary(IReadOnlyList<GameRecord> games, DateTime periodStart, DateTime now, string? currentSession)
    {
        return new SummaryStats
        {
            Games = games.Count,
            Wins = games.Count(g => g.Result == GameResult.Win),
            Losses = games.Count(g => g.Result == GameResult.Loss),
            Xp = games.Sum(g => g.Xp ?? 0),
            XpSeconds = games.Sum(g => g.XpSeconds),
            PlaySeconds = games.Sum(g => g.WithMulliganSeconds),
            OnlineSeconds = OnlineSeconds(games, periodStart, now, currentSession),
        };
    }

    public static IReadOnlyList<DeckStats> Decks(IEnumerable<GameRecord> games) =>
        games.GroupBy(g => g.Deck.Length > 0 ? g.Deck : "(unknown)")
            .Select(group => new DeckStats
            {
                Deck = group.Key,
                Games = group.Count(),
                Wins = group.Count(g => g.Result == GameResult.Win),
                Xp = group.Sum(g => g.Xp ?? 0),
                XpSeconds = group.Sum(g => g.XpSeconds),
                PlaySeconds = group.Sum(g => g.WithMulliganSeconds),
            })
            .OrderByDescending(d => d.XpPerHour)
            .ToList();

    public static LostXpStats Lost(IReadOnlyList<GameRecord> games, SummaryStats summary)
    {
        var lost = new LostXpStats();
        foreach (var game in games)
        {
            var rate = XpRates.RateFor(game.Mode, game.Result);
            lost.AfkTurns += game.AfkTurns;
            if (rate is null)
            {
                continue;
            }

            if (game.AfkTurns > 0)
            {
                lost.AfkXp += Math.Max(0, XpRates.ExpectedXp(game.PlaySeconds, rate.Value) - (game.Xp ?? 0));
            }

            if (game.XpSeconds < XpRates.MinXpSeconds)
            {
                lost.ShortGames++;
                lost.ShortGamesXp += XpRates.FormulaXp(game.XpSeconds, rate.Value);
            }
        }

        lost.OutsideGamesSeconds = Math.Max(0, summary.OnlineSeconds - summary.PlaySeconds);
        lost.OutsideGamesXp = (int)Math.Round(lost.OutsideGamesSeconds / 3600 * summary.XpPerHour);
        return lost;
    }

    public static IReadOnlyList<DayStats> Days(IEnumerable<GameRecord> games) =>
        games.GroupBy(g => g.Start.Date)
            .OrderBy(group => group.Key)
            .Select(group => new DayStats
            {
                Date = group.Key,
                Xp = group.Sum(g => g.Xp ?? 0),
                XpSeconds = group.Sum(g => g.XpSeconds),
            })
            .ToList();

    // Each session runs from Hearthstone's start (log folder name) to its last game, or to now while it is running.
    private static double OnlineSeconds(IEnumerable<GameRecord> games, DateTime periodStart, DateTime now, string? currentSession)
    {
        double total = 0;
        foreach (var session in games.GroupBy(g => g.Session))
        {
            var start = GameRecord.SessionStart(session.Key) ?? session.Min(g => g.Start.AddSeconds(g.PlaySeconds - g.WithMulliganSeconds));
            var end = session.Key.Length > 0 && session.Key == currentSession ? now : session.Max(g => g.End);
            if (start < periodStart)
            {
                start = periodStart;
            }

            total += Math.Max(0, (end - start).TotalSeconds);
        }

        return total;
    }
}
