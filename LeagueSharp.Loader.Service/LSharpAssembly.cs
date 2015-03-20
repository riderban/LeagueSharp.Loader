using System.Runtime.Serialization;
using System.Security.Permissions;

namespace LeagueSharp.Loader.Service
{
    [DataContract]
    public class LSharpAssembly
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string PathToBinary { get; set; }

        [DataMember]
        public PermissionState PermissionState { get; set; }
    }
}