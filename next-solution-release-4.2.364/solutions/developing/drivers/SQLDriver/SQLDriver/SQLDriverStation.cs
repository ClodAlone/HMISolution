using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using DriverBaseInterfaces;
using DriverCodeBase.Helpers;

namespace SQLDriver
{
    public class SQLDriverStation : Station
    {
                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public SQLDriverStation(CommunicationDriver commdriver, SQLDriverStationSettings settings)
            : base(commdriver, settings)
        {
            _SQLDriverTableName = settings.SQLDriverTableName;
            _SQLDriverColumnNameTagName = settings.SQLDriverColumnNameTagName;
            _SQLDriverColumnValue = settings.SQLDriverColumnValue;
            _SQLDriverMultiColumn = settings.SQLDriverMultiColumn;
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as SQLDriverCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new SQLDriverCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as SQLDriverTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new SQLDriverCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new SQLDriverTag(td);
        }
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as SQLDriverCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new SQLDriverCommJobSettings(session, commJob);
        }

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            SQLDriverCommJob mJ = e.Job as SQLDriverCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();
                if (Answer != null)
                {
                    if (SQLDriverProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                        foreach (var tag in ChangedTags)
                        {
                            var j = tag as Tag;
                            if (j != null)
                                e.ChangedTags.Add(j);
                        }
                    //e.ChangedTags.AddRange(ChangedTags);
                    else
                        e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                }
                SetStateCommandVariableBit(false, (UInt16)StationVariableBits.StationErrorState);
            }
            else
                SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut) || (int)e.ErrorCode == (int)SQLDriverErrorCodes.ErrorConnectionToDevice;
            

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

            base.ProcessJobValues(e);
        }

        #endregion

        #region Override Methods

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            SQLDriverCommJob mj = job as SQLDriverCommJob;

            SQLDriverProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
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
                    innerList.Clear();
                    lMethods.Clear();//ELIMINATARIGA
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

        protected override bool PromoteJobToWrite(CommJob job)
        {
            if (ChannelBase != null)
            {
                return job.Type == LinkType.ExceptionOutput || !Channel.IsJobInPollingInErrorState(job);
            }
            return false;
        }

        #endregion

        #region Specific Methods
        #endregion

        #region Properties
        private string _SQLDriverTableName;

        public string SQLDriverTableName
        {
            get { return _SQLDriverTableName; }
            set { _SQLDriverTableName = value; }
        }

        private string _SQLDriverColumnNameTagName;

        public string SQLDriverColumnNameTagName
        {
            get { return _SQLDriverColumnNameTagName; }
            set
            {
                _SQLDriverColumnNameTagName = value;
            }
        }

        private string _SQLDriverColumnValue;
        public string SQLDriverColumnValue
        {
            get { return _SQLDriverColumnValue; }
            set
            {
                _SQLDriverColumnValue = value;
            }
        }

        private bool _SQLDriverMultiColumn;
        public bool SQLDriverMultiColumn
        {
            get { return _SQLDriverMultiColumn; }
            set
            {
                _SQLDriverMultiColumn = value;
            }
        }
        #endregion

    }
}
