# `FindTextCore` 阅读指南

## 1. 这个类解决什么问题

`FindTextCore` 是 `FindText.ahk` 核心算法的 .NET 版。
它做的事情可以概括成 4 步：

1. 截图并缓存屏幕像素
2. 解析 AHK 风格的模板字符串
3. 在截图里执行匹配
4. 把命中结果整理成 `FindTextResult`

---

## 2. 最重要的调用链

### 主调用链

```text
FindText()
  -> CreateSearchRect()
  -> GetBitsFromScreen()
  -> ParsePicInfos()
      -> PicInfo()
          -> BuildMode5Info() / BuildTextModeInfo()
  -> CreateSearchContext()
  -> RunSearchPass()
      -> RunDirectSearch() / RunJoinedSearch()
          -> PicFind()
  -> 返回 List<FindTextResult>
```

### 截图转模板调用链

```text
GetTextFromScreen()
  -> CreateSearchRect()
  -> GetBitsFromScreen()
  -> CaptureGrayRegion()
  -> BuildBitString()
      -> BuildGrayDiffBitString() / BuildGrayThresholdBitString()
  -> TrimBinaryRows()
  -> BitToBase64()
```

---

## 3. 关键数据结构

### `ScreenBits`

截图缓存。
主要字段：

- `Scan0`：像素内存首地址
- `HBM`：GDI 位图句柄
- `Stride`：每行字节数
- `Zx/Zy/Zw/Zh`：当前缓存截图对应的虚拟屏幕范围

可以把它理解成“当前截图的底层像素缓冲区”。

### `PicInfoData`

一个模板字符串解析后的结果。
主要字段：

- `RawData`：模板原始数据
- `Width/Height`：模板尺寸
- `Mode`：搜索模式
- `Color`：附加参数
- `N`：颜色规则数量或点数量
- `Comment`：模板注释

### `FindTextResult`

单个命中结果：

- `X1/Y1`：左上角
- `Width/Height`：尺寸
- `X/Y`：中心点
- `Id`：模板注释

---

## 4. `FindText()` 怎么工作

### 第一步：确定搜索区域

`CreateSearchRect()` 把 `(x1, y1, x2, y2)` 转成标准矩形。
如果 4 个值都是 0，则表示全屏搜索。

### 第二步：截图

`GetBitsFromScreen()`：

- `screenShot=true`：重新抓屏
- `screenShot=false`：复用上次缓存

它最后返回 `ScreenBits`，后续所有匹配都直接基于这块像素内存完成。

### 第三步：解析模板

`ParsePicInfos()` 会把字符串里的多个 `|...` 模板拆开。
每个片段都交给 `PicInfo()` 解析。

`PicInfo()` 会识别模式：

- `mode=1`：颜色模式
- `mode=2`：灰度阈值模式
- `mode=3`：灰度差分模式
- `mode=4`：颜色位置模式
- `mode=5`：找图 / 多点颜色 / 形状模式

### 第四步：执行匹配

`RunSearchPass()` 决定走哪条路：

- `RunDirectSearch()`：逐模板直接匹配
- `RunJoinedSearch()`：按顺序组合多个模板匹配

它们最终都会调用 `PicFind()`。

---

## 5. `PicFind()` 为什么是核心

`PicFind()` 是整个算法最重要的函数，对应 AHK 版里的内嵌 C 代码。

它的工作分成几个阶段：

### 阶段 A：理解模板

根据 `mode` 不同，模板含义不同：

- 普通文字模式：模板是一串 `01` 位图
- 找图模式：模板是完整图片像素
- 多点颜色模式：模板是若干采样点和颜色规则

### 阶段 B：预处理截图

对 `mode 1/2/3`，先把截图区域转成中间图：

- `BuildColorMap()`
- `BuildGrayThresholdMap()`
- `BuildGrayDifferenceMap()`

这样后续匹配时就不用每次都重新算颜色或灰度。

### 阶段 C：扫描候选位置

`EnumeratePositions()` 按方向生成候选坐标。
支持：

- 从左到右 / 从上到下
- 从右到左
- 从下到上
- 按列扫描
- 从中心螺旋展开

### 阶段 D：逐点容错匹配

对每个候选位置：

- 检查前景点是否足够匹配
- 检查背景点是否足够匹配
- 超过容错就判失败
- 满足条件就记为命中

### 阶段 E：排序和去重

- `dir=0` 时按误差最小排序
- 再做重叠结果去重

---

## 6. `GetTextFromScreen()` 怎么工作

这个方法用于把屏幕区域转成模板字符串。

### 流程

1. 截图
2. `CaptureGrayRegion()` 把区域转成灰度数组
3. `BuildBitString()` 生成 01 位串
4. `TrimBinaryRows()` 自动裁边
5. `BitToBase64()` 编码成模板字符串

### 两种生成模式

#### 普通阈值模式

`BuildGrayThresholdBitString()`：

- 灰度 <= 阈值 记为 `1`
- 灰度 > 阈值 记为 `0`

#### 灰度差分模式

`BuildGrayDiffBitString()`：

- 不直接看像素亮不亮
- 而是看周围是否出现明显灰度差
- 更适合识别边缘/轮廓

---

## 7. 为什么有 `BitToBase64()` / `Base64ToBit()`

原版 FindText 并不直接把模板保存成图片，而是把二值图压缩成一个短字符串。

流程大致是：

```text
01位串 -> 自定义64进制字符串
```

这样模板可以直接写进代码里，便于复制、保存、共享。

注意这里不是标准 Base64，而是 AHK 原版自己的字符表，所以必须兼容。

---

## 8. `ImageSearch()` / `PixelSearch()` / `PixelCount()` 的关系

这 3 个方法本质上都是包装：

- `ImageSearch()`：把图片参数转成 `FindText` 能识别的模式字符串
- `PixelSearch()`：把颜色搜索转成单点多颜色模式
- `PixelCount()`：复用 `PicFind()` 的颜色匹配逻辑做计数

也就是说，真正的核心仍然是：

```text
PicInfo() + PicFind()
```

---

## 9. 如果要继续阅读，推荐顺序

建议按这个顺序看代码：

1. `FindTextResult.cs`
2. `ScreenBits.cs`
3. `PicInfoData.cs`
4. `FindTextCore.FindText()`
5. `FindTextCore.PicInfo()`
6. `FindTextCore.PicFind()`
7. `FindTextCore.GetTextFromScreen()`
8. `NativeMethods.cs`

这样会比直接从 `PicFind()` 开始轻松很多。

---

## 10. 当前版本和原版 AHK 的差异

当前核心版保留了匹配算法，但没有继续移植这些部分：

- GUI 捕图工具
- 热键和托盘
- 交互选区
- 一些调试/提示 UI
- 绑定窗口的完整高级行为

所以它更适合作为“算法库”来调用，而不是直接替代原脚本的全部工具界面。
