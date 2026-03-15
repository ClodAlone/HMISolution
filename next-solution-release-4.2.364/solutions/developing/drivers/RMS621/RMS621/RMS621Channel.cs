using System;
using System.Globalization;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using SerialDriverCodeBase;

namespace RMS621
{
    public class RMS621Channel : SerialChannel
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public RMS621Channel(CommunicationDriver commdriver, RMS621ChannelSettings settings)
            : base(commdriver, settings)
        {
        }

        #endregion

        #region Override Methods
        public override void ExecuteJob(CommJob job)
        {
            RMS621CommJob mJob = job as RMS621CommJob;
            if (mJob == null)
            {
                return;
            }

            if (job.IsPending == true)
            {
                return;
            }

            base.ExecuteJob(job);

            // Prepare the request frame
            uint dim = GetRequestFrameLength(mJob);
            if (dim == 0)
            {
                lock (lockThreadObject)
                {
                    RemovePendingJob(job);
                    job.IsPending = false;
                }
                return;
            }
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
            job.LastExecutionTime = DateTime.UtcNow;
        }

        public override bool ProcessNewData(CommJob pendingjob)
        {
            if (LastErrorCode != (int)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
            {
                ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                OnJobExecuted(eAJob);
                LastErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                ReceiveBuffer.Clear();
                return true;
            }

            lock (lockList)
            {
                if (pendingjob == null)
                {
                    ReceiveBuffer.Clear();
                    return false;
                }
                RMS621CommJob mJob = pendingjob as RMS621CommJob;
                if (mJob == null)
                {
                    ReceiveBuffer.Clear();
                    return false;
                }

                byte[] Data;
                int MessageEnd;
                int Ret = 0;

                if (mJob.Type == LinkType.Input || (mJob.Type == LinkType.InputOutput && mJob.TagsListToWrite.Count == 0))
                {
                    Ret = RMS621Protocol.ParseReadData(ReceiveBuffer, out Data, out MessageEnd);
                    if (Ret == (int)(DriverErrorCodes.ErrorNoError))
                        Ret = ConvertDataDeviceTypeToMoviconType(mJob, ref Data);                    
                }
                else
                    Ret = RMS621Protocol.ParseWritedData(ReceiveBuffer, out Data, out MessageEnd);

                switch (Ret)
                {
                    // message incomplete --> wait to recive others data
                    case (int)RMS621Protocol.RMS621ErrorCodes.DataMissing:
                        return false;
                    case (int)DriverErrorCodes.ErrorNoError:
                        LastErrorMessage = "";
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Values = Data;
                        eJob.ErrorCode = DriverErrorCodes.ErrorNoError;
                        ReceiveBuffer.RemoveRange(0, MessageEnd);
                        eJob.Job = pendingjob;
                        OnJobExecuted(eJob);
                        break;
                    default:    // generic error
                        ExecutedJobArgs eAJob = new ExecutedJobArgs { ErrorCode = LastErrorCode, Job = pendingjob };
                        eAJob.ErrorCode = (DriverErrorCodes)Ret;
                        ReceiveBuffer.Clear();
                        eAJob.Job = pendingjob;
                        OnJobExecuted(eAJob);

                        // dont't put driver in error on data casting error from device data to movicon data
                        if (Ret == (int)RMS621Protocol.RMS621ErrorCodes.JobDataFormat)
                            Ret = (int)DriverErrorCodes.ErrorNoError;
                        break;
                }
                                
                return (Ret == (int)DriverErrorCodes.ErrorNoError);
            }
        }

        #endregion

        #region Specific Methods

        private uint GetRequestFrameLength(RMS621CommJob job)
        {
            uint DataFrameLength = 0;

            // The frame length depends on the selected PLC type
            RMS621Station fxStation = (RMS621Station)job.Station;
            if (fxStation == null)
            {
                return DataFrameLength;
            }

            // Input request
            if (job.Type == DriverCodeBase.Enumerators.LinkType.Input ||
                (job.Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
                job.TagsListToWrite.Count == 0))
            {
                DataFrameLength = RMS621Protocol.GetRequestCommandMinLength();
            }
            else
            {
                DataFrameLength = RMS621Protocol.GetWriteCommandMinLength();
            }

            return DataFrameLength;
        }

        private uint PrepareRequestFrame(RMS621CommJob job, ref byte[] buffer)
        {
            if ((job == null) || (job.Station as RMS621Station == null) || !job.IsValid)
            {
                return 0;
            }

            uint byteCount = 0;

            if (job.Type == DriverCodeBase.Enumerators.LinkType.Input ||
                (job.Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
                job.TagsListToWrite.Count == 0))
            {
                byteCount = PrepareReadRequestFrame(job, ref buffer);
            }
            else
            {
                byteCount = PrepareWriteRequestFrame(job, ref buffer);
            }

            return byteCount;
        }


        private int ConvertDataDeviceTypeToMoviconType(RMS621CommJob job, ref byte[] buffer)
        {
            int ConvertResult = (int)DriverErrorCodes.ErrorNoError;

            if ((uint)job.TagsList[0].TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.Double)
                return ConvertResult;

            Double fValue = BitConverter.ToDouble(buffer, 0);
            Array.Resize(ref buffer, (int)job.TagsList[0].Size);
            
            try
            {
                /// <summary>   The read value. </summary>
                switch ((uint)job.TagsList[0].TagNode.DataType.Identifier)
                {
                    case (uint)Opc.Ua.DataTypes.SByte:
                        if (fValue >= SByte.MinValue && fValue <= SByte.MaxValue)
                        {
                            SByte vSByte = (SByte)fValue;
                            buffer = BitConverter.GetBytes(vSByte);
                        }
                        else
                        {
                            ConvertResult = (int)RMS621Protocol.RMS621ErrorCodes.JobDataFormat;
                        }                        
                        break;

                    case (uint)Opc.Ua.DataTypes.Byte:
                        if (fValue >= Byte.MinValue && fValue <= Byte.MaxValue)
                        {
                            Byte VByte = (Byte)fValue;
                            buffer = BitConverter.GetBytes(VByte);
                        }
                        else
                        {
                            ConvertResult = (int)RMS621Protocol.RMS621ErrorCodes.JobDataFormat;
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.UInt16:
                        if (fValue >= UInt16.MinValue && fValue <= UInt16.MaxValue)
                        {
                            UInt16 VUInt16 = (UInt16)fValue;
                            buffer = BitConverter.GetBytes(VUInt16);
                        }
                        else
                        {
                            ConvertResult = (int)RMS621Protocol.RMS621ErrorCodes.JobDataFormat;
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.Double:
                        buffer = BitConverter.GetBytes(fValue);
                        break;

                    case (uint)Opc.Ua.DataTypes.Int16:
                        if (fValue >= UInt16.MinValue && fValue <= UInt16.MaxValue)
                        {
                            Int16 VInt16 = (Int16)fValue;
                            buffer = BitConverter.GetBytes(VInt16);
                        }
                        else
                        {
                            ConvertResult = (int)RMS621Protocol.RMS621ErrorCodes.JobDataFormat;
                        }

                        break;

                    case (uint)Opc.Ua.DataTypes.Int32:
                        if (fValue >= Int32.MinValue && fValue <= Int32.MaxValue)
                        {
                            Int32 VInt32 = (Int32)fValue;
                            buffer = BitConverter.GetBytes(VInt32);
                        }
                        else
                        {
                            ConvertResult = (int)RMS621Protocol.RMS621ErrorCodes.JobDataFormat;
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.Int64:
                        if (fValue >= Int64.MinValue && fValue <= Int64.MaxValue)
                        {
                            Int64 VInt64 = (Int64)fValue;
                            buffer = BitConverter.GetBytes(VInt64);
                        }
                        else
                        {
                            ConvertResult = (int)RMS621Protocol.RMS621ErrorCodes.JobDataFormat;
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.UInt32:
                        if (fValue >= UInt32.MinValue && fValue <= UInt32.MaxValue)
                        {
                            UInt32 VUInt32 = (UInt32)fValue;
                            buffer = BitConverter.GetBytes(VUInt32);
                        }
                        else
                        {
                            ConvertResult = (int)RMS621Protocol.RMS621ErrorCodes.JobDataFormat;
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.UInt64:
                        if (fValue >= UInt64.MinValue && fValue <= UInt64.MaxValue)
                        {
                            UInt64 VUInt64 = (UInt64)fValue;
                            buffer = BitConverter.GetBytes(VUInt64);
                        }
                        else
                        {
                            ConvertResult = (int)RMS621Protocol.RMS621ErrorCodes.JobDataFormat;
                        }
                        break;

                    case (uint)Opc.Ua.DataTypes.Float:
                        if (fValue >= Single.MinValue && fValue <= Single.MaxValue)
                        {
                            Single VSingle = (Single)fValue;
                            buffer = BitConverter.GetBytes(VSingle);
                        }
                        else
                        {
                            ConvertResult = (int)RMS621Protocol.RMS621ErrorCodes.JobDataFormat;
                        }
                        break;
                        
                }
            } catch (Exception ex)
            {
                ConvertResult = (int)RMS621Protocol.RMS621ErrorCodes.JobDataFormat;
            }

            return ConvertResult;
        }


        private uint PrepareReadRequestFrame(RMS621CommJob job, ref byte[] buffer)
        {            
            return RMS621Protocol.DoReadData(ref buffer, (int)job.Process, job.ProcessNumber, ((RMS621Station)job.Station).UnitNumber);
        }

        private uint PrepareWriteRequestFrame(RMS621CommJob job, ref byte[] buffer)
        {
            Tag tag;
            object objectData = null;
            lock (job.retLockList())
            {
                if (job.Type == LinkType.UnconditionalOutput)
                {
                    if (job.TagsListToWrite.Count == 0)
                    {
                        job.TagsListToWrite.AddRange(job.TagsList);
                    }
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

            return RMS621Protocol.DoWriteData(ref buffer, jobdata,(int)job.Command, job.ProcessNumber, ((RMS621Station)job.Station).UnitNumber);
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

            return ((auxByte0 != buffer[(int)(bufferLength - 2)]) || (auxByte1 != buffer[(int)(bufferLength - 1)]));
        }


        #endregion
    }
}
