using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;

using DekapuSkillLauncher.Models;
using DekapuSkillLauncher.Services;

namespace DekapuSkillLauncher;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var s = AppSettings.Load();
        if (!s.HardwareAcceleration)
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        LocaleManager.Apply(s.Language);
        ThemeManager.Apply(s.Theme);
    }
}
