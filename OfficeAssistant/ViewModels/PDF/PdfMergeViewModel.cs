using Avalonia.Platform.Storage;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace OfficeAssistant.ViewModels.PDF
{
    /// <summary>
    /// PDF合并功能的视图模型
    /// </summary>
    public class PdfMergeViewModel : ViewModelBase
    {
        // 状态消息，用于显示操作结果
        private string _statusMessage = "";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        // 存储选中的PDF文件路径集合
        public ObservableCollection<string> SelectedFiles { get; } = [];

        /// <summary>
        /// 选择PDF文件的方法
        /// </summary>
        public async Task SelectFiles()
        {
            // 获取存储提供程序
            var storageProvider = App.MainWindow.StorageProvider;
            // 打开文件选择器，允许多选PDF文件
            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = true,
                FileTypeFilter = [new("PDF Files") { Patterns = ["*.pdf"] }]
            });

            // 如果用户选择了文件
            if (files != null && files.Count > 0)
            {
                // 遍历所有选中的文件
                foreach (var file in files)
                {
                    // 获取文件的本地路径
                    var path = file.Path.LocalPath;
                    // 如果文件尚未添加到列表中，则添加
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
        /// <param name="file">要移除的文件路径</param>
        public void RemoveFile(string file)
        {
            SelectedFiles.Remove(file);
        }

        /// <summary>
        /// 合并选中的PDF文件
        /// </summary>
        public async Task MergeFiles()
        {
            // 至少需要两个文件才能合并
            if (SelectedFiles.Count < 2) return;

            try
            {
                // 获取存储提供程序
                var storageProvider = App.MainWindow.StorageProvider;
                // 打开保存文件对话框
                var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "保存合并后的PDF",
                    DefaultExtension = "pdf",
                    FileTypeChoices = new[] { new FilePickerFileType("PDF Files") { Patterns = new[] { "*.pdf" } } }
                });

                // 如果用户选择了保存位置
                if (file != null)
                {
                    // 创建新的PDF文档
                    using var output = new PdfDocument();
                    // 遍历所有选中的PDF文件
                    foreach (var pdfFile in SelectedFiles)
                    {
                        // 以导入模式打开PDF文件
                        using var document = PdfReader.Open(pdfFile, PdfDocumentOpenMode.Import);
                        // 将文档的所有页面添加到输出文档
                        for (int i = 0; i < document.PageCount; i++)
                        {
                            output.AddPage(document.Pages[i]);
                        }
                    }
                    // 保存合并后的文档
                    output.Save(file.Path.LocalPath);
                    // 显示成功消息
                    await ShowTemporaryMessage("PDF合并完成！", message => StatusMessage = message);
                }
            }
            catch (Exception ex)
            {
                // 显示错误消息
                await ShowTemporaryMessage($"合并失败：{ex.Message}", message => StatusMessage = message);
            }
        }

        /// <summary>
        /// 清除所有选中的文件
        /// </summary>
        public void ClearAllFiles()
        {
            SelectedFiles.Clear();
        }
    }
}