using System;
using System.Globalization;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class SpeedCurveTests
{
    [Fact]
    public void Last30Days_starts_30_days_before_now()
    {
        Assert.Equal(At("2026-10-09 12:00:00"), Dashboard.PeriodStart(DashboardPeriod.Last30Days, At("2026-11-08 12:00:00")));
    }

    [Fact]
    public void Today_has_one_running_point_per_game()
    {
        var games = new[] { FirstWin(), FirstLoss() };

        var curve = Dashboard.SpeedCurve(games, DashboardPeriod.Today);

        Assert.Equal(2, curve.Count);

        // 78 XP in 600 s of XP time.
        Assert.Equal(At("2026-11-08 18:20:00"), curve[0].Time);
        Assert.Equal(468, curve[0].XpPerHour, 2);

        // Both games: 134 XP in 1200 s.
        Assert.Equal(At("2026-11-08 18:40:00"), curve[1].Time);
        Assert.Equal(402, curve[1].XpPerHour, 2);
    }

    [Fact]
    public void Longer_periods_have_one_running_point_per_day()
    {
        var games = new[] { FirstWin(), FirstLoss(), NextDayWin() };

        var curve = Dashboard.SpeedCurve(games, DashboardPeriod.Week);

        Assert.Equal(2, curve.Count);
        Assert.Equal(At("2026-11-08 00:00:00"), curve[0].Time);
        Assert.Equal(402, curve[0].XpPerHour, 2);

        // 212 XP in 1800 s.
        Assert.Equal(At("2026-11-09 00:00:00"), curve[1].Time);
        Assert.Equal(424, curve[1].XpPerHour, 2);
    }

    [Fact]
    public void No_games_give_no_points()
    {
        Assert.Empty(Dashboard.SpeedCurve(Array.Empty<GameRecord>(), DashboardPeriod.Today));
        Assert.Empty(Dashboard.ProgressCurve(Array.Empty<GameRecord>()));
    }

    [Fact]
    public void Progress_curve_adds_up_play_time_and_xp_from_zero()
    {
        var games = new[] { FirstWin(), FirstLoss(), NextDayWin() };

        var curve = Dashboard.ProgressCurve(games);

        Assert.Equal(4, curve.Count);
        Assert.Equal(0, curve[0].PlaySeconds);
        Assert.Equal(0, curve[0].Xp);
        Assert.Equal(640, curve[1].PlaySeconds, 2);
        Assert.Equal(78, curve[1].Xp);
        Assert.Equal(1280, curve[2].PlaySeconds, 2);
        Assert.Equal(134, curve[2].Xp);
        Assert.Equal(1920, curve[3].PlaySeconds, 2);
        Assert.Equal(212, curve[3].Xp);
    }

    // Each game: 600 s of XP time, 640 s including the mulligan.
    private static GameRecord FirstWin() => Game("2026-11-08 18:10:00", "2026-11-08 18:20:00", GameResult.Win, 78);

    private static GameRecord FirstLoss() => Game("2026-11-08 18:30:00", "2026-11-08 18:40:00", GameResult.Loss, 56);

    private static GameRecord NextDayWin() => Game("2026-11-09 18:05:00", "2026-11-09 18:15:00", GameResult.Win, 78);

    private static GameRecord Game(string start, string end, GameResult result, int xp) => new()
    {
        Session = "Hearthstone_" + start.Substring(0, 10).Replace('-', '_') + "_18_00_00",
        Start = At(start),
        End = At(end),
        Mode = GameMode.Ranked,
        Result = result,
        XpSeconds = 600,
        PlaySeconds = 600,
        WithMulliganSeconds = 640,
        Xp = xp,
    };

    private static DateTime At(string text) => DateTime.ParseExact(text, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
