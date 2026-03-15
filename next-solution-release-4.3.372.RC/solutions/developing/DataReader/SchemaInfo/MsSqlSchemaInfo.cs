using DataReader.Extensions;
using DevExpress.Xpo.DB.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReader.SchemaInfo
{
    public class MsSqlSchemaInfo : DbSchemaInfo
    {
        #region Declarations
        static readonly Dictionary<Type, SqlDbType> typeToSqlDbType = new Dictionary<Type, SqlDbType>()
        {
            { typeof(byte) , SqlDbType.TinyInt },
            { typeof(sbyte) , SqlDbType.SmallInt },
            { typeof(ushort) , SqlDbType.Int },
            { typeof(short) , SqlDbType.SmallInt },
            { typeof(uint) , SqlDbType.BigInt },
            { typeof(int) , SqlDbType.Int },
            { typeof(ulong) , SqlDbType.BigInt },
            { typeof(long) , SqlDbType.BigInt },
            { typeof(float) , SqlDbType.Real },
            { typeof(double) , SqlDbType.Float },
            { typeof(decimal) , SqlDbType.Decimal },
            { typeof(bool) , SqlDbType.Bit },
            { typeof(string) , SqlDbType.NVarChar },
            { typeof(char) , SqlDbType.Char },
            { typeof(Guid) , SqlDbType.UniqueIdentifier },
            { typeof(DateTime) , SqlDbType.DateTime2 },
            { typeof(DateTimeOffset) , SqlDbType.DateTimeOffset },
            { typeof(byte[]) , SqlDbType.Binary },
            { typeof(byte?) , SqlDbType.TinyInt },
            { typeof(sbyte?) , SqlDbType.SmallInt },
            { typeof(ushort?) , SqlDbType.Int },
            { typeof(short?) , SqlDbType.SmallInt },
            { typeof(uint?) , SqlDbType.BigInt },
            { typeof(int?) , SqlDbType.Int },
            { typeof(ulong?) , SqlDbType.BigInt },
            { typeof(long?) , SqlDbType.BigInt },
            { typeof(float?) , SqlDbType.Real },
            { typeof(double?) , SqlDbType.Float },
            { typeof(decimal?) , SqlDbType.Decimal },
            { typeof(bool?) , SqlDbType.Bit },
            { typeof(char?) , SqlDbType.Char },
            { typeof(Guid?) , SqlDbType.UniqueIdentifier },
            { typeof(DateTime?) , SqlDbType.DateTime2 },
            { typeof(DateTimeOffset?) , SqlDbType.DateTimeOffset },
#if !NET_STANDARD
            { typeof(System.Data.Linq.Binary) , SqlDbType.Binary },
#endif
            { typeof(TimeSpan) , SqlDbType.Time }
        };

        static readonly Dictionary<SqlDbType, SqlDbType> compatibleSqlDbType = new Dictionary<SqlDbType, SqlDbType>()
        {
            { SqlDbType.DateTime2 , SqlDbType.DateTime }
        };

        static readonly Dictionary<Type, Type> unsupportedNetType = new Dictionary<Type, Type>()
        {
            { typeof(sbyte) , typeof(byte) }
        };

        static readonly string AttachDbFilenameKeyword = "AttachDbFilename";
        static readonly string CatalogSourceKeyword = "Initial Catalog";
        static readonly string UserKeyword = "user id";
        static readonly string PasswordKeyword = "password";
        static readonly string TrustedKeyword = "integrated security";
        #endregion

        #region Constructors
        public MsSqlSchemaInfo(string dataProviderName, string connectionString) : 
            base(dataProviderName, connectionString)
        {
        }
        #endregion
        
        #region Methods
        SqlDbType GetSqlDbType(Type type)
        {
            if (!typeToSqlDbType.ContainsKey(type))
                throw new ArgumentException(String.Format("Unsupported data type value '{0}' for provider {1}!", type.FullName, DataProviderName));

            var dbType = typeToSqlDbType[type];
            if (IsTypeMatch(type, (int)dbType))
                return dbType;

            dbType = GetCompatibleSqlDbType(dbType);
            if (ProviderTypeToNames.ContainsKey((int)dbType))
                return dbType;

            dbType = SqlDbType.NVarChar;
            if (ProviderTypeToNames.ContainsKey((int)dbType))
                return dbType;
            return GetCompatibleSqlDbType(dbType);
        }

        SqlDbType GetCompatibleSqlDbType(SqlDbType dbType)
        {
            while (compatibleSqlDbType.ContainsKey(dbType))
            {
                dbType = compatibleSqlDbType[dbType];
                if (ProviderTypeToNames.ContainsKey((int)dbType))
                    return dbType;
            }

            return dbType;
        }
        #endregion

        #region Public Static Methods
        public static void CreateDatabase(string connectionString)
        {
            var helper = new ConnectionStringParser(connectionString);
            if (helper.PartExists(AttachDbFilenameKeyword))
            {
                var dbFileName = helper.GetPartByName(AttachDbFilenameKeyword);
                var directoryName = System.IO.Path.GetDirectoryName(dbFileName);
                if (String.IsNullOrEmpty(directoryName))
                    dbFileName = String.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), dbFileName);
                if (!System.IO.File.Exists(dbFileName))
                {
                    helper.RemovePartByName(AttachDbFilenameKeyword);

                    string dbName = dbFileName;
                    if (helper.PartExists(CatalogSourceKeyword))
                    {
                        dbName = helper.GetPartByName(CatalogSourceKeyword);
                        helper.RemovePartByName(CatalogSourceKeyword);
                    }

                    var cleanConnString = helper.GetConnectionString();
                    using (var connection = new SqlConnection(cleanConnString))
                    {
                        connection.Open();
                        using (SqlCommand cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = String.Format("CREATE DATABASE [{0}] ON (NAME = N'{0}', FILENAME = '{1}')", dbName, dbFileName);
                            cmd.ExecuteNonQuery();
                        }
                    }

                    var waitTime = Properties.Settings.Default.SQLServerCatalogUpdateTime;
                    if (waitTime > 0)
                        System.Threading.Thread.Sleep(waitTime);
                }
            }
            else if (helper.PartExists(CatalogSourceKeyword))
            {
                var dbName = helper.GetPartByName(CatalogSourceKeyword);
                helper.RemovePartByName(CatalogSourceKeyword);
                var cleanConnString = helper.GetConnectionString();
                using (var connection = new SqlConnection(cleanConnString))
                {
                    connection.Open();
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = String.Format("CREATE DATABASE [{0}]", dbName);
                        cmd.ExecuteNonQuery();
                    }
                }

                var waitTime = Properties.Settings.Default.SQLServerCatalogUpdateTime;
                if (waitTime > 0)
                    System.Threading.Thread.Sleep(waitTime);
            }
        }

        public static void CreateBackup(string connectionString)
        {
            var helper = new ConnectionStringParser(connectionString);
            if (helper.PartExists(CatalogSourceKeyword))
            {
                var dbName = helper.GetPartByName(CatalogSourceKeyword);
                helper.RemovePartByName(CatalogSourceKeyword);
                var cleanConnString = helper.GetConnectionString();
                using (var connection = new SqlConnection(cleanConnString))
                {
                    connection.Open();
                    string backupDirectory = null;
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = Properties.Settings.Default.MSSQLBackupDirectoryQuery;
                        using (var reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                                backupDirectory = reader["Data"].ToString();
                        }
                    }

                    if (backupDirectory != null)
                    {
                        using (SqlCommand cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = String.Format("BACKUP DATABASE [{0}] To DISK = N'{1}\\{0}.bak' WITH INIT, NAME = N'{0}-Full Database Backup'", dbName, backupDirectory);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public static void ApplyCFR21Requirements(string connectionString)
        {
            var helper = new ConnectionStringParser(connectionString);
            if (helper.PartExists(CatalogSourceKeyword))
            {
                var dbName = helper.GetPartByName(CatalogSourceKeyword);
                helper.RemovePartByName(CatalogSourceKeyword);
                var cleanConnString = helper.GetConnectionString();
                using (var connection = new SqlConnection(cleanConnString))
                {
                    connection.Open();

                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = String.Format("ALTER DATABASE [{0}] SET RECOVERY FULL", dbName);
                        cmd.ExecuteNonQuery();
                    }

                    bool isFullBackupExists = false;
                    using (SqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = String.Format(Properties.Settings.Default.MSSQLFullBackupExistsQuery, dbName);
                        isFullBackupExists = (int)cmd.ExecuteScalar() > 0;
                    }

                    if (!isFullBackupExists)
                    {
                        string backupDirectory = null;
                        using (SqlCommand cmd = connection.CreateCommand())
                        {
                            cmd.CommandText = Properties.Settings.Default.MSSQLBackupDirectoryQuery;
                            using (var reader = cmd.ExecuteReader())
                            {
                                if (reader.Read())
                                    backupDirectory = reader["Data"].ToString();
                            }
                        }

                        if (backupDirectory != null)
                        {
                            using (SqlCommand cmd = connection.CreateCommand())
                            {
                                cmd.CommandText = String.Format("BACKUP DATABASE [{0}] To DISK = N'{1}\\{0}.bak' WITH INIT, NAME = N'{0}-Full Database Backup'", dbName, backupDirectory);
                                cmd.ExecuteNonQuery();
                            }
                        }
                    }
                }
            }
        }

        public static string EnsureTrustedConnectionStrings(string connectionString)
        {
            try
            {
                var helper = new ConnectionStringParser(connectionString);
                if (!helper.PartExists(TrustedKeyword))
                {
                    helper.RemovePartByName(UserKeyword);
                    helper.RemovePartByName(PasswordKeyword);
                    helper.AddPart(TrustedKeyword, "SSPI");
                    return helper.GetConnectionString();
                }
            }
            catch
            { }

            return connectionString;
        }
        #endregion

        #region Overrides
        protected override int GetDbType(Type type)
        {
            return (int)GetSqlDbType(type);
        }

        protected override string FormatDefaultValue(Type type, object value)
        {
            var dbType = GetSqlDbType(type);
            if (dbType == SqlDbType.Bit)
            {
                bool result = (bool)TypeExtensions.ChangeType(value, typeof(bool), force: true);
                return String.Format("{0}", result ? 1 : 0);
            }
            else if (dbType == SqlDbType.Date)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            else if (dbType == SqlDbType.DateTime)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            }
            else if (dbType == SqlDbType.DateTime2)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
            }
            else if (dbType == SqlDbType.DateTimeOffset)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("yyyy-MM-dd HH:mm:ss K", CultureInfo.InvariantCulture));
            }
            else
            {
                var info = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
                return String.Format(info, "{0}", TypeExtensions.ChangeType(value, type, force: true));
            }
        }

        protected override string GetAutoIncrementKeyword(long seed, long step)
        {
            return string.Format("IDENTITY({0}, {1}) NOT FOR REPLICATION", seed, step);
        }

        public override Type GetProviderDataType(Type type)
        {
            var netType = base.GetProviderDataType(type);
            if (unsupportedNetType.ContainsKey(netType))
                return unsupportedNetType[netType];

            return netType;
        }

        public override string FormatDataTypeName(DataColumn column)
        {
            var dbType = GetSqlDbType(column.DataType);
            if (dbType == SqlDbType.NVarChar ||
                dbType == SqlDbType.VarChar ||
                dbType == SqlDbType.VarBinary)
            {
                var typeName = GetProviderTypeName(column.DataType);
                if (column.MaxLength > 0)
                    return string.Format("{0}({1})", typeName, column.MaxLength);
                else
                    return string.Format("{0}(MAX)", typeName);
            }

            return base.FormatDataTypeName(column);
        }

        public override string FormatParameterName(string parameterName)
        {
            return string.Format("@{0}", parameterName.Replace(' ', '_'));
        }

        public override string GetTableName(DataRow schemaRow, bool useSchemaQuotes)
        {
            if (useSchemaQuotes)
            {
                return String.Format("{0}.{1}",
                    WrapObjectName((string)schemaRow["table_schema"]),
                    WrapObjectName((string)schemaRow["table_name"]));
            }
            else
                return (string)schemaRow["table_name"];
        }

        public override string GetViewName(DataRow schemaRow, bool useSchemaQuotes)
        {
            if (useSchemaQuotes)
            {
                return String.Format("{0}.{1}",
                    WrapObjectName((string)schemaRow["view_schema"]),
                    WrapObjectName((string)schemaRow["view_name"]));
            }
            else
                return (string)schemaRow["view_name"];
        }
        #endregion
    }
}
