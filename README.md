# Office Assistant
[English-machine translation](..\README.md)

Office Assistant 是一款基于 Avalonia UI 开发的跨平台办公助手，专注于PDF文档的批量处理。它提供直观易用的用户界面，以及高效稳定的文档处理功能，包括PDF文件的批量合并、智能拆分、页面替换、PDF转图片等核心操作。该工具采用Views、ViewModels架构设计，确保代码具备良好的可维护性和扩展性，为用户提供流畅的使用体验。

主要特点：
- 🚀 高效批处理：支持多个 PDF 文件的批量操作
- 🎯 功能专注：专注于 PDF 文档处理的核心需求
- 💻 跨平台支持：基于 Avalonia UI 开发，支持 Windows、Linux 和 macOS
- 🛠 可扩展性：基于 MVVM 架构，易于扩展新功能
- 📦 轻量级：单文件发布，无需安装，即点即用

![Office Assistant](https://github.com/user-attachments/assets/917b4ad7-a6c5-4552-b339-3f22e2a47283)

## 功能特性

- PDF 文件批量合并
  - 支持选择多个 PDF 文件
  - 文件列表可视化管理
  - 支持删除已选文件
  - 合并完成后状态提示

- PDF 文件批量压缩（依赖Ghostscript）
  - 支持选择多个 PDF 文件
  - 提供低/中/高三种压缩等级预设
  - 支持自定义压缩等级(1-100)
  - 可指定输出目录
  - 压缩完成后状态提示

- PDF 文件批量拆分
  - 支持选择一个或多个 PDF 文件
  - 支持每页拆分为单独 PDF
  - 支持按页码范围拆分（如：1-3,5,7-9）
  - 支持自定义输出目录
  - 拆分完成后状态提示

- PDF 批量替换
  - 支持批量选择源文件和替换文件
  - 源文件和替换文件数量必须一致
  - 可指定要替换的页码
  - 直接覆盖源文件进行替换

- PDF 批量转图片（依赖Ghostscript）
  - 支持选择一个或多个 PDF 文件
  - 支持自定义页码范围（如：1-3,5,7-9 或全部页面）
  - 支持设置图片分辨率（72-1200 DPI）
  - 自动识别PDF总页数（基于PdfSharpCore）
  - 输出图片自动保存到“[PDF文件名]_Png”子文件夹
  - 实时显示进度和处理耗时

## 开发环境要求

- .NET 8.0 SDK
- Visual Studio Code 或其他支持 .NET 的 IDE

## 使用的技术

- Avalonia UI 11.2.7
- PdfSharpCore（PDF页数识别与处理）

## 快速开始

1. 克隆仓库
```bash
git clone https://github.com/yaoleistable/OfficeAssistant.git
```

2. 进入项目目录
```bash
cd OfficeAssistant
```

3. 构建项目
```bash
# dotnet clean # 清理解决方案
dotnet build
# dotnet build -c Release # 构建项目
```

4. 运行应用
```bash
dotnet run
```

5. 发布应用
```bash
# 发布 Windows 版本，不含.NET 运行时
dotnet publish -c Release -r win-x64 --no-self-contained
# 将应用程序打包为单个可执行文件（.exe）
dotnet publish -r win-x64 -c Release --self-contained false /p:PublishSingleFile=true
```

## 使用说明

### PDF 批量合并
1. 启动应用程序
2. 在左侧导航栏选择"PDF合并"
3. 点击"选择PDF文件"按钮选择需要合并的PDF文件
4. 在文件列表中查看和管理已选文件
5. 点击"合并PDF"按钮选择保存位置并完成合并

### PDF 批量压缩
1. 在左侧导航栏选择"PDF压缩"
2. 点击"选择PDF文件"按钮选择需要压缩的PDF文件
3. 选择压缩等级：
   - 低/中/高：使用预设的压缩等级
   - 自定义：手动输入1-100之间的压缩等级
4. 可选：点击"选择输出目录"设置压缩后文件的保存位置
   - 若未设置，将在原文件所在目录创建"压缩文件"文件夹
5. 点击"开始压缩"按钮开始处理
6. 处理过程中可查看进度条和状态信息

### PDF 批量拆分
1. 在左侧导航栏选择"PDF拆分"
2. 点击"选择PDF文件"按钮选择需要拆分的PDF文件
3. 选择拆分模式：
   - 每页拆分为单独PDF：将每一页保存为独立的PDF文件
   - 按页码范围拆分：输入页码范围（如：1-3,5,7-9）
4. 可选：点击"选择输出目录"设置保存位置
   - 若未设置，将在原文件所在目录创建"拆分文件"文件夹
5. 点击"开始拆分"按钮完成拆分

### PDF 批量替换
1. 在左侧导航栏选择"PDF替换"
2. 点击"选择源文件"按钮选择需要替换的PDF文件
3. 点击"选择替换文件"按钮选择用于替换的PDF文件
4. 确保源文件和替换文件数量一致
5. 可选：输入要替换的页码（如：1,3,5）
6. 点击"开始替换"按钮完成替换

### PDF 批量转图片
1. 在左侧导航栏选择"PDF转图片"
2. 点击"选择PDF文件"按钮选择需要转换的PDF文件
3. 可选：输入页码范围（如：1-3,5,7-9 或留空表示全部页面）
4. 可选：设置图片分辨率（72-1200 DPI）
5. 点击"开始转换"按钮，图片将自动保存到每个PDF同名的_Png子文件夹
6. 处理过程中可查看进度条和耗时信息

## 运行环境要求

- Windows 10/11
- .NET 8.0 运行时
  - 下载地址：[.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0)
- Ghostscript
  - 下载地址：[Ghostscript官网下载](https://ghostscript.com/releases/gsdnld.html)、[Ghostscript Github下载](https://github.com/ArtifexSoftware/ghostpdl-downloads/releases)
  - Mac电脑安装：`brew install ghostscript`
  - Linux电脑安装：`sudo apt install ghostscript`

## 许可证

MIT License

## 贡献指南

欢迎提交 Issue 和 Pull Request。