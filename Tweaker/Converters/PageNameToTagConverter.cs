using System;
using System.Globalization;
using System.Windows.Data;

namespace Tweaker.Converters
{
    public class PageNameToTagConverter : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string currentPage && parameter is string targetPage)
            {
                return currentPage.Equals(targetPage, StringComparison.OrdinalIgnoreCase) 
                    ? "Active" 
                    : null;
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
