using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
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
using System.Threading;
using System.Diagnostics;

namespace SQLDriver
{
    public struct WritingTag
    {
        /// <summary>   Identifier for the node. </summary>
        //public NodeId NodeId;
        public CommJob Job;
        public object Value;
    }
        public enum SQLDriverErrorCodes : int
        {
            ErrorConnectionToDevice = 1400,
            ErrorReadError = 1401,
            ErrorCreatingTable = 1402,
            ErrorWriteError = 1043,
            ErrorCreatingDataSet = 1044,
            ErrorDriverSettings = 1045,
            ErrorUnmappedTag = 1046
    }

    class SQLDriverChannel :  ChannelList
    {
        #region Constructors

        /// <summary>
        /// Initializes the SQLDriverChannel object.
        /// </summary>
        public SQLDriverChannel(SQLDriverDriver commdriver, SQLDriverChannelSettings settings)
            : base(commdriver, settings)
        {
            _Provider = settings.Provider;
            _ConnectionString = XpoHelpers.XpoHelper.NormalizeConnectionString(settings.ConnectionString, SQLDriverDriver.GetRootProjectPah(commdriver.StrConnectionString));
            _BackupConnectionString = settings.BackupConnectionString;
            _BackupProvider = settings.BackupProvider;
            _ChannelSwitchHostTimeout = settings.ChannelSwitchHostTimeout;
            _PublishingInterval = settings.PublishingInterval;

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
        // contain the list of database tables (station) whose existence on the database has been verified
        private Dictionary<string, bool> tableTested = new Dictionary<string, bool>();
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
            if(dbconnection != null)
            { 
                if (dbconnection.State == ConnectionState.Open)
                {
                    inactivityTimer = DateTime.Now;
                    IsConnected = true;
                    selectedProvider = Provider;
                    SetStateCommandVariableBit(!selectedConnectionString.Equals(ConnectionString), (UInt16)ChannelVariableBits.ConnectedHost);
                }
                else if (dbconnection.State == ConnectionState.Closed || dbconnection.State == ConnectionState.Broken)
                {
                    IsConnected = false;
                }
            }
            else
            {
                IsConnected = false;
            }
            bool returnValue = IsConnected;
            return returnValue;
        }

        bool PrimaryHostErrorState = false;
        bool BackupHostErrorState = false;
        bool varState;
        private int testDbBackupDelay_S = Properties.Settings.Default.testDbBackupDelay_S;

        string CurrentErrorMessage = string.Empty;
        string CurrentBackupErrorMessage = string.Empty;
        bool SelectPrimaryServer()
        {
            //Case if there is no backup server I remain on the primary server.
            if (string.IsNullOrWhiteSpace(BackupConnectionString) || string.IsNullOrWhiteSpace(BackupProvider))
                return true;

            //Case Switch request commanded by the conditional variable of the channel            
            bool PresenceOfTheSwitchServerTag = GetStateCommandVariableBit(ref varState, (UInt16)ChannelVariableBits.SwitchServer);
            if (PresenceOfTheSwitchServerTag && varState)
                return switchHost;

            //Case Switch request triggered by connected server connection error
            if (ChannelSwitchHostTimeout > 0 && (BackupHostErrorState || PrimaryHostErrorState))
            {
                if (((DateTime.Now - inactivityTimer).TotalMilliseconds > ChannelSwitchHostTimeout))
                {
                    inactivityTimer = DateTime.Now;
                    switchHost = !switchHost;
                    return switchHost;
                }
            }

            //I stay on the selected server
            return !switchHost;
        }
        public override bool DeviceOpen()
        {
            if(SelectPrimaryServer())
            {
                try//Primary server 
                {
                    if (varState)
                    {
                        varState = false;
                        SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.SwitchServer);
                    }

                    selectedConnectionString = ConnectionString;
                    if ((dbconnection != null) && (dbconnection.State == ConnectionState.Closed))
                        dbconnection.Dispose();

                    dbconnection = DataReader.DataReader.CreateDbConnection(Provider, ConnectionString);
                    
                    dbconnection.Open();
                    switchHost = false;
                    IsConnected = true;
                    PrimaryHostErrorState = false;
                    selectedProvider = Provider;
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
                    PrimaryHostErrorState = true;
                    dbconnection.Close();
                    dbconnection.Dispose();
                    dbconnection = null;
                }
            }
            else
            {
                try// Backup Server 
                {
                    if (varState)
                    {
                        varState = false;
                        SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.SwitchServer);
                    }

                    selectedConnectionString = BackupConnectionString;
                    if ((dbconnection != null) && (dbconnection.State == ConnectionState.Closed))
                        dbconnection.Dispose();

                    dbconnection = DataReader.DataReader.CreateDbConnection(BackupProvider, BackupConnectionString);
                
                    dbconnection.Open();
                    switchHost = true;
                    IsConnected = true;
                    BackupHostErrorState = false;
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
                    BackupHostErrorState = true;
 
                    dbconnection.Close();
                    dbconnection.Dispose();
                    dbconnection = null;
                }
                selectedProvider = BackupProvider;
            }
            SetStateCommandVariableBit(PrimaryHostErrorState, (UInt16)ChannelVariableBits.PrimaryHostErrorState);
            SetStateCommandVariableBit(BackupHostErrorState, (UInt16)ChannelVariableBits.BackupHostErrorState);
            SetStateCommandVariableBit((PrimaryHostErrorState && BackupHostErrorState), (UInt16)ChannelVariableBits.ChannelUnconnected);
            return IsConnected;
        }

        public override bool DeviceClose()
        {
            SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return true;
        }
        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        public override uint GetBytesToRead() { return 1; }
        public override uint GetBytesToWrite() { return 1; }

        #endregion

        #region Data Members

        List<SQLDriverCommJob> nextlist = new List<SQLDriverCommJob>();
        List<CommJob> ListJobExec = new List<CommJob>();

        DateTime lastWriteTime = DateTime.MinValue;

        string lastReadExceptionMessage = String.Empty;

        Dictionary<NodeId, WritingTag> TagToWriteCache;

        protected Timer WritingTimer;
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
        public override void Dispose()
        {
            AutoResetEvent waitHandle = null;
            lock (lockThreadObject)
            {
                if (WritingTimer != null)
                {
                    waitHandle = new AutoResetEvent(false);
                    WritingTimer.Dispose(waitHandle);
                    WritingTimer = null;
                }
            }
            if (waitHandle != null)
            {
                waitHandle.WaitOne();
                waitHandle.Dispose();
            }
            base.Dispose();

        }
        #endregion

        #region Local Methods


        public void AddTagToWriteCache(NodeId tagnodeid, WritingTag value)
        {
            
            if(PublishingInterval == 0 || lastWriteTime == DateTime.MinValue || (DateTime.UtcNow-lastWriteTime).TotalMilliseconds > PublishingInterval)
            {
                //write now
                PushJobToWrite(tagnodeid, value.Job, value.Value);
            }
            else
            {
                lock (lockThreadObject)
                { 
                    var dueTime =(int)( PublishingInterval - (DateTime.UtcNow - lastWriteTime).TotalMilliseconds);
                    //let the timer do the job
                    if (TagToWriteCache == null)
                        TagToWriteCache = new Dictionary<NodeId, WritingTag>();
                    TagToWriteCache[tagnodeid] = value;
                    //start timer
                    if(WritingTimer == null)
                        WritingTimer = new Timer(writingProcedure, this, dueTime, System.Threading.Timeout.Infinite);
                }
                
            }
        }

        private void writingProcedure(object state)
        {
            lock(lockThreadObject)
            {
                if (WritingTimer != null)
                {
                    WritingTimer.Dispose();
                    WritingTimer = null;
                }
                foreach(var t in TagToWriteCache)
                {
                    PushJobToWrite(t.Key, t.Value.Job, t.Value.Value);
                }
                TagToWriteCache.Clear();
            }
        }
        void PushJobToWrite(NodeId tagnodeid, CommJob job, object value)
        {
            uint ret = job.OnWriteTag(tagnodeid, ref value, true);
            if (ret == StatusCodes.Good)
            {
                // for job in PollingInError state, wait "normal" scheduling (to avoid station state var unstable value)
                if (!IsJobInPollingInErrorState(job))
                    ChangeStateJob(job, CommJobState.PollingNow);
            }
        }
        

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

        public override bool IsScheduledJobsListFull(List<CommJob> jobList)
        {
            return false;
        }

        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {
#if DEBUG
            Trace.WriteLine($"@@@-> {DateTime.Now.ToString("HH:mm:ss.fff")} - SplitInExecutionLists exList.Count: {exList.Count} - jobList.Count: {jobList.Count}");
#endif
            int jobIndex = 0;
            while (jobIndex < jobList.Count)
            {
                bool write = false;
                bool first = true;
                string station = string.Empty;
                List<CommJob> list = new List<CommJob>();
                string firstColumnValue = string.Empty;

                while (jobIndex < jobList.Count)
                {
                    SQLDriverCommJob j = jobList.ElementAt(jobIndex) as SQLDriverCommJob;
                    if (j != null)
                    {
                        if (first)
                        {
                            write = (j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;//j.TagsListToWrite.Count > 0 || j.Type == LinkType.UnconditionalOutput;
                            station = j.Station.Name;
                            first = false;
                            firstColumnValue = j.SQLDriverColumnValue;
                        }  
                       
                        bool jwrite = (j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;
                        if (j.Station != null && j.Station.Name == station && (write == jwrite))
                        {
                            // the condition is the tags can be aggregated only if they belong to the same column
                            if (write )
                            {
                                //if  (firstColumnValue == j.SQLDriverColumnValue)
                                //{
                                    list.Add(j);
                                    jobList.RemoveAt(jobIndex);
                                    jobIndex--;
                                //}
                            }
                            else if (j.Type != LinkType.ExceptionOutput)
                            {
                                list.Add(j);
                                jobList.RemoveAt(jobIndex);
                                jobIndex--;
                            }                                
                        }                       
                    }
                    jobIndex++;                    
                }
                
                if (list.Count > 0)
                {
                    //filled the list, a new one is required...                        
                    exList.Add(list);
#if DEBUG
                    Trace.WriteLine($"@@@-> {DateTime.Now.ToString("HH:mm:ss.fff")} -  SplitInExecutionLists Job list numer jobs: {list.Count}");
#endif
                    // restart from 0 especially to manage multistation channel
                    jobIndex = 0;
                }
            }
#if DEBUG
            Trace.WriteLine($"@@@-> {DateTime.Now.ToString("HH:mm:ss.fff")} -  SplitInExecutionLists exList numer of jobs List: {exList.Count}");
#endif
        }

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> exjoblist)
        {
            if (conn == DriverErrorCodes.ErrorDeviceOpenFailed)
            {
                SetJobsInError(DriverErrorCodes.ErrorTimeOut, exjoblist);
                return false;
            }
            ExecuteJobList(exjoblist);
            return false;
        }
        private void ExecuteJobList(List<CommJob> l)
        {
            List<SQLDriverCommJob> list = l.Cast<SQLDriverCommJob>().ToList();

            ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
            //using the property WaitTime > greater than 0, and consider on the first job on the list to decide whether to read or write,
            //it is possible to that within the list there are jobs that must only read,
            //and using the condition described above, the job that must only read, would be written.

            //if (list[0].Type == LinkType.Input || (list[0].Type == LinkType.InputOutput && list[0].TagsListToWrite.Count == 0))
            //    ReadData(list);
            //else
            //    WriteData(list);
            var listRead = list.FindAll(j => (j.Type == LinkType.Input || (j.Type == LinkType.InputOutput && j.GetTagListOnWritingCount() == 0)));
            if (listRead != null && listRead.Count > 0)
            {
                foreach(var j in listRead)
                {
                    if(list.Contains(j))
                    {
                        list.Remove(j);
                    }
                }
                ReadData(listRead);
            }
            if (list.Count > 0)
            {
                WriteData(list);
            }
 
        }

        protected void SetJobsInError(DriverErrorCodes error, List<CommJob> exjoblist)
        {
            ExecutedJobArgs e = new ExecutedJobArgs();

            for (int jobIndex = 1; jobIndex < exjoblist.Count; jobIndex++)
                OnJobExecuted(new ExecutedJobArgs() { Job = exjoblist[jobIndex], ErrorCode = DriverErrorCodes.ErrorTimeOut });

            // use first job's list to put driver in general error
            OnJobExecuted(new ExecutedJobArgs() { Job = exjoblist[0], ErrorCode = error });

            exjoblist.Clear();

            LastErrorCode = error;
        }

        protected void ReadData(List<SQLDriverCommJob> list)
        {
#if DEBUG
            DateTime dt = DateTime.Now;
            System.Diagnostics.Trace.TraceInformation("----§§§§§§ ReadData #:{0} {1}.{2}", list.Count, dt.ToLongTimeString(), dt.Millisecond);
#endif

            SQLDriverStation s = list[0].Station as SQLDriverStation;
            if (s == null)
                return;

            ReadFromDatabase(s, list);
        }

        protected void ReadRecordValue(string command, SQLDriverStation s, CommJob j, System.Data.DataRow Row, ref byte[] stringAnswer)
        {
            int Dim = 0;

            switch ((uint)j.TagsList[0].TagNode.DataType.Identifier)
            {
                case (uint)BuiltInType.Boolean:
                    if (j.TagsList[0].TagNode.ArrayDimension != 0)
                        Dim = 1 * (int)j.TagsList[0].TagNode.ArrayDimension;
                    else
                        Dim = 1;
                    break;
                case (uint)BuiltInType.Int16:
                    if (j.TagsList[0].TagNode.ArrayDimension != 0)
                        Dim = 2 * (int)j.TagsList[0].TagNode.ArrayDimension;
                    else
                        Dim = 2;
                    break;
                case (uint)BuiltInType.Byte:
                    if (j.TagsList[0].TagNode.ArrayDimension != 0)
                        Dim = 1 * (int)j.TagsList[0].TagNode.ArrayDimension;
                    else
                        Dim = 1;
                    break;
                case (uint)BuiltInType.SByte:
                    if (j.TagsList[0].TagNode.ArrayDimension != 0)
                        Dim = 1 * (int)j.TagsList[0].TagNode.ArrayDimension;
                    else
                        Dim = 1;
                    break;
                case (uint)BuiltInType.Int32:
                    if (j.TagsList[0].TagNode.ArrayDimension != 0)
                        Dim = 4 * (int)j.TagsList[0].TagNode.ArrayDimension;
                    else
                        Dim = 4;
                    break;
                case (uint)BuiltInType.Int64:
                    if (j.TagsList[0].TagNode.ArrayDimension != 0)
                        Dim = 8 * (int)j.TagsList[0].TagNode.ArrayDimension;
                    else
                        Dim = 8;
                    break;
                case (uint)BuiltInType.Double:
                    if (j.TagsList[0].TagNode.ArrayDimension != 0)
                        Dim = 8 * (int)j.TagsList[0].TagNode.ArrayDimension;
                    else
                        Dim = 8;
                    break;
                case (uint)BuiltInType.Float:
                    if (j.TagsList[0].TagNode.ArrayDimension != 0)
                        Dim = 4 * (int)j.TagsList[0].TagNode.ArrayDimension;
                    else
                        Dim = 4;
                    break;
                case (uint)BuiltInType.UInt16:
                    if (j.TagsList[0].TagNode.ArrayDimension != 0)
                        Dim = 2 * (int)j.TagsList[0].TagNode.ArrayDimension;
                    else
                        Dim = 2;
                    break;
                case (uint)BuiltInType.UInt32:
                    if (j.TagsList[0].TagNode.ArrayDimension != 0)
                        Dim = 4 * (int)j.TagsList[0].TagNode.ArrayDimension;
                    else
                        Dim = 4;
                    break;
                case (uint)BuiltInType.UInt64:
                    if (j.TagsList[0].TagNode.ArrayDimension != 0)
                        Dim = 8 * (int)j.TagsList[0].TagNode.ArrayDimension;
                    else
                        Dim = 8;
                    break;
            }
            stringAnswer = new byte[Dim];
            DiagnLastTaskRxBytes += Dim;
            if (s.SQLDriverMultiColumn)
            {
                switch ((uint)j.TagsList[0].TagNode.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        try
                        {
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueBool = Convert.ToBoolean(Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)));
                                stringAnswer = BitConverter.GetBytes((Boolean)ValueBool);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn));
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueInt = Convert.ToInt16(Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)));
                                stringAnswer = BitConverter.GetBytes((UInt16)ValueInt);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)).ToString();
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueByte = Convert.ToByte(Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)));
                                stringAnswer = BitConverter.GetBytes((Byte)ValueByte);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)).ToString();
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
                    case (uint)BuiltInType.SByte:
                        try
                        {
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueSByte = Convert.ToSByte(Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)));
                                stringAnswer = BitConverter.GetBytes((SByte)ValueSByte);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                    string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                    string[] values = pureElement.Split('|');
                                    SByte[] ValueByte = new SByte[values.Length];
                                    for (int Count = 0; Count < values.Length; Count++)
                                    {
                                        ValueByte[Count] = Convert.ToSByte(values[Count]);
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueInt32 = Convert.ToInt32(Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)));
                                stringAnswer = BitConverter.GetBytes((Int32)ValueInt32);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)).ToString();
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueInt64 = Convert.ToInt64(Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)));
                                stringAnswer = BitConverter.GetBytes((Int64)ValueInt64);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)).ToString();
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueDouble = Convert.ToDouble(Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)));
                                stringAnswer = BitConverter.GetBytes((Double)ValueDouble);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)).ToString();
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueFloat = Convert.ToSingle(Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)));
                                stringAnswer = BitConverter.GetBytes((Single)ValueFloat);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)).ToString();
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueSt = Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                stringAnswer = Encoding.UTF8.GetBytes(ValueSt);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)).ToString();
                                    string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                    pureElement = pureElement.Replace(" |", "|");
                                    string[] values = pureElement.Split('|');
                                    Dim = 0;
                                    //The elements of the string arrays must have the same size,
                                    //for this reason the longest string is searched.
                                    for (int jk = 0; jk < values.Length; jk++)
                                    {
                                        if (Encoding.UTF8.GetBytes(values[jk]).Length > Dim)
                                        {
                                            Dim = Encoding.UTF8.GetBytes(values[jk]).Length;
                                        }
                                    }
                                    Array.Resize<byte>(ref stringAnswer, (Dim * (int)j.TagsList[0].TagNode.ArrayDimension));
                                    int Offset = 0;
                                    for (int jk = 0; jk < values.Length; jk++)
                                    {
                                        Buffer.BlockCopy(Encoding.UTF8.GetBytes(values[jk]), 0, stringAnswer, Offset, Encoding.UTF8.GetBytes(values[jk]).Length);
                                        Offset += Dim;
                                    }
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueUInt16 = Convert.ToUInt16(Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)));
                                stringAnswer = BitConverter.GetBytes((UInt16)ValueUInt16);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)).ToString();
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueUInt32 = Convert.ToUInt32(Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)));
                                stringAnswer = BitConverter.GetBytes((UInt32)ValueUInt32);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)).ToString();
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueUInt64 = Convert.ToUInt64(Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)));
                                stringAnswer = BitConverter.GetBytes((UInt64)ValueUInt64);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.Field<string>((((SQLDriverDynTagSettings)j.TagsList[0].DynSettings).SQLDriverColumn)).ToString();
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
                switch ((uint)j.TagsList[0].TagNode.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        try
                        {
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
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
                    case (uint)BuiltInType.SByte:
                        try
                        {
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueSByte = Convert.ToSByte(Row.ItemArray[1]);
                                stringAnswer = BitConverter.GetBytes((SByte)ValueSByte);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.ItemArray[1].ToString();
                                    string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                    string[] values = pureElement.Split('|');
                                    SByte[] ValueByte = new SByte[values.Length];
                                    for (int Count = 0; Count < values.Length; Count++)
                                    {
                                        ValueByte[Count] = Convert.ToSByte(values[Count]);
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                var ValueSt = Row.ItemArray[1].ToString();
                                stringAnswer = Encoding.UTF8.GetBytes(ValueSt);
                            }
                            else
                            {
                                {
                                    string elementDB = Row.ItemArray[1].ToString();
                                    string pureElement = elementDB.Substring(1, elementDB.Length - 2);
                                    pureElement = pureElement.Replace(" |", "|");
                                    string[] values = pureElement.Split('|');
                                    Dim = 0;
                                    //The elements of the string arrays must have the same size,
                                    //for this reason the longest string is searched.
                                    for (int jk = 0; jk < values.Length; jk++)
                                    {
                                        if (Encoding.UTF8.GetBytes(values[jk]).Length > Dim)
                                        {
                                            Dim = Encoding.UTF8.GetBytes(values[jk]).Length;
                                        }
                                    }
                                    Array.Resize<byte>(ref stringAnswer, (Dim * (int)j.TagsList[0].TagNode.ArrayDimension));
                                    int Offset = 0;
                                    for (int jk = 0; jk < values.Length; jk++)
                                    {
                                        Buffer.BlockCopy(Encoding.UTF8.GetBytes(values[jk]), 0, stringAnswer, Offset, Encoding.UTF8.GetBytes(values[jk]).Length);
                                        Offset += Dim;
                                    }
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
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
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
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
        }

        protected int ReadFromDatabase(SQLDriverStation s, List<SQLDriverCommJob> pendingjob)
        {
            DateTime date = DateTime.UtcNow;
            string command = string.Empty;
            List<string> columnToRead = new List<string>();
#if DEBUG
            DateTime dt = DateTime.Now;
            Trace.WriteLine($"@@@-> {DateTime.Now.ToString("HH:mm:ss.fff")} -  ReadFromDatabase pendingjob numer of jobs: {pendingjob.Count}");
#endif

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

                if (s.SQLDriverMultiColumn)
                {
                    Dictionary<string, List<SQLDriverCommJob>> Arraylist = (from job in pendingjob
                                                                            group job by ((SQLDriverCommJob)job).TagName into jobsToGroup
                                                                            select new { Row = jobsToGroup.Key, Values = jobsToGroup.ToList() }).ToDictionary(t => t.Row, t => t.Values);
                    foreach (var groupedJobs in Arraylist)
                    {
                        byte[] stringAnswer = null;
                        LastErrorCode = DriverErrorCodes.ErrorNoError;

                        // search record by tagName
                        System.Data.DataRow[] Rows = gridDataSet.Tables[0].Select(s.SQLDriverColumnNameTagName + " = '" + groupedJobs.Key + "'");
                        // record found ?
                        if (Rows.Length > 0)
                        {
                            System.Data.DataRow Row = Rows[0]/*gridDataSet.Tables[0].Rows.Find(pendingjob[i].TagName)*/;

                            foreach (SQLDriverCommJob j in groupedJobs.Value)
                            {
                                // LastErrorCode = 0 inside ReadRecordValue
                                ReadRecordValue(command, s, j, Rows[0], ref stringAnswer);

                                ExecutedJobArgs eJob = new ExecutedJobArgs();
                                ExecuteJob(j);

                                if (LastErrorCode == DriverErrorCodes.ErrorNoError)
                                {
                                    j.TagsList[0].Size = (uint)stringAnswer.Length;
                                    eJob.Values = stringAnswer;
                                }
                                eJob.Job = j;
                                eJob.Job.IsRead = true;
                                j.TagsListOnWriting.Clear();
                                eJob.ErrorCode = LastErrorCode;
                                OnJobExecuted(eJob);
                                j.lastExecutionTimeSchede = date;
                            }
                        }

                        else
                        {
                            foreach (SQLDriverCommJob j in groupedJobs.Value)
                            {
                                LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorUnmappedTag;

                                ExecutedJobArgs eJob = new ExecutedJobArgs();
                                ExecuteJob(j);                                
                                eJob.Job = j;
                                eJob.Job.IsRead = true;
                                j.TagsListOnWriting.Clear();
                                eJob.ErrorCode = LastErrorCode;
                                OnJobExecuted(eJob);
                                j.lastExecutionTimeSchede = date;
                            }
                        }
                    }
                }
                else
                {
                    foreach (SQLDriverCommJob j in pendingjob)
                    {
                        byte[] stringAnswer = null;
                        LastErrorCode = DriverErrorCodes.ErrorNoError;

                        // search record by tagName
                        System.Data.DataRow[] Rows = gridDataSet.Tables[0].Select(s.SQLDriverColumnNameTagName + " = '" + j.TagName + "'");
                        // record found ?
                        if (Rows.Length > 0)
                        {
                            System.Data.DataRow Row = Rows[0]/*gridDataSet.Tables[0].Rows.Find(pendingjob[i].TagName)*/;

                            // LastErrorCode = 0 inside ReadRecordValue
                            ReadRecordValue(command, s, j, Rows[0], ref stringAnswer);
                        }
                        else
                        {
                            LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorUnmappedTag;
                        }

                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        ExecuteJob(j);

                        if (LastErrorCode == DriverErrorCodes.ErrorNoError)
                        {
                            j.TagsList[0].Size = (uint)stringAnswer.Length;
                            eJob.Values = stringAnswer;
                        }
                        eJob.Job = j;
                        eJob.Job.IsRead = true;
                        j.TagsListOnWriting.Clear();
                        eJob.ErrorCode = LastErrorCode;
                        OnJobExecuted(eJob);
                        j.lastExecutionTimeSchede = date;
                    }
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
#if DEBUG
            dt = DateTime.Now;
            Trace.WriteLine($"@@@-> {DateTime.Now.ToString("HH:mm:ss.fff")} - ReadFromDatabase END  pendingjob numer of jobs: {pendingjob.Count}");
#endif
            return 1;
        }

        bool backupConnection = false;
        bool firstBackupConnection = true;

        //Sort the list by column type
        protected int SortListbyColumnType(SQLDriverStation s, List<SQLDriverCommJob> pendingjob)
        {
            
            Dictionary<string, List<SQLDriverCommJob>> Arraylist = (from job in pendingjob                                                                  
                                                                  group job by ((SQLDriverCommJob)job).SQLDriverColumnValue into jobsToGroup
                                                                  select new { Column = jobsToGroup.Key, Values = jobsToGroup.ToList() }).ToDictionary(t => t.Column, t => t.Values);

            int ErrorCode = 0;
            foreach (var groupedJobs in Arraylist)
            {
                ErrorCode = WriteToDatabase(s, groupedJobs.Value);
            }
            return ErrorCode;
        }
        protected int WriteToDatabase(SQLDriverStation s, List<SQLDriverCommJob> pendingjob)
        {
            if(pendingjob.Count == 0) 
            {
                return 0;
            }
#if DEBUG
            DateTime dt = DateTime.Now;
            System.Diagnostics.Trace.TraceInformation("----§§§§§§ WriteToDatabase #:{0} {1}.{2}", pendingjob.Count, dt.ToLongTimeString(), dt.Millisecond);
            Trace.WriteLine($"@@@-> {DateTime.Now.ToString("HH:mm:ss.fff")} -  WriteToDatabase pendingjob numer of jobs: {pendingjob.Count}");
#endif
            DateTime date = DateTime.UtcNow;

            DataSet WriteDataSet = new DataSet();

            if (!string.IsNullOrEmpty(pendingjob[0].SQLDriverColumnValue) && !string.IsNullOrEmpty(s.SQLDriverTableName) && !string.IsNullOrEmpty(s.SQLDriverColumnNameTagName))
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

                WriteDataSet.Tables[0].Columns.Add(new DataColumn()
                {
                    ColumnName = pendingjob[0].SQLDriverColumnValue,
                    DataType = typeof(String),
                    Unique = false,
                    MaxLength = 250,
                    ColumnMapping = MappingType.Element
                });
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
                    pendingjob[i].ClearTagListOnWriting();
                    if (!WriteDataSet.Tables[0].Rows.Contains(pendingjob[i].TagName))
                    {
                        WriteDataSet.Tables[0].Rows.Add(pendingjob[i].TagName, String.Format(CultureInfo.InvariantCulture, "{0}", pendingjob[i].TagsList[0].WriteVal));
                    }
                    else
                    {
                        ((DataRow)WriteDataSet.Tables[0].Rows.Find(pendingjob[i].TagName))[pendingjob[i].SQLDriverColumnValue] = String.Format(CultureInfo.InvariantCulture, "{0}", pendingjob[i].TagsList[0].WriteVal);
                    }
                }
                catch (Exception ex)
                {
                    if (!(IsTableUnTested(s.Name) && s.SQLDriverMultiColumn))
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
                    if (IsTableUnTested(s.Name) || (firstBackupConnection && backupConnection))
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
                                // add value's fields to tables if not present
                                writer.CheckTables(schema, true);
                            }
                            catch (DataWriter.InvalidSchemaTableException ex)
                            {
                                CommDriver.OnSystemEvent(null, string.Format(Properties.Resources.InvalidSchemaDefinition, ex.Message), Opc.Ua.EventSeverity.MediumLow);
                            }

                            SetTableTested(s.Name);
                            if (backupConnection)
                                firstBackupConnection = false;
                        }
                        catch (DataWriter.InvalidSchemaTableException ex)
                        {
                            LastErrorCode = (DriverErrorCodes)SQLDriverErrorCodes.ErrorCreatingTable;
                            CommDriver.OnSystemEvent(null, ex.Message, Opc.Ua.EventSeverity.Max);
                            writer.CreateTable(WriteDataSet.Tables[0]);
                            SetTableTested(s.Name);
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

                        if (writer.UpdateRows(dataView, new List<String>(), new List<String>(), skipOrUseColumns: true, updateSingle: false)
                            < WriteDataSet.Tables[0].Rows.Count)
                            try
                            {
                                writer.DeleteRows(dataView, true);
                                writer.InsertRows(dataView);
                            }
                            catch (Exception ex)
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
                        for (int ix = 0; ix < pendingjob.Count; ix++)
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            var j = pendingjob[ix];
                            ExecuteJob(j);
                            eJob.Job = pendingjob[ix];
                            OnJobExecuted(eJob);
                            j.lastExecutionTimeSchede = date;                            
                        }
                    }
                    catch (Exception ex)
                    {
                        var sEx = ex as SqlException;
                        if (sEx != null && sEx.Class >= 20)
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
                SetTableTested(s.Name);
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
            //}
#if DEBUG
            Trace.WriteLine($"@@@-> {DateTime.Now.ToString("HH:mm:ss.fff")} -  WriteToDatabase End");
#endif
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

        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                // back compatibilty with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }
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
            return false;
        }

        protected void WriteData(List<SQLDriverCommJob>list)
        {
#if DEBUG
            DateTime dt = DateTime.Now;
            System.Diagnostics.Trace.TraceInformation("----§§§§§§ WriteData #:{0} {1}.{2}", list.Count, dt.ToLongTimeString(), dt.Millisecond);
#endif
            SQLDriverStation s = list[0].Station as SQLDriverStation;
            if (s == null)
                return;

            if(s.SQLDriverMultiColumn)
            {
                SortListbyColumnType(s, list);
            }
            else 
            {
                WriteToDatabase(s, list);
            }
        }

        private bool IsTableUnTested(string stationName)
        {
            return (!tableTested.ContainsKey(stationName));
        }

        private void SetTableTested(string stationName)
        {
            if (!tableTested.ContainsKey(stationName))
               tableTested[stationName] = true;
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

#endregion
    }

}
