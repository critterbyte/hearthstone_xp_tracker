using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XpTracker.Plugin;

/// <summary>
/// Small text box on the HDT overlay, placed above the Hearthstone settings button.
/// </summary>
internal sealed class XpOverlay : Border
{
    // Position relative to the overlay size, measured from the bottom right corner.
    // First guess for "above the settings button"; to be tuned on a real screen.
    private const double RightOffset = 0.012;
    private const double BottomOffset = 0.085;
    private const double FontSizeFactor = 0.018;

    private readonly List<TextBlock> _lines = new();
    private readonly StackPanel _panel = new();

    public XpOverlay()
    {
        Background = new SolidColorBrush(Color.FromArgb(0xC8, 0x14, 0x14, 0x14));
        BorderBrush = new SolidColorBrush(Color.FromArgb(0xC8, 0xC8, 0xA0, 0x50));
        BorderThickness = new Thickness(1);
        CornerRadius = new CornerRadius(4);
        Padding = new Thickness(6, 3, 6, 3);
        IsHitTestVisible = false;
        Visibility = Visibility.Collapsed;
        Child = _panel;
    }

    public void Show(IReadOnlyList<string> lines)
    {
        if (lines.Count == 0)
        {
            Visibility = Visibility.Collapsed;
            return;
        }

        while (_lines.Count < lines.Count)
        {
            var block = new TextBlock { Foreground = Brushes.White, FontWeight = FontWeights.SemiBold };
            _lines.Add(block);
            _panel.Children.Add(block);
        }

        for (var i = 0; i < _lines.Count; i++)
        {
            _lines[i].Text = i < lines.Count ? lines[i] : string.Empty;
            _lines[i].Visibility = i < lines.Count ? Visibility.Visible : Visibility.Collapsed;
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
        foreach (var line in _lines)
        {
            line.FontSize = fontSize;
        }

        Canvas.SetRight(this, canvasWidth * RightOffset);
        Canvas.SetBottom(this, canvasHeight * BottomOffset);
    }
}
