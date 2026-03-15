using System;
using System.Collections.Generic;
using System.Threading;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using DriverBaseInterfaces;
using DriverCodeBaseEx.Helpers;

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

        public override uint OnWriteTag(NodeId tagnodeid, ref object value, ref StatusCode statusCode, ref DateTime timestamp)
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
                if (job.DuplicatedTagValueRemoved(tagnodeid, ref value))
                    return StatusCodes.Good;

                if (!CommDriver.WriteAsync && job.IsConditionalVariableOn())
                {
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
                    if (job.DuplicatedTagValueRemoved(tagnodeid, ref value))
                        return StatusCodes.Good;

                    var SqlChannel = Channel as SQLDriverChannel;
                    if (SqlChannel != null)
                        SqlChannel.AddTagToWriteCache(tagnodeid, new WritingTag() { Job = job, Value = value }) ;
                    ret = StatusCodes.Good;
                }
            }
            return ret;
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
