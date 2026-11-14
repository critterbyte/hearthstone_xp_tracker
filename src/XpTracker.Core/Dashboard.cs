using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace XpTracker.Core;

public enum DashboardPeriod
{
    /// <summary>One Hearthstone session: from the start of the game client until it was closed (or until now).</summary>
    Session,
    Today,
    Week,

    /// <summary>The ranked season, which runs for a calendar month.</summary>
    Season,

    /// <summary>The last 30 calendar days, including today.</summary>
    Last30Days,
    All,
}

/// <summary>
/// One step of the chart time axis: an hour of today, or a day of a longer period, with the games that started in it.
/// </summary>
public sealed class TimeBucket
{
    public DateTime Start { get; set; }

    public DateTime End { get; set; }

    /// <summary>Axis label, for example "20" (hour), "Mon 11-09" (weekday), "9" (day of the month) or "11-09".</summary>
    public string Label { get; set; } = string.Empty;

    /// <summary>Full name of the step for tooltips, for example "Mon 2026-11-09, 20:00–21:00" or "Mon 2026-11-09".</summary>
    public string Title => End - Start < TimeSpan.FromDays(1)
        ? Start.ToString("ddd yyyy-MM-dd, HH:mm", Localizer.Culture) + "–" + End.ToString("HH:mm", Localizer.Culture)
        : Start.ToString("ddd yyyy-MM-dd", Localizer.Culture);

    public int Games { get; set; }

    public int Xp { get; set; }

    public double XpSeconds { get; set; }

    /// <summary>XP per hour of XP time, or null when no game started in this step.</summary>
    public double? XpPerHour => XpSeconds > 0 ? Xp / (XpSeconds / 3600) : null;
}

public sealed class SummaryStats
{
    public int Games { get; set; }

    public int Wins { get; set; }

    public int Losses { get; set; }

    public int Xp { get; set; }

    /// <summary>Event XP of the games played during an event.</summary>
    public int EventXp { get; set; }

    /// <summary>At least one game of the period was played during an event.</summary>
    public bool HasEventXp { get; set; }

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
    /// <summary>Shown for games without a deck name (HDT did not record the game).</summary>
    public static string NoDeck => Localizer.Text("deck.none");

    public static DateTime PeriodStart(DashboardPeriod period, DateTime now) => period switch
    {
        DashboardPeriod.Today => now.Date,
        DashboardPeriod.Week => now.Date.AddDays(-(((int)now.DayOfWeek + 6) % 7)),
        DashboardPeriod.Season => new DateTime(now.Year, now.Month, 1),
        DashboardPeriod.Last30Days => now.Date.AddDays(-29),
        _ => DateTime.MinValue,
    };

    /// <summary>
    /// The time axis of the charts, always complete so gaps stay visible: the 24 hours of today, the 7 days of the week
    /// from Monday, every day of the month (season), the last 30 days, or every day since the first game (all time).
    /// Each game counts in the step in which it started.
    /// </summary>
    public static IReadOnlyList<TimeBucket> Buckets(IReadOnlyList<GameRecord> games, DashboardPeriod period, DateTime now)
    {
        var buckets = new List<TimeBucket>();
        var today = now.Date;
        switch (period)
        {
            case DashboardPeriod.Today:
                for (var hour = 0; hour < 24; hour++)
                {
                    buckets.Add(new TimeBucket { Start = today.AddHours(hour), End = today.AddHours(hour + 1), Label = hour.ToString("00", CultureInfo.InvariantCulture) });
                }

                break;
            case DashboardPeriod.Week:
                AddDays(buckets, PeriodStart(period, now), 7, "ddd MM-dd");
                break;
            case DashboardPeriod.Season:
                AddDays(buckets, PeriodStart(period, now), DateTime.DaysInMonth(now.Year, now.Month), "%d");
                break;
            case DashboardPeriod.Last30Days:
                AddDays(buckets, PeriodStart(period, now), 30, "MM-dd");
                break;
            default:
                if (games.Count == 0)
                {
                    return buckets;
                }

                var first = games.Min(g => g.Start).Date;
                AddDays(buckets, first, Math.Max(1, (int)(today - first).TotalDays + 1), "MM-dd");
                break;
        }

        Fill(buckets, games);
        return buckets;
    }

    /// <summary>Games of one Hearthstone session (log folder name), ordered by start.</summary>
    public static IReadOnlyList<GameRecord> InSession(IEnumerable<GameRecord> games, string? session) =>
        session is null
            ? Array.Empty<GameRecord>()
            : games.Where(g => g.Session == session).OrderBy(g => g.Start).ToList();

    /// <summary>
    /// The session to show: the one the log reader is reading (also after Hearthstone was closed, until it starts again),
    /// otherwise the newest session in the history.
    /// </summary>
    public static string? SelectSession(IEnumerable<GameRecord> games, string? logSession) =>
        logSession ?? games.OrderBy(g => g.Start).LastOrDefault()?.Session;

    /// <summary>
    /// Hourly time axis of a session, from the hour it started to the hour it ended (or now while it runs);
    /// it may run past midnight. Each game counts in the hour in which it started.
    /// </summary>
    public static IReadOnlyList<TimeBucket> SessionBuckets(IReadOnlyList<GameRecord> games, DateTime sessionStart, DateTime sessionEnd)
    {
        var buckets = new List<TimeBucket>();
        var first = new DateTime(sessionStart.Year, sessionStart.Month, sessionStart.Day, sessionStart.Hour, 0, 0);
        var last = sessionEnd > sessionStart ? sessionEnd : sessionStart;
        for (var hour = first; hour <= last; hour = hour.AddHours(1))
        {
            buckets.Add(new TimeBucket { Start = hour, End = hour.AddHours(1), Label = hour.ToString("HH", CultureInfo.InvariantCulture) });
        }

        Fill(buckets, games);
        return buckets;
    }

    private static void Fill(List<TimeBucket> buckets, IEnumerable<GameRecord> games)
    {
        foreach (var game in games)
        {
            var bucket = buckets.FirstOrDefault(b => game.Start >= b.Start && game.Start < b.End);
            if (bucket is null)
            {
                continue;
            }

            bucket.Games++;
            bucket.Xp += game.Xp ?? 0;
            bucket.XpSeconds += game.XpSeconds;
        }
    }

    private static void AddDays(List<TimeBucket> buckets, DateTime first, int days, string labelFormat)
    {
        for (var day = 0; day < days; day++)
        {
            var start = first.AddDays(day);
            buckets.Add(new TimeBucket { Start = start, End = start.AddDays(1), Label = start.ToString(labelFormat, Localizer.Culture) });
        }
    }

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
            EventXp = games.Sum(g => g.EventXp ?? 0),
            HasEventXp = games.Any(g => g.EventXp is not null),
            XpSeconds = games.Sum(g => g.XpSeconds),
            PlaySeconds = games.Sum(g => g.WithMulliganSeconds),
            OnlineSeconds = OnlineSeconds(games, periodStart, now, currentSession),
        };
    }

    public static IReadOnlyList<DeckStats> Decks(IEnumerable<GameRecord> games) =>
        games.GroupBy(g => g.Deck.Length > 0 ? g.Deck : NoDeck)
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
                lost.AfkXp += Math.Max(0, XpRates.ExpectedXp(game.PlaySeconds, rate.Value, game.XpBonus) - (game.Xp ?? 0));
            }

            if (game.XpSeconds < XpRates.MinXpSeconds)
            {
                lost.ShortGames++;
                lost.ShortGamesXp += XpRates.FormulaXp(game.XpSeconds, rate.Value, game.XpBonus);
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
