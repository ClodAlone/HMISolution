using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace MelsecFX
{
    public enum MelsecFXCommandCode : byte
    {
        Command_Invalid = 0,
        Command_Read_Bytes = 0x30,
        Command_Write_Bytes = 0x31,
        Command_Write_BitOn = 0x37,
        Command_Write_BitOff = 0x38
    };

    public class MelsecFXCommJob : CommJob
    {
        #region Constructors

        public MelsecFXCommJob(Station station, MelsecFXCommJobSettings settings)
            : base(station, settings)
        {
            _Address = settings.Address;
            AddressObj = new MelsecFXAddress(_Address);
            _BitVariables = false;
            _CommandCode = (byte)MelsecFXCommandCode.Command_Invalid;
            _ArrayVariables = false;
            CheckJobValid();
        }

        public MelsecFXCommJob(Station station, MelsecFXTag defTag)
            : base(station, defTag)
        {
            //NodeId n = new NodeId(new Guid("00000000000000000000000000000000"));
            //if (defTag.TagNode.DataType.Identifier.ToString() == n.Identifier.ToString())
            //{
            //    List<DriverBaseInterfaces.TagDefinition> tList = new List<DriverBaseInterfaces.TagDefinition>();
            //    station.GetCommDriver().OnTagPrototypeQuery(defTag.TagNode.NodeId, ref tList);
            //    foreach (var tag in tList)
            //    {
            //        System.Diagnostics.Trace.TraceInformation(string.Format("member: {2} type:{0} Settings:{1}",
            //                                                                tag.DataType.Identifier,
            //                                                                tag.DynamicSettings,
            //                                                                tag.NodeId.ToString()));
            //    }
            //}

            _Address = defTag.MelsecFXDynSettings.Address;
            AddressObj = new MelsecFXAddress(_Address);
            _BitVariables = false;
            _ArrayVariables = false;
            _CommandCode = (byte)MelsecFXCommandCode.Command_Invalid;

            if ((ElementNumber > 0 && !isProtocolBool()) || ProtocolDataSizeBig())
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }

            CheckJobValid();
        }

        public MelsecFXCommJob(Station station)
            : base(station)
        {
            _Address = String.Empty;
            AddressObj = new MelsecFXAddress();
            _BitVariables = false;
            _ArrayVariables = false;
            _CommandCode = (byte)MelsecFXCommandCode.Command_Invalid;
            CheckJobValid();
        }
 
        protected MelsecFXCommJob()
        {
            _Address = String.Empty;
            AddressObj = new MelsecFXAddress();
            _BitVariables = false;
            _ArrayVariables = false;
            _CommandCode = (byte)MelsecFXCommandCode.Command_Invalid;
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

        /// <summary>
        /// Command Code
        /// </summary>
        private byte _CommandCode;
        public byte CommandCode
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

        #endregion

        #region Override Methods

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

        public static uint GetMaxJobSize(bool isBitVariables)
        {
            uint maxJobSize = 64;
            if (isBitVariables == true)
            {
                maxJobSize *= 8;
            }
            return maxJobSize;
        }

        public static UFUAModel.DataType DataType(MelsecFXAddress AddressObj,UFUAModel.DataType vartype)
        {
            switch (AddressObj.DataArea)
            {
                case MelsecFXDataArea.DataArea_X:
                case MelsecFXDataArea.DataArea_Y:
                case MelsecFXDataArea.DataArea_M:
                case MelsecFXDataArea.DataArea_S:
                case MelsecFXDataArea.DataArea_M_Special:
                    if (vartype ==  UFUAModel.DataType.Boolean)
                        return UFUAModel.DataType.Boolean;
                    else
                        return UFUAModel.DataType.Byte;

                case MelsecFXDataArea.DataArea_TN:
                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_D_Special:
                case MelsecFXDataArea.DataArea_C_16:
                    return UFUAModel.DataType.UInt16;

                case MelsecFXDataArea.DataArea_C_32:
                    return UFUAModel.DataType.UInt32;
                default:
                    return 0;
            }
        }

        public override uint getProtocolDataType()
        {
            UFUAModel.DataType vartype = UFUAModel.DataType.Byte;
            if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                vartype = UFUAModel.DataType.Boolean;
            switch (DataType(AddressObj,vartype))
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
       
        public override uint GetMaxJobSize()
        {
            return GetMaxJobSize(_BitVariables);
        }

        public override void GetJobData(ref object jobData)
        {
            var listToWrite = new List<Tag>();
            var listOnWriting = new List<Tag>();

            lock (lockListObject)
            {
                listToWrite.AddRange(TagsListToWrite);
                TagsListToWrite.Clear();
            }

            List<byte> outData = new List<byte>();
            //prepare a write request
            listToWrite.Sort(CompareTagByOffset);
            Tag cand = null;
            byte[] jobdata;
            UInt16 nData = 0;
            do
            {
                // Driver specific: write just one bit per time
                if (BitVariables && !ArrayVariables && listOnWriting.Count != 0 &&
                    ((AddressObj.DataArea == MelsecFXDataArea.DataArea_X) ||
                     (AddressObj.DataArea == MelsecFXDataArea.DataArea_Y) ||
                     (AddressObj.DataArea == MelsecFXDataArea.DataArea_M) ||
                     (AddressObj.DataArea == MelsecFXDataArea.DataArea_S) ||
                     (AddressObj.DataArea == MelsecFXDataArea.DataArea_M_Special)))
                    break;

                if (cand != null)
                {
                    if ((cand.ByteOffset + cand.Size) != listToWrite[0].ByteOffset)
                        break;
                }
                cand = listToWrite[0];
                listToWrite.Remove(cand);
                if (!listOnWriting.Contains(cand))
                    listOnWriting.Add(cand);
                if (isProtocolBool())
                    nData = (UInt16)((cand.Size + 7) / 8);
                else if (ElementNumber > 0 && !ProtocolDataSizeBig())
                {
                    if (cand.TagNode.ArrayDimension == 0)
                        nData = (ushort)(GetProtocolDataByteSize());
                    else
                        nData = (ushort)(GetProtocolDataByteSize() * cand.TagNode.ArrayDimension);
                }
                else
                    nData = (UInt16)cand.Size;
                lock (lockListObject)
                {
                    cand.LastValue = cand.Value.Value;
                    jobdata = new byte[nData];
                    cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                }
                uint ArraySize = cand.TagNode.ArrayDimension;
                if (ArraySize == 0)
                    ArraySize = 1;
                if (ProtocolDataSizeBig())
                {
                    List<byte> correctData = new List<byte>();
                    UInt16 sizeDataType = (UInt16)GetDataTypeByteSize((uint)cand.TagNode.DataType.Identifier);
                    UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                    for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                    {
                        byte[] tmpdata = new byte[sizeDataType];
                        if ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                            Array.Copy(jobdata, ArrayIndex * sizeDataType, tmpdata, 0, sizeDataType);
                        else
                        {
                            if ((1 << (ArrayIndex % 8) & jobdata[ArrayIndex / 8]) == 0)
                                tmpdata[0] = 0;
                            else
                                tmpdata[0] = 1;
                        }
                        cand.getWriteValueFromMemRW(ref tmpdata, sizeDataType, sizeProtocolData, ElementNumber, ArrayIndex);
                        correctData.AddRange(tmpdata);
                    }
                    jobdata = correctData.ToArray();
                }
                else if (isProtocolBool())
                {
                    if (ElementNumber == 0 || (uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                    {
                        byte[] tmpData = new byte[ArraySize * GetDataTypeBitSize((uint)cand.TagNode.DataType.Identifier)];
                        for (int ArrayIndex = 0; ArrayIndex < tmpData.Length; ArrayIndex++)
                        {
                            if ((jobdata[ArrayIndex / 8] & (1 << (ArrayIndex % 8))) != 0)
                                tmpData[ArrayIndex] = 1;
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

            lock (lockListObject)
            {
                listOnWriting.ForEach((tag) =>
                {
                    if (!TagsListOnWriting.Contains(tag))
                        TagsListOnWriting.Add(tag);
                });
                listOnWriting.Clear();

                if (listToWrite.Count > 0)
                {
                    var tempListToWrite = new List<Tag>();
                    tempListToWrite.AddRange(TagsListToWrite);
                    TagsListToWrite.Clear();
                    listToWrite.ForEach((tag) =>
                    {
                        if (!TagsListToWrite.Contains(tag))
                            TagsListToWrite.Add(tag);
                    });
                    listToWrite.Clear();
                    tempListToWrite.ForEach((tag) =>
                    {
                        if (!TagsListToWrite.Contains(tag))
                            TagsListToWrite.Add(tag);
                    });
                    tempListToWrite.Clear();
                }
            }

            if (isProtocolBool())
            {
                byte[] tmpData = new byte[(outData.Count() + 7) / 8];
                for (int ArrayIndex = 0; ArrayIndex < outData.Count(); ArrayIndex++)
                {
                    if (outData[ArrayIndex] != 0)
                        tmpData[ArrayIndex / 8] |= (byte)(1 << (ArrayIndex % 8));
                }
                jobData = tmpData;
            }
            else
                jobData = outData.ToArray();
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;

            base.SetJobData(rec, ref changed);
            lock (lockListObject)
            {
               
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                for (int TagIndex = 0; TagIndex < TagsList.Count; TagIndex++)
                {
                    if (TagsList[TagIndex].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    {
                        if (isProtocolBool())
                        {
                            if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset))
                                changed.Add(TagsList[TagIndex]);
                        }
                        else
                        {
                            uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;
                            if (ArraySize == 0)
                                ArraySize = 1;
                            byte[] tmpData = new byte[ArraySize];
                            TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)(sizeProtocolData * ArraySize));
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                bool valBool = TagsList[TagIndex].getBoolValueFromMemRW(sizeProtocolData * ArrayIndex, ElementNumber);
                                tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
                            }

                            if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
                                changed.Add(TagsList[TagIndex]);
                        }
                    }
                    else
                    {
                        if (isProtocolBool())
                        {
                            if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
                                changed.Add(TagsList[TagIndex]);
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
                                    changed.Add(TagsList[TagIndex]);
                            }
                            else
                            {
                                UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)TagsList[TagIndex].TagNode.DataType.Identifier);
                                uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;
                                if (ArraySize == 0)
                                    ArraySize = 1;
                                byte[] tmpData = new byte[sizeTmpData * ArraySize];
                                int indexTmpData = 0;
                                TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)(sizeProtocolData * ArraySize));
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    Array.Copy(rec, TagsList[TagIndex].ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                    indexTmpData += sizeTmpData;
                                }

                                if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
                                    changed.Add(TagsList[TagIndex]);
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
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} TestAggregateJob JobAggregImpossible return point 1",
                //    AddressObj.Address));
                return JobAggregationType.JobAggregImpossible;
            }

            MelsecFXCommJob testJob = candJob as MelsecFXCommJob;
            if (testJob == null)
            {
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} TestAggregateJob JobAggregImpossible return point 2",
                //    AddressObj.Address));
                return JobAggregationType.JobAggregImpossible;
            }

            if (!AddressObj.IsValid || !testJob.AddressObj.IsValid)
            {
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 3",
                //    AddressObj.Address, testJob.AddressObj.Address));
                return JobAggregationType.JobAggregImpossible;
            }

            if (AddressObj.DataArea != testJob.AddressObj.DataArea)
            {
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 4",
                //    AddressObj.Address, testJob.AddressObj.Address));
                return JobAggregationType.JobAggregImpossible;
            }

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType)))
            {
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 5",
                //    AddressObj.Address, testJob.AddressObj.Address));
                return JobAggregationType.JobAggregImpossible;
            }

            NodeId nBool = new NodeId((uint)BuiltInType.Boolean);
            int boolCount = (from elem in TagsList where (uint)(elem.TagNode.DataType.Identifier) == (uint)BuiltInType.Boolean select elem).ToList().Count;
            if (((boolCount > 0) && (testJob.TagsList[0].TagNode.DataType != nBool)) ||
                ((boolCount < 1) && (testJob.TagsList[0].TagNode.DataType == nBool)))
            {
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 6",
                //    AddressObj.Address, testJob.AddressObj.Address));
                return JobAggregationType.JobAggregImpossible;
            }

            ExtraBytes = 0;
            //uint Granularity = Station.GetCommDriver().AggregationThreshold;
            uint gLimit = Station.GetCommDriver().AggregationThreshold;
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
                case MelsecFXDataArea.DataArea_M_Special:
                    if (TagsList[0].TagNode.DataType == nBool/*BuiltInType.Boolean*/)
                    {                        
                        end = start + TotalJobSize - 1;
                        // TODO: check if at this point testJob.TotalJobSize is valid (has been calculated)
                        endTest = startTest + testJob.TotalJobSize - 1;

                        //// don't aggregate bit of 2 different byte
                        //if ((start / 8) != (endTest / 8))
                        //    return JobAggregationType.JobAggregImpossible;
                    }
                    else
                    {
                        if (startTest % 8 > 0)
                        {
                            //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 7",
                            //    AddressObj.Address, testJob.AddressObj.Address));
                            return JobAggregationType.JobAggregImpossible;
                        }
                        end = start + TotalJobSize * 8 - 1;
                        // TODO: check if at this point testJob.TotalJobSize is valid (has been calculated)
                        endTest = startTest + testJob.TotalJobSize * 8 - 1;
                    }
                    gLimit *= 8;
                    break;

                case MelsecFXDataArea.DataArea_TN:
                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_D_Special:
                case MelsecFXDataArea.DataArea_C_16:
                    // TODO: check if at this point testJob.TotalJobSize is valid (has been calculated)
                    if (testJob.TotalJobSize % 2 > 0)
                    {
                        //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 8",
                        //    AddressObj.Address, testJob.AddressObj.Address));
                        return JobAggregationType.JobAggregImpossible;
                    }
                    end = start + TotalJobSize/2 - 1;
                    endTest = startTest + testJob.TotalJobSize/2 - 1;
                    gLimit /= 2;
                    break;

                case MelsecFXDataArea.DataArea_C_32:
                    // TODO: check if at this point testJob.TotalJobSize is valid (has been calculated)
                    if (testJob.TotalJobSize % 4 > 0)
                    {
                        //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 9",
                        //    AddressObj.Address, testJob.AddressObj.Address));
                        return JobAggregationType.JobAggregImpossible;
                    }
                    end = start + TotalJobSize / 4 - 1;
                    endTest = startTest + testJob.TotalJobSize / 4 - 1;
                    gLimit /= 4;
                    break;

                default:
                    //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 10",
                    //        AddressObj.Address, testJob.AddressObj.Address));
                    return JobAggregationType.JobAggregImpossible;
            }

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
                        case MelsecFXDataArea.DataArea_M_Special:
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
                        case MelsecFXDataArea.DataArea_D:
                        case MelsecFXDataArea.DataArea_D_Special:
                        case MelsecFXDataArea.DataArea_C_16:
                            startTag = start + tag.ByteOffset / 2;
                            endTag = startTag + tag.Size / 2 - 1;
                            break;

                        case MelsecFXDataArea.DataArea_C_32:
                            startTag = start + tag.ByteOffset / 4;
                            endTag = startTag + tag.Size / 4 - 1;
                            break;

                        default:
                            //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 11",
                            //AddressObj.Address, testJob.AddressObj.Address));
                            return JobAggregationType.JobAggregImpossible;
                    }

                    if ((startTest >= startTag && startTest <= endTag)
                        || (endTest >= startTag && endTest <= endTag))
                    {
                        //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 12",
                        //AddressObj.Address, testJob.AddressObj.Address));
                        return JobAggregationType.JobAggregImpossible;
                    }
                }
            }

            else if ((startTest < start) && (endTest > end))
            {
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 13",
                //AddressObj.Address, testJob.AddressObj.Address));
                return JobAggregationType.JobAggregImpossible;
            }

            // Check granularity
            else
            {
                //if (TagsList[0].TagNode.DataType == nBool)
                //{
                //    gLimit *= 8;
                //}
                if (endTest < start)
                {
                    if ((start - endTest) > (gLimit + 1))
                    {
                        //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 14",
                        //AddressObj.Address, testJob.AddressObj.Address));
                        return JobAggregationType.JobAggregImpossible;
                    }
                }
                else if ((startTest - end) > (gLimit + 1))
                {
                    //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 15",
                    //AddressObj.Address, testJob.AddressObj.Address));
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
                    case MelsecFXDataArea.DataArea_M_Special:
                        if (TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
                        {
                            newoffset /= 8;
                        }
                        break;

                    case MelsecFXDataArea.DataArea_TN:
                    case MelsecFXDataArea.DataArea_D:
                    case MelsecFXDataArea.DataArea_D_Special:
                    case MelsecFXDataArea.DataArea_C_16:
                        newoffset *= 2;
                        break;

                    case MelsecFXDataArea.DataArea_C_32:
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

                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregFits return point 16",
                //AddressObj.Address, testJob.AddressObj.Address));
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
                case MelsecFXDataArea.DataArea_M_Special:
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
                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_D_Special:
                case MelsecFXDataArea.DataArea_C_16:
                    newJobSize = (newEndAddress - newStartAddress + 1) * 2;
                    break;

                case MelsecFXDataArea.DataArea_C_32:
                    newJobSize = (newEndAddress - newStartAddress + 1) * 4;
                    break;
            }
            if (newJobSize > GetAggregateMaxJobSize())
            {
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregImpossible return point 17",
                //AddressObj.Address, testJob.AddressObj.Address));
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

                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregForward return point 18",
                //AddressObj.Address, testJob.AddressObj.Address));
                return JobAggregationType.JobAggregForward;
            }

            // Aggregate backward
            else
            {
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} TestAggregateJob JobAggregBackward return point 19",
                //AddressObj.Address, testJob.AddressObj.Address));
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
                        TagsList.Add(new MelsecFXTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
                    }
                    break;

                case JobAggregationType.JobAggregForward:
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        TagsList.Add(new MelsecFXTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
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
                        TagsList.Add(new MelsecFXTag(candJob.TagsList[i].TagNode, candJob.TagsList[i].ByteOffset, 0));
                    }
                    TotalJobSize += ExtraBytes;
                    Address = ((MelsecFXCommJob)candJob).Address;
                    AddressObj.Set(Address);
                    break;

                default:
                    return false;
            }

            return true;
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

        List<DriverBaseInterfaces.TagDefinition> GetSimpleTagList(DriverBaseInterfaces.TagDefinition t)
        {
            List<DriverBaseInterfaces.TagDefinition> l = new List<DriverBaseInterfaces.TagDefinition>();
            if (t.DataType.IdType == IdType.Guid)
            {
                //prototype?
                List<DriverBaseInterfaces.TagDefinition> tList = new List<DriverBaseInterfaces.TagDefinition>();
                Station.GetCommDriver().OnTagPrototypeQuery(t.NodeId, ref tList);
                foreach (var a in tList)
                {
                    l.AddRange(GetSimpleTagList(a));
                }
            }
            else
            {
                l.Add(t);
            }
            return l;
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
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList =
                                new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                if (!IsTypeAdmitted(t.TagNode.DataType) &&
                    t.DynSettings.MethodID == -1)
                {
                    IsValid = false;
                    InvalidReason = string.Format(
                                    Properties.Resources.ErrorInvalidTagType,
                                    t.TagNode.NodeId.ToString(),
                                    t.TagNode.DataType.Identifier.ToString());
                    return;
                }
                tagnamelist += (tagnamelist.Length > 0 ?
                            ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            if (!AddressObj.IsValid)
            {
                IsValid = false;
                InvalidReason = string.Format(
                              Properties.Resources.ErrorInvalidAssignedAddress,
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
                InvalidReason = string.Format(Properties.Resources.ErrorInputTypeRequired,tagnamelist, DriverCodeBase.Properties.Resources.LinkType_Input);
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
            if (CommandCode == (byte)MelsecFXCommandCode.Command_Read_Bytes && receiveBuffer != null)
            {
                byte[] recData = receiveBuffer;
                if (isProtocolBool())
                {
                    int byteBufferSize = (int)TotalJobSize;
                    recData = new byte[byteBufferSize];
                                                           
                    int bytecount = 0;
                    int bitOffset = 0;
                    byte mask = 0;
                    int stAdd = 0;

                    Int32 byteStartAddress = AddressObj.StartAddress / 8;

                    for (int i = 0; i < TagsList.Count; i++)
                    {
                        if (TagsList[i].DynSettings.MethodID != -1)
                            continue;
                    
                        // single bit or aggregated
                        if (TagsList[i].TagNode.ArrayDimension == 0)
                        {
                            stAdd = (int)(AddressObj.StartAddress + TagsList[i].ByteOffset);

                            // byteOffset is the bit nr
                            bitOffset = (byte)(stAdd % 8);
                            
                            bytecount = (stAdd / 8) - byteStartAddress;

                            mask = (byte)Math.Pow(2, bitOffset);

                            if ((int)(receiveBuffer[bytecount] & mask) == (int)mask)
                                recData[TagsList[i].ByteOffset] = 1;
                            else
                                recData[TagsList[i].ByteOffset] = 0;
                        }
                        else
                        {                                                        
                            for (int j = 0; j < TagsList[i].TagNode.ArrayDimension; j++)
                            {
                                stAdd = (int)(AddressObj.StartAddress + TagsList[i].ByteOffset + j);

                                // byteOffset is the bit nr
                                bitOffset = (byte)(stAdd % 8);

                                bytecount = (stAdd / 8) - byteStartAddress;

                                mask = (byte)Math.Pow(2, bitOffset);

                                if ((int)(receiveBuffer[bytecount] & mask) == (int)mask)
                                    recData[j] = 1;
                                else
                                    recData[j] = 0;
                            }
                        }
                    }               
                }

                SetJobData(recData, ref changed);
                items.AddRange(changed);
            }

            return true;
        }

       #endregion
    }
}
