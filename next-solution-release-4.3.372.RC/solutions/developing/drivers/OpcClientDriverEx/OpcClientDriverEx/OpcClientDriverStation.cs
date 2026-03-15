using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverBaseInterfaces;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using OPCUAViewModel;
using System.Threading;
using DriverCodeBaseEx.Helpers;
using System.Threading.Tasks;
using System.Globalization;
using Opc.Ua.Helpers;

namespace OpcClientDriver
{
    class OpcClientDriverStation : Station
    {
        #region Data
        protected readonly Dictionary<string, List<OpcClientDriverCommJob>> dictSessionToJobInitialized = new Dictionary<string, List<OpcClientDriverCommJob>>();
        protected readonly Dictionary<string, Timer> dictConnectionTimeoutTimer = new Dictionary<string, Timer>();
        protected readonly List<string> timeoutSessions = new List<string>();
        internal Dictionary<String, AppNameSettings> map = new Dictionary<String, AppNameSettings>();
        const int defaultDelay = 50;
        private long suspendCounter = 1;
        protected Dictionary<string, long> maxNodesPerWrite = new Dictionary<string, long>();
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public OpcClientDriverStation(CommunicationDriver commdriver, OpcClientDriverStationSettings settings)
            : base(commdriver, settings)
        {
            _RemoveDisabledItemAfterSecs = settings.RemoveDisabledItemAfterSecs.Value;
            _MaxCleanCount = settings.MaxCleanCount.Value;
            _UseAlwaysSecureConnections = settings.UseAlwaysSecureConnections.Value;
            _FastSamplingInterval = settings.FastSamplingInterval.Value;
            _SlowSamplingInterval = settings.SlowSamplingInterval.Value;
            _DisableWhenNotUsed = settings.DisableWhenNotUsed.Value;
            _PublishingInterval = settings.PublishingInterval.Value;
            _UsePollingRead = settings.UsePollingRead;
            _ConnectionTimeout = settings.ConnectionTimeout.Value;
            MaxRetriesBeforeError = 0;
            _SettingsApplyed = false;
            _User = settings.User;
            _Password = settings.Password;
            _UseServerDiscovery = settings.UseServerDiscovery.Value;
            _UseLocalTimestamp = settings.UseLocalTimestamp.Value;
            // standard behaviour of OPC Driver
            RewritingOfTheSameValue = true;
        }

        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as OpcClientDriverCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new OpcClientDriverCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as OpcClientDriverTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new OpcClientDriverCommJob(this, conf);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as OpcClientDriverCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new OpcClientDriverCommJobSettings(session, commJob);
        }
        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new OpcClientDriverTag(td);
        }

        #endregion


        #region Overrides

        public override void OnJobExecuted(object sender, ExecutedJobArgs e)
        {
            if (e.Job.Station != this)
            {
                return;
            }

            ProcessJobValues(e);
            ProcessJobLists(e.Job, e.ErrorCode);

            // set quality only if in error (lo stato di buona qualità viene impostato nella DataValueChanged() method
            if (e.ChangedTags.Count == 0 && e.ErrorCode != DriverErrorCodes.ErrorNoError)
                e.Job.SetErrorState((int)e.ErrorCode);

            if (e.Job.SetRWState(e.ErrorCode))
            {
                ChannelBase.ChangeStateJob(e.Job, CommJobState.PollingNow);
            }

            if (StatisticsData != null)
            {
                if (InErrorState)
                    StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), true);
                else
                    StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), false);
            }
            e.Job.IsPending = false;
        }


        public override void ParseDynamicTagsStructSplit(Tag tag, List<Tag> innerList, List<Tag> lMethods)
        {            
            OpcClientDriverDynTagSettings dtCalc = tag.DynSettings as OpcClientDriverDynTagSettings;
            if (tag.TagNode.DataType.IdType == IdType.Guid && CommDriver.IsPrototypeSplitEnabled() && !dtCalc.Struct_IEC61131_3)
            {
                //struttura
                List<TagDefinition> pList = new List<TagDefinition>();
                GetPrototypeTagList(tag.TagNode.NodeId, ref pList);
                string dynsettings = string.Empty;
                TagDefinition olddtag;
                for (int i = 0; i < pList.Count; i++)
                {

                    TagDefinition dtag = pList[i];
                    olddtag = dtag;
                    if (dtag.DynamicSettings.Length == 0)
                    {
                        dynsettings = dtCalc.GetFirstDynSetting(tag, dtag);
                        dtag.DynamicSettings = dynsettings;
                    }
                    //get dynsettings for each tagdefinition.
                    Tag t = GetCommDriver().CreateTag(dtag);
                    if (t.DynSettings.MethodID > -1)
                    {
                        //ricordati del metodo...
                        lMethods.Add(t);
                        if (dynsettings.Length > 0)
                            dtag = olddtag;
                        continue;
                    }
                    else
                    {
                        if (t.bIsValid)
                        {
                            innerList.Add(t);
                        }
                        else
                        {
                            CommDriver.OnTagChanged(t.TagNode.NodeId, new DataValue(StatusCodes.BadConfigurationError));
                            CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidTagDefinition, CommDriver.DriverName, t.TagNode.NodeId.Identifier, t.InvalidReason), System.Diagnostics.EventLogEntryType.Error);
                        }
                    }
                }
            }
            else
            {
                if (tag.DynSettings.MethodID > -1)
                    lMethods.Add(tag);
                innerList.Add(tag);
            }                    
        }

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            OpcClientDriverCommJob mJ = e.Job as OpcClientDriverCommJob;
            if (mJ == null || mJ.TagsList == null || mJ.TagsList.Count == 0 || mJ.TagsList[0].Value == null || e.Values == null)
                return;

            if (StatisticsData != null)
            {
                if (e.ErrorCode != DriverErrorCodes.ErrorNoError)
                {
                    StatisticsData.Update(StatisticSetting.NodeDataNames.LastErrorTime.ToString(), DateTime.Now.ToString());
                    uint quality;
                    String DiagnErrorMessage = "";
                    CommDriver.GetDriverErrorInfo((int)e.ErrorCode, out quality, out DiagnErrorMessage);
                    StatisticsData.Update(StatisticSetting.NodeDataNames.LastError.ToString(), DiagnErrorMessage);
                    StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalJobsError.ToString());

                    if(Channel.StatisticsData != null)
                    {
                        Channel.StatisticsData.Update(StatisticSetting.NodeDataNames.LastErrorTime.ToString(), DateTime.Now.ToString());
                        Channel.StatisticsData.Update(StatisticSetting.NodeDataNames.LastError.ToString(), DiagnErrorMessage);
                        Channel.StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalJobsError.ToString());
                    }
                }
            }

            //if ((e.ErrorCode != DriverErrorCodes.ErrorNoError))
            //{
            //    SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
            //}
            //else
            //{
            //    if (Channel.InErrorJobs(this) == 0)
            //    {
            //        SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
            //    }
            //    e.Job.ResetConditionalVariable();
            //}
            //if changedTags, notify all of them. (it's a structure) data decoding already done in receiva event (ugly!!!)
            DateTime currentTime = DateTime.UtcNow;
            if (e.ChangedTags.Count > 0)
            {
                foreach (var t in e.ChangedTags)
                {
                    var rt = mJ.TagsList.FindIndex(o => o.TagNode.NodeId == t.TagNode.NodeId);
                    if (rt != -1)
                    {
                        mJ.TagsList[rt].Value.Value = Utils.Clone(t.Value.Value);
                        mJ.TagsList[rt].Value.StatusCode = t.Value.StatusCode;
                        if(UseLocalTimestamp == false)
                        {
                            mJ.TagsList[rt].Value.SourceTimestamp = t.Value.SourceTimestamp;
                            mJ.TagsList[rt].Value.ServerTimestamp = t.Value.ServerTimestamp;
                        }
                        else
                        {
                            mJ.TagsList[rt].Value.SourceTimestamp = currentTime;
                            mJ.TagsList[rt].Value.ServerTimestamp = currentTime;
                        }
                        GetCommDriver().OnTagChanged(mJ.TagsList[rt].TagNode.NodeId, mJ.TagsList[rt].Value, mJ.TagsList[rt].Value.SourceTimestamp);
                    }
                }
                return;
            }
            var value = e.Values as Opc.Ua.DataValue;
            mJ.TagsList[0].Value.Value = Utils.Clone(value.Value);
            mJ.TagsList[0].Value.StatusCode = value.StatusCode;
            if (UseLocalTimestamp == false)
            {
                mJ.TagsList[0].Value.SourceTimestamp = value.SourceTimestamp;
                mJ.TagsList[0].Value.ServerTimestamp = value.ServerTimestamp;
            }
            else
            {
                mJ.TagsList[0].Value.SourceTimestamp = currentTime;
                mJ.TagsList[0].Value.ServerTimestamp = currentTime;
            }

            GetCommDriver().OnTagChanged(mJ.TagsList[0].TagNode.NodeId, mJ.TagsList[0].Value, mJ.TagsList[0].Value.SourceTimestamp);
        }
        int stationSampling = -1;
        public override InUseStates SetInUse(NodeId tagnodeid, bool bInUse, double samplinginterval = -1)
        {
            InUseStates retInUseState = InUseStates.InvalidTagNodeId;
            OpcClientDriverCommJob job = null;
            var listWholeJob = new List<CommJob>();
            lock (lockListObject)
            {
                NodeId n = tagnodeid;
                if (!mapTagJob.ContainsKey(n))
                {
                    if (tagnodeid.IdType == IdType.String)
                    {
                        string s = tagnodeid.ToString();
                        int p = s.IndexOf("?");
                        if (p != -1)
                            s = s.Substring(0, p);
                        s = s.Replace(";s=", ";g=");
                        n = new NodeId(s);
                        if (!mapTagJob.ContainsKey(n))
                            return retInUseState;
                    }
                    else
                        return retInUseState;
                }

                job = mapTagJob[n] as OpcClientDriverCommJob;
                if (job == null)
                    return retInUseState;
                listWholeJob.AddRange(ListWholeJob);
            }

            var inuse = job.InUse;
            if (Interlocked.Read(ref suspendCounter) == 0)
            { 
                if ((!inuse && bInUse)) //put in use
                {
                    job.PrepareExecution(Name, listWholeJob);
                }
                else if (inuse && !bInUse)//put not in use
                    job.Unsubscribe(Name);
            }

            bool ret = job.SetInUse(tagnodeid, bInUse, samplinginterval);

            if (ChannelBase != null && job.InUse != inuse)
            {
                if (job.InUse)
                {
                    retInUseState = InUseStates.InUse;
                    ChannelBase.ChangeStateJob(job, CommJobState.PollingInUse);
                }
                else
                {
                    retInUseState = InUseStates.NotInUse;
                    ChannelBase.ChangeStateJob(job, CommJobState.PollingNotInUse);
                }
            }

            int oldStationSampling = stationSampling;
            if (stationSampling == -1 || (samplinginterval > -1 && stationSampling > samplinginterval && !bInUse))
            {
                //calculate
                var lst = (from j in listWholeJob where j.InUse == true orderby j.SamplingInterval ascending select j.SamplingInterval).ToList();
                if(lst.Count > 0)
                    stationSampling = Convert.ToInt32(lst[0]);
            }
            else if (samplinginterval > -1 && stationSampling > samplinginterval && bInUse)
            {
                stationSampling = Convert.ToInt32(samplinginterval);
            }

            if (oldStationSampling != stationSampling && SettingsApplyed)
            {
                var currSettings = RealTimeConnectionManagerViewModel.GetSession(Name);
                    currSettings.FastSamplingInterval = stationSampling;
                RealTimeConnectionManagerViewModel.AddSessionSettings(Name, currSettings);
            }
                
            return retInUseState;
        }
        
        public void SetSessionSettings(string sessionname, string[] serverArray)
        {
            if (!SettingsApplyed)
            {
                int slowSI = (SlowSamplingInterval == FastSamplingInterval ? SlowSamplingInterval + 1 : SlowSamplingInterval);
                RealTimeConnectionManagerViewModel.AddSessionSettings(sessionname,
                new SessionSettings()
                {
                    UserName = User,
                    Password = Password,
                    DisableWhenNotUsed = DisableWhenNotUsed,
                    FastSamplingInterval = FastSamplingInterval,
                    MaxCleanCount = MaxCleanCount,
                    PublishingInterval = PublishingInterval,
                    RemoveDisabledItemAfterSecs = RemoveDisabledItemAfterSecs,
                    SlowSamplingInterval = slowSI,
                    UseAlwaysSecureConnections = UseAlwaysSecureConnections,
                    MapAppNameSettings = map,                    
                    ServerArray = serverArray
                }); 
                SettingsApplyed = true;
            }
        }

        public override bool Startup()
        {
            exitQueueTagToWrite = false;

            var interlokCounter = Interlocked.Decrement(ref suspendCounter);
            if (interlokCounter != 0)
                return true;

            var listWholeJob = new List<CommJob>();
            //SetSessionSettings(Name);
            lock (lockListObject)
            {
                listWholeJob.AddRange(ListWholeJob);
            }

            var listJob = new List<CommJob>();
            var listJobNotInUse = new List<CommJob>();
            var listJobQuality = new List<CommJob>();
            Parallel.ForEach(listWholeJob, job =>
            {
                if (job.InUse)
                {
                    lock (listJob)
                    {
                        listJob.Add(job);
                    }
                }
                else
                {
                    lock (listJobNotInUse)
                    {
                        listJobNotInUse.Add(job);
                    }
                }

                if ((job.Type != LinkType.InputOutput) && (job.Type != LinkType.Input))
                {
                    lock (listJobQuality)
                    {
                        listJobQuality.Add(job);
                    }
                }
            });

            foreach (var job in listJobQuality)
            {
                job.SetUncertainQuality();
            }

            foreach (var job in listJob)
            {
                (job as OpcClientDriverCommJob).PrepareExecution(Name, listJob);
            }

            if (Channel != null)
            {
                foreach (var job in listJob)
                {
                    Channel.SubscribeJob(job, CommJobState.PollingInUse);
                }
                foreach (var job in listJobNotInUse)
                {
                    Channel.SubscribeJob(job, CommJobState.PollingNotInUse);
                }
            }

            return true;
        }
                  
        internal void SuspendChannel()
        {
            var channel = this.GetChannel();
            var commDriver = this.GetCommDriver();
            var stations = commDriver.GetChannelStations(channel);
            
            bool suspendBitNewValue = false;
            int suspendedStation = 0;
            foreach (var c in stations)
            {
                if (c.GetStateCommandVariableBit(ref suspendBitNewValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
                {
                    if(suspendBitNewValue)
                    {
                        suspendedStation++;
                    }
                    suspendBitNewValue = false;
                }
            }

            if (stations.Count == suspendedStation)
            {
                foreach(var s in stations)
                    RealTimeConnectionManagerViewModel.CleanSpecificDeadConnection(s.Name);
            }
        }

        public override uint ExecuteSyncroJob(CommJob job, bool localmethod, int methodid, IList<object> args, bool takedata = true, NodeId tagNodeId = null, object value = null)
        {
            uint ret = StatusCodes.Good;

            job.LocalMethod = localmethod;
            job.MethodID = methodid;

            Channel.Startup();

            Channel.commExecuteSyncro(job, tagNodeId, value);

            args[0] = job.SynchroError;

            //prendi i dati o il risultato  dell'esecuzione e ritorna
            if (takedata && job.SynchroError == (int)DriverErrorCodes.ErrorNoError &&
                localmethod == false && methodid == (int)DriverSynchroOperation.ReadSynchro)
            {
                if (job.SynchroError == (int)DriverErrorCodes.ErrorNoError)
                {
                    args[0] = StatusCodes.Good;
                    args[1] = job.TagsList[0].Value.Value;
                }
                else
                {
                    args[0] = StatusCodes.BadArgumentsMissing;
                }
            }
#if DEBUG
            System.Diagnostics.Trace.TraceInformation("{4} ExecuteSyncroJob ret:{0} err:{1} {2}.{3}", ret, args[0], DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond, Thread.CurrentThread.ManagedThreadId);
#endif
            return ret;
        }

        private bool IsJobSubscribedToStation(CommJob job)
        {
            bool subscribed = false;

            lock (lockListObject)
                subscribed = (mapTagJob.ContainsKey(job.TagsList[0].TagNode.NodeId));

            return subscribed;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the syncro job operation. </summary>
        ///
        /// <param name="job" type="CommJob">           The job. </param>
        ///        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ExecuteSyncroJob(CommJob job)
        {
            OpcClientDriverCommJob opcJob = (OpcClientDriverCommJob)job;
            string mySessionName = Name;

            DriverErrorCodes errorCode = DriverErrorCodes.ErrorNoError;
            ManualResetEvent syncDataValueChangedCompleted = null;
            ManualResetEvent syncNodeIdViewModelCompleted = null;
            bool unSubscribeJob = false;
            
            ExecutedJobArgs e = new ExecutedJobArgs();
            e.Job = job;

            object jobData = null;
            NodeId jobDataNodeId = null;
            bool read = job.ReadRequest();
            if (!read)
            {
                lock (lockListObject)
                {
                    if (job.Type == LinkType.UnconditionalOutput)
                    {                       
                        jobData = (((OpcClientDriverTag)job.TagsList[0]).GetOpcWriteVal());
                        jobDataNodeId = job.TagsList[0].TagNode.NodeId;
                    }
                    else
                    {
                        if (job.TagsListOnWriting.Count > 0)
                        {
                            jobData = ((OpcClientDriverTag)job.TagsListOnWriting[0]).GetOpcWriteVal();
                            jobDataNodeId = job.TagsListOnWriting[0].TagNode.NodeId;
                        }
                    }
                }
                if (jobData == null)
                    errorCode = (DriverErrorCodes)OpcClientErrorCodes.OpcClientErrorReadWriteFailed;
            }
            
            if (errorCode == DriverErrorCodes.ErrorNoError)
            {
                // check if job is external to the driver (recipe, etc)
                bool jobSubscribed = IsJobSubscribedToStation(job);
                if (!jobSubscribed)
                {
                    unSubscribeJob = true;
                    syncDataValueChangedCompleted = new ManualResetEvent(false);
                    syncNodeIdViewModelCompleted = new ManualResetEvent(false);
                    if (opcJob.PrepareExecution(mySessionName, new List<CommJob> { job }, syncDataValueChangedCompleted, syncNodeIdViewModelCompleted))
                    {
                        // wait opc call back of 2 specific events
                        if (WaitHandle.WaitAll(new WaitHandle[] { syncDataValueChangedCompleted, syncNodeIdViewModelCompleted }, ConnectionTimeout))
                            jobSubscribed = true;
                        else
                            errorCode = (DriverErrorCodes)OpcClientErrorCodes.OpcClientErrorCodeBadConnetion;
                    }
                    else
                    {
                        errorCode = (DriverErrorCodes)OpcClientErrorCodes.OpcClientErrorCodeBadConnetion;
                    }
                }
                
                if (jobSubscribed)
                {
                    if (read)
                    {
                        if (opcJob.SyncReadOpcValue(out DataValue readValue) != StatusCodes.Good)
                            errorCode = (DriverErrorCodes)OpcClientErrorCodes.OpcClientErrorReadWriteFailed;
                        else
                            e.Values = readValue;
                    }
                    else
                    {
                        if (opcJob.SyncWriteOpcValue(jobDataNodeId, ref jobData) != StatusCodes.Good)
                            errorCode = (DriverErrorCodes)OpcClientErrorCodes.OpcClientErrorReadWriteFailed;
                    }
                }

                if (syncDataValueChangedCompleted != null)
                {
                    syncDataValueChangedCompleted.Dispose();
                    syncDataValueChangedCompleted = null;
                }
                if (syncNodeIdViewModelCompleted != null)
                {
                    syncNodeIdViewModelCompleted.Dispose();
                    syncNodeIdViewModelCompleted = null;
                }
            }

            e.ErrorCode = errorCode;
            if (e.Job.LocalMethod == false)
            {
                e.Job.SynchroValues = e.Values;
                e.Job.SynchroError = (int)e.ErrorCode;
            }
            OnJobExecuted(job, e);

            // once terminated, if job is external to the driver (recipe, etc), unsubscribe it
            if (unSubscribeJob)
                opcJob.Unsubscribe(mySessionName);
        }

        public override bool Suspend(bool setJobQuality = true)
        {
            var interlokCounter = Interlocked.Increment(ref suspendCounter);
            if (interlokCounter != 1 && !setJobQuality)
                return true;

            if (ChannelBase == null)
            {
                //Init failed!!!!
                CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.SuspendStationNotInitialized, CommDriver.DriverName, Name), EventSeverity.High);
                return false;
            }

            // stops (if running) the thread used to spool the values ​​to be written
            ExitPendingWriteJobsThreads();

            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                if (setJobQuality)
                    listJob.AddRange(ListWholeJob);
                else
                    listJob = (from j in ListWholeJob.AsParallel() where j.InUse == true select j).ToList();
            }

            foreach (var job in listJob)
            {
                if (setJobQuality)
                    (job as OpcClientDriverCommJob).SetUncertainLastUsableValueQuality();
                (job as OpcClientDriverCommJob).Unsubscribe(Name);
            }
            SuspendChannel();
            return true;
        }

        public override bool Terminate()
        {
            return true;
        }
        
        public override void Dispose()
        {
            ExitPendingWriteJobsThreads();

            var waitHandles = new List<WaitHandle>();
            lock (lockListObject)
            {
                foreach (var timer in dictConnectionTimeoutTimer.Values)
                {
                    var waitHandle = new AutoResetEvent(false);
                    timer.Change(0, Timeout.Infinite);
                    timer.Dispose(waitHandle);
                    waitHandles.Add(waitHandle);
                }

                dictConnectionTimeoutTimer.Clear();
                dictSessionToJobInitialized.Clear();
                timeoutSessions.Clear();
            }

            if (waitHandles.Count > 0)
            {
                WaitHandle.WaitAll(waitHandles.ToArray());
                waitHandles.ForEach((notify) => notify.Dispose());
            }

            foreach(var job in ListWholeJob)
            {
                var opcJob = job as IDisposable;
                if (opcJob != null)
                    opcJob.Dispose();
            }

            base.Dispose();
        }
        #endregion

        #region methods
        internal void AddJobInitialized(OpcClientDriverCommJob job, string sessionName)
        {
            lock (lockListObject)
            { 
                if (timeoutSessions.Contains(sessionName))
                {
                    //timeout elapsed!!
                    ExecutedJobArgs eJob = new ExecutedJobArgs() { Job = job };
                    eJob.ErrorCode = DriverErrorCodes.ErrorTimeOut;
                    OnJobExecuted(job, eJob);
                    SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                    return;
                }

                if (!dictSessionToJobInitialized.ContainsKey(sessionName))
                    dictSessionToJobInitialized.Add(sessionName, new List<OpcClientDriverCommJob>());
                if (!dictSessionToJobInitialized[sessionName].Contains(job))
                    dictSessionToJobInitialized[sessionName].Add(job);

                StartTimeoutTimer(ConnectionTimeout, sessionName);
            }
        }

        internal void StopTimeoutTimer(string sessionName)
        {
            AutoResetEvent waitHandle = null;
            var commJobs = new List<OpcClientDriverCommJob>();
            lock (lockListObject)
            {
                if (dictConnectionTimeoutTimer.ContainsKey(sessionName))
                {
                    var timer = dictConnectionTimeoutTimer[sessionName];
                    dictConnectionTimeoutTimer.Remove(sessionName);
                    waitHandle = new AutoResetEvent(false);
                    timer.Change(0, System.Threading.Timeout.Infinite);
                    timer.Dispose(waitHandle);
                }

                if (dictSessionToJobInitialized.ContainsKey(sessionName))
                {
                    commJobs.AddRange(dictSessionToJobInitialized[sessionName]);
                    dictSessionToJobInitialized.Remove(sessionName);
                }
                if (timeoutSessions.Contains(sessionName))
                    timeoutSessions.Remove(sessionName);
            }

            if (waitHandle != null)
            {
                waitHandle.WaitOne();
                waitHandle.Dispose();
            }

            foreach (var commJob in commJobs)
            {
                SessionViewModel session = null;
                if (commJob.OPCItem.MonitoredItemViewModel != null)
                    session = commJob.OPCItem.MonitoredItemViewModel.GetSubscriptionViewModelParent().GetSessionViewModelParent();
                if (session != null && session.Connected)
                    commJob.SetErrorState((int)OpcClientErrorCodes.OpcClientErrorCodeBadWaitingForInitialData);
            }
        }

        void StartTimeoutTimer(int delay, string sessionName)
        {
            lock (lockListObject)
            {
                if (timeoutSessions.Contains(sessionName) || 
                    dictConnectionTimeoutTimer.ContainsKey(sessionName) || 
                    string.IsNullOrEmpty(sessionName))
                    return;

                var timer = new Timer((o) =>
                {
                    var commJobs = new List<OpcClientDriverCommJob>();
                    lock (lockListObject)
                    {
                        if (dictConnectionTimeoutTimer.ContainsKey(sessionName))
                        {
                            dictConnectionTimeoutTimer[sessionName].Dispose();
                            dictConnectionTimeoutTimer.Remove(sessionName);
                        }

                        if (dictSessionToJobInitialized.ContainsKey(sessionName))
                        {
                            commJobs.AddRange(dictSessionToJobInitialized[sessionName]);
                            if (!timeoutSessions.Contains(sessionName))
                                timeoutSessions.Add(sessionName);
                        }
                    }

                    foreach (var commJob in commJobs)
                    {
                        SessionViewModel session = null;
                        if (commJob.OPCItem.MonitoredItemViewModel != null)
                            session = commJob.OPCItem.MonitoredItemViewModel.GetSubscriptionViewModelParent().GetSessionViewModelParent();
                        if (session == null || !session.Connected)
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs() { Job = commJob };
                            eJob.ErrorCode = DriverErrorCodes.ErrorTimeOut;
                            OnJobExecuted(commJob, eJob);
                            SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                        }
                    }
                }, sessionName, TimeSpan.FromMilliseconds(delay), TimeSpan.FromMilliseconds(-1));
                dictConnectionTimeoutTimer.Add(sessionName, timer);
            }
        }

        internal void RemoveJobInitialized(OpcClientDriverCommJob job, string sessionName)
        {
            lock (lockListObject)
            {
                if (dictSessionToJobInitialized.ContainsKey(sessionName))
                    dictSessionToJobInitialized[sessionName].Remove(job);
            }
        }

        public override List<Tag> SortTags(IList<Tag> tags)
        {
            return tags.ToList();
        }

        /// <summary>
        /// From queued jobs to write return the jobs that can be write together to the device 
        /// </summary>
        /// <param name="jobList"></param>
        /// <param name="exList"></param>
        private void GetJobsListToWrite(List<CommJob> jobList, List<CommJob> exList)
        {
            int jobIndex = 0;                
            bool first = true;
            string endpointUrl = string.Empty;
            long maxNodesPerRead = 0;

            while (jobIndex < jobList.Count)
            {
                OpcClientDriverCommJob j = jobList.ElementAt(jobIndex) as OpcClientDriverCommJob;
                if (j != null)
                {
                    if (first)
                    {                    
                        endpointUrl = j.GetOPCItemEndpointUrl();

                        // get the nr of tags (jobs) that can be write together from the device
                        maxNodesPerRead = FetchServerWriteCapabilities(endpointUrl, j.OPCItem);
                        
                        first = false;
                    }

                    // group the jobs with same EndpointUrl --> to the same device
                    if (!String.IsNullOrEmpty(j.GetOPCItemEndpointUrl()) && j.GetOPCItemEndpointUrl() == endpointUrl)
                    {
                        if (j.GetTagListToWriteCount() > 0)
                        {
                            // == 0 no limits
                            if (maxNodesPerRead != 0 && exList.Count > maxNodesPerRead)
                                break;

                            exList.Add(j);
                            jobList.RemoveAt(jobIndex);
                            j.UpdateTagsListOnWriting();
                        }
                        else
                        {
                            // ExtensionObject case: each tag is managed by adding a new job: when the 1st added, the following lose the data write
                            jobList.RemoveAt(jobIndex);
                        }

                        jobIndex--;
                    }                
                }
                jobIndex++;
            }           
        }

        List<CommJob> queuedJobsToWrite = new List<CommJob>();
        Thread queueTagToWriteThread;
        ManualResetEvent queueTagToWriteEvent;
        bool exitQueueTagToWrite = false;
        private object lockQueueTagToWriteThread = new object();
        public uint QueueTagToWrite(NodeId tagnodeid)
        {            
            if (exitQueueTagToWrite)
                return StatusCodes.BadNodeIdInvalid;

            CommJob j = null; 
            lock (lockListObject)
            {
                if (mapTagJob.ContainsKey(tagnodeid))
                    j = mapTagJob[tagnodeid];
            }

            if (j == null)
                return StatusCodes.BadNodeIdInvalid;

            lock (lockQueueTagToWriteThread)
            {
                if (queueTagToWriteThread == null)
                {                    
                    queueTagToWriteEvent = new ManualResetEvent(false);
                    queueTagToWriteThread = new Thread((o) =>
                    {
                        while (!exitQueueTagToWrite)
                        {
                            if (!exitQueueTagToWrite)
                            {
                                queueTagToWriteEvent.WaitOne();
                            }
                            List<CommJob> wList = new List<CommJob>();
                            lock (lockQueueTagToWriteThread)                            
                                GetJobsListToWrite(queuedJobsToWrite, wList);                                
                            if (wList.Count > 0)
                            {
                                //System.Diagnostics.Debug.WriteLine("OpcClientWrite WriteJob = {0}", list.Count);
                                List<uint> writeResults = new List<uint>();
                                WriteJobs(wList, writeResults);

                                for (int i = 0; i < wList.Count; i++)
                                {
                                    CommJob job = wList[i];
                                    ExecutedJobArgs e = new ExecutedJobArgs();
                                    e.Job = job;
                                    if (writeResults[i] == StatusCodes.Good)
                                        e.ErrorCode = DriverErrorCodes.ErrorNoError;
                                    else
                                        e.ErrorCode = (DriverErrorCodes)writeResults[i];
                                    OnJobExecuted(job, e);
                                }
                            }

                            if (queuedJobsToWrite.Count == 0)
                            {
                                lock (lockQueueTagToWriteThread)
                                    queueTagToWriteEvent.Reset();
                            }
                        }
                    });
                    queueTagToWriteThread.Start();
                }
                
                queuedJobsToWrite.Add(j);
                //System.Diagnostics.Debug.WriteLine("pendingJobsToWrite.Count = {0}", queuedJobsToWrite.Count);
                queueTagToWriteEvent.Set();
            }

            return StatusCodes.Good;
        }

        /// <summary>
        /// Stops the thread used to spool the values ​​to be written
        /// </summary>
        void ExitPendingWriteJobsThreads()
        {
            exitQueueTagToWrite = true;

            Thread tr = queueTagToWriteThread;

            lock (lockQueueTagToWriteThread)
            {
                if (queueTagToWriteEvent != null)
                    queueTagToWriteEvent.Set();
            }

            if (tr != null)
                tr.Join();
            
            queueTagToWriteThread = null;

            if (queueTagToWriteEvent != null)
            {
                queueTagToWriteEvent.Dispose();
                queueTagToWriteEvent = null;
            }

            queuedJobsToWrite.Clear();            
        }

        /// <summary>
        /// Writes tags values to OPC Server
        /// </summary>
        /// <param name="jobsList"></param>
        /// <param name="jobsWriteResults"></param>
        private void WriteJobs(List<CommJob> jobsList, List<uint> jobsWriteResults)
        {
            List<CommJob> jobs = new List<CommJob>();
            List<object> values = new List<object>();
            List<MonitoredItemViewModel> monitoredItemViewModels = new List<MonitoredItemViewModel>();
            List<uint> writeResults = new List<uint>();
                        
            List<CommJob> badJobs = new List<CommJob>();
            List<uint> badJobswriteResults = new List<uint>();

            try
            {
                foreach (OpcClientDriverCommJob j in jobsList)
                {
                    if (j.OPCItem == null)
                    {
                        badJobs.Add(j);
                        //badJobswriteResults.Add(StatusCodes.BadNodeIdUnknown);
                        badJobswriteResults.Add((uint)OpcClientErrorCodes.OpcClientErrorBadNodeIdOrInvalidState);
                    }
                    else if (j.OPCItem.MonitoredItemViewModel == null)
                    {
                        badJobs.Add(j);
                        //badJobswriteResults.Add(StatusCodes.BadInvalidState);
                        badJobswriteResults.Add((uint)OpcClientErrorCodes.OpcClientErrorBadNodeIdOrInvalidState);
                    }
                    else
                    {
                        if (j.IsExtensionObject)
                        {
                            List<NodeId> tagnodeids = new List<NodeId>();
                            List<object> tagnodevalues = new List<object>();
                            foreach (Tag tag in j.TagsListOnWriting)
                            {
                                tagnodeids.Add(tag.TagNode.NodeId);
                                tagnodevalues.Add(((OpcClientDriverTag)tag).GetOpcWriteVal());
                            }
                            try
                            {
                                DataValue dataValue = j.ReadAndMergeExtensionObjectWithWriteValues(ref tagnodeids, ref tagnodevalues);
                                jobs.Add(j);
                                values.Add(Utils.Clone(dataValue));
                            }
                            catch (ServiceResultException ex)
                            {
                                //GetCommDriver().OnSystemEvent(null, string.Format(Properties.Resources.ErrorOnWriting, ((OpcClientDriverDynTagSettings)(j.TagsList[0]).DynSettings).RelativePath, ex.Message), EventSeverity.High);
                                //badJobswriteResults.Add(ex.StatusCode);
                                badJobs.Add(j);
                                badJobswriteResults.Add((uint)OpcClientErrorCodes.OpcClientErrorWritingUnexpectedError);
                            }
                            catch (Exception ex)
                            {
                                //GetCommDriver().OnSystemEvent(null, string.Format(Properties.Resources.ErrorOnWriting, ((OpcClientDriverDynTagSettings)(j.TagsList[0]).DynSettings).RelativePath, ex.Message), EventSeverity.High);
                                badJobs.Add(j);
                                badJobswriteResults.Add((uint)OpcClientErrorCodes.OpcClientErrorWritingUnexpectedError);
                            }
                        }
                        else
                        {
                            jobs.Add(j);
                            values.Add(((OpcClientDriverTag)j.TagsList[0]).GetOpcWriteVal());
                        }

                        //writeResults.Add(StatusCodes.Bad);
                        writeResults.Add((uint)OpcClientErrorCodes.OpcClientErrorWritingUnexpectedError);
                        monitoredItemViewModels.Add(j.OPCItem.MonitoredItemViewModel);
                    }
                    j.ClearTagListOnWriting();
                }
            }
            catch (Exception ex)
            {
                jobs.Clear();
                badJobs.Clear();
                badJobswriteResults.Clear();
                for (int i = 0; i < jobsList.Count; i++)
                {
                    //GetCommDriver().OnSystemEvent(null, string.Format(Properties.Resources.ErrorOnWriting, ((OpcClientDriverDynTagSettings)(jobsList[i].TagsList[0]).DynSettings).RelativePath, ex.Message), EventSeverity.High);
                    badJobs.Add(jobsList[i]);
                    badJobswriteResults.Add((uint)OpcClientErrorCodes.OpcClientErrorWritingUnexpectedError);
                }
            }

            if (values.Count > 0)
            {
                try
                {
                    WriteValues(values, monitoredItemViewModels, writeResults);
                }
                catch (ServiceResultException ex)
                {
                    for (int i = 0; i < jobs.Count; i++)
                    {
                        //GetCommDriver().OnSystemEvent(null, string.Format(Properties.Resources.ErrorOnWriting, ((OpcClientDriverDynTagSettings)(jobs[i].TagsList[0]).DynSettings).RelativePath, ex.Message), EventSeverity.High);
                        //writeResults[i] = ex.StatusCode;
                        writeResults[i] = (uint)OpcClientErrorCodes.OpcClientErrorWritingUnexpectedError;
                    }
                }
                catch (Exception ex)
                {
                    for (int i = 0; i < jobs.Count; i++)
                    {
                        //GetCommDriver().OnSystemEvent(null, string.Format(Properties.Resources.ErrorOnWriting, ((OpcClientDriverDynTagSettings)(jobs[i].TagsList[0]).DynSettings).RelativePath, ex.Message), EventSeverity.High);
                        //writeResults[i] = StatusCodes.BadUnexpectedError;
                        writeResults[i] = (uint)OpcClientErrorCodes.OpcClientErrorWritingUnexpectedError;
                    }
                }
            }

            jobsList.Clear();
            for (int i=0; i < badJobs.Count; i++)
            {
                jobsList.Add(badJobs[i]);
                jobsWriteResults.Add(badJobswriteResults[i]);
            }
            for (int i = 0; i < jobs.Count; i++)
            {
                jobsList.Add(jobs[i]);
                jobsWriteResults.Add(writeResults[i]);
            }
        }

        /// <summary>
        /// Get the nr of tags (jobs) that can be write together from the device : if fails, return 1
        /// </summary>
        /// <param name="endPoint"></param>
        /// <param name="monitor"></param>
        /// <returns></returns>
        long FetchServerWriteCapabilities(string endPoint, OPCUAEntityReference opcItem)
        {
            long writeCapabilities = 1;

            if (maxNodesPerWrite.ContainsKey(endPoint))
            {
                writeCapabilities = maxNodesPerWrite[endPoint];
            }
            else
            {
                try
                {
                    Opc.Ua.Client.Session session = null;
                    var subscription = opcItem.MonitoredItemViewModel.GetSubscriptionViewModelParent();
                    if (subscription != null)
                        session = subscription.GetSessionViewModelParent()?.Session;
                    if (session != null)
                    {
                        //throw new NullReferenceException("session cannot be null while writing a new value.");

                        var serverSpecialNodes = new NodeIdCollection();
                        serverSpecialNodes.Add(Variables.Server_ServerCapabilities_OperationLimits_MaxNodesPerWrite);

                        var expectedTypes = new List<Type>();
                        expectedTypes.Add(typeof(UInt32));

                        List<object> values;
                        List<ServiceResult> errors;

                        //sessionViewModel.Session.ReadValues(serverSpecialNodes, expectedTypes, out values, out errors);
                        session.ReadValues(serverSpecialNodes, expectedTypes, out values, out errors);

                        bool noError = true;
                        for (int ii = 0; ii < errors.Count; ii++)
                            noError &= ServiceResult.IsGood(errors[ii].StatusCode);

                        if (noError)
                        {
                            maxNodesPerWrite[endPoint] = (UInt32)values[0];

                            writeCapabilities = (UInt32)values[0];
                        }
                    }
                }
                catch (Exception ex)
                { }                
            }

            return writeCapabilities;
        }

        object GetNewValue(object v, object currentValue, int indexBit, BuiltInType builtinType, out bool bit)
        {
            switch (builtinType)
            {
                case BuiltInType.Byte:
                    {
                        if (indexBit > 7)
                            throw new IndexOutOfRangeException(String.Format(OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        byte lValue = System.Convert.ToByte(v);
                        byte shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToByte(currentValue);
                            if (bit)
                                lValue |= (byte)((byte)1 << indexBit);
                            else
                                lValue &= (byte)(~((byte)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.SByte:
                    {
                        if (indexBit > 7)
                            throw new IndexOutOfRangeException(String.Format(OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        sbyte lValue = System.Convert.ToSByte(v);
                        sbyte shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToSByte(currentValue);
                            if (bit)
                                lValue |= (sbyte)((sbyte)1 << indexBit);
                            else
                                lValue &= (sbyte)(~((sbyte)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Int16:
                    {
                        if (indexBit > 15)
                            throw new IndexOutOfRangeException(String.Format(OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        short lValue = System.Convert.ToInt16(v);
                        short shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToInt16(currentValue);
                            if (bit)
                                lValue |= (short)((short)1 << indexBit);
                            else
                                lValue &= (short)(~((short)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.UInt16:
                    {
                        if (indexBit > 15)
                            throw new IndexOutOfRangeException(String.Format(OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        ushort lValue = System.Convert.ToUInt16(v);
                        ushort shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToUInt16(currentValue);
                            if (bit)
                                lValue |= (ushort)((ushort)1 << indexBit);
                            else
                                lValue &= (ushort)(~((ushort)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Int32:
                    {
                        if (indexBit > 31)
                            throw new IndexOutOfRangeException(String.Format(OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        int lValue = System.Convert.ToInt32(v);
                        int shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToInt32(currentValue);
                            if (bit)
                                lValue |= (int)((int)1 << indexBit);
                            else
                                lValue &= (int)(~((int)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.UInt32:
                    {
                        if (indexBit > 31)
                            throw new IndexOutOfRangeException(String.Format(OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        uint lValue = System.Convert.ToUInt32(v);
                        uint shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToUInt32(currentValue);
                            if (bit)
                                lValue |= (uint)((uint)1 << indexBit);
                            else
                                lValue &= (uint)(~((uint)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Int64:
                case BuiltInType.Integer:
                    {
                        if (indexBit > 63)
                            throw new IndexOutOfRangeException(String.Format(OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        long lValue = System.Convert.ToInt64(v);
                        long shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToInt64(currentValue);
                            if (bit)
                                lValue |= (long)((long)1 << indexBit);
                            else
                                lValue &= (long)(~((long)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.UInt64:
                case BuiltInType.UInteger:
                    {
                        if (indexBit > 63)
                            throw new IndexOutOfRangeException(String.Format(OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        ulong lValue = System.Convert.ToUInt64(v);
                        ulong shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = System.Convert.ToUInt64(currentValue);
                            if (bit)
                                lValue |= (ulong)((ulong)1 << indexBit);
                            else
                                lValue &= (ulong)(~((ulong)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Float:
                    {
                        if (indexBit > 31)
                            throw new IndexOutOfRangeException(String.Format(OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        long lValue = (long)System.Convert.ToSingle(v);
                        long shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = (long)System.Convert.ToSingle(currentValue);
                            if (bit)
                                lValue |= (long)((long)1 << indexBit);
                            else
                                lValue &= (long)(~((long)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                case BuiltInType.Double:
                case BuiltInType.Number:
                    {
                        if (indexBit > 63)
                            throw new IndexOutOfRangeException(String.Format(OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteInvalidIndexBit, indexBit));

                        long lValue = (long)System.Convert.ToDouble(v);
                        long shift = 1;
                        bit = (lValue & (shift << indexBit)) != 0;
                        if (currentValue != null)
                        {
                            lValue = (long)System.Convert.ToDouble(currentValue);
                            if (bit)
                                lValue |= (long)((long)1 << indexBit);
                            else
                                lValue &= (long)(~((long)1 << indexBit));
                            return Opc.Ua.TypeInfo.Cast(lValue, builtinType);
                        }
                        else
                            return Opc.Ua.TypeInfo.Cast(v, builtinType);
                    }
                default:
                    {
                        bit = false;
                        return v;
                    }
            }
        }

        public bool WriteValues(List<Object> v, List<OPCUAViewModel.MonitoredItemViewModel> m, List<uint> r)
        {
            return WriteValues(v, -1, -1, m, r);
        }

        public bool WriteValues(List<Object> v, int indexArray, int indexBit, List<OPCUAViewModel.MonitoredItemViewModel> m, List<uint> r)
        {
            try
            {
                return WriteValues(v, indexArray, indexBit, useIndexRange: true, m, r);
            }
            catch (ServiceResultException ex)
            {
                if ((indexArray >= 0 || indexBit >= 0) && ex.StatusCode == StatusCodes.BadWriteNotSupported)
                {
                    // opc ua server doesn't support write via IndexRange.
                    return WriteValues(v, -1, indexBit, useIndexRange: false, m, r);
                }
                else if (indexBit >= 0 && (ex.StatusCode == StatusCodes.BadTypeMismatch ||
                    ex.StatusCode == StatusCodes.BadIndexRangeInvalid ||
                    ex.StatusCode == StatusCodes.BadIndexRangeNoData))
                {
                    // opc ua server doesn't support write via IndexRange for bits.
                    return WriteValues(v, indexArray, indexBit, useIndexRange: false, m, r);
                }
                throw;
            }
        }

        object lockObject = new object();
        //private DataValue _dataValue = new DataValue(StatusCodes.Uncertain);
        bool WriteValues(List<Object> values, int indexArray, int indexBit, bool useIndexRange, List<OPCUAViewModel.MonitoredItemViewModel> mivms, List<uint> writeResults)
        {
            WriteValueCollection valuesToWrite = new WriteValueCollection();

            for (int valueIndex = 0; valueIndex < values.Count; valueIndex++)
            {
                var v = values[valueIndex];
                var mivm = mivms[valueIndex];
                WriteValue value = null;
                lock (lockObject)
                {
                    BuiltInType builtinType;
                    if (mivm.monitoredItem == null)
                    {
                        //if (bIsReadOnly)
                        //    throw new Exception("This is a temporary reaonly item, please wait for the real connected item to write");

                        if (mivm.DataValue == null)
                            //Opc.Ua.DataValue DataValue = new Opc.Ua.DataValue(new Variant(v), StatusCodes.Good);
                            mivm.DataValue = new Opc.Ua.DataValue(new Variant(v), StatusCodes.Good);
                        else if (v != null && mivm.DataValue.Value != null)
                        {
                            try
                            {
                                uint arraydimension = 0;
                                Type type;
                                if (mivm.DataValue.Value is Array)
                                {
                                    arraydimension = (uint)(mivm.DataValue.Value as Array).GetUpperBound(0) + 1;
                                    type = (mivm.DataValue.Value as Array).GetValue(0).GetType();
                                }
                                else
                                    type = mivm.DataValue.Value.GetType();

                                builtinType = MonitoredItemViewModel.GetBuiltInType(type.Name);
                                if (builtinType == BuiltInType.Double || builtinType == BuiltInType.Float)
                                {
                                    if (v is string)
                                    {
                                        CultureInfo culture = new CultureInfo(System.Globalization.CultureInfo.CurrentCulture.Name, true);
                                        v = (v as string).Replace(culture.NumberFormat.NumberDecimalSeparator, ".");
                                    }
                                }
                                try
                                {
                                    try
                                    {
                                        v = ChangeTypeHelper.ChangeType(v, builtinType, arraydimension); // prechange type base on currenthread localization first
                                    }
                                    catch (Exception)
                                    {
                                        if (v is String && builtinType != BuiltInType.String &&
                                            (builtinType == BuiltInType.Int64 || builtinType == BuiltInType.UInt64))
                                        {
                                            System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo();
                                            info.NumberDecimalSeparator = ".";
                                            info.NumberGroupSeparator = ",";
                                            v = Convert.ToInt64(v, info);
                                        }

                                        string binary = null;
                                        switch (builtinType)
                                        {
                                            case BuiltInType.Byte:
                                                binary = Convert.ToString((long)v, 2);
                                                binary = binary.Substring(Math.Max(0, binary.Length - 8));
                                                v = Convert.ToByte(binary, 2);
                                                break;
                                            case BuiltInType.SByte:
                                                binary = Convert.ToString((long)v, 2);
                                                binary = binary.Substring(Math.Max(0, binary.Length - 8));
                                                v = Convert.ToSByte(binary, 2);
                                                break;
                                            case BuiltInType.Int16:
                                                binary = Convert.ToString((long)v, 2);
                                                binary = binary.Substring(Math.Max(0, binary.Length - 16));
                                                v = Convert.ToInt16(binary, 2);
                                                break;
                                            case BuiltInType.UInt16:
                                                binary = Convert.ToString((long)v, 2);
                                                binary = binary.Substring(Math.Max(0, binary.Length - 16));
                                                v = Convert.ToUInt16(binary, 2);
                                                break;
                                            case BuiltInType.Int32:
                                                binary = Convert.ToString((long)v, 2);
                                                binary = binary.Substring(Math.Max(0, binary.Length - 32));
                                                v = Convert.ToInt32(binary, 2);
                                                break;
                                            case BuiltInType.UInt32:
                                                binary = Convert.ToString((long)v, 2);
                                                binary = binary.Substring(Math.Max(0, binary.Length - 32));
                                                v = Convert.ToUInt32(binary, 2);
                                                break;
                                            case BuiltInType.Int64:
                                            case BuiltInType.Integer:
                                                binary = Convert.ToString((Int64)v, 2);
                                                // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                                                v = Convert.ToInt64(binary, 2);
                                                break;
                                            case BuiltInType.UInt64:
                                            case BuiltInType.UInteger:
                                                binary = Convert.ToString((Int64)v, 2);
                                                // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                                                v = Convert.ToUInt64(binary, 2);
                                                break;
                                        }
                                    }
                                    if (arraydimension == 0)
                                    {
                                        if (indexBit >= 0 && Opc.Ua.TypeInfo.IsNumericType(builtinType))
                                        {
                                            bool bit;
                                            v = GetNewValue(v, mivm.DataValue.Value, indexBit, builtinType, out bit);
                                            mivm.DataValue = new DataValue(new Variant(v), StatusCodes.Good, DateTime.UtcNow);
                                        }
                                        else
                                            mivm.DataValue = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(v, builtinType)), StatusCodes.Good, DateTime.UtcNow);
                                    }
                                    else if (v is Array)
                                    {
                                        var array = v as Array;
                                        if (indexArray >= 0 && mivm.DataValue.Value is Array)
                                        {
                                            var dValue = mivm.DataValue.Value as Array;
                                            var vValue = array.GetValue(indexArray);
                                            if (indexBit >= 0 && Opc.Ua.TypeInfo.IsNumericType(builtinType))
                                            {
                                                bool bit;
                                                vValue = GetNewValue(vValue, dValue.GetValue(indexArray), indexBit, builtinType, out bit);
                                            }
                                            dValue.SetValue(vValue, indexArray);
                                            mivm.DataValue = new DataValue(new Variant(Opc.Ua.TypeInfo.CastArray(dValue, builtinType, builtinType, ChangeTypeHelper.CastArrayElement)), StatusCodes.Good, DateTime.UtcNow);
                                        }
                                        else
                                            mivm.DataValue = new DataValue(new Variant(Opc.Ua.TypeInfo.CastArray(array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement)), StatusCodes.Good, DateTime.UtcNow);
                                    }
                                }
                                catch (Exception ex)
                                {
                                    for (int i = 0; i < mivms.Count; i++)
                                        mivm.LastMessage = String.Format("{0} - {1}, Value : {2}, Error : {3}", mivm.Title, OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteError, values[i], ex.Message);
                                    Utils.Trace(ex, OPCUAViewModel.Properties.Resource.MonitoredTemporaryItemWriteError);
                                    throw;
                                }
                            }
                            catch (Exception)
                            {
                                //throw new InvalidCastException(String.Format("Cannot cast the value '{0}' to type {1}", v, _dataValue.Value.GetType()));
                                throw;
                            }
                        }

                        return true;
                    }

                    //Node node = monitoredItem.Subscription.Session.NodeCache.Find(monitoredItem.ResolvedNodeId) as Node;

                    value = new WriteValue
                    {
                        NodeId = mivm.monitoredItem.ResolvedNodeId,
                        AttributeId = Attributes.Value,
                        IndexRange = null
                    };

                    // read the display name for non-variables.
                    if ((mivm.monitoredItem.NodeClass & (NodeClass.Variable | NodeClass.VariableType)) == 0)
                    {
                        value.AttributeId = Attributes.DisplayName;
                    }

                    NodeId datatypeId = Attributes.GetDataTypeId(mivm.monitoredItem.AttributeId);
                    int valueRank = Attributes.GetValueRank(mivm.monitoredItem.AttributeId);
                    uint arraySizeOneDimension = 0;

                    Opc.Ua.Client.Session session = null;
                    var subscription = mivm.GetSubscriptionViewModelParent();
                    if (subscription != null)
                        session = subscription.GetSessionViewModelParent()?.Session;
                    if (session == null)
                        throw new NullReferenceException("session cannot be null while writing a new value.");

                    if (mivm.monitoredItem.AttributeId == Attributes.Value)
                    {
                        if (session != null)
                        {
                            var vnode = session.NodeCache.Find(value.NodeId) as VariableNode;
                            if (vnode != null)
                            {
                                datatypeId = vnode.DataType;
                                valueRank = vnode.ValueRank;
                                arraySizeOneDimension = (vnode.ArrayDimensions != null && vnode.ArrayDimensions.Count > 0) ? vnode.ArrayDimensions[0] : 0;
                            }
                        }
                    }

                    builtinType = Opc.Ua.TypeInfo.GetBuiltInType(datatypeId, session.TypeTree);
                    object newValue = null;

                    try
                    {
                        v = ChangeTypeHelper.ChangeType(v, builtinType, arraySizeOneDimension); // prechange type base on currenthread localization first
                        if (builtinType == BuiltInType.ExtensionObject)
                        {
                            value.Value = (Opc.Ua.DataValue)v;
                        }
                        else if (arraySizeOneDimension == 0)
                        {
                            if (indexBit >= 0 && Opc.Ua.TypeInfo.IsNumericType(builtinType))
                            {
                                if (useIndexRange)
                                {
                                    bool bit;
                                    newValue = GetNewValue(v, mivm.DataValue?.Value, indexBit, builtinType, out bit);
                                    value.IndexRange = Convert.ToString(indexBit, CultureInfo.InvariantCulture);
                                    value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(bit, builtinType)));
                                }
                                else
                                {
                                    bool bit;
                                    var datavalue = session.ReadValue(mivm.monitoredItem.ResolvedNodeId);
                                    v = GetNewValue(v, datavalue?.Value, indexBit, builtinType, out bit);
                                    value.Value = new DataValue(new Variant(v));
                                }
                            }
                            else
                                value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(v, builtinType)));
                        }
                        else if (v is Array)
                        {
                            var array = v as Array;
                            if (indexArray >= 0)
                            {
                                var vValue = array.GetValue(indexArray);
                                string subrange = null;
                                if (indexBit >= 0 && Opc.Ua.TypeInfo.IsNumericType(builtinType))
                                {
                                    if (useIndexRange)
                                    {
                                        bool bit;
                                        object currentValue = null;
                                        if (mivm.DataValue != null && mivm.DataValue.Value is Array)
                                            currentValue = (mivm.DataValue.Value as Array).GetValue(indexArray);
                                        vValue = GetNewValue(vValue, currentValue, indexBit, builtinType, out bit);
                                        subrange = Convert.ToString(indexBit, CultureInfo.InvariantCulture);
                                        array.SetValue(vValue, indexArray);
                                        value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(bit, builtinType)));
                                        if (mivm.DataValue != null && mivm.DataValue.Value is Array)
                                        {
                                            newValue = Opc.Ua.TypeInfo.CastArray(mivm.DataValue.Value as Array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                                            (newValue as Array).SetValue(vValue, indexArray);
                                        }
                                    }
                                    else
                                    {
                                        bool bit;
                                        object currentValue = null;
                                        var datavalue = session.ReadValue(mivm.monitoredItem.ResolvedNodeId);
                                        if (datavalue != null && datavalue.Value is Array)
                                            currentValue = (datavalue.Value as Array).GetValue(indexArray);
                                        vValue = GetNewValue(vValue, currentValue, indexBit, builtinType, out bit);
                                        array.SetValue(vValue, indexArray);
                                        var element = Opc.Ua.TypeInfo.CreateArray(builtinType, 1);
                                        element.SetValue(vValue, 0);
                                        value.Value = new DataValue(new Variant(element));
                                        if (datavalue != null && datavalue.Value is Array)
                                        {
                                            newValue = Opc.Ua.TypeInfo.CastArray(datavalue.Value as Array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                                            (newValue as Array).SetValue(vValue, indexArray);
                                        }
                                    }
                                }
                                else
                                {
                                    var element = Opc.Ua.TypeInfo.CreateArray(builtinType, 1);
                                    element.SetValue(vValue, 0);
                                    value.Value = new DataValue(new Variant(element));
                                    if (mivm.DataValue != null && mivm.DataValue.Value is Array)
                                    {
                                        newValue = Opc.Ua.TypeInfo.CastArray(mivm.DataValue.Value as Array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                                        (newValue as Array).SetValue(vValue, indexArray);
                                    }
                                }
                                value.IndexRange = Convert.ToString(indexArray, CultureInfo.InvariantCulture);
                                if (subrange != null)
                                    value.IndexRange = String.Format("{0},{1}", value.IndexRange, subrange);
                                if (newValue == null)
                                    newValue = Opc.Ua.TypeInfo.CastArray(array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                            }
                            else
                                value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.CastArray(array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement)));
                        }
                    }
                    catch (Exception exception)
                    {
                        try
                        {
                            if (v is String && builtinType != BuiltInType.String &&
                                (builtinType == BuiltInType.Int64 || builtinType == BuiltInType.UInt64))
                            {
                                System.Globalization.NumberFormatInfo info = new System.Globalization.NumberFormatInfo();
                                info.NumberDecimalSeparator = ".";
                                info.NumberGroupSeparator = ",";
                                v = Convert.ToInt64(v, info);
                            }

                            string binary = null;
                            switch (builtinType)
                            {
                                case BuiltInType.Byte:
                                    binary = Convert.ToString((long)v, 2);
                                    binary = binary.Substring(Math.Max(0, binary.Length - 8));
                                    v = Convert.ToByte(binary, 2);
                                    break;
                                case BuiltInType.SByte:
                                    binary = Convert.ToString((long)v, 2);
                                    binary = binary.Substring(Math.Max(0, binary.Length - 8));
                                    v = Convert.ToSByte(binary, 2);
                                    break;
                                case BuiltInType.Int16:
                                    binary = Convert.ToString((long)v, 2);
                                    binary = binary.Substring(Math.Max(0, binary.Length - 16));
                                    v = Convert.ToInt16(binary, 2);
                                    break;
                                case BuiltInType.UInt16:
                                    binary = Convert.ToString((long)v, 2);
                                    binary = binary.Substring(Math.Max(0, binary.Length - 16));
                                    v = Convert.ToUInt16(binary, 2);
                                    break;
                                case BuiltInType.Int32:
                                    binary = Convert.ToString((long)v, 2);
                                    binary = binary.Substring(Math.Max(0, binary.Length - 32));
                                    v = Convert.ToInt32(binary, 2);
                                    break;
                                case BuiltInType.UInt32:
                                    binary = Convert.ToString((long)v, 2);
                                    binary = binary.Substring(Math.Max(0, binary.Length - 32));
                                    v = Convert.ToUInt32(binary, 2);
                                    break;
                                case BuiltInType.Int64:
                                case BuiltInType.Integer:
                                    binary = Convert.ToString((Int64)v, 2);
                                    // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                                    v = Convert.ToInt64(binary, 2);
                                    break;
                                case BuiltInType.UInt64:
                                case BuiltInType.UInteger:
                                    binary = Convert.ToString((Int64)v, 2);
                                    // binary = binary.Substring(Math.Max(0, binary.Length - 63));
                                    v = Convert.ToUInt64(binary, 2);
                                    break;
                            }
                            if (arraySizeOneDimension == 0)
                            {
                                if (indexBit >= 0 && Opc.Ua.TypeInfo.IsNumericType(builtinType))
                                {
                                    if (useIndexRange)
                                    {
                                        bool bit;
                                        newValue = GetNewValue(v, mivm.DataValue?.Value, indexBit, builtinType, out bit);
                                        value.IndexRange = Convert.ToString(indexBit, CultureInfo.InvariantCulture);
                                        value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(bit, builtinType)));
                                    }
                                    else
                                    {
                                        bool bit;
                                        var datavalue = session.ReadValue(mivm.monitoredItem.ResolvedNodeId);
                                        v = GetNewValue(v, datavalue?.Value, indexBit, builtinType, out bit);
                                        value.Value = new DataValue(new Variant(v));
                                    }
                                }
                                else
                                    value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(v, builtinType)));
                            }
                            else if (v is Array)
                            {
                                var array = v as Array;
                                if (indexArray >= 0)
                                {
                                    var vValue = array.GetValue(indexArray);
                                    string subrange = null;
                                    if (indexBit >= 0 && Opc.Ua.TypeInfo.IsNumericType(builtinType))
                                    {
                                        if (useIndexRange)
                                        {
                                            bool bit;
                                            object currentValue = null;
                                            if (mivm.DataValue != null && mivm.DataValue.Value is Array)
                                                currentValue = (mivm.DataValue.Value as Array).GetValue(indexArray);
                                            vValue = GetNewValue(vValue, currentValue, indexBit, builtinType, out bit);
                                            subrange = Convert.ToString(indexBit, CultureInfo.InvariantCulture);
                                            array.SetValue(vValue, indexArray);
                                            value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.Cast(bit, builtinType)));
                                            if (mivm.DataValue != null && mivm.DataValue.Value is Array)
                                            {
                                                newValue = Opc.Ua.TypeInfo.CastArray(mivm.DataValue.Value as Array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                                                (newValue as Array).SetValue(vValue, indexArray);
                                            }
                                        }
                                        else
                                        {
                                            bool bit;
                                            object currentValue = null;
                                            var datavalue = session.ReadValue(mivm.monitoredItem.ResolvedNodeId);
                                            if (datavalue != null && datavalue.Value is Array)
                                                currentValue = (datavalue.Value as Array).GetValue(indexArray);
                                            vValue = GetNewValue(vValue, currentValue, indexBit, builtinType, out bit);
                                            array.SetValue(vValue, indexArray);
                                            var element = Opc.Ua.TypeInfo.CreateArray(builtinType, 1);
                                            element.SetValue(vValue, 0);
                                            value.Value = new DataValue(new Variant(element));
                                            if (datavalue != null && datavalue.Value is Array)
                                            {
                                                newValue = Opc.Ua.TypeInfo.CastArray(datavalue.Value as Array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                                                (newValue as Array).SetValue(vValue, indexArray);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        var element = Opc.Ua.TypeInfo.CreateArray(builtinType, 1);
                                        element.SetValue(vValue, 0);
                                        value.Value = new DataValue(new Variant(element));
                                        if (mivm.DataValue != null && mivm.DataValue.Value is Array)
                                        {
                                            newValue = Opc.Ua.TypeInfo.CastArray(mivm.DataValue.Value as Array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                                            (newValue as Array).SetValue(vValue, indexArray);
                                        }
                                    }
                                    value.IndexRange = Convert.ToString(indexArray, CultureInfo.InvariantCulture);
                                    if (subrange != null)
                                        value.IndexRange = String.Format("{0},{1}", value.IndexRange, subrange);
                                    if (newValue == null)
                                        newValue = Opc.Ua.TypeInfo.CastArray(array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement);
                                }
                                else
                                    value.Value = new DataValue(new Variant(Opc.Ua.TypeInfo.CastArray(array, builtinType, builtinType, ChangeTypeHelper.CastArrayElement)));
                            }
                            else
                                throw;
                        }
                        catch (Exception ex)
                        {
                            //LastMessage = String.Format("{0} - {1}, Value : {2}, Error : {3}", Title, Properties.Resource.OPCUAMonitoredItemWriteError, Value, ex.Message);
                            //Utils.Trace(ex, Properties.Resource.OPCUAMonitoredItemWriteError);
                            throw;
                        }
                    }

                    value.Value.StatusCode = StatusCodes.Good;
                    value.Value.ServerTimestamp = DateTime.MinValue;
                    value.Value.SourceTimestamp = DateTime.MinValue;

                    valuesToWrite.Add(value);
                }
            }

            {
                Opc.Ua.Client.Session session = null;
                var subscription = mivms[0].GetSubscriptionViewModelParent();
                if (subscription != null)
                    session = subscription.GetSessionViewModelParent()?.Session;
                if (session == null)
                    throw new NullReferenceException("session cannot be null while writing a new value.");

                StatusCodeCollection results = null;
                DiagnosticInfoCollection diagnosticInfos = null;

                try
                {
                    ResponseHeader responseHeader = session.Write(
                        null,
                        valuesToWrite,
                        out results,
                        out diagnosticInfos);

                    ClientBase.ValidateResponse(results, values);
                    ClientBase.ValidateDiagnosticInfos(diagnosticInfos, values);

                    //foreach (StatusCode sc in results)
                    for (int i = 0; i < values.Count; i++)
                    {
                        if (StatusCode.IsGood(results[i]))
                        {
                            writeResults[i] = StatusCodes.Good;
                            valuesToWrite[i].Value.ServerTimestamp = mivms[i].DataValue.ServerTimestamp;
                            valuesToWrite[i].Value.SourcePicoseconds = mivms[i].DataValue.SourcePicoseconds;
                            valuesToWrite[i].Value.SourceTimestamp = mivms[i].DataValue.SourceTimestamp;
                            valuesToWrite[i].Value.SourcePicoseconds = mivms[i].DataValue.SourcePicoseconds;

                            //// LastMessage = String.Format("{0} - {1}", Title, Properties.Resource.OPCUAMonitoredItemWriteSuccessful);
                            //// AddOutputText(String.Format("Write Successfully to {0}", mi.DisplayName), false);
                            if (mivms[i].DataValue != null && StatusCode.IsGood(mivms[i].DataValue.StatusCode))
                                mivms[i].LastMessage = String.Empty;
                        }
                        else
                        {
                            writeResults[i] = (uint)OpcClientErrorCodes.OpcClientErrorWritingUnexpectedError; //StatusCodes.Bad;
                            mivms[i].LastMessage = String.Format("{0} - {1}, Reason : {2}", mivms[i].Title, OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteFailure, results[i]);
                            //throw new ServiceResultException(new ServiceResult(sc, null, responseHeader.StringTable));
                        }
                    }
                }
                catch (Exception ex)
                {
                    for (int i = 0; i < mivms.Count; i++)
                    {
                        mivms[i].LastMessage = String.Format("{0} - {1}, Value : {2}, Error : {3}", mivms[i].Title, OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteError, values[i], ex.Message);
                        Utils.Trace(ex, OPCUAViewModel.Properties.Resource.OPCUAMonitoredItemWriteError);
                        writeResults[i] = (uint)OpcClientErrorCodes.OpcClientErrorWritingUnexpectedError; //StatusCodes.Bad;
                    }
                    throw;
                }
            }

            return true;
        }

        #endregion

        #region Properties

        private int _RemoveDisabledItemAfterSecs;
        public int RemoveDisabledItemAfterSecs
        {
            get
            {
                return _RemoveDisabledItemAfterSecs;
            }
            set
            {
                _RemoveDisabledItemAfterSecs =  value;
            }
        }

        private int _MaxCleanCount;
        public int MaxCleanCount
        {
            get
            {
                return _MaxCleanCount;
            }
            set
            {
                _MaxCleanCount = value;
            }
        }

        private bool _UseAlwaysSecureConnections;
        public bool UseAlwaysSecureConnections
        {
            get
            {
                return _UseAlwaysSecureConnections;
            }
            set
            {
                _UseAlwaysSecureConnections = value;
            }
        }

        private int _FastSamplingInterval;
        public int FastSamplingInterval
        {
            get
            {
                return _FastSamplingInterval;
            }
            set
            {
                _FastSamplingInterval = value;
            }
        }

        private int _SlowSamplingInterval;
        public int SlowSamplingInterval
        {
            get
            {
                return _SlowSamplingInterval;
            }
            set
            {
                _SlowSamplingInterval = value;
            }
        }

        private bool _DisableWhenNotUsed;
        public bool DisableWhenNotUsed
        {
            get
            {
                return _DisableWhenNotUsed;
            }
            set
            {
                _DisableWhenNotUsed = value;
            }
        }

        private bool _UsePollingRead;
        public bool UsePollingRead
        {
            get
            {
                return _UsePollingRead;
            }
            set
            {
                _UsePollingRead = value;
            }
        }

        private int _PublishingInterval;
        public int PublishingInterval
        {
            get
            {
                return _PublishingInterval;
            }
            set
            {
                _PublishingInterval = value;
            }
        }

        private int _ConnectionTimeout;
        public int ConnectionTimeout
        {
            get
            {
                return _ConnectionTimeout;
            }
            set
            {
                _ConnectionTimeout  = value;
            }
        }

        private uint _StationID;
        public uint StationID
        {
            get { return _StationID; }
            set
            {
                _StationID = value;
            }
        }

        private bool _SettingsApplyed;
        internal bool SettingsApplyed {
            get { return _SettingsApplyed; }
            set { _SettingsApplyed = value; }
        }

        private string _User;
        public string User
        {
            get
            {
                return _User;
            }
            set
            {
                _User = value;
            }
        }

        private string _Password;
        //[ValueConverter(typeof(UFUserModel.EncryptedValueConverter))]
        [Size(200)]
        public string Password
        {
            get
            {
                return _Password;
            }
            set
            {
                _Password = value;
            }
        }

        private bool _UseServerDiscovery;

        public bool UseServerDiscovery
        {
            get
            {
                return _UseServerDiscovery;
            }
            set
            {
                _UseServerDiscovery = value;
            }
        }

        private bool _UseLocalTimestamp;

        public bool UseLocalTimestamp
        {
            get
            {
                return _UseLocalTimestamp;
            }
            set
            {
                _UseLocalTimestamp = value;
            }
        }



        #endregion

        #region  Statistics
        /// <summary>   The statistic nodes. </summary>
        public static readonly StatisticSetting.NodeDataNames[] OpcClientStatisticNodes = new StatisticSetting.NodeDataNames[]
        {
            StatisticSetting.NodeDataNames.TotalJobsError,
            StatisticSetting.NodeDataNames.LastErrorTime,
            StatisticSetting.NodeDataNames.LastError,
            StatisticSetting.NodeDataNames.InErrorState,
        };
        #endregion
    }
}

