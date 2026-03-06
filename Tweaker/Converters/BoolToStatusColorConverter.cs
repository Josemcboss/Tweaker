using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

namespace Tweaker.Converters
{
    /// <summary>
    /// Converter para cambiar el color del texto según el estado del CheckBox
    /// Verde (#0E7A0D) cuando está ON, Rojo (#E74C3C) cuando está OFF
    /// </summary>
    public class BoolToStatusColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool isChecked)
            {
                // Verde para ON, Rojo para OFF
                return isChecked
                    ? new SolidColorBrush(Color.FromRgb(14, 122, 13))   // #0E7A0D (Verde)
                    : new SolidColorBrush(Color.FromRgb(231, 76, 60));  // #E74C3C (Rojo)
            }
            // Default: Gris
            return new SolidColorBrush(Color.FromRgb(160, 160, 160)); // #A0A0A0
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
