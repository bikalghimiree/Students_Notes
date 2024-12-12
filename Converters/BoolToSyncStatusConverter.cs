using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace Students_Notes.Converters
{
    public class BoolToSyncStatusConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "All notes are synced and up to date" : "Sync is currently offline";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
} 