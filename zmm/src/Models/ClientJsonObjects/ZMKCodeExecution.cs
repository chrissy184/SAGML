using System;
using System.Collections.Generic;
using System.Text.Json;


namespace ZMM.Models.ClientJsonObjects.ZMKCodeExecution
{
    public class Information
    {
        public string property { get; set; }
        public string value { get; set; }
    }

    public class RunningTask
    {
        public string idforData { get; set; }
        public string status { get; set; }
        public string createdOn { get; set; }
        public string type { get; set; }
        public long pid { get; set; }
        public string newPMMLFileName { get; set; }
        public List<Information> information { get; set; }
        public string taskName { get; set; }       
        public string completedOn { get; set; }
    }

    public class RootObject
    {
        public List<RunningTask> runningTask { get; set; }
    }
}
