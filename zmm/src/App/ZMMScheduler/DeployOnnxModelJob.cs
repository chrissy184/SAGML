using System;
using System.Threading.Tasks;
using System.Text.Json;
using Quartz;
using ZMM.App.MLEngineService;
using ZMM.Models.ResponseMessages;
using ZMM.Models.Payloads;
using System.Linq;
using ZMM.Helpers.Common;

namespace MLW.SchedulerService
{
    public class DeployOnnxModelJob : IJob
    {
        private readonly IOnnxClient OnnxClient;

        public DeployOnnxModelJob(IOnnxClient _onnxClient)
        {
           this.OnnxClient = _onnxClient;
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

            Console.WriteLine($">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>Model deploy started at {baseAddress} -");

            #region deploy model

            onnxResponse = await OnnxClient.DeployModelAsync(zmodId, filePath);            

            #endregion

            ModelResponse record = ModelPayload.Get().Where(i => i.Id == id).FirstOrDefault();
            //
            if (string.IsNullOrEmpty(onnxResponse) || onnxResponse.Contains("Fail@@"))
            {
                record.Deployed = false;
                record.ReasonFailed = onnxResponse.Replace("Fail@@","");
                ModelPayload.Update(record);
                return;
            }
            MleResponse mle = JsonSerializer.Deserialize<MleResponse>(onnxResponse);
            //add response to ModelResponse            
            record.MleResponse = mle;
            record.Deployed = true;            
            ModelPayload.Update(record);

            return;
        }
    }
}