using System.IO;
using ZMM.Helpers.ZMMDirectory;

namespace ZMM.Helpers.ZMMDirectory
{
    public static class DirectoryCreator
    {
        public static bool CreateFolders(string path)
        {
            //variable
            bool result = false;
            string codePath = $"{path}/Code/";
            string dataPath = $"{path}/Data/";
            string modelPath = $"{path}/Models/";
            //
            if(!string.IsNullOrEmpty(path))
            {
                //check if code folder path exists...if not then create folder
                if (!Directory.Exists(codePath))
                {
                    Directory.CreateDirectory(codePath);
                }

                //check if data folder path exists...if not then create folder
                if (!Directory.Exists(dataPath))
                {
                    Directory.CreateDirectory(dataPath);
                }

                //check if code folder path exists...if not then create folder
                if (!Directory.Exists(modelPath))
                {
                    Directory.CreateDirectory(modelPath);
                }
            }
            return result;
        }
        
    }
}