using Avalonia.Controls;
using OfficeAssistant.Views;

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
            if (ContentArea == null) return;
            
            var index = ((ListBox)sender!).SelectedIndex;
            switch (index)
            {
                case 0:
                    ContentArea.Content = new PdfMergeView();
                    break;
                case 1:
                    ContentArea.Content = new PdfSplitView();
                    break;
            }
        }
    }
}