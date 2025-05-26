using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;

namespace OfficeAssistant.ViewModels
{
    /// <summary>
    /// PDF压缩功能的视图模型
    /// </summary>
    public class PdfCompressViewModel : ViewModelBase
    {
        // 状态消息
        private string _statusMessage = "";
        // 压缩质量 (1-100)
        private bool _isLow;
        public bool IsLow
        {
            get => _isLow;
            set
            {
                if (_isLow != value)
                {
                    _isLow = value;
                    if (value) SetField(ref _compressionLevel, 30); // 低
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsMedium));
                    OnPropertyChanged(nameof(IsHigh));
                }
            }
        }
        private bool _isMedium;
        public bool IsMedium
        {
            get => _isMedium;
            set
            {
                if (_isMedium != value)
                {
                    _isMedium = value;
                    if (value) SetField(ref _compressionLevel, 75); // 中
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsLow));
                    OnPropertyChanged(nameof(IsHigh));
                }
            }
        }
        private bool _isHigh;
        
        // 添加压缩等级私有字段，默认值为中等压缩(75)
        private int _compressionLevel = 75;
        
        public bool IsHigh
        {
            get => _isHigh;
            set
            {
                if (_isHigh != value)
                {
                    _isHigh = value;
                    if (value) SetField(ref _compressionLevel, 100); // 高
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsLow));
                    OnPropertyChanged(nameof(IsMedium));
                }
            }
        }
        // 增加自定义压缩等级相关属性
        private bool _isCustom;
        public bool IsCustom
        {
            get => _isCustom;
            set
            {
                if (_isCustom != value)
                {
                    _isCustom = value;
                    if (value)
                    {
                        // 选中自定义时，压缩等级设为自定义值
                        CompressionLevel = CustomCompressionLevel;
                    }
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(IsLow));
                    OnPropertyChanged(nameof(IsMedium));
                    OnPropertyChanged(nameof(IsHigh));
                }
            }
        }
        private int _customCompressionLevel = 75;
        public int CustomCompressionLevel
        {
            get => _customCompressionLevel;
            set
            {
                if (_customCompressionLevel != value)
                {
                    // 限制范围在1-100
                    _customCompressionLevel = Math.Max(1, Math.Min(100, value));
                    if (IsCustom)
                    {
                        CompressionLevel = _customCompressionLevel;
                    }
                    OnPropertyChanged();
                }
            }
        }
        // 修改CompressionLevel属性，支持自定义同步
        public int CompressionLevel
        {
            get => _compressionLevel;
            set
            {
                if (_compressionLevel != value)
                {
                    _compressionLevel = value;
                    OnPropertyChanged();
                    IsLow = value == 30;
                    IsMedium = value == 75;
                    IsHigh = value == 100;
                    IsCustom = value != 30 && value != 75 && value != 100;
                    if (IsCustom)
                    {
                        CustomCompressionLevel = value;
                    }
                }
            }
        }
        // 输出目录
        private string _outputPath = "";
        // 处理进度
        private double _progress = 0;
        // 处理时间
        private string _processingTime = "";

        // 选中的PDF文件集合
        public ObservableCollection<string> SelectedFiles { get; } = [];

        // 状态消息属性
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        // 输出目录属性
        public string OutputPath
        {
            get => _outputPath;
            set => SetField(ref _outputPath, value);
        }

        // 处理进度属性
        public double Progress
        {
            get => _progress;
            set => SetField(ref _progress, value);
        }

        // 处理时间属性
        public string ProcessingTime
        {
            get => _processingTime;
            set => SetField(ref _processingTime, value);
        }

        /// <summary>
        /// 选择PDF文件
        /// </summary>
        public async Task SelectFiles()
        {
            var storageProvider = App.MainWindow.StorageProvider;
            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = true,
                FileTypeFilter = [new FilePickerFileType("PDF Files") { Patterns = ["*.pdf"] }]
            });

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var path = file.Path.LocalPath;
                    if (!SelectedFiles.Contains(path))
                    {
                        SelectedFiles.Add(path);
                    }
                }
            }
        }

        /// <summary>
        /// 选择输出目录
        /// </summary>
        public async Task SelectOutputPath()
        {
            var storageProvider = App.MainWindow.StorageProvider;
            var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "选择输出目录"
            });

            if (folders != null && folders.Count > 0)
            {
                OutputPath = folders[0].Path.LocalPath;
            }
        }

        /// <summary>
        /// 执行PDF压缩
        /// </summary>
        // 修改方法名并调整查找逻辑
        private static string FindGhostscriptExecutablePath()
        {
            // 1. 遍历PATH环境变量查找gswin64c.exe (Windows) 或 gs (Linux/macOS)
            var paths = (Environment.GetEnvironmentVariable("PATH") ?? "").Split(Path.PathSeparator);
            string[] exeNames;
            if (OperatingSystem.IsWindows())
            {
                exeNames = ["gswin64c.exe", "gswin32c.exe"]; // Windows 64位和32位
            }
            else if (OperatingSystem.IsLinux() || OperatingSystem.IsMacOS())
            {
                exeNames = ["gs"]; // Linux/macOS
            }
            else
            {
                throw new PlatformNotSupportedException("Unsupported OS for Ghostscript executable search.");
            }

            foreach (var dir in paths)
            {
                foreach (var exe in exeNames)
                {
                    try
                    {
                        var fullPath = Path.Combine(dir.Trim(), exe);
                        if (File.Exists(fullPath))
                            return fullPath;
                    }
                    catch { /* 忽略路径组合等错误 */ }
                }
            }

            // 2. 可选：检查常见安装目录 (以Windows为例，您可以根据需要添加其他平台的路径)
            string[] commonDirs = 
            [
                // 注意：这里的路径应该指向包含可执行文件的bin目录
                @"C:\Program Files\gs\gs10.05.1\bin", 
                @"D:\Program Files\gs\gs10.05.1\bin",
                // Linux/macOS 示例 (通常gs在PATH中)
                "/usr/bin/", 
                "/usr/local/bin/"
            ];

            foreach (var dir in commonDirs)
            {
                foreach (var exe in exeNames)
                {
                    try
                    {
                        var fullPath = Path.Combine(dir.Trim(), exe);
                        if (File.Exists(fullPath))
                            return fullPath;
                    }
                    catch { /* 忽略路径组合等错误 */ }
                }
            }

            throw new FileNotFoundException("未能自动找到Ghostscript可执行文件，请检查Ghostscript安装并确保其在系统PATH中。");
        }

        public async Task CompressFiles()
        {
            if (SelectedFiles.Count == 0) return;
            try
            {
                StatusMessage = "正在压缩中，请稍候……"; // 新增：压缩开始时提示
                var stopwatch = Stopwatch.StartNew();
                Progress = 0;
                ProcessingTime = "";
                
                string gsPath = FindGhostscriptExecutablePath(); // 查找Ghostscript可执行文件路径

                await Task.Run(() =>
                {
                    var outputDir = string.IsNullOrEmpty(OutputPath)
                        ? Path.GetDirectoryName(SelectedFiles[0]) ?? string.Empty
                        : OutputPath;

                    for (int i = 0; i < SelectedFiles.Count; i++)
                    {
                        var currentInputFile = SelectedFiles[i];
                        var currentOutputFile = Path.Combine(outputDir,
                            Path.GetFileNameWithoutExtension(currentInputFile) + "_compressed.pdf");

                        // 构建Ghostscript命令行参数
                        // 注意：参数根据您的需求调整，这里使用与之前类似的 /ebook 设置
                        // 您可以根据需要调整 -dCompatibilityLevel 和 -dPDFSETTINGS
                        var arguments = string.Format(
                            "-sDEVICE=pdfwrite -dNOPAUSE -dBATCH -dSAFER " +
                            "-dCompatibilityLevel=1.4 -dPDFSETTINGS=/ebook " +
                            "-sOutputFile=\"{0}\" \"{1}\"",
                            currentOutputFile, currentInputFile);

                        ProcessStartInfo startInfo = new()
                        {
                            FileName = gsPath,
                            Arguments = arguments,
                            CreateNoWindow = true,
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true
                        };

                        using (Process process = new()
                        { StartInfo = startInfo })
                        {
                            process.Start();
                            string output = process.StandardOutput.ReadToEnd(); // 可以选择记录输出
                            string error = process.StandardError.ReadToEnd();   // 记录错误信息
                            process.WaitForExit();

                            if (process.ExitCode != 0)
                            {
                                // 如果Ghostscript返回非0退出码，则认为压缩失败
                                throw new Exception($"Ghostscript处理失败 (退出码: {process.ExitCode})。输入文件: {currentInputFile}。错误信息: {error}");
                            }
                        }
                        Progress = (i + 1) * 100.0 / SelectedFiles.Count;
                    }
                });
                stopwatch.Stop();
                ProcessingTime = $"处理完成，耗时: {stopwatch.Elapsed.TotalSeconds:F2}秒";
                await ShowTemporaryMessage("PDF压缩完成！", message => StatusMessage = message);
            }
            catch (Exception ex)
            {
                await ShowTemporaryMessage($"压缩失败：{ex.Message}", message => StatusMessage = message);
            }
        }

        /// <summary>
        /// 从列表中移除文件
        /// </summary>
        public void RemoveFile(string file)
        {
            SelectedFiles.Remove(file);
        }

        /// <summary>
        /// 清除所有选中的文件
        /// </summary>
        public void ClearAllFiles()
        {
            SelectedFiles.Clear();
        }
        private int _compressionLevelIndex = 1; // 0=低, 1=中, 2=高
        public int CompressionLevelIndex
        {
            get => _compressionLevelIndex;
            set
            {
                if (SetField(ref _compressionLevelIndex, value))
                {
                    // 低=30，中=75，高=100，可根据实际需求调整
                    switch (value)
                    {
                        case 0: CompressionLevel = 30; break;
                        case 1: CompressionLevel = 75; break;
                        case 2: CompressionLevel = 100; break;
                    }
                }
            }
        }
    }
}
