namespace XpTracker.Core;

public enum GamePhase
{
    /// <summary>No game (menu), or the game is still being set up.</summary>
    None,

    /// <summary>
    /// In a game, but the state cannot be read right now, for example while a Rewind recreates the player entities.
    /// </summary>
    Unavailable,
    Mulligan,
    Turns,
    Over,
}

/// <summary>
/// The parts of the game state the XP clock needs, read from the game at one moment.
/// </summary>
public sealed class GameSample
{
    public GameSample(GamePhase phase, Side? currentSide, int playerTurnLimit, GameResult result, GameMode mode)
    {
        Phase = phase;
        CurrentSide = currentSide;
        PlayerTurnLimit = playerTurnLimit;
        Result = result;
        Mode = mode;
    }

    public GamePhase Phase { get; }

    /// <summary>Whose turn it is; null if unknown.</summary>
    public Side? CurrentSide { get; }

    /// <summary>The local player's turn limit in seconds (TIMEOUT tag); 10 after an AFK turn.</summary>
    public int PlayerTurnLimit { get; }

    public GameResult Result { get; }

    public GameMode Mode { get; }

    public static GameSample NoGame { get; } = new(GamePhase.None, null, 0, GameResult.Unknown, GameMode.Other);

    public static GameSample Unavailable { get; } = new(GamePhase.Unavailable, null, 0, GameResult.Unknown, GameMode.Other);
}
