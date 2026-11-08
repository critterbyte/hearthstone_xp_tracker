using XpTracker.Core;

namespace XpTracker.Core.Tests;

public sealed class GameHistoryTests : IDisposable
{
    private readonly string _folder = Path.Combine(Path.GetTempPath(), "xp-tracker-history-" + Guid.NewGuid().ToString("N"));

    private string File => Path.Combine(_folder, "games.csv");

    [Fact]
    public void AddedGames_AreLoadedBackUnchanged()
    {
        var history = new GameHistory(File);
        var game = Sample("10:18:45");
        game.Deck = "Pirate, \"Fast\" Rogue";

        Assert.True(history.Add(game));
        var loaded = Assert.Single(new GameHistory(File).Load());

        Assert.Equal(game.Key, loaded.Key);
        // Times are stored with millisecond precision.
        Assert.Equal(game.End, loaded.End, TimeSpan.FromMilliseconds(1));
        Assert.Equal(GameMode.Ranked, loaded.Mode);
        Assert.Equal("Wild", loaded.Format);
        Assert.Equal(GameResult.Win, loaded.Result);
        Assert.Equal(349.68, loaded.XpSeconds, 2);
        Assert.Equal(412.14, loaded.PlaySeconds, 2);
        Assert.Equal(472.62, loaded.WithMulliganSeconds, 2);
        Assert.Equal(1, loaded.AfkTurns);
        Assert.Equal(46, loaded.Xp);
        Assert.Equal("Pirate, \"Fast\" Rogue", loaded.Deck);
        Assert.Equal("ROGUE", loaded.PlayerClass);
        Assert.Equal("WARRIOR", loaded.OpponentClass);
    }

    [Fact]
    public void SameGameTwice_IsStoredOnce_AlsoAcrossInstances()
    {
        Assert.True(new GameHistory(File).Add(Sample("10:18:45")));

        var reopened = new GameHistory(File);
        Assert.False(reopened.Add(Sample("10:18:45")));
        Assert.True(reopened.Add(Sample("10:30:00")));

        Assert.Equal(2, new GameHistory(File).Load().Count);
    }

    [Fact]
    public void GameWithoutRate_StoresEmptyXp()
    {
        var game = Sample("10:18:45");
        game.Mode = GameMode.Other;
        game.Xp = null;
        new GameHistory(File).Add(game);

        Assert.Null(Assert.Single(new GameHistory(File).Load()).Xp);
    }

    [Fact]
    public void MissingFile_LoadsEmpty()
    {
        Assert.Empty(new GameHistory(File).Load());
    }

    [Fact]
    public void SessionStart_IsReadFromTheLogFolderName()
    {
        Assert.Equal(new DateTime(2026, 11, 8, 9, 10, 51), GameRecord.SessionStart("Hearthstone_2026_11_08_09_10_51"));
        Assert.Null(GameRecord.SessionStart("unknown"));
    }

    public void Dispose()
    {
        if (Directory.Exists(_folder))
        {
            Directory.Delete(_folder, recursive: true);
        }
    }

    private static GameRecord Sample(string start) => new()
    {
        Session = "Hearthstone_2026_11_08_09_10_51",
        Start = DateTime.Parse("2026-11-08 " + start, System.Globalization.CultureInfo.InvariantCulture),
        End = DateTime.Parse("2026-11-08 " + start, System.Globalization.CultureInfo.InvariantCulture).AddSeconds(412.14),
        Mode = GameMode.Ranked,
        Format = "Wild",
        Result = GameResult.Win,
        XpSeconds = 349.68,
        PlaySeconds = 412.14,
        WithMulliganSeconds = 472.62,
        AfkTurns = 1,
        Xp = 46,
        PlayerClass = "ROGUE",
        OpponentClass = "WARRIOR",
    };
}
