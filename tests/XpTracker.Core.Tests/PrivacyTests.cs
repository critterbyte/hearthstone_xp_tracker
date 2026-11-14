using System.Text.RegularExpressions;

namespace XpTracker.Core.Tests;

// Guards against personal data copied from real logs into the repository, for example a Power.log line in a test.
// Test data uses placeholders only: players "Alice" and "Bob", account ids such as [hi=1 lo=1], [hi=0 lo=0] for the computer.
public class PrivacyTests
{
    private static readonly string[] TextExtensions = { ".cs", ".md", ".txt", ".ps1", ".yml", ".yaml", ".csproj", ".props", ".json", ".slnx" };
    private static readonly string[] SkippedFolders = { "bin", "obj", ".git", "artifacts", ".vs" };

    // Real Battle.net account ids are large numbers; placeholders stay below 100.
    private static readonly Regex AccountId = new(@"GameAccountId=\[hi=(\d+) lo=(\d+)\]");

    // A BattleTag is a name followed by "#" and 4 or 5 digits, for example "Name#1234".
    private static readonly Regex BattleTag = new(@"\b\p{L}[\p{L}\p{N}]{2,11}#\d{4,5}\b");

    [Fact]
    public void Repository_ContainsNoRealAccountIds()
    {
        var found = Scan(line => AccountId.Matches(line).Any(m => long.Parse(m.Groups[1].Value) >= 100 || long.Parse(m.Groups[2].Value) >= 100));

        Assert.Empty(found);
    }

    [Fact]
    public void Repository_ContainsNoBattleTags()
    {
        var found = Scan(BattleTag.IsMatch);

        Assert.Empty(found);
    }

    [Theory]
    [InlineData("Player EntityID=2 PlayerID=1 GameAccountId=[hi=1 lo=1]", false)]
    [InlineData("Player EntityID=3 PlayerID=2 GameAccountId=[hi=0 lo=0]", false)]
    [InlineData("Player EntityID=2 PlayerID=1 GameAccountId=[hi=144115188075855872 lo=12345678]", true)]
    public void AccountIdCheck_FlagsOnlyLargeNumbers(string line, bool flagged)
    {
        Assert.Equal(flagged, AccountId.Matches(line).Any(m => long.Parse(m.Groups[1].Value) >= 100 || long.Parse(m.Groups[2].Value) >= 100));
    }

    [Theory]
    [InlineData("TAG_CHANGE Entity=Someone#12345 tag=TIMEOUT value=10", true)]
    [InlineData("TAG_CHANGE Entity=Alice tag=TIMEOUT value=10", false)]
    [InlineData("See [USAGE.md](USAGE.md#update-hint) and PR #1234.", false)]
    public void BattleTagCheck_FlagsNamesWithDigits(string line, bool flagged)
    {
        Assert.Equal(flagged, BattleTag.IsMatch(line));
    }

    private static List<string> Scan(Func<string, bool> isPersonal)
    {
        var root = RepositoryRoot();
        var self = Path.Combine(root, "tests", "XpTracker.Core.Tests", nameof(PrivacyTests) + ".cs");
        var found = new List<string>();
        foreach (var file in Directory.EnumerateFiles(root, "*", SearchOption.AllDirectories))
        {
            var relative = Path.GetRelativePath(root, file);
            if (file == self
                || !TextExtensions.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase)
                || relative.Split(Path.DirectorySeparatorChar).Any(part => SkippedFolders.Contains(part, StringComparer.OrdinalIgnoreCase)))
            {
                continue;
            }

            var number = 0;
            foreach (var line in File.ReadLines(file))
            {
                number++;
                if (isPersonal(line))
                {
                    found.Add(relative + ":" + number);
                }
            }
        }

        return found;
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
