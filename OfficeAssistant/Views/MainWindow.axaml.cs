using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Platform.Storage;
using PdfSharpCore.Pdf;
using PdfSharpCore.Pdf.IO;
using OfficeAssistant.Views.PDF;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;


namespace OfficeAssistant.Views;

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
    private readonly PdfMergeView? _pdfMergeView;
    private readonly PdfSplitView? _pdfSplitView;
    private readonly PdfReplaceView? _pdfReplaceView;
    private readonly PdfDeleteView? _pdfDeleteView;
    private readonly PdfInsertView? _pdfInsertView;
    private readonly PdfCompressView? _pdfCompressView;

    public MainWindow()
    {
        InitializeComponent();
        // DataContext = new ViewModels.MainWindowViewModel();

        // 查找并初始化所有视图
        _pdfMergeView = this.Find<PdfMergeView>("PdfMergeView");
        _pdfSplitView = this.Find<PdfSplitView>("PdfSplitView");
        _pdfReplaceView = this.Find<PdfReplaceView>("PdfReplaceView");
        _pdfDeleteView = this.Find<PdfDeleteView>("PdfDeleteView");
        _pdfInsertView = this.Find<PdfInsertView>("PdfInsertView");
        _pdfCompressView = this.Find<PdfCompressView>("PdfCompressView");

        // 验证所有必需的视图都已找到
        if (_pdfMergeView == null || _pdfSplitView == null || _pdfReplaceView == null ||
            _pdfDeleteView == null || _pdfInsertView == null || _pdfCompressView == null)
        {
            throw new InvalidOperationException("无法找到所有必需的视图控件");
        }

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
    private void OnNavigationSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is ListBox listBox && 
            _pdfMergeView != null && _pdfSplitView != null && _pdfReplaceView != null &&
            _pdfDeleteView != null && _pdfInsertView != null && _pdfCompressView != null)
        {
            _pdfMergeView.IsVisible = listBox.SelectedIndex == 0;
            _pdfSplitView.IsVisible = listBox.SelectedIndex == 1;
            _pdfCompressView.IsVisible = listBox.SelectedIndex == 2;
            _pdfReplaceView.IsVisible = listBox.SelectedIndex == 3;
            _pdfDeleteView.IsVisible = listBox.SelectedIndex == 4;
            _pdfInsertView.IsVisible = listBox.SelectedIndex == 5;
        }
    }

    // Pdf图标点击事件，切换到PDF相关导航栏
    private void OnPdfIconClick(object? sender, RoutedEventArgs e)
    {
        // 显示PDF相关导航栏
        var navList = this.FindControl<ListBox>("NavigationList");
        if (navList != null)
        {
            navList.IsVisible = true;
            // 显示所有PDF相关导航项，隐藏OCR导航项
            foreach (var item in navList.Items)
            {
                if (item is ListBoxItem lbi)
                {
                    if (lbi.Name == "OcrNavItem")
                        lbi.IsVisible = false;
                    else
                        lbi.IsVisible = true;
                }
            }
            // 默认选中第一个PDF功能
            navList.SelectedIndex = 0;
        }
    }

    // OCR图标点击事件，切换到OCR相关导航栏（预留，实际功能待开发）
    private void OnOcrIconClick(object? sender, RoutedEventArgs e)
    {
        var navList = this.FindControl<ListBox>("NavigationList");
        if (navList != null)
        {
            // 只显示OCR导航项，隐藏PDF相关项
            navList.SelectedIndex = -1;
            foreach (var item in navList.Items)
            {
                if (item is ListBoxItem lbi)
                {
                    if (lbi.Name == "OcrNavItem")
                        lbi.IsVisible = true;
                    else
                        lbi.IsVisible = false;
                }
            }
        }
    }
}