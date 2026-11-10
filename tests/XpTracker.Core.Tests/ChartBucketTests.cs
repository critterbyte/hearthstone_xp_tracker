using System;
using System.Globalization;
using System.Linq;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class ChartBucketTests
{
    // Monday, 9 November 2026, evening.
    private static readonly DateTime Now = At("2026-11-09 21:30:00");

    [Fact]
    public void Today_has_the_24_hours_of_the_day()
    {
        var buckets = Dashboard.Buckets(Array.Empty<GameRecord>(), DashboardPeriod.Today, Now);

        Assert.Equal(24, buckets.Count);
        Assert.Equal("00", buckets[0].Label);
        Assert.Equal("23", buckets[23].Label);
        Assert.Equal(At("2026-11-09 20:00:00"), buckets[20].Start);
        Assert.Equal("Mon 2026-11-09, 20:00–21:00", buckets[20].Title);
        Assert.Equal("Mon 2026-11-09, 23:00–00:00", buckets[23].Title);
    }

    [Fact]
    public void Day_steps_are_titled_with_weekday_and_date()
    {
        var buckets = Dashboard.Buckets(Array.Empty<GameRecord>(), DashboardPeriod.Season, Now);

        Assert.Equal("Sun 2026-11-01", buckets[0].Title);
        Assert.Equal("Mon 2026-11-09", buckets[8].Title);
    }

    [Fact]
    public void Games_count_in_the_hour_they_started()
    {
        var games = new[]
        {
            Game("2026-11-09 20:40:25", 255.21, 24),
            Game("2026-11-09 20:49:22", 443.90, 58),
            Game("2026-11-09 21:08:15", 435.79, 41),
            Game("2026-11-08 23:59:00", 300, 39),
        };

        var buckets = Dashboard.Buckets(games, DashboardPeriod.Today, Now);

        Assert.Equal(2, buckets[20].Games);
        Assert.Equal(82, buckets[20].Xp);
        Assert.Equal(82 / ((255.21 + 443.90) / 3600), buckets[20].XpPerHour!.Value, 6);
        Assert.Equal(1, buckets[21].Games);

        // The game from yesterday is not part of today; hours without games have no speed.
        Assert.Equal(3, buckets.Sum(b => b.Games));
        Assert.Null(buckets[19].XpPerHour);
        Assert.Equal(0, buckets[19].Xp);
    }

    [Fact]
    public void Week_runs_from_Monday_to_Sunday()
    {
        var sunday = At("2026-11-08 12:00:00");

        var buckets = Dashboard.Buckets(Array.Empty<GameRecord>(), DashboardPeriod.Week, sunday);

        Assert.Equal(7, buckets.Count);
        Assert.Equal("Mon 11-02", buckets[0].Label);
        Assert.Equal("Sun 11-08", buckets[6].Label);
    }

    [Theory]
    [InlineData("2026-11-09 12:00:00", 30)]
    [InlineData("2026-12-01 12:00:00", 31)]
    [InlineData("2028-02-10 12:00:00", 29)]
    public void Season_has_every_day_of_the_month(string now, int days)
    {
        var buckets = Dashboard.Buckets(Array.Empty<GameRecord>(), DashboardPeriod.Season, At(now));

        Assert.Equal(days, buckets.Count);
        Assert.Equal("1", buckets[0].Label);
        Assert.Equal(days.ToString(CultureInfo.InvariantCulture), buckets[days - 1].Label);
    }

    [Fact]
    public void Last_30_days_end_today()
    {
        var buckets = Dashboard.Buckets(Array.Empty<GameRecord>(), DashboardPeriod.Last30Days, Now);

        Assert.Equal(30, buckets.Count);
        Assert.Equal("10-11", buckets[0].Label);
        Assert.Equal("11-09", buckets[29].Label);
        Assert.Equal(At("2026-10-11 00:00:00"), Dashboard.PeriodStart(DashboardPeriod.Last30Days, Now));
    }

    [Fact]
    public void All_time_runs_from_the_first_game_to_today()
    {
        var games = new[] { Game("2026-11-07 19:00:00", 600, 78), Game("2026-11-09 20:00:00", 600, 56) };

        var buckets = Dashboard.Buckets(games, DashboardPeriod.All, Now);

        Assert.Equal(3, buckets.Count);
        Assert.Equal("11-07", buckets[0].Label);
        Assert.Equal(0, buckets[1].Games);
        Assert.Equal(56, buckets[2].Xp);
    }

    [Fact]
    public void All_time_without_games_has_no_steps()
    {
        Assert.Empty(Dashboard.Buckets(Array.Empty<GameRecord>(), DashboardPeriod.All, Now));
    }

    private static GameRecord Game(string start, double xpSeconds, int xp) => new()
    {
        Start = At(start),
        End = At(start).AddSeconds(xpSeconds),
        Mode = GameMode.Ranked,
        Result = GameResult.Win,
        XpSeconds = xpSeconds,
        PlaySeconds = xpSeconds,
        WithMulliganSeconds = xpSeconds + 30,
        Xp = xp,
    };

    private static DateTime At(string text) => DateTime.ParseExact(text, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
