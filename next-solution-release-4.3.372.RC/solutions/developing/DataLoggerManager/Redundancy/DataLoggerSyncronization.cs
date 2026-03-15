using DataReader;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using System.Data;
using RedundancyHistory;
using DataReader.SchemaInfo;
using System.Threading;

namespace DataLoggerManager.Redundancy
{
    public class DataLoggerSyncronization : RedundancyHistory.RedundancyHistory
    {
        #region Declarations
        Dictionary<int, SyncDataTable> syncDataItems = new Dictionary<int, SyncDataTable>();
        readonly Dictionary<int, IList<String>> pendingUniqueIds = new Dictionary<int, IList<String>>();
        readonly Dictionary<int, DateTime> syncStartTimes = new Dictionary<int, DateTime>();

        readonly object lockObject = new object();

        static string defaultDateTimeFormat = "yyyy-MM-ddTHH:mm:ss";
        #endregion

        #region Constructors
        public DataLoggerSyncronization(int maxTake) : 
            base(maxTake)
        {
        }

        public DataLoggerSyncronization(int maxTake, int threadpool) :
            base(maxTake, threadpool)
        {
        }

        public DataLoggerSyncronization(int maxTake, int threadpool, ILog log) :
            base(maxTake, threadpool, log)
        {
        }
        #endregion

        #region Overrides
        protected override DateTime ReadSyncStartTime(string destinationConn)
        {
            DateTime startTime = DateTime.MinValue;
            var parameters = destinationConn.FromXml<DataLoggerModel.Helpers.DataLoggerInfo>();
            var dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(parameters.DataProvider, parameters.Connection);
            var select = String.Format("SELECT {0} FROM {1}",
                dbSchemaInfo.WrapObjectName(parameters.RedundancyColumnName),
                dbSchemaInfo.WrapObjectName(parameters.TableName));
            var sort = String.Format("{0} DESC", dbSchemaInfo.WrapObjectName(parameters.RedundancyColumnName));
            var where = String.Format("{0} IS NOT NULL", dbSchemaInfo.WrapObjectName(parameters.RedundancyColumnName));

            var list = DataReader.DataReader.GetDynamicSqlData(parameters.DataProvider,
                parameters.Connection, select, where, null, sort, maxTake: 1).ToList();
            if (list.Count > 0)
            {
                var map = list[0] as IDictionary<string, object>;
                startTime = (DateTime)map[parameters.RedundancyColumnName];
            }

            return startTime;
        }

        protected override int ReadSyncDataValues(int syncId, string sourceConn, DateTime startTime, DateTime endTime, int maxTake)
        {
            var parameters = sourceConn.FromXml<DataLoggerModel.Helpers.DataLoggerInfo>();
            var dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(parameters.DataProvider, parameters.Connection);
            var dateTimeFormat = GetDateTimeFormat(dbSchemaInfo);
            DateTime ageTime = DateTime.UtcNow - parameters.MaxAge;
            string select = String.Format("SELECT * FROM {0}", dbSchemaInfo.WrapObjectName(parameters.TableName));
            string sort = String.Format("{0} ASC", dbSchemaInfo.WrapObjectName(parameters.RedundancyColumnName));
            string where = String.Format("{0} IS NOT NULL AND {0} >= '{1}' AND {2} >= '{3}'",
                                        dbSchemaInfo.WrapObjectName(parameters.RedundancyColumnName),
                                        startTime.ToString(dateTimeFormat),
                                        dbSchemaInfo.WrapObjectName(parameters.UtcTimeColumnName),
                                        ageTime.ToString(dateTimeFormat));

            if (endTime != DateTime.MinValue)
                where += String.Format(" AND {0} < '{1}'",
                                        dbSchemaInfo.WrapObjectName(parameters.RedundancyColumnName),
                                        endTime.ToString(dateTimeFormat));

            var dataTable = DataReader.DataReader.ReadSqlData(parameters.DataProvider,
                parameters.Connection, select, where, null, sort, maxTake: maxTake);
            dataTable.TableName = parameters.TableName;

            lock (lockObject)
            {
                syncDataItems[syncId] = new SyncDataTable(dataTable, parameters);
            }

            return dataTable.Rows.Count;
        }

        protected override void ElaborateSyncDataValues(int syncId, int maxRecords)
        {
            SyncDataTable dataitems = null;
            lock (lockObject)
            {
                if (syncDataItems.ContainsKey(syncId))
                    dataitems = syncDataItems[syncId] as SyncDataTable;
            }

            var pendingIds = new List<String>();
            DateTime newStartTime = DateTime.MaxValue;
            if (dataitems != null/* && dataitems.DataTable.Rows.Count == maxRecords*/)
            {
                for (int ii = 0; ii < dataitems.DataTable.Rows.Count; ii++)
                {
                    DataRow row = dataitems.DataTable.Rows[ii];
                    DateTime datetime = (DateTime)row[dataitems.Parameters.RedundancyColumnName];
                    if (ii == maxRecords - 1)
                        newStartTime = datetime;

                    pendingIds.Add(GetUniqueId(row, dataitems.Parameters));
                }
            }

            lock (lockObject)
            {
                syncStartTimes[syncId] = newStartTime;
                pendingUniqueIds[syncId] = pendingIds;
            }
        }

        protected override DateTime GetNewSyncStartTime(int SyncId)
        {
            lock (lockObject)
            {
                if (syncStartTimes.ContainsKey(SyncId))
                    return syncStartTimes[SyncId];

                return DateTime.MinValue;
            }
        }

        protected override IList<String> GetPendingSyncIds(int syncId)
        {
            lock (lockObject)
            {
                List<String> guids = new List<String>();
                if (pendingUniqueIds.ContainsKey(syncId))
                    guids.AddRange(pendingUniqueIds[syncId]);

                return guids;
            }
        }

        protected override int RemoveDuplicatedSyncDataValues(int syncId, string destinationConn, IList<String> pendingUniqueIds)
        {
            SyncDataTable dataitems = null;
            lock (lockObject)
            {
                if (syncDataItems.ContainsKey(syncId))
                {
                    dataitems = syncDataItems[syncId] as SyncDataTable;
                }
            }

            var rowToDelete = new List<DataRow>();
            if (dataitems != null && dataitems.DataTable.Rows.Count > 0)
            {
                var uniqueIds = new List<String>();

                var parameters = destinationConn.FromXml<DataLoggerModel.Helpers.DataLoggerInfo>();
                var dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(parameters.DataProvider, parameters.Connection);
                var select = String.Format("SELECT * FROM {0}", dbSchemaInfo.WrapObjectName(parameters.TableName));
                var where = new StringBuilder();

                var dateTimeFormat = GetDateTimeFormat(dbSchemaInfo);
                for (int ii = 0; ii < dataitems.DataTable.Rows.Count; ii++)
                {
                    DataRow row = dataitems.DataTable.Rows[ii];
                    DateTime datetime = (DateTime)row[dataitems.Parameters.UtcTimeColumnName];
                    if (where.Length > 0)
                        where.Append(" OR ");
                    where.AppendFormat("{0} = '{1}'",
                        dbSchemaInfo.WrapObjectName(parameters.UtcTimeColumnName),
                        datetime.ToString(dateTimeFormat.ToString()));
                    if ((ii > 0 && (ii % 50) == 0) || ii == dataitems.DataTable.Rows.Count - 1)
                    {
                        var dataTable = DataReader.DataReader.GetDataSetSqlData(parameters.DataProvider,
                            parameters.Connection, select, where.ToString(), null, null, DataSchemaType.Source);
                        dataTable.TableName = parameters.TableName;
                        where.Clear();
                        for (int cc = 0; cc < dataTable.Rows.Count; cc++)
                            uniqueIds.Add(GetUniqueId(dataTable.Rows[cc], dataitems.Parameters));
                    }
                }

                for (int ii = 0; ii < dataitems.DataTable.Rows.Count; ii++)
                {
                    DataRow row = dataitems.DataTable.Rows[ii];
                    var uniqueId = GetUniqueId(row, dataitems.Parameters);
                    if (uniqueIds.Contains(uniqueId) || pendingUniqueIds.Contains(uniqueId))
                        rowToDelete.Add(row);
                }

                rowToDelete.ForEach((row) => dataitems.DataTable.Rows.Remove(row));
            }

            return rowToDelete.Count;
        }

        protected override int WriteSyncDataValues(int syncId, string destinationConn)
        {
            int writeCounter = 0;
            SyncDataTable dataitems = null;
            lock (lockObject)
            {
                if (syncDataItems.ContainsKey(syncId))
                {
                    dataitems = syncDataItems[syncId];
                }
            }

            if (dataitems != null && dataitems.DataTable.Rows.Count > 0)
            {
                var parameters = destinationConn.FromXml<DataLoggerModel.Helpers.DataLoggerInfo>();
                var dbCommandBuilder = DataReader.DataReader.CreateDbCommandBuilder(parameters.DataProvider);
                // check which records need to updated or inserted.
                var clonedTable = parameters.TableSchema.Clone();
                using (var writer = new DataWriter.DataSetWriter(parameters.DataProvider, parameters.Connection, 1))
                {
                    for (int ii = 0; ii < dataitems.DataTable.Rows.Count; ii++)
                    {
                        if (IsAborting())
                            break;

                        DataRow row = dataitems.DataTable.Rows[ii];
                        DataRowView rowView = clonedTable.DefaultView.AddNew();
                        rowView.BeginEdit();
                        foreach (DataColumn col in clonedTable.Columns)
                        {
                            if (!dataitems.DataTable.Columns.Contains(col.ColumnName))
                                continue;

                            rowView.Row[col.ColumnName] = row[col.ColumnName];
                        }
                        if (IsActiveServer)
                            rowView.Row[parameters.RedundancyColumnName] = DateTime.UtcNow;
                        rowView.EndEdit();

                        writeCounter++;
                    }

                    writer.InsertRows(clonedTable.DefaultView);
                    writer.Commit();
                }
            }

            return writeCounter;
        }

        protected override string GetConnectionStringDisplayName(string sourceConn)
        {
            var parameters = sourceConn.FromXml<DataLoggerModel.Helpers.DataLoggerInfo>();
            return parameters.Connection;
        }

        protected override void CleanSyncDataValues(int syncId)
        {
            lock (lockObject)
            {
                syncDataItems.Remove(syncId);
                pendingUniqueIds.Remove(syncId);
                syncStartTimes.Remove(syncId);
            }
        }

#if DEBUG
        protected override string GetRedundancyName()
        {
            return "DataLoggerSyncronization";
        }
#endif
        #endregion

        #region Private Methods
        String GetUniqueId(DataRow row, DataLoggerModel.Helpers.DataLoggerInfo parameters)
        {
            return String.Format("{0}-{1}", (DateTime)row[parameters.UtcTimeColumnName], row[parameters.MillisecondsColumnName]);
        }

        String GetDateTimeFormat(DbSchemaInfo dbSchemaInfo)
        {
            var dateTimeFormat = new StringBuilder(defaultDateTimeFormat);
            var scale = dbSchemaInfo.GetParameterScale(typeof(System.DateTime));
            if (scale > 0)
            {
                dateTimeFormat.Append(".");
                dateTimeFormat.Append('f', scale);
            }

            return dateTimeFormat.ToString();
        }
        #endregion
    }

    internal class SyncDataTable
    {
        #region Declarations
        readonly DataTable dataTable;
        readonly DataLoggerModel.Helpers.DataLoggerInfo parameters;
        #endregion

        #region Constructors
        public SyncDataTable(DataTable dataTable, DataLoggerModel.Helpers.DataLoggerInfo parameters)
        {
            this.dataTable = dataTable;
            this.parameters = parameters;
        }
        #endregion

        #region Properties
        public DataTable DataTable
        {
            get
            {
                return dataTable;
            }
        }

        public DataLoggerModel.Helpers.DataLoggerInfo Parameters
        {
            get
            {
                return parameters;
            }
        }
        #endregion
    }
}
