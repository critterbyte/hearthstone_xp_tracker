using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interop;
using System.Windows.Media;

namespace XpTracker.App;

/// <summary>
/// Transparent window that lies exactly over Hearthstone's game area and carries the <see cref="XpOverlay"/>.
/// </summary>
/// <remarks>
/// Like Hearthstone Deck Tracker's overlay it only looks at the position of the game window: it lets every click
/// through to the game, never takes the focus, is not in the taskbar or Alt+Tab, and is shown only while Hearthstone
/// is the active window.
/// </remarks>
internal sealed class OverlayWindow : Window
{
    // Hearthstone is a Unity game; its main window has this class and title.
    private const string GameWindowClass = "UnityWndClass";
    private const string GameWindowTitle = "Hearthstone";

    // How often the window is put on top again while it is shown; other topmost windows can cover it.
    private static readonly TimeSpan TopmostInterval = TimeSpan.FromSeconds(1);

    private readonly Canvas _canvas = new() { Background = Brushes.Transparent };
    private IntPtr _handle;
    private NativeMethods.Rect _bounds;
    private DateTime _nextTopmost;
    private bool _shown;

    public OverlayWindow(XpOverlay overlay)
    {
        Overlay = overlay;
        Title = "Hearthstone XP Tracker overlay";
        WindowStyle = WindowStyle.None;
        AllowsTransparency = true;
        Background = Brushes.Transparent;
        ResizeMode = ResizeMode.NoResize;
        ShowInTaskbar = false;
        ShowActivated = false;
        Topmost = true;
        Focusable = false;
        IsHitTestVisible = false;
        Left = -10000;
        Top = -10000;
        Width = 1;
        Height = 1;
        _canvas.Children.Add(overlay);
        Content = _canvas;
    }

    public XpOverlay Overlay { get; }

    /// <summary>Width and height of the game area in device-independent units, for placing the overlay.</summary>
    public Size GameArea => new(_canvas.ActualWidth, _canvas.ActualHeight);

    protected override void OnSourceInitialized(EventArgs e)
    {
        base.OnSourceInitialized(e);
        _handle = new WindowInteropHelper(this).Handle;
        var style = NativeMethods.GetWindowLong(_handle, NativeMethods.GwlExStyle);
        NativeMethods.SetWindowLong(_handle, NativeMethods.GwlExStyle, style | NativeMethods.WsExTransparent | NativeMethods.WsExToolWindow | NativeMethods.WsExNoActivate);
    }

    /// <summary>
    /// Follows the game window: covers its game area while it is the active window, hides otherwise.
    /// Call regularly on the UI thread.
    /// </summary>
    public void Follow()
    {
        var game = NativeMethods.FindWindow(GameWindowClass, GameWindowTitle);
        if (game == IntPtr.Zero || NativeMethods.IsIconic(game) || NativeMethods.GetForegroundWindow() != game || !TryGetGameArea(game, out var area))
        {
            if (_shown)
            {
                Hide();
                _shown = false;
            }

            return;
        }

        if (!_shown)
        {
            Show();
            _shown = true;
            _bounds = default;
        }

        var now = DateTime.UtcNow;
        if (!SameBounds(area, _bounds) || now >= _nextTopmost)
        {
            // Position and size in screen pixels, so Windows' display scaling needs no conversion here.
            NativeMethods.SetWindowPos(_handle, NativeMethods.HwndTopmost, area.Left, area.Top, area.Right - area.Left, area.Bottom - area.Top, NativeMethods.SwpNoActivate | NativeMethods.SwpNoOwnerZOrder);
            _bounds = area;
            _nextTopmost = now + TopmostInterval;
        }
    }

    // The game area without the title bar and border, in screen pixels.
    private static bool TryGetGameArea(IntPtr game, out NativeMethods.Rect area)
    {
        area = default;
        if (!NativeMethods.GetClientRect(game, out var client) || client.Right <= 0 || client.Bottom <= 0)
        {
            return false;
        }

        var origin = new NativeMethods.Point();
        if (!NativeMethods.ClientToScreen(game, ref origin))
        {
            return false;
        }

        area = new NativeMethods.Rect { Left = origin.X, Top = origin.Y, Right = origin.X + client.Right, Bottom = origin.Y + client.Bottom };
        return true;
    }

    private static bool SameBounds(NativeMethods.Rect a, NativeMethods.Rect b) =>
        a.Left == b.Left && a.Top == b.Top && a.Right == b.Right && a.Bottom == b.Bottom;
}
