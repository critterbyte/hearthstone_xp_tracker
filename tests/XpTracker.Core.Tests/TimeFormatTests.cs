using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class TimeFormatTests
{
    [Theory]
    [InlineData(0, "0:00")]
    [InlineData(9.99, "0:09")]
    [InlineData(372.4, "6:12")]
    [InlineData(1037, "17:17")]
    [InlineData(-3, "0:00")]
    public void MinutesSeconds_FormatsAsMinutesAndSeconds(double seconds, string expected)
    {
        Assert.Equal(expected, TimeFormat.MinutesSeconds(seconds));
    }

    [Theory]
    [InlineData(0, "0:00:00")]
    [InlineData(5077, "1:24:37")]
    [InlineData(36000, "10:00:00")]
    public void HoursMinutesSeconds_FormatsWithHours(double seconds, string expected)
    {
        Assert.Equal(expected, TimeFormat.HoursMinutesSeconds(seconds));
    }
}
