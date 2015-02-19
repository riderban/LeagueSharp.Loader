using System.Collections.ObjectModel;
using System.Xml.Serialization;
using GalaSoft.MvvmLight;

namespace LeagueSharp.Loader.Model.Settings
{
    [XmlType(AnonymousType = true)]
    public class Hotkeys : ObservableObject
    {
        private ObservableCollection<HotkeyEntry> _selectedHotkeys;

        [XmlArrayItem("SelectedHotkeys", IsNullable = true)]
        public ObservableCollection<HotkeyEntry> SelectedHotkeys
        {
            get { return _selectedHotkeys; }
            set { Set(() => SelectedHotkeys, ref _selectedHotkeys, value); }
        }
    }
}