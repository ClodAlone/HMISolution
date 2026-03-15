using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PhoenixContactPLCI
{
    public class PhoenixContactPLCICommJob : CommJob
    {
        #region Constructors
        public PhoenixContactPLCICommJob(Station station, PhoenixContactPLCICommJobSettings settings)
            : base(station, settings)
        {
            _Address = settings.Address;
            _DataFormat = settings.DataFormat;
            _StringLength = settings.StringLength;
            _ParseOk = settings.ParseOk;
            ResetInternalElements();
            CheckJobValid();
        }

        public PhoenixContactPLCICommJob(Station station, PhoenixContactPLCITag defTag)
            : base(station, defTag)
        {
            _Address = defTag.PhoenixContactPLCIDynSettings.Address;
            _DataFormat = defTag.PhoenixContactPLCIDynSettings.DataFormat;
            _StringLength = defTag.PhoenixContactPLCIDynSettings.StringLength;
            _ParseOk = defTag.PhoenixContactPLCIDynSettings.ParseOk;

            // correct size of array of bool
            if ((uint)defTag.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean && defTag.TagNode.ArrayDimension > 0)
                TotalJobSize = (UInt16)((TotalJobSize + 7) / 8);

            ResetInternalElements();
            CheckJobValid();
        }

        public PhoenixContactPLCICommJob(Station station)
            : base(station)
        {
            ElementNumber = 0;
            ResetInternalElements();
            CheckJobValid();

        }

        protected PhoenixContactPLCICommJob()
        {
            ElementNumber = 0;
            ResetInternalElements();
            CheckJobValid();
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

            if (TagsList[0].TagNode.DataType.IdType == IdType.Numeric && (uint)TagsList[0].TagNode.DataType.Identifier == (uint)BuiltInType.String)
            {
                if (!PhoenixContactPLCIProtocol.IsValidStringSize(_StringLength))
                {
                    IsValid = false;
                    InvalidReason = string.Format("{0} {1}",Properties.Resources.ErrorInvalidStringLength , TagsList[0].DynSettings.ToString());
                    return;
                }
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public override uint getProtocolDataType()
        {
            switch (PhoenixContactPLCIProtocol.GetDataType(_DataFormat))
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
            return PhoenixContactPLCIProtocol.MAX_DATA_BYTES;
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
            uint nData = 0;
            lock (lockListObject)
            {
                if (TagsListOnWriting.Count == 0)
                {
                    jobData = outData.ToArray();
                    return;
                }
                cand = TagsListOnWriting[0];
            }

            if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
            {
                nData = (UInt16)((cand.Size + 7) / 8);
            }
            else if (ElementNumber > 0 && !ProtocolDataSizeBig())
            {
                if (cand.TagNode.ArrayDimension == 0)
                    nData = (ushort)(GetProtocolDataByteSize());
                else
                    nData = (ushort)(GetProtocolDataByteSize() * cand.TagNode.ArrayDimension);
            }
            //With tags of type string, array, members of struct and strings, 
            //when writing the buffer coming from movicon is considered.
            else if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.String)
            {
                if (cand.TagNode.ArrayDimension == 0)
                {
                    string szTemp = cand.Value.Value.ToString();
                    //One is added to the length of the string in case the string is empty
                    nData = (UInt16)(szTemp.Length + 1);
                }
                else
                {
                    //in the case of an array of strings, the longest string is searched for, 
                    //multiplied by the number of elements in the array, 
                    //the value obtained is used to size the buffer, to be sent over the network.
                    nData = 0;
                    String[] array1d = (String[]) cand.Value.Value;
                    try
                    {
                        for (int i = 0; i < cand.TagNode.ArrayDimension; i++)
                        {
                            if (nData < (UInt16)array1d[i].Length)
                            {
                                //One is added to the length of the string in case the string is empty
                                nData = (UInt16)(array1d[i].Length + 1);
                            }
                        }
                    }
                    catch
                    {
                        jobData = outData.ToArray();
                        return;
                    }
                    
                    nData *= cand.TagNode.ArrayDimension;
                }                    
            }
            else
            {
                nData = (UInt16)cand.Size;
            }

            lock (lockListObject)
            {
                // moved to base class
                //if (StatusCode.IsGood(cand.Value.StatusCode) && (cand.LastValue != null))
                //{
                //    if ((Type == LinkType.ExceptionOutput || Type == LinkType.InputOutput))
                //    {
                //        if ((Station.RewritingOfTheSameValue == false) && (cand.LastValue.Equals(cand.Value.Value)))
                //        {
                //            if (TagsListOnWriting.Contains(cand))
                //            {
                //                TagsListOnWriting.Remove(cand);
                //            }
                //            return;
                //        }
                //    }
                //}

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
            if(jobdata != null)
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
       
        /// <summary>
        /// Reset all internal var used to manage low level comunication with PhoenixContactPLCI driver
        /// </summary>
        public void ResetInternalElements() {
            ReadWriteInvalidationErrorCode = (PhoenixContactPLCIProtocol.ErrorCodes)DriverErrorCodes.ErrorNoError;
            ResetSubscription();            
        }

        public void ResetSubscription()
        {
            SubscriptionId = -1;
            SubscriptionElementNumber = -1;
        }

        public bool IsSubscribed() {
            return SubscriptionId != -1;
        }

        public void ResetDiagnRxTxBytes() {
            DiagnRxBytes  = 0;            
            DiagnTxBytes = 0;
        }

        public bool IsReadWriteInvalid()
        {
            return (ReadWriteInvalidationErrorCode != (PhoenixContactPLCIProtocol.ErrorCodes)DriverErrorCodes.ErrorNoError);
        }

        public override bool IsJobAggregable()
        {
            return false;
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

        private PhoenixContactPLCI.PhoenixContactPLCIProtocol.VarType _DataFormat;
        public PhoenixContactPLCIProtocol.VarType DataFormat
        {
            get { return _DataFormat; }
            set { _DataFormat = value; }
        }

        private uint _StringLength;
        public uint StringLength
        {
            get { return _StringLength; }
            set { _StringLength = value; }
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

        // identify cause of job invalidation
        public PhoenixContactPLCIProtocol.ErrorCodes ReadWriteInvalidationErrorCode;

        public int SubscriptionId { set; get; }

        public int SubscriptionElementNumber { set; get; }

        // for statistic use --> calculate rx bytes
        public long DiagnRxBytes { set; get; } = 0;
        // for statistic use --> calculate tx bytes
        public long DiagnTxBytes { set; get; } = 0;
        #endregion
    }
}
