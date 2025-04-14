using System;  // 添加这行，用于 Exception
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Avalonia.Platform.Storage;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;

namespace OfficeAssistant.ViewModels
{
    public class PdfMergeViewModel : ViewModelBase
    {
        private string _statusMessage = "";
        public string StatusMessage
        {
            get => _statusMessage;
            set => SetField(ref _statusMessage, value);
        }

        public ObservableCollection<string> SelectedFiles { get; } = new();

        public async Task SelectFiles()
        {
            var storageProvider = App.MainWindow.StorageProvider;
            var files = await storageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = true,
                FileTypeFilter = new[] { new FilePickerFileType("PDF Files") { Patterns = new[] { "*.pdf" } } }
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

        public void RemoveFile(string file)
        {
            SelectedFiles.Remove(file);
        }

        public async Task MergeFiles()
        {
            if (SelectedFiles.Count < 2) return;

            try
            {
                var storageProvider = App.MainWindow.StorageProvider;
                var file = await storageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
                {
                    Title = "保存合并后的PDF",
                    DefaultExtension = "pdf",
                    FileTypeChoices = new[] { new FilePickerFileType("PDF Files") { Patterns = new[] { "*.pdf" } } }
                });

                if (file != null)
                {
                    using var output = new PdfDocument();
                    foreach (var pdfFile in SelectedFiles)
                    {
                        using var document = PdfReader.Open(pdfFile, PdfDocumentOpenMode.Import);
                        for (int i = 0; i < document.PageCount; i++)
                        {
                            output.AddPage(document.Pages[i]);
                        }
                    }
                    output.Save(file.Path.LocalPath);
                    await ShowTemporaryMessage("PDF合并完成！", message => StatusMessage = message);
                }
            }
            catch (Exception ex)
            {
                await ShowTemporaryMessage($"合并失败：{ex.Message}", message => StatusMessage = message);
            }
        }

        public void ClearAllFiles()
        {
            SelectedFiles.Clear();
        }
    }
}