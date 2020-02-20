using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZMM.App.Clients.Repo
{
    public class Resource
    {       
        public string Id { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<string> Authors { get; set; }
        public int TotalDownloads { get; set; }
     
    }
}