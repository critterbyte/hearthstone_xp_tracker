using System.Globalization;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class XpClockTests
{
    private const string Afk = "afk";

    // Turn starts of measured games (whole seconds as shown in the turn list), with the XP shown in the client.

    [Fact]
    public void RankedLoss_WithTwoAfkTurns_ExcludesThemAndTheFollowingOpponentTurns()
    {
        var clock = Play(
            ("00:16:30", Side.Opponent, null),
            ("00:16:39", Side.Player, null),
            ("00:17:30", Side.Opponent, null),
            ("00:18:04", Side.Player, Afk),
            ("00:19:02", Side.Opponent, null),
            ("00:19:45", Side.Player, Afk),
            ("00:19:59", Side.Opponent, null),
            ("00:20:21", Side.Player, null),
            ("00:21:42", Side.Opponent, null),
            ("00:22:06", Side.Player, null),
            ("00:22:42", Side.Opponent, null),
            ("00:23:30", Side.Player, null),
            ("00:24:32", Side.Opponent, null),
            ("00:25:00", Side.Player, null));
        clock.EndGame(At("00:26:16"), GameResult.Loss);

        var snapshot = clock.GetSnapshot(At("00:30:00"));

        Assert.Equal(586, snapshot.PlaySeconds, 3);
        Assert.Equal(449, snapshot.XpSeconds, 3);
        Assert.Equal(2, clock.AfkTurns);
        Assert.Equal(42, snapshot.ExpectedXp(XpRates.RankedLoss));
    }

    [Fact]
    public void RankedWin_WithAfkFirstTurn_ExcludesItAndTheFollowingOpponentTurn()
    {
        var clock = Play(
            ("10:18:45", Side.Player, Afk),
            ("10:19:40", Side.Opponent, null),
            ("10:19:47", Side.Player, null),
            ("10:20:49", Side.Opponent, null),
            ("10:21:03", Side.Player, null),
            ("10:22:24", Side.Opponent, null),
            ("10:22:30", Side.Player, null),
            ("10:23:51", Side.Opponent, null),
            ("10:24:10", Side.Player, null),
            ("10:25:28", Side.Opponent, null));
        clock.EndGame(At("10:25:37"), GameResult.Win);

        var snapshot = clock.GetSnapshot(At("10:30:00"));

        Assert.Equal(412, snapshot.PlaySeconds, 3);
        Assert.Equal(350, snapshot.XpSeconds, 3);
        Assert.Equal(46, snapshot.ExpectedXp(XpRates.RankedWin));
        Assert.Equal(54, XpRates.ExpectedXp(snapshot.PlaySeconds, XpRates.RankedWin));
    }

    [Fact]
    public void RankedLoss_WithAfkTurnAfterOpponentStart_MatchesMeasuredXp()
    {
        var clock = Play(
            ("16:32:47", Side.Opponent, null),
            ("16:32:58", Side.Player, Afk),
            ("16:33:50", Side.Opponent, null),
            ("16:34:01", Side.Player, null),
            ("16:35:00", Side.Opponent, null));
        clock.EndGame(At("16:49:42"), GameResult.Loss);

        var snapshot = clock.GetSnapshot(At("17:00:00"));

        Assert.Equal(952, snapshot.XpSeconds, 3);
        Assert.Equal(89, snapshot.ExpectedXp(XpRates.RankedLoss));
    }

    [Fact]
    public void Mulligan_IsIncludedOnlyInTheTimeWithMulligan()
    {
        var clock = new XpClock();
        clock.StartMulligan(At("10:17:44"));
        clock.StartTurn(Side.Player, At("10:18:45"));
        clock.EndGame(At("10:25:37"), GameResult.Win);

        var snapshot = clock.GetSnapshot(At("10:30:00"));

        Assert.Equal(412, snapshot.PlaySeconds, 3);
        Assert.Equal(473, snapshot.WithMulliganSeconds, 3);
    }

    [Fact]
    public void LiveSnapshot_IsPausedDuringTheOpponentTurnAfterAnAfkTurn()
    {
        var clock = Play(
            ("10:18:45", Side.Player, Afk),
            ("10:19:40", Side.Opponent, null));

        var paused = clock.GetSnapshot(At("10:19:45"));
        Assert.True(paused.IsPaused);
        Assert.Equal(0, paused.XpSeconds, 3);

        clock.StartTurn(Side.Player, At("10:19:47"));
        var running = clock.GetSnapshot(At("10:20:00"));
        Assert.False(running.IsPaused);
        Assert.Equal(13, running.XpSeconds, 3);
        Assert.Equal(75, running.PlaySeconds, 3);
    }

    [Fact]
    public void LiveSnapshot_CountsTheCurrentTurnUntilItIsMarkedAfk()
    {
        var clock = Play(("10:18:45", Side.Player, null));

        Assert.Equal(50, clock.GetSnapshot(At("10:19:35")).XpSeconds, 3);

        clock.MarkCurrentTurnAfk();
        Assert.Equal(0, clock.GetSnapshot(At("10:19:40")).XpSeconds, 3);
    }

    [Fact]
    public void OpponentAfkTurn_DoesNotReduceXpTime()
    {
        var clock = Play(("12:00:00", Side.Player, null), ("12:01:00", Side.Opponent, null));
        clock.MarkCurrentTurnAfk();
        clock.StartTurn(Side.Player, At("12:02:20"));
        clock.EndGame(At("12:03:00"), GameResult.Win);

        Assert.Equal(180, clock.GetSnapshot(At("12:05:00")).XpSeconds, 3);
        Assert.Equal(0, clock.AfkTurns);
    }

    [Fact]
    public void LongWin_IsCappedAtThirtyMinutes()
    {
        var clock = Play(("12:00:00", Side.Player, null));
        clock.EndGame(At("12:40:00"), GameResult.Win);

        Assert.Equal(235, clock.GetSnapshot(At("13:00:00")).ExpectedXp(XpRates.RankedWin));
    }

    [Fact]
    public void EventsAfterTheGameEnded_AreIgnored()
    {
        var clock = Play(("12:00:00", Side.Player, null));
        clock.EndGame(At("12:05:00"), GameResult.Win);
        clock.StartTurn(Side.Opponent, At("12:06:00"));
        clock.MarkCurrentTurnAfk();

        var snapshot = clock.GetSnapshot(At("12:10:00"));
        Assert.Equal(300, snapshot.XpSeconds, 3);
        Assert.Equal(GameResult.Win, clock.Result);
    }

    private static XpClock Play(params (string Time, Side Side, string? Flag)[] turns)
    {
        var clock = new XpClock();
        for (var i = 0; i < turns.Length; i++)
        {
            clock.StartTurn(turns[i].Side, At(turns[i].Time));
            if (turns[i].Flag == Afk)
            {
                clock.MarkCurrentTurnAfk();
            }
        }

        return clock;
    }

    private static DateTime At(string time) =>
        DateTime.ParseExact("2026-11-08 " + time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
