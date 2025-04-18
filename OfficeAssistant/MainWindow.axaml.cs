using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace OfficeAssistant;

public partial class MainWindow : Window
{
    // 存储选中的文件列表
    private readonly ObservableCollection<string> selectedFiles;
    // 记录最后一次保存的文件路径
    private string? lastSavedFile;

    // 获取应用程序版本号
    public static string Version
    {
        get
        {
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            var version = assembly.GetName().Version;
            return $"V{version?.Major}.{version?.Minor}.{version?.Build}";
        }
    }

    // 窗口构造函数
    public MainWindow()
    {
        InitializeComponent();
        selectedFiles = [];
        DataContext = this;
    }

    // 公开选中文件列表的属性
    public ObservableCollection<string> SelectedFiles => selectedFiles;

    // 选择PDF文件的方法
    private async Task SelectFiles()
    {
        var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
        {
            Title = "选择PDF文件",
            AllowMultiple = true,
            FileTypeFilter = [FilePickerFileTypes.Pdf]
        });

        // 添加新选择的文件到列表中，避免重复
        foreach (var file in files)
        {
            if (!selectedFiles.Contains(file.Path.LocalPath))
            {
                selectedFiles.Add(file.Path.LocalPath);
            }
        }
    }

    // 合并PDF文件的方法
    private async Task MergeFiles()
    {
        // 至少需要两个文件才能合并
        if (selectedFiles.Count < 2) return;

        // 选择保存位置
        var file = await StorageProvider.SaveFilePickerAsync(new FilePickerSaveOptions
        {
            Title = "保存合并后的PDF",
            DefaultExtension = "pdf",
            FileTypeChoices = [FilePickerFileTypes.Pdf]
        });

        if (file == null) return;

        try
        {
            // 创建新的PDF文档并合并所有页面
            using (var output = new PdfDocument())
            {
                foreach (var path in selectedFiles)
                {
                    using var input = PdfReader.Open(path, PdfDocumentOpenMode.Import);
                    for (var i = 0; i < input.PageCount; i++)
                    {
                        output.AddPage(input.Pages[i]);
                    }
                }
                output.Save(file.Path.LocalPath);
            }

            lastSavedFile = file.Path.LocalPath;

            // 显示成功消息
            var messageText = this.FindControl<TextBlock>("MessageText");
            if (messageText != null)
            {
                messageText.Text = $"PDF文件已成功合并并保存到：{file.Path.LocalPath}";
                messageText.Opacity = 1;

                // 3秒后淡出消息
                await Task.Delay(3000);
                messageText.Opacity = 0;
            }
        }
        catch (Exception ex)
        {
            // 显示错误消息
            var messageText = this.FindControl<TextBlock>("MessageText");
            if (messageText != null)
            {
                messageText.Text = $"合并PDF文件时发生错误：{ex.Message}";
                messageText.Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Red);
                messageText.Opacity = 1;

                // 3秒后淡出消息
                await Task.Delay(3000);
                messageText.Opacity = 0;
                messageText.Foreground = new Avalonia.Media.SolidColorBrush(Avalonia.Media.Colors.Green);
            }
        }
    }

    // 从列表中移除文件
    private void RemoveFile(string file)
    {
        selectedFiles.Remove(file);
    }

    // 选择文件按钮点击事件处理
    private async void SelectFiles(object sender, RoutedEventArgs e)
    {
        await SelectFiles();
    }

    // 合并文件按钮点击事件处理
    private async void MergeFiles(object sender, RoutedEventArgs e)
    {
        await MergeFiles();
    }

    // 移除文件按钮点击事件处理
    private void RemoveFile(object sender, RoutedEventArgs e)
    {
        if (e.Source is Button button && button.CommandParameter is string file)
        {
            RemoveFile(file);
        }
    }

    // 导航栏选择变更事件处理
    private void OnNavigationSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        // 根据选择的导航项显示对应的视图
        if (NavigationList.SelectedIndex == 0)
        {
            PdfMergeView.IsVisible = true;
            PdfSplitView.IsVisible = false;
            PdfReplaceView.IsVisible = false;
        }
        else if (NavigationList.SelectedIndex == 1)
        {
            PdfMergeView.IsVisible = false;
            PdfSplitView.IsVisible = true;
            PdfReplaceView.IsVisible = false;
        }
        else if (NavigationList.SelectedIndex == 2)
        {
            PdfMergeView.IsVisible = false;
            PdfSplitView.IsVisible = false;
            PdfReplaceView.IsVisible = true;
        }
    }
}