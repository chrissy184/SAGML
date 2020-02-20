using System;
using System.Collections.Generic;

namespace ZMM.App.Clients.Repo
{
    public class ResourceCatelogEntry
    {
        public string Id { get; set; }
        public string Version { get; set; }
        public string Description { get; set; }
        public int Downloads { get; set; }
        public string Language { get; set; }
        public string LicenseUrl { get; set; }
        public bool HasReadme {get; set;}
        public IEnumerable<string> PackageTypes { get; set; }
        public IEnumerable<string> Tags { get; set; }
        public IEnumerable<ResourceDependencyGroups> DependencyGroups { get; set; }
        
        
    }
}
