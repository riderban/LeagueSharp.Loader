using GalaSoft.MvvmLight;
using log4net.Core;

namespace LeagueSharp.Loader.Model.Log
{
    internal class LogItem : ObservableObject
    {
        private Level _level;
        private string _message;
        private string _source;

        public Level Level
        {
            get { return _level; }
            set { Set(() => Level, ref _level, value); }
        }

        public string Message
        {
            get { return _message; }
            set { Set(() => Message, ref _message, value); }
        }

        public string Source
        {
            get { return _source; }
            set { Set(() => Source, ref _source, value); }
        }
    }
}