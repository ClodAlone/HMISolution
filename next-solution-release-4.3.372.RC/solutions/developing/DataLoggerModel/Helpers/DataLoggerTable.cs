using DataReader;
using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace DataLoggerModel.Helpers
{
    [DataContract(Name = "DataLoggerTable", Namespace = UFInterfaces.Constants.Namespaces.UriProgea)]
    public class DataLoggerTable
    {
        #region Declarations
        [DataMember]
        DataTable dt;
        [DataMember]
        DataReaderModel connectionSettings;
        [DataMember]
        bool isDataProtectionEnabled;
        [DataMember]
        string utcTimeColumnType;
        [DataMember]
        string localTimeColumnType;
        [DataMember]
        string millisecondsColumnType;
        [DataMember]
        string userColumnType;
        [DataMember]
        string reasonColumnType;
        [DataMember]
        string name;
        [DataMember]
        string tableName;
        [DataMember]
        string utcTimeColumnName;
        [DataMember]
        string localTimeColumnName;
        [DataMember]
        string millisecondsColumnName;
        [DataMember]
        string userColumnName;
        [DataMember]
        string reasonColumnName;
        [DataMember]
        string redundancyColumnName;
        [DataMember]
        string autoIncrementColumnName;
        [DataMember]
        List<string> statusCodeColumnNamesList;
        #endregion

        #region Properties
        [DataMember]
        public DataTable TableStructure
        {
            get
            {
                return dt;
            }
            set { }
        }

        [DataMember]
        public DataReaderModel ConnectionSettings
        {
            get
            {
                return connectionSettings;
            }
            set { }
        }

        [DataMember]
        public bool IsDataProtectionEnabled
        {
            get
            {
                return isDataProtectionEnabled;
            }
            set { }
        }

        [DataMember]
        public string Name
        {
            get
            {
                return name;
            }
            set { }
        }

        [DataMember]
        public string TableName
        {
            get
            {
                return tableName;
            }
            set { }
        }

        [DataMember]
        public string UtcTimeColumnName {
            get
            {
                return utcTimeColumnName;
            }
            set { }
        }

        [DataMember]
        public string UtcTimeColumnType
        {
            get
            {
                return utcTimeColumnType;
            }
            set { }
        }

        [DataMember]
        public string LocalTimeColumnName
        {
            get
            {
                return localTimeColumnName;
            }
            set { }
        }

        [DataMember]
        public string LocalTimeColumnType
        {
            get
            {
                return localTimeColumnType;
            }
            set { }
        }

        [DataMember]
        public string MillisecondsColumnName
        {
            get
            {
                return millisecondsColumnName;
            }
            set { }
        }

        [DataMember]
        public string MillisecondsColumnType
        {
            get
            {
                return millisecondsColumnType;
            }
            set { }
        }

        [DataMember]
        public string UserColumnName
        {
            get
            {
                return userColumnName;
            }
            set { }
        }

        [DataMember]
        public string UserColumnType
        {
            get
            {
                return userColumnType;
            }
            set { }
        }

        [DataMember]
        public string ReasonColumnName
        {
            get
            {
                return reasonColumnName;
            }
            set { }
        }

        [DataMember]
        public string ReasonColumnType
        {
            get
            {
                return reasonColumnType;
            }
            set { }
        }

        [DataMember]
        public string RedundancyColumnName
        {
            get
            {
                return redundancyColumnName;
            }
            set { }
        }

        [DataMember]
        public string AutoIncrementColumnName
        {
            get
            {
                return autoIncrementColumnName;
            }
            set { }
        }

        [DataMember]
        public List<string> StatusCodeColumnNamesList
        {
            get
            {
                return statusCodeColumnNamesList != null ? statusCodeColumnNamesList : new List<string>();
            }
            set { }
        }
        #endregion

        #region Constructors
        public DataLoggerTable()
        { }

        public DataLoggerTable(DataLoggerSettings setting, string projectRoot)
        {
            name = setting.Name;
            var helper = new DataLoggerSettingsHelper(setting, projectRoot);

            dt = helper.CreateDataTable(skipDuplicatedColumns: true);
            tableName = helper.TableName;
            connectionSettings = setting.ConnectionSettings;
            isDataProtectionEnabled = setting.EnableDataProtection;
            utcTimeColumnName = helper.UtcTimeColumnName;
            utcTimeColumnType = dt.Columns[dt.Columns[utcTimeColumnName].Ordinal].DataType.ToString();
            localTimeColumnName = helper.LocalTimeColumnName;
            localTimeColumnType = dt.Columns[dt.Columns[localTimeColumnName].Ordinal].DataType.ToString();
            millisecondsColumnName = helper.MillisecondsColumnName;
            millisecondsColumnType = dt.Columns[dt.Columns[millisecondsColumnName].Ordinal].DataType.ToString();
            userColumnName = helper.UserColumnName;
            userColumnType = dt.Columns[dt.Columns[userColumnName].Ordinal].DataType.ToString();
            reasonColumnName = helper.ReasonColumnName;
            reasonColumnType = dt.Columns[dt.Columns[reasonColumnName].Ordinal].DataType.ToString();
            redundancyColumnName = helper.RedundancyColumnName;
            autoIncrementColumnName = helper.AutoIncrementColumnName;
            statusCodeColumnNamesList = helper.StatusCodeColumnNamesList;
        }
        #endregion
    }
}
