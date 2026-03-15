using System;
using System.Collections.Generic;
using System.Linq;
using Opc.Ua;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;

namespace EIB
{
    public enum EibCommJobStatus : byte
    {
        Idle,
        ReadRequestPending,
        WriteRequestPending,
        WaitingToSendRequest
    }

    /// <summary>   Bits of the conditional variable of a job. </summary>
    public enum EIBConditionalVariableBits : ushort
    {
        BitReadError  = 0,
        BitWriteError = 1,
        BitForceRead  = 2,
        BitForceWrite = 3,
        BitNewData    = 7
    }

    public class EIBCommJob : CommJob
    {
        public const double EIB_EIS5_MAX_VALUE = 670760.96;
        public const double EIB_EIS5_MIN_VALUE = -671088.64;

        #region Constructors
        public EIBCommJob(Station station, EIBCommJobSettings settings)
            : base(station, settings)
        {
            _EnablePolling = settings.EnablePolling;
            _RetryInitialPolling = settings.RetryInitialPolling;
            _PollingOnlyOnRequest = settings.PollingOnlyOnRequest;
            _OutputOnlyOnRequest = settings.OutputOnlyOnRequest;
            _AutoResetNewDataTime = settings.AutoResetNewDataTime;
            _RetryOutput = settings.RetryOutput;
            _InputGroups = settings.InputGroups;
            _PollingGroup = settings.PollingGroup;
            _OutputGroup = settings.OutputGroup;
            _DataFormat = settings.DataFormat;
            _PollingTime = settings.PollingTime;

            // Set runtime properties
            _PollingEnabled = _EnablePolling;
            _EnableOnlyInitialPolling = true;
            if (_EnablePolling && (_PollingTime > 0))
            {
                _EnableOnlyInitialPolling = false;
            }
            _Status = EibCommJobStatus.Idle;
            _LastReadRequestTimeStamp = DateTime.MinValue;
            _PollingGroupIntValue = (int)EibGroupAddressToUInt16(_PollingGroup);
            _OutputGroupIntValue = (int)EibGroupAddressToUInt16(_OutputGroup);
            //_ErrorCode = (EIBProtocol.EIB_ERROR_CODES)DriverErrorCodes.ErrorNoError;

            // Set data members
            ParseListOfGroupAddresses(_InputGroups, InputGroupIntValues);

            // Check if the job is valid 
            CheckJobValid();

            //if (PollingTime > 0)
            //    SamplingMinInterval = PollingTime;
        }

        public EIBCommJob(Station station, EIBTag defTag)
            : base(station, defTag)
        {
            _EnablePolling = defTag.EIBDynSettings.EnablePolling;
            _RetryInitialPolling = defTag.EIBDynSettings.RetryInitialPolling;
            _PollingOnlyOnRequest = defTag.EIBDynSettings.PollingOnlyOnRequest;
            _OutputOnlyOnRequest = defTag.EIBDynSettings.OutputOnlyOnRequest;
            _AutoResetNewDataTime = defTag.EIBDynSettings.AutoResetNewDataTime;
            _RetryOutput = defTag.EIBDynSettings.RetryOutput;
            _InputGroups = defTag.EIBDynSettings.InputGroups;
            _PollingGroup = defTag.EIBDynSettings.PollingGroup;
            _OutputGroup = defTag.EIBDynSettings.OutputGroup;
            _DataFormat = defTag.EIBDynSettings.DataFormat;
            _PollingTime = defTag.EIBDynSettings.PollingTime;

            // Set runtime properties
            _PollingEnabled = _EnablePolling;
            _EnableOnlyInitialPolling = true;
            if (_EnablePolling && (_PollingTime > 0))
            {
                _EnableOnlyInitialPolling = false;
            }
            _Status = EibCommJobStatus.Idle;
            _LastReadRequestTimeStamp = DateTime.MinValue;
            _PollingGroupIntValue = (int)EibGroupAddressToUInt16(_PollingGroup);
            _OutputGroupIntValue = (int)EibGroupAddressToUInt16(_OutputGroup);
            _ErrorCode = (EIBProtocol.EIB_ERROR_CODES)DriverErrorCodes.ErrorNoError;

            // Set data members
            ParseListOfGroupAddresses(_InputGroups, InputGroupIntValues);

            CheckJobValid();

            //if (PollingTime > 0)
            //    SamplingMinInterval = PollingTime;
        }

        public EIBCommJob(Station station)
            : base(station)
        {
            _EnablePolling = true;
            _EnableOnlyInitialPolling = true;
            _RetryInitialPolling = false;
            _PollingOnlyOnRequest = false;
            _OutputOnlyOnRequest = false;
            _AutoResetNewDataTime = 0;
            _RetryOutput = false;
            _InputGroups = String.Empty;
            _PollingGroup = String.Empty;
            _OutputGroup = String.Empty;
            _DataFormat = (int)EIBProtocol.EISDATAFORMAT.EISDFBit;

            // Set runtime properties
            _PollingEnabled = false;
            _Status = EibCommJobStatus.Idle;
            _LastReadRequestTimeStamp = DateTime.MinValue;
            _PollingGroupIntValue = 0;
            _OutputGroupIntValue = 0;
            _ErrorCode = (EIBProtocol.EIB_ERROR_CODES)DriverErrorCodes.ErrorNoError;

            //if (PollingTime > 0)
            //    SamplingMinInterval = PollingTime;
        }

        protected EIBCommJob()
        {
            _EnablePolling = true;
            _EnableOnlyInitialPolling = true;
            _RetryInitialPolling = false;
            _PollingOnlyOnRequest = false;
            _OutputOnlyOnRequest = false;
            _AutoResetNewDataTime = 0;
            _RetryOutput = false;
            _InputGroups = String.Empty;
            _PollingGroup = String.Empty;
            _OutputGroup = String.Empty;
            _DataFormat = (int)EIBProtocol.EISDATAFORMAT.EISDFBit;

            // Set runtime properties
            _PollingEnabled = false;
            _Status = EibCommJobStatus.Idle;
            _LastReadRequestTimeStamp = DateTime.MinValue;
            _PollingGroupIntValue = 0;
            _OutputGroupIntValue = 0;
            _ErrorCode = (EIBProtocol.EIB_ERROR_CODES)DriverErrorCodes.ErrorNoError;
        }
        #endregion

        #region Data members
        List<int> InputGroupIntValues = new List<int>();
        #endregion

        #region Override Methods

        public override uint GetMaxJobSize()
        {
            return 10;
        }

        public override JobAggregationType TestAggregateJob(CommJob candJob, out uint ExtraBytes)
        {
            ExtraBytes = 0;
            return JobAggregationType.JobAggregImpossible;//steve 280711
        }

        public override bool AggregateJob(CommJob candJob, JobAggregationType AggType, uint ExtraBytes)
        {
            switch (AggType)
            {
                case JobAggregationType.JobAggregFits:
                    TagsList.Add(new EIBTag(candJob.TagsList[0].TagNode, candJob.TagsList[0].ByteOffset, 0));
                    break;
                default:
                    return false;
            }
            return true;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Checks if the value of the conditional variable is different from 0 (or false). </summary>
        ///
        /// <returns>   true if the value of the conditional variable is different from 0 or if the conditional variable has not been defined. </returns>
        /////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool IsConditionalVariableOn()
        {
            bool returnValue = true;

            if (conditionalVariableHasBeenSet == true)
            {
                AutoResetNewDataFlag();

                // Manage job properties specific of this driver
                if(((Type == LinkType.Input) || (Type == LinkType.InputOutput))
                   && (_PollingEnabled == true)
                   && (_PollingOnlyOnRequest == false))
                {
                    return (true);
                }
                else if ((Type != LinkType.Input) &&
                         (_OutputOnlyOnRequest == false))
                {
                    return (true);
                }

                ////Check up if Conditional variable was inizialized
                //if ((jobConditionalVariable.varValue == null) || (jobConditionalVariable.varValue.Value == null))
                //{
                //    return (true);
                //}

                // Get the current value of the conditional variable
                //object value = jobConditionalVariable.varValue.Value;
                //if ((value != null) && !(value is Array))
                //{
                uint uintValue = 0;
                if (jobConditionalVariable.GetStateCommandVariableValue(ref uintValue) == true)
                {
                   // uint uintValue = ConvertValueToUint(value);
                    // Force Polling?
                    if ((Type == LinkType.Input) || (Type == LinkType.InputOutput))
                    {
                        if((uintValue & 0x04) != 0)
                        {
                            return (true);
                        }
                    }

                    // Force Writing?
                    if (Type != LinkType.Input)
                    {
                        if ((uintValue & 0x08) != 0)
                        {
                            return (true);
                        }
                    }
                }

                returnValue = false;
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
        }

        /// <summary>
        /// Tag management associated to a conditional variable 
        /// </summary>
        /// <param name="value">Conditional var value</param>
        public override void ManageUpdatedValueForTheConditionalVariable(DataValue value)
        {
            base.ManageUpdatedValueForTheConditionalVariable(value);

            // FORCE_WRITE_BIT_NUM (Bit 3) = 1, the job associated to the conditional variable is performed even if the variable linked to the task is not in use
            if (WriteCanBePerformed(true))
            {
                if (TagsList[0].Value.Value != null)
                {
                    // get last send or initial value
                    object WriteValue = Utils.Clone(TagsList[0].Value.Value);

                    // force tag to be written with last value
                    OnWriteTag(TagsList[0].TagNode.NodeId, ref WriteValue, true);
                }
            }

            // FORCE_READ_BIT_NUM (Bit 2) = 1, the job associated to the conditional variable is performed even if the variable linked to the task is not in use
            if (PollingCanBePerformed(true, out uint nextExecutionTime))
            {
                // force tag to be is use
                Station.GetChannel().ChangeStateJob(this, CommJobState.PollingNow);
            }
        }
        #endregion

        #region Specific Methods

        //public void ProcessNewData(byte[] Data)
        //{
        //    // Check the argument
        //    if ((Data == null) || (Data.GetLength(0) < 1))
        //    {
        //        return;
        //    }

        //    // Check the tag list
        //    if (TagsList.Count < 1)
        //    {
        //        return;
        //    }

        //    if (TagsList[0].DynSettings.MethodID != -1)
        //    {
        //        return;
        //    }

        //    // Set the tag value
        //    switch (_DataFormat)
        //    {
        //        case (int)EISDATAFORMAT.EISDFBit:
        //            TagsList[0].SetTagValue(ref Data, 0);
        //            SetErrorState((int)DriverErrorCodes.ErrorNoError);
        //            Station.GetCommDriver().OnTagChanged(TagsList[0].TagNode.NodeId, TagsList[0].Value);
        //            if(PollingEnabled && EnableOnlyInitialPolling)
        //            {
        //                PollingEnabled = false;
        //            }
        //            if(Status == EibCommJobStatus.ReadRequestPending)
        //            {
        //                Status = EibCommJobStatus.Idle;
        //            }
        //            LastExecutionTime = DateTime.UtcNow;
        //            LastReadRequestTimeStamp = LastExecutionTime;
        //        break;

        //        case (int)EISDATAFORMAT.EISDFByte:
        //            TagsList[0].SetTagValue(ref Data, 0);
        //            SetErrorState((int)DriverErrorCodes.ErrorNoError);
        //            Station.GetCommDriver().OnTagChanged(TagsList[0].TagNode.NodeId, TagsList[0].Value);
        //            if(PollingEnabled && EnableOnlyInitialPolling)
        //            {
        //                PollingEnabled = false;
        //            }
        //            if(Status == EibCommJobStatus.ReadRequestPending)
        //            {
        //                Status = EibCommJobStatus.Idle;
        //            }
        //            LastExecutionTime = DateTime.UtcNow;
        //            LastReadRequestTimeStamp = LastExecutionTime;
        //        break;

        //        case (int)EISDATAFORMAT.EISDFWord:
        //            TagsList[0].SetTagValue(ref Data, 0);
        //            SetErrorState((int)DriverErrorCodes.ErrorNoError);
        //            Station.GetCommDriver().OnTagChanged(TagsList[0].TagNode.NodeId, TagsList[0].Value);
        //            if(PollingEnabled && EnableOnlyInitialPolling)
        //            {
        //                PollingEnabled = false;
        //            }
        //            if(Status == EibCommJobStatus.ReadRequestPending)
        //            {
        //                Status = EibCommJobStatus.Idle;
        //            }
        //            LastExecutionTime = DateTime.UtcNow;
        //            LastReadRequestTimeStamp = LastExecutionTime;
        //        break;

        //        case (int)EISDATAFORMAT.EISDFDWord:
        //            TagsList[0].SetTagValue(ref Data, 0);
        //            SetErrorState((int)DriverErrorCodes.ErrorNoError);
        //            Station.GetCommDriver().OnTagChanged(TagsList[0].TagNode.NodeId, TagsList[0].Value);
        //            if(PollingEnabled && EnableOnlyInitialPolling)
        //            {
        //                PollingEnabled = false;
        //            }
        //            if(Status == EibCommJobStatus.ReadRequestPending)
        //            {
        //                Status = EibCommJobStatus.Idle;
        //            }
        //            LastExecutionTime = DateTime.UtcNow;
        //            LastReadRequestTimeStamp = LastExecutionTime;
        //        break;

        //        case (int)EISDATAFORMAT.EISDFInt64:
        //            TagsList[0].SetTagValue(ref Data, 0);
        //            SetErrorState((int)DriverErrorCodes.ErrorNoError);
        //            Station.GetCommDriver().OnTagChanged(TagsList[0].TagNode.NodeId, TagsList[0].Value);
        //            if (PollingEnabled && EnableOnlyInitialPolling)
        //            {
        //                PollingEnabled = false;
        //            }
        //            if (Status == EibCommJobStatus.ReadRequestPending)
        //            {
        //                Status = EibCommJobStatus.Idle;
        //            }
        //            LastExecutionTime = DateTime.UtcNow;
        //            LastReadRequestTimeStamp = LastExecutionTime;
        //            break;

        //        case (int)EISDATAFORMAT.EISDFFloat:
        //        {
        //            if (Data.Count() != 4)
        //            {
        //                return;
        //            }
        //            byte[] LocalBuffer = new byte[4];
        //            int i = 0;
        //            for (i = 0; i < 4; i++)
        //            {
        //                LocalBuffer[i] = Data[i];
        //            }
        //            SwapByteBuffer(ref LocalBuffer);
        //            SwapWordBuffer(ref LocalBuffer);
        //            TagsList[0].SetTagValue(ref LocalBuffer, 0);
        //            SetErrorState((int)DriverErrorCodes.ErrorNoError);
        //            Station.GetCommDriver().OnTagChanged(TagsList[0].TagNode.NodeId, TagsList[0].Value);
        //            if (PollingEnabled && EnableOnlyInitialPolling)
        //            {
        //                PollingEnabled = false;
        //            }
        //            if (Status == EibCommJobStatus.ReadRequestPending)
        //            {
        //                Status = EibCommJobStatus.Idle;
        //            }
        //            LastExecutionTime = DateTime.UtcNow;
        //            LastReadRequestTimeStamp = LastExecutionTime;
        //        }
        //        break;

        //        // Time: dddhhhhh 00mmmmmm 00ssssss
        //        // d = day: 0 = no day, 1 = monday ... 7 = sunday
        //        // h = hours
        //        // m = minutes
        //        // s = seconds
        //        case (int)EISDATAFORMAT.EISDFEIS3:
        //        {
        //            if (Data.Count() != 3)
        //            {
        //                return;
        //            }
        //            byte[] LocalBuffer = new byte[4];
        //            // Day
        //            LocalBuffer[0] = Data[0];
        //            LocalBuffer[0] &= 0xE0;
        //            LocalBuffer[0] >>= 5;
        //            // Hours
        //            LocalBuffer[1] = (byte) (Data[0] & 0x1F);
        //            // Minutes
        //            LocalBuffer[2] = Data[1];
        //            // Seconds
        //            LocalBuffer[3] = Data[2];

        //            TagsList[0].SetTagValue(ref LocalBuffer, 0);
        //            SetErrorState((int)DriverErrorCodes.ErrorNoError);
        //            Station.GetCommDriver().OnTagChanged(TagsList[0].TagNode.NodeId, TagsList[0].Value);
        //            if (PollingEnabled && EnableOnlyInitialPolling)
        //            {
        //                PollingEnabled = false;
        //            }
        //            if (Status == EibCommJobStatus.ReadRequestPending)
        //            {
        //                Status = EibCommJobStatus.Idle;
        //            }
        //            LastExecutionTime = DateTime.UtcNow;
        //            LastReadRequestTimeStamp = LastExecutionTime;
        //        }
        //        break;

        //        // Date: 000DDDDD 0000MMMM 0YYYYYYY
        //        // D = day: 1 .. 31
        //        // M = Month: 1 .. 12
        //        // Y = Year: 0 .. 99
        //        case (int)EISDATAFORMAT.EISDFEIS4:
        //        {
        //            if (Data.Count() != 3)
        //            {
        //                return;
        //            }
        //            byte[] LocalBuffer = new byte[4];
        //            // Day
        //            LocalBuffer[0] = Data[0];
        //            // Month
        //            LocalBuffer[1] = Data[1];
        //            // Year
        //            UInt16 AuxUShort = Data[2];
        //            if (AuxUShort < 90)
        //            {
        //                AuxUShort += 100;
        //            }
        //            AuxUShort += 1900;
        //            byte[] BufferUInt16 = BitConverter.GetBytes(AuxUShort);
        //            LocalBuffer[2] = BufferUInt16[0];
        //            LocalBuffer[3] = BufferUInt16[1];

        //            TagsList[0].SetTagValue(ref LocalBuffer, 0);
        //            SetErrorState((int)DriverErrorCodes.ErrorNoError);
        //            Station.GetCommDriver().OnTagChanged(TagsList[0].TagNode.NodeId, TagsList[0].Value);
        //            if (PollingEnabled && EnableOnlyInitialPolling)
        //            {
        //                PollingEnabled = false;
        //            }
        //            if (Status == EibCommJobStatus.ReadRequestPending)
        //            {
        //                Status = EibCommJobStatus.Idle;
        //            }
        //            LastExecutionTime = DateTime.UtcNow;
        //            LastReadRequestTimeStamp = LastExecutionTime;
        //        }
        //        break;

        //        // Value: SEEEEMMM MMMMMMMM
        //        // S = Sign, E = exponent basis 2, M = mantissa
        //        case (int)EISDATAFORMAT.EISDFEIS5:
        //        {
        //            if (Data.Count() != 2)
        //            {
        //                return;
        //            }

        //            // Sign
        //            byte AuxByte = (byte)(Data[0] & 0x80);
        //            double Sign = 1.0;
        //            if (AuxByte != 0)
        //            {
        //                Sign = -1.0;
        //            }

        //            // Exponent
        //            AuxByte = (byte)(Data[0] & 0x78);
        //            AuxByte >>= 3;
        //            ushort Exponent = 1;
        //            Exponent <<= AuxByte;

        //            // Mantissa
        //            AuxByte = (byte)(Data[0] & 0x07);
        //            if (Sign < 0.0)
        //            {
        //                AuxByte |= 0x08;
        //            }
        //            ushort Mantissa = AuxByte;
        //            Mantissa <<= 8;
        //            Mantissa += Data[1];
        //            if (Sign < 0.0)
        //            {
        //                // 2-complement
        //                Mantissa = (ushort)(~Mantissa);
        //                Mantissa &= 0x0FFF;
        //                Mantissa += 1;
        //            }

        //            // Value calculation
        //            double AuxDouble = Sign* 0.01 * (double)Mantissa * (double)Exponent;

        //            byte[] LocalBuffer = BitConverter.GetBytes(AuxDouble);

        //            TagsList[0].SetTagValue(ref LocalBuffer, 0);
        //            SetErrorState((int)DriverErrorCodes.ErrorNoError);
        //            Station.GetCommDriver().OnTagChanged(TagsList[0].TagNode.NodeId, TagsList[0].Value);
        //            if (PollingEnabled && EnableOnlyInitialPolling)
        //            {
        //                PollingEnabled = false;
        //            }
        //            if (Status == EibCommJobStatus.ReadRequestPending)
        //            {
        //                Status = EibCommJobStatus.Idle;
        //            }
        //            LastExecutionTime = DateTime.UtcNow;
        //            LastReadRequestTimeStamp = LastExecutionTime;
        //        }
        //        break;

        //        // Scaling
        //        case (int)EISDATAFORMAT.EISDFEIS6:
        //        {
        //            UInt16 AuxUShort = Data[0];
        //            AuxUShort *= 100;
        //            AuxUShort += 49;
        //            AuxUShort /= 255;
        //            byte[] LocalBuffer = new byte[1];
        //            LocalBuffer[0] = (byte)AuxUShort;

        //            TagsList[0].SetTagValue(ref LocalBuffer, 0);
        //            SetErrorState((int)DriverErrorCodes.ErrorNoError);
        //            Station.GetCommDriver().OnTagChanged(TagsList[0].TagNode.NodeId, TagsList[0].Value);
        //            if (PollingEnabled && EnableOnlyInitialPolling)
        //            {
        //                PollingEnabled = false;
        //            }
        //            if (Status == EibCommJobStatus.ReadRequestPending)
        //            {
        //                Status = EibCommJobStatus.Idle;
        //            }
        //            LastExecutionTime = DateTime.UtcNow;
        //            LastReadRequestTimeStamp = LastExecutionTime;
        //        }
        //        break;

        //        case (int)EISDATAFORMAT.EISDFAccessPWD6Bytes:
        //            if (Data.Count() != 6)
        //            {
        //                return;
        //            }
        //            TagsList[0].SetTagValue(ref Data, 0);
        //            SetErrorState((int)DriverErrorCodes.ErrorNoError);
        //            Station.GetCommDriver().OnTagChanged(TagsList[0].TagNode.NodeId, TagsList[0].Value);
        //            if(PollingEnabled && EnableOnlyInitialPolling)
        //            {
        //                PollingEnabled = false;
        //            }
        //            if(Status == EibCommJobStatus.ReadRequestPending)
        //            {
        //                Status = EibCommJobStatus.Idle;
        //            }
        //            LastExecutionTime = DateTime.UtcNow;
        //            LastReadRequestTimeStamp = LastExecutionTime;
        //        break;

        //        case (int)EISDATAFORMAT.EISDFAccessPWD10Bytes:
        //            if (Data.Count() != 10)
        //            {
        //                return;
        //            }
        //            TagsList[0].SetTagValue(ref Data, 0);
        //            SetErrorState((int)DriverErrorCodes.ErrorNoError);
        //            Station.GetCommDriver().OnTagChanged(TagsList[0].TagNode.NodeId, TagsList[0].Value);
        //            if(PollingEnabled && EnableOnlyInitialPolling)
        //            {
        //                PollingEnabled = false;
        //            }
        //            if(Status == EibCommJobStatus.ReadRequestPending)
        //            {
        //                Status = EibCommJobStatus.Idle;
        //            }
        //            LastExecutionTime = DateTime.UtcNow;
        //            LastReadRequestTimeStamp = LastExecutionTime;
        //        break;
        //    }
        //}

        public override void GetJobData(ref object jobData)
        {
            Tag cand;
            List<byte> outData = new List<byte>();
            lock (lockListObject)
            {
                // Check the tag list
                if (TagsListOnWriting.Count == 0)
                    return;
                cand = TagsListOnWriting[0];
 
                cand.LastValue = cand.Value.Value;
            
                if (cand.DynSettings.MethodID != -1)
                {
                    return;
                }
                if (cand.Size == 0)
                {
                    return;
                }

                // Prepare output data
                byte[] tagData = null;
                switch (_DataFormat)
                {
                    case (int)EIBProtocol.EISDATAFORMAT.EISDFBit:
                    case (int)EIBProtocol.EISDATAFORMAT.EISDFByte:
                        {
                            tagData = new byte[1];
                            cand.GetTagBuffer(ref tagData, false, 0, 1);
                            //outData.AddRange(tagData);
                        }
                        break;

                    case (int)EIBProtocol.EISDATAFORMAT.EISDFWord:
                        {
                            tagData = new byte[2];
                            cand.GetTagBuffer(ref tagData, false, 0, 2);
                           // outData.AddRange(tagData);
                        }
                        break;

                    case (int)EIBProtocol.EISDATAFORMAT.EISDFDWord:
                        {
                            tagData = new byte[4];
                            cand.GetTagBuffer(ref tagData, false, 0, 4);
                            //outData.AddRange(tagData);
                        }
                        break;

                    case (int)EIBProtocol.EISDATAFORMAT.EISDFFloat:
                        {
                            tagData = new byte[4];
                            cand.GetTagBuffer(ref tagData, false, 0, 4);
                            SwapByteBuffer(ref tagData);
                            SwapWordBuffer(ref tagData);
                            //outData.AddRange(tagData);
                        }
                        break;

                    case (int)EIBProtocol.EISDATAFORMAT.EISDFInt64:
                        {
                            tagData = new byte[8];
                            cand.GetTagBuffer(ref tagData, false, 0, 8);
                            SwapByteBuffer(ref tagData);
                            SwapWordBuffer(ref tagData);
                            SwapDWordBuffer(ref tagData);
                            //outData.AddRange(tagData);
                        }
                        break;

                    // Time: dddhhhhh 00mmmmmm 00ssssss
                    // d = day: 0 = no day, 1 = monday ... 7 = sunday
                    // h = hours
                    // m = minutes
                    // s = seconds
                    case (int)EIBProtocol.EISDATAFORMAT.EISDFEIS3:
                        {
                            byte[] localBuffer = new byte[4];
                            cand.GetTagBuffer(ref localBuffer, false, 0, 4);
                            tagData = new byte[3];
                            byte byteAux = localBuffer[0]; // day
                            byteAux &= 0x7;
                            byteAux <<= 5;
                            byte byteAux2 = localBuffer[1]; // hour
                            byteAux2 &= 0x1f;
                            tagData[0] = (byte)(byteAux | byteAux2);
                            byteAux = localBuffer[2]; // minutes
                            byteAux &= 0x3f;
                            tagData[1] = byteAux;
                            byteAux = localBuffer[3]; // seconds
                            byteAux &= 0x3f;
                            tagData[2] = byteAux;
                            //outData.AddRange(tagData);
                        }
                        break;

                    // Date: 000DDDDD 0000MMMM 0YYYYYYY
                    // D = day: 1 .. 31
                    // M = Month: 1 .. 12
                    // Y = Year: 0 .. 99
                    case (int)EIBProtocol.EISDATAFORMAT.EISDFEIS4:
                        {
                            byte[] localBuffer = new byte[4];
                            cand.GetTagBuffer(ref localBuffer, false, 0, 4);
                            tagData = new byte[3];
                            byte byteAux = localBuffer[0]; // day
                            byteAux &= 0x1f;
                            tagData[0] = byteAux;
                            byteAux = localBuffer[1]; // month
                            byteAux &= 0xf;
                            tagData[1] = byteAux;
                            UInt16 wordAux = localBuffer[3]; // year
                            wordAux <<= 8;
                            wordAux += localBuffer[2];
                            if(wordAux <= 1990)
                            {
                                byteAux = 0;
                            }
                            else if(wordAux <= 1999)
                            {
                                byteAux = (byte)(wordAux - 1900);
                            }
                            else if (wordAux <= 2089)
                            {
                                byteAux = (byte)(wordAux - 2000);
                            }
                            else
                            {
                                byteAux = 89;
                            }
                            tagData[2] = byteAux;
                            //outData.AddRange(tagData);
                        }
                        break;

                    // Value: SEEEEMMM MMMMMMMM
                    // S = Sign, E = exponent basis 2, M = mantissa
                    case (int)EIBProtocol.EISDATAFORMAT.EISDFEIS5:
                        {
                            double dAux = 0.0;
                            try
                            {
                                object curVal = cand.Value.Value;
                                dAux = Convert.ToDouble(curVal);
                            }
                            catch (Exception ex)
                            {
                                dAux = 0.0;
                            }

                            // The value to be encoded must be in the representable range 
                            if(dAux > EIB_EIS5_MAX_VALUE)
                            {
                                dAux = EIB_EIS5_MAX_VALUE;
                            }
                            else if(dAux < EIB_EIS5_MIN_VALUE)
                            {
                                dAux = EIB_EIS5_MIN_VALUE;
                            }

                            // Calculation of the mantissa
                            // Resolution = 0.01 --> Multiply by 100 the value to be encoded
                            Double dMantissa = dAux*100.0;
                            // Get the absolute value
                            if(dAux < 0.0)
                            {
                                dMantissa *= -1.0;
                            }
                            // Calculate also the exponent
                            int exponent = 0;
                            while(dMantissa > 2047.0)
                            {
                                dMantissa /= 2.0;
                                exponent++;
                            }
                            // Be sure the exponent is encoded in 4 bits
                            exponent &= 0xf; 
                            // Convert the mantissa to an integer value;
                            int mantissaIntValue = (int)(dMantissa + 0.5);
                            if(mantissaIntValue > 2047)
                            {
                                mantissaIntValue = 2047;
                            }
                            // If the number is negative --> Two's complement
                            if(dAux < 0.0)
                            {
                                mantissaIntValue = ~mantissaIntValue;
                                mantissaIntValue += 1;
                            }
                            // Be sure the mantissa is encode in 11 bits
                            mantissaIntValue &= 0x7ff;

                            // Allocate the converted data buffer
                            tagData = new byte[2];

                            // Set the sign
                            if(dAux < 0.0)
                            {
                                tagData[0] = 0x80;
                            }

                            // Set the exponent
                            tagData[0] |= (byte)(exponent<<3);

                            // Set the mantissa
                            UInt16 wAux = (UInt16)mantissaIntValue;
                            wAux >>= 8;
                            wAux &= 0x7;
                            tagData[0] |= (byte)wAux;
                            wAux = (UInt16)mantissaIntValue;
                            tagData[1] = (byte)wAux;

                            //outData.AddRange(tagData);
                        }
                        break;
                    
                    // Scaling
                    case (int)EIBProtocol.EISDATAFORMAT.EISDFEIS6:
                        {
                            byte[] localBuffer = new byte[1];
                            cand.GetTagBuffer(ref localBuffer, false, 0, 1);
                            tagData = new byte[1];
                            UInt16 wAux = localBuffer[0];
                            if(wAux > 100)
                            {
                                wAux = 100;
                            }
                            wAux *= 255;
                            if(wAux > 0)
                            {
                                wAux += 127;
                            }
                            if(wAux > 25500)
                            {
                                wAux = 25500;
                            }
                            wAux /= 100;
                            tagData[0] = (byte)wAux;
                            //outData.AddRange(tagData);
                        }
                        break;

                    case (int)EIBProtocol.EISDATAFORMAT.EISDFAccessPWD6Bytes:
                        {
                            tagData = new byte[6];
                            cand.GetTagBuffer(ref tagData, false, 0, 6);
                            //outData.AddRange(tagData);
                        }
                        break;

                    case (int)EIBProtocol.EISDATAFORMAT.EISDFAccessPWD10Bytes:
                        {
                            tagData = new byte[10];
                            cand.GetTagBuffer(ref tagData, false, 0, 10);
                            //outData.AddRange(tagData);
                        }
                        break;
                }
                if (tagData != null)
                { 
                    if ((_DataFormat != (int)EIBProtocol.EISDATAFORMAT.EISDFBit)&&
                    (_DataFormat != (int)EIBProtocol.EISDATAFORMAT.EISDFByte)&&
                    (_DataFormat != (int)EIBProtocol.EISDATAFORMAT.EISDFEIS6))
                    {
                        if (SwapBytes)
                        {
                            SwapByteBuffer(ref tagData);
                        }

                        if (SwapWords)
                        {
                            SwapWordBuffer(ref tagData);
                        }
                    }
                    outData.AddRange(tagData);
                }
            }            
            jobData = outData.ToArray();
        }

        public override void SetJobData(object jobData, ref List<Tag> changed)
        {
            byte[] Data = jobData as byte[];

            // Check the argument
            if ((Data == null) || (Data.GetLength(0) < 1))
            {
                if (Status == EibCommJobStatus.WriteRequestPending)
                {
                    Status = EibCommJobStatus.Idle;
                    LastExecutionTime = DateTime.UtcNow;
                    changed.Add(TagsList[0]);
#if DEBUG
                    String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    String DbgText = String.Format(
                    "EIB DBG - SetJDa - {0} No da, wr pen, Out Add = {1}, Chd = {2}",
                    dbgTime, OutputGroup, changed.Count);
                    System.Diagnostics.Debug.WriteLine(DbgText);
#endif
                }

                return;
            }

            base.SetJobData(Data, ref changed);

            // Check the tag list
            if (TagsList.Count < 1)
            {
                return;
            }

            if (TagsList[0].DynSettings.MethodID != -1)
            {
                return;
            }

            // Set the tag value
            switch (_DataFormat)
            {
                case (int)EIBProtocol.EISDATAFORMAT.EISDFBit:
                    {
                        TagsList[0].SetTagValue(ref Data, 0);
                        changed.Add(TagsList[0]);
                        if (PollingEnabled && EnableOnlyInitialPolling)
                        {
                            PollingEnabled = false;
                        }
                        if (Status == EibCommJobStatus.ReadRequestPending)
                        {
                            Status = EibCommJobStatus.Idle;
                        }
                        LastExecutionTime = DateTime.UtcNow;
                        LastReadRequestTimeStamp = LastExecutionTime;

#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgText =
                        String.Format(
                        "EIB DBG - SetJDa - {0} Pol Add = {1} - In Add = {2} - Da (EISDFBit) =",
                        dbgTime, PollingGroup, InputGroups);
                        for (int i = 0; i < Data.GetLength(0); i++)
                        {
                            DbgText += " ";
                            DbgText += Data.GetValue(i).ToString();
                        }
                        System.Diagnostics.Debug.WriteLine(DbgText);
#endif
                    }
                    break;

                case (int)EIBProtocol.EISDATAFORMAT.EISDFByte:
                    {
                        TagsList[0].SetTagValue(ref Data, 0);
                        changed.Add(TagsList[0]);
                        if (PollingEnabled && EnableOnlyInitialPolling)
                        {
                            PollingEnabled = false;
                        }
                        if (Status == EibCommJobStatus.ReadRequestPending)
                        {
                            Status = EibCommJobStatus.Idle;
                        }
                        LastExecutionTime = DateTime.UtcNow;
                        LastReadRequestTimeStamp = LastExecutionTime;

#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgText =
                        String.Format(
                        "EIB DBG - SetJDa - {0} Pol Add = {1} - In Add = {2} - Da (EISDFByte) =",
                        dbgTime, PollingGroup, InputGroups);
                        for (int i = 0; i < Data.GetLength(0); i++)
                        {
                            DbgText += " ";
                            DbgText += Data.GetValue(i).ToString();
                        }
                        System.Diagnostics.Debug.WriteLine(DbgText);
#endif
                    }
                    break;

                case (int)EIBProtocol.EISDATAFORMAT.EISDFWord:
                    {
                        TagsList[0].SetTagValue(ref Data, 0);
                        changed.Add(TagsList[0]);
                        if (PollingEnabled && EnableOnlyInitialPolling)
                        {
                            PollingEnabled = false;
                        }
                        if (Status == EibCommJobStatus.ReadRequestPending)
                        {
                            Status = EibCommJobStatus.Idle;
                        }
                        LastExecutionTime = DateTime.UtcNow;
                        LastReadRequestTimeStamp = LastExecutionTime;
#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgText =
                        String.Format(
                        "EIB DBG - SetJDa - {0} Pol Add = {1} - In Add = {2} - Da (EISDFWord) =",
                        dbgTime, PollingGroup, InputGroups);
                        for (int i = 0; i < Data.GetLength(0); i++)
                        {
                            DbgText += " ";
                            DbgText += Data.GetValue(i).ToString();
                        }
                        System.Diagnostics.Debug.WriteLine(DbgText);
#endif
                    }
                    break;

                case (int)EIBProtocol.EISDATAFORMAT.EISDFDWord:
                    {
                        TagsList[0].SetTagValue(ref Data, 0);
                        changed.Add(TagsList[0]);
                        if (PollingEnabled && EnableOnlyInitialPolling)
                        {
                            PollingEnabled = false;
                        }
                        if (Status == EibCommJobStatus.ReadRequestPending)
                        {
                            Status = EibCommJobStatus.Idle;
                        }
                        LastExecutionTime = DateTime.UtcNow;
                        LastReadRequestTimeStamp = LastExecutionTime;
#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgText =
                        String.Format(
                        "EIB DBG - SetJDa - {0} Pol Add: {1} - In Add: {2} - Da (EISDFDWord):",
                        dbgTime, PollingGroup, InputGroups);
                        for (int i = 0; i < Data.GetLength(0); i++)
                        {
                            DbgText += " ";
                            DbgText += Data.GetValue(i).ToString();
                        }
                        System.Diagnostics.Debug.WriteLine(DbgText);
#endif
                    }
                    break;

                case (int)EIBProtocol.EISDATAFORMAT.EISDFFloat:
                    {
                        if (Data.Count() != 4)
                        {
                            return;
                        }
                        byte[] LocalBuffer = new byte[4];
                        int i = 0;
                        for (i = 0; i < 4; i++)
                        {
                            LocalBuffer[i] = Data[i];
                        }
                        SwapByteBuffer(ref LocalBuffer);
                        SwapWordBuffer(ref LocalBuffer);
                        TagsList[0].SetTagValue(ref LocalBuffer, 0);
                        changed.Add(TagsList[0]);
                        if (PollingEnabled && EnableOnlyInitialPolling)
                        {
                            PollingEnabled = false;
                        }
                        if (Status == EibCommJobStatus.ReadRequestPending)
                        {
                            Status = EibCommJobStatus.Idle;
                        }
                        LastExecutionTime = DateTime.UtcNow;
                        LastReadRequestTimeStamp = LastExecutionTime;
#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgText =
                        String.Format(
                        "EIB DBG - SetJDa - {0} Pol Add: {1} - In Add: {2} - Da (EISDFFloat):",
                        dbgTime, PollingGroup, InputGroups);
                        for (i = 0; i < LocalBuffer.GetLength(0); i++)
                        {
                            DbgText += " ";
                            DbgText += LocalBuffer.GetValue(i).ToString();
                        }
                        System.Diagnostics.Debug.WriteLine(DbgText);
#endif
                    }
                    break;

                case (int)EIBProtocol.EISDATAFORMAT.EISDFInt64:
                    {
                        if (Data.Count() != 8)
                        {
                            return;
                        }
                        byte[] LocalBuffer = new byte[8];
                        int i = 0;
                        for (i = 0; i < 8; i++)
                        {
                            LocalBuffer[i] = Data[i];
                        }
                        SwapByteBuffer(ref LocalBuffer);
                        SwapWordBuffer(ref LocalBuffer);
                        SwapDWordBuffer(ref LocalBuffer);
                        TagsList[0].SetTagValue(ref LocalBuffer, 0);
                        changed.Add(TagsList[0]);
                        if (PollingEnabled && EnableOnlyInitialPolling)
                        {
                            PollingEnabled = false;
                        }
                        if (Status == EibCommJobStatus.ReadRequestPending)
                        {
                            Status = EibCommJobStatus.Idle;
                        }
                        LastExecutionTime = DateTime.UtcNow;
                        LastReadRequestTimeStamp = LastExecutionTime;
#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgText =
                        String.Format(
                        "EIB DBG - SetJDa - {0} Pol Add: {1} - In Add: {2} - Da (EISDInt64):",
                        dbgTime, PollingGroup, InputGroups);
                        for (i = 0; i < LocalBuffer.GetLength(0); i++)
                        {
                            DbgText += " ";
                            DbgText += LocalBuffer.GetValue(i).ToString();
                        }
                        System.Diagnostics.Debug.WriteLine(DbgText);
#endif
                    }
                    break;

                // Time: dddhhhhh 00mmmmmm 00ssssss
                // d = day: 0 = no day, 1 = monday ... 7 = sunday
                // h = hours
                // m = minutes
                // s = seconds
                case (int)EIBProtocol.EISDATAFORMAT.EISDFEIS3:
                    {
                        if (Data.Count() != 3)
                        {
                            return;
                        }
                        byte[] LocalBuffer = new byte[4];
                        // Day
                        LocalBuffer[0] = Data[0];
                        LocalBuffer[0] &= 0xE0;
                        LocalBuffer[0] >>= 5;
                        // Hours
                        LocalBuffer[1] = (byte)(Data[0] & 0x1F);
                        // Minutes
                        LocalBuffer[2] = Data[1];
                        // Seconds
                        LocalBuffer[3] = Data[2];

                        TagsList[0].SetTagValue(ref LocalBuffer, 0);
                        changed.Add(TagsList[0]);
                        if (PollingEnabled && EnableOnlyInitialPolling)
                        {
                            PollingEnabled = false;
                        }
                        if (Status == EibCommJobStatus.ReadRequestPending)
                        {
                            Status = EibCommJobStatus.Idle;
                        }
                        LastExecutionTime = DateTime.UtcNow;
                        LastReadRequestTimeStamp = LastExecutionTime;
#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgText =
                        String.Format(
                        "EIB DBG - SetJDa - {0} Pol Add: {1} - In Add: {2} - Da (EISDFEIS3):",
                        dbgTime, PollingGroup, InputGroups);
                        for (int i = 0; i < LocalBuffer.GetLength(0); i++)
                        {
                            DbgText += " ";
                            DbgText += LocalBuffer.GetValue(i).ToString();
                        }
                        System.Diagnostics.Debug.WriteLine(DbgText);
#endif
                    }
                    break;

                // Date: 000DDDDD 0000MMMM 0YYYYYYY
                // D = day: 1 .. 31
                // M = Month: 1 .. 12
                // Y = Year: 0 .. 99
                case (int)EIBProtocol.EISDATAFORMAT.EISDFEIS4:
                    {
                        if (Data.Count() != 3)
                        {
                            return;
                        }
                        byte[] LocalBuffer = new byte[4];
                        // Day
                        LocalBuffer[0] = Data[0];
                        // Month
                        LocalBuffer[1] = Data[1];
                        // Year
                        UInt16 AuxUShort = Data[2];
                        if (AuxUShort < 90)
                        {
                            AuxUShort += 100;
                        }
                        AuxUShort += 1900;
                        byte[] BufferUInt16 = BitConverter.GetBytes(AuxUShort);
                        LocalBuffer[2] = BufferUInt16[0];
                        LocalBuffer[3] = BufferUInt16[1];

                        TagsList[0].SetTagValue(ref LocalBuffer, 0);
                        changed.Add(TagsList[0]);
                        if (PollingEnabled && EnableOnlyInitialPolling)
                        {
                            PollingEnabled = false;
                        }
                        if (Status == EibCommJobStatus.ReadRequestPending)
                        {
                            Status = EibCommJobStatus.Idle;
                        }
                        LastExecutionTime = DateTime.UtcNow;
                        LastReadRequestTimeStamp = LastExecutionTime;
#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgText =
                        String.Format(
                        "EIB DBG - SJobDa - {0} Pol Add: {1} - In Add: {2} - Da (EISDFEIS4):",
                        dbgTime, PollingGroup, InputGroups);
                        for (int i = 0; i < LocalBuffer.GetLength(0); i++)
                        {
                            DbgText += " ";
                            DbgText += LocalBuffer.GetValue(i).ToString();
                        }
                        System.Diagnostics.Debug.WriteLine(DbgText);
#endif
                    }
                    break;

                // Value: SEEEEMMM MMMMMMMM
                // S = Sign, E = exponent basis 2, M = mantissa
                case (int)EIBProtocol.EISDATAFORMAT.EISDFEIS5:
                    {
                        if (Data.Count() != 2)
                        {
                            return;
                        }

                        // Sign
                        byte AuxByte = (byte)(Data[0] & 0x80);
                        double Sign = 1.0;
                        if (AuxByte != 0)
                        {
                            Sign = -1.0;
                        }

                        // Exponent
                        AuxByte = (byte)(Data[0] & 0x78);
                        AuxByte >>= 3;
                        ushort Exponent = 1;
                        Exponent <<= AuxByte;

                        // Mantissa
                        AuxByte = (byte)(Data[0] & 0x07);
                        if (Sign < 0.0)
                        {
                            AuxByte |= 0x08;
                        }
                        ushort Mantissa = AuxByte;
                        Mantissa <<= 8;
                        Mantissa += Data[1];
                        if (Sign < 0.0)
                        {
                            // 2-complement
                            Mantissa = (ushort)(~Mantissa);
                            Mantissa &= 0x0FFF;
                            Mantissa += 1;
                        }

                        // Value calculation
                        double AuxDouble = Sign * 0.01 * (double)Mantissa * (double)Exponent;

                        byte[] LocalBuffer;
                        if (TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Float)
                        {
                           LocalBuffer = BitConverter.GetBytes((float)AuxDouble);
                        }
                        else
                        {
                            LocalBuffer = BitConverter.GetBytes(AuxDouble);
                        }


                        TagsList[0].SetTagValue(ref LocalBuffer, 0);
                        changed.Add(TagsList[0]);
                        if (PollingEnabled && EnableOnlyInitialPolling)
                        {
                            PollingEnabled = false;
                        }
                        if (Status == EibCommJobStatus.ReadRequestPending)
                        {
                            Status = EibCommJobStatus.Idle;
                        }
                        LastExecutionTime = DateTime.UtcNow;
                        LastReadRequestTimeStamp = LastExecutionTime;
#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgText =
                        String.Format(
                        "EIB DBG - SetJDa - {0} PolAdd: {1} - InAdd: {2} - Da (EISDFEIS5):",
                        dbgTime, PollingGroup, InputGroups);
                        for (int i = 0; i < LocalBuffer.GetLength(0); i++)
                        {
                            DbgText += " ";
                            DbgText += LocalBuffer.GetValue(i).ToString();
                        }
                        System.Diagnostics.Debug.WriteLine(DbgText);
#endif
                    }
                    break;

                // Scaling
                case (int)EIBProtocol.EISDATAFORMAT.EISDFEIS6:
                    {
                        UInt16 AuxUShort = Data[0];
                        AuxUShort *= 100;
                        AuxUShort += 49;
                        AuxUShort /= 255;
                        byte[] LocalBuffer = new byte[1];
                        LocalBuffer[0] = (byte)AuxUShort;

                        TagsList[0].SetTagValue(ref LocalBuffer, 0);
                        changed.Add(TagsList[0]);
                        if (PollingEnabled && EnableOnlyInitialPolling)
                        {
                            PollingEnabled = false;
                        }
                        if (Status == EibCommJobStatus.ReadRequestPending)
                        {
                            Status = EibCommJobStatus.Idle;
                        }
                        LastExecutionTime = DateTime.UtcNow;
                        LastReadRequestTimeStamp = LastExecutionTime;
#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgText =
                        String.Format(
                        "EIB DBG - SetJDa - {0} PolAdd: {1} - InAdd: {2} - Da (EISDFEIS6) =",
                        dbgTime, PollingGroup, InputGroups);
                        for (int i = 0; i < LocalBuffer.GetLength(0); i++)
                        {
                            DbgText += " ";
                            DbgText += LocalBuffer.GetValue(i).ToString();
                        }
                        System.Diagnostics.Debug.WriteLine(DbgText);
#endif
                    }
                    break;

                case (int)EIBProtocol.EISDATAFORMAT.EISDFAccessPWD6Bytes:
                    {
                        if (Data.Count() != 6)
                        {
                            return;
                        }
                        TagsList[0].SetTagValue(ref Data, 0);
                        changed.Add(TagsList[0]);
                       if (PollingEnabled && EnableOnlyInitialPolling)
                        {
                            PollingEnabled = false;
                        }
                        if (Status == EibCommJobStatus.ReadRequestPending)
                        {
                            Status = EibCommJobStatus.Idle;
                        }
                        LastExecutionTime = DateTime.UtcNow;
                        LastReadRequestTimeStamp = LastExecutionTime;
#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgText =
                        String.Format(
                        "EIB DBG - SetJDa - {0} PolAdd: {1} - InAdd: {2} - Da (EISDFAccessPWD6Bytes):",
                        dbgTime, PollingGroup, InputGroups);
                        for (int i = 0; i < Data.GetLength(0); i++)
                        {
                            DbgText += " ";
                            DbgText += Data.GetValue(i).ToString();
                        }
                        System.Diagnostics.Debug.WriteLine(DbgText);
#endif
                    }
                    break;

                case (int)EIBProtocol.EISDATAFORMAT.EISDFAccessPWD10Bytes:
                    {
                        if (Data.Count() != 10)
                        {
                            return;
                        }
                        TagsList[0].SetTagValue(ref Data, 0);
                        changed.Add(TagsList[0]);
                        if (PollingEnabled && EnableOnlyInitialPolling)
                        {
                            PollingEnabled = false;
                        }
                        if (Status == EibCommJobStatus.ReadRequestPending)
                        {
                            Status = EibCommJobStatus.Idle;
                        }
                        LastExecutionTime = DateTime.UtcNow;
                        LastReadRequestTimeStamp = LastExecutionTime;
#if DEBUG
                        String dbgTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        String DbgText =
                        String.Format(
                        "EIB DBG - SetJDa - {0} PolAdd: {1} - InAdd: {2} - Da (EISDFAccessPWD10Bytes) =",
                        dbgTime, PollingGroup, InputGroups);
                        for (int i = 0; i < Data.GetLength(0); i++)
                        {
                            DbgText += " ";
                            DbgText += Data.GetValue(i).ToString();
                        }
                        System.Diagnostics.Debug.WriteLine(DbgText);
#endif
                    }
                    break;
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
                    nType == (uint)BuiltInType.UInteger
                    )
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return false;
        }

        private void CheckJobValid()
        {
            foreach (var d in TagsList)
            {
                if (!IsTypeAdmitted(d.TagNode.DataType) && d.DynSettings.MethodID == -1)
                {
                    IsValid = false;
                    InvalidReason = string.Format(
                                    Properties.Resources.ErrorInvalidTagType,
                                    d.TagNode.NodeId.ToString(),
                                    d.TagNode.DataType.Identifier.ToString());
                    return;
                }
            }

            // Input task --> At least a valid input address or a valid polling address must be assigned
            if ((Type == LinkType.Input) || (Type == LinkType.InputOutput))
            {
                if((PollingGroup == String.Empty) && (InputGroups == String.Empty))
                {
                    IsValid = false;
                    InvalidReason =
                        string.Format(
                               Properties.Resources.ErrorInvalidTaskNoPollingInputAddress,
                               TagsList[0].TagNode.DynamicSettings);
                    System.Diagnostics.Debug.WriteLine(InvalidReason);
                    return;
                }
                if(PollingGroup != String.Empty)
                {
                    if(!EIBCommJob.IsValidAddress(PollingGroup))
                    {
                        IsValid = false;
                        InvalidReason =
                            string.Format(
                                   Properties.Resources.ErrorInvalidTaskInvalidPollingAddress,
                                   TagsList[0].TagNode.DynamicSettings);
                        System.Diagnostics.Debug.WriteLine(InvalidReason);
                        return;
                    }
                }
                if (InputGroups != String.Empty)
                {
                    if (!EIBCommJob.IsValidListAddress(InputGroups))
                    {
                        IsValid = false;
                        InvalidReason =
                            string.Format(
                                   Properties.Resources.ErrorInvalidTaskInvalidInputAddress,
                                   TagsList[0].TagNode.DynamicSettings);
                        System.Diagnostics.Debug.WriteLine(InvalidReason);
                        return;
                    }
                }

                // Input/Output task --> At least a valid output address must
                // be assigned
                if (Type == LinkType.InputOutput)
                {
                    if (OutputGroup == String.Empty)
                    {
                        IsValid = false;
                        InvalidReason =
                            string.Format(
                                   Properties.Resources.ErrorInvalidTaskNoOutputAddress,
                                   TagsList[0].TagNode.DynamicSettings);
                        System.Diagnostics.Debug.WriteLine(InvalidReason);
                        return;
                    }
                    if (!EIBCommJob.IsValidAddress(OutputGroup))
                    {
                        IsValid = false;
                        InvalidReason =
                            string.Format(
                                   Properties.Resources.ErrorInvalidTaskInvalidOutputAddress,
                                   TagsList[0].TagNode.DynamicSettings);
                        System.Diagnostics.Debug.WriteLine(InvalidReason);
                        return;
                    }
                }
            }
            // Output task --> At least a valid output address must be assigned
            else
            {
                if (OutputGroup == String.Empty)
                {
                    IsValid = false;
                    InvalidReason =
                        string.Format(
                               Properties.Resources.ErrorInvalidTaskNoOutputAddress,
                               TagsList[0].TagNode.DynamicSettings);
                    System.Diagnostics.Debug.WriteLine(InvalidReason);
                    return;
                }
                if (!EIBCommJob.IsValidAddress(OutputGroup))
                {
                    IsValid = false;
                    InvalidReason =
                        string.Format(
                               Properties.Resources.ErrorInvalidTaskInvalidOutputAddress,
                               TagsList[0].TagNode.DynamicSettings);
                    System.Diagnostics.Debug.WriteLine(InvalidReason);
                    return;
                }
            }

            IsValid = true;
            InvalidReason = string.Empty;
        }        

        public bool MustSendARequest(out uint delay)
        {
            delay = 0;
            bool returnValue = false;

            if (PollingCanBePerformed(out delay) && EIBCommJob.IsValidAddress(PollingGroup))
            {
                if(PollingIsForced())
                {
                    returnValue = true;
                }
                else
                {                    
                    double dtime = (double)PollingTime - (DateTime.UtcNow - LastReadRequestTimeStamp).TotalMilliseconds ;
                    if (dtime > 0)
                    {
                        System.Diagnostics.Debug.WriteLine(string.Format("@@@Fra Gr {0} dtime: {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), dtime));
                        delay = (uint)dtime;
                        returnValue = true;
                    }
                    else
                    {
                        returnValue = true;
                    }
                }
            }
            else if (WriteCanBePerformed() && EIBCommJob.IsValidAddress(OutputGroup))
            {
                if(WriteIsForced())
                {
                    returnValue = true;
                    if(GetTagListOnWritingCount() == 0)
                    {
                        lock (lockListObject)
                        {
                            foreach(var tag in TagsList)
                            {
                                TagsListOnWriting.Add(tag);
                            }
                        }
                    }
                }
                else if (GetTagListOnWritingCount() > 0)
                {
                    returnValue = true;
                }
            }

            //String szNow = DateTime.Now.ToString("HH:mm:ss.fff");
            //String DbgText = String.Format("EIB DBG - MSAReq ret {0} - {1} - Pol Gr {2} - Out Gr {3}",
            //                               returnValue.ToString(), szNow, PollingGroup, OutputGroup);
            //System.Diagnostics.Debug.WriteLine(DbgText);

            return returnValue;
        }

        public bool ParseListOfGroupAddresses(string Addresses, List<int> ListOfAddresses)
        {
            ListOfAddresses.Clear();
            if(!IsValidListAddress(Addresses))
            {
                return(false);
            }

            // Extract the single addresses from the list
            string[] ListAddressSplit = Addresses.Split(new Char[] { ';' });
            int AddressCount = ListAddressSplit.Count();

            // Convert each address
            int i;
            for (i = 0; i < AddressCount; i++)
            {
                int AddressIntValue = (int)EibGroupAddressToUInt16(ListAddressSplit[i]);
                ListOfAddresses.Add(AddressIntValue);
            }

            return(true);
        }

        public bool WriteIsForced()
        {
            if (conditionalVariableHasBeenSet == false)
            {
                return (false);
            }

            if (Type == LinkType.Input)
            {
                return (false);
            }

            bool forceWriteBit = false;
            if (GetConditionalVariableBit(ref forceWriteBit, (ushort)EIBConditionalVariableBits.BitForceWrite) == false)
            {
                return (false);
            }
            if (forceWriteBit == true)
            {
                return (true);
            }

            return (false);
        }

        public bool PollingIsForced()
        {
            if (conditionalVariableHasBeenSet == false)
            {
                return (false);
            }

            if ((Type != LinkType.Input) && (Type != LinkType.InputOutput))
            {
                return (false);
            }

            bool forceReadBit = false;
            if (GetConditionalVariableBit(ref forceReadBit, (ushort)EIBConditionalVariableBits.BitForceRead) == false)
            {
                return (false);
            }
            if(forceReadBit == true)
            {
                return (true);
            }

            return (false);
        }

        public bool PollingCanBePerformed()
        {
            return PollingCanBePerformed(false, out uint nextExecutionTime);
        }

        public bool PollingCanBePerformed(out uint delay)
        {
            return PollingCanBePerformed(false, out delay);
        }

        private bool PollingCanBePerformed(bool fromConditionalVar, out uint delay)
        {
            bool returnValue = false;

            delay = 0;
            if (fromConditionalVar)
            {   
                if (PollingIsForced() && GetTagListOnWritingCount() == 0)
                {
                    if (((PollingEnabled == true) && !PollingOnlyOnRequest) ||
                       ((conditionalVariableHasBeenSet == true) && (PollingIsForced() == true)))
                    {
                        returnValue = true;
                    }
                }
            }
            else
            {
                // standard loop
                if ((Type == LinkType.Input) ||
                    ((Type == LinkType.InputOutput) &&
                        (GetTagListOnWritingCount() == 0) && !WriteIsForced()))
                {
                    if (((PollingEnabled == true) && !PollingOnlyOnRequest) ||
                       ((conditionalVariableHasBeenSet == true) && (PollingIsForced() == true)))
                    {
                        returnValue = true;
                    }
                    else
                    {
                        if (!PollingEnabled || PollingOnlyOnRequest)
                            delay = CommJob.JOB_NOT_SCHEDULABLE;
                    }
                }
            }

            return (returnValue);
        }

        public bool WriteCanBePerformed()
        {
            return WriteCanBePerformed(false);
        }

        private bool WriteCanBePerformed(bool fromConditionalVar)
        {
            bool returnValue = false;

            if (fromConditionalVar)
            {
                // from conditional var check bit number 3 and output type
                if (WriteIsForced())
                {
                    if ((Type != LinkType.Input) && ((!OutputOnlyOnRequest && GetTagListOnWritingCount() == 0)))
                        returnValue = true;
                }
            }
            else
            {
                // standard loop
                if ((Type != LinkType.Input) &&
                    ((!OutputOnlyOnRequest && (GetTagListOnWritingCount() > 0)) ||
                     WriteIsForced()))
                {
                    returnValue = true;
                }
            }

            return (returnValue);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Get a bit of the conditional variable of the job. </summary>
        ///
        /// <param name="bitValue" type="ref bool">   The bit value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool GetConditionalVariableBit(ref bool bitValue, UInt16 bitIndex)
        {
            if (conditionalVariableHasBeenSet == false)
            {
                return (false);
            }
            if (bitIndex > 31)
            {
                return (false);
            }

            //Check up if Conditional variable was inizialized
            if ((jobConditionalVariable.varValue == null) || (jobConditionalVariable.varValue.Value == null))
            {
                return (false);
            }

            object value = jobConditionalVariable.varValue.Value;

            if (value is Array)
            {
                return (false);
            }
            Type systemType = value.GetType();
            BuiltInType builtInType = Station.GetBuiltInType(systemType);
            uint uintValue = 0;
            switch (builtInType)
            {
                case BuiltInType.Boolean:
                    {
                        if (bitIndex > 0)
                        {
                            return (false);
                        }

                        bool boolValue = (bool)value;
                        if (boolValue == true)
                        {
                            uintValue = 1;
                        }
                    }
                    break;

                case BuiltInType.SByte:
                    {
                        if (bitIndex > 7)
                        {
                            return (false);
                        }

                        sbyte sbyteValue = (sbyte)value;
                        uintValue = (uint)sbyteValue;
                    }
                    break;

                case BuiltInType.Byte:
                    {
                        if (bitIndex > 7)
                        {
                            return (false);
                        }

                        byte byteValue = (byte)value;
                        uintValue = (uint)byteValue;
                    }
                    break;

                case BuiltInType.Int16:
                    {
                        if (bitIndex > 15)
                        {
                            return (false);
                        }

                        short shortValue = (short)value;
                        uintValue = (uint)shortValue;
                    }
                    break;

                case BuiltInType.UInt16:
                    {
                        if (bitIndex > 15)
                        {
                            return (false);
                        }

                        ushort ushortValue = (ushort)value;
                        uintValue = (uint)ushortValue;
                    }
                    break;

                case BuiltInType.Int32:
                    {
                        if (bitIndex > 31)
                        {
                            return (false);
                        }

                        int intValue = (int)value;
                        uintValue = (uint)intValue;
                    }
                    break;

                case BuiltInType.UInt32:
                    if (bitIndex > 31)
                    {
                        return (false);
                    }

                    uintValue = (uint)value;
                    break;

                case BuiltInType.Float:
                    if (bitIndex > 31)
                    {
                        float FloatValue = (float)value;
                        uintValue = (uint)FloatValue;
                    }
                    break;

                case BuiltInType.Int64:
                    if (bitIndex > 64)
                    {
                        Int64 Int64Value = (Int64)value;
                        uintValue = (uint)Int64Value;
                    }
                    break;

                case BuiltInType.UInt64:
                    if (bitIndex > 64)
                    {
                        UInt64 UInt64Value = (UInt64)value;
                        uintValue = (uint)UInt64Value;
                    }
                    break;

                case BuiltInType.Double:
                    if (bitIndex > 64)
                    {
                        Double DoubleValue = (Double)value;
                        uintValue = (uint)DoubleValue;
                    }
                    break;

                case BuiltInType.String:
                    {
                        string st = value.ToString();
                        if (!string.IsNullOrWhiteSpace(st))
                        {
                            if (!uint.TryParse(st, out uintValue))
                            {
                                return (false);
                            }
                        }
                    }
                    break;

                default:
                    return (false);
            }

            uint bitMask = (uint)Math.Pow(2, bitIndex);
            bitValue = false;
            if ((uintValue & bitMask) != 0)
            {
                bitValue = true;
            }

            return (true);
        }

        public void AutoResetNewDataFlag()
        {
            if (((Type == LinkType.Input) || (Type == LinkType.InputOutput)) &&
                (conditionalVariableHasBeenSet == true) &&
                (AutoResetNewDataTime > 0))
            {
                double dtime = (DateTime.UtcNow - LastExecutionTime).TotalMilliseconds;
                if(dtime > AutoResetNewDataTime)
                {
                    SetConditionalVariableBit(false, (ushort)EIBConditionalVariableBits.BitNewData);
                }
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Set a bit of the conditional variable of the job. </summary>
        ///
        /// <param name="bitValue" type="bool">   The bit new value. </param>
        /// <param name="bitIndex" type="UInt16">     The index of the bit. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool SetConditionalVariableBit(bool bitValue, UInt16 bitIndex)
        {
            if (conditionalVariableHasBeenSet == false)
            {
                return (false);
            }
            bool bitCurrentValue = false;
            if (GetConditionalVariableBit(ref bitCurrentValue, bitIndex) == false)
            {
                return (false);
            }
            if (bitCurrentValue == bitValue)
            {
                return (true);
            }

            //Check up if Conditional variable was inizialized
            if ((jobConditionalVariable.varValue == null) || (jobConditionalVariable.varValue.Value == null))
            {
                return (false);
            }

            object currentValue = jobConditionalVariable.varValue.Value;
            
            if (currentValue is Array)
            {
                return (false);
            }

            Type systemType = currentValue.GetType();
            BuiltInType builtInType = Station.GetBuiltInType(systemType);
            uint uintValue = 0;
            switch (builtInType)
            {
                case BuiltInType.Boolean:
                    {
                        if (bitIndex > 0)
                        {
                            return (false);
                        }

                        bool boolValue = (bool)currentValue;
                        if (boolValue == true)
                        {
                            uintValue = 1;
                        }
                    }
                    break;

                case BuiltInType.SByte:
                    {
                        if (bitIndex > 7)
                        {
                            return (false);
                        }

                        sbyte sbyteValue = (sbyte)currentValue;
                        uintValue = (uint)sbyteValue;
                    }
                    break;

                case BuiltInType.Byte:
                    {
                        if (bitIndex > 7)
                        {
                            return (false);
                        }

                        byte byteValue = (byte)currentValue;
                        uintValue = (uint)byteValue;
                    }
                    break;

                case BuiltInType.Int16:
                    {
                        if (bitIndex > 15)
                        {
                            return (false);
                        }

                        short shortValue = (short)currentValue;
                        uintValue = (uint)shortValue;
                    }
                    break;

                case BuiltInType.UInt16:
                    {
                        if (bitIndex > 15)
                        {
                            return (false);
                        }

                        ushort ushortValue = (ushort)currentValue;
                        uintValue = (uint)ushortValue;
                    }
                    break;

                case BuiltInType.Int32:
                    {
                        if (bitIndex > 31)
                        {
                            return (false);
                        }

                        int intValue = (int)currentValue;
                        uintValue = (uint)intValue;
                    }
                    break;

                case BuiltInType.UInt32:
                    if (bitIndex > 31)
                    {
                        return (false);
                    }

                    uintValue = (uint)currentValue;
                    break;

                case BuiltInType.Float:
                    if (bitIndex > 31)
                    {
                        return (false);
                    }
                    float FloatValue = (float)currentValue;
                    uintValue = (uint)FloatValue;                    
                    break;

                case BuiltInType.Int64:
                    if (bitIndex > 64)
                    {
                        return (false);
                    }
                    Int64 Int64Value = (Int64)currentValue;
                    uintValue = (uint)Int64Value;                    
                    break;

                case BuiltInType.UInt64:
                    if (bitIndex > 64)
                    {
                        return (false);
                    }
                    UInt64 UInt64Value = (UInt64)currentValue;
                    uintValue = (uint)UInt64Value;
                    break;

                case BuiltInType.Double:
                    if (bitIndex > 64)
                    {
                        return (false);
                    }
                    Double DoubleValue = (Double)currentValue;
                    uintValue = (uint)DoubleValue;
                    break;

                case BuiltInType.String:
                    {
                        string st = currentValue.ToString();
                        if (!string.IsNullOrWhiteSpace(st))
                        {
                            if (!uint.TryParse(st, out uintValue))
                            {
                                return (false);
                            }
                        }
                    }
                    break;

                default:
                    return (false);
            }

            uint uintNewValue = uintValue;
            uint bitMask = (uint)Math.Pow(2, bitIndex);
            if (bitValue == false)
            {
                uintNewValue &= ~bitMask;
            }
            else
            {
                uintNewValue |= bitMask;
            }

            //jobConditionalVariable.varValue = null;
            jobConditionalVariable.varValue = new DataValue(new Variant(uintNewValue));

            Station.GetCommDriver().OnTagChanged(jobConditionalVariable.varNodeId, jobConditionalVariable.varValue);

            return (true);
        }

        public void ResetInternalState()
        {
            if (this.EnablePolling && this.PollingTime == 0)
            {
                if (!this.PollingEnabled)
                    this.PollingEnabled = true;
            }
            // Deactivate the job
            if (this.Status == EibCommJobStatus.ReadRequestPending || this.Status == EibCommJobStatus.WriteRequestPending)
                this.Status = EibCommJobStatus.Idle;
        }

        public override bool IsJobAggregable()
        {
            return false;
        }
        #endregion

        #region Static Methods

        public static bool IsValidAddress(string Address)
        {
            // At least 5 characters
            if (Address.Length < 5)
            {
                return false;
            }

            // Check address format: x/x/x 
            string[] AddressSplit = Address.Split(new Char[] { '/' });
            if (AddressSplit.Count() != 3)
            {
                return false;
            }

            // Check values of the three elements of the address
            try
            {
                int AddressMainPart = int.Parse(AddressSplit[0]);
                int AddressMiddlePart = int.Parse(AddressSplit[1]);
                int AddressSubPart = int.Parse(AddressSplit[2]);
                if ((AddressMainPart < 0) || (AddressMiddlePart < 0) ||
                   (AddressSubPart < 0))
                {
                    return false;
                }
                if ((AddressMainPart > 31) || (AddressMiddlePart > 7) ||
                   (AddressSubPart > 255))
                {
                    return false;
                }
                if ((AddressMainPart == 0) && (AddressMiddlePart == 0) &&
                   (AddressSubPart == 0))
                {
                    return false;
                }
            }
            catch(Exception ex)
            {
                return false;
            }
            

            return true;
        }

        public static bool IsValidListAddress(string ListAddress)
        {
            // At least 5 characters
            if (ListAddress.Length < 5)
            {
                return false;
            }

            // Extract the single addresses from the list
            string[] ListAddressSplit = ListAddress.Split(new Char[] { ';' });

            // At least one address
            int AddressCount = ListAddressSplit.Count();
            if (AddressCount < 1)
            {
                return false;
            }

            // Check each address
            int i;
            for(i=0; i<AddressCount; i++)
            {
                if(!EIBCommJob.IsValidAddress(ListAddressSplit[i]))
                {
                    return (false);
                }
            }

            return (true);
        }

        public static UInt16 EibGroupAddressToUInt16(string Address)
        {
            if (!IsValidAddress(Address))
            {
                return (0);
            }

            string[] AddressSplit = Address.Split(new Char[] { '/' });
            UInt16 AddressMainPart = UInt16.Parse(AddressSplit[0]);
            UInt16 AddressMiddlePart = UInt16.Parse(AddressSplit[1]);
            UInt16 AddressSubPart = UInt16.Parse(AddressSplit[2]);

            UInt16 ReturnValue = (UInt16) (AddressMainPart << 11);
            ReturnValue += (UInt16)(AddressMiddlePart << 8);
            ReturnValue += AddressSubPart;
            return ReturnValue;
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

        #region Properties

        /// <summary>
        /// Enable Polling
        /// </summary>
        private bool _EnablePolling;
        public bool EnablePolling
        {
            get
            {
                return _EnablePolling;
            }

            set
            {
                _EnablePolling = value;
            }
        }

        /// <summary>
        /// Enable Only Initial Polling
        /// </summary>
        private bool _EnableOnlyInitialPolling;
        public bool EnableOnlyInitialPolling
        {
            get
            {
                return _EnableOnlyInitialPolling;
            }

            set
            {
                _EnableOnlyInitialPolling = value;
            }
        }

        /// <summary>
        /// Retry Initial Polling in Case of Error
        /// </summary>
        private bool _RetryInitialPolling;
        public bool RetryInitialPolling
        {
            get
            {
                return _RetryInitialPolling;
            }

            set
            {
               _RetryInitialPolling = value;
            }
        }

        /// <summary>
        /// Polling Only On Request
        /// </summary>
        private bool _PollingOnlyOnRequest;
        public bool PollingOnlyOnRequest
        {
            get
            {
                return _PollingOnlyOnRequest;
            }

            set
            {
                _PollingOnlyOnRequest = value;
            }
        }

        /// <summary>
        /// Output Only On Request
        /// </summary>
        private bool _OutputOnlyOnRequest;
        public bool OutputOnlyOnRequest
        {
            get
            {
                return _OutputOnlyOnRequest;
            }

            set
            {
                _OutputOnlyOnRequest = value;
            }
        }

        /// <summary>
        /// Auto Reset "New Data" Notification Time
        /// </summary>
        private uint _AutoResetNewDataTime;
        public uint AutoResetNewDataTime
        {
            get
            {
                return _AutoResetNewDataTime;
            }

            set
            {
                _AutoResetNewDataTime = value;
            }
        }

        /// <summary>
        /// Retry Output in Case of Error
        /// </summary>
        private bool _RetryOutput;
        public bool RetryOutput
        {
            get
            {
                return _RetryOutput;
            }

            set
            {
                _RetryOutput = value;
            }
        }

        /// <summary>
        /// Input Group List
        /// </summary>
        private string _InputGroups;
        public string InputGroups
        {
            get
            {
                return _InputGroups;
            }

            set
            {
               _InputGroups = value;
            }
        }

        /// <summary>
        /// Polling Group
        /// </summary>
        private string _PollingGroup;
        public string PollingGroup
        {
            get
            {
                return _PollingGroup;
            }

            set
            {
                _PollingGroup = value;
            }
        }

        /// <summary>
        /// Output Group
        /// </summary>
        private string _OutputGroup;
        public string OutputGroup
        {
            get
            {
                return _OutputGroup;
            }

            set
            {
                _OutputGroup = value;
            }
        }

        /// <summary>
        /// Data Format EIS
        /// </summary>
        private /*EISDATAFORMAT*/int _DataFormat;
        public /*EISDATAFORMAT*/int DataFormat
        {
            get
            {
                return _DataFormat;
            }

            set
            {
                _DataFormat = value;
            }
        }

        /// <summary>
        /// PollingEnabled
        /// </summary>
        private bool _PollingEnabled;
        public bool PollingEnabled
        {
            get
            {
                return _PollingEnabled;
            }

            set
            {
                _PollingEnabled = value;
            }
        }

        /// <summary>
        /// PollingGroupIntValue
        /// </summary>
        private int _PollingGroupIntValue;
        public int PollingGroupIntValue
        {
            get
            {
                return _PollingGroupIntValue;
            }

            set
            {
                _PollingGroupIntValue = value;
            }
        }

        /// <summary>
        /// OutputGroupIntValue
        /// </summary>
        private int _OutputGroupIntValue;
        public int OutputGroupIntValue
        {
            get
            {
                return _OutputGroupIntValue;
            }

            set
            {
                _OutputGroupIntValue = value;
            }
        }

        /// <summary>
        /// Status
        /// </summary>
        private EibCommJobStatus _Status;
        public EibCommJobStatus Status
        {
            get
            {
                return _Status;
            }

            set
            {
                _Status = value;
            }
        }

        /// <summary>
        /// ErrorCode
        /// </summary>
        private EIBProtocol.EIB_ERROR_CODES _ErrorCode;
        public EIBProtocol.EIB_ERROR_CODES ErrorCode
        {
            get
            {
                return _ErrorCode;
            }

            set
            {
                _ErrorCode = value;
            }
        }

        /// <summary>
        /// LastReadRequestTimeStamp
        /// </summary>
        private DateTime _LastReadRequestTimeStamp;
        public DateTime LastReadRequestTimeStamp
        {
            get
            {
                return _LastReadRequestTimeStamp;
            }

            set
            {
                _LastReadRequestTimeStamp = value;
            }
        }

        /// <summary>
        /// Polling Time
        /// </summary>
        private uint _PollingTime;
        public uint PollingTime
        {
            get
            {
                return _PollingTime;
            }

            set
            {
                _PollingTime = value;
            }
        }

        /// <summary>
        /// Mark if job is added to channel internal map for unsollecied data
        /// </summary>
        private bool _AddedToMap = false;
        public bool AddedToMap
        {
            get
            {
                return _AddedToMap;
            }

            set
            {
                _AddedToMap = value;
            }
        }

        public bool UnSolicited { get; set; } = false;

        public override uint SamplingInterval
        {
            get
            {
                uint newSamplingInterval = _SamplingInterval;
                                
                if (((EIBChannel)this.Station.GetChannel()).IsCommunicationInstable())
                {
                    newSamplingInterval = Properties.Settings.Default.ConnectionRepeatDelay;
                }
                else
                {                    
                    if (MustSendARequest(out uint delay))
                    {
                        if (delay != 0)
                            newSamplingInterval = delay;
                    }
                    else
                    {
                        newSamplingInterval = CommJob.JOB_NOT_SCHEDULABLE;
                    }                   
                }
                return newSamplingInterval;
            }
        }
        #endregion
    }
}
