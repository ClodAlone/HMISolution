using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLoggerModel.Converters;
using DataLoggerModel.Helpers;
using DataReader;
using DevExpress.Xpo;
using DevExpress.Xpo.DB.Helpers;
using DevExpress.Xpo.Metadata;
using Utilities;
using UFUAModel;
using DataReader.Helpers;
using UFInterfaces.PropertyControl;
#if !NET_STANDARD
using Utilities.Xpo.UndoRedo;
#endif

namespace DataLoggerModel
{
    [Exportable(RequiredKeys = new string[] { "Name" }, AggregatedProperties = new string[] { "EnableRecordingTagName", "RecordingTagName", "ResettingTagName" })]
    [DeferredDeletion(false)]
    public class DataLoggerSettings : XPObject, IDataErrorInfo, INotifyPropertyVisibilityChanged
#if !NET_STANDARD
        , IUndoRedoXpo
#endif
    {
        #region Constructors
        /// <summary>
        /// Initializes the XPObject by passing the session.
        /// </summary>
        /// <param name="session"></param>
        public DataLoggerSettings(Session session)
            : base(session)
        {
        }
        #endregion

        #region Properties Default Values
        // List of constant default values for each property where you want handle a default value.
        const bool defaultEnable = true;
        const int defaultMaxLength = 255;
        const uint defaultMaxTransactionsBeforeCommit = 10;
        const byte defaultMaxErrorBeforeFlush = 3;
        const uint defaultWaitBeforeRetry = 10;
        const uint defaultMaxCacheSize = 10000;

        /// <summary>
        /// Adds inside this method the nullable property where you want handle a default value.
        /// </summary>
        private void EnsureDefaultValues()
        {
            // Examples of how to handle a default value
            //if (!PropertyName.HasValue)
            //    PropertyName = defaultPropertyName;
            //if (TimeSpanPropertyName == TimeSpan.Zero)
            //    TimeSpanPropertyName = TimeSpan.FromMinutes(1);
            //if (DateTimePropertyName == DateTime.MinValue)
            //    DateTimePropertyName = DateTime.UtcNow;

            if (!Enable.HasValue)
                Enable = defaultEnable;
            if (!MaxAge.HasValue)
                MaxAge = TimeSpan.FromDays(1.0);
            if (!MaxLength.HasValue)
                MaxLength = defaultMaxLength;
            if (!MaxTransactionsBeforeCommit.HasValue)
                MaxTransactionsBeforeCommit = defaultMaxTransactionsBeforeCommit;
            if (!MaxErrorBeforeFlush.HasValue)
                MaxErrorBeforeFlush = defaultMaxErrorBeforeFlush;
            if (!WaitBeforeRetry.HasValue)
                WaitBeforeRetry = defaultWaitBeforeRetry;
            if (!MaxCacheSize.HasValue)
                MaxCacheSize = defaultMaxCacheSize;
        }
        #endregion

        #region Public methods
        public string GetDataloggerName()
        {
            return Name;
        }
        #endregion

        #region Not Persistance Properties
        /// <summary>
        /// Return true if the datalogger can be used for recording values.
        /// </summary>
        [NonPersistent]
        [Browsable(false)]
        public bool IsValid
        {
            get
            {
                if (Columns == null)
                    return false;

                var columns = (from c in Columns where c.IsValid select c).ToList();
                return !String.IsNullOrEmpty(Name) && (RecordingTimeInterval != TimeSpan.Zero || (RecordingTag != null && !RecordingTag.IsEmpty()) || RecordOnDataChange) && columns.Count > 0 
                    && RecordingTimeInterval.TotalMilliseconds < UInt32.MaxValue - 2;
            }
        }

        [NonPersistent]
        [Browsable(false)]
        public bool IsRunningInstance { get; set; }

        /// <summary>
        /// Get or Set the connection string in a suitable format.
        /// </summary>
        [NonPersistent]
        [Browsable(false)]
        [Exportable]
        public String ExportableConnectionString
        {
            get
            {
                if (ConnectionSettings == null || String.IsNullOrEmpty(ConnectionSettings.DataProvider) || String.IsNullOrEmpty(ConnectionSettings.Connection))
                    return String.Empty;

                return String.Format("DataProvider={0};{1}", ConnectionSettings.DataProvider, ConnectionSettings.Connection);
            }
            set
            {
                if (ConnectionSettings == null)
                    ConnectionSettings = new DataReaderModel();

                if (String.IsNullOrEmpty(value))
                {
                    ConnectionSettings.Connection =
                        ConnectionSettings.DataProvider =
                        ConnectionSettings.DataProviderDisplayName =
                        ConnectionSettings.DataProviderDescription =
                        ConnectionSettings.DataProviderShortDisplayName =
                        ConnectionSettings.DataSourceName =
                        ConnectionSettings.DataSourceDisplayName = String.Empty;
                }
                else
                {
                    ConnectionStringParser helper = new ConnectionStringParser(value);
                    string dataProvider;
                    string connectionString;
                    dataProvider = helper.GetPartByName("DataProvider");
                    helper.RemovePartByName("DataProvider");
                    connectionString = helper.GetConnectionString();

                    if (ConnectionSettings.Connection != connectionString)
                        ConnectionSettings.Connection = connectionString;
                    if (ConnectionSettings.DataProvider != dataProvider)
                        ConnectionSettings.DataProvider = dataProvider;
                }

                RaisePropertyChangedEvent("ConnectionSettings");
            }
        }

        /// <summary>
        /// Get or Set the connection string in a suitable format.
        /// </summary>
        [NonPersistent]
        [Browsable(false)]
        public String ReadableConnectionString
        {
            get
            {
                if (ConnectionSettings == null || String.IsNullOrEmpty(ConnectionSettings.DataProvider) || String.IsNullOrEmpty(ConnectionSettings.Connection))
                    return String.Empty;

                return String.Format("DataProvider={0};{1}", String.IsNullOrEmpty(ConnectionSettings.DataProviderDisplayName) ? ConnectionSettings.DataProvider : ConnectionSettings.DataProviderDisplayName, ConnectionSettings.Connection);
            }
            set
            {
                if (ConnectionSettings == null)
                    ConnectionSettings = new DataReaderModel();

                if (String.IsNullOrEmpty(value))
                {
                    ConnectionSettings.Connection =
                        ConnectionSettings.DataProvider =
                        ConnectionSettings.DataProviderDisplayName =
                        ConnectionSettings.DataProviderDescription =
                        ConnectionSettings.DataProviderShortDisplayName =
                        ConnectionSettings.DataSourceName =
                        ConnectionSettings.DataSourceDisplayName = String.Empty;
                }
                else
                {
                    ConnectionStringParser helper = new ConnectionStringParser(value);
                    //var dataProvider = helper.GetPartByName("DataProvider");
                    //if (DataProvider != dataProvider)
                    //{
                    //    DataProvider = 
                    //        DataProviderDisplayName =
                    //        DataProviderShortDisplayName = 
                    //        DataProviderDescription = dataProvider;
                    //}

                    helper.RemovePartByName("DataProvider");
                    var connectionString = helper.GetConnectionString();
                    if (ConnectionSettings.Connection != connectionString)
                        ConnectionSettings.Connection = connectionString;
                }

                RaisePropertyChangedEvent("ConnectionSettings");
            }
        }

        /// <summary>
        /// Get total number of columns in the datalogger.
        /// </summary>
        [NonPersistent]
        [Browsable(false)]
        public int NumColumns
        {
            get
            {
                if (Columns == null)
                    return 0;

                return Columns.Count;
            }
        }
        [Browsable(false)]
        [NonPersistent]
        [Exportable]
        public String EnableRecordingTagName
        {
            get
            {
                if (EnableRecordingTag != null)
                    return $"{EnableRecordingTag.StringRepresentation}";
                return string.Empty;
            }
        }
        [Browsable(false)]
        [NonPersistent]
        [Exportable]
        public String RecordingTagName
        {
            get
            {
                if (RecordingTag != null)
                    return $"{RecordingTag.StringRepresentation}";
                return string.Empty;
            }
        }
        [Browsable(false)]
        [NonPersistent]
        [Exportable]
        public String ResettingTagName
        {
            get
            {
                if (ResettingTag != null)
                    return $"{ResettingTag.StringRepresentation}";
                return string.Empty;
            }
        }
        #endregion

        #region Properties
        private string _Name;
        /// <summary>
        /// The name used to define the data logger.
        /// </summary>
        /// <remarks>
        /// The name is unique for each data logger in the collection.
        /// </remarks>
        [Indexed(Unique = false)]
        [MergablePropertyAttribute(false)]
        [Exportable]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                SetPropertyValue("Name", ref _Name, value);
            }
        }

        private bool? _Enable;
        /// <summary>
        /// Allow to enable or disable the operations over this data logger
        /// </summary>
        [Exportable]
        public bool? Enable
        {
            get
            {
                return _Enable;
            }
            set
            {
                SetPropertyValue("Enable", ref _Enable, value);
            }
        }

        private bool _EnableDataProtection;
        /// <summary>
        /// Allow to enable or disable the data protection manager over this data logger
        /// </summary>
        [Exportable]
        public bool EnableDataProtection
        {
            get
            {
                return _EnableDataProtection;
            }
            set
            {
                if (SetPropertyValue("EnableDataProtection", ref _EnableDataProtection, value))
                {
                    RaisePropertyChangedEvent("ConnectionSettings");
                    OnPropertyVisiblityChanged("EnableDataProtection");
                }
            }
        }

        private bool _UseAggregatedTables;
        /// <summary>
        /// Allow to enable or disable the use of aggregated tables over this data logger
        /// </summary>
        [Exportable]
        public bool UseAggregatedTables
        {
            get
            {
                return _UseAggregatedTables;
            }
            set
            {
                SetPropertyValue("UseAggregatedTables", ref _UseAggregatedTables, value);
            }
        }

        private QualityCheckType _QualityCheck;
        /// <summary>
        /// The type of quality check chosen for the data logger
        /// </summary>
        [Exportable]
        public QualityCheckType QualityCheck
        {
            get
            {
                return _QualityCheck;
            }
            set
            {
                SetPropertyValue("QualityCheck", ref _QualityCheck, value);
            }
        }

        private string _TableName;
        /// <summary>
        /// The name used as table name in the database.
        /// </summary>
        /// <remarks>
        /// The name is optional. An empty or null value force the data logger to use itself name as table name.
        /// </remarks>
        [Exportable]
        public string TableName
        {
            get
            {
                return _TableName;
            }
            set
            {
                SetPropertyValue("TableName", ref _TableName, value);
            }
        }

        private string _UtcTimeColumnName;
        /// <summary>
        /// The column name used for recording the date and time expressed as the UTC.
        /// </summary>
        /// <remarks>
        /// The name is optional. An empty or null value force the data logger to use a default value.
        /// </remarks>
        [Exportable]
        public string UtcTimeColumnName
        {
            get
            {
                return _UtcTimeColumnName;
            }
            set
            {
                SetPropertyValue("UtcTimeColumnName", ref _UtcTimeColumnName, value);
            }
        }

        private string _LocalTimeColumnName;
        /// <summary>
        /// The column name used for recording the date and time expressed as local time
        /// </summary>
        /// <remarks>
        /// The name is optional. An empty or null value force the data logger to use a default value.
        /// </remarks>
        [Exportable]
        public string LocalTimeColumnName
        {
            get
            {
                return _LocalTimeColumnName;
            }
            set
            {
                SetPropertyValue("LocalTimeColumnName", ref _LocalTimeColumnName, value);
            }
        }

        private string _MillisecondsColumnName;
        /// <summary>
        /// The column name used for recording the milliseconds.
        /// </summary>
        /// <remarks>
        /// The name is optional. An empty or null value force the data logger to use a default value.
        /// </remarks>
        [Exportable]
        public string MillisecondsColumnName
        {
            get
            {
                return _MillisecondsColumnName;
            }
            set
            {
                SetPropertyValue("MillisecondsColumnName", ref _MillisecondsColumnName, value);
            }
        }

        private string _UserColumnName;
        /// <summary>
        /// The column name used for recording the user who performs the recording action.
        /// </summary>
        /// <remarks>
        /// The name is optional. An empty or null value force the data logger to use a default value.
        /// </remarks>
        [Exportable]
        public string UserColumnName
        {
            get
            {
                return _UserColumnName;
            }
            set
            {
                SetPropertyValue("UserColumnName", ref _UserColumnName, value);
            }
        }

        private string _ReasonColumnName;
        /// <summary>
        /// The column name used for recording the reason who performs the recording action.
        /// </summary>
        /// <remarks>
        /// The name is optional. An empty or null value force the data logger to use a default value.
        /// </remarks>
        [Exportable]
        public string ReasonColumnName
        {
            get
            {
                return _ReasonColumnName;
            }
            set
            {
                SetPropertyValue("ReasonColumnName", ref _ReasonColumnName, value);
            }
        }

        private UFUAModel.TagEntityReference _EnableRecordingTag;
        /// <summary>
        /// The UFUATag to use for enabling the recording of new records.
        /// </summary>
        /// <remarks>
        /// The recording of new records is disable when this tag's value is zero or false.
        /// </remarks>
        [ValueConverter(typeof(UFUAModel.ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public UFUAModel.TagEntityReference EnableRecordingTag
        {
            get
            {
                return _EnableRecordingTag;
            }
            set
            {
                SetPropertyValue("EnableRecordingTag", ref _EnableRecordingTag, value);
            }
        }

        private UFUAModel.TagEntityReference _RecordingTag;
        /// <summary>
        /// The UFUATag to use for insert a new record inside table.
        /// </summary>
        /// <remarks>
        /// The tag is automatically reset after the operation has been performed.
        /// </remarks>
        [ValueConverter(typeof(UFUAModel.ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public UFUAModel.TagEntityReference RecordingTag
        {
            get
            {
                return _RecordingTag;
            }
            set
            {
                SetPropertyValue("RecordingTag", ref _RecordingTag, value);
            }
        }

        private UFUAModel.TagEntityReference _ResettingTag;
        /// <summary>
        /// The UFUATag to use for delete all records from table.
        /// </summary>
        /// <remarks>
        /// The tag is automatically reset after the operation has been performed.
        /// </remarks>
        [ValueConverter(typeof(UFUAModel.ConvertTagEntityReference))]
        [Size(SizeAttribute.Unlimited)]
        public UFUAModel.TagEntityReference ResettingTag
        {
            get
            {
                return _ResettingTag;
            }
            set
            {
                SetPropertyValue("ResettingTag", ref _ResettingTag, value);
            }
        }

        private TimeSpan _RecordingTimeInterval;
        /// <summary>
        /// The time interval to use for adding new data.
        /// </summary>
        /// <remarks>
        /// A time interval set to TimeSpan.Zero disable the recording on time.
        /// </remarks>
        [Exportable]
        public TimeSpan RecordingTimeInterval
        {
            get
            {
                return _RecordingTimeInterval;
            }
            set
            {
                SetPropertyValue("RecordingTimeInterval", ref _RecordingTimeInterval, value);
            }
        }

        private bool _RecordOnDataChange;
        /// <summary>
        /// Allow to enable or disable the record on data change.
        /// </summary>
        [Exportable]
        public bool RecordOnDataChange
        {
            get
            {
                return _RecordOnDataChange;
            }
            set
            {
                if (SetPropertyValue("RecordOnDataChange", ref _RecordOnDataChange, value))
                {
                    OnPropertyVisiblityChanged("RecordOnDataChange");
                }
            }
        }

        /// <summary>
        /// The time interval to use like hysteresis.
        /// </summary>
        /// <remarks>
        /// A time interval set to TimeSpan.Zero disable the hysteresis.
        /// </remarks>
        private TimeSpan _HysteresisTimeInterval;
        [Exportable]
        public TimeSpan HysteresisTimeInterval
        {
            get
            {
                return _HysteresisTimeInterval;
            }
            set
            {
                SetPropertyValue("HysteresisTimeInterval", ref _HysteresisTimeInterval, value);
            }
        }

        private TimeSpan? _MaxAge;
        /// <summary>
        /// The max age of records to keep in the table.
        /// </summary>
        [Exportable]
        public TimeSpan? MaxAge
        {
            get
            {
                return _MaxAge;
            }
            set
            {
                SetPropertyValue("MaxAge", ref _MaxAge, value);
            }
        }

        //private long _MaxRecords = 1000;
        ///// <summary>
        ///// The max numbers of records to keep in the table
        ///// </summary>
        //public long MaxRecords
        //{
        //    get
        //    {
        //        return _MaxRecords;
        //    }
        //    set
        //    {
        //        SetPropertyValue("MaxRecords", ref _MaxRecords, value);
        //    }
        //}

        private DataReaderModel _ConnectionSettings;
        /// <summary>
        /// The connection setting informations used for connecting to database.
        /// </summary>
        [ValueConverter(typeof(ConvertDataReaderModel))]
        [Size(SizeAttribute.Unlimited)]
        public DataReaderModel ConnectionSettings
        {
            get
            {
                return _ConnectionSettings;
            }
            set 
            {
                if (SetPropertyValue("ConnectionSettings", ref _ConnectionSettings, value))
                    RaisePropertyChangedEvent("ReadableConnectionString");
            }
        }
        //private String _FlushDataSafelyPath;
        ///// <summary>
        ///// The path to use for flush data safely to disk.
        ///// </summary>
        //[Size(SizeAttribute.Unlimited)]
        //public String FlushDataSafelyPath
        //{
        //    get
        //    {
        //        return _FlushDataSafelyPath;
        //    }
        //    set
        //    {
        //        SetPropertyValue("FlushDataSafelyPath", ref _FlushDataSafelyPath, value);
        //    }
        //}

        private int? _MaxLength;
        /// <summary>
        /// The max lenght of fixed lenght string dbType
        /// </summary>
        [Exportable]
        public int? MaxLength
        {
            get
            {
                return _MaxLength;
            }
            set
            {
                SetPropertyValue("MaxLength", ref _MaxLength, value);
            }
        }

        /// <summary>
        /// The max number of transactions before commit changes to database.
        /// </summary>
        private uint? _MaxTransactionsBeforeCommit;
        [Exportable]
        public uint? MaxTransactionsBeforeCommit
        {
            get
            {
                return _MaxTransactionsBeforeCommit;
            }
            set
            {
                SetPropertyValue("MaxTransactionsBeforeCommit", ref _MaxTransactionsBeforeCommit, value);
            }
        }

        /// <summary>
        /// The max number of errors before flush safely to file.
        /// </summary>
        private byte? _MaxErrorBeforeFlush;
        [Exportable]
        public byte? MaxErrorBeforeFlush
        {
            get
            {
                return _MaxErrorBeforeFlush;
            }
            set
            {
                SetPropertyValue("MaxErrorBeforeFlush", ref _MaxErrorBeforeFlush, value);
            }
        }

        /// <summary>
        /// The time in seconds to wait before retry to write data.
        /// </summary>
        private uint? _WaitBeforeRetry;
        [Exportable]
        public uint? WaitBeforeRetry
        {
            get
            {
                return _WaitBeforeRetry;
            }
            set
            {
                SetPropertyValue("WaitBeforeRetry", ref _WaitBeforeRetry, value);
            }
        }

        /// <summary>
        /// The max number of pending records before flush safely to file.
        /// </summary>
        private uint? _MaxCacheSize;
        [Exportable]
        public uint? MaxCacheSize
        {
            get
            {
                return _MaxCacheSize;
            }
            set
            {
                SetPropertyValue("MaxCacheSize", ref _MaxCacheSize, value);
            }
        }

        private bool _AllowDuplicatedRows;
        /// <summary>
        /// Allow to enable or disable duplicated rows based on recording datetime.
        /// </summary>
        [Exportable]
        public bool AllowDuplicatedRows
        {
            get
            {
                return _AllowDuplicatedRows;
            }
            set
            {
                SetPropertyValue("AllowDuplicatedRows", ref _AllowDuplicatedRows, value);
            }
        }

        private bool _SkipCheckColumnsType;
        /// <summary>
        /// Allow to enable or disable the check comlumns table on startup.
        /// </summary>
        [Exportable]
        public bool SkipCheckColumnsType
        {
            get
            {
                return _SkipCheckColumnsType;
            }
            set
            {
                SetPropertyValue("SkipCheckColumnsType", ref _SkipCheckColumnsType, value);
            }
        }

        /// <summary>
        /// The list of columns handle by this data logger.
        /// </summary>
        [Association("DataLoggerSettings-Columns"), Aggregated]
        [Browsable(false)]
        public XPCollection<DataLoggerColumn> Columns
        {
            get
            {
                return GetCollection<DataLoggerColumn>("Columns");
            }
        }
        #endregion

        #region Override Methods

        public override void AfterConstruction()
        {
            base.AfterConstruction();

            EnsureDefaultValues();
        }

        protected override void OnLoaded()
        {
            base.OnLoaded();

            EnsureDefaultValues();
        }

        #endregion

        #region Private Methods
        protected String PerformValidation(String propertyName)
        {
            if (propertyName == "Name")
            {
                if (String.IsNullOrEmpty(Name))
                {
                    return Properties.Resources.DataLoggerSettingsNameEmpty;
                }
                else if (!DBNameValidator.IsValidName(Name))
                {
                    return Properties.Resources.DataLoggerSettingsNameInvalidChars;
                }
                else if ((from c in new XPQuery<DataLoggerModel.DataLoggerSettings>(Session, true)/*.AsParallel()*/
                     where c != this && c.Name == Name
                     select c).ToList().Count > 0)
                {
                    return Properties.Resources.DataLoggerSettingsNameAlreadyExists;
                }
            }
            else if (propertyName == "TableName")
            {
                if (!String.IsNullOrEmpty(TableName) && !DBNameValidator.IsValidTableName(TableName))
                {
                    return Properties.Resources.DataLoggerSettingsTableNameInvalidChars;
                }
            }
            else if (propertyName == "UtcTimeColumnName")
            {
                if (!String.IsNullOrEmpty(UtcTimeColumnName) && !DBNameValidator.IsValidName(UtcTimeColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameInvalidChars;
                }
                else if (DataLoggerSettingsHelper.ReservedColumnNames.ToList().Contains(UtcTimeColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameReservedName;
                }
            }
            else if (propertyName == "LocalTimeColumnName")
            {
                if (!String.IsNullOrEmpty(LocalTimeColumnName) && !DBNameValidator.IsValidName(LocalTimeColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameInvalidChars;
                }
                else if (DataLoggerSettingsHelper.ReservedColumnNames.ToList().Contains(LocalTimeColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameReservedName;
                }
            }
            else if (propertyName == "MillisecondsColumnName")
            {
                if (!String.IsNullOrEmpty(MillisecondsColumnName) && !DBNameValidator.IsValidName(MillisecondsColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameInvalidChars;
                }
                else if (DataLoggerSettingsHelper.ReservedColumnNames.ToList().Contains(MillisecondsColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameReservedName;
                }
            }
            else if (propertyName == "UserColumnName")
            {
                if (!String.IsNullOrEmpty(UserColumnName) && !DBNameValidator.IsValidName(UserColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameInvalidChars;
                }
                else if (DataLoggerSettingsHelper.ReservedColumnNames.ToList().Contains(UserColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameReservedName;
                }
            }
            else if (propertyName == "ReasonColumnName")
            {
                if (!String.IsNullOrEmpty(ReasonColumnName) && !DBNameValidator.IsValidName(ReasonColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameInvalidChars;
                }
                else if (DataLoggerSettingsHelper.ReservedColumnNames.ToList().Contains(ReasonColumnName))
                {
                    return Properties.Resources.DataLoggerColumnNameReservedName;
                }
            }
            else if (propertyName == "EnableRecordingTag")
            {
                if (EnableRecordingTag != null && !EnableRecordingTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == EnableRecordingTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagEntityReferenceNotFound;
                }
            }
            else if (propertyName == "RecordingTag")
            {
                if (RecordingTag != null && !RecordingTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == RecordingTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagEntityReferenceNotFound;
                }
            }
            else if (propertyName == "ResettingTag")
            {
                if (ResettingTag != null && !ResettingTag.IsEmpty())
                {
                    var listfound = (from tag in new XPQuery<UFUAModel.UFUATag>(Session, true)/*.AsParallel()*/
                                     where tag.NodeId == ResettingTag.Guid
                                     select tag).ToList();
                    if (listfound.Count == 0)
                        return Properties.Resources.TagEntityReferenceNotFound;
                }
            }
            else if (propertyName == "ConnectionSettings" ||
                propertyName == "ReadableConnectionString")
            {
                if (String.IsNullOrEmpty(ReadableConnectionString))
                {
                    var configuration = (from tag in new XPQuery<UFUAModel.UFUAConfiguration>(Session, true).AsParallel() select tag).FirstOrDefault();
                    if (configuration != null)
                    {
                        if (!XpoConversionHelper.IsDotNetConvertible(configuration.HistorianDefaultConnection))
                            return Properties.Resources.ConnectionStringRequired;
                        else if (EnableDataProtection && !XpoHelpers.XpoHelper.IsMSSQlDataProvider(configuration.HistorianDefaultConnection))
                            return UFUAModel.Properties.Resources.DataProtectionXpoProviderNotSupported;
                    }
                }
                else if (EnableDataProtection &&
                    ConnectionSettings.DataProvider !=
#if !NET_STANDARD
                    Microsoft.Data.ConnectionUI.DataProvider.SqlDataProvider.Name
#else
                    "System.Data.SqlClient"
#endif
                    )
                {
                    return Properties.Resources.DataProtectionProviderNotSupported;
                }
            }
            else if (propertyName == "RecordingTimeInterval")
            {
                if (RecordingTimeInterval.TotalMilliseconds > UInt32.MaxValue - 2)
                {
                    TimeSpan t = TimeSpan.FromMilliseconds(UInt32.MaxValue - 2);
                    return String.Format(Properties.Resources.DataLoggerMaxRecordingTimeInterval, t.Days, t.Hours, t.Minutes, t.Seconds, t.Milliseconds);
                }
            }
            //else if (propertyName == "FlushDataSafelyPath")
            //{
            //    if (!String.IsNullOrEmpty(FlushDataSafelyPath) && !DirectoryHelper.IsValidDirectory(FlushDataSafelyPath, false))
            //    {
            //        return Properties.Resources.DataLoggerSettingsFlushPathInvalid;
            //    }
            //}

            return null;
        }
#endregion

#if !NET_STANDARD
#region IUndoRedoXpo
        [Browsable(false)]
        [NonPersistent]
        public String PathIdentifier
        {
            get
            {
                return Name;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String UniqueIdentifier
        {
            get
            {
                return Name;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String ParentIdentifier
        {
            get
            {
                return String.Empty;
            }
        }

        [Browsable(false)]
        [NonPersistent]
        public String OwnerIdentifier
        {
            get
            {
                return String.Empty;
            }
        }
#endregion
#endif

#region IDataErrorInfo
        [Browsable(false)]
        public string Error
        {
            get
            {
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null);
                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();

                return !System.ComponentModel.DataAnnotations.Validator.TryValidateObject(this, context, results)
                    ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                    : null;
            }
        }

        public string this[string propertyName]
        {
            get
            {
                String s = PerformValidation(propertyName);
                if (!String.IsNullOrEmpty(s))
                    return s;
                var context = new System.ComponentModel.DataAnnotations.ValidationContext(this, null, null)
                {
                    MemberName = propertyName
                };

                var results = new List<System.ComponentModel.DataAnnotations.ValidationResult>();
                var propertyInfo = GetType().GetProperty(propertyName);
                if (propertyInfo != null)
                {
                    var value = propertyInfo.GetValue(this, null);

                    return !System.ComponentModel.DataAnnotations.Validator.TryValidateProperty(value, context, results)
                        ? string.Join(Environment.NewLine, results.Select(x => x.ErrorMessage))
                        : null;
                }

                return null;
            }
        }
#endregion

#region INotifyPropertyVisibilityChanged Members

        /// <summary>
        /// Gets the visibility state for the property with the given name.
        /// </summary>
        /// <param name="propertyName">The property name that you want konw the current visibility state.</param>
        /// <returns></returns>
        bool INotifyPropertyVisibilityChanged.this[string propertyName]
        {
            get
            {
                if (propertyName == "HysteresisTimeInterval")
                {
                    return RecordOnDataChange;
                }

                return true;
            }
        }

        /// <summary>
        /// Raised when a property visibility state on this object has a new value.
        /// </summary>
        public event PropertyChangedEventHandler PropertyVisiblityChanged;

        /// <summary>
        /// Raises this object's PropertyVisiblityChanged event.
        /// </summary>
        /// <param name="propertyName">The property that has changed his value and has triggered the change of visibility.</param>
        protected void OnPropertyVisiblityChanged(string propertyName)
        {
            PropertyChangedEventHandler handler = PropertyVisiblityChanged;
            if (handler != null)
            {
                var e = new PropertyChangedEventArgs(propertyName);
                handler(this, e);
            }
        }

#endregion
    }
}
