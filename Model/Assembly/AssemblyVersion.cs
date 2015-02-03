#region

using System;

#endregion

namespace LeagueSharp.Loader.Model.Assembly
{

    #region

    #endregion

    public class AssemblyVersion
    {
        public AssemblyVersion()
        {
            Color = "Green";
        }

        public int Id { get; set; }
        public DateTimeOffset Date { get; set; }
        public string Message { get; set; }
        public string Color { get; set; }
    }
}