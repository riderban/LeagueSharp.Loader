using System.Linq;
using GalaSoft.MvvmLight.Threading;
using LeagueSharp.Loader.Core.Compiler;
using MahApps.Metro;

namespace LeagueSharp.Loader
{

    #region

    #endregion

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

            //NuGetResolver.Resolve(@"D:\GitHub\LeagueSharp.Loader\packages.config");
        }
    }
}