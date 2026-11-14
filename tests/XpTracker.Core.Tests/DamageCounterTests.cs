using XpTracker.Core;
using static XpTracker.Core.Tests.PowerLogTrackerTests;

namespace XpTracker.Core.Tests;

// Lines shortened from real Power.log games of November 2026. The local player is player 1 (Alice).
public class DamageCounterTests
{
    private const string MyMinion = "[entityName=Argent Squire id=52 zone=PLAY zonePos=1 cardId=EX1_008 player=1]";
    private const string MyHero = "[entityName=Uther Lightbringer id=64 zone=PLAY zonePos=0 cardId=HERO_04 player=1]";
    private const string EnemyHero = "[entityName=Metamorphosis Illidan id=70 zone=PLAY zonePos=0 cardId=HERO_10am player=2]";
    private const string EnemyMinion = "[entityName=Shattershard Turret id=51 zone=PLAY zonePos=4 cardId=GDB_105 player=2]";

    [Fact]
    public void OwnMinionAttackingTheEnemyHero_Counts()
    {
        var tracker = Started();
        Feed(tracker, Attack(MyMinion, EnemyHero));
        Feed(tracker, Hit(EnemyHero, 3, source: 52, damageAfter: 3));
        Feed(tracker, Power("12:00:10", "BLOCK_END"));

        Assert.Equal(3, tracker.DamageDealt);
        Assert.Equal(3, tracker.HeroDamageDealt);
    }

    [Fact]
    public void SameSourceAgain_CountsAlthoughTheSourceIsNotLoggedAgain()
    {
        var tracker = Started();
        Feed(tracker, Hit(EnemyHero, 1, source: 52, damageAfter: 1));
        Feed(tracker, Hit(EnemyHero, 2, source: null, damageAfter: 3));
        Feed(tracker, Power("12:00:10", "BLOCK_END"));

        Assert.Equal(3, tracker.DamageDealt);
        Assert.Equal(3, tracker.HeroDamageDealt);
    }

    [Fact]
    public void HeroDamage_CountsOnlyHitsOnTheEnemyHero()
    {
        var tracker = Started();
        Feed(tracker, Hit(EnemyMinion, 2, source: 52, damageAfter: 2));
        Feed(tracker, Hit(EnemyHero, 5, source: 52, damageAfter: 5));
        Feed(tracker, Hit(MyHero, 4, source: 51, damageAfter: 4));
        Feed(tracker, Power("12:00:10", "BLOCK_END"));

        Assert.Equal(7, tracker.DamageDealt);
        Assert.Equal(5, tracker.HeroDamageDealt);
    }

    [Fact]
    public void HeroCardPlayedByTheOpponent_BecomesTheHeroForHeroDamage()
    {
        // The opponent's hero card is hidden in hand and revealed with its card type when played.
        const string NewHero = "[entityName=Scourgeborne Arthas id=88 zone=PLAY zonePos=0 cardId=HERO_11p_LichKing player=2]";
        var tracker = Started();
        Feed(tracker,
            Power("12:00:05", "SHOW_ENTITY - Updating Entity=[entityName=UNKNOWN ENTITY [cardType=INVALID] id=88 zone=HAND zonePos=3 cardId= player=2] CardID=HERO_11p_LichKing"),
            Power("12:00:05", "    tag=CONTROLLER value=2"),
            Power("12:00:05", "    tag=CARDTYPE value=HERO"),
            Power("12:00:05", "BLOCK_END"));
        Feed(tracker, Hit(NewHero, 6, source: 52, damageAfter: 6));
        Feed(tracker, Power("12:00:10", "BLOCK_END"));

        Assert.Equal(6, tracker.HeroDamageDealt);
    }

    [Fact]
    public void MinionFight_CountsOnlyTheOwnMinionsDamage()
    {
        var tracker = Started();
        Feed(tracker, Attack(MyMinion, EnemyMinion));
        Feed(tracker, Hit(EnemyMinion, 2, source: 52, damageAfter: 2));
        Feed(tracker, Hit(MyMinion, 4, source: 51, damageAfter: 4));
        Feed(tracker, Power("12:00:10", "BLOCK_END"));

        Assert.Equal(2, tracker.DamageDealt);
        Assert.Equal(0, tracker.HeroDamageDealt);
    }

    [Fact]
    public void OpponentDamage_DoesNotCount()
    {
        var tracker = Started();
        Feed(tracker, Hit(MyHero, 5, source: 51, damageAfter: 5));
        Feed(tracker, Power("12:00:10", "BLOCK_END"));

        Assert.Equal(0, tracker.DamageDealt);
    }

    [Fact]
    public void DivineShield_GivesZero()
    {
        var tracker = Started();
        Feed(tracker,
            Power("12:00:05", $"TAG_CHANGE Entity={EnemyMinion} tag=DIVINE_SHIELD value=0 "),
            Power("12:00:05", "    META_DATA - Meta=DAMAGE Data=0 InfoCount=1"),
            Power("12:00:05", $"                Info[0] = {EnemyMinion}"),
            Power("12:00:05", "BLOCK_END"));

        Assert.Equal(0, tracker.DamageDealt);
    }

    [Fact]
    public void DamageToOwnSide_DoesNotCount()
    {
        var tracker = Started();
        Feed(tracker, Hit(MyMinion, 2, source: 64, damageAfter: 2));
        Feed(tracker, Power("12:00:10", "BLOCK_END"));

        Assert.Equal(0, tracker.DamageDealt);
    }

    [Fact]
    public void DamageWithoutSource_DoesNotCount()
    {
        // Fatigue and paying health set LAST_AFFECTED_BY to 0.
        var tracker = Started();
        Feed(tracker, Hit(EnemyHero, 4, source: 52, damageAfter: 4));
        Feed(tracker, Hit(EnemyHero, 1, source: 0, damageAfter: 5));
        Feed(tracker, Power("12:00:10", "BLOCK_END"));

        Assert.Equal(4, tracker.DamageDealt);
    }

    [Fact]
    public void CreatedEntities_AndBareIds_AreResolved()
    {
        var tracker = Started();
        Feed(tracker,
            Power("12:00:05", "FULL_ENTITY - Creating ID=90 CardID=EX1_116t"),
            Power("12:00:05", "    tag=CONTROLLER value=1"),
            Power("12:00:05", "    tag=ZONE value=PLAY"),
            Power("12:00:05", "FULL_ENTITY - Creating ID=91 CardID=CS2_101t"),
            Power("12:00:05", "    tag=CONTROLLER value=2"),
            Power("12:00:06", "    META_DATA - Meta=DAMAGE Data=2 InfoCount=1"),
            Power("12:00:06", "                Info[0] = 91"),
            Power("12:00:06", "    TAG_CHANGE Entity=91 tag=LAST_AFFECTED_BY value=90 "),
            Power("12:00:06", "    TAG_CHANGE Entity=91 tag=DAMAGE value=2 "),
            Power("12:00:06", "BLOCK_END"));

        Assert.Equal(2, tracker.DamageDealt);
    }

    [Fact]
    public void MindControl_ChangesTheSide()
    {
        var tracker = Started();
        Feed(tracker,
            Power("12:00:05", $"TAG_CHANGE Entity={EnemyMinion} tag=CONTROLLER value=1 "),
            Power("12:00:05", "BLOCK_END"));
        Feed(tracker, Hit(EnemyHero, 3, source: 51, damageAfter: 3));
        Feed(tracker, Power("12:00:10", "BLOCK_END"));

        Assert.Equal(3, tracker.DamageDealt);
    }

    [Fact]
    public void FinishedGame_CarriesTheDamage_AndANewGameStartsAtZero()
    {
        var finished = new List<FinishedGame>();
        var tracker = Started();
        tracker.GameFinished += finished.Add;
        Feed(tracker, Hit(EnemyHero, 30, source: 52, damageAfter: 30));
        Feed(tracker,
            Power("12:05:00", "TAG_CHANGE Entity=Alice tag=PLAYSTATE value=WON"),
            Power("12:05:00", "TAG_CHANGE Entity=GameEntity tag=STATE value=COMPLETE"));

        var game = Assert.Single(finished);
        Assert.Equal(30, game.DamageDealt);
        Assert.Equal(30, game.HeroDamageDealt);
        Assert.Equal(30, tracker.DamageDealt);

        Feed(tracker, GameStart("12:10:00", "GT_RANKED", localStarts: true));
        Assert.Equal(0, tracker.DamageDealt);
        Assert.Equal(0, tracker.HeroDamageDealt);
    }

    // A started game with both heroes and one minion per side on the board. Heroes are created with their card type, as in the log.
    private static PowerLogTracker Started()
    {
        var tracker = StartedRankedGame("12:00:00");
        Feed(tracker,
            Power("12:00:00", "FULL_ENTITY - Creating ID=64 CardID=HERO_04"),
            Power("12:00:00", "    tag=CONTROLLER value=1"),
            Power("12:00:00", "    tag=CARDTYPE value=HERO"),
            Power("12:00:00", "FULL_ENTITY - Creating ID=70 CardID=HERO_10am"),
            Power("12:00:00", "    tag=CONTROLLER value=2"),
            Power("12:00:00", "    tag=CARDTYPE value=HERO"),
            Power("12:00:00", "FULL_ENTITY - Creating ID=51 CardID=GDB_105"),
            Power("12:00:00", "    tag=CONTROLLER value=2"),
            Power("12:00:00", "    tag=CARDTYPE value=MINION"));
        foreach (var entity in new[] { MyHero, MyMinion, EnemyHero, EnemyMinion })
        {
            Feed(tracker, Power("12:00:01", $"TAG_CHANGE Entity={entity} tag=ZONE value=PLAY "));
        }

        return tracker;
    }

    private static string[] Attack(string attacker, string defender) => new[]
    {
        Power("12:00:05", $"BLOCK_START BlockType=ATTACK Entity={attacker} EffectCardId=System.Collections.Generic.List`1[System.String] EffectIndex=0 Target={defender} SubOption=-1 "),
        Power("12:00:05", $"    TAG_CHANGE Entity={attacker} tag=ATTACKING value=1 "),
        Power("12:00:05", "    META_DATA - Meta=SLUSH_TIME Data=1200 InfoCount=1"),
        Power("12:00:05", $"                Info[0] = {attacker}"),
        Power("12:00:05", $"    TAG_CHANGE Entity={defender} tag=DEFENDING value=1 "),
    };

    // A hit as the server logs it: the damage record, then the source (only if it changed) and the new damage.
    private static string[] Hit(string target, int amount, int? source, int damageAfter)
    {
        var lines = new List<string>
        {
            Power("12:00:06", $"    TAG_CHANGE Entity={target} tag=PREDAMAGE value={amount} "),
            Power("12:00:06", $"    TAG_CHANGE Entity={target} tag=PREDAMAGE value=0 "),
            Power("12:00:06", $"    META_DATA - Meta=DAMAGE Data={amount} InfoCount=1"),
            Power("12:00:06", $"                Info[0] = {target}"),
            Power("12:00:06", $"    TAG_CHANGE Entity={target} tag=4741 value=1 "),
        };
        if (source is { } id)
        {
            lines.Add(Power("12:00:06", $"    TAG_CHANGE Entity={target} tag=LAST_AFFECTED_BY value={id} "));
        }

        lines.Add(Power("12:00:06", $"    TAG_CHANGE Entity={target} tag=DAMAGE value={damageAfter} "));
        return lines.ToArray();
    }

    private static void Feed(PowerLogTracker tracker, params string[] lines)
    {
        foreach (var line in lines)
        {
            tracker.ProcessLine(line);
        }
    }
}
