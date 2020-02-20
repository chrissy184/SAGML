using System.Collections.Generic;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using System.Runtime.Serialization;

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

        public string Registration { get; set; }

        public IEnumerable<PackageVersion> Versions { get; set; }
     
    }
}