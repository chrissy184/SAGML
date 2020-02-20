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


        //https://???/v3/registration/helloworld.pmml/index.json
        public Task<Package> Get(string ResourceId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Package>> GetModels()
        {            
            HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(Constants.RepoURLQueryByResourceType + "model");
            if (!ResponseFromRepo.IsSuccessStatusCode) throw new Exception("Exception while request to repo. Status Code : " + ResponseFromRepo.StatusCode);            
            Resources SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<Resources>();
            return SearchResultSetInListOfResources.Data;
        }

        public async Task<IEnumerable<Package>> GetData()
        {
            HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(Constants.RepoURLQueryByResourceType + "data");
            if (!ResponseFromRepo.IsSuccessStatusCode) throw new Exception("Exception while request to repo. Status Code : " + ResponseFromRepo.StatusCode);            
            Resources SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<Resources>();
            return SearchResultSetInListOfResources.Data;
        }

        public async Task<IEnumerable<Package>> GetCode()
        {
            HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(Constants.RepoURLQueryByResourceType + "code");
            if (!ResponseFromRepo.IsSuccessStatusCode) throw new Exception("Exception while request to repo. Status Code : " + ResponseFromRepo.StatusCode);            
            Resources SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<Resources>();
            return SearchResultSetInListOfResources.Data;
        }

        public async Task<IEnumerable<Package>> Query(string QueryString)
        {
            HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(Constants.RepoURLQuery + QueryString);
            if (!ResponseFromRepo.IsSuccessStatusCode) throw new Exception("Exception while request to repo. Status Code : " + ResponseFromRepo.StatusCode);            
            Resources SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<Resources>();
            return SearchResultSetInListOfResources.Data;
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
        
    }
}