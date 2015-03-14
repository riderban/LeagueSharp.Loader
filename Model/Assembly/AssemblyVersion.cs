using System;
using GalaSoft.MvvmLight;

namespace LeagueSharp.Loader.Model.Assembly
{
    internal class AssemblyVersion : ObservableObject
    {
        private string _color;
        private DateTimeOffset _date;
        private int _id;
        private string _message;
        private string _hash;

        public string Color
        {
            get { return _color; }
            set { Set(() => Color, ref _color, value); }
        }

        public DateTimeOffset Date
        {
            get { return _date; }
            set { Set(() => Date, ref _date, value); }
        }

        public string Hash
        {
            get { return _hash; }
            set { Set(() => Hash, ref _hash, value); }
        }

        public int Id
        {
            get { return _id; }
            set { Set(() => Id, ref _id, value); }
        }

        public string Message
        {
            get { return _message; }
            set { Set(() => Message, ref _message, value); }
        }

        public AssemblyVersion()
        {
            Color = "Green";
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} >> {2}", Id, Date, Message);
        }
    }
}