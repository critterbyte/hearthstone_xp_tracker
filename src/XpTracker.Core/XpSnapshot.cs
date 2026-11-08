namespace XpTracker.Core;

/// <summary>
/// Times of a game at one moment, in seconds.
/// </summary>
public sealed class XpSnapshot
{
    public XpSnapshot(double xpSeconds, double playSeconds, double withMulliganSeconds, bool isPaused)
    {
        XpSeconds = xpSeconds;
        PlaySeconds = playSeconds;
        WithMulliganSeconds = withMulliganSeconds;
        IsPaused = isPaused;
    }

    /// <summary>Time that counts towards reward track XP (before the 30-minute cap).</summary>
    public double XpSeconds { get; }

    /// <summary>Time since the first turn started.</summary>
    public double PlaySeconds { get; }

    /// <summary>Time since the mulligan started; equals <see cref="PlaySeconds"/> if the mulligan start is unknown.</summary>
    public double WithMulliganSeconds { get; }

    /// <summary>True while the current turn does not count (own AFK turn or the opponent turn after it).</summary>
    public bool IsPaused { get; }

    public int ExpectedXp(double rate) => XpRates.ExpectedXp(XpSeconds, rate);
}
