// Replays Power.log files through the calculation engine and prints one line per finished game.
//
// Usage:
//   XpTracker.LogCheck <Power.log | archive.zip | folder> [...]   replay files, print games as tab-separated values
//   XpTracker.LogCheck --tail <Hearthstone Logs folder>           measure how long the plugin's first and next reads take
using System.Diagnostics;
using System.Globalization;
using System.IO.Compression;
using XpTracker.Core;

if (args.Length == 2 && args[0] == "--tail")
{
    MeasureTail(args[1]);
    return 0;
}

if (args.Length == 0)
{
    Console.Error.WriteLine("Usage: XpTracker.LogCheck <Power.log | archive.zip | folder> [...] | --tail <Logs folder>");
    return 1;
}

Console.WriteLine("Date\tFirstTurn\tMode\tResult\tXpSeconds\tPlaySeconds\tWithMulliganSeconds\tAfkTurns\tDamage");
foreach (var source in args.SelectMany(Expand).OrderBy(Path.GetFileName, StringComparer.Ordinal))
{
    var watch = Stopwatch.StartNew();
    var tracker = new PowerLogTracker(SessionDate(source));
    tracker.GameFinished += Print;
    using (var reader = OpenLog(source))
    {
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            tracker.ProcessLine(line);
        }
    }

    Console.Error.WriteLine($"{Path.GetFileName(source)}: {tracker.LinesProcessed} lines in {watch.ElapsedMilliseconds} ms");
}

return 0;

static void Print(FinishedGame game)
{
    var s = game.Clock.GetSnapshot(DateTime.MaxValue);
    Console.WriteLine(string.Join('\t',
        game.FirstTurn.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
        game.FirstTurn.ToString("HH:mm:ss", CultureInfo.InvariantCulture),
        game.Mode,
        game.Clock.Result,
        s.XpSeconds.ToString("0.00", CultureInfo.InvariantCulture),
        s.PlaySeconds.ToString("0.00", CultureInfo.InvariantCulture),
        s.WithMulliganSeconds.ToString("0.00", CultureInfo.InvariantCulture),
        game.Clock.AfkTurns,
        game.DamageDealt));
}

static IEnumerable<string> Expand(string path) =>
    Directory.Exists(path)
        ? Directory.EnumerateFiles(path, "*.zip").Concat(Directory.EnumerateFiles(path, "Power.log", SearchOption.AllDirectories))
        : new[] { path };

static StreamReader OpenLog(string path)
{
    if (!path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
    {
        return new StreamReader(new FileStream(path, FileMode.Open, FileAccess.Read, FileShare.ReadWrite | FileShare.Delete));
    }

    var zip = ZipFile.OpenRead(path);
    var entry = zip.GetEntry("Power.log") ?? throw new InvalidDataException($"{path} has no Power.log");
    return new StreamReader(entry.Open());
}

// Log folders and archives are named Hearthstone_yyyy_MM_dd_HH_mm_ss.
static DateTime SessionDate(string path)
{
    var name = path.EndsWith(".zip", StringComparison.OrdinalIgnoreCase)
        ? Path.GetFileNameWithoutExtension(path)
        : Path.GetFileName(Path.GetDirectoryName(path)) ?? string.Empty;
    return name.Length >= 22 && DateTime.TryParseExact(name.Substring(12, 10), "yyyy_MM_dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
        ? date
        : File.GetCreationTime(path).Date;
}

static void MeasureTail(string logsRoot)
{
    var newest = PowerLogTail.FindNewestPowerLog(logsRoot);
    if (newest is null)
    {
        Console.Error.WriteLine("No Power.log found.");
        return;
    }

    var size = new FileInfo(newest).Length;
    var watch = Stopwatch.StartNew();
    var start = PowerLogTail.FindLastGameStart(newest);
    var seek = watch.Elapsed;

    var tail = new PowerLogTail(() => logsRoot);
    tail.Poll();
    var first = tail.LastPollDuration;
    var polls = new List<TimeSpan>();
    for (var i = 0; i < 20; i++)
    {
        tail.Poll();
        polls.Add(tail.LastPollDuration);
    }

    Console.WriteLine($"Log size: {size / 1024.0 / 1024.0:0.00} MB, current game starts {(size - start) / 1024.0:0} KB before the end");
    Console.WriteLine($"Find game start: {seek.TotalMilliseconds:0.0} ms");
    Console.WriteLine($"First poll (finds the file, reads the current game): {first.TotalMilliseconds:0.0} ms, {tail.Read(t => t?.LinesProcessed ?? 0)} lines");
    Console.WriteLine($"Next polls (no new data): avg {polls.Average(p => p.TotalMilliseconds):0.00} ms, max {polls.Max(p => p.TotalMilliseconds):0.00} ms");
}
