using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace XpTracker.Core;

/// <summary>
/// Stores finished games in a CSV file, one row per game, without duplicates.
/// </summary>
public sealed class GameHistory
{
    private static readonly string[] Columns =
    {
        "Session", "Start", "End", "Mode", "Format", "Result", "XpSeconds", "PlaySeconds",
        "WithMulliganSeconds", "AfkTurns", "Xp", "Deck", "PlayerClass", "OpponentClass", "EventXp", "XpBonus",
    };

    // Files written before EventXp and XpBonus were added have fewer columns; they are still read.
    private const int OldColumnCount = 14;
    private const int EventXpColumn = 14;
    private const int XpBonusColumn = 15;

    private const string DateFormat = "yyyy-MM-ddTHH:mm:ss.fff";

    private readonly string _path;
    private readonly object _sync = new();
    private HashSet<string>? _keys;

    public GameHistory(string path)
    {
        _path = path;
    }

    /// <summary>
    /// Rows of the file that could not be read at the last <see cref="Load"/> or <see cref="ReplaceAll"/>, for example
    /// after the file was saved by a spreadsheet with other date or number formats. They are skipped, and kept
    /// unchanged at the end of the file when it is rewritten.
    /// </summary>
    public int UnreadableRows { get; private set; }

    /// <summary>Appends the game unless a game with the same session and start is already stored.</summary>
    /// <returns>True if the game was written.</returns>
    public bool Add(GameRecord record)
    {
        lock (_sync)
        {
            _keys ??= new HashSet<string>(Load().Select(r => r.Key));
            if (!_keys.Add(record.Key))
            {
                return false;
            }

            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            var newFile = !File.Exists(_path) || new FileInfo(_path).Length == 0;
            if (!newFile && File.ReadLines(_path, Encoding.UTF8).FirstOrDefault() != string.Join(",", Columns))
            {
                // Older file without the newest columns: rewrite it once with the current header.
                var all = Load().ToList();
                all.Add(record);
                ReplaceAll(all);
                return true;
            }

            using var writer = new StreamWriter(_path, append: true, new UTF8Encoding(false));
            if (newFile)
            {
                writer.WriteLine(string.Join(",", Columns));
            }

            writer.WriteLine(ToCsv(record));
            return true;
        }
    }

    /// <summary>Rewrites the whole file, for example after deck names were filled in.</summary>
    /// <remarks>Writes a temporary file first and swaps it in, so a failure never leaves a half-written history.</remarks>
    public void ReplaceAll(IEnumerable<GameRecord> records)
    {
        lock (_sync)
        {
            var list = records.ToList();
            var directory = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(directory))
            {
                Directory.CreateDirectory(directory);
            }

            // Rows that cannot be read are not in the records; they are written back as they are, so nothing is lost.
            ReadFile(out var unreadable);
            UnreadableRows = unreadable.Count;

            var temp = _path + ".tmp";
            using (var writer = new StreamWriter(temp, append: false, new UTF8Encoding(false)))
            {
                writer.WriteLine(string.Join(",", Columns));
                foreach (var record in list)
                {
                    writer.WriteLine(ToCsv(record));
                }

                foreach (var line in unreadable)
                {
                    writer.WriteLine(line);
                }
            }

            if (File.Exists(_path))
            {
                File.Replace(temp, _path, null);
            }
            else
            {
                File.Move(temp, _path);
            }

            _keys = new HashSet<string>(list.Select(r => r.Key));
        }
    }

    /// <summary>Reads all games. Rows that cannot be read are skipped and counted in <see cref="UnreadableRows"/>.</summary>
    public IReadOnlyList<GameRecord> Load()
    {
        lock (_sync)
        {
            var records = ReadFile(out var unreadable);
            UnreadableRows = unreadable.Count;
            return records;
        }
    }

    private List<GameRecord> ReadFile(out List<string> unreadable)
    {
        var records = new List<GameRecord>();
        unreadable = new List<string>();
        if (!File.Exists(_path))
        {
            return records;
        }

        foreach (var line in File.ReadLines(_path, Encoding.UTF8).Skip(1))
        {
            if (line.Trim().Length == 0)
            {
                continue;
            }

            var fields = SplitCsv(line);
            if (fields.Count >= OldColumnCount && TryFromCsv(fields, out var record))
            {
                records.Add(record);
            }
            else
            {
                unreadable.Add(line);
            }
        }

        return records;
    }

    // A row edited by hand or saved by a spreadsheet can have other date or number formats.
    private static bool TryFromCsv(IReadOnlyList<string> fields, out GameRecord record)
    {
        try
        {
            record = FromCsv(fields);
            return true;
        }
        catch (Exception ex) when (ex is FormatException or OverflowException)
        {
            record = new GameRecord();
            return false;
        }
    }

    private static string ToCsv(GameRecord r) => string.Join(",", new[]
    {
        Quote(r.Session),
        r.Start.ToString(DateFormat, CultureInfo.InvariantCulture),
        r.End.ToString(DateFormat, CultureInfo.InvariantCulture),
        r.Mode.ToString(),
        Quote(r.Format),
        r.Result.ToString(),
        r.XpSeconds.ToString("0.00", CultureInfo.InvariantCulture),
        r.PlaySeconds.ToString("0.00", CultureInfo.InvariantCulture),
        r.WithMulliganSeconds.ToString("0.00", CultureInfo.InvariantCulture),
        r.AfkTurns.ToString(CultureInfo.InvariantCulture),
        r.Xp?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
        Quote(r.Deck),
        Quote(r.PlayerClass),
        Quote(r.OpponentClass),
        r.EventXp?.ToString(CultureInfo.InvariantCulture) ?? string.Empty,
        r.XpBonus.ToString(CultureInfo.InvariantCulture),
    });

    private static GameRecord FromCsv(IReadOnlyList<string> f) => new()
    {
        Session = f[0],
        Start = DateTime.ParseExact(f[1], DateFormat, CultureInfo.InvariantCulture),
        End = DateTime.ParseExact(f[2], DateFormat, CultureInfo.InvariantCulture),
        Mode = Enum.TryParse<GameMode>(f[3], out var mode) ? mode : GameMode.Other,
        Format = f[4],
        Result = Enum.TryParse<GameResult>(f[5], out var result) ? result : GameResult.Unknown,
        XpSeconds = double.Parse(f[6], CultureInfo.InvariantCulture),
        PlaySeconds = double.Parse(f[7], CultureInfo.InvariantCulture),
        WithMulliganSeconds = double.Parse(f[8], CultureInfo.InvariantCulture),
        AfkTurns = int.Parse(f[9], CultureInfo.InvariantCulture),
        Xp = f[10].Length == 0 ? null : int.Parse(f[10], CultureInfo.InvariantCulture),
        Deck = f[11],
        PlayerClass = f[12],
        OpponentClass = f[13],
        EventXp = f.Count > EventXpColumn && f[EventXpColumn].Length > 0 ? int.Parse(f[EventXpColumn], CultureInfo.InvariantCulture) : null,
        XpBonus = f.Count > XpBonusColumn && f[XpBonusColumn].Length > 0 ? int.Parse(f[XpBonusColumn], CultureInfo.InvariantCulture) : 0,
    };

    private static string Quote(string value) =>
        value.IndexOfAny(new[] { ',', '"', '\n', '\r' }) < 0 ? value : "\"" + value.Replace("\"", "\"\"") + "\"";

    private static List<string> SplitCsv(string line)
    {
        var fields = new List<string>();
        var current = new StringBuilder();
        var quoted = false;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (quoted)
            {
                if (c == '"' && i + 1 < line.Length && line[i + 1] == '"')
                {
                    current.Append('"');
                    i++;
                }
                else if (c == '"')
                {
                    quoted = false;
                }
                else
                {
                    current.Append(c);
                }
            }
            else if (c == '"')
            {
                quoted = true;
            }
            else if (c == ',')
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(c);
            }
        }

        fields.Add(current.ToString());
        return fields;
    }
}
