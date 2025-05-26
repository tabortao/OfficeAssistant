# Office Assistant
[English-machine translation](..\README.md)

Office Assistant 是一款基于 Avalonia UI 开发的跨平台办公助手，专注于 PDF 文档的批量处理。它提供直观易用的用户界面，以及高效稳定的文档处理功能，包括 PDF 文件的批量合并、智能拆分和页面替换等核心操作。该工具采用 Views、ViewModels架构设计，确保代码具备良好的可维护性和扩展性，为用户提供流畅的使用体验。

主要特点：
- 🚀 高效批处理：支持多个 PDF 文件的批量操作
- 🎯 功能专注：专注于 PDF 文档处理的核心需求
- 💻 跨平台支持：基于 Avalonia UI 开发，支持 Windows、Linux 和 macOS
- 🎨 现代界面：采用 Semi.Avalonia 主题，提供现代化的用户体验
- 🛠 可扩展性：基于 MVVM 架构，易于扩展新功能
- 📦 轻量级：单文件发布，无需安装，即点即用

![Image](https://github.com/user-attachments/assets/ea265412-f210-4573-a6f3-3bbc72f26248)

## 功能特性

- PDF 文件批量合并
  - 支持选择多个 PDF 文件
  - 文件列表可视化管理
  - 支持删除已选文件
  - 合并完成后状态提示

- PDF 文件批量压缩
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

- PDF批量替换
  - 支持批量选择源文件和替换文件
  - 源文件和替换文件数量必须一致
  - 可指定要替换的页码
  - 直接覆盖源文件进行替换

## 开发环境要求

- .NET 8.0 SDK
- Visual Studio 2022 或其他支持 .NET 的 IDE

## 使用的技术

- Avalonia UI 11.2.7
- PdfSharpCore

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

## 运行环境要求

- Windows 10/11
- .NET 8.0 运行时
  - 下载地址：[.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/zh-cn/download/dotnet/8.0)，下载自己系统的.NET 8 运行时即可。
![.NET8运行时](https://lei-1258171996.cos.ap-guangzhou.myqcloud.com/imgs/2024/202504142154045.jpg)

## 许可证

MIT License

## 贡献指南

欢迎提交 Issue 和 Pull Request。