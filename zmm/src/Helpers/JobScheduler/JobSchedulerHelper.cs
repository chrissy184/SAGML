using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using ZMM.Models.Payloads;
using ZMM.Models.ResponseMessages;

public static class JobSchedulerHelper
{
    public static bool AddZMKResponses(string id, string resp, string type)
    {
        bool result = false;
        var schObj = SchedulerPayload.Get().Where(s => s.Id == id).FirstOrDefault();
        JObject so = JObject.Parse(resp);
        switch (type)
        {
            case "ExecuteCode":
                var objExecute = JsonConvert.DeserializeObject<ExecuteCodeResponse>(resp);
                schObj.History.Add(objExecute);
                break;
            case "Train":
                var obj = JsonConvert.DeserializeObject<TrainingResponse>(resp);
                schObj.History.Add(obj);
                break;

        }
        //update payload
        SchedulerPayload.Update(schObj);

        return result;
    }
    public static bool AddZMKResponses(string id, object resp)
    {
        bool result = false;
        var schObj = SchedulerPayload.Get().Where(s => s.Id == id).FirstOrDefault();
        //schObj.ZMKResponse.Add(resp);
        schObj.History.Add(resp);
        //update payload
        SchedulerPayload.Update(schObj);

        return result;
    }
}