using System.Windows;
using Microsoft.Win32;

namespace DekapuSkillLauncher;

public partial class SettingsWindow : Window
{
    public int Profile { get; private set; }
    public string ExtraArgs { get; private set; } = "";
    public int OscPort { get; private set; }
    public string OscAddress { get; private set; } = "";
    public bool VrEnabled { get; private set; }
    public bool CheckVrcProcess { get; private set; }
    public string LauncherPath { get; private set; } = "";
    public string Theme { get; private set; } = "System";
    public string AppLanguage { get; private set; } = "ja";

    public SettingsWindow(int profile, string extraArgs, int oscPort, string oscAddress,
                          bool vrEnabled, bool checkVrcProcess, string launcherPath,
                          string theme, string language)
    {
        InitializeComponent();
        _profileBox.Text = profile.ToString();
        _extraArgsBox.Text = extraArgs;
        _oscPortBox.Text = oscPort.ToString();
        _oscAddressBox.Text = oscAddress;
        _vrCheck.IsChecked = vrEnabled;
        _vrcCheckBox.IsChecked = checkVrcProcess;
        _launcherPathBox.Text = launcherPath;

        switch (theme)
        {
            case "Light": _themeLightRadio.IsChecked  = true; break;
            case "Dark":  _themeDarkRadio.IsChecked   = true; break;
            default:      _themeSystemRadio.IsChecked = true; break;
        }

        if (language == "en")      _langEnRadio.IsChecked = true;
        else if (language == "zh") _langZhRadio.IsChecked = true;
        else if (language == "ko") _langKoRadio.IsChecked = true;
        else                       _langJaRadio.IsChecked = true;
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
        ExtraArgs = _extraArgsBox.Text.Trim();
        OscPort = oscPort;
        OscAddress = oscAddress;
        VrEnabled = _vrCheck.IsChecked == true;
        CheckVrcProcess = _vrcCheckBox.IsChecked == true;
        LauncherPath = _launcherPathBox.Text.Trim();
        Theme = _themeDarkRadio.IsChecked == true ? "Dark"
              : _themeLightRadio.IsChecked == true ? "Light"
              : "System";
        AppLanguage = _langEnRadio.IsChecked == true ? "en" : _langZhRadio.IsChecked == true ? "zh" : _langKoRadio.IsChecked == true ? "ko" : "ja";
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

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
        => DialogResult = false;
}
