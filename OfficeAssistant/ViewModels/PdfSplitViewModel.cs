using Avalonia.Platform.Storage;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace OfficeAssistant.ViewModels
{
    /// <summary>
    /// PDF拆分功能的视图模型
    /// </summary>
    public class PdfSplitViewModel : ViewModelBase
    {
        // 状态消息，用于显示操作结果
        private string _statusMessage = "";
        // 页码范围，格式如："1-3,5,7-9"
        private string _pageRange = "";
        // 拆分模式：true表示每页拆分，false表示按页码范围拆分
        private bool _isSplitByPage = true;
        // 输出目录路径
        private string _outputPath = "";

        // 选中的PDF文件集合
        public ObservableCollection<string> SelectedFiles { get; } = [];

        // 状态消息属性
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        // 页码范围属性
        public string PageRange
        {
            get => _pageRange;
            set => SetField(ref _pageRange, value);
        }

        // 拆分模式属性
        public bool IsSplitByPage
        {
            get => _isSplitByPage;
            set => SetField(ref _isSplitByPage, value);
        }

        // 输出目录路径属性
        public string OutputPath
        {
            get => _outputPath;
            set => SetField(ref _outputPath, value);
        }

        /// <summary>
        /// 选择要拆分的PDF文件
        /// </summary>
        public async Task SelectFiles()
        {
            var storageProvider = App.MainWindow.StorageProvider;
            // 打开文件选择器，允许多选PDF文件
            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = true,
                FileTypeFilter = [new FilePickerFileType("PDF Files") { Patterns = ["*.pdf"] }]
            });

            // 如果用户选择了文件
            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    var path = file.Path.LocalPath;
                    // 避免重复添加相同的文件
                    if (!SelectedFiles.Contains(path))
                    {
                        SelectedFiles.Add(path);
                    }
                }
            }
        }

        /// <summary>
        /// 从列表中移除指定的文件
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

        /// <summary>
        /// 选择PDF拆分后的输出目录
        /// </summary>
        public async Task SelectOutputPath()
        {
            var storageProvider = App.MainWindow.StorageProvider;
            // 打开文件夹选择器
            var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "选择输出目录"
            });

            // 如果用户选择了文件夹
            if (folders != null && folders.Count > 0)
            {
                OutputPath = folders[0].Path.LocalPath;
            }
        }

        /// <summary>
        /// 执行PDF拆分操作
        /// </summary>
        public async Task SplitFiles()
        {
            // 验证是否选择了文件
            if (SelectedFiles.Count == 0) return;

            try
            {
                // 在后台线程执行PDF拆分操作
                await Task.Run(() =>
                {
                    // 确定输出目录：如果未指定，则在源文件目录下创建"拆分文件"文件夹
                    var defaultOutputFolder = string.IsNullOrEmpty(OutputPath)
                        ? Path.Combine(Path.GetDirectoryName(SelectedFiles[0]) ?? "", "拆分文件")
                        : OutputPath;

                    // 确保输出目录存在
                    Directory.CreateDirectory(defaultOutputFolder);

                    // 处理每个选中的PDF文件
                    foreach (var file in SelectedFiles)
                    {
                        // 以导入模式打开PDF文件
                        using var document = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                        var baseFileName = Path.GetFileNameWithoutExtension(file);

                        if (IsSplitByPage)
                        {
                            // 每页拆分模式：将每页保存为单独的PDF文件
                            for (int i = 0; i < document.PageCount; i++)
                            {
                                using var output = new PdfDocument();
                                output.AddPage(document.Pages[i]);
                                output.Save(Path.Combine(defaultOutputFolder, $"{baseFileName}_page_{i + 1}.pdf"));
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(PageRange))
                        {
                            // 页码范围模式：按指定的页码范围拆分
                            var ranges = PageRange.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (var range in ranges)
                            {
                                // 解析页码范围（例如："1-3" 或 "5"）
                                var parts = range.Trim().Split('-');
                                if (int.TryParse(parts[0], out int start))
                                {
                                    // 如果是单页，结束页等于开始页；如果是范围，解析结束页
                                    int end = parts.Length > 1 && int.TryParse(parts[1], out int e) ? e : start;

                                    // 验证页码范围的有效性
                                    if (start > 0 && end >= start && start <= document.PageCount)
                                    {
                                        using var output = new PdfDocument();
                                        // 复制指定范围的页面到新文档
                                        for (int i = start - 1; i < end && i < document.PageCount; i++)
                                        {
                                            output.AddPage(document.Pages[i]);
                                        }
                                        output.Save(Path.Combine(defaultOutputFolder, $"{baseFileName}_pages_{start}-{end}.pdf"));
                                    }
                                }
                            }
                        }
                    }
                });

                // 显示成功消息
                await ShowTemporaryMessage("PDF拆分完成！", message => StatusMessage = message);
            }
            catch (Exception ex)
            {
                // 显示错误消息
                await ShowTemporaryMessage($"拆分失败：{ex.Message}", message => StatusMessage = message);
            }
        }
    }
}