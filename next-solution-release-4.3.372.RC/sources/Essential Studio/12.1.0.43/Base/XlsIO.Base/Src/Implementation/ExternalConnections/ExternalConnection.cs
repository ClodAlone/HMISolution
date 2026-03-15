#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.XlsIO.Interfaces;
using System.IO;

namespace Syncfusion.XlsIO.Implementation
{
    public class ExternalConnection : CommonObject
    , IConnection
    {
        public ExternalConnection(IApplication application, object parent)
            : base(application, parent)
        {
            initialize();
        }
        private void initialize()
        {
        }

        #region Members
        /// <summary>
        /// Represent the connection name
        /// </summary>
        private string m_name;
        /// <summary>
        /// Represent the connection description
        /// </summary>
        private string m_description; 
        /// <summary>
        /// Represent the connection id
        /// </summary>     
        private uint m_connectionId;
        /// <summary>
        /// It's define the connection file
        /// </summary>
        private string m_connectionFile;
        /// <summary>
        /// Represent the source file
        /// </summary>
        private string m_sourceFile =string.Empty;
        /// <summary>
        /// Represent the data base type
        /// </summary>
        private ExcelConnectionsType m_dataBaseType;    
        /// <summary>
        /// Represent the oledb connection
        /// </summary>
        private OLEDBConnection m_oledbConnection;
        /// <summary>
        /// Represent the odbc connection
        /// </summary>
        private ODBCConnection m_odbcConnection;
        /// <summary>
        /// It's define the refreshed version
        /// </summary>
        private uint m_refershedVersion;
        /// <summary>
        /// Represent connection is deleted or not
        /// </summary>
        private bool m_deleted;
        /// <summary>
        /// Represenet the background query
        /// </summary>
        private bool m_backgroundQuery = true;
        /// <summary>
        /// It's define the connection string
        /// </summary>
        private object m_connectionstring;
        /// <summary>
        /// It's define whether the connection is used or not
        /// </summary>
        private bool m_isexist;
        /// <summary>
        /// Data base connection string
        /// </summary>
        private string m_dbConnectionString;
        /// <summary>
        /// It's define the query table range
        /// </summary>
        private IRange m_range;
        /// <summary>
        /// Represent the ODBC password
        /// </summary>
        private string m_password;
        /// <summary>
        /// connection password events
        /// </summary>
        public event ConnectionPasswordEventHandler OnConnectionPassword;
        //Web Properties
        private bool m_isXml;
        private string m_connectionURL;
        /// <summary>
        /// Olap maintain as Stream
        /// </summary>
        private Stream m_olapProperty;
        /// <summary>
        /// Ext_Lst maintain as Stream
        /// </summary>
        private Stream m_extLst;
        /// <summary>
        /// Represent textPr as stream
        /// </summary>
        internal Stream m_textPr;
        #endregion

        #region Properties
        /// <summary>
        /// Represent the Description
        /// </summary>
        public string Description
        {
            get
            {
                return m_description;
            }
            set
            {
                m_description = value;
            }
        }                   
        /// <summary>
        /// Represent the connection id
        /// </summary>
        public uint ConncetionId
        {
            get
            {
                return m_connectionId;
            }
            set
            {
                m_connectionId = value;
            }
        }
        /// <summary>
        /// Represent the connection file
        /// </summary>
        public string ConnectionFile
        {
            get
            {
                return m_connectionFile;
            }
            set
            {
                m_connectionFile = value;
            }
        }
        /// <summary>
        /// Represent the source file
        /// </summary>
        public string SourceFile
        {
            get
            {
                return m_sourceFile;
            }
            set
            {
                m_sourceFile = value;
            }
        }
        /// <summary>
        /// Represent the Database Type
        /// </summary>
        public ExcelConnectionsType DataBaseType
        {
            get
            {
                return m_dataBaseType;
            }
            set
            {
                m_dataBaseType = value;
            }
        }
        /// <summary>
        /// Represent the connection name
        /// </summary>
        public string Name
        {
            get
            {
                return m_name;
            }
            set
            {
                m_name = value;
            }
        }
        /// <summary>
        /// Represent the Oledb Connection
        /// </summary>
        public OLEDBConnection OLEDBConnection
        {
            get
            {
                return m_oledbConnection;
            }
            set
            {
                m_oledbConnection = value;
            }
        }
        public ODBCConnection ODBCConnection
        {
            get
            {
                return m_odbcConnection;
            }
            set
            {
                m_odbcConnection = value;
            }
        }
        /// <summary>
        /// Delete the connection
        /// </summary>
        
        /// <summary>
        /// Represent the refershed verion
        /// </summary>
        public uint RefershedVersion
        {
            get
            {
                return m_refershedVersion;
            }
            set
            {
                m_refershedVersion = value;
            }
        }        
        /// <summary>
        /// Represent the connection deleted or not
        /// </summary>
        public Boolean Deleted
        {
            get
            {
                return m_deleted;
            }
            set
            {
                m_deleted = value;
            }
        }
        /// <summary>
        /// The Query is asynchronus with DB
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
        /// Delete the connection
        /// </summary>
        public void Delete()
        {
            if (OLEDBConnection != null)
            {
                OLEDBConnection.ConnectionString = (object)string.Empty;
                OLEDBConnection.CommandText = (object)string.Empty;
            }
            else if (ODBCConnection != null)
            {
                ODBCConnection.ConnectionString = (object)string.Empty;
                ODBCConnection.CommandText = (object)string.Empty;
            }
            m_deleted = true;
            //if (m_deletedConnection == null)
            //    m_deletedConnection = new ExternalConnectionCollection(this.Application, this.Parent);
            WorkbookImpl book = this.Range.Worksheet.Workbook as WorkbookImpl;
            book.DeleteConnection(this); 
            
        }
        internal object ConnectionString
        {
            set
            {
                if (OLEDBConnection != null)
                    OLEDBConnection.ConnectionString = value;
                else if (ODBCConnection != null)
                    ODBCConnection.ConnectionString = value;
            }
            get
            {
                if (OLEDBConnection != null)
                    return OLEDBConnection.ConnectionString;
                else if (ODBCConnection != null)
                    return ODBCConnection.ConnectionString;
                else
                    return null;
            }
        }
        /// <summary>
        /// It's define the connection is exist for query table or not
        /// </summary>
        public bool IsExist
        {
            get
            {
                return m_isexist;
            }
            set
            {
                m_isexist = value;
            }
        }
        /// <summary>
        /// Database connection string
        /// </summary>
        public string DBConnectionString
        {
            get
            {
                return m_dbConnectionString;
            }
            set
            {
                m_dbConnectionString=value;
            }
        }
        public IRange Range
        {
            get
            {
                if (m_range == null)                
                    m_range = new RangeImpl(this.Application, this.Parent);         

                return m_range;
            }
            set
            {
                m_range = value;
            }
        }
        internal bool IsXml
        {
            get
            {
                return m_isXml;
            }
            set
            {
                m_isXml = value;
            }
        }
        internal string ConnectionURL
        {
            get
            {
                return m_connectionURL;
            }
            set
            {
                m_connectionURL = value;
            }
        }
        public string password
        {
            get
            {
                return m_password;
            }
            set
            {
                m_password = value;
            }
        }
        /// <summary>
        /// Olap Stream
        /// </summary>
        public Stream OlapProperty
        {
            get
            {
                return m_olapProperty;
            }
            set
            {
                m_olapProperty = value;
            }
        }
        /// <summary>
        /// Ext_Lst Stream
        /// </summary>
        public Stream ExtLstProperty
        {
            get
            {
                return m_extLst;
            }
            set
            {
                m_extLst = value;
            }
        }
        #endregion

        #region Methods
        public void RaiseEvent(object sender, ConnectionPassword args)
        {
            if (OnConnectionPassword != null)
            {
                //this.m_password = args.Connectionpassword;

                OnConnectionPassword(sender, args);
            }
        }
        internal ExternalConnection Clone(WorkbookImpl book, string ConnectionName)
        {
            ExternalConnection Connection = (ExternalConnection)MemberwiseClone();
            ExternalConnectionCollection Connections;
            if (Connection.Deleted)
                Connections = book.DeletedConnections as ExternalConnectionCollection;
            else
            Connections = book.Connections as ExternalConnectionCollection;
            Connection.ConncetionId = (uint)Connections.GetConnectionId();
            Connection.Name = ConnectionName;
            Connections.Add(Connection);
            return Connection;
        }
        public void Disposeall()
        {
            this.ODBCConnection = null;
            this.OLEDBConnection = null;
            this.ConnectionString = null;
            this.Dispose();
        }
        #endregion

        ~ExternalConnection()
        {
        }
        
    }
}
