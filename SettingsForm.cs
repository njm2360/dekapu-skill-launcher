namespace SimpleLauncherWinForms;

public class SettingsForm : Form
{
    private readonly NumericUpDown _profileSpinner;
    private readonly TextBox _extraArgsBox;
    private readonly NumericUpDown _oscPortSpinner;
    private readonly TextBox _oscAddressBox;

    public int Profile => (int)_profileSpinner.Value;
    public string ExtraArgs => _extraArgsBox.Text.Trim();
    public int OscPort => (int)_oscPortSpinner.Value;
    public string OscAddress => _oscAddressBox.Text.Trim();

    public SettingsForm(int profile, string extraArgs, int oscPort, string oscAddress)
    {
        Text = "起動設定";
        Font = new Font("Yu Gothic UI", 10f);
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        StartPosition = FormStartPosition.CenterParent;
        ClientSize = new Size(360, 200);

        var main = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(12),
            RowCount = 5,
            ColumnCount = 2,
        };
        main.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
        main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
        main.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        main.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        main.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        main.RowStyles.Add(new RowStyle(SizeType.AutoSize));
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
        Controls.Add(main);

        // プロファイル
        main.Controls.Add(new Label
        {
            Text = "プロファイル:",
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 6, 8, 6),
        }, 0, 0);

        _profileSpinner = new NumericUpDown
        {
            Minimum = 0,
            Maximum = 9,
            Width = 60,
            Value = profile,
            Margin = new Padding(0, 4, 0, 4),
        };
        main.Controls.Add(_profileSpinner, 1, 0);

        // 起動オプション
        main.Controls.Add(new Label
        {
            Text = "起動オプション:",
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 6, 8, 6),
        }, 0, 1);

        _extraArgsBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Text = extraArgs,
            Margin = new Padding(0, 4, 0, 4),
        };
        main.Controls.Add(_extraArgsBox, 1, 1);

        // OSCポート
        main.Controls.Add(new Label
        {
            Text = "OSCポート:",
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 6, 8, 6),
        }, 0, 2);

        _oscPortSpinner = new NumericUpDown
        {
            Minimum = 1,
            Maximum = 65535,
            Width = 80,
            Value = oscPort,
            Margin = new Padding(0, 4, 0, 4),
        };
        main.Controls.Add(_oscPortSpinner, 1, 2);

        // OSCアドレス
        main.Controls.Add(new Label
        {
            Text = "OSCアドレス:",
            AutoSize = true,
            TextAlign = ContentAlignment.MiddleLeft,
            Margin = new Padding(0, 6, 8, 6),
        }, 0, 3);

        _oscAddressBox = new TextBox
        {
            Dock = DockStyle.Fill,
            Text = oscAddress,
            Margin = new Padding(0, 4, 0, 4),
        };
        main.Controls.Add(_oscAddressBox, 1, 3);

        // ボタン行
        var btnPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Margin = new Padding(0, 6, 0, 0),
        };
        main.SetColumnSpan(btnPanel, 2);
        main.Controls.Add(btnPanel, 0, 4);

        var cancelBtn = new Button { Text = "キャンセル", Width = 90, Height = 26, DialogResult = DialogResult.Cancel, Margin = new Padding(0) };
        var okBtn = new Button { Text = "OK", Width = 72, Height = 26, DialogResult = DialogResult.OK, Margin = new Padding(0, 0, 4, 0) };
        btnPanel.Controls.Add(cancelBtn);
        btnPanel.Controls.Add(okBtn);

        AcceptButton = okBtn;
        CancelButton = cancelBtn;
    }
}
