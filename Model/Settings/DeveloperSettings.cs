using GalaSoft.MvvmLight;

namespace LeagueSharp.Loader.Model.Settings
{
    internal class DeveloperSettings : ObservableObject
    {
        private bool _ignoreUpdate;

        public bool IgnoreUpdate
        {
            get { return _ignoreUpdate; }
            set { _ignoreUpdate = value; }
        }
    }
}