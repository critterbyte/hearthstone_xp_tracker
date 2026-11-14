using System.Collections.Generic;
using System.Globalization;

namespace XpTracker.Core;

/// <summary>
/// XP boost of the Tavern Pass, which grows with the rewards track level.
/// </summary>
/// <remarks>
/// Not measured yet. According to the Hearthstone Wiki and senseos.gg (November 2026, Reign of the Black Empire),
/// the pass gives 10% from the purchase, 15% from level 20 and 20% from level 70; older expansions used other levels.
/// The boost is applied before rounding: XP = round(seconds × rate × (1 + boost)). See docs/FORMULA.md.
/// </remarks>
public static class TavernPassBonus
{
    public const int MinLevel = 1;
    public const int MaxLevel = 999;

    /// <summary>Boost steps: from this rewards track level on, this boost in percent applies. Sorted by level.</summary>
    public static readonly IReadOnlyList<BonusStep> Steps = new[]
    {
        new BonusStep(1, 10),
        new BonusStep(20, 15),
        new BonusStep(70, 20),
    };

    /// <summary>Boost in percent for a player with or without the pass at this rewards track level.</summary>
    public static int Percent(bool hasPass, int level)
    {
        if (!hasPass)
        {
            return 0;
        }

        var percent = 0;
        var clamped = ClampLevel(level);
        foreach (var step in Steps)
        {
            if (clamped >= step.Level)
            {
                percent = step.Percent;
            }
        }

        return percent;
    }

    /// <summary>The next step above this level, or null if the highest boost is reached.</summary>
    public static BonusStep? NextStep(int level)
    {
        var clamped = ClampLevel(level);
        foreach (var step in Steps)
        {
            if (step.Level > clamped)
            {
                return step;
            }
        }

        return null;
    }

    public static int ClampLevel(int level) => level < MinLevel ? MinLevel : level > MaxLevel ? MaxLevel : level;

    /// <summary>Short text for the dashboard, for example "15% XP boost, 50 levels to 20%".</summary>
    public static string Describe(bool hasPass, int level)
    {
        if (!hasPass)
        {
            return "No XP boost";
        }

        var text = Percent(true, level).ToString(CultureInfo.InvariantCulture) + "% XP boost";
        if (NextStep(level) is not { } next)
        {
            return text + " (maximum)";
        }

        var levels = next.Level - ClampLevel(level);
        return text + ", " + levels.ToString(CultureInfo.InvariantCulture) + (levels == 1 ? " level" : " levels")
            + " to " + next.Percent.ToString(CultureInfo.InvariantCulture) + "%";
    }
}

/// <summary>One step of the Tavern Pass boost: from <see cref="Level"/> on, <see cref="Percent"/> applies.</summary>
public sealed class BonusStep
{
    public BonusStep(int level, int percent)
    {
        Level = level;
        Percent = percent;
    }

    public int Level { get; }

    public int Percent { get; }
}
