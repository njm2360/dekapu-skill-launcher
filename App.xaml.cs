using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using DekapuSkillLauncher.Models;
using DekapuSkillLauncher.Services;

namespace DekapuSkillLauncher;

public partial class App : Application
{
    public static AppSettings Settings { get; private set; } = null!;

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        Settings = AppSettings.Load();
        if (!Settings.HardwareAcceleration)
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        LocaleManager.Apply(Settings.Language);
        ThemeManager.Apply(Settings.Theme);
    }
}
