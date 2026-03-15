using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;

namespace S7TCP
{
    public class S7TCPStation : Station
    {
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public S7TCPStation(CommunicationDriver commdriver, S7TCPStationSettings settings)
            : base(commdriver, settings)
        {
            _DeviceID = settings.DeviceID;
            _Rack = settings.Rack;
            _Slot = settings.Slot;
            _AppHandle = 0;
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as S7TCPCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new S7TCPCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as S7TCPTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new S7TCPCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new S7TCPTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as S7TCPCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new S7TCPCommJobSettings(session, commJob);
        }

        #endregion

        public override uint ExecuteSyncroJob(CommJob job, bool localmethod, int methodid, LinkType lt, IList<object> args, bool takedata = true)
        {
            uint ReqDim = 12;
            List<S7TCPCommJob> list = new List<S7TCPCommJob>();
            if (!(job is S7TCPCommJob) || !(Channel as S7TCPChannel).TryToAddToWriteJobList(ref ReqDim, ref list, (job as S7TCPCommJob)))
                return StatusCodes.BadNotWritable;

            return base.ExecuteSyncroJob(job, localmethod, methodid, lt, args, takedata);
        }
        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            S7TCPCommJob mJ = e.Job as S7TCPCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                byte[] Answer = (byte[])e.Values;
                //S7Protocol P = new S7Protocol();
                List<object> ChangedTags = new List<object>();
                if (Answer != null)
                {
                    if (/*P*/S7Protocol.ParseData(Answer, ref mJ, ref ChangedTags))
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
            }

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

            base.ProcessJobValues(e);
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            S7TCPCommJob mj = job as S7TCPCommJob;

            S7Protocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        private static int CompareTagByDynamic(Tag x, Tag y)
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
                    S7TCPDynTagSettings dts = new S7TCPDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}AR{3}OF{4}DB{5}LN{6}FT{7}TR{8}BT{9}GE{10}S7{11}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.Area,
                        dts.Offset.ToString("00000"), dts.DbNumber.ToString("000"),
                        dts.Length.ToString("00000"), ((int)dts.Format).ToString("00"), ((int)dts.Trans).ToString("00"), dts.Bit.ToString("00"), dts.German, dts.S7_200);
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}AR{3}OF{4}DB{5}LN{6}FT{7}TR{8}BT{9}GE{10}S7{11}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.Area,
                        dts.Offset.ToString("00000"), dts.DbNumber.ToString("000"),
                        dts.Length.ToString("00000"), ((int)dts.Format).ToString("00"), ((int)dts.Trans).ToString("00"), dts.Bit.ToString("00"), dts.German, dts.S7_200);
                    return cx.CompareTo(cy);
                    //return x.TagNode.DynamicSettings.CompareTo(y.TagNode.DynamicSettings);
                }
            }
        }
        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Executes the write tag action. </summary>
        ///
        /// <param name="tagnodeid" type="NodeId">  The tagnodeid. </param>
        /// <param name="value" type="ref object">  [in,out] The value. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint OnWriteTag(NodeId tagnodeid, ref object value, ref StatusCode statusCode, ref DateTime timestamp, bool ignoreWriteAsync = false, bool forceSynchWrite = false)
        {
            //System.Diagnostics.Trace.TraceInformation("Station.OnWriteTag tag:{0} value:{1}", tagnodeid.ToString(), value.ToString());
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
                                               (t.Value.Value == null) ||
                                               (!t.Value.Value.Equals(objTmp.Value)))
                                        select t).Count();
                    if (nTagsToWrite == 0)
                    {
                        return (StatusCodes.Good);
                    }
                }
            }

            uint ret = StatusCodes.BadConfigurationError;
            if (ChannelBase != null)
            {
                ret = job.OnWriteTag(tagnodeid, ref value);
                if (ret == StatusCodes.Good)
                {
                    // for job in PollingInError state, wait "normal" scheduling (to avoid station state var unstable value)
                    if (!Channel.IsJobInPollingInErrorState(job))
                        ChannelBase.ChangeStateJob(job, CommJobState.PollingNow);
                }
            }
            return ret;
        }

        #region Properties
        private UInt16 _AppHandle;
        public UInt16 AppHandle
        {
            get { return _AppHandle; }
            set { _AppHandle = value; }
        }

        private byte _DeviceID;
        public byte DeviceID
        {
            get { return _DeviceID; }
            set { _DeviceID = value; }
        }
        private byte _Rack;
        public byte Rack
        {
            get { return _Rack; }
            set { _Rack = value; }
        }
        private byte _Slot;
        public byte Slot
        {
            get { return _Slot; }
            set { _Slot = value; }
        }
        private bool _Connected;
        public bool Connected
        {
            get { return _Connected; }
            set { _Connected = value; }
        }
        #endregion

    }
}
