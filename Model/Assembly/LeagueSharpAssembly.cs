using System.Collections.Generic;
using GalaSoft.MvvmLight;
using LeagueSharp.Loader.Core;
using LeagueSharp.Loader.Core.Compiler;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;

namespace LeagueSharp.Loader.Model.Assembly
{
    internal class LeagueSharpAssembly : ObservableObject
    {
        private string _author;
        private string _imageRating;
        private bool _inject;
        private string _location;
        private string _name;
        private OptimizationLevel _optimization = OptimizationLevel.Release;
        private OutputKind _outputKind = OutputKind.ConsoleApplication;
        private string _pathToBinary;
        private string _project;
        private AssemblyState _state;
        private AssemblyType _type;
        private int _version;
        private List<AssemblyVersion> _versions = new List<AssemblyVersion>();

        public string Author
        {
            get { return _author; }
            set { Set(() => Author, ref _author, value); }
        }

        [JsonIgnore]
        public AssemblyVersion CurrentVersion
        {
            get { return Versions[Version]; }
            set
            {
                Version = Versions.IndexOf(value);
                RaisePropertyChanged();
            }
        }

        public bool Inject
        {
            get { return _inject; }
            set { Set(() => Inject, ref _inject, value); }
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

        public OptimizationLevel Optimization
        {
            get { return _optimization; }
            set { Set(() => Optimization, ref _optimization, value); }
        }

        public OutputKind OutputKind
        {
            get { return _outputKind; }
            set { Set(() => OutputKind, ref _outputKind, value); }
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

        [JsonIgnore]
        public List<AssemblyVersion> Versions
        {
            get { return _versions; }
            set { Set(() => Versions, ref _versions, value); }
        }

        public override bool Equals(object obj)
        {
            var assembly = obj as LeagueSharpAssembly;
            if (assembly != null)
            {
                return assembly.Project == Project;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return Project.GetHashCode();
        }

        public void Update()
        {
            if (State == AssemblyState.Downloading)
            {
                return;
            }

            State = AssemblyState.Downloading;
            State = GitUpdater.Update(this) ? AssemblyState.Ready : AssemblyState.DownloadingError;
        }

        public void Compile()
        {
            if (State == AssemblyState.Compiling)
            {
                return;
            }

            State = AssemblyState.Compiling;
            State = RoslynCompiler.Compile(this) ? AssemblyState.Ready : AssemblyState.CompilingError;
        }
    }
}