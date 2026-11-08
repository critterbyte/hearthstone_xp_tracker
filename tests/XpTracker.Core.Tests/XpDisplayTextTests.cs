using System.Globalization;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class XpDisplayTextTests
{
    [Fact]
    public void LiveGame_ShowsXpTimeAndBothOutcomes()
    {
        var clock = Started("10:00:00");

        var lines = XpDisplayText.For(clock, GameMode.Ranked, At("10:06:12"));

        Assert.Equal(new[] { "XP time 6:12", "If win: +49 XP", "If loss: +35 XP" }, lines);
    }

    [Fact]
    public void LiveGame_AfterAnAfkTurn_ShowsPaused()
    {
        var clock = Started("10:00:00");
        clock.MarkCurrentTurnAfk();
        clock.StartTurn(Side.Opponent, At("10:00:55"));

        var lines = XpDisplayText.For(clock, GameMode.Ranked, At("10:01:00"));

        Assert.Equal("XP time 0:00 (paused)", lines[0]);
    }

    [Fact]
    public void FinishedGame_ShowsSummaryWithMulliganAndEarnedXp()
    {
        var clock = new XpClock();
        clock.StartMulligan(At("09:59:20"));
        clock.StartTurn(Side.Player, At("10:00:00"));
        clock.EndGame(At("10:17:17"), GameResult.Win);

        var lines = XpDisplayText.For(clock, GameMode.TavernBrawl, At("10:20:00"));

        Assert.Equal(new[] { "Last game: Win", "XP time 17:17", "Total time 17:57", "+97 XP" }, lines);
    }

    [Fact]
    public void LongGame_ShowsTheCap()
    {
        var clock = Started("10:00:00");

        var lines = XpDisplayText.For(clock, GameMode.Ranked, At("10:35:00"));

        Assert.Equal(new[] { "XP time 30:00 (max)", "If win: +235 XP", "If loss: +168 XP" }, lines);
    }

    [Fact]
    public void UnmeasuredMode_ShowsNothing()
    {
        Assert.Empty(XpDisplayText.For(Started("10:00:00"), GameMode.Other, At("10:05:00")));
    }

    [Fact]
    public void BeforeTheFirstTurn_ShowsNothing()
    {
        var clock = new XpClock();
        clock.StartMulligan(At("10:00:00"));

        Assert.Empty(XpDisplayText.For(clock, GameMode.Ranked, At("10:00:30")));
        Assert.Empty(XpDisplayText.For(null, GameMode.Ranked, At("10:00:30")));
    }

    private static XpClock Started(string time)
    {
        var clock = new XpClock();
        clock.StartTurn(Side.Player, At(time));
        return clock;
    }

    private static DateTime At(string time) =>
        DateTime.ParseExact("2026-11-08 " + time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
