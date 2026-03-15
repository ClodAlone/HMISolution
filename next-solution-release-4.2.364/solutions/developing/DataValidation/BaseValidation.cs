using DataReader.Helpers;
using DataReader.SchemaInfo;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Data.SqlTypes;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Threading;
using UFUAServerInfo;
using Utilities;

namespace DataValidation
{
    public abstract class BaseValidation
    {
        #region Declarations
        readonly protected string name;
        readonly protected string invariantName;
        readonly protected string defaultConnection;

        readonly List<int> insertedRows = new List<int>();
        readonly List<int> deletedRows = new List<int>();
        readonly List<int> changedRows = new List<int>();
        //readonly List<int> authorizedRows = new List<int>();

        protected int totalRows;
        long unauthorizedAccess;

        DbSchemaInfo dbSchemaInfo;
        long allocUnitId;
        
        #region Constants
        const string queryBackupFiles =
            "SELECT msdb.dbo.backupset.server_name, " +
            "msdb.dbo.backupset.database_name, " +
            "msdb.dbo.backupset.backup_start_date, " +
            "msdb.dbo.backupset.backup_finish_date, " +
            "msdb.dbo.backupset.expiration_date, " +
            "msdb.dbo.backupset.type, " +
            "msdb.dbo.backupset.database_backup_lsn, " +
            "msdb.dbo.backupset.backup_size, " +
            "msdb.dbo.backupmediafamily.logical_device_name, " +
            "msdb.dbo.backupmediafamily.physical_device_name, " +
            "msdb.dbo.backupset.name AS backupset_name, " +
            "msdb.dbo.backupset.description " +
            "FROM msdb.dbo.backupmediafamily " +
            "INNER JOIN msdb.dbo.backupset ON msdb.dbo.backupmediafamily.media_set_id = msdb.dbo.backupset.media_set_id " +
            "WHERE msdb.dbo.backupset.database_name = '{0}'";

        const string fromLog1 = "fn_dblog(null, null)";

        const string fromLog2 = "fn_dump_dblog(null, null, N'DISK', {0}, N'{1}', " +
            "DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, " +
            "DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, " +
            "DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, " +
            "DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, " +
            "DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, " +
            "DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, " +
            "DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT, DEFAULT)";
        #endregion

        #endregion

        #region Constructors
        public BaseValidation() { }

        public BaseValidation(string name, string defaultConnection) :
            this(name, name, defaultConnection)
        { }

        public BaseValidation(string name, string invariantName, string defaultConnection)
        {
            this.name = name;
            this.invariantName = invariantName;
            this.defaultConnection = defaultConnection;
            this.queryTimeout = Properties.Settings.Default.DefaultQueryTimeout;
        }
        #endregion

        #region Events
        public event EventHandler<RowEventArg> ProcessingRow;

        protected void OnProcessingRow(int current)
        {
            var e = ProcessingRow;
            if (e != null)
                e(this, new RowEventArg(current, totalRows));
        }

        protected void OnProcessingRow(string message)
        {
            var e = ProcessingRow;
            if (e != null)
                e(this, new RowEventArg(message));
        }

        protected void OnProcessingRow(string message, int current, int total)
        {
            var e = ProcessingRow;
            if (e != null)
                e(this, new RowEventArg(message, current, total));
        }

        public event EventHandler<long> UnauthorizedAccess;
        protected void OnUnauthorizedAccess(long current)
        {
            var e = UnauthorizedAccess;
            if (e != null)
                e(this, current);
        }
        #endregion

        #region Abstract Methods
        public abstract ObservableCollection<Column> Columns { get; }

        public abstract ObservableCollection<DynamicRow> Rows { get; }

        protected abstract String ProviderName { get; }

        protected abstract String ConnectionString { get; }

        protected abstract String TableName { get; }

        protected abstract String AutoIncrementColumnName { get; }

        protected abstract bool InternalValidate(DateTime startDateTime, DateTime endDateTime, bool bContinueOnError);
        #endregion

        #region Public Methods
        public bool ValidateData(DateTime startDateTime, DateTime endDateTime, bool bContinueOnError)
        {
            ClearAllCounters();

            if(!skipValidation)
                ParseTransactionLog(startDateTime, endDateTime);

            if(skipValidation)
                return InternalValidate(startDateTime, endDateTime, bContinueOnError); 

            bool validationResult = InternalValidate(startDateTime, endDateTime, bContinueOnError);

            if (validationResult || bContinueOnError)
            {
                if (deletedRows.Count > 0)
                {
                    foreach (var oid in deletedRows)
                    {
                        if (Token != null)
                            Token.ThrowIfCancellationRequested();

                        if (insertedRows.Contains(oid))
                            continue;

                        AddMissingRow(oid, Properties.Resources.RemovedRecordValue);

                        if (!bContinueOnError)
                            break;
                    }

                    validationResult = false;
                }
            }

            if (validationResult || bContinueOnError)
            {
                if (insertedRows.Count > 0)
                {
                    foreach (var oid in insertedRows)
                    {
                        if (Token != null)
                            Token.ThrowIfCancellationRequested();

                        AddMissingRow(oid, Properties.Resources.AddedRecordValue);

                        if (!bContinueOnError)
                            break;
                    }

                    validationResult = false;
                }
            }

            if (validationResult || bContinueOnError)
            {
                if (changedRows.Count > 0)
                {
                    foreach (var oid in changedRows)
                    {
                        if (Token != null)
                            Token.ThrowIfCancellationRequested();

                        if (insertedRows.Contains(oid))
                            continue;

                        AddMissingRow(oid, Properties.Resources.ChangedRecordValue);

                        if (!bContinueOnError)
                            break;
                    }

                    validationResult = false;
                }
            }

            return validationResult && totalUnauthorizedAccess == 0;
        }

        public List<string> GetBackupFiles()
        {
            if (dbSchemaInfo == null)
                dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(ProviderName, ConnectionString);

            if (!dbSchemaInfo.DataSourceProductName.Contains("Microsoft SQL Server"))
                throw new InvalidOperationException(string.Format(Properties.Resources.MSSQLEngineRequired, dbSchemaInfo.DataSourceProductName));

            var backupFiles = new List<string>();
            using (var connection = DataReader.DataReader.CreateDbConnection(ProviderName, ConnectionString))
            {
                connection.Open();

                var dbdapater = DataReader.DataReader.CreateDbDataAdapter(ProviderName);
                dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(ProviderName);
                dbdapater.SelectCommand.Connection = connection;
                dbdapater.SelectCommand.CommandTimeout = queryTimeout;

                dbdapater.SelectCommand.CommandText = String.Format(queryBackupFiles, XpoConversionHelper.GetDataBaseName(ConnectionString));
                using (var reader = dbdapater.SelectCommand.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (Token != null)
                            Token.ThrowIfCancellationRequested();

                        if (((decimal)reader["database_backup_lsn"] == 0 || reader["type"].ToString() == "L") && reader["physical_device_name"] != DBNull.Value)
                        {
                            var filePath = reader["physical_device_name"].ToString();
                            if (!backupFiles.Contains(filePath))
                                backupFiles.Add(filePath);
                        }
                    }
                }
            }

            return backupFiles;
        }
        #endregion

        #region Protected Methods
        protected DateTime AdjustDateTime(DateTime dateTime, bool bUseUniversalTime = true)
        {
            if (bUseUniversalTime)
                dateTime = dateTime.ToUniversalTime();
            if (dateTime < SqlDateTime.MinValue.Value)
                dateTime = SqlDateTime.MinValue.Value;
            else if (dateTime > SqlDateTime.MaxValue.Value)
                dateTime = SqlDateTime.MaxValue.Value;

            return dateTime;
        }

        protected bool CheckIsValid(int oid)
        {
            if (skipValidation)
                return true;

            bool isValid = !changedRows.Contains(oid) && !insertedRows.Contains(oid) && !deletedRows.Contains(oid);

            changedRows.Remove(oid);
            insertedRows.Remove(oid);
            deletedRows.Remove(oid);

            //if (isValid && !authorizedRows.Contains(oid))
            //{
            //    Rows.Clear();
            //    throw new InvalidOperationException(Properties.Resources.MissingRecordsOnTransactionLog.Replace("--newline--", Environment.NewLine));
            //}

            return isValid;
        }

        protected virtual void ClearAllCounters()
        {
            totalRows = 0;
            unauthorizedAccess = 0;
            allocUnitId = 0;

            insertedRows.Clear();
            deletedRows.Clear();
            changedRows.Clear();
            //authorizedRows.Clear();
        }

        void ParseTransactionLog(DateTime startDateTime, DateTime endDateTime)
        {
            if (dbSchemaInfo == null)
                dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(ProviderName, ConnectionString);

            if (!dbSchemaInfo.DataSourceProductName.Contains("Microsoft SQL Server"))
                throw new InvalidOperationException(string.Format(Properties.Resources.MSSQLEngineRequired, dbSchemaInfo.DataSourceProductName));

            using (var connection = DataReader.DataReader.CreateDbConnection(ProviderName, ConnectionString))
            {
                connection.Open();

                OnProcessingRow(Properties.Resources.CheckingBackupsIntegrity);

                CheckBackupsIntegrity(connection);

                allocUnitId = GetAllocUnitId(connection);

                OnProcessingRow(String.Format(Properties.Resources.CheckingUnauthorizedAccessMessage, Name));

                //AnalyzeInsertedRows(connection);
                var suspectedTransactionIds = GetSuspectedTransactionIds(connection, startDateTime, endDateTime);

                var maxNumParametersQuery = Properties.Settings.Default.MaxNumParametersQuery;
                while (suspectedTransactionIds.Count > 0)
                {
                    if (Token != null)
                        Token.ThrowIfCancellationRequested();

                    var parameters = suspectedTransactionIds.Take(maxNumParametersQuery);
                    try
                    {
                        AnalyzeSuspectedTransactionIds(connection, parameters.ToArray());
                        suspectedTransactionIds.RemoveRange(0, parameters.Count());
                    }
                    catch (System.Data.SqlClient.SqlException)
                    {
                        maxNumParametersQuery = maxNumParametersQuery / 2;
                        if (maxNumParametersQuery == 0)
                            throw;
                    }
                }

                OnProcessingRow(null);
            }

            if (backupFiles != null && backupFiles.Length > 0)
            {
                if (Token != null)
                    Token.ThrowIfCancellationRequested();

                OnProcessingRow(Properties.Resources.AnalyzingBackupFiles);

                var backupsQueue = new ConcurrentQueue<BackupData>();
                var exceptions = new ConcurrentQueue<Exception>();

                long current = 0;
                int total = backupFiles.Length;
                Parallel.ForEach(backupFiles, backupFile =>
                {
                    try
                    {
                        if (Token != null)
                            Token.ThrowIfCancellationRequested();

                        OnProcessingRow(Properties.Resources.AnalyzingBackupFiles, (int)Interlocked.Read(ref current), total);

                        using (var connection = DataReader.DataReader.CreateDbConnection(ProviderName, ConnectionString))
                        {
                            connection.Open();

                            var backupAnalyzer = new BackupFileAnalyzer(backupFile, Token);
                            backupAnalyzer.Analyze(connection, ProviderName, ConnectionString);

                            foreach (var loginfo in backupAnalyzer.LogInfo)
                                backupsQueue.Enqueue(loginfo);
                        }

                        OnProcessingRow(Properties.Resources.AnalyzingBackupFiles, (int)Interlocked.Increment(ref current), total);
                    }
                    catch (Exception e)
                    {
                        exceptions.Enqueue(e);
                    }
                });

                if (exceptions.Count > 0)
                    throw new AggregateException(exceptions);

                if (backupsQueue.Count > 0)
                {
                    current = 0;
                    total = backupsQueue.Count;
                    Parallel.ForEach(backupsQueue, loginfo =>
                    {
                        try
                        {
                            if (Token != null)
                                Token.ThrowIfCancellationRequested();

                            OnProcessingRow(Properties.Resources.AnalyzingBackupFiles, (int)Interlocked.Read(ref current), total);

                            using (var connection = DataReader.DataReader.CreateDbConnection(ProviderName, ConnectionString))
                            {
                                connection.Open();

                                //if (loginfo.StartDate < startDateTime || loginfo.StartDate > endDateTime)
                                //    continue;

                                //AnalyzeInsertedRows(connection, backupFile.FilePath, position);
                                var suspectedTransactionIds = GetSuspectedTransactionIds(connection, startDateTime, endDateTime, loginfo.FilePath, loginfo.Position);
                                var maxNumParametersQuery = Properties.Settings.Default.MaxNumParametersQuery;
                                while (suspectedTransactionIds.Count > 0)
                                {
                                    if (Token != null)
                                        Token.ThrowIfCancellationRequested();

                                    var parameters = suspectedTransactionIds.Take(maxNumParametersQuery);
                                    try
                                    {
                                        AnalyzeSuspectedTransactionIds(connection, parameters.ToArray(), loginfo.FilePath, loginfo.Position);
                                        suspectedTransactionIds.RemoveRange(0, parameters.Count());
                                    }
                                    catch (System.Data.SqlClient.SqlException)
                                    {
                                        maxNumParametersQuery = maxNumParametersQuery / 2;
                                        if (maxNumParametersQuery == 0)
                                            throw;
                                    }
                                }
                            }

                            OnProcessingRow(Properties.Resources.AnalyzingBackupFiles, (int)Interlocked.Increment(ref current), total);
                        }
                        catch (Exception e)
                        {
                            exceptions.Enqueue(e);
                        }
                    });
                }

                if (exceptions.Count > 0)
                    throw new AggregateException(exceptions);

                OnProcessingRow(null);
            }
        }

        void AddMissingRow(int oid, string reason = null)
        {
            if (reason == null)
                reason = Properties.Resources.MissingRecordValue;

            var expandoObject = new ExpandoObject() as IDictionary<string, object>;
            foreach (var column in Columns)
            {
                if (column.FieldName == AutoIncrementColumnName)
                    expandoObject.Add(column.FieldName, oid);
                else
                    expandoObject.Add(column.FieldName, reason);
            }

            Rows.Add(new DynamicRow(expandoObject, false));
        }
        #endregion

        #region Privates
        void CheckBackupsIntegrity(System.Data.Common.DbConnection connection)
        {
            if (backupFiles == null || backupFiles.Length == 0)
                throw new InvalidOperationException(Properties.Resources.MissingFirstFullDatabaseBackup.Replace("--newline--", Environment.NewLine));

            var listBackups = new SortedList<decimal, BackupData>();
            foreach (var backupFile in backupFiles)
            {
                if (Token != null)
                    Token.ThrowIfCancellationRequested();

                var backupAnalyzer = new BackupFileAnalyzer(backupFile, Token);
                backupAnalyzer.Analyze(connection, ProviderName, ConnectionString);

                if (backupAnalyzer.FullBackup != null)
                    listBackups.Add(0, backupAnalyzer.FullBackup);

                foreach (var loginfo in backupAnalyzer.LogInfo)
                {
                    if (Token != null)
                        Token.ThrowIfCancellationRequested();

                    if (!listBackups.ContainsKey(loginfo.FirstLSN))
                        listBackups.Add(loginfo.FirstLSN, loginfo);
                }
            }

            if (!listBackups.ContainsKey(0))
                throw new InvalidOperationException(Properties.Resources.MissingFirstFullDatabaseBackup.Replace("--newline--", Environment.NewLine));

            var currentLSN = GetCurrentLSN(connection);
            foreach (var lsn in listBackups.Keys)
            {
                if (lsn == 0)
                {
                    if (!listBackups.ContainsKey(listBackups[lsn].FirstLSN) && listBackups[lsn].CheckPointLSN < currentLSN)
                        throw new InvalidOperationException(Properties.Resources.MissingTransactionLogBackup.Replace("--newline--", Environment.NewLine));
                }
                else if (!listBackups.ContainsKey(listBackups[lsn].LastLSN) && listBackups[lsn].LastLSN < currentLSN)
                    throw new InvalidOperationException(Properties.Resources.MissingTransactionLogBackup.Replace("--newline--", Environment.NewLine));
            }
        }

        decimal GetCurrentLSN(System.Data.Common.DbConnection connection)
        {
            var dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(ProviderName, ConnectionString);

            var dbdapater = DataReader.DataReader.CreateDbDataAdapter(ProviderName);
            dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(ProviderName);
            dbdapater.SelectCommand.Connection = connection;
            dbdapater.SelectCommand.CommandTimeout = queryTimeout;

            dbdapater.SelectCommand.CommandText = string.Format("SELECT TOP 1 [Current LSN] FROM {0}", GetLogFromSqlClause(null, 0));
            var result = dbdapater.SelectCommand.ExecuteScalar();
            if (result == null)
                throw new InvalidOperationException(String.Format(Properties.Resources.InvalidOrMissingTable, TableName));

            var currentLSN = (string)result;
            var values = currentLSN.Split(':');
            currentLSN = int.Parse(values[0], System.Globalization.NumberStyles.HexNumber).ToString("D");
            currentLSN = string.Format("{0}{1}", currentLSN, int.Parse(values[1], System.Globalization.NumberStyles.HexNumber).ToString("D10"));
            currentLSN = string.Format("{0}{1}", currentLSN, int.Parse(values[2], System.Globalization.NumberStyles.HexNumber).ToString("D5"));
            return decimal.Parse(currentLSN);
        }

        long GetAllocUnitId(System.Data.Common.DbConnection connection)
        {
            var dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(ProviderName, ConnectionString);

            var dbdapater = DataReader.DataReader.CreateDbDataAdapter(ProviderName);
            dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(ProviderName);
            dbdapater.SelectCommand.Connection = connection;
            dbdapater.SelectCommand.CommandTimeout = queryTimeout;

            var commantText = new StringBuilder();
            commantText.AppendFormat("SELECT AU.allocation_unit_id FROM sys.partitions AS P " +
                "INNER JOIN sys.allocation_units AS AU ON AU.container_id = P.partition_id " +
                "WHERE P.object_id = OBJECT_ID('{0}') AND index_id = 1 AND type = 1", dbSchemaInfo.WrapObjectName(TableName));

            dbdapater.SelectCommand.CommandText = commantText.ToString();
            var result = dbdapater.SelectCommand.ExecuteScalar();
            if (result == null)
                throw new InvalidOperationException(String.Format(Properties.Resources.InvalidOrMissingTable, TableName));
            return (long)result;
        }

        List<string> GetSuspectedTransactionIds(System.Data.Common.DbConnection connection, DateTime startDateTime, DateTime endDateTime, String backupFile = null, short position = 1)
        {
            if (string.IsNullOrEmpty(userSid))
                throw new InvalidOperationException(Properties.Resources.MissingUserSid);

            startDateTime = AdjustDateTime(startDateTime, bUseUniversalTime: false);
            endDateTime = AdjustDateTime(endDateTime, bUseUniversalTime: false);

            var transactionIds = new List<String>();
            var dbdapater = DataReader.DataReader.CreateDbDataAdapter(ProviderName);
            dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(ProviderName);
            dbdapater.SelectCommand.Connection = connection;
            dbdapater.SelectCommand.CommandTimeout = queryTimeout;

            var commantText = new StringBuilder();
            commantText.AppendFormat("SELECT {0} FROM {1} AS T1 WHERE Operation IN('LOP_BEGIN_XACT')", dbSchemaInfo.WrapObjectName("Transaction ID"), GetLogFromSqlClause(backupFile, position));

            var startDate = DataReader.DataReader.CreateDbParameter(ProviderName);
            startDate.DbType = System.Data.DbType.String;
            startDate.ParameterName = dbSchemaInfo.FormatParameterName("StartDate");
            startDate.Value = startDateTime.ToString("yyyy/MM/dd HH:mm:ss:fff");
            dbdapater.SelectCommand.Parameters.Add(startDate);

            var endDate = DataReader.DataReader.CreateDbParameter(ProviderName);
            endDate.DbType = System.Data.DbType.String;
            endDate.ParameterName = dbSchemaInfo.FormatParameterName("EndDate");
            endDate.Value = endDateTime.ToString("yyyy/MM/dd HH:mm:ss:fff");
            dbdapater.SelectCommand.Parameters.Add(endDate);

            commantText.AppendFormat(" AND {0} <> 0x{1}",
                dbSchemaInfo.WrapObjectName("Transaction SID"), userSid);
            commantText.AppendFormat(" AND ({0} >= {1} AND {2} <= {3})",
                dbSchemaInfo.WrapObjectName("Begin Time"), startDate.ParameterName,
                dbSchemaInfo.WrapObjectName("Begin Time"), endDate.ParameterName);

            // Exclude roolback transactions
            commantText.AppendFormat(" AND EXISTS (SELECT {0} FROM {1} AS T2 WHERE Operation IN('LOP_COMMIT_XACT') AND T1.{0} = T2.{0})", dbSchemaInfo.WrapObjectName("Transaction ID"), GetLogFromSqlClause(backupFile, position));

            dbdapater.SelectCommand.CommandText = commantText.ToString();
            var task = dbdapater.SelectCommand.ExecuteReaderAsync(Token).ContinueWith((ret) =>
            {
                if (ret.Exception != null)
                    throw ret.Exception;

                using (var reader = ret.Result)
                {
                    while (reader.Read())
                    {
                        if (Token != null)
                            Token.ThrowIfCancellationRequested();

                        transactionIds.Add(reader["Transaction ID"].ToString());
                    }
                }
            });

            task.Wait(Token);

            return transactionIds;
        }

        void AnalyzeSuspectedTransactionIds(System.Data.Common.DbConnection connection, String[] transactionIds, String backupFile = null, short position = 1)
        {
            var dbdapater = DataReader.DataReader.CreateDbDataAdapter(ProviderName);
            dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(ProviderName);
            dbdapater.SelectCommand.Connection = connection;
            dbdapater.SelectCommand.CommandTimeout = queryTimeout;

            //dbdapater.SelectCommand.CommandText = "SELECT * FROM fn_dblog(null,null)";

            var commantText = new StringBuilder();
            commantText.AppendFormat("SELECT * FROM {0} WHERE Operation IN('LOP_INSERT_ROWS', 'LOP_MODIFY_ROW', 'LOP_DELETE_ROWS', 'LOP_MODIFY_COLUMNS')", GetLogFromSqlClause(backupFile, position));

            var transactionText = new StringBuilder();
            for (int ii = 0; ii < transactionIds.Length; ii++)
            {
                var transId = DataReader.DataReader.CreateDbParameter(ProviderName);
                transId.DbType = System.Data.DbType.String;
                transId.ParameterName = dbSchemaInfo.FormatParameterName(String.Format("{0}", ii));
                transId.Value = transactionIds[ii];
                dbdapater.SelectCommand.Parameters.Add(transId);

                if (transactionText.Length > 0)
                    transactionText.Append(" OR ");
                transactionText.AppendFormat("{0} = {1}",
                    dbSchemaInfo.WrapObjectName("Transaction ID"), transId.ParameterName);
            }

            if (transactionText.Length > 0)
                commantText.AppendFormat(" AND ({0})", transactionText);

            var unitId = DataReader.DataReader.CreateDbParameter(ProviderName);
            unitId.DbType = System.Data.DbType.Int64;
            unitId.ParameterName = dbSchemaInfo.FormatParameterName("AllocUnitId");
            unitId.Value = allocUnitId;
            dbdapater.SelectCommand.Parameters.Add(unitId);

            commantText.AppendFormat(" AND {0} = {1}",
                dbSchemaInfo.WrapObjectName("AllocUnitId"), unitId.ParameterName);

            dbdapater.SelectCommand.CommandText = commantText.ToString();
            var task = dbdapater.SelectCommand.ExecuteReaderAsync(Token).ContinueWith((ret) =>
            {
                if (ret.Exception != null)
                    throw ret.Exception;

                using (var reader = ret.Result)
                {
                    while (reader.Read())
                    {
                        if (Token != null)
                            Token.ThrowIfCancellationRequested();

                        long current = 0;
                        if (reader["Operation"].ToString() == "LOP_INSERT_ROWS")
                        {
                            current = Interlocked.Increment(ref unauthorizedAccess);
                            if (reader["RowLog Contents 0"] != System.DBNull.Value &&
                                reader["Context"].ToString() == "LCX_CLUSTERED")
                            {
                                var rowId = FindRowIdFromLog0(reader["RowLog Contents 0"] as byte[]);
                                if (rowId > 0)
                                {
                                    lock (insertedRows)
                                    {
                                        if (!insertedRows.Contains(rowId))
                                            insertedRows.Add(rowId);
                                    }
                                }
                            }
                        }
                        else if (reader["Operation"].ToString() == "LOP_MODIFY_ROW" || reader["Operation"].ToString() == "LOP_MODIFY_COLUMNS")
                        {
                            current = Interlocked.Increment(ref unauthorizedAccess);
                            if (reader["RowLog Contents 2"] != System.DBNull.Value &&
                                reader["Context"].ToString() == "LCX_CLUSTERED")
                            {
                                var rowId = FindRowIdFromLog2(reader["RowLog Contents 2"] as byte[]);
                                if (rowId > 0)
                                {
                                    lock (changedRows)
                                    {
                                        if (!changedRows.Contains(rowId))
                                            changedRows.Add(rowId);
                                    }
                                }
                            }
                        }
                        else if (reader["Operation"].ToString() == "LOP_DELETE_ROWS")
                        {
                            current = Interlocked.Increment(ref unauthorizedAccess);
                            if (reader["RowLog Contents 0"] != System.DBNull.Value &&
                                reader["Context"].ToString() == "LCX_MARK_AS_GHOST")
                            {
                                var rowId = FindRowIdFromLog0(reader["RowLog Contents 0"] as byte[]);
                                if (rowId > 0)
                                {
                                    lock (deletedRows)
                                    {
                                        if (!deletedRows.Contains(rowId))
                                            deletedRows.Add(rowId);
                                    }
                                }
                            }
                        }

                        if (current > 0)
                            OnUnauthorizedAccess(current);
                    }
                }
            });

            task.Wait(Token);
        }

        //void AnalyzeInsertedRows(System.Data.Common.DbConnection connection, String backupFile = null, short position = 1)
        //{
        //    var dbdapater = DataReader.DataReader.CreateDbDataAdapter(ProviderName);
        //    dbdapater.SelectCommand = DataReader.DataReader.CreateDbCommand(ProviderName);
        //    dbdapater.SelectCommand.Connection = connection;
        //    dbdapater.SelectCommand.CommandTimeout = queryTimeout;

        //    //dbdapater.SelectCommand.CommandText = "SELECT * FROM fn_dblog(null,null)";

        //    var commantText = new StringBuilder();
        //    commantText.AppendFormat("SELECT {0} FROM {1} WHERE Operation IN('LOP_INSERT_ROWS')", dbSchemaInfo.WrapObjectName("RowLog Contents 0"), GetLogFromSqlClause(backupFile, position));

        //    var unitId = DataReader.DataReader.CreateDbParameter(ProviderName);
        //    unitId.DbType = System.Data.DbType.Int64;
        //    unitId.ParameterName = dbSchemaInfo.FormatParameterName("AllocUnitId");
        //    unitId.Value = allocUnitId;
        //    dbdapater.SelectCommand.Parameters.Add(unitId);

        //    commantText.AppendFormat(" AND {0} = {1}",
        //        dbSchemaInfo.WrapObjectName("AllocUnitId"), unitId.ParameterName);

        //    dbdapater.SelectCommand.CommandText = commantText.ToString();

        //    using (var reader = dbdapater.SelectCommand.ExecuteReader())
        //    {
        //        while (reader.Read())
        //        {
        //            if (Token != null)
        //                Token.ThrowIfCancellationRequested();

        //            var rowId = FindRowIdFromLog0(reader["RowLog Contents 0"] as byte[]);
        //            if (rowId > 0 && !authorizedRows.Contains(rowId))
        //                authorizedRows.Add(rowId);
        //        }
        //    }
        //}

        //RowLog Contents 0 (0x3000 7C00 01000000 ...)
        //Offset    Name                    Size in Bytes       Data Conversion
        //0	        Status Bits	            2	                None
        //2	        Fixed Length Size   	2	                Hex to decimal
        //4	        Fixed Length Data	    n	                Hex to decimal
        int FindRowIdFromLog0(byte[] rowLog)
        {
            int rowId = 0;
            if (rowLog != null && rowLog.Length >= 8)
            {
                try
                {
                    rowId = BitConverter.ToInt32(rowLog, 4);
                }
                catch
                { }
            }
            return rowId;
        }

        //RowLog Contents 2 (0x16 04000000 010000) 
        //Offset    Name                    Size in Bytes       Data Conversion
        //0	        Status Bit	            1	                None
        //1	        OID	                    1                   Hex to decimal
        int FindRowIdFromLog2(byte[] rowLog)
        {
            int rowId = 0;
            if (rowLog != null && rowLog.Length >= 5)
            {
                try
                {
                    rowId = BitConverter.ToInt32(rowLog, 1);
                }
                catch
                { }
            }
            return rowId;
        }

        string GetLogFromSqlClause(string backupFile, short position)
        {
            return backupFile == null ? fromLog1 : String.Format(fromLog2, position, backupFile);
        }
        #endregion

        #region Properties
        public string Name
        {
            get
            {
                return name;
            }
        }

        public string InvariantName
        {
            get
            {
                return invariantName;
            }
        }

        internal CancellationToken Token { get; set; }

        internal String userSid { get; set; }

        internal String[] backupFiles { get; set; }

        internal int queryTimeout { get; set; }

        internal bool skipValidation { get; set; }

        internal long totalUnauthorizedAccess
        {
            get
            {
                return Interlocked.Read(ref unauthorizedAccess);
            }
        }
        #endregion
    }
}
