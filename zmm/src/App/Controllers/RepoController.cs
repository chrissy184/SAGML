using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZMM.App.PyServicesClient;
using Microsoft.Extensions.Logging;



namespace ZMM.App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class RepoController : Controller
    {
        #region Variables... 
        private readonly IWebHostEnvironment Env;
        readonly ILogger<CodeController> Logger;
        public IConfiguration Configuration { get; }     

        public RepoController(IWebHostEnvironment environment, IConfiguration configuration, ILogger<CodeController> log)
        {
            //update 
            Env = environment ?? throw new ArgumentNullException(nameof(environment));
            this.Configuration = configuration;            
            this.Logger = log;            
        }
        #endregion
        

        #region Get
        [HttpGet]
        public IActionResult Get(bool refresh)
        {
             return Ok(new { message = "Get Resource Controller" });
        }
        #endregion

        [HttpGet("{id}")]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(new { message = "Get Resource Controller with " + id });
        }
        
    }
}