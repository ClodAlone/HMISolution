//-------------------------------------------------------------------------------------------------
// <copyright file="SqlDataProvider.cs" company="syncfusion">
//  Copyright (c) Syncfusion Inc. 2001 - 2014. All rights reserved.
//  Use of this code is subject to the terms of our license.
//  A copy of the current license can be obtained at any time by e-mailing
//  licensing@syncfusion.com. Re-distribution in any form is strictly
//  prohibited. Any infringement will be prosecuted under applicable laws. 
// </copyright>
//-------------------------------------------------------------------------------------------------

namespace Syncfusion.RDL.Data
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using Syncfusion.RDL.Internal;

    /// <summary>
    /// A Sql data provider for accessing the SQLServer Datasource.
    /// </summary>
    internal class SqlDataProvider
        : IDataProvider
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDataProvider"/> class.
        /// </summary>
        public SqlDataProvider()
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
            using (SqlCommand command = new SqlCommand(query, (SqlConnection)connection))
            {
                using (SqlDataReader sqlDataReader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Columns.Add(ReportingConstants.SchemaColumn, typeof(string));
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
            try
            {
                using (SqlCommand command = new SqlCommand(query, (SqlConnection)connection))
                {
                    using (SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(command))
                    {
                        DataSet ds = new DataSet();
                        sqlDataAdapter.Fill(ds);
                        return ds.Tables[0];
                    }
                }
            }
            catch (SqlException excep)
            {
                throw excep;
            }
            catch (OutOfMemoryException mmyExcep)
            {
                throw mmyExcep;
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
            using (SqlCommand command = new SqlCommand(query, (SqlConnection)connection))
            {
                using (SqlDataReader sqlDataReader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Columns.Add(ReportingConstants.NameColumn, typeof(string));
                    table.Columns.Add(ReportingConstants.SchemaColumn, typeof(string));
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
            using (SqlCommand command = new SqlCommand(query, (SqlConnection)connection))
            {
                using (SqlDataReader sqlDataReader = command.ExecuteReader())
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
            using (SqlConnection connection = new SqlConnection(connectionstring))
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
        public DataTable GetTable(DbConnection dbConnection, SqlCommand command)
        {
            using (command.Connection = (SqlConnection)dbConnection)
            {
                DataTable dtable = new DataTable();
                using (SqlDataAdapter dAdapter = new SqlDataAdapter(command))
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
        public List<ReportData> GetTableData(string connectionstring, SqlCommand command, string tableName)
        {
            using (SqlConnection connection = new SqlConnection(connectionstring))
            {
                connection.Open();
                List<ReportData> dataSource = new List<ReportData>();
                try
                {
                    using (command.Connection = connection)
                    {
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                ReportData data = new ReportData();
                                data.Data = new Dictionary<string, object>();
                                for (int i = 0; i < reader.FieldCount; i++)
                                {
                                    data.Data.Add(reader.GetName(i), reader[i] == System.DBNull.Value ? null : reader[i]);
                                }
                                dataSource.Add(data);
                            }
                        }
                        return dataSource;
                    }
                }
                catch (SqlException ex)
                {
                    throw ex;
                }
                catch (Exception excep)
                {
                    throw excep;
                }
                finally
                {
                    connection.Close();
                }
            }
        }

        /// <summary>
        /// Checks whether the connection is valid.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public bool ChecksWhetherValidConnection(string connectionString)
        {

            SqlConnection connection = new SqlConnection(connectionString);
            try
            {
                connection.Open();
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                connection.Close();
            }

        }

        #endregion
    }

}
