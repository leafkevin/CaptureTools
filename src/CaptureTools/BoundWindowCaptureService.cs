namespace CaptureTools;

internal static class BoundWindowCaptureService
{
    public static IntPtr GetForegroundWindowHandle() => NativeMethods.GetForegroundWindow();

    public static Bitmap? Capture(IntPtr hwnd, int mode)
    {
        if (hwnd == IntPtr.Zero || !NativeMethods.GetWindowRect(hwnd, out var rect))
        {
            return null;
        }

        var width = rect.Right - rect.Left;
        var height = rect.Bottom - rect.Top;
        if (width <= 0 || height <= 0)
        {
            return null;
        }

        var hBitmap = NativeMethods.CreateDIBSection32(width, height, out var scan0);
        if (hBitmap == IntPtr.Zero || scan0 == IntPtr.Zero)
        {
            return null;
        }

        var memoryDc = NativeMethods.CreateCompatibleDC(IntPtr.Zero);
        var old = NativeMethods.SelectObject(memoryDc, hBitmap);
        try
        {
            if (mode < 2)
            {
                var dc = NativeMethods.GetDCEx(hwnd, IntPtr.Zero, 3);
                try
                {
                    NativeMethods.BitBlt(memoryDc, 0, 0, width, height, dc, 0, 0, NativeMethods.SRCCOPY | NativeMethods.CAPTUREBLT_FLAG);
                }
                finally
                {
                    NativeMethods.ReleaseDC(hwnd, dc);
                }
            }
            else
            {
                NativeMethods.UpdateWindow(hwnd);
                _ = NativeMethods.PrintWindow(hwnd, memoryDc, mode > 3 ? 3u : 0u);
            }

            return Image.FromHbitmap(hBitmap);
        }
        finally
        {
            NativeMethods.SelectObject(memoryDc, old);
            NativeMethods.DeleteDC(memoryDc);
            NativeMethods.DeleteObject(hBitmap);
        }
    }

    public static string Describe(IntPtr hwnd)
    {
        if (hwnd == IntPtr.Zero)
        {
            return "Î´°ó¶¨´°¿Ú";
        }

        return $"0x{hwnd.ToInt64():X}";
    }
}
