using System;
using System.Globalization;
using System.Linq;
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
            return value.ToString().Split(',').Select(s => s.Trim()).ToArray();
        }
    }
}