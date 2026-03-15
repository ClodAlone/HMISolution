////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	BACnetCommJob.cs
//
// summary:	Implements the driver BACnet communications job class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;

namespace BACnet
{
    /// <summary>   Protocol's task of the BACnet driver. </summary>
    public class BACnetCommJob : CommJob
    {
        public enum BACnetState : int
        {
            None = 0,
            BBMDInitialization,
            WhoIs,
            WhoHas,
            Cov,
            CovSubscribedPolling,
            CovSubscribed,
            Polling,
            UnknownObject, // variable don't exist into device
        }

        /// <summary>   Bits of the conditional variable of a job. </summary>
        public enum BACnetConditionalVariableBits : ushort
        {
            ForceReadWrite = 0,
            RelinquishCommand,
        }

        public enum CommandTypes : int
        {
            Invalid = -1,
            Init,       // bacnet object initialization
            ReadCmd,    // read polling
            WriteCmd,   // write

        }

        #region Constructors
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the BACnetCommJob. </summary>
        ///
        /// <param name="station">  assigned to an object of type Station. </param>
        /// <param name="settings"> set with an object of type BACnetCommJobSettings. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetCommJob(Station station, BACnetCommJobSettings settings)
            : base(station, settings)
        {
            _BACnetObjectType = settings.BACnetObjectType;
            _ObjectName = settings.ObjectName;
            _PropertyIdentifier = settings.PropertyIdentifier;
            _COVEnable = settings.COVEnable;
            _DataSize = settings.DataSize;
            _ArrayIndex = settings.ArrayIndex;
            _DataLogMode = settings.DataLogMode;
            _PriorityLevel = settings.PriorityLevel;
            _InstanceNumber = settings.InstanceNumber;

            ResetCOVTimes();
            if (ProtocolDataSizeSmall())
                ElementNumber = 1;

            Init();
            CheckJobValid();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the BACnetCommJob. </summary>
        ///
        /// <param name="station">  assigned to an object of type Station. </param>
        /// <param name="defTag">   set with an object of type BACnetTag. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetCommJob(Station station, BACnetTag defTag)
            : base(station, defTag)
        {
            _BACnetObjectType = defTag.BACnetDynSettings.BACnetObjectType;
            _ObjectName = defTag.BACnetDynSettings.ObjectName;
            _PropertyIdentifier = defTag.BACnetDynSettings.PropertyIdentifier;
            _COVEnable = defTag.BACnetDynSettings.COVEnable;
            _DataSize = defTag.BACnetDynSettings.DataSize;
            _ArrayIndex = defTag.BACnetDynSettings.ArrayIndex;
            _DataLogMode = defTag.BACnetDynSettings.DataLogMode;
            _PriorityLevel = defTag.BACnetDynSettings.PriorityLevel;
            _InstanceNumber = defTag.BACnetDynSettings.InstanceNumber;

            ResetCOVTimes();
            if (ProtocolDataSizeSmall())
                ElementNumber = 1;

            Init();
            CheckJobValid();
        }

        private void Init()
        {
            _BacnetState = BACnetState.None;
            ObjectID = null;
            CovSubscriberID = 0;
            if (!BACnetEnums.ObjectPropertyWritableDictionary[_BACnetObjectType.ToString()][_PropertyIdentifier])
                Type = LinkType.Input;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the BACnetCommJob. </summary>
        ///
        /// <param name="station">  assigned to an object of type Station. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetCommJob(Station station)
            : base(station)
        {
            if (ProtocolDataSizeSmall())
                ElementNumber = 1;

            CheckJobValid();
        }

        /// <summary>   Initializes the BACnetCommJob. </summary>
        protected BACnetCommJob()
        {
            if (ProtocolDataSizeSmall())
                ElementNumber = 1;

            CheckJobValid();
        }
        #endregion

        #region data Member

        public BACnetObjectIdentifier ObjectID;
        public byte InvokeID;
        public UInt32 CovSubscriberID;

        public DateTime COVSubscriptionTime;    // date-time cov subscription
        public DateTime COVExpireTime;  // date-time cov expire
        #endregion

        #region Static methods
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Test if NodeId type is admitted. </summary>
        ///
        /// <param name="type"> . </param>
        ///
        /// <returns>   true if type admitted, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
        #endregion

        #region Methods
        public void CheckBacNetState()
        {
            if (BacnetState == BACnetState.None)
            {
                if (((BACnetStation)Station).HasBBMDDevice())
                    BacnetState = BACnetState.BBMDInitialization;
                else
                    BacnetState = BACnetState.WhoIs;
            }

            if (BacnetState == BACnetState.BBMDInitialization)
            {
                if (((BACnetStation)Station).BBDMDevice.IsRegistred)
                    BacnetState = BACnetState.WhoIs;
                else
                    return;  // send BBMD registrazion command
            }

            if (BacnetState == BACnetState.WhoIs)
            {
                if (((BACnetStation)Station).WhoIsDisabled)
                {
                    if (!((BACnetStation)Station).BACnetInitDone)
                        ((BACnetChannel)Station.GetChannel()).InitStationWithNoWhoIsData((BACnetStation)Station);
                    BacnetState = BACnetState.WhoHas;
                }
                else
                {
                    if (((BACnetStation)Station).BACnetInitDone && ((BACnetStation)Station).HasDeviceInstance())
                        BacnetState = BACnetState.WhoHas;
                    else
                        return; // send WhoIs request
                }
            }

            if (BacnetState == BACnetState.WhoHas)
            {
                if (HasInstanceNumber())
                {
                    SetObjectID();
                    if (COVEnable)
                        BacnetState = BACnetState.Cov;
                    else
                        BacnetState = BACnetState.Polling;
                }
                else
                {
                    return; // send WhoHas request
                }
            }

            if (BacnetState == BACnetState.Cov)
            {
                if (IsCovSubscribed())
                {
                    // not data and forced to request to device ?
                    if (((BACnetStation)Station).ForceInitialPolling && !HasCovValue())
                        BacnetState = BACnetState.CovSubscribedPolling;
                    else
                        BacnetState = BACnetState.CovSubscribed;
                }
                else
                {
                    return; // send cov subscription
                }
            }

            if (BacnetState == BACnetState.CovSubscribedPolling)
            {
                if (HasCovValue())
                {
                    BacnetState = BACnetState.CovSubscribed;
                }
                else
                {
                    // request data 
                    return;
                }
            }

            if (BacnetState == BACnetState.CovSubscribed)
            {
                // BBDM device required to refresh registration ?
                if (((BACnetStation)Station).HasBBMDDevice() && ((BACnetStation)Station).BBDMDevice.IsRegistrationExpired())
                {
                    // force driver to refresh bbmd registration
                    BacnetState = BACnetState.BBMDInitialization;
                    return;
                }

                if (IsCovSubscriptionExpired())
                {
                    // force driver to refresh cov registration
                    BacnetState = BACnetState.Cov;
                    return; // send Cov subscription, refresh cov subscription, ... 
                }
                else
                {
                    // Is Time to check device presence with WhoIs ?
                    return;
                }
            }

            if (BacnetState == BACnetState.Polling)
            {
                // BBDM device required to refresh registration ?
                if (((BACnetStation)Station).HasBBMDDevice() && ((BACnetStation)Station).BBDMDevice.IsRegistrationExpired())
                {
                    // force driver to refresh bbmd registration
                    BacnetState = BACnetState.BBMDInitialization;
                    return;
                }
                // do nothing here
                return;
            }

            if (BacnetState == BACnetState.UnknownObject)
            {
                // do nothing 
                return;
            }
        }

        public void SetBacNetStateInError()
        {
            if (((BACnetStation)Station).HasBBMDDevice())
                if (((BACnetStation)Station).BBDMDevice.IsRegistred)
                    BacnetState = BACnetState.None;
                else
                    BacnetState = BACnetState.BBMDInitialization;
            else
                BacnetState = BACnetState.None;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   return tag object memory size. </summary>
        ///
        /// <param name="t">    . </param>
        ///
        /// <returns>   The tag size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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
                    default:
                        return 0;
                }
            }
            return 0;
        }

        /// <summary>   Test if BACnetCommJob object is valid. </summary>
        private void CheckJobValid()
        {
            uint size = 0;
            string tagnamelist = string.Empty;

            List<DriverBaseInterfaces.TagDefinition> tempList = new List<DriverBaseInterfaces.TagDefinition>();
            foreach (var t in TagsList)
            {
                tagnamelist += (tagnamelist.Length > 0 ? ", " : string.Empty) + t.TagNode.NodeId.ToString();
                tempList.AddRange(GetSimpleTagList(t.TagNode));
            }

            bool bit = false;
            bool num = false;
            foreach (var t in tempList)
            {
                if (!IsTypeAdmitted(t.DataType))
                {
                    IsValid = false;
                    InvalidReason = string.Format("The Tag type is invalid for its Data Type. (Tag: {0} Data Type: {1})", t.NodeId.ToString(), t.DataType.Identifier.ToString());
                    return;
                }

                //TODO
                //if (BACnetProtocol.InvalidAreaTypeLinkType(_AreaType, Type))
                //{
                //    IsValid = false;
                //    InvalidReason = string.Format(Properties.Resources.AreaTypeRequireInput);
                //    return;
                //}

                if (_ObjectName == "")
                {
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.InvalidNullObjectName);
                    return;
                }

                if (((uint)t.DataType.Identifier == (uint)BuiltInType.String) && (DataSize <= 0))
                {
                    IsValid = false;
                    InvalidReason = string.Format("Error: {0} Name: {1}", Properties.Resources.ErrorTheStringCanNotHaveDataSizeZero,
                                                   ObjectName);
                    return;
                }

                if (_InstanceNumber > BACnetProtocol.MAX_INSTANCE_NUMBER)
                {
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.ErrorFormatValueInstansNumber);
                    return;
                }

                if (COVEnable && !BACnetEnums.isCovSupported(this.BACnetObjectType.ToString(), this.PropertyIdentifier.ToString()))
                {
                    IsValid = false;
                    InvalidReason = string.Format(Properties.Resources.ErrorCovNotSupportedForThisObject, this.BACnetObjectType.ToString(), this.PropertyIdentifier.ToString());
                    return;
                }

                if ((uint)t.DataType.Identifier == (uint)BuiltInType.Boolean)
                    bit = true;
                else
                    num = true;
                size += GetTagSize(t);
            }

            //tag must be all bit or not
            if ((bit && num))
            {
                IsValid = false;
                InvalidReason = string.Format("The Tag type are inconsistent for the Object Type. (Tags: {0} ObjectType: {1})", tagnamelist, BACnetObjectType);
                return;
            }

            if (!BACnetEnums.isCovSupported(BACnetObjectType.ToString(), PropertyIdentifier.ToString()) && COVEnable)
            {
                IsValid = false;
                InvalidReason = string.Format("The Tag don't support Cov. (Tags: {0} )", tagnamelist);
                return;
            }


            // TODO
            //string errorDesc;
            //if (!ProtocolDataSizeIsValid(out errorDesc))
            //{
            //    IsValid = false;
            //    InvalidReason = string.Format("The {0} is invalid for the ObjectType. (Tags: {1} F.Code: {2})", errorDesc, tagnamelist, ObjectType);
            //    return;
            //}

            IsValid = true;
            InvalidReason = string.Empty;
        }

        public override uint getProtocolDataType()
        {
            switch (PropertyIdentifier)
            {
                case BACnetEnums.PropertyIdentifier.STATE_TEXT:
                case BACnetEnums.PropertyIdentifier.PRIORITY_ARRAY:
                    if (ArrayIndex == 0)
                        return (uint)BuiltInType.UInt32;
                    else
                        return BACnetEnums.ApplicationTagBuiltInType[BACnetEnums.ObjectPropertyTypeDictionary[BACnetObjectType.ToString()][PropertyIdentifier]];
                default:
                    return BACnetEnums.ApplicationTagBuiltInType[BACnetEnums.ObjectPropertyTypeDictionary[BACnetObjectType.ToString()][PropertyIdentifier]];
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return maximum memory size of BACnetCommJob objects. </summary>
        ///
        /// <returns>   The maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override uint GetMaxJobSize()
        {
            return 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Test if it can be aggregated candJob. </summary>
        ///
        /// <param name="candJob">      . </param>
        /// <param name="ExtraBytes">   [out] additional byte size for the aggregation. </param>
        ///
        /// <returns>   A JobAggregationType. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Aggregate candJob as specified by AggType and manage ExtraBytes. </summary>
        ///
        /// <param name="candJob">      . </param>
        /// <param name="AggType">      . </param>
        /// <param name="ExtraBytes">   . </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            return false;
        }

        public override void GetJobData(ref object jobData)
        {
            var listToWrite = new List<Tag>();
            var listOnWriting = new List<Tag>();

            lock (lockListObject)
            {
                listToWrite.AddRange(TagsListOnWriting);
            }

            List<byte> outData = new List<byte>();
            //prepare a write request --> requested to write mamber's of struct (many tags) with one command only
            listToWrite.Sort(CompareTagByOffset);
            Tag cand = null;
            byte[] jobdata;
            UInt16 nData = 0;
            //bool equalValue = true;
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
                    //if (equalValue && (!StatusCode.IsGood(cand.Value.StatusCode) || cand.LastValue == null || !cand.LastValue.Equals(cand.Value.Value)))
                    //{
                    //    equalValue = false;
                    //}
                    cand.LastValue = cand.Value.Value;
                    jobdata = new byte[nData];
                    cand.GetTagBuffer(ref jobdata, false, 0, (ElementNumber > 0 && !ProtocolDataSizeBig() ? GetProtocolDataByteSize() : 0));
                }
                uint ArraySize = cand.TagNode.ArrayDimension;
                if (ArraySize == 0)
                    ArraySize = 1;
                if ((uint)cand.TagNode.DataType.Identifier == (uint)BuiltInType.String)
                {
                    ushort answerSize = 0;
                    while (jobdata[answerSize] != 0 && answerSize < jobdata.Length)
                        answerSize++;
                    byte[] tmpData = new byte[answerSize];
                    Array.Copy(jobdata, tmpData, answerSize);
                    jobdata = tmpData;
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

            //if ( equalValue && (Type == LinkType.ExceptionOutput || Type == LinkType.InputOutput))
            //{
            //    listOnWriting.Clear();
            //}
            //else
            //{
            lock (lockListObject)
            {
                TagsListOnWriting.Clear();
                listOnWriting.ForEach((tag) =>
                {
                    TagsListOnWriting.Add(tag);
                    tag.LastValue = tag.Value.Value;
                });

                listToWrite.ForEach((tag) =>
                {
                    if (!TagsListToWrite.Contains(tag))
                        TagsListToWrite.Add(tag);
                });

                listOnWriting.ForEach((tag) =>
                {
                    TagsListToWrite.Remove(tag);
                });

                listOnWriting.Clear();
                listToWrite.Clear();
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
            //}
        }


        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   write the BACnetCommJob's tags with JobData. </summary>
        ///
        /// <param name="jobData">  data buffer to write. </param>
        /// <param name="changed">  [in,out] out list of changed tags. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            byte[] rec = jobData as byte[];
            if (rec == null)
                return;
            if (rec.Length < GetDataTypeByteSize((uint)TagsList[0].TagNode.DataType.Identifier))
            {
                byte[] tmp = new byte[GetDataTypeByteSize((uint)TagsList[0].TagNode.DataType.Identifier)];
                rec.CopyTo(tmp, 0);
                rec = tmp;
            }

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
                            TagsList[TagIndex].setMemRW(rec, (int)TagsList[TagIndex].ByteOffset, rec.Length);
                            for (int ArrayIndex = 0; ArrayIndex < ArraySize; ArrayIndex++)
                            {
                                bool valBool = TagsList[TagIndex].getBoolValueFromMemRW((ushort)(rec.Length / ArraySize) * ArrayIndex, ElementNumber);
                                tmpData[ArrayIndex] = (byte)(valBool ? 1 : 0);
                            }

                            if (TagsList[TagIndex].SetTagValue(ref tmpData, 0))
                                changed.Add(TagsList[TagIndex]);
                        }
                    }
                    else
                    {
                        if (TagsList[TagIndex].SetTagValue(ref rec, (int)TagsList[TagIndex].ByteOffset,
                           (uint)(ElementNumber > 0 ? GetProtocolDataByteSize() : 0)))
                            changed.Add(TagsList[TagIndex]);
                    }
                }

                FirstTime = false;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compare tags by offset. Return 0 if x = y , 1 if x &gt; y and -1 if x &lt; y.
        /// </summary>
        ///
        /// <param name="x">    . </param>
        /// <param name="y">    . </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
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

        public bool IsRelinquishRequest()
        {
            bool returnValue = false;

            if (conditionalVariableHasBeenSet == true)
            {
                bool bitValue = false;
                if (jobConditionalVariable.GetStateCommandVariableBit(ref bitValue, (ushort)BACnetConditionalVariableBits.RelinquishCommand))
                    returnValue = bitValue;
            }
            return (returnValue);
        }

        public bool IsRelinquishForced()
        {
            bool returnValue = false;

            if (conditionalVariableHasBeenSet == true)
            {
                bool bitValue = false;
                if (jobConditionalVariable.GetStateCommandVariableBit(ref bitValue, (ushort)BACnetConditionalVariableBits.RelinquishCommand))
                {
                    if (bitValue && (!BACnetEnums.isPrioritySupported(_BACnetObjectType.ToString(), _PropertyIdentifier.ToString()) || Type == LinkType.Input))
                        ResetRelinquishForced();
                    else
                        returnValue = bitValue;
                }
            }
            return (returnValue);
        }
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Resets the value of the conditional variable. Driver specific: do nothing </summary>
        ///
        /// <returns>   void. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ResetConditionalVariable()
        {
            if (conditionalVariableHasBeenSet)
            {
                bool bitValue = false;
                if (jobConditionalVariable.GetStateCommandVariableBit(ref bitValue, (ushort)BACnetConditionalVariableBits.RelinquishCommand))
                {
                    if (bitValue)
                        ResetRelinquishForced();
                    else
                        ResetForceReadWrite();
                }
                else
                {
                    ResetForceReadWrite();
                }
            }
        }
        #endregion

        #region Offset variable Management
        //public override void ManageUpdatedValueForTheConditionalVariable(DataValue value)
        //{
        //    if (jobConditionalVariable.hasBeenSet)
        //    {
        //        if (GetOffsetVariableIntNumericValue(out Int64 v))
        //        {
        //            uint bitMask = (uint)Math.Pow(2, (ushort)BACnetConditionalVariableBits.RelinquishCommand);
        //            if ((v & bitMask) != 0)
        //            {

        //            } 
        //            else
        //            {
        //                base.ManageUpdatedValueForTheConditionalVariable(value);
        //            }
        //        }

        //        //base.ManageUpdatedValueForTheConditionalVariable(value);

        //        //if (jobConditionalVariable.varValue == value)
        //        //    return;

        //        //jobConditionalVariable.varValue = value;
        //        //if (Station == null)
        //        //    return;
        //        //var jobChannel = Station.GetChannel();
        //        //if (jobChannel != null)
        //        //    jobChannel.ForceExecution();
        //    }
        //}

        public void ResetRelinquishForced()
        {
            if (conditionalVariableHasBeenSet == true)
            {
                bool bitValue = false;
                if (jobConditionalVariable.GetStateCommandVariableBit(ref bitValue, (ushort)BACnetConditionalVariableBits.RelinquishCommand))
                {
                    if (bitValue)
                        jobConditionalVariable.SetStateCommandVariableBit(false, (ushort)BACnetConditionalVariableBits.RelinquishCommand, this.Station.GetCommDriver());
                }
            }
        }

        public void ResetForceReadWrite()
        {
            if (conditionalVariableHasBeenSet == true)
            {
                bool bitValue = false;
                if (jobConditionalVariable.GetStateCommandVariableBit(ref bitValue, (ushort)BACnetConditionalVariableBits.ForceReadWrite))
                {
                    if (bitValue)
                        jobConditionalVariable.SetStateCommandVariableBit(false, (ushort)BACnetConditionalVariableBits.ForceReadWrite, this.Station.GetCommDriver());
                }
            }
        }

        public bool HasInstanceNumber()
        {
            return (_InstanceNumber != -1 || _RunTimeInstanceNumber != -1);
        }

        public bool HasRunTimeInstanceNumber()
        {
            return (_RunTimeInstanceNumber != -1);
        }

        public bool HasSubscriberID()
        {
            return (CovSubscriberID != 0);
        }

        public bool IsCovSubscribed()
        {
            return (HasSubscriberID() && COVExpireTime > DateTime.UtcNow);
        }

        public bool IsCovSubscriptionExpired()
        {
            return (HasSubscriberID() && DateTime.UtcNow > COVExpireTime);
        }

        public void SetObjectID()
        {
            if (_InstanceNumber != -1)
                ObjectID = new BACnetObjectIdentifier(BACnetObjectType, (uint)_InstanceNumber);
            else if (_RunTimeInstanceNumber != -1)
                ObjectID = new BACnetObjectIdentifier(BACnetObjectType, (uint)_RunTimeInstanceNumber);
            else
                ObjectID = new BACnetObjectIdentifier(0);
        }

        public bool HasPendingWriteCmd()
        {
            return (_CommandType == CommandTypes.Init && GetTagListOnWritingCount() > 0 && IsBacNetObjectInitializationDone());
        }

        public bool IsBacNetObjectInitializationDone()
        {
            return (_BacnetState == BACnetState.CovSubscribed || _BacnetState == BACnetState.Polling);
        }

        public bool HasCovValue()
        {
            return (TagsList[0].GetReadValue() != null);
        }

        public void GetReadWriteState()
        {
            if (ReadRequest())
            {
                if (IsBacNetObjectInitializationDone()) {
                    if (IsRelinquishRequest())
                        // force relinquish command
                        _CommandType = BACnetCommJob.CommandTypes.WriteCmd;
                    else
                        _CommandType = BACnetCommJob.CommandTypes.ReadCmd;
                }
                else {
                    _CommandType = BACnetCommJob.CommandTypes.Init;
                }
            }
            else
            {
                if (IsBacNetObjectInitializationDone())
                    _CommandType = BACnetCommJob.CommandTypes.WriteCmd;
                else
                    _CommandType = BACnetCommJob.CommandTypes.Init;
            }
        }

        public void ResetBacnetRunTimeParameters()
        {
            if (_COVEnable)
            {
                ((BACnetChannel)Station.GetChannel()).CovAssignUnSubscriberID(this, DriverErrorCodes.ErrorTimeOut);
            }
            _RunTimeInstanceNumber = 0;
            // reinit job id
            SetObjectID();

            _BacnetState = BACnetState.None;
        }

        public bool IsBacnetCovState()
        {
            return (BacnetState == BACnetState.Cov || BacnetState == BACnetState.CovSubscribedPolling || BacnetState == BACnetState.CovSubscribed);
        }

        public bool IsBacnetJobState()
        {
            return (BacnetState >= BACnetState.WhoIs && BacnetState <= BACnetState.Polling);
        }

        public void SetCOVSubscriptionTime()
        {
            if (COVSubscriptionTime == DateTime.MinValue)
            {
                COVSubscriptionTime = DateTime.UtcNow;
            }
            else
            {
                if (DateTime.UtcNow >= COVSubscriptionTime.AddSeconds(((BACnetStation)Station).COVInterval))
                    COVSubscriptionTime = DateTime.UtcNow;
            }
        }

        public uint GetNextCOVScheduletInterval()
        {
            // Cov never expire
            if (COVExpireTime == DateTime.MaxValue)
                return uint.MaxValue;

            // get how mamy time remain from suscription refresh start from now
            int scheduleInterval = (int)COVExpireTime.Subtract(DateTime.UtcNow).TotalMilliseconds;
            
            if (scheduleInterval < 0)
                scheduleInterval = 0;
            //else if (scheduleInterval > Properties.Settings.Default.WhoIsRepeatDelaySlow)
            //    scheduleInterval = Properties.Settings.Default.WhoIsRepeatDelaySlow;

            return (uint)scheduleInterval;
        }

        public void SetCOVExpireTime()
        {
            if (DateTime.UtcNow > COVExpireTime)
            {
                // never expire
                if (((BACnetStation)Station).COVInterval == 0)
                    COVExpireTime = DateTime.MaxValue;
                else
                    COVExpireTime = DateTime.UtcNow.AddSeconds(((BACnetStation)Station).COVInterval);
            }
        }

        public void ResetCOVTimes()
        {
            COVSubscriptionTime = DateTime.MinValue;
            COVExpireTime = DateTime.MinValue;
        }

        #endregion

        #region Override Methods

        public override uint OnWriteTag(NodeId tagnodeid, ref object value, bool forceValue)
        {            
            lock (lockListObject)
            {
                uint ret = base.OnWriteTag(tagnodeid, ref value, true);
                if (ret == StatusCodes.Good)
                {
                    if ((Type == LinkType.ExceptionOutput || Type == LinkType.InputOutput))
                    {
                        // copy "unwritten" data from TagList to write members of struct with one command only
                        foreach (Tag writeTag in TagsList)
                        {
                            if (!TagsListToWrite.Contains(writeTag))
                                TagsListToWrite.Add(writeTag);
                        }
                    }
                }

                SetInUse(tagnodeid, true);

                return ret;
            }
        }


        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Checks if the value of the conditional variable is different from 0 (or false). </summary>
        ///
        /// <returns>   true if the value of the conditional variable is different from 0 or if the conditional variable has not been defined. </returns>
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool IsConditionalVariableOn()
        {
            bool result = true;

            if (conditionalVariableHasBeenSet == true)
            {
                result = base.IsConditionalVariableOn();

                //Check up if Conditional variable was inizialized
                uint uintValue = 0;
                if (jobConditionalVariable.GetStateCommandVariableValue(ref uintValue) == false)
                {
                    return (result);
                }
                
                uint bitMask = (uint)Math.Pow(2, (uint)BACnetConditionalVariableBits.RelinquishCommand);
                
                // Do relinquish?
                if ((uintValue & bitMask) != 0)
                {
                    if (!BACnetEnums.isPrioritySupported(_BACnetObjectType.ToString(), _PropertyIdentifier.ToString()) || (Type == LinkType.Input))
                    {
                        ResetRelinquishForced();
                        result = false;
                    }
                }
            }

            return result;
        }

        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Get a bit of the conditional variable of the job. </summary>
        /////
        ///// <param name="Value" type="uint">   The value. </param>
        ///// <param name="bitIndex" type="BACnetConditionalVariableBits">     The index of the bit. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public bool GetConditionalVariableBit(ref bool bitValue, BACnetConditionalVariableBits bitIndex)
        //{
        //    if (conditionalVariableHasBeenSet == false)
        //    {
        //        return (false);
        //    }

        //    //Check up if Conditional variable was inizialized
        //    if ((jobConditionalVariable.varValue == null) || (jobConditionalVariable.varValue.Value == null))
        //    {
        //        return (false);
        //    }
        //    object value = jobConditionalVariable.varValue.Value;
        //    if (value is Array)
        //    {
        //        return (false);
        //    }
        //    Type systemType = value.GetType();
        //    BuiltInType builtInType = Station.GetBuiltInType(systemType);
        //    uint uintValue = 0;
        //    switch (builtInType)
        //    {
        //        case BuiltInType.Boolean:
        //            {
        //                if ((uint)bitIndex > 0)
        //                {
        //                    return (false);
        //                }

        //                bool boolValue = (bool)value;
        //                if (boolValue == true)
        //                {
        //                    uintValue = 1;
        //                }
        //            }
        //            break;

        //        case BuiltInType.SByte:
        //            {
        //                if ((uint)bitIndex > 7)
        //                {
        //                    return (false);
        //                }

        //                sbyte sbyteValue = (sbyte)value;
        //                uintValue = (uint)sbyteValue;
        //            }
        //            break;

        //        case BuiltInType.Byte:
        //            {
        //                if ((uint)bitIndex > 7)
        //                {
        //                    return (false);
        //                }

        //                byte byteValue = (byte)value;
        //                uintValue = (uint)byteValue;
        //            }
        //            break;

        //        case BuiltInType.Int16:
        //            {
        //                if ((uint)bitIndex > 15)
        //                {
        //                    return (false);
        //                }

        //                short shortValue = (short)value;
        //                uintValue = (uint)shortValue;
        //            }
        //            break;

        //        case BuiltInType.UInt16:
        //            {
        //                if ((uint)bitIndex > 15)
        //                {
        //                    return (false);
        //                }

        //                ushort ushortValue = (ushort)value;
        //                uintValue = (uint)ushortValue;
        //            }
        //            break;

        //        case BuiltInType.Int32:
        //            {
        //                if ((uint)bitIndex > 31)
        //                {
        //                    return (false);
        //                }

        //                int intValue = (int)value;
        //                uintValue = (uint)intValue;
        //            }
        //            break;

        //        case BuiltInType.UInt32:
        //            if ((uint)bitIndex > 31)
        //            {
        //                return (false);
        //            }

        //            uintValue = (uint)value;
        //            break;

        //        case BuiltInType.Float:
        //            {
        //                float FloatValue = (float)value;
        //                uintValue = (uint)FloatValue;
        //            }
        //            break;

        //        case BuiltInType.Int64:
        //            {
        //                Int64 Int64Value = (Int64)value;
        //                uintValue = (uint)Int64Value;
        //            }
        //            break;

        //        case BuiltInType.UInt64:
        //            {
        //                UInt64 UInt64Value = (UInt64)value;
        //                uintValue = (uint)UInt64Value;
        //            }
        //            break;

        //        case BuiltInType.Double:
        //            {
        //                Double DoubleValue = (Double)value;
        //                uintValue = (uint)DoubleValue;
        //            }
        //            break;

        //        case BuiltInType.String:
        //            {
        //                string st = value.ToString();
        //                if (!string.IsNullOrWhiteSpace(st))
        //                {
        //                    if (!uint.TryParse(st, out uintValue))
        //                    {
        //                        return (false);
        //                    }
        //                }
        //            }
        //            break;

        //        default:
        //            return (false);
        //    }

        //    uint bitMask = (uint)Math.Pow(2, (uint)bitIndex);
        //    bitValue = false;
        //    if ((uintValue & bitMask) != 0)
        //    {
        //        bitValue = true;
        //    }

        //    return (true);
        //}
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   Set a bit of the conditional variable of the job. </summary>
        /////
        ///// <param name="bitValue" type="bool">   The bit new value. </param>
        ///// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public bool SetConditionalVariableBit(bool bitValue, BACnetConditionalVariableBits bitIndex)
        //{
        //    if (conditionalVariableHasBeenSet == false)
        //    {
        //        return (false);
        //    }
        //    bool bitCurrentValue = false;
        //    if (GetConditionalVariableBit(ref bitCurrentValue, bitIndex) == false)
        //    {
        //        return (false);
        //    }
        //    if (bitCurrentValue == bitValue)
        //    {
        //        return (true);
        //    }

        //    //Check up if Conditional variable was inizialized
        //    if ((jobConditionalVariable.varValue == null) || (jobConditionalVariable.varValue.Value == null))
        //    {
        //        return (false);
        //    }
        //    object currentValue = jobConditionalVariable.varValue.Value;
        //    if (currentValue is Array)
        //    {
        //        return (false);
        //    }

        //    Type systemType = currentValue.GetType();
        //    BuiltInType builtInType = Station.GetBuiltInType(systemType);
        //    uint uintValue = 0;
        //    switch (builtInType)
        //    {
        //        case BuiltInType.Boolean:
        //            {
        //                if ((uint)bitIndex > 0)
        //                {
        //                    return (false);
        //                }

        //                bool boolValue = (bool)currentValue;
        //                if (boolValue == true)
        //                {
        //                    uintValue = 1;
        //                }
        //            }
        //            break;

        //        case BuiltInType.SByte:
        //            {
        //                if ((uint)bitIndex > 7)
        //                {
        //                    return (false);
        //                }

        //                sbyte sbyteValue = (sbyte)currentValue;
        //                uintValue = (uint)sbyteValue;
        //            }
        //            break;

        //        case BuiltInType.Byte:
        //            {
        //                if ((uint)bitIndex > 7)
        //                {
        //                    return (false);
        //                }

        //                byte byteValue = (byte)currentValue;
        //                uintValue = (uint)byteValue;
        //            }
        //            break;

        //        case BuiltInType.Int16:
        //            {
        //                if ((uint)bitIndex > 15)
        //                {
        //                    return (false);
        //                }

        //                short shortValue = (short)currentValue;
        //                uintValue = (uint)shortValue;
        //            }
        //            break;

        //        case BuiltInType.UInt16:
        //            {
        //                if ((uint)bitIndex > 15)
        //                {
        //                    return (false);
        //                }

        //                ushort ushortValue = (ushort)currentValue;
        //                uintValue = (uint)ushortValue;
        //            }
        //            break;

        //        case BuiltInType.Int32:
        //            {
        //                if ((uint)bitIndex > 31)
        //                {
        //                    return (false);
        //                }

        //                int intValue = (int)currentValue;
        //                uintValue = (uint)intValue;
        //            }
        //            break;

        //        case BuiltInType.UInt32:
        //            if ((uint)bitIndex > 31)
        //            {
        //                return (false);
        //            }

        //            uintValue = (uint)currentValue;
        //            break;

        //        case BuiltInType.Float:
        //            if ((uint)bitIndex > 31)
        //            {
        //                return (false);
        //            }
        //            float FloatValue = (float)currentValue;
        //            uintValue = (uint)FloatValue;

        //            break;

        //        case BuiltInType.Int64:
        //            if ((uint)bitIndex > 64)
        //            {
        //                return (false);
        //            }
        //            Int64 Int64Value = (Int64)currentValue;
        //            uintValue = (uint)Int64Value;

        //            break;

        //        case BuiltInType.UInt64:
        //            if ((uint)bitIndex > 64)
        //            {
        //                return (false);
        //            }
        //            UInt64 UInt64Value = (UInt64)currentValue;
        //            uintValue = (uint)UInt64Value;

        //            break;

        //        case BuiltInType.Double:
        //            if ((uint)bitIndex > 64)
        //            {
        //                return (false);
        //            }
        //            Double DoubleValue = (Double)currentValue;
        //            uintValue = (uint)DoubleValue;

        //            break;

        //        case BuiltInType.String:
        //            {
        //                string st = currentValue.ToString();
        //                if (!string.IsNullOrWhiteSpace(st))
        //                {
        //                    if (!uint.TryParse(st, out uintValue))
        //                    {
        //                        return (false);
        //                    }
        //                }
        //            }
        //            break;

        //        default:
        //            return (false);
        //    }

        //    uint uintNewValue = uintValue;
        //    uint bitMask = (uint)Math.Pow(2, (uint)bitIndex);
        //    if (bitValue == false)
        //    {
        //        uintNewValue &= ~bitMask;
        //    }
        //    else
        //    {
        //        uintNewValue |= bitMask;
        //    }

        //    //jobConditionalVariable.varValue = null;
        //    jobConditionalVariable.varValue = new DataValue(new Variant(uintNewValue));

        //    Station.GetCommDriver().OnTagChanged(jobConditionalVariable.varNodeId, jobConditionalVariable.varValue);

        //    return (true);
        //}


        public override bool IsJobAggregable()
        {
            return false;
        }
        #endregion

        #region Properties

        /// <summary>   The Object Type. </summary>
        private BACnetEnums.ObjectTypes _BACnetObjectType;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   The Object Type. </summary>
        ///
        /// <value> The Object Type. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetEnums.ObjectTypes BACnetObjectType
        {
            get { return _BACnetObjectType; }
            set { _BACnetObjectType = value; }
        }

        /// <summary>   The Property Identifier. </summary>
        private BACnetEnums.PropertyIdentifier _PropertyIdentifier;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Property Identifier. </summary>
        ///
        /// <value> The Property Identifier. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetEnums.PropertyIdentifier PropertyIdentifier
        {
            get { return _PropertyIdentifier; }
            set { _PropertyIdentifier = value; }
        }

        /// <summary>   The object name. </summary>
        private string _ObjectName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   object name. </summary>
        ///
        /// <value> The object name. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string ObjectName
        {
            get { return _ObjectName; }
            set { _ObjectName = value; }
        }

        public string ObjectFullName
        {
            get { return string.Format("{0}_{1}_{2}_{3}", Station.Name, _ObjectName, _BACnetObjectType, _InstanceNumber); }
        }

        /// <summary>   The COV Enable. </summary>
        private bool _COVEnable;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   COV Enable. </summary>
        ///
        /// <value> The COV Enable. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool COVEnable
        {
            get { return _COVEnable; }
            set { _COVEnable = value; }
        }

        /// <summary>   The Data Size. </summary>
        private ushort _DataSize;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Data Size. </summary>
        ///
        /// <value> The Data Size. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ushort DataSize
        {
            get { return _DataSize; }
            set { _DataSize = value; }
        }

        /// <summary>   The Array Index. </summary>
        private ushort _ArrayIndex;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Array Index. </summary>
        ///
        /// <value> The Array Index. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public ushort ArrayIndex
        {
            get { return _ArrayIndex; }
            set { _ArrayIndex = value; }
        }

        /// <summary>   The Data Log Mode. </summary>
        private DataLogModes _DataLogMode;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Data Log Mode. </summary>
        ///
        /// <value> The Data Log Mode. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public DataLogModes DataLogMode
        {
            get { return _DataLogMode; }
            set { _DataLogMode = value; }
        }

        /// <summary>   The PriorityLevel. </summary>
        private PriorityLevels _PriorityLevel;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Priority Level. </summary>
        ///
        /// <value> The Priority Level. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public PriorityLevels PriorityLevel
        {
            get { return _PriorityLevel; }
            set { PriorityLevel = value; }
        }

        /// <summary>   read/write Data. </summary>
        private List<byte> _writeData = new List<byte>();
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Bufer for write data. </summary>
        ///
        /// <value> The write data Bufer. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public List<byte> writeData
        {
            get { return _writeData; }
            set { _writeData = value; }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   String for grouping the BACnet CommJobs Object Type. </summary>
        ///
        /// <value> The group string. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override string GroupString
        {
            get
            {
                if (Station == null)
                    return string.Empty;
                string ret = base.GroupString;
                return string.Format("{0}OBT{1:00}", ret, (uint)BACnetObjectType);
            }
        }

        /// <summary>   The Instance Number. </summary>
        private Int32 _InstanceNumber = -1;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Instance Number. </summary>
        ///
        /// <value> The Instance Number. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Int32 InstanceNumber
        {
            get { return _InstanceNumber; }
            set { _InstanceNumber = value; }
        }

        private int _RunTimeInstanceNumber = -1;
        public Int32 RunTimeInstanceNumber
        {
            get { return _RunTimeInstanceNumber; }
            set { _RunTimeInstanceNumber = value; }
        }

        private BACnetState _BacnetState = BACnetState.None;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Instance Number. </summary>
        ///
        /// <value> The Instance Number. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetState BacnetState
        {
            get { return _BacnetState; }
            set { _BacnetState = value; }
        }

        CommandTypes _CommandType = CommandTypes.Invalid;
        public CommandTypes CommandType
        {
            get { return _CommandType; }
            set { _CommandType = value; }
        }

        public override uint SamplingInterval
        {
            get
            {
                if (_COVEnable)
                {
                    if (ConditionalVariableSet && IsConditionalVariableOn())
                        return _SamplingInterval;

                    if (_BacnetState == BACnetCommJob.BACnetState.CovSubscribed)
                    {
                        uint interval = GetNextCOVScheduletInterval();
                        if (interval == uint.MaxValue)
                            return CommJob.JOB_NOT_SCHEDULABLE;
                        else
                            return interval;
                    }
                }
             
                return _SamplingInterval;                
            }

        }    
        #endregion
    }
}

