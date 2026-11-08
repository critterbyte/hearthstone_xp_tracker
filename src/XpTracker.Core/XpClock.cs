using System;
using System.Collections.Generic;

namespace XpTracker.Core;

/// <summary>
/// Tracks the XP time of one game from turn events.
/// </summary>
/// <remarks>
/// XP time starts with the first turn. An own turn that ends by timeout without any action
/// (an AFK turn) does not count, and neither does the opponent turn that follows it.
/// The caller reports AFK turns with <see cref="MarkCurrentTurnAfk"/> before the next turn starts:
/// the server signals them by lowering the player's next turn limit to 10 seconds, and a turn
/// played under that limit without any action is AFK as well.
/// </remarks>
public sealed class XpClock
{
    private readonly List<Turn> _turns = new();
    private DateTime? _mulliganStart;
    private DateTime? _gameEnd;

    public GameResult Result { get; private set; }

    public bool HasStarted => _turns.Count > 0;

    public bool IsOver => _gameEnd.HasValue;

    public int AfkTurns { get; private set; }

    public void StartMulligan(DateTime time)
    {
        if (_mulliganStart is null && !HasStarted)
        {
            _mulliganStart = time;
        }
    }

    public void StartTurn(Side side, DateTime time)
    {
        if (IsOver)
        {
            return;
        }

        CloseCurrentTurn(time);
        _turns.Add(new Turn(side, time));
    }

    public void MarkCurrentTurnAfk()
    {
        if (IsOver || !HasStarted)
        {
            return;
        }

        var current = _turns[_turns.Count - 1];
        if (current.Side == Side.Player && !current.IsAfk)
        {
            current.IsAfk = true;
            AfkTurns++;
        }
    }

    public void EndGame(DateTime time, GameResult result)
    {
        if (IsOver)
        {
            return;
        }

        CloseCurrentTurn(time);
        _gameEnd = time;
        Result = result;
    }

    public XpSnapshot GetSnapshot(DateTime now)
    {
        var end = _gameEnd ?? now;
        double playSeconds = 0;
        double xpSeconds = 0;

        for (var i = 0; i < _turns.Count; i++)
        {
            var turn = _turns[i];
            var length = Seconds(turn.Start, turn.End ?? end);
            playSeconds += length;
            if (!IsExcluded(i))
            {
                xpSeconds += length;
            }
        }

        var withMulligan = _mulliganStart.HasValue && HasStarted ? Seconds(_mulliganStart.Value, end) : playSeconds;
        var isPaused = !IsOver && HasStarted && IsExcluded(_turns.Count - 1);
        return new XpSnapshot(xpSeconds, playSeconds, withMulligan, isPaused);
    }

    private bool IsExcluded(int index)
    {
        var turn = _turns[index];
        if (turn.Side == Side.Player)
        {
            return turn.IsAfk;
        }

        return index > 0 && _turns[index - 1].Side == Side.Player && _turns[index - 1].IsAfk;
    }

    private void CloseCurrentTurn(DateTime time)
    {
        if (HasStarted && _turns[_turns.Count - 1].End is null)
        {
            _turns[_turns.Count - 1].End = time;
        }
    }

    private static double Seconds(DateTime from, DateTime to) => Math.Max(0, (to - from).TotalSeconds);

    private sealed class Turn
    {
        public Turn(Side side, DateTime start)
        {
            Side = side;
            Start = start;
        }

        public Side Side { get; }

        public DateTime Start { get; }

        public DateTime? End { get; set; }

        public bool IsAfk { get; set; }
    }
}
