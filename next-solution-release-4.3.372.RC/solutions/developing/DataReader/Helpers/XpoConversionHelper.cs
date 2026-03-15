using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.DB.Helpers;
#if !NET_STANDARD
using Microsoft.Data.ConnectionUI;
#endif

namespace DataReader.Helpers
{
    public class XpoConversionHelper
    {
        #region Declarations
        const string CatalogSourceHeader = "initial catalog";
        const string PostgreSQLEncondingDataProvidrerHeader = "Client Encoding";
        const string PostgreSQLEncondingXpoProvidrerHeader = "Encoding";

        const string DataProviderNetHeader = "DataProvider";

        static readonly Dictionary<String, String> XpoToNetProvider = new Dictionary<String, String>()
        {
#if !NET_STANDARD
            { AccessConnectionProvider.XpoProviderTypeString , DataProvider.OleDBDataProvider.Name },
            { MSSqlConnectionProvider.XpoProviderTypeString , DataProvider.SqlDataProvider.Name },
            { MSSqlCEConnectionProvider.XpoProviderTypeString , SqlCe.SqlCeDataProvider.Name },
            { OracleConnectionProvider.XpoProviderTypeString , DataProvider.OracleDataProvider.Name },
            { MySqlConnectionProvider.XpoProviderTypeString , DataProvider.MySQLDataProvider.Name },
            { SQLiteConnectionProvider.XpoProviderTypeString, DataProvider.SQLiteDataProvider.Name },
            { PostgreSqlConnectionProvider.XpoProviderTypeString, DataProvider.PostgreSQLDataProvider.Name }
#else
            { MSSqlConnectionProvider.XpoProviderTypeString , "System.Data.SqlClient" },
            { ODPManagedConnectionProvider.XpoProviderTypeString , "System.Data.OracleClient" },
            { MySqlConnectionProvider.XpoProviderTypeString , "MySql.Data.MySqlClient" },
            { SQLiteConnectionProvider.XpoProviderTypeString, "System.Data.SQLite" },
            { PostgreSqlConnectionProvider.XpoProviderTypeString, "PostgreSQL.Data.PostgreSQLClient" }
#endif
        };

        static readonly Dictionary<String, String> NetToXpoProvider = new Dictionary<String, String>()
        {
#if !NET_STANDARD
            { DataProvider.OleDBDataProvider.Name , AccessConnectionProvider.XpoProviderTypeString },
            { DataProvider.SqlDataProvider.Name , MSSqlConnectionProvider.XpoProviderTypeString },
            { SqlCe.SqlCeDataProvider.Name , MSSqlCEConnectionProvider.XpoProviderTypeString },
            { DataProvider.OracleDataProvider.Name , OracleConnectionProvider.XpoProviderTypeString },
            { DataProvider.MySQLDataProvider.Name , MySqlConnectionProvider.XpoProviderTypeString },
            { DataProvider.SQLiteDataProvider.Name, SQLiteConnectionProvider.XpoProviderTypeString },
            { DataProvider.PostgreSQLDataProvider.Name, PostgreSqlConnectionProvider.XpoProviderTypeString }
#else
            { "System.Data.SqlClient" , MSSqlConnectionProvider.XpoProviderTypeString },
            { "System.Data.OracleClient" , ODPManagedConnectionProvider.XpoProviderTypeString },
            { "MySql.Data.MySqlClient" , MySqlConnectionProvider.XpoProviderTypeString },
            { "System.Data.SQLite", SQLiteConnectionProvider.XpoProviderTypeString },
            { "PostgreSQL.Data.PostgreSQLClient", PostgreSqlConnectionProvider.XpoProviderTypeString }
#endif
        };
#endregion

#region Convert from Xpo to .Net
        public static bool IsDotNetConvertible(string conn)
        {
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                return XpoToNetProvider.ContainsKey(providerType);
            }
            catch
            { }

            return false;
        }

        public static String GetDataProviderFromXpoConnection(String conn)
        {
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (XpoToNetProvider.ContainsKey(providerType))
                {
                    return XpoToNetProvider[providerType];
                }
            }
            catch (Exception ex)
            {
            }

            return String.Empty;
        }

        public static String GetConnectionStringFromXpoConnection(String conn)
        {
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataStoreBase.XpoProviderTypeParameterName);
                if (XpoToNetProvider.ContainsKey(providerType))
                {
                    helper.RemovePartByName(DataStoreBase.XpoProviderTypeParameterName);
                    if (providerType == PostgreSqlConnectionProvider.XpoProviderTypeString
                        && helper.PartExists(PostgreSQLEncondingXpoProvidrerHeader))
                    {
                        var encondig = helper.GetPartByName(PostgreSQLEncondingXpoProvidrerHeader);
                        helper.RemovePartByName(PostgreSQLEncondingXpoProvidrerHeader);
                        helper.AddPart(PostgreSQLEncondingDataProvidrerHeader, encondig);
                    }
                    return helper.GetConnectionString();
                }
            }
            catch (Exception ex)
            {
            }

            return String.Empty;
        }
#endregion

#region Convert from .Net to Xpo
        public static bool IsXpoConvertible(string conn)
        {
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataProviderNetHeader);
                return NetToXpoProvider.ContainsKey(providerType);
            }
            catch
            { }

            return false;
        }

        public static String GetXpoProviderFromNetConnection(String conn)
        {
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataProviderNetHeader);
                if (NetToXpoProvider.ContainsKey(providerType))
                {
                    return NetToXpoProvider[providerType];
                }
            }
            catch
            { }

            return String.Empty;
        }

        public static String GetXpoConnectionStringFromNetConnection(String conn)
        {
            try
            {
                var helper = new ConnectionStringParser(conn);
                string providerType = helper.GetPartByName(DataProviderNetHeader);
                if (NetToXpoProvider.ContainsKey(providerType))
                {
                    helper.RemovePartByName(DataProviderNetHeader);
                    if (providerType ==
#if !NET_STANDARD
                        DataProvider.PostgreSQLDataProvider.Name
#else
                        "PostgreSQL.Data.PostgreSQLClient"
#endif
                        && helper.PartExists(PostgreSQLEncondingDataProvidrerHeader))
                    {
                        var encondig = helper.GetPartByName(PostgreSQLEncondingDataProvidrerHeader);
                        helper.RemovePartByName(PostgreSQLEncondingDataProvidrerHeader);
                        helper.AddPart(PostgreSQLEncondingXpoProvidrerHeader, encondig);
                    }
                    return helper.GetConnectionString();
                }
            }
            catch
            { }

            return String.Empty;
        }
#endregion

#region Generics
        public static String GetDataBaseName(String conn)
        {
            try
            {
                var helper = new ConnectionStringParser(conn);
                if (helper.PartExists(CatalogSourceHeader))
                    return helper.GetPartByName(CatalogSourceHeader);
            }
            catch
            { }

            return String.Empty;
        }

        public static String RemoveDataBaseName(String conn)
        {
            try
            {
                var helper = new ConnectionStringParser(conn);
                if (helper.PartExists(CatalogSourceHeader))
                    helper.RemovePartByName(CatalogSourceHeader);
                return helper.GetConnectionString();
            }
            catch
            { }

            return String.Empty;
        }
#endregion
    }
}
