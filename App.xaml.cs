using System.Windows;
using DekapuSkillLauncher.Models;
using DekapuSkillLauncher.Services;

namespace DekapuSkillLauncher;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);
        var s = AppSettings.Load();
        LocaleManager.Apply(s.Language);
        ThemeManager.Apply(s.Theme);
    }
}
