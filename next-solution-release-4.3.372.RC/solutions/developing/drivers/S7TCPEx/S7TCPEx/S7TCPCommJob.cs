using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using System.Threading.Tasks;
using DriverBaseInterfaces;

namespace S7TCP
{
    public class S7TCPCommJob : CommJob
    {
        #region Constructors
        public S7TCPCommJob(Station station, S7TCPCommJobSettings settings)
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
            _S7_200 = settings.S7_200;
            _LenStringEnable = settings.LenStringEnable;
            _SwapDWords = settings.SwapDWords;

            InitOffsetVariableManagement();

            CheckJobValid();
        }

        public S7TCPCommJob(Station station, S7TCPTag defTag)
            : base(station, defTag)
        {
            _Area = defTag.S7TCPDynSettings.Area;
            _Format = defTag.S7TCPDynSettings.Format;
            _Trans = defTag.S7TCPDynSettings.Trans;
            _Offset = defTag.S7TCPDynSettings.Offset;
            _Length = defTag.S7TCPDynSettings.Length;
            _DbNumber = defTag.S7TCPDynSettings.DbNumber;
            _Bit = defTag.S7TCPDynSettings.Bit;
            _German = defTag.S7TCPDynSettings.German;
            _S7_200 = defTag.S7TCPDynSettings.S7_200;
            _LenStringEnable = defTag.S7TCPDynSettings.LenStringEnable;
            if (TotalJobSize == 0)
                TotalJobSize = (uint)defTag.S7TCPDynSettings.Length;
            if (ElementNumber > 0 || ProtocolDataSizeBig())
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }
            _SwapDWords = defTag.S7TCPDynSettings.SwapDWords;

            if (TagsList[0].TagNode.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)TagsList[0].TagNode.DataType.Identifier)
                {
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        Length *= 8;
                        break;
                }
            }

            InitOffsetVariableManagement();

            CheckJobValid();
        }

        public S7TCPCommJob(Station station)
            : base(station)
        {
            InitOffsetVariableManagement();

            CheckJobValid();
        }

        protected S7TCPCommJob()
        {
            InitOffsetVariableManagement();

            CheckJobValid();
           
        }
        #endregion
        #region data Member

        public byte WriteItems;
        
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

        private void CheckJobValid()
        {
                
            bool bit = false;
            bool num = false;

            string tagnamelist = string.Empty;

            // skip any kind of controls for Atomic Struct Job --> controls are valid only for standard job
            if (!IsStructAtomic())
            {
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

                //Added (FOGBUGZ 19139)
                uint jobByteSize = TotalJobSize;                
                if (!IsBigArray() && (bit == true) && (TagsList[0].TagNode.ArrayDimension != 0))
                {
                    if (jobByteSize > 8)
                    {
                        jobByteSize /= 8;
                        if ((TotalJobSize % 8) > 0)
                        {
                            jobByteSize++;
                        }
                    }
                }

                // for string with len > 200, driver force internal size to fit data into 1 message
                if ((uint)tempList[0].DataType.Identifier == (uint)BuiltInType.String && _LenStringEnable)
                    LimitBigStringSize(ref jobByteSize);

                if (!IsBigArray() && (jobByteSize > GetMaxJobSize()))                
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
                if (isProtocolBool() && (uint)tempList[0].DataType.Identifier != (uint)BuiltInType.Boolean)
                {
                    IsValid = false;
                    InvalidReason = string.Format("The {0} is data type incompatible for the Address. (Tags: {1} )", errorDesc, tagnamelist);
                    return;
                }
                if ((uint)tempList[0].DataType.Identifier == (uint)BuiltInType.String && tempList[0].ArrayDimension != 0)
                {
                    IsValid = false;
                    InvalidReason = string.Format("Unacceptable string array. (Tags: {0} )", tagnamelist);
                    return;
                }

                //if ((uint)tempList[0].DataType.Identifier == (uint)BuiltInType.String && Length > 212)
                //{
                //    IsValid = false;
                //    InvalidReason = string.Format("String exceed the maximum size (Tags: {0})", tagnamelist);
                //    return;
                //}

                S7Protocol.SetS7JobLength(this, TotalJobSize/*size*/);
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public bool IsBigArray(Tag defTag = null)
        {
            if (defTag == null)
                defTag = TagsList[0];

            if (defTag.TagNode.DataType.IdType != IdType.Numeric)
                return false;

            bool ret = false;
            uint val = (uint)defTag.TagNode.DataType.Identifier;
            if (defTag.TagNode.ArrayDimension != 0)
            {
                switch(val)
                {
                    case (uint)Opc.Ua.DataTypes.Boolean:
                        {
                            uint byteSize = defTag.Size / 8;
                            if ((defTag.Size % 8) > 0)
                            {
                                byteSize++;
                            }
                            if (byteSize > ((S7TCPChannel)Station.GetChannel()).MAX_DATA_BYTES)
                            {
                                ret = true;
                            }
                        }
                        break;
                    case (uint)Opc.Ua.DataTypes.String:
                        break;
                    default:
                        if(defTag.Size > ((S7TCPChannel)Station.GetChannel()).MAX_DATA_BYTES)
                        {
                            ret = true;
                        }
                        break;
                }
            }
            //else if(val == (uint)Opc.Ua.DataTypes.String)
            //{
            //    uint sizeToBeChecked = defTag.Size;
            //    if(((S7TCPDynTagSettings)defTag.DynSettings).LenStringEnable == true)
            //    {
            //        sizeToBeChecked++;
            //    }
            //    if (sizeToBeChecked > ((S7TCPChannel)Station.GetChannel()).MAX_DATA_BYTES)
            //    {
            //        ret = true;
            //    }
            //}

            return ret;
        }
        
        public bool IsBitArray(Tag defTag = null)
        {
            if (defTag == null)
                defTag = TagsList[0];

            return S7Protocol.IsBitArrayTag(defTag);
        }

        // Added to solve FOGBUGZ 13224
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
            // Removed to solve FOGBUGZ 13224
            //if (Station.GetCommDriver().AggregationLimit != 0)
            //    return Station.GetCommDriver().AggregationLimit;

            return ((S7TCPChannel)Station.GetChannel()).MAX_DATA_BYTES;
        }

        public override uint getProtocolDataType()
        {
            switch (S7Protocol.DataType(Format))
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
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            if (base.TestAggregateJob(candJob, out ExtraBytes) == JobAggregationType.JobAggregImpossible)
                return JobAggregationType.JobAggregImpossible;

            S7TCPCommJob testJob = candJob as S7TCPCommJob;
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

            if (!IsTypeAdmitted((testJob.TagsList[0].TagNode.DataType)))
                return JobAggregationType.JobAggregImpossible;

            if (IsBigArray() || testJob.IsBigArray())
                return JobAggregationType.JobAggregImpossible;

            if ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.String ||
                (uint)candJob.TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.String)
                return JobAggregationType.JobAggregImpossible;

            if (SwapDWords != testJob.SwapDWords)
                return JobAggregationType.JobAggregImpossible;

            NodeId nBool = new NodeId((uint)BuiltInType.Boolean);
            if ((from elem in TagsList
                 where elem.TagNode.DataType == BuiltInType.Boolean
                 select elem).ToList().Count > 0 && testJob.TagsList[0].TagNode.DataType != nBool/*BuiltInType.Boolean*/)
                return JobAggregationType.JobAggregImpossible;

            // a custom job (bit array, big array, atomic struc, etc) cannot be aggregated with other jobs
            if (IsCustomJob() || testJob.IsCustomJob())
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
                if (testJob.TagsList[0].TagNode.ArrayDimension > 0)
                {
                    return JobAggregationType.JobAggregImpossible;
                }
            }
            else if ((Format != Step7Format.frmByte) && (Area != Step7Area.aT) && (Area != Step7Area.aC))
            {
                if (Format == Step7Format.frmDWord)
                    elementsize = 4;
                if(((Math.Max(start, startTest) - Math.Min(start, startTest))%elementsize) != 0)
                    return JobAggregationType.JobAggregImpossible;
            }

            if (SwapDWords && (GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier) < 64 || GetDataTypeBitSize((uint)candJob.TagsList[0].TagNode.DataType.Identifier) < 64))
                return JobAggregationType.JobAggregImpossible;

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

            if (testJob.OffsetVariableSet)
            {
                // don't aggregate "simple" bit tag with offset variable --> if added value exceed byte sinze, don't work
                if (!(((S7TCPTag)testJob.TagsList[0]).IsMemberOfStruct) && testJob.Format == Step7Format.frmBit)
                    return JobAggregationType.JobAggregImpossible;

                // when offset variabile is set, don't aggreate simple tag with member of struct --> offset variable value change (member of struct = bytes; simple tag = bits)
                if (this.OffsetVariableSet)
                {
                    if (((S7TCPTag)this.TagsList[0]).IsMemberOfStruct && !((S7TCPTag)testJob.TagsList[0]).IsMemberOfStruct)
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
            // Modified to solve FOGBUGZ 13224
            //if( nNewJobSize > GetMaxJobSize() )	{
	        if( nNewJobSize > GetAggregateMaxJobSize() )	{
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
            switch (AggType)
            {
                case JobAggregationType.JobAggregFits:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new S7TCPTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    break;
                case JobAggregationType.JobAggregForward:
                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new S7TCPTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    TotalJobSize += ExtraBytes;
                    break;
                case JobAggregationType.JobAggregBackward:
                    foreach (var tag in TagsList)
                        tag.ByteOffset += ExtraBytes;

                    foreach (var tag in candJob.TagsList)
                    {
                        var t = new S7TCPTag(tag.TagNode, tag.ByteOffset, 0);
                        t.Value.Value = Utils.Clone(tag.Value.Value);
                        TagsList.Add(t);
                    }
                    TotalJobSize += ExtraBytes;
                    Offset = ((S7TCPCommJob)candJob).Offset;
                    break;
                default:
                    return false;
            }

            CheckJobValid();

            return true;
        }


        public override bool InputOutputRequiresReadAfterContinuosWrite()
        {
            return base.InputOutputRequiresReadAfterContinuosWriteBase();
        }

        public override void GetJobData(ref object jobData)
        {
        }

        public bool LimitatedGetJobData(ref object jobData, long RequestSize,out S7TCPTag firstTag)
        {
            var listOnWriting = new List<Tag>();
            firstTag = null;
            
            uint len = 0; //data area length
            List<byte> outData = new List<byte>();
            //prepare a write request
            Tag cand = null;
            byte[] jobdata;
            UInt16 nData = 0;
            bool rt = true;
            long currentRequestSize = RequestSize;

            List<Tag> listToWrite = new List<Tag>();
            listToWrite.AddRange(TagsListOnWriting);

            do
            {
                if (cand != null)
                {
                    if(cand.ByteOffset + cand.Size != listToWrite[0].ByteOffset)
                        break;
                }
                else
                    firstTag = (listToWrite[0] as S7TCPTag);

                cand = listToWrite[0];

                //if (currentRequestSize >= S7Protocol.MAX_TEL_LENGTH && isProtocolBool() && cand.TagNode.ArrayDimension != 0)
                //{
                //    rt = false;
                //    break;
                //}

                //add data
                uint tsize;

                if (isProtocolBool())
                {
                    nData = 1;
                }
                    
                else if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                    nData = (UInt16)cand.Size;
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
                    jobdata = new byte[nData];
                   
                    tsize = cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                }

                uint ArraySize = cand.TagNode.ArrayDimension;
                if (ArraySize == 0)
                    ArraySize = 1;
                if (cand.TagNode.DataType.IdType == IdType.Numeric && (uint)cand.TagNode.DataType.Identifier == Opc.Ua.DataTypes.String)
                {
                    byte inTsize = (byte)tsize;
                    if (tsize < cand.Size)
                    {
                        uint diff = (cand.Size - tsize);
                        byte defaultChar = (byte)(isLenStringEnable ? 0x00 : 0x20);
                        for (int i = 0; i < diff; i++)
                        {
                            jobdata[len + tsize + i] = defaultChar;
                        }
                        tsize += diff;
                    }
                    else
                    {
                        uint diff = (tsize - cand.Size);
                        tsize -= diff;
                    }
                    if (isLenStringEnable)
                    {   
                        byte[]tmpdata = new byte[tsize + 1];
                        Array.Copy(jobdata, 0, tmpdata, 1, tsize);
                        // empty string return (from base class) inTsize = 1; check if 1st character is empty (0x00) --> empty string
                        if ((jobdata[0] == 0 && inTsize == 1) || ((jobdata[0] == 0) && (jobdata[1] == 0)))
                            inTsize = 0;
                        tmpdata[0] = inTsize;
                        jobdata = tmpdata;
                        tsize++;
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
                    tsize = (uint)jobdata.Count();
                }
                else if (isProtocolBool())
                {
                    if (ElementNumber == 0 || (uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                    {
                        byte[] tmpData = new byte[GetDataTypeBitSize((uint)cand.TagNode.DataType.Identifier)];
                        tmpData[0] = (byte)(jobdata[0] & 1);
                        jobdata = tmpData;
                    }
                }

                if (Format != Step7Format.frmBit)
                {
                    S7Protocol.PrepareDataWrite(ref jobdata, 0, tsize, Format, Trans);
                    if ((Area == Step7Area.aT) || (Trans == Step7WordTrans.wtT))
                        tsize /= 2;
                    if (SwapBytes)
                    {
                        if (!isLenStringEnable || ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.String) || (tsize <= 1))
                        {
                            SwapByteBuffer(ref jobdata, 0, (int)tsize);
                        }
                        else
                        {
                            SwapByteBuffer(ref jobdata, 1, (int)(tsize - 1));
                        }
                    }
                    if (SwapWords)
                        SwapWordBuffer(ref jobdata, 0, (int)tsize);
                    if(SwapDWords)
                        SwapDWordBuffer(ref jobdata, 0, (int)tsize);
                }

                if ( jobdata.Length + currentRequestSize > ((S7TCPChannel)Station.GetChannel()).MAX_MPI_TLG_LEN )
                {
                    rt = false;
                    break;
                }

                currentRequestSize += jobdata.Length;
                //after the checks here the "jobdata" buffer arrives with the data size as UIN32, 
                //if it is a data type S5Time, the buffer must be compacted to Word
                if ((Area == Step7Area.aT) || (Trans == Step7WordTrans.wtT))
                {
                    byte[] tmpjobdata = new byte[tsize];
                    for (uint index = 0,  indexSor = 0; index < tsize; index += 2, indexSor += 4)
                    { 
                        tmpjobdata[index] = jobdata[indexSor];
                        if((index + 1) < tsize)
                            tmpjobdata[index + 1] = jobdata[indexSor + 1];
                    }
                    jobdata = tmpjobdata;
                }

                len += tsize;                
                outData.AddRange(jobdata);

                if (!listOnWriting.Contains(listToWrite[0]))
                    listOnWriting.Add(listToWrite[0]);
                if ((uint)cand.TagNode.DataType.Identifier != (uint)BuiltInType.Boolean || cand.TagNode.ArrayDimension == 0 )
                {
                    listToWrite.RemoveAt(0);
                }
               
                if (Format == Step7Format.frmBit && outData.Count() > 0)
                    break;

            } while (listToWrite.Count > 0);

            UpdateTagsListOnWritingAndTagsListToWrite(listOnWriting);

            listToWrite.Clear();
            listOnWriting.Clear();

            jobData = outData.ToArray();
            return rt;
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            base.SetJobData(jobData, ref changed);
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;
            if (SwapDWords)
                SwapDWordBuffer(ref rec);
            //for (int i = 0; i < TagsList.Count; i++)
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

        public static void SwapDWordBuffer(ref byte[] buf, int init = 0, int len = 0)
        {
            byte[] _temp;
            if (buf.Length == 0 || init + len > buf.Length)
                return;
            int max = (len <= 0 ? buf.Length : (/*init +*/ len));
            if (max % 8 != 0)
                return;
            _temp = new byte[max];
            Buffer.BlockCopy(buf, init + 4, _temp, 0, max - 4);
            for (int i = 0; i < max; i += 8)
            {
                _temp[i + 4] = buf[init + i];
                _temp[i + 4 + 1] = buf[init + i + 1];
                _temp[i + 4 + 2] = buf[init + i + 2];
                _temp[i + 4 + 3] = buf[init + i + 3];
            }
            Buffer.BlockCopy(_temp, 0, buf, init, max);
        }
        #endregion

        #region Offset variable Management
        public override void ManageUpdatedValueForTheOffsetVariable(NodeId node, DataValue value)
        {
            base.ManageUpdatedValueForTheOffsetVariable(node, value);

            if (GetOffsetVariableIntNumericValue(out Int64 offsetValue))
            {
                _AddressOffsetValue = (Int32)offsetValue;
            }

            if (GetOffsetVariableArrayIntNumericValue(out Int64[] offsetArrayValues))
            {
                if (offsetArrayValues.Length == 2)
                {
                    _AddressOffsetValue = (Int32)offsetArrayValues[0];
                    _DBOffsetValue = (Int32)offsetArrayValues[1];
                }
            }
        }

        /// <summary>
        /// Calculate variable communication address using offset value parameters (is set)
        /// </summary>
        /// <param name="dbOffsetValue"></param>
        /// <param name="addressOffsetValue"></param>
        public void CalculateAddressWithOffsetValues(Int32? dbOffsetValue = null, Int32? addressOffsetValue = null)
        {
            if (dbOffsetValue != null)
                _DBOffsetValue = dbOffsetValue;

            if (addressOffsetValue != null)
                _AddressOffsetValue = addressOffsetValue;

            if (_DBOffsetValue.HasValue)
            {
                _DbNumber = _OriginalDbNumber + (int)_DBOffsetValue;
                _DBOffsetValue = null;
            }

            if (_AddressOffsetValue.HasValue)
            {
                if (Format == Step7Format.frmBit)
                {
                    // when member of struct, change address "byte" part only
                    if (((S7TCPTag)TagsList[0]).IsMemberOfStruct)
                    {
                        _Offset = _OriginalOffset + (int)_AddressOffsetValue;
                    }
                    else
                    {
                        // calculate new address, including byte's part
                        int addBytes = Math.DivRem((_OriginalOffset * 8) + _OriginalBit + (int)_AddressOffsetValue, 8, out int restBit);
                        _Offset = (int)addBytes;
                        _Bit = restBit;
                        // don't allow _Bit < 0, otherwhise GetReadRequestLength() fail to calculate correct frame size
                        // when _Bit <0, _Offset = 0
                        if (_Bit < 0)
                            _Bit = 0;
                    }
                }
                else
                {
                    _Offset = _OriginalOffset + (int)_AddressOffsetValue;
                }

                _AddressOffsetValue = null;
            }
        }

        private void InitOffsetVariableManagement()
        {
            _OriginalOffset = _Offset;
            _OriginalDbNumber = _DbNumber;
            _OriginalBit = _Bit;
            _DBOffsetValue = null;
            _AddressOffsetValue = null;
        }

        /// <summary>
        /// Return an error message contain the current address --> address calculated with offset variable value 
        /// </summary>
        /// <returns></returns>
        public string GetOffsetVariableAddionalErrorInfo()
        {
            string result = string.Empty;
            string currenAddress = string.Empty;

            if (this.TagsList.Count > 0)
                result = this.TagsList[0].DynSettings.ToString();

            result += " , ";

            currenAddress = string.Format("Data block:{0}, Address:{1}", _DbNumber, _Offset);
            if (Format == Step7Format.frmBit)
                currenAddress += string.Format(" ,Bit:{0}", _Bit);

            result += string.Format(Properties.Resources.CurrentAddressWithOffsetVariableValue, currenAddress);

            return result;
        }
        #endregion

        #region Properties        
        public bool WriteExecuted
        {
            get { return WriteItems > 0; }
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
            get { return isLenStringEnable ? _Offset - 1 : _Offset; }
            set { _Offset = value; }
        }
        private int _Length;
        public int Length
        {
            get { return isLenStringEnable ? _Length + 1 : _Length ; }
            set { _Length = value ; }
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
        private bool _S7_200;
        public bool S7_200
        {
            get { return _S7_200; }
            set { _S7_200 = value; }
        }

        private bool _LenStringEnable;
        public bool LenStringEnable
        {
            get { return _LenStringEnable; }
            set { _LenStringEnable = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the exchanged tag. </summary>
        ///
        /// <value> The exchanged tag. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint S7CommJobExchangedTag
        { get; set; }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the exchanged byte. </summary>
        ///
        /// <value> The exchanged byte. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint S7CommJobExchangedByte
        { get; set; }

        public bool isLenStringEnable
        {
            get { return ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.String && _Offset > 0 && _LenStringEnable); }

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

        public uint GetReadRequestLength()
        {
            return 12;
        }
        public uint GetReadResponseLength()
        {
            uint nBytes = 0;
            if (!IsValid)
                return nBytes;

            switch (Format)
            { 
                case Step7Format.frmBit:
                    {
                        uint nTotBits = (uint) (Bit + Length);
	                    nBytes = nTotBits / 8;
	                    if( nTotBits % 8 != 0 ) 
		                    nBytes ++;
	                }
                    break;
                case Step7Format.frmByte:
                    nBytes = (uint)Length;
                    break;
                case Step7Format.frmDWord:
                    nBytes = (uint) Length * 4;
                    break;
                case Step7Format.frmWord:
                    nBytes = (uint) Length * 2;
                    break;
            }

            return 4 + nBytes;
        }

        public uint GetWriteRequestLength()
        {
            const uint HeaderWriteAnyPointer = 12;
            const uint HeaderWriteVarValue = 4;
            
            uint size = 0;
            
            if (this.Type == LinkType.UnconditionalOutput)
            {
                size += TotalJobSize;
            }
            else if (IsStructAtomic())
            {
                if (TotalJobSize > ((S7TCPChannel)Station.GetChannel()).MAX_DATA_BYTES)
                    size += ((S7TCPChannel)Station.GetChannel()).MAX_DATA_BYTES;
                else
                    size += TotalJobSize;
            }
            else
            {
                List<Tag> listToWrite = new List<Tag>();
                List<Tag> actuallist = new List<Tag>();
                lock (lockListObject)
                {
                    actuallist.AddRange(GetTagListOnWriting());
                }

                if (Area != Step7Area.aT && Trans != Step7WordTrans.wtT)
                {
                    int i = 0;
                    listToWrite.Add(actuallist[0]);
                    while (i < actuallist.Count)
                    {
                        size += actuallist[i].Size;
                        if (i < actuallist.Count - 1)
                        {
                            // struct atomic contains different data type member not contiguous --> skip control
                            if (actuallist[i + 1].ByteOffset != actuallist[i].ByteOffset + actuallist[i].Size)
                                break;
                            else
                                listToWrite.Add(actuallist[i + 1]);
                        }
                        i++;
                    }

                    if (Format == Step7Format.frmBit)
                    {
                        // change size from bit to byte only for array
                        if (TagsList.Count == 1 && TagsList[0].TagNode.ArrayDimension > 0)
                        {
                            if ((size % 8) != 0)
                                size++;
                            size /= 8;
                        }
                    }
                    else if (isLenStringEnable == true)
                    {
                        size++;
                    }
                }
                else
                {
                    int i = 0;
                    actuallist.Sort(S7Protocol.CompareTagByOffset);
                    listToWrite.Add(actuallist[0]);
                    while (i < actuallist.Count)
                    {
                        size += actuallist[i].Size;
                        if (i < actuallist.Count - 1)
                        {
                            if (actuallist[i + 1].ByteOffset != actuallist[i].ByteOffset + actuallist[i].Size)
                                break;
                            else
                                listToWrite.Add(actuallist[i + 1]);
                        }
                        i++;
                    }
                }

                UpdateTagsListOnWritingAndTagsListToWrite(listToWrite);
            }

            size += (HeaderWriteAnyPointer + HeaderWriteVarValue);

            return size;
        }

        public int GetDataLength()
        {
            if (!IsValid )
                return 0;

            switch (Format)
            {
                case Step7Format.frmBit:
                    {
                        if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || ElementNumber > 0)
                            return ((Bit + Length + 7) / 8);
                        else
                            return ((Bit + Length * 8 + 7) / 8);
                    }
                case Step7Format.frmByte:
                    return Length;
                case Step7Format.frmWord:
                    return Length * 2;
                case Step7Format.frmDWord:
                    return Length * 4;
            }
            
            return 0;
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

        private void LimitBigStringSize(ref uint jobByteSize)
        {
            //if ((uint)TagsList[0].TagNode.DataType.Identifier == Opc.Ua.DataTypes.String && _LenStringEnable)
            //{
            if (this.Length > (uint)S7Protocol.SINGLEFRAME_STRING_MAX_SIZE)
            {
                this.Length = (int)(S7Protocol.SINGLEFRAME_STRING_MAX_SIZE);
                this.TotalJobSize = (int)(S7Protocol.SINGLEFRAME_STRING_MAX_SIZE);
                this.TagsList[0].Size = this.TotalJobSize;
                jobByteSize = this.TagsList[0].Size;
            }
            //}
        }

        // used to recalculate ByteOffset with Offset variable value
        private int _OriginalOffset;
        private int _OriginalDbNumber;
        private int _OriginalBit;

        private Int32? _AddressOffsetValue;
        public Int32? AddressOffsetValue
        {
            get { return _AddressOffsetValue; }
            set { _AddressOffsetValue = value; }
        }

        private Int32? _DBOffsetValue;
        public Int32? DBOffsetValue
        {
            get { return _DBOffsetValue; }
            set { _DBOffsetValue = value; }
        }

        private Dictionary<NodeId, CommJob> _ParseChildJobsRead;
        public Dictionary<NodeId, CommJob> ParseChildJobsRead
        {
            get { return _ParseChildJobsRead; }
            set { _ParseChildJobsRead = value; }
        }

        #endregion

        #region Override methods        
        public override void BasicCalculateStatistic()
        {
        }
        #endregion

        #region Custom Job

        public override bool IsCustomJob(Tag defTag)
        {
            return (IsStructAtomic(defTag) || IsBigArray(defTag) || IsBitArray(defTag));
        }

        /// <summary>
        /// Split big job into small jobs (read/write)
        /// </summary>
        /// <param name="defTag"></param>
        /// <returns></returns>
        private Dictionary<NodeId, CommJob> CreateChildJobBigArray(Tag defTag)
        {
            Dictionary<NodeId, CommJob> childJobs = new Dictionary<NodeId, CommJob>();

            S7TCPDynTagSettings startDyn = (S7TCPDynTagSettings)((S7TCPDynTagSettings)(defTag).DynSettings).Clone();
                        
            int startAddress = startDyn.Offset;

            uint tagSize = (defTag.Size / defTag.TagNode.ArrayDimension);
            uint dataFrame = ((S7TCPChannel)Station.GetChannel()).MAX_DATA_BYTES - (((S7TCPChannel)Station.GetChannel()).MAX_DATA_BYTES % tagSize);
            uint subArrayDimension = dataFrame / tagSize;
            uint totalArrayDimension = defTag.TagNode.ArrayDimension;

            while (totalArrayDimension > 0)
            {
                if (subArrayDimension > totalArrayDimension)
                    subArrayDimension = totalArrayDimension;

                startDyn.Offset = (int)startAddress;

                Tag ntag = Station.GetCommDriver().CreateTag(new TagDefinition
                {
                    NodeId = new NodeId(Guid.NewGuid()),
                    DynamicSettings = startDyn.ToString(),
                    DataType = defTag.TagNode.DataType,
                    SamplingInterval = defTag.SamplingInterval,
                    ArrayDimension = subArrayDimension,
                    InitialValue = defTag.TagNode.InitialValue,
                    MemberOrder = defTag.TagNode.MemberOrder,
                    Name = defTag.TagNode.Name,
                }, 0, 0);

                CommJob subArrayJob = Station.CreateJob(ntag);
                subArrayJob.ParentJob = this;
                childJobs[subArrayJob.TagsList[0].TagNode.NodeId] = subArrayJob;

                totalArrayDimension -= subArrayDimension;
                startAddress += (int)(subArrayDimension * tagSize);
            }

            return childJobs;
        }

        public override void CreateChildJobsRead(Tag defTag, List<Tag> membersList)
        {
            if (IsStructAtomic(defTag))
            {
                //SetAsACustomJob();

                //// create a single byte array "main job" to read all structure's members : create also a sub list of jobs (standard structure split jobs) to parse array of byte
                //ChildJobsRead = new Dictionary<NodeId, CommJob>();

                //// to manage Atomic Struct create a custom job (array of bytes) that fit struct size (in bytes)
                //S7TCPDynTagSettings startDyn = (S7TCPDynTagSettings)((S7TCPDynTagSettings)(membersList[0]).DynSettings).Clone();
                ////S7Protocol.GetAddressOffsetAndSize(membersList[0], out int startTagOffset, out uint startTagSize);
                //int startTagOffset = S7Protocol.GetTagStartAddress(membersList[0]);
                //S7Protocol.GetAddressOffsetAndSize(membersList[membersList.Count - 1], out int stopTagOffset, out uint stopTagSizeByte);

                //uint arrayDimension = (uint)((stopTagOffset + stopTagSizeByte) - startTagOffset);

                //startDyn.Format = Step7Format.frmByte;
                //startDyn.Offset = startTagOffset;

                //string dynamicSettings = startDyn.ToString();

                //NamedTag sourcevar = new NamedTag() { ArrayDimension = arrayDimension, Name = "tagToRead", DataType = new NodeId(Opc.Ua.DataTypes.Byte), NodeId = new NodeId(Guid.NewGuid()) };
                //var tagToRead = Station.CreateTag(new TagDefinition()
                //{
                //    DynamicSettings = dynamicSettings,
                //    DataType = sourcevar.DataType,
                //    NodeId = sourcevar.NodeId
                //});

                //DynTagSettings dtCalc = tagToRead.DynSettings;

                //NodeId dt = Opc.Ua.DataTypes.GetDataTypeId(((byte)Type));
                //var tdef = new TagDefinition() { ArrayDimension = (uint)arrayDimension, DataType = dt, NodeId = new NodeId(Guid.NewGuid()) };
                //tdef.DynamicSettings = tagToRead.TagNode.DynamicSettings;
                //dtCalc.TryParse(tagToRead.TagNode.DynamicSettings);
                //var ntag = Station.CreateTag(tdef);

                //// check and split byte array into sub jobs
                //ChildJobsRead = CreateChildJobBigArray(ntag);

                //_ParseChildJobsRead = new Dictionary<NodeId, CommJob>();
                //foreach (var tag in membersList)
                //{
                //    CommJob candJob = Station.CreateJob(tag);
                //    if (candJob.IsValid)
                //    {
                //        System.Diagnostics.Debug.WriteLine(String.Format("TagSize={0}, TotalJobSize={1}",tag.Size, candJob.TotalJobSize));
                //        _ParseChildJobsRead[candJob.TagsList[0].TagNode.NodeId] = candJob;
                //    }
                //}



                SetAsACustomJob();

                CommJob lastJobMemberList = null;
                _ParseChildJobsRead = new Dictionary<NodeId, CommJob>();
                foreach (var tag in membersList)
                {
                    CommJob candJob = Station.CreateJob(tag);
                    if (candJob.IsValid)
                    {
                        lastJobMemberList = candJob;  
                        System.Diagnostics.Debug.WriteLine(String.Format("TagSize={0}, TotalJobSize={1}", tag.Size, candJob.TotalJobSize));
                        _ParseChildJobsRead[candJob.TagsList[0].TagNode.NodeId] = candJob;
                    }
                }

                // to manage Atomic Struct create a custom job (array of bytes) that fit struct size (in bytes)
                S7TCPDynTagSettings startDyn = (S7TCPDynTagSettings)((S7TCPDynTagSettings)(membersList[0]).DynSettings).Clone();                
                int startTagOffset = S7Protocol.GetTagStartAddress(membersList[0]);
                S7Protocol.GetJobAddressOffsetAndSize(lastJobMemberList, out int stopTagOffset, out uint stopTagSizeByte);

                uint arrayDimension = (uint)((stopTagOffset + stopTagSizeByte) - startTagOffset);

                startDyn.Format = Step7Format.frmByte;
                startDyn.Offset = startTagOffset;

                string dynamicSettings = startDyn.ToString();

                NamedTag sourcevar = new NamedTag() { ArrayDimension = arrayDimension, Name = "tagToRead", DataType = new NodeId(Opc.Ua.DataTypes.Byte), NodeId = new NodeId(Guid.NewGuid()) };
                var tagToRead = Station.CreateTag(new TagDefinition()
                {
                    DynamicSettings = dynamicSettings,
                    DataType = sourcevar.DataType,
                    NodeId = sourcevar.NodeId
                });

                DynTagSettings dtCalc = tagToRead.DynSettings;

                NodeId dt = Opc.Ua.DataTypes.GetDataTypeId(((byte)Type));
                var tdef = new TagDefinition() { ArrayDimension = (uint)arrayDimension, DataType = dt, NodeId = new NodeId(Guid.NewGuid()) };
                tdef.DynamicSettings = tagToRead.TagNode.DynamicSettings;
                dtCalc.TryParse(tagToRead.TagNode.DynamicSettings);
                var ntag = Station.CreateTag(tdef);

                // create a single byte array "main job" to read all structure's members : create also a sub list of jobs (standard structure split jobs) to parse array of byte
                ChildJobsRead = CreateChildJobBigArray(ntag);
            }
            else if (IsBitArray(defTag))
            {
                SetAsACustomJob();

                // create a single byte array job to read all bit
                ChildJobsRead = new Dictionary<NodeId, CommJob>();

                int startTagOffset = 0;
                S7TCPDynTagSettings startDyn = (S7TCPDynTagSettings)((S7TCPDynTagSettings)(TagsList[0]).DynSettings).Clone();                
                uint totalJobSizeByte = (uint)(((startDyn.Bit % 8) + TagsList[0].Size) / 8);
                if (((startDyn.Bit % 8) + TagsList[0].Size) > 0)
                    totalJobSizeByte++;

                while (totalJobSizeByte > 0)
                {                    
                    startDyn = (S7TCPDynTagSettings)((S7TCPDynTagSettings)(TagsList[0]).DynSettings).Clone();
                    startTagOffset = S7Protocol.GetTagStartAddress(TagsList[0]);

                    uint arrayDimension = (((S7TCPChannel)Station.GetChannel()).MAX_DATA_BYTES > totalJobSizeByte ? totalJobSizeByte : ((S7TCPChannel)Station.GetChannel()).MAX_DATA_BYTES);
                                        
                    startDyn.Format = Step7Format.frmByte;
                    startDyn.Offset = startTagOffset;

                    string dynamicSettings = startDyn.ToString();

                    NamedTag sourcevar = new NamedTag() { ArrayDimension = arrayDimension, Name = "tagToRead", DataType = new NodeId(Opc.Ua.DataTypes.Byte), NodeId = new NodeId(Guid.NewGuid()) };
                    var tagToRead = Station.CreateTag(new TagDefinition()
                    {
                        DynamicSettings = dynamicSettings,
                        DataType = sourcevar.DataType,
                        NodeId = sourcevar.NodeId
                    });

                    DynTagSettings dtCalc = tagToRead.DynSettings;

                    NodeId dt = Opc.Ua.DataTypes.GetDataTypeId(((byte)Type));
                    var tdef = new TagDefinition() { ArrayDimension = (uint)arrayDimension, DataType = dt, NodeId = new NodeId(Guid.NewGuid()) };
                    tdef.DynamicSettings = tagToRead.TagNode.DynamicSettings;
                    dtCalc.TryParse(tagToRead.TagNode.DynamicSettings);
                    var ntag = Station.CreateTag(tdef);

                    CommJob subArrayJob = Station.CreateJob(ntag);
                    subArrayJob.ParentJob = this;
                    ChildJobsRead[subArrayJob.TagsList[0].TagNode.NodeId] = subArrayJob;

                    totalJobSizeByte -= arrayDimension;
                }
            } 
            else if (IsBigArray(defTag))
            {
                SetAsACustomJob();
                // split big array into sub array (of the same data type)
                ChildJobsRead = CreateChildJobBigArray(defTag);
            }            
        }

        public override void CreateChildJobsWrite(Tag defTag, List<Tag> membersList)
        {
            if (IsStructAtomic(defTag))
            {
                // only changed tags will be written : create one job for each tag (standard structure split jobs) 
                base.CreateChildJobsWrite(defTag, membersList);
            }
            else if (IsBitArray(defTag))
            {
                SetAsACustomJob();
                // only changed array element will be written : create list of jobs to write data on fly
                ChildJobsWrite = new Dictionary<NodeId, CommJob>();
            } 
            else if (IsBigArray(defTag))
            {
                SetAsACustomJob();
                // split big array into sub array (of the same data type)
                ChildJobsWrite = CreateChildJobBigArray(defTag);
            }            
        }

        // create a job to write a bit
        internal CommJob CreateChildJobWriteArrayBit(S7TCPTag.ChangedBit changedBit)
        {
            S7TCPDynTagSettings startDyn = (S7TCPDynTagSettings)((S7TCPDynTagSettings)(TagsList[0]).DynSettings).Clone();
                        
            int bitAddress = startDyn.Offset * 8 + startDyn.Bit + changedBit.BitNr;
            
            startDyn.Offset = bitAddress / 8;
            startDyn.Bit = bitAddress % 8;

            Tag ntag = Station.GetCommDriver().CreateTag(new TagDefinition
            {
                NodeId = new NodeId(Guid.NewGuid()),
                DynamicSettings = startDyn.ToString(),
                DataType = TagsList[0].TagNode.DataType,
                SamplingInterval = TagsList[0].SamplingInterval,
                ArrayDimension = 0,
                InitialValue = TagsList[0].TagNode.InitialValue,
                MemberOrder = TagsList[0].TagNode.MemberOrder,
                Name = TagsList[0].TagNode.Name,
            }, 0, 0);

            CommJob bitJob = Station.CreateJob(ntag);
            bitJob.ParentJob = this;
                        
            return bitJob;
        }

        /// <summary>
        /// Force custom job to be recreated 
        /// </summary>
        internal void ReCreateCustomJob()
        {
            if (IsStructAtomic())
                CreateCustomJob(StructTag);
            else
                CreateCustomJob(TagsList[0]);
        }

        public override List<CommJob> GetReadChildJobs()
        {
            List<CommJob> result = new List<CommJob>();

            if (IsStructAtomic())
            {
                if (ChildJobsRead != null)
                    result.AddRange(ChildJobsRead.Values);
            }
            else if (IsBitArray())
            {
                if (ChildJobsRead != null)
                    result.AddRange(ChildJobsRead.Values);                
            }
            else if (IsBigArray())
            {
                if (ChildJobsRead != null)
                    result.AddRange(ChildJobsRead.Values);
            }

            return result;
        }
        #endregion
    }
}
