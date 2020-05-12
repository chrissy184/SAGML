using System.Threading.Tasks;
using ZMM.Tools.NT;


namespace ZMM.App.PyServicesClient
{
    public interface IPyNetronServiceClient
    {        
        Netron GetNetronTool();
    }
}