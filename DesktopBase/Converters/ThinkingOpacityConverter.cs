using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace DesktopBase.Converters;

public class ThinkingOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is bool isThinking)
        {
            return isThinking ? 0.7 : 1.0;
        }
        return 1.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}