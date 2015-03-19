using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using GalaSoft.MvvmLight;
using LeagueSharp.Loader.Core;
using LeagueSharp.Loader.Core.Compiler;
using LeagueSharp.Loader.Model.Settings;
using Microsoft.CodeAnalysis;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LeagueSharp.Loader.Model.Assembly
{
    internal class LeagueSharpAssembly : ObservableObject
    {
        private string _author;
        private bool _inject;
        private string _location;
        private string _name;
        private OptimizationLevel _optimization = OptimizationLevel.Release;
        private OutputKind _outputKind = OutputKind.DynamicallyLinkedLibrary;
        private string _pathToBinary;
        private string _pathToRepository;
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

        [JsonConverter(typeof (StringEnumConverter))]
        public OptimizationLevel Optimization
        {
            get { return _optimization; }
            set { Set(() => Optimization, ref _optimization, value); }
        }

        [JsonConverter(typeof (StringEnumConverter))]
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

        public string PathToRepository
        {
            get
            {
                if (string.IsNullOrEmpty(_pathToRepository) && Location.StartsWith("http"))
                {
                    var match = Regex.Match(Location, @"^(http[s]?)://(?<host>.*?)/(?<author>.*?)/(?<repo>.*?)(/{1}|$)");
                    PathToRepository = Path.Combine(Directories.RepositoryDirectory, match.Groups["host"].Value,
                        match.Groups["author"].Value, match.Groups["repo"].Value);
                }

                return _pathToRepository;
            }
            set { Set(() => PathToRepository, ref _pathToRepository, value); }
        }

        public string Project
        {
            get { return _project; }
            set { Set(() => Project, ref _project, value); }
        }

        [JsonIgnore]
        public AssemblyState State
        {
            get { return _state; }
            set { Set(() => State, ref _state, value); }
        }

        [JsonConverter(typeof (StringEnumConverter))]
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

        //public override bool Equals(object obj)
        //{
        //    var assembly = obj as LeagueSharpAssembly;
        //    if (assembly != null)
        //    {
        //        return assembly.GetHashCode() == Project.GetHashCode();
        //    }
        //    return false;
        //}

        //public override int GetHashCode()
        //{
        //    return Project != null ? Project.GetHashCode() : (Name + Location).GetHashCode();
        //}

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