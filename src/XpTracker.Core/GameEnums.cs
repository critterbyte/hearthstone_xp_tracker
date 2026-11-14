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

    /// <summary>Arena. Measured in November 2026: pays the Casual rates, event XP as in the other modes.</summary>
    Arena,
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

/// <summary>Corner of the screen the overlay is placed in.</summary>
public enum OverlayPosition
{
    /// <summary>Bottom left, above the player's portrait.</summary>
    Left,

    /// <summary>Bottom right, above the Hearthstone settings button.</summary>
    Right,
}
