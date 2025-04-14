using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using System.Collections.ObjectModel;
using System.Linq;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System.IO;
using System;
using System.Windows.Input;
using System.Threading.Tasks;

namespace OfficeAssistant;

public partial class MainWindow : Window
{
    private ObservableCollection<PdfFileItem> pdfFiles;
    public ICommand DeleteFile { get; }

    public MainWindow()
    {
        InitializeComponent();
        pdfFiles = new ObservableCollection<PdfFileItem>();
        FileList.Items = pdfFiles;
        
        DeleteFile = new RelayCommand<PdfFileItem>(file => 
        {
            if (file != null)
            {
                pdfFiles.Remove(file);
                UpdateMergeButtonState();
            }
        });

        SelectFilesButton.Click += SelectFiles_Click;
        MergeButton.Click += MergeFiles_Click;

        // 添加拖放事件处理
        AddHandler(DragDrop.DropEvent, Drop);
        AddHandler(DragDrop.DragOverEvent, DragOver);
    }

    private void DragOver(object? sender, DragEventArgs e)
    {
        e.DragEffects = e.DragEffects & (DragDropEffects.Copy | DragDropEffects.Link);
        e.Handled = true;
    }

    private void Drop(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.Files))
        {
            var files = e.Data.GetFiles()?.ToArray();
            if (files == null) return;

            foreach (var file in files)
            {
                if (file.Path.LocalPath.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
                {
                    AddPdfFile(file.Path.LocalPath);
                }
            }
        }
    }

    private async void SelectFiles_Click(object? sender, RoutedEventArgs e)
    {
        var dialog = new OpenFileDialog
        {
            AllowMultiple = true,
            Filters = new[] { new FileDialogFilter { Name = "PDF文件", Extensions = new[] { "pdf" } } }
        };

        var result = await dialog.ShowAsync(this);
        if (result != null)
        {
            foreach (var file in result)
            {
                AddPdfFile(file);
            }
        }
    }

    private void AddPdfFile(string filePath)
    {
        if (!pdfFiles.Any(f => f.FilePath == filePath))
        {
            pdfFiles.Add(new PdfFileItem(filePath));
            UpdateMergeButtonState();
        }
    }

    private async void MergeFiles_Click(object? sender, RoutedEventArgs e)
    {
        if (pdfFiles.Count < 2) return;

        var dialog = new SaveFileDialog
        {
            DefaultExtension = "pdf",
            Filters = new[] { new FileDialogFilter { Name = "PDF文件", Extensions = new[] { "pdf" } } }
        };

        var result = await dialog.ShowAsync(this);
        if (string.IsNullOrEmpty(result)) return;

        try
        {
            StatusMessage.Text = "正在合并...";
            MergeButton.IsEnabled = false;

            await Task.Run(() =>
            {
                using (var targetDoc = new PdfDocument())
                {
                    foreach (var file in pdfFiles)
                    {
                        using (var sourceDoc = PdfReader.Open(file.FilePath, PdfDocumentOpenMode.Import))
                        {
                            for (int i = 0; i < sourceDoc.PageCount; i++)
                            {
                                targetDoc.AddPage(sourceDoc.Pages[i]);
                            }
                        }
                    }
                    targetDoc.Save(result);
                }
            });

            StatusMessage.Text = "PDF合并成功！";
        }
        catch (Exception ex)
        {
            StatusMessage.Text = $"合并失败：{ex.Message}";
        }
        finally
        {
            MergeButton.IsEnabled = true;
        }
    }

    private void UpdateMergeButtonState()
    {
        MergeButton.IsEnabled = pdfFiles.Count >= 2;
    }
}

public class PdfFileItem
{
    public string FilePath { get; }
    public string FileName => Path.GetFileName(FilePath);

    public PdfFileItem(string filePath)
    {
        FilePath = filePath;
    }
}

public class RelayCommand<T> : ICommand
{
    private readonly Action<T> _execute;

    public RelayCommand(Action<T> execute)
    {
        _execute = execute;
    }

    public event EventHandler? CanExecuteChanged;

    public bool CanExecute(object? parameter) => true;

    public void Execute(object? parameter)
    {
        if (parameter is T param)
        {
            _execute(param);
        }
    }
}