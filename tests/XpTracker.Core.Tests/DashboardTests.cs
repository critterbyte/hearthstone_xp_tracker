using System.Globalization;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class DashboardTests
{
    private const string Session = "Hearthstone_2026_11_08_09_10_51";

    [Fact]
    public void PeriodStart_UsesCalendarDayMondayWeekAndMonthSeason()
    {
        var sunday = At("2026-11-08 16:00:00");

        Assert.Equal(At("2026-11-08 00:00:00"), Dashboard.PeriodStart(DashboardPeriod.Today, sunday));
        Assert.Equal(At("2026-11-02 00:00:00"), Dashboard.PeriodStart(DashboardPeriod.Week, sunday));
        Assert.Equal(At("2026-11-01 00:00:00"), Dashboard.PeriodStart(DashboardPeriod.Season, sunday));
        Assert.Equal(DateTime.MinValue, Dashboard.PeriodStart(DashboardPeriod.All, sunday));
    }

    [Fact]
    public void InPeriod_KeepsOnlyGamesStartedInThePeriod()
    {
        var games = new[] { Game("2026-11-07 23:52:30", 402, GameResult.Loss, 27), Game("2026-11-08 00:03:03", 370, GameResult.Loss, 34) };

        var today = Dashboard.InPeriod(games, DashboardPeriod.Today, At("2026-11-08 12:00:00"));

        Assert.Equal(At("2026-11-08 00:03:03"), Assert.Single(today).Start);
    }

    [Fact]
    public void Summary_CountsXpTimesAndOnlineTimeOfTheSession()
    {
        var games = new[]
        {
            Game("2026-11-08 09:52:23", 672.79, GameResult.Win, 88),
            Game("2026-11-08 10:18:45", 412.14, GameResult.Win, 46, xpSeconds: 349.68, afkTurns: 1),
        };

        var summary = Dashboard.Summary(games, At("2026-11-08 00:00:00"), At("2026-11-08 12:00:00"), currentSession: null);

        Assert.Equal(2, summary.Games);
        Assert.Equal(2, summary.Wins);
        Assert.Equal(134, summary.Xp);
        Assert.Equal(672.79 + 349.68, summary.XpSeconds, 2);
        // Session from 09:10:51 (log folder) to the end of the last game (10:25:37.14).
        Assert.Equal((At("2026-11-08 10:25:37") - At("2026-11-08 09:10:51")).TotalSeconds + 0.14, summary.OnlineSeconds, 2);
    }

    [Fact]
    public void Summary_RunningSession_CountsOnlineTimeUntilNow()
    {
        var games = new[] { Game("2026-11-08 09:52:23", 672.79, GameResult.Win, 88) };

        var summary = Dashboard.Summary(games, At("2026-11-08 00:00:00"), At("2026-11-08 11:10:51"), Session);

        Assert.Equal(2 * 3600, summary.OnlineSeconds, 2);
    }

    [Fact]
    public void Summary_SessionStartedBeforeThePeriod_IsClipped()
    {
        var game = Game("2026-11-08 00:03:03", 369.64, GameResult.Loss, 34);
        game.Session = "Hearthstone_2026_11_07_23_51_24";

        var summary = Dashboard.Summary(new[] { game }, At("2026-11-08 00:00:00"), At("2026-11-08 12:00:00"), currentSession: null);

        Assert.Equal((game.End - At("2026-11-08 00:00:00")).TotalSeconds, summary.OnlineSeconds, 2);
    }

    [Fact]
    public void Decks_AreGroupedAndSortedByXpPerHour()
    {
        var fast = Game("2026-11-08 10:00:00", 360, GameResult.Win, 47);
        fast.Deck = "Pirate Rogue";
        var slow = Game("2026-11-08 11:00:00", 360, GameResult.Loss, 34);
        slow.Deck = "Thief Priest";
        var unknown = Game("2026-11-08 12:00:00", 360, GameResult.Loss, 34);

        var decks = Dashboard.Decks(new[] { slow, fast, unknown });

        Assert.Equal("Pirate Rogue", decks[0].Deck);
        Assert.Equal(1, decks[0].WinRate);
        Assert.Equal(470, decks[0].XpPerHour, 0);
        Assert.Contains(decks, d => d.Deck == Dashboard.NoDeck);
    }

    [Fact]
    public void Lost_CountsAfkXpShortGamesAndTimeOutsideGames()
    {
        // Measured: AFK first turn cost 8 XP (54 instead of 46); an early opponent concede at 34.34 s gave 0 instead of 4.
        var afk = Game("2026-11-08 10:18:45", 412.14, GameResult.Win, 46, xpSeconds: 349.68, afkTurns: 1);
        var shortGame = Game("2026-11-08 15:33:42", 34.34, GameResult.Win, 0);
        var games = new[] { afk, shortGame };
        var summary = new SummaryStats { Xp = 46, XpSeconds = 384.02, PlaySeconds = 446.48, OnlineSeconds = 446.48 + 3600 };

        var lost = Dashboard.Lost(games, summary);

        Assert.Equal(1, lost.AfkTurns);
        Assert.Equal(8, lost.AfkXp);
        Assert.Equal(1, lost.ShortGames);
        Assert.Equal(4, lost.ShortGamesXp);
        Assert.Equal(3600, lost.OutsideGamesSeconds, 2);
        Assert.Equal((int)Math.Round(summary.XpPerHour), lost.OutsideGamesXp);
    }

    [Fact]
    public void Days_SumXpPerCalendarDay()
    {
        var games = new[]
        {
            Game("2026-11-07 23:52:30", 402, GameResult.Loss, 27),
            Game("2026-11-08 00:03:03", 370, GameResult.Loss, 34),
            Game("2026-11-08 09:52:23", 673, GameResult.Win, 88),
        };

        var days = Dashboard.Days(games);

        Assert.Equal(2, days.Count);
        Assert.Equal(27, days[0].Xp);
        Assert.Equal(122, days[1].Xp);
    }

    private static GameRecord Game(string start, double playSeconds, GameResult result, int xp, double? xpSeconds = null, int afkTurns = 0)
    {
        var begin = At(start);
        return new GameRecord
        {
            Session = Session,
            Start = begin,
            End = begin.AddSeconds(playSeconds),
            Mode = GameMode.Ranked,
            Format = "Wild",
            Result = result,
            XpSeconds = xpSeconds ?? playSeconds,
            PlaySeconds = playSeconds,
            WithMulliganSeconds = playSeconds,
            AfkTurns = afkTurns,
            Xp = xp,
        };
    }

    private static DateTime At(string value) => DateTime.ParseExact(value, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
