using System;
using System.Collections.Generic;

namespace ZMM.App.Clients.Repo
{
    public class ResourceInfo
    {
        public int Count { get; set; }
        public IEnumerable<ResourceItems> Items { get; set; }
    }
}
