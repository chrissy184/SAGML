using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace ZMM.App.Clients.Repo
{
    public interface IRepoClient
    {
        Task<IEnumerable<Package>> Get();
        Task<Package> Get(string ResourceId);
        Task<IEnumerable<Package>> GetModels();
        Task<IEnumerable<Package>> GetData();
        Task<IEnumerable<Package>> GetCode();
        Task<IEnumerable<Package>> Query(string QueryString);
        Task<IRepoResponse> Add(Package ResourceInfo);
        Task<IRepoResponse> Delete(Package ResourceInfo);
        Task<IRepoResponse> Publish(Package ResourceInfo);
        Task<IRepoResponse> Compress();
        Task<IRepoResponse> Deploy();

    }
}