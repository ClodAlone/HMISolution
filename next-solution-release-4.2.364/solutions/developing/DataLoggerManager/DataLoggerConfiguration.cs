using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoggerManager
{
    public class DataLoggerConfiguration
    {
        public String xpoDataConnectionString;
        public String xpoSafeDataConnectionString;
        public String projectRootFolder;
        public int redundancyServerId;
        public int redundancyHistoryThreadPool;
        public int redundancyMaxSyncRecords;
        public int MaxRestoreProcess;
        public long MaxTotalSafelyFilesSize;
    }
}
