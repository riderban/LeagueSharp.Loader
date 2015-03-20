using System.Runtime.Serialization;

namespace LeagueSharp.Loader.Service
{
    [DataContract(Name = "http://joduska.me/v1/LoaderService")]
    public class LoginCredentials
    {
        [DataMember]
        public string User { get; set; }

        [DataMember]
        public string Password { get; set; }
    }
}