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

    [Fact]
    public void ExpectedXp_IsZeroForNegativeTime()
    {
        Assert.Equal(0, XpRates.ExpectedXp(-5, XpRates.RankedWin));
    }
}
