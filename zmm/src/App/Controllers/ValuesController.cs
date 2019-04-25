using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ZMM.App.PyServicesClient;
using ZMM.Helpers.ZMMDirectory;
using ZMM.Tasks;
using ZMM.Tools.JNB;

namespace ZMM.App.Controllers
{
    [Route("[controller]/[action]")]
    public class ValuesController : Controller
    {

       private readonly IPyJupyterServiceClient jupyterClient;   
       readonly ILogger<ValuesController>  Logger;

       private readonly IConfiguration configuration;

       public ValuesController(IConfiguration configuration, ILogger<ValuesController> log , IPyJupyterServiceClient _jupyterClient)
       {
           this.jupyterClient = _jupyterClient;
           this.Logger = log;
           this.configuration = configuration;
       }
       IConfiguration Configuration { get; }

         [HttpGet("path")]
        public string GetPath()
        {
            return DirectoryHelper.fileUploadDirectoryPath;
        }

        // POST api/values
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }

        #region To stop any running instance of Jupyter Notebook 
        //This is work around code to stop jupyter notebook instance.
        [HttpGet]        
        public JsonResult Stop()
        {  
            string ResourceName = Request.Query["name"].ToString();
            string ResourceNameWithoutExtension = ResourceName.Substring(0, ResourceName.LastIndexOf("."));
            string notebookDir = DirectoryHelper.GetCodeDirectoryPath() + ResourceNameWithoutExtension;
            string ResourcePath = notebookDir + System.IO.Path.DirectorySeparatorChar + ResourceName; //http://localhost:5000//uploads/TestUser/code/HelloClass2.ipynb
            string Message = "Error";
            System.Console.WriteLine("Resource Path " + ResourcePath);
            Dictionary<string,string> Result = new Dictionary<string, string>(); 
            var obj = new
            {         
                base_url = "/",             
                NotebookDir = $"{notebookDir}",
                ResourcePath = $"{ResourcePath}"
            };
            try
            {              
                   
                JupyterNotebook  JNBTool = this.jupyterClient.GetJupyterNotebookTool();                 
                ITask JupyterNoteBookTask = JNBTool.FindTask(ResourcePath);         
                if(JupyterNoteBookTask.IsEmpty())
                {
                    Message = "Error : There is no such task running";
                }   
                else
                {
                    JNBTool.StopTask(ResourcePath);
                    Message = "Notebook successfully stop";                    
                }
                
            }
            catch(Exception ex)
            {
                Message =  ex.Message;
            }
            Result.Add("Result", Message);
            return new JsonResult(Result);
        }
        #endregion
    }
}

