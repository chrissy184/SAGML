using System;
using System.IO;
using System.IO.Compression;
using System.Security.Permissions;
using System.Threading.Tasks;
using ZMM.Helpers.ZMMDirectory;

namespace ZMM.Helpers.Zipper
{
    /// <summary>
    /// This class is used for unzip the .zip or .7z
    /// </summary>
    public class ZipHelper
    {       
        string zipOr7zPath = string.Empty;
        string extractPath = string.Empty;

        private static readonly long MAXLIMITOFEXTRATEDFILESIZE = 2147483648;

        private const int MAXIMUMNUMBEROFFILES = 100000;

        private static FileSystemWatcher DirectoryWatcher;

        #region Constructor
        public ZipHelper()
        {

        }
        #endregion

        #region Extract archive file
        public static async Task<bool> ExtractAsync(string zipOr7zPath, string extractPath)
        {
            bool result = false;   
            if ((!string.IsNullOrEmpty(zipOr7zPath)) && (!string.IsNullOrEmpty(extractPath)))
            {                      
                if(SanitizeZipFile(zipOr7zPath))    
                {      
                    ZipFile.ExtractToDirectory(zipOr7zPath, extractPath);                    
                    await Task.FromResult(0);
                    result = true;
                }
            }
            return result;
        }
        #endregion

        #region Zip Bomb resolutions - To restrict zip file contents size before extraction
        public static bool SanitizeZipFile(string FilePath)
        {
            bool IsFileSanitized = true;
            long TempSumOfVolume = 0;
            int NumberOfFiles = 0;
            if(Path.GetExtension(FilePath).Contains("zip")) 
            {
                using (ZipArchive archive = ZipFile.OpenRead(FilePath))
                {
                    foreach (ZipArchiveEntry entry in archive.Entries)
                    {
                        NumberOfFiles++;
                        TempSumOfVolume += entry.Length; 
                        Console.WriteLine ("UnZipping file : " + FilePath + " Size : " + entry.Length); 
                        if(entry.Length > MAXLIMITOFEXTRATEDFILESIZE || TempSumOfVolume > MAXLIMITOFEXTRATEDFILESIZE || NumberOfFiles > MAXIMUMNUMBEROFFILES) 
                        {
                            throw new Exception("Zip file exceeds maximum size limit 2 GB or maximum number of contents items limit 1024. Please, upload zip file which has contents size less then 2 GB.");   
                        }
                    }
                } 
            }
            else throw new Exception("Not supported file type");
            return IsFileSanitized;
        }      
        
        #endregion
    }
}
