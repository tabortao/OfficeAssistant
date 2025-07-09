using System;
using Avalonia.Data.Converters;
using System.Globalization;

namespace OfficeAssistant.Converters
{
    public class ListBoxMaxHeightConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            // 让ListBox最大高度为UserControl实际高度的40%，最小200，最大600
            if (value is double actualHeight)
            {
                var h = Math.Max(200, Math.Min(600, actualHeight * 0.4));
                return h;
            }
            return 300;
        }

        public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
