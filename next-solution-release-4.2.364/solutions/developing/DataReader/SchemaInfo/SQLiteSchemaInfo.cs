using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using DataReader.Extensions;
using System.Data;
using DevExpress.Xpo.DB.Helpers;
using System.Globalization;

namespace DataReader.SchemaInfo
{
    public class SQLiteSchemaInfo : DbSchemaInfo
    {
        #region Declarations

        public enum SQLiteType : int
        {
            Blob = 1,
            Byte = 2,
            Bool = 3,
            DateTime = 6,
            Decimal = 7,
            Double = 8,
            Guid = 9,
            Int16 = 10,
            Int32 = 11,
            Int64 = 12,
            Single = 15,
            String = 16
        };

        static readonly Dictionary<Type, SQLiteType> typeToSQLiteType = new Dictionary<Type, SQLiteType>()
        {
            { typeof(byte) , SQLiteType.Byte },
            { typeof(sbyte) , SQLiteType.Byte },
            { typeof(ushort) , SQLiteType.Int16 },
            { typeof(short) , SQLiteType.Int16 },
            { typeof(uint) , SQLiteType.Int32 },
            { typeof(int) , SQLiteType.Int32 },
            { typeof(ulong) , SQLiteType.Int64 },
            { typeof(long) , SQLiteType.Int64 },
            { typeof(float) , SQLiteType.Single },
            { typeof(double) , SQLiteType.Double },
            { typeof(decimal) , SQLiteType.Decimal },
            { typeof(bool) , SQLiteType.Bool },
            { typeof(string) , SQLiteType.String },
            { typeof(char) , SQLiteType.String },
            { typeof(Guid) , SQLiteType.Guid },
            { typeof(DateTime) , SQLiteType.DateTime },
            { typeof(DateTimeOffset) , SQLiteType.DateTime},
            { typeof(byte[]) , SQLiteType.Blob },
            { typeof(byte?) , SQLiteType.Byte },
            { typeof(sbyte?) , SQLiteType.Byte },
            { typeof(ushort?) , SQLiteType.Int16 },
            { typeof(short?) , SQLiteType.Int16 },
            { typeof(uint?) , SQLiteType.Int32 },
            { typeof(int?) , SQLiteType.Int32 },
            { typeof(ulong?) , SQLiteType.Int64 },
            { typeof(long?) , SQLiteType.Int64 },
            { typeof(float?) , SQLiteType.Single },
            { typeof(double?) , SQLiteType.Double },
            { typeof(decimal?) , SQLiteType.Decimal },
            { typeof(bool?) , SQLiteType.Bool },
            { typeof(char?) , SQLiteType.String },
            { typeof(Guid?) , SQLiteType.Guid },
            { typeof(DateTime?) , SQLiteType.DateTime },
            { typeof(DateTimeOffset?) , SQLiteType.DateTime },
#if !NET_STANDARD
            { typeof(System.Data.Linq.Binary) , SQLiteType.Blob }
#endif
        };

        static readonly Dictionary<SQLiteType, SQLiteType> compatibleSQLiteType = new Dictionary<SQLiteType, SQLiteType>()
        {
            { SQLiteType.Byte , SQLiteType.Int16 },
            { SQLiteType.Int16 , SQLiteType.Int32 },
            { SQLiteType.Int32 , SQLiteType.Int64 },
            { SQLiteType.Int64 , SQLiteType.Decimal }
        };

        static readonly string DataBaseKeyword = "Data Source";
        #endregion

        #region Constructors
        public SQLiteSchemaInfo(string dataProviderName, string connectionString) : 
            base(dataProviderName, connectionString)
        {
        }
        #endregion

        #region Methods
        int GetSQLiteType(Type type)
        {
            if (!typeToSQLiteType.ContainsKey(type))
                throw new ArgumentException(String.Format("Unsupported data type value '{0}' for provider {1}!", type.FullName, DataProviderName));

            var dbType = typeToSQLiteType[type];
            if (IsTypeMatch(type, (int)dbType))
                return (int)dbType;

            dbType = GetCompatibleSQLiteType(dbType);
            if (ProviderTypeToNames.ContainsKey((int)dbType))
                return (int)dbType;

            return (int)dbType;
        }

        SQLiteType GetCompatibleSQLiteType(SQLiteType dbType)
        {
            while (compatibleSQLiteType.ContainsKey(dbType))
            {
                dbType = compatibleSQLiteType[dbType];
                if (ProviderTypeToNames.ContainsKey((int)dbType))
                    return dbType;
            }

            return dbType;
        }
        #endregion

        #region Public Static Methods
        public static void CreateDatabase(string connectionString)
        {
            XpoHelpers.XpoHelper.CreateDirectoryIfNotExists(connectionString);
            var helper = new ConnectionStringParser(connectionString);
            if (helper.PartExists(DataBaseKeyword))
            {
                var dbName = helper.GetPartByName(DataBaseKeyword);
                var directoryName = System.IO.Path.GetDirectoryName(dbName);
                if (String.IsNullOrEmpty(directoryName))
                    dbName = String.Format("{0}{1}{2}", System.IO.Directory.GetCurrentDirectory(), System.IO.Path.DirectorySeparatorChar, dbName);
                if (!System.IO.File.Exists(dbName))
                {
                    using (var connection = new System.Data.SQLite.SQLiteConnection(connectionString))
                    {
                        connection.Open();
                    }
                }
            }
        }
        #endregion

        #region Overrides
        protected override int GetDbType(Type type)
        {
            return (int)GetSQLiteType(type);
        }

        protected override string FormatDefaultValue(Type type, object value)
        {
            var sqlDbType = GetSQLiteType(type);
            if (sqlDbType == (int)SQLiteType.DateTime)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            }
            else
            {
                var info = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
                return String.Format(info, "{0}", TypeExtensions.ChangeType(value, type, force: true));
            }
        }

        protected override string GetAutoIncrementKeyword(long seed, long step)
        {
            return "AUTOINCREMENT";
        }

        public override string FormatParameterName(string parameterName)
        {
            return string.Format("@{0}", parameterName.Replace(' ', '_'));
        }

        public override string GetTableName(DataRow schemaRow, bool useSchemaQuotes)
        {
            //if (useSchemaQuotes)
            //{
            //    return String.Format("{0}.{1}",
            //        WrapObjectName((string)schemaRow["table_schema"]),
            //        WrapObjectName((string)schemaRow["table_name"]));
            //}
            //else
                return (string)schemaRow["table_name"];
        }

        public override string GetViewName(DataRow schemaRow, bool useSchemaQuotes)
        {
            //if (useSchemaQuotes)
            //{
            //    return String.Format("{0}.{1}",
            //        WrapObjectName((string)schemaRow["table_schema"]),
            //        WrapObjectName((string)schemaRow["table_name"]));
            //}
            //else
                return (string)schemaRow["table_name"];
        }
        #endregion
    }
}
