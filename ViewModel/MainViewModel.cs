#region LICENSE

// Copyright 2015-2015 LeagueSharp.Loader
// MainViewModel.cs is part of LeagueSharp.Loader.
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

    using System.Windows;
    using GalaSoft.MvvmLight;
    using LeagueSharp.Loader.View;

    #endregion

    public class MainViewModel : ViewModelBase
    {
        /// <summary>
        ///     The <see cref="CurrentView" /> property's name.
        /// </summary>
        public const string CurrentViewPropertyName = "CurrentView";

        /// <summary>
        ///     The <see cref="CurrentAppBarView" /> property's name.
        /// </summary>
        public const string CurrentAppBarViewPropertyName = "CurrentAppBarView";

        private FrameworkElement _currentAppBarViewElement;
        private FrameworkElement _currentView;

        /// <summary>
        ///     Initializes a new instance of the MainViewModel class.
        /// </summary>
        public MainViewModel()
        {
            CurrentView = new DatabaseView();
            CurrentAppBarView = new AppBarView();
        }

        /// <summary>
        ///     Sets and gets the CurrentAppBarView property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public FrameworkElement CurrentAppBarView
        {
            get { return _currentAppBarViewElement; }

            set
            {
                if (_currentAppBarViewElement == value)
                {
                    return;
                }

                _currentAppBarViewElement = value;
                RaisePropertyChanged(() => CurrentAppBarView);
            }
        }

        /// <summary>
        ///     Sets and gets the CurrentView property.
        ///     Changes to that property's value raise the PropertyChanged event.
        /// </summary>
        public FrameworkElement CurrentView
        {
            get { return _currentView; }

            set
            {
                if (_currentView == value)
                {
                    return;
                }

                _currentView = value;
                RaisePropertyChanged(() => CurrentView);
            }
        }
    }
}