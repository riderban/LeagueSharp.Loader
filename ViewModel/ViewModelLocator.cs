using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LeagueSharp.Loader.Design;
using LeagueSharp.Loader.Model.Service;
using LeagueSharp.Loader.ViewModel.Settings;
using Microsoft.Practices.ServiceLocation;

namespace LeagueSharp.Loader.ViewModel
{
    /// <summary>
    ///     This class contains static references to all the view models in the
    ///     application and provides an entry point for the bindings.
    /// </summary>
    internal class ViewModelLocator
    {
        static ViewModelLocator()
        {
            ServiceLocator.SetLocatorProvider(() => SimpleIoc.Default);

            if (ViewModelBase.IsInDesignModeStatic)
            {
                SimpleIoc.Default.Register<IDataService, DesignDataService>();
            }
            else
            {
                SimpleIoc.Default.Register<IDataService, AssemblyDatabaseService>();
            }

            SimpleIoc.Default.Register<MainViewModel>();
            SimpleIoc.Default.Register<AppBarViewModel>();
            SimpleIoc.Default.Register<DatabaseViewModel>();
            SimpleIoc.Default.Register<AssembliesViewModel>();
            SimpleIoc.Default.Register<InstallerViewModel>();
            SimpleIoc.Default.Register<UpdaterViewModel>();
            SimpleIoc.Default.Register<GeneralViewModel>();
            SimpleIoc.Default.Register<HotkeysViewModel>();
            SimpleIoc.Default.Register<LogViewModel>();
            SimpleIoc.Default.Register<MenuConfigViewModel>();
        }

        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        public AppBarViewModel AppBar
        {
            get { return ServiceLocator.Current.GetInstance<AppBarViewModel>(); }
        }

        public DatabaseViewModel Database
        {
            get { return ServiceLocator.Current.GetInstance<DatabaseViewModel>(); }
        }

        public AssembliesViewModel Assemblies
        {
            get { return ServiceLocator.Current.GetInstance<AssembliesViewModel>(); }
        }

        /// <summary>
        ///     Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}