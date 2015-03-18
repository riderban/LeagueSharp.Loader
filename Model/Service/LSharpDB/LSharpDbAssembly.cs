using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace LeagueSharp.Loader.Model.Service.LSharpDB
{
    internal class LSharpDbAssemblyRepository
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

    internal class LSharpDbAssembly : ObservableObject
    {
        public string Champion { get; set; }
        public int Count { get; set; }
        public string GithubFolder { get; set; }
        public string Name { get; set; }
    }
}