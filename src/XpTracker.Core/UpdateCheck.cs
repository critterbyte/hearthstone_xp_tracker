using System;
using System.Collections.Generic;
using System.Globalization;

namespace XpTracker.Core;

/// <summary>
/// A release as listed by GitHub.
/// </summary>
public sealed class ReleaseInfo
{
    public ReleaseInfo(string tag, string url, bool isDraft = false)
    {
        Tag = tag;
        Url = url;
        IsDraft = isDraft;
    }

    public string Tag { get; }

    public string Url { get; }

    public bool IsDraft { get; }

    /// <summary>Version from the tag, or null if the tag is not of the form vX.Y.Z.</summary>
    public Version? Version => UpdateCheck.ParseTag(Tag);

    /// <summary>Version as shown to the user, for example "0.2.0".</summary>
    public string VersionText => Version is { } version ? UpdateCheck.Display(version) : Tag;
}

/// <summary>
/// Finds out whether a newer release exists. Release tags must have the form vX.Y.Z.
/// </summary>
public static class UpdateCheck
{
    public const string Repository = "critterbyte/hearthstone_xp_tracker";

    // The list includes pre-releases; "releases/latest" would skip them.
    public const string ReleasesApi = "https://api.github.com/repos/" + Repository + "/releases?per_page=20";

    public const string ReleasesPage = "https://github.com/" + Repository + "/releases";

    private const string LinkPrefix = "https://github.com/" + Repository + "/";

    /// <summary>Reads "v1.2.3" or "1.2.3"; anything else (for example "v1.2.3-beta") gives null.</summary>
    public static Version? ParseTag(string? tag)
    {
        if (tag is null)
        {
            return null;
        }

        var text = tag.Trim();
        if (text.StartsWith("v", StringComparison.OrdinalIgnoreCase))
        {
            text = text.Substring(1);
        }

        var parts = text.Split('.');
        if (parts.Length != 3)
        {
            return null;
        }

        var numbers = new int[3];
        for (var i = 0; i < 3; i++)
        {
            if (!int.TryParse(parts[i], NumberStyles.None, CultureInfo.InvariantCulture, out numbers[i]))
            {
                return null;
            }
        }

        return new Version(numbers[0], numbers[1], numbers[2]);
    }

    /// <summary>
    /// Reduces a version to major.minor.patch. The assembly version has four parts (0.1.0.0) and
    /// <see cref="Version"/> treats 0.1.0 as older than 0.1.0.0, so both sides are compared with three parts.
    /// </summary>
    public static Version Normalize(Version version) => new(version.Major, version.Minor, Math.Max(version.Build, 0));

    public static string Display(Version version) => Normalize(version).ToString(3);

    /// <summary>The newest release that is newer than the installed version, or null. Drafts and other tags are ignored.</summary>
    public static ReleaseInfo? Newer(IEnumerable<ReleaseInfo> releases, Version installed)
    {
        var current = Normalize(installed);
        ReleaseInfo? best = null;
        Version? bestVersion = null;
        foreach (var release in releases)
        {
            var version = release.Version;
            if (release.IsDraft || version is null || version <= current)
            {
                continue;
            }

            if (bestVersion is null || version > bestVersion)
            {
                best = release;
                bestVersion = version;
            }
        }

        return best;
    }

    /// <summary>The link if it points into this repository on GitHub, otherwise the releases page.</summary>
    public static string SafeLink(string? url) =>
        url is not null && url.StartsWith(LinkPrefix, StringComparison.Ordinal) && url.IndexOf("..", StringComparison.Ordinal) < 0 && !ContainsWhiteSpace(url)
            ? url
            : ReleasesPage;

    private static bool ContainsWhiteSpace(string text)
    {
        foreach (var c in text)
        {
            if (char.IsWhiteSpace(c))
            {
                return true;
            }
        }

        return false;
    }
}
