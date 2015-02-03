#region LICENSE

// Copyright 2015-2015 LeagueSharp.Loader
// LeagueSharpAssembly.cs is part of LeagueSharp.Loader.
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

namespace LeagueSharp.Loader.Model
{
    #region

    using System.Collections.Generic;

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
            get { return Versions[Version]; }
            set { Version = Versions.IndexOf(value); }
        }

        public List<AssemblyVersion> Versions { get; set; }
        public string Author { get; set; }
        public string Location { get; set; }
        public List<Tag> Tags { get; set; }
    }
}