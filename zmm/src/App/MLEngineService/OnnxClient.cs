using System;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ZMM.Helpers.Common;
using ZMM.Models.Payloads;

namespace ZMM.App.MLEngineService
{
    public class OnnxClient : IOnnxClient
    {
        private readonly IConfiguration configuration;

        public OnnxClient(IConfiguration _configuration)
        {
            this.configuration = _configuration;
        }
        public async Task<string> DeployModelAsync(string zmodId, string filePath)
        {
            string jsonResult = string.Empty;
            var tuple = ZSSettingPayload.GetUserCredetials(zmodId, "MLE-ONNX");
            var auth = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{tuple.Item2}:{tuple.Item3}")));

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(tuple.Item1);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = auth;
                httpClient.Timeout=TimeSpan.FromMinutes(30);

                using (var content = new MultipartFormDataContent("Upload--" + DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                {
                    var memory = new MemoryStream();
                    using (var stream = new FileStream(filePath, FileMode.Open))
                    {
                        await stream.CopyToAsync(memory);
                    }
                    memory.Position = 0;
                    content.Add(new StreamContent(memory), "file", filePath);
                    HttpResponseMessage response = await httpClient.PostAsync("service/zementis/onnx/models", content);
                    if (response.IsSuccessStatusCode)
                    {
                        jsonResult = await response.Content.ReadAsStringAsync();
                    }
                    else if (response.StatusCode.ToString() == "402")
                    {
                        return "FileExists";
                    }
                    else
                    {
                        return $"Fail@@{jsonResult}";
                    }

                }                
            }

            return jsonResult;
        }

        public async Task<string> GetAllModel(string zmodId)
        {
            string jsonResult = string.Empty;
            var tuple = ZSSettingPayload.GetUserCredetials(zmodId, "MLE-ONNX");
            var auth = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{tuple.Item2}:{tuple.Item3}")));

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(tuple.Item1);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = auth;

                try
                {
                    HttpResponseMessage response = await httpClient.GetAsync("service/onnx/models");

                    if (response.IsSuccessStatusCode)
                    {
                        jsonResult = await response.Content.ReadAsStringAsync();
                    }
                    else
                        return ZMMConstants.ErrorFailed;
                }
                catch (HttpRequestException ex)
                {
                    jsonResult = "{'message': '" + ex.Message + "', 'error':'" + ZMMConstants.ErrorFailed + "'}";
                }
            }

            return jsonResult;
        }

        public Task<string> GetModelInfo(string zmodId, string mleModelId)
        {
            throw new System.NotImplementedException();
        }

        public async Task<string> RemoveModelAsync(string zmodId, string mleModelId)
        {
            string jsonResult = string.Empty;
            var tuple = ZSSettingPayload.GetUserCredetials(zmodId, "MLE-ONNX");
            var auth = new AuthenticationHeaderValue("Basic", Convert.ToBase64String(Encoding.UTF8.GetBytes($"{tuple.Item2}:{tuple.Item3}")));

            using (var httpClient = new HttpClient())
            {
                httpClient.BaseAddress = new System.Uri(tuple.Item1);
                httpClient.DefaultRequestHeaders.Accept.Clear();
                httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                httpClient.DefaultRequestHeaders.Authorization = auth;

                try
                {
                    HttpResponseMessage response = await httpClient.DeleteAsync($"service/onnx/models/{mleModelId}");

                    if (response.IsSuccessStatusCode)
                    {
                        jsonResult = await response.Content.ReadAsStringAsync();
                    }
                    else
                        return ZMMConstants.ErrorFailed;
                }
                catch (HttpRequestException ex)
                {
                    jsonResult = "{'message': '" + ex.Message + "', 'error':'" + ZMMConstants.ErrorFailed + "'}";
                }
            }

            return $"Success@@{jsonResult}";
        }
    }
}