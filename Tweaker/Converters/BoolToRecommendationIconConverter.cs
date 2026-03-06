using System;
using System.Globalization;
using System.Windows.Data;

namespace Tweaker.Converters
{
    /// <summary>
    /// Converter para mostrar iconos de recomendación basados en si el tweak es recomendado o no
    /// </summary>
    public class BoolToRecommendationIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool recommended)
            {
                return recommended ? "?" : "??";
            }
            return "??";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
