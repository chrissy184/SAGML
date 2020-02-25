using System;
using System.Collections.Generic;

namespace ZMM.App.Clients.Repo
{
    public class ResourceItems
    {
        public int count { get; set; }
        public string lower { get; set; }
        public string upper { get; set; }
        public IEnumerable<ResourceItem> Items { get; set; }



    }
}
