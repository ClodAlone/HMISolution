using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using DataReader.SchemaInfo;
using DataReader.Extensions;
using System.Threading;

namespace DataWriter
{
    /// <summary>
    /// Class Helper for inserting, deletting or updating a database.
    /// </summary>
    public class DataSetWriter : IDisposable
    {
        #region Constructors
        /// <summary>
        /// Create new instance of DataViewWriter by pass provider name and connection string as parameters.
        /// </summary>
        /// <param name="provider">
        /// Provider name to use for connecting to database.
        /// </param>
        /// <param name="connection">
        /// Connection string to use for connecting to database.
        /// </param>
        public DataSetWriter(String provider, String connection)
        {
            dataProvider = provider;
            connectionString = connection;
            maxTransactionsBeforeCommit = 0;
        }

        /// <summary>
        /// Create new instance of DataViewWriter by pass provider name, connection string and max transactions as parameters.
        /// </summary>
        /// <param name="provider">
        /// Provider name to use for connecting to database.
        /// </param>
        /// <param name="connection">
        /// Connection string to use for connecting to database.
        /// </param>
        /// <param name="maxtransactions">
        /// Set to zero for avoiding to use transactions on perform queries to database.
        /// </param>
        /// <remarks>
        /// This instance will use transactions for handling the queries to database.
        /// </remarks>
        public DataSetWriter(String provider, String connection, uint maxtransactions)
        {
            dataProvider = provider;
            connectionString = connection;
            maxTransactionsBeforeCommit = maxtransactions;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Get the DbTransaction linked to this instance.
        /// </summary>
        /// <remarks>
        /// Returned value can be null if the DbTransaction wasn't created.
        /// </remarks>
        public DbTransaction DbTransaction
        {
            get
            {
                return dbTransaction;
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Apply the default schema info to use in order to generate queries.
        /// </summary>
        public void ApplyDbSchemaInfo(DbSchemaInfo dbSchemaInfo)
        {
            if (this.dbSchemaInfo == null)
            {
                keepDbSchemaInfo = true;
                this.dbSchemaInfo = dbSchemaInfo;
            }
        }

        /// <summary>
        /// Add a DbType to SqlDbType specific mapping conversion.
        /// </summary>
        public void AddDbTypeToSqlDbTypeMapping(DbType dbType, SqlDbType sqlDbType)
        {
            if (dbTypeToSqlDbType == null)
                dbTypeToSqlDbType = new Dictionary<DbType, SqlDbType>();
            dbTypeToSqlDbType[dbType] = sqlDbType;
        }

        /// <summary>
        /// Try to open the connection with the database.
        /// </summary>
        /// <returns>
        /// Return true if the connection has been opened or was already opened.
        /// </returns>
        public bool TryOpenConnection()
        {
            try
            {
                OpenConnection();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TryOpenConnection error '{0}'", ex.Message);
                return false;
            }

            return dbConnection != null && dbConnection.State == ConnectionState.Open;
        }

        /// <summary>
        /// Open the connection if isn't opened.
        /// </summary>
        public void OpenConnection()
        {
            if (dbConnection == null)
                dbConnection = DataReader.DataReader.CreateDbConnection(dataProvider, connectionString);
            if (dbConnection.State == ConnectionState.Broken)
                dbConnection.Close();
            if (dbConnection.State == ConnectionState.Closed)
                dbConnection.Open();
        }

        /// <summary>
        /// Close the connection.
        /// </summary>
        public void CloseConnection()
        {
            if (dbConnection != null)
                dbConnection.Close();
        }

        /// <summary>
        /// Try to commits the database transaction.
        /// </summary>
        /// <returns>true if the operaion succeded.</returns>
        public bool TryCommit()
        {
            try
            {
                Commit();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Commits the database transaction.
        /// </summary>
        public void Commit()
        {
            if (dbTransaction != null)
                dbTransaction.Commit();
        }    

        /// <summary>
        /// Try to rolls back a transaction from a pending state.
        /// </summary>
        /// <returns>true if the operation succeded.</returns>
        public bool TryRollback()
        {
            try
            {
                Rollback();
            }
            catch
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Rolls back a transaction from a pending state.
        /// </summary>
        public void Rollback()
        {
            if (dbTransaction != null && dbTransaction.Connection != null)
                dbTransaction.Rollback();
        }

        /// <summary>
        /// Delete the rows using the mapping DataView object pass like parameter.
        /// </summary>
        /// <remarks>
        /// A primary key in the table must exist in order to perform this action, 
        /// when 'basedOnPrimary' parameter has been set to 'true'.
        /// </remarks>
        /// <param name="dataView">
        /// DataView object with the rows to remove.
        /// </param>
        /// <param name="basedOnPrimary">
        /// Set to 'false' for disabling the primary key check.
        /// </param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public int DeleteRows(DataView dataView, bool basedOnPrimary = true)
        {
            PrepareDeleteRows();

            int rowCount = 0;
            if (basedOnPrimary && dataView.Table.PrimaryKey.Count() > 0)
            {
                foreach (DataRowView rowView in dataView)
                {
                    dbDataAdapter.DeleteCommand.Parameters.Clear();

                    var keys = new StringBuilder();
                    for (int ii = 0; ii < dataView.Table.PrimaryKey.Count(); ii++)
                    {
                        var keyparameter = DataReader.DataReader.CreateDbParameter(dataProvider);
                        keyparameter.ParameterName = dbSchemaInfo.FormatParameterName(dataView.Table.PrimaryKey[ii].ColumnName);
                        var dbType = dbSchemaInfo.GetProviderDbType(dataView.Table.PrimaryKey[ii].DataType);
                        if (keyparameter is System.Data.SqlClient.SqlParameter && dbTypeToSqlDbType != null && dbTypeToSqlDbType.ContainsKey(dbType))
                            (keyparameter as System.Data.SqlClient.SqlParameter).SqlDbType = dbTypeToSqlDbType[dbType];
                        else
                            keyparameter.DbType = dbType;
                        keyparameter.Scale = (byte)dbSchemaInfo.GetParameterScale(dataView.Table.PrimaryKey[ii].DataType);
                        keyparameter.Value = TypeExtensions.ChangeType(rowView.Row[dataView.Table.PrimaryKey[ii], DataRowVersion.Original], keyparameter.DbType);
                        dbDataAdapter.DeleteCommand.Parameters.Add(keyparameter);

                        if (keys.Length > 0)
                            keys.Append(" AND ");
                        if (keyparameter.Value.Equals(System.DBNull.Value))
                            keys.Append(String.Format("{0} IS NULL",
                                dbSchemaInfo.WrapObjectName(dataView.Table.PrimaryKey[ii].ColumnName)));
                        else
                            keys.Append(String.Format("{0}={1}",
                                dbSchemaInfo.WrapObjectName(dataView.Table.PrimaryKey[ii].ColumnName),
                                keyparameter.ParameterName));

                    }

                    dbDataAdapter.DeleteCommand.CommandText = String.Format("DELETE FROM {0} WHERE {1}",
                        dbSchemaInfo.WrapObjectName(dataView.Table.TableName), keys.ToString());
                    rowCount += dbDataAdapter.DeleteCommand.ExecuteNonQuery();
                }
            }
            else
            {
                foreach (DataRowView rowView in dataView)
                {
                    dbDataAdapter.DeleteCommand.Parameters.Clear();

                    var colkey = new StringBuilder();
                    for (int ii = 0; ii < dataView.Table.Columns.Count; ii++)
                    {
                        var colparameter = DataReader.DataReader.CreateDbParameter(dataProvider);
                        colparameter.ParameterName = dbSchemaInfo.FormatParameterName(dataView.Table.Columns[ii].ColumnName);
                        var dbType = dbSchemaInfo.GetProviderDbType(dataView.Table.Columns[ii].DataType);
                        if (colparameter is System.Data.SqlClient.SqlParameter && dbTypeToSqlDbType != null && dbTypeToSqlDbType.ContainsKey(dbType))
                            (colparameter as System.Data.SqlClient.SqlParameter).SqlDbType = dbTypeToSqlDbType[dbType];
                        else
                            colparameter.DbType = dbType;
                        colparameter.Scale = (byte)dbSchemaInfo.GetParameterScale(dataView.Table.Columns[ii].DataType);
                        colparameter.Value = TypeExtensions.ChangeType(rowView.Row[dataView.Table.Columns[ii].ColumnName, DataRowVersion.Original], colparameter.DbType);
                        dbDataAdapter.DeleteCommand.Parameters.Add(colparameter);

                        if (colkey.Length > 0)
                            colkey.Append(" AND ");
                        if (colparameter.Value.Equals(System.DBNull.Value))
                            colkey.Append(String.Format("{0} IS NULL",
                                dbSchemaInfo.WrapObjectName(dataView.Table.Columns[ii].ColumnName)));
                        else
                            colkey.Append(String.Format("{0}={1}",
                                dbSchemaInfo.WrapObjectName(dataView.Table.Columns[ii].ColumnName),
                                colparameter.ParameterName));
                    }


                    dbDataAdapter.DeleteCommand.CommandText = String.Format("DELETE FROM {0} WHERE {1}",
                        dbSchemaInfo.WrapObjectName(dataView.Table.TableName), colkey);
                    rowCount += dbDataAdapter.DeleteCommand.ExecuteNonQuery();
                }
            }

            return rowCount;
        }

        /// <summary>
        /// Insert the rows using the mapping DataView object pass like parameter.
        /// </summary>
        /// <param name="dataView">
        /// DataView object with the rows to add.
        /// </param>
        /// <returns>
        /// Return the number of records insert in the database.
        /// </returns>
        public int InsertRows(DataView dataView)
        {
            return InsertRows(dataView, new List<String>(), true);
        }

        /// <summary>
        /// Insert the rows using the mapping DataView object pass like parameter by skipping or using the column pass as second parameter.
        /// </summary>
        /// <param name="dataView">
        /// DataView object with the rows to add.
        /// </param>
        /// <returns>
        /// Return the number of records insert in the database.
        /// <param name="listColumns">
        /// List of columns to skip or use in the update command.
        /// </param>
        /// <param name="skipOrUse">
        /// 'true' for skpping, 'false' for using.
        /// </param>
        /// <returns>
        /// Return the number of records insert in the database.
        /// </returns>        
        public int InsertRows(DataView dataView, IList<String> listColumns, bool skipOrUse, Dictionary<String, DataReader.SchemaInfo.ColumnInfo> columnInfo = null)
        {
            PrepareInsertRows();

            int rowCount = 0;
            foreach (DataRowView rowView in dataView)
            {
                dbDataAdapter.InsertCommand.Parameters.Clear();
                var columns = new StringBuilder();
                var values = new StringBuilder();
                for (int ii = 0; ii < dataView.Table.Columns.Count; ii++)
                {
                    if ((skipOrUse && listColumns.Contains(dataView.Table.Columns[ii].ColumnName)) ||
                        (!skipOrUse && !listColumns.Contains(dataView.Table.Columns[ii].ColumnName)) ||
                        dataView.Table.Columns[ii].AutoIncrement)
                        continue;

                    var parameter = DataReader.DataReader.CreateDbParameter(dataProvider);
                    parameter.ParameterName = dbSchemaInfo.FormatParameterName(dataView.Table.Columns[ii].ColumnName);
                    var dbType = dbSchemaInfo.GetProviderDbType(dataView.Table.Columns[ii].DataType);
                    if (parameter is System.Data.SqlClient.SqlParameter && dbTypeToSqlDbType != null && dbTypeToSqlDbType.ContainsKey(dbType))
                        (parameter as System.Data.SqlClient.SqlParameter).SqlDbType = dbTypeToSqlDbType[dbType];
                    else
                        parameter.DbType = dbType;
                    byte scale;
                    if (columnInfo != null && columnInfo.ContainsKey(dataView.Table.Columns[ii].ColumnName))
                    {
                        scale = (byte)columnInfo[dataView.Table.Columns[ii].ColumnName].Scale;
                        parameter.Precision = (byte)columnInfo[dataView.Table.Columns[ii].ColumnName].Precision;
                    }
                    else
                        scale = (byte)dbSchemaInfo.GetParameterScale(dataView.Table.Columns[ii].DataType);
                    parameter.Scale = scale;
                    parameter.Value = TypeExtensions.ChangeType(rowView[dataView.Table.Columns[ii].ColumnName], parameter.DbType);
                    dbDataAdapter.InsertCommand.Parameters.Add(parameter);

                    if (values.Length > 0)
                        values.Append(", ");
                    values.Append(parameter.ParameterName);

                    if (columns.Length > 0)
                        columns.Append(", ");
                    columns.Append(dbSchemaInfo.WrapObjectName(dataView.Table.Columns[ii].ColumnName));
                }


                if (columns.Length == 0 || values.Length == 0)
                    continue;

                dbDataAdapter.InsertCommand.CommandText = String.Format("INSERT INTO {0} ({1}) VALUES ({2})",
                    dbSchemaInfo.WrapObjectName(dataView.Table.TableName), columns, values);
                rowCount += dbDataAdapter.InsertCommand.ExecuteNonQuery();
            }

            return rowCount;
        }

        /// <summary>
        /// Update the rows using the mapping DataView object pass like parameter.
        /// </summary>
        /// <remarks>
        /// A primary key in the table must exist in order to perform this action.
        /// </remarks>
        /// <param name="dataView">
        /// DataView object with the rows to update.
        /// </param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public int UpdateRows(DataView dataView)
        {
            return UpdateRows(dataView, new List<String>(), new List<String>(), skipOrUseColumns: true);
        }

        /// <summary>
        /// Update the rows using the mapping DataView object pass like parameter by skipping or using the column pass as second parameter.
        /// </summary>
        /// <remarks>
        /// A primary key in the table must exist in order to perform this action,
        /// when 'skipOrUsePrimary' parameter has been set to 'true'
        /// </remarks>
        /// <param name="dataView">
        /// DataView object with the rows to update.
        /// </param>
        /// <param name="listColumns">
        /// List of columns to skip or use in the update command.
        /// </param>
        /// <param name="skipOrUseColumns">
        /// 'true' for skpping, 'false' for using.
        /// </param>
        /// <param name="skipOrUsePrimary">
        /// Set to 'true' for skpping the primary key check.
        /// </param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public int UpdateRows(DataView dataView, IList<String> listColumns, bool skipOrUseColumns, bool skipOrUsePrimary, Dictionary<String, DataReader.SchemaInfo.ColumnInfo> columnInfo = null, bool updateSingle = true)
        {
            PrepareUpdateRows();
            int nCnt = 0;
            var sSQL = new StringBuilder();
            int rowCount = 0;

            if (!updateSingle)
                dbDataAdapter.UpdateCommand.Parameters.Clear();
            foreach (DataRowView rowView in dataView)   
            {
                if(updateSingle)
                    dbDataAdapter.UpdateCommand.Parameters.Clear();
                var columns = new StringBuilder();
                for (int ii = 0; ii < dataView.Table.Columns.Count; ii++)
                {
                    if ((dataView.Table.PrimaryKey.Contains(dataView.Table.Columns[ii]) && skipOrUsePrimary) ||
                        (skipOrUseColumns && listColumns.Contains(dataView.Table.Columns[ii].ColumnName)) ||
                        (!skipOrUseColumns && !listColumns.Contains(dataView.Table.Columns[ii].ColumnName)) ||
                        dataView.Table.Columns[ii].AutoIncrement)
                        continue;

                        var parameter = DataReader.DataReader.CreateDbParameter(dataProvider);
                        parameter.ParameterName = dbSchemaInfo.FormatParameterName($"{dataView.Table.Columns[ii].ColumnName}_{nCnt}");
                        var dbType = dbSchemaInfo.GetProviderDbType(dataView.Table.Columns[ii].DataType);
                        if (parameter is System.Data.SqlClient.SqlParameter && dbTypeToSqlDbType != null && dbTypeToSqlDbType.ContainsKey(dbType))
                            (parameter as System.Data.SqlClient.SqlParameter).SqlDbType = dbTypeToSqlDbType[dbType];
                        else
                            parameter.DbType = dbType;
                        byte scale;
                        if (columnInfo != null && columnInfo.ContainsKey(dataView.Table.Columns[ii].ColumnName))
                        {
                            scale = (byte)columnInfo[dataView.Table.Columns[ii].ColumnName].Scale;
                            parameter.Precision = (byte)columnInfo[dataView.Table.Columns[ii].ColumnName].Precision;
                        }
                        else
                            scale = (byte)dbSchemaInfo.GetParameterScale(dataView.Table.Columns[ii].DataType);
                        parameter.Scale = scale;
                        parameter.Value = TypeExtensions.ChangeType(rowView[dataView.Table.Columns[ii].ColumnName], parameter.DbType);
                        dbDataAdapter.UpdateCommand.Parameters.Add(parameter);

                        if (columns.Length > 0)
                            columns.Append(", ");
                        columns.Append(String.Format("{0}={1}", 
                            dbSchemaInfo.WrapObjectName(dataView.Table.Columns[ii].ColumnName), parameter.ParameterName));
                }

                if (columns.Length == 0)
                    continue;

                if (skipOrUsePrimary && dataView.Table.PrimaryKey.Count() > 0)
                {
                    var keys = new StringBuilder();
                    for (int ii = 0; ii < dataView.Table.PrimaryKey.Count(); ii++)
                    {
                        var keyparameter = DataReader.DataReader.CreateDbParameter(dataProvider);
                        keyparameter.ParameterName = dbSchemaInfo.FormatParameterName($"{dataView.Table.PrimaryKey[ii].ColumnName}_New_{nCnt}");
                        var dbType = dbSchemaInfo.GetProviderDbType(dataView.Table.PrimaryKey[ii].DataType);
                        if (keyparameter is System.Data.SqlClient.SqlParameter && dbTypeToSqlDbType != null && dbTypeToSqlDbType.ContainsKey(dbType))
                            (keyparameter as System.Data.SqlClient.SqlParameter).SqlDbType = dbTypeToSqlDbType[dbType];
                        else
                            keyparameter.DbType = dbType;
                        byte scale;
                        if (columnInfo != null && columnInfo.ContainsKey(dataView.Table.PrimaryKey[ii].ColumnName))
                        {
                            scale = (byte)columnInfo[dataView.Table.PrimaryKey[ii].ColumnName].Scale;
                            keyparameter.Precision = (byte)columnInfo[dataView.Table.PrimaryKey[ii].ColumnName].Precision;
                        }
                        else
                            scale = (byte)dbSchemaInfo.GetParameterScale(dataView.Table.PrimaryKey[ii].DataType);
                        keyparameter.Scale = scale;
                        keyparameter.Value = TypeExtensions.ChangeType(rowView.Row[dataView.Table.PrimaryKey[ii], DataRowVersion.Original], keyparameter.DbType);
                        dbDataAdapter.UpdateCommand.Parameters.Add(keyparameter);

                        if (keys.Length > 0)
                            keys.Append(" AND ");
                        if (keyparameter.Value.Equals(System.DBNull.Value))
                            keys.Append(String.Format("{0} IS NULL",
                                dbSchemaInfo.WrapObjectName(dataView.Table.PrimaryKey[ii].ColumnName)));
                        else
                            keys.Append(String.Format("{0}={1}",
                                dbSchemaInfo.WrapObjectName(dataView.Table.PrimaryKey[ii].ColumnName), 
                                keyparameter.ParameterName));

                    }
                    if(updateSingle)
                    {
                        dbDataAdapter.UpdateCommand.CommandText = String.Format("UPDATE {0} SET {1} WHERE {2}",
                                                                dataView.Table.TableName,
                                                                columns,
                                                                keys);

                        rowCount += dbDataAdapter.UpdateCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        if (sSQL.Length > 0)
                            sSQL.Append(";");
                        sSQL.Append(String.Format("UPDATE {0} SET {1} WHERE {2}",
                                                                dataView.Table.TableName,
                                                                columns,
                                                                keys));
                    }
                }
                else
                {
                    var colkey = new StringBuilder();
                    for (int ii = 0; ii < dataView.Table.Columns.Count; ii++)
                    {
                        if (!skipOrUseColumns && listColumns != null && listColumns.Contains(dataView.Table.Columns[ii].ColumnName))
                            continue;

                        var colparameter = DataReader.DataReader.CreateDbParameter(dataProvider);
                        colparameter.ParameterName = dbSchemaInfo.FormatParameterName($"{dataView.Table.Columns[ii].ColumnName}_New_{nCnt}");
                        var dbType = dbSchemaInfo.GetProviderDbType(dataView.Table.Columns[ii].DataType);
                        if (colparameter is System.Data.SqlClient.SqlParameter && dbTypeToSqlDbType != null && dbTypeToSqlDbType.ContainsKey(dbType))
                            (colparameter as System.Data.SqlClient.SqlParameter).SqlDbType = dbTypeToSqlDbType[dbType];
                        else
                            colparameter.DbType = dbType;
                        byte scale;
                        if (columnInfo != null && columnInfo.ContainsKey(dataView.Table.Columns[ii].ColumnName))
                        {
                            scale = (byte)columnInfo[dataView.Table.Columns[ii].ColumnName].Scale;
                            colparameter.Precision = (byte)columnInfo[dataView.Table.Columns[ii].ColumnName].Precision;
                        }
                        else
                            scale = (byte)dbSchemaInfo.GetParameterScale(dataView.Table.Columns[ii].DataType);
                        colparameter.Scale = scale;
                        colparameter.Value = TypeExtensions.ChangeType(rowView.Row[dataView.Table.Columns[ii].ColumnName, DataRowVersion.Original], colparameter.DbType);
                        dbDataAdapter.UpdateCommand.Parameters.Add(colparameter);

                        if (colkey.Length > 0)
                            colkey.Append(" AND ");
                        if (colparameter.Value.Equals(System.DBNull.Value))
                            colkey.Append(String.Format("{0} IS NULL", 
                                dbSchemaInfo.WrapObjectName(dataView.Table.Columns[ii].ColumnName)));
                        else     
                            colkey.Append(String.Format("{0}={1}",
                                dbSchemaInfo.WrapObjectName(dataView.Table.Columns[ii].ColumnName),
                                colparameter.ParameterName));
                    }
                    if(updateSingle)
                    {
                        dbDataAdapter.UpdateCommand.CommandText = String.Format("UPDATE {0} SET {1} WHERE {2}",
                                                                dataView.Table.TableName,
                                                                columns,
                                                                colkey);
                        rowCount += dbDataAdapter.UpdateCommand.ExecuteNonQuery();
                    }
                    else
                    {
                        if (sSQL.Length > 0)
                            sSQL.Append(";");
                        sSQL.Append(String.Format("UPDATE {0} SET {1} WHERE {2}",
                                                                dataView.Table.TableName,
                                                                columns,
                                                                colkey));
                    }
                }
                nCnt++;
            }
            if(!updateSingle && sSQL.Length > 0)
            {
                dbDataAdapter.UpdateCommand.CommandText = sSQL.ToString();
                rowCount += dbDataAdapter.UpdateCommand.ExecuteNonQuery();
            }
            
            return rowCount;
        }

        /// <summary>
        /// Update the rows using the mapping DataView object pass like parameter by skipping or using the column pass as second parameter.
        /// </summary>
        /// <remarks>
        /// A primary key in the table must exist in order to perform this action.
        /// </remarks>
        /// <param name="dataView">
        /// DataView object with the rows to update.
        /// </param>
        /// <param name="listColumns">
        /// List of columns to skip or use in the update command.
        /// </param>
        /// <param name="skipOrUseColumns">
        /// 'true' for skpping, 'false' for using.
        /// </param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public int UpdateRows(DataView dataView, IList<String> listColumns, bool skipOrUseColumns)
        {
            return UpdateRows(dataView, listColumns, new List<String>(), skipOrUseColumns);
        }

        /// <summary>
        /// Update the rows using the mapping DataView object pass like parameter by skipping or using the column pass as second parameter.
        /// </summary>
        /// <remarks>
        /// A primary key in the table must exist in order to perform this action.
        /// </remarks>
        /// <param name="dataView">
        /// DataView object with the rows to update.
        /// </param>
        /// <param name="listColumns">
        /// List of columns to skip or use in the update command.
        /// </param>
        /// <param name="listKeys">
        /// List of columns to use in the where clause for the update command.
        /// </param>
        /// <param name="skipOrUseColumns">
        /// 'true' for skpping, 'false' for using.
        /// </param>
        /// <returns>
        /// The number of rows affected.
        /// </returns>
        public int UpdateRows(DataView dataView, IList<String> listColumns, IList<String> listKeys, bool skipOrUseColumns, bool updateSingle = true)
        {
            PrepareUpdateRows();
            int nCnt = 0;
            var sSQL = new StringBuilder();
            int rowCount = 0;
            
            if(!updateSingle)
                dbDataAdapter.UpdateCommand.Parameters.Clear();
            foreach (DataRowView rowView in dataView)
            {
                if(updateSingle)
                    dbDataAdapter.UpdateCommand.Parameters.Clear();
                var columns = new StringBuilder();
                for (int ii = 0; ii < dataView.Table.Columns.Count; ii++)
                {
                    if (dataView.Table.PrimaryKey.Contains(dataView.Table.Columns[ii]) ||
                        (skipOrUseColumns && listColumns.Contains(dataView.Table.Columns[ii].ColumnName)) ||
                        (!skipOrUseColumns && !listColumns.Contains(dataView.Table.Columns[ii].ColumnName)) ||
                        dataView.Table.Columns[ii].AutoIncrement)
                        continue;

                    var parameter = DataReader.DataReader.CreateDbParameter(dataProvider);
                    parameter.ParameterName = dbSchemaInfo.FormatParameterName($"{dataView.Table.Columns[ii].ColumnName}_{nCnt}");
                    var dbType = dbSchemaInfo.GetProviderDbType(dataView.Table.Columns[ii].DataType);
                    if (parameter is System.Data.SqlClient.SqlParameter && dbTypeToSqlDbType != null && dbTypeToSqlDbType.ContainsKey(dbType))
                        (parameter as System.Data.SqlClient.SqlParameter).SqlDbType = dbTypeToSqlDbType[dbType];
                    else
                        parameter.DbType = dbType;
                    parameter.Scale = (byte)dbSchemaInfo.GetParameterScale(dataView.Table.Columns[ii].DataType);
                    parameter.Value = TypeExtensions.ChangeType(rowView[dataView.Table.Columns[ii].ColumnName], parameter.DbType);
                    dbDataAdapter.UpdateCommand.Parameters.Add(parameter);

                    if (columns.Length > 0)
                        columns.Append(", ");
                    columns.Append(String.Format("{0}={1}",
                        dbSchemaInfo.WrapObjectName(dataView.Table.Columns[ii].ColumnName),
                        parameter.ParameterName));
                }

                if (columns.Length == 0)
                    continue;

                if (listKeys.Count == 0 && dataView.Table.PrimaryKey.Count() > 0)
                {
                    var where = new StringBuilder();
                    foreach (var key in dataView.Table.PrimaryKey)
                    {
                        var keyparameter = DataReader.DataReader.CreateDbParameter(dataProvider);
                        keyparameter.ParameterName = dbSchemaInfo.FormatParameterName($"{key.ColumnName}_New_{nCnt}");
                        var dbType = dbSchemaInfo.GetProviderDbType(key.DataType);
                        if (keyparameter is System.Data.SqlClient.SqlParameter && dbTypeToSqlDbType != null && dbTypeToSqlDbType.ContainsKey(dbType))
                            (keyparameter as System.Data.SqlClient.SqlParameter).SqlDbType = dbTypeToSqlDbType[dbType];
                        else
                            keyparameter.DbType = dbType;
                        keyparameter.Scale = (byte)dbSchemaInfo.GetParameterScale(key.DataType);
                        keyparameter.Value = TypeExtensions.ChangeType(rowView.Row[key, DataRowVersion.Original], keyparameter.DbType);
                        dbDataAdapter.UpdateCommand.Parameters.Add(keyparameter);

                        if (where.Length > 0)
                            where.Append(" AND ");
                        where.AppendFormat("{0}={1}", dbSchemaInfo.WrapObjectName(key.ColumnName), keyparameter.ParameterName);
                    }

                    if(updateSingle)
                    {
                        dbDataAdapter.UpdateCommand.CommandText = String.Format("UPDATE {0} SET {1} WHERE {2}",
                        dbSchemaInfo.WrapObjectName(dataView.Table.TableName),
                        columns, where.ToString());
                    }
                    else
                    {
                        if (sSQL.Length > 0)
                            sSQL.Append(";");
                        sSQL.Append(String.Format("UPDATE {0} SET {1} WHERE {2}",
                            dbSchemaInfo.WrapObjectName(dataView.Table.TableName),
                            columns, where.ToString()));
                    }
                }
                else
                {
                    var colkey = new StringBuilder();
                    for (int ii = 0; ii < dataView.Table.Columns.Count; ii++)
                    {
                        if (!skipOrUseColumns && listColumns != null && listColumns.Contains(dataView.Table.Columns[ii].ColumnName) || 
                            listKeys.Count > 0 && !listKeys.Contains(dataView.Table.Columns[ii].ColumnName))
                            continue;

                        var colparameter = DataReader.DataReader.CreateDbParameter(dataProvider);
                        colparameter.ParameterName = dbSchemaInfo.FormatParameterName($"{dataView.Table.Columns[ii].ColumnName}_New_{nCnt}");
                        var dbType = dbSchemaInfo.GetProviderDbType(dataView.Table.Columns[ii].DataType);
                        if (colparameter is System.Data.SqlClient.SqlParameter && dbTypeToSqlDbType != null && dbTypeToSqlDbType.ContainsKey(dbType))
                            (colparameter as System.Data.SqlClient.SqlParameter).SqlDbType = dbTypeToSqlDbType[dbType];
                        else
                            colparameter.DbType = dbType;
                        colparameter.Scale = (byte)dbSchemaInfo.GetParameterScale(dataView.Table.Columns[ii].DataType);
                        colparameter.Value = TypeExtensions.ChangeType(rowView.Row[dataView.Table.Columns[ii].ColumnName, DataRowVersion.Original], colparameter.DbType);
                        dbDataAdapter.UpdateCommand.Parameters.Add(colparameter);

                        if (colkey.Length > 0)
                            colkey.Append(" AND ");
                        if (colparameter.Value.Equals(System.DBNull.Value))
                            colkey.Append(String.Format("{0} IS NULL",
                                dbSchemaInfo.WrapObjectName(dataView.Table.Columns[ii].ColumnName)));
                        else
                            colkey.Append(String.Format("{0}={1}",
                                dbSchemaInfo.WrapObjectName(dataView.Table.Columns[ii].ColumnName),
                                colparameter.ParameterName));
                    }
                    if(updateSingle)
                    {
                        dbDataAdapter.UpdateCommand.CommandText = String.Format("UPDATE {0} SET {1} WHERE {2}",
                                                            dataView.Table.TableName,
                                                            columns,
                                                            colkey);
                    }
                    else
                    {
                        if (sSQL.Length > 0)
                            sSQL.Append(";");
                        sSQL.Append(String.Format("UPDATE {0} SET {1} WHERE {2}",
                                                                dataView.Table.TableName,
                                                                columns,
                                                                colkey));
                    }
                    
                }
                nCnt++;
                if(updateSingle)
                    rowCount += dbDataAdapter.UpdateCommand.ExecuteNonQuery();
            }
            if(!updateSingle && sSQL.Length > 0)
            {
                dbDataAdapter.UpdateCommand.CommandText = sSQL.ToString();
                
                rowCount += dbDataAdapter.UpdateCommand.ExecuteNonQuery();
            }
            
            return rowCount;
        }

        /// <summary>
        /// Export data to file in XML format.
        /// </summary>
        /// <param name="dataView">
        /// DataView with the data to be exported.
        /// </param>
        /// <param name="filePath">
        /// File path where data have to be exported.
        /// </param>
        public void ExportDataToXmlFile(DataView dataView, String filePath)
        {
            var path = System.IO.Path.GetDirectoryName(filePath);
            if (!System.IO.Directory.Exists(path))
                System.IO.Directory.CreateDirectory(path);
            dataView.Table.WriteXml(filePath, true);
        }

        /// <summary>
        /// Export data to string in XML format.
        /// </summary>
        /// <param name="dataView">
        /// DataView with the data to be exported.
        /// </param>
        public string ExportDataToXmlString(DataView dataView)
        {
            using (var memoryStream = new System.IO.MemoryStream())
            {
                dataView.Table.WriteXml(memoryStream, true);
                return Convert.ToBase64String(memoryStream.ToArray());
            }
        }

        /// <summary>
        /// Check the presence and schemaof the dataset's tables inside the database.
        /// </summary>
        /// <param name="dataSet">
        /// DataSet structure of the tables to check
        public void CheckTables(DataSet dataSet)
        {
            CheckTables(dataSet, skipCheckColumns: false);
        }

        /// <summary>
        /// Check the presence and schemaof the dataset's tables inside the database.
        /// </summary>
        /// <param name="dataSet">
        /// DataSet structure of the tables to check
        ///  <param name="skipCheckColumns">
        /// Allow to skip the columns check consistency.
        /// </param>
        public void CheckTables(DataSet dataSet, bool skipCheckColumns)
        {
            CheckTables(dataSet, new string[] { }, skipCheckColumns);
        }

        /// <summary>
        /// Check the presence and schemaof the dataset's tables inside the database.
        /// </summary>
        /// <param name="dataSet">
        /// DataSet structure of the tables to check
        ///  <param name="indexColumns">
        /// Index columns to insert in the table.
        /// </param>
        ///  <param name="skipCheckColumns">
        /// Allow to skip the columns check consistency.
        /// </param>
        public void CheckTables(DataSet dataSet, string[] indexColumns, bool skipCheckColumns)
        {
            CheckTables(dataSet, indexColumns, CancellationToken.None, skipCheckColumns);
        }

        /// <summary>
        /// Check the presence and schemaof the dataset's tables inside the database.
        /// </summary>
        /// <param name="dataSet">
        /// DataSet structure of the tables to check
        ///  <param name="token">
        /// Cancellation token to use for abort the long operations.
        /// </param>
        ///  <param name="skipCheckColumns">
        /// Allow to skip the columns check consistency.
        /// </param>
        public void CheckTables(DataSet dataSet, CancellationToken token, bool skipCheckColumns)
        {
            CheckTables(dataSet, new string[] { }, token, skipCheckColumns);
        }

        /// <summary>
        /// Check the presence and schemaof the dataset's tables inside the database.
        /// </summary>
        /// <param name="dataSet">
        /// DataSet structure of the tables to check
        ///  <param name="indexColumns">
        /// Index columns to insert in the table.
        /// </param>
        ///  <param name="token">
        /// Cancellation token to use for abort the long operations.
        /// </param>
        ///  <param name="skipCheckColumns">
        /// Allow to skip the columns check consistency.
        /// </param>
        public void CheckTables(DataSet dataSet, string[] indexColumns, CancellationToken token, bool skipCheckColumns)
        {
            List<DataColumn> columnsToDrop;
            List<DataColumn> columnsToAdd;
            List<DataTable> tablesToAdd;
            Dictionary<DataColumn, String> columnsToRename;
            List<DataTable> tablesToRebuildKeys;

            try
            {
                CheckTables(dataSet, out columnsToDrop, out columnsToAdd, out columnsToRename, out tablesToAdd, out tablesToRebuildKeys, skipCheckColumns);
            }
            catch (Exception ex)
            {
                throw new InvalidSchemaTableException(FaultOperation.QuerySchema, ex.Message);
            }

            try
            {
                if (tablesToRebuildKeys.Count > 0)
                {
                    foreach (var table in tablesToRebuildKeys)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        // Try to remove the precedent primary key constraint.
                        try
                        {
                            DropConstraint(table);
                        }
                        catch
                        { }
                    }
                }

                if (columnsToRename.Count > 0)
                {
                    foreach (var col in columnsToRename.Keys)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        RenameColumn(col, columnsToRename[col]);
                    }
                }

                // Before try to remove columns from table.
                if (columnsToDrop.Count > 0)
                {
                    foreach (var col in columnsToDrop)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        if (!columnsToAdd.Contains(col))
                            continue;

                        try
                        {
                            DropConstraint(col);
                        }
                        catch
                        { }

                        try
                        {
                            DropIndex(col);
                        }
                        catch
                        { }

                        DropColumn(col);
                    }
                }

                // After try to add columns inside table.
                if (columnsToAdd.Count > 0)
                {
                    foreach (var col in columnsToAdd)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        AddColumn(col);
                        if (indexColumns.Contains(col.ColumnName))
                            CreateIndex(col);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidSchemaTableException(FaultOperation.ChangeSchema, ex.Message);
            }

            try
            {
                // Finish try to add new tables.
                if (tablesToAdd.Count > 0)
                {
                    foreach (var table in tablesToAdd)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        CreateTable(table);
                        foreach (var colname in indexColumns)
                        {
                            if (token.IsCancellationRequested)
                                return;

                            if (table.Columns.Contains(colname))
                                CreateIndex(table.Columns[colname]);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidSchemaTableException(FaultOperation.CreateSchema, ex.Message);
            }

            try
            {
                if (tablesToRebuildKeys.Count > 0)
                {
                    foreach (var table in tablesToRebuildKeys)
                    {
                        if (token.IsCancellationRequested)
                            return;

                        RebuildPrimaryKeys(table);
                    }
                }
            }
            catch (Exception ex)
            {
                throw new InvalidSchemaTableException(FaultOperation.RebuildPrimaryKeys, ex.Message);
            }
        }

        /// <summary>
        /// Try create Database using the provider name and connection string pass in the constructor of this instance.
        /// </summary>
        public bool TryCreateDataBase()
        {
            try
            {
                CreateDataBase();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TryCreateDataBase error '{0}'", ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Try create Database Backup using the provider name and connection string pass in the constructor of this instance.
        /// </summary>
        public bool TryCreateBackup()
        {
            try
            {
                CreateBackup();
            }
            catch (Exception ex)
            {
                Debug.WriteLine("TryCreateBackup error '{0}'", ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Create Database using the provider name and connection string pass in the constructor of this instance.
        /// </summary>
        public void CreateDataBase()
        {
            DataReader.SchemaInfo.DbSchemaInfoFactory.CreateDatabase(dataProvider, connectionString);
        }

        /// <summary>
        /// Create Database Backup using the provider name and connection string pass in the constructor of this instance.
        /// </summary>
        public void CreateBackup()
        {
            DataReader.SchemaInfo.DbSchemaInfoFactory.CreateBackup(dataProvider, connectionString);
        }

        /// <summary>
        /// Create a new table inside database.
        /// </summary>
        /// <param name="table"></param>
        public void CreateTable(DataTable table)
        {
            PrepareSqlCommand();
            StringBuilder columns = new StringBuilder();
            for (int ii = 0; ii < table.Columns.Count; ii++)
            {
                var col = table.Columns[ii];
                    
                if (columns.Length > 0)
                    columns.Append(", ");

                columns.AppendFormat("{0} {1}", 
                    dbSchemaInfo.WrapObjectName(col.ColumnName), dbSchemaInfo.FormatDataTypeName(col));

                if (col.AutoIncrement)
                {
                    columns.Append(dbSchemaInfo.FormatAutoIncrement(col));
                }
                else
                {
                    if (!col.AllowDBNull && col.DefaultValue != null && !String.IsNullOrEmpty(col.DefaultValue.ToString()))
                    {
                        if (dbSchemaInfo.IsSupportedConstraintKeyword)
                            columns.AppendFormat(" CONSTRAINT {0} DEFAULT {1}",
                                dbSchemaInfo.WrapObjectName(String.Format("df_{0}_{1}", table.TableName, col.ColumnName)),
                                dbSchemaInfo.FormatDefaultValue(col));
                        else
                            columns.AppendFormat(" DEFAULT {0}", dbSchemaInfo.FormatDefaultValue(col));
                    }
                }

                if (!col.AllowDBNull)
                    columns.Append(" NOT NULL");
                if (col.Unique && !table.PrimaryKey.Contains(col))
                    columns.Append(" UNIQUE");
            }

            if (table.PrimaryKey.Length > 0)
            {
                var keys = new StringBuilder();
                foreach (var key in table.PrimaryKey)
                {
                    if (keys.Length > 0)
                        keys.Append(", ");
                    keys.Append(dbSchemaInfo.WrapObjectName(key.ColumnName));
                }

                if (dbSchemaInfo.IsSupportedConstraintKeyword)
                    columns.AppendFormat(", CONSTRAINT {0} PRIMARY KEY ({1})", dbSchemaInfo.WrapObjectName(String.Format("pk_{0}", table.TableName)), keys);
                else
                    columns.AppendFormat(", PRIMARY KEY ({0})", keys);
            }

            dbCommand.CommandText = String.Format("CREATE TABLE {0} ({1})",
                dbSchemaInfo.WrapObjectName(table.TableName), columns);
            dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Rebuild the primary keys inside the table.
        /// </summary>
        /// <param name="table"></param>
        public void RebuildPrimaryKeys(DataTable table)
        {
            if (table.PrimaryKey.Length > 0)
            {
                PrepareSqlCommand();

                var keys = new StringBuilder();
                foreach (var key in table.PrimaryKey)
                {
                    if (keys.Length > 0)
                        keys.Append(", ");
                    keys.Append(dbSchemaInfo.WrapObjectName(key.ColumnName));
                }

                if (dbSchemaInfo.IsSupportedConstraintKeyword)
                    dbCommand.CommandText = String.Format("ALTER TABLE {0} ADD CONSTRAINT {1} PRIMARY KEY ({2})", 
                        dbSchemaInfo.WrapObjectName(table.TableName), 
                        dbSchemaInfo.WrapObjectName(String.Format("pk_{0}", table.TableName)), keys);
                else
                    dbCommand.CommandText = String.Format("ALTER TABLE {0} ADD PRIMARY KEY ({1})", 
                        dbSchemaInfo.WrapObjectName(table.TableName), keys);
                dbCommand.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Create a new index inside the table.
        /// </summary>
        /// <param name="col"></param>
        public void CreateIndex(DataColumn col)
        {
            PrepareSqlCommand();

            var builder = new StringBuilder("CREATE");
            if (col.Unique)
                builder.Append(" UNIQUE");

            builder.AppendFormat(" INDEX {0} ON {1} ({2})", 
                dbSchemaInfo.WrapObjectName(String.Format("{0}_Index", col.ColumnName)), 
                dbSchemaInfo.WrapObjectName(col.Table.TableName),
                dbSchemaInfo.WrapObjectName(col.ColumnName));
            
            dbCommand.CommandText = builder.ToString();
            dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Clean all records inside existing table from database.
        /// </summary>
        /// <param name="table"></param>
        /// <returns>
        /// Return the number of records deleted from table
        /// </returns>
        public int CleanTable(DataTable table)
        {
            PrepareSqlCommand();
            dbCommand.CommandText = String.Format("DELETE FROM {0}", dbSchemaInfo.WrapObjectName(table.TableName));
            return dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Clean records inside existing table by pass a timecolumn and date time in utc format.
        /// </summary>
        /// <param name="table">
        /// DataTable schema.
        /// </param>
        /// <param name="timeCol">
        /// DataColumn must be of DateTime type.
        /// </param>
        /// <param name="utcTime">
        /// DateTime to use for deleting old data.
        /// </param>
        /// <returns>
        /// Return the number of records deleted from table
        /// </returns>
        public int CleanTable(DataTable table, DataColumn timeCol, DateTime utcTime)
        {
            PrepareSqlCommand();
            var keyparameter = DataReader.DataReader.CreateDbParameter(dataProvider);
            keyparameter.ParameterName = dbSchemaInfo.FormatParameterName(timeCol.ColumnName);
            var dbType = dbSchemaInfo.GetProviderDataType(timeCol.DataType).ToDbType();
            if (keyparameter is System.Data.SqlClient.SqlParameter && dbTypeToSqlDbType != null && dbTypeToSqlDbType.ContainsKey(dbType))
                (keyparameter as System.Data.SqlClient.SqlParameter).SqlDbType = dbTypeToSqlDbType[dbType];
            else
                keyparameter.DbType = dbType;
            keyparameter.Scale = (byte)dbSchemaInfo.GetParameterScale(timeCol.DataType);
            keyparameter.Value = TypeExtensions.ChangeType(AdjustDateTime(utcTime), keyparameter.DbType);
            dbCommand.Parameters.Add(keyparameter);

            dbCommand.CommandText = String.Format("DELETE FROM {0} WHERE {1}<{2}",
            dbSchemaInfo.WrapObjectName(table.TableName), 
            dbSchemaInfo.WrapObjectName(timeCol.ColumnName), keyparameter.ParameterName);
            return dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Clean records inside the provided table name by passing a specific command text.
        /// </summary>
        /// <param name="cleanCommand">
        /// String with the text of the command to execute.
        /// The string must contain the following parameters:
        /// {0}: it will be replaced with the table name.
        /// {1}: it will be replaced with the time column name.
        /// {2}: it will be replaced with the utc time value.
        /// </param>
        /// <param name="tableName">
        /// Name of the table where performing the clean operation.
        /// </param>
        /// <param name="timeCol">
        /// DataColumn must be of DateTime type.
        /// </param>
        /// <param name="utcTime">
        /// DateTime to use for deleting old data.
        /// </param>
        /// <returns>
        /// Return the number of records deleted from table
        /// </returns>
        public int CleanTable(string cleanCommand, string tableName, DataColumn timeCol, DateTime utcTime)
        {
            PrepareSqlCommand();
            var keyparameter = DataReader.DataReader.CreateDbParameter(dataProvider);
            keyparameter.ParameterName = dbSchemaInfo.FormatParameterName(timeCol.ColumnName);
            var dbType = dbSchemaInfo.GetProviderDataType(timeCol.DataType).ToDbType();
            if (keyparameter is System.Data.SqlClient.SqlParameter && dbTypeToSqlDbType != null && dbTypeToSqlDbType.ContainsKey(dbType))
                (keyparameter as System.Data.SqlClient.SqlParameter).SqlDbType = dbTypeToSqlDbType[dbType];
            else
                keyparameter.DbType = dbType;
            keyparameter.Scale = (byte)dbSchemaInfo.GetParameterScale(timeCol.DataType);
            keyparameter.Value = TypeExtensions.ChangeType(AdjustDateTime(utcTime), keyparameter.DbType);
            dbCommand.Parameters.Add(keyparameter);

            dbCommand.CommandText = String.Format(cleanCommand, 
                tableName, timeCol.ColumnName, keyparameter.ParameterName);
            return dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Remove an existing table from database.
        /// </summary>
        /// <param name="tableName"></param>
        public void RemoveTable(string tableName)
        {
            PrepareSqlCommand();
            dbCommand.CommandText = String.Format("DROP TABLE {0}",
                dbSchemaInfo.WrapObjectName(tableName));
            dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Remove a constraint inside an existing table inside database.
        /// </summary>
        /// <param name="col"></param>
        public void DropConstraint(DataColumn col)
        {
            PrepareSqlCommand();
            if (dbSchemaInfo.IsSupportedConstraintKeyword)
                 dbCommand.CommandText = String.Format("ALTER TABLE {0} DROP CONSTRAINT {1}", 
                    dbSchemaInfo.WrapObjectName(col.Table.TableName),
                    dbSchemaInfo.WrapObjectName(String.Format("df_{0}_{1}", col.Table.TableName, col.ColumnName)));
            else
                dbCommand.CommandText = String.Format("ALTER TABLE {0} ALTER {1} DROP DEFAULT", 
                    dbSchemaInfo.WrapObjectName(col.Table.TableName), 
                    dbSchemaInfo.WrapObjectName(col.ColumnName));
            dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Remove a constraint inside an existing table inside database.
        /// </summary>
        /// <param name="table"></param>
        public void DropConstraint(DataTable table)
        {
            PrepareSqlCommand();
            if (dbSchemaInfo.IsSupportedConstraintKeyword)
                dbCommand.CommandText = String.Format("ALTER TABLE {0} DROP CONSTRAINT {1}", 
                    dbSchemaInfo.WrapObjectName(table.TableName),
                    dbSchemaInfo.WrapObjectName(String.Format("pk_{0}", table.TableName)));
            else
                dbCommand.CommandText = String.Format("ALTER TABLE {0} DROP PRIMARY KEY", 
                    dbSchemaInfo.WrapObjectName(table.TableName));
            dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Try to remove all constraints inside an existing table of the database.
        /// </summary>
        /// <param name="tableName"></param>
        public void TryDropConstraints(string tableName)
        {
            // Drop constraints.
            DataTable dbTable = new DataTable();
            dbDataAdapter.SelectCommand.CommandText = String.Format("SELECT * FROM {0}", dbSchemaInfo.WrapObjectName(tableName));
            dbDataAdapter.FillSchema(dbTable, SchemaType.Source);

            try
            {
                DropConstraint(dbTable);
            }
            catch
            { }

            foreach (DataColumn column in dbTable.Columns)
            {
                try
                {
                    DropConstraint(column);
                }
                catch
                { }
            }
        }

        /// <summary>
        /// Remove a index inside an existing table of the database.
        /// </summary>
        /// <param name="col"></param>
        public void DropIndex(DataColumn col)
        {
            PrepareSqlCommand();

            if (dbSchemaInfo.IsTableNameNeedOnDropIndex)
            {
                dbCommand.CommandText = String.Format("DROP INDEX {0} ON {1}",
                    dbSchemaInfo.WrapObjectName(String.Format("{0}_Index", col.ColumnName)),
                    dbSchemaInfo.WrapObjectName(col.Table.TableName));
            }
            else
            {
                dbCommand.CommandText = String.Format("DROP INDEX {0}",
                    dbSchemaInfo.WrapObjectName(String.Format("{0}_Index", col.ColumnName)));
            }
            dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Remove a column from an existing table inside database.
        /// </summary>
        /// <param name="col"></param>
        public void DropColumn(DataColumn col)
        {
            PrepareSqlCommand();
            dbCommand.CommandText = String.Format("ALTER TABLE {0} DROP COLUMN {1}", 
                dbSchemaInfo.WrapObjectName(col.Table.TableName), 
                dbSchemaInfo.WrapObjectName(col.ColumnName));
            dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Add a new column inside an exisisting table if the database.
        /// </summary>
        /// <param name="col"></param>
        public void AddColumn(DataColumn col)
        {
            PrepareSqlCommand();
            var column = String.Format("{0} {1}", dbSchemaInfo.WrapObjectName(col.ColumnName), GetColumnDefinition(col));
            dbCommand.CommandText = String.Format("ALTER TABLE {0} ADD {1}", dbSchemaInfo.WrapObjectName(col.Table.TableName), column);
            dbCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Rename an existing table inside database.
        /// </summary>
        /// <param name="col"></param>
        public void RenameTable(string oldName, string newName)
        {
            try
            {
                PrepareSqlCommand();
                dbCommand.CommandText = String.Format(dbSchemaInfo.RenameTableNameSyntax,
                    oldName, newName);
                dbCommand.ExecuteNonQuery();
            }
            catch
            {
                // Fill schema table.
                DataTable dbTable = new DataTable();
                dbDataAdapter.SelectCommand.CommandText = String.Format("SELECT * FROM {0}", dbSchemaInfo.WrapObjectName(oldName));
                dbDataAdapter.FillSchema(dbTable, SchemaType.Source);

                // Create new table.
                dbTable.TableName = newName;
                CreateTable(dbTable);

                // Copy all records from old table to new table.
                dbCommand.CommandText = String.Format("INSERT INTO {0} SELECT * FROM {1}", dbSchemaInfo.WrapObjectName(newName), dbSchemaInfo.WrapObjectName(oldName));
                dbCommand.ExecuteNonQuery();

                // Remove old table and dropconstraints
                RemoveTable(oldName);
            }
        }

        /// <summary>
        /// Rename a column from an existing table inside database.
        /// </summary>
        /// <param name="col"></param>
        public void RenameColumn(DataColumn col, string oldName)
        {
            PrepareSqlCommand();
            dbCommand.CommandText = String.Format(dbSchemaInfo.RenameTableColumnNameSyntax,
                col.Table.TableName, oldName, col.ColumnName);
            if (dbSchemaInfo.IsDataTypeNeedOnColumnChange)
            {
                dbCommand.CommandText = String.Format("{0} {1}", 
                    dbCommand.CommandText, GetColumnDefinition(col));
            }
            dbCommand.ExecuteNonQuery();
        }
        #endregion

        #region Private Methods
        void PrepareDeleteRows()
        {
            OpenConnection();

            if (dbDataAdapter == null)
                dbDataAdapter = DataReader.DataReader.CreateDbDataAdapter(dataProvider);
            if (dbSchemaInfo == null)
                dbSchemaInfo = DbSchemaInfoFactory.CreateSchemaInfo(dataProvider, connectionString);
            if (maxTransactionsBeforeCommit > 0 && dbTransaction == null)
                dbTransaction = dbConnection.BeginTransaction();

            if (dbDataAdapter.DeleteCommand == null)
            {
                dbDataAdapter.DeleteCommand = DataReader.DataReader.CreateDbCommand(dataProvider);
                dbDataAdapter.DeleteCommand.Connection = dbConnection;
                dbDataAdapter.DeleteCommand.Transaction = dbTransaction;
            }
        }

        void PrepareInsertRows()
        {
            OpenConnection();

            if (dbDataAdapter == null)
                dbDataAdapter = DataReader.DataReader.CreateDbDataAdapter(dataProvider);
            if (dbSchemaInfo == null)
                dbSchemaInfo = DbSchemaInfoFactory.CreateSchemaInfo(dataProvider, connectionString);
            if (maxTransactionsBeforeCommit > 0 && dbTransaction == null)
                dbTransaction = dbConnection.BeginTransaction();

            if (dbDataAdapter.InsertCommand == null)
            {
                dbDataAdapter.InsertCommand = DataReader.DataReader.CreateDbCommand(dataProvider);
                dbDataAdapter.InsertCommand.Connection = dbConnection;
                dbDataAdapter.InsertCommand.Transaction = dbTransaction;
            }
        }

        void PrepareUpdateRows()
        {
            OpenConnection();

            if (dbDataAdapter == null)
                dbDataAdapter = DataReader.DataReader.CreateDbDataAdapter(dataProvider);
            if (dbSchemaInfo == null)
                dbSchemaInfo = DbSchemaInfoFactory.CreateSchemaInfo(dataProvider, connectionString);
            if (maxTransactionsBeforeCommit > 0 && dbTransaction == null)
                dbTransaction = dbConnection.BeginTransaction();

            if (dbDataAdapter.UpdateCommand == null)
            {
                dbDataAdapter.UpdateCommand = DataReader.DataReader.CreateDbCommand(dataProvider);
                dbDataAdapter.UpdateCommand.Connection = dbConnection;
                dbDataAdapter.UpdateCommand.Transaction = dbTransaction;
            }
        }

        void PrepareSelectRows()
        {
            OpenConnection();

            if (dbDataAdapter == null)
                dbDataAdapter = DataReader.DataReader.CreateDbDataAdapter(dataProvider);
            if (dbSchemaInfo == null)
                dbSchemaInfo = DbSchemaInfoFactory.CreateSchemaInfo(dataProvider, connectionString);
            if (maxTransactionsBeforeCommit > 0 && dbTransaction == null)
                dbTransaction = dbConnection.BeginTransaction();

            if (dbDataAdapter.SelectCommand == null)
            {
                dbDataAdapter.SelectCommand = DataReader.DataReader.CreateDbCommand(dataProvider);
                dbDataAdapter.SelectCommand.Connection = dbConnection;
                dbDataAdapter.SelectCommand.Transaction = dbTransaction;
            }
        }

        void PrepareSqlCommand()
        {
            OpenConnection();

            if (dbSchemaInfo == null)
                dbSchemaInfo = DbSchemaInfoFactory.CreateSchemaInfo(dataProvider, connectionString);
            if (maxTransactionsBeforeCommit > 0 && dbTransaction == null)
                dbTransaction = dbConnection.BeginTransaction();

            if (dbCommand == null)
            {
                dbCommand = DataReader.DataReader.CreateDbCommand(dataProvider);
                dbCommand.CommandType = CommandType.Text;
                dbCommand.Connection = dbConnection;
                dbCommand.Transaction = dbTransaction;
            }
        }

        void CheckTables(DataSet dataSet, out List<DataColumn> columnsToRemove, out List<DataColumn> columnsToAdd, out Dictionary<DataColumn, String> columnsToRename, out List<DataTable> tablesToAdd, out List<DataTable> tablesToRebuildKeys, bool skipCheckColumns)
        {
            // Initialize the output parameters.
            columnsToRemove = new List<DataColumn>();
            columnsToAdd = new List<DataColumn>();
            columnsToRename = new Dictionary<DataColumn, String>();
            tablesToAdd = new List<DataTable>();
            tablesToRebuildKeys = new List<DataTable>();

            var tables = DataReader.DataReader.ListTables(dataProvider, connectionString);
            PrepareSelectRows();
            for (int cc = 0; cc < dataSet.Tables.Count; cc++)
            {
                var table = dataSet.Tables[cc];
                if (tables.Contains(table.TableName, StringComparer.CurrentCultureIgnoreCase))
                {
                    DataTable dbTable = new DataTable();
                    dbDataAdapter.SelectCommand.CommandText = String.Format("SELECT * FROM {0}", dbSchemaInfo.WrapObjectName(table.TableName));
                    dbDataAdapter.FillSchema(dbTable, SchemaType.Source);
                    foreach (DataColumn col in table.Columns)
                    {
                        if (dbTable.Columns.Contains(col.ColumnName))
                        {
                            DataColumn dbCol = dbTable.Columns[col.ColumnName];
                            if (!skipCheckColumns && !CompareColumnProperties(dbCol, col))
                            {
                                Debug.WriteLine(String.Format("Column {0} for Table '{1}' must be recreated !",
                                                                col.ColumnName, table.TableName));

                                columnsToRemove.Add(col);
                                columnsToAdd.Add(col);

                                if (!tablesToRebuildKeys.Contains(table) &&
                                    dbTable.PrimaryKey.Contains(dbCol))
                                {
                                    tablesToRebuildKeys.Add(table);
                                }
                            }

                            if (!tablesToRebuildKeys.Contains(table) &&
                                (dbTable.PrimaryKey.Contains(dbCol) && !table.PrimaryKey.Contains(col) ||
                                !dbTable.PrimaryKey.Contains(dbCol) && table.PrimaryKey.Contains(col)))
                            {
                                tablesToRebuildKeys.Add(table);
                            }
                        }
                        else
                        {
                            columnsToAdd.Add(col);

                            if (!tablesToRebuildKeys.Contains(table) &&
                                table.PrimaryKey.Contains(col))
                            {
                                tablesToRebuildKeys.Add(table);
                            }
                        }
                    }

                    foreach (DataColumn dbCol in dbTable.Columns)
                    {
                        if (!table.Columns.Contains(dbCol.ColumnName))
                        {
                            columnsToRemove.Add(dbCol);

                            if (!tablesToRebuildKeys.Contains(table) &&
                                dbTable.PrimaryKey.Contains(dbCol))
                            {
                                tablesToRebuildKeys.Add(table);
                            }
                        }
                    }

                    if (table.PrimaryKey.Length > 0 && table.PrimaryKey.Length == dbTable.PrimaryKey.Length)
                    {
                        var dbPrimaryKeys = (from c in dbTable.PrimaryKey select c).ToDictionary(c => c.ColumnName, c => c);
                        foreach (var col in table.PrimaryKey)
                        {
                            if (dbPrimaryKeys.ContainsKey(col.ColumnName))
                                continue;

                            var matchColumnName = (from key in dbPrimaryKeys.Keys
                                                   where key.ToUpper() != col.ColumnName.ToUpper() && 
                                                   CompareColumnProperties(dbPrimaryKeys[key], col)
                                                   select key).FirstOrDefault();

                            if (matchColumnName != null)
                            {
                                columnsToAdd.Remove(col);
                                columnsToRemove.Remove(dbPrimaryKeys[matchColumnName]);
                                columnsToRename.Add(col, matchColumnName);
                                dbPrimaryKeys.Remove(matchColumnName);
                            }
                        }
                    }
                }
                else
                    tablesToAdd.Add(table);
            }
        }

        bool CompareColumnProperties(DataColumn tableCol, DataColumn datasetCol)
        {
            return dbSchemaInfo.AreCompatibleDataTypes(tableCol.DataType, datasetCol.DataType);
        }

        string GetColumnDefinition(DataColumn col)
        {
            StringBuilder column = new StringBuilder(dbSchemaInfo.FormatDataTypeName(col));

            if (!col.AllowDBNull && col.DefaultValue != null && !String.IsNullOrEmpty(col.DefaultValue.ToString()))
            {
                if (dbSchemaInfo.IsSupportedConstraintKeyword)
                    column.AppendFormat(" CONSTRAINT {0} DEFAULT {1}",
                        dbSchemaInfo.WrapObjectName(String.Format("df_{0}_{1}", col.Table.TableName, col.ColumnName)),
                        dbSchemaInfo.FormatDefaultValue(col));
                else
                    column.AppendFormat(" DEFAULT {0}", dbSchemaInfo.FormatDefaultValue(col));
            }
            if (!col.AllowDBNull)
                column.Append(" NOT NULL");
            if (col.Unique)
                column.Append(" UNIQUE");

            return column.ToString();
        }

        DateTime AdjustDateTime(DateTime dateTime)
        {
            if (dateTime < System.Data.SqlTypes.SqlDateTime.MinValue.Value)
                dateTime = System.Data.SqlTypes.SqlDateTime.MinValue.Value;
            else if (dateTime > System.Data.SqlTypes.SqlDateTime.MaxValue.Value)
                dateTime = System.Data.SqlTypes.SqlDateTime.MaxValue.Value;

            return dateTime;
        }
        #endregion

        #region Private Members
        readonly String dataProvider;
        readonly String connectionString;
        readonly uint maxTransactionsBeforeCommit;

        bool keepDbSchemaInfo;

        DbConnection dbConnection;
        DbDataAdapter dbDataAdapter;
        DbSchemaInfo dbSchemaInfo;
        DbCommand dbCommand;
        DbTransaction dbTransaction;
        Dictionary<DbType, SqlDbType> dbTypeToSqlDbType;
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose the internal IDisposable object's class
        /// </summary>
        /// </param>
        public void Dispose()
        {
            Dispose(false);
        }

        /// <summary>
        /// Dispose the internal IDisposable object's class
        /// </summary>
        /// </param>
        /// <param name="keepConnectionOpen">
        /// Connection string to use for connecting to database.
        /// </param>
        public void Dispose(bool keepConnectionOpen)
        {
            if (dbTransaction != null)
                dbTransaction.Dispose();
            dbTransaction = null;

            if (!keepConnectionOpen && dbConnection != null)
            {
                dbConnection.Close();
                dbConnection = null;
            }

            if (!keepDbSchemaInfo && dbSchemaInfo != null)
            {
                dbSchemaInfo.Dispose();
                dbSchemaInfo = null;
            }

            if (dbDataAdapter != null)
                dbDataAdapter.Dispose();
            dbDataAdapter = null;

            if (dbCommand != null)
                dbCommand.Dispose();
            dbCommand = null;
        }
        #endregion
    }
}
