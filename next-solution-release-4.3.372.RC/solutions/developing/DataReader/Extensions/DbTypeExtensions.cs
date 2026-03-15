using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataReader.Extensions
{
    public static class DbTypeExtensions
    {
        #region DbType to Type Converter
        static readonly Dictionary<DbType, Type> dbTypeToNetType = new Dictionary<DbType, Type>()
        {
            { DbType.AnsiString , typeof(string) },
            { DbType.Binary , typeof(byte[]) },
            { DbType.Byte , typeof(byte) },
            { DbType.Boolean , typeof(bool) },
            { DbType.Currency , typeof(double) },
            { DbType.Date , typeof(DateTime) },
            { DbType.DateTime , typeof(DateTime) },
            { DbType.Decimal , typeof(decimal) },
            { DbType.Double , typeof(double) },
            { DbType.Guid , typeof(Guid) },
            { DbType.Int16 , typeof(short) },
            { DbType.Int32 , typeof(int) },
            { DbType.Int64 , typeof(long) },
            { DbType.Object , typeof(object) },
            { DbType.SByte , typeof(sbyte) },
            { DbType.Single , typeof(float) },
            { DbType.String , typeof(string) },
            { DbType.Time , typeof(TimeSpan) },
            { DbType.UInt16 , typeof(ushort) },
            { DbType.UInt32 , typeof(uint) },
            { DbType.UInt64 , typeof(ulong) },
            { DbType.VarNumeric , typeof(decimal) },
            { DbType.AnsiStringFixedLength , typeof(string) },
            { DbType.StringFixedLength , typeof(string) },
            { DbType.Xml , typeof(System.Xml.XmlDocument) },
            { DbType.DateTime2 , typeof(DateTime) },
            { DbType.DateTimeOffset , typeof(DateTimeOffset) }
        };

        public static Type ToNetType(this DbType dbType)
        {
            if (!dbTypeToNetType.ContainsKey(dbType))
                throw new ArgumentException(String.Format("Unsupported data type conversion ('{0}') !", dbType));

            return dbTypeToNetType[dbType];
        }
        #endregion
    }
}
