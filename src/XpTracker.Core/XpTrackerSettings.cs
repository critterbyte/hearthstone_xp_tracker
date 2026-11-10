using System;
using System.Collections.Generic;
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
