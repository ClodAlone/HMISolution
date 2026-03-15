using System;
using System.Data.Common;

#if NET_STANDARD
namespace System.Data.Common
{
    public static class DbProviderFactories
    {
        //
        // Summary:
        //     Returns an instance of a System.Data.Common.DbProviderFactory.
        //
        // Parameters:
        //   providerInvariantName:
        //     Invariant name of a provider.
        //
        // Returns:
        //     An instance of a System.Data.Common.DbProviderFactory for a specified provider
        //     name.
        public static DbProviderFactory GetFactory(string providerInvariantName)
        {
            if (providerInvariantName == "System.Data.Odbc")
                return System.Data.Odbc.OdbcFactory.Instance;
            //else if (providerInvariantName == "System.Data.OracleClient")
            //    return System.Data.OracleClient.OracleClientFactory.Instance;
            else if (providerInvariantName == "System.Data.SqlClient")
                return System.Data.SqlClient.SqlClientFactory.Instance;
            else if (providerInvariantName == "MySql.Data.MySqlClient")
                return MySql.Data.MySqlClient.MySqlClientFactory.Instance;
            else if (providerInvariantName == "System.Data.SQLite")
                return System.Data.SQLite.SQLiteFactory.Instance;
            else if (providerInvariantName == "System.Data.SQLite")
                return System.Data.SQLite.SQLiteFactory.Instance;
            else if (providerInvariantName == "PostgreSQL.Data.PostgreSQLClient")
                return Npgsql.NpgsqlFactory.Instance;
            else
                throw new ArgumentException(String.Format("Unsupported provider name ('{0}') !", providerInvariantName));

        }
    }
}
#endif
