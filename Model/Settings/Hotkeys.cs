using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace LeagueSharp.Loader.Model.Settings
{
    internal class Hotkeys : ObservableObject
    {
        private ObservableCollection<HotkeyEntry> _selectedHotkeys;

        public ObservableCollection<HotkeyEntry> SelectedHotkeys
        {
            get { return _selectedHotkeys; }
            set { Set(() => SelectedHotkeys, ref _selectedHotkeys, value); }
        }
    }
}