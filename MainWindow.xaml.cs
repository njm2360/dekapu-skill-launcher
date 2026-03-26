using System.Collections.ObjectModel;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Controls;

namespace DekapuSkillLauncher;

public record InstanceRow(string Id, string DisplayNameOrName, int UserCount, bool IsClosed);

public partial class MainWindow : Window
{
    private static readonly TimeZoneInfo TZ =
        TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

    private readonly InstanceService _ctrl;
    private readonly AppSettings _settings = AppSettings.Load();
    private readonly ObservableCollection<InstanceRow> _rows = new();
    private DateTimeOffset? _lastUpdatedAt;

    public MainWindow()
    {
        _ctrl = new InstanceService(_settings);
        InitializeComponent();
        _table.ItemsSource = _rows;
        ApplyLocale();
    }

    private async void Window_Loaded(object sender, RoutedEventArgs e)
        => await UpdateInstancesAsync(refresh: true);

    private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
    {
        try { _settings.Save(); }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, LocaleManager.Get("S.ErrTitle"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void RefreshBtn_Click(object sender, RoutedEventArgs e)
        => await UpdateInstancesAsync(refresh: true);

    private void LaunchBtn_Click(object sender, RoutedEventArgs e)
        => LaunchSelected();

    private void SettingsBtn_Click(object sender, RoutedEventArgs e)
        => OpenSettings();

    private void Table_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        var row = _table.SelectedItem as InstanceRow;
        _launchBtn.IsEnabled = row is not null && !row.IsClosed;
    }

    private async void FixedBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await OscSender.SendAsync("127.0.0.1", _settings.OscPort, _settings.OscAddress, 0);
            await Task.Delay(100);
            await OscSender.SendAsync("127.0.0.1", _settings.OscPort, _settings.OscAddress, 1);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, LocaleManager.Get("S.ErrOscTitle"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private async void ReleaseBtn_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            await OscSender.SendAsync("127.0.0.1", _settings.OscPort, _settings.OscAddress, 0);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, LocaleManager.Get("S.ErrOscTitle"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void OpenSettings()
    {
        var dlg = new SettingsWindow(
            _settings.Profile, _settings.ExtraArgs,
            _settings.OscPort, _settings.OscAddress,
            _settings.VrEnabled, _settings.CheckVrcProcess,
            _settings.LauncherPath, _settings.Theme, _settings.Language)
        {
            Owner = this
        };
        if (dlg.ShowDialog() != true)
            return;

        _settings.Profile = dlg.Profile;
        _settings.ExtraArgs = dlg.ExtraArgs;
        _settings.OscPort = dlg.OscPort;
        _settings.OscAddress = dlg.OscAddress;
        _settings.VrEnabled = dlg.VrEnabled;
        _settings.CheckVrcProcess = dlg.CheckVrcProcess;
        _settings.LauncherPath = dlg.LauncherPath;
        _settings.Theme = dlg.Theme;
        _settings.Language = dlg.AppLanguage;
        try { _settings.Save(); }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, LocaleManager.Get("S.ErrTitle"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
        }
        ThemeManager.Apply(_settings.Theme);
        LocaleManager.Apply(_settings.Language);
        ApplyLocale();
    }

    private void ApplyLocale()
    {
        var ver = Assembly.GetExecutingAssembly().GetName().Version;
        Title = $"{LocaleManager.Get("S.AppTitle")} v{ver?.Major}.{ver?.Minor}.{ver?.Build}";
        _table.Columns[0].Header = LocaleManager.Get("S.ColName");
        _table.Columns[1].Header = LocaleManager.Get("S.ColUsers");
        UpdateUpdatedLabel();
    }

    private void UpdateUpdatedLabel()
    {
        if (_lastUpdatedAt is null)
            return;
        var jst = TimeZoneInfo.ConvertTime(_lastUpdatedAt.Value, TZ);
        _updatedLabel.Text = string.Format(LocaleManager.Get("S.UpdatedAt"), jst.ToString("yyyy-MM-dd HH:mm:ss"));
    }

    private async Task UpdateInstancesAsync(bool refresh = false)
    {
        SetLoading(true);

        InstanceCache cache;
        try
        {
            cache = await _ctrl.GetInstancesAsync(refresh);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, LocaleManager.Get("S.ErrTitle"),
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }
        finally
        {
            SetLoading(false);
        }

        _rows.Clear();
        var ordered = cache.Instances
            .OrderBy(i => i.ClosedAt.HasValue ? 1 : 0)
            .ThenByDescending(i => i.UserCount)
            .ToList();

        if (ordered.Count == 0)
        {
            _rows.Add(new InstanceRow("", LocaleManager.Get("S.None"), 0, false));
        }
        else
        {
            foreach (var inst in ordered)
                _rows.Add(new InstanceRow(inst.Id, inst.DisplayName ?? inst.Name, inst.UserCount, inst.ClosedAt.HasValue));
        }

        _lastUpdatedAt = cache.UpdatedAt;
        UpdateUpdatedLabel();
    }

    private void SetLoading(bool loading)
    {
        _loadingOverlay.Visibility = loading ? Visibility.Visible : Visibility.Collapsed;
    }

    private void LaunchSelected()
    {
        if (_table.SelectedItem is not InstanceRow row) return;

        InstanceInfo inst;
        try
        {
            inst = _ctrl.GetInstanceById(row.Id);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, LocaleManager.Get("S.ErrTitle"),
                MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var name = inst.DisplayName ?? inst.Name;
        var confirm = MessageBox.Show(
            string.Format(LocaleManager.Get("S.DlgLaunchMsg"), name),
            LocaleManager.Get("S.DlgConfirmTitle"),
            MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        if (_settings.CheckVrcProcess && Process.GetProcessesByName("VRChat").Length > 0)
        {
            MessageBox.Show(LocaleManager.Get("S.VrcRunningMsg"),
                LocaleManager.Get("S.ErrTitle"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!File.Exists(_settings.LauncherPath))
        {
            MessageBox.Show(string.Format(LocaleManager.Get("S.ErrNoLauncher"), _settings.LauncherPath),
                LocaleManager.Get("S.ErrTitle"), MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var url = $"vrchat://launch?ref={Assembly.GetExecutingAssembly().GetName().Name}&id={inst.Id}&shortName={inst.ShortName}";
        var psi = new ProcessStartInfo
        {
            FileName = _settings.LauncherPath,
            UseShellExecute = false,
        };
        psi.ArgumentList.Add(url);
        if (_settings.Profile != 0)
            psi.ArgumentList.Add($"--profile={_settings.Profile}");
        if (!_settings.VrEnabled)
            psi.ArgumentList.Add("--no-vr");
        foreach (var arg in _settings.ExtraArgs.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            psi.ArgumentList.Add(arg);
        Process.Start(psi);
    }
}
