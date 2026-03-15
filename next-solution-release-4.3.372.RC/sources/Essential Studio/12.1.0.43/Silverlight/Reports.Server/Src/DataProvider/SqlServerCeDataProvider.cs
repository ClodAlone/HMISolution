//-------------------------------------------------------------------------------------------------
// <copyright file="SqlDataProvider.cs" company="syncfusion">
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
    using System.Data.SqlClient;
    using System.Data.SqlServerCe;
    using System.Linq;

    /// <summary>
    /// A SqlServerCe data provider for SQLServerCe datasource.
    /// </summary>
    internal class SqlServerCeDataProvider 
        : IDataProvider
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlServerCeDataProvider"/> class.
        /// </summary>
        public SqlServerCeDataProvider()
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
            using (SqlCeCommand command = new SqlCeCommand(query, (SqlCeConnection)connection))
            {
                using (SqlCeDataReader sqlDataReader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Columns.Add(Syncfusion.Reports.Server.Utils.ReportingConstants.SchemaColumn, typeof(string));
                    while (sqlDataReader.Read())
                    {
                        table.Rows.Add(new object[] { sqlDataReader.GetSqlString(0) });
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
            using (SqlCeCommand command = new SqlCeCommand(query, (SqlCeConnection)connection))
            {
                using (SqlCeDataAdapter sqlDataAdapter = new SqlCeDataAdapter(command))
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
            using (SqlCeCommand command = new SqlCeCommand(query, (SqlCeConnection)connection))
            {
                using (SqlCeDataReader sqlDataReader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Columns.Add(Syncfusion.Reports.Server.Utils.ReportingConstants.NameColumn, typeof(string));
                    table.Columns.Add(Syncfusion.Reports.Server.Utils.ReportingConstants.SchemaColumn, typeof(string));
                    while (sqlDataReader.Read())
                    {
                        table.Rows.Add(new object[] { sqlDataReader.GetSqlString(0), sqlDataReader.GetSqlString(1) });
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
            using (SqlCeCommand command = new SqlCeCommand(query, (SqlCeConnection)connection))
            {
                using (SqlCeDataReader sqlDataReader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(sqlDataReader);
                    return table;
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
            using (SqlCeConnection connection = new SqlCeConnection(connectionstring))
            {
                try
                {
                    connection.Open();
                    DataTable dataTable = GetTable(query, connection);   // SqlDataProvider.GetTable(query, connection);
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
        public DataTable GetTable(DbConnection dbConnection, SqlCeCommand command)
        {
            using (command.Connection = (SqlCeConnection)dbConnection)
            {
                DataTable dtable = new DataTable();
                using (SqlCeDataAdapter dAdapter = new SqlCeDataAdapter(command))
                {
                    DataSet dset = new DataSet();
                    dAdapter.Fill(dset);
                    dtable = dset.Tables[0];
                }
                return dtable;
            }
        }

        #endregion

        #region Helper Mehod

        /// <summary>
        /// Gets Table informatation for the given connection string and query.
        /// </summary>
        /// <param name="connectionstring">connection string.</param>
        /// <param name="command">Command Represents to database.</param>
        /// <returns>Data table containing </returns>
        public DataTable GetTable(string connectionstring, SqlCeCommand command, string tableName)
        {
            using (SqlCeConnection connection = new SqlCeConnection(connectionstring))
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
