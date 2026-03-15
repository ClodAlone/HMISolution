using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using Accon.AGLink;

namespace S7TIASymbolic
{
    public class S7TIACommJob : CommJob
    {
        #region Constructors
        public S7TIACommJob(Station station, S7TIACommJobSettings settings)
            : base(station, settings)
        {
            _StringLength = settings.StringLength;
            _S7DataFormat = settings.S7DataFormat;
            if (_S7DataFormat == S7DataFormats.String || _S7DataFormat == S7DataFormats.WString)
                SetStringTagDataFormat(_S7DataFormat);
            _StartAddress = settings.TiaStartAddress;
            //_Trans = settings.Trans;
            _Trans = Step7WordTrans.wtT;
            ClearStructureAGLinkWrite();
            ClearStructureAGLinkRead();
            CheckJobValid();
            // S7DataFormats.S7_DTL is a structure, that is not possible split. 
            // Therefore, the element number parametre must always be equal to zero.
            if (_S7DataFormat == S7DataFormats.S7_DTL)
            {
                ElementNumber = 0;
            }
            else if (ProtocolDataSizeSmall())
            {
                ElementNumber = 1;
            }
        }

        

        public S7TIACommJob(Station station, S7TIATag defTag)
            : base(station, defTag)
        {
            _StringLength = defTag.S7TIADynTagSettings.StringLength;
            _S7DataFormat = defTag.S7TIADynTagSettings.S7DataFormat;
            if (_S7DataFormat == S7DataFormats.String || _S7DataFormat == S7DataFormats.WString)
                SetStringTagDataFormat(_S7DataFormat);
            _StartAddress = defTag.S7TIADynTagSettings.StartAddress;
            //_Trans = defTag.S7TIADynTagSettings.Trans;
            _Trans = Step7WordTrans.wtT;

            if (ElementNumber > 0 || ProtocolDataSizeBig())
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }
            // S7DataFormats.S7_DTL is a structure, that is not possible split.
            // Therefore, the element number parametre must always be equal to zero. 
            if (_S7DataFormat == S7DataFormats.S7_DTL)
            {
                ElementNumber = 0;
            }
            else if (ProtocolDataSizeSmall())
            {
                ElementNumber = 1;
            }

            CheckJobValid();
        }

        public S7TIACommJob(Station station)
            : base(station)
        {
            CheckJobValid();
            
        }

        protected S7TIACommJob()
        {
            CheckJobValid();
           
        }
        #endregion
        #region data Member

        public enum AccessHandleState
        {
            Undefined,
            Found,
            NotFound
        }

        public AGL4.SymbolicRW m_stSymbolicRW;
        public AGL4.SymbolicRW m_stSymbolicRWrite;
        public AccessHandleState AccessHandle = AccessHandleState.Undefined;

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

        public static S7DataFormats GetS7TIAType(string Identifier)
        {
            switch (Identifier)
            {
                case "1":
                    return S7DataFormats.Bool;
                case "2":
                    return S7DataFormats.SInt;
                case "3":
                    return S7DataFormats.Byte;
                case "4":
                    return S7DataFormats.Int;
                case "5":
                    return S7DataFormats.UInt;
                case "6":
                    return S7DataFormats.DInt;
                case "7":
                    return S7DataFormats.UDInt;
                case "8":
                    return S7DataFormats.LInt;
                case "9":
                    return S7DataFormats.LWord;
                case "10":
                    return S7DataFormats.Real;
                case "11":
                    return S7DataFormats.LReal;
                case "12":
                    return S7DataFormats.String;
                case "13":
                    return S7DataFormats.Struct;
                case "14":
                    return S7DataFormats.S7_DTL;
                case "15":
                    return S7DataFormats.ULInt;
            }
            return S7DataFormats.Bool;
        }
        #endregion

        #region Methods

        private void CheckJobValid()
        {
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

            if (TotalJobSize > GetMaxJobSize())
            {
                IsValid = false;
                InvalidReason = string.Format("Job exceed the maximum size (Tags: {0})", tagnamelist);
                return;
            }
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
            // Removed to solve FOGBUGZ 13224
            //if (Station.GetCommDriver().AggregationLimit != 0)
            //    return Station.GetCommDriver().AggregationLimit;

            return S7TIAProtocol.MAX_DATA_BYTES;
        }

        public override uint getProtocolDataType()
        {
            switch (S7TIAProtocol.DataType(S7DataFormat))
            {
                case UFUAModel.DataType.UInt32:
                    return (uint)BuiltInType.UInt32;
                case UFUAModel.DataType.UInt16:
                    return (uint)BuiltInType.UInt16;
                case UFUAModel.DataType.Byte:
                    return (uint)BuiltInType.Byte;
                case UFUAModel.DataType.Boolean:
                    return (uint)BuiltInType.Boolean;
                case UFUAModel.DataType.Int32:
                    return (uint)BuiltInType.Int32;
                case UFUAModel.DataType.Int16:
                    return (uint)BuiltInType.Int16;
                case UFUAModel.DataType.SByte:
                    return (uint)BuiltInType.SByte;
                case UFUAModel.DataType.UInt64:
                    return (uint)BuiltInType.UInt64;
                case UFUAModel.DataType.Int64:
                    return (uint)BuiltInType.Int64;
                case UFUAModel.DataType.Float:
                    return (uint)BuiltInType.Float;
                case UFUAModel.DataType.Double:
                    return (uint)BuiltInType.Double;
                case UFUAModel.DataType.String:
                    return (uint)BuiltInType.String;                
                default:
                    return 0;
            }
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
                if (TagsListOnWriting.Count == 0)
                {
                    return;
                }
                cand = TagsListOnWriting[0];
            }

            if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            {
                nData = (UInt16)(cand.Size) ;
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
                cand.LastValue = cand.Value.Value;
                // for WString, instead of byte[] retur string/string[] object
                if (_S7DataFormat == S7DataFormats.WString)
                {
                    ((S7TIATag)cand).GetTagBufferWString(ref jobData);
                    return;
                }
                else
                {
                    jobdata = new byte[nData];
                    cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                }
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
            lock (lockListObject)
            {
                UInt16 sizeProtocolData = (UInt16)GetProtocolDataByteSize();
                if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                {
                    if (isProtocolBool())
                    {
                        if (((S7TIATag)(TagsList[0])).SetTagValue(ref rec, (int)TagsList[0].ByteOffset, UpdateTimeStamp))
                            changed.Add(TagsList[0]);
                    }
                    else
                    {
                        uint ArraySize = TagsList[0].TagNode.ArrayDimension;
                        if (ArraySize == 0)
                            ArraySize = 1;
                        byte[] tmpData = new byte[ArraySize];
                        TagsList[0].setMemRW(rec, (int)TagsList[0].ByteOffset, (int)(sizeProtocolData * ArraySize));
                        for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                        {
                            bool valBool = TagsList[0].getBoolValueFromMemRW((int)TagsList[0].ByteOffset + sizeProtocolData * ArrayIndex, ElementNumber);
                            tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
                        }

                        if (((S7TIATag)(TagsList[0])).SetTagValue(ref tmpData, 0, UpdateTimeStamp))
                            changed.Add(TagsList[0]);
                    }
                }
                else
                {
                    if (isProtocolBool() && ((uint)TagsList[0].TagNode.DataType.Identifier == Opc.Ua.DataTypes.Structure))
                    {
                        if (((S7TIATag)(TagsList[0])).SetTagValue(ref rec, (int)TagsList[0].ByteOffset, UpdateTimeStamp, (uint)(ElementNumber > 0 ? 1 : 0)))
                            changed.Add(TagsList[0]);
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
                            if(S7DataFormat == S7DataFormats.S7_DTL)
                            {
                                for( int nCicli = 0, offset = 0; nCicli < TagsList.Count; nCicli++)
                                {
                                    if(nCicli > 7)
                                    {
                                        break;
                                    }
                                    if (nCicli > 0)
                                    { 
                                        offset = nCicli + 1;
                                    }
                                    if (((S7TIATag)(TagsList[nCicli])).SetTagValue(ref rec, offset, UpdateTimeStamp, elemsize))
                                    {
                                        changed.Add(TagsList[nCicli]);
                                    }
                                }
                            }
                            else if (((S7TIATag)(TagsList[0])).SetTagValue(ref rec, (int)TagsList[0].ByteOffset, UpdateTimeStamp, elemsize))
                            {
                                changed.Add(TagsList[0]);
                            }
                            //else
                            //{//TODO
                            //    if ((TagsList[0].TagNode.ArrayDimension != 0) && (S7DataFormat == S7DataFormats.String))
                            //    {

                            //    }
                            //}
                        }
                        else
                        {
                            UInt16 sizeTmpData = (UInt16)GetDataTypeByteSize((uint)TagsList[0].TagNode.DataType.Identifier);
                            uint ArraySize = TagsList[0].TagNode.ArrayDimension;
                            if (ArraySize == 0)
                                ArraySize = 1;
                            byte[] tmpData = new byte[sizeTmpData * ArraySize];
                            int indexTmpData = 0;
                            TagsList[0].setMemRW(rec, (int)TagsList[0].ByteOffset, (int)(sizeProtocolData * ArraySize));
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                Array.Copy(rec, TagsList[0].ByteOffset + sizeProtocolData * ArrayIndex + ElementNumber * sizeTmpData, tmpData, indexTmpData, sizeTmpData);
                                indexTmpData += sizeTmpData;
                            }

                            if (((S7TIATag)(TagsList[0])).SetTagValue(ref tmpData, 0, UpdateTimeStamp))
                                changed.Add(TagsList[0]);
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

        public void ClearStructureAGLinkRead()
        {
            m_stSymbolicRW.Buffer = null;
            m_stSymbolicRW.BufferLen = 0;
            m_stSymbolicRW.Result = 0;
            m_stSymbolicRW.SError = 0;
            m_stSymbolicRW.AccessHandle = IntPtr.Zero;
        }

        public void ClearStructureAGLinkWrite()
        {
            m_stSymbolicRWrite.Buffer = null;
            m_stSymbolicRWrite.BufferLen = 0;
            m_stSymbolicRWrite.Result = 0;
            m_stSymbolicRWrite.SError = 0;
            m_stSymbolicRWrite.AccessHandle = IntPtr.Zero;
        }
        //public bool AccessHandleIsLoaded()
        //{
        //    return (m_stSymbolicRW.AccessHandle != IntPtr.Zero);
        //}

        public bool LoadAccessHandleAndSize(IntPtr pRootHandle, string StationName, ref string szError)
        {
            int result = AGL4.AGL40_SUCCESS;
            int error_pos = 0;
            szError = "";

            result = AGL4.Symbolic_CreateAccessByPath(pRootHandle, _StartAddress, ref m_stSymbolicRW.AccessHandle, ref error_pos);
            if(result != AGL4.AGL40_SUCCESS)
            {                
                szError = string.Format("{0} {1} {2}", StationName, _StartAddress, string.Format(Properties.Resources.ErrorFromDllAGLink, result));
                return (false);
            }
  
            result = AGL4.Symbolic_GetAccessBufferSize(m_stSymbolicRW.AccessHandle, ref m_stSymbolicRW.BufferLen);
            if (result != AGL4.AGL40_SUCCESS)
            {                
                szError = string.Format("{0} {1} {2}", StationName, _StartAddress, string.Format(Properties.Resources.ErrorFromDllAGLink, result));
                return (false);
            }
            m_stSymbolicRWrite = m_stSymbolicRW;
            m_stSymbolicRW.Buffer = new byte[m_stSymbolicRW.BufferLen];
            m_stSymbolicRWrite.Buffer = new byte[m_stSymbolicRWrite.BufferLen];

            return (true);
        }

        public bool LoadAccessHandleAndSize(IntPtr pRootHandle, string StationName, string addres, ref string szError)
        {
            int result = AGL4.AGL40_SUCCESS;
            int error_pos = 0;
            szError = "";

            result = AGL4.Symbolic_CreateAccessByPath(pRootHandle, addres, ref m_stSymbolicRW.AccessHandle, ref error_pos);
            if (result != AGL4.AGL40_SUCCESS)
            {                
                szError = string.Format("{0} {1} {2}", StationName, _StartAddress, string.Format(Properties.Resources.ErrorFromDllAGLink, result));
                return (false);
            }

            result = AGL4.Symbolic_GetAccessBufferSize(m_stSymbolicRW.AccessHandle, ref m_stSymbolicRW.BufferLen);
            if (result != AGL4.AGL40_SUCCESS)
            {
                szError = string.Format("{0} {1} {2}",StationName, _StartAddress, string.Format(Properties.Resources.ErrorFromDllAGLink, result));
                return (false);
            }
            m_stSymbolicRWrite = m_stSymbolicRW;
            m_stSymbolicRW.Buffer = new byte[m_stSymbolicRW.BufferLen];
            m_stSymbolicRWrite.Buffer = new byte[m_stSymbolicRWrite.BufferLen];

            return (true);
        }

        public void SetStringTagDataFormat(S7DataFormats s7StringDataFormat)
        {
            foreach (var tag in TagsList)
            {
                ((S7TIATag)tag).S7StringDataFormat = s7StringDataFormat;
                TotalJobSize = tag.Size;
            }
        }

        public void IfTagsListToWriteEmptyFillWithTagsList()
        {
            lock (lockListObject)
            {
                if (TagsListToWrite.Count == 0)
                    TagsListToWrite.AddRange(TagsList);
            }
        }

        public void IfTagsListOnWritingEmptyFillWithTagsList()
        {
            lock (lockListObject)
            {
                if (TagsListOnWriting.Count == 0)
                    TagsListOnWriting.AddRange(TagsList);
            }
        }

        public override bool IsJobAggregable()
        {
            return false;
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
        
        private string _StartAddress;
        public string StartAddress
        {
            get { return _StartAddress; }
            set
            {
                _StartAddress = value;
            }
        }

        private uint _StringLength;
        public uint StringLength
        {
            get { return _StringLength; }
            set { _StringLength = value; }
        }
        private S7DataFormats _S7DataFormat;
        public S7DataFormats S7DataFormat
        {
            get { return _S7DataFormat; }
            set { _S7DataFormat = value; }
        }

        private int _Length;
        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }

        private Step7WordTrans _Trans;
        public Step7WordTrans Trans
        {
            get { return _Trans; }
            set { _Trans = value; }
        }

        public uint GetReadRequestLength()
        {
            return 12;
        }
        public uint GetReadResponseLength()
        {
          return 0;
        }

        private bool _UpdateTimeStamp;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Select if the driver have to update timestamp at every read. </summary>
        ///
        /// <value> Always update TimeStamp. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool UpdateTimeStamp
        {
            get
            {
                return _UpdateTimeStamp;
            }
            set
            {
                _UpdateTimeStamp = value;
            }
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
