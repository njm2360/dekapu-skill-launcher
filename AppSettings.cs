using System.IO;
using System.Reflection;
using System.Text.Json;

namespace DekapuSkillLauncher;

public class AppSettings
{
    public int Profile { get; set; } = 0;
    public string ExtraArgs { get; set; } = "";
    public int OscPort { get; set; } = 9000;
    public string OscAddress { get; set; } = "/input/UseLeft";
    public string ApiBaseUrl { get; set; } = "https://dekapu.njm2360.com";
    public bool VrEnabled { get; set; } = false;
    public string LauncherPath { get; set; } = @"C:\Program Files (x86)\Steam\steamapps\common\VRChat\launch.exe";
    public bool CheckVrcProcess { get; set; } = true;
    public string Theme { get; set; } = "System";
    public string SelectedGroupId { get; set; } = GroupDefinitions.Groups[0].Id;
    public string Language { get; set; } =
        System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName switch
        {
            "ja" => "ja",
            "zh" => "zh",
            "ko" => "ko",
            _ => "en"
        };

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
                var s = JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath)) ?? new();
                if (GroupDefinitions.Groups.All(g => g.Id != s.SelectedGroupId))
                    s.SelectedGroupId = GroupDefinitions.Groups[0].Id;
                return s;
            }
        }
        catch { }
        return new();
    }

    public void Save()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
        File.WriteAllText(SettingsPath, JsonSerializer.Serialize(this, SerializerOptions));
    }
}
