using System.Collections.Generic;

namespace ZMM.App.Clients.Repo
{
    public class Resources
    {
        public int TotalHits { get; set; }
        public IEnumerable<Resource> Data { get; set; }
    }
}