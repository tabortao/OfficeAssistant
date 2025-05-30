using Avalonia.Controls;
using Avalonia.Interactivity;
using OfficeAssistant.ViewModels.PDF;

namespace OfficeAssistant.Views.PDF
{
    public partial class PdfImageView : UserControl
    {
        private readonly PdfImageViewModel _viewModel;

        public PdfImageView()
        {
            InitializeComponent();
            _viewModel = new PdfImageViewModel();
            DataContext = _viewModel;

            // 在控件加载完成后设置StorageProvider
            this.Loaded += (s, e) =>
            {
                _viewModel.StorageProvider = TopLevel.GetTopLevel(this)?.StorageProvider;
            };
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

        private async void ConvertToImages(object sender, RoutedEventArgs e)
        {
            await _viewModel.ConvertToImages();
        }
    }
}
