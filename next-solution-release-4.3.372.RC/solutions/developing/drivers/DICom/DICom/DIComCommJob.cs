using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace DICom
{
    public class DIComCommJob : CommJob
    {
        #region Constructors
        public DIComCommJob(Station station, DIComCommJobSettings settings)
            : base(station, settings)
        {
            _VarName = settings.VarName;
            CheckJobValid();
        }

        public DIComCommJob(Station station, DIComTag defTag)
            : base(station, defTag)
        {
            _VarName = defTag.DIComDynSettings.VarName;
            CheckJobValid();
        }

        public DIComCommJob(Station station)
            : base(station)
        {
            CheckJobValid();
        }

        protected DIComCommJob()
        {
            CheckJobValid();
        }
        #endregion
        #region data Member



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
                    nType == (uint)BuiltInType.UInteger ||
                    nType == (uint)BuiltInType.String
                )
                    return true;
            }

            return false;
        }
        #endregion

        #region Methods
       
        private void CheckJobValid()
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
            }

            if (IsTagDataTypeString())
            {
                TagsList[0].Size = DIComProtocol.STRING_MAX_SIZE;
                TotalJobSize = DIComProtocol.STRING_MAX_SIZE;
            }
            else
                TotalJobSize = GetProtocolDataByteSize();

            // by default, driver publish only changed data
            _ForceValueUpdate = false;

            IsValid = true;
            InvalidReason = string.Empty;
        }

        private bool IsTagDataTypeString()
        {
            return ((uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.String);
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


        //public override uint getProtocolDataType()
        //{
        //    switch ((uint)TagsList[0].TagNode.DataType.Identifier)
        //    {
        //        case (uint)BuiltInType.Boolean:
        //        case (uint)BuiltInType.SByte:
        //        case (uint)BuiltInType.Byte:
        //            return 1;
        //        case (uint)BuiltInType.Int16:
        //        case (uint)BuiltInType.UInt16:
        //            return 2;
        //        case (uint)BuiltInType.Float:
        //        case (uint)BuiltInType.UInt32:
        //        case (uint)BuiltInType.Int32:
        //            return 4;
        //        case (uint)BuiltInType.UInt64:
        //        case (uint)BuiltInType.Int64:
        //        case (uint)BuiltInType.Double:
        //            return 8;
        //        case (uint)BuiltInType.String:
        //            return TotalJobSize * 8;
        //        default:
        //            return 0;
        //    }            
        //}

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
            {
                byte[] tmpBuffer = outData.ToArray();
                SwapByteBuffer(ref tmpBuffer);
                jobData = tmpBuffer;
            }

        }

        public override uint GetMaxJobSize()
        {
            return 65535;  //ModbusSlaveProtocol.GetMaxJobSize(DataArea);
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
                        if (((DIComTag)(TagsList[0])).SetTagValue(ref rec, (int)TagsList[0].ByteOffset, _ForceValueUpdate))
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

                        if (((DIComTag)(TagsList[0])).SetTagValue(ref tmpData, 0, _ForceValueUpdate))
                            changed.Add(TagsList[0]);
                    }
                }
                else
                {
                    if (isProtocolBool() && ((uint)TagsList[0].TagNode.DataType.Identifier == Opc.Ua.DataTypes.Structure))
                    {
                        if (((DIComTag)(TagsList[0])).SetTagValue(ref rec, (int)TagsList[0].ByteOffset, (uint)(ElementNumber > 0 ? 1 : 0), 0, _ForceValueUpdate))
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
                            else if (((DIComTag)(TagsList[0])).SetTagValue(ref rec, (int)TagsList[0].ByteOffset, elemsize, 0, _ForceValueUpdate))
                            {
                                changed.Add(TagsList[0]);
                            }                            
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

                            if (((DIComTag)(TagsList[0])).SetTagValue(ref tmpData, 0, _ForceValueUpdate))
                                changed.Add(TagsList[0]);
                        }
                    }
                }
                FirstTime = false;
            }
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

        public bool IsJobDataTypeMatchingVarType(DIComProtocol.DiComVar var)
        {
            bool match = false;

            if (TagsList[0].TagNode.DataType.IdType == IdType.Numeric)
            {
                uint nType = (uint)TagsList[0].TagNode.DataType.Identifier;
                switch (nType)
                {
                    case (uint)BuiltInType.Boolean:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_BOOL);
                        break;
                    case (uint)BuiltInType.Byte:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_BYTE || var.VarType == DIComProtocol.DiCommVarType.TYPE_USINT);
                        break;
                    case (uint)BuiltInType.SByte:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_BYTE || var.VarType == DIComProtocol.DiCommVarType.TYPE_USINT);
                        break;
                    case (uint)BuiltInType.Int16:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_INT);
                        break;
                    case (uint)BuiltInType.UInt16:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_UINT || var.VarType == DIComProtocol.DiCommVarType.TYPE_WORD);
                        break;
                    case (uint)BuiltInType.Int32:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_DINT);
                        break;                    
                    case (uint)BuiltInType.UInt32:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_UDINT || var.VarType == DIComProtocol.DiCommVarType.TYPE_DWORD);
                        break;
                    case (uint)BuiltInType.Integer:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_INT);
                        break;
                    case (uint)BuiltInType.UInteger:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_UINT || var.VarType == DIComProtocol.DiCommVarType.TYPE_WORD);
                        break;
                    case (uint)BuiltInType.Int64:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_LINT);
                        break;
                    case (uint)BuiltInType.UInt64:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_ULINT || var.VarType == DIComProtocol.DiCommVarType.TYPE_LWORD);
                        break;
                    case (uint)BuiltInType.Float:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_REAL);
                        break;
                    case (uint)BuiltInType.Double:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_LREAL);
                        break;
                    case (uint)BuiltInType.String:
                        match = (var.VarType == DIComProtocol.DiCommVarType.TYPE_STRING || var.VarType == DIComProtocol.DiCommVarType.TYPE_WSTRING);
                        break;
                }
            }

            return match;
        }        

        #endregion

        #region Properties        
        private string _VarName;
        public string VarName
        {
            get { return _VarName; }
            set { _VarName = value; }
        }

        private int _VarIndex;
        public int VarIndex
        {
            get { return _VarIndex; }
            set { _VarIndex = value; }
        }

        private bool _ForceValueUpdate;
        public bool ForceValueUpdate
        {
            get { return _ForceValueUpdate; }
            set { _ForceValueUpdate = value; }
        }
        #endregion
    }
}
