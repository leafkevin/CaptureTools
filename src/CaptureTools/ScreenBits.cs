namespace CaptureTools;

/// <summary>
/// 一次截图缓存对应的底层位图信息。
/// 对应 AHK 原版里的 <c>bits</c> 对象。
/// </summary>
public sealed class ScreenBits : IDisposable
{
    /// <summary>
    /// 位图像素首地址。
    /// </summary>
    public nint Scan0 { get; set; }

    /// <summary>
    /// GDI 位图句柄。
    /// </summary>
    public nint HBM { get; set; }

    /// <summary>
    /// 每一行像素占用的字节数。
    /// 32bpp 时通常等于 <c>宽度 * 4</c>。
    /// </summary>
    public int Stride { get; set; }

    /// <summary>
    /// 虚拟屏幕左上角 X。
    /// </summary>
    public int Zx { get; set; }

    /// <summary>
    /// 虚拟屏幕左上角 Y。
    /// </summary>
    public int Zy { get; set; }

    /// <summary>
    /// 当前截图宽度。
    /// </summary>
    public int Zw { get; set; }

    /// <summary>
    /// 当前截图高度。
    /// </summary>
    public int Zh { get; set; }

    /// <summary>
    /// 已分配缓存的历史宽度。
    /// 用于判断是否需要重新分配位图缓冲区。
    /// </summary>
    public int OldZw { get; set; }

    /// <summary>
    /// 已分配缓存的历史高度。
    /// </summary>
    public int OldZh { get; set; }

    /// <summary>
    /// 预留字段，对应原版绑定窗口句柄。
    /// 当前核心版未实际使用。
    /// </summary>
    public nint BindWindow { get; set; }

    private bool _disposed;

    public void Dispose()
    {
        if (!_disposed)
        {
            if (HBM != 0)
                NativeMethods.DeleteObject(HBM);
            _disposed = true;
        }

        GC.SuppressFinalize(this);
    }

    ~ScreenBits() => Dispose();
}