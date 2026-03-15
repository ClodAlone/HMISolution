using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using IpDriverCodeBase;
using System.Threading;

namespace MelsecFXTCP
{
    public enum MelsecFXErrorCodes : int
    {
        ErrorCodeUnknownSubHeader = 1000,
        ErrorCodeMismatchSubheader = 1001,
        ErrorCodeInvalidCommand = 1002,
        ErrorCodeIncorrectDeviceDesignation = 1003,
        ErrorCodeExceedingAddOrNumOfPoints = 1004,
        ErrorCodeWrongHeadDeviceNum = 1005,
        ErrorCodePCNumberError = 1006,
        ErrorCodeModeError = 1007,
        ErrorCodeRemoteError = 1008,
        ErrorCodeAbnormalCode = 1009,
        ErrorCodeMonitoringTimerExceeded = 1010,
        ErrorCodeGenericError = 1011,
        ErrorCodeIncompleteFrame = 1012
    }

    class MelsecFXTCPChannel : TcpChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public MelsecFXTCPChannel(CommunicationDriver commdriver, MelsecFXTCPChannelSettings settings)
            : base(commdriver, settings, false)
        {
        }

        #endregion

        #region Override Methods

        public override void ExecuteJob(CommJob job)
        {
            MelsecFXTCPCommJob mJob = job as MelsecFXTCPCommJob;
            if (mJob == null)
                return;

            if (job.IsPending == true)
            {
                return;
            }

            base.ExecuteJob(job);

            // Prepare the request frame
            uint dim = GetRequestFrameLength(mJob);
            byte[] requestFrame = new byte[dim];
            uint expectedReplyMaxLength = 0;
            uint requestLength = PrepareRequestFrame(mJob, ref requestFrame, ref expectedReplyMaxLength);
            if (requestLength == 0)
            {
                lock (lockThreadObject)
                {
                    RemovePendingJob(job);
                    job.IsPending = false;
                }
                return;
            }

            // Send the request frame
            ReceiveBuffer.Clear();
            if (!DeviceWrite(requestFrame, requestLength))
            {
                return;
            }

            // Start the procedure for receiving the device reply
            if (!BeginDeviceRead((int)expectedReplyMaxLength))
            {
                return;
            }

            job.LastExecutionTime = DateTime.UtcNow;
            System.Diagnostics.Trace.TraceInformation(string.Format(
                                             "{0} ExecuteJob end",
                                             DateTime.Now.ToLongTimeString()));
        }

        public override bool ProcessNewData(CommJob pendingjob)
        {
            System.Diagnostics.Trace.TraceInformation(string.Format(
                                             "ProcessNewData Enter:{0}",
                                             DateTime.Now.ToLongTimeString()));
            if (LastErrorCode != (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
            {

                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                OnJobExecuted(eJob);
                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                Flush();
                return true;
            }

            lock (lockList)
            {
                if (pendingjob == null)
                {
                    ReceiveBuffer.Clear();
                    Flush();
                    return false;
                }

                Int32 minLen = 2;
                if (ReceiveBuffer.Count < minLen)
                {
                    Flush();
                    return false;
                }

                // Check the subheader
                MelsecFXTCPCommJob mJob = pendingjob as MelsecFXTCPCommJob;
                if (mJob == null)
                {
                    Flush();
                    return false;
                }
                byte subHeader = ReceiveBuffer[0];
                if((subHeader < (byte)(0x80 + MelsecFXCommandCode.Command_BatchRead_BitUnits)) ||
                   (subHeader > (byte)(0x80 + MelsecFXCommandCode.Command_BatchWrite_WordUnits)))
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeUnknownSubHeader;
                    ReceiveBuffer.Clear();
                    Flush();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    return true;
                }
                if(subHeader != (byte)(0x80 + mJob.CommandCode))
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeMismatchSubheader;
                    ReceiveBuffer.Clear();
                    Flush();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    return true;
                }

                // Check the complete code
                byte completeCode = ReceiveBuffer[1];
                switch (completeCode)
                {
                    // No error
                    case 0: 
                    break;

                    // Error command/response out of specifications
                    case 0x50:
                    {
                        ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeInvalidCommand;
                        ReceiveBuffer.Clear();
                        Flush();
                        eAJob.Job = pendingjob;
                        OnJobExecuted(eAJob);
                        return true;
                    }

                    // Error device designation incorrect
                    case 0x56:
                    {
                        ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeIncorrectDeviceDesignation;
                        ReceiveBuffer.Clear();
                        Flush();
                        eAJob.Job = pendingjob;
                        OnJobExecuted(eAJob);
                        return true;
                    }

                    // Error exceeding address or number of points
                    case 0x57:
                    {
                        ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeExceedingAddOrNumOfPoints;
                        ReceiveBuffer.Clear();
                        Flush();
                        eAJob.Job = pendingjob;
                        OnJobExecuted(eAJob);
                        return true;
                    }

                    // Error wrong head device number
                    case 0x58:
                    {
                        ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeWrongHeadDeviceNum;
                        ReceiveBuffer.Clear();
                        Flush();
                        eAJob.Job = pendingjob;
                        OnJobExecuted(eAJob);
                        return true;
                    }

                    // Error with abnormal code
                    case 0x5B:
                    {
                        byte abnormalCode = 0;
                        if (ReceiveBuffer.Count > minLen)
                        {
                            abnormalCode = ReceiveBuffer[2];
                        }

                        ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                        switch (abnormalCode)
                        {
                            // Wrong PC number
                            case 0x10:
                                eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodePCNumberError;
                            break;

                            // Mode error
                            case 0x11:
                                eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeModeError;
                            break;

                            // Remote error
                            case 0x18:
                                eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeRemoteError;
                            break;

                            // Unrecognized abnormal code
                            default:
                                eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeAbnormalCode;
                            break;
                        }

                        ReceiveBuffer.Clear();
                        Flush();
                        eAJob.Job = pendingjob;
                        OnJobExecuted(eAJob);
                        return true;
                    }

                    // Error monitoring timer exceeded
                    case 0x60:
                    {
                        ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeMonitoringTimerExceeded;
                        ReceiveBuffer.Clear();
                        Flush();
                        eAJob.Job = pendingjob;
                        OnJobExecuted(eAJob);
                        return true;
                    }

                    // Generic error
                    default:
                    {
                        ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                        eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeGenericError;
                        ReceiveBuffer.Clear();
                        Flush();
                        eAJob.Job = pendingjob;
                        OnJobExecuted(eAJob);
                        return true;
                    }

                }

                int dataLength = 0;
                int totalLength = 2;
                switch (mJob.CommandCode)
                {
                    case (ushort)MelsecFXCommandCode.Command_BatchRead_BitUnits:
                    {
                        dataLength = (int)(mJob.PointCount / 2);
                        if (mJob.PointCount % 2 > 0)
                        {
                            dataLength++;
                        }
                    }
                    break;

                    case (ushort)MelsecFXCommandCode.Command_BatchRead_WordUnits:
                    {
                        dataLength = (int)(mJob.PointCount * 2);
                    }
                    break;

                    case (ushort)MelsecFXCommandCode.Command_BatchWrite_BitUnits:
                    break;
	
                    case (ushort)MelsecFXCommandCode.Command_BatchWrite_WordUnits:
                    break;
                }
                totalLength += dataLength;

                // Incomplete frame?
                if (ReceiveBuffer.Count < totalLength)
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeIncompleteFrame;
                    ReceiveBuffer.Clear();
                    Flush();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    return true;
                }
                if (((mJob.CommandCode ==
                      (ushort)MelsecFXCommandCode.Command_BatchRead_BitUnits) ||
                     (mJob.CommandCode ==
                      (ushort)MelsecFXCommandCode.Command_BatchRead_WordUnits)) &&
                    (dataLength <= 0))
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeIncompleteFrame;
                    ReceiveBuffer.Clear();
                    Flush();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    return true;
                }

                LastErrorMessage = "";
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;

                // Copy received data
                if ((mJob.CommandCode ==
                    (ushort)MelsecFXCommandCode.Command_BatchRead_BitUnits) ||
                    (mJob.CommandCode ==
                    (ushort)MelsecFXCommandCode.Command_BatchRead_WordUnits))
                {
                    byte[] Answer;
                    Answer = new byte[dataLength];
                    ReceiveBuffer.CopyTo(minLen, Answer, 0, dataLength);
                    eJob.Values = Answer;
                }

                ReceiveBuffer.RemoveRange(0, totalLength);
                Flush();
                eJob.Job = pendingjob;
                OnJobExecuted(eJob);

                return true;
            }
        }

        #endregion

        #region Specific Methods

        private uint GetRequestFrameLength(MelsecFXTCPCommJob job)
        {
            // Input request (length always equal to 12)
            if (job.Type == DriverCodeBase.Enumerators.LinkType.Input ||
                (job.Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
                job.TagsListToWrite.Count == 0))
            {
                return 12;
            }

            // Output request (total length = 12 + data length)
            uint DataFrameLength = 0;
            uint StartAddress = (uint)job.AddressObj.StartAddress;
            if (job.BitVariables)
            {
                if ((job.TotalJobSize % 16 > 0) || (StartAddress % 16 > 0))
                {
                    DataFrameLength = (uint)(job.TotalJobSize / 2);
                    if (job.TotalJobSize % 2 > 0)
                    {
                        DataFrameLength++;
                    }
                }
                else
                {
                    DataFrameLength = (job.TotalJobSize/16) * 2;
                }
            }
            else
            {
                DataFrameLength = (job.TotalJobSize / 2) * 2;
                switch (job.AddressObj.DataArea)
                {
                    case MelsecFXDataArea.DataArea_X:
                    case MelsecFXDataArea.DataArea_Y:
                    case MelsecFXDataArea.DataArea_M:
                    case MelsecFXDataArea.DataArea_S:
                    case MelsecFXDataArea.DataArea_L:
                    case MelsecFXDataArea.DataArea_B:
                    case MelsecFXDataArea.DataArea_F:
                    case MelsecFXDataArea.DataArea_TS:
                    case MelsecFXDataArea.DataArea_TC:
                    case MelsecFXDataArea.DataArea_CS:
                    case MelsecFXDataArea.DataArea_CC:
                    case MelsecFXDataArea.DataArea_M_Special:
                    case MelsecFXDataArea.DataArea_CS_16:
                    case MelsecFXDataArea.DataArea_CS_32:
                        //if (job.TotalJobSize == 1)
                        //{
                        //    DataFrameLength = 4;
                        //}
                        DataFrameLength = job.TotalJobSize * 4;
                    break;
                }
            }

            return (12 + DataFrameLength);
        }

        private uint PrepareRequestFrame(MelsecFXTCPCommJob job,
                                         ref byte[] buffer, ref uint expectedReplyMaxLength)
        {
            expectedReplyMaxLength = 0;
            if (job == null || (job.Station as MelsecFXTCPStation == null))
            {
                return 0;
            }

            job.PointCount = 0;
            job.CommandCode = 0;
            uint byteCount = 0;

            if (job.Type == DriverCodeBase.Enumerators.LinkType.Input ||
                (job.Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
                job.TagsListToWrite.Count == 0))
            {
                byteCount = PrepareReadRequestFrame(job, ref buffer);
                expectedReplyMaxLength = 2;
                if (job.CommandCode == (ushort)MelsecFXCommandCode.Command_BatchRead_BitUnits)
                {
                    expectedReplyMaxLength += job.PointCount / 2;
                    if ((job.PointCount % 2) != 0)
                    {
                        expectedReplyMaxLength++;
                    }
                }
                else
                {
                    expectedReplyMaxLength += job.PointCount * 2;
                }
            }
            else
            {
                byteCount = PrepareWriteRequestFrame(job, ref buffer);
                expectedReplyMaxLength = 4;
            }

            if (expectedReplyMaxLength < 4)
            {
                expectedReplyMaxLength = 4;
            }

            return (byteCount);
        }

        private uint PrepareReadRequestFrame(MelsecFXTCPCommJob job,
                                             ref byte[] buffer)
        {
            job.PointCount = job.TotalJobSize;
            job.CommandCode = (ushort)MelsecFXCommandCode.Command_BatchRead_WordUnits;
            if (job.BitVariables)
            {
                if ((job.TotalJobSize % 16 > 0) || (job.AddressObj.StartAddress % 16 > 0))
                {
                    job.CommandCode = (ushort)MelsecFXCommandCode.Command_BatchRead_BitUnits;
                }
                else
                {
                    job.PointCount /= 16;
                }
            }
            else
            {
                job.PointCount /= 2;
                if (job.TotalJobSize % 2 > 0)
                {
                    job.PointCount++;
                }
            }

            uint byteCount = 0;

            // Command Code
            buffer[byteCount++] = (byte)job.CommandCode;
            // PC number
            buffer[byteCount++] = 255;
            // CPU monitoring timer (timeout)
            ushort Aux = (ushort)(Timeout/250);
            buffer[byteCount++] = (byte)(Aux & 0xFF);
            buffer[byteCount++] = (byte)(Aux >> 8);
            // Head device (start address)
            Aux = (ushort)(job.AddressObj.StartAddress);
            buffer[byteCount++] = (byte)(Aux & 0xFF);
            buffer[byteCount++] = (byte)(Aux >> 8);
            Aux = (ushort)(job.AddressObj.StartAddress >> 16);
            buffer[byteCount++] = (byte)(Aux & 0xFF);
            buffer[byteCount++] = (byte)(Aux >> 8);
            // Data area code
            Aux = (ushort)job.AddressObj.DataAreaCode;
            buffer[byteCount++] = (byte)(Aux & 0xFF);
            buffer[byteCount++] = (byte)(Aux >> 8);
            // Number of device points
            buffer[byteCount++] = (byte)(job.PointCount);
            buffer[byteCount++] = 0;

            return (byteCount);
        }

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

        private uint PrepareWriteRequestFrame(MelsecFXTCPCommJob job,
                                              ref byte[] buffer)
        {
            Tag tag;
            object objectData = null;
            lock (job.retLockList())
            {
                if (job.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                {
                    if (job.TagsListToWrite.Count == 0)
                        job.TagsListToWrite.AddRange(job.TagsList);
                }
                if (job.TagsListToWrite.Count == 0)
                {
                    return 0;
                }

                job.GetJobData(ref objectData);
                if (job.TagsListOnWriting.Count == 0)
                    return 0;
                tag = job.TagsListOnWriting[0];
            }
                
            byte[] jobdata = (byte[])objectData;
            UInt16 DataSize = (UInt16)jobdata.Length;

            uint DataFrameLength = 0;
            job.PointCount = DataSize;
            job.CommandCode = (ushort)MelsecFXCommandCode.Command_BatchWrite_WordUnits;

            uint AddressOffset = tag.ByteOffset;

            uint StartAddress = (uint)job.AddressObj.StartAddress;
            if (job.BitVariables && !job.ArrayVariables)
            {
                if ((DataSize % 16 > 0) || (StartAddress % 16 > 0))
                {
                    job.CommandCode = (ushort)MelsecFXCommandCode.Command_BatchWrite_BitUnits;
                    DataFrameLength = (uint)(DataSize / 2);
                    if (DataSize % 2 > 0)
                    {
                        DataFrameLength++;
                    }
                }
                else
                {
                    job.PointCount /= 16;
                    DataFrameLength = job.PointCount * 2;
                }
              
            }
            else if (job.BitVariables && job.ArrayVariables)
            {/*((job.TotalJobSize % 8) == 0) || (*/
                if ((buffer.Length - 11) < (job.TotalJobSize/2))
                {
                    job.CommandCode = (ushort)MelsecFXCommandCode.Command_BatchWrite_WordUnits;
                    job.PointCount = job.TotalJobSize / 16;
                    DataFrameLength = (uint)(DataSize);
                } 
                else
                {
                    job.CommandCode = (ushort)MelsecFXCommandCode.Command_BatchWrite_BitUnits;
                    job.PointCount = job.TotalJobSize;
                    DataFrameLength = (uint)(DataSize);
                }                

            }
            else
            {
                job.PointCount /= 2;
                DataFrameLength = job.PointCount * 2;
                switch (job.AddressObj.DataArea)
                {
                    case MelsecFXDataArea.DataArea_X:
                    case MelsecFXDataArea.DataArea_Y:
                    case MelsecFXDataArea.DataArea_M:
                    case MelsecFXDataArea.DataArea_S:
                    case MelsecFXDataArea.DataArea_L:
                    case MelsecFXDataArea.DataArea_B:
                    case MelsecFXDataArea.DataArea_F:
                    case MelsecFXDataArea.DataArea_TS:
                    case MelsecFXDataArea.DataArea_TC:
                    case MelsecFXDataArea.DataArea_CS:
                    case MelsecFXDataArea.DataArea_CC:
                    case MelsecFXDataArea.DataArea_M_Special:
                    case MelsecFXDataArea.DataArea_CS_16:
                    case MelsecFXDataArea.DataArea_CS_32:
                        if (DataSize == 1)
                        {
                            job.PointCount = 8;
                            DataFrameLength = 4;
                            job.CommandCode = (ushort)MelsecFXCommandCode.Command_BatchWrite_BitUnits;
                        }
                        AddressOffset *= 8;
                    break;

                    case MelsecFXDataArea.DataArea_CN_32:
                        AddressOffset /= 4;
                    break;

                    default:
                        AddressOffset /= 2;
                    break;
                }
            }

            if (DataFrameLength == 0)
            {
                return 0;
            }

            uint byteCount = 0;

            // Command Code
            buffer[byteCount++] = (byte)job.CommandCode;
            // PC number
            buffer[byteCount++] = 255;
            // CPU monitoring timer (timeout)
            ushort Aux = (ushort)(Timeout / 250);
            buffer[byteCount++] = (byte)(Aux & 0xFF);
            buffer[byteCount++] = (byte)(Aux >> 8);
            // Head device (start address)
            StartAddress += AddressOffset;
            Aux = (ushort)StartAddress;
            buffer[byteCount++] = (byte)(Aux & 0xFF);
            buffer[byteCount++] = (byte)(Aux >> 8);
            Aux = (ushort)(StartAddress >> 16);
            buffer[byteCount++] = (byte)(Aux & 0xFF);
            buffer[byteCount++] = (byte)(Aux >> 8);
            // Data area code
            Aux = (ushort)job.AddressObj.DataAreaCode;
            buffer[byteCount++] = (byte)(Aux & 0xFF);
            buffer[byteCount++] = (byte)(Aux >> 8);
            // Number of device points
            buffer[byteCount++] = (byte)(job.PointCount);
            buffer[byteCount++] = 0;
        
            int j = 0;
            int i = 0;           
            byte byteValue = 0;
            if (job.BitVariables)
            {
                // Bit variables written in bit units 
                if (job.CommandCode == (ushort)MelsecFXCommandCode.Command_BatchWrite_BitUnits)
                {
                    if ((job.BitArrayVariables == false) && (job.ArrayVariables == false))
                    {
                        for (i = 0; i < job.PointCount; i += 2)
                        {
                            if (jobdata[i] > 0)
                            {
                                byteValue = 0x10;
                            }
                            else
                            {
                                byteValue = 0;
                            }
                            if ((i + 1) < job.PointCount)
                            {
                                if (jobdata[i + 1] > 0)
                                {
                                    byteValue |= 1;
                                }
                            }
                            buffer[byteCount++] = byteValue;
                        }
                    }
                    else if ((job.BitArrayVariables == false) && (job.ArrayVariables == true))
                    {
                        byte byteMask = 1;
                        for (int indexByte = 0; indexByte < jobdata.Length; indexByte++)
                        {                            
                            for (i = 0; i < 8; i += 2)
                            {
                                
                                if (((indexByte * 8 )+i) >= job.PointCount) 
                                {
                                    break;
                                }
                                byteMask = (byte)(1 << i);
                                if ((jobdata[indexByte] & byteMask) != 0)
                                {
                                    byteValue = 0x10;
                                }
                                else
                                {
                                    byteValue = 0;
                                }
                                
                                byteMask = (byte)(1 << (i + 1));
                                if ((jobdata[indexByte] & byteMask) != 0)
                                {
                                    byteValue |= 1;
                                }
                                 
                                buffer[byteCount++] = byteValue;
                                              
                            }
                        }
                    }
                    else
                    {
                        byte byteMask = 1;
                        for (i = 0, j = 0; i < job.PointCount; i += 2)
                        {
                            byteMask = (byte)(1 << (i % 8));
                            if ((jobdata[j] & byteMask) > 0)
                            {
                                byteValue = 0x10;
                            }
                            else
                            {
                                byteValue = 0;
                            }
                            if ((i + 1) < job.PointCount)
                            {
                                byteMask = (byte)(1 << ((i + 1) % 8));
                                if ((jobdata[j] & byteMask) > 0)
                                {
                                    byteValue |= 1;
                                }
                            }
                            buffer[byteCount++] = byteValue;
                            if ((i % 8) == 6)
                            {
                                j++;
                            }
                        }
                    }
                }

                // Bit variables written in WORD units
                else
                {
                    if ((job.BitArrayVariables == false) && (job.ArrayVariables == false))
                    {
                        for (i = 0; i < job.PointCount; i++)
                        {
                            Aux = 0;
                            for (j = 0; j < 16; j++)
                            {
                                if (jobdata[i * 16 + j] > 0)
                                {
                                    Aux |= (ushort)(1 << j);
                                }
                            }
                            buffer[byteCount++] = (byte)(Aux & 0xFF);
                            buffer[byteCount++] = (byte)(Aux >> 8);
                        }
                    }
                    else
                    {
                        for (i = 0, j = 0; i < job.PointCount; i++, j += 2)
                        {
                            buffer[byteCount++] = jobdata[j];
                            buffer[byteCount++] = jobdata[j + 1];
                        }
                    }
                }
            }
            else
            {
                byte byteAux = 0;
                // Single byte variable written in bit units 
                if (job.CommandCode == (ushort)MelsecFXCommandCode.Command_BatchWrite_BitUnits)
                {
                    for (i = 0; i < job.PointCount; i += 2)
                    {
                        byteAux = jobdata[i / 8];
                        if ((byteAux & (1 << (i % 8))) > 0)
                        {
                            byteValue = 0x10;
                        }
                        else
                        {
                            byteValue = 0;
                        }
                        if ((i + 1) < job.PointCount)
                        {
                            byteAux = jobdata[(i + 1) / 8];
                            if ((byteAux & (1 << ((i + 1) % 8))) > 0)
                            {
                                byteValue |= 1;
                            }
                        }
                        buffer[byteCount++] = byteValue;
                    }
                }

                // WORD variables written in WORD units
                else
                {
                    for (i = 0; i < DataSize; i++)
                    {
                        buffer[byteCount++] = jobdata[i];
                    }
                }
            }
            return (byteCount);
        }
        #endregion
    }
}
