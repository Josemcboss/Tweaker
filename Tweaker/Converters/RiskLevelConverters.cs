using System;
using System.Globalization;
using System.Windows.Data;
using System.Windows.Media;

using Tweaker.Models;

namespace Tweaker.Converters
{
    /// <summary>
    /// Convierte el nivel de riesgo de un tweak a un color visual
    /// Verde (Safe), Amarillo (Moderate), Rojo (Advanced)
    /// </summary>
    public class RiskLevelToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RiskLevel risk)
            {
                return risk switch
                {
                    RiskLevel.Safe => new SolidColorBrush(Color.FromRgb(76, 175, 80)),      // Verde Material Design
                    RiskLevel.Moderate => new SolidColorBrush(Color.FromRgb(255, 152, 0)),  // Naranja Material Design
                    RiskLevel.Advanced => new SolidColorBrush(Color.FromRgb(244, 67, 54)),  // Rojo Material Design
                    _ => new SolidColorBrush(Colors.Gray)
                };
            }
            return new SolidColorBrush(Colors.Gray);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("RiskLevelToBrushConverter solo soporta conversión unidireccional");
        }
    }

    /// <summary>
    /// Convierte el nivel de riesgo a un texto descriptivo
    /// </summary>
    public class RiskLevelToTextConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RiskLevel risk)
            {
                return risk switch
                {
                    RiskLevel.Safe => "Seguro",
                    RiskLevel.Moderate => "Moderado",
                    RiskLevel.Advanced => "Avanzado",
                    _ => "Desconocido"
                };
            }
            return "Desconocido";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Convierte el nivel de riesgo a un emoji/icono
    /// </summary>
    public class RiskLevelToIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is RiskLevel risk)
            {
                return risk switch
                {
                    RiskLevel.Safe => "?",      // Check mark
                    RiskLevel.Moderate => "?",  // Warning
                    RiskLevel.Advanced => "?",  // High voltage
                    _ => "?"
                };
            }
            return "?";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
