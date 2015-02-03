#region

using System.Collections.Generic;
using LeagueSharp.Loader.Model.Assembly;

#endregion

namespace LeagueSharp.Loader.Model.Settings
{

    #region

    #endregion

    public class Tag
    {
        public string Name { get; set; }
        public List<LeagueSharpAssembly> Assemblies { get; set; }
    }
}