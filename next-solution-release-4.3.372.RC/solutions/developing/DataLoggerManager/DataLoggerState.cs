using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DataLoggerModel;
using DataLoggerModel.Helpers;
using Utilities;

namespace DataLoggerManager
{
    /// <summary>
    /// Memory mapped object of a data logger state used for retriving the memory data to write in database
    /// </summary>
    internal class DataLoggerState : IDisposable
    {
        #region Declarations
        readonly Object lockObject = new Object();
        readonly int redundancyServerId;
        #endregion

        #region Constructors
        /// <summary>
        /// Initialize a new instance of DataLoggerEntity by passing the data logger configuration settings
        /// </summary>
        /// <param name="settings"></param>
        public DataLoggerState(DataLoggerSettings settings, string projectRoot, int redundancyId)
        {
            dataLoggerSettingsHelper = new DataLoggerSettingsHelper(settings, projectRoot);
            redundancyServerId = redundancyId;
            dataLoggerDataSet = CreateDataSet(settings);
            NeedToDeleteOldData = true;
            EnableFlag = true;
            lastTimeError = DateTime.MinValue;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Get the data logger configuration settings used for initializing this instance
        /// </summary>
        public DataLoggerSettings DataLoggerSettings
        {
            get
            {
                return dataLoggerSettingsHelper.DataLoggerSettings;
            }
        }

        /// <summary>
        /// Get the data provider of the data logger configuration settings used for initializing this instance
        /// </summary>
        public String DataProvider
        {
            get
            {
                return dataLoggerSettingsHelper.DataLoggerSettings.ConnectionSettings.DataProvider;
            }
        }

        /// <summary>
        /// Get the connection of the data logger configuration settings used for initializing this instance
        /// </summary>
        public String Connection
        {
            get
            {
                return dataLoggerSettingsHelper.DataLoggerSettings.ConnectionSettings.Connection;
            }
        }

        /// <summary>
        /// Get the table name of this instance of data logger state.
        /// </summary>
        public String TableName
        {
            get
            {
                return dataLoggerSettingsHelper.TableName;
            }
        }

        /// <summary>
        /// Get an array with all the valid column name of this instance of data logger state.
        /// </summary>
        public String[] ValidColumnNames
        {
            get
            {
                return dataLoggerSettingsHelper.GetValidColumnNames();
            }
        }

        /// <summary>
        /// Get an array with all the index column name of this instance of data logger state.
        /// </summary>
        public String[] IndexColumns
        {
            get
            {
                return dataLoggerSettingsHelper.IndexColumns;
            }
        }

        /// <summary>
        /// Get the UTC column name of this instance of data logger state.
        /// </summary>
        public String UtcTimeColumnName
        {
            get
            {
                return dataLoggerSettingsHelper.UtcTimeColumnName;
            }
        }

        /// <summary>
        /// Get the local time column name of this instance of data logger state.
        /// </summary>
        public String LocalTimeColumnName
        {
            get
            {
                return dataLoggerSettingsHelper.LocalTimeColumnName;
            }
        }

        /// <summary>
        /// Get the Milliseconds column name of this instance of data logger state.
        /// </summary>
        public String MillisecondsColumnName
        {
            get
            {
                return dataLoggerSettingsHelper.MillisecondsColumnName;
            }
        }

        /// <summary>
        /// Get the Redundancy column name of this instance of data logger state.
        /// </summary>
        public String RedundancyColumnName
        {
            get
            {
                return dataLoggerSettingsHelper.RedundancyColumnName;
            }
        }

        /// <summary>
        /// Get the auto-increment column name of this instance of data logger state.
        /// </summary>
        public String AutoIncrementColumnName
        {
            get
            {
                return dataLoggerSettingsHelper.AutoIncrementColumnName;
            }
        }

        /// <summary>
        /// Get the DataSet schema based on the data logger configuration settings used for initializing this instance
        /// </summary>
        /// Implemented in a thread safe way
        /// </reremarks>
        /// <remarks>
        /// DataSet won't contains any records
        /// </remarks>
        public DataSet DataLoggerDataSetSchema
        {
            get
            {
                lock (lockObject)
                {
                    return dataLoggerDataSet.Clone();
                }
            }
        }

        /// <summary>
        /// Get the total number of rows in the DataSet instance.
        /// </summary>
        /// <reremarks>
        /// Implemented in a thread safe way
        /// </reremarks>
        public int DataSetSize
        {
            get
            {
                lock (lockObject)
                {
                    DataView dataView = new DataView(dataLoggerDataSet.Tables[0])
                    {
                        RowStateFilter = DataViewRowState.CurrentRows,
                    };

                    return dataView.Count;
                }
            }
        }

        /// <summary>
        /// Get a DataView instance of all added rows in the data logger
        /// </summary>
        /// <reremarks>
        /// Implemented in a thread safe way
        /// </reremarks>
        internal DataView FullAddedRows
        {
            get
            {
                lock (lockObject)
                {
                    DataView dataView = new DataView(dataLoggerDataSet.Tables[0])
                    {
                        RowStateFilter = DataViewRowState.Added,
                        Sort = String.Format("{0} ASC", dataLoggerSettingsHelper.UtcTimeColumnName)
                    };

                    return CopyDataView(dataView, true);
                }
            }
        }

        /// <summary>
        /// Get a DataView instance of all added rows in the data logger
        /// </summary>
        /// <reremarks>
        /// Implemented in a thread safe way
        /// </reremarks>
        public DataView AddedRows
        {
            get
            {
                lock (lockObject)
                {
                    DataView dataView = new DataView(dataLoggerDataSet.Tables[0])
                    {
                        RowStateFilter = DataViewRowState.Added,
                        Sort = String.Format("{0} ASC", dataLoggerSettingsHelper.UtcTimeColumnName)
                    };

                    return CopyDataView(dataView);
                }
            }
        }

        /// <summary>
        /// Get a DataView instance of all deleted rows in the data logger
        /// </summary>
        /// <reremarks>
        /// Implemented in a thread safe way
        /// </reremarks>
        public DataView DeletedRows
        {
            get
            {
                lock (lockObject)
                {
                    DataView dataView = new DataView(dataLoggerDataSet.Tables[0])
                    {
                        RowStateFilter = DataViewRowState.Deleted,
                        Sort = String.Format("{0} ASC", dataLoggerSettingsHelper.UtcTimeColumnName)
                    };

                    return CopyDataView(dataView);
                }
            }
        }

        /// <summary>
        /// Get a DataView instance of all updated rows in the data logger
        /// </summary>
        /// <reremarks>
        /// Implemented in a thread safe way
        /// </reremarks>
        public DataView UpdatedRows
        {
            get
            {
                lock (lockObject)
                {
                    DataView dataView = new DataView(dataLoggerDataSet.Tables[0])
                    {
                        RowStateFilter = DataViewRowState.ModifiedCurrent,
                        Sort = String.Format("{0} ASC", dataLoggerSettingsHelper.UtcTimeColumnName)
                    };

                    return CopyDataView(dataView);
                }
            }
        }

        /// <summary>
        /// Check if the data logger settings are valid.
        /// </summary>
        public bool ValidFlag
        {
            get
            {
                return dataLoggerSettingsHelper.IsValidSettings;
            }
        }

        bool enableFlag;
        /// <summary>
        /// Get currently enable flag state of the this instance of data logger.
        /// </summary>
        public bool EnableFlag
        {
            get
            {
                return enableFlag;
            }
            set
            {
                if (enableFlag == value)
                    return;
                enableFlag = value;
            }
        }

        bool checkedFlag;
        /// <summary>
        /// Get currently checked flag state of the this instance of data logger.
        /// </summary>
        public bool CheckedFlag
        {
            get
            {
                return checkedFlag;
            }
            set
            {
                if (checkedFlag == value)
                    return;
                checkedFlag = value;
            }
        }

        bool checkingFlag;
        /// <summary>
        /// Get currently checking flag state of the this instance of data logger.
        /// </summary>
        public bool CheckingFlag
        {
            get
            {
                return checkingFlag;
            }
            set
            {
                if (checkingFlag == value)
                    return;
                checkingFlag = value;
            }
        }

        private bool errorFlag;
        /// <summary>
        /// Get currently error flag state of the this instance of data logger.
        /// </summary>
        public bool ErrorFlag
        {
            get
            {
                return errorFlag;
            }
        }

        bool resetFlag;
        /// <summary>
        /// Get currently reset flag state of the this instance of data logger.
        /// </summary>
        public bool ResetFlag
        {
            get
            {
                return resetFlag;
            }
        }

        private DateTime lastTimeReset;
        /// <summary>
        /// Get the date time to use for reseting command.
        /// </summary>
        public DateTime LastTimeReset
        {
            get
            {
                return lastTimeReset;
            }
        }

        private DateTime nextTimeDelete;
        /// <summary>
        /// Get the next date time that who this instance of data logger need to try to delete old max age data.
        /// </summary>
        public DateTime NextTimeDelete
        {
            get
            {
                return nextTimeDelete;
            }
        }

        /// <summary>
        /// Get a value to indicate if the old max ages data must be deleted.
        /// </summary>
        /// <returns></returns>
        public bool NeedToDeleteOldData
        {
            get
            {
                if (DataLoggerSettings.MaxAge.Value == TimeSpan.Zero)
                    return false;

                lock (lockObject)
                {
                    if (nextTimeDelete == DateTime.MinValue)
                        return false;

                    return nextTimeDelete < DateTime.UtcNow;
                }
            }
            internal set
            {
                if (DataLoggerSettings.MaxAge.Value == TimeSpan.Zero)
                    return;

                lock (lockObject)
                {
                    if (value)
                        nextTimeDelete = DateTime.UtcNow;
                    else
                    {
                        // randomizing the deletion of the records for avoiding too many concurrencies
                        TimeSpan span;
                        var rnd = new Random();
                        if (DataLoggerSettings.MaxAge.Value.Days > 0)
                            span = TimeSpan.FromHours(1) + TimeSpan.FromMinutes((double)rnd.Next(30));
                        else if (DataLoggerSettings.MaxAge.Value.Hours > 0)
                            span = TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds((double)rnd.Next(30));
                        else if (DataLoggerSettings.MaxAge.Value.Minutes > 0)
                            span = TimeSpan.FromMinutes(1) + TimeSpan.FromSeconds((double)rnd.Next(30));
                        else
                            span = DataLoggerSettings.MaxAge.Value + TimeSpan.FromMilliseconds((double)rnd.Next(500)); ;

                        nextTimeDelete = DateTime.UtcNow + span;
                    }
                }
            }
        }

        private uint writeErrorCounter;
        /// <summary>
        /// Get or Set the error counter of this instance of data logger.
        /// </summary>
        public uint WriteErrorCounter
        {
            get
            {
                return writeErrorCounter;
            }
            internal set 
            {
                lock (lockObject)
                {
                    if (writeErrorCounter == value)
                        return;
                    writeErrorCounter = value;
                }
            }
        }

        private String lastErrorMessage;
        /// <summary>
        /// Get the last error message of this instance of data logger.
        /// </summary>
        public String LastErrorMessage
        {
            get
            {
                return lastErrorMessage;
            }
        }

        private DateTime lastTimeError;
        /// <summary>
        /// Get the last date time in utc format, who this instance of data logger went in error.
        /// </summary>
        public DateTime LastTimeError
        {
            get
            {
                return lastTimeError;
            }
        }

        /// <summary>
        /// Get the information who the time to wait before retry to query the database has been elapsed.
        /// </summary>
        public bool RetryTimeElapsed
        {
            get
            {
                if (lastTimeError == DateTime.MinValue)
                    return true;

                return DateTime.UtcNow >= (lastTimeError + TimeSpan.FromSeconds(DataLoggerSettings.WaitBeforeRetry.Value));
            }
            internal set 
            {
                lock (lockObject)
                {
                    lastTimeError = DateTime.UtcNow;
                }
            }
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Add a new entry in the DataSet linked to this instance of Data Logger state.
        /// </summary>
        /// <param name="entity"></param>
        /// <reremarks>
        /// Implemented in a thread safe way
        /// </reremarks>
        public void AddNewEntry(DataLoggerEntity entity)
        {
            lock (lockObject)
            {
                DataView view = new DataView(dataLoggerDataSet.Tables[0]);
                DataRowView rowView = view.AddNew();
                rowView.BeginEdit();
                if (entity.recordingTime != DateTime.MinValue)
                {
                    rowView.Row[dataLoggerSettingsHelper.UtcTimeColumnName] = entity.recordingTime;
                    rowView.Row[dataLoggerSettingsHelper.LocalTimeColumnName] = entity.recordingTime.ToLocalTime();
                    rowView.Row[dataLoggerSettingsHelper.MillisecondsColumnName] = entity.recordingTime.Millisecond;
                    rowView.Row[dataLoggerSettingsHelper.UserColumnName] = entity.userName ?? String.Empty;
                    rowView.Row[dataLoggerSettingsHelper.ReasonColumnName] = DataLoggerEntity.GetReason(entity.recordingType);
                }
                if (redundancyServerId >= 0)
                    rowView.Row[dataLoggerSettingsHelper.RedundancyColumnName] = DateTime.UtcNow;
                foreach (var entityColumn in entity.listDataColumnEntities)
                {
                    rowView.Row[entityColumn.columnName] = 
                        entityColumn.columnValue.Value == null ? System.DBNull.Value : entityColumn.columnValue.Value;
                    // Set optional columns
                    var column = dataLoggerSettingsHelper.GetDataLoggerColumn(entityColumn.columnName);
                    if (entityColumn.statusCodeColumnEnabled)
                    {
                        var statusCode = entityColumn.columnValue.StatusCode == null ? System.DBNull.Value : entityColumn.columnValue.StatusCode;
                        rowView.Row[DataLoggerSettingsHelper.GetStatusCodeColumnName(column)] = statusCode;
                    }
                    if (entityColumn.serverTimeStampColumnEnabled)
                    {
                        var serverTimeStamp = entityColumn.columnValue.ServerTimeStamp == null ? System.DBNull.Value : entityColumn.columnValue.ServerTimeStamp;
                        if (!(serverTimeStamp is DateTime) || (DateTime)serverTimeStamp != DateTime.MinValue)
                            rowView.Row[DataLoggerSettingsHelper.GetServerTimeStampColumnName(column)] = serverTimeStamp;
                    }
                    if (entityColumn.sourceTimeStampColumnEnabled)
                    {
                        var sourceTimeStamp = entityColumn.columnValue.SourceTimeStamp == null ? System.DBNull.Value : entityColumn.columnValue.SourceTimeStamp;
                        if (!(sourceTimeStamp is DateTime) || (DateTime)sourceTimeStamp != DateTime.MinValue)
                            rowView.Row[DataLoggerSettingsHelper.GetSourceTimeStampColumnName(column)] = sourceTimeStamp;
                    }
                    if (entityColumn.userColumnEnabled)
                        rowView.Row[DataLoggerSettingsHelper.GetUserColumnName(column)] = entityColumn.userName ?? String.Empty;
                    if (entityColumn.stringValueColumnEnabled)
                        rowView.Row[DataLoggerSettingsHelper.GetStringValueColumnName(column)] = entityColumn.stringValue;
                }

                rowView.EndEdit();
            }
        }

        /// <summary>
        /// Add a set of rows in the DataSet linked to this instance of Data Logger state.
        /// </summary>
        /// <param name="dataView"></param>
        /// <reremarks>
        /// Implemented in a thread safe way
        /// </reremarks>
        public void AddRows(DataView dataView, bool bThrow = true)
        {
            lock (lockObject)
            {
                foreach (DataRowView rowView in dataView)
                {
                    try
                    {
                        dataLoggerDataSet.Tables[0].ImportRow(rowView.Row);
                    }
                    catch
                    {
                        if (bThrow)
                            throw;
                    }
                }
            }
        }

        /// <summary>
        /// Set or Reset the reset signal of this instance of data logger state.
        /// </summary>
        /// <param name="newvalue"></param>
        /// <returns>
        /// Return 'true' if the current reset state has been changed.
        /// </returns>
        public bool SetResetFlag(bool newvalue)
        {
            lock (lockObject)
            {
                bool currentlyFlag = resetFlag;
                resetFlag = newvalue;

                if (!newvalue)
                {
                    lastTimeReset = DateTime.MinValue;
                }
                else
                {
                    lastTimeReset = DateTime.UtcNow;
                }

                return currentlyFlag != resetFlag;
            }
        }

        /// <summary>
        /// Set or Reset the error signal of this instance of data logger state.
        /// </summary>
        /// <param name="newvalue"></param>
        /// <returns>
        /// Return 'true' if the current error state has been changed.
        /// </returns>
        public bool SetErrorFlag(bool newvalue, String lastError = null)
        {
            lock (lockObject)
            {
                bool currentlyFlag = errorFlag;
                errorFlag = newvalue;

                if (!newvalue)
                {
                    writeErrorCounter = 0;
                    lastTimeError = DateTime.MinValue;
                    lastErrorMessage = null;
                }
                else
                {
                    lastTimeError = DateTime.UtcNow;
                    if (!String.IsNullOrEmpty(lastError))
                        lastErrorMessage = lastError;
                }

                return currentlyFlag != errorFlag;
            }
        }
        #endregion

        #region Private Methods
        DataSet CreateDataSet(DataLoggerSettings settings)
        {
            var ds = new DataSet(settings.Name);
            ds.Tables.Add(dataLoggerSettingsHelper.CreateDataTable(redundancyServerId >= 0));
            
            return ds;
        }

        DataView CopyDataView(DataView dataView, bool allRecords = false)
        {
            DataView copyDataView = null;
            uint maxTransactions = dataLoggerSettingsHelper.DataLoggerSettings.MaxTransactionsBeforeCommit.Value;
            if (maxTransactions > 0)
            {
                copyDataView = new DataView(dataView.Table.Clone());
                while (dataView.Count > 0)
                {
                    if (!allRecords && copyDataView.Count >= maxTransactions)
                        break;

                    copyDataView.Table.ImportRow(dataView[0].Row);
                    dataView.Delete(0);
                }
            }
            
            return copyDataView;
        }
        #endregion

        #region Private Members
        readonly DataLoggerSettingsHelper dataLoggerSettingsHelper;
        DataSet dataLoggerDataSet;
        #endregion

        #region IDisposable Interface
        public void Dispose()
        {
            if (dataLoggerDataSet != null)
                dataLoggerDataSet.Dispose();
            dataLoggerDataSet = null;
        }
        #endregion

    }
}
