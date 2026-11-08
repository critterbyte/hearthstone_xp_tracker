using HearthDb.Enums;
using XpTracker.Core;
using CoreGameMode = XpTracker.Core.GameMode;
using HdtCore = Hearthstone_Deck_Tracker.API.Core;
using HdtGameMode = Hearthstone_Deck_Tracker.Enums.GameMode;

namespace XpTracker.Plugin;

/// <summary>
/// Reads the game state from Hearthstone Deck Tracker into a <see cref="GameSample"/>.
/// </summary>
internal static class HdtGameReader
{
    public static GameSample Read()
    {
        var game = HdtCore.Game;
        if (game is null || game.IsInMenu)
        {
            return GameSample.NoGame;
        }

        var gameEntity = game.GameEntity;
        var player = game.PlayerEntity;
        if (gameEntity is null || player is null)
        {
            return GameSample.NoGame;
        }

        var phase = PhaseOf((Step)gameEntity.GetTag(GameTag.STEP), (State)gameEntity.GetTag(GameTag.STATE));
        Side? side = player.GetTag(GameTag.CURRENT_PLAYER) == 1 ? Side.Player
            : game.OpponentEntity?.GetTag(GameTag.CURRENT_PLAYER) == 1 ? Side.Opponent
            : null;

        return new GameSample(
            phase,
            side,
            player.GetTag(GameTag.TIMEOUT),
            ResultOf((PlayState)player.GetTag(GameTag.PLAYSTATE)),
            ModeOf(game.CurrentGameMode));
    }

    private static GamePhase PhaseOf(Step step, State state)
    {
        if (state == State.COMPLETE || step == Step.FINAL_WRAPUP || step == Step.FINAL_GAMEOVER)
        {
            return GamePhase.Over;
        }

        if (step == Step.BEGIN_MULLIGAN)
        {
            return GamePhase.Mulligan;
        }

        // XP time starts with MAIN_READY of the first turn; earlier setup steps do not count.
        return step >= Step.MAIN_READY ? GamePhase.Turns : GamePhase.None;
    }

    private static GameResult ResultOf(PlayState playState) => playState switch
    {
        PlayState.WON => GameResult.Win,
        PlayState.LOST or PlayState.CONCEDED => GameResult.Loss,
        PlayState.TIED => GameResult.Tie,
        _ => GameResult.Unknown,
    };

    private static CoreGameMode ModeOf(HdtGameMode mode) => mode switch
    {
        HdtGameMode.Ranked => CoreGameMode.Ranked,
        HdtGameMode.Casual => CoreGameMode.Casual,
        HdtGameMode.Brawl => CoreGameMode.TavernBrawl,
        _ => CoreGameMode.Other,
    };
}
