using System;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using Microsoft.Win32;
using XpTracker.Core;

namespace XpTracker.App;

/// <summary>
/// "Start with Windows" and the Start menu shortcut, both for the current user only and without administrator rights:
/// a value under HKCU\...\Run and a shortcut in %APPDATA%\Microsoft\Windows\Start Menu\Programs.
/// </summary>
/// <remarks>
/// The tracker is portable, so the user may move its folder. At every start, an entry or shortcut that points to an EXE
/// that no longer exists is pointed to the running EXE.
/// </remarks>
internal static class StartupIntegration
{
    private const string RunKey = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ApprovedKey = @"Software\Microsoft\Windows\CurrentVersion\Explorer\StartupApproved\Run";

    private static string ExePath => Assembly.GetEntryAssembly()?.Location ?? typeof(StartupIntegration).Assembly.Location;

    private static string ShortcutPath => Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.Programs),
        WindowsStartup.ShortcutFileName);

    /// <summary>Whether Windows starts the tracker at sign-in: the entry exists and is not switched off in the Task Manager.</summary>
    public static bool AutostartOn()
    {
        try
        {
            using var run = Registry.CurrentUser.OpenSubKey(RunKey);
            if (run?.GetValue(WindowsStartup.RunValueName) is not string command || WindowsStartup.ExePathOf(command) is null)
            {
                return false;
            }

            using var approved = Registry.CurrentUser.OpenSubKey(ApprovedKey);
            return WindowsStartup.ApprovedByWindows(approved?.GetValue(WindowsStartup.RunValueName) as byte[]);
        }
        catch (Exception ex) when (ex is System.Security.SecurityException or UnauthorizedAccessException or IOException)
        {
            AppLog.Error(ex);
            return false;
        }
    }

    /// <summary>Adds or removes the autostart entry; returns whether it worked.</summary>
    public static bool SetAutostart(bool on)
    {
        try
        {
            using var run = Registry.CurrentUser.CreateSubKey(RunKey);
            if (on)
            {
                run.SetValue(WindowsStartup.RunValueName, WindowsStartup.RunCommand(ExePath), RegistryValueKind.String);

                // Ticking the box again also undoes "Disable" in the Task Manager.
                using var approved = Registry.CurrentUser.OpenSubKey(ApprovedKey, writable: true);
                approved?.DeleteValue(WindowsStartup.RunValueName, throwOnMissingValue: false);
            }
            else
            {
                run.DeleteValue(WindowsStartup.RunValueName, throwOnMissingValue: false);
            }

            return true;
        }
        catch (Exception ex) when (ex is System.Security.SecurityException or UnauthorizedAccessException or IOException)
        {
            AppLog.Error(ex);
            return false;
        }
    }

    /// <summary>Whether the Start menu shortcut exists.</summary>
    public static bool ShortcutExists() => File.Exists(ShortcutPath);

    /// <summary>Creates or deletes the Start menu shortcut; returns whether it worked.</summary>
    public static bool SetShortcut(bool on)
    {
        try
        {
            if (on)
            {
                WriteShortcut(ShortcutPath, ExePath);
            }
            else if (File.Exists(ShortcutPath))
            {
                File.Delete(ShortcutPath);
            }

            return true;
        }
        catch (Exception ex) when (ex is COMException or IOException or UnauthorizedAccessException or TargetInvocationException)
        {
            AppLog.Error(ex);
            return false;
        }
    }

    /// <summary>Points the autostart entry and the shortcut to the running EXE when their EXE no longer exists.</summary>
    public static void RepairMovedFolder()
    {
        try
        {
            using (var run = Registry.CurrentUser.OpenSubKey(RunKey))
            {
                var target = WindowsStartup.ExePathOf(run?.GetValue(WindowsStartup.RunValueName) as string);
                if (target is not null && !WindowsStartup.SamePath(target, ExePath) && !File.Exists(target))
                {
                    using var writable = Registry.CurrentUser.CreateSubKey(RunKey);
                    writable.SetValue(WindowsStartup.RunValueName, WindowsStartup.RunCommand(ExePath), RegistryValueKind.String);
                    AppLog.Info("Pointed the autostart entry to " + ExePath);
                }
            }

            if (File.Exists(ShortcutPath))
            {
                var target = ReadShortcutTarget(ShortcutPath);
                if (!string.IsNullOrEmpty(target) && !WindowsStartup.SamePath(target, ExePath) && !File.Exists(target))
                {
                    WriteShortcut(ShortcutPath, ExePath);
                    AppLog.Info("Pointed the Start menu shortcut to " + ExePath);
                }
            }
        }
        catch (Exception ex) when (ex is System.Security.SecurityException or UnauthorizedAccessException or IOException or COMException or TargetInvocationException)
        {
            AppLog.Error(ex);
        }
    }

    // The shortcut is written with the Windows Script Host shell object, which is part of Windows.
    private static void WriteShortcut(string path, string exe)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(path)!);
        WithShortcut(path, (type, link) =>
        {
            type.InvokeMember("TargetPath", BindingFlags.SetProperty, null, link, new object[] { exe });
            type.InvokeMember("WorkingDirectory", BindingFlags.SetProperty, null, link, new object[] { Path.GetDirectoryName(exe)! });
            type.InvokeMember("IconLocation", BindingFlags.SetProperty, null, link, new object[] { exe + ",0" });
            type.InvokeMember("Description", BindingFlags.SetProperty, null, link, new object[] { "Hearthstone XP Tracker" });
            type.InvokeMember("Save", BindingFlags.InvokeMethod, null, link, null);
            return null;
        });
    }

    private static string? ReadShortcutTarget(string path) =>
        WithShortcut(path, (type, link) => type.InvokeMember("TargetPath", BindingFlags.GetProperty, null, link, null) as string);

    private static string? WithShortcut(string path, Func<Type, object, string?> use)
    {
        var shellType = Type.GetTypeFromProgID("WScript.Shell") ?? throw new COMException("WScript.Shell is not available.");
        var shell = Activator.CreateInstance(shellType);
        object? link = null;
        try
        {
            link = shellType.InvokeMember("CreateShortcut", BindingFlags.InvokeMethod, null, shell, new object[] { path });
            return use(link!.GetType(), link);
        }
        finally
        {
            if (link is not null)
            {
                Marshal.FinalReleaseComObject(link);
            }

            Marshal.FinalReleaseComObject(shell);
        }
    }
}
