using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ZMM.Helpers.Common;
using ZMM.Models.Payloads;

namespace ZMM.App.Clients.Repo
{
    public class RepoClient : IRepoClient
    {
        public IConfiguration Config { get; }  
        readonly ILogger<RepoClient> Logger;

        public RepoClient(IConfiguration Conf)
        {
            this.Config = Conf;
        }

        public async Task<IEnumerable<Package>> Get()
        {            
            HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(Constants.RepoURL);
            if (!ResponseFromRepo.IsSuccessStatusCode) throw new Exception("Exception while request to repo. Status Code : " + ResponseFromRepo.StatusCode);            
            Resources SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<Resources>();
            return SearchResultSetInListOfResources.Data;
        }


        public Task<Resources> Get(string ResourceId)
        {
            throw new NotImplementedException();
        }

        public Task<IRepoResponse> GetModels()
        {
            throw new NotImplementedException();
        }

        public Task<IRepoResponse> GetData()
        {
            throw new NotImplementedException();
        }

        public Task<IRepoResponse> GetCode()
        {
            throw new NotImplementedException();
        }

        public Task<IRepoResponse> Query(string QueryString)
        {
            throw new NotImplementedException();
        }

        public Task<IRepoResponse> Add(Package ResourceInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IRepoResponse> Delete(Package ResourceInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IRepoResponse> Publish(Package ResourceInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IRepoResponse> Compress()
        {
            throw new NotImplementedException();
        }

        public Task<IRepoResponse> Deploy()
        {
            throw new NotImplementedException();
        }

        Task<IRepoResponse> IRepoClient.Get(string ResourceId)
        {
            throw new NotImplementedException();
        }
    }
}