using System.Text;
using XpTracker.Core;
using static XpTracker.Core.Tests.PowerLogTrackerTests;

namespace XpTracker.Core.Tests;

public sealed class PowerLogTailTests : IDisposable
{
    private readonly string _root = Path.Combine(Path.GetTempPath(), "xp-tracker-tests-" + Guid.NewGuid().ToString("N"));
    private readonly string _logFile;

    public PowerLogTailTests()
    {
        var folder = Path.Combine(_root, "Hearthstone_2026_11_08_09_10_51");
        Directory.CreateDirectory(folder);
        _logFile = Path.Combine(folder, "Power.log");
    }

    [Fact]
    public void StartingMidGame_ReadsOnlyTheCurrentGameFromItsStart()
    {
        Append(GameStart("09:20:00", "GT_RANKED", localStarts: true));
        Append(Power("09:25:00", "TAG_CHANGE Entity=GameEntity tag=STATE value=COMPLETE"));
        Append(GameStart("09:30:00", "GT_RANKED", localStarts: true));
        Append(
            Line("09:30:20", "GameState.SendChoices()", "id=1 ChoiceType=MULLIGAN"),
            Power("09:31:00", "TAG_CHANGE Entity=GameEntity tag=STEP value=MAIN_READY"));

        var tail = new PowerLogTail(() => _root);
        tail.Poll();

        var (xpSeconds, lines) = tail.Read(t => (t!.Clock!.GetSnapshot(new DateTime(2026, 11, 8, 9, 32, 0)).XpSeconds, t.LinesProcessed));
        Assert.Equal(60, xpSeconds, 3);
        Assert.Equal(12, lines);
    }

    [Fact]
    public void LaterPolls_ReadOnlyNewCompleteLines()
    {
        Append(GameStart("09:30:00", "GT_RANKED", localStarts: true));
        Append(Line("09:30:20", "GameState.SendChoices()", "id=1 ChoiceType=MULLIGAN"));
        var tail = new PowerLogTail(() => _root);
        tail.Poll();

        Append(Power("09:31:00", "TAG_CHANGE Entity=GameEntity tag=STEP value=MAIN_READY"));
        File.AppendAllText(_logFile, "D 09:31:05.0000000 GameState.DebugPrintPower() - TAG_CHANGE Entity=Bob tag=CURRENT_P", Encoding.UTF8);
        tail.Poll();

        // The unfinished last line is not processed yet, so the player's turn is still running.
        Assert.Equal(10, tail.Read(t => t!.Clock!.GetSnapshot(new DateTime(2026, 11, 8, 9, 31, 10)).XpSeconds), 3);
        Assert.Equal(12, tail.Read(t => t!.LinesProcessed));
    }

    [Fact]
    public void EmptiedFile_StartsOver()
    {
        Append(GameStart("09:30:00", "GT_RANKED", localStarts: true));
        var tail = new PowerLogTail(() => _root);
        tail.Poll();

        // Hearthstone empties the file on exit; the next poll (250 ms later) sees the shorter file.
        File.WriteAllText(_logFile, string.Empty);
        tail.Poll();
        Append(GameStart("09:40:00", "GT_CASUAL", localStarts: true));
        tail.Poll();

        Assert.Equal(GameMode.Casual, tail.Read(t => t!.Mode));
    }

    [Fact]
    public void NewSessionFolderWithoutLog_StartsAnEmptySessionAtOnce()
    {
        // Hearthstone was restarted: the old log was renamed, the new folder has no Power.log until the first game.
        Append(GameStart("09:30:00", "GT_RANKED", localStarts: true));
        var tail = new PowerLogTail(() => _root, TimeSpan.Zero);
        tail.Poll();
        File.Move(_logFile, Path.Combine(Path.GetDirectoryName(_logFile)!, "Power_old.log"));
        Directory.CreateDirectory(Path.Combine(_root, "Hearthstone_2026_11_08_12_00_00"));

        tail.Poll();

        Assert.Equal("Hearthstone_2026_11_08_12_00_00", tail.CurrentSession);
        Assert.Null(tail.Read(t => t!.Clock));
    }

    [Fact]
    public void LogAppearingLater_IsReadFromItsStart()
    {
        var folder = Path.Combine(_root, "Hearthstone_2026_11_08_12_00_00");
        Directory.CreateDirectory(folder);
        var tail = new PowerLogTail(() => _root, TimeSpan.Zero);
        tail.Poll();

        File.WriteAllText(Path.Combine(folder, "Power.log"), string.Concat(new[]
        {
            GameStart("12:05:00", "GT_CASUAL", localStarts: true),
            new[]
            {
                Line("12:05:20", "GameState.SendChoices()", "id=1 ChoiceType=MULLIGAN"),
                Power("12:06:00", "TAG_CHANGE Entity=GameEntity tag=STEP value=MAIN_READY"),
            },
        }.SelectMany(l => l).Select(l => l + "\r\n")), Encoding.UTF8);
        tail.Poll();

        Assert.Equal("Hearthstone_2026_11_08_12_00_00", tail.CurrentSession);
        Assert.Equal(GameMode.Casual, tail.Read(t => t!.Mode));
        Assert.Equal(60, tail.Read(t => t!.Clock!.GetSnapshot(new DateTime(2026, 11, 8, 12, 7, 0)).XpSeconds), 3);
    }

    [Fact]
    public void WithAClock_StartingAfterMidnight_DatesTheGameByTheClock()
    {
        // The session folder is from the 8th; the reader starts on the 9th, two minutes into a game that began at 00:10.
        Append(GameStart("00:10:00", "GT_RANKED", localStarts: true));
        Append(
            Line("00:10:00", "GameState.SendChoices()", "id=1 ChoiceType=MULLIGAN"),
            Power("00:10:00", "TAG_CHANGE Entity=GameEntity tag=STEP value=MAIN_READY"));
        var now = new DateTime(2026, 11, 9, 0, 12, 0);

        var tail = new PowerLogTail(() => _root, now: () => now);
        tail.Poll();

        Assert.Equal(120, tail.Read(t => t!.Clock!.GetSnapshot(now).XpSeconds), 3);
    }

    [Fact]
    public void FailingLine_IsSkippedAndTheReaderGoesOn()
    {
        Append(GameStart("09:30:00", "GT_RANKED", localStarts: true));
        Append(
            Line("09:30:20", "GameState.SendChoices()", "id=1 ChoiceType=MULLIGAN"),
            Power("09:31:00", "TAG_CHANGE Entity=GameEntity tag=STEP value=MAIN_READY"),
            Power("09:35:00", "TAG_CHANGE Entity=GameEntity tag=STATE value=COMPLETE"));
        var tail = new PowerLogTail(() => _root);
        tail.GameFinished += _ => throw new InvalidOperationException("handler failed");

        tail.Poll();
        Append(GameStart("09:40:00", "GT_CASUAL", localStarts: true));
        tail.Poll();

        Assert.Equal(1, tail.FailedLines);
        Assert.Equal(GameMode.Casual, tail.Read(t => t!.Mode));
        // 13 lines of the first game and 10 of the second, each read once.
        Assert.Equal(23, tail.Read(t => t!.LinesProcessed));
    }

    [Fact]
    public void MissingLogsFolder_DoesNothing()
    {
        var tail = new PowerLogTail(() => Path.Combine(_root, "missing"));
        tail.Poll();

        Assert.Null(tail.Read(t => t));
    }

    public void Dispose()
    {
        if (Directory.Exists(_root))
        {
            Directory.Delete(_root, recursive: true);
        }
    }

    private void Append(params string[] lines) =>
        File.AppendAllText(_logFile, string.Concat(lines.Select(l => l + "\r\n")), Encoding.UTF8);
}
