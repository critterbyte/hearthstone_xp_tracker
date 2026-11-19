using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using XpTracker.Core;

namespace XpTracker.App;

/// <summary>
/// The running tracker: reads Hearthstone's Power.log on a background thread, stores finished games
/// and shows the overlay and the dashboard. Created and used on the UI thread.
/// </summary>
/// <remarks>
/// Game times come from Hearthstone's Power.log, so a game that is already running when the app starts is still read
/// in full.
/// </remarks>
internal sealed class XpTrackerApp
{
    private static readonly TimeSpan PollInterval = TimeSpan.FromMilliseconds(250);

    // The overlay text changes at most once per second; the window follows the game window ten times per second.
    private static readonly TimeSpan OverlayInterval = TimeSpan.FromMilliseconds(100);

    /// <summary>The app's own data folder: game history, settings and the error log.</summary>
    public static readonly string DataFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HearthstoneXpTracker");

    private static readonly string HistoryPath = Path.Combine(DataFolder, "hs_games.csv");
    private static readonly string SettingsPath = Path.Combine(DataFolder, "settings.ini");

    private readonly GameHistory _history = new(HistoryPath);
    private XpTrackerSettings _settings = new();
    private ReleaseInfo? _update;
    private DashboardWindow? _dashboard;
    private PowerLogTail? _tail;
    private TrayIcon? _tray;
    private CancellationTokenSource? _stop;
    private OverlayWindow? _overlayWindow;
    private DispatcherTimer? _overlayTimer;
    private string _shownKey = string.Empty;
    private string? _lastOverlayError;

    public void Start()
    {
        _settings = XpTrackerSettings.Load(SettingsPath);
        Localizer.SetLanguage(_settings.Language);

        _tail = new PowerLogTail(FindLogsFolder, now: () => DateTime.Now);
        _tail.GameFinished += OnGameFinished;
        _stop = new CancellationTokenSource();
        var token = _stop.Token;
        var tail = _tail;
        Task.Run(() => PollLoop(tail, token), token);

        _tray = new TrayIcon(OpenDashboard, Exit);

        _overlayWindow = new OverlayWindow(new XpOverlay { Position = _settings.OverlayPosition });
        _overlayTimer = new DispatcherTimer { Interval = OverlayInterval };
        _overlayTimer.Tick += (_, _) => UpdateOverlay();
        _overlayTimer.Start();

        if (_settings.CheckForUpdates)
        {
            StartUpdateCheck();
        }
    }

    public void OpenDashboard()
    {
        if (_tail is null)
        {
            return;
        }

        if (_dashboard is null)
        {
            _dashboard = new DashboardWindow(_history, HistoryPath, () => _tail?.CurrentSession, IsHearthstoneRunning, _settings.CheckForUpdates, SetCheckForUpdates, _settings.EventActive, SetEventActive, _settings.TavernPass, _settings.TavernPassLevel, SetTavernPass, _settings.ShowDamage, SetShowDamage, _settings.Language, SetLanguage, _settings.OverlayPosition, SetOverlayPosition);
            _dashboard.ShowUpdate(_update);
            _dashboard.Closed += (_, _) => _dashboard = null;
            _dashboard.Show();
        }
        else
        {
            _dashboard.Refresh();
            if (_dashboard.WindowState == WindowState.Minimized)
            {
                _dashboard.WindowState = WindowState.Normal;
            }
        }

        _dashboard.Activate();
    }

    private void Exit()
    {
        _overlayTimer?.Stop();
        _overlayTimer = null;
        _overlayWindow?.Close();
        _overlayWindow = null;
        _stop?.Cancel();
        _stop = null;
        if (_tail is not null)
        {
            _tail.GameFinished -= OnGameFinished;
            _tail = null;
        }

        _dashboard?.Close();
        _dashboard = null;
        _tray?.Dispose();
        _tray = null;
        Application.Current.Shutdown();
    }

    // Shows the current or last game on the overlay and keeps the overlay over the game window.
    private void UpdateOverlay()
    {
        if (_overlayWindow is not { } window || _tail is not { } tail)
        {
            return;
        }

        try
        {
            var now = DateTime.Now;
            var update = _update?.VersionText;
            var showDamage = _settings.ShowDamage;
            var lines = tail.Read(t => XpDisplayText.WithUpdate(
                XpDisplayText.For(
                    t?.Clock,
                    t?.Mode ?? GameMode.Other,
                    now,
                    _settings.EventActive,
                    showDamage ? t?.DamageDealt : null,
                    _settings.XpBonusPercent,
                    showDamage ? t?.HeroDamageDealt : null),
                gameRunning: t?.IsGameRunning ?? false,
                update));

            // Only touch the visual tree when the text changes.
            var key = string.Join("\n", lines.Select(l => l + (l.IsXp ? " xp" : l.IsEvent ? " event" : string.Empty)));
            if (key != _shownKey)
            {
                _shownKey = key;
                window.Overlay.Show(lines);
            }

            window.Follow();
            var area = window.GameArea;
            window.Overlay.Place(area.Width, area.Height);
        }
        catch (Exception ex)
        {
            // A failing update must not stop the timer; the next tick tries again. Each kind of error is logged once.
            var text = ex.GetType().FullName + ": " + ex.Message;
            if (text != _lastOverlayError)
            {
                _lastOverlayError = text;
                AppLog.Error(ex);
            }
        }
    }

    // One request per app start (or when the check is switched on again); runs in the background.
    private void StartUpdateCheck()
    {
        if (_stop is null)
        {
            return;
        }

        var token = _stop.Token;
        var installed = UpdateCheck.Normalize(typeof(XpTrackerApp).Assembly.GetName().Version);
        Task.Run(
            async () =>
            {
                var update = await UpdateChecker.FindNewerAsync(installed, token).ConfigureAwait(false);
                if (update is not null && !token.IsCancellationRequested)
                {
                    Application.Current?.Dispatcher.BeginInvoke(new Action(() => ShowUpdate(update)));
                }
            },
            token);
    }

    private void ShowUpdate(ReleaseInfo update)
    {
        if (_stop is null || !_settings.CheckForUpdates)
        {
            return;
        }

        _update = update;
        _dashboard?.ShowUpdate(update);
    }

    // Called from the dashboard check box "Event running": shows the event XP row and stores event XP with new games.
    private void SetEventActive(bool active)
    {
        _settings.EventActive = active;
        SaveSettings();
        _shownKey = string.Empty;
    }

    // Called from the dashboard's language selection. The menu changes at once; the dashboard is built again in the
    // new language at the same place, after the selection handler has returned.
    private void SetLanguage(string language)
    {
        _settings.Language = Localizer.Normalize(language);
        SaveSettings();
        Localizer.SetLanguage(_settings.Language);
        _tray?.UpdateTexts();
        _shownKey = string.Empty;

        if (_dashboard is { } old)
        {
            Application.Current?.Dispatcher.BeginInvoke(new Action(() =>
            {
                var bounds = (old.Left, old.Top, old.Width, old.Height, old.WindowState);
                old.Close();
                OpenDashboard();
                if (_dashboard is { } fresh)
                {
                    fresh.WindowStartupLocation = WindowStartupLocation.Manual;
                    (fresh.Left, fresh.Top, fresh.Width, fresh.Height) = (bounds.Left, bounds.Top, bounds.Width, bounds.Height);
                    fresh.WindowState = bounds.WindowState;
                }
            }));
        }
    }

    // Called from the dashboard's overlay position selection; the overlay moves with the next update.
    private void SetOverlayPosition(OverlayPosition position)
    {
        _settings.OverlayPosition = position;
        SaveSettings();
        if (_overlayWindow is not null)
        {
            _overlayWindow.Overlay.Position = position;
        }
    }

    // Called from the dashboard check box "Show damage on the overlay": shows or hides the two damage rows at once.
    private void SetShowDamage(bool show)
    {
        _settings.ShowDamage = show;
        SaveSettings();
        _shownKey = string.Empty;
    }

    // Called from the dashboard: "Tavern Pass" check box and the rewards track level. Changes the overlay at once and the XP
    // stored with new games; games already stored keep the boost they were played with.
    private void SetTavernPass(bool owned, int level)
    {
        _settings.TavernPass = owned;
        _settings.TavernPassLevel = TavernPassBonus.ClampLevel(level);
        SaveSettings();
        _shownKey = string.Empty;
    }

    private void SetCheckForUpdates(bool check)
    {
        _settings.CheckForUpdates = check;
        SaveSettings();

        if (check)
        {
            StartUpdateCheck();
        }
        else
        {
            _update = null;
            _dashboard?.ShowUpdate(null);
            _shownKey = string.Empty;
        }
    }

    private void SaveSettings()
    {
        try
        {
            _settings.Save(SettingsPath);
        }
        catch (IOException ex)
        {
            // The setting applies until the app is closed; the next start reads the old file.
            AppLog.Error(ex);
        }
        catch (UnauthorizedAccessException ex)
        {
            AppLog.Error(ex);
        }
    }

    // Raised on the polling thread; the history is written on the UI thread.
    private void OnGameFinished(FinishedGame game)
    {
        Application.Current?.Dispatcher.BeginInvoke(new Action(() => SaveGame(game)));
    }

    private void SaveGame(FinishedGame game)
    {
        var record = GameRecord.From(game, _settings.EventActive, _settings.XpBonusPercent);

        // If the file is open in another program, the game is kept in memory and written with the next game
        // or the next refresh of the dashboard.
        if (_history.TryAdd(record))
        {
            _dashboard?.Refresh();
        }
    }

    private static async Task PollLoop(PowerLogTail tail, CancellationToken token)
    {
        string? lastError = null;
        while (!token.IsCancellationRequested)
        {
            try
            {
                tail.Poll();
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                // The log may be locked or replaced for a moment; try again on the next poll.
            }
            catch (Exception ex)
            {
                // Anything else must not end the reader. Each kind of error is logged once.
                var text = ex.GetType().FullName + ": " + ex.Message;
                if (text != lastError)
                {
                    lastError = text;
                    AppLog.Error(ex);
                }
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

    // The running game tells its folder; otherwise the default installation folder.
    private static string? FindLogsFolder()
    {
        try
        {
            var game = Process.GetProcessesByName("Hearthstone").FirstOrDefault();
            var exe = game?.MainModule?.FileName;
            if (exe is not null)
            {
                return Path.Combine(Path.GetDirectoryName(exe)!, "Logs");
            }
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception)
        {
        }

        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        var fallback = Path.Combine(programFiles, "Hearthstone", "Logs");
        return Directory.Exists(fallback) ? fallback : null;
    }
}
