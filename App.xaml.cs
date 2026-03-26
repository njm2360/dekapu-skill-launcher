using System.Windows;

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
