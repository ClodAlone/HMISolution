using System;
using System.Collections.Generic;
using System.Text;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using SerialDriverCodeBaseEx;
using System.Threading;
using DriverCodeBaseEx.Enumerators;
using System.Linq;
using System.Threading.Tasks;
using DevExpress.Office.Utils;
using DevExpress.Xpo.DB;
using System.Reflection.Emit;

namespace ROCDriver
{
    public class ROCDriverChannel : ChannelList, IDisposable
    {
        #region Constructors

        /// <summary>
        /// Initializes the Channel object.
        /// </summary>
        public ROCDriverChannel(CommunicationDriver commdriver, ROCDriverChannelSettings settings)
            : base(commdriver, settings)
        {
            _ChannelType = settings.ChannelType;
            _HostAddress = settings.HostAddress;
            _HostGroup = settings.HostGroup;
            _CheckConnectionFrequency = settings.CheckConnectionFrequency;
            switch (_ChannelType)
            {
                case ChannelTypes.Serial:
                    ROCDriverSerialChannelSettings SerialSetting = new ROCDriverSerialChannelSettings();
                    SerialSetting.CopyProperties(settings);
                    ChildChannel = new ROCDriverSerialChannel(commdriver, SerialSetting);
                    ((ROCDriverSerialChannel)ChildChannel).ParentChannel = this;
                    USBconnection = settings.USBconnection;
                    ((ROCDriverSerialChannel)ChildChannel).SetStateCommandVariable(channelStateCommandVariable);
                break;
                case ChannelTypes.Socket:
                    ROCDriverTcpChannelSettings TcpSetting = new ROCDriverTcpChannelSettings();
                    TcpSetting.CopyProperties(settings);
                    ChildChannel = new ROCDriverTcpChannel(commdriver, TcpSetting);
                    ((ROCDriverTcpChannel)ChildChannel).ParentChannel = this;
                    ((ROCDriverTcpChannel)ChildChannel).SetStateCommandVariable(channelStateCommandVariable);
                    break;
            }
           
        }

        #endregion

        #region Elements
        public bool TestConnection = false;
        #endregion

        #region Methods

        public void OnJobExecuted(ExecutedJobArgs e)
        {
            DiagnLastTaskRxBytes += e.RxBytes;
            DiagnLastTaskTxBytes += e.TxBytes;

            base.OnJobExecuted(e);
        }

        public byte[] GetNextDataParameter(ROCDriverCommJob j, byte[] pdu, ref int pduIndex)
        {
            //// Check the TLP address of the data point: it must match the one assigned to the job
            //if ((pdu[pduIndex] != j.PointType) || (pdu[pduIndex+1] != j.LogicalNumber) || (pdu[pduIndex+2] != j.Parameter))
            //{
            //    return (null);
            //}

            // Calculate the data size
            uint byteSize = ROCDriverProtocol.GetJobDataSize(j);
            if (byteSize == 0)
            {
                return (null);
            }

            // Check the length of the remaining part of the PDU 
            if ((pdu.Length - ROCDriverProtocol.CRCLength) < (ROCDriverProtocol.TLPAddressLength + byteSize + pduIndex))
            {
                return (null);
            }

            byte[] parameterData = new byte[ROCDriverProtocol.TLPAddressLength + byteSize];
            Array.Copy(pdu, pduIndex, parameterData, 0, ROCDriverProtocol.TLPAddressLength + byteSize);
            pduIndex += (int)(byteSize + ROCDriverProtocol.TLPAddressLength);

            return (parameterData);
        }

        #endregion

        #region Connection's check timer management

        private bool checkConnectionCyclically = false;

        private List<Station> GetDevicesToBeChecked()
        {
            List<Station> list = new List<Station>();

            foreach (ROCDriverStation s in CommDriver.GetChannelStations(this))
            {
                if (s.ConnectionMustBeChecked(CheckConnectionFrequency))
                    list.Add(s);
            }

            return list;
        }

        List<Station> stationsToCheck = null;
        protected override bool CyclicCheck()
        {
            stationsToCheck = GetDevicesToBeChecked();
            // if no station need to be checked, exit and wait next timer execution (60sec) without stoping Scheduler/Jobs execution
            if (stationsToCheck == null || stationsToCheck.Count == 0)
                return true;

            if (_ChannelType == ChannelTypes.Socket)
            {
                ((ROCDriverTcpChannel)ChildChannel).CheckDevicesConnection(stationsToCheck);
            }
            else
            {
                ((ROCDriverSerialChannel)ChildChannel).CheckDevicesConnection(stationsToCheck);
            }
            return true;
        }

        #endregion

        #region Properties

        private ChannelTypes _ChannelType;
        public ChannelTypes ChannelType
        {
            get { return _ChannelType; }
            set
            {
                _ChannelType = value;
            }
        }

        /// <summary>   Connection Check Frequency. </summary>
        private uint _CheckConnectionFrequency;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the frequency of the connection check. </summary>
        ///
        /// <value> The frequency of the connection check. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint CheckConnectionFrequency
        {
            get
            {
                return _CheckConnectionFrequency;
            }
            set
            {
                _CheckConnectionFrequency = value;
            }
        }

        /// <summary>   Host Address (Unit). </summary>
        private Byte _HostAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel host address. </summary>
        ///
        /// <value> The TCP channel host address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Byte HostAddress
        {
            get
            {
                return _HostAddress;
            }
            set
            {
                _HostAddress = value;
            }
        }

        /// <summary>   Host Group. </summary>
        private Byte _HostGroup;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel host group. </summary>
        ///
        /// <value> The TCP channel host group. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Byte HostGroup
        {
            get
            {
                return _HostGroup;
            }
            set
            {
                _HostGroup = value;
            }
        }

        private bool _USBconnection;
        public bool USBconnection
        {
            get
            {
                return _USBconnection;
            }
            set
            {
                _USBconnection = value;
            }
        }

        #endregion

        #region Override Methods

        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {

#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - SplitInExecutionLists - jobList.Count: {1}",
                                                   currentTime, jobList.Count));
            }
#endif

            // sort by station name and last execution time only when more stations are configured
            if (CommDriver.GetChannelStations(this).Count() > 1)
                jobList.OrderBy(c => c.Station.Name).ThenBy(c => c.LastExecutionTime).ToList();

            int jobIndex = 0;
            while (jobIndex < jobList.Count())
            {
                bool write = false;
                bool first = true;
                string station = string.Empty;
                List<CommJob> dequeueListJob = new List<CommJob>();
                uint requestFrameTotalSize = 0;
                uint responseFrameTotalSize = 0;
                bool process = true;

                #region fill a list with job as long as size not exceed mamimux frame size
                while (jobIndex < jobList.Count())
                {
                    ROCDriverCommJob j = jobList.ElementAt(jobIndex) as ROCDriverCommJob;
                    ROCDriverStation s = j.Station as ROCDriverStation;
                    uint requestFrameJobSize = 0;
                    uint responseFrameJobSize = 0;
                    bool bRW = false;

                    if (first)
                    {
                        write = j.IsWriteRequest() && !j.IsReadRWReady();
                        bRW = j.IsReadRWReady();
                        station = j.Station.Name;
                        first = false;
                        if(write)
                        {
                            requestFrameTotalSize = ROCDriverProtocol.WriteRequestOverhead;
                            responseFrameTotalSize = ROCDriverProtocol.WriteReplyOverhead;
                        }
                        else
                        {
                            requestFrameTotalSize = ROCDriverProtocol.ReadRequestOverhead;
                            responseFrameTotalSize = ROCDriverProtocol.ReadReplyOverhead;
                        }

#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - SplitInExecutionLists - first == true - job: {1} {2} {3} - write: {4} - bRW: {5}",
                                                               currentTime, j.PointType, j.LogicalNumber, j.Parameter, write, bRW));
                        }
#endif
                    }

                    bool jwrite = j.IsWriteRequest() && !j.IsReadRWReady();

#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - SplitInExecutionLists - job: {1} {2} {3} - jwrite: {4} - write: {5}",
                                                           currentTime, j.PointType, j.LogicalNumber, j.Parameter, jwrite, write));
                    }
#endif
                    
                    process = true;
                    // If the job is in error state or it's not been executed yet, execute it separately
                    if (j.InErrorState || j.FirstTime)
                    {
                        process = false;
                        if (dequeueListJob.Count == 0)
                        {
                            dequeueListJob.Add(j);
                            jobList.RemoveAt(jobIndex);
                            break;
                        }
                    }

                    if (process)
                    {

                        if (j.Station != null && j.Station.Name == station)
                        {
                            // write
                            if (write && jwrite)
                            {
                                if (ROCDriverProtocol.GetJobWriteRequestResponseSize(ref j, ref requestFrameJobSize, ref responseFrameJobSize) &&
                                   ((requestFrameTotalSize + requestFrameJobSize) <= ROCDriverProtocol.MAX_FRAME_LENGTH) &&
                                   ((responseFrameTotalSize + responseFrameJobSize) <= ROCDriverProtocol.MAX_FRAME_LENGTH))
                                {
                                    dequeueListJob.Add(j);
                                    jobList.RemoveAt(jobIndex);
                                    jobIndex--;
                                    requestFrameTotalSize += requestFrameJobSize;
                                    responseFrameTotalSize += responseFrameJobSize;

#if DEBUG
                                    {
                                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - SplitInExecutionLists - write && jwrite adding job: {1} {2} {3} - dequeueListJob.Count: {4} - jobIndex: {5} - requestFrameTotalSize: {6} - responseFrameTotalSize: {7}",
                                                                           currentTime, j.PointType, j.LogicalNumber, j.Parameter, dequeueListJob.Count, jobIndex, requestFrameTotalSize, responseFrameTotalSize));
                                    }
#endif
                                }
                                else
                                {
                                    // job exceeds maximum frame size
#if DEBUG
                                    {
                                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - SplitInExecutionLists - write && jwrite job sixe exceeds maximum frame size, job: {1} {2} {3} - requestFrameTotalSize: {4} - responseFrameTotalSize: {5} -  requestFrameJobSize: {6} - responseFrameJobSize: {7}",
                                                                           currentTime, j.PointType, j.LogicalNumber, j.Parameter, requestFrameTotalSize, responseFrameTotalSize, requestFrameJobSize, responseFrameJobSize));
                                    }
#endif
                                    break;
                                }
                            }
                            else
                            {
                                // read
                                if (!write && !jwrite && j.Type != LinkType.ExceptionOutput && j.IsReadRWReady() == bRW)
                                {
                                    if (ROCDriverProtocol.GetJobReadRequestResponseSize(ref j, ref requestFrameJobSize, ref responseFrameJobSize) &&
                                       ((requestFrameTotalSize + requestFrameJobSize) <= ROCDriverProtocol.MAX_FRAME_LENGTH) &&
                                       ((responseFrameTotalSize + responseFrameJobSize) <= ROCDriverProtocol.MAX_FRAME_LENGTH))
                                    {
                                        j.GestRWState();
                                        dequeueListJob.Add(j);
                                        jobList.RemoveAt(jobIndex);
                                        jobIndex--;
                                        requestFrameTotalSize += requestFrameJobSize;
                                        responseFrameTotalSize += responseFrameJobSize;

#if DEBUG
                                        {
                                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                            System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - SplitInExecutionLists - !write && !jwrite - adding job: {1} {2} {3} - dequeueListJob.Count: {4} - jobIndex: {5} - requestFrameTotalSize: {6} - responseFrameTotalSize: {7}",
                                                                               currentTime, j.PointType, j.LogicalNumber, j.Parameter, dequeueListJob.Count, jobIndex, requestFrameTotalSize, responseFrameTotalSize));
                                        }
#endif
                                    }
                                    else
                                    {
                                        // job exceeds maximum frame size
#if DEBUG
                                        {
                                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                            System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - SplitInExecutionLists - !write && !jwrite job sixe exceeds maximum frame size, job: {1} {2} {3} - requestFrameTotalSize: {4} - responseFrameTotalSize: {5} -  requestFrameJobSize: {6} - responseFrameJobSize: {7}",
                                                                               currentTime, j.PointType, j.LogicalNumber, j.Parameter, requestFrameTotalSize, responseFrameTotalSize, requestFrameJobSize, responseFrameJobSize));
                                        }
#endif
                                        break;
                                    }
                                }
                            }
                        }
                        else
                        {
                            // station changed; 
                            break;
                        }
                    }

                    jobIndex++;

                }

                if (dequeueListJob.Count > 0)
                {
                    exList.Add(dequeueListJob);
                    jobIndex = 0;
                }
                #endregion
            }
        }

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> exjoblist)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                // back compatibilty with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }
            if(_ChannelType == ChannelTypes.Socket)
            {
                return (((ROCDriverTcpChannel)ChildChannel).ExecuteJobList(ref conn, exjoblist));
            }
            else
            {
                return (((ROCDriverSerialChannel)ChildChannel).ExecuteJobList(ref conn, exjoblist));
            }
        }

        private void ExecuteJobList(List<CommJob> list)
        {
            DateTime ExecutionTime = DateTime.UtcNow;
            Parallel.ForEach(list, j =>
            {
                base.ExecuteJob(j);
            });
        }

        public void ReceiveClear()
        {
            if (_ChannelType == ChannelTypes.Socket)
            {
                ((ROCDriverTcpChannel)ChildChannel).ReceiveClear();
            }
            else
            {
                ((ROCDriverSerialChannel)ChildChannel).ReceiveClear();
            }
        }

        public override bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
            DateTime messageReceiveDateTime = DateTime.UtcNow;
            if (conn != (int)DriverErrorCodes.ErrorNoError)
            {

                foreach (ROCDriverCommJob j in list)
                {
                    j.LastExecutionTime = DateTime.UtcNow;
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = conn, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();

                return false;
            }

            if (list.Count < 1)
            {
                return (false);
            }

            if (_ChannelType == ChannelTypes.Socket)
            {
                return(((ROCDriverTcpChannel)ChildChannel).ProcessNewDataList(conn, list));
            }
            else
            {
                return(((ROCDriverSerialChannel)ChildChannel).ProcessNewDataList(conn, list));
            }
        }

        public override bool IsDeviceOpen()
        {
            return ChildChannel.IsDeviceOpen();
        }

        public override bool DeviceOpen()
        {
            return ChildChannel.DeviceOpen();
        }

        public override bool DeviceClose()
        {
            return ChildChannel.DeviceClose();
        }

        public override bool DeviceRead(byte[] buffer, uint count)
        {
            return ChildChannel.DeviceRead(buffer,count);
        }

        public override bool DeviceWrite(byte[] buffer, uint count)
        {
            return ChildChannel.DeviceWrite(buffer, count);
        }

        public override uint GetBytesToRead()
        {
            return ChildChannel.GetBytesToRead();
        }

        public override uint GetBytesToWrite()
        {
            return ChildChannel.GetBytesToWrite();
        }

        public override bool Startup()
        {
            if (!bChannelStarted)
            {
                // start timer used to check communication periodically
                if(CheckConnectionFrequency < 1)
                {
                    CheckConnectionFrequency = 1;
                }
                CyclicCheckStart((int)CheckConnectionFrequency*500, (int)CheckConnectionFrequency*500);

                ChildChannel.Startup();
            }

            return base.Startup();
        }

        public override void Suspend()
        {
            base.Suspend();                        
        }

        public override void ResetNewDataEvent()
        {
            ChildChannel.ResetNewDataEvent();
        }

        public override void WaitNewDataEvent(ref DriverErrorCodes conn)
        {
            ChildChannel.WaitNewDataEvent(ref conn);
        }

        public override void SetNewDataEvent()
        {
            ChildChannel.SetNewDataEvent();
        }

        #endregion

        #region Member
        public byte[] requestBuffer = new byte[ROCDriverProtocol.MAX_FRAME_LENGTH];
        public Channel ChildChannel;
        public ushort ExpCharsTotal;
        public int maxReplyLength = 0;
        #endregion

        #region IDisposable

        public override void Dispose()
        {
            // first dispose base class to stop scheduler
            base.Dispose();

            // then dispose child to close device
            ChildChannel.Dispose();
        }
        #endregion

    }
 
    class ROCDriverTcpChannel : TcpChannelList
    {
        #region Constructors

        /// <summary>
        /// Initializes the ROCDriverTcpChannel object.
        /// </summary>
        public ROCDriverTcpChannel(CommunicationDriver commdriver, ROCDriverTcpChannelSettings settings)
            : base(commdriver, settings)
        {
            HostAddress = settings.TcpHostAddress;
            HostGroup = settings.TcpHostGroup;
            SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.ChannelUnconnected);
        }

        public void SetStateCommandVariable(StateCommandVariable chStateCommandVariable)
        {
            channelStateCommandVariable = chStateCommandVariable;
            SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.ChannelUnconnected);
        }

        #endregion

        #region Override Methods

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> list)
        {
            DateTime ExecutionTime = DateTime.UtcNow;
            Parallel.ForEach(list, j =>
            {
                base.ExecuteJob(j);
            });

            uint requestFrameLength = ROCDriverProtocol.PrepareFrame(list, ParentChannel, ref ParentChannel.requestBuffer);

            if(requestFrameLength < 1)
            {
                foreach (ROCDriverCommJob j in list)
                    RemovePendingJob(j);
                list.Clear();

                return(false);
            }

            if (!DeviceWrite(ParentChannel.requestBuffer, requestFrameLength))
            {
                conn = DriverErrorCodes.ErrorTXFullError;
                return false;
            }

            if (!BeginDeviceRead(ROCDriverProtocol.HeaderLength))
            {
                conn = DriverErrorCodes.ErrorTXFullError;
                return false;
            }

            return true;
        }

        public void ReceiveClear()
        {
            lock (lockThreadObject)
                ReceiveBuffer.Clear();
            Flush();
        }

        private DriverErrorCodes WaitCompleteMessage(ref List<Byte> recBuffer)
        {
            // Read the header of the reply message
            lock (lockThreadObject)
            {
                // Check if the header part of the message has been received
                if (ReceiveBuffer.Count < ROCDriverProtocol.HeaderLength)
                {
                    ReceiveClear();
                    return ((DriverErrorCodes)ROCDriverErrorCodes.ErrorIncompleteReply);
                }
            }

            // Get the remaining part of the message
            byte dataLength = ReceiveBuffer[(int)ROCDriverOffsets.DataLength];
            uint expectedPduTotalSize = (uint)(ROCDriverProtocol.HeaderLength + dataLength + ROCDriverProtocol.CRCLength);
            byte[] pdu = new byte[expectedPduTotalSize];
            uint numberOfReceivedBytes = (uint)ReceiveBuffer.Count();
            lock (lockThreadObject)
            {
                if (numberOfReceivedBytes <= expectedPduTotalSize)
                {
                    ReceiveBuffer.CopyTo(pdu, 0);
                }
                else
                {
                    ReceiveBuffer.CopyTo(0, pdu, 0, (int)expectedPduTotalSize);
                }
                ReceiveBuffer.Clear();
            }
            if (numberOfReceivedBytes < expectedPduTotalSize)
            {
                uint numberOfBytesToBeRead = expectedPduTotalSize - numberOfReceivedBytes;
                byte[] remainingPartOfPdu = new byte[numberOfBytesToBeRead];
                numberOfReceivedBytes = DeviceReadSynchronous(remainingPartOfPdu, numberOfBytesToBeRead);
                if (numberOfReceivedBytes < numberOfBytesToBeRead)
                {
                    ReceiveClear();
                    return ((DriverErrorCodes)ROCDriverErrorCodes.ErrorIncompleteReply);
                }

                Array.Copy(remainingPartOfPdu, 0, pdu, expectedPduTotalSize - numberOfBytesToBeRead, numberOfBytesToBeRead);
            }

            recBuffer.AddRange(pdu);

            return (DriverErrorCodes.ErrorNoError);
        }

        public override bool ProcessNewData(DriverErrorCodes conn, CommJob pendingjob)
        {
            return true;            
        }

        private bool SetErrorCodesAndRemoveJobsInError(byte[] pdu, List<CommJob> list)
        {
            byte dataLength = pdu[(int)ROCDriverOffsets.DataLength];
            if((dataLength == 0) || (dataLength%2 > 0))
            {
                return(false);
            }

            List<CommJob> listOfJobsToBeRemoved = new List<CommJob>();
            int pduIndex = (int)ROCDriverOffsets.DataLength + 1;
            DateTime messageReceiveDateTime = DateTime.UtcNow;
            while (pduIndex < (pdu.Length - ROCDriverProtocol.CRCLength))
            {
                byte errorCode = pdu[pduIndex++];
                byte jobIndex = (byte)(pdu[pduIndex++] - 1);
                if(jobIndex < list.Count)
                {
                    int error = ROCDriverProtocol.GetErrorCode(errorCode);
                    ROCDriverCommJob j = (ROCDriverCommJob)list[jobIndex];
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)error, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                    listOfJobsToBeRemoved.Add(j);
                }
            }
            foreach(CommJob j in listOfJobsToBeRemoved)
            {
                if(list.Contains(j))
                {
                    list.Remove(j);
                }
            }
            return (true);
        }

        public override bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
            // The "conn" and "list" parameters have been checked in the method ProcessNewDataList of the parent channel
        
            DateTime messageReceiveDateTime = DateTime.UtcNow;

            // Check if the header part of the message has been received
            if(ReceiveBuffer.Count < ROCDriverProtocol.HeaderLength)
            {
                foreach (ROCDriverCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorIncompleteReply, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                return(false);
            }

            if (list.Count < 1)
            {
                return (true);
            }

            // Get the message header and check it
            byte[] pduHeader = new byte[ROCDriverProtocol.HeaderLength];
            lock(lockThreadObject)
            {
                ReceiveBuffer.CopyTo(0, pduHeader, 0, ROCDriverProtocol.HeaderLength);
            }
            byte dataLength = 0;
            ROCDriverStation station = (ROCDriverStation)((ROCDriverCommJob)list[0]).Station;
            station.SetLastCommunicationTime(DateTime.UtcNow);
            byte opcode = (byte)ROCDriverOpcodes.RequestParameters;
            if(((ROCDriverCommJob)list[0]).IsWriteRequest() && !((ROCDriverCommJob)list[0]).IsReadRWReady())
            {
                opcode = (byte)ROCDriverOpcodes.WriteParameters;
            }
            int errorCode = ROCDriverProtocol.CheckMessageHeader(pduHeader, ParentChannel, station, opcode, ref dataLength);
            if(errorCode != (int)DriverErrorCodes.ErrorNoError)
            {
                foreach (ROCDriverCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)errorCode, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                return (false);
            }

            // The reply to a read request must contain the requested data;
            // an error message must contain, at least, an error code (2 bytes)
            if((dataLength == 0) && (ParentChannel.TestConnection == false) &&
                ((opcode == (byte)ROCDriverOpcodes.RequestParameters) || (opcode == (byte)ROCDriverOpcodes.ErrorIndicator)))
            {
                foreach (ROCDriverCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorMissingData, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                return (false);
            }

            // Get the remaining part of the message
            uint expectedPduTotalSize = (uint)(ROCDriverProtocol.HeaderLength + dataLength + ROCDriverProtocol.CRCLength);
            byte[] pdu = new byte[expectedPduTotalSize];
            uint numberOfReceivedBytes = (uint)ReceiveBuffer.Count();
            lock (lockThreadObject)
            {
                if (numberOfReceivedBytes <= expectedPduTotalSize)
                {
                    ReceiveBuffer.CopyTo(pdu, 0);
                }
                else
                {
                    ReceiveBuffer.CopyTo(0, pdu, 0, (int)expectedPduTotalSize);
                }
                ReceiveBuffer.Clear();
            }
            if (numberOfReceivedBytes < expectedPduTotalSize)
            {
                uint numberOfBytesToBeRead = expectedPduTotalSize - numberOfReceivedBytes;
                byte[] remainingPartOfPdu = new byte[numberOfBytesToBeRead];
                numberOfReceivedBytes = DeviceReadSynchronous(remainingPartOfPdu, numberOfBytesToBeRead);
                if(numberOfReceivedBytes < numberOfBytesToBeRead)
                {
                    messageReceiveDateTime = DateTime.UtcNow;
                    foreach (ROCDriverCommJob j in list)
                    {
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorIncompleteReply, Job = j, LastExecutionTime = messageReceiveDateTime };
                        OnJobExecuted(eJob);
                    }

                    ReceiveClear();
                    return (false);
                }

                Array.Copy(remainingPartOfPdu, 0, pdu, expectedPduTotalSize - numberOfBytesToBeRead, numberOfBytesToBeRead);
            }

            // Check the CRC
            if(!ROCDriverProtocol.CheckCRC(pdu, expectedPduTotalSize))
            {
                foreach (ROCDriverCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorCrc, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                return (false);
            }

            // Check and set error codes
            if((ParentChannel.TestConnection == false) && ROCDriverProtocol.IsErrorMessage(pdu))
            {
                if(!SetErrorCodesAndRemoveJobsInError(pdu, list))
                {
                    foreach (ROCDriverCommJob j in list)
                    {
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorMalformattedErrorMessage, Job = j, LastExecutionTime = messageReceiveDateTime };
                        OnJobExecuted(eJob);
                    }

                    ReceiveClear();
                }
                return (false);
            }

            // Output jobs or communication test
            if ((ParentChannel.TestConnection == true) || (opcode == (byte)ROCDriverOpcodes.WriteParameters))
            {
                foreach (ROCDriverCommJob j in list)
                {
                    if(j.FirstTime)
                    {
                        j.FirstTime = false;
                    }
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorNoError, Job = j, LastExecutionTime = messageReceiveDateTime, Values = new byte[0] };
                    OnJobExecuted(eJob);
                }

                return (true);
            }

            // Input jobs

            // Check the number of the received parameters: it must be equal to the number of jobs
            byte numberOfReceivedParameters = pdu[(int)ROCDriverOffsets.NumberOfParameters];
            if(numberOfReceivedParameters != list.Count)
            {
                foreach (ROCDriverCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorUnexpectedNumberOfParameters, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                return (false);
            }

            // Check the length of the received PDU
            uint expectedReadReplyLength = ROCDriverProtocol.GetReadResponseTotalSize(list);
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - TcpChannel.ProcessNewDataList - expectedReadReplyLength: {1} - expectedPduTotalSize: {2}",
                                                   currentTime, expectedReadReplyLength, expectedPduTotalSize));
            }
#endif
            if (expectedReadReplyLength != expectedPduTotalSize)
            {
                foreach (ROCDriverCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorReadReplyLength, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                return (false);
            }

            int pduIndex = (int)ROCDriverOffsets.NumberOfParameters + 1;
            // Copy the receive data
            foreach (ROCDriverCommJob j in list)
            {
                byte[] parameterBuffer = ParentChannel.GetNextDataParameter(j, pdu, ref pduIndex);
                if (parameterBuffer == null)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorParsingParameterData, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                    break;
                }
                else
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorNoError, Job = j, LastExecutionTime = messageReceiveDateTime, Values = parameterBuffer };
                    OnJobExecuted(eJob);
                }
            }

            return (true);
        }

        #endregion

        #region static const

        #endregion

        #region Methods
        public void CheckDevicesConnection(List<Station> stationsToCheck)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of TcpChannel - Number of stations to be checked: {1}",
                                                   currentTime, stationsToCheck.Count));
            }
#endif
            DriverErrorCodes connChannel = DriverErrorCodes.ErrorNoError;
            if (!IsDeviceOpen())
            {
                DeviceClose();
                if (!DeviceOpen())
                    connChannel = DriverErrorCodes.ErrorDeviceOpenFailed;
            }
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of TcpChannel - Connection of the channel: {1}",
                                                   currentTime, connChannel));
            }
#endif

            foreach (ROCDriverStation s in stationsToCheck)
            {
                DriverErrorCodes conn = connChannel;
                if(conn == DriverErrorCodes.ErrorNoError)
                {
                    uint testRequestLength = ROCDriverProtocol.PrepareTestRequest(s, ParentChannel, ref ParentChannel.requestBuffer);
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of TcpChannel - ROCDriverProtocol.PrepareTestRequest returned: {1} for station: {2}",
                                                           currentTime, testRequestLength, s.Name));
                    }
#endif
                    if (testRequestLength == 0)
                    {
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of TcpChannel - ROCDriverProtocol.PrepareTestRequest returned 0 for station: {1}",
                                                               currentTime, s.Name));
                        }
#endif
                        continue;
                    }

                    if (!DeviceWrite(ParentChannel.requestBuffer, testRequestLength) ||
                        !BeginDeviceRead(ROCDriverProtocol.HeaderLength))
                    {
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of TcpChannel - Error in DeviceWrite or in BeginDeviceRead for station: {1}",
                                                               currentTime, s.Name));
                        }
#endif
                        // Do nothing here
                    }

                    // Wait to receive the reply
                    WaitNewDataEvent(ref conn);

#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of TcpChannel - After WaitNewDataEvent - conn == {1} for station: {2}",
                                                           currentTime, conn, s.Name));
                    }
#endif
                }

                if (conn == DriverErrorCodes.ErrorNoError)
                {
                    // Wait to complete the reception of the reply message
                    List<Byte> receiveBuffer = new List<byte>();
                    conn = WaitCompleteMessage(ref receiveBuffer);
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of TcpChannel - After WaitCompleteMessage - conn == {1} for station: {2}",
                                                           currentTime, conn, s.Name));
                    }
#endif
                    if (conn == DriverErrorCodes.ErrorNoError)
                    {
                        byte[] pdu = receiveBuffer.ToArray();
                        conn = (DriverErrorCodes)ROCDriverProtocol.CheckReplyToPingMessage(pdu, ParentChannel, s);
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of TcpChannel - ROCDriverProtocol.CheckReplyToPingMessage returned: {1} for station: {2}",
                                                               currentTime, conn, s.Name));
                        }
#endif
                    }
                }

                s.SetLastCommunicationTime(DateTime.UtcNow);

                // Error on the channel connection, or receiving the reply, or in the message format?
                if (conn != DriverErrorCodes.ErrorNoError)
                {
                    s.SetConnectionError();
                }
                else
                {
                    s.LastErrorCode = DriverErrorCodes.ErrorNoError;
                }
            }
        }

        #endregion

        #region Properties

        /// <summary>   Host Address (Unit). </summary>
        private Byte _HostAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel host address. </summary>
        ///
        /// <value> The TCP channel host address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Byte HostAddress
        {
            get
            {
                return _HostAddress;
            }
            set
            {
                _HostAddress = value;
            }
        }

        /// <summary>   Host Group. </summary>
        private Byte _HostGroup;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel host group. </summary>
        ///
        /// <value> The TCP channel host group. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Byte HostGroup
        {
            get
            {
                return _HostGroup;
            }
            set
            {
                _HostGroup = value;
            }
        }

        #endregion

        #region Member
        public ROCDriverChannel ParentChannel;

        #endregion

        #region IChannelBase Interface

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            if (ParentChannel != null)
            {
                e.RxBytes = DiagnLastTaskRxBytes;
                DiagnLastTaskRxBytes = 0;
                e.TxBytes = DiagnLastTaskTxBytes;
                DiagnLastTaskTxBytes = 0;
                ParentChannel.OnJobExecuted(e);
            }
        }
        
        #endregion

    }

    class ROCDriverSerialChannel : SerialChannelList
    {
        #region Constructors

        /// <summary>
        /// Initializes the ModbusChannel object.
        /// </summary>
        public ROCDriverSerialChannel(CommunicationDriver commdriver, ROCDriverSerialChannelSettings settings)
            : base(commdriver, settings)
        {
            HostAddress = settings.SerialHostAddress;
            HostGroup = settings.SerialHostGroup;
            SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.ChannelUnconnected);
        }

        public void SetStateCommandVariable(StateCommandVariable chStateCommandVariable)
        {
            channelStateCommandVariable = chStateCommandVariable;
            SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.ChannelUnconnected);
        }

        #endregion

        #region Override Methods
        public void CheckDevicesConnection(List<Station> stationsToCheck)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of SerialChannel - Number of stations to be checked: {1}",
                                                   currentTime, stationsToCheck.Count));
            }
#endif
            DriverErrorCodes connChannel = DriverErrorCodes.ErrorNoError;
            if (!IsDeviceOpen())
            {
                DeviceClose();
                if (!DeviceOpen())
                    connChannel = DriverErrorCodes.ErrorDeviceOpenFailed;
            }
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of SerialChannel - Connection of the channel: {1}",
                                                   currentTime, connChannel));
            }
#endif
            foreach (ROCDriverStation s in stationsToCheck)
            {
                DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
                if(conn == DriverErrorCodes.ErrorNoError)
                {
                    uint testRequestLength = ROCDriverProtocol.PrepareTestRequest(s, ParentChannel, ref ParentChannel.requestBuffer);
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of SerialChannel - ROCDriverProtocol.PrepareTestRequest returned: {1} for station: {2}",
                                                           currentTime, testRequestLength, s.Name));
                    }
#endif
                    if (testRequestLength == 0)
                    {
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of SerialChannel - ROCDriverProtocol.PrepareTestRequest returned 0 for station: {1}",
                                                               currentTime, s.Name));
                        }
#endif
                        continue;
                    }

                    if (!DeviceWrite(ParentChannel.requestBuffer, testRequestLength))
                    {
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of SerialChannel - Error in DeviceWrite for station: {1}",
                                                               currentTime, s.Name));
                        }
#endif
                        // Do nothing here
                    }

                    // Wait to receive the reply
                    WaitNewDataEvent(ref conn);

#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of SerialChannel - After WaitNewDataEvent - conn == {1} for station: {2}",
                                                           currentTime, conn, s.Name));
                    }
#endif
                }

                if (conn == DriverErrorCodes.ErrorNoError)
                {
                    // Wait to complete the reception of the reply message
                    List<Byte> receiveBuffer = new List<byte>();
                    conn = WaitCompleteMessage(ref receiveBuffer, s);
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of SerialChannel - After WaitCompleteMessage - conn == {1} for station: {2}",
                                                           currentTime, conn, s.Name));
                    }
#endif
                    if (conn == DriverErrorCodes.ErrorNoError)
                    {
                        byte[] pdu = receiveBuffer.ToArray();
                        conn = (DriverErrorCodes)ROCDriverProtocol.CheckReplyToPingMessage(pdu, ParentChannel, s);
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - CheckDevicesConnection of SerialChannel - ROCDriverProtocol.CheckReplyToPingMessage returned: {1} for station: {2}",
                                                               currentTime, conn, s.Name));
                        }
#endif
                    }
                }

                s.SetLastCommunicationTime(DateTime.UtcNow);

                // Error receiving the reply or in the message format?
                if (conn != DriverErrorCodes.ErrorNoError)
                {
                    s.SetConnectionError();
                }
                else
                {
                    s.LastErrorCode = DriverErrorCodes.ErrorNoError;
                }
            }
        }

        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob job)
        {
            return true;
        }

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> list)
        {
            DateTime ExecutionTime = DateTime.UtcNow;
            Parallel.ForEach(list, j =>
            {
                base.ExecuteJob(j);
            });

            // Prepare the request
            uint requestFrameLength = ROCDriverProtocol.PrepareFrame(list, ParentChannel, ref ParentChannel.requestBuffer);

            if (requestFrameLength < 1)
            {
                foreach (ROCDriverCommJob j in list)
                    RemovePendingJob(j);
                list.Clear();

                return (false);
            }

            // Clear the receiving buffer, before sending the request
            ReceiveClear();

            // Send the request
            if (!DeviceWrite(ParentChannel.requestBuffer, requestFrameLength))
            {
                conn = DriverErrorCodes.ErrorTXFullError;
                return false;
            }

            return true;
        }

        public void ReceiveClear()
        {
            lock (lockThreadObject)
                ReceiveBuffer.Clear();
            Flush();
        }

        private bool HeaderHasBeenReceived(ref List<Byte> recBuffer, ROCDriverStation station)
        {
            bool returnValue = false;
            while((recBuffer.Count >= ROCDriverProtocol.HeaderLength) && (returnValue == false))
            {
                // Check the initial part of the header
                // Check the first received byte: it must be equal to the unit ID of the channel
                if(recBuffer[(int)ROCDriverOffsets.DestinationUnit] != ParentChannel.HostAddress)
                {
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - HeaderHasBeenReceived - Case 1 - Removing: {1}",
                                                           currentTime, recBuffer[(int)ROCDriverOffsets.DestinationUnit]));
                    }
#endif
                    recBuffer.RemoveAt((int)ROCDriverOffsets.DestinationUnit);
                }
                // Check the second received byte: it must be equal to the unit group of the channel
                else if (recBuffer[(int)ROCDriverOffsets.DestinationGroup] != ParentChannel.HostGroup)
                {
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - HeaderHasBeenReceived - Case 2 - Removing: {1}, {2}",
                                                           currentTime, recBuffer[(int)ROCDriverOffsets.DestinationUnit], recBuffer[(int)ROCDriverOffsets.DestinationGroup]));
                    }
#endif
                    recBuffer.RemoveAt((int)ROCDriverOffsets.DestinationGroup);
                    recBuffer.RemoveAt((int)ROCDriverOffsets.DestinationUnit);
                }
                // Check the third received byte: it must be equal to the unit ID of the station
                else if (recBuffer[(int)ROCDriverOffsets.SourceUnit] != station.StationID)
                {
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - HeaderHasBeenReceived - Case 3 - Removing: {1}, {2}, {3}",
                                                           currentTime, recBuffer[(int)ROCDriverOffsets.DestinationUnit], recBuffer[(int)ROCDriverOffsets.DestinationGroup], recBuffer[(int)ROCDriverOffsets.SourceUnit]));
                    }
#endif
                    recBuffer.RemoveAt((int)ROCDriverOffsets.SourceUnit);
                    recBuffer.RemoveAt((int)ROCDriverOffsets.DestinationGroup);
                    recBuffer.RemoveAt((int)ROCDriverOffsets.DestinationUnit);
                }
                // Check the fourth received byte: it must be equal to the unit group of the station
                else if (recBuffer[(int)ROCDriverOffsets.SourceGroup] != station.StationGroup)
                {
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - HeaderHasBeenReceived - Case 4 - Removing: {1}, {2}, {3}, {4}",
                                                           currentTime, recBuffer[(int)ROCDriverOffsets.DestinationUnit], recBuffer[(int)ROCDriverOffsets.DestinationGroup], recBuffer[(int)ROCDriverOffsets.SourceUnit], recBuffer[(int)ROCDriverOffsets.SourceGroup]));
                    }
#endif
                    recBuffer.RemoveAt((int)ROCDriverOffsets.SourceGroup);
                    recBuffer.RemoveAt((int)ROCDriverOffsets.SourceUnit);
                    recBuffer.RemoveAt((int)ROCDriverOffsets.DestinationGroup);
                    recBuffer.RemoveAt((int)ROCDriverOffsets.DestinationUnit);
                }
                else
                {
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - HeaderHasBeenReceived - Returning true",
                                                           currentTime));
                    }
#endif
                    returnValue = true;
                }
            }

            return (returnValue);
        }

        private DriverErrorCodes WaitCompleteMessage(ref List<Byte> recBuffer, ROCDriverStation station)
        {
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;

            // Read the header of the reply message
            if (!ReceiveBufferPull(ref recBuffer))
            {
                // error
                conn = (DriverErrorCodes)ROCDriverErrorCodes.ErrorIncompleteReply;
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - WaitCompleteMessage of SerialChannel - ReceiveBufferPull false",
                                                       currentTime));
                }
#endif
            }

            // Check if the complete header has been received
            bool headerReceived = false;
            if(conn == (int)DriverErrorCodes.ErrorNoError)
            {
                headerReceived = HeaderHasBeenReceived(ref recBuffer, station);
            }
            while (conn == (int)DriverErrorCodes.ErrorNoError && !headerReceived && recBuffer.Count < ROCDriverProtocol.HeaderLength)
            {
                WaitNewDataEvent(ref conn);
                if(conn == DriverErrorCodes.ErrorNoError)
                {
                    ReceiveBufferPull(ref recBuffer);
                    headerReceived = HeaderHasBeenReceived(ref recBuffer, station);
                }
            }
            if(conn != DriverErrorCodes.ErrorNoError)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - WaitCompleteMessage of SerialChannel - WaitNewDataEvent set conn to {1}",
                                                       currentTime, conn));
                }
#endif
                return (conn);
            }
            else if (recBuffer.Count < ROCDriverProtocol.HeaderLength)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - WaitCompleteMessage of SerialChannel - conn: {1} - recBuffer.Count: {2} < 6",
                                                       currentTime, conn, recBuffer.Count));
                }
#endif
                return ((DriverErrorCodes)ROCDriverErrorCodes.ErrorIncompleteReply);
            }

            byte dataLength = recBuffer[(int)ROCDriverOffsets.DataLength];

            // Read the remaining part of the message
            uint expectedPduTotalSize = (uint)(ROCDriverProtocol.HeaderLength + dataLength + ROCDriverProtocol.CRCLength);
            while (conn == (int)DriverErrorCodes.ErrorNoError && recBuffer.Count < expectedPduTotalSize)
            {
                WaitNewDataEvent(ref conn);
                if (conn == DriverErrorCodes.ErrorNoError)
                {
                    ReceiveBufferPull(ref recBuffer);
                }
            }
            if (conn != DriverErrorCodes.ErrorNoError)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - WaitCompleteMessage of SerialChannel - WaitNewDataEvent (2nd step) set conn to {1}",
                                                       currentTime, conn));
                }
#endif
                return (conn);
            }
            else if (recBuffer.Count < expectedPduTotalSize)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - WaitCompleteMessage of SerialChannel - recBuffer.Count: {1} < {2}",
                                                       currentTime, recBuffer.Count, expectedPduTotalSize));
                }
#endif
                return ((DriverErrorCodes)ROCDriverErrorCodes.ErrorIncompleteReply);
            }
            else
            {
                return ((int)DriverErrorCodes.ErrorNoError);
            }
        }

        private bool SetErrorCodesAndRemoveJobsInError(byte[] pdu, List<CommJob> list)
        {
            byte dataLength = pdu[(int)ROCDriverOffsets.DataLength];
            if ((dataLength == 0) || (dataLength % 2 > 0))
            {
                return (false);
            }

            List<CommJob> listOfJobsToBeRemoved = new List<CommJob>();
            int pduIndex = (int)ROCDriverOffsets.DataLength + 1;
            DateTime messageReceiveDateTime = DateTime.UtcNow;
            while (pduIndex < (pdu.Length - ROCDriverProtocol.CRCLength))
            {
                byte errorCode = pdu[pduIndex++];
                byte jobIndex = (byte)(pdu[pduIndex++] - 1);
                if (jobIndex < list.Count)
                {
                    int error = ROCDriverProtocol.GetErrorCode(errorCode);
                    ROCDriverCommJob j = (ROCDriverCommJob)list[jobIndex];
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)error, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                    listOfJobsToBeRemoved.Add(j);
                }
            }
            foreach (CommJob j in listOfJobsToBeRemoved)
            {
                if (list.Contains(j))
                {
                    list.Remove(j);
                }
            }
            return (true);
        }

        public override bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
            // The "conn" and "list" parameters have been checked in the method ProcessNewDataList of the parent channel
            ROCDriverStation station = list[0].Station as ROCDriverStation;

            // Spool data from the serial buffer
            List<Byte> receiveBuffer = new List<byte>();
            int errorCode = (int)WaitCompleteMessage(ref receiveBuffer, station);
            DateTime messageReceiveDateTime = DateTime.UtcNow;
            if (errorCode != (int)DriverErrorCodes.ErrorNoError)
            {
                foreach (ROCDriverCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)errorCode, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                return (false);
            }

            // Check the message header
            byte[] pdu = receiveBuffer.ToArray();
            byte dataLength = 0;
            station.SetLastCommunicationTime(DateTime.UtcNow);
            byte opcode = (byte)ROCDriverOpcodes.RequestParameters;
            if (((ROCDriverCommJob)list[0]).IsWriteRequest() && !((ROCDriverCommJob)list[0]).IsReadRWReady())
            {
                opcode = (byte)ROCDriverOpcodes.WriteParameters;
            }
            errorCode = ROCDriverProtocol.CheckMessageHeader(pdu, ParentChannel, station, opcode, ref dataLength);
            if (errorCode != (int)DriverErrorCodes.ErrorNoError)
            {
                foreach (ROCDriverCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)errorCode, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                return (false);
            }

            // The reply to a read request must contain the requested data;
            // an error message must contain, at least, an error code (2 bytes)
            if ((dataLength == 0) && (ParentChannel.TestConnection == false) &&
                ((opcode == (byte)ROCDriverOpcodes.RequestParameters) || (opcode == (byte)ROCDriverOpcodes.ErrorIndicator)))
            {
                foreach (ROCDriverCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorMissingData, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                return (false);
            }

            // Check if the message has been completely received
            uint expectedPduTotalSize = (uint)(ROCDriverProtocol.HeaderLength + dataLength + ROCDriverProtocol.CRCLength);
            uint numberOfReceivedBytes = (uint)pdu.Length;
            if (numberOfReceivedBytes < expectedPduTotalSize)
            {
                foreach (ROCDriverCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorIncompleteReply, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                return (false);
            }

            // Check the CRC
            if (!ROCDriverProtocol.CheckCRC(pdu, expectedPduTotalSize))
            {
                foreach (ROCDriverCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorCrc, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                return (false);
            }

            // Check and set error codes
            if ((ParentChannel.TestConnection == false) && ROCDriverProtocol.IsErrorMessage(pdu))
            {
                if (!SetErrorCodesAndRemoveJobsInError(pdu, list))
                {
                    foreach (ROCDriverCommJob j in list)
                    {
                        ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorMalformattedErrorMessage, Job = j, LastExecutionTime = messageReceiveDateTime };
                        OnJobExecuted(eJob);
                    }

                    ReceiveClear();
                }
                return (false);
            }

            // Output jobs or communication test
            if ((ParentChannel.TestConnection == true) || (opcode == (byte)ROCDriverOpcodes.WriteParameters))
            {
                foreach (ROCDriverCommJob j in list)
                {
                    if (j.FirstTime)
                    {
                        j.FirstTime = false;
                    }
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorNoError, Job = j, LastExecutionTime = messageReceiveDateTime, Values = new byte[0] };
                    OnJobExecuted(eJob);
                }

                return (true);
            }

            // Input jobs

            // Check the number of the received parameters: it must be equal to the number of jobs
            byte numberOfReceivedParameters = pdu[(int)ROCDriverOffsets.NumberOfParameters];
            if (numberOfReceivedParameters != list.Count)
            {
                foreach (ROCDriverCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorUnexpectedNumberOfParameters, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                return (false);
            }

            // Check the length of the received PDU
            uint expectedReadReplyLength = ROCDriverProtocol.GetReadResponseTotalSize(list);
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("ROC DBG - {0} - SerialChannel.ProcessNewDataList - expectedReadReplyLength: {1} - expectedPduTotalSize: {2}",
                                                   currentTime, expectedReadReplyLength, expectedPduTotalSize));
            }
#endif
            if (expectedReadReplyLength != expectedPduTotalSize)
            {
                foreach (ROCDriverCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorReadReplyLength, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                return (false);
            }

            int pduIndex = (int)ROCDriverOffsets.NumberOfParameters + 1;
            // Copy the receive data
            foreach (ROCDriverCommJob j in list)
            {
                byte[] parameterBuffer = ParentChannel.GetNextDataParameter(j, pdu, ref pduIndex);
                if (parameterBuffer == null)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)ROCDriverErrorCodes.ErrorParsingParameterData, Job = j, LastExecutionTime = messageReceiveDateTime };
                    OnJobExecuted(eJob);
                    break;
                }
                else
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorNoError, Job = j, LastExecutionTime = messageReceiveDateTime, Values = parameterBuffer };
                    OnJobExecuted(eJob);
                }
            }

            return true;
        }

        #endregion

        #region Properties

        /// <summary>   Host Address (Unit). </summary>
        private Byte _HostAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the serial channel host address. </summary>
        ///
        /// <value> The Serial channel host address. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Byte HostAddress
        {
            get
            {
                return _HostAddress;
            }
            set
            {
                _HostAddress = value;
            }
        }

        /// <summary>   Host Group. </summary>
        private Byte _HostGroup;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the serial channel host group. </summary>
        ///
        /// <value> The Serial channel host group. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public Byte HostGroup
        {
            get
            {
                return _HostGroup;
            }
            set
            {
                _HostGroup = value;
            }
        }
        #endregion


        #region static const


        #endregion

        #region methods

        private void Flush()
        {
            // wait until all data in write buffer was sent
            uint WaitingBytes = GetBytesToWrite();
            while (WaitingBytes != 0)
                WaitingBytes = GetBytesToWrite();

            // read buffer is automatically spooled by Serial Class Base

            // ReceiveBuffer is filled async so, for security reason, if an error occour in the previous job's execution, clean up
            lock (lockThreadObject)
            {
                ReceiveBuffer.Clear();
            }        
        }

        bool ReceiveBufferPull(ref List<Byte> p)
        {
            bool RetValue = false;

            lock (lockThreadObject)
            {
                List<byte> data = new List<byte>();
                if (ReceiveBuffer.Count > 0)
                {
                    data.AddRange(ReceiveBuffer);
                    ReceiveBuffer.Clear();
                }
                if (data.Count > 0)
                {
                    p.AddRange(data);
                    RetValue = true;
                }
            }
            return RetValue;
        }

        #endregion

        #region Member        
        public ROCDriverChannel ParentChannel;        
        #endregion
 
        #region IChannelBase Interface

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {            
            if (ParentChannel != null)
            {
                e.RxBytes = DiagnLastTaskRxBytes;
                DiagnLastTaskRxBytes = 0;
                e.TxBytes = DiagnLastTaskTxBytes;
                DiagnLastTaskTxBytes = 0;
                ParentChannel.OnJobExecuted(e);
            }
        }
        
        #endregion
    
    }

}
