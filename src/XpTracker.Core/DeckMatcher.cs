using System;
using System.Collections.Generic;
using System.Linq;

namespace XpTracker.Core;

/// <summary>
/// Deck and classes of a game as recorded by Hearthstone Deck Tracker.
/// </summary>
public sealed class DeckInfo
{
    public DeckInfo(DateTime start, DateTime end, string deck, string playerClass, string opponentClass)
    {
        Start = start;
        End = end;
        Deck = deck;
        PlayerClass = playerClass;
        OpponentClass = opponentClass;
    }

    public DateTime Start { get; }

    public DateTime End { get; }

    public string Deck { get; }

    public string PlayerClass { get; }

    public string OpponentClass { get; }
}

/// <summary>
/// Finds the HDT game that belongs to a history record, to fill in deck and classes.
/// </summary>
/// <remarks>
/// Games are matched by their end time. The start is not reliable: HDT starts its record when it
/// first sees the game, which is later if HDT was started during the game or after a Rewind.
/// </remarks>
public static class DeckMatcher
{
    public static readonly TimeSpan EndTolerance = TimeSpan.FromSeconds(60);

    public static DeckInfo? Find(GameRecord game, IEnumerable<DeckInfo> candidates) =>
        candidates
            .Where(c => c.Start <= game.End && Math.Abs((c.End - game.End).TotalSeconds) <= EndTolerance.TotalSeconds)
            .OrderBy(c => Math.Abs((c.End - game.End).TotalSeconds))
            .FirstOrDefault();

    /// <summary>Fills deck and classes of records that have no deck yet.</summary>
    /// <returns>Number of records that were changed.</returns>
    public static int FillMissing(IEnumerable<GameRecord> games, IReadOnlyList<DeckInfo> candidates)
    {
        var changed = 0;
        foreach (var game in games.Where(g => g.Deck.Length == 0))
        {
            var match = Find(game, candidates);
            if (match is null || match.Deck.Length == 0)
            {
                continue;
            }

            game.Deck = match.Deck;
            if (game.PlayerClass.Length == 0)
            {
                game.PlayerClass = match.PlayerClass;
            }

            if (game.OpponentClass.Length == 0)
            {
                game.OpponentClass = match.OpponentClass;
            }

            changed++;
        }

        return changed;
    }
}
