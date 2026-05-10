using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tweaker.Converters
{
    /// <summary>
    /// Convierte int > 0 a true/false para habilitar botones
    /// </summary>
    public class GreaterThanZeroConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is int number)
            {
                return number > 0;
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
