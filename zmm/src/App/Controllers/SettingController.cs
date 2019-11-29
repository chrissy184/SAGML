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
using ZMM.Models.Payloads;

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
            string jsonBody = "", zmodId = "testdk";
            //read from body
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                jsonBody = body.ToString();
            }
            // zmodId = ZSSettingPayload.GetUserNameOrEmail(HttpContext);
            //parse
            JObject jObj = JObject.Parse(jsonBody);
            JArray jArr = (JArray)jObj["settings"];
            var setList = jArr.ToObject<List<SettingProperty>>();
            //add to payload
            var newRecord = new ZSSettingResponse()
            {
                ZmodId = zmodId,
                Settings = setList
            };
            ZSSettingPayload.CreateOrUpdate(newRecord);

            return Json(ZSSettingPayload.GetSettingsByUser(zmodId));
        }
        #endregion

        #region GET Settings
        [HttpGet]
        public async Task<IActionResult> GetSettingsAsync()
        {
            //get the zmodId
            string zmodId = "";// ZSSettingPayload.GetUserNameOrEmail(HttpContext);
            await Task.FromResult(0);
            var settings = ZSSettingPayload.GetSettingsByUser(zmodId);
            // //mask uname and pass with *
            // foreach (var record in settings)
            // {
            //     foreach (var s in record.Settings)
            //     {
            //         s.username = "********";
            //         s.password = "********";
            //     }
            // }

            if(settings.Count == 0)
            {
                
                return Json(new List<SettingProperty>{
                    new SettingProperty{ name="Cumulocity",type="C8Y",tenantID="ai", username="",password="",url="",selected=false },
                    new SettingProperty{ name="Zementis Server",type="ZS",tenantID="ai", username="",password="",url="",selected=false }
                });
            }

            return Json(settings);
        }
        #endregion

    }
}