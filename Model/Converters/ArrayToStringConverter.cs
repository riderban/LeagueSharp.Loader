using System;
using System.Globalization;
using System.Windows.Data;

namespace LeagueSharp.Loader.Model.Converters
{
    internal class ArrayToStringConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return string.Join(", ", (string[]) value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return new string[0];
        }
    }
}