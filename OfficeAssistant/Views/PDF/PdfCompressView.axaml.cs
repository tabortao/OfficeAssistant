using Avalonia.Controls;
using Avalonia.Interactivity;
using OfficeAssistant.ViewModels.PDF;

namespace OfficeAssistant.Views.PDF
{
    public partial class PdfCompressView : UserControl
    {
        private readonly OfficeAssistant.ViewModels.PDF.PdfCompressViewModel _viewModel;

        public PdfCompressView()
        {
            InitializeComponent();
            _viewModel = new PdfCompressViewModel();
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

        private async void CompressFiles(object sender, RoutedEventArgs e)
        {
            await _viewModel.CompressFiles();
        }

        private void ClearAllFiles(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearAllFiles();
        }
    }
}