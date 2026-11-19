using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class LogConfigTests
{
    private const string PowerOn = "[Power]\r\nLogLevel=1\r\nFilePrinting=True\r\nConsolePrinting=False\r\nScreenPrinting=False\r\nVerbose=True\r\n";

    [Fact]
    public void MissingFile_GetsThePowerSection()
    {
        Assert.Equal(PowerOn, LogConfig.Update(null));
        Assert.Equal(PowerOn, LogConfig.Update(string.Empty));
    }

    [Fact]
    public void SectionThatIsAlreadyOn_LeavesTheFileUnchanged()
    {
        Assert.Null(LogConfig.Update(PowerOn));
        Assert.Null(LogConfig.Update("[power]\r\nloglevel = 1\r\nfileprinting = true\r\nverbose = TRUE\r\n"));
    }

    [Fact]
    public void OtherSections_AreKeptAsTheyAre()
    {
        var hdt = "[Achievements]\r\nLogLevel=1\r\nFilePrinting=True\r\n[Arena]\r\nLogLevel=1\r\nFilePrinting=True\r\n";

        var updated = LogConfig.Update(hdt);

        Assert.Equal(hdt + PowerOn, updated);
    }

    [Fact]
    public void WrongValues_AreCorrectedInPlace()
    {
        var content = "[Zone]\r\nLogLevel=1\r\n[Power]\r\nLogLevel=1\r\nFilePrinting=False\r\nConsolePrinting=True\r\nVerbose=False\r\n[Arena]\r\nLogLevel=1\r\n";

        var updated = LogConfig.Update(content);

        Assert.Equal("[Zone]\r\nLogLevel=1\r\n[Power]\r\nLogLevel=1\r\nFilePrinting=True\r\nConsolePrinting=True\r\nVerbose=True\r\n[Arena]\r\nLogLevel=1\r\n", updated);
    }

    [Fact]
    public void MissingValues_AreAddedAtTheEndOfTheSection()
    {
        var content = "[Power]\r\nLogLevel=1\r\n[Arena]\r\nLogLevel=1\r\n";

        var updated = LogConfig.Update(content);

        Assert.Equal("[Power]\r\nLogLevel=1\r\nFilePrinting=True\r\nVerbose=True\r\n[Arena]\r\nLogLevel=1\r\n", updated);
    }

    [Fact]
    public void UnixLineEndings_AreKept()
    {
        var updated = LogConfig.Update("[Arena]\nLogLevel=1\n");

        Assert.Equal("[Arena]\nLogLevel=1\n" + PowerOn.Replace("\r\n", "\n"), updated);
    }

    [Fact]
    public void FileWithoutFinalNewline_GetsTheSectionOnANewLine()
    {
        Assert.Equal("[Arena]\r\nLogLevel=1\r\n" + PowerOn, LogConfig.Update("[Arena]\r\nLogLevel=1"));
    }

    [Fact]
    public void EnsureFile_WritesOnceAndKeepsABackup()
    {
        var folder = Path.Combine(Path.GetTempPath(), "XpTrackerTests", Guid.NewGuid().ToString("N"));
        var path = Path.Combine(folder, "log.config");
        try
        {
            Directory.CreateDirectory(folder);
            File.WriteAllText(path, "[Arena]\r\nLogLevel=1\r\n");

            Assert.Equal(LogConfigResult.Updated, LogConfig.EnsureFile(path, out var error));
            Assert.Null(error);
            Assert.Equal("[Arena]\r\nLogLevel=1\r\n", File.ReadAllText(path + ".bak"));
            Assert.Contains("[Power]", File.ReadAllText(path));

            Assert.Equal(LogConfigResult.AlreadyOn, LogConfig.EnsureFile(path, out _));
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
        }
    }

    [Fact]
    public void EnsureFile_CreatesAMissingFileWithoutBackup()
    {
        var folder = Path.Combine(Path.GetTempPath(), "XpTrackerTests", Guid.NewGuid().ToString("N"));
        var path = Path.Combine(folder, "Hearthstone", "log.config");
        try
        {
            Assert.Equal(LogConfigResult.Updated, LogConfig.EnsureFile(path, out _));
            Assert.Equal(PowerOn, File.ReadAllText(path));
            Assert.False(File.Exists(path + ".bak"));
        }
        finally
        {
            Directory.Delete(folder, recursive: true);
        }
    }
}
