using System.Globalization;
using System.Text.RegularExpressions;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

// Switching the language changes global state, so these tests never run in parallel with others.
[CollectionDefinition("Language", DisableParallelization = true)]
public class LanguageCollection
{
}

public class TranslationFileTests
{
    private static readonly Regex Placeholder = new(@"\{\d+\}");

    public static TheoryData<string> Translations()
    {
        var data = new TheoryData<string>();
        foreach (var code in Localizer.Languages.Where(c => c != Localizer.DefaultLanguage))
        {
            data.Add(code);
        }

        return data;
    }

    [Theory]
    [MemberData(nameof(Translations))]
    public void Translation_HasExactlyTheEnglishKeys(string code)
    {
        var english = Localizer.TextsOf("en")!;
        var translation = Localizer.TextsOf(code)!;

        Assert.Empty(english.Keys.Except(translation.Keys));
        Assert.Empty(translation.Keys.Except(english.Keys));
    }

    [Theory]
    [MemberData(nameof(Translations))]
    public void Translation_KeepsThePlaceholders(string code)
    {
        var english = Localizer.TextsOf("en")!;
        var translation = Localizer.TextsOf(code)!;
        var wrong = english.Keys
            .Where(translation.ContainsKey)
            .Where(k => !Placeholders(english[k]).SetEquals(Placeholders(translation[k])))
            .ToList();

        Assert.Empty(wrong);
    }

    [Theory]
    [MemberData(nameof(Translations))]
    public void Translation_HasNoEmptyTexts(string code)
    {
        Assert.Empty(Localizer.TextsOf(code)!.Where(p => p.Value.Trim().Length == 0).Select(p => p.Key));
    }

    [Fact]
    public void EveryEmbeddedLanguage_IsAClientLanguage()
    {
        var client = Localizer.ClientLanguages.Select(l => l.Key).ToHashSet();

        Assert.All(Localizer.Languages, code => Assert.Contains(code, client));
    }

    [Fact]
    public void Selection_ListsEmbeddedLanguagesInClientOrder()
    {
        var selectable = Localizer.SelectableLanguages().Select(l => l.Key).ToList();

        Assert.Contains("en", selectable);
        Assert.Contains("de", selectable);
        Assert.True(selectable.IndexOf("de") < selectable.IndexOf("en"));
        Assert.Equal(Localizer.Languages.Count, selectable.Count);
    }

    [Fact]
    public void Settings_RememberTheLanguage_EnglishByDefault()
    {
        Assert.Equal("en", XpTrackerSettings.Parse(Array.Empty<string>()).Language);
        Assert.Equal("de", XpTrackerSettings.Parse(new[] { "Language=de" }).Language);
        Assert.Equal("de", XpTrackerSettings.Parse(new[] { "Language=DE" }).Language);
        Assert.Equal("en", XpTrackerSettings.Parse(new[] { "Language=xx" }).Language);
        Assert.Contains("Language=de", new XpTrackerSettings { Language = "de" }.ToLines());
    }

    private static HashSet<string> Placeholders(string text) => Placeholder.Matches(text).Select(m => m.Value).ToHashSet();
}

[Collection("Language")]
public class GermanTextTests : IDisposable
{
    public GermanTextTests() => Localizer.SetLanguage("de");

    public void Dispose() => Localizer.SetLanguage(Localizer.DefaultLanguage);

    [Fact]
    public void Overlay_IsGerman()
    {
        var clock = new XpClock();
        clock.StartTurn(Side.Player, At("10:00:00"));

        var lines = XpDisplayText.For(clock, GameMode.Ranked, At("10:06:12"), eventActive: true, damageDealt: 47, heroDamageDealt: 21);

        Assert.Equal(
            new[] { "EP-Zeit: 6:12", "Bei Sieg: +49", "Bei Niederlage: +35", "Event-EP: +6", "Gesamtschaden: 47", "Heldenschaden: 21" },
            lines.Select(l => l.ToString()));
    }

    [Fact]
    public void Summary_UsesGermanResultAndTerms()
    {
        var clock = new XpClock();
        clock.StartTurn(Side.Player, At("10:00:00"));
        clock.EndGame(At("10:05:00"), GameResult.Loss);

        var lines = XpDisplayText.For(clock, GameMode.Ranked, At("10:06:00")).Select(l => l.ToString()).ToList();

        Assert.Equal("Letzte Partie: Niederlage", lines[0]);
        Assert.Equal("Erhalten: +28", lines[3]);
    }

    [Fact]
    public void GameTerms_AndNumbers_AreGerman()
    {
        Assert.Equal("de", Localizer.Language);
        Assert.Equal("Gewertet", GameText.Mode(GameMode.Ranked));
        Assert.Equal("Kampf in der Taverne", GameText.Mode(GameMode.TavernBrawl));
        Assert.Equal("Todesritter", GameText.Class("Deathknight"));
        Assert.Equal("Dämonenjäger", GameText.Class("DemonHunter"));
        Assert.Equal("15 % EP-Bonus, noch 50 Stufen bis 20 %", TavernPassBonus.Describe(true, 20));
        Assert.Equal("Fr", new DateTime(2026, 11, 13).ToString("ddd", Localizer.Culture));
    }

    [Fact]
    public void UnknownLanguage_SwitchesToEnglish()
    {
        Localizer.SetLanguage("xx");

        Assert.Equal("en", Localizer.Language);
        Assert.Equal("XP time:", Localizer.Text("overlay.xpTime"));
    }

    private static DateTime At(string time) =>
        DateTime.ParseExact("2026-11-08 " + time, "yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture);
}
