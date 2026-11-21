using System;

namespace XpTracker.Core;

/// <summary>
/// Rules for starting the tracker with Windows and for its Start menu shortcut. The app writes the registry value and
/// the shortcut; this class only builds and reads the values, so they can be tested without Windows.
/// </summary>
public static class WindowsStartup
{
    /// <summary>Name of the value under HKCU\Software\Microsoft\Windows\CurrentVersion\Run.</summary>
    public const string RunValueName = "HearthstoneXpTracker";

    /// <summary>Passed by the autostart entry: the tracker starts in the notification area without opening the dashboard.</summary>
    public const string AutostartArgument = "--autostart";

    /// <summary>File name of the shortcut in the user's Start menu folder; the text is what the Start menu shows.</summary>
    public const string ShortcutFileName = "Hearthstone XP Tracker.lnk";

    /// <summary>The command stored in the Run value: the quoted path of the EXE and the autostart argument.</summary>
    public static string RunCommand(string exePath) => "\"" + exePath + "\" " + AutostartArgument;

    /// <summary>The EXE path of a Run command, quoted or not; null for an empty value.</summary>
    public static string? ExePathOf(string? command)
    {
        var text = command?.Trim();
        if (string.IsNullOrEmpty(text))
        {
            return null;
        }

        if (text![0] == '"')
        {
            var end = text.IndexOf('"', 1);
            return end > 1 ? text.Substring(1, end - 1) : null;
        }

        // Without quotes the path ends before the first argument.
        var exe = text.IndexOf(".exe", StringComparison.OrdinalIgnoreCase);
        return exe > 0 ? text.Substring(0, exe + 4) : text;
    }

    /// <summary>Whether two Windows paths name the same file; letter case and slashes do not matter.</summary>
    public static bool SamePath(string? a, string? b)
    {
        if (a is null || b is null)
        {
            return false;
        }

        return string.Equals(Normalize(a), Normalize(b), StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Whether Windows runs a Run entry, from its value under Explorer\StartupApproved\Run. The user can switch an entry
    /// off in the Task Manager (tab "Startup apps"); Windows then sets an odd first byte. No value means the entry runs.
    /// </summary>
    public static bool ApprovedByWindows(byte[]? approved) => approved is not { Length: > 0 } || approved[0] % 2 == 0;

    /// <summary>Whether the command line asks for a start in the notification area only.</summary>
    public static bool IsAutostart(string[]? args)
    {
        if (args is null)
        {
            return false;
        }

        foreach (var arg in args)
        {
            if (string.Equals(arg, AutostartArgument, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }

        return false;
    }

    private static string Normalize(string path) => path.Trim().Replace('/', '\\').TrimEnd('\\');
}
