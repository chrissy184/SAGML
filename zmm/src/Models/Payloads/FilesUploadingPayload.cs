using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ZMM.Models.ResponseMessages;
using ZMM.Models.Storage;

namespace ZMM.Models.Payloads
{
    public static class FilesUploadingPayload
    {
        #region Create
        public static FilesInProgress Create(FilesInProgress newRecord)
        {
            GlobalStorage.FilesInProgressStorage.TryAdd(newRecord.Id, newRecord);
            return newRecord;
        }

        #endregion

        #region Read     
        public static List<FilesInProgress> Get(string module)
        {
            List<FilesInProgress> wip = new List<FilesInProgress>();
            if (GlobalStorage.FilesInProgressStorage != null)
            {
                foreach (var item in GlobalStorage.FilesInProgressStorage)
                {
                    if (item.Value.Module == module) wip.Add(item.Value);
                }
            }

            return wip;
        }

        #endregion

        #region Remove completed from GlobalStorage
        public static bool RemoveCompleted(string id)
        {
            //id is the filename
            bool result = false;            
            FilesInProgress _data = new FilesInProgress();
            result = GlobalStorage.FilesInProgressStorage.TryRemove(id, out _data);
            return result;
        }
        #endregion
    }
}