using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using ZMM.Tools.NT;

namespace ZMM.App.PyServicesClient
{
    public class PyNetronServiceClient : IPyNetronServiceClient
    {
        
        private Netron NetronTool;
        
        public PyNetronServiceClient(string HostURL, string RoutePrefix, int[] PortRangeInUse)
        {  
            NetronTool = new Netron(HostURL, RoutePrefix, PortRangeInUse);          
        }      

        public Netron GetNetronTool()
        {
            return this.NetronTool;
        }
        


    }
}