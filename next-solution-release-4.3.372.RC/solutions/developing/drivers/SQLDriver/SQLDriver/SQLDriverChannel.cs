using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;
using System.Text;
using Utilities;
using System.Globalization;
using System.Data.Common;

namespace SQLDriver
{

    public enum SQLDriverErrorCodes : int
    {
        ErrorConnectionToDevice = 1400,
        ErrorReadError = 1401,
        ErrorCreatingTable = 1402,
        ErrorWriteError = 1043,
        ErrorCreatingDataSet = 1044,
        ErrorDriverSettings = 1045

    }

    class SQLDriverChannel : Channel
    {
        #region Constructors

        /// <summary>
        /// Initializes the SQLDriverChannel object.
        /// </summary>
        public SQLDriverChannel(SQLDriverDriver commdriver, SQLDriverChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _Provider = settings.Provider;
            _ConnectionString = settings.ConnectionString;
            _BackupConnectionString = settings.BackupConnectionString;
            _BackupProvider = settings.BackupProvider;
            _ChannelSwitchHostTimeout = settings.ChannelSwitchHostTimeout;
            _PublishingInterval = settings.PublishingInterval;
            _MaxConsecutiveWrite = settings.MaxConsecutiveWrite.Value;

            if (String.IsNullOrEmpty(_Provider) && !String.IsNullOrEmpty(_ConnectionString))
            {
                var helper = new DevExpress.Xpo.DB.Helpers.ConnectionStringParser(_ConnectionString);
                _Provider = helper.GetPartByName("DataProvider");
                helper.RemovePartByName("DataProvider");
                _ConnectionString = helper.GetConnectionString();
            }


            if (String.IsNullOrEmpty(_Provider) || String.IsNullOrEmpty(_ConnectionString))
            {
                _Provider = commdriver.DefaultProvider;
                _ConnectionString = commdriver.DefaultConnection;
            }
        }

        private DbConnection dbconnection;
        private DataWriter.DataSetWriter writer;
        /// <summary>   The lock connection. </summary>
        protected object lockConn = new object();
        #endregion

        #region Abstracts Methods

        private bool IsConnected;
        bool switchHost = false;
        string selectedConnectionString;
        string selectedProvider;
        DateTime inactivityTimer;
        public override bool IsDeviceOpen()
        {
            if (!switchHost)
                selectedConnectionString = ConnectionString;
            else
                selectedConnectionString = BackupConnectionString;
            if(dbconnection.State == ConnectionState.Open )
            {
                inactivityTimer = DateTime.Now;
                IsConnected = true;
                selectedProvider = Provider;
                SetStateCommandVariableBit(selectedConnectionString != ConnectionString, (UInt16)ChannelVariableBits.ConnectedHost);
            }
            else if(dbconnection.State == ConnectionState.Closed || dbconnection.State == ConnectionState.Broken)
            {
                IsConnected = false;
            }
            bool returnValue = IsConnected;
            return returnValue;
        }
        bool varState;
        bool BackupHostErrorState;
        DateTime testDbBackupTimer;
        private int testDbBackupDelay_S = Properties.Settings.Default.testDbBackupDelay_S;
        private void testBackupConnection()
        {
            DbConnection testBackupConnection = DataReader.DataReader.CreateDbConnection(BackupProvider, BackupConnectionString);
            try
            {
                testBackupConnection.Open();
                BackupHostErrorState = false;
                testBackupConnection.Close();
                testBackupConnection.Dispose();
            }
            catch(Exception Ex)
            {
                BackupHostErrorState = true;
                testBackupConnection.Dispose();
            }
            testDbBackupTimer = DateTime.Now;
        }
        string CurrentErrorMessage = string.Empty;
        string CurrentBackupErrorMessage = string.Empty;
        public override bool DeviceOpen()
        {
            if ((DateTime.Now - testDbBackupTimer).TotalSeconds > testDbBackupDelay_S && !string.IsNullOrEmpty(BackupConnectionString))
            {
                testBackupConnection();
                SetStateCommandVariableBit(BackupHostErrorState, (UInt16)ChannelVariableBits.BackupHostErrorState);
            }
            if (!switchHost || ((GetStateCommandVariableBit(ref varState, (UInt16)ChannelVariableBits.SwitchServer) == true) && varState && switchHost))
            {
                try
                {
                    if (varState && switchHost)
                        SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.SwitchServer);
                    if (varState && string.IsNullOrEmpty(BackupConnectionString))
                        SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.SwitchServer);
                    selectedConnectionString = ConnectionString;
                    dbconnection.Open();
                    switchHost = false;
                    IsConnected = true;
                    selectedProvider = Provider;
                    SetStateCommandVariableBit(selectedConnectionString != ConnectionString, (UInt16)ChannelVariableBits.ConnectedHost);
                    SetStateCommandVariableBit(!IsConnected, (UInt16)ChannelVariableBits.ChannelUnconnected);
                    inactivityTimer = DateTime.Now;
                    CurrentErrorMessage = string.Empty;
                    CurrentBackupErrorMessage = string.Empty;
                }
                catch (Exception ex)
                {
                    if (!switchHost && !IsConnected && CurrentErrorMessage != ex.Message)
                    {
                        CurrentErrorMessage = ex.Message;
                        IsConnected = false;
                        CommDriver.OnSystemEvent(null, string.Format(Properties.Resources.ErrorConnectionToDevice, ex.Message, dbconnection.ConnectionString), Opc.Ua.EventSeverity.Low);
                    }
                }
            }
            if (!(string.IsNullOrEmpty(BackupConnectionString) && string.IsNullOrEmpty(BackupProvider)) &&
                                    (((DateTime.Now - inactivityTimer).TotalMilliseconds > ChannelSwitchHostTimeout) || (switchHost == true)) || ((GetStateCommandVariableBit(ref varState, (UInt16)ChannelVariableBits.SwitchServer) == true) && !switchHost && varState) && !string.IsNullOrEmpty(BackupConnectionString))
            {
                if (!switchHost && varState)
                    SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.SwitchServer);
                switchHost = true;
                selectedConnectionString = BackupConnectionString;
                dbconnection = DataReader.DataReader.CreateDbConnection(BackupProvider, BackupConnectionString);
                try
                {
                    dbconnection.Open();
                    IsConnected = true;
                    CurrentBackupErrorMessage = string.Empty;
                }
                catch(Exception ex)
                {
                    switchHost = false;
                    IsConnected = false;
                    if (CurrentBackupErrorMessage != ex.Message)
                    {
                        CurrentBackupErrorMessage = ex.Message;
                        CommDriver.OnSystemEvent(null, string.Format(Properties.Resources.ErrorConnectionToDevice, ex.Message, dbconnection.ConnectionString), Opc.Ua.EventSeverity.High);
                        LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorConnectionToDevice;
                    }
                }
                selectedProvider = BackupProvider;
                SetStateCommandVariableBit(!IsConnected, (UInt16)ChannelVariableBits.ChannelUnconnected);
                SetStateCommandVariableBit(selectedConnectionString != ConnectionString && IsConnected, (UInt16)ChannelVariableBits.ConnectedHost);
            }
            return IsConnected;
        }

        public override bool DeviceClose() { return true; }
        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        public override uint GetBytesToRead() { return 1; }
        public override uint GetBytesToWrite() { return 1; }

        protected override void ScheduleCommJobs(List<CommJob> joblist, CommJobState type, bool bAddInError = false)
        {
            base.ScheduleCommJobs(joblist, type, type == CommJobState.PollingInError);
        }
        #endregion

        #region Data Members

        List<SQLDriverCommJob> nextlist = new List<SQLDriverCommJob>();
        List<CommJob> ListJobExec = new List<CommJob>();

        DateTime lastWriteTime = DateTime.MinValue;

        string lastReadExceptionMessage = String.Empty;

        #endregion

        #region Sending task
        public const int NUMBERAGGREGATESJOBS = 20;
        List<SQLDriverCommJob> jobCompleteList = new List<SQLDriverCommJob>();
        List<SQLDriverCommJob> jobSendingList = new List<SQLDriverCommJob>();
        Dictionary<UInt32, List<SQLDriverCommJob>> pendingWriteStateJobs = new Dictionary<UInt32, List<SQLDriverCommJob>>();
        readonly Object lockSQLDriverListObject = new Object();
        string conn = string.Empty;

        #endregion


        #region Override Methods

        protected override void WorkingThread(object data)
        {
            int SleepCycle = (Int32)WaitTime;
            if (SleepCycle == 0)
            {
                SleepCycle = 1;
            }
            ListJobPending.Clear();
            ListJobExecuted.Clear();

            NextScheduleTimeJobsList = DateTime.UtcNow;
            int Loop = 0;
            while (true)
            {
                nextlist.Clear();
                if (ListJobPending.Count == 0)
                {
                    ScheduleListJob();
                    lock (lockThreadObject)
                    {
                        if (SynchroJob != null)
                            nextlist.Add(SynchroJob as SQLDriverCommJob);
                        else
                            GetNextPendingList(ref nextlist);
                    }
                    if (nextlist.Count > 0)
                        ListJobPending.AddRange(nextlist);
                }
                if (nextlist.Count > 0)
                {
                    lock (lockThreadObject)
                    {
                        ExecuteJobList(nextlist);
                    }
                }
                lock (lockThreadObject)
                    if (ListJobPending.Count > 0)
                    {
                        ListJobExec.Clear();
                        foreach (var job in ListJobPending)
                        {
                            ListJobExecuted.Add(job);
                        }
                        foreach (var job in ListJobExecuted)
                        {
                            job.LastExecutionTime = DateTime.UtcNow;
                            if (SynchroJob != null && SynchroJob == job)
                            {
                                job.ResetSynchro.WaitOne(Timeout);
                                SynchroJob = null;
                                job.ResetSynchro.Reset();
                            }
                            ListJobPending.Remove(job);
                        }
                        ListJobExecuted.Clear();
                    }

                if (nextlist.Count != 0 && StopWorkerThread.WaitOne(SleepCycle, false))
                {
                    break;
                }
                else if (++Loop > 4)
                {
                    Loop = 0;
                    if (StopWorkerThread.WaitOne(SleepCycle, false))
                        break;
                }
                StopWorkerThread.WaitOne(0, false);
            }
        }

        #endregion

        #region Local Methods

        protected void SetConnectionError(List<SQLDriverCommJob> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.ErrorCode = DriverErrorCodes.ErrorTimeOut;
                eJob.GeneralError = true;
                eJob.Job = list[i];
                OnJobExecuted(eJob);
            }
            return;
        }

        public bool Connect(string ConnectionString, SqlConnection Connection)
        {
            Connection.ConnectionString = ConnectionString;
            Connection.Open();
            return true;
        }

        public bool Disconnect(SqlConnection Connection)
        {
            Connection.Close();
            return true;
        }

        int nWriteCount = 0;
        protected void GetNextPendingList (ref List<SQLDriverCommJob> List)
        {
            bool Write = false;
            LinkType jLink = LinkType.InputOutput;
            string Station = string.Empty;
            bool First = true;
            lock(lockScheduleFlag)
            {
                List<CommJob> examinationList = new List<CommJob>();
                //Write?
                if (
                    (PublishingInterval > 0 && lastWriteTime.AddMilliseconds(PublishingInterval) <= DateTime.UtcNow && nWriteCount > 0) ||
                    (PublishingInterval == 0 && nWriteCount > 0))
                {
                    if(GetScheduledJobQueue(CommJobState.PollingInError).Count > 0)
                        examinationList.AddRange(GetScheduledJobQueue(CommJobState.PollingInError));
                    if (GetScheduledJobQueue(CommJobState.PollingInUse).Count > 0)
                        examinationList.AddRange(GetScheduledJobQueue(CommJobState.PollingInUse));
                    if (GetScheduledJobQueue(CommJobState.PollingNotInUse).Count > 0)
                        examinationList.AddRange(GetScheduledJobQueue(CommJobState.PollingNotInUse));
                    if (GetScheduledJobQueue(CommJobState.PollingNow).Count > 0)
                        examinationList.AddRange(GetScheduledJobQueue(CommJobState.PollingNow));
                    if((from j in examinationList where j.TagsListToWrite.Count > 0 select j).ToList().Count > 0)
                    {
                        //write
                        for (int i = (int)CommJobState.PollingNow; i >= (int)CommJobState.PollingInError; i--)
                        {
                            var queue = GetScheduledJobQueue((CommJobState)i);
                            examinationList.Clear();
                            if (queue.Count > 0)
                                examinationList.AddRange(queue);
                            if(examinationList.Count > 0)
                            {
                                if(string.IsNullOrEmpty(Station))
                                    Station = examinationList[0].Station.Name;
                                var toWr = (from SQLDriverCommJob j in examinationList
                                            where (j.Station.Name == Station && ((j.TagsListToWrite.Count > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput))
                                            select j).ToList();
                                if(toWr.Count > 0)
                                {
                                    //write! put in list and remove from queue!
                                    List.AddRange(toWr);
                                    int elCount = queue.Count;
                                    for(int k = 0; k < elCount; k++)
                                    {
                                        CommJob dequeuedJob = null;
                                        var d = queue.TryDequeue(out dequeuedJob);
                                        if (dequeuedJob != null && !toWr.Contains(dequeuedJob))
                                            queue.Enqueue(dequeuedJob);
                                    }
                                }
                            }
                        }
                        nWriteCount--;
                        System.Diagnostics.Trace.TraceInformation("GetNext lW:{0} w:{1}", List.Count, nWriteCount);
                        return;
                    }
                }
                Station = string.Empty;
                //Read?
                for (int i = (int)CommJobState.PollingNow; i >= (int)CommJobState.PollingInError; i--)
                {
                    var queue = GetScheduledJobQueue((CommJobState)i);
                    examinationList.Clear();
                    if (queue.Count > 0)
                        examinationList.AddRange(queue);
                    if (examinationList.Count > 0)
                    {
                        if (string.IsNullOrEmpty(Station))
                            Station = examinationList[0].Station.Name;
                        var toRd = (from SQLDriverCommJob j in examinationList
                                    where (j.Station.Name == Station && ((j.TagsListToWrite.Count == 0 && (j.Type == LinkType.Input || j.Type == LinkType.InputOutput))))
                                    select j).ToList();
                        if (toRd.Count > 0)
                        {
                            //Read! put in list and remove from queue!
                            List.AddRange(toRd);
                            int elCount = queue.Count;
                            for (int k = 0; k < elCount; k++)
                            {
                                CommJob dequeuedJob = null;
                                var d = queue.TryDequeue(out dequeuedJob);
                                if (dequeuedJob != null && !toRd.Contains(dequeuedJob))
                                    queue.Enqueue(dequeuedJob);
                            }
                        }
                    }
                }
                nWriteCount = MaxConsecutiveWrite;
            }
        }

        protected override void ScheduleListJob(bool bAddPollingNow = false)
        {
            base.ScheduleListJob(true);
        }
        protected void ExecuteJobList(List<SQLDriverCommJob> List)
        {
            SQLDriverStation s = List[0].Station as SQLDriverStation;
            if (s == null)
                return;

            ExchangeData(List);
        }

        protected void ExchangeData(List<SQLDriverCommJob>List)
        {
            for (int index = 0; index < List.Count; index++)
            {
                if (List[index].Type == LinkType.UnconditionalOutput)
                {
                    if (List[index].TagsListToWrite.Count == 0)
                    {
                        List[index].TagsListToWrite.AddRange(List[index].TagsList);
                    }
                }
            }
            if (List[0].Type == LinkType.Input || (List[0].Type == LinkType.InputOutput && List[0].TagsListToWrite.Count == 0))
            {
                ReadData(List);
            }
            else
            {
                WriteData(List);
            }
        }

        protected void ReadData(List<SQLDriverCommJob> list)
        {
            uint index = 0;
            int[] ErrorCodes = new int[list.Count];
            string szError = string.Empty;
            SQLDriverCommJob j = null;
            ArrayList ListIndexToRead = new ArrayList();
            SQLDriverStation s = list[0].Station as SQLDriverStation;
            if (s == null)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                j = list[i];
                ErrorCodes[i] = (int)DriverErrorCodes.ErrorNoError;
                ListIndexToRead.Add(i);
                index++;
                if (index == 0)
                {
                    return;
                }
                int serror = 0;
                int nRet = 1;
                nRet = ReadFromDatabase(ref serror, s, list);
                if (nRet == 0)
                {

                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                }
            }
            ListIndexToRead.Clear();
        }

        protected int ReadFromDatabase(ref int serror, SQLDriverStation s, List<SQLDriverCommJob> pendingjob)
        {
            DateTime date = DateTime.UtcNow;
            //var Connection = DataReader.DataReader.CreateDbConnection(Provider, ConnectionString);
            string command = string.Empty;
            List<string> columnToRead = new List<string>();
            dbconnection = DataReader.DataReader.CreateDbConnection(Provider, ConnectionString);
            //dbconnection.Open();
            
            try
            {
                if (!IsDeviceOpen())
                    DeviceOpen();
                else
                    inactivityTimer = DateTime.Now;

                var dbadapter = DataReader.DataReader.CreateDbDataAdapter(selectedProvider);
                DataReader.SchemaInfo.DbSchemaInfo dbSchemaInfo = DataReader.SchemaInfo.DbSchemaInfoFactory.CreateSchemaInfo(selectedProvider, selectedConnectionString);
                dbadapter.SelectCommand = DataReader.DataReader.CreateDbCommand(selectedProvider);
                dbadapter.SelectCommand.Connection = dbconnection;
                    
                StringBuilder commandText = new StringBuilder("SELECT ");
                DataSet gridDataSet = new DataSet();
                bool first = true;
                if(s.SQLDriverMultiColumn)
                {
                    foreach (var j in pendingjob)
                    {
                        if (!columnToRead.Contains(((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn))
                        {
                            columnToRead.Add(((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn);
                            if (first)
                            {
                                commandText.AppendFormat("{0},{1}", dbSchemaInfo.WrapObjectName(s.SQLDriverColumnNameTagName), dbSchemaInfo.WrapObjectName(((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn));
                                first = false;
                            }
                            else
                                commandText.AppendFormat(",{0}", dbSchemaInfo.WrapObjectName(((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn));
                        }
                    }
                }
                else
                    commandText.AppendFormat("{0},{1}", dbSchemaInfo.WrapObjectName(s.SQLDriverColumnNameTagName), dbSchemaInfo.WrapObjectName(s.SQLDriverColumnValue));
                commandText.AppendFormat(" FROM {0}", dbSchemaInfo.WrapObjectName(s.SQLDriverTableName), dbSchemaInfo.WrapObjectName(s.SQLDriverColumnNameTagName));
                dbadapter.SelectCommand.CommandText = commandText.ToString();
                dbadapter.Fill(gridDataSet, s.SQLDriverTableName);
                command = commandText.ToString();
                DataColumn[] columns = new DataColumn[1];
                columns[0] = gridDataSet.Tables[0].Columns[0];
                gridDataSet.Tables[0].PrimaryKey = columns;
                int Dim = 0;
                for (int i = 0; i < pendingjob.Count; i++)
                {
                    System.Data.DataRow[] Rows = gridDataSet.Tables[0].Select(s.SQLDriverColumnNameTagName + " = '" + pendingjob[i].TagName + "'");
                    System.Data.DataRow Row = Rows[0]/*gridDataSet.Tables[0].Rows.Find(pendingjob[i].TagName)*/;
                        switch ((uint)pendingjob[i].TagsList[0].TagNode.DataType.Identifier)
                    {
                        case (uint)BuiltInType.Boolean:
                            if (pendingjob[i].TagsList[0].TagNode.ArrayDimension != 0)
                                Dim = 1 * (int)pendingjob[i].TagsList[0].TagNode.ArrayDimension;
                            else
                                Dim = 1;
                            break;
                        case (uint)BuiltInType.Int16:
                            if (pendingjob[i].TagsList[0].TagNode.ArrayDimension != 0)
                                Dim = 2 * (int)pendingjob[i].TagsList[0].TagNode.ArrayDimension;
                            else
                                Dim = 2;
                            break;
                        case (uint)BuiltInType.Byte:
                            if (pendingjob[i].TagsList[0].TagNode.ArrayDimension != 0)
                                Dim = 1 * (int)pendingjob[i].TagsList[0].TagNode.ArrayDimension;
                            else
                                Dim = 1;
                            break;
                        case (uint)BuiltInType.Int32:
                            if (pendingjob[i].TagsList[0].TagNode.ArrayDimension != 0)
                                Dim = 4 * (int)pendingjob[i].TagsList[0].TagNode.ArrayDimension;
                            else
                                Dim = 4;
                            break;
                        case (uint)BuiltInType.Int64:
                            if (pendingjob[i].TagsList[0].TagNode.ArrayDimension != 0)
                                Dim = 8 * (int)pendingjob[i].TagsList[0].TagNode.ArrayDimension;
                            else
                                Dim = 8;
                            break;
                        case (uint)BuiltInType.Double:
                            if (pendingjob[i].TagsList[0].TagNode.ArrayDimension != 0)
                                Dim = 8 * (int)pendingjob[i].TagsList[0].TagNode.ArrayDimension;
                            else
                                Dim = 8;
                            break;
                        case (uint)BuiltInType.Float:
                            if (pendingjob[i].TagsList[0].TagNode.ArrayDimension != 0)
                                Dim = 4 * (int)pendingjob[i].TagsList[0].TagNode.ArrayDimension;
                            else
                                Dim = 4;
                            break;
                        case (uint)BuiltInType.UInt16:
                            if (pendingjob[i].TagsList[0].TagNode.ArrayDimension != 0)
                                Dim = 2 * (int)pendingjob[i].TagsList[0].TagNode.ArrayDimension;
                            else
                                Dim = 2;
                            break;
                        case (uint)BuiltInType.UInt32:
                            if (pendingjob[i].TagsList[0].TagNode.ArrayDimension != 0)
                                Dim = 4 * (int)pendingjob[i].TagsList[0].TagNode.ArrayDimension;
                            else
                                Dim = 4;
                            break;
                        case (uint)BuiltInType.UInt64:
                            if (pendingjob[i].TagsList[0].TagNode.ArrayDimension != 0)
                                Dim = 8 * (int)pendingjob[i].TagsList[0].TagNode.ArrayDimension;
                            else
                                Dim = 8;
                            break;
                    }
                    byte[] stringAnswer = new byte[Dim];
                    DiagnLastTaskRxBytes += Dim;
                    LastErrorCode = DriverErrorCodes.ErrorNoError;
                    if(s.SQLDriverMultiColumn)
                    {
                        switch ((uint)pendingjob[i].TagsList[0].TagNode.DataType.Identifier)
                        {
                            case (uint)BuiltInType.Boolean:
                                try
                                {
                                    if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                    {
                                        var ValueBool = Convert.ToBoolean(Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)));
                                        stringAnswer = BitConverter.GetBytes((Boolean)ValueBool);
                                    }
                                    else
                                    {
                                        {
                                            string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn));
                                            string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                            string[] values = pureElement.Split('|');
                                            Boolean[] ValueBoolean = new Boolean[values.Length];
                                            for (int Count = 0; Count < values.Length; Count++)
                                            {
                                                ValueBoolean[Count] = Convert.ToBoolean(values[Count]);
                                            }
                                            Buffer.BlockCopy(ValueBoolean, 0, stringAnswer, 0, stringAnswer.Length);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex);
                                    string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                    if (exceptionMessage != lastReadExceptionMessage )
                                    {
                                        lastReadExceptionMessage = exceptionMessage;
                                        CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                    }
                                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                    break;
                                }
                                break;
                            case (uint)BuiltInType.Int16:
                                try
                                {
                                    if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                    {
                                        var ValueInt = Convert.ToInt16(Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)));
                                        stringAnswer = BitConverter.GetBytes((UInt16)ValueInt);
                                    }
                                    else
                                    {
                                        {
                                            string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                            string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                            string[] values = pureElement.Split('|');
                                            Int16[] ValueInt16 = new Int16[values.Length];
                                            for (int Count = 0; Count < values.Length; Count++)
                                            {
                                                ValueInt16[Count] = Convert.ToInt16(values[Count]);
                                            }
                                            Buffer.BlockCopy(ValueInt16, 0, stringAnswer, 0, stringAnswer.Length);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex);
                                    string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                    if (exceptionMessage != lastReadExceptionMessage)
                                    {
                                        lastReadExceptionMessage = exceptionMessage;
                                        CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                    }
                                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                    break;
                                }
                                break;
                            case (uint)BuiltInType.Byte:
                                try
                                {
                                    if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                    {
                                        var ValueByte = Convert.ToByte(Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)));
                                        stringAnswer = BitConverter.GetBytes((Byte)ValueByte);
                                    }
                                    else
                                    {
                                        {
                                            string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                            string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                            string[] values = pureElement.Split('|');
                                            Byte[] ValueByte = new Byte[values.Length];
                                            for (int Count = 0; Count < values.Length; Count++)
                                            {
                                                ValueByte[Count] = Convert.ToByte(values[Count]);
                                            }
                                            Buffer.BlockCopy(ValueByte, 0, stringAnswer, 0, stringAnswer.Length);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex);
                                    string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                    if (exceptionMessage != lastReadExceptionMessage)
                                    {
                                        lastReadExceptionMessage = exceptionMessage;
                                        CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                    }
                                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                    break;
                                }
                                break;
                            case (uint)BuiltInType.Int32:
                                try
                                {
                                    if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                    {
                                        var ValueInt32 = Convert.ToInt32(Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)));
                                        stringAnswer = BitConverter.GetBytes((Int32)ValueInt32);
                                    }
                                    else
                                    {
                                        {
                                            string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                            string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                            string[] values = pureElement.Split('|');
                                            Int32[] ValueInt32 = new Int32[values.Length];
                                            for (int Count = 0; Count < values.Length; Count++)
                                            {
                                                ValueInt32[Count] = Convert.ToInt32(values[Count]);
                                            }
                                            Buffer.BlockCopy(ValueInt32, 0, stringAnswer, 0, stringAnswer.Length);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex);
                                    string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                    if (exceptionMessage != lastReadExceptionMessage)
                                    {
                                        lastReadExceptionMessage = exceptionMessage;
                                        CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                    }
                                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                    break;
                                }
                                break;
                            case (uint)BuiltInType.Int64:
                                try
                                {
                                    if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                    {
                                        var ValueInt64 = Convert.ToInt64(Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)));
                                        stringAnswer = BitConverter.GetBytes((Int64)ValueInt64);
                                    }
                                    else
                                    {
                                        {
                                            string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                            string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                            string[] values = pureElement.Split('|');
                                            Int64[] ValueInt64 = new Int64[values.Length];
                                            for (int Count = 0; Count < values.Length; Count++)
                                            {
                                                ValueInt64[Count] = Convert.ToInt64(values[Count]);
                                            }
                                            Buffer.BlockCopy(ValueInt64, 0, stringAnswer, 0, stringAnswer.Length);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex);
                                    string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                    if (exceptionMessage != lastReadExceptionMessage)
                                    {
                                        lastReadExceptionMessage = exceptionMessage;
                                        CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                    }
                                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                    break;
                                }
                                break;
                            case (uint)BuiltInType.Double:
                                try
                                {
                                    if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                    {
                                        var ValueDouble = Convert.ToDouble(Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)));
                                        stringAnswer = BitConverter.GetBytes((Double)ValueDouble);
                                    }
                                    else
                                    {
                                        {
                                            string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                            string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                            string[] values = pureElement.Split('|');
                                            Double[] ValueDouble = new Double[values.Length];
                                            for (int Count = 0; Count < values.Length; Count++)
                                            {
                                                ValueDouble[Count] = Convert.ToDouble(values[Count]);
                                            }
                                            Buffer.BlockCopy(ValueDouble, 0, stringAnswer, 0, stringAnswer.Length);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex);
                                    string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                    if (exceptionMessage != lastReadExceptionMessage)
                                    {
                                        lastReadExceptionMessage = exceptionMessage;
                                        CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                    }
                                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                    break;
                                }
                                break;
                            case (uint)BuiltInType.Float:
                                try
                                {
                                    if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                    {
                                        var ValueFloat = Convert.ToSingle(Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)));
                                        stringAnswer = BitConverter.GetBytes((Single)ValueFloat);
                                    }
                                    else
                                    {
                                        {
                                            string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                            string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                            string[] values = pureElement.Split('|');
                                            Single[] ValueFloat = new Single[values.Length];
                                            for (int Count = 0; Count < values.Length; Count++)
                                            {
                                                ValueFloat[Count] = Convert.ToSingle(values[Count]);
                                            }
                                            Buffer.BlockCopy(ValueFloat, 0, stringAnswer, 0, stringAnswer.Length);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex);
                                    string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                    if (exceptionMessage != lastReadExceptionMessage)
                                    {
                                        lastReadExceptionMessage = exceptionMessage;
                                        CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                    }
                                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                    break;
                                }
                                break;
                            case (uint)BuiltInType.String:
                                try
                                {
                                    if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                    {
                                        var ValueSt = Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                        string ValueString = (string)ValueSt;
                                        stringAnswer = Encoding.Default.GetBytes(ValueString);
                                    }
                                    else
                                    {
                                        {
                                            string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                            string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                            string[] values = pureElement.Split('|');
                                            Dim = 0;
                                            for (int jk = 0; jk < values.Length; jk++)
                                                Dim += values[jk].Length;
                                            Array.Resize<byte>(ref stringAnswer, Dim);
                                            int Offset = 0;
                                            byte[] ArrayOfByte = new byte[stringAnswer.Length];
                                            for (int jk = 0; jk < values.Length; jk++)
                                            {
                                                ArrayOfByte = Encoding.Default.GetBytes(values[jk]);
                                                Buffer.BlockCopy(ArrayOfByte, 0, stringAnswer, Offset, ArrayOfByte.Length);
                                                Offset += ArrayOfByte.Length;
                                            }
                                            //}
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex);
                                    string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                    if (exceptionMessage != lastReadExceptionMessage)
                                    {
                                        lastReadExceptionMessage = exceptionMessage;
                                        CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                    }
                                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                    break;
                                }
                                break;
                            case (uint)BuiltInType.UInt16:
                                try
                                {
                                    if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                    {
                                        var ValueUInt16 = Convert.ToUInt16(Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)));
                                        stringAnswer = BitConverter.GetBytes((UInt16)ValueUInt16);
                                    }
                                    else
                                    {
                                        {
                                            string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                            string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                            string[] values = pureElement.Split('|');
                                            UInt16[] ValueUInt16 = new UInt16[values.Length];
                                            for (int Count = 0; Count < values.Length; Count++)
                                            {
                                                ValueUInt16[Count] = Convert.ToUInt16(values[Count]);
                                            }
                                            Buffer.BlockCopy(ValueUInt16, 0, stringAnswer, 0, stringAnswer.Length);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex);
                                    string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                    if (exceptionMessage != lastReadExceptionMessage)
                                    {
                                        lastReadExceptionMessage = exceptionMessage;
                                        CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                    }
                                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                    break;
                                }
                                break;
                            case (uint)BuiltInType.UInt32:
                                try
                                {
                                    if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                    {
                                        var ValueUInt32 = Convert.ToUInt32(Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)));
                                        stringAnswer = BitConverter.GetBytes((UInt32)ValueUInt32);
                                    }
                                    else
                                    {
                                        {
                                            string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                            string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                            string[] values = pureElement.Split('|');
                                            UInt32[] ValueUInt32 = new UInt32[values.Length];
                                            for (int Count = 0; Count < values.Length; Count++)
                                            {
                                                ValueUInt32[Count] = Convert.ToUInt32(values[Count]);
                                            }
                                            Buffer.BlockCopy(ValueUInt32, 0, stringAnswer, 0, stringAnswer.Length);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex);
                                    string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                    if (exceptionMessage != lastReadExceptionMessage)
                                    {
                                        lastReadExceptionMessage = exceptionMessage;
                                        CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                    }
                                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                    break;
                                }
                                break;
                            case (uint)BuiltInType.UInt64:
                                try
                                {
                                    if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                    {
                                        var ValueUInt64 = Convert.ToUInt64(Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)));
                                        stringAnswer = BitConverter.GetBytes((UInt64)ValueUInt64);
                                    }
                                    else
                                    {
                                        {
                                            string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                            string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                            string[] values = pureElement.Split('|');
                                            UInt64[] ValueUInt64 = new UInt64[values.Length];
                                            for (int Count = 0; Count < values.Length; Count++)
                                            {
                                                ValueUInt64[Count] = Convert.ToUInt64(values[Count]);
                                            }
                                            Buffer.BlockCopy(ValueUInt64, 0, stringAnswer, 0, stringAnswer.Length);
                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine(ex);
                                    string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                    if (exceptionMessage != lastReadExceptionMessage)
                                    {
                                        lastReadExceptionMessage = exceptionMessage;
                                        CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                    }
                                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                    break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        switch ((uint)pendingjob[i].TagsList[0].TagNode.DataType.Identifier)
                        {
                            case (uint)BuiltInType.Boolean:
                                try
                            {
                                if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                {
                                    var ValueBool = Convert.ToBoolean(Row.ItemArray[1]);
                                    stringAnswer = BitConverter.GetBytes((Boolean)ValueBool);
                                }
                                else
                                {
                                    {
                                        string elementDB = Row.ItemArray[1].ToString();
                                        string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                        string[] values = pureElement.Split('|');
                                        Boolean[] ValueBoolean = new Boolean[values.Length];
                                        for (int Count = 0; Count < values.Length; Count++)
                                        {
                                            ValueBoolean[Count] = Convert.ToBoolean(values[Count]);
                                        }
                                        Buffer.BlockCopy(ValueBoolean, 0, stringAnswer, 0, stringAnswer.Length);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex);
                                string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                if (exceptionMessage != lastReadExceptionMessage)
                                {
                                    lastReadExceptionMessage = exceptionMessage;
                                    CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                }
                                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                break;
                            }
                            break;
                            case (uint)BuiltInType.Int16:
                                try
                            {
                                if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                {
                                    var ValueInt = Convert.ToInt16(Row.ItemArray[1]);
                                    stringAnswer = BitConverter.GetBytes((UInt16)ValueInt);
                                }
                                else
                                {
                                    {
                                        string elementDB = Row.ItemArray[1].ToString();
                                        string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                        string[] values = pureElement.Split('|');
                                        Int16[] ValueInt16 = new Int16[values.Length];
                                        for (int Count = 0; Count < values.Length; Count++)
                                        {
                                            ValueInt16[Count] = Convert.ToInt16(values[Count]);
                                        }
                                        Buffer.BlockCopy(ValueInt16, 0, stringAnswer, 0, stringAnswer.Length);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex);
                                string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                if (exceptionMessage != lastReadExceptionMessage)
                                {
                                    lastReadExceptionMessage = exceptionMessage;
                                    CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                }
                                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                break;
                            }
                            break;
                            case (uint)BuiltInType.Byte:
                                try
                            {
                                if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                {
                                    var ValueByte = Convert.ToByte(Row.ItemArray[1]);
                                    stringAnswer = BitConverter.GetBytes((Byte)ValueByte);
                                }
                                else
                                {
                                    {
                                        string elementDB = Row.ItemArray[1].ToString();
                                        string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                        string[] values = pureElement.Split('|');
                                        Byte[] ValueByte = new Byte[values.Length];
                                        for (int Count = 0; Count < values.Length; Count++)
                                        {
                                            ValueByte[Count] = Convert.ToByte(values[Count]);
                                        }
                                        Buffer.BlockCopy(ValueByte, 0, stringAnswer, 0, stringAnswer.Length);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex);
                                string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                if (exceptionMessage != lastReadExceptionMessage)
                                {
                                    lastReadExceptionMessage = exceptionMessage;
                                    CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                }
                                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                break;
                            }
                            break;
                            case (uint)BuiltInType.Int32:
                                try
                            {
                                if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                {
                                    var ValueInt32 = Convert.ToInt32(Row.ItemArray[1]);
                                    stringAnswer = BitConverter.GetBytes((Int32)ValueInt32);
                                }
                                else
                                {
                                    {
                                        string elementDB = Row.ItemArray[1].ToString();
                                        string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                        string[] values = pureElement.Split('|');
                                        Int32[] ValueInt32 = new Int32[values.Length];
                                        for (int Count = 0; Count < values.Length; Count++)
                                        {
                                            ValueInt32[Count] = Convert.ToInt32(values[Count]);
                                        }
                                        Buffer.BlockCopy(ValueInt32, 0, stringAnswer, 0, stringAnswer.Length);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex);
                                string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                if (exceptionMessage != lastReadExceptionMessage)
                                {
                                    lastReadExceptionMessage = exceptionMessage;
                                    CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                }
                                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                break;
                            }
                            break;
                            case (uint)BuiltInType.Int64:
                                try
                            {
                                if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                {
                                    var ValueInt64 = Convert.ToInt64(Row.ItemArray[1]);
                                    stringAnswer = BitConverter.GetBytes((Int64)ValueInt64);
                                }
                                else
                                {
                                    {
                                        string elementDB = Row.ItemArray[1].ToString();
                                        string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                        string[] values = pureElement.Split('|');
                                        Int64[] ValueInt64 = new Int64[values.Length];
                                        for (int Count = 0; Count < values.Length; Count++)
                                        {
                                            ValueInt64[Count] = Convert.ToInt64(values[Count]);
                                        }
                                        Buffer.BlockCopy(ValueInt64, 0, stringAnswer, 0, stringAnswer.Length);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex);
                                string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                if (exceptionMessage != lastReadExceptionMessage)
                                {
                                    lastReadExceptionMessage = exceptionMessage;
                                    CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                }
                                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                break;
                            }
                            break;
                            case (uint)BuiltInType.Double:
                                try
                            {
                                if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                {
                                    var ValueDouble = Convert.ToDouble(Row.ItemArray[1]);
                                    stringAnswer = BitConverter.GetBytes((Double)ValueDouble);
                                }
                                else
                                {
                                    {
                                        string elementDB = Row.ItemArray[1].ToString();
                                        string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                        string[] values = pureElement.Split('|');
                                        Double[] ValueDouble = new Double[values.Length];
                                        for (int Count = 0; Count < values.Length; Count++)
                                        {
                                            ValueDouble[Count] = Convert.ToDouble(values[Count]);
                                        }
                                        Buffer.BlockCopy(ValueDouble, 0, stringAnswer, 0, stringAnswer.Length);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex);
                                string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                if (exceptionMessage != lastReadExceptionMessage)
                                {
                                    lastReadExceptionMessage = exceptionMessage;
                                    CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                }
                                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                break;
                            }
                            break;
                            case (uint)BuiltInType.Float:
                                try
                            {
                                if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                {
                                    var ValueFloat = Convert.ToSingle(Row.ItemArray[1]);
                                    stringAnswer = BitConverter.GetBytes((Single)ValueFloat);
                                }
                                else
                                {
                                    {
                                        string elementDB = Row.ItemArray[1].ToString();
                                        string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                        string[] values = pureElement.Split('|');
                                        Single[] ValueFloat = new Single[values.Length];
                                        for (int Count = 0; Count < values.Length; Count++)
                                        {
                                            ValueFloat[Count] = Convert.ToSingle(values[Count]);
                                        }
                                        Buffer.BlockCopy(ValueFloat, 0, stringAnswer, 0, stringAnswer.Length);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex);
                                string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                if (exceptionMessage != lastReadExceptionMessage)
                                {
                                    lastReadExceptionMessage = exceptionMessage;
                                    CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                }
                                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                break;
                            }
                            break;
                            case (uint)BuiltInType.String:
                                try
                            {
                                if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                {
                                    var ValueSt = Row.ItemArray[1].ToString();
                                    string ValueString = (string)ValueSt;
                                    stringAnswer = Encoding.Default.GetBytes(ValueString);
                                }
                                else
                                {
                                    {
                                        string elementDB = Row.ItemArray[1].ToString();
                                        string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                        string[] values = pureElement.Split('|');
                                        Dim = 0;
                                        for (int jk = 0; jk < values.Length; jk++)
                                            Dim += values[jk].Length;
                                        Array.Resize<byte>(ref stringAnswer, Dim);
                                        int Offset = 0;
                                        byte[] ArrayOfByte = new byte[stringAnswer.Length];
                                        for (int jk = 0; jk < values.Length; jk++)
                                        {
                                            ArrayOfByte = Encoding.Default.GetBytes(values[jk]);
                                            Buffer.BlockCopy(ArrayOfByte, 0, stringAnswer, Offset, ArrayOfByte.Length);
                                            Offset += ArrayOfByte.Length;
                                        }
                                        //}
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex);
                                string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                if (exceptionMessage != lastReadExceptionMessage)
                                {
                                    lastReadExceptionMessage = exceptionMessage;
                                    CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                }
                                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                break;
                            }
                            break;
                            case (uint)BuiltInType.UInt16:
                                try
                            {
                                if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                {
                                    var ValueUInt16 = Convert.ToUInt16(Row.ItemArray[1]);
                                    stringAnswer = BitConverter.GetBytes((UInt16)ValueUInt16);
                                }
                                else
                                {
                                    {
                                        string elementDB = Row.ItemArray[1].ToString();
                                        string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                        string[] values = pureElement.Split('|');
                                        UInt16[] ValueUInt16 = new UInt16[values.Length];
                                        for (int Count = 0; Count < values.Length; Count++)
                                        {
                                            ValueUInt16[Count] = Convert.ToUInt16(values[Count]);
                                        }
                                        Buffer.BlockCopy(ValueUInt16, 0, stringAnswer, 0, stringAnswer.Length);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex);
                                string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                if (exceptionMessage != lastReadExceptionMessage)
                                {
                                    lastReadExceptionMessage = exceptionMessage;
                                    CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                }
                                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                break;
                            }
                            break;
                            case (uint)BuiltInType.UInt32:
                                try
                            {
                                if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                {
                                    var ValueUInt32 = Convert.ToUInt32(Row.ItemArray[1]);
                                    stringAnswer = BitConverter.GetBytes((UInt32)ValueUInt32);
                                }
                                else
                                {
                                    {
                                        string elementDB = Row.ItemArray[1].ToString();
                                        string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                        string[] values = pureElement.Split('|');
                                        UInt32[] ValueUInt32 = new UInt32[values.Length];
                                        for (int Count = 0; Count < values.Length; Count++)
                                        {
                                            ValueUInt32[Count] = Convert.ToUInt32(values[Count]);
                                        }
                                        Buffer.BlockCopy(ValueUInt32, 0, stringAnswer, 0, stringAnswer.Length);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex);
                                string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                if (exceptionMessage != lastReadExceptionMessage)
                                {
                                    lastReadExceptionMessage = exceptionMessage;
                                    CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                }
                                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                break;
                            }
                            break;
                            case (uint)BuiltInType.UInt64:
                                try
                            {
                                if (pendingjob[i].TagsList[0].TagNode.ArrayDimension == 0)
                                {
                                    var ValueUInt64 = Convert.ToUInt64(Row.ItemArray[1]);
                                    stringAnswer = BitConverter.GetBytes((UInt64)ValueUInt64);
                                }
                                else
                                {
                                    {
                                        string elementDB = Row.ItemArray[1].ToString();
                                        string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                        string[] values = pureElement.Split('|');
                                        UInt64[] ValueUInt64 = new UInt64[values.Length];
                                        for (int Count = 0; Count < values.Length; Count++)
                                        {
                                            ValueUInt64[Count] = Convert.ToUInt64(values[Count]);
                                        }
                                        Buffer.BlockCopy(ValueUInt64, 0, stringAnswer, 0, stringAnswer.Length);
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debug.WriteLine(ex);
                                string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                                if (exceptionMessage != lastReadExceptionMessage)
                                {
                                    lastReadExceptionMessage = exceptionMessage;
                                    CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                                }
                                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                                break;
                            }
                            break;
                        }
                    }
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    pendingjob[i].TagsList[0].Size = (uint)stringAnswer.Length;
                    var j = pendingjob[i];
                    ExecuteJob(j);
                    eJob.Job = pendingjob[i];
                    eJob.Values = stringAnswer;
                    eJob.Job = pendingjob[i];
                    eJob.Job.IsRead = true;
                    j.TagsListOnWriting.Clear();
                    RemovePendingJob(j);
                    //j.IsPending = false;
                    eJob.ErrorCode = LastErrorCode;
                    eJob.Job.StartExecutionTime = date;
                    OnJobExecuted(eJob);
                    j.lastExecutionTimeSchede = date;
                    }
                gridDataSet.Dispose();
                dbconnection.Close();
            }
            catch (Exception ex)
            {

                IsConnected = false;
                IsDeviceOpen();
                System.Diagnostics.Debug.WriteLine(ex);
                var sEx = ex as SqlException;
                if (sEx != null && sEx.Class >= 20)
                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorConnectionToDevice;
                else
                    LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorReadError;
                string exceptionMessage = string.Format(Properties.Resources.HandledException, ex.Message, command);
                if (exceptionMessage != lastReadExceptionMessage)
                {
                    lastReadExceptionMessage = exceptionMessage;
                    CommDriver.OnSystemEvent(null, exceptionMessage, Opc.Ua.EventSeverity.Max);
                }
                foreach (SQLDriverCommJob j in pendingjob)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = j;
                    eJob.ErrorCode = LastErrorCode;
                    OnJobExecuted(eJob);
                }
                dbconnection.Close();
                return 0;
            }
                return 1;
        }
        bool firstConnection = true;
        bool backupConnection = false;
        bool firstBackupConnection = true;
        protected int WriteToDatabase(ref int serror, SQLDriverStation s, List<SQLDriverCommJob> pendingjob)
        {
#if DEBUG
            DateTime dt = DateTime.Now;
            System.Diagnostics.Trace.TraceInformation("----§§§§§§ WriteToDatabase #:{0} {1}.{2}", pendingjob.Count, dt.ToLongTimeString(), dt.Millisecond);
#endif
            DateTime date = DateTime.UtcNow;
            DataSet WriteDataSet = new DataSet();
            if (!string.IsNullOrEmpty(s.SQLDriverColumnValue) && !string.IsNullOrEmpty(s.SQLDriverTableName) && !string.IsNullOrEmpty(s.SQLDriverColumnNameTagName))
            {
                WriteDataSet.Tables.Add(s.SQLDriverTableName);
                WriteDataSet.Tables[0].Columns.Add(new DataColumn()
                {
                    ColumnName = s.SQLDriverColumnNameTagName,
                    DataType = typeof(String),
                    Unique = true,
                    MaxLength = 250,
                    ColumnMapping = MappingType.Element
                });
                WriteDataSet.Tables[0].PrimaryKey = new DataColumn[] { WriteDataSet.Tables[0].Columns[s.SQLDriverColumnNameTagName] };
	            if (!s.SQLDriverMultiColumn)
	            {
	                WriteDataSet.Tables[0].Columns.Add(new DataColumn()
	                {
	                    ColumnName = s.SQLDriverColumnValue,
	                    DataType = typeof(String),
	                    Unique = false,
	                    MaxLength = 250,
	                    ColumnMapping = MappingType.Element
	                });
	            }
	            //else if (firstConnection && s.SQLDriverMultiColumn)
	            //{
	            //    var colArray = s.SQLDriverColumnValue.Split('|');
	
	            //    foreach (var c in colArray)
	            //    {
	            //        WriteDataSet.Tables[0].Columns.Add(new DataColumn()
	            //        {
	            //            ColumnName = c,
	            //            DataType = typeof(String),
	            //            Unique = false,
	            //            MaxLength = 250,
	            //            ColumnMapping = MappingType.Element
	            //        });
	            //    }
	            //}
	            else if (/*!firstConnection &&*/ s.SQLDriverMultiColumn)
	            {
		            List<String> columnToWrite = new List<string>();
	                foreach (var j in pendingjob)
	                {
	                    if (!columnToWrite.Contains(((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn))
	                    {
	                        columnToWrite.Add(((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn);
	                        WriteDataSet.Tables[0].Columns.Add(new DataColumn()
	                        {
	                            ColumnName = ((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn,
	                            DataType = typeof(String),
	                            Unique = false,
	                            MaxLength = 250,
	                            ColumnMapping = MappingType.Element
	                        });
	                    }
	                }
	            }
            }
            else
            {
                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorDriverSettings;
                return 0;
            }
            for (int i = 0; i < pendingjob.Count; i++)
            {
                WriteDataSet.Tables[0].PrimaryKey = new DataColumn[] { WriteDataSet.Tables[0].Columns[s.SQLDriverColumnNameTagName] };
                try
                {
                    pendingjob[i].TagsListToWrite.Clear();
                    WriteDataSet.Tables[0].Rows.Add(pendingjob[i].TagName, string.Format(CultureInfo.InvariantCulture, pendingjob[i].TagsList[0].Value.ToString()));
                }
                catch (Exception ex)
                {
                    if (!(firstConnection && s.SQLDriverMultiColumn))
                    {
                        LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorCreatingDataSet;
                        CommDriver.OnSystemEvent(null, ex.Message, Opc.Ua.EventSeverity.Max);
                        foreach (SQLDriverCommJob j in pendingjob)
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.Job = j;
                            eJob.ErrorCode = LastErrorCode;
                            OnJobExecuted(eJob);
                        }
                        return 0;
                    }
                    else
                    {
                        WriteDataSet.Tables[0].Rows.Find(pendingjob[i].TagName)[((SQLDriverDynTagSettings)pendingjob[i].TagsList[0].DynSettings).SQLDriverColumn] = string.Format(CultureInfo.InvariantCulture, pendingjob[i].TagsList[0].Value.ToString());
                    }
                }

            }
            try
            {
                OpenWriter();
                using (writer)
                {
                    if (firstConnection   || (firstBackupConnection && backupConnection))
                    {
                        
                        try
                        {
                            DataSet schema = new DataSet();
                            if (s.SQLDriverMultiColumn)
                            {
                                var colArray = s.SQLDriverColumnValue.Split('|');
                                schema.Tables.Add(s.SQLDriverTableName);
                                schema.Tables[0].Columns.Add(new DataColumn()
                                {
                                    ColumnName = s.SQLDriverColumnNameTagName,
                                    DataType = typeof(String),
                                    Unique = false,
                                    MaxLength = 250,
                                    ColumnMapping = MappingType.Element
                                });
                                foreach (var c in colArray)
                                {
                                    schema.Tables[0].Columns.Add(new DataColumn()
                                    {
                                        ColumnName = c,
                                        DataType = typeof(String),
                                        Unique = false,
                                        MaxLength = 250,
                                        ColumnMapping = MappingType.Element
                                    });
                                }
                            }
                            else
                                schema = WriteDataSet;
                            try
                            {
                                writer.CheckTables(schema);
                            }
                            catch (DataWriter.InvalidSchemaTableException ex)
                            {
                                CommDriver.OnSystemEvent(null, string.Format(Properties.Resources.InvalidSchemaDefinition, ex.Message), Opc.Ua.EventSeverity.MediumLow);
                            }
                            
                            firstConnection = false;
                            if (backupConnection)
                                firstBackupConnection = false;
                        }
                        catch (DataWriter.InvalidSchemaTableException ex)
                        {
                            LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorCreatingTable;
                            CommDriver.OnSystemEvent(null, ex.Message, Opc.Ua.EventSeverity.Max);
                            writer.CreateTable(WriteDataSet.Tables[0]);
                            firstConnection = false;
                            foreach (SQLDriverCommJob j in pendingjob)
                            {
                                ExecutedJobArgs eJob = new ExecutedJobArgs();
                                eJob.Job = j;
                                eJob.ErrorCode = LastErrorCode;
                                OnJobExecuted(eJob);
                            }
                            return 0;
                        }
                    }
                    try
                    {
                        DataView dataView = new DataView(WriteDataSet.Tables[0]);

                        foreach (DataRowView rowView in dataView)
                            rowView.Row.AcceptChanges();
                        
                        if (writer.UpdateRows(dataView, new List<String>(), new List<String>(), skipOrUseColumns: true, updateSingle:false) 
                            < WriteDataSet.Tables[0].Rows.Count)
                        try
                            {
                                writer.DeleteRows(dataView, true);
                                writer.InsertRows(dataView);
                            }
                            catch(Exception ex)
                            {
                                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorConnectionToDevice;
                                CommDriver.OnSystemEvent(null, ex.Message, Opc.Ua.EventSeverity.Max);
                            }
                        writer.Commit();
                        IsConnected = true;
                        lastWriteTime = DateTime.UtcNow;
#if DEBUG
                        System.Diagnostics.Trace.TraceInformation("lastWriteTime={0}.{1}", lastWriteTime.ToLongTimeString(), lastWriteTime.Millisecond);
#endif
                        for (int i = 0; i < pendingjob.Count; i++)
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            var j = pendingjob[i];
                            ExecuteJob(j);
                            eJob.Job = pendingjob[i];
                            eJob.Job.StartExecutionTime = date;
                            OnJobExecuted(eJob);
                            j.lastExecutionTimeSchede = date;
                            j.TagsList[0].LastValue = j.TagsList[0].Value;
                        }
                    }
                    catch(Exception ex)
                    {
                        var sEx = ex as SqlException;
                        if(sEx != null && sEx.Class >= 20)
                            LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorConnectionToDevice;
                        else
                            LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorWriteError;
                        CommDriver.OnSystemEvent(null, ex.Message, Opc.Ua.EventSeverity.Max);
                        foreach (SQLDriverCommJob j in pendingjob)
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.Job = j;
                            eJob.ErrorCode = LastErrorCode;
                            OnJobExecuted(eJob);
                        }
                        return 0;
                    }
                }
            }
            catch (Exception ex)
            {
                IsConnected = false;
                firstConnection = false;
                writer.CloseConnection();
                System.Diagnostics.Debug.WriteLine(ex);
                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorConnectionToDevice;
                CommDriver.OnSystemEvent(null, ex.Message, Opc.Ua.EventSeverity.Max);
                foreach (SQLDriverCommJob j in pendingjob)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = j;
                    eJob.ErrorCode = LastErrorCode;
                    OnJobExecuted(eJob);
                }
                return 0;
            }
            writer.CloseConnection();
            return 1;
        }

        private void OpenWriter()
        {
            try
            {
                if(!switchHost)
                {
                    writer = new DataWriter.DataSetWriter(Provider, ConnectionString);
                    writer.OpenConnection();
                    switchHost = false;
                    IsConnected = true;
                    backupConnection = false;
                    inactivityTimer = DateTime.Now;
                }
                else
                {
                    writer = new DataWriter.DataSetWriter(Provider, BackupConnectionString);
                    writer.OpenConnection();
                    IsConnected = true;
                    backupConnection = true;
                    inactivityTimer = DateTime.Now;
                }
            }
            catch(Exception Ex)
            {
                if (!(string.IsNullOrEmpty(BackupConnectionString) && string.IsNullOrEmpty(BackupProvider)) &&
                                  (((DateTime.Now - inactivityTimer).TotalMilliseconds > ChannelSwitchHostTimeout) || (switchHost == true)))
                {
                    switchHost = true;
                    writer = new DataWriter.DataSetWriter(BackupProvider, BackupConnectionString);
                    IsConnected = true;
                    backupConnection = true;
                    writer.OpenConnection();
                }
            }
            bool returnValue = IsConnected;
            SetStateCommandVariableBit(!returnValue, (UInt16)ChannelVariableBits.ChannelUnconnected);
            SetStateCommandVariableBit(selectedConnectionString != ConnectionString, (UInt16)ChannelVariableBits.ConnectedHost);
        }

        public override void ExecuteJob(CommJob job)
        {
            base.ExecuteJob(job);
            if(StatisticsData != null)
            {
                if (job.IsRead)
                {
                    foreach (var tag in job.TagsList)
                        DiagnLastTaskRxBytes += tag.Size;
                }
                else
                {
                    foreach (var tag in job.TagsList)
                        DiagnLastTaskTxBytes += tag.Size;
                }
            }
        }
        protected void WriteData(List<SQLDriverCommJob>list)
        {
#if DEBUG
            DateTime dt = DateTime.Now;
            System.Diagnostics.Trace.TraceInformation("----§§§§§§ WriteData #:{0} {1}.{2}", list.Count, dt.ToLongTimeString(), dt.Millisecond);
#endif
            int serror = 0;

            SQLDriverStation s = list[0].Station as SQLDriverStation;
            if ( s == null)
                return;

            WriteToDatabase(ref serror, s, list);
        }
     
#endregion

#region Properties


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Number of subscribed job in the channel. </summary>
        ///
        /// <value> The subscribed jobs. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public long SQLDriverSubscribedJobs
        {
            get
            {
                long result = 0;
                lock (lockSQLDriverListObject)
                {
                    result = jobCompleteList.Count;
                }

                return result;
            }
        }

#endregion

#region methods

#endregion

#region properties

        /// <summary>   Device Token. </summary>

        private string _ConnectionString;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the ConnectionString. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string ConnectionString
        {
            get { return _ConnectionString; }
        }

        /// <summary>   Device Token. </summary>
        private string _Provider;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Provider. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Provider
        {
            get { return _Provider; }
        }

        /// <summary>   Device Token. </summary>

        private string _BackupConnectionString;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the BackupConnectionString. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string BackupConnectionString
        {
            get { return _BackupConnectionString; }
        }

        /// <summary>   Device Token. </summary>
        private string _BackupProvider;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the BackupProvider. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <value> The Publish Key. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string BackupProvider
        {
            get { return _BackupProvider; }
        }

        /// <summary>   Switch Host timeout. </summary>
        private int _ChannelSwitchHostTimeout;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel settings Switch Host timeout. </summary>
        ///
        /// <value> The TCP channel settings Switc hHost timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int ChannelSwitchHostTimeout
        {
            get
            {
                return _ChannelSwitchHostTimeout;
            }
            set
            {
                _ChannelSwitchHostTimeout = value;
            }
        }

        private int _PublishingInterval;
        public int PublishingInterval
        {
            get { return _PublishingInterval; }
            set { _PublishingInterval = value; }
        }

        private int _MaxConsecutiveWrite;
        public int MaxConsecutiveWrite
        {
            get { return _MaxConsecutiveWrite; }
            set { _MaxConsecutiveWrite = value; }
        }
#endregion
    }

}
