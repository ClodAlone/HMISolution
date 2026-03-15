using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace MpiPcAdapter
{
    public class MpiPcAdapterCommJob : CommJob
    {
        #region Constructors
        public MpiPcAdapterCommJob(Station station, MpiPcAdapterCommJobSettings settings)
            : base(station, settings)
        {
            _Area = settings.Area;
            _Format = settings.Format;
            _Trans = settings.Trans;
            _Offset = settings.Offset;
            _Length = settings.Length;
            _DbNumber = settings.DbNumber;
            _Bit = settings.Bit;
            _German = settings.German;
            ExecuteTask = false;
            PartialElementStart = 0;
            PartialElementEnd = 0;


            CheckJobValid();
        }

        public MpiPcAdapterCommJob(Station station, MpiPcAdapterTag defTag)
            : base(station, defTag)
        {

            _Area = defTag.MpiPcAdapterDynSettings.Area;
            _Format = defTag.MpiPcAdapterDynSettings.Format;
            _Trans = defTag.MpiPcAdapterDynSettings.Trans;
            _Offset = defTag.MpiPcAdapterDynSettings.Offset;
            _Length = defTag.MpiPcAdapterDynSettings.Length;
            _DbNumber = defTag.MpiPcAdapterDynSettings.DbNumber;
            _Bit = defTag.MpiPcAdapterDynSettings.Bit;
            _German = defTag.MpiPcAdapterDynSettings.German;
            if (TotalJobSize == 0)
                TotalJobSize = (uint)defTag.MpiPcAdapterDynSettings.Length;
            ExecuteTask = false;
            if (ElementNumber > 0 || ProtocolDataSizeBig())
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }
            PartialElementStart = 0;
            PartialElementEnd = 0;
           
            CheckJobValid();
        }

        public MpiPcAdapterCommJob(Station station)
            : base(station)
        {
            ExecuteTask = false;
            CheckJobValid();
            
        }

        protected MpiPcAdapterCommJob()
        {
            ExecuteTask = false;
            CheckJobValid();
           
        }
        #endregion
        #region data Member
        public bool ExecuteTask;
        public ushort PartialElementStart;
        public ushort PartialElementEnd;

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
                nType == (uint)BuiltInType.UInteger ||
                    nType == (uint)BuiltInType.String
                )
                    return true;
                return false;
            }
            
            return false;
        }
        #endregion
        
        #region Methods

        public int GetNumOfElements( int elementsize)
        {
            if (PartialElementEnd == PartialElementStart)
                return Length;
            else if (PartialElementEnd == 0)
                return Length - PartialElementStart / elementsize;
            else
                return (PartialElementEnd - PartialElementStart)/elementsize;

        }

        public uint GetTagWriteData(byte[] bufferIn, ref byte[] buffer, int elementsize, int index = 0)
        {
            if (PartialElementEnd == PartialElementStart)
            {
                Array.Copy(bufferIn, 0, buffer, index, bufferIn.Length);
                return (uint)bufferIn.Length;
            }
            else
            {
                uint length = (uint)(GetNumOfElements(elementsize) * elementsize);
                if (Format == Step7Format.frmBit)
                {
                    byte indexIn;
                    for (byte i = 0; i < length; i++)
                    {
                        indexIn = (byte)(i + PartialElementStart);
                        if ((bufferIn[indexIn / 8] & (1 << (indexIn % 8))) == 0)
                            buffer[i / 8] &= (byte)((1 << (i % 8))^0xff);
                        else
                            buffer[i / 8] |= (byte)(1 << (i % 8));
                    }
                }
                else
                    Array.Copy(bufferIn, PartialElementStart, buffer, index, length);
                return length;
            }

        }

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

            if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                return;
            }
            string errorDesc;
            if (!ProtocolDataSizeIsValid(out errorDesc))
            {
                IsValid = false;
                InvalidReason = string.Format("The {0} is invalid for the Address. (Tags: {1} )", errorDesc, tagnamelist);
                return;
            }

            if ((uint)tempList[0].DataType.Identifier == (uint)BuiltInType.String && tempList[0].ArrayDimension != 0)
            {
                IsValid = false;
                InvalidReason = string.Format("Unacceptable string array. (Tags: {0} )", tagnamelist);
                return;
            }

            MpiPcAdapterProtocol.SetS7JobLength(this);
            IsValid = true;
            InvalidReason = string.Empty;
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

        public override uint getProtocolDataType()
        {
            switch (MpiPcAdapterProtocol.DataType(Format))
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
            //return MpiPcAdapterProtocol.MAX_JOB_DATA_BYTES;
            return MpiPcAdapterProtocol.MAX_SINGLE_TASK_BYTE;
        }
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            if (base.TestAggregateJob(candJob, out ExtraBytes) == JobAggregationType.JobAggregImpossible)
                return JobAggregationType.JobAggregImpossible;

            MpiPcAdapterCommJob testJob = candJob as MpiPcAdapterCommJob;
            if (testJob == null)
                return JobAggregationType.JobAggregImpossible;

            if(!testJob.IsValid)
                return JobAggregationType.JobAggregImpossible;

            if(Area != testJob.Area)
                return JobAggregationType.JobAggregImpossible;

            if(Format != testJob.Format)
                return JobAggregationType.JobAggregImpossible;

            if(Trans != testJob.Trans)
                return JobAggregationType.JobAggregImpossible;

            if (Area == Step7Area.aD && DbNumber != testJob.DbNumber)
                return JobAggregationType.JobAggregImpossible;

            if (testJob.TagsList.Count() < 1)
                return JobAggregationType.JobAggregImpossible;

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType)))
                return JobAggregationType.JobAggregImpossible;


            NodeId nBool = new NodeId((uint)BuiltInType.Boolean);
            if ((from elem in TagsList
                 where elem.TagNode.DataType == BuiltInType.Boolean
                 select elem).ToList().Count > 0 && testJob.TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
                return JobAggregationType.JobAggregImpossible;

            if ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.String ||
                (uint)candJob.TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.String)
                return JobAggregationType.JobAggregImpossible;

            ExtraBytes = 0;
            uint Granularity = Station.GetCommDriver().AggregationThreshold;
            uint start = (uint)Offset;
            uint startTest = (uint)testJob.Offset;
            uint elementsize = 2;

            if (Format == Step7Format.frmBit)
            {
                start = (uint)(Offset * 8 + Bit);
                startTest = (uint)(testJob.Offset * 8 + testJob.Bit);
            }
            else if ((Format != Step7Format.frmByte) && (Area != Step7Area.aT) && (Area != Step7Area.aC))
            {
                if (Format == Step7Format.frmDWord)
                    elementsize = 4;
                if(((Math.Max(start, startTest) - Math.Min(start, startTest))%elementsize) != 0)
                    return JobAggregationType.JobAggregImpossible;
            }

            uint nJobSize = TotalJobSize;
	        uint endTest;
	        uint end;
	        switch( Area ) {
		        case  Step7Area.aD:
		        case Step7Area.aP:
		        case Step7Area.aQ: // Byte addresses also for the bit areas 
		        case Step7Area.aI: // Byte addresses also for the bit areas
		        case Step7Area.aM: // Byte addresses also for the bit areas
		        case Step7Area.aAI:
		        case Step7Area.aAQ:
		        case Step7Area.aS:
		        case Step7Area.aPE:
		        case Step7Area.aPA:

			        if (Trans !=  Step7WordTrans.wtT)
			        {
				        endTest = startTest + testJob.TotalJobSize - 1;
				        end = start + nJobSize - 1;
			        }else{
				        endTest = startTest + testJob.TotalJobSize/2 - 1;
				        end = start + nJobSize/2 - 1;
			        }
	            break;

		        case Step7Area.aC:
		        case Step7Area.aTIEC:
		        case Step7Area.aCIEC:
                    endTest = startTest + testJob.TotalJobSize / 2 - 1;
                    end = start + nJobSize / 2 - 1;
	            break;

		        case Step7Area.aT:
                case Step7Area.aH: // DWord addresses for this data area
                    endTest = startTest + testJob.TotalJobSize / 4 - 1;
                    end = start + nJobSize / 4 - 1;
		        break;
		        default:
                    return JobAggregationType.JobAggregImpossible;
		        break;
	        }

            if( ((startTest >= start) && (startTest <= end)) || 
                ((endTest >= start) && (endTest <= end)) ) {
		
                foreach (var tag in TagsList)
                {
                    uint startTag = start + tag.ByteOffset;
                    uint endTag = startTag + (tag.Size) - 1;
                    
                    switch (Area) {
                        case Step7Area.aC:
                        case Step7Area.aTIEC:
                        case Step7Area.aCIEC:
                            startTag = start + tag.ByteOffset/2;
                            endTag = startTag + (tag.Size/2) - 1;
                            break;
                        case Step7Area.aT:
                        case Step7Area.aH:
                            startTag = start + tag.ByteOffset/4;
                            endTag = startTag + (tag.Size/4) - 1;
                            break;
                    }
                    if ((startTest >= startTag && startTest <= endTag)
                        || (endTest >= startTag && endTest <= endTag))
                        return JobAggregationType.JobAggregImpossible;
                }
		    }
	        // If the address range of the job is strictly included in the range of the
	        // tag, the job cannot aggregate 
	        else if( (startTest < start) && (endTest > end) ) {
		        return JobAggregationType.JobAggregImpossible;
	        }
	        // If there is no intersection between the address ranges of the tag and of
	        // the job, check the granularity.
	        else 
	        {
		        if( Format == Step7Format.frmBit ) {
			        Granularity *= 8;
		        }
		        if( endTest < start )
                {
			        if( (start - endTest) > (Granularity + 1) ) {
				        return JobAggregationType.JobAggregImpossible;
			        }
		        }
		        else if( (startTest - end) > (Granularity + 1) ) {
			        return JobAggregationType.JobAggregImpossible;
		        }
	        }

            // Added to solve FOGBUGZ 11410 and 11411
            if ((Area != Step7Area.aT) && (
                (Area == Step7Area.aC) ||
                (Area == Step7Area.aTIEC) ||
                (Area == Step7Area.aCIEC)
                || (Trans == Step7WordTrans.wtT)
                ))
            {
                elementsize = 2; // Words
            }
            else if ((Area == Step7Area.aT) ||
                    (Area == Step7Area.aH))
            {
                elementsize = 4; // DWords
            }
            else
            {
                elementsize = 1; // Bytes or Bits
            }

	        // If the address range of the tag is included in the range of the job, the
	        // job can aggregate the new data without any extension
	        if( (startTest >= start) && (endTest <= end) ) {
		        ExtraBytes = 0;

                if (candJob.TagsList.Count > 1)
                {
                    uint initfit = (startTest - start);
                    foreach (var tag in candJob.TagsList)
                    {
                        // Modified to solve FOGBUGZ 11410 and 11411
                        //tag.ByteOffset = tag.ByteOffset + initfit;
                        tag.ByteOffset = tag.ByteOffset + initfit * elementsize;
                    }
                }
                else
                {
                    // Modified to solve FOGBUGZ 11410 and 11411
                    //candJob.TagsList[0].ByteOffset = (startTest - start);
                    candJob.TagsList[0].ByteOffset = (startTest - start) * elementsize;
                }
		        return JobAggregationType.JobAggregFits;
	        }

	        // Calculate and check the new job size
	        uint nNewStartAdd = Math.Min( startTest, start);
	        uint nNewEndAdd = Math.Max( endTest, end );
	        uint nNewJobSize = 0;

	        if((Area !=  Step7Area.aT) && (
		        ( Area == Step7Area.aC ) ||
		        ( Area == Step7Area.aTIEC ) ||
		        ( Area == Step7Area.aCIEC ) 
		        || (Trans == Step7WordTrans.wtT)
		        )) {
			        nNewJobSize = (nNewEndAdd - nNewStartAdd + 1) * 2; // Words
	        }
	        else if((Area == Step7Area.aT) ||
                    (Area == Step7Area.aH))
            {
		        nNewJobSize = (nNewEndAdd - nNewStartAdd + 1)*4; // DWords
	        }
	        else {
		        nNewJobSize = nNewEndAdd - nNewStartAdd + 1; // Bytes
	        }

            if (nNewJobSize > GetAggregateMaxJobSize())
            {
                return JobAggregationType.JobAggregImpossible;
            }


	        // The job can aggregate the new data. Calculate the extension required
	        ExtraBytes = nNewJobSize - nJobSize;

	        if( endTest <= end ) {
                uint initoffset = candJob.TagsList[0].ByteOffset;
                foreach (var tag in candJob.TagsList)
                {
                    tag.ByteOffset = tag.ByteOffset - initoffset;
                }
                //candJob.TagsList[0].ByteOffset = 0;
                return JobAggregationType.JobAggregBackward;
	        }

            uint initcand = (startTest - start);
            foreach (var tag in candJob.TagsList)
            {
                // Modified to solve FOGBUGZ 11410 and 11411
                //tag.ByteOffset = tag.ByteOffset + initcand;
                tag.ByteOffset = tag.ByteOffset + initcand * elementsize;
            }

            //candJob.TagsList[0].ByteOffset = (startTest - start);
            return JobAggregationType.JobAggregForward;
        }
        
        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            /*
             * Fits: add tag with correct offset
             * Forward: tag added extend job to higher addresses. Calculate new job size, Start address doesn't change.
             * Backward: tag added extend job to lower addresses. Calculate new job size and new Start address.
             */
            MpiPcAdapterCommJob mJ = candJob as MpiPcAdapterCommJob;
            switch (AggType)
            {
                case JobAggregationType.JobAggregFits:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new MpiPcAdapterTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    break;
                case JobAggregationType.JobAggregForward:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new MpiPcAdapterTag(tag.TagNode, tag.ByteOffset, 0);
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
                        var t = new MpiPcAdapterTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    TotalJobSize += ExtraBytes;
                    Offset = ((MpiPcAdapterCommJob)candJob).Offset;
                    break;
                default:
                    return false;
            }

            CheckJobValid();

            return true;
        }

        public override void GetJobData(ref object jobData)
        {
            var listToWrite = new List<Tag>();
            var listOnWriting = new List<Tag>();
            Tag startTag = null;
            List<byte> outData = new List<byte>();

            lock (lockListObject)
            {
                //The synchronous writings used in the recipes are task UnconditionalOutput assignments and are not inser in the TagsListToWrite
                //if (Type == LinkType.UnconditionalOutput)
                //    FillWholeTagListToWrite();

                //if (TagsListToWrite.Count == 0)
                //{
                //    TagsListOnWriting.Clear();
                //    return;
                //}
                listToWrite.AddRange(TagsListOnWriting);
                startTag = listToWrite[0];
                //TagsListToWrite.Clear();
                //}

                uint len = 0; //data area length
                //List<byte> outData = new List<byte>();
                //prepare a write request
                listToWrite.Sort(CompareTagByOffset);

                Tag cand = null;
                byte[] jobdata;
                UInt16 nData = 0;
                bool bLookForFirstTag = true;
                int IndexstartTag = listToWrite.IndexOf(startTag);


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

                    //if (cand != null && (cand.ByteOffset + cand.Size != listToWrite[0].ByteOffset))
                    //    break;

                    //cand = listToWrite[0];

                    //add data
                    uint tsize;

                    //if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                    if (isProtocolBool())
                    {
                        nData = (UInt16)((cand.Size + 7) / 8);
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
                        tsize = cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                    }
                    uint ArraySize = cand.TagNode.ArrayDimension;
                    if (ArraySize == 0)
                    {
                        ArraySize = 1;
                    }
                    if (cand.TagNode.DataType.IdType == IdType.Numeric && (uint)cand.TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                    {
                        if (tsize < cand.Size)
                        {
                            uint diff = (cand.Size - tsize);
                            for (int i = 0; i < diff; i++)
                            {
                                jobdata[len + tsize + i] = 0x20;
                            }
                            tsize += diff;
                        }
                        else
                        {
                            uint diff = (tsize - cand.Size);
                            tsize -= diff;
                        }
                    }
                    else if (ProtocolDataSizeBig())
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
                            cand.getWriteValueFromMemRW(ref tmpdata, sizeDataType, sizeProtocolData, ElementNumber, ArrayIndex);
                            correctData.AddRange(tmpdata);
                        }
                        jobdata = correctData.ToArray();
                        tsize = (uint)jobdata.Count();
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

                    //listToWrite.Remove(cand);
                    //if (!listOnWriting.Contains(cand))
                    //    listOnWriting.Add(cand);

                    if (Format != Step7Format.frmBit)
                    {
                        MpiPcAdapterProtocol.PrepareDataWrite(ref jobdata, Format, Trans, 0, tsize);
                        if ((Area == Step7Area.aT) || (Trans == Step7WordTrans.wtT))
                            tsize /= 2;
                        if (SwapBytes)
                            SwapByteBuffer(ref jobdata, (int)len, (int)tsize);
                        if (SwapWords)
                            SwapWordBuffer(ref jobdata, (int)len, (int)tsize);
                    }

                    len += tsize;
                    outData.AddRange(jobdata);

                    if (Format == Step7Format.frmBit && outData.Count() > 0)
                        break;

                } while (listToWrite.Count > 0);

                //lock (lockListObject)
                //{

                UpdateTagsListOnWritingAndTagsListToWrite(listOnWriting);
                listOnWriting.Clear();
                listToWrite.Clear();

                //}

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
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            base.SetJobData(jobData, ref changed);
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;
            //for (int i = 0; i < TagsList.Count; i++)
            lock(lockListObject)
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
                        return 0;
                    else
                        return -1;

                }
            }
        }

        #endregion

        #region Properties
        private bool _WriteExecuted;//steve 201011
        public bool WriteExecuted
        {
            get { return _WriteExecuted; }
            set
            {
                _WriteExecuted = value;
            }
        }
        
        
        private Step7Area _Area;
        public Step7Area Area
        {
            get { return _Area; }
            set
            {
                _Area = value;
            }
        }

        private Step7Format _Format;
        public Step7Format Format
        {
            get { return _Format; }
            set { _Format = value; }
        }
        private Step7WordTrans _Trans;
        public Step7WordTrans Trans
        {
            get { return _Trans; }
            set { _Trans = value; }
        }
        private int _Offset;
        public int Offset
        {
            get { return _Offset; }
            set { _Offset = value; }
        }
        private int _Length;
        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }
        private int _DbNumber;
        public int DbNumber
        {
            get { return _DbNumber; }
            set { _DbNumber = value; }
        }
        private int _Bit;
        public int Bit
        {
            get { return _Bit; }
            set { _Bit = value; }
        }
        private bool _German;
        public bool German
        {
            get { return _German; }
            set { _German = value; }
        }

        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return ret;
            }
        }

        #endregion
    }
}
