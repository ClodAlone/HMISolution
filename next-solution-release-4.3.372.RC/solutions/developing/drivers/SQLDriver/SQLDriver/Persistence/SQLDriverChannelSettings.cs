using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;
using System.Data.SqlClient;
using System.Data;

namespace SQLDriver
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class SQLDriverChannelSettings : ChannelSettings
    {
                #region Constructors

        public SQLDriverChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
            session.UpdateSchema(typeof(SQLDriverChannelSettings));
        }

        public SQLDriverChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            base.Timeout = 30000;
            //_SQLServer = "(local)\\SQLEXPRESS";
            //_SQLDatabase = string.Empty;
            //_SQLLogin = string.Empty;
            //_SQLPassword = string.Empty;
            _ConnectionString = string.Empty;
            _Provider = string.Empty;
            _BackupConnectionString = string.Empty;
            _BackupProvider = string.Empty;
            _ChannelSwitchHostTimeout = 0;
            _PublishingInterval = 0;
            _MaxConsecutiveWrite = 3;
        }

        //steve 080711
        public void CopyProperties(SQLDriverChannelSettings ch)
        {
            base.CopyProperties(ch);

            Provider = ch.Provider;
            ConnectionString = ch.ConnectionString;
            BackupProvider = ch.BackupProvider;
            BackupConnectionString = ch.BackupConnectionString;
            ChannelSwitchHostTimeout = ch.ChannelSwitchHostTimeout;
            PublishingInterval = ch.PublishingInterval;
            MaxConsecutiveWrite = ch.MaxConsecutiveWrite;
        }
        /////////////////////////////

        #region Properties

        /// <summary>   SQL Connection String. </summary>
        /// 
        /// 
        /// <summary>   SQL Connection String. </summary>
        private string _ConnectionString;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Connection String. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        [Size(SizeAttribute.Unlimited)]
        public string ConnectionString
        {
            get { return _ConnectionString; }
            set
            {
                SetPropertyValue("ConnectionString", ref _ConnectionString, value);
            }
        }
        /// <summary>   SQL Connection String. </summary>
        /// 
        /// 
        /// <summary>   SQL Connection String. </summary>
        private string _BackupConnectionString;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Backup Connection String. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        [Size(SizeAttribute.Unlimited)]
        public string BackupConnectionString
        {
            get { return _BackupConnectionString; }
            set
            {
                SetPropertyValue("BackupConnectionString", ref _BackupConnectionString, value);
            }
        }
        /// <summary>   SQL Connection String. </summary>
        /// 
        /// 
        /// <summary>   SQL Connection String. </summary>
        private string _Provider;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Provider. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        public string Provider
        {
            get { return _Provider; }
            set
            {
                SetPropertyValue("Provider", ref _Provider, value);
            }
        }

        /// <summary>   SQL Connection String. </summary>
        /// 
        /// 
        /// <summary>   SQL Connection String. </summary>
        private string _BackupProvider;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Backup Provider. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        public string BackupProvider
        {
            get { return _BackupProvider; }
            set
            {
                SetPropertyValue("BackupProvider", ref _BackupProvider, value);
            }
        }

        /// <summary>   SQL Connection String. </summary>
        /// 
        /// 
        /// <summary>   SQL Connection String. </summary>
        private int _ChannelSwitchHostTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the ChannelSwitchHostTimeout. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        public int ChannelSwitchHostTimeout
        {
            get { return _ChannelSwitchHostTimeout; }
            set
            {
                SetPropertyValue("ChannelSwitchHostTimeout", ref _ChannelSwitchHostTimeout, value);
            }
        }

        private int _PublishingInterval;

        public int PublishingInterval
        {
            get { return _PublishingInterval; }
            set { SetPropertyValue("PublishingInterval", ref _PublishingInterval, value); }
        }

        private int? _MaxConsecutiveWrite;
        public int? MaxConsecutiveWrite
        {
            get { return _MaxConsecutiveWrite; }
            set { SetPropertyValue("MaxConsecutiveWrite", ref _MaxConsecutiveWrite, value); }
        }
        #endregion

        const int defaultMaxConsecutiveWrite = 1;
        private void EnsureDefaultValues()
        {
            if (!_MaxConsecutiveWrite.HasValue)
                _MaxConsecutiveWrite = defaultMaxConsecutiveWrite;
        }
        public override void AfterConstruction()
        {
            base.AfterConstruction();

            EnsureDefaultValues();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            EnsureDefaultValues();
        }

        #region IDataErrorInfo Members

        //public static List<string> GetTables(string connectionString)
        //{
        //    using (SqlConnection connection = new SqlConnection(connectionString))
        //    {
        //        connection.Open();
        //        DataTable schema = connection.GetSchema("Tables");
        //        List<string> TableNames = new List<string>();
        //        foreach (DataRow row in schema.Rows)
        //        {
        //            TableNames.Add(row[2].ToString());
        //        }
        //        return TableNames;
        //    }
        //}


        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;
            switch (propertyName)
            {
                case "PublishingInterval":
                    if (PublishingInterval > int.MaxValue || PublishingInterval < 0)
                        return string.Format(Properties.Resources.PublishingIntervaOutOfRange, int.MaxValue);
                    break;
                case "MaxConsecutiveWrite":
                    if (MaxConsecutiveWrite > int.MaxValue || MaxConsecutiveWrite < 1)
                        return string.Format(Properties.Resources.MaxConsecutiveWriteOutOfRange, int.MaxValue);
                break;
            }

            return null;
        }
        #endregion

    }
}
