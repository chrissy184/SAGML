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
using Newtonsoft.Json;
using System.Text;

namespace ZMM.App.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class SettingController : Controller
    {
        #region Variables... 
        private readonly IWebHostEnvironment _environment;
        readonly ILogger<SettingController> Logger;
        public IConfiguration Configuration { get; }
        // private List<ZSSettingResponse> settingsResponse;

        public static string ZmodId;

        #endregion

        #region Constructor...
        public SettingController(IWebHostEnvironment environment, IConfiguration configuration, ILogger<SettingController> log)
        {
            //update 
            _environment = environment ?? throw new ArgumentNullException(nameof(environment));
            this.Configuration = configuration;
            this.Logger = log;
        }
        #endregion

        #region Add settings
        [HttpPost("~/api/setting")]
        public async Task<IActionResult> AddSettingAsync()
        {
            string jsonBody = "", zmodId = "";
            //read from body
            using (var reader = new StreamReader(Request.Body))
            {
                var body = await reader.ReadToEndAsync();
                jsonBody = body.ToString();
            }

            if (!string.IsNullOrEmpty(jsonBody))
            {
                zmodId = ZSSettingPayload.GetUserNameOrEmail(HttpContext);
                //parse
                JObject jObj = JObject.Parse(jsonBody);
                JArray jArr = (JArray)jObj["settings"];
                var setList = jArr.ToObject<List<SettingProperty>>();

                //fetch the original record
                List<SettingProperty> setListOrig = ZSSettingPayload.GetSettingsByUser(zmodId).SelectMany(b => b.Settings).ToList<SettingProperty>();
                foreach(var p in setList)
                {
                    if(p.username.Contains("******"))
                    {
                        p.username = setListOrig.Where(c => c.url == p.url).Select(c=> c.username).FirstOrDefault().ToString();
                    }
                    if(p.password.Contains("******"))
                    {
                        p.password = setListOrig.Where(c => c.url == p.url).Select(c=> c.password).FirstOrDefault().ToString();
                    }
                }

                //add to payload
                var newRecord = new ZSSettingResponse()
                {
                    ZmodId = zmodId,
                    Settings = setList
                };
                ZSSettingPayload.CreateOrUpdate(newRecord);
                return Json(JObject.Parse(jsonBody));
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        #region GET Settings
        [HttpGet("~/api/setting")]
        public async Task<IActionResult> GetSettingsAsync(string type,bool selected, bool unmask, bool showall)
        {
            //get the zmodId
            string UserEmailId = ZSSettingPayload.GetUserNameOrEmail(HttpContext);
            // string UserEmailId = "generic@softwareag.com";
            JObject jObj = new JObject();
            await Task.FromResult(0);
            var settings = ZSSettingPayload.GetSettingsByUser(UserEmailId);
            List<SettingProperty> settingProperties;

            if(string.IsNullOrEmpty(type))
            {
                settingProperties = settings.SelectMany(b => b.Settings).ToList<SettingProperty>();
            }
            else
            {                
                settingProperties = settings.SelectMany(b => b.Settings).ToList<SettingProperty>(); 

                if(!string.IsNullOrEmpty(type) && (showall == true)) 
                    settingProperties = settingProperties.Where(c=>c.type == $"{type}").ToList<SettingProperty>();
                else
                    settingProperties = settingProperties.Where(c=>c.type == $"{type}" && c.selected == selected).ToList<SettingProperty>();
            }
            // var selectedType = settingProperties.Where(c=>c.type == $"{type}").ToList<SettingProperty>();
            //
            #region seed settings
            if(settings.Count == 0)
            {     
                var template = new ZSSettingResponse
                {    
                    ZmodId = UserEmailId,                
                    Settings = new List<SettingProperty> {
                        new SettingProperty{ name="Cumulocity",type="C8Y",tenantID="ai", username="*******",password="*******",url="https://ai.eu-latest.cumulocity.com",selected=true },
                        new SettingProperty{ name="Cumulocity",type="C8Y",tenantID="ai", username="*******",password="*******",url="https://ai.cumulocity.com",selected=false },
                        new SettingProperty{ name="Zementis Server",type="ZS",tenantID="zserver", username="*******",password="*******",url="https://ai.eu-latest.cumulocity.com/",selected=true },
                        new SettingProperty{ name="Zementis Server",type="ZS",tenantID="zserver", username="*******",password="*******",url="https://zserver.zmod.org/adapars/",selected=false },
                        new SettingProperty{ name="Repo Server 1",type="NR",tenantID="repo", username="*******",password="*******",url="https://repo.umoya.ai/",selected=false },
                        new SettingProperty{ name="Repo Server 2",type="NR",tenantID="hub", username="*******",password="*******",url="https://hub.umoya.ai/",selected=true },
                        new SettingProperty{ name="DataHub 1",type="DH",driver="Dremio Connector", username="*******",password="*******",url="https://url",port="0000",selected=true }
                    }
                };
                jObj = JObject.Parse(JsonConvert.SerializeObject(template)); 
                ZSSettingPayload.CreateOrUpdate(template);            
            }
            else
            {
                var template = new ZSSettingResponse
                {    
                    ZmodId = UserEmailId,                
                    Settings = settingProperties
                };

                jObj = JObject.Parse(JsonConvert.SerializeObject(template));  
            } 

            #endregion
                       
            jObj.Remove("zmodId");
            //
            foreach (var p in jObj["settings"])
            {
                if (unmask == false)
                {
                    p["username"] = "******";
                    p["password"] = "******";
                }

                if(p["type"].ToString() == "DH")
                {                    
                    p["tenantID"].Parent.Remove();
                }
                else
                {
                    p["port"].Parent.Remove();
                    p["driver"].Parent.Remove();
                }
            }
            //

            return Json(jObj);
        }
        #endregion

    }
}