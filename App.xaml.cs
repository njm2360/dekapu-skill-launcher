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
        DispatcherUnhandledException += (_, args) =>
        {
            ShowUnexpectedError(args.Exception);
            args.Handled = true;
        };
        AppDomain.CurrentDomain.UnhandledException += (_, args) =>
        {
            if (args.ExceptionObject is Exception ex)
                ShowUnexpectedError(ex);
        };
        TaskScheduler.UnobservedTaskException += (_, args) => args.SetObserved();

        base.OnStartup(e);
        Settings = AppSettings.Load();
        if (!Settings.HardwareAcceleration)
            RenderOptions.ProcessRenderMode = RenderMode.SoftwareOnly;
        LocaleManager.Apply(Settings.Language);
        ThemeManager.Apply(Settings.Theme);
    }

    private static void ShowUnexpectedError(Exception ex)
    {
        string message, title;
        try
        {
            message = string.Format(LocaleManager.Get("S.ErrUnexpectedMsg"), ex.Message);
            title = LocaleManager.Get("S.ErrTitle");
        }
        catch
        {
            message = ex.Message;
            title = "Error";
        }
        MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
    }
}
