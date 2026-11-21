using System;
using System.Globalization;
using System.IO;

namespace XpTracker.App;

/// <summary>
/// Small log file for unexpected errors, in the app's data folder. It never grows beyond about 1 MB.
/// </summary>
internal static class AppLog
{
    private const long MaxSize = 1024 * 1024;
    private static readonly object Sync = new();

    public static string FilePath => Path.Combine(XpTrackerApp.DataFolder, "xptracker.log");

    public static void Error(Exception ex) => Write("Error", ex.ToString());

    public static void Info(string text) => Write("Info", text);

    private static void Write(string level, string text)
    {
        try
        {
            lock (Sync)
            {
                Directory.CreateDirectory(XpTrackerApp.DataFolder);
                var file = new FileInfo(FilePath);
                if (file.Exists && file.Length > MaxSize)
                {
                    // Keep the previous part once, so the last errors before the cut are not lost.
                    File.Copy(FilePath, FilePath + ".old", overwrite: true);
                    File.Delete(FilePath);
                }

                var line = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture) + " " + level + " " + text + Environment.NewLine;
                File.AppendAllText(FilePath, line);
            }
        }
        catch (IOException)
        {
            // Logging must never stop the app.
        }
        catch (UnauthorizedAccessException)
        {
        }
    }
}
