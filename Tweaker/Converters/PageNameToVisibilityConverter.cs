using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Tweaker.Converters
{
    public class PageNameToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string currentPage && parameter is string targetPage)
            {
                return currentPage.Equals(targetPage, StringComparison.OrdinalIgnoreCase) 
                    ? Visibility.Visible 
                    : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
