using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverBaseInterfaces;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using OPCUAViewModel;
using System.Threading;
using DriverCodeBase.Helpers;
using System.Threading.Tasks;

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
            lock (e.Job.retLockList())
            {
                if (e.Job.RWState == CommJob.RWStates.ReadForRW &&
                    e.ErrorCode == DriverErrorCodes.ErrorNoError)
                {
                    e.Job.RWState = CommJob.RWStates.WriteForRW;
                    ChannelBase.ChangeStateJob(e.Job, CommJobState.PollingNow);
                }
                else
                    e.Job.RWState = CommJob.RWStates.Standard;
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


        public override bool ParseDynamicTags(IList<Tag> tags)
        {
            //start aggregation thread/procedure for this station.
            lock (lockListObject)
            {
                if (tags == null || tags.Count == 0)
                    return false;

                if (mapGroupedJobs == null)
                    mapGroupedJobs = new Dictionary<string, List<CommJob>>();

                List<Tag> tList = SortTags(tags);

                tags.Clear();

                List<Tag> innerList = new List<Tag>();
                List<Tag> lMethods = new List<Tag>();

                foreach (var tag in tList)
                {
                    innerList.Clear();
                    lMethods.Clear();
                    if (tag.TagNode.DataType.IdType == IdType.Guid && CommDriver.IsPrototypeSplitEnabled())
                    {
                        //struttura
                        List<TagDefinition> pList = new List<TagDefinition>();
                        GetPrototypeTagList(tag.TagNode.NodeId, ref pList);
                        string dynsettings = string.Empty;
                        OpcClientDriverDynTagSettings dtCalc = tag.DynSettings as OpcClientDriverDynTagSettings;
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
                                    CommunicationDriver.log.Error(string.Format(Properties.Resources.InvalidTagDefinition, CommDriver.DriverName, t.TagNode.NodeId.Identifier, t.InvalidReason));
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
                    foreach (var candTag in innerList)
                    {
                        CommJob candJob = CreateJob(candTag);
                        List<CommJob> tmpList;

                        if (!candJob.IsValid || mapTagJob.ContainsKey(candTag.TagNode.NodeId))
                            continue;

                        tags.Add(candTag);

                        if (mapGroupedJobs.ContainsKey(candJob.GroupString))
                            tmpList = mapGroupedJobs[candJob.GroupString];
                        else
                            tmpList = new List<CommJob>();

                        if (tmpList.Count == 0)
                        {
                            tmpList.Add(candJob);
                            mapTagJob.Add(candTag.TagNode.NodeId, candJob);
                            if (!ListWholeJob.Contains(candJob))
                                ListWholeJob.Add(candJob);

                            if (lMethods.Count > 0)
                            {
                                if (!mapJobMethods.Keys.Contains(candJob))
                                    mapJobMethods[candJob] = new List<NodeId>();
                                foreach (var m in lMethods)
                                {
                                    if (!mapMethodJobs.Keys.Contains(m.TagNode.NodeId))
                                        mapMethodJobs[m.TagNode.NodeId] = new List<CommJob>();
                                    if (!mapMethodJobs[m.TagNode.NodeId].Contains(candJob))
                                        mapMethodJobs[m.TagNode.NodeId].Add(candJob);

                                    if (!mapJobMethods[candJob].Contains(m.TagNode.NodeId))
                                        mapJobMethods[candJob].Add(m.TagNode.NodeId);

                                    if (!candJob.TagsList.Contains(m))
                                        candJob.TagsList.Add(m);
                                    AddToMapTagJob(m.TagNode.NodeId, candJob);
                                    GetCommDriver().AddToTagToStationMap(m.TagNode.NodeId, this);
                                }
                            }
                        }
                        else
                        {
                            bool bAggregated = false;
                            foreach (var testJob in tmpList)
                            {
                                uint newIndex = 0;

                                var aggType = testJob.TestAggregateJob(candJob, out newIndex);

                                bool bMethod = mapJobMethods.Keys.Contains(testJob) && mapJobMethods.Keys.Contains(candJob);
                                if (bMethod)
                                {
                                    bMethod = mapJobMethods[testJob].Count == mapJobMethods[candJob].Count;
                                    if (bMethod)
                                    {
                                        foreach (var p in mapJobMethods[testJob])
                                        {
                                            if (!mapJobMethods[testJob].Contains(p))
                                            {
                                                aggType = JobAggregationType.JobAggregImpossible;
                                                break;
                                            }
                                        }
                                    }
                                }
                                if (aggType != JobAggregationType.JobAggregImpossible)
                                {
                                    //Aggregate
                                    testJob.AggregateJob(candJob, aggType, newIndex);

                                    foreach (var a in candJob.TagsList)
                                        mapTagJob[a.TagNode.NodeId] = testJob;

                                    if (lMethods.Count > 0)
                                    {
                                        if (!mapJobMethods.Keys.Contains(testJob))
                                            mapJobMethods[testJob] = new List<NodeId>();
                                        foreach (var m in lMethods)
                                        {
                                            if (!mapMethodJobs.Keys.Contains(m.TagNode.NodeId))
                                                mapMethodJobs[m.TagNode.NodeId] = new List<CommJob>();
                                            if (!mapMethodJobs[m.TagNode.NodeId].Contains(testJob))
                                                mapMethodJobs[m.TagNode.NodeId].Add(testJob);

                                            if (!mapJobMethods[testJob].Contains(m.TagNode.NodeId))
                                                mapJobMethods[testJob].Add(m.TagNode.NodeId);

                                            if (!testJob.TagsList.Contains(m))
                                                testJob.TagsList.Add(m);
                                            AddToMapTagJob(m.TagNode.NodeId, testJob);
                                            GetCommDriver().AddToTagToStationMap(m.TagNode.NodeId, this);
                                        }
                                    }


                                    bAggregated = true;
                                    break;
                                }
                            }
                            if (!bAggregated)
                            {
                                //add candJob to the list
                                tmpList.Add(candJob);
                                mapTagJob.Add(candTag.TagNode.NodeId, candJob);
                                if (!ListWholeJob.Contains(candJob))
                                    ListWholeJob.Add(candJob);

                                if (lMethods.Count > 0)
                                {
                                    if (!mapJobMethods.Keys.Contains(candJob))
                                        mapJobMethods[candJob] = new List<NodeId>();
                                    foreach (var m in lMethods)
                                    {
                                        if (!mapMethodJobs.Keys.Contains(m.TagNode.NodeId))
                                            mapMethodJobs[m.TagNode.NodeId] = new List<CommJob>();
                                        if (!mapMethodJobs[m.TagNode.NodeId].Contains(candJob))
                                            mapMethodJobs[m.TagNode.NodeId].Add(candJob);

                                        if (!mapJobMethods[candJob].Contains(m.TagNode.NodeId))
                                            mapJobMethods[candJob].Add(m.TagNode.NodeId);

                                        if (!candJob.TagsList.Contains(m))
                                            candJob.TagsList.Add(m);
                                        AddToMapTagJob(m.TagNode.NodeId, candJob);
                                        GetCommDriver().AddToTagToStationMap(m.TagNode.NodeId, this);
                                    }
                                }
                            }
                        }
                        UpdateTagVaue(candTag.TagNode.NodeId, new DataValue()
                        {
                            Value = candTag.TagNode.InitialValue,
                            ServerTimestamp = DateTime.UtcNow,
                            SourceTimestamp = DateTime.MinValue,
                            StatusCode = candTag.TagNode.InitialValue != null ? StatusCodes.Good : StatusCodes.BadWaitingForInitialData
                        });

                        mapGroupedJobs[candJob.GroupString] = tmpList;
                    }
                }
            }
            return true;
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

            if ((e.ErrorCode != DriverErrorCodes.ErrorNoError))
            {
                SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
            }
            else
            {
                if (Channel.InErrorJobs(this) == 0)
                {
                    SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
                }
                e.Job.ResetConditionalVariable();
            }

            var value = e.Values as Opc.Ua.DataValue;
            mJ.TagsList[0].Value.Value = Utils.Clone(value.Value);
            mJ.TagsList[0].Value.StatusCode = value.StatusCode;
            mJ.TagsList[0].Value.SourceTimestamp = value.SourceTimestamp;
            mJ.TagsList[0].Value.ServerTimestamp = value.ServerTimestamp;

            GetCommDriver().OnTagChanged(mJ.TagsList[0].TagNode.NodeId, mJ.TagsList[0].Value, mJ.TagsList[0].Value.SourceTimestamp);
        }
        int stationSampling = -1;
        public override bool SetInUse(NodeId tagnodeid, bool bInUse, double samplinginterval = -1)
        {
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
                            return false;
                    }
                    else
                        return false;
                }

                job = mapTagJob[n] as OpcClientDriverCommJob;
                if (job == null)
                    return false;
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
            else
            {
                job.SetUncertainLastUsableValueQuality();
            }

            bool ret = job.SetInUse(tagnodeid, bInUse, samplinginterval);

            if (ChannelBase != null && job.InUse != inuse)
            {
                if (job.InUse)
                    ChannelBase.ChangeStateJob(job, CommJobState.PollingInUse);
                else
                    ChannelBase.ChangeStateJob(job, CommJobState.PollingNotInUse);
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
                
            return(ret);
        }

        public void SetSessionSettings(string sessionname)
        {
            if (!SettingsApplyed)
            {
                int slowSI = (SlowSamplingInterval == FastSamplingInterval ? SlowSamplingInterval + 1 : SlowSamplingInterval);
                RealTimeConnectionManagerViewModel.AddSessionSettings(sessionname,
                new SessionSettings()
                {
                    DisableWhenNotUsed = DisableWhenNotUsed,
                    FastSamplingInterval = FastSamplingInterval,
                    MaxCleanCount = MaxCleanCount,
                    PublishingInterval = PublishingInterval,
                    RemoveDisabledItemAfterSecs = RemoveDisabledItemAfterSecs,
                    SlowSamplingInterval = slowSI,
                    UseAlwaysSecureConnections = UseAlwaysSecureConnections       ,
                    MapAppNameSettings = map
                });
                if (!string.IsNullOrEmpty(User) && !string.IsNullOrEmpty(Password))
                    RealTimeConnectionManagerViewModel.SetUserIdentity(sessionname, new UserIdentity(User, Password), new StringCollection(), true);
                SettingsApplyed = true;
            }
        }
        public override bool Startup()
        {
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

        public override bool Suspend()
        {
            return SuspendStation();
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
        public override bool SuspendStation()
        {
            var interlokCounter = Interlocked.Increment(ref suspendCounter);
            if (interlokCounter != 1)
                return true;

            if (ChannelBase == null)
            {
                //Init failed!!!!
                CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.SuspendStationNotInitialized, CommDriver.DriverName, Name), EventSeverity.High);
                return false;
            }
            
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob = (from j in ListWholeJob.AsParallel() where j.InUse == true select j).ToList();
            }

            foreach (var job in listJob)
            {
                (job as OpcClientDriverCommJob).SetUncertainLastUsableValueQuality();
                (job as OpcClientDriverCommJob).Unsubscribe(Name);
            }
            SuspendChannel();
            return true;
        }

        public override bool Terminate()
        {
            RealTimeConnectionManagerViewModel.CleanSpecificDeadConnection(Name);
            return true;
        }

        public override void Dispose()
        {
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
