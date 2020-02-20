using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using ZMM.Helpers.Common;
using ZMM.Models.Payloads;

namespace ZMM.App.Clients.Repo
{
    public class RepoClient : IRepoClient
    {
        public IConfiguration Config { get; }
        public RepoClient(IConfiguration Conf)
        {
            this.Config = Conf;
        }

        public Task<IRepoResponse> Get()
        {
            throw new NotImplementedException();
        }

        public Task<IRepoResponse> Get(string ResourceId)
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

        public Task<IRepoResponse> Add(Resource ResourceInfo)
        {
            throw new NotImplementedException();
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