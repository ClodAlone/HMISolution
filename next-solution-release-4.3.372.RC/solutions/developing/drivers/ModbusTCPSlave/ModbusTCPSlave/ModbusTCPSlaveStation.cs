using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DevExpress.Xpo;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using IpDriverCodeBase;

namespace ModbusTCPSlave
{
    class MemoryDataAreas
    {
        #region Constructors

        public MemoryDataAreas(List<CommJob> ListJob)
        {
            CoilsMap = new Dictionary<ushort,bool>();
            CoilsJobsMap = new Dictionary<ushort, List<ModbusTCPSlaveCommJob>>();
            DiscreteInputsMap = new Dictionary<ushort, bool>();
            DiscreteInputsJobsMap = new Dictionary<ushort, List<ModbusTCPSlaveCommJob>>();
            HoldingRegistersMap = new Dictionary<ushort, ushort>();
            HoldingRegistersJobsMap = new Dictionary<ushort, List<ModbusTCPSlaveCommJob>>();
            InputRegistersMap = new Dictionary<ushort, ushort>();
            InputRegistersJobsMap = new Dictionary<ushort, List<ModbusTCPSlaveCommJob>>();
            foreach (CommJob job in ListJob)
                addToMemory(job as ModbusTCPSlaveCommJob);
        }
        #endregion

        #region Data Members
        Dictionary<ushort, bool> CoilsMap;
        Dictionary<ushort, List<ModbusTCPSlaveCommJob>> CoilsJobsMap;
        Dictionary<ushort, bool> DiscreteInputsMap;
        Dictionary<ushort, List<ModbusTCPSlaveCommJob>> DiscreteInputsJobsMap;
        Dictionary<ushort, ushort> HoldingRegistersMap;
        Dictionary<ushort, List<ModbusTCPSlaveCommJob>> HoldingRegistersJobsMap;
        Dictionary<ushort, ushort> InputRegistersMap;
        Dictionary<ushort, List<ModbusTCPSlaveCommJob>> InputRegistersJobsMap;
        protected Object lockMemory = new Object();
        #endregion
        #region Methods
        Dictionary<ushort, bool> getBoolMap(DataAreas dataArea)
        {
            switch (dataArea)
            {
                case DataAreas.Coils:
                    return CoilsMap;
                case DataAreas.DiscreteInputs:
                    return DiscreteInputsMap;
            }
            return CoilsMap;
        }
        Dictionary<ushort, List<ModbusTCPSlaveCommJob>> getJobMap(DataAreas dataArea)
        {
            switch (dataArea)
            {
                case DataAreas.Coils:
                    return CoilsJobsMap;
                case DataAreas.DiscreteInputs:
                    return DiscreteInputsJobsMap;
                case DataAreas.HoldingRegisters:
                    return HoldingRegistersJobsMap;
                case DataAreas.InputRegisters:
                    return InputRegistersJobsMap;
            }
            return HoldingRegistersJobsMap;
        }
        Dictionary<ushort, ushort> getRegisterMap(DataAreas dataArea)
        {
            switch (dataArea)
            {
                case DataAreas.HoldingRegisters:
                    return HoldingRegistersMap;
                case DataAreas.InputRegisters:
                    return InputRegistersMap;
            }
            return HoldingRegistersMap;
        }

        void addToMemory(ModbusTCPSlaveCommJob job)
        {
            ushort Address = job.StartAddress;
            short size = (short)job.TotalJobSize;
            while (size > 0)
            {
                switch (job.DataArea)
                {
                    case DataAreas.Coils:
                    case DataAreas.DiscreteInputs:
                        if (!getBoolMap(job.DataArea).ContainsKey(Address))
                        {
                            getBoolMap(job.DataArea)[Address] = false;
                            getJobMap(job.DataArea)[Address] = new List<ModbusTCPSlaveCommJob>();
                        }
                        size--;
                        break;
                    case DataAreas.HoldingRegisters:
                    case DataAreas.InputRegisters:
                        if (!getRegisterMap(job.DataArea).ContainsKey(Address))
                        {
                            getRegisterMap(job.DataArea)[Address] = 0;
                            getJobMap(job.DataArea)[Address] = new List<ModbusTCPSlaveCommJob>();
                        }
                        size-=2;
                        break;
                }
                getJobMap(job.DataArea)[Address].Add(job);
                Address++;
            }
        }

        public bool validateMemory(ref ReceiveItem receiveItem)
        {
            for (ushort index = 0; index < receiveItem.Pdu.quantity; index++)
            {
                switch (ModbusSlaveProtocol.DataAreaOfFunctionCode(receiveItem.Pdu.functionCode))
                {
                    case DataAreas.DiscreteInputs:
                    case DataAreas.Coils:
                        if (!getBoolMap(ModbusSlaveProtocol.DataAreaOfFunctionCode(receiveItem.Pdu.functionCode)).ContainsKey((ushort)(receiveItem.Pdu.startAddress + index)))
                            return false;
                        break;
                    case DataAreas.HoldingRegisters:
                    case DataAreas.InputRegisters:
                        if (!getRegisterMap(ModbusSlaveProtocol.DataAreaOfFunctionCode(receiveItem.Pdu.functionCode)).ContainsKey((ushort)(receiveItem.Pdu.startAddress + index)))
                            return false;
                        break;
                    default:
                        return false;
                }
            }
            return true;
        }

        public byte[] getMemoryData(ref ReceiveItem receiveItem)
        {
            lock (lockMemory)
            {
                List<byte> retVal = new List<byte>();
                byte[] buffer;

                try
                {
                    switch (receiveItem.Pdu.functionCode)
                    {
                        case FunctionCodes.ReadCoils:
                        case FunctionCodes.ReadDiscreteInputs:
                            ushort quantity = receiveItem.Pdu.quantity;
                            ushort Address = 0;
                            buffer = new byte[(quantity + 7) / 8];
                            for (ushort index = 0; index < receiveItem.Pdu.quantity; index++)
                            {
                                Address = (ushort)(receiveItem.Pdu.startAddress + index);
                                if (getBoolMap(ModbusSlaveProtocol.DataAreaOfFunctionCode(receiveItem.Pdu.functionCode))[Address])
                                {
                                    if(quantity == 1)
                                    {
                                        buffer[0] = (byte)1;
                                    }
                                    else
                                    {
                                        buffer[(index)/ 8] |= (byte)(1 << ((index) % 8));
                                    }                                   
                                }
                            }
                            retVal.AddRange(buffer);
                            break;
                        case FunctionCodes.ReadInputRegisters:
                        case FunctionCodes.ReadHoldingRegisters:
                        case FunctionCodes.MaskWriteRegister:
                            for (ushort index = receiveItem.Pdu.startAddress; index < receiveItem.Pdu.startAddress + receiveItem.Pdu.quantity; index++)
                                retVal.AddRange(BufferExpand.toArray(getRegisterMap(ModbusSlaveProtocol.DataAreaOfFunctionCode(receiveItem.Pdu.functionCode))[index]));
                            break;
                    }
                }
                catch
                {
                    retVal = new List<byte>();
                }

                return retVal.ToArray();
            }
        }

        void ListAddRange(ref List<ModbusTCPSlaveCommJob> Jobs,List<ModbusTCPSlaveCommJob> addJobs)
        {
            foreach (ModbusTCPSlaveCommJob job in addJobs)
                if (!Jobs.Contains(job))
                    Jobs.Add(job);
        }
        public void setMemoryData(ref ReceiveItem receiveItem, ref byte[] buffer, out List<ModbusTCPSlaveCommJob> changeJobs)
        {
            lock (lockMemory)
            {
                changeJobs = new List<ModbusTCPSlaveCommJob>();
                try
                {
                    switch (ModbusSlaveProtocol.DataAreaOfFunctionCode(receiveItem.Pdu.functionCode))
                    {
                        case DataAreas.Coils:
                            for (ushort index = 0; index < receiveItem.Pdu.quantity; index++)
                            {
                                ushort memoryIndex = (ushort)(index + receiveItem.Pdu.startAddress);
                                if (getBoolMap(DataAreas.Coils).ContainsKey(memoryIndex))
                                {
                                    getBoolMap(DataAreas.Coils)[memoryIndex] = (buffer[index / 8] & (byte)(1 << (index % 8))) != 0;
                                    ListAddRange(ref changeJobs, getJobMap(DataAreas.Coils)[memoryIndex]);
                                }

                            }
                            break;
                        case DataAreas.HoldingRegisters:
                            int Offset = 0;
                            for (ushort index = receiveItem.Pdu.startAddress; index < receiveItem.Pdu.startAddress + buffer.Length / 2; index++)
                            {
                                if (getRegisterMap(DataAreas.HoldingRegisters).ContainsKey(index))
                                {
                                    getRegisterMap(DataAreas.HoldingRegisters)[index] = BufferExpand.toUInt16(ref buffer, ref Offset);
                                    ListAddRange(ref changeJobs, getJobMap(DataAreas.HoldingRegisters)[index]);
                                }
                                else
                                    Offset += 2;
                            }
                            break;
                    }
                }
                catch
                {
                    changeJobs = new List<ModbusTCPSlaveCommJob>();
                }
            }
        }

        public byte[] getMemoryData(ref ModbusTCPSlaveCommJob job)
        {
            lock (lockMemory)
            {
                List<byte> retVal = new List<byte>();

                try
                {
                    switch (job.DataArea)
                    {
                        case DataAreas.Coils:
                        case DataAreas.DiscreteInputs:
                            byte[] buffer = new byte[(job.TotalJobSize + 7) / 8];
                            for (ushort index = 0; index < job.TotalJobSize; index++)
                            {
                                if (getBoolMap(job.DataArea)[(ushort)(index + job.StartAddress)])
                                    buffer[index / 8] |= (byte)(1 << (index % 8));
                            }
                            retVal.AddRange(buffer);
                            break;
                        case DataAreas.InputRegisters:
                        case DataAreas.HoldingRegisters:
                            for (ushort index = job.StartAddress; index < job.StartAddress + job.TotalJobSize / 2; index++)
                                retVal.AddRange(BufferExpand.toArray(getRegisterMap(job.DataArea)[index]));
                            break;
                    }
                }
                catch
                {
                    retVal = new List<byte>();
                }

                return retVal.ToArray();
            }
        }
        public void setMemoryData(ref ModbusTCPSlaveCommJob job, ref byte[] buffer)
        {
            List<ModbusTCPSlaveCommJob> changeJobs;
            setMemoryData(ref job, ref buffer, out changeJobs);

        }

        public void setMemoryData(ref ModbusTCPSlaveCommJob job, ref byte[] buffer, out List<ModbusTCPSlaveCommJob> changeJobs)
        {
            lock (lockMemory)
            {
                changeJobs = new List<ModbusTCPSlaveCommJob>();
                try
                {
                    switch (job.DataArea)
                    {
                        case DataAreas.Coils:
                        case DataAreas.DiscreteInputs:
                            for (ushort index = 0; index < job.TotalJobSize; index++)
                                if (getBoolMap(job.DataArea).ContainsKey((ushort)(index + job.StartAddress)))
                                {
                                    getBoolMap(job.DataArea)[(ushort)(index + job.StartAddress)] = (buffer[index / 8] & (byte)(1 << (index % 8))) != 0;
                                    ListAddRange(ref changeJobs, getJobMap(job.DataArea)[(ushort)(index + job.StartAddress)]);
                                }
                            break;
                        case DataAreas.HoldingRegisters:
                        case DataAreas.InputRegisters:
                            int Offset = 0;
                            for (ushort index = job.StartAddress; index < job.StartAddress + buffer.Length / 2; index++)
                            {
                                if (getRegisterMap(job.DataArea).ContainsKey(index))
                                {
                                    getRegisterMap(job.DataArea)[index] = BufferExpand.toUInt16(ref buffer, ref Offset);
                                    ListAddRange(ref changeJobs, getJobMap(job.DataArea)[index]);
                                }
                                else
                                    Offset += 2;

                            }
                            break;
                    }
                }
                catch
                {
                    changeJobs = new List<ModbusTCPSlaveCommJob>();
                }
            }
        }

        public void getChangedJobs(ref ReceiveItem receiveItem, out List<ModbusTCPSlaveCommJob> changeJobs)
        {
            changeJobs = new List<ModbusTCPSlaveCommJob>();
            try
            {
                switch (ModbusSlaveProtocol.DataAreaOfFunctionCode(receiveItem.Pdu.functionCode))
                {
                    case DataAreas.Coils:
                        for (ushort index = receiveItem.Pdu.startAddress; index < receiveItem.Pdu.startAddress + receiveItem.Pdu.quantity; index++)
                            if (getBoolMap(DataAreas.Coils).ContainsKey(index))
                                ListAddRange(ref changeJobs, getJobMap(DataAreas.Coils)[index]);
                        break;
                    case DataAreas.HoldingRegisters:
                        for (ushort index = receiveItem.Pdu.startAddress; index < receiveItem.Pdu.startAddress + receiveItem.Pdu.quantity; index++)
                            if (getRegisterMap(DataAreas.HoldingRegisters).ContainsKey(index))
                                ListAddRange(ref changeJobs, getJobMap(DataAreas.HoldingRegisters)[index]);
                        break;
                }
            }
            catch
            {
                changeJobs = new List<ModbusTCPSlaveCommJob>();
            }

        }

        #endregion

    }

    public class ModbusTCPSlaveStation : Station
    {

        public enum addressingModes : byte
        {
            unitID,
            IpAddres,
            invalid,
        }

        #region Constructors

        /// <summary>
        /// Initializes the station object.
        /// </summary>
        public ModbusTCPSlaveStation(CommunicationDriver commdriver, ModbusTCPSlaveStationSettings settings)
            : base(commdriver, settings)
        {
            _StationID = settings.StationID;
            _DeviceHostName = settings.DeviceHostName;
            _DeviceBackupHostName = settings.DeviceBackupHostName;
            _ipAddress = null;
            _backupIpAddress = null;
            initMemoryData();
        }
       
        #endregion

        #region Data Members

        MemoryDataAreas memory;
        protected Object lockProcessJob = new Object();

        #endregion
        #region Methods

        public void initMemoryData()
        {
            memory = new MemoryDataAreas(new List<CommJob>());
        }
        public void createMemoryData()
        {
            memory = new MemoryDataAreas(ListWholeJob);
        }
        public bool validateMemory(ref ReceiveItem receiveItem)
        {
            return memory.validateMemory(ref receiveItem);
        }
        public byte[] getMemoryData(ref ReceiveItem receiveItem)
        {
            return memory.getMemoryData(ref receiveItem);
        }
        public void setMemoryData(ref ReceiveItem receiveItem, ref byte[] buffer, out List<ModbusTCPSlaveCommJob> changeJobs)
        {
            memory.setMemoryData(ref receiveItem, ref buffer, out changeJobs);
        }
        public void getChangedJobs(ref ReceiveItem receiveItem, out List<ModbusTCPSlaveCommJob> changeJobs)
        {
            memory.getChangedJobs(ref receiveItem, out changeJobs);
        }
        public byte[] getMemoryData(ModbusTCPSlaveCommJob job)
        {
            return memory.getMemoryData(ref job);
        }
        public void setMemoryData(ModbusTCPSlaveCommJob job, ref byte[] buffer)
        {
            memory.setMemoryData(ref job, ref buffer);
        }

        public void setMemoryData(ModbusTCPSlaveCommJob job, ref byte[] buffer, out List<ModbusTCPSlaveCommJob> changeJobs)
        {
            memory.setMemoryData(ref job, ref buffer, out changeJobs);
        }
        public List<ModbusTCPSlaveCommJob> totalJobs()
        {
            List<ModbusTCPSlaveCommJob> rt = new List<ModbusTCPSlaveCommJob>();
            foreach (CommJob job in ListWholeJob)
                rt.Add(job as ModbusTCPSlaveCommJob);
            return rt;
        }

        #endregion

        #region Abstract Methods

        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as ModbusTCPSlaveCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new ModbusTCPSlaveCommJob(this, conf);
        }

        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as ModbusTCPSlaveTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new ModbusTCPSlaveCommJob(this, conf);
        }

        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new ModbusTCPSlaveTag(td);
        }
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as ModbusTCPSlaveCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new ModbusTCPSlaveCommJobSettings(session, commJob);
        }

        public override uint OnWriteTag(NodeId tagnodeid, ref object value, ref StatusCode statusCode, ref DateTime timestamp, bool ignoreWriteAsync = false, bool forceSynchWrite = false)
        {
            uint ret = base.OnWriteTag(tagnodeid,ref value, ref statusCode, ref timestamp);
            if (ret == StatusCodes.Good)
            {
                //System.Diagnostics.Trace.TraceInformation("Station.OnWriteTag tag:{0} value:{1}", tagnodeid.ToString(), value.ToString());
                lock (lockListObject)
                {
                    if (!mapTagJob.ContainsKey(tagnodeid))
                        return StatusCodes.BadNodeIdInvalid;

                    ModbusTCPSlaveCommJob job = mapTagJob[tagnodeid] as ModbusTCPSlaveCommJob;
                    byte[] writeData;
                    if (job.PrepareData(out writeData))
                    {
                        lock (job.retLockList())
                        {
                            Channel.ExecuteJob(job);
                        }
                        List<ModbusTCPSlaveCommJob> changedJob;
                        setMemoryData(job, ref writeData, out changedJob);
                        changedJob.Remove(job);
                        (Channel as TcpServer).ExecuteReadJob(ref changedJob);
                        job.LastExecutionTime = DateTime.UtcNow;
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Values = new byte[0];
                        eJob.Job = job;
                        (Channel as TcpServer).PublicOnJobExecuted(eJob);
                    }
                }
            }

            return ret;
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
                    ModbusTCPSlaveDynTagSettings dts = new ModbusTCPSlaveDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}DA{3}SA{4}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.DataArea,
                        dts.StartAddress.ToString("00000"));
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}DA{3}SA{4}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.DataArea,
                        dts.StartAddress.ToString("00000"));
                    return cx.CompareTo(cy);
                    //return x.TagNode.DynamicSettings.CompareTo(y.TagNode.DynamicSettings);
                }
            }
        }

        #region override Methods

        public override List<Tag> SortTags(IList<Tag> tags)
        {
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
        }

        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            ModbusTCPSlaveCommJob mJ = e.Job as ModbusTCPSlaveCommJob;
            if (mJ == null)
                return;
            lock (lockProcessJob)
            {

                // analyzing answer if no error exists before
                if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                {
                    //ModbusProtocol P = new ModbusProtocol();
                    byte[] Answer = (byte[])e.Values;
                    if (Answer != null && Answer.Length != 0)
                    {
                        List<object> ChangedTags = new List<object>();
                        if (/*P*/ModbusSlaveProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                            foreach (var tag in ChangedTags)
                            {
                                var j = tag as Tag;
                                if (j != null)
                                    e.ChangedTags.Add(j);
                            }
                        //e.ChangedTags.AddRange(ChangedTags);
                        else
                        {
                            if (ChangedTags.Count > 0)
                                e.ErrorCode = (DriverErrorCodes)((ModbusErrorCodes)ChangedTags[0]);
                            else
                                e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                        }
                    }
                }

                //put all the job in error?
                e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

                base.ProcessJobValues(e);
            }
        }

        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            ModbusTCPSlaveCommJob mj = job as ModbusTCPSlaveCommJob;

            ModbusSlaveProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }

        #endregion
    
        #region Properties

        private uint _StationID;
        public uint StationID
        {
            get { return _StationID; }
            set
            {
                _StationID = value;
            }
        }

        private string _DeviceHostName;
        public string DeviceHostName
        {
            get { return _DeviceHostName; }
            set
            {
                _DeviceHostName = value;
            }
        }

        private string _DeviceBackupHostName;
        public string DeviceBackupHostName
        {
            get { return _DeviceBackupHostName; }
            set
            {
                _DeviceBackupHostName = value;
            }
        }

        string _ipAddress;
        public string ipAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_ipAddress) && !string.IsNullOrEmpty(_DeviceHostName))
                {
                    IPAddress resolvedIPAddress;
                    if (UdpChannel.GetResolvedConnecionIPAddress(_DeviceHostName, out resolvedIPAddress))
                        _ipAddress = resolvedIPAddress.ToString();
                }
                return _ipAddress;
            }
        }

        string _backupIpAddress;
        public string backupIpAddress
        {
            get
            {
                if (string.IsNullOrEmpty(_backupIpAddress) && !string.IsNullOrEmpty(_DeviceBackupHostName))
                {
                    IPAddress resolvedIPAddress;
                    if (UdpChannel.GetResolvedConnecionIPAddress(_DeviceBackupHostName, out resolvedIPAddress))
                        _backupIpAddress = resolvedIPAddress.ToString();
                }
                return _backupIpAddress;
            }
        }

        public addressingModes addressingMode
        {
            get
            {
                if (string.IsNullOrEmpty(_DeviceHostName))
                    return addressingModes.unitID;
                else if (!string.IsNullOrEmpty(ipAddress))
                    return addressingModes.IpAddres;
                else
                    return addressingModes.invalid;
            }
        }

       
        #endregion

    }
}
