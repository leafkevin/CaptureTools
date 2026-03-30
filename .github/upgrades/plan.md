# AHK `GUI` 字库工具迁移到 `.NET 10 / Windows Forms` 计划

## 目录
- [1. Executive Summary](#1-executive-summary)
- [2. Migration Strategy](#2-migration-strategy)
- [3. Detailed Dependency Analysis](#3-detailed-dependency-analysis)
- [4. Project-by-Project Plans](#4-project-by-project-plans)
- [5. Package Update Reference](#5-package-update-reference)
- [6. Breaking Changes Catalog](#6-breaking-changes-catalog)
- [7. Risk Management](#7-risk-management)
- [8. Testing & Validation Strategy](#8-testing--validation-strategy)
- [9. Complexity & Effort Assessment](#9-complexity--effort-assessment)
- [10. Source Control Strategy](#10-source-control-strategy)
- [11. Success Criteria](#11-success-criteria)

## 1. Executive Summary

### Scenario Description
本计划面向 `FindText.ahk` 中“`GUI` 字库工具”能力向当前 `WinFormsApp1` 的 `.NET 10 / Windows Forms` 版本迁移，范围聚焦于：主工具窗口、交互式抓图窗口、图像二值化与裁边、字库模式串生成、代码片段输出、截图保存/加载、取色与多点颜色记录，以及与现有 `FindTextCore` 的集成方式。

### Current Scope
- 解决方案项目数：`1`
- 目标框架：`net10.0-windows`
- 当前可复用核心：`FindTextCore.cs`、`NativeMethods.cs`、`ScreenBits.cs`
- 当前 UI 状态：`Form1` 仍为空壳窗口，尚未承载字库工具功能
- AHK 侧目标能力：主窗口 + 捕获窗口 + 交互选区 + 网格编辑 + 多模式二值化 + 导出/测试辅助

### Selected Strategy
**All-At-Once Strategy** - 对单项目中的全部目标能力做一次统一迁移规划，并以单一协调改造的方式落地；不把同一套 GUI 工具拆成互不兼容的长期并行版本。

### Rationale
- 仅有 `1` 个项目，不存在跨项目升级编排成本。
- 底层识别算法已经迁移到 C#，需要补的是上层工具化能力，而不是多项目框架升级。
- `GUI` 字库工具的多个子能力高度耦合：抓图、取色、二值化、裁边、导出、测试、预览共享同一截图与编辑状态。
- 若按零散功能逐块长期分叉实现，容易造成 UI、状态模型和输出格式不一致。

### Complexity Classification
**Complex（单项目高交互复杂度）**

虽然项目数量很小，但复杂度仍高，原因如下：
- 存在多个高风险交互域：覆盖层选区、热键、悬浮提示、绑定窗口、后台截图、像素级编辑。
- AHK 原工具是“复合式桌面工具”，不是普通表单界面。
- 需要保持与 AHK 模式串、注释格式、自定义 64 进制编码的兼容性。
- `Bind0`~`Bind4`、`GetRange()`、`GetRange2()`、`Gray2Two`、`GrayDiff2Two`、`Color2Two`、`ColorPos2Two`、`SplitAdd` 等功能之间存在状态联动。

### Critical Issues
- `Form1` 目前没有任何字库工具 UI。
- 交互式截图与选区工作流尚未迁移。
- 像素编辑与模板导出流程尚未在 C# 中建立。
- 窗口绑定、后台抓图、热键和覆盖层行为仍主要停留在 AHK。

### Recommended Planning Shape
本计划采用“**风险优先 + 逻辑分层**”的写法：
- 先定义整体迁移策略与内部依赖关系。
- 再按“宿主窗口 / 捕获与选区 / 图像编辑与模板生成 / 辅助能力”拆解单项目实施内容。
- 虽然执行策略是 `All-at-Once`，但文档仍按逻辑阶段组织，便于执行代理在一个统一改造中有序实施。

### Expected Planning Iterations
预计通过 `8` 次规划迭代完成：
- Phase 1：`3` 次（骨架、分类、总纲）
- Phase 2：`3` 次（依赖分析、迁移策略、项目/风险概览）
- Phase 3：`2` 次（详细迁移说明、收尾章节）

## 2. Migration Strategy

### Selected Approach

**All-At-Once Strategy** - 在同一个 `WinFormsApp1` 项目内，对 `GUI` 字库工具的全部目标能力进行一次统一规划和统一迁移，不长期维持 AHK GUI 与 C# GUI 并行演进。

### Why All-At-Once Is Appropriate Here

虽然本次不是传统的框架升级，但从代码结构和交互耦合看，仍适合使用 `All-at-Once`：

- 只有一个项目，没有多项目依赖升级压力。
- 现有 `FindTextCore` 已经提供核心算法，主要缺的是一整层工具化 UI；这层 UI 之间耦合很强。
- 如果先做“半套窗口 + 半套导出 + 半套抓图”，会形成长期中间态，既不完整，也不利于行为一致性。
- 最终交付物本质上应是一套可闭环使用的字库工具，而不是若干彼此割裂的局部界面。

### Execution Principle

本计划强调：

- **单项目统一改造**
- **单一状态模型**
- **单套导出格式**
- **无长期中间行为分叉**

也就是说，执行阶段应以“统一替换式”思路完成以下目标：
- 主窗口与捕获窗口同时建立
- 图像捕获、编辑、二值化、导出流程同时打通
- 与现有 `FindTextCore` 的调用边界一次性固定
- 热键、截图保存/加载、辅助预览纳入同一工具体验

### Implementation Timeline

#### Phase 0: Foundation Alignment
**Purpose**: 固定迁移边界与内部模块职责，避免执行阶段边做边改架构。

**Operations**:
- 明确主窗口、捕获窗口、覆盖层、截图服务、导出服务的职责边界
- 明确哪些 AHK 行为需要完全兼容，哪些允许按 `WinForms` 习惯重构
- 明确 `FindTextCore` 与 GUI 层之间的公开接口边界

**Deliverables**:
- 可执行的模块分工
- 一致的输出兼容目标
- 明确的功能范围

#### Phase 1: Atomic GUI Tool Migration
**Purpose**: 在单次协调改造中建立完整 GUI 工具主链路。

**Operations**:
- 建立主窗口和捕获窗口
- 建立截图、预览、选区、范围提示、像素网格和编辑状态
- 实现灰度阈值、灰度差、颜色列表、颜色位置、多点颜色/形状模式
- 实现裁边、自动裁边、人工修改黑白点
- 实现模板串生成、ASCII 预览、代码片段输出、剪贴板/文件辅助

**Deliverables**:
- 工具可完成“截图 -> 编辑 -> 导出”闭环
- 输出模式串与现有 `FindTextCore` 兼容

#### Phase 2: Advanced Behavior Completion
**Purpose**: 补齐与原 AHK 工具体验最接近的高级能力。

**Operations**:
- 热键截图
- 绑定窗口和后台抓图模式
- 保存/加载截图资产
- 偏移计算、测试入口、结果高亮辅助

**Deliverables**:
- 高级桌面交互能力可用
- 工具体验接近 AHK 原版

#### Phase 3: Integrated Validation
**Purpose**: 用统一验证确认工具不是“能显示”，而是“能生产可用模板”。

**Operations**:
- 用典型截图样本验证二值化和导出正确性
- 验证输出模板可被 `FindTextCore` 正常回读/测试
- 验证窗口绑定、坐标换算和高 DPI 行为

**Deliverables**:
- 生成的模板可用于查找
- 工具关键交互稳定

### Parallel vs Sequential Decisions

在执行阶段，以下内容应视为**同一原子改造的一部分**，不建议长期拆开：

- 主窗口与捕获窗口框架
- 统一编辑状态模型
- 预览区与 `71 x 25` 网格同步机制
- 二值化模式与导出格式

以下内容可以在统一改造内按逻辑顺序实现，但最终仍应在同一轮迁移中交付：

- 先完成基本截图/网格/导出闭环
- 再补齐热键、绑定窗口、测试辅助和目录管理

### Strategy-Specific Considerations

对于本次 `All-at-Once` 迁移，需要特别遵守以下原则：

1. **不要把 GUI 工具长期拆成两套实现**  
   不建议出现“主窗口先是临时版、捕获窗口以后再说”的长期中间态。

2. **不要让窗体直接持有过多底层细节**  
   尽量通过服务/状态对象连接 `Form` 与 `FindTextCore`，避免窗体代码直接耦合 GDI 细节。

3. **一次性固定导出兼容格式**  
   `|<comment>...$...`、自定义 64 进制编码、`ASCII` 预览格式都应保持稳定，避免后续重新回填数据转换。

4. **优先保持行为闭环，而不是控件数量对等**  
   执行代理应优先保证 `Capture`、`CaptureS`、`Gray2Two`、`GrayDiff2Two`、`Color2Two`、`OK`、`SplitAdd` 这类主链路可用，再追求视觉细节完全一致。

## 3. Detailed Dependency Analysis

### Solution-Level Dependency Graph

当前解决方案只有一个项目：

```text
WinFormsApp1
```

不存在项目到项目的引用链、循环依赖或分层发布问题，因此“依赖顺序”的核心不在项目图，而在**单项目内部模块依赖**。

### Internal Feature Dependency Graph

```text
Program.cs
  -> 主宿主窗体 / 主工具窗口
       -> 捕获窗口
       -> 覆盖层选区与范围提示
       -> 代码输出 / 预览 / 剪贴板辅助
       -> 热键与截图入口

FindTextCore.cs
  -> 截图缓存
  -> 模式串生成
  -> 取色 / 写色
  -> 图片 / 颜色 / 文本匹配

NativeMethods.cs
  -> GDI 位图
  -> PrintWindow / GetDCEx / ClientToScreen / DrawIconEx

ScreenBits.cs
  -> 截图缓冲模型
```

### Functional Dependency Order

虽然最终执行采用 `All-at-Once` 方式，但功能上存在明确的底层依赖顺序：

1. **宿主窗口与状态容器**  
   需要先定义主窗口、捕获窗口、共享状态对象和命令入口，否则后续截图、编辑、导出逻辑无稳定承载点。

2. **截图与屏幕坐标基础设施**  
   包括截图缓存复用、屏幕/客户区坐标换算、位图裁切、图片预览载入、截图保存。这些能力是选区、取色、二值化和导出的共同基础。

3. **覆盖层选区与范围提示**  
   `GetRange()`、`GetRange2()`、`RangeTip()`、`MouseTip()` 对应的行为必须建立在截图和坐标基础设施之上。

4. **捕获窗口的像素网格与图片预览**  
   `71 x 25` 编辑区、滚动预览、选择框、保存图列表共享截图数据和位图复制能力。

5. **图像转换与编辑状态机**  
   `Gray2Two`、`GrayDiff2Two`、`Color2Two`、`ColorPos2Two`、裁边、自动裁边、手工修改像素、多点取色都依赖统一的编辑状态。

6. **模板生成与导出**  
   `OK`、`SplitAdd`、`AllAdd`、`ASCII` 预览、代码片段输出、剪贴板复制依赖前述编辑结果。

7. **高级能力**  
   热键、绑定窗口、后台截图、测试辅助、偏移计算建立在前面所有能力可用之后。

### Critical Path

本迁移的关键路径如下：

```text
主窗口/捕获窗口骨架
-> 统一状态模型
-> 截图与位图服务
-> 选区/覆盖层交互
-> 像素网格与预览同步
-> 二值化/裁边/编辑逻辑
-> 模板串与代码导出
-> 热键/绑定窗口/测试辅助补齐
```

### Coupling Hotspots

以下区域耦合最高，需要在计划中重点控制边界：

| Area | Current Source of Truth | Dependency Risk |
|---|---|---|
| 截图缓存 | `FindTextCore` / `ScreenBits` | 被多个 UI 行为共享，若边界不清会导致状态污染 |
| 编辑状态 | AHK 中的 `show`、`ascii`、`cors`、`Result` | 若直接照搬脚本式全局变量，后续维护会很差 |
| 选区交互 | `GetRange()` / `GetRange2()` / `RangeTip()` | 涉及 DPI、屏幕坐标、鼠标捕获、透明窗体 |
| 绑定窗口 | `BindWindow()` + `GetBitsFromScreen()` | 涉及不同抓图后端与窗口样式副作用 |
| 导出格式 | `bit2base64()` / 注释格式 / AHK 代码模板 | 需要与现有 `FindTextCore` 输出保持兼容 |

### Circular Dependencies

- **项目级循环依赖**：无
- **功能级潜在循环耦合**：存在风险，但应通过分层设计消解。尤其是“UI 控件事件 -> 截图状态 -> 编辑状态 -> UI 刷新”这一闭环，需要在规划中明确状态单向流转，避免窗体直接互相操纵内部缓存。

## 4. Project-by-Project Plans

### Project: `WinFormsApp1`

**Project Type**: `Windows Forms` desktop application  
**Target Framework**: `net10.0-windows`  
**Current Role**: 现有 `FindTextCore` 核心算法宿主，但 UI 仍为空壳  
**Primary Input Files**:
- `WinFormsApp1/Program.cs`
- `WinFormsApp1/Form1.cs`
- `WinFormsApp1/Form1.Designer.cs`
- `WinFormsApp1/FindTextCore.cs`
- `WinFormsApp1/NativeMethods.cs`
- `WinFormsApp1/ScreenBits.cs`
- `WinFormsApp1/FindText.ahk`

**Current State**:
- 已具备底层截图、模式串解析、模板生成与匹配能力。
- 没有主工具窗口、捕获窗口、选区覆盖层、热键、截图目录管理等 GUI 工具功能。
- `FindText.ahk` 是行为对照源，当前 `.NET` 工程不是完整替代品。

**Target State**:
- 单项目内提供完整的 `.NET` GUI 字库工具。
- 主窗口能够发起抓图、展示 ASCII/代码输出、复制文本、测试模板。
- 捕获窗口能够完成网格预览、图片预览、二值化、裁边、编辑、多点取色、导出。
- 关键输出格式与现有 `FindTextCore` / AHK 字库字符串保持兼容。

**Migration Focus Areas**:
1. 宿主 UI 重建
2. 捕获与选区交互
3. 像素级编辑状态模型
4. 模板与代码导出
5. 高级行为（热键、绑定窗口、后台抓图、辅助测试）

**Relative Risk**: High  
**Relative Complexity**: High

#### Current State vs Target State Mapping

| Current Asset | Current Role | Target Role |
|---|---|---|
| `Program.cs` | 启动默认空窗体 | 启动主字库工具宿主窗体 |
| `Form1.cs` / `Form1.Designer.cs` | 空白窗体 | 主工具窗口，承载抓图入口、代码预览、复制/测试/偏移辅助 |
| `FindTextCore.cs` | 核心算法库 | 作为 GUI 工具的底层引擎，不直接承担复杂窗体状态 |
| `NativeMethods.cs` | 底层 Win32 API 声明 | 为截图、覆盖层、坐标换算、窗口绑定和鼠标绘制提供基础能力 |
| `ScreenBits.cs` | 截图缓存模型 | 继续作为截图缓存和位图缓冲模型，必要时增强绑定窗口使用方式 |
| `FindText.ahk` | 原始行为定义与参考实现 | 迁移对照源，用于确认行为一致性与输出格式 |

#### Detailed Migration Steps

##### 1. Host Window Reconstruction

将 AHK `MakeMainWindow` 对应能力迁移到主宿主窗体，至少覆盖以下职责：

- 抓图入口：`Capture`、`CaptureS`
- 结果预览：`MyPic`、`scr`、`ClipText`
- 辅助操作：`Copy`、`Paste`、`Test`、`TestClip`
- 参数辅助：`GetRange`、`GetOffset`、`GetClipOffset`
- 热键设置：`NowHotkey`、`SetHotkey1`、`SetHotkey2`、`Apply`

**Reference AHK regions**:
- `FindText.ahk:2496-2567`
- `FindText.ahk:2807-2904`

##### 2. Capture Window Reconstruction

为 AHK `MakeCaptureWindow` 对应能力建立独立捕获窗口，至少包括：

- 顶部模式页签：`捕获` / `截图`
- `71 x 25` 网格编辑区
- 图片预览区与滚动滑块
- 图片列表、载入、保存、打开目录、清空截图
- 当前像素 `Gray / RGB` 读数
- 裁边按钮、`Auto` 自动裁边、`Modify` 人工修改黑白点
- 注释、确定、追加、取消、绑定窗口、保存裁剪图

**Reference AHK regions**:
- `FindText.ahk:2328-2489`
- `FindText.ahk:2673-2791`
- `FindText.ahk:3388-3460`

##### 3. Unified Editing State Model

需要在 C# 中显式建模 AHK 中散落的共享状态，避免直接照搬脚本全局变量。至少应统一管理以下状态：

- 当前截图原图数据
- 当前编辑区域矩形
- 当前网格可见/启用掩码（对应 AHK `show`）
- 当前二值化结果（对应 AHK `ascii`）
- 当前颜色源数据（对应 AHK `cors`）
- 当前裁边状态：`CutLeft`、`CutRight`、`CutUp`、`CutDown`
- 当前模式与参数：`Threshold`、`GrayDiff`、`ColorList`、`Similar1/2/3`
- 当前多点取色结果（对应 AHK `Result`）

##### 4. Capture and Selection Services

迁移以下 AHK 桌面交互行为：

- `GetRange()`：固定宽高、支持方向键微调的范围选取
- `GetRange2()`：覆盖层拖拽选区
- `RangeTip()`：高亮边框提示
- `MouseTip()`：结果位置高亮
- `ShowScreenShot()` / `ShowPic()`：调试/预览用悬浮显示

**Reference AHK regions**:
- `FindText.ahk:1502-1667`
- `FindText.ahk:1742-1790`（显示相关行为）

##### 5. Image Transformation and Binary Conversion

将以下 AHK 模式在 C# GUI 层打通：

- `Gray2Two`
- `GrayDiff2Two`
- `Color2Two`
- `ColorPos2Two`
- `Auto`
- 手工像素翻转与掩码显示

执行阶段应优先复用现有 `FindTextCore` 中已经存在的能力或公式，避免重复发明算法。

**Reference AHK regions**:
- `FindText.ahk:3024-3245`

##### 6. Template Output and Code Generation

输出层需要支持：

- 生成与 AHK 兼容的 `|<comment>...$...` 字符串
- `ASCII` 预览
- `OK`：生成全新代码片段
- `SplitAdd`：按列拆分输出多个字符模板
- `AllAdd`：整体追加模板
- `Update`：从人工编辑后的 ASCII 文本回写模式串

**Reference AHK regions**:
- `FindText.ahk:3246-3363`
- `FindText.ahk:3490-3522`（主窗口编辑回写相关行为）

##### 7. Advanced Desktop Behaviors

以下高级能力属于本迁移范围，但在执行中可作为闭环完成后的补齐部分：

- 热键截图
- 绑定窗口 `Bind0` ~ `Bind4`
- 后台抓图模式与透明样式恢复
- 截图目录维护
- 剪贴板图像保存
- 偏移计算与模板测试

**Reference AHK regions**:
- `FindText.ahk:1171-1203`
- `FindText.ahk:1683-1738`
- `FindText.ahk:2891-2923`

#### Validation Checklist for `WinFormsApp1`

- [ ] 主窗口可打开并展示主要命令入口
- [ ] 捕获窗口可打开并展示网格编辑区与图片预览区
- [ ] 可从屏幕或截图中选择区域进入编辑流程
- [ ] 灰度阈值模式可生成可用模板
- [ ] 灰度差模式可生成可用模板
- [ ] 颜色列表模式可生成可用模板
- [ ] 颜色位置模式可生成可用模板
- [ ] 多点找色/找形状模式可记录颜色点并导出
- [ ] 模板导出后可由现有 `FindTextCore` 测试使用
- [ ] 图片保存/加载、剪贴板、ASCII 预览可用
- [ ] 热键与绑定窗口行为达到计划要求范围

## 5. Package Update Reference

### Package Update Summary

本次 assessment 没有识别到需要为本迁移额外引入或升级的 NuGet 包版本信息；当前重点是**在现有 `.NET 10 + Windows Forms` 工程内补建桌面工具能力**，而不是包升级。

### Current Known Dependency Posture

| Scope | Status | Notes |
|---|---|---|
| .NET runtime | Already on `net10.0-windows` | 当前不是版本升级任务 |
| `Windows Forms` | In use | 继续作为 GUI 宿主技术 |
| `System.Drawing` / GDI | In use | 当前截图和位图处理链路依赖该路线 |
| Win32 P/Invoke | In use | `NativeMethods.cs` 已含部分所需 API |

### Planning Note

执行阶段如果发现以下能力必须新增依赖，应先验证是否真的无法通过现有 `Windows Forms` + P/Invoke 实现：

- 全局热键封装
- 更稳定的图像编解码/剪贴板辅助
- 可视化控件增强

在没有明确必要之前，计划建议**优先复用现有框架与系统 API**，避免为了重建 AHK 工具而引入过多额外包依赖。

## 6. Breaking Changes Catalog

### Expected Breaking-Change Categories

本次不是传统 `.NET` 版本升级，因此“Breaking Changes”主要指**AHK 脚本行为迁移到 C# GUI 时的行为差异风险**。

| Category | Expected Difference | Impact Area |
|---|---|---|
| 输入模型差异 | AHK 的 `Hotkey`、`OnMessage`、全局脚本状态与 WinForms 事件模型不同 | 热键、快捷操作、焦点切换 |
| 窗口模型差异 | AHK `Gui()` 可轻量创建无边框透明窗体，WinForms 需要更明确处理窗体样式、激活与穿透 | 选区覆盖层、RangeTip、ToolTip |
| 绘图模型差异 | AHK 直接对控件句柄和位图句柄做操作，C# 需要更稳妥的资源释放与 UI 刷新机制 | 网格显示、图片预览、位图复制 |
| 状态模型差异 | AHK 使用大量共享脚本变量，C# 更适合显式对象状态 | 捕获窗口联动、导出逻辑 |
| 线程/消息时序差异 | AHK 中 `Sleep` + 消息回调的交互方式与 WinForms 消息循环不同 | 拖拽、预览刷新、热键响应 |
| 坐标系差异 | 屏幕、窗口、客户区和高 DPI 的换算在 WinForms 中更容易出偏移 | 选区、结果高亮、绑定窗口 |

### High-Attention Behavior Parity Items

以下行为需要在执行阶段特别对照 AHK：

1. `GetRange()` 的两次点击/方向键微调体验
2. `GetRange2()` 的覆盖层拖拽与剪贴板输出
3. `Gray2Two` 自动阈值计算结果
4. `GrayDiff2Two` 的边缘识别结果
5. `Color2Two` / `ColorPos2Two` 的颜色距离判定
6. `SplitAdd` 的拆分规则
7. `Update` 对 ASCII 编辑结果的回写逻辑
8. `Bind0` ~ `Bind4` 的截图差异

### Non-Goals Disguised as Breaking Changes

以下项不应误判为必须完全复刻的“破坏性差异”：

- 控件布局像素级完全一致
- 提示文本的字体、大小、位置完全一致
- AHK 内部全局变量命名或事件分发方式在 C# 中保持原样

计划要求的是：**工作流、输出格式、关键交互效果一致**，而不是脚本实现细节逐行一致。

## 7. Risk Management

### High-Risk Changes Table

| Area | Risk Level | Description | Mitigation |
|---|---|---|---|
| 覆盖层选区 | High | 透明窗体、坐标换算、鼠标拖拽、高 DPI 下容易出现偏移 | 先固定坐标系与屏幕边界规则，再接入 UI 事件 |
| 捕获窗口网格编辑 | High | `71 x 25` 网格、滚动预览、原图/二值图同步逻辑复杂 | 使用统一编辑状态对象，避免控件直接互改底层缓存 |
| 导出格式兼容 | High | 若模式串、注释格式、编码规则偏离 AHK，将导致字库不可复用 | 以现有 `FindTextCore.BitToBase64()` / `Base64ToBit()` 为唯一编码基准 |
| 绑定窗口与后台抓图 | High | `GetDCEx` / `PrintWindow` 在不同窗口样式下行为不稳定 | 将绑定窗口能力作为高级子系统，单独验证每种模式 |
| 热键与输入交互 | Medium | 全局热键、窗体焦点、覆盖层输入可能互相干扰 | 使用明确的消息入口和状态切换，避免多个窗口重复注册 |
| 图片保存/加载/临时目录 | Medium | 文件目录、剪贴板与位图句柄释放处理不当易造成资源泄露 | 统一封装位图生命周期与文件 I/O 规则 |

### Risk Priorities

本计划的风险控制优先级如下：

1. **先保证数据流正确**：截图 -> 编辑 -> 导出 -> 回读验证
2. **再保证交互正确**：选区、拖拽、取色、预览、裁边
3. **最后补齐高级行为**：热键、绑定窗口、后台抓图、测试辅助

### Contingency Planning

- 如果绑定窗口模式在不同目标窗口下行为不一致，执行阶段应允许先交付基础前台抓图闭环，再补充/限制后台模式说明。
- 如果透明覆盖层在高 DPI 多屏环境下存在偏移，应优先保证单屏与主显示器行为正确，再扩展复杂场景。
- 如果 AHK 原始控件布局不适合 `WinForms`，允许重构视觉布局，但不允许破坏主工作流与导出格式兼容性。

### Rollback Considerations

- 当前工作区不是 Git 仓库，执行前应优先建立版本控制或外部备份。
- 对于执行代理，回退粒度应按“单次统一改造”控制，而不是多个相互交织的小碎片修改。

## 8. Testing & Validation Strategy

### Validation Philosophy

本迁移的测试重点不是“窗口能打开”，而是确认 `.NET` 工具可以稳定地产出**可被 `FindTextCore` 使用的模板**。

### Multi-Level Validation

#### 1. Foundation Validation

- [ ] 主窗口启动正常
- [ ] 捕获窗口可显示和关闭
- [ ] 图片预览可载入截图或外部图片
- [ ] `71 x 25` 网格区可显示并响应滚动/选择
- [ ] 位图资源在窗口切换和关闭时不泄露

#### 2. Interaction Validation

- [ ] `GetRange()` 可选取固定宽高范围
- [ ] `GetRange2()` 可拖拽任意区域
- [ ] `RangeTip()` 可正确显示与关闭
- [ ] 点击网格像素可更新当前颜色信息
- [ ] 右键拖动图片预览时滚动行为正确

#### 3. Binary Conversion Validation

针对每一种模式至少准备一组典型样本：

- [ ] 灰度阈值样本
- [ ] 灰度差样本
- [ ] 颜色列表样本
- [ ] 颜色位置样本
- [ ] 多点找色样本
- [ ] 找形状样本

每组样本至少验证：
- [ ] 可完成二值化或取色记录
- [ ] 可生成模式串
- [ ] 模式串可回显为正确 `ASCII`
- [ ] 模式串可被 `FindTextCore.FindText()` 使用

#### 4. Export Validation

- [ ] `OK` 生成的代码片段格式正确
- [ ] `SplitAdd` 生成多个字符模板时不破坏注释和编码
- [ ] `AllAdd` 追加逻辑不会污染已有文本
- [ ] `Update` 可把 ASCII 编辑内容回写成模式串
- [ ] 剪贴板复制内容与界面显示一致

#### 5. Advanced Behavior Validation

- [ ] 热键截图能落盘到预期目录
- [ ] 保存图片与加载图片流程可闭环
- [ ] 绑定窗口模式至少在目标支持范围内行为稳定
- [ ] 偏移计算结果可用于生成点击辅助代码
- [ ] 测试入口能够对导出的模板执行一次真实匹配验证

### Recommended Test Assets

执行阶段应准备以下资产用于回归：

- 单字样本（高对比度）
- 多字符样本（间距变化）
- 低对比度文字样本
- 带边缘轮廓样本（用于灰度差）
- 颜色点阵样本（用于多点找色）
- 一张用于 `FindPic`/图片模式的稳定截图

### Validation Exit Criteria

只有当以下条件同时成立，才可视为工具迁移完成：

- 主链路 `Capture/CaptureS -> 编辑 -> 导出 -> 测试` 闭环可用
- 导出字符串与现有核心兼容
- 关键高级能力达到计划承诺范围
- 不再依赖 AHK GUI 才能完成字库生成工作流

## 9. Complexity & Effort Assessment

### Complexity Summary

| Scope | Complexity | Reason |
|---|---|---|
| 主窗口重建 | Medium | 控件多但行为清晰，主要是命令组织与文本展示 |
| 捕获窗口重建 | High | 控件数量多、状态联动强、需要同步原图/编辑图/网格 |
| 覆盖层选区与提示 | High | 涉及透明窗体、全局坐标、鼠标拖拽、高 DPI |
| 二值化与裁边编辑 | High | 需要从 AHK 脚本状态模型迁移到可维护的 C# 状态对象 |
| 模板导出与代码拼装 | Medium | 逻辑清晰，但必须保持输出兼容 |
| 热键与绑定窗口 | High | Win32 细节多，且对最终体验影响大 |

### Phase Complexity

| Phase | Complexity | Notes |
|---|---|---|
| Phase 0: Foundation Alignment | Medium | 主要是边界与模型设计 |
| Phase 1: Atomic GUI Tool Migration | High | 本次迁移的主工作量集中在此 |
| Phase 2: Advanced Behavior Completion | High | 风险集中在 Win32 与输入交互 |
| Phase 3: Integrated Validation | Medium | 验证面广，但建立在前面功能闭环之上 |

### Resource Requirements

- **WinForms / Windows desktop**：High
- **Win32 / GDI / PInvoke**：High
- **图像处理与像素级调试**：High
- **现有 `FindTextCore` 理解**：Medium
- **AHK 行为对照理解**：High

## 10. Source Control Strategy

### Current Repository State

assessment 显示当前工作区**不是 Git 仓库**。这意味着执行阶段默认没有分支、提交点和标准回滚机制。

### Recommended Source Control Approach

尽管当前不是 Git 仓库，本计划仍建议执行阶段优先采用以下策略：

1. **如果允许初始化 Git**  
   - 在执行前为当前工作区建立 Git 仓库
   - 使用单一升级/迁移分支承载全部 GUI 工具改造
   - 采用 `All-at-Once` 的单次统一提交策略

2. **如果不能初始化 Git**  
   - 至少在执行前保留完整工作区副本
   - 在关键节点保留人工快照（例如压缩包或副本目录）
   - 避免在无回滚机制前提下进行多轮不可追踪的试错修改

### Branching Strategy

若执行阶段启用 Git，建议：

- Source branch：当前默认工作树基线
- Upgrade branch：单一功能分支，例如 `feature/findtext-gui-tool`
- Merge approach：完成整体闭环后一次性合并

### Commit Strategy

本计划遵循 `All-at-Once` 思路，推荐：

- **优先单次统一提交**：在主链路完整可用后提交一次集中改造
- 如必须拆分提交，也应按逻辑边界拆分，而不是按零碎控件改动拆分

建议的逻辑提交边界（仅在无法单提交时使用）：
- 窗体与状态模型
- 截图/覆盖层/预览基础设施
- 二值化与导出主链路
- 热键/绑定窗口/辅助能力

### Review Strategy

代码审查时应重点检查：

- `FindTextCore` 是否仍保持“核心算法层”边界
- 窗体代码是否过度耦合底层 GDI/Win32 细节
- 导出格式是否保持 AHK 兼容
- 位图句柄、DC、剪贴板对象是否正确释放
- 高 DPI / 多屏 / 客户区坐标换算是否有明显偏移风险

## 11. Success Criteria

### Technical Criteria

- `WinFormsApp1` 中存在可用的主工具窗口与捕获窗口
- `.NET` GUI 可独立完成字库生成，不再依赖 AHK GUI
- 可完成 `Capture` 与 `CaptureS` 两条主工作流
- 可完成以下至少核心模式：
  - `Gray2Two`
  - `GrayDiff2Two`
  - `Color2Two`
  - `ColorPos2Two`
  - `MultiColor`
  - `FindShape`
- 导出的模式串可被现有 `FindTextCore` 正常读取与使用
- ASCII 预览、复制、保存/加载图片、偏移辅助等关键辅助功能可用

### Quality Criteria

- 导出格式保持与 AHK 约定兼容：`|<comment>...$...`
- 自定义 64 进制编码与现有核心保持一致
- 捕获、预览、编辑、导出之间的状态流转清晰稳定
- 关键桌面资源（位图、DC、透明窗体、剪贴板对象）无明显泄露
- 主工作流在常见单屏 Windows 场景下稳定

### Strategy-Specific Criteria

作为 `All-at-Once` 迁移，本次完成标准还包括：

- 不保留长期并行的“临时 GUI 工具版本”
- 不把核心工作流拆成多个互不兼容的半成品入口
- 在单项目内一次性完成宿主窗体、捕获窗体、编辑状态和导出格式统一

### Process Criteria

- 迁移遵循本计划定义的内部依赖顺序
- 高风险项（覆盖层、绑定窗口、坐标换算、导出格式）已被专门验证
- 若启用版本控制，改造过程可追溯且具备回滚点
- 执行阶段未破坏现有 `FindTextCore` 作为底层算法库的职责定位

### Definition of Done

当且仅当下面全部成立时，本计划对应的迁移视为完成：

1. 用户可以在 `.NET 10 / Windows Forms` 工具中完成一次完整的字库制作流程；
2. 该流程生成的模板可被当前 C# 核心正常用于识别；
3. 核心 GUI 能力不再依赖 AHK 原始脚本窗口；
4. 高风险交互行为已在计划承诺范围内达到可用状态。
