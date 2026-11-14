using System.Globalization;
using XpTracker.Core;
using static XpTracker.Core.Tests.PowerLogTrackerTests;

namespace XpTracker.Core.Tests;

// Measured Arena games of 14 November 2026: XP time, result, XP and event XP shown by the game.
public class ArenaTests
{
    [Theory]
    [InlineData(771.81, GameResult.Win, 72, 13)]
    [InlineData(1101.72, GameResult.Win, 103, 18)]
    [InlineData(1163.04, GameResult.Loss, 77, 19)]
    [InlineData(712.97, GameResult.Win, 66, 12)] // upper limit of the shared win rate: 0.093273 would give 67
    public void Arena_PaysTheCasualRates_AndEventXp(double xpSeconds, GameResult result, int xp, int eventXp)
    {
        var rate = XpRates.RateFor(GameMode.Arena, result);

        Assert.Equal(XpRates.RateFor(GameMode.Casual, result), rate);
        Assert.Equal(xp, XpRates.ExpectedXp(xpSeconds, rate!.Value));
        Assert.Equal(eventXp, XpRates.EventXpFor(GameMode.Arena, xpSeconds));
    }

    [Fact]
    public void ArenaGameType_IsRecognized()
    {
        var tracker = new PowerLogTracker(new DateTime(2026, 11, 14));
        foreach (var line in GameStart("20:13:01", "GT_ARENA", localStarts: true))
        {
            tracker.ProcessLine(line);
        }

        Assert.Equal(GameMode.Arena, tracker.Mode);
    }

    [Fact]
    public void Overlay_ShowsValuesInArena()
    {
        var clock = new XpClock();
        clock.StartTurn(Side.Player, DateTime.ParseExact("2026-11-14 20:13:01", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));

        var lines = XpDisplayText.For(clock, GameMode.Arena, DateTime.ParseExact("2026-11-14 20:19:13", "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));

        Assert.Equal(new[] { "XP time: 6:12", "If win: +35", "If loss: +25" }, lines.Select(l => l.ToString()));
    }

    [Fact]
    public void StoredArenaGames_AreRead()
    {
        Assert.True(Enum.TryParse<GameMode>("Arena", out var mode));
        Assert.Equal(GameMode.Arena, mode);
        Assert.Equal("Arena", GameText.Mode(GameMode.Arena));
    }
}
