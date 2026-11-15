using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Shapes;
using XpTracker.Core;

namespace XpTracker.Plugin;

/// <summary>
/// Overlay box placed above the Hearthstone settings button: plain white labels on the left,
/// values in blue tubes on the right in the style of the reward track XP bar, XP values with a scroll icon.
/// </summary>
internal sealed class XpOverlay : Border
{
    // Positions relative to the overlay size.
    // Right: above the Hearthstone settings button, measured from the bottom right corner.
    private const double RightOffset = 0.012;
    private const double RightBottomOffset = 0.085;

    // Left: above the player's name plate ("You" with the rank), measured from the bottom left corner,
    // below the card lists HDT shows on the left; the box grows to the right.
    private const double LeftOffset = 0.006;
    private const double LeftBottomOffset = 0.2;
    private const double FontSizeFactor = 0.016;

    private static readonly Brush BoxFill = Frozen(new SolidColorBrush(Color.FromArgb(0xC8, 0x14, 0x14, 0x14)));
    private static readonly Brush BoxBorder = Frozen(new SolidColorBrush(Color.FromArgb(0xC8, 0xC8, 0xA0, 0x50)));
    private static readonly Brush TubeFill = Frozen(Vertical(
        (Color.FromRgb(0x5A, 0x9C, 0xE8), 0.0),
        (Color.FromRgb(0x2C, 0x6C, 0xC4), 0.45),
        (Color.FromRgb(0x17, 0x45, 0x8C), 1.0)));
    private static readonly Brush TubeGloss = Frozen(Vertical(
        (Color.FromArgb(0x60, 0xFF, 0xFF, 0xFF), 0.0),
        (Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), 1.0)));
    private static readonly Brush TubeBorder = Frozen(new SolidColorBrush(Color.FromRgb(0x10, 0x14, 0x22)));

    // Mana blue, as the check marks and selected values in the dashboard: the update hint stands out from the white labels.
    private static readonly Brush UpdateLabel = Frozen(new SolidColorBrush(Color.FromRgb(0x5D, 0xAD, 0xF2)));
    private static readonly FontFamily LabelFont = new("Segoe UI");
    private static readonly FontFamily ValueFont = new("Georgia");

    private readonly Grid _grid = new();
    private readonly List<Row> _rows = new();
    private double _fontSize = 14;

    public XpOverlay()
    {
        Background = BoxFill;
        BorderBrush = BoxBorder;
        BorderThickness = new Thickness(1);
        CornerRadius = new CornerRadius(4);
        IsHitTestVisible = false;
        Visibility = Visibility.Collapsed;
        _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        _grid.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Auto });
        Child = _grid;
    }

    public void Show(IReadOnlyList<XpLine> lines)
    {
        if (lines.Count == 0)
        {
            Visibility = Visibility.Collapsed;
            return;
        }

        while (_rows.Count < lines.Count)
        {
            var row = new Row(_rows.Count, _grid);
            row.Resize(_fontSize);
            _rows.Add(row);
        }

        for (var i = 0; i < _rows.Count; i++)
        {
            _rows[i].Set(i < lines.Count ? lines[i] : null);
        }

        Visibility = Visibility.Visible;
    }

    /// <summary>Corner the overlay is placed in; applied with the next <see cref="Place"/>.</summary>
    public OverlayPosition Position { get; set; } = OverlayPosition.Right;

    public void Place(double canvasWidth, double canvasHeight)
    {
        if (canvasWidth <= 0 || canvasHeight <= 0)
        {
            return;
        }

        var fontSize = Math.Max(10, canvasHeight * FontSizeFactor);
        if (Math.Abs(fontSize - _fontSize) > 0.01)
        {
            _fontSize = fontSize;
            Padding = new Thickness(fontSize * 0.5, fontSize * 0.35, fontSize * 0.5, fontSize * 0.1);
            foreach (var row in _rows)
            {
                row.Resize(fontSize);
            }
        }

        if (Position == OverlayPosition.Left)
        {
            Canvas.SetRight(this, double.NaN);
            Canvas.SetLeft(this, canvasWidth * LeftOffset);
            Canvas.SetBottom(this, canvasHeight * LeftBottomOffset);
        }
        else
        {
            Canvas.SetLeft(this, double.NaN);
            Canvas.SetRight(this, canvasWidth * RightOffset);
            Canvas.SetBottom(this, canvasHeight * RightBottomOffset);
        }
    }

    private static LinearGradientBrush Vertical(params (Color Color, double Offset)[] stops)
    {
        var brush = new LinearGradientBrush { StartPoint = new Point(0, 0), EndPoint = new Point(0, 1) };
        foreach (var (color, offset) in stops)
        {
            brush.GradientStops.Add(new GradientStop(color, offset));
        }

        return brush;
    }

    private static Brush Frozen(Brush brush)
    {
        brush.Freeze();
        return brush;
    }

    private sealed class Row
    {
        private readonly TextBlock _label;
        private readonly Border _tube;
        private readonly Border _gloss;
        private readonly TextBlock _value;
        private readonly FrameworkElement _icon;
        private readonly FrameworkElement _crystal;

        public Row(int index, Grid grid)
        {
            grid.RowDefinitions.Add(new RowDefinition { Height = GridLength.Auto });

            _label = new TextBlock
            {
                Foreground = Brushes.White,
                FontFamily = LabelFont,
                FontWeight = FontWeights.SemiBold,
                VerticalAlignment = VerticalAlignment.Center,
            };
            Grid.SetRow(_label, index);
            Grid.SetColumn(_label, 0);

            _value = new TextBlock
            {
                Foreground = Brushes.White,
                FontFamily = ValueFont,
                FontWeight = FontWeights.Bold,
                VerticalAlignment = VerticalAlignment.Center,
                // Dark halo keeps the white text readable on the light top of the tube.
                Effect = new DropShadowEffect { Color = Colors.Black, ShadowDepth = 1, BlurRadius = 3, Opacity = 0.9 },
            };
            _icon = ScrollIcon.Create();
            _crystal = CrystalIcon.Create();

            var content = new StackPanel { Orientation = Orientation.Horizontal, HorizontalAlignment = HorizontalAlignment.Center };
            content.Children.Add(_icon);
            content.Children.Add(_crystal);
            content.Children.Add(_value);

            _gloss = new Border { Background = TubeGloss, VerticalAlignment = VerticalAlignment.Top };
            var layers = new Grid();
            layers.Children.Add(_gloss);
            layers.Children.Add(content);

            _tube = new Border { Background = TubeFill, BorderBrush = TubeBorder, Child = layers };
            Grid.SetRow(_tube, index);
            Grid.SetColumn(_tube, 1);

            grid.Children.Add(_label);
            grid.Children.Add(_tube);
        }

        public void Set(XpLine? line)
        {
            var visibility = line is null ? Visibility.Collapsed : Visibility.Visible;
            _label.Visibility = visibility;
            _tube.Visibility = visibility;
            if (line is null)
            {
                return;
            }

            _label.Text = line.Label;
            _label.Foreground = line.IsUpdate ? UpdateLabel : Brushes.White;
            _value.Text = line.Value;
            _icon.Visibility = line.IsXp ? Visibility.Visible : Visibility.Collapsed;
            _crystal.Visibility = line.IsEvent ? Visibility.Visible : Visibility.Collapsed;
        }

        public void Resize(double fontSize)
        {
            var height = fontSize * 1.6;
            _label.FontSize = fontSize;
            _label.Margin = new Thickness(0, 0, fontSize * 0.6, fontSize * 0.25);
            _value.FontSize = fontSize;

            // Georgia sits low in its line box; lift the numbers a little so they look centered in the tube.
            _value.RenderTransform = new TranslateTransform(0, -fontSize * 0.09);
            _icon.Height = fontSize * 1.1;
            _icon.Margin = new Thickness(0, 0, fontSize * 0.25, 0);
            _crystal.Height = fontSize * 1.2;
            _crystal.Margin = new Thickness(0, 0, fontSize * 0.3, 0);
            _tube.Height = height;
            _tube.MinWidth = fontSize * 5.5;
            _tube.CornerRadius = new CornerRadius(height / 2);
            _tube.BorderThickness = new Thickness(Math.Max(1.5, fontSize * 0.12));
            _tube.Padding = new Thickness(height * 0.4, 0, height * 0.4, 0);
            _tube.Margin = new Thickness(0, 0, 0, fontSize * 0.25);
            _gloss.Height = height * 0.45;
            _gloss.CornerRadius = new CornerRadius(height / 2, height / 2, 0, 0);
            _gloss.Margin = new Thickness(-height * 0.3, 0, -height * 0.3, 0);
        }
    }

    /// <summary>
    /// Small rolled parchment drawn as vector shapes, used as the XP symbol (no game artwork).
    /// Also used in the dashboard title.
    /// </summary>
    internal static class ScrollIcon
    {
        private static readonly Brush Paper = Frozen(Vertical((Color.FromRgb(0xF6, 0xE7, 0xBE), 0.0), (Color.FromRgb(0xD8, 0xBA, 0x7C), 1.0)));
        private static readonly Brush Roll = Frozen(Vertical((Color.FromRgb(0xD9, 0xB0, 0x6A), 0.0), (Color.FromRgb(0x8C, 0x62, 0x2C), 1.0)));
        private static readonly Brush Ink = Frozen(new SolidColorBrush(Color.FromRgb(0x5A, 0x3A, 0x16)));

        public static FrameworkElement Create()
        {
            // Drawn on a 22 x 16 grid and scaled to the font size.
            var canvas = new Canvas { Width = 22, Height = 16 };
            Add(canvas, new Rectangle { Width = 14, Height = 11, Fill = Paper, Stroke = Ink, StrokeThickness = 1 }, 4, 2.5);
            Add(canvas, new Line { X1 = 7, Y1 = 6, X2 = 15, Y2 = 6, Stroke = Ink, StrokeThickness = 0.8 }, 0, 0);
            Add(canvas, new Line { X1 = 7, Y1 = 9.5, X2 = 13.5, Y2 = 9.5, Stroke = Ink, StrokeThickness = 0.8 }, 0, 0);
            Add(canvas, new Rectangle { Width = 4.5, Height = 15, RadiusX = 2.2, RadiusY = 2.2, Fill = Roll, Stroke = Ink, StrokeThickness = 1 }, 1, 0.5);
            Add(canvas, new Rectangle { Width = 4.5, Height = 15, RadiusX = 2.2, RadiusY = 2.2, Fill = Roll, Stroke = Ink, StrokeThickness = 1 }, 16.5, 0.5);
            canvas.RenderTransform = new RotateTransform(-12, 11, 8);

            return new Viewbox { Child = canvas, VerticalAlignment = VerticalAlignment.Center, Stretch = Stretch.Uniform };
        }

        private static void Add(Canvas canvas, UIElement shape, double left, double top)
        {
            Canvas.SetLeft(shape, left);
            Canvas.SetTop(shape, top);
            canvas.Children.Add(shape);
        }
    }

    /// <summary>
    /// Glowing red crystal drawn as vector shapes, used as the event XP symbol (own drawing, no game artwork).
    /// </summary>
    private static class CrystalIcon
    {
        private static readonly Brush Body = Frozen(Vertical(
            (Color.FromRgb(0xFF, 0x9A, 0x7A), 0.0),
            (Color.FromRgb(0xE8, 0x3A, 0x22), 0.45),
            (Color.FromRgb(0x8A, 0x10, 0x08), 1.0)));
        private static readonly Brush Core = Frozen(Vertical(
            (Color.FromArgb(0xC0, 0xFF, 0xC0, 0x90), 0.0),
            (Color.FromArgb(0x30, 0xFF, 0x60, 0x30), 1.0)));
        private static readonly Brush Edge = Frozen(new SolidColorBrush(Color.FromRgb(0x4A, 0x08, 0x04)));
        private static readonly Brush Facet = Frozen(new SolidColorBrush(Color.FromArgb(0xB0, 0x5A, 0x0C, 0x06)));

        public static FrameworkElement Create()
        {
            // Drawn on a 16 x 22 grid and scaled to the font size: an elongated gem with a lighter inner facet.
            var canvas = new Canvas { Width = 16, Height = 22 };
            canvas.Children.Add(new Polygon
            {
                Points = new PointCollection { new(8, 0.5), new(15, 6), new(13.5, 17), new(8, 21.5), new(2.5, 17), new(1, 6) },
                Fill = Body,
                Stroke = Edge,
                StrokeThickness = 1,
                StrokeLineJoin = PenLineJoin.Round,
            });
            canvas.Children.Add(new Polygon
            {
                Points = new PointCollection { new(8, 4), new(11.5, 8), new(10.5, 15), new(8, 17.5), new(5.5, 15), new(4.5, 8) },
                Fill = Core,
                Stroke = Facet,
                StrokeThickness = 0.7,
            });
            foreach (var (x1, y1, x2, y2) in new[] { (8.0, 0.5, 8.0, 4.0), (1.0, 6.0, 4.5, 8.0), (15.0, 6.0, 11.5, 8.0), (2.5, 17.0, 5.5, 15.0), (13.5, 17.0, 10.5, 15.0), (8.0, 21.5, 8.0, 17.5) })
            {
                canvas.Children.Add(new Line { X1 = x1, Y1 = y1, X2 = x2, Y2 = y2, Stroke = Facet, StrokeThickness = 0.7 });
            }

            return new Viewbox
            {
                Child = canvas,
                VerticalAlignment = VerticalAlignment.Center,
                Stretch = Stretch.Uniform,
                Effect = new DropShadowEffect { Color = Color.FromRgb(0xFF, 0x40, 0x20), ShadowDepth = 0, BlurRadius = 6, Opacity = 0.8 },
            };
        }
    }
}
