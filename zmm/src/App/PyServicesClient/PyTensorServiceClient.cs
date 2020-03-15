using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.IO;
using ZMM.Tools.TB;

namespace ZMM.App.PyServicesClient
{
    public class PyTensorServiceClient : IPyTensorServiceClient
    {

        private TensorBoard TensorBoardTool;
        
        public PyTensorServiceClient(string HostURL, string RoutePrefix, int[] PortRangeInUse, string ContentDir)
        {   
            TensorBoardTool = new TensorBoard(HostURL, RoutePrefix, PortRangeInUse, ContentDir + System.IO.Path.DirectorySeparatorChar + "data");          
        }        

        public TensorBoard GetTensorBoardTool()
        {
            return this.TensorBoardTool;
        }
    }
}