using Avalonia.Controls;
using Avalonia.Interactivity;
using OfficeAssistant.ViewModels.PDF;

namespace OfficeAssistant.Views.PDF
{
    public partial class PdfDeleteView : UserControl
    {
        private readonly OfficeAssistant.ViewModels.PDF.PdfDeleteViewModel _viewModel;

        public PdfDeleteView()
        {
            InitializeComponent();
            _viewModel = new PdfDeleteViewModel();
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

        private void ClearAllFiles(object sender, RoutedEventArgs e)
        {
            _viewModel.ClearAllFiles();
        }

        private async void DeletePages(object sender, RoutedEventArgs e)
        {
            await _viewModel.DeletePages();
        }
    }
}
