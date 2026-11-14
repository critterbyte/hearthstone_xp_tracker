using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace XpTracker.Core;

/// <summary>
/// User settings of the plugin, stored as "Key=value" lines.
/// </summary>
public sealed class XpTrackerSettings
{
    /// <summary>Ask GitHub for a newer release when HDT starts.</summary>
    public bool CheckForUpdates { get; set; } = true;

    /// <summary>A Hearthstone event is running: show and store event XP. Switched on and off by the user.</summary>
    public bool EventActive { get; set; }

    /// <summary>Show the rows "Total Damage:" and "Hero Damage:" on the overlay.</summary>
    public bool ShowDamage { get; set; } = true;

    /// <summary>Language of the overlay and the dashboard, for example "en" or "de"; English by default.</summary>
    public string Language { get; set; } = Localizer.DefaultLanguage;

    /// <summary>The player owns the Tavern Pass of the current expansion: XP get the pass boost.</summary>
    public bool TavernPass { get; set; }

    /// <summary>Rewards track level, entered by the user; sets the size of the Tavern Pass boost.</summary>
    public int TavernPassLevel { get; set; } = TavernPassBonus.MinLevel;

    /// <summary>The Tavern Pass boost in percent that applies now; 0 without the pass.</summary>
    public int XpBonusPercent => TavernPassBonus.Percent(TavernPass, TavernPassLevel);

    /// <summary>Reads the settings; unknown keys and invalid values are ignored and keep their defaults.</summary>
    public static XpTrackerSettings Parse(IEnumerable<string> lines)
    {
        var settings = new XpTrackerSettings();
        foreach (var line in lines)
        {
            var separator = line.IndexOf('=');
            if (separator <= 0)
            {
                continue;
            }

            var key = line.Substring(0, separator).Trim();
            var value = line.Substring(separator + 1).Trim();
            if (key.Equals(nameof(CheckForUpdates), StringComparison.OrdinalIgnoreCase) && bool.TryParse(value, out var check))
            {
                settings.CheckForUpdates = check;
            }
            else if (key.Equals(nameof(EventActive), StringComparison.OrdinalIgnoreCase) && bool.TryParse(value, out var eventActive))
            {
                settings.EventActive = eventActive;
            }
            else if (key.Equals(nameof(ShowDamage), StringComparison.OrdinalIgnoreCase) && bool.TryParse(value, out var showDamage))
            {
                settings.ShowDamage = showDamage;
            }
            else if (key.Equals(nameof(Language), StringComparison.OrdinalIgnoreCase))
            {
                settings.Language = Localizer.Normalize(value);
            }
            else if (key.Equals(nameof(TavernPass), StringComparison.OrdinalIgnoreCase) && bool.TryParse(value, out var tavernPass))
            {
                settings.TavernPass = tavernPass;
            }
            else if (key.Equals(nameof(TavernPassLevel), StringComparison.OrdinalIgnoreCase)
                && int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var level))
            {
                settings.TavernPassLevel = TavernPassBonus.ClampLevel(level);
            }
        }

        return settings;
    }

    /// <summary>Loads the settings from a file; a missing or unreadable file gives the defaults.</summary>
    public static XpTrackerSettings Load(string path)
    {
        try
        {
            return File.Exists(path) ? Parse(File.ReadAllLines(path)) : new XpTrackerSettings();
        }
        catch (IOException)
        {
            return new XpTrackerSettings();
        }
        catch (UnauthorizedAccessException)
        {
            return new XpTrackerSettings();
        }
    }

    public IEnumerable<string> ToLines()
    {
        yield return nameof(CheckForUpdates) + "=" + (CheckForUpdates ? "true" : "false");
        yield return nameof(EventActive) + "=" + (EventActive ? "true" : "false");
        yield return nameof(ShowDamage) + "=" + (ShowDamage ? "true" : "false");
        yield return nameof(Language) + "=" + Language;
        yield return nameof(TavernPass) + "=" + (TavernPass ? "true" : "false");
        yield return nameof(TavernPassLevel) + "=" + TavernPassLevel.ToString(CultureInfo.InvariantCulture);
    }

    public void Save(string path)
    {
        var folder = Path.GetDirectoryName(path);
        if (!string.IsNullOrEmpty(folder))
        {
            Directory.CreateDirectory(folder);
        }

        File.WriteAllLines(path, ToLines());
    }
}
