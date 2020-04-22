using System.Threading.Tasks;


namespace ZMM.App.MLEngineService
{
    public interface IOnnxClient
    {
        Task<string> GetAllModel(string zmodId); 
        Task<string> GetModelInfo(string zmodId);
        Task<string> DeployModelAsync(string zmodId, string filePath);
        Task<string> RemoveModelAsync(string zmodId);

    }
}