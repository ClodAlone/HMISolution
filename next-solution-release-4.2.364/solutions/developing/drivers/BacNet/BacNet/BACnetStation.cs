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
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using Opc.Ua;
using IpDriverCodeBase;
using System.Threading.Tasks;

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
            _IsBBMDRegistred = false;
            _BBMDRegister = settings.BBMDRegister;
            _BBMDAddress = settings.BBMDAddress;
            _BBMDLifetime = settings.BBMDLifetime;
            _BBMDId = _BBMDAddress;
            if (string.IsNullOrEmpty(_BBMDId))
                _BBMDId = string.Empty;
            _DeviceHostName = settings.DeviceHostName;
            _DeviceHostPort = settings.DeviceHostPort;
            _DeviceInstance = settings.DeviceInstance;
            _WhoIsDisabled = settings.WhoIsDisabled;

            _WhoIsError = false;
            BACnetInitDone = false;
            DeviceIdentifier = new BACnetObjectIdentifier(0);
            MaxAPDULength = 0;
            DestinationSpecifierPresent = false;

            SegmentationSupported = 0;
            VendorIdentifier = 0;
            WhoIsTxTime = DateTime.MinValue;
            mapObjectID = new Dictionary<string, BACnetObjectIdentifier>();
            mapSubscribeTime = new Dictionary<UInt32, DateTime>();
            mapObjectIDSubscribedID = new Dictionary<UInt32, UInt32>();
            mapObjectSubscribeID = new Dictionary<string, UInt32>();

            WhoIsDelayMs = Properties.Settings.Default.WhoIsRepeatDelay;
        }
       
        #endregion

        #region Abstract Methods

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

        #endregion

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Compare tags by Dynamic Settings. Return 0 if x = y , 1 if x &gt; y and -1 if x &lt; y.
        /// </summary>
        ///
        /// <param name="x">    . </param>
        /// <param name="y">    . </param>
        ///
        /// <returns>   An int. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        private static int CompareTagByDynamic(Tag x, Tag y)
        {
            if (x == null)
            {
                if (y == null)
                    return 0; //==
                else
                    return -1;// x < y
            }
            else
            {
                //x!= null
                if (y == null)
                    return 1; //x > y
                else
                {
                    BACnetDynTagSettings dts = new BACnetDynTagSettings();
                    dts.TryParse(x.TagNode.DynamicSettings);
                    string cx = string.Format("DS{0}TL{1}MI{2}OBN{3}OBT{4}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.ObjectName,
                        dts.BACnetObjectType);
                    dts.TryParse(y.TagNode.DynamicSettings);
                    string cy = string.Format("DS{0}TL{1}MI{2}OBN{3}OBT{4}",
                        dts.DeviceSize.ToString("000"), dts.TagLinkType.ToString("000"),
                        dts.MethodID.ToString("000"), dts.ObjectName,
                        dts.BACnetObjectType);
                    return cx.CompareTo(cy);
                    //return x.TagNode.DynamicSettings.CompareTo(y.TagNode.DynamicSettings);
                }
            }
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
            List<Tag> listTag = new List<Tag>();
            listTag.AddRange(tags);
            listTag.Sort(CompareTagByDynamic);
            return listTag;
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

                        if (Set) {
                            bt.ValueNullIsGood = true;
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

            base.ProcessJobValues(e);
        }

        int ConsecutiveGeneralErrors;

        protected override void ProcessJobLists(CommJob job, DriverErrorCodes error, bool bCheckNumRetries = true)
        {
            base.ProcessJobLists(job, error, bCheckNumRetries);
            if (error == (DriverErrorCodes)BACnetErrorCodes.ErrorBACnetTimeOut && ++ConsecutiveGeneralErrors >= MaxRetriesBeforeError)
            {
                //too much severe errors. Unsubscribe jobs, set the not in error to badnotconnected, trigger a sudded  WhoIs for the station
                var listJob = new List<CommJob>();
                lock (lockListObject)
                {
                    listJob.AddRange(ListWholeJob);
                }
                foreach (var tempjob in listJob)
                {
                    var ejob = tempjob as BACnetCommJob;
                    ChannelBase.UnsubscribeJob(ejob);
                    if (!ejob.InErrorState)
                        ejob.SetBadConnectionQuality();
                    ejob.BACnetInitDone = false;
                }
                WhoIsTxTime = DateTime.UtcNow.AddMilliseconds(-Properties.Settings.Default.WhoIsRepeatDelay);
                BACnetInitDone = false;
                System.Diagnostics.Debug.WriteLine("ErrorBACnetTimeOut consecutive errors limit reached");
            }
            else if(ConsecutiveGeneralErrors != 0)
            {
                ConsecutiveGeneralErrors = 0;
            }
        }

        ///// <summary>
        ///// Check if at least one job containt one tag quality is marked badtimeout; use this information to determine if station is still connected or not
        ///// </summary>
        ///// <returns></returns>
        //public bool AnyJobsInTimeOut()
        //{
        //    int NrJobs = 0;
        //    lock (lockListObject)
        //    {
        //        List<CommJob> jobs = (from job in ListWholeJob where job.InErrorState select job).AsParallel().ToList();
        //        foreach (var job in jobs)
        //            NrJobs += ((from tag in job.TagsList where tag.Value.StatusCode == StatusCodes.BadTimeout select tag).AsParallel().Count() > 0 ? 1 : 0);

        //        //System.Diagnostics.Debug.WriteLine(string.Format("Station:{0}, CountJobInTimeOut Nr:{1}",this.Name,jobs.Count));
        //    }
        //    return (NrJobs>0);
        //}


        public object getLockBool()
        {
            return lockBool;
        }
        public DriverCodeBase.IChannelBase GetChannelBase()
        {
            return ChannelBase;
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

        public DateTime WhoIsTxTime;
        public int WhoIsDelayMs;

        /// <summary>   The map ObjectID. </summary>
        public Dictionary<string, BACnetObjectIdentifier> mapObjectID;
        public Dictionary<UInt32, DateTime> mapSubscribeTime;
        public Dictionary<UInt32, UInt32> mapObjectIDSubscribedID;  //< ObjectID.value,SubscriberID >
        public Dictionary<string, UInt32> mapObjectSubscribeID;   //< name,SubscriberID  >

        #endregion

        #region Properties
        //private bool _RegisterBBMD;
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        ///// <summary>    Register to BBMD as foreign </summary>
        /////
        ///// <value> This option force the driver to register with a BBMD device as foreign, with the aim of 
        /////         being addressed with the broadcast messages originated in the BBMD BACnet network. 
        /////         This is compulsory if the device belong to a TCP/IP network different from that of the driver, 
        /////         connected through a router. Routers stops all the global broadcast and this prevent a 
        /////         correct information exchange. </value>
        //////////////////////////////////////////////////////////////////////////////////////////////////////
        //public bool RegisterBBMD
        //{
        //    get
        //    {
        //        return _RegisterBBMD;
        //    }
        //    set
        //    {
        //        _RegisterBBMD = value;
        //    }
        //}

        private uint _BBMDLifetime;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>    BBMD registration lifetime (min.)  </summary>
        ///
        /// <value> Duration in minutes of the registration as foreign device.
        ///              durations less then 15 minutes are not allowed </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint BBMDLifetime
        {
            get
            {
                return _BBMDLifetime;
            }
            set
            {
                _BBMDLifetime = value;
            }
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
            get
            {
                return _BBMDAddress;
            }
            set
            {
                _BBMDAddress = value;
            }
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
            get
            {
                return _BBMDId;
            }            
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
            get
            {
                return _BBMDRegister;
            }
            set
            {
                _BBMDRegister = value;
            }
        }

        private bool _IsBBMDRegistred;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   IsBBMDRegister
        ///
        /// <value> Station's registration to BBMD status --> active/disactive
        ///         </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool IsBBMDRegistred
        {
            get
            {
                return _IsBBMDRegistred;
            }
            set
            {
                _IsBBMDRegistred = value;
            }
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
            get
            {
                return _TimeSync;
            }
            set
            {
                _TimeSync = value;
            }
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
            get
            {
                return _COVInterval;
            }
            set
            {
                _COVInterval = value;
            }
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
            get
            {
                return _ForceInitialPolling;
            }
            set
            {
                _ForceInitialPolling = value;
            }
        }

        private bool _WhoIsError;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   'Who Is' request has been successfully replied </summary>
        ///
        /// <value> true if the station has not replied to the 'Who Is' request, false otherwise
        /// </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool WhoIsError
        {
            get
            {
                return _WhoIsError;
            }
            set
            {
                _WhoIsError = value;
            }
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
            set
            {
                _DeviceHostName = value;
            }
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
            get
            {
                return _DeviceHostPort;
            }
            set
            {
                _DeviceHostPort = value;
            }
        }
        /// <summary>   Device Number. </summary>
        private string _DeviceInstance;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the Device Number. </summary>
        ///
        /// <value> The Device Number. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string DeviceInstance
        {
            get
            {
                return _DeviceInstance;
            }
            set
            {
                _DeviceInstance = value;
            }
        }

        /// <summary>   The UDP remote end point. </summary>
        IPEndPoint _RemoteEndPoint = null;
        public IPEndPoint RemoteEndPoint
        {
            get
            {
                if (_RemoteEndPoint == null && !String.IsNullOrEmpty(_DeviceHostName))
                {
                    IPAddress resolvedIPAddress;
                    if (UdpChannel.GetResolvedConnecionIPAddress(_DeviceHostName, out resolvedIPAddress))
                        _RemoteEndPoint = new IPEndPoint(resolvedIPAddress, _DeviceHostPort);
                }
                return _RemoteEndPoint;
            }
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
            get
            {
                return _WhoIsDisabled;
            }
            set
            {
                _WhoIsDisabled = value;
            }
        }

        /// <summary>   Return ListWholeJob job list. </summary>
        public List<CommJob> getListWholeJob
        {
            get
            {
                return ListWholeJob;
            }
        }

        public List<CommJob>  GetListWholeJobCopy()
        {
            var listJob = new List<CommJob>();
            lock (lockListObject)
            {
                listJob.AddRange(ListWholeJob);
            }
            return listJob;
        }
        public int getDeviceInstance()
        {
            if (string.IsNullOrEmpty(_DeviceInstance))
                return -1;
            UInt32 numVal;
            try
            {
                numVal = Convert.ToUInt32(_DeviceInstance);
            }
            catch (FormatException e)
            {
                return -1;
            }
            catch (OverflowException e)
            {
                return -1;
            }
            if (numVal > BACnetEnums.MAX_INSTANCE)
                return -1;
            return (int)numVal;
        }

        public byte[] getDeviceInstanceToADR()
        {
            int numVal = getDeviceInstance();
            byte[] ADR = new byte[6];
            byte[] InstanceADR = BitConverter.GetBytes(numVal);

            //Array.Reverse(InstanceADR, 0, InstanceADR.Length);

            for (int Index=0; Index<InstanceADR.Length;Index++)
            {
                if (InstanceADR[Index] !=0)
                {
                    Array.Copy(InstanceADR, Index,ADR, 0, InstanceADR.Length - Index);
                    break;
                }
            }
                        
            return ADR;
        }

        /// <summary>
        /// Set last date/time of good answer from device; used "to simulate " the last Who-Is good answer
        /// </summary>
        public void SetLastDateTimeGoodAnswer()
        {
            if (BACnetInitDone)
                WhoIsTxTime = DateTime.UtcNow;
            else
                // if valid data was recived after disconnection (tipically cov), reset WhoIsTxTime timesout so driver will execute WhoIs immediatly
                WhoIsTxTime = new DateTime();
        }

        #endregion

    }
}
