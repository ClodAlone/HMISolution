////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	BACnetStation.cs
//
// summary:	Implements the driver BACnet station class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Net;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using IpDriverCodeBase;
using System.Threading.Tasks;

namespace BACnet
{
    /// <summary>   Communication target device. </summary>
    public class BACnetBBMDDevice //: Station
    {

        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the station object. </summary>
        ///
        /// <param name="commdriver" type="CommunicationDriver">            The commdriver. </param>
        /// <param name="settings" type="BACnetStationSettings">  Options for controlling the
        ///                                                                 operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        //public BACnetDevice(CommunicationDriver commdriver, BACnetStationSettings settings)
        //    : base(commdriver, settings)
        public BACnetBBMDDevice(BACnetChannel channel)
        {
            _Channel = channel;
            _Id = string.Empty;
            _Address = string.Empty;
            _Port = 47808;
            _Lifetime = 600;
            _State = BBMDStates.RegisterAsForeignDevice;
            _IsRegistred = false;
        }    

        public BACnetBBMDDevice(BACnetChannel channel,string id,string address,int port, uint lifetime, bool register)
        {
            _Channel = channel;
            _Id = id;
            _Address = address;
            _Port = port;
            _Lifetime = lifetime;
            _LastSend = DateTime.UtcNow;
            _IsRegistred = false;
        }

        #endregion

        #region Abstract Methods

        #endregion


        #region member

        #endregion

        #region Properties

        public enum BBMDStates
        {
            RegisterAsForeignDevice,
            WaitRegisterAsForeignDevice,
            //ReadDFTTable,
            //WaitReadDFTTable,
            //ReadBDTTable,
            //WaitReadBDTTable,
            RefreshRegistration,
            WaitRefreshRegistration,
        }

        System.Timers.Timer _DiagnosticTimer;
        System.Timers.Timer _RegistrationTimer;
        BACnetChannel _Channel;        
        

        private string _Id;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   ID
        ///
        /// <value> BBMD device ID
        ///         </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }
                
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>    Register to BBMD as foreign </summary>
        ///
        /// <value> This option force the driver to register with a BBMD device as foreign, with the aim of 
        ///         being addressed with the broadcast messages originated in the BBMD BACnet network. 
        ///         This is compulsory if the device belong to a TCP/IP network different from that of the driver, 
        ///         connected through a router. Routers stops all the global broadcast and this prevent a 
        ///         correct information exchange. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private bool _IsRegistred;
        public bool IsRegistred
        {
            get
            {
                return _IsRegistred;
            }            
        }

        private uint _Lifetime;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>    BBMD registration lifetime (min.)  </summary>
        ///
        /// <value> Duration in minutes of the registration as foreign device.
        ///              durations less then 15 minutes are not allowed </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint Lifetime
        {
            get
            {
                return _Lifetime;
            }
            set
            {
                _Lifetime = value;
            }
        }

        private string _Address;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   BBMD IP address. </summary>
        ///
        /// <value> IP address of the BBMD device. If it is left void, the main BACnet device IP is used. 
        ///         </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string Address
        {
            get
            {
                return _Address;
            }
            set
            {
                _Address = value;
            }
        }                   

        private int _Port;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Send time synchronization. </summary>
        ///
        /// <value> Select to synchronize the time of the device with that of the PC. Two modes are allowed: 
        ///         System Time or UTC Time. 
        ///         If selected, the time synchronization is made during the initialization of the communication. 
        ///         </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int Port
        {
            get
            {
                return _Port;
            }
            set
            {
                _Port = value;
            }
        }

        /// <summary>   The UDP remote end point. </summary>
        IPEndPoint _RemoteEndPoint = null;
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                if (_RemoteEndPoint == null && !String.IsNullOrEmpty(_Address))
                {
                    IPAddress resolvedIPAddress;
                    if (UdpChannel.GetResolvedConnecionIPAddress(_Address, out resolvedIPAddress))
                        _RemoteEndPoint = new IPEndPoint(resolvedIPAddress, _Port);
                }
                return _RemoteEndPoint;
            }
        }

        private BBMDStates _State;
        public BBMDStates State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;
                switch (_State)
                {
                    case BACnetBBMDDevice.BBMDStates.RefreshRegistration:
                        if (_DiagnosticTimer != null)
                        {
                            _DiagnosticTimer.Stop();
                        }

                        if (_RegistrationTimer != null)
                        {
                            uint NewInterval = 0;
                            if (_IsRegistred)
                                NewInterval = (_Lifetime / 2) * 1000;
                            else
                                NewInterval = (uint)1000;

                            if (NewInterval != _RegistrationTimer.Interval)
                            {
                                _RegistrationTimer.Stop();
                                _RegistrationTimer.Interval = NewInterval;
                                _RegistrationTimer.Start();
                            }                                
                        }
                                                
                        break;
                    case BACnetBBMDDevice.BBMDStates.WaitRefreshRegistration:
                        if (_RegistrationTimer != null)
                        {
                            _RegistrationTimer.Stop();
                            _RegistrationTimer.Interval = _Channel.Timeout;
                            _RegistrationTimer.Start();
                        }
                        break;
                }
            }
        }

        private DateTime _LastSend;
        public DateTime LastSend
        {
            get
            {
                return _LastSend;
            }
            set
            {
                _LastSend = value;
            }
        }       

        public List<BACnetStation>RelatedStations = new List<BACnetStation>();
        #endregion

        public void SetRelatedStationStatus(DriverErrorCodes errorCode)
        {
            _IsRegistred = (errorCode == DriverErrorCodes.ErrorNoError);

            //foreach (BACnetStation station in RelatedStations)
            Parallel.ForEach(RelatedStations, station =>
             {
                 bool StatusChanged = (station.IsBBMDRegistred != _IsRegistred);

                 station.IsBBMDRegistred = _IsRegistred;
                 if (errorCode != DriverErrorCodes.ErrorNoError)
                 {
                     System.Diagnostics.Debug.WriteLine("Station {0} - Channel {1} ", station.Name, station.GetChannel().Name);
                     if (station.LastErrorCode == DriverErrorCodes.ErrorNoError)
                     {
                         station.LastErrorTime = DateTime.UtcNow;
                         station.LastErrorCode = errorCode;
                        //AdditionalError = (TagsList.Count > 0 ? TagsList[0].DynSettings.ToString() : null);
                    }
                     //lock (((BACnetStation)station).getLockBool())
                     //{
                        //if (InUse)
                        station.InErrorState = true;
                        //if (((BACnetStation)Station).GetChannelBase() != null)
                        //    ((BACnetStation)Station).GetChannelBase().ChangeStateJob(this, CommJobState.PollingInError);

                    //}
                 }
                 else if (station.InErrorState)
                 {
                     var listJob = station.GetChannel().GetJobStateList(CommJobState.PollingInError);
                     bool resetError = true;
                     foreach (var commjob in listJob)
                     {
                         if (commjob.InErrorState && commjob.InUse)
                             resetError = false;
                     }
                     if (resetError)
                     {
                         System.Diagnostics.Debug.WriteLine("Station {0} ResetErrors - Channel {1} ResetErrors", station.Name, station.GetChannel().Name);
                         station.InErrorState = false;

                         station.LastErrorTime = DateTime.MinValue;
                         station.LastErrorCode = DriverErrorCodes.ErrorNoError;
                     }
                 }

                 //lock (station.getLockBool())
                 //{
                    //if (!InErrorState && Station.FirstTime)
                    if (station.FirstTime)
                     {
                         station.FirstTime = false;
                         station.GetCommDriver().OnSystemEvent(null, String.Format(DriverCodeBase.Properties.Resources.StationResumeError, station.GetCommDriver().DriverName/*DriverInfo.GetDriverName()*/, station.Name), EventSeverity.Low);
                     }
                 //}

                 if (!station.IsBBMDRegistred)
                 {
                     var listJobs = station.GetListWholeJobCopy();
                     foreach (var elJob in listJobs)
                     {
                         var job = elJob as BACnetCommJob;
                         _Channel.UnsubscribeJob(job);
                         if (!job.InErrorState)
                             job.SetBadConnectionQuality();
                         job.BACnetInitDone = false;
                     }

                     station.WhoIsTxTime = DateTime.UtcNow;
                     station.BACnetInitDone = false;
                     station.mapSubscribeTime.Clear();
                 }
                 else
                 {
                    // force to check connection to devices immediatly
                    if (StatusChanged)
                         station.WhoIsTxTime = DateTime.MinValue;
                 }
             });
        }

        /// <summary>
        /// Start timer ofr Foreign Registration and Keep Alive management
        /// </summary>
        public void StartKeepAliveRegistration()
        {
            if (_DiagnosticTimer == null)
            {
                _DiagnosticTimer = new System.Timers.Timer() { Interval = 1000 };
                _DiagnosticTimer.Elapsed += _DiagnosticTimer_Elapsed;
                _DiagnosticTimer.Start();
            }
            if (_RegistrationTimer == null)
            {
                _RegistrationTimer = new System.Timers.Timer() { Interval = 1 };
                _RegistrationTimer.Elapsed += _RegistrationTimer_Elapsed;
                _RegistrationTimer.Stop(); // --> start it after diagnostic timer complete sequence
            }            
        }


        private bool _IsDiagnosticTimerRunning = false;
        private void _DiagnosticTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_IsDiagnosticTimerRunning)
            {
                _IsDiagnosticTimerRunning = true;
                switch (State)
                {
                    case BACnetBBMDDevice.BBMDStates.RegisterAsForeignDevice:
                        if (!_Channel.DeviceWrite(IPFrameFactory.RegisterAsForeignDevice(_Channel, (ushort)Lifetime), RemoteEndPoint))
                        {
                            SetRelatedStationStatus((DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);
                        }
                        else
                        {
                            _LastSend = DateTime.UtcNow;
                            _State = BACnetBBMDDevice.BBMDStates.WaitRegisterAsForeignDevice;
                        }
                        break;
                    //case BACnetBBMDDevice.BBMDStates.WaitRegisterAsForeignDevice:
                    //    // no answer from device; retry
                    //    if ((DateTime.UtcNow.Subtract(LastSend)).TotalMilliseconds > _Channel.Timeout)
                    //    {
                    //        SetRelatedStationStatus((DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);
                    //        _State = BACnetBBMDDevice.BBMDStates.RegisterAsForeignDevice;
                    //    }
                    //    break;
                    //case BACnetBBMDDevice.BBMDStates.ReadDFTTable:
                    //    if (!_Channel.DeviceWrite(IPFrameFactory.ReadForeignDeviceTable(_Channel), RemoteEndPoint))
                    //    {
                    //        SetRelatedStationStatus((DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);
                    //    }
                    //    else
                    //    {
                    //        LastSend = DateTime.UtcNow;
                    //        _State = BACnetBBMDDevice.BBMDStates.WaitReadDFTTable;
                    //    }
                    //    break;
                    //case BACnetBBMDDevice.BBMDStates.WaitReadDFTTable:
                    //    // no answer from device; retry
                    //    if ((DateTime.UtcNow.Subtract(LastSend)).TotalMilliseconds > _Channel.Timeout)
                    //    {
                    //        _LastSend = DateTime.UtcNow;
                    //        _State = BACnetBBMDDevice.BBMDStates.RegisterAsForeignDevice;
                    //    }
                    //    break;
                    //case BACnetBBMDDevice.BBMDStates.ReadBDTTable:
                    //    if (!_Channel.DeviceWrite(IPFrameFactory.ReadBBMDDeviceTable(_Channel), _RemoteEndPoint))
                    //    {
                    //        SetRelatedStationStatus((DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);
                    //    }
                    //    else
                    //    {
                    //        _LastSend = DateTime.UtcNow;
                    //        _State = BACnetBBMDDevice.BBMDStates.WaitReadBDTTable;
                    //    }
                    //    break;
                    case BACnetBBMDDevice.BBMDStates.WaitRegisterAsForeignDevice:
                        // no answer from device; retry
                        if ((DateTime.UtcNow.Subtract(LastSend)).TotalMilliseconds > _Channel.Timeout)
                        {
                            SetRelatedStationStatus((DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);

                            _LastSend = DateTime.UtcNow;
                            _State = BACnetBBMDDevice.BBMDStates.RegisterAsForeignDevice;
                        }
                        break;
                }
                _IsDiagnosticTimerRunning = false;
            }
        }

        private bool _IsRegistrationTimerRunning = false;
        private void _RegistrationTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            if (!_IsRegistrationTimerRunning)
            {
                _IsRegistrationTimerRunning = true;

                switch (State)
                {
                    case BACnetBBMDDevice.BBMDStates.RefreshRegistration:
                        if (!_Channel.DeviceWrite(IPFrameFactory.RegisterAsForeignDevice(_Channel, (ushort)_Lifetime), _RemoteEndPoint))
                        {
                            SetRelatedStationStatus((DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);
                            State = BACnetBBMDDevice.BBMDStates.RefreshRegistration;
                        }
                        else
                        {
                            LastSend = DateTime.UtcNow;
                            State = BACnetBBMDDevice.BBMDStates.WaitRefreshRegistration;                            
                        }
                        break;
                    case BACnetBBMDDevice.BBMDStates.WaitRefreshRegistration:
                        SetRelatedStationStatus((DriverErrorCodes)BACnetErrorCodes.ErrorRegisterToBBMD);
                        State = BACnetBBMDDevice.BBMDStates.RefreshRegistration;
                        break;
                }

                _IsRegistrationTimerRunning = false;
            }
        }

        /// <summary>
        /// Stop timer ofr Foreign Registration and Keep Alive management
        /// </summary>
        public void StopKeepAliveRegistration()
        {
            if (_DiagnosticTimer != null)
            {
                _DiagnosticTimer.Stop();
                _DiagnosticTimer.Elapsed -= _DiagnosticTimer_Elapsed;
                _DiagnosticTimer.Dispose();
            }
            if (_RegistrationTimer != null)
            {
                _RegistrationTimer.Stop();
                _RegistrationTimer.Elapsed -= _RegistrationTimer_Elapsed;
                _RegistrationTimer.Dispose();
            }
        }
    }
}
