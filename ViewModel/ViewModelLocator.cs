using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Ioc;
using LeagueSharp.Loader.Design;
using LeagueSharp.Loader.Model.Service;
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
            SimpleIoc.Default.Register<DatabaseViewModel>();
            SimpleIoc.Default.Register<AppBarViewModel>();
        }

        /// <summary>
        ///     Gets the AppBar property.
        /// </summary>
        public AppBarViewModel AppBar
        {
            get { return ServiceLocator.Current.GetInstance<AppBarViewModel>(); }
        }

        /// <summary>
        ///     Gets the Database property.
        /// </summary>
        public DatabaseViewModel Database
        {
            get { return ServiceLocator.Current.GetInstance<DatabaseViewModel>(); }
        }

        /// <summary>
        ///     Gets the Main property.
        /// </summary>
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        /// <summary>
        ///     Cleans up all the resources.
        /// </summary>
        public static void Cleanup()
        {
        }
    }
}