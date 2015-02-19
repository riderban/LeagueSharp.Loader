using System.Collections.ObjectModel;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;

namespace LeagueSharp.Loader.Model.Settings
{
    [XmlType(AnonymousType = true)]
    internal class ConfigSettings : ObservableObject
    {
        private ObservableCollection<GameSettings> _gameSettings;

        [XmlArrayItem("GameSettings", IsNullable = true)]
        public ObservableCollection<GameSettings> GameSettings
        {
            get { return _gameSettings; }
            set { Set(() => GameSettings, ref _gameSettings, value); }
        }
    }
}