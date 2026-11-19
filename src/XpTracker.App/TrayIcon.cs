using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using XpTracker.Core;

namespace XpTracker.App;

/// <summary>
/// Icon in the Windows notification area: a left click opens the dashboard, the menu has the dashboard and Exit.
/// </summary>
internal sealed class TrayIcon : IDisposable
{
    private readonly NotifyIcon _icon;
    private readonly ContextMenuStrip _menu = new();
    private readonly ToolStripMenuItem _dashboard = new();
    private readonly ToolStripMenuItem _exit = new();
    private readonly Icon _image = CreateImage();

    public TrayIcon(Action openDashboard, Action exit)
    {
        _dashboard.Font = new Font(_dashboard.Font, System.Drawing.FontStyle.Bold);
        _dashboard.Click += (_, _) => openDashboard();
        _exit.Click += (_, _) => exit();
        _menu.Items.Add(_dashboard);
        _menu.Items.Add(new ToolStripSeparator());
        _menu.Items.Add(_exit);

        _icon = new NotifyIcon
        {
            Icon = _image,
            Text = "Hearthstone XP Tracker",
            ContextMenuStrip = _menu,
        };
        _icon.MouseClick += (_, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                openDashboard();
            }
        };

        UpdateTexts();
        _icon.Visible = true;
    }

    /// <summary>Sets the menu texts in the current language.</summary>
    public void UpdateTexts()
    {
        _dashboard.Text = Localizer.Text("app.dashboard");
        _exit.Text = Localizer.Text("app.exit");
    }

    public void Dispose()
    {
        _icon.Visible = false;
        _icon.Dispose();
        _menu.Dispose();
        _image.Dispose();
    }

    // A gold coin with "XP" in the colors of the dashboard; drawn at run time, so no image file is needed.
    private static Icon CreateImage()
    {
        using var bitmap = new Bitmap(32, 32);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            using var rim = new SolidBrush(Color.FromArgb(0x22, 0x17, 0x0F));
            using var coin = new LinearGradientBrush(new Rectangle(0, 0, 32, 32), Color.FromArgb(0xF0, 0xC7, 0x5E), Color.FromArgb(0xC8, 0xA0, 0x50), 90f);
            using var text = new SolidBrush(Color.FromArgb(0x22, 0x17, 0x0F));
            using var font = new Font("Georgia", 11f, System.Drawing.FontStyle.Bold, GraphicsUnit.Pixel);
            g.FillEllipse(rim, 0, 0, 31, 31);
            g.FillEllipse(coin, 2, 2, 27, 27);
            using var format = new StringFormat { Alignment = StringAlignment.Center, LineAlignment = StringAlignment.Center };
            g.DrawString("XP", font, text, new RectangleF(0, 1, 32, 31), format);
        }

        var handle = bitmap.GetHicon();
        try
        {
            using var temporary = Icon.FromHandle(handle);
            return (Icon)temporary.Clone();
        }
        finally
        {
            DestroyIcon(handle);
        }
    }

    [DllImport("user32.dll")]
    private static extern bool DestroyIcon(IntPtr handle);
}
