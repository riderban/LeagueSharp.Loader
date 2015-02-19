using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

namespace LeagueSharp.Loader.Model.Log
{
    internal class Log : ObservableObject
    {
        private ObservableCollection<LogItem> _items = new ObservableCollection<LogItem>();

        public ObservableCollection<LogItem> Items
        {
            get { return _items; }
            set { Set(() => Items, ref _items, value); }
        }
    }
}