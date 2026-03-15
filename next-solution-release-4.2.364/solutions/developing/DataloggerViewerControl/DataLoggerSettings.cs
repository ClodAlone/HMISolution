using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataloggerViewerControl
{
    internal class DataLoggerSettings
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public string UtcTimeColumnName { get; set; }
        public string LocalTimeColumnName { get; set; }
        public string MillisecondsColumnName { get; set; }
        public string UserColumnName { get; set; }
        public string ReasonColumnName { get; set; }
        public DataLoggerColumns Columns { get; set; }
    }

    internal class DataLoggerColumns : List<DataLoggerColumn>
    {
        public DataLoggerColumns() 
        { 
        }
    }

    internal class DataLoggerColumn
    {
        public string Name { get; set; }
    }
    
}
