using GalaSoft.MvvmLight;
using Newtonsoft.Json;

namespace LeagueSharp.Loader.Model.Service.LSharpDB
{
    internal class LSharpDbAssemblyRepository
    {
        [JsonProperty(PropertyName = "count")]
        public int Count { get; set; }

        [JsonProperty(PropertyName = "githubFolder")]
        public string GithubFolder { get; set; }

        [JsonProperty(PropertyName = "name")]
        public string Name { get; set; }

        [JsonProperty(PropertyName = "champions")]
        public string[] Champions { get; set; }
    }

    internal class LSharpDbAssembly : ObservableObject
    {
        public int Count { get; set; }
        public string GithubFolder { get; set; }
        public string Name { get; set; }
        public string Champion { get; set; }
    }
}