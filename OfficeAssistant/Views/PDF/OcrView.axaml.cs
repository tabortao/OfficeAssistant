using Avalonia.Controls;
using Avalonia.Interactivity;
using System.Diagnostics;

namespace OfficeAssistant.Views.PDF;

public partial class OcrView : UserControl
{
    public OcrView()
    {
        InitializeComponent();
    }

    private void OnGitHubLinkClick(object? sender, RoutedEventArgs e)
    {
        var url = "https://github.com/tabortao/PeachOCR";
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
