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
        "WithMulliganSeconds", "AfkTurns", "Xp", "Deck", "PlayerClass", "OpponentClass", "EventXp",
    };

    // Files written before EventXp was added have one column less; they are still read.
    private const int OldColumnCount = 14;

    private const string DateFormat = "yyyy-MM-ddTHH:mm:ss.fff";

    private readonly string _path;
    private readonly object _sync = new();
    private HashSet<string>? _keys;

    public GameHistory(string path)
    {
        _path = path;
    }

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

            var temp = _path + ".tmp";
            using (var writer = new StreamWriter(temp, append: false, new UTF8Encoding(false)))
            {
                writer.WriteLine(string.Join(",", Columns));
                foreach (var record in list)
                {
                    writer.WriteLine(ToCsv(record));
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

    public IReadOnlyList<GameRecord> Load()
    {
        if (!File.Exists(_path))
        {
            return Array.Empty<GameRecord>();
        }

        var records = new List<GameRecord>();
        foreach (var line in File.ReadLines(_path, Encoding.UTF8).Skip(1))
        {
            var fields = SplitCsv(line);
            if (fields.Count >= OldColumnCount)
            {
                records.Add(FromCsv(fields));
            }
        }

        return records;
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
        EventXp = f.Count > OldColumnCount && f[OldColumnCount].Length > 0 ? int.Parse(f[OldColumnCount], CultureInfo.InvariantCulture) : null,
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
