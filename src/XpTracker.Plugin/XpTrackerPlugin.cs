using System;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.Plugins;
using XpTracker.Core;
using HdtCore = Hearthstone_Deck_Tracker.API.Core;

namespace XpTracker.Plugin;

/// <summary>
/// Entry point loaded by Hearthstone Deck Tracker.
/// </summary>
public class XpTrackerPlugin : IPlugin
{
    private readonly GameObserver _observer = new();
    private XpOverlay? _overlay;
    private bool _enabled;

    public string Name => "Hearthstone XP Tracker";

    public string Description => "Shows the reward track XP of the current game and the XP time it is based on.";

    public string ButtonText => "Settings";

    public string Author => "critterbyte";

    public Version Version => typeof(XpTrackerPlugin).Assembly.GetName().Version;

    // HDT documents null as "no menu entry", but declares the property as non-nullable.
    public MenuItem MenuItem => null!;

    public void OnLoad()
    {
        _overlay = new XpOverlay();
        HdtCore.OverlayCanvas.Children.Add(_overlay);
        _enabled = true;
    }

    public void OnUnload()
    {
        _enabled = false;
        if (_overlay is not null)
        {
            HdtCore.OverlayCanvas.Children.Remove(_overlay);
            _overlay = null;
        }
    }

    public void OnButtonPress()
    {
    }

    // Called by HDT about every 100 ms.
    public void OnUpdate()
    {
        if (!_enabled || _overlay is null)
        {
            return;
        }

        var now = DateTime.Now;
        _observer.Update(HdtGameReader.Read(), now);
        _overlay.Show(XpDisplayText.For(_observer, now));

        var canvas = HdtCore.OverlayCanvas;
        _overlay.Place(canvas.ActualWidth, canvas.ActualHeight);
    }
}
