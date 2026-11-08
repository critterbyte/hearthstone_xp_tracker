using System.Globalization;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class GameObserverTests
{
    private const Side P = Side.Player;
    private const Side O = Side.Opponent;

    [Fact]
    public void RankedWin_WithAfkFirstTurn_DetectsAfkFromTheTurnLimit()
    {
        var observer = new GameObserver();
        Feed(observer, GameMode.Ranked,
            ("10:17:40", GamePhase.None, null, 0),
            ("10:17:44", GamePhase.Mulligan, P, 45),
            ("10:18:45", GamePhase.Turns, P, 45),
            ("10:19:40", GamePhase.Turns, O, 10),
            ("10:19:47", GamePhase.Turns, P, 10),
            ("10:20:49", GamePhase.Turns, O, 55),
            ("10:21:03", GamePhase.Turns, P, 55),
            ("10:22:24", GamePhase.Turns, O, 75),
            ("10:22:30", GamePhase.Turns, P, 75),
            ("10:23:51", GamePhase.Turns, O, 75),
            ("10:24:10", GamePhase.Turns, P, 75),
            ("10:25:28", GamePhase.Turns, O, 75));
        observer.Update(Sample(GamePhase.Over, O, 75, GameResult.Win, GameMode.Ranked), At("10:25:37"));

        var clock = observer.Clock!;
        var snapshot = clock.GetSnapshot(At("10:30:00"));
        Assert.True(clock.IsOver);
        Assert.Equal(GameResult.Win, clock.Result);
        Assert.Equal(1, clock.AfkTurns);
        Assert.Equal(350, snapshot.XpSeconds, 3);
        Assert.Equal(412, snapshot.PlaySeconds, 3);
        Assert.Equal(473, snapshot.WithMulliganSeconds, 3);
        Assert.Equal(46, snapshot.ExpectedXp(XpRates.RateFor(observer.Mode, clock.Result)!.Value));
    }

    [Fact]
    public void RankedLoss_WithFollowUpAfkTurn_DetectsBothAfkTurns()
    {
        var observer = new GameObserver();
        Feed(observer, GameMode.Ranked,
            ("00:16:00", GamePhase.Mulligan, O, 45),
            ("00:16:30", GamePhase.Turns, O, 45),
            ("00:16:39", GamePhase.Turns, P, 45),
            ("00:17:30", GamePhase.Turns, O, 55),
            ("00:18:04", GamePhase.Turns, P, 55),
            ("00:19:02", GamePhase.Turns, O, 10),
            ("00:19:45", GamePhase.Turns, P, 10),
            ("00:19:59", GamePhase.Turns, O, 10),
            ("00:20:21", GamePhase.Turns, P, 10),
            ("00:21:42", GamePhase.Turns, O, 75),
            ("00:22:06", GamePhase.Turns, P, 75),
            ("00:22:42", GamePhase.Turns, O, 75),
            ("00:23:30", GamePhase.Turns, P, 75),
            ("00:24:32", GamePhase.Turns, O, 75),
            ("00:25:00", GamePhase.Turns, P, 75));
        observer.Update(Sample(GamePhase.Over, P, 75, GameResult.Loss, GameMode.Ranked), At("00:26:16"));

        var clock = observer.Clock!;
        var snapshot = clock.GetSnapshot(At("00:30:00"));
        Assert.Equal(2, clock.AfkTurns);
        Assert.Equal(449, snapshot.XpSeconds, 3);
        Assert.Equal(42, snapshot.ExpectedXp(XpRates.RankedLoss));
    }

    [Fact]
    public void CurrentPlayerDuringMulligan_DoesNotStartTheClock()
    {
        var observer = new GameObserver();
        Feed(observer, GameMode.Casual, ("12:00:00", GamePhase.Mulligan, O, 45));

        Assert.False(observer.Clock!.HasStarted);
    }

    [Fact]
    public void RepeatedSamplesOfTheSameTurn_DoNotSplitTheTurn()
    {
        var observer = new GameObserver();
        Feed(observer, GameMode.Ranked,
            ("12:00:00", GamePhase.Turns, P, 45),
            ("12:00:10", GamePhase.Turns, P, 45),
            ("12:00:20", GamePhase.Turns, P, 10));

        var snapshot = observer.Clock!.GetSnapshot(At("12:00:30"));
        Assert.Equal(30, snapshot.XpSeconds, 3);
        Assert.Equal(0, observer.Clock.AfkTurns);
    }

    [Fact]
    public void NextMulligan_StartsANewGame()
    {
        var observer = new GameObserver();
        Feed(observer, GameMode.Ranked, ("12:00:00", GamePhase.Turns, P, 45));
        observer.Update(Sample(GamePhase.Over, P, 45, GameResult.Win, GameMode.Ranked), At("12:05:00"));
        var first = observer.Clock;

        Feed(observer, GameMode.TavernBrawl, ("12:06:00", GamePhase.Mulligan, P, 45));

        Assert.NotSame(first, observer.Clock);
        Assert.Equal(GameMode.TavernBrawl, observer.Mode);
        Assert.False(observer.Clock!.IsOver);
    }

    [Fact]
    public void ReturningToTheMenuBeforeTheEnd_ClosesTheGameWithTheLastKnownResult()
    {
        var observer = new GameObserver();
        Feed(observer, GameMode.Ranked, ("12:00:00", GamePhase.Turns, P, 45));
        observer.Update(Sample(GamePhase.Turns, P, 45, GameResult.Win, GameMode.Ranked), At("12:04:59"));
        observer.Update(GameSample.NoGame, At("12:05:00"));

        Assert.True(observer.Clock!.IsOver);
        Assert.Equal(GameResult.Win, observer.Clock.Result);
        Assert.Equal(300, observer.Clock.GetSnapshot(At("12:10:00")).XpSeconds, 3);
    }

    private static void Feed(GameObserver observer, GameMode mode, params (string Time, GamePhase Phase, Side? Side, int Limit)[] samples)
    {
        foreach (var s in samples)
        {
            observer.Update(Sample(s.Phase, s.Side, s.Limit, GameResult.Unknown, mode), At(s.Time));
        }
    }

    private static GameSample Sample(GamePhase phase, Side? side, int limit, GameResult result, GameMode mode) =>
        new(phase, side, limit, result, mode);

    private static DateTime At(string time) =>
        DateTime.ParseExact("2026-11-08 " + time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
