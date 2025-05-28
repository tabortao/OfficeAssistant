using Avalonia.Platform.Storage;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace OfficeAssistant.ViewModels.PDF
{
    /// <summary>
    /// PDF页面删除功能的视图模型
    /// </summary>
    public class PdfDeleteViewModel : ViewModelBase
    {
        // 状态消息
        private string _statusMessage = "";
        // 要删除的页码范围
        private string _pageRange = "";
        // 处理进度
        private double _progress;
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

        // 页码范围属性
        public string PageRange
        {
            get => _pageRange;
            set => SetField(ref _pageRange, value);
        }

        // 进度属性
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
            var files = await App.MainWindow.StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
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
        /// 从列表中移除指定的文件
        /// </summary>
        public void RemoveFile(string file) => SelectedFiles.Remove(file);

        /// <summary>
        /// 清除所有选中的文件
        /// </summary>
        public void ClearAllFiles() => SelectedFiles.Clear();

        /// <summary>
        /// 执行PDF页面删除操作
        /// </summary>
        public async Task DeletePages()
        {
            if (SelectedFiles.Count == 0)
            {
                await ShowTemporaryMessage("请选择要处理的PDF文件", message => StatusMessage = message);
                return;
            }

            if (string.IsNullOrWhiteSpace(PageRange))
            {
                await ShowTemporaryMessage("请输入要删除的页码范围", message => StatusMessage = message);
                return;
            }

            Progress = 0;
            var stopwatch = Stopwatch.StartNew();
            var total = SelectedFiles.Count;
            var processed = 0;

            try
            {
                await Task.Run(() =>
                {
                    foreach (var file in SelectedFiles)
                    {
                        // 创建备份文件名
                        var backupFile = Path.Combine(
                            Path.GetDirectoryName(file) ?? "",
                            Path.GetFileNameWithoutExtension(file) + "_backup.pdf"
                        );

                        // 复制原文件作为备份
                        File.Copy(file, backupFile, true);

                        try
                        {
                            using var document = PdfReader.Open(backupFile, PdfDocumentOpenMode.Import);
                            using var output = new PdfDocument();
                            var pagesToDelete = ParsePageRanges(PageRange, document.PageCount);

                            // 复制不需要删除的页面
                            for (int i = 0; i < document.PageCount; i++)
                            {
                                if (!pagesToDelete.Contains(i + 1))
                                {
                                    output.AddPage(document.Pages[i]);
                                }
                            }

                            // 保存文件
                            output.Save(file);
                            File.Delete(backupFile); // 删除备份文件
                        }
                        catch
                        {
                            // 恢复备份文件
                            if (File.Exists(backupFile))
                            {
                                File.Copy(backupFile, file, true);
                                File.Delete(backupFile);
                            }
                            throw;
                        }

                        processed++;
                        Progress = (double)processed / total * 100;
                        ProcessingTime = $"处理时间：{stopwatch.Elapsed.TotalSeconds:F1}秒";
                    }
                });

                stopwatch.Stop();
                ProcessingTime = $"总处理时间：{stopwatch.Elapsed.TotalSeconds:F1}秒";
                await ShowTemporaryMessage("PDF页面删除完成！", message => StatusMessage = message);
            }
            catch (Exception ex)
            {
                await ShowTemporaryMessage($"删除失败：{ex.Message}", message => StatusMessage = message);
            }
        }

        /// <summary>
        /// 解析页码范围字符串
        /// </summary>
        private HashSet<int> ParsePageRanges(string pageRange, int maxPages)
        {
            var pages = new HashSet<int>();
            var ranges = pageRange.Split(',', StringSplitOptions.RemoveEmptyEntries);

            foreach (var range in ranges)
            {
                var parts = range.Trim().Split('-');
                if (parts.Length == 1 && int.TryParse(parts[0], out int page))
                {
                    if (page > 0 && page <= maxPages)
                    {
                        pages.Add(page);
                    }
                }
                else if (parts.Length == 2 && 
                         int.TryParse(parts[0], out int start) && 
                         int.TryParse(parts[1], out int end))
                {
                    for (int i = start; i <= end && i <= maxPages; i++)
                    {
                        if (i > 0)
                        {
                            pages.Add(i);
                        }
                    }
                }
            }

            return pages;
        }
    }
}