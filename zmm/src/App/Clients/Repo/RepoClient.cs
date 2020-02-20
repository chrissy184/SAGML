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

        private List<string> ResourceTypes = new List<string>(){"model", "code", "data"};
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

        public Task<IEnumerable<Package>> Get(string ResourceType, string QueryString)
        {
            if(!string.IsNullOrEmpty(ResourceType))
            {
                if(ResourceTypes.Contains(ResourceType.ToLower()))
                {
                    if(!string.IsNullOrEmpty(QueryString))
                    {
                        return GetResourcesByTypeAndQueryString(ResourceType, QueryString);
                    }
                    else
                    {                    
                        return GetResourcesByType(ResourceType);              
                    } 
                }
                else throw new Exception("Given resource type is not valid. Valid resource types are model, code, data");
            }
            else 
            {
                if(!string.IsNullOrEmpty(QueryString)) return GetResourcesByQuery(QueryString);
                else return Get();
            }
        }

        private async Task<IEnumerable<Package>> GetResourcesByType(string ResourceType)
        {            
            HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(Constants.RepoURLQueryByResourceType + ResourceType);
            if (!ResponseFromRepo.IsSuccessStatusCode) throw new Exception("Exception while request to repo. Status Code : " + ResponseFromRepo.StatusCode);            
            Resources SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<Resources>();
            return SearchResultSetInListOfResources.Data;
        }

        private async Task<IEnumerable<Package>> GetResourcesByTypeAndQueryString(string ResourceType, string QueryString)
        {            
            HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(Constants.RepoURLQueryByResourceTypeAndQueryString.Replace("ResourceType", ResourceType).Replace("QueryString", QueryString));
            if (!ResponseFromRepo.IsSuccessStatusCode) throw new Exception("Exception while request to repo. Status Code : " + ResponseFromRepo.StatusCode);            
            Resources SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<Resources>();
            return SearchResultSetInListOfResources.Data;
        }        

        private async Task<IEnumerable<Package>> GetResourcesByQuery(string QueryString)
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