using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataReader.Extensions;
using System.Diagnostics;

namespace DataReader.SchemaInfo
{
    public class ColumnInfo
    {
        public short Scale { get; set; }
        public short Precision { get; set; }
    }

    public abstract class DbSchemaInfo : IDisposable
    {
        #region Declarations
        readonly string dataProviderName;
        readonly string connectionString;

        string parameterMarkerFormat;
        string dataSourceProductName;

        DbCommandBuilder commandBuilder;
        Dictionary<String, IDictionary<String, Object>> providerTypeInfo;
        Dictionary<int, List<String>> providerTypeToNames;
        #endregion

        #region Constructors
        public DbSchemaInfo(string dataProviderName, string connectionString)
        {
            this.dataProviderName = dataProviderName;
            this.connectionString = connectionString;

            ReadSchemaInfo();
        }
        #endregion

        #region Private Methods
        void ReadSchemaInfo()
        {
            providerTypeInfo = new Dictionary<String, IDictionary<String, Object>>();
            providerTypeToNames = new Dictionary<int, List<String>>();

            var dynDataSourceInformation = DataReader.GetSchemaInfo(dataProviderName, connectionString, DbMetaDataCollectionNames.DataSourceInformation).ToList();
            if (dynDataSourceInformation.Count > 0)
            {
                var info = dynDataSourceInformation[0] as IDictionary<String, Object>;
                if (info.ContainsKey("ParameterMarkerFormat"))
                {
                    if (!(info["ParameterMarkerFormat"] is System.DBNull))
                        parameterMarkerFormat = info["ParameterMarkerFormat"] as String;
                }

                if (info.ContainsKey("DataSourceProductName"))
                {
                    if (!(info["DataSourceProductName"] is System.DBNull))
                        dataSourceProductName = info["DataSourceProductName"] as String;
                }
            }

            var dynDataTypes = DataReader.GetSchemaInfo(dataProviderName, connectionString, DbMetaDataCollectionNames.DataTypes).ToList();
            foreach (var dynType in dynDataTypes)
            {
                var typeInfo = dynType as IDictionary<String, Object>;
                
                if (typeInfo.ContainsKey("TypeName"))
                {

                    var typeName = typeInfo["TypeName"] as String;
#if DEBUG
                    string sz = string.Format("SchemaDataTypes: {0}", typeName);
                    if (typeInfo.ContainsKey("ProviderDbType") && !(typeInfo["ProviderDbType"] is System.DBNull))
                        sz = string.Format("dbData--> {0} DbType: {1}", typeName, (int)typeInfo["ProviderDbType"]);
                    if (typeInfo.ContainsKey("DataType") && !(typeInfo["DataType"] is System.DBNull))
                        sz += string.Format(" DataType {0}", typeInfo["DataType"].ToString());
                    Debug.WriteLine(sz);
#endif
                    if (!providerTypeInfo.ContainsKey(typeName))
                        providerTypeInfo.Add(typeName, typeInfo);

                    if (typeInfo.ContainsKey("ProviderDbType") && !(typeInfo["ProviderDbType"] is System.DBNull))
                    {
                        var providerDbType = (int)typeInfo["ProviderDbType"];
                        if (!providerTypeToNames.ContainsKey(providerDbType))
                            providerTypeToNames.Add(providerDbType, new List<String>());
                        providerTypeToNames[providerDbType].Add(typeName);
                    }
                }
            }

            commandBuilder = DataReader.CreateDbCommandBuilder(dataProviderName);
        }

        string DataType(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["DataType"] is System.DBNull))
                return null;

            return (string)info["DataType"];
        }

        bool IsAutoincrementable(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["IsAutoincrementable"] is System.DBNull))
                return false;

            return bool.Parse(info["IsAutoincrementable"].ToString());
        }

        bool IsBestMatch(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["IsBestMatch"] is System.DBNull))
                return false;

            return bool.Parse(info["IsBestMatch"].ToString());
        }

        bool IsFixedLength(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["IsFixedLength"] is System.DBNull))
                return false;

            return bool.Parse(info["IsFixedLength"].ToString());
        }

        bool IsFixedPrecisionScale(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["IsFixedPrecisionScale"] is System.DBNull))
                return false;

            return bool.Parse(info["IsFixedPrecisionScale"].ToString());
        }

        bool IsNullable(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["IsNullable"] is System.DBNull))
                return false;

            return bool.Parse(info["IsNullable"].ToString());
        }

        bool? IsUnsigned(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["IsUnsigned"] is System.DBNull))
                return null;

            return bool.Parse(info["IsUnsigned"].ToString());
        }

        string CreateFormat(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["CreateFormat"] is System.DBNull))
                return null;

            return (string)info["CreateFormat"];
        }

        string CreateParameters(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["CreateParameters"] is System.DBNull))
                return null;

            return (string)info["CreateParameters"];
        }

        long? ColumnSize(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["ColumnSize"] is System.DBNull))
                return null;

            return long.Parse(info["ColumnSize"].ToString());
        }

        short? MaximumScale(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["MaximumScale"] is System.DBNull))
                return null;

            return short.Parse(info["MaximumScale"].ToString());
        }

        short? MinimumScale(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["MinimumScale"] is System.DBNull))
                return null;

            return short.Parse(info["MinimumScale"].ToString());
        }

        protected virtual string LiteralPrefix(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["LiteralPrefix"] is System.DBNull))
                return string.Empty;

            return (string)info["LiteralPrefix"];
        }

        protected virtual string LiteralSuffix(string typeName)
        {
            var info = ProviderTypeInfo[typeName];
            if ((info["LiteralSuffix"] is System.DBNull))
                return string.Empty;

            return (string)info["LiteralSuffix"];
        }

        protected Type GetProviderDataType(string typeName)
        {
            var netTypeName = DataType(typeName);
            return Type.GetType(netTypeName);
        }
        #endregion

        #region Protected Properties
        protected Dictionary<String, IDictionary<String, Object>> ProviderTypeInfo
        {
            get
            {
                return providerTypeInfo;
            }
        }

        protected Dictionary<int, List<String>> ProviderTypeToNames
        {
            get
            {
                return providerTypeToNames;
            }
        }
        #endregion

        #region Protected Methods
        protected string GetProviderTypeName(Type type)
        {
            var dbType = GetDbType(type);
            if (!ProviderTypeToNames.ContainsKey(dbType))
                throw new ArgumentException(String.Format("Missing data type value '{0}' for provider {1}!", dbType, DataProviderName));

            var typeNames = ProviderTypeToNames[dbType];
            
            long bestSize = 0;
            var typeName = typeNames[0];
            for (int ii = 0; ii < typeNames.Count; ii++)
            {
                if (IsBestMatch(typeNames[ii]))
                    return typeNames[ii];

                var columnSize = ColumnSize(typeNames[ii]);
                var unsigned = IsUnsigned(typeNames[ii]);
                if (columnSize.HasValue && columnSize.Value > bestSize && 
                    (!unsigned.HasValue || type.IsUnsigned() == unsigned.Value))
                {
                    bestSize = columnSize.Value;
                    typeName = typeNames[ii];
                }
            }

            return typeName;
        }

        protected bool IsTypeMatch(Type type, int dbType)
        {
            if (!ProviderTypeToNames.ContainsKey(dbType))
                return false;

            bool? unsigned = null;
            bool match = type.IsUnsigned();
            var typeNames = ProviderTypeToNames[dbType];
            for (int ii = 0; ii < typeNames.Count; ii++)
            {
                if (IsUnsigned(typeNames[ii]).HasValue)
                {
                    unsigned = IsUnsigned(typeNames[ii]);
                    // Check sign match with request type.
                    if (unsigned.Value == match)
                    {
                        var netType = GetProviderDataType(typeNames[ii]);
                        // Check sign match with db schema type.
                        if (netType.IsUnsigned() == unsigned.Value)
                            return true;
                    }
                }
            }

            if (!unsigned.HasValue)
                return true;

            return false;

        }
        #endregion

        #region Public Properties
        public string DataProviderName
        {
            get
            {
                return dataProviderName;
            }
        }

        public string ConnectionString
        {
            get
            {
                return connectionString;
            }
        }

        public string DataSourceProductName
        {
            get
            {
                return dataSourceProductName;
            }
        }

        public string RenameTableNameSyntax
        {
            get
            {
                if (dataSourceProductName != null)
                {
                    if (dataSourceProductName.Contains("Microsoft SQL Server"))
                        return "sp_rename '{0}', '{1}'";
                }

                return string.Format("ALTER TABLE {0} RENAME TO {1}",
                            WrapObjectName("{0}"), WrapObjectName("{1}"));
            }
        }

        public string RenameTableColumnNameSyntax
        {
            get
            {
                if (dataSourceProductName != null)
                {
                    if (dataSourceProductName.Contains("Microsoft SQL Server"))
                        return "sp_rename '{0}.{1}', '{2}', 'COLUMN'";
                    else if (dataSourceProductName.Contains("MySQL"))
                        return string.Format("ALTER TABLE {0} CHANGE COLUMN {1} {2}",
                            WrapObjectName("{0}"), WrapObjectName("{1}"), WrapObjectName("{2}"));
                }

                return string.Format("ALTER TABLE {0} RENAME COLUMN {1} TO {2}",
                            WrapObjectName("{0}"), WrapObjectName("{1}"), WrapObjectName("{2}"));
            }
        }

        public bool IsSupportedConstraintKeyword
        {
            get
            {
                if (dataSourceProductName == null)
                    return false;

                return dataSourceProductName.Contains("Microsoft SQL Server") ||
                    dataSourceProductName.Contains("Microsoft Access");
            }
        }

        public bool IsSupportedTopKeyword
        {
            get
            {
                if (dataSourceProductName == null)
                    return false;

                return dataSourceProductName.Contains("Microsoft SQL Server") ||
                    dataSourceProductName.Contains("Microsoft Access");
            }
        }

        public bool IsDataTypeNeedOnColumnChange
        {
            get
            {
                if (dataSourceProductName == null)
                    return false;

                return dataSourceProductName.Contains("MySQL");
            }
        }
        #endregion

        #region Abstract Methods
        protected abstract int GetDbType(Type type);

        protected abstract string FormatDefaultValue(Type type, object value);

        protected abstract string GetAutoIncrementKeyword(long seed, long step);
        #endregion

        #region Public Methods
        public IDictionary<String, Object> GetProviderTypeInfo(Type type)
        {
            try
            {
                var typeName = GetProviderTypeName(type);
                if (ProviderTypeInfo.ContainsKey(typeName))
                    return ProviderTypeInfo[typeName].ToDictionary(t => t.Key, t => t.Value);
            }
            catch
            { }

            return null;
        }

        public virtual Type GetProviderDataType(Type type)
        {
            var typeName = GetProviderTypeName(type);
            return GetProviderDataType(typeName);
        }

        public short GetParameterScale(Type type)
        {
            var typeName = GetProviderTypeName(type);
            if (!MaximumScale(typeName).HasValue)
                return 0;

            return MaximumScale(typeName).Value;
        }

        public bool IsNullable(Type type)
        {
            var typeName = GetProviderTypeName(type);
            return IsNullable(typeName);
        }

        public bool IsFixedLength(Type type)
        {
            var typeName = GetProviderTypeName(type);
            return IsFixedLength(typeName);
        }

        public virtual bool AreCompatibleDataTypes(Type type1, Type type2)
        {
            var providerType1 = GetProviderDataType(type1);
            var providerType2 = GetProviderDataType(type2);
            return providerType1 == providerType2;
        }

        public string WrapObjectName(string dataBaseName)
        {
            if (commandBuilder == null)
                return dataBaseName;
            
            return string.Format("{0}{1}{2}", !dataBaseName.StartsWith(commandBuilder.QuotePrefix) ? commandBuilder.QuotePrefix : string.Empty, dataBaseName, !dataBaseName.EndsWith(commandBuilder.QuoteSuffix) ? commandBuilder.QuoteSuffix : string.Empty);
        }

        public virtual string FormatParameterName(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterMarkerFormat))
                return "?";

            if (!parameterMarkerFormat.Contains('{') && !parameterMarkerFormat.Contains('}'))
                return parameterMarkerFormat;
            else
                return string.Format(parameterMarkerFormat, parameterName.Replace(' ', '_'));
        }

        public virtual string FormatDataTypeName(DataColumn column)
        {
            var typeName = GetProviderTypeName(column.DataType);
            if (DataType(typeName) == typeof(System.String).FullName)
            {
                long maxLenght = 255;
                if (column.MaxLength > 0)
                    maxLenght = column.MaxLength;
                else if (ColumnSize(typeName).HasValue && ColumnSize(typeName).Value > 0)
                    maxLenght = ColumnSize(typeName).Value / 2;

                if (CreateFormat(typeName) != null)
                    return string.Format(CreateFormat(typeName), maxLenght);
                else if (CreateParameters(typeName) != null)
                    return string.Format("{0}({1})", typeName, maxLenght);
            }
            else if (IsFixedPrecisionScale(typeName))
            {
                long precision = 10;
                if (ColumnSize(typeName).HasValue)
                    precision = ColumnSize(typeName).Value;

                short scale = 2;
                if (MaximumScale(typeName).HasValue)
                    scale = MaximumScale(typeName).Value;

                if (CreateFormat(typeName) != null)
                    return string.Format(CreateFormat(typeName), precision, scale);
                else if (CreateParameters(typeName) != null)
                    return string.Format("{0}({1},{2})", typeName, precision, scale);
            }

            return typeName;
        }

        public string FormatDefaultValue(DataColumn column)
        {
            var typeName = GetProviderTypeName(column.DataType);
            return String.Format("{0}{1}{2}", LiteralPrefix(typeName), FormatDefaultValue(column.DataType, column.DefaultValue), LiteralSuffix(typeName));
        }

        public string FormatAutoIncrement(DataColumn column)
        {
            if (!column.AutoIncrement)
                return string.Empty;

            return string.Format(" {0} ", GetAutoIncrementKeyword(column.AutoIncrementSeed, column.AutoIncrementStep));
        }

        public virtual string GetTableName(DataRow schemaRow, bool useSchemaQuotes)
        {
            foreach (DataColumn column in schemaRow.Table.Columns)
            {
                if (column.ColumnName.ToLower().Contains("name"))
                {
                    if (useSchemaQuotes)
                        return WrapObjectName((string)schemaRow[column]);
                    else
                        return (string)schemaRow[column];
                }
            }

            return null;
        }

        public virtual string GetViewName(DataRow schemaRow, bool useSchemaQuotes)
        {
            foreach (DataColumn column in schemaRow.Table.Columns)
            {
                if (column.ColumnName.ToLower().Contains("name"))
                {
                    if (useSchemaQuotes)
                        return WrapObjectName((string)schemaRow[column]);
                    else
                        return (string)schemaRow[column];
                }
            }

            return null;
        }
        #endregion

        #region IDisposable Members
        /// <summary>
        /// Dispose the internal IDisposable object's class
        /// </summary>
        public void Dispose()
        {
            if (commandBuilder != null)
            {
                commandBuilder.Dispose();
                commandBuilder = null;
            }

            providerTypeInfo.Clear();
            providerTypeInfo = null;

            providerTypeToNames.Clear();
            providerTypeToNames = null;
        }
        #endregion
    }
}
