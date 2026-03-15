using System;
using DataReader.SchemaInfo;
#if !NET_STANDARD
using Microsoft.Data.ConnectionUI;
#endif

namespace DataReader.SchemaInfo
{
    public static class DbSchemaInfoFactory
    {
        public static DbSchemaInfo CreateSchemaInfo(string providerInvariantName, string connectionString)
        {
#if !NET_STANDARD
            if (providerInvariantName == DataProvider.OdbcDataProvider.Name)
                return new OdbcSchemaInfo(providerInvariantName, connectionString);
            else if (providerInvariantName == DataProvider.OleDBDataProvider.Name)
                return new OledDbSchemaInfo(providerInvariantName, connectionString);
            else if (providerInvariantName == DataProvider.OracleDataProvider.Name)
                return new OracleSchemaInfo(providerInvariantName, connectionString);
            else if (providerInvariantName == DataProvider.SqlDataProvider.Name)
                return new MsSqlSchemaInfo(providerInvariantName, connectionString);
            else if (providerInvariantName == SqlCe.SqlCeDataProvider.Name)
                return new MsSqlCeSchemaInfo(providerInvariantName, connectionString);
            else if (providerInvariantName == DataProvider.MySQLDataProvider.Name)
                return new MySQLSchemaInfo(providerInvariantName, connectionString);
            else if (providerInvariantName == DataProvider.SQLiteDataProvider.Name)
                return new SQLiteSchemaInfo(providerInvariantName, connectionString);

#else
            if (providerInvariantName == "System.Data.Odbc")
                return new OdbcSchemaInfo(providerInvariantName, connectionString);
            //else if (providerInvariantName == "System.Data.OracleClient")
            //    return new OracleSchemaInfo(providerInvariantName, connectionString);
            else if (providerInvariantName == "System.Data.SqlClient")
                return new MsSqlSchemaInfo(providerInvariantName, connectionString);
            else if (providerInvariantName == "MySql.Data.MySqlClient")
                return new MySQLSchemaInfo(providerInvariantName, connectionString);
            else if (providerInvariantName == "System.Data.SQLite")
                return new SQLiteSchemaInfo(providerInvariantName, connectionString);
#endif
            else
                throw new ArgumentException(String.Format("Unsupported provider name ('{0}') !", providerInvariantName));
        }

        public static bool TryCreateSchemaInfo(string providerInvariantName, string connectionString, out DbSchemaInfo dbSchemaInfo)
        {
            try
            {
                dbSchemaInfo = CreateSchemaInfo(providerInvariantName, connectionString);
            }
            catch
            {
                dbSchemaInfo = null;
            }

            return dbSchemaInfo != null;
        }

        public static void CreateDatabase(string providerInvariantName, string connectionString)
        {
#if !NET_STANDARD
            if (providerInvariantName == DataProvider.SqlDataProvider.Name)
                MsSqlSchemaInfo.CreateDatabase(connectionString);
            else if (providerInvariantName == SqlCe.SqlCeDataProvider.Name)
                MsSqlCeSchemaInfo.CreateDatabase(connectionString);
            else if (providerInvariantName == DataProvider.MySQLDataProvider.Name)
                MySQLSchemaInfo.CreateDatabase(connectionString);
            else if (providerInvariantName == DataProvider.SQLiteDataProvider.Name)
                SQLiteSchemaInfo.CreateDatabase(connectionString);
#else
            if (providerInvariantName == "System.Data.SqlClient")
                MsSqlSchemaInfo.CreateDatabase(connectionString);
            else if (providerInvariantName == "MySql.Data.MySqlClient")
                MySQLSchemaInfo.CreateDatabase(connectionString);
            else if (providerInvariantName == "System.Data.SQLite")
                SQLiteSchemaInfo.CreateDatabase(connectionString);
#endif
            else
                throw new NotImplementedException(String.Format("CreateDatabase not implemented for provider name '{0}' !", providerInvariantName));
        }

        public static void CreateBackup(string providerInvariantName, string connectionString)
        {
#if !NET_STANDARD
            if (providerInvariantName == DataProvider.SqlDataProvider.Name)
                MsSqlSchemaInfo.CreateBackup(connectionString);
            //else if (providerInvariantName == SqlCe.SqlCeDataProvider.Name)
            //    MsSqlCeSchemaInfo.CreateDatabase(connectionString);
            //else if (providerInvariantName == DataProvider.MySQLDataProvider.Name)
            //    MySQLSchemaInfo.CreateDatabase(connectionString);
#else
            if (providerInvariantName == "System.Data.SqlClient")
                MsSqlSchemaInfo.CreateBackup(connectionString);
#endif
            else
                throw new NotImplementedException(String.Format("CreateBackup not implemented for provider name '{0}' !", providerInvariantName));
        }

        public static void ApplyCFR21Requirements(string providerInvariantName, string connectionString)
        {
#if !NET_STANDARD
            if (providerInvariantName == DataProvider.SqlDataProvider.Name)
                MsSqlSchemaInfo.ApplyCFR21Requirements(connectionString);
#else
            if (providerInvariantName == "System.Data.SqlClient")
                MsSqlSchemaInfo.ApplyCFR21Requirements(connectionString);
#endif
        }

        public static string EnsureTrustedConnectionStrings(string providerInvariantName, string connectionString)
        {
#if !NET_STANDARD
            if (providerInvariantName == DataProvider.SqlDataProvider.Name)
                return MsSqlSchemaInfo.EnsureTrustedConnectionStrings(connectionString);
#else
            if (providerInvariantName == "System.Data.SqlClient")
                return MsSqlSchemaInfo.EnsureTrustedConnectionStrings(connectionString);
#endif

            return connectionString;
        }
    }
}
