namespace CaptureTools;

internal enum SelectionMode
{
    Free,
    Fixed
}

internal sealed class SelectionOverlayForm : Form
{
    private Point _start;
    private Point _current;
    private bool _dragging;
    private readonly SelectionMode _mode;
    private int _fixedWidth;
    private int _fixedHeight;

    private SelectionOverlayForm(SelectionMode mode, int fixedWidth, int fixedHeight)
    {
        _mode = mode;
        _fixedWidth = Math.Max(1, fixedWidth);
        _fixedHeight = Math.Max(1, fixedHeight);
        Bounds = SystemInformation.VirtualScreen;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        DoubleBuffered = true;
        BackColor = Color.Black;
        Opacity = 0.20d;
        Cursor = Cursors.Cross;
        KeyPreview = true;
    }

    public Rectangle SelectedRectangle { get; private set; } = Rectangle.Empty;

    public static Rectangle? SelectArea(IWin32Window? owner = null)
    {
        using var form = new SelectionOverlayForm(SelectionMode.Free, 1, 1);
        return form.ShowDialog(owner) == DialogResult.OK ? form.SelectedRectangle : null;
    }

    public static Rectangle? SelectFixedArea(int width, int height, IWin32Window? owner = null)
    {
        using var form = new SelectionOverlayForm(SelectionMode.Fixed, width, height);
        return form.ShowDialog(owner) == DialogResult.OK ? form.SelectedRectangle : null;
    }

    protected override bool ShowWithoutActivation => true;

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _start = PointToClient(Cursor.Position);
        _current = _start;
        Focus();
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        if (_mode == SelectionMode.Fixed)
        {
            SelectedRectangle = GetFixedRectangle(e.Location);
            DialogResult = DialogResult.OK;
            Close();
            return;
        }

        _start = e.Location;
        _current = e.Location;
        _dragging = true;
        Invalidate();
    }

    protected override void OnMouseMove(MouseEventArgs e)
    {
        base.OnMouseMove(e);
        _current = e.Location;
        Invalidate();
    }

    protected override void OnMouseUp(MouseEventArgs e)
    {
        base.OnMouseUp(e);
        if (_mode == SelectionMode.Free && _dragging)
        {
            _dragging = false;
            SelectedRectangle = Normalize(_start, e.Location);
            if (SelectedRectangle.Width > 0 && SelectedRectangle.Height > 0)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }
    }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (e.KeyCode == Keys.Escape)
        {
            DialogResult = DialogResult.Cancel;
            Close();
            return;
        }

        if (_mode == SelectionMode.Fixed)
        {
            if (e.KeyCode == Keys.Left && _fixedWidth > 1) _fixedWidth--;
            if (e.KeyCode == Keys.Right) _fixedWidth++;
            if (e.KeyCode == Keys.Up && _fixedHeight > 1) _fixedHeight--;
            if (e.KeyCode == Keys.Down) _fixedHeight++;
            Invalidate();
        }
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var rect = _mode == SelectionMode.Fixed ? GetFixedRectangle(_current) : Normalize(_start, _current);
        if (rect.Width <= 0 || rect.Height <= 0)
        {
            return;
        }

        using var borderPen = new Pen(Color.Red, 2f);
        using var fillBrush = new SolidBrush(Color.FromArgb(40, Color.LightSkyBlue));
        e.Graphics.FillRectangle(fillBrush, rect);
        e.Graphics.DrawRectangle(borderPen, rect);

        var info = _mode == SelectionMode.Fixed
            ? $"x: {rect.X + Left}, y: {rect.Y + Top}, w: {rect.Width}, h: {rect.Height}  (方向键可调尺寸，单击确认)"
            : $"x: {rect.X + Left}, y: {rect.Y + Top}, w: {rect.Width}, h: {rect.Height}  (拖拽完成，Esc 取消)";
        TextRenderer.DrawText(e.Graphics, info, Font, new Point(16, 16), Color.Yellow);
    }

    private Rectangle GetFixedRectangle(Point cursor)
    {
        return new Rectangle(cursor.X - (_fixedWidth / 2), cursor.Y - (_fixedHeight / 2), _fixedWidth, _fixedHeight);
    }

    private static Rectangle Normalize(Point p1, Point p2)
    {
        var x = Math.Min(p1.X, p2.X);
        var y = Math.Min(p1.Y, p2.Y);
        var w = Math.Abs(p1.X - p2.X) + 1;
        var h = Math.Abs(p1.Y - p2.Y) + 1;
        return new Rectangle(x, y, w, h);
    }
}
