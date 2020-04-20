using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Quartz;
using ZMM.App.PyServicesClient;
using ZMM.Models.ResponseMessages;
using System.Runtime.Serialization.Json;
using Newtonsoft.Json;

public class TrainModelJob : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        string jsonResult = string.Empty;
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        Console.WriteLine($"Model training started: {dataMap.GetString("filePath")} - {dataMap.GetString("id")} is now running at {DateTime.Now.ToString()} and a random number is : {new Random().Next(0, 99999)}");
        string filePath = dataMap.GetString("filePath");
        string baseAddress = $"{dataMap.GetString("baseurl")}";
        string requestBody = dataMap.GetString("reqBody");        
        Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>BASE-ADDR = {baseAddress}");
        //
        using (var httpClient = new HttpClient())
        {
            httpClient.BaseAddress = new System.Uri(baseAddress);
            httpClient.DefaultRequestHeaders.Accept.Clear();
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            HttpContent _httpContent = new StringContent(requestBody);            
            string _contentType = "application/json";
            _httpContent.Headers.ContentType = new MediaTypeHeaderValue(_contentType);
            //
            try
            {
                HttpResponseMessage response = await httpClient.PostAsync($"trainNNModel", _httpContent);
                if (response.IsSuccessStatusCode)
                {
                    jsonResult = await response.Content.ReadAsStringAsync();
                }
                TrainingResponse trainingResp = JsonConvert.DeserializeObject<TrainingResponse>(jsonResult); 
                trainingResp.executedAt = DateTime.Now;
                JobSchedulerHelper.AddZMKResponses(dataMap.GetString("id"), trainingResp);
            }
            catch (HttpRequestException ex)
            {
                jsonResult = "{'message': '" + ex.Message + "'";
            }
        }
        Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>{jsonResult}");
        return;        
    }
}