using System.Windows;
using Microsoft.Win32;
using DekapuSkillLauncher.Models;
using DekapuSkillLauncher.Services;

namespace DekapuSkillLauncher.Views;

public partial class SettingsWindow : Window
{
    public int Profile { get; private set; }
    public int OscPort { get; private set; }
    public string OscAddress { get; private set; } = "";
    public bool CheckVrcProcess { get; private set; }
    public string LauncherPath { get; private set; } = "";
    public string Theme { get; private set; } = "System";
    public string AppLanguage { get; private set; } = LocaleManager.DefaultLanguage;

    public LaunchOptions LaunchOptions { get; private set; } = new();

    private LaunchOptions _launchDraft;

    public SettingsWindow(AppSettings settings)
    {
        InitializeComponent();

        _launchDraft = settings.LaunchOptions.Clone();

        _profileBox.Text = settings.Profile.ToString();
        _oscPortBox.Text = settings.OscPort.ToString();
        _oscAddressBox.Text = settings.OscAddress;
        _vrcCheckBox.IsChecked = settings.CheckVrcProcess;
        _launcherPathBox.Text = settings.LauncherPath;

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

        Profile = profile;
        OscPort = oscPort;
        OscAddress = oscAddress;
        CheckVrcProcess = _vrcCheckBox.IsChecked == true;
        LauncherPath = _launcherPathBox.Text.Trim();
        Theme = _themeDarkRadio.IsChecked == true ? "Dark"
              : _themeLightRadio.IsChecked == true ? "Light"
              : "System";
        AppLanguage = (_langCombo.SelectedItem as LanguageItem)?.Code
                      ?? LocaleManager.DefaultLanguage;

        LaunchOptions = _launchDraft;

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
