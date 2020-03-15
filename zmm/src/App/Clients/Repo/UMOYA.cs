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
                    instance.Init();
                }
                return instance;                
            }
        }

        public UMOYA()
        {
            ResourceDirectory = new DirectoryInfo(Helpers.ZMMDirectory.DirectoryHelper.GetDataDirectoryPath()).Parent.FullName;
        }

        private void Init()
        {
            APIs.InitZMOD(ResourceDirectory);
            UpdateInfo("https://hub.umoya.ai/v3/index.json");
        }

        private void UpdateInfo(string RepoURL)
        {
            if(File.Exists(Constants.UMOYACLIOutputFile)) File.Delete(Constants.UMOYACLIOutputFile);
            APIs.CaptureConsoleOutPut("info", "-u " + RepoURL, ResourceDirectory, Constants.UMOYACLIOutputFile);
        }

        public void Add(Resource ResourceInfo)
        {
            Console.WriteLine("UMOYA add " + ResourceInfo.Id + "@" + ResourceInfo.Version);
            if(File.Exists(Constants.UMOYACLIOutputFile)) File.Delete(Constants.UMOYACLIOutputFile);
            APIs.CaptureConsoleOutPut("add", ResourceInfo.Id + "@" + ResourceInfo.Version, ResourceDirectory, Constants.UMOYACLIOutputFile);
        }

    }
}
