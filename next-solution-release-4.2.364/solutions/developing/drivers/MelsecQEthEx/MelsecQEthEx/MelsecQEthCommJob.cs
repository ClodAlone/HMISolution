using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace MelsecQEth
{
    public enum MelsecQCommandCode : ushort
    {
        WordUnits = 0,
        BitUnits = 1,
        WordUnits32bits = 2,    // IQR series
        BitUnits32bits = 3,     // IQR series
    };
    public enum CpuTargets
    {
        ControlPLC,
        Plc1,
        Plc2,
        Plc3,
        Plc4
    };

    public class MelsecQEthCommJob : CommJob
    {
        #region Constructors

        public MelsecQEthCommJob(Station station, MelsecQEthCommJobSettings settings)
            : base(station, settings)
        {
            _AddressType = settings.AddressType;
            _Address = settings.Address;
            _CpuTarget = settings.CpuTarget;
            _StringLength = settings.StringLength;
            _UnicodeString = settings.UnicodeString;
            if (settings.AddressType == MelsecQEthProtocol.AddressTypes.Label && MelsecQEthProtocol.IsStringJob(this) && _StringLength == 0)
            {
                _StringSettingsAutoDetect = MelsecQEthProtocol.StringSettingsAutoDetectStates.Requested;
                ForceStringSettingsAutoDetect();
            }
            AddressObj = new MelsecQAddress(_AddressType, _Address);
            _BitVariables = false;
            _PointCount = 0;
            _ArrayVariables = false;
            _CommandCode = 0;
            _AlreadyWritten = false;
            _AlreadyExchanged = false;
            _BadNotFoundStateUncertain = false;
            CheckJobValid();
        }

        public MelsecQEthCommJob(Station station, MelsecQEthTag defTag)
            : base(station, defTag)
        {
            _AddressType = defTag.MelsecQEthDynSettings.AddressType;
            _Address = defTag.MelsecQEthDynSettings.Address;
            _CpuTarget = defTag.MelsecQEthDynSettings.CpuTarget;
            _StringLength = defTag.MelsecQEthDynSettings.StringLength;
            if (AddressType == MelsecQEthProtocol.AddressTypes.Label && MelsecQEthProtocol.IsStringJob(defTag) && _StringLength == 0)
            {
                _StringSettingsAutoDetect = MelsecQEthProtocol.StringSettingsAutoDetectStates.Requested;
                ForceStringSettingsAutoDetect();
            }
            _UnicodeString = defTag.MelsecQEthDynSettings.UnicodeString;
            AddressObj = new MelsecQAddress(_AddressType, _Address);
            _BitVariables = false;
            _PointCount = 0;
            _ArrayVariables = false;
            _CommandCode = 0;
            _AlreadyWritten = false;
            _AlreadyExchanged = false;
            _BadNotFoundStateUncertain = false;

            if (TotalJobSize == 0)
            {
                if (defTag.MelsecQEthDynSettings.UnicodeString == true)
                {
                    TotalJobSize = (uint)(defTag.MelsecQEthDynSettings.StringLength) * 2;
                }
                else
                {
                    TotalJobSize = (uint)(defTag.MelsecQEthDynSettings.StringLength);
                }
            }

            if ((ElementNumber > 0 && !isProtocolBool()) || ProtocolDataSizeBig())
            {
                Tag objetBaseTag = (Tag)defTag;
                if (defTag.TagNode.ArrayDimension == 0)
                {
                    TotalJobSize = GetProtocolDataByteSize();
                }
                else
                {
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
                }
            }

            CheckJobValid();
        }

        public MelsecQEthCommJob(Station station)
            : base(station)
        {
            _Address = String.Empty;
            _CpuTarget = CpuTargets.ControlPLC;
            _StringLength = 0;
            _UnicodeString = false;
            AddressObj = new MelsecQAddress();
            _BitVariables = false;
            _PointCount = 0;
            _CommandCode = 0;
            _ArrayVariables = false;
            CheckJobValid();
        }
 
        protected MelsecQEthCommJob()
        {
            _Address = String.Empty;
            _CpuTarget = CpuTargets.ControlPLC;
            _StringLength = 0;
            _UnicodeString = false;
            AddressObj = new MelsecQAddress();
            _BitVariables = false;
            _PointCount = 0;
            _CommandCode = 0;
            _ArrayVariables = false;
            CheckJobValid();
        }
       
        #endregion

        #region Data members

        public MelsecQAddress AddressObj;

        #endregion

        #region constants
        public const int MAXBYTE_SIZE  = 480;
        #endregion

        #region Properties

        public MelsecQEthProtocol.AddressTypes _AddressType;
        public MelsecQEthProtocol.AddressTypes AddressType
        {
            get { return _AddressType; }
            set { _AddressType = value; }
        }

        /// <summary>
        /// Address
        /// </summary>
        private string _Address;
        public string Address
        {
            get
            {
                return _Address;
            }

            set
            {
                _Address = value;
            }
        }

        /// <summary>
        /// CpuTarget
        /// </summary>
        private CpuTargets _CpuTarget;
        public CpuTargets CpuTarget
        {
            get
            {
                return _CpuTarget;
            }

            set
            {
                _CpuTarget = value;
            }
        }

        /// <summary>
        /// String size
        /// </summary>
        private UInt16 _StringLength;
        public UInt16 StringLength
        {
            get
            {
                return _StringLength;
            }

            set
            {
                _StringLength = value;
            }
        }

        private bool _UnicodeString;
        public bool UnicodeString
        {
            get
            {
                return _UnicodeString;
            }

            set
            {
                _UnicodeString = value;
            }
        }

        /// <summary>
        /// BitVariables = Variables of type bit?
        /// </summary>
        private bool _BitVariables;
        public bool BitVariables
        {
            get
            {
                return _BitVariables;
            }

            set
            {
                _BitVariables = value;
            }
        }


        /// <summary>
        /// PointCount = number of device items to be exchanged
        /// </summary>
        private uint _PointCount;
        public uint PointCount
        {
            get
            {
                return _PointCount;
            }

            set
            {
                _PointCount = value;
            }
        }

        /// <summary>
        /// Command Code
        /// </summary>
        private UInt16 _CommandCode;
        public UInt16 CommandCode
        {
            get
            {
                return _CommandCode;
            }

            set
            {
                _CommandCode = value;
            }
        }

        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return string.Format("{0}DA{1:00}", ret, (uint)AddressObj.DataArea);
            }
        }

        /// <summary>
        /// BitVariables = Variables of type bit?
        /// </summary>
        private bool _ArrayVariables;
        public bool ArrayVariables
        {
            get
            {
                return _ArrayVariables;
            }

            set
            {
                _ArrayVariables = value;
            }
        }

        /// <summary>
        /// State used to understand if a value has already been written: in which case it is possible to aggregate the writing at runtime
        /// </summary>
        private bool _AlreadyWritten;
        public bool AlreadyWritten
        {
            get { return _AlreadyWritten; }
            set { _AlreadyWritten = value; }
        }

        /// <summary>
        /// State indicating if a job was already exchanged
        /// </summary>
        private bool _AlreadyExchanged;
        public bool AlreadyExchanged
        {
            get { return _AlreadyExchanged; }
            set { _AlreadyExchanged = value; }
        }

        /// <summary>
        /// State used when a multiple request failed with ErrorLabelDoesNotExist or ErrorLabelsDoesNotExist state : job need to be re evaluate individually
        /// </summary>
        private bool _BadNotFoundStateUncertain;
        public bool BadNotFoundStateUncertain
        {
            get { return _BadNotFoundStateUncertain; }
            set { _BadNotFoundStateUncertain = value; }
        }

        /// <summary>
        /// The driver requests some settings (size, unicode) from the PLC for the string type tag (if StringLength == 0)
        /// </summary>
        private MelsecQEthProtocol.StringSettingsAutoDetectStates _StringSettingsAutoDetect = MelsecQEthProtocol.StringSettingsAutoDetectStates.NotRequested;
        public MelsecQEthProtocol.StringSettingsAutoDetectStates StringSettingsAutoDetect
        {
            get { return _StringSettingsAutoDetect; }
            set { _StringSettingsAutoDetect = value; }
        }
        #endregion

        #region Override Methods

        public override void GetJobData(ref object jobData)
        {
            var listToWrite = new List<Tag>();
            var listOnWriting = new List<Tag>();
            Tag startTag = null;
            List<byte> outData = new List<byte>();

            lock (lockListObject)
            {
                listToWrite.AddRange(TagsListOnWriting);
                startTag = listToWrite[0];
                //prepare a write request
                listToWrite.Sort(CompareTagByOffset);
                Tag cand = null;
                byte[] jobdata;
                UInt16 nData = 0;
                bool bLookForFirstTag = true;

                do
                {
                    if (cand != null)
                    {
                        if ((cand.ByteOffset + cand.Size) != listToWrite[0].ByteOffset)
                            break;
                    }
                    cand = listToWrite[0];

                    listToWrite.Remove(cand);
                    if (bLookForFirstTag && cand != startTag)
                    {
                        cand = null;
                        continue;
                    }
                    bLookForFirstTag = false;

                    if (!listOnWriting.Contains(cand))
                        listOnWriting.Add(cand);
                    
                    if (isProtocolBool())
                    {
                        nData = (UInt16)((cand.Size + 7) / 8);
                        //nData = (UInt16)cand.Size;
                    }
                    else if (ElementNumber > 0 && !ProtocolDataSizeBig())
                    {
                        if (cand.TagNode.ArrayDimension == 0)
                        {
                            nData = (ushort)(GetProtocolDataByteSize());
                        }
                        else
                        {
                            nData = (ushort)(GetProtocolDataByteSize() * cand.TagNode.ArrayDimension);
                        }
                    }
                    else
                    {
                        nData = (UInt16)cand.Size;
                    }
                    lock (lockListObject)
                    {
                        cand.LastValue = cand.Value.Value;
                        jobdata = new byte[nData];
                        cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                    }
                    uint ArraySize = cand.TagNode.ArrayDimension;
                    if (ArraySize == 0)
                    {
                        ArraySize = 1;
                    }
                    if (ProtocolDataSizeBig())
                    {
                        List<byte> correctData = new List<byte>();
                        UInt16 sizeDataType = (UInt16)GetDataTypeByteSize((uint)cand.TagNode.DataType.Identifier);
                        UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                        for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                        {
                            byte[] tmpdata = new byte[sizeDataType];
                            if ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                            {
                                Array.Copy(jobdata, ArrayIndex * sizeDataType, tmpdata, 0, sizeDataType);
                            }
                            else
                            {
                                if ((1 << (ArrayIndex % 8) & jobdata[ArrayIndex / 8]) == 0)
                                {
                                    tmpdata[0] = 0;
                                }
                                else
                                {
                                    tmpdata[0] = 1;
                                }
                            }
                            /* Punto in cui viene utilizzato il Buffer locale caricato nel metodo SetJobData */
                            if (cand.TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                            {
                                byte[] jobdataTmp = new byte[sizeProtocolData];
                                byte[] tmpMemRw = new byte[sizeProtocolData];
                                int memRwArrayOffset = (int)(ArrayIndex * sizeProtocolData);
                                MelsecQEthTag tagTmp = (MelsecQEthTag)cand;
                                if (tagTmp.MemRWTag != null)
                                    if (tagTmp.MemRWTag.Length >= sizeProtocolData + memRwArrayOffset)
                                        for (int i = 0; i < sizeProtocolData; i++)
                                            tmpMemRw[i] = tagTmp.MemRWTag[i + memRwArrayOffset];
                                for (int i = 0; i < sizeProtocolData; i++)
                                {
                                    if (ElementNumber / 8 == i)
                                    {
                                        byte mask = (byte)(1 << (ElementNumber % 8));
                                        jobdataTmp[i] = (byte)((tmpMemRw[i] & (mask ^ 0xff)) | (byte)(tmpdata[0] != 0 ? mask : 0));
                                    }
                                    else
                                        jobdataTmp[i] = tmpMemRw[i];
                                }
                                tmpdata = jobdataTmp;
                            }
                            else
                            {
                                cand.getWriteValueFromMemRW(ref tmpdata, sizeDataType, sizeProtocolData, ElementNumber, ArrayIndex);
                            }

                            correctData.AddRange(tmpdata);
                        }

                        // bit is written a block of word 
                        if (MelsecQEthProtocol.IsLabelAddress(this) && cand.TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                        {
                            int nrWord = Math.DivRem((int)ArraySize, 16, out int rest);
                            if (rest > 0)
                                nrWord++;
                            // in bytes
                            correctData = correctData.Take(nrWord * 2).ToList();
                        }

                        jobdata = correctData.ToArray();                        
                    }
                    else  if (isProtocolBool())
                    {
                        if (ElementNumber > 0 && (uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                        {
                            byte[] tmpData = new byte[(ArraySize + 7) / 8];
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                if (jobdata[ArrayIndex] != 0)
                                    tmpData[ArrayIndex / 8] |= (byte)(1 << (ArrayIndex % 8));
                            }
                            jobdata = tmpData;
                        }
                    }

                    if (!isProtocolBool())
                    {
                        if (SwapBytes)
                        {
                            SwapByteBuffer(ref jobdata);
                        }

                        if (SwapWords)
                        {
                            SwapWordBuffer(ref jobdata);
                        }
                    }

                    outData.AddRange(jobdata);
                } while (listToWrite.Count > 0);

                UpdateTagsListOnWritingAndTagsListToWrite(listOnWriting);
                listOnWriting.Clear();
                listToWrite.Clear();
            }

            jobData = outData.ToArray();
        }

        public uint GetAggregateMaxJobSize()
        {
            uint AggregLimit = Station.GetCommDriver().AggregationLimit;
            uint JobMaxSize = GetMaxJobSize();
            if ((AggregLimit != 0) && (AggregLimit < JobMaxSize))
            {
                return AggregLimit;
            }
            else
            {
                return JobMaxSize;
            }
        }

        public override uint GetMaxJobSize()
        {
            return MelsecQEthProtocol.GetMaxJobSize(((MelsecQEthStation)Station).PlcType);
        }

        public static UFUAModel.DataType DataType(MelsecQAddress AddressObj, UFUAModel.DataType vartype)
        {
            switch (AddressObj.AddressType)
            {
                case MelsecQEthProtocol.AddressTypes.Label:
                    return vartype;
                    break;
                case MelsecQEthProtocol.AddressTypes.DataArea:
                    switch (AddressObj.DataArea)
                    {
                        case DataArea.X:
                        case DataArea.Y:
                        case DataArea.S:
                        case DataArea.M:
                        case DataArea.SM:
                        case DataArea.L:
                        case DataArea.B:
                        case DataArea.SB:
                        case DataArea.F:
                        case DataArea.TS:
                        case DataArea.TC:
                        case DataArea.CS:
                        case DataArea.CC:
                            if (vartype == UFUAModel.DataType.Boolean)
                                return UFUAModel.DataType.Boolean;
                            else
                                return UFUAModel.DataType.Byte;
                        default:
                            return UFUAModel.DataType.UInt16;
                    }
                    break;
                default:
                    return UFUAModel.DataType.UInt16;
                    break;
            }
        }

        public override uint getProtocolDataType()
        {
            UFUAModel.DataType vartype = UFUAModel.DataType.Byte;
            if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
            {
                vartype = UFUAModel.DataType.Boolean;
            }

            switch (DataType(AddressObj, vartype))
            {
                case UFUAModel.DataType.UInt16:
                    return (uint)BuiltInType.UInt16;
                case UFUAModel.DataType.Byte:
                    return (uint)BuiltInType.UInt16;
                case UFUAModel.DataType.Boolean:
                    return (uint)BuiltInType.Boolean;
                default:
                    return 0;
            }
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            byte[] rec = jobData as byte[];
            if (rec == null)
            {
                return;
            }           

            lock (lockListObject)
            {
                base.SetJobData(rec, ref changed);
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                uint bitNumber = 0;
                uint recByteNumber = 0;
                for (int TagIndex = 0; TagIndex < TagsList.Count; TagIndex++)
                {
                    if ((TagsList[0].TagNode.DataType.IdType == Opc.Ua.IdType.Numeric) && 
                        (TagsList[TagIndex].TagNode.DataType == Opc.Ua.DataTypes.Boolean))
                    {
                        if(CommandCode == (ushort)MelsecQCommandCode.BitUnits32bits || CommandCode == (ushort)MelsecQCommandCode.BitUnits)
                        {
                            if (TagsList[TagIndex].TagNode.ArrayDimension == 0)
                            {
                                byte[] temporaryBuffer = new byte[1];
                                if (TagsList[TagIndex].ByteOffset >= rec.Length)
                                {
                                    continue;
                                }

                                temporaryBuffer[0] = rec[TagsList[TagIndex].ByteOffset];

                                // Set the tag value
                                if (((MelsecQEthTag)(TagsList[TagIndex])).SetTagValue(ref temporaryBuffer, 0))
                                {
                                    changed.Add(TagsList[TagIndex]);
                                }
                                
                            }
                            // Array of bits
                            else
                            {
                                uint arrayDim = TagsList[TagIndex].TagNode.ArrayDimension;
                                byte[] temporaryBuffer = new byte[arrayDim];
                                for (int j = 0; j < arrayDim; j++)
                                {
                                    if (recByteNumber >= rec.Length)
                                    {
                                        continue;
                                    }
                                    temporaryBuffer[j] = rec[recByteNumber++];

                                }

                                // Set the tag value
                                if (((MelsecQEthTag)(TagsList[TagIndex])).SetTagValue(ref temporaryBuffer, 0))
                                {
                                    changed.Add(TagsList[TagIndex]);
                                }
                            }
                        }
                        else if (CommandCode == (ushort)MelsecQCommandCode.WordUnits32bits || CommandCode == (ushort)MelsecQCommandCode.WordUnits)
                        {
                            uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;

                            /* Implementando la gestione dell Element Number, non è possibile utilizzare il buffer 
                             * "MemRW" del file "Tegs.cs" della libreria base perché si sarebbe
                             * dovuto estendere le modifiche anche a questo file, la causa è l'aggregazione dei bit. 
                             * Il driver, quando  ha la possibilità di aggregare, utilizza tutti i bit di una word (16), aggregando un numero massimo di 
                             * 160 bit in 20 byte, nel caso in cui non possa aggregare ongni bit utilizza un nibble, questi due differenti 
                             * modi di costruire il frame da inviare, hanno portato a decider di rendere locare il baffer
                             * per memorizzare l'ultimo valore letto e utilizzarlo poi nel metodo GetJobData  */
                            if (ProtocolDataSizeBig())
                            { 
                                MelsecQEthTag tagTmp = (MelsecQEthTag)TagsList[TagIndex];
                                tagTmp.setMemRWTag(rec, (int)TagsList[TagIndex].ByteOffset, rec.Length);
                            }
                           
                            byte[] temporaryBuffer = new byte[1];

                            if (TagsList[TagIndex].TagNode.ArrayDimension == 0)
                            {
                                bitNumber = TagsList[TagIndex].ByteOffset;
                                byte maskbit = 0x01;
                                if (ElementNumber > 0)
                                {
                                    maskbit <<= (byte)(ElementNumber % 8);
                                    recByteNumber = (uint)(ElementNumber / 8);
                                }
                                else
                                {
                                    maskbit <<= (byte)(bitNumber % 8);
                                    recByteNumber = (bitNumber / 8);
                                }
                                temporaryBuffer[0] = (byte)(((rec[recByteNumber] & maskbit) > 0) ? 1 : 0);
                                // Set the tag value
                                if (((MelsecQEthTag)(TagsList[TagIndex])).SetTagValue(ref temporaryBuffer, 0))
                                {
                                    changed.Add(TagsList[TagIndex]);
                                }
                            }
                            else
                            {
                                byte[] tmpData = new byte[ArraySize];
                                byte maskbit = 0x01;
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    if (ElementNumber > 0)
                                    {
                                        maskbit = 0x01;
                                        maskbit <<= (byte)(ElementNumber % 8);
                                        recByteNumber = (uint)(ElementNumber / 8);
                                    } 
                                    else
                                    {
                                        maskbit = 0x01;
                                        maskbit <<= (byte)(ArrayIndex % 8);
                                        recByteNumber = (uint)(ArrayIndex / 8);                                        
                                    }
                                    tmpData[ArrayIndex] = (byte)(((rec[recByteNumber] & maskbit) > 0) ? 1 : 0);
                                }

                                if (((MelsecQEthTag)(TagsList[TagIndex])).SetTagValue(ref tmpData, 0))
                                {
                                    changed.Add(TagsList[TagIndex]);
                                }
                            }
                        }
                    }
                    else 
                    {
                        if (isProtocolBool())
                        {
                            if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
                            {
                                changed.Add(TagsList[TagIndex]);
                            }
                        }
                        else
                        {
                            if (!ProtocolDataSizeBig())
                            {
                                uint elemsize = 0;
                                if (ElementNumber > 0)
                                {
                                    elemsize = GetProtocolDataByteSize();
                                }
                                if (((MelsecQEthTag)(TagsList[TagIndex])).SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, elemsize))
                                {
                                    changed.Add(TagsList[TagIndex]);
                                }
                            }
                            else
                            {
                                UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)TagsList[TagIndex].TagNode.DataType.Identifier);
                                uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;
                                if (ArraySize == 0)
                                {
                                    ArraySize = 1;
                                }

                                byte[] tmpData = new byte[sizeTmpData * ArraySize];
                                int indexTmpData = 0;
                                TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)(sizeProtocolData * ArraySize));
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    Array.Copy(rec, TagsList[TagIndex].ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                    indexTmpData += sizeTmpData;
                                }

                                if (((MelsecQEthTag)(TagsList[TagIndex])).SetTagValue(ref tmpData, 0))
                                {
                                    changed.Add(TagsList[TagIndex]);
                                }
                            }
                        }
                    }
                }

                FirstTime = false;
            }
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            if (base.TestAggregateJob(candJob, out ExtraBytes) == JobAggregationType.JobAggregImpossible)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            MelsecQEthCommJob testJob = candJob as MelsecQEthCommJob;
            if (testJob == null)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (!AddressObj.IsValid || !testJob.AddressObj.IsValid)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (AddressObj.DataArea != testJob.AddressObj.DataArea)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType)))
            {
                return JobAggregationType.JobAggregImpossible;
            }

            NodeId nBool = new NodeId((uint)BuiltInType.Boolean);
            int boolCount = (from elem in TagsList where (uint)(elem.TagNode.DataType.Identifier) == (uint)BuiltInType.Boolean select elem).ToList().Count;
            //if ((from elem in TagsList
            //     where elem.TagNode.DataType == BuiltInType.Boolean
            //     select elem).ToList().Count > 0 && testJob.TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
            if (((boolCount > 0) && (testJob.TagsList[0].TagNode.DataType != nBool)) ||
                ((boolCount < 1) && (testJob.TagsList[0].TagNode.DataType == nBool)))
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if ((uint)testJob.TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            {
                if ((testJob.TagsList[0].TagNode.ArrayDimension > 0) || (TagsList[0].TagNode.ArrayDimension > 0))
                {
                    return JobAggregationType.JobAggregImpossible;
                }
            }

            // tag with label address cannot be aggregated
            if (MelsecQEthProtocol.IsLabelAddress(this) || MelsecQEthProtocol.IsLabelAddress(testJob))
                return JobAggregationType.JobAggregImpossible;


            ExtraBytes = 0;
            uint Granularity = Station.GetCommDriver().AggregationThreshold;
            uint start = (uint)AddressObj.StartAddress;
            uint startTest = (uint)testJob.AddressObj.StartAddress;
            uint end = 0;
            uint endTest = 0;
            DataArea DataArea = AddressObj.DataArea;
            switch (DataArea)
            {
                case DataArea.X:
                case DataArea.Y:
                case DataArea.S:
                case DataArea.M:
                case DataArea.SM:
                case DataArea.L:
                case DataArea.B:
                case DataArea.SB:
                case DataArea.F:
                case DataArea.TS:
                case DataArea.TC:
                case DataArea.CS:
                case DataArea.CC:
                    if (TagsList[0].TagNode.DataType == nBool/*BuiltInType.Boolean*/)
                    {
                        end = start + TotalJobSize - 1;
                        endTest = startTest + testJob.TotalJobSize - 1;
                    }
                    else
                    {
                        if (startTest % 8 > 0)
                        {
                            return JobAggregationType.JobAggregImpossible;
                        }
                        end = start + TotalJobSize * 8 - 1;
                        endTest = startTest + testJob.TotalJobSize * 8 - 1;
                    }
                    Granularity *= 8;
                    break;

                case DataArea.TN:
                case DataArea.CN:
                case DataArea.D:
                case DataArea.W:
                case DataArea.SW:
                case DataArea.R:
                case DataArea.SD:
                case DataArea.ZR:
                    if (testJob.TotalJobSize % 2 > 0)
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                    end = start + TotalJobSize / 2 - 1;
                    endTest = startTest + testJob.TotalJobSize / 2 - 1;
                    Granularity /= 2;
                    break;
                default:
                    return JobAggregationType.JobAggregImpossible;
            }

            uint gLimit = Granularity;

            // Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
            if (((startTest >= start) && (startTest <= end)) ||
               ((endTest >= start) && (endTest <= end)))
            {
                uint startTag = 0;
                uint endTag = 0;
                foreach (var tag in TagsList)
                {
                    switch (DataArea)
                    {
                        case DataArea.X:
                        case DataArea.Y:
                        case DataArea.S:
                        case DataArea.M:
                        case DataArea.SM:
                        case DataArea.L:
                        case DataArea.B:
                        case DataArea.SB:
                        case DataArea.F:
                        case DataArea.TS:
                        case DataArea.TC:
                        case DataArea.CS:
                        case DataArea.CC:
                            if (TagsList[0].TagNode.DataType == nBool/*BuiltInType.Boolean*/)
                            {
                                startTag = start + tag.ByteOffset;
                                endTag = startTag + tag.Size - 1;
                            }
                            else
                            {
                                startTag = start + tag.ByteOffset * 8;
                                endTag = startTag + tag.Size * 8 - 1;
                            }
                            break;

                        case DataArea.TN:
                        case DataArea.CN:
                        case DataArea.D:
                        case DataArea.W:
                        case DataArea.SW:
                        case DataArea.R:
                        case DataArea.SD:
                        case DataArea.ZR:
                            startTag = start + tag.ByteOffset / 2;
                            endTag = startTag + tag.Size / 2 - 1;
                            break;

                        default:
                            return JobAggregationType.JobAggregImpossible;
                    }

                    if ((startTest >= startTag && startTest <= endTag)
                        || (endTest >= startTag && endTest <= endTag))
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                }
            }

            else if ((startTest < start) && (endTest > end))
            {
                return JobAggregationType.JobAggregImpossible;
            }

            // Check granularity
            else
            {
                if (TagsList[0].TagNode.DataType == nBool)
                {
                    gLimit *= 8;
                }
                if (endTest < start)
                {
                    if ((start - endTest) > (gLimit))
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                }
                else if ((startTest - end) > (gLimit))
                {
                    return JobAggregationType.JobAggregImpossible;
                }
            }

            // Calculate the new offset for the job tags
            uint newoffset = 0;
            if (startTest >= start)
            {
                newoffset = startTest - start;
                switch (DataArea)
                {
                    case DataArea.X:
                    case DataArea.Y:
                    case DataArea.S:
                    case DataArea.M:
                    case DataArea.SM:
                    case DataArea.L:
                    case DataArea.B:
                    case DataArea.SB:
                    case DataArea.F:
                    case DataArea.TS:
                    case DataArea.TC:
                    case DataArea.CS:
                    case DataArea.CC:
                        if (TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
                        {
                            newoffset /= 8;
                        }
                    break;

                    case DataArea.TN:
                    case DataArea.CN:
                    case DataArea.D:
                    case DataArea.W:
                    case DataArea.SW:
                    case DataArea.R:
                    case DataArea.SD:
                    case DataArea.ZR:
                        newoffset *= 2;
                    break;
                }
            }

            if ((startTest >= start) && (endTest <= end))
            {
                for (int i = 0; i < candJob.TagsList.Count; i++)
                {
                    candJob.TagsList[i].ByteOffset += newoffset;
                }

                return JobAggregationType.JobAggregFits;
            }

            // Calculate and check the new job total size
            uint newStartAddress = start;
            if (newStartAddress > startTest)
            {
                newStartAddress = startTest;
            }
            uint newEndAddress = end;
            if (newEndAddress < endTest)
            {
                newEndAddress = endTest;
            }
            uint newJobSize = 0;
            switch (DataArea)
            {
                case DataArea.X:
                case DataArea.Y:
                case DataArea.S:
                case DataArea.M:
                case DataArea.SM:
                case DataArea.L:
                case DataArea.B:
                case DataArea.SB:
                case DataArea.F:
                case DataArea.TS:
                case DataArea.TC:
                case DataArea.CS:
                case DataArea.CC:
                    if (TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
                    {
                        newJobSize = (newEndAddress - newStartAddress + 1) / 8;
                    }
                    else
                    {
                        newJobSize = newEndAddress - newStartAddress + 1;
                    }
                break;

                case DataArea.TN:
                case DataArea.CN:
                case DataArea.D:
                case DataArea.W:
                case DataArea.SW:
                case DataArea.R:
                case DataArea.SD:
                case DataArea.ZR:
                    newJobSize = (newEndAddress - newStartAddress + 1) * 2;
                break;

            }
            if (newJobSize > GetAggregateMaxJobSize())
            {
                return JobAggregationType.JobAggregImpossible;
            }

            // The job can aggregate the new data. Calculate the extension required
            ExtraBytes = newJobSize - TotalJobSize;

            // Aggregate forward
            if (startTest >= start)
            {
                for (int i = 0; i < candJob.TagsList.Count; i++)
                {
                    candJob.TagsList[i].ByteOffset += newoffset;
                }

                return JobAggregationType.JobAggregForward;
            }

            // Aggregate backward
            else
            {
                return JobAggregationType.JobAggregBackward;
            }
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            // Fits: add tag with correct offset
            // Forward: tag added extend job to higher addresses. Calculate new job size, address doesn't change
            // Backward: tag added extend job to lower addresses. Calculate new job size and new address
            switch (AggType)
            {
                case JobAggregationType.JobAggregFits:
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        TagsList.Add(new MelsecQEthTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
                    }
                break;

                case JobAggregationType.JobAggregForward:
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        TagsList.Add(new MelsecQEthTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
                    }
                    TotalJobSize += ExtraBytes;
                break;

                case JobAggregationType.JobAggregBackward:
                    foreach (var tag in TagsList)
                    {
                        tag.ByteOffset += ExtraBytes;
                    }
 
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        TagsList.Add(new MelsecQEthTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
                    }
                    TotalJobSize += ExtraBytes;
                    Address = ((MelsecQEthCommJob)candJob).Address;
                    AddressObj.Set(AddressType, Address);
                break;

                default:
                    return false;
            }
            return true;
        }

        public override bool IsJobAggregable()
        {
            return (_AddressType == MelsecQEthProtocol.AddressTypes.DataArea);
        }

        #endregion

        #region Specific Methods

        private static int CompareTagByOffset(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                {
                    return 0; //==
                }
                else
                {
                    return -1;// x < y
                }
            }
            else
            {
                //x!= null
                if (y == null)
                {
                    return 1; //x > y
                }
                else
                {
                    if (x.ByteOffset > y.ByteOffset)
                    {
                        return 1;
                    }
                    else if (x.ByteOffset == y.ByteOffset)
                    {
                        return 0;
                    }
                    else
                    {
                        return -1;
                    }
                }
            }
        }

        public bool IsTypeAdmitted(NodeId type)
        {
            if (type.IdType == IdType.Numeric)
            {
                uint nType = (uint)type.Identifier;
                if (nType == (uint)BuiltInType.Boolean ||
                    nType == (uint)BuiltInType.Byte ||
                    nType == (uint)BuiltInType.Double ||
                    nType == (uint)BuiltInType.Float ||
                    nType == (uint)BuiltInType.Int16 ||
                    nType == (uint)BuiltInType.Int32 ||
                    nType == (uint)BuiltInType.Int64 ||
                    nType == (uint)BuiltInType.Integer ||
                    nType == (uint)BuiltInType.SByte ||
                    nType == (uint)BuiltInType.UInt16 ||
                    nType == (uint)BuiltInType.UInt32 ||
                    nType == (uint)BuiltInType.UInt64 ||
                    nType == (uint)BuiltInType.UInteger||
                    nType == (uint)BuiltInType.String)

                {
                    return true;
                }

                return false;
            }

            return false;
        }

       uint GetTagSize(DriverBaseInterfaces.TagDefinition t)
        {
            if (t.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)t.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        if (t.ArrayDimension == 0)
                        {
                            return 1;
                        }
                        else
                        {
                            return (t.ArrayDimension);
                        }
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        if (t.ArrayDimension == 0)
                        {
                            return 2;
                        }
                        else
                        {
                            return (t.ArrayDimension * 2);
                        }
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        if (t.ArrayDimension == 0)
                        {
                            return 4;
                        }
                        else
                        {
                            return (t.ArrayDimension * 4);
                        }
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        if (t.ArrayDimension == 0)
                        {
                            return 8;
                        }
                        else
                        {
                            return (t.ArrayDimension * 8);
                        }
                    default:
                        return 0;
                }
            }
            return 0;
        }

        private void CheckJobValid()
        {
            if (AddressType == MelsecQEthProtocol.AddressTypes.DataArea)
                CheckJobValidDataArea();
            else
                CheckJobValidLabel();
        }

        private void CheckJobValidDataArea()
        {
            _BitVariables = false;
            _ArrayVariables = false;
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList =
                                new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                if (!IsTypeAdmitted(t.TagNode.DataType) &&
                    t.DynSettings.MethodID == -1)
                {
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.ErrorInvalidTagType,
                                                  t.TagNode.NodeId.ToString(),
                                                  t.TagNode.DataType.Identifier.ToString());
                    return;
                }
                tagnamelist += (tagnamelist.Length > 0 ? ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            if (!AddressObj.IsValid)
            {
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.ErrorInvalidAssignedAddress,
                                                Address);
                return;
            }

            UFUAModel.DataType vartype = UFUAModel.DataType.Byte;
            if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
            {
                vartype = UFUAModel.DataType.Boolean;
            }

            if (MelsecQEthDynTagSettings.CheckVariablesSize(AddressObj, DataType(AddressObj, vartype)))
            {
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.ErrorInvalidAssignedAddress,
                                                Address);
                return;
            }

            if ((TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.String) && (TagsList[0].TagNode.ArrayDimension != 0))
            {
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.ErrorStringArraryNotSupported);
                return;
            }

            bool arrayVar = false;
            uint varSize = 0;
            uint totalSize = 0;
            foreach (var t in tempList)
            {
                varSize = GetTagSize(t);
                if (t.ArrayDimension > 0)
                {
                    arrayVar = true;
                }
                if (isProtocolBool())
                {
                    _BitVariables = true;
                }
                totalSize += varSize;
            }

            if (arrayVar)
            {
                _ArrayVariables = true;
            }

            DataArea DataArea = AddressObj.DataArea;
            if (DataArea == DataArea.X &&
                Type != LinkType.Input)
            {
                // Input type required
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.ErrorInputTypeRequired, tagnamelist, DriverCodeBaseEx.Properties.Resources.LinkType_Input);
                return;
            }

            if (GetDataFrameLength() > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format(
                                           Properties.Resources.ErrorJobTooBig,
                                           tagnamelist);
                return;
            }

            string errorDesc;
            if (!ProtocolDataSizeIsValid(out errorDesc))
            {
                IsValid = false;
                InvalidReason = string.Format("The {0} is invalid for the Function Code. (Tags: {1} Data Area: {2})", errorDesc, tagnamelist, DataArea);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public void CheckJobValidLabel()
        {
            // set if job contain array
            _ArrayVariables = (TagsList.Count(t => t.TagNode.ArrayDimension > 0) > 0);

            if (!MelsecQEthProtocol.PlcSupportLabelAddress(((MelsecQEthStation)Station).PlcType))
            {
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.ErrorPlcDontSupportLabelAddress, Station.Name);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public uint GetDataFrameLength()
        {
            uint DataFrameLength = 0;
            uint StartAddress = (uint)AddressObj.StartAddress;
            if (BitVariables)
            {
                if ((TotalJobSize % 16 > 0) || (StartAddress % 16 > 0))
                {
                    DataFrameLength = (uint)((TotalJobSize + 1) / 2);
                }
                else
                {
                    DataFrameLength = (TotalJobSize / 16) * 2;
                }
            }
            else
            {
                DataFrameLength = (TotalJobSize / 2) * 2;
                switch (AddressObj.DataArea)
                {
                    case DataArea.X:
                    case DataArea.Y:
                    case DataArea.S:
                    case DataArea.M:
                    case DataArea.SM:
                    case DataArea.L:
                    case DataArea.B:
                    case DataArea.SB:
                    case DataArea.F:
                    case DataArea.TS:
                    case DataArea.TC:
                    case DataArea.CS:
                    case DataArea.CC:
                        //if (job.TotalJobSize == 1)
                        //{
                        //    DataFrameLength = 4;
                        //}
                        DataFrameLength = TotalJobSize ;
                        break;
                }
            }

            return DataFrameLength;
        }

        public bool ParseData(byte[] receiveBuffer, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);

            if (areArguments)
            {
                BuiltInType bt = Station.GetBuiltInType(items[0].GetType());
                if (bt != BuiltInType.Byte && bt != BuiltInType.Double &&
                    bt != BuiltInType.Float && bt != BuiltInType.Int16 &&
                    bt != BuiltInType.Int32 && bt != BuiltInType.Int64 &&
                    bt != BuiltInType.Integer && bt != BuiltInType.Number &&
                    bt != BuiltInType.SByte && bt != BuiltInType.UInt16 &&
                    bt != BuiltInType.UInt32 && bt != BuiltInType.UInt64 &&
                    bt != BuiltInType.UInteger)
                {
                    return false;
                }

                if (CommandCode == (ushort)MelsecQCommandCode.BitUnits32bits || CommandCode == (ushort)MelsecQCommandCode.BitUnits || CommandCode == (ushort)MelsecQCommandCode.WordUnits32bits || CommandCode == (ushort)MelsecQCommandCode.WordUnits)
                {
                    if (items.Count < 2)
                    {
                        items[0] = 11;
                        return false;
                    }
                }
            }

            byte[] jobdata;
            int byteCount = receiveBuffer.Length;
            if (_AddressType == MelsecQEthProtocol.AddressTypes.DataArea)
            {                
                switch (CommandCode)
                {
                    case (ushort)MelsecQCommandCode.BitUnits32bits:
                    case (ushort)MelsecQCommandCode.BitUnits:                    
                    case (ushort)MelsecQCommandCode.WordUnits32bits:
                    case (ushort)MelsecQCommandCode.WordUnits:
                        {
                            jobdata = new byte[byteCount];
                            receiveBuffer.ToList().CopyTo(0, jobdata, 0, byteCount);
                            SetJobData(jobdata, ref changed);
                            if (areArguments)
                            {
                                if (TagsList.Count == items.Count - 1)
                                {
                                    for (int k = 0; k < TagsList.Count; k++)
                                    {
                                        items[k + 1] = TagsList[k].Value.Value;
                                    }
                                }
                            }
                            else
                            {
                                items.AddRange(changed);
                            }
                        }
                        break;
                }
            }
            else
            {
                jobdata = new byte[byteCount];
                receiveBuffer.ToList().CopyTo(0, jobdata, 0, byteCount);
                SetJobData(jobdata, ref changed);
                if (areArguments)
                {
                    if (TagsList.Count == items.Count - 1)
                    {
                        for (int k = 0; k < TagsList.Count; k++)
                        {
                            items[k + 1] = TagsList[k].Value.Value;
                        }
                    }
                }
                else
                {
                    items.AddRange(changed);
                }
            }

            return true;
        }

        public uint GetStationNetworkNumber()
        {
            return (uint) (Station == null ? 0 : ((MelsecQEthStation)Station).NetworkNumber);
        }
        public uint GetStationPcNumber()
        {
            return (uint)(Station == null ? 0 : ((MelsecQEthStation)Station).PcNumber);
        }

        public uint GetLabelTotalJobSize()
        {
            return (MelsecQEthProtocol.IsStringJob(this) ? MelsecQEthProtocol.GetStringJobSize(this) : this.TotalJobSize);
        }

        private void ForceStringSettingsAutoDetect()
        {            
            _StringLength = 250;
            TotalJobSize = 250;
            TagsList[0].Size = _StringLength;
            _UnicodeString = false;
        }

        public void SetStringSettings(int size, bool unicodeString, uint arrayDimension)
        {
            _StringLength = (UInt16)size;
            TotalJobSize = (arrayDimension == 0 ? (uint)size : (uint)(size * arrayDimension));
            TagsList[0].Size = (arrayDimension == 0 ? _StringLength : (uint)(_StringLength * arrayDimension));
            UnicodeString = unicodeString;
            ((MelsecQEthTag)TagsList[0]).UnicodeString = unicodeString;
            if (unicodeString)
                _StringLength /= 2;
        }
        #endregion
    }
}
