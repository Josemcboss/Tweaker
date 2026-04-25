using System;
using System.Globalization;
using System.Windows.Data;

namespace Tweaker.Converters;

[ValueConversion(typeof(bool), typeof(string))]
public sealed class BoolToStringConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        string[]? parts = parameter?.ToString()?.Split('|');
        if (parts is null || parts.Length != 2)
        {
            return value?.ToString() ?? string.Empty;
        }

        return value is bool isTrue && isTrue ? parts[0] : parts[1];
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
