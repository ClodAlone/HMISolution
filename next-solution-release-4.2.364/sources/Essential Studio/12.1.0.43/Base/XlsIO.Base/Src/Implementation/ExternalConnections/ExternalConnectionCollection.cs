#region Copyright Syncfusion Inc. 2001 - 2014
// Copyright Syncfusion Inc. 2001 - 2014. All rights reserved.
// Use of this code is subject to the terms of our license.
// A copy of the current license can be obtained at any time by e-mailing
// licensing@syncfusion.com. Any infringement will be prosecuted under
// applicable laws. 
#endregion
using System;
using Syncfusion.XlsIO.Interfaces;
using Syncfusion.XlsIO.Implementation.Collections;
using System.Collections.Generic;
using System.IO;

namespace Syncfusion.XlsIO.Implementation
{
    public class ExternalConnectionCollection : CollectionBaseEx<IConnection>
    , IConnections
    , ICloneParent
    , IList<IConnection>
    {
        private const string Default_Oledb_Provider = "Provider=Microsoft.ACE.OLEDB.12.0";
        public ExternalConnectionCollection(IApplication application, object parent)
            : base(application, parent)
        {

        }

        /// <summary>
        /// Connection added to workbook
        /// </summary>        
        public IConnection Add(string connectionName, string description, object connectionString, object commandText, ExcelCommandType commandType)
        {
            checkname(connectionName);
            ExternalConnection Connection = new ExternalConnection(Application, Parent);
            string conn_str = (string)connectionString;
            if (conn_str.StartsWith("OLEDB;"))
                CreateOledbConnection(Connection, conn_str, commandText, commandType);
            else if (conn_str.StartsWith("ODBC;"))
                CreateObdcConnection(Connection, connectionString, commandText, commandType);
                                            
            Connection.Name = connectionName;
            Connection.Description = description;
            Connection.SourceFile = FindDataSource(connectionString.ToString());
            Connection.ConncetionId =(uint) GetConnectionId();
            
            
            //Connection.
            base.Add(Connection);
            return Connection;
        }
        
        public IConnection Add(ExcelConnectionsType Type)
        {
            ExternalConnection connection = new ExternalConnection(Application, Parent);
            connection.DataBaseType = Type;
            if (Type == ExcelConnectionsType.ConnectionTypeODBC)
                connection.ODBCConnection = new ODBCConnection(connection);
            else if (Type == ExcelConnectionsType.ConnectionTypeOLEDB)
                connection.OLEDBConnection = new OLEDBConnection(connection);
            base.Add(connection);
            return connection;
        }       

        public IConnection AddFromFile(string Name,string FilePath)
        {
            string path = Path.GetExtension(FilePath);
            checkname(Name);
            if (path != ".xml")
                throw new ArgumentException("The file Format is wrong");
            ExternalConnection connection = new ExternalConnection(this.Application,this.Parent);
            connection.ConnectionURL = FilePath;
            connection.DataBaseType = ExcelConnectionsType.ConnectionTypeWEB;
            connection.ConncetionId = (uint)GetConnectionId();
            connection.Name = Path.GetFileNameWithoutExtension(FilePath);
            connection.IsXml = true;
            base.Add(connection);
            return connection;
        }

        /// <summary>
        /// Check the connection name
        /// </summary>
        /// <param name="name"></param>
        private void checkname(string name)
        {
            string Name;
            int count = this.Count;            
            for (int i = 0; i < count; i++)
            {
                WorkbookImpl book = this.Parent as WorkbookImpl;
                if (book.Connections[i].Name == name)
                    throw new ArgumentException("Connection Name Already Exist");
            }

        }
        /// <summary>
        /// Create OLEDB connection
        /// </summary>
        /// <param name="connection"></param>
        /// <param name="connectionSting"></param>
        /// <param name="commandText"></param>
        /// <param name="commandType"></param>
        private void CreateOledbConnection(ExternalConnection connection, object connectionSting, object commandText, ExcelCommandType commandType)
        {
            string conn_str = connectionSting.ToString();
            conn_str = conn_str.Substring(conn_str.IndexOf(";") + 1);
            OLEDBConnection Oledb = new OLEDBConnection(connection);
            connection.DataBaseType = ExcelConnectionsType.ConnectionTypeOLEDB;
            connection.DBConnectionString = conn_str;
            Oledb.ConnectionString = Checkconnection(conn_str);
            Oledb.CommandText = (string)commandText;
            Oledb.CommandType = commandType;
            Oledb.RefreshOnFileOpen = true;
            connection.OLEDBConnection = Oledb;
        }
        /// <summary>
        /// Create ODBC connection
        /// </summary>
        /// <param name="Connection"></param>
        /// <param name="ConnectionSting"></param>
        /// <param name="CommandText"></param>
        /// <param name="CommandType"></param>
        private void CreateObdcConnection(ExternalConnection Connection, object ConnectionSting, object CommandText, ExcelCommandType CommandType)
        {
            if (CommandType != ExcelCommandType.Sql)
                throw new ArgumentException("command type is not valid for ODBC connection");
            string conn_str = ConnectionSting.ToString();
            conn_str = conn_str.Substring(conn_str.IndexOf(";") + 1);
            ODBCConnection Odbc = new ODBCConnection(Connection);
            Connection.DataBaseType = ExcelConnectionsType.ConnectionTypeODBC;
            Odbc.ConnectionString = conn_str;
            Connection.DBConnectionString = conn_str;
            Odbc.CommandText = (string)CommandText;
            Odbc.CommandType = CommandType;
            Odbc.RefreshOnFileOpen = true;
            Connection.ODBCConnection = Odbc;
        }
        /// <summary>
        /// Find the Data source from the connection string
        /// </summary>
        /// <param name="connectionString"></param>
        /// <returns></returns>
        private string FindDataSource(string connectionString)
        {
            string tem_conn = connectionString.ToLower();
            if (tem_conn.Contains("data source"))
            {
                int startindex = tem_conn.IndexOf("data source") + 12;
                string conn = connectionString.Substring(startindex);
                if (conn.Contains(";"))
                conn = conn.Remove(conn.IndexOf(";") + 1);
                return conn;
            }

            return string.Empty;
        }
        /// <summary>
        /// Check the connection string
        /// </summary>
        /// <param name="connection"></param>
        /// <returns></returns>
        internal static string Checkconnection(string connection)
        {
            string change = connection.ToLower();
            if (change.Contains("provider"))
            {
                int start = change.IndexOf("provider");
                int end = change.IndexOf(";", start);
                end.ToString();
                string provider = connection.Substring(start, end - start);
                change = change.Substring(start, end - start);
                if (provider != null && provider != "" && change.Contains("jet"))
                {
                    connection = connection.Replace(provider, Default_Oledb_Provider);
                }
            }
            return connection;
        }
        /// <summary>
        /// Find the connection ID
        /// </summary>
        /// <returns></returns>
        internal int GetConnectionId()
        {
            int Id = (((this.Parent as WorkbookImpl).Connections.Count + 1) + (this.Parent as WorkbookImpl).DeletedConnections.Count);
            for (; ; Id++)
            {
                if ((Checkconnections(this, Id)) || (Checkconnections((this.Parent as WorkbookImpl).DeletedConnections, Id)))
                    continue;
                else
                    break;
            }
            return Id;
        }
        private bool Checkconnections(IConnections connections,int Id)
        {
            for (int i = 0; i < connections.Count; i++)
            {
                if (connections[i].ConncetionId == (uint)Id)
                    return true;
            }
                return false;
        }
        public void Dispose()
        {
            int count = this.Count;
            for (int i = count - 1; i > -1; i--)
            {
                ExternalConnection conn = (ExternalConnection)this[i];
                conn.Disposeall();
                this[i] = null;
                this.RemoveAt(i);
            }
        }
    }
}
