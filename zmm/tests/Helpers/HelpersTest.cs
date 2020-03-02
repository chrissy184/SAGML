using System;
using System.Collections.Generic;
using System.IO;
using Xunit;
using ZMM.Helpers.Common;
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
            //create folder
            if (!Directory.Exists(TestAPIs.TestDir))
            {
                System.Console.WriteLine("Creating directory to get zip files from github");
                Directory.CreateDirectory(TestAPIs.TestDir);
            }
            else
            {
                string[] filePaths = Directory.GetFiles(TestAPIs.TestDir);
                foreach (string filePath in filePaths)
                    File.Delete(filePath);
            }
            System.Console.WriteLine("Getting zip file for positive scenario");
            TestAPIs.ProcessStart("https://github.com/nimeshgit/mlw-testdata/raw/master/DisDriver.zip");
            System.Console.WriteLine("Getting zip file name");
            string fileName = Path.GetFileName(Directory.GetFiles(TestAPIs.TestDir)[0]);
            System.Console.WriteLine("Checking zip file is sanitized");
            Assert.True(FilePathHelper.IsFileNameValid(fileName), "Zip file is sanitized.");
            System.Console.WriteLine("End Test : TestZipSanitizeWithPositiveInput");
        }
        #endregion

        #region ZipFileSanitize for positive test scenarios for Zip bomb file
        [Fact]
        public void TestZipSanitizeForZipBombPositiveInput()
        {
            System.Console.WriteLine("Start Test : TestZipSanitizeForZipBombPositiveInput");
            System.Console.WriteLine("Check if directory exists to get zip files from github");
            //create folder
            if (!Directory.Exists(TestAPIs.TestDir))
            {
                System.Console.WriteLine("Creating directory to get zip files from github");
                Directory.CreateDirectory(TestAPIs.TestDir);
            }
            else
            {
                string[] filePaths = Directory.GetFiles(TestAPIs.TestDir);
                foreach (string filePath in filePaths)
                    File.Delete(filePath);
            }
            System.Console.WriteLine("Getting zip file for positive scenario");
            TestAPIs.ProcessStart("https://github.com/nimeshgit/mlw-testdata/raw/master/5GB%20ZIP%20Bomb%20fIle.zip");
            System.Console.WriteLine("Getting zip file name");
            string fileName = Path.GetFileName(Directory.GetFiles(TestAPIs.TestDir)[0]);
            System.Console.WriteLine("Checking zip file is sanitized");
            Assert.True(FilePathHelper.IsFileNameValid(fileName), "Zip file is sanitized.");
            System.Console.WriteLine("End Test : TestZipSanitizeForZipBombPositiveInput");
        }
        #endregion

        #region ZipFileSanitize for negative test scenarios for Zip bomb file
        [Fact]
        public void TestZipSanitizeForZipBombNegativeInput()
        {
            System.Console.WriteLine("Start Test : TestZipSanitizeForZipBombNegativeInput");
            System.Console.WriteLine("Check if directory exists to get zip files from github");
            //create folder
            if (!Directory.Exists(TestAPIs.TestDir))
            {
                System.Console.WriteLine("Creating directory to get zip files from github");
                Directory.CreateDirectory(TestAPIs.TestDir);
            }
            else
            {
                string[] filePaths = Directory.GetFiles(TestAPIs.TestDir);
                foreach (string filePath in filePaths)
                    File.Delete(filePath);
            }
            System.Console.WriteLine("Getting zip file for positive scenario");
            TestAPIs.ProcessStart("https://github.com/nimeshgit/mlw-testdata/raw/master/13GB%20ZIP%20Bomb%20fIle.zip");
            System.Console.WriteLine("Getting zip file name");
            string fileName = Path.GetFileName(Directory.GetFiles(TestAPIs.TestDir)[0]);
            System.Console.WriteLine("Checking zip file is sanitized");
            Assert.True(FilePathHelper.IsFileNameValid(fileName), "Zip file is sanitized.");
            System.Console.WriteLine("End Test : TestZipSanitizeForZipBombNegativeInput");
        }
        #endregion

    }
}
