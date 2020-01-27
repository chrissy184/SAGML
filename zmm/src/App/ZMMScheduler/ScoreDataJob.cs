using System;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Quartz;
using System.Runtime.Serialization.Json;
using ZMM.Models.ResponseMessages;
public class ScoreDataJob : IJob
{
    public Task Execute(IJobExecutionContext context)
    {
        /* predict data ZMK */
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        string filePath = dataMap.GetString("filePath");
        string baseAddress = $"{dataMap.GetString("baseurl")}models/{Path.GetFileNameWithoutExtension(filePath)}/score";
        //
        try
        {
            WebRequest reqObj = WebRequest.Create(baseAddress);
            reqObj.Method = "POST";
            reqObj.ContentType = "application/json";
            filePath = filePath.Replace("\\", "\\\\");
            string requestBody = "{\"filePath\":\"" + filePath + "\",\"params\":\"" + string.Empty + "\"}";
            byte[] byteArray = Encoding.UTF8.GetBytes(requestBody);
            using (StreamWriter writer = new StreamWriter(reqObj.GetRequestStream()))
            {
                writer.Write(requestBody);
                writer.Flush();
                writer.Close();
                var response = (HttpWebResponse)reqObj.GetResponse();

                using (var rdr = response.GetResponseStream())
                {
                    DataContractJsonSerializer deserializer = new DataContractJsonSerializer(typeof(ExecuteCodeResponse));
                    ExecuteCodeResponse executeCodeResp = (ExecuteCodeResponse)deserializer.ReadObject(rdr);
                    executeCodeResp.executedAt = DateTime.Now;
                    JobSchedulerHelper.AddZMKResponses(dataMap.GetString("id"), executeCodeResp);
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
        }
        return Task.FromResult(0);
    }
}