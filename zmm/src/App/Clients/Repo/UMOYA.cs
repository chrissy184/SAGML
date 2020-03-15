using System;
using System.IO;

namespace ZMM.App.Clients.Repo
{
    public class UMOYA
    {
        string ResourceDirectory;

        private static UMOYA instance;

        public static UMOYA Instance 
        { 
            get 
            { 
                if(instance==null)
                {
                    instance = new UMOYA();
                }
                return instance;                
            }
        }

        public UMOYA()
        {
            ResourceDirectory = new DirectoryInfo(Helpers.ZMMDirectory.DirectoryHelper.GetDataDirectoryPath()).Parent.FullName;
        }

        public void Init(string RepoURL, string RepoVersion, string RepoPAT)
        {
            APIs.InitZMOD(ResourceDirectory);
            UpdateInfo(RepoURL + "/" + RepoVersion + "/index.json", RepoPAT);
        }

        private void UpdateInfo(string RepoURL, string RepoPAT)
        {
            if(File.Exists(Constants.UMOYACLIOutputFile)) File.Delete(Constants.UMOYACLIOutputFile);
            APIs.CaptureConsoleOutPut("info", "-u " + RepoURL + " -k " + RepoPAT, ResourceDirectory, Constants.UMOYACLIOutputFile);
        }


        public void Add(Resource ResourceInfo)
        {
            Console.WriteLine("UMOYA add " + ResourceInfo.Id + "@" + ResourceInfo.Version);
            if(File.Exists(Constants.UMOYACLIOutputFile)) File.Delete(Constants.UMOYACLIOutputFile);
            APIs.CaptureConsoleOutPut("add", ResourceInfo.Id + "@" + ResourceInfo.Version, ResourceDirectory, Constants.UMOYACLIOutputFile);
        }

    }
}
