using System.Collections.Generic;

namespace ZMM.Helpers.Common
{
    public class DeployedModels
    {
        public string id { get; set; }
    }

    public class RootDeployedModel
    {
        public List<DeployedModels> deployedModels { get; set; }
    }
}
