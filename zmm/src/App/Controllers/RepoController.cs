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
        

        #region Get
        /// <summary>
        /// Get all resource(s) from UMOYA (Repo) Server.
        /// </summary>
        /// <returns> List of Resource(s)</returns>
        /// <response code="200">Returns list of resource(s)</response>
        /// <response code="204">If no resource(s) found</response>
        /// <response code="400">If any error when getting resource(s) from UMOYA (Repo) Server </response>   
        [HttpGet("/api/repo")]  
        [Produces("application/json")]    
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Package>>> Get()
        {   
            try
            {
                IEnumerable<Package> ListOfResources = await Client.Get();
                if(ListOfResources.Count() > 0) return Ok(ListOfResources.ToList());
                else return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region Get Resource Info
        [HttpGet("/api/repo/{id}")]
        /// <summary>
        /// Get Resource meta data (info) from UMOYA (Repo) Server.
        /// </summary>
        /// <returns>Resource meta data (info)</returns>
        /// <response code="200">Return resource meta data(info) along with list of version(s)</response>
        /// <response code="204">If resource is not found</response>
        /// <response code="400">If any error when getting resource(s) from UMOYA (Repo) Server </response>      
        [Produces("application/json")]    
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Get(string id)
        {
            return Ok(new { message = "Not yet implemented " + id });
        }
        #endregion

        #region GetModels
        /// <summary>
        /// Get all model resource(s) from UMOYA (Repo) Server.
        /// </summary>
        /// <returns> List of Model Resource(s)</returns>
        /// <response code="200">Returns list of resource(s) which are models</response>
        /// <response code="204">If no resource(s) found</response>
        /// <response code="400">If any error when getting resource(s) from UMOYA (Repo) Server </response>   
        [HttpGet("/api/repo/models")]    
        [Produces("application/json")]    
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Package>>> GetModels()
        {   
            try
            {
                IEnumerable<Package> ListOfResources = await Client.GetModels();
                if(ListOfResources.Count() > 0) return Ok(ListOfResources.ToList());
                else return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region GetCode
        /// <summary>
        /// Get all code resource(s) from UMOYA (Repo) Server.
        /// </summary>
        /// <returns> List of Code Resource(s)</returns>
        /// <response code="200">Returns list of code resource(s)</response>
        /// <response code="204">If no resource(s) found</response>
        /// <response code="400">If any error when getting resource(s) from UMOYA (Repo) Server </response>   
        [HttpGet("/api/repo/codes")]    
        [Produces("application/json")]    
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Package>>> GetCodes()
        {   
            try
            {
                IEnumerable<Package> ListOfResources = await Client.GetCode();
                if(ListOfResources.Count() > 0) return Ok(ListOfResources.ToList());
                else return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region GetData
        /// <summary>
        /// Get all data resource(s) from UMOYA (Repo) Server.
        /// </summary>
        /// <returns> List of Data Resource(s)</returns>
        /// <response code="200">Returns list of data resource(s)</response>
        /// <response code="204">If no resource(s) found</response>
        /// <response code="400">If any error when getting resource(s) from UMOYA (Repo) Server </response>   
        [HttpGet("/api/repo/data")]    
        [Produces("application/json")]    
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Package>>> GetData()
        {   
            try
            {
                IEnumerable<Package> ListOfResources = await Client.GetData();
                if(ListOfResources.Count() > 0) return Ok(ListOfResources.ToList());
                else return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        #region GetData
        /// <summary>
        /// Get all resource(s) from UMOYA (Repo) Server by Query.
        /// </summary>
        /// <returns> List of Resource(s) by query</returns>
        /// <response code="200">Returns list of resource(s) by query</response>
        /// <response code="204">If no resource(s) found</response>
        /// <response code="400">If any error when getting resource(s) from UMOYA (Repo) Server </response>   
        [HttpGet("/api/repo/query")]    
        [Produces("application/json")]    
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<Package>>> Query(string QueryString)
        {   
            try
            {
                IEnumerable<Package> ListOfResources = await Client.Query(QueryString);
                if(ListOfResources.Count() > 0) return Ok(ListOfResources.ToList());
                else return NoContent();
            }
            catch(Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        #endregion

        
        
    }
}