using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using System.Runtime.InteropServices;
using System.Threading;
using DriverCodeBaseEx.UI;

namespace BrPvi.UI
{
    /// <summary>
    /// Interaction logic for ImportTagsEditorTree.xaml
    /// </summary>
    public partial class BrPviPlcImportParser : BrPviImportBase, IDisposable
    {
        #region Properties
        Dictionary<string, BrPviStationSettings> dictionaryOfStationSettings = new Dictionary<string, BrPviStationSettings>();
        public Dictionary<string, BrPviStationSettings> DictionaryOfStationSettings { set { dictionaryOfStationSettings = value; }  }

        Dictionary<string, BrPviChannelSettings> dictionaryOfChannelSettings = new Dictionary<string, BrPviChannelSettings>();
        public Dictionary<string, BrPviChannelSettings> DictionaryOfChannelSettings { set { dictionaryOfChannelSettings = value; } }

        // Direct Import
        string selectedStation = string.Empty;
        PviComManager pviCommunicationManager;
        ImportPviSink importerPviSink;
        Thread communicationThread;
        public ManualResetEvent StopCommunicationThread;
        ImportPviComEvents pviEventsObj;
        GCHandle gcHandleImporter;
        BrPviProcol.BrPviConnectionStatus connectionStatus = BrPviProcol.BrPviConnectionStatus.NotInitialized;
        int communicationTimeout = 10000;
        // Result of the communication thread
        int lastConnectionErrorCode = 0;
        string lastConnectionErrorMessage = String.Empty;
        // Global events
        List<PviEventInfo> listOfGlobalEvents;
        PviCallbackWithData64Bit pviConnectCallback64Bit;
        PviCallbackWithData32Bit pviConnectCallback32Bit;
        GCHandle gcHandleConnectCallbackFunc;
        IntPtr intptrConnectCallback;
        PviCallbackWithData64Bit pviDisconnectCallback64Bit;
        PviCallbackWithData32Bit pviDisconnectCallback32Bit;
        GCHandle gcHandleDisconnectCallbackFunc;
        IntPtr intptrDisconnectCallback;
        PviCallbackWithData64Bit pviArrangeCallback64Bit;
        PviCallbackWithData32Bit pviArrangeCallback32Bit;
        GCHandle gcHandleArrangeCallbackFunc;
        IntPtr intptrArrangeCallback;
        // Data events
        public PviCallbackWithData64Bit pviDataCallback64Bit;
        public PviCallbackWithData32Bit pviDataCallback32Bit;
        public GCHandle gcHandleDataCallbackFunc;
        public IntPtr intptrDataCallback;
        // PVI objects (PLC connection)
        BrPviPviObject pviLine;
        BrPviPviObject pviDevice;
        BrPviPviObject pviStation;
        BrPviPviObject pviCpu;
        GCHandle gcHandlePviLineObj;
        GCHandle gcHandlePviDeviceObj;
        GCHandle gcHandlePviStationObj;
        GCHandle gcHandlePviCpuObj;
        // General dictionary of PVI objects
        Object lockmapPviObjects;
        Dictionary<string, BrPviPviObject> mapPviObjects;
        // List of events on PVI objects
        Object pviEventLockObject;
        List<PviObjEvent> listOfPviEvents;
        // List of pending PVI object create requests
        List<string> pendingCreateRequests;
        // List of pending PVI object read requests
        List<string> pendingReadRequests;
        // PVI tasks
        string[] pviTaskNames;
        List<BrPviPviObject> taskPviObjects;
        // Dictionary of global handles for the PVI objects
        Dictionary<string, GCHandle> mapGlobalHandles;
        // List of PVI variable objects to be created in order to get type definitions (one variable for each type) 
        List<BrPviPviObject> variablePviObjects;
        
        // plc var was declared as reference/pointer
        private const string POBJ_ACC_LIST_PVAR_DYNAMIC = "d";
        #endregion

        #region Constructors
        public BrPviPlcImportParser() : base()
        {
        }
        #endregion

        #region Methods
        public override ImportDataModel Import(GetStationName readStationName ,string station)
        {
            selectedStation = station;
            using (new WaitCursor())
            {                
                AllocateObjectsForDirectImport();

                importDataModel = new ImportDataModelBrPVI(readStationName);
                
                lastConnectionErrorCode = 0;
                lastConnectionErrorMessage = String.Empty;

                // Run a separate thread in order to get PLC data info
                //communicationThread = new Thread(WorkingThread);
                communicationThread.Name = "ImportPviComExternalThread";
                communicationThread.SetApartmentState(ApartmentState.STA);
                StopCommunicationThread.Reset();
                if (!communicationThread.IsAlive)
                {
                    communicationThread.Start(0);
                }

                // Wait for the termination of the thread
                communicationThread.Join();

                // Check errors
                if (lastConnectionErrorCode == 0)
                {
                    ParsePlcData();
                } else
                {
                    base.LastError = lastConnectionErrorMessage;
                }

                EmptyListsAndMapsForDirectImport();

                DeallocateObjectsForDirectImport();
                
            }

            return importDataModel;
        }
     
        void AllocateObjectsForDirectImport()
        {
            listOfGlobalEvents = new List<PviEventInfo>();
            lockmapPviObjects = new Object();
            mapPviObjects = new Dictionary<string, BrPviPviObject>();
            pviEventLockObject = new Object();
            listOfPviEvents = new List<PviObjEvent>();
            pendingCreateRequests = new List<string>();
            pendingReadRequests = new List<string>();
            taskPviObjects = new List<BrPviPviObject>();
            mapGlobalHandles = new Dictionary<string, GCHandle>();
            variablePviObjects = new List<BrPviPviObject>();
            mapPviVariables = new Dictionary<string, string[]>();
            mapPviVarTypes = new Dictionary<string, string>();
            pviEventsObj = new ImportPviComEvents(this);
            unsafe
            {
                pviConnectCallback64Bit = pviEventsObj.PviConnectCallback64Bit;
                pviDisconnectCallback64Bit = pviEventsObj.PviDisconnectCallback64Bit;
                pviArrangeCallback64Bit = pviEventsObj.PviArrangeCallback64Bit;
                pviDataCallback64Bit = pviEventsObj.PviDataCallback64Bit;
                pviConnectCallback32Bit = pviEventsObj.PviConnectCallback32Bit;
                pviDisconnectCallback32Bit = pviEventsObj.PviDisconnectCallback32Bit;
                pviArrangeCallback32Bit = pviEventsObj.PviArrangeCallback32Bit;
                pviDataCallback32Bit = pviEventsObj.PviDataCallback32Bit;
            }
            importerPviSink = new ImportPviSink(this);
            StopCommunicationThread = new ManualResetEvent(false);
            communicationThread = new Thread(WorkingThread);
        }

        void DeallocateObjectsForDirectImport()
        {
            if (listOfGlobalEvents != null)
            {
                listOfGlobalEvents.Clear();
                listOfGlobalEvents = null;
            }
            if (lockmapPviObjects != null)
            {
                lockmapPviObjects = null;
            }
            if (mapPviObjects != null)
            {
                mapPviObjects.Clear();
                mapPviObjects = null;
            }
            if (pviEventLockObject != null)
            {
                pviEventLockObject = null;
            }
            if (listOfPviEvents != null)
            {
                listOfPviEvents.Clear();
                listOfPviEvents = null;
            }
            if (pendingCreateRequests != null)
            {
                pendingCreateRequests.Clear();
                pendingCreateRequests = null;
            }
            if (pendingReadRequests != null)
            {
                pendingReadRequests.Clear();
                pendingReadRequests = null;
            }
            if (taskPviObjects != null)
            {
                taskPviObjects.Clear();
                taskPviObjects = null;
            }
            if (mapGlobalHandles != null)
            {
                mapGlobalHandles.Clear();
                mapGlobalHandles = null;
            }
            if (variablePviObjects != null)
            {
                variablePviObjects.Clear();
                variablePviObjects = null;
            }
            if (mapPviVariables != null)
            {
                mapPviVariables.Clear();
                mapPviVariables = null;
            }
            if (mapPviVarTypes != null)
            {
                mapPviVarTypes.Clear();
                mapPviVarTypes = null;
            }
            unsafe
            {
                pviConnectCallback64Bit = null;
                pviDisconnectCallback64Bit = null;
                pviArrangeCallback64Bit = null;
                pviDataCallback64Bit = null;
                pviConnectCallback32Bit = null;
                pviDisconnectCallback32Bit = null;
                pviArrangeCallback32Bit = null;
                pviDataCallback32Bit = null;
            }
            pviEventsObj = null;
            if (importerPviSink != null)
            {
                importerPviSink.Dispose();
                //    Thread.Sleep(500);
                importerPviSink = null;
            }
            if (StopCommunicationThread != null)
            {
                StopCommunicationThread.Dispose();
                StopCommunicationThread = null;
            }
            communicationThread = null;
        }
               
        #region Import from PLC --> Reading data from Device
        public void PviXInitialize()
        {
            lastConnectionErrorCode = 0;
            if (pviCommunicationManager == null)
            {
                pviCommunicationManager = new PviComManager();
            }
            string channelName = dictionaryOfStationSettings[selectedStation].Channel;
            communicationTimeout = (int)dictionaryOfChannelSettings[channelName].BrPviServerCommunicationTimeout * 1000;
            int retryTime = (int)dictionaryOfChannelSettings[channelName].BrPviRetryTime;
            string serverAddress = dictionaryOfChannelSettings[channelName].BrPviServerAddress;
            int portNumber = (int)dictionaryOfChannelSettings[channelName].BrPviServerPort;
            lastConnectionErrorCode = pviCommunicationManager.PviXInitialize(communicationTimeout, retryTime, serverAddress, portNumber);
            if (lastConnectionErrorCode != 0)
            {
                lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorPviXInitialize, lastConnectionErrorCode, channelName);
            }
        }

        public void SetPviGlobalEvents()
        {
            lastConnectionErrorCode = pviCommunicationManager.SetPviGlobalEvents(intptrConnectCallback, intptrDisconnectCallback, intptrArrangeCallback, GCHandle.ToIntPtr(gcHandleImporter));
            if (lastConnectionErrorCode != 0)
            {
                lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorSetGlobalEvents, lastConnectionErrorCode);
            }
        }

        public void PviXDeinitialize()
        {
            pviCommunicationManager.PviXDeinitialize();
        }

        bool DeviceOpen()
        {
            if (pviCommunicationManager == null)
                pviCommunicationManager = new PviComManager();

            if (pviCommunicationManager.IsInitialized())
                return (true);

            // Set global handles for the callback functions
            DeallocateGlobalHandles();

            gcHandleImporter = GCHandle.Alloc(this);
            if (System.Environment.Is64BitProcess)
            {
                intptrConnectCallback = Marshal.GetFunctionPointerForDelegate(pviConnectCallback64Bit);
                gcHandleConnectCallbackFunc = GCHandle.Alloc(pviConnectCallback64Bit);
                intptrDisconnectCallback = Marshal.GetFunctionPointerForDelegate(pviDisconnectCallback64Bit);
                gcHandleDisconnectCallbackFunc = GCHandle.Alloc(pviDisconnectCallback64Bit);
                intptrArrangeCallback = Marshal.GetFunctionPointerForDelegate(pviArrangeCallback64Bit);
                gcHandleArrangeCallbackFunc = GCHandle.Alloc(pviArrangeCallback64Bit);
                intptrDataCallback = Marshal.GetFunctionPointerForDelegate(pviDataCallback64Bit);
                gcHandleDataCallbackFunc = GCHandle.Alloc(pviDataCallback64Bit);
            }
            else
            {
                intptrConnectCallback = Marshal.GetFunctionPointerForDelegate(pviConnectCallback32Bit);
                gcHandleConnectCallbackFunc = GCHandle.Alloc(pviConnectCallback32Bit);
                intptrDisconnectCallback = Marshal.GetFunctionPointerForDelegate(pviDisconnectCallback32Bit);
                gcHandleDisconnectCallbackFunc = GCHandle.Alloc(pviDisconnectCallback32Bit);
                intptrArrangeCallback = Marshal.GetFunctionPointerForDelegate(pviArrangeCallback32Bit);
                gcHandleArrangeCallbackFunc = GCHandle.Alloc(pviArrangeCallback32Bit);
                intptrDataCallback = Marshal.GetFunctionPointerForDelegate(pviDataCallback32Bit);
                gcHandleDataCallbackFunc = GCHandle.Alloc(pviDataCallback32Bit);
            }

            // Initialize the PVICOM interface, establish a PVICOM communication instance,
            // and initiate the registration of the communication instance (client) with PVI Manager(server)
            if (!importerPviSink.PviXInitialize())
            {
                return (false);
            }
            if (lastConnectionErrorCode != 0)
            {
                return (false);
            }

            // Set the callback functions for receiving notifications for the global events POBJ_EVENT_PVI_CONNECT,
            // POBJ_EVENT_PVI_DISCONN and POBJ_EVENT_PVI_ARRANGE
            if (!importerPviSink.PviXSetGlobEventMsg())
            {
                return (false);
            }
            if (lastConnectionErrorCode != 0)
            {
                return (false);
            }

            return (true);
        }

        void DeallocateGlobalHandles()
        {
            if (gcHandleImporter.IsAllocated)
            {
                gcHandleImporter.Free();
            }
            if (gcHandleConnectCallbackFunc.IsAllocated)
            {
                gcHandleConnectCallbackFunc.Free();
            }
            if (gcHandleDisconnectCallbackFunc.IsAllocated)
            {
                gcHandleDisconnectCallbackFunc.Free();
            }
            if (gcHandleArrangeCallbackFunc.IsAllocated)
            {
                gcHandleArrangeCallbackFunc.Free();
            }
            if (gcHandleDataCallbackFunc.IsAllocated)
            {
                gcHandleDataCallbackFunc.Free();
            }

            EmptyGCHandleDictionary();
        }

        void EmptyGCHandleDictionary()
        {
            lock (lockmapPviObjects)
            {
                Dictionary<string, GCHandle>.ValueCollection valueColl = mapGlobalHandles.Values;
                foreach (GCHandle handle in valueColl)
                {
                    if (handle.IsAllocated)
                    {
                        handle.Free();
                    }
                }
                mapGlobalHandles.Clear();
            }
        }

        bool DeviceClose()
        {
            if (pviCommunicationManager.IsInitialized())
            {
                if (!importerPviSink.PviXDeinitialize() || (lastConnectionErrorCode != 0))
                {
                    DeallocateGlobalHandles();
                    return (false);
                }
            }

            DeallocateGlobalHandles();

            return true;
        }

        public void CreateStationPviObjects()
        {
            lock (lockmapPviObjects)
            {
                pendingCreateRequests.Clear();
            }

            // Line
            lastConnectionErrorCode = pviCommunicationManager.PviXCreateRequest(intptrDataCallback, pviLine, GCHandle.ToIntPtr(gcHandlePviLineObj));
            if (lastConnectionErrorCode != 0)
            {
                lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorPviXCreate, lastConnectionErrorCode, pviLine.Name);
                //System.Diagnostics.Debug.WriteLine("BR_DEBUG -ImportTagsEditorTree.CreateStationPviObjects {0} - return code of PviComManager.PviXCreateRequest for {1}: {2}",
                //                                   DateTime.Now.ToString("HH:mm:ss.fff"), pviLine.Name, lastConnectionErrorCode);
                return;
            }
            lock (lockmapPviObjects)
            {
                // List of pending PVI object create requests
                if (!pendingCreateRequests.Contains(pviLine.Name))
                {
                    pendingCreateRequests.Add(pviLine.Name);
                }
            }

            // Device
            lastConnectionErrorCode = pviCommunicationManager.PviXCreateRequest(intptrDataCallback, pviDevice, GCHandle.ToIntPtr(gcHandlePviDeviceObj));
            if (lastConnectionErrorCode != 0)
            {
                lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorPviXCreate, lastConnectionErrorCode, pviDevice.Name);
                //System.Diagnostics.Debug.WriteLine("BR_DEBUG -ImportTagsEditorTree.CreateStationPviObjects {0} - return code of PviComManager.PviXCreateRequest for {1}: {2}",
                //                                   DateTime.Now.ToString("HH:mm:ss.fff"), pviDevice.Name, lastConnectionErrorCode);
                return;
            }
            lock (lockmapPviObjects)
            {
                // List of pending PVI object create requests
                if (!pendingCreateRequests.Contains(pviDevice.Name))
                {
                    pendingCreateRequests.Add(pviDevice.Name);
                }
            }

            // Station
            lastConnectionErrorCode = pviCommunicationManager.PviXCreateRequest(intptrDataCallback, pviStation, GCHandle.ToIntPtr(gcHandlePviStationObj));
            if (lastConnectionErrorCode != 0)
            {
                lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorPviXCreate, lastConnectionErrorCode, pviStation.Name);
                //System.Diagnostics.Debug.WriteLine("BR_DEBUG -ImportTagsEditorTree.CreateStationPviObjects {0} - return code of PviComManager.PviXCreateRequest for {1}: {2}",
                //                                   DateTime.Now.ToString("HH:mm:ss.fff"), pviStation.Name, lastConnectionErrorCode);
                return;
            }
            lock (lockmapPviObjects)
            {
                // Dictionary of pending PVI object create requests
                if (!pendingCreateRequests.Contains(pviStation.Name))
                {
                    pendingCreateRequests.Add(pviStation.Name);
                }
            }

            // Cpu
            lastConnectionErrorCode = pviCommunicationManager.PviXCreateRequest(intptrDataCallback, pviCpu, GCHandle.ToIntPtr(gcHandlePviCpuObj));
            if (lastConnectionErrorCode != 0)
            {
                lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorPviXCreate, lastConnectionErrorCode, pviCpu.Name);
                //System.Diagnostics.Debug.WriteLine("BR_DEBUG -ImportTagsEditorTree.CreateStationPviObjects {0} - return code of PviComManager.PviXCreateRequest for {1}: {2}",
                //                                   DateTime.Now.ToString("HH:mm:ss.fff"), pviCpu.Name, lastConnectionErrorCode);
            }
            lock (lockmapPviObjects)
            {
                // Dictionary of pending PVI object create requests
                if (!pendingCreateRequests.Contains(pviCpu.Name))
                {
                    pendingCreateRequests.Add(pviCpu.Name);
                }
            }
        }

        bool CreatePlcPviObjects()
        {
            if (!dictionaryOfStationSettings.ContainsKey(selectedStation))
            {
                lastConnectionErrorCode = -1;
                lastConnectionErrorMessage = Properties.Resources.ImportEnterStationName;
                return (false);
            }

            // Build the PVI objects
            BrPviStationSettings brStationSettings;
            if (!dictionaryOfStationSettings.TryGetValue(selectedStation, out brStationSettings) || (brStationSettings == null))
            {
                lastConnectionErrorCode = -1;
                lastConnectionErrorMessage = Properties.Resources.ImportEnterStationName;
                return (false);
            }

            // Line
            if (gcHandlePviLineObj.IsAllocated)
            {
                gcHandlePviLineObj.Free();
            }
            if (pviLine != null)
            {
                pviLine = null;
            }
            pviLine = new BrPviPviObject(brStationSettings.Name);
            pviLine.Name = "@Pvi/LNINA2";
            pviLine.ConnDescr = "CD=LnIna2";
            pviLine.LinkDescr = "EV=";
            pviLine.Type = PviObjectTypes.POBJ_LINE;
            lock (lockmapPviObjects)
            {
                mapPviObjects[pviLine.Name] = pviLine;
                gcHandlePviLineObj = GCHandle.Alloc(pviLine);
                mapGlobalHandles[pviLine.Name] = gcHandlePviLineObj;
            }

            // Device
            if (gcHandlePviDeviceObj.IsAllocated)
            {
                gcHandlePviDeviceObj.Free();
            }
            if (pviDevice != null)
            {
                pviDevice = null;
            }
            pviDevice = new BrPviPviObject(brStationSettings.Name);
            gcHandlePviDeviceObj = GCHandle.Alloc(pviDevice);
            // Ethernet
            if (brStationSettings.BrPviInterfaceType == BrPviProcol.InterfaceTypes.Ethernet)
            {
                pviDevice.Name = String.Format("{0}/DEV{1}", pviLine.Name, brStationSettings.BrPviSourceStationID);
                pviDevice.ConnDescr = String.Format("CD=\"/IF=tcpip /LOPO={0} /SA={1}\"",
                                                    brStationSettings.BrPviSourceStationPort, brStationSettings.BrPviSourceStationID);
            }
            // Serial
            else
            {
                pviDevice.Name = String.Format("{0}/COM{1}", pviLine.Name, brStationSettings.BrPviSerialPort);
                pviDevice.ConnDescr = String.Format("CD=\"/IF=com{0} /BD={1}",
                                                    brStationSettings.BrPviSerialPort, brStationSettings.BrPviSerialBaudrate);
                switch (brStationSettings.BrPviSerialFlowControl)
                {
                    case (int)BrPviProcol.FlowControlTypes.None:
                        pviDevice.ConnDescr += String.Format(" /RS=-1");
                        break;
                    case (int)BrPviProcol.FlowControlTypes.RtsOff:
                        pviDevice.ConnDescr += String.Format(" /RS=0");
                        break;
                    case (int)BrPviProcol.FlowControlTypes.RS422Mode:
                        pviDevice.ConnDescr += String.Format(" /RS=422");
                        break;
                    default: // FlowControlTypes.RS232Mode
                        pviDevice.ConnDescr += String.Format(" /RS=232");
                        break;
                }
                pviDevice.ConnDescr += String.Format(" /PA={0}\"", brStationSettings.BrPviSerialParity);
            }
            pviDevice.LinkDescr = "EV=";
            pviDevice.Type = PviObjectTypes.POBJ_DEVICE;
            lock (lockmapPviObjects)
            {
                mapPviObjects[pviDevice.Name] = pviDevice;
                mapGlobalHandles[pviDevice.Name] = gcHandlePviDeviceObj;
            }

            // Station
            if (gcHandlePviStationObj.IsAllocated)
            {
                gcHandlePviStationObj.Free();
            }
            if (pviStation != null)
            {
                pviStation = null;
            }
            pviStation = new BrPviPviObject(brStationSettings.Name);
            gcHandlePviStationObj = GCHandle.Alloc(pviStation);
            pviStation.Name = String.Format("{0}/Station", pviDevice.Name);
            pviStation.ConnDescr = "CD=NA";
            pviStation.LinkDescr = "EV=";
            pviStation.Type = PviObjectTypes.POBJ_STATION;
            lock (lockmapPviObjects)
            {
                mapPviObjects[pviStation.Name] = pviStation;
                mapGlobalHandles[pviStation.Name] = gcHandlePviStationObj;
            }

            // CPU
            if (gcHandlePviCpuObj.IsAllocated)
            {
                gcHandlePviCpuObj.Free();
            }
            if (pviCpu != null)
            {
                pviCpu = null;
            }
            pviCpu = new BrPviPviObject(brStationSettings.Name);
            gcHandlePviCpuObj = GCHandle.Alloc(pviCpu);
            int spaceIndex = brStationSettings.Name.IndexOf(" ");
            if (spaceIndex < 0)
            {
                pviCpu.Name = String.Format("{0}/{1}", pviStation.Name, brStationSettings.Name);
            }
            else
            {
                pviCpu.Name = String.Format("{0}/\"{1}\"", pviStation.Name, brStationSettings.Name);
            }
            pviCpu.ConnDescr = "CD=\"";
            if (!String.IsNullOrWhiteSpace(brStationSettings.BrPviRoutingPath))
            {
                pviCpu.ConnDescr += String.Format("/CN={0} ", brStationSettings.BrPviRoutingPath);
            }
            if (brStationSettings.BrPviInterfaceType == BrPviProcol.InterfaceTypes.Ethernet)
            {
                pviCpu.ConnDescr += String.Format("/DA={0} ", brStationSettings.BrPviDestinationStationID);
                if (!String.IsNullOrWhiteSpace(brStationSettings.BrPviDestinationStationIPAddress))
                {
                    pviCpu.ConnDescr += String.Format("/DAIP={0} ", brStationSettings.BrPviDestinationStationIPAddress);
                }
                pviCpu.ConnDescr += String.Format("/REPO={0} ", brStationSettings.BrPviDestinationStationPort);
            }
            pviCpu.ConnDescr += String.Format("/RT={0}\"", brStationSettings.BrPviResponseTimeout);
            pviCpu.LinkDescr = "EV=";
            pviCpu.Type = PviObjectTypes.POBJ_CPU;
            lock (lockmapPviObjects)
            {
                mapPviObjects[pviCpu.Name] = pviCpu;
                mapGlobalHandles[pviCpu.Name] = gcHandlePviCpuObj;
            }

            if (!importerPviSink.CreateStationPviObjects())
            {
                return (false);
            }
            if (lastConnectionErrorCode != 0)
            {
                return (false);
            }

            return (true);
        }

        public void TaskListRequest()
        {
            lastConnectionErrorCode = 0;
            lock (lockmapPviObjects)
            {
                pendingReadRequests.Clear();
            }
            lastConnectionErrorCode = pviCommunicationManager.PviXReadRequest(intptrDataCallback, pviCpu, PviComManager.POBJ_ACC_LIST_TASK, GCHandle.ToIntPtr(gcHandlePviCpuObj));
            if (lastConnectionErrorCode != 0)
            {
                lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorReadingTaskList, lastConnectionErrorCode, pviCpu.Name);
            }
            else
            {
                lock (lockmapPviObjects)
                {
                    // Dictionary of pending PVI object read requests
                    if (!pendingReadRequests.Contains(pviCpu.Name))
                    {
                        pendingReadRequests.Add(pviCpu.Name);
                    }
                }
            }
        }

        bool ReadTaskList()
        {
            if (!importerPviSink.TaskListRequest())
            {
                return (false);
            }
            if (lastConnectionErrorCode != 0)
            {
                return (false);
            }

            return (true);
        }

        public void CreateTaskObjects()
        {
            lastConnectionErrorCode = 0;
            lock (lockmapPviObjects)
            {
                pendingCreateRequests.Clear();
            }

            foreach (BrPviPviObject pviObj in taskPviObjects)
            {
                GCHandle handle;
                if (mapGlobalHandles.TryGetValue(pviObj.Name, out handle))
                {
                    lastConnectionErrorCode = pviCommunicationManager.PviXCreateRequest(intptrDataCallback, pviObj, GCHandle.ToIntPtr(handle));
                    //System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportTagsEditorTree.CreateTaskObjects {0} - return code of PviComManager.PviXCreateRequest for {1}: {2}",
                    //                                   DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, lastConnectionErrorCode);
                    lock (lockmapPviObjects)
                    {
                        // List of pending PVI object create requests
                        if (!pendingCreateRequests.Contains(pviObj.Name))
                        {
                            pendingCreateRequests.Add(pviObj.Name);
                        }
                    }
                }
            }
        }

        bool CreatePviTaskObjects(string stationName)
        {
            // Any task present in the CPU?
            if ((pviTaskNames == null) || (pviTaskNames.Count() == 0))
            {
                // Nothing to do
                return (true);
            }

            // Build the task PVI objects
            foreach (string taskname in pviTaskNames)
            {
                BrPviPviObject taskObj = new BrPviPviObject(stationName);
                int spaceIndex = taskname.IndexOf(" ");
                if (spaceIndex < 0)
                {
                    taskObj.Name = String.Format("{0}/{1}", pviCpu.Name, taskname);
                    taskObj.ConnDescr = String.Format("CD={0}", taskname);
                }
                else
                {
                    taskObj.Name = String.Format("{0}/\"{1}\"", pviCpu.Name, taskname);
                    taskObj.ConnDescr = String.Format("CD=\"{0}\"", taskname);
                }
                taskObj.LinkDescr = "EV=";
                taskObj.Type = PviObjectTypes.POBJ_TASK;
                taskObj.Status = PviObjectStatus.MustBeCreated;
                lock (lockmapPviObjects)
                {
                    mapPviObjects[taskObj.Name] = taskObj;
                    if (!taskPviObjects.Contains(taskObj))
                    {
                        taskPviObjects.Add(taskObj);
                    }
                    GCHandle handle;
                    handle = GCHandle.Alloc(taskObj);
                    mapGlobalHandles[taskObj.Name] = handle;
                }
            }

            if (!importerPviSink.CreatePviTaskObjects())
            {
                return (false);
            }
            if (lastConnectionErrorCode != 0)
            {
                return (false);
            }

            return (true);
        }

        public void VarTypeRequest()
        {
            lastConnectionErrorCode = 0;
            lock (lockmapPviObjects)
            {
                pendingReadRequests.Clear();
            }

            foreach (BrPviPviObject pviObj in variablePviObjects)
            {
                GCHandle handle;
                BrPviPviObject pviObject;
                if (mapGlobalHandles.TryGetValue(pviObj.Name, out handle) && mapPviObjects.TryGetValue(pviObj.Name, out pviObject))
                {
                    lastConnectionErrorCode = pviCommunicationManager.PviXReadRequest(intptrDataCallback, pviObject, PviComManager.POBJ_ACC_TYPE_EXTERN, GCHandle.ToIntPtr(handle));
                    if (lastConnectionErrorCode != 0)
                    {
                        lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorReadVariableList, lastConnectionErrorCode, pviObj.Name);
                        return;
                    }
                    lock (lockmapPviObjects)
                    {
                        // Dictionary of pending PVI object read requests
                        if (!pendingReadRequests.Contains(pviObj.Name))
                        {
                            pendingReadRequests.Add(pviObj.Name);
                        }
                    }
                }
            }
        }

        bool ReadTypeInfo()
        {
            if (!importerPviSink.VarTypeRequest())
            {
                return (false);
            }
            if (lastConnectionErrorCode != 0)
            {
                return (false);
            }

            return (true);
        }

        public void RequestVariableInfo()
        {
            lastConnectionErrorCode = 0;
            lock (lockmapPviObjects)
            {
                pendingReadRequests.Clear();
            }

            // Global (CPU) Variables
            lastConnectionErrorCode = pviCommunicationManager.PviXReadRequest(intptrDataCallback, pviCpu, PviComManager.POBJ_ACC_LIST_PVAR, GCHandle.ToIntPtr(gcHandlePviCpuObj));
            if (lastConnectionErrorCode != 0)
            {
                lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorReadVariableList, lastConnectionErrorCode, pviCpu.Name);
                return;
            }
            lock (lockmapPviObjects)
            {
                // Dictionary of pending PVI object read requests
                if (!pendingReadRequests.Contains(pviCpu.Name))
                {
                    pendingReadRequests.Add(pviCpu.Name);
                }
            }

            // Local (Task) variables
            foreach (BrPviPviObject pviTaskObj in taskPviObjects)
            {
                GCHandle handle;
                if (mapGlobalHandles.TryGetValue(pviTaskObj.Name, out handle))
                {
                    lastConnectionErrorCode = pviCommunicationManager.PviXReadRequest(intptrDataCallback, pviTaskObj, PviComManager.POBJ_ACC_LIST_PVAR, GCHandle.ToIntPtr(handle));
                    if (lastConnectionErrorCode != 0)
                    {
                        lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorReadVariableList, lastConnectionErrorCode, pviTaskObj.Name);
                        return;
                    }
                    lock (lockmapPviObjects)
                    {
                        // Dictionary of pending PVI object read requests
                        if (!pendingReadRequests.Contains(pviTaskObj.Name))
                        {
                            pendingReadRequests.Add(pviTaskObj.Name);
                        }
                    }
                }
            }
        }

        bool ReadVariableInfo()
        {
            if (!importerPviSink.ReadVariableInfo())
            {
                return (false);
            }
            if (lastConnectionErrorCode != 0)
            {
                return (false);
            }

            return (true);
        }

        void EliminateDuplications(string[] globalVariables, string[] allVariables, out string[] localVariables)
        {
            localVariables = (from s in allVariables
                              where (globalVariables.Contains(s) == false)
                              select s).ToArray();
        }

        void EliminateDuplicateVariableDefinitions()
        {
            lock (lockmapPviObjects)
            {
                string[] globalVariables;
                if (!mapPviVariables.TryGetValue(pviCpu.Name, out globalVariables))
                {
                    return;
                }

                // The arrays of variable definitions of Tasks contain also the global variables, so
                // we need to remove them
                Dictionary<string, string[]> mapPviVariablesCopy = new Dictionary<string, string[]>();
                foreach (KeyValuePair<string, string[]> item in mapPviVariables)
                {
                    //System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportTagsEditorTree.EliminateDuplicateVariableDefinitions {0} - Count of Variables of {1} with duplications: {2}",
                    //                                   DateTime.Now.ToString("HH:mm:ss.fff"), item.Key, item.Value.Count());
                    if (item.Key != pviCpu.Name)
                    {
                        string[] localVariables;
                        EliminateDuplications(globalVariables, item.Value, out localVariables);
                        mapPviVariablesCopy[item.Key] = localVariables;
                    }
                    else
                    {
                        mapPviVariablesCopy[item.Key] = item.Value;
                    }
                    //System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportTagsEditorTree.EliminateDuplicateVariableDefinitions {0} - Count of Variables of {1} without duplications: {2}",
                    //                                   DateTime.Now.ToString("HH:mm:ss.fff"), item.Key, mapPviVariablesCopy[item.Key].Count());
                }
                mapPviVariables = mapPviVariablesCopy;
            }
        }

        public void SendCreateVarObjects()
        {
            lastConnectionErrorCode = 0;
            lock (lockmapPviObjects)
            {
                pendingCreateRequests.Clear();
            }

            foreach (BrPviPviObject pviObj in variablePviObjects)
            {
                GCHandle handle;
                if (mapGlobalHandles.TryGetValue(pviObj.Name, out handle))
                {
                    lastConnectionErrorCode = pviCommunicationManager.PviXCreateRequest(intptrDataCallback, pviObj, GCHandle.ToIntPtr(handle));
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportTagsEditorTree.SendCreateVarObjects {0} - return code of PviComManager.PviXCreateRequest for {1} handle {2}: {3}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, GCHandle.ToIntPtr(handle), lastConnectionErrorCode);
                    lock (lockmapPviObjects)
                    {
                        // List of pending PVI object create requests
                        if (!pendingCreateRequests.Contains(pviObj.Name))
                        {
                            pendingCreateRequests.Add(pviObj.Name);
                        }
                    }
                }
            }
        }

        bool CreateVariableObjects(string stationName)
        {
            foreach (KeyValuePair<string, string[]> item in mapPviVariables)
            {
                foreach (string varDefinition in item.Value)
                {
                    // Parse the variable name
                    int spaceIndex = varDefinition.IndexOf(" ");
                    if (spaceIndex > 0)
                    {
                        string varName = varDefinition.Substring(0, spaceIndex);

                        // Build a PVI object corresponding to the variable:
                        // it will be used to get the type info.
                        BrPviPviObject pviObj = new BrPviPviObject(stationName);
                        pviObj.Name = String.Format("{0}/{1}", item.Key, varName);
                        pviObj.ConnDescr = String.Format("CD=\"{0}\"", varName);
                        pviObj.LinkDescr = "EV=";
                        pviObj.Type = PviObjectTypes.POBJ_PVAR;
                        pviObj.Status = PviObjectStatus.MustBeCreated;
                        lock (lockmapPviObjects)
                        {
                            mapPviObjects[pviObj.Name] = pviObj;
                            if (!variablePviObjects.Contains(pviObj))
                            {
                                variablePviObjects.Add(pviObj);
                                mapPviVarTypes[pviObj.Name] = String.Empty;
                            }
                            GCHandle handle;
                            handle = GCHandle.Alloc(pviObj);
                            mapGlobalHandles[pviObj.Name] = handle;
                            System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportTagsEditorTree.CreateVariableObjects {0} - added to mapGlobalHandles (n. elem. {1}) PVI obj {2} with handle {3}",
                                                               DateTime.Now.ToString("HH:mm:ss.fff"), mapGlobalHandles.Count, pviObj.Name, GCHandle.ToIntPtr(handle));
                        }
                    }
                }
            }
            if (!importerPviSink.CreatePviVariableObjects())
            {
                return (false);
            }
            if (lastConnectionErrorCode != 0)
            {
                return (false);
            }
            return (true);
        }

        enum WorkingThreadStatus
        {
            InitConnectionToPviManager,
            WaitConnectionToPviManager,
            InitCreatePlcObjects,
            WaitCreatePlcObjects,
            InitTaskList,
            WaitTaskList,
            InitCreateTaskObjects,
            WaitCreateTaskObjects,
            InitVariableInfo,
            WaitVariableInfo,
            InitCreateVariableObjects,
            WaitCreateVariableObjects,
            InitTypeInfo,
            WaitTypeInfo
        }

        void WorkingThread(object data)
        {
            connectionStatus = BrPviProcol.BrPviConnectionStatus.NotInitialized;
            int sleepCycle = 10;
            DateTime startTime = DateTime.UtcNow;
            WorkingThreadStatus currentStatus = WorkingThreadStatus.InitConnectionToPviManager;
            bool doLoop = true;
            while (doLoop)
            {
                if (!ManageGlobalEvents() || !ManagePviEvents())
                {
                    // Communication error --> Terminate the thread
                    DeviceClose();
                    break;
                }

                switch (currentStatus)
                {
                    case WorkingThreadStatus.InitConnectionToPviManager:
                        // Send the connection request to the PVI Manager
                        if (DeviceOpen())
                        {
                            currentStatus = WorkingThreadStatus.WaitConnectionToPviManager;
                            startTime = DateTime.UtcNow;
                        }
                        else
                        {
                            // Error --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorConnection;
                            }
                            doLoop = false;
                        }
                        break;

                    case WorkingThreadStatus.WaitConnectionToPviManager:
                        if (connectionStatus == BrPviProcol.BrPviConnectionStatus.Arranged)
                        {
                            currentStatus = WorkingThreadStatus.InitCreatePlcObjects;
                        }
                        else if ((DateTime.UtcNow - startTime).TotalMilliseconds > communicationTimeout)
                        {
                            // Timeout elapsed --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorTimeoutConnection;
                            }
                            doLoop = false;
                        }
                        break;

                    case WorkingThreadStatus.InitCreatePlcObjects:
                        // Create the PVI objects to establish the connection to the PLC
                        if (CreatePlcPviObjects())
                        {
                            currentStatus = WorkingThreadStatus.WaitCreatePlcObjects;
                            startTime = DateTime.UtcNow;
                        }
                        else
                        {
                            // Error --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorCreatePLCObjects;
                            }
                            doLoop = false;
                        }
                        break;

                    case WorkingThreadStatus.WaitCreatePlcObjects:
                        // All requested Pvi Objects have been created?
                        if (pendingCreateRequests.Count == 0)
                        {
                            currentStatus = WorkingThreadStatus.InitTaskList;
                        }
                        else if ((DateTime.UtcNow - startTime).TotalMilliseconds > communicationTimeout)
                        {
                            // Timeout elapsed --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorTimeoutCreatePLCObjects;
                            }
                            doLoop = false;
                        }
                        break;

                    case WorkingThreadStatus.InitTaskList:
                        if (ReadTaskList())
                        {
                            currentStatus = WorkingThreadStatus.WaitTaskList;
                            startTime = DateTime.UtcNow;
                        }
                        else
                        {
                            // Error --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorReadTasks;
                            }
                            doLoop = false;
                        }
                        break;

                    case WorkingThreadStatus.WaitTaskList:
                        // All requested data have been received?
                        if (pendingReadRequests.Count == 0)
                        {
                            currentStatus = WorkingThreadStatus.InitCreateTaskObjects;
                        }
                        else if ((DateTime.UtcNow - startTime).TotalMilliseconds > communicationTimeout)
                        {
                            // Timeout elapsed --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorTimeoutReadTasks;
                            }
                            doLoop = false;
                        }
                        break;

                    case WorkingThreadStatus.InitCreateTaskObjects:
                        if (CreatePviTaskObjects(selectedStation.ToString()))
                        {
                            currentStatus = WorkingThreadStatus.WaitCreateTaskObjects;
                            startTime = DateTime.UtcNow;
                        }
                        else
                        {
                            // Error --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorCreateTaskObjects;
                            }
                            doLoop = false;
                        }
                        break;

                    case WorkingThreadStatus.WaitCreateTaskObjects:
                        // All requested Pvi Objects have been created?
                        if (pendingCreateRequests.Count == 0)
                        {
                            currentStatus = WorkingThreadStatus.InitVariableInfo;
                        }
                        else if ((DateTime.UtcNow - startTime).TotalMilliseconds > communicationTimeout)
                        {
                            // Timeout elapsed --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorTimeoutCreateTaskObjects;
                            }
                            doLoop = false;
                        }
                        break;

                    case WorkingThreadStatus.InitVariableInfo:
                        if (ReadVariableInfo())
                        {
                            currentStatus = WorkingThreadStatus.WaitVariableInfo;
                            startTime = DateTime.UtcNow;
                        }
                        else
                        {
                            // Error --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorReadVarList;
                            }
                            doLoop = false;
                        }
                        break;

                    case WorkingThreadStatus.WaitVariableInfo:
                        // All requested data have been received?
                        if (pendingReadRequests.Count == 0)
                        {
                            // Eliminate duplicates variable definitions
                            EliminateDuplicateVariableDefinitions();
                            currentStatus = WorkingThreadStatus.InitCreateVariableObjects;
                        }
                        else if ((DateTime.UtcNow - startTime).TotalMilliseconds > communicationTimeout)
                        {
                            // Timeout elapsed --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorTimeoutReadVarList;
                            }
                            doLoop = false;
                        }
                        break;

                    case WorkingThreadStatus.InitCreateVariableObjects:
                        if (CreateVariableObjects(selectedStation.ToString()))
                        {
                            currentStatus = WorkingThreadStatus.WaitCreateVariableObjects;
                            startTime = DateTime.UtcNow;
                        }
                        else
                        {
                            // Error --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorCreateVariableObjects;
                            }
                            doLoop = false;
                        }
                        break;

                    case WorkingThreadStatus.WaitCreateVariableObjects:
                        // All requested data have been received?
                        if (pendingCreateRequests.Count == 0)
                        {
                            currentStatus = WorkingThreadStatus.InitTypeInfo;
                        }
                        else if ((DateTime.UtcNow - startTime).TotalMilliseconds > (communicationTimeout * 3))
                        {
                            // Timeout elapsed --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorTimeoutCreateVariableObjects;
                            }
                            doLoop = false;
                        }
                        break;

                    case WorkingThreadStatus.InitTypeInfo:
                        if (ReadTypeInfo())
                        {
                            currentStatus = WorkingThreadStatus.WaitTypeInfo;
                            startTime = DateTime.UtcNow;
                        }
                        else
                        {
                            // Error --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorReadVarTypeInfo;
                            }
                            doLoop = false;
                        }
                        break;

                    case WorkingThreadStatus.WaitTypeInfo:
                        // All requested data have been received?
                        if (pendingReadRequests.Count == 0)
                        {
                            // Data acquisition successfully completed --> Terminate the thread
                            DeviceClose();
                            doLoop = false;
                        }
                        else if ((DateTime.UtcNow - startTime).TotalMilliseconds > (communicationTimeout * 3))
                        {
                            // Timeout elapsed --> Terminate the thread
                            DeviceClose();
                            if (lastConnectionErrorCode == 0)
                            {
                                lastConnectionErrorCode = -1;
                                lastConnectionErrorMessage = Properties.Resources.ImportErrorReadVarTypeInfo;
                            }
                            doLoop = false;
                        }
                        break;
                }

                // This event is never set. It is used just for making a short pause 
                if (StopCommunicationThread.WaitOne(sleepCycle))
                {
                    break;
                }
            }

            EmptyAusiliaryListsAndMapsForDirectImport();
        }

        void EmptyAusiliaryListsAndMapsForDirectImport()
        {
            lock (lockmapPviObjects)
            {
                taskPviObjects.Clear();
                variablePviObjects.Clear();
                pviTaskNames = null;
                pendingCreateRequests.Clear();
                pendingReadRequests.Clear();
            }

            lock (pviEventLockObject)
            {
                listOfPviEvents.Clear();
            }
        }

        void EmptyListsAndMapsForDirectImport()
        {
            lock (lockmapPviObjects)
            {
                mapPviObjects.Clear();
                mapPviVarTypes.Clear();
                mapPviVariables.Clear();
            }
        }

        bool ManageGlobalEvents()
        {
            // Make a local copy of the received global events
            List<PviEventInfo> listOfGlobalEventsCopy = null;
            lock (pviEventLockObject)
            {
                if (listOfGlobalEvents.Count > 0)
                {
                    listOfGlobalEventsCopy = new List<PviEventInfo>(listOfGlobalEvents);
                    listOfGlobalEvents.Clear();
                }
            }
            if (listOfGlobalEventsCopy != null)
            {
                foreach (var info in listOfGlobalEventsCopy)
                {
                    PviEventInfo eventInfo = info;
                    //System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportTagsEditorTree.ManageGlobalEvents {0} - Received event {1} with Error Code {2}",
                    //                                   DateTime.Now.ToString("HH:mm:ss.fff"), eventInfo.nType, eventInfo.ErrCode);
                    switch (eventInfo.nType)
                    {
                        case PviComManager.POBJ_EVENT_PVI_CONNECT:
                            if (eventInfo.ErrCode == 0) // Connection OK
                            {
                                lastConnectionErrorCode = 0;
                                if (connectionStatus != BrPviProcol.BrPviConnectionStatus.Arranged)
                                {
                                    connectionStatus = BrPviProcol.BrPviConnectionStatus.Connected;
                                }
                            }
                            else // Connection error
                            {
                                lastConnectionErrorCode = (int)eventInfo.ErrCode;
                                lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorConnectionBroken, lastConnectionErrorCode);
                                return (false);
                            }
                            break;

                        case PviComManager.POBJ_EVENT_PVI_DISCONN:
                            lastConnectionErrorCode = (int)eventInfo.ErrCode;
                            lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorConnectionBroken, lastConnectionErrorCode);
                            return (false);

                        case PviComManager.POBJ_EVENT_PVI_ARRANGE:
                            if (eventInfo.ErrCode == 0) // Connection OK
                            {
                                lastConnectionErrorCode = 0;
                                connectionStatus = BrPviProcol.BrPviConnectionStatus.Arranged;
                            }
                            else // Connection error
                            {
                                lastConnectionErrorCode = (int)eventInfo.ErrCode;
                                lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorConnectionBroken, lastConnectionErrorCode);
                                return (false);
                            }
                            break;
                    }
                }
            }

            return (true);
        }

        const uint POBJ_EVENT_ERROR = 3;

        bool ManagePviEvents()
        {
            List<PviObjEvent> listOfPviEventsCopy = null;
            lock (pviEventLockObject)
            {
                if (listOfPviEvents.Count > 0)
                {
                    listOfPviEventsCopy = new List<PviObjEvent>(listOfPviEvents);
                    listOfPviEvents.Clear();
                }
            }

            //try
            //{
            lock (lockmapPviObjects)
            {
                if (listOfPviEventsCopy != null)
                {
                    foreach (var eventObj in listOfPviEventsCopy)
                    {
                        PviObjEvent pviEvent = eventObj;
                        //System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportTagsEditorTree.ManagePviEvents {0} - Received event for PVI Obj: {1} nMode: {2} nType: {3} ErrCode: {4}",
                        //                                   DateTime.Now.ToString("HH:mm:ss.fff"), pviEvent.PviObj.Name, pviEvent.EventInfo.nMode, pviEvent.EventInfo.nType, pviEvent.EventInfo.ErrCode);
                        BrPviPviObject pviObj;
                        if (mapPviObjects.TryGetValue(pviEvent.PviObj.Name, out pviObj))
                        {
                            switch (pviEvent.EventInfo.nMode)
                            {
                                case (uint)PviObjectModes.POBJ_MODE_CREATE:
                                    // Remove the Pvi Object entry in the dictionary of pending create requests
                                    if (pendingCreateRequests.Contains(pviObj.Name))
                                    {
                                        pendingCreateRequests.Remove(pviObj.Name);
                                    }

                                    // Error?
                                    if (pviEvent.EventInfo.ErrCode != 0)
                                    {
                                        pendingCreateRequests.Clear();
                                        lastConnectionErrorCode = (int)pviEvent.EventInfo.ErrCode;
                                        lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorPviXCreate, lastConnectionErrorCode, pviObj.Name);
                                        return (false);
                                    }

                                    // PVI object successfully created
                                    else
                                    {
                                        lastConnectionErrorCode = 0;
                                        pviObj.LastError = 0;
                                        pviObj.LinkID = pviEvent.EventInfo.LinkID;
                                        pviObj.Status = PviObjectStatus.Ready;
                                    }
                                    break;

                                case (uint)PviObjectModes.POBJ_MODE_READ:
                                    // Remove the Pvi Object entry in the dictionary of read requests
                                    if (pendingReadRequests.Contains(pviObj.Name))
                                    {
                                        pendingReadRequests.Remove(pviObj.Name);
                                    }

                                    // Error?
                                    if (pviEvent.EventInfo.ErrCode != 0)
                                    {
                                        pendingReadRequests.Clear();
                                        lastConnectionErrorCode = (int)pviEvent.EventInfo.ErrCode;
                                        lastConnectionErrorMessage = String.Format(Properties.Resources.ImportErrorRead, lastConnectionErrorCode, pviObj.Name);
                                        return (false);
                                    }
                                    // Data received
                                    else
                                    {
                                        pviObj.LastError = 0;
                                        pviObj.Status = PviObjectStatus.Ready;
                                        lastConnectionErrorCode = 0;
                                        switch (pviEvent.EventInfo.nType)
                                        {
                                            case PviComManager.POBJ_ACC_LIST_TASK:
                                                ParseTaskList(pviEvent.DataArray, pviEvent.DataLength);
                                                break;

                                            case PviComManager.POBJ_ACC_LIST_PVAR:
                                                ParseVariableList(pviEvent.DataArray, pviEvent.DataLength, pviObj);
                                                break;

                                            case PviComManager.POBJ_ACC_TYPE_EXTERN:
                                                ParseVarType(pviEvent.DataArray, pviEvent.DataLength, pviObj);
                                                break;
                                        }
                                    }
                                    break;
                            }
                        }
                    }
                }
            }
            //}
            //catch (Exception ex)
            //{
            //    System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportTagsEditorTree.ManagePviEvents {0} - Exception: {1}",
            //                                       DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
            //}
            return (true);
        }

        void ParseTaskList(byte[] dataArray, uint dataLen)
        {
            if (dataLen == 0)
            {
                return;
            }

            // Make a local copy of received data as a string (eliminate the final byte == 0)
            byte[] dataBuffer = new byte[dataLen - 1];
            Array.Copy(dataArray, dataBuffer, dataLen - 1);
            String dataString = ASCIIEncoding.ASCII.GetString(dataBuffer);
            //System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportTagsEditorTree.ParseTaskList {0} - Received Task List: {1}",
            //                                   DateTime.Now.ToString("HH:mm:ss.fff"), dataString);
            // Split the string for isolating the task names
            char[] splitParameters = new char[1];
            splitParameters[0] = '\t';
            pviTaskNames = dataString.Split(splitParameters);
        }

        void ParseVariableList(byte[] dataArray, uint dataLength, BrPviPviObject pviObj)
        {
            if (dataLength == 0)
            {
                return;
            }

            // Make a local copy of received data as a string (eliminate the final byte == 0)
            byte[] dataBuffer = new byte[dataLength - 1];
            Array.Copy(dataArray, dataBuffer, dataLength - 1);
            String dataString = ASCIIEncoding.ASCII.GetString(dataBuffer);
            //System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportTagsEditorTree.ParseVariableList {0} - PVI Object {1} - Received Variable List: {2}",
            //                                   DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, dataString);

            // Split the string for isolating the single variable definitions
            char[] splitParameters = new char[1];
            splitParameters[0] = '\t';
            string[] variableDefinitions = dataString.Split(splitParameters);

            // Store variable definitions in the corresponding dictionary
            lock (lockmapPviObjects)
            {
                mapPviVariables[pviObj.Name] = variableDefinitions;
            }
        }

        void ParseVarType(byte[] dataArray, uint dataLength, BrPviPviObject pviObj)
        {
            if (dataLength == 0)
            {
                return;
            }

            // Make a local copy of received data as a string (eliminate the final byte == 0)
            byte[] dataBuffer = new byte[dataLength - 1];
            Array.Copy(dataArray, dataBuffer, dataLength - 1);
            String dataString = ASCIIEncoding.ASCII.GetString(dataBuffer);
            //System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportTagsEditorTree.ParseVarType {0} - PVI Object {1} - Received Type definition: {2}",
            //                                   DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, dataString);

            lock (lockmapPviObjects)
            {
                string typeDefinition;
                if (mapPviVarTypes.TryGetValue(pviObj.Name, out typeDefinition))
                {
                    mapPviVarTypes[pviObj.Name] = dataString;
                }
            }
        }

        public void AddGlobalEvent(PviEventInfo globalEventInfo)
        {
            lock (pviEventLockObject)
            {
                listOfGlobalEvents.Add(globalEventInfo);
            }
        }

        public void AddPviEvent(PviObjEvent pviEvent)
        {
            lock (pviEventLockObject)
            {
                listOfPviEvents.Add(pviEvent);
            }
        }
        #endregion

        //private void AddPviStandardVar(string szNamePrefix,
        //                               string szVarName,
        //                               string szAddress,
        //                               string taskName,
        //                               string description,
        //                               string szVarType,
        //                               int moviconType,
        //                               uint nVarSize,
        //                               DriverCodeBase.Enumerators.LinkType jobType,
        //                               int parentId = -1,
        //                               ImportData inRootItem = null)
        //{
        //    ImportData v = importDataModel.addImportData();
        //    v.PreName = szNamePrefix;
        //    v.Name = szVarName;
        //    v.Size = nVarSize;
        //    v.szType = szVarType;
        //    v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
        //    v.Task = taskName;
        //    v.ElemType = szVarType;
        //    v.Type = (DataType)moviconType;
        //    v.Address = szAddress;
        //    v.Description = description;
        //    v.parentId = parentId;
        //    v.Id = m_lGlobalID++;
        //    v.ArrayDimension = 0;
        //    v.ImportDataType = ImportTypes.Standard;
        //    v.JobType = jobType;
        //    AddTreeItem(v, inRootItem);
        //}

        //int GetMoviconType(string varType)
        //{
        //    int moviconType = 0;
        //    if (!SupportedPlcVariableTypes.TryGetValue(varType, out moviconType))
        //    {
        //        return (-1);
        //    }
        //    return (moviconType);
        //}

        void ParsePlcData()
        {
            if (mapPviVarTypes.Count < 1)
            {
                return;
            }

            foreach (KeyValuePair<string, string> item in mapPviVarTypes)
            {
                BrPviPviObject pviObject;
                if (mapPviObjects.TryGetValue(item.Key, out pviObject))
                {
                    AddPlcVariable(pviObject, item.Value);
                }
            }
        }

        void AddPlcVariable(BrPviPviObject pviObject, string typeDefinition)
        {
            // Split the complete name of the PVI Variable Object and
            // set the variable name (mandatory) and the task name (optional)
            char[] splitParameters = new char[1];
            splitParameters[0] = '/';
            string[] completeNameFields = pviObject.Name.Split(splitParameters);
            // @PVI/<Line>/<Device>/Station/<CPU>/<Variable> or
            // @PVI/<Line>/<Device>/Station/<CPU>/<Task>/<Variable>
            if (completeNameFields.Count() < 6)
            {
                return;
            }
            string taskName = String.Empty;
            string variableName = String.Empty;
            if (completeNameFields.Count() == 6)
            {
                variableName = completeNameFields[5];
                if (String.IsNullOrWhiteSpace(variableName))
                {
                    return;
                }
            }
            else
            {
                taskName = completeNameFields[5];
                variableName = completeNameFields[6];
                if (String.IsNullOrWhiteSpace(taskName) || String.IsNullOrWhiteSpace(variableName))
                {
                    return;
                }
            }

            // Parse the variable type (mandatory parameter)
            string varType;
            ParsePviTypeAttribute(typeDefinition, "VT", out varType);
            if (String.IsNullOrWhiteSpace(varType))
            {
                return;
            }
            if (!IsSupportedPlcType(varType))
            {
                return;
            }

            // Parse the variable scope (mandatory parameter)
            string varScope;
            ParsePviTypeAttribute(typeDefinition, "SC", out varScope);
            if (varScope.IndexOf(POBJ_ACC_LIST_PVAR_DYNAMIC) >= 0)
            {
                return;
            }

            // Parse the access type in order to set the job type for this variable
            DriverCodeBaseEx.Enumerators.LinkType jobType = DriverCodeBaseEx.Enumerators.LinkType.InputOutput;
            string accessType;
            ParsePviTypeAttribute(typeDefinition, "AT", out accessType);
            if (!String.IsNullOrWhiteSpace(accessType))
            {
                if (accessType.IndexOf("r") >= 0)
                {
                    if (accessType.IndexOf("w") < 0)
                    {
                        jobType = DriverCodeBaseEx.Enumerators.LinkType.Input;
                    }
                }
                else if (accessType.IndexOf("w") >= 0)
                {
                    jobType = DriverCodeBaseEx.Enumerators.LinkType.ExceptionOutput;
                }
            }

            // Parse the variable length
            string varLength;
            uint variableLength = 1;
            ParsePviTypeAttribute(typeDefinition, "VL", out varLength);
            if (!String.IsNullOrWhiteSpace(varLength))
            {
                uint auxUint = 0;
                if (uint.TryParse(varLength, out auxUint))
                {
                    variableLength = auxUint;
                }
            }

            // Parse the number of elements
            string elemNum;
            uint elementsNumber = 1;
            ParsePviTypeAttribute(typeDefinition, "VN", out elemNum);
            if (!String.IsNullOrWhiteSpace(elemNum))
            {
                uint auxUint = 0;
                if (uint.TryParse(elemNum, out auxUint))
                {
                    elementsNumber = auxUint;
                }
            }

            // Set the first part of the variable name
            string namePrefix = String.Empty;
            if (!String.IsNullOrWhiteSpace(taskName))
            {
                namePrefix = taskName + "_";
            }

            // Structure?
            if (IsPlcStructureType(varType))
            {
                // Parse the structure type (mandatory parameter)
                ParsePviTypeAttribute(typeDefinition, "SN", out varType);
                if (String.IsNullOrWhiteSpace(varType))
                {
                    return;
                }

                // Add the structure variable and its elements to the tree
                AddPviStructureVariable(namePrefix,
                                        taskName,
                                        variableName,
                                        varType,
                                        variableLength,
                                        typeDefinition,
                                        jobType,
                                        elementsNumber);
                return;
            }

            // Array?
            if (elementsNumber > 1)
            {
                // Add the array variable and its elements to the tree
                AddPviArrayVariable(namePrefix,
                                    String.Empty,
                                    taskName,
                                    variableName,
                                    varType,
                                    typeDefinition,
                                    jobType,
                                    elementsNumber,
                                    variableLength);
                return;
            }

            // Standard variable
            // Get the corresponding MOVICON type
            int moviconType = GetMoviconType(varType);
            // Invalid Type?
            if (moviconType == -1)
            {
                return;
            }
            // Add the variable to the tree
            AddPviStandardVar(namePrefix,
                              variableName,
                              variableName,
                              taskName,
                              String.Empty,
                              varType,
                              moviconType,
                              variableLength,
                              jobType,
                              -1,
                              null);
        }

        void AddPviArrayVariable(string namePrefix,
                                 string addressPrefix,
                                 string taskName,
                                 string variableName,
                                 string elementType,
                                 string typeDefinition,
                                 DriverCodeBaseEx.Enumerators.LinkType jobType,
                                 uint elementsNumber,
                                 uint elementSize,
                                 int parentId = -1,
                                 ImportData inRootItem = null)
        {
            // Get the MOVICON type corresponding to the element type
            int moviconType = GetMoviconType(elementType);
            // Invalid Type?
            if (moviconType == -1)
            {
                return;
            }

            // Additional Array info present?
            string additionalInfo;
            ParsePviTypeAttribute(typeDefinition, "VS", out additionalInfo);
            uint arrayDimCount = CountArrayDimensions(additionalInfo);
            uint[] arrayStartIndexes = new uint[arrayDimCount];
            uint[] arrayIndexLimits = new uint[arrayDimCount];
            if (!ParseArrayInfo(additionalInfo, arrayDimCount, elementsNumber, ref arrayStartIndexes, ref arrayIndexLimits))
            {
                return;
            }

            string varScope;
            ParsePviTypeAttribute(typeDefinition, "SC", out varScope);
            if (varScope.IndexOf(POBJ_ACC_LIST_PVAR_DYNAMIC) >= 0)
            {
                return;
            }

            // Add the variable to the tree
            AddPviArray(namePrefix,
                        addressPrefix,
                        taskName,
                        variableName,
                        elementType,
                        moviconType,
                        elementSize,
                        jobType,
                        elementsNumber,
                        arrayDimCount,
                        arrayStartIndexes,
                        arrayIndexLimits,
                        parentId,
                        inRootItem);
        }

        //private void AddPviArray(string szNamePrefix,
        //                         string szAddressPrefix,
        //                         string szTaskName,
        //                         string szArrayName,
        //                         string szElemType,
        //                         int moviconType,
        //                         uint nElemSize,
        //                         DriverCodeBase.Enumerators.LinkType jobType,
        //                         uint elementsNumber,
        //                         uint nDimCount,
        //                         uint[] startIndexes,
        //                         uint[] indexLimits,
        //                         int parentId = -1,
        //                         ImportData inRootItem = null)
        //{
        //    if (nDimCount == 0)
        //    {
        //        return;
        //    }

        //    string szAddress = szAddressPrefix + szArrayName;
        //    // Add the array variable to the tree
        //    ImportData v = importDataModel.addImportData();
        //    v.PreName = szNamePrefix;
        //    v.Name = szArrayName;
        //    v.Size = elementsNumber * nElemSize;
        //    v.szType = "ARRAY OF " + szElemType;
        //    v.szTypeView = GetMoviconTypeFromPlcType(v.szType);
        //    v.Task = szTaskName;
        //    v.ElemType = szElemType;
        //    v.Type = (DataType)moviconType;
        //    v.Address = szAddress;
        //    v.Description = String.Empty;
        //    v.parentId = parentId;
        //    v.Id = m_lGlobalID++;
        //    v.ArrayDimension = elementsNumber;
        //    v.ImportDataType = ImportTypes.Array;
        //    v.JobType = jobType;
        //    AddTreeItem(v, inRootItem);

        //    // Add the array elements to the tree
        //    string varNamePrefix = String.Format("{0}[", szArrayName);
        //    string addrPrefix = String.Format("{0}[", szAddress);
        //    AddArrayElements(szNamePrefix,
        //                     szTaskName,
        //                     varNamePrefix,
        //                     addrPrefix,
        //                     v.Id,
        //                     nDimCount,
        //                     0,
        //                     szElemType,
        //                     moviconType,
        //                     nElemSize,
        //                     jobType,
        //                     startIndexes,
        //                     indexLimits,
        //                     v);
        //}

        //void AddArrayElements(string szNamePrefix,
        //                      string szTaskName,
        //                      string varNamePrefix,
        //                      string addressPrefix,
        //                      int parentId,
        //                      uint nDimCount,
        //                      uint currentDim,
        //                      string szElemType,
        //                      int moviconType,
        //                      uint nElemSize,
        //                      DriverCodeBase.Enumerators.LinkType jobType,
        //                      uint[] startIndexes,
        //                      uint[] indexLimits,
        //                      ImportData inRootItem)
        //{
        //    for (uint i = startIndexes[currentDim]; i < indexLimits[currentDim]; i++)
        //    {
        //        uint RealArrayIndex = i - startIndexes[currentDim];
        //        string szAddress = String.Format("{0}{1}", addressPrefix, RealArrayIndex);
        //        string varName = String.Format("{0}{1}", varNamePrefix, RealArrayIndex);
        //        if (currentDim == (nDimCount - 1))
        //        {
        //            varName += "]";
        //            szAddress += "]";
        //            AddPviStandardVar(szNamePrefix,
        //                              varName,
        //                              szAddress,
        //                              szTaskName,
        //                              String.Empty,
        //                              szElemType,
        //                              moviconType,
        //                              nElemSize,
        //                              jobType,
        //                              parentId,
        //                              inRootItem);
        //        }
        //        else
        //        {
        //            varName += ",";
        //            szAddress += ",";
        //            AddArrayElements(szNamePrefix,
        //                             szTaskName,
        //                             varName,
        //                             szAddress,
        //                             parentId,
        //                             nDimCount,
        //                             currentDim + 1,
        //                             szElemType,
        //                             moviconType,
        //                             nElemSize,
        //                             jobType,
        //                             startIndexes,
        //                             indexLimits,
        //                             inRootItem);
        //        }
        //    }
        //}

        void AddPviStructureVariable(string namePrefix,
                                     string taskName,
                                     string variableName,
                                     string variableType,
                                     uint nVarSize,
                                     string typeDefinition,
                                     DriverCodeBaseEx.Enumerators.LinkType jobType,
                                     uint elementsNumber,
                                     int parentId = -1,
                                     ImportData inRootItem = null)
        {
            // Parse the definitions of the structure elements
            int searchIndex = typeDefinition.IndexOf('{');
            if (searchIndex < 0)
            {
                return;
            }
            string auxString = typeDefinition.Substring(searchIndex);
            char[] splitParameters = new char[1];
            splitParameters[0] = '{';
            string[] structureElements = typeDefinition.Split(splitParameters);
            if (structureElements.Count() < 1)
            {
                return;
            }

            // If needed, add the task name to the type name
            string completeTypeName = String.Empty;
            if (!String.IsNullOrWhiteSpace(taskName))
            {
                completeTypeName = taskName + "_" + variableType;
            }
            else
            {
                completeTypeName = variableType;
            }

            // Simple structure?
            if (elementsNumber == 1)
            {
                // Build the structure variable to be added to the tree
                ImportData v = importDataModel.addImportData();
                ((ImportDataBrPvi)v).PreName = namePrefix;
                v.Name = variableName;
                ((ImportDataBrPvi)v).Size = nVarSize;
                v.szType = completeTypeName;
                ((ImportDataBrPvi)v).szTypeView = GetMoviconTypeFromPlcType(v.szType);
                ((ImportDataBrPvi)v).Task = taskName;
                ((ImportDataBrPvi)v).ElemType = completeTypeName;
                v.Address = variableName;
                v.Description = String.Empty;
                v.parentId = parentId;
                v.Id = m_lGlobalID++;
                v.ArrayDimension = 0;
                ((ImportDataBrPvi)v).ImportDataType = ImportTypes.StructOrEnum;
                ((ImportDataBrPvi)v).JobType = jobType;

                // Add the structure elements to the tree
                string addressPrefix = variableName;
                AddPviStructureElements(namePrefix,
                                        taskName,
                                        addressPrefix,
                                        structureElements,
                                        jobType,
                                        v.Id,
                                        inRootItem,
                                        v);
            }
            // Array of Structures
            else
            {
                // Add the array elements to the tree
                AddPviStructureArray(namePrefix,
                                     variableName,
                                     taskName,
                                     completeTypeName,
                                     structureElements,
                                     jobType,
                                     elementsNumber,
                                     parentId,
                                     inRootItem);
            }
        }

        void AddPviStructureArray(string namePrefix,
                                  string variableName,
                                  string taskName,
                                  string completeTypeName,
                                  string[] structureElements,
                                  DriverCodeBaseEx.Enumerators.LinkType jobType,
                                  uint elementsNumber,
                                  int parentId,
                                  ImportData inRootItem)
        {

            // Additional Array info present?
            string additionalInfo;
            ParsePviTypeAttribute(structureElements[0], "VS", out additionalInfo);
            uint arrayDimCount = CountArrayDimensions(additionalInfo);
            uint[] arrayStartIndexes = new uint[arrayDimCount];
            uint[] arrayIndexLimits = new uint[arrayDimCount];
            if (!ParseArrayInfo(additionalInfo, arrayDimCount, elementsNumber, ref arrayStartIndexes, ref arrayIndexLimits))
            {
                return;
            }

            string varNamePrefix = String.Format("{0}[", variableName);
            string addrPrefix = String.Format("{0}[", namePrefix + variableName);

            AddPviStructureArrayElements(namePrefix,
                                         varNamePrefix,
                                         addrPrefix,
                                         taskName,
                                         completeTypeName,
                                         structureElements,
                                         jobType,
                                         elementsNumber,
                                         arrayDimCount,
                                         0,
                                         arrayStartIndexes,
                                         arrayIndexLimits,
                                         parentId,
                                         inRootItem);
        }

        void AddPviStructureArrayElements(string namePrefix,
                                  string varNamePrefix,
                                  string addrPrefix,
                                  string taskName,
                                  string completeTypeName,
                                  string[] structureElements,
                                  DriverCodeBaseEx.Enumerators.LinkType jobType,
                                  uint elementsNumber,
                                  uint arrayDimCount,
                                  uint currentDim,
                                  uint[] arrayStartIndexes,
                                  uint[] arrayIndexLimits,
                                  int parentId,
                                  ImportData inRootItem)
        {
            for (uint i = arrayStartIndexes[currentDim]; i < arrayIndexLimits[currentDim]; i++)
            {

                uint RealArrayIndex = i - arrayStartIndexes[currentDim];
                //in multidimension array, from 2nd dimension, use start index (standard array, always start from 0)
                if (currentDim > 0)
                    RealArrayIndex = i;
                string szAddress = String.Format("{0}{1}", addrPrefix, RealArrayIndex);
                string varName = String.Format("{0}{1}", varNamePrefix, RealArrayIndex);
                if (currentDim == (arrayDimCount - 1))
                {
                    varName += "]";
                    szAddress += "]";
                    AddPviStructureArraySingleElement(namePrefix,
                                                      varName,
                                                      szAddress,
                                                      taskName,
                                                      completeTypeName,
                                                      structureElements,
                                                      jobType,
                                                      parentId,
                                                      inRootItem);
                }
                else
                {
                    varName += ",";
                    szAddress += ",";
                    AddPviStructureArrayElements(namePrefix,
                                                 varName,
                                                 szAddress,
                                                 taskName,
                                                 completeTypeName,
                                                 structureElements,
                                                 jobType,
                                                 elementsNumber,
                                                 arrayDimCount,
                                                 currentDim + 1,
                                                 arrayStartIndexes,
                                                 arrayIndexLimits,
                                                 parentId,
                                                 inRootItem);
                }
            }
        }

        void AddPviStructureArraySingleElement(string namePrefix,
                                               string varName,
                                               string szAddress,
                                               string taskName,
                                               string completeTypeName,
                                               string[] structureElements,
                                               DriverCodeBaseEx.Enumerators.LinkType jobType,
                                               int parentId,
                                               ImportData inRootItem)
        {

            // Parse the variable length
            string varLength;
            uint variableLength = 1;
            ParsePviTypeAttribute(structureElements[0], "VL", out varLength);
            if (!String.IsNullOrWhiteSpace(varLength))
            {
                uint auxUint = 0;
                if (uint.TryParse(varLength, out auxUint))
                {
                    variableLength = auxUint;
                }
            }

            // Build the structure variable to be added to the tree
            ImportData v = importDataModel.addImportData();
            ((ImportDataBrPvi)v).PreName = namePrefix;
            ((ImportDataBrPvi)v).Name = varName;
            ((ImportDataBrPvi)v).Size = variableLength;
            v.szType = completeTypeName;
            ((ImportDataBrPvi)v).szTypeView = GetMoviconTypeFromPlcType(v.szType);
            ((ImportDataBrPvi)v).Task = taskName;
            ((ImportDataBrPvi)v).ElemType = completeTypeName;
            v.Address = szAddress;
            v.Description = String.Empty;
            v.parentId = parentId;
            v.Id = m_lGlobalID++;
            v.ArrayDimension = 0;
            ((ImportDataBrPvi)v).ImportDataType = ImportTypes.StructOrEnum;
            ((ImportDataBrPvi)v).JobType = jobType;

            // Add the structure elements to the tree
            if (structureElements.Count() < 2)
            {
                return;
            }
            string[] elementDefinitions = new string[structureElements.Count() - 1];
            for (int i = 1; i < structureElements.Count(); i++)
            {
                elementDefinitions[i - 1] = structureElements[i];
            }
            string addressPrefix = szAddress;
            AddPviStructureElements(namePrefix,
                                    taskName,
                                    addressPrefix,
                                    elementDefinitions,
                                    jobType,
                                    v.Id,
                                    inRootItem,
                                    v);
        }

        void AddPviStructureElements(string namePrefix,
                                     string taskName,
                                     string addressPrefix,
                                     string[] structureElements,
                                     DriverCodeBaseEx.Enumerators.LinkType jobType,
                                     int parentId,
                                     ImportData inParentRootItem,
                                     ImportData inRootItem)
        {
            bool parentStructHasBeenAddedToTheTree = false;
            for (int i = 0; i < structureElements.Count();)
            {
                string structElement = structureElements[i];
                // Parse the element name
                int searchIndex = structElement.IndexOf(' ');
                if (searchIndex < 1)
                {
                    i++;
                    continue;
                }
                string auxString = structElement.Substring(0, searchIndex);
                // The first character is '.' --> Skip it in the element name
                string elementName = auxString.Substring(1);
                string elementAddress = addressPrefix + auxString;
                string varPreName = namePrefix + addressPrefix + ".";

                // Parse the element type
                string varType;
                ParsePviTypeAttribute(structElement, "VT", out varType);
                if (String.IsNullOrWhiteSpace(varType))
                {
                    i++;
                    continue;
                }
                if (!IsSupportedPlcType(varType))
                {
                    i++;
                    continue;
                }

                // Parse the variable scope (mandatory parameter)
                string varScope;
                ParsePviTypeAttribute(structElement, "SC", out varScope);
                if (varScope.IndexOf(POBJ_ACC_LIST_PVAR_DYNAMIC) >= 0)
                {
                    return;
                }

                // Parse the variable length
                string varLength;
                uint variableLength = 1;
                ParsePviTypeAttribute(structElement, "VL", out varLength);
                if (!String.IsNullOrWhiteSpace(varLength))
                {
                    uint auxUint = 0;
                    if (uint.TryParse(varLength, out auxUint))
                    {
                        variableLength = auxUint;
                    }
                }

                // Parse the number of elements
                string elemNum;
                uint elementsNumber = 1;
                ParsePviTypeAttribute(structElement, "VN", out elemNum);
                if (!String.IsNullOrWhiteSpace(elemNum))
                {
                    uint auxUint = 0;
                    if (uint.TryParse(elemNum, out auxUint))
                    {
                        elementsNumber = auxUint;
                    }
                }

                // Structure?
                if (IsPlcStructureType(varType))
                {
                    // Parse the structure type (mandatory parameter)
                    ParsePviTypeAttribute(structElement, "SN", out varType);
                    if (String.IsNullOrWhiteSpace(varType))
                    {
                        i++;
                        continue;
                    }

                    string completeTypeName = String.Empty;
                    if (!String.IsNullOrWhiteSpace(taskName))
                    {
                        completeTypeName = taskName + "_" + varType;
                    }
                    else
                    {
                        completeTypeName = varType;
                    }

                    // Build the array of elements of the substructure
                    uint numberOfSubstructureElements = CountSubstructureElements(elementName, i, structureElements);
                    if (numberOfSubstructureElements == 0)
                    {
                        i++;
                        continue;
                    }
                    string[] substructureElements = new string[numberOfSubstructureElements];
                    for (int j = 0; j < (int)numberOfSubstructureElements; j++)
                    {
                        substructureElements[j] = structureElements[i + j + 1];
                    }
                    RemoveStructNameFromElementNames(elementName, ref substructureElements);

                    // Array of structures?
                    if (elementsNumber > 1)
                    {
                        string[] substructureDefinition = new string[substructureElements.Count() + 1];
                        substructureDefinition[0] = structureElements[i];
                        for (int j = 0; j < (int)numberOfSubstructureElements; j++)
                        {
                            substructureDefinition[j + 1] = substructureElements[j];
                        }
                        AddPviStructureArray(varPreName,
                                             elementName,
                                             taskName,
                                             completeTypeName,
                                             substructureDefinition,
                                             jobType,
                                             elementsNumber,
                                             -1,
                                             null);
                        i += (int)numberOfSubstructureElements + 1;
                        continue;
                    }

                    // Add the parent structure variable to the tree
                    if ((inRootItem != null) && !parentStructHasBeenAddedToTheTree)
                    {
                        AddTreeItem(inRootItem, inParentRootItem);
                        parentStructHasBeenAddedToTheTree = true;
                    }

                    // Build the substructure variable to be added to the tree
                    ImportData v = importDataModel.addImportData();
                    ((ImportDataBrPvi)v).PreName = varPreName;
                    v.Name = elementName;
                    ((ImportDataBrPvi)v).Size = variableLength;
                    v.szType = completeTypeName;
                    ((ImportDataBrPvi)v).szTypeView = GetMoviconTypeFromPlcType(v.szType);
                    ((ImportDataBrPvi)v).Task = taskName;
                    ((ImportDataBrPvi)v).ElemType = completeTypeName;
                    v.Address = elementAddress;
                    v.Description = String.Empty;
                    v.parentId = parentId;
                    v.Id = m_lGlobalID++;
                    v.ArrayDimension = 0;
                    ((ImportDataBrPvi)v).ImportDataType = ImportTypes.StructOrEnum;
                    ((ImportDataBrPvi)v).JobType = jobType;

                    // Add the substructure elements to the tree
                    AddPviStructureElements(namePrefix,
                                            taskName,
                                            elementAddress,
                                            substructureElements,
                                            jobType,
                                            v.Id,
                                            inRootItem,
                                            v);

                    i += (int)numberOfSubstructureElements + 1;
                    continue;
                }

                // Get the corresponding MOVICON type
                int moviconType = GetMoviconType(varType);
                // Invalid Type?
                if (moviconType == -1)
                {
                    i++;
                    continue;
                }

                // Array?
                if (elementsNumber > 1)
                {
                    // Add the parent structure variable to the tree
                    if ((inRootItem != null) && !parentStructHasBeenAddedToTheTree)
                    {
                        AddTreeItem(inRootItem, inParentRootItem);
                        parentStructHasBeenAddedToTheTree = true;
                    }

                    string addrPrefix = addressPrefix + ".";
                    // Add the array variable and its elements to the tree
                    AddPviArrayVariable(varPreName,
                                        addrPrefix,
                                        taskName,
                                        elementName,
                                        varType,
                                        structElement,
                                        jobType,
                                        elementsNumber,
                                        variableLength,
                                        parentId,
                                        inRootItem);
                    i++;
                    continue;
                }

                // Add the parent structure variable to the tree
                if ((inRootItem != null) && !parentStructHasBeenAddedToTheTree)
                {
                    AddTreeItem(inRootItem, inParentRootItem);
                    parentStructHasBeenAddedToTheTree = true;
                }

                // Standard variable

                // Add the standard variable to the tree
                AddPviStandardVar(varPreName,
                                  elementName,
                                  elementAddress,
                                  taskName,
                                  String.Empty, // No description
                                  varType,
                                  moviconType,
                                  variableLength,
                                  jobType,
                                  parentId,
                                  inRootItem);
                i++;
            }
        }

        void RemoveStructNameFromElementNames(string structName, ref string[] structureElements)
        {
            string searchString = "." + structName;
            for (int i = 0; i < structureElements.Count(); i++)
            {
                string element = structureElements[i];
                int searchIndex = element.IndexOf(searchString);
                if (searchIndex != 0)
                {
                    continue;
                }
                string newElement = element.Substring(searchString.Length);
                structureElements[i] = newElement;
            }
        }

        uint CountSubstructureElements(string structName, int structDefinitionIndex, string[] structureElements)
        {
            uint elementCount = 0;
            string searchString = "." + structName + ".";
            for (int i = structDefinitionIndex + 1; i < structureElements.Count(); i++)
            {
                if (structureElements[i].IndexOf(searchString) == 0)
                {
                    elementCount++;
                }
                else
                {
                    break;
                }
            }
            return (elementCount);
        }

        bool ParseArrayInfo(string typeInfo, uint dimCount, uint elementsNumber, ref uint[] arrayStartIndexes, ref uint[] arrayIndexLimits)
        {
            if (String.IsNullOrWhiteSpace(typeInfo) || (typeInfo.IndexOf("a,") < 0))
            {
                arrayStartIndexes[0] = 0;
                arrayIndexLimits[0] = elementsNumber;
                return (true);
            }

            string[] stringSeparators = new string[] { ";" };
            string[] arrayAttributes = typeInfo.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
            char[] splitParameters = new char[1];
            splitParameters[0] = ',';
            int i = 0;
            foreach (string attribute in arrayAttributes)
            {
                if (i >= arrayStartIndexes.Length)
                {
                    break;
                }
                arrayStartIndexes[i] = 0;
                arrayIndexLimits[i] = 0;
                string auxString = attribute;
                int searchIndex = attribute.IndexOf("a,");
                if (searchIndex == 0)
                {
                    auxString = attribute.Substring(2);
                }
                else
                {
                    continue;
                }
                string[] indexes = auxString.Split(splitParameters);
                if (indexes.Count() == 2)
                {
                    if (!uint.TryParse(indexes[0], out arrayStartIndexes[i]))
                    {
                        return (false);
                    }
                    if (!uint.TryParse(indexes[1], out arrayIndexLimits[i]))
                    {
                        return (false);
                    }
                    else
                    {
                        arrayIndexLimits[i]++;
                    }
                }
                else
                {
                    continue;
                }
                i++;
            }
            return (true);
        }

        uint CountArrayDimensions(string typeInfo)
        {
            uint dimCount = 0;
            if (!String.IsNullOrWhiteSpace(typeInfo) && (typeInfo.IndexOf("a,") >= 0))
            {
                string[] stringSeparators = new string[] { ";" };
                string[] arrayAttributes = typeInfo.Split(stringSeparators, StringSplitOptions.RemoveEmptyEntries);
                if (arrayAttributes.Count() > 0)
                {
                    foreach (string attribute in arrayAttributes)
                    {
                        if (attribute.IndexOf("a,") == 0)
                        {
                            dimCount++;
                        }
                    }
                }
            }
            if (dimCount == 0)
            {
                dimCount++;
            }
            return (dimCount);
        }

        void ParsePviTypeAttribute(string typeDefinition, string typeParameter, out string pviTypeAttribute)
        {
            pviTypeAttribute = String.Empty;
            if (String.IsNullOrWhiteSpace(typeParameter))
            {
                return;
            }
            string pviTypeParameter = typeParameter + "=";
            int searchIndex = typeDefinition.IndexOf(pviTypeParameter);
            if (searchIndex < 0)
            {
                return;
            }
            string auxString = typeDefinition.Substring(searchIndex + pviTypeParameter.Length);
            if (String.IsNullOrWhiteSpace(auxString))
            {
                return;
            }
            searchIndex = auxString.IndexOf(" ");
            if (searchIndex > 0)
            {
                pviTypeAttribute = auxString.Substring(0, searchIndex);
            }
            else
            {
                pviTypeAttribute = auxString;
            }
        }
        //#endregion
                
        #region IDisposable Members

        public void Dispose()
        {
            if (importerPviSink != null)
            {
                importerPviSink.Dispose();
                importerPviSink = null;
            }
        }
        #endregion
    }
    #endregion
}
