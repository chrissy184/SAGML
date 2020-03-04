using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using ZMM.Helpers.Common;
using ZMM.Helpers.Zipper;
using ZMM.Models.ResponseMessages;

namespace ZMM.Helpers.Tests
{
    public class HelpersTest
    {

        #region FilePathHelper
        [Fact]
        public void Test_Code_GetFilePathById()
        {
            //
            List<Property> _prop = new List<Property>();
            CodeResponse record = new CodeResponse()
            {
                Id = "HelloCode",
                Name = "Hello.py",
                User = "",
                Created_on = DateTime.Now.ToString(),
                Edited_on = DateTime.Now.ToString(),
                Extension = ".py",
                MimeType = "application/image",
                Size = 111,
                Type = "PY",
                Url = "http://localhost/uploads/Code/Hello.png",
                FilePath = "some Path",
                Properties = _prop
            };

            List<CodeResponse> records = new List<CodeResponse> {
                record
            };

            string result = FilePathHelper.GetFilePathById("HelloCode", records);
            //
            Assert.Equal("some Path", result);
        }

        [Fact]
        public void Test_Data_GetFilePathById()
        {
            //
            List<Property> _prop = new List<Property>();
            DataResponse record = new DataResponse()
            {
                Id = "HelloData",
                Name = "Hello.csv",
                User = "",
                Created_on = DateTime.Now.ToString(),
                Edited_on = DateTime.Now.ToString(),
                Extension = ".csv",
                MimeType = "application/csv",
                Size = 111,
                Type = "CSV",
                Url = "http://localhost/uploads/Data/Hello.csv",
                FilePath = "/home/zmod/data/Hello.csv",
                Properties = _prop
            };

            List<DataResponse> records = new List<DataResponse> {
                record
            };

            string result = FilePathHelper.GetFilePathById("HelloData", records);
            //
            Assert.Equal("/home/zmod/data/Hello.csv", result);
        }

        [Fact]
        public void TestIsFileNameValid()
        {
            System.Console.WriteLine("Start Test : TestIsFileNameValid");
            string fileName = @"Hello.csv";

            Assert.True(FilePathHelper.IsFileNameValid(fileName));
            System.Console.WriteLine("End Test : TestIsFileNameValid");
        }
        #endregion
        #region ZipFileSanitize for positive test scenarios
        [Fact]
        public void TestZipSanitizeWithPositiveInput()
        {
            System.Console.WriteLine("Start Test : TestZipSanitizeWithPositiveInput");
            System.Console.WriteLine("Check if directory exists to get zip files from github");
             if (!Directory.Exists(TestAPIs.TestDir))
            {
                System.Console.WriteLine("Creating test directory");
                Directory.CreateDirectory(TestAPIs.TestDir);
            }
            else
            {
                System.Console.WriteLine("Deleting test directory which already exists");
                string[] filePaths = Directory.GetFiles(TestAPIs.TestDir);
                foreach (string filePath in filePaths)
                    File.Delete(filePath);
            }
            System.Console.WriteLine("Getting zip file for negative scenario");
            string fileToGetFromGitHub = "https://github.com/nimeshgit/mlw-testdata/raw/master/DisDriver.zip";
            TestAPIs.ProcessStart(fileToGetFromGitHub);
            System.Console.WriteLine("URL for test data to get from github: " + fileToGetFromGitHub);

            System.Console.WriteLine("Checking if file is sanitized");
            Assert.True(ZipHelper.SanitizeZipFile(Directory.GetFiles(TestAPIs.TestDir)[0]),"Zip file is sanitized");
           
            System.Console.WriteLine("End Test : TestZipSanitizeWithPositiveInput");
            #region  Do cleanup
            Directory.Delete(TestAPIs.TestDir, true);
            #endregion
        }
        #endregion

        #region ZipFileSanitize for negative test scenarios for Zip bomb file
        [Fact]
        public void TestZipSanitizeForZipBombNegativeInput()
        {
            System.Console.WriteLine("Start Test : TestZipSanitizeForZipBombNegativeInput");
            System.Console.WriteLine("Check if directory exists to get zip files from github");
            string fileToGetFromGitHub = "https://github.com/nimeshgit/mlw-testdata/raw/master/5GB%20ZIP%20Bomb%20fIle.zip";
            if (!Directory.Exists(TestAPIs.TestDir))
            {
                System.Console.WriteLine("Creating test directory");
                Directory.CreateDirectory(TestAPIs.TestDir);
            }
            else
            {
                System.Console.WriteLine("Deleting test directory which already exists");
                string[] filePaths = Directory.GetFiles(TestAPIs.TestDir);
                foreach (string filePath in filePaths)
                    File.Delete(filePath);
            }
            System.Console.WriteLine("Getting zip file for negative scenario");
            TestAPIs.ProcessStart(fileToGetFromGitHub);
            System.Console.WriteLine("URL for test data to get from github: " + fileToGetFromGitHub);

            System.Console.WriteLine("Checking if file is sanitized");
            string fileName = Path.GetFileName(Directory.GetFiles(TestAPIs.TestDir)[0]);
            string errorMessage = "Zip file exceeds maximum size limit 2 GB or maximum number of contents items limit 1024. Please, upload zip file which has contents size less then 2 GB.";
            var ex = Assert.Throws<Exception>(() => ZipHelper.SanitizeZipFile(Directory.GetFiles(TestAPIs.TestDir)[0]));
            Assert.Equal(errorMessage, ex.Message);

            System.Console.WriteLine("End Test : TestZipSanitizeForZipBombNegativeInput");

            #region  Do cleanup
            Directory.Delete(TestAPIs.TestDir, true);
            #endregion
        }
        #endregion

        #region ZipFileSanitize for negative test scenarios for Zip bomb file
        [Fact]
        public void TestZipSanitizeForZipBombRecursiveNegativeInput()
        {
            System.Console.WriteLine("Start Test : TestZipSanitizeForZipBombNegativeInput");
            System.Console.WriteLine("Check if directory exists to get zip files from github");
            //create folder
            string fileToGetFromGitHub = "https://github.com/nimeshgit/mlw-testdata/raw/master/13GB%20ZIP%20Bomb%20fIle.zip";
            if (!Directory.Exists(TestAPIs.TestDir))
            {
                System.Console.WriteLine("Creating test directory");
                Directory.CreateDirectory(TestAPIs.TestDir);
            }
            else
            {
                System.Console.WriteLine("Deleting test directory which already exists");
                string[] filePaths = Directory.GetFiles(TestAPIs.TestDir);
                foreach (string filePath in filePaths)
                    File.Delete(filePath);
            }
            System.Console.WriteLine("URL for test data to get from github: " + fileToGetFromGitHub);
            TestAPIs.ProcessStart(fileToGetFromGitHub);

            System.Console.WriteLine("File name to test: " + fileToGetFromGitHub);
            System.Console.WriteLine("Check if file is sanitized");

            string fileName = Path.GetFileName(Directory.GetFiles(TestAPIs.TestDir)[0]);
            string errorMessage = "Zip file exceeds maximum size limit 2 GB or maximum number of contents items limit 1024. Please, upload zip file which has contents size less then 2 GB.";
            var ex = Assert.Throws<Exception>(() => ZipHelper.SanitizeZipFile(Directory.GetFiles(TestAPIs.TestDir)[0]));
            Assert.Equal(errorMessage, ex.Message);
            System.Console.WriteLine("End Test : TestZipSanitizeForZipBombNegativeInput");

            #region  Do cleanup
            Directory.Delete(TestAPIs.TestDir, true);
            #endregion

        }
        #endregion
    }
}
