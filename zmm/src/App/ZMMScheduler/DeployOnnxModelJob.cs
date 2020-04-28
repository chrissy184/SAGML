using System;
using System.Threading.Tasks;
using System.Text.Json;
using Quartz;
using ZMM.App.MLEngineService;
using ZMM.Models.ResponseMessages;
using ZMM.Models.Payloads;
using System.Linq;
using ZMM.Helpers.Common;
using System.IO;
using Microsoft.Extensions.Configuration;

public class DeployOnnxModelJob : IJob
{
    private readonly IOnnxClient OnnxClient;
    private IConfiguration Configuration;

    // public DeployOnnxModelJob(IOnnxClient _onnxClient)
    // {
    //     this.OnnxClient = _onnxClient;
    // }
    public DeployOnnxModelJob()
    {
        Configuration = GetConfiguration();
        this.OnnxClient = new OnnxClient(Configuration);
    }
    public async Task Execute(IJobExecutionContext context)
    {
        #region Variables
        JobDataMap dataMap = context.JobDetail.JobDataMap;
        string filePath = dataMap.GetString("filePath");
        string zmodId = dataMap.GetString("zmodId");
        string id = dataMap.GetString("id");
        string onnxResponse = "";
        #endregion

        Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> {id} Model deploy started");

        #region deploy model

        onnxResponse = await OnnxClient.DeployModelAsync(zmodId, filePath);

        #endregion

        ModelResponse record = ModelPayload.Get().Where(i => i.Id == id).FirstOrDefault();
        //
        if (string.IsNullOrEmpty(onnxResponse) || onnxResponse.Contains("Fail@@"))
        {
            record.Deployed = false;
            record.ReasonFailed = onnxResponse.Replace("Fail@@", "");
            ModelPayload.Update(record);
        }
        else
        {
            MleResponse mle = JsonSerializer.Deserialize<MleResponse>(onnxResponse);
            //add response to ModelResponse            
            record.MleResponse = mle;
            record.Deployed = true;
            ModelPayload.Update(record);
        }

        await Task.FromResult(0);
        Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>> {id} Model deploy completed");
    }

    private IConfiguration GetConfiguration()
    {
        var builder = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .AddEnvironmentVariables();

        Configuration = builder.Build();
        return Configuration;
    }
}