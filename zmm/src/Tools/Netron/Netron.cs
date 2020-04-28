using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Diagnostics;
using System.IO;
using ZMM.Tasks;
using Microsoft.Extensions;
using TaskFactory = ZMM.Tasks.TaskFactory;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using Task = ZMM.Tasks.Task;
using System.Text.RegularExpressions;

/// <summary>
/// see bottom of file for developer notes
/// </summary>
namespace ZMM.Tools.Netron
{

    public enum TaskTypes
    {
        Start,
        List,
        Stop,
        Kill
    }

    public class Netron : Tool
    {
        protected static string HostURL = "http://localhost";
        private const string TokenPattern = "?token=";
        private static string RoutePrefix = "/modelviewer";
        private static bool IsNetronStarted = false;
        private static string NetronTemplateResource = "Netron";

        //We can add this to configuration
        private static List<int> ListOfAllowedPorts = new List<int> { 8080 };

        public Netron(string Host, string HostRoutePrefix, int[] InstancePortsToUse) : base(ToolTypes.Netron, Host.Contains("https"))
        {
            HostURL = Host;
            RoutePrefix = HostRoutePrefix;
            InitInstancePortsInUse(InstancePortsToUse, ref ListOfAllowedPorts);            
        }

        public void StartTaskAsync(int taskType, string taskName, JObject info)
        {
            switch ((TaskTypes)taskType)
            {
                case TaskTypes.Start:
                    if (!IsNetronStarted)
                    {
                        ITask tempTask = FindTask(taskName);
                        if (tempTask.IsEmpty())
                        {
                            int FreePort = GetAvailablePort(ListOfAllowedPorts.First(), ListOfAllowedPorts.Last());
                            if (FreePort > 0)
                            {
                                UpdateStartTaskInfo(ref info, FreePort);
                                tempTask = TaskFactory.Get(taskType, taskName, this, info);
                                tempTask.StartAsync();
                                string token = WaitForStartTaskToken(tempTask, FreePort);
                                if (token.Equals(string.Empty)) throw new Exception("Something went wrong. Jupyter notebook cannot be started. Try again.");
                                tempTask.UpdateInput("Token", token);
                                AddTask(tempTask.GetName(), tempTask);
                                IsNetronStarted = true;
                            }
                            else throw new Exception("It reaches maximum number of allowed Model Viewer (Netron) instance. Please, contact Administrator.");
                        }
                    }
                    break;
                default:
                    base.StartTaskAsync(taskType, taskName, info);
                    break;
            }

        }        

        private int GetPortFromLiveTask(ITask task)
        {
            if (task.IsAlive()) return int.Parse(task.GetInput().MetaData["Port"]);
            else return -1;
        }

        private void UpdateStartTaskInfo(ref JObject info, int port)
        {
            string NotebookConfigFile = Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + "config" + System.IO.Path.DirectorySeparatorChar + "zementis.notebook.config.py";
            string NotebookCertFile = Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + "config" + System.IO.Path.DirectorySeparatorChar + "zmm.jnb.pem";
            string NotebookKeyFile = Environment.CurrentDirectory + System.IO.Path.DirectorySeparatorChar + "config" + System.IO.Path.DirectorySeparatorChar + "zmm.jnb.key";
            string LinkPrefixString = GetLinkPrefix();
            bool IsProductionEnvironment = !HostURL.Contains("localhost");
            info.Add("Port", port.ToString());
            info.Add("NotebookConfigFile", NotebookConfigFile);
            info.Add("LinkPrefix", LinkPrefixString);
            info.Add("NotebookCertFile", NotebookCertFile);
            info.Add("NotebookKeyFile", NotebookKeyFile);
        }

        private void UpdateStopTaskInfo(ref JObject info, int port)
        {
            info.Add("Port", port.ToString());
        }

        private string GetLinkPrefix()
        {
            string Prefix = RoutePrefix;
            return Prefix;
        }

        public string GetResourceLink(string resourcePath)
        {
            string outLink = "Error when getting link.";
            try
            {
                if(!IsNetronStarted)
                {
                    #region start netron with template
                    var obj = new
                    {
                        base_url = "/",
                        ResourcePath = $"{NetronTemplateResource}"
                    };
                    StartTaskAsync((int)ZMM.Tools.Netron.TaskTypes.Start, NetronTemplateResource, (JObject)JObject.FromObject(obj));
                    #endregion
                }      
                outLink = Netron.HostURL + GetLinkPrefix() + "/?url=" + Netron.HostURL + "/api/model/download/" + resourcePath;
            }
            catch(Exception ex)
            {
                outLink = ex.Message;
            }
            return outLink;
        }

        private string WaitForStartTaskToken(ITask task, int TaskPort)
        {
            string tokenId = string.Empty;
            for (int i = 1; i < 50; i++)
            {
                if (tokenId != string.Empty) break;
                else
                {
                    if (IsPortBusyInRange(TaskPort, ListOfAllowedPorts.First(), ListOfAllowedPorts.Last()))
                    {
                        Console.WriteLine("Waiting for token id...");
                        tokenId = "NetronNoToken";
                    }
                    else Console.WriteLine("Waiting for task expected port " + TaskPort + "  to start");
                    System.Threading.Thread.Sleep(500);
                }
            }
            System.Threading.Thread.Sleep(500);
            return tokenId;
        }

    }
}
