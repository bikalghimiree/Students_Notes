using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Students_Notes.Converters
{
    public class BoolToImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "checkmark_green.png" : "sync_off.png";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 