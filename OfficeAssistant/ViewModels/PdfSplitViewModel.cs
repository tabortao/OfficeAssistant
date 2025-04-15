using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;  // 添加这行
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace OfficeAssistant.ViewModels
{
    public class PdfSplitViewModel : ViewModelBase
    {
        private string _statusMessage = "";
        private string _pageRange = "";
        private bool _isSplitByPage = true;
        private string _outputPath = "";

        public ObservableCollection<string> SelectedFiles { get; } = new();

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public string PageRange
        {
            get => _pageRange;
            set => SetField(ref _pageRange, value);
        }

        public bool IsSplitByPage
        {
            get => _isSplitByPage;
            set => SetField(ref _isSplitByPage, value);
        }

        public string OutputPath
        {
            get => _outputPath;
            set => SetField(ref _outputPath, value);
        }

        public async Task SelectFiles()
        {
            var storageProvider = App.MainWindow.StorageProvider;
            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = true,
                FileTypeFilter = new[] { new FilePickerFileType("PDF Files") { Patterns = new[] { "*.pdf" } } }
            });

            if (files != null && files.Count > 0)  // 修改这里，避免 null 引用
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

        public void RemoveFile(string file)  // 移除 async 关键字，因为这是同步操作
        {
            SelectedFiles.Remove(file);
        }

        public void ClearAllFiles()
        {
            SelectedFiles.Clear();
        }

        public async Task SelectOutputPath()
        {
            var storageProvider = App.MainWindow.StorageProvider;
            var folders = await storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions
            {
                Title = "选择输出目录"
            });

            if (folders != null && folders.Count > 0)  // 修改这里，因为返回的是列表
            {
                OutputPath = folders[0].Path.LocalPath;  // 获取第一个选择的文件夹
            }
        }

        public async Task SplitFiles()
        {
            if (SelectedFiles.Count == 0) return;

            try
            {
                await Task.Run(() =>
                {
                    // 如果没有设置输出目录，创建默认的"拆分文件"文件夹
                    var defaultOutputFolder = string.IsNullOrEmpty(OutputPath)
                        ? Path.Combine(Path.GetDirectoryName(SelectedFiles[0]) ?? "", "拆分文件")
                        : OutputPath;
                    
                    Directory.CreateDirectory(defaultOutputFolder);

                    foreach (var file in SelectedFiles)
                    {
                        using var document = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                        var baseFileName = Path.GetFileNameWithoutExtension(file);

                        if (IsSplitByPage)
                        {
                            // 每页拆分为单独的PDF
                            for (int i = 0; i < document.PageCount; i++)
                            {
                                using var output = new PdfDocument();
                                output.AddPage(document.Pages[i]);
                                output.Save(Path.Combine(defaultOutputFolder, $"{baseFileName}_page_{i + 1}.pdf"));
                            }
                        }
                        else if (!string.IsNullOrWhiteSpace(PageRange))
                        {
                            // 按页码范围拆分
                            var ranges = PageRange.Split(',', StringSplitOptions.RemoveEmptyEntries);
                            foreach (var range in ranges)
                            {
                                var parts = range.Trim().Split('-');
                                if (int.TryParse(parts[0], out int start))
                                {
                                    int end = parts.Length > 1 && int.TryParse(parts[1], out int e) ? e : start;
                                    
                                    if (start > 0 && end >= start && start <= document.PageCount)
                                    {
                                        using var output = new PdfDocument();
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
                
                await ShowTemporaryMessage("PDF拆分完成！", message => StatusMessage = message);
            }
            catch (Exception ex)
            {
                await ShowTemporaryMessage($"拆分失败：{ex.Message}", message => StatusMessage = message);
            }
        }
    }
}