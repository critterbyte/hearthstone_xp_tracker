using System;
using System.Collections.Generic;

namespace XpTracker.Core;

/// <summary>
/// A game read completely from the log.
/// </summary>
public sealed class FinishedGame
{
    public FinishedGame(string session, DateTime firstTurn, DateTime end, GameMode mode, string format, XpClock clock, int damageDealt = 0, int heroDamageDealt = 0)
    {
        Session = session;
        FirstTurn = firstTurn;
        End = end;
        Mode = mode;
        Format = format;
        Clock = clock;
        DamageDealt = damageDealt;
        HeroDamageDealt = heroDamageDealt;
    }

    /// <summary>Damage the player's side dealt to the opponent's side, see <see cref="DamageCounter"/>.</summary>
    public int DamageDealt { get; }

    /// <summary>The part of <see cref="DamageDealt"/> that hit the opponent's hero.</summary>
    public int HeroDamageDealt { get; }

    /// <summary>Log session the game was read from (the Hearthstone_yyyy_MM_dd_HH_mm_ss folder name), or empty.</summary>
    public string Session { get; }

    public DateTime FirstTurn { get; }

    public DateTime End { get; }

    public GameMode Mode { get; }

    /// <summary>"Wild", "Standard" and so on; empty if the log did not say.</summary>
    public string Format { get; }

    public XpClock Clock { get; }
}

/// <summary>
/// Builds an <see cref="XpClock"/> per game from Hearthstone's Power.log lines, using the log timestamps.
/// </summary>
/// <remarks>
/// Only GameState lines are used; PowerTaskList lines repeat the same data with a delay.
/// A Rewind (GAME_RESET) recreates entities with plain tag lines, which are ignored, so the clock runs on.
/// Log lines carry only the time of day. A live reader sets <see cref="ReferenceTime"/>, so the date comes from the
/// system clock; without it (replaying a stored log) the date is counted up from the session date.
/// </remarks>
public sealed class PowerLogTracker
{
    // The server lowers a player's next turn limit to this value when a turn ran out without any action.
    public const int AfkTurnLimit = 10;

    // A line may be a moment newer than the reference time that was taken before it was read.
    private static readonly TimeSpan ReferenceTolerance = TimeSpan.FromMinutes(1);

    private const string UnknownPlayerName = "UNKNOWN HUMAN PLAYER";

    private readonly DateTime _sessionDate;
    private readonly string _session;
    private int _dayOffset;
    private TimeSpan? _lastTimeOfDay;
    private Game? _game;

    /// <param name="sessionDate">
    /// Date the log file was started; log lines only carry the time of day. Used while no <see cref="ReferenceTime"/> is set.
    /// </param>
    /// <param name="session">Name of the log session, passed on to finished games.</param>
    public PowerLogTracker(DateTime sessionDate, string session = "")
    {
        _sessionDate = sessionDate.Date;
        _session = session;
    }

    /// <summary>
    /// The current local time while lines are read as they are written, set by the reader before each batch of lines.
    /// Each line then gets the latest date on which its time of day is not after this time. That is right for every
    /// line written within the last 24 hours, also when the session started on an earlier day or paused for many hours.
    /// Null when a stored log is replayed: the date then counts up from the session date at each midnight in the log.
    /// </summary>
    public DateTime? ReferenceTime { get; set; }

    public event Action<FinishedGame>? GameFinished;

    public XpClock? Clock => _game?.Clock;

    public GameMode Mode => _game?.Mode ?? GameMode.Other;

    /// <summary>Damage the player's side dealt to the opponent's side in the current or last game.</summary>
    public int DamageDealt => _game?.Damage?.Dealt ?? 0;

    /// <summary>The part of <see cref="DamageDealt"/> that hit the opponent's hero.</summary>
    public int HeroDamageDealt => _game?.Damage?.DealtToHero ?? 0;

    public long LinesProcessed { get; private set; }

    public void ProcessLine(string line)
    {
        LinesProcessed++;

        // Format: "D HH:mm:ss.fffffff <Source> - <Body>"
        if (line.Length < 20 || line[0] != 'D' || line[1] != ' ')
        {
            return;
        }

        var sourceStart = line.IndexOf(' ', 2) + 1;
        var sourceEnd = line.IndexOf(" - ", 19, StringComparison.Ordinal);
        if (sourceStart <= 0 || sourceEnd < 0 || string.CompareOrdinal(line, sourceStart, "GameState.", 0, 10) != 0)
        {
            return;
        }

        var source = line.Substring(sourceStart, sourceEnd - sourceStart);
        var body = line.Substring(sourceEnd + 3).Trim();

        if (!TryParseTime(line, out var time))
        {
            return;
        }

        switch (source)
        {
            case "GameState.DebugPrintPower()":
                OnPower(body, time);
                break;
            case "GameState.DebugPrintGame()":
                OnGameInfo(body);
                break;
            case "GameState.DebugPrintEntityChoices()":
                OnEntityChoices(body);
                break;
            case "GameState.SendChoices()":
                OnSendChoices(body);
                break;
        }
    }

    private void OnPower(string body, DateTime time)
    {
        if (body == "CREATE_GAME")
        {
            _game = new Game();
            _game.Damage = new DamageCounter(playerId => SideOf(playerId) == Side.Player);
            return;
        }

        if (_game is null)
        {
            return;
        }

        if (!body.StartsWith("TAG_CHANGE Entity=", StringComparison.Ordinal))
        {
            _game.Damage!.OnLine(body);
        }

        if (body.StartsWith("Player EntityID=", StringComparison.Ordinal))
        {
            var parts = body.Split(' ');
            if (parts.Length >= 3 && TryValue(parts[1], "EntityID=", out var entityId) && TryValue(parts[2], "PlayerID=", out var playerId))
            {
                _game.EntityToPlayer[entityId] = playerId;
                _game.Damage!.SetController(entityId, playerId);
            }

            // Only the computer plays without an account (the Innkeeper in PvE Tavern Brawls, practice, adventures).
            if (body.Contains("GameAccountId=[hi=0 lo=0]"))
            {
                _game.AgainstComputer = true;
            }

            return;
        }

        if (!body.StartsWith("TAG_CHANGE Entity=", StringComparison.Ordinal))
        {
            return;
        }

        var tagIndex = body.LastIndexOf(" tag=", StringComparison.Ordinal);
        var valueIndex = body.LastIndexOf(" value=", StringComparison.Ordinal);
        if (tagIndex < 0 || valueIndex < tagIndex)
        {
            return;
        }

        var entity = body.Substring(18, tagIndex - 18);
        var tag = body.Substring(tagIndex + 5, valueIndex - tagIndex - 5);
        var value = body.Substring(valueIndex + 7).Trim();
        OnTagChange(entity, tag, value, time);
    }

    private void OnTagChange(string entity, string tag, string value, DateTime time)
    {
        var game = _game!;
        game.Damage!.OnTagChange(entity, tag, value);
        if (entity == "GameEntity")
        {
            if (tag == "STEP" && value == "BEGIN_MULLIGAN")
            {
                game.Clock.StartMulligan(time);
            }
            else if (tag == "STEP" && value == "MAIN_READY" && !game.Clock.HasStarted && game.CurrentPlayer is { } first)
            {
                game.FirstTurn = time;
                game.Clock.StartTurn(SideOf(first), time);
                game.CurrentSide = SideOf(first);
            }
            else if (tag == "STATE" && value == "COMPLETE" && game.Clock.HasStarted && !game.Clock.IsOver)
            {
                game.Clock.EndGame(time, game.Result);
                game.Damage!.Flush();
                GameFinished?.Invoke(new FinishedGame(_session, game.FirstTurn ?? time, time, game.Mode, game.Format, game.Clock, game.Damage.Dealt, game.Damage.DealtToHero));
            }

            return;
        }

        if (tag != "TIMEOUT" && tag != "CURRENT_PLAYER" && tag != "PLAYSTATE")
        {
            return;
        }

        var player = ResolvePlayer(entity);
        if (player is not { } playerId)
        {
            return;
        }

        switch (tag)
        {
            case "TIMEOUT":
                if (int.TryParse(value, out var limit))
                {
                    game.TurnLimits[playerId] = limit;
                }

                break;

            case "CURRENT_PLAYER":
                if (value == "1")
                {
                    game.CurrentPlayer = playerId;
                    OnTurnChange(SideOf(playerId), time);
                }

                break;

            case "PLAYSTATE":
                if (playerId == game.LocalPlayer)
                {
                    game.Result = value switch
                    {
                        "WON" => GameResult.Win,
                        "LOST" or "CONCEDED" => GameResult.Loss,
                        "TIED" => GameResult.Tie,
                        _ => game.Result,
                    };
                }

                break;
        }
    }

    private void OnTurnChange(Side side, DateTime time)
    {
        var game = _game!;
        if (!game.Clock.HasStarted || game.CurrentSide == side)
        {
            return;
        }

        // The server sets the player's next turn limit to 10 seconds when the turn ran out without any action,
        // and keeps it there while the player idles. The change is logged before the turn changes.
        if (game.CurrentSide == Side.Player && game.LocalPlayer is { } local
            && game.TurnLimits.TryGetValue(local, out var limit) && limit == AfkTurnLimit)
        {
            game.Clock.MarkCurrentTurnAfk();
        }

        game.Clock.StartTurn(side, time);
        game.CurrentSide = side;
    }

    private void OnGameInfo(string body)
    {
        if (_game is null)
        {
            return;
        }

        if (body.StartsWith("GameType=", StringComparison.Ordinal))
        {
            _game.Type = body.Substring(9).Trim() switch
            {
                "GT_RANKED" => GameMode.Ranked,
                "GT_CASUAL" => GameMode.Casual,
                "GT_TAVERNBRAWL" => GameMode.TavernBrawl,
                "GT_ARENA" => GameMode.Arena,
                _ => GameMode.Other,
            };
        }
        else if (body.StartsWith("FormatType=FT_", StringComparison.Ordinal))
        {
            // FT_WILD -> Wild, FT_STANDARD -> Standard
            var format = body.Substring(14).Trim();
            _game.Format = format.Length == 0 ? string.Empty : format.Substring(0, 1) + format.Substring(1).ToLowerInvariant();
        }
        else if (body.StartsWith("PlayerID=", StringComparison.Ordinal))
        {
            var comma = body.IndexOf(", PlayerName=", StringComparison.Ordinal);
            if (comma > 0 && int.TryParse(body.Substring(9, comma - 9), out var playerId))
            {
                _game.Names[playerId] = body.Substring(comma + 13).Trim();
            }
        }
    }

    private void OnEntityChoices(string body)
    {
        // "id=1 Player=Name TaskList=..." maps a choice id to a player name.
        var playerIndex = body.IndexOf(" Player=", StringComparison.Ordinal);
        var taskIndex = body.IndexOf(" TaskList=", StringComparison.Ordinal);
        if (_game is null || !body.StartsWith("id=", StringComparison.Ordinal) || playerIndex < 0 || taskIndex < playerIndex)
        {
            return;
        }

        if (int.TryParse(body.Substring(3, playerIndex - 3), out var choiceId))
        {
            _game.ChoiceNames[choiceId] = body.Substring(playerIndex + 8, taskIndex - playerIndex - 8).Trim();
        }
    }

    private void OnSendChoices(string body)
    {
        // Only the local client sends its mulligan choice, which identifies the local player.
        var typeIndex = body.IndexOf(" ChoiceType=MULLIGAN", StringComparison.Ordinal);
        if (_game is null || !body.StartsWith("id=", StringComparison.Ordinal) || typeIndex < 0
            || !int.TryParse(body.Substring(3, typeIndex - 3), out var choiceId))
        {
            return;
        }

        if (_game.ChoiceNames.TryGetValue(choiceId, out var name))
        {
            foreach (var pair in _game.Names)
            {
                if (pair.Value == name)
                {
                    _game.LocalPlayer = pair.Key;
                    return;
                }
            }
        }

        _game.LocalPlayer = choiceId;
    }

    private int? ResolvePlayer(string entity)
    {
        var game = _game!;
        if (int.TryParse(entity, out var entityId))
        {
            return game.EntityToPlayer.TryGetValue(entityId, out var byId) ? byId : null;
        }

        foreach (var pair in game.Names)
        {
            if (pair.Value == entity)
            {
                return pair.Key;
            }
        }

        // The opponent's name is hidden as "UNKNOWN HUMAN PLAYER" until it is revealed.
        if (entity.StartsWith("[", StringComparison.Ordinal) || game.LocalPlayer is not { } local)
        {
            return null;
        }

        var opponent = local == 1 ? 2 : 1;
        game.Names[opponent] = entity;
        return opponent;
    }

    private Side SideOf(int playerId)
    {
        var game = _game!;
        if (game.LocalPlayer is null)
        {
            // Fallback when no mulligan choice was logged: the local name is the one that is not hidden.
            foreach (var pair in game.Names)
            {
                if (pair.Value != UnknownPlayerName && game.Names.Count == 2)
                {
                    game.LocalPlayer = pair.Key;
                    break;
                }
            }
        }

        return playerId == game.LocalPlayer ? Side.Player : Side.Opponent;
    }

    private bool TryParseTime(string line, out DateTime time)
    {
        time = default;
        if (line[4] != ':' || line[7] != ':' || line[10] != '.')
        {
            return false;
        }

        var hours = (line[2] - '0') * 10 + (line[3] - '0');
        var minutes = (line[5] - '0') * 10 + (line[6] - '0');
        var seconds = (line[8] - '0') * 10 + (line[9] - '0');
        long ticks = 0;
        var digits = 0;
        for (var i = 11; i < line.Length && digits < 7 && line[i] >= '0' && line[i] <= '9'; i++, digits++)
        {
            ticks = ticks * 10 + (line[i] - '0');
        }

        for (; digits < 7; digits++)
        {
            ticks *= 10;
        }

        var timeOfDay = new TimeSpan(hours, minutes, seconds) + TimeSpan.FromTicks(ticks);
        if (ReferenceTime is { } reference)
        {
            var latest = reference + ReferenceTolerance;
            time = latest.Date + timeOfDay;
            if (time > latest)
            {
                time = time.AddDays(-1);
            }

            return true;
        }

        if (_lastTimeOfDay is { } last && timeOfDay < last - TimeSpan.FromHours(12))
        {
            _dayOffset++;
        }

        _lastTimeOfDay = timeOfDay;
        time = _sessionDate.AddDays(_dayOffset) + timeOfDay;
        return true;
    }

    private static bool TryValue(string part, string prefix, out int value)
    {
        value = 0;
        return part.StartsWith(prefix, StringComparison.Ordinal) && int.TryParse(part.Substring(prefix.Length), out value);
    }

    private sealed class Game
    {
        public XpClock Clock { get; } = new();

        /// <summary>Set right after the game is created; needs the tracker to tell the local player.</summary>
        public DamageCounter? Damage { get; set; }

        /// <summary>Mode from the log's GameType line.</summary>
        public GameMode Type { get; set; } = GameMode.Other;

        /// <summary>The opponent has no account, so it is the computer.</summary>
        public bool AgainstComputer { get; set; }

        /// <summary>A Tavern Brawl against the computer pays differently from one against a player.</summary>
        public GameMode Mode => Type == GameMode.TavernBrawl && AgainstComputer ? GameMode.TavernBrawlPvE : Type;

        public string Format { get; set; } = string.Empty;

        public GameResult Result { get; set; }

        public DateTime? FirstTurn { get; set; }

        public int? LocalPlayer { get; set; }

        public int? CurrentPlayer { get; set; }

        public Side? CurrentSide { get; set; }

        public Dictionary<int, int> EntityToPlayer { get; } = new();

        public Dictionary<int, string> Names { get; } = new();

        public Dictionary<int, string> ChoiceNames { get; } = new();

        public Dictionary<int, int> TurnLimits { get; } = new();
    }
}
