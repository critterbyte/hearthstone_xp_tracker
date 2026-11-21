using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;
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

    // Where the former Hearthstone Deck Tracker plugin kept its history and settings; only read.
    private static readonly string PluginFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "HearthstoneDeckTracker", "XpTracker");

    // Read once per app start; the installation folder does not change while the app runs.
    private static readonly Lazy<string?> InstallLocation = new(ReadInstallLocation);

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
    private LogConfigResult _logConfig = LogConfigResult.AlreadyOn;

    // Set when the app switched the game log on while Hearthstone was running: the game reads log.config only when it
    // starts, so it writes the log after its next start.
    private DateTime? _restartNeededSince;

    public void Start()
    {
        ImportPluginData();
        _settings = XpTrackerSettings.Load(SettingsPath);
        Localizer.SetLanguage(_settings.Language);
        SwitchOnGameLog();
        StartupIntegration.RepairMovedFolder();

        _tail = new PowerLogTail(FindLogsFolder, now: () => DateTime.Now);
        _tail.GameFinished += OnGameFinished;
        _stop = new CancellationTokenSource();
        var token = _stop.Token;
        var tail = _tail;
        Task.Run(() => PollLoop(tail, token), token);

        _tray = new TrayIcon(OpenDashboard, Exit);
        if (_logConfig == LogConfigResult.Failed)
        {
            _tray.ShowNotice(Localizer.Format("log.failed", LogConfig.DefaultPath), warning: true);
        }
        else if (_restartNeededSince is not null)
        {
            _tray.ShowNotice(Localizer.Text("log.restart"));
        }

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
            _dashboard = new DashboardWindow(_history, HistoryPath, () => _tail?.CurrentSession, IsHearthstoneRunning, GameLogStatus, _settings.CheckForUpdates, SetCheckForUpdates, _settings.EventActive, SetEventActive, _settings.TavernPass, _settings.TavernPassLevel, SetTavernPass, _settings.ShowDamage, SetShowDamage, _settings.Language, SetLanguage, _settings.OverlayPosition, SetOverlayPosition, StartupIntegration.AutostartOn(), StartupIntegration.SetAutostart, StartupIntegration.ShortcutExists(), StartupIntegration.SetShortcut, !_settings.StartMenuAsked, OnStartMenuAnswered);
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

    // Takes over what the HDT plugin recorded: its settings once, when the app has none yet, and at every start the
    // games the app does not have yet, so games played with the plugin while the app was not running are not lost.
    private void ImportPluginData()
    {
        try
        {
            var oldSettings = Path.Combine(PluginFolder, "settings.ini");
            if (!File.Exists(SettingsPath) && File.Exists(oldSettings))
            {
                Directory.CreateDirectory(DataFolder);
                File.Copy(oldSettings, SettingsPath);
                AppLog.Info("Took over the settings of the HDT plugin.");
            }

            var games = _history.ImportFrom(Path.Combine(PluginFolder, "hs_games.csv"));
            if (games > 0)
            {
                AppLog.Info("Took over " + games + " games from the HDT plugin.");
            }
        }
        catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
        {
            // For example the plugin is just writing its file; the next start tries again.
            AppLog.Error(ex);
        }
    }

    // Makes sure Hearthstone writes the Power.log; see LogConfig.
    private void SwitchOnGameLog()
    {
        _logConfig = LogConfig.EnsureFile(LogConfig.DefaultPath, out var error);
        switch (_logConfig)
        {
            case LogConfigResult.Updated:
                AppLog.Info("Switched on the [Power] section in " + LogConfig.DefaultPath);
                if (IsHearthstoneRunning())
                {
                    _restartNeededSince = DateTime.Now;
                }

                break;
            case LogConfigResult.Failed:
                AppLog.Info("Could not switch on the game log in " + LogConfig.DefaultPath + ": " + error);
                break;
        }
    }

    // Status line in the dashboard: the text, and whether it needs the user's attention.
    private (string Text, bool Problem) GameLogStatus()
    {
        if (_logConfig == LogConfigResult.Failed)
        {
            return (Localizer.Format("log.failed", LogConfig.DefaultPath), true);
        }

        if (_restartNeededSince is { } since)
        {
            var started = GameStartTime();
            if (started is not null && started < since)
            {
                return (Localizer.Text("log.restart"), true);
            }

            _restartNeededSince = null;
        }

        return (Localizer.Text("log.on"), false);
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

    // The one-time question in the dashboard "Add the tracker to the Start menu?" was answered, either way.
    private void OnStartMenuAnswered()
    {
        _settings.StartMenuAsked = true;
        SaveSettings();
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

    // When the running Hearthstone was started, or null when it is not running.
    private static DateTime? GameStartTime()
    {
        var processes = Process.GetProcessesByName("Hearthstone");
        try
        {
            return processes.Length == 0 ? null : processes.Min(p => p.StartTime);
        }
        catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception)
        {
            return null;
        }
        finally
        {
            foreach (var process in processes)
            {
                process.Dispose();
            }
        }
    }

    // The running game, then the folder from the list of installed programs, then the default folder.
    private static string? FindLogsFolder() =>
        HearthstoneFolder.FindLogsFolder(RunningGameExe(), InstallLocation.Value, Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86));

    private static string? RunningGameExe()
    {
        var processes = Process.GetProcessesByName("Hearthstone");
        try
        {
            foreach (var process in processes)
            {
                try
                {
                    if (process.MainModule?.FileName is { } file)
                    {
                        return file;
                    }
                }
                catch (Exception ex) when (ex is InvalidOperationException or System.ComponentModel.Win32Exception)
                {
                    // The process is just ending, or Windows does not tell its path.
                }
            }

            return null;
        }
        finally
        {
            foreach (var process in processes)
            {
                process.Dispose();
            }
        }
    }

    // Battle.net registers the game as "Hearthstone" in the uninstall list, with its installation folder.
    private static string? ReadInstallLocation()
    {
        foreach (var view in new[] { RegistryView.Registry32, RegistryView.Registry64 })
        {
            try
            {
                using var root = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view);
                using var key = root.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall\Hearthstone");
                if (key?.GetValue("InstallLocation") is string location && location.Length > 0)
                {
                    return location;
                }
            }
            catch (Exception ex) when (ex is System.Security.SecurityException or UnauthorizedAccessException or IOException)
            {
            }
        }

        return null;
    }
}
