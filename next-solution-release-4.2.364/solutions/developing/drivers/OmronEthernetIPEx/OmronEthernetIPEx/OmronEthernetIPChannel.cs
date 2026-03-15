using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using IpDriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using System.Threading.Tasks;
using System.Threading;

namespace OmronEthernetIP
{
    
    public class OmronEthernetIPChannel : TcpChannelList, IDisposable
    {
        public enum CommandRequestOfDevice
        {
            GetTheTypeOfDevice,
            GetNumberOfItem,
            GetAllItems,
            GetAttributeSingle,
            GetAttributesAll,
            GetAttributeSingleVariableTypeObject,
            GetAttributesAllVariableTypeObject,
        }       
        public class ItemImportDevice
        {
            public ItemImportDevice()
            {
                _Name = string.Empty;
                _InstanceID = 0;
                _nestedInstanceID = 0;
                _nextInstanceID = 0;
                _ClassID = 0;
                _Type = string.Empty ;
                _arraydimension = 0;
                ListOfIndexArray = new List<uint>();
                _datatypeofarray = string.Empty;
                _size = 0; 
                _numberofmembers = 0;
                _Parent = -1;
                _indexArray = string.Empty;
            }
            private string _Name;
            public string Name
            {
                get { return _Name; }
                set { _Name = value; }
            }

            private UInt32 _InstanceID;
            public UInt32 InstanceID
            {
                get { return _InstanceID; }
                set { _InstanceID = value; }
            }

            private UInt32 _nestedInstanceID;
            public UInt32 NestedInstanceID
            {
                get { return _nestedInstanceID; }
                set { _nestedInstanceID = value; }
            }

            private UInt32 _nextInstanceID;
            public UInt32 NextInstanceID
            {
                get { return _nextInstanceID; }
                set { _nextInstanceID = value; }
            }

            private ushort _ClassID;
            public ushort ClassID
            {
                get { return _ClassID; }
                set { _ClassID = value; }
            }

            private uint _size;
            public uint Size
            {
                get { return _size; }
                set { _size = value; }
            }

            private string _Type;
            public string Type
            {
                get { return _Type; }
                set { _Type = value; }
            }

            uint _arraydimension; 
            public uint ArrayDimension
            {
                get { return _arraydimension; }
                set { _arraydimension = value; }
            }

            private string _datatypeofarray;
            public string Datatypeofarray
            {
                get { return _datatypeofarray; }
                set { _datatypeofarray = value; }
            }

            /// <summary>
            /// Numemo di membri contenuti nella struttura.
            /// Number of members contained in the structure
            /// </summary>
            private ushort _numberofmembers;
            public ushort NumberofMembers
            {
                get { return _numberofmembers; }
                set { _numberofmembers = value; }
            }

            /// <summary>
            /// Elenco degli indici di un array, indice iniziale e finale es(array[1..2] 1 e 2 sono salvati nella lista)
            /// List of indices of an array, initial and final index eg (array [1..2] 1 and 2 are saved in the list)
            /// </summary>
            public List<uint> ListOfIndexArray;

            private int _Parent;
            public int Parent
            {
                get { return _Parent; }
                set { _Parent = value; }
            }

            private string _indexArray;
            public string indexArray
            {
                get { return _indexArray; }
                set { _indexArray = value; }
            }

        }
        public class RequestOfDevice
        {
            public RequestOfDevice()
            {
                _request = CommandRequestOfDevice.GetTheTypeOfDevice;
                _NumberOfItemsToRequest = 0;
                _devivetype = string.Empty;
                _NumberOfItems = 0;
                _vendorID = 0;
                _productName = string.Empty;
                _numVariableObject = 0;
                _nextVariableTypeObject = 0;
                _error = string.Empty;
                _cyclesNestedInstanceID = 0;
                  

            }

            private string _devivetype;
            public string DeviceType
            {
                get { return _devivetype; }
                set { _devivetype = value; }
            }

            private CommandRequestOfDevice _request;
            public CommandRequestOfDevice Request
            {
                get { return _request; }
                set { _request = value; }
            }

            private uint _NumberOfItemsToRequest;
            public uint NumberOfItemsToRequest
            {
                get { return _NumberOfItemsToRequest; }
                set { _NumberOfItemsToRequest = value; }
            }

            private uint _NumberOfItems;
            public uint NumberOfItems
            {
                get { return _NumberOfItems; }
                set { _NumberOfItems = value; }
            }

            private ushort _vendorID;
            public ushort VendorID
            {
                get { return _vendorID; }
                set { _vendorID = value; }
            }

            private string _productName;
            public string ProductName
            {
                get { return _productName; }
                set { _productName = value; }
            }

            private uint _numVariableObject;
            public uint NumVariableObject
            {
                get { return _numVariableObject; }
                set { _numVariableObject = value; }
            }

            private uint _nextVariableTypeObject;
            public uint NextVariableTypeObject
            {
                get { return _nextVariableTypeObject; }
                set { _nextVariableTypeObject = value; }
            }

            private string _error;
            public string Error
            {
                get { return _error; }
                set { _error = value; }
            }

            private uint _cyclesNestedInstanceID;
            public uint CycleNestedInstanceID
            {
                get { return _cyclesNestedInstanceID; }
                set { _cyclesNestedInstanceID = value; }
            }

            public Dictionary<uint,ItemImportDevice> ListItemsImportDevice = new Dictionary<uint, ItemImportDevice>();
            public List<uint> ListNestedInstanceID = new List<uint>();
            public Dictionary<uint, ItemImportDevice> DictionaryNestedInstanceID = new Dictionary<uint, ItemImportDevice>();
        }

        #region Constructors

        /// <summary>
        /// Initializes the OmronEthernetIPChannel object.
        /// </summary>
        public OmronEthernetIPChannel(CommunicationDriver commdriver, OmronEthernetIPChannelSettings settings)
            : base(commdriver, settings)
        {
            SessionHandle = new uintUnion(0);
        }

        #endregion

        #region Members

        public byte[] requestBuffer = new byte[OmronEthernetIPProtocol.TCP_MAX_SEGMENT_SIZE];
        public ushortUnion requestBufferPointer = new ushortUnion(0);
        private bool _DeviceOpenLogged = false;
        public uintUnion SessionHandle;
        byte[] fragmentedRequestBuffer = new byte[OmronEthernetIPProtocol.TCP_MAX_SEGMENT_SIZE];

        #endregion

        #region Override Methods

        public override bool TestChannelComm()
        {
            bool retValue = DeviceOpen();
            if (retValue)
            {
                DeviceClose();
            }
            return retValue;
        }

        public override void SubscribeJob(CommJob job, CommJobState state)
        {
            ((OmronEthernetIPCommJob)job).ResetOmronEthernetIPCommJob();
            base.SubscribeJob(job, state);
        }

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            ((OmronEthernetIPCommJob)e.Job).ResetOmronEthernetIPCommJob();
            base.OnJobExecuted(e);
        }

        public bool SessionRegisterOk()
        {
            return SessionHandle.UINT != 0;
        }

        public void ClearSession()
        {
            SessionHandle.UINT = 0;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a device is open. </summary>
        ///
        /// <returns>   true if a device is open, false if not. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OmronEthernetIPIsDeviceOpen()
        {
            if (!IsDeviceOpen())
                return false;

            if (!SessionRegisterOk())
                return false;

            return true;
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Queries if a given device open. </summary>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public bool OmronEthernetIPDeviceOpen()
        {
            if (!IsDeviceOpen())
                if (!DeviceOpen())
                    return false;

            if (!OmronEthernetIPProtocol.RegisterSession(ref requestBuffer, ref requestBufferPointer, this))
            {
                //Error connection
                CommDriver.OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorRegisterSession, Opc.Ua.EventSeverity.High);
                OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                return false;
            }

            return true;
        }
        public bool OmronEthernetIGetListServices()
        {
            if (!IsDeviceOpen())
                if (!DeviceOpen())
                    return false;

            if (!OmronEthernetIPProtocol.GetListServices(ref requestBuffer, ref requestBufferPointer, this))
            {
                //Error connection
                CommDriver.OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorRegisterSession, Opc.Ua.EventSeverity.High);
                OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                return false;
            }

            return true;
        }

        public override DriverErrorCodes CheckDevice(List<CommJob> exjoblist, object thischannel)
        {
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            if (!OmronEthernetIPIsDeviceOpen())
            {
                if (!OmronEthernetIPDeviceOpen())
                {
                    conn = DriverErrorCodes.ErrorDeviceOpenFailed;
                }
                else
                {
                    if (!OmronEthernetIPForwardOpen(exjoblist))
                        conn = DriverErrorCodes.ErrorDeviceOpenFailed;
                    else
                    {
                        // Check if the fragmented reading can be used (a specific flag is set on the station of the jobs) 
                        CheckIfVariableMonitoringObjectIsSupported(exjoblist);
                    }
                }
            }
            return conn;
        }

        private void CheckIfVariableMonitoringObjectIsSupported(List<CommJob> joblist)
        {
            if ((joblist == null) || (joblist.Count < 1))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckIfVariableMonitoringObjectIsSupported - Invalid job list",
                                                       currentTime));
                }
#endif
                return;
            }

            OmronEthernetIPStation s = joblist[0].Station as OmronEthernetIPStation;
            if (s == null)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckIfVariableMonitoringObjectIsSupported - Invalid job station",
                                                       currentTime));
                }
#endif
                return;
            }

            s.FragmentedReadingIsSupported = FragmentedProtocolSupported.NotYetVerified;

            // Get the Vendor ID and the Product Name of the device
            ushort vendorID = 0;
            string productName = String.Empty;
            requestBufferPointer.USHORT = 0;
            if (!OmronEthernetIPProtocol.GetDeviceIdentity(ref requestBuffer, ref requestBufferPointer, this, s, ref vendorID, ref productName))
            {
                s.FragmentedReadingIsSupported = FragmentedProtocolSupported.UnSupported;
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckIfVariableMonitoringObjectIsSupported - Error in GetDeviceIdentity",
                                                       currentTime));
                }
#endif
                return;
            }

#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckIfVariableMonitoringObjectIsSupported - vendor ID: {1} - Product name: {2}",
                                                   currentTime, vendorID, productName));
            }
#endif

            // Check the Vendor ID and the Product Name of the device
            if (!OmronEthernetIPProtocol.CheckDeviceIdentity(vendorID, productName))
            {
                s.FragmentedReadingIsSupported = FragmentedProtocolSupported.UnSupported;
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckIfVariableMonitoringObjectIsSupported - CheckDeviceIdentity failed",
                                                       currentTime));
                }
#endif
                return;
            }

            // Check if the Variable Monitoring Object is supported
            requestBufferPointer.USHORT = 0;
            if (!OmronEthernetIPProtocol.GetVariableMonitoringObjectAttributes(ref requestBuffer, ref requestBufferPointer, this, s))
            {
                s.FragmentedReadingIsSupported = FragmentedProtocolSupported.UnSupported;
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckIfVariableMonitoringObjectIsSupported - Error in GetDeviceIdentity",
                                                       currentTime));
                }
#endif
                return;
            }

            s.FragmentedReadingIsSupported = FragmentedProtocolSupported.Supported;

#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckIfVariableMonitoringObjectIsSupported - Station {1} supports Fragment Protocol",
                                                   currentTime, s.Name));
            }
#endif

            return;
        }

        public bool OmronEthernetIPForwardOpen(List<CommJob> list)
        {
            OmronEthernetIPStation s = list[0].Station as OmronEthernetIPStation;
            if (s == null)
                return false;

            if ((s.PlcType == PlcTypes.NJ) || (s.PlcType == PlcTypes.NX))
            {
                bool bRetryTheConnection = false;
                if (!OmronEthernetIPProtocol.Logix5550LargeForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this, ref bRetryTheConnection))
                {
                    if (bRetryTheConnection)
                    {
                        if (s.PlcType == PlcTypes.NX)
                        {
                            s.PlcType = PlcTypes.NJ;
                            s.MaxPduSize = OmronEthernetIPProtocol.GetMaxPduSize(s.PlcType); //1994;
                            if (!OmronEthernetIPProtocol.Logix5550LargeForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this, ref bRetryTheConnection))
                            {
                                //Check invalid connection size (0x0109)
                                if (bRetryTheConnection)
                                {
                                    s.PlcType = PlcTypes.Other_PLC;
                                    s.MaxPduSize = OmronEthernetIPProtocol.GetMaxPduSize(s.PlcType);// 502;
                                    if (!OmronEthernetIPProtocol.Logix5550ForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this))
                                    {
                                        //Error connection
                                        CommDriver.OnSystemEvent(ObjectIds.Server,
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorForwardOpen_Code, requestBuffer[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS]),
                                                        Opc.Ua.EventSeverity.High);
                                        //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                                        return false;
                                    }
                                }
                                else
                                {
                                    //Error connection
                                    //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            s.PlcType = PlcTypes.Other_PLC;
                            s.MaxPduSize = OmronEthernetIPProtocol.GetMaxPduSize(s.PlcType); //502;
                            if (!OmronEthernetIPProtocol.Logix5550ForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this))
                            {
                                //Error connection
                                CommDriver.OnSystemEvent(ObjectIds.Server,
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorForwardOpen_Code, requestBuffer[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS]),
                                                        Opc.Ua.EventSeverity.High);
                                //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //Error connection
                        //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                        return false;
                    }
                }

                // only on first connection good 
                if (!_DeviceOpenLogged)
                {
                    _DeviceOpenLogged = true;
                    // trace on db parameters used by driver to open connection with PLC
                    CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(OmronEthernetIP.Properties.Resources.DeviceOpenUsedParameters, (s.MaxPduSize == OmronEthernetIPProtocol.GetMaxPduSize(PlcTypes.Other_PLC) ? "Forwad Open" : "Large Forward Open"), s.MaxPduSize), Opc.Ua.EventSeverity.Min);
                }
            }
            else
            {
                if (!OmronEthernetIPProtocol.Logix5550ForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this))
                {
                    //Error connection
                    CommDriver.OnSystemEvent(ObjectIds.Server,
                                            string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                            string.Format(OmronEthernetIP.Properties.Resources.ErrorForwardOpen_Code, requestBuffer[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS]),
                                            Opc.Ua.EventSeverity.High);
                    //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                    return false;
                }
            }

            return true;
        }
        public bool OmronEthernetIPForwardOpen(OmronEthernetIPStation s)
        {
            if (s == null)
                return false;

            if ((s.PlcType == PlcTypes.NJ) || (s.PlcType == PlcTypes.NX))
            {
                bool bRetryTheConnection = false;
                if (!OmronEthernetIPProtocol.Logix5550LargeForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this, ref bRetryTheConnection))
                {
                    if (bRetryTheConnection)
                    {
                        if (s.PlcType == PlcTypes.NX)
                        {
                            s.PlcType = PlcTypes.NJ;
                            s.MaxPduSize = OmronEthernetIPProtocol.GetMaxPduSize(s.PlcType); //1994;
                            if (!OmronEthernetIPProtocol.Logix5550LargeForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this, ref bRetryTheConnection))
                            {
                                //Check invalid connection size (0x0109)
                                if (bRetryTheConnection)
                                {
                                    s.PlcType = PlcTypes.Other_PLC;
                                    s.MaxPduSize = OmronEthernetIPProtocol.GetMaxPduSize(s.PlcType);// 502;
                                    if (!OmronEthernetIPProtocol.Logix5550ForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this))
                                    {
                                        //Error connection
                                        CommDriver.OnSystemEvent(ObjectIds.Server,
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorForwardOpen_Code, requestBuffer[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS]),
                                                        Opc.Ua.EventSeverity.High);
                                        //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                                        return false;
                                    }
                                }
                                else
                                {
                                    //Error connection
                                    //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                                    return false;
                                }
                            }
                        }
                        else
                        {
                            s.PlcType = PlcTypes.Other_PLC;
                            s.MaxPduSize = OmronEthernetIPProtocol.GetMaxPduSize(s.PlcType); //502;
                            if (!OmronEthernetIPProtocol.Logix5550ForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this))
                            {
                                //Error connection
                                CommDriver.OnSystemEvent(ObjectIds.Server,
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorForwardOpen_Code, requestBuffer[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS]),
                                                        Opc.Ua.EventSeverity.High);
                                //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                                return false;
                            }
                        }
                    }
                    else
                    {
                        //Error connection
                        //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                        return false;
                    }
                }

                // only on first connection good 
                if (!_DeviceOpenLogged)
                {
                    _DeviceOpenLogged = true;
                    // trace on db parameters used by driver to open connection with PLC
                    CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(OmronEthernetIP.Properties.Resources.DeviceOpenUsedParameters, (s.MaxPduSize == OmronEthernetIPProtocol.GetMaxPduSize(PlcTypes.Other_PLC) ? "Forwad Open" : "Large Forward Open"), s.MaxPduSize), Opc.Ua.EventSeverity.Min);
                }
            }
            else
            {
                if (!OmronEthernetIPProtocol.Logix5550ForwardOpen(s, ref requestBuffer, ref requestBufferPointer, this))
                {
                    //Error connection
                    CommDriver.OnSystemEvent(ObjectIds.Server,
                                            string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                            string.Format(OmronEthernetIP.Properties.Resources.ErrorForwardOpen_Code, requestBuffer[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS]),
                                            Opc.Ua.EventSeverity.High);
                    //OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                    return false;
                }
            }

            return true;
        }

        public bool OmronEthernetIPRequestOnTheDevice(OmronEthernetIPStation s, ref RequestOfDevice request)
        {
            bool success = false;
  
            if (s == null)
                return (success);

            // Get the Vendor ID and the Product Name of the device            
            string productName = String.Empty;
            requestBufferPointer.USHORT = 0;
            if (!OmronEthernetIPProtocol.RequestOnTheDevive(ref requestBuffer, ref requestBufferPointer, this, s, ref request))
            {
                s.FragmentedReadingIsSupported = FragmentedProtocolSupported.UnSupported;
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckIfVariableMonitoringObjectIsSupported - Error in GetDeviceIdentity",
                                                       currentTime));
                }
#endif
            }
            else
            {
                success = true;
            }
            return (success);
        }

        public void InitializeNewDataEvent( )
        {
            lock (lockThreadObject)
            {
                if (NewDataToAnlyze == null)
                    NewDataToAnlyze = new ManualResetEvent(false);
                NewDataToAnlyze.Reset();
            }
        }


        public byte[] GetReceiveBuffer()
        {
            List<byte> receiveBuffer = new List<byte>();
            lock (lockThreadObject)
            {
                receiveBuffer.AddRange(ReceiveBuffer);
                ReceiveBuffer.Clear();
            }

            return receiveBuffer.ToArray();
        }

        #endregion

        #region methods     

        public override bool IsScheduledJobsListFull(List<CommJob> jobList)
        {
            return false;
        }

        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {

#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - SplitInExecutionLists - jobList.Count: {1}",
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
                bool fragmentProtocol = false;
                string station = string.Empty;
                List<CommJob> dequeueListJob = new List<CommJob>();
                OmronEthernetIPProtocol.ReadWriteListLimitateSize frameSize = new OmronEthernetIPProtocol.ReadWriteListLimitateSize();

                #region fill a list with job as long as size not exceed mamimux frame size
                while (jobIndex < jobList.Count())
                {
                    OmronEthernetIPCommJob j = jobList.ElementAt(jobIndex) as OmronEthernetIPCommJob;
                    OmronEthernetIPStation s = j.Station as OmronEthernetIPStation;

                    if (first)
                    {
                        write = j.WriteRequest();
                        j.Init(write);
                        station = j.Station.Name;
                        fragmentProtocol = FragmentProtocolCanBeApplied(j);
                        first = false;

#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - SplitInExecutionLists - first == true - job: {1} - write: {2} - fragmentProtocol: {3}",
                                                               currentTime, j.ABAddress, write, fragmentProtocol));
                        }
#endif
                    }

                    bool jwrite = j.WriteRequest();
                    j.Init(jwrite);
                    bool jfragmentProtocol = FragmentProtocolCanBeApplied(j);

#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - SplitInExecutionLists - job: {1} - jwrite: {2} - jfragmentProtocol: {3} - write: {4} - fragmentProtocol: {5}",
                                                           currentTime, j.ABAddress, jwrite, jfragmentProtocol, write, fragmentProtocol));
                    }
#endif

                    if (j.Station.Name == station)
                    {
                        // write
                        if (write && jwrite)
                        {
                            if (OmronEthernetIPProtocol.getWriteListLimitate(ref j, ref frameSize, OmronEthernetIPProtocol.ReadWriteLimitate.EvaluateOnly))
                            {
                                frameSize.TotalNrJobs++;
                                dequeueListJob.Add(j);
                                jobList.RemoveAt(jobIndex);
                                jobIndex--;

#if DEBUG
                                {
                                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - SplitInExecutionLists - write && jwrite adding job: {1} - dequeueListJob.Count: {2} - jobIndex: {3}",
                                                                       currentTime, j.ABAddress, dequeueListJob.Count, jobIndex));
                                }
#endif
                            }
                            else
                            {
                                // job exceed maximum frame size
                                break;
                            }
                        }
                        else
                        {
                            // read
                            if (!write && !jwrite)
                            {
                                if(fragmentProtocol == jfragmentProtocol)
                                {
                                    if (OmronEthernetIPProtocol.getReadListLimitate(ref j, ref frameSize, OmronEthernetIPProtocol.ReadWriteLimitate.EvaluateOnly))
                                    {
                                        System.Diagnostics.Debug.WriteLine(String.Format("DEBUG OMRON - {0} - Adding {1} - To a list with {2} elements", DateTime.Now.ToString("HH:mm:ss.fff"), j.ABAddress, dequeueListJob.Count));
                                        frameSize.TotalNrJobs++;
                                        dequeueListJob.Add(j);
                                        jobList.RemoveAt(jobIndex);
                                        jobIndex--;

#if DEBUG
                                        {
                                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                            System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - SplitInExecutionLists - !write && !jwrite - adding job: {1} - dequeueListJob.Count: {2} - jobIndex: {3} - fragmentProtocol: {4} - jfragmentProtocol: {5}",
                                                                               currentTime, j.ABAddress, dequeueListJob.Count, jobIndex, fragmentProtocol, jfragmentProtocol));
                                        }
#endif
                                        if (s.FragmentedReadingIsSupported == FragmentedProtocolSupported.NotYetVerified)
                                        {
                                            // If the support of the fragmented protocol has not been yet verified for the station, do not aggregate aggregate more jobs in the same list
                                            break;
                                        }
                                    }
                                    else
                                    {
                                        // job exceed maximum frame size
                                        break;
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // station changed; 
                        break;
                    }

                    jobIndex++;

                }

#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - SplitInExecutionLists - frameSize.TotalNrJobs: {1} - frameSize.TotalSize: {2} - frameSize.ResponseSize: {3}",
                                                       currentTime, frameSize.TotalNrJobs, frameSize.TotalSize, frameSize.ResponseSize));
                }
#endif

                if (dequeueListJob.Count > 0)
                {
                    exList.Add(dequeueListJob);
                    jobIndex = 0;
                }
                #endregion
            }
        }

        protected override bool SetSynchroJobData(CommJob exjob, NodeId tagNodeId = null, object value = null)
        {
            bool bRet = base.SetSynchroJobData(exjob, tagNodeId, value);
            if (bRet)
            {
                exjob.UpdateTagsListOnWriting();
                List<CommJob> wList = new List<CommJob>() { exjob };
                // if was shceduled before, remove from SchedulingQueue
                if (wList[0].IsQueued)
                    RemoveScheduledJobs(exjob.Station, wList);
                List<List<CommJob>> exList = new List<List<CommJob>>();
                // use SplitInExecutionLists method to initialize internal's read/write parameters
                SplitInExecutionLists(wList, ref exList);
            }
            return bRet;
        }

        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> exjoblist)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                // back compatibilty with old driver
                conn = DriverErrorCodes.ErrorTimeOut;
                return false;
            }

            #region special case
            if ((exjoblist.Count == 1) && !FragmentProtocolCanBeApplied(exjoblist))
            {
                OmronEthernetIPCommJob j = exjoblist[0] as OmronEthernetIPCommJob;
                // manage big job (big array)
                OmronEthernetIPProtocol.ReadWriteListLimitateSize requestSize = new OmronEthernetIPProtocol.ReadWriteListLimitateSize();
                if (j.CommandType == CommandTypes.ReadCmd || j.CommandType == CommandTypes.ReadDataFormatCmd)
                    OmronEthernetIPProtocol.getReadListLimitate(ref j, ref requestSize, OmronEthernetIPProtocol.ReadWriteLimitate.Add);
                else
                    OmronEthernetIPProtocol.getWriteListLimitate(ref j, ref requestSize, OmronEthernetIPProtocol.ReadWriteLimitate.Add);
            }
            #endregion

            ExecuteJobList(exjoblist);

            return (exjoblist.Count > 0);
        }

        public bool FragmentProtocolCanBeApplied(OmronEthernetIPCommJob j)
        {
            if ((j.CommandType == CommandTypes.WriteCmd) || (j.CommandType == CommandTypes.ReadDataFormatCmd))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - FragmentProtocolCanBeApplied - Unsupported CommandType: {1} --> return false",
                                                       currentTime, j.CommandType));
                }
#endif
                return (false);
            }

            OmronEthernetIPStation s = j.Station as OmronEthernetIPStation;
            if (s == null)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - FragmentProtocolCanBeApplied - Invalid station --> return false",
                                                       currentTime));
                }
#endif
                return (false);
            }

            if(s.FragmentedReadingIsSupported == FragmentedProtocolSupported.UnSupported)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - FragmentProtocolCanBeApplied - Fragmented Reading unsupported --> return false",
                                                       currentTime));
                }
#endif
                return (false);
            }

            // Compatibility problem: the fragment protocol can't be used for reading the values of an array if its address contains the index of the first element to be read.
            if (j.ArrayIsRequestedWithItemIndex())
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - FragmentProtocolCanBeApplied - Array Requested With Item Index --> return false",
                                                           currentTime));
                }
#endif
                return (false);
            }

            if (j.IsTooBigForSingleFragment())
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - FragmentProtocolCanBeApplied - Big Array --> return false",
                                                           currentTime));
                }
#endif
                return (false);
            }

            return (true);
        }

        // Check if the fragmented protocol can be applied
        private bool FragmentProtocolCanBeApplied(List<CommJob> list)
        {
            if ((list == null) || (list.Count < 1))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - FragmentProtocolCanBeApplied(list) - Null or empty job list --> return false",
                                                       currentTime));
                }
#endif
                return (false);
            }

            OmronEthernetIPCommJob j = list[0] as OmronEthernetIPCommJob;
            return (FragmentProtocolCanBeApplied(j));
        }

        private void ExecuteJobList(List<CommJob> list)
        {
            DateTime ExecutionTime = DateTime.UtcNow;
            Parallel.ForEach(list, j =>
            {
                base.ExecuteJob(j);
            });

            OmronEthernetIPStation s = ((OmronEthernetIPCommJob)list[0]).Station as OmronEthernetIPStation;
            if (s == null)
            {
                return;
            }

            if (FragmentProtocolCanBeApplied(list))
            {
                // Read using the Fragment Protocol
                ExecuteFragmentedReading(list);
                return;
            }

            if (OmronEthernetIPProtocol.Logix5550PrepareRequest(ref list, this, ref s, ref requestBuffer, ref requestBufferPointer) == 0)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(String.Format("{0} - Failure in Logix5550PrepareRequest num. of jobs: {1}", DateTime.UtcNow.ToString(), list.Count.ToString("00000")));
#endif
                foreach (OmronEthernetIPCommJob j in list)
                    RemovePendingJob(j);
                list.Clear();

                return;
            }
            if (!OmronEthernetIPDeviceWrite(ref requestBuffer, ref requestBufferPointer))
            {
                OmronEthernetIPProtocol.Logix5550ForwardClose(s, ref requestBuffer, ref requestBufferPointer, this);
                OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                return;
            }

            BeginDeviceRead(OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE);
        }

        private void ExecuteFragmentedReading(List<CommJob> list)
        {
            OmronEthernetIPStation s = ((OmronEthernetIPCommJob)list[0]).Station as OmronEthernetIPStation;
            if (s == null)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ExecuteFragmentedReading - Invalid station",
                                                       currentTime));
                }
#endif
                return;
            }

            // Get the whole list of the variable names (the list to be fragmented)
            ushort totalVariableNumber = 0;
            ushort completeNameListLength = OmronEthernetIPProtocol.GetCompleteVariableListForFragmentRequest(ref list, ref fragmentedRequestBuffer, ref totalVariableNumber);
            if ((completeNameListLength == 0) || totalVariableNumber == 0)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ExecuteFragmentedReading - Empty variable list",
                                                       currentTime));
                }
#endif
                return;
            }

            // Get the maximum length for a single fragment
            ushort fragmentMaxLength = (ushort)(s.MaxPduSize - OmronEthernetIPProtocol.FRAGMENT_REQUEST_FIX_OVERHEAD - OmronEthernetIPProtocol.CIP_SEQUENCE_COUNT_SIZE);

            // Send the read request using the fragmented protocol
            OmronEthernetIPFragmentInfo fragmentInfo = new OmronEthernetIPFragmentInfo();
            fragmentInfo.SetBufferToBeFragmented(fragmentedRequestBuffer, completeNameListLength);
            fragmentInfo.TotalNumberOfVariables = totalVariableNumber;
            fragmentInfo.MaxLengthOfASingleFragment = fragmentMaxLength;

            while (fragmentInfo.FType != FragmentType.Last)
            {
                // Prepare a fragment of the request
                requestBufferPointer.USHORT = 0;
                ushort fragmentLength = OmronEthernetIPProtocol.PrepareConnectedFragmentedReadRequest(this, s, ref requestBuffer, ref requestBufferPointer, ref fragmentInfo);
                if (fragmentLength == 0)
                {
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ExecuteFragmentedReading - Error in PrepareFragmentedReadRequest",
                                                           currentTime));
                    }
#endif
                    foreach (OmronEthernetIPCommJob j in list)
                    {
                        RemovePendingJob(j);
                    }
                    list.Clear();

                    return;
                }

                // Send the request fragment
                if (!OmronEthernetIPDeviceWrite(ref requestBuffer, ref requestBufferPointer))
                {
                    OmronEthernetIPProtocol.Logix5550ForwardClose(s, ref requestBuffer, ref requestBufferPointer, this);
                    OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                    return;
                }

                // If the sent fragment is not the last one, wait for the PLC reply
                switch (fragmentInfo.FType)
                {
                    // First fragment: the reply of the PLC contains an handle to be used for sending the request of following fragments 
                    case FragmentType.First:
                    case FragmentType.Middle:
                        if (!GetFragmentReply(ref fragmentInfo, ref s))
                        {
#if DEBUG
                            {
                                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                                System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ExecuteFragmentedReading - Error in GetFragmentReply",
                                                                   currentTime));
                            }
#endif
                            foreach (OmronEthernetIPCommJob j in list)
                            {
                                RemovePendingJob(j);
                            }
                            list.Clear();
                            ClearSession();

                            return;
                        }
                        break;

                    // Last fragment of the request sent, don't wait for the PLC reply
                    default:
                        BeginDeviceRead(OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE);
                        break;
                }
            }
        }

        private bool GetFragmentReply(ref OmronEthernetIPFragmentInfo fragmentInfo, ref OmronEthernetIPStation s)
        {
            // Do not wait for the PLC reply to the last fragment of the request
            if (fragmentInfo.FType == FragmentType.Last)
            {
                return (true);
            }

            // Read the Ethernet/IP header of the message
            if (!DeviceRead(requestBuffer, OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetFragmentReply - Error reading the encapsulation header",
                                                       currentTime));
                }
#endif

                ReceiveClear();
                return (false);
            }

            // Check the length of the remaining part of the message
            ushortUnion lenData = new ushortUnion(requestBuffer, OmronEthernetIPProtocol.EDATA_LEN_OFFS);
            ushort minimumExpectedLength = OmronEthernetIPProtocol.REPLY_CIP_SERVICE_OFFSET + 4;
            if (lenData.USHORT < minimumExpectedLength)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetFragmentReply - Reply too short",
                                                       currentTime));
                }
#endif

                ReceiveClear();
                return (false);
            }

            byte[] readBuffer = new byte[lenData.USHORT];

            // Read the remaining part of the reply
            if (!DeviceRead(readBuffer, (uint)lenData.USHORT)) //error timeout RX
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetFragmentReply - Error reading the remaining part of the reply",
                                                       currentTime));
                }
#endif

                ReceiveClear();
                return (false);
            }
            readBuffer.CopyTo(requestBuffer, OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE);

            // Check the reply
            if (!CheckReplyToConnectedService0x50(ref requestBuffer, (ushort)(lenData.USHORT + OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE), ref fragmentInfo, ref s))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetFragmentReply - Error in CheckReplyToService0x50",
                                                       currentTime));
                }
#endif

                ReceiveClear();
                return (false);
            }

            ReceiveClear();
            return (true);
        }

        private bool CheckReplyToConnectedService0x50(ref byte[] pdu, ushort pduLength, ref OmronEthernetIPFragmentInfo fragmentInfo, ref OmronEthernetIPStation s)
        {
            // Check the CIP sequence count
            ushortUnion expectedCipSequenceCount = new ushortUnion(s.GetTransaction().USHORT);
            ushortUnion receivedCipSequenceCount = new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_TNS_OFFS);
            if (expectedCipSequenceCount.USHORT != receivedCipSequenceCount.USHORT)
            {
                s.LastTransOK = false;
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckReplyToConnectedService0x50 - Wrong CIP sequence count: expected = {1}, received = {2}",
                                                       currentTime, expectedCipSequenceCount.USHORT, receivedCipSequenceCount.USHORT));
                }
#endif
                return (false);
            }

            // Expected service ID: 0xd0 == reply to service "Get Variables Multiples" (0x50)
            ushort pduOffset = OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE + OmronEthernetIPProtocol.REPLY_CIP_SERVICE_OFFSET;
            int omronErrorCode = CheckServiceIdAndErrorCodeResponse(ref pdu, pduOffset, 0xd0);
            if (omronErrorCode != (int)DriverErrorCodes.ErrorNoError)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckReplyToConnectedService0x50 - Error returned by CheckServiceIdAndErrorCodeResponse: {1}",
                                                       currentTime, omronErrorCode));
                }
#endif
                return (false);
            }

            // Check the message length
            ushort minimumLength = OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE + OmronEthernetIPProtocol.REPLY_CIP_SERVICE_OFFSET + 6;
            if (fragmentInfo.FType == FragmentType.First)
            {
                // The reply to the first fragment of the request contains two more bytes: the handle number
                minimumLength += 2;
            }
            if (pduLength < minimumLength)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckReplyToConnectedService0x50 - Uncomplete reply - PDU Length {1} - Fragment Type: {2}",
                                                       currentTime, pduLength, fragmentInfo.FType));
                }
#endif
                return (false);
            }

            // Check the sequential ID
            pduOffset += 4;
            ushortUnion seqID = new ushortUnion(pdu, pduOffset);
            if (!fragmentInfo.CheckReplySequenceID(seqID.USHORT))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckReplyToConnectedService0x50 - Error in CheckReplySequenceID",
                                                       currentTime));
                }
#endif
                return (false);
            }

            // If it is the reply to the first fragment of the request, parse the handle
            if (fragmentInfo.FType == FragmentType.First)
            {
                pduOffset += 2;
                ushortUnion handle = new ushortUnion(pdu, pduOffset);
                fragmentInfo.Handle = handle.USHORT;
            }

            // If it is the reply to the first fragment of the request, update the type of the next request fragment
            if (fragmentInfo.FType == FragmentType.First)
            {
                fragmentInfo.FType = FragmentType.Middle;
            }

            // Update the sequence number of the next fragment request
            if (fragmentInfo.SequenceNumber < OmronEthernetIPProtocol.FRAGMENT_MAX_SEQUENCE_NUMBER)
            {
                fragmentInfo.SequenceNumber++;
            }
            else
            {
                fragmentInfo.SequenceNumber = 0;
            }

            s.LastTransOK = true;

            return (true);
        }

        private int CheckServiceIdAndErrorCodeResponse(ref byte[] pdu, ushort replyOffset, byte serviceID)
        {
            // Check the service ID of the reply
            if (pdu.Count() < (replyOffset + 1))
            {
                return ((int)OmronEthernetIPErrorCodes.ErrorRepTooShort);
            }
            if (pdu[replyOffset] != serviceID)
            {
                return ((int)OmronEthernetIPErrorCodes.ErrorRepSerCode);
            }

            // Check the status (error code) of the reply
            if (pdu.Count() < (replyOffset + 3))
            {
                return ((int)OmronEthernetIPErrorCodes.ErrorRepTooShort);
            }
            byte replyStatus = pdu[replyOffset + 2];
            if (replyStatus == 0)
            {
                return ((int)DriverErrorCodes.ErrorNoError);
            }

            // Set the error code corresponding to the error returned in the reply 
            int omronErrorCode = (int)DriverErrorCodes.ErrorNoError;
            switch (replyStatus)
            {
                case 0x02:
                    omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus02;
                    break;

                case 0x04:
                    omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus04;
                    break;

                case 0x05:
                    omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus05;
                    break;

                case 0x0C:
                    omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus0C;
                    break;

                case 0x11:
                    omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus11;
                    break;

                case 0x13:
                    omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus13;
                    break;

                case 0x15:
                    omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus15;
                    break;

                case 0x1F:
                    omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus1F;
                    break;

                case 0x20:
                    omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus20;
                    break;

                default:
                    omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus;
                    break;
            }

            if ((omronErrorCode == (int)OmronEthernetIPErrorCodes.ErrorCipStatus) || (omronErrorCode == (int)OmronEthernetIPErrorCodes.ErrorCipStatus20))
            {
                return (omronErrorCode);
            }

            // Get additional information on the error
            if (pdu.Count() < (replyOffset + 4))
            {
                return ((int)OmronEthernetIPErrorCodes.ErrorRepTooShort);
            }

            byte addStatusSize = pdu[replyOffset + 3];
            if (addStatusSize > 0)
            {
                if (pdu.Count() < (replyOffset + 6))
                {
                    return ((int)OmronEthernetIPErrorCodes.ErrorRepTooShort);
                }
                ushortUnion addInfo = new ushortUnion(pdu, (ushort)(replyOffset + 4));
                switch (addInfo.USHORT)
                {
                    case 0x102:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus102;
                        break;
                    case 0x104:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus104;
                        break;
                    case 0x1103:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus1103;
                        break;
                    case 0x2103:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus2103;
                        break;
                    case 0x2104:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus2104;
                        break;
                    case 0x8001:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8001;
                        break;
                    case 0x8007:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8007;
                        break;
                    case 0x8009:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8009;
                        break;
                    case 0x800F:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus800F;
                        break;
                    case 0x8010:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8010;
                        break;
                    case 0x8011:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8011;
                        break;
                    case 0x8017:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8017;
                        break;
                    case 0x8018:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8018;
                        break;
                    case 0x8021:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8021;
                        break;
                    case 0x8022:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8022;
                        break;
                    case 0x8023:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8023;
                        break;
                    case 0x8024:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8024;
                        break;
                    case 0x8025:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8025;
                        break;
                    case 0x8028:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8028;
                        break;
                    case 0x8029:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8029;
                        break;
                    case 0x8031:
                        omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8031;
                        break;
                }
            }

            return (omronErrorCode);
        }

        bool AtLeastOneStationIsConnected()
        {
            List<Station> connectedStationList = (from station in CommDriver.GetChannelStations(this).AsParallel()
                                                  where (((OmronEthernetIPStation)station).ConnectionIDSet == true)
                                                  select station).ToList();
            return (connectedStationList.Count() > 0);
        }

        public void InvalidateStationConnections()
        {
            Parallel.ForEach(CommDriver.GetChannelStations(this), s =>
            {
                ((OmronEthernetIPStation)s).ClearConnection();
            });
        }

        private DateTime messageReciveDateTime = DateTime.UtcNow;
        public override bool ProcessNewDataList(DriverErrorCodes conn, List<CommJob> list)
        {
            messageReciveDateTime = DateTime.UtcNow;
            if (conn != (int)DriverErrorCodes.ErrorNoError)
            {
                //try
                //{
                if (conn == DriverErrorCodes.ErrorTimeOut)
                {
                    ((OmronEthernetIPStation)list[0].Station).ClearConnection();
                    if (list[0].Station.LastErrorCode != DriverErrorCodes.ErrorTimeOut)
                    {
                        ClearSession();
                        InvalidateStationConnections();
                    }
                }

                foreach (OmronEthernetIPCommJob j in list)
                {
                    j.LastExecutionTime = DateTime.UtcNow;
                    //j.TerminateTask = false;
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = conn, Job = j, LastExecutionTime = messageReciveDateTime };
                    OnJobExecuted(eJob);
                }

                ReceiveClear();
                
                return true;
            }

            if (list.Count < 1)
            {
                return (true);
            }

            if (FragmentProtocolCanBeApplied(list))
            {
                return (FragmentedProcessNewDataList(list));
            }

            if (list.Count != 0)
            {
                bool bRet = false;
                OmronEthernetIPStation s = ((OmronEthernetIPCommJob)list[0]).Station as OmronEthernetIPStation;
                ushortUnion dim = new ushortUnion();
                byte[] pduData = new byte[0];
                byte[] pdu = new byte[0];
                if (s != null)
                {
                    lock (lockThreadObject)
                    {
                        if (ReceiveBuffer.Count >= OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE)
                        {
                            dim = new ushortUnion(ReceiveBuffer, OmronEthernetIPProtocol.EDATA_LEN_OFFS);
                            if (dim.USHORT != 0)
                            {
                                pdu = new byte[OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE + dim.USHORT];
                                pduData = new byte[dim.USHORT];
                                ReceiveBuffer.CopyTo(pdu, 0);

                                bRet = true;
                            }
                        }

                        ReceiveBuffer.Clear();
                    }
                    if (bRet)
                    {
                        uint numberOfReceivedBytes = 0;
                        uint numberOfBytesToBeRead = (uint)dim.USHORT;
                        uint numberOfReadBytes = 0;
                        byte[] readData = new byte[dim.USHORT];
                        do
                        {
                            numberOfReadBytes = DeviceReadSynchronous(readData, numberOfBytesToBeRead);
                            if (numberOfReadBytes > 0)
                            {
                                Array.Copy(readData, 0, pduData, numberOfReceivedBytes, numberOfReadBytes);
                                numberOfReceivedBytes += numberOfReadBytes;
                                numberOfBytesToBeRead -= numberOfReadBytes;
                            }
                        } while ((numberOfReceivedBytes < (uint)dim.USHORT) && (numberOfReadBytes > 0));

                        bRet = (numberOfReceivedBytes == (uint)dim.USHORT);

                        if (bRet)
                        {
                            pduData.CopyTo(pdu, OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE);

                            if (testTagNameTransaction(ref pdu, list, s))
                            {
                                switch (((OmronEthernetIPCommJob)list[0]).CommandType)
                                {
                                    case CommandTypes.ReadCmd:
                                        GetReadData(ref pdu, list, s);
                                        break;
                                    case CommandTypes.ReadDataFormatCmd:
                                        GetReadDataFormat(ref pdu, list, s);
                                        break;
                                    case CommandTypes.WriteCmd:
                                        CheckTheWritingResponse(ref pdu, list, s);
                                        break;
                                }
                            }
                        }
                    }
                    if (bRet == false)
                    {
                        OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                        foreach (OmronEthernetIPCommJob j in list)
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorNoRep, Job = j, LastExecutionTime = messageReciveDateTime };
                            OnJobExecuted(eJob);
                        }

                        ReceiveClear();
                    }
                }
            }

            return true;
        }

        private bool FragmentedProcessNewDataList(List<CommJob> list)
        {
            messageReciveDateTime = DateTime.UtcNow;

            if (list.Count != 0)
            {
                bool bRet = false;
                OmronEthernetIPStation s = ((OmronEthernetIPCommJob)list[0]).Station as OmronEthernetIPStation;
                ushortUnion dim = new ushortUnion();
                byte[] pduData = new byte[0];
                byte[] pdu = new byte[0];
                if (s != null)
                {
                    lock (lockThreadObject)
                    {
                        // Check if the Ethernet/IP header has been received
                        if (ReceiveBuffer.Count >= OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE)
                        {
                            dim = new ushortUnion(ReceiveBuffer, OmronEthernetIPProtocol.EDATA_LEN_OFFS);
                            if (dim.USHORT != 0)
                            {
                                pdu = new byte[OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE + dim.USHORT];
                                pduData = new byte[dim.USHORT];
                                ReceiveBuffer.CopyTo(pdu, 0);

                                bRet = true;
                            }
                        }

                        ReceiveBuffer.Clear();
                    }
                    if (bRet)
                    {
                        // Read the remaining part of the message
                        uint numberOfReceivedBytes = 0;
                        uint numberOfBytesToBeRead = (uint)dim.USHORT;
                        uint numberOfReadBytes = 0;
                        byte[] readData = new byte[dim.USHORT];
                        do
                        {
                            numberOfReadBytes = DeviceReadSynchronous(readData, numberOfBytesToBeRead);
                            if (numberOfReadBytes > 0)
                            {
                                Array.Copy(readData, 0, pduData, numberOfReceivedBytes, numberOfReadBytes);
                                numberOfReceivedBytes += numberOfReadBytes;
                                numberOfBytesToBeRead -= numberOfReadBytes;
                            }
                        } while ((numberOfReceivedBytes < (uint)dim.USHORT) && (numberOfReadBytes > 0));

                        bRet = (numberOfReceivedBytes == (uint)dim.USHORT);
                    }
                    if (bRet == false)
                    {
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - FragmentedProcessNewDataList - Error reading the reply",
                                                               currentTime));
                        }
#endif
                        OmronEthernetIPProtocol.UnRegisterSession(ref requestBuffer, ref requestBufferPointer, this);
                        foreach (OmronEthernetIPCommJob j in list)
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorNoRep, Job = j, LastExecutionTime = messageReciveDateTime };
                            OnJobExecuted(eJob);
                        }

                        ReceiveClear();

                        return (true);
                    }

                    pduData.CopyTo(pdu, OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE);
                    // Test the CIP sequence count
                    ushortUnion expectedCipSequenceCount = new ushortUnion(s.GetTransaction().USHORT);
                    ushortUnion receivedCipSequenceCount = new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_TNS_OFFS);
                    if (expectedCipSequenceCount.USHORT != receivedCipSequenceCount.USHORT)
                    {
                        s.LastTransOK = false;
                        foreach (OmronEthernetIPCommJob j in list)
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorWrongTransaction, Job = j, LastExecutionTime = messageReciveDateTime };
                            OnJobExecuted(eJob);
                        }
                        return (true);
                    }
                    s.LastTransOK = true;

                    // Check the received reply
                    int omronErrorCode = (int)DriverErrorCodes.ErrorNoError;
                    if (!CheckReplyToService0x50(ref pdu, (ushort)pdu.Count(), ref omronErrorCode))
                    {
                        // Special case: error INVALID_PARAMETER -->
                        // Check which variables are inacessible and set in error only the corresponding jobs              
                        if ((omronErrorCode != (int)OmronEthernetIPErrorCodes.ErrorCipStatus20) || !ManageErrorInvalidParameter(list, ref pdu, (ushort)pdu.Count()))
                        {
                            foreach (OmronEthernetIPCommJob j in list)
                            {
                                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorInvalidReplyFragmentedProtocol, Job = j, LastExecutionTime = messageReciveDateTime };
                                OnJobExecuted(eJob);
                            }

                            ReceiveClear();
                        }

                        return (true);
                    }

                    // Parse the received data
                    GetFragmentedData(ref pdu, list, s);
                }
            }

            return (true);
        }

        // Special case: error INVALID_PARAMETER -->
        // Check which variables are inacessible and set in error only the corresponding jobs              
        private bool ManageErrorInvalidParameter(List<CommJob> list, ref byte[] pdu, ushort pduLength)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - Init - list.Count: {1}",
                                                   currentTime, list.Count()));
            }
#endif
            ushort pduOffset = OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE + OmronEthernetIPProtocol.REPLY_CIP_SERVICE_OFFSET + 2;
            if(pduLength <= pduOffset)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - Error 1 - pduLength: {1} - pduOffset: {2}",
                                                       currentTime, pduLength, pduOffset));
                }
#endif
                return (false);
            }
            
            // Check the error code: it must be 0x20 == INVALID_PARAMETER
            byte replyStatus = pdu[pduOffset];
            if (replyStatus != 0x20)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - Error 2 - replyStatus: {1}",
                                                       currentTime, replyStatus));
                }
#endif
                return (false);
            }

            // Get the size in WORDs of additional information on the error
            if (pduLength <= (pduOffset + 1))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - Error 3 - pduLength: {1} - pduOffset: {2}",
                                                       currentTime, pduLength, pduOffset));
                }
#endif
                return (false);
            }
            byte addStatusSize = pdu[pduOffset + 1];
            if(addStatusSize < 4)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - Error 4 - addStatusSize: {1}",
                                                       currentTime, addStatusSize));
                }
#endif
                return (false);
            }
            if (pduLength <= (pduOffset + 1 + 2 *addStatusSize))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - Error 5 - pduLength: {1} - pduOffset: {2} - addStatusSize: {3}",
                                                       currentTime, pduLength, pduOffset, addStatusSize));
                }
#endif
                return (false);
            }

            // Get the number of inaccessible variables
            ushortUnion numberOfInaccessibleVariables = new ushortUnion(pdu, (ushort)(pduOffset + 6));
            if (pduLength < (pduOffset + 6 + 2 * (numberOfInaccessibleVariables.USHORT + 1)))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - Error 6 - pduLength: {1} - pduOffset: {2} - numberOfInaccessibleVariables.USHORT: {3}",
                                                       currentTime, pduLength, pduOffset, numberOfInaccessibleVariables.USHORT));
                }
#endif
                return (false);
            }
            if((numberOfInaccessibleVariables.USHORT > (ushort)list.Count) || (numberOfInaccessibleVariables.USHORT < 1))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - Error 7 - numberOfInaccessibleVariables.USHORT: {1} - list.Count: {2}",
                                                       currentTime, numberOfInaccessibleVariables.USHORT, list.Count));
                }
#endif
                return (false);
            }

            // Build the list of indexes of the inaccessible variables
            List<ushort> inaccessibleVariableList = new List<ushort>();
            if(!GetListOfInaccessibleVariables(ref pdu, pduLength, (ushort)(pduOffset + 8), (ushort)list.Count, numberOfInaccessibleVariables.USHORT, ref inaccessibleVariableList))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - Error 8 - Error in GetListOfInaccessibleVariables",
                                                       currentTime));
                }
#endif
                return (false);
            }
            if((inaccessibleVariableList.Count() < 1) || (inaccessibleVariableList.Count() != numberOfInaccessibleVariables.USHORT))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - Error 9 - inaccessibleVariableList.Count: {1} - numberOfInaccessibleVariables.USHORT: {2}",
                                                       currentTime, inaccessibleVariableList.Count, numberOfInaccessibleVariables.USHORT));
                }
#endif
                return (false);
            }

            // Set in error the jobs of the inaccessible variables
            foreach (ushort index in inaccessibleVariableList)
            {
                OmronEthernetIPCommJob j = (OmronEthernetIPCommJob)list[index];
                ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorCipStatus20, Job = j, LastExecutionTime = messageReciveDateTime };
                OnJobExecuted(eJob);
            }

            // Remove the jobs of the inaccessible variables from the list of jobs to be executed
            ushort numberOfRemovedJobs = 0;
            foreach (ushort index in inaccessibleVariableList)
            {
                if(index < numberOfRemovedJobs)
                {
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - Error 10 - index: {1} - numberOfRemovedJobs: {2}",
                                                           currentTime, index, numberOfRemovedJobs));
                    }
#endif
                    return (false);
                }
                ushort jobIndex = (ushort)(index - numberOfRemovedJobs);
                if (jobIndex >= list.Count())
                {
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - Error 11 - jobIndex: {1} - list.Count: {2}",
                                                           currentTime, index, list.Count()));
                    }
#endif
                    return (false);
                }
                list.RemoveAt(jobIndex);
                numberOfRemovedJobs++;
            }

            ReceiveClear();

#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - End - list.Count: {1} - numberOfInaccessibleVariables.USHORT: {2} - numberOfRemovedJobs: {3}",
                                                   currentTime, list.Count(), numberOfInaccessibleVariables.USHORT, numberOfRemovedJobs));
                foreach (ushort index in inaccessibleVariableList)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ManageErrorInvalidParameter - Variable {1} is inaccessible",
                                                       currentTime, index + 1));
                }
            }
#endif

            return (true);
        }

        private bool GetListOfInaccessibleVariables(ref byte[] pdu, ushort pduLength, ushort pduOffset, ushort numberOfJobs, ushort numberOfInaccessibleVariables, ref List<ushort>  inaccessibleVariableList)
        {
            for(ushort i=0; i<numberOfInaccessibleVariables; i++)
            {
                ushortUnion variableIndex = new ushortUnion(pdu, (ushort)(pduOffset + i * 2));
                ushort varIndex = variableIndex.USHORT;
                if((varIndex < 1) || (varIndex > numberOfJobs))
                {
                    inaccessibleVariableList.Clear();
                    return (false);
                }
                inaccessibleVariableList.Add((ushort)(varIndex - 1));
            }

            return (true);
        }

        private bool CheckReplyToService0x50(ref byte[] pdu, ushort pduLength, ref int omronErrorCode)
        {
            // Check the PDU length
            ushort minimumLength = OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE + OmronEthernetIPProtocol.REPLY_CIP_SERVICE_OFFSET + 4;
            if (pduLength < minimumLength)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckReplyToService0x50 (2nd version) - Reply too short: {1}",
                                                       currentTime, pduLength));
                }
#endif
                return (false);
            }

            // Expected service ID: 0xd0 == reply to service "Get Variables Multiples" (0x50)
            ushort pduOffset = OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE + OmronEthernetIPProtocol.REPLY_CIP_SERVICE_OFFSET;
            omronErrorCode = CheckServiceIdAndErrorCodeResponse(ref pdu, pduOffset, 0xd0);
            if (omronErrorCode != (int)DriverErrorCodes.ErrorNoError)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckReplyToService0x50 - Error returned by CheckServiceIdAndErrorCodeResponse: {1}",
                                                       currentTime, omronErrorCode));
                }
#endif
                return (false);
            }

            return (true);
        }

        private bool testTagNameTransaction(ref byte[] pdu, List<CommJob> list, OmronEthernetIPStation s)
        {
            if (s.GetTransaction().USHORT == new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_TNS_OFFS).USHORT)
            {
                // when return false, all jobs are already set in error with OnJobExecuted(...);
                s.LastTransOK = Logix5550TagsCheckReply(ref pdu, s, list);
            }
            else
            {
                s.LastTransOK = false;
                foreach (OmronEthernetIPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorWrongTransaction, Job = j, LastExecutionTime = messageReciveDateTime };
                    OnJobExecuted(eJob);
                }
            }
            return s.LastTransOK;
        }

        void GetFragmentedData(ref byte[] pdu, List<CommJob> list, OmronEthernetIPStation s)
        {
            ushort readJobTag;
            ushort totalPduTag = 0;

            ExecutedJobArgs eJob;
            ushort dataOffset = OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE + OmronEthernetIPProtocol.REPLY_CIP_SERVICE_OFFSET + 6; // + 6 --> Skip the service, the status and the sequential ID of the reply
            foreach (OmronEthernetIPCommJob j in list)
            {
                DriverErrorCodes outErrorCode = DriverErrorCodes.ErrorNoError;
                readJobTag = 0;
                foreach (Tag tag in j.TagsList)
                {
                    if (outErrorCode != DriverErrorCodes.ErrorNoError)
                        break;
                    outErrorCode = copyTagFragmentedData(tag, ref pdu, ref dataOffset, j);
                    readJobTag++;
                }
                totalPduTag += readJobTag;

                if (outErrorCode == DriverErrorCodes.ErrorNoError)
                {
                    j.ReadTagStart = j.ReadTagEnd;
                    j.PartialArrayStart = j.PartialArrayEnd;

                    if (j.ReadTagStart == 0 && j.ReadTagEnd == 0 && j.PartialArrayStart == 0 && j.PartialArrayEnd == 0)
                    {
                        eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorNoError, Job = j, LastExecutionTime = messageReciveDateTime };

                        if (j.TagFormat == TagFormats.STRING)
                        {
                            byte[] answer = j.answer;
                            if(answer == null)
                            {
                                answer = new byte[0];
                            }
                            eJob.Values = answer;
                        }
                        else
                        {
                            eJob.Values = j.answer;
                        }

                        OnJobExecuted(eJob);
                    }
                }
                else
                {
                    eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)outErrorCode, Job = j, LastExecutionTime = messageReciveDateTime };
                    OnJobExecuted(eJob);
                }
            }
        }

        private DriverErrorCodes copyTagFragmentedData(Tag pendingTag, ref byte[] pdu, ref ushort dataOffset, OmronEthernetIPCommJob j)
        {
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - copyTagFragmentedData - Init - pdu.Count: {1} - dataOffset: {2}",
                                                   currentTime, pdu.Count(), dataOffset));
            }
#endif

            // Check the PDU length
            if (pdu.Count() < (dataOffset + 2))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - copyTagFragmentedData - Message too short (1) - pdu.Count: {1} - dataOffset: {2}",
                                                       currentTime, pdu.Count(), dataOffset));
                }
#endif
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
            }

            // Get the length of data for the tag (tag type + tag data) and check the PDU length
            ushortUnion tagDataLength = new ushortUnion(pdu, dataOffset);
            dataOffset += 2;
            if (pdu.Count() < (dataOffset + tagDataLength.USHORT))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - copyTagFragmentedData - Message too short (2) - pdu.Count: {1} - dataOffset: {2} - tagDataLength.USHORT: {3}",
                                                       currentTime, pdu.Count(), dataOffset, tagDataLength.USHORT));
                }
#endif
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
            }

            // Store data offset of the current item
            ushort previousDataOffset = dataOffset;

            // Check the data type and go the beginning of tag data
            bool isString = false;
            ushort stringLength = 0;
            DriverErrorCodes outErrorCode = GetFragmentedDataBufferInit(pdu, ref dataOffset, ref isString, ref stringLength);
            if (outErrorCode != DriverErrorCodes.ErrorNoError)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - copyTagFragmentedData - Error {1} in GetFragmentedDataBufferInit",
                                                       currentTime, outErrorCode));
                }
#endif
                return outErrorCode;
            }

            // Check the reply length
            ushort tagReplyLength = j.adjSize(pendingTag);

            // Special case: strings
            if(isString)
            {
                tagReplyLength = stringLength;
            }
            // Special case: array of Booleans
            else if ((pendingTag.TagNode.ArrayDimension > 0) && ((uint)pendingTag.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean))
            {
                tagReplyLength = (ushort)((tagReplyLength + 7) / 8);
            }

            if (pdu.Count() < (dataOffset + tagReplyLength))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - copyTagFragmentedData - Message too short (3) - pdu.Count: {1} - dataOffset: {2} - tagReplyLength: {3}",
                                                       currentTime, pdu.Count(), dataOffset, tagReplyLength));
                }
#endif
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
            }

            if (j.answer == null)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - copyTagFragmentedData - Error ErrorRepTagSize",
                                                       currentTime));
                }
#endif
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTagSize;
            }

            if (j.TagFormat == TagFormats.STRING)
            {
                // If the tag is a string, reset the contents of the job buffer, before copying the new value
                j.answer = new byte[tagReplyLength];
            }

            ushort dataLength = tagReplyLength;
            if (pendingTag.ByteOffset + dataLength > j.answer.Length)
            {
                dataLength = (ushort)(j.answer.Length - pendingTag.ByteOffset);
            }

            if (pdu.Count() < (dataOffset + dataLength))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - copyTagFragmentedData - Message too short (4) - pdu.Count: {1} - dataOffset: {2} - dataLength: {3}",
                                                       currentTime, pdu.Count(), dataOffset, dataLength));
                }
#endif
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
            }

            if(dataLength > 0)
            {
                Array.Copy(pdu, dataOffset, j.answer, pendingTag.ByteOffset, dataLength);
            }
            dataOffset = (ushort)(previousDataOffset + tagDataLength.USHORT);

#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - copyTagFragmentedData - End - Init - pdu.Count: {1} - dataOffset: {2}",
                                                   currentTime, pdu.Count(), dataOffset));
            }
#endif

            return DriverErrorCodes.ErrorNoError;
        }

        private DriverErrorCodes GetFragmentedDataBufferInit(byte[] pdu, ref ushort dataOffset, ref bool isString, ref ushort stringLength)
        {
            // Initialization of output parameters
            isString = false;
            stringLength = 0;

            // Get the data type (abbreviated type)
            switch (pdu[dataOffset])
            {
                // Structure element
                case 0xCC:
                    if (pdu.Count() < (dataOffset + 5))
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }

                    dataOffset += 4;

                    return GetFragmentedDataBufferInit(pdu, ref dataOffset, ref isString, ref stringLength);

                // Structure
                case 0xA0:
                    // data type not supported by driver
                    return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepDataType;

                // BOOL
                case 0xC1:

                // SINT
                case 0xC2:

                // INT
                case 0xC3:

                // DINT
                case 0xC4:

                // USINT
                case 0xC6:

                // UINT
                case 0xC7:

                // UDINT
                case 0xC8:

                // REAL
                case 0xCA:

                // LREAL
                case 0xCB:

                // BYTE
                case 0xD1:

                // WORD
                case 0xD2:

                // DWORD
                case 0xD3:

                // LWORD
                case 0xD4:

                // LINT
                case 0xC5:

                // ULINT
                case 0xC9:
                    if (pdu.Count() < (dataOffset + 3))
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }

                    dataOffset += 2;
                    break;

                // STRING
                case 0xD0:
                    {
                        if (pdu.Count() < (dataOffset + 4))
                        {
                            return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                        }
                        isString = true;
                        ushortUnion strLength = new ushortUnion(pdu, (ushort)(dataOffset + 2));
                        stringLength = strLength.USHORT;
                        dataOffset += 4;
                        if(pdu.Count() < (dataOffset + stringLength))
                        {
                            return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                        }
                    }
                    break;

                // ?
                default:
                    return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepDataType;
            }

            return DriverErrorCodes.ErrorNoError;
        }

        void GetReadData(ref byte[] pdu, List<CommJob> list, OmronEthernetIPStation s)
        {
            ushort readJobTag;
            ushort totalPduTag = 0;

            ExecutedJobArgs eJob;
            foreach (OmronEthernetIPCommJob j in list)
            {
                DriverErrorCodes outErrorCode = DriverErrorCodes.ErrorNoError;

                readJobTag = 0;
                for (ushort tagIndex = j.ReadTagStart; (tagIndex < (j.ReadTagEnd == 0 ? (ushort)j.TagsList.Count : j.ReadTagEnd)) && (outErrorCode == DriverErrorCodes.ErrorNoError); tagIndex++)
                {
                    if (outErrorCode != DriverErrorCodes.ErrorNoError)
                        break;
                    if(tagIndex >= j.TagsList.Count)
                    {
                        outErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                        break;
                    }

                    Tag tag = (Tag)j.TagsList[tagIndex];
                    outErrorCode = copyTagData(tag, ref pdu, (ushort)(totalPduTag + tagIndex - j.ReadTagStart), j);
                    readJobTag++;
                }
                totalPduTag += readJobTag;

                if (outErrorCode == DriverErrorCodes.ErrorNoError)
                {
                    j.ReadTagStart = j.ReadTagEnd;
                    j.PartialArrayStart = j.PartialArrayEnd;

                    if (j.ReadTagStart == 0 && j.ReadTagEnd == 0 && j.PartialArrayStart == 0 && j.PartialArrayEnd == 0)
                    {
                        eJob = new ExecutedJobArgs { ErrorCode = DriverErrorCodes.ErrorNoError, Job = j, LastExecutionTime = messageReciveDateTime };

                        if (j.TagFormat == TagFormats.STRING)
                        {
                            byte[] jobAnswer = j.answer;
                            byte[] answer;
                            if (jobAnswer != null)
                            {
                                ushort answerSize = 0;
                                while (answerSize < jobAnswer.Length && jobAnswer[answerSize] != 0)
                                    answerSize++;
                                answer = new byte[answerSize];
                                Array.Copy(jobAnswer, 0, answer, 0, answerSize);

                            }
                            else
                                answer = new byte[0];

                            eJob.Values = answer;
                        }
                        else
                        {
                            eJob.Values = j.answer;
                        }

                        OnJobExecuted(eJob);
                    }             
                }
                else
                {
                    eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)outErrorCode, Job = j, LastExecutionTime = messageReciveDateTime };
                    OnJobExecuted(eJob);
                }
            }            
        }

        /// <summary>
        /// Manage the request of correct data type of variable when write of value return specific error (used in 2 differents call)
        /// </summary>
        /// <param name="pdu"></param>
        /// <param name="list"></param>
        /// <param name="s"></param>
        void GetReadDataFormat(ref byte[] pdu, List<CommJob> list, OmronEthernetIPStation s)
        {
            ushort totalPduTag = 0;

            // Check the number of replied services
            ushortUnion recieveReply = new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);
            
            for (int i = 0; i < recieveReply.USHORT; i++)
            {
                OmronEthernetIPCommJob j = (OmronEthernetIPCommJob)list[i];

                ushortUnion ReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + 2 + i * 2));
                ReplyOffset.USHORT += OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS;

                int cipErrorCode = Logix5550TagsCheckSingleReply(ref pdu, list, ReplyOffset, ((OmronEthernetIPCommJob)list[0]).CommandType);

                DriverErrorCodes outErrorCode = DriverErrorCodes.ErrorNoError;

                // Special case (0x12018022): Invalid Parameter, Additional status = 0x8022 (wrong data type)
                // don't manage cipErrorCode with a different error code, probably is a different job on error
                if (cipErrorCode == 0 || cipErrorCode == 0x12018022) {
                    ushort readJobTag = 0;
                    for (ushort tagIndex = j.ReadTagStart; (tagIndex < (j.ReadTagEnd == 0 ? (ushort)j.TagsList.Count : j.ReadTagEnd)) && (outErrorCode == DriverErrorCodes.ErrorNoError); tagIndex++)
                    {
                        if (outErrorCode != DriverErrorCodes.ErrorNoError)
                            break;
                        if (tagIndex >= j.TagsList.Count)
                        {
                            outErrorCode = DriverErrorCodes.ErrorParsingAnswer;
                            break;
                        }

                        Tag tag = (Tag)j.TagsList[tagIndex];
                        outErrorCode = copyTagDataFormat(tag, ref pdu, (ushort)(totalPduTag + tagIndex - j.ReadTagStart), j, out bool tagFormatChanged);
                        // 1st call : write operation failed to because data type don't match
                        if (cipErrorCode == 0x12018022)
                        {
                            // leave j.CommandType state into ReadDataFormatCmd --> next cycle execute job as "read" to get correct data format type
                            // force to continue job excecution
                            outErrorCode = DriverErrorCodes.ErrorNoError;
                        }
                        else
                        {
                            // 2st call : correct data type was retrived during read cicle
                            if (IsPlcDataTypeSupported(j.TagFormat))
                            {
                                if (tagFormatChanged)
                                {
                                    j.CommandType = CommandTypes.WriteCmd;
                                    // write operation failed before --> move again data into TagListToWrite for new write operation (now with correct data type)
                                    list[i].ReFillTagListToWrite();
                                    list[i].ClearTagListOnWriting();
                                    // force to continue job excecution
                                    outErrorCode = DriverErrorCodes.ErrorNoError;
                                }
                            }
                            else
                            {
                                // unsupported PLC DataType --> put job in error
                                outErrorCode = (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepDataType;
                            }

                        }
                        readJobTag++;
                    }
                    totalPduTag += readJobTag;
                }
                //repeatOutputJobList.Add(j);
                //checkDataFormatJobList.Remove(j);

                if (!j.RequestCorrectDataTypeToPlcSequence || outErrorCode != DriverErrorCodes.ErrorNoError)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)outErrorCode, Job = j, LastExecutionTime = messageReciveDateTime };
                    OnJobExecuted(eJob);
                }                
            }            
        }

        private bool IsPlcDataTypeSupported(TagFormats tagFormat)
        {
            return (tagFormat != TagFormats.STRUCTURE);
        }

        private DriverErrorCodes copyTagDataFormat(Tag pendingTag, ref byte[] pdu, ushort tagIndex, OmronEthernetIPCommJob j, out bool tagFormatChanged)
        {
            tagFormatChanged = false;

            ushortUnion responseTag = new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (tagIndex >= responseTag.USHORT)
            {
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
            }
            ushortUnion ReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + tagIndex * 2 + 2));
            ReplyOffset.USHORT += (OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (ReplyOffset.USHORT >= pdu.Count())
            {
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
            }

            ushort ReplyLength;
            if (tagIndex == responseTag.USHORT - 1)
            {
                ReplyLength = (ushort)(pdu.Count() - ReplyOffset.USHORT);
            }
            else
            {
                ushortUnion nextReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + tagIndex * 2 + 4));
                nextReplyOffset.USHORT += (OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);

                if ((ReplyOffset.USHORT >= nextReplyOffset.USHORT) || (nextReplyOffset.USHORT >= pdu.Count()))
                {
                    return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                }

                ReplyLength = (ushort)(nextReplyOffset.USHORT - ReplyOffset.USHORT);
            }

            TagFormats tagFormat = TagFormats.BOOL;
            DriverErrorCodes outErrorCode = getDataFormat(pdu, ref ReplyOffset, ref ReplyLength, ref tagFormat);
            if (outErrorCode != DriverErrorCodes.ErrorNoError)
            {
                return outErrorCode;
            }

            if(j.TagFormat != tagFormat)
            {
                j.TagFormat = tagFormat;
                tagFormatChanged = true;
            }

            return DriverErrorCodes.ErrorNoError;
        }

        private DriverErrorCodes copyTagData(Tag pendingTag, ref byte[] pdu, ushort tagIndex, OmronEthernetIPCommJob j)
        {
            ushortUnion responseTag = new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (tagIndex >= responseTag.USHORT)
            {
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
            }
            ushortUnion ReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + tagIndex * 2 + 2));
            ReplyOffset.USHORT += (OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);
            if (ReplyOffset.USHORT >= pdu.Count())
            {
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
            }

            ushort ReplyLength;
            if (tagIndex == responseTag.USHORT - 1)
            {
                ReplyLength = (ushort)(pdu.Count() - ReplyOffset.USHORT);
            }
            else
            {
                ushortUnion nextReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + tagIndex * 2 + 4));
                nextReplyOffset.USHORT += (OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);

                if ((ReplyOffset.USHORT >= nextReplyOffset.USHORT) || (nextReplyOffset.USHORT >= pdu.Count()))
                {
                    return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                }

                ReplyLength = (ushort)(nextReplyOffset.USHORT - ReplyOffset.USHORT);
            }

            DriverErrorCodes outErrorCode = getDataBufferInit(pdu, ref ReplyOffset, ref ReplyLength);
            if (outErrorCode != DriverErrorCodes.ErrorNoError)
                return outErrorCode;
            ushort tagReplyLength;
            if (j.PartialArrayEnd == j.PartialArrayStart)
                tagReplyLength = j.adjSize(pendingTag) ;
            else
            {
                if (j.PartialArrayEnd != 0)
                    tagReplyLength = (ushort)((j.PartialArrayEnd - j.PartialArrayStart) * j.ElemSize);
                else
                    tagReplyLength = (ushort)(j.adjSize(pendingTag) - j.PartialArrayStart * j.ElemSize);
            }

            ushort dataLength = ReplyLength;
            if(dataLength > tagReplyLength)
            {
                dataLength = tagReplyLength;
            }
            if (j.answer == null)
            {
                return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTagSize;
            }

            if (j.TagFormat == TagFormats.STRING)
            {
                // If the tag is a string, reset the contents of the job buffer, before copying the new value
                j.answer = new byte[j.TotalJobSize];
            }

            if (pendingTag.ByteOffset + dataLength > j.answer.Length)
            {
                dataLength = (ushort)(j.answer.Length - pendingTag.ByteOffset);
            }

            Array.Copy(pdu, ReplyOffset.USHORT, j.answer, pendingTag.ByteOffset + j.PartialArrayStart * j.ElemSize, dataLength);

            return DriverErrorCodes.ErrorNoError;
        }
        private DriverErrorCodes getDataBufferInit(byte[] pdu, ref ushortUnion ReplyOffset, ref ushort ReplyLength)
        {
            // Get the data type (abbreviated type)
            switch (pdu[ReplyOffset.USHORT])
            {
                // Structure element
                case 0xCC:
                    if (ReplyLength < 5)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }

                    ReplyOffset.USHORT += 4;
                    ReplyLength -= 4;

                    return getDataBufferInit(pdu, ref ReplyOffset, ref ReplyLength);


                // Structure
                case 0xA0:
                    // data type don't supported by driver
                    return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepDataType;
                    //if (ReplyLength < 5)
                    //{
                    //    return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    //}

                    //ReplyOffset.USHORT += 4;
                    //ReplyLength -= 4;

                    //break;

                // BOOL
                case 0xC1:

                // SINT
                case 0xC2:

                // INT
                case 0xC3:

                // DINT
                case 0xC4:

                // USINT
                case 0xC6:

                // UINT
                case 0xC7:

                // UDINT
                case 0xC8:

                // REAL
                case 0xCA:

                // LREAL
                case 0xCB:

                // BYTE
                case 0xD1:

                // WORD
                case 0xD2:

                // DWORD
                case 0xD3:

                // LWORD
                case 0xD4:

                // LINT
                case 0xC5:

                // ULINT
                case 0xC9:

                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    ReplyOffset.USHORT += 2;
                    ReplyLength -= 2;
                    break;

                // STRING
                case 0xD0:
                    if (ReplyLength < 4)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }

                    ReplyOffset.USHORT += 4;
                    ReplyLength -= 4;

                    break;

                // ?
                default:
                    return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepDataType;
            }
            return DriverErrorCodes.ErrorNoError;
        }

        private DriverErrorCodes getDataFormat(byte[] pdu, ref ushortUnion ReplyOffset, ref ushort ReplyLength, ref TagFormats tagFormat)
        {
            // Get the data type (abbreviated type)
            switch (pdu[ReplyOffset.USHORT])
            {
                // Structure element
                case 0xCC:
                    if (ReplyLength < 5)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }

                    ReplyOffset.USHORT += 4;
                    ReplyLength -= 4;

                    return getDataFormat( pdu, ref ReplyOffset, ref ReplyLength, ref tagFormat);                  

                // Structure
                case 0xA0:
                    if (ReplyLength < 5)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.STRUCTURE;                    
                    break;

                // BOOL
                case 0xC1:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.BOOL;
                    break;

                // SINT
                case 0xC2:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.SINT;
                    break;

                // INT
                case 0xC3:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.INT;
                    break;

                // DINT
                case 0xC4:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.DINT;
                    break;

                // USINT
                case 0xC6:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.USINT;
                    break;

                // UINT
                case 0xC7:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.UINT;
                    break;

                // UDINT
                case 0xC8:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.UDINT;
                    break;

                // REAL
                case 0xCA:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.REAL;
                    break;

                // LREAL
                case 0xCB:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.LREAL;
                    break;

                // BYTE
                case 0xD1:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.BYTE;
                    break;

                // WORD
                case 0xD2:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.WORD;
                    break;

                // DWORD
                case 0xD3:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.DWORD;
                    break;

                // LWORD
                case 0xD4:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.LWORD;
                    break;

                // LINT
                case 0xC5:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.LINT;
                    break;

                // ULINT
                case 0xC9:
                    if (ReplyLength < 3)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.ULINT;
                    break;

                // STRING
                case 0xD0:
                    if (ReplyLength < 4)
                    {
                        return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepTooShort;
                    }
                    tagFormat = TagFormats.STRING;
                    break;

                // ?
                default:
                    return (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepDataType;
            }

            return DriverErrorCodes.ErrorNoError;
        }

        bool Logix5550TagsCheckReply(ref byte[] pdu, OmronEthernetIPStation s, List<CommJob> list)
        {            
            OmronEthernetIPErrorCodes encapsulationErrorCode = AnalyzeReplyHeader(ref pdu, OmronEthernetIPProtocol.ecSendUnitData.USHORT, SessionHandle.UINT);
            if (encapsulationErrorCode != (OmronEthernetIPErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                foreach (OmronEthernetIPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)encapsulationErrorCode, Job = j, LastExecutionTime = messageReciveDateTime };
                    OnJobExecuted(eJob);
                }
                return false;
            }

            byte MRStatus = pdu[OmronEthernetIPProtocol.LOGIX5550_REP_GENSTS_OFFS];
            if ((MRStatus != 0) && (MRStatus != 0x1E)) // 0x1E == Embedded service error
            {
                int omronErrorCode = (int)DriverErrorCodes.ErrorNoError;
                switch((int)MRStatus)
                {
                    case 0x02:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus02;
                        break;

                    case 0x04:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus04;
                        break;

                    case 0x05:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus05;
                        break;

                    case 0x0C:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus0C;
                        break;

                    case 0x11:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus11;
                        break;

                    case 0x13:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus13;
                        break;

                    case 0x15:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus15;
                        break;

                    case 0x1F:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus1F;
                        break;

                    case 0x20:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus20;
                        break;

                    default:
                        omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus;
                        break;
                }

                // Length (in words) of the additional status  
                byte MRAddStatusSize = pdu[OmronEthernetIPProtocol.LOGIX5550_REP_GENSTSSZ_OFFS];

                if ((MRAddStatusSize > 0) && (omronErrorCode != (int)OmronEthernetIPErrorCodes.ErrorCipStatus))
                {
                    ushortUnion addInfo = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_GENSTSSZ_OFFS + 1));
                    switch((int)addInfo.USHORT)
                    {
                        case 0x102:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus102;
                            break;
                        case 0x104:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus104;
                            break;
                        case 0x1103:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus1103;
                            break;
                        case 0x2103:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus2103;
                            break;
                        case 0x2104:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus2104;
                            break;
                        case 0x8001:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8001;
                            break;
                        case 0x8007:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8007;
                            break;
                        case 0x8009:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8009;
                            break;
                        case 0x800F:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus800F;
                            break;
                        case 0x8010:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8010;
                            break;
                        case 0x8011:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8011;
                            break;
                        case 0x8017:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8017;
                            break;
                        case 0x8018:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8018;
                            break;
                        case 0x8021:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8021;
                            break;
                        case 0x8022:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8022;
                            break;
                        case 0x8023:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8023;
                            break;
                        case 0x8024:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8024;
                            break;
                        case 0x8025:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8025;
                            break;
                        case 0x8028:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8028;
                            break;
                        case 0x8029:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8029;
                            break;
                        case 0x8031:
                            omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8031;
                            break;
                    }
                }

                foreach (OmronEthernetIPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)omronErrorCode, Job = j, LastExecutionTime = messageReciveDateTime };
                    OnJobExecuted(eJob);
                }
                return false;
            }

            // Check the number of replied services
            ushortUnion recieveReply = new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);

            if (recieveReply.USHORT != s.lastProcessedTags.USHORT)
            {
                foreach (OmronEthernetIPCommJob j in list)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)OmronEthernetIPErrorCodes.ErrorRepSerNum, Job = j, LastExecutionTime = messageReciveDateTime };
                    OnJobExecuted(eJob);
                }
                return false;
            }


            // Special case when write values: Invalid Parameter, Additional status = 0x8022 (wrong data type)
            // put job in a separate list to obtain information about tag (ReadDataFormatCmd)
            if (list.Count> 0 && ((OmronEthernetIPCommJob)list[0]).CommandType == CommandTypes.WriteCmd)
            {
                for (int i = 0; i < recieveReply.USHORT; i++)
                {
                    ushortUnion ReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + 2 + i * 2));
                    ReplyOffset.USHORT += OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS;

                    int cipErrorCode = Logix5550TagsCheckSingleReply(ref pdu, list, ReplyOffset, ((OmronEthernetIPCommJob)list[0]).CommandType);
                    if (cipErrorCode == 0x12018022)
                    {
                        if (i < list.Count)
                        {
                            ((OmronEthernetIPCommJob)list[i]).RequestCorrectDataTypeToPlcSequence = true;
                            ((OmronEthernetIPCommJob)list[i]).CommandType = CommandTypes.ReadDataFormatCmd;
                        }
                    }
                    //localListPendingJob.Add(list[i]);
                }
            }

            return true;
        }

        void CheckTheWritingResponse(ref byte[] pdu, List<CommJob> list, OmronEthernetIPStation s)
        {
            LastErrorMessage = "";

            // Check the number of replied services
            ushortUnion recieveReply = new ushortUnion(pdu, OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS);            
            for (int i = 0; i < recieveReply.USHORT; i++)
            {
                OmronEthernetIPCommJob j = list[i] as OmronEthernetIPCommJob;

                ushortUnion ReplyOffset = new ushortUnion(pdu, (ushort)(OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS + 2 + i * 2));
                ReplyOffset.USHORT += OmronEthernetIPProtocol.LOGIX5550_REP_SERNUM_OFFS;

                int cipErrorCode = Logix5550TagsCheckSingleReply(ref pdu, list, ReplyOffset, j.CommandType);
                // Special case: Invalid Parameter, Additional status = 0x8022 (wrong data type)
                // put job in a separate list to obtain information about tag
                if (cipErrorCode == 0x12018022)
                {

                }
                else
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)cipErrorCode, Job = j, LastExecutionTime = messageReciveDateTime };
                    if (((DriverErrorCodes)cipErrorCode) == DriverErrorCodes.ErrorNoError)
                    {
                        if ((j.PartialArrayEnd != 0) && (j.CommandType == CommandTypes.WriteCmd) && (j.TagsList[0].TagNode.ArrayDimension > 0))
                        {
                            //System.Diagnostics.Debug.WriteLine("--- Debug --- j.PartialArrayStart:{0} j.PartialArrayEnd:{1}", job.PartialArrayStart, job.PartialArrayEnd);
                            j.PartialArrayStart = j.PartialArrayEnd;
                        }
                    }
                    else
                    {
                        j.PartialArrayStart = 0;
                        j.PartialArrayEnd = 0;
                    }
                    // don't execute (close) job is incomplete
                    if (j.ReadTagStart == 0 && j.ReadTagEnd == 0 && j.PartialArrayStart == 0 && j.PartialArrayEnd == 0)
                    {
                        //System.Diagnostics.Debug.WriteLine(string.Format("CheckTheWritingResponse OnJobExecuted {0}", j.TagsList[0].DynSettings.ToString()));
                        OnJobExecuted(eJob);
                    }                    
                }
            }
        }

        private int Logix5550TagsCheckSingleReply(ref byte[] pdu, List<CommJob> list, ushortUnion ReplyOffset, CommandTypes CommandType)
        {
            int omronErrorCode = (int)DriverErrorCodes.ErrorNoError;
            //String addressInfo = string.Format(Properties.Resources.AddressInfo,
            //        ((OmronEthernetIPCommJob)list[0]).ABAddress,
            //        ((OmronEthernetIPCommJob)list[0]).TotalJobSize,
            //        ((OmronEthernetIPCommJob)list[0]).TagsList[0].ByteOffset);	//Remember that the transaction was OK

            if ((CommandType == CommandTypes.ReadCmd && pdu[ReplyOffset.USHORT] != 0xCC) ||
                (CommandType == CommandTypes.WriteCmd && pdu[ReplyOffset.USHORT] != 0xCD))
            {
                omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorRepSerCode;
            }
            else
            {
                byte ReplyStatus = pdu[ReplyOffset.USHORT + 2];
                if (ReplyStatus != 0)
                {
                    switch ((int)ReplyStatus)
                    {
                        case 0x02:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus02;
                            break;

                        case 0x04:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus04;
                            break;

                        case 0x05:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus05;
                            break;

                        case 0x0C:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus0C;
                            break;

                        case 0x11:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus11;
                            break;

                        case 0x13:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus13;
                            break;

                        case 0x15:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus15;
                            break;

                        case 0x1F:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus1F;
                            break;

                        case 0x20:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus20;
                            break;

                        default:
                            omronErrorCode = (int)OmronEthernetIPErrorCodes.ErrorCipStatus;
                            break;
                    }

                    // Length (in words) of the additional status  
                    byte AddStatusSize = pdu[ReplyOffset.USHORT + 3];
                    if ((AddStatusSize > 0) && (omronErrorCode != (int)OmronEthernetIPErrorCodes.ErrorCipStatus))
                    {
                        ushortUnion addInfo = new ushortUnion(pdu, (ushort)(ReplyOffset.USHORT + 4));
                        switch ((int)addInfo.USHORT)
                        {
                            case 0x102:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus102;
                                break;
                            case 0x104:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus104;
                                break;
                            case 0x1103:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus1103;
                                break;
                            case 0x2103:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus2103;
                                break;
                            case 0x2104:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus2104;
                                break;
                            case 0x8001:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8001;
                                break;
                            case 0x8007:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8007;
                                break;
                            case 0x8009:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8009;
                                break;
                            case 0x800F:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus800F;
                                break;
                            case 0x8010:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8010;
                                break;
                            case 0x8011:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8011;
                                break;
                            case 0x8017:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8017;
                                break;
                            case 0x8018:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8018;
                                break;
                            case 0x8021:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8021;
                                break;
                            case 0x8022:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8022;
                                break;
                            case 0x8023:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8023;
                                break;
                            case 0x8024:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8024;
                                break;
                            case 0x8025:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8025;
                                break;
                            case 0x8028:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8028;
                                break;
                            case 0x8029:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8029;
                                break;
                            case 0x8031:
                                omronErrorCode += (int)OmronEthernetIPErrorCodes.ErrorCipAddStatus8031;
                                break;
                        }
                    }
                }
            }

            return (omronErrorCode);
        }

        void PlcResponseError(byte stsCode, byte extStsCode, OmronEthernetIPCommJob mJob)
        {
            OmronEthernetIPErrorCodes OmronEthernetIPErrorCode;
            
            if (stsCode > 0 && stsCode < 10)
            {
                OmronEthernetIPErrorCode = OmronEthernetIPErrorCodes.ErrorStatusNotZero;
            }
            else if (stsCode == 0xF0)
            {
                if (extStsCode >= 0x01 && extStsCode <= 0x24)
                {
                    OmronEthernetIPErrorCode = OmronEthernetIPErrorCodes.ErrorEXTSTSCodeExt;
                }
                else
                {
                    OmronEthernetIPErrorCode = OmronEthernetIPErrorCodes.ErrorUnknSTSEXTCode;
                }
            }
            else
            {
                OmronEthernetIPErrorCode = OmronEthernetIPErrorCodes.ErrorUnknSTSCode;
            }

            ExecutedJobArgs eJob = new ExecutedJobArgs { ErrorCode = (DriverErrorCodes)OmronEthernetIPErrorCode, Job = mJob, LastExecutionTime = messageReciveDateTime };
            OnJobExecuted(eJob);
        }

        public void ReceiveClear()
        {
            lock (lockThreadObject)
                ReceiveBuffer.Clear();
            Flush();
        }
        
        public bool OmronEthernetIPDeviceWrite(ref byte[] TcpBuffer, ref ushortUnion TcpBufferSize)
        {
            //fill in packet data length
            if (TcpBufferSize.USHORT < OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("DEBUG RegisterSession - OmronEthernetIPDeviceWrite TcpBufferSize too short");
#endif
                return false;
            }
            ushort TcpDataSize = TcpBufferSize.USHORT;

            TcpBufferSize.USHORT -= OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE;  
            TcpBuffer[OmronEthernetIPProtocol.EDATA_LEN_OFFS] = TcpBufferSize.LOBYTE;				//Enc.data length lobyte
            TcpBuffer[OmronEthernetIPProtocol.EDATA_LEN_OFFS + 1] = TcpBufferSize.HIBYTE;			//Enc.data length hibyte

            return DeviceWrite(TcpBuffer, TcpDataSize);
        }

        public OmronEthernetIPErrorCodes AnalyzeReplyHeader(ref byte[] pdu, ushort Cmd, uint stationSHandle)    
        {
            ushortUnion CmdHeader = new ushortUnion(pdu, 0);
            uintUnion Status = new uintUnion(pdu, OmronEthernetIPProtocol.EDATA_STS_OFFS);
            uintUnion replySHandle = new uintUnion(pdu, OmronEthernetIPProtocol.EDATA_SESS_HND_OFFS);

            if( CmdHeader.USHORT != Cmd || stationSHandle !=replySHandle.UINT )
            {
                return OmronEthernetIPErrorCodes.ErrorOutOfSync;
            }
            //Examine the Encapsulation command status

            if (Status.UINT == 1)
            {
                return OmronEthernetIPErrorCodes.ErrorEncapStatus1;
            }
            else if (Status.UINT == 2)
            {
                return OmronEthernetIPErrorCodes.ErrorEncapStatus2;
            }
            else if (Status.UINT == 3)
            {
                return OmronEthernetIPErrorCodes.ErrorEncapStatus3;
            }
            else if (Status.UINT == 0x64)
            {
                return OmronEthernetIPErrorCodes.ErrorEncapStatus64;
            }
            else if (Status.UINT == 0x65)
            {
                return OmronEthernetIPErrorCodes.ErrorEncapStatus65;
            }
            else if (Status.UINT == 0x69)
            {
                return OmronEthernetIPErrorCodes.ErrorEncapStatus69;
            }
            else if (Status.UINT != 0)
            {
                return OmronEthernetIPErrorCodes.ErrorUnknEncapStatus;
            }
            return (OmronEthernetIPErrorCodes)DriverErrorCodes.ErrorNoError;
        }

        public CommunicationDriver getCommDriver()
        {
            return CommDriver;
        }

        #endregion

        #region ErrorCodeArrayString

        protected string[] PlcResponseErrorList1 = 
        {
            Properties.Resources.ErrorSTSCode01,
            Properties.Resources.ErrorSTSCode02,
            Properties.Resources.ErrorSTSCode03,
            Properties.Resources.ErrorSTSCode04,
            Properties.Resources.ErrorSTSCode05,
            Properties.Resources.ErrorSTSCode06,
            Properties.Resources.ErrorSTSCode07,
            Properties.Resources.ErrorSTSCode08,
            Properties.Resources.ErrorSTSCode09Spare,
        };

        protected string[] PlcResponseErrorList2 = 
        {
            Properties.Resources.ErrorSTSCode10,
            Properties.Resources.ErrorSTSCode20,
            Properties.Resources.ErrorSTSCode30,
            Properties.Resources.ErrorSTSCode40,
            Properties.Resources.ErrorSTSCode50,
            Properties.Resources.ErrorSTSCode60,
            Properties.Resources.ErrorSTSCode70,
            Properties.Resources.ErrorSTSCode80,
            Properties.Resources.ErrorSTSCode90,
            Properties.Resources.ErrorSTSCodeA0,
            Properties.Resources.ErrorSTSCodeB0,
            Properties.Resources.ErrorSTSCodeC0,
            Properties.Resources.ErrorSTSCodeD0,
            Properties.Resources.ErrorSTSCodeE0,
        };

        protected string[] PlcResponseErrorList3 = 
        {
            Properties.Resources.ErrorEXTSTS01,
            Properties.Resources.ErrorEXTSTS02,
            Properties.Resources.ErrorEXTSTS03,
            Properties.Resources.ErrorEXTSTS04,
            Properties.Resources.ErrorEXTSTS05,
            Properties.Resources.ErrorEXTSTS06,
            Properties.Resources.ErrorEXTSTS07,
            Properties.Resources.ErrorEXTSTS08,
            Properties.Resources.ErrorEXTSTS09,
            Properties.Resources.ErrorEXTSTS0A,
            Properties.Resources.ErrorEXTSTS0B,
            Properties.Resources.ErrorEXTSTS0C,
            Properties.Resources.ErrorEXTSTS0D,
            Properties.Resources.ErrorEXTSTS0E,
            Properties.Resources.ErrorEXTSTS0F,
            Properties.Resources.ErrorEXTSTS10,
            Properties.Resources.ErrorEXTSTS11,
            Properties.Resources.ErrorEXTSTS12,
            Properties.Resources.ErrorEXTSTS13,
            Properties.Resources.ErrorEXTSTS14,
            Properties.Resources.ErrorEXTSTS15,
            Properties.Resources.ErrorEXTSTS16,
            Properties.Resources.ErrorEXTSTS17,
            Properties.Resources.ErrorEXTSTS18,
            Properties.Resources.ErrorEXTSTS19,
            Properties.Resources.ErrorEXTSTS1A,
            Properties.Resources.ErrorEXTSTS1B,
            Properties.Resources.ErrorEXTSTS1C,
            Properties.Resources.ErrorEXTSTS1D,
            Properties.Resources.ErrorEXTSTS1E,
            Properties.Resources.ErrorEXTSTS1F,
            Properties.Resources.ErrorEXTSTS20,
            Properties.Resources.ErrorEXTSTS21,
            Properties.Resources.ErrorEXTSTS22,
            Properties.Resources.ErrorEXTSTS23,
            Properties.Resources.ErrorEXTSTS24,
         };



        #endregion

        #region Properties

        #endregion

        #region IDisposable Interface

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged
        /// resources.
        /// </summary>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public override void Dispose()
        {
            OmronEthernetIPProtocol.CloseSession(ref requestBuffer, ref requestBufferPointer, this);

            base.Dispose();
        }

        #endregion

    }
}