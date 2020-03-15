using System;
using System.Diagnostics;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.IO.Compression;

namespace ZMM.App.Clients.Repo
{
    public class APIs
    {
        internal static void CaptureConsoleOutPut(string ActionName, string ParamString, string WorkingDir, string ActualoutputFilePath)
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
                    System.Threading.Thread.Sleep(500);
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

        public static bool InitZMOD(string ZMODPath)
        {
            bool Status = true;
            CaptureConsoleOutPut("init", string.Empty, ZMODPath, string.Empty);
            CaptureConsoleOutPut("info", "-d False -sp False", ZMODPath, string.Empty);
            return Status;
        }

    }
}
