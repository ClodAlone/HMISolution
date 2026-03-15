using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using DriverCodeBaseEx.Helpers;
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
                uint returnValue = MQTTClientProtocol.ParseData(Answer, ref mJ, ref ChangedTags);
                if (returnValue == StatusCodes.Good)
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
                        e.ErrorCode = (DriverErrorCodes)returnValue;
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
            MQTTClientCommJob mj = job as MQTTClientCommJob;
            string Answer = string.Empty;
            try
            {
                if (receivedbuffer != null)
                {
                    Answer = new String(Encoding.UTF8.GetChars(receivedbuffer));
                }
            }
            catch(Exception ex)
            {
                return (false);
            }

            return (MQTTClientProtocol.ParseData(Answer, ref mj, ref arguments) == StatusCodes.Good);
        }

        public override void ParseDynamicTagsStructSplit(Tag tag, List<Tag> innerList, List<Tag> lMethods)
        {                    
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
        }

        public override uint OnWriteTag(NodeId tagnodeid, ref object value, ref StatusCode statusCode, ref DateTime timestamp)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - MQTTClientStation.OnWriteTag called",
                                                   currentTime));
            }
#endif
            CommJob job;
            lock (lockListObject)
            {
                if (!mapTagJob.ContainsKey(tagnodeid))
                {
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - MQTTClientStation.OnWriteTag returning StatusCodes.BadNodeIdInvalid",
                                                           currentTime));
                    }
#endif
                    return StatusCodes.BadNodeIdInvalid;
                }

                job = mapTagJob[tagnodeid];

                //check if the tag must be written 
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

#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - MQTTClientStation.OnWriteTag nothing to write",
                                                               currentTime));
                        }
#endif

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
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - MQTTClientStation.OnWriteTag - Write operation discarded due to hysteresis",
                                                           currentTime));
                        }
#endif
                        return (StatusCodes.Good);
                    }
                }
            }

            uint ret = StatusCodes.BadConfigurationError;
            if (ChannelBase != null)
            {
                // Synchronous write
                if (!CommDriver.WriteAsync && job.IsConditionalVariableOn())
                {
                    List<object> outputvalues = new List<object>();
                    outputvalues.Add(new uint());

#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - MQTTClientStation.OnWriteTag - calling ExecuteSyncroJob",
                                                                         currentTime));
                    }
#endif
                    ret = ExecuteSyncroJob(job, false, 0, outputvalues, false, tagnodeid, value);
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - MQTTClientStation.OnWriteTag - ExecuteSyncroJob returned: {1}",
                                                       currentTime, ret));
                    }
#endif
                    if (ret == StatusCodes.Good)
                        ret = (Convert.ToInt32(outputvalues[0]) != 0 ? StatusCodes.Bad : StatusCodes.Good);
                    int errorcode = (int)outputvalues[0];
                    string error;
                    uint quality;
                    GetCommDriver().GetDriverErrorInfo(errorcode, out quality, out error);
                    statusCode = (StatusCode)quality;
                    timestamp = DateTime.UtcNow;
                    job.ClearTagListWrite();
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - MQTTClientStation.OnWriteTag - Case ExecuteSyncroJob - Returning value: {1}",
                                                       currentTime, ret));
                    }
#endif
                }
                else
                {
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - MQTTClientStation.OnWriteTag calling job.OnWriteTag",
                                                       currentTime));
                    }
#endif
                    ret = job.OnWriteTag(tagnodeid, ref value);
                    if (ret == StatusCodes.Good)
                    {
                        // for job in PollingInError state, wait "normal" scheduling (to avoid station state var unstable value)
                        if(!job.InErrorState || (job.Type == LinkType.ExceptionOutput) || (job.Type == LinkType.UnconditionalOutput))
                        {
#if DEBUG
                            {
                                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                System.Diagnostics.Debug.WriteLine(String.Format("MQTTClient DBG - {0} - MQTTClientStation.OnWriteTag calling ChangeStateJob(job, CommJobState.PollingNow)",
                                                               currentTime));
                            }
#endif
                            ChannelBase.ChangeStateJob(job, CommJobState.PollingNow);
                        }
                    }
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
            //lock (lockBool)
            //{
                if (LastErrorCode == DriverErrorCodes.ErrorNoError)
                {
                    var listJob = new List<CommJob>();
                    lock (lockListObject)
                    {
                        listJob.AddRange(ListWholeJob);
                    }                    
                    listJob = (from j in listJob.AsParallel()
                               where j.InUse && !j.InErrorState
                               select j).ToList();

                    foreach (var commjob in listJob)
                    {
                        if (((commjob.Type != LinkType.ExceptionOutput) && !commjob.ConditionalVariableSet))
                            commjob.SetErrorState((int)MQTTClientErrorCodes.ErrorCodeErrorConnectionBroken, false);
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
            //}
        }

        //public void ManageConnectionBroken()
        //{
        //    if (ListWholeJob.Count > 0)
        //    {
        //        var listJob = new List<CommJob>();
        //        lock (lockListObject)
        //        {
        //            listJob.AddRange(ListWholeJob);
        //        }
        //        foreach (var job in listJob)
        //        {
        //            // Deactivate the job
        //            MQTTClientCommJob pJob = (MQTTClientCommJob)job;
        //            pJob.Status = MQTTClientCommJobStatus.Idle;
        //        }

        //        ExecutedJobArgs e = new ExecutedJobArgs();
        //        e.Job = listJob[0];
        //        e.ErrorCode = (DriverErrorCodes)(MQTTClientErrorCodes.ErrorCodeErrorConnectionBroken);
        //        // Put all the jobs in error
        //        e.GeneralError = true;

        //        if (Channel != null)
        //        {
        //            Channel.SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
        //        }

        //        base.ProcessJobValues(e);
        //    }
        //}

        public long NumberOfJobsInError()
        {
            return (InErrorJobs());
        }

        //public object getLockBool()
        //{
        //    return lockBool;
        //}
        public DriverCodeBaseEx.IChannelBase GetChannelBase()
        {
            return ChannelBase;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sort the list of tags for dynamic settings. </summary>
        ///
        /// <param name="tags"> . </param>
        ///
        /// <returns>   The sorted tags. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override List<Tag> SortTags(IList<Tag> tags)
        {
            return tags.ToList();
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
