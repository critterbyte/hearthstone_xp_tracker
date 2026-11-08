using System;
using System.Globalization;

namespace XpTracker.Core;

public static class TimeFormat
{
    /// <summary>Formats seconds as m:ss, for example 372.4 as "6:12".</summary>
    public static string MinutesSeconds(double seconds)
    {
        var total = (int)Math.Floor(Math.Max(seconds, 0));
        return string.Format(CultureInfo.InvariantCulture, "{0}:{1:00}", total / 60, total % 60);
    }

    /// <summary>Formats seconds as h:mm:ss, for example 5077 as "1:24:37".</summary>
    public static string HoursMinutesSeconds(double seconds)
    {
        var total = (int)Math.Floor(Math.Max(seconds, 0));
        return string.Format(CultureInfo.InvariantCulture, "{0}:{1:00}:{2:00}", total / 3600, total / 60 % 60, total % 60);
    }
}
