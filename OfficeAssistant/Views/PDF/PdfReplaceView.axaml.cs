using Avalonia.Controls;
using Avalonia.Interactivity;
using OfficeAssistant.ViewModels.PDF;

namespace OfficeAssistant.Views.PDF
{
    public partial class PdfReplaceView : UserControl
    {
        private readonly OfficeAssistant.ViewModels.PDF.PdfReplaceViewModel _viewModel;

        public PdfReplaceView()
        {
            InitializeComponent();
            _viewModel = new PdfReplaceViewModel();
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

        private async void SelectReplacementFiles(object sender, RoutedEventArgs e)
        {
            await _viewModel.SelectReplacementFiles();
        }

        private void RemoveReplacementFile(object sender, RoutedEventArgs e)
        {
            if (e.Source is Button button && button.CommandParameter is string file)
            {
                _viewModel.RemoveReplacementFile(file);
            }
        }

        private void ClearReplacementFiles(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearReplacementFiles();
        }

        private async void ReplacePages(object sender, RoutedEventArgs e)
        {
            await _viewModel.ReplacePages();
        }
    }
}