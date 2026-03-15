using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoggerManager
{
    public class FlushedDataSafelyEventArgs : EventArgs
    {
        public String DataLoggerName { get; set; }
        public String FilePath { get; set; }
        public int RecordsCounter { get; set; }
        public String ErrorMessage { get; set; }
    }
}
