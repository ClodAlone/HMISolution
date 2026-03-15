using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using IpDriverCodeBase;
using System.Threading;
using System.Runtime.InteropServices;

namespace MelsecQEth
{
    public enum MelsecQErrorCodes : int
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
        ErrorCodeIncompleteFrame = 1012,
        ErrorWrongReply = 1013,
        ErrorFromDevice = 2000,
        ErrorOnlineChangeDisabled = ErrorFromDevice + 0x55,
    }

    #region dataTypes

    [StructLayout(LayoutKind.Explicit)]
    public struct ushortUnion
    {
        [FieldOffset(0)]
        public ushort USHORT;

        [FieldOffset(0)]
        public byte LOBYTE;
        [FieldOffset(1)]
        public byte HIBYTE;
        // Constructor:
        public ushortUnion(ushort USHORT)
        {
            this.LOBYTE = 0;
            this.HIBYTE = 0;
            this.USHORT = USHORT;
        }
        public ushortUnion(byte LOBYTE, byte HIBYTE)
        {
            this.USHORT = 0;
            this.LOBYTE = LOBYTE;
            this.HIBYTE = HIBYTE;
        }
        public ushortUnion(byte[] buffer, ushort index)
        {
            this.USHORT = 0;
            this.LOBYTE = buffer[index];
            this.HIBYTE = buffer[index + 1];
        }
        public ushortUnion(List<byte> buffer, ushort index)
        {
            this.USHORT = 0;
            this.LOBYTE = buffer[index];
            this.HIBYTE = buffer[index + 1];
        }

    }

    [StructLayout(LayoutKind.Explicit)]
    public struct uintUnion
    {
        [FieldOffset(0)]
        public uint UINT;

        [FieldOffset(0)]
        public ushortUnion LOUSHORT;
        [FieldOffset(2)]
        public ushortUnion HIUSHORT;

        // Constructor:
        public uintUnion(uint UINT)
        {
            this.LOUSHORT = new ushortUnion(0x0000);
            this.HIUSHORT = new ushortUnion(0x0000);
            this.UINT = UINT;
        }
        public uintUnion(byte LOBYTE_LW, byte HIBYTE_LW, byte LOBYTE_HW, byte HIBYTE_HW)
        {
            this.UINT = 0;
            this.LOUSHORT = new ushortUnion(LOBYTE_LW, HIBYTE_LW);
            this.HIUSHORT = new ushortUnion(LOBYTE_HW, HIBYTE_HW);
        }
        public uintUnion(byte[] buffer, ushort index)
        {
            this.UINT = 0;
            this.LOUSHORT = new ushortUnion(buffer, index);
            this.HIUSHORT = new ushortUnion(buffer, (ushort)(index + 2));
        }
        public uintUnion(List<byte> buffer, ushort index)
        {
            this.UINT = 0;
            this.LOUSHORT = new ushortUnion(buffer, index);
            this.HIUSHORT = new ushortUnion(buffer, (ushort)(index + 2));
        }

    }

    #endregion
    class MelsecQEthChannel : TcpChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public MelsecQEthChannel(CommunicationDriver commdriver, MelsecQEthChannelSettings settings)
            : base(commdriver, settings, false)
        {
        }

        #endregion

        #region constants
        private const int MESSAGE_HEADER_LENGTH = 9;
        #endregion

        #region Override Methods

        public override void ExecuteJob(CommJob job)
        {
            MelsecQEthCommJob mJob = job as MelsecQEthCommJob;
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
            uint requestLength = PrepareRequestFrame(mJob, ref requestFrame);
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
            if (!BeginDeviceRead(MESSAGE_HEADER_LENGTH))
            {
                return;
            }

            job.LastExecutionTime = DateTime.UtcNow;
            //System.Diagnostics.Trace.TraceInformation(string.Format(
            //                                 "{0} ExecuteJob end",
            //                                 DateTime.Now.ToLongTimeString()));
        }

        protected override void ManageTimeoutError()
        {
            DeviceClose();
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

            Int32 minLen = 2;
            if (ReceiveBuffer.Count < minLen)
            {
                Flush();
                return false;
            }

            // Check the subheader
            MelsecQEthCommJob mJob = pendingjob as MelsecQEthCommJob;
            if (mJob == null)
            {
                Flush();
                return false;
            }

            bool bRet = false;
            ushort nErrCode = 0xffff;
            lock (lockList)
            {
                byte[] pdu;
                if (pendingjob == null)
                {
                    ReceiveBuffer.Clear();
                    Flush();
                    return false;
                }
                if (ReceiveBuffer.Count < MESSAGE_HEADER_LENGTH)
                {
                    return false;
                }

                LastErrorMessage = "";
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;

                ushortUnion dim = new ushortUnion(ReceiveBuffer, MESSAGE_HEADER_LENGTH - 2);
                pdu = new byte[MESSAGE_HEADER_LENGTH + dim.USHORT];
                if (dim.USHORT != 0)
                {
                    byte[] pduData = new byte[dim.USHORT];
                    int BytesToCopy = ReceiveBuffer.Count;
                    if (BytesToCopy > pdu.Length)
                    {
                        BytesToCopy = pdu.Length;
                    }
                    ReceiveBuffer.CopyTo(0,pdu, 0, BytesToCopy);

                    ushort numberOfReceivedBytes = 0;
                    ushort numberOfBytesToBeRead = dim.USHORT;
                    ushort numberOfReadBytes = 0;
                    byte[] readData = new byte[dim.USHORT];
                    do
                    {
                        numberOfReadBytes = (ushort)DeviceReadSynchronous(readData, (uint)numberOfBytesToBeRead);
                        if (numberOfReadBytes > 0)
                        {
                            Array.Copy(readData, 0, pduData, numberOfReceivedBytes, numberOfReadBytes);
                            numberOfReceivedBytes += numberOfReadBytes;
                            numberOfBytesToBeRead -= numberOfReadBytes;
                            if ((numberOfReceivedBytes < (uint)dim.USHORT))
                            {
                                StopWorkerThread.WaitOne(1, false);
                            }
                        }
                    } while ((numberOfReceivedBytes < (uint)dim.USHORT) && (numberOfReadBytes > 0));
                    if (numberOfReceivedBytes == (uint)dim.USHORT)
                    {
                        pduData.CopyTo(pdu, MESSAGE_HEADER_LENGTH);
                        if(!AnswerOk(ref pdu, ref nErrCode))
                        {
                            switch (nErrCode)
                            {
                                // Wrong PC number
                                case 0xffff:
                                    eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecQErrorCodes.ErrorWrongReply;
                                    break;

                                // Unrecognized abnormal code
                                default:
                                    eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)(MelsecQErrorCodes.ErrorFromDevice + nErrCode);
                                    break;
                            }
                        }
                    }
                    else
                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecQErrorCodes.ErrorCodeIncompleteFrame;
                }

                // Copy received data
                if (eJob.ErrorCode == DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError &&  mJob.IsRead)
                {
                    ushort dataLength = 0;
                    switch (mJob.CommandCode)
                    {
                        case (ushort)MelsecQCommandCode.BitUnits:
                            {
                                dataLength = (ushort)(mJob.PointCount / 2);
                                if (mJob.PointCount % 2 > 0)
                                {
                                    dataLength++;
                                }
                            }
                            break;

                        case (ushort)MelsecQCommandCode.WordUnits:
                            {
                                dataLength = (ushort)(mJob.PointCount * 2);
                            }
                            break;

                    }

                    byte[] Answer = GetData(ref pdu, dataLength, mJob);
                    if (Answer.Length == 0)
                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)MelsecQErrorCodes.ErrorCodeIncompleteFrame;
                    else
                        eJob.Values = Answer;
                }

                ReceiveBuffer.Clear();
                Flush();
                eJob.Job = pendingjob;
                OnJobExecuted(eJob);
                return true;

            }
        }

        #endregion

        #region Specific Methods

        private uint GetRequestFrameLength(MelsecQEthCommJob job)
        {
            // Input request (length always equal to 12)
            if (job.IsRead)
            {
                return 21;
            }

            // Output request (total length = 12 + data length)
            return (21 + job.GetDataFrameLength());
        }
  
        private uint PrepareRequestFrame(MelsecQEthCommJob job,
                                         ref byte[] buffer)
        {
            if (job == null || (job.Station as MelsecQEthStation == null))
            {
                return 0;
            }

            job.PointCount = 0;
            job.CommandCode = 0;
            uint byteCount = 0;

            if (job.IsRead)
            {
                byteCount = PrepareReadRequestFrame(job, ref buffer);
            }
            else
            {
                byteCount = PrepareWriteRequestFrame(job, ref buffer);
            }


            return (byteCount);
        }

        private uint PrepareReadRequestFrame(MelsecQEthCommJob job,
                                             ref byte[] buffer)
        {

            // Set the subcommand and the number of device points (bits or words).
            job.PointCount = job.TotalJobSize;
            job.CommandCode = (ushort)MelsecQCommandCode.WordUnits;
            if (job.BitVariables)
            {
                if ((job.TotalJobSize % 16 > 0) || (job.AddressObj.StartAddress % 16 > 0))
                {
                    job.CommandCode = (ushort)MelsecQCommandCode.BitUnits;
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

            buffer[byteCount++] = 0x50; // Subheader (low byte).
            buffer[byteCount++] = 0; // Subheader (high byte).
            buffer[byteCount++] = (byte)(job.GetStationNetworkNumber() & 0xff); // Network num.
            buffer[byteCount++] = (byte)(job.GetStationPcNumber() & 0xff); // PC number.

            job.SetModuleTarget(ref buffer, ref byteCount);

            buffer[byteCount++] = 0x0C; // Request length (low byte).
            buffer[byteCount++] = 0x00; // Request length (high byte).

            // CPU monitoring timer (timeout).
            ushortUnion Aux = new ushortUnion((ushort)(Timeout / 250));
            buffer[byteCount++] = Aux.LOBYTE;
            buffer[byteCount++] = Aux.HIBYTE;

            // Batch read command.
            buffer[byteCount++] = 0x01; // Low byte.
            buffer[byteCount++] = 0x04; // High byte.

            // Subcommand.
            Aux.USHORT = (ushort)(job.CommandCode);
            buffer[byteCount++] = Aux.LOBYTE;
            buffer[byteCount++] = Aux.HIBYTE;

            // Start address (head device).
            uintUnion AuxUint = new uintUnion((uint)job.AddressObj.StartAddress);
            buffer[byteCount++] = AuxUint.LOUSHORT.LOBYTE;
            buffer[byteCount++] = AuxUint.LOUSHORT.HIBYTE;
            buffer[byteCount++] = AuxUint.HIUSHORT.LOBYTE;

            // Data area code
            buffer[byteCount++] = (byte)(job.AddressObj.DataAreaCode);

            // Number of device points.
            Aux.USHORT = (ushort)(job.PointCount);
            buffer[byteCount++] = Aux.LOBYTE;
            buffer[byteCount++] = Aux.HIBYTE;

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

        private uint PrepareWriteRequestFrame(MelsecQEthCommJob job,
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
            uint PointCount = DataSize;
            job.CommandCode = (ushort)MelsecQCommandCode.WordUnits;

            uint AddressOffset = tag.ByteOffset;

            uint StartAddress = (uint)job.AddressObj.StartAddress;
            if (job.BitVariables && !job.ArrayVariables)
            {
                if ((DataSize % 16 > 0) || (StartAddress % 16 > 0))
                {
                    job.CommandCode = (ushort)MelsecQCommandCode.BitUnits;
                    DataFrameLength = (uint)(DataSize / 2);
                    if (DataSize % 2 > 0)
                    {
                        DataFrameLength++;
                    }
                }
                else
                {
                    PointCount /= 16;
                    DataFrameLength = PointCount * 2;
                }
              
            }
            else if (job.BitVariables && job.ArrayVariables)
            {/*((job.TotalJobSize % 8) == 0) || (*/
                if ((buffer.Length - 21) < (job.TotalJobSize/2))
                {
                    job.CommandCode = (ushort)MelsecQCommandCode.WordUnits;
                    PointCount = job.TotalJobSize / 16;
                    DataFrameLength = (uint)(DataSize);
                } 
                else
                {
                    job.CommandCode = (ushort)MelsecQCommandCode.BitUnits;
                    PointCount = job.TotalJobSize;
                    DataFrameLength = (uint)((PointCount + 1)/2);
                }                

            }
            else
            {
                PointCount /= 2;
                DataFrameLength = PointCount * 2;
                switch (job.AddressObj.DataArea)
                {
                    case DataArea.X:
                    case DataArea.Y:
                    case DataArea.S:
                    case DataArea.M:
                    case DataArea.SM:
                    case DataArea.L:
                    case DataArea.B:
                    case DataArea.SB:
                    case DataArea.F:
                    case DataArea.TS:
                    case DataArea.TC:
                    case DataArea.CS:
                    case DataArea.CC:
                        if (DataSize == 1)
                        {
                            PointCount = 8;
                            DataFrameLength = 4;
                            job.CommandCode = (ushort)MelsecQCommandCode.BitUnits;
                        }
                        AddressOffset *= 8;
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


            uint nDevPointsNum = 0;
            ushort nSubCommandCode = 0;
            uint byteCount = 0;

            buffer[byteCount++] = 0x50; // Subheader (low byte).
            buffer[byteCount++] = 0; // Subheader (high byte).
            buffer[byteCount++] = (byte)(job.GetStationNetworkNumber() & 0xff); // Network num.
            buffer[byteCount++] = (byte)(job.GetStationPcNumber() & 0xff); // PC number.

            job.SetModuleTarget(ref buffer, ref byteCount);

            // Request length.
            ushortUnion Aux = new ushortUnion((ushort)(12 + DataFrameLength));
            buffer[byteCount++] = Aux.LOBYTE;
            buffer[byteCount++] = Aux.HIBYTE;

            // CPU monitoring timer (timeout).
            Aux.USHORT = (ushort)(Timeout / 250);
            buffer[byteCount++] = Aux.LOBYTE;
            buffer[byteCount++] = Aux.HIBYTE;

            // Batch write command.
            buffer[byteCount++] = 0x01; // Low byte.
            buffer[byteCount++] = 0x14; // High byte.

            // Subcommand.
            Aux.USHORT = (ushort)(job.CommandCode);
            buffer[byteCount++] = Aux.LOBYTE;
            buffer[byteCount++] = Aux.HIBYTE;

            // Start address (head device).
            uintUnion AuxUint = new uintUnion((uint)(job.AddressObj.StartAddress + AddressOffset));
            buffer[byteCount++] = AuxUint.LOUSHORT.LOBYTE;
            buffer[byteCount++] = AuxUint.LOUSHORT.HIBYTE;
            buffer[byteCount++] = AuxUint.HIUSHORT.LOBYTE;


            // Data area code
            buffer[byteCount++] = (byte)(job.AddressObj.DataAreaCode);

            // Number of device points.
            Aux.USHORT = (ushort)(PointCount);
            buffer[byteCount++] = Aux.LOBYTE;
            buffer[byteCount++] = Aux.HIBYTE;

            // Data bytes.
            int j = 0;
            int i = 0;
            byte byteValue = 0;
            if (job.BitVariables)
            {
                // Bit variables written in bit units 
                if (job.CommandCode == (ushort)MelsecQCommandCode.BitUnits)
                {
                    if (job.ArrayVariables == false)
                    {
                        for (i = 0; i < PointCount; i += 2)
                        {
                            if (jobdata[i] > 0)
                            {
                                byteValue = 0x10;
                            }
                            else
                            {
                                byteValue = 0;
                            }
                            if ((i + 1) < PointCount)
                            {
                                if (jobdata[i + 1] > 0)
                                {
                                    byteValue |= 1;
                                }
                            }
                            buffer[byteCount++] = byteValue;
                        }
                    }
                    else
                    {
                        byte byteMask = 1;
                        for (int indexByte = 0; indexByte < jobdata.Length; indexByte++)
                        {
                            for (i = 0; i < 8; i += 2)
                            {

                                if (((indexByte * 8) + i) >= PointCount)
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
                }

                // Bit variables written in WORD units
                else
                {
                    if (job.ArrayVariables == false)
                    {
                        for (i = 0; i < PointCount; i++)
                        {
                            Aux.USHORT = 0;
                            for (j = 0; j < 16; j++)
                            {
                                if (jobdata[i * 16 + j] > 0)
                                {
                                    Aux.USHORT |= (ushort)(1 << j);
                                }
                            }
                            buffer[byteCount++] = Aux.LOBYTE;
                            buffer[byteCount++] = Aux.HIBYTE;
                        }
                    }
                    else
                    {
                        for (i = 0, j = 0; i < PointCount; i++, j += 2)
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
                if (job.CommandCode == (ushort)MelsecQCommandCode.BitUnits)
                {
                    for (i = 0; i < PointCount; i += 2)
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
                        if ((i + 1) < PointCount)
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
        bool AnswerOk(ref byte[] buffer,ref ushort nErrCode)
        {
            bool nRetValue = false;
            nErrCode = 0;
            if ((buffer.Length < MESSAGE_HEADER_LENGTH + 2) || (buffer[0] != 0xD0) ||
                (buffer[1] != 0))
            {
                nErrCode = 0xffff;
            }
            else
            {
                ushortUnion wErrorCode = new ushortUnion(buffer, MESSAGE_HEADER_LENGTH) ;
                if (wErrorCode.USHORT != 0)
                    nErrCode = wErrorCode.USHORT;
                else
                    nRetValue = true;
            }
            return (nRetValue);
        }
        bool CheckDataLength(ushort nData, int wReplyLength, MelsecQEthCommJob job)
        {
            bool bReturnValue = false;
            ushort nExpectedDataLength = 0;
            ushort nDataBytes = (ushort)(wReplyLength - 11);

            // Bit variables.
            if (job.BitVariables)
            {
                // Bit data read in bit units
                if (nData % 16 != 0)
                {
                    nExpectedDataLength = (ushort)(nData / 2 + nData % 2);
                }

                // Bit data read in WORD units
                else
                {
                    nExpectedDataLength = (ushort)(nData / 8);
                }
            }
            else
            {
                nExpectedDataLength = (ushort)(nData + nData % 2);
            }

            if (nExpectedDataLength == nDataBytes)
            {
                bReturnValue = true;
            }

            return (bReturnValue);
        }
        byte[] GetData(ref byte[] buffer, ushort nData,  MelsecQEthCommJob job)
        {
            if (buffer.Length < 12)
            {
                return new byte[0];
            }

            //System.Diagnostics.Debug.WriteLine("{0} --DEBUG -- GetData buffer.Length: {1}", DateTime.Now.ToString("HH:MM:ss.fff"), buffer.Length);


            ushort nDataBytes = (ushort)(buffer.Length - 11);
            // In case of unexpected reply an array with 0 elements is returned
            byte[] outBuffer = new byte[0];

            //
            // Case bit variables.
            //

            ushort nCopiedData = 0;
            if (job.BitVariables)
            {
                ushort i;

                //
                // Case bit data read in bit units. 
                //

                if (job.CommandCode == (ushort)MelsecQCommandCode.BitUnits)
                {

                    if((job.PointCount/2 + 11) > buffer.Length)
                    {
                        return (outBuffer);
                    }

                    outBuffer = new byte[job.PointCount];
                    for (i = 0; i < job.PointCount ; i++)
                    {
                        if ((i % 2 == 1 ? (buffer[11 + i / 2] & 0x0F) : (buffer[11 + i / 2] & 0xF0)) != 0)
                        {
                            outBuffer[i] = 1;
                        }
                        else
                        {
                            outBuffer[i] = 0;
                        }
                    }
                }

                //
                // Case bit data read in word units. 
                //

                else
                {
                    if ((job.PointCount * 2 + 11) != buffer.Length)
                    {
                        return (outBuffer);
                    }

                    outBuffer = new byte[job.PointCount * 16];
                    for (i = 0; i < outBuffer.Length/8; i++)
                    {
                        outBuffer[i] = buffer[11 + i ];
                    }
                }
            }
            else
            { // if( m_bBitVars )
                outBuffer = new byte[nDataBytes];
                ushort nCopy = nData;
                if (nCopy > nDataBytes)
                {
                    nCopy = nDataBytes;
                }
                Array.Copy(buffer, 11, outBuffer,0, nCopy);
                nCopiedData += nCopy;
            }

            return outBuffer;
        }




        #endregion
    }
}
