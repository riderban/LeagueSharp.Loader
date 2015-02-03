#region

using System.Windows;
using GalaSoft.MvvmLight;
using LeagueSharp.Loader.View;

#endregion

namespace LeagueSharp.Loader.ViewModel
{

    #region

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