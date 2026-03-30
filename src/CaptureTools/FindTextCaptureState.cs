using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace CaptureTools;

internal sealed class FindTextCaptureState
{
    private Bitmap? _sourceBitmap;
    private bool[] _manualHiddenMask = [];
    private int _cropLeft;
    private int _cropRight;
    private int _cropUp;
    private int _cropDown;

    public int ScreenX { get; private set; }
    public int ScreenY { get; private set; }
    public int Width { get; private set; }
    public int Height { get; private set; }
    public int[] Colors { get; private set; } = [];
    public bool[] VisibleMask { get; private set; } = [];
    public bool[] BinaryMask { get; private set; } = [];
    public bool BinaryReady { get; private set; }
    public int CropLeft => _cropLeft;
    public int CropRight => _cropRight;
    public int CropUp => _cropUp;
    public int CropDown => _cropDown;
    public int SelectedIndex { get; private set; } = -1;
    public string CurrentModePrefix { get; private set; } = string.Empty;
    public string CurrentColorList { get; private set; } = string.Empty;
    public int CurrentThreshold { get; private set; }
    public int CurrentGrayDiff { get; private set; } = 50;
    public double CurrentSimilarity2 { get; private set; } = 0.9;
    public List<MultiColorPoint> MultiColorPoints { get; } = [];

    public Bitmap SourceBitmap => _sourceBitmap ?? new Bitmap(1, 1);

    public void Load(Bitmap bitmap, int screenX, int screenY)
    {
        _sourceBitmap?.Dispose();
        _sourceBitmap = new Bitmap(bitmap);
        ScreenX = screenX;
        ScreenY = screenY;
        Width = _sourceBitmap.Width;
        Height = _sourceBitmap.Height;
        Colors = new int[Width * Height];
        VisibleMask = new bool[Width * Height];
        BinaryMask = new bool[Width * Height];
        _manualHiddenMask = new bool[Width * Height];
        _cropLeft = _cropRight = _cropUp = _cropDown = 0;
        SelectedIndex = -1;
        BinaryReady = false;
        CurrentModePrefix = string.Empty;
        CurrentColorList = string.Empty;
        MultiColorPoints.Clear();

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var index = GetIndex(x, y);
                var color = _sourceBitmap.GetPixel(x, y);
                Colors[index] = color.ToArgb() & 0xFFFFFF;
                VisibleMask[index] = true;
                BinaryMask[index] = false;
            }
        }
    }

    public void Reset()
    {
        if (_sourceBitmap is null)
        {
            return;
        }

        Load(_sourceBitmap, ScreenX, ScreenY);
    }

    public bool HasImage => _sourceBitmap is not null;

    public int GetIndex(int x, int y) => (y * Width) + x;

    public bool IsWithin(int x, int y) => x >= 0 && y >= 0 && x < Width && y < Height;

    public int GetGray(int index)
    {
        var color = Colors[index];
        var r = (color >> 16) & 0xFF;
        var g = (color >> 8) & 0xFF;
        var b = color & 0xFF;
        return ((r * 38) + (g * 75) + (b * 15)) >> 7;
    }

    public Color GetColor(int index) => Color.FromArgb(255, (Colors[index] >> 16) & 0xFF, (Colors[index] >> 8) & 0xFF, Colors[index] & 0xFF);

    public void SetSelected(int x, int y)
    {
        SelectedIndex = IsWithin(x, y) ? GetIndex(x, y) : -1;
    }

    public void ToggleBinary(int x, int y)
    {
        if (!IsWithin(x, y) || !BinaryReady)
        {
            return;
        }

        var index = GetIndex(x, y);
        if (!VisibleMask[index])
        {
            return;
        }

        BinaryMask[index] = !BinaryMask[index];
    }

    public void ToggleVisibility(int x, int y)
    {
        if (!IsWithin(x, y))
        {
            return;
        }

        var index = GetIndex(x, y);
        _manualHiddenMask[index] = !_manualHiddenMask[index];
        RebuildVisibility();
    }

    public void CutLeft() => ChangeCrop(ref _cropLeft, 1, Width - _cropRight - 1);
    public void CutRight() => ChangeCrop(ref _cropRight, 1, Width - _cropLeft - 1);
    public void CutUp() => ChangeCrop(ref _cropUp, 1, Height - _cropDown - 1);
    public void CutDown() => ChangeCrop(ref _cropDown, 1, Height - _cropUp - 1);
    public void RepLeft() => ChangeCrop(ref _cropLeft, -1, Width);
    public void RepRight() => ChangeCrop(ref _cropRight, -1, Width);
    public void RepUp() => ChangeCrop(ref _cropUp, -1, Height);
    public void RepDown() => ChangeCrop(ref _cropDown, -1, Height);

    private void ChangeCrop(ref int target, int delta, int maxVisible)
    {
        if (!HasImage)
        {
            return;
        }

        target = Math.Max(0, target + delta);
        if (maxVisible <= 0)
        {
            target = Math.Max(0, target - delta);
        }

        RebuildVisibility();
    }

    private void RebuildVisibility()
    {
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var index = GetIndex(x, y);
                var inCrop = x >= _cropLeft && x < Width - _cropRight && y >= _cropUp && y < Height - _cropDown;
                VisibleMask[index] = inCrop && !_manualHiddenMask[index];
            }
        }
    }

    public int ApplyGrayThreshold(int? threshold)
    {
        var value = threshold ?? AutoThreshold();
        CurrentThreshold = value;
        CurrentModePrefix = $"*{value}";
        BinaryReady = true;
        CurrentColorList = string.Empty;
        MultiColorPoints.Clear();

        for (var i = 0; i < Colors.Length; i++)
        {
            BinaryMask[i] = GetGray(i) <= value;
        }

        return value;
    }

    public void ApplyGrayDiff(int grayDiff)
    {
        if (Width < 3 || Height < 3)
        {
            return;
        }

        if (CropLeft == 0) CutLeft();
        if (CropRight == 0) CutRight();
        if (CropUp == 0) CutUp();
        if (CropDown == 0) CutDown();

        CurrentGrayDiff = grayDiff;
        CurrentModePrefix = $"**{grayDiff}";
        BinaryReady = true;
        CurrentColorList = string.Empty;
        MultiColorPoints.Clear();

        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var index = GetIndex(x, y);
                if (x == 0 || y == 0 || x == Width - 1 || y == Height - 1)
                {
                    BinaryMask[index] = false;
                    continue;
                }

                var n = GetGray(index) + grayDiff;
                BinaryMask[index] = GetGray(GetIndex(x - 1, y)) > n
                    || GetGray(GetIndex(x + 1, y)) > n
                    || GetGray(GetIndex(x, y - 1)) > n
                    || GetGray(GetIndex(x, y + 1)) > n
                    || GetGray(GetIndex(x - 1, y - 1)) > n
                    || GetGray(GetIndex(x + 1, y - 1)) > n
                    || GetGray(GetIndex(x - 1, y + 1)) > n
                    || GetGray(GetIndex(x + 1, y + 1)) > n;
            }
        }
    }

    public void ApplyColorList(string colorList)
    {
        var rules = ParseColorRules(colorList);
        if (rules.Count == 0)
        {
            return;
        }

        CurrentColorList = string.Join('/', rules.Select(rule => rule.Raw));
        CurrentModePrefix = CurrentColorList;
        BinaryReady = true;
        MultiColorPoints.Clear();

        var inclusiveMode = !CurrentColorList.StartsWith("-", StringComparison.Ordinal);
        for (var i = 0; i < Colors.Length; i++)
        {
            var matched = rules.Any(rule => rule.IsMatch(Colors[i]));
            BinaryMask[i] = inclusiveMode ? matched : !matched;
        }
    }

    public void ApplyColorPosition(double similarity)
    {
        if (SelectedIndex < 0)
        {
            return;
        }

        CurrentSimilarity2 = similarity;
        var threshold = SimilarityToThreshold(similarity);
        BinaryReady = true;
        MultiColorPoints.Clear();

        var visibleOrdinal = GetVisibleOrdinal(SelectedIndex);
        if (visibleOrdinal <= 0)
        {
            return;
        }

        CurrentModePrefix = $"#{visibleOrdinal}-{similarity.ToString("0.##", CultureInfo.InvariantCulture)}";
        var baseColor = Colors[SelectedIndex];
        var rr = (baseColor >> 16) & 0xFF;
        var gg = (baseColor >> 8) & 0xFF;
        var bb = baseColor & 0xFF;

        for (var i = 0; i < Colors.Length; i++)
        {
            var c = Colors[i];
            var r = ((c >> 16) & 0xFF) - rr;
            var g = ((c >> 8) & 0xFF) - gg;
            var b = (c & 0xFF) - bb;
            BinaryMask[i] = (1024 + (r + rr + rr)) * r * r + (2048 * g * g) + (1534 - (r + rr + rr)) * b * b <= threshold;
        }
    }

    public void AutoCrop()
    {
        if (!BinaryReady)
        {
            return;
        }

        var rows = BuildBinaryRows();
        if (rows.Count == 0)
        {
            return;
        }

        var background = GetBackgroundBit(rows);
        while (rows.Count > 0)
        {
            if (rows[0].All(ch => ch == background))
            {
                CutUp();
            }
            else if (rows[^1].All(ch => ch == background))
            {
                CutDown();
            }
            else if (rows.All(row => row.Length > 0 && row[0] == background))
            {
                CutLeft();
            }
            else if (rows.All(row => row.Length > 0 && row[^1] == background))
            {
                CutRight();
            }
            else
            {
                break;
            }

            rows = BuildBinaryRows();
            if (rows.Count == 0)
            {
                break;
            }
        }
    }

    public void AddMultiColorPoint(int x, int y)
    {
        if (!IsWithin(x, y))
        {
            return;
        }

        var index = GetIndex(x, y);
        if (!VisibleMask[index])
        {
            return;
        }

        var point = new MultiColorPoint(x, y, Colors[index]);
        if (!MultiColorPoints.Any(existing => existing.X == point.X && existing.Y == point.Y && existing.Color == point.Color))
        {
            MultiColorPoints.Add(point);
        }
    }

    public void UndoMultiColorPoint()
    {
        if (MultiColorPoints.Count > 0)
        {
            MultiColorPoints.RemoveAt(MultiColorPoints.Count - 1);
        }
    }

    public string? BuildTemplateString(string comment)
    {
        if (!BinaryReady)
        {
            return null;
        }

        var (bitString, width) = BuildBitString();
        if (string.IsNullOrEmpty(bitString) || width <= 0)
        {
            return null;
        }

        return $"|<{comment}>{CurrentModePrefix}${width}.{FindTextCore.BitToBase64(bitString)}";
    }

    public IReadOnlyList<string> SplitTemplateStrings(string comment)
    {
        if (!BinaryReady)
        {
            return [];
        }

        var rows = BuildBinaryRows();
        if (rows.Count == 0)
        {
            return [];
        }

        var background = GetBackgroundBit(rows);
        var width = rows[0].Length;
        var segments = new List<(int Start, int End)>();
        var start = -1;
        for (var x = 0; x < width; x++)
        {
            var isBackgroundColumn = rows.All(row => row[x] == background);
            if (!isBackgroundColumn && start < 0)
            {
                start = x;
            }
            else if (isBackgroundColumn && start >= 0)
            {
                segments.Add((start, x - 1));
                start = -1;
            }
        }

        if (start >= 0)
        {
            segments.Add((start, width - 1));
        }

        var result = new List<string>();
        var comments = comment.ToCharArray();
        for (var i = 0; i < segments.Count; i++)
        {
            var segment = segments[i];
            var sb = new StringBuilder();
            var segmentWidth = segment.End - segment.Start + 1;
            foreach (var row in rows)
            {
                sb.Append(row.Substring(segment.Start, segmentWidth));
            }

            var pieceComment = i < comments.Length ? comments[i].ToString() : comment;
            result.Add($"|<{pieceComment}>{CurrentModePrefix}${segmentWidth}.{FindTextCore.BitToBase64(sb.ToString())}");
        }

        return result;
    }

    public string? BuildMultiColorTemplate(string comment, double similarity3, bool findShape)
    {
        if (MultiColorPoints.Count == 0)
        {
            return null;
        }

        var first = MultiColorPoints[0];
        var n = SimilarityToThreshold(similarity3);
        var rr = (first.Color >> 16) & 0xFF;
        var gg = (first.Color >> 8) & 0xFF;
        var bb = first.Color & 0xFF;
        var prefix = $"##{similarity3.ToString("0.##", CultureInfo.InvariantCulture)}";
        var parts = new List<string>();
        foreach (var point in MultiColorPoints)
        {
            var rx = point.X - first.X;
            var ry = point.Y - first.Y;
            if (findShape)
            {
                var r = ((point.Color >> 16) & 0xFF) - rr;
                var g = ((point.Color >> 8) & 0xFF) - gg;
                var b = (point.Color & 0xFF) - bb;
                var match = (1024 + (r + rr + rr)) * r * r + (2048 * g * g) + (1534 - (r + rr + rr)) * b * b <= n;
                parts.Add($"{rx}/{ry}/{(match ? 1 : 0)}");
            }
            else
            {
                parts.Add($"{rx}/{ry}/{point.Color:X6}");
            }
        }

        return $"|<{comment}>{prefix}${string.Join(',', parts)}";
    }

    public string GetAsciiPreview()
    {
        if (!BinaryReady)
        {
            return string.Empty;
        }

        var rows = BuildBinaryRows();
        return string.Join(Environment.NewLine, rows.Select(row => row.Replace('0', '_').Replace('1', '0')));
    }

    public Rectangle GetVisibleBounds()
    {
        var minX = int.MaxValue;
        var minY = int.MaxValue;
        var maxX = int.MinValue;
        var maxY = int.MinValue;
        for (var y = 0; y < Height; y++)
        {
            for (var x = 0; x < Width; x++)
            {
                var index = GetIndex(x, y);
                if (!VisibleMask[index])
                {
                    continue;
                }

                minX = Math.Min(minX, x);
                minY = Math.Min(minY, y);
                maxX = Math.Max(maxX, x);
                maxY = Math.Max(maxY, y);
            }
        }

        if (minX == int.MaxValue)
        {
            return Rectangle.Empty;
        }

        return Rectangle.FromLTRB(minX, minY, maxX + 1, maxY + 1);
    }

    private List<string> BuildBinaryRows()
    {
        var bounds = GetVisibleBounds();
        if (bounds == Rectangle.Empty)
        {
            return [];
        }

        var rows = new List<string>();
        var background = GetBackgroundBit();
        for (var y = bounds.Top; y < bounds.Bottom; y++)
        {
            var sb = new StringBuilder(bounds.Width);
            for (var x = bounds.Left; x < bounds.Right; x++)
            {
                var index = GetIndex(x, y);
                var bit = VisibleMask[index] ? (BinaryMask[index] ? '1' : '0') : background;
                sb.Append(bit);
            }

            rows.Add(sb.ToString());
        }

        return rows;
    }

    private (string BitString, int Width) BuildBitString()
    {
        var rows = BuildBinaryRows();
        if (rows.Count == 0)
        {
            return (string.Empty, 0);
        }

        return (string.Concat(rows), rows[0].Length);
    }

    private char GetBackgroundBit()
    {
        var ones = 0;
        var zeros = 0;
        for (var i = 0; i < VisibleMask.Length; i++)
        {
            if (!VisibleMask[i])
            {
                continue;
            }

            if (BinaryMask[i])
            {
                ones++;
            }
            else
            {
                zeros++;
            }
        }

        return zeros >= ones ? '0' : '1';
    }

    private static char GetBackgroundBit(IEnumerable<string> rows)
    {
        var ones = rows.Sum(row => row.Count(ch => ch == '1'));
        var zeros = rows.Sum(row => row.Count(ch => ch == '0'));
        return zeros >= ones ? '0' : '1';
    }

    private int AutoThreshold()
    {
        var histogram = new int[256];
        for (var i = 0; i < Colors.Length; i++)
        {
            if (VisibleMask[i])
            {
                histogram[GetGray(i)]++;
            }
        }

        long ip0 = 0;
        long is0 = 0;
        for (var i = 0; i < histogram.Length; i++)
        {
            ip0 += (long)i * histogram[i];
            is0 += histogram[i];
        }

        var threshold = is0 == 0 ? 0 : (int)(ip0 / is0);
        for (var i = 0; i < 20; i++)
        {
            var last = threshold;
            long ip1 = 0;
            long is1 = 0;
            for (var k = 0; k <= last; k++)
            {
                ip1 += (long)k * histogram[k];
                is1 += histogram[k];
            }

            var ip2 = ip0 - ip1;
            var is2 = is0 - is1;
            if (is1 != 0 && is2 != 0)
            {
                threshold = (int)((ip1 / is1 + ip2 / is2) / 2);
            }

            if (threshold == last)
            {
                break;
            }
        }

        return threshold;
    }

    private int GetVisibleOrdinal(int selectedIndex)
    {
        var ordinal = 0;
        var bounds = GetVisibleBounds();
        if (bounds == Rectangle.Empty)
        {
            return 0;
        }

        for (var y = bounds.Top; y < bounds.Bottom; y++)
        {
            for (var x = bounds.Left; x < bounds.Right; x++)
            {
                var index = GetIndex(x, y);
                if (!VisibleMask[index])
                {
                    continue;
                }

                ordinal++;
                if (index == selectedIndex)
                {
                    return ordinal;
                }
            }
        }

        return 0;
    }

    private static List<ColorRule> ParseColorRules(string colorList)
    {
        var tokens = Regex.Replace(colorList ?? string.Empty, "(?i)\\s|0x", string.Empty)
            .Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var rules = new List<ColorRule>();
        foreach (var token in tokens)
        {
            var parts = (token.Trim('-') + "-").Split('-', StringSplitOptions.None);
            if (!int.TryParse(parts[0], NumberStyles.HexNumber, CultureInfo.InvariantCulture, out _)
                && !Regex.IsMatch(parts[0], "^(Black|White|Red|Green|Blue|Yellow|Silver|Gray|Teal|Navy|Aqua|Olive|Lime|Fuchsia|Purple|Maroon)$", RegexOptions.IgnoreCase))
            {
                continue;
            }

            rules.Add(new ColorRule(token, parts[0], parts.ElementAtOrDefault(1) ?? string.Empty));
        }

        return rules;
    }

    private static int SimilarityToThreshold(double value)
    {
        return value <= 0 || value > 1 ? 0 : (int)Math.Floor(4606d * 255d * 255d * (1 - value) * (1 - value));
    }

    public void Dispose()
    {
        _sourceBitmap?.Dispose();
    }

    internal readonly record struct MultiColorPoint(int X, int Y, int Color);

    private sealed class ColorRule(string raw, string colorValue, string toleranceValue)
    {
        private readonly uint _color = ToRgb(colorValue);
        private readonly string _tolerance = toleranceValue;

        public string Raw { get; } = raw;

        public bool IsMatch(int rgb)
        {
            var rr = (rgb >> 16) & 0xFF;
            var gg = (rgb >> 8) & 0xFF;
            var bb = rgb & 0xFF;
            var r = (int)((_color >> 16) & 0xFF) - rr;
            var g = (int)((_color >> 8) & 0xFF) - gg;
            var b = (int)(_color & 0xFF) - bb;
            if (_tolerance.Contains('.', StringComparison.Ordinal))
            {
                var similarity = double.TryParse(_tolerance, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : 0d;
                var n = SimilarityToThreshold(similarity);
                return (1024 + (r + rr + rr)) * r * r + (2048 * g * g) + (1534 - (r + rr + rr)) * b * b <= n;
            }

            var diff = uint.TryParse(_tolerance, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var parsed) ? parsed : 0u;
            var dR = (int)((diff >> 16) & 0xFF);
            var dG = (int)((diff >> 8) & 0xFF);
            var dB = (int)(diff & 0xFF);
            return Math.Abs(r) <= dR && Math.Abs(g) <= dG && Math.Abs(b) <= dB;
        }
    }

    private static uint ToRgb(string color)
    {
        var namedColors = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["Black"] = "000000",
            ["White"] = "FFFFFF",
            ["Red"] = "FF0000",
            ["Green"] = "008000",
            ["Blue"] = "0000FF",
            ["Yellow"] = "FFFF00",
            ["Silver"] = "C0C0C0",
            ["Gray"] = "808080",
            ["Teal"] = "008080",
            ["Navy"] = "000080",
            ["Aqua"] = "00FFFF",
            ["Olive"] = "808000",
            ["Lime"] = "00FF00",
            ["Fuchsia"] = "FF00FF",
            ["Purple"] = "800080",
            ["Maroon"] = "800000"
        };

        var value = namedColors.TryGetValue(color, out var named) ? named : color;
        value = value.Replace("0x", string.Empty, StringComparison.OrdinalIgnoreCase);
        return uint.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var rgb) ? rgb : 0u;
    }
}
