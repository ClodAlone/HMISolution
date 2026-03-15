using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySql.Data.MySqlClient;
using DataReader.Extensions;
using System.Data;
using DevExpress.Xpo.DB.Helpers;

namespace DataReader.SchemaInfo
{
    public class MySQLSchemaInfo : DbSchemaInfo
    {
        #region Declarations
        static readonly Dictionary<Type, MySqlDbType> typeToMySQLType = new Dictionary<Type, MySqlDbType>()
        {
            { typeof(byte) , MySqlDbType.UByte },
            { typeof(sbyte) , MySqlDbType.Byte },
            { typeof(ushort) , MySqlDbType.UInt16 },
            { typeof(short) , MySqlDbType.Int16 },
            { typeof(uint) , MySqlDbType.UInt32 },
            { typeof(int) , MySqlDbType.Int32 },
            { typeof(ulong) , MySqlDbType.UInt64 },
            { typeof(long) , MySqlDbType.Int64 },
            { typeof(float) , MySqlDbType.Float },
            { typeof(double) , MySqlDbType.Double },
            { typeof(decimal) , MySqlDbType.Decimal },
            { typeof(bool) , MySqlDbType.Bit },
            { typeof(string) , MySqlDbType.VarChar},
            { typeof(char) , MySqlDbType.UByte },
            { typeof(Guid) , MySqlDbType.Guid },
            { typeof(DateTime) , MySqlDbType.DateTime },
            { typeof(DateTimeOffset) , MySqlDbType.Timestamp },
            { typeof(byte[]) , MySqlDbType.TinyBlob },
            { typeof(byte?) , MySqlDbType.UByte },
            { typeof(sbyte?) , MySqlDbType.Byte },
            { typeof(ushort?) , MySqlDbType.UInt16 },
            { typeof(short?) , MySqlDbType.Int16 },
            { typeof(uint?) , MySqlDbType.UInt32 },
            { typeof(int?) , MySqlDbType.Int32 },
            { typeof(ulong?) , MySqlDbType.UInt64 },
            { typeof(long?) , MySqlDbType.Int64 },
            { typeof(float?) , MySqlDbType.Float },
            { typeof(double?) , MySqlDbType.Double },
            { typeof(decimal?) , MySqlDbType.Decimal },
            { typeof(bool?) , MySqlDbType.Bit },
            { typeof(char?) , MySqlDbType.UByte },
            { typeof(Guid?) , MySqlDbType.Guid },
            { typeof(DateTime?) , MySqlDbType.DateTime },
            { typeof(DateTimeOffset?) , MySqlDbType.Timestamp },
#if !NET_STANDARD
            { typeof(System.Data.Linq.Binary) , MySqlDbType.Blob }
#endif
        };

        static readonly Dictionary<MySqlDbType, MySqlDbType> compatibleMySQLType = new Dictionary<MySqlDbType, MySqlDbType>()
        {
            { MySqlDbType.DateTime , MySqlDbType.Timestamp },
            { MySqlDbType.Guid , MySqlDbType.String },
        };

        static readonly string DataBaseKeyword = "Database";
        #endregion

        #region Constructors
        public MySQLSchemaInfo(string dataProviderName, string connectionString) : 
            base(dataProviderName, connectionString)
        {
        }
        #endregion

        #region Methods
        MySqlDbType GetMySQLType(Type type)
        {
            if (!typeToMySQLType.ContainsKey(type))
                throw new ArgumentException(String.Format("Unsupported data type value '{0}' for provider {1}!", type.FullName, DataProviderName));

            var dbType = typeToMySQLType[type];
            if (IsTypeMatch(type, (int)dbType))
                return dbType;

            dbType = GetCompatibleMySQLDbType(dbType);
            if (ProviderTypeToNames.ContainsKey((int)dbType))
                return dbType;

            //dbType = MySqlDbType.NVarChar;
            if (ProviderTypeToNames.ContainsKey((int)dbType))
                return dbType;
            return GetCompatibleMySQLDbType(dbType);
        }

        MySqlDbType GetCompatibleMySQLDbType(MySqlDbType dbType)
        {
            while (compatibleMySQLType.ContainsKey(dbType))
            {
                dbType = compatibleMySQLType[dbType];
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
            if (helper.PartExists(DataBaseKeyword))
            {
                var dbName = helper.GetPartByName(DataBaseKeyword);
                helper.RemovePartByName(DataBaseKeyword);
                var cleanConnString = helper.GetConnectionString();
                using (var connection = new MySql.Data.MySqlClient.MySqlConnection(cleanConnString))
                {
                    connection.Open();
                    using (MySqlCommand cmd = connection.CreateCommand())
                    {
                        cmd.CommandText = String.Format("CREATE DATABASE `{0}`", dbName);
                        cmd.ExecuteNonQuery();
                    }
                }

                //var waitTime = Properties.Settings.Default.SQLServerCatalogUpdateTime;
                //if (waitTime > 0)
                //    System.Threading.Thread.Sleep(waitTime);
            }
        }
        #endregion

        #region Overrides
        protected override int GetDbType(Type type)
        {
            return (int)GetMySQLType(type);
        }

        protected override string LiteralPrefix(string typeName)
        {
            var prefix = base.LiteralPrefix(typeName);
            if (String.IsNullOrEmpty(prefix) && GetProviderDataType(typeName) == typeof(string))
               return Properties.Settings.Default.MySQLLiteralPrefixForStringType;
            else
                return prefix;
        }

        protected override string LiteralSuffix(string typeName)
        {
            var prefix = base.LiteralSuffix(typeName);
            if (String.IsNullOrEmpty(prefix) && GetProviderDataType(typeName) == typeof(string))
                return Properties.Settings.Default.MySQLLiteralSuffixForStringType;
            else
                return prefix;
        }

        protected override string FormatDefaultValue(Type type, object value)
        {
            var sqlDbType = GetMySQLType(type);
            if (sqlDbType == MySqlDbType.DateTime || sqlDbType == MySqlDbType.Timestamp)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("yyyyMMddHHmmss"));
            }
            else
            {
                var info = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
                return String.Format(info, "{0}", TypeExtensions.ChangeType(value, type, force: true));
            }
        }

        protected override string GetAutoIncrementKeyword(long seed, long step)
        {
            return "AUTO_INCREMENT";
        }

        public override Type GetProviderDataType(Type type)
        {
            var netType = base.GetProviderDataType(type);
            if (type.IsUnsigned() && !netType.IsUnsigned())
                return TypeExtensions.ConvertSignedOrUnsigned(netType);

            return netType;
        }

        public override string FormatDataTypeName(DataColumn column)
        {
            var dbType = GetMySQLType(column.DataType);
            if (dbType == MySqlDbType.String && column.DataType == typeof(Guid))
            {
                var typeName = GetProviderTypeName(column.DataType);
                return string.Format("{0}(36)", typeName);
            }
            else if (column.DataType.IsUnsigned())
            {
                var typeName = base.FormatDataTypeName(column);
                if (!typeName.Contains("UNSIGNED"))
                    return String.Format("{0} {1}", typeName, "UNSIGNED");
                else
                    return typeName;
            }
            else if (column.DataType == typeof(DateTime)) 
                return String.Format("{0}(6)", base.FormatDataTypeName(column));    //In order to have column with size equal to 6
                                                                                    //to represent also the milliseconds
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
                if(schemaRow.Table.Columns.Contains("OWNER"))
                    return String.Format("{0}.{1}",
                        WrapObjectName((string)schemaRow["OWNER"]),
                        WrapObjectName((string)schemaRow["TABLE_NAME"]));
                else if (schemaRow.Table.Columns.Contains("TABLE_SCHEMA"))
                    return String.Format("{0}.{1}",
                    WrapObjectName((string)schemaRow["TABLE_SCHEMA"]),
                    WrapObjectName((string)schemaRow["TABLE_NAME"]));
                else
                    return (string)schemaRow["TABLE_NAME"];
            }
            else
                return (string)schemaRow["TABLE_NAME"];
        }

        public override string GetViewName(DataRow schemaRow, bool useSchemaQuotes)
        {
            if (useSchemaQuotes)
            {
                return String.Format("{0}.{1}",
                    WrapObjectName((string)schemaRow["OWNER"]),
                    WrapObjectName((string)schemaRow["VIEW_NAME"]));
            }
            else
                return (string)schemaRow["VIEW_NAME"];
        }

        public override bool AreCompatibleDataTypes(Type type1, Type type2)
        {
            if (!base.AreCompatibleDataTypes(type1, type2))
            {
                var sqlDbType1 = GetMySQLType(type1);
                var sqlDbType2 = GetMySQLType(type2);
                return (sqlDbType1 == MySqlDbType.Bit && sqlDbType2 == MySqlDbType.UInt64 ||
                    sqlDbType2 == MySqlDbType.Bit && sqlDbType1 == MySqlDbType.UInt64);
            }

            return true;
        }
        #endregion
    }
}
