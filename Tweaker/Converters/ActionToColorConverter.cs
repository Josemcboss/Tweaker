using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Tweaker.Converters;

[ValueConversion(typeof(string), typeof(SolidColorBrush))]
public sealed class ActionToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return value?.ToString() == "ON"
            ? new SolidColorBrush(Color.FromRgb(0x4C, 0xAF, 0x50))
            : new SolidColorBrush(Color.FromRgb(0xF4, 0x43, 0x36));
    }

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
