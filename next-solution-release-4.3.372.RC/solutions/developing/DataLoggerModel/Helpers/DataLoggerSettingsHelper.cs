using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLoggerModel;
using DataReader.Extensions;
using UFUAModel.Extensions;
using System.Data;
using Utilities.Converters;

namespace DataLoggerModel.Helpers
{
    /// <summary>
    /// Helper for handle the DataLoggerSettings class object
    /// </summary>
    public sealed class DataLoggerSettingsHelper
    {
        #region Constants
        const string DefaultUtcTimeColumnName = "UtcTimeCol";
        const string DefaultLocalTimeColumnName = "LocalTimeCol";
        const string DefaultMillisecondsColumnName = "MillisecondsCol";
        const string DefaultUserColumnName = "UserCol";
        const string DefaultReasonColumnName = "ReasonCol";
        const string DefaultRedundancyColumnName = "RedundancySyncTime";
        const string DefaultAutoIncrementColumnName = "OID";

        const string DefaultStatusCodeSuffixColumnName = "Quality";
        const string DefaultServerTimeStampSuffixColumnName = "ServerTimeStamp";
        const string DefaultSourceTimeStampSuffixColumnName = "SourceTimeStamp";
        const string DefaultUserSuffixColumnName = "User";
        const string DefaultStringValueSuffixColumnName = "StringValue";
        #endregion

        #region Declarations
        List<string> statusCodeColumnNamesList;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialize a new instance of DataLoggerSettingsHelper by passing the data logger settings
        /// </summary>
        /// <param name="settings"></param>
        /// <param name="projectRoot">
        /// project root folder to use for replacing placheholder.
        /// </param>
        public DataLoggerSettingsHelper(DataLoggerSettings settings, string projectRoot)
        {
            dataLoggerSettings = settings;
            dataLoggerSettings.ConnectionSettings?.NormalizeConnectionString(projectRoot);
        }
        #endregion

        #region Public Properties
        public List<string> StatusCodeColumnNamesList
        {
            get
            {
                if (statusCodeColumnNamesList == null)
                {
                    statusCodeColumnNamesList = new List<string>();
                    var listcolumns = (from c in dataLoggerSettings.Columns
                                       where c.IsValid && c.AddStatusCodeColumn
                                       orderby c.Oid
                                       select c).ToList();
                    foreach (var column in listcolumns)
                    {
                        var statusColName = GetStatusCodeColumnName(column);
                        if (!statusCodeColumnNamesList.Contains(statusColName))
                            statusCodeColumnNamesList.Add(statusColName);
                    }
                }
                return statusCodeColumnNamesList;
            }
        }
        /// <summary>
        /// Return 'true' is the data logger settings are valid.
        /// </summary>
        public bool IsValidSettings
        {
            get
            {
                return dataLoggerSettings.ConnectionSettings.IsValid();
            }
        }

        /// <summary>
        /// Get the data logger configuration object used for initializing this instance
        /// </summary>
        public DataLoggerSettings DataLoggerSettings
        {
            get
            {
                return dataLoggerSettings;
            }
        }

        /// <summary>
        /// Get the table name of the data logger used for initializing this instance
        /// </summary>
        public String TableName
        {
            get
            {
                return String.IsNullOrEmpty(DataLoggerSettings.TableName) ? DataLoggerSettings.Name : DataLoggerSettings.TableName;
            }
        }

        /// <summary>
        /// Get the UTC time column name of the data logger used for initializing this instance
        /// </summary>
        public String UtcTimeColumnName
        {
            get
            {
                return String.IsNullOrEmpty(DataLoggerSettings.UtcTimeColumnName) ? DefaultUtcTimeColumnName : DataLoggerSettings.UtcTimeColumnName;
            }
        }

        /// <summary>
        /// Get the local time column name of the data logger used for initializing this instance
        /// </summary>
        public String LocalTimeColumnName
        {
            get
            {
                return String.IsNullOrEmpty(DataLoggerSettings.LocalTimeColumnName) ? DefaultLocalTimeColumnName : DataLoggerSettings.LocalTimeColumnName;
            }
        }

        /// <summary>
        /// Get the Milliseconds column name of the data logger used for initializing this instance
        /// </summary>
        public String MillisecondsColumnName
        {
            get
            {
                return String.IsNullOrEmpty(DataLoggerSettings.MillisecondsColumnName) ? DefaultMillisecondsColumnName : DataLoggerSettings.MillisecondsColumnName;
            }
        }

        /// <summary>
        /// Get the user column name of the data logger used for initializing this instance
        /// </summary>
        public String UserColumnName
        {
            get
            {
                return String.IsNullOrEmpty(DataLoggerSettings.UserColumnName) ? DefaultUserColumnName : DataLoggerSettings.UserColumnName;
            }
        }

        /// <summary>
        /// Get the reason column name of the data logger used for initializing this instance
        /// </summary>
        public String ReasonColumnName
        {
            get
            {
                return String.IsNullOrEmpty(DataLoggerSettings.ReasonColumnName) ? DefaultReasonColumnName : DataLoggerSettings.ReasonColumnName;
            }
        }

        /// <summary>
        /// Get the redundancy sync column name of the data logger used for initializing this instance
        /// </summary>
        public String RedundancyColumnName
        {
            get
            {
                return DefaultRedundancyColumnName;
            }
        }

        /// <summary>
        /// Get the auto-increment column name of the data logger used for initializing this instance
        /// </summary>
        public String AutoIncrementColumnName
        {
            get
            {
                return DefaultAutoIncrementColumnName;
            }
        }

        /// <summary>
        /// Get an array with all the index column name of this instance of data logger state.
        /// </summary>
        public String[] IndexColumns
        {
            get
            {
                return new String[]
                {
                    AutoIncrementColumnName,
                    UtcTimeColumnName,
                    LocalTimeColumnName,
                    RedundancyColumnName
                };
            }
        }

        public static String[] ReservedColumnNames
        {
            get
            {
                return new String[] 
                {
                    DefaultUtcTimeColumnName,
                    DefaultLocalTimeColumnName,
                    DefaultMillisecondsColumnName,
                    DefaultUserColumnName,
                    DefaultReasonColumnName,
                    DefaultRedundancyColumnName,
                    DefaultAutoIncrementColumnName
                };
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the 'DataLoggerColumn' object instance find it by name.
        /// </summary>
        /// <param name="columnName">
        /// Name of the column to search.
        /// </param>
        /// <returns></returns>
        public DataLoggerColumn GetDataLoggerColumn(String columnName)
        {
            return (from c in dataLoggerSettings.Columns where c.ColumnName == columnName select c).FirstOrDefault();
        }

        /// <summary>
        /// Create a System.Data.DataTable object that respect the data logger settings.
        /// </summary>
        /// <param name="isRedundancyEnabled">
        /// Set to 'true' in order to add the Redundancy columns at the System.Data.DataTable returned.
        /// </param>
        /// <returns></returns>
        public DataTable CreateDataTable(bool isRedundancyEnabled = false, bool skipDuplicatedColumns = false)
        {
            var dt = new DataTable(TableName);
            var keys = new List<DataColumn>();

            if (DataLoggerSettings.EnableDataProtection)
            {
                if (!skipDuplicatedColumns || !dt.Columns.Contains(AutoIncrementColumnName))
                {
                    dt.Columns.Add(new DataColumn(AutoIncrementColumnName)
                    {
                        Unique = true,
                        ReadOnly = true,
                        AutoIncrement = true,
                        AutoIncrementSeed = 1,
                        AutoIncrementStep = 1,
                        AllowDBNull = false,
                        DataType = typeof(Int32),
                        ColumnMapping = MappingType.Element
                    });
                    keys.Add(dt.Columns[AutoIncrementColumnName]);
                }
            }

            if (!skipDuplicatedColumns || !dt.Columns.Contains(UtcTimeColumnName))
            {
                // Add predefined UTC date time data logger's column
                dt.Columns.Add(new DataColumn(UtcTimeColumnName)
                {
                    Unique = false,
                    AllowDBNull = false,
                    DataType = typeof(DateTime),
                    DefaultValue = System.Data.SqlTypes.SqlDateTime.MinValue.Value,
                    ColumnMapping = MappingType.Element
                });
            }

            if (!skipDuplicatedColumns || !dt.Columns.Contains(LocalTimeColumnName))
            {
                // Add predefined local date time data logger's column
                dt.Columns.Add(new DataColumn(LocalTimeColumnName)
                {
                    Unique = false,
                    AllowDBNull = true,
                    DataType = typeof(DateTime),
                    ColumnMapping = MappingType.Element
                });
            }

            if (!skipDuplicatedColumns || !dt.Columns.Contains(MillisecondsColumnName))
            {
                // Add predefined UTC date time data logger's column
                dt.Columns.Add(new DataColumn(MillisecondsColumnName)
                {
                    Unique = false,
                    AllowDBNull = false,
                    DataType = typeof(ushort),
                    DefaultValue = 0,
                    ColumnMapping = MappingType.Element
                });
            }

            if (!skipDuplicatedColumns || !dt.Columns.Contains(UserColumnName))
            {
                // Add predefined user data logger's column
                dt.Columns.Add(new DataColumn(UserColumnName)
                {
                    Unique = false,
                    AllowDBNull = true,
                    DataType = typeof(String),
                    MaxLength = dataLoggerSettings.MaxLength.Value,
                    ColumnMapping = MappingType.Element
                });
            }

            if (!skipDuplicatedColumns || !dt.Columns.Contains(ReasonColumnName))
            {
                // Add predefined reason data logger's column
                dt.Columns.Add(new DataColumn(ReasonColumnName)
                {
                    Unique = false,
                    AllowDBNull = true,
                    DataType = typeof(String),
                    MaxLength = dataLoggerSettings.MaxLength.Value,
                    ColumnMapping = MappingType.Element
                });
            }

            // Add data logger's columns
            var listcolumns = (from c in dataLoggerSettings.Columns
                               where c.IsValid
                               orderby c.Oid
                               select c).ToList();
            foreach (var column in listcolumns)
            {
                if (dataLoggerSettings.IsRunningInstance && column.UFUATagReference == null)
                    continue;
            	
                if (!skipDuplicatedColumns || !dt.Columns.Contains(column.ColumnName))
                {
                    // Add Value field.
                    dt.Columns.Add(new DataColumn(column.ColumnName)
                    {
                        Unique = false,
                        AllowDBNull = true,
                        DataType = GetColumnType(column),
                        MaxLength = GetMaxLength(column, DataLoggerSettings.MaxLength.Value),
                        DefaultValue = GetDefaultValue(column),
                        ColumnMapping = MappingType.Element
                    });
                }

                // Add optional source time stamp field.
                if (column.AddSourceTimeStampColumn)
                {
                    if (!skipDuplicatedColumns || !dt.Columns.Contains(GetSourceTimeStampColumnName(column)))
                    {
                        dt.Columns.Add(new DataColumn(GetSourceTimeStampColumnName(column))
                        {
                            Unique = false,
                            AllowDBNull = true,
                            DataType = IsObjectTypeValue(column) ? typeof(string) : typeof(DateTime),
                            ColumnMapping = MappingType.Element
                        });
                    }
                }

                // Add optional server time stamp field.
                if (column.AddServerTimeStampColumn)
                {
                    if (!skipDuplicatedColumns || !dt.Columns.Contains(GetServerTimeStampColumnName(column)))
                    {
                        dt.Columns.Add(new DataColumn(GetServerTimeStampColumnName(column))
                        {
                            Unique = false,
                            AllowDBNull = true,
                            DataType = IsObjectTypeValue(column) ? typeof(string) : typeof(DateTime),
                            ColumnMapping = MappingType.Element
                        });
                    }
                }

                // Add optional status code field.
                if (column.AddStatusCodeColumn)
                {
                    if (!skipDuplicatedColumns || !dt.Columns.Contains(GetStatusCodeColumnName(column)))
                    {
                        dt.Columns.Add(new DataColumn(GetStatusCodeColumnName(column))
                        {
                            Unique = false,
                            AllowDBNull = true,
                            DataType = IsObjectTypeValue(column) ? typeof(string) : typeof(uint),
                            ColumnMapping = MappingType.Element
                        });
                    }
                }

                // Add optional user name field.
                if (column.AddUserColumn)
                {
                    if (!skipDuplicatedColumns || !dt.Columns.Contains(GetUserColumnName(column)))
                    {
                        dt.Columns.Add(new DataColumn(GetUserColumnName(column))
                        {
                            Unique = false,
                            AllowDBNull = true,
                            DataType = typeof(String),
                            MaxLength = dataLoggerSettings.MaxLength.Value,
                            ColumnMapping = MappingType.Element
                        });
                    }
                }

                // Add optional string value name field.
                if (column.AddStringValueColumn)
                {
                    if (!skipDuplicatedColumns || !dt.Columns.Contains(GetStringValueColumnName(column)))
                    {
                        dt.Columns.Add(new DataColumn(GetStringValueColumnName(column))
                        {
                            Unique = false,
                            AllowDBNull = true,
                            DataType = typeof(String),
                            MaxLength = dataLoggerSettings.MaxLength.Value,
                            ColumnMapping = MappingType.Element
                        });
                    }
                }
            }

            // Add redundancy data logger's column
            if (isRedundancyEnabled)
            {
                if (!skipDuplicatedColumns || !dt.Columns.Contains(RedundancyColumnName))
                {
                    dt.Columns.Add(new DataColumn(RedundancyColumnName)
                    {
                        Unique = false,
                        AllowDBNull = true,
                        DataType = typeof(DateTime),
                        ColumnMapping = MappingType.Element
                    });
                }
            }

            if (!DataLoggerSettings.AllowDuplicatedRows)
            {
                // Create primary keys
                keys.Add(dt.Columns[UtcTimeColumnName]);
                keys.Add(dt.Columns[MillisecondsColumnName]);
            }

            if (keys.Count > 0)
                dt.PrimaryKey = keys.ToArray();
            
            return dt;
        }

        public String[] GetValidColumnNames()
        {
            return (from c in dataLoggerSettings.Columns
                    where c.IsValid
                    orderby c.Oid
                    select c.Name).ToArray();
        }
        #endregion

        #region Static Methods
        /// <summary>
        /// Get the .NET Type based on the column object type pass as parameter
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static Type GetColumnType(DataLoggerColumn column)
        {
            if (!String.IsNullOrEmpty(column.Expression))
            {
                var type = ExpressionValueConverter.GetFormulaType(column.Expression);
                if (type == ExpressionType.Expression &&
                    column.UFUATagReference != null && column.UFUATagReference.DataType.HasValue &&
                    !column.UFUATagReference.DataType.IsVariableLenght())
                    return typeof(Double);
                else if (type == ExpressionType.Array &&
                    column.UFUATagReference != null && column.UFUATagReference.DataType.HasValue)
                    return column.UFUATagReference.DataType.ToNetType();
                else if (type == ExpressionType.ArrayPlusBit || type == ExpressionType.Bit)
                    return typeof(Boolean);
            }
            else if (IsArrayValue(column) || IsObjectTypeValue(column))
                return typeof(String);
            else if (column.UFUATagReference != null && column.UFUATagReference.DataType.HasValue)
                return column.UFUATagReference.DataType.ToNetType();

            return typeof(String);
        }

        /// <summary>
        /// Get the maximum string size to use based on the column object type pass as parameter
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static int GetMaxLength(DataLoggerColumn column, int defaultValue)
        {
            if (!String.IsNullOrEmpty(column.Expression))
            {
                if (column.UFUATagReference != null && column.UFUATagReference.DataType.HasValue &&
                    column.UFUATagReference.DataType.IsVariableLenght())
                    return defaultValue;
            }
            else if (IsArrayValue(column) || IsObjectTypeValue(column))
                return -1;
            else if (column.UFUATagReference == null ||
                (column.UFUATagReference.DataType.HasValue &&
                column.UFUATagReference.DataType.IsVariableLenght()))
                return defaultValue;

            return -1;
        }

        /// <summary>
        /// Get the default value to use based on the column object type pass as parameter
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static object GetDefaultValue(DataLoggerColumn column)
        {
            if (!String.IsNullOrEmpty(column.Expression) || IsArrayValue(column) || IsObjectTypeValue(column))
                return null;
            else if (column.UFUATagReference != null && column.UFUATagReference.DataType.HasValue)
                return DataTypeExtensions.ChangeType(column.UFUATagReference.InitialValue, column.UFUATagReference.DataType);

            return null;
        }

        /// <summary>
        /// Return 'true' if the data logger colmun store array values.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool IsArrayValue(DataLoggerColumn column)
        {
            if (column.UFUATagReference != null && column.UFUATagReference.ArrayDimension > 0)
                return true;

            return false;
        }

        /// <summary>
        /// Return 'true' if the data logger colmun store object type values.
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static bool IsObjectTypeValue(DataLoggerColumn column)
        {
            if (column.UFUATagReference != null && column.UFUATagReference.ModelType == UFUAModel.ModelType.ObjectType)
                return true;

            return false;
        }

        /// <summary>
        /// Get the status code column name to use based on the column object type pass as parameter
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static String GetStatusCodeColumnName(DataLoggerColumn column)
        {
            return String.Format("{0}_{1}", column.ColumnName,
                String.IsNullOrEmpty(column.StatusCodeSuffixColumnName) ? DefaultStatusCodeSuffixColumnName : column.StatusCodeSuffixColumnName);
        }

        /// <summary>
        /// Get the server time stamp column name to use based on the column object type pass as parameter
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static String GetServerTimeStampColumnName(DataLoggerColumn column)
        {
            return String.Format("{0}_{1}", column.ColumnName,
                String.IsNullOrEmpty(column.ServerTimeStampSuffixColumnName) ? DefaultServerTimeStampSuffixColumnName : column.ServerTimeStampSuffixColumnName);
        }

        /// <summary>
        /// Get the source time stamp column name to use based on the column object type pass as parameter
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static String GetSourceTimeStampColumnName(DataLoggerColumn column)
        {
            return String.Format("{0}_{1}", column.ColumnName,
                String.IsNullOrEmpty(column.SourceTimeStampSuffixColumnName) ? DefaultSourceTimeStampSuffixColumnName : column.SourceTimeStampSuffixColumnName);
        }

        /// <summary>
        /// Get the user column name to use based on the column object type pass as parameter
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static String GetUserColumnName(DataLoggerColumn column)
        {
            return String.Format("{0}_{1}", column.ColumnName,
                String.IsNullOrEmpty(column.UserSuffixColumnName) ? DefaultUserSuffixColumnName : column.UserSuffixColumnName);
        }

        /// <summary>
        /// Get the string value column name to use based on the column object type pass as parameter
        /// </summary>
        /// <param name="column"></param>
        /// <returns></returns>
        public static String GetStringValueColumnName(DataLoggerColumn column)
        {
            return String.Format("{0}_{1}", column.ColumnName,
                String.IsNullOrEmpty(column.StringValueSuffixColumnName) ? DefaultStringValueSuffixColumnName : column.StringValueSuffixColumnName);
        }

        /// <summary>
        /// Compare two datalogger settings and return 'true' if an update of aggregate tables is required.
        /// </summary>
        /// <param name="oldObject"></param>
        /// <param name="newObject"></param>
        /// <returns></returns>
        public static bool NeedUpdateAggregatedTables(DataLoggerModel.DataLoggerSettings oldObject, DataLoggerModel.DataLoggerSettings newObject)
        {
            return oldObject.UseAggregatedTables && newObject.UseAggregatedTables &&
                (oldObject.Name != newObject.Name && String.IsNullOrEmpty(oldObject.TableName) ||
                oldObject.TableName != newObject.TableName ||
                oldObject.UtcTimeColumnName != newObject.UtcTimeColumnName ||
                oldObject.LocalTimeColumnName != newObject.LocalTimeColumnName ||
                oldObject.NumColumns != newObject.NumColumns);
        }

        /// <summary>
        /// Compare two column settings and return 'true' if an update of aggregate tables is required. 
        /// </summary>
        /// <param name="oldObject"></param>
        /// <param name="newObject"></param>
        /// <returns></returns>
        public static bool NeedUpdateAggregatedTables(DataLoggerModel.DataLoggerColumn oldObject, DataLoggerModel.DataLoggerColumn newObject)
        {
            return oldObject.DataLoggerReference != null && oldObject.DataLoggerReference.UseAggregatedTables && 
                (oldObject.ColumnName != newObject.ColumnName ||
                oldObject.ColumnTag != newObject.ColumnTag ||
                oldObject.Expression != newObject.Expression);
        }
        #endregion

        #region Private Members
        readonly DataLoggerSettings dataLoggerSettings;
        #endregion
    }
}
