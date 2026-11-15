using System;
using System.IO;
using System.Linq;
using XpTracker.Core;
using Xunit;

namespace XpTracker.Core.Tests;

public class UpdateCheckTests
{
    private const string Page = "https://github.com/critterbyte/hearthstone_xp_tracker/releases/tag/";

    [Theory]
    [InlineData("v0.2.0", 0, 2, 0)]
    [InlineData("0.10.3", 0, 10, 3)]
    [InlineData(" V1.0.0 ", 1, 0, 0)]
    public void ParseTag_reads_versions(string tag, int major, int minor, int patch)
    {
        Assert.Equal(new Version(major, minor, patch), UpdateCheck.ParseTag(tag));
    }

    [Theory]
    [InlineData((string?)null)]
    [InlineData("")]
    [InlineData("v0.2")]
    [InlineData("v0.2.0-beta")]
    [InlineData("v0.2.0.1")]
    [InlineData("v-1.0.0")]
    [InlineData("latest")]
    public void ParseTag_ignores_other_tags(string? tag)
    {
        Assert.Null(UpdateCheck.ParseTag(tag));
    }

    [Fact]
    public void Same_version_is_not_an_update_although_the_assembly_version_has_four_parts()
    {
        var releases = new[] { Release("v0.1.0") };

        Assert.Null(UpdateCheck.Newer(releases, new Version(0, 1, 0, 0)));
    }

    [Fact]
    public void Newest_newer_release_is_chosen_in_any_order()
    {
        var releases = new[] { Release("v0.2.0"), Release("v0.3.0"), Release("v0.2.5") };

        Assert.Equal("v0.3.0", UpdateCheck.Newer(releases, new Version(0, 1, 0, 0))?.Tag);
    }

    [Fact]
    public void Drafts_and_invalid_tags_are_ignored()
    {
        var releases = new[] { Release("v0.4.0", draft: true), Release("nightly"), Release("v0.2.0") };

        Assert.Equal("v0.2.0", UpdateCheck.Newer(releases, new Version(0, 1, 0, 0))?.Tag);
    }

    [Fact]
    public void Older_releases_are_not_updates()
    {
        var releases = new[] { Release("v0.1.0"), Release("v0.0.9") };

        Assert.Null(UpdateCheck.Newer(releases, new Version(0, 2, 0, 0)));
    }

    [Fact]
    public void Version_text_has_three_parts()
    {
        Assert.Equal("0.2.0", Release("v0.2.0").VersionText);
        Assert.Equal("0.1.0", UpdateCheck.Display(new Version(0, 1, 0, 0)));
        Assert.Equal("0.1.0", UpdateCheck.Normalize(new Version(0, 1, 0, 0)).ToString());
    }

    [Theory]
    [InlineData(Page + "v0.2.0", true)]
    [InlineData("https://github.com/critterbyte/hearthstone_xp_tracker.evil/releases", false)]
    [InlineData("https://example.com/critterbyte/hearthstone_xp_tracker/releases", false)]
    [InlineData("https://github.com/critterbyte/hearthstone_xp_tracker/../../other/repo", false)]
    [InlineData("https://github.com/critterbyte/hearthstone_xp_tracker/releases x", false)]
    [InlineData("", false)]
    public void Links_outside_the_repository_fall_back_to_the_releases_page(string url, bool kept)
    {
        Assert.Equal(kept ? url : UpdateCheck.ReleasesPage, UpdateCheck.SafeLink(url));
    }

    [Fact]
    public void Update_row_is_shown_when_no_game_is_running()
    {
        var lines = XpDisplayText.WithUpdate(Array.Empty<XpLine>(), gameRunning: false, "0.2.0");

        var line = Assert.Single(lines);
        Assert.Equal("Update available:", line.Label);
        Assert.Equal("0.2.0", line.Value);
        Assert.True(line.IsUpdate);
        Assert.False(line.IsXp);
    }

    [Fact]
    public void Update_row_follows_the_last_game_summary()
    {
        var summary = new[] { new XpLine("Last game:", "Win"), new XpLine("Earned:", "+40", isXp: true) };

        var lines = XpDisplayText.WithUpdate(summary, gameRunning: false, "0.2.0");

        Assert.Equal(new[] { "Last game:", "Earned:", "Update available:" }, lines.Select(l => l.Label));
        Assert.Equal(new[] { false, false, true }, lines.Select(l => l.IsUpdate));
    }

    [Fact]
    public void Update_row_is_never_shown_during_a_game()
    {
        var live = new[] { new XpLine("XP time:", "3:12") };

        Assert.Same(live, XpDisplayText.WithUpdate(live, gameRunning: true, "0.2.0"));
    }

    [Fact]
    public void No_update_row_without_a_newer_version()
    {
        Assert.Empty(XpDisplayText.WithUpdate(Array.Empty<XpLine>(), gameRunning: false, null));
    }

    [Fact]
    public void Settings_check_for_updates_by_default()
    {
        Assert.True(XpTrackerSettings.Parse(Array.Empty<string>()).CheckForUpdates);
        Assert.True(XpTrackerSettings.Parse(new[] { "CheckForUpdates=maybe", "garbage" }).CheckForUpdates);
    }

    [Fact]
    public void Settings_read_the_switched_off_check()
    {
        Assert.False(XpTrackerSettings.Parse(new[] { " checkforupdates = False " }).CheckForUpdates);
    }

    [Fact]
    public void Settings_survive_saving_and_loading()
    {
        var folder = Path.Combine(Path.GetTempPath(), "XpTrackerTests", Guid.NewGuid().ToString("N"));
        var path = Path.Combine(folder, "settings.ini");
        try
        {
            Assert.True(XpTrackerSettings.Load(path).CheckForUpdates);

            new XpTrackerSettings { CheckForUpdates = false }.Save(path);

            Assert.False(XpTrackerSettings.Load(path).CheckForUpdates);
        }
        finally
        {
            if (Directory.Exists(folder))
            {
                Directory.Delete(folder, recursive: true);
            }
        }
    }

    private static ReleaseInfo Release(string tag, bool draft = false) => new(tag, Page + tag, draft);
}
