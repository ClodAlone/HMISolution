using DataReader.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReader.SchemaInfo
{
    public class OledDbSchemaInfo : DbSchemaInfo
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
        #endregion

        #region Constructors
        public OledDbSchemaInfo(string dataProviderName, string connectionString) : 
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

        #region Overrides
        protected override int GetDbType(Type type)
        {
            return (int)GetOleDbType(type);
        }

        protected override string FormatDefaultValue(Type type, object value)
        {
            var sqlDbType = GetOleDbType(type);
            if (sqlDbType == OleDbType.Boolean)
            {
                bool result = (bool)TypeExtensions.ChangeType(value, typeof(bool), force: true);
                return String.Format("{0}", result ? 1 : 0);
            }
            else if (sqlDbType == OleDbType.DBDate)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            else if (sqlDbType == OleDbType.DBTime)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
            }
            else if (sqlDbType == OleDbType.DBTimeStamp)
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
            return "IDENTITY";
        }

        public override string GetTableName(DataRow schemaRow, bool useSchemaQuotes)
        {
            if (useSchemaQuotes && schemaRow["TABLE_SCHEMA"] != System.DBNull.Value)
            {
                return String.Format("{0}.{1}",
                    WrapObjectName((string)schemaRow["TABLE_SCHEMA"]),
                    WrapObjectName((string)schemaRow["TABLE_NAME"]));
            }
            else
                return (string)schemaRow["TABLE_NAME"];
        }

        public override string GetViewName(DataRow schemaRow, bool useSchemaQuotes)
        {
            if (useSchemaQuotes)
            {
                return String.Format("{0}.{1}",
                    WrapObjectName((string)schemaRow["TABLE_SCHEMA"]),
                    WrapObjectName((string)schemaRow["TABLE_NAME"]));
            }
            else
                return (string)schemaRow["TABLE_NAME"];
        }
        #endregion
    }
}
