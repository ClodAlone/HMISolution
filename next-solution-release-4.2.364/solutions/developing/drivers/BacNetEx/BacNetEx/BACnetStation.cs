////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	BACnetStation.cs
//
// summary:	Implements the driver BACnet station class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using DevExpress.Xpo;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using IpDriverCodeBaseEx;
using Opc.Ua;
using UFUAModel.Extensions;

namespace BACnet
{
    /// <summary>   Communication target device. </summary>
    public class BACnetStation : Station
    {
        #region Constructors

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Initializes the station object. </summary>
        ///
        /// <param name="commdriver" type="CommunicationDriver">            The commdriver. </param>
        /// <param name="settings" type="BACnetStationSettings">  Options for controlling the
        ///                                                                 operation. </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public BACnetStation(CommunicationDriver commdriver, BACnetStationSettings settings)
            : base(commdriver, settings)
        {
            _ForceInitialPolling = settings.ForceInitialPolling;
            _COVInterval = settings.COVInterval;
            _TimeSync = settings.TimeSync;            
            _BBMDRegister = settings.BBMDRegister;            
            _BBMDAddress = settings.BBMDAddress;
            _BBMDLifetime = settings.BBMDLifetime;
            _BBMDId = _BBMDAddress;
            if (string.IsNullOrEmpty(_BBMDId))
                _BBMDId = string.Empty;
            _DeviceHostName = settings.DeviceHostName;
            _DeviceHostPort = settings.DeviceHostPort;
            _DeviceInstance = BACnetProtocol.ParseDeviceInstanceString(settings.DeviceInstance);
            _RunTimeDeviceInstance = -1;
            _WhoIsDisabled = settings.WhoIsDisabled;

            BACnetInitDone = false;
            DeviceIdentifier = new BACnetObjectIdentifier(0);
            MaxAPDULength = 0;
            DestinationSpecifierPresent = false;

            SegmentationSupported = 0;
            VendorIdentifier = 0;
            LastMessageFromDevice = DateTime.MinValue;
        }

        #endregion

        #region Override Methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new protocol task instance from CommJobSettings. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="JobSettings">  . </param>
        ///
        /// <returns>   The new job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJob CreateJob(CommJobSettings JobSettings)
        {
            var conf = JobSettings as BACnetCommJobSettings;
            if (conf == null)
                throw new ArgumentException("Invalid communication job settings");

            return new BACnetCommJob(this, conf);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new protocol task instance from Tag. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="defTag">   . </param>
        ///
        /// <returns>   The new job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJob CreateJob(Tag defTag)
        {
            var conf = defTag as BACnetTag;
            if (conf == null)
                throw new ArgumentException("Invalid tag object");

            return new BACnetCommJob(this, conf);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new variable instance from TagDefinition. </summary>
        ///
        /// <param name="td">   . </param>
        ///
        /// <returns>   The new tag. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override Tag CreateTag(DriverBaseInterfaces.TagDefinition td)
        {
            return new BACnetTag(td);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Driver call for create a new settings instance for protocol's task. </summary>
        ///
        /// <exception cref="ArgumentException">    Thrown when one or more arguments have unsupported or
        ///                                         illegal values. </exception>
        ///
        /// <param name="session">  . </param>
        /// <param name="job">      . </param>
        ///
        /// <returns>   The new job settings. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override CommJobSettings CreateJobSettings(Session session, CommJob job)
        {
            var commJob = job as BACnetCommJob;
            if (commJob == null)
                throw new ArgumentException("Invalid job object");

            return new BACnetCommJobSettings(session, commJob);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Sort the list of tags for dynamic settings. </summary>
        ///
        /// <param name="tags"> . </param>
        ///
        /// <returns>   The sorted tags. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override List<Tag> SortTags(IList<Tag> tags)
        {
            return tags.ToList();
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Processed received data in ExecutedJobArgs. To call asyncronously... </summary>
        ///
        /// <param name="e">    . </param>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void ProcessJobValues(ExecutedJobArgs e)
        {
            BACnetCommJob mJ = e.Job as BACnetCommJob;
            if (mJ == null)
                return;

            BACnetExecutedJobArgs be = e as BACnetExecutedJobArgs;

            // analyzing answer if no error exists before
            if (e.ErrorCode == DriverErrorCodes.ErrorNoError && (be.IsRead == true))
            {
                if (e.Values == null)
                {
                    foreach (Tag tag in mJ.TagsList)
                    {
                        bool Set = false;

                        BACnetTag bt = tag as BACnetTag;
                        if (tag.Value.Value != null)
                        {
                            Set = true;
                        }
                        else
                        {
                            // handle array property null value condition --> could be null on first cicle
                            if (bt.BACnetDynSettings.PropertyIdentifier == BACnetEnums.PropertyIdentifier.PRIORITY_ARRAY)
                                Set = (bt.ProcessJobValuesFirstTime || bt.GetReadValue() != tag.Value.Value);
                        }

                        if (Set)
                        {
                            //bt.ValueNullIsGood = true;
                            tag.SetInternalValues(null);
                            tag.SetReadValue(null);
                            tag.Value.Value = null;
                            e.ChangedTags.Add(tag);
                        }
                        bt.ProcessJobValuesFirstTime = false;
                    }
                }
                else
                {
                    // data were copied from different job --> don't make any to check if change
                    if (be.IsCovDataFromDifferentJob)
                    {
                        foreach (var tag in e.Job.TagsList)
                        {
                            var j = tag as Tag;
                            if (j != null)
                                e.ChangedTags.Add(j);

                            BACnetTag bt = tag as BACnetTag;
                            if (bt != null)
                                bt.ProcessJobValuesFirstTime = false;
                        }
                    }
                    else
                    {

                        byte[] Answer = (byte[])e.Values;
                        List<object> ChangedTags = new List<object>();
                        if (BACnetProtocol.ParseData(Answer, ref mJ, ref ChangedTags))
                            foreach (var tag in ChangedTags)
                            {
                                var j = tag as Tag;
                                if (j != null)
                                    e.ChangedTags.Add(j);

                                BACnetTag bt = tag as BACnetTag;
                                if (bt != null)
                                    bt.ProcessJobValuesFirstTime = false;

                            }
                        else
                            e.ErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                    }
                }
            }

            e.GeneralError = (e.ErrorCode == DriverErrorCodes.ErrorTimeOut);

            base.ProcessJobValues(e);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Validation of the response data type input. If correct copy data in job. </summary>
        ///
        /// <param name="receivedbuffer">   . </param>
        /// <param name="job">              . </param>
        /// <param name="arguments">        [in,out]. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override bool ParseReceivedToArguments(byte[] receivedbuffer, CommJob job, ref List<Object> arguments)
        {
            BACnetCommJob mj = job as BACnetCommJob;

            BACnetProtocol.ParseData(receivedbuffer, ref mj, ref arguments);
            return true;
        }
        #endregion

        #region Method
        public void PutAllJobsInError()
        {
            List<BACnetCommJob> listJobs;
            lock (lockListObject)
            {
                listJobs = ListWholeJob.ConvertAll(x => (BACnetCommJob)x).Where(a => a.InUse).ToList();
            }

            if (listJobs.Count > 0)
            {
                // force to execute WhoIs after communication timeout; other BACnet command could be 
                BACnetInitDone = false;
                //_RunTimeDeviceInstance = -1;            

                SetStateCommandVariableBit(true, (UInt16)StationVariableBits.StationErrorState);

                foreach (BACnetCommJob job in listJobs)
                { 
                    if (!job.InErrorState)
                        job.SetBadConnectionQuality();
                    PutJobInError(job);
                }
            }
        }

        public void PutJobInError(BACnetCommJob job)
        {            
            if (job.COVEnable)
                ((BACnetChannel)Channel).SetLastCOVExecutionTime(job, DriverErrorCodes.ErrorTimeOut);

            //reset all parameters acquired at runtime like Instance Number, Cov and So On
            // BACnet state is reset but not Instance Number, Object ID and so on; necessary to manage WhoIs
            //job.ResetBacnetRunTimeParameters();

            job.SetBacNetStateInError();
        }

        public List<CommJob> GetListWholeJobCopy()
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }
            return listJob;
        }

        public bool HasDeviceInstance()
        {
            return (_DeviceInstance != -1 || _RunTimeDeviceInstance != -1);
        }

        public bool HasRunTimeDeviceInstance()
        {
            return (_RunTimeDeviceInstance != -1);
        }

        public bool HasBBMDDevice()
        {
            return BBMDRegister;
        }
        
        public int GetDeviceInstance()
        {
            //int instance = BACnetProtocol.ParseDeviceInstanceString(_DeviceInstance);
            //if (instance == -1 && string.IsNullOrEmpty(_DeviceInstance))
            //    instance = BACnetProtocol.ParseDeviceInstanceString(RunTimeDeviceInstance);
            if (_DeviceInstance != -1)
                return _DeviceInstance;
            else if (_RunTimeDeviceInstance != -1)
                return _RunTimeDeviceInstance;
            else
                return -1;
        }

        public byte[] getDeviceInstanceToADR()
        {
            int numVal = GetDeviceInstance();
            byte[] ADR = new byte[6];
            byte[] InstanceADR = BitConverter.GetBytes(numVal);

            //Array.Reverse(InstanceADR, 0, InstanceADR.Length);

            for (int Index = 0; Index < InstanceADR.Length; Index++)
            {
                if (InstanceADR[Index] != 0)
                {
                    Array.Copy(InstanceADR, Index, ADR, 0, InstanceADR.Length - Index);
                    break;
                }
            }

            return ADR;
        }

        /* use to identify a special cov's job (only 1 for station) used to send WhoIs to check if device is stil connected; LastExecutionTime is move forward each exection
         * by COV_WATCHDOG_POLLING_TIME */
        public bool IsTimeToExecuteWhoIs()
        {
            //return (DateTime.UtcNow.Subtract(lastAnswerFromDevice).TotalSeconds > BACnetProtocol.COV_WATCHDOG_POLLING_TIME);
            return (DateTime.UtcNow.Subtract(LastMessageFromDevice).TotalMilliseconds > (uint)(Properties.Settings.Default.WhoIsRepeatDelaySlow));
        }

        public bool IsSuspended()
        {
            bool suspendBitNewValue = false;
            if (GetStateCommandVariableBit(ref suspendBitNewValue, (UInt16)StationVariableBits.StationActiveCommand) == true)
            {
                if (suspendBitNewValue)
                    return true;                    
            }

            return false;
        }
        #endregion

        #region member
        public bool BACnetInitDone;
        public BACnetObjectIdentifier DeviceIdentifier;
        public UInt32 MaxAPDULength;
        public uint SegmentationSupported;
        public UInt16 VendorIdentifier;
        public bool DestinationSpecifierPresent;
        public UInt16 DNET;
        public byte DLEN;
        public byte[] DADR;

        public DateTime LastMessageFromDevice;
        #endregion

        #region Properties
        private uint _BBMDLifetime;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>    BBMD registration lifetime (min.)  </summary>
        ///
        /// <value> Duration in minutes of the registration as foreign device.
        ///              durations less then 15 minutes are not allowed </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint BBMDLifetime
        {
            get { return _BBMDLifetime; }
            set { _BBMDLifetime = value; }
        }

        private string _BBMDAddress;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   BBMD IP address. </summary>
        ///
        /// <value> IP address of the BBMD device. If it is left void, the main BACnet device IP is used. 
        ///         </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string BBMDAddress
        {
            get { return _BBMDAddress; }
            set { _BBMDAddress = value; }
        }

        private int _BBMDAddressHostPort
        {
            get { return _DeviceHostPort; }
        }

        private string _BBMDId;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>  BBMD ID </summary>
        ///
        /// <value> Identifier of BBMD device
        ///         </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string BBMDId
        {
            get { return _BBMDId;}
        }

        private bool _BBMDRegister;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   BBMDRegister
        ///
        /// <value> Registration to BBMD device is required
        ///         </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool BBMDRegister
        {
            get { return _BBMDRegister; }
            set { _BBMDRegister = value; }
        }
        
        private TimeSyncs _TimeSync;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Send time synchronization. </summary>
        ///
        /// <value> Select to synchronize the time of the device with that of the PC. Two modes are allowed: 
        ///         System Time or UTC Time. 
        ///         If selected, the time synchronization is made during the initialization of the communication. 
        ///         </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public TimeSyncs TimeSync
        {
            get { return _TimeSync; }
            set { _TimeSync = value; }
        }

        private uint _COVInterval;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   COV duration (sec.). </summary>
        ///
        /// <value> Specifies the duration of the Change Of Value subscription for a property of a Data Object,
        ///         in seconds. 0 means indefinite subscription,   values other than 0 require the renewal of 
        ///         the subscription, upon elapsing of this time.
        ///
        /// . </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint COVInterval
        {
            get { return _COVInterval; }
            set { _COVInterval = value; }
        }

        private bool _ForceInitialPolling;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Force Initial Polling. </summary>
        ///
        /// <value> Normally, after a COV subscription is accepted, the device sends the "present value" of 
        ///         a data object. If the device doesn't act so, set this option to "True", to force a 
        ///         single read operation of the current value of a data object, after the COV subscription.
        /// </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool ForceInitialPolling
        {
            get { return _ForceInitialPolling; }
            set { _ForceInitialPolling = value; }
        }

        /// <summary>   Device Host name. </summary>
        private string _DeviceHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the Device host. </summary>
        ///
        /// <value> The name of the Device host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string DeviceHostName
        {
            get { return _DeviceHostName; }
            set { _DeviceHostName = value;}
        }

        /// <summary>   Device Host port. </summary>
        private int _DeviceHostPort;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Device host port. </summary>
        ///
        /// <value> The Device host port. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int DeviceHostPort
        {
            get { return _DeviceHostPort; }
            set {_DeviceHostPort = value; }
        }
        /// <summary>   Device Number. </summary>
        private int _DeviceInstance;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Device Number. </summary>
        ///
        /// <value> The Device Number. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int DeviceInstance
        {
            get { return _DeviceInstance; }
            set { _DeviceInstance = value; }
        }

        public int _RunTimeDeviceInstance;
        public int RunTimeDeviceInstance
        {
            get { return _RunTimeDeviceInstance; }
            set { _RunTimeDeviceInstance = value; }
        }

        /// <summary>   The UDP remote end point. </summary>
        IPEndPoint _RemoteEndPoint = null;
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                if (_RemoteEndPoint == null && !String.IsNullOrEmpty(_DeviceHostName))
                {
                    try
                    {
                        if (UDPManager.GetResolvedConnecionIPAddress(_DeviceHostName, out IPAddress resolvedIPAddress))
                            _RemoteEndPoint = new IPEndPoint(resolvedIPAddress, _DeviceHostPort);
                    } catch (Exception ex)
                    {
                        _RemoteEndPoint = null;
                    }
                }
                return _RemoteEndPoint;
            }
        }

        /// <summary>   The UDP remote end point. </summary>
        IPEndPoint _BBMDEndPoint = null;
        public IPEndPoint BBMDEndPoint
        {
            get
            {
                if (_BBMDEndPoint == null && !String.IsNullOrEmpty(_BBMDAddress))
                {
                    try
                    {
                        if (UDPManager.GetResolvedConnecionIPAddress(_BBMDAddress, out IPAddress resolvedIPAddress))
                            _BBMDEndPoint = new IPEndPoint(resolvedIPAddress, _BBMDAddressHostPort);
                    }
                    catch (Exception ex)
                    {
                        _BBMDEndPoint = null;
                    }
                }
                return _BBMDEndPoint;
            }
        }


        BACnetBBMDDevice _bbmdDevice;
        public BACnetBBMDDevice BBDMDevice
        {
            get { return _bbmdDevice; }
            set { _bbmdDevice = value; }
        }

        private bool _WhoIsDisabled;        

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   'Who Is' request is enabled </summary>
        ///
        /// <value> true if the station has not use 'Who Is' to retrive information from device , false otherwise
        ///         Used in some network configuration to baypass the use of BBMD --> Device Identifier is required
        /// </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool WhoIsDisabled
        {
            get { return _WhoIsDisabled;}
            set { _WhoIsDisabled = value; }
        }
        #endregion
    }
}
