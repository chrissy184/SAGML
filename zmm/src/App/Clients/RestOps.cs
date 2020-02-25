using System;
using System.Net.Http;

namespace ZMM.App.Clients
{
    public static class RestOps
    {
        public static async System.Threading.Tasks.Task<bool> IsEndPointPresentAsync(string EndPointURL)
        {
            bool Status = false;            
            HttpResponseMessage Response = await GetResponseAsync(EndPointURL);
            if (Response.IsSuccessStatusCode) return true;
            return Status;
        }

        public static async System.Threading.Tasks.Task<HttpResponseMessage> GetResponseAsync(string EndPointURL)
        {            
            var httpClient = new HttpClient();
            return await httpClient.GetAsync(EndPointURL);
        }

        public static string AppendQueryInEndPoint(string EndPointURL, string VariableName, string VariableValue)
        {
            string AppendChar = "&";
            if(!EndPointURL.Contains('?')) AppendChar = "?";
            return EndPointURL + AppendChar + VariableName + "=" + VariableValue;
        }
    }
}
