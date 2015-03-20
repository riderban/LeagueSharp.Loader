using Newtonsoft.Json;

namespace LeagueSharp.Loader.Model.Service.LeagueSharp
{
    internal class UpdateInfo
    {
        [JsonProperty(PropertyName = "url")]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
    }
}