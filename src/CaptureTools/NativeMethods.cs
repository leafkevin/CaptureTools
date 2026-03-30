using System.Runtime.InteropServices;

namespace CaptureTools;

/// <summary>
/// <c>FindTextCore</c> 使用到的 Win32 / GDI PInvoke 声明。
/// 主要用于：截图、位图创建、DPI 感知设置等。
/// </summary>
internal static class NativeMethods
{
    // ===== GDI32 =====

    [DllImport("gdi32.dll")]
    public static extern nint CreateCompatibleDC(nint hdc);

    [DllImport("gdi32.dll")]
    public static extern nint SelectObject(nint hdc, nint h);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteDC(nint hdc);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DeleteObject(nint ho);

    [DllImport("gdi32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool BitBlt(nint hdc, int x, int y, int cx, int cy,
        nint hdcSrc, int x1, int y1, uint rop);

    [DllImport("gdi32.dll")]
    public static extern nint CreateDIBSection(nint hdc, ref BITMAPINFO pbmi,
        uint usage, out nint ppvBits, nint hSection, uint offset);

    [DllImport("gdi32.dll")]
    public static extern int GetObject(nint h, int c, nint pv);

    // ===== USER32 =====

    [DllImport("user32.dll")]
    public static extern nint GetDC(nint hWnd);

    [DllImport("user32.dll")]
    public static extern nint GetDCEx(nint hWnd, nint hrgnClip, int flags);

    [DllImport("user32.dll")]
    public static extern int ReleaseDC(nint hWnd, nint hDC);

    [DllImport("user32.dll")]
    public static extern nint GetDesktopWindow();

    [DllImport("user32.dll")]
    public static extern nint GetForegroundWindow();

    [DllImport("user32.dll")]
    public static extern nint GetWindowDC(nint hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetWindowRect(nint hWnd, out RECT lpRect);

    [DllImport("user32.dll")]
    public static extern int GetSystemMetrics(int nIndex);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool PrintWindow(nint hWnd, nint hdcBlt, uint nFlags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UpdateWindow(nint hWnd);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorInfo(ref CURSORINFO pci);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetIconInfo(nint hIcon, out ICONINFO piconinfo);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool DrawIconEx(nint hdc, int xLeft, int yTop, nint hIcon,
        int cxWidth, int cyWidth, uint istepIfAniCur, nint hbrFlickerFreeDraw, uint diFlags);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool ClientToScreen(nint hWnd, ref POINT lpPoint);

    [DllImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool GetCursorPos(out POINT lpPoint);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool RegisterHotKey(nint hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    public static extern bool UnregisterHotKey(nint hWnd, int id);

    // ===== DWMAPI =====

    [DllImport("dwmapi.dll")]
    public static extern int DwmIsCompositionEnabled(out int pfEnabled);

    // ===== KERNEL32 =====

    [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
    public static extern void CopyMemory(nint dest, nint src, nint size);

    [DllImport("user32.dll")]
    public static extern nint SetThreadDpiAwarenessContext(nint dpiContext);

    // ===== 常量 =====

    public const uint SRCCOPY = 0xCC0020;
    public const uint CAPTUREBLT_FLAG = 0x40000000;
    public const uint MERGECOPY = 0xC000CA;
    public const int SM_XVIRTUALSCREEN = 76;
    public const int SM_YVIRTUALSCREEN = 77;
    public const int SM_CXVIRTUALSCREEN = 78;
    public const int SM_CYVIRTUALSCREEN = 79;
    public const int WM_HOTKEY = 0x0312;

    /// <summary>
    /// 每显示器 DPI 感知 V2。
    /// 用于减少高分屏下截图坐标和实际像素不一致的问题。
    /// </summary>
    public static readonly nint DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new(-4);

    // ===== 结构体 =====

    [StructLayout(LayoutKind.Sequential)]
    public struct RECT
    {
        public int Left, Top, Right, Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFOHEADER
    {
        public uint biSize;
        public int biWidth;
        public int biHeight;
        public ushort biPlanes;
        public ushort biBitCount;
        public uint biCompression;
        public uint biSizeImage;
        public int biXPelsPerMeter;
        public int biYPelsPerMeter;
        public uint biClrUsed;
        public uint biClrImportant;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct BITMAPINFO
    {
        public BITMAPINFOHEADER bmiHeader;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct CURSORINFO
    {
        public uint cbSize;
        public uint flags;
        public nint hCursor;
        public POINT ptScreenPos;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct ICONINFO
    {
        [MarshalAs(UnmanagedType.Bool)]
        public bool fIcon;
        public int xHotspot;
        public int yHotspot;
        public nint hbmMask;
        public nint hbmColor;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct POINT
    {
        public int X, Y;
    }

    // ===== 辅助方法 =====

    public static nint CreateDIBSection32(int w, int h, out nint ppvBits)
    {
        // 创建 32 位、top-down 的位图。
        // 这样可以直接按从上到下的顺序访问像素，不需要额外翻转。
        var bi = new BITMAPINFO();
        bi.bmiHeader.biSize = 40;
        bi.bmiHeader.biWidth = w;
        bi.bmiHeader.biHeight = -h;
        bi.bmiHeader.biPlanes = 1;
        bi.bmiHeader.biBitCount = 32;
        return CreateDIBSection(0, ref bi, 0, out ppvBits, 0, 0);
    }

    public static void GetBitmapWH(nint hBM, out int w, out int h)
    {
        // 读取 GDI 位图对象的宽高。
        int size = nint.Size == 8 ? 32 : 24;
        nint bm = Marshal.AllocHGlobal(size);
        try
        {
            GetObject(hBM, size, bm);
            w = Marshal.ReadInt32(bm, 4);
            h = Math.Abs(Marshal.ReadInt32(bm, 8));
        }
        finally
        {
            Marshal.FreeHGlobal(bm);
        }
    }

    public static void CopyHBM(nint hBM1, int x1, int y1,
        nint hBM2, int x2, int y2, int w, int h, bool clear = false)
    {
        // 在两个 HBITMAP 之间做 BitBlt 拷贝。
        if (w < 1 || h < 1 || hBM1 == 0 || hBM2 == 0) return;

        var mDC1 = CreateCompatibleDC(0);
        var oBM1 = SelectObject(mDC1, hBM1);
        var mDC2 = CreateCompatibleDC(0);
        var oBM2 = SelectObject(mDC2, hBM2);

        BitBlt(mDC1, x1, y1, w, h, mDC2, x2, y2, SRCCOPY);

        if (clear)
            BitBlt(mDC1, x1, y1, w, h, mDC1, x1, y1, MERGECOPY);

        SelectObject(mDC1, oBM1);
        DeleteDC(mDC1);
        SelectObject(mDC2, oBM2);
        DeleteDC(mDC2);
    }
}