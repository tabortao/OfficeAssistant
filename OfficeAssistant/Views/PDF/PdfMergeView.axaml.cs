using Avalonia.Controls;
using Avalonia.Interactivity;
using OfficeAssistant.ViewModels.PDF;

namespace OfficeAssistant.Views.PDF
{
    public partial class PdfMergeView : UserControl
    {
        private readonly OfficeAssistant.ViewModels.PDF.PdfMergeViewModel _viewModel;

        public PdfMergeView()
        {
            InitializeComponent();
            _viewModel = new PdfMergeViewModel();
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

        private async void MergeFiles(object sender, RoutedEventArgs e)
        {
            await _viewModel.MergeFiles();
        }

        private void ClearAllFiles(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearAllFiles();
        }
    }
}