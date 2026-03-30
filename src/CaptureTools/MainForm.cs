using System.Text.RegularExpressions;

namespace CaptureTools;

public partial class MainForm : Form
{
    private const int HotkeyId = 0x5300;

    private readonly FindTextCore _core = new();
    private readonly string _screenshotDirectory = Path.Combine(Path.GetTempPath(), "Ahk_ScreenShot");
    private string? _registeredHotkey;

    public MainForm()
    {
        InitializeComponent();
        _cmbHotkey.SelectedIndex = 0;
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        UnregisterHotkey();
        _core.Dispose();
        base.OnFormClosed(e);
    }

    private void CaptureFixed()
    {
        var rect = SelectionOverlayForm.SelectFixedArea((int)_numWidth.Value, (int)_numHeight.Value, this);
        if (rect is null)
        {
            return;
        }

        using var bitmap = Utilities.CaptureScreen(rect.Value);
        OpenCaptureTool(bitmap, rect.Value.Location);
    }

    private void CaptureFree()
    {
        var rect = SelectionOverlayForm.SelectArea(this);
        if (rect is null)
        {
            return;
        }

        using var bitmap = Utilities.CaptureScreen(rect.Value);
        OpenCaptureTool(bitmap, rect.Value.Location);
    }

    private void OpenCaptureTool(Bitmap bitmap, Point location)
    {
        using var dialog = new CaptureToolForm(_core, bitmap, location, _screenshotDirectory);
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        if (dialog.ResultAction == CaptureResultAction.Ok)
        {
            _txtScr.Text = dialog.ResultScriptText + Environment.NewLine;
        }
        else if (dialog.ResultAction is CaptureResultAction.SplitAdd or CaptureResultAction.AllAdd)
        {
            AppendScript(dialog.ResultScriptText);
        }

        if (!string.IsNullOrWhiteSpace(dialog.ResultTemplateText))
        {
            _txtClipText.Text = dialog.ResultTemplateText;
            _txtMyPic.Text = _core.ASCII(dialog.ResultTemplateText);
        }
    }

    private void AppendScript(string text)
    {
        if (string.IsNullOrWhiteSpace(_txtScr.Text))
        {
            _txtScr.Text = text;
            return;
        }

        _txtScr.AppendText(Environment.NewLine + text);
    }

    private void TestCode(string source)
    {
        var template = Utilities.ExtractFirstTemplate(source);
        if (string.IsNullOrWhiteSpace(template))
        {
            MessageBox.Show(this, "没有找到可测试的模板字符串。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var results = _core.FindText(template, 0, 0, 0, 0, 0, 0, true, true);
        if (results.Count == 0)
        {
            MessageBox.Show(this, "未找到匹配结果。", "测试", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var hit = results[0];
        RangeFlashForm.ShowFlash(new Rectangle(hit.X1, hit.Y1, hit.Width, hit.Height), Color.Red, 3, 800);
        Clipboard.SetText($"{hit.X},{hit.Y}");
        MessageBox.Show(this, $"找到 {results.Count} 个结果\r\n位置: {hit.X}, {hit.Y}\r\n结果: <{hit.Id}>", "测试", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void CopyCode()
    {
        var text = string.IsNullOrWhiteSpace(_txtScr.SelectedText) ? _txtScr.Text : _txtScr.SelectedText;
        if (!_chkAddFunc.Checked)
        {
            text = Regex.Replace(text, @"(?is)\n\s*class\s+FindTextClass.*$", string.Empty);
        }

        Clipboard.SetText(Utilities.NormalizeLineEndings(text));
    }

    private void GetRange()
    {
        var rect = SelectionOverlayForm.SelectArea(this);
        if (rect is null)
        {
            return;
        }

        var value = $"{rect.Value.Left}, {rect.Value.Top}, {rect.Value.Right - 1}, {rect.Value.Bottom - 1}";
        _txtOffset.Text = value;
        var rangeRegex = new Regex(@"(?i)(FindText\([^\n]*?)([^(,\n]*,){4}([^,\n]*,[^,\n]*,[^,\n]*Text)");
        _txtScr.Text = rangeRegex.Replace(_txtScr.Text, $"$1 {value},$3", 1);
    }

    private void GetOffset(bool useClipText)
    {
        var rect = SelectionOverlayForm.SelectArea(this);
        if (rect is null)
        {
            return;
        }

        var template = Utilities.ExtractFirstTemplate(useClipText ? _txtClipText.Text : _txtScr.Text);
        if (string.IsNullOrWhiteSpace(template))
        {
            MessageBox.Show(this, "没有可用于偏移计算的模板。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var results = _core.FindText(template, 0, 0, 0, 0, 0, 0, true, false);
        if (results.Count == 0)
        {
            MessageBox.Show(this, "当前屏幕未找到该模板。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var hit = results[0];
        var offset = $"X+{((rect.Value.Left + rect.Value.Right) / 2) - hit.X}, Y+{((rect.Value.Top + rect.Value.Bottom) / 2) - hit.Y}".Replace("+-", "-");
        _txtOffset.Text = offset;
        if (!useClipText)
        {
            var clickRegex = new Regex(@"(?i)(Click\s*\()[^,\n]*,[^,)\n]*");
            _txtScr.Text = clickRegex.Replace(_txtScr.Text, $"$1{offset}", 1);
        }
    }

    private void PasteClipTemplate()
    {
        if (!Clipboard.ContainsText())
        {
            return;
        }

        var text = Clipboard.GetText();
        var template = Utilities.ExtractFirstTemplate(text);
        if (string.IsNullOrWhiteSpace(template))
        {
            return;
        }

        _txtClipText.Text = template;
        _txtMyPic.Text = _core.ASCII(template);
    }

    private void TrimAscii(char side)
    {
        var rows = _txtMyPic.Text.Replace("\r", string.Empty).Split('\n', StringSplitOptions.RemoveEmptyEntries).ToList();
        if (rows.Count == 0)
        {
            return;
        }

        switch (side)
        {
            case 'U':
                rows.RemoveAt(0);
                break;
            case 'D':
                rows.RemoveAt(rows.Count - 1);
                break;
            case 'L':
                rows = rows.Select(row => row.Length > 0 ? row[1..] : row).ToList();
                break;
            case 'R':
                rows = rows.Select(row => row.Length > 0 ? row[..^1] : row).ToList();
                break;
        }

        _txtMyPic.Text = string.Join(Environment.NewLine, rows);
    }

    private void UpdateTemplateFromAscii()
    {
        var match = Regex.Match(_txtScr.Text, @"(<[^>\n]*>[^$\n]+\$)\d+\.([\w+/]+)");
        if (!match.Success)
        {
            return;
        }

        var rows = _txtMyPic.Text.Replace("\r", string.Empty).Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (rows.Length == 0)
        {
            return;
        }

        var width = rows[0].Length;
        var bits = string.Concat(rows.Select(row => row.Replace('_', '0').Replace('0', '1')));
        var replacement = $"{match.Groups[1].Value}{width}.{FindTextCore.BitToBase64(bits)}";
        var templateRegex = new Regex(Regex.Escape(match.Value));
        _txtScr.Text = templateRegex.Replace(_txtScr.Text, replacement, 1);
    }

    private void SyncAsciiFromCurrentScript()
    {
        var template = Utilities.ExtractFirstTemplate(_txtScr.Text);
        if (!string.IsNullOrWhiteSpace(template))
        {
            _txtMyPic.Text = _core.ASCII(template);
        }
    }

    private void ApplyHotkey()
    {
        UnregisterHotkey();
        var key = _cmbHotkey.SelectedItem?.ToString();
        if (string.IsNullOrWhiteSpace(key))
        {
            _txtNowHotkey.Text = string.Empty;
            return;
        }

        if (!Enum.TryParse<Keys>(key, true, out var keys))
        {
            MessageBox.Show(this, "无法解析热键。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (!NativeMethods.RegisterHotKey(Handle, HotkeyId, 0, (uint)keys))
        {
            MessageBox.Show(this, "注册热键失败。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _registeredHotkey = key;
        _txtNowHotkey.Text = key;
    }

    private void UnregisterHotkey()
    {
        NativeMethods.UnregisterHotKey(Handle, HotkeyId);
        _registeredHotkey = null;
    }

    protected override void WndProc(ref Message m)
    {
        if (m.Msg == NativeMethods.WM_HOTKEY && m.WParam.ToInt32() == HotkeyId)
        {
            SaveHotkeyScreenshot();
        }

        base.WndProc(ref m);
    }

    private void SaveHotkeyScreenshot()
    {
        Directory.CreateDirectory(_screenshotDirectory);
        var rect = SelectionOverlayForm.SelectFixedArea((int)_numWidth.Value, (int)_numHeight.Value, this);
        if (rect is null)
        {
            return;
        }

        using var bitmap = Utilities.CaptureScreen(rect.Value);
        var file = Path.Combine(_screenshotDirectory, $"{Directory.GetFiles(_screenshotDirectory, "*.bmp").Length + 1:000}.bmp");
        Utilities.SaveBitmap(bitmap, file);
        MessageBox.Show(this, $"截屏成功\r\n{file}", "截屏热键", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    private void buttonApplyHotkey_Click(object sender, EventArgs e) => ApplyHotkey();
    private void buttonCapture_Click(object sender, EventArgs e) => CaptureFixed();
    private void buttonCaptureS_Click(object sender, EventArgs e) => CaptureFree();
    private void buttonTest_Click(object sender, EventArgs e)
    {
        //var strText = "|<>2229C2-0.90$64.0000080000000001U00000000000000000000000000000000000000000000000kM1k1k07075000M0042104010U00M004E04000100010000004001400M000E040E00A001007100080040000000000E040080000100DUUEE80U4000Vk0C0TksS00000000000000000000020U000000003sU";
        //strText = "|<>*166$63.00000A0000000001k000000000000000000000000000000000000UE000U0000wC0z1w0rk7lUkMM1UDa1XA6210A0kMM9UkE81U6331A6300A0kMM9UkD01U631XA60C0A0kMDlUk0M1U6330A641UA0kMM1UkkA1U631zAD330A0kMM8ykTkDwT7a0000000000k00000000031000000000Dw";
        //var matches = this._core.FindText(strText, 0, 0, 2560, 1440, 0, 0);
        TestCode(_txtScr.Text);
    }
    private void buttonCopy_Click(object sender, EventArgs e) => CopyCode();
    private void buttonGetRange_Click(object sender, EventArgs e) => GetRange();
    private void buttonGetOffset_Click(object sender, EventArgs e) => GetOffset(false);
    private void buttonGetClipOffset_Click(object sender, EventArgs e) => GetOffset(true);
    private void buttonPaste_Click(object sender, EventArgs e) => PasteClipTemplate();
    private void buttonTestClip_Click(object sender, EventArgs e) => TestCode(_txtClipText.Text);
    private void buttonCopyOffset_Click(object sender, EventArgs e) => Clipboard.SetText(_txtOffset.Text);
    private void buttonTrimLeft_Click(object sender, EventArgs e) => TrimAscii('L');
    private void buttonTrimRight_Click(object sender, EventArgs e) => TrimAscii('R');
    private void buttonTrimUp_Click(object sender, EventArgs e) => TrimAscii('U');
    private void buttonTrimDown_Click(object sender, EventArgs e) => TrimAscii('D');
    private void buttonUpdateAscii_Click(object sender, EventArgs e) => UpdateTemplateFromAscii();
    private void _txtScr_TextChanged(object sender, EventArgs e) => SyncAsciiFromCurrentScript();
}