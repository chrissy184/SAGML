using System;
using System.Collections.Generic;

namespace ZMM.Models.ResponseMessages
{
    public class ZSSettingResponse
    {
        public string ZmodId { get; set; }
        public string name { get; set; }
        public string type { get; set; }
        public string tenantID { get; set; }
        public string username { get; set; }
        public string password { get; set; }
        public string url { get; set; }
        public bool selected { get; set; }
    }    
}