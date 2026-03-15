using System;
using System.Collections.Generic;
using DataReader.Extensions;
using System.Data;
using DevExpress.Xpo.DB.Helpers;
using NpgsqlTypes;

namespace DataReader.SchemaInfo
{
    public class PostgreSQLSchemaInfo : DbSchemaInfo
    {
        #region Declarations
        static readonly Dictionary<Type, NpgsqlDbType> typeToPostgreSQLType = new Dictionary<Type, NpgsqlDbType>()
        {
            { typeof(byte) , NpgsqlDbType.Smallint },
            { typeof(sbyte) , NpgsqlDbType.Smallint },
            { typeof(ushort) , NpgsqlDbType.Integer },
            { typeof(short) , NpgsqlDbType.Smallint },
            { typeof(uint) , NpgsqlDbType.Bigint },
            { typeof(int) , NpgsqlDbType.Integer },
            { typeof(ulong) , NpgsqlDbType.Bigint },
            { typeof(long) , NpgsqlDbType.Bigint },
            { typeof(float) , NpgsqlDbType.Real },
            { typeof(double) , NpgsqlDbType.Double },
            { typeof(decimal) , NpgsqlDbType.Numeric },
            { typeof(bool) , NpgsqlDbType.Boolean },
            { typeof(string) , NpgsqlDbType.Text },
            { typeof(char) , NpgsqlDbType.Char },
            { typeof(Guid) , NpgsqlDbType.Uuid },
            { typeof(DateTime) , NpgsqlDbType.Timestamp },
            { typeof(DateTimeOffset) , NpgsqlDbType.Interval },
            { typeof(byte[]) , NpgsqlDbType.Bytea },
            { typeof(byte?) , NpgsqlDbType.Smallint },
            { typeof(sbyte?) , NpgsqlDbType.Smallint },
            { typeof(ushort?) , NpgsqlDbType.Integer },
            { typeof(short?) , NpgsqlDbType.Smallint },
            { typeof(uint?) , NpgsqlDbType.Bigint },
            { typeof(int?) , NpgsqlDbType.Integer },
            { typeof(ulong?) , NpgsqlDbType.Bigint },
            { typeof(long?) , NpgsqlDbType.Bigint },
            { typeof(float?) , NpgsqlDbType.Real },
            { typeof(double?) , NpgsqlDbType.Double },
            { typeof(decimal?) , NpgsqlDbType.Numeric },
            { typeof(bool?) , NpgsqlDbType.Boolean },
            { typeof(char?) , NpgsqlDbType.Char },
            { typeof(Guid?) , NpgsqlDbType.Uuid },
            { typeof(DateTime?) , NpgsqlDbType.Timestamp },
            { typeof(DateTimeOffset?) , NpgsqlDbType.Interval },
            { typeof(System.Collections.BitArray) , NpgsqlDbType.Bit },
#if !NET_STANDARD
            { typeof(System.Data.Linq.Binary) , NpgsqlDbType.Bytea }
#endif
        };

        static readonly Dictionary<NpgsqlDbType, NpgsqlDbType> compatiblePostgreSQLType = new Dictionary<NpgsqlDbType, NpgsqlDbType>()
        {
            { NpgsqlDbType.Varchar , NpgsqlDbType.Text }
        };

        static readonly string DataBaseKeyword = "Database";
        #endregion

        #region Constructors
        public PostgreSQLSchemaInfo(string dataProviderName, string connectionString) : 
            base(dataProviderName, connectionString)
        {
        }
        #endregion

        #region Methods
        NpgsqlDbType GetPostgreSQLType(Type type)
        {
            if (!typeToPostgreSQLType.ContainsKey(type))
                throw new ArgumentException(String.Format("Unsupported data type value '{0}' for provider {1}!", type.FullName, DataProviderName));

            var dbType = typeToPostgreSQLType[type];
            if (IsTypeMatch(type, (int)dbType))
                return dbType;

            dbType = GetCompatiblePostgreSQLDbType(dbType);
            if (ProviderTypeToNames.ContainsKey((int)dbType))
                return dbType;

            //dbType = PostgreSQLDbType.NVarChar;
            if (ProviderTypeToNames.ContainsKey((int)dbType))
                return dbType;
            return GetCompatiblePostgreSQLDbType(dbType);
        }

        NpgsqlDbType GetCompatiblePostgreSQLDbType(NpgsqlDbType dbType)
        {
            while (compatiblePostgreSQLType.ContainsKey(dbType))
            {
                dbType = compatiblePostgreSQLType[dbType];
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
                using (var connection = new Npgsql.NpgsqlConnection(cleanConnString))
                {
                    connection.Open();
                    using (var cmd = connection.CreateCommand())
                    {
                        try
                        {
                            cmd.CommandText = String.Format("CREATE DATABASE \"{0}\"", dbName);
                            cmd.ExecuteNonQuery();
                        }
                        catch (Npgsql.PostgresException) 
                        {
                            //CREATE DATABASE actually works by copying an existing database. By default, it copies the standard system database named template1.
                            //Please see https://www.postgresql.org/docs/current/manage-ag-templatedbs.html for more details.
                            cmd.CommandText = "SELECT pg_terminate_backend(pid) FROM pg_stat_activity WHERE datname = 'template1'";
                            cmd.ExecuteNonQuery();
                            cmd.CommandText = String.Format("CREATE DATABASE \"{0}\"", dbName);
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        #endregion

        #region Overrides
        protected override int GetDbType(Type type)
        {
            return (int)GetPostgreSQLType(type);
        }

        protected override string FormatDefaultValue(Type type, object value)
        {
            var sqlDbType = GetPostgreSQLType(type);
            if (sqlDbType == NpgsqlDbType.Timestamp)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("'{0}'", dateTime.ToString("yyyy-MM-dd HH:mm:ss"));
            }
            else
            {
                var info = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
                return String.Format(info, "{0}", TypeExtensions.ChangeType(value, type, force: true));
            }
        }

        protected override string GetAutoIncrementKeyword(long seed, long step)
        {
            return "SERIAL";
        }

        public override Type GetProviderDataType(Type type)
        {
            var sqlDbType = GetPostgreSQLType(type);
            if (sqlDbType == NpgsqlDbType.Bit)
                return typeof(System.Collections.BitArray);
            else
            return base.GetProviderDataType(type);
        }

        public override DbType GetProviderDbType(Type type)
        {
            var dbType = GetProviderDataType(type).ToDbType();
            if (dbType == DbType.DateTime)
                dbType = DbType.DateTime2;
            return dbType;
        }

        public override string FormatParameterName(string parameterName)
        {
            return string.Format("@{0}", parameterName.Replace(' ', '_'));
        }

        public override string FormatDataTypeName(DataColumn column)
        {
            var dbType = GetPostgreSQLType(column.DataType);
            if (dbType == NpgsqlDbType.Text)
                return GetProviderTypeName(column.DataType);

            return base.FormatDataTypeName(column);
        }

        protected override bool IsFixedPrecisionScale(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["IsFixedPrecisionAndScale"] is System.DBNull))
                return false;

            return bool.Parse(info["IsFixedPrecisionAndScale"].ToString());
        }

        #endregion
    }
}
