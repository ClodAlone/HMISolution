using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using DriverCodeBase.Helpers;
using Opc.Ua;
using DriverBaseInterfaces;

namespace MQTTClient
{
    #region enums
    public enum MQTTClientMessageFormats : byte
    {
        XML,
        JSON,
        RAW
    }
    #endregion

    class MQTTClientStation : Station
    {
                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public MQTTClientStation(CommunicationDriver commdriver, MQTTClientStationSettings settings)
            : base(commdriver, settings)
        {
            _MQTTClientMessageFormat = settings.MQTTClientMessageFormat;
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as MQTTClientCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new MQTTClientCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as MQTTClientTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new MQTTClientCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new MQTTClientTag(td);
        }
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as MQTTClientCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new MQTTClientCommJobSettings(session, commJob);
        }

        #endregion

        #region Override Methods

        public override bool Startup()
        {
            // Call the base class method
            if (!base.Startup())
            {
                return false;
            }

            // Add input jobs to the dictionary Topic-Jobs
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }
            List<CommJob> inputJobList = (from j in listJob.AsParallel()
                                          where ((j.Type == LinkType.Input) || (j.Type == LinkType.InputOutput))
                                          select j).ToList();
            MQTTClientChannel mqttChannel = (MQTTClientChannel)Channel;
            foreach (var job in inputJobList)
            {
                MQTTClientCommJob MQTTClientJob = (MQTTClientCommJob)job;
                mqttChannel.AddJobToTopicJobsDictionary(MQTTClientJob);
            }

            return true;
        }

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            MQTTClientCommJob mJ = e.Job as MQTTClientCommJob;
            if (mJ == null)
                return;

            // Analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                byte[] AnswerBuffer = (byte[])e.Values;
                string Answer = string.Empty;
                // e.values could be null for output jobs
                if(AnswerBuffer != null)
                {
                    Answer = new String(Encoding.UTF8.GetChars(AnswerBuffer));
                }
                List<object> ChangedTags = new List<object>();
                if (MQTTClientProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                {
                    foreach (var tag in ChangedTags)
                    {
                        var j = tag as Tag;
                        if (j != null)
                        {
                            e.ChangedTags.Add(j);
                            e.Timestamp = j.Value.SourceTimestamp;
                        }
                    }
                }
                else
                {
                    if (ChangedTags.Count > 0)
                    {
                        e.ErrorCode = (DriverErrorCodes)((MQTTClientErrorCodes)ChangedTags[0]);
                    }
                    else
                    {
                        //e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                        return;
                    }
                }
            }

            mJ.Status = MQTTClientCommJobStatus.Idle;

            // Put all the jobs in error?
            e.GeneralError = ((e.ErrorCode == DriverErrorCodes.ErrorTimeOut) ||
                              (e.ErrorCode == (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorSubscription));

            if (Channel != null)
            {
                if (e.ErrorCode != DriverErrorCodes.ErrorNoError)
                {
                    Channel.SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
                }
                else
                {
                    Channel.SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.ChannelUnconnected);
                }
            }

            base.ProcessJobValues(e);
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Parse dynamic tags. </summary>
        ///
        /// <param name="tags" type="IList<Tag>">   The tags. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
                    tag.ByteOffset = 0;
                    tag.BitOffset = 0;
                    innerList.Clear();
                    lMethods.Clear();
                    if (tag.TagNode.DataType.IdType == IdType.Guid && CommDriver.IsPrototypeSplitEnabled() && (MQTTClientMessageFormat == MQTTClientMessageFormats.XML))
                    {
                        //struttura
                        List<TagDefinition> pList = new List<TagDefinition>();
                        GetPrototypeTagList(tag.TagNode.NodeId, ref pList);
                        string dynsettings = string.Empty;
                        MQTTClientDynTagSettings dtCalc = (MQTTClientDynTagSettings)tag.DynSettings;
                        TagDefinition olddtag;
                        for (int i = 0; i < pList.Count; i++)
                        {

                            TagDefinition dtag = pList[i];
                            olddtag = dtag;
                            if (dtag.DynamicSettings.Length == 0)
                            {
                                if (dynsettings.Length == 0)
                                {
                                    dynsettings = dtCalc.GetFirstDynSetting(tag, dtag, _MQTTClientMessageFormat);
                                }
                                else
                                {
                                    //dtCalc.TryParse(dynsettings);
                                    dynsettings = dtCalc.GetNextDynSetting(pList[i - 1], dtag, _MQTTClientMessageFormat);
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
                        // if job creation failed, tag's DynamicSettings is not valid --> use job info to populate tag error
                        if (!candJob.IsValid)
                        {
                            candTag.bIsValid = false;
                            candTag.InvalidReason = candJob.InvalidReason;
                        }
                        
                        if (!candJob.IsValid || mapTagJob.ContainsKey(candTag.TagNode.NodeId))
                            continue;

                        tags.Add(candTag);

                        List<CommJob> tmpList;
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

        public override uint OnWriteTag(NodeId tagnodeid, ref object value, ref StatusCode statusCode, ref DateTime timestamp, bool ignoreWriteAsync = false, bool forceSynchWrite = false)
        {
            CommJob job;
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(tagnodeid))
                    return StatusCodes.BadNodeIdInvalid;

                job = mapTagJob[tagnodeid];

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

                // Driver specific: for jobs of type ExceptionOutput check also the Hysteresis value
                if (job.Type == LinkType.ExceptionOutput)
                {
                    MQTTClientCommJob mqttJob = (MQTTClientCommJob)job;
                    if(!mqttJob.CheckHysteresis(ref value))
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

                if (((!ignoreWriteAsync && !CommDriver.WriteAsync) || forceSynchWrite) && job.IsConditionalVariableOn())
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
                    if (!Channel.IsJobInPollingInErrorState(job))
                        ChannelBase.ChangeStateJob(job, CommJobState.PollingNow);
                }
            }
            return ret;
        }
        #endregion

        #region Specific Methods

        public void ManageConnectionRestored()
        {
            LastErrorCode = DriverErrorCodes.ErrorNoError;
        }

        public void SetConnectionError()
        {
            lock (lockBool)
            {
                if (LastErrorCode == DriverErrorCodes.ErrorNoError)
                {
                    var listJob = new List<CommJob>();
                    lock (lockListObject)
                    {
                        listJob.AddRange(ListWholeJob);
                    }
                    foreach (var commjob in listJob)
                    {
                        commjob.SetErrorState((int)MQTTClientErrorCodes.ErrorCodeErrorConnectionBroken);
                    }
                    SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);
                    LastErrorCode = (DriverErrorCodes)MQTTClientErrorCodes.ErrorCodeErrorConnectionBroken;
                    InErrorState = true;
                }
                if (StatisticsData != null)
                {
                    StatisticsData.Update(StatisticSetting.NodeDataNames.LastErrorTime.ToString(), DateTime.Now.ToString());
                    uint quality;
                    String DiagnErrorMessage = String.Empty;
                    CommDriver.GetDriverErrorInfo((int)MQTTClientErrorCodes.ErrorCodeErrorConnectionBroken, out quality, out DiagnErrorMessage);
                    StatisticsData.Update(StatisticSetting.NodeDataNames.LastError.ToString(), DiagnErrorMessage);
                    StatisticsData.Update(StatisticSetting.NodeDataNames.InErrorState.ToString(), true);
                }
            }
        }

        public void ManageConnectionBroken()
        {
            if (ListWholeJob.Count > 0)
            {
                var listJob = new List<CommJob>();
                lock (lockListObject)
                {
                    listJob.AddRange(ListWholeJob);
                }
                foreach (var job in listJob)
                {
                    // Deactivate the job
                    MQTTClientCommJob pJob = (MQTTClientCommJob)job;
                    pJob.Status = MQTTClientCommJobStatus.Idle;
                }

                ExecutedJobArgs e = new ExecutedJobArgs();
                e.Job = listJob[0];
                e.ErrorCode = (DriverErrorCodes)(MQTTClientErrorCodes.ErrorCodeErrorConnectionBroken);
                // Put all the jobs in error
                e.GeneralError = true;

                if (Channel != null)
                {
                    Channel.SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
                }

                base.ProcessJobValues(e);
            }
        }

        public object getLockBool()
        {
            return lockBool;
        }
        public DriverCodeBase.IChannelBase GetChannelBase()
        {
            return ChannelBase;
        }

        #endregion

        #region Properties
        private MQTTClientMessageFormats _MQTTClientMessageFormat;
        public MQTTClientMessageFormats MQTTClientMessageFormat
        {
            get { return _MQTTClientMessageFormat; }
            set { _MQTTClientMessageFormat = value; }
        }

        #endregion

    }
}
