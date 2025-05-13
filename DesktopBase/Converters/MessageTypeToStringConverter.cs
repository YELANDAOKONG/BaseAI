using System;
using System.Globalization;
using Avalonia.Data.Converters;
using DesktopBase.Models;

namespace DesktopBase.Converters;

public class MessageTypeToStringConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is MessageType messageType)
        {
            return messageType switch
            {
                MessageType.User => "You",
                MessageType.AI => "AI",
                MessageType.System => "System",
                _ => string.Empty
            };
        }
        return string.Empty;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}