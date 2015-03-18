using System;
using System.Globalization;
using System.Windows.Data;
using LeagueSharp.Loader.Model.Settings;

namespace LeagueSharp.Loader.Model.Converters
{
    internal class IntToProfileConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Config.Instance.SelectedProfile;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return Config.Instance.Profiles.IndexOf((Profile) value);
        }
    }
}