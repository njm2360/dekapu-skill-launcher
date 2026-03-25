using System.Reflection;
using System.Text.Json;

namespace SimpleLauncherWinForms;

public class AppSettings
{
    public int Profile { get; set; } = 0;
    public string ExtraArgs { get; set; } = "--process-priority=2 --main-thread-priority=2";
    public int OscPort { get; set; } = 9000;
    public string OscAddress { get; set; } = "/input/UseLeft";

    private static readonly string SettingsPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        Assembly.GetExecutingAssembly().GetName().Name!,
        "settings.json");

    public static AppSettings Load()
    {
        try
        {
            if (File.Exists(SettingsPath))
                return JsonSerializer.Deserialize<AppSettings>(File.ReadAllText(SettingsPath)) ?? new();
        }
        catch { }
        return new();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(SettingsPath)!);
            File.WriteAllText(SettingsPath,
                JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch { }
    }
}
