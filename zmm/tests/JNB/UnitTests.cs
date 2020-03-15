using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;
using Xunit;
using Xunit.Extensions.Ordering;
using ZMM.Tools.JNB;

//Optional
[assembly: CollectionBehavior(DisableTestParallelization = true)]
//Optional
[assembly: TestCaseOrderer("Xunit.Extensions.Ordering.TestCaseOrderer", "Xunit.Extensions.Ordering")]
//Optional
[assembly: TestCollectionOrderer("Xunit.Extensions.Ordering.CollectionOrderer", "Xunit.Extensions.Ordering")]

namespace ZMM.JNB.Tests
{

   
    public class UnitTests
    {
        
        static internal char DirectoryPathSeperator = System.IO.Path.DirectorySeparatorChar;
        static internal string TestDir = Environment.CurrentDirectory + DirectoryPathSeperator + "ZMOD" + DirectoryPathSeperator + "Code";
        static internal string JNBFirstFile = TestDir + DirectoryPathSeperator + "HelloWorld.ipynb" + DirectoryPathSeperator + "HelloWorld.ipynb";
        static internal string JNBSecondFile = TestDir + DirectoryPathSeperator + "HelloWorld2.ipynb" + DirectoryPathSeperator + "HelloWorld2.ipynb";
        static internal string JNBThirdFile = TestDir + DirectoryPathSeperator + "HelloWorld3.ipynb" + DirectoryPathSeperator + "HelloWorld3.ipynb";

        static readonly string ExceptionAfterAllInstanceRunning = "It reaches maximum number of allowed jupyter notebook instance. Please, stop previously running notebook from \"Assets\" or contact Administrator.";
        static internal string JNBFourthFile = TestDir + DirectoryPathSeperator + "HelloWorld4.ipynb" + DirectoryPathSeperator + "HelloWorld4.ipynb";
        public static JupyterNotebook JupyterNotebookTool = new JupyterNotebook("http://localhost", "/jnb", new int[] {8888,8890});

        /**************** Usecase (s) *****************************
        //FindWhenThereIsNoInstanceRunning
        //Start
        //Stop
        //GetResourceLinkForOneRunningInstance
        //Start all
        //Stop all
        //Start all 3 and start 4 th one
        //Start without Notebook installed
        //Start all 3 and close 2nd then start
        ****************************/


        [Fact, Order(1)]
        [Trait("Category", "JNB")]
        
        public void FindWhenThereIsNoInstanceRunning()
        {
            System.Console.WriteLine("Start Test : Find");
            Tasks.ITask JupyterNotebookTask = JupyterNotebookTool.FindTask("HelloWorld.ipynb");
            Assert.False(JupyterNotebookTask.IsAlive());
            Assert.True(JupyterNotebookTask.IsEmpty());
            System.Console.WriteLine("End Test : Find");
        }

        
        [Fact, Order(2)]
        [Trait("Category", "JNB")]
        public void StartSingleInstance()
        {
            System.Console.WriteLine("Start Test : Start Single Instance");
            var obj = new
            {
                base_url = "/",
                NotebookDir = TestDir + System.IO.Path.DirectorySeparatorChar + "HelloWorld.ipynb",
                ResourcePath = JNBFirstFile
            };
            JupyterNotebookTool.StartTaskAsync((int)TaskTypes.Start, JNBFirstFile, (JObject)JObject.FromObject(obj));
            System.Threading.Thread.Sleep(2000);
            Assert.True(JupyterNotebookTool.IsPortBusyInRange(8888, 8888, 8890));
            Assert.Equal(JupyterNotebookTool.GetTasks().Count, 1);
            System.Console.WriteLine("End Test : Start Single Instance");
        }

        
        [Fact, Order(3)]
        [Trait("Category", "JNB")]
        public void FindWhenThereIsOneInstanceRunning()
        {
            System.Console.WriteLine("Start Test : FindWhenThereIsOneInstanceRunning");
            Tasks.ITask JupyterNotebookTask = JupyterNotebookTool.FindTask(JNBFirstFile);
            Assert.True(JupyterNotebookTask.IsAlive());
            Assert.False(JupyterNotebookTask.IsEmpty());
            System.Console.WriteLine("End Test : FindWhenThereIsOneInstanceRunning");
        }


        
        [Fact, Order(4)]
        [Trait("Category", "JNB")]
        public void StopSingleInstance()
        {
            System.Console.WriteLine("Start Test : StopSingleInstance");
            Tasks.ITask JupyterNotebookTask = JupyterNotebookTool.FindTask(JNBFirstFile);
            Assert.True(JupyterNotebookTask.IsAlive());
            Assert.False(JupyterNotebookTask.IsEmpty());
            JupyterNotebookTool.StopTask(JNBFirstFile);
            System.Threading.Thread.Sleep(2000);
            JupyterNotebookTask = JupyterNotebookTool.FindTask(JNBFirstFile);
            Assert.False(JupyterNotebookTask.IsAlive());
            Assert.True(JupyterNotebookTask.IsEmpty());
            System.Console.WriteLine("End Test : StopSingleInstance");
        }

        
        [Fact, Order(5)]
        [Trait("Category", "JNB")]
        public void GetResourceLinkForOneRunningInstance()
        {
            System.Console.WriteLine("Start Test : GetResourceLinkForOneRunningInstance");
            StartSingleInstance();
            Assert.Contains("/jnb1/notebooks/HelloWorld.ipynb", JupyterNotebookTool.GetResourceLink(JNBFirstFile));
            StopSingleInstance();
            System.Console.WriteLine("End Test : GetResourceLinkForOneRunningInstance");
        }

        
        
        [Fact, Order(6)]
        [Trait("Category", "JNB")]
        public void StartAllAllowedInstances()
        {
            JupyterNotebookTool = new JupyterNotebook("http://localhost", "/jnb", new int[] {8888,8890});
            System.Console.WriteLine("Start Test : StartAllAllowedInstances");
            var obj = new
            {
                base_url = "/",
                NotebookDir = TestDir + System.IO.Path.DirectorySeparatorChar + "HelloWorld.ipynb",
                ResourcePath = JNBFirstFile
            };
            JupyterNotebookTool.StartTaskAsync((int)TaskTypes.Start, JNBFirstFile, (JObject)JObject.FromObject(obj));
            System.Threading.Thread.Sleep(2000);
            Assert.True(JupyterNotebookTool.IsPortBusyInRange(8888, 8888, 8890));

            var obj2 = new
            {
                base_url = "/",
                NotebookDir = TestDir + System.IO.Path.DirectorySeparatorChar + "HelloWorld2.ipynb",
                ResourcePath = JNBSecondFile
            };
            JupyterNotebookTool.StartTaskAsync((int)TaskTypes.Start, JNBSecondFile, (JObject)JObject.FromObject(obj2));
            System.Threading.Thread.Sleep(2000);
            Assert.True(JupyterNotebookTool.IsPortBusyInRange(8889, 8888, 8890));

            var obj3 = new
            {
                base_url = "/",
                NotebookDir = TestDir + System.IO.Path.DirectorySeparatorChar + "HelloWorld3.ipynb",
                ResourcePath = JNBThirdFile
            };
            JupyterNotebookTool.StartTaskAsync((int)TaskTypes.Start, JNBThirdFile, (JObject)JObject.FromObject(obj3));
            System.Threading.Thread.Sleep(2000);
            Assert.True(JupyterNotebookTool.IsPortBusyInRange(8890, 8888, 8890));
            System.Console.WriteLine("End Test : StartAllAllowedInstances");
        }


        
        [Fact, Order(7)]
        [Trait("Category", "JNB")]
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
                    NotebookDir = TestDir + System.IO.Path.DirectorySeparatorChar + "HelloWorld4.ipynb",
                    ResourcePath = JNBFourthFile
                };
                JupyterNotebookTool.StartTaskAsync((int)TaskTypes.Start, JNBFourthFile, (JObject)JObject.FromObject(obj4));
                System.Threading.Thread.Sleep(2000);
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
        [Trait("Category", "JNB")]
        public void StartAllAllowedInstancesCloseSecondAndThenStartNewInstance()
        {
            System.Console.WriteLine("Start Test : StartAllAllowedInstancesCloseSecondAndThenStartNewInstance");
            JupyterNotebookTool.StopTask(JNBSecondFile);
            System.Threading.Thread.Sleep(2000);
            var obj4 = new
            {
                base_url = "/",
                NotebookDir = TestDir + System.IO.Path.DirectorySeparatorChar + "HelloWorld4.ipynb",
                ResourcePath = JNBFourthFile
            };
            JupyterNotebookTool.StartTaskAsync((int)TaskTypes.Start, JNBFourthFile, (JObject)JObject.FromObject(obj4));
            System.Threading.Thread.Sleep(2000);
            Assert.Contains("/jnb2/notebooks/HelloWorld4.ipynb", JupyterNotebookTool.GetResourceLink(JNBFourthFile));
            System.Console.WriteLine("End Test : StartAllAllowedInstancesCloseSecondAndThenStartNewInstance");
        }

        
        [Fact, Order(9)]
        [Trait("Category", "JNB")]
        public void StopAllAllowedInstances()
        {
            System.Console.WriteLine("Start Test : StopAllAllowedInstances");
            JupyterNotebookTool.StopTask(JNBFirstFile);
            System.Threading.Thread.Sleep(2000);
            JupyterNotebookTool.StopTask(JNBFourthFile);
            System.Threading.Thread.Sleep(2000);
            JupyterNotebookTool.StopTask(JNBThirdFile);
            System.Threading.Thread.Sleep(2000);
            Assert.False(JupyterNotebookTool.IsPortBusyInRange(8888, 8888, 8890), "First Notebook is not running.");
            Assert.False(JupyterNotebookTool.IsPortBusyInRange(8889, 8888, 8890), "Forth Notebook is not running.");
            Assert.False(JupyterNotebookTool.IsPortBusyInRange(8890, 8888, 8890), "Third Notebook is not running");
            System.Console.WriteLine("End Test : StopAllAllowedInstances");
        }


        /*
        
        [Fact]
        [Trait("Category", "Default")]
        public void TestFindStart()
        {
            System.Console.WriteLine("Start Test : FindStart");    
            string NotebookFile = TestDir + "/data/HelloClass/HelloClass.ipynb";
            JupyterNotebook.Init("/home/nimesh/Zementis/Software/jupyter/bin/jupyter-notebook", "http://localhost");
            var obj = new
            {        
                notebook_dir = $"{TestDir}"                          
            };            
            NotebookInfo NewBook = new NotebookInfo(obj);            
            JupyterNotebook nb = JupyterNotebook.Find(NotebookFile);            
            Assert.Null(nb);
            nb = new JupyterNotebook(NewBook);
            nb.Start();       
            System.Threading.Thread.Sleep(5000);                            
            nb = JupyterNotebook.Find(NotebookFile);            
            Assert.NotNull(nb);  
            Assert.True(nb.Info.Status == JupyterNotebook.Status.Running); 
            Assert.True(nb.Info.Pid > 0);  
            Assert.True(nb.Info.Port == 8888); 
            string Ls = nb.Link(NotebookFile);
            System.Console.WriteLine("End Test : FindStart");                            
        }
        

        [Fact]
        public void TestFindStopStart()
        {
            System.Console.WriteLine("Start Test : FindStopStart");
            string NotebookFile = TestDir + "/data/HelloClass/HelloClass.ipynb";
            JupyterNotebook.Init("/home/nimesh/Zementis/Software/jupyter/bin/jupyter-notebook", "http://localhost");
            JupyterNotebook nb = JupyterNotebook.Find(NotebookFile);     
            Assert.NotNull(nb);
            Assert.True(nb.Info.Status == JupyterNotebook.Status.Running);                   
            nb.Stop();
            Task.Delay(20000).Wait(); 
            nb = JupyterNotebook.Find(NotebookFile);             
            Assert.Null(nb);  
            System.Console.WriteLine("End Test : FindStopStart");      
        }
        */

    }
}
