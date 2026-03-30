using System.Globalization;
using System.Text;

namespace WinFormsApp1;

internal enum CaptureResultAction
{
    None,
    Ok,
    SplitAdd,
    AllAdd
}

internal sealed partial class CaptureToolForm : Form
{
    private readonly FindTextCore _core;
    private readonly string _screenshotDirectory;
    private readonly Bitmap _originalBitmap;
    private readonly FindTextCaptureState _state = new();
    private Rectangle _pictureSelection = Rectangle.Empty;
    private Point _pictureSelectionStart;
    private bool _pictureDragging;
    private IntPtr _boundWindow;
    private int _boundMode;

    public CaptureToolForm(FindTextCore core, Bitmap bitmap, Point screenLocation, string screenshotDirectory)
    {
        _core = core;
        _screenshotDirectory = screenshotDirectory;
        _originalBitmap = new Bitmap(bitmap);
        _state.Load(bitmap, screenLocation.X, screenLocation.Y);
        InitializeComponent();
        HookEvents();
        LoadStateIntoUi();
        LoadSavedImages();
    }

    public CaptureResultAction ResultAction { get; private set; }
    public string ResultTemplateText { get; private set; } = string.Empty;
    public string ResultScriptText { get; private set; } = string.Empty;

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _originalBitmap.Dispose();
        _state.Dispose();
        base.OnFormClosed(e);
    }

    private void HookEvents()
    {
        _chkModify.CheckedChanged += (_, _) => _grid.SetModifyMode(_chkModify.Checked);
        _chkMultiColor.CheckedChanged += (_, _) =>
        {
            if (!_chkMultiColor.Checked)
            {
                _state.MultiColorPoints.Clear();
                SetStatus("已清空多点颜色记录");
            }
        };
        _chkFindShape.CheckedChanged += (_, _) =>
        {
            if (_chkFindShape.Checked)
            {
                _chkMultiColor.Checked = true;
            }
        };

        _grid.CellMouseAction += GridOnCellMouseAction;
        _gridHScroll.Scroll += (_, _) => _grid.SetViewport(_gridHScroll.Value, _gridVScroll.Value);
        _gridVScroll.Scroll += (_, _) => _grid.SetViewport(_gridHScroll.Value, _gridVScroll.Value);

        _pictureBox.MouseDown += PictureBoxOnMouseDown;
        _pictureBox.MouseMove += PictureBoxOnMouseMove;
        _pictureBox.MouseUp += PictureBoxOnMouseUp;
        _pictureBox.Paint += PictureBoxOnPaint;

        _savedImagesList.DoubleClick += (_, _) => LoadSelectedSavedImage();

        Button("buttonLoadImage").Click += (_, _) => LoadImageFromDisk();
        Button("buttonSaveImage").Click += (_, _) => SaveCurrentImage();
        Button("buttonOpenDir").Click += (_, _) => OpenDirectory();
        Button("buttonClearImages").Click += (_, _) => ClearSavedImages();

        Button("buttonBind0").Click += (_, _) => BindWindow(0);
        Button("buttonBind1").Click += (_, _) => BindWindow(1);
        Button("buttonBind2").Click += (_, _) => BindWindow(2);
        Button("buttonBind3").Click += (_, _) => BindWindow(3);
        Button("buttonBind4").Click += (_, _) => BindWindow(4);
        Button("buttonSaveTrimmed").Click += (_, _) => SaveTrimmedImage();

        Button("buttonGray").Click += (_, _) => ApplyGrayThreshold();
        Button("buttonGrayDiff").Click += (_, _) => ApplyGrayDiff();
        Button("buttonAddSimilarity").Click += (_, _) => AddColorBySimilarity();
        Button("buttonAddDiff").Click += (_, _) => AddColorByDiff();
        Button("buttonUndoColor").Click += (_, _) => UndoColorRule();
        Button("buttonColorBinary").Click += (_, _) => ApplyColorList();
        Button("buttonColorPosition").Click += (_, _) => ApplyColorPosition();
        Button("buttonUndoPoint").Click += (_, _) => { _state.UndoMultiColorPoint(); SetStatus("已撤销一次颜色点记录"); };

        Button("buttonRepUp").Click += (_, _) => { _state.RepUp(); RefreshAllViews(); };
        Button("buttonCutUp").Click += (_, _) => { _state.CutUp(); RefreshAllViews(); };
        Button("buttonCutUp3").Click += (_, _) => Repeat(3, _state.CutUp);
        Button("buttonRepLeft").Click += (_, _) => { _state.RepLeft(); RefreshAllViews(); };
        Button("buttonCutLeft").Click += (_, _) => { _state.CutLeft(); RefreshAllViews(); };
        Button("buttonCutLeft3").Click += (_, _) => Repeat(3, _state.CutLeft);
        Button("buttonAutoCrop").Click += (_, _) => { _state.AutoCrop(); RefreshAllViews(); };
        Button("buttonRepRight").Click += (_, _) => { _state.RepRight(); RefreshAllViews(); };
        Button("buttonCutRight").Click += (_, _) => { _state.CutRight(); RefreshAllViews(); };
        Button("buttonCutRight3").Click += (_, _) => Repeat(3, _state.CutRight);
        Button("buttonRepDown").Click += (_, _) => { _state.RepDown(); RefreshAllViews(); };
        Button("buttonCutDown").Click += (_, _) => { _state.CutDown(); RefreshAllViews(); };
        Button("buttonCutDown3").Click += (_, _) => Repeat(3, _state.CutDown);
        Button("buttonReset").Click += (_, _) => ResetState();

        Button("buttonSplitAdd").Click += (_, _) => Finish(CaptureResultAction.SplitAdd);
        Button("buttonAllAdd").Click += (_, _) => Finish(CaptureResultAction.AllAdd);
        Button("buttonOk").Click += (_, _) => Finish(CaptureResultAction.Ok);
        Button("buttonCancel").Click += (_, _) => Close();
    }

    private Button Button(string name) => Controls.Find(name, true).OfType<Button>().First();

    private void LoadStateIntoUi()
    {
        _pictureBox.Image?.Dispose();
        _pictureBox.Image = new Bitmap(_state.SourceBitmap);
        _pictureBox.Size = _state.SourceBitmap.Size;
        _grid.BindState(_state);
        _gridHScroll.Maximum = Math.Max(0, _state.Width - PixelGridControl.ViewColumns);
        _gridVScroll.Maximum = Math.Max(0, _state.Height - PixelGridControl.ViewRows);
        _gridHScroll.Enabled = _state.Width > PixelGridControl.ViewColumns;
        _gridVScroll.Enabled = _state.Height > PixelGridControl.ViewRows;
        RefreshAllViews();
    }

    private void RefreshAllViews()
    {
        _grid.Invalidate();
        _pictureBox.Invalidate();
    }

    private void GridOnCellMouseAction(object? sender, GridCellEventArgs e)
    {
        if (!_state.IsWithin(e.X, e.Y))
        {
            return;
        }

        _state.SetSelected(e.X, e.Y);
        var index = _state.SelectedIndex;
        if (index >= 0)
        {
            var color = _state.GetColor(index);
            _txtSelGray.Text = _state.GetGray(index).ToString(CultureInfo.InvariantCulture);
            _txtSelColor.Text = $"0x{(_state.Colors[index] & 0xFFFFFF):X6}";
            _txtSelR.Text = color.R.ToString(CultureInfo.InvariantCulture);
            _txtSelG.Text = color.G.ToString(CultureInfo.InvariantCulture);
            _txtSelB.Text = color.B.ToString(CultureInfo.InvariantCulture);
        }

        if (_chkModify.Checked && _state.BinaryReady && e.Button == MouseButtons.Left)
        {
            _state.ToggleBinary(e.X, e.Y);
            SetStatus($"已修改像素 ({e.X}, {e.Y}) 的黑白值");
        }
        else if (_chkMultiColor.Checked && e.Button == MouseButtons.Left)
        {
            _state.AddMultiColorPoint(e.X, e.Y);
            SetStatus($"已记录颜色点，当前共 {_state.MultiColorPoints.Count} 个");
        }
        else if (e.Button == MouseButtons.Right)
        {
            _state.ToggleVisibility(e.X, e.Y);
            SetStatus($"已切换像素 ({e.X}, {e.Y}) 的可见状态");
        }

        RefreshAllViews();
    }

    private void PictureBoxOnMouseDown(object? sender, MouseEventArgs e)
    {
        if (_pictureBox.Image is null || e.Button != MouseButtons.Left)
        {
            return;
        }

        _pictureDragging = true;
        _pictureSelectionStart = e.Location;
        _pictureSelection = new Rectangle(e.Location, Size.Empty);
        _pictureBox.Invalidate();
    }

    private void PictureBoxOnMouseMove(object? sender, MouseEventArgs e)
    {
        if (!_pictureDragging)
        {
            return;
        }

        _pictureSelection = Rectangle.FromLTRB(
            Math.Min(_pictureSelectionStart.X, e.X),
            Math.Min(_pictureSelectionStart.Y, e.Y),
            Math.Max(_pictureSelectionStart.X, e.X),
            Math.Max(_pictureSelectionStart.Y, e.Y));
        _pictureBox.Invalidate();
    }

    private void PictureBoxOnMouseUp(object? sender, MouseEventArgs e)
    {
        if (!_pictureDragging)
        {
            return;
        }

        _pictureDragging = false;
        if (_pictureSelection.Width < 3 || _pictureSelection.Height < 3)
        {
            _pictureSelection = Rectangle.Empty;
            _pictureBox.Invalidate();
            return;
        }

        using var cropped = CaptureUtilities.Crop(_state.SourceBitmap, _pictureSelection);
        _state.Load(cropped, _state.ScreenX + _pictureSelection.X, _state.ScreenY + _pictureSelection.Y);
        _pictureSelection = Rectangle.Empty;
        LoadStateIntoUi();
        SetStatus("已从预览图中重新选择子区域");
    }

    private void PictureBoxOnPaint(object? sender, PaintEventArgs e)
    {
        if (_pictureSelection.Width > 0 && _pictureSelection.Height > 0)
        {
            using var pen = new Pen(Color.Red, 2f);
            e.Graphics.DrawRectangle(pen, _pictureSelection);
        }
    }

    private void ApplyGrayThreshold()
    {
        var value = int.TryParse(_txtThreshold.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var threshold) ? threshold : (int?)null;
        var actual = _state.ApplyGrayThreshold(value);
        _txtThreshold.Text = actual.ToString(CultureInfo.InvariantCulture);
        RefreshAllViews();
        SetStatus($"已完成灰度阈值二值化，阈值={actual}");
    }

    private void ApplyGrayDiff()
    {
        if (!int.TryParse(_txtGrayDiff.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var grayDiff))
        {
            MessageBox.Show(this, "请先设定灰度差值！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _state.ApplyGrayDiff(grayDiff);
        RefreshAllViews();
        SetStatus($"已完成灰度差值二值化，差值={grayDiff}");
    }

    private void AddColorBySimilarity()
    {
        if (_state.SelectedIndex < 0)
        {
            MessageBox.Show(this, "请先选择核心颜色！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var color = _state.Colors[_state.SelectedIndex] & 0xFFFFFF;
        var value = (_trackSimilar1.Value / 100d).ToString("0.##", CultureInfo.InvariantCulture);
        _txtColorList.Text = AppendOrReplaceColorRule(_txtColorList.Text, $"{color:X6}-{value}");
        ApplyColorList();
    }

    private void AddColorByDiff()
    {
        if (_state.SelectedIndex < 0)
        {
            MessageBox.Show(this, "请先选择核心颜色！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        var color = _state.Colors[_state.SelectedIndex] & 0xFFFFFF;
        var diff = (int)_numDiffRgb2.Value;
        var value = $"{diff:X2}{diff:X2}{diff:X2}";
        _txtColorList.Text = AppendOrReplaceColorRule(_txtColorList.Text, $"{color:X6}-{value}");
        ApplyColorList();
    }

    private void UndoColorRule()
    {
        var parts = _txtColorList.Text.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        if (parts.Count > 0)
        {
            parts.RemoveAt(parts.Count - 1);
        }

        _txtColorList.Text = string.Join('/', parts);
        SetStatus("已撤销颜色列表中的最后一项");
    }

    private void ApplyColorList()
    {
        if (string.IsNullOrWhiteSpace(_txtColorList.Text))
        {
            MessageBox.Show(this, "请先添加颜色到颜色列表！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _state.ApplyColorList(_txtColorList.Text);
        RefreshAllViews();
        SetStatus("已完成颜色二值化");
    }

    private void ApplyColorPosition()
    {
        if (_state.SelectedIndex < 0)
        {
            MessageBox.Show(this, "请先选择核心颜色！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        _state.ApplyColorPosition(_trackSimilar2.Value / 100d);
        RefreshAllViews();
        SetStatus("已完成颜色位置二值化");
    }

    private void ResetState()
    {
        _state.Load(_originalBitmap, _state.ScreenX, _state.ScreenY);
        LoadStateIntoUi();
        SetStatus("已恢复原始彩色图像");
    }

    private void Repeat(int count, Action action)
    {
        for (var i = 0; i < count; i++)
        {
            action();
        }

        RefreshAllViews();
    }

    private void BindWindow(int mode)
    {
        _boundWindow = BoundWindowCaptureService.GetForegroundWindowHandle();
        _boundMode = mode;
        using var bitmap = BoundWindowCaptureService.Capture(_boundWindow, mode);
        if (bitmap is null)
        {
            MessageBox.Show(this, "无法捕获当前前台窗口。", "提示", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        _state.Load(bitmap, 0, 0);
        LoadStateIntoUi();
        SetStatus($"已绑定窗口 {BoundWindowCaptureService.Describe(_boundWindow)}，模式={mode}");
    }

    private void SaveCurrentImage()
    {
        using var dialog = new SaveFileDialog { Filter = "Bitmap|*.bmp", FileName = "capture.bmp" };
        if (dialog.ShowDialog(this) == DialogResult.OK)
        {
            CaptureUtilities.SaveBitmap(_state.SourceBitmap, dialog.FileName);
            LoadSavedImages();
            SetStatus($"已保存图片到 {dialog.FileName}");
        }
    }

    private void SaveTrimmedImage()
    {
        var bounds = _state.GetVisibleBounds();
        if (bounds == Rectangle.Empty)
        {
            return;
        }

        using var bitmap = CaptureUtilities.Crop(_state.SourceBitmap, bounds);
        Directory.CreateDirectory(_screenshotDirectory);
        var file = Path.Combine(_screenshotDirectory, $"trimmed_{DateTime.Now:yyyyMMdd_HHmmss}.bmp");
        CaptureUtilities.SaveBitmap(bitmap, file);
        LoadSavedImages();
        SetStatus($"已保存裁剪图到 {file}");
    }

    private void LoadImageFromDisk()
    {
        using var dialog = new OpenFileDialog { Filter = "Bitmap|*.bmp;*.png;*.jpg|All files|*.*" };
        if (dialog.ShowDialog(this) != DialogResult.OK)
        {
            return;
        }

        using var bitmap = new Bitmap(dialog.FileName);
        _state.Load(bitmap, 0, 0);
        LoadStateIntoUi();
        SetStatus($"已载入图片 {dialog.FileName}");
    }

    private void LoadSavedImages()
    {
        Directory.CreateDirectory(_screenshotDirectory);
        var files = Directory.EnumerateFiles(_screenshotDirectory, "*.bmp").OrderBy(path => path).ToArray();
        _savedImagesList.Items.Clear();
        foreach (var file in files)
        {
            _savedImagesList.Items.Add(file);
        }
    }

    private void LoadSelectedSavedImage()
    {
        if (_savedImagesList.SelectedItem is not string file || !File.Exists(file))
        {
            return;
        }

        using var bitmap = new Bitmap(file);
        _state.Load(bitmap, 0, 0);
        LoadStateIntoUi();
        SetStatus($"已载入截图 {file}");
    }

    private void ClearSavedImages()
    {
        if (MessageBox.Show(this, "你确定要删除所有的截图吗？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Question) != DialogResult.Yes)
        {
            return;
        }

        foreach (var file in Directory.EnumerateFiles(_screenshotDirectory, "*.bmp"))
        {
            File.Delete(file);
        }

        LoadSavedImages();
        SetStatus("已清空所有保存的截图");
    }

    private void OpenDirectory()
    {
        Directory.CreateDirectory(_screenshotDirectory);
        System.Diagnostics.Process.Start("explorer.exe", _screenshotDirectory);
    }

    private void Finish(CaptureResultAction action)
    {
        string? template;
        if (_chkMultiColor.Checked)
        {
            template = _state.BuildMultiColorTemplate(_txtComment.Text.Trim(), _trackSimilar3.Value / 100d, _chkFindShape.Checked);
        }
        else if (action == CaptureResultAction.SplitAdd)
        {
            if (_state.CurrentModePrefix.StartsWith("#", StringComparison.Ordinal))
            {
                MessageBox.Show(this, "不能用于颜色位置二值化模式, 因为分割后会导致位置错误", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var pieces = _state.SplitTemplateStrings(_txtComment.Text.Trim());
            if (pieces.Count == 0)
            {
                MessageBox.Show(this, "请先将图像二值化！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ResultTemplateText = string.Join(Environment.NewLine, pieces.Select(piece => $"Text.=\"{piece}\""));
            ResultScriptText = ResultTemplateText;
            ResultAction = action;
            DialogResult = DialogResult.OK;
            Close();
            return;
        }
        else
        {
            template = _state.BuildTemplateString(_txtComment.Text.Trim());
        }

        if (string.IsNullOrWhiteSpace(template))
        {
            MessageBox.Show(this, "请先将图像二值化！", "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            return;
        }

        ResultTemplateText = template;
        ResultScriptText = action == CaptureResultAction.Ok ? BuildGeneratedCode(template) : $"Text.=\"{template}\"";
        ResultAction = action;
        DialogResult = DialogResult.OK;
        Close();
    }

    private string BuildGeneratedCode(string template)
    {
        var bounds = _state.GetVisibleBounds();
        var centerX = _state.ScreenX + bounds.Left + (bounds.Width / 2);
        var centerY = _state.ScreenY + bounds.Top + (bounds.Height / 2);
        var sb = new StringBuilder();
        sb.AppendLine("; #Include <FindText>");
        sb.AppendLine("var findText = new FindTextCore();");
        sb.AppendLine($"string text = \"{template.Replace("\\", "\\\\").Replace("\"", "\\\"")}\";");
        sb.AppendLine($"var results = findText.FindText(text, {centerX - 150000}, {centerY - 150000}, {centerX + 150000}, {centerY + 150000});");
        sb.AppendLine("if (results.Count > 0)");
        sb.AppendLine("{");
        sb.AppendLine("    var hit = results[0];");
        sb.AppendLine("    // 可在这里执行点击或其它后续动作");
        sb.AppendLine("}");
        return sb.ToString().TrimEnd();
    }

    private static string AppendOrReplaceColorRule(string existing, string newRule)
    {
        var rules = existing.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries).ToList();
        rules.RemoveAll(rule => rule.StartsWith(newRule[..6], StringComparison.OrdinalIgnoreCase));
        rules.Add(newRule);
        return string.Join('/', rules);
    }

    private void SetStatus(string text)
    {
        _lblStatus.Text = text;
    }
}
