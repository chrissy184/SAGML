using System.Threading.Tasks;
using ZMM.Tools.Netron;


namespace ZMM.App.PyServicesClient
{
    public interface IPyNetronServiceClient
    {        
        Netron GetNetronTool();
    }
}