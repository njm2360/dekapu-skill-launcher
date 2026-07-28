using System.Windows;
using Microsoft.Win32;
using DekapuSkillLauncher.Models;
using DekapuSkillLauncher.Services;

namespace DekapuSkillLauncher.Views;

public partial class SettingsWindow : Window
{
    private readonly AppSettings _settings;
    private LaunchOptions _launchDraft;
    private readonly bool _initialHardwareAcceleration;

    public SettingsWindow(AppSettings settings)
    {
        InitializeComponent();

        _settings = settings;
        _launchDraft = settings.LaunchOptions.Clone();

        _profileBox.Text = settings.Profile.ToString();
        _oscPortBox.Text = settings.OscPort.ToString();
        _oscAddressBox.Text = settings.OscAddress;
        _vrcCheckBox.IsChecked = settings.CheckVrcProcess;
        _launcherPathBox.Text = settings.LauncherPath;
        _hwAccelBox.IsChecked = settings.HardwareAcceleration;
        _initialHardwareAcceleration = settings.HardwareAcceleration;

        switch (settings.Theme)
        {
            case "Light": _themeLightRadio.IsChecked = true; break;
            case "Dark": _themeDarkRadio.IsChecked = true; break;
            default: _themeSystemRadio.IsChecked = true; break;
        }

        _langCombo.ItemsSource = LocaleManager.SupportedLanguages;
        _langCombo.SelectedItem = LocaleManager.SupportedLanguages.FirstOrDefault(l => l.Code == settings.Language)
                                  ?? LocaleManager.SupportedLanguages[0];
    }

    private void OkBtn_Click(object sender, RoutedEventArgs e)
    {
        if (!int.TryParse(_profileBox.Text, out var profile) || profile < 0 || profile > 99)
        {
            MessageBox.Show(LocaleManager.Get("S.ErrProfileMsg"), LocaleManager.Get("S.ErrInput"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!int.TryParse(_oscPortBox.Text, out var oscPort) || oscPort < 1 || oscPort > 65535)
        {
            MessageBox.Show(LocaleManager.Get("S.ErrPortMsg"), LocaleManager.Get("S.ErrInput"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var oscAddress = _oscAddressBox.Text.Trim();
        if (!oscAddress.StartsWith('/'))
        {
            MessageBox.Show(LocaleManager.Get("S.ErrOscAddressMsg"), LocaleManager.Get("S.ErrInput"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        _settings.Profile = profile;
        _settings.OscPort = oscPort;
        _settings.OscAddress = oscAddress;
        _settings.CheckVrcProcess = _vrcCheckBox.IsChecked == true;
        _settings.LauncherPath = _launcherPathBox.Text.Trim();
        _settings.Theme = _themeDarkRadio.IsChecked == true ? "Dark"
                        : _themeLightRadio.IsChecked == true ? "Light"
                        : "System";
        _settings.Language = (_langCombo.SelectedItem as LanguageItem)?.Code
                             ?? LocaleManager.DefaultLanguage;
        _settings.HardwareAcceleration = _hwAccelBox.IsChecked == true;
        _settings.LaunchOptions = _launchDraft;

        if (_settings.HardwareAcceleration != _initialHardwareAcceleration)
        {
            MessageBox.Show(this,
                LocaleManager.Get("S.MsgRestartRequired"),
                LocaleManager.Get("S.DlgInfoTitle"),
                MessageBoxButton.OK, MessageBoxImage.Information);
        }

        DialogResult = true;
    }

    private void BrowseBtn_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new OpenFileDialog
        {
            Title = LocaleManager.Get("S.DlgBrowseTitle"),
            Filter = LocaleManager.Get("S.DlgBrowseFilter"),
            FileName = _launcherPathBox.Text,
        };
        if (dlg.ShowDialog(this) == true)
            _launcherPathBox.Text = dlg.FileName;
    }

    private void LaunchOptionsBtn_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new LaunchOptionsWindow(_launchDraft) { Owner = this };
        if (dlg.ShowDialog() == true)
            _launchDraft = dlg.LaunchOptions;
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
        => DialogResult = false;
}
