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
    private static readonly FontFamily TitleFont = new("Georgia");

    private readonly GameHistory _history;
    private readonly string _historyPath;
    private readonly Func<string?> _currentSession;
    private readonly Dictionary<DashboardPeriod, ToggleButton> _periodButtons = new();
    private readonly UniformGrid _overview = new() { Columns = 4, VerticalAlignment = VerticalAlignment.Top };
    private readonly UniformGrid _lost = new() { Columns = 3, VerticalAlignment = VerticalAlignment.Top };
    private readonly DataGrid _decks = CreateGrid();
    private readonly DataGrid _games = CreateGrid();
    private readonly CartesianChart _speedChart = CreateChart("XP per hour");
    private readonly CartesianChart _progressChart = CreateProgressChart();
    private readonly bool _checkForUpdates;
    private readonly Action<bool>? _setCheckForUpdates;
    private readonly TextBlock _updateText = new() { Foreground = Parchment, VerticalAlignment = VerticalAlignment.Center, TextWrapping = TextWrapping.Wrap };
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
        tabs.Items.Add(Tab("Overview", Padded(_overview)));
        tabs.Items.Add(Tab("Decks", _decks));
        tabs.Items.Add(Tab("Lost XP", Padded(_lost)));
        tabs.Items.Add(Tab("Games", _games));
        tabs.Items.Add(Tab("Charts", Charts()));

        var root = new DockPanel();
        var header = Header();
        DockPanel.SetDock(header, Dock.Top);
        root.Children.Add(header);
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
        var link = new Hyperlink(new Run("Open the download page")) { Foreground = GoldBright };
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

        var curve = Dashboard.SpeedCurve(games, _period);
        var labelFormat = _period == DashboardPeriod.Today ? "HH:mm" : "MM-dd";
        _speedChart.Series = new SeriesCollection
        {
            new LineSeries
            {
                Title = "XP per hour of game time",
                Values = new ChartValues<double>(curve.Select(p => Math.Round(p.XpPerHour))),
                Stroke = Mana,
                StrokeThickness = 2.5,
                Fill = Brushes.Transparent,
                PointGeometrySize = 7,
                PointForeground = Mana,
                LineSmoothness = 0.3,
            },
        };
        _speedChart.AxisX[0].Labels = curve.Select(p => p.Time.ToString(labelFormat, CultureInfo.InvariantCulture)).ToArray();

        var progress = Dashboard.ProgressCurve(games);
        _progressChart.Series = new SeriesCollection
        {
            new LineSeries
            {
                Title = "XP",
                Values = new ChartValues<ObservablePoint>(progress.Select(p => new ObservablePoint(p.PlaySeconds / 3600, p.Xp))),
                Stroke = GoldBright,
                StrokeThickness = 2.5,
                Fill = Solid(0x4A, 0x32, 0x1D),
                PointGeometrySize = 6,
                PointForeground = GoldBright,
                LineSmoothness = 0,
                LabelPoint = p => $"{p.Y:0} XP after {HoursMinutes(p.X)} of play",
            },
        };
    }

    // Hours as h:mm, for the progress chart's time axis.
    private static string HoursMinutes(double hours)
    {
        var minutes = (int)Math.Round(hours * 60);
        return (minutes / 60).ToString(CultureInfo.InvariantCulture) + ":" + (minutes % 60).ToString("00", CultureInfo.InvariantCulture);
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
        foreach (var (period, text) in new[]
        {
            (DashboardPeriod.Today, "Today"),
            (DashboardPeriod.Week, "This week"),
            (DashboardPeriod.Season, "This season"),
            (DashboardPeriod.Last30Days, "Last 30 days"),
            (DashboardPeriod.All, "All time"),
        })
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

    // Two charts above each other: the running XP per hour, and the XP earned over the play time.
    private FrameworkElement Charts()
    {
        var grid = new Grid { Margin = new Thickness(14, 8, 14, 10) };
        grid.RowDefinitions.Add(new RowDefinition());
        grid.RowDefinitions.Add(new RowDefinition());
        var speed = ChartBlock(
            "XP per hour",
            "Running average of XP per hour of game time from the start of the period: one point per game today, one point per day for longer periods.",
            _speedChart);
        var progress = ChartBlock(
            "XP over play time",
            "XP earned so far over the time spent in games (including the mulligan). The steeper the line, the faster you earn XP.",
            _progressChart);
        Grid.SetRow(progress, 1);
        grid.Children.Add(speed);
        grid.Children.Add(progress);
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
        };
        chart.AxisX.Add(new Axis { Foreground = Muted, Separator = new LiveCharts.Wpf.Separator { StrokeThickness = 0 } });
        chart.AxisY.Add(new Axis { Title = title, Foreground = Muted, MinValue = 0, Separator = new LiveCharts.Wpf.Separator { Stroke = grid, StrokeThickness = 1 } });
        return chart;
    }

    // XP over play time: a numeric time axis in h:mm instead of one label per point.
    private static CartesianChart CreateProgressChart()
    {
        var chart = CreateChart("XP");
        chart.AxisX[0] = new Axis
        {
            Title = "Play time (h:mm)",
            Foreground = Muted,
            MinValue = 0,
            LabelFormatter = HoursMinutes,
            Separator = new LiveCharts.Wpf.Separator { StrokeThickness = 0 },
        };
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

    private static Border Tile(string title, string value, string? note = null)
    {
        var panel = new StackPanel();
        panel.Children.Add(new TextBlock { Text = title, Foreground = Muted, FontSize = 13, FontWeight = FontWeights.SemiBold });
        panel.Children.Add(new TextBlock { Text = value, Foreground = ManaBright, FontFamily = TitleFont, FontSize = 28, FontWeight = FontWeights.Bold, Margin = new Thickness(0, 4, 0, 2) });
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
