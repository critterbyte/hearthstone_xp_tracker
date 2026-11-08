using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Effects;

namespace XpTracker.Plugin;

/// <summary>
/// Overlay lines placed above the Hearthstone settings button, each drawn as a blue tube
/// in the style of the reward track XP bar.
/// </summary>
internal sealed class XpOverlay : StackPanel
{
    // Position relative to the overlay size, measured from the bottom right corner.
    // First guess for "above the settings button"; to be tuned on a real screen.
    private const double RightOffset = 0.012;
    private const double BottomOffset = 0.085;
    private const double FontSizeFactor = 0.018;

    private static readonly Brush TubeFill = Frozen(new LinearGradientBrush(
        new GradientStopCollection
        {
            new(Color.FromRgb(0x5A, 0x9C, 0xE8), 0.0),
            new(Color.FromRgb(0x2C, 0x6C, 0xC4), 0.45),
            new(Color.FromRgb(0x17, 0x45, 0x8C), 1.0),
        },
        new Point(0, 0),
        new Point(0, 1)));

    private static readonly Brush TubeGloss = Frozen(new LinearGradientBrush(
        new GradientStopCollection
        {
            new(Color.FromArgb(0x70, 0xFF, 0xFF, 0xFF), 0.0),
            new(Color.FromArgb(0x00, 0xFF, 0xFF, 0xFF), 1.0),
        },
        new Point(0, 0),
        new Point(0, 1)));

    private static readonly Brush TubeBorder = Frozen(new SolidColorBrush(Color.FromRgb(0x10, 0x14, 0x22)));
    private static readonly FontFamily TextFont = new("Georgia");

    private readonly List<Tube> _tubes = new();

    public XpOverlay()
    {
        IsHitTestVisible = false;
        Visibility = Visibility.Collapsed;
    }

    public void Show(IReadOnlyList<string> lines)
    {
        if (lines.Count == 0)
        {
            Visibility = Visibility.Collapsed;
            return;
        }

        while (_tubes.Count < lines.Count)
        {
            var tube = new Tube();
            _tubes.Add(tube);
            Children.Add(tube.Root);
        }

        for (var i = 0; i < _tubes.Count; i++)
        {
            _tubes[i].Text.Text = i < lines.Count ? lines[i] : string.Empty;
            _tubes[i].Root.Visibility = i < lines.Count ? Visibility.Visible : Visibility.Collapsed;
        }

        Visibility = Visibility.Visible;
    }

    public void Place(double canvasWidth, double canvasHeight)
    {
        if (canvasWidth <= 0 || canvasHeight <= 0)
        {
            return;
        }

        var fontSize = Math.Max(10, canvasHeight * FontSizeFactor);
        foreach (var tube in _tubes)
        {
            tube.Resize(fontSize);
        }

        Canvas.SetRight(this, canvasWidth * RightOffset);
        Canvas.SetBottom(this, canvasHeight * BottomOffset);
    }

    private static Brush Frozen(Brush brush)
    {
        brush.Freeze();
        return brush;
    }

    private sealed class Tube
    {
        private readonly Border _gloss;

        public Tube()
        {
            Text = new TextBlock
            {
                Foreground = Brushes.White,
                FontFamily = TextFont,
                FontWeight = FontWeights.Bold,
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center,
                // Dark halo keeps the white text readable on the light top of the tube.
                Effect = new DropShadowEffect { Color = Colors.Black, ShadowDepth = 1, BlurRadius = 3, Opacity = 0.9 },
            };

            _gloss = new Border { Background = TubeGloss, VerticalAlignment = VerticalAlignment.Top };

            var content = new Grid();
            content.Children.Add(_gloss);
            content.Children.Add(Text);

            Root = new Border
            {
                Background = TubeFill,
                BorderBrush = TubeBorder,
                Child = content,
            };
        }

        public Border Root { get; }

        public TextBlock Text { get; }

        public void Resize(double fontSize)
        {
            var height = fontSize * 1.75;
            Text.FontSize = fontSize;
            Root.Height = height;
            Root.MinWidth = fontSize * 9;
            Root.CornerRadius = new CornerRadius(height / 2);
            Root.BorderThickness = new Thickness(Math.Max(1.5, fontSize * 0.12));
            Root.Padding = new Thickness(height * 0.45, 0, height * 0.45, 0);
            Root.Margin = new Thickness(0, 0, 0, fontSize * 0.25);
            _gloss.Height = height * 0.45;
            _gloss.CornerRadius = new CornerRadius(height / 2, height / 2, 0, 0);
            _gloss.Margin = new Thickness(-height * 0.3, 0, -height * 0.3, 0);
        }
    }
}
