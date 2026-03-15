using DataReader.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Odbc;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReader.SchemaInfo
{
    public class OdbcSchemaInfo : DbSchemaInfo
    {
        #region Declarations
        static readonly Dictionary<Type, OdbcType> typeToOdbcType = new Dictionary<Type, OdbcType>()
        {
            { typeof(byte) , OdbcType.TinyInt },
            { typeof(sbyte) , OdbcType.TinyInt },
            { typeof(ushort) , OdbcType.SmallInt },
            { typeof(short) , OdbcType.SmallInt },
            { typeof(uint) , OdbcType.Int },
            { typeof(int) , OdbcType.Int },
            { typeof(ulong) , OdbcType.BigInt },
            { typeof(long) , OdbcType.BigInt },
            { typeof(float) , OdbcType.Real },
            { typeof(double) , OdbcType.Double },
            { typeof(decimal) , OdbcType.Decimal },
            { typeof(bool) , OdbcType.Bit },
            { typeof(string) , OdbcType.NVarChar },
            { typeof(char) , OdbcType.Char },
            { typeof(Guid) , OdbcType.UniqueIdentifier },
            { typeof(DateTime) , OdbcType.DateTime },
            { typeof(DateTimeOffset) , OdbcType.DateTime },
            { typeof(byte[]) , OdbcType.Binary },
            { typeof(byte?) , OdbcType.TinyInt },
            { typeof(sbyte?) , OdbcType.TinyInt },
            { typeof(ushort?) , OdbcType.SmallInt },
            { typeof(short?) , OdbcType.SmallInt },
            { typeof(uint?) , OdbcType.Int },
            { typeof(int?) , OdbcType.Int },
            { typeof(ulong?) , OdbcType.BigInt },
            { typeof(long?) , OdbcType.BigInt },
            { typeof(float?) , OdbcType.Real },
            { typeof(double?) , OdbcType.Double },
            { typeof(decimal?) , OdbcType.Decimal },
            { typeof(bool?) , OdbcType.Bit },
            { typeof(char?) , OdbcType.Char },
            { typeof(Guid?) , OdbcType.UniqueIdentifier },
            { typeof(DateTime?) , OdbcType.DateTime },
            { typeof(DateTimeOffset?) , OdbcType.DateTime },
#if !NET_STANDARD
            { typeof(System.Data.Linq.Binary) , OdbcType.Binary }
#endif
        };

        static readonly Dictionary<OdbcType, OdbcType> compatibleOdbcType = new Dictionary<OdbcType, OdbcType>()
        {
            { OdbcType.DateTime , OdbcType.SmallDateTime }, 
            { OdbcType.SmallDateTime , OdbcType.Date }, 

            { OdbcType.NVarChar , OdbcType.VarChar }, 
            { OdbcType.VarChar , OdbcType.NText }, 
            { OdbcType.NText , OdbcType.Text },

            { OdbcType.TinyInt , OdbcType.SmallInt }, 
            { OdbcType.SmallInt , OdbcType.Int }, 
            { OdbcType.Int , OdbcType.BigInt }
        };
        #endregion

        #region Constructors
        public OdbcSchemaInfo(string dataProviderName, string connectionString) : 
            base(dataProviderName, connectionString)
        {
        }
        #endregion

        #region Methods
        OdbcType GetOdbcType(Type type)
        {
            if (!typeToOdbcType.ContainsKey(type))
                throw new ArgumentException(String.Format("Unsupported data type value '{0}' for provider {1}!", type.FullName, DataProviderName));

            var dbType = typeToOdbcType[type];
            if (IsTypeMatch(type, (int)dbType))
                return dbType;

            dbType = GetCompatibleOdbcType(dbType);
            if (ProviderTypeToNames.ContainsKey((int)dbType))
                return dbType;

            dbType = OdbcType.NVarChar;
            if (ProviderTypeToNames.ContainsKey((int)dbType))
                return dbType;
            return GetCompatibleOdbcType(dbType);
       }

        OdbcType GetCompatibleOdbcType(OdbcType dbType)
        {
            while (compatibleOdbcType.ContainsKey(dbType))
            {
                dbType = compatibleOdbcType[dbType];
                if (ProviderTypeToNames.ContainsKey((int)dbType))
                    return dbType;
            }

            return dbType;
        }
        #endregion

        #region Overrides
        protected override int GetDbType(Type type)
        {
            return (int)GetOdbcType(type);
        }

        protected override string FormatDefaultValue(Type type, object value)
        {
            var dbType = GetOdbcType(type);
            if (dbType == OdbcType.Bit)
            {
                bool result = (bool)TypeExtensions.ChangeType(value, typeof(bool), force: true);
                return String.Format("{0}", result ? 1 : 0);
            }
            else if (dbType == OdbcType.SmallDateTime || dbType == OdbcType.DateTime)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            }
            else if (dbType == OdbcType.Date)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            }
            else if (dbType == OdbcType.Time)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("HH:mm:ss", CultureInfo.InvariantCulture));
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
            if (useSchemaQuotes)
            {
                return String.Format("{0}.{1}",
                    WrapObjectName((string)schemaRow["TABLE_SCHEM"]),
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
                    WrapObjectName((string)schemaRow["TABLE_SCHEM"]),
                    WrapObjectName((string)schemaRow["TABLE_NAME"]));
            }
            return (string)schemaRow["TABLE_NAME"];
        }
        #endregion
    }
}
