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
        internal static async System.Threading.Tasks.Task CaptureConsoleOutPutAsync(string ActionName, string ParamString, string WorkingDir, string ActualoutputFilePath)
        {

            var process = new Process();
            System.Threading.Thread CLIThread = new System.Threading.Thread( ()=>
            {
            process.StartInfo.FileName = "umoya";
            process.StartInfo.Arguments = ActionName + " " + ParamString;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.WorkingDirectory = WorkingDir;
            process.Start();
            process.WaitForExit();
            });
            CLIThread.Start();
            while(true)
            {
                if(CLIThread.IsAlive) 
                {
                    // System.Threading.Thread.Sleep(500);
                    await System.Threading.Tasks.Task.Delay(500);
                    Console.WriteLine("Executing add command.");
                }
                else 
                {
                    Console.WriteLine("Command execution finished");
                    if (!ActualoutputFilePath.Equals(string.Empty)) File.WriteAllText(ActualoutputFilePath, process.StandardOutput.ReadToEnd());
                    break;
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
