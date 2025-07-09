using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using OfficeAssistant.ViewModels.PDF;

namespace OfficeAssistant.Views.PDF
{
    public partial class PdfSplitView : UserControl
    {
        private readonly OfficeAssistant.ViewModels.PDF.PdfSplitViewModel _viewModel;

        public PdfSplitView()
        {
            InitializeComponent();
            _viewModel = new PdfSplitViewModel();
            DataContext = _viewModel;

            // 通过代码注册 Drop 和 DragOver 事件，确保控件已加载
            this.AttachedToVisualTree += (s, e) =>
            {
                var listBox = this.FindControl<ListBox>("FileListBox");
                if (listBox != null)
                {
                    listBox.AddHandler(DragDrop.DropEvent, ListBox_OnDrop, Avalonia.Interactivity.RoutingStrategies.Bubble);
                    listBox.AddHandler(DragDrop.DragOverEvent, ListBox_OnDragOver, Avalonia.Interactivity.RoutingStrategies.Bubble);
                }
            };
        }
        // 处理拖拽悬停，显示可放置样式
        private void ListBox_OnDragOver(object? sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Files))
            {
                e.DragEffects = DragDropEffects.Copy;
            }
            else
            {
                e.DragEffects = DragDropEffects.None;
            }
        }

        private async void SelectFiles(object sender, RoutedEventArgs e)
        {
            await _viewModel.SelectFiles();
        }

        private void RemoveFile(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && button.CommandParameter is string file)
            {
                _viewModel.RemoveFile(file);
            }
        }

        private async void SelectOutputPath(object sender, RoutedEventArgs e)
        {
            await _viewModel.SelectOutputPath();
        }

        private async void SplitFiles(object sender, RoutedEventArgs e)
        {
            await _viewModel.SplitFiles();
        }

        private void ClearAllFiles(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearAllFiles();
        }
        // 支持拖拽文件到ListBox
        private void ListBox_OnDrop(object? sender, DragEventArgs e)
        {
            if (e.Data.Contains(DataFormats.Files))
            {
                var files = e.Data.GetFiles();
                if (files != null)
                {
                    foreach (var file in files)
                    {
                        var path = file.Path.LocalPath;
                        if (path.EndsWith(".pdf", System.StringComparison.OrdinalIgnoreCase))
                        {
                            if (!_viewModel.SelectedFiles.Contains(path))
                                _viewModel.SelectedFiles.Add(path);
                        }
                    }
                }
            }
        }        
    }
}