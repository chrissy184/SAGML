using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ZMM.App.Clients.Repo
{
    public interface IRepoClient
    {
        Task<IEnumerable<Resource>> Get();
        Task<ResourceInfo> Get(string ResourceId);
        Task<IEnumerable<Resource>> Get(string ResourceType, string QueryString);
        Task<bool> Add(Resource ResourceInfo);
        Task<IRepoResponse> Delete(Resource ResourceInfo);
        Task<IRepoResponse> Publish(Resource ResourceInfo);
        Task<IRepoResponse> Compress();
        Task<IRepoResponse> Deploy();

    }
}