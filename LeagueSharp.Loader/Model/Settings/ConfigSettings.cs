using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace LeagueSharp.Loader.Model.Settings
{
    internal class ConfigSettings : ObservableObject
    {
        private ObservableCollection<GameSettings> _gameSettings;

        public ObservableCollection<GameSettings> GameSettings
        {
            get { return _gameSettings; }
            set { Set(() => GameSettings, ref _gameSettings, value); }
        }
    }
}