#region

using System.Collections.Generic;
using LeagueSharp.Loader.Model.Settings;

#endregion

namespace LeagueSharp.Loader.Model.Assembly
{

    #region

    #endregion

    public class LeagueSharpAssembly
    {
        public LeagueSharpAssembly()
        {
            State = AssemblyState.Unknown;
            Tags = new List<Tag>();
            Versions = new List<AssemblyVersion>();
        }

        public string Name { get; set; }
        public int Rating { get; set; }

        public string ImageRating
        {
            get { return "Rating" + Rating + ".png"; }
        }

        public AssemblyState State { get; set; }
        public AssemblyType Type { get; set; }
        public int Version { get; set; }

        public AssemblyVersion CurrentVersion
        {
            get { return Versions.Count >= Version && Version != 0 ? Versions[Version] : null; }
            set { Version = Versions.IndexOf(value); }
        }

        public List<AssemblyVersion> Versions { get; set; }
        public string Author { get; set; }
        public string Location { get; set; }
        public List<Tag> Tags { get; set; }
    }
}