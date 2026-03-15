using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;
using System.Dynamic;
using System.Data;
using System.Xml.Linq;
using System.Data.Common;
using System.Threading;
using log4net;
using DataReader.SchemaInfo;

namespace DataReader
{
    public class DataReader
    {
#if !NET_STANDARD
        private static readonly ILog logDataReader = LogManager.GetLogger(Properties.Resources.DataReader);
#else
        private static readonly ILog logDataReader = LogManager.GetLogger(System.Reflection.Assembly.GetEntryAssembly(), Properties.Resources.DataReader);
#endif

#if !NET_STANDARD
        public static IEnumerable<String> GetProviderFactoryClasses()
        {
            // Retrieve the installed providers and factories.
            using (DataTable table = DbProviderFactories.GetFactoryClasses())
            {
                // Display each row and column value.
                foreach (DataRow row in table.Rows)
                {
                    foreach (DataColumn column in table.Columns)
                    {
                        yield return row[column].ToString();
                    }
                }
            }
        }
#endif

        // Given a provider name and connection string, 
        // create the DbProviderFactory and DbConnection.
        // Returns a DbConnection on success; null on failure.
        public static DbConnection CreateDbConnection(
            string providerName, string connectionString)
        {
            // Assume failure.
            DbConnection connection = null;

            // Create the DbProviderFactory and DbConnection.
            if (connectionString != null)
            {
                try
                {
                    DbProviderFactory factory = null;
                    factory = DbProviderFactories.GetFactory(providerName);

                    connection = factory.CreateConnection();
                    connection.ConnectionString = connectionString;
                }
                catch (Exception ex)
                {
                    // Set the connection to null if it was created.
                    if (connection != null)
                    {
                        connection.Dispose();
                        connection = null;
                    }
                    logDataReader.Error(String.Format(Properties.Resources.ErrorOpening, connectionString, ex.Message), ex);
                    throw;
                }
            }
            // Return the connection.
            return connection;
        }

        public static DbDataAdapter CreateDbDataAdapter(
            string providerName)
        {
            // Assume failure.
            // Create the DbProviderFactory and DbConnection.
            if (providerName != null)
            {
                try
                {
                    DbProviderFactory factory = null;
                    factory = DbProviderFactories.GetFactory(providerName);

                    return factory.CreateDataAdapter();
                }
                catch (Exception ex)
                {
                    logDataReader.Error(String.Format(Properties.Resources.ErrorOpening, providerName, ex.Message), ex);
                }
            }
            // Return the connection.
            return null;
        }

        public static DbCommand CreateDbCommand(
            string providerName)
        {
            // Assume failure.
            // Create the DbProviderFactory and DbConnection.
            if (providerName != null)
            {
                try
                {
                    DbProviderFactory factory = null;
                    factory = DbProviderFactories.GetFactory(providerName);

                    return factory.CreateCommand();
                }
                catch (Exception ex)
                {
                    logDataReader.Error(String.Format(Properties.Resources.ErrorOpening, providerName, ex.Message), ex);
                }
            }
            // Return the connection.
            return null;
        }

        public static DbCommandBuilder CreateDbCommandBuilder(
            string providerName)
        {
            // Assume failure.
            // Create the DbProviderFactory and DbConnection.
            if (providerName != null)
            {
                try
                {
                    DbProviderFactory factory = null;
                    factory = DbProviderFactories.GetFactory(providerName);

                    return factory.CreateCommandBuilder();
                }
                catch (Exception ex)
                {
                    logDataReader.Error(String.Format(Properties.Resources.ErrorOpening, providerName, ex.Message), ex);
                }
            }
            // Return the connection.
            return null;
        }

        public static DbParameter CreateDbParameter(
            string providerName)
        {
            // Assume failure.
            // Create the DbProviderFactory and DbConnection.
            if (providerName != null)
            {
                try
                {
                    DbProviderFactory factory = null;
                    factory = DbProviderFactories.GetFactory(providerName);

                    return factory.CreateParameter();
                }
                catch (Exception ex)
                {
                    logDataReader.Error(String.Format(Properties.Resources.ErrorOpening, providerName, ex.Message), ex);
                }
            }
            // Return the connection.
            return null;
        }

        public static IEnumerable<string> ListTables(string dataprovider, string connectionstring, bool useSchemaQuotes = false)
        {
            using (var dbSchemaInfo = SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(dataprovider, connectionstring))
            {
                using (var connection = CreateDbConnection(dataprovider, connectionstring))
                {
                    connection.Open();
                    var dt = connection.GetSchema("Tables");
                    foreach (DataRow row in dt.Rows.AsParallel())
                    {
                        string tablename = dbSchemaInfo.GetTableName(row, useSchemaQuotes);
                        if (tablename == null)
                            continue;
                        yield return tablename;
                    }
                }
            }
        }

        public static IEnumerable<string> ListViews(string dataprovider, string connectionstring, bool useSchemaQuotes = false)
        {
            using (var dbSchemaInfo = SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(dataprovider, connectionstring))
            {
                using (var connection = CreateDbConnection(dataprovider, connectionstring))
                {
                    connection.Open();
                    var dt = connection.GetSchema("Views");
                    foreach (DataRow row in dt.Rows.AsParallel())
                    {
                        string tablename = dbSchemaInfo.GetViewName(row, useSchemaQuotes);
                        if (tablename == null)
                            continue;
                        yield return tablename;
                    }
                }
            }
        }

        public static IEnumerable<string> ListDataTypes(string dataprovider, string connectionstring)
        {
            using (var connection = CreateDbConnection(dataprovider, connectionstring))
            {
                connection.Open();
                var dt = connection.GetSchema("DataTypes");
                foreach (DataRow row in dt.Rows.AsParallel())
                {
                    string typename = (string)row["TypeName"];
                    yield return typename;
                }
            }
        }

        public static IDictionary<string, Type> ListColumns(string dataprovider, string connectionstring, string sql, string groupby = null, bool useSchemaQuotes = false)
        {
            using (var dbSchemaInfo = SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(dataprovider, connectionstring))
            {
                using (var connection = CreateDbConnection(dataprovider, connectionstring))
                {
                    connection.Open();
                    var columns = new Dictionary<string, Type>();

                    var dbdapater = CreateDbDataAdapter(dataprovider);
                    dbdapater.SelectCommand = CreateDbCommand(dataprovider);
                    dbdapater.SelectCommand.Connection = connection;
                    dbdapater.SelectCommand.CommandText = sql;
                    if (!String.IsNullOrEmpty(groupby))
                        dbdapater.SelectCommand.CommandText = String.Format("{0} group by {1}", dbdapater.SelectCommand.CommandText, groupby);

                    var ds = new DataSet();
                    try
                    {
                        dbdapater.Fill(ds);

                        for (var iCol = 0; iCol < ds.Tables[0].Columns.Count; iCol++)
                        {
                            var columnName = ds.Tables[0].Columns[iCol].ColumnName;
                            if (useSchemaQuotes)
                                columnName = dbSchemaInfo.WrapObjectName(columnName);
                            columns.Add(columnName, ds.Tables[0].Columns[iCol].DataType);
                        }
                    }
                    catch (Exception ex)
                    {
                        logDataReader.Error(String.Format(Properties.Resources.ErrorExecuting, sql, ex.Message), ex);
                    }

                    return columns;
                }
            }
        }

        public static IEnumerable<dynamic> GetSchemaInfo(string dataprovider, string connectionstring, string collectionName, string[] restrictionValues = null)
        {
            using (var connection = CreateDbConnection(dataprovider, connectionstring))
            {
                connection.Open();
                DataTable dt = null;
                if(restrictionValues != null)
                    dt = connection.GetSchema(collectionName, restrictionValues);
                else
                    dt = connection.GetSchema(collectionName);

                foreach (DataRow row in dt.Rows.AsParallel())
                {
                    yield return SqlDataRowToExpando(row);
                }
            }
        }

        public static dynamic SqlDataRowToExpando(DataRow row)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (var ii = 0; ii < row.Table.Columns.Count; ii++)
                expandoObject.Add(row.Table.Columns[ii].ColumnName, row[ii]);

            return expandoObject;
        }

        private static dynamic SqlDataReaderToExpando(DbDataReader reader)
        {
            var expandoObject = new ExpandoObject() as IDictionary<string, object>;

            for (var i = 0; i < reader.FieldCount; i++)
                expandoObject.Add(reader.GetName(i), reader[i]);

            return expandoObject;
        }

        public static DataTable GetDataSetSqlData(string dataprovider,
                                                    string connectionstring,
                                                    string select,
                                                    string where = null,
                                                    string groupby = null,
                                                    string sort = null,
                                                    DataSchemaType schema = DataSchemaType.None)
        {
            using (var connection = CreateDbConnection(dataprovider, connectionstring))
            {
                connection.Open();
                var dbdapater = CreateDbDataAdapter(dataprovider);
                dbdapater.SelectCommand = CreateDbCommand(dataprovider);
                dbdapater.SelectCommand.Connection = connection;
                if (!string.IsNullOrEmpty(where))
                    dbdapater.SelectCommand.CommandText = String.Format("{0} where {1}", select, where);
                else
                    dbdapater.SelectCommand.CommandText = select;
                if (!string.IsNullOrEmpty(groupby))
                    dbdapater.SelectCommand.CommandText = String.Format("{0} group by {1}", dbdapater.SelectCommand.CommandText, groupby);
                if (!string.IsNullOrEmpty(sort))
                    dbdapater.SelectCommand.CommandText = String.Format("{0} order by {1}", dbdapater.SelectCommand.CommandText, sort);

                var ret = new DataSet();
                try
                {
                    if (schema == DataSchemaType.Mapped || schema == DataSchemaType.MappedOnlySchema)
                        dbdapater.FillSchema(ret, SchemaType.Mapped);
                    else if (schema == DataSchemaType.Source || schema == DataSchemaType.SourceOnlySchema)
                        dbdapater.FillSchema(ret, SchemaType.Source);
                    if (schema != DataSchemaType.MappedOnlySchema && schema != DataSchemaType.SourceOnlySchema)
                        dbdapater.Fill(ret);
                }
                catch (Exception ex)
                {
                    logDataReader.Error(String.Format(Properties.Resources.ErrorExecuting, select, ex.Message), ex);
                }

                if (ret.Tables.Count > 0)
                    return ret.Tables[0];
                return null;
            }
        }

        public static DataTable ReadSqlData(string dataprovider,
                                            string connectionstring,
                                            string select,
                                            string where = null,
                                            string groupby = null,
                                            string sort = null,
                                            CancellationTokenSource cts = null, int maxTake = 0)
        {
            using (var connection = CreateDbConnection(dataprovider, connectionstring))
            {
                connection.Open();
                var dbdapater = CreateDbDataAdapter(dataprovider);
                dbdapater.SelectCommand = CreateDbCommand(dataprovider);
                dbdapater.SelectCommand.Connection = connection;
                if (!string.IsNullOrEmpty(where))
                    dbdapater.SelectCommand.CommandText = String.Format("{0} where {1}", select, where);
                else
                    dbdapater.SelectCommand.CommandText = select;
                if (!string.IsNullOrEmpty(groupby))
                    dbdapater.SelectCommand.CommandText = String.Format("{0} group by {1}", dbdapater.SelectCommand.CommandText, groupby);
                if (!string.IsNullOrEmpty(sort))
                    dbdapater.SelectCommand.CommandText = String.Format("{0} order by {1}", dbdapater.SelectCommand.CommandText, sort);

                var ret = new DataSet();
                dbdapater.FillSchema(ret, SchemaType.Source);

                return DataTableFromDataSet(ret, dbdapater, maxTake, cts);
            }
        }

        public static DataTable DataTableFromDataSet(DataSet ret, DbDataAdapter dbdapater, int maxTake = 0, CancellationTokenSource cts = null)
        {
            if (ret.Tables.Count > 0)
            {
                int nCount = 0;
                using (var reader = dbdapater.SelectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (cts != null)
                            cts.Token.ThrowIfCancellationRequested();

                        if (maxTake > 0 && ++nCount > maxTake)
                            break;

                        var row = ret.Tables[0].NewRow();
                        row.BeginEdit();
                        for (var ii = 0; ii < reader.FieldCount; ii++)
                            row[reader.GetName(ii)] = reader[ii];
                        row.EndEdit();
                        ret.Tables[0].Rows.Add(row);
                    }
                    ret.Tables[0].AcceptChanges();
                }
            }

            if (ret.Tables.Count > 0)
                return ret.Tables[0];
            return null;
        }

        public static IEnumerable<dynamic> GetDynamicSqlData(string dataprovider,
                                                              string connectionstring,
                                                              string select,
                                                              string where = null,
                                                              string groupby = null,
                                                              string sort = null, 
                                                              CancellationTokenSource cts = null, int maxTake = 0)
        {
            using (var connection = CreateDbConnection(dataprovider, connectionstring))
            {
                connection.Open();
                var dbdapater = CreateDbDataAdapter(dataprovider);
                dbdapater.SelectCommand = CreateDbCommand(dataprovider);
                dbdapater.SelectCommand.Connection = connection;
                if (!string.IsNullOrEmpty(where))
                    dbdapater.SelectCommand.CommandText = String.Format("{0} where {1}", select, where);
                else
                    dbdapater.SelectCommand.CommandText = select;
                if (!string.IsNullOrEmpty(groupby))
                    dbdapater.SelectCommand.CommandText = String.Format("{0} group by {1}", dbdapater.SelectCommand.CommandText, groupby);
                if (!string.IsNullOrEmpty(sort))
                    dbdapater.SelectCommand.CommandText = String.Format("{0} order by {1}", dbdapater.SelectCommand.CommandText, sort);

                int nCount = 0;
                using (var reader = dbdapater.SelectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (cts != null)
                            cts.Token.ThrowIfCancellationRequested();

                        if (maxTake > 0 && ++nCount > maxTake)
                            break;

                        yield return SqlDataReaderToExpando(reader);
                    }
                }
            }
        }
    }
}
