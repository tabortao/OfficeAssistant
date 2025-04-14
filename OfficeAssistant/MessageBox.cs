using Avalonia.Controls;
using System.Threading.Tasks;

namespace OfficeAssistant;

public class MessageBox
{
    public enum MessageBoxButtons
    {
        Ok
    }

    public static async Task Show(Window parent, string message, string title, MessageBoxButtons buttons)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 400,
            Height = 200,
            WindowStartupLocation = WindowStartupLocation.CenterOwner
        };

        var messageText = new TextBlock
        {
            Text = message,
            TextWrapping = Avalonia.Media.TextWrapping.Wrap,
            Margin = new Avalonia.Thickness(20)
        };

        var okButton = new Button
        {
            Content = "确定",
            HorizontalAlignment = Avalonia.Layout.HorizontalAlignment.Center,
            Width = 80,
            Margin = new Avalonia.Thickness(0, 10, 0, 20)
        };

        okButton.Click += (s, e) => dialog.Close();

        var panel = new StackPanel
        {
            Margin = new Avalonia.Thickness(10)
        };
        panel.Children.Add(messageText);
        panel.Children.Add(okButton);

        dialog.Content = panel;

        await dialog.ShowDialog(parent);
    }
}