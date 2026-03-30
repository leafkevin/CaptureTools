namespace WinFormsApp1;

internal sealed class RangeFlashForm : Form
{
    private readonly Color _borderColor;
    private readonly int _thickness;
    private readonly global::System.Windows.Forms.Timer _timer;

    private RangeFlashForm(Rectangle rect, Color borderColor, int thickness, int durationMs)
    {
        Bounds = rect;
        _borderColor = borderColor;
        _thickness = thickness;
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.Manual;
        ShowInTaskbar = false;
        TopMost = true;
        DoubleBuffered = true;
        BackColor = Color.Magenta;
        TransparencyKey = Color.Magenta;
        _timer = new global::System.Windows.Forms.Timer { Interval = durationMs };
        _timer.Tick += (_, _) =>
        {
            _timer.Stop();
            Close();
        };
    }

    public static void ShowFlash(Rectangle rect, Color color, int thickness = 3, int durationMs = 450)
    {
        if (rect.Width <= 0 || rect.Height <= 0)
        {
            return;
        }

        var form = new RangeFlashForm(rect, color, thickness, durationMs);
        form.Show();
        form._timer.Start();
    }

    protected override bool ShowWithoutActivation => true;

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var pen = new Pen(_borderColor, _thickness);
        e.Graphics.DrawRectangle(pen, new Rectangle(0, 0, Width - 1, Height - 1));
    }
}
