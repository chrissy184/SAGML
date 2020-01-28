using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Extensions.Ordering;
using ZMM.Tools.TB;

//Optional
[assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//Optional
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]

namespace ZMM.TB.Tests
{

   
    public class UnitTests
    {
        
        static internal char DirectoryPathSeperator = System.IO.Path.DirectorySeparatorChar;
        static internal string TestDir = Environment.CurrentDirectory + DirectoryPathSeperator + "ZMOD" + DirectoryPathSeperator + "Data";
        static internal string TBFirstResourceFile = TestDir + DirectoryPathSeperator + "HelloWorld.pmml";
        static internal string TBSecondResourceFile = TestDir + DirectoryPathSeperator + "HelloWorld2.pmml";
        static internal string TBThirdResourceFile = TestDir + DirectoryPathSeperator + "HelloWorld3.pmml"; 

        const string ExceptionAfterAllInstanceRunning = "It reaches maximum number of allowed tensorboard instance. Please, stop previously running Tensorboard from \"Assets\" or contact Administrator.";
        static internal string TBFourthResourceFile = TestDir + DirectoryPathSeperator + "HelloWorld4.pmml";
        public static TensorBoard TensorBoardTool; 

        const int LOWERLIMIT_ALLOWED_PORT = 6006;
        const int UPPERLIMIT_ALLOWED_PORT = 6008;
        public UnitTests()
        {
            if(TensorBoardTool == null)
            {
                string LogDirectory = System.Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + "logs";
                if(!Directory.Exists(LogDirectory)) Directory.CreateDirectory(LogDirectory);
                TensorBoardTool = new TensorBoard("http://localhost", LogDirectory);
            }
        }
        /**************** Usecase (s) *****************************
        //FindWhenThereIsNoInstanceRunning
        //Start
        //Stop
        //GetResourceLinkForOneRunningInstance
        //Start all
        //Stop all
        //Start all 3 and start 4 th one
        //Start without TB installed
        //Start all 3 and close 2nd then start
        ****************************/


        [Fact, Order(1)]
        [Trait("Category", "TB")]
        
        public void FindInstanceWhenThereIsNoInstanceRunning()
        {
            System.Console.WriteLine("Start Test : Find");
            Tasks.ITask TensorBoardTask = TensorBoardTool.FindTask("HelloWorld.pmml");
            Assert.False(TensorBoardTask.IsAlive());
            Assert.True(TensorBoardTask.IsEmpty());
            System.Console.WriteLine("End Test : Find");
        }

        
        [Fact, Order(2)]
        [Trait("Category", "TB")]
        public void StartSingleInstance()
        {
            System.Console.WriteLine("Start Test : Start Single Instance");
            var obj = new
            {
                base_url = "/",
                ResourcePath = TBFirstResourceFile
            };
            TensorBoardTool.StartTaskAsync((int)TaskTypes.Start, TBFirstResourceFile, (JObject)JObject.FromObject(obj));
            System.Threading.Thread.Sleep(500);
            Assert.True(TensorBoardTool.IsPortBusyInRange(6006, LOWERLIMIT_ALLOWED_PORT, UPPERLIMIT_ALLOWED_PORT));
            Assert.Equal(TensorBoardTool.GetTasks().Count, 1);
            System.Console.WriteLine("End Test : Start Single Instance");
        }

        
        [Fact, Order(3)]
        [Trait("Category", "TB")]
        public void FindWhenThereIsOneInstanceRunning()
        {
            System.Console.WriteLine("Start Test : FindWhenThereIsOneInstanceRunning");
            Tasks.ITask TensorBoardTask = TensorBoardTool.FindTask(TBFirstResourceFile);
            Assert.True(TensorBoardTask.IsAlive());
            Assert.False(TensorBoardTask.IsEmpty());
            System.Console.WriteLine("End Test : FindWhenThereIsOneInstanceRunning");
        }


        
        [Fact, Order(4)]
        [Trait("Category", "TB")]
        public void StopSingleInstance()
        {
            System.Console.WriteLine("Start Test : StopSingleInstance");
            Tasks.ITask TensorBoardTask = TensorBoardTool.FindTask(TBFirstResourceFile);
            Assert.True(TensorBoardTask.IsAlive());
            Assert.False(TensorBoardTask.IsEmpty());
            TensorBoardTool.StopTask(TBFirstResourceFile);
            System.Threading.Thread.Sleep(500);
            TensorBoardTask = TensorBoardTool.FindTask(TBFirstResourceFile);
            Assert.False(TensorBoardTask.IsAlive());
            Assert.True(TensorBoardTask.IsEmpty());
            System.Console.WriteLine("End Test : StopSingleInstance");
        }

        
        [Fact, Order(5)]
        [Trait("Category", "TB")]
        public void GetResourceLinkForOneRunningInstance()
        {
            System.Console.WriteLine("Start Test : GetResourceLinkForOneRunningInstance");
            StartSingleInstance();
            string LogDirecotry = string.Empty;
            Assert.Contains("/tb1/", TensorBoardTool.GetResourceLink(TBFirstResourceFile, out LogDirecotry));
            StopSingleInstance();
            System.Console.WriteLine("End Test : GetResourceLinkForOneRunningInstance");
        }

        
        
        [Fact, Order(6)]
        [Trait("Category", "TB")]
        public void StartAllAllowedInstances()
        {
            System.Console.WriteLine("Start Test : StartAllAllowedInstances");
            var obj = new
            {
                base_url = "/",
                ResourcePath = TBFirstResourceFile
            };
            TensorBoardTool.StartTaskAsync((int)TaskTypes.Start, TBFirstResourceFile, (JObject)JObject.FromObject(obj));
            System.Threading.Thread.Sleep(500);
            Assert.True(TensorBoardTool.IsPortBusyInRange(6006, LOWERLIMIT_ALLOWED_PORT, UPPERLIMIT_ALLOWED_PORT));

            var obj2 = new
            {
                base_url = "/",
                ResourcePath = TBSecondResourceFile
            };
            TensorBoardTool.StartTaskAsync((int)TaskTypes.Start, TBSecondResourceFile, (JObject)JObject.FromObject(obj2));
            System.Threading.Thread.Sleep(500);
            Assert.True(TensorBoardTool.IsPortBusyInRange(6007, LOWERLIMIT_ALLOWED_PORT, UPPERLIMIT_ALLOWED_PORT));

            var obj3 = new
            {
                base_url = "/",
                ResourcePath = TBThirdResourceFile
            };
            TensorBoardTool.StartTaskAsync((int)TaskTypes.Start, TBThirdResourceFile, (JObject)JObject.FromObject(obj3));
            System.Threading.Thread.Sleep(500);
            Assert.True(TensorBoardTool.IsPortBusyInRange(6008, LOWERLIMIT_ALLOWED_PORT, UPPERLIMIT_ALLOWED_PORT));
            System.Console.WriteLine("End Test : StartAllAllowedInstances");
        }


        
        [Fact, Order(7)]
        [Trait("Category", "TB")]
        public void StartAllAllowedInstancesAndOneMore()
        {
            bool FoundException = false;
            string ExceptionMessage = string.Empty;
            try
            {
                System.Console.WriteLine("Start Test : StartAllAllowedInstancesAndOneMore");
                var obj4 = new
                {
                    base_url = "/",
                    ResourcePath = TBFourthResourceFile
                };
                TensorBoardTool.StartTaskAsync((int)TaskTypes.Start, TBFourthResourceFile, (JObject)JObject.FromObject(obj4));
                System.Threading.Thread.Sleep(500);
            }
            catch (Exception ex)
            {
                FoundException = true;
                ExceptionMessage = ex.Message;
            }
            Assert.True(FoundException, "Exception is not found.");
            Assert.Equal(ExceptionAfterAllInstanceRunning, ExceptionMessage);
            System.Console.WriteLine("End Test : StartAllAllowedInstancesAndOneMore");
        }


        
        [Fact, Order(8)]
        [Trait("Category", "TB")]
        public void StartAllAllowedInstancesCloseSecondAndThenStartNewInstance()
        {
            System.Console.WriteLine("Start Test : StartAllAllowedInstancesCloseSecondAndThenStartNewInstance");
            TensorBoardTool.StopTask(TBSecondResourceFile);
            System.Threading.Thread.Sleep(500);
            var obj4 = new
            {
                base_url = "/",
                ResourcePath = TBFourthResourceFile
            };
            TensorBoardTool.StartTaskAsync((int)TaskTypes.Start, TBFourthResourceFile, (JObject)JObject.FromObject(obj4));
            System.Threading.Thread.Sleep(500);
            string TempLogDirectory;
            Assert.Contains("/tb2/", TensorBoardTool.GetResourceLink(TBFourthResourceFile, out TempLogDirectory));
            System.Console.WriteLine("End Test : StartAllAllowedInstancesCloseSecondAndThenStartNewInstance");
        }

        
        [Fact, Order(9)]
        [Trait("Category", "TB")]
        public void StopAllAllowedInstances()
        {
            System.Console.WriteLine("Start Test : StopAllAllowedInstances");
            TensorBoardTool.StopTask(TBFirstResourceFile);
            System.Threading.Thread.Sleep(500);
            TensorBoardTool.StopTask(TBFourthResourceFile);
            System.Threading.Thread.Sleep(500);
            TensorBoardTool.StopTask(TBThirdResourceFile);
            System.Threading.Thread.Sleep(500);
            Assert.False(TensorBoardTool.IsPortBusyInRange(6006, LOWERLIMIT_ALLOWED_PORT, UPPERLIMIT_ALLOWED_PORT), "First Tensorboard is not running.");
            Assert.False(TensorBoardTool.IsPortBusyInRange(6007, LOWERLIMIT_ALLOWED_PORT, UPPERLIMIT_ALLOWED_PORT), "Forth Tensorboard is not running.");
            Assert.False(TensorBoardTool.IsPortBusyInRange(6008, LOWERLIMIT_ALLOWED_PORT, UPPERLIMIT_ALLOWED_PORT), "Third Tensorboard is not running");
            System.Console.WriteLine("End Test : StopAllAllowedInstances");
        }

    }
}
