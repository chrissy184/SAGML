
using System;
using System.Collections.Generic;

namespace ZMM.Models.ResponseMessages
{
     public class Data
    {
        public int position { get; set; }
        public string variable { get; set; }
        public string dtype { get; set; }
        public int missing_val { get; set; }
        public string changedataType { get; set; }
        public string imputation_method { get; set; }
        public string data_transformation_step { get; set; }
        public bool use_for_model { get; set; }
    } 

     public class Parameters
    {
        public int generation { get; set; }
        public int population_size { get; set; }
        public string model_name { get; set; }
        public string scoring { get; set; }
        public List<string> algorithm { get; set; }
    }
 
    public class AutoMLResponse
    {
        public DateTime executedAt { get; set; }
        public List<Data> data { get; set; }
        public string problem_type { get; set; }
        public string target_variable { get; set; }
        public string status { get; set; }
        public string idforData { get; set; }
        public string newPMMLFileName { get; set; }
        public string filePath { get; set; }
        public Parameters parameters { get; set; }
    }
}