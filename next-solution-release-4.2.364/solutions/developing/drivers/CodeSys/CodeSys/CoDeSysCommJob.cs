using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace CoDeSys
{
    public class CoDeSysCommJob : CommJob
    {
        #region Constructors
        public CoDeSysCommJob(Station station, CoDeSysCommJobSettings settings)
            : base(station, settings)
        {
            _Address = settings.Address;
            _ShortAddress = settings.ShortAddress;
            _CoDeSysVarType = settings.CoDeSysVarType;
            _StringLength = settings.StringLength;

            _ParseOk = settings.ParseOk;

            //_TagName = settings.TagName;

            InitCoDeSysCommJob();
            CheckJobValid();

            // S7DataFormats.S7_DTL is a structure, that is not possible split.
            // Therefore, the element number parametre must always be equal to zero. 
            //if (_S7DataFormat == S7DataFormats.S7_DTL)
            //{
            //    ElementNumber = 0;
            //}
            //else 
            if (ProtocolDataSizeSmall())
            {
                ElementNumber = 1;
            }
            _CoDeSysArrayDimension = 0;
        }

        public CoDeSysCommJob(Station station, CoDeSysTag defTag)
            : base(station, defTag)
        {
            _Address = defTag.CoDeSysDynSettings.Address;
            _ShortAddress = defTag.CoDeSysDynSettings.ShortAddress;
            _CoDeSysVarType = defTag.CoDeSysDynSettings.CoDeSysVarType;
            _StringLength = defTag.CoDeSysDynSettings.StringLength;
            _ParseOk = defTag.CoDeSysDynSettings.ParseOk;

            //Symbolic Address
            //_TagName = defTag.CoDeSysDynSettings.TagName;

            if (ElementNumber > 0 || ProtocolDataSizeBig())
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }

            // S7DataFormats.S7_DTL is a structure, that is not possible split.
            // Therefore, the element number parametre must always be equal to zero. 
            //if (_S7DataFormat == S7DataFormats.S7_DTL)
            //{
            //    ElementNumber = 0;
            //}
            //else 
            //if (ProtocolDataSizeSmall())
            //{
            //    ElementNumber = 1;
            //}

            InitCoDeSysCommJob();
            CheckJobValid();
        }

        public CoDeSysCommJob(Station station)
            : base(station)
        {
            //if (AddressType == AddressTypes.TagName && ProtocolDataSizeSmall())
            //ElementNumber = 1;
            InitCoDeSysCommJob();
            CheckJobValid();

        }

        protected CoDeSysCommJob()
        {
            //if (AddressType == AddressTypes.TagName && ProtocolDataSizeSmall())
            ElementNumber = 0;
            InitCoDeSysCommJob();
            CheckJobValid();

        }

        public void InitCoDeSysCommJob()
        {
            ResetCoDeSysElement();
        }

        #endregion

        #region data Member
        #endregion

        #region Methods

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
                nType == (uint)BuiltInType.UInteger ||
                    nType == (uint)BuiltInType.String
                )
                    return true;
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
                        return (uint)StringLength;
                    default:
                        return 0;
                }
            }
            return 0;
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
            if (TagsList.Count() == 0)
            {
                IsValid = false;
                InvalidReason = "Job not contain tag";
                return;
            }

            uint size = 0;
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList = new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                tagnamelist += (tagnamelist.Length > 0 ? ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            foreach (var t in tempList)
            {
                if (!IsTypeAdmitted(t.DataType))
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tag type is invalid for its Data Type. (Tag: {0} Data Type: {1})", t.NodeId.ToString(), t.DataType.Identifier.ToString());
                    return;
                }
                size += GetTagSize(t);
            }

            if (!_ParseOk)
            {
                IsValid = false;
                InvalidReason = string.Format("{0}", Properties.Resources.ParseKo);
                return;
            }

            if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                return;
            }
            uint DataTypeBitSize = TagsList[0].TagNode.ArrayDimension == 0 ? 1 : TagsList[0].TagNode.ArrayDimension;
            if (ElementNumber == 0)
                DataTypeBitSize = DataTypeBitSize * GetDataTypeBitSize((uint)TagsList[0].TagNode.DataType.Identifier);

            IsValid = true;
            InvalidReason = string.Empty;
        }

        //public uint GetAggregateMaxJobSize()
        //{
        //    uint AggregLimit = Station.GetCommDriver().AggregationLimit;
        //    uint JobMaxSize = GetMaxJobSize();
        //    if ((AggregLimit != 0) && (AggregLimit < JobMaxSize))
        //    {
        //        return AggregLimit;
        //    }
        //    else
        //    {
        //        return JobMaxSize;
        //    }
        //}

        public override uint getProtocolDataType()
        {
            switch (CoDeSysProtocol.GetDataType(_CoDeSysVarType))
            {
                case UFUAModel.DataType.Boolean:
                    return (uint)BuiltInType.Boolean;
                case UFUAModel.DataType.Byte:
                    return (uint)BuiltInType.Byte;
                case UFUAModel.DataType.SByte:
                    return (uint)BuiltInType.SByte;
                case UFUAModel.DataType.Int16:
                    return (uint)BuiltInType.Int16;
                case UFUAModel.DataType.UInt16:
                    return (uint)BuiltInType.UInt16;
                case UFUAModel.DataType.Int32:
                    return (uint)BuiltInType.Int32;
                case UFUAModel.DataType.UInt32:
                    return (uint)BuiltInType.UInt32;
                case UFUAModel.DataType.Int64:
                    return (uint)BuiltInType.Int64;
                case UFUAModel.DataType.UInt64:
                    return (uint)BuiltInType.UInt64;
                case UFUAModel.DataType.Float:
                    return (uint)BuiltInType.Float;
                case UFUAModel.DataType.Double:
                    return (uint)BuiltInType.Double;
                default:
                    return 0;
            }
        }
        public override uint GetMaxJobSize()
        {
            return CoDeSysProtocol.MAX_DATA_BYTES;
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            return false;
        }

        public override void GetJobData(ref object jobData)
        {
            List<byte> outData = new List<byte>();
            //prepare a write request
            Tag cand = null;
            byte[] jobdata;
            UInt16 nData = 0;
            lock (lockListObject)
            {
                if (TagsListToWrite.Count == 0)
                {
                    TagsListOnWriting.Clear();
                    return;
                }
                cand = TagsListToWrite[0];
                TagsListToWrite.Remove(cand);
                if (!TagsListOnWriting.Contains(cand))
                    TagsListOnWriting.Add(cand);
            }

            if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            {
                nData = (UInt16)(cand.Size);
            }
            else
            if (ElementNumber > 0 && !ProtocolDataSizeBig())
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
                if (StatusCode.IsGood(cand.Value.StatusCode) && (cand.LastValue != null))
                {
                    if ((Type == LinkType.ExceptionOutput || Type == LinkType.InputOutput))
                    {
                        if ((Station.RewritingOfTheSameValue == false) && (cand.LastValue.Equals(cand.Value.Value)))
                        {
                            if (TagsListOnWriting.Contains(cand))
                            {
                                TagsListOnWriting.Remove(cand);
                            }
                            return;
                        }
                    }
                }

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
            jobData = outData.ToArray();
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            base.SetJobData(jobData, ref changed);
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;
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

        /// <summary>
        /// Reset all internal var used to manage low level comunication with CoDeSys driver
        /// </summary>
        public void ResetCoDeSysElement() {

            MappingErrorCode = DriverErrorCodes.ErrorNoError;
            //never this flag used to log undefined var on read event --> only 1st time
            //MappingErrorReported = false;
            CycVarListID = -1;
            CoDeSysVarType = CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN;
            CoDeSysArrayDimension = 0;
            // don't reset --> necessary for job validation
            //TotalJobSize = 0;
            Parallel.ForEach(TagsList, tag =>
            {
                tag.Size = 0;
            });
        }

        public void ResetDiagnRxTxBytes() {
            DiagnRxBytes  = 0;            
            DiagnTxBytes = 0;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Variable "full" address
        /// </summary>
        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { _Address = value; }
        }

        /// <summary>
        /// Variable address without string length
        /// </summary>
        private string _ShortAddress;
        public string ShortAddress
        {
            get { return _ShortAddress; }
            set { _ShortAddress = value; }
        }

        private CoDeSys.CoDeSysProtocol.VarType _CoDeSysVarType;
        public CoDeSysProtocol.VarType CoDeSysVarType
        {
            get { return _CoDeSysVarType; }
            set { _CoDeSysVarType = value; }
        }

        private uint _StringLength;
        public uint StringLength
        {
            get { return _StringLength; }
            set { _StringLength = value; }
        }

        private uint _CoDeSysArrayDimension;
        public uint CoDeSysArrayDimension
        {
            get { return _CoDeSysArrayDimension; }
            set { _CoDeSysArrayDimension = value; }
        }

        /// <summary>
        /// Job validation result
        /// </summary>
        private bool _ParseOk;
        public bool ParseOk
        {
            get { return _ParseOk; }
            set { _ParseOk = value; }
        }

        public bool MappingError
        {
            get { return (MappingErrorCode != DriverErrorCodes.ErrorNoError); }
        }
        // mark when tag is not mapped on device / array size of Movicon is bigger than device
        public DriverErrorCodes MappingErrorCode = DriverErrorCodes.ErrorNoError;
        //public bool MappingErrorReported;
        public int CycVarListID { set; get; }

        // for statistic use --> calculate rx bytes
        public long DiagnRxBytes { set; get; } = 0;
        // for statistic use --> calculate tx bytes
        public long DiagnTxBytes { set; get; } = 0;
        #endregion
    }
}
