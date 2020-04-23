using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace ZMM.Models.ResponseMessages
{
    public class ModelResponse : BaseResponse
    {
        [JsonProperty("loaded")]
        public bool Loaded { get; set; }
        [JsonProperty("deployed")]
        public bool Deployed { get; set; }

        [JsonProperty("modelName")]
        public string ModelName { get; set; }

        [JsonProperty("dateCreated")]
        public DateTime DateCreated { get; set; }

        [JsonProperty("mleresponse")]
        public MleResponse MleResponse { get; set; }

        [JsonProperty("modelGeneratedFrom")]
        public string ModelGeneratedFrom { get; set; }

    }

    public class MleResponse
    {
        [JsonProperty("modelName")]
        public string MLEModelName { get; set; }

        [JsonProperty("modelVersion")]
        public long ModelVersion { get; set; }

        [JsonProperty("onnxVersion")]
        public long OnnxVersion { get; set; }

        [JsonProperty("producerName")]
        public string ProducerName { get; set; }

        [JsonProperty("producerVersion")]
        public string ProducerVersion { get; set; }

        [JsonProperty("inputs")]
        public string Inputs { get; set; }

        [JsonProperty("outputs")]
        public string Outputs { get; set; }
    }
}