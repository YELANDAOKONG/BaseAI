using Avalonia.Data.Converters;
using System;
using System.Globalization;
using DesktopBase.Models;

namespace DesktopBase.Converters;

public class MessageTypeToClassConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is MessageType messageType && parameter is string themeClass)
        {
            return messageType switch
            {
                MessageType.User => $"userMessage {themeClass}",
                MessageType.AI => $"aiMessage {themeClass}",
                _ => themeClass
            };
        }
        return "light";
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}