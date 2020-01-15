using System;
using System.Collections.Generic;
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

        
        #endregion
    }
}
