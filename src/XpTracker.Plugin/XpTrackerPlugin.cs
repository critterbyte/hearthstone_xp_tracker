using System;
using System.Windows.Controls;
using Hearthstone_Deck_Tracker.Plugins;

namespace XpTracker.Plugin;

/// <summary>
/// Entry point loaded by Hearthstone Deck Tracker. Overlay and game tracking follow in later versions.
/// </summary>
public class XpTrackerPlugin : IPlugin
{
    public string Name => "Hearthstone XP Tracker";

    public string Description => "Shows the reward track XP of the current game and the XP time it is based on.";

    public string ButtonText => "Settings";

    public string Author => "critterbyte";

    public Version Version => typeof(XpTrackerPlugin).Assembly.GetName().Version;

    // HDT documents null as "no menu entry", but declares the property as non-nullable.
    public MenuItem MenuItem => null!;

    public void OnLoad()
    {
    }

    public void OnUnload()
    {
    }

    public void OnButtonPress()
    {
    }

    public void OnUpdate()
    {
    }
}
