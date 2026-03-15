//-------------------------------------------------------------------------------------------------
// <copyright file="OleDbDataProvider.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.Reports.Server.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;  
    using System.Linq;
    using System.Data.OleDb;

    /// <summary>
    /// A data provider for supporting OleDb connection oriented data sources.
    /// </summary>
    internal class OleDbDataProvider 
        : IDataProvider
    {
        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="OleDbDataProvider"/> class.
        /// </summary>
        public OleDbDataProvider()
        {
        }

        #endregion

        #region IDataProvider Methods

        /// <summary>
        /// Gets schema informatation for the given connection string and query.
        /// </summary>
        /// <param name="query">Query string.</param>
        /// <param name="connection">Represents connection to the database.</param>
        /// <returns>Data table containing </returns>
        public DataTable GetSchema(string query, DbConnection connection)
        {
            using (OleDbCommand command = new OleDbCommand(query, (OleDbConnection)connection))
            {
                using (OleDbDataReader oleDbDataReader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Columns.Add(Syncfusion.Reports.Server.Utils.ReportingConstants.SchemaColumn, typeof(string));
                    while (oleDbDataReader.Read())
                    {
                        table.Rows.Add(new object[] { oleDbDataReader.GetString(0)});
                    }

                    return table;
                }
            }
        }

        /// <summary>
        /// Gets schema's columns informatation for the given connection string and query.
        /// </summary>
        /// <param name="query">Query string.</param>
        /// <param name="connection">Represents connection to the database.</param>
        /// <returns>Data table containing </returns>
        public DataTable GetSchemaColumn(string query, DbConnection connection)
        {
            using (OleDbCommand command = new OleDbCommand(query, (OleDbConnection)connection))
            {
                using (OleDbDataAdapter sqlDataAdapter = new OleDbDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    sqlDataAdapter.Fill(table);
                    return table;
                }
            }
        }

        /// <summary>
        /// Gets schema's tables informatation for the given connection string and query.
        /// </summary>
        /// <param name="query">Query string.</param>
        /// <param name="connection">Represents connection to the database.</param>
        /// <returns>Data table containing </returns>
        public DataTable GetSchemaTable(string query, DbConnection connection)
        {
            using (OleDbCommand command = new OleDbCommand(query, (OleDbConnection)connection))
            {
                using (OleDbDataReader sqlDataReader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Columns.Add(Syncfusion.Reports.Server.Utils.ReportingConstants.NameColumn, typeof(string));
                    table.Columns.Add(Syncfusion.Reports.Server.Utils.ReportingConstants.SchemaColumn, typeof(string));
                    while (sqlDataReader.Read())
                    {
                        table.Rows.Add(new object[] { sqlDataReader.GetString(0), sqlDataReader.GetString(1) });
                    }

                    return table;
                }
            }
        }

        /// <summary>
        /// Gets table informatation for the given connection string and query.
        /// </summary>
        /// <param name="query">Query string.</param>
        /// <param name="connection">Represents connection to the database.</param>
        /// <returns>Data table containing </returns>
        public DataTable GetTable(string query, DbConnection connection)
        {
            using (OleDbCommand command = new OleDbCommand(query, (OleDbConnection)connection))
            {
                command.CommandType = CommandType.Text;
                using (OleDbDataAdapter dAdapter = new OleDbDataAdapter(command))
                {
                    DataTable dtable = new DataTable();
                    DataSet dset = new DataSet();
                    dAdapter.Fill(dset);
                    dtable = dset.Tables[0];
                    return dtable;
                   
                }
            }
        }

        /// <summary>
        /// Gets Table informatation for the given connection string and query.
        /// </summary>
        /// <param name="connectionstring">connection string.</param>
        /// <param name="query">query string.</param>
        /// <returns>Data table containing </returns>
        public DataTable GetTable(string connectionstring, string query)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionstring))
            {
                try
                {
                    connection.Open();
                    DataTable dataTable = GetTable(query, connection);
                    return dataTable;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Gets Table informatation for the given connection string and query.
        /// </summary>
        /// <param name="connectionString">connection string.</param>
        /// <param name="query">Query string.</param>
        /// <param name="tableName">String for TableName.</param>
        /// <returns>Data table containing </returns>
        public DataTable GetTable(string connectionString, string query, string tableName)
        {
            DataTable dataTable = GetTable(connectionString, query);
            dataTable.TableName = tableName;
            return dataTable;
        }

        /// <summary>
        /// Gets Table informations
        /// </summary>
        /// <param name="dbConnection">Connection Represents to Database</param>
        /// <param name="command">Command Represents to Database</param>
        /// <returns></returns>
        public DataTable GetTable(DbConnection dbConnection, OleDbCommand command)
        {
            using (command.Connection = (OleDbConnection)dbConnection)
            {
                DataTable dtable = new DataTable();
                using (OleDbDataAdapter dAdapter = new OleDbDataAdapter(command))
                {
                    DataSet dset = new DataSet();
                    dAdapter.Fill(dset);
                    dtable = dset.Tables[0];
                }
                return dtable;
            }
        }

        #endregion

        #region Helper Method

        /// <summary>
        /// Gets Table informatation for the given connection string and query.
        /// </summary>
        /// <param name="connectionstring">connection string.</param>
        /// <param name="command">Command Represents to database.</param>
        /// <returns>Data table containing </returns>
        public DataTable GetTable(string connectionstring, OleDbCommand command, string tableName)
        {
            using (OleDbConnection connection = new OleDbConnection(connectionstring))
            {
                try
                {
                    connection.Open();
                    DataTable dataTable = GetTable(connection, command);
                    dataTable.TableName = tableName;
                    return dataTable;
                }
                catch (Exception ex)
                {
                    throw ex;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        #endregion       
    }
}
