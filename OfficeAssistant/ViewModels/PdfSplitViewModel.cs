using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Platform.Storage;  // 添加这行
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace OfficeAssistant.ViewModels
{
    public class PdfSplitViewModel : ViewModelBase
    {
        public ObservableCollection<string> SelectedFiles { get; } = new();
        public bool IsSplitByPage { get; set; } = true;
        public string PageRange { get; set; } = "";
        public string OutputPath { get; set; } = "";

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

        public void RemoveFile(string file)
        {
            SelectedFiles.Remove(file);
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

            foreach (var file in SelectedFiles)
            {
                using var document = PdfReader.Open(file, PdfDocumentOpenMode.Import);
                var outputFolder = string.IsNullOrEmpty(OutputPath) 
                    ? Path.Combine(Path.GetDirectoryName(file) ?? "", "Split_" + Path.GetFileNameWithoutExtension(file))  // 修改这里，处理 null
                    : OutputPath;
                
                Directory.CreateDirectory(outputFolder);

                if (IsSplitByPage)
                {
                    // 每页拆分为单独的PDF
                    for (int i = 0; i < document.PageCount; i++)
                    {
                        using var output = new PdfDocument();
                        output.AddPage(document.Pages[i]);
                        output.Save(Path.Combine(outputFolder, $"page_{i + 1}.pdf"));
                    }
                }
                else
                {
                    // 按页码范围拆分
                    var ranges = PageRange.Split(',');
                    foreach (var range in ranges)
                    {
                        var parts = range.Split('-');
                        int start = int.Parse(parts[0]);
                        int end = parts.Length > 1 ? int.Parse(parts[1]) : start;

                        using var output = new PdfDocument();
                        for (int i = start - 1; i < end && i < document.PageCount; i++)
                        {
                            output.AddPage(document.Pages[i]);
                        }
                        output.Save(Path.Combine(outputFolder, $"pages_{start}-{end}.pdf"));
                    }
                }
            }

            await ShowMessage("PDF拆分完成！");
        }

        private async Task ShowMessage(string message)
        {
            await MessageBox.Show(App.MainWindow, "提示", message, MessageBox.MessageBoxButtons.Ok);
        }
    }
}