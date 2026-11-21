using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class WindowsStartupTests
{
    private const string Exe = @"C:\Users\Alice\Documents\HearthstoneXpTracker\HearthstoneXpTracker.exe";

    [Fact]
    public void RunCommand_QuotesThePathAndAddsTheAutostartArgument()
    {
        Assert.Equal("\"" + Exe + "\" --autostart", WindowsStartup.RunCommand(Exe));
    }

    [Fact]
    public void ExePathOf_ReadsTheCommandBack()
    {
        Assert.Equal(Exe, WindowsStartup.ExePathOf(WindowsStartup.RunCommand(Exe)));
    }

    [Fact]
    public void ExePathOf_ReadsCommandsWithoutQuotes()
    {
        Assert.Equal(@"C:\Tools\HearthstoneXpTracker.exe", WindowsStartup.ExePathOf(@"C:\Tools\HearthstoneXpTracker.exe --autostart"));
        Assert.Equal(@"C:\Tools\HearthstoneXpTracker.EXE", WindowsStartup.ExePathOf(@"C:\Tools\HearthstoneXpTracker.EXE"));
    }

    [Fact]
    public void ExePathOf_GivesNullForEmptyOrBrokenValues()
    {
        Assert.Null(WindowsStartup.ExePathOf(null));
        Assert.Null(WindowsStartup.ExePathOf("   "));
        Assert.Null(WindowsStartup.ExePathOf("\""));
        Assert.Null(WindowsStartup.ExePathOf("\"\" --autostart"));
    }

    [Fact]
    public void SamePath_IgnoresCaseSlashesAndSpaces()
    {
        Assert.True(WindowsStartup.SamePath(Exe, Exe.ToUpperInvariant()));
        Assert.True(WindowsStartup.SamePath(Exe, " " + Exe.Replace('\\', '/') + " "));
        Assert.False(WindowsStartup.SamePath(Exe, @"D:\HearthstoneXpTracker\HearthstoneXpTracker.exe"));
        Assert.False(WindowsStartup.SamePath(Exe, null));
    }

    [Fact]
    public void ApprovedByWindows_IsFalseOnlyWhenTheUserSwitchedTheEntryOff()
    {
        Assert.True(WindowsStartup.ApprovedByWindows(null));
        Assert.True(WindowsStartup.ApprovedByWindows(new byte[0]));
        Assert.True(WindowsStartup.ApprovedByWindows(new byte[] { 0x02, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 }));
        Assert.False(WindowsStartup.ApprovedByWindows(new byte[] { 0x03, 0, 0, 0, 0x1A, 0x2B, 0, 0, 0, 0, 0, 0 }));
    }

    [Fact]
    public void IsAutostart_LooksForTheArgument()
    {
        Assert.True(WindowsStartup.IsAutostart(new[] { "--autostart" }));
        Assert.True(WindowsStartup.IsAutostart(new[] { "--AUTOSTART" }));
        Assert.False(WindowsStartup.IsAutostart(new string[0]));
        Assert.False(WindowsStartup.IsAutostart(null));
        Assert.False(WindowsStartup.IsAutostart(new[] { "autostart" }));
    }

    [Fact]
    public void StartMenuAsked_IsStoredInTheSettings()
    {
        Assert.False(XpTrackerSettings.Parse(System.Array.Empty<string>()).StartMenuAsked);
        Assert.True(XpTrackerSettings.Parse(new[] { "StartMenuAsked=true" }).StartMenuAsked);
        Assert.Contains("StartMenuAsked=true", new XpTrackerSettings { StartMenuAsked = true }.ToLines());
    }
}
