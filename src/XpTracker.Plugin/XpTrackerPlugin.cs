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
    private static readonly string SettingsPath = Path.Combine(Config.AppDataPath, "XpTracker", "settings.ini");

    private readonly GameHistory _history = new(HistoryPath);
    private XpTrackerSettings _settings = new();
    private ReleaseInfo? _update;
    private MenuItem? _menuItem;
    private DashboardWindow? _dashboard;
    private XpOverlay? _overlay;
    private PowerLogTail? _tail;
    private CancellationTokenSource? _stop;
    private string _shownKey = string.Empty;
    private bool _enabled;

    public string Name => "Hearthstone XP Tracker";

    public string Description => "Shows the reward track XP of the current game and the XP time it is based on.";

    public string ButtonText => "XP dashboard";

    public string Author => "critterbyte";

    public Version Version => typeof(XpTrackerPlugin).Assembly.GetName().Version;

    // Entry in HDT's Plugins menu; created on first access, which happens on the UI thread.
    public MenuItem MenuItem
    {
        get
        {
            if (_menuItem is null)
            {
                _menuItem = new MenuItem { Header = "XP dashboard" };
                _menuItem.Click += (_, _) => OpenDashboard();
            }

            return _menuItem;
        }
    }

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

        _settings = XpTrackerSettings.Load(SettingsPath);
        if (_settings.CheckForUpdates)
        {
            StartUpdateCheck();
        }
    }

    public void OnUnload()
    {
        _enabled = false;
        _update = null;
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

        _dashboard?.Close();
        _dashboard = null;
    }

    public void OnButtonPress() => OpenDashboard();

    // Called by HDT about every 100 ms on the UI thread; only reads the state prepared by the background thread.
    public void OnUpdate()
    {
        if (!_enabled || _overlay is null || _tail is null)
        {
            return;
        }

        var now = DateTime.Now;
        var update = _update?.VersionText;
        var lines = _tail.Read(t => XpDisplayText.WithUpdate(
            XpDisplayText.For(t?.Clock, t?.Mode ?? GameMode.Other, now, _settings.EventActive),
            gameRunning: t?.Clock is { HasStarted: true, IsOver: false },
            update));
        // The text changes at most once per second; only touch the visual tree when it does.
        var key = string.Join("\n", lines.Select(l => l + (l.IsXp ? " xp" : l.IsEvent ? " event" : string.Empty)));
        if (key != _shownKey)
        {
            _shownKey = key;
            _overlay.Show(lines);
        }

        var canvas = HdtCore.OverlayCanvas;
        _overlay.Place(canvas.ActualWidth, canvas.ActualHeight);
    }

    private void OpenDashboard()
    {
        if (_dashboard is null)
        {
            _dashboard = new DashboardWindow(_history, HistoryPath, () => _tail?.CurrentSession, IsHearthstoneRunning, HdtDecks.All, _settings.CheckForUpdates, SetCheckForUpdates, _settings.EventActive, SetEventActive);
            _dashboard.ShowUpdate(_update);
            _dashboard.Closed += (_, _) => _dashboard = null;
            _dashboard.Show();
        }
        else
        {
            _dashboard.Refresh();
            _dashboard.Activate();
        }
    }

    // One request per HDT start (or when the check is switched on again); runs in the background.
    private void StartUpdateCheck()
    {
        if (_stop is null)
        {
            return;
        }

        var token = _stop.Token;
        var installed = Version;
        Task.Run(
            async () =>
            {
                var update = await UpdateChecker.FindNewerAsync(installed, token).ConfigureAwait(false);
                if (update is not null && !token.IsCancellationRequested)
                {
                    System.Windows.Application.Current?.Dispatcher.BeginInvoke(new Action(() => ShowUpdate(update)));
                }
            },
            token);
    }

    private void ShowUpdate(ReleaseInfo update)
    {
        if (!_enabled || !_settings.CheckForUpdates)
        {
            return;
        }

        _update = update;
        _dashboard?.ShowUpdate(update);
    }

    // Called from the dashboard check box.
    // Called from the dashboard check box "Event running": shows the event XP row and stores event XP with new games.
    private void SetEventActive(bool active)
    {
        _settings.EventActive = active;
        SaveSettings();
        _shownKey = string.Empty;
    }

    private void SaveSettings()
    {
        try
        {
            _settings.Save(SettingsPath);
        }
        catch (IOException)
        {
            // The setting applies until HDT is closed; the next start reads the old file.
        }
        catch (UnauthorizedAccessException)
        {
        }
    }

    private void SetCheckForUpdates(bool check)
    {
        _settings.CheckForUpdates = check;
        try
        {
            _settings.Save(SettingsPath);
        }
        catch (IOException)
        {
            // The setting applies until HDT is closed; the next start reads the old file.
        }
        catch (UnauthorizedAccessException)
        {
        }

        if (check)
        {
            StartUpdateCheck();
        }
        else
        {
            _update = null;
            _dashboard?.ShowUpdate(null);
        }
    }

    // Raised on the polling thread; HDT's game state is read on the UI thread.
    private void OnGameFinished(FinishedGame game)
    {
        System.Windows.Application.Current?.Dispatcher.BeginInvoke(new Action(() => SaveGame(game)));
    }

    private void SaveGame(FinishedGame game)
    {
        var record = GameRecord.From(game, _settings.EventActive);
        if (DateTime.Now - game.End < RecentGame)
        {
            record.Deck = HdtDecks.Current();
            // OriginalClass: hero class at the start of the game (CurrentClass changes with hero cards).
            record.PlayerClass = HdtCore.Game.Player?.OriginalClass ?? string.Empty;
            record.OpponentClass = HdtCore.Game.Opponent?.OriginalClass ?? string.Empty;
        }

        if (record.Deck.Length == 0)
        {
            // Older game read again from the log, or no deck selected: look it up in HDT's statistics.
            DeckMatcher.FillMissing(new[] { record }, HdtDecks.All());
        }

        try
        {
            if (_history.Add(record))
            {
                _dashboard?.Refresh();
            }
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

    private static bool IsHearthstoneRunning()
    {
        var processes = Process.GetProcessesByName("Hearthstone");
        foreach (var process in processes)
        {
            process.Dispose();
        }

        return processes.Length > 0;
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
