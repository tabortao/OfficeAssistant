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
    /// PDF页面替换功能的视图模型
    /// </summary>
    public partial class PdfReplaceViewModel : ViewModelBase
    {
        // 状态消息，用于显示操作结果
        private string _statusMessage = "";
        // 要替换的页码
        private string _pageNumber = "1";

        // 源PDF文件集合（将被替换页面的文件）
        public ObservableCollection<string> SourceFiles { get; } = [];
        // 替换PDF文件集合（用于替换的文件）
        public ObservableCollection<string> ReplacementFiles { get; } = [];

        // 状态消息属性
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        // 页码属性
        public string PageNumber
        {
            get => _pageNumber;
            set => SetField(ref _pageNumber, value);
        }

        /// <summary>
        /// 选择源PDF文件
        /// </summary>
        public async Task SelectSourceFiles()
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
                    if (!SourceFiles.Contains(path))
                    {
                        SourceFiles.Add(path);
                    }
                }
            }
        }

        /// <summary>
        /// 选择用于替换的PDF文件
        /// </summary>
        public async Task SelectReplacementFiles()
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
                    if (!ReplacementFiles.Contains(path))
                    {
                        ReplacementFiles.Add(path);
                    }
                }
            }
        }

        /// <summary>
        /// 从列表中移除指定的源文件
        /// </summary>
        public void RemoveSourceFile(string file)
        {
            SourceFiles.Remove(file);
        }

        /// <summary>
        /// 从列表中移除指定的替换文件
        /// </summary>
        public void RemoveReplacementFile(string file)
        {
            ReplacementFiles.Remove(file);
        }

        /// <summary>
        /// 清除所有源文件
        /// </summary>
        public void ClearSourceFiles()
        {
            SourceFiles.Clear();
        }

        /// <summary>
        /// 清除所有替换文件
        /// </summary>
        public void ClearReplacementFiles()
        {
            ReplacementFiles.Clear();
        }

        /// <summary>
        /// 执行PDF页面替换操作
        /// </summary>
        public async Task ReplacePages()
        {
            // 验证是否选择了文件
            if (SourceFiles.Count == 0 || ReplacementFiles.Count == 0)
            {
                await ShowTemporaryMessage("请选择源PDF文件和替换PDF文件", message => StatusMessage = message);
                return;
            }

            // 验证源文件和替换文件数量是否一致
            if (SourceFiles.Count != ReplacementFiles.Count)
            {
                await ShowTemporaryMessage("源PDF文件数量必须与替换PDF文件数量一致", message => StatusMessage = message);
                return;
            }

            // 验证页码是否有效
            if (!int.TryParse(PageNumber, out int pageIndex) || pageIndex < 1)
            {
                await ShowTemporaryMessage("请输入有效的页码", message => StatusMessage = message);
                return;
            }

            try
            {
                // 在后台线程执行PDF替换操作
                await Task.Run(() =>
                {
                    // 遍历所有文件对
                    for (int i = 0; i < SourceFiles.Count; i++)
                    {
                        string sourceFile = SourceFiles[i];
                        string replacementFile = ReplacementFiles[i];

                        // 以导入模式打开源文件
                        using var sourceDoc = PdfReader.Open(sourceFile, PdfDocumentOpenMode.Import);
                        // 以导入模式打开替换文件
                        using var replacementDoc = PdfReader.Open(replacementFile, PdfDocumentOpenMode.Import);

                        // 验证源文件页码是否有效
                        if (pageIndex > sourceDoc.PageCount)
                        {
                            throw new Exception($"文件 {Path.GetFileName(sourceFile)} 的页数少于 {pageIndex}");
                        }

                        // 验证替换文件是否有页面
                        if (replacementDoc.PageCount == 0)
                        {
                            throw new Exception($"替换文件 {Path.GetFileName(replacementFile)} 没有页面");
                        }

                        // 创建新的PDF文档
                        using var outputDoc = new PdfDocument();

                        // 复制源文档的所有页面到新文档
                        for (int j = 0; j < sourceDoc.PageCount; j++)
                        {
                            outputDoc.AddPage(sourceDoc.Pages[j]);
                        }

                        // 替换指定页面：先移除原页面，再插入新页面
                        outputDoc.Pages.Remove(outputDoc.Pages[pageIndex - 1]);
                        outputDoc.Pages.Insert(pageIndex - 1, replacementDoc.Pages[0]);

                        // 保存文档，直接覆盖源文件
                        outputDoc.Save(sourceFile);
                    }
                });

                // 显示成功消息
                await ShowTemporaryMessage("PDF页面替换完成！源文件已被覆盖", message => StatusMessage = message);
            }
            catch (Exception ex)
            {
                // 显示错误消息
                await ShowTemporaryMessage($"替换失败：{ex.Message}", message => StatusMessage = message);
            }
        }
    }
}