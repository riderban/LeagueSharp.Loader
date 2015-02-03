#region

using System.Collections.ObjectModel;
using GalaSoft.MvvmLight;

#endregion

namespace LeagueSharp.Loader.Model.Log
{
    public class Log : ObservableObject
    {
        private ObservableCollection<LogItem> _items = new ObservableCollection<LogItem>();

        public ObservableCollection<LogItem> Items
        {
            get { return _items; }
            set { Set(() => Items, ref _items, value); }
        }
    }
}