using System;
using System.Threading.Tasks;

namespace ZMM.App.ZSServiceClient
{
    public interface IZSModelPredictionClient
    {
        Task<string> GetModels();
        Task<string> UploadPmml(string dirFullPath);
        Task<string> DeletePmml(string modelName);
        Task<string> SingleScoring(string modelName,string record); 
        Task<string> MultipleScoring(string modelName,string record); 
        Task<string> ImageScoring(string modelName,string record);
    }
}