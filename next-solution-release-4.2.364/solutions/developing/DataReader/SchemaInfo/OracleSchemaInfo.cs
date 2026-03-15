using DataReader.Extensions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReader.SchemaInfo
{
    public class OracleSchemaInfo : DbSchemaInfo
    {
        #region Declarations
        static readonly Dictionary<Type, OracleType> typeToOracleType = new Dictionary<Type, OracleType>()
        {
            { typeof(byte) , OracleType.Byte },
            { typeof(sbyte) , OracleType.SByte },
            { typeof(ushort) , OracleType.UInt16 },
            { typeof(short) , OracleType.Int16 },
            { typeof(uint) , OracleType.UInt32 },
            { typeof(int) , OracleType.Int32 },
            { typeof(ulong) , OracleType.Number },
            { typeof(long) , OracleType.Number },
            { typeof(float) , OracleType.Float },
            { typeof(double) , OracleType.Double },
            { typeof(decimal) , OracleType.Number },
            { typeof(bool) , OracleType.Byte },
            { typeof(string) , OracleType.NVarChar },
            { typeof(char) , OracleType.Char },
            { typeof(Guid) , OracleType.NVarChar },
            { typeof(DateTime) , OracleType.Timestamp },
            { typeof(DateTimeOffset) , OracleType.TimestampWithTZ },
            { typeof(byte[]) , OracleType.Raw },
            { typeof(byte?) , OracleType.Byte },
            { typeof(sbyte?) , OracleType.SByte },
            { typeof(ushort?) , OracleType.UInt16 },
            { typeof(short?) , OracleType.Int16 },
            { typeof(uint?) , OracleType.UInt32 },
            { typeof(int?) , OracleType.Int32 },
            { typeof(ulong?) , OracleType.Number },
            { typeof(long?) , OracleType.Number },
            { typeof(float?) , OracleType.Float },
            { typeof(double?) , OracleType.Double },
            { typeof(decimal?) , OracleType.Number },
            { typeof(bool?) , OracleType.Byte },
            { typeof(char?) , OracleType.Char },
            { typeof(Guid?) , OracleType.NVarChar },
            { typeof(DateTime?) , OracleType.Timestamp },
            { typeof(DateTimeOffset?) , OracleType.TimestampWithTZ },
#if !NET_STANDARD
            { typeof(System.Data.Linq.Binary) , OracleType.LongRaw }
#endif
        };

        static readonly Dictionary<OracleType, OracleType> compatibleOracleType = new Dictionary<OracleType, OracleType>()
        {
            { OracleType.DateTime , OracleType.Timestamp }, 
            { OracleType.Timestamp , OracleType.TimestampLocal }, 
            { OracleType.TimestampLocal , OracleType.TimestampWithTZ },

            { OracleType.NVarChar , OracleType.VarChar }, 
            { OracleType.VarChar , OracleType.NClob }, 
            { OracleType.NClob , OracleType.NChar }, 

            { OracleType.Byte , OracleType.Number }, 
            { OracleType.SByte , OracleType.Number }, 
            { OracleType.UInt16 , OracleType.Number }, 
            { OracleType.Int16 , OracleType.Number }, 
            { OracleType.UInt32 , OracleType.Number }, 
            { OracleType.Int32 , OracleType.Number }, 
            { OracleType.Float , OracleType.Number }, 
            { OracleType.Double , OracleType.Number }
        };
        #endregion

        #region Constructors
        public OracleSchemaInfo(string dataProviderName, string connectionString) : 
            base(dataProviderName, connectionString)
        {
        }
        #endregion

        #region Methods
        OracleType GetOracleType(Type type)
        {
            if (!typeToOracleType.ContainsKey(type))
                throw new ArgumentException(String.Format("Unsupported data type value '{0}' for provider {1}!", type.FullName, DataProviderName));

            var dbType = typeToOracleType[type];
            if (IsTypeMatch(type, (int)dbType))
                return dbType;

            dbType = GetCompatibleOracleDbType(dbType);
            if (ProviderTypeToNames.ContainsKey((int)dbType))
                return dbType;

            dbType = OracleType.NVarChar;
            if (ProviderTypeToNames.ContainsKey((int)dbType))
                return dbType;
            return GetCompatibleOracleDbType(dbType);
        }

        OracleType GetCompatibleOracleDbType(OracleType dbType)
        {
            while (compatibleOracleType.ContainsKey(dbType))
            {
                dbType = compatibleOracleType[dbType];
                if (ProviderTypeToNames.ContainsKey((int)dbType))
                    return dbType;
            }

            return dbType;
        }
        #endregion

        #region Overrides
        protected override int GetDbType(Type type)
        {
            return (int)GetOracleType(type);
        }

        protected override string FormatDefaultValue(Type type, object value)
        {
            var sqlDbType = GetOracleType(type);
            if (sqlDbType == OracleType.DateTime)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("yyyy-MM-dd HH:mm:ss", CultureInfo.InvariantCulture));
            }
            else if (sqlDbType == OracleType.Timestamp)
            {
                DateTime dateTime = (DateTime)TypeExtensions.ChangeType(value, typeof(DateTime), force: true);
                return String.Format("{0}", dateTime.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
            }
            else if (sqlDbType == OracleType.TimestampLocal || sqlDbType == OracleType.TimestampWithTZ)
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
            return string.Format("GENERATED BY DEFAULT AS IDENTITY (START WITH {0} INCREMENT BY {1})", seed, step);
        }

        public override string FormatParameterName(string parameterName)
        {
            return string.Format(":{0}", parameterName.Replace(' ', '_'));
        }

        public override string GetTableName(DataRow schemaRow, bool useSchemaQuotes)
        {
            if (useSchemaQuotes)
            {
                return String.Format("{0}.{1}",
                    WrapObjectName((string)schemaRow["OWNER"]),
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
                    WrapObjectName((string)schemaRow["OWNER"]),
                    WrapObjectName((string)schemaRow["VIEW_NAME"]));
            }
            else
                return (string)schemaRow["VIEW_NAME"];
        }
        #endregion
    }
}
