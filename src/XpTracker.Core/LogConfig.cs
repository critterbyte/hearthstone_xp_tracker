using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XpTracker.Core;

/// <summary>Result of <see cref="LogConfig.EnsureFile"/>.</summary>
public enum LogConfigResult
{
    /// <summary>The game log was already switched on; the file is unchanged.</summary>
    AlreadyOn,

    /// <summary>The missing settings were added; Hearthstone writes the log after its next start.</summary>
    Updated,

    /// <summary>The file could not be read or written.</summary>
    Failed,
}

/// <summary>
/// Switches on Hearthstone's Power.log in <c>%LOCALAPPDATA%\Blizzard\Hearthstone\log.config</c>, the game's own
/// logging configuration.
/// </summary>
/// <remarks>
/// Other tools such as Hearthstone Deck Tracker write their sections into the same file, so only the [Power] section
/// is touched and only the values the tracker needs are set. Everything else stays as it is, including comments,
/// order and line endings. Hearthstone reads the file when it starts.
/// </remarks>
public static class LogConfig
{
    public const string Section = "Power";

    // Values the tracker needs: written to a file at all, normal detail, and verbose, which adds the tag changes the
    // turns and times are read from.
    private static readonly (string Key, string Value)[] Required =
    {
        ("LogLevel", "1"),
        ("FilePrinting", "True"),
        ("Verbose", "True"),
    };

    // Written only with a new section, as Hearthstone Deck Tracker does; existing values are kept.
    private static readonly (string Key, string Value)[] NewSectionOnly =
    {
        ("ConsolePrinting", "False"),
        ("ScreenPrinting", "False"),
    };

    /// <summary>The default location of the file for the current Windows user.</summary>
    public static string DefaultPath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "Blizzard", "Hearthstone", "log.config");

    /// <summary>
    /// Returns the file content with the [Power] section switched on, or null when nothing has to change.
    /// </summary>
    /// <param name="content">Current content, or null when the file does not exist.</param>
    public static string? Update(string? content)
    {
        var newline = content is not null && !content.Contains("\r\n") && content.Contains("\n") ? "\n" : "\r\n";
        var lines = string.IsNullOrEmpty(content) ? new List<string>() : content!.Replace("\r\n", "\n").Split('\n').ToList();
        var endsWithNewline = lines.Count > 0 && lines[lines.Count - 1].Length == 0;
        if (endsWithNewline)
        {
            lines.RemoveAt(lines.Count - 1);
        }

        var changed = false;
        var start = lines.FindIndex(l => IsSection(l, Section));
        if (start < 0)
        {
            lines.Add("[" + Section + "]");
            lines.AddRange(Required.Take(2).Concat(NewSectionOnly).Concat(Required.Skip(2)).Select(p => p.Key + "=" + p.Value));
            changed = true;
        }
        else
        {
            var end = lines.FindIndex(start + 1, l => l.TrimStart().StartsWith("[", StringComparison.Ordinal));
            if (end < 0)
            {
                end = lines.Count;
            }

            foreach (var (key, value) in Required)
            {
                var index = lines.FindIndex(start + 1, end - start - 1, l => KeyOf(l) is { } k && string.Equals(k, key, StringComparison.OrdinalIgnoreCase));
                if (index < 0)
                {
                    lines.Insert(end, key + "=" + value);
                    end++;
                    changed = true;
                }
                else if (!string.Equals(ValueOf(lines[index]), value, StringComparison.OrdinalIgnoreCase))
                {
                    lines[index] = key + "=" + value;
                    changed = true;
                }
            }
        }

        if (!changed)
        {
            return null;
        }

        return string.Join(newline, lines) + newline;
    }

    /// <summary>
    /// Switches on the game log in the file at <paramref name="path"/>. Before the first change a copy of the old file
    /// is kept as <c>log.config.bak</c>.
    /// </summary>
    public static LogConfigResult EnsureFile(string path, out string? error)
    {
        error = null;
        try
        {
            var content = File.Exists(path) ? File.ReadAllText(path) : null;
            var updated = Update(content);
            if (updated is null)
            {
                return LogConfigResult.AlreadyOn;
            }

            var folder = Path.GetDirectoryName(path);
            if (!string.IsNullOrEmpty(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var backup = path + ".bak";
            if (content is not null && !File.Exists(backup))
            {
                File.Copy(path, backup);
            }

            File.WriteAllText(path, updated);
            return LogConfigResult.Updated;
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or System.Security.SecurityException)
        {
            error = ex.Message;
            return LogConfigResult.Failed;
        }
    }

    private static bool IsSection(string line, string name)
    {
        var trimmed = line.Trim();
        return trimmed.Length > 2 && trimmed[0] == '[' && trimmed[trimmed.Length - 1] == ']'
            && string.Equals(trimmed.Substring(1, trimmed.Length - 2).Trim(), name, StringComparison.OrdinalIgnoreCase);
    }

    private static string? KeyOf(string line)
    {
        var equals = line.IndexOf('=');
        return equals <= 0 ? null : line.Substring(0, equals).Trim();
    }

    private static string ValueOf(string line) => line.Substring(line.IndexOf('=') + 1).Trim();
}
