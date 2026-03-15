////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	IEC60870_5_104Channel.cs
//
// summary:	Implements the driver IEC60870_5_104 channel class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using System.Threading;

namespace IEC60870_5_104
{
    /// <summary>   Communication channel of the IEC60870_5_104 driver. </summary>
    public class IEC60870_5_104Channel : TcpChannel
    {    
        enum IEC60870_5_104EventID : int
        {
            TimeOut = 1000,
            NewDataToAnalize = 1001,
        }

        public enum LinkStates
        {
            BEGIN,
            WaitCONNECT,
            StartTIMESYNC,
            WaitTIMESYNC,
            StartGENINTERR,
            WaitGENINTERRConf,
            WaitGENINTERREnd,
            StarCOUNTINTERR,
            WaitCOUNTINTERRConf,
            WaitCOUNTINTERREnd,
            EndInit,
            WaitTestConf,
        }

        public enum ReadWriteJobStates
        {
            READ_WRITE_SCHEDULE,
            WRITE_SEND,
            WRITE_SEND_SBO,
            WRITE_ANSWER,
            WRITE_ANSWER_SBO,
            WRITE_FILESELECTANSWER,
            WRITE_FILEREQUEST,
            WRITE_FILEREQUESTANSWER,
            WRITE_SECTIONREQUEST,
            WRITE_SECTIONREQUESTANSWER,
            WRITE_ACKNOWLEDGESECTION,
            WRITE_ACKNOWLEDGEFILE,
            READ_SEND,
            READ_CALLDIRECTORYANSWER,
            READ_WRITE_END
        }

        enum ReceiveStates
        {
            Init,
            WaitAPCI,
            WaitASDU,
        }

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the IEC60870_5_104Channel object. </summary>
        ///
        /// <param name="commdriver">   . </param>
        /// <param name="settings">     . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public IEC60870_5_104Channel(CommunicationDriver commdriver, IEC60870_5_104ChannelSettings settings)
            : base(commdriver, settings, null)
        {
            _OriginatorAddress = settings.OriginatorAddress;
            _InitializationTimeOut = settings.InitializationTimeOut;
            _RepeatInitialInterrogation = settings.RepeatInitialInterrogation;
            _EnableInitialClockSynchronization = settings.EnableInitialClockSynchronization;
            senderCounter = 0;
            receiverControlCounter = 0;
            ReceiveState = ReceiveStates.Init;
            LinkState = LinkStates.BEGIN;
            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
            MaxCounterReceived = false;            
        }

        #endregion

        #region Member
        private ushort senderCounter;  // sender packet counter
        private ushort receiverControlCounter;  // receiver packet control counter
        IEC60870_5_104Station station;
        bool receiveTcp;
        ReceiveStates ReceiveState;
        public LinkStates LinkState;
        DateTime ReceiveLoopTestTime;
        DateTime LinkLoopTestTime;
        IEC60870_5_104CommJob OnIdleJob;
        DateTime dtSentUType;
        DateTime dtSentIType;
        DateTime dtLastIncomingAPDU;
        bool bEnableStartDtLastIncomingITypeAPDU;
        DateTime dtLastIncomingITypeAPDU;
        int NumIncomingITypeAPDUs;
        int NumAck;
        bool bSentUType;
        bool MaxCounterReceived;
        ReadWriteJobStates ReadWriteJobState;
        DateTime ReadWriteJobLoopTestTime;

        WaitHandle[] waitHandles;
        #endregion

        #region Override Methods        
        private void GetNextMinTimeOut(ref int minWaitTimeOut, int newWaitTimeOut)
        {
            GetNextMinTimeOut(ref minWaitTimeOut, new TimeSpan(0, 0, 0, 0, newWaitTimeOut));
        }
        private void GetNextMinTimeOut(ref int minWaitTimeOut, uint newWaitTimeOut)
        {
            GetNextMinTimeOut(ref minWaitTimeOut, new TimeSpan(0, 0, 0, 0, (int)newWaitTimeOut));
        }

        private void GetNextMinTimeOut(ref int minWaitTimeOut, TimeSpan newWaitTimeOut)
        {
            double diff = newWaitTimeOut.TotalMilliseconds;
            if (diff > 0 && (int)diff < minWaitTimeOut)
                minWaitTimeOut = (int)diff;            
        }

        private void GetWaitNewDataEventConditions(out TimeSpan timeout, out bool someJobsPenging)
        {
            int minWaitTimeOut = (Timeout / 2);
            someJobsPenging = false;

            // on general error or connection error, slow down reconnect operations
            if (!IsDeviceOpen() || station.InGeneralErrorState)
            {
                minWaitTimeOut = (int)Timeout;
            }
            else
            {
                #region Receive Loop
                //since time cannot be calculated earlier, take the minimum of 2
                GetNextMinTimeOut(ref minWaitTimeOut, station.MissingAckTimeout);
                GetNextMinTimeOut(ref minWaitTimeOut, station.NoInMsgTimeout);
                #endregion

                #region LinkLoop
                switch (LinkState)
                {
                    case LinkStates.BEGIN:
                        GetNextMinTimeOut(ref minWaitTimeOut, 1);
                        break;
                    case LinkStates.WaitCONNECT:
                        GetNextMinTimeOut(ref minWaitTimeOut, (Timeout / 2)); // (DateTime.UtcNow - LinkLoopTestTime));
                        break;
                    case LinkStates.StartTIMESYNC:
                        GetNextMinTimeOut(ref minWaitTimeOut, 1);
                        break;
                    case LinkStates.WaitTIMESYNC:
                        GetNextMinTimeOut(ref minWaitTimeOut, (Timeout / 2)); // (DateTime.UtcNow - LinkLoopTestTime));
                        break;
                    case LinkStates.StartGENINTERR:
                        GetNextMinTimeOut(ref minWaitTimeOut, 1);
                        break;
                    case LinkStates.WaitGENINTERRConf:
                        GetNextMinTimeOut(ref minWaitTimeOut, (Timeout / 2)); // (DateTime.UtcNow - LinkLoopTestTime));
                        break;
                    case LinkStates.WaitGENINTERREnd:
                        GetNextMinTimeOut(ref minWaitTimeOut, ((InitializationTimeOut * 1000) / 2)); // (DateTime.UtcNow - LinkLoopTestTime));
                        break;
                    case LinkStates.StarCOUNTINTERR:
                        GetNextMinTimeOut(ref minWaitTimeOut, 1);
                        break;
                    case LinkStates.WaitCOUNTINTERRConf:
                        GetNextMinTimeOut(ref minWaitTimeOut, (Timeout /2)); // (DateTime.UtcNow - LinkLoopTestTime));
                        break;
                    case LinkStates.WaitCOUNTINTERREnd:
                        GetNextMinTimeOut(ref minWaitTimeOut, ((InitializationTimeOut * 1000) / 2)); // (DateTime.UtcNow - LinkLoopTestTime));
                        break;

                    case LinkStates.EndInit:
                        GetNextMinTimeOut(ref minWaitTimeOut, ((InitializationTimeOut * 1000) / 2)); // (DateTime.UtcNow - LinkLoopTestTime));
                        GetNextMinTimeOut(ref minWaitTimeOut, (station.NoActivityTimeout / 2)); //  (DateTime.UtcNow - dtLastIncomingAPDU));
                        break;
                    case LinkStates.WaitTestConf:
                        GetNextMinTimeOut(ref minWaitTimeOut, (int)((Timeout + station.NoActivityTimeout)/2)); // (DateTime.UtcNow - dtLastIncomingAPDU));
                        break;
                }
                #endregion

                #region ReadWriteJobLoop
                if (ReadWriteJobState != ReadWriteJobStates.READ_WRITE_SCHEDULE)
                    GetNextMinTimeOut(ref minWaitTimeOut, (uint)(Timeout / 2));
                #endregion

                #region ConfirmeAct
                // nothing to calculate
                #endregion

                // check is some jobs are pending --> force job execution
                someJobsPenging = (!(LinkState < LinkStates.EndInit) && ReadWriteJobState == ReadWriteJobStates.READ_WRITE_SCHEDULE && NrRemainingJobsInQueues() > 0);
            }
            
            timeout = new TimeSpan(0,0,0,0, minWaitTimeOut);
        }

        /// <summary>
        /// Wait until an "external" event or timepout to start process
        /// </summary>
        /// <param name="sleepCycle"></param>
        /// <returns></returns>
        private IEC60870_5_104EventID WaitNewEvent(int sleepCycle) 
        {           
            IEC60870_5_104EventID eventID = (IEC60870_5_104EventID)CheckPriorityEvents();
            if (eventID == (IEC60870_5_104EventID)EventID.None)
            {
                GetWaitNewDataEventConditions(out TimeSpan timeOut, out bool someJobsPenging);

                // if some jobs are pending, force execution
                if (someJobsPenging)
                    SetRequestSchedule();

                //wait for answer
                int eventNr = WaitHandle.WaitAny(waitHandles, timeOut);
                eventID = (IEC60870_5_104EventID)CheckPriorityEvents();
                if (eventID == (IEC60870_5_104EventID)EventID.None)
                {
                    //waitHandles = new WaitHandle[] { CommunicationEvent, NewDataToAnlyze };
                    switch (eventNr)
                    {
                        case 0:
                            eventID = (IEC60870_5_104EventID)EventID.NewJobExecution;
                            ResetCommunicationThread();
                            break;
                        case 1:
                            ResetNewDataEvent();
                            lock (lockThreadObject)
                            {
                                if (ReceiveBuffer.Count == 0)
                                    eventID = IEC60870_5_104EventID.TimeOut;
                                else
                                    eventID = IEC60870_5_104EventID.NewDataToAnalize;
                            }
                            break;
                        case System.Threading.WaitHandle.WaitTimeout:
                            eventID = IEC60870_5_104EventID.TimeOut;
                            break;
                    }                        
                }                
            }

            return eventID;
        }

        protected override void CommunicationToDeviceThread(object data)
        {
            station = CommDriver.GetChannelStations(this)[0] as IEC60870_5_104Station;
            station.createMemoryData();
            ReceiveItem receiveItem = new ReceiveItem();

            // only for compatibility with WaitNewEvent method --> not really used
            int sleepCycle = SLEEP_CYCLE_INFINITE;

            lock (lockThreadObject)
                waitHandles = new WaitHandle[] { CommunicationEvent, NewDataToAnlyze };

            if (DeviceOpen())
                LogicInit();

            while (true)
            {
                IEC60870_5_104EventID newEventID = WaitNewEvent(sleepCycle);
                if (newEventID == (IEC60870_5_104EventID)EventID.ThreadTerminate)
                    break;

                lock (lockSuspendJobs)
                {
                    if (IsDeviceOpen())
                    {
                        IEC60870_5_104ErrorCodes error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;
                        bool fatalError = false;
                        ReceiveLoop(ref receiveItem, ref error, ref fatalError, newEventID);
                        LinkLoop(ref receiveItem, ref error, ref fatalError, newEventID);
                        ConsumeSpontaneousResponse(ref receiveItem, ref error, ref fatalError, newEventID);
                        ReadWriteJobLoop(ref receiveItem, ref error, ref fatalError, newEventID);
                        ConfirmeAct(ref receiveItem, ref error, ref fatalError, newEventID);

                        if (fatalError)
                        {
                            ManageConnectionBroken(OnIdleJob);
                            DeviceClose();
                        }

                        receiveItem.isValid = false;
                    }
                    else
                    {
                        ManageConnectionBroken(OnIdleJob);
                        DeviceClose();

                        if (DeviceOpen())
                            LogicInit();
                    }
                }
            }
                        
            DeviceClose();

            lock (lockThreadObject)
                waitHandles = null;
        }

        private void ManageConnectionBroken(CommJob jobInError = null, IEC60870_5_104ErrorCodes error = IEC60870_5_104ErrorCodes.ErrorConnectionBroken)
        {
            // put in error if job was not yet processed
            if (jobInError != null && jobInError.IsPending)
                OnJobExecuted(new ExecutedJobArgs() { ErrorCode = (DriverErrorCodes)error, Job = jobInError });
                        
            station.ManageConnectionBroken(error);
        }

        public bool DeviceWrite(APDU apdu)
        {
            if (apdu.Apci.CFFormat == CFFormats.I)
            {
                apdu.Apci.SendSequenceNumber = senderCounter;
                senderCounter = (ushort)((senderCounter + 1) % 32768);
                if (senderCounter == 0)
                    MaxCounterReceived = true;
                apdu.Apci.ReceiveSequenceNumber = receiverControlCounter;
                apdu.Asdu.UnitId.OrigAdd = OriginatorAddress;
                apdu.Asdu.UnitId.CommAdd = station.CommonAddress;
                NumAck++;
                dtSentIType = DateTime.UtcNow;
            }
            else if (apdu.Apci.CFFormat == CFFormats.S)
            {
                apdu.Apci.ReceiveSequenceNumber = receiverControlCounter;
                NumIncomingITypeAPDUs = 0;
                bEnableStartDtLastIncomingITypeAPDU = true;
            }
            else if ((apdu.Apci.CFFormat == CFFormats.U) && (apdu.Apci.CONTROL != Control.TESTFRCON))
            {
                dtSentUType = DateTime.UtcNow;
                bSentUType = true;
            }
            bool rt = DeviceWrite(apdu.Pack());
            return rt;
        }
        
        public override bool DeviceWrite(byte[] Buffer, uint Count = 0)
        {
            if (Count == 0)
                return base.DeviceWrite(Buffer, (uint)Buffer.Length);
            else
                return base.DeviceWrite(Buffer, Count);
        }

        private void LogicInit()
        {
            station.ClearTimeStamps();
            ReceiveState = ReceiveStates.Init;
            LinkState = LinkStates.BEGIN;
            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
            senderCounter = 0;
            MaxCounterReceived = false;
            receiverControlCounter = 0;
            dtLastIncomingAPDU = DateTime.UtcNow;
            dtLastIncomingITypeAPDU = dtLastIncomingAPDU;
            bEnableStartDtLastIncomingITypeAPDU = true;
            NumIncomingITypeAPDUs = 0;
            NumAck = 0;
            bSentUType = false;
            dtSentUType = DateTime.UtcNow;
            dtSentIType = DateTime.UtcNow;
        }
        #endregion

        #region Properties
        /// <summary>   The Originator Address for this Driver. </summary>
        private byte _OriginatorAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Originator Address for this Driver. </summary>
        ///
        /// <value> The Originator Address for this Driver. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public byte OriginatorAddress
        {
            get
            {
                return _OriginatorAddress;
            }
            set
            {
                _OriginatorAddress = value;
            }
        }

        /// <summary>   The timeout for the initial data exchange procedure. </summary>
        private uint _InitializationTimeOut;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initialization Timeout. </summary>
        ///
        /// <value> The Initialization Timeout. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint InitializationTimeOut
        {
            get
            {
                return _InitializationTimeOut;
            }
            set
            {
                _InitializationTimeOut = value;
            }
        }

        /// <summary>   Repeat initial data request. </summary>
        private bool _RepeatInitialInterrogation;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   If any data is missing, repeat periodically the initial data request until all data have been initialized. </summary>
        ///
        /// <value> Repeat initial data request. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool RepeatInitialInterrogation
        {
            get
            {
                return _RepeatInitialInterrogation;
            }
            set
            {
                _RepeatInitialInterrogation = value;
            }
        }

        /// <summary>   Enable Initial Clock Synchronization. </summary>
        private bool _EnableInitialClockSynchronization;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   If set to True, an initial clock synchronization between the supervisor and the devices is performed. </summary>
        ///
        /// <value> Enable Initial Clock Synchronization. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool EnableInitialClockSynchronization
        {
            get
            {
                return _EnableInitialClockSynchronization;
            }
            set
            {
                _EnableInitialClockSynchronization = value;
            }
        }
        #endregion

        #region Methods       
        private void ReceiveLoop(ref ReceiveItem receiveItem, ref IEC60870_5_104ErrorCodes Error, ref bool fatalError, IEC60870_5_104EventID newEventID)
        {
            if (newEventID == IEC60870_5_104EventID.NewDataToAnalize)
                receiveTcp = true;

            try
            {
                switch (ReceiveState)
                {
                    case ReceiveStates.Init:
                        lock (lockThreadObject)
                            ReceiveBuffer.Clear();
                        receiveTcp = false;
                        //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReceiveLoop - BeginDeviceRead " + DateTime.Now.ToString());
                        BeginDeviceRead(APCI.Size);
                        ReceiveState = ReceiveStates.WaitAPCI;
                        NumIncomingITypeAPDUs = 0;
                        dtLastIncomingAPDU = DateTime.UtcNow;
                        dtLastIncomingITypeAPDU = dtLastIncomingAPDU;
                        bEnableStartDtLastIncomingITypeAPDU = true;
                        bSentUType = false;
                        NumAck = 0;
                        break;

                    case ReceiveStates.WaitAPCI:
                        if (receiveTcp)
                        {
                            lock (lockThreadObject)
                            {
                                if (ReceiveBuffer.Count < APCI.Size)
                                    throw new ArgumentException("Invalid size");
                                int Offset = 0;
                                byte[] Header = new byte[APCI.Size];
                                ReceiveBuffer.CopyTo(0, Header, 0, APCI.Size);
                                receiveItem.Apdu.Apci = new APCI(ref Header, ref Offset);

                                if (receiveItem.Apdu.Apci.start != APCI.START)
                                    throw new ArgumentException("Invalid start");
                                if (receiveItem.Apdu.Apci.AsduLength == 0)
                                {
                                    Offset = 0;
                                    ReceiveBuffer.RemoveRange(0, Header.Length);
                                    receiveItem.Apdu = new APDU(ref Header, ref Offset);
                                    receiveItem.isValid = true;
                                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReceiveLoop - BeginDeviceRead " + DateTime.Now.ToString());
                                    BeginDeviceRead(APCI.Size);
                                }
                                else
                                {
                                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReceiveLoop - BeginDeviceRead " + DateTime.Now.ToString());
                                    BeginDeviceRead(receiveItem.Apdu.Apci.AsduLength);
                                    ReceiveState = ReceiveStates.WaitASDU;
                                    ReceiveLoopTestTime = DateTime.UtcNow;
                                }
                            }
                            receiveTcp = false;
                        }

                        break;
                    case ReceiveStates.WaitASDU:
                        if (receiveTcp)
                        {
                            lock (lockThreadObject)
                            {
                                receiveTcp = false;
                                if (ReceiveBuffer.Count < APCI.Size + receiveItem.Apdu.Apci.AsduLength)
                                    throw new ArgumentException("Invalid size");
                                int Offset = 0;
                                byte[] Frame = new byte[APCI.Size + receiveItem.Apdu.Apci.AsduLength];
                                ReceiveBuffer.CopyTo(0, Frame, 0, Frame.Length);
                                ReceiveBuffer.RemoveRange(0, Frame.Length);
                                receiveItem.Apdu = new APDU(ref Frame, ref Offset);
                                if (receiveItem.Apdu.Apci.CFFormat != CFFormats.U)
                                {
                                    if (receiveItem.Apdu.Apci.SendSequenceNumber != receiverControlCounter)
                                        throw new ArgumentException("Invalid Sequence Number");
                                    receiverControlCounter = (ushort)((receiverControlCounter + 1) % 32768);
                                }
                                ReceiveState = ReceiveStates.WaitAPCI;
                                //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReceiveLoop - BeginDeviceRead " + DateTime.Now.ToString());
                                BeginDeviceRead(APCI.Size);
                                receiveItem.isValid = true;
                            }
                        }
                        else
                        {
                            if ((DateTime.UtcNow - ReceiveLoopTestTime).TotalMilliseconds > Timeout)
                                ReceiveState = ReceiveStates.Init;
                        }
                        break;

                }

                if (receiveItem.isValid)
                {
                    dtLastIncomingAPDU = DateTime.UtcNow;
                    if (receiveItem.Apdu.Apci.CFFormat == CFFormats.I)
                    {
                        NumAck = 0;
                        NumIncomingITypeAPDUs++;
                        if (bEnableStartDtLastIncomingITypeAPDU)
                        {
                            dtLastIncomingITypeAPDU = dtLastIncomingAPDU;
                            bEnableStartDtLastIncomingITypeAPDU = false;
                        }
                    }
                    else if (receiveItem.Apdu.Apci.CFFormat == CFFormats.S)
                    {
                        NumAck = 0;
                    }
                    else if (receiveItem.Apdu.Apci.CFFormat == CFFormats.U &&
                        (receiveItem.Apdu.Apci.CONTROL == Control.STARTDTCON ||
                        receiveItem.Apdu.Apci.CONTROL == Control.STOPDTCON ||
                        receiveItem.Apdu.Apci.CONTROL == Control.TESTFRCON))
                    {
                        bSentUType = false;
                    }
                }

                if ((DateTime.UtcNow - dtSentUType).TotalMilliseconds > station.MissingAckTimeout && bSentUType)
                    throw new ArgumentException("Missing U type response");
                
                if (((DateTime.UtcNow - dtSentIType).TotalMilliseconds > station.MissingAckTimeout && NumAck > 0) || (station.MaxOutAPDU != 0 && NumAck > station.MaxOutAPDU))
                    throw new ArgumentException("Missing Ack Timeout");

                if (((DateTime.UtcNow - dtLastIncomingITypeAPDU).TotalMilliseconds > station.NoInMsgTimeout && NumIncomingITypeAPDUs > 0) || (station.MaxInAPDU > 0 && NumIncomingITypeAPDUs >= station.MaxInAPDU))
                {
                    if (!DeviceWrite(APDUFactory.SType()))
                    {
                        fatalError = true;
                        throw new ArgumentException("Write Error");
                    }
                    else
                        Thread.Sleep(1);
                }
            }
            catch (Exception ex)
            {
                receiveItem.isValid = false;
                Error = IEC60870_5_104ErrorCodes.ErrorRxRead;
                fatalError = true;
            }
        }

        /// <summary>   StartupLoop . </summary>
        private void LinkLoop(ref ReceiveItem receiveItem, ref IEC60870_5_104ErrorCodes Error, ref bool fatalError, IEC60870_5_104EventID newEventID)
        {
            if (Error != (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError)
                return;

            switch (LinkState)
            {
                case LinkStates.BEGIN:
                    ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                    senderCounter = 0;
                    MaxCounterReceived = false;
                    receiverControlCounter = 0;
                    LinkLoopTestTime = DateTime.UtcNow;

                    if (DeviceWrite(APDUFactory.StartDT()))
                        LinkState = LinkStates.WaitCONNECT;
                    else
                    {
                        Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                        fatalError = true;
                    }
                    break;

                case LinkStates.WaitCONNECT:
                    if (receiveItem.isValid && APDUValidator.StartDTConf(ref receiveItem.Apdu))
                    {
                        station.ManageConnectionRestored();
                        receiveItem.isValid = false;
                        if (EnableInitialClockSynchronization)
                            LinkState = LinkStates.StartTIMESYNC;
                        else
                            LinkState = LinkStates.StartGENINTERR;
                    }
                    if (LinkState == LinkStates.WaitCONNECT)
                    {
                        if ((DateTime.UtcNow - LinkLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            fatalError = true;
                        }
                    }
                    break;
                case LinkStates.StartTIMESYNC:
                    LinkLoopTestTime = DateTime.UtcNow;
                    if (DeviceWrite(APDUFactory.ClockSync()))
                        LinkState = LinkStates.WaitTIMESYNC;
                    else
                    {
                        Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                        fatalError = true;
                    }
                    break;
                case LinkStates.WaitTIMESYNC:
                    if (receiveItem.isValid && APDUValidator.TestCommonAddress(ref receiveItem, station.CommonAddress) && APDUValidator.ClockSyncConf(ref receiveItem))
                    {
                        receiveItem.isValid = false;
                        LinkState = LinkStates.StartGENINTERR;
                    }
                    if (LinkState == LinkStates.WaitTIMESYNC)
                    {
                        if ((DateTime.UtcNow - LinkLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            LinkState = LinkStates.BEGIN;
                            Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            fatalError = true;
                        }
                    }
                    break;
                case LinkStates.StartGENINTERR:
                    if (station.AllGeneralInitilized())
                    {
                        LinkState = LinkStates.StarCOUNTINTERR;
                        break;
                    }
                    LinkLoopTestTime = DateTime.UtcNow;
                    if (DeviceWrite(APDUFactory.GeneralInterrogation(CausesOfTrasmission.interrogated_by_station_interrogation)))
                        LinkState = LinkStates.WaitGENINTERRConf;
                    else
                    {
                        Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                        fatalError = true;
                    }
                    break;
                case LinkStates.WaitGENINTERRConf:
                    if (receiveItem.isValid && APDUValidator.TestCommonAddress(ref receiveItem, station.CommonAddress) && APDUValidator.GeneralInterrogationConf(ref receiveItem))
                    {
                        receiveItem.isValid = false;
                        LinkLoopTestTime = DateTime.UtcNow;
                        LinkState = LinkStates.WaitGENINTERREnd;
                    }
                    if (LinkState == LinkStates.WaitGENINTERRConf)
                    {
                        if ((DateTime.UtcNow - LinkLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            LinkState = LinkStates.BEGIN;
                            Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            fatalError = true;
                        }
                    }
                    break;
                case LinkStates.WaitGENINTERREnd:
                    if (receiveItem.isValid && APDUValidator.TestCommonAddress(ref receiveItem, station.CommonAddress) && APDUValidator.GeneralInterrogationEnd(ref receiveItem))
                    {
                        receiveItem.isValid = false;
                        LinkLoopTestTime = DateTime.UtcNow;
                        LinkState = LinkStates.StarCOUNTINTERR;
                    }
                    else
                    {
                        if ((DateTime.UtcNow - LinkLoopTestTime).TotalSeconds > InitializationTimeOut || station.AllGeneralInitilized())
                        {
                            LinkLoopTestTime = DateTime.UtcNow;
                            LinkState = LinkStates.StarCOUNTINTERR;
                        }
                    }

                    break;
                case LinkStates.StarCOUNTINTERR:
                    if (station.AllCounterInitialized())
                    {
                        LinkLoopTestTime = DateTime.UtcNow;
                        LinkState = LinkStates.EndInit;
                        break;
                    }
                    LinkLoopTestTime = DateTime.UtcNow;
                    if (DeviceWrite(APDUFactory.CounterInterrogation(QCCRequests.generalRequestCounter, QCCFreezes.read)))
                        LinkState = LinkStates.WaitCOUNTINTERRConf;
                    else
                    {
                        Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                        fatalError = true;
                    }
                    break;
                case LinkStates.WaitCOUNTINTERRConf:
                    if (receiveItem.isValid && APDUValidator.TestCommonAddress(ref receiveItem, station.CommonAddress) && APDUValidator.CounterInterrogationConf(ref receiveItem))
                    {
                        receiveItem.isValid = false;
                        LinkLoopTestTime = DateTime.UtcNow;
                        LinkState = LinkStates.WaitCOUNTINTERREnd;
                    }
                    if (LinkState == LinkStates.WaitCOUNTINTERRConf)
                    {
                        if ((DateTime.UtcNow - LinkLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            LinkState = LinkStates.BEGIN;
                            Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            fatalError = true;
                        }
                    }
                    break;
                case LinkStates.WaitCOUNTINTERREnd:
                    if (receiveItem.isValid && APDUValidator.TestCommonAddress(ref receiveItem, station.CommonAddress) && APDUValidator.CounterInterrogationEnd(ref receiveItem))
                    {
                        receiveItem.isValid = false;
                        LinkLoopTestTime = DateTime.UtcNow;
                        LinkState = LinkStates.EndInit;
                    }
                    else
                    {
                        if ((DateTime.UtcNow - LinkLoopTestTime).TotalSeconds > InitializationTimeOut || station.AllCounterInitialized())
                        {
                            LinkLoopTestTime = DateTime.UtcNow;
                            LinkState = LinkStates.EndInit;
                        }
                    }
                    break;

                case LinkStates.EndInit:
                    if (RepeatInitialInterrogation && (DateTime.UtcNow - LinkLoopTestTime).TotalSeconds > InitializationTimeOut)
                    {
                        LinkState = LinkStates.StartGENINTERR;
                        break;
                    }
                    if ((DateTime.UtcNow - dtLastIncomingAPDU).TotalMilliseconds > station.NoActivityTimeout && ReceiveState > ReceiveStates.Init)
                    {
                        if (DeviceWrite(APDUFactory.Test()))
                        {
                            LinkState = LinkStates.WaitTestConf;
                            break;
                        }
                        else
                        {
                            Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                            fatalError = true;
                        }
                    }
                    break;
                case LinkStates.WaitTestConf:
                    if (receiveItem.isValid && APDUValidator.TestConf(ref receiveItem.Apdu))
                    {
                        receiveItem.isValid = false;
                        LinkState = LinkStates.EndInit;
                    }
                    if (LinkState == LinkStates.WaitTestConf)
                    {
                        if ((DateTime.UtcNow - dtLastIncomingAPDU).TotalMilliseconds > Timeout + station.NoActivityTimeout)
                        {
                            LinkState = LinkStates.BEGIN;
                            Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            fatalError = true;
                        }
                    }
                    break;
            }
            return;
        }


        private void ConsumeSpontaneousResponse(ref ReceiveItem receiveItem, ref IEC60870_5_104ErrorCodes Error, ref bool fatalError, IEC60870_5_104EventID newEventID)
        {
            if (receiveItem.isValid && APDUValidator.TestCommonAddress(ref receiveItem, station.CommonAddress))
            {
                if (APDUValidator.ASDUSpontaneousInputTypes(ref receiveItem.Apdu, out List<iecInfoObj> InfoObjList))
                {                    
                    foreach (iecInfoObj InfoObj in InfoObjList)
                    {
                        List<IEC60870_5_104CommJob> changedJob = (station.getChangedJobs(InfoObj));
                        if (InfoObj.getValue() != new byte[0])
                        {
                            foreach (IEC60870_5_104CommJob JobExecute in changedJob)
                            {
                                if (JobExecute.Type == LinkType.InputOutput || JobExecute.Type == LinkType.Input)
                                {
                                    (JobExecute.TagsList[0] as IEC60870_5_104Tag).Quality = InfoObj.getQuality();
                                    (JobExecute.TagsList[0] as IEC60870_5_104Tag).Cot = receiveItem.Apdu.Asdu.UnitId.Cause;

                                    base.ExecuteJob(JobExecute);
                                    OnJobExecuted(new ExecutedJobArgs
                                    {
                                        ErrorCode = DriverErrorCodes.ErrorNoError,
                                        Job = JobExecute,
                                        Values = InfoObj.getValue(),
                                        Timestamp = InfoObj.getTimestamp(),
                                    });
                                }
                            }
                        }
                        station.SetTimeStamp(InfoObj, InfoObj.getTimestamp());
                    }
                    receiveItem.isValid = false;
                }
                else if (APDUValidator.SType(ref receiveItem.Apdu))
                {
                    receiveItem.isValid = false;
                }
            }
        }

        /// <summary>  Send Read/Write requests and receive device replies. </summary>
        private void ReadWriteJobLoop(ref ReceiveItem receiveItem, ref IEC60870_5_104ErrorCodes Error, ref bool fatalError, IEC60870_5_104EventID newEventID)
        {
            if (Error != (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError)
                return;

            switch (ReadWriteJobState)
            {
                case ReadWriteJobStates.READ_WRITE_SCHEDULE:
                    if (LinkState < LinkStates.EndInit)
                    {
                        break;
                    }
                    else
                    {
                        if (MaxCounterReceived && station.EnablePeriodicReset)
                        {
                            DeviceClose();
                            break;
                        }

                        if (newEventID == (IEC60870_5_104EventID)EventID.NewJobExecution)
                        {
                            OnIdleJob = GetNextPendingJob() as IEC60870_5_104CommJob;
                                                
                            if (OnIdleJob != null)
                            {
                                ReadWriteJobLoopTestTime = DateTime.UtcNow;

                                if (!OnIdleJob.ReadRequest())
                                {
                                    ReadWriteJobState = ReadWriteJobStates.WRITE_SEND;
                                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.WRITE_SEND scheduled " + DateTime.Now.ToString());
                                }
                                else
                                {
                                    ReadWriteJobState = ReadWriteJobStates.READ_SEND;
                                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.READ_SEND scheduled " + DateTime.Now.ToString());
                                }
                            }
                        }
                    }
                    break;
                case ReadWriteJobStates.WRITE_SEND:
                    {
                        //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.WRITE_SEND start to process " + DateTime.Now.ToString());
                        APDU Apdu;
                        switch (OnIdleJob.ASDUType)
                        {
                            case ASDUSelectableTypes.UploadFile:
                                try
                                {
                                    base.ExecuteJob(OnIdleJob);
                                    OnIdleJob.JobSelectAndCallQualifier = (byte)SelectAndCallQualifier.selectFile;
                                    OnIdleJob.FileSection = 0; // Default value
                                    OnIdleJob.FileLength = 0; // Default value
                                    OnIdleJob.SectionLength = 0; // Default value
                                    OnIdleJob.FileChecksum = 0; // Default value
                                    OnIdleJob.SectionChecksum = 0; // Default value
                                    OnIdleJob.SectionBytes = new List<byte>();
                                    OnIdleJob.FileBytes = new List<byte>();
                                    Apdu = APDUFactory.SelectFileJob(OnIdleJob);

                                    if (!DeviceWrite(Apdu))
                                    {
                                        Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                                        fatalError = true;
                                    }
                                    else
                                    {
                                        ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                        ReadWriteJobState = ReadWriteJobStates.WRITE_FILESELECTANSWER;
                                        OnIdleJob.LastExecutionTime = DateTime.UtcNow;
                                    }
                                }
                                catch (Exception Ex)
                                {
                                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - Exception");
                                    receiveItem.isValid = false;
                                    Error = IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                                    ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                    ExecutedJobArgs e = new ExecutedJobArgs();
                                    e.Job = OnIdleJob;
                                    e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                                    OnJobExecuted(e);
                                }
                                break;

                            default:
                                try
                                {
                                    
                                    base.ExecuteJob(OnIdleJob);
                                    bool sendSelect = OnIdleJob.CheckSendSelect(ReadWriteJobState);
                                    Apdu = APDUFactory.confirmedWriteJob(OnIdleJob, sendSelect);
                                    switch (OnIdleJob.CommandWriteType)
                                    {
                                        case CommandType.Operate:
                                        case CommandType.Select:
                                        case CommandType.SelectExecuteValue:
                                            Apdu.Asdu.UnitId.Cause = CausesOfTrasmission.activation;
                                            break;
                                        case CommandType.DeactivateSelect:
                                        case CommandType.DeactivateOperate:
                                            OnIdleJob.ClearTagListWrite();
                                            Apdu.Asdu.UnitId.Cause = CausesOfTrasmission.deactivation;
                                            break;
                                    }                                    

                                    //System.Diagnostics.Trace.TraceInformation("{0} start send  ", DateTime.Now.ToString("HH:mm:ss.fff"));
                                    if (!DeviceWrite(Apdu))
                                    {
                                        Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                                        //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - Write failed");
                                        fatalError = true;
                                    }
                                    else
                                    {
                                        ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                        if (OnIdleJob.CommandWriteType == CommandType.SelectExecuteValue && ReadWriteJobState == ReadWriteJobStates.WRITE_SEND_SBO)
                                            ReadWriteJobState = ReadWriteJobStates.WRITE_ANSWER_SBO;
                                        else
                                            ReadWriteJobState = ReadWriteJobStates.WRITE_ANSWER;

                                        OnIdleJob.LastExecutionTime = DateTime.UtcNow;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - Exception");
                                    receiveItem.isValid = false;
                                    Error = IEC60870_5_104ErrorCodes.ErrorWriteException;
                                    ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                    ExecutedJobArgs e = new ExecutedJobArgs();
                                    e.Job = OnIdleJob;
                                    e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorWriteException;
                                    OnJobExecuted(e);
                                }
                                break;
                        }
                    }
                    break;
                case ReadWriteJobStates.WRITE_ANSWER:
                case ReadWriteJobStates.WRITE_ANSWER_SBO:
                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.WRITE_ANSWER");
                    if (receiveItem.isValid && APDUValidator.TestCommonAddress(ref receiveItem, station.CommonAddress))
                    {
                        if (APDUValidator.GetError(ref receiveItem, out Error) || APDUValidator.isWriteResponse(ref receiveItem, OnIdleJob))
                        {
                            // if CommandType.SelectBeforeOperate keep alive job to execute write (1st cycle send select)
                            if (Error == IEC60870_5_104ErrorCodes.ErrorNoError && OnIdleJob.CommandWriteType == CommandType.SelectExecuteValue && ReadWriteJobState == ReadWriteJobStates.WRITE_ANSWER)
                            {
                                OnIdleJob.TagsListToWrite.Clear();
                                OnIdleJob.TagsListToWrite.AddRange(OnIdleJob.TagsListOnWriting);
                                OnIdleJob.TagsListOnWriting.Clear();
                                ReadWriteJobState = ReadWriteJobStates.WRITE_SEND_SBO;
                            }
                            else
                            {
                                OnJobExecuted(new ExecutedJobArgs
                                {
                                    ErrorCode = (DriverErrorCodes)Error,
                                    Job = OnIdleJob,
                                });
                                ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.WRITE_ANSWER processed");
                                receiveItem.isValid = false;
                                switch (Error)
                                {
                                    case IEC60870_5_104ErrorCodes.unknown_type_identification:
                                    case IEC60870_5_104ErrorCodes.unknown_cause_of_transmission:
                                    case IEC60870_5_104ErrorCodes.unknown_common_address_of_ASDU:
                                    case IEC60870_5_104ErrorCodes.unknown_information_object_address:
                                    case IEC60870_5_104ErrorCodes.cause_of_transmission_pn:
                                        OnIdleJob.ClearTagListToWrite();
                                        break;
                                }
                            }
                        }
                    }

                    if (ReadWriteJobState == ReadWriteJobStates.WRITE_ANSWER || ReadWriteJobState == ReadWriteJobStates.WRITE_ANSWER_SBO)
                    {
                        if ((DateTime.UtcNow - ReadWriteJobLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            LinkState = LinkStates.BEGIN;
                            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = DriverErrorCodes.ErrorTimeOut,
                                Job = OnIdleJob,
                            });
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_FILESELECTANSWER:
                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.WRITE_FILESELECTANSWER");
                    if (receiveItem.isValid && APDUValidator.TestCommonAddress(ref receiveItem, station.CommonAddress))
                    {
                        // Manage errors
                        // General error
                        if (APDUValidator.GetError(ref receiveItem, out Error))
                        {
                            // Do not mantain active the job in case of errors, so clear its lists TagsListToWrite and TagsListOnWriting
                            OnIdleJob.ClearTagListWrite();
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)Error,
                                Job = OnIdleJob,
                            });
                            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 4 - Setting receiveItem.isValid = false");
                            receiveItem.isValid = false;
                        }

                        else if (APDUValidator.isFileSelectResponse(ref receiveItem, OnIdleJob))
                        {
                            // Received the expected reply, validate it and get the file length
                            uint fileLength = 0;
                            if (!APDUValidator.FileReadyQualifier(ref receiveItem, out Error, out fileLength))
                            {
                                // Do not mantain active the job in case of errors, so clear its lists TagsListToWrite and TagsListOnWriting
                                OnIdleJob.ClearTagListWrite();
                                OnJobExecuted(new ExecutedJobArgs
                                {
                                    ErrorCode = (DriverErrorCodes)Error,
                                    Job = OnIdleJob,
                                });
                                ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                //System.Diagnostics.Debug.WriteLine("readWriteJobLoop 5 - Setting receiveItem.isValid = false");
                                receiveItem.isValid = false;
                            }
                            else
                            {
                                // File ready for the transfer, go to next step (the job will remain pending)
                                OnIdleJob.FileLength = fileLength;
                                OnIdleJob.CurrentFileLength = 0;
                                OnIdleJob.SectionLength = 0; // Default value
                                ReadWriteJobState = ReadWriteJobStates.WRITE_FILEREQUEST;
                                receiveItem.isValid = false;
                            }
                        }
                    }

                    // Check if the timeout is elapsed
                    if (ReadWriteJobState == ReadWriteJobStates.WRITE_FILESELECTANSWER)
                    {
                        if ((DateTime.UtcNow - ReadWriteJobLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            LinkState = LinkStates.BEGIN;
                            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = DriverErrorCodes.ErrorTimeOut,
                                Job = OnIdleJob,
                            });
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_FILEREQUEST:
                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.WRITE_FILEREQUEST");
                    if ((OnIdleJob != null) && !OnIdleJob.ReadRequest() && (OnIdleJob.ASDUType == ASDUSelectableTypes.UploadFile))
                    {
                        APDU Apdu;
                        try
                        {
                            //base.ExecuteJob(OnIdleJob);
                            OnIdleJob.JobSelectAndCallQualifier = (byte)SelectAndCallQualifier.requestFile;
                            OnIdleJob.FileSection = 1; // First section of the file
                            OnIdleJob.SectionLength = 0; // Default value
                            Apdu = APDUFactory.SelectFileJob(OnIdleJob);

                            if (!DeviceWrite(Apdu))
                            {
                                Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                                fatalError = true;
                            }
                            else
                            {
                                ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                ReadWriteJobState = ReadWriteJobStates.WRITE_FILEREQUESTANSWER;
                                OnIdleJob.LastExecutionTime = DateTime.UtcNow;
                            }
                        }
                        catch (Exception Ex)
                        {
                            //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 6 - Setting receiveItem.isValid = false");
                            receiveItem.isValid = false;
                            Error = IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            ExecutedJobArgs e = new ExecutedJobArgs();
                            e.Job = OnIdleJob;
                            e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                            OnJobExecuted(e);
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_FILEREQUESTANSWER:
                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.WRITE_FILEREQUESTANSWER");
                    if (receiveItem.isValid && APDUValidator.TestCommonAddress(ref receiveItem, station.CommonAddress))
                    {
                        // Manage errors
                        // General error
                        if (APDUValidator.GetError(ref receiveItem, out Error))
                        {
                            // Do not mantain active the job in case of errors, so clear its lists TagsListToWrite and TagsListOnWriting
                            OnIdleJob.ClearTagListWrite();
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)Error,
                                Job = OnIdleJob,
                            });
                            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 7 - Setting receiveItem.isValid = false");
                            receiveItem.isValid = false;
                        }

                        else if (APDUValidator.isFileRequestResponse(ref receiveItem, OnIdleJob))
                        {
                            // Received the expected reply, validate it and get the file length
                            uint sectionLength = 0;
                            if (!APDUValidator.SectionReadyQualifier(ref receiveItem, out Error, out sectionLength))
                            {
                                // Do not mantain active the job in case of errors, so clear its lists TagsListToWrite and TagsListOnWriting
                                OnIdleJob.ClearTagListWrite();
                                OnJobExecuted(new ExecutedJobArgs
                                {
                                    ErrorCode = (DriverErrorCodes)Error,
                                    Job = OnIdleJob,
                                });
                                ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 8 - Setting receiveItem.isValid = false");
                                receiveItem.isValid = false;
                            }
                            else
                            {
                                // Section ready for the transfer, go to next step (the job will remain pending)
                                OnIdleJob.SectionLength = sectionLength;
                                OnIdleJob.SectionChecksum = 0; // Default value
                                ReadWriteJobState = ReadWriteJobStates.WRITE_SECTIONREQUEST;
                                receiveItem.isValid = false;
                            }
                        }
                    }

                    // Check if the timeout is elapsed
                    if (ReadWriteJobState == ReadWriteJobStates.WRITE_FILESELECTANSWER)
                    {
                        if ((DateTime.UtcNow - ReadWriteJobLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            LinkState = LinkStates.BEGIN;
                            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = DriverErrorCodes.ErrorTimeOut,
                                Job = OnIdleJob,
                            });
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_SECTIONREQUEST:
                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.WRITE_SECTIONREQUEST");
                    if ((OnIdleJob != null) && !OnIdleJob.ReadRequest() && (OnIdleJob.ASDUType == ASDUSelectableTypes.UploadFile))
                    {
                        APDU Apdu;
                        try
                        {
                            //base.ExecuteJob(OnIdleJob);
                            OnIdleJob.JobSelectAndCallQualifier = (byte)SelectAndCallQualifier.requestSection;
                            OnIdleJob.SectionLength = 0; // Default value
                            OnIdleJob.SectionChecksum = 0; // Default value
                            OnIdleJob.SectionBytes.Clear();
                            Apdu = APDUFactory.SelectFileJob(OnIdleJob);

                            if (!DeviceWrite(Apdu))
                            {
                                Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                                fatalError = true;
                            }
                            else
                            {
                                ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                ReadWriteJobState = ReadWriteJobStates.WRITE_SECTIONREQUESTANSWER;
                                OnIdleJob.LastExecutionTime = DateTime.UtcNow;
                            }
                        }
                        catch (Exception Ex)
                        {
                            //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 9 - Setting receiveItem.isValid = false");
                            receiveItem.isValid = false;
                            Error = IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            ExecutedJobArgs e = new ExecutedJobArgs();
                            e.Job = OnIdleJob;
                            e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                            OnJobExecuted(e);
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_SECTIONREQUESTANSWER:
                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.WRITE_SECTIONREQUESTANSWER");
                    if (receiveItem.isValid && APDUValidator.TestCommonAddress(ref receiveItem, station.CommonAddress))
                    {
                        // Manage errors
                        // General error
                        if (APDUValidator.GetError(ref receiveItem, out Error))
                        {
                            // Do not mantain active the job in case of errors, so clear its lists TagsListToWrite and TagsListOnWriting
                            OnIdleJob.ClearTagListWrite();
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)Error,
                                Job = OnIdleJob,
                            });
                            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 10 - Setting receiveItem.isValid = false");
                            receiveItem.isValid = false;
                        }

                        else if (APDUValidator.isSectionRequestResponse(ref receiveItem, OnIdleJob))
                        {

                            // Received the expected reply, validate it and get the segment data
                            uint segmentLength = 0;
                            byte[] segmentData = null;
                            byte checksum = 0;
                            if (!APDUValidator.Segment(ref receiveItem, out Error, out byte segmentType, ref segmentLength, ref segmentData, ref checksum))
                            {
                                // Do not mantain active the job in case of errors, so clear its lists TagsListToWrite and TagsListOnWriting
                                OnIdleJob.ClearTagListWrite();
                                OnJobExecuted(new ExecutedJobArgs
                                {
                                    ErrorCode = (DriverErrorCodes)Error,
                                    Job = OnIdleJob,
                                });
                                ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                //System.Diagnostics.Debug.WriteLine("readWriteJobLoop 11 - Setting receiveItem.isValid = false");
                                receiveItem.isValid = false;
                            }
                            else
                            {
                                // Segment received go to next step (the job will remain pending):
                                // received next segment, or, if this is the last segment, request the next section, or, if this is the last segment of the last section, complete the file reception
                                switch (segmentType)
                                {
                                    // Received data of the segment
                                    case (byte)SegmentType.dataSegment:
                                        OnIdleJob.CurrentFileLength += segmentLength;
                                        OnIdleJob.SectionBytes.AddRange(segmentData.ToList());
                                        ReadWriteJobState = ReadWriteJobStates.WRITE_SECTIONREQUESTANSWER;
                                        ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                        receiveItem.isValid = false;
                                        break;

                                    // Received the last section of the file: job completed 
                                    case (byte)SegmentType.lastSection:
                                        OnIdleJob.FileChecksum = checksum;
                                        ReadWriteJobState = ReadWriteJobStates.WRITE_ACKNOWLEDGEFILE;
                                        ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                        receiveItem.isValid = false;
                                        break;

                                    // Received the last segment of a section of the file 
                                    case (byte)SegmentType.lastSegment:
                                        OnIdleJob.SectionChecksum = checksum;
                                        ReadWriteJobState = ReadWriteJobStates.WRITE_ACKNOWLEDGESECTION;
                                        ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                        receiveItem.isValid = false;
                                        break;
                                }
                            }
                        }
                    }

                    // Check if the timeout is elapsed
                    if (ReadWriteJobState == ReadWriteJobStates.WRITE_SECTIONREQUESTANSWER)
                    {
                        if ((DateTime.UtcNow - ReadWriteJobLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            LinkState = LinkStates.BEGIN;
                            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = DriverErrorCodes.ErrorTimeOut,
                                Job = OnIdleJob,
                            });
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_ACKNOWLEDGESECTION:
                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.WRITE_ACKNOWLEDGESECTION");
                    if ((OnIdleJob != null) && !OnIdleJob.ReadRequest() && (OnIdleJob.ASDUType == ASDUSelectableTypes.UploadFile))
                    {
                        APDU Apdu;
                        try
                        {
                            //base.ExecuteJob(OnIdleJob);
                            // Check the received data
                            if (IEC60870_5_104Protocol.isChecksumCorrect(OnIdleJob.SectionBytes, OnIdleJob.SectionChecksum))
                            {
                                OnIdleJob.JobAcknowledgeFileQualifier = (byte)AcknowledgeFileQualifier.positiveAcknowledgeOfSectionTransfer;
                                OnIdleJob.FileBytes.AddRange(OnIdleJob.SectionBytes);
                                OnIdleJob.SectionBytes.Clear();
                                //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - Section checksum OK");
                            }
                            else
                            {
                                OnIdleJob.JobAcknowledgeFileQualifier = (byte)AcknowledgeFileQualifier.negativeAcknowledgeOfSectionTransfer;
                                OnIdleJob.SectionBytes.Clear();
                                //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - Wrong section checksum");
                            }
                            Apdu = APDUFactory.AcknowledgeFile(OnIdleJob);

                            if (!DeviceWrite(Apdu))
                            {
                                Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                                fatalError = true;
                            }
                            else
                            {
                                ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                OnIdleJob.LastExecutionTime = DateTime.UtcNow;
                                if (OnIdleJob.JobAcknowledgeFileQualifier == (byte)AcknowledgeFileQualifier.positiveAcknowledgeOfSectionTransfer)
                                {
                                    if (OnIdleJob.CurrentFileLength < OnIdleJob.FileLength)
                                    {
                                        // Increment the section name
                                        OnIdleJob.FileSection++;
                                        OnIdleJob.SectionLength = 0; // Default value
                                                                     // the ready section message for the next file section should be sent by the device without any further request
                                                                     //WriteJobState = WriteJobStates.SECTIONREQUEST;
                                        ReadWriteJobState = ReadWriteJobStates.WRITE_FILEREQUESTANSWER;
                                    }
                                    else
                                    {
                                        // No more section to be requested --> Wait for the last section message
                                        ReadWriteJobState = ReadWriteJobStates.WRITE_SECTIONREQUESTANSWER;
                                    }
                                }
                                else
                                {
                                    // File transfer interrupted because of a wrong checksum --> Wait for the last section message
                                    ReadWriteJobState = ReadWriteJobStates.WRITE_ACKNOWLEDGEFILE;
                                }
                            }
                        }
                        catch (Exception Ex)
                        {
                            //System.Diagnostics.Debug.WriteLine("readWriteJobLoop 12 - Setting receiveItem.isValid = false");
                            receiveItem.isValid = false;
                            Error = IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            ExecutedJobArgs e = new ExecutedJobArgs();
                            e.Job = OnIdleJob;
                            e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                            OnJobExecuted(e);
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_ACKNOWLEDGEFILE:
                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.WRITE_ACKNOWLEDGEFILE");
                    if ((OnIdleJob != null) && !OnIdleJob.ReadRequest() && (OnIdleJob.ASDUType == ASDUSelectableTypes.UploadFile))
                    {
                        if (OnIdleJob.JobAcknowledgeFileQualifier == (byte)AcknowledgeFileQualifier.negativeAcknowledgeOfSectionTransfer)
                        {
                            Error = IEC60870_5_104ErrorCodes.ErrorFileTransferChecksum;
                            OnIdleJob.SectionBytes.Clear();
                            OnIdleJob.FileBytes.Clear();
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)Error,
                                Job = OnIdleJob,
                            });
                            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 13 - Setting receiveItem.isValid = false");
                            receiveItem.isValid = false;
                        }
                        else
                        {
                            APDU Apdu;
                            try
                            {
                                //base.ExecuteJob(OnIdleJob);
                                // Check the received data
                                if (IEC60870_5_104Protocol.isChecksumCorrect(OnIdleJob.FileBytes, OnIdleJob.FileChecksum))
                                {
                                    OnIdleJob.JobAcknowledgeFileQualifier = (byte)AcknowledgeFileQualifier.positiveAcknowledgeOfFileTransfer;
                                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - File checksum OK");
                                }
                                else
                                {
                                    OnIdleJob.JobAcknowledgeFileQualifier = (byte)AcknowledgeFileQualifier.negativeAcknowledgeOfSectionTransfer;
                                    Error = IEC60870_5_104ErrorCodes.ErrorFileTransferChecksum;
                                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - Wrong file checksum");
                                }
                                Apdu = APDUFactory.AcknowledgeFile(OnIdleJob);

                                if (!DeviceWrite(Apdu))
                                {
                                    Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                                    fatalError = true;
                                }
                                else
                                {
                                    // Save the file to disk
                                    Error = OnIdleJob.SaveFile();
                                    OnIdleJob.SectionBytes.Clear();
                                    OnIdleJob.FileBytes.Clear();
                                    // Received the last section of the file: job completed 
                                    OnJobExecuted(new ExecutedJobArgs
                                    {
                                        ErrorCode = (DriverErrorCodes)Error,
                                        Job = OnIdleJob,
                                    });
                                    ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 14 - Setting receiveItem.isValid = false");
                                    receiveItem.isValid = false;
                                }
                            }
                            catch (Exception Ex)
                            {
                                //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 15 - Setting receiveItem.isValid = false");
                                receiveItem.isValid = false;
                                Error = IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                                ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                ExecutedJobArgs e = new ExecutedJobArgs();
                                e.Job = OnIdleJob;
                                e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                                OnJobExecuted(e);
                            }
                        }
                    }
                    break;

                case ReadWriteJobStates.READ_SEND:
                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.READ_SEND");
                    {
                        APDU Apdu;
                        switch (OnIdleJob.ASDUType)
                        {
                            case ASDUSelectableTypes.ReadDirectoryContents:
                                try
                                {
                                    base.ExecuteJob(OnIdleJob);
                                    OnIdleJob.JobSelectAndCallQualifier = (byte)SelectAndCallQualifier.selectFile;
                                    OnIdleJob.FileSection = 0; // Default value
                                    OnIdleJob.FileLength = 0; // Default value
                                    OnIdleJob.SectionLength = 0; // Default value
                                    OnIdleJob.FileChecksum = 0; // Default value
                                    OnIdleJob.SectionChecksum = 0; // Default value
                                    OnIdleJob.DirectoryBytes = new List<byte>();
                                    Apdu = APDUFactory.SelectFileJob(OnIdleJob);

                                    if (!DeviceWrite(Apdu))
                                    {
                                        Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                                        fatalError = true;
                                    }
                                    else
                                    {
                                        ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                        ReadWriteJobState = ReadWriteJobStates.READ_CALLDIRECTORYANSWER;
                                        OnIdleJob.LastExecutionTime = DateTime.UtcNow;
                                    }
                                }
                                catch (Exception Ex)
                                {
                                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 16 - Setting receiveItem.isValid = false");
                                    receiveItem.isValid = false;
                                    Error = IEC60870_5_104ErrorCodes.ErrorDirectoryException;
                                    ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                    ExecutedJobArgs e = new ExecutedJobArgs();
                                    e.Job = OnIdleJob;
                                    e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorDirectoryException;
                                    OnJobExecuted(e);
                                }
                                break;
                            default:
                                ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                if (OnIdleJob != null)
                                {
                                    RemovePendingJob(OnIdleJob);
                                    OnIdleJob.LastExecutionTime = DateTime.UtcNow;
                                }
                                break;
                        }
                    }
                    break;

                case ReadWriteJobStates.READ_CALLDIRECTORYANSWER:
                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.CALLDIRECTORYANSWER");
                    if (receiveItem.isValid && APDUValidator.TestCommonAddress(ref receiveItem, station.CommonAddress))
                    {
                        // Manage errors
                        // General error
                        if (APDUValidator.GetError(ref receiveItem, out Error))
                        {
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)Error,
                                Job = OnIdleJob
                            });
                            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 17 - Setting receiveItem.isValid = false");
                            receiveItem.isValid = false;
                        }

                        else if (APDUValidator.isDirectoryResponse(ref receiveItem, OnIdleJob))
                        {
                            // Received the expected reply, validate it and get the file length
                            uint directoryLength = 0;
                            byte[] directoryData = null;
                            uint directoryNumberOfElements = 0;
                            bool directoryComplete = false;
                            if (!APDUValidator.Directory(ref receiveItem, out Error, ref directoryLength, ref directoryData, ref directoryNumberOfElements, ref directoryComplete))
                            {
                                OnJobExecuted(new ExecutedJobArgs
                                {
                                    ErrorCode = (DriverErrorCodes)Error,
                                    Job = OnIdleJob
                                });
                                ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 18 - Setting receiveItem.isValid = false");
                                receiveItem.isValid = false;
                            }
                            else
                            {
                                // Get the directory data
                                OnIdleJob.DirectoryBytes.AddRange(directoryData.ToList());
                                // Directory complete?
                                if (!directoryComplete)
                                {
                                    // Directory not complete: wait for the next data 
                                    ReadWriteJobState = ReadWriteJobStates.READ_CALLDIRECTORYANSWER;
                                    ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                }
                                else
                                {
                                    try
                                    {
                                        // Received all the directory contents: job completed
                                        // Parse the received data
                                        byte[] directoryContents = OnIdleJob.DirectoryBytes.ToArray();
                                        string fileList = String.Empty;
                                        IEC60870_5_104Protocol.ParseDirectoryData(ref directoryContents, ref fileList);
                                        byte[] directoryBuffer = System.Text.Encoding.UTF8.GetBytes(fileList);
                                        //System.Diagnostics.Debug.WriteLine(String.Format("IEC60870-5-104Channel-ReadWriteJobLoop - directoryContents.Length: {0} - fileList: {1} - directoryBuffer.Length: {2}", directoryContents.Length, fileList, directoryBuffer.Length));
                                        // Set the size of the job and of its tag equal to the received string length

                                        if (OnIdleJob.TagsList.Count > 0)
                                        {
                                            OnIdleJob.TagsList[0].Size = (uint)directoryBuffer.Length;
                                            OnIdleJob.TotalJobSize = (uint)directoryBuffer.Length;
                                        }

                                        OnJobExecuted(new ExecutedJobArgs
                                        {
                                            ErrorCode = (DriverErrorCodes)Error,
                                            Job = OnIdleJob,
                                            Values = directoryBuffer
                                        });
                                        ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                        //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 19 - Setting receiveItem.isValid = false");
                                        receiveItem.isValid = false;
                                    }
                                    catch (Exception Ex)
                                    {
                                        //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop 20 - Setting receiveItem.isValid = false");
                                        receiveItem.isValid = false;
                                        Error = IEC60870_5_104ErrorCodes.ErrorDirectoryException;
                                        ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                        ExecutedJobArgs e = new ExecutedJobArgs();
                                        e.Job = OnIdleJob;
                                        e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorDirectoryException;
                                        OnJobExecuted(e);
                                    }
                                    break;
                                }
                            }
                        }
                    }

                    // Check if the timeout is elapsed
                    if (ReadWriteJobState == ReadWriteJobStates.READ_CALLDIRECTORYANSWER)
                    {
                        if ((DateTime.UtcNow - ReadWriteJobLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            LinkState = LinkStates.BEGIN;
                            ReadWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = DriverErrorCodes.ErrorTimeOut,
                                Job = OnIdleJob
                            });
                        }
                    }
                    break;

                case ReadWriteJobStates.READ_WRITE_END:
                    //System.Diagnostics.Debug.WriteLine("IEC60870-5-104Channel-ReadWriteJobLoop - ReadWriteJobStates.READ_WRITE_END");
                    break;
            }
            return;
        }

        /// <summary>   ConfirmeAct . </summary>
        private void ConfirmeAct(ref ReceiveItem receiveItem, ref IEC60870_5_104ErrorCodes Error, ref bool fatalError, IEC60870_5_104EventID newEventID)
        {
            if (Error != (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError)
                return;
            if (!receiveItem.isValid)
                return;

            if (APDUValidator.StartDTAct(ref receiveItem.Apdu))
            {
                receiveItem.isValid = false;
                if (!DeviceWrite(APDUFactory.StartDTConf()))
                {
                    Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                    fatalError = true;
                }
            }
            else if (APDUValidator.TestAct(ref receiveItem.Apdu))
            {
                receiveItem.isValid = false;
                if (!DeviceWrite(APDUFactory.TestConf()))
                {
                    Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                    fatalError = true;
                }
            }
            else if (APDUValidator.StopDTAct(ref receiveItem.Apdu))
            {
                receiveItem.isValid = false;
                if (!DeviceWrite(APDUFactory.StopDTconf()))
                {
                    Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                    fatalError = true;
                }
            }
            return;
        }
        #endregion

        public override void Dispose()
        {            
            base.Dispose();
        }
    }
}
