using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class XpRatesTests
{
    // Measured games: XP time in seconds and the XP shown in the client.
    [Theory]
    [InlineData(488.30, XpRates.RankedWin, 64)]
    [InlineData(762.64, XpRates.RankedWin, 100)]
    [InlineData(412.58, XpRates.RankedLoss, 38)]
    [InlineData(933.37, XpRates.RankedLoss, 87)]
    [InlineData(1037.00, XpRates.CasualWin, 97)]
    [InlineData(413.63, XpRates.CasualLoss, 28)]
    public void ExpectedXp_MatchesMeasuredGames(double xpSeconds, double rate, int expectedXp)
    {
        Assert.Equal(expectedXp, XpRates.ExpectedXp(xpSeconds, rate));
    }

    [Fact]
    public void ExpectedXp_IsCappedAtThirtyMinutes()
    {
        Assert.Equal(XpRates.ExpectedXp(XpRates.MaxXpSeconds, XpRates.RankedWin), XpRates.ExpectedXp(2400, XpRates.RankedWin));
    }

    [Theory]
    [InlineData(GameMode.Ranked, GameResult.Win, XpRates.RankedWin)]
    [InlineData(GameMode.Ranked, GameResult.Loss, XpRates.RankedLoss)]
    [InlineData(GameMode.Casual, GameResult.Win, XpRates.CasualWin)]
    [InlineData(GameMode.TavernBrawl, GameResult.Loss, XpRates.CasualLoss)]
    public void RateFor_ReturnsMeasuredRates(GameMode mode, GameResult result, double expected)
    {
        Assert.Equal(expected, XpRates.RateFor(mode, result));
    }

    [Theory]
    [InlineData(GameMode.Other, GameResult.Win)]
    [InlineData(GameMode.Ranked, GameResult.Tie)]
    [InlineData(GameMode.Ranked, GameResult.Unknown)]
    public void RateFor_ReturnsNullWhenNotMeasured(GameMode mode, GameResult result)
    {
        Assert.Null(XpRates.RateFor(mode, result));
    }

    // Measured concedes around the minimum XP time.
    [Theory]
    [InlineData(55.01, 0)]
    [InlineData(59.23, 0)]
    [InlineData(59.89, 0)]
    [InlineData(60.78, 6)]
    [InlineData(61.13, 6)]
    public void ExpectedXp_IsZeroBelowTheMinimumXpTime(double xpSeconds, int expectedXp)
    {
        Assert.Equal(expectedXp, XpRates.ExpectedXp(xpSeconds, XpRates.RankedLoss));
    }

    [Fact]
    public void ExpectedXp_IsZeroForNegativeTime()
    {
        Assert.Equal(0, XpRates.ExpectedXp(-5, XpRates.RankedWin));
    }
}
