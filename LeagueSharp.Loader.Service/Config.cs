using System.Runtime.Serialization;

namespace LeagueSharp.Loader.Service
{
    [DataContract(Name = "http://joduska.me/v1/LoaderService")]
    public class Configuration
    {
        [DataMember]
        public string DataDirectory { get; set; }

        [DataMember]
        public uint ReloadKey { get; set; }

        [DataMember]
        public uint UnloadKey { get; set; }

        [DataMember]
        public uint ReloadAndRecompileKey { get; set; }

        [DataMember]
        public uint MenuKey { get; set; }

        [DataMember]
        public uint MenuToggleKey { get; set; }

        [DataMember]
        public bool AntiAfk { get; set; }

        [DataMember]
        public bool ExtendedZoom { get; set; }

        [DataMember]
        public bool TowerRange { get; set; }

        [DataMember]
        public bool Console { get; set; }
    }
}