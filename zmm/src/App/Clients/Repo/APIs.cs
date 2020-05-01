using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO.Compression;
using System.Threading.Tasks;

namespace ZMM.App.Clients.Repo
{
    public class APIs
    {
        internal static async System.Threading.Tasks.Task CaptureConsoleOutPutAsync(string ActionName, string ParamString, string WorkingDir, string ActualoutputFilePath, bool NeedToRedirect=false)
        {
            Process process = new Process();
            int processId = 0;
            System.Threading.Thread CLIThread = new System.Threading.Thread( ()=>
            {
                process.StartInfo.FileName = "umoya";
                process.StartInfo.Arguments = ActionName + " " + ParamString;
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.CreateNoWindow = true;
                process.StartInfo.RedirectStandardOutput = NeedToRedirect;
                process.StartInfo.WorkingDirectory = WorkingDir;
                process.Start();
                processId = process.Id;
                process.WaitForExit();
            });
            CLIThread.Start();            
            int MaxCount = 120;
            while(true)
            {
                if(!CLIThread.IsAlive || MaxCount < 1) 
                {
                    try
                    {
                        Process tempProcess = Process.GetProcessById(processId);
                        tempProcess.Kill();
                    }
                    catch(Exception ex)
                    {
                        Console.WriteLine(ex.StackTrace);
                    }
                    break;
                } 
                else 
                {
                    System.Threading.Thread.Sleep(1000);
                    MaxCount--;
                    Console.WriteLine("Executing command " + ActionName + " process id " + processId);
                }
            }
        }             

        public static async Task<bool> InitZMOD(string ZMODPath)
        {
            bool Status = true;
            await CaptureConsoleOutPutAsync("init", string.Empty, ZMODPath, string.Empty);
            await CaptureConsoleOutPutAsync("info", "-d False -sp False", ZMODPath, string.Empty);
            return Status;
        }

    }
}
