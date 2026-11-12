using System;
using System.Collections.Generic;

namespace XpTracker.Core;

/// <summary>
/// Adds up the damage the local player's side deals to the opponent's side in one game, from Power.log lines.
/// </summary>
/// <remarks>
/// Every hit is logged as <c>META_DATA - Meta=DAMAGE Data=N</c> with the target in the next <c>Info[0]</c> line.
/// The source is the target's <c>LAST_AFFECTED_BY</c> tag, which the server sets in the lines after the hit.
/// The tag is only logged when it changes, so the last known value is used when the same source hits again.
/// Hits without a source (fatigue, paying health) have <c>LAST_AFFECTED_BY</c> 0 and are not counted.
/// Damage to the player's own hero or minions is not counted either. The amount is the full hit as the game reports it,
/// so a hit absorbed by armor counts, a hit on a Divine Shield gives 0.
/// </remarks>
public sealed class DamageCounter
{
    private readonly Func<int, bool> _isLocalPlayer;
    private readonly Dictionary<int, int> _controllers = new();
    private readonly Dictionary<int, int> _lastSource = new();
    private readonly List<int> _pendingTargets = new();
    private int _pendingAmount;
    private bool _readingTargets;
    private int? _describedEntity;

    /// <param name="isLocalPlayer">Tells whether a player id (1 or 2) is the local player.</param>
    public DamageCounter(Func<int, bool> isLocalPlayer)
    {
        _isLocalPlayer = isLocalPlayer;
    }

    /// <summary>Damage dealt so far; a hit is added once the lines after it named its source.</summary>
    public int Dealt { get; private set; }

    /// <summary>A player entity ("Player EntityID=2 PlayerID=1") belongs to its player.</summary>
    public void SetController(int entityId, int playerId) => _controllers[entityId] = playerId;

    /// <summary>Every GameState power line except TAG_CHANGE lines, trimmed.</summary>
    public void OnLine(string body)
    {
        if (body.StartsWith("Info[", StringComparison.Ordinal))
        {
            var equals = body.IndexOf("= ", StringComparison.Ordinal);
            if (_readingTargets && equals > 0)
            {
                var target = body.Substring(equals + 2);
                Note(target);
                if (EntityId(target) is { } id)
                {
                    _pendingTargets.Add(id);
                }
            }

            return;
        }

        _readingTargets = false;

        // Tag lines under FULL_ENTITY or SHOW_ENTITY describe that entity.
        if (body.StartsWith("tag=", StringComparison.Ordinal))
        {
            if (_describedEntity is { } entity)
            {
                var valueIndex = body.IndexOf(" value=", StringComparison.Ordinal);
                if (valueIndex > 4)
                {
                    OnTag(entity, body.Substring(4, valueIndex - 4), body.Substring(valueIndex + 7).Trim());
                }
            }

            return;
        }

        _describedEntity = null;
        if (body.StartsWith("META_DATA", StringComparison.Ordinal) || body.StartsWith("BLOCK_", StringComparison.Ordinal))
        {
            Resolve();
        }

        if (body.StartsWith("META_DATA - Meta=DAMAGE ", StringComparison.Ordinal))
        {
            _pendingAmount = IntAfter(body, "Data=") ?? 0;
            _readingTargets = _pendingAmount > 0;
        }
        else if (body.StartsWith("FULL_ENTITY - Creating ID=", StringComparison.Ordinal))
        {
            Resolve();
            _describedEntity = IntAfter(body, "ID=");
        }
        else if (body.StartsWith("SHOW_ENTITY - Updating Entity=", StringComparison.Ordinal)
                 || body.StartsWith("CHANGE_ENTITY - Updating Entity=", StringComparison.Ordinal))
        {
            Resolve();
            var entity = body.Substring(body.IndexOf("Entity=", StringComparison.Ordinal) + 7);
            Note(entity);
            _describedEntity = EntityId(entity);
        }
    }

    /// <summary>A TAG_CHANGE line, split by the tracker.</summary>
    public void OnTagChange(string entity, string tag, string value)
    {
        _readingTargets = false;
        _describedEntity = null;
        Note(entity);
        if ((tag == "LAST_AFFECTED_BY" || tag == "CONTROLLER") && EntityId(entity) is { } id)
        {
            OnTag(id, tag, value);
        }
    }

    /// <summary>Adds the last hit when the game ends right after it.</summary>
    public void Flush() => Resolve();

    private void OnTag(int entity, string tag, string value)
    {
        if (!int.TryParse(value, out var number))
        {
            return;
        }

        if (tag == "LAST_AFFECTED_BY")
        {
            _lastSource[entity] = number;
        }
        else if (tag == "CONTROLLER")
        {
            _controllers[entity] = number;
        }
    }

    private void Resolve()
    {
        _readingTargets = false;
        foreach (var target in _pendingTargets)
        {
            if (_lastSource.TryGetValue(target, out var source) && source != 0
                && _controllers.TryGetValue(source, out var sourcePlayer) && _isLocalPlayer(sourcePlayer)
                && _controllers.TryGetValue(target, out var targetPlayer) && !_isLocalPlayer(targetPlayer))
            {
                Dealt += _pendingAmount;
            }
        }

        _pendingTargets.Clear();
        _pendingAmount = 0;
    }

    // "[entityName=Argent Squire id=52 zone=PLAY zonePos=1 cardId=EX1_008 player=2]" gives entity 52 of player 2.
    private void Note(string entity)
    {
        if (entity.StartsWith("[", StringComparison.Ordinal) && EntityId(entity) is { } id
            && IntAfter(entity, " player=", fromEnd: true) is { } player)
        {
            _controllers[id] = player;
        }
    }

    private static int? EntityId(string entity)
    {
        if (entity.StartsWith("[", StringComparison.Ordinal))
        {
            return IntAfter(entity, " id=", fromEnd: true);
        }

        return int.TryParse(entity.Trim(), out var id) ? id : null;
    }

    private static int? IntAfter(string text, string key, bool fromEnd = false)
    {
        var index = fromEnd ? text.LastIndexOf(key, StringComparison.Ordinal) : text.IndexOf(key, StringComparison.Ordinal);
        if (index < 0)
        {
            return null;
        }

        var start = index + key.Length;
        var end = start;
        while (end < text.Length && text[end] >= '0' && text[end] <= '9')
        {
            end++;
        }

        return end > start && int.TryParse(text.Substring(start, end - start), out var value) ? value : null;
    }
}
