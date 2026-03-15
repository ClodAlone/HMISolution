////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	Station.cs
//
// summary:	Implements the station class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Text.RegularExpressions;
using DevExpress.Xpo;
using Opc.Ua;
using DriverCodeBase.Enumerators;
using System.Collections;
using System.Globalization;
using DriverBaseInterfaces;
using DriverCodeBase.Helpers;
using System.Xml;

namespace DriverCodeBase
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
            _RewritingOfTheSameValue = settings.RewritingOfTheSameValue;
            if (_StateCommandTag != null && !NodeId.IsNull(_StateCommandTag.NodeId))
            {
                stationStateCommandVariable = new StateCommandVariable(_StateCommandTag.Name, _StateCommandTag.NodeId.ToString());
            }
        }

        /// <summary>   Specialized default constructor for use only by derived class. </summary>
        protected Station()
        {
            _InErrorState = false;
            CurRetriesBeforeError = 0;
            _FirstTime = true;
            stationStateCommandVariable = new StateCommandVariable();
            _RewritingOfTheSameValue = false;
        }

        #endregion

        #region Data Members

        /// <summary>   The communications driver. </summary>
        protected readonly CommunicationDriver CommDriver;

        /// <summary>   The channel. </summary>
        protected Channel Channel;
        /// <summary>   The channel base. </summary>
        protected IChannelBase ChannelBase;

        /// <summary>   The lock list object. </summary>
        protected Object lockListObject = new Object();
        /// <summary>   The list whole job. </summary>
        protected internal List<CommJob> ListWholeJob = new List<CommJob>();
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
        /// <summary>   The lock bool. </summary>
        protected object lockBool = new object();

        /// <summary>   The total partial jobs. </summary>
        protected long TotalPartialJobs = 0;
        /// <summary>   The total partial tags. </summary>
        protected long TotalPartialTags = 0;
        /// <summary>   The total partial in bytes. </summary>
        protected long TotalPartialBytes = 0;

        /// <summary>  The State/Command variable associated to the station. </summary>
        protected StateCommandVariable stationStateCommandVariable;
        /// <summary>  Flag that signal if a State/Command variable has been associated to the station. </summary>
        //bool stateCommandVariableHasBeenSet;
        protected readonly Dictionary<NodeId, List<CommJob>> ObservedTagToJobsMap = new Dictionary<NodeId, List<CommJob>>();
        
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
        public virtual uint OnWriteTag(NodeId tagnodeid, ref object value, ref StatusCode statusCode, ref DateTime timestamp)
        {
            //System.Diagnostics.Trace.TraceInformation("Station.OnWriteTag tag:{0} value:{1}", tagnodeid.ToString(), value.ToString());
            CommJob job;
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(tagnodeid))
                    return StatusCodes.BadNodeIdInvalid;

                job = mapTagJob[tagnodeid];

                if (job.Type == LinkType.Input)
                    return (StatusCodes.BadNotWritable);

                //check if the tag must be write 
                if ((RewritingOfTheSameValue == false) &&
                    (job.Type == LinkType.ExceptionOutput || job.Type == LinkType.InputOutput))
                {
                    DataValue objTmp = new DataValue();
                    objTmp.Value = Utils.Clone(value);
                    int nTagsToWrite = (from t in job.TagsList
                                        where (t.TagNode.NodeId == tagnodeid) &&
                                               (!StatusCode.IsGood(t.Value.StatusCode) ||
                                               (t.LastValue == null) ||
                                               (!t.LastValue.Equals(objTmp.Value)))
                                        select t).Count();
                    if (nTagsToWrite == 0)
                    {
                        job.RemoveTagFromTagListToWrite(tagnodeid);

                        return (StatusCodes.Good);
                    }
                }
            }

            uint ret = StatusCodes.BadConfigurationError;
            if (ChannelBase != null) 
            {
                ret = job.OnWriteTag(tagnodeid, ref value);
                
                if (!CommDriver.WriteAsync && job.IsConditionalVariableOn())
                {
                    List<object> outputvalues = new List<object>();
                    outputvalues.Add(new uint());
                    
                    ret = ExecuteSyncroJob(job, false, 0, job.Type, outputvalues, false);
                    job.ResetSynchro.Set(); // channel working thread can continue to process other jobs
                    job.ClearTagListWrite();
                }
                else if (ret == StatusCodes.Good)
                {
                    // for job in PollingInError state, wait "normal" scheduling (to avoid station state var unstable value)
                    if (PromoteJobToWrite(job))
                        ChannelBase.ChangeStateJob(job, CommJobState.PollingNow);
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
                nodeIDs.Add(stationStateCommandVariable.varNodeId);
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
            if ((stationStateCommandVariable.hasBeenSet == true) && (node == stationStateCommandVariable.varNodeId))
            {
                ManageUpdatedValueForTheStateCommandVariable(value);
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
                            job.ManageUpdatedValueForTheConditionalVariable(value);
                        else if (job.OffsetVariableSet && node.ToString() == job.OffsetVariableId)
                            job.ManageUpdatedValueForTheOffsetVariable(value);
                    }
                }
            }
        }

        public void ManageUpdatedValueForTheStateCommandVariable(DataValue value)
        {
            bool suspendBitSavedValue = false;
            bool suspendBitCanBeManaged = false;
            if (stationStateCommandVariable.GetStateCommandVariableBit(ref suspendBitSavedValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
            {
                suspendBitCanBeManaged = true;
            }

            stationStateCommandVariable.varValue = value;

            if (suspendBitCanBeManaged == true)
            {
                bool suspendBitNewValue = false;
                if (stationStateCommandVariable.GetStateCommandVariableBit(ref suspendBitNewValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
                {
                    if (suspendBitSavedValue != suspendBitNewValue)
                    {
                        if (suspendBitNewValue == true)
                        {
                            bool returnValue = SuspendStation();
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
        public virtual uint OnUpdateTag(NodeId tagnodeid, ref object value, ref StatusCode statusCode, ref DateTime timestamp)
        {
            //System.Diagnostics.Trace.TraceInformation("Station.OnWriteTag tag:{0} value:{1}", tagnodeid.ToString(), value.ToString());
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
                ServerTimestamp = DateTime.UtcNow,
                SourceTimestamp = DateTime.MinValue,
                StatusCode = StatusCodes.Good
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

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sets in use. </summary>
        ///
        /// <param name="tagnodeid" type="NodeId">          The tagnodeid. </param>
        /// <param name="bInUse" type="bool">               true to in use. </param>
        /// <param name="samplinginterval" type="double">   (Optional) the samplinginterval. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool SetInUse(NodeId tagnodeid, bool bInUse, double samplinginterval = -1)
        {
            CommJob job = null;
            NodeId n = tagnodeid;
            lock (lockListObject)
            {                
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
                            return false;
                    }
                    else
                        return false;
                }

                job = mapTagJob[n];
            }
            bool ret = false;
            if (job != null)
            {
                var inuse = job.InUse;
                ret = job.SetInUse(n, bInUse, samplinginterval);

                if (ChannelBase != null && job.InUse != inuse)
                {
                    if (job.InUse)
                        ChannelBase.ChangeStateJob(job, CommJobState.PollingInUse);
                    else
                        ChannelBase.ChangeStateJob(job, CommJobState.PollingNotInUse);
                }
            }

            return ret;
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
            if (stationStateCommandVariable.GetStateCommandVariableBit(ref suspendBitNewValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
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
                lock (job.retLockList())
                {
                    job.IsPending = false;
                    // If the list TagsListOnWriting is not empty, the job will no longer be scheduled, so empty it
                    job.TagsListOnWriting.Clear();
                }
            }

            if (ChannelBase != null)
            {
                ChannelBase.JobExecuted += OnJobExecuted;
                foreach (var job in listJob)
                {

                    List<Tag> tl = (from t in job.TagsList/*.AsParallel()*/
                                    where (t.DynSettings.MethodID != -1)
                                    select t).ToList();
                    if (tl.Count != job.TagsList.Count)
                        ChannelBase.SubscribeJob(job, (job.InUse ? CommJobState.PollingInUse : CommJobState.PollingNotInUse));
                }
            }
            //System.Diagnostics.Debug.WriteLine("_-- DEBUG -- {0} ManageUpdatedValueForTheStateCommandVariable Startup Name {1}", DateTime.Now.ToString("HH:MM:ss.fff"),Name);
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Suspends this object. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool Suspend()
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }

            if (ChannelBase != null)
            {
                foreach (var job in listJob)
                {
                    ChannelBase.UnsubscribeJob(job);
                    lock (job.retLockList())
                        job.IsPending = false;
                }
                ChannelBase.JobExecuted -= OnJobExecuted;
                ChannelBase.Suspend();
            }

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Suspends a station without suspending the corresponding channel. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public virtual bool SuspendStation()
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }

            if (ChannelBase != null)
            {
                foreach (var job in listJob)
                {
                    job.SetUncertainLastUsableValueQuality();
                    ChannelBase.UnsubscribeJob(job);
                    lock (job.retLockList())
                        job.IsPending = false;
                }
                ChannelBase.JobExecuted -= OnJobExecuted;
                //ChannelBase.Suspend();
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
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }

            if (ChannelBase != null)
            {
                foreach (var job in listJob)
                    ChannelBase.UnsubscribeJob(job);
                ChannelBase.JobExecuted -= OnJobExecuted;
            }

            return true;
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
                    if (tag.TagNode.DataType.IdType == IdType.Guid && CommDriver.IsPrototypeSplitEnabled())
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
                                    //dtCalc.TryParse(dynsettings);
                                    dynsettings = dtCalc.GetNextDynSetting(pList[i - 1], dtag);
                                    //dynsettings = dtCalc.GetNextDynSetting(pList[i - 1]);
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
                                if(t.bIsValid)
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
                        // if job creation failed, tag's DynamicSettings is not valid --> use job info to populate tag error
                        if (!candJob.IsValid)
                        {
                            candTag.bIsValid = false;
                            candTag.InvalidReason = candJob.InvalidReason;
                        }
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
                                        AddToMapTagJob(a.TagNode.NodeId, testJob);

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

            GetCommDriver().SmartThreadPool.QueueWorkItem(() =>
            {
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
                    if (_InErrorState)
                        StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), true);
                    else
                        StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), false);
                }
                //System.Diagnostics.Debug.WriteLine("-- DEBUG -- OnJobExecuted IsPending = false {0}", e.Job.TagsList[0].TagNode.NodeId);
                e.Job.IsPending = false;
            });
        }

        /// <summary>   Executes the refresh diagnostic action. </summary>
        public void OnRefreshDiagnostic()
        {
            if (StatisticsData != null)
            {

                StatisticsData.Update(StatisticSetting.NodeDataNames.JobRate.ToString(), TotalPartialJobs);
                StatisticsData.Update(StatisticSetting.NodeDataNames.TagRate.ToString(), TotalPartialTags);
                StatisticsData.Update(StatisticSetting.NodeDataNames.ByteRate.ToString(), TotalPartialBytes);
                TotalPartialJobs = 0;
                TotalPartialTags = 0;
                TotalPartialBytes = 0;
                if (_InErrorState)
                    StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), true);
                else
                    StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), false);

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

            bool bAllJobsInError = e.GeneralError;
            bool inInErrorState = e.Job.InErrorState;

            if (e.ErrorCode != DriverErrorCodes.ErrorNoError && bAllJobsInError)
            {
                //all jobs in error
                List<CommJob> erlist = new List<CommJob>();
                lock (lockListObject)
                {
                    erlist.AddRange(ListWholeJob);
                }
                foreach (var j in erlist)
                {
                    /*When a general error occurs, the jobs are put in error, it is necessary 
                     * to distinguish the jobs that have conditional variables or jobs excepionOutput, 
                     * because at the resumption of communication (eg Timeout), 
                     * jobs of type exceptionoutput and jobs with conditional variables, 
                     * would keep in error the station until their execution.*/
                    if ((e.Job == j) || ((j.Type != LinkType.ExceptionOutput) && !j.ConditionalVariableSet))
                        j.SetErrorState((int)e.ErrorCode);
                }
            }
            else if ((e.ChangedTags == null) || (e.ChangedTags.Count == 0))
            {
                e.Job.SetErrorState((int)e.ErrorCode);
            }

            if (inInErrorState && !e.Job.InErrorState)
                Channel.QuickPollingJobInError();

            if ((e.ErrorCode != DriverErrorCodes.ErrorNoError) || (bAllJobsInError == true))
            {
                stationStateCommandVariable.SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState, GetCommDriver());
            }
            else
            {
                if (Channel.InErrorJobs(this) == 0)
                {
                    stationStateCommandVariable.SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState, GetCommDriver());
                }
                e.Job.ResetConditionalVariable();
            }

            if(e.ChangedTags.Count > 0 )
            {
                uint quality;
                string error;
                GetCommDriver().GetDriverErrorInfo((int)e.ErrorCode, out quality, out error);
                foreach (var tag in e.ChangedTags)
                {
                    tag.Value.StatusCode = quality;
                    GetCommDriver().OnTagChanged(tag.TagNode.NodeId, tag.Value, e.Timestamp);
                }
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
            if (systemType == typeof(string)) { return BuiltInType.String; }
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
                    //case BuiltInType.String:
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

                    List<Tag> tl = (from t in job.TagsList/*.AsParallel()*/
                                    where (t.TagNode.NodeId != node && t.DynSettings.MethodID == -1)
                                    select t).ToList();
                    bool localMethod = (tl.Count > 0);

                    if (tag == null)
                        return StatusCodes.BadMethodInvalid;
                    if (!Channel.ThreadRunning())
                        Channel.Startup();

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

                    uint resexec = ExecuteSyncroJob(job, localMethod, tag.DynSettings.MethodID, lt, outputArguments);
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
                    job.ResetSynchro.Set();
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
        public virtual uint ExecuteSyncroJob(CommJob job, bool localmethod, int methodid, LinkType lt, IList<object> args, bool takedata = true)
        {
            if (!Channel.ThreadRunning())
                Channel.Startup();

            job.EndSynchroExec.Reset();
            DateTime dt1 = DateTime.UtcNow;
            while (Channel.SynchroJob != null)
            {
                job.EndSynchroExec.WaitOne(0);
                //check timeout
                if ((DateTime.UtcNow - dt1).TotalMilliseconds > Channel.Timeout)
                {
                    return StatusCodes.BadTimeout;
                }
            }
            job.LocalMethod = localmethod;
            job.SyncroExec = true;
            job.MethodID = methodid;
            Channel.SynchroJob = job;
            if (job.EndSynchroExec.WaitOne(Channel.Timeout/*rest of the timeout*/))
            {

                job.EndSynchroExec.Reset();
                args[0] = job.SynchroError;//DriverErrorCodes.ErrorNoError;

                //prendi i dati o il risultato  dell'esecuzione e ritorna
                if (takedata && job.SynchroError == (int)DriverErrorCodes.ErrorNoError &&
                    localmethod == false && methodid == (int)DriverSynchroOperation.ReadSynchro)
                {
                    byte[] recval = (byte[])job.SynchroValues;
                    List<object> outarg = (List<object>)args;
                    ParseReceivedToArguments(recval, job, ref outarg);
                }
            }
            else
            {
                args[0] = DriverErrorCodes.ErrorTimeOut;
                Channel.SynchroJob = null;
            }
            return StatusCodes.Good;

        }


        protected virtual bool PromoteJobToWrite(CommJob job)
        {
            if (ChannelBase != null)
            {
                return !Channel.IsJobInPollingInErrorState(job);
            }
            return false;
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
            lock (lockListObject)
            {
                if (!ListWholeJob.Contains(entity))
                {
                    ListWholeJob.Add(entity);

                    List<CommJob> tmpList;
                    if (mapGroupedJobs.ContainsKey(entity.GroupString))
                        tmpList = mapGroupedJobs[entity.GroupString];
                    else
                        tmpList = new List<CommJob>();

                    if (!tmpList.Contains(entity))
                        tmpList.Add(entity);

                    mapGroupedJobs[entity.GroupString] = tmpList;


                    foreach (var tag in entity.TagsList)
                    {
                        if (!mapTagJob.ContainsKey(tag.TagNode.NodeId))
                            mapTagJob.Add(tag.TagNode.NodeId, entity);
                    }
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
                    if (!ListWholeJob.Contains(entity))
                        ListWholeJob.Add(entity);
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
        #endregion

        #region Private Methods
        public string AdditionalError;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Process the job lists. </summary>
        ///
        /// <param name="job" type="CommJob">               The job. </param>
        /// <param name="error" type="DriverErrorCodes">    The error. </param>
        /// <param name="bCheckNumRetries" type="bool">     (Optional) true to check number retries. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        protected virtual void ProcessJobLists(CommJob job, DriverErrorCodes error, bool bCheckNumRetries = true)
        {
            //lock (lockListObject)
            {
                if (error != DriverErrorCodes.ErrorNoError)
                {
                    System.Diagnostics.Trace.TraceInformation("Station {0} Errors:{1} - Channel {2} Errors:{3}", Name, InErrorState, Channel.Name, Channel.ConsecutiveCommErrors);
                    if (_LastErrorCode == DriverErrorCodes.ErrorNoError)
                    {
                        _LastErrorTime = DateTime.UtcNow;
                        _LastErrorCode = error;
                        AdditionalError = (job.TagsList.Count > 0 ? job.TagsList[0].DynSettings.ToString() : null);
                    }
                    lock (lockBool)
                    {
                        if (!InErrorState)
                        {
                            if (!bCheckNumRetries || ++CurRetriesBeforeError >= MaxRetriesBeforeError || job.TagsListOnWriting.Count != 0)
                            {
                                if (isStationError(error))
                                {
                                    var listJob = new List<CommJob>();
                                    lock (lockListObject)
                                    {
                                        foreach (var commjob in ListWholeJob)
                                            if (commjob.InUse)
                                                listJob.Add(commjob);
                                    }

                                    foreach (var commjob in listJob)
                                    {
                                        ChannelBase.ChangeStateJob(commjob, CommJobState.PollingNotInUse);
                                    }

                                    Channel.RemoveStationJobsOnQueue(this);
                                    
                                    foreach (CommJob commjob in listJob)
                                    {
                                        if(IsCompatibleWithErrorList(commjob))
                                        {
                                            /*When a general error occurs, the jobs are put in error it is necessary 
                                             * to distinguish the jobs that have conditional variables or jobs excepionOutput, 
                                             * because at the resumption of communication (eg Timeout), 
                                             * jobs of type exceptionoutput and jobs with conditional variables, 
                                             * would keep in error the station until their execution.*/
                                            if ((commjob == job) || ((commjob.Type != LinkType.ExceptionOutput) && !commjob.ConditionalVariableSet))
                                                commjob.SetErrorState((int)error);
                                            Thread.Sleep(1);
                                            ChannelBase.ChangeStateJob(commjob, CommJobState.PollingInError);
                                        }
                                    }
                                }
                                else
                                {
                                    job.SetErrorState((int)error);
                                    ChannelBase.ChangeStateJob(job, CommJobState.PollingInError);
                                }
                                CurRetriesBeforeError = 0;
                                InErrorState = true;
                                Channel.ConsecutiveCommErrors++;
                            }
                        }


                        if (InErrorState)
                        {
                            if (ChannelBase != null)
                            {
                                if (!isStationError(error) && job != null)
                                {
                                    ChannelBase.ChangeStateJob(job, CommJobState.PollingInError);
                                }

                                if (SuspendJobInError)
                                {
                                    var listJob = new List<CommJob>();
                                    lock (lockListObject)
                                    {
                                        listJob.AddRange(ListWholeJob);
                                    }

                                    foreach (var commjob in listJob)
                                        ChannelBase.UnsubscribeJob(commjob);
                                }
                            }
                        }
                    }

                }
                else if ((job != null && job.InErrorState) || InErrorState)
                {
            
                    //var listJob = Channel.GetJobStateList(CommJobState.PollingInError);
                    List<CommJob> listJob = Channel.GetJobStateList(CommJobState.PollingInError).Where(j => j.Station == this).AsParallel().ToList();
                                       
                    bool resetError = true;
                    foreach (var commjob in listJob)
                    {
                        if (commjob.InErrorState && (commjob != job))
                            resetError = false;
                    }
                    if (resetError)
                    {
                        Channel.ConsecutiveCommErrors = 0;
                        System.Diagnostics.Trace.TraceInformation("Station {0} ResetErrors - Channel {1} ResetErrors", Name, Channel.Name);
                        InErrorState = false;

                        _LastErrorTime = DateTime.MinValue;
                        _LastErrorCode = DriverErrorCodes.ErrorNoError;
                    }
                    
                    if (ChannelBase != null)
                    {
                        /*
                        if (job != null && job.InUse)
                            ChannelBase.ChangeStateJob(job, CommJobState.PollingInUse);
                        else if (job != null)
                            ChannelBase.ChangeStateJob(job, CommJobState.PollingNotInUse);
                         */

                        //var listJob = new List<CommJob>();
                        if (SuspendJobInError)
                        {
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
                                    ChannelBase.ChangeStateJob(job, CommJobState.PollingNotInUse);
                            }
                        }
                    }
                }

                lock (lockBool)
                {
                    if (job != null && !job.InErrorState && FirstTime)
                    {
                        FirstTime = false;
                        CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.StationResumeError, CommDriver.DriverName/*DriverInfo.GetDriverName()*/, Name), EventSeverity.Low);
                    }
                }
                lock (job.retLockList())
                {
                    if (job != null && job.TagsListOnWriting.Count > 0)
                    {
                        // FOGBUGZ 11531
                        if (job.Type == LinkType.InputOutput)
                        {
                            foreach (var tag in job.TagsListOnWriting)
                            {
                                if (error == DriverErrorCodes.ErrorNoError)
                                {
                                    tag.SetInternalValues(tag.Value.Value);
                                }
                                tag.SetReadValue(tag.Value.Value);
                            }
                        }
                        else if (error != DriverErrorCodes.ErrorNoError &&
                            (job.Type == LinkType.UnconditionalOutput || job.Type == LinkType.ExceptionOutput))
                        {
                            foreach (var tag in job.TagsListOnWriting)
                            {
                                if (!job.TagsListToWrite.Contains(tag))
                                {
                                    job.TagsListToWrite.Add(tag);
                                }
                            }
                        }

                        job.TagsListOnWriting.Clear();
                        if (job.TagsListToWrite.Count == 0)
                        {
                            ChannelBase.ChangeStateJob(job, (job.InUse ? CommJobState.PollingInUse : CommJobState.PollingNotInUse));
                        }
                    }
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
                case DriverErrorCodes.ErrorTimeOut:
                    return true;
            }
            return false;
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
                        CommDriver.OnSystemEvent(/*ObjectIds.Server*/null, String.Format(Properties.Resources.StationResumeError, CommDriver.DriverName/*DriverInfo.GetDriverName()*/, Name), EventSeverity.Low);
                        CommDriver.OnStateChanged(ComunicationState.Running);
                    }
                }
            }
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
                    CommDriver.NamespaceIndex), new DataValue(e.newValue));
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
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set a bit of the State/Command variable of the channel. </summary>
        ///
        /// <param name="bitValue" type="bool">   The bit new value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SetStateCommandVariableBit(bool bitValue, UInt16 bitIndex)
        {
            return stationStateCommandVariable.SetStateCommandVariableBit(bitValue, bitIndex, GetCommDriver());
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get a bit of the State/Command variable of the channel. </summary>
        ///
        /// <param name="bitValue" type="ref bool">   The bit value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetStateCommandVariableBit(ref bool bitValue, UInt16 bitIndex)
        {
            return stationStateCommandVariable.GetStateCommandVariableBit(ref bitValue, bitIndex);
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