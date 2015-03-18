using System.Collections.Generic;
using GalaSoft.MvvmLight;
using LeagueSharp.Loader.Core;
using Newtonsoft.Json;

namespace LeagueSharp.Loader.Model.Settings
{
    internal class GameSettings : ObservableObject
    {
        private string _name;
        private string _value;
        private List<string> _values;

        [JsonIgnore]
        public string DisplayName
        {
            get { return Utility.GetMultiLanguageText(Name); }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    RaisePropertyChanged();
                    RaisePropertyChanged("DisplayName");
                }
            }
        }

        public string Value
        {
            get { return _value; }
            set { Set(() => Value, ref _value, value); }
        }

        public List<string> Values
        {
            get { return _values; }
            set { Set(() => Values, ref _values, value); }
        }
    }
}