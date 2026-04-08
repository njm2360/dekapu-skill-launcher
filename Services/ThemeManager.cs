using Microsoft.Win32;
using System.Windows;

namespace DekapuSkillLauncher.Services;

public static class ThemeManager
{
    public static void Apply(string theme)
    {
        var effective = theme == "System" ? GetSystemTheme() : theme;
        var uri = new Uri(effective == "Dark" ? "Themes/Dark.xaml" : "Themes/Light.xaml", UriKind.Relative);
        var dict = new ResourceDictionary { Source = uri };

        var dicts = Application.Current.Resources.MergedDictionaries;
        var existing = dicts.FirstOrDefault(d => d.Source?.OriginalString.StartsWith("Themes/") == true);
        if (existing != null) dicts.Remove(existing);
        dicts.Add(dict);
    }

    public static string GetSystemTheme()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Themes\Personalize");
            if (key?.GetValue("AppsUseLightTheme") is int val && val == 0)
                return "Dark";
        }
        catch { }
        return "Light";
    }
}
