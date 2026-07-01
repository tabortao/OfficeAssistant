# Office Assistant

Office Assistant 是一款基于 Avalonia UI 开发的跨平台办公助手，专注于 PDF 文档的批量处理。

## 项目概述

- **技术栈**: C# / .NET 10.0 / Avalonia UI 11.2.7
- **架构模式**: MVVM (Model-View-ViewModel)
- **目标平台**: Windows (主要), Linux, macOS
- **核心库**: PdfSharpCore (PDF 处理), Ghostscript (压缩/转图片)
- **命名空间**: `OfficeAssistant`
- **当前版本**: 2.0.1

## 常用命令

```bash
# 构建项目
dotnet build

# 运行应用
dotnet run

# 发布 Windows 版本（不含 .NET 运行时）
dotnet publish -c Release -r win-x64 --no-self-contained

# 发布为单文件可执行文件
dotnet publish -r win-x64 -c Release --self-contained false /p:PublishSingleFile=true

# 清理解决方案
dotnet clean
```

## 项目结构

```
OfficeAssistant/
├── OfficeAssistant.sln              # 解决方案文件
├── OfficeAssistant.iss              # InnoSetup 安装包脚本
├── OfficeAssistant/                 # 主项目目录
│   ├── OfficeAssistant.csproj       # 项目文件
│   ├── Program.cs                   # 应用入口
│   ├── App.axaml / App.axaml.cs     # 应用定义与生命周期
│   ├── ViewModels/                  # 视图模型层
│   │   ├── ViewModelBase.cs         # 基类 (INPC + SetField)
│   │   └── PDF/                     # PDF 功能 ViewModels
│   │       ├── PdfMergeViewModel.cs
│   │       ├── PdfSplitViewModel.cs
│   │       ├── PdfCompressViewModel.cs
│   │       ├── PdfReplaceViewModel.cs
│   │       ├── PdfDeleteViewModel.cs
│   │       ├── PdfInsertViewModel.cs
│   │       └── PdfImageViewModel.cs
│   ├── Views/                       # 视图层
│   │   ├── MainWindow.axaml(.cs)    # 主窗口 + 导航
│   │   └── PDF/                     # PDF 功能 Views
│   │       ├── PdfMergeView.axaml(.cs)
│   │       ├── PdfSplitView.axaml(.cs)
│   │       ├── PdfCompressView.axaml(.cs)
│   │       ├── PdfReplaceView.axaml(.cs)
│   │       ├── PdfDeleteView.axaml(.cs)
│   │       ├── PdfInsertView.axaml(.cs)
│   │       ├── PdfImageView.axaml(.cs)
│   │       └── OcrView.axaml(.cs)
│   ├── Helpers/                     # 工具类
│   │   └── GhostscriptHelper.cs     # Ghostscript 路径查找
│   ├── Converters/                  # 值转换器
│   │   └── ListBoxMaxHeightConverter.cs
│   └── Resources/                   # 图标与静态资源
├── scripts/                         # 打包脚本 (AppImage, Debian, macOS, ZIP)
├── docs/                            # 文档与参考资源
│   ├── Reference/ghostscript/       # 内置 Ghostscript 二进制
│   └── ...
└── .github/workflows/               # CI/CD 工作流
```

## 架构与编码规范

### MVVM 模式

- **ViewModelBase** 提供 `INotifyPropertyChanged` 实现，子类通过 `SetField(ref field, value)` 设置属性
- 临时消息使用 `ShowTemporaryMessage(message, setter, duration)` 方法
- ViewModel 命名: `{功能}ViewModel.cs`，位于 `ViewModels/PDF/` 子目录
- View 命名: `{功能}View.axaml` + 对应 code-behind，位于 `Views/PDF/` 子目录

### Code-Behind 事件处理

- Avalonia Views 使用 code-behind 事件处理模式（非纯命令绑定）
- 事件处理方法调用 ViewModel 中的 `async Task` 方法
- 示例模式:
  ```csharp
  private async void SelectFiles(object sender, RoutedEventArgs e)
  {
      await _viewModel.SelectFiles();
  }
  ```

### 文件选择

- 使用 `App.MainWindow.StorageProvider` 获取文件选择器
- 文件路径以 `string` 类型存储在 `ObservableCollection<string>` 中
- 通过 `file.Path.LocalPath` 获取本地路径字符串

### Ghostscript 集成

- `GhostscriptHelper.FindGhostscriptExecutablePath()` 负责查找 Ghostscript 可执行文件
- 查找优先级: 应用内 ghostscript 目录 → 应用根目录 → PATH → 常见安装目录
- 压缩和 PDF 转图片功能依赖 Ghostscript
- 通过 `Process.Start` 调用 Ghostscript

### 导航

- MainWindow 使用左侧 ListBox 导航 + 右侧 ContentArea 切换视图
- 视图切换通过设置 `IsVisible` 属性实现
- PDF 图标和 OCR 图标控制不同的导航栏可见性

## 代码风格

- **注释**: 使用中文 XML 文档注释 (`/// <summary>`)
- **字符串**: 中文硬编码在 UI 和消息中（无本地化框架）
- **文件操作**: PDF 处理使用 `PdfSharpCore`，路径使用 `System.IO.Path`
- **异步**: 耗时操作使用 `Task.Run` + `await`，UI 线程通过 `ShowTemporaryMessage` 更新状态
- **异常处理**: 每个操作都有 try-catch，错误通过 `StatusMessage` 显示

## 版本管理

- 版本号定义在 `OfficeAssistant.csproj` 的 `<Version>` 属性中
- 需要同步更新 `OfficeAssistant.iss` 中的 `#define MyAppVersion`
- 发布时修改 `AssemblyVersion` 和 `FileVersion`

## CI/CD

- GitHub Actions 工作流位于 `.github/workflows/`
- 支持 Windows、Linux、macOS 多平台构建
- 打包脚本位于 `scripts/` 目录
