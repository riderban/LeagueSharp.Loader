using System.Collections.Generic;
using GalaSoft.MvvmLight;

namespace LeagueSharp.Loader.Model.Assembly
{
    internal class LeagueSharpAssembly : ObservableObject
    {
        private string _author;
        private string _imageRating;
        private string _location;
        private string _name;
        private string _pathToBinary;
        private string _project;
        private int _rating;
        private AssemblyState _state;
        private AssemblyType _type;
        private int _version;
        private List<AssemblyVersion> _versions = new List<AssemblyVersion>();

        public string Author
        {
            get { return _author; }
            set { Set(() => Author, ref _author, value); }
        }

        public AssemblyVersion CurrentVersion
        {
            get { return Versions[Version]; }
            set
            {
                Version = Versions.IndexOf(value);
                RaisePropertyChanged();
            }
        }

        public string ImageRating
        {
            get { return _imageRating; }
            set { Set(() => ImageRating, ref _imageRating, value); }
        }

        public string Location
        {
            get { return _location; }
            set { Set(() => Location, ref _location, value); }
        }

        public string Name
        {
            get { return _name; }
            set { Set(() => Name, ref _name, value); }
        }

        public string PathToBinary
        {
            get { return _pathToBinary; }
            set { Set(() => PathToBinary, ref _pathToBinary, value); }
        }

        public string Project
        {
            get { return _project; }
            set { Set(() => Project, ref _project, value); }
        }

        public int Rating
        {
            get { return _rating; }
            set { Set(() => Rating, ref _rating, value); }
        }

        public AssemblyState State
        {
            get { return _state; }
            set { Set(() => State, ref _state, value); }
        }

        public AssemblyType Type
        {
            get { return _type; }
            set { Set(() => Type, ref _type, value); }
        }

        public int Version
        {
            get { return _version; }
            set
            {
                Set(() => Version, ref _version, value);
                RaisePropertyChanged("CurrentVersion");
            }
        }

        public List<AssemblyVersion> Versions
        {
            get { return _versions; }
            set { Set(() => Versions, ref _versions, value); }
        }
    }
}