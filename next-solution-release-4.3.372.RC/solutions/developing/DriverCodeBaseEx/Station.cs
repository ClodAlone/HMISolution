////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Station.cs
//
// summary:	Implements the station class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using System.Collections;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;
using System.Threading.Tasks;

namespace DriverCodeBaseEx
{

    /// <summary>   communication target device. </summary>
    public abstract class Station : IStatistics, IDisposable
    {

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the station object. </summary>
        ///
        /// <param name="commdriver">                       The communications driver. </param>
        /// <param name="settings" type="StationSettings">  Options for controlling the operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Station(CommunicationDriver commdriver, StationSettings settings)
            : this()
        {
            CommDriver = commdriver;
            _Name = settings.Name;
            _MaxRetriesBeforeError = settings.MaxRetriesBeforeError;
            _FirstTime = true;
            _StateCommandTag = settings.StateCommandTag;
            _StateTag = settings.StateTag;
            _CommandTag = settings.CommandTag;
            _RewritingOfTheSameValue = settings.RewritingOfTheSameValue;
            _DisableQualityUpdate = settings.DisableQualityUpdate;
            if (StateCommandVariable.IsTagsSet(_StateCommandTag, _StateTag, _CommandTag))
            {
                StateCommandVariableDuplicated = settings.StateCommandVariableAlreadyUsed();
                stationStateCommandVariable = new StateCommandVariable(StateCommandVariable.ObjectTypes.Station, _StateCommandTag, _StateTag, _CommandTag);
            }
        }

        /// <summary>   Specialized default constructor for use only by derived class. </summary>
        protected Station()
        {
            _InErrorState = false;
            CurRetriesBeforeError = 0;
            ReturnFromGeneralError = false;
            _FirstTime = true;
            stationStateCommandVariable = new StateCommandVariable();
            _RewritingOfTheSameValue = false;
            _DisableQualityUpdate = false;
        }

        #endregion

        #region Data Members

        /// <summary>   The communications driver. </summary>
        protected readonly CommunicationDriver CommDriver;

        /// <summary>   The channel. </summary>
        protected Channel Channel;
        /// <summary>   The channel base. </summary>
        protected IChannelBase ChannelBase;

        protected List<CommJob> listErrorJobs = new List<CommJob>();
        /// <summary>   The lock list object. </summary>
        protected Object lockListObject = new Object();
        /// <summary>   The list whole job. </summary>
        protected internal List<CommJob> ListWholeJob = new List<CommJob>();
        /// <summary> The list of jobs managed in maps to speed up access </summary>
        protected internal Dictionary<CommJob, bool> MapWholeJob = new Dictionary<CommJob, bool>();
        /// <summary>   The map tag job. </summary>
        protected Dictionary<NodeId, CommJob> mapTagJob = new Dictionary<NodeId, CommJob>();
        /// <summary>   The map grouped jobs. </summary>
        protected Dictionary<string, List<CommJob>> mapGroupedJobs = new Dictionary<string, List<CommJob>>();

        /// <summary>   The map method jobs. </summary>
        protected Dictionary<NodeId, List<CommJob>> mapMethodJobs = new Dictionary<NodeId, List<CommJob>>();
        /// <summary>   The map job methods. </summary>
        protected Dictionary<CommJob, List<NodeId>> mapJobMethods = new Dictionary<CommJob, List<NodeId>>();

        /// <summary>   The current retries before error. </summary>
        protected uint CurRetriesBeforeError;

        /// <summary>   Set when station is in GeneralError state, Reset when 1st "good" job was excecuted. </summary>
        private bool _ReturnFromGeneralError;
        protected bool ReturnFromGeneralError
        {
            set { 
                _ReturnFromGeneralError = value;
                if (Channel != null)
                    Channel.CheckInGeneralErrorState();
            }
            get { return _ReturnFromGeneralError; }
        }

        /// <summary>   The total partial jobs. </summary>
        protected long TotalPartialJobs = 0;
        /// <summary>   The total partial tags. </summary>
        protected long TotalPartialTags = 0;
        /// <summary>   The total partial in bytes. </summary>
        protected long TotalPartialBytes = 0;

        /// <summary>  The State/Command variable associated to the station. </summary>
        protected StateCommandVariable stationStateCommandVariable;        
        protected readonly Dictionary<NodeId, List<CommJob>> ObservedTagToJobsMap = new Dictionary<NodeId, List<CommJob>>();
        bool StateCommandVariableDuplicated = false;

        #endregion

        #region Internal Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates the tag vaue. </summary>
        ///
        /// <param name="tagnodeid" type="NodeId">  The tagnodeid. </param>
        /// <param name="value" type="DataValue">   The value. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected uint UpdateTagVaue(NodeId tagnodeid, DataValue value)
        {
            CommJob job;
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(tagnodeid))
                    return StatusCodes.BadNodeIdInvalid;

                job = mapTagJob[tagnodeid];
            }

            return job.UpdateTagVaue(tagnodeid, value);
        }

        #endregion

        #region Abstract Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a job. </summary>
        ///
        /// <param name="JobSettings" type="CommJobSettings">   The job settings. </param>
        ///
        /// <returns>   The new job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract CommJob CreateJob(CommJobSettings JobSettings);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a job. </summary>
        ///
        /// <param name="defTag" type="Tag">    The definition tag. </param>
        ///
        /// <returns>   The new job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract CommJob CreateJob(Tag defTag);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates job settings. </summary>
        ///
        /// <param name="session" type="Session">   The session. </param>
        /// <param name="job" type="CommJob">       The job. </param>
        ///
        /// <returns>   The new job settings. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract CommJobSettings CreateJobSettings(Session session, CommJob job);
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Creates a tag. </summary>
        ///
        /// <param name="td" type="DriverBaseInterfaces.TagDefinition"> The td. </param>
        ///
        /// <returns>   The new tag. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public abstract Tag CreateTag(DriverBaseInterfaces.TagDefinition td);
        #endregion

        #region Virtual Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the write tag action. </summary>
        ///
        /// <param name="tagnodeid" type="NodeId">  The tagnodeid. </param>
        /// <param name="value" type="ref object">  [in,out] The value. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint OnWriteTag(NodeId tagnodeid, ref object value, ref StatusCode statusCode, ref DateTime timestamp, bool ignoreWriteAsync = false, bool forceSynchWrite = false)
        {
            CommJob job;
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(tagnodeid))
                    return StatusCodes.BadNodeIdInvalid;

                job = mapTagJob[tagnodeid];

                if (job.Type == LinkType.Input)
                    return (StatusCodes.BadNotWritable);
            }

            uint ret = StatusCodes.BadConfigurationError;
            if (ChannelBase != null)
            {
                if (((!ignoreWriteAsync && !CommDriver.WriteAsync) || forceSynchWrite) && job.IsConditionalVariableOn())
                {
                    if (job.DuplicatedTagValueRemoved(tagnodeid, ref value))
                        return StatusCodes.Good;

                    List<object> outputvalues = new List<object>();
                    outputvalues.Add(new uint());
#if DEBUG
                    System.Diagnostics.Trace.TraceInformation("{0} OnWriteTag !CommDriver.WriteAsync", Thread.CurrentThread.ManagedThreadId);
#endif
                    ret = ExecuteSyncroJob(job, false, 0, outputvalues, false, tagnodeid, value); //synchro...
                    if (ret == StatusCodes.Good)
                        ret = (Convert.ToInt32(outputvalues[0]) != 0 ? StatusCodes.Bad : StatusCodes.Good);
                    int errorcode = (int)outputvalues[0];
                    string error;
                    uint quality;
                    GetCommDriver().GetDriverErrorInfo(errorcode, out quality, out error);
                    statusCode = (StatusCode)quality;
                    timestamp = DateTime.UtcNow;
                    job.ClearTagListWrite();
                }
                else
                {
                    ret = job.OnWriteTag(tagnodeid, ref value);
                    if (ret == StatusCodes.Good)
                    {
                        // for job in PollingInError state, wait "normal" scheduling (to avoid station state var unstable value)
                        if (!Channel.IsJobInPollingInErrorState(job))
                            ChannelBase.ChangeStateJob(job, CommJobState.PollingNow);
                    }
                }
            }
            return ret;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the list of node ids to be observed for this station. </summary>
        ///
        /// <returns>   Gets the list of node ids to be observed for this station. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual List<NodeId> GetObservingNodes()
        {
            List<NodeId> nodeIDs = new List<NodeId>();
            // Add to the list the Node ID of the State\Command Variable of the station
            if (stationStateCommandVariable.hasBeenSet == true)
            {
                nodeIDs.AddRange(stationStateCommandVariable.GetNodesId());
            }

            // Add to the list the Node IDs of the Conditional Variables of the jobs
            lock (lockListObject)
            {
                foreach (var job in ListWholeJob)
                {
                    List<NodeId> jobNodeIDs = job.GetObservingNodes();
                    if (jobNodeIDs.Count > 0)
                    {
                        nodeIDs.AddRange(jobNodeIDs);
                        foreach (var node in jobNodeIDs)
                        {
                            if (!ObservedTagToJobsMap.Keys.Contains(node))
                            {
                                ObservedTagToJobsMap[node] = new List<CommJob>();
                            }
                            ObservedTagToJobsMap[node].Add(job);
                        }
                    }
                }
            }

            return nodeIDs;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Updates the value of an observed node id. </summary>
        ///
        /// <returns>   Updates the value of an observed node id. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void UpdateObservedTag(NodeId node, DataValue value)
        {
            // Update the value of the state/command variable
            if ((stationStateCommandVariable.hasBeenSet == true) && stationStateCommandVariable.ContainsNodeId(node))
            {
                // if is not yet started or is a redundancy server not started set only value
                if (!CommDriver.IsRunning())
                    SetStateCommandVariableValue(node, value);
                else
                    ManageUpdatedValueForTheStateCommandVariable(node, value);
            }

            // Update the value of the conditional variable of a job.
            else
            {
                // Get the station that manages the node ID and pass to it the updated value of the variable
                if (ObservedTagToJobsMap.Keys.Contains(node) == true)
                {
                    List<CommJob> jobList = ObservedTagToJobsMap[node];
                    foreach (var job in jobList)
                    {
                        if (job.ConditionalVariableSet && node.ToString() == job.ConditionalVariableId)
                        {
                            // if is not yet started or is a redundancy server not started set only value
                            if (!CommDriver.IsRunning())
                                job.SetConditionalVariableValue(node, value);
                            else
                                job.ManageUpdatedValueForTheConditionalVariable(node, value);
                        }
                        else if (job.OffsetVariableSet && node.ToString() == job.OffsetVariableId)
                        {
                            job.ManageUpdatedValueForTheOffsetVariable(node, value);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Set (only) the value of state command variable
        /// </summary>
        /// <param name="value"></param>
        public void SetStateCommandVariableValue(NodeId node, DataValue value)
        {
            stationStateCommandVariable.SetValueDataType(node, value.WrappedValue.TypeInfo.BuiltInType);
            stationStateCommandVariable.SetValue(node, value);
        }

        public virtual void ManageUpdatedValueForTheStateCommandVariable(NodeId node, DataValue value)
        {
            bool suspendBitSavedValue = false;
            bool suspendBitCanBeManaged = false;
            if (GetStateCommandVariableBit(ref suspendBitSavedValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
            {
                suspendBitCanBeManaged = true;
            }

            SetStateCommandVariableValue(node, value);            
            
            if (suspendBitCanBeManaged == true)
            {
                bool suspendBitNewValue = false;
                if (GetStateCommandVariableBit(ref suspendBitNewValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
                {
                    if (suspendBitSavedValue != suspendBitNewValue)
                    {
                        if (suspendBitNewValue == true)
                        {
                            bool returnValue = Suspend();
                        }
                        else
                        {
                            bool returnValue = Startup();
                        }
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the update tag action without writing anything on device. </summary>
        ///
        /// <param name="tagnodeid" type="NodeId">  The tagnodeid. </param>
        /// <param name="value" type="ref object">  [in,out] The value. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint OnUpdateTag(NodeId tagnodeid, ref object value, ref StatusCode statusCode,
            ref DateTime timestamp)
        {
            CommJob job;
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(tagnodeid))
                    return StatusCodes.BadNodeIdInvalid;

                job = mapTagJob[tagnodeid];
            }

            if (job.Type == LinkType.Input)
                return (StatusCodes.BadNotWritable);

            var datavalue = new DataValue()
            {
                Value = value,
                ServerTimestamp = timestamp,// DateTime.UtcNow,
                SourceTimestamp = DateTime.MinValue,
                StatusCode = statusCode//StatusCodes.Good
            };

            return job.UpdateTagVaue(tagnodeid, datavalue, true);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Process the write tag described by stateInfo. </summary>
        ///
        /// <param name="stateInfo" type="object">  Information describing the state. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void ProcessWriteTag(object stateInfo)
        {
            object[] args = (object[])stateInfo;
            NodeId tagnodeid = (NodeId)args[0];
            object value = (object)args[1];
            CommJob job;
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(tagnodeid))
                    return;

                job = mapTagJob[tagnodeid];
            }

            if (ChannelBase != null)
                ChannelBase.ChangeStateJob(job, CommJobState.PollingNow);
            job.OnWriteTag(tagnodeid, ref value);
        }

        public enum InUseStates
        {
            None,
            InvalidTagNodeId,
            UnchangedState,
            InUse,
            NotInUse,
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets in use. </summary>
        ///
        /// <param name="tagnodeid" type="NodeId">          The tagnodeid. </param>
        /// <param name="bInUse" type="bool">               true to in use. </param>
        /// <param name="samplinginterval" type="double">   (Optional) the samplinginterval. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual InUseStates SetInUse(NodeId tagnodeid, bool bInUse, double samplinginterval = -1)
        {
            CommJob job = null;
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
                        {
                            s = s.Substring(0, p);
                            s = s.Replace(";s=", ";g=");
                        }

                        n = new NodeId(s);
                        if (!mapTagJob.ContainsKey(n))
                            return InUseStates.InvalidTagNodeId;
                    }
                    else
                        return InUseStates.InvalidTagNodeId;
                }

                job = mapTagJob[n];
            }
            bool ret = false;
            if (job != null)
            {
                var inuse = job.InUse;
                uint sampint = job.SamplingInterval;
                
                ret = job.SetInUse(tagnodeid, bInUse, samplinginterval);

                if (ChannelBase != null && (job.InUse != inuse || job.SamplingInterval != sampint))
                {
                    if (job.InUse)
                    {
                        ChannelBase.ChangeStateJob(job, CommJobState.PollingInUse);
                        return InUseStates.InUse;
                    }
                    else
                    {
                        ChannelBase.ChangeStateJob(job, CommJobState.PollingNotInUse);
                        return InUseStates.NotInUse;
                    }
                }
                else
                {
                    return InUseStates.UnchangedState;
                }
            }

            return InUseStates.InvalidTagNodeId;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initialises this object. </summary>
        ///
        /// <param name="channel" type="Channel">   The channel. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Init(Channel channel)
        {
            Channel = channel;

            if (Channel != null)
                ChannelBase = Channel as IChannelBase;

            if (StateCommandVariableDuplicated)
            {
                stationStateCommandVariable.hasBeenSet = false;
                CommDriver.OnSystemEvent(null, string.Format(Properties.Resources.StateCommandTagNameAlreadyInUse, _StateCommandTag.StringRepresentation), EventSeverity.Medium);
            }
            return ChannelBase != null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Prepares this object for use. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Startup()
        {
            System.Diagnostics.Debug.WriteLine("{0} -- DEBUG -- Station.Startup station name: {1}", DateTime.UtcNow.ToString("HH:MM:ss.fff"), Name);
            bool suspendBitNewValue = false;
            if (GetStateCommandVariableBit(ref suspendBitNewValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
            {
                if (suspendBitNewValue == true)
                {
                    return (true);
                }
            }

            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }

            foreach (var job in listJob)
            {
                if (job.Type == LinkType.ExceptionOutput || job.Type == LinkType.UnconditionalOutput)
                    job.SetQuality(StatusCodes.Uncertain);
                else
                {
                    job.SetInternalQuality(StatusCodes.BadWaitingForInitialData);
                }
                job.IsPending = false;
            }

            if (ChannelBase != null)
            {
                ChannelBase.JobExecuted += OnJobExecuted;
                foreach (var job in listJob)
                {
                    List<Tag> tl = (from t in job.TagsList.AsParallel()
                                    where (t.DynSettings.MethodID != -1)
                                    select t).ToList();
                    if (tl.Count != job.TagsList.Count)
                        ChannelBase.SubscribeJob(job, (job.InUse ? CommJobState.PollingInUse : CommJobState.PollingNotInUse));
                }
                
                Channel.Startup();                
                Channel.SetRequestSchedule();
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Suspends this object. </summary>
        ///               
        /// <param name="setJobQuality"></param>
        // <returns>   true if it succeeds, false if it fails. </returns>        
        public virtual bool Suspend(bool setJobQuality = true)
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }

            if (ChannelBase != null)
            {
                ChannelBase.RemoveStationJobs(this);
                ChannelBase.JobExecuted -= OnJobExecuted;

                if (Channel.NrActiveStations() == 0)
                    Channel.Suspend();

                foreach (var job in listJob)
                {
                    job.ScheduleQueue = CommJobState.UnScheduled;
                    job.IsPending = false;
                    if (setJobQuality)
                        job.SetUncertainLastUsableValueQuality();
                }
            }

            return true;
        }
              
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Terminates this object. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Terminate()
        {
            if (ChannelBase != null)
            {
                ChannelBase.RemoveStationJobs(this);
                ChannelBase.JobExecuted -= OnJobExecuted;
            }

            return true;
        }

        /// <summary>
        /// If is a TimeOut error, wait until curRetriesBefore> = MaxRetriesBeforeError
        /// </summary>
        /// <param name="curRetriesBeforeError"></param>
        /// <param name="error"></param>
        protected virtual bool IsConsecutiveTimeOut(uint curRetriesBeforeError, DriverErrorCodes error)
        {
            if (error != DriverErrorCodes.ErrorTimeOut)
                return true;

            return (++curRetriesBeforeError >= MaxRetriesBeforeError);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Compare by dynamic. </summary>
        ///
        /// <param name="x" type="Tag"> The Tag to process. </param>
        /// <param name="y" type="Tag"> The Tag to process. </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static int CompareByDynamic(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    return x.TagNode.DynamicSettings.CompareTo(y.TagNode.DynamicSettings);
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sort tags. </summary>
        ///
        /// <param name="tags" type="IList<Tag>">   The tags. </param>
        ///
        /// <returns>   The sorted tags. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareByDynamic);
            return listTag;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets prototype tag list. </summary>
        ///
        /// <param name="node" type="NodeId">                   The node. </param>
        /// <param name="list" type="ref List<TagDefinition>">  [in,out] The list. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetPrototypeTagList(NodeId node, ref List<TagDefinition> list)
        {
            if (list == null)
                return false;
            List<TagDefinition> tList = new List<TagDefinition>();
            GetCommDriver().OnTagPrototypeQuery(node, ref tList);
            foreach (var tag in tList)
            {
                if (tag.DataType.IdType == IdType.Guid || (tag.DataType.IdType == IdType.Numeric && (uint)tag.DataType.Identifier == ObjectTypes.FolderType))
                {
                    List<TagDefinition> inList = new List<TagDefinition>();
                    if (GetPrototypeTagList(tag.NodeId, ref inList))
                        list.AddRange(inList);
                }
                else
                {
                    list.Add(tag);
                }
            }
            return true;
        }


        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Substitute the names of the elements of a template with the device names. </summary>
        /////
        ///// <param name="elementList" type="ref List<TagDefinition>">  [in,out] The list. </param>
        /////
        ///// <returns>   void. </returns>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public virtual void SubstituteStructElementNamesWithOriginalNames(List<TagDefinition> elementList)
        //{
        //    Parallel.ForEach(elementList, tag =>
        //    {
        //        if (!String.IsNullOrWhiteSpace(tag.OriginalName))
        //        {
        //            tag.Name = tag.OriginalName;
        //        }
        //    });
        //}

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Parse dynamic tags. </summary>
        ///
        /// <param name="tags" type="IList<Tag>">   The tags. </param>
        /// <param name="invalidTags" type="IList<Tag>">   The list of invalid tags. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool ParseDynamicTags(IList<Tag> tags)
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
                    tag.ByteOffset = 0;
                    tag.BitOffset = 0;
                    innerList.Clear();
                    lMethods.Clear();

                    // if tag is a struct split its members
                    ParseDynamicTagsStructSplit(tag, innerList, lMethods);

                    // try to aggregate result tags with of others jobs to speed-up communication
                    ParseDynamicTagsAggregate(tag, innerList, lMethods, tags);
                }
            }
            return true;
        }

        public virtual void SplitStructMembers(Tag tag, List<Tag> innerList, List<Tag> lMethods)
        {
            //struttura
            List<TagDefinition> pList = new List<TagDefinition>();
            GetPrototypeTagList(tag.TagNode.NodeId, ref pList);
            string dynsettings = string.Empty;
            DynTagSettings dtCalc = tag.DynSettings;
            TagDefinition olddtag;
            for (int i = 0; i < pList.Count; i++)
            {
                TagDefinition dtag = pList[i];
                olddtag = dtag;
                if (dtag.DynamicSettings.Length == 0)
                {
                    if (dynsettings.Length == 0)
                    {
                        dynsettings = dtCalc.GetFirstDynSetting(tag, dtag);
                    }
                    else
                    {
                        dynsettings = dtCalc.GetNextDynSetting(pList[i - 1], dtag);
                    }
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
                        CommDriver.OnTagChanged(t.TagNode.NodeId, new DataValue(StatusCodes.BadConfigurationError), DisableQualityUpdate);
                        CommunicationDriver.OnLogEvent(string.Format(Properties.Resources.InvalidTagDefinition, CommDriver.DriverName, t.TagNode.NodeId.Identifier, t.InvalidReason), System.Diagnostics.EventLogEntryType.Error);
                    }
                }
            }
        }

        /// <summary>
        /// If tag is struct, try to split into its memebers, othewhise return tag
        /// </summary>
        /// <param name="tag">tag to split</param>
        /// <param name="innerList">resulted tags from split</param>
        /// <param name="lMethods">resulted methods from split</param>
        public virtual void ParseDynamicTagsStructSplit(Tag tag, List<Tag> innerList, List<Tag> lMethods)
        {
            if (tag.TagNode.DataType.IdType == IdType.Guid && CommDriver.IsPrototypeSplitEnabled())
            {
                SplitStructMembers(tag, innerList, lMethods);
            }
            else
            {
                if (tag.DynSettings.MethodID > -1)
                    lMethods.Add(tag);
                innerList.Add(tag);
            }
        }

        /// <summary>
        /// Verify is a tag is an Atomic Struct
        /// </summary>
        /// <param name="defTag"></param>
        /// <returns></returns>
        public virtual bool IsStructAtomic(Tag defTag)
        {            
            return false;
        }

        /// <summary>
        /// If job is aggregable, try to aggregate to tags of existing jobs to speed-up communication
        /// </summary>
        /// <param name="tag">source tag</param>
        /// <param name="innerList">resulted tags from split</param>
        /// <param name="lMethods">resulted methods from split</param>
        /// <param name="tags">list of all tags</param>
        public virtual void ParseDynamicTagsAggregate(Tag tag, List<Tag> innerList, List<Tag> lMethods, IList<Tag> tags)
        {   
            // check if tag is a struct and can be Atomic Struct
            if (IsStructAtomic(tag))
            { 
                innerList.Clear();
                innerList.Add(tag);
            }

            foreach (var candTag in innerList)
            {
                CommJob candJob = CreateJob(candTag);
                                
                // if job creation failed, tag's DynamicSettings is not valid --> use job info to populate tag error
                if (!candJob.IsValid)
                {
                    candTag.bIsValid = false;
                    candTag.InvalidReason = candJob.InvalidReason;
                }

                if (!candJob.IsValid || mapTagJob.ContainsKey(candTag.TagNode.NodeId))
                    continue;

                tags.Add(candTag);
                if (candJob.IsStruct())
                {
                    foreach (var t in candJob.TagsList)
                        tags.Add(t);
                }
                
                bool bAggregated = false;
                if (candJob.IsJobAggregable())
                {
                    if (!mapGroupedJobs.ContainsKey(candJob.GroupString))
                        mapGroupedJobs[candJob.GroupString] = new List<CommJob>();

                    List<CommJob> tmpList = mapGroupedJobs[candJob.GroupString];

                    // try to a aggregate job
                    for (int jobIndex = tmpList.Count - 1; jobIndex >= 0; jobIndex--)
                    {
                        CommJob testJob = tmpList[jobIndex];

                        var aggType = testJob.TestAggregateJob(candJob, out uint newIndex);

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

                            // set job that will be use to create all reference
                            candJob = testJob;

                            bAggregated = true;
                            break;
                        }
                    }

                    if (!bAggregated)
                        //add candJob to the list
                        tmpList.Add(candJob);                
                }
                if (!bAggregated)
                {
                    if (!MapWholeJob.ContainsKey(candJob))
                    {
                        MapWholeJob[candJob] = true;
                        ListWholeJob.Add(candJob);
                    }                    
                }

                AddToMapTagJob(candTag.TagNode.NodeId, candJob);
                GetCommDriver().AddToTagToStationMap(candTag.TagNode.NodeId, this);

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

                UpdateTagVaue(candTag.TagNode.NodeId, new DataValue()
                {
                    Value = candTag.TagNode.InitialValue,
                    ServerTimestamp = DateTime.UtcNow,
                    SourceTimestamp = DateTime.MinValue,
                    StatusCode = candTag.TagNode.InitialValue != null ? StatusCodes.Good : StatusCodes.BadWaitingForInitialData
                });
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the job executed action. </summary>
        ///
        /// <param name="sender" type="object">     Source of the event. </param>
        /// <param name="e" type="ExecutedJobArgs"> The ExecutedJobArgs to process. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void OnJobExecuted(object sender, ExecutedJobArgs e)
        {
            if (e.Job.Station != this)
            {
                return;
            }
#if DEBUG
            //System.Diagnostics.Trace.TraceInformation("{3} station.OnJobExecuted '{2}' onWriting:{0} synchro:{1}", e.Job.TagsListOnWriting.Count, e.Job.SyncroExec, e.Job.Name, Thread.CurrentThread.ManagedThreadId);
#endif
            ProcessJobValues(e);
            ProcessJob(e);
        }

        void ProcessJob(ExecutedJobArgs e)
        {
            if (e.ChangedTags.Count == 0)
                e.Job.SetErrorState((int)e.ErrorCode);

            if (e.Job.IsChildJob)
            {
                e.Job.ChildJobErrorCode = e.ErrorCode;                
                AllignTagsListOnWritingAndToWrite(e.Job, e.ErrorCode);
                return;
            }

            ProcessJobLists(e.Job, e.ErrorCode, bGeneralError: e.GeneralError);

            if (StatisticsData != null)
            {
                if (_InErrorState)
                    StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), true);
                else
                    StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), false);
            }

            //e.Job.IsPending = false;

#if DEBUG
            //System.Diagnostics.Trace.TraceInformation("{3} ProcessJob '{2}' IsPending:{0} Synchro:{1}",
            //    e.Job.IsPending, e.Job.SyncroExec, e.Job.Name, Thread.CurrentThread.ManagedThreadId);

#endif
        }
        /// <summary>   Executes the refresh diagnostic action. </summary>
        public void OnRefreshDiagnostic()
        {
            if (StatisticsData != null)
            {

                StatisticsData.Update(StatisticSetting.NodeDataNames.JobRate.ToString(), TotalPartialJobs, send: true);
                StatisticsData.Update(StatisticSetting.NodeDataNames.TagRate.ToString(), TotalPartialTags, send: true);
                StatisticsData.Update(StatisticSetting.NodeDataNames.ByteRate.ToString(), TotalPartialBytes, send: true);
                TotalPartialJobs = 0;
                TotalPartialTags = 0;
                TotalPartialBytes = 0;
                if (_InErrorState)
                    StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), true, send: true);
                else
                    StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), false, send: true);

                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalJobsRead.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalJobsWrite.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalTagsRead.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalTagsWrite.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalBytesRead.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalBytesWrite.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalRxBytes.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalTxBytes.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.TotalJobsError.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.LastErrorTime.ToString());
                StatisticsData.NotifyValue(StatisticSetting.NodeDataNames.LastError.ToString());
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   processed received data in ExecutedJobArgs to call asyncronously... </summary>
        ///
        /// <param name="e">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void ProcessJobValues(ExecutedJobArgs e)
        {
            if (StatisticsData != null)
            {
                e.Job.BasicCalculateStatistic();

                StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalRxBytes.ToString(), e.RxBytes);
                StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalTxBytes.ToString(), e.TxBytes);
                e.RxBytes = 0;
                e.TxBytes = 0;

                if (e.ErrorCode != DriverErrorCodes.ErrorNoError)
                {
                    StatisticsData.Update(StatisticSetting.NodeDataNames.LastErrorTime.ToString(), DateTime.Now.ToString());
                    uint quality;
                    String DiagnErrorMessage = "";
                    CommDriver.GetDriverErrorInfo((int)e.ErrorCode, out quality, out DiagnErrorMessage);
                    StatisticsData.Update(StatisticSetting.NodeDataNames.LastError.ToString(), DiagnErrorMessage);
                    StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalJobsError.ToString());
                }
                else
                {
                    if (e.Job.IsRead)
                    {
                        StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalJobsRead.ToString());
                        StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalTagsRead.ToString(), e.Job.ExchangedTag);
                        StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalBytesRead.ToString(), e.Job.ExchangedByte);
                    }
                    else
                    {
                        StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalJobsWrite.ToString());
                        StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalTagsWrite.ToString(), e.Job.ExchangedTag);
                        StatisticsData.IncreaseCounter(StatisticSetting.NodeDataNames.TotalBytesWrite.ToString(), e.Job.ExchangedByte);
                    }
                    TotalPartialJobs++;
                    TotalPartialTags += e.Job.ExchangedTag;
                    TotalPartialBytes += e.Job.ExchangedByte;
                }
            }

            if (e.ChangedTags.Count > 0)
            {
#if DEBUG
                //System.Diagnostics.Trace.TraceInformation(string.Format("ChangedTags: {0}", e.ChangedTags.Count));
#endif
                if (e.Job.IsChildJob)
                    return;

                if (e.Job.SyncroExec)
                    NotifyTags(e.ChangedTags, (int)e.ErrorCode, e.Timestamp);
                else
                    GetCommDriver().SmartThreadPool.QueueWorkItem(() =>
                    {
                        NotifyTags(e.ChangedTags, (int)e.ErrorCode, e.Timestamp);
                    });
            }
        }

        void NotifyTags(List<Tag> changedTags, int errCode, DateTime timeStamp)
        {
            uint quality;
            string error;
            GetCommDriver().GetDriverErrorInfo(errCode, out quality, out error);
            foreach (var tag in changedTags)
            {
                tag.Value.StatusCode = quality;
                GetCommDriver().OnTagChanged(tag.TagNode.NodeId, tag.Value, timeStamp, DisableQualityUpdate);
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets built in type. </summary>
        ///
        /// <param name="systemType" type="Type">   Type of the system. </param>
        ///
        /// <returns>   The built in type. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BuiltInType GetBuiltInType(Type systemType)
        {
            if (systemType == typeof(bool)) { return BuiltInType.Boolean; }
            if (systemType == typeof(sbyte)) { return BuiltInType.SByte; }
            if (systemType == typeof(byte)) { return BuiltInType.Byte; }
            if (systemType == typeof(short)) { return BuiltInType.Int16; }
            if (systemType == typeof(ushort)) { return BuiltInType.UInt16; }
            if (systemType == typeof(int)) { return BuiltInType.Int32; }
            if (systemType == typeof(uint)) { return BuiltInType.UInt32; }
            if (systemType == typeof(long)) { return BuiltInType.Int64; }
            if (systemType == typeof(ulong)) { return BuiltInType.UInt64; }
            if (systemType == typeof(float)) { return BuiltInType.Float; }
            if (systemType == typeof(double)) { return BuiltInType.Double; }
            //if (systemType == typeof(string)) { return BuiltInType.String; }
            //if (systemType == typeof(DateTime)) { return BuiltInType.DateTime; }
            //if (systemType == typeof(Guid)) { return BuiltInType.Guid; }
            //if (systemType == typeof(Uuid)) { return BuiltInType.Guid; }
            //if (systemType == typeof(byte[])) { return BuiltInType.ByteString; }
            //if (systemType == typeof(XmlElement)) { return BuiltInType.XmlElement; }
            //if (systemType == typeof(NodeId)) { return BuiltInType.NodeId; }
            //if (systemType == typeof(ExpandedNodeId)) { return BuiltInType.ExpandedNodeId; }
            //if (systemType == typeof(StatusCode)) { return BuiltInType.StatusCode; }
            //if (systemType == typeof(DiagnosticInfo)) { return BuiltInType.DiagnosticInfo; }
            //if (systemType == typeof(QualifiedName)) { return BuiltInType.QualifiedName; }
            //if (systemType == typeof(LocalizedText)) { return BuiltInType.LocalizedText; }
            //if (systemType == typeof(ExtensionObject)) { return BuiltInType.ExtensionObject; }
            //if (systemType == typeof(DataValue)) { return BuiltInType.DataValue; }
            //if (systemType == typeof(Variant)) { return BuiltInType.Variant; }
            //if (systemType == typeof(object)) { return BuiltInType.Variant; }

            // not a recognized type.
            return BuiltInType.Null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets array built in type. </summary>
        ///
        /// <param name="systemType" type="Type">   Type of the system. </param>
        ///
        /// <returns>   The built in type. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BuiltInType GetBuiltInArrayType(Type systemType)
        {
            if (systemType == typeof(bool[])) { return BuiltInType.Boolean; }
            if (systemType == typeof(sbyte[])) { return BuiltInType.SByte; }
            if (systemType == typeof(byte[])) { return BuiltInType.Byte; }
            if (systemType == typeof(short[])) { return BuiltInType.Int16; }
            if (systemType == typeof(ushort[])) { return BuiltInType.UInt16; }
            if (systemType == typeof(int[])) { return BuiltInType.Int32; }
            if (systemType == typeof(uint[])) { return BuiltInType.UInt32; }
            if (systemType == typeof(long[])) { return BuiltInType.Int64; }
            if (systemType == typeof(ulong[])) { return BuiltInType.UInt64; }
            if (systemType == typeof(float[])) { return BuiltInType.Float; }
            if (systemType == typeof(double[])) { return BuiltInType.Double; }
            //if (systemType == typeof(string)) { return BuiltInType.String; }
            //if (systemType == typeof(DateTime)) { return BuiltInType.DateTime; }
            //if (systemType == typeof(Guid)) { return BuiltInType.Guid; }
            //if (systemType == typeof(Uuid)) { return BuiltInType.Guid; }
            //if (systemType == typeof(byte[])) { return BuiltInType.ByteString; }
            //if (systemType == typeof(XmlElement)) { return BuiltInType.XmlElement; }
            //if (systemType == typeof(NodeId)) { return BuiltInType.NodeId; }
            //if (systemType == typeof(ExpandedNodeId)) { return BuiltInType.ExpandedNodeId; }
            //if (systemType == typeof(StatusCode)) { return BuiltInType.StatusCode; }
            //if (systemType == typeof(DiagnosticInfo)) { return BuiltInType.DiagnosticInfo; }
            //if (systemType == typeof(QualifiedName)) { return BuiltInType.QualifiedName; }
            //if (systemType == typeof(LocalizedText)) { return BuiltInType.LocalizedText; }
            //if (systemType == typeof(ExtensionObject)) { return BuiltInType.ExtensionObject; }
            //if (systemType == typeof(DataValue)) { return BuiltInType.DataValue; }
            //if (systemType == typeof(Variant)) { return BuiltInType.Variant; }
            //if (systemType == typeof(object)) { return BuiltInType.Variant; }

            // not a recognized type.
            return BuiltInType.Null;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets type size. </summary>
        ///
        /// <param name="systemType" type="Type">   Type of the system. </param>
        ///
        /// <returns>   The type size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static uint GetTypeSize(Type systemType)
        {
            if (systemType == typeof(bool)) { return sizeof(bool); }
            if (systemType == typeof(sbyte)) { return sizeof(sbyte); }
            if (systemType == typeof(byte)) { return sizeof(byte); }
            if (systemType == typeof(short)) { return sizeof(short); }
            if (systemType == typeof(ushort)) { return sizeof(ushort); }
            if (systemType == typeof(int)) { return sizeof(int); }
            if (systemType == typeof(uint)) { return sizeof(uint); }
            if (systemType == typeof(long)) { return sizeof(long); }
            if (systemType == typeof(ulong)) { return sizeof(ulong); }
            if (systemType == typeof(float)) { return sizeof(float); }
            if (systemType == typeof(double)) { return sizeof(double); }
            //if (systemType == typeof(string)) { return sizeof(string); }
            return 0;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets bin t size. </summary>
        ///
        /// <param name="bt" type="BuiltInType">    The bt. </param>
        ///
        /// <returns>   The bin t size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static uint GetBinTSize(BuiltInType bt)
        {
            uint size = 0;
            switch (bt)
            {
                case BuiltInType.Boolean:
                case BuiltInType.SByte:
                case BuiltInType.Byte:
                    size = 1;
                    break;
                case BuiltInType.Int16:
                case BuiltInType.UInt16:
                    size = 2;
                    break;
                case BuiltInType.Int32:
                case BuiltInType.UInt32:
                    size = 4;
                    break;
                case BuiltInType.Int64:
                case BuiltInType.UInt64:
                    size = 8;
                    break;
                case BuiltInType.Float:
                    size = 4;
                    break;
                case BuiltInType.Double:
                    size = 8;
                    break;
            }
            return size;

        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Parse received to arguments. </summary>
        ///
        /// <param name="receivedbuffer" type="byte[]">         The receivedbuffer. </param>
        /// <param name="job" type="CommJob">                   The job. </param>
        /// <param name="arguments" type="ref List<Object>">    [in,out] The arguments. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            return true;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the method call action. </summary>
        ///
        /// <param name="node" type="NodeId">                   The node. </param>
        /// <param name="inputArguments" type="IList<object>">  The input arguments. </param>
        /// <param name="outputArguments" type="IList<object>"> The output arguments. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint OnMethodCall(NodeId node, IList<object> inputArguments, IList<object> outputArguments)
        {
            CommDriver.OnSystemEvent(null, String.Format("OnMethodCall {1} - {0}", CommDriver.DriverName, Name), EventSeverity.Low);
            lock (lockListObject)
            {

                if (mapMethodJobs.ContainsKey(node))
                    return StatusCodes.BadNodeIdInvalid;
                foreach (var job in mapMethodJobs[node])
                {

                    Tag tag = job.TagsList.Find(o => { return o.TagNode.NodeId == node; });

                    List<Tag> tl = (from t in job.TagsList.AsParallel()
                                    where (t.TagNode.NodeId != node && t.DynSettings.MethodID == -1)
                                    select t).ToList();
                    bool localMethod = (tl.Count > 0);

                    if (tag == null)
                        return StatusCodes.BadMethodInvalid;

                    uint tempJobSize = job.TotalJobSize;
                    List<Tag> tempTagList = new List<Tag>();
                    if (!localMethod)
                    {
                        foreach (var d in job.TagsList)
                            tempTagList.Add(d);
                        job.TagsList.Clear();
                        job.TotalJobSize = 0;
                    }
                    LinkType lt = job.Type;

                    switch (tag.DynSettings.MethodID)
                    {
                        /*case (int)DriverMethods.ReadSynchro:
                            job.Type = LinkType.Input;
                            if (!localMethod)
                            {
                                for (int i = 1; i < outputArguments.Count; i++)
                                {
                                    Type t = outputArguments[i].GetType();
                                    bool array = t.IsArray;
                                    uint size = GetTypeSize(t);
                                    if (size == 0)
                                    {
                                        job.TotalJobSize = tempJobSize;
                                        job.TagsList.Clear();
                                        foreach (var d in tempTagList)
                                            job.TagsList.Add(d);
                                        job.Type = lt;
                                        return StatusCodes.BadInvalidArgument;
                                    }
                                    BuiltInType bt = GetBuiltInType(t);
                                    if (bt == BuiltInType.Null)
                                    {
                                        job.TotalJobSize = tempJobSize;
                                        job.TagsList.Clear();
                                        foreach (var d in tempTagList)
                                            job.TagsList.Add(d);
                                        job.Type = lt;
                                        return StatusCodes.BadInvalidArgument;
                                    }
                                    Tag tt = CreateTag(new TagDefinition() { DataType = new NodeId((uint)bt) });

                                    tt.Size = size;
                                    tt.ByteOffset = job.TotalJobSize;
                                    tt.Value.Value = outputArguments[i];

                                    job.TagsList.Add(tt);
                                    job.TotalJobSize += tt.Size;
                                }
                            }
                            break;
                        case (int)DriverMethods.WriteSynchro:
                            job.Type = LinkType.UnconditionalOutput;
                            if (!localMethod)
                            {
                                for (int i = 1; i < inputArguments.Count; i++)
                                {
                                    Type t = inputArguments[i].GetType();
                                    bool array = t.IsArray;
                                    uint size = GetTypeSize(t);
                                    if (size == 0)
                                    {
                                        job.TotalJobSize = tempJobSize;
                                        job.TagsList.Clear();
                                        foreach (var d in tempTagList)
                                        {
                                            job.TagsList.Add(d);
                                        }
                                        job.Type = lt;
                                        return StatusCodes.BadInvalidArgument;
                                    }
                                    BuiltInType bt = GetBuiltInType(t);
                                    if (bt == BuiltInType.Null)
                                    {
                                        job.TotalJobSize = tempJobSize;
                                        job.TagsList.Clear();
                                        foreach (var d in tempTagList)
                                            job.TagsList.Add(d);
                                        job.Type = lt;
                                        return StatusCodes.BadInvalidArgument;
                                    }
                                    Tag tt = CreateTag(new TagDefinition() { DataType = new NodeId((uint)bt) });

                                    tt.Size = size;
                                    tt.ByteOffset = job.TotalJobSize;
                                    tt.Value.Value = inputArguments[i];

                                    job.TagsList.Add(tt);
                                    job.TagsListToWrite.Add(tt);
                                    job.TotalJobSize += tt.Size;
                                }
                            }
                            break;*/
                        default:
                            job.TotalJobSize = tempJobSize;
                            job.TagsList.Clear();
                            foreach (var d in tempTagList)
                                job.TagsList.Add(d);
                            job.Type = lt;
                            return StatusCodes.BadMethodInvalid;
                    }

                    uint resexec = ExecuteSyncroJob(job, localMethod, tag.DynSettings.MethodID, outputArguments);
                    if (resexec != StatusCodes.Good)
                    {
                        outputArguments[0] = DriverErrorCodes.ErrorTimeOut;
                        job.TotalJobSize = tempJobSize;
                        job.TagsList.Clear();
                        foreach (var d in tempTagList)
                            job.TagsList.Add(d);
                        job.Type = lt;
                        return resexec;
                    }


                    job.LocalMethod = false;
                    job.SyncroExec = false;
                    job.MethodID = -1;
                    job.Type = lt;
                    if (!localMethod)
                    {
                        job.TotalJobSize = tempJobSize;
                        job.TagsList.Clear();
                        foreach (var d in tempTagList)
                            job.TagsList.Add(d);
                        job.TagsListToWrite.Clear();
                    }
                }
            }

            return StatusCodes.Good;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the syncro job operation. </summary>
        ///
        /// <param name="job" type="CommJob">           The job. </param>
        /// <param name="localmethod" type="bool">      true to localmethod. </param>
        /// <param name="methodid" type="int">          The methodid. </param>
        /// <param name="lt" type="LinkType">           The lt. </param>
        /// <param name="args" type="IList<object>">    The arguments. </param>
        /// <param name="takedata" type="bool">         (Optional) true to takedata. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual uint ExecuteSyncroJob(CommJob job, bool localmethod, int methodid, IList<object> args, bool takedata = true, NodeId tagNodeId = null, object value = null)
        {
            uint ret = StatusCodes.Good;

            job.LocalMethod = localmethod;
            job.MethodID = methodid;

            Channel.Startup();


            Channel.commExecuteSyncro(job, tagNodeId, value, Channel.Timeout);

            args[0] = job.SynchroError;

            //prendi i dati o il risultato  dell'esecuzione e ritorna
            if (takedata && job.SynchroError == (int)DriverErrorCodes.ErrorNoError &&
                localmethod == false && methodid == (int)DriverSynchroOperation.ReadSynchro)
            {
                byte[] recval = (byte[])job.SynchroValues;
                List<object> outarg = (List<object>)args;
                ParseReceivedToArguments(recval, job, ref outarg);
            }
#if DEBUG
            System.Diagnostics.Trace.TraceInformation("{4} ExecuteSyncroJob ret:{0} err:{1} {2}.{3}", ret, args[0], DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond, Thread.CurrentThread.ManagedThreadId);
#endif
            return ret;
        }
        #endregion

        #region Public Methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the channel. </summary>
        ///
        /// <returns>   The channel. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Channel GetChannel()
        {
            return Channel;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets communications driver. </summary>
        ///
        /// <returns>   The communications driver. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public CommunicationDriver GetCommDriver()
        {
            return CommDriver;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the tag described by oldTag. </summary>
        ///
        /// <param name="oldTag" type="Tag">    The old tag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RemoveTag(Tag oldTag)
        {
            lock (lockListObject)
            {
                if (mapTagJob.ContainsKey(oldTag.TagNode.NodeId))
                    if (mapTagJob[oldTag.TagNode.NodeId].RemoveTag(oldTag))
                        mapTagJob.Remove(oldTag.TagNode.NodeId);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds a job. </summary>
        ///
        /// <param name="entity" type="CommJob">    The entity. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddJob(CommJob entity)
        {
            bool notContained = false;
            lock (lockListObject)
            {
                if (!MapWholeJob.ContainsKey(entity))
                {
                    MapWholeJob[entity] = true;
                    ListWholeJob.Add(entity);

                    notContained = true;
                }
            }

            if (notContained)
            {
                foreach (var tag in entity.TagsList)
                {                    
                    UpdateTagVaue(tag.TagNode.NodeId, new DataValue()
                    {
                        Value = tag.TagNode.InitialValue,
                        ServerTimestamp = DateTime.UtcNow,
                        SourceTimestamp = DateTime.MinValue,
                        StatusCode = tag.TagNode.InitialValue != null ? StatusCodes.Good : StatusCodes.BadWaitingForInitialData
                    });                    
                }

                if (entity.IsStruct())
                {
                    UpdateTagVaue(entity.StructTag.TagNode.NodeId, new DataValue()
                    {
                        Value = entity.StructTag.TagNode.InitialValue,
                        ServerTimestamp = DateTime.UtcNow,
                        SourceTimestamp = DateTime.MinValue,
                        StatusCode = entity.StructTag.TagNode.InitialValue != null ? StatusCodes.Good : StatusCodes.BadWaitingForInitialData
                    });
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds a job list. </summary>
        ///
        /// <param name="entities" type="IList<CommJob>">   The entities. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddJobList(IList<CommJob> entities)
        {
            foreach (CommJob entity in entities)
            {
                lock (lockListObject)
                {
                    if (!MapWholeJob.ContainsKey(entity))
                    {
                        MapWholeJob[entity] = true;
                        ListWholeJob.Add(entity);
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the job described by entity. </summary>
        ///
        /// <param name="entity" type="CommJob">    The entity. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RemoveJob(CommJob entity)
        {
            lock (lockListObject)
            {
                ListWholeJob.Remove(entity);
                if (MapWholeJob.ContainsKey(entity))
                    MapWholeJob.Remove(entity);
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Removes the job list described by entities. </summary>
        ///
        /// <param name="entities" type="IList<CommJob>">   The entities. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void RemoveJobList(IList<CommJob> entities)
        {
            foreach (CommJob entity in entities)
            {
                lock (lockListObject)
                {
                    ListWholeJob.Remove(entity);
                }
            }
        }

        public void RemoveAllJobs()
        {
            lock (lockListObject)
            {
                foreach (CommJob job in ListWholeJob)
                    job.TagsList.Clear();
                ListWholeJob.Clear();
                MapWholeJob.Clear();
                mapTagJob.Clear();
                foreach (var KeyValue in mapGroupedJobs)
                    KeyValue.Value.Clear();
                mapGroupedJobs.Clear();
                foreach (var KeyValue in mapMethodJobs)
                    KeyValue.Value.Clear();
                mapMethodJobs.Clear();
                foreach (var KeyValue in mapJobMethods)
                    KeyValue.Value.Clear();
                mapJobMethods.Clear();
                foreach (var KeyValue in ObservedTagToJobsMap)
                    KeyValue.Value.Clear();
                ObservedTagToJobsMap.Clear();
                listErrorJobs.Clear();
            }
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Adds to the map tag job. </summary>
        ///
        /// <param name="node" type="NodeId">   The node. </param>
        /// <param name="c" type="CommJob">     The CommJob to process. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void AddToMapTagJob(NodeId node, CommJob c)
        {
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(node))
                    mapTagJob.Add(node, c);
            }
        }

        public List<Tag> GetAllTags()
        {
            List<Tag> tags = new List<Tag>();
            lock (lockListObject)
            {
                foreach (var job in ListWholeJob)
                {
                    tags.AddRange(job.TagsList);
                    if (job.IsStruct())
                        tags.Add(job.StructTag);
                }
            }

            return tags;
        }

        public int GetAllTagsCount()
        {
            int tags = 0;
            lock (lockListObject)
                tags = ListWholeJob.Sum(job => job.TagsList.Count);

            return tags;
        }

        public int GetListWholeJobCount()
        {
            int jobs = 0;
            lock (lockListObject)
                jobs = ListWholeJob.Count;

            return jobs;
        }
        #endregion

        #region Private Methods

        protected void AllignTagsListOnWritingAndToWrite(CommJob job, DriverErrorCodes error)
        {
            if (job != null && (job.RWState != CommJob.RWStates.ReadForRW && job.GetTagListOnWritingCount() > 0))
            {
                job.NrConsecutiveWrite++;
                // FOGBUGZ 11531
                switch (job.Type)
                {
                    case LinkType.InputOutput:
                        // assign written value only if no error occured
                        if (error == DriverErrorCodes.ErrorNoError)
                        {
                            foreach (var tag in job.GetTagListOnWriting())
                                tag.SetValue(tag.WriteVal);
                        }
                        break;
                    case LinkType.UnconditionalOutput:
                    case LinkType.ExceptionOutput:
                        if (error == DriverErrorCodes.ErrorNoError)
                        {
                            foreach (var tag in job.GetTagListOnWriting())
                                tag.SetValue(tag.WriteVal);
                        }
                        else
                        {
                            job.ReFillTagListToWrite();
                        }
                        break;                   
                }
                
                job.ClearTagListOnWriting();
                if (job.GetTagListToWriteCount() == 0)
                    ChannelBase.ChangeStateJob(job, (job.InUse ? CommJobState.PollingInUse : CommJobState.PollingNotInUse));
            }
            else
            {
                job.NrConsecutiveWrite = 0;
            }

            if (job.SyncroExec && job.InErrorState)
            {
                job.ClearTagListWrite();
                job.ClearTagListOnWriting();
            }
        }

        public string AdditionalError;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Process the job lists. </summary>
        ///
        /// <param name="job" type="CommJob">               The job. </param>
        /// <param name="error" type="DriverErrorCodes">    The error. </param>
        /// <param name="bCheckNumRetries" type="bool">     (Optional) true to check number retries. </param>
        /// <param name="bGeneralError" type="bool">        (Optional) signal an error that influences all the job of the station. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void ProcessJobLists(CommJob job, DriverErrorCodes error, bool bCheckNumRetries = true, bool bGeneralError = false)
        {
            bool inInErrorState = job.InErrorState;
            if (error != DriverErrorCodes.ErrorNoError)
            {
                _StationLastErrorTime = DateTime.UtcNow;                

                if (_LastErrorCode == DriverErrorCodes.ErrorNoError)
                {
                    _LastErrorTime = DateTime.UtcNow;
                    _LastErrorCode = error;
                    AdditionalError = (job.TagsList.Count > 0 ? job.TagsList[0].DynSettings.ToString() : null);
                }
                    
                if (!InErrorState)
                {
                    // keep general error "active" if is not TIMEOUT and consecutive CurRetriesBeforeError >= MaxRetriesBeforeError
                    if (bGeneralError)
                        bGeneralError = IsConsecutiveTimeOut(CurRetriesBeforeError, error);
                    if (!bCheckNumRetries || ++CurRetriesBeforeError >= MaxRetriesBeforeError || (job.RWState != CommJob.RWStates.ReadForRW && job.GetTagListOnWritingCount() != 0) || bGeneralError)
                    {
                        CurRetriesBeforeError = 0;
                        InErrorState = true;
                        Channel.ConsecutiveCommErrors++;

                        if (isStationError(error) || bGeneralError)
                        {
                            SetJobsGeneralError((int)error);
                            ReturnFromGeneralError = true;
                        }
                        else
                        {
                            job.SetErrorState((int)error, !job.InErrorState);
                            ChannelBase.ChangeStateJob(job, CommJobState.PollingInError);
                        }                        
#if DEBUG
                        System.Diagnostics.Trace.TraceInformation(string.Format("InErrorState:{0}  {1}.{2}", InErrorState, DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond));
#endif                    
                    }
#if DEBUG
                    //System.Diagnostics.Trace.TraceInformation(string.Format("CurRetriesBeforeError ERR {0} InErrorState:{3}  {1}.{2}", CurRetriesBeforeError, DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond, InErrorState));
#endif
                }
                if (!job.InErrorState)
                    job.SetErrorState((int)error);
                else
                    job.UpdateErrorTime();

                if (InErrorState)
                {
                    if (ChannelBase != null)
                    {
                        if (!isStationError(error) && job != null)
                        {
                            ChannelBase.ChangeStateJob(job, CommJobState.PollingInError);
                        }

                        if (bGeneralError && !ReturnFromGeneralError)
                        {
                            SetJobsGeneralError((int)error);
                            ReturnFromGeneralError = true;
                        }

                        if (SuspendJobInError)
                        {
                            var listJob = new List<CommJob>();
                            lock (lockListObject)
                            {
                                listJob.AddRange(ListWholeJob);
                            }

                            //foreach (var commjob in listJob)
                            //    ChannelBase.UnsubscribeJob(commjob);
                            ChannelBase.UnsubscribeJobs(listJob);
                        }
                    }
                    SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                }
            }
            else
            {
                CurRetriesBeforeError = 0;//count consecutive errors...
#if DEBUG
                //System.Diagnostics.Trace.TraceInformation(string.Format("CurRetriesBeforeError RESET {0} {1}.{2}", CurRetriesBeforeError, DateTime.Now.ToLongTimeString(), DateTime.Now.Millisecond));
#endif
                // reset not is use jobs is error state when driver reconnect to device
                if (ReturnFromGeneralError)
                {
                    ReturnFromGeneralError = false;

                    List<CommJob> tempListErrorJobs = new List<CommJob>();
                    lock (lockListObject)
                        tempListErrorJobs.AddRange(listErrorJobs);

                    Parallel.ForEach(tempListErrorJobs, j =>
                    {
                        if (!j.InUse)
                        {
                            j.SetInErrorStateInternalValue(false);
                            j.SetUncertainLastUsableValueQuality();
                        }

                        // quick polling in error
                        j.UpdateErrorTime();
                        j.ErrorPollingTime = 0; // force to execute immediatly
                    });

                    Parallel.ForEach(ListWholeJob, j =>
                    {
                        if ((j != job) && (j.Type == LinkType.ExceptionOutput))
                        {
                            // Uncertain is the initial quality for jobs of type Exception Output
                            j.SetUncertainQuality();
                        }
                    });
                }

                if ((job != null && job.InErrorState) || InErrorState)
                {
                    if (job != null)
                        job.SetErrorState((int)error, false);
                    
                    bool resetError = (InErrorJobs() == 0);
                    if (resetError)
                    {
                        Channel.ConsecutiveCommErrors = 0;
                        InErrorState = false;

                        _LastErrorTime = DateTime.MinValue;
                        _LastErrorCode = DriverErrorCodes.ErrorNoError;
                    }

                    if (ChannelBase != null)
                    {
                        if (SuspendJobInError)
                        {
                            var listJob = new List<CommJob>();
                            lock (lockListObject)
                            {
                                listJob.AddRange(listErrorJobs);
                                listErrorJobs.Clear();
                            }

                            foreach (var commjob in listJob)
                            {
                                if (commjob.InUse)
                                    ChannelBase.SubscribeJob(commjob, CommJobState.PollingInUse);
                                else
                                    ChannelBase.SubscribeJob(commjob, CommJobState.PollingNotInUse);
                            }
                        }
                        else
                        {
                            if (job != null)
                            {
                                if (job.InUse)
                                    ChannelBase.ChangeStateJob(job, CommJobState.PollingInUse);
                                else
                                {
                                    ChannelBase.ChangeStateJob(job, CommJobState.PollingNotInUse);
                                }
                            }
                        }
                    }
                }
            }

            // state varible reset
            if (error == DriverErrorCodes.ErrorNoError)
            {
                // station
                bool inError = false;
                if (GetStateCommandVariableBit(ref inError, (UInt16)StationVariableBits.StationErrorState) == true) 
                {                    
                    if (inError && InErrorJobs() == 0)
                        SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
                }
                // job
                job.ResetConditionalVariable();
            }            

            lock (lockListObject)
            {
                if (job != null && !job.InErrorState && FirstTime)
                {
                    FirstTime = false;
                    CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.StationResumeError, CommDriver.DriverName/*DriverInfo.GetDriverName()*/, Name), EventSeverity.Low);
                }
            }

            // once job is terminated, clear/move data from TagListOnWriting to TagListToWriteCount depending on error value
            AllignTagsListOnWritingAndToWrite(job, error);
        }

        void SetJobsGeneralError(int error)
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }
            listJob = (from j in listJob.AsParallel()
                       where j.InUse && !j.InErrorState
                       select j).ToList();

            Channel.RemoveStationJobsOnQueue(this);
            foreach (CommJob commjob in listJob)
            {
                if (IsCompatibleWithErrorList(commjob))
                {
                    /*When a general error occurs, the jobs are put in error it is necessary 
                        * to distinguish the jobs that have conditional variables or jobs excepionOutput, 
                        * because at the resumption of communication (eg Timeout), 
                        * jobs of type exceptionoutput and jobs with conditional variables, 
                        * would keep in error the station until their execution.*/
                    if (((commjob.Type != LinkType.ExceptionOutput) && !commjob.ConditionalVariableSet))
                        commjob.SetErrorState((int)error, false);
                    else if(commjob.Type == LinkType.ExceptionOutput)
                    {
                        // Uncertain is the initial quality for jobs of type Exception Output
                        commjob.SetUncertainQuality();
                    }
                    ChannelBase.ChangeStateJob(commjob, CommJobState.PollingInError);
                }
            }
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   States if a job can be put in the list of job in error. </summary>
        ///
        /// <returns>   true if the job can enter the error list; otherwise, false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool IsCompatibleWithErrorList(CommJob commjob)
        {
            return true;
        }

        bool isStationError(DriverErrorCodes errorCode)
        {
            switch (errorCode)
            {
                case DriverErrorCodes.ErrorDeviceOpenFailed:
                case DriverErrorCodes.ErrorTimeOut:
                    return true;
            }
            return false;
        }


        public void AddErrorJob(CommJob job)
        {
            lock (lockListObject)
            {
                if (!job.SyncroExec && !job.IsChildJob)
                {
                    if (!listErrorJobs.Contains(job))
                        listErrorJobs.Add(job);
                }
            }
        }
        public void RemoveErrorJob(CommJob job)
        {
            lock (lockListObject)
                listErrorJobs.Remove(job);
        }

        protected long InErrorJobs()
        {
            lock (lockListObject)
                return (long)listErrorJobs.Count;
        }
        #endregion

        #region Properties
        /// <summary>   The last error time. </summary>
        private DateTime _LastErrorTime;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the last error time. </summary>
        ///
        /// <value> The last error time. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DateTime LastErrorTime
        {
            get { return _LastErrorTime; }
            set
            {
                _LastErrorTime = value;
            }
        }
        /// <summary>   The last error code. </summary>
        private DriverErrorCodes _LastErrorCode;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the last error code. </summary>
        ///
        /// <value> The last error code. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DriverErrorCodes LastErrorCode
        {
            get { return _LastErrorCode; }
            set
            {
            	_LastErrorCode = value;
            }
        }
        /// <summary>   true to in error state. </summary>
        private bool _InErrorState;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the in error state. </summary>
        ///
        /// <value> true if in error state, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool InErrorState
        {
            get { return _InErrorState; }
            set
            {
                if (_InErrorState != value)
                {
                    _InErrorState = value;
                    if (value == true)
                    {
                        uint quality;
                        string error;
                        CommDriver.GetDriverErrorInfo((int)LastErrorCode, out quality, out error);
                        string add = string.Empty;
                        if (AdditionalError != null)
                            add = string.Format("({0})", AdditionalError);
                        CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.StationInError, CommDriver.DriverName/*DriverInfo.GetDriverName()*/, Name, error, add), EventSeverity.Low);
                        CommDriver.OnStateChanged(ComunicationState.Fault);
                    }
                    else 
                    {
                        CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.StationResumeError, CommDriver.DriverName/*DriverInfo.GetDriverName()*/, Name), EventSeverity.Low);
                        CommDriver.OnStateChanged(ComunicationState.Running);
                    }
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the station is in general error state. </summary>
        ///
        /// <value> true if in general error state, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool InGeneralErrorState
        {
            get { return InErrorState && ReturnFromGeneralError; }
        }

        /// <summary>   The name. </summary>
        private string _Name;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name. </summary>
        ///
        /// <value> The name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Name
        {
            get { return _Name; }
            set
            {
                _Name = value;
            }
        }

        private bool _RewritingOfTheSameValue;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the rewriting of the some value. </summary>
        ///
        ///
        /// <value> The name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool RewritingOfTheSameValue
        {
            get
            {
                return _RewritingOfTheSameValue;
            }
            set
            {
                 _RewritingOfTheSameValue = value;
            }
        }

        private bool _DisableQualityUpdate;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Disable Quality Update. </summary>
        ///
        ///
        /// <value> The name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool DisableQualityUpdate
        {
            get
            {
                return _DisableQualityUpdate;
            }
            set
            {
                _DisableQualityUpdate = value;
            }
        }

        /// <summary>   The maximum retries before error. </summary>
        private uint _MaxRetriesBeforeError;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the maximum retries before error. </summary>
        ///
        /// <value> The maximum retries before error. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint MaxRetriesBeforeError
        {
            get { return _MaxRetriesBeforeError; }
            set
            {
                _MaxRetriesBeforeError = value;
            }
        }

        /// <summary>   true to suspend job in error. </summary>
        private bool _SuspendJobInError;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the suspend job in error. </summary>
        ///
        /// <value> true if suspend job in error, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SuspendJobInError
        {
            get { return _SuspendJobInError; }
            set
            {
                _SuspendJobInError = value;
            }
        }

        /// <summary>   true to first time. </summary>
        private bool _FirstTime;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets a value indicating whether the first time. </summary>
        ///
        /// <value> true if first time, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool FirstTime
        {
            get { return _FirstTime; }
            set
            {
                _FirstTime = value;
            }
        }

        /// <summary>  The State-Command variable of the station. </summary>
        private UFUAModel.TagEntityReference _StateCommandTag;
        /// <summary>
        /// Gets or sets the the State-Command variable of the station.
        /// </summary>
        public UFUAModel.TagEntityReference StateCommandTag
        {
            get
            {
                return _StateCommandTag;
            }
            set
            {
                _StateCommandTag = value;
            }
        }

        private UFUAModel.TagEntityReference _StateTag;
        /// <summary>
        /// Gets or sets the the State variable of the station.
        /// </summary>
        public UFUAModel.TagEntityReference StateTag
        {
            get
            {
                return _StateTag;
            }
            set
            {
                _StateTag = value;
            }
        }

        private UFUAModel.TagEntityReference _CommandTag;
        /// <summary>
        /// Gets or sets the the Command variable of the station.
        /// </summary>
        public UFUAModel.TagEntityReference CommandTag
        {
            get
            {
                return _CommandTag;
            }
            set
            {
                _CommandTag = value;
            }
        }

        /// <summary>
        /// Gets the last statistic error.
        /// </summary>
        public String LastStatisticError
        {
            get
            {
                if (StatisticsData != null)
                {
                    return StatisticsData.GetStringValue(StatisticSetting.NodeDataNames.LastError.ToString());
                }
                return "";
            }
        }
        /// <summary>
        /// Gets the last statistic error time.
        /// </summary>
        public String LastStatisticErrorTime
        {
            get
            {
                if (StatisticsData != null)
                {
                    return StatisticsData.GetStringValue(StatisticSetting.NodeDataNames.LastErrorTime.ToString());
                }
                return "";
            }
        }

        /// <summary>   The last error time. </summary>
        private DateTime _StationLastErrorTime;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the last execution error time. </summary>
        ///
        /// <value> The last execution error time. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DateTime StationLastErrorTime
        {
            get { return _StationLastErrorTime; }
            set { _StationLastErrorTime = value; }
        }

        #endregion

        #region IStatistics Inrerface

        /// <summary>   Information describing the statistics. </summary>
        protected StatisticCounters StatisticsData;
		public StatisticCounters getStatisticsData()
        {
            return StatisticsData;
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Allow to know if the statistics are available for this object. </summary>
        ///
        /// <returns>   true if statistics are available; otherwise, false. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsStatisticsAvailable()
        {
            return StatisticsData != null;
        }

        /// <summary>   Initialize statistics. </summary>
        public void StartStatistics()
        {
            if (StatisticsData == null)
            {
                StatisticsData = new StatisticCounters();
                StatisticsData.ChangedCounter += StatisticsData_ChangedCounter;
                StatisticsData.StartWatch();
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Event handler. Called by StatisticsData for changed counter events. </summary>
        ///
        /// <param name="sender" type="object">             Source of the event. </param>
        /// <param name="e" type="ChangedCounterEventArgs"> Changed counter event information. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public void StatisticsData_ChangedCounter(object sender, ChangedCounterEventArgs e)
        {
                CommDriver.OnTagChanged(new NodeId(string.Format(
                    "{0}?{1}Statistics/Stations/{2}/{3}", CommDriver.RootDriversGuid, CommDriver.DriverName, Name, e.Key),
                    CommDriver.NamespaceIndex), new DataValue(e.newValue),DisableQualityUpdate);
        }

        /// <summary>   Terminate statistics and free the counters. </summary>
        public void TerminateStatistics()
        {
            if (StatisticsData != null)
                StatisticsData.Dispose();

            StatisticsData = null;
        }

        /// <summary>   Suspend statistics watcher. </summary>
        public void SuspendStatistics()
        {
            if (StatisticsData != null)
                StatisticsData.StopWatch();
        }

        /// <summary>   Resum statistics watcher. </summary>
        public void ResumeStatistics()
        {
            if (StatisticsData != null)
                StatisticsData.StartWatch();
        }

        /// <summary>   Reset whole statistic counters to zero. </summary>
        public void ResetStatistcs()
        {
            if (StatisticsData != null)
                StatisticsData.ResetStatistcs();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Allow to retrieve a copy of the current statistics dictionary. </summary>
        ///
        /// <param name="totaltimeon" type="out TimeSpan">  [out] out value with the elapsed total time
        ///                                                 with statistics enabled. </param>
        ///
        /// <returns>   An IDictionary. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IDictionary RetrieveStatisticCounters(out TimeSpan totaltimeon)
        {
            if (StatisticsData != null)
                return StatisticsData.RetrieveStatistcs(out totaltimeon);

            totaltimeon = new TimeSpan();
            return new Dictionary<string, long>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return the number of statistic values recorded. </summary>
        ///
        /// <returns>   The total counters. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int GetTotalCounters()
        {
            if (StatisticsData != null)
                return StatisticsData.GetTotalCounters();

            return 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return a list with the name of each statistic counter. </summary>
        ///
        /// <returns>   The list of counters name. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IList GetListOfCountersName()
        {
            if (StatisticsData != null)
                return StatisticsData.GetListOfCountersName();

            return new List<String>();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return the value of a counter. </summary>
        ///
        /// <param name="key">  name of a valid counter. </param>
        ///
        /// <returns>   The counter value. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public long GetCounterValue(string key)
        {
            if (StatisticsData != null)
                return StatisticsData.GetCounterValue(key);

            return 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return the elapsed total time with statistics enabled. </summary>
        ///
        /// <returns>   The total time on. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public TimeSpan GetTotalTimeOn()
        {
            if (StatisticsData != null)
                return StatisticsData.GetTotalTimeOn();

            return new TimeSpan();
        }

        /// <summary>   The statistic nodes. </summary>
        public static readonly StatisticSetting.NodeDataNames[] StatisticNodes = new StatisticSetting.NodeDataNames[]
        {
            StatisticSetting.NodeDataNames.TotalJobsRead,
            StatisticSetting.NodeDataNames.TotalJobsWrite,
            StatisticSetting.NodeDataNames.JobRate,
            StatisticSetting.NodeDataNames.TagRate,
            StatisticSetting.NodeDataNames.ByteRate,
            StatisticSetting.NodeDataNames.TotalTagsRead,
            StatisticSetting.NodeDataNames.TotalTagsWrite,
            StatisticSetting.NodeDataNames.TotalBytesRead,
            StatisticSetting.NodeDataNames.TotalBytesWrite,
            StatisticSetting.NodeDataNames.TotalRxBytes,
            StatisticSetting.NodeDataNames.TotalTxBytes,
            StatisticSetting.NodeDataNames.TotalJobsError,
            StatisticSetting.NodeDataNames.LastErrorTime,
            StatisticSetting.NodeDataNames.LastError,
            StatisticSetting.NodeDataNames.InErrorState,
        };

        #endregion

        #region StateCommandVariable
        object lockStCmdVar = new object();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set a bit of the State/Command variable of the channel. </summary>
        ///
        /// <param name="bitValue" type="bool">   The bit new value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SetStateCommandVariableBit(bool bitValue, UInt16 bitIndex)
        {
            lock(lockStCmdVar)
            {
                return stationStateCommandVariable.SetStateCommandVariableBit(bitValue, bitIndex, CommDriver);
            }            
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get a bit of the State/Command variable of the channel. </summary>
        ///
        /// <param name="bitValue" type="ref bool">   The bit value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetStateCommandVariableBit(ref bool bitValue, UInt16 bitIndex)
        {
            lock (lockStCmdVar)
            {
                return stationStateCommandVariable.GetStateCommandVariableBit(ref bitValue, bitIndex);
            }
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Check if a bit of the State/Command variable of the channel is equal to refered value. </summary>
        ///        
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        /// <param name="value" type="bool">        The value used to compare with command state bit value</param>
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsStateCommandVariableBit(UInt16 bitIndex, bool value)
        {
            bool bitValue = false;
            lock (lockStCmdVar)
            {
                return (stationStateCommandVariable.GetStateCommandVariableBit(ref bitValue, bitIndex) && (bitValue == value));
            }
        }

        #endregion

        #region IDisposable Interface

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual void Dispose()
        {
            if (StatisticsData != null)
            {
                StatisticsData.Dispose();
                StatisticsData.ChangedCounter -= StatisticsData_ChangedCounter;
                StatisticsData = null;
            }

        }

        #endregion

        
    }
}