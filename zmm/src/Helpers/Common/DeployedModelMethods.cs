using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ZMM.Helpers.ZMMDirectory;
using ZMM.Models.ResponseMessages;
using ZMM.Models.Payloads;

namespace ZMM.Helpers.Common
{
    public static class DeployedModelFunctions
    {
       private static string dirFullpath = DirectoryHelper.GetZSDeployedModelDirectoryPath();
          
        public static bool CreateUpdateJSONFile(string fileName, string modelName)
        {
            bool status = true;
            //string dirFullpath = DirectoryHelper.GetZSDeployedModelDirectoryPath();
            fileName = $"{dirFullpath}{fileName}";
            
            if (String.IsNullOrEmpty(modelName))
                return status = false;
            else
            {
                DeployedModels deployedModel = new DeployedModels();
                deployedModel.id = modelName;
                if (File.Exists(fileName))
                {
                    var jo = Newtonsoft.Json.JsonConvert.DeserializeObject<RootDeployedModel>(File.ReadAllText(fileName));
                    if (jo.deployedModels.ToList().Where(c => c.id.Contains(deployedModel.id)).Count() == 0)
                    {
                        jo.deployedModels.Add(deployedModel);
                        var JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jo);
                        File.WriteAllText(fileName, JsonString);
                    }
                }
                else
                {
                    using (var tw = new StreamWriter(fileName, true))
                    {
                        tw.Close();
                    }
                    RootDeployedModel modrootDeployedModel = new RootDeployedModel();
                    modrootDeployedModel.deployedModels = new List<DeployedModels>();
                    modrootDeployedModel.deployedModels.Add(deployedModel);
                    var JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(modrootDeployedModel);
                    File.WriteAllText(fileName, JsonString);
                }
            }

            return status;
        }
        public static bool DeleteDeployedModel(string fileName, string modelName)
        {
            bool status = true;
            fileName = $"{dirFullpath}{fileName}";
            try
            {
                if (File.Exists(fileName))
                {
                    var jo = Newtonsoft.Json.JsonConvert.DeserializeObject<RootDeployedModel>(File.ReadAllText(fileName));
                    if (jo.deployedModels.ToList().Where(c => c.id.Contains(modelName)).Count() > 0)
                    {
                        var item = jo.deployedModels.Single(x => x.id.Contains(modelName));
                        jo.deployedModels.Remove(item);
                        var JsonString = Newtonsoft.Json.JsonConvert.SerializeObject(jo);
                        File.WriteAllText(fileName, JsonString);
                    }
                }
                else
                {
                    return status = false;
                }
                return status;
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error while deleting data.. " + ex.StackTrace);
                return status = false;
            }
        }
    
    public static bool GetDeployedModel(string fileName, List<ModelResponse> modelsList)
    {
         bool status = true;
            fileName = $"{dirFullpath}{fileName}";
            try
            {
                if (File.Exists(fileName))
                {
                    var jo = Newtonsoft.Json.JsonConvert.DeserializeObject<RootDeployedModel>(File.ReadAllText(fileName));
                    if (jo.deployedModels.ToList().Count() > 0)
                    {
                        foreach(var model in modelsList)
                        {
                            foreach(var item in jo.deployedModels)
                            {
                                if(model.Id == item.id)
                                { model.Deployed = true;
                                   ModelPayload.Update(model);
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error while deleting data.. " + ex.StackTrace);
                return status = false;
            }

        return status;
    }
    }
}