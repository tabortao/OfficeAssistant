using Avalonia.Controls;
using Avalonia.Interactivity;
using OfficeAssistant.ViewModels;

namespace OfficeAssistant.Views
{
    public partial class PdfInsertView : UserControl
    {
        private readonly PdfInsertViewModel _viewModel;

        public PdfInsertView()
        {
            InitializeComponent();
            _viewModel = new PdfInsertViewModel();
            DataContext = _viewModel;
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