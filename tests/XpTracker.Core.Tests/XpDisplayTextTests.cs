using System.Globalization;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class XpDisplayTextTests
{
    [Fact]
    public void LiveGame_ShowsXpTimeAndBothOutcomes()
    {
        var observer = Start(GameMode.Ranked, "10:00:00");

        var lines = XpDisplayText.For(observer, At("10:06:12"));

        Assert.Equal(new[] { "XP time 6:12", "Win 49 XP", "Loss 35 XP" }, lines);
    }

    [Fact]
    public void LiveGame_AfterAnAfkTurn_ShowsPaused()
    {
        var observer = Start(GameMode.Ranked, "10:00:00");
        observer.Update(new GameSample(GamePhase.Turns, Side.Opponent, 10, GameResult.Unknown, GameMode.Ranked), At("10:00:55"));

        var lines = XpDisplayText.For(observer, At("10:01:00"));

        Assert.Equal("XP time 0:00 (paused)", lines[0]);
    }

    [Fact]
    public void FinishedGame_ShowsSummaryWithMulliganAndEarnedXp()
    {
        var observer = new GameObserver();
        observer.Update(new GameSample(GamePhase.Mulligan, Side.Player, 45, GameResult.Unknown, GameMode.TavernBrawl), At("09:59:20"));
        observer.Update(new GameSample(GamePhase.Turns, Side.Player, 45, GameResult.Unknown, GameMode.TavernBrawl), At("10:00:00"));
        observer.Update(new GameSample(GamePhase.Over, Side.Player, 45, GameResult.Win, GameMode.TavernBrawl), At("10:17:17"));

        var lines = XpDisplayText.For(observer, At("10:20:00"));

        Assert.Equal(new[] { "XP time 17:17", "Game 17:57 incl. mulligan", "Earned 97 XP" }, lines);
    }

    [Fact]
    public void LongGame_ShowsTheCap()
    {
        var observer = Start(GameMode.Ranked, "10:00:00");

        var lines = XpDisplayText.For(observer, At("10:35:00"));

        Assert.Equal(new[] { "XP time 30:00 (max)", "Win 235 XP", "Loss 168 XP" }, lines);
    }

    [Fact]
    public void UnmeasuredMode_ShowsNothing()
    {
        var observer = Start(GameMode.Other, "10:00:00");

        Assert.Empty(XpDisplayText.For(observer, At("10:05:00")));
    }

    [Fact]
    public void BeforeTheFirstTurn_ShowsNothing()
    {
        var observer = new GameObserver();
        observer.Update(new GameSample(GamePhase.Mulligan, Side.Player, 45, GameResult.Unknown, GameMode.Ranked), At("10:00:00"));

        Assert.Empty(XpDisplayText.For(observer, At("10:00:30")));
    }

    private static GameObserver Start(GameMode mode, string time)
    {
        var observer = new GameObserver();
        observer.Update(new GameSample(GamePhase.Turns, Side.Player, 45, GameResult.Unknown, mode), At(time));
        return observer;
    }

    private static DateTime At(string time) =>
        DateTime.ParseExact("2026-11-08 " + time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
