using GalaSoft.MvvmLight;

namespace LeagueSharp.Loader.Model.Settings
{
    internal class DeveloperSettings : ObservableObject
    {
        public bool IgnoreUpdate { get; set; }
    }
}