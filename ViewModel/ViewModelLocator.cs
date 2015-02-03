#region LICENSE

// Copyright 2015-2015 LeagueSharp.Loader
// ViewModelLocator.cs is part of LeagueSharp.Loader.
// 
// LeagueSharp.Loader is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// LeagueSharp.Loader is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with LeagueSharp.Loader. If not, see <http://www.gnu.org/licenses/>.

#endregion

namespace LeagueSharp.Loader.ViewModel
{
    #region

    using GalaSoft.MvvmLight;
    using GalaSoft.MvvmLight.Ioc;
    using LeagueSharp.Loader.Design;
    using LeagueSharp.Loader.Model.Service;
    using Microsoft.Practices.ServiceLocation;

    #endregion

    /// <summary>
    ///     This class contains static references to all the view models in the
    ///     application and provides an entry point for the bindings.
    /// </summary>
    public class ViewModelLocator
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
        ///     Gets the Main property.
        /// </summary>
        public MainViewModel Main
        {
            get { return ServiceLocator.Current.GetInstance<MainViewModel>(); }
        }

        /// <summary>
        ///     Gets the Database property.
        /// </summary>
        public DatabaseViewModel Database
        {
            get { return ServiceLocator.Current.GetInstance<DatabaseViewModel>(); }
        }

        /// <summary>
        ///     Gets the AppBar property.
        /// </summary>
        public AppBarViewModel AppBar
        {
            get { return ServiceLocator.Current.GetInstance<AppBarViewModel>(); }
        }

        /// <summary>
        ///     Cleans up all the resources.
        /// </summary>
        public static void Cleanup() {}
    }
}