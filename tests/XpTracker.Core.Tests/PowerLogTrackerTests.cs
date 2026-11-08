using System.Globalization;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class PowerLogTrackerTests
{
    private static readonly DateTime Day = new(2026, 11, 8);

    [Fact]
    public void RankedWin_WithAfkFirstTurn_MatchesTheMeasuredGame()
    {
        var finished = new List<FinishedGame>();
        var tracker = new PowerLogTracker(Day, "Hearthstone_2026_11_08_09_10_51");
        tracker.GameFinished += finished.Add;

        Feed(tracker, GameStart("10:17:40", "GT_RANKED", localStarts: true));
        Feed(tracker,
            Power("10:17:44", "TAG_CHANGE Entity=GameEntity tag=STEP value=BEGIN_MULLIGAN"),
            Line("10:18:10", "GameState.DebugPrintEntityChoices()", "id=1 Player=Alice TaskList=4 ChoiceType=MULLIGAN CountMin=0 CountMax=3"),
            Line("10:18:12", "GameState.SendChoices()", "id=1 ChoiceType=MULLIGAN"),
            Power("10:18:45", "TAG_CHANGE Entity=GameEntity tag=STEP value=MAIN_READY"),
            // Alice idles: the server lowers her next limit to 10 before the turn changes.
            Power("10:19:40", "TAG_CHANGE Entity=Alice tag=TIMEOUT value=10"),
            Power("10:19:40", "TAG_CHANGE Entity=Alice tag=CURRENT_PLAYER value=0"),
            Power("10:19:40", "TAG_CHANGE Entity=Bob tag=CURRENT_PLAYER value=1"),
            Power("10:19:47", "TAG_CHANGE Entity=Alice tag=CURRENT_PLAYER value=1"),
            // Alice plays this turn: the limit goes back up.
            Power("10:20:49", "TAG_CHANGE Entity=Alice tag=TIMEOUT value=55"),
            Power("10:20:49", "TAG_CHANGE Entity=Bob tag=CURRENT_PLAYER value=1"),
            Power("10:21:03", "TAG_CHANGE Entity=Alice tag=CURRENT_PLAYER value=1"),
            Power("10:22:24", "TAG_CHANGE Entity=Bob tag=CURRENT_PLAYER value=1"),
            Power("10:22:30", "TAG_CHANGE Entity=Alice tag=CURRENT_PLAYER value=1"),
            Power("10:23:51", "TAG_CHANGE Entity=Bob tag=CURRENT_PLAYER value=1"),
            Power("10:24:10", "TAG_CHANGE Entity=Alice tag=CURRENT_PLAYER value=1"),
            Power("10:25:28", "TAG_CHANGE Entity=Bob tag=CURRENT_PLAYER value=1"),
            Power("10:25:37", "TAG_CHANGE Entity=Bob tag=PLAYSTATE value=CONCEDED"),
            Power("10:25:37", "TAG_CHANGE Entity=Alice tag=PLAYSTATE value=WON"),
            Power("10:25:37", "TAG_CHANGE Entity=GameEntity tag=STATE value=COMPLETE"));

        var game = Assert.Single(finished);
        var snapshot = game.Clock.GetSnapshot(At("10:30:00"));
        Assert.Equal(GameMode.Ranked, game.Mode);
        Assert.Equal("Wild", game.Format);
        Assert.Equal("Hearthstone_2026_11_08_09_10_51", game.Session);
        Assert.Equal(GameResult.Win, game.Clock.Result);
        Assert.Equal(At("10:18:45"), game.FirstTurn);
        Assert.Equal(At("10:25:37"), game.End);
        Assert.Equal(46, GameRecord.From(game).Xp);
        Assert.Equal(1, game.Clock.AfkTurns);
        Assert.Equal(350, snapshot.XpSeconds, 3);
        Assert.Equal(412, snapshot.PlaySeconds, 3);
        Assert.Equal(473, snapshot.WithMulliganSeconds, 3);
        Assert.Equal(46, snapshot.ExpectedXp(XpRates.RankedWin));
    }

    [Fact]
    public void OpponentStarting_UsesTheEntityIdsBeforeTheMulligan()
    {
        var tracker = new PowerLogTracker(Day);
        Feed(tracker, GameStart("12:00:00", "GT_CASUAL", localStarts: false));
        Feed(tracker,
            Line("12:00:10", "GameState.DebugPrintEntityChoices()", "id=1 Player=Alice TaskList=4 ChoiceType=MULLIGAN CountMin=0 CountMax=4"),
            Line("12:00:12", "GameState.SendChoices()", "id=1 ChoiceType=MULLIGAN"),
            Power("12:00:30", "TAG_CHANGE Entity=GameEntity tag=STEP value=MAIN_READY"),
            Power("12:00:45", "TAG_CHANGE Entity=Alice tag=CURRENT_PLAYER value=1"));

        var snapshot = tracker.Clock!.GetSnapshot(At("12:01:00"));
        Assert.Equal(GameMode.Casual, tracker.Mode);
        Assert.Equal(30, snapshot.XpSeconds, 3);
        Assert.False(snapshot.IsPaused);
    }

    [Fact]
    public void Rewind_DoesNotResetTheClock()
    {
        var tracker = StartedRankedGame("11:42:22");
        var clock = tracker.Clock;
        Feed(tracker,
            Power("11:43:10", "TAG_CHANGE Entity=Bob tag=CURRENT_PLAYER value=1"),
            Power("11:50:00", "BLOCK_START BlockType=GAME_RESET Entity=[entityName=Some Card id=60 zone=PLAY zonePos=1 cardId=X player=2] EffectCardId=System.Collections.Generic.List`1[System.String] EffectIndex=0 Target=0 SubOption=-1 TriggerKeyword=TAG_NOT_SET"),
            Power("11:50:00", "    tag=TIMEOUT value=75"),
            Power("11:50:00", "    tag=PLAYSTATE value=PLAYING"),
            Power("11:50:00", "    tag=CURRENT_PLAYER value=1"),
            Power("11:50:26", "TAG_CHANGE Entity=Alice tag=CURRENT_PLAYER value=1"),
            Power("11:50:30", "TAG_CHANGE Entity=Alice tag=PLAYSTATE value=WON"),
            Power("11:50:30", "TAG_CHANGE Entity=GameEntity tag=STATE value=COMPLETE"));

        Assert.Same(clock, tracker.Clock);
        var snapshot = tracker.Clock!.GetSnapshot(At("11:55:00"));
        Assert.Equal(488, snapshot.XpSeconds, 3);
        Assert.Equal(64, snapshot.ExpectedXp(XpRates.RankedWin));
    }

    [Fact]
    public void PowerTaskListAndCardLines_AreIgnored()
    {
        var tracker = StartedRankedGame("12:00:00");
        Feed(tracker,
            Line("12:01:00", "PowerTaskList.DebugPrintPower()", "TAG_CHANGE Entity=GameEntity tag=STATE value=COMPLETE"),
            Power("12:01:00", "TAG_CHANGE Entity=83 tag=ZONE value=PLAY"),
            Power("12:01:00", "TAG_CHANGE Entity=[entityName=Some Card id=83 zone=HAND zonePos=1 cardId=X player=1] tag=COST value=1"));

        Assert.False(tracker.Clock!.IsOver);
        Assert.Equal(90, tracker.Clock.GetSnapshot(At("12:01:30")).XpSeconds, 3);
    }

    [Fact]
    public void GameAcrossMidnight_CountsTheFullTime()
    {
        var tracker = StartedRankedGame("23:59:30");
        Feed(tracker,
            Power("00:00:30", "TAG_CHANGE Entity=Alice tag=PLAYSTATE value=LOST"),
            Power("00:00:30", "TAG_CHANGE Entity=GameEntity tag=STATE value=COMPLETE"));

        Assert.Equal(GameResult.Loss, tracker.Clock!.Result);
        Assert.Equal(60, tracker.Clock.GetSnapshot(At("00:05:00").AddDays(1)).XpSeconds, 3);
    }

    [Fact]
    public void NewGame_StartsANewClock()
    {
        var tracker = StartedRankedGame("12:00:00");
        var first = tracker.Clock;
        Feed(tracker, GameStart("12:10:00", "GT_TAVERNBRAWL", localStarts: true));

        Assert.NotSame(first, tracker.Clock);
        Assert.Equal(GameMode.TavernBrawl, tracker.Mode);
        Assert.False(tracker.Clock!.HasStarted);
    }

    internal static string[] GameStart(string time, string gameType, bool localStarts) => new[]
    {
        Power(time, "CREATE_GAME"),
        Power(time, "    Player EntityID=2 PlayerID=1 GameAccountId=[hi=1 lo=1]"),
        Power(time, "    Player EntityID=3 PlayerID=2 GameAccountId=[hi=2 lo=2]"),
        Line(time, "GameState.DebugPrintGame()", "GameType=" + gameType),
        Line(time, "GameState.DebugPrintGame()", "FormatType=FT_WILD"),
        Line(time, "GameState.DebugPrintGame()", "PlayerID=1, PlayerName=Alice"),
        Line(time, "GameState.DebugPrintGame()", "PlayerID=2, PlayerName=UNKNOWN HUMAN PLAYER"),
        Power(time, "    TAG_CHANGE Entity=2 tag=TIMEOUT value=45"),
        Power(time, "    TAG_CHANGE Entity=3 tag=TIMEOUT value=45"),
        Power(time, "    TAG_CHANGE Entity=" + (localStarts ? "2" : "3") + " tag=CURRENT_PLAYER value=1"),
    };

    internal static PowerLogTracker StartedRankedGame(string firstTurn)
    {
        var tracker = new PowerLogTracker(Day);
        Feed(tracker, GameStart(firstTurn, "GT_RANKED", localStarts: true));
        Feed(tracker,
            Line(firstTurn, "GameState.SendChoices()", "id=1 ChoiceType=MULLIGAN"),
            Power(firstTurn, "TAG_CHANGE Entity=GameEntity tag=STEP value=MAIN_READY"));
        return tracker;
    }

    internal static string Power(string time, string body) => Line(time, "GameState.DebugPrintPower()", body);

    internal static string Line(string time, string source, string body) => $"D {time}.0000000 {source} - {body}";

    private static void Feed(PowerLogTracker tracker, params string[] lines)
    {
        foreach (var line in lines)
        {
            tracker.ProcessLine(line);
        }
    }

    private static DateTime At(string time) =>
        DateTime.ParseExact("2026-11-08 " + time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
