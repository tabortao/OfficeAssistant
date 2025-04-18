# Office Assistant

[中文介绍](docs\README-zh.md)

Office Assistant is a cross-platform office tool developed with Avalonia UI, focusing on batch processing of PDF documents. It provides an intuitive user interface and efficient, stable document processing functions, including batch merging, intelligent splitting, and page replacement of PDF files. The tool is designed with MVVM architecture, ensuring good maintainability and extensibility of the code while providing a smooth user experience.

Key Features:
- 🚀 Efficient Batch Processing: Support batch operations for multiple PDF files
- 🎯 Focused Functionality: Concentrate on core PDF document processing needs
- 💻 Cross-platform Support: Developed with Avalonia UI, supporting Windows, Linux, and macOS
- 🎨 Modern Interface: Using Semi.Avalonia theme for a modern user experience
- 🛠 Extensibility: Based on MVVM architecture, easy to extend new features
- 📦 Lightweight: Single-file deployment, no installation required, ready to use

![Image](https://github.com/user-attachments/assets/ea265412-f210-4573-a6f3-3bbc72f26248)

## Features

- PDF File Batch Merge
  - Support selecting multiple PDF files
  - Visual management of file list
  - Support deleting selected files
  - Status notification after merging

- PDF File Batch Split
  - Support selecting one or multiple PDF files
  - Support splitting each page into separate PDFs
  - Support splitting by page range (e.g., 1-3,5,7-9)
  - Support custom output directory
  - Status notification after splitting

- PDF Batch Replace
  - Support batch selection of source and replacement files
  - Source and replacement files must be equal in number
  - Can specify pages to replace
  - Direct replacement of source files

## Development Requirements

- .NET 8.0 SDK
- Visual Studio 2022 or other .NET-supporting IDEs

## Technologies Used

- Avalonia UI 11.2.7
- PdfSharpCore

## Quick Start

1. Clone repository
```bash
git clone https://github.com/yaoleistable/OfficeAssistant.git
```

2. Enter project directory
```bash
cd OfficeAssistant
```

3. Build project
```bash
# dotnet clean # Clean solution
dotnet build
# dotnet build -c Release # Build project
```

4. Run application
```bash
dotnet run
```

5. Publish application
```bash
# Publish Windows version without .NET runtime
dotnet publish -c Release -r win-x64 --no-self-contained
# Package application as a single executable file (.exe)
dotnet publish -r win-x64 -c Release --self-contained false /p:PublishSingleFile=true
```

## Usage Instructions

### PDF Batch Merge
1. Launch the application
2. Select "PDF Merge" in the left navigation bar
3. Click "Select PDF Files" button to choose PDFs for merging
4. View and manage selected files in the file list
5. Click "Merge PDF" button to choose save location and complete merging

### PDF Batch Split
1. Select "PDF Split" in the left navigation bar
2. Click "Select PDF Files" button to choose PDFs for splitting
3. Choose split mode:
   - Split each page into separate PDF: Save each page as an independent PDF file
   - Split by page range: Enter page range (e.g., 1-3,5,7-9)
4. Optional: Click "Select Output Directory" to set save location
   - If not set, a "Split Files" folder will be created in the original file directory
5. Click "Start Split" button to complete splitting

### PDF Batch Replace
1. Select "PDF Replace" in the left navigation bar
2. Click "Select Source Files" button to choose PDFs to be replaced
3. Click "Select Replacement Files" button to choose PDFs for replacement
4. Ensure source and replacement files are equal in number
5. Optional: Enter pages to replace (e.g., 1,3,5)
6. Click "Start Replace" button to complete replacement

## Runtime Requirements

- Windows 10/11
- .NET 8.0 Runtime
  - Download: [.NET 8.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0), download the .NET 8 runtime for your system.
![.NET8 Runtime](https://lei-1258171996.cos.ap-guangzhou.myqcloud.com/imgs/2024/202504142154045.jpg)

## License

MIT License

## Contributing

Issues and Pull Requests are welcome.
