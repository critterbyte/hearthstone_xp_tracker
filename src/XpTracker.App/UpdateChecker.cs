using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using XpTracker.Core;

namespace XpTracker.App;

/// <summary>
/// Asks GitHub for the plugin's releases. The request carries only a User-Agent with the plugin name and version.
/// </summary>
internal static class UpdateChecker
{
    private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(10);

    /// <summary>The newest newer release, or null if there is none or the check failed.</summary>
    public static async Task<ReleaseInfo?> FindNewerAsync(Version installed, CancellationToken token)
    {
        try
        {
            using var client = new HttpClient { Timeout = Timeout };
            using var request = new HttpRequestMessage(HttpMethod.Get, UpdateCheck.ReleasesApi);
            request.Headers.UserAgent.ParseAdd("HearthstoneXpTracker/" + UpdateCheck.Display(installed));
            request.Headers.Accept.ParseAdd("application/vnd.github+json");
            using var response = await client.SendAsync(request, token).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
            {
                // For example 404 while the repository is private, or 403 when GitHub's rate limit is reached.
                return null;
            }

            var json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return UpdateCheck.Newer(Parse(json), installed);
        }
        catch (Exception)
        {
            // Offline, timeout, unload or an unexpected answer: the update hint is optional, so nothing is shown.
            return null;
        }
    }

    private static IReadOnlyList<ReleaseInfo> Parse(string json)
    {
        if (JToken.Parse(json) is not JArray releases)
        {
            return Array.Empty<ReleaseInfo>();
        }

        return releases
            .OfType<JObject>()
            .Select(r => new ReleaseInfo((string?)r["tag_name"] ?? string.Empty, (string?)r["html_url"] ?? string.Empty, (bool?)r["draft"] ?? false))
            .ToList();
    }
}
