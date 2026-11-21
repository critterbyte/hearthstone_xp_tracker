using XpTracker.Core;
using static XpTracker.Core.Tests.PowerLogTrackerTests;

namespace XpTracker.Core.Tests;

/// <summary>
/// The classes of both heroes come from the log's game start: each player's HERO_ENTITY, and the CLASS of that hero.
/// </summary>
public class HeroClassTests
{
    [Fact]
    public void BothClasses_ComeFromTheHeroesAtTheStart()
    {
        var game = PlayGame(localIsPlayer1: true, "ROGUE", "PALADIN");

        Assert.Equal("Rogue", game.PlayerClass);
        Assert.Equal("Paladin", game.OpponentClass);

        var record = GameRecord.From(game);
        Assert.Equal("Rogue", record.PlayerClass);
        Assert.Equal("Paladin", record.OpponentClass);
    }

    [Fact]
    public void LocalPlayer_CanBePlayerTwo()
    {
        var game = PlayGame(localIsPlayer1: false, "WARLOCK", "DEATHKNIGHT");

        Assert.Equal("Deathknight", game.PlayerClass);
        Assert.Equal("Warlock", game.OpponentClass);
    }

    [Theory]
    [InlineData("DEMONHUNTER", "DemonHunter", "Demon Hunter")]
    [InlineData("DEATHKNIGHT", "Deathknight", "Death Knight")]
    [InlineData("PRIEST", "Priest", "Priest")]
    public void ClassNames_AreStoredInTheHistoryFormatAndShownLocalized(string logClass, string stored, string shown)
    {
        Localizer.SetLanguage("en");

        var game = PlayGame(localIsPlayer1: true, "MAGE", logClass);

        Assert.Equal(stored, game.OpponentClass);
        Assert.Equal(shown, GameText.Class(game.OpponentClass));
    }

    [Fact]
    public void HeroPlayedLater_DoesNotChangeTheClass()
    {
        var game = PlayGame(localIsPlayer1: true, "ROGUE", "PRIEST", afterStart: new[]
        {
            // A hero card from another class replaces the hero during the game.
            Power("12:03:00", "FULL_ENTITY - Creating ID=90 CardID=HERO_08"),
            Power("12:03:00", "    tag=CARDTYPE value=HERO"),
            Power("12:03:00", "    tag=CLASS value=MAGE"),
            Power("12:03:00", "TAG_CHANGE Entity=Alice tag=HERO_ENTITY value=90"),
        });

        Assert.Equal("Rogue", game.PlayerClass);
    }

    [Fact]
    public void LogWithoutHeroes_LeavesTheClassesEmpty()
    {
        var finished = new List<FinishedGame>();
        var tracker = new PowerLogTracker(new DateTime(2026, 11, 8));
        tracker.GameFinished += finished.Add;

        foreach (var line in GameStart("12:00:00", "GT_RANKED", localStarts: true).Concat(new[] { MainReady }).Concat(new[] { Complete("12:05:00") }))
        {
            tracker.ProcessLine(line);
        }

        var game = Assert.Single(finished);
        Assert.Equal(string.Empty, game.PlayerClass);
        Assert.Equal(string.Empty, game.OpponentClass);
    }

    private static FinishedGame PlayGame(bool localIsPlayer1, string player1Class, string player2Class, string[]? afterStart = null)
    {
        const string t = "12:00:00";
        var finished = new List<FinishedGame>();
        var tracker = new PowerLogTracker(new DateTime(2026, 11, 8));
        tracker.GameFinished += finished.Add;

        var local = localIsPlayer1 ? "Alice" : "Bob";
        var lines = new List<string>
        {
            Power(t, "CREATE_GAME"),
            Power(t, "    GameEntity EntityID=1"),
            Power(t, "        tag=TURN value=1"),
            Power(t, "    Player EntityID=2 PlayerID=1 GameAccountId=[hi=1 lo=1]"),
            Power(t, "        tag=HERO_ENTITY value=64"),
            Power(t, "        tag=CONTROLLER value=1"),
            Power(t, "    Player EntityID=3 PlayerID=2 GameAccountId=[hi=2 lo=2]"),
            Power(t, "        tag=HERO_ENTITY value=66"),
            Power(t, "        tag=CONTROLLER value=2"),
            Power(t, "    FULL_ENTITY - Creating ID=64 CardID=HERO_TEST1"),
            Power(t, "        tag=CARDTYPE value=HERO"),
            Power(t, "        tag=CLASS value=" + player1Class),
            Power(t, "    FULL_ENTITY - Creating ID=65 CardID=HERO_TEST1hp"),
            Power(t, "        tag=CARDTYPE value=HERO_POWER"),
            Power(t, "        tag=CLASS value=NEUTRAL"),
            Power(t, "    FULL_ENTITY - Creating ID=66 CardID=HERO_TEST2"),
            Power(t, "        tag=CARDTYPE value=HERO"),
            Power(t, "        tag=CLASS value=" + player2Class),
            Line(t, "GameState.DebugPrintGame()", "GameType=GT_RANKED"),
            Line(t, "GameState.DebugPrintGame()", "FormatType=FT_WILD"),
            Line(t, "GameState.DebugPrintGame()", "PlayerID=1, PlayerName=" + (localIsPlayer1 ? "Alice" : "UNKNOWN HUMAN PLAYER")),
            Line(t, "GameState.DebugPrintGame()", "PlayerID=2, PlayerName=" + (localIsPlayer1 ? "UNKNOWN HUMAN PLAYER" : "Bob")),
            Power(t, "    TAG_CHANGE Entity=2 tag=CURRENT_PLAYER value=1"),
            Line(t, "GameState.DebugPrintEntityChoices()", "id=" + (localIsPlayer1 ? "1" : "2") + " Player=" + local + " TaskList=4 ChoiceType=MULLIGAN CountMin=0 CountMax=3"),
            Line(t, "GameState.SendChoices()", "id=" + (localIsPlayer1 ? "1" : "2") + " ChoiceType=MULLIGAN"),
        };
        lines.Add(MainReady);
        lines.AddRange(afterStart ?? Array.Empty<string>());
        lines.Add(Complete("12:05:00"));

        foreach (var line in lines)
        {
            tracker.ProcessLine(line);
        }

        return Assert.Single(finished);
    }

    private static readonly string MainReady = Power("12:00:30", "TAG_CHANGE Entity=GameEntity tag=STEP value=MAIN_READY");

    private static string Complete(string time) => Power(time, "TAG_CHANGE Entity=GameEntity tag=STATE value=COMPLETE");
}
