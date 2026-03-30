namespace WinFormsApp1;

internal sealed class PixelGridControl : Control
{
    public const int ViewColumns = 71;
    public const int ViewRows = 25;
    public const int CellSize = 12;

    private FindTextCaptureState? _state;

    private int _viewX;
    private int _viewY;
    private bool _modifyMode;

    public event EventHandler<GridCellEventArgs>? CellMouseAction;

    public PixelGridControl()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
        BackColor = Color.White;
        Size = new Size(ViewColumns * CellSize, ViewRows * CellSize);
    }

    public void BindState(FindTextCaptureState? state)
    {
        _state = state;
        Invalidate();
    }

    public void SetViewport(int viewX, int viewY)
    {
        _viewX = viewX;
        _viewY = viewY;
        Invalidate();
    }

    public void SetModifyMode(bool modifyMode)
    {
        _modifyMode = modifyMode;
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        e.Graphics.Clear(Color.FromArgb(0xDD, 0xEE, 0xFF));
        if (_state is null || !_state.HasImage)
        {
            return;
        }

        for (var row = 0; row < ViewRows; row++)
        {
            for (var col = 0; col < ViewColumns; col++)
            {
                var x = _viewX + col;
                var y = _viewY + row;
                var rect = new Rectangle(col * CellSize, row * CellSize, CellSize - 1, CellSize - 1);
                if (!_state.IsWithin(x, y))
                {
                    using var emptyBrush = new SolidBrush(Color.FromArgb(0xDD, 0xEE, 0xFF));
                    e.Graphics.FillRectangle(emptyBrush, rect);
                    continue;
                }

                var index = _state.GetIndex(x, y);
                var color = !_state.VisibleMask[index]
                    ? Color.FromArgb(0xFF, 0xFF, 0xAA)
                    : _state.BinaryReady
                        ? (_state.BinaryMask[index] ? Color.Black : Color.White)
                        : _state.GetColor(index);

                using var fillBrush = new SolidBrush(color);
                e.Graphics.FillRectangle(fillBrush, rect);
                e.Graphics.DrawRectangle(Pens.LightGray, rect);
                if (_state.SelectedIndex == index)
                {
                    using var pen = new Pen(Color.Red, 2f);
                    e.Graphics.DrawRectangle(pen, rect);
                }
            }
        }
    }

    protected override void OnMouseDown(MouseEventArgs e)
    {
        base.OnMouseDown(e);
        var gridX = _viewX + (e.X / CellSize);
        var gridY = _viewY + (e.Y / CellSize);
        CellMouseAction?.Invoke(this, new GridCellEventArgs(gridX, gridY, e.Button));
    }
}

internal sealed class GridCellEventArgs(int x, int y, MouseButtons button) : EventArgs
{
    public int X { get; } = x;
    public int Y { get; } = y;
    public MouseButtons Button { get; } = button;
}
