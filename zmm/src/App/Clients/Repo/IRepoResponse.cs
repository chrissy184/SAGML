using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ZMM.App.Clients.Repo
{
    public class RepoResult
    {
        public Exception Exception { get; set; }
        public object Data { get; set; }
    }

    public class IRepoResponse : IActionResult
    {
        private readonly RepoResult _result;

        public IRepoResponse(RepoResult result)
        {
            _result = result;
        }

        public async Task ExecuteResultAsync(ActionContext context)
        {
            var objectResult = new ObjectResult(_result.Exception ?? _result.Data)
            {
                StatusCode = _result.Exception != null
                    ? StatusCodes.Status500InternalServerError
                    : StatusCodes.Status200OK
            };

            await objectResult.ExecuteResultAsync(context);
        }
    }
    
}
