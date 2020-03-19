
using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZMM.Models.ResponseMessages
{
    public class FilesInProgress
    {
        public string Id { get; set; }
        public string Type { get; set; }
        public string Name { get; set; }
        [JsonProperty("uploadStatus")] public string UploadStatus { get; set; }
        public string Module { get; set; }
        public DateTime CreatedAt {get;set;}
    }

    public class RootObject
    {
        public List<FilesInProgress> filesInProgress { get; set; }
    }
}