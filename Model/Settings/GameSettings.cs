using System.Collections.Generic;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;
using LeagueSharp.Loader.Core;

namespace LeagueSharp.Loader.Model.Settings
{
    internal class GameSettings : ObservableObject
    {
        private string _name;
        private List<string> _posibleValues;
        private string _selectedValue;

        [XmlIgnore]
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

        public List<string> PosibleValues
        {
            get { return _posibleValues; }
            set { Set(() => PosibleValues, ref _posibleValues, value); }
        }

        public string SelectedValue
        {
            get { return _selectedValue; }
            set { Set(() => SelectedValue, ref _selectedValue, value); }
        }
    }
}