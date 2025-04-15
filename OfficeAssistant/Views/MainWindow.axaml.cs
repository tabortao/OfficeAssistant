using Avalonia.Controls;

namespace OfficeAssistant.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void OnNavigationChanged(object? sender, SelectionChangedEventArgs e)
        {
            if (this.FindControl<ContentControl>("ContentArea") == null) return;

            var index = ((ListBox)sender!).SelectedIndex;
            switch (index)
            {
                case 0:
                    var contentArea = this.FindControl<ContentControl>("ContentArea");
                    if (contentArea != null)
                    {
                        contentArea.Content = new PdfMergeView();
                    }
                    break;
                case 1:
                    var contentArea2 = this.FindControl<ContentControl>("ContentArea");
                    if (contentArea2 != null)
                    {
                        contentArea2.Content = new PdfSplitView();
                    }
                    break;
            }
        }
    }
}