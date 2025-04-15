using Avalonia.Platform.Storage;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;

namespace OfficeAssistant.ViewModels
{
    public partial class PdfReplaceViewModel : ViewModelBase
    {
        private string _statusMessage = "";
        private string _pageNumber = "1";

        public ObservableCollection<string> SourceFiles { get; } = [];
        public ObservableCollection<string> ReplacementFiles { get; } = [];

        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public string PageNumber
        {
            get => _pageNumber;
            set => SetField(ref _pageNumber, value);
        }

        public async Task SelectSourceFiles()
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
                    if (!SourceFiles.Contains(path))
                    {
                        SourceFiles.Add(path);
                    }
                }
            }
        }

        public async Task SelectReplacementFiles()
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
                    if (!ReplacementFiles.Contains(path))
                    {
                        ReplacementFiles.Add(path);
                    }
                }
            }
        }

        public void RemoveSourceFile(string file)
        {
            SourceFiles.Remove(file);
        }

        public void RemoveReplacementFile(string file)
        {
            ReplacementFiles.Remove(file);
        }

        public void ClearSourceFiles()
        {
            SourceFiles.Clear();
        }

        public void ClearReplacementFiles()
        {
            ReplacementFiles.Clear();
        }

        public async Task ReplacePages()
        {
            if (SourceFiles.Count == 0 || ReplacementFiles.Count == 0)
            {
                await ShowTemporaryMessage("请选择源PDF文件和替换PDF文件", message => StatusMessage = message);
                return;
            }

            if (SourceFiles.Count != ReplacementFiles.Count)
            {
                await ShowTemporaryMessage("源PDF文件数量必须与替换PDF文件数量一致", message => StatusMessage = message);
                return;
            }

            if (!int.TryParse(PageNumber, out int pageIndex) || pageIndex < 1)
            {
                await ShowTemporaryMessage("请输入有效的页码", message => StatusMessage = message);
                return;
            }

            try
            {
                // 移除选择输出目录的部分，直接处理文件
                await Task.Run(() =>
                {
                    for (int i = 0; i < SourceFiles.Count; i++)
                    {
                        string sourceFile = SourceFiles[i];
                        string replacementFile = ReplacementFiles[i];

                        // 打开源文件
                        using var sourceDoc = PdfReader.Open(sourceFile, PdfDocumentOpenMode.Import);
                        // 打开替换文件
                        using var replacementDoc = PdfReader.Open(replacementFile, PdfDocumentOpenMode.Import);

                        // 检查页码是否有效
                        if (pageIndex > sourceDoc.PageCount)
                        {
                            throw new Exception($"文件 {Path.GetFileName(sourceFile)} 的页数少于 {pageIndex}");
                        }

                        if (replacementDoc.PageCount == 0)
                        {
                            throw new Exception($"替换文件 {Path.GetFileName(replacementFile)} 没有页面");
                        }

                        // 创建新文档
                        using var outputDoc = new PdfDocument();

                        // 复制源文档的所有页面到新文档
                        for (int j = 0; j < sourceDoc.PageCount; j++)
                        {
                            outputDoc.AddPage(sourceDoc.Pages[j]);
                        }

                        // 替换指定页面
                        outputDoc.Pages.Remove(outputDoc.Pages[pageIndex - 1]);
                        outputDoc.Pages.Insert(pageIndex - 1, replacementDoc.Pages[0]);

                        // 直接覆盖源文件
                        outputDoc.Save(sourceFile);
                    }
                });

                await ShowTemporaryMessage("PDF页面替换完成！源文件已被覆盖", message => StatusMessage = message);
            }
            catch (Exception ex)
            {
                await ShowTemporaryMessage($"替换失败：{ex.Message}", message => StatusMessage = message);
            }
        }
    }
}