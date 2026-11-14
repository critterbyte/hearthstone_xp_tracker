using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;

namespace XpTracker.Core;

/// <summary>
/// Texts of the overlay and the dashboard in the selected language.
/// </summary>
/// <remarks>
/// Each language is a text file <c>Localization/&lt;code&gt;.txt</c> embedded in this assembly, with one
/// <c>key=value</c> per line; lines starting with <c>#</c> are comments. Placeholders are <c>{0}</c>, <c>{1}</c>, …
/// A key missing in a language falls back to English, and a key missing in English shows the key itself.
/// Stored data (history, settings) never uses these texts, so files stay the same in every language.
/// </remarks>
public static class Localizer
{
    public const string DefaultLanguage = "en";

    private const string ResourcePrefix = "XpTracker.Core.Localization.";
    private static readonly Dictionary<string, string> English = Load(DefaultLanguage) ?? new Dictionary<string, string>();

    // Swapped as a whole, so readers on other threads always see one complete language.
    private static volatile Table _current = new(DefaultLanguage, English, CultureInfo.InvariantCulture);

    /// <summary>Code of the current language, for example "en" or "de".</summary>
    public static string Language => _current.Code;

    /// <summary>Culture for numbers, dates and day names in the current language; invariant for English.</summary>
    public static CultureInfo Culture => _current.Culture;

    /// <summary>Codes of all embedded languages, English first.</summary>
    public static IReadOnlyList<string> Languages { get; } = FindLanguages();

    /// <summary>
    /// The languages of the Hearthstone client in its own order, each with its name in that language.
    /// English and the two Spanish versions follow the client; Portuguese is Brazilian Portuguese as in the client.
    /// </summary>
    public static IReadOnlyList<KeyValuePair<string, string>> ClientLanguages { get; } = new[]
    {
        new KeyValuePair<string, string>("de", "Deutsch"),
        new KeyValuePair<string, string>("en", "English"),
        new KeyValuePair<string, string>("es-MX", "Español (AL)"),
        new KeyValuePair<string, string>("es-ES", "Español (EU)"),
        new KeyValuePair<string, string>("fr", "Français"),
        new KeyValuePair<string, string>("it", "Italiano"),
        new KeyValuePair<string, string>("ja", "日本語"),
        new KeyValuePair<string, string>("pl", "Polski"),
        new KeyValuePair<string, string>("pt-BR", "Português"),
        new KeyValuePair<string, string>("ru", "Русский"),
        new KeyValuePair<string, string>("th", "ภาษาไทย"),
        new KeyValuePair<string, string>("ko", "한국어"),
        new KeyValuePair<string, string>("zh-TW", "繁體中文"),
        new KeyValuePair<string, string>("zh-CN", "简体中文"),
    };

    /// <summary>The client languages that are embedded, for the language selection.</summary>
    public static IReadOnlyList<KeyValuePair<string, string>> SelectableLanguages()
    {
        var selectable = new List<KeyValuePair<string, string>>();
        foreach (var language in ClientLanguages)
        {
            if (Normalize(language.Key) == language.Key)
            {
                selectable.Add(language);
            }
        }

        return selectable;
    }

    /// <summary>Switches the language; an unknown code switches to English.</summary>
    public static void SetLanguage(string? code)
    {
        var normalized = Normalize(code);
        if (normalized == DefaultLanguage || Load(normalized) is not { } texts)
        {
            _current = new Table(DefaultLanguage, English, CultureInfo.InvariantCulture);
            return;
        }

        _current = new Table(normalized, texts, CultureFor(normalized));
    }

    /// <summary>Returns the language code if it is embedded, otherwise English.</summary>
    public static string Normalize(string? code)
    {
        foreach (var language in Languages)
        {
            if (string.Equals(language, code?.Trim(), StringComparison.OrdinalIgnoreCase))
            {
                return language;
            }
        }

        return DefaultLanguage;
    }

    /// <summary>The text for <paramref name="key"/> in the current language.</summary>
    public static string Text(string key)
    {
        if (_current.Texts.TryGetValue(key, out var text) || English.TryGetValue(key, out text))
        {
            return text;
        }

        return key;
    }

    /// <summary>The text for <paramref name="key"/> with its placeholders filled in, formatted for the current language.</summary>
    public static string Format(string key, params object[] args) => string.Format(Culture, Text(key), args);

    /// <summary>The text for <paramref name="key"/>, or null when no language has it; for optional keys such as class names.</summary>
    public static string? TextOrNull(string key) =>
        _current.Texts.TryGetValue(key, out var text) || English.TryGetValue(key, out text) ? text : null;

    /// <summary>Keys and texts of an embedded language, or null if it is not embedded; used by tests and the dashboard.</summary>
    public static IReadOnlyDictionary<string, string>? TextsOf(string code) => Load(code);

    /// <summary>Reads <c>key=value</c> lines; comments, empty lines and lines without "=" are skipped.</summary>
    public static Dictionary<string, string> Parse(TextReader reader)
    {
        var texts = new Dictionary<string, string>(StringComparer.Ordinal);
        string? line;
        while ((line = reader.ReadLine()) is not null)
        {
            var trimmed = line.TrimStart();
            var separator = trimmed.IndexOf('=');
            if (trimmed.Length == 0 || trimmed[0] == '#' || separator <= 0)
            {
                continue;
            }

            texts[trimmed.Substring(0, separator).Trim()] = trimmed.Substring(separator + 1).TrimEnd();
        }

        return texts;
    }

    private static Dictionary<string, string>? Load(string code)
    {
        if (code.Length == 0)
        {
            return null;
        }

        using var stream = typeof(Localizer).GetTypeInfo().Assembly.GetManifestResourceStream(ResourcePrefix + code + ".txt");
        if (stream is null)
        {
            return null;
        }

        using var reader = new StreamReader(stream, Encoding.UTF8);
        return Parse(reader);
    }

    private static IReadOnlyList<string> FindLanguages()
    {
        var languages = new List<string> { DefaultLanguage };
        foreach (var name in typeof(Localizer).GetTypeInfo().Assembly.GetManifestResourceNames())
        {
            if (name.StartsWith(ResourcePrefix, StringComparison.Ordinal) && name.EndsWith(".txt", StringComparison.Ordinal))
            {
                var code = name.Substring(ResourcePrefix.Length, name.Length - ResourcePrefix.Length - 4);
                if (code != DefaultLanguage)
                {
                    languages.Add(code);
                }
            }
        }

        return languages;
    }

    private static CultureInfo CultureFor(string code)
    {
        try
        {
            // Dates keep the Gregorian calendar, so the year matches the history (Thai would use the Buddhist year).
            var culture = (CultureInfo)CultureInfo.GetCultureInfo(code).Clone();
            if (culture.DateTimeFormat.Calendar is not GregorianCalendar)
            {
                foreach (var calendar in culture.OptionalCalendars)
                {
                    if (calendar is GregorianCalendar gregorian)
                    {
                        culture.DateTimeFormat.Calendar = gregorian;
                        break;
                    }
                }
            }

            return CultureInfo.ReadOnly(culture);
        }
        catch (CultureNotFoundException)
        {
            return CultureInfo.InvariantCulture;
        }
    }

    private sealed class Table
    {
        public Table(string code, Dictionary<string, string> texts, CultureInfo culture)
        {
            Code = code;
            Texts = texts;
            Culture = culture;
        }

        public string Code { get; }

        public Dictionary<string, string> Texts { get; }

        public CultureInfo Culture { get; }
    }
}
