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
using ZMM.App.Clients.Repo;
using System.Net.Mime;

namespace ZMM.App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class RepoController : Controller
    {
        #region Variables... 
        private readonly IWebHostEnvironment Env;
        readonly ILogger<RepoController> Logger;
        public IConfiguration Configuration { get; }     

        private IRepoClient Client;

        public RepoController(IWebHostEnvironment Env, IConfiguration Conf, ILogger<RepoController> Log, IRepoClient RClient)
        {
            //update 
            this.Env = Env ?? throw new ArgumentNullException(nameof(Env));
            this.Configuration = Conf;            
            this.Logger = Log;    
            this.Client = RClient;        
        }
        #endregion
        
       

        #region Get Resource(s) from UMOYA (Repo) Server by Type and or Query string or all
        /// <summary>
        /// Get all resource(s) from UMOYA (Repo) Server by Resource Type and or Query.
        /// </summary>
        /// <returns> List of Resource(s) by resource type and or query string</returns>
        /// <response code="200">Returns list of resource(s) which are part of given resource type and or query string</response>
        /// <response code="204">If no resource(s) found</response>
        /// <response code="400">If any error when getting resource(s) from UMOYA (Repo) Server </response>   
        [HttpGet("/api/repo")]    
        [Produces("application/json")]    
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Resource>>> Get(string ResourceType, string QueryString)
        {   
            try
            {
                IEnumerable<Resource> ListOfResources = await Client.Get(ResourceType, QueryString);
                if(ListOfResources.Count() > 0) return Ok(ListOfResources.ToList());
                else return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion
        
        #region Get Resource Info with dependencies and meta data
        [HttpGet("/api/repo/{id}")]
        /// <summary>
        /// Get Resource meta data (info) from UMOYA (Repo) Server.
        /// </summary>
        /// <returns>Resource meta data (info)</returns>
        /// <response code="200">Return resource meta data(info) along with list of version(s)</response>
        /// <response code="400">If any error when getting resource(s) from UMOYA (Repo) Server </response>      
        [Produces("application/json")]    
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<ResourceInfo>> Get(string id)
        {
            try
            {
                return await Client.Get(id);
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        
        
    }
}