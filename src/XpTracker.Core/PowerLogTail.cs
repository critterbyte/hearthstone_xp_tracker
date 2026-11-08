using System;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace XpTracker.Core;

/// <summary>
/// Follows the newest Power.log under Hearthstone's Logs folder and feeds new lines to a <see cref="PowerLogTracker"/>.
/// </summary>
/// <remarks>
/// On the first read it seeks back to the start of the current game only, so starting the tracker in the
/// middle of a game still gives the full game time. Afterwards only new complete lines are read.
/// The file is opened read-only and shared, like Hearthstone Deck Tracker does.
/// </remarks>
public sealed class PowerLogTail
{
    private const string GameStartMarker = "GameState.DebugPrintPower() - CREATE_GAME";
    private const int ChunkSize = 64 * 1024;
    private const int LinePrefixLength = 19;

    private readonly Func<string?> _logsRoot;
    private readonly object _sync = new();
    private string? _path;
    private long _position;
    private PowerLogTracker? _tracker;
    private DateTime _nextFolderCheck;

    /// <param name="logsRoot">Returns Hearthstone's Logs folder; called again when the folder may have changed.</param>
    public PowerLogTail(Func<string?> logsRoot)
    {
        _logsRoot = logsRoot;
    }

    /// <summary>Raised on the polling thread when a game ends in the log.</summary>
    public event Action<FinishedGame>? GameFinished;

    /// <summary>Name of the log session being read (Hearthstone_yyyy_MM_dd_HH_mm_ss), or null before the first read.</summary>
    public string? CurrentSession
    {
        get
        {
            var path = _path;
            return path is null ? null : Path.GetFileName(Path.GetDirectoryName(path));
        }
    }

    /// <summary>Time the last <see cref="Poll"/> took, for performance checks.</summary>
    public TimeSpan LastPollDuration { get; private set; }

    /// <summary>Reads new lines. Call periodically from a background thread.</summary>
    public void Poll()
    {
        var watch = Stopwatch.StartNew();
        var now = DateTime.UtcNow;
        if (_path is null || now >= _nextFolderCheck)
        {
            _nextFolderCheck = now.AddSeconds(5);
            var newest = FindNewestPowerLog(_logsRoot());
            if (newest is not null && !string.Equals(newest, _path, StringComparison.OrdinalIgnoreCase))
            {
                Open(newest, FindLastGameStart(newest));
            }
        }

        if (_path is not null)
        {
            ReadNewLines();
        }

        LastPollDuration = watch.Elapsed;
    }

    /// <summary>Runs <paramref name="read"/> on the current tracker state without racing the reader.</summary>
    public T Read<T>(Func<PowerLogTracker?, T> read)
    {
        lock (_sync)
        {
            return read(_tracker);
        }
    }

    public static string? FindNewestPowerLog(string? logsRoot)
    {
        if (string.IsNullOrEmpty(logsRoot) || !Directory.Exists(logsRoot))
        {
            return null;
        }

        // Folders are named Hearthstone_yyyy_MM_dd_HH_mm_ss, so the name order is the time order.
        return Directory.GetDirectories(logsRoot!, "Hearthstone_*")
            .OrderByDescending(d => Path.GetFileName(d), StringComparer.Ordinal)
            .Select(d => Path.Combine(d, "Power.log"))
            .FirstOrDefault(File.Exists);
    }

    /// <summary>Returns the byte offset of the line that starts the last game, or the file end if there is none.</summary>
    public static long FindLastGameStart(string path)
    {
        var marker = Encoding.ASCII.GetBytes(GameStartMarker);
        using var stream = OpenShared(path);
        var buffer = new byte[ChunkSize + marker.Length];
        var end = stream.Length;

        while (end > 0)
        {
            var start = Math.Max(0, end - ChunkSize);
            var count = (int)(Math.Min(stream.Length, end + marker.Length) - start);
            stream.Seek(start, SeekOrigin.Begin);
            var read = ReadFully(stream, buffer, count);

            for (var i = read - marker.Length; i >= 0; i--)
            {
                if (Matches(buffer, i, marker))
                {
                    // The marker follows the fixed line prefix "D HH:mm:ss.fffffff ".
                    return Math.Max(0, start + i - LinePrefixLength);
                }
            }

            end = start;
        }

        return stream.Length;
    }

    private void Open(string path, long position)
    {
        lock (_sync)
        {
            _path = path;
            _position = position;
            _tracker = new PowerLogTracker(SessionDate(path), Path.GetFileName(Path.GetDirectoryName(path)) ?? string.Empty);
            _tracker.GameFinished += game => GameFinished?.Invoke(game);
        }
    }

    private void ReadNewLines()
    {
        using var stream = OpenShared(_path!);
        if (stream.Length < _position)
        {
            // Hearthstone emptied the file: start over.
            Open(_path!, 0);
        }

        var available = stream.Length - _position;
        if (available <= 0)
        {
            return;
        }

        var bytes = new byte[available];
        stream.Seek(_position, SeekOrigin.Begin);
        var read = ReadFully(stream, bytes, bytes.Length);
        var lastNewline = Array.LastIndexOf(bytes, (byte)'\n', read - 1);
        if (lastNewline < 0)
        {
            return;
        }

        // Only complete lines; a line still being written is read on the next poll.
        var text = Encoding.UTF8.GetString(bytes, 0, lastNewline + 1);
        if (_position == 0 && text.Length > 0 && text[0] == '﻿')
        {
            // Byte order mark at the start of the file
            text = text.Substring(1);
        }

        lock (_sync)
        {
            foreach (var line in text.Split('\n'))
            {
                if (line.Length > 0)
                {
                    _tracker!.ProcessLine(line.TrimEnd('\r'));
                }
            }

            _position += lastNewline + 1;
        }
    }

    private static DateTime SessionDate(string path)
    {
        var folder = Path.GetFileName(Path.GetDirectoryName(path)) ?? string.Empty;
        return folder.Length >= 22 && DateTime.TryParseExact(folder.Substring(12, 10), "yyyy_MM_dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : File.GetCreationTime(path).Date;
    }

    private static FileStream OpenShared(string path) =>
        new(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete);

    private static int ReadFully(Stream stream, byte[] buffer, int count)
    {
        var total = 0;
        while (total < count)
        {
            var read = stream.Read(buffer, total, count - total);
            if (read == 0)
            {
                break;
            }

            total += read;
        }

        return total;
    }

    private static bool Matches(byte[] buffer, int index, byte[] marker)
    {
        for (var j = 0; j < marker.Length; j++)
        {
            if (buffer[index + j] != marker[j])
            {
                return false;
            }
        }

        return true;
    }
}
