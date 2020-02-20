using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ZMM.App.Clients.Repo
{
    public interface IRepoClient
    {
        Task<IEnumerable<Package>> Get();
        Task<IRepoResponse> Get(string ResourceId);
        Task<IRepoResponse> GetModels();
        Task<IRepoResponse> GetData();
        Task<IRepoResponse> GetCode();
        Task<IRepoResponse> Query(string QueryString);
        Task<IRepoResponse> Add(Package ResourceInfo);
        Task<IRepoResponse> Delete(Package ResourceInfo);
        Task<IRepoResponse> Publish(Package ResourceInfo);
        Task<IRepoResponse> Compress();
        Task<IRepoResponse> Deploy();

    }
}