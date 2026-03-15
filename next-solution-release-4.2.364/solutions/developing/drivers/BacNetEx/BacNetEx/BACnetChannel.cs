////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	BACnetChannel.cs
//
// summary:	Implements the driver BACnet channel class
////////////////////////////////////////////////////////////////////////////////////////////////////
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using System.IO;
using System.Threading.Tasks;
using System.Threading;

namespace BACnet
{
    /// <summary>   Communication channel of the BACnet driver. </summary>
    public class BACnetChannel : BacknetUdpChannel
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the BACnetChannel object. </summary>
        ///
        /// <param name="commdriver">   . </param>
        /// <param name="settings">     . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetChannel(CommunicationDriver commdriver, BACnetChannelSettings settings)
            : base(commdriver, settings)
        {
            //EnableBroadcast = true;
            deviceInstance = settings.getDeviceInstance();
            deviceHostName = settings.UdpChannelSettingsLocalHostName;
            devicelHostPort = settings.UdpChannelSettingsLocalHostPort;
            //this.AllowOtherBACnetClients = (commdriver as BACnetDriver).AllowOtherBACnetClients;
            //commChannel = new BACNetCommunicationChannel(commdriver, settings);
            channelSettings = settings;
        }

        #endregion

        #region member

        int deviceInstance;
        string deviceHostName;
        int devicelHostPort;
        BACnetChannelSettings channelSettings;
        List<BACnetStation> listStation;
        Dictionary<string, BACnetBBMDDevice> bbmdDevices = null;
        byte invokeId = 0;
        BACnetDeviceServer deviceServer = null;
        //map of device's ip managed by channel --> use map for speed reason
        public Dictionary<string, BACnetStation> AllowedIp = new Dictionary<string, BACnetStation>();
        private object lockCov = new object();
        // relation from BACnet object to CovSubscribeID
        private Dictionary<string, uint> mapCovSubscribeID = new Dictionary<string, uint>();
        // relation from jobs related to same BACnet object and CovSubscribeID
        Dictionary<UInt32, List<BACnetCommJob>> mapCovSubscribeIDJob = new Dictionary<uint, List<BACnetCommJob>>();
        // communication channel used to recive answer from device and consume cov notification
        //BACnetCommunicationChannel commChannel;
        private ReceiveItem receiveItem = null;
        private bool anyJobActive = false;
        private BACnetCommJob activeJob = null;
        private object currentJobLock = new object();

        private Timer _TmrCheckConnectionCyclically = null;
        private ManualResetEvent _TmrCheckConnectionCyclicallyFinished = null;
        private bool checkConnectionCyclically = false;

        #endregion

        #region Override Methods        

        private bool AnyActiveJob()
        {
            return anyJobActive;
        }

        private void SetActiveJob(BACnetCommJob job)
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

        private void ManageWriteCommand(ref BACnetCommJob job, out DriverErrorCodes result, out bool noDataToWrite)
        {
            result = DriverErrorCodes.ErrorNoError;
            noDataToWrite = false;
            job.InvokeID = getInvokeID();
            IPFrameFactory.WriteFormatResult WriteMessage = IPFrameFactory.confirmedWriteProperty(out BACnetIPFrame frame, ref job, out string errorOnWriteMessage);
            switch (WriteMessage)
            {
                case IPFrameFactory.WriteFormatResult.OK:
                    if (!DeviceWrite(frame, ((BACnetStation)job.Station).RemoteEndPoint, job))
                    {
                        result = (DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                    }
                    break;
                // ERR_NO_DATA_TO_WRITE = data to write not changed
                case IPFrameFactory.WriteFormatResult.ERR_NO_DATA_TO_WRITE:
                    noDataToWrite = true;
                    break;
                default:
                    // unmanaged error
                    result = (DriverErrorCodes)BACnetErrorCodes.ErrorInvalidDataType;
                    break;
            }
        }

        public override DriverErrorCodes CheckDevice(CommJob exjob, object thischannel)
        {
            DriverErrorCodes result = base.CheckDevice(exjob, thischannel);
            // wait some time after connection failed (for UDP driver, fail connection [local binding] is immedidate [0 msec]); to avoid scheduling new job without immediatly, generate 
            // "wait time"
            if (result != DriverErrorCodes.ErrorNoError)
                System.Threading.Thread.Sleep(Properties.Settings.Default.ConnectionFailedWaitTime);

            return result;
        }

        public override bool ExecuteJob(ref DriverErrorCodes conn, CommJob exjob)
        {
            // manage connection error into ProcessNewData
            if (conn != DriverErrorCodes.ErrorNoError)
                return false;

            BACnetCommJob mJob = exjob as BACnetCommJob;

            base.ExecuteJob(exjob);            

            // invalid or null station HostName
            if (((BACnetStation)mJob.Station).RemoteEndPoint == null)
            {
                ((BACnetStation)exjob.Station).PutAllJobsInError();

                OnJobExecuted(new BACnetExecutedJobArgs() { Job = mJob, ErrorCode = (DriverErrorCodes)BACnetErrorCodes.ErrorInvalidStation, Values = null });

                return false;
            }
            else
            {
                if (deviceServer != null)
                    DeviceServerWhoIsIam();

                mJob.CheckBacNetState();

                if (mJob.IsBacnetJobState())
                    mJob.GetReadWriteState();

                switch (mJob.BacnetState)
                {
                    case BACnetCommJob.BACnetState.BBMDInitialization:
                        ((BACnetStation)mJob.Station).BBDMDevice.CheckBacNetState();
                        switch (((BACnetStation)mJob.Station).BBDMDevice.BacnetState) {
                            //case BACnetState.BBMDWhoIs:
                            //    break;
                            case BACnetBBMDDevice.BACnetState.RegisterAsForeignDevice:
                            case BACnetBBMDDevice.BACnetState.RefreshRegistration:
                                if (!DeviceWrite(IPFrameFactory.RegisterAsForeignDevice(this, (ushort)((BACnetStation)mJob.Station).BBMDLifetime), ((BACnetStation)mJob.Station).BBMDEndPoint, mJob))
                                {
                                    conn = (DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                                    // manage error into ProcessNewData() and put station in error
                                    return false;
                                }
                                break;
                            //case BACnetBBMDDevice.BACnetState.ReadDFTTable:
                            //    if (!DeviceWrite(IPFrameFactory.ReadForeignDeviceTable(this), ((BACnetStation)mJob.Station).BBMDEndPoint, mJob))
                            //    {
                            //        conn = (DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                            //        // manage error into ProcessNewData() and put station in error
                            //        return false;
                            //    }
                            //    break;
                            //case BACnetBBMDDevice.BACnetState.ReadBDTTable:
                            //    if (!DeviceWrite(IPFrameFactory.ReadBBMDDeviceTable(this), ((BACnetStation)mJob.Station).BBMDEndPoint, mJob))
                            //    {
                            //        conn = (DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                            //        // manage error into ProcessNewData() and put station in error
                            //        return false;
                            //    }
                            //    break;
                        }
                        break;

                    case BACnetCommJob.BACnetState.WhoIs:
                        TryToAddGoodStationToAllowedIP((BACnetStation)mJob.Station);
                        if (!DeviceWrite(IPFrameFactory.WhoIs((BACnetStation)mJob.Station), ((BACnetStation)mJob.Station).RemoteEndPoint, mJob))
                        {
                            conn = (DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                            // manage error into ProcessNewData() and put station in error
                            return false;
                        }
                        break;

                    case BACnetCommJob.BACnetState.WhoHas:
                        if (!DeviceWrite(IPFrameFactory.WhoHas(mJob.ObjectName, mJob.Station as BACnetStation), (mJob.Station as BACnetStation).RemoteEndPoint, mJob))
                        {
                            conn = (DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                            // manage error into ProcessNewData()
                            return false;
                        }
                        break;

                    case BACnetCommJob.BACnetState.Cov:
                        if (!mJob.HasSubscriberID())
                        {
                            // search if another job have subscribed the same object
                            BACnetCommJob jobCov = GetJobWithSameCovParameters(mJob);
                            if (jobCov != null)
                            {
                                CopyTagValueFromAnotherJob(jobCov, mJob);

                                SetLastCOVExecutionTime(mJob, DriverErrorCodes.ErrorNoError);

                                OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = DriverErrorCodes.ErrorNoError, Job = mJob, IsCovDataFromDifferentJob = true });

                                mJob.CheckBacNetState();

                                return false;
                            }
                        }

                        AssignCovSubscriberID(ref mJob);
                        mJob.InvokeID = getInvokeID();
                        //System.Diagnostics.Debug.WriteLine(string.Format("Subscribe COV {0}", mJob.ObjectFullName));
                        if (!DeviceWrite(IPFrameFactory.confirmedSubscribeCov(mJob.InvokeID, mJob.CovSubscriberID, mJob.ObjectID, (mJob.Station as BACnetStation)), (mJob.Station as BACnetStation).RemoteEndPoint, mJob))
                        {
                            conn = (DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                            // manage error into ProcessNewData()
                            return false;
                        }
                        break;

                    case BACnetCommJob.BACnetState.CovSubscribedPolling:
                        mJob.InvokeID = getInvokeID();
                        if (!DeviceWrite(IPFrameFactory.confirmedReadProperty(mJob), (exjob.Station as BACnetStation).RemoteEndPoint, mJob))
                        {
                            conn = (DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                            // manage error into ProcessNewData()
                            return false;
                        }
                        break;

                    case BACnetCommJob.BACnetState.CovSubscribed:
                        switch (mJob.CommandType)
                        {
                            case BACnetCommJob.CommandTypes.ReadCmd:
                                // used to manage conditional variable and read value after write operation
                                OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = (mJob.InErrorState ? (DriverErrorCodes)BACnetErrorCodes.ErrorProtocolError : DriverErrorCodes.ErrorNoError), Job = mJob });
                                return false;
                                break;

                            case BACnetCommJob.CommandTypes.WriteCmd:
                                ManageWriteCommand(ref mJob, out DriverErrorCodes result, out bool noDataToWrite);
                                if (result != DriverErrorCodes.ErrorNoError)
                                {
                                    // manage error into ProcessNewData()
                                    conn = result;
                                    return false;
                                }
                                if (noDataToWrite)
                                {
                                    RemovePendingJob(mJob);                                    
                                    return false;
                                }
                                break;
                        }
                        break;

                    case BACnetCommJob.BACnetState.Polling:
                        switch (mJob.CommandType)
                        {
                            case BACnetCommJob.CommandTypes.ReadCmd:
                                mJob.InvokeID = getInvokeID();
                                if (!DeviceWrite(IPFrameFactory.confirmedReadProperty(mJob), (exjob.Station as BACnetStation).RemoteEndPoint, mJob))
                                {
                                    conn = (DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
                                    // manage error into ProcessNewData()
                                    return false;
                                }
                                break;
                            case BACnetCommJob.CommandTypes.WriteCmd:
                                ManageWriteCommand(ref mJob, out DriverErrorCodes result, out bool noDataToWrite);
                                if (result != DriverErrorCodes.ErrorNoError)
                                {
                                    // manage error into ProcessNewData()
                                    conn = result;
                                    return false;
                                }
                                if (noDataToWrite)
                                {
                                    RemovePendingJob(mJob);
                                    return false;
                                }
                                break;
                        }
                        break;

                    case BACnetCommJob.BACnetState.UnknownObject:
                        OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = (DriverErrorCodes)BACnetErrorCodes.ErrorUnknownObject, Job = mJob });

                        return false;
                        break;
                }
            }

            return true;
        }

        public override void WaitNewDataEvent(ref DriverErrorCodes conn)
        {
            base.WaitNewDataEvent(ref conn);
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
                    PutAllStationsAndJobsInError(listStation);

                    OnJobExecuted(new BACnetExecutedJobArgs { ErrorCode = conn, Job = exjob, Values = null });
                    return true;
                case DriverErrorCodes.ErrorTimeOut: // manage at station level, don't close/reopen channel
                    error = DriverErrorCodes.ErrorTimeOut;
                    // a message (that doesn't match driver request) come from device --> usefull in case I want to distinguish a "full" timeout from UnconfirmedMessage
                    if (DateTime.UtcNow.Subtract(((BACnetStation)exjob.Station).LastMessageFromDevice).TotalMilliseconds < Timeout)
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
                        // check error containt into answer message
                        if (IPFrameValidator.isErrorFrame(receiveItem.Frame))
                        {
                            error = (DriverErrorCodes)IPFrameValidator.GetError(receiveItem.Frame);
                        }
                        else
                        {
                            if (IPFrameValidator.isRejectFrame(receiveItem.Frame))
                            {
                                error = (DriverErrorCodes)BACnetErrorCodes.ErrorInvalidDataType;
                            }
                            else
                            {
                                if (conn != DriverErrorCodes.ErrorNoError)
                                    error = conn;
                            }
                        }
                    }
                    break;
            }
            #endregion

            BACnetCommJob mJob = exjob as BACnetCommJob;
            switch (mJob.BacnetState)
            {
                //case BACnetState.BBMDWhoIs:
                //    break;
                case BACnetCommJob.BACnetState.BBMDInitialization:
                    switch (((BACnetStation)mJob.Station).BBDMDevice.BacnetState)
                    {
                        case BACnetBBMDDevice.BACnetState.RegisterAsForeignDevice:
                            if (error != DriverErrorCodes.ErrorNoError)
                            {
                                PutAllStationsAndJobsInError(((BACnetStation)mJob.Station).BBDMDevice.RelatedStations);
                                OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                            }
                            else
                            {
                                //((BACnetStation)mJob.Station).BBDMDevice.BacnetState = BACnetBBMDDevice.BACnetState.ReadDFTTable;
                                ((BACnetStation)mJob.Station).BBDMDevice.SetRegistred();
                                // keep job pending
                                return false;
                            }
                            break;

                        //case BACnetBBMDDevice.BACnetState.ReadDFTTable:
                        //    if (error != DriverErrorCodes.ErrorNoError)
                        //    {
                        //        PutAllStationsAndJobsInError(((BACnetStation)mJob.Station).BBDMDevice.RelatedStations);
                        //        OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                        //    }
                        //    else// if (IPFrameValidator.IAm(receiveItem.Frame, out IAmAnswer iAmAnswer))
                        //    {
                        //        ((BACnetStation)mJob.Station).BBDMDevice.BacnetState = BACnetBBMDDevice.BACnetState.ReadBDTTable;
                        //        // keep job pending
                        //        return false;
                        //    }
                        //    break;

                        //case BACnetBBMDDevice.BACnetState.ReadBDTTable:
                        //    if (error != DriverErrorCodes.ErrorNoError)
                        //    {
                        //        PutAllStationsAndJobsInError(((BACnetStation)mJob.Station).BBDMDevice.RelatedStations);
                        //        OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                        //    }
                        //    else// if (IPFrameValidator.IAm(receiveItem.Frame, out IAmAnswer iAmAnswer))
                        //    {
                        //        ((BACnetStation)mJob.Station).BBDMDevice.SetRegistred();
                        //        // keep job pending
                        //        return false;
                        //    }
                        //    break;

                        case BACnetBBMDDevice.BACnetState.RefreshRegistration:
                            if (error != DriverErrorCodes.ErrorNoError)
                            {
                                PutAllStationsAndJobsInError(((BACnetStation)mJob.Station).BBDMDevice.RelatedStations);
                                OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                            }
                            else
                            {
                                ((BACnetStation)mJob.Station).BBDMDevice.SetRegistred();
                                // keep job pending
                                return false;
                            }
                            break;
                    }
                    break;

                case BACnetCommJob.BACnetState.WhoIs:
                    if (error != DriverErrorCodes.ErrorNoError)
                    {
                        ((BACnetStation)mJob.Station).PutAllJobsInError();

                        OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });

                    } else if (IPFrameValidator.IAm(receiveItem.Frame, out IAmAnswer iAmAnswer)) {
                        if (!((BACnetStation)mJob.Station).HasDeviceInstance())
                            ((BACnetStation)mJob.Station).RunTimeDeviceInstance = (int)iAmAnswer.DeviceIdentifier.instance;

                        InitStationWithWhoIsData((BACnetStation)mJob.Station, iAmAnswer.DeviceIdentifier, iAmAnswer.MaxAPDULength, iAmAnswer.VendorIdentifier, iAmAnswer.DestinationSpecifierPresent, iAmAnswer.DNET, iAmAnswer.DLEN, iAmAnswer.DADR);

                        mJob.LastExecutionTime = DateTime.UtcNow;
                        // keep job pending
                        return false;
                    }
                    break;

                case BACnetCommJob.BACnetState.WhoHas:
                    if (error != DriverErrorCodes.ErrorNoError)
                    {
                        // WhoAs answer in 
                        if (error == DriverErrorCodes.ErrorTimeOut)
                            error = (DriverErrorCodes)BACnetErrorCodes.ErrorUnknownProperty;

                        if (error == (DriverErrorCodes)BACnetErrorCodes.ErrorUnknownProperty)
                            mJob.BacnetState = BACnetCommJob.BACnetState.UnknownObject;

                        OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    } else if (IPFrameValidator.IHave(receiveItem.Frame, (mJob.Station as BACnetStation).DeviceIdentifier, mJob.ObjectName, out BACnetObjectIdentifier objectID)) {
                        if (!mJob.HasRunTimeInstanceNumber())
                            mJob.RunTimeInstanceNumber = (int)objectID.instance;

                        mJob.SetObjectID();

                        // keep job pending
                        return false;
                    }
                    break;

                case BACnetCommJob.BACnetState.Cov:
                    if (error == DriverErrorCodes.ErrorNoError)
                    {
                        if (IPFrameValidator.isCovSubscriptionFailed(receiveItem.Frame, mJob.InvokeID))
                        {
                            mJob.BacnetState = BACnetCommJob.BACnetState.UnknownObject;
                            error = (DriverErrorCodes)BACnetErrorCodes.ErrorCovSubscriptionFailed;
                        }
                    }

                    if (error != DriverErrorCodes.ErrorNoError)
                    {
                        error = (DriverErrorCodes)BACnetErrorCodes.ErrorCovSubscriptionFailed;
                        CovAssignUnSubscriberID(mJob, error);
                        OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    }
                    else
                    {
                        if (IPFrameValidator.isCovSubscriptionOk(receiveItem.Frame, mJob.InvokeID))
                        {
                            SetLastCOVExecutionTime(mJob, (DriverErrorCodes)error);

                            // change state from BACnetState.Cov to BACnetState.CovSubscribed
                            mJob.CheckBacNetState();

                            // keep job alive to get initial data (if necessary) by polling request
                            if (mJob.BacnetState == BACnetCommJob.BACnetState.CovSubscribedPolling)
                                return false;
                            else // cov subscribed
                                OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                        }
                    }

                    break;

                case BACnetCommJob.BACnetState.CovSubscribedPolling:
                    if (error != DriverErrorCodes.ErrorNoError)
                    {
                        OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    }
                    else
                    {
                        if (IPFrameValidator.ConfirmedReadProperty(receiveItem.Frame, mJob.InvokeID, mJob.ObjectID, mJob.PropertyIdentifier, mJob.ArrayIndex, mJob, out byte[] answer, out error))
                        {
                            SetLastCOVExecutionTime(mJob, (DriverErrorCodes)error);

                            // change state from BACnetState.Cov to BACnetState.CovSubscribed
                            mJob.CheckBacNetState();

                            OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = true });
                        }
                        else
                        {
                            OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob });
                        }
                        break;
                    }
                    break;

                case BACnetCommJob.BACnetState.CovSubscribed:
                    switch (mJob.CommandType)
                    {
                        case BACnetCommJob.CommandTypes.ReadCmd:
                            // managed into ExecuteJob()
                            break;
                        case BACnetCommJob.CommandTypes.WriteCmd:
                            bool relinquishCmd = mJob.IsRelinquishRequest();
                            OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                            if (error == DriverErrorCodes.ErrorNoError)
                            {
                                // keep job pending in case of input/output to allow driver to read "current" device value
                                if (relinquishCmd)
                                {
                                    if (mJob.Type == LinkType.InputOutput)
                                        return false;
                                }
                            }
                            break;
                    }
                    break;

                case BACnetCommJob.BACnetState.Polling:
                    if (error != DriverErrorCodes.ErrorNoError) {
                        switch (error)
                        {
                            case DriverErrorCodes.ErrorTimeOut:
                                ((BACnetStation)mJob.Station).PutAllJobsInError();
                                break;
                            case (DriverErrorCodes)BACnetErrorCodes.ErrorUnknownObject:
                                mJob.BacnetState = BACnetCommJob.BACnetState.UnknownObject;
                                break;
                        }
                        OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                    }
                    else
                    {
                        switch (mJob.CommandType)
                        {
                            case BACnetCommJob.CommandTypes.ReadCmd:
                                if (IPFrameValidator.ConfirmedReadProperty(receiveItem.Frame, mJob.InvokeID, mJob.ObjectID, mJob.PropertyIdentifier, mJob.ArrayIndex, mJob, out byte[] answer, out error))
                                    OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = answer, IsRead = true });
                                else
                                    OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob});
                                break;
                            case BACnetCommJob.CommandTypes.WriteCmd:
                                bool relinquishCmd = mJob.IsRelinquishRequest();
                                OnJobExecuted(new BACnetExecutedJobArgs() { ErrorCode = error, Job = mJob, Values = null });
                                // keep job pending in case of input/output to allow driver to read "current" device value
                                if (relinquishCmd)
                                {
                                    if (mJob.Type == LinkType.InputOutput)
                                        return false;
                                }
                                break;
                        }
                    }
                    break;
                case BACnetCommJob.BACnetState.UnknownObject:
                    // do nothing here
                    break;
            }

            if (error == DriverErrorCodes.ErrorNoError)
            {
                // initialization sequence terminated; some writing operation pending ?
                if (mJob.HasPendingWriteCmd())
                    return false;
            }

            return true;
        }

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            // from error timeout, reset internal BACnet identifiert --> next ExecuteJob() reinit BACnet object
            if (e.ErrorCode == DriverErrorCodes.ErrorTimeOut)
                ((BACnetStation)e.Job.Station).PutJobInError((BACnetCommJob)e.Job);

            base.OnJobExecuted(e);
        }

        public bool DeviceWrite(BACnetIPFrame Frame, IPEndPoint RemoteEndPoint)
        {
            byte[] buffer = Frame.Pack();
            return base.DeviceWrite(buffer, (uint)buffer.Length, RemoteEndPoint);

        }

        public bool DeviceWrite(BACnetIPFrame Frame, IPEndPoint RemoteEndPoint, BACnetCommJob job)
        {
            bool result = DeviceWrite(Frame, RemoteEndPoint);
            if (result)
                SetActiveJob(job);

            return result;
        }

        public override bool DeviceRead(byte[] Buffer, uint Count = 0)
        {
            if (Count == 0)
                return base.DeviceRead(Buffer, (uint)Buffer.Length);
            else
                return base.DeviceRead(Buffer, Count);
        }

        public override bool Startup()
        {
            if (!bChannelStarted)
            {
                Init();
                
                this.NewDataReceived += BACnetChannel_NewDataReceived;

                StartTmrCheckDeviceCyclically();

                //// create and open the listening channel                                
                //commChannel = new BACnetCommunicationChannel(this.CommDriver, channelSettings);
                //commChannel.SourceChannel = this;
                //commChannel.StartThread();
            }
            return base.Startup();
        }

        public override void Suspend()
        {
            this.NewDataReceived -= BACnetChannel_NewDataReceived;

            DeviceServerTerminate();

            BBMDTerminate();

            StopTmrCheckDeviceCyclically();

            //commChannel.SuspenThread();
            //commChannel.Dispose();
            //commChannel = null;

            base.Suspend();
        }

        #endregion

        #region Properties

        //private bool _BBMDRegister;
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>    Register to BBMD as foreign </summary>
        /////
        ///// <value> This option force the driver to register with a BBMD device as foreign, with the aim of 
        /////         being addressed with the broadcast messages originated in the BBMD BACnet network. 
        /////         This is compulsory if the device belong to a TCP/IP network different from that of the driver, 
        /////         connected through a router. Routers stops all the global broadcast and this prevent a 
        /////         correct information exchange. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public bool BBMDRegister
        //{
        //    get
        //    {
        //        return _BBMDRegister;
        //    }
        //    set
        //    {
        //        _BBMDRegister = value;
        //    }
        //}

        //private uint _BBMDLifetime;
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>    BBMD registration lifetime (min.)  </summary>
        /////
        ///// <value> Duration in minutes of the registration as foreign device.
        /////              durations less then 15 minutes are not allowed </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public uint BBMDLifetime
        //{
        //    get
        //    {
        //        return _BBMDLifetime;
        //    }
        //    set
        //    {
        //        _BBMDLifetime = value;
        //    }
        //}

        //private string _BBMDAddress;
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>   BBMD IP address. </summary>
        /////
        ///// <value> IP address of the BBMD device. If it is left void, the main BACnet device IP is used. 
        /////         </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public string BBMDAddress
        //{
        //    get
        //    {
        //        return _BBMDAddress;
        //    }
        //    set
        //    {
        //        _BBMDAddress = value;
        //    }
        //}

        #endregion

        #region methods
        private byte getInvokeID()
        {
            return invokeId++;
        }

        public void ConsumeCovNotification(ref ReceiveItem receiveItem)
        {
            if (IPFrameValidator.UnconfirmedCovNotification(receiveItem.Frame, out UInt32 subscriberID, out BACnetObjectIdentifier deviceID, out BACnetObjectIdentifier objectID, out UInt32 timeRemaining, out ServiceTagList tagList))
            {
                List<BACnetCommJob> servedJoblist = null;
                lock (lockCov)
                {
                    mapCovSubscribeIDJob.TryGetValue(subscriberID, out List<BACnetCommJob> tempList);
                    if (tempList != null && tempList.Count > 0)
                        servedJoblist = new List<BACnetCommJob>(tempList);
                }

                if (servedJoblist != null)
                {
                    // manage cov data only if job is in cov state
                    foreach (BACnetCommJob job in servedJoblist)
                    {
                        if (receiveItem.ComingFromEndPoint(((BACnetStation)job.Station).RemoteEndPoint) && ((BACnetStation)job.Station).DeviceIdentifier.valueUint == deviceID.valueUint && job.ObjectID.valueUint == objectID.valueUint)
                        {
                            if (job.IsBacnetCovState())
                            {
                                byte[] answer = null;
                                if (job.PropertyIdentifier == (BACnetEnums.PropertyIdentifier)tagList.List[0].UInt)
                                    answer = tagList.List[1].TagList.List[0].Values;
                                else if (job.PropertyIdentifier == (BACnetEnums.PropertyIdentifier)tagList.List[2].UInt)
                                    answer = tagList.List[3].TagList.List[0].Values;

                                if (answer != null)
                                {
                                    OnJobExecuted(new BACnetExecutedJobArgs
                                    {
                                        ErrorCode = DriverErrorCodes.ErrorNoError,
                                        Job = job,
                                        Values = answer,
                                        IsRead = true
                                    });
                                }
                            }
                        }
                    }
                }
            }
        }

        public void InitStationWithNoWhoIsData(BACnetStation station)
        {
            InitStationWithWhoIsData(station, new BACnetObjectIdentifier(BACnetEnums.ObjectTypes.DEVICE, (uint)station.GetDeviceInstance()), 0, 0, false, 0, 0, new byte[0]);
        }

        private void InitStationWithWhoIsData(BACnetStation station, BACnetObjectIdentifier deviceIdentifier, UInt32 maxAPDULength, UInt16 vendorIdentifier, bool destinationSpecifierPresent, UInt16 dnet, byte dlen, byte[] dadr)
        {
            System.Diagnostics.Debug.WriteLine(string.Format("-------------------------BACnet {0},Station:{1},Message WhoIs Reviced fr", DateTime.Now, station.RemoteEndPoint));

            //Report Who-IS success to Movicon for diagnostic 
            CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.ErrorWhoHisSucess, station.Name), EventSeverity.Low);

            //Setup station, subscribe job, do WhoHas
            station.LastErrorCode = DriverErrorCodes.ErrorNoError;
            station.InErrorState = false;
            station.BACnetInitDone = true;
            station.DeviceIdentifier = deviceIdentifier;
            station.MaxAPDULength = maxAPDULength;
            station.SegmentationSupported = 0;
            station.VendorIdentifier = vendorIdentifier;
            station.DestinationSpecifierPresent = destinationSpecifierPresent;
            station.DNET = dnet;
            station.DLEN = dlen;
            station.DADR = dadr;

            if (station.TimeSync != TimeSyncs.None)
            {
                BACnetIPFrame tymeSync = station.TimeSync == TimeSyncs.System_Time ? IPFrameFactory.TimeSynch(station) : IPFrameFactory.UtcTimeSynch(station);
                DeviceWrite(tymeSync, station.RemoteEndPoint);
                //StopWorkerThread.WaitOne(20);
            }
        }

        private string filebaseSubscriberID = string.Empty;
        private InMemoryDataStore inMemorySubscriberID = null;
        private IDataLayer idlSubscriberID = null;
        private UnitOfWork ufwSubscriberID = null;
        private void AssignCovSubscriberID(ref BACnetCommJob job)
        {
            lock (lockCov)
            {
                if (mapCovSubscribeIDJob.ContainsKey(job.CovSubscriberID))
                    return;

                if (idlSubscriberID == null)
                {
                    idlSubscriberID = GetPathToTheFileSaveData((BACnetDriver)CommDriver, Name, out filebaseSubscriberID, out inMemorySubscriberID);
                    if (idlSubscriberID == null)
                    {
                        return;
                    }
                    ufwSubscriberID = new UnitOfWork(idlSubscriberID);
                }

                if (mapCovSubscribeID.Count == 0)
                {
                    if (!string.IsNullOrWhiteSpace(filebaseSubscriberID))
                    {
                        if (File.Exists(filebaseSubscriberID) && (inMemorySubscriberID != null))
                        {
                            inMemorySubscriberID.ReadXml(filebaseSubscriberID);
                        }
                    }
                    //mapSubscribeID = (from tag in new XPQuery<BACnetSubscribeId>(ufwSubscriberID).AsParallel() select tag).ToDictionary(tag => tag.Name, tag => tag.SubcribeId  );
                    mapCovSubscribeID = (from tag in new XPQuery<BACnetSubscribeId>(ufwSubscriberID)/*.AsParallel()*/ select tag)
                                                                                                                    .GroupBy(p => p.Name)
                                                                                                                    .Select(grp => grp.FirstOrDefault())
                                                                                                                    .ToDictionary(tag => tag.Name, tag => tag.SubcribeId);
                }

                //string keyName = station.Name + "_" + job.ObjectName;                
                string keyName = job.ObjectFullName;
                if (mapCovSubscribeID.Count == 0 || !mapCovSubscribeID.ContainsKey(keyName))
                {
                    job.CovSubscriberID = BACnetProtocol.GetNewCovSubscriberID(ref mapCovSubscribeIDJob); // getSubscriberID();
                    var subdcribedId = new BACnetSubscribeId(ufwSubscriberID);
                    subdcribedId.Name = keyName;
                    subdcribedId.SubcribeId = job.CovSubscriberID;
                    ufwSubscriberID.CommitChanges();
                    if (!string.IsNullOrWhiteSpace(filebaseSubscriberID) && (inMemorySubscriberID != null))
                        inMemorySubscriberID.WriteXml(filebaseSubscriberID);

                    mapCovSubscribeID.Add(keyName, job.CovSubscriberID);
                }
                else if (mapCovSubscribeID.ContainsKey(keyName))
                {
                    job.CovSubscriberID = mapCovSubscribeID[keyName];
                }

                if (!mapCovSubscribeIDJob.ContainsKey(job.CovSubscriberID))
                    mapCovSubscribeIDJob.Add(job.CovSubscriberID, new List<BACnetCommJob>());
                mapCovSubscribeIDJob[job.CovSubscriberID].Add(job);
            }
        }

        public void CovAssignUnSubscriberID(BACnetCommJob job, DriverErrorCodes error)
        {
            // allign all job's timeout
            SetLastCOVExecutionTime(job, error);

            string keyName = job.ObjectFullName;
            lock (lockCov)
            {
                if (mapCovSubscribeID.ContainsKey(keyName))
                {
                    uint covSubscriberID = job.CovSubscriberID;
                    if (mapCovSubscribeIDJob.TryGetValue(covSubscriberID, out List<BACnetCommJob> servedJoblist))
                    {
                        // reset CovID
                        Parallel.ForEach(servedJoblist, jobCov =>
                        {
                            jobCov.CovSubscriberID = 0;
                        });
                        mapCovSubscribeIDJob.Remove(covSubscriberID);
                    }

                    mapCovSubscribeID.Remove(keyName);
                }
            }
        }

        private bool AnyCovSuScribedForStation(Station s)
        {
            lock (lockCov)
            {
                foreach (var element in mapCovSubscribeIDJob)
                {
                    if (element.Value.Count(j => j.Station == s) > 0)// && ((BACnetCommJob)j).COVEnable && ((BACnetCommJob)j).IsCovSubscribed()) > 0)
                        return true;
                }
            }

            return false;
        }

        private BACnetCommJob GetJobWithSameCovParameters(BACnetCommJob jobIn)
        {
            BACnetCommJob jobOut = null;

            string keyName = jobIn.ObjectFullName;
            lock (lockCov)
            {
                if (mapCovSubscribeID.ContainsKey(keyName))
                {
                    uint subscriberID = mapCovSubscribeID[keyName];
                    if (mapCovSubscribeIDJob.TryGetValue(subscriberID, out List<BACnetCommJob> servedJoblist))
                        if (servedJoblist != null && servedJoblist.Count > 0)
                            jobOut = servedJoblist[0];
                }
            }

            return jobOut;
        }

        private void CopyTagValueFromAnotherJob(BACnetCommJob jobSource, BACnetCommJob jobDest)
        {
            for (int i = 0; i < jobSource.TagsList.Count; i++)
            {
                if (jobDest.TagsList.Count >= i)
                {
                    jobDest.TagsList[i].SetReadValue(jobSource.TagsList[i].GetReadValue());
                    jobDest.TagsList[i].SetInternalValues(jobSource.TagsList[i].GetReadValue());
                    jobDest.TagsList[i].Value.Value = (jobSource.TagsList[i].GetReadValue());
                    jobDest.TagsList[i].LastValue = (jobSource.TagsList[i].GetReadValue());
                }
            }

            AssignCovSubscriberID(ref jobDest);
        }

        public void SetLastCOVExecutionTime(BACnetCommJob job, DriverErrorCodes error)
        {
            if (error == DriverErrorCodes.ErrorNoError)
            {
                // set when cov was subscribed
                job.SetCOVSubscriptionTime();

                // set when cov expire
                job.SetCOVExpireTime();

                // reschedule jobs after xxx sec, then, if no message from device, try WhoIs to check device presence  
                //job.SamplingInterval = job.GetNextCOVScheduletInterval();                
                job.LastExecutionTime = DateTime.UtcNow;
            }
            else
            {
                job.ResetCOVTimes();
                //job.SamplingInterval = 0;
                job.LastExecutionTime = DateTime.UtcNow;
            }

            // assign the same refresh cov "parameters" to similar jobs (cov that subscribed the same variable)
            List<BACnetCommJob> servedJoblist = null;
            lock (lockCov)
            {
                mapCovSubscribeIDJob.TryGetValue(job.CovSubscriberID, out servedJoblist);
            }
            if (servedJoblist != null && servedJoblist.Count > 1)
            {
                // manage cov data only if job is in cov state
                Parallel.ForEach(servedJoblist, jobCov =>
                {
                    jobCov.COVSubscriptionTime = job.COVSubscriptionTime;
                    jobCov.COVExpireTime = job.COVExpireTime;
                    //jobCov.SamplingInterval = job.SamplingInterval;
                    jobCov.LastExecutionTime = job.LastExecutionTime;
                });
            }
        }

        private IDataLayer GetPathToTheFileSaveData(BACnetDriver BACnetCommDriver, string name, out string filebase, out InMemoryDataStore InMemory)
        {
            string connect = CommunicationDriver.GetConnectionString(BACnetCommDriver.StrConnectionString, "Drivers", name, Properties.Settings.Default.ExtentionOfTheFileCov);
            IDataLayer dl = CommunicationDriver.GetSpecificDataLayer(connect, out filebase, out InMemory, out bool targetIsFile);
            return (dl);
        }

        public void CheckDevicesConnection(List<Station> list)
        {
            checkConnectionCyclically = true;            

            ResetNewDataEvent();

            foreach (var s in list)
            {
                TryToAddGoodStationToAllowedIP((BACnetStation)s);
                if (!DeviceWrite(IPFrameFactory.WhoIs((BACnetStation)s), ((BACnetStation)s).RemoteEndPoint))
                {
                    // do nothing here
                }
            }

            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            WaitNewDataEvent(ref conn);

            checkConnectionCyclically = false;

            foreach (var s in list)
            {
                if (((BACnetStation)s).IsTimeToExecuteWhoIs())
                    ((BACnetStation)s).PutAllJobsInError();
            }            
        }

        public DriverErrorCodes CheckDeviceConnectionWhoIs(Station s)
        {
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;

            checkConnectionCyclically = true;

            ResetNewDataEvent();

            TryToAddGoodStationToAllowedIP((BACnetStation)s);
            if (!DeviceWrite(IPFrameFactory.WhoIs((BACnetStation)s), ((BACnetStation)s).RemoteEndPoint))
            {
                conn = (DriverErrorCodes)BACnetErrorCodes.ErrorTxWrite;
            }
            else
            {
                WaitNewDataEvent(ref conn);

                checkConnectionCyclically = false;

                if (!((BACnetStation)s).IsTimeToExecuteWhoIs())
                    conn = DriverErrorCodes.ErrorDeviceOpenFailed;
            }

            return conn;
        }

        #endregion

        #region Events
        private void BACnetChannel_NewDataReceived(object sender, NewDataReceivedArgs e)
        {
            e.SetNewDataEvent = false;

            ReceiveItem receiveItem = null;
            lock (lockThreadObject)
            {
                BACnetProtocol.BACnetParseData(e.Sender, e.RxBytes, e.Timestamp, AllowedIp, out receiveItem);
            }

            // invalida data
            if (receiveItem == null || !receiveItem.isValid) //  && !IPFrameValidator.isErrorFrame(receiveItem.Frame)
            {
                return;
            }

            #region Device check connection
            if (checkConnectionCyclically)
            {
                if (IPFrameValidator.IAm(receiveItem.Frame, out IAmAnswer iAmAnswer))
                {                    
                    // a valid data recived from device automatically update LastMessageFromDevice; when all device/station has this property update, stop
                    if (stationsToCheck.Count(s => ((BACnetStation)s).IsTimeToExecuteWhoIs()) == 0)
                    {
                        e.SetNewDataEvent = true;
                        return;
                    }  
                }
            }
            #endregion

            lock (currentJobLock)
            {
                #region Cov Notification Management
                if (IPFrameValidator.isUnconfirmedCovNotification(receiveItem.Frame))
                {
                    // manage here unconfirmed cov notification
                    ConsumeCovNotification(ref receiveItem);
                    return;
                }
                #endregion                

                #region special case
                if (deviceServer != null)
                {
                    //Device Server --> answer to WhoIs message with IAM (driver answer like a BACNet device)
                    if (IPFrameValidator.isUnConfirmedWhoIsRequest(receiveItem))
                    {
                        // Is WhoIs direct to local PC station ?
                        if (!receiveItem.ComingFromEndPoint(deviceServer.LocalEndPoint))
                        {
                            // manage here unconfirmed cov notification
                            if (!deviceServer.SendiAm(receiveItem.RemoteEndPoint))
                                CommDriver.OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorSendiAm, Opc.Ua.EventSeverity.Low);
                            return;
                        }
                    }
                }
                #endregion

                // no active job (scheduled job)
                if (!AnyActiveJob())
                {
                    // discard data
                    return;
                }
                else
                {
                    if (!receiveItem.ComingFromEndPoint(((BACnetStation)activeJob.Station).RemoteEndPoint) && !receiveItem.ComingFromEndPoint(((BACnetStation)activeJob.Station).BBMDEndPoint))
                        return;

                    if (IPFrameValidator.isErrorFrame(receiveItem.Frame) || IPFrameValidator.isRejectFrame(receiveItem.Frame))
                    {
                        UnSetActiveJob(receiveItem);
                        e.SetNewDataEvent = true;
                        return;
                    }

                    switch (activeJob.BacnetState)
                    {
                        case BACnetCommJob.BACnetState.None:
                            // discard data
                            break;

                        //case BACnetState.BBMDWhoIs:
                        //    break;
                        case BACnetCommJob.BACnetState.BBMDInitialization:
                            switch (((BACnetStation)activeJob.Station).BBDMDevice.BacnetState)
                            {

                                case BACnetBBMDDevice.BACnetState.RegisterAsForeignDevice:
                                case BACnetBBMDDevice.BACnetState.RefreshRegistration:
                                    if (IPFrameValidator.isconfirmedForeignDeviceRegistration(receiveItem))
                                    {
                                        UnSetActiveJob(receiveItem);
                                        e.SetNewDataEvent = true;
                                    }
                                    break;
                                //case BACnetBBMDDevice.BACnetState.ReadDFTTable:
                                //    if (IPFrameValidator.isConfirmedReadDFTTableDeviceRegistration(receiveItem))
                                //    {
                                //        UnSetActiveJob(receiveItem);
                                //        e.SetNewDataEvent = true;
                                //    }
                                //    break;
                                //case BACnetBBMDDevice.BACnetState.ReadBDTTable:
                                //    if (IPFrameValidator.isconfirmedReadBDTTableDeviceRegistration(receiveItem))
                                //    {
                                //        UnSetActiveJob(receiveItem);
                                //        e.SetNewDataEvent = true;
                                //    }
                                //    break;
                            }
                            break;

                        case BACnetCommJob.BACnetState.WhoIs:
                            {
                                if (IPFrameValidator.IAm(receiveItem.Frame, out IAmAnswer iAmAnswer))
                                {
                                    // sender match to job's station ?
                                    if ((((BACnetStation)activeJob.Station).GetDeviceInstance() == iAmAnswer.DeviceIdentifier.instance || ((BACnetStation)activeJob.Station).GetDeviceInstance() < 0) && receiveItem.ComingFromEndPoint(((BACnetStation)activeJob.Station).RemoteEndPoint))
                                    {
                                        UnSetActiveJob(receiveItem);
                                        e.SetNewDataEvent = true;
                                    }
                                }
                            }
                            break;
                        case BACnetCommJob.BACnetState.WhoHas:
                            if (IPFrameValidator.IHave(receiveItem.Frame, (activeJob.Station as BACnetStation).DeviceIdentifier, activeJob.ObjectName, out BACnetObjectIdentifier objectID))
                            {
                                UnSetActiveJob(receiveItem);
                                e.SetNewDataEvent = true;
                            }
                            break;

                        case BACnetCommJob.BACnetState.Cov:
                            if (IPFrameValidator.isCovSubscriptionOk(receiveItem.Frame, activeJob.InvokeID))
                            {
                                UnSetActiveJob(receiveItem);
                                e.SetNewDataEvent = true;
                            }
                            else if (IPFrameValidator.isCovSubscriptionFailed(receiveItem.Frame, activeJob.InvokeID))
                            {
                                UnSetActiveJob(receiveItem);
                                e.SetNewDataEvent = true;
                            }
                            break;

                        case BACnetCommJob.BACnetState.CovSubscribedPolling:
                            if (IPFrameValidator.isForThisReadJob(receiveItem.Frame, activeJob.InvokeID))
                            {
                                if (IPFrameValidator.isConfirmedReadProperty(receiveItem.Frame, activeJob.InvokeID, activeJob.ObjectID, activeJob.PropertyIdentifier, activeJob.ArrayIndex))
                                {
                                    UnSetActiveJob(receiveItem);
                                    e.SetNewDataEvent = true;
                                }
                            }
                            break;

                        case BACnetCommJob.BACnetState.CovSubscribed:
                            switch (activeJob.CommandType)
                            {
                                case BACnetCommJob.CommandTypes.ReadCmd:
                                    {
                                        if (IPFrameValidator.IAm(receiveItem.Frame, out IAmAnswer iAmAnswer))
                                        {
                                            // sender match to job's station ?
                                            if ((((BACnetStation)activeJob.Station).GetDeviceInstance() == iAmAnswer.DeviceIdentifier.instance || ((BACnetStation)activeJob.Station).GetDeviceInstance() < 0) && receiveItem.ComingFromEndPoint(((BACnetStation)activeJob.Station).RemoteEndPoint))
                                            {
                                                UnSetActiveJob(receiveItem);
                                                e.SetNewDataEvent = true;
                                            }
                                        }
                                    }
                                    break;
                                case BACnetCommJob.CommandTypes.WriteCmd:
                                    if (IPFrameValidator.isForThisWriteJob(receiveItem.Frame, activeJob.InvokeID))
                                    {
                                        UnSetActiveJob(receiveItem);
                                        e.SetNewDataEvent = true;
                                    }
                                    break;
                            }
                            break;

                        //case BACnetState.InitCompleted:
                        //    break;

                        case BACnetCommJob.BACnetState.Polling:
                            if (receiveItem.ComingFromEndPoint(((BACnetStation)activeJob.Station).RemoteEndPoint))
                            {
                                switch (activeJob.CommandType)
                                {
                                    case BACnetCommJob.CommandTypes.ReadCmd:
                                        if (IPFrameValidator.isForThisReadJob(receiveItem.Frame, activeJob.InvokeID))
                                        {
                                            if (IPFrameValidator.isConfirmedReadProperty(receiveItem.Frame, activeJob.InvokeID, activeJob.ObjectID, activeJob.PropertyIdentifier, activeJob.ArrayIndex))
                                            {
                                                UnSetActiveJob(receiveItem);
                                                e.SetNewDataEvent = true;
                                            }
                                        }
                                        break;
                                    case BACnetCommJob.CommandTypes.WriteCmd:
                                        if (IPFrameValidator.isForThisWriteJob(receiveItem.Frame, activeJob.InvokeID))
                                        {
                                            UnSetActiveJob(receiveItem);
                                            e.SetNewDataEvent = true;
                                        }
                                        break;
                                }
                            }
                            break;

                        case BACnetCommJob.BACnetState.UnknownObject:
                            // do nothing here
                            break;
                    }
                }
            }
        }

        private void PutAllStationsAndJobsInError(List<BACnetStation> list)
        {
            foreach (var s in list)
            {
                if (((BACnetStation)s).HasBBMDDevice())
                    ((BACnetStation)s).BBDMDevice.SetUnRegistred();
                ((BACnetStation)s).PutAllJobsInError();
            }
        }
        #endregion

        #region Server for WhoIs/i-Am

        private void DeviceServerInit()
        {
            if (deviceInstance != -1)
            {
                deviceServer = new BACnetDeviceServer(this, deviceInstance, deviceHostName, devicelHostPort);
                deviceServer.Init();
            }
        }

        private void DeviceServerTerminate()
        {
            if (deviceInstance != -1)
                deviceServer = null;
        }

        private void DeviceServerWhoIsIam()//ref ReceiveItem receiveItem)
        {
            // only on startup send iAm message to informs others devices about our presence
            if (deviceServer.FirstCycle)
            {
                deviceServer.FirstCycle = false;
                if (!deviceServer.SendiAm())
                    CommDriver.OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorSendiAmOnStartUp, Opc.Ua.EventSeverity.Low);
            }

            //if (!receiveItem.isValid)
            //    return;

            //if (IPFrameValidator.isunconfirmedWhoIsRequest(receiveItem))
            //{
            //    receiveItem.isValid = false;
            //    if (!DeviceServer.SendiAm(receiveItem.RemoteEndPoint))
            //        CommDriver.OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorSendiAm, Opc.Ua.EventSeverity.Low);
            //}
        }

        public List<Station> GetStations()
        {
            return CommDriver.GetChannelStations(this);
        }

        public void Init()
        {
            listStation = CommDriver.GetChannelStations(this).ConvertAll(x => (BACnetStation)x);

            DeviceServerInit();

            InitAllowedIP(listStation);

            BBMDInit();
        }

        #endregion

        #region Allowed IP

        /// <summary>
        /// Try to add station (with resolved IP) to list of allowed ip incoming message
        /// Execute on start up and before WhoIs
        /// </summary>
        /// <param name="s"></param>
        private void TryToAddGoodStationToAllowedIP(BACnetStation s)
        {
            if (s.RemoteEndPoint != null)
                if (!AllowedIp.ContainsKey(s.RemoteEndPoint.Address.ToString()))
                    AllowedIp[s.RemoteEndPoint.Address.ToString()] = s;

            if (s.HasBBMDDevice())
            {
                if (!AllowedIp.ContainsKey(s.BBMDEndPoint.Address.ToString()))
                    AllowedIp[s.BBMDEndPoint.Address.ToString()] = null; // string.Format("BBMD {0}", device.Value.RemoteEndPoint.Address.ToString());
            }
        }

        //Init list of ip message allowed to this station
        private void InitAllowedIP(List<BACnetStation> list)
        {
            AllowedIp.Clear();
            foreach (BACnetStation s in list)
                TryToAddGoodStationToAllowedIP(s);

            if (bbmdDevices != null)
            {
                foreach (var device in bbmdDevices)
                {
                    BACnetBBMDDevice bbmd = device.Value;
                    if (!string.IsNullOrEmpty(device.Value.RemoteEndPoint.Address.ToString()))
                        if (!AllowedIp.ContainsKey(device.Value.RemoteEndPoint.Address.ToString()))
                            AllowedIp[device.Value.RemoteEndPoint.Address.ToString()] = null; // string.Format("BBMD {0}", device.Value.RemoteEndPoint.Address.ToString());
                }
            }
        }
        #endregion

        #region BBMD

        private void BBMDInit()
        {
            bbmdDevices = new Dictionary<string, BACnetBBMDDevice>();
            BACnetBBMDDevice bbmd;

            foreach (BACnetStation station in listStation)
            {
                if (station.BBMDRegister)
                {
                    if (!bbmdDevices.ContainsKey(station.BBMDId))
                    {
                        //Id and Hostname now is the same parameter
                        bbmd = new BACnetBBMDDevice(station.BBMDId, station.BBMDAddress, station.DeviceHostPort, station.BBMDLifetime);
                        bbmdDevices.Add(bbmd.Id, bbmd);
                    }
                    else
                    {
                        bbmd = bbmdDevices[station.BBMDId];
                        if (bbmd.Lifetime < station.BBMDLifetime)
                            bbmd.Lifetime = station.BBMDLifetime;
                    }
                    bbmd.RelatedStations.Add(station);

                    station.BBDMDevice = bbmd;
                }
            }

            if (bbmdDevices.Count == 0)
                bbmdDevices = null;
        }

        private void BBMDTerminate()
        {
            if (bbmdDevices != null)
            {
                bbmdDevices = null;
            }
        }
        #endregion

        #region Connection's check timer management
        private void StartTmrCheckDeviceCyclically()
        {
            lock (lockThreadObject)
            {
                if (_TmrCheckConnectionCyclically == null)
                    _TmrCheckConnectionCyclically = new Timer(CheckDeviceCyclically, this, Properties.Settings.Default.WhoIsRepeatDelaySlow, Properties.Settings.Default.WhoIsRepeatDelaySlow); // System.Threading.Timeout.Infinite);
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


        private List<Station> GetDevicesToBeChecked()
        {
            List<Station> list = new List<Station>();
            
            foreach (BACnetStation s in CommDriver.GetChannelStations(this))
            {                
                if (!s.WhoIsDisabled && !s.IsSuspended() && s.IsTimeToExecuteWhoIs() && AnyCovSuScribedForStation(s))
                    list.Add(s);
            }
                
            return list;
        }

        Thread currentThread;
        List<Station> stationsToCheck = null;
        private void CheckDeviceCyclically(Object state)
        {            
            try
            {
                stationsToCheck = GetDevicesToBeChecked();
                // if no station need to be checked, exit and wait next timer execution (60sec) without stoping Scheduler/Jobs execution
                if (stationsToCheck == null || stationsToCheck.Count == 0)
                    return;

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
                        SuspendJobsExecution();

                        CheckDevicesConnection(stationsToCheck);
                    }
                    finally
                    {
                        RestartJobsExecution();
                    }
                }
                
            }
            finally
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

        #region Disposable
        public override void Dispose()
        {
            this.NewDataReceived -= BACnetChannel_NewDataReceived;

            StopTmrCheckDeviceCyclically();

            #region Connection's check timer management
            lock (lockThreadObject)
            {
                if (_TmrCheckConnectionCyclicallyFinished != null)
                    _TmrCheckConnectionCyclicallyFinished.Dispose();

            }
            #endregion


            base.Dispose();

            lock (lockCov)
            {
                if (idlSubscriberID != null)
                    idlSubscriberID.Dispose();
                if (ufwSubscriberID != null)
                    ufwSubscriberID.Dispose();
            }            
        }
        #endregion
    }
}
