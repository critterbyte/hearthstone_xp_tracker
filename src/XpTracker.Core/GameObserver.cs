using System;

namespace XpTracker.Core;

/// <summary>
/// Turns periodic <see cref="GameSample"/>s into events for an <see cref="XpClock"/>, one clock per game.
/// </summary>
public sealed class GameObserver
{
    // The server lowers a player's next turn limit to this value when a turn ran out without any action.
    // It stays there while the player keeps idling and goes back up after the player acts again.
    public const int AfkTurnLimit = 10;

    private Side? _currentSide;
    private GameResult _lastResult;

    public XpClock? Clock { get; private set; }

    public GameMode Mode { get; private set; }

    public void Update(GameSample sample, DateTime now)
    {
        switch (sample.Phase)
        {
            case GamePhase.None:
                // Back in the menu before the end was seen: close the game with what is known.
                if (Clock is { HasStarted: true, IsOver: false })
                {
                    Clock.EndGame(now, _lastResult);
                }

                return;

            case GamePhase.Mulligan:
                if (Clock is null || Clock.IsOver || Clock.HasStarted)
                {
                    StartNewGame(sample.Mode);
                }

                Clock!.StartMulligan(now);
                return;

            case GamePhase.Turns:
                if (Clock is null || Clock.IsOver)
                {
                    StartNewGame(sample.Mode);
                }

                TrackTurn(sample, now);
                return;

            case GamePhase.Over:
                if (sample.Result != GameResult.Unknown)
                {
                    _lastResult = sample.Result;
                }

                if (Clock is { HasStarted: true, IsOver: false })
                {
                    Clock.EndGame(now, _lastResult);
                }

                return;
        }
    }

    private void TrackTurn(GameSample sample, DateTime now)
    {
        if (sample.Result != GameResult.Unknown)
        {
            _lastResult = sample.Result;
        }

        if (sample.CurrentSide is not { } side || side == _currentSide)
        {
            return;
        }

        if (_currentSide == Side.Player && sample.PlayerTurnLimit == AfkTurnLimit)
        {
            Clock!.MarkCurrentTurnAfk();
        }

        Clock!.StartTurn(side, now);
        _currentSide = side;
    }

    private void StartNewGame(GameMode mode)
    {
        Clock = new XpClock();
        Mode = mode;
        _currentSide = null;
        _lastResult = GameResult.Unknown;
    }
}
