using System.Diagnostics;

namespace SimpleLauncherWinForms;

public class MainForm : Form
{
    private const string GroupId = "grp_f664b62c-df1a-4ad4-a1df-2b9df679bc04";
    private static readonly string LauncherPath =
        @"C:\Program Files (x86)\Steam\steamapps\common\VRChat\launch.exe";
    private static readonly TimeZoneInfo TZ =
        TimeZoneInfo.FindSystemTimeZoneById("Tokyo Standard Time");

    private readonly InstanceService _ctrl = new();
    private readonly AppSettings _settings = AppSettings.Load();
    private DataGridView _table = null!;
    private Label _updatedLabel = null!;
    private Button _launchBtn = null!;

    public MainForm()
    {
        Text = "ブッパ連合起動ツール";
        Font = new Font("Yu Gothic UI", 10f);
        Icon = Icon.ExtractAssociatedIcon(Application.ExecutablePath);
        FormBorderStyle = FormBorderStyle.FixedSingle;
        MaximizeBox = false;
        ClientSize = new Size(400, 190);

        BuildUI();
        Load += async (_, _) => await UpdateInstancesAsync(refresh: true);
        FormClosing += (_, _) => _settings.Save();
    }

    private void BuildUI()
    {
        var main = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            Padding = new Padding(8),
            RowCount = 3,
            ColumnCount = 2,
        };
        main.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));   // 左: メインコンテンツ
        main.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 110f));  // 右: OSCボタン
        main.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // header
        main.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));  // table
        main.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // footer
        Controls.Add(main);

        // --- ヘッダー（更新・起動のみ）---
        var header = new FlowLayoutPanel
        {
            AutoSize = true,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = false,
            Margin = new Padding(0, 0, 0, 5),
        };

        var refreshBtn = new Button { Text = "更新", Width = 64, Height = 26, Margin = new Padding(0, 0, 4, 0) };
        refreshBtn.Click += async (_, _) => await UpdateInstancesAsync(refresh: true);
        header.Controls.Add(refreshBtn);

        _launchBtn = new Button { Text = "起動", Width = 64, Height = 26, Margin = new Padding(0, 0, 4, 0) };
        _launchBtn.Click += (_, _) => LaunchSelected();
        header.Controls.Add(_launchBtn);

        var settingsBtn = new Button { Text = "設定", Width = 64, Height = 26, Margin = new Padding(0) };
        settingsBtn.Click += (_, _) => OpenSettings();
        header.Controls.Add(settingsBtn);

        main.Controls.Add(header, 0, 0);

        // --- テーブル ---
        _table = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            ReadOnly = true,
            SelectionMode = DataGridViewSelectionMode.FullRowSelect,
            MultiSelect = false,
            RowHeadersVisible = false,
            AllowUserToResizeRows = false,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.None,
            ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize,
            ScrollBars = ScrollBars.Vertical,
        };

        _table.Columns.Add(new DataGridViewTextBoxColumn { Name = "id", Visible = false });
        _table.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "name",
            HeaderText = "名前",
            Width = 200,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleLeft },
        });
        _table.Columns.Add(new DataGridViewTextBoxColumn
        {
            Name = "count",
            HeaderText = "人数",
            Width = 60,
            DefaultCellStyle = { Alignment = DataGridViewContentAlignment.MiddleCenter },
        });

        _table.SelectionChanged += (_, _) =>
        {
            if (_table.SelectedRows.Count == 0) { _launchBtn.Enabled = true; return; }
            _launchBtn.Enabled = _table.SelectedRows[0].Tag as string != "closed";
        };

        main.Controls.Add(_table, 0, 1);

        // --- フッター（更新日時）---
        var footer = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 1,
            ColumnCount = 1,
            Margin = new Padding(0, 4, 0, 0),
            Height = 26,
        };
        footer.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));

        _updatedLabel = new Label
        {
            Text = "更新日時: -",
            AutoSize = false,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
        };
        footer.Controls.Add(_updatedLabel, 0, 0);

        main.Controls.Add(footer, 0, 2);

        // --- 右側OSCボタン（全行スパン）---
        var oscPanel = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            RowCount = 2,
            ColumnCount = 1,
            Margin = new Padding(6, 0, 0, 0),
        };
        oscPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
        oscPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));

        var fixedBtn = new Button
        {
            Text = "固定",
            Dock = DockStyle.Fill,
            Font = new Font("Yu Gothic UI", 14f, FontStyle.Bold),
            Margin = new Padding(0, 0, 0, 4),
        };
        var releaseBtn = new Button
        {
            Text = "解放",
            Dock = DockStyle.Fill,
            Font = new Font("Yu Gothic UI", 14f, FontStyle.Bold),
            Margin = new Padding(0),
        };

        fixedBtn.Click += async (_, _) =>
        {
            fixedBtn.Enabled = releaseBtn.Enabled = false;
            try
            {
                await OscSender.SendAsync("127.0.0.1", _settings.OscPort, _settings.OscAddress, 0);
                await Task.Delay(100);
                await OscSender.SendAsync("127.0.0.1", _settings.OscPort, _settings.OscAddress, 1);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OSCエラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                fixedBtn.Enabled = releaseBtn.Enabled = true;
            }
        };

        releaseBtn.Click += async (_, _) =>
        {
            fixedBtn.Enabled = releaseBtn.Enabled = false;
            try
            {
                await OscSender.SendAsync("127.0.0.1", _settings.OscPort, _settings.OscAddress, 0);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "OSCエラー", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            finally
            {
                fixedBtn.Enabled = releaseBtn.Enabled = true;
            }
        };

        oscPanel.Controls.Add(fixedBtn, 0, 0);
        oscPanel.Controls.Add(releaseBtn, 0, 1);

        main.SetRowSpan(oscPanel, 3);
        main.Controls.Add(oscPanel, 1, 0);
    }

    private void OpenSettings()
    {
        using var dlg = new SettingsForm(
            _settings.Profile, _settings.ExtraArgs,
            _settings.OscPort, _settings.OscAddress);
        if (dlg.ShowDialog(this) != DialogResult.OK) return;

        _settings.Profile    = dlg.Profile;
        _settings.ExtraArgs  = dlg.ExtraArgs;
        _settings.OscPort    = dlg.OscPort;
        _settings.OscAddress = dlg.OscAddress;
        _settings.Save();
    }

    private async Task UpdateInstancesAsync(bool refresh = false)
    {
        InstanceCache cache;
        try
        {
            cache = await _ctrl.GetGroupInstancesAsync(GroupId, refresh);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        _table.Rows.Clear();
        foreach (var inst in cache.Instances
            .OrderBy(i => i.ClosedAt.HasValue ? 1 : 0)
            .ThenByDescending(i => i.UserCount))
        {
            var rowIndex = _table.Rows.Add(inst.Id, inst.DisplayName ?? inst.Name, inst.UserCount);
            if (inst.ClosedAt.HasValue)
            {
                var row = _table.Rows[rowIndex];
                row.DefaultCellStyle.BackColor = Color.LightGray;
                row.DefaultCellStyle.ForeColor = Color.DarkGray;
                row.Tag = "closed";
            }
        }

        var updatedJst = TimeZoneInfo.ConvertTime(cache.UpdatedAt, TZ);
        _updatedLabel.Text = $"更新日時：{updatedJst:yyyy-MM-dd HH:mm:ss}";
    }

    private void LaunchSelected()
    {
        if (_table.SelectedRows.Count == 0)
        {
            MessageBox.Show("インスタンスを選択してください", "警告",
                MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        var instId = _table.SelectedRows[0].Cells["id"].Value?.ToString()!;
        InstanceInfo inst;
        try
        {
            inst = _ctrl.GetInstanceById(GroupId, instId);
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message, "エラー", MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        if (inst.ClosedAt.HasValue)
        {
            MessageBox.Show("すでにクローズされています", "エラー",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var profile = _settings.Profile;
        var name = inst.DisplayName ?? inst.Name;

        var confirm = MessageBox.Show(
            $"以下のインスタンスで起動しますか？\n\n名前: {name}\nプロファイル: {profile}",
            "確認", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
        if (confirm != DialogResult.Yes)
            return;

        if (!File.Exists(LauncherPath))
        {
            MessageBox.Show($"VRChat launcher not found:\n{LauncherPath}", "エラー",
                MessageBoxButtons.OK, MessageBoxIcon.Error);
            return;
        }

        var url = $"vrchat://launch?ref=VRCQuickLauncher&id={inst.Id}&shortName={inst.ShortName}";
        var psi = new ProcessStartInfo
        {
            FileName = LauncherPath,
            UseShellExecute = false,
        };
        psi.ArgumentList.Add($"--profile={profile}");
        psi.ArgumentList.Add(url);
        psi.ArgumentList.Add("--no-vr");
        foreach (var arg in _settings.ExtraArgs.Split(' ', StringSplitOptions.RemoveEmptyEntries))
            psi.ArgumentList.Add(arg);
        Process.Start(psi);
    }
}
