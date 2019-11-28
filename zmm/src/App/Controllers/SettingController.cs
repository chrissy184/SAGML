using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Linq;
using ZMM.Models.ResponseMessages;

namespace ZMM.App.Controllers
{
    // [Authorize]
    [Route("api/[controller]")]
    public class SettingController : Controller
    {
        #region Variables... 
        private readonly IWebHostEnvironment _environment;
        readonly ILogger<CodeController> Logger;
        public IConfiguration Configuration { get; }
        private List<ZSSettingResponse> settingsResponse;       

        #endregion

        #region Constructor...
        public SettingController(IWebHostEnvironment environment, IConfiguration configuration, ILogger<CodeController> log)
        {
            //update 
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            this.Configuration = configuration;            
            this.Logger = log; 
        }
        #endregion
    
        #region Add settings
        [HttpPost]
        public async Task<IActionResult> AddSettingAsync()
        {
            string jsonBody ="";
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                jsonBody = body.ToString();
            }
            JObject jObj = JObject.Parse(jsonBody);
            //jObj.Add("ZmodId", "testdk");
            JArray jArr =  (JArray) jObj["settings"];
            var setList = jArr.ToObject<List<SettingProperty>>();

            var newRecord = new ZSSettingResponse()
            {
                ZmodId = "testdk",
                Settings = setList
            };
            
            Models.Payloads.ZSSettingPayload.CreateOrUpdate(newRecord);
            return Json(Models.Payloads.ZSSettingPayload.GetSettingsByUser("testdk"));
        }
        #endregion
    }
}