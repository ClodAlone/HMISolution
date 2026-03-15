using DataReader.Extensions;
using DevExpress.Xpo.DB.Helpers;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlServerCe;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReader.SchemaInfo
{
    public class MsSqlCeSchemaInfo : DbSchemaInfo
    {
        #region Declarations
        static readonly Dictionary<Type, OleDbType> typeToOleDbType = new Dictionary<Type, OleDbType>()
        {
            { typeof(byte) , OleDbType.UnsignedTinyInt },
            { typeof(sbyte) , OleDbType.TinyInt },
            { typeof(ushort) , OleDbType.UnsignedSmallInt },
            { typeof(short) , OleDbType.SmallInt },
            { typeof(uint) , OleDbType.UnsignedInt },
            { typeof(int) , OleDbType.Integer },
            { typeof(ulong) , OleDbType.UnsignedBigInt },
            { typeof(long) , OleDbType.BigInt },
            { typeof(float) , OleDbType.Single },
            { typeof(double) , OleDbType.Double },
            { typeof(decimal) , OleDbType.Decimal },
            { typeof(bool) , OleDbType.Boolean },
            { typeof(string) , OleDbType.BSTR },
            { typeof(char) , OleDbType.Char },
            { typeof(Guid) , OleDbType.Guid },
            { typeof(DateTime) , OleDbType.Date },
            { typeof(DateTimeOffset) , OleDbType.Date },
            { typeof(byte[]) , OleDbType.Binary },
            { typeof(byte?) , OleDbType.UnsignedTinyInt },
            { typeof(sbyte?) , OleDbType.TinyInt },
            { typeof(ushort?) , OleDbType.UnsignedSmallInt },
            { typeof(short?) , OleDbType.SmallInt },
            { typeof(uint?) , OleDbType.UnsignedInt },
            { typeof(int?) , OleDbType.Integer },
            { typeof(ulong?) , OleDbType.UnsignedBigInt },
            { typeof(long?) , OleDbType.BigInt },
            { typeof(float?) , OleDbType.Single },
            { typeof(double?) , OleDbType.Double },
            { typeof(decimal?) , OleDbType.Decimal },
            { typeof(bool?) , OleDbType.Boolean },
            { typeof(char?) , OleDbType.Char },
            { typeof(Guid?) , OleDbType.Guid },
            { typeof(DateTime?) , OleDbType.Date },
            { typeof(DateTimeOffset?) , OleDbType.Date },
            { typeof(System.Data.Linq.Binary) , OleDbType.Binary }
        };

        static readonly Dictionary<OleDbType, OleDbType> compatibleOleDbType = new Dictionary<OleDbType, OleDbType>()
        {
            { OleDbType.Date , OleDbType.DBDate },
            { OleDbType.DBDate , OleDbType.DBTimeStamp },
            { OleDbType.DBTimeStamp , OleDbType.Filetime },

            { OleDbType.VarWChar , OleDbType.WChar },
            { OleDbType.WChar , OleDbType.BSTR },

            { OleDbType.TinyInt , OleDbType.SmallInt },
            { OleDbType.UnsignedTinyInt , OleDbType.SmallInt },
            { OleDbType.UnsignedSmallInt , OleDbType.Integer },
            { OleDbType.UnsignedInt , OleDbType.BigInt },
            { OleDbType.UnsignedBigInt , OleDbType.BigInt }
        };

        static readonly Dictionary<Type, Type> unsupportedNetType = new Dictionary<Type, Type>()
        {
            { typeof(sbyte) , typeof(byte) }
        };

        static readonly string DataSourceKeyword = "Data Source";
        #endregion

        #region Constructors
        public MsSqlCeSchemaInfo(string dataProviderName, string connectionString) : 
            base(dataProviderName, connectionString)
        {
        }
        #endregion

        #region Methods
        OleDbType GetOleDbType(Type type)
        {
            if (!typeToOleDbType.ContainsKey(type))
                throw new ArgumentException(String.Format("Unsupported data type value '{0}' for provider {1}!", type.FullName, DataProviderName));

            var dbType = typeToOleDbType[type];
            if (IsTypeMatch(type, (int)dbType))
                return dbType;

            dbType = GetCompatibleOleDbType(dbType);
            if (ProviderTypeToNames.ContainsKey((int)dbType))
                return dbType;

            dbType = OleDbType.VarWChar;
            if (ProviderTypeToNames.ContainsKey((int)dbType))
                return dbType;
            return GetCompatibleOleDbType(dbType);
        }

        OleDbType GetCompatibleOleDbType(OleDbType dbType)
        {
            while (compatibleOleDbType.ContainsKey(dbType))
            {
                dbType = compatibleOleDbType[dbType];
                if (ProviderTypeToNames.ContainsKey((int)dbType))
                    return dbType;
            }

            return dbType;
        }
        #endregion

        #region Public Static Methods
        public static void CreateDatabase(string connectionString)
        {
            if (!MSSqlCEHelper.Utilities.IsV40Installed())
                throw new InvalidOperationException(Properties.Resources.SqlCENotInstalled);

            XpoHelpers.XpoHelper.CreateDirectoryIfNotExists(connectionString);
            var helper = new ConnectionStringParser(connectionString);
            if (helper.PartExists(DataSourceKeyword))
            {
                var dbFileName = helper.GetPartByName(DataSourceKeyword);
                var directoryName = System.IO.Path.GetDirectoryName(dbFileName);
                if (String.IsNullOrEmpty(directoryName))
                    dbFileName = String.Format("{0}\\{1}", Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), dbFileName);
                if (!System.IO.File.Exists(dbFileName))
                {
                    using (SqlCeEngine engine = new SqlCeEngine(connectionString))
                    {
                        engine.CreateDatabase();
                    }
                }
            }
        }
        #endregion

        #region Overrides
        protected override int GetDbType(Type type)
        {
            return (int)GetOleDbType(type);
        }

        protected override string FormatDefaultValue(Type type, object value)
        {
            var dbType = GetOleDbType(type);
            if (dbType == OleDbType.Boolean)
            {
                bool result = (bool)TypeExtensions.ChangeType(value, typeof(bool), force: true);
                return String.Format("{0}", result ? 1 : 0);
            }
            else if (dbType == OleDbType.Date)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
            }
            else
            {
                var info = new System.Globalization.NumberFormatInfo { NumberDecimalSeparator = ".", NumberGroupSeparator = "," };
                return String.Format(info, "{0}", TypeExtensions.ChangeType(value, type, force: true));
            }
        }

        protected override string GetAutoIncrementKeyword(long seed, long step)
        {
            return string.Format("IDENTITY({0}, {1})", seed, step);
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
            var dbType = GetOleDbType(column.DataType);
            if (dbType == OleDbType.BSTR ||
                dbType == OleDbType.VarChar ||
                dbType == OleDbType.VarBinary)
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
