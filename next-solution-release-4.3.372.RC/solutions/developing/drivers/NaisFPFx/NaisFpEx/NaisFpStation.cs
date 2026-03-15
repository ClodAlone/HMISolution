using System;
using System.Collections.Generic;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;

namespace NaisFp
{
    class NaisFpStation : Station
    {
                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public NaisFpStation(CommunicationDriver commdriver, NaisFpStationSettings settings)
            : base(commdriver, settings)
        {
            _StationID = settings.StationID;
            _FrameFormat = settings.FrameFormat;
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as NaisFpCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new NaisFpCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as NaisFpTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new NaisFpCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new NaisFpTag(td);
        }
        
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as NaisFpCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new NaisFpCommJobSettings(session, commJob);
        }

        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
        }

        #endregion
        #region override Methods

        public override void ProcessJobValues(ExecutedJobArgs e/* CommJob job, DriverErrorCodes error*/)
        {
            NaisFpCommJob mJ = e.Job as NaisFpCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError && e.Values != null)
            {
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();
                if (Answer.Length != 0)
                {
                    if (NaisFpProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                        foreach (var tag in ChangedTags)
                        {
                            var j = tag as Tag;
                            if (j != null)
                                e.ChangedTags.Add(j);
                        }
                    else
                    {
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
            NaisFpCommJob mj = job as NaisFpCommJob;

            NaisFpProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        #endregion

        #region Methods

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
                    NaisFpDynTagSettings dts = new NaisFpDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    NaisFpTag TagX = x as NaisFpTag;
                    NaisFpAddress TagXAddress = new NaisFpAddress(TagX.NaisFpDynSettings.Address);
                    string cx = string.Format("DS{0}TL{1}MI{2}Addr{3}{4}",
                        string.Empty, dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), TagXAddress.MemoryArea, TagXAddress.GetNaisAddress(TagXAddress.DataFormat));

                    dts.TryParse(y.TagNode.DynamicSettings);
                    NaisFpTag TagY = y as NaisFpTag;
                    NaisFpAddress TagYAddress = new NaisFpAddress(TagY.NaisFpDynSettings.Address);
                    string cy = string.Format("DS{0}TL{1}MI{2}Addr{3}{4}",
                        string.Empty, dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), TagYAddress.MemoryArea, TagYAddress.GetNaisAddress(TagYAddress.DataFormat));

                    return cx.CompareTo(cy);
                }
            }
        }

        #endregion

        #region Member


        #endregion

        #region Properties

        private byte _StationID;
        public byte StationID
        {
            get { return _StationID; }
            set
            {
                _StationID = value;
            }
        }

        private FrameFormats _FrameFormat;
        public FrameFormats FrameFormat
        {
            get { return _FrameFormat; }
            set
            {
                _FrameFormat = value;
            }
        }
               
        #endregion

    }
}
