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
    /// PDF页面插入功能的视图模型
    /// </summary>
    public partial class PdfInsertViewModel : ViewModelBase
    {
        // 状态消息，用于显示操作结果
        private string _statusMessage = "";
        // 要插入的页码
        private string _pageNumber = "1";
        // 插入位置（之前或之后）
        private bool _insertBefore = true;

        // 源PDF文件集合（将被插入页面的文件）
        public ObservableCollection<string> SourceFiles { get; } = [];
        // 待插入PDF文件
        public string InsertFile { get; set; } = "";

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

        // 插入位置属性
        public bool InsertBefore
        {
            get => _insertBefore;
            set => SetField(ref _insertBefore, value);
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
        /// 选择待插入的PDF文件
        /// </summary>
        public async Task SelectInsertFile()
        {
            var storageProvider = App.MainWindow.StorageProvider;
            // 打开文件选择器，选择单个PDF文件
            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                FileTypeFilter = [new FilePickerFileType("PDF Files") { Patterns = ["*.pdf"] }]
            });

            // 如果用户选择了文件
            if (files != null && files.Count > 0)
            {
                InsertFile = files[0].Path.LocalPath;
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
        /// 清除所有源文件
        /// </summary>
        public void ClearSourceFiles()
        {
            SourceFiles.Clear();
        }

        /// <summary>
        /// 清除待插入文件
        /// </summary>
        public void ClearInsertFile()
        {
            InsertFile = "";
        }

        /// <summary>
        /// 执行PDF页面插入操作
        /// </summary>
        public async Task InsertPages()
        {
            // 验证是否选择了文件
            if (SourceFiles.Count == 0 || string.IsNullOrEmpty(InsertFile))
            {
                await ShowTemporaryMessage("请选择源PDF文件和待插入PDF文件", message => StatusMessage = message);
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
                // 在后台线程执行PDF插入操作
                await Task.Run(() =>
                {
                    // 以导入模式打开待插入文件
                    using var insertDoc = PdfReader.Open(InsertFile, PdfDocumentOpenMode.Import);

                    // 验证待插入文件是否有页面
                    if (insertDoc.PageCount == 0)
                    {
                        throw new Exception($"待插入文件 {Path.GetFileName(InsertFile)} 没有页面");
                    }

                    // 遍历所有源文件
                    foreach (string sourceFile in SourceFiles)
                    {
                        // 以导入模式打开源文件
                        using var sourceDoc = PdfReader.Open(sourceFile, PdfDocumentOpenMode.Import);

                        // 验证源文件页码是否有效
                        if (pageIndex > sourceDoc.PageCount + 1)
                        {
                            throw new Exception($"文件 {Path.GetFileName(sourceFile)} 的页数少于 {pageIndex - 1}");
                        }

                        // 创建新的PDF文档
                        using var outputDoc = new PdfDocument();

                        // 复制源文档的所有页面到新文档
                        for (int j = 0; j < sourceDoc.PageCount; j++)
                        {
                            outputDoc.AddPage(sourceDoc.Pages[j]);
                        }

                        // 插入页面到指定位置
                        if (InsertBefore)
                        {
                            outputDoc.Pages.Insert(pageIndex - 1, insertDoc.Pages[0]);
                        }
                        else
                        {
                            outputDoc.Pages.Insert(pageIndex, insertDoc.Pages[0]);
                        }

                        // 保存文档，直接覆盖源文件
                        outputDoc.Save(sourceFile);
                    }
                });

                // 显示成功消息
                await ShowTemporaryMessage("PDF页面插入完成！源文件已被覆盖", message => StatusMessage = message);
            }
            catch (Exception ex)
            {
                // 显示错误消息
                await ShowTemporaryMessage($"插入失败：{ex.Message}", message => StatusMessage = message);
            }
        }
    }
}