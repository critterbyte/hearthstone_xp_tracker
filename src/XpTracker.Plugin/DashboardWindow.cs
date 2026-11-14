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
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Effects;
using LiveCharts;
using LiveCharts.Defaults;
using LiveCharts.Wpf;
using XpTracker.Core;

namespace XpTracker.Plugin;

/// <summary>
/// XP dashboard opened from the HDT plugins menu: overview, decks, lost XP, game list and the XP speed curve for a period.
/// </summary>
/// <remarks>
/// Colors in the style of the Hearthstone client (dark wood, gold, parchment, mana blue); no game artwork or fonts are used.
/// </remarks>
internal sealed class DashboardWindow : Window
{
    private static readonly Brush Back = Solid(0x22, 0x17, 0x0F);
    private static readonly Brush Panel = Solid(0x3A, 0x27, 0x16);
    private static readonly Brush PanelAlt = Solid(0x4A, 0x32, 0x1D);
    private static readonly Brush Gold = Solid(0xC8, 0xA0, 0x50);
    private static readonly Brush GoldBright = Solid(0xF0, 0xC7, 0x5E);
    private static readonly Brush Parchment = Solid(0xF4, 0xE4, 0xC1);
    private static readonly Brush Muted = Solid(0xC2, 0xA5, 0x7E);
    private static readonly Brush Mana = Solid(0x4A, 0x9B, 0xE8);

    // Slightly lighter mana blue for large numbers, so they stay easy to read on the dark wood.
    private static readonly Brush ManaBright = Solid(0x5D, 0xAD, 0xF2);

    // Warm red of the event crystal, for event XP values.
    private static readonly Brush EventRed = Solid(0xF2, 0x6A, 0x4F);
    private static readonly FontFamily TitleFont = new("Georgia");

    private readonly GameHistory _history;
    private readonly string _historyPath;
    private readonly Func<string?> _logSession;
    private readonly Func<bool> _hearthstoneRunning;
    private readonly Dictionary<DashboardPeriod, ToggleButton> _periodButtons = new();
    private readonly TextBlock _sessionInfo = new() { Foreground = Muted, FontSize = 12, Margin = new Thickness(14, 0, 14, 6) };
    private readonly UniformGrid _overview = new() { Columns = 4, VerticalAlignment = VerticalAlignment.Top };
    private readonly UniformGrid _lost = new() { Columns = 3, VerticalAlignment = VerticalAlignment.Top };
    private readonly DataGrid _decks = CreateGrid();
    private readonly DataGrid _games = CreateGrid();
    private readonly CartesianChart _xpChart = CreateChart(T("chart.xpAxis"));
    private readonly CartesianChart _rateChart = CreateChart(T("chart.xpPerHour"));
    private readonly bool _checkForUpdates;
    private readonly Action<bool>? _setCheckForUpdates;
    private readonly Action<bool>? _setEventActive;
    private bool _eventActive;
    private readonly Action<bool, int>? _setTavernPass;
    private readonly Action<bool>? _setShowDamage;
    private bool _showDamage;
    private bool _tavernPass;
    private int _tavernPassLevel;
    private readonly TextBlock _updateText = new() { Foreground = Parchment, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap };
    private readonly Border _updateBar;
    private string _updateUrl = UpdateCheck.ReleasesPage;
    private DashboardPeriod _period = DashboardPeriod.Session;

    /// <param name="logSession">Session the log reader is reading; it stays the same after Hearthstone was closed.</param>
    /// <param name="hearthstoneRunning">Whether the game client is running, so the online time of its session counts up to now.</param>
    public DashboardWindow(
        GameHistory history,
        string historyPath,
        Func<string?> logSession,
        Func<bool> hearthstoneRunning,
        Func<IReadOnlyList<DeckInfo>>? hdtDecks = null,
        bool checkForUpdates = true,
        Action<bool>? setCheckForUpdates = null,
        bool eventActive = false,
        Action<bool>? setEventActive = null,
        bool tavernPass = false,
        int tavernPassLevel = TavernPassBonus.MinLevel,
        Action<bool, int>? setTavernPass = null,
        bool showDamage = true,
        Action<bool>? setShowDamage = null)
    {
        _showDamage = showDamage;
        _setShowDamage = setShowDamage;
        _tavernPass = tavernPass;
        _tavernPassLevel = TavernPassBonus.ClampLevel(tavernPassLevel);
        _setTavernPass = setTavernPass;
        _history = history;
        _historyPath = historyPath;
        _logSession = logSession;
        _hearthstoneRunning = hearthstoneRunning;
        _checkForUpdates = checkForUpdates;
        _setCheckForUpdates = setCheckForUpdates;
        _eventActive = eventActive;
        _setEventActive = setEventActive;
        _updateBar = UpdateBar();
        UpdateStoredGames(hdtDecks);

        Title = "HS XP Tracker";
        Width = 1000;
        Height = 680;
        Background = Back;
        Foreground = Parchment;
        Resources = Theme();
        WindowStartupLocation = WindowStartupLocation.CenterScreen;

        SetupDeckColumns();
        SetupGameColumns();

        var tabs = new TabControl { Background = Back, BorderBrush = Gold, BorderThickness = new Thickness(1), Padding = new Thickness(0), Margin = new Thickness(10, 0, 10, 10) };
        tabs.Items.Add(Tab(T("tab.overview"), Padded(_overview)));
        tabs.Items.Add(Tab(T("tab.decks"), _decks));
        tabs.Items.Add(Tab(T("tab.lostXp"), Padded(_lost)));
        tabs.Items.Add(Tab(T("tab.games"), _games));
        tabs.Items.Add(Tab(T("tab.charts"), Charts()));

        var root = new DockPanel();
        var header = Header();
        DockPanel.SetDock(header, Dock.Top);
        root.Children.Add(header);
        DockPanel.SetDock(_updateBar, Dock.Top);
        root.Children.Add(_updateBar);
        var toolbar = Toolbar();
        DockPanel.SetDock(toolbar, Dock.Top);
        root.Children.Add(toolbar);
        DockPanel.SetDock(_sessionInfo, Dock.Top);
        root.Children.Add(_sessionInfo);
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
        _updateText.Text = installed is null
            ? F("update.available", update.VersionText)
            : F("update.availableInstalled", update.VersionText, UpdateCheck.Display(installed));
        _updateUrl = UpdateCheck.SafeLink(update.Url);
        _updateBar.Visibility = Visibility.Visible;
    }

    private Border UpdateBar()
    {
        var link = new Hyperlink(new Run(T("update.openPage"))) { Foreground = GoldBright };
        link.Click += (_, _) => Process.Start(new ProcessStartInfo(_updateUrl) { UseShellExecute = true });
        var linkText = new TextBlock(link) { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(12, 0, 0, 0) };

        var panel = new DockPanel();
        DockPanel.SetDock(linkText, Dock.Right);
        panel.Children.Add(linkText);
        panel.Children.Add(_updateText);

        return new Border
        {
            Background = Solid(0x1F, 0x3B, 0x5C),
            BorderBrush = Mana,
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

        // The log reader keeps naming the last session after Hearthstone was closed; it only counts as running while the game runs.
        var logSession = _logSession();
        var runningSession = _hearthstoneRunning() ? logSession : null;
        var session = Dashboard.SelectSession(all, logSession);

        IReadOnlyList<GameRecord> games;
        DateTime periodStart;
        IReadOnlyList<TimeBucket> buckets;
        if (_period == DashboardPeriod.Session)
        {
            games = Dashboard.InSession(all, session);
            periodStart = (session is null ? null : GameRecord.SessionStart(session)) ?? (games.Count > 0 ? games[0].Start : now);
            var running = session is not null && session == runningSession;
            var end = running ? now : games.Count > 0 ? games.Max(g => g.End) : periodStart;
            buckets = Dashboard.SessionBuckets(games, periodStart, end);
            _sessionInfo.Text = session is null
                ? T("session.none")
                : running
                    ? F("session.running", periodStart.ToString("ddd yyyy-MM-dd, HH:mm", Localizer.Culture))
                    : F("session.ended", periodStart.ToString("ddd yyyy-MM-dd, HH:mm", Localizer.Culture));
            _sessionInfo.Visibility = Visibility.Visible;
        }
        else
        {
            games = Dashboard.InPeriod(all, _period, now);
            periodStart = Dashboard.PeriodStart(_period, now);
            buckets = Dashboard.Buckets(games, _period, now);
            _sessionInfo.Visibility = Visibility.Collapsed;
        }

        var summary = Dashboard.Summary(games, periodStart, now, runningSession);
        var lost = Dashboard.Lost(games, summary);

        _overview.Children.Clear();
        _overview.Children.Add(Tile(T("overview.xp"), summary.Xp.ToString(Localizer.Culture)));
        _overview.Children.Add(Tile(T("overview.xpTime"), TimeFormat.HoursMinutesSeconds(summary.XpSeconds)));
        _overview.Children.Add(Tile(T("overview.playTime"), TimeFormat.HoursMinutesSeconds(summary.PlaySeconds)));
        _overview.Children.Add(Tile(T("overview.onlineTime"), TimeFormat.HoursMinutesSeconds(summary.OnlineSeconds)));
        _overview.Children.Add(Tile(T("overview.games"), summary.Games.ToString(Localizer.Culture), F("overview.winsLosses", summary.Wins, summary.Losses)));
        _overview.Children.Add(Tile(T("overview.xpPerHour"), summary.XpPerHour.ToString("0", Localizer.Culture), T("overview.xpPerHourNote")));
        _overview.Children.Add(Tile(T("overview.xpPerOnlineHour"), summary.XpPerOnlineHour.ToString("0", Localizer.Culture), T("overview.xpPerOnlineHourNote")));
        _overview.Children.Add(Tile(T("overview.winRate"), summary.Games > 0 ? ((double)summary.Wins / summary.Games).ToString("0%", Localizer.Culture) : "-"));
        if (summary.HasEventXp)
        {
            _overview.Children.Add(Tile(T("overview.eventXp"), summary.EventXp.ToString(Localizer.Culture), T("overview.eventXpNote"), EventRed));
        }

        _lost.Children.Clear();
        _lost.Children.Add(Tile(T("lost.afk"), F("lost.xpValue", lost.AfkXp), F("lost.afkNote", lost.AfkTurns)));
        _lost.Children.Add(Tile(T("lost.short"), F("lost.xpValue", lost.ShortGamesXp), F("lost.shortNote", lost.ShortGames)));
        _lost.Children.Add(Tile(T("lost.outside"), F("lost.xpValue", lost.OutsideGamesXp), F("lost.outsideNote", TimeFormat.HoursMinutesSeconds(lost.OutsideGamesSeconds))));

        _decks.ItemsSource = Dashboard.Decks(games).Select(d => new DeckRow(d)).ToList();
        _games.ItemsSource = games.Reverse().Select(g => new GameRow(g)).ToList();

        var labels = buckets.Select(b => b.Label).ToArray();
        ShowXpCurve(buckets, labels);
        ShowRateCurve(buckets, labels, summary.XpPerHour);
    }

    // XP earned per hour (today) or per day: a smooth gold curve over a gold gradient; steps without games are at 0.
    private void ShowXpCurve(IReadOnlyList<TimeBucket> buckets, string[] labels)
    {
        _xpChart.Series = new SeriesCollection
        {
            new LineSeries
            {
                Title = T("chart.xpEarned"),
                Values = new ChartValues<double>(buckets.Select(b => (double)b.Xp)),
                Stroke = GoldBright,
                StrokeThickness = 3,
                Fill = Fade(0xF0, 0xC7, 0x5E, 0x90),
                LineSmoothness = 0.6,
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 9,
                PointForeground = Back,
                LabelPoint = p => F("chart.xpTooltip", buckets[p.Key].Title, buckets[p.Key].Xp, Games(buckets[p.Key].Games)),
            },
        };
        SetTimeAxis(_xpChart, labels);
    }

    // XP per hour in each step: a glowing mana-blue curve through the steps with games, and the period average as a dashed line.
    private void ShowRateCurve(IReadOnlyList<TimeBucket> buckets, string[] labels, double average)
    {
        ChartValues<ObservablePoint> Rates()
        {
            var values = new ChartValues<ObservablePoint>();
            for (var i = 0; i < buckets.Count; i++)
            {
                if (buckets[i].XpPerHour is { } rate)
                {
                    values.Add(new ObservablePoint(i, Math.Round(rate)));
                }
            }

            return values;
        }

        var series = new SeriesCollection
        {
            // A wide, faint copy below the line gives it a soft glow.
            new LineSeries
            {
                Title = "Glow",
                Values = Rates(),
                Stroke = Frozen(new SolidColorBrush(Color.FromArgb(0x40, 0x4A, 0x9B, 0xE8))),
                StrokeThickness = 11,
                Fill = Brushes.Transparent,
                LineSmoothness = 0.6,
                PointGeometry = null,
                LabelPoint = _ => string.Empty,
            },
            new LineSeries
            {
                Title = T("chart.xpPerHour"),
                Values = Rates(),
                Stroke = ManaBright,
                StrokeThickness = 3,
                Fill = Fade(0x4A, 0x9B, 0xE8, 0x60),
                LineSmoothness = 0.6,
                PointGeometry = DefaultGeometries.Circle,
                PointGeometrySize = 9,
                PointForeground = Back,
                LabelPoint = p => F("chart.rateTooltip", buckets[(int)p.X].Title, Math.Round(p.Y), Games(buckets[(int)p.X].Games)),
            },
        };

        if (buckets.Count > 1 && buckets.Any(b => b.XpPerHour is not null))
        {
            var level = Math.Round(average);
            series.Add(new LineSeries
            {
                Title = T("chart.average"),
                Values = new ChartValues<ObservablePoint> { new(0, level), new(buckets.Count - 1, level) },
                Stroke = Muted,
                StrokeThickness = 1.5,
                StrokeDashArray = new DoubleCollection { 5, 4 },
                Fill = Brushes.Transparent,
                LineSmoothness = 0,
                PointGeometry = null,
                LabelPoint = p => F("chart.averageTooltip", Math.Round(p.Y)),
            });
        }

        _rateChart.Series = series;
        SetTimeAxis(_rateChart, labels);
        SetRateAxis(buckets.Select(b => b.XpPerHour).OfType<double>().Append(average));
    }

    // 320 to 480 XP/h covers a Ranked loss (336) and win (470), so differences in speed stand out.
    // The range only widens when a value lies outside, for example Casual losses (240).
    private void SetRateAxis(IEnumerable<double> values)
    {
        var list = values.Where(v => v > 0).ToList();
        var axis = _rateChart.AxisY[0];
        axis.MinValue = Math.Min(320, list.Count > 0 ? Math.Floor(list.Min() / 40) * 40 : 320);
        axis.MaxValue = Math.Max(480, list.Count > 0 ? Math.Ceiling(list.Max() / 40) * 40 : 480);
        axis.Separator.Step = 40;
    }

    private static string Games(int count) => count == 1 ? T("chart.gameOne") : F("chart.gameMany", count);

    private static string T(string key) => Localizer.Text(key);

    private static string F(string key, params object[] args) => Localizer.Format(key, args);

    // The same complete time axis for both charts, so they line up; labels are thinned out for long periods.
    private static void SetTimeAxis(CartesianChart chart, string[] labels)
    {
        var axis = chart.AxisX[0];
        axis.Labels = labels;
        axis.MinValue = -0.4;
        axis.MaxValue = Math.Max(labels.Length, 1) - 0.6;
        axis.Separator.Step = labels.Length switch
        {
            <= 12 => 1,
            <= 24 => 2,
            <= 31 => 3,
            _ => Math.Ceiling(labels.Length / 12.0),
        };
    }

    // Vertical gradient from the given color at the top to almost transparent at the bottom.
    private static Brush Fade(byte r, byte g, byte b, byte topAlpha) =>
        Frozen(new LinearGradientBrush(Color.FromArgb(topAlpha, r, g, b), Color.FromArgb(0x08, r, g, b), 90));

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
        // Centered, so the buttons keep their size when the options column on the right is taller than one row.
        var bar = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(10), VerticalAlignment = VerticalAlignment.Center };
        foreach (var (period, text) in new[]
        {
            (DashboardPeriod.Session, T("period.session")),
            (DashboardPeriod.Today, T("period.today")),
            (DashboardPeriod.Week, T("period.week")),
            (DashboardPeriod.Season, T("period.season")),
            (DashboardPeriod.Last30Days, T("period.last30Days")),
            (DashboardPeriod.All, T("period.all")),
        })
        {
            var button = new ToggleButton { Content = text, Padding = new Thickness(12, 4, 12, 4), Margin = new Thickness(0, 0, 6, 0), IsChecked = period == _period };
            button.Click += (_, _) => SelectPeriod(period);
            _periodButtons[period] = button;
            bar.Children.Add(button);
        }

        bar.Children.Add(new Border { Width = 18 });
        bar.Children.Add(Action(T("toolbar.refresh"), Refresh));
        bar.Children.Add(Action(T("toolbar.openFolder"), OpenFolder));

        var toolbar = new DockPanel();
        var options = new StackPanel { VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 4, 10, 4) };
        if (_setEventActive is not null)
        {
            var eventCheck = new CheckBox
            {
                Content = T("option.event"),
                IsChecked = _eventActive,
                Margin = new Thickness(0, 2, 0, 2),
                ToolTip = T("option.eventTip"),
            };
            eventCheck.Click += (_, _) =>
            {
                _eventActive = eventCheck.IsChecked == true;
                _setEventActive(_eventActive);
            };
            options.Children.Add(eventCheck);
        }

        if (_setShowDamage is not null)
        {
            var damageCheck = new CheckBox
            {
                Content = T("option.damage"),
                IsChecked = _showDamage,
                Margin = new Thickness(0, 2, 0, 2),
                ToolTip = T("option.damageTip"),
            };
            damageCheck.Click += (_, _) =>
            {
                _showDamage = damageCheck.IsChecked == true;
                _setShowDamage(_showDamage);
            };
            options.Children.Add(damageCheck);
        }

        if (_setTavernPass is not null)
        {
            options.Children.Add(TavernPassControls());
        }

        if (_setCheckForUpdates is not null)
        {
            var check = new CheckBox
            {
                Content = T("option.updates"),
                IsChecked = _checkForUpdates,
                Margin = new Thickness(0, 2, 0, 2),
            };
            check.Click += (_, _) => _setCheckForUpdates(check.IsChecked == true);
            options.Children.Add(check);
        }

        DockPanel.SetDock(options, Dock.Right);
        toolbar.Children.Add(options);
        toolbar.Children.Add(bar);
        return toolbar;
    }

    // "Tavern Pass" check box, the rewards track level as [-] [number] [+], and the boost that follows from it.
    // The boost applies to the overlay and to new games; stored games keep the boost they were played with.
    private FrameworkElement TavernPassControls()
    {
        var panel = new StackPanel { Margin = new Thickness(0, 2, 0, 2) };
        var passCheck = new CheckBox
        {
            Content = T("tavern.check"),
            IsChecked = _tavernPass,
            ToolTip = T("tavern.checkTip"),
        };

        var levelBox = new TextBox
        {
            Text = _tavernPassLevel.ToString(CultureInfo.InvariantCulture),
            Width = 44,
            TextAlignment = TextAlignment.Center,
            VerticalContentAlignment = VerticalAlignment.Center,
            Background = Panel,
            Foreground = Parchment,
            BorderBrush = Gold,
            CaretBrush = Parchment,
            Margin = new Thickness(4, 0, 4, 0),
            ToolTip = T("tavern.levelTip"),
        };
        var minus = StepButton("−");
        var plus = StepButton("+");
        // Own line below the level, so the options column does not get wider than the check box texts.
        var boostText = new TextBlock { FontSize = 11, Margin = new Thickness(22, 3, 0, 0) };

        // Shows the current state; disabled controls are dimmed, because the button template has no disabled look.
        void ShowState()
        {
            Enable(levelBox, _tavernPass);
            Enable(minus, _tavernPass && _tavernPassLevel > TavernPassBonus.MinLevel);
            Enable(plus, _tavernPass && _tavernPassLevel < TavernPassBonus.MaxLevel);
            boostText.Text = TavernPassBonus.Describe(_tavernPass, _tavernPassLevel);
            boostText.Foreground = _tavernPass ? ManaBright : Muted;
        }

        void Apply(int level, bool updateBox)
        {
            _tavernPassLevel = TavernPassBonus.ClampLevel(level);
            if (updateBox)
            {
                levelBox.Text = _tavernPassLevel.ToString(CultureInfo.InvariantCulture);
            }

            ShowState();
            _setTavernPass?.Invoke(_tavernPass, _tavernPassLevel);
        }

        passCheck.Click += (_, _) =>
        {
            _tavernPass = passCheck.IsChecked == true;
            Apply(_tavernPassLevel, updateBox: false);
        };
        minus.Click += (_, _) => Apply(_tavernPassLevel - 1, updateBox: true);
        plus.Click += (_, _) => Apply(_tavernPassLevel + 1, updateBox: true);

        // Typed numbers apply as soon as they are valid; leaving the box puts a cleaned-up value back (empty or 0 gives 1).
        levelBox.TextChanged += (_, _) =>
        {
            if (int.TryParse(levelBox.Text, NumberStyles.None, CultureInfo.InvariantCulture, out var typed) && typed != _tavernPassLevel)
            {
                Apply(typed, updateBox: false);
            }
        };
        levelBox.PreviewTextInput += (_, e) => e.Handled = !e.Text.All(char.IsDigit);
        levelBox.LostFocus += (_, _) => levelBox.Text = _tavernPassLevel.ToString(CultureInfo.InvariantCulture);

        var levelRow = new StackPanel { Orientation = Orientation.Horizontal, Margin = new Thickness(22, 4, 0, 0) };
        levelRow.Children.Add(new TextBlock { Text = T("tavern.level"), Foreground = Parchment, VerticalAlignment = VerticalAlignment.Center, Margin = new Thickness(0, 0, 6, 0) });
        levelRow.Children.Add(minus);
        levelRow.Children.Add(levelBox);
        levelRow.Children.Add(plus);

        panel.Children.Add(passCheck);
        panel.Children.Add(levelRow);
        panel.Children.Add(boostText);

        // Show the stored state without saving it again.
        ShowState();
        return panel;
    }

    private static void Enable(UIElement control, bool enabled)
    {
        control.IsEnabled = enabled;
        control.Opacity = enabled ? 1 : 0.4;
    }

    private static Button StepButton(string text) => new()
    {
        Content = text,
        Width = 24,
        Height = 22,
        Padding = new Thickness(0),
        FontWeight = FontWeights.Bold,
    };

    private void SelectPeriod(DashboardPeriod period)
    {
        _period = period;
        foreach (var pair in _periodButtons)
        {
            pair.Value.IsChecked = pair.Key == period;
        }

        Refresh();
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

    // Two charts above each other on the same time axis: XP earned, and the speed in XP per hour.
    private FrameworkElement Charts()
    {
        var grid = new Grid { Margin = new Thickness(14, 8, 14, 10) };
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        var earned = ChartBlock(T("chart.xpEarned"), T("chart.xpEarnedNote"), _xpChart);
        var speed = ChartBlock(T("chart.xpPerHour"), T("chart.xpPerHourNote"), _rateChart);
        Grid.SetRow(speed, 1);
        grid.Children.Add(earned);
        grid.Children.Add(speed);
        return grid;
    }

    private static FrameworkElement ChartBlock(string title, string note, UIElement chart)
    {
        var heading = new StackPanel { Margin = new Thickness(0, 6, 0, 4) };
        heading.Children.Add(new TextBlock { Text = title, Foreground = GoldBright, FontFamily = TitleFont, FontSize = 16, FontWeight = FontWeights.Bold });
        heading.Children.Add(new TextBlock { Text = note, Foreground = Muted, FontSize = 12, TextWrapping = TextWrapping.Wrap });
        var block = new DockPanel();
        DockPanel.SetDock(heading, Dock.Top);
        block.Children.Add(heading);
        block.Children.Add(chart);
        return block;
    }

    // Large gold title with the scroll icon from the overlay.
    private static FrameworkElement Header()
    {
        var icon = XpOverlay.ScrollIcon.Create();
        icon.Height = 30;
        icon.Margin = new Thickness(0, 0, 10, 0);
        var title = new TextBlock
        {
            Text = "HS XP Tracker",
            FontFamily = TitleFont,
            FontSize = 28,
            FontWeight = FontWeights.Bold,
            Foreground = GoldBright,
            VerticalAlignment = VerticalAlignment.Center,
            Effect = new DropShadowEffect { Color = Colors.Black, ShadowDepth = 2, BlurRadius = 4, Opacity = 0.8 },
        };
        var row = new StackPanel { Orientation = Orientation.Horizontal };
        row.Children.Add(icon);
        row.Children.Add(title);
        return new Border
        {
            Child = row,
            Padding = new Thickness(14, 10, 14, 8),
            BorderBrush = Gold,
            BorderThickness = new Thickness(0, 0, 0, 1),
            Background = Panel,
        };
    }

    // Styles for buttons, period buttons, tabs, the check box and selected rows. Own templates, so the look
    // does not depend on the Windows theme or on the styles HDT loads.
    private static ResourceDictionary Theme() => (ResourceDictionary)XamlReader.Parse(@"
<ResourceDictionary xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
                    xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'>
  <SolidColorBrush x:Key='{x:Static SystemColors.HighlightBrushKey}' Color='#6B4A2A'/>
  <SolidColorBrush x:Key='{x:Static SystemColors.InactiveSelectionHighlightBrushKey}' Color='#5A3E24'/>
  <SolidColorBrush x:Key='{x:Static SystemColors.HighlightTextBrushKey}' Color='#F4E4C1'/>
  <SolidColorBrush x:Key='{x:Static SystemColors.InactiveSelectionHighlightTextBrushKey}' Color='#F4E4C1'/>

  <Style x:Key='HsButtonBase' TargetType='ButtonBase'>
    <Setter Property='Foreground' Value='#F4E4C1'/>
    <Setter Property='Background' Value='#3A2716'/>
    <Setter Property='BorderBrush' Value='#C8A050'/>
    <Setter Property='BorderThickness' Value='1'/>
    <Setter Property='Cursor' Value='Hand'/>
    <Setter Property='Template'>
      <Setter.Value>
        <ControlTemplate TargetType='ButtonBase'>
          <Border x:Name='Box' Background='{TemplateBinding Background}' BorderBrush='{TemplateBinding BorderBrush}'
                  BorderThickness='{TemplateBinding BorderThickness}' CornerRadius='4' Padding='{TemplateBinding Padding}'>
            <ContentPresenter HorizontalAlignment='Center' VerticalAlignment='Center'/>
          </Border>
          <ControlTemplate.Triggers>
            <Trigger Property='IsPressed' Value='True'>
              <Setter TargetName='Box' Property='Background' Value='#5A3E24'/>
            </Trigger>
          </ControlTemplate.Triggers>
        </ControlTemplate>
      </Setter.Value>
    </Setter>
    <Style.Triggers>
      <Trigger Property='IsMouseOver' Value='True'>
        <Setter Property='BorderBrush' Value='#F0C75E'/>
      </Trigger>
    </Style.Triggers>
  </Style>
  <Style TargetType='Button' BasedOn='{StaticResource HsButtonBase}'/>

  <!-- Period buttons: the selected one in dark mana blue with light text; a soft blue glow on hover. -->
  <Style TargetType='ToggleButton' BasedOn='{StaticResource HsButtonBase}'>
    <Style.Triggers>
      <Trigger Property='IsChecked' Value='True'>
        <Setter Property='Background' Value='#1F3B5C'/>
        <Setter Property='BorderBrush' Value='#4A9BE8'/>
        <Setter Property='Foreground' Value='#FFFFFF'/>
        <Setter Property='FontWeight' Value='SemiBold'/>
        <Setter Property='Effect'>
          <Setter.Value>
            <DropShadowEffect Color='#4A9BE8' ShadowDepth='0' BlurRadius='8' Opacity='0.5'/>
          </Setter.Value>
        </Setter>
      </Trigger>
      <Trigger Property='IsMouseOver' Value='True'>
        <Setter Property='BorderBrush' Value='#7CC0FF'/>
        <Setter Property='Effect'>
          <Setter.Value>
            <DropShadowEffect Color='#4A9BE8' ShadowDepth='0' BlurRadius='14' Opacity='0.9'/>
          </Setter.Value>
        </Setter>
      </Trigger>
    </Style.Triggers>
  </Style>

  <Style TargetType='TabItem'>
    <Setter Property='Foreground' Value='#C2A57E'/>
    <Setter Property='FontWeight' Value='SemiBold'/>
    <Setter Property='Cursor' Value='Hand'/>
    <Setter Property='Template'>
      <Setter.Value>
        <ControlTemplate TargetType='TabItem'>
          <Border x:Name='Tab' Background='#2C1E12' BorderBrush='#C8A050' BorderThickness='1,1,1,0'
                  CornerRadius='6,6,0,0' Margin='0,0,4,0' Padding='14,6'>
            <ContentPresenter ContentSource='Header' HorizontalAlignment='Center'/>
          </Border>
          <ControlTemplate.Triggers>
            <Trigger Property='IsMouseOver' Value='True'>
              <Setter Property='Foreground' Value='#F4E4C1'/>
            </Trigger>
            <Trigger Property='IsSelected' Value='True'>
              <Setter TargetName='Tab' Property='Background' Value='#4A321D'/>
              <Setter Property='Foreground' Value='#F0C75E'/>
            </Trigger>
          </ControlTemplate.Triggers>
        </ControlTemplate>
      </Setter.Value>
    </Setter>
  </Style>

  <Style TargetType='CheckBox'>
    <Setter Property='Foreground' Value='#F4E4C1'/>
    <Setter Property='Cursor' Value='Hand'/>
    <Setter Property='Template'>
      <Setter.Value>
        <ControlTemplate TargetType='CheckBox'>
          <StackPanel Orientation='Horizontal' Background='Transparent'>
            <Border Width='16' Height='16' Background='#3A2716' BorderBrush='#C8A050' BorderThickness='1'
                    CornerRadius='3' VerticalAlignment='Center'>
              <Path x:Name='Mark' Data='M 3,8 L 6.5,11.5 L 13,4' Stroke='#F0C75E' StrokeThickness='2' Visibility='Collapsed'/>
            </Border>
            <ContentPresenter Margin='6,0,0,0' VerticalAlignment='Center'/>
          </StackPanel>
          <ControlTemplate.Triggers>
            <Trigger Property='IsChecked' Value='True'>
              <Setter TargetName='Mark' Property='Visibility' Value='Visible'/>
            </Trigger>
          </ControlTemplate.Triggers>
        </ControlTemplate>
      </Setter.Value>
    </Setter>
  </Style>
</ResourceDictionary>");

    private void SetupDeckColumns()
    {
        _decks.Columns.Add(Column(T("column.deck"), nameof(DeckRow.Deck), nameof(DeckRow.Deck), width: 2));
        _decks.Columns.Add(Column(T("column.games"), nameof(DeckRow.Games), nameof(DeckRow.Games)));
        _decks.Columns.Add(Column(T("column.winRate"), nameof(DeckRow.WinRate), nameof(DeckRow.WinRateValue)));
        _decks.Columns.Add(Column(T("column.xp"), nameof(DeckRow.Xp), nameof(DeckRow.Xp)));
        _decks.Columns.Add(Column(T("column.xpPerHour"), nameof(DeckRow.XpPerHour), nameof(DeckRow.XpPerHourValue)));
        _decks.Columns.Add(Column(T("column.avgGame"), nameof(DeckRow.AverageGame), nameof(DeckRow.AverageGameSeconds)));
    }

    private void SetupGameColumns()
    {
        _games.Columns.Add(Column(T("column.date"), nameof(GameRow.Date), nameof(GameRow.StartValue)));
        _games.Columns.Add(Column(T("column.start"), nameof(GameRow.Time), nameof(GameRow.StartValue)));
        _games.Columns.Add(Column(T("column.mode"), nameof(GameRow.Mode), nameof(GameRow.Mode)));
        _games.Columns.Add(Column(T("column.result"), nameof(GameRow.Result), nameof(GameRow.Result)));
        _games.Columns.Add(Column(T("column.xpTime"), nameof(GameRow.XpTime), nameof(GameRow.XpSeconds)));
        _games.Columns.Add(Column(T("column.totalTime"), nameof(GameRow.TotalTime), nameof(GameRow.TotalSeconds)));
        _games.Columns.Add(Column(T("column.xp"), nameof(GameRow.Xp), nameof(GameRow.XpValue)));
        _games.Columns.Add(Column(T("column.event"), nameof(GameRow.EventXp), nameof(GameRow.EventXpValue)));
        _games.Columns.Add(Column(T("column.afk"), nameof(GameRow.AfkTurns), nameof(GameRow.AfkTurns)));
        _games.Columns.Add(Column(T("column.deck"), nameof(GameRow.Deck), nameof(GameRow.Deck), width: 2));
        _games.Columns.Add(Column(T("column.opponent"), nameof(GameRow.Opponent), nameof(GameRow.Opponent)));
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
        headerStyle.Setters.Add(new Setter(Control.ForegroundProperty, GoldBright));
        headerStyle.Setters.Add(new Setter(Control.BorderBrushProperty, Gold));
        headerStyle.Setters.Add(new Setter(Control.BorderThicknessProperty, new Thickness(0, 0, 0, 1)));
        headerStyle.Setters.Add(new Setter(Control.PaddingProperty, new Thickness(6, 5, 6, 5)));
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
            Foreground = Parchment,
            BorderBrush = Gold,
            BorderThickness = new Thickness(1),
            ColumnHeaderStyle = headerStyle,
            Margin = new Thickness(10),
        };
    }

    private static CartesianChart CreateChart(string title)
    {
        var grid = Solid(0x4A, 0x32, 0x1D);
        var chart = new CartesianChart { Foreground = Parchment, LegendLocation = LegendLocation.None, DisableAnimations = true };

        // Dark tooltip with light text; LiveCharts' default light grey box is hard to read here.
        chart.DataTooltip = new DefaultTooltip
        {
            Background = Frozen(new SolidColorBrush(Color.FromArgb(0xF0, 0x22, 0x17, 0x0F))),
            Foreground = Parchment,
            BorderBrush = Gold,
            BorderThickness = new Thickness(1),

            // Only the hovered curve; the glow and the average line do not clutter the tooltip.
            SelectionMode = TooltipSelectionMode.OnlySender,
        };
        chart.AxisX.Add(new Axis { Foreground = Muted, Separator = new LiveCharts.Wpf.Separator { StrokeThickness = 0 } });
        chart.AxisY.Add(new Axis { Title = title, Foreground = Muted, MinValue = 0, Separator = new LiveCharts.Wpf.Separator { Stroke = grid, StrokeThickness = 1 } });
        return chart;
    }

    private static Brush Solid(byte r, byte g, byte b) => Frozen(new SolidColorBrush(Color.FromRgb(r, g, b)));

    private static TabItem Tab(string header, UIElement content) => new() { Header = header, Content = content, Padding = new Thickness(12, 4, 12, 4) };

    private static FrameworkElement Padded(UIElement content) =>
        new ScrollViewer { Content = new Border { Padding = new Thickness(10), Child = content }, VerticalScrollBarVisibility = ScrollBarVisibility.Auto };

    private static Button Action(string text, Action action)
    {
        var button = new Button { Content = text, Padding = new Thickness(12, 4, 12, 4), Margin = new Thickness(0, 0, 6, 0) };
        button.Click += (_, _) => action();
        return button;
    }

    private static Border Tile(string title, string value, string? note = null, Brush? valueColor = null)
    {
        var panel = new StackPanel();
        panel.Children.Add(new TextBlock { Text = title, Foreground = Muted, FontSize = 13, FontWeight = FontWeights.SemiBold });
        panel.Children.Add(new TextBlock { Text = value, Foreground = valueColor ?? ManaBright, FontFamily = TitleFont, FontSize = 28, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 4, 0, 2) });
        if (note is not null)
        {
            panel.Children.Add(new TextBlock { Text = note, Foreground = Muted, FontSize = 11, TextWrapping = TextWrapping.Wrap });
        }

        return new Border { Background = Panel, BorderBrush = Gold, BorderThickness = new Thickness(1), CornerRadius = new CornerRadius(6), Padding = new Thickness(14), Margin = new Thickness(6), Child = panel };
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

        public string WinRate => WinRateValue.ToString("0%", Localizer.Culture);

        public int Xp { get; }

        public double XpPerHourValue { get; }

        public string XpPerHour => XpPerHourValue.ToString("0", Localizer.Culture);

        public double AverageGameSeconds { get; }

        public string AverageGame => TimeFormat.MinutesSeconds(AverageGameSeconds);
    }

    private sealed class GameRow
    {
        public GameRow(GameRecord g)
        {
            StartValue = g.Start;
            var mode = GameText.Mode(g.Mode);
            Mode = g.Format.Length > 0 ? mode + " " + GameText.Format(g.Format) : mode;
            Result = GameText.Result(g.Result);
            XpSeconds = g.XpSeconds;
            TotalSeconds = g.WithMulliganSeconds;
            XpValue = g.Xp ?? -1;
            Xp = g.Xp?.ToString(CultureInfo.InvariantCulture) ?? "-";
            EventXpValue = g.EventXp ?? -1;
            EventXp = g.EventXp?.ToString(CultureInfo.InvariantCulture) ?? "-";
            AfkTurns = g.AfkTurns;
            Deck = g.Deck;
            Opponent = GameText.Class(g.OpponentClass);
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

        public int EventXpValue { get; }

        public string EventXp { get; }

        public int AfkTurns { get; }

        public string Deck { get; }

        public string Opponent { get; }
    }
}
