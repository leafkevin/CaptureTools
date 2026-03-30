using System.Drawing.Imaging;
using System.Text.RegularExpressions;

namespace WinFormsApp1;

internal static class CaptureUtilities
{
    public static Bitmap CaptureScreen(Rectangle region)
    {
        var bitmap = new Bitmap(region.Width, region.Height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen(region.Location, Point.Empty, region.Size, CopyPixelOperation.SourceCopy);
        return bitmap;
    }

    public static void SaveBitmap(Bitmap bitmap, string filePath)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(filePath)!);
        bitmap.Save(filePath, ImageFormat.Bmp);
    }

    public static Bitmap Crop(Bitmap source, Rectangle rect)
    {
        var safeRect = Rectangle.Intersect(new Rectangle(Point.Empty, source.Size), rect);
        var bitmap = new Bitmap(safeRect.Width, safeRect.Height, PixelFormat.Format32bppArgb);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.DrawImage(source, new Rectangle(Point.Empty, safeRect.Size), safeRect, GraphicsUnit.Pixel);
        return bitmap;
    }

    public static string? ExtractFirstTemplate(string text)
    {
        var match = Regex.Match(text ?? string.Empty, "\\|?<[^>\\n]*>[^$\\n]+\\$[^`\"'\\r\\n]+", RegexOptions.Singleline);
        return match.Success ? match.Value.TrimStart('|') : null;
    }

    public static string BuildAsciiEditorText(string template)
    {
        return new FindTextCore().ASCII(template);
    }

    public static string NormalizeLineEndings(string text)
    {
        return Regex.Replace(text ?? string.Empty, "\\r?\\n", Environment.NewLine);
    }
}
