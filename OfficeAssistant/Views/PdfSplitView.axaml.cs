using Avalonia.Controls;
using Avalonia.Interactivity;
using OfficeAssistant.ViewModels;

namespace OfficeAssistant.Views
{
    public partial class PdfSplitView : UserControl
    {
        private readonly PdfSplitViewModel _viewModel;

        public PdfSplitView()
        {
            InitializeComponent();
            _viewModel = new PdfSplitViewModel();
            DataContext = _viewModel;
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
    }
}