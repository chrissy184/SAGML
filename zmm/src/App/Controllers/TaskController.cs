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
using Quartz;
using Quartz.Impl;
using System.Linq;
using ZMM.Models.ClientJsonObjects;

namespace ZMM.App.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    public class TaskController : Controller
    {
        #region variables
        private readonly IWebHostEnvironment _environment;
        private IConfiguration Configuration { get; }
        readonly ILogger<TaskController> Logger;
        private readonly IPyNNServiceClient nnclient;
        private readonly IPyAutoMLServiceClient autoMLclient;

        private List<TaskResponse> taskResponse;
        private List<ModelResponse> modelResponse;
        private readonly string CURRENT_USER = "";

        #endregion

        #region Constructor...
        public TaskController(IWebHostEnvironment environment, IConfiguration configuration, ILogger<TaskController> log, IPyNNServiceClient srv, IPyAutoMLServiceClient automlSrv)
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
            catch (Exception ex)
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
            var tasks = SchedulerPayload.Get().Select(t => new
            {
                t.Id,
                t.Name,
                t.CreatedOn,
                t.EditedOn,
                t.Type,
                t.CronExpression,
                t.StartDate,
                t.StartTimeH,
                t.StartTimeM,
                t.Recurrence,
                t.Status
            });
            await Task.FromResult(0);
            if (tasks.Count() > 0)
            {
                return Json(tasks);
            }
            else
            {
                return Ok(new { });
            }

        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetSelectedTaskAysnc(string id)
        {
            var taskData = SchedulerPayload.Get().Where(s => s.Id == id).FirstOrDefault();
            var _type = SchedulerPayload.Get().Where(s => s.Id == id).Select(t => t.Type).FirstOrDefault();
            List<object> taskHistNew = new List<object>();

            if (!string.IsNullOrEmpty(taskData.Id))
            {
                string origid = "";
                if (id.IndexOf('-') > 0)
                {
                    origid = id.Substring(0, id.IndexOf('-'));
                }
                else
                {
                    origid = id;
                }
                var resp = await nnclient.GetRunningTaskByTaskName(origid);
                if((JObject.Parse(resp)["runningTask"].Count() == 0) && (taskData.Id != taskData.Name))
                {
                    resp = await nnclient.GetRunningTaskByTaskName(origid.Substring(0, origid.IndexOf(taskData.Name)));
                }
                var joResp = JsonConvert.DeserializeObject<ZMM.Models.ClientJsonObjects.ZMKCodeExecution.RootObject>(resp);
                JArray jArr = JArray.FromObject(joResp.runningTask);

                // JObject joResp = JObject.Parse(resp);
                // JArray jArr = (JArray)joResp["runningTask"];
                //
                if(taskData.Recurrence == "REPEAT")
                {
                    taskHistNew.Clear();
                    foreach (var j in taskData.History)
                    {
                        if (_type.Contains("PYTHON"))
                        {
                            ExecuteCodeResponse ecr = (ExecuteCodeResponse)j;
                            string _status = jArr.Where(s => s["idforData"].ToString() == ecr.idforData).Select(s => s["status"].Value<string>()).FirstOrDefault();
                            ecr.status = _status;
                            taskHistNew.Add(ecr);
                        }
                        if (_type.Contains("AUTOML"))
                        {
                            AutoMLResponse obj = (AutoMLResponse)j;
                            string _status = jArr.Where(s => s["idforData"].ToString() == obj.idforData).Select(s => s["status"].Value<string>()).FirstOrDefault();
                            obj.status = _status;
                            taskHistNew.Add(obj);
                        }
                        if (_type.Contains("NN"))
                        {
                            TrainingResponse obj = (TrainingResponse)j;
                            string _status = jArr.Where(s => s["idforData"].ToString() == obj.idforData).Select(s => s["status"].Value<string>()).FirstOrDefault();
                            obj.status = _status;
                            taskHistNew.Add(obj);
                        }
                    }                    
                }
                else if(taskData.Recurrence == "ONE_TIME")
                {
                    taskHistNew.Clear();
                    foreach (var j in taskData.History)
                    {
                        if(j.ToString() == "ZMM.Models.ResponseMessages.TrainingResponse")
                        {
                            TrainingResponse jlist = (TrainingResponse)j;
                            string _idfordata = jArr.Where(s => s["idforData"].ToString() == jlist.idforData).Select(s => s["idforData"].Value<string>()).FirstOrDefault();
                            if (!string.IsNullOrEmpty(_idfordata))
                            {
                                JObject obj = JObject.FromObject(new
                                {
                                    idforData = _idfordata,
                                    status = jArr.Where(s => s["idforData"].ToString() == jlist.idforData).Select(s => s["status"].Value<string>()).FirstOrDefault(),
                                    executedAt = jlist.executedAt
                                });
                                taskHistNew.Add(obj);
                            }

                        }
                        else
                        {
                            var jo = JObject.Parse(j.ToString());
                            string _idfordata = jArr.Where(s => s["idforData"].ToString() == jo["idforData"].ToString()).Select(s => s["idforData"].Value<string>()).FirstOrDefault();
                            if (!string.IsNullOrEmpty(_idfordata))
                            {
                                JObject obj = JObject.FromObject(new
                                {
                                    idforData = _idfordata,
                                    status = jArr.Where(s => s["idforData"].ToString() == jo["idforData"].ToString()).Select(s => s["status"].Value<string>()).FirstOrDefault(),
                                    executedAt = jo["executedAt"].ToString()
                                });
                                taskHistNew.Add(obj);
                            }
                        }
                        
                    }
                }
                taskData.History.Clear();
                taskData.History = taskHistNew;
                SchedulerPayload.Update(taskData);

                return Json(SchedulerPayload.Get().Where(s => s.Id == id).FirstOrDefault());
            }
            else
            {
                return Ok(new { });
            }
        }

        [HttpGet("{id}/history/{idforData}")]
        public async Task<IActionResult> GetSelectedTaskAysnc(string id, string idforData)
        {
            if (!string.IsNullOrEmpty(id) && (!string.IsNullOrEmpty(idforData)))
            {
                var zmkresp = await nnclient.GetRunningTaskByTaskNameAndId(id, idforData);
                var jo = JObject.Parse(zmkresp);

                return Json(jo);
            }
            else
            {
                return NotFound();
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
            if (!string.IsNullOrEmpty(reqBody))
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
                if (!Directory.Exists(dirFullpath))
                {
                    Directory.CreateDirectory(dirFullpath);
                }

                //call py service and add content to pmml file
                fileContent = await autoMLclient.SaveBestPmml(filePath);
                //
                using (StreamWriter writer = new StreamWriter(Path.Combine(dirFullpath, newFile)))
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
                    Extension = "pmml",
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
            //delete job from scheduler
            // First we must get a reference to a scheduler
            ISchedulerFactory schfack = new StdSchedulerFactory();
            IScheduler scheduler = await schfack.GetScheduler();
            string filePath = SchedulerPayload.GetById(id).Select(c => c.FilePath).FirstOrDefault();
            var jobKey = new JobKey(filePath);
            bool isDeleted = await scheduler.DeleteJob(jobKey);
            //
            SchedulerPayload.Delete(id);
            response = await nnclient.DeleteRunningTask(id);
            return Json(new { message = "Task deleted.", id = id });
        }

        #endregion

        #region Scheduler related
        //
        #region Stop running scheduler
        [HttpGet("{id}/stop")]
        public async Task<IActionResult> PauseScheduledJob(string id)
        {
            string response = string.Empty;
            string filePath = Helpers.Common.FilePathHelper.GetFilePathById(id, CodePayload.Get());

            ISchedulerFactory schfack = new StdSchedulerFactory();
            IScheduler scheduler = await schfack.GetScheduler();
            await scheduler.PauseJob(new JobKey(filePath));

            var resp = new
            {
                id = id,
                taskName = id,
                createdOn = "",
                type = "Code",
                cronExpression = "0 * * ? * *",
                status = "STOPPED"
            };
            return Ok(resp);
        }
        #endregion

        #endregion
    }
}