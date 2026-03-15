using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace NaisFp
{
    public class NaisFpCommJob : CommJob
    {
        #region Constructors

        public NaisFpCommJob(Station station, NaisFpCommJobSettings settings)
            : base(station, settings)
        {
            Address = settings.Address;
            //PartialWriteStart = 0;
            //LastErrorCode = DriverErrorCodes.ErrorNoError;
            
            CheckJobValid();
        }

        public NaisFpCommJob(Station station, NaisFpTag defTag)
            : base(station, defTag)
        {
            Address = defTag.NaisFpDynSettings.Address;
            //PartialWriteStart = 0;
            //LastErrorCode = DriverErrorCodes.ErrorNoError;

            if ((ElementNumber > 0 && !isProtocolBool()) || ProtocolDataSizeBig())
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }

            CheckJobValid();
        }

        public NaisFpCommJob(Station station)
            : base(station)
        {
            Address = String.Empty;
            //PartialWriteStart = 0;

            CheckJobValid();
        }

        protected NaisFpCommJob()
        {
            Address = String.Empty;
            //PartialWriteStart = 0;
            //LastErrorCode = DriverErrorCodes.ErrorNoError;

            CheckJobValid();
        }
        #endregion

        #region data Member

        public NaisFpAddress AddressObj  = new NaisFpAddress();
        //public ushort PartialWriteStart;
        //public DriverErrorCodes LastErrorCode;

        #endregion

        #region Static methods
        public static bool IsTypeAdmitted(NodeId type)
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
                nType == (uint)BuiltInType.UInteger
                )
                    return true;
                return false;
            }
            return false;
        }

        #endregion
        
        #region Methods


        public bool IsOnlyInput()
        {
            return (AddressObj.MemoryArea == MemoryAreas.X);
        }

        public bool IsReadCommand()
        {
            lock (lockListObject)
            {
                return (Type == DriverCodeBaseEx.Enumerators.LinkType.Input ||
                (Type == DriverCodeBaseEx.Enumerators.LinkType.InputOutput &&
                TagsListOnWriting.Count == 0));
            }
        }

        public string GetCommandCode()
        {
            if (!IsReadCommand() && (AddressObj.DataFormat == DataFormats.BOOL))
            {
                switch (AddressObj.MemoryArea)
                {
                    case MemoryAreas.X:
                        return "";
                    case MemoryAreas.Y:
                    case MemoryAreas.R:
                    case MemoryAreas.L:
                        return String.Format("CP");
                }
            }

            switch (AddressObj.MemoryArea)
            {
                case MemoryAreas.X:
                    if(!IsReadCommand())
                        return "";
                    return "CCX";
                case MemoryAreas.Y:
                    return "CCY";
                case MemoryAreas.R:
                    return "CCR";
                case MemoryAreas.L:
                    return "CCL";
                case MemoryAreas.DT:
                    return "DD";
                case MemoryAreas.FL:
                    return "DF";
                case MemoryAreas.LD:
                    return "DL";
                case MemoryAreas.SV:
                    return "S";
                case MemoryAreas.EV:
                    return "K";
            }

            return "";
        }


        public static uint GetNodeSize(NodeId DataType)
        {
            if (DataType.IdType == IdType.Numeric)
            {
                switch ((uint)(DataType.Identifier))
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
                }
            }
            return 0;
        }

 
        public static int CompareTagByOffset(Tag x, Tag y)
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
                    {
                        if (x.BitOffset > y.BitOffset)
                            return 1;
                        else if (x.BitOffset == y.BitOffset)
                            return 0;
                        else
                            return -1;// x < y
                    }
                    else
                        return -1;// x < y
                }
            }
        }

        public bool GetWriteData(ref ComBuffer requestBuffer)
        {
            //ushort WriteOffset = PartialWriteStart;
            Tag tag;
            object objectData = null;
            lock (lockListObject)
            {
                GetJobData(ref objectData);
                // discard inputoutput/exception output unchanged write value operation
                if (TagsListOnWriting.Count == 0 || objectData == null)
                    return false;
                tag = TagsListOnWriting[0];
            }
            byte[] jobdata = (byte[])objectData;

            if (AddressObj.DataFormat == DataFormats.BOOL)
            {
                ushort NumOfBit ;
                //if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)                    
                //    NumOfBit = (ushort)TagsListOnWriting.Count;
                if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                {
                    NumOfBit = 0;
                    //NumOfBit = (ushort)TagsListOnWriting.Count;
                    foreach (Tag WriteTag in TagsListOnWriting)
                    {
                        NumOfBit += (ushort)(WriteTag.TagNode.ArrayDimension == 0 ? 1 : (ushort)(WriteTag.TagNode.ArrayDimension));
                    }
                }
                else if ( ElementNumber == 0)
                    NumOfBit = (ushort)(TotalJobSize * 8);
                else
                    NumOfBit = (ushort)(TotalJobSize / GetDataTypeByteSize((uint)TagsList[0].TagNode.DataType.Identifier));

                //requestBuffer.insert((byte)(NumOfBit + 48));
                //int bitOffset = (int)tag.BitOffset + WriteOffset;
                //for (int i = 0; i < NumOfBit; i++)
                //{
                //    NaisFpTag TagBit = TagsListOnWriting[i] as NaisFpTag;
                //    NaisFpAddress TagAddress = new NaisFpAddress(TagBit.NaisFpDynSettings.Address);
                //    //format register address for protocol purpose
                //    requestBuffer.insert(TagAddress.GetNaisAddress(TagAddress.DataFormat, 0));
                //    requestBuffer.insert((byte)(jobdata[i] == 0 ? '0' : '1'));
                //}

                requestBuffer.insert((byte)(NumOfBit + 48));
                int i = 0;
                foreach (Tag WriteTag in TagsListOnWriting)
                {
                    NaisFpTag TagBit = WriteTag as NaisFpTag;
                    NaisFpAddress TagAddress = new NaisFpAddress(TagBit.NaisFpDynSettings.Address);

                    if (WriteTag.TagNode.ArrayDimension == 0)
                    {
                        //reformat register address for protocol purpose
                        requestBuffer.insert(TagAddress.GetNaisAddress(TagAddress.DataFormat, 0));
                        requestBuffer.insert((byte)(jobdata[i] == 0 ? '0' : '1'));
                        i++;
                    }
                    else
                    {
                        for (int bit = 0; bit < TagBit.TagNode.ArrayDimension; bit++)
                        {                            
                            //reformat register address for protocol purpose
                            requestBuffer.insert(TagAddress.GetNaisAddress(TagAddress.DataFormat, bit));
                            requestBuffer.insert((byte)(jobdata[i] == 0 ? '0' : '1'));
                            i++;
                        }
                    }
                }
            }
            else
            {
                //add data to write, ask the job...

                requestBuffer.insert(AddressObj.GetNaisAddress(AddressObj.DataFormat, (int)(tag.ByteOffset / 2)));
                requestBuffer.insert(AddressObj.GetNaisAddress(AddressObj.DataFormat, (int)(jobdata.Count() / 2 + tag.ByteOffset / 2 - 1)));

                for (int i = 0; i < jobdata.Count(); i++)
                {
                    requestBuffer.insert(String.Format("{0:X2}", jobdata[i]));
                }
            }

            return true;
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
                l.Add(t);
            return l;
        }

        private void CheckJobValid()
        {
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

            bool bit = false;
            bool num = false;
            uint size = 0;
            foreach (var t in tempList)
            {
                if ((uint)t.DataType.Identifier == (uint)BuiltInType.Boolean)
                {
                    bit = true;
                }
                else
                {
                    num = true;
                }
                size += GetNodeSize(t.DataType);

            }

            // Do not mix bit and non bit variables
            if (bit && num)
            {
                IsValid = false;
                InvalidReason = string.Format(
                    Properties.Resources.ErrorValidateJob,
                    tagnamelist,
                    Properties.Resources.ErrorMixBitsAndOthers);
                return;
            }

            ushort NumOfByte;
            if (AddressObj.DataFormat == DataFormats.BOOL)
            {
                if (TagsList[0].TagNode.DataType != Opc.Ua.DataTypes.Boolean && ElementNumber == 0)
                    NumOfByte = (ushort)TotalJobSize;
                else
                    NumOfByte = (ushort)((AddressObj.BitNumber + TagsList[0].BitOffset + TotalJobSize + 0x07) / 8);
            }
            else
            {
                NumOfByte = (ushort)TotalJobSize;
            }

            if (NumOfByte > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                return;
            }
            
            if (AddressObj.DataFormat == DataFormats.BOOL)
            {
                ushort NumOfBit;
                if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    NumOfBit = (ushort)TotalJobSize;
                else if (ElementNumber == 0)
                    NumOfBit = (ushort)(TotalJobSize * 8);
                else
                    NumOfBit = (ushort)(TotalJobSize / GetDataTypeByteSize((uint)TagsList[0].TagNode.DataType.Identifier));

                if (NumOfBit > 8)
                {
                    IsValid = false;
                    InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                    return;

                }

            }

            string errorDesc;
            if (!ProtocolDataSizeIsValid(out errorDesc))
            {
                IsValid = false;
                InvalidReason = string.Format("The {0} is invalid for the Address. (Tags: {1} Address: {2})", errorDesc, tagnamelist, Address);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public uint GetAggregateMaxJobSize()
        {
            uint AggregLimit = Station.GetCommDriver().AggregationLimit;
            uint JobMaxSize = GetMaxJobSize();
            if ((AggregLimit != 0) && (AggregLimit < JobMaxSize))
            {
                JobMaxSize = AggregLimit;            
            }

            if (Type != LinkType.Input)
            {
                NaisFpAddress addObj = new NaisFpAddress(_Address);
                switch (addObj.DataFormat)
                {
                    // protocol allow to write in a single frame only 8 dirrent bit
                    case DataFormats.BOOL:
                        JobMaxSize = 8;
                        break;                                            
                }
            }
            else
            {
                NaisFpAddress addObj = new NaisFpAddress(_Address);
                switch (addObj.DataFormat)
                {
                    case DataFormats.BOOL:
                        // TestAggregateJob aggreate bit in block of 16 bit 
                        JobMaxSize = ((JobMaxSize /8) * 8) * 8;
                        break;
                }           
            }

            return JobMaxSize;
        }


        public override uint getProtocolDataType()
        {
            NaisFpAddress addObj = new NaisFpAddress(_Address);
            switch (NaisFpProtocol.DataType(addObj))
            {
                case UFUAModel.DataType.UInt32:
                    return (uint)BuiltInType.UInt32;
                case UFUAModel.DataType.UInt16:
                    return (uint)BuiltInType.UInt16;
                case UFUAModel.DataType.Boolean:
                    return (uint)BuiltInType.Boolean;
                default:
                    return 0;
            }
        }

        public override uint GetMaxJobSize()
        {
            return NaisFpProtocol.GetMaxJobSize(((NaisFpStation)Station).FrameFormat);
        }
        
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            if (base.TestAggregateJob(candJob, out ExtraBytes) == JobAggregationType.JobAggregImpossible)
                return JobAggregationType.JobAggregImpossible;

            NaisFpCommJob testJob = candJob as NaisFpCommJob;
            if (testJob == null)
                return JobAggregationType.JobAggregImpossible;

            if (!AddressObj.IsValid || !testJob.AddressObj.IsValid)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (AddressObj.DataFormat != testJob.AddressObj.DataFormat)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if (AddressObj.MemoryArea != testJob.AddressObj.MemoryArea)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            /*
             * exclude var type not admitable, strings or odd size in word areas.
             * states de presence of bit variables or not: bit and numeric tags can't be aggregate together
             */

            if (!IsTypeAdmitted(testJob.TagsList[0].TagNode.DataType) ) 
                return JobAggregationType.JobAggregImpossible;

            bool bBit = AddressObj.isBit();
            bool bTagBit = testJob.AddressObj.isBit();
            if (bBit ^ bTagBit)
            {
                return JobAggregationType.JobAggregImpossible;
            }

            if ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            {
                if ((uint)testJob.TagsList[0].TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                    return JobAggregationType.JobAggregImpossible;
            }

            if ((uint)testJob.TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            {
                if ((uint)TagsList[0].TagNode.DataType.Identifier != (uint)BuiltInType.Boolean)
                    return JobAggregationType.JobAggregImpossible;
            }

            uint Granularity = Station.GetCommDriver().AggregationThreshold;
            uint candJobStartAdd;
            uint JobStartAdd;
            uint candJobEndAdd;
            uint JobEndAdd;
            bool BitFormat = false;
            uint elementsize = 2;

            if (AddressObj.DataFormat == DataFormats.BOOL)
            {                
                BitFormat = true;                
                elementsize = 1;
                Granularity *= 8;
            }


            if (BitFormat)
            {
                candJobStartAdd = (uint)((testJob.AddressObj.StartAddress.USHORT * 16) + testJob.AddressObj.BitNumber + testJob.TagsList[0].BitOffset);
                JobStartAdd = (uint)((AddressObj.StartAddress.USHORT * 16) + AddressObj.BitNumber + TagsList[0].BitOffset);
            }
            else
            {
                candJobStartAdd = (uint)testJob.AddressObj.StartAddress.USHORT;
                JobStartAdd = (uint)AddressObj.StartAddress.USHORT;
            }

            candJobEndAdd = candJobStartAdd + testJob.TotalJobSize / elementsize - 1;
            JobEndAdd = JobStartAdd + TotalJobSize / elementsize - 1;

            /*
             * Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
             */

            // If the intersection between the address ranges of the tag and of the job
            // is not empty, check that there are not collisions among the addresses of
            // the job tags and the ones of the tag.
            if (((candJobStartAdd >= JobStartAdd) && (candJobStartAdd <= JobEndAdd)) ||
                ((candJobEndAdd >= JobStartAdd) && (candJobEndAdd <= JobEndAdd)))
            {
                /*
                 * Control on tag collision, colliding variable can be admitted for Input jobs. Consider for the future.
                 */
                foreach (var tag in TagsList)
                {
                    if (TagAndDynVarCollide(candJobStartAdd, candJobEndAdd,
                                           JobStartAdd, tag, elementsize))
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                }
            }

            // If the address range of the job is strictly included in the range of the
            // tag, the job cannot aggregate. 
            else if ((candJobStartAdd < JobStartAdd) && (candJobEndAdd > JobEndAdd))
            {
                return JobAggregationType.JobAggregImpossible;
            }

            // If there is no intersection between the address ranges of the tag and of
            // the job, check the granularity.
            else
            {
                // Check the granularity.
                if (candJobEndAdd < JobStartAdd)
                {                   
                    if ((JobStartAdd - candJobEndAdd) > (Granularity + 1))
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                }
                else
                {
                    if ((candJobStartAdd - JobEndAdd) > (Granularity + 1))
                    {
                        return JobAggregationType.JobAggregImpossible;
                    }
                }
            }

            if (candJobStartAdd >= JobStartAdd && candJobEndAdd <= JobEndAdd)
            {
                candJob.TagsList[0].ByteOffset = (candJobStartAdd - JobStartAdd) * elementsize;
                return JobAggregationType.JobAggregFits;
            }

            if (candJobStartAdd >= JobStartAdd && candJobStartAdd <= JobEndAdd + Granularity)
            {
                //if (((endTest - startTest + 1) * elementsize) <= GetMaxJobSize())
                if (((candJobEndAdd - JobStartAdd + 1) * elementsize) <= GetAggregateMaxJobSize())
                {
                    candJob.TagsList[0].ByteOffset = (candJobStartAdd - JobStartAdd) * elementsize;
                    ExtraBytes = (candJobEndAdd - JobEndAdd) * elementsize;
                    return JobAggregationType.JobAggregForward;
                }
                return JobAggregationType.JobAggregImpossible;
            }

            uint startBkw;
            if (JobStartAdd > Granularity)
                startBkw = JobStartAdd - Granularity;
            else
                startBkw = 0;
            if (candJobEndAdd >= startBkw && candJobEndAdd <= JobEndAdd)
                if (((JobEndAdd - candJobStartAdd + 1) * elementsize) <= GetAggregateMaxJobSize())
                {
                    candJob.TagsList[0].ByteOffset = 0;
                    ExtraBytes = (JobStartAdd - candJobStartAdd) * elementsize;
                    return JobAggregationType.JobAggregBackward;
                }

            return JobAggregationType.JobAggregImpossible;
        }

        bool TagAndDynVarCollide(uint DynVarStartAdd, uint DynVarEndAdd,
            uint JobStartAddress, Tag tag, uint elementsize)
        {
            uint TagStartAdd = 0;
            uint TagEndAdd = 0;

            TagStartAdd = JobStartAddress + tag.ByteOffset / elementsize;
            TagEndAdd = TagStartAdd + tag.Size / elementsize - 1;

            if (((DynVarStartAdd < TagStartAdd) && (DynVarEndAdd < TagStartAdd)) ||
                ((DynVarStartAdd > TagEndAdd) && (DynVarEndAdd > TagEndAdd)))
            {
                return false;
            }

            return (true);
        }
        
        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch(AggType)
            {
                case JobAggregationType.JobAggregFits:
                    TagsList.Add(new NaisFpTag(candJob.TagsList[0].TagNode, candJob.TagsList[0].ByteOffset, candJob.TagsList[0].BitOffset));
                    break;
                case JobAggregationType.JobAggregForward:
                    TagsList.Add(new NaisFpTag(candJob.TagsList[0].TagNode, candJob.TagsList[0].ByteOffset, candJob.TagsList[0].BitOffset));
                    TotalJobSize += ExtraBytes;
                    break;
                case JobAggregationType.JobAggregBackward:
                    foreach (var tag in TagsList)
                        tag.ByteOffset += ExtraBytes;
                    TagsList.Insert(0, new NaisFpTag(candJob.TagsList[0].TagNode, candJob.TagsList[0].ByteOffset, candJob.TagsList[0].BitOffset));
                    TotalJobSize += ExtraBytes;
                    Address = ((NaisFpCommJob)candJob).Address;
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

        public override bool InputOutputRequiresReadAfterContinuosWrite()
        {
            return base.InputOutputRequiresReadAfterContinuosWriteBase();
        }

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
                        if (!isProtocolBool())
                        {
                            if ((cand.ByteOffset + cand.Size) != listToWrite[0].ByteOffset)
                                break;
                        }
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
                        nData = (UInt16)cand.Size;

                    jobdata = new byte[nData];
                    cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));

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

                UpdateTagsListOnWritingAndTagsListToWrite(listOnWriting);
                listOnWriting.Clear();
                listToWrite.Clear();

                // no data to write --> return null
                if (outData.Count == 0)
                {
                    jobData = null;
                    return;
                }
            }
            
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
                            if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset + AddressObj.BitNumber))
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
        #endregion

        #region Properties
        private string _Address;
        public string Address
        {
            get { return _Address; }
            set
            {
                _Address = value;
                AddressObj.Parse(_Address);
            }
        }

        
        #endregion


    }
}
