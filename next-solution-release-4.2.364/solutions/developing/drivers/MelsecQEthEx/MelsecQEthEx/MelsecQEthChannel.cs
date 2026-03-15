using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using System.Runtime.InteropServices;
using DriverCodeBaseEx.Enumerators;
using System.Text;

namespace MelsecQEth
{    
    public class MelsecQEthChannel : TcpChannelList
    {
        [StructLayout(LayoutKind.Explicit)]
        private struct ushortUnion
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
        private struct uintUnion
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

        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public MelsecQEthChannel(CommunicationDriver commdriver, MelsecQEthChannelSettings settings)
            : base(commdriver, settings)
        {
        }

        #endregion

        #region constants
        private const int MESSAGE_HEADER_LENGTH = 9;
        #endregion

        #region Override Methods

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> list)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                // back compatibilty with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }

            bool result = false;
            if (MelsecQEthProtocol.IsLabelAddress(list))
                result = ExecuteJobListLabel(ref conn, list);
            else
                result = ExecuteJobListDataArea(ref conn, list);
            return result;
        }

        public override bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
            if (conn != (int)DriverErrorCodes.ErrorNoError)
            {
                foreach (CommJob job in list)
                    OnJobExecuted(new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorTimeOut, Job = job });

                ResetInternalJobsState();

                DeviceClose(); //Flush();
                lock (lockThreadObject)
                    ReceiveBuffer.Clear();
                return false;
            }

            bool result = false;
            if (MelsecQEthProtocol.IsLabelAddress(list))
                result = ProcessNewDataListLabel(conn, list);
            else
                result = ProcessNewDataListDataArea(conn, list);
            return result;
        }

        public override bool IsScheduledJobsListFull(List<CommJob> jobList)
        {
            if (MelsecQEthProtocol.IsLabelAddress(jobList))
                return false;
            else
                return true;    // data area address manage only 1 job at time
        }

        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {
            int jobIndex = 0;
            while (jobIndex < jobList.Count)
            {
                bool write = false;
                bool first = true;
                string station = string.Empty;
                CpuTargets cpuTarget = CpuTargets.ControlPLC;
                uint networkNumber = 0;
                uint pcNumber = 0;
                bool process = true;
                bool alreadyExchanged = false;
                MelsecQEthProtocol.ReadWriteListLimitateSize frameSize = new MelsecQEthProtocol.ReadWriteListLimitateSize();

                //append a new list to the end of exList
                List<CommJob> list = new List<CommJob>();
                while (jobIndex < jobList.Count)
                {
                    MelsecQEthCommJob j = jobList.ElementAt(jobIndex) as MelsecQEthCommJob;
                    if (MelsecQEthProtocol.IsLabelAddress(j))
                    {
                        if (first)
                        {
                            write = !j.ReadRequest();
                            station = j.Station.Name;
                            first = false;
                            cpuTarget = j.CpuTarget;
                            networkNumber = ((MelsecQEthStation)j.Station).NetworkNumber;
                            pcNumber = ((MelsecQEthStation)j.Station).PcNumber;
                            alreadyExchanged = j.AlreadyExchanged;
                        }

                        process = true;
                        // arrays cannot be aggregated with other elements (protocol limits)
                        // string data type variable settings (size, unicode) is requested ad runtime to PLC
                        // If the data has never been written the 1st time it is written by itself: this is because the writing error is general for all the tags of the frame                        
                        // exchange one job at a time as the error is cumulative to all the jobs of the frame --> j.BadNotFoundStateUncertain 
                        if (j.ArrayVariables || (j.StringSettingsAutoDetect == MelsecQEthProtocol.StringSettingsAutoDetectStates.Requested) || (!j.ReadRequest() && !j.AlreadyWritten) || j.BadNotFoundStateUncertain)
                        {
                            process = false;
                            if (list.Count == 0)
                            {
                                list.Add(j);
                                jobList.RemoveAt(jobIndex);
                                break;
                            }
                        }

                        if (process)
                        {
                            if (j.Station.Name == station && j.CpuTarget == cpuTarget && ((MelsecQEthStation)j.Station).NetworkNumber == networkNumber && ((MelsecQEthStation)j.Station).PcNumber == pcNumber && j.AlreadyExchanged == alreadyExchanged)
                            {
                                // write one value at time because protocol return only one singol error for all jobs in the frame
                                if (write && !j.ReadRequest())
                                {
                                    if (GetReadWriteListLimitateLabel(j, write, ref frameSize))
                                    {
                                        list.Add(j);
                                        jobList.RemoveAt(jobIndex);
                                        jobIndex--;
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                                else
                                {
                                    if (!write && j.ReadRequest())
                                    {
                                        if (GetReadWriteListLimitateLabel(j, write, ref frameSize))
                                        {
                                            list.Add(j);
                                            jobList.RemoveAt(jobIndex);
                                            jobIndex--;
                                        }
                                        else
                                        {
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (list.Count == 0)
                        {
                            // only 1 job at time for data area jobs
                            list.Add(j);
                            jobList.RemoveAt(jobIndex);
                            break;
                        }
                    }
                    jobIndex++;
                }

                if (list.Count > 0)
                {
                    exList.Add(list);
                    jobIndex = 0;
                }
            }
        }

        private void ResetInternalJobsState()
        {
            List<Station> stations = CommDriver.GetChannelStations(this);
            foreach (MelsecQEthStation st in stations)
            {
                if (MelsecQEthProtocol.PlcSupportLabelAddress(st.PlcType))
                    st.ResetInternalJobsState();
            }
        }
        #endregion

        #region Data Area Specific Methods
        private bool ExecuteJobListDataArea(ref DriverErrorCodes conn, List<CommJob> list)
        {
            MelsecQEthCommJob mJob = list[0] as MelsecQEthCommJob;

            base.ExecuteJob(ref conn, mJob);

            // Prepare the request frame            
            uint requestLength = PrepareRequestFrameDataArea(mJob, out byte[] requestFrame);
            if (requestLength == 0)
            {
                RemovePendingJob(mJob);
                return (false);
            }

            // Send the request frame
            if (!DeviceWrite(requestFrame, requestLength))
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return (false);
            }

            // Start the procedure for receiving the device reply
            if (!BeginDeviceRead(MESSAGE_HEADER_LENGTH))
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return (false);
            }

            return (true);
        }

        private bool ProcessNewDataListDataArea(DriverErrorCodes conn, List<CommJob> list)
        {
            CommJob pendingjob = list[0];

            List<byte> receiveBuffer = new List<byte>();
            lock (lockThreadObject)
            {
                receiveBuffer.AddRange(ReceiveBuffer);
                ReceiveBuffer.Clear();
            }

            // Check the subheader            
            Int32 minLen = 2;
            if (receiveBuffer.Count < minLen || receiveBuffer.Count < MESSAGE_HEADER_LENGTH)
            {
                OnJobExecuted(new ExecutedJobArgs { Job = pendingjob, ErrorCode = (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame });
                DeviceClose(); //Flush();
                return false;
            }

            MelsecQEthCommJob mJob = pendingjob as MelsecQEthCommJob;
            ExecutedJobArgs eJob = new ExecutedJobArgs();
            eJob.ErrorCode = DriverErrorCodes.ErrorNoError;

            ushortUnion dim = new ushortUnion(receiveBuffer, MESSAGE_HEADER_LENGTH - 2);

            byte[] pdu = new byte[MESSAGE_HEADER_LENGTH + dim.USHORT];
            if (dim.USHORT != 0)
            {
                byte[] pduData = new byte[dim.USHORT];

                int BytesToCopy = receiveBuffer.Count;
                if (BytesToCopy > pdu.Length)
                    BytesToCopy = pdu.Length;

                receiveBuffer.CopyTo(0, pdu, 0, BytesToCopy);
                ushort numberOfReceivedBytes = (ushort)DeviceReadSynchronous(pduData, (uint)dim.USHORT);

                if (numberOfReceivedBytes == (uint)dim.USHORT)
                {
                    pduData.CopyTo(pdu, MESSAGE_HEADER_LENGTH);
                    if (!AnswerOkDataArea(ref pdu, out ushort nErrCode))
                    {
                        switch (nErrCode)
                        {
                            // Wrong PC number
                            case 0xffff:
                                eJob.ErrorCode = (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorWrongReply;
                                break;

                            // Unrecognized abnormal code
                            default:
                                eJob.ErrorCode = (DriverErrorCodes)(MelsecQEthProtocol.MelsecQErrorCodes.ErrorFromDevice + nErrCode);
                                break;
                        }
                    }
                }
                else
                    eJob.ErrorCode = (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame;
            }

            // Copy received data
            if (eJob.ErrorCode == DriverErrorCodes.ErrorNoError && mJob.IsRead)
            {
                ushort dataLength = 0;
                switch (mJob.CommandCode)
                {
                    case (ushort)MelsecQCommandCode.BitUnits:
                    case (ushort)MelsecQCommandCode.BitUnits32bits:
                        {
                            dataLength = (ushort)(mJob.PointCount / 2);
                            if (mJob.PointCount % 2 > 0)
                            {
                                dataLength++;
                            }
                        }
                        break;

                    case (ushort)MelsecQCommandCode.WordUnits:
                    case (ushort)MelsecQCommandCode.WordUnits32bits:
                        {
                            dataLength = (ushort)(mJob.PointCount * 2);
                        }
                        break;
                }

                byte[] Answer = ParseReadDataDataArea(ref pdu, dataLength, mJob);
                if (Answer.Length == 0)
                    eJob.ErrorCode = (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame;
                else
                    eJob.Values = Answer;
            }

            if (eJob.ErrorCode == (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame)
                DeviceClose();
            //Flush();
            eJob.Job = pendingjob;
            OnJobExecuted(eJob);
            return true;
        }

        private uint PrepareRequestFrameDataArea(MelsecQEthCommJob job, out byte[] buffer)
        {
            job.PointCount = 0;
            job.CommandCode = 0;
            uint byteCount = 0;

            if (job.ReadRequest())
                byteCount = PrepareReadRequestFrameDataArea(job, out buffer);
            else
                byteCount = PrepareWriteRequestFrameDataArea(job, out buffer);

            return (byteCount);
        }

        private uint PrepareReadRequestFrameDataArea(MelsecQEthCommJob job, out byte[] buffer)
        {
            List<byte> resultBuffer = new List<byte>();
            buffer = null;
            // Set the subcommand and the number of device points (bits or words).
            job.PointCount = job.TotalJobSize;
            job.CommandCode = (((MelsecQEthStation)job.Station).PlcType == MelsecQEthProtocol.PlcTypes.CPU_MODEL_iQR ? (ushort)MelsecQCommandCode.WordUnits32bits : (ushort)MelsecQCommandCode.WordUnits);
            //job.CommandCode = 2;
            if (job.BitVariables)
            {
                if ((job.TotalJobSize % 16 > 0) || (job.AddressObj.StartAddress % 16 > 0))
                {
                    job.CommandCode = (((MelsecQEthStation)job.Station).PlcType == MelsecQEthProtocol.PlcTypes.CPU_MODEL_iQR ? (ushort)MelsecQCommandCode.BitUnits32bits : (ushort)MelsecQCommandCode.BitUnits);
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
            resultBuffer.Add(0x50); // Subheader (low byte).
            resultBuffer.Add(0); // Subheader (high byte).
            resultBuffer.Add((byte)(job.GetStationNetworkNumber() & 0xff)); // Network num.
            resultBuffer.Add((byte)(job.GetStationPcNumber() & 0xff)); // PC number.

            resultBuffer.AddRange(MelsecQEthProtocol.GetModuleTargetBuffer(job.CpuTarget));

            int requestlengthStartIndex = resultBuffer.Count;
            resultBuffer.Add(0x00); // Request length (low byte).
            resultBuffer.Add(0x00); // Request length (high byte).

            // CPU monitoring timer (timeout).
            ushortUnion Aux = new ushortUnion((ushort)(Timeout / 250));
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            // Batch read command.
            resultBuffer.Add(0x01); // Low byte.
            resultBuffer.Add(0x04); // High byte.

            // Subcommand.
            Aux.USHORT = (ushort)(job.CommandCode);
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            // Start address (head device).
            if (((MelsecQEthStation)job.Station).PlcType == MelsecQEthProtocol.PlcTypes.CPU_MODEL_iQR)
            {
                uintUnion AuxUint = new uintUnion((uint)job.AddressObj.StartAddress);
                resultBuffer.Add(AuxUint.LOUSHORT.LOBYTE);
                resultBuffer.Add(AuxUint.LOUSHORT.HIBYTE);
                resultBuffer.Add(AuxUint.HIUSHORT.LOBYTE);
                resultBuffer.Add(AuxUint.HIUSHORT.HIBYTE);
            }
            else 
            {
                uintUnion AuxUint = new uintUnion((uint)job.AddressObj.StartAddress);
                resultBuffer.Add(AuxUint.LOUSHORT.LOBYTE);
                resultBuffer.Add(AuxUint.LOUSHORT.HIBYTE);
                resultBuffer.Add(AuxUint.HIUSHORT.LOBYTE);
            }

            // Data area code
            if (((MelsecQEthStation)job.Station).PlcType == MelsecQEthProtocol.PlcTypes.CPU_MODEL_iQR)
            {
                Aux.USHORT = (ushort)(job.AddressObj.DataAreaCode);
                resultBuffer.Add(Aux.LOBYTE);
                resultBuffer.Add(Aux.HIBYTE);                
            }
            else
            {
                resultBuffer.Add((byte)(job.AddressObj.DataAreaCode));
            }

            // Number of device points.
            Aux.USHORT = (ushort)(job.PointCount);
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            Aux = new ushortUnion((ushort)(resultBuffer.Count - requestlengthStartIndex - 2));
            resultBuffer[requestlengthStartIndex] = Aux.LOBYTE;
            resultBuffer[requestlengthStartIndex + 1] = Aux.HIBYTE;

            buffer = resultBuffer.ToArray();

            return (uint)buffer.Count();
        }

        private uint PrepareWriteRequestFrameDataArea(MelsecQEthCommJob job, out byte[] buffer)
        {
            List<byte> resultBuffer = new List<byte>();
            buffer = null;
            Tag tag;
            object objectData = null;
            job.GetJobData(ref objectData);
            if (job.TagsListOnWriting.Count == 0)
                return 0;
            tag = job.TagsListOnWriting[0];

            byte[] jobdata = (byte[])objectData;
            UInt16 DataSize = (UInt16)jobdata.Length;

            uint DataFrameLength = 0;
            uint PointCount = DataSize;
            job.CommandCode = (((MelsecQEthStation)job.Station).PlcType == MelsecQEthProtocol.PlcTypes.CPU_MODEL_iQR ? (ushort)MelsecQCommandCode.WordUnits32bits : (ushort)MelsecQCommandCode.WordUnits);

            uint AddressOffset = tag.ByteOffset;

            uint StartAddress = (uint)job.AddressObj.StartAddress;
            if (job.BitVariables && !job.ArrayVariables)
            {
                if ((DataSize % 16 > 0) || (StartAddress % 16 > 0))
                {
                    job.CommandCode = job.CommandCode = (((MelsecQEthStation)job.Station).PlcType == MelsecQEthProtocol.PlcTypes.CPU_MODEL_iQR ? (ushort)MelsecQCommandCode.BitUnits32bits : (ushort)MelsecQCommandCode.BitUnits);
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
                //if ((buffer.Length - 21) < (job.TotalJobSize / 2))
                if (!((job.TotalJobSize % 16 > 0) || (((MelsecQEthCommJob)job).AddressObj.StartAddress % 16 > 0)))
                {
                    job.CommandCode = (((MelsecQEthStation)job.Station).PlcType == MelsecQEthProtocol.PlcTypes.CPU_MODEL_iQR ? (ushort)MelsecQCommandCode.WordUnits32bits : (ushort)MelsecQCommandCode.WordUnits);
                    PointCount = job.TotalJobSize / 16;
                    DataFrameLength = (uint)(DataSize);
                }
                else
                {
                    job.CommandCode = (((MelsecQEthStation)job.Station).PlcType == MelsecQEthProtocol.PlcTypes.CPU_MODEL_iQR ? (ushort)MelsecQCommandCode.BitUnits32bits : (ushort)MelsecQCommandCode.BitUnits);
                    PointCount = job.TotalJobSize;
                    DataFrameLength = (uint)((PointCount + 1) / 2);
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
                            job.CommandCode = (((MelsecQEthStation)job.Station).PlcType == MelsecQEthProtocol.PlcTypes.CPU_MODEL_iQR ? (ushort)MelsecQCommandCode.BitUnits32bits : (ushort)MelsecQCommandCode.BitUnits);
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

            resultBuffer.Add(0x50); // Subheader (low byte).
            resultBuffer.Add(0); // Subheader (high byte).
            resultBuffer.Add((byte)(job.GetStationNetworkNumber() & 0xff)); // Network num.
            resultBuffer.Add((byte)(job.GetStationPcNumber() & 0xff)); // PC number.

            resultBuffer.AddRange(MelsecQEthProtocol.GetModuleTargetBuffer(job.CpuTarget));

            // Request length.
            int requestlengthStartIndex = resultBuffer.Count;
            resultBuffer.Add(0x00); // Request length (low byte).
            resultBuffer.Add(0x00); // Request length (high byte).

            // CPU monitoring timer (timeout).
            ushortUnion Aux = new ushortUnion((ushort)(Timeout / 250));
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            // Batch write command.
            resultBuffer.Add(0x01); // Low byte.
            resultBuffer.Add(0x14); // High byte.

            // Subcommand.
            Aux.USHORT = (ushort)(job.CommandCode);
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            // Start address (head device).
            if (((MelsecQEthStation)job.Station).PlcType == MelsecQEthProtocol.PlcTypes.CPU_MODEL_iQR)
            {
                uintUnion AuxUint = new uintUnion((uint)(job.AddressObj.StartAddress + AddressOffset));
                resultBuffer.Add(AuxUint.LOUSHORT.LOBYTE);
                resultBuffer.Add(AuxUint.LOUSHORT.HIBYTE);
                resultBuffer.Add(AuxUint.HIUSHORT.LOBYTE);
                resultBuffer.Add(AuxUint.HIUSHORT.HIBYTE);
            }
            else
            {
                uintUnion AuxUint = new uintUnion((uint)(job.AddressObj.StartAddress + AddressOffset));
                resultBuffer.Add(AuxUint.LOUSHORT.LOBYTE);
                resultBuffer.Add(AuxUint.LOUSHORT.HIBYTE);
                resultBuffer.Add(AuxUint.HIUSHORT.LOBYTE);
            }

            // Data area code
            if (((MelsecQEthStation)job.Station).PlcType == MelsecQEthProtocol.PlcTypes.CPU_MODEL_iQR)
            {
                Aux.USHORT = (ushort)(job.AddressObj.DataAreaCode);
                resultBuffer.Add(Aux.LOBYTE);
                resultBuffer.Add(Aux.HIBYTE);
            }
            else
            {
                resultBuffer.Add((byte)(job.AddressObj.DataAreaCode));
            }

            // Number of device points.
            Aux.USHORT = (ushort)(PointCount);
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            // Data bytes.
            int j = 0;
            int i = 0;
            byte byteValue = 0;
            if (job.BitVariables)
            {
                // Bit variables written in bit units 
                if (job.CommandCode == (ushort)MelsecQCommandCode.BitUnits32bits || job.CommandCode == (ushort)MelsecQCommandCode.BitUnits)
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
                            resultBuffer.Add(byteValue);
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

                                resultBuffer.Add(byteValue);

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
                            resultBuffer.Add(Aux.LOBYTE);
                            resultBuffer.Add(Aux.HIBYTE);
                        }
                    }
                    else
                    {
                        for (i = 0, j = 0; i < PointCount; i++, j += 2)
                        {
                            resultBuffer.Add(jobdata[j]);
                            resultBuffer.Add(jobdata[j + 1]);
                        }
                    }
                }
            }
            else
            {
                byte byteAux = 0;
                // Single byte variable written in bit units 
                if (job.CommandCode == (ushort)MelsecQCommandCode.BitUnits32bits || job.CommandCode == (ushort)MelsecQCommandCode.BitUnits)
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
                        resultBuffer.Add(byteValue);
                    }
                }

                // WORD variables written in WORD units
                else
                {
                    for (i = 0; i < DataSize; i++)
                    {
                        resultBuffer.Add(jobdata[i]);
                    }
                }
            }

            Aux = new ushortUnion((ushort)(resultBuffer.Count - requestlengthStartIndex - 2));
            resultBuffer[requestlengthStartIndex] = Aux.LOBYTE;
            resultBuffer[requestlengthStartIndex + 1] = Aux.HIBYTE;

            buffer = resultBuffer.ToArray();

            return (uint)buffer.Count();
        }

        private bool AnswerOkDataArea(ref byte[] buffer, out ushort nErrCode)
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
                ushortUnion wErrorCode = new ushortUnion(buffer, MESSAGE_HEADER_LENGTH);
                if (wErrorCode.USHORT != 0)
                    nErrCode = wErrorCode.USHORT;
                else
                    nRetValue = true;
            }
            return (nRetValue);
        }

        private byte[] ParseReadDataDataArea(ref byte[] buffer, ushort nData, MelsecQEthCommJob job)
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

                if (job.CommandCode == (ushort)MelsecQCommandCode.BitUnits32bits || job.CommandCode == (ushort)MelsecQCommandCode.BitUnits)
                {

                    if ((job.PointCount / 2 + 11) > buffer.Length)
                    {
                        return (outBuffer);
                    }

                    outBuffer = new byte[job.PointCount];
                    for (i = 0; i < job.PointCount; i++)
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
                    for (i = 0; i < outBuffer.Length / 8; i++)
                    {
                        outBuffer[i] = buffer[11 + i];
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
                Array.Copy(buffer, 11, outBuffer, 0, nCopy);
                nCopiedData += nCopy;
            }

            return outBuffer;
        }
        #endregion

        #region Label Specific Methods
        private bool ExecuteJobListLabel(ref DriverErrorCodes conn, List<CommJob> list)
        {
            uint nrMaxJobs = ((MelsecQEthStation)list[0].Station).GetNrMaxJobsRuntimeAggregationLimits(list[0].ReadRequest() ? MelsecQEthProtocol.RuntimeAggregationLimits.Read : MelsecQEthProtocol.RuntimeAggregationLimits.Write);
            if (list.Count > nrMaxJobs)
            {
                while (list.Count > nrMaxJobs)
                {
                    RemovePendingJob(list[0]);
                    list.RemoveAt(0);
                }
            }
            foreach (CommJob job in list)
                base.ExecuteJob(ref conn, job);

            uint requestLength = PrepareRequestFrameLabel(list, out byte[] requestFrame);
            if (requestLength == 0)
            {
                foreach (CommJob job in list)
                    RemovePendingJob(job);
                return (false);
            }

            // Send the request frame
            if (!DeviceWrite(requestFrame, requestLength))
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return (false);
            }

            // Start the procedure for receiving the device reply
            if (!BeginDeviceRead(MESSAGE_HEADER_LENGTH))
            {
                conn = DriverErrorCodes.ErrorTimeOut;
                return (false);
            }

            return (true);
        }

        private bool ProcessNewDataListLabel(DriverErrorCodes conn, List<CommJob> list)
        {
            List<byte> receiveBuffer = new List<byte>();
            lock (lockThreadObject)
            {
                receiveBuffer.AddRange(ReceiveBuffer);
                ReceiveBuffer.Clear();
            }

            // Check the subheader            
            Int32 minLen = 2;
            if (receiveBuffer.Count < minLen || receiveBuffer.Count < MESSAGE_HEADER_LENGTH)
            {
                foreach (CommJob job in list)
                    OnJobExecuted(new ExecutedJobArgs { Job = job, ErrorCode = (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame });
                DeviceClose(); //Flush();
                return false;
            }

            bool read = (list.Count > 0 && ((MelsecQEthCommJob)list[0]).IsRead);

            DriverErrorCodes errorCode = DriverErrorCodes.ErrorNoError;
            ushortUnion dim = new ushortUnion(receiveBuffer, MESSAGE_HEADER_LENGTH - 2);

            byte[] pdu = new byte[MESSAGE_HEADER_LENGTH + dim.USHORT];
            if (dim.USHORT != 0)
            {
                byte[] pduData = new byte[dim.USHORT];

                int BytesToCopy = receiveBuffer.Count;
                if (BytesToCopy > pdu.Length)
                    BytesToCopy = pdu.Length;

                receiveBuffer.CopyTo(0, pdu, 0, BytesToCopy);
                ushort numberOfReceivedBytes = (ushort)DeviceReadSynchronous(pduData, (uint)dim.USHORT);

                if (numberOfReceivedBytes == (uint)dim.USHORT)
                {
                    pduData.CopyTo(pdu, MESSAGE_HEADER_LENGTH);
                    if (!AnswerOkLabel(ref pdu, out ushort nErrCode))
                    {
                        switch (nErrCode)
                        {
                            case (ushort)MelsecQEthProtocol.MelsecQErrorCodes.ErrorWrongReply:
                                errorCode = (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorWrongReply;
                                break;
                            // Unrecognized abnormal code
                            default:
                                errorCode = (DriverErrorCodes)(MelsecQEthProtocol.MelsecQErrorCodes.ErrorFromDevice + nErrCode);
                                // if the plc return an error of too many labels reduce the nr of jobs aggregable at runtime, remove the jobs from list (without putting in error) and retry to execute it 
                                switch ((MelsecQEthProtocol.MelsecQErrorCodes)nErrCode)                                
                                {                                    
                                    case MelsecQEthProtocol.MelsecQErrorCodes.ErrorLabelTooManyRequest:
                                        ((MelsecQEthStation)list[0].Station).SetNrMaxJobsRuntimeAggregationLimits((read ? MelsecQEthProtocol.RuntimeAggregationLimits.Read : MelsecQEthProtocol.RuntimeAggregationLimits.Write), list.Count);
                                        // keeo jobs alive --> exeeding jobs from list will be removed in the executedJobs
                                        return false;
                                    case MelsecQEthProtocol.MelsecQErrorCodes.ErrorLabelDoesNotExist:
                                    case MelsecQEthProtocol.MelsecQErrorCodes.ErrorLabelsDoesNotExist:
                                        // set all processed jobs in BadNotFoundStateUncertain (will be process individually) instead of put all jobs in error : error is cumulative
                                        // 1 job in error should be consider tag BADNOTFOUND
                                        if (list.Count > 1)
                                        {
                                            while (list.Count > 0)
                                            {
                                                System.Diagnostics.Debug.WriteLine("--DEBUG -- Job={0}, ErrorLabelDoesNotExist BadNotFoundStateUncertain={1} in error ", ((MelsecQEthCommJob)list[0]).AddressObj.Address, true, conn);
                                                ((MelsecQEthCommJob)list[0]).BadNotFoundStateUncertain = true;
                                                RemovePendingJob(list[0]);
                                                list.RemoveAt(0);
                                            }
                                            return false;
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                    else
                    {
                        pdu = pdu.Skip(MESSAGE_HEADER_LENGTH + 2).Take(pdu.Length - MESSAGE_HEADER_LENGTH - 2).ToArray();
                    }
                }
                else
                {
                    errorCode = (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame;
                }
            }

            int pduIndex = 0;
            int nrPoints = (read ? ParseReadDataLabelGetNrPoints(pdu, ref pduIndex) : 0);

            foreach (MelsecQEthCommJob job in list)
            {
                job.AlreadyExchanged = true;
                job.BadNotFoundStateUncertain = false;
                bool executeJob = true;
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = job;
                eJob.ErrorCode = errorCode;
                if (errorCode == DriverErrorCodes.ErrorNoError)
                {
                    if (read)
                    {
                        if (nrPoints == list.Count)
                        {
                            eJob.ErrorCode = ParseReadDataLabel(job, ref pduIndex, pdu, out byte[] Answer);
                            if (eJob.ErrorCode == DriverErrorCodes.ErrorNoError)
                                eJob.Values = Answer;                            
                        }
                        else
                        {
                            // request nr of values differ from list of jobs to process
                            eJob.ErrorCode = (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame;
                        }
                    }
                }

                // string settings
                if (eJob.ErrorCode == DriverErrorCodes.ErrorNoError)
                {
                    if (!job.IsRead && !job.AlreadyWritten)
                        job.AlreadyWritten = true;

                    // string settings is get requesting at runtime when is set = 0 in the dynamic link : after get settings, keep job alive and execute it
                    if (job.StringSettingsAutoDetect == MelsecQEthProtocol.StringSettingsAutoDetectStates.Requested)
                    {
                        job.StringSettingsAutoDetect = MelsecQEthProtocol.StringSettingsAutoDetectStates.RequestedDone;
                        // keep job alive
                        executeJob = false;
                    }
                }

                if (executeJob)
                    OnJobExecuted(eJob);
            }

            if (errorCode == (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame)
                DeviceClose();

            return true;
        }

        private int ParseReadDataLabelGetNrPoints(byte[] pdu, ref int pduIndex)
        {
            if ((pduIndex + 2) > pdu.Length)
                return 0;

            pduIndex += 2;

            return BitConverter.ToUInt16(pdu, 0);
        }

        private DriverErrorCodes ParseReadDataLabel(MelsecQEthCommJob job, ref int pduIndex, byte[] pdu, out byte[] data)
        {
            if (!((MelsecQEthCommJob)job).ArrayVariables || (((MelsecQEthCommJob)job).StringSettingsAutoDetect == MelsecQEthProtocol.StringSettingsAutoDetectStates.Requested))
                return ParseReadDataLabelStandard(job, ref pduIndex, pdu, out data);
            else
                return ParseReadDataLabelArray(job, ref pduIndex, pdu, out data);
        }
        
        private DriverErrorCodes ParseReadDataLabelStandard(MelsecQEthCommJob job, ref int pduIndex, byte[] pdu, out byte[] data)
        {
            data = null;
            //type of data read
            if ((pduIndex + 1) > pdu.Length)
                return (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame;
            byte dataTypeID = pdu[pduIndex];
            pduIndex++;

            //spare
            if ((pduIndex + 1) > pdu.Length)
                return (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame;
            byte spare = pdu[pduIndex];
            pduIndex++;

            //size of data 
            if ((pduIndex + 2) > pdu.Length)
                return (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame;
            ushort readDataLength = BitConverter.ToUInt16(pdu, pduIndex);
            pduIndex += 2;

            // data
            if ((pduIndex + readDataLength) > pdu.Length)
                return (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame;
            data = pdu.Skip(pduIndex).Take(readDataLength).ToArray();
            pduIndex += readDataLength;
            
            if (MelsecQEthProtocol.IsStringJob(job))
            {
                if (job.StringSettingsAutoDetect == MelsecQEthProtocol.StringSettingsAutoDetectStates.Requested)
                {
                    switch (dataTypeID)
                    {
                        case (byte)MelsecQEthProtocol.DataTypeID.String:                        
                            job.SetStringSettings(readDataLength - MelsecQEthProtocol.STRING_TERMINATOR_NULL_CHARS, false, job.TagsList[0].TagNode.ArrayDimension);
                            break;
                        case (byte)MelsecQEthProtocol.DataTypeID.WString:
                            job.SetStringSettings(readDataLength - MelsecQEthProtocol.STRING_TERMINATOR_NULL_CHARS, true, job.TagsList[0].TagNode.ArrayDimension);
                            break;
                        default:
                            return (DriverErrorCodes)((int)MelsecQEthProtocol.MelsecQErrorCodes.ErrorFromDevice + (int)MelsecQEthProtocol.MelsecQErrorCodes.ErrorLabelSizeMismatch);
                            break;
                    }                                            
                }
                else
                {
                    if (readDataLength != MelsecQEthProtocol.GetStringJobSize(job))
                        return (DriverErrorCodes)((int)MelsecQEthProtocol.MelsecQErrorCodes.ErrorFromDevice + (int)MelsecQEthProtocol.MelsecQErrorCodes.ErrorLabelSizeMismatch);
                }
            }

            return DriverErrorCodes.ErrorNoError;
        }

        private DriverErrorCodes ParseReadDataLabelArray(MelsecQEthCommJob job, ref int pduIndex, byte[] pdu, out byte[] data)
        {
            data = null;
            //type of data read
            if ((pduIndex + 1) > pdu.Length)
                return (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame;
            byte dataTypeID = pdu[pduIndex];
            pduIndex ++;
                        
            //read unit specification
            if ((pduIndex + 1) > pdu.Length)
                return (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame;
            ushort readUnitSpecification = pdu[pduIndex];
            pduIndex += 1;

            //size of data 
            if ((pduIndex + 2) > pdu.Length)
                return (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame;
            ushort readDataLength = BitConverter.ToUInt16(pdu, pduIndex);
            pduIndex += 2;

            // bits return packet in a "block" of word = 2bytes
            if (job.TagsList[0].TagNode.DataType.IdType == Opc.Ua.IdType.Numeric && (uint)job.TagsList[0].TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.Boolean)
            {
                int nrWord = Math.DivRem(readDataLength, 16, out int rest);
                if (rest > 0)
                    nrWord++;

                readDataLength = (ushort)(nrWord * 2);
            }
            
            // data
            if ((pduIndex + readDataLength) > pdu.Length)
                return (DriverErrorCodes)MelsecQEthProtocol.MelsecQErrorCodes.ErrorCodeIncompleteFrame;
            data = pdu.Skip(pduIndex).Take(readDataLength).ToArray();
            pduIndex += readDataLength;

            return DriverErrorCodes.ErrorNoError;
        }

        private uint PrepareRequestFrameLabel(List<CommJob> list, out byte[] buffer)
        {
            uint byteCount = 0;
            if (list[0].ReadRequest() || (((MelsecQEthCommJob)list[0]).StringSettingsAutoDetect == MelsecQEthProtocol.StringSettingsAutoDetectStates.Requested))
                byteCount = PrepareReadRequestFrameLabel(list, out buffer);
            else
                byteCount = PrepareWriteRequestFrameLabel(list, out buffer);

            return (byteCount);
        }

        private uint PrepareReadRequestFrameLabel(List<CommJob> list, out byte[] buffer)
        {
            if (!((MelsecQEthCommJob)list[0]).ArrayVariables || (((MelsecQEthCommJob)list[0]).StringSettingsAutoDetect == MelsecQEthProtocol.StringSettingsAutoDetectStates.Requested))
                return PrepareReadRequestFrameLabelStandard(list, out buffer);
            else
                return PrepareReadRequestFrameLabelArray(list, out buffer);
        }

        private uint PrepareReadRequestFrameLabelStandard(List<CommJob> list, out byte[] buffer)
        {
            List<byte> resultBuffer = new List<byte>();

            MelsecQEthCommJob commonJob = list[0] as MelsecQEthCommJob;

            resultBuffer.Add(0x50); // Subheader (low byte).
            resultBuffer.Add(0); // Subheader (high byte).
            resultBuffer.Add((byte)(commonJob.GetStationNetworkNumber() & 0xff)); // Network num.
            resultBuffer.Add((byte)(commonJob.GetStationPcNumber() & 0xff)); // PC number.

            resultBuffer.AddRange(MelsecQEthProtocol.GetModuleTargetBuffer(commonJob.CpuTarget));

            // Request length. --> will be set at the end
            int requestlengthStartIndex = resultBuffer.Count;
            resultBuffer.Add(0);
            resultBuffer.Add(0);

            // CPU monitoring timer (timeout).
            ushortUnion Aux = new ushortUnion((ushort)(Timeout / 250));
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            // Label Read Command
            resultBuffer.Add(0x1C); // Low byte.
            resultBuffer.Add(0x04); // High byte.

            // Subcommand.
            resultBuffer.Add(0x0);
            resultBuffer.Add(0x0);

            // nr elements to read
            Aux = new ushortUnion((ushort)list.Count);
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            // Number of abbreviated nr elements to read
            resultBuffer.Add(0x0);
            resultBuffer.Add(0x0);

            foreach (MelsecQEthCommJob job in list)
            {
                job.IsRead = true;

                string address = job.AddressObj.Address;
                // to get string settings about array request 1st element
                if (MelsecQEthProtocol.IsStringJob(job) && job.ArrayVariables && job.StringSettingsAutoDetect == MelsecQEthProtocol.StringSettingsAutoDetectStates.Requested)
                    address = string.Format("{0}[0]", job.AddressObj.Address);

                Aux = new ushortUnion((ushort)address.Length);
                resultBuffer.Add(Aux.LOBYTE);
                resultBuffer.Add(Aux.HIBYTE);

                // label address
                resultBuffer.AddRange(ASCIIEncoding.Unicode.GetBytes(address));
            }

            Aux = new ushortUnion((ushort)(resultBuffer.Count - requestlengthStartIndex - 2));
            resultBuffer[requestlengthStartIndex] = Aux.LOBYTE;
            resultBuffer[requestlengthStartIndex + 1] = Aux.HIBYTE;

            buffer = resultBuffer.ToArray();

            return (uint)buffer.Count();
        }

        
        private uint PrepareReadRequestFrameLabelArray(List<CommJob> list, out byte[] buffer)
        {
            List<byte> resultBuffer = new List<byte>();

            MelsecQEthCommJob commonJob = list[0] as MelsecQEthCommJob;

            resultBuffer.Add(0x50); // Subheader (low byte).
            resultBuffer.Add(0); // Subheader (high byte).
            resultBuffer.Add((byte)(commonJob.GetStationNetworkNumber() & 0xff)); // Network num.
            resultBuffer.Add((byte)(commonJob.GetStationPcNumber() & 0xff)); // PC number.

            resultBuffer.AddRange(MelsecQEthProtocol.GetModuleTargetBuffer(commonJob.CpuTarget));

            // Request length. --> will be set at the end
            int requestlengthStartIndex = resultBuffer.Count;
            resultBuffer.Add(0);
            resultBuffer.Add(0);

            // CPU monitoring timer (timeout).
            ushortUnion Aux = new ushortUnion((ushort)(Timeout / 250));
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            // Array Label Read Command
            resultBuffer.Add(0x1A); // Low byte.
            resultBuffer.Add(0x04); // High byte.

            // Subcommand.
            resultBuffer.Add(0x0);
            resultBuffer.Add(0x0);

            // nr elements to read
            Aux = new ushortUnion((ushort)list.Count);
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            // Number of abbreviated nr elements to read
            resultBuffer.Add(0x0);
            resultBuffer.Add(0x0);

            foreach (MelsecQEthCommJob job in list)
            {
                job.IsRead = true;                

                Aux = new ushortUnion((ushort)job.AddressObj.Address.Length);
                resultBuffer.Add(Aux.LOBYTE);
                resultBuffer.Add(Aux.HIBYTE);

                // label address
                resultBuffer.AddRange(ASCIIEncoding.Unicode.GetBytes(job.AddressObj.Address));                

                // unit specification
                resultBuffer.Add(MelsecQEthProtocol.GetArrayLabelUnitSpecification(job.TagsList[0]));
                
                // Fixed value
                resultBuffer.Add(0);

                // Read array data length                
                // bits return packet in a "block" of word = 2bytes
                if (job.TagsList[0].TagNode.DataType.IdType == Opc.Ua.IdType.Numeric)
                {
                    switch ((uint)job.TagsList[0].TagNode.DataType.Identifier)
                    {
                        case (uint)Opc.Ua.DataTypes.Boolean:
                            Aux = new ushortUnion((ushort)job.TagsList[0].TagNode.ArrayDimension);
                            break;
                        case (uint)Opc.Ua.DataTypes.String:
                            Aux = new ushortUnion((ushort)(job.TotalJobSize + (job.TagsList[0].TagNode.ArrayDimension + MelsecQEthProtocol.STRING_TERMINATOR_NULL_CHARS)));
                            break;
                        default: // size in bytes
                            Aux = new ushortUnion((ushort)job.TotalJobSize);
                            break;
                    }
                }

                resultBuffer.Add(Aux.LOBYTE);
                resultBuffer.Add(Aux.HIBYTE);
            }

            Aux = new ushortUnion((ushort)(resultBuffer.Count - requestlengthStartIndex - 2));
            resultBuffer[requestlengthStartIndex] = Aux.LOBYTE;
            resultBuffer[requestlengthStartIndex + 1] = Aux.HIBYTE;

            buffer = resultBuffer.ToArray();

            return (uint)buffer.Count();
        }

        private uint PrepareWriteRequestFrameLabel(List<CommJob> list, out byte[] buffer)
        {
            if (((MelsecQEthCommJob)list[0]).ArrayVariables)
                return PrepareWriteRequestFrameLabelArray(list, out buffer);
            else
                return PrepareWriteRequestFrameLabelStandard(list, out buffer);
        }

        private uint PrepareWriteRequestFrameLabelStandard(List<CommJob> list, out byte[] buffer)
        {
            MelsecQEthCommJob commonJob = list[0] as MelsecQEthCommJob;

            List<byte> resultBuffer = new List<byte>();

            resultBuffer.Add(0x50); // Subheader (low byte).
            resultBuffer.Add(0); // Subheader (high byte).
            resultBuffer.Add((byte)(commonJob.GetStationNetworkNumber() & 0xff)); // Network num.
            resultBuffer.Add((byte)(commonJob.GetStationPcNumber() & 0xff)); // PC number.

            resultBuffer.AddRange(MelsecQEthProtocol.GetModuleTargetBuffer(commonJob.CpuTarget));
            
            // Request length. --> will be set at the end
            int requestlengthStartIndex = resultBuffer.Count;
            resultBuffer.Add(0);
            resultBuffer.Add(0);

            // CPU monitoring timer (timeout).
            ushortUnion Aux = new ushortUnion((ushort)(Timeout / 250));
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            // Label random write command.
            resultBuffer.Add(0x1B); // Low byte.
            resultBuffer.Add(0x14); // High byte.

            // Subcommand.
            resultBuffer.Add(0); // Low byte.
            resultBuffer.Add(0); // High byte.

            // nr elements to read
            Aux = new ushortUnion((ushort)list.Count);
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            // Number of abbreviated nr elements to write
            resultBuffer.Add(0x0);
            resultBuffer.Add(0x0);

            byte[] writeData = new byte[0];
            foreach (MelsecQEthCommJob job in list)
            {
                job.IsRead = false;
                Aux = new ushortUnion((ushort)job.AddressObj.Address.Length);
                resultBuffer.Add(Aux.LOBYTE);
                resultBuffer.Add(Aux.HIBYTE);

                // label address
                resultBuffer.AddRange(ASCIIEncoding.Unicode.GetBytes(job.AddressObj.Address));

                //prepare a write request
                object objectData = null;
                job.GetJobData(ref objectData);
                if (job.GetTagListOnWritingCount() != 0)
                    writeData = (byte[])objectData;

                // nr bytes to write
                Aux = new ushortUnion((ushort)writeData.Length);

                resultBuffer.Add(Aux.LOBYTE);
                resultBuffer.Add(Aux.HIBYTE);
                // data to write
                resultBuffer.AddRange(writeData);
            }

            Aux = new ushortUnion((ushort)(resultBuffer.Count - requestlengthStartIndex - 2));
            resultBuffer[requestlengthStartIndex] = Aux.LOBYTE;
            resultBuffer[requestlengthStartIndex + 1] = Aux.HIBYTE;

            buffer = resultBuffer.ToArray();

            return (uint)buffer.Count();
        }

        private uint PrepareWriteRequestFrameLabelArray(List<CommJob> list, out byte[] buffer)
        {
            MelsecQEthCommJob commonJob = list[0] as MelsecQEthCommJob;

            List<byte> resultBuffer = new List<byte>();

            resultBuffer.Add(0x50); // Subheader (low byte).
            resultBuffer.Add(0); // Subheader (high byte).
            resultBuffer.Add((byte)(commonJob.GetStationNetworkNumber() & 0xff)); // Network num.
            resultBuffer.Add((byte)(commonJob.GetStationPcNumber() & 0xff)); // PC number.

            resultBuffer.AddRange(MelsecQEthProtocol.GetModuleTargetBuffer(commonJob.CpuTarget));

            // Request length. --> will be set at the end
            int requestlengthStartIndex = resultBuffer.Count;
            resultBuffer.Add(0);
            resultBuffer.Add(0);

            // CPU monitoring timer (timeout).
            ushortUnion Aux = new ushortUnion((ushort)(Timeout / 250));
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            // Array Label random write command.
            resultBuffer.Add(0x1A); // Low byte.
            resultBuffer.Add(0x14); // High byte.

            //// Subcommand.
            resultBuffer.Add(0); // Low byte.
            resultBuffer.Add(0); // High byte.

            // nr elements to write
            Aux = new ushortUnion((ushort)list.Count);
            resultBuffer.Add(Aux.LOBYTE);
            resultBuffer.Add(Aux.HIBYTE);

            // Number of abbreviated nr elements to read
            resultBuffer.Add(0x0);
            resultBuffer.Add(0x0);

            byte[] writeData = new byte[0];
            foreach (MelsecQEthCommJob job in list)
            {
                job.IsRead = false;
                Aux = new ushortUnion((ushort)job.AddressObj.Address.Length);
                resultBuffer.Add(Aux.LOBYTE);
                resultBuffer.Add(Aux.HIBYTE);

                // label address
                resultBuffer.AddRange(ASCIIEncoding.Unicode.GetBytes(job.AddressObj.Address));

                // unit specification
                resultBuffer.Add(MelsecQEthProtocol.GetArrayLabelUnitSpecification(job.TagsList[0]));

                // Fixed value
                resultBuffer.Add(0);

                //prepare a write request
                object objectData = null;
                job.GetJobData(ref objectData);
                if (job.GetTagListOnWritingCount() != 0)
                    writeData = (byte[])objectData;

                // nr bytes to write
                Aux = new ushortUnion((ushort)writeData.Length);                
                // bits return packet in a "block" of word = 2bytes
                if (job.TagsList[0].TagNode.DataType.IdType == Opc.Ua.IdType.Numeric)
                {
                    switch ((uint)job.TagsList[0].TagNode.DataType.Identifier)
                    {
                        case (uint)Opc.Ua.DataTypes.Boolean:
                            Aux = new ushortUnion((ushort)job.TagsList[0].TagNode.ArrayDimension);
                            break;
                    }
                }                
                resultBuffer.Add(Aux.LOBYTE);
                resultBuffer.Add(Aux.HIBYTE);

                // data to write
                resultBuffer.AddRange(writeData);
            }

            Aux = new ushortUnion((ushort)(resultBuffer.Count - requestlengthStartIndex - 2));
            resultBuffer[requestlengthStartIndex] = Aux.LOBYTE;
            resultBuffer[requestlengthStartIndex + 1] = Aux.HIBYTE;

            buffer = resultBuffer.ToArray();

            return (uint)buffer.Count();
        }

        private bool AnswerOkLabel(ref byte[] buffer, out ushort nErrCode)
        {
            bool nRetValue = false;
            nErrCode = 0;
            if ((buffer.Length < MESSAGE_HEADER_LENGTH + 2) || (buffer[0] != 0xD0) || (buffer[1] != 0))
            {
                nErrCode = (ushort)MelsecQEthProtocol.MelsecQErrorCodes.ErrorWrongReply;
            }
            else
            {
                ushortUnion wErrorCode = new ushortUnion(buffer, MESSAGE_HEADER_LENGTH);
                if (wErrorCode.USHORT != 0)
                    nErrCode = wErrorCode.USHORT;
                else
                    nRetValue = true;
            }
            return (nRetValue);
        }

        private bool GetReadWriteListLimitateLabel(MelsecQEthCommJob j, bool write, ref MelsecQEthProtocol.ReadWriteListLimitateSize frameSize)
        {
            if (frameSize.IsEmpty())
                GetPlcInitialFrameSizeLabel(j, write, ref frameSize);

            GetPlcJobRequestResponseSizeLabel(j, write, out uint requestSize, out uint responseSize);

            // add job if request size fit protocol size or is the only job to schedule 
            bool add = (((frameSize.TotalRequestSize + requestSize < frameSize.ProtocolSize) && (frameSize.TotalResponseSize + responseSize < frameSize.ProtocolSize) && frameSize.TotalNrJobs < frameSize.MaxNrJobs) || frameSize.TotalNrJobs == 0);
            if (add)
            {
                frameSize.TotalRequestSize += requestSize;
                frameSize.TotalResponseSize += responseSize;
                frameSize.TotalNrJobs++;
            }
            return add;
        }

        private void GetPlcInitialFrameSizeLabel(MelsecQEthCommJob j, bool write, ref MelsecQEthProtocol.ReadWriteListLimitateSize frameSize)
        {
            frameSize.ProtocolSize = j.GetMaxJobSize();
            if (j.AlreadyExchanged)
                // set the nr of max of label (calculated at runtime by the result if PLC) that can be insert in the frame
                frameSize.MaxNrJobs = ((MelsecQEthStation)j.Station).GetNrMaxJobsRuntimeAggregationLimits((write ? MelsecQEthProtocol.RuntimeAggregationLimits.Write : MelsecQEthProtocol.RuntimeAggregationLimits.Read));
            else
                frameSize.MaxNrJobs = ((MelsecQEthStation)j.Station).GetNrMaxJobsRuntimeAggregationLimits(MelsecQEthProtocol.RuntimeAggregationLimits.NotExchanged);

            frameSize.TotalRequestSize = 19;
            frameSize.TotalResponseSize = 13;

            if (write)
            {
                uint writeTotalRequestSize = 19;
                uint writeTotalResponseSize = 13;                

                frameSize.TotalRequestSize = (frameSize.TotalRequestSize > writeTotalResponseSize ? frameSize.TotalRequestSize : writeTotalResponseSize);
                frameSize.TotalResponseSize = (frameSize.TotalResponseSize > writeTotalRequestSize ? frameSize.TotalResponseSize : writeTotalRequestSize);
            }
        }

        private void GetPlcJobRequestResponseSizeLabel(MelsecQEthCommJob j, bool write, out uint requestSize, out uint responseSize)
        {
            requestSize = (uint)(2 + j.AddressObj.GetLabelAddressLength());
            responseSize = (uint)(2 + 2 + j.TotalJobSize);

            if (write)
            {
                uint requestLabelWriteRequest = (uint)(2 + (j.AddressObj.GetLabelAddressLength()) + 2 + j.GetLabelTotalJobSize());
                uint requestLabelWriteResponse = 0;
                
                requestSize = (requestSize > requestLabelWriteRequest ? requestSize : requestLabelWriteRequest);
                responseSize = (responseSize > requestLabelWriteResponse ? responseSize : requestLabelWriteResponse);
            }
        }

        /// <summary>
        /// The ReadCPUModelName read the name of PLC and the model type
        /// </summary>
        /// <param name="station"></param>
        /// <param name="error"></param>
        /// <returns>Return a bulean value (success or failure)</returns>
        public bool ReadCPUModelName(MelsecQEthStation station, out string error)
        {
            error = string.Empty;
            byte[] requestFrame = new byte[15];
            uint requestLength = PrepareRequestFrameReadCPUModelName(ref requestFrame, station);
            if (requestLength == 0)
            {
                return (false);
            }

            if (!IsDeviceOpen())
            {
                if (!DeviceOpen())
                    return (false);
            }

            // Send the request frame
            if (!DeviceWrite(requestFrame, requestLength))
            {
                return (false);
            }

            // Start the procedure for receiving the device reply
            if (!BeginDeviceRead(MESSAGE_HEADER_LENGTH))
            {
                return (false);
            }

            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            WaitNewDataEvent(ref conn);

            if (conn != DriverErrorCodes.ErrorNoError)
            {
                DeviceClose(); //Flush();
                return false;
            }

            List<byte> receiveBuffer = new List<byte>();
            lock (lockThreadObject)
            {
                receiveBuffer.AddRange(ReceiveBuffer);
                ReceiveBuffer.Clear();
            }

            // Check the subheader            
            Int32 minLen = 2;
            if (receiveBuffer.Count < minLen || receiveBuffer.Count < MESSAGE_HEADER_LENGTH)
            {
                DeviceClose(); //Flush();
                return false;
            }


            //ushort nErrCode = 0xffff;
            ushortUnion dim = new ushortUnion(receiveBuffer, MESSAGE_HEADER_LENGTH - 2);

            byte[] pdu = new byte[MESSAGE_HEADER_LENGTH + dim.USHORT];
            if (dim.USHORT != 0)
            {
                byte[] pduData = new byte[dim.USHORT];

                int BytesToCopy = receiveBuffer.Count;
                if (BytesToCopy > pdu.Length)
                    BytesToCopy = pdu.Length;

                receiveBuffer.CopyTo(0, pdu, 0, BytesToCopy);
                ushort numberOfReceivedBytes = (ushort)DeviceReadSynchronous(pduData, (uint)dim.USHORT);

                if (numberOfReceivedBytes == (uint)dim.USHORT)
                {
                    pduData.CopyTo(pdu, MESSAGE_HEADER_LENGTH);
                    if (!AnswerOkDataArea(ref pdu, out ushort nErrCode))
                    {
                        switch (nErrCode)
                        {
                            // Wrong PC number
                            case 0xffff:
                                return false;
                                break;

                            // Unrecognized abnormal code
                            default:
                                return false;
                                break;
                        }
                    }
                    string plcName = Encoding.UTF8.GetString(pduData, 2, 16);
                    ushortUnion code = new ushortUnion(pduData, 18);
                    error = String.Format(Properties.Resources.ConnectedDevice, plcName.Trim());
                }
                else
                {
                    DeviceClose(); //Flush();
                    return false;
                }
            }
            DeviceClose(); //Flush();
            return (true);
        }

        /// <summary>
        /// The  PrepareRequestFrameReadCPUModelName  Prepare the frama for read tre CPU name and model
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="station"></param>
        /// <returns>Return the legth of the message to send</returns>
        private uint PrepareRequestFrameReadCPUModelName(ref byte[] buffer, MelsecQEthStation station)
        {
            uint byteCount = 0;

            buffer[byteCount++] = 0x50; // Subheader (low byte).
            buffer[byteCount++] = 0; // Subheader (high byte).
            buffer[byteCount++] = (byte)(((MelsecQEthStation)station).NetworkNumber & 0xff); // Network num.
            buffer[byteCount++] = (byte)(((MelsecQEthStation)station).PcNumber & 0xff); // PC number.

            buffer[byteCount++] = 0xFF; // Dest. mod. I/O N. (l. byte).
            buffer[byteCount++] = 0x03; // Dest. mod. I/O N. (h. byte).
            buffer[byteCount++] = 0; // Dest. module station number.

            buffer[byteCount++] = 0x06; // Request length (low byte).
            buffer[byteCount++] = 0x00; // Request length (high byte).

            // CPU monitoring timer (timeout).
            ushortUnion Aux = new ushortUnion((ushort)(Timeout / 250));
            buffer[byteCount++] = Aux.LOBYTE;
            buffer[byteCount++] = Aux.HIBYTE;

            // Batch read command.
            buffer[byteCount++] = 0x01; // Low byte.
            buffer[byteCount++] = 0x01; // High byte.

            // Subcommand.
            buffer[byteCount++] = 0x00;
            buffer[byteCount++] = 0x00;

            return (byteCount);
        }
        #endregion
    }
}

