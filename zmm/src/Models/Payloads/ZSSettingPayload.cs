using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZMM.Models.ResponseMessages;
using ZMM.Models.Storage;

namespace ZMM.Models.Payloads
{
    public static class ZSSettingPayload
    {
        #region CreateOrUpdate
        public static ZSSettingResponse CreateOrUpdate(ZSSettingResponse newRecord)
        {
            var existingSettings = GetSettingsByUser(newRecord.ZmodId);
            ZSSettingResponse response = new ZSSettingResponse();
            if(existingSettings.Count > 0)
            {
                //update
                bool result = GlobalStorage.ZSSettingStorage.TryRemove(newRecord.ZmodId, out response);
                if (result)  GlobalStorage.ZSSettingStorage.TryAdd(newRecord.ZmodId, newRecord);
            }
            else
            {
                //add new
                GlobalStorage.ZSSettingStorage.TryAdd(newRecord.ZmodId, newRecord);  
            }
                      
            return newRecord;
        }
        #endregion

        #region GetSettingsByUser
        public static List<ZSSettingResponse> GetSettingsByUser(string zmodId)
        {
            List<ZSSettingResponse> _settings = new List<ZSSettingResponse>();
            if (GlobalStorage.ZSSettingStorage != null)
            {
                foreach (var item in GlobalStorage.ZSSettingStorage)
                {
                    if (item.Key == zmodId)
                    {
                        _settings.Add(item.Value);
                    }
                }
            }
            
            return _settings;
        }
        #endregion

    }
}