using System.Globalization;

namespace XpTracker.Core;

/// <summary>
/// Display names of game terms in the current language. Stored data keeps the English codes ("Ranked", "Win", "Wild", class names).
/// </summary>
public static class GameText
{
    public static string Result(GameResult result) => Localizer.TextOrNull("result." + result) ?? result.ToString();

    public static string Mode(GameMode mode) => Localizer.TextOrNull("mode." + mode) ?? mode.ToString();

    /// <summary>"Wild", "Standard" and so on; unknown formats are shown as stored.</summary>
    public static string Format(string format) => format.Length == 0 ? string.Empty : Localizer.TextOrNull("format." + format) ?? format;

    /// <summary>Class name as HDT stores it ("Deathknight", "DemonHunter", "Mage" …); unknown classes are shown capitalized.</summary>
    public static string Class(string storedClass)
    {
        if (storedClass.Length == 0)
        {
            return string.Empty;
        }

        var key = "class." + storedClass.Replace(" ", string.Empty).ToUpperInvariant();
        return Localizer.TextOrNull(key)
            ?? char.ToUpperInvariant(storedClass[0]) + storedClass.Substring(1).ToLower(CultureInfo.InvariantCulture);
    }
}
