using System.Windows;
using System.Windows.Controls;
using DekapuSkillLauncher.Models;
using DekapuSkillLauncher.Services;

namespace DekapuSkillLauncher.Views;

public partial class LaunchOptionsWindow : Window
{
    public LaunchOptions LaunchOptions { get; private set; } = new();

    public LaunchOptionsWindow(LaunchOptions options)
    {
        InitializeComponent();

        PopulatePriorityCombo(_processPriorityCombo, isMainThread: false);
        PopulatePriorityCombo(_mainThreadPriorityCombo, isMainThread: true);

        _vrCheck.IsChecked = options.VrEnabled;
        _fpsBox.Text = options.Fps > 0 ? options.Fps.ToString() : "";

        _oscCheck.IsChecked = options.OscEnabled;
        _oscInPortBox.Text = options.OscInPort.ToString();
        _oscOutIpBox.Text = options.OscOutIp;
        _oscOutPortBox.Text = options.OscOutPort.ToString();

        _affinityBox.Text = options.Affinity;
        SelectComboByTag(_processPriorityCombo, options.ProcessPriority);
        SelectComboByTag(_mainThreadPriorityCombo, options.MainThreadPriority);

        _screenWidthBox.Text = options.ScreenWidth > 0 ? options.ScreenWidth.ToString() : "";
        _screenHeightBox.Text = options.ScreenHeight > 0 ? options.ScreenHeight.ToString() : "";
        SelectComboByTag(_screenFullscreenCombo, options.ScreenFullscreen.ToString());
        _monitorBox.Text = options.Monitor > 0 ? options.Monitor.ToString() : "";
        SelectComboByTag(_hwVideoDecodingCombo, options.HwVideoDecoding);
        _disableAmdStutterCheck.IsChecked = options.DisableAmdStutterWorkaround;

        _watchWorldsCheck.IsChecked = options.WatchWorlds;
        _watchAvatarsCheck.IsChecked = options.WatchAvatars;
        _debugGuiCheck.IsChecked = options.DebugGui;
        _sdkLogLevelsCheck.IsChecked = options.SdkLogLevels;
        _udonDebugLoggingCheck.IsChecked = options.UdonDebugLogging;
        _skipRegistryInstallCheck.IsChecked = options.SkipRegistryInstall;
        _enforceWorldServerChecksCheck.IsChecked = options.EnforceWorldServerChecks;
        _ignoreTrackersBox.Text = options.IgnoreTrackers;
        _midiBox.Text = options.Midi;

        _extraArgsBox.Text = options.ExtraArgs;

        UpdateOscFields();
    }

    private static void PopulatePriorityCombo(ComboBox combo, bool isMainThread)
    {
        combo.Items.Add(new ComboBoxItem { Tag = "", Content = "--" });
        combo.Items.Add(new ComboBoxItem { Tag = "-2", Content = $"-2  {LocaleManager.Get(isMainThread ? "S.PrioLowest" : "S.PrioIdle")}" });
        combo.Items.Add(new ComboBoxItem { Tag = "-1", Content = $"-1  {LocaleManager.Get("S.PrioBelowNormal")}" });
        combo.Items.Add(new ComboBoxItem { Tag = "0", Content = $" 0  {LocaleManager.Get("S.PrioNormal")}" });
        combo.Items.Add(new ComboBoxItem { Tag = "1", Content = $" 1  {LocaleManager.Get("S.PrioAboveNormal")}" });
        combo.Items.Add(new ComboBoxItem { Tag = "2", Content = $" 2  {LocaleManager.Get(isMainThread ? "S.PrioHighest" : "S.PrioHigh")}" });
    }

    private static void SelectComboByTag(ComboBox combo, string tag)
    {
        foreach (ComboBoxItem item in combo.Items)
        {
            if ((string?)item.Tag == tag)
            {
                combo.SelectedItem = item;
                return;
            }
        }
        combo.SelectedIndex = 0;
    }

    private void AffinityBtn_Click(object sender, RoutedEventArgs e)
    {
        var dlg = new CpuAffinityWindow(_affinityBox.Text) { Owner = this };
        if (dlg.ShowDialog() == true)
            _affinityBox.Text = dlg.AffinityHex;
    }

    private void OscCheck_Changed(object sender, RoutedEventArgs e)
        => UpdateOscFields();

    private void UpdateOscFields()
    {
        var enabled = _oscCheck.IsChecked == true;
        _oscInPortBox.IsEnabled = enabled;
        _oscOutIpBox.IsEnabled = enabled;
        _oscOutPortBox.IsEnabled = enabled;
    }

    private void OkBtn_Click(object sender, RoutedEventArgs e)
    {
        // FPS
        int fps = 0;
        var fpsText = _fpsBox.Text.Trim();
        if (!string.IsNullOrEmpty(fpsText) && (!int.TryParse(fpsText, out fps) || fps < 0))
        {
            MessageBox.Show(LocaleManager.Get("S.ErrFpsMsg"), LocaleManager.Get("S.ErrInput"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // OSC
        int oscInPort = int.TryParse(_oscInPortBox.Text, out var parsedInPort) ? parsedInPort : 9001;
        int oscOutPort = int.TryParse(_oscOutPortBox.Text, out var parsedOutPort) ? parsedOutPort : 9000;
        if (_oscCheck.IsChecked == true)
        {
            if (oscInPort < 1 || oscInPort > 65535)
            {
                MessageBox.Show(LocaleManager.Get("S.ErrOscInPortMsg"), LocaleManager.Get("S.ErrInput"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (oscOutPort < 1 || oscOutPort > 65535)
            {
                MessageBox.Show(LocaleManager.Get("S.ErrOscOutPortMsg"), LocaleManager.Get("S.ErrInput"),
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        // Screen dimensions
        int screenWidth = 0, screenHeight = 0;
        if (!string.IsNullOrEmpty(_screenWidthBox.Text.Trim()) &&
            (!int.TryParse(_screenWidthBox.Text, out screenWidth) || screenWidth <= 0))
        {
            MessageBox.Show(LocaleManager.Get("S.ErrScreenWidthMsg"), LocaleManager.Get("S.ErrInput"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (!string.IsNullOrEmpty(_screenHeightBox.Text.Trim()) &&
            (!int.TryParse(_screenHeightBox.Text, out screenHeight) || screenHeight <= 0))
        {
            MessageBox.Show(LocaleManager.Get("S.ErrScreenHeightMsg"), LocaleManager.Get("S.ErrInput"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        // Monitor
        int monitor = 0;
        if (!string.IsNullOrEmpty(_monitorBox.Text.Trim()) &&
            (!int.TryParse(_monitorBox.Text, out monitor) || monitor < 1))
        {
            MessageBox.Show(LocaleManager.Get("S.ErrMonitorMsg"), LocaleManager.Get("S.ErrInput"),
                MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        LaunchOptions = new LaunchOptions
        {
            VrEnabled = _vrCheck.IsChecked == true,
            ExtraArgs = _extraArgsBox.Text.Trim(),
            Fps = fps,
            Midi = _midiBox.Text.Trim(),
            OscEnabled = _oscCheck.IsChecked == true,
            OscInPort = oscInPort,
            OscOutIp = _oscOutIpBox.Text.Trim(),
            OscOutPort = oscOutPort,
            Affinity = _affinityBox.Text.Trim(),
            ProcessPriority = GetComboTag(_processPriorityCombo),
            MainThreadPriority = GetComboTag(_mainThreadPriorityCombo),
            ScreenWidth = screenWidth,
            ScreenHeight = screenHeight,
            ScreenFullscreen = int.TryParse(GetComboTag(_screenFullscreenCombo), out var fs) ? fs : -1,
            Monitor = monitor,
            HwVideoDecoding = GetComboTag(_hwVideoDecodingCombo),
            DisableAmdStutterWorkaround = _disableAmdStutterCheck.IsChecked == true,
            WatchWorlds = _watchWorldsCheck.IsChecked == true,
            WatchAvatars = _watchAvatarsCheck.IsChecked == true,
            DebugGui = _debugGuiCheck.IsChecked == true,
            SdkLogLevels = _sdkLogLevelsCheck.IsChecked == true,
            UdonDebugLogging = _udonDebugLoggingCheck.IsChecked == true,
            SkipRegistryInstall = _skipRegistryInstallCheck.IsChecked == true,
            EnforceWorldServerChecks = _enforceWorldServerChecksCheck.IsChecked == true,
            IgnoreTrackers = _ignoreTrackersBox.Text.Trim(),
        };

        DialogResult = true;
    }

    private static string GetComboTag(ComboBox combo)
        => ((ComboBoxItem?)combo.SelectedItem)?.Tag as string ?? "";

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
        => DialogResult = false;
}
