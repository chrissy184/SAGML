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
public class UploadDataJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        string filePath = dataMap.GetString("filePath");
        string id = dataMap.GetString("id");
        
        Console.WriteLine("Uploading {filePath} with Id:{id} started...");
        System.Threading.Thread.Sleep(1000000);
        Console.WriteLine("Uploading {filePath} with Id:{id} completed.");
        return Task.FromResult(0);
    }
}