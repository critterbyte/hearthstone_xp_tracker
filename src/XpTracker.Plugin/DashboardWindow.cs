using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;
using XpTracker.Core;

namespace XpTracker.Plugin;

/// <summary>
/// XP dashboard opened from the HDT plugins menu: overview, decks, lost XP, game list and charts for a period.
/// </summary>
internal sealed class DashboardWindow : Window
{
    private static readonly Brush Back = Frozen(new SolidColorBrush(Color.FromRgb(0x1E, 0x1F, 0x26)));
    private static readonly Brush Panel = Frozen(new SolidColorBrush(Color.FromRgb(0x2A, 0x2C, 0x36)));
    private static readonly Brush PanelAlt = Frozen(new SolidColorBrush(Color.FromRgb(0x32, 0x34, 0x40)));
    private static readonly Brush Accent = Frozen(new SolidColorBrush(Color.FromRgb(0x5A, 0x9C, 0xE8)));
    private static readonly Brush Muted = Frozen(new SolidColorBrush(Color.FromRgb(0xA8, 0xAD, 0xBD)));

    private readonly GameHistory _history;
    private readonly string _historyPath;
    private readonly Func<string?> _currentSession;
    private readonly Dictionary<DashboardPeriod, ToggleButton> _periodButtons = new();
    private readonly UniformGrid _overview = new() { Columns = 4, VerticalAlignment = VerticalAlignment.Top };
    private readonly UniformGrid _lost = new() { Columns = 3, VerticalAlignment = VerticalAlignment.Top };
    private readonly DataGrid _decks = CreateGrid();
    private readonly DataGrid _games = CreateGrid();
    private readonly CartesianChart _xpChart = CreateChart("XP");
    private readonly CartesianChart _rateChart = CreateChart("XP per hour");
    private readonly bool _checkForUpdates;
    private readonly Action<bool>? _setCheckForUpdates;
    private readonly TextBlock _updateText = new() { Foreground = Brushes.White, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap };
    private readonly Border _updateBar;
    private string _updateUrl = UpdateCheck.ReleasesPage;
    private DashboardPeriod _period = DashboardPeriod.Today;

    public DashboardWindow(
        GameHistory history,
        string historyPath,
        Func<string?> currentSession,
        Func<IReadOnlyList<DeckInfo>>? hdtDecks = null,
        bool checkForUpdates = true,
        Action<bool>? setCheckForUpdates = null)
    {
        _history = history;
        _historyPath = historyPath;
        _currentSession = currentSession;
        _checkForUpdates = checkForUpdates;
        _setCheckForUpdates = setCheckForUpdates;
        _updateBar = UpdateBar();
        UpdateStoredGames(hdtDecks);

        Title = "Hearthstone XP Tracker";
        Width = 960;
        Height = 620;
        Background = Back;
        Foreground = Brushes.White;
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        SetupDeckColumns();
        SetupGameColumns();

        var tabs = new TabControl { Background = Back, BorderBrush = Panel, Margin = new Thickness(10, 0, 10, 10) };
        tabs.Items.Add(Tab("Overview", Padded(_overview)));
        tabs.Items.Add(Tab("Decks", _decks));
        tabs.Items.Add(Tab("Lost XP", Padded(_lost)));
        tabs.Items.Add(Tab("Games", _games));
        tabs.Items.Add(Tab("Charts", Charts()));

        var root = new DockPanel();
        DockPanel.SetDock(_updateBar, Dock.Top);
        root.Children.Add(_updateBar);
        var toolbar = Toolbar();
        DockPanel.SetDock(toolbar, Dock.Top);
        root.Children.Add(toolbar);
        root.Children.Add(tabs);
        Content = root;

        Refresh();
    }

    /// <summary>Shows the hint for a newer release, or hides it when <paramref name="update"/> is null.</summary>
    public void ShowUpdate(ReleaseInfo? update)
    {
        if (update is null)
        {
            _updateBar.Visibility = Visibility.Collapsed;
            return;
        }

        var installed = typeof(DashboardWindow).Assembly.GetName().Version;
        _updateText.Text = $"Version {update.VersionText} is available" + (installed is null ? "." : $" (installed: {UpdateCheck.Display(installed)}).");
        _updateUrl = UpdateCheck.SafeLink(update.Url);
        _updateBar.Visibility = Visibility.Visible;
    }

    private Border UpdateBar()
    {
        var link = new Hyperlink(new Run("Open the download page")) { Foreground = new SolidColorBrush(Color.FromRgb(0x9C, 0xC8, 0xFF)) };
        link.Click += (_, _) => Process.Start(new ProcessStartInfo(_updateUrl) { UseShellExecute = true });
        var linkText = new TextBlock(link) { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0, 0, 0) };

        var panel = new DockPanel();
        DockPanel.SetDock(linkText, Dock.Right);
        panel.Children.Add(linkText);
        panel.Children.Add(_updateText);

        return new Border
        {
            Background = Frozen(new SolidColorBrush(Color.FromRgb(0x1F, 0x3B, 0x5C))),
            BorderBrush = Accent,
            BorderThickness = new Thickness(1),
            CornerRadius = new CornerRadius(4),
            Padding = new Thickness(12, 6, 12, 6),
            Margin = new Thickness(10, 10, 10, 0),
            Child = panel,
            Visibility = Visibility.Collapsed,
        };
    }

    public void Refresh()
    {
        var now = DateTime.Now;
        IReadOnlyList<GameRecord> all;
        try
        {
            all = _history.Load();
        }
        catch (IOException)
        {
            all = Array.Empty<GameRecord>();
        }

        var games = Dashboard.InPeriod(all, _period, now);
        var summary = Dashboard.Summary(games, Dashboard.PeriodStart(_period, now), now, _currentSession());
        var lost = Dashboard.Lost(games, summary);

        _overview.Children.Clear();
        _overview.Children.Add(Tile("XP", summary.Xp.ToString(CultureInfo.InvariantCulture)));
        _overview.Children.Add(Tile("XP time", TimeFormat.HoursMinutesSeconds(summary.XpSeconds)));
        _overview.Children.Add(Tile("Play time incl. mulligan", TimeFormat.HoursMinutesSeconds(summary.PlaySeconds)));
        _overview.Children.Add(Tile("Online time", TimeFormat.HoursMinutesSeconds(summary.OnlineSeconds)));
        _overview.Children.Add(Tile("Games", summary.Games.ToString(CultureInfo.InvariantCulture), $"{summary.Wins} wins, {summary.Losses} losses"));
        _overview.Children.Add(Tile("XP per hour", summary.XpPerHour.ToString("0", CultureInfo.InvariantCulture), "of XP time"));
        _overview.Children.Add(Tile("XP per online hour", summary.XpPerOnlineHour.ToString("0", CultureInfo.InvariantCulture), "including menus and queue"));
        _overview.Children.Add(Tile("Win rate", summary.Games > 0 ? ((double)summary.Wins / summary.Games).ToString("0%", CultureInfo.InvariantCulture) : "-"));

        _lost.Children.Clear();
        _lost.Children.Add(Tile("AFK turns", lost.AfkXp.ToString(CultureInfo.InvariantCulture) + " XP", $"{lost.AfkTurns} turns without action or End Turn"));
        _lost.Children.Add(Tile("Games under 1 minute", lost.ShortGamesXp.ToString(CultureInfo.InvariantCulture) + " XP", $"{lost.ShortGames} games below the 60 s minimum"));
        _lost.Children.Add(Tile("Time outside games", lost.OutsideGamesXp.ToString(CultureInfo.InvariantCulture) + " XP", TimeFormat.HoursMinutesSeconds(lost.OutsideGamesSeconds) + " online but not in a game (menus, queue, breaks)"));

        _decks.ItemsSource = Dashboard.Decks(games).Select(d => new DeckRow(d)).ToList();
        _games.ItemsSource = games.Reverse().Select(g => new GameRow(g)).ToList();

        var days = Dashboard.Days(games);
        var labels = days.Select(d => d.Date.ToString("MM-dd", CultureInfo.InvariantCulture)).ToArray();
        _xpChart.Series = new SeriesCollection { new ColumnSeries { Title = "XP", Values = new ChartValues<int>(days.Select(d => d.Xp)), Fill = Accent } };
        _xpChart.AxisX[0].Labels = labels;
        _rateChart.Series = new SeriesCollection { new LineSeries { Title = "XP per hour", Values = new ChartValues<double>(days.Select(d => Math.Round(d.XpPerHour))), Stroke = Accent, Fill = Brushes.Transparent } };
        _rateChart.AxisX[0].Labels = labels;
    }

    // Brings stored games up to date: XP is calculated again with the current rates (a rate may have been corrected),
    // and games stored without a deck (for example imported from logs) get deck and classes from HDT's statistics.
    private void UpdateStoredGames(Func<IReadOnlyList<DeckInfo>>? hdtDecks)
    {
        try
        {
            var games = _history.Load().ToList();
            var changed = false;
            foreach (var game in games)
            {
                changed |= game.RecalculateXp();
            }

            if (hdtDecks is not null && games.Any(g => g.Deck.Length == 0) && DeckMatcher.FillMissing(games, hdtDecks()) > 0)
            {
                changed = true;
            }

            if (changed)
            {
                _history.ReplaceAll(games);
            }
        }
        catch (IOException)
        {
            // The history stays as it is; the next opening tries again.
        }
    }

    private FrameworkElement Toolbar()
    {
        var bar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10) };
        foreach (var (period, text) in new[] { (DashboardPeriod.Today, "Today"), (DashboardPeriod.Week, "This week"), (DashboardPeriod.Season, "This season"), (DashboardPeriod.All, "All time") })
        {
            var button = new ToggleButton { Content = text, Padding = new Thickness(12, 4, 12, 4), Margin = new Thickness(0, 0, 6, 0), IsChecked = period == _period };
            button.Click += (_, _) => SelectPeriod(period);
            _periodButtons[period] = button;
            bar.Children.Add(button);
        }

        bar.Children.Add(new Border { Width = 18 });
        bar.Children.Add(Action("Refresh", Refresh));
        bar.Children.Add(Action("Export CSV", Export));
        bar.Children.Add(Action("Open data folder", OpenFolder));

        var toolbar = new DockPanel();
        if (_setCheckForUpdates is not null)
        {
            var check = new CheckBox
            {
                Content = "Check for updates when HDT starts",
                IsChecked = _checkForUpdates,
                Foreground = Brushes.White,
                VerticalAlignment = VerticalAlignment.Center,
                Margin = new Thickness(0, 10, 10, 10),
            };
            check.Click += (_, _) => _setCheckForUpdates(check.IsChecked == true);
            DockPanel.SetDock(check, Dock.Right);
            toolbar.Children.Add(check);
        }

        toolbar.Children.Add(bar);
        return toolbar;
    }

    private void SelectPeriod(DashboardPeriod period)
    {
        _period = period;
        foreach (var pair in _periodButtons)
        {
            pair.Value.IsChecked = pair.Key == period;
        }

        Refresh();
    }

    private void Export()
    {
        var dialog = new Microsoft.Win32.SaveFileDialog { FileName = "xp-history.csv", Filter = "CSV files (*.csv)|*.csv" };
        if (dialog.ShowDialog(this) == true && File.Exists(_historyPath))
        {
            File.Copy(_historyPath, dialog.FileName, overwrite: true);
        }
    }

    private void OpenFolder()
    {
        var folder = Path.GetDirectoryName(_historyPath);
        if (folder is not null)
        {
            Directory.CreateDirectory(folder);
            Process.Start("explorer.exe", "\"" + folder + "\"");
        }
    }

    private FrameworkElement Charts()
    {
        var grid = new Grid { Margin = new Thickness(10) };
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        Grid.SetRow(_rateChart, 1);
        grid.Children.Add(_xpChart);
        grid.Children.Add(_rateChart);
        return grid;
    }

    private void SetupDeckColumns()
    {
        _decks.Columns.Add(Column("Deck", nameof(DeckRow.Deck), nameof(DeckRow.Deck), width: 2));
        _decks.Columns.Add(Column("Games", nameof(DeckRow.Games), nameof(DeckRow.Games)));
        _decks.Columns.Add(Column("Win rate", nameof(DeckRow.WinRate), nameof(DeckRow.WinRateValue)));
        _decks.Columns.Add(Column("XP", nameof(DeckRow.Xp), nameof(DeckRow.Xp)));
        _decks.Columns.Add(Column("XP per hour", nameof(DeckRow.XpPerHour), nameof(DeckRow.XpPerHourValue)));
        _decks.Columns.Add(Column("Avg. game", nameof(DeckRow.AverageGame), nameof(DeckRow.AverageGameSeconds)));
    }

    private void SetupGameColumns()
    {
        _games.Columns.Add(Column("Date", nameof(GameRow.Date), nameof(GameRow.StartValue)));
        _games.Columns.Add(Column("Start", nameof(GameRow.Time), nameof(GameRow.StartValue)));
        _games.Columns.Add(Column("Mode", nameof(GameRow.Mode), nameof(GameRow.Mode)));
        _games.Columns.Add(Column("Result", nameof(GameRow.Result), nameof(GameRow.Result)));
        _games.Columns.Add(Column("XP time", nameof(GameRow.XpTime), nameof(GameRow.XpSeconds)));
        _games.Columns.Add(Column("Total time", nameof(GameRow.TotalTime), nameof(GameRow.TotalSeconds)));
        _games.Columns.Add(Column("XP", nameof(GameRow.Xp), nameof(GameRow.XpValue)));
        _games.Columns.Add(Column("AFK", nameof(GameRow.AfkTurns), nameof(GameRow.AfkTurns)));
        _games.Columns.Add(Column("Deck", nameof(GameRow.Deck), nameof(GameRow.Deck), width: 2));
        _games.Columns.Add(Column("Opponent", nameof(GameRow.Opponent), nameof(GameRow.Opponent)));
    }

    private static DataGridTextColumn Column(string header, string display, string sort, double width = 1) => new()
    {
        Header = header,
        Binding = new Binding(display),
        SortMemberPath = sort,
        Width = new DataGridLength(width, DataGridLengthUnitType.Star),
    };

    private static DataGrid CreateGrid()
    {
        var headerStyle = new Style(typeof(DataGridColumnHeader));
        headerStyle.Setters.Add(new Setter(Control.BackgroundProperty, PanelAlt));
        headerStyle.Setters.Add(new Setter(Control.ForegroundProperty, Brushes.White));
        headerStyle.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(6, 4, 6, 4)));
        headerStyle.Setters.Add(new Setter(Control.FontWeightProperty, FontWeights.SemiBold));

        return new DataGrid
        {
            AutoGenerateColumns = false,
            IsReadOnly = true,
            CanUserAddRows = false,
            HeadersVisibility = DataGridHeadersVisibility.Column,
            GridLinesVisibility = DataGridGridLinesVisibility.None,
            Background = Panel,
            RowBackground = Panel,
            AlternatingRowBackground = PanelAlt,
            Foreground = Brushes.White,
            BorderThickness = new Thickness(0),
            ColumnHeaderStyle = headerStyle,
            Margin = new Thickness(10),
        };
    }

    private static CartesianChart CreateChart(string title)
    {
        var chart = new CartesianChart { Foreground = Brushes.White, LegendLocation = LegendLocation.Top, DisableAnimations = true };
        chart.AxisX.Add(new Axis { Foreground = Muted, Separator = new LiveCharts.Wpf.Separator { Step = 1, StrokeThickness = 0 } });
        chart.AxisY.Add(new Axis { Title = title, Foreground = Muted, MinValue = 0 });
        return chart;
    }

    private static TabItem Tab(string header, UIElement content) => new() { Header = header, Content = content, Padding = new Thickness(12, 4, 12, 4) };

    private static FrameworkElement Padded(UIElement content) =>
        new ScrollViewer { Content = new Border { Padding = new Thickness(10), Child = content }, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };

    private static Button Action(string text, Action action)
    {
        var button = new Button { Content = text, Padding = new Thickness(12, 4, 12, 4), Margin = new Thickness(0, 0, 6, 0) };
        button.Click += (_, _) => action();
        return button;
    }

    private static Border Tile(string title, string value, string? note = null)
    {
        var panel = new StackPanel();
        panel.Children.Add(new TextBlock { Text = title, Foreground = Muted, FontSize = 13 });
        panel.Children.Add(new TextBlock { Text = value, Foreground = Brushes.White, FontSize = 26, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 4, 0, 2) });
        if (note is not null)
        {
            panel.Children.Add(new TextBlock { Text = note, Foreground = Muted, FontSize = 11, TextWrapping = TextWrapping.Wrap });
        }

        return new Border { Background = Panel, CornerRadius = new CornerRadius(6), Padding = new Thickness(14), Margin = new Thickness(6), Child = panel };
    }

    private static Brush Frozen(Brush brush)
    {
        brush.Freeze();
        return brush;
    }

    private sealed class DeckRow
    {
        public DeckRow(DeckStats d)
        {
            Deck = d.Deck;
            Games = d.Games;
            WinRateValue = d.WinRate;
            Xp = d.Xp;
            XpPerHourValue = d.XpPerHour;
            AverageGameSeconds = d.AverageGameSeconds;
        }

        public string Deck { get; }

        public int Games { get; }

        public double WinRateValue { get; }

        public string WinRate => WinRateValue.ToString("0%", CultureInfo.InvariantCulture);

        public int Xp { get; }

        public double XpPerHourValue { get; }

        public string XpPerHour => XpPerHourValue.ToString("0", CultureInfo.InvariantCulture);

        public double AverageGameSeconds { get; }

        public string AverageGame => TimeFormat.MinutesSeconds(AverageGameSeconds);
    }

    private sealed class GameRow
    {
        public GameRow(GameRecord g)
        {
            StartValue = g.Start;
            Mode = g.Format.Length > 0 ? g.Mode + " " + g.Format : g.Mode.ToString();
            Result = g.Result.ToString();
            XpSeconds = g.XpSeconds;
            TotalSeconds = g.WithMulliganSeconds;
            XpValue = g.Xp ?? -1;
            Xp = g.Xp?.ToString(CultureInfo.InvariantCulture) ?? "-";
            AfkTurns = g.AfkTurns;
            Deck = g.Deck;
            Opponent = g.OpponentClass.Length > 0 ? char.ToUpperInvariant(g.OpponentClass[0]) + g.OpponentClass.Substring(1).ToLowerInvariant() : string.Empty;
        }

        public DateTime StartValue { get; }

        public string Date => StartValue.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

        public string Time => StartValue.ToString("HH:mm", CultureInfo.InvariantCulture);

        public string Mode { get; }

        public string Result { get; }

        public double XpSeconds { get; }

        public string XpTime => TimeFormat.MinutesSeconds(XpSeconds);

        public double TotalSeconds { get; }

        public string TotalTime => TimeFormat.MinutesSeconds(TotalSeconds);

        public int XpValue { get; }

        public string Xp { get; }

        public int AfkTurns { get; }

        public string Deck { get; }

        public string Opponent { get; }
    }
}
