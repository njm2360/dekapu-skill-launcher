using System.Windows;

namespace DekapuSkillLauncher;

public static class LocaleManager
{
    public static void Apply(string lang)
    {
        var uri = new Uri(lang switch
        {
            "en" => "Locales/en.xaml",
            "zh" => "Locales/zh.xaml",
            "ko" => "Locales/ko.xaml",
            _ => "Locales/ja.xaml"
        }, UriKind.Relative);
        var dict = new ResourceDictionary { Source = uri };

        var dicts = Application.Current.Resources.MergedDictionaries;
        var existing = dicts.FirstOrDefault(d => d.Source?.OriginalString.StartsWith("Locales/") == true);
        if (existing != null) dicts.Remove(existing);
        dicts.Add(dict);
    }

    public static string Get(string key) =>
        Application.Current.Resources[key] as string ?? key;
}
