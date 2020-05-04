using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Extensions.Ordering;
using ZMM.Tools.NT;

//Optional
[assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//Optional
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]

namespace ZMM.Netron.Tests
{   
    public class UnitTests
    {
        
        static internal char DirectoryPathSeperator = System.IO.Path.DirectorySeparatorChar;
        static internal string TestDir = Environment.CurrentDirectory + DirectoryPathSeperator + "ZMOD" + DirectoryPathSeperator + "Models";
        static internal string NetronFirstResourceFile = "HelloWorld.h5";
        static internal string NetronSecondResourceFile = "HelloWorld.onnx";
        static internal string NetronThirdResourceFile = "HelloWorld.mlmodel"; 

        const string ExceptionForNonSupportedModel = "";
        static internal string NetronFourthResourceFile = "HelloWorld.pmml";
        public static ZMM.Tools.NT.Netron NetronTool; 

        const int LOWERLIMIT_ALLOWED_PORT = 8080;
        const int UPPERLIMIT_ALLOWED_PORT = 8080;
        public UnitTests()
        {
            if(NetronTool == null)
            {
                NetronTool = new ZMM.Tools.NT.Netron("http://localhost", "/nrn1", new int[] {8080,8080});
            }
        }
        /**************** Usecase (s) *****************************
        //FindWhenThereIsNoInstanceRunning
        //Start
        //GetResourceLinkForOneRunningInstance
        ****************************/


        [Fact, Order(1)]
        [Trait("Category", "Netron")]
        
        public void FindInstanceWhenThereIsNoInstanceRunning()
        {
            System.Console.WriteLine("Start Test : Find");
            Tasks.ITask NetronTemplateTask = NetronTool.FindTask("HelloWorld.h5");
            Assert.False(NetronTemplateTask.IsAlive());
            Assert.True(NetronTemplateTask.IsEmpty());
            System.Console.WriteLine("End Test : Find");
        }

        
        [Fact, Order(2)]
        [Trait("Category", "Netron")]
        public void StartSingleInstance()
        {
            System.Console.WriteLine("Start Test : Start Single Instance");
            var obj = new
            {
                base_url = "/",
                ResourcePath = NetronFirstResourceFile
            };
            NetronTool.StartTaskAsync((int)TaskTypes.Start, NetronFirstResourceFile, (JObject)JObject.FromObject(obj));
            Assert.Equal(NetronTool.GetTasks().Count, 1);
            System.Console.WriteLine("End Test : Start Single Instance");
        }      


        
        [Fact, Order(3)]
        [Trait("Category", "Netron")]
        public void PreviewAllDifferentResourcesWithValidFormats()
        {
            System.Console.WriteLine("Start Test : PreviewAllDifferentResourcesWithValidFormats");
            bool FoundException = false;
            string ExceptionMessage = string.Empty;
            try
            {
                Assert.Contains("/nrn1/", NetronTool.GetResourceLink(NetronFirstResourceFile));
                Assert.Contains("/nrn1/", NetronTool.GetResourceLink(NetronSecondResourceFile));
                Assert.Contains("/nrn1/", NetronTool.GetResourceLink(NetronThirdResourceFile));
            }
            catch (Exception ex)
            {
                FoundException = true;
                ExceptionMessage = ex.Message;
            }            
            Assert.False(FoundException, "Exception is found." + ExceptionMessage);
            System.Console.WriteLine("End Test : PreviewAllDifferentResourcesWithValidFormats");
        }

         [Fact, Order(4)]
        [Trait("Category", "Netron")]
        public void StopModelViewerService()
        {
            System.Console.WriteLine("Start Test : StopModelViewerService");
            NetronTool.GetTasks().FirstOrDefault().Value.Stop();
            System.Console.WriteLine("Stop Test : StopModelViewerService");
        }



    }
}
