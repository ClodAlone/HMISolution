using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;


namespace OmronFinsEthernet
{
    class OmronFinsEthernetStation : Station
    {
                
        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public OmronFinsEthernetStation(CommunicationDriver commdriver, OmronFinsEthernetStationSettings settings)
            : base(commdriver, settings)
        {
            _DestinationNetworkAddress = settings.DestinationNetworkAddress;
            _DestinationNode = settings.DestinationNode;
            _DestinationUnit = settings.DestinationUnit;
            _SID = 0;
        }
       
        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as OmronFinsEthernetCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new OmronFinsEthernetCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as OmronFinsEthernetTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new OmronFinsEthernetCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new OmronFinsEthernetTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as OmronFinsEthernetCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new OmronFinsEthernetCommJobSettings(session, commJob);
        }

        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
        }

        public override void ProcessJobValues(ExecutedJobArgs e/* CommJob job, DriverErrorCodes error*/)
        {
            OmronFinsEthernetCommJob mJ = e.Job as OmronFinsEthernetCommJob;
            if (mJ == null)
            {
                return;
            }

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                if (e.Values != null)
                {
                    //OmronFinsEthernetProtocol P = new OmronFinsEthernetProtocol();
                    byte[] Answer = (byte[])e.Values;
                    List<object> ChangedTags = new List<object>();
                    if (/*P*/OmronFinsEthernetProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                    {
                        foreach (var tag in ChangedTags)
                        {
                            var j = tag as Tag;
                            if (j != null)
                            {
                                e.ChangedTags.Add(j);
                            }
                        }
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
            OmronFinsEthernetCommJob mj = job as OmronFinsEthernetCommJob;

            OmronFinsEthernetProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        #endregion

        #region Methods
        public bool CheckSID(byte inSID)
        {
            return inSID ==_SID;
        }

        public byte GetSID()
        {
            return(_SID);
        }

        #endregion



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
                    OmronFinsEthernetDynTagSettings dts = new OmronFinsEthernetDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}DCT{3}Addr{4}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.DataConversionType, dts.OmronFormattedAddress());
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}DCT{3}Addr{4}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.DataConversionType, dts.OmronFormattedAddress());
                    return cx.CompareTo(cy);
                }
            }
        }

        #region Properties

        private byte _DestinationNetworkAddress;
        public byte DestinationNetworkAddress
        {
            get
            {
                return _DestinationNetworkAddress;
            }
            set
            {
                _DestinationNetworkAddress = value;
            }
        }

        private byte _DestinationNode;
        public byte DestinationNode
        {
            get
            {
                return _DestinationNode;
            }
            set
            {
                _DestinationNode = value;
            }
        }

        private byte _DestinationUnit;
        public byte DestinationUnit
        {
            get
            {
                return _DestinationUnit;
            }
            set
            {
                _DestinationUnit = value;
            }
        }

        private byte _SID;
        public byte SID
        {
            get
            {
                return ++_SID;
            }
        }
        
        #endregion

    }
}
