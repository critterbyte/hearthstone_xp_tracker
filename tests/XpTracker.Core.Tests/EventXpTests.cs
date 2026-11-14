using System.Globalization;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

public sealed class EventXpTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "xp-tracker-event-" + Guid.NewGuid().ToString("N"));

    private string File => Path.Combine(_folder, "hs_games.csv");

    // Measured games of the event in November 2026: XP time in seconds and the event XP shown by the game.
    [Theory]
    [InlineData(173.17, 3)]
    [InlineData(749.71, 12)] // 12:29.71, just below the half minute
    [InlineData(468.95, 8)]
    [InlineData(352.41, 6)]
    [InlineData(36.52, 1)] // no 60-second minimum for event XP
    [InlineData(33.21, 1)]
    [InlineData(27.14, 0)]
    [InlineData(784.16, 13)]
    [InlineData(359.56, 6)]
    public void EventXp_MatchesMeasuredGames(double xpSeconds, int expected)
    {
        Assert.Equal(expected, XpRates.EventXp(xpSeconds));
    }

    [Fact]
    public void EventXp_RoundsHalfMinutesUp()
    {
        Assert.Equal(13, XpRates.EventXp(750));
        Assert.Equal(0, XpRates.EventXp(-5));
    }

    [Fact]
    public void Overlay_ShowsAnEventRow_OnlyDuringAnEvent()
    {
        var clock = Started("10:00:00");

        var during = XpDisplayText.For(clock, GameMode.Ranked, At("10:07:49"), eventActive: true);
        var outside = XpDisplayText.For(clock, GameMode.Ranked, At("10:07:49"));

        var row = during[^1];
        Assert.Equal("Event XP:", row.Label);
        Assert.Equal("+8", row.Value);
        Assert.True(row.IsEvent);
        Assert.False(row.IsXp);
        Assert.DoesNotContain(outside, l => l.IsEvent);
    }

    [Fact]
    public void Overlay_SummaryEndsWithTheEventXp()
    {
        var clock = Started("10:00:00");
        clock.EndGame(At("10:13:04"), GameResult.Loss);

        var lines = XpDisplayText.For(clock, GameMode.Ranked, At("10:15:00"), eventActive: true);

        Assert.Equal(new[] { "Last game:", "XP time:", "Total time:", "Earned:", "Event XP:" }, lines.Select(l => l.Label));
        Assert.Equal("+13", lines[^1].Value);
    }

    [Fact]
    public void History_StoresEventXp()
    {
        var game = Sample("20:13:37");
        game.EventXp = 12;
        new GameHistory(File).Add(game);

        Assert.Equal(12, Assert.Single(new GameHistory(File).Load()).EventXp);
    }

    [Fact]
    public void History_ReadsOldFilesWithoutTheEventColumn_AndUpgradesThemOnTheNextGame()
    {
        Directory.CreateDirectory(_folder);
        System.IO.File.WriteAllLines(File, new[]
        {
            "Session,Start,End,Mode,Format,Result,XpSeconds,PlaySeconds,WithMulliganSeconds,AfkTurns,Xp,Deck,PlayerClass,OpponentClass",
            "Hearthstone_2026_11_08_09_10_51,2026-11-08T10:18:45.000,2026-11-08T10:25:37.140,Ranked,Wild,Win,349.68,412.14,472.62,1,46,Rogue,ROGUE,WARRIOR",
        });

        var old = Assert.Single(new GameHistory(File).Load());
        Assert.Equal(46, old.Xp);
        Assert.Null(old.EventXp);

        var game = Sample("20:13:37");
        game.EventXp = 12;
        Assert.True(new GameHistory(File).Add(game));

        Assert.EndsWith(",EventXp,XpBonus", System.IO.File.ReadLines(File).First());
        var loaded = new GameHistory(File).Load();
        Assert.Equal(2, loaded.Count);
        Assert.Null(loaded[0].EventXp);
        Assert.Equal(12, loaded[1].EventXp);
    }

    [Fact]
    public void Recalculation_KeepsGamesOutsideEventsWithoutEventXp()
    {
        var outside = Sample("10:18:45");
        var during = Sample("20:13:37");
        during.EventXp = 99;

        Assert.False(outside.RecalculateXp());
        Assert.Null(outside.EventXp);
        Assert.True(during.RecalculateXp());
        Assert.Equal(6, during.EventXp);
    }

    [Fact]
    public void Settings_RememberTheEventSwitch()
    {
        Assert.False(XpTrackerSettings.Parse(Array.Empty<string>()).EventActive);
        Assert.True(XpTrackerSettings.Parse(new[] { "EventActive=true" }).EventActive);
        Assert.Contains("EventActive=true", new XpTrackerSettings { EventActive = true }.ToLines());
    }

    [Fact]
    public void Settings_RememberTheDamageSwitch_OnByDefault()
    {
        Assert.True(XpTrackerSettings.Parse(Array.Empty<string>()).ShowDamage);
        Assert.False(XpTrackerSettings.Parse(new[] { "ShowDamage=false" }).ShowDamage);
        Assert.Contains("ShowDamage=false", new XpTrackerSettings { ShowDamage = false }.ToLines());
    }

    [Fact]
    public void Summary_AddsUpEventXp()
    {
        var a = Sample("10:18:45");
        a.EventXp = 6;
        var b = Sample("10:30:00");
        b.EventXp = 8;
        var c = Sample("09:00:00");

        var summary = Dashboard.Summary(new[] { c, a, b }, DateTime.MinValue, At("12:00:00"), null);

        Assert.Equal(14, summary.EventXp);
        Assert.True(summary.HasEventXp);
        Assert.False(Dashboard.Summary(new[] { c }, DateTime.MinValue, At("12:00:00"), null).HasEventXp);
    }

    public void Dispose()
    {
        if (Directory.Exists(_folder))
        {
            Directory.Delete(_folder, recursive: true);
        }
    }

    private static GameRecord Sample(string start) => new()
    {
        Session = "Hearthstone_2026_11_08_09_10_51",
        Start = At(start),
        End = At(start).AddSeconds(412.14),
        Mode = GameMode.Ranked,
        Format = "Wild",
        Result = GameResult.Win,
        XpSeconds = 349.68,
        PlaySeconds = 412.14,
        WithMulliganSeconds = 472.62,
        AfkTurns = 1,
        Xp = 46,
        Deck = "Rogue",
        PlayerClass = "ROGUE",
        OpponentClass = "WARRIOR",
    };

    private static XpClock Started(string time)
    {
        var clock = new XpClock();
        clock.StartTurn(Side.Player, At(time));
        return clock;
    }

    private static DateTime At(string time) =>
        DateTime.ParseExact("2026-11-08 " + time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
