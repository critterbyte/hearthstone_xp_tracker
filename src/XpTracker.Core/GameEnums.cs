namespace XpTracker.Core;

public enum GameMode
{
    Other,
    Ranked,
    Casual,
    TavernBrawl,

    /// <summary>
    /// Tavern Brawl against the computer (the opponent has no account). Measured in November 2026:
    /// losses give no reward track XP and no event XP.
    /// </summary>
    TavernBrawlPvE,
}

public enum GameResult
{
    Unknown,
    Win,
    Loss,
    Tie,
}

public enum Side
{
    Player,
    Opponent,
}
