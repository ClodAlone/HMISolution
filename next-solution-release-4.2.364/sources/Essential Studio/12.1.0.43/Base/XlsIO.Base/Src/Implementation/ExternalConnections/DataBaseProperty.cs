#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace Syncfusion.XlsIO.Implementation
{
    public class DataBaseProperty
    {
        #region Members
        /// <summary>
        /// Represent the source data file
        /// </summary>
        private string m_sourceDataFile;
        /// <summary>
        /// The Query Table is automatically updated when open the workbook
        /// </summary>
        private bool m_refreshOnFileOpen;
        /// <summary>
        /// Define connection command type
        /// </summary>
        private ExcelCommandType m_commandType;
        /// <summary>
        /// Define the connection commandtext 
        /// </summary>
        private object m_commandText;
        /// <summary>
        /// It's define th connection string
        /// </summary>
        private object m_connectionString;
        /// <summary>
        /// It's desine the background query process
        /// </summary>
        private bool m_backgroundQuery = true;
        /// <summary>
        /// Represent the number of minutes between refreshes
        /// </summary>
        private int m_refreshPeriod = -1;
        /// <summary>
        /// Save the password for implement the odbc connection
        /// </summary>
        private bool m_savePassword;
        /// <summary>
        /// Represent the connection file
        /// </summary>
        private bool m_alwaysUseConnectionFile;
        /// <summary>
        /// Represent the enable refresh
        /// </summary>
        private bool m_enableRefresh=true;
        /// <summary>
        /// It's defind the server credential 
        /// </summary>
        private ExcelCredentialsMethod m_serverCredentialsMethod = ExcelCredentialsMethod.integrated;
#endregion 

        #region Properties
        /// <summary>
        /// Represent the source Data File
        /// </summary>
        public string SourceDataFile
        {
            get
            {
                FindDataSource();
                return m_sourceDataFile;
            }
        }
        /// <summary>
        /// The Query Table is automatically updated when open the workbook
        /// </summary>
        public bool RefreshOnFileOpen
        {
            get
            {
                return m_refreshOnFileOpen;
            }
            set
            {
                m_refreshOnFileOpen = value;
            }
        }
        /// <summary>
        /// Command Type for specified connection
        /// </summary>
        public ExcelCommandType CommandType
        {
            get
            {
                return m_commandType;
            }
            set
            {
                m_commandType = value;
            }
        }
        /// <summary>
        /// Represent the command string for the specified data source
        /// </summary>
        public object CommandText
        {
            get
            {
                return m_commandText;
            }
            set
            {
                m_commandText = value;
            }
        }
        /// <summary>
        /// Define connection information
        /// </summary>
        public virtual object ConnectionString
        {
            get
            {
                return m_connectionString;
            }
            set
            {
                    m_connectionString = value;
            }
        }
        /// <summary>
        /// Query table performed Asynchronously
        /// </summary>
        public bool BackgroundQuery
        {
            get
            {
                return m_backgroundQuery;
            }
            set
            {
                m_backgroundQuery = value;
            }
        }
        /// <summary>
        /// Represent the number of minutes between refreshes
        /// </summary>
        public int RefreshPeriod
        {
            get
            {
                return m_refreshPeriod;
            }
            set
            {
                m_refreshPeriod = value;
            }
        }
        /// <summary>
        /// Password information saved for ODBC connection
        /// </summary>
        public bool SavePassword
        {
            get
            {
               return m_savePassword;
            }
            set
            {
                m_savePassword = value;
            }
        }
        /// <summary>
        /// Connection file is always used to establish the connection to the DataSource
        /// </summary>
        public bool AlwaysUseConnectionFile
        {
            get
            {
                return m_alwaysUseConnectionFile;
            }
            set
            {
                m_alwaysUseConnectionFile = value;
            }
        }
        /// <summary>
        /// Connection can be refreshed by the user.
        /// </summary>
        public bool EnableRefresh
        {
            get
            {
                return m_enableRefresh;
            }
            set
            {
                m_enableRefresh = value;
            }
        }
        /// <summary>
        /// Represent the server credential.
        /// </summary>
        public ExcelCredentialsMethod ServerCredentialsMethod
        {
            get
            {
                return m_serverCredentialsMethod;
            }
            set
            {
                m_serverCredentialsMethod = value;
            }
        }
        #endregion

        #region Methods
        internal void FindDataSource()
        {
            string connectionstring = (string)ConnectionString;
            int startindex = connectionstring.IndexOf("Data Source=");
            m_sourceDataFile = connectionstring.Substring(startindex + 12);
            if (m_sourceDataFile.IndexOf(";") != -1)
                m_sourceDataFile = m_sourceDataFile.Remove(m_sourceDataFile.IndexOf(";"));           
        }
        #endregion
    }
}
