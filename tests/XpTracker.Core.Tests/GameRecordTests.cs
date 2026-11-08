using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class GameRecordTests
{
    [Fact]
    public void RecalculateXp_corrects_xp_stored_with_an_older_rate()
    {
        // Stored with the old Ranked win rate 0.1308 (417.37 s gave 55); the game showed 54.
        var game = new GameRecord { Mode = GameMode.Ranked, Result = GameResult.Win, XpSeconds = 417.37, Xp = 55 };

        Assert.True(game.RecalculateXp());
        Assert.Equal(54, game.Xp);
    }

    [Fact]
    public void RecalculateXp_reports_no_change_when_the_xp_is_current()
    {
        var game = new GameRecord { Mode = GameMode.Ranked, Result = GameResult.Loss, XpSeconds = 933.37, Xp = 87 };

        Assert.False(game.RecalculateXp());
        Assert.Equal(87, game.Xp);
    }

    [Fact]
    public void RecalculateXp_applies_the_minimum_xp_time()
    {
        var game = new GameRecord { Mode = GameMode.Ranked, Result = GameResult.Loss, XpSeconds = 59.89, Xp = 6 };

        Assert.True(game.RecalculateXp());
        Assert.Equal(0, game.Xp);
    }

    [Theory]
    [InlineData(GameMode.Other, GameResult.Win)]
    [InlineData(GameMode.Ranked, GameResult.Unknown)]
    public void RecalculateXp_keeps_games_without_a_measured_rate(GameMode mode, GameResult result)
    {
        var game = new GameRecord { Mode = mode, Result = result, XpSeconds = 600, Xp = null };

        Assert.False(game.RecalculateXp());
        Assert.Null(game.Xp);
    }
}
