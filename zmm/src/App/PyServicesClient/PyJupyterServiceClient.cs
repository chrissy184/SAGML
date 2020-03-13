using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using ZMM.Tools.JNB;

namespace ZMM.App.PyServicesClient
{
    public class PyJupyterServiceClient : IPyJupyterServiceClient
    {
        
        private JupyterNotebook JupyterNotebookTool;
        
        public PyJupyterServiceClient(string HostURL, string RoutePrefix, int[] PortRangeInUse)
        {  
            JupyterNotebookTool = new JupyterNotebook(HostURL, RoutePrefix, PortRangeInUse);          
        }      

        public JupyterNotebook GetJupyterNotebookTool()
        {
            return this.JupyterNotebookTool;
        }
        


    }
}