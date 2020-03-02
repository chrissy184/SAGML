using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ZMM.Helpers.ZMMDirectory;
using ZMM.Models.ResponseMessages;
using ZMM.Models.Payloads;

namespace ZMM.Helpers.Common
{
    public static class XSSBlackList
    {
       static string[]  xssBlackList = { "javascript","script","<",">" ,"alert"};
        public static bool CheckString(string strData)
        {
            if (xssBlackList.ToList().Any(strData.ToLower().Contains))
                return true;
            else
                return false;
        }
    }
}
