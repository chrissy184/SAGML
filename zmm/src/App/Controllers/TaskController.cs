using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZMM.App.PyServicesClient;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;
using ZMM.Models.ResponseMessages;
using ZMM.Models.Payloads;
using ZMM.Helpers.ZMMDirectory;

namespace ZMM.App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TaskController: Controller
    {   
        #region variables
        private readonly IHostingEnvironment _environment; 
        private IConfiguration Configuration { get; }
        readonly ILogger<TaskController>  Logger;
        private readonly IPyNNServiceClient nnclient;      
        private readonly IPyAutoMLServiceClient autoMLclient;     
        
        private List<TaskResponse> taskResponse;
        private List<ModelResponse> modelResponse;
        private readonly string CURRENT_USER = "";
        #endregion
       
        #region Constructor...
        public TaskController(IHostingEnvironment environment,IConfiguration configuration, ILogger<TaskController> log ,IPyNNServiceClient srv, IPyAutoMLServiceClient automlSrv)
        {
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));   
            this.Configuration = configuration; 
            this.nnclient = srv;
            this.Logger = log;
            this.autoMLclient = automlSrv;

            try
            {   
                taskResponse = TaskPayload.Get();
                modelResponse = ModelPayload.Get();
            }
            catch(Exception ex)
            {
                //ILogger
                string error = ex.Message;
            }           
        } 
        #endregion
        
        #region Get tasks...
        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {  
            string response = string.Empty;
            
            response = await nnclient.GetAllRunningTask();

            if(!string.IsNullOrEmpty(response))    
            {
                JObject jsonObj = JObject.Parse(response);
                return Json(jsonObj);
            }       
            else
            {
                return BadRequest("error!");
            }
                       
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSelectedTaskAysnc(string id)
        {
            string response = string.Empty;

            response = await autoMLclient.GetSelectedTask(id);
            if(!string.IsNullOrEmpty(response))    
            {
                JObject jsonObj = JObject.Parse(response);
                return Json(jsonObj);
            }       
            else
            {
                return BadRequest("error!");
            }            
        }

        #endregion

        #region Save model...
        [HttpPost("{id}/saveModel")]
        public async Task<IActionResult> SavePmmlAsync(string id)
        {  
            //variables
            string response = string.Empty;
            string reqBody = string.Empty;
            string filePath = string.Empty;
            string fileName = string.Empty;
            string fileContent = string.Empty;
            string fileUrl = string.Empty;
            ModelResponse _data = new ModelResponse();
            List<Property> _props = new List<Property>();

            //read from request body
            using (var reader = new StreamReader(Request.Body))
            {
                var body = reader.ReadToEnd();
                reqBody = body.ToString(); 
            }
            //
            JObject jObj = new JObject();
            if(!string.IsNullOrEmpty(reqBody))    
            {
                jObj = JObject.Parse(reqBody);
                filePath = jObj["filePath"].ToString();
                fileName = jObj["fileName"].ToString();
            }
            //Create blank pmml file
            long fileSize = 0L;
            string dirFullpath = DirectoryHelper.GetModelDirectoryPath();
            string newFile = fileName;
            
            try
            {
                //check if folder path exists...if not then create folder
                if(!Directory.Exists(dirFullpath))
                {
                    Directory.CreateDirectory(dirFullpath);
                }              

                //call py service and add content to pmml file
                fileContent = await autoMLclient.SaveBestPmml(filePath);                               
                //
                using (StreamWriter writer = new StreamWriter(Path.Combine(dirFullpath,newFile))) 
                {
                    await writer.WriteLineAsync(fileContent);
                    writer.Flush();
                    fileSize = writer.BaseStream.Length;
                } 
                string _url = DirectoryHelper.GetModelUrl(newFile);  
                string _filePath = Path.Combine(dirFullpath, newFile);
                //
                ModelResponse newRecord = new ModelResponse()
                {
                    Created_on = DateTime.Now.ToString(),
                    Deployed = false,
                    Edited_on = DateTime.Now.ToString(),
                    Extension = ".pmml",
                    FilePath = _filePath,
                    Id = newFile.Replace($".pmml", ""),
                    Loaded = false,
                    MimeType = "application/octet-stream",
                    Name = newFile,
                    Size = fileSize,
                    Type = "PMML",
                    Url = _url,
                    User = CURRENT_USER,
                    Properties = _props
                };
                //
                _data = ModelPayload.Create(newRecord);
            }
            catch (Exception ex)
            {
                //write to Ilogger
                string message = ex.Message;
            }
            string jsonStr = JsonConvert.SerializeObject(_data, Formatting.None); 
            return Json(_data);
        }
        #endregion
    
        #region delete task
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTaskAsync(string id)
        { 
            string response = string.Empty;

            response = await nnclient.DeleteRunningTask(id);
            if(!string.IsNullOrEmpty(response))    
            {
                JObject jsonObj = JObject.Parse(response);
                return Json(jsonObj);
            }       
            else
            {
                return BadRequest("error!");
            }      

        }

        #endregion
    }
}