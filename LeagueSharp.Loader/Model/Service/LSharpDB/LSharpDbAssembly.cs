using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace LeagueSharp.Loader.Model.Service.LSharpDB
{
    internal class LSharpDbAssembly : ObservableObject
    {
        [JsonProperty(PropertyName = "champions")]
        public string[] Champions { get; set; }

        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "githubFolder")]
        public string GithubFolder { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }
    }
}