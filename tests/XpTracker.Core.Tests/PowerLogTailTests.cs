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
