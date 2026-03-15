using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;

namespace DataLoggerManager
{
    /// <summary>
    /// Object to store a single value to add to table's column.
    /// </summary>
    public class DataColumEntity : ICloneable
    {
        public NodeId nodeId;
        public string columnName;
        public DataColumnValue columnValue;
        public string userName;
        public string stringValue;

        public bool skipDataChange;
        public bool statusCodeColumnEnabled;
        public bool sourceTimeStampColumnEnabled;
        public bool serverTimeStampColumnEnabled;
        public bool userColumnEnabled;
        public bool stringValueColumnEnabled;
        public string expression;
   
        #region ICloneable
        public object Clone()
        {
            var ret = new DataColumEntity();
            ret.nodeId = nodeId;
            ret.columnName = columnName;
            ret.userName = userName;
            ret.stringValue = stringValue;
            ret.skipDataChange = skipDataChange;
            ret.statusCodeColumnEnabled = statusCodeColumnEnabled;
            ret.sourceTimeStampColumnEnabled = sourceTimeStampColumnEnabled;
            ret.serverTimeStampColumnEnabled = serverTimeStampColumnEnabled;
            ret.userColumnEnabled = userColumnEnabled;
            ret.stringValueColumnEnabled = stringValueColumnEnabled;
            ret.expression = expression;
            ret.columnValue = columnValue.Clone() as DataColumnValue;

            return ret;
        }
        #endregion

    }
}
