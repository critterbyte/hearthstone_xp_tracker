using System.Text.RegularExpressions;
using XpTracker.Core;

namespace XpTracker.Core.Tests;

public class LocalizerTests
{
    // Literal keys used in the code, for example Localizer.Text("overlay.xpTime"), T("tab.games") or F("lost.afkNote", ...).
    private static readonly Regex KeyUse = new(@"(?:Localizer\.(?:Text|Format)|\bT|\bF)\(\s*""([a-zA-Z0-9]+\.[a-zA-Z0-9.]+)""");

    // Keys built from a prefix and a value at run time: they are checked against the enums below.
    private static readonly string[] DynamicPrefixes = { "result.", "mode.", "format.", "class." };

    [Fact]
    public void English_IsTheDefault()
    {
        Assert.Equal("en", Localizer.DefaultLanguage);
        Assert.Contains("en", Localizer.Languages);
        Assert.Equal("en", Localizer.Normalize(null));
        Assert.Equal("en", Localizer.Normalize("xx"));
    }

    [Fact]
    public void EveryKeyUsedInTheCode_ExistsInEnglish()
    {
        var english = Localizer.TextsOf("en")!;
        var missing = UsedKeys().Where(k => !english.ContainsKey(k)).ToList();

        Assert.Empty(missing);
    }

    [Fact]
    public void EveryEnglishKey_IsUsed()
    {
        var used = UsedKeys().ToHashSet();
        var unused = Localizer.TextsOf("en")!.Keys
            .Where(k => !used.Contains(k) && !DynamicPrefixes.Any(p => k.StartsWith(p, StringComparison.Ordinal)))
            .ToList();

        Assert.Empty(unused);
    }

    [Fact]
    public void ResultsAndModes_HaveEnglishNames()
    {
        var english = Localizer.TextsOf("en")!;
        foreach (var result in Enum.GetNames<GameResult>())
        {
            Assert.True(english.ContainsKey("result." + result), result);
        }

        foreach (var mode in Enum.GetNames<GameMode>())
        {
            Assert.True(english.ContainsKey("mode." + mode), mode);
        }
    }

    [Theory]
    [InlineData("Deathknight", "Death Knight")]
    [InlineData("DemonHunter", "Demon Hunter")]
    [InlineData("WARRIOR", "Warrior")]
    [InlineData("Mage", "Mage")]
    [InlineData("Newclass", "Newclass")]
    [InlineData("", "")]
    public void ClassNames_UseTheTextOrTheStoredName(string stored, string expected)
    {
        Assert.Equal(expected, GameText.Class(stored));
    }

    [Fact]
    public void MissingKey_ShowsTheKey()
    {
        Assert.Equal("no.such.key", Localizer.Text("no.such.key"));
        Assert.Null(Localizer.TextOrNull("no.such.key"));
    }

    [Fact]
    public void Parse_SkipsCommentsAndKeepsTheValueAsWritten()
    {
        var texts = Localizer.Parse(new StringReader("# comment\n\n a.b = Text with = sign \nnot a line\nc.d=\n"));

        Assert.Equal(2, texts.Count);
        Assert.Equal(" Text with = sign", texts["a.b"]);
        Assert.Equal(string.Empty, texts["c.d"]);
    }

    [Fact]
    public void Format_FillsThePlaceholders()
    {
        Assert.Equal("Version 0.2.0 is available (installed: 0.1.0).", Localizer.Format("update.availableInstalled", "0.2.0", "0.1.0"));
        Assert.Equal("15% XP boost, 50 levels to 20%", TavernPassBonus.Describe(true, 20));
        Assert.Equal("10% XP boost, 1 level to 15%", TavernPassBonus.Describe(true, 19));
        Assert.Equal("20% XP boost (maximum)", TavernPassBonus.Describe(true, 70));
        Assert.Equal("No XP boost", TavernPassBonus.Describe(false, 70));
    }

    private static IEnumerable<string> UsedKeys()
    {
        var src = Path.Combine(RepositoryRoot(), "src");
        return Directory.EnumerateFiles(src, "*.cs", SearchOption.AllDirectories)
            .Where(f => !f.Contains(Path.DirectorySeparatorChar + "obj" + Path.DirectorySeparatorChar))
            .SelectMany(f => KeyUse.Matches(File.ReadAllText(f)).Select(m => m.Groups[1].Value))
            .Distinct();
    }

    private static string RepositoryRoot()
    {
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null && !File.Exists(Path.Combine(dir.FullName, "HearthstoneXpTracker.slnx")))
        {
            dir = dir.Parent;
        }

        return dir?.FullName ?? throw new InvalidOperationException("Repository root not found.");
    }
}
