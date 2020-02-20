using Newtonsoft.Json;

namespace ZMM.App.Clients.Repo
{
    public class PackageVersion
    {
        [JsonProperty("@id")]
        public string Id { get; set; }
        public string Version { get; set; }
        public int Downloads { get; set; }
    }
}