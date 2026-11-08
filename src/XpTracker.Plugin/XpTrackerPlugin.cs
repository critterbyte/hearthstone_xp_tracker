using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker;
using Hearthstone_Deck_Tracker.Plugins;
using XpTracker.Core;
using HdtCore = Hearthstone_Deck_Tracker.API.Core;

namespace XpTracker.Plugin;

/// <summary>
/// Entry point loaded by Hearthstone Deck Tracker.
/// </summary>
/// <remarks>
/// Game times come from Hearthstone's Power.log (read on a background thread), not from HDT's game state,
/// so the plugin has the full game even when HDT is started in the middle of it.
/// </remarks>
public class XpTrackerPlugin : IPlugin
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(250);

    // Deck and classes are taken from HDT only for games that just ended; replays of older games leave them empty.
    private static readonly TimeSpan RecentGame = TimeSpan.FromMinutes(2);

    // Kept outside the plugin folder so that installing a new plugin version does not touch it.
    private static readonly string HistoryPath = Path.Combine(Config.AppDataPath, "XpTracker", "hs_games.csv");

    private readonly GameHistory _history = new(HistoryPath);
    private XpOverlay? _overlay;
    private PowerLogTail? _tail;
    private CancellationTokenSource? _stop;
    private string _shownKey = string.Empty;
    private bool _enabled;

    public string Name => "Hearthstone XP Tracker";

    public string Description => "Shows the reward track XP of the current game and the XP time it is based on.";

    public string ButtonText => "Settings";

    public string Author => "critterbyte";

    public Version Version => typeof(XpTrackerPlugin).Assembly.GetName().Version;

    // HDT documents null as "no menu entry", but declares the property as non-nullable.
    public MenuItem MenuItem => null!;

    public void OnLoad()
    {
        _overlay = new XpOverlay();
        HdtCore.OverlayCanvas.Children.Add(_overlay);
        _tail = new PowerLogTail(FindLogsFolder);
        _tail.GameFinished += OnGameFinished;
        _stop = new CancellationTokenSource();
        var token = _stop.Token;
        var tail = _tail;
        Task.Run(() => PollLoop(tail, token), token);
        _enabled = true;
    }

    public void OnUnload()
    {
        _enabled = false;
        _stop?.Cancel();
        _stop = null;
        if (_tail is not null)
        {
            _tail.GameFinished -= OnGameFinished;
            _tail = null;
        }
        if (_overlay is not null)
        {
            HdtCore.OverlayCanvas.Children.Remove(_overlay);
            _overlay = null;
        }
    }

    public void OnButtonPress()
    {
    }

    // Called by HDT about every 100 ms on the UI thread; only reads the state prepared by the background thread.
    public void OnUpdate()
    {
        if (!_enabled || _overlay is null || _tail is null)
        {
            return;
        }

        var now = DateTime.Now;
        var lines = _tail.Read(t => XpDisplayText.For(t?.Clock, t?.Mode ?? GameMode.Other, now));
        // The text changes at most once per second; only touch the visual tree when it does.
        var key = string.Join("\n", lines.Select(l => l + (l.IsXp ? " xp" : string.Empty)));
        if (key != _shownKey)
        {
            _shownKey = key;
            _overlay.Show(lines);
        }

        var canvas = HdtCore.OverlayCanvas;
        _overlay.Place(canvas.ActualWidth, canvas.ActualHeight);
    }

    // Raised on the polling thread; HDT's game state is read on the UI thread.
    private void OnGameFinished(FinishedGame game)
    {
        System.Windows.Application.Current?.Dispatcher.BeginInvoke(new Action(() => SaveGame(game)));
    }

    private void SaveGame(FinishedGame game)
    {
        var record = GameRecord.From(game);
        if (DateTime.Now - game.End < RecentGame)
        {
            record.Deck = DeckList.Instance.ActiveDeck?.Name ?? string.Empty;
            // OriginalClass: hero class at the start of the game (CurrentClass changes with hero cards).
            record.PlayerClass = HdtCore.Game.Player?.OriginalClass ?? string.Empty;
            record.OpponentClass = HdtCore.Game.Opponent?.OriginalClass ?? string.Empty;
        }

        try
        {
            _history.Add(record);
        }
        catch (IOException)
        {
            // The file may be open in another program; the game is missing from the history then.
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private static async Task PollLoop(PowerLogTail tail, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                tail.Poll();
            }
            catch (IOException)
            {
                // The log may be locked or replaced for a moment; try again on the next poll.
            }
            catch (UnauthorizedAccessException)
            {
            }

            try
            {
                await Task.Delay(PollInterval, token).ConfigureAwait(false);
            }
            catch (TaskCanceledException)
            {
                return;
            }
        }
    }

    // HDT knows the Hearthstone folder (detected or set in its options); fall back to the running game.
    private static string? FindLogsFolder()
    {
        var config = Config.Instance;
        var fromConfig = Path.Combine(config.HearthstoneDirectory ?? string.Empty, config.HearthstoneLogsDirectoryName ?? "Logs");
        if (Directory.Exists(fromConfig))
        {
            return fromConfig;
        }

        try
        {
            var game = Process.GetProcessesByName("Hearthstone").FirstOrDefault();
            var exe = game?.MainModule?.FileName;
            return exe is null ? null : Path.Combine(Path.GetDirectoryName(exe)!, "Logs");
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception)
        {
            return null;
        }
    }
}
