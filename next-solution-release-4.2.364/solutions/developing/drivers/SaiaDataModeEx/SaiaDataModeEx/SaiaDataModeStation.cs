using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;

namespace SaiaDataMode
{
    class SaiaDataModeStation : Station
    {
                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public SaiaDataModeStation(CommunicationDriver commdriver, SaiaDataModeStationSettings settings)
            : base(commdriver, settings)
        {
            _StationID = settings.StationID;
            RequestSequence = new ushortUnion(0); 
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as SaiaDataModeCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new SaiaDataModeCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as SaiaDataModeTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new SaiaDataModeCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new SaiaDataModeTag(td);
        }
        
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as SaiaDataModeCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new SaiaDataModeCommJobSettings(session, commJob);
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
            SaiaDataModeCommJob mJ = e.Job as SaiaDataModeCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError && e.Values != null)
            {
                byte[] Answer = (byte[])e.Values;
                List<object> ChangedTags = new List<object>();
                if (Answer.Length != 0)
                {
                    if (SaiaDataModeProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
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
            SaiaDataModeCommJob mj = job as SaiaDataModeCommJob;

            SaiaDataModeProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
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
                    SaiaDataModeDynTagSettings dts = new SaiaDataModeDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"));
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"));
                    return cx.CompareTo(cy);
                    //return x.TagNode.DynamicSettings.CompareTo(y.TagNode.DynamicSettings);
                }
            }
        }

        public bool CheckRequestSequence(ushort inRequestSequence)
        {
            return inRequestSequence == RequestSequence.USHORT;
        }

        public void GetRequestSequence(ref byte[] buffer, ref uintUnion Count)
        {
            RequestSequence.USHORT++;
            buffer[Count.UINT++] = RequestSequence.HIBYTE;
            buffer[Count.UINT++] = RequestSequence.LOBYTE;

        }

        #endregion

        #region Member

        private ushortUnion RequestSequence;

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
                
        #endregion

    }
}
