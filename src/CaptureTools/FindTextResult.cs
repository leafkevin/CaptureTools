namespace CaptureTools;

/// <summary>
/// 单个命中结果。
/// 对应 AHK 中的：
/// <c>{1:X, 2:Y, 3:W, 4:H, x:CenterX, y:CenterY, id:Comment}</c>
/// </summary>
public sealed class FindTextResult
{
    /// <summary>
    /// 命中区域左上角 X。
    /// </summary>
    public int X1 { get; set; }

    /// <summary>
    /// 命中区域左上角 Y。
    /// </summary>
    public int Y1 { get; set; }

    /// <summary>
    /// 命中区域宽度。
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 命中区域高度。
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 命中区域中心 X。
    /// </summary>
    public int X => X1 + Width / 2;

    /// <summary>
    /// 命中区域中心 Y。
    /// </summary>
    public int Y => Y1 + Height / 2;

    /// <summary>
    /// 模板注释或识别出的字符内容。
    /// </summary>
    public string Id { get; set; } = "";

    public override string ToString() =>
        $"{{X1={X1}, Y1={Y1}, W={Width}, H={Height}, X={X}, Y={Y}, Id=\"{Id}\"}}";
}

/// <summary>
/// OCR 聚合结果。
/// 把多个 <see cref="FindTextResult"/> 按阅读顺序拼接后的输出。
/// </summary>
public sealed class OcrResult
{
    /// <summary>
    /// 最终拼出的文本。
    /// </summary>
    public string Text { get; set; } = "";

    /// <summary>
    /// 聚合区域左上角 X。
    /// </summary>
    public int X { get; set; }

    /// <summary>
    /// 聚合区域左上角 Y。
    /// </summary>
    public int Y { get; set; }

    /// <summary>
    /// 聚合区域宽度。
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 聚合区域高度。
    /// </summary>
    public int Height { get; set; }
}
