using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace ZMM.App.Clients.Repo
{
    public class IRepoResponse : IActionResult
    {
        public Task ExecuteResultAsync(ActionContext context)
        {
            throw new NotImplementedException();
        }
    }
}
