using System.Windows;
using GalaSoft.MvvmLight;

namespace LeagueSharp.Loader.Model.Service
{
    internal class ProgressController : ObservableObject
    {
        private int _max;
        private int _min;
        private int _value;
        private Visibility _visibility = Visibility.Collapsed;

        public int Max
        {
            get { return _max; }
            set { Set(() => Max, ref _max, value); }
        }

        public int Min
        {
            get { return _min; }
            set { Set(() => Min, ref _min, value); }
        }

        public int Value
        {
            get { return _value; }
            set { Set(() => Value, ref _value, value); }
        }

        public Visibility Visibility
        {
            get { return _visibility; }
            set { Set(() => Visibility, ref _visibility, value); }
        }

        public bool Start(int value, int min, int max)
        {
            if (Visibility == Visibility.Visible)
            {
                return false;
            }

            Min = min;
            Max = max;
            Value = value;
            Visibility = Visibility.Visible;
            return true;
        }

        public void Stop()
        {
            Visibility = Visibility.Collapsed;
        }
    }
}