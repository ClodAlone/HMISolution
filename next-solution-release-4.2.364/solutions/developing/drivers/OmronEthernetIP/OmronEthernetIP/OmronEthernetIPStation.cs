using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;

namespace OmronEthernetIP
{
    public class OmronEthernetIPStation : Station
    {

        #region Data Members
 
        public bool LastTransOK;
        private ushortUnion Transaction;
        public bool ConnectionIDSet;
        public uintUnion OTNetConnID;
        public uintUnion TONetConnID;
        public ushortUnion ConnectionSerialNumber;
        public ushortUnion SequenceCount;
        public ushortUnion lastProcessedTags;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public OmronEthernetIPStation(CommunicationDriver commdriver, OmronEthernetIPStationSettings settings)
            : base(commdriver, settings)
        {
            _CPUSlot = settings.CPUSlot;
            _PlcType = settings.PlcType;
            _MaxPduSize = OmronEthernetIPProtocol.GetMaxPduSize(_PlcType);
            LastTransOK = true;
            Transaction = new ushortUnion(0);
            ConnectionIDSet = false;
            OTNetConnID = new uintUnion(0);
            TONetConnID = new uintUnion(0);
            ConnectionSerialNumber = new ushortUnion(0);
            SequenceCount = new ushortUnion(0);
            lastProcessedTags = new ushortUnion(0);
        }

        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as OmronEthernetIPCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new OmronEthernetIPCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as OmronEthernetIPTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new OmronEthernetIPCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new OmronEthernetIPTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as OmronEthernetIPCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new OmronEthernetIPCommJobSettings(session, commJob);
        }

        #endregion

        #region override Methods

        public override void ProcessJobValues(ExecutedJobArgs e/* CommJob job, DriverErrorCodes error*/)
        {
            OmronEthernetIPCommJob mJ = e.Job as OmronEthernetIPCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                if (e.Values != null)
                {
                    byte[] Answer = (byte[])e.Values;
                    List<object> ChangedTags = new List<object>();
                    if (OmronEthernetIPProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
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

            mJ.ResetOmronEthernetIPCommJob();
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            OmronEthernetIPCommJob mj = job as OmronEthernetIPCommJob;

            OmronEthernetIPProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
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
                    OmronEthernetIPDynTagSettings dts = new OmronEthernetIPDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}TF{3}AB{4}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.TagFormat, dts.ABAddress);
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}TF{3}AB{4}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.TagFormat, dts.ABAddress);
                    return cx.CompareTo(cy);
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

        #region Methods
 
        public ushortUnion GetTransaction()
        {
            if (LastTransOK)
                Transaction.USHORT++;

            LastTransOK = false;
            return Transaction;
        }

        public void ClearConnection()
        {
            LastTransOK = true;
            Transaction.USHORT = 0;
            ConnectionIDSet = false;
        }

        public void Logix5550SetInitConnIDs()
        {
            DateTime centuryBegin = new DateTime(2001, 1, 1);
            DateTime currentDate = DateTime.Now;
            OTNetConnID.UINT = (uint)((currentDate.Ticks - centuryBegin.Ticks) % 0x10000);
            TONetConnID.UINT = OTNetConnID.UINT + 3;
        }

        #endregion


        #region Properties

        private byte _CPUSlot;
        public byte CPUSlot
        {
            get { return _CPUSlot; }
            set { _CPUSlot = value; }
        }

        private PlcTypes _PlcType;
        public PlcTypes PlcType
        {
            get { return _PlcType; }
            set { _PlcType = value; }
        }

        private uint _MaxPduSize;
        public uint MaxPduSize
        {
            get { return _MaxPduSize; }
            set { _MaxPduSize = value; }
        }

        #endregion

    }
}
