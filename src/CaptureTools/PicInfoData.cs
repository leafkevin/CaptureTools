namespace CaptureTools;

/// <summary>
/// 解析后的模板信息。
/// 对应 AHK <c>PicInfo()</c> 的结果：
/// <c>[text, w, h, seterr, err1, err0, mode, color, n, comment]</c>
/// </summary>
public sealed class PicInfoData
{
    /// <summary>
    /// 原始模板缓冲区。
    /// 可能是：
    /// - 01 位图字节串
    /// - 图片 BGRA 原始像素
    /// - 多点颜色描述缓冲区
    /// </summary>
    public byte[] RawData { get; set; } = [];

    /// <summary>
    /// 模板宽度。
    /// </summary>
    public int Width { get; set; }

    /// <summary>
    /// 模板高度。
    /// </summary>
    public int Height { get; set; }

    /// <summary>
    /// 是否显式设置了容错参数。
    /// </summary>
    public bool SetErr { get; set; }

    /// <summary>
    /// 前景容错比例。
    /// </summary>
    public double Err1 { get; set; }

    /// <summary>
    /// 背景容错比例。
    /// </summary>
    public double Err0 { get; set; }

    /// <summary>
    /// 搜索模式：
    /// 1 = Color
    /// 2 = GrayThreshold
    /// 3 = GrayDiff
    /// 4 = ColorPos
    /// 5 = FindPic / MultiColor / FindShape
    /// </summary>
    public int Mode { get; set; }

    /// <summary>
    /// 模式附加值，例如颜色、颜色坐标或模式标记。
    /// </summary>
    public uint Color { get; set; }

    /// <summary>
    /// 颜色规则数量或点数量。
    /// </summary>
    public int N { get; set; }

    /// <summary>
    /// 模板注释，对应模式字符串中的 <c>&lt;comment&gt;</c>。
    /// </summary>
    public string Comment { get; set; } = "";
}
