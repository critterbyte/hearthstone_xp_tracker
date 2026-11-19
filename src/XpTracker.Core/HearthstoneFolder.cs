using System;
using System.IO;

namespace XpTracker.Core;

/// <summary>
/// Finds Hearthstone's Logs folder, where the game writes one folder per session with its Power.log.
/// </summary>
public static class HearthstoneFolder
{
    /// <summary>
    /// The Logs folder of the Hearthstone installation, or null when none is found.
    /// </summary>
    /// <param name="runningGameExe">Path of the running Hearthstone.exe, or null when the game is not running.</param>
    /// <param name="installLocation">Installation folder from the Windows uninstall entry, or null.</param>
    /// <param name="programFilesX86">The "Program Files (x86)" folder, where Battle.net installs the game by default.</param>
    /// <param name="exists">Tells whether a folder exists; replaced in tests.</param>
    /// <remarks>
    /// The running game is the surest source. The folder from the uninstall entry and the default folder count only
    /// when they contain the game, because the Logs folder itself appears only after the first logged game.
    /// </remarks>
    public static string? FindLogsFolder(string? runningGameExe, string? installLocation, string? programFilesX86, Func<string, bool>? exists = null)
    {
        exists ??= Directory.Exists;

        if (!string.IsNullOrWhiteSpace(runningGameExe) && Path.GetDirectoryName(runningGameExe!.Trim()) is { Length: > 0 } running)
        {
            return Path.Combine(running, "Logs");
        }

        foreach (var candidate in new[] { installLocation, string.IsNullOrWhiteSpace(programFilesX86) ? null : Path.Combine(programFilesX86!, "Hearthstone") })
        {
            if (string.IsNullOrWhiteSpace(candidate))
            {
                continue;
            }

            var folder = candidate!.Trim().Trim('"').TrimEnd('\\', '/');
            if (folder.Length > 0 && exists(folder))
            {
                return Path.Combine(folder, "Logs");
            }
        }

        return null;
    }
}
