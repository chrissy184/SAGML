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
public class TrainModelJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        Console.WriteLine($"Model training started: {dataMap.GetString("filePath")} - {dataMap.GetString("id")} is now running at {DateTime.Now.ToString()} and a random number is : {new Random().Next(0, 99999)}");
        string filePath = dataMap.GetString("filePath");
        string baseAddress = $"{dataMap.GetString("baseurl")}newtrainmodels/{dataMap.GetString("id")}";
        //
        try
        {
            WebRequest reqObj = WebRequest.Create(baseAddress);
            reqObj.Method = "Get";
            reqObj.ContentType = "application/json";
            var response = (HttpWebResponse)reqObj.GetResponse();

            using (var rdr = response.GetResponseStream())
            {
                DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(TrainingResponse));
                TrainingResponse trainingResp = (TrainingResponse)deserializer.ReadObject(rdr);
                trainingResp.executedAt = DateTime.Now;
                JobSchedulerHelper.AddZMKResponses(dataMap.GetString("id"), trainingResp);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
        }
        return Task.FromResult(0);
    }
}