using System;
using System.Globalization;
using System.Windows.Data;

namespace LyricsManager.Converter
{
    /// <summary>
    ///     Invertiert einen Boolean-Wert (True->False, False->True) 
    /// </summary>
    [ValueConversion(typeof(bool), typeof(bool))]
    public class InvertBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool booleanValue = (bool)value;
            return !booleanValue;
        }
    }
}
