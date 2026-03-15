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
        public enum ReadWriteJobStates
        {
            READ_WRITE_SCHEDULE,
            WRITE_SEND,
            WRITE_ANSWER,
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
            : base(commdriver, settings, null, true) // enable to relaunch automatically BeginDeviceRead()
        {
            _OriginatorAddress = settings.OriginatorAddress;
            _InitializationTimeOut = settings.InitializationTimeOut;
            _RepeatInitialInterrogation = settings.RepeatInitialInterrogation;
            _EnableInitialClockSynchronization = settings.EnableInitialClockSynchronization;
            senderCounter = 0;
            receiverControlCounter = 0;
            receiveState = ReceiveStates.Init;            
            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
            maxCounterReceived = false;
            _LastMessageFromDevice = DateTime.UtcNow;
        }

        #endregion

        #region Member
        private ushort senderCounter;  // sender packet counter
        private ushort receiverControlCounter;  // receiver packet control counter
        IEC60870_5_104Station Station;
        ReceiveStates receiveState;
        IEC60870_5_104CommJob OnIdleJob;
        DateTime dtSentUType;
        DateTime dtSentIType;
        DateTime dtLastIncomingAPDU;
        bool bEnableStartDtLastIncomingITypeAPDU;
        DateTime dtLastIncomingITypeAPDU;
        int NumIncomingITypeAPDUs;
        int NumAck;
        bool bSentUType;
        bool maxCounterReceived;
        ReadWriteJobStates readWriteJobState;
        DateTime ReadWriteJobLoopTestTime;     

        private int waitNewDataEventTimeOut = 0;
        private ReceiveItem receiveItem = null;
        private bool anyJobActive = false;
        private IEC60870_5_104CommJob activeJob = null;
        private object currentJobLock = new object();

        #endregion

        #region Override Methods

        bool serverParseDataReceivedSubscribed = false;
        public override bool DeviceOpen()
        {
            bool result = base.DeviceOpen();
            if (result && !serverParseDataReceivedSubscribed)
            {
                BeginDeviceRead(APCI.Size);
                serverParseDataReceivedSubscribed = true;
                this.ServerParseDataReceived += IEC60870_5_104Channel_NewDataReceived;
            }

            return result;
        }

        public override bool DeviceClose()
        {
            if (serverParseDataReceivedSubscribed)
            {
                serverParseDataReceivedSubscribed = false;
                this.ServerParseDataReceived -= IEC60870_5_104Channel_NewDataReceived;
            }
            return base.DeviceClose();
        }

        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob exjob)
        {
            // manage connection error into ProcessNewData
            if (conn != DriverErrorCodes.ErrorNoError)
                return false;

            IEC60870_5_104CommJob mJob = exjob as IEC60870_5_104CommJob;

            base.ExecuteJob(exjob);

            mJob.CheckIECState();

            // is initialization terminated ?
            if (mJob.IsIECJobState())
                mJob.GetReadWriteState();

            switch (mJob.IecState)
            {
                case IEC60870_5_104CommJob.IECStates.StationInitialization:
                    IEC60870_5_104ErrorCodes errorState = StationSendLinkLoop(mJob);
                    if (errorState != (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError)
                    {
                        conn = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorTxWrite;
                        // manage error into ProcessNewData() and put station in error
                        return false;
                    }                    
                    break;

                case IEC60870_5_104CommJob.IECStates.FileManagement:
                    //ReadWriteJobLoop(ref receiveItem, out IEC60870_5_104ErrorCodes dummy2, out bool fatalError2);
                    break;

                case IEC60870_5_104CommJob.IECStates.WaitData:
                    break;

                //    case BACnetCommJob.BACnetState.UnknownObject:
                //        OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = (DriverErrorCodes)BACnetErrorCodes.ErrorUnknownObject, Job = mJob });

                //        return false;
                //        break;
            }

            return true;
        }

        public override void WaitNewDataEvent(ref DriverErrorCodes conn)
        {
            // custom timeout execution --> only 1 shot
            if (waitNewDataEventTimeOut != 0)
            {
                base.WaitNewDataEvent(ref conn, waitNewDataEventTimeOut);
                waitNewDataEventTimeOut = 0;
            }
            else
            {
                base.WaitNewDataEvent(ref conn);
            }
            if (conn != DriverErrorCodes.ErrorNoError)
                UnSetActiveJob(null);
        }

        public override bool ProcessNewData(DriverErrorCodes conn, CommJob exjob)
        {
            bool reciveGenericMessageFromDevice = false;

            #region check communication's result
            DriverErrorCodes error = DriverErrorCodes.ErrorNoError;
            switch (conn)
            {
                case DriverErrorCodes.ErrorDeviceOpenFailed:
                    PutAllJobsInError();
                    OnJobExecuted(new ExecutedJobArgs { ErrorCode = conn, Job = exjob, Values = null });
                    return true;
                case DriverErrorCodes.ErrorTimeOut: // manage at station level, don't close/reopen channel
                    error = DriverErrorCodes.ErrorTimeOut;
                    // a message (that doesn't match driver request) come from device --> usefull in case I want to distinguish a "full" timeout from UnconfirmedMessage
                    if (DateTime.UtcNow.Subtract(LastMessageFromDevice).TotalMilliseconds < Timeout)
                        reciveGenericMessageFromDevice = true;
                    break;
                default:
                    // during driver dispose, possible case 
                    if (receiveItem == null)
                    {
                        error = DriverErrorCodes.ErrorTimeOut;
                    }
                    else
                    {
                        //// check error containt into answer message
                        //if (IPFrameValidator.isErrorFrame(receiveItem.Frame))
                        //{
                        //    error = (DriverErrorCodes)IPFrameValidator.GetError(receiveItem.Frame);
                        //}
                        //else
                        //{
                        //    if (IPFrameValidator.isRejectFrame(receiveItem.Frame))
                        //        error = (DriverErrorCodes)BACnetErrorCodes.ErrorInvalidDataType;
                        //}
                    }
                    break;
            }
            #endregion

            IEC60870_5_104CommJob mJob = exjob as IEC60870_5_104CommJob;
            switch (mJob.IecState)
            {
                case IEC60870_5_104CommJob.IECStates.StationInitialization:
                    switch (Station.LinkState)
                    {
                        case IEC60870_5_104Station.LinkStates.CONNECT:                                
                            if (error != DriverErrorCodes.ErrorNoError)
                            {
                                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-ProcessNewData-->LinkStates.CONNECT->Error");
                                PutAllJobsInError();
                                OnJobExecuted(new ExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                            }
                            else
                            {
                                Station.ManageConnectionRestored();

                                Station.StartDTReceived = true;
                                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-ProcessNewData-->StartDTReceived=true");

                                // keep job pending
                                return false;
                            }
                            break;
                        case IEC60870_5_104Station.LinkStates.TIMESYNC:
                            if (error != DriverErrorCodes.ErrorNoError)
                            {
                                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-ProcessNewData-->LinkStates.TIMESYNC->Error");
                                PutAllJobsInError();
                                OnJobExecuted(new ExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                            }
                            else
                            {
                                Station.TimeSyncronized = true;
                                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-ProcessNewData-->TimeSyncronized=true");

                                // keep job pending
                                return false;
                            }
                            break;
                        case IEC60870_5_104Station.LinkStates.GENINTERRConf:
                            if (error != DriverErrorCodes.ErrorNoError)
                            {
                                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-ProcessNewData-->LinkStates.GENINTERRConf->Error");
                                PutAllJobsInError();
                                OnJobExecuted(new ExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                            }
                            else
                            {
                                Station.GeneralInterrConfReceived = true;
                                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-ProcessNewData-->GeneralInterrConfReceived=true");

                                // keep job pending
                                return false;
                            }
                            break;
                        case IEC60870_5_104Station.LinkStates.GENINTERREnd:
                            if (error != DriverErrorCodes.ErrorNoError)
                            {
                                if (Station.AllCounterInitialized())
                                    error = DriverErrorCodes.ErrorNoError;
                            }

                            if (error != DriverErrorCodes.ErrorNoError)
                            {
                                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-ProcessNewData-->LinkStates.GENINTERRConf->Error");
                                PutAllJobsInError();
                                OnJobExecuted(new ExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                            }
                            else
                            {
                                Station.GeneralInterrEndReceived = true;
                                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-ProcessNewData-->GeneralInterrEndReceived = true");
                                // keep job pending
                                return false;
                            }
                            break;

                        case IEC60870_5_104Station.LinkStates.WaitTestConf:
                            if (APDUValidator.TestConf(ref receiveItem.Apdu))
                            {
                                receiveItem.IsValid = false;
                                Station.LinkState = IEC60870_5_104Station.LinkStates.EndInit;
                            }
                            Station.LinkState = IEC60870_5_104Station.LinkStates.END;
                            //if (Station.LinkState == IEC60870_5_104Station.LinkStates.WaitTestConf)
                            //{
                            //    if ((DateTime.UtcNow - dtLastIncomingAPDU).TotalMilliseconds > Timeout + Station.NoActivityTimeout)
                            //    {
                            //        Station.LinkState = IEC60870_5_104Station.LinkStates.BEGIN;
                            //        error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            //        fatalError = true;
                            //    }
                            //}
                            break;
                        case IEC60870_5_104Station.LinkStates.END:
                            break;
                    }                    
                    break;

                case IEC60870_5_104CommJob.IECStates.FileManagement:
                    //ReadWriteJobLoop(ref receiveItem, out IEC60870_5_104ErrorCodes dummy2, out bool fatalError2);
                    break;

                case IEC60870_5_104CommJob.IECStates.WaitData:
                    break;

                    //    case BACnetCommJob.BACnetState.BBMDInitialization:
                    //        switch (((BACnetStation)mJob.Station).BBDMDevice.BacnetState)
                    //        {
                    //            case BACnetBBMDDevice.BACnetState.RegisterAsForeignDevice:
                    //                if (error != DriverErrorCodes.ErrorNoError)
                    //                {
                    //                    PutAllStationsAndJobsInError(((BACnetStation)mJob.Station).BBDMDevice.RelatedStations);
                    //                    OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    //                }
                    //                else
                    //                {
                    //                    ((BACnetStation)mJob.Station).BBDMDevice.BacnetState = BACnetBBMDDevice.BACnetState.ReadDFTTable;
                    //                    // keep job pending
                    //                    return false;
                    //                }
                    //                break;

                    //            case BACnetBBMDDevice.BACnetState.ReadDFTTable:
                    //                if (error != DriverErrorCodes.ErrorNoError)
                    //                {
                    //                    PutAllStationsAndJobsInError(((BACnetStation)mJob.Station).BBDMDevice.RelatedStations);
                    //                    OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    //                }
                    //                else// if (IPFrameValidator.IAm(receiveItem.Frame, out IAmAnswer iAmAnswer))
                    //                {
                    //                    ((BACnetStation)mJob.Station).BBDMDevice.BacnetState = BACnetBBMDDevice.BACnetState.ReadBDTTable;
                    //                    // keep job pending
                    //                    return false;
                    //                }
                    //                break;

                    //            case BACnetBBMDDevice.BACnetState.ReadBDTTable:
                    //                if (error != DriverErrorCodes.ErrorNoError)
                    //                {
                    //                    PutAllStationsAndJobsInError(((BACnetStation)mJob.Station).BBDMDevice.RelatedStations);
                    //                    OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    //                }
                    //                else// if (IPFrameValidator.IAm(receiveItem.Frame, out IAmAnswer iAmAnswer))
                    //                {
                    //                    ((BACnetStation)mJob.Station).BBDMDevice.SetRegistred();
                    //                    // keep job pending
                    //                    return false;
                    //                }
                    //                break;

                    //            case BACnetBBMDDevice.BACnetState.RefreshRegistration:
                    //                if (error != DriverErrorCodes.ErrorNoError)
                    //                {
                    //                    PutAllStationsAndJobsInError(((BACnetStation)mJob.Station).BBDMDevice.RelatedStations);
                    //                    OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    //                }
                    //                else
                    //                {
                    //                    ((BACnetStation)mJob.Station).BBDMDevice.SetRegistred();
                    //                    // keep job pending
                    //                    return false;
                    //                }
                    //                break;
                    //        }
                    //        break;

                    //    case BACnetCommJob.BACnetState.WhoIs:
                    //        if (error != DriverErrorCodes.ErrorNoError)
                    //        {
                    //            ((BACnetStation)mJob.Station).PutAllJobsInError();

                    //            OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });

                    //        }
                    //        else if (IPFrameValidator.IAm(receiveItem.Frame, out IAmAnswer iAmAnswer))
                    //        {
                    //            if (!((BACnetStation)mJob.Station).HasDeviceInstance())
                    //                ((BACnetStation)mJob.Station).RunTimeDeviceInstance = (int)iAmAnswer.DeviceIdentifier.instance;

                    //            InitStationWithWhoIsData((BACnetStation)mJob.Station, iAmAnswer.DeviceIdentifier, iAmAnswer.MaxAPDULength, iAmAnswer.VendorIdentifier, iAmAnswer.DestinationSpecifierPresent, iAmAnswer.DNET, iAmAnswer.DLEN, iAmAnswer.DADR);

                    //            mJob.LastExecutionTime = DateTime.UtcNow;
                    //            // keep job pending
                    //            return false;
                    //        }
                    //        break;

                    //    case BACnetCommJob.BACnetState.WhoHas:
                    //        if (error != DriverErrorCodes.ErrorNoError)
                    //        {
                    //            // WhoAs answer in 
                    //            if (error == DriverErrorCodes.ErrorTimeOut)
                    //                error = (DriverErrorCodes)BACnetErrorCodes.ErrorUnknownProperty;

                    //            if (error == (DriverErrorCodes)BACnetErrorCodes.ErrorUnknownProperty)
                    //                mJob.BacnetState = BACnetCommJob.BACnetState.UnknownObject;

                    //            OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    //        }
                    //        else if (IPFrameValidator.IHave(receiveItem.Frame, (mJob.Station as BACnetStation).DeviceIdentifier, mJob.ObjectName, out BACnetObjectIdentifier objectID))
                    //        {
                    //            if (!mJob.HasRunTimeInstanceNumber())
                    //                mJob.RunTimeInstanceNumber = (int)objectID.instance;

                    //            mJob.SetObjectID();

                    //            // keep job pending
                    //            return false;
                    //        }
                    //        break;

                    //    case BACnetCommJob.BACnetState.Cov:
                    //        if (error == DriverErrorCodes.ErrorNoError)
                    //        {
                    //            if (IPFrameValidator.isCovSubscriptionFailed(receiveItem.Frame, mJob.InvokeID))
                    //            {
                    //                mJob.BacnetState = BACnetCommJob.BACnetState.UnknownObject;
                    //                error = (DriverErrorCodes)BACnetErrorCodes.ErrorCovSubscriptionFailed;
                    //            }
                    //        }

                    //        if (error != DriverErrorCodes.ErrorNoError)
                    //        {
                    //            error = (DriverErrorCodes)BACnetErrorCodes.ErrorCovSubscriptionFailed;
                    //            CovAssignUnSubscriberID(mJob, error);
                    //            OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    //        }
                    //        else
                    //        {
                    //            if (IPFrameValidator.isCovSubscriptionOk(receiveItem.Frame, mJob.InvokeID))
                    //            {
                    //                SetLastCOVExecutionTime(mJob, (DriverErrorCodes)error);

                    //                // change state from BACnetState.Cov to BACnetState.CovSubscribed
                    //                mJob.CheckBacNetState();

                    //                // keep job alive to get initial data (if necessary) by polling request
                    //                if (mJob.BacnetState == BACnetCommJob.BACnetState.CovSubscribedPolling)
                    //                    return false;
                    //                else // cov subscribed
                    //                    OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    //            }
                    //        }

                    //        break;

                    //    case BACnetCommJob.BACnetState.CovSubscribedPolling:
                    //        if (error != DriverErrorCodes.ErrorNoError)
                    //        {
                    //            OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    //        }
                    //        else
                    //        {
                    //            if (IPFrameValidator.ConfirmedReadProperty(receiveItem.Frame, mJob.InvokeID, mJob.ObjectID, mJob.PropertyIdentifier, out byte[] answer, mJob.ArrayIndex, mJob))
                    //            {
                    //                SetLastCOVExecutionTime(mJob, (DriverErrorCodes)error);

                    //                // change state from BACnetState.Cov to BACnetState.CovSubscribed
                    //                mJob.CheckBacNetState();

                    //                OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = true });
                    //            }
                    //            break;
                    //        }
                    //        break;

                    //    case BACnetCommJob.BACnetState.CovSubscribed:
                    //        switch (mJob.CommandType)
                    //        {
                    //            case BACnetCommJob.CommandTypes.ReadCmd:
                    //                // WhoIs management
                    //                if (error != DriverErrorCodes.ErrorNoError)
                    //                {
                    //                    ((BACnetStation)mJob.Station).PutAllJobsInError();

                    //                    //OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    //                }
                    //                SetLastCOVExecutionTime(mJob, (DriverErrorCodes)error);

                    //                //if (IPFrameValidator.IAm(receiveItem.Frame, out IAmAnswer iAmAnswer))
                    //                //{
                    //                //    // do nothing here    
                    //                //}
                    //                OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    //                break;
                    //            case BACnetCommJob.CommandTypes.WriteCmd:
                    //                bool relinquishCmd = mJob.IsRelinquishRequest();
                    //                OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    //                if (error == DriverErrorCodes.ErrorNoError)
                    //                {
                    //                    // keep job pending in case of input/output to allow driver to read "current" device value
                    //                    if (relinquishCmd)
                    //                    {
                    //                        if (mJob.Type == LinkType.InputOutput)
                    //                            return false;
                    //                    }
                    //                }
                    //                break;
                    //        }
                    //        break;

                    //    case BACnetCommJob.BACnetState.Polling:
                    //        if (error != DriverErrorCodes.ErrorNoError)
                    //        {
                    //            switch (error)
                    //            {
                    //                case DriverErrorCodes.ErrorTimeOut:
                    //                    ((BACnetStation)mJob.Station).PutAllJobsInError();
                    //                    break;
                    //                case (DriverErrorCodes)BACnetErrorCodes.ErrorUnknownObject:
                    //                    mJob.BacnetState = BACnetCommJob.BACnetState.UnknownObject;
                    //                    break;
                    //            }
                    //            OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    //        }
                    //        else
                    //        {
                    //            switch (mJob.CommandType)
                    //            {
                    //                case BACnetCommJob.CommandTypes.ReadCmd:
                    //                    if (IPFrameValidator.ConfirmedReadProperty(receiveItem.Frame, mJob.InvokeID, mJob.ObjectID, mJob.PropertyIdentifier, out byte[] answer, mJob.ArrayIndex, mJob))
                    //                        OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = answer, IsRead = true });
                    //                    break;
                    //                case BACnetCommJob.CommandTypes.WriteCmd:
                    //                    bool relinquishCmd = mJob.IsRelinquishRequest();
                    //                    OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    //                    // keep job pending in case of input/output to allow driver to read "current" device value
                    //                    if (relinquishCmd)
                    //                    {
                    //                        if (mJob.Type == LinkType.InputOutput)
                    //                            return false;
                    //                    }
                    //                    break;
                    //            }
                    //        }
                    //        break;
                    //    case BACnetCommJob.BACnetState.UnknownObject:
                    //        // do nothing here
                    //        break;
            }

            if (error == DriverErrorCodes.ErrorNoError)
            {
                // initialization sequence terminated; some writing operation pending ?
                //if (mJob.HasPendingWriteCmd())
                //    return false;
            }

            return true;
        }

        private void PutAllJobsInError()
        {
            Station.PutAllJobsInError();
        }

        public bool DeviceWriteUsingActiveJob(APDU apdu)
        {
            return DeviceWrite(apdu, activeJob);
        }

        public bool DeviceWrite(APDU apdu, IEC60870_5_104CommJob job = null)
        {
            switch (apdu.Apci.CFFormat)
            {
                case CFFormats.I:
                    apdu.Apci.SendSequenceNumber = senderCounter;
                    senderCounter = (ushort)((senderCounter + 1) % 32768);
                    if (senderCounter == 0)
                        maxCounterReceived = true;
                    apdu.Apci.ReceiveSequenceNumber = receiverControlCounter;
                    apdu.Asdu.UnitId.OrigAdd = OriginatorAddress;
                    apdu.Asdu.UnitId.CommAdd = Station.CommonAddress;
                    NumAck++;
                    dtSentIType = DateTime.UtcNow;
                    break;
                case CFFormats.S:
                    apdu.Apci.ReceiveSequenceNumber = receiverControlCounter;
                    NumIncomingITypeAPDUs = 0;
                    bEnableStartDtLastIncomingITypeAPDU = true;
                    break;
                case CFFormats.U:
                    if (apdu.Apci.CONTROL != Control.TESTFRCON)
                    {
                        dtSentUType = DateTime.UtcNow;
                        bSentUType = true;
                    }
                    break;
            }
            bool rt = DeviceWrite(apdu.Pack());
            if (rt)
                SetActiveJob(job);
            return rt;
        }

        public override bool DeviceWrite(byte[] Buffer, uint Count = 0)
        {
            if (Count == 0)
                return base.DeviceWrite(Buffer, (uint)Buffer.Length);
            else
                return base.DeviceWrite(Buffer, Count);

        }

        //public bool DeviceWrite(byte[] Buffer, uint Count = 0, IEC60870_5_104CommJob job = null)
        //{
        //    bool rt = base.DeviceWrite(Buffer, (Count == 0 ? (uint)Buffer.Length : Count));
        //    if (rt)
        //        SetActiveJob(job);

        //    return rt;
        //}

        public override bool Startup()
        {
            if (!bChannelStarted)
            {
                LogicInit();

                lock (lockThreadObject)
                {
                    //this.ServerParseDataReceived += IEC60870_5_104Channel_NewDataReceived;
                    //SocketManager.ClientDisconnect += IEC60870_5_104Channel_ClientDisconnect;

                    //BeginDeviceRead(APCI.Size);                    
                }

                //StartTmrCheckDeviceCyclically();
            }
            return base.Startup();
        }

        public override void Suspend()
        {
            lock (lockThreadObject)
            {
                //this.ServerParseDataReceived -= IEC60870_5_104Channel_NewDataReceived;
                //SocketManager.ClientDisconnect -= IEC60870_5_104Channel_ClientDisconnect;
            }
            
            StopTmrCheckDeviceCyclically();            

            base.Suspend();
        }

        event EventHandler<NewDataReceivedRelaunchArgs> ServerParseDataReceived;

        public override void UpdateReceiveBuffer(object sender, byte[] rec, DateTime dt, out bool setNewDataEvent, out int beginDeviceReadSize)
        {
            lock (lockThreadObject)
                ReceiveBuffer.AddRange(rec);

            beginDeviceReadSize = rec.Length;            
            EventHandler<NewDataReceivedRelaunchArgs> temp = ServerParseDataReceived;
            if (temp != null)
            {
                NewDataReceivedRelaunchArgs e = new NewDataReceivedRelaunchArgs();
                e.Sender = sender;
                e.Timestamp = dt;
                e.RxBytes = new List<byte>();
                e.RxBytes.AddRange(ReceiveBuffer);
                e.NextBeginDeviceReadSize = beginDeviceReadSize;

                temp(this, e);
                if (e.NextBeginDeviceReadSize > 0)
                    beginDeviceReadSize = e.NextBeginDeviceReadSize;
                
                setNewDataEvent = e.SetNewDataEvent;

                lock (lockThreadObject)
                {
                    ReceiveBuffer.Clear();
                    ReceiveBuffer.AddRange(e.RxBytes);
                }
            }
            else
            {            
                setNewDataEvent = true;
            }
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

        private DateTime _LastMessageFromDevice;
        public DateTime LastMessageFromDevice
        {
            get { return _LastMessageFromDevice; }
            set { _LastMessageFromDevice = value; }
        }
        #endregion

        #region methods

        private ReceiveItem callBackReceiveItem = new ReceiveItem();
        private void IEC60870_5_104Channel_NewDataReceived(object sender, NewDataReceivedRelaunchArgs e)
        {
            //StopTmrCheckDeviceCyclically();            
            lock (lockThreadObject)
            {
                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-IEC60870_5_104Channel_NewDataReceived-->ReceiveLoop");
                ReceiveLoop(ref callBackReceiveItem, ref e);                
            }               

            try
            {
                if (callBackReceiveItem.IsValid)
                {
                    dtLastIncomingAPDU = DateTime.UtcNow;
                    if (callBackReceiveItem.Apdu.Apci.CFFormat == CFFormats.I)
                    {
                        NumAck = 0;
                        NumIncomingITypeAPDUs++;
                        if (bEnableStartDtLastIncomingITypeAPDU)
                        {
                            dtLastIncomingITypeAPDU = dtLastIncomingAPDU;
                            bEnableStartDtLastIncomingITypeAPDU = false;
                        }
                    }
                    else if (callBackReceiveItem.Apdu.Apci.CFFormat == CFFormats.S)
                    {
                        NumAck = 0;
                    }
                    else if (callBackReceiveItem.Apdu.Apci.CFFormat == CFFormats.U &&
                        (callBackReceiveItem.Apdu.Apci.CONTROL == Control.STARTDTCON ||
                        callBackReceiveItem.Apdu.Apci.CONTROL == Control.STOPDTCON ||
                        callBackReceiveItem.Apdu.Apci.CONTROL == Control.TESTFRCON))
                    {
                        bSentUType = false;
                    }
                }
                if ((DateTime.UtcNow - dtSentUType).TotalMilliseconds > Station.MissingAckTimeout && bSentUType)
                    throw new ArgumentException("Missing U type response");
                if (((DateTime.UtcNow - dtSentIType).TotalMilliseconds > Station.MissingAckTimeout && NumAck > 0) ||
                    (Station.MaxOutAPDU != 0 && NumAck > Station.MaxOutAPDU))
                    throw new ArgumentException("Missing Ack Timeout");
                if (((DateTime.UtcNow - dtLastIncomingITypeAPDU).TotalMilliseconds > Station.NoInMsgTimeout && NumIncomingITypeAPDUs > 0) ||
                    (Station.MaxInAPDU > 0 && NumIncomingITypeAPDUs >= Station.MaxInAPDU))
                {
                    if (!DeviceWriteUsingActiveJob(APDUFactory.SType()))
                    {
                        throw new ArgumentException("Write Error");
                    }
                }
            }
            catch (Exception ex)
            {
                receiveItem.Error = IEC60870_5_104ErrorCodes.ErrorRxRead;
            }

            // some error occours ?
            if (callBackReceiveItem.Error != (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                callBackReceiveItem.Reset();
                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-IEC60870_5_104Channel_NewDataReceived-->data incomplete --> wait next call back");
                return;
            } // no data --> CallBack already restarted             

            // data incomplete --> wait 
            if (!callBackReceiveItem.IsValid)
            {                
                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-IEC60870_5_104Channel_NewDataReceived-->data incomplete --> wait next call back");
                return;
            }
            
            #region not solicited/spontaneous data management
            if (APDUValidator.TestCommonAddress(ref callBackReceiveItem, Station.CommonAddress) && APDUValidator.IsASDUSpontaneousInputTypes(ref callBackReceiveItem.Apdu))
            {
                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-IEC60870_5_104Channel_NewDataReceived-->not solicited/spontaneous data management");
                ConsumeSpontaneousResponse(ref callBackReceiveItem);                
                return;
            }
            #endregion

            // no active job (scheduled job)
            if (!AnyActiveJob())
            {                
                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-IEC60870_5_104Channel_NewDataReceived-->Data not processed because no active job");
                // discard data
                return;
            }
            else
            {
                switch (activeJob.IecState)
                {
                    case IEC60870_5_104CommJob.IECStates.StationInitialization:
                        switch (Station.LinkState)
                        {
                            //case IEC60870_5_104Station.LinkStates.BEGIN: --> not managed
                            case IEC60870_5_104Station.LinkStates.CONNECT:
                                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-IEC60870_5_104Channel_NewDataReceived-->LinkStates.BEGIN-CONNECT");
                                if (APDUValidator.StartDTConf(ref callBackReceiveItem.Apdu))
                                {
                                    System.Diagnostics.Debug.WriteLine("IEC60870-5.104-IEC60870_5_104Channel_NewDataReceived-->LinkStates.BEGIN-CONNECT data processed");
                                    UnSetActiveJob(callBackReceiveItem);
                                    e.SetNewDataEvent = true;
                                }
                                break;
                            case IEC60870_5_104Station.LinkStates.TIMESYNC:
                                if (APDUValidator.TestCommonAddress(ref callBackReceiveItem, Station.CommonAddress) && APDUValidator.ClockSyncConf(ref callBackReceiveItem))
                                {
                                    System.Diagnostics.Debug.WriteLine("IEC60870-5.104-IEC60870_5_104Channel_NewDataReceived-->LinkStates.TYMESYNC data processed");
                                    UnSetActiveJob(callBackReceiveItem);
                                    e.SetNewDataEvent = true;
                                }
                                break;
                            
                            case IEC60870_5_104Station.LinkStates.GENINTERRConf:
                                if (APDUValidator.TestCommonAddress(ref callBackReceiveItem, Station.CommonAddress) && APDUValidator.GeneralInterrogationConf(ref callBackReceiveItem))
                                {
                                    System.Diagnostics.Debug.WriteLine("IEC60870-5.104-IEC60870_5_104Channel_NewDataReceived-->LinkStates.GENINTERRConf data processed");
                                    UnSetActiveJob(callBackReceiveItem);
                                    e.SetNewDataEvent = true;
                                }
                                break;

                            case IEC60870_5_104Station.LinkStates.GENINTERREnd:
                                if (APDUValidator.TestCommonAddress(ref callBackReceiveItem, Station.CommonAddress) || APDUValidator.GeneralInterrogationEnd(ref callBackReceiveItem))
                                {
                                    System.Diagnostics.Debug.WriteLine("IEC60870-5.104-IEC60870_5_104Channel_NewDataReceived-->LinkStates.GENINTERREnd data processed");
                                    UnSetActiveJob(callBackReceiveItem);
                                    e.SetNewDataEvent = true;
                                }
                                break;

                            case IEC60870_5_104Station.LinkStates.COUNTINTERR:
                                if (APDUValidator.TestCommonAddress(ref callBackReceiveItem, Station.CommonAddress) && APDUValidator.CounterInterrogationConf(ref callBackReceiveItem))
                                {
                                    System.Diagnostics.Debug.WriteLine("IEC60870-5.104-IEC60870_5_104Channel_NewDataReceived-->LinkStates.COUNTINTERR data processed");
                                    UnSetActiveJob(callBackReceiveItem);
                                    e.SetNewDataEvent = true;
                                }
                                break;

                            case IEC60870_5_104Station.LinkStates.COUNTINTERREnd:
                                if (APDUValidator.TestCommonAddress(ref callBackReceiveItem, Station.CommonAddress) || APDUValidator.CounterInterrogationEnd(ref callBackReceiveItem))
                                {
                                    System.Diagnostics.Debug.WriteLine("IEC60870-5.104-IEC60870_5_104Channel_NewDataReceived-->LinkStates.COUNTINTERREnd data processed");
                                    UnSetActiveJob(callBackReceiveItem);
                                    e.SetNewDataEvent = true;
                                }
                                break;
                            case IEC60870_5_104Station.LinkStates.END:
                                break;
                        }
                        //if (errorState != (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError)
                        //{
                        //    conn = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorTxWrite;
                        //    // manage error into ProcessNewData() and put station in error
                        //    return false;
                        //}
                        break;

                    case IEC60870_5_104CommJob.IECStates.FileManagement:
                        //ReadWriteJobLoop(ref callBackReceiveItem, out IEC60870_5_104ErrorCodes dummy2, out bool fatalError2);
                        break;

                    case IEC60870_5_104CommJob.IECStates.WaitData:
                        break;
                }
            }

            //StartTmrCheckDeviceCyclically();
        }

        private void LogicInit()
        {
            Station = CommDriver.GetChannelStations(this)[0] as IEC60870_5_104Station;
            Station.ClearTimeStamps();
            receiveState = ReceiveStates.Init;
            Station.LinkState = IEC60870_5_104Station.LinkStates.BEGIN;
            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
            senderCounter = 0;
            maxCounterReceived = false;
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

        private void ReceiveLoop(ref ReceiveItem receiveItem, ref NewDataReceivedRelaunchArgs e)
        {
            receiveItem.Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;

            if (receiveState == ReceiveStates.Init)
            {
                e.NextBeginDeviceReadSize = APCI.Size;
                receiveState = ReceiveStates.WaitAPCI;
                NumIncomingITypeAPDUs = 0;
                dtLastIncomingAPDU = DateTime.UtcNow;
                dtLastIncomingITypeAPDU = dtLastIncomingAPDU;
                bEnableStartDtLastIncomingITypeAPDU = true;
                bSentUType = false;
                NumAck = 0;
                receiveItem.Reset();
                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-ReceiveLoop-ReceiveStates.Init");
            }

            if (receiveItem.IsValid)
            {
                receiveState = ReceiveStates.WaitAPCI;
                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-ReceiveLoop-ReceiveStates.Init");
                receiveItem.Reset();
            }

            lock (lockThreadObject)
            {
                try
                {
                    switch (receiveState)
                    {
                        case ReceiveStates.WaitAPCI:
                            {
                                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-ReceiveLoop-ReceiveStates.WaitAPCI");
                                if (e.RxBytes.Count < APCI.Size)
                                    throw new ArgumentException("Invalid size");

                                LastMessageFromDevice = DateTime.UtcNow;

                                int offset = 0;
                                byte[] Header = new byte[APCI.Size];
                                e.RxBytes.CopyTo(0, Header, 0, APCI.Size);                                
                                receiveItem.Apdu.Apci = new APCI(ref Header, ref offset);

                                if (receiveItem.Apdu.Apci.start != APCI.START)
                                    throw new ArgumentException("Invalid start");
                                if (receiveItem.Apdu.Apci.AsduLength == 0)
                                {
                                    offset = 0;
                                    e.RxBytes.RemoveRange(0, Header.Length);
                                    receiveItem.Apdu = new APDU(ref Header, ref offset);
                                    receiveItem.IsValid = true;
                                    e.NextBeginDeviceReadSize = APCI.Size;
                                }
                                else
                                {
                                    e.NextBeginDeviceReadSize = receiveItem.Apdu.Apci.AsduLength;
                                    receiveState = ReceiveStates.WaitASDU;                                    
                                }
                                break;
                            }

                        case ReceiveStates.WaitASDU:
                            {
                                System.Diagnostics.Debug.WriteLine("IEC60870-5.104-ReceiveLoop-ReceiveStates.WaitASDU");
                                if (e.RxBytes.Count < APCI.Size + receiveItem.Apdu.Apci.AsduLength)
                                    throw new ArgumentException("Invalid size");

                                LastMessageFromDevice = DateTime.UtcNow;

                                int offset = 0;
                                byte[] Frame = new byte[APCI.Size + receiveItem.Apdu.Apci.AsduLength];
                                e.RxBytes.CopyTo(0, Frame, 0, Frame.Length);
                                e.RxBytes.RemoveRange(0, Frame.Length);
                                receiveItem.Apdu = new APDU(ref Frame, ref offset);
                                if (receiveItem.Apdu.Apci.CFFormat != CFFormats.U)
                                {
                                    if (receiveItem.Apdu.Apci.SendSequenceNumber != receiverControlCounter)
                                        throw new ArgumentException("Invalid Sequence Number");
                                    receiverControlCounter = (ushort)((receiverControlCounter + 1) % 32768);
                                }
                                receiveState = ReceiveStates.WaitAPCI;
                                e.NextBeginDeviceReadSize = APCI.Size;
                                // data complete
                                receiveItem.IsValid = true;                                
                                break;
                            }
                    }                    
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("IEC60870-5.104-ReceiveLoop-Unhandled exception " + ex.Message);
                    receiveItem.Error = IEC60870_5_104ErrorCodes.ErrorRxRead;
                    receiveState = ReceiveStates.Init;
                    e.NextBeginDeviceReadSize = APCI.Size;
                }
            }
        }

        internal void InitLinkLoop()
        {
            System.Diagnostics.Debug.WriteLine("IEC60870-5.104-InitLinkLoop()");

            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
            senderCounter = 0;
            maxCounterReceived = false;
            receiverControlCounter = 0;

            Station.StartDTReceived = false;
            Station.TimeSyncronized = false;

            Station.GeneralInterrConfReceived = false;
            Station.GeneralInterrEndReceived = false;
        }

        /// <summary>   StartupLoop . </summary>
        private IEC60870_5_104ErrorCodes StationSendLinkLoop(IEC60870_5_104CommJob job)
        {
            IEC60870_5_104ErrorCodes error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;

            switch (Station.LinkState)
            {
                case IEC60870_5_104Station.LinkStates.CONNECT:                                        
                    if (!DeviceWrite(APDUFactory.StartDT(), job))
                    {
                        error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                    }
                    System.Diagnostics.Debug.WriteLine(String.Format("IEC60870-5.104-StationSendLinkLoop-->LinkStates.CONNECT--Error(0)", error));
                    break;

                case IEC60870_5_104Station.LinkStates.TIMESYNC:
                    if (!DeviceWrite(APDUFactory.ClockSync(), job))
                    {
                        error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                    }
                    System.Diagnostics.Debug.WriteLine(String.Format("IEC60870-5.104-StationSendLinkLoop-->LinkStates.TIMESYNC--Error(0)", error));
                    break;

                case IEC60870_5_104Station.LinkStates.GENINTERRConf:
                    if (!DeviceWrite(APDUFactory.GeneralInterrogation(CausesOfTrasmission.interrogated_by_station_interrogation), job))
                    {
                        error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                    }
                    System.Diagnostics.Debug.WriteLine(String.Format("IEC60870-5.104-StationSendLinkLoop-->LinkStates.GENINTERRConf--Error(0)", error));
                    break;

                case IEC60870_5_104Station.LinkStates.GENINTERREnd:
                    // do nothing --> increase timeout (InitializationTimeOut) to wait more data
                    System.Diagnostics.Debug.WriteLine("IEC60870-5.104-StationSendLinkLoop-->LinkStates.GENINTERREnd");
                    waitNewDataEventTimeOut = (int)_InitializationTimeOut;
                    break;

                case IEC60870_5_104Station.LinkStates.COUNTINTERR:
                    if (DeviceWrite(APDUFactory.CounterInterrogation(QCCRequests.generalRequestCounter, QCCFreezes.read), job))
                    {
                        error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                    }
                    break;

                case IEC60870_5_104Station.LinkStates.COUNTINTERREnd:
                    // do nothing --> increase timeout (InitializationTimeOut) to wait more data
                    waitNewDataEventTimeOut = (int)_InitializationTimeOut;
                    break;

                //case IEC60870_5_104Station.LinkStates.EndInit:
                //    if (RepeatInitialInterrogation && (DateTime.UtcNow - LinkLoopTestTime).TotalSeconds > InitializationTimeOut)
                //    {
                //        Station.LinkState = IEC60870_5_104Station.LinkStates.StartGENINTERR;
                //        break;
                //    }
                //    if ((DateTime.UtcNow - dtLastIncomingAPDU).TotalMilliseconds > Station.NoActivityTimeout && ReceiveState > ReceiveStates.Init)
                //    {
                //        if (DeviceWrite(APDUFactory.Test()))
                //        {
                //            Station.LinkState = IEC60870_5_104Station.LinkStates.WaitTestConf;
                //            break;
                //        }
                //        else
                //        {
                //            error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                //            fatalError = true;
                //        }
                //    }
                //    break;
                //case IEC60870_5_104Station.LinkStates.WaitTestConf:
                //    if (receiveItem.isValid && APDUValidator.TestConf(ref receiveItem.Apdu))
                //    {
                //        receiveItem.isValid = false;
                //        Station.LinkState = IEC60870_5_104Station.LinkStates.EndInit;
                //    }
                //    Station.LinkState = IEC60870_5_104Station.LinkStates.END;
                //    //if (Station.LinkState == IEC60870_5_104Station.LinkStates.WaitTestConf)
                //    //{
                //    //    if ((DateTime.UtcNow - dtLastIncomingAPDU).TotalMilliseconds > Timeout + Station.NoActivityTimeout)
                //    //    {
                //    //        Station.LinkState = IEC60870_5_104Station.LinkStates.BEGIN;
                //    //        error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                //    //        fatalError = true;
                //    //    }
                //    //}
                //    break;
                case IEC60870_5_104Station.LinkStates.END:
                    break;
            }

            return error;
        }

        private void ConsumeSpontaneousResponse(ref ReceiveItem receiveItem)
        {
            List<iecInfoObj> infoObjList = APDUValidator.GetASDUSpontaneousInputTypes(ref receiveItem.Apdu);
            if (infoObjList != null)
            { 
                foreach (iecInfoObj InfoObj in infoObjList)
                {
                    List<IEC60870_5_104CommJob> changedJob = (Station.getChangedJobs(InfoObj));
                    if (InfoObj.getValue() != new byte[0])
                    {
                        foreach (IEC60870_5_104CommJob JobExecute in changedJob)
                        {
                            if (JobExecute.Type == LinkType.InputOutput || JobExecute.Type == LinkType.Input)
                            {
                                (JobExecute.TagsList[0] as IEC60870_5_104Tag).Quality = InfoObj.getQuality();
                                (JobExecute.TagsList[0] as IEC60870_5_104Tag).Cot = receiveItem.Apdu.Asdu.UnitId.Cause;
                                lock (JobExecute.retLockList())
                                {
                                    base.ExecuteJob(JobExecute);
                                }
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
                    Station.SetTimeStamp(InfoObj, InfoObj.getTimestamp());
                }
            }
            //else if (APDUValidator.SType(ref receiveItem.Apdu))
            //{
            //    receiveItem.isValid = false;
            //}            
        }

        /// <summary>  Send Read/Write requests and receive device replies. </summary>
        private void ReadWriteJobLoop(ref ReceiveItem receiveItem, out IEC60870_5_104ErrorCodes error, out bool fatalError)
        {
            error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError;
            fatalError = false;

            switch (readWriteJobState)
            {
                case ReadWriteJobStates.READ_WRITE_SCHEDULE:
                    if (Station.LinkState < IEC60870_5_104Station.LinkStates.EndInit)
                    {
                        break;
                    }
                    else
                    {
                        if (maxCounterReceived && Station.EnablePeriodicReset)
                        {
                            DeviceClose();
                            break;
                        }
                        //ScheduleListJob();
                        OnIdleJob = GetNextPendingJob() as IEC60870_5_104CommJob;
                        if (OnIdleJob != null)
                        {
                            if (!OnIdleJob.ReadRequest())
                            {
                                readWriteJobState = ReadWriteJobStates.WRITE_SEND;
                            }
                            else
                            {
                                readWriteJobState = ReadWriteJobStates.READ_SEND;
                            }
                        }
                    }
                    break;
                case ReadWriteJobStates.WRITE_SEND:
                    {
                        System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - ReadWriteJobStates.WRITE_SEND");
                        APDU Apdu;
                        switch (OnIdleJob.ASDUType)
                        {
                            case ASDUSelectableTypes.UploadFile:
                                try
                                {
                                    lock (OnIdleJob.retLockList())
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
                                    }
                                    if (!DeviceWrite(Apdu))
                                    {
                                        error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                                        fatalError = true;
                                    }
                                    else
                                    {
                                        ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                        readWriteJobState = ReadWriteJobStates.WRITE_FILESELECTANSWER;
                                        OnIdleJob.LastExecutionTime = DateTime.UtcNow;
                                    }
                                }
                                catch (Exception Ex)
                                {
                                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 1 - Setting receiveItem.isValid = false");
                                    receiveItem.IsValid = false;
                                    error = IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                                    readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                    ExecutedJobArgs e = new ExecutedJobArgs();
                                    e.Job = OnIdleJob;
                                    e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                                    OnJobExecuted(e);
                                }
                                break;

                            default:
                                try
                                {
                                    lock (OnIdleJob.retLockList())
                                    {
                                        base.ExecuteJob(OnIdleJob);
                                        Apdu = APDUFactory.confirmedWriteJob(OnIdleJob);
                                    }
                                    Apdu.Asdu.UnitId.Cause = CausesOfTrasmission.activation;
                                    //System.Diagnostics.Trace.TraceInformation("{0} start send  ", DateTime.Now.ToString("HH:mm:ss.fff"));
                                    if (!DeviceWrite(Apdu))
                                    {
                                        error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                                        fatalError = true;
                                    }
                                    else
                                    {
                                        ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                        readWriteJobState = ReadWriteJobStates.WRITE_ANSWER;
                                        OnIdleJob.LastExecutionTime = DateTime.UtcNow;
                                    }
                                }
                                catch (Exception ex)
                                {
                                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 2 - Setting receiveItem.isValid = false");
                                    receiveItem.IsValid = false;
                                    error = IEC60870_5_104ErrorCodes.ErrorWriteException;
                                    readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
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
                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - ReadWriteJobStates.WRITE_ANSWER");
                    if (receiveItem.IsValid && APDUValidator.TestCommonAddress(ref receiveItem, Station.CommonAddress))
                    {
                        if (APDUValidator.GetError(ref receiveItem, out error) || APDUValidator.isWriteResponse(ref receiveItem, OnIdleJob))
                        {
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)error,
                                Job = OnIdleJob,
                            });
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 3 - Setting receiveItem.isValid = false");
                            receiveItem.IsValid = false;
                            switch (error)
                            {
                                case IEC60870_5_104ErrorCodes.unknown_type_identification:
                                case IEC60870_5_104ErrorCodes.unknown_cause_of_transmission:
                                case IEC60870_5_104ErrorCodes.unknown_common_address_of_ASDU:
                                case IEC60870_5_104ErrorCodes.unknown_information_object_address:
                                case IEC60870_5_104ErrorCodes.cause_of_transmission_pn:
                                    OnIdleJob.TagsListToWrite.Clear();
                                    OnIdleJob.TagsListOnWriting.Clear();
                                    break;
                            }
                        }

                    }
                    if (readWriteJobState == ReadWriteJobStates.WRITE_ANSWER)
                    {
                        if ((DateTime.UtcNow - ReadWriteJobLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            Station.LinkState = IEC60870_5_104Station.LinkStates.BEGIN;
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = DriverErrorCodes.ErrorTimeOut,
                                Job = OnIdleJob,
                            });
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_FILESELECTANSWER:
                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - ReadWriteJobStates.WRITE_FILESELECTANSWER");
                    if (receiveItem.IsValid && APDUValidator.TestCommonAddress(ref receiveItem, Station.CommonAddress))
                    {
                        // Manage errors
                        // General error
                        if (APDUValidator.GetError(ref receiveItem, out error))
                        {
                            // Do not mantain active the job in case of errors, so clear its lists TagsListToWrite and TagsListOnWriting
                            OnIdleJob.ClearTagListWrite();
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)error,
                                Job = OnIdleJob,
                            });
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 4 - Setting receiveItem.isValid = false");
                            receiveItem.IsValid = false;
                        }

                        else if (APDUValidator.isFileSelectResponse(ref receiveItem, OnIdleJob))
                        {
                            // Received the expected reply, validate it and get the file length
                            uint fileLength = 0;
                            if (!APDUValidator.FileReadyQualifier(ref receiveItem, out error, out fileLength))
                            {
                                // Do not mantain active the job in case of errors, so clear its lists TagsListToWrite and TagsListOnWriting
                                OnIdleJob.ClearTagListWrite();
                                OnJobExecuted(new ExecutedJobArgs
                                {
                                    ErrorCode = (DriverErrorCodes)error,
                                    Job = OnIdleJob,
                                });
                                readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                System.Diagnostics.Debug.WriteLine("readWriteJobLoop 5 - Setting receiveItem.isValid = false");
                                receiveItem.IsValid = false;
                            }
                            else
                            {
                                // File ready for the transfer, go to next step (the job will remain pending)
                                OnIdleJob.FileLength = fileLength;
                                OnIdleJob.CurrentFileLength = 0;
                                OnIdleJob.SectionLength = 0; // Default value
                                readWriteJobState = ReadWriteJobStates.WRITE_FILEREQUEST;
                                receiveItem.IsValid = false;
                            }
                        }
                    }

                    // Check if the timeout is elapsed
                    if (readWriteJobState == ReadWriteJobStates.WRITE_FILESELECTANSWER)
                    {
                        if ((DateTime.UtcNow - ReadWriteJobLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            Station.LinkState = IEC60870_5_104Station.LinkStates.BEGIN;
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = DriverErrorCodes.ErrorTimeOut,
                                Job = OnIdleJob,
                            });
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_FILEREQUEST:
                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - ReadWriteJobStates.WRITE_FILEREQUEST");
                    if ((OnIdleJob != null) && !OnIdleJob.ReadRequest() && (OnIdleJob.ASDUType == ASDUSelectableTypes.UploadFile))
                    {
                        APDU Apdu;
                        try
                        {
                            lock (OnIdleJob.retLockList())
                            {
                                //base.ExecuteJob(OnIdleJob);
                                OnIdleJob.JobSelectAndCallQualifier = (byte)SelectAndCallQualifier.requestFile;
                                OnIdleJob.FileSection = 1; // First section of the file
                                OnIdleJob.SectionLength = 0; // Default value
                                Apdu = APDUFactory.SelectFileJob(OnIdleJob);
                            }
                            if (!DeviceWrite(Apdu))
                            {
                                error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                                fatalError = true;
                            }
                            else
                            {
                                ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                readWriteJobState = ReadWriteJobStates.WRITE_FILEREQUESTANSWER;
                                OnIdleJob.LastExecutionTime = DateTime.UtcNow;
                            }
                        }
                        catch (Exception Ex)
                        {
                            System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 6 - Setting receiveItem.isValid = false");
                            receiveItem.IsValid = false;
                            error = IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            ExecutedJobArgs e = new ExecutedJobArgs();
                            e.Job = OnIdleJob;
                            e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                            OnJobExecuted(e);
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_FILEREQUESTANSWER:
                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - ReadWriteJobStates.WRITE_FILEREQUESTANSWER");
                    if (receiveItem.IsValid && APDUValidator.TestCommonAddress(ref receiveItem, Station.CommonAddress))
                    {
                        // Manage errors
                        // General error
                        if (APDUValidator.GetError(ref receiveItem, out error))
                        {
                            // Do not mantain active the job in case of errors, so clear its lists TagsListToWrite and TagsListOnWriting
                            OnIdleJob.ClearTagListWrite();
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)error,
                                Job = OnIdleJob,
                            });
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 7 - Setting receiveItem.isValid = false");
                            receiveItem.IsValid = false;
                        }

                        else if (APDUValidator.isFileRequestResponse(ref receiveItem, OnIdleJob))
                        {
                            // Received the expected reply, validate it and get the file length
                            uint sectionLength = 0;
                            if (!APDUValidator.SectionReadyQualifier(ref receiveItem, out error, out sectionLength))
                            {
                                // Do not mantain active the job in case of errors, so clear its lists TagsListToWrite and TagsListOnWriting
                                OnIdleJob.ClearTagListWrite();
                                OnJobExecuted(new ExecutedJobArgs
                                {
                                    ErrorCode = (DriverErrorCodes)error,
                                    Job = OnIdleJob,
                                });
                                readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 8 - Setting receiveItem.isValid = false");
                                receiveItem.IsValid = false;
                            }
                            else
                            {
                                // Section ready for the transfer, go to next step (the job will remain pending)
                                OnIdleJob.SectionLength = sectionLength;
                                OnIdleJob.SectionChecksum = 0; // Default value
                                readWriteJobState = ReadWriteJobStates.WRITE_SECTIONREQUEST;
                                receiveItem.IsValid = false;
                            }
                        }
                    }

                    // Check if the timeout is elapsed
                    if (readWriteJobState == ReadWriteJobStates.WRITE_FILESELECTANSWER)
                    {
                        if ((DateTime.UtcNow - ReadWriteJobLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            Station.LinkState = IEC60870_5_104Station.LinkStates.BEGIN;
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = DriverErrorCodes.ErrorTimeOut,
                                Job = OnIdleJob,
                            });
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_SECTIONREQUEST:
                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - ReadWriteJobStates.WRITE_SECTIONREQUEST");
                    if ((OnIdleJob != null) && !OnIdleJob.ReadRequest() && (OnIdleJob.ASDUType == ASDUSelectableTypes.UploadFile))
                    {
                        APDU Apdu;
                        try
                        {
                            lock (OnIdleJob.retLockList())
                            {
                                //base.ExecuteJob(OnIdleJob);
                                OnIdleJob.JobSelectAndCallQualifier = (byte)SelectAndCallQualifier.requestSection;
                                OnIdleJob.SectionLength = 0; // Default value
                                OnIdleJob.SectionChecksum = 0; // Default value
                                OnIdleJob.SectionBytes.Clear();
                                Apdu = APDUFactory.SelectFileJob(OnIdleJob);
                            }
                            if (!DeviceWrite(Apdu))
                            {
                                error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                                fatalError = true;
                            }
                            else
                            {
                                ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                readWriteJobState = ReadWriteJobStates.WRITE_SECTIONREQUESTANSWER;
                                OnIdleJob.LastExecutionTime = DateTime.UtcNow;
                            }
                        }
                        catch (Exception Ex)
                        {
                            System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 9 - Setting receiveItem.isValid = false");
                            receiveItem.IsValid = false;
                            error = IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            ExecutedJobArgs e = new ExecutedJobArgs();
                            e.Job = OnIdleJob;
                            e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                            OnJobExecuted(e);
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_SECTIONREQUESTANSWER:
                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - ReadWriteJobStates.WRITE_SECTIONREQUESTANSWER");
                    if (receiveItem.IsValid && APDUValidator.TestCommonAddress(ref receiveItem, Station.CommonAddress))
                    {
                        // Manage errors
                        // General error
                        if (APDUValidator.GetError(ref receiveItem, out error))
                        {
                            // Do not mantain active the job in case of errors, so clear its lists TagsListToWrite and TagsListOnWriting
                            OnIdleJob.ClearTagListWrite();
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)error,
                                Job = OnIdleJob,
                            });
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 10 - Setting receiveItem.isValid = false");
                            receiveItem.IsValid = false;
                        }

                        else if (APDUValidator.isSectionRequestResponse(ref receiveItem, OnIdleJob))
                        {

                            // Received the expected reply, validate it and get the segment data
                            uint segmentLength = 0;
                            byte[] segmentData = null;
                            byte checksum = 0;
                            if (!APDUValidator.Segment(ref receiveItem, out error, out byte segmentType, ref segmentLength, ref segmentData, ref checksum))
                            {
                                // Do not mantain active the job in case of errors, so clear its lists TagsListToWrite and TagsListOnWriting
                                OnIdleJob.ClearTagListWrite();
                                OnJobExecuted(new ExecutedJobArgs
                                {
                                    ErrorCode = (DriverErrorCodes)error,
                                    Job = OnIdleJob,
                                });
                                readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                System.Diagnostics.Debug.WriteLine("readWriteJobLoop 11 - Setting receiveItem.isValid = false");
                                receiveItem.IsValid = false;
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
                                        readWriteJobState = ReadWriteJobStates.WRITE_SECTIONREQUESTANSWER;
                                        ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                        receiveItem.IsValid = false;
                                        break;

                                    // Received the last section of the file: job completed 
                                    case (byte)SegmentType.lastSection:
                                        OnIdleJob.FileChecksum = checksum;
                                        readWriteJobState = ReadWriteJobStates.WRITE_ACKNOWLEDGEFILE;
                                        ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                        receiveItem.IsValid = false;
                                        break;

                                    // Received the last segment of a section of the file 
                                    case (byte)SegmentType.lastSegment:
                                        OnIdleJob.SectionChecksum = checksum;
                                        readWriteJobState = ReadWriteJobStates.WRITE_ACKNOWLEDGESECTION;
                                        ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                        receiveItem.IsValid = false;
                                        break;
                                }
                            }
                        }
                    }

                    // Check if the timeout is elapsed
                    if (readWriteJobState == ReadWriteJobStates.WRITE_SECTIONREQUESTANSWER)
                    {
                        if ((DateTime.UtcNow - ReadWriteJobLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            Station.LinkState = IEC60870_5_104Station.LinkStates.BEGIN;
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = DriverErrorCodes.ErrorTimeOut,
                                Job = OnIdleJob,
                            });
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_ACKNOWLEDGESECTION:
                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - ReadWriteJobStates.WRITE_ACKNOWLEDGESECTION");
                    if ((OnIdleJob != null) && !OnIdleJob.ReadRequest() && (OnIdleJob.ASDUType == ASDUSelectableTypes.UploadFile))
                    {
                        APDU Apdu;
                        try
                        {
                            lock (OnIdleJob.retLockList())
                            {
                                //base.ExecuteJob(OnIdleJob);
                                // Check the received data
                                if (IEC60870_5_104Protocol.isChecksumCorrect(OnIdleJob.SectionBytes, OnIdleJob.SectionChecksum))
                                {
                                    OnIdleJob.JobAcknowledgeFileQualifier = (byte)AcknowledgeFileQualifier.positiveAcknowledgeOfSectionTransfer;
                                    OnIdleJob.FileBytes.AddRange(OnIdleJob.SectionBytes);
                                    OnIdleJob.SectionBytes.Clear();
                                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - Section checksum OK");
                                }
                                else
                                {
                                    OnIdleJob.JobAcknowledgeFileQualifier = (byte)AcknowledgeFileQualifier.negativeAcknowledgeOfSectionTransfer;
                                    OnIdleJob.SectionBytes.Clear();
                                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - Wrong section checksum");
                                }
                                Apdu = APDUFactory.AcknowledgeFile(OnIdleJob);
                            }
                            if (!DeviceWrite(Apdu))
                            {
                                error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
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
                                        readWriteJobState = ReadWriteJobStates.WRITE_FILEREQUESTANSWER;
                                    }
                                    else
                                    {
                                        // No more section to be requested --> Wait for the last section message
                                        readWriteJobState = ReadWriteJobStates.WRITE_SECTIONREQUESTANSWER;
                                    }
                                }
                                else
                                {
                                    // File transfer interrupted because of a wrong checksum --> Wait for the last section message
                                    readWriteJobState = ReadWriteJobStates.WRITE_ACKNOWLEDGEFILE;
                                }
                            }
                        }
                        catch (Exception Ex)
                        {
                            System.Diagnostics.Debug.WriteLine("readWriteJobLoop 12 - Setting receiveItem.isValid = false");
                            receiveItem.IsValid = false;
                            error = IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            ExecutedJobArgs e = new ExecutedJobArgs();
                            e.Job = OnIdleJob;
                            e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                            OnJobExecuted(e);
                        }
                    }
                    break;

                case ReadWriteJobStates.WRITE_ACKNOWLEDGEFILE:
                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - ReadWriteJobStates.WRITE_ACKNOWLEDGEFILE");
                    if ((OnIdleJob != null) && !OnIdleJob.ReadRequest() && (OnIdleJob.ASDUType == ASDUSelectableTypes.UploadFile))
                    {
                        if (OnIdleJob.JobAcknowledgeFileQualifier == (byte)AcknowledgeFileQualifier.negativeAcknowledgeOfSectionTransfer)
                        {
                            error = IEC60870_5_104ErrorCodes.ErrorFileTransferChecksum;
                            OnIdleJob.SectionBytes.Clear();
                            OnIdleJob.FileBytes.Clear();
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)error,
                                Job = OnIdleJob,
                            });
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 13 - Setting receiveItem.isValid = false");
                            receiveItem.IsValid = false;
                        }
                        else
                        {
                            APDU Apdu;
                            try
                            {
                                lock (OnIdleJob.retLockList())
                                {
                                    //base.ExecuteJob(OnIdleJob);
                                    // Check the received data
                                    if (IEC60870_5_104Protocol.isChecksumCorrect(OnIdleJob.FileBytes, OnIdleJob.FileChecksum))
                                    {
                                        OnIdleJob.JobAcknowledgeFileQualifier = (byte)AcknowledgeFileQualifier.positiveAcknowledgeOfFileTransfer;
                                        System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - File checksum OK");
                                    }
                                    else
                                    {
                                        OnIdleJob.JobAcknowledgeFileQualifier = (byte)AcknowledgeFileQualifier.negativeAcknowledgeOfSectionTransfer;
                                        error = IEC60870_5_104ErrorCodes.ErrorFileTransferChecksum;
                                        System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - Wrong file checksum");
                                    }
                                    Apdu = APDUFactory.AcknowledgeFile(OnIdleJob);
                                }
                                if (!DeviceWrite(Apdu))
                                {
                                    error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                                    fatalError = true;
                                }
                                else
                                {
                                    // Save the file to disk
                                    error = OnIdleJob.SaveFile();
                                    OnIdleJob.SectionBytes.Clear();
                                    OnIdleJob.FileBytes.Clear();
                                    // Received the last section of the file: job completed 
                                    OnJobExecuted(new ExecutedJobArgs
                                    {
                                        ErrorCode = (DriverErrorCodes)error,
                                        Job = OnIdleJob,
                                    });
                                    readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 14 - Setting receiveItem.isValid = false");
                                    receiveItem.IsValid = false;
                                }
                            }
                            catch (Exception Ex)
                            {
                                System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 15 - Setting receiveItem.isValid = false");
                                receiveItem.IsValid = false;
                                error = IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                                readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                ExecutedJobArgs e = new ExecutedJobArgs();
                                e.Job = OnIdleJob;
                                e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorFileTransferException;
                                OnJobExecuted(e);
                            }
                        }
                    }
                    break;

                case ReadWriteJobStates.READ_SEND:
                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - ReadWriteJobStates.READ_SEND");
                    {
                        APDU Apdu;
                        switch (OnIdleJob.ASDUType)
                        {
                            case ASDUSelectableTypes.ReadDirectoryContents:
                                try
                                {
                                    lock (OnIdleJob.retLockList())
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
                                    }
                                    if (!DeviceWrite(Apdu))
                                    {
                                        error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                                        fatalError = true;
                                    }
                                    else
                                    {
                                        ReadWriteJobLoopTestTime = DateTime.UtcNow;
                                        readWriteJobState = ReadWriteJobStates.READ_CALLDIRECTORYANSWER;
                                        OnIdleJob.LastExecutionTime = DateTime.UtcNow;
                                    }
                                }
                                catch (Exception Ex)
                                {
                                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 16 - Setting receiveItem.isValid = false");
                                    receiveItem.IsValid = false;
                                    error = IEC60870_5_104ErrorCodes.ErrorDirectoryException;
                                    readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                    ExecutedJobArgs e = new ExecutedJobArgs();
                                    e.Job = OnIdleJob;
                                    e.ErrorCode = (DriverErrorCodes)IEC60870_5_104ErrorCodes.ErrorDirectoryException;
                                    OnJobExecuted(e);
                                }
                                break;
                            default:
                                readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                break;
                        }
                    }
                    break;

                case ReadWriteJobStates.READ_CALLDIRECTORYANSWER:
                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - ReadWriteJobStates.CALLDIRECTORYANSWER");
                    if (receiveItem.IsValid && APDUValidator.TestCommonAddress(ref receiveItem, Station.CommonAddress))
                    {
                        // Manage errors
                        // General error
                        if (APDUValidator.GetError(ref receiveItem, out error))
                        {
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = (DriverErrorCodes)error,
                                Job = OnIdleJob
                            });
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 17 - Setting receiveItem.isValid = false");
                            receiveItem.IsValid = false;
                        }

                        else if (APDUValidator.isDirectoryResponse(ref receiveItem, OnIdleJob))
                        {
                            // Received the expected reply, validate it and get the file length
                            uint directoryLength = 0;
                            byte[] directoryData = null;
                            uint directoryNumberOfElements = 0;
                            bool directoryComplete = false;
                            if (!APDUValidator.Directory(ref receiveItem, out error, ref directoryLength, ref directoryData, ref directoryNumberOfElements, ref directoryComplete))
                            {
                                OnJobExecuted(new ExecutedJobArgs
                                {
                                    ErrorCode = (DriverErrorCodes)error,
                                    Job = OnIdleJob
                                });
                                readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 18 - Setting receiveItem.isValid = false");
                                receiveItem.IsValid = false;
                            }
                            else
                            {
                                // Get the directory data
                                OnIdleJob.DirectoryBytes.AddRange(directoryData.ToList());
                                // Directory complete?
                                if (!directoryComplete)
                                {
                                    // Directory not complete: wait for the next data 
                                    readWriteJobState = ReadWriteJobStates.READ_CALLDIRECTORYANSWER;
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
                                        System.Diagnostics.Debug.WriteLine(String.Format("ReadWriteJobLoop - directoryContents.Length: {0} - fileList: {1} - directoryBuffer.Length: {2}", directoryContents.Length, fileList, directoryBuffer.Length));
                                        // Set the size of the job and of its tag equal to the received string length
                                        lock(OnIdleJob.retLockList())
                                        {
                                            if(OnIdleJob.TagsList.Count > 0)
                                            {
                                                OnIdleJob.TagsList[0].Size = (uint)directoryBuffer.Length;
                                                OnIdleJob.TotalJobSize = (uint)directoryBuffer.Length;
                                            }
                                        }
                                        OnJobExecuted(new ExecutedJobArgs
                                        {
                                            ErrorCode = (DriverErrorCodes)error,
                                            Job = OnIdleJob,
                                            Values = directoryBuffer
                                        });
                                        readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                                        System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 19 - Setting receiveItem.isValid = false");
                                        receiveItem.IsValid = false;
                                    }
                                    catch (Exception Ex)
                                    {
                                        System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop 20 - Setting receiveItem.isValid = false");
                                        receiveItem.IsValid = false;
                                        error = IEC60870_5_104ErrorCodes.ErrorDirectoryException;
                                        readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
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
                    if (readWriteJobState == ReadWriteJobStates.READ_CALLDIRECTORYANSWER)
                    {
                        if ((DateTime.UtcNow - ReadWriteJobLoopTestTime).TotalMilliseconds > Timeout)
                        {
                            Station.LinkState = IEC60870_5_104Station.LinkStates.BEGIN;
                            readWriteJobState = ReadWriteJobStates.READ_WRITE_SCHEDULE;
                            error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
                            OnJobExecuted(new ExecutedJobArgs
                            {
                                ErrorCode = DriverErrorCodes.ErrorTimeOut,
                                Job = OnIdleJob
                            });
                        }
                    }
                    break;

                case ReadWriteJobStates.READ_WRITE_END:
                    System.Diagnostics.Debug.WriteLine("ReadWriteJobLoop - ReadWriteJobStates.READ_WRITE_END");
                    break;
            }
            return;
        }

        /// <summary>   ConfirmeAct . </summary>
        private void ConfirmeAct(ref ReceiveItem receiveItem, ref IEC60870_5_104ErrorCodes Error, ref bool fatalError)
        {
            if (Error != (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorNoError)
                return;
            if (!receiveItem.IsValid)
                return ;

            if (APDUValidator.StartDTAct(ref receiveItem.Apdu))
            {
                receiveItem.IsValid = false;
                if (!DeviceWrite(APDUFactory.StartDTConf()))
                {
                    Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                    fatalError = true;
                }
            }
            else if (APDUValidator.TestAct(ref receiveItem.Apdu))
            {
                receiveItem.IsValid = false;
                if (!DeviceWrite(APDUFactory.TestConf()))
                {
                    Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                    fatalError = true;
                }
            }
            else if (APDUValidator.StopDTAct(ref receiveItem.Apdu))
            {
                receiveItem.IsValid = false;
                if (!DeviceWrite(APDUFactory.StopDTconf()))
                {
                    Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
                    fatalError = true;
                }
            }
            return;
        }

        private bool AnyActiveJob()
        {
            return anyJobActive;
        }

        private void SetActiveJob(IEC60870_5_104CommJob job)
        {
            lock (currentJobLock)
            {
                receiveItem = null;
                activeJob = job;
                anyJobActive = true;
            }
        }

        private void UnSetActiveJob(ReceiveItem newItem)
        {
            anyJobActive = false;
            if (newItem != null)
                receiveItem = (ReceiveItem)newItem.Clone();
            else
                receiveItem = null;
        }

        public bool IsFileManagementTerminated()
        {
            return (readWriteJobState == ReadWriteJobStates.READ_WRITE_END);
        }

        private void CheckStationNoActivity()
        {
            //case LinkStates.EndInit:
            //        if (RepeatInitialInterrogation && (DateTime.UtcNow - LinkLoopTestTime).TotalSeconds > InitializationTimeOut)
            //{
            //    LinkState = LinkStates.StartGENINTERR;
            //    break;
            //}
            //if ((DateTime.UtcNow - dtLastIncomingAPDU).TotalMilliseconds > Station.NoActivityTimeout && ReceiveState > ReceiveStates.Init)
            //{
            //    if (DeviceWrite(APDUFactory.Test()))
            //    {
            //        LinkState = LinkStates.WaitTestConf;
            //        break;
            //    }
            //    else
            //    {
            //        Error = IEC60870_5_104ErrorCodes.ErrorTxWrite;
            //        fatalError = true;
            //    }
            //}
            //break;
            //    case LinkStates.WaitTestConf:
            //        if (receiveItem.isValid && APDUValidator.TestConf(ref receiveItem.Apdu))
            //{
            //    receiveItem.isValid = false;
            //    LinkState = LinkStates.EndInit;
            //}
            //if (LinkState == LinkStates.WaitTestConf)
            //{
            //    if ((DateTime.UtcNow - dtLastIncomingAPDU).TotalMilliseconds > Timeout + Station.NoActivityTimeout)
            //    {
            //        LinkState = LinkStates.BEGIN;
            //        Error = (IEC60870_5_104ErrorCodes)DriverErrorCodes.ErrorTimeOut;
            //        fatalError = true;
            //    }
            //}
            //break;
        }

        #endregion

        #region Connection's check timer management
        private Timer _TmrCheckConnectionCyclically = null;
        private ManualResetEvent _TmrCheckConnectionCyclicallyFinished = null;
        private void StartTmrCheckDeviceCyclically()
        {
            lock (lockThreadObject)
            {
                if (_TmrCheckConnectionCyclically == null)
                    _TmrCheckConnectionCyclically = new Timer(CheckDeviceCyclically, this, Timeout, Timeout);
            }
        }

        private void StopTmrCheckDeviceCyclically()
        {
            WaitHandle waitHandle = null;
            lock (lockThreadObject)
            {
                if (_TmrCheckConnectionCyclically != null)
                {
                    waitHandle = new AutoResetEvent(false);
                    _TmrCheckConnectionCyclically.Dispose(waitHandle);
                    _TmrCheckConnectionCyclically = null;
                }
            }

            if (waitHandle != null)
            {
                waitHandle.WaitOne();
                waitHandle.Dispose();
                waitHandle = null;
            }

            lock (lockThreadObject)
            {
                if (currentThread != Thread.CurrentThread)
                {
                    // waiting unitl timer execution was completed
                    waitHandle = _TmrCheckConnectionCyclicallyFinished;
                }
            }

            if (waitHandle != null)
                waitHandle.WaitOne();
        }

        private void ReStartTmrCheckDeviceCyclically()
        {
            StopTmrCheckDeviceCyclically();
            StartTmrCheckDeviceCyclically();
        }

        Thread currentThread;
        private void CheckDeviceCyclically(Object state)
        {
            bool disconnect = false;
            try
            {
                bool bExecute = false;
                lock (lockThreadObject)
                {
                    currentThread = System.Threading.Thread.CurrentThread;
                    // destroy timet object
                    if (_TmrCheckConnectionCyclically != null)
                    {
                        bExecute = true;
                        _TmrCheckConnectionCyclically.Dispose();
                        _TmrCheckConnectionCyclically = null;
                    }

                    // timer is running
                    if (_TmrCheckConnectionCyclicallyFinished == null)
                        _TmrCheckConnectionCyclicallyFinished = new ManualResetEvent(false);
                    else
                        _TmrCheckConnectionCyclicallyFinished.Reset();
                }

                if (!bDisposed && !bSuspended && bExecute)
                {
                    try
                    {
                        StopTmrCheckDeviceCyclically();

                        CheckStationNoActivity();

                        //disconnect = (!Connected(inactivityTimeout));
                        //if (disconnect)
                        //{
                        //    // send event to TcpServer so it can dispose channel
                        //    EventHandler<EventArgs> temp = Terminated;
                        //    if (temp != null)
                        //        temp(this, new EventArgs());
                        //}
                    }
                    finally
                    {
                        StartTmrCheckDeviceCyclically();
                    }
                }
            }
            finally
            {
                if (!disconnect)
                {
                    lock (lockThreadObject)
                    {
                        if (_TmrCheckConnectionCyclicallyFinished != null)
                        {
                            // timer execution complete
                            _TmrCheckConnectionCyclicallyFinished.Set();
                        }
                        currentThread = null;
                    }
                }
            }
        }

        protected override void StartTimers()
        {
            base.StartTimers();
            StartTmrCheckDeviceCyclically();
        }

        protected override void StopTimers(bool bTerminate = true)
        {
            base.StopTimers(bTerminate);
            StopTmrCheckDeviceCyclically();
        }
        #endregion
    }
}
