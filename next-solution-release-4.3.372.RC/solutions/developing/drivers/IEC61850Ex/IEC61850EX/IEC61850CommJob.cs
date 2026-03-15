using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace IEC61850
{
    public class IEC61850CommJob : CommJob
    {
        private const int MaxDataLength = 250;

        #region Constructors
        public IEC61850CommJob(Station station, IEC61850CommJobSettings settings)
            : base(station, settings)
        {
            _LogicalDeviceName = settings.LogicalDeviceName;
            _LogicalNodeName = settings.LogicalNodeName;
            _FunctionalConstraint = settings.FunctionalConstraint;
            _DataItemIdentifier = settings.DataItemIdentifier;
            _MMSDataType = settings.MMSDataType;
            _DataMaximumLength = settings.DataMaximumLength;
            _RetryOutputInCaseOfError = settings.RetryOutputInCaseOfError;
            _ReportLogicalDeviceName = settings.ReportLogicalDeviceName;
            _ReportLogicalNodeName = settings.ReportLogicalNodeName;
            _ReportName = settings.ReportName;
            _ReportType = settings.ReportType;
            _InitializeData = settings.InitializeData;
            SetMMSDataItemID();
            SetMMSReportID();
            CheckJobValid();           
        }

        public IEC61850CommJob(Station station, IEC61850Tag defTag)
            : base(station, defTag)
        {
            _LogicalDeviceName = defTag.IEC61850DynSettings.LogicalDeviceName;
            _LogicalNodeName = defTag.IEC61850DynSettings.LogicalNodeName;
            _FunctionalConstraint = defTag.IEC61850DynSettings.FunctionalConstraint;
            _DataItemIdentifier = defTag.IEC61850DynSettings.DataItemIdentifier;
            _MMSDataType = defTag.IEC61850DynSettings.MMSDataType;
            _DataMaximumLength = defTag.IEC61850DynSettings.DataMaximumLength;
            _RetryOutputInCaseOfError = defTag.IEC61850DynSettings.RetryOutputInCaseOfError;
            _ReportLogicalDeviceName = defTag.IEC61850DynSettings.ReportLogicalDeviceName;
            _ReportLogicalNodeName = defTag.IEC61850DynSettings.ReportLogicalNodeName;
            _ReportName = defTag.IEC61850DynSettings.ReportName;
            _ReportType = defTag.IEC61850DynSettings.ReportType;
            _InitializeData = defTag.IEC61850DynSettings.InitializeData;
            SetMMSDataItemID();
            SetMMSReportID();

            if (TotalJobSize == 0)
                TotalJobSize = (uint)((defTag.IEC61850DynSettings.DataMaximumLength + 1) / 2) * 2;

            //if ((ElementNumber > 0 && !isProtocolBool()) || (ElementNumber >= 0 && ProtocolDataSizeBig()))
            //{
            //    if (defTag.TagNode.ArrayDimension == 0)
            //        TotalJobSize = GetProtocolDataByteSize();
            //    else
            //        TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            //}
            //if (ElementNumber < 0 && ProtocolDataSizeBig())
            //{
            //    uint ArraySize = TagsList[0].TagNode.ArrayDimension;
            //    if (ArraySize == 0)
            //        ArraySize = 1;
            //    if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
            //        TotalJobSize = ((ArraySize + 15) / 16 ) * 2;
            //    else
            //        TotalJobSize = ((ArraySize + 1) / 2) * 2;

            //}

            CheckJobValid();
        }

        public IEC61850CommJob(Station station)
            : base(station)
        {
            CheckJobValid();
        }

        protected IEC61850CommJob()
        {
            CheckJobValid();
        }
        #endregion

        #region data Member     
        public RequestTypes RequestType = RequestTypes.ReadValue;
        #endregion

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
                    //nType == (uint)BuiltInType.Int64 ||
                    nType == (uint)BuiltInType.Integer ||
                    nType == (uint)BuiltInType.SByte ||
                    nType == (uint)BuiltInType.UInt16 ||
                    nType == (uint)BuiltInType.UInt32 ||
                    //nType == (uint)BuiltInType.UInt64 ||
                    nType == (uint)BuiltInType.String ||
                    nType == (uint)BuiltInType.UInteger
                )
                {
                    return true;
                }
                return false;
            }
            
            return true;
        }

        #region override Methods

        public override bool ProtocolDataSizeIsValid(out string errorDesc)
        {
            errorDesc = null;
            if (ProtocolDataSizeBig())
            {
                if (ElementNumber > (GetProtocolDataBitSize() / GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier) - 1))
                    errorDesc = "ElementNumber";
            }
            if (errorDesc == null)
                return true;
            else
                return false;
        }


        public override uint SamplingInterval
        {
            get
            {
                if (this.bReportReaded)                    
                    return CommJob.JOB_NOT_SCHEDULABLE;
                else
                    return _SamplingInterval;
            }
        }

        public override bool IsJobAggregable()
        {
            return false;
        }

        #endregion

        #region Methods

        void SetMMSDataItemID()
        {
            // Set the string of the Functional Constraint
            string functionalConstraint = String.Empty;
            switch (_FunctionalConstraint)
            {
                case FunctionalConstraints.ST:
                    functionalConstraint = "ST";
                    break;

                case FunctionalConstraints.MX:
                    functionalConstraint = "MX";
                    break;

                case FunctionalConstraints.SG:
                    functionalConstraint = "SG";
                    break;

                case FunctionalConstraints.CO:
                    functionalConstraint = "CO";
                    break;

                case FunctionalConstraints.SP:
                    functionalConstraint = "SP";
                    break;

                case FunctionalConstraints.SV:
                    functionalConstraint = "SV";
                    break;

                case FunctionalConstraints.CF:
                    functionalConstraint = "CF";
                    break;

                case FunctionalConstraints.DC:
                    functionalConstraint = "DC";
                    break;

                case FunctionalConstraints.RP:
                    functionalConstraint = "RP";
                    break;

                case FunctionalConstraints.BR:
                    functionalConstraint = "BR";
                    break;
            }

            // Build the string of the Data Item ID in MMS format
            string dataItemID = String.Empty;
            // The Functional Constraint is optional
            if (!String.IsNullOrEmpty(functionalConstraint))
            {
                dataItemID = string.Format("{0}${1}${2}", _LogicalNodeName, functionalConstraint, _DataItemIdentifier);
            }
            else
            {
                dataItemID = string.Format("{0}${1}", _LogicalNodeName, _DataItemIdentifier);
            }
            _MMSDataItemId = dataItemID.Replace('.', '$');
            _MMSDataItemCompletePath = string.Format("{0}/{1}", _LogicalDeviceName, _MMSDataItemId);
        }

        void SetMMSReportID()
        {
            if (String.IsNullOrEmpty(_ReportName) || String.IsNullOrEmpty(_ReportLogicalNodeName) || String.IsNullOrEmpty(_ReportLogicalDeviceName))
            {
                _MMSReportID = string.Empty;
                _MMSReportCompletePath = string.Empty;
                return;
            }

            string szFunctionalConstraint = string.Empty;
            switch (_ReportType) {
                case ReportTypes.Unbuffered:
                    szFunctionalConstraint = "RP";
                    break;
                case ReportTypes.Buffered:
                    szFunctionalConstraint = "BR";
                    break;
            }

            _MMSReportID = string.Format("{0}${1}${2}", _ReportLogicalNodeName, szFunctionalConstraint, _ReportName);
            _MMSReportID = _MMSReportID.Replace('.', '$');

            _MMSReportCompletePath = string.Format("{0}/{1}", _ReportLogicalDeviceName, _MMSReportID);
        }

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
                        return (uint)((_DataMaximumLength + 1) / 2) * 2;
                    default:
                        return 0;
                }            
            }
            return 0;
        }

        public void SetIsValid(bool isValid)
        {
            IsValid = isValid;
        }

        private void CheckJobValid()
        {
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList = new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                if (!IsTypeAdmitted(t.TagNode.DataType) && t.DynSettings.MethodID == -1)
                {
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.IEC61850ErrorInvalidTagType, t.TagNode.NodeId.ToString(), t.TagNode.DataType.Identifier.ToString());
                    return;
                }
                tagnamelist += (tagnamelist.Length > 0 ? ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            if (String.IsNullOrWhiteSpace(_LogicalDeviceName))
            {
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.IEC61850ErrorInvalidTaskEmptyLDN, TagsList[0].TagNode.DynamicSettings);
                return;
            }

            if (String.IsNullOrWhiteSpace(_LogicalNodeName))
            {
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.IEC61850ErrorInvalidTaskEmptyLNN, TagsList[0].TagNode.DynamicSettings);
                return;
            }

            switch (_FunctionalConstraint)
            {
                case FunctionalConstraints.None:
                    break;
                case FunctionalConstraints.ST:
                case FunctionalConstraints.MX:
                case FunctionalConstraints.SG:
                    if (Type != LinkType.Input)
                    {
                        IsValid = false;
                        InvalidReason = string.Format(Properties.Resources.IEC61850ErrorReadOnlyFunctionalConstraint, TagsList[0].TagNode.DynamicSettings,DriverCodeBaseEx.Properties.Resources.LinkType_Input);
                        return;
                    }
                    break;
                case FunctionalConstraints.CO:
                case FunctionalConstraints.SP:
                case FunctionalConstraints.SV:
                case FunctionalConstraints.CF:
                case FunctionalConstraints.DC:
                case FunctionalConstraints.RP:
                case FunctionalConstraints.BR:
                    break;
                default:
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.IEC61850ErrorInvalidFunctionalConstraint, TagsList[0].TagNode.DynamicSettings);
                    return;
            }

            if (String.IsNullOrWhiteSpace(_DataItemIdentifier))
            {
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.IEC61850ErrorInvalidTaskEmptyDII, TagsList[0].TagNode.DynamicSettings);
                return;
            }

            if ((((IEC61850Tag)this.TagsList[0]).IEC61850DynSettings).IsMMSDataStringType() && this.DataMaximumLength == 0)
            {
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.IEC61850ErrorInvalidStringLength, this.DataMaximumLength, TagsList[0].TagNode.DynamicSettings);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
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

        public uint getProtocolDataType(MMSDataTypes mmsDataType)
        {
            switch (mmsDataType)
            {
                case MMSDataTypes.Boolean:
                    return ((uint)BuiltInType.Boolean);

                case MMSDataTypes.Integer8Bits:
                    return ((uint)BuiltInType.SByte);

                case MMSDataTypes.UnsignedInteger8Bits:
                    return ((uint)BuiltInType.Byte);

                case MMSDataTypes.Integer16Bits:
                    return ((uint)BuiltInType.Int16);

                case MMSDataTypes.UnsignedInteger16Bits:
                    return ((uint)BuiltInType.UInt16);

                case MMSDataTypes.Integer32Bits:
                    return ((uint)BuiltInType.Int32);

                case MMSDataTypes.UnsignedInteger32Bits:
                    return ((uint)BuiltInType.UInt32);

                case MMSDataTypes.FloatingPoint32Bits:
                    return ((uint)BuiltInType.Float);

                case MMSDataTypes.FloatingPoint64Bits:
                    return ((uint)BuiltInType.Double);

                case MMSDataTypes.BitString:
                    if (_DataMaximumLength <= 8)
                    {
                        return ((uint)BuiltInType.Byte);
                    }
                    else if (_DataMaximumLength <= 16)
                    {
                        return ((uint)BuiltInType.UInt16);
                    }
                    else if (_DataMaximumLength <= 32)
                    {
                        return ((uint)BuiltInType.UInt32);
                    }
                    else
                    {
                        return ((uint)BuiltInType.String);
                    }

                case MMSDataTypes.OctetString:
                case MMSDataTypes.VisibleString:
                case MMSDataTypes.MMSString:
                case MMSDataTypes.BinaryTime:
                case MMSDataTypes.UTCTime:
                    return ((uint)BuiltInType.String);
            }

            return ((uint)BuiltInType.Boolean);
        }

        public override uint getProtocolDataType()
        {
            switch (_MMSDataType)
            {
                case MMSDataTypes.Boolean:
                    return ((uint)BuiltInType.Boolean);

                case MMSDataTypes.Integer8Bits:
                    return ((uint)BuiltInType.SByte);

                case MMSDataTypes.UnsignedInteger8Bits:
                    return ((uint)BuiltInType.Byte);

                case MMSDataTypes.Integer16Bits:
                    return ((uint)BuiltInType.Int16);

                case MMSDataTypes.UnsignedInteger16Bits:
                    return ((uint)BuiltInType.UInt16);

                case MMSDataTypes.Integer32Bits:
                    return ((uint)BuiltInType.Int32);

                case MMSDataTypes.UnsignedInteger32Bits:
                    return ((uint)BuiltInType.UInt32);

                case MMSDataTypes.FloatingPoint32Bits:
                    return ((uint)BuiltInType.Float);

                case MMSDataTypes.FloatingPoint64Bits:
                    return ((uint)BuiltInType.Double);

                case MMSDataTypes.BitString:
                    if (_DataMaximumLength <= 8)
                    {
                        return ((uint)BuiltInType.Byte);
                    }
                    else if (_DataMaximumLength <= 16)
                    {
                        return ((uint)BuiltInType.UInt16);
                    }
                    else if (_DataMaximumLength <= 32)
                    {
                        return ((uint)BuiltInType.UInt32);
                    }
                    else
                    {
                        return ((uint)BuiltInType.String);
                    }

                case MMSDataTypes.OctetString:
                case MMSDataTypes.VisibleString:
                case MMSDataTypes.MMSString:
                case MMSDataTypes.BinaryTime:
                case MMSDataTypes.UTCTime:
                    return ((uint)BuiltInType.String);

                    //case MMSDataTypes.Structure:
                    //    return (UFUAModel.DataType.Boolean);
            }

            return ((uint)BuiltInType.Boolean);
        }

        public override uint GetMaxJobSize()
        {
            return(32767);
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }
       
        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch (AggType)
            {
                case JobAggregationType.JobAggregFits:
                    TagsList.Add(new IEC61850Tag(candJob.TagsList[0].TagNode, candJob.TagsList[0].ByteOffset, 0));
                    break;
                default:
                    return false;
            }
            return true;
        }

        public void GetJobDataMemberOfStruct(Tag tag, ref object jobData)
        {
            lock (lockListObject)
            {
                if (TagsListOnWriting.Contains(tag))
                    jobData = tag.WriteVal;
                else
                    jobData = tag.Value.Value;
            }
        }

        public override void GetJobData(ref object jobData)
        {
            var listToWrite = new List<Tag>();
            var listOnWriting = new List<Tag>();

            lock (lockListObject)
            {
                listToWrite.AddRange(TagsListOnWriting);
         
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

                UpdateTagsListOnWritingAndTagsListToWrite(listOnWriting);

                listToWrite.Clear();
                listOnWriting.Clear();

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
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;

            base.SetJobData(rec, ref changed);

            bool forceUpdateDataForReportTags = false;
            if (!string.IsNullOrWhiteSpace(MMSReportID))
            {
                forceUpdateDataForReportTags = true;
            }

            lock (lockListObject)
            {
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                for (int TagIndex = 0; TagIndex < TagsList.Count; TagIndex++)
                {
                    if (TagsList[TagIndex].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                    {
                        if (isProtocolBool())
                        {
                            if (((IEC61850Tag)(TagsList[TagIndex])).SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, 0, 0, forceUpdateDataForReportTags))
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
                            if (((IEC61850Tag)(TagsList[TagIndex])).SetTagValue(ref tmpData, 0, 0, 0, forceUpdateDataForReportTags))
                                changed.Add(TagsList[TagIndex]);
                        }
                    }
                    else
                    {
                        if (isProtocolBool())
                        {
                            if (((IEC61850Tag)(TagsList[TagIndex])).SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0), 0, forceUpdateDataForReportTags))
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
                                if (((IEC61850Tag)(TagsList[TagIndex])).SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset, elemsize, 0, forceUpdateDataForReportTags))
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
                                    Array.Copy(rec, TagsList[TagIndex].ByteOffset, tmpData, 0, TotalJobSize);
                                }

                                if (((IEC61850Tag)(TagsList[TagIndex])).SetTagValue(ref tmpData, 0, 0, 0, forceUpdateDataForReportTags))
                                    changed.Add(TagsList[TagIndex]);
                            }
                        }
                    }
                }

                FirstTime = false;
            }
        }

        public int CalculateWriteDataMMSLength()
        {
            int encodedDataLength = 0;

            lock (lockListObject)
            {
                // Check if the job must be executed in write mode
                if (GetTagListOnWritingCount() == 0)
                    return -1;

                Tag cand = TagsListOnWriting[0];

                switch (MMSDataType)
                {
                    case MMSDataTypes.Boolean:
                    case MMSDataTypes.Integer8Bits:
                    case MMSDataTypes.UnsignedInteger8Bits:
                        encodedDataLength = 1;
                        break;

                    case MMSDataTypes.Integer16Bits:
                        {
                            Int16 value = 0;
                            switch ((uint)cand.TagNode.DataType.Identifier)
                            {
                                case (uint)BuiltInType.SByte:
                                    {
                                        SByte auxValue = (SByte)cand.Value.Value;
                                        value = (Int16)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckLongSize((int)value);
                                    }
                                    break;
                                case (uint)BuiltInType.Int16:
                                    value = (Int16)cand.Value.Value;
                                    encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckLongSize((int)value);
                                    break;
                                case (uint)BuiltInType.Int32:
                                    {
                                        Int32 auxValue = (Int32)cand.Value.Value;
                                        value = (Int16)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckLongSize((int)value);
                                    }
                                    break;
                                case (uint)BuiltInType.Int64:
                                    {
                                        Int64 auxValue = (Int64)cand.Value.Value;
                                        value = (Int16)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckLongSize((int)value);
                                    }
                                    break;
                            }
                        }
                        break;

                    case MMSDataTypes.Integer32Bits:
                        {
                            Int32 value = 0;
                            switch ((uint)cand.TagNode.DataType.Identifier)
                            {
                                case (uint)BuiltInType.SByte:
                                    {
                                        SByte auxValue = (SByte)cand.Value.Value;
                                        value = (Int32)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckLongSize(value);
                                    }
                                    break;
                                case (uint)BuiltInType.Int16:
                                    {
                                        Int16 auxValue = (Int16)cand.Value.Value;
                                        value = (Int32)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckLongSize(value);
                                    }
                                    break;
                                case (uint)BuiltInType.Int32:
                                    value = (Int32)cand.Value.Value;
                                    encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckLongSize(value);
                                    break;
                                case (uint)BuiltInType.Int64:
                                    {
                                        Int64 auxValue = (Int64)cand.Value.Value;
                                        value = (Int32)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckLongSize(value);
                                    }
                                    break;
                            }
                        }
                        break;

                    case MMSDataTypes.UnsignedInteger16Bits:
                        {
                            UInt16 value = 0;
                            switch ((uint)cand.TagNode.DataType.Identifier)
                            {
                                case (uint)BuiltInType.Byte:
                                    {
                                        Byte auxValue = (Byte)cand.Value.Value;
                                        value = (UInt16)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckUnsignedSize(value);
                                    }
                                    break;
                                case (uint)BuiltInType.UInt16:
                                    value = (UInt16)cand.Value.Value;
                                    encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckUnsignedSize(value);
                                    break;
                                case (uint)BuiltInType.UInt32:
                                    {
                                        UInt32 auxValue = (UInt32)cand.Value.Value;
                                        value = (UInt16)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckUnsignedSize(value);
                                    }
                                    break;
                                case (uint)BuiltInType.UInt64:
                                    {
                                        UInt64 auxValue = (UInt64)cand.Value.Value;
                                        value = (UInt16)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckUnsignedSize(value);
                                    }
                                    break;
                            }
                        }
                        break;

                    case MMSDataTypes.UnsignedInteger32Bits:
                        {
                            UInt32 value = 0;
                            switch ((uint)cand.TagNode.DataType.Identifier)
                            {
                                case (uint)BuiltInType.Byte:
                                    {
                                        Byte auxValue = (Byte)cand.Value.Value;
                                        value = (UInt32)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckUnsignedSize(value);
                                    }
                                    break;
                                case (uint)BuiltInType.UInt16:
                                    {
                                        UInt16 auxValue = (UInt16)cand.Value.Value;
                                        value = (UInt32)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckUnsignedSize(value);
                                    }
                                    break;
                                case (uint)BuiltInType.UInt32:
                                    value = (UInt32)cand.Value.Value;
                                    encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckUnsignedSize(value);
                                    System.Diagnostics.Debug.WriteLine("IEC61850 DEBUG - {0} - CalculateWriteDataMMSLength - value: {1} - encodedDataLength: {2}", DateTime.Now, value, encodedDataLength);
                                    break;
                                case (uint)BuiltInType.UInt64:
                                    {
                                        UInt64 auxValue = (UInt64)cand.Value.Value;
                                        value = (UInt32)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckUnsignedSize(value);
                                    }
                                    break;
                            }
                        }
                        break;

                    case MMSDataTypes.FloatingPoint32Bits:
                        encodedDataLength = 5;
                        break;

                    case MMSDataTypes.FloatingPoint64Bits:
                        encodedDataLength = 9;
                        break;

                    case MMSDataTypes.BitString:
                        {
                            uint size = DataMaximumLength / 8;
                            if ((DataMaximumLength % 8) != 0)
                            {
                                size++;
                            }

                            uint value = 0;
                            switch ((uint)cand.TagNode.DataType.Identifier)
                            {
                                case (uint)BuiltInType.SByte:
                                    {
                                        SByte auxValue = (SByte)cand.Value.Value;
                                        value = (uint)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckLongSize((int)value);
                                        if (encodedDataLength < size)
                                        {
                                            encodedDataLength = (int)size;
                                        }
                                        encodedDataLength++; // Padding byte
                                    }
                                    break;

                                case (uint)BuiltInType.Byte:
                                    {
                                        Byte auxValue = (Byte)cand.Value.Value;
                                        value = (uint)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckUnsignedSize(value);
                                        if (encodedDataLength < size)
                                        {
                                            encodedDataLength = (int)size;
                                        }
                                        encodedDataLength++; // Padding byte
                                    }
                                    break;

                                case (uint)BuiltInType.Int16:
                                    {
                                        Int16 auxValue = (Int16)cand.Value.Value;
                                        value = (uint)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckLongSize((int)value);
                                        if (encodedDataLength < size)
                                        {
                                            encodedDataLength = (int)size;
                                        }
                                        encodedDataLength++; // Padding byte
                                    }
                                    break;

                                case (uint)BuiltInType.UInt16:
                                    {
                                        UInt16 auxValue = (UInt16)cand.Value.Value;
                                        value = (uint)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckUnsignedSize(value);
                                        if (encodedDataLength < size)
                                        {
                                            encodedDataLength = (int)size;
                                        }
                                        encodedDataLength++; // Padding byte
                                    }
                                    break;

                                case (uint)BuiltInType.Int32:
                                    {
                                        Int32 auxValue = (Int32)cand.Value.Value;
                                        value = (uint)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckLongSize((int)value);
                                        if (encodedDataLength < size)
                                        {
                                            encodedDataLength = (int)size;
                                        }
                                        encodedDataLength++; // Padding byte
                                    }
                                    break;

                                case (uint)BuiltInType.UInt32:
                                    {
                                        UInt32 auxValue = (UInt32)cand.Value.Value;
                                        value = (uint)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckUnsignedSize(value);
                                        if (encodedDataLength < size)
                                        {
                                            encodedDataLength = (int)size;
                                        }
                                        encodedDataLength++; // Padding byte
                                    }
                                    break;

                                case (uint)BuiltInType.Int64:
                                    {
                                        Int64 auxValue = (Int64)cand.Value.Value;
                                        value = (uint)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckLongSize((int)value);
                                        if (encodedDataLength < size)
                                        {
                                            encodedDataLength = (int)size;
                                        }
                                        encodedDataLength++; // Padding byte
                                    }
                                    break;

                                case (uint)BuiltInType.UInt64:
                                    {
                                        UInt64 auxValue = (UInt64)cand.Value.Value;
                                        value = (uint)auxValue;
                                        encodedDataLength = (int)IEC61850Protocol.AsnEncoderCheckUnsignedSize(value);
                                        if (encodedDataLength < size)
                                        {
                                            encodedDataLength = (int)size;
                                        }
                                        encodedDataLength++; // Padding byte
                                    }
                                    break;

                                case (uint)BuiltInType.String:
                                    {
                                        string auxValue = (string)cand.Value.Value;
                                        uint nonNullChar = (uint)auxValue.Length;
                                        encodedDataLength = (int)nonNullChar / 8;
                                        if ((nonNullChar % 8) != 0)
                                        {
                                            encodedDataLength++;
                                        }
                                        if (encodedDataLength < size)
                                        {
                                            encodedDataLength = (int)size;
                                        }
                                        encodedDataLength++; // Padding byte
                                    }
                                    break;
                            }
                        }
                        break;

                    case MMSDataTypes.OctetString:
                        // Calculate the string length
                        if((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.String)
                        {
                            string value = (string)cand.Value.Value;
                            uint nonNullChar = (uint)value.Length;

                            // The supervisor variable is a string composed by a series of
                            // two hexadecimal digits values, separated by blanks
                            encodedDataLength = (int)nonNullChar / 3;
                            if ((nonNullChar % 3) != 0)
                                encodedDataLength++;

                            if (encodedDataLength > DataMaximumLength)
                                encodedDataLength = (int)DataMaximumLength;
                        }
                        break;

                    case MMSDataTypes.VisibleString:
                    case MMSDataTypes.MMSString:
                        if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.String)
                        {
                            string value = (string)cand.Value.Value;
                            uint nonNullChar = (uint)value.Length;
                            encodedDataLength = (int)nonNullChar;
                            if (encodedDataLength > (int)DataMaximumLength)
                            {
                                encodedDataLength = (int)DataMaximumLength;
                            }
                        }
                        break;

                    case MMSDataTypes.UTCTime:
                        encodedDataLength = 8;
                        break;

                    case MMSDataTypes.BinaryTime:
                        encodedDataLength = 6;
                        break;
                }
            }

            return (encodedDataLength);
        }

        public uint GetProtocolDataBitSize(MMSDataTypes mmsType)
        {
            return GetDataTypeBitSize(getProtocolDataType(mmsType));
        }

        public uint GetProtocolDataByteSize(MMSDataTypes mmsType)
        {
            return (GetProtocolDataBitSize(mmsType) + 7) / 8;
        }

        public bool isProtocolBool(MMSDataTypes mmsType)
        {
            return (GetProtocolDataBitSize(mmsType) < 8);
        }

        public bool ProtocolDataSizeBig(int tagIndex, MMSDataTypes mmsType)
        {
            switch ((uint)TagsList[tagIndex].TagNode.DataType.Identifier)
            {
                case (uint)BuiltInType.Float:
                case (uint)BuiltInType.Double:
                case (uint)BuiltInType.String:
                    return false;
            }
            return GetProtocolDataBitSize(mmsType) > GetDataTypeBitSize((uint)TagsList[tagIndex].TagNode.DataType.Identifier);
        }

        public void SetJobTagData(object jobData, int tagIndex, MMSDataTypes mmsType, ref List<Tag> changed)
        {
            byte[] rec = jobData as byte[];
            if (rec == null)
            {
                return;
            }

            bool forceUpdateData = false;
            if(!string.IsNullOrWhiteSpace(MMSReportID) || (!StatusCode.IsGood(TagsList[tagIndex].Value.StatusCode)))
            {
                forceUpdateData = true;
            }

            lock (lockListObject)
            {
                // Special case: 1) the tag is an element of a structure (this method is used only for structure elements);
                //               2) the tag is of type string;
                //               3) the size of the tag is 0.
                // In this case set a default size for the tag, depending on the MMS type of the received data
                if((TagsList[tagIndex].TagNode.DataType == Opc.Ua.DataTypes.String) && (TagsList[tagIndex].Size == 0))
                {
                    ((IEC61850Tag)(TagsList[tagIndex])).SetDefaultStringSize(mmsType);
                }

                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize(mmsType);
                if (TagsList[tagIndex].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                {
                    if (isProtocolBool(mmsType))
                    {
                        if (((IEC61850Tag)(TagsList[tagIndex])).SetTagValue(ref rec, 0, 0, 0, forceUpdateData))
                        {
                            changed.Add(TagsList[tagIndex]);
                        }
                    }
                    else
                    {
                        uint ArraySize = TagsList[tagIndex].TagNode.ArrayDimension;
                        if (ArraySize == 0)
                            ArraySize = 1;
                        byte[] tmpData = new byte[ArraySize];

                        if (ElementNumber >= 0)
                        {
                            TagsList[tagIndex].setMemRW(rec, 0, (int)(sizeProtocolData * ArraySize));
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                tmpData[ArrayIndex] = (byte)(TagsList[tagIndex].getBoolValueFromMemRW(sizeProtocolData * ArrayIndex, ElementNumber) ? 1 : 0);
                            }
                        }
                        else
                        {
                            TagsList[tagIndex].setMemRW(rec, 0, (int)TagsList[tagIndex].Size);
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                tmpData[ArrayIndex] = (byte)(TagsList[tagIndex].getBoolValueFromMemRW(0, ArrayIndex) ? 1 : 0);
                            }
                        }
                        if (((IEC61850Tag)(TagsList[tagIndex])).SetTagValue(ref tmpData, 0, 0, 0, forceUpdateData))
                        {
                            changed.Add(TagsList[tagIndex]);
                        }
                    }
                }
                else
                {
                    if (isProtocolBool(mmsType))
                    {
                        if (((IEC61850Tag)(TagsList[tagIndex])).SetTagValue(ref rec, (int)TagsList[tagIndex].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0), 0, forceUpdateData))
                        {
                            changed.Add(TagsList[tagIndex]);
                        }
                    }
                    else
                    {
                        if (!ProtocolDataSizeBig(tagIndex, mmsType))
                        {
                            uint elemsize = 0;
                            if (ElementNumber > 0)
                            {
                                elemsize = GetProtocolDataByteSize(mmsType);
                            }
                            if (((IEC61850Tag)(TagsList[tagIndex])).SetTagValue(ref rec, 0, elemsize, 0, forceUpdateData))
                            {
                                changed.Add(TagsList[tagIndex]);
                            }
                        }
                        else
                        {
                            byte[] tmpData;
                            if (ElementNumber >= 0)
                            {
                                UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)TagsList[tagIndex].TagNode.DataType.Identifier);
                                uint ArraySize = TagsList[tagIndex].TagNode.ArrayDimension;
                                if (ArraySize == 0)
                                {
                                    ArraySize = 1;
                                }
                                tmpData = new byte[sizeTmpData * ArraySize];
                                int indexTmpData = 0;
                                TagsList[tagIndex].setMemRW(rec, 0, (int)(sizeProtocolData * ArraySize));
                                for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                                {
                                    Array.Copy(rec, sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                    indexTmpData += sizeTmpData;
                                }
                            }
                            else
                            {
                                tmpData = new byte[TagsList[tagIndex].Size];
                                TagsList[tagIndex].setMemRW(rec, 0, tmpData.Length);
                                Array.Copy(rec, 0, tmpData, 0, TagsList[tagIndex].Size);
                            }

                            if (((IEC61850Tag)(TagsList[tagIndex])).SetTagValue(ref tmpData, 0, 0, 0, forceUpdateData))
                            {
                                changed.Add(TagsList[tagIndex]);
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

        public void ResetReportFlags()
        {
            _bAddedToReport = false;
            _bReportReaded = false;
            _bIsFullReport = false;
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

        public uint GetTheNumberOfStructureFields()
        {
            if (!this.IsStruct())
                return 0;

            NumberOfStructureFields = (uint)GetTagCount();

            return (uint)GetTagCount(); 
        }
        #endregion

        public uint CalculateWriteStructureDataMMSLength()
        {
            // Check the MMS data format
            if (MMSDataType != MMSDataTypes.Structure)
                return 0;
                        
            // The tag must be of a structure
            if (!IsStruct())
                return 0;

            // Check the number of members of the structure that must be written
            if (NumberOfParsedStructureFieldsTypes == 0|| (NumberOfParsedStructureFieldsTypes > NumberOfStructureFields))
                return 0;

            // Check the numbers of parsed structures (it must be at least 1)
            if (TotalNumberOfStructures < 1)
                return 0;

            // Allocate (if needed the array for the data structures lengths)
            if (StructuresLengths == null)
                StructuresLengths = new uint[TotalNumberOfStructures];

            // Reset the lengths of the data structures
            for (int i = 0; i < (int)TotalNumberOfStructures; i++)
                StructuresLengths[i] = 0;

            // Parse the structure associated to the job for calculating the data length
            uint nParsedFieldsCount = 0;
            uint nStructureDataSize = CalculateSingleStructureMMSLength(ref nParsedFieldsCount, 1);
            return (nStructureDataSize);
        }

        uint CalculateSingleStructureMMSLength(ref uint nFieldIndex, byte nPreviousNestingLevel)
        {
            uint nDataLength = 0;
            Boolean bFieldFound = true;
            while (bFieldFound && (nFieldIndex < NumberOfParsedStructureFieldsTypes))
            {
                // Build the structure field name
                nFieldIndex++;

                if (nFieldIndex > GetTagCount())
                    continue;
                                
                // Check the nesting level of the field
                if (StructureFieldsTypes[2 * (nFieldIndex - 1)] > nPreviousNestingLevel)
                {
                    // Parse the fields of the nested structure
                    nFieldIndex--;
                    uint nStructLength = CalculateSingleStructureMMSLength(ref nFieldIndex, StructureFieldsTypes[2 * nFieldIndex]);
                    if (nStructLength == 0)
                    {
                        // Parsing terminated
                        nDataLength = 0;
                        break;
                    }

                    // Calculate the size of the encoded length
                    uint nEncodedLengthSize = IEC61850Protocol.BerEncoderCheckLengthSize(nStructLength);
                    if (nEncodedLengthSize == 0)
                    {
                        // Parsing terminated
                        nDataLength = 0;
                        break;
                    }

                    // Add the length of the nested structure to the total data length
                    nDataLength += 1 + nEncodedLengthSize + nStructLength;
                }
                else if (StructureFieldsTypes[2 * (nFieldIndex - 1)] < nPreviousNestingLevel)
                {
                    // Parsing terminated
                    nFieldIndex--;
                    break;
                }
                else
                {                    
                    // Calculate the length of the data of the current field
                    uint nFieldLength = CalculateSingleFieldMMSLength(nFieldIndex - 1, ""); //davide

                    // Calculate the size of the encoded length
                    uint nEncodedLengthSize = IEC61850Protocol.BerEncoderCheckLengthSize(nFieldLength);
                    if (nEncodedLengthSize == 0)
                        break;

                    // Add the length of the field to the total data length
                    nDataLength += 1 + nEncodedLengthSize + nFieldLength;
                }
            }

            if (nDataLength > 0)
            {
                // Store the structure size
                StructuresLengths[nPreviousNestingLevel - 1] = nDataLength;
            }

            return (nDataLength);
        }

        uint CalculateSingleFieldDataLength(Tag tag, byte nMMSType, uint nBitStringInfo)
        {
            uint nDataLength = 0;
            switch (nMMSType)
            {
                case (byte)MMSDataTypes.Boolean:
                    nDataLength = 1;
                    break;

                case (byte)MMSDataTypes.Integer8Bits:
                case (byte)MMSDataTypes.Integer16Bits:
                case (byte)MMSDataTypes.Integer32Bits:
                    {
                        int nIntValue = 0;
                        switch ((uint)tag.TagNode.DataType.Identifier)
                        {
                            case (uint)BuiltInType.Byte://VT_UI1:
                                nIntValue = (int)(byte)tag.Value.Value;
                                break;

                            case (uint)BuiltInType.SByte: // VT_I1:
                                nIntValue = (int)(sbyte)tag.Value.Value;
                                break;

                            case (uint)BuiltInType.UInt16: // VT_UI2:
                                nIntValue = (int)(UInt16)tag.Value.Value;
                                break;

                            case (uint)BuiltInType.Int16: // VT_I2:
                                nIntValue = (int)(Int16)tag.Value.Value;
                                break;

                            case (uint)BuiltInType.UInt32: // VT_UI4:
                                nIntValue = (int)(UInt32)tag.Value.Value;
                                break;

                            default:
                                nIntValue = (int)tag.Value.Value;
                                break;
                        }

                        //BYTE pValueBuffer[4];
                        nDataLength = IEC61850Protocol.AsnEncoderCheckLongSize(nIntValue);
                    }
                    break;

                case (byte)MMSDataTypes.UnsignedInteger8Bits:
                case (byte)MMSDataTypes.UnsignedInteger16Bits:
                case (byte)MMSDataTypes.UnsignedInteger32Bits:
                    {
                        uint nUintValue = 0;
                        switch ((uint)tag.TagNode.DataType.Identifier)
                        {
                            case (uint)BuiltInType.Byte: // VT_UI1:
                                nUintValue = (uint)(byte)tag.Value.Value;
                                break;

                            case (uint)BuiltInType.SByte: // VT_I1:
                                nUintValue = (uint)(byte)tag.Value.Value;
                                break;

                            case (uint)BuiltInType.UInt16:  //VT_UI2:
                                nUintValue = (uint)(byte)tag.Value.Value;
                                break;

                            case (uint)BuiltInType.Int16: // VT_I2:
                                nUintValue = (uint)(byte)tag.Value.Value;
                                break;

                            case (uint)BuiltInType.Int32: // VT_I4:
                                nUintValue = (uint)(byte)tag.Value.Value;
                                break;

                            default:
                                nUintValue = (uint)(byte)tag.Value.Value;
                                break;
                        }

                        nDataLength = IEC61850Protocol.AsnEncoderCheckUnsignedSize(nUintValue);                        
                    }
                    break;

                case (byte)MMSDataTypes.FloatingPoint32Bits:
                    nDataLength = 5;
                    break;

                case (byte)MMSDataTypes.FloatingPoint64Bits:
                    nDataLength = 9;
                    break;

                case (byte)MMSDataTypes.BitString:
                    {
                        uint nBitLength = nBitStringInfo & 0xffffff;
                        uint nSize = nBitLength / 8;
                        if (nBitLength % 8 != 0)
                        {
                            nSize++;
                        }

                        // Case integer value
                        if (nSize <= 4)
                        {
                            nDataLength = nSize;
                        }

                        // Case string
                        else
                        {
                            string szAux = tag.Value.Value.ToString();
                            int nNonNullChar = szAux.Length;
                            nDataLength = (uint)nNonNullChar / 8;
                            if (nNonNullChar % 8 != 0)
                            {
                                nDataLength++;
                            }
                            if (nDataLength > nSize)
                            {
                                nDataLength = nSize;
                            }
                        }

                        nDataLength++; // Padding byte
                    }
                    break;

                case (byte)MMSDataTypes.OctetString:
                    {
                        // Calculate the string length
                        string szAux = tag.Value.Value.ToString();
                        int nNonNullChar = szAux.Length;

                        // The supervisor variable is a string is composed by a series of
                        // two hexadecimal digits values, separated by blanks
                        nDataLength = (uint)(nNonNullChar / 3);
                        if (nNonNullChar % 3 != 0)
                        {
                            nDataLength++;
                        }
                    }
                    break;

                case (byte)MMSDataTypes.VisibleString:
                case (byte)MMSDataTypes.MMSString:
                    {
                        // Calculate the string length
                        string szAux = tag.Value.Value.ToString();
                        nDataLength = (uint)szAux.Length;
                        if (nDataLength > MaxDataLength)
                            nDataLength = MaxDataLength;
                    }
                    break;

                case (byte)MMSDataTypes.UTCTime:
                    nDataLength = 8;
                    break;

                case (byte)MMSDataTypes.BinaryTime:
                    nDataLength = 6;
                    break;
            }

            return (nDataLength);
        }

        uint CalculateSingleFieldMMSLength(uint nFieldIndex, string szFieldName)
        {
            // Parsing terminated
            if (nFieldIndex >= this.GetTagCount())
                return 0;

            uint nDataLength = CalculateSingleFieldDataLength(TagsList[(int)nFieldIndex], StructureFieldsTypes[2 * nFieldIndex + 1], StructureBitStringInfos[nFieldIndex]);

            return nDataLength;
        }


        #region Properties

        // Name of the logical device
        private string _LogicalDeviceName;
        public string LogicalDeviceName
        {
            get { return _LogicalDeviceName; }
            set { _LogicalDeviceName = value; }
        }

        //Name of the logical node
        private string _LogicalNodeName;
        public string LogicalNodeName
        {
            get { return _LogicalNodeName; }
            set { _LogicalNodeName = value; }
        }

        // Functional constraint of the data item
        private FunctionalConstraints _FunctionalConstraint;
        public FunctionalConstraints FunctionalConstraint
        {
            get { return _FunctionalConstraint; }
            set
            {
                _FunctionalConstraint = value;
            }
        }

        // Identifier of the data item
        private string _DataItemIdentifier;
        public string DataItemIdentifier
        {
            get { return _DataItemIdentifier; }
            set { _DataItemIdentifier = value; }
        }

        // Identifier of the data item in MMS format
        private string _MMSDataItemId;
        public string MMSDataItemId
        {
            get { return _MMSDataItemId; }
            set { _MMSDataItemId = value; }
        }

        // Search path for the the data item of the job
        private string _MMSDataItemCompletePath;
        public string MMSDataItemCompletePath
        {
            get { return _MMSDataItemCompletePath; }
            set { _MMSDataItemCompletePath = value; }
        }

        // MMS data type
        private MMSDataTypes _MMSDataType;
        public MMSDataTypes MMSDataType
        {
            get { return _MMSDataType; }
            set
            {
                _MMSDataType = value;
            }
        }

        // The data maximum length
        private uint _DataMaximumLength;
        public uint DataMaximumLength
        {
            get { return _DataMaximumLength; }
            set { _DataMaximumLength = value; }
        }

        // Retry output in case of error
        private bool _RetryOutputInCaseOfError;
        public bool RetryOutputInCaseOfError
        {
            get { return _RetryOutputInCaseOfError; }
            set { _RetryOutputInCaseOfError = value; }
        }

        // Name of the logical device of the report
        private string _ReportLogicalDeviceName;
        public string ReportLogicalDeviceName
        {
            get { return _ReportLogicalDeviceName; }
            set { _ReportLogicalDeviceName = value; }
        }

        // Name of the logical node of the report
        private string _ReportLogicalNodeName;
        public string ReportLogicalNodeName
        {
            get { return _ReportLogicalNodeName; }
            set { _ReportLogicalNodeName = value; }
        }

        // Name of the logical node of the report
        private string _ReportName;
        public string ReportName
        {
            get { return _ReportName; }
            set { _ReportName = value; }
        }

        // Report type
        private ReportTypes _ReportType;
        public ReportTypes ReportType
        {
            get { return _ReportType; }
            set
            {
                _ReportType = value;
            }
        }

        // Name of the report ID
        private string _MMSReportID;
        public string MMSReportID
        {
            get { return _MMSReportID; }
            set { _MMSReportID = value; }
        }

        // Complete name Name of the report
        private string _MMSReportCompletePath;
        public string MMSReportCompletePath
        {
            get { return _MMSReportCompletePath; }
            set { _MMSReportCompletePath = value; }
        }

        // Initialize Data
        private bool _InitializeData;

        public bool InitializeData
        {
            get { return _InitializeData; }
            set { _InitializeData = value; }
        }

        private bool _bAddedToReport;
        public bool bAddedToReport
        {
            get { return _bAddedToReport; }
            set { _bAddedToReport = value; }
        }

        private bool _bReportReaded;
        public bool bReportReaded
        {
            get { return _bReportReaded; }
            set { _bReportReaded = value; }
        }

        private bool _bIsFullReport;
        public bool bIsFullReport
        {
            get { return _bIsFullReport; }
            set { _bIsFullReport = value; }
        }

        private uint _nDataNestingLevel;
        public uint nDataNestingLevel
        {
            get { return _nDataNestingLevel; }
            set { _nDataNestingLevel = value; }
        }

        public byte[] _StructureFieldsTypes = null;
        public byte[] StructureFieldsTypes
        {
            get { return _StructureFieldsTypes; }
            set { _StructureFieldsTypes = value; }
        }

        public uint[] _StructureBitStringInfos = null;
        public uint[] StructureBitStringInfos
        {
            get { return _StructureBitStringInfos; }
            set { _StructureBitStringInfos = value; }
        }

        public uint _NumberOfParsedStructureFieldsTypes = 0;
        public uint NumberOfParsedStructureFieldsTypes
        {
            get { return _NumberOfParsedStructureFieldsTypes; }
            set { _NumberOfParsedStructureFieldsTypes = value; }
        }

        public uint _TotalNumberOfStructures = 0;
        public uint TotalNumberOfStructures
        {
            get { return _TotalNumberOfStructures; }
            set { _TotalNumberOfStructures = value; }
        }

        public uint _NumberOfStructureFields = 0;
        public uint NumberOfStructureFields
        {
            get { return _NumberOfStructureFields; }
            set { _NumberOfStructureFields = value; }
        }

        public uint[] _StructuresLengths = null;
        public uint[] StructuresLengths
        {
            get { return _StructuresLengths; }
            set { _StructuresLengths = value; }
        }

        #endregion
    }
}
