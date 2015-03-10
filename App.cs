using GalaSoft.MvvmLight.Threading;

namespace LeagueSharp.Loader
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        static App()
        {
            DispatcherHelper.Initialize();
        }
    }
}