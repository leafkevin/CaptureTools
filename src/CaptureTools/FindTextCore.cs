using System.Buffers.Binary;
using System.Drawing.Imaging;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace WinFormsApp1;

/// <summary>
/// `FindText.ahk` 核心算法的 .NET 版本。
/// 主要职责：
/// 1. 截图并缓存屏幕像素。
/// 2. 解析 AHK 的文本/图色模式字符串。
/// 3. 在截图中执行找字、找图、找色、多点颜色匹配。
/// 4. 提供 OCR、排序、颜色读取等辅助能力。
/// </summary>
public sealed class FindTextCore : IDisposable
{
    // AHK 原版在传入 0,0,0,0 时，会把搜索范围扩展成一个非常大的矩形，
    // 再在截图函数里裁剪到虚拟屏幕范围。这里保留同样做法，兼容原逻辑。
    private const int FullScreenRange = 150000;

    // 原版 FindText 使用的自定义“64进制字符表”，不是标准 Base64。
    // 因此 bit <-> string 编解码也必须保持一致，否则字库字符串无法互通。
    private static readonly string CustomBase64Chars = "0123456789+/ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";

    // 最近一次截图缓冲。
    private readonly ScreenBits _bits = new();

    // 解析后的模式缓存，避免同一个 Text 字符串重复做 PicInfo 解析。
    private readonly Dictionary<string, PicInfoData> _picInfoCache = new(StringComparer.Ordinal);

    // AHK 原版支持用颜色名代替 RRGGBB，这里保留这套映射。
    private readonly Dictionary<string, string> _namedColors = new(StringComparer.OrdinalIgnoreCase)
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

    private readonly uint _captureBlt;

    /// <summary>
    /// 最近一次搜索结果，等价于 AHK 中的 `FindText().ok`。
    /// </summary>
    public IReadOnlyList<FindTextResult> LastResults { get; private set; } = [];

    public FindTextCore()
    {
        try
        {
            // 打开较新的 DPI 感知模式，减少高分屏下截图坐标与实际像素不一致的问题。
            _ = NativeMethods.SetThreadDpiAwarenessContext(NativeMethods.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);

        }
        catch
        {
        }

        try
        {
            // DWM 开启时某些窗口内容抓取行为与旧系统不同，
            // 这里沿用原版 CAPTUREBLT 的兼容逻辑。
            NativeMethods.DwmIsCompositionEnabled(out var enabled);
            _captureBlt = enabled != 0 ? 0u : NativeMethods.CAPTUREBLT_FLAG;
        }
        catch
        {
            _captureBlt = 0u;
        }
    }

    public void Dispose()
    {
        _bits.Dispose();
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// 仅做截图并缓存，不执行搜索。
    /// 后续 `screenShot=false` 的搜索会直接复用这份截图。
    /// </summary>
    public void ScreenShot(int x1 = 0, int y1 = 0, int x2 = 0, int y2 = 0)
    {
        CreateSearchRect(x1, y1, x2, y2, out var x, out var y, out var w, out var h);
        _ = GetBitsFromScreen(ref x, ref y, ref w, ref h, true, out _, out _, out _, out _);
    }

    /// <summary>
    /// 主搜索入口。
    /// `text` 参数直接兼容 AHK FindText 的模式字符串，可以包含多个 `|...` 模式。
    /// </summary>
    public List<FindTextResult> FindText(
        string text,
        int x1 = 0,
        int y1 = 0,
        int x2 = 0,
        int y2 = 0,
        double err1 = 0,
        double err0 = 0,
        bool screenShot = true,
        bool findAll = true,
        IReadOnlyList<string>? joinTexts = null,
        int offsetX = 20,
        int offsetY = 10,
        int dir = 0,
        double zoomW = 1,
        double zoomH = 1)
    {
        // 1. 计算搜索矩形并获取截图。
        CreateSearchRect(x1, y1, x2, y2, out var x, out var y, out var w, out var h);
        var bits = GetBitsFromScreen(ref x, ref y, ref w, ref h, screenShot, out var zx, out var zy, out _, out _);
        x -= zx;
        y -= zy;

        // 2. 把 Text 字符串拆成一个或多个模式对象。
        var infos = ParsePicInfos(text, joinTexts, out var maxPattern, out var infoByComment);

        if (w < 1 || h < 1 || infos.Count == 0 || bits.Scan0 == 0)
        {
            LastResults = [];
            return [];
        }

        // 4. 准备搜索上下文。
        //    `S1/S0`：记录前景/背景采样点偏移。
        //    `AllPos`：记录所有命中位置。
        //    `Errors`：在 dir=0 时用于按最小误差排序。
        var context = CreateSearchContext(bits, x, y, w, h, zx, zy, err1, err0, zoomW, zoomH, maxPattern, findAll, joinTexts is { Count: > 0 });

        var results = new List<FindTextResult>();
        var currentErr1 = err1;
        var currentErr0 = err0;

        // 5. 与原版保持一致：第一次严格匹配，必要时自动退一步给 5% 容错再试一次。
        for (var attempt = 0; attempt < 2; attempt++)
        {
            if (currentErr1 == 0 && currentErr0 == 0 && (infos.Count > 1 || attempt > 0))
            {
                currentErr1 = 0.05;
                currentErr0 = 0.05;
                context.Err1 = currentErr1;
                context.Err0 = currentErr0;
            }

            if (RunSearchPass(results, context, infos, infoByComment, joinTexts, findAll, offsetX, offsetY, dir))
            {
                LastResults = results;
                return results;
            }

            if (currentErr1 != 0 || currentErr0 != 0 || results.Count > 0 || infos[0].SetErr || infos[0].Mode == 5)
            {
                break;
            }
        }

        LastResults = results;
        return results;
    }

    /// <summary>
    /// 解析所有模式字符串，并顺便收集组合搜索所需的分组信息。
    /// </summary>
    private List<PicInfoData> ParsePicInfos(string text, IReadOnlyList<string>? joinTexts, out int maxPattern, out Dictionary<string, List<PicInfoData>> infoByComment)
    {
        var infos = new List<PicInfoData>();
        maxPattern = 0;
        infoByComment = new Dictionary<string, List<PicInfoData>>(StringComparer.Ordinal);

        foreach (var raw in text.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var info = PicInfo(raw);
            if (info is null)
            {
                continue;
            }

            infos.Add(info);
            maxPattern = Math.Max(maxPattern, info.Mode == 5 && info.Color != 2 ? info.N : info.Width * info.Height);

            if (joinTexts is null || string.IsNullOrEmpty(info.Comment))
            {
                continue;
            }

            if (!infoByComment.TryGetValue(info.Comment, out var list))
            {
                list = [];
                infoByComment[info.Comment] = list;
            }

            list.Add(info);
        }

        return infos;
    }

    /// <summary>
    /// 创建一次搜索所需的上下文对象。
    /// </summary>
    private SearchContext CreateSearchContext(ScreenBits bits, int x, int y, int w, int h, int zx, int zy, double err1, double err0, double zoomW, double zoomH, int maxPattern, bool findAll, bool hasJoinTexts)
    {
        var allPosMax = findAll || hasJoinTexts ? 10000 : 1;
        return new SearchContext
        {
            Bits = bits,
            Sx = x,
            Sy = y,
            Sw = w,
            Sh = h,
            Zx = zx,
            Zy = zy,
            Err1 = err1,
            Err0 = err0,
            ZoomW = zoomW,
            ZoomH = zoomH,
            S1 = new int[Math.Max(maxPattern, 1)],
            S0 = new int[Math.Max(maxPattern, 1)],
            AllPos = new int[allPosMax],
            Errors = new int[allPosMax],
            AllPosMax = allPosMax,
        };
    }

    /// <summary>
    /// 执行单轮搜索。
    /// 返回 <see langword="true"/> 表示已经得到“可直接返回”的结果。
    /// </summary>
    private bool RunSearchPass(List<FindTextResult> results, SearchContext context, IReadOnlyList<PicInfoData> infos, Dictionary<string, List<PicInfoData>> infoByComment, IReadOnlyList<string>? joinTexts, bool findAll, int offsetX, int offsetY, int dir)
    {
        return joinTexts is null || joinTexts.Count == 0
            ? RunDirectSearch(results, context, infos, findAll, dir)
            : RunJoinedSearch(results, context, infoByComment, joinTexts, findAll, offsetX, offsetY, dir);
    }

    /// <summary>
    /// 普通模式：逐个模板直接搜索。
    /// </summary>
    private bool RunDirectSearch(List<FindTextResult> results, SearchContext context, IReadOnlyList<PicInfoData> infos, bool findAll, int dir)
    {
        foreach (var info in infos)
        {
            var ok = PicFind(context, info, dir, context.Sx, context.Sy, context.Sw, context.Sh);
            if (AppendFoundResults(results, context, info, ok) && !findAll)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 把 <see cref="PicFind"/> 返回的位置数组转换成结果对象。
    /// </summary>
    private bool AppendFoundResults(List<FindTextResult> results, SearchContext context, PicInfoData info, int ok)
    {
        for (var i = 0; i < ok; i++)
        {
            var pos = context.AllPos[i];
            var rx = (pos & 0xFFFF) + context.Zx;
            var ry = (pos >> 16) + context.Zy;
            var rw = (int)Math.Floor(info.Width * context.ZoomW);
            var rh = (int)Math.Floor(info.Height * context.ZoomH);
            results.Add(new FindTextResult
            {
                X1 = rx,
                Y1 = ry,
                Width = rw,
                Height = rh,
                Id = info.Comment
            });
        }

        return ok > 0;
    }

    /// <summary>
    /// 组合模式：按顺序搜索多个字符/词，并限制相对偏移范围。
    /// </summary>
    private bool RunJoinedSearch(List<FindTextResult> results, SearchContext context, Dictionary<string, List<PicInfoData>> infoByComment, IReadOnlyList<string> joinTexts, bool findAll, int offsetX, int offsetY, int dir)
    {
        foreach (var line in joinTexts)
        {
            var parts = Regex.Replace(line, "\\s*\\|[|\\s]*", "|")
                .Trim('|', ' ', '\t')
                .Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

            if (parts.Length == 0)
            {
                continue;
            }

            if (JoinText(results, context, infoByComment, parts, 0, offsetX, offsetY, findAll, dir, 0, 0, 0, context.Sx, context.Sy, context.Sw, context.Sh) && !findAll)
            {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// 从屏幕区域生成 FindText 模式字符串。
    /// 相当于 AHK 里的“截图转字库字符串”核心步骤，不包含 GUI 部分。
    /// </summary>
    public string? GetTextFromScreen(
        int x1,
        int y1,
        int x2,
        int y2,
        out int rx,
        out int ry,
        string threshold = "",
        bool screenShot = true,
        bool cut = true)
    {
        rx = 0;
        ry = 0;
        CreateSearchRect(x1, y1, x2, y2, out var x, out var y, out var w, out var h);
        var bits = GetBitsFromScreen(ref x, ref y, ref w, ref h, screenShot, out var zx, out var zy, out _, out _);
        if (w < 1 || h < 1 || bits.Scan0 == 0)
        {
            return null;
        }

        x -= zx;
        y -= zy;
        var grays = CaptureGrayRegion(bits, x, y, w, h);
        var bitString = BuildBitString(grays, ref x, ref y, ref w, ref h, ref threshold);
        if (bitString is null)
        {
            return null;
        }

        var cutUp = 0;
        var cutDown = 0;
        if (cut)
        {
            bitString = TrimBinaryRows(bitString, w, out cutUp, out cutDown);
        }

        rx = x + zx + (w / 2);
        ry = y + zy + cutUp + ((h - cutUp - cutDown) / 2);
        return $"|<>{threshold}${w}.{BitToBase64(bitString)}";
    }

    /// <summary>
    /// 把指定区域的屏幕像素转成灰度数组。
    /// </summary>
    private unsafe byte[] CaptureGrayRegion(ScreenBits bits, int x, int y, int w, int h)
    {
        var grays = new byte[w * h];
        var bmp = (byte*)bits.Scan0;
        var index = 0;
        for (var yy = 0; yy < h; yy++)
        {
            var row = bmp + ((y + yy) * bits.Stride) + (x * 4);
            for (var xx = 0; xx < w; xx++)
            {
                var p = row + (xx * 4);
                grays[index++] = Gray(p[2], p[1], p[0]);
            }
        }

        return grays;
    }

    /// <summary>
    /// 根据阈值模式构造 01 位串，并在灰度差分模式下同步修正搜索矩形。
    /// </summary>
    private string? BuildBitString(byte[] grays, ref int x, ref int y, ref int w, ref int h, ref string threshold)
    {
        if (threshold.Contains("**", StringComparison.Ordinal))
        {
            var diff = ParseInt(threshold.Trim('*', ' '));
            if (diff == 0)
            {
                diff = 50;
            }

            var sourceWidth = w;
            var innerW = w - 2;
            var innerH = h - 2;
            if (innerW < 1 || innerH < 1)
            {
                return null;
            }

            var bitString = BuildGrayDiffBitString(grays, sourceWidth, diff, innerW, innerH);
            threshold = $"**{diff}";
            x += 1;
            y += 1;
            w = innerW;
            h = innerH;
            return bitString;
        }

        var value = threshold.Trim('*', ' ');
        var t = value.Length == 0 ? AutoThreshold(grays) : ParseInt(value);
        threshold = $"*{t}";
        return BuildGrayThresholdBitString(grays, t);
    }

    /// <summary>
    /// 灰度差分模式下，把区域转换成边缘 01 位串。
    /// </summary>
    private static string BuildGrayDiffBitString(byte[] grays, int sourceWidth, int diff, int innerW, int innerH)
    {
        var sb = new StringBuilder(innerW * innerH);
        for (var yy = 0; yy < innerH; yy++)
        {
            for (var xx = 0; xx < innerW; xx++)
            {
                var i = yy * sourceWidth + xx + 1;
                var n = grays[i] + diff;
                var v = grays[i - 1] > n || grays[i + 1] > n
                    || grays[i - sourceWidth] > n || grays[i + sourceWidth] > n
                    || grays[i - sourceWidth - 1] > n || grays[i - sourceWidth + 1] > n
                    || grays[i + sourceWidth - 1] > n || grays[i + sourceWidth + 1] > n;
                sb.Append(v ? '1' : '0');
            }
        }

        return sb.ToString();
    }

    /// <summary>
    /// 普通灰度阈值模式下，把区域转换成 01 位串。
    /// </summary>
    private static string BuildGrayThresholdBitString(byte[] grays, int threshold)
    {
        var sb = new StringBuilder(grays.Length);
        foreach (var gray in grays)
        {
            sb.Append(gray <= threshold ? '1' : '0');
        }

        return sb.ToString();
    }

    /// <summary>
    /// 去掉顶部和底部整行同色边框。
    /// </summary>
    private static string TrimBinaryRows(string bitString, int rowWidth, out int cutUp, out int cutDown)
    {
        cutUp = 0;
        cutDown = 0;
        var row0 = new string(bitString.Length > 0 ? bitString[0] : '0', rowWidth);
        var row1 = new string(bitString.Length > 0 ? (bitString[0] == '1' ? '0' : '1') : '1', rowWidth);

        while (bitString.StartsWith(row0, StringComparison.Ordinal) || bitString.StartsWith(row1, StringComparison.Ordinal))
        {
            bitString = bitString[rowWidth..];
            cutUp++;
            if (bitString.Length < rowWidth)
            {
                break;
            }
        }

        while (bitString.Length >= rowWidth)
        {
            var tail = bitString[^rowWidth..];
            if (tail == row0 || tail == row1)
            {
                bitString = bitString[..^rowWidth];
                cutDown++;
            }
            else
            {
                break;
            }
        }

        return bitString;
    }

    /// <summary>
    /// 从最近截图中读取某点颜色，返回 `0xRRGGBB`。
    /// </summary>
    public uint GetColor(int x, int y)
    {
        var px = x;
        var py = y;
        var w = 1;
        var h = 1;
        var bits = GetBitsFromScreen(ref px, ref py, ref w, ref h, false, out var zx, out var zy, out var zw, out var zh);
        var rx = x - zx;
        var ry = y - zy;
        if (rx < 0 || ry < 0 || rx >= zw || ry >= zh || bits.Scan0 == 0)
        {
            return 0xFFFFFF;
        }

        return (uint)Marshal.ReadInt32(bits.Scan0, (ry * bits.Stride) + (rx * 4)) & 0xFFFFFFu;
    }

    /// <summary>
    /// 直接修改最近截图缓存中的像素颜色，不会改动真实屏幕。
    /// </summary>
    public void SetColor(int x, int y, uint color)
    {
        var px = x;
        var py = y;
        var w = 1;
        var h = 1;
        var bits = GetBitsFromScreen(ref px, ref py, ref w, ref h, false, out var zx, out var zy, out var zw, out var zh);
        var rx = x - zx;
        var ry = y - zy;
        if (rx < 0 || ry < 0 || rx >= zw || ry >= zh || bits.Scan0 == 0)
        {
            return;
        }

        Marshal.WriteInt32(bits.Scan0, (ry * bits.Stride) + (rx * 4), unchecked((int)color));
    }

    /// <summary>
    /// 兼容 AHK `ImageSearch` 风格的简单包装。
    /// </summary>
    public List<FindTextResult> ImageSearch(
        string imageFile,
        int x1 = 0,
        int y1 = 0,
        int x2 = 0,
        int y2 = 0,
        bool screenShot = true,
        bool findAll = false,
        int dir = 1)
    {
        var builder = new StringBuilder();
        foreach (var item in imageFile.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            if (item.Contains('$'))
            {
                builder.Append('|').Append(item);
                continue;
            }

            var variationMatch = Regex.Match(item, @"(^|\s)\*(\d+)\s");
            var variation = variationMatch.Success ? ParseInt(variationMatch.Groups[2].Value) * 0x010101 : 0;
            var transMatch = Regex.Match(item, @"(?i)(^|\s)\*Trans(\S+)\s");
            var trans = transMatch.Success ? "/" + transMatch.Groups[2].Value.Trim('/') : string.Empty;
            var path = Regex.Replace(item, @"(^|\s)\*\S+", string.Empty).Trim();
            builder.Append('|').Append($"##{variation:X6}").Append(trans).Append('$').Append(path);
        }

        return FindText(builder.ToString(), x1, y1, x2, y2, 0, 0, screenShot, findAll, null, 20, 10, dir);
    }

    /// <summary>
    /// 兼容 AHK `PixelSearch` 风格的简单包装。
    /// </summary>
    public List<FindTextResult> PixelSearch(
        string colorId,
        int variation = 0,
        int x1 = 0,
        int y1 = 0,
        int x2 = 0,
        int y2 = 0,
        bool screenShot = true,
        bool findAll = false,
        int dir = 1)
    {
        var text = $"##{(variation * 0x010101):X6}$0/0/{colorId.Replace("|", "/", StringComparison.Ordinal).Trim(' ', '/')}";
        return ImageSearch(text, x1, y1, x2, y2, screenShot, findAll, dir);
    }

    /// <summary>
    /// 统计某颜色在区域内出现的次数。
    /// 实现上复用了 `PicFind` 的颜色匹配逻辑。
    /// </summary>
    public int PixelCount(
        string colorId,
        int variation = 0,
        int x1 = 0,
        int y1 = 0,
        int x2 = 0,
        int y2 = 0,
        bool screenShot = true)
    {
        CreateSearchRect(x1, y1, x2, y2, out var x, out var y, out var w, out var h);
        var bits = GetBitsFromScreen(ref x, ref y, ref w, ref h, screenShot, out var zx, out var zy, out _, out _);
        x -= zx;
        y -= zy;

        var text = $"##{(variation * 0x010101):X6}$0/0/{colorId.Split(',')[0].Replace("|", "/", StringComparison.Ordinal).Trim(' ', '/')}";
        var info = PicInfo(text);
        if (info is null)
        {
            return 0;
        }

        var context = new SearchContext
        {
            Bits = bits,
            Sx = x,
            Sy = y,
            Sw = w,
            Sh = h,
            Zx = zx,
            Zy = zy,
            Err1 = 0,
            Err0 = 0,
            ZoomW = 1,
            ZoomH = 1,
            S1 = new int[Math.Max(info.N, 1)],
            S0 = new int[Math.Max(info.N, 1)],
            AllPos = [],
            Errors = [],
            AllPosMax = 0
        };

        return PicFind(context, info, 1, x, y, w, h);
    }

    /// <summary>
    /// 生成一个纯色块模板字符串。
    /// 适合做“某区域有足够多指定颜色像素”的检测。
    /// </summary>
    public string ColorBlock(string colorId, int w, int h, int count1 = 0, int count0 = 0)
    {
        if (count0 > 0)
        {
            count1 = 0;
        }

        var value = 1 - (count1 > 0 ? (double)count1 / (w * h) : 0);
        var value0 = 1 - (count0 > 0 ? (double)count0 / (w * h) : 0);
        var fill = new string(count0 > 0 ? '0' : '1', w * h);
        return $"|<>[{value.ToString(CultureInfo.InvariantCulture)},{value0.ToString(CultureInfo.InvariantCulture)}]{colorId.Replace("|", "/", StringComparison.Ordinal).Trim(' ', '/')}${w}.{BitToBase64(fill)}";
    }

    /// <summary>
    /// 把多个识别结果按横向阅读顺序拼成文本。
    /// </summary>
    public OcrResult Ocr(IReadOnlyList<FindTextResult> ok, int offsetX = 20, int offsetY = 20, int overlapW = 0)
    {
        var result = new OcrResult();
        if (ok.Count == 0)
        {
            return result;
        }

        var minX = ok.Min(v => v.X1);
        var maxX = ok.Max(v => v.X1);
        int? currentY = null;
        int? dx = null;
        var cursor = minX;
        var minY = 0;
        var maxY = 0;
        var started = false;
        var sb = new StringBuilder();

        while (cursor <= maxX)
        {
            FindTextResult? left = null;
            foreach (var item in ok)
            {
                if (item.X1 < cursor)
                {
                    continue;
                }

                if (currentY is not null && Math.Abs(item.Y1 - currentY.Value) > offsetY)
                {
                    continue;
                }

                if (left is null || item.X1 < left.X1)
                {
                    left = item;
                }
            }

            if (left is null)
            {
                break;
            }

            if (!started)
            {
                result.X = left.X1;
                minY = left.Y1;
                maxY = left.Y1 + left.Height;
                started = true;
            }

            if (sb.Length > 0 && dx is not null && left.X1 > dx.Value)
            {
                sb.Append('*');
            }

            sb.Append(left.Id);
            cursor = left.X1 + left.Width - Math.Min(overlapW, left.Width / 2);
            dx = left.X1 + left.Width + offsetX;
            currentY = left.Y1;
            minY = Math.Min(minY, left.Y1);
            maxY = Math.Max(maxY, left.Y1 + left.Height);
        }

        result.Text = sb.ToString();
        result.Y = minY;
        result.Width = cursor - result.X;
        result.Height = maxY - minY;
        return result;
    }

    /// <summary>
    /// 按“从左到右、从上到下”排序，允许同一行内有少量高度偏差。
    /// </summary>
    public List<FindTextResult> Sort(IReadOnlyList<FindTextResult> ok, int dy = 10)
    {
        var yRows = new List<int>();
        return ok
            .Select((item, index) =>
            {
                var y = item.Y;
                foreach (var row in yRows)
                {
                    if (Math.Abs(y - row) <= dy)
                    {
                        y = row;
                        goto done;
                    }
                }

                yRows.Add(y);

            done:
                return (SortKey: y * 150000L + item.X, Index: index, Item: item);
            })
            .OrderBy(v => v.SortKey)
            .Select(v => v.Item)
            .ToList();
    }

    /// <summary>
    /// 按距给定点最近的顺序排序。
    /// </summary>
    public List<FindTextResult> Sort2(IReadOnlyList<FindTextResult> ok, int px, int py)
    {
        return ok.OrderBy(v => ((long)v.X - px) * ((long)v.X - px) + ((long)v.Y - py) * ((long)v.Y - py)).ToList();
    }

    /// <summary>
    /// 按指定搜索方向排序，等价于原版 `Sort3`。
    /// </summary>
    public List<FindTextResult> Sort3(IReadOnlyList<FindTextResult> ok, int dir = 1)
    {
        const long n = 150000;
        return ok.OrderBy(v => dir switch
        {
            1 => v.X1 + v.Y1 * n,
            2 => -v.X1 + v.Y1 * n,
            3 => v.X1 - v.Y1 * n,
            4 => -v.X1 - v.Y1 * n,
            5 => v.Y1 + v.X1 * n,
            6 => -v.Y1 + v.X1 * n,
            7 => v.Y1 - v.X1 * n,
            8 => -v.Y1 - v.X1 * n,
            _ => v.X1 + v.Y1 * n
        }).ToList();
    }

    /// <summary>
    /// 把模式字符串中的位图还原成可阅读的 ASCII 形状。
    /// 便于调试模板内容。
    /// </summary>
    public string ASCII(string text)
    {
        var match = Regex.Match(text, "\\$(\\d+)\\.([\\w+/]+)");
        if (!match.Success)
        {
            return string.Empty;
        }

        var width = ParseInt(match.Groups[1].Value);
        var bits = Base64ToBit(match.Groups[2].Value);
        var sb = new StringBuilder();
        for (var i = 0; i < bits.Length; i += width)
        {
            var line = bits.Substring(i, Math.Min(width, bits.Length - i))
                .Replace("0", "_")
                .Replace("1", "0");
            sb.AppendLine(line);
        }

        return sb.ToString().TrimEnd();
    }

    /// <summary>
    /// 把自定义 64 进制字符串恢复成 01 位串。
    /// </summary>
    public static string Base64ToBit(string value)
    {
        var sb = new StringBuilder(value.Length * 6);
        foreach (var ch in value)
        {
            var index = CustomBase64Chars.IndexOf(ch);
            if (index >= 0)
            {
                sb.Append(Convert.ToString(index, 2).PadLeft(6, '0'));
            }
        }

        return Regex.Replace(sb.ToString(), "10*$", string.Empty);
    }

    /// <summary>
    /// 把 01 位串编码成 FindText 使用的自定义 64 进制字符串。
    /// </summary>
    public static string BitToBase64(string value)
    {
        var bits = Regex.Replace(value, "[^01]+", string.Empty);
        var append = 6 - (bits.Length % 6);
        if (append == 0)
        {
            append = 6;
        }

        bits += "100000"[..append];
        var sb = new StringBuilder(bits.Length / 6);
        for (var i = 0; i < bits.Length; i += 6)
        {
            var chunk = bits.Substring(i, 6);
            var index = Convert.ToInt32(chunk, 2);
            sb.Append(CustomBase64Chars[index]);
        }

        return sb.ToString();
    }

    /// <summary>
    /// 组合文本搜索。
    /// 例如先找到第一个字，再在其右侧指定偏移范围内递归找后续字符。
    /// </summary>
    private bool JoinText(
        List<FindTextResult> results,
        SearchContext context,
        Dictionary<string, List<PicInfoData>> info2,
        IReadOnlyList<string> text,
        int index,
        int offsetX,
        int offsetY,
        bool findAll,
        int dir,
        int minX,
        int minY,
        int maxY,
        int sx,
        int sy,
        int sw,
        int sh)
    {
        if (index >= text.Count || !info2.TryGetValue(text[index], out var group))
        {
            return false;
        }

        foreach (var info in group)
        {
            var currentWidth = index == 0
                ? sw
                : Math.Min(sx + offsetX + (int)Math.Floor(info.Width * context.ZoomW), context.Sx + context.Sw) - sx;
            if (currentWidth < 1)
            {
                continue;
            }

            var ok = PicFind(context, info, dir, sx, sy, currentWidth, sh);
            if (ok <= 0)
            {
                continue;
            }

            var positions = context.AllPos.Take(ok).ToArray();
            foreach (var pos in positions)
            {
                var x = pos & 0xFFFF;
                var y = pos >> 16;
                var w = (int)Math.Floor(info.Width * context.ZoomW);
                var h = (int)Math.Floor(info.Height * context.ZoomH);
                if (index == 0)
                {
                    minX = x;
                    minY = y;
                    maxY = y + h;
                }

                var minY1 = Math.Min(y, minY);
                var maxY1 = Math.Max(y + h, maxY);
                var sx1 = x + w;

                if (index < text.Count - 1)
                {
                    var sy1 = Math.Max(minY1 - offsetY, context.Sy);
                    var sh1 = Math.Min(maxY1 + offsetY, context.Sy + context.Sh) - sy1;
                    if (sh1 < 1)
                    {
                        continue;
                    }

                    if (JoinText(results, context, info2, text, index + 1, offsetX, offsetY, findAll, 5, minX, minY1, maxY1, sx1, sy1, 0, sh1) && (index > 0 || !findAll))
                    {
                        return true;
                    }
                }
                else
                {
                    var rx = minX + context.Zx;
                    var ry = minY1 + context.Zy;
                    results.Add(new FindTextResult
                    {
                        X1 = rx,
                        Y1 = ry,
                        Width = sx1 - minX,
                        Height = maxY1 - minY1,
                        Id = string.Concat(text)
                    });

                    if (index > 0 || !findAll)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

    /// <summary>
    /// 核心匹配函数，对应 AHK 内嵌 C 代码里的 `PicFind`。
    /// 这里保留了原算法的几个关键阶段：
    /// 1. 预处理模板，生成前景/背景采样点。
    /// 2. 需要时把截图转换成二值图或边缘图。
    /// 3. 按指定方向扫描区域并进行容错匹配。
    /// 4. 对结果按误差排序并去重。
    /// </summary>
    private unsafe int PicFind(SearchContext context, PicInfoData info, int dir, int sx, int sy, int sw, int sh)
    {
        var w = info.Width;
        var h = info.Height;
        var mode = info.Mode;
        var color = info.Color;
        var n = info.N;
        if (context.Bits.Scan0 == 0 || mode is < 1 or > 5 || sw < 1 || sh < 1)
        {
            return 0;
        }

        var err1f = info.SetErr ? info.Err1 : context.Err1;
        var err0f = info.SetErr ? info.Err0 : context.Err0;
        var err1Value = (int)Math.Floor(Math.Abs(err1f) * 1024d);
        var err0Value = (int)Math.Floor(Math.Abs(err0f) * 1024d);
        var moreErr = err1f < 0 || err0f < 0;
        var newW = (int)Math.Floor(w * context.ZoomW);
        var newH = (int)Math.Floor(h * context.ZoomH);
        if (newW < 1 || newH < 1 || sw < newW || sh < newH)
        {
            return 0;
        }

        var s1 = context.S1;
        var s0 = context.S0;
        var len1 = 0;
        var len0 = 0;
        var ok = 0;
        var bmp = (byte*)context.Bits.Scan0;
        var stride = context.Bits.Stride;
        var zw = context.Bits.Zw;
        var zh = context.Bits.Zh;
        var pic = false;
        var shape = false;
        ReadOnlySpan<uint> cors = ReadOnlySpan<uint>.Empty;
        byte[]? searchMap = null;
        var text = info.RawData;
        uint firstShapeColor = 0;
        var picDR = 0;
        var picDG = 0;
        var picDB = 0;
        var colorPosThreshold = 0;
        var colorPosOffset = 0;

        if (mode == 5)
        {
            // mode=5：FindPic / FindMultiColor / FindColor / FindShape
            if (color == 2)
            {
                // FindPic：模板是完整位图，后面跟透明色规则。
                pic = true;
                cors = MemoryMarshal.Cast<byte, uint>(text.AsSpan(w * h * 4));
                var step = (err0Value >> 10) + 1;
                var n2 = n * 2;
                var k = n2 <= 2 || cors[2] < 0x1000000;
                var c2 = cors[1];
                var r0 = (int)((c2 >> 16) & 0xFF);
                var g0 = (int)((c2 >> 8) & 0xFF);
                var b0 = (int)(c2 & 0xFF);
                picDR = r0 * r0;
                picDG = g0 * g0;
                picDB = b0 * b0;

                for (var yy = 0; yy < h; yy += step)
                {
                    for (var xx = 0; xx < w; xx += step)
                    {
                        var o = (yy * w + xx) * 4;
                        var rr = text[o + 2];
                        var gg = text[o + 1];
                        var bb = text[o];
                        var v = false;
                        for (var i = 2; i < n2;)
                        {
                            var c1 = cors[i++];
                            c2 = cors[i++];
                            var r = ((int)((c1 >> 16) & 0xFF)) - rr;
                            var g = ((int)((c1 >> 8) & 0xFF)) - gg;
                            var b = ((int)(c1 & 0xFF)) - bb;
                            v = c2 < 0x40000000
                                ? (1024 + (r + rr + rr)) * r * r + 2048 * g * g + (1534 - (r + rr + rr)) * b * b <= c2
                                : r * r <= (int)((c2 >> 16) & 0xFF) * (int)((c2 >> 16) & 0xFF)
                                    && g * g <= (int)((c2 >> 8) & 0xFF) * (int)((c2 >> 8) & 0xFF)
                                    && b * b <= (int)(c2 & 0xFF) * (int)(c2 & 0xFF);
                            if (v)
                            {
                                break;
                            }
                        }

                        if (v == k)
                        {
                            continue;
                        }

                        s1[len1] = (yy * newH / h) * stride + (xx * newW / w) * 4;
                        s0[len1++] = (rr << 16) | (gg << 8) | bb;
                    }
                }
            }
            else
            {
                // MultiColor / Shape：模板由若干采样点组成。
                shape = color == 1;
                cors = MemoryMarshal.Cast<byte, uint>(text);
                var o = 0;
                for (var i = 0; i < n; i++, o += 22)
                {
                    var c = cors[o];
                    var yy = (int)(c >> 16);
                    var xx = (int)(c & 0xFFFF);
                    s1[len1] = (yy * newH / h) * stride + (xx * newW / w) * 4;
                    s0[len1++] = o + (int)cors[o + 1] * 2;
                }

                cors = cors[2..];
            }
        }
        else
        {
            // 文字/二值图模式：模板本身是由 01 串编码得到的。
            var o = 0;
            for (var yy = 0; yy < h; yy++)
            {
                for (var xx = 0; xx < w; xx++)
                {
                    var i = mode == 4
                        ? (yy * newH / h) * stride + (xx * newW / w) * 4
                        : (yy * newH / h) * sw + (xx * newW / w);
                    if (text[o++] == (byte)'1')
                    {
                        s1[len1++] = i;
                    }
                    else
                    {
                        s0[len0++] = i;
                    }
                }
            }

            if (mode == 4)
            {
                var yy = (int)(color >> 16);
                var xx = (int)(color & 0xFFFF);
                colorPosOffset = (yy * newH / h) * stride + (xx * newW / w) * 4;
                colorPosThreshold = n;
            }
            else
            {
                // 把搜索区域转换成算法需要的中间图。
                searchMap = mode switch
                {
                    1 => BuildColorMap(bmp, stride, sx, sy, sw, sh, text, w, h, n),
                    2 => BuildGrayThresholdMap(bmp, stride, sx, sy, sw, sh, (int)color),
                    3 => BuildGrayDifferenceMap(bmp, stride, zw, zh, sx, sy, sw, sh, (int)color),
                    _ => null
                };

                if (searchMap is null)
                {
                    return 0;
                }

                if (moreErr)
                {
                    // 原版中的横向膨胀容错：允许文本轻微错位。
                    var expanded = new byte[sw * sh];
                    for (var yy = 0; yy < sh; yy++)
                    {
                        for (var xx = 0; xx < sw; xx++)
                        {
                            var i = yy * sw + xx;
                            expanded[i] = (byte)(searchMap[i]
                                | (xx == 0 ? 0 : searchMap[i - 1])
                                | (xx == sw - 1 ? 0 : searchMap[i + 1]));
                        }
                    }

                    searchMap = expanded;
                }
            }
        }

        var allowErr1 = (len1 * err1Value) >> 10;
        var allowErr0 = (len0 * err0Value) >> 10;
        if (allowErr1 >= len1)
        {
            len1 = 0;
        }

        if (allowErr0 >= len0)
        {
            len0 = 0;
        }

        var max = Math.Max(len1, len0);
        var maxX = sw - newW;
        var maxY = sh - newH;
        if (maxX < 0 || maxY < 0)
        {
            return 0;
        }

        foreach (var (x, y) in EnumeratePositions(dir, maxX, maxY))
        {
            // 扫描每一个候选位置，并在该位置上做容错匹配。
            var e1 = allowErr1;
            var e0 = allowErr0;
            var matched = true;

            if (mode < 4)
            {
                var offset = y * sw + x;
                for (var i = 0; i < max; i++)
                {
                    if (i < len1 && searchMap![offset + s1[i]] < 2 && --e1 < 0)
                    {
                        matched = false;
                        break;
                    }

                    if (i < len0 && (searchMap![offset + s0[i]] & 1) == 0 && --e0 < 0)
                    {
                        matched = false;
                        break;
                    }
                }
            }
            else if (mode == 5)
            {
                var offset = ((sy + y) * stride) + ((sx + x) * 4);
                if (pic)
                {
                    for (var i = 0; i < max; i++)
                    {
                        var j = offset + s1[i];
                        var c = s0[i];
                        var r = bmp[j + 2] - ((c >> 16) & 0xFF);
                        var g = bmp[j + 1] - ((c >> 8) & 0xFF);
                        var b = bmp[j] - (c & 0xFF);
                        if ((r * r > picDR || g * g > picDG || b * b > picDB) && --e1 < 0)
                        {
                            matched = false;
                            break;
                        }
                    }
                }
                else
                {
                    for (var i = 0; i < max; i++)
                    {
                        var j = offset + s1[i];
                        var rr = bmp[j + 2];
                        var gg = bmp[j + 1];
                        var bb = bmp[j];
                        var jj = i * 22;
                        var exclude = cors[jj] > 0xFFFFFF;
                        var end = s0[i];
                        var pointMatched = false;
                        while (jj < end)
                        {
                            var c1 = cors[jj++];
                            var c2 = cors[jj++];
                            if (shape)
                            {
                                if (i == 0)
                                {
                                    firstShapeColor = (uint)((rr << 16) | (gg << 8) | bb);
                                    pointMatched = true;
                                    break;
                                }

                                c1 = firstShapeColor;
                            }

                            var r = ((int)((c1 >> 16) & 0xFF)) - rr;
                            var g = ((int)((c1 >> 8) & 0xFF)) - gg;
                            var b = ((int)(c1 & 0xFF)) - bb;
                            var v = c2 < 0x40000000
                                ? (1024 + (r + rr + rr)) * r * r + 2048 * g * g + (1534 - (r + rr + rr)) * b * b <= c2
                                : r * r <= (int)((c2 >> 16) & 0xFF) * (int)((c2 >> 16) & 0xFF)
                                    && g * g <= (int)((c2 >> 8) & 0xFF) * (int)((c2 >> 8) & 0xFF)
                                    && b * b <= (int)(c2 & 0xFF) * (int)(c2 & 0xFF);
                            if (v)
                            {
                                if (exclude)
                                {
                                    pointMatched = false;
                                    break;
                                }

                                pointMatched = true;
                                break;
                            }
                        }

                        if (!pointMatched && exclude)
                        {
                            pointMatched = true;
                        }

                        if (!pointMatched)
                        {
                            if (i == 0 || --e1 < 0)
                            {
                                matched = false;
                                break;
                            }
                        }
                    }
                }
            }
            else
            {
                var offset = ((sy + y) * stride) + ((sx + x) * 4);
                var j = offset + colorPosOffset;
                var rr = bmp[j + 2];
                var gg = bmp[j + 1];
                var bb = bmp[j];
                for (var i = 0; i < max; i++)
                {
                    if (i < len1)
                    {
                        j = offset + s1[i];
                        var r = bmp[j + 2] - rr;
                        var g = bmp[j + 1] - gg;
                        var b = bmp[j] - bb;
                        if ((1024 + (r + rr + rr)) * r * r + 2048 * g * g + (1534 - (r + rr + rr)) * b * b > colorPosThreshold && --e1 < 0)
                        {
                            matched = false;
                            break;
                        }
                    }

                    if (i < len0)
                    {
                        j = offset + s0[i];
                        var r = bmp[j + 2] - rr;
                        var g = bmp[j + 1] - gg;
                        var b = bmp[j] - bb;
                        if ((1024 + (r + rr + rr)) * r * r + 2048 * g * g + (1534 - (r + rr + rr)) * b * b <= colorPosThreshold && --e0 < 0)
                        {
                            matched = false;
                            break;
                        }
                    }
                }
            }

            if (!matched)
            {
                continue;
            }

            ok++;
            if (context.AllPosMax > 0)
            {
                context.AllPos[ok - 1] = ((sy + y) << 16) | (sx + x);
                if (dir == 0)
                {
                    context.Errors[ok - 1] = allowErr1 - e1;
                }

                if (ok >= context.AllPosMax)
                {
                    break;
                }
            }
        }

        if (context.AllPosMax <= 0 || newW * newH == 1)
        {
            return ok;
        }

        if (dir == 0 && ok > 1)
        {
            // dir=0 时，原版会按“误差最小优先”输出结果。
            Array.Sort(context.Errors, context.AllPos, 0, ok);
        }

        // 去掉互相重叠太近的结果，保留较优命中。
        var overlapW = (newW * newW) >> 2;
        var overlapH = (newH * newH) >> 2;
        var total = ok;
        ok = 0;
        for (var i = 0; i < total; i++)
        {
            var c1 = context.AllPos[i];
            var x1 = c1 & 0xFFFF;
            var y1 = c1 >> 16;
            var hit = false;
            for (var j = 0; j < ok; j++)
            {
                var c2 = context.AllPos[j];
                var dx = (c2 & 0xFFFF) - x1;
                var dy = (c2 >> 16) - y1;
                if (dx * dx < overlapW && dy * dy < overlapH)
                {
                    hit = true;
                    break;
                }
            }

            if (!hit)
            {
                context.AllPos[ok++] = c1;
            }
        }

        return ok;
    }

    /// <summary>
    /// 根据颜色规则把搜索区域转成二值图：匹配像素记为 2，不匹配记为 1。
    /// </summary>
    private unsafe byte[] BuildColorMap(byte* bmp, int stride, int sx, int sy, int sw, int sh, byte[] text, int w, int h, int n)
    {
        var map = new byte[sw * sh];
        var cors = MemoryMarshal.Cast<byte, uint>(text.AsSpan(w * h));
        var n2 = n * 2;
        var k = cors[0] < 0x1000000;
        var index = 0;
        for (var yy = 0; yy < sh; yy++)
        {
            var row = bmp + ((sy + yy) * stride) + (sx * 4);
            for (var xx = 0; xx < sw; xx++, index++)
            {
                var p = row + xx * 4;
                var rr = p[2];
                var gg = p[1];
                var bb = p[0];
                var v = false;
                for (var i = 0; i < n2;)
                {
                    var c1 = cors[i++];
                    var c2 = cors[i++];
                    var r = ((int)((c1 >> 16) & 0xFF)) - rr;
                    var g = ((int)((c1 >> 8) & 0xFF)) - gg;
                    var b = ((int)(c1 & 0xFF)) - bb;
                    v = c2 < 0x40000000
                        ? (1024 + (r + rr + rr)) * r * r + 2048 * g * g + (1534 - (r + rr + rr)) * b * b <= c2
                        : r * r <= (int)((c2 >> 16) & 0xFF) * (int)((c2 >> 16) & 0xFF)
                            && g * g <= (int)((c2 >> 8) & 0xFF) * (int)((c2 >> 8) & 0xFF)
                            && b * b <= (int)(c2 & 0xFF) * (int)(c2 & 0xFF);
                    if (v)
                    {
                        break;
                    }
                }

                map[index] = (byte)(v == k ? 2 : 1);
            }
        }

        return map;
    }

    /// <summary>
    /// 按灰度阈值生成二值图。
    /// </summary>
    private unsafe byte[] BuildGrayThresholdMap(byte* bmp, int stride, int sx, int sy, int sw, int sh, int color)
    {
        var map = new byte[sw * sh];
        var threshold = (color + 1) << 7;
        var index = 0;
        for (var yy = 0; yy < sh; yy++)
        {
            var row = bmp + ((sy + yy) * stride) + (sx * 4);
            for (var xx = 0; xx < sw; xx++, index++)
            {
                var p = row + xx * 4;
                map[index] = (byte)((p[2] * 38 + p[1] * 75 + p[0] * 15 < threshold) ? 2 : 1);
            }
        }

        return map;
    }

    /// <summary>
    /// 生成灰度差分图，用于强调边缘和轮廓。
    /// </summary>
    private unsafe byte[] BuildGrayDifferenceMap(byte* bmp, int stride, int zw, int zh, int sx, int sy, int sw, int sh, int color)
    {
        var gw = sw + 2;
        var gh = sh + 2;
        var gs = new byte[gw * gh];
        for (var yy = sy - 1; yy <= sy + sh; yy++)
        {
            for (var xx = sx - 1; xx <= sx + sw; xx++)
            {
                var dst = (yy - sy + 1) * gw + (xx - sx + 1);
                if (xx < 0 || yy < 0 || xx >= zw || yy >= zh)
                {
                    gs[dst] = 0;
                    continue;
                }

                var p = bmp + (yy * stride) + (xx * 4);
                gs[dst] = (byte)((p[2] * 38 + p[1] * 75 + p[0] * 15) >> 7);
            }
        }

        var map = new byte[sw * sh];
        var index = 0;
        for (var yy = 1; yy <= sh; yy++)
        {
            for (var xx = 1; xx <= sw; xx++, index++)
            {
                var o = yy * gw + xx;
                var n = gs[o] + color;
                map[index] = (byte)(gs[o - 1] > n || gs[o + 1] > n
                    || gs[o - gw - 1] > n || gs[o - gw] > n || gs[o - gw + 1] > n
                    || gs[o + gw - 1] > n || gs[o + gw] > n || gs[o + gw + 1] > n ? 2 : 1);
            }
        }

        return map;
    }

    /// <summary>
    /// 根据方向枚举搜索位置。
    /// 1~8 是八种线性扫描方式，9 是中心螺旋搜索。
    /// </summary>
    private IEnumerable<(int X, int Y)> EnumeratePositions(int dir, int maxX, int maxY)
    {
        switch (dir)
        {
            case 2:
                for (var y = 0; y <= maxY; y++)
                    for (var x = maxX; x >= 0; x--)
                        yield return (x, y);
                yield break;
            case 3:
                for (var y = maxY; y >= 0; y--)
                    for (var x = 0; x <= maxX; x++)
                        yield return (x, y);
                yield break;
            case 4:
                for (var y = maxY; y >= 0; y--)
                    for (var x = maxX; x >= 0; x--)
                        yield return (x, y);
                yield break;
            case 5:
                for (var x = 0; x <= maxX; x++)
                    for (var y = 0; y <= maxY; y++)
                        yield return (x, y);
                yield break;
            case 6:
                for (var x = 0; x <= maxX; x++)
                    for (var y = maxY; y >= 0; y--)
                        yield return (x, y);
                yield break;
            case 7:
                for (var x = maxX; x >= 0; x--)
                    for (var y = 0; y <= maxY; y++)
                        yield return (x, y);
                yield break;
            case 8:
                for (var x = maxX; x >= 0; x--)
                    for (var y = maxY; y >= 0; y--)
                        yield return (x, y);
                yield break;
            case 9:
                foreach (var item in EnumerateSpiral(maxX, maxY))
                    yield return item;
                yield break;
            default:
                for (var y = 0; y <= maxY; y++)
                    for (var x = 0; x <= maxX; x++)
                        yield return (x, y);
                yield break;
        }
    }

    /// <summary>
    /// 从中心向四周螺旋展开，用于 `dir=9`。
    /// </summary>
    private IEnumerable<(int X, int Y)> EnumerateSpiral(int maxX, int maxY)
    {
        var x = (maxX) / 2;
        var y = (maxY) / 2;
        var allCount1 = (maxX + 1) * (maxY + 1);
        var i = Math.Max(maxX + 1, maxY + 1) + 8;
        var allCount2 = i * i;
        var runCount = 0;
        var dirCount = 1;
        var runDir = 0;
        for (var ii = 0; runCount < allCount1 && ii < allCount2;)
        {
            for (var jj = 0; jj < dirCount; jj++, ii++)
            {
                if (x >= 0 && x <= maxX && y >= 0 && y <= maxY)
                {
                    runCount++;
                    yield return (x, y);
                }

                switch (runDir)
                {
                    case 0:
                        y--;
                        break;
                    case 1:
                        x++;
                        break;
                    case 2:
                        y++;
                        break;
                    default:
                        x--;
                        break;
                }
            }

            if ((runDir & 1) == 1)
            {
                dirCount++;
            }

            runDir = (runDir + 1) & 3;
        }
    }

    /// <summary>
    /// 解析单个模板字符串，等价于 AHK `PicInfo`。
    /// 解析结果会被缓存。
    /// </summary>
    private PicInfoData? PicInfo(string text)
    {
        if (!text.Contains('$', StringComparison.Ordinal))
        {
            return null;
        }

        var key = text.Trim('|');
        if (_picInfoCache.TryGetValue(key, out var cached))
        {
            return cached;
        }

        var value = key;
        var comment = string.Empty;
        var setErr = false;
        var err1 = 0d;
        var err0 = 0d;

        var match = Regex.Match(value, "<([^>\n]*)>");
        if (match.Success)
        {
            value = value.Replace(match.Value, string.Empty, StringComparison.Ordinal);
            comment = match.Groups[1].Value.Trim();
        }

        match = Regex.Match(value, "\\[([^\\]\n]*)]");
        if (match.Success)
        {
            value = value.Replace(match.Value, string.Empty, StringComparison.Ordinal);
            var parts = (match.Groups[1].Value + ",").Split(',', StringSplitOptions.TrimEntries);
            setErr = true;
            err1 = ParseDouble(parts.ElementAtOrDefault(0));
            err0 = ParseDouble(parts.ElementAtOrDefault(1));
        }

        var dollar = value.IndexOf('$');
        if (dollar < 0)
        {
            return null;
        }

        var color = value[..dollar];
        var data = value[(dollar + 1)..].Trim();
        var mode = color.Contains("##", StringComparison.Ordinal)
            ? 5
            : color.Contains('#')
                ? 4
                : color.Contains("**", StringComparison.Ordinal)
                    ? 3
                    : color.Contains('*')
                        ? 2
                        : 1;

        color = Regex.Replace(color.Replace("@", "-", StringComparison.Ordinal), "[*#\\s]", string.Empty);
        if (mode is 1 or 5)
        {
            color = color.Replace("0x", string.Empty, StringComparison.OrdinalIgnoreCase);
        }

        PicInfoData? result = mode switch
        {
            5 => BuildMode5Info(color, data, comment, setErr, err1, err0),
            _ => BuildTextModeInfo(mode, color, data, comment, setErr, err1, err0)
        };

        if (result is not null)
        {
            _picInfoCache[key] = result;
        }

        return result;
    }

    /// <summary>
    /// 构建 `mode=5` 的模板信息：FindPic / MultiColor / FindShape。
    /// </summary>
    private PicInfoData? BuildMode5Info(string color, string data, string comment, bool setErr, double err1, double err0)
    {
        if (!Regex.IsMatch(data, "^[\\s\\-\\w.]+/[\\s\\-\\w.]+/[\\s\\-\\w./,]+$"))
        {
            // FindPic：data 是图片路径。
            if (!TryLoadBitmapBytes(data, out var bytes, out var w, out var h))
            {
                return null;
            }

            var arr = SplitWithSentinel(color, '/');
            var n = arr.Length;
            var picRaw = new byte[w * h * 4 + n * 8];
            Buffer.BlockCopy(bytes, 0, picRaw, 0, w * h * 4);
            var offset = w * h * 4;
            var defaultColor = n > 0 ? arr[0].Trim('-') : string.Empty;
            for (var i = 0; i < n; i++)
            {
                var item = arr[i];
                var parts = (item.Trim('-') + "-" + defaultColor).Split('-', StringSplitOptions.None);
                var colorValue = ToRgb(parts[0], item.StartsWith("-", StringComparison.Ordinal));
                var variation = parts.Length > 1 && parts[1].Contains('.', StringComparison.Ordinal)
                    ? (uint)(i == 0 ? SimilarityToPicThreshold(parts[1]) : SimilarityToThreshold(parts[1]))
                    : ParseHex(parts.Length > 1 ? parts[1] : string.Empty) | 0x40000000u;
                WriteUInt32(picRaw, ref offset, colorValue);
                WriteUInt32(picRaw, ref offset, variation);
            }

            return new PicInfoData
            {
                RawData = picRaw,
                Width = w,
                Height = h,
                SetErr = setErr,
                Err1 = err1,
                Err0 = err0,
                Mode = 5,
                Color = 2,
                N = n,
                Comment = comment
            };
        }

        // MultiColor / Shape：data 是 x/y/color,color... 的点列表。
        var defaultAllowed = SplitWithSentinel(color, '/').FirstOrDefault()?.Trim('-') ?? string.Empty;
        var items = Regex.Replace(data, "(?i)\\s|0x", string.Empty).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (items.Length == 0)
        {
            return null;
        }

        var shape = items.Length > 1 && ((items[0].Split('/').ElementAtOrDefault(2)?.Length) == 1);
        var x1 = 0;
        var y1 = 0;
        var x2 = 0;
        var y2 = 0;
        for (var i = 0; i < items.Length; i++)
        {
            var parts = items[i].Split('/');
            var px = ParseInt(parts.ElementAtOrDefault(0));
            var py = ParseInt(parts.ElementAtOrDefault(1));
            if (i == 0)
            {
                x1 = x2 = px;
                y1 = y2 = py;
            }
            else
            {
                x1 = Math.Min(x1, px);
                x2 = Math.Max(x2, px);
                y1 = Math.Min(y1, py);
                y2 = Math.Max(y2, py);
            }
        }

        var raw = new byte[items.Length * 22 * 4];
        for (var i = 0; i < items.Length; i++)
        {
            var parts = items[i].Split('/');
            var px = ParseInt(parts.ElementAtOrDefault(0)) - x1;
            var py = ParseInt(parts.ElementAtOrDefault(1)) - y1;
            var baseOffset = i * 22 * 4;
            BinaryPrimitives.WriteUInt32LittleEndian(raw.AsSpan(baseOffset, 4), (uint)((py << 16) | px));
            var count = Math.Min(Math.Max(parts.Length - 2, 0), shape ? 1 : 10);
            BinaryPrimitives.WriteUInt32LittleEndian(raw.AsSpan(baseOffset + 4, 4), (uint)count);
            for (var j = 0; j < count; j++)
            {
                var token = parts[2 + j];
                var group = (token.Trim('-') + "-" + defaultAllowed).Split('-', StringSplitOptions.None);
                var c = ToRgb(group[0], token.StartsWith("-", StringComparison.Ordinal));
                var n = group.Length > 1 && group[1].Contains('.', StringComparison.Ordinal)
                    ? (uint)SimilarityToThreshold(group[1])
                    : ParseHex(group.Length > 1 ? group[1] : string.Empty) | 0x40000000u;
                BinaryPrimitives.WriteUInt32LittleEndian(raw.AsSpan(baseOffset + 8 + j * 8, 4), c);
                BinaryPrimitives.WriteUInt32LittleEndian(raw.AsSpan(baseOffset + 12 + j * 8, 4), n);
            }
        }

        return new PicInfoData
        {
            RawData = raw,
            Width = x2 - x1 + 1,
            Height = y2 - y1 + 1,
            SetErr = setErr,
            Err1 = err1,
            Err0 = err0,
            Mode = 5,
            Color = shape ? 1u : 0u,
            N = items.Length,
            Comment = comment
        };
    }

    /// <summary>
    /// 构建普通文字/二值图模式的信息。
    /// </summary>
    private PicInfoData? BuildTextModeInfo(int mode, string color, string data, string comment, bool setErr, double err1, double err0)
    {
        var parts = data.Split('.', 2);
        if (parts.Length != 2)
        {
            return null;
        }

        var width = ParseInt(parts[0]);
        var bits = Base64ToBit(parts[1]);
        if (width < 1 || bits.Length == 0 || bits.Length % width != 0)
        {
            return null;
        }

        var height = bits.Length / width;
        var colorParts = SplitWithSentinel(color, '/');
        var bitBytes = Encoding.ASCII.GetBytes(bits);

        if (mode == 1)
        {
            var raw = new byte[bitBytes.Length + colorParts.Length * 8];
            Buffer.BlockCopy(bitBytes, 0, raw, 0, bitBytes.Length);
            var offset = bitBytes.Length;
            foreach (var item in colorParts)
            {
                var sub = (item.Trim('-') + "-").Split('-', StringSplitOptions.None);
                var c = ToRgb(sub[0], item.StartsWith("-", StringComparison.Ordinal));
                var n = sub.Length > 1 && sub[1].Contains('.', StringComparison.Ordinal)
                    ? (uint)SimilarityToThreshold(sub[1])
                    : ParseHex(sub.Length > 1 ? sub[1] : string.Empty) | 0x40000000u;
                WriteUInt32(raw, ref offset, c);
                WriteUInt32(raw, ref offset, n);
            }

            return new PicInfoData
            {
                RawData = raw,
                Width = width,
                Height = height,
                SetErr = setErr,
                Err1 = err1,
                Err0 = err0,
                Mode = 1,
                Color = 0,
                N = colorParts.Length,
                Comment = comment
            };
        }

        if (mode == 4)
        {
            var sub = ((colorParts.FirstOrDefault() ?? string.Empty).Trim('-') + "-").Split('-', StringSplitOptions.None);
            var n = sub.Length > 1 ? SimilarityToThreshold(sub[1]) : 0;
            var c = ParseInt(sub[0]);
            var pos = c < 1 || c > width * height ? 0u : (uint)((((c - 1) / width) << 16) | ((c - 1) % width));
            return new PicInfoData
            {
                RawData = bitBytes,
                Width = width,
                Height = height,
                SetErr = setErr,
                Err1 = err1,
                Err0 = err0,
                Mode = 4,
                Color = pos,
                N = n,
                Comment = comment
            };
        }

        return new PicInfoData
        {
            RawData = bitBytes,
            Width = width,
            Height = height,
            SetErr = setErr,
            Err1 = err1,
            Err0 = err0,
            Mode = mode,
            Color = (uint)ParseInt(color),
            N = colorParts.Length,
            Comment = comment
        };
    }

    /// <summary>
    /// 按小端写入一个 `uint`，用于拼装模式缓冲区。
    /// </summary>
    private static void WriteUInt32(byte[] buffer, ref int offset, uint value)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(buffer.AsSpan(offset, 4), value);
        offset += 4;
    }

    /// <summary>
    /// 把颜色名称或十六进制颜色转成 `0xRRGGBB`，可附带“排除色”标记位。
    /// </summary>
    private uint ToRgb(string color, bool excluded = false)
    {
        var value = _namedColors.TryGetValue(color, out var named) ? named : color;
        value = value.Replace("0x", string.Empty, StringComparison.OrdinalIgnoreCase);
        return ParseHex(value) & 0xFFFFFFu | (excluded ? 0x1000000u : 0u);
    }

    /// <summary>
    /// 为了模仿原版 `Split(...), Pop()` 的行为，先补一个分隔符再拆分。
    /// </summary>
    private static string[] SplitWithSentinel(string value, char separator)
    {
        return (value + separator)
            .Split(separator, StringSplitOptions.None)
            .Where(v => v.Length > 0)
            .ToArray();
    }

    /// <summary>
    /// 把相似度（0~1）转换成颜色距离阈值。
    /// </summary>
    private static int SimilarityToThreshold(string value)
    {
        var x = ParseDouble(value);
        return x <= 0 || x > 1 ? 0 : (int)Math.Floor(4606d * 255d * 255d * (1 - x) * (1 - x));
    }

    /// <summary>
    /// FindPic 首个透明色规则使用的较简单阈值换算。
    /// </summary>
    private static int SimilarityToPicThreshold(string value)
    {
        var x = ParseDouble(value);
        return x <= 0 || x > 1 ? 0 : (int)Math.Floor(255d * (1 - x)) * 0x010101;
    }

    /// <summary>
    /// 兼容十进制/十六进制字符串到整数的转换。
    /// </summary>
    private static int ParseInt(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        value = value.Trim();
        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            return (int)Convert.ToUInt32(value[2..], 16);
        }

        return int.TryParse(value, NumberStyles.Integer, CultureInfo.InvariantCulture, out var result)
            ? result
            : 0;
    }

    /// <summary>
    /// 按不变文化解析浮点数。
    /// </summary>
    private static double ParseDouble(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        return double.TryParse(value.Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out var result)
            ? result
            : 0;
    }

    /// <summary>
    /// 十六进制字符串转 `uint`。
    /// </summary>
    private static uint ParseHex(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return 0;
        }

        value = value.Trim();
        if (value.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
        {
            value = value[2..];
        }

        return uint.TryParse(value, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out var result)
            ? result
            : 0;
    }

    /// <summary>
    /// 与原版一致的灰度换算公式。
    /// </summary>
    private static byte Gray(int r, int g, int b) => (byte)((r * 38 + g * 75 + b * 15) >> 7);

    /// <summary>
    /// 自动估算二值化阈值，逻辑沿用原版 FindText。
    /// </summary>
    private static int AutoThreshold(byte[] grays)
    {
        var histogram = new int[256];
        foreach (var gray in grays)
        {
            histogram[gray]++;
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

    /// <summary>
    /// 读取外部图片文件并转成 32bpp ARGB 原始字节，供 FindPic 使用。
    /// </summary>
    private bool TryLoadBitmapBytes(string file, out byte[] bytes, out int width, out int height)
    {
        bytes = [];
        width = 0;
        height = 0;
        if (!File.Exists(file))
        {
            return false;
        }

        using var source = new Bitmap(file);
        using var bitmap = new Bitmap(source.Width, source.Height, PixelFormat.Format32bppArgb);
        using (var g = Graphics.FromImage(bitmap))
        {
            g.DrawImage(source, 0, 0, source.Width, source.Height);
        }

        width = bitmap.Width;
        height = bitmap.Height;
        var rect = new Rectangle(0, 0, width, height);
        var data = bitmap.LockBits(rect, ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);
        try
        {
            bytes = new byte[width * height * 4];
            for (var y = 0; y < height; y++)
            {
                var srcRow = data.Stride >= 0
                    ? data.Scan0 + y * data.Stride
                    : data.Scan0 + (height - 1 - y) * (-data.Stride);
                Marshal.Copy(srcRow, bytes, y * width * 4, width * 4);
            }
        }
        finally
        {
            bitmap.UnlockBits(data);
        }

        return true;
    }

    /// <summary>
    /// 把两点坐标转换成标准矩形；若四个值全是 0，则表示全屏搜索。
    /// </summary>
    private static void CreateSearchRect(int x1, int y1, int x2, int y2, out int x, out int y, out int w, out int h)
    {
        if (x1 == 0 && y1 == 0 && x2 == 0 && y2 == 0)
        {
            x = -FullScreenRange;
            y = -FullScreenRange;
            w = FullScreenRange * 2;
            h = FullScreenRange * 2;
            return;
        }

        x = Math.Min(x1, x2);
        y = Math.Min(y1, y2);
        w = Math.Abs(x2 - x1) + 1;
        h = Math.Abs(y2 - y1) + 1;
    }

    /// <summary>
    /// 获取截图缓存。
    /// - `screenShot=true`：重新抓屏。
    /// - `screenShot=false`：复用上次截图。
    /// </summary>
    private ScreenBits GetBitsFromScreen(ref int x, ref int y, ref int w, ref int h, bool screenShot, out int zx, out int zy, out int zw, out int zh)
    {
        if (!screenShot && _bits.Scan0 != 0)
        {
            zx = _bits.Zx;
            zy = _bits.Zy;
            zw = _bits.Zw;
            zh = _bits.Zh;
            ClampToScreen(ref x, ref y, ref w, ref h, zx, zy, zw, zh);
            return _bits;
        }

        zx = NativeMethods.GetSystemMetrics(NativeMethods.SM_XVIRTUALSCREEN);
        zy = NativeMethods.GetSystemMetrics(NativeMethods.SM_YVIRTUALSCREEN);
        zw = NativeMethods.GetSystemMetrics(NativeMethods.SM_CXVIRTUALSCREEN);
        zh = NativeMethods.GetSystemMetrics(NativeMethods.SM_CYVIRTUALSCREEN);
        UpdateBits(_bits, zx, zy, zw, zh);
        ClampToScreen(ref x, ref y, ref w, ref h, zx, zy, zw, zh);

        if (!screenShot || w < 1 || h < 1 || _bits.HBM == 0)
        {
            return _bits;
        }

        var mdc = NativeMethods.CreateCompatibleDC(0);
        var obm = NativeMethods.SelectObject(mdc, _bits.HBM);
        var desktop = NativeMethods.GetDesktopWindow();
        var hdc = NativeMethods.GetWindowDC(desktop);
        try
        {
            NativeMethods.BitBlt(mdc, 0, 0, zw, zh, hdc, zx, zy, NativeMethods.SRCCOPY | _captureBlt);
        }
        finally
        {
            NativeMethods.ReleaseDC(desktop, hdc);
            NativeMethods.SelectObject(mdc, obm);
            NativeMethods.DeleteDC(mdc);
        }

        return _bits;
    }

    /// <summary>
    /// 把请求区域裁剪到虚拟屏幕范围内。
    /// </summary>
    private static void ClampToScreen(ref int x, ref int y, ref int w, ref int h, int zx, int zy, int zw, int zh)
    {
        w = Math.Min(x + w, zx + zw);
        x = Math.Max(x, zx);
        w -= x;
        h = Math.Min(y + h, zy + zh);
        y = Math.Max(y, zy);
        h -= y;
    }

    /// <summary>
    /// 根据当前虚拟屏幕大小更新截图位图缓存。
    /// </summary>
    private static void UpdateBits(ScreenBits bits, int zx, int zy, int zw, int zh)
    {
        if (zw > bits.OldZw || zh > bits.OldZh || bits.HBM == 0)
        {
            if (bits.HBM != 0)
            {
                NativeMethods.DeleteObject(bits.HBM);
            }

            bits.HBM = NativeMethods.CreateDIBSection32(zw, zh, out var scan0);
            bits.Scan0 = bits.HBM == 0 ? 0 : scan0;
            bits.Stride = zw * 4;
            bits.OldZw = zw;
            bits.OldZh = zh;
        }

        bits.Zx = zx;
        bits.Zy = zy;
        bits.Zw = zw;
        bits.Zh = zh;
    }

    /// <summary>
    /// 一次搜索所需的上下文数据。
    /// 主要是为了避免频繁分配临时数组，并把共享参数集中管理。
    /// </summary>
    private sealed class SearchContext
    {
        public required ScreenBits Bits { get; init; }
        public required int[] S1 { get; init; }
        public required int[] S0 { get; init; }
        public required int[] AllPos { get; init; }
        public required int[] Errors { get; init; }
        public int Sx { get; init; }
        public int Sy { get; init; }
        public int Sw { get; init; }
        public int Sh { get; init; }
        public int Zx { get; init; }
        public int Zy { get; init; }
        public int AllPosMax { get; init; }
        public double Err1 { get; set; }
        public double Err0 { get; set; }
        public double ZoomW { get; init; }
        public double ZoomH { get; init; }
    }
}
