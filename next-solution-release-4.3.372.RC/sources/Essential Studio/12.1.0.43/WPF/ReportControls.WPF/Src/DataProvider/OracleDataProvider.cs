//-------------------------------------------------------------------------------------------------
// <copyright file="OracleDataProvider.cs" company="syncfusion">
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
    using System.Data.OracleClient;
    using System.Linq;
    using System.Windows;
    using Syncfusion.RDL.Internal;

    /// <summary>
    /// An Oracle data provider for accessing the oracle data source.
    /// </summary>
    [ObsoleteAttribute("OracleCommand has been deprecated. http://go.microsoft.com/fwlink/?LinkID=144260", false)]
    internal class OracleDataProvider
        : IDataProvider
    {
        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="OracleDataProvider"/> class.
        /// </summary>
        public OracleDataProvider()
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
            using (OracleCommand command = new OracleCommand(query, (OracleConnection)connection))
            {
                using (OracleDataReader OracleDataReader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Columns.Add(ReportingConstants.SchemaColumn, typeof(string));
                    while (OracleDataReader.Read())
                    {
                        table.Rows.Add(new object[] { OracleDataReader.GetString(0) });
                    }

                    return table;
                }
            }
        }

        /// <summary>
        /// Gets schema Column informatation for the given connection string and query.
        /// </summary>
        /// <param name="query">Query string.</param>
        /// <param name="connection">Represents connection to the database.</param>
        /// <returns>Data table containing </returns> 
        public DataTable GetSchemaColumn(string query, DbConnection connection)
        {
            using (OracleCommand command = new OracleCommand(query, (OracleConnection)connection))
            {
                using (OracleDataAdapter sqlDataAdapter = new OracleDataAdapter(command))
                {
                    DataTable table = new DataTable();
                    try
                    {
                        sqlDataAdapter.Fill(table);
                    }

                    catch (Exception oe)
                    {
                        throw oe;
                    }
                    return table;
                }
            }
        }

        /// <summary>
        /// Gets schemaTable informatation for the given connection string and query.
        /// </summary>
        /// <param name="query">Query string.</param>
        /// <param name="connection">Represents connection to the database.</param>
        /// <returns>Data table containing </returns>
        public DataTable GetSchemaTable(string query, DbConnection connection)
        {
            using (OracleCommand command = new OracleCommand(query, (OracleConnection)connection))
            {
                using (OracleDataReader OracleDataReader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Columns.Add(ReportingConstants.NameColumn, typeof(string));
                    table.Columns.Add(ReportingConstants.SchemaColumn, typeof(string));
                    while (OracleDataReader.Read())
                    {
                        table.Rows.Add(new object[] { OracleDataReader.GetString(0), OracleDataReader.GetString(1) });
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
            using (OracleCommand command = new OracleCommand(query, (OracleConnection)connection))
            {
                using (OracleDataReader OracleDataReader = command.ExecuteReader())
                {
                    DataTable table = new DataTable();
                    table.Load(OracleDataReader);
                    return table;
                }
            }
        }

        /// <summary>
        /// Gets Table informatation for the given connection string and query.
        /// </summary>
        /// <param name="connectionstring">Connection string.</param>
        /// <param name="query">Query string.</param>
        /// <returns>Data table containing </returns>
        public DataTable GetTable(string connectionstring, string query)
        {
            using (OracleConnection connection = new OracleConnection(connectionstring))
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
        /// <param name="connectionString">Connection string.</param>
        /// <param name="query">Query string.</param>
        /// <param name="tableName">Table Name as string</param>
        /// <returns>Data table containing </returns>
        public DataTable GetTable(string connectionString, string query, string tableName)
        {
            DataTable dataTable = GetTable(connectionString, query);
            dataTable.TableName = tableName;
            return dataTable;
        }

        #endregion

        #region Helpmer Methods

        /// <summary>
        /// Get Table informations
        /// </summary>
        /// <param name="dbConnection">Connection Represents to Database</param>
        /// <param name="command">Command Represents to Database</param>
        /// <returns>DataTable containing</returns>
        public DataTable GetTable(DbConnection dbConnection, OracleCommand command)
        {
            using (command.Connection = (OracleConnection)dbConnection)
            {
                DataTable dtable = new DataTable();
                using (OracleDataAdapter dAdapter = new OracleDataAdapter(command))
                {
                    DataSet dset = new DataSet();
                    dAdapter.Fill(dset);
                    dtable = dset.Tables[0];
                }
                return dtable;
            }

        }

        /// <summary>
        /// Gets Table informatation for the given connection string and query.
        /// </summary>
        /// <param name="connectionstring">Connection string.</param>
        /// <param name="command">Command Represents to Database.</param>
        /// <returns>Data table containing </returns>
        public List<ReportData> GetTableData(string connectionstring, OracleCommand command, string tableName)
        {
            using (OracleConnection connection = new OracleConnection(connectionstring))
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
        /// Checks whether the connection is valid.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <returns></returns>
        public bool ChecksWhetherValidConnection(string connectionString)
        {

            OracleConnection connection = new OracleConnection(connectionString);
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
