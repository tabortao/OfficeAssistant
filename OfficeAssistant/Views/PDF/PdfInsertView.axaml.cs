using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Input;
using OfficeAssistant.ViewModels.PDF;

namespace OfficeAssistant.Views.PDF
{
    public partial class PdfInsertView : UserControl
    {
        private readonly OfficeAssistant.ViewModels.PDF.PdfInsertViewModel _viewModel;

        public PdfInsertView()
        {
            InitializeComponent();
            _viewModel = new PdfInsertViewModel();
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
                            if (!_viewModel.SourceFiles.Contains(path))
                                _viewModel.SourceFiles.Add(path);
                        }
                    }
                }
            }
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

        private async void SelectSourceFiles(object sender, RoutedEventArgs e)
        {
            await _viewModel.SelectSourceFiles();
        }

        private void RemoveSourceFile(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && button.CommandParameter is string file)
            {
                _viewModel.RemoveSourceFile(file);
            }
        }

        private void ClearSourceFiles(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearSourceFiles();
        }

        private async void SelectInsertFile(object sender, RoutedEventArgs e)
        {
            await _viewModel.SelectInsertFile();
        }

        private void ClearInsertFile(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearInsertFile();
        }

        private async void InsertPages(object sender, RoutedEventArgs e)
        {
            await _viewModel.InsertPages();
        }
    }
}