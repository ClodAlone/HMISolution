using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using SerialDriverCodeBaseEx;
using System.Threading;
using DriverCodeBaseEx.Enumerators;
using System.Threading.Tasks;

namespace MelsecFX
{
    public enum MelsecFXErrorCodes : int
    {
        ErrorCodeNakReplyToOutputCommand = 1000,
        ErrorCodeNakReplyToInputRequest = 1001,
        ErrorCodeIncompleteFrame = 1002,
        ErrorCodeStx = 1003,
        ErrorCodeEtx = 1004,
        ErrorCodeFcs = 1005
    }


    public class MelsecFXChannel : SerialChannel
    {
        public bool bGoto = false;
        public enum StateMessage
        {
            Ok,
            no_ACK,
            ACK,
            NAK,
            TimeOut,
            Pendingjob_No_Valid
        }

        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public MelsecFXChannel(CommunicationDriver commdriver, MelsecFXChannelSettings settings)
            : base(commdriver, settings)
        {
            bGoto = false;
        }

        #endregion

        #region Override Methods

         public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                // back compatibilty with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }

            MelsecFXCommJob mJob = job as MelsecFXCommJob;
            if (mJob == null)
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return (false);
            }

            base.ExecuteJob(job);

            // Prepare the request frame
            uint dim = GetRequestFrameLength(mJob);
            if (dim == 0)
            {
                RemovePendingJob(job);                
                conn = DriverErrorCodes.ErrorTimeOut;
                return (false);
            }
            byte[] requestFrame = new byte[dim];
            uint requestLength = PrepareRequestFrame(mJob, ref requestFrame);
            if (requestLength == 0)
            {
                RemovePendingJob(job);                
                conn = DriverErrorCodes.ErrorTimeOut;
                return (false);
            }

            // Send the request frame
            ReceiveBuffer.Clear();
            if (!DeviceWrite(requestFrame, requestLength))
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return (false);
            }

            job.LastExecutionTime = DateTime.UtcNow;
            //System.Diagnostics.Trace.TraceInformation(string.Format("{0} ExecuteJob", DateTime.Now.ToLongTimeString()));
            return (true);
        }

        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            //System.Diagnostics.Trace.TraceInformation(string.Format(
            //                                 "ProcessNewData Enter:{0}",
            //                                 DateTime.Now.ToLongTimeString()));

            if (conn != (int)DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError)
            {

                ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                OnJobExecuted(eAJob);
                //conn = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                lock (lockThreadObject)
                    ReceiveBuffer.Clear();
                //System.Diagnostics.Trace.TraceInformation(string.Format("{1} ProcessNewData Error:{0} return point 1",
                //    eAJob.ErrorCode, DateTime.Now.ToLongTimeString()));
                return true;
            }


            if (pendingjob == null)
            {
                lock (lockThreadObject)
                    ReceiveBuffer.Clear();
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} ProcessNewData return point 2", DateTime.Now.ToLongTimeString()));
                return false;
            }
            MelsecFXCommJob mJob = pendingjob as MelsecFXCommJob;
            if (mJob == null)
            {
                lock (lockThreadObject)
                    ReceiveBuffer.Clear();
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} ProcessNewData return point 3", DateTime.Now.ToLongTimeString()));
                return false;
            }

            StateMessage state = WaitCompleteMessage(pendingjob);
            //lock (lockThreadObject)
            //{                           
                if (state == StateMessage.TimeOut)
                {
                    conn = DriverErrorCodes.ErrorTimeOut;
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                    lock (lockThreadObject)
                        ReceiveBuffer.Clear();
                    OnJobExecuted(eAJob);
                     return false;
                }
                byte auxByte = ReceiveBuffer[0];
                if (mJob.ExpectedReplyLength == 1)
                {
                    // ACK = positive reply to output commands
                    if (auxByte != 0x06)
                    {
                        ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                        eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeNakReplyToOutputCommand;
                        lock (lockThreadObject)
                            ReceiveBuffer.Clear();
                        eAJob.Job = pendingjob;
                        OnJobExecuted(eAJob);
                        //System.Diagnostics.Trace.TraceInformation(string.Format("{1} - {2} ProcessNewData Error:{0} return point 5",
                        //    eAJob.ErrorCode, DateTime.Now.ToLongTimeString(), mJob.AddressObj.Address));
                        return true;
                    }

                    //LastErrorMessage = "";
                    ExecutedJobArgs eMJob = new ExecutedJobArgs();
                    lock (lockThreadObject)
                        ReceiveBuffer.RemoveRange(0, 1);
                    eMJob.Job = pendingjob;
                    OnJobExecuted(eMJob);
                    //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} ProcessNewData Output Job OK return point 6",
                    //                                            DateTime.Now.ToLongTimeString(),
                    //                                            mJob.AddressObj.Address));
                    return true;
                }

                // Input request
                if (auxByte == 0x15) // NAK
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeNakReplyToInputRequest;
                    lock (lockThreadObject)
                        ReceiveBuffer.Clear();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    //System.Diagnostics.Trace.TraceInformation(string.Format("{1} - {2} ProcessNewData Error:{0} return point 7",
                    //    eAJob.ErrorCode, DateTime.Now.ToLongTimeString(), mJob.AddressObj.Address));
                    return true;
                }

                // The first character of the frame must be STX
                if (auxByte != 0x02)
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeStx;
                    lock (lockThreadObject)
                        ReceiveBuffer.Clear();
                    eAJob.Job = pendingjob;
                    //System.Diagnostics.Trace.TraceInformation(string.Format("{1} - {2} ProcessNewData Error:{0} return point 9",
                    //    eAJob.ErrorCode, DateTime.Now.ToLongTimeString(), mJob.AddressObj.Address));
                    OnJobExecuted(eAJob);
                    return true;
                }
                // Check the message termination (ETX character)
                lock (lockThreadObject)
                    auxByte = ReceiveBuffer[(int)(mJob.ExpectedReplyLength - 3)];
                if (auxByte != 0x03)
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeEtx;
                    lock (lockThreadObject)
                        ReceiveBuffer.Clear();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    //System.Diagnostics.Trace.TraceInformation(string.Format("{1} - {2} ProcessNewData Error:{0} return point 10",
                    //    eAJob.ErrorCode, DateTime.Now.ToLongTimeString(), mJob.AddressObj.Address));
                    return true;
                }
                // Check the FCS
                byte[] receivedFrame = new byte[mJob.ExpectedReplyLength];
                lock (lockThreadObject)
                    ReceiveBuffer.CopyTo(1, receivedFrame, 0, (int)(mJob.ExpectedReplyLength - 1));
                if (CheckFCS(ref receivedFrame, mJob.ExpectedReplyLength - 1) != true)
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = conn, Job = pendingjob };
                    eAJob.ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeFcs;
                    lock (lockThreadObject)
                        ReceiveBuffer.Clear();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    //System.Diagnostics.Trace.TraceInformation(string.Format("{1} - {2} ProcessNewData Error:{0} return point 11",
                    //    eAJob.ErrorCode, DateTime.Now.ToLongTimeString(), mJob.AddressObj.Address));
                    return true;
                }

                // Convert the data in binary format
                int dataByteCount = 0;
                byte[] answewr = null;
                try
                {
                    lock (lockThreadObject)
                        ReceiveBuffer.CopyTo(1, receivedFrame, 0, (int)(mJob.ExpectedReplyLength - 4));
                    string auxString = Encoding.UTF8.GetString(receivedFrame);
                    dataByteCount = (int)((mJob.ExpectedReplyLength - 4) / 2);
                    answewr = new byte[(mJob.ExpectedReplyLength - 4) / 2];
                    for (int i = 0; i < dataByteCount; i++)
                    {
                        answewr[i] = Convert.ToByte(auxString.Substring(2 * i, 2), 16);
                    }
                }
                catch
                {
                    ExecutedJobArgs eAJob = new ExecutedJobArgs
                    {
                        ErrorCode = (DriverCodeBaseEx.Enumerators.DriverErrorCodes)MelsecFXErrorCodes.ErrorCodeNakReplyToInputRequest,
                        Job = pendingjob
                    };
                    lock (lockThreadObject)
                        ReceiveBuffer.Clear();
                    eAJob.Job = pendingjob;
                    OnJobExecuted(eAJob);
                    //System.Diagnostics.Trace.TraceInformation(string.Format("{1} - {2} ProcessNewData Error:{0} return point 7",
                    //    eAJob.ErrorCode, DateTime.Now.ToLongTimeString(), mJob.AddressObj.Address));
                    return true;
                }
                //LastErrorMessage = "";
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Values = answewr;
                lock (lockThreadObject)
                    ReceiveBuffer.RemoveRange(0, (int)mJob.ExpectedReplyLength);
                eJob.Job = pendingjob;
                OnJobExecuted(eJob);
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0} - {1} ProcessNewData Input Job OK return point 12",
                //    DateTime.Now.ToLongTimeString(), mJob.AddressObj.Address));
                //return true;

            //}
            return true;
        }

        #endregion

        #region Specific Methods


        private StateMessage WaitCompleteMessage(CommJob pendingjob)
        {
            
            MelsecFXCommJob mJob = pendingjob as MelsecFXCommJob;           
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            while (true)
            {        
                lock (lockThreadObject)
                {
                    
                    if (ReceiveBuffer.Count >= mJob.ExpectedReplyLength)
                    {
                        break;
                    }
                    if(ReceiveBuffer.Count > 1 && (ReceiveBuffer[0] == 0x15))
                    {
                        break;
                    }
                }
                
                conn = DriverErrorCodes.ErrorNoError;
                WaitNewDataEvent(ref conn);

                if (conn != (int)DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError)
                {
                    return StateMessage.TimeOut;
                }
            } 
            return StateMessage.Ok;
        }

        private uint GetRequestFrameLength(MelsecFXCommJob job)
        {
            // The frame length depends on the selected PLC type
            MelsecFXStation fxStation = (MelsecFXStation)job.Station;
            if (fxStation == null)
            {
                return 0;
            }
            MelsecFXPLCType plcType = fxStation.PLCType;
            MelsecFXDataArea dataArea = job.AddressObj.DataArea;

            // Input request
            if (job.ReadRequest())
            {
                switch (plcType)
                {
                    case MelsecFXPLCType.FX:
                        return 11;
                    case MelsecFXPLCType.FX2N:
                    case MelsecFXPLCType.FX3U:
                        if ((dataArea != MelsecFXDataArea.DataArea_M_Special) &&
                            (dataArea != MelsecFXDataArea.DataArea_D_Special))
                        {
                            return 13;
                        }
                        else
                        {
                            return 11;
                        }
                    default:
                        return 0;
                }
            }

            // Output request
            uint DataFrameLength = 0;
            //uint StartAddress = (uint)job.AddressObj.StartAddress;
            switch (dataArea)
            {
                // Special case: bits are written one per time
                case MelsecFXDataArea.DataArea_X:
                case MelsecFXDataArea.DataArea_Y:
                case MelsecFXDataArea.DataArea_S:
                case MelsecFXDataArea.DataArea_M:
                case MelsecFXDataArea.DataArea_M_Special:
                    if (job.BitVariables && !job.ArrayVariables)
                    {
                        switch (plcType)
                        {
                            case MelsecFXPLCType.FX:
                                return 9;
                            case MelsecFXPLCType.FX2N:
                            case MelsecFXPLCType.FX3U:
                                if (dataArea != MelsecFXDataArea.DataArea_M_Special)
                                {
                                    return 10;
                                }
                                else
                                {
                                    return 9;
                                }
                            default:
                                return 0;
                        }
                    }
                    break;
            }
            DataFrameLength = 2*job.TotalJobSize;
            switch (plcType)
            {
                case MelsecFXPLCType.FX:
                    DataFrameLength += 11;
                    break;
                case MelsecFXPLCType.FX2N:
                case MelsecFXPLCType.FX3U:
                    if ((dataArea != MelsecFXDataArea.DataArea_M_Special) &&
                        (dataArea != MelsecFXDataArea.DataArea_D_Special))
                    {
                        DataFrameLength += 13;
                    }
                    else
                    {
                        DataFrameLength += 11;
                    }
                    break;
                default:
                    return 0;
            }
            return DataFrameLength;
        }

        private uint PrepareRequestFrame(MelsecFXCommJob job, ref byte[] buffer)
        {
            if ((job == null) || (job.Station as MelsecFXStation == null) ||
                !job.AddressObj.IsValid)
            {
                return 0;
            }

            job.ExpectedReplyLength = 0;
            uint byteCount = 0;

            if (job.ReadRequest())
            {
                byteCount = PrepareReadRequestFrame(job, ref buffer);
            }
            else
            {
                byteCount = PrepareWriteRequestFrame(job, ref buffer);
            }

            return byteCount;
        }

        private uint PrepareReadRequestFrame(MelsecFXCommJob job, ref byte[] buffer)
        {
            // Get the device start address in bytes
            MelsecFXPLCType plcType = (job.Station as MelsecFXStation).PLCType;
            Int32 byteStartAddress = job.AddressObj.GetByteStartAddress(plcType, 0);
            if (byteStartAddress < 0)
            {
                return 0;
            }
            uint frameLength = 0;
            uint checksum = 0;

            // Frame header
            buffer[frameLength++] = 0X02; // STX
            //System.Diagnostics.Trace.TraceInformation("RR <02");
            if ((plcType != MelsecFXPLCType.FX) &&
                (job.AddressObj.DataArea != MelsecFXDataArea.DataArea_M_Special) &&
                (job.AddressObj.DataArea != MelsecFXDataArea.DataArea_D_Special))
            {
                buffer[frameLength++] = (byte)'E'; // Extended Protocol
                //System.Diagnostics.Trace.TraceInformation("45");
                checksum += (byte)'E';
            }
            buffer[frameLength++] = (byte)MelsecFXCommandCode.Command_Read_Bytes;
            //System.Diagnostics.Trace.TraceInformation("30");
            checksum += (byte)MelsecFXCommandCode.Command_Read_Bytes;
            job.CommandCode = (byte)MelsecFXCommandCode.Command_Read_Bytes;

            // Calculate the number of bytes to be read
            uint itemCount = job.TotalJobSize;
            switch (job.AddressObj.DataArea)
            {
                case MelsecFXDataArea.DataArea_X:
                case MelsecFXDataArea.DataArea_Y:
                case MelsecFXDataArea.DataArea_M:
                case MelsecFXDataArea.DataArea_S:
                case MelsecFXDataArea.DataArea_M_Special:
                    if (job.BitVariables)
                    {
                        // single bit
                        if (job.TotalJobSize == 1)
                        {
                            itemCount = 1;
                        }
                        else
                        {
                            int byteEndAddress = job.AddressObj.GetByteStartAddress(plcType, job.TotalJobSize);
                            itemCount = (uint)(byteEndAddress - byteStartAddress);
                            // if last tag's bit is not the last of the byte, add one byte (to manage /8 error conversion in the GetByteStartAddress() method)
                            if ((job.AddressObj.StartAddress + (int)job.TotalJobSize) % 8 != 0)
                                itemCount++;
                        }
                    }
                    break;

                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_D_Special:
                case MelsecFXDataArea.DataArea_C_16:
                case MelsecFXDataArea.DataArea_C_32:
                case MelsecFXDataArea.DataArea_TN:
                    break;

                default:
                    return 0;
            }

            // Start address
            byte auxByte = 0;
            if ((plcType != MelsecFXPLCType.FX) &&
                (job.AddressObj.DataArea != MelsecFXDataArea.DataArea_M_Special) &&
                (job.AddressObj.DataArea != MelsecFXDataArea.DataArea_D_Special))
            {
                auxByte = (byte)byteStartAddress.ToString("X5")[0];
                buffer[frameLength++] = auxByte;
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0}",
                //   auxByte.ToString("X2")));
                checksum += auxByte;
                auxByte = (byte)byteStartAddress.ToString("X5")[1];
                buffer[frameLength++] = auxByte;
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0}",
                //   auxByte.ToString("X2")));
                checksum += auxByte;
                auxByte = (byte)byteStartAddress.ToString("X5")[2];
                buffer[frameLength++] = auxByte;
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0}",
                //   auxByte.ToString("X2")));
                checksum += auxByte;
                auxByte = (byte)byteStartAddress.ToString("X5")[3];
                buffer[frameLength++] = auxByte;
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0}",
                //   auxByte.ToString("X2")));
                checksum += auxByte;
                auxByte = (byte)byteStartAddress.ToString("X5")[4];
                buffer[frameLength++] = auxByte;
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0}",
                //   auxByte.ToString("X2")));
                checksum += auxByte;
            }
            else
            {
                auxByte = (byte)byteStartAddress.ToString("X4")[0];
                buffer[frameLength++] = auxByte;
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0}",
                //   auxByte.ToString("X2")));
                checksum += auxByte;
                auxByte = (byte)byteStartAddress.ToString("X4")[1];
                buffer[frameLength++] = auxByte;
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0}",
                //   auxByte.ToString("X2")));
                checksum += auxByte;
                auxByte = (byte)byteStartAddress.ToString("X4")[2];
                buffer[frameLength++] = auxByte;
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0}",
                //   auxByte.ToString("X2")));
                checksum += auxByte;
                auxByte = (byte)byteStartAddress.ToString("X4")[3];
                buffer[frameLength++] = auxByte;
                //System.Diagnostics.Trace.TraceInformation(string.Format("{0}",
                //   auxByte.ToString("X2")));
                checksum += auxByte;
            }
 
            // Byte count
            auxByte = (byte)itemCount.ToString("X2")[0];
            buffer[frameLength++] = auxByte;
            //System.Diagnostics.Trace.TraceInformation(string.Format("{0}",
            //   auxByte.ToString("X2")));
            checksum += auxByte;
            auxByte = (byte)itemCount.ToString("X2")[1];
            buffer[frameLength++] = auxByte;
            //System.Diagnostics.Trace.TraceInformation(string.Format("{0}",
            //   auxByte.ToString("X2")));
            checksum += auxByte;

            // ETX
            buffer[frameLength++] = 0X03;
            //System.Diagnostics.Trace.TraceInformation("03");
            checksum += 0X03;

            // Checksum
            auxByte = (byte)checksum;
            byte auxByte2 = (byte)auxByte.ToString("X2")[0];
            //System.Diagnostics.Trace.TraceInformation(string.Format("{0}",
            //          auxByte2.ToString("X2")));
            auxByte2 = (byte)auxByte.ToString("X2")[1];
            //System.Diagnostics.Trace.TraceInformation(string.Format("{0}>",
            //          auxByte2.ToString("X2")));
            buffer[frameLength++] = (byte)auxByte.ToString("X2")[0];
            buffer[frameLength++] = (byte)auxByte.ToString("X2")[1];

            // Expected reply length
            job.ExpectedReplyLength = itemCount * 2 + 4;

            return frameLength;
        }

        private uint PrepareWriteRequestFrame(MelsecFXCommJob job, ref byte[] buffer)
        {
            Tag tag;
            object objectData = null;

            job.GetJobData(ref objectData);
            if (job.GetTagListOnWritingCount() == 0)
            {
                return 0;
            }
            
            tag = job.GetTagListOnWriting()[0];
            //}

            byte[] jobdata = (byte[])objectData;
            UInt16 DataSize = (UInt16)jobdata.Length;

            bool singleBit = false;
            uint offset = tag.ByteOffset;
            bool isBoolArray = false;
            switch (job.AddressObj.DataArea)
            {
                case MelsecFXDataArea.DataArea_X:
                case MelsecFXDataArea.DataArea_Y:
                case MelsecFXDataArea.DataArea_M:
                case MelsecFXDataArea.DataArea_S:
                case MelsecFXDataArea.DataArea_M_Special:
                    if (job.BitVariables)
                    {
                        singleBit = true;
                    }
                    else
                    {
                        offset *= 8;
                    }
                    break;

                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_D_Special:
                case MelsecFXDataArea.DataArea_C_16:
                case MelsecFXDataArea.DataArea_TN:
                    offset /= 2;
                    break;

                case MelsecFXDataArea.DataArea_C_32:
                    offset /= 4;
                    break;
            }

            // Get the true device address
            MelsecFXPLCType plcType = (job.Station as MelsecFXStation).PLCType;
            Int32 startAddress = -1;
            if (singleBit)
            {
                startAddress = job.AddressObj.GetBitStartAddress(plcType, offset);
            }
            else {
                startAddress = job.AddressObj.GetByteStartAddress(plcType, offset);
            }
            if (startAddress < 0)
            {
                return 0;
            }

            // Frame header
            uint frameLength = 0;
            uint checksum = 0;
            buffer[frameLength++] = 0X02; // STX
            //System.Diagnostics.Trace.TraceInformation("<02");
            if ((plcType != MelsecFXPLCType.FX) &&
                (job.AddressObj.DataArea != MelsecFXDataArea.DataArea_M_Special) &&
                (job.AddressObj.DataArea != MelsecFXDataArea.DataArea_D_Special))
            {
                buffer[frameLength++] = (byte)'E'; // Extended Protocol
                checksum += (byte)'E';
                //System.Diagnostics.Trace.TraceInformation(" 45");
            }
            if (!singleBit)
            {
                buffer[frameLength++] = (byte)MelsecFXCommandCode.Command_Write_Bytes;
                checksum += (byte)MelsecFXCommandCode.Command_Write_Bytes;
                job.CommandCode = (byte)MelsecFXCommandCode.Command_Write_Bytes;
                //System.Diagnostics.Trace.TraceInformation(" 31");
            }
            else if (tag.TagNode.ArrayDimension == 0)
            {
                if ((bool)tag.Value.Value == true)
                {
                    buffer[frameLength++] = (byte)MelsecFXCommandCode.Command_Write_BitOn;
                    checksum += (byte)MelsecFXCommandCode.Command_Write_BitOn;
                    job.CommandCode = (byte)MelsecFXCommandCode.Command_Write_BitOn;
                    //System.Diagnostics.Trace.TraceInformation(" 37");
                }
                else
                {
                    buffer[frameLength++] = (byte)MelsecFXCommandCode.Command_Write_BitOff;
                    checksum += (byte)MelsecFXCommandCode.Command_Write_BitOff;
                    job.CommandCode = (byte)MelsecFXCommandCode.Command_Write_BitOff;
                    //System.Diagnostics.Trace.TraceInformation(" 38");
                }
            }
            else
            {
                // Special case: array of booleans
                buffer[frameLength++] = (byte)MelsecFXCommandCode.Command_Write_Bytes;
                checksum += (byte)MelsecFXCommandCode.Command_Write_Bytes;
                job.CommandCode = (byte)MelsecFXCommandCode.Command_Write_Bytes;
                isBoolArray = true;
                // Modified to solve FOGBUGZ 10946
                //startAddress = job.AddressObj.GetByteStartAddress(plcType, offset);
                startAddress = job.AddressObj.GetByteStartAddress(plcType,
                               offset + (uint)(job.AddressObj.StartAddress%8));

                //System.Diagnostics.Trace.TraceInformation(" 31");
           }

            byte auxByte = 0;
            if (!singleBit)
            {
                // Address
                if ((plcType != MelsecFXPLCType.FX) &&
                    (job.AddressObj.DataArea != MelsecFXDataArea.DataArea_M_Special) &&
                    (job.AddressObj.DataArea != MelsecFXDataArea.DataArea_D_Special))
                {
                    auxByte = (byte)startAddress.ToString("X5")[0];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)startAddress.ToString("X5")[1];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)startAddress.ToString("X5")[2];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)startAddress.ToString("X5")[3];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)startAddress.ToString("X5")[4];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                }
                else
                {
                    auxByte = (byte)startAddress.ToString("X4")[0];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)startAddress.ToString("X4")[1];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    auxByte = (byte)startAddress.ToString("X4")[2];
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)startAddress.ToString("X4")[3];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                }

                // Byte count
                auxByte = (byte)DataSize.ToString("X2")[0];
                //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                //       auxByte.ToString("X2")));
                buffer[frameLength++] = auxByte;
                checksum += auxByte;
                auxByte = (byte)DataSize.ToString("X2")[1];
                //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                //       auxByte.ToString("X2")));
                buffer[frameLength++] = auxByte;
                checksum += auxByte;

                // Data
                int i = 0;
                for (i = 0; i < DataSize; i++)
                {
                    auxByte = (byte)jobdata[i].ToString("X2")[0];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)jobdata[i].ToString("X2")[1];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                }
            }
            else if (isBoolArray)
            {
                // Address
                if ((plcType != MelsecFXPLCType.FX) &&
                    (job.AddressObj.DataArea != MelsecFXDataArea.DataArea_M_Special) &&
                    (job.AddressObj.DataArea != MelsecFXDataArea.DataArea_D_Special))
                {
                    auxByte = (byte)startAddress.ToString("X5")[0];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)startAddress.ToString("X5")[1];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)startAddress.ToString("X5")[2];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)startAddress.ToString("X5")[3];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)startAddress.ToString("X5")[4];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                }
                else
                {
                    auxByte = (byte)startAddress.ToString("X4")[0];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)startAddress.ToString("X4")[1];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)startAddress.ToString("X4")[2];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)startAddress.ToString("X4")[3];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                }

                // Byte count
                // Calculate the dimension in bytes 
                ushort byteDataSize = (ushort)(DataSize / 8);
                if ((DataSize % 8) > 0)
                {
                    byteDataSize++;
                }
                auxByte = (byte)byteDataSize.ToString("X2")[0];
                //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                //       auxByte.ToString("X2")));
                buffer[frameLength++] = auxByte;
                checksum += auxByte;
                auxByte = (byte)byteDataSize.ToString("X2")[1];
                //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                //       auxByte.ToString("X2")));
                buffer[frameLength++] = auxByte;
                checksum += auxByte;

                int i = 0;

                // Added to solve FOGBUGZ 10946
                byte aux2Byte = 0;

                // Add data to the frame
                for (i = 0; i < byteDataSize; i++)
                {
                    // Modified to solve FOGBUGZ 10946
                    //auxByte = (byte)jobdata[i].ToString("X2")[0];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    //buffer[frameLength++] = auxByte;
                    //checksum += auxByte;
                    //auxByte = (byte)jobdata[i].ToString("X2")[1];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    //buffer[frameLength++] = auxByte;
                    //checksum += auxByte;
                    aux2Byte = jobdata[i];
                    if ((job.AddressObj.StartAddress + offset) % 8 > 0)
                    {
                        aux2Byte <<= (byte)((job.AddressObj.StartAddress + offset) % 8);
                    }
                    auxByte = (byte)aux2Byte.ToString("X2")[0];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                    auxByte = (byte)aux2Byte.ToString("X2")[1];
                    //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                    //   auxByte.ToString("X2")));
                    buffer[frameLength++] = auxByte;
                    checksum += auxByte;
                }
            }
            // Write a single bit
            else
            {
                // Address
                auxByte = (byte)startAddress.ToString("X4")[2];
                //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                //       auxByte.ToString("X2")));
                buffer[frameLength++] = auxByte;
                checksum += auxByte;
                auxByte = (byte)startAddress.ToString("X4")[3];
                //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                //       auxByte.ToString("X2")));
                buffer[frameLength++] = auxByte;
                checksum += auxByte;
                auxByte = (byte)startAddress.ToString("X4")[0];
                //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                //       auxByte.ToString("X2")));
                buffer[frameLength++] = auxByte;
                checksum += auxByte;
                auxByte = (byte)startAddress.ToString("X4")[1];
                //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
                //       auxByte.ToString("X2")));
                buffer[frameLength++] = auxByte;
                checksum += auxByte;
            }

            // ETX
            buffer[frameLength++] = 0X03;
            //System.Diagnostics.Trace.TraceInformation(" 03");
            checksum += 0X03;

            // Checksum
            auxByte = (byte)checksum;
            byte auxByte2 = (byte)auxByte.ToString("X2")[0];
            //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}",
            //          auxByte2.ToString("X2")));
            auxByte2 = (byte)auxByte.ToString("X2")[1];
            //System.Diagnostics.Trace.TraceInformation(string.Format(" {0}>",
            //          auxByte2.ToString("X2")));
            buffer[frameLength++] = (byte)auxByte.ToString("X2")[0];
            buffer[frameLength++] = (byte)auxByte.ToString("X2")[1];

            // Expected reply length
            job.ExpectedReplyLength = 1;

            return frameLength;
        }

        private byte CalculateChecksum(ref byte[] buffer, uint bufferLength)
        {
            UInt16 checksum = 0;
            for (uint i = 0; i < bufferLength; i++)
            {
                checksum += buffer[i];
            }
            return( (byte) checksum );
        }

        private bool CheckFCS(ref byte[] buffer, uint bufferLength)
        {
            byte calculatedFCS = CalculateChecksum(ref buffer, bufferLength-2);
 
            // Compare the calculated FCS with the one of the received frame
            byte auxByte0 = (byte)calculatedFCS.ToString("X2")[0];
            byte auxByte1 = (byte)calculatedFCS.ToString("X2")[1];
            if ((auxByte0 != buffer[(int)(bufferLength - 2)]) ||
                (auxByte1 != buffer[(int)(bufferLength - 1)]))
            {
                return false;
            }

            return true;
        }


        #endregion

        #region Custom Job
        protected override void GetCustomChildJobs(CommJob exjob, out List<CommJob> resultJobs, out bool write)
        {
            resultJobs = new List<CommJob>();

            MelsecFXCommJob exjobFX = exjob as MelsecFXCommJob;

            write = (exjobFX.GetTagListOnWritingCount() > 0 || exjobFX.Type == LinkType.UnconditionalOutput);
            if (write)
            {
                if (exjob.Type == LinkType.UnconditionalOutput)
                    exjob.FillWholeTagsListOnWriting();
                
                if (exjobFX.IsBitArray())
                {
                    exjobFX.ChildJobsWrite.Clear();

                    foreach (MelsecFXTag tagToWrite in exjobFX.GetTagListOnWriting())
                    {
                        List<MelsecFXTag.ChangedBit> changedBits = tagToWrite.GetTagBufferBits();
                        foreach (MelsecFXTag.ChangedBit changedBit in changedBits)
                        {
                            MelsecFXCommJob childJob = exjobFX.CreateChildJobWriteArrayBit(changedBit) as MelsecFXCommJob;                            
                            childJob.TagsList[0].Value.Value = changedBit.BitValue;
                            childJob.AddTagListOnWriting(childJob.TagsList[0]);

                            exjobFX.ChildJobsWrite[childJob.TagsList[0].TagNode.NodeId] = childJob;

                            resultJobs.Add(childJob);
                        }
                    }

                    Parallel.ForEach(resultJobs, j =>
                    {
                        j.InitChildJobBeforeExecution();
                    });
                }                
            }
            else
            {   
                // use driver existing array of bit management
                if (exjobFX.IsBitArray())
                    resultJobs.Add(exjob);
            }            
        }

        /// <summary>
        /// Merge the result of "Big job child jobs" into exJob job and then publish result
        /// </summary>
        /// <param name="exjob"></param>
        /// <param name="childJobs"></param>
        /// <param name="write"></param>
        protected override void MergeCustomJobChildJobs(CommJob exjob, List<CommJob> childJobs, bool write)
        {
            MelsecFXCommJob exjobFX = exjob as MelsecFXCommJob;

            ExecutedJobArgs e = new ExecutedJobArgs();
            e.Job = exjob;
            e.ErrorCode = DriverErrorCodes.ErrorNoError;
            foreach (CommJob j in childJobs)
            {
                if (!j.IsPending && j.ChildJobErrorCode != DriverErrorCodes.ErrorNoError)
                {
                    e.ErrorCode = j.ChildJobErrorCode;
                    break;
                }
            }                
            if (exjobFX.IsBitArray())
            {
                if (write)
                {
                    if (e.ErrorCode == DriverErrorCodes.ErrorNoError)
                    {
                        exjobFX.TagsList[0].SetIniternalValue(exjobFX.TagsList[0].Value.Value);
                    }
                    exjob.RWState = CommJob.RWStates.Standard;
                    OnJobExecuted(e);
                }
                // else --> manage as a standard job
            }                        
        }
        #endregion
    }
}
