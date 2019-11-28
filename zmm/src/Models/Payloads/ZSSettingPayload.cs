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
        #region Create
        public static ZSSettingResponse Create(ZSSettingResponse newRecord)
        {
            GlobalStorage.ZSSettingStorage.TryAdd(newRecord.ZmodId, newRecord);
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
            var sortDesc = _settings.OrderBy(d => d.name);
            return sortDesc.ToList();
        }
        #endregion

    }
}