using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class OverlayPositionTests
{
    [Fact]
    public void Default_IsLeftAboveThePortrait()
    {
        Assert.Equal(OverlayPosition.Left, new XpTrackerSettings().OverlayPosition);
        Assert.Equal(OverlayPosition.Left, XpTrackerSettings.Parse(Array.Empty<string>()).OverlayPosition);
    }

    [Theory]
    [InlineData("OverlayPosition=Right", OverlayPosition.Right)]
    [InlineData("overlayposition = right", OverlayPosition.Right)]
    [InlineData("OverlayPosition=Left", OverlayPosition.Left)]
    [InlineData("OverlayPosition=Top", OverlayPosition.Left)]
    [InlineData("OverlayPosition=5", OverlayPosition.Left)]
    public void Setting_IsReadAndInvalidValuesKeepTheDefault(string line, OverlayPosition expected)
    {
        Assert.Equal(expected, XpTrackerSettings.Parse(new[] { line }).OverlayPosition);
    }

    [Fact]
    public void Setting_IsWritten()
    {
        Assert.Contains("OverlayPosition=Right", new XpTrackerSettings { OverlayPosition = OverlayPosition.Right }.ToLines());
    }
}
