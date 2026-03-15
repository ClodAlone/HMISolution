using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using DriverBaseInterfaces;
using System.Threading.Tasks;

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

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            S7TCPCommJob mJ = e.Job as S7TCPCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                List<object> ChangedTags = new List<object>();
                if (e.Job.IsCustomJob(CommJob.CustomJobCheckStates.Read))
                {
                    e.Job.GetCustomJobsChangedTags(ref ChangedTags);
                    foreach (var tag in ChangedTags)
                    {
                        var j = tag as Tag;
                        if (j != null)
                            e.ChangedTags.Add(j);
                    }
                }
                else
                {
                    byte[] Answer = (byte[])e.Values;                                        
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
                        string.Empty, dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.Area,
                        dts.Offset.ToString("00000"), dts.DbNumber.ToString("000"),
                        dts.Length.ToString("00000"), ((int)dts.Format).ToString("00"), ((int)dts.Trans).ToString("00"), dts.Bit.ToString("00"), dts.German, dts.S7_200);
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}AR{3}OF{4}DB{5}LN{6}FT{7}TR{8}BT{9}GE{10}S7{11}",
                        string.Empty, dts.TagLinkType.ToString("000"),
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


        #region Struct Atomic
        public override bool IsStructAtomic(Tag candTag)
        {
            bool isStruct = false;

            if (candTag !=null && candTag.TagNode.DataType.IdType == IdType.Guid)
            {
                if (candTag.DynSettings.StructAtomicEnable || (!String.IsNullOrEmpty(candTag.DynSettings.ConditionalVariableName) && !String.IsNullOrEmpty(candTag.DynSettings.ConditionalVariableId)))
                {
                    S7TCPDynTagSettings startDyn = (S7TCPDynTagSettings)(candTag).DynSettings;

                    // area M or Data block
                    if (startDyn.Area == Step7Area.aD || startDyn.Area == Step7Area.aQ || startDyn.Area == Step7Area.aM || startDyn.Area == Step7Area.aI)
                        isStruct = true;
                }
            }
            return isStruct;
        }

        /// <summary>
        /// Force custom job to be recreated 
        /// </summary>
        internal void ReCreateCustomJobs()
        {
            var listJobs = new List<CommJob>();
            lock (lockListObject)
                listJobs.AddRange(ListWholeJob);

            Parallel.ForEach(listJobs, j =>
            {
                if (j.IsCustomJob())
                    ((S7TCPCommJob)j).ReCreateCustomJob();
            });
        }


        #endregion

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
