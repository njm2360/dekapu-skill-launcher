using System.IO;
using System.Reflection;
using System.Text.Json;

using DekapuSkillLauncher.Services;

namespace DekapuSkillLauncher.Models;

public class AppSettings
{
    public const int CurrentVersion = 2;
    public int Version { get; set; } = CurrentVersion;
    public int Profile { get; set; } = 0;
    public int OscPort { get; set; } = 9000;
    public string OscAddress { get; set; } = "/input/UseLeft";
    public string ApiBaseUrl { get; set; } = "https://dekapu.njm2360.com";
    public string LauncherPath { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\VRChat\launch.exe";
    public bool CheckVrcProcess { get; set; } = true;
    public string Theme { get; set; } = "System";
    public string SelectedGroupId { get; set; } = GroupDefinitions.Groups[0].Id;
    public string Language { get; set; } =
        LocaleManager.SupportedLanguages
            .FirstOrDefault(l => l.Code == System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName)
            ?.Code ?? "en";

    public LaunchOptions LaunchOptions { get; set; } = new();

    private static readonly JsonSerializerOptions SerializerOptions = new() { WriteIndented = true };

    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        Assembly.GetExecutingAssembly().GetName().Name!,
        "settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var json = File.ReadAllText(SettingsPath);
                AppSettings s;

                using var doc = JsonDocument.Parse(json);
                var root = doc.RootElement;
                s = JsonSerializer.Deserialize<AppSettings>(json) ?? new();
                if (!root.TryGetProperty(nameof(Version), out _))
                    MigrateFromV1(s, json);

                if (GroupDefinitions.Groups.All(g => g.Id != s.SelectedGroupId))
                    s.SelectedGroupId = GroupDefinitions.Groups[0].Id;
                return s;
            }
        }
        catch { }
        return new();
    }

    private static void MigrateFromV1(AppSettings s, string json)
    {
        s.LaunchOptions = JsonSerializer.Deserialize<LaunchOptions>(json) ?? new();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, SerializerOptions));
    }
}
