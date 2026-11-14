using System.Globalization;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

public sealed class TavernPassTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "xp-tracker-pass-" + Guid.NewGuid().ToString("N"));

    private string File => Path.Combine(_folder, "hs_games.csv");

    [Theory]
    [InlineData(1, 10)]
    [InlineData(19, 10)]
    [InlineData(20, 15)]
    [InlineData(69, 15)]
    [InlineData(70, 20)]
    [InlineData(100, 20)]
    [InlineData(999, 20)]
    [InlineData(0, 10)] // clamped to level 1
    [InlineData(1500, 20)] // clamped to the highest level
    public void Boost_GrowsWithTheRewardsTrackLevel(int level, int percent)
    {
        Assert.Equal(percent, TavernPassBonus.Percent(true, level));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(50)]
    [InlineData(100)]
    public void Boost_IsZeroWithoutThePass(int level)
    {
        Assert.Equal(0, TavernPassBonus.Percent(false, level));
    }

    [Theory]
    [InlineData(true, 18, "10% XP boost, 2 levels to 15%")]
    [InlineData(true, 19, "10% XP boost, 1 level to 15%")]
    [InlineData(true, 20, "15% XP boost, 50 levels to 20%")]
    [InlineData(true, 70, "20% XP boost (maximum)")]
    [InlineData(false, 50, "No XP boost")]
    public void Describe_ShowsTheBoostAndTheNextStep(bool hasPass, int level, string expected)
    {
        Assert.Equal(expected, TavernPassBonus.Describe(hasPass, level));
    }

    [Fact]
    public void Boost_IsAppliedBeforeRounding()
    {
        // 378.43 s × 0.13054 = 49.40 XP. Boost first: 49.40 × 1.15 = 56.81 → 57.
        // Rounding first would give round(49) × 1.15 = 56.35 → 56.
        Assert.Equal(57, XpRates.ExpectedXp(378.43, XpRates.RankedWin, 15));
    }

    [Theory]
    [InlineData(425.27, 0, 56)]
    [InlineData(425.27, 10, 61)] // 55.51 × 1.10 = 61.07
    [InlineData(425.27, 15, 64)] // 55.51 × 1.15 = 63.84
    [InlineData(425.27, 20, 67)] // 55.51 × 1.20 = 66.62
    public void Boost_RaisesTheXpOfAWin(double xpSeconds, int percent, int expected)
    {
        Assert.Equal(expected, XpRates.ExpectedXp(xpSeconds, XpRates.RankedWin, percent));
    }

    [Fact]
    public void Boost_KeepsTheMinimumAndTheCap()
    {
        Assert.Equal(0, XpRates.ExpectedXp(59.9, XpRates.RankedWin, 20));

        // 1800 s × 0.13054 × 1.20 = 281.97
        Assert.Equal(282, XpRates.ExpectedXp(2400, XpRates.RankedWin, 20));
    }

    [Fact]
    public void Overlay_ShowsBoostedXp_ButEventXpWithoutBoost()
    {
        var clock = Started("10:00:00");

        var lines = XpDisplayText.For(clock, GameMode.Ranked, At("10:06:12"), eventActive: true, xpBonus: 20);

        // 372 s: win 48.56 × 1.2 = 58.27, loss 34.67 × 1.2 = 41.60; event XP 6.2 minutes → 6.
        Assert.Equal(new[] { "XP time:|6:12", "If win:|+58", "If loss:|+42", "Event XP:|+6" }, lines.Select(l => l.Label + "|" + l.Value));
    }

    [Fact]
    public void Overlay_SummaryShowsTheBoostedXp()
    {
        var clock = Started("10:00:00");
        clock.EndGame(At("10:06:12"), GameResult.Win);

        var lines = XpDisplayText.For(clock, GameMode.Ranked, At("10:07:00"), xpBonus: 15);

        // 48.56 × 1.15 = 55.85
        Assert.Equal("+56", lines.Single(l => l.Label == "Earned:").Value);
    }

    [Fact]
    public void Settings_RememberThePassAndTheLevel()
    {
        var settings = XpTrackerSettings.Parse(new[] { "TavernPass=true", "TavernPassLevel=42" });

        Assert.True(settings.TavernPass);
        Assert.Equal(42, settings.TavernPassLevel);
        Assert.Equal(15, settings.XpBonusPercent);
        Assert.Contains("TavernPass=true", settings.ToLines());
        Assert.Contains("TavernPassLevel=42", settings.ToLines());
    }

    [Fact]
    public void Settings_DefaultToNoPassAndIgnoreInvalidLevels()
    {
        var empty = XpTrackerSettings.Parse(Array.Empty<string>());
        Assert.False(empty.TavernPass);
        Assert.Equal(1, empty.TavernPassLevel);
        Assert.Equal(0, empty.XpBonusPercent);

        Assert.Equal(1, XpTrackerSettings.Parse(new[] { "TavernPassLevel=abc" }).TavernPassLevel);
        Assert.Equal(1, XpTrackerSettings.Parse(new[] { "TavernPassLevel=0" }).TavernPassLevel);
        Assert.Equal(0, XpTrackerSettings.Parse(new[] { "TavernPass=false", "TavernPassLevel=80" }).XpBonusPercent);
    }

    [Fact]
    public void Recalculation_KeepsTheBoostOfEachGame()
    {
        var boosted = Sample("10:18:45");
        boosted.XpBonus = 15;
        var plain = Sample("10:30:00");

        // 349.68 s × 0.13054 = 45.65 XP; with 15%: 52.49 → 52.
        Assert.True(boosted.RecalculateXp());
        Assert.Equal(52, boosted.Xp);
        Assert.False(plain.RecalculateXp());
        Assert.Equal(46, plain.Xp);
    }

    [Fact]
    public void History_StoresTheBoost()
    {
        var game = Sample("20:13:37");
        game.XpBonus = 20;
        new GameHistory(File).Add(game);

        Assert.Equal(20, Assert.Single(new GameHistory(File).Load()).XpBonus);
    }

    [Fact]
    public void History_ReadsFilesWithoutTheBoostColumnAsNoBoost_AndUpgradesThem()
    {
        Directory.CreateDirectory(_folder);
        System.IO.File.WriteAllLines(File, new[]
        {
            "Session,Start,End,Mode,Format,Result,XpSeconds,PlaySeconds,WithMulliganSeconds,AfkTurns,Xp,Deck,PlayerClass,OpponentClass,EventXp",
            "Hearthstone_2026_11_08_09_10_51,2026-11-08T10:18:45.000,2026-11-08T10:25:37.140,Ranked,Wild,Win,349.68,412.14,472.62,1,46,Rogue,ROGUE,WARRIOR,6",
        });

        var old = Assert.Single(new GameHistory(File).Load());
        Assert.Equal(0, old.XpBonus);
        Assert.Equal(6, old.EventXp);

        var game = Sample("20:13:37");
        game.XpBonus = 10;
        Assert.True(new GameHistory(File).Add(game));

        Assert.EndsWith(",EventXp,XpBonus", System.IO.File.ReadLines(File).First());
        var loaded = new GameHistory(File).Load();
        Assert.Equal(new[] { 0, 10 }, loaded.Select(g => g.XpBonus));
        Assert.Equal(46, loaded[0].Xp);
    }

    [Fact]
    public void LostXp_UsesTheBoostOfEachGame()
    {
        var shortGame = Sample("10:18:45");
        shortGame.Result = GameResult.Loss;
        shortGame.XpSeconds = 50;
        shortGame.PlaySeconds = 50;
        shortGame.AfkTurns = 0;
        shortGame.Xp = 0;
        shortGame.XpBonus = 20;

        var summary = Dashboard.Summary(new[] { shortGame }, DateTime.MinValue, At("12:00:00"), null);
        var lost = Dashboard.Lost(new[] { shortGame }, summary);

        // 50 s × 0.0932 = 4.66 XP; with 20%: 5.59 → 6 (without the boost it would be 5).
        Assert.Equal(6, lost.ShortGamesXp);
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
