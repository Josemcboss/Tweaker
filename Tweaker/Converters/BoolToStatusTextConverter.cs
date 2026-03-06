using System;
using System.Globalization;
using System.Windows.Data;

namespace Tweaker.Converters
{
    /// <summary>
    /// Converter para mostrar "ON" o "OFF" según el estado del CheckBox
    /// </summary>
    public class BoolToStatusTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked)
            {
                return isChecked ? "ON" : "OFF";
            }
            return "OFF";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
