using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;

namespace PPI
{
    class PPIStation : Station
    {
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public PPIStation(CommunicationDriver commdriver, PPIStationSettings settings)
            : base(commdriver, settings)
        {
            _StationAddress = settings.StationAddress;
            _AppHandle = 0;

            _ConnEstablished = false;

            _LocalHandle = 0;
            _RemoteHandle = 0;
            _SeqNumber = 0;
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as PPICommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new PPICommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as PPITag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new PPICommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new PPITag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as PPICommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new PPICommJobSettings(session, commJob);
        }

        #endregion
        
        #region Member
        #endregion

        #region Methods
        #endregion

        #region override Methods

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            PPICommJob mJ = e.Job as PPICommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                if (mJ.IsRead)
                {
                    if (e.Values != null)
                    {
                        byte[] Answer = (byte[])e.Values;
                        List<object> ChangedTags = new List<object>();
                        if (PPIProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                            foreach (var tag in ChangedTags)
                            {
                                var j = tag as Tag;
                                if (j != null)
                                    e.ChangedTags.Add(j);
                            }
                        else
                            e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                    }
                }
            }

            //put all the job in error?
            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

            base.ProcessJobValues(e);
            mJ.ExecuteTask = false;
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            PPICommJob mj = job as PPICommJob;

            PPIProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
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
                    PPIDynTagSettings dts = new PPIDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}AR{3}OF{4}LN{5}FT{6}TR{7}BT{8}GE{9}",
                        string.Empty, dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.Area,
                        dts.Offset.ToString("00000"), 
                        dts.Length.ToString("00000"), ((int)dts.Format).ToString("00"), ((int)dts.Trans).ToString("00"), dts.Bit.ToString("00"), dts.German);
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}AR{3}OF{4}LN{5}FT{6}TR{7}BT{8}GE{9}",
                        string.Empty, dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.Area,
                        dts.Offset.ToString("00000"), 
                        dts.Length.ToString("00000"), ((int)dts.Format).ToString("00"), ((int)dts.Trans).ToString("00"), dts.Bit.ToString("00"), dts.German);
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
        #endregion

        #region Properties
        private UInt16 _AppHandle;
        public UInt16 AppHandle
        {
            get { return _AppHandle; }
            set { _AppHandle = value; }
        }
        private byte _StationAddress;
        public byte StationAddress
        {
            get { return _StationAddress; }
            set { _StationAddress = value; }
        }


        private ushort _SeqNumber;
        public ushort SeqNumber
        {
            get
            {
                if (_SeqNumber == 0)
                    _SeqNumber++;
                return _SeqNumber++;
            }
            set { _SeqNumber = value; }
        }
        private byte _LocalHandle;
        public byte LocalHandle
        {
            get { return _LocalHandle; }
            set { _LocalHandle = value; }
        }
        private byte _RemoteHandle;
        public byte RemoteHandle
        {
            get { return _RemoteHandle; }
            set { _RemoteHandle = value; }
        }
        private bool _ConnEstablished;
        public bool ConnEstablished
        {
            get { return _ConnEstablished; }
            set { _ConnEstablished = value; }
        }

        #endregion

    }
}
