using System;
using System.IO;
using System.Threading.Tasks;

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

        public async Task Init(string RepoURL, string RepoVersion, string RepoPAT)
        {
            await APIs.InitZMOD(ResourceDirectory);
            await UpdateInfo(RepoURL + "/" + RepoVersion + "/index.json", RepoPAT);
        }

        private async Task UpdateInfo(string RepoURL, string RepoPAT)
        {
            if(File.Exists(Constants.UMOYACLIOutputFile)) File.Delete(Constants.UMOYACLIOutputFile);
            await APIs.CaptureConsoleOutPutAsync("info", "-u " + RepoURL + " -k " + RepoPAT, ResourceDirectory, Constants.UMOYACLIOutputFile);
        }


        public async Task Add(Resource ResourceInfo)
        {
            Console.WriteLine("UMOYA add " + ResourceInfo.Id + "@" + ResourceInfo.Version);
            if(File.Exists(Constants.UMOYACLIOutputFile)) File.Delete(Constants.UMOYACLIOutputFile);
            await APIs.CaptureConsoleOutPutAsync("add", ResourceInfo.Id + "@" + ResourceInfo.Version, ResourceDirectory, Constants.UMOYACLIOutputFile);
        }

    }
}
