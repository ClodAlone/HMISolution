using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace DataLoggerModel.Helpers
{
    [DataContract]
    public class DataLoggerInfo
    {
        #region Constructors
        public DataLoggerInfo()
        { }

        public DataLoggerInfo(DataLoggerSettings setting)
        {
            var helper = new DataLoggerSettingsHelper(setting, null);

            IsDataProtected = setting.EnableDataProtection;
            TableSchema = helper.CreateDataTable();
            DataProvider = setting.ConnectionSettings.DataProvider;
            Connection = setting.ConnectionSettings.Connection;
            TableName = helper.TableName;
            UtcTimeColumnName = helper.UtcTimeColumnName;
            MillisecondsColumnName = helper.MillisecondsColumnName;
            AutoIncrementColumnName = helper.AutoIncrementColumnName;
            RedundancyColumnName = helper.RedundancyColumnName;
            MaxAge = setting.MaxAge.Value;
        }
        #endregion

        #region Data Members
        [DataMember]
        public bool IsDataProtected;
        [DataMember]
        public System.Data.DataTable TableSchema;
        [DataMember]
        public String DataProvider;
        [DataMember]
        public String Connection;
        [DataMember]
        public String TableName;
        [DataMember]
        public String UtcTimeColumnName;
        [DataMember]
        public String MillisecondsColumnName;
        [DataMember]
        public String AutoIncrementColumnName;
        [DataMember]
        public String RedundancyColumnName;
        [DataMember]
        public TimeSpan MaxAge;
        #endregion

        #region Methods
        public bool IsValid()
        {
            return TableSchema != null &&
                !String.IsNullOrEmpty(DataProvider) &&
                !String.IsNullOrEmpty(Connection) &&
                !String.IsNullOrEmpty(TableName) &&
                !String.IsNullOrEmpty(UtcTimeColumnName) &&
                !String.IsNullOrEmpty(MillisecondsColumnName) &&
                !String.IsNullOrEmpty(AutoIncrementColumnName) && 
                !String.IsNullOrEmpty(RedundancyColumnName);
        }
        #endregion
    }
}
