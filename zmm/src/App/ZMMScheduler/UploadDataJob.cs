using System;
using System.IO;
using System.Threading.Tasks;
using Quartz;
public class UploadDataJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        string filePath = dataMap.GetString("filePath");
        string extn = Path.GetExtension(filePath);        
        string id = dataMap.GetString("id");        
        Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Uploading {filePath} with Id:{id} started...");
        
        Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Uploading {filePath} with Id:{id} completed.");
        return Task.FromResult(0);
    }
}