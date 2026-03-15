using System;
using System.Collections.Generic;
using System.Linq;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;

namespace EtherNetIP
{
    public struct PlcTagInstanceInfo
    {

        public ushort m_nInstance;
        public ushort m_nType;
        public ushort m_nProgramInstance;
        public ushort m_nTemplateInstance;
        public UInt32 m_nDim0;
        public UInt32 m_nDim1;
        public UInt32 m_nDim2;
        public string m_szTemplateName;
    }

    public struct PlcTemplateFieldInfo
    {
        public UInt32 m_nIndex;
        public UInt32 m_nOffset;
        public ushort m_nBitNumber;
        public ushort m_nType;
        public ushort m_nTemplateInstance;
        public UInt32 m_nDim0;
        public string m_szTemplateName;
        public string m_szName;
    }

    public struct PlcTemplateInfo
    {
        public bool m_bIsString;
        public UInt32 m_nMemorySize;
        public Dictionary<string, PlcTemplateFieldInfo> m_mapFieldInfo;
    }

    public struct EtherNetIPTemplateFieldInfo
    {
        public String m_szName;
        public ushort m_nType;
        public uint m_nOffset;
        public ushort m_nBitNumber;
        public UInt32 m_nDim0;
        public string m_szTemplateName;
    };

    public class EtherNetIPStation : Station
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
        public bool BuildInfoMaps;
        public byte FirmwareVersion;
        public ushort CpuModel;

        public List<ushort> m_listProgramsTrueIstances = new List<ushort>();
        public List<string> m_listPrograms = new List<string>();
        public Dictionary<string, ushort> m_mapProgramsInstances = new Dictionary<string, ushort>();
        public Dictionary<uint, ushort> m_mapProgramsAddresses = new Dictionary<uint, ushort>();
        public Dictionary<string, PlcTagInstanceInfo> m_mapPlcTagInstanceInfo = new Dictionary<string, PlcTagInstanceInfo>();
        public Dictionary<ushort, PlcTemplateInfo> m_mapPlcTemplateInfo = new Dictionary<ushort, PlcTemplateInfo>();
        public byte[] m_TemplateBuffer;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public EtherNetIPStation(CommunicationDriver commdriver, EtherNetIPStationSettings settings)
            : base(commdriver, settings)
        {
            _PlcType = settings.PlcType;
            _CPUSlot = settings.CPUSlot;
            _Logix5550NonBlockPhAdd = settings.Logix5550NonBlockPhAdd;
            _ReadStructuresMode = settings.ReadStructuresMode;
            _PLCStatusPollingTime = settings.PLCStatusPollingTime;
            LastTransOK = true;
            Transaction = new ushortUnion(0);
            ConnectionIDSet = false;
            OTNetConnID = new uintUnion(0);
            TONetConnID = new uintUnion(0);
            ConnectionSerialNumber = new ushortUnion(0);
            SequenceCount = new ushortUnion(0);
            lastProcessedTags = new ushortUnion(0);
            BuildInfoMaps = false;
        }

        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as EtherNetIPCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new EtherNetIPCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as EtherNetIPTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new EtherNetIPCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new EtherNetIPTag(td);
        }

        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as EtherNetIPCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new EtherNetIPCommJobSettings(session, commJob);
        }

        #endregion

        #region override Methods

        public override void ProcessJobValues(ExecutedJobArgs e/* CommJob job, DriverErrorCodes error*/)
        {
            EtherNetIPCommJob mJ = e.Job as EtherNetIPCommJob;
            if (mJ == null)
                return;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                if (e.Values != null)
                {
                    byte[] Answer = (byte[])e.Values;
                    List<object> ChangedTags = new List<object>();
                    if (EtherNetIpProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
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

            mJ.InitEtherNetIPCommJob(false);
        }
     

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            EtherNetIPCommJob mj = job as EtherNetIPCommJob;

            EtherNetIpProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
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
                    EtherNetIPDynTagSettings dts = new EtherNetIPDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}AT{3}TF{4}AB{5}",
                                                dts.DeviceSize.ToString("000"), 
                                                dts.TagLinkType.ToString("000"),
                                                dts.MethodID.ToString("000"), 
                                                dts.AddressType, 
                                                dts.TagFormat ,
                                                //FormatTheNumericalPartOfTheString(dts.ABAddress, dts.AddressType));
                                                FormatTheNumericalPartOfTheString(dts));
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}AT{3}TF{4}AB{5}",
                                                dts.DeviceSize.ToString("000"), 
                                                dts.TagLinkType.ToString("000"),
                                                dts.MethodID.ToString("000"), 
                                                dts.AddressType, 
                                                dts.TagFormat,
                                                //FormatTheNumericalPartOfTheString(dts.ABAddress, dts.AddressType));
                                                FormatTheNumericalPartOfTheString(dts));
                    return cx.CompareTo(cy);
                    //return x.TagNode.DynamicSettings.CompareTo(y.TagNode.DynamicSettings);
                }
            }
        }

        public static string FormatTheNumericalPartOfTheString(EtherNetIPDynTagSettings dts)
        {
            if (dts.AddressType != AddressTypes.DataFile)
            {
                return dts.ABAddress;
            }

            string formattedString = dts.FileType.ToString()        + dts.FileNum.ToString("00000") + 
                                     dts.Slot.ToString("00000")     + dts.Word.ToString("00000") + 
                                     dts.Element.ToString("00000")  + dts.Bit.ToString("00")  + 
                                     dts.SubElement.ToString();

            return (formattedString);
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

        public void Logix5550NonBlockPhAddEmptyInfoMaps()
        {
            m_listPrograms.Clear();
            m_listProgramsTrueIstances.Clear();
            m_mapProgramsInstances.Clear();
            m_mapPlcTagInstanceInfo.Clear();
            Logix5000V21EmptyPlcTemplatesMap(ref m_mapPlcTemplateInfo);
            m_mapProgramsAddresses.Clear();
        }
        public static void Logix5000V21EmptyPlcTemplatesMap(ref Dictionary<ushort, PlcTemplateInfo> mapPlcTemplateInfo)
        {
            foreach (KeyValuePair<ushort, PlcTemplateInfo> entry in mapPlcTemplateInfo)
                if (entry.Value.m_mapFieldInfo != null)
                    entry.Value.m_mapFieldInfo.Clear();
            mapPlcTemplateInfo.Clear();
        }

        //public static void RemoveMapPlcTemplateInfoNameStruct(ref Dictionary<ushort, List<string>> map)
        //{
        //    //Remove all element in the maps
        //    if (map.Count() == 0)
        //    {
        //        return;
        //    }
        //    foreach (KeyValuePair<ushort, List<string>> entry in map)
        //        if (entry.Value.Count != 0)
        //            entry.Value.Clear();
        //    map.Clear();
        //}
        //static void CopyMapPlcTemplateInfoNameStruct(ref Dictionary<ushort, List<string>> mapDestination, ref Dictionary<ushort, List<string>> mapSource)
        //{
        //    //Remove all element in the maps
        //    foreach (KeyValuePair<ushort, List<string>> entry in mapSource)
        //        if (entry.Value.Count != 0)
        //            mapDestination[entry.Key] = entry.Value;
        //    mapSource.Clear();
        //}

        #endregion


        #region Properties

        private PlcTypes _PlcType;
        public PlcTypes PlcType
        {
            get { return _PlcType; }
            set { _PlcType = value; }
        }

        private byte _CPUSlot;
        public byte CPUSlot
        {
            get { return _CPUSlot; }
            set { _CPUSlot = value; }
        }

        private PhisicalAddressesOptimizzations _Logix5550NonBlockPhAdd;
        public PhisicalAddressesOptimizzations Logix5550NonBlockPhAdd
        {
            get { return _Logix5550NonBlockPhAdd; }
            set { _Logix5550NonBlockPhAdd = value; }
        }

        private ReadStructuresModes _ReadStructuresMode;
        public ReadStructuresModes ReadStructuresMode
        {
            get { return _ReadStructuresMode; }
            set { _ReadStructuresMode = value; }
        }

        private uint _PLCStatusPollingTime;
        public uint PLCStatusPollingTime
        {
            get { return _PLCStatusPollingTime; }
            set { _PLCStatusPollingTime = value; }
        }

        #endregion

    }
}
