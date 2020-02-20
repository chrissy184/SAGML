using System;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ZMM.App.Clients.Repo
{
    public interface IRepoClient
    {
        Task<IRepoResponse> Get();
        Task<IRepoResponse> Get(string ResourceId);
        Task<IRepoResponse> GetModels();
        Task<IRepoResponse> GetData();
        Task<IRepoResponse> GetCode();
        Task<IRepoResponse> Query(string QueryString);
        Task<IRepoResponse> Add(Resource ResourceInfo);
        Task<IRepoResponse> Delete(Resource ResourceInfo);
        Task<IRepoResponse> Publish(Resource ResourceInfo);
        Task<IRepoResponse> Compress();
        Task<IRepoResponse> Deploy();

    }
}