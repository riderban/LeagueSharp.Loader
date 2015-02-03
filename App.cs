#region

using GalaSoft.MvvmLight.Threading;

#endregion

namespace LeagueSharp.Loader
{

    #region

    #endregion

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