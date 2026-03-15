using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace ModbusTCP
{
    public class ModbusTCPCommJob : CommJob
    {
                #region Constructors
        public ModbusTCPCommJob(Station station, ModbusTCPCommJobSettings settings)
            : base(station, settings)
        {
            _FunctionCode = settings.FunctionCode;
            _StartAddress = settings.StartAddress;
            ModbusTCPStation modbusStation = (ModbusTCPStation)station;
            AddressType = modbusStation.AddressType;
            _FileNumber = settings.FileNumber;
            _SwapDWords = settings.SwapDWords;
            _StringLength = settings.StringLength;

            _BroadCast = settings.BroadCast;
            CheckJobValid();
        }

        public ModbusTCPCommJob(Station station, ModbusTCPTag defTag)
            : base(station, defTag)
        {
            _FunctionCode = defTag.ModbusTCPDynSettings.FunctionCode;
            _StartAddress = defTag.ModbusTCPDynSettings.StartAddress;
            ModbusTCPStation modbusStation = (ModbusTCPStation)station;
            AddressType = modbusStation.AddressType;
            _FileNumber = defTag.ModbusTCPDynSettings.FileNumber;
            _SwapDWords = defTag.ModbusTCPDynSettings.SwapDWords;

            _StringLength = defTag.ModbusTCPDynSettings.StringLength;
            _BroadCast = defTag.ModbusTCPDynSettings.BroadCast;

            if (TotalJobSize == 0)
                TotalJobSize = (uint)((defTag.ModbusTCPDynSettings.StringLength + 1) / 2) * 2;

            if ((ElementNumber > 0 && !isProtocolBool()) || (ElementNumber >= 0 && ProtocolDataSizeBig()))
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }
            if (ElementNumber < 0 && ProtocolDataSizeBig())
            {
                uint ArraySize = TagsList[0].TagNode.ArrayDimension;
                if (ArraySize == 0)
                    ArraySize = 1;
                if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    TotalJobSize = ((ArraySize + 15) / 16 ) * 2;
                else
                    TotalJobSize = ((ArraySize + 1) / 2) * 2;

            }


            CheckJobValid();
        }

        public ModbusTCPCommJob(Station station)
            : base(station)
        {
            ModbusTCPStation modbusStation = (ModbusTCPStation)station;
            AddressType = modbusStation.AddressType;
            CheckJobValid();
        }

        protected ModbusTCPCommJob()
        {
            CheckJobValid();
        }
        #endregion
        #region data Member

        public AddressTypes AddressType = AddressTypes.ZeroBased;

        #endregion

        #region Static methods
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
                nType == (uint)BuiltInType.String ||
                nType == (uint)BuiltInType.UInteger
                )
                    return true;
                return false;
            }
            
            return false;
        }
        #endregion

        #region override Methods

        public override bool ProtocolDataSizeIsValid(out string errorDesc)
        {
            errorDesc = null;
            if (ProtocolDataSizeBig())
            {
                if (ElementNumber > (GetProtocolDataBitSize() / GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier) - 1))
                    errorDesc = "ElementNumber";
                if (FunctionCode != FunctionCodes.MaskWriteRegister &&
                    (Type == LinkType.ExceptionOutput || Type == LinkType.UnconditionalOutput))
                    errorDesc = "LinkType";
            }
            if (errorDesc == null)
                return true;
            else
                return false;
        }

        #endregion

        #region Methods
        uint GetTagSize(DriverBaseInterfaces.TagDefinition t)
        {
            if (t.DataType.IdType == IdType.Numeric)
            {
                switch((uint)t.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        return 1;
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        return 2;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        return 4;
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        return 8;
                    case (uint)BuiltInType.String:
                        return (uint)((StringLength + 1) / 2) * 2;
                    default:
                        return 0;
                }            
            }
            return 0;
        }
        /*
        nella classe base???????
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
                l.Add(t);
            //order ...
            return l;
        }*/

        private void CheckJobValid()
        {
                
            bool bit = false;
            bool num = false;
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList = new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                if (!IsTypeAdmitted(t.TagNode.DataType) && t.DynSettings.MethodID == -1)
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tag type is invalid for its Data Type. (Tag: {0} Data Type: {1})", t.TagNode.NodeId.ToString(), t.TagNode.DataType.Identifier.ToString());
                    return;
                }
                tagnamelist += (tagnamelist.Length > 0 ? ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            foreach (var t in tempList)
            {
                if ((uint)t.DataType.Identifier == (uint)BuiltInType.Boolean)
                    bit = true;
                else
                    num = true;
            }

            //tag must be all bit or not
            if ((bit && num))
            {
                IsValid = false;
                InvalidReason = string.Format("The Tags type are inconsistent for the Function Code. (Tags: {0} F.Code: {1})", tagnamelist, FunctionCode);
                return;
            }

            string errorDesc;
            if (!ProtocolDataSizeIsValid(out errorDesc))
            {
                IsValid = false;
                InvalidReason = string.Format("The {0} is invalid for the Function Code. (Tags: {1} F.Code: {2})", errorDesc, tagnamelist, FunctionCode);
                return;
            }

            if ((FunctionCode == FunctionCodes.InputRegisters || FunctionCode == FunctionCodes.DiscreteInputs)
                && Type != LinkType.Input)
            { 
                //must be Input type
                IsValid = false;
                InvalidReason = string.Format("The Job must be of type Input. (Tags: {0})", tagnamelist);
                return;
            }
            
            if (CalculateTotalBytesJobSize(bit) > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        private uint CalculateTotalBytesJobSize(bool bit)
        {
            if (bit) {
                uint tot = (uint)Math.DivRem((int)TotalJobSize, 8, out int bitRest);
                if (bitRest > 0)
                    tot++;
                return tot;
            }
            else
            {
                return TotalJobSize;
            }
        }

        // Added to solve FOGBUGZ 10394
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

        public override uint getProtocolDataType()
        {
            switch (FunctionCode)
            {
                case FunctionCodes.Coils: // Coils
                case FunctionCodes.SingleCoil://Single coil
                case FunctionCodes.DiscreteInputs: // Input discretes
                case FunctionCodes.MaskWriteRegister:
                    return (uint)BuiltInType.Boolean;
                case FunctionCodes.MultipleRegisters: // Multiple registers
                case FunctionCodes.InputRegisters: // Input registers
                case FunctionCodes.SingleRegister: // Single register
                case FunctionCodes.FileRecord: // File Record
                    switch (ElementNumber){
                        case 2:                   
                            return ((uint)BuiltInType.UInt32);
                        case 4:
                            return ((uint)BuiltInType.UInt64);
                        default:
                            return (uint)BuiltInType.UInt16;
                    }
            }
            return 0;
        }

        public override uint GetMaxJobSize()
        {
            return ModbusProtocol.GetMaxJobSize(FunctionCode, Type);
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;

            ModbusTCPCommJob testJob = candJob as ModbusTCPCommJob;
            if (testJob == null)
                return JobAggregationType.JobAggregImpossible;

            if (FunctionCode != testJob.FunctionCode)
                return JobAggregationType.JobAggregImpossible;

            // ElementNumber can be used to read more data elements ( >=1 ) so don't allow aggregations
            if (ElementNumber != candJob.ElementNumber)
                return JobAggregationType.JobAggregImpossible;

            if (FunctionCode != FunctionCodes.MaskWriteRegister)
            {
                if (base.TestAggregateJob(candJob, out ExtraBytes) == JobAggregationType.JobAggregImpossible)
                {
                    return JobAggregationType.JobAggregImpossible;
                }
            }
            else
            {
                return TestAggregateJobForMaskWriteRegister(testJob, out ExtraBytes);
            }
            if (FunctionCode == FunctionCodes.FileRecord && FileNumber != testJob.FileNumber)
                return JobAggregationType.JobAggregImpossible;

            if (BroadCast != testJob.BroadCast)
                return JobAggregationType.JobAggregImpossible;

            if (SwapDWords != testJob.SwapDWords)
                return JobAggregationType.JobAggregImpossible;

            if ((FunctionCode == FunctionCodes.SingleRegister || FunctionCode == FunctionCodes.SingleCoil) &&
                (testJob.Type == LinkType.ExceptionOutput || testJob.Type == LinkType.UnconditionalOutput))
                return JobAggregationType.JobAggregImpossible;

            if (FunctionCode == FunctionCodes.ExceptionStatus || testJob.FunctionCode == FunctionCodes.ExceptionStatus ||
                FunctionCode == FunctionCodes.MaskWriteRegister)
                return JobAggregationType.JobAggregImpossible;

            /*
             * exclude var type not admitable, strings or odd size in word areas.
             * states de presence of bit variables or not: bit and numeric tags can't be aggregate together
             */

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType)))
                return JobAggregationType.JobAggregImpossible;

            if (TagsList[0].TagNode.DataType == (uint)BuiltInType.String ^ testJob.TagsList[0].TagNode.DataType == (uint)BuiltInType.String)
                return JobAggregationType.JobAggregImpossible;

            NodeId nBool = new NodeId((uint)BuiltInType.Boolean);
            // Modified to solve FOGBUGZ 12571
            //if ((from elem in TagsList 
            //     where elem.TagNode.DataType == BuiltInType.Boolean
            //     select elem).ToList().Count > 0 && testJob.TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
            if ((from elem in TagsList
                 where elem.TagNode.DataType == nBool
                 select elem).ToList().Count > 0 && testJob.TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
                return JobAggregationType.JobAggregImpossible;

            if (SwapDWords && (GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier) < 64 || GetDataTypeBitSize((uint)candJob.TagsList[0].TagNode.DataType.Identifier) < 64))
                return JobAggregationType.JobAggregImpossible;

            ExtraBytes = 0;
            uint Granularity = Station.GetCommDriver().AggregationThreshold;
            uint start = StartAddress;
            uint startTest = testJob.StartAddress;
            uint elementsize = 2;

            if (FunctionCode == FunctionCodes.Coils || FunctionCode == FunctionCodes.DiscreteInputs
                || FunctionCode == FunctionCodes.SingleCoil)
            {
                if (TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
                {
                    if (System.Math.Abs(start - startTest) % 8 != 0)
                        return JobAggregationType.JobAggregImpossible;

                    start /= 8;
                    startTest /= 8;
                }
                else
                    Granularity *= 8;

                elementsize = 1;
            }

            uint end = start + (TotalJobSize / elementsize) - 1;
            uint endTest = startTest + (testJob.TotalJobSize / elementsize) - 1;

            /*
             * Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
             */
            foreach (var tag in TagsList)
            {
                uint startTag = start + tag.ByteOffset / elementsize;
                uint endTag = startTag + (tag.Size / elementsize) - 1;
                if ((startTest >= startTag && startTest <= endTag)
                    || (endTest >= startTag && endTest <= endTag))
                    return JobAggregationType.JobAggregImpossible;
            }

            if (startTest < start && endTest > end)
                return JobAggregationType.JobAggregImpossible;

            if (FunctionCode != FunctionCodes.Coils && FunctionCode != FunctionCodes.DiscreteInputs && FunctionCode != FunctionCodes.SingleCoil)
                if (SwapWords)
                    if (start % 2 != startTest % 2)
                        return JobAggregationType.JobAggregImpossible;

            if (startTest >= start && endTest <= end)
            {
                uint newoffset = (startTest - start) * elementsize;
                for (int i = 0; i < candJob.TagsList.Count; i++)
                {
                    candJob.TagsList[i].ByteOffset = newoffset;
                    newoffset += candJob.TagsList[i].Size;
                }
                return JobAggregationType.JobAggregFits;
            }

            if (startTest >= start && startTest <= end + Granularity)
                //if (((endTest - startTest + 1) * elementsize) <= GetMaxJobSize())
                // Modified to solve FOGBUGZ 10394
                //if (((endTest - start + 1) * elementsize) <= GetMaxJobSize())
                if (((endTest - start + 1) * elementsize) <= GetAggregateMaxJobSize())

                {
                    uint newoffset = (startTest - start) * elementsize;
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset = newoffset;
                        newoffset += candJob.TagsList[i].Size;
                    }
                    ExtraBytes = (endTest - end) * elementsize;
                    return JobAggregationType.JobAggregForward;
                }

            uint startBkw;
            if (start > Granularity)
                startBkw = start - Granularity;
            else
                startBkw = 0;
            if (endTest >= startBkw && endTest <= end)
                // Modified to solve FOGBUGZ 10394
                //if (((endTest - startBkw + 1) * elementsize) <= GetMaxJobSize())
                if (((end - startTest + 1) * elementsize) <= GetAggregateMaxJobSize())

                {
                    uint newoffset = 0;
                    for (int i = 0; i < candJob.TagsList.Count; i++)
                    {
                        candJob.TagsList[i].ByteOffset = newoffset;
                        newoffset += candJob.TagsList[i].Size;
                    }
                    ExtraBytes = (start - startTest) * elementsize;
                    return JobAggregationType.JobAggregBackward;
                }

            return JobAggregationType.JobAggregImpossible;
        }

        public JobAggregationType TestAggregateJobForMaskWriteRegister(ModbusTCPCommJob testJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;

            if (testJob == null)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (FunctionCode != testJob.FunctionCode)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            // Special case "Mask Write Register": avoid some checks done by the method of the the base class
            if (Station != testJob.Station)
                return JobAggregationType.JobAggregImpossible;
            if (Type != testJob.Type)
                return JobAggregationType.JobAggregImpossible;
            if (SwapBytes != testJob.SwapBytes)
                return JobAggregationType.JobAggregImpossible;
            if (SwapWords != testJob.SwapWords)
                return JobAggregationType.JobAggregImpossible;
            if (SwapBytes && (GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier) < 16 || GetDataTypeBitSize((uint)testJob.TagsList[0].TagNode.DataType.Identifier) < 16))
                return JobAggregationType.JobAggregImpossible;
            if (SwapWords && (GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier) < 32 || GetDataTypeBitSize((uint)testJob.TagsList[0].TagNode.DataType.Identifier) < 32))
                return JobAggregationType.JobAggregImpossible;
            if (SamplingInterval != testJob.SamplingInterval)
                return JobAggregationType.JobAggregImpossible;
            if (ProtocolDataSizeBig() ^ testJob.ProtocolDataSizeBig())
                return JobAggregationType.JobAggregImpossible;
            if ((uint)TagsList[0].TagNode.DataType.Identifier != (uint)BuiltInType.Boolean &&
                GetProtocolDataBitSize() < 8)
                return JobAggregationType.JobAggregImpossible;
            if ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean ^
                (uint)testJob.TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                return JobAggregationType.JobAggregImpossible;

            if (conditionalVariableHasBeenSet != testJob.conditionalVariableHasBeenSet)
            {
                return JobAggregationType.JobAggregImpossible;
            }
            if (conditionalVariableHasBeenSet == true)
            {
                if ((ConditionalVariableName != testJob.ConditionalVariableName) || (ConditionalVariableId != testJob.ConditionalVariableId))
                {
                    return JobAggregationType.JobAggregImpossible;
                }
            }

            if (SwapDWords != testJob.SwapDWords)
                return JobAggregationType.JobAggregImpossible;

            /*
             * exclude var type not admitable, strings or odd size in word areas.
             * states de presence of bit variables or not: bit and numeric tags can't be aggregate together
             */

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType))) 
                return JobAggregationType.JobAggregImpossible;

            if (TagsList[0].TagNode.DataType == (uint)BuiltInType.String ^ testJob.TagsList[0].TagNode.DataType == (uint)BuiltInType.String)
                return JobAggregationType.JobAggregImpossible;

            NodeId nBool = new NodeId((uint)BuiltInType.Boolean);
            if ((from elem in TagsList
                 where elem.TagNode.DataType == nBool
                 select elem).ToList().Count > 0 && testJob.TagsList[0].TagNode.DataType != nBool)
               return JobAggregationType.JobAggregImpossible;

            if (SwapDWords && (GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier) < 64 || GetDataTypeBitSize((uint)testJob.TagsList[0].TagNode.DataType.Identifier) < 64))
                return JobAggregationType.JobAggregImpossible;

            // Specific checks
            if (TagsList[0].TagNode.DataType != nBool)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            uint Granularity = Station.GetCommDriver().AggregationThreshold * 8;
            uint start = (uint)(StartAddress*16 + ElementNumber);
            uint startTest = (uint)(testJob.StartAddress*16 + testJob.ElementNumber);
            uint elementsize = 1;
            uint end = start + TotalJobSize - 1;
            uint endTest = startTest + testJob.TotalJobSize - 1;

            /*
             * Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
             */
            foreach (var tag in TagsList)
            {
                uint startTag = start + tag.ByteOffset;
                uint endTag = startTag + tag.Size - 1;
                if ((startTest >= startTag && startTest <= endTag)
                    || (endTest >= startTag && endTest <= endTag))
                    return JobAggregationType.JobAggregImpossible;
            }

            if (startTest < start && endTest > end)
                return JobAggregationType.JobAggregImpossible;

            if (SwapWords)
                if ((start/8) % 2 != (startTest/8) % 2)
                    return JobAggregationType.JobAggregImpossible;

            if (startTest >= start && endTest <= end)
            {
                uint newoffset = startTest - start;
                for (int i = 0; i < testJob.TagsList.Count; i++)
                {
                    testJob.TagsList[i].ByteOffset += newoffset;
                }
                return JobAggregationType.JobAggregFits;
            }

            if (startTest >= start && startTest <= end + Granularity)
                if ((endTest - start + 1) <= GetAggregateMaxJobSize())
                {
                    uint newoffset = startTest - start;
                    for (int i = 0; i < testJob.TagsList.Count; i++)
                    {
                        testJob.TagsList[i].ByteOffset += newoffset;
                    }
                    ExtraBytes = (endTest - end) * elementsize;
                    return JobAggregationType.JobAggregForward;
                }

            uint startBkw;
            if (start > Granularity)
                startBkw = start - Granularity;
            else
                startBkw = 0;
            if (endTest >= startBkw && endTest <= end)
                if (((end - startTest + 1) * elementsize) <= GetAggregateMaxJobSize())

                {
                    ExtraBytes = start - startTest;
                    return JobAggregationType.JobAggregBackward;
                }

            return JobAggregationType.JobAggregImpossible;
        }
        
        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch(AggType)
            {
                case JobAggregationType.JobAggregFits:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new ModbusTCPTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    
                    break;
                case JobAggregationType.JobAggregForward:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new ModbusTCPTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }

                    TotalJobSize += ExtraBytes;
                    break;
                case JobAggregationType.JobAggregBackward:
                    foreach (var tag in TagsList)
                    {
                        tag.ByteOffset += ExtraBytes;
                    }

                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new ModbusTCPTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    TotalJobSize += ExtraBytes;

                    StartAddress = ((ModbusTCPCommJob)candJob).StartAddress;

                    if(FunctionCode == FunctionCodes.MaskWriteRegister)
                    {
                        ElementNumber = ((ModbusTCPCommJob)candJob).ElementNumber;
                    }
                    break;
                default:
                    return false;
            }
            /*
             * Fits: add tag with correct offset
             * Forward: tag added extend job to higher addresses. Calculate new job size, Start address doesn't change.
             * Backward: tag added extend job to lower addresses. Calculate new job size and new Start address.
             */
            return true;
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
                if (cand != null)
                {
                    if((FunctionCode == FunctionCodes.SingleCoil) || (FunctionCode == FunctionCodes.SingleRegister) || (FunctionCode == FunctionCodes.MaskWriteRegister))
                    {
                        // Write just one variable per time
                        break;
                    }

                    if ((cand.ByteOffset + cand.Size) != listToWrite[0].ByteOffset)
                        break;
                }
                cand = listToWrite[0];
                listToWrite.Remove(cand);
                if (!listOnWriting.Contains(cand))
                    listOnWriting.Add(cand);
                if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                    nData = (UInt16)((cand.Size + 7) / 8);
                else if (ElementNumber > 0 && !ProtocolDataSizeBig())
                {
                    if (cand.TagNode.ArrayDimension == 0)
                        nData = (ushort)(GetProtocolDataByteSize());
                    else
                        nData = (ushort)(GetProtocolDataByteSize() * cand.TagNode.ArrayDimension);
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
                    ArraySize = 1;
                if (ProtocolDataSizeBig())
                {
                    List<byte> correctData = new List<byte>();
                    if (ElementNumber >= 0)
                    {
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
                    }
                    else
                    {
                        UInt16 sizeDataType = (UInt16)GetDataTypeByteSize((uint)cand.TagNode.DataType.Identifier);
                        if ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                        {
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                byte[] tmpdata = new byte[sizeDataType];
                                Array.Copy(jobdata, ArrayIndex * sizeDataType, tmpdata, 0, sizeDataType);
                                correctData.AddRange(tmpdata);
                            }
                        }
                        else
                        {
                            byte[] tmpdata = new byte[TotalJobSize];
                            Array.Copy(jobdata, tmpdata, TotalJobSize);
                            correctData.AddRange(tmpdata);
                        }
                        if ((correctData.Count & 1) == 1)
                            correctData.Add(0);
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

                if(listToWrite.Count > 0)
                {
                    var tempListToWrite = new List<Tag>();
                    tempListToWrite.AddRange(TagsListToWrite);
                    TagsListToWrite.Clear();
                    listToWrite.ForEach((tag) =>
                    {
                        if (!TagsListToWrite.Contains(tag))
                        {
                            TagsListToWrite.Add(tag);
                        }
                    });
                    listToWrite.Clear();
                    tempListToWrite.ForEach((tag) =>
                    {
                        if (!TagsListToWrite.Contains(tag))
                        {
                           TagsListToWrite.Add(tag);
                        }
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

            if ((FunctionCode == FunctionCodes.MultipleRegisters ||
                FunctionCode == FunctionCodes.SingleRegister ||
                FunctionCode == FunctionCodes.FileRecord ||
                FunctionCode == FunctionCodes.InputRegisters) ^ (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.String))
                SwapByteBuffer(ref rec);

            if (SwapDWords)
                SwapDWordBuffer(ref rec);

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
                            if (((ModbusTCPTag)(TagsList[TagIndex])).SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset))
                                changed.Add(TagsList[TagIndex]);
                        }
                        else
                        {
                            uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;
                            if (ArraySize == 0)
                                ArraySize = 1;
                            byte[] tmpData = new byte[ArraySize];

                            if (ElementNumber >= 0)
                            {
                                TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)(sizeProtocolData * ArraySize));
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    tmpData[ArrayIndex] = (byte)(TagsList[TagIndex].getBoolValueFromMemRW(sizeProtocolData * ArrayIndex, ElementNumber) ? 1 : 0);
                                }
                            }
                            else
                            {
                                TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)TotalJobSize);
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    tmpData[ArrayIndex] = (byte)(TagsList[TagIndex].getBoolValueFromMemRW(0, ArrayIndex) ? 1 : 0);
                                }
                            }
                            if (((ModbusTCPTag)(TagsList[TagIndex])).SetTagValue(ref tmpData, 0))
                                changed.Add(TagsList[TagIndex]);
                        }
                    }
                    else
                    {
                        if (isProtocolBool())
                        {
                            if (((ModbusTCPTag)(TagsList[TagIndex])).SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0)))
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
                                if (((ModbusTCPTag)(TagsList[TagIndex])).SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, elemsize))
                                    changed.Add(TagsList[TagIndex]);
                            }
                            else
                            {
                                byte[] tmpData;
                                if (ElementNumber >= 0)
                                {
                                    UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)TagsList[TagIndex].TagNode.DataType.Identifier);
                                    uint ArraySize = TagsList[TagIndex].TagNode.ArrayDimension;
                                    if (ArraySize == 0)
                                        ArraySize = 1;
                                    tmpData = new byte[sizeTmpData * ArraySize];
                                    int indexTmpData = 0;
                                    TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, (int)(sizeProtocolData * ArraySize));
                                    for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                    {
                                        Array.Copy(rec, TagsList[TagIndex].ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                        indexTmpData += sizeTmpData;
                                    }
                                }
                                else
                                {
                                    tmpData = new byte[TotalJobSize];
                                    TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, tmpData.Length);
                                    Array.Copy(rec, TagsList[TagIndex].ByteOffset, tmpData,0, TotalJobSize);
                                }

                                if (((ModbusTCPTag)(TagsList[TagIndex])).SetTagValue(ref tmpData, 0))
                                    changed.Add(TagsList[TagIndex]);
                            }
                        }
                    }
                }

                FirstTime = false;
            }
        }
        public ushort elementOnWrite(IList<Tag> listOnWriting)
        {
            ushort ItemCount = 0;
            foreach (var tag in listOnWriting)
            {
                ItemCount += (ushort)(tag.TagNode.ArrayDimension == 0 ? 1 : tag.TagNode.ArrayDimension);
            }
            return ItemCount;
        }

        private static int CompareTagByOffset(Tag x, Tag y)
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
                    if (x.ByteOffset > y.ByteOffset)
                        return 1;
                    else if (x.ByteOffset == y.ByteOffset)
                        return 0;
                    else
                        return -1;

                }
            }
        }

        public static void SwapDWordBuffer(ref byte[] buf, int init = 0, int len = 0)
        {
            byte[] _temp;
            if (buf.Length == 0 || init + len > buf.Length)
                return;
            int max = (len <= 0 ? buf.Length : (/*init +*/ len));
            if (max % 8 != 0 )
                return;
                _temp = new byte[max];
            Buffer.BlockCopy(buf, init + 4, _temp, 0, max - 4);
            for (int i = 0; i < max ; i+=8)
            {
                _temp[i + 4] = buf[init + i];
                _temp[i + 4 + 1] = buf[init + i + 1];
                _temp[i + 4 + 2] = buf[init + i + 2];
                _temp[i + 4 + 3] = buf[init + i + 3];
            }
            Buffer.BlockCopy(_temp, 0, buf, init, max);
        }
        #endregion

        #region Properties
        private byte _Executioncode;
        public byte Executioncode
        {
            get { return _Executioncode; }
            set
            {
                _Executioncode = value;
            }
        }
        
        private FunctionCodes _FunctionCode;
        public FunctionCodes FunctionCode
        {
            get { return _FunctionCode; }
            set
            {
                _FunctionCode = value;
            }
        }

        private UInt16 _StartAddress;
        public UInt16 StartAddress
        {
            get { return _StartAddress; }
            set
            {
                _StartAddress = value;
            }
        }

        private uint _FileNumber;
        public uint FileNumber
        {
            get { return _FileNumber; }
            set
            {
                _FileNumber = value;
            }
        }

        /// <summary>   true to swap dwords. </summary>
        private bool _SwapDWords;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets a value indicating whether the swap dwords. </summary>
        ///
        /// <value> true if swap dwords, false if not. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SwapDWords
        {
            get { return _SwapDWords; }
            set
            {
                _SwapDWords = value;
            }
        }

        private bool _BroadCast;
        public bool BroadCast
        {
            get { return _BroadCast; }
            set
            {
                _BroadCast = value;
            }
        }

        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return string.Format("{0}FC{1:00}", ret, (uint)FunctionCode);
            }
        }

        /*
        private ModbusCommJobSettings _JobSettings;
        public ModbusCommJobSettings JobSettings
        {
            get 
            {
                if ((Settings as ModbusCommJobSettings) != null)
                    _JobSettings = (ModbusCommJobSettings)Settings;
                
                return _JobSettings; 
            }
        }
        */
   
        /// <summary>   The String Length. </summary>
        private uint _StringLength;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   String Length Property. </summary>
        ///
        /// <value> The String Length. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint StringLength
        {
            get { return _StringLength; }
            set { _StringLength = value; }
        }

        #endregion
    }
}
