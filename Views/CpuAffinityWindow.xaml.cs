using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace DekapuSkillLauncher.Views;

public partial class CpuAffinityWindow : Window
{
    private readonly int _coreCount;
    private readonly int _hexDigits;
    private readonly List<ToggleButton> _coreBoxes = [];
    private bool _updating;
    public string AffinityHex { get; private set; } = "";

    public CpuAffinityWindow(string currentAffinity)
    {
        InitializeComponent();

        _coreCount = Math.Min(Environment.ProcessorCount, 64);
        _hexDigits = (_coreCount + 3) / 4;

        for (int i = 0; i < _coreCount; i++)
        {
            var btn = new ToggleButton
            {
                Content = $"{i}",
                Style = (Style)FindResource("CoreToggleStyle"),
                Tag = i,
            };
            btn.Checked += CoreBox_Changed;
            btn.Unchecked += CoreBox_Changed;
            _coreBoxes.Add(btn);
            _corePanel.Children.Add(btn);
        }

        LoadFromHex(currentAffinity);

        DataObject.AddPastingHandler(_hexBox, HexBox_Pasting);
    }

    private static bool IsHexChar(char c)
        => (c >= '0' && c <= '9') || (c >= 'a' && c <= 'f') || (c >= 'A' && c <= 'F');

    private void HexBox_PreviewTextInput(object sender, TextCompositionEventArgs e)
        => e.Handled = !e.Text.All(IsHexChar);

    private void HexBox_Pasting(object sender, DataObjectPastingEventArgs e)
    {
        if (e.DataObject.GetDataPresent(typeof(string)))
        {
            var text = (string)e.DataObject.GetData(typeof(string));
            if (!text.All(IsHexChar))
                e.CancelCommand();
        }
        else
        {
            e.CancelCommand();
        }
    }

    private static ulong ParseHex(string text)
    {
        var s = text.Trim();
        ulong.TryParse(s, NumberStyles.HexNumber, null, out var v);
        return v;
    }

    private void LoadFromHex(string hex)
    {
        _updating = true;
        var mask = ParseHex(hex);

        for (int i = 0; i < _coreCount; i++)
            _coreBoxes[i].IsChecked = (mask & (1UL << i)) != 0;

        _hexBox.Text = mask == 0 ? "" : mask.ToString($"X{_hexDigits}");
        _updating = false;
    }

    private ulong MaskFromCheckBoxes()
    {
        ulong mask = 0;
        for (int i = 0; i < _coreCount; i++)
            if (_coreBoxes[i].IsChecked == true)
                mask |= 1UL << i;
        return mask;
    }

    private void CoreBox_Changed(object sender, RoutedEventArgs e)
    {
        if (_updating) return;
        _updating = true;
        var mask = MaskFromCheckBoxes();
        _hexBox.Text = mask == 0 ? "" : mask.ToString($"X{_hexDigits}");
        _updating = false;
    }

    private void HexBox_TextChanged(object sender, TextChangedEventArgs e)
    {
        if (_updating) return;
        _updating = true;

        var raw = ParseHex(_hexBox.Text);

        ulong validMask = _coreCount < 64 ? (1UL << _coreCount) - 1 : ulong.MaxValue;
        var mask = raw & validMask;

        for (int i = 0; i < _coreCount; i++)
            _coreBoxes[i].IsChecked = (mask & (1UL << i)) != 0;

        if ((raw & ~validMask) != 0)
        {
            var caretPos = _hexBox.CaretIndex;
            _hexBox.Text = mask == 0 ? "" : mask.ToString($"X{_hexDigits}");
            _hexBox.CaretIndex = Math.Min(caretPos, _hexBox.Text.Length);
        }

        _updating = false;
    }

    private void SelectAll_Click(object sender, RoutedEventArgs e)
    {
        _updating = true;
        foreach (var cb in _coreBoxes) cb.IsChecked = true;
        _hexBox.Text = MaskFromCheckBoxes().ToString($"X{_hexDigits}");
        _updating = false;
    }

    private void ClearAll_Click(object sender, RoutedEventArgs e)
    {
        _updating = true;
        foreach (var cb in _coreBoxes) cb.IsChecked = false;
        _hexBox.Text = "";
        _updating = false;
    }

    private void OkBtn_Click(object sender, RoutedEventArgs e)
    {
        AffinityHex = _hexBox.Text.Trim();
        DialogResult = true;
    }

    private void CancelBtn_Click(object sender, RoutedEventArgs e)
        => DialogResult = false;
}
