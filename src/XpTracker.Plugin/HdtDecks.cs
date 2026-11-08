using System.Collections.Generic;
using System.Linq;
using Hearthstone_Deck_Tracker.Stats;
using XpTracker.Core;
using HdtCore = Hearthstone_Deck_Tracker.API.Core;
using HdtDeckStats = Hearthstone_Deck_Tracker.Stats.DeckStats;

namespace XpTracker.Plugin;

/// <summary>
/// Reads deck names and classes from Hearthstone Deck Tracker's own game statistics. Call on the UI thread.
/// </summary>
internal static class HdtDecks
{
    public static IReadOnlyList<DeckInfo> All()
    {
        var stats = DeckStatsList.Instance.DeckStats.Values.Concat(DefaultDeckStats.Instance.DeckStats ?? new List<HdtDeckStats>());
        return stats
            .SelectMany(d => d.Games ?? new List<GameStats>())
            .Select(g => new DeckInfo(g.StartTime, g.EndTime, Clean(g.DeckName), g.PlayerHero ?? string.Empty, g.OpponentHero ?? string.Empty))
            .ToList();
    }

    /// <summary>Deck of the game HDT is tracking right now, or empty.</summary>
    public static string Current() =>
        Clean(HdtCore.Game.CurrentGameStats?.DeckName) is { Length: > 0 } deck ? deck : Clean(Hearthstone_Deck_Tracker.DeckList.Instance.ActiveDeck?.Name);

    // HDT reports "none" when a game has no deck assigned.
    private static string Clean(string? name) => string.IsNullOrEmpty(name) || name == "none" ? string.Empty : name!;
}
