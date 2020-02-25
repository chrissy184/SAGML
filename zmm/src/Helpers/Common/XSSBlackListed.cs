using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using ZMM.Helpers.ZMMDirectory;
using ZMM.Models.ResponseMessages;
using ZMM.Models.Payloads;

namespace ZMM.Helpers.Common
{
    public static class XSSBlackListed
    {
       static string[]  xssBlackListed = { "javascript","script","<",">" ,"alert"};
        public static bool CheckString(string strData)
        {
            if (xssBlackListed.ToList().Any(strData.Contains))
                return true;
            else
                return false;
        }
    }
}
