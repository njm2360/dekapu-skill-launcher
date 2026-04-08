using System.Windows;

namespace DekapuSkillLauncher.Services;

public class LanguageItem
{
    public string Code { get; set; } = "";
    public string Name { get; set; } = "";

    public override string ToString() => Name;
}

public static class LocaleManager
{
    public static string DefaultLanguage => SupportedLanguages[0].Code;

    public static readonly LanguageItem[] SupportedLanguages =
    [
        new() { Code = "ja", Name = "日本語" },
        new() { Code = "en", Name = "English" },
        new() { Code = "zh", Name = "中文" },
        new() { Code = "ko", Name = "한국어" },
    ];

    public static void Apply(string lang)
    {
        var resolved = SupportedLanguages.Any(l => l.Code == lang) ? lang : DefaultLanguage;
        var uri = new Uri($"Locales/{resolved}.xaml", UriKind.Relative);
        var dict = new ResourceDictionary { Source = uri };

        var dicts = Application.Current.Resources.MergedDictionaries;
        var existing = dicts.FirstOrDefault(d => d.Source?.OriginalString.StartsWith("Locales/") == true);
        if (existing != null) dicts.Remove(existing);
        dicts.Add(dict);
    }

    public static string Get(string key) =>
        Application.Current.Resources[key] as string ?? key;
}
