using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace MelsecFXTCP
{
    public enum MelsecFXCommandCode : ushort
    {
	    Command_BatchRead_BitUnits = 0,
	    Command_BatchRead_WordUnits = 1,
	    Command_BatchWrite_BitUnits = 2,
	    Command_BatchWrite_WordUnits = 3
    };

    public class MelsecFXTCPCommJob : CommJob
    {
        #region Constructors

        public MelsecFXTCPCommJob(Station station, MelsecFXTCPCommJobSettings settings)
            : base(station, settings)
        {
            _Address = settings.Address;
            AddressObj = new MelsecFXAddress(_Address);
            _BitVariables = false;
            _BitArrayVariables = false;
            _PointCount = 0;
            _ArrayVariables = false;
            _CommandCode = 0;
            CheckJobValid();
        }

        public MelsecFXTCPCommJob(Station station, MelsecFXTCPTag defTag)
            : base(station, defTag)
        {
            _Address = defTag.MelsecFXTCPDynSettings.Address;
            AddressObj = new MelsecFXAddress(_Address);
            _BitVariables = false;
            _BitArrayVariables = false;
            _PointCount = 0;
            _ArrayVariables = false;
            _CommandCode = 0;

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

        public MelsecFXTCPCommJob(Station station)
            : base(station)
        {
            _Address = String.Empty;
            AddressObj = new MelsecFXAddress();
            _BitVariables = false;
            _BitArrayVariables = false;
            _PointCount = 0;
            _CommandCode = 0;
            _ArrayVariables = false;
            CheckJobValid();
        }
 
        protected MelsecFXTCPCommJob()
        {
            _Address = String.Empty;
            AddressObj = new MelsecFXAddress();
            _BitVariables = false;
            _BitArrayVariables = false;
            _PointCount = 0;
            _CommandCode = 0;
            _ArrayVariables = false;
            CheckJobValid();
        }
       
        #endregion

        #region Data members

        public MelsecFXAddress AddressObj;

        #endregion

        #region Properties

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
        /// BitArrayVariables = Variables of type array of bit?
        /// </summary>
        private bool _BitArrayVariables;
        public bool BitArrayVariables
        {
            get
            {
                return _BitArrayVariables;
            }

            set
            {
                _BitArrayVariables = value;
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
        /// ExpectedReplyLength = Expected Reply Length
        /// </summary>
        private uint _ExpectedReplyLength;
        public uint ExpectedReplyLength
        {
            get
            {
                return _ExpectedReplyLength;
            }

            set
            {
                _ExpectedReplyLength = value;
            }
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
                                MelsecFXTCPTag tagTmp = (MelsecFXTCPTag)cand;
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
            if (_BitVariables == true)
            {
                AggregLimit *= 8;
            }
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
            // Modified in version 2.2.30.0 (FOGBUGZ 15458)
            //return 128;
            if (_BitVariables)
            {
                return 160;
            }
            else
            {
                return 128;
            }
        }

        public static UFUAModel.DataType DataType(MelsecFXAddress AddressObj, UFUAModel.DataType vartype)
        {
            switch (AddressObj.DataArea)
            {
                case MelsecFXDataArea.DataArea_X:
                case MelsecFXDataArea.DataArea_Y:
                case MelsecFXDataArea.DataArea_S:
                case MelsecFXDataArea.DataArea_B:
                case MelsecFXDataArea.DataArea_F:
                case MelsecFXDataArea.DataArea_L:
                case MelsecFXDataArea.DataArea_M:
                case MelsecFXDataArea.DataArea_M_Special:
                case MelsecFXDataArea.DataArea_CS:
                case MelsecFXDataArea.DataArea_TS:
                    if (vartype == UFUAModel.DataType.Boolean)
                    {
                        return UFUAModel.DataType.Boolean;
                    }
                    else
                    {
                        return UFUAModel.DataType.Byte;
                    }

                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_D_Special:
                case MelsecFXDataArea.DataArea_R:
                case MelsecFXDataArea.DataArea_W:
                case MelsecFXDataArea.DataArea_TN:
                case MelsecFXDataArea.DataArea_CN:
                case MelsecFXDataArea.DataArea_CS_16:
                case MelsecFXDataArea.DataArea_CN_16:
                    {
                        return UFUAModel.DataType.UInt16;
                    }

                case MelsecFXDataArea.DataArea_CN_32:
                    {
                        return UFUAModel.DataType.UInt32;
                    }

                default:
                    return 0;
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
                case UFUAModel.DataType.UInt32:
                    return (uint)BuiltInType.UInt32;
                case UFUAModel.DataType.UInt16:
                    return (uint)BuiltInType.UInt16;
                case UFUAModel.DataType.Byte:
                    return (uint)BuiltInType.Byte;
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
                        if(CommandCode == (ushort)MelsecFXCommandCode.Command_BatchRead_BitUnits)
                        {
                            if (TagsList[TagIndex].TagNode.ArrayDimension == 0)
                            {
                                byte[] temporaryBuffer = new byte[1];
                                bitNumber = TagsList[TagIndex].ByteOffset;
                                recByteNumber = bitNumber / 2;
                                if (recByteNumber >= rec.Length)
                                {
                                    continue;
                                }

                                // First nibble
                                if (bitNumber % 2 == 0)
                                {
                                    temporaryBuffer[0] = (byte)(((rec[recByteNumber] & 0xF0) > 0) ? 1 : 0);
                                }

                                // Second nibble
                                else
                                {
                                    temporaryBuffer[0] = (byte)(((rec[recByteNumber] & 0x0F) > 0) ? 1 : 0);
                                }
                                //}
                                // Set the tag value
                                if (TagsList[TagIndex].SetTagValue(ref temporaryBuffer, 0))
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
                                    bitNumber = (uint)(TagsList[TagIndex].ByteOffset + j);
                                    recByteNumber = bitNumber / 2;

                                    // First nibble
                                    if (bitNumber % 2 == 0)
                                    {
                                        temporaryBuffer[j] = (byte)(((rec[recByteNumber] & 0xF0) > 0) ? 1 : 0);
                                    }

                                    // Second nibble
                                    else
                                    {
                                        temporaryBuffer[j] = (byte)(((rec[recByteNumber] & 0x0F) > 0) ? 1 : 0);
                                    }
                                }

                                // Set the tag value
                                if (TagsList[TagIndex].SetTagValue(ref temporaryBuffer, 0))
                                {
                                    changed.Add(TagsList[TagIndex]);
                                }
                            }
                        }
                        else if (CommandCode == (ushort)MelsecFXCommandCode.Command_BatchRead_WordUnits)
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
                                MelsecFXTCPTag tagTmp = (MelsecFXTCPTag)TagsList[TagIndex];
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
                                if (TagsList[TagIndex].SetTagValue(ref temporaryBuffer, 0))
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

                                if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
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
                                if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, elemsize))
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

                                if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
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

            MelsecFXTCPCommJob testJob = candJob as MelsecFXTCPCommJob;
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

            ExtraBytes = 0;
            uint Granularity = Station.GetCommDriver().AggregationThreshold;
            uint start = (uint)AddressObj.StartAddress;
            uint startTest = (uint)testJob.AddressObj.StartAddress;
            uint end = 0;
            uint endTest = 0;
            MelsecFXDataArea DataArea = AddressObj.DataArea;
            switch (DataArea)
            {
                case MelsecFXDataArea.DataArea_X:
                case MelsecFXDataArea.DataArea_Y:
                case MelsecFXDataArea.DataArea_M:
                case MelsecFXDataArea.DataArea_S:
                case MelsecFXDataArea.DataArea_L:
                case MelsecFXDataArea.DataArea_B:
                case MelsecFXDataArea.DataArea_F:
                case MelsecFXDataArea.DataArea_TS:
                case MelsecFXDataArea.DataArea_TC:
                case MelsecFXDataArea.DataArea_CS:
                case MelsecFXDataArea.DataArea_CC:
                case MelsecFXDataArea.DataArea_M_Special:
                case MelsecFXDataArea.DataArea_CS_16:
                case MelsecFXDataArea.DataArea_CS_32:
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

                case MelsecFXDataArea.DataArea_TN:
                case MelsecFXDataArea.DataArea_CN:
                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_W:
                case MelsecFXDataArea.DataArea_R:
                case MelsecFXDataArea.DataArea_C:
                case MelsecFXDataArea.DataArea_D_Special:
                case MelsecFXDataArea.DataArea_CN_16:
                    if (testJob.TotalJobSize % 2 > 0)
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                    end = start + TotalJobSize/2 - 1;
                    endTest = startTest + testJob.TotalJobSize/2 - 1;
                    Granularity /= 2;
                break;

                case MelsecFXDataArea.DataArea_CN_32:
                    if (testJob.TotalJobSize % 4 > 0)
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                    end = start + TotalJobSize / 4 - 1;
                    endTest = startTest + testJob.TotalJobSize / 4 - 1;
                    Granularity /= 4;
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
                        case MelsecFXDataArea.DataArea_X:
                        case MelsecFXDataArea.DataArea_Y:
                        case MelsecFXDataArea.DataArea_M:
                        case MelsecFXDataArea.DataArea_S:
                        case MelsecFXDataArea.DataArea_L:
                        case MelsecFXDataArea.DataArea_B:
                        case MelsecFXDataArea.DataArea_F:
                        case MelsecFXDataArea.DataArea_TS:
                        case MelsecFXDataArea.DataArea_TC:
                        case MelsecFXDataArea.DataArea_CS:
                        case MelsecFXDataArea.DataArea_CC:
                        case MelsecFXDataArea.DataArea_M_Special:
                        case MelsecFXDataArea.DataArea_CS_16:
                        case MelsecFXDataArea.DataArea_CS_32:
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

                        case MelsecFXDataArea.DataArea_TN:
                        case MelsecFXDataArea.DataArea_CN:
                        case MelsecFXDataArea.DataArea_D:
                        case MelsecFXDataArea.DataArea_W:
                        case MelsecFXDataArea.DataArea_R:
                        case MelsecFXDataArea.DataArea_C:
                        case MelsecFXDataArea.DataArea_D_Special:
                        case MelsecFXDataArea.DataArea_CN_16:
                            startTag = start + tag.ByteOffset / 2;
                            endTag = startTag + tag.Size / 2 - 1;
                            break;

                        case MelsecFXDataArea.DataArea_CN_32:
                            startTag = start + tag.ByteOffset / 4;
                            endTag = startTag + tag.Size / 4 - 1;
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
                    if ((start - endTest) > (gLimit + 1))
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                }
                else if ((startTest - end) > (gLimit + 1))
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
                    case MelsecFXDataArea.DataArea_X:
                    case MelsecFXDataArea.DataArea_Y:
                    case MelsecFXDataArea.DataArea_M:
                    case MelsecFXDataArea.DataArea_S:
                    case MelsecFXDataArea.DataArea_L:
                    case MelsecFXDataArea.DataArea_B:
                    case MelsecFXDataArea.DataArea_F:
                    case MelsecFXDataArea.DataArea_TS:
                    case MelsecFXDataArea.DataArea_TC:
                    case MelsecFXDataArea.DataArea_CS:
                    case MelsecFXDataArea.DataArea_CC:
                    case MelsecFXDataArea.DataArea_M_Special:
                    case MelsecFXDataArea.DataArea_CS_16:
                    case MelsecFXDataArea.DataArea_CS_32:
                        if (TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
                        {
                            newoffset /= 8;
                        }
                    break;

                    case MelsecFXDataArea.DataArea_TN:
                    case MelsecFXDataArea.DataArea_CN:
                    case MelsecFXDataArea.DataArea_D:
                    case MelsecFXDataArea.DataArea_W:
                    case MelsecFXDataArea.DataArea_R:
                    case MelsecFXDataArea.DataArea_C:
                    case MelsecFXDataArea.DataArea_D_Special:
                    case MelsecFXDataArea.DataArea_CN_16:
                        newoffset *= 2;
                    break;

                    case MelsecFXDataArea.DataArea_CN_32:
                        newoffset *= 4;
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
                case MelsecFXDataArea.DataArea_X:
                case MelsecFXDataArea.DataArea_Y:
                case MelsecFXDataArea.DataArea_M:
                case MelsecFXDataArea.DataArea_S:
                case MelsecFXDataArea.DataArea_L:
                case MelsecFXDataArea.DataArea_B:
                case MelsecFXDataArea.DataArea_F:
                case MelsecFXDataArea.DataArea_TS:
                case MelsecFXDataArea.DataArea_TC:
                case MelsecFXDataArea.DataArea_CS:
                case MelsecFXDataArea.DataArea_CC:
                case MelsecFXDataArea.DataArea_M_Special:
                case MelsecFXDataArea.DataArea_CS_16:
                case MelsecFXDataArea.DataArea_CS_32:
                    if (TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
                    {
                        newJobSize = (newEndAddress - newStartAddress + 1) / 8;
                    }
                    else
                    {
                        newJobSize = newEndAddress - newStartAddress + 1;
                    }
                break;

                case MelsecFXDataArea.DataArea_TN:
                case MelsecFXDataArea.DataArea_CN:
                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_W:
                case MelsecFXDataArea.DataArea_R:
                case MelsecFXDataArea.DataArea_C:
                case MelsecFXDataArea.DataArea_D_Special:
                case MelsecFXDataArea.DataArea_CN_16:
                    newJobSize = (newEndAddress - newStartAddress + 1) * 2;
                break;

                case MelsecFXDataArea.DataArea_CN_32:
                    newJobSize = (newEndAddress - newStartAddress + 1) * 4;
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
                        TagsList.Add(new MelsecFXTCPTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
                    }
                break;

                case JobAggregationType.JobAggregForward:
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        TagsList.Add(new MelsecFXTCPTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
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
                        TagsList.Add(new MelsecFXTCPTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
                    }
                    TotalJobSize += ExtraBytes;
                    Address = ((MelsecFXTCPCommJob)candJob).Address;
                    AddressObj.Set(Address);
                break;

                default:
                    return false;
            }
            return true;
        }

        public override bool InputOutputRequiresReadAfterContinuosWrite()
        {
            return base.InputOutputRequiresReadAfterContinuosWriteBase();
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
                    nType == (uint)BuiltInType.UInteger)
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
            _BitVariables = false;
            _ArrayVariables = false;
            _BitArrayVariables = false;
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
                InvalidReason = string.Format( Properties.Resources.ErrorInvalidAssignedAddress,
                                                Address);
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

            MelsecFXDataArea DataArea = AddressObj.DataArea;
            if (DataArea == MelsecFXDataArea.DataArea_X &&
                Type != LinkType.Input)
            {
                // Input type required
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.ErrorInputTypeRequired,tagnamelist, DriverCodeBaseEx.Properties.Resources.LinkType_Input);
                return;
            }

            if (TotalJobSize > GetMaxJobSize())
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

                if ((CommandCode == (ushort)MelsecFXCommandCode.Command_BatchRead_BitUnits) ||
                    (CommandCode == (ushort)MelsecFXCommandCode.Command_BatchRead_WordUnits))
                {
                    if (items.Count < 2)
                    {
                        items[0] = 11;
                        return false;
                    }
                }
            }
            
            int byteCount = receiveBuffer.Length;
            switch (CommandCode)
            {
                case (ushort)MelsecFXCommandCode.Command_BatchRead_BitUnits:
                case (ushort)MelsecFXCommandCode.Command_BatchRead_WordUnits:
                {
                    byte[] jobdata = new byte[byteCount];
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

                case (ushort)MelsecFXCommandCode.Command_BatchWrite_BitUnits:
                case (ushort)MelsecFXCommandCode.Command_BatchWrite_WordUnits:
                break;
            }

            return true;
        }

        #endregion
    }
}
