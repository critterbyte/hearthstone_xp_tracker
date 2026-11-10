using System;
using System.Globalization;
using System.Linq;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class SessionTests
{
    private const string Evening = "Hearthstone_2026_11_10_19_56_25";
    private const string Earlier = "Hearthstone_2026_11_10_16_37_01";

    [Fact]
    public void Only_games_of_the_session_are_included()
    {
        var games = new[] { Game(Earlier, "2026-11-10 16:38:09"), Game(Evening, "2026-11-10 20:13:37"), Game(Evening, "2026-11-10 20:09:11") };

        var session = Dashboard.InSession(games, Evening);

        Assert.Equal(2, session.Count);
        Assert.Equal(At("2026-11-10 20:09:11"), session[0].Start);
        Assert.Empty(Dashboard.InSession(games, null));
    }

    [Fact]
    public void The_log_session_wins_even_without_games()
    {
        var games = new[] { Game(Earlier, "2026-11-10 16:38:09") };

        // Hearthstone was started again: the new session is shown, still empty.
        Assert.Equal(Evening, Dashboard.SelectSession(games, Evening));
    }

    [Fact]
    public void Without_a_log_session_the_newest_session_of_the_history_is_shown()
    {
        var games = new[] { Game(Evening, "2026-11-10 20:09:11"), Game(Earlier, "2026-11-10 16:38:09") };

        Assert.Equal(Evening, Dashboard.SelectSession(games, null));
        Assert.Null(Dashboard.SelectSession(Array.Empty<GameRecord>(), null));
    }

    [Fact]
    public void Session_axis_runs_hourly_from_start_to_end_across_midnight()
    {
        var games = new[] { Game(Evening, "2026-11-10 22:40:00", xp: 50), Game(Evening, "2026-11-11 00:20:00", xp: 30) };

        var buckets = Dashboard.SessionBuckets(games, At("2026-11-10 21:56:25"), At("2026-11-11 01:05:00"));

        Assert.Equal(new[] { "21", "22", "23", "00", "01" }, buckets.Select(b => b.Label));
        Assert.Equal(50, buckets[1].Xp);
        Assert.Equal(30, buckets[3].Xp);
        Assert.Equal("Wed 2026-11-11, 00:00–01:00", buckets[3].Title);
    }

    [Fact]
    public void A_new_session_without_games_has_one_empty_hour()
    {
        var start = At("2026-11-10 19:56:25");

        var buckets = Dashboard.SessionBuckets(Array.Empty<GameRecord>(), start, start);

        var hour = Assert.Single(buckets);
        Assert.Equal("19", hour.Label);
        Assert.Equal(0, hour.Games);
    }

    private static GameRecord Game(string session, string start, int xp = 40) => new()
    {
        Session = session,
        Start = At(start),
        End = At(start).AddMinutes(5),
        Mode = GameMode.Ranked,
        Result = GameResult.Win,
        XpSeconds = 300,
        PlaySeconds = 300,
        WithMulliganSeconds = 330,
        Xp = xp,
    };

    private static DateTime At(string text) => DateTime.ParseExact(text, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
