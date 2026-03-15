using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace RMS621
{    
    public class RMS621CommJob : CommJob
    {
        #region Constructors

        public RMS621CommJob(Station station, RMS621CommJobSettings settings)
            : base(station, settings)
        {
            _Command = settings.Command;
            _Process = settings.Process;
            _ProcessNumber = settings.ProcessNumber;

            CheckJobValid();
        }

        public RMS621CommJob(Station station, RMS621Tag defTag)
            : base(station, defTag)
        {
            _Command = defTag.RMS621DynSettings.Command;
            _Process = defTag.RMS621DynSettings.Process;
            _ProcessNumber = defTag.RMS621DynSettings.ProcessNumber;
                       
            if ((ElementNumber > 0 && !isProtocolBool()) || ProtocolDataSizeBig())
            {
                if (defTag.TagNode.ArrayDimension == 0)
                    TotalJobSize = GetProtocolDataByteSize();
                else
                    TotalJobSize = GetProtocolDataByteSize() * defTag.TagNode.ArrayDimension;
            }

            CheckJobValid();
        }

        public RMS621CommJob(Station station)
            : base(station)
        {
            _Command = RMS621Protocol.Command.Read;
            _Process = RMS621Protocol.Process.None;
            _ProcessNumber = 1;            
            CheckJobValid();
        }
 
        protected RMS621CommJob()
        {
            _Command = RMS621Protocol.Command.Read;
            _Process = RMS621Protocol.Process.None;
            _ProcessNumber = 1;
            CheckJobValid();
        }
       
        #endregion

        #region Data members
                

        #endregion

        #region Properties

        ///// <summary>
        ///// Address
        ///// </summary>
        //private string _Address;
        //public string Address
        //{
        //    get
        //    {
        //        return _Address;
        //    }

        //    set
        //    {
        //        _Address = value;
        //    }
        //}

        ///// <summary>
        ///// BitVariables = Variables of type bit?
        ///// </summary>
        //private bool _BitVariables;
        //public bool BitVariables
        //{
        //    get
        //    {
        //        return _BitVariables;
        //    }

        //    set
        //    {
        //        _BitVariables = value;
        //    }
        //}

        ///// <summary>
        ///// BitVariables = Variables of type bit?
        ///// </summary>
        //private bool _ArrayVariables;
        //public bool ArrayVariables
        //{
        //    get
        //    {
        //        return _ArrayVariables;
        //    }

        //    set
        //    {
        //        _ArrayVariables = value;
        //    }
        //}
        
        private RMS621Protocol.Command _Command;
        public RMS621Protocol.Command Command
        {
            get { return _Command; }
            set
            {
                _Command = value;
            }
        }

        private RMS621Protocol.Process _Process;
        public RMS621Protocol.Process Process
        {
            get { return _Process; }
            set
            {
                _Process = value;
            }
        }

        private uint _ProcessNumber;
        public uint ProcessNumber
        {
            get { return _ProcessNumber; }
            set
            {
                _ProcessNumber = value;
            }
        }

        #endregion

        #region Override Methods

        public uint GetAggregateMaxJobSize()
        {
            uint AggregLimit = Station.GetCommDriver().AggregationLimit;
            //if (_BitVariables == true)
            //{
            //    AggregLimit *= 8;
            //}
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
            return (uint)(RMS621Protocol.BUFFER_SIZE);
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
                    if (StatusCode.IsGood(cand.Value.StatusCode) && (cand.LastValue != null))
                    {
                        if ((Type == LinkType.ExceptionOutput || Type == LinkType.InputOutput))
                        {
                            if ((Station.RewritingOfTheSameValue == false) && (cand.LastValue.Equals(cand.Value.Value)))
                            {
                                if (listOnWriting.Contains(cand))
                                {
                                    listOnWriting.Remove(cand);
                                }
                                continue;
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
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            return false;
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
            //_Command = RMS621Protocol.Command.None;
            //_Process = RMS621Protocol.Process.None;
            //_ProcessNumber = 1;
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

            if (!IsValid)
            {
                IsValid = false;
                InvalidReason = string.Format(Properties.Resources.ErrorInvalidAssignedAddress,"");
                return;
            }

            //bool arrayVar = false;
            //uint varSize = 0;
            //uint totalSize = 0;
            //foreach (var t in tempList)
            //{
            //    varSize = GetTagSize(t);
            //    if (t.ArrayDimension > 0)
            //    {
            //        arrayVar = true;
            //    }
            //    if (isProtocolBool())
            //    {
            //        _BitVariables = true;
            //    }
            //    totalSize += varSize;
            //}

            //if (arrayVar)
            //{
            //    IsValid = false;
            //    InvalidReason = string.Format(
            //                               Properties.Resources.ErrorJobTooBig,
            //                               tagnamelist);
            //}           

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
                InvalidReason = string.Format("The {0} is invalid for the Function Code. (Tags: {1} )", errorDesc, tagnamelist);
                return;
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

       #endregion
    }
}
