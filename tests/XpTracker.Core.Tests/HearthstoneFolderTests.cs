using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class HearthstoneFolderTests
{
    private static readonly string Default = Path.Combine("C:", "Program Files (x86)");

    [Fact]
    public void RunningGame_IsUsedFirst()
    {
        var exe = Path.Combine("D:", "Games", "Hearthstone", "Hearthstone.exe");

        var logs = HearthstoneFolder.FindLogsFolder(exe, Path.Combine("E:", "Other"), Default, _ => true);

        Assert.Equal(Path.Combine("D:", "Games", "Hearthstone", "Logs"), logs);
    }

    [Fact]
    public void InstallLocation_IsUsedWhenTheGameIsNotRunning()
    {
        var install = Path.Combine("D:", "Games", "Hearthstone");

        var logs = HearthstoneFolder.FindLogsFolder(null, "\"" + install + "\\\"", Default, f => f == install);

        Assert.Equal(Path.Combine(install, "Logs"), logs);
    }

    [Fact]
    public void DefaultFolder_IsTheLastChoice()
    {
        var game = Path.Combine(Default, "Hearthstone");

        var logs = HearthstoneFolder.FindLogsFolder(null, Path.Combine("D:", "Missing"), Default, f => f == game);

        Assert.Equal(Path.Combine(game, "Logs"), logs);
    }

    [Fact]
    public void NoInstallation_GivesNull()
    {
        Assert.Null(HearthstoneFolder.FindLogsFolder(null, null, Default, _ => false));
        Assert.Null(HearthstoneFolder.FindLogsFolder(" ", "", null, _ => true));
    }
}
