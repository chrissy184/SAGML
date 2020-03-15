using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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

        public async Task<IEnumerable<Resource>> Get()
        {      
            HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(Constants.RepoURL);
            if (!ResponseFromRepo.IsSuccessStatusCode) throw new Exception("Exception while request to repo. Status Code : " + ResponseFromRepo.StatusCode);            
            Resources SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<Resources>();
            return SearchResultSetInListOfResources.Data;
        }


        //https://???/v3/registration/helloworld.pmml/index.json
        public async Task<ResourceInfo> Get(string ResourceId)
        {
            HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(Constants.RepoURLByResourceId.Replace("ResourceId", ResourceId));
            if (!ResponseFromRepo.IsSuccessStatusCode) throw new Exception("Exception while request to repo. Status Code : " + ResponseFromRepo.StatusCode);            
            ResourceInfo SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<ResourceInfo>();
            return SearchResultSetInListOfResources;
        }

        public Task<IEnumerable<Resource>> Get(string ResourceType, string QueryString)
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

        private async Task<IEnumerable<Resource>> GetResourcesByType(string ResourceType)
        {            
            HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(Constants.RepoURLQueryByResourceType + ResourceType);
            if (!ResponseFromRepo.IsSuccessStatusCode) throw new Exception("Exception while request to repo. Status Code : " + ResponseFromRepo.StatusCode);            
            Resources SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<Resources>();
            return SearchResultSetInListOfResources.Data;
        }

        private async Task<IEnumerable<Resource>> GetResourcesByTypeAndQueryString(string ResourceType, string QueryString)
        {            
            HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(Constants.RepoURLQueryByResourceTypeAndQueryString.Replace("ResourceType", ResourceType).Replace("QueryString", QueryString));
            if (!ResponseFromRepo.IsSuccessStatusCode) throw new Exception("Exception while request to repo. Status Code : " + ResponseFromRepo.StatusCode);            
            Resources SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<Resources>();
            return SearchResultSetInListOfResources.Data;
        }        

        private async Task<IEnumerable<Resource>> GetResourcesByQuery(string QueryString)
        {
            HttpResponseMessage ResponseFromRepo = await RestOps.GetResponseAsync(Constants.RepoURLQuery + QueryString);
            if (!ResponseFromRepo.IsSuccessStatusCode) throw new Exception("Exception while request to repo. Status Code : " + ResponseFromRepo.StatusCode);            
            Resources SearchResultSetInListOfResources = await ResponseFromRepo.Content.ReadAsAsync<Resources>();
            return SearchResultSetInListOfResources.Data;
        }


        public async Task<bool> Add(Resource ResourceInfo)
        {
            UMOYA.Instance.Add(ResourceInfo);
            return true;
        }

        public Task<IRepoResponse> Delete(Resource ResourceInfo)
        {
            throw new NotImplementedException();
        }

        public Task<IRepoResponse> Publish(Resource ResourceInfo)
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