using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataLoggerColumnListControl
{
    public class DataLoggerSettings
    {
        public string Name { get; set; }
        public string TableName { get; set; }
        public string UtcTimeColumnName { get; set; }
        public string SourceTimeStampColumnName { get; set; }
        public string ConnectionString { get; set; }
        public DataLoggerColumns Columns { get; set; }
    }

    public class DataLoggerColumns : List<DataLoggerColumn>
    {
        public DataLoggerColumns()
        {
        }
    }

    public class DataLoggerColumn
    {
        public string Name { get; set; }
        public string SourceTimeStampColumnName { get; set; }
        public bool AddSourceTimeStampColumn { get; set; }
        public string ColumnTagName { get; set; }
        public string ColumnTagGuid { get; set; }
    }

}
