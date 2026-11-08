using System.Globalization;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class DeckMatcherTests
{
    // HDT records and history rows of measured games (start = HDT game start / first turn).
    private static readonly DeckInfo[] Hdt =
    {
        Info("00:12:29", "00:15:42", "Core Rogue", "Priest"),       // HDT started during the game
        Info("11:50:02", "11:50:30", "Fast Rogue AI OP", "Warrior"), // HDT restarted its record at a Rewind
        Info("15:45:58", "15:52:42", "Fast Rogue AI OP", "Mage"),
        Info("16:00:48", "16:17:13", "Fast Rogue AI OP", "Priest"),
    };

    [Theory]
    [InlineData("00:10:40", "00:15:40", "Core Rogue")]
    [InlineData("11:42:22", "11:50:30", "Fast Rogue AI OP")]
    [InlineData("15:45:24", "15:52:42", "Fast Rogue AI OP")]
    [InlineData("16:01:15", "16:17:11", "Fast Rogue AI OP")]
    public void Find_MatchesByEndTime(string start, string end, string deck)
    {
        Assert.Equal(deck, DeckMatcher.Find(Record(start, end), Hdt)?.Deck);
    }

    [Fact]
    public void Find_ReturnsNullForAGameHdtDidNotRecord()
    {
        Assert.Null(DeckMatcher.Find(Record("15:33:42", "15:34:16"), Hdt));
    }

    [Fact]
    public void Find_IgnoresAnHdtGameThatStartedAfterTheRecordEnded()
    {
        var later = new[] { Info("12:00:30", "12:00:50", "Other", "Mage") };

        Assert.Null(DeckMatcher.Find(Record("11:59:00", "12:00:20"), later));
    }

    [Fact]
    public void FillMissing_FillsOnlyRecordsWithoutDeck()
    {
        var missing = Record("16:01:15", "16:17:11");
        var known = Record("15:45:24", "15:52:42");
        known.Deck = "Kept";
        known.OpponentClass = "Kept";

        var changed = DeckMatcher.FillMissing(new[] { missing, known }, Hdt);

        Assert.Equal(1, changed);
        Assert.Equal("Fast Rogue AI OP", missing.Deck);
        Assert.Equal("Rogue", missing.PlayerClass);
        Assert.Equal("Priest", missing.OpponentClass);
        Assert.Equal("Kept", known.Deck);
    }

    private static DeckInfo Info(string start, string end, string deck, string opponent) =>
        new(At(start), At(end), deck, "Rogue", opponent);

    private static GameRecord Record(string start, string end) => new() { Start = At(start), End = At(end), Mode = GameMode.Ranked };

    private static DateTime At(string time) =>
        DateTime.ParseExact("2026-11-08 " + time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
