using System.Globalization;
using XpTracker.Core;
using static XpTracker.Core.Tests.PowerLogTrackerTests;

namespace XpTracker.Core.Tests;

public class PveBrawlTests
{
    private static readonly DateTime Day = new(2026, 11, 10);

    // Start of a game as in the log of 10 November 2026: the Innkeeper has no account.
    private static string[] StartAgainstComputer(string time, string gameType) => new[]
    {
        Power(time, "CREATE_GAME"),
        Power(time, "    Player EntityID=2 PlayerID=1 GameAccountId=[hi=1 lo=1]"),
        Power(time, "    Player EntityID=3 PlayerID=2 GameAccountId=[hi=0 lo=0]"),
        Line(time, "GameState.DebugPrintGame()", "GameType=" + gameType),
        Line(time, "GameState.DebugPrintGame()", "FormatType=FT_WILD"),
        Line(time, "GameState.DebugPrintGame()", "ScenarioID=5235"),
        Line(time, "GameState.DebugPrintGame()", "PlayerID=1, PlayerName=Alice"),
        Line(time, "GameState.DebugPrintGame()", "PlayerID=2, PlayerName=The Innkeeper"),
    };

    [Fact]
    public void TavernBrawl_AgainstTheComputer_IsRecognized()
    {
        var tracker = new PowerLogTracker(Day);

        Feed(tracker, StartAgainstComputer("22:49:07", "GT_TAVERNBRAWL"));

        Assert.Equal(GameMode.TavernBrawlPvE, tracker.Mode);
    }

    [Fact]
    public void TavernBrawl_AgainstAPlayer_StaysPvp()
    {
        var tracker = new PowerLogTracker(Day);

        Feed(tracker, GameStart("12:10:00", "GT_TAVERNBRAWL", localStarts: true));

        Assert.Equal(GameMode.TavernBrawl, tracker.Mode);
    }

    [Fact]
    public void Practice_AgainstTheComputer_StaysUnmeasured()
    {
        var tracker = new PowerLogTracker(Day);

        Feed(tracker, StartAgainstComputer("12:10:00", "GT_VS_AI"));

        Assert.Equal(GameMode.Other, tracker.Mode);
    }

    [Theory]
    [InlineData(98.93)] // measured: 1:39 loss gave 0 XP and 0 event XP
    [InlineData(269.07)] // measured: 4:29 loss gave 0 XP and 0 event XP
    public void PveBrawl_GivesNoXpAndNoEventXp(double xpSeconds)
    {
        Assert.Equal(0, XpRates.ExpectedXp(xpSeconds, XpRates.RateFor(GameMode.TavernBrawlPvE, GameResult.Loss)!.Value));
        Assert.Equal(0, XpRates.ExpectedXp(xpSeconds, XpRates.RateFor(GameMode.TavernBrawlPvE, GameResult.Win)!.Value));
        Assert.Equal(0, XpRates.EventXpFor(GameMode.TavernBrawlPvE, xpSeconds));
    }

    [Fact]
    public void Overlay_ShowsZeroForPveBrawl()
    {
        var clock = new XpClock();
        clock.StartTurn(Side.Player, At("10:00:00"));

        var lines = XpDisplayText.For(clock, GameMode.TavernBrawlPvE, At("10:04:29"), eventActive: true);

        Assert.Equal(new[] { "+0", "+0", "+0" }, lines.Skip(1).Select(l => l.Value));
    }

    [Fact]
    public void EventXp_IsUnknownForUnmeasuredModes()
    {
        Assert.Null(XpRates.EventXpFor(GameMode.Other, 600));
        Assert.Equal(10, XpRates.EventXpFor(GameMode.TavernBrawl, 600));
    }

    [Fact]
    public void History_KeepsThePveMode()
    {
        var folder = Path.Combine(Path.GetTempPath(), "xp-tracker-pve-" + Guid.NewGuid().ToString("N"));
        var file = Path.Combine(folder, "hs_games.csv");
        try
        {
            new GameHistory(file).Add(new GameRecord
            {
                Session = "Hearthstone_2026_11_10_19_59_21",
                Start = At("22:49:27"),
                End = At("22:51:06"),
                Mode = GameMode.TavernBrawlPvE,
                Format = "Wild",
                Result = GameResult.Loss,
                XpSeconds = 98.93,
                Xp = 0,
                EventXp = 0,
            });

            var loaded = Assert.Single(new GameHistory(file).Load());
            Assert.Equal(GameMode.TavernBrawlPvE, loaded.Mode);
            Assert.False(loaded.RecalculateXp());
        }
        finally
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, recursive: true);
            }
        }
    }

    private static void Feed(PowerLogTracker tracker, params string[] lines)
    {
        foreach (var line in lines)
        {
            tracker.ProcessLine(line);
        }
    }

    private static DateTime At(string time) =>
        DateTime.ParseExact("2026-11-10 " + time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
