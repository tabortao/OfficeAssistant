using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Linq;
using Avalonia.Platform.Storage;

namespace OfficeAssistant.ViewModels.PDF
{
    /// <summary>
    /// PDF转图片功能的视图模型
    /// </summary>
    public class PdfImageViewModel : ViewModelBase
    {
        // 状态消息
        private string _statusMessage = "";
        // 处理进度
        private double _progress = 0;
        // 处理时间
        private string _processingTime = "";
        // 页码范围
        private string _pageRange = "";
        // 图片分辨率
        private int _resolution = 300;

        // 选中的PDF文件集合
        public ObservableCollection<string> SelectedFiles { get; } = [];

        // 状态消息属性
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
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

        // 页码范围属性
        public string PageRange
        {
            get => _pageRange;
            set => SetField(ref _pageRange, value);
        }

        // 图片分辨率属性
        public int Resolution
        {
            get => _resolution;
            set => SetField(ref _resolution, Math.Max(72, Math.Min(1200, value)));
        }

        /// <summary>
        /// Avalonia 的 IStorageProvider，由外部注入
        /// </summary>
        public IStorageProvider? StorageProvider { get; set; }

        /// <summary>
        /// 选择PDF文件
        /// </summary>
        public async Task SelectFiles()
        {
            if (StorageProvider == null)
            {
                StatusMessage = "未设置 StorageProvider";
                return;
            }

            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "选择PDF文件",
                AllowMultiple = true,
                FileTypeFilter = [FilePickerFileTypes.Pdf]
            });

            if (files != null && files.Count > 0)
            {
                foreach (var file in files)
                {
                    if (!SelectedFiles.Contains(file.Path.LocalPath))
                    {
                        SelectedFiles.Add(file.Path.LocalPath);
                    }
                }
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
        /// 清空所有已选文件
        /// </summary>
        public void ClearAllFiles()
        {
            SelectedFiles.Clear();
        }

        /// <summary>
        /// 验证页码范围格式
        /// </summary>
        private bool ValidatePageRange(string range)
        {
            if (string.IsNullOrWhiteSpace(range))
                return true; // 空字符串表示全部页面

            var pattern = @"^(\d+(-\d+)?)(,\d+(-\d+)?)*$";
            if (!Regex.IsMatch(range, pattern))
                return false;

            foreach (var part in range.Split(','))
            {
                if (part.Contains('-'))
                {
                    var numbers = part.Split('-').Select(int.Parse).ToArray();
                    if (numbers[0] >= numbers[1])
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// 获取PDF总页数，仅用PdfSharpCore
        /// </summary>
        private Task<int> GetPdfPageCount(string pdfFile)
        {
            if (!File.Exists(pdfFile))
                throw new Exception("PDF文件不存在");

            using (var stream = File.OpenRead(pdfFile))
            {
                var doc = PdfSharpCore.Pdf.IO.PdfReader.Open(stream, PdfSharpCore.Pdf.IO.PdfDocumentOpenMode.InformationOnly);
                if (doc.PageCount > 0)
                    return Task.FromResult(doc.PageCount);
                throw new Exception("无法获取PDF页数");
            }
        }

        /// <summary>
        /// 执行PDF转图片操作
        /// </summary>
        public async Task ConvertToImages()
        {
            if (SelectedFiles.Count == 0)
            {
                StatusMessage = "请先选择PDF文件";
                return;
            }

            if (!ValidatePageRange(PageRange))
            {
                StatusMessage = "页码范围格式无效，请使用如 1-3,5,7-9 的格式";
                return;
            }

            var sw = Stopwatch.StartNew();
            Progress = 0;

            for (int i = 0; i < SelectedFiles.Count; i++)
            {
                var pdfFile = SelectedFiles[i];
                var pdfName = Path.GetFileNameWithoutExtension(pdfFile);
                var outputDir = Path.Combine(
                    Path.GetDirectoryName(pdfFile)!,
                    $"{pdfName}_Png"
                );

                try
                {
                    // 创建输出目录
                    Directory.CreateDirectory(outputDir);

                    // 获取需要处理的页码列表
                    var pages = new List<int>();
                    
                    if (string.IsNullOrWhiteSpace(PageRange))
                    {
                        // 空页码范围，获取总页数并处理所有页面
                        var totalPages = await GetPdfPageCount(pdfFile);
                        pages = Enumerable.Range(1, totalPages).ToList();
                        StatusMessage = $"PDF共 {totalPages} 页，开始转换所有页面";
                    }
                    else
                    {
                        // 解析指定的页码范围
                        foreach (var part in PageRange.Split(',', StringSplitOptions.RemoveEmptyEntries))
                        {
                            if (part.Contains('-'))
                            {
                                var range = part.Split('-').Select(int.Parse).ToArray();
                                if (range.Length == 2 && range[0] <= range[1])
                                {
                                    pages.AddRange(Enumerable.Range(range[0], range[1] - range[0] + 1));
                                }
                            }
                            else if (int.TryParse(part.Trim(), out int pageNum))
                            {
                                pages.Add(pageNum);
                            }
                        }
                    }

                    // 为每个页码单独运行Ghostscript
                    double progressPerPage = 100.0 / (pages.Count * SelectedFiles.Count);
                    int processedPages = 0;

                    foreach (var pageNum in pages)
                    {
                        var outputFile = Path.Combine(outputDir, $"{pdfName}_页{pageNum}.png");
                        var gsArgs = $"-dNOPAUSE -dBATCH -dSAFER -sDEVICE=png16m -r{Resolution} " +
                                   $"-dFirstPage={pageNum} -dLastPage={pageNum} " +
                                   $"-sOutputFile=\"{outputFile}\" \"{pdfFile}\"";

                        var startInfo = new ProcessStartInfo
                        {
                            FileName = "gswin64c",
                            Arguments = gsArgs,
                            UseShellExecute = false,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        };

                        using var process = Process.Start(startInfo);
                        if (process == null)
                        {
                            throw new Exception($"处理第 {pageNum} 页时无法启动Ghostscript进程");
                        }

                        await process.WaitForExitAsync();
                        if (process.ExitCode != 0)
                        {
                            var error = await process.StandardError.ReadToEndAsync();
                            throw new Exception($"处理第 {pageNum} 页时出错: {error}");
                        }

                        processedPages++;
                        Progress = (processedPages * progressPerPage);
                        StatusMessage = $"正在处理第 {i + 1}/{SelectedFiles.Count} 个文件，已完成 {processedPages}/{pages.Count} 页";
                    }
                }
                catch (Exception ex)
                {
                    StatusMessage = $"处理文件 {Path.GetFileName(pdfFile)} 时出错: {ex.Message}";
                    return;
                }
            }

            sw.Stop();
            ProcessingTime = $"处理完成，耗时: {sw.Elapsed.TotalSeconds:F1} 秒";
            StatusMessage = "所有文件处理完成！";
            Progress = 100;
        }
    }
}
