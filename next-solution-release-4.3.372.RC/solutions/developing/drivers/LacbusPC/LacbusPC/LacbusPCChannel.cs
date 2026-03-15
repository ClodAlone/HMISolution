using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
//using TwinCAT.Ads;
using Opc.Ua;
using System.IO;
using System.Runtime.InteropServices;

namespace LacbusPC
{
    public enum LacbusPCErrorCodes : int
    {
        ErrorCodeUnableToProcessTheCommand = 432,
        ErrorCodeSyntaxError = 501,
        ErrorCodeForbiddenCommand = 531,
        ErrorCodeBadRtuNumber = 533,
        ErrorCodeAlreadyAuthenticatedSCADA = 536,
        ErrorCodeDisabledScadaNumber = 537,
        ErrorCodeAuthenticationNeeded = 540,
        ErrorCodeUnsupportedProtocolVersion = 541,
        ErrorCodeUnexpectedErrorCode = 1000,
        ErrorCodeConnectionBroken = 1001,
        ErrorCodeErrorPreparingFrameRequest = 1002,
        ErrorCodeErrorSendingFrameRequest = 1003,
        ErrorCodeErrorOpeningRequestChannel = 1004,
        ErrorCodeErrorOpeningRequestChannelSyntax = 1005,
        ErrorCodeErrorOpeningRequestChannelAuthentication = 1006,
        ErrorCodeReceivedIncompleteMessage = 1007,
        ErrorCodeReceivedIncorrectMessageType = 1008,
        ErrorCodeReceivedIncorrectUnderlyingProtocol = 1009,
        ErrorCodeReceivedIncorrectProtocolVersion = 1010,
        ErrorCodeReceivedIncorrectLacbusPCHeader = 1011,
        ErrorCodeReceivedIncorrectLacbusPCHeaderLength = 1012,
        ErrorCodeRTUPollRequestFailed = 2000,
        ErrorRTUDisconnected = 10000
    }

    /// <summary>   Specific Bits of the state/command variable of a station. </summary>
    public enum LacbusPCStationVariableBits : ushort
    {
        StationUseSecondMedium = 2
    }

    public class LacbusPCChannel : Channel
    {
        public const uint MAX_REQUEST_NUMBER = 500;

        #region Constructors

        /// <summary>
        /// Initializes the LacbusPCChannel object.
        /// </summary>
        public LacbusPCChannel(CommunicationDriver commdriver, LacbusPCChannelSettings settings)
            : base(commdriver, settings, false)
        {
            _LacbusPCHostName = settings.LacbusPCHostName;
            _LacbusPCHostPort = settings.LacbusPCHostPort;
            _LacbusPCConnectionTimeout = settings.LacbusPCConnectionTimeout;
            _LacbusPCCommunicationTimeout = settings.LacbusPCCommunicationTimeout;
            _LacbusPCPCNumber = settings.LacbusPCPCNumber;
            _LacbusPCMaxNumberOfAggregatedRequests = settings.LacbusPCMaxNumberOfAggregatedRequests;
            if (_LacbusPCMaxNumberOfAggregatedRequests == 0)
            {
                _LacbusPCMaxNumberOfAggregatedRequests = 1;
            }
            _LacbusPCBackupHostName = settings.LacbusPCBackupHostName;
            selectedHostName = _LacbusPCHostName;
            _LacbusPCTimeZone = settings.LacbusPCTimeZone;
            _LacbusPCMaxPendingRTUPollReq = settings.LacbusPCMaxPendingRTUPollReq;
            if (_LacbusPCMaxPendingRTUPollReq == 0)
            {
                _LacbusPCMaxPendingRTUPollReq = 1;
            }
            WaitInCaseOfConnectionError = new ManualResetEvent(false);
            vortexEventsObj = new VortexEvents(this);
            unsafe
            {
                remoteFrameReceivedCallbackFunc = vortexEventsObj.FrameReceivedFromRemotePeerCallbackFunc;
                replyFrameReceivedCallbackFunc = vortexEventsObj.ReplyReceivedFromRemotePeerCallbackFunc;
                onConnectionCloseCallbackFunc = vortexEventsObj.ConnectionCloseCallbackFunc;
            }
        }

        #endregion

        #region Data Members

        protected object lockStream = new object();
        List<LacbusPCCommJob> ListPollRtuJobPending = new List<LacbusPCCommJob>();
        protected ManualResetEvent WaitInCaseOfConnectionError;
        protected DateTime lastFailedConnectionTime = DateTime.MinValue;
        protected int lastConnectionError = 0;

        VortexDevManager chVortexDevManager = new VortexDevManager();
        bool vortexDevManagerInitialized = false;
        bool vortexConnectionInitialized = false;
        bool identificationChannelCreated = false;
        bool supervisorSendChannelCreated = false;
        bool isConnectionBroken = false;
        string communicationProfile = "http://sofrel.com/Lacbus-PC/PC2FR";
        string identificationProfile = "http://sofrel.com/Lacbus-PC/IDENTPC";
        string remoteCommunicationProfile = "http://sofrel.com/Lacbus-PC/FR2PC";
        unsafe void* identificationChannel = null;
        unsafe void* supervisorSendChannel = null;
        unsafe OnFrameReceivedCallbackFunc remoteFrameReceivedCallbackFunc;
        unsafe OnFrameReceivedCallbackFunc replyFrameReceivedCallbackFunc;
        unsafe OnCloseConnectionCallbackFunc onConnectionCloseCallbackFunc;
        GCHandle gcHandleRemoteFrameReceivedCallbackFunc;
        GCHandle gcHandleReplyFrameReceivedCallbackFunc;
        GCHandle gcHandleReplyOnCloseConnectionCallbackFunc;
        VortexEvents vortexEventsObj;
        List<byte[]> ReceivedFramesFromRemoteList = new List<byte[]>();
        protected Object lockReceivedFramesFromRemoteListObject = new Object();
        Dictionary<ushort, LacbusPCStation> mapStations = new Dictionary<ushort, LacbusPCStation>();
        protected Object lockMapInputJobsObject = new Object();
        public Dictionary<UInt64, List<LacbusPCCommJob>> mapInputJobs = new Dictionary<UInt64, List<LacbusPCCommJob>>();
        Dictionary<int, byte[]> mapReplies = new Dictionary<int, byte[]>();
        protected Object lockRepliesFromRemoteListObject = new Object();
        DateTime lastActivityTimeOnSendChannel = DateTime.UtcNow;
        int sendChannelMaxIdleTime = 30000;
        int lastRequestMessageNumber = 0;

        List<LacbusPCStation> pollStationList = new List<LacbusPCStation>();
        List<LacbusPCStation> pendingPollStationList = new List<LacbusPCStation>();
        DateTime lastPollStationTime = DateTime.MinValue;

        string selectedHostName = String.Empty;
        bool switchHost = false;
        bool firstConnectionAttempt = true;
        bool backupHostIsActive = false;
        bool performDeviceCloseConnection = false;

        TimeZoneInfo channelTimeZoneInfo;

        // List of stations with an active poll request
        Dictionary<string, LacbusPCStation> activePollStationMap = new Dictionary<string, LacbusPCStation>();
        private uint MaxTimeWithoutData = Properties.Settings.Default.ConstMaxTimeWithoutData;
        DateTime lastActivePollStationCheckTime = DateTime.MinValue;

        #endregion

        #region Abstracts Methods

        public override bool TestChannelComm()
        {
            return (true);
        }

        public override bool IsDeviceOpen()
        {
            lock (lockStream)
            {
                bool returnValue = (vortexDevManagerInitialized &&
                                    vortexConnectionInitialized &&
                                    identificationChannelCreated);

                // Check if the server must be changed
                if (!String.IsNullOrWhiteSpace(LacbusPCBackupHostName))
                {
                    switchHost = false;
                    if ((GetStateCommandVariableBit(ref switchHost, (UInt16)ChannelVariableBits.SwitchServer) == true) && (switchHost == true))
                    {
                        //System.Diagnostics.Debug.WriteLine("SwitchServerDEBUG - IsDeviceOpen {0} - Switch Server!", DateTime.Now.ToString("HH:mm:ss.fff"));
                        return (false);
                    }
                }

                SetStateCommandVariableBit(!returnValue, (UInt16)ChannelVariableBits.ChannelUnconnected);
                SetStateCommandVariableBit(backupHostIsActive, (UInt16)ChannelVariableBits.ConnectedHost);
                return (returnValue);
            }
        }

        bool IsActiveCommunicationChannelOpened()
        {
            return (supervisorSendChannelCreated);
        }

        void UnreferenceActiveCommunicationChannel()
        {
            unsafe
            {
                if ((supervisorSendChannelCreated == true) && (supervisorSendChannel != null))
                {
                    chVortexDevManager.vortex_channel_unref(ref supervisorSendChannel);
                }

                supervisorSendChannelCreated = false;
                supervisorSendChannel = null;
            }
        }

        bool CloseActiveCommunicationChannel()
        {
            unsafe
            {
                if ((supervisorSendChannelCreated == true) && (supervisorSendChannel != null))
                {
                    if (chVortexDevManager.vortex_channel_close(supervisorSendChannel) != 0)
                    {
                        return (false);
                    }

                }

                supervisorSendChannelCreated = false;
                supervisorSendChannel = null;
            }

            if(gcHandleReplyFrameReceivedCallbackFunc.IsAllocated)
            {
                gcHandleReplyFrameReceivedCallbackFunc.Free();
            }
            return (true);
        }

        bool OpenActiveCommunicationChannel()
        {
            if (supervisorSendChannelCreated == false)
            {
                unsafe
                {
                    IntPtr intptrReplyFrameReceivedCallbackFunc = Marshal.GetFunctionPointerForDelegate(replyFrameReceivedCallbackFunc);
                    gcHandleReplyFrameReceivedCallbackFunc = GCHandle.Alloc(replyFrameReceivedCallbackFunc);
                    int returnValue = chVortexDevManager.vortex_channel_new(communicationProfile, ref supervisorSendChannel, (IntPtr)null, intptrReplyFrameReceivedCallbackFunc, (IntPtr)null);
                    if (returnValue != 0)
                    {
                        return (false);
                    }
                }

                supervisorSendChannelCreated = true;
            }

            return (true);
        }

        uint GetPollRTURequestFrameLength(bool useSecondMedium)
        {
            uint frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
            frameSize += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
            frameSize += 2; // Block data
            // Secondary medium
            if (useSecondMedium == true)
            {
                frameSize += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                frameSize += 1; // Block data
            }

            return (frameSize);
        }

        uint GetRequestFrameLength(LacbusPCCommJob job, LacbusPcUnderlyingProtocols underlyingProtocol)
        {
            // Input request
            if (job.Type == DriverCodeBase.Enumerators.LinkType.Input ||
                (job.Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
                job.TagsListOnWriting.Count == 0))
            {
                System.Diagnostics.Debug.WriteLine("GetRequestFrameLength error 1");
                return 0;
            }

            // Output request
            uint frameSize = 0;
            switch (job.LacbusPCDatumType)
            {
                case DatumTypes.SetDateTime:
                    switch (underlyingProtocol)
                    {
                        case LacbusPcUnderlyingProtocols.LacbusPC:
                            frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
                            frameSize += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                            frameSize += 6; // Block data
                            break;

                        case LacbusPcUnderlyingProtocols.LacbusRTU:
                        case LacbusPcUnderlyingProtocols.SofbusPL:
                            {
                                frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
                                uint duLength = LacbusPcProtocol.LacbusRtuDUHeaderLength; // Lacbus RTU DU header
                                duLength += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                                duLength += 6; // Block data
                                if ((duLength % 2) > 0)
                                {
                                    duLength++;
                                }
                                frameSize += duLength;
                            }
                            break;

                        default:
                            System.Diagnostics.Debug.WriteLine("GetRequestFrameLength error 3a");
                            break;
                    }
                    break;

                case DatumTypes.ModbusCoilSetpoints:
                case DatumTypes.ModbusRegisterSetpoints:
                    if (underlyingProtocol == LacbusPcUnderlyingProtocols.LacbusPC)
                    {
                        frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
                        frameSize += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                        frameSize += 5 + job.TotalJobSize;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("GetRequestFrameLength error 3b");
                    }
                    break;

                case DatumTypes.RTUPollRequest:
                    frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
                    frameSize += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                    frameSize += 2; // Block data
                    // Secondary medium
                    if (job.LacbusPCDatumNumber != 0)
                    {
                        frameSize += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                        frameSize += 1; // Block data
                    }
                    break;

                case DatumTypes.ShutdownFR1000FrontEnd:
                    if (underlyingProtocol == LacbusPcUnderlyingProtocols.LacbusPC)
                    {
                        frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
                        frameSize += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                        frameSize += 7; // Block data
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("GetRequestFrameLength error 3c");
                    }
                    break;

                case DatumTypes.DigitalOutput:
                    switch(underlyingProtocol)
                    {
                        case LacbusPcUnderlyingProtocols.LacbusPC:
                            frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
                            frameSize += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                            frameSize += 7; // Block data
                            break;

                        case LacbusPcUnderlyingProtocols.LacbusRTU:
                            {
                                frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
                                uint duLength = LacbusPcProtocol.LacbusRtuDUHeaderLength; // Lacbus RTU DU header
                                duLength += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                                duLength += 4; // Block data
                                if((duLength%2) > 0)
                                {
                                    duLength++;
                                }
                                frameSize += duLength;
                            }
                            break;

                        case LacbusPcUnderlyingProtocols.SofbusPL:
                            {
                                frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
                                uint duLength = LacbusPcProtocol.LacbusRtuDUHeaderLength; // SOFBUS PL DU header (same size of the Lacbus RTU DU header)
                                duLength += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                                duLength += 3; // Block data
                                if ((duLength % 2) > 0)
                                {
                                    duLength++;
                                }
                                frameSize += duLength;
                            }
                            break;

                        default:
                            System.Diagnostics.Debug.WriteLine("GetRequestFrameLength error 3");
                            break;
                    }
                    break;

                case DatumTypes.AnalogOutput:
                    switch (underlyingProtocol)
                    {
                        case LacbusPcUnderlyingProtocols.LacbusPC:
                            frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
                            frameSize += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                            frameSize += 14;
                            break;

                        case LacbusPcUnderlyingProtocols.LacbusRTU:
                            {
                                frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
                                uint duLength = LacbusPcProtocol.LacbusRtuDUHeaderLength; // Lacbus RTU DU header
                                duLength += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                                duLength += 11; // Block data
                                if ((duLength % 2) > 0)
                                {
                                    duLength++;
                                }
                                frameSize += duLength;
                            }
                            break;

                        case LacbusPcUnderlyingProtocols.SofbusPL:
                            {
                                frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
                                uint duLength = LacbusPcProtocol.LacbusRtuDUHeaderLength; // SOFBUS PL DU header (same size of the Lacbus RTU DU header)
                                duLength += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                                duLength += 5; // Block data
                                if ((duLength % 2) > 0)
                                {
                                    duLength++;
                                }
                                frameSize += duLength;
                            }
                            break;

                        default:
                            System.Diagnostics.Debug.WriteLine("GetRequestFrameLength error 4");
                            break;
                    }
                    break;

                case DatumTypes.CountInput:
                    switch (underlyingProtocol)
                    {
                        case LacbusPcUnderlyingProtocols.LacbusRTU:
                            {
                                frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
                                uint duLength = LacbusPcProtocol.LacbusRtuDUHeaderLength; // Lacbus RTU DU header
                                duLength += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                                duLength += 7; // Block data
                                if ((duLength % 2) > 0)
                                {
                                    duLength++;
                                }
                                frameSize += duLength;
                            }
                            break;

                        case LacbusPcUnderlyingProtocols.SofbusPL:
                            {
                                frameSize = LacbusPcProtocol.LacbusPCMessageHeaderLength; // Lacbus PC header
                                uint duLength = LacbusPcProtocol.LacbusRtuDUHeaderLength; // SOFBUS PL DU header (same size of the Lacbus RTU DU header)
                                duLength += LacbusPcProtocol.LacbusRtuBlockHeaderLength; // Block header
                                duLength += 6; // Block data
                                if ((duLength % 2) > 0)
                                {
                                    duLength++;
                                }
                                frameSize += duLength;
                            }
                            break;

                        default:
                            System.Diagnostics.Debug.WriteLine("GetRequestFrameLength error 5");
                            break;
                    }
                    break;

                default:
                    System.Diagnostics.Debug.WriteLine("GetRequestFrameLength error 2");
                    break;
            }

            return (frameSize);
        }

        uint PreparePollRTURequestFrame(LacbusPCStation lStation, bool useSecondMedium, ushort communicationDuration, ref byte[] requestFrame)
        {
            if (lStation == null)
            {
                return (0);
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - PreparePollRTURequestFrame has been called for station {1}", DateTime.Now, lStation.Name);
#endif

            // Lacbus PC header
            LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
            messageHeader.headerProtocolVersion = 1;
            messageHeader.headerUnderlyingProtocol = LacbusPcUnderlyingProtocols.LacbusPC;
            messageHeader.headerMessageType = 0x47; // 'G' message
            messageHeader.headerRtuNumber = lStation.LacbusPCRTUNumber;
            messageHeader.headerDataLength = LacbusPcProtocol.LacbusRtuBlockHeaderLength + 2;
            // Secondary medium
            if (useSecondMedium == true)
            {
                messageHeader.headerDataLength += LacbusPcProtocol.LacbusRtuBlockHeaderLength + 1;
            }
            messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
            uint frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);
            // Data block 'D'
            requestFrame[frameSize++] = 0x44;
            // Body length
            requestFrame[frameSize++] = 0;
            requestFrame[frameSize++] = 2;
            // Data = Communication Duration
            requestFrame[frameSize++] = (byte)(communicationDuration >> 8);
            requestFrame[frameSize++] = (byte)(communicationDuration);
            // Secondary medium
            if (useSecondMedium == true)
            {
                // Data block 'S'
                requestFrame[frameSize++] = 0x53;
                // Body length
                requestFrame[frameSize++] = 0;
                requestFrame[frameSize++] = 1;
                // Data = Communication on secondary medium
                requestFrame[frameSize++] = (byte)0xff;
            }
            return (frameSize);
        }

        uint PrepareRequestFrame(LacbusPCCommJob job, object jobData, ref byte[] requestFrame, LacbusPcUnderlyingProtocols underlyingProtocol)
        {
            System.Diagnostics.Debug.WriteLine("PrepareRequestFrame has been called");

            // Input request
            if (job.Type == DriverCodeBase.Enumerators.LinkType.Input ||
                (job.Type == DriverCodeBase.Enumerators.LinkType.InputOutput &&
                job.TagsListOnWriting.Count == 0))
            {
                System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 1");
                return 0;
            }

            // Output request
            uint frameSize = 0;
            LacbusPCStation lStation = (LacbusPCStation)job.Station;
            if(lStation == null)
            {
                return (0);
            }
            switch (job.LacbusPCDatumType)
            {
                case DatumTypes.SetDateTime:
                    switch (underlyingProtocol)
                    {
                        case LacbusPcUnderlyingProtocols.LacbusPC:
                        {
                            // Lacbus PC header
                            LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                            messageHeader.headerProtocolVersion = 1;
                            messageHeader.headerUnderlyingProtocol = LacbusPcUnderlyingProtocols.LacbusPC;
                            messageHeader.headerMessageType = 0x50; // 'P' message
                            messageHeader.headerRtuNumber = 0;
                            messageHeader.headerDataLength = 9;
                            //messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                            frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);
                            // Data block 'H'
                            requestFrame[frameSize++] = 0x48;
                            // Body length
                            requestFrame[frameSize++] = 0;
                            requestFrame[frameSize++] = 6;
                            // Get current time
                            DateTime dt = DateTime.UtcNow;
                            dt = ConvertTimeFromUtc(dt);
                            requestFrame[frameSize++] = (byte)dt.Day;
                            requestFrame[frameSize++] = (byte)dt.Month;
                            requestFrame[frameSize++] = (byte)(dt.Year - 2000);
                            requestFrame[frameSize++] = (byte)dt.Hour;
                            requestFrame[frameSize++] = (byte)dt.Minute;
                            requestFrame[frameSize++] = (byte)dt.Second;
                        }
                        break;

                        case LacbusPcUnderlyingProtocols.LacbusRTU:
                        case LacbusPcUnderlyingProtocols.SofbusPL:
                        {
                            // Lacbus PC header
                            LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                            messageHeader.headerProtocolVersion = 1;
                            messageHeader.headerUnderlyingProtocol = underlyingProtocol;
                            messageHeader.headerMessageType = 0x43; // 'C' message
                            messageHeader.headerRtuNumber = lStation.LacbusPCRTUNumber;
                            ushort duLength = (ushort)(LacbusPcProtocol.LacbusRtuDUHeaderLength + LacbusPcProtocol.LacbusRtuBlockHeaderLength + 6);
                            bool addFramingByte = false;
                            if ((duLength % 2) > 0)
                            {
                                addFramingByte = true;
                                duLength++;
                            }
                            messageHeader.headerDataLength = duLength;
                            messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                            frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);

                            // Data Unit Header
                            LacbusRtuDataUnitHeader duHeader = new LacbusRtuDataUnitHeader();
                            duHeader.DUTotalLength = (ushort)(duLength / 2); // Number of words
                            duHeader.DURtuNumber = lStation.LacbusPCRTUNumber;
                            duHeader.DUType = 0x43; // Data Unit 'C'
                            duHeader.DUProtocolVersion = 0;
                            LacbusPcProtocol.PrepareLacbusRtuDuHeader(ref requestFrame, duHeader, ref frameSize);

                            // Data block 'H'
                            requestFrame[frameSize++] = 0x48;
                            // Body length
                            ushort aux = (ushort)(duLength - LacbusPcProtocol.LacbusRtuDUHeaderLength - LacbusPcProtocol.LacbusRtuBlockHeaderLength);
                            if (addFramingByte == true)
                            {
                               aux--;
                            }
                            requestFrame[frameSize++] = (byte)(aux >> 8);
                            requestFrame[frameSize++] = (byte)aux;
                            // Get current time
                            DateTime dt = DateTime.UtcNow;
                            dt = ConvertTimeFromUtc(dt);
                            requestFrame[frameSize++] = (byte)dt.Day;
                            requestFrame[frameSize++] = (byte)dt.Month;
                            requestFrame[frameSize++] = (byte)(dt.Year - 2000);
                            requestFrame[frameSize++] = (byte)dt.Hour;
                            requestFrame[frameSize++] = (byte)dt.Minute;
                            requestFrame[frameSize++] = (byte)dt.Second;
                            // Framing byte
                            if (addFramingByte == true)
                            {
                                requestFrame[frameSize++] = 0;
                            }
                        }
                        break;
                    }
                    break;

                case DatumTypes.ModbusCoilSetpoints:
                    {
                        var listOnWriting = new List<Tag>();
                        lock (job.retLockList())
                        {
                            listOnWriting.AddRange(job.TagsListOnWriting);
                        }
                        if (listOnWriting.Count == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 2");
                            break;
                        }
                        byte[] writingData = (byte[])jobData;
                        ushort itemCount = 0;
                        ushort startAddress = job.LacbusPCDatumNumber;
                        if (listOnWriting[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean)
                        {
                            itemCount = job.modbusElementOnWrite(listOnWriting);
                            startAddress += (UInt16)listOnWriting[0].ByteOffset;
                        }
                        else
                        {
                            startAddress += (UInt16)(listOnWriting[0].ByteOffset * 8);
                            itemCount = (ushort)(writingData.Count() * 8);
                        }

                        // Lacbus PC header
                        LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                        messageHeader.headerProtocolVersion = 1;
                        messageHeader.headerUnderlyingProtocol = LacbusPcUnderlyingProtocols.LacbusPC;
                        messageHeader.headerMessageType = 0x4d; // 'M' message
                        //LacbusPCStation lStation = (LacbusPCStation)job.Station;
                        messageHeader.headerRtuNumber = lStation.LacbusPCRTUNumber;
                        messageHeader.headerDataLength = (ushort)(8 + writingData.Count());
                        messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                        frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);
                        // Data block 'C'
                        requestFrame[frameSize++] = 0x43;
                        // Body length
                        ushort aux = (ushort)(5 + writingData.Count());
                        aux >>= 8;
                        requestFrame[frameSize++] = (byte)aux;
                        requestFrame[frameSize++] = (byte)(5 + writingData.Count());
                        // MODBUS header
                        // Function Code
                        requestFrame[frameSize++] = 0x0f;
                        // MODBUS Address
                        requestFrame[frameSize++] = (byte)(startAddress >> 8);
                        requestFrame[frameSize++] = (byte)startAddress;
                        // Number of bits to be written
                        requestFrame[frameSize++] = (byte)(itemCount >> 8);
                        requestFrame[frameSize++] = (byte)itemCount;
                        // Bit data
                        for (int i = 0; i < writingData.Count(); i++)
                        {
                            requestFrame[frameSize++] = writingData[i];
                        }
                    }
                    break;

                case DatumTypes.ModbusRegisterSetpoints:
                    {
                        var listOnWriting = new List<Tag>();
                        lock (job.retLockList())
                        {
                            listOnWriting.AddRange(job.TagsListOnWriting);
                        }
                        if (listOnWriting.Count == 0)
                        {
                            System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 3");
                            break;
                        }
                        byte[] writingData = (byte[])jobData;
                        ushort itemCount = (ushort)(writingData.Count() >> 1);
                        ushort startAddress = (ushort)(job.LacbusPCDatumNumber + (listOnWriting[0].ByteOffset >> 1));

                        // Lacbus PC header
                        LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                        messageHeader.headerProtocolVersion = 1;
                        messageHeader.headerUnderlyingProtocol = LacbusPcUnderlyingProtocols.LacbusPC;
                        messageHeader.headerMessageType = 0x4d; // 'M' message
                        //LacbusPCStation lStation = (LacbusPCStation)job.Station;
                        messageHeader.headerRtuNumber = lStation.LacbusPCRTUNumber;
                        ushort aux = (ushort)(8 + writingData.Count());
                        messageHeader.headerDataLength = aux;
                        messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                        frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);
                        // Data block 'C'
                        requestFrame[frameSize++] = 0x43;
                        // Body length
                        aux -= 3;
                        requestFrame[frameSize++] = (byte)(aux >> 8);
                        requestFrame[frameSize++] = (byte)aux;
                        // MODBUS header
                        // Function Code
                        requestFrame[frameSize++] = 0x10;
                        // MODBUS Address
                        requestFrame[frameSize++] = (byte)(startAddress >> 8);
                        requestFrame[frameSize++] = (byte)startAddress;
                        // Number of bits to be written
                        requestFrame[frameSize++] = (byte)(itemCount >> 8);
                        requestFrame[frameSize++] = (byte)itemCount;
                        // Register data
                        if (!job.SwapBytes && (writingData.GetLength(0) > 1))
                        {
                            CommJob.SwapByteBuffer(ref writingData);
                        }
                        for (int i = 0; i < writingData.Count(); i++)
                        {
                            requestFrame[frameSize++] = writingData[i];
                        }
                    }
                    break;

                case DatumTypes.RTUPollRequest:
                    {
                        // Lacbus PC header
                        LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                        messageHeader.headerProtocolVersion = 1;
                        messageHeader.headerUnderlyingProtocol = LacbusPcUnderlyingProtocols.LacbusPC;
                        messageHeader.headerMessageType = 0x47; // 'G' message
                        messageHeader.headerRtuNumber = lStation.LacbusPCRTUNumber;
                        messageHeader.headerDataLength = LacbusPcProtocol.LacbusRtuBlockHeaderLength + 2;
                        // Secondary medium
                        if (job.LacbusPCDatumNumber != 0)
                        {
                            messageHeader.headerDataLength += LacbusPcProtocol.LacbusRtuBlockHeaderLength + 1;
                        }
                        messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                        frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);
                        // Data block 'D'
                        requestFrame[frameSize++] = 0x44;
                        // Body length
                        requestFrame[frameSize++] = 0;
                        requestFrame[frameSize++] = 2;
                        // Data = Communication Duration
                        requestFrame[frameSize++] = (byte)(job.LacbusPCCommunicationDuration >> 8);
                        requestFrame[frameSize++] = (byte)(job.LacbusPCCommunicationDuration);
                        // Secondary medium
                        if (job.LacbusPCDatumNumber != 0)
                        {
                            // Data block 'S'
                            requestFrame[frameSize++] = 0x53;
                            // Body length
                            requestFrame[frameSize++] = 0;
                            requestFrame[frameSize++] = 1;
                            // Data = Check communication on secondary medium
                            requestFrame[frameSize++] = (byte)0xff;
                        }
                    }
                    break;

                case DatumTypes.ShutdownFR1000FrontEnd:
                    {
                        // Lacbus PC header
                        LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                        messageHeader.headerProtocolVersion = 1;
                        messageHeader.headerUnderlyingProtocol = LacbusPcUnderlyingProtocols.LacbusPC;
                        messageHeader.headerMessageType = 0x50; // 'P' message
                        messageHeader.headerRtuNumber = 0;
                        messageHeader.headerDataLength = 7;
                        frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);
                        // Data block 'C'
                        requestFrame[frameSize++] = 0x43;
                        // Body length
                        ushort aux = 4;
                        requestFrame[frameSize++] = (byte)(aux >> 8);
                        requestFrame[frameSize++] = (byte)aux;
                        // Data
                        // Datum Number
                        aux = 5;
                        requestFrame[frameSize++] = (byte)(aux >> 8);
                        requestFrame[frameSize++] = (byte)aux;
                        // Datum Format
                        requestFrame[frameSize++] = 1; // Logical datum
                        // Value
                        requestFrame[frameSize++] = 0xff;
                    }
                    break;

                case DatumTypes.DigitalOutput:
                    switch (underlyingProtocol)
                    {
                        case LacbusPcUnderlyingProtocols.LacbusPC:
                            {
                                var listOnWriting = new List<Tag>();
                                lock (job.retLockList())
                                {
                                    listOnWriting.AddRange(job.TagsListOnWriting);
                                }
                                if (listOnWriting.Count == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 4");
                                    break;
                                }
                                byte[] writingData = (byte[])jobData;
                                int itemSize = 1;
                                if (writingData.Count() < itemSize)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 5");
                                    break;
                                }

                                // Lacbus PC header
                                LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                                messageHeader.headerProtocolVersion = 1;
                                messageHeader.headerUnderlyingProtocol = LacbusPcUnderlyingProtocols.LacbusPC;
                                messageHeader.headerMessageType = 0x50; // 'P' message
                                messageHeader.headerRtuNumber = 0;
                                ushort aux = (ushort)(6 + itemSize);
                                messageHeader.headerDataLength = aux;
                                //messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                                frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);
                                // Data block 'C'
                                requestFrame[frameSize++] = 0x43;
                                // Body length
                                aux -= 3;
                                requestFrame[frameSize++] = (byte)(aux >> 8);
                                requestFrame[frameSize++] = (byte)aux;
                                // Data
                                // Datum Number
                                aux = (ushort)job.LacbusPCDatumNumber;
                                requestFrame[frameSize++] = (byte)(aux >> 8);
                                requestFrame[frameSize++] = (byte)aux;
                                // Datum Format
                                requestFrame[frameSize++] = 1; // Logical datum
                                // Value
                                if (writingData[0] != 0)
                                {
                                    requestFrame[frameSize++] = 0xff;
                                }
                                else
                                {
                                    requestFrame[frameSize++] = 0;
                                }
                            }
                            break;

                        case LacbusPcUnderlyingProtocols.LacbusRTU:
                            {
                                var listOnWriting = new List<Tag>();
                                lock (job.retLockList())
                                {
                                    listOnWriting.AddRange(job.TagsListOnWriting);
                                }
                                if (listOnWriting.Count == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 6");
                                    break;
                                }

                                if(!job.LockMustBeManaged())
                                {
                                    byte[] writingData = (byte[])jobData;
                                    int itemSize = 1;
                                    if (writingData.Count() < itemSize)
                                    {
                                        System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 7");
                                        break;
                                    }

                                    // Lacbus PC header
                                    LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                                    messageHeader.headerProtocolVersion = 1;
                                    messageHeader.headerUnderlyingProtocol = underlyingProtocol;
                                    messageHeader.headerMessageType = 0x43; // 'C' message
                                    messageHeader.headerRtuNumber = lStation.LacbusPCRTUNumber;
                                    ushort duLength = (ushort)(LacbusPcProtocol.LacbusRtuDUHeaderLength + LacbusPcProtocol.LacbusRtuBlockHeaderLength + 3 + itemSize);
                                    bool addFramingByte = false;
                                    if ((duLength % 2) > 0)
                                    {
                                        addFramingByte = true;
                                        duLength++;
                                    }
                                    messageHeader.headerDataLength = duLength;
                                    messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                                    frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);

                                    // Data Unit Header
                                    LacbusRtuDataUnitHeader duHeader = new LacbusRtuDataUnitHeader();
                                    duHeader.DUTotalLength = (ushort)(duLength / 2); // Number of words
                                    duHeader.DURtuNumber = lStation.LacbusPCRTUNumber;
                                    duHeader.DUType = 0x43; // Data Unit 'C'
                                    duHeader.DUProtocolVersion = 0;
                                    LacbusPcProtocol.PrepareLacbusRtuDuHeader(ref requestFrame, duHeader, ref frameSize);

                                    // Data block 'f'
                                    requestFrame[frameSize++] = 0x66;
                                    // Body Length
                                    ushort aux = (ushort)(duLength - LacbusPcProtocol.LacbusRtuDUHeaderLength - LacbusPcProtocol.LacbusRtuBlockHeaderLength);
                                    if (addFramingByte == true)
                                    {
                                        aux--;
                                    }
                                    requestFrame[frameSize++] = (byte)(aux >> 8);
                                    requestFrame[frameSize++] = (byte)aux;
                                    // Data
                                    // Datum Number & Datum Format
                                    aux = (ushort)(job.LacbusPCDatumNumber << 4);
                                    requestFrame[frameSize++] = (byte)(aux >> 8);
                                    aux &= 0xf0;
                                    requestFrame[frameSize++] = (byte)(aux + 0x03);
                                    // Value
                                    if (writingData[0] != 0)
                                    {
                                        requestFrame[frameSize++] = 0xff;
                                    }
                                    else
                                    {
                                        requestFrame[frameSize++] = 0;
                                    }
                                    // Lock action (lock and set value)
                                    requestFrame[frameSize++] = 1;
                                    // Framing byte
                                    if (addFramingByte == true)
                                    {
                                        requestFrame[frameSize++] = 0;
                                    }
                                }
                                else
                                {
                                    // Special case, manage two tags: value and lock status
                                    if (listOnWriting.Count != 2)
                                    {
                                        System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 7a");
                                        break;
                                    }
                                    byte[] valueData = new byte[listOnWriting[0].Size];
                                    byte[] lockData = new byte[listOnWriting[1].Size];
                                    listOnWriting[0].GetTagBuffer(ref valueData, false);
                                    listOnWriting[1].GetTagBuffer(ref lockData, false);
                                    int itemSize = 1;

                                    // Lacbus PC header
                                    LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                                    messageHeader.headerProtocolVersion = 1;
                                    messageHeader.headerUnderlyingProtocol = underlyingProtocol;
                                    messageHeader.headerMessageType = 0x43; // 'C' message
                                    messageHeader.headerRtuNumber = lStation.LacbusPCRTUNumber;
                                    ushort duLength = (ushort)(LacbusPcProtocol.LacbusRtuDUHeaderLength + LacbusPcProtocol.LacbusRtuBlockHeaderLength + 3 + itemSize);
                                    bool addFramingByte = false;
                                    if ((duLength % 2) > 0)
                                    {
                                        addFramingByte = true;
                                        duLength++;
                                    }
                                    messageHeader.headerDataLength = duLength;
                                    messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                                    frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);

                                    // Data Unit Header
                                    LacbusRtuDataUnitHeader duHeader = new LacbusRtuDataUnitHeader();
                                    duHeader.DUTotalLength = (ushort)(duLength / 2); // Number of words
                                    duHeader.DURtuNumber = lStation.LacbusPCRTUNumber;
                                    duHeader.DUType = 0x43; // Data Unit 'C'
                                    duHeader.DUProtocolVersion = 0;
                                    LacbusPcProtocol.PrepareLacbusRtuDuHeader(ref requestFrame, duHeader, ref frameSize);

                                    // Data block 'f'
                                    requestFrame[frameSize++] = 0x66;
                                    // Body Length
                                    ushort aux = (ushort)(duLength - LacbusPcProtocol.LacbusRtuDUHeaderLength - LacbusPcProtocol.LacbusRtuBlockHeaderLength);
                                    if (addFramingByte == true)
                                    {
                                        aux--;
                                    }
                                    requestFrame[frameSize++] = (byte)(aux >> 8);
                                    requestFrame[frameSize++] = (byte)aux;
                                    // Data
                                    // Datum Number & Datum Format
                                    aux = (ushort)(job.LacbusPCDatumNumber << 4);
                                    requestFrame[frameSize++] = (byte)(aux >> 8);
                                    aux &= 0xf0;
                                    requestFrame[frameSize++] = (byte)(aux + 0x03);
                                    // Value
                                    if (valueData[0] != 0)
                                    {
                                        requestFrame[frameSize++] = 0xff;
                                    }
                                    else
                                    {
                                        requestFrame[frameSize++] = 0;
                                    }
                                    // Lock action
                                    if (lockData[0] != 0)
                                    {
                                        // Lock
                                        requestFrame[frameSize++] = 1;
                                    }
                                    else
                                    {
                                        // Unlock
                                        requestFrame[frameSize++] = 3;
                                    }
                                    // Framing byte
                                    if (addFramingByte == true)
                                    {
                                        requestFrame[frameSize++] = 0;
                                    }
                                }
                            }
                            break;

                        case LacbusPcUnderlyingProtocols.SofbusPL:
                            {
                                var listOnWriting = new List<Tag>();
                                lock (job.retLockList())
                                {
                                    listOnWriting.AddRange(job.TagsListOnWriting);
                                }
                                if (listOnWriting.Count == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 8");
                                    break;
                                }
                                byte[] writingData = (byte[])jobData;
                                int itemSize = 1;
                                if (writingData.Count() < itemSize)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 9");
                                    break;
                                }

                                // Lacbus PC header
                                LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                                messageHeader.headerProtocolVersion = 1;
                                messageHeader.headerUnderlyingProtocol = underlyingProtocol;
                                messageHeader.headerMessageType = 0x43; // 'C' message
                                messageHeader.headerRtuNumber = lStation.LacbusPCRTUNumber;
                                ushort duLength = (ushort)(LacbusPcProtocol.LacbusRtuDUHeaderLength + LacbusPcProtocol.LacbusRtuBlockHeaderLength + 2 + itemSize);
                                bool addFramingByte = false;
                                if ((duLength % 2) > 0)
                                {
                                    addFramingByte = true;
                                    duLength++;
                                }
                                messageHeader.headerDataLength = duLength;
                                messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                                frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);

                                // Data Unit Header
                                LacbusRtuDataUnitHeader duHeader = new LacbusRtuDataUnitHeader();
                                duHeader.DUTotalLength = (ushort)(duLength / 2); // Number of words
                                duHeader.DURtuNumber = lStation.LacbusPCRTUNumber;
                                duHeader.DUType = 0x43; // Data Unit 'C'
                                duHeader.DUProtocolVersion = 0;
                                LacbusPcProtocol.PrepareLacbusRtuDuHeader(ref requestFrame, duHeader, ref frameSize);

                                // Data block 'a'
                                requestFrame[frameSize++] = 0x61;
                                // Body Length
                                ushort aux = (ushort)(duLength - LacbusPcProtocol.LacbusRtuDUHeaderLength - LacbusPcProtocol.LacbusRtuBlockHeaderLength);
                                if (addFramingByte == true)
                                {
                                    aux--;
                                }
                                requestFrame[frameSize++] = (byte)(aux >> 8);
                                requestFrame[frameSize++] = (byte)aux;
                                // Data
                                // Datum Number
                                requestFrame[frameSize++] = (byte)(job.LacbusPCDatumNumber >> 8);
                                requestFrame[frameSize++] = (byte)job.LacbusPCDatumNumber;
                                // Value
                                if (writingData[0] != 0)
                                {
                                    requestFrame[frameSize++] = 1;
                                }
                                else
                                {
                                    requestFrame[frameSize++] = 0;
                                }
                                // Framing byte
                                if (addFramingByte == true)
                                {
                                    requestFrame[frameSize++] = 0;
                                }
                            }
                            break;
                    }
                    break;

                case DatumTypes.AnalogOutput:
                    switch (underlyingProtocol)
                    {
                        case LacbusPcUnderlyingProtocols.LacbusPC:
                            {
                                var listOnWriting = new List<Tag>();
                                lock (job.retLockList())
                                {
                                    listOnWriting.AddRange(job.TagsListOnWriting);
                                }
                                if (listOnWriting.Count == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 10");
                                    break;
                                }
                                byte[] auxArray = (byte[])jobData;
                                if ((auxArray == null) || auxArray.GetLength(0) == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 11");
                                    break;
                                }
                                byte[] writingData;
                                uint tagDataType = (uint)listOnWriting[0].TagNode.DataType.Identifier;
                                int itemSize = 8;
                                if (tagDataType == (uint)BuiltInType.Float)
                                {
                                    itemSize = 4;
                                    float floatValue = BitConverter.ToSingle(auxArray, 0);
                                    writingData = BitConverter.GetBytes(floatValue);
                                }
                                else
                                {
                                    double doubleValue = 0.0;
                                    if (LacbusPcProtocol.ConvertByteArrayToDoubleValue(auxArray, (BuiltInType)tagDataType, ref doubleValue) == false)
                                    {
                                        break;
                                    }
                                    writingData = BitConverter.GetBytes(doubleValue);
                                }
                                if (writingData.Count() < itemSize)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 12");
                                    break;
                                }

                                if (!job.isProtocolBool())
                                {
                                    if (job.SwapBytes)
                                    {
                                        CommJob.SwapByteBuffer(ref writingData);
                                    }

                                    if (job.SwapWords)
                                    {
                                        CommJob.SwapWordBuffer(ref writingData);
                                    }
                                }

                                // Lacbus PC header
                                LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                                messageHeader.headerProtocolVersion = 1;
                                messageHeader.headerUnderlyingProtocol = LacbusPcUnderlyingProtocols.LacbusPC;
                                messageHeader.headerMessageType = 0x50; // 'P' message
                                messageHeader.headerRtuNumber = 0;
                                ushort aux = (ushort)(6 + itemSize);
                                messageHeader.headerDataLength = aux;
                                frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);
                                // Data block 'C'
                                requestFrame[frameSize++] = 0x43;
                                // Body length
                                aux -= 3;
                                requestFrame[frameSize++] = (byte)(aux >> 8);
                                requestFrame[frameSize++] = (byte)aux;
                                // Data
                                // Datum Number
                                aux = (ushort)job.LacbusPCDatumNumber;
                                requestFrame[frameSize++] = (byte)(aux >> 8);
                                requestFrame[frameSize++] = (byte)aux;
                                // Datum Format
                                if(itemSize == 8)
                                {
                                    requestFrame[frameSize++] = 2; // 64-bit floating point
                                }
                                else
                                {
                                    requestFrame[frameSize++] = 3; // 32-bit floating point
                                }
                                // Value
                                int i = 0;
                                byte[] auxBuffer = new byte[itemSize];
                                for (i = 0; i < itemSize; i++)
                                {
                                    auxBuffer[itemSize - 1 - i] = writingData[i];
                                }
                                for (i = 0; i < itemSize; i++)
                                {
                                    requestFrame[frameSize++] = auxBuffer[i];
                                }
                            }
                            break;

                        case LacbusPcUnderlyingProtocols.LacbusRTU:
                            {
                                var listOnWriting = new List<Tag>();
                                lock (job.retLockList())
                                {
                                    listOnWriting.AddRange(job.TagsListOnWriting);
                                }
                                if (listOnWriting.Count == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 13");
                                    break;
                                }

                                if (!job.LockMustBeManaged())
                                {
                                    byte[] auxArray = (byte[])jobData;
                                    if ((auxArray == null) || auxArray.GetLength(0) == 0)
                                    {
                                        System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 14");
                                        break;
                                    }
                                    byte[] writingData;
                                    uint tagDataType = (uint)listOnWriting[0].TagNode.DataType.Identifier;
                                    int itemSize = 8;
                                    if (tagDataType == (uint)BuiltInType.Float)
                                    {
                                        itemSize = 4;
                                        float floatValue = BitConverter.ToSingle(auxArray, 0);
                                        writingData = BitConverter.GetBytes(floatValue);
                                    }
                                    else
                                    {
                                        double doubleValue = 0.0;
                                        if (LacbusPcProtocol.ConvertByteArrayToDoubleValue(auxArray, (BuiltInType)tagDataType, ref doubleValue) == false)
                                        {
                                            System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 15");
                                            break;
                                        }
                                        writingData = BitConverter.GetBytes(doubleValue);
                                    }
                                    if (writingData.Count() < itemSize)
                                    {
                                        System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 16");
                                        break;
                                    }

                                    if (!job.isProtocolBool())
                                    {
                                        if (job.SwapBytes)
                                        {
                                            CommJob.SwapByteBuffer(ref writingData);
                                        }

                                        if (job.SwapWords)
                                        {
                                            CommJob.SwapWordBuffer(ref writingData);
                                        }
                                    }

                                    // Lacbus PC header
                                    LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                                    messageHeader.headerProtocolVersion = 1;
                                    messageHeader.headerUnderlyingProtocol = underlyingProtocol;
                                    messageHeader.headerMessageType = 0x43; // 'C' message
                                    messageHeader.headerRtuNumber = lStation.LacbusPCRTUNumber;
                                    ushort duLength = (ushort)(LacbusPcProtocol.LacbusRtuDUHeaderLength + LacbusPcProtocol.LacbusRtuBlockHeaderLength + 3 + itemSize);
                                    bool addFramingByte = false;
                                    if ((duLength % 2) > 0)
                                    {
                                        addFramingByte = true;
                                        duLength++;
                                    }
                                    messageHeader.headerDataLength = duLength;
                                    messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                                    frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);

                                    // Data Unit Header
                                    LacbusRtuDataUnitHeader duHeader = new LacbusRtuDataUnitHeader();
                                    duHeader.DUTotalLength = (ushort)(duLength / 2); // Number of words
                                    duHeader.DURtuNumber = lStation.LacbusPCRTUNumber;
                                    duHeader.DUType = 0x43; // Data Unit 'C'
                                    duHeader.DUProtocolVersion = 0;
                                    LacbusPcProtocol.PrepareLacbusRtuDuHeader(ref requestFrame, duHeader, ref frameSize);

                                    // Data block 'f'
                                    requestFrame[frameSize++] = 0x66;
                                    // Body Length
                                    ushort aux = (ushort)(duLength - LacbusPcProtocol.LacbusRtuDUHeaderLength - LacbusPcProtocol.LacbusRtuBlockHeaderLength);
                                    if (addFramingByte == true)
                                    {
                                        aux--;
                                    }
                                    requestFrame[frameSize++] = (byte)(aux >> 8);
                                    requestFrame[frameSize++] = (byte)aux;
                                    // Data
                                    // Datum Number & Datum Format
                                    aux = (ushort)(job.LacbusPCDatumNumber << 4);
                                    requestFrame[frameSize++] = (byte)(aux >> 8);
                                    aux &= 0xf0;
                                    if (itemSize == 8)
                                    {
                                        requestFrame[frameSize++] = (byte)(aux + 0x05); // 64-bit floating point
                                    }
                                    else
                                    {
                                        requestFrame[frameSize++] = (byte)(aux + 0x07); // 32-bit floating point
                                    }
                                    // Value
                                    byte[] auxBuffer = new byte[itemSize];
                                    int i = 0;
                                    for (i = 0; i < itemSize; i++)
                                    {
                                        auxBuffer[itemSize - 1 - i] = writingData[i];
                                    }
                                    for (i = 0; i < itemSize; i++)
                                    {
                                        requestFrame[frameSize++] = auxBuffer[i];
                                    }
                                    // Lock action (lock and set value)
                                    requestFrame[frameSize++] = 1;
                                    // Framing byte
                                    if (addFramingByte == true)
                                    {
                                        requestFrame[frameSize++] = 0;
                                    }
                                }
                                else
                                {
                                    // Special case, manage two tags: value and lock status
                                    if (listOnWriting.Count != 2)
                                    {
                                        System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 15a");
                                        break;
                                    }
                                    byte[] valueData = new byte[listOnWriting[0].Size];
                                    byte[] lockData = new byte[listOnWriting[1].Size];
                                    listOnWriting[0].GetTagBuffer(ref valueData, false);
                                    listOnWriting[1].GetTagBuffer(ref lockData, false);
                                    byte[] writingData;
                                    uint tagDataType = (uint)listOnWriting[0].TagNode.DataType.Identifier;
                                    int itemSize = 8;
                                    if (tagDataType == (uint)BuiltInType.Float)
                                    {
                                        itemSize = 4;
                                        float floatValue = BitConverter.ToSingle(valueData, 0);
                                        writingData = BitConverter.GetBytes(floatValue);
                                    }
                                    else
                                    {
                                        double doubleValue = 0.0;
                                        if (LacbusPcProtocol.ConvertByteArrayToDoubleValue(valueData, (BuiltInType)tagDataType, ref doubleValue) == false)
                                        {
                                            System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 15b");
                                            break;
                                        }
                                        writingData = BitConverter.GetBytes(doubleValue);
                                    }
                                    if (writingData.Count() < itemSize)
                                    {
                                        System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 16a");
                                        break;
                                    }

                                    if (!job.isProtocolBool())
                                    {
                                        if (job.SwapBytes)
                                        {
                                            CommJob.SwapByteBuffer(ref writingData);
                                        }

                                        if (job.SwapWords)
                                        {
                                            CommJob.SwapWordBuffer(ref writingData);
                                        }
                                    }

                                    // Lacbus PC header
                                    LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                                    messageHeader.headerProtocolVersion = 1;
                                    messageHeader.headerUnderlyingProtocol = underlyingProtocol;
                                    messageHeader.headerMessageType = 0x43; // 'C' message
                                    messageHeader.headerRtuNumber = lStation.LacbusPCRTUNumber;
                                    ushort duLength = (ushort)(LacbusPcProtocol.LacbusRtuDUHeaderLength + LacbusPcProtocol.LacbusRtuBlockHeaderLength + 3 + itemSize);
                                    bool addFramingByte = false;
                                    if ((duLength % 2) > 0)
                                    {
                                        addFramingByte = true;
                                        duLength++;
                                    }
                                    messageHeader.headerDataLength = duLength;
                                    messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                                    frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);

                                    // Data Unit Header
                                    LacbusRtuDataUnitHeader duHeader = new LacbusRtuDataUnitHeader();
                                    duHeader.DUTotalLength = (ushort)(duLength / 2); // Number of words
                                    duHeader.DURtuNumber = lStation.LacbusPCRTUNumber;
                                    duHeader.DUType = 0x43; // Data Unit 'C'
                                    duHeader.DUProtocolVersion = 0;
                                    LacbusPcProtocol.PrepareLacbusRtuDuHeader(ref requestFrame, duHeader, ref frameSize);

                                    // Data block 'f'
                                    requestFrame[frameSize++] = 0x66;
                                    // Body Length
                                    ushort aux = (ushort)(duLength - LacbusPcProtocol.LacbusRtuDUHeaderLength - LacbusPcProtocol.LacbusRtuBlockHeaderLength);
                                    if (addFramingByte == true)
                                    {
                                        aux--;
                                    }
                                    requestFrame[frameSize++] = (byte)(aux >> 8);
                                    requestFrame[frameSize++] = (byte)aux;
                                    // Data
                                    // Datum Number & Datum Format
                                    aux = (ushort)(job.LacbusPCDatumNumber << 4);
                                    requestFrame[frameSize++] = (byte)(aux >> 8);
                                    aux &= 0xf0;
                                    if (itemSize == 8)
                                    {
                                        requestFrame[frameSize++] = (byte)(aux + 0x05); // 64-bit floating point
                                    }
                                    else
                                    {
                                        requestFrame[frameSize++] = (byte)(aux + 0x07); // 32-bit floating point
                                    }
                                    // Value
                                    byte[] auxBuffer = new byte[itemSize];
                                    int i = 0;
                                    for (i = 0; i < itemSize; i++)
                                    {
                                        auxBuffer[itemSize - 1 - i] = writingData[i];
                                    }
                                    
                                    for (i = 0; i < itemSize; i++)
                                    {
                                        requestFrame[frameSize++] = auxBuffer[i];
                                    }
                                    // Lock action
                                    if (lockData[0] != 0)
                                    {
                                        // Lock
                                        requestFrame[frameSize++] = 1;
                                    }
                                    else
                                    {
                                        // Unlock
                                        requestFrame[frameSize++] = 3;
                                    }
                                    // Framing byte
                                    if (addFramingByte == true)
                                    {
                                        requestFrame[frameSize++] = 0;
                                    }
                                }
                            }
                            break;

                        case LacbusPcUnderlyingProtocols.SofbusPL:
                            {
                                var listOnWriting = new List<Tag>();
                                lock (job.retLockList())
                                {
                                    listOnWriting.AddRange(job.TagsListOnWriting);
                                }
                                if (listOnWriting.Count == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 17");
                                    break;
                                }
                                byte[] auxArray = (byte[])jobData;
                                if ((auxArray == null) || auxArray.GetLength(0) == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 18");
                                    break;
                                }
                                uint tagDataType = (uint)listOnWriting[0].TagNode.DataType.Identifier;
                                int itemSize = 2;
                                Int16 int16Value = 0;
                                // If requested, apply the data conversion
                                if (job.ConversionCanBeApplied())
                                {
                                    double doubleValue = 0.0;
                                    if (LacbusPcProtocol.ConvertByteArrayToDoubleValue(auxArray, (BuiltInType)tagDataType, ref doubleValue) == false)
                                    {
                                        System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 19a");
                                        break;
                                    }
                                    int16Value = (Int16)job.ApplyDataInverseConversion(doubleValue);
                                }
                                else
                                {
                                    if (LacbusPcProtocol.ConvertByteArrayToInt16Value(auxArray, (BuiltInType)tagDataType, ref int16Value) == false)
                                    {
                                        System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 19");
                                        break;
                                    }
                                }
                                byte[] writingData = BitConverter.GetBytes(int16Value);
                                if (writingData.Count() < itemSize)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 20");
                                    break;
                                }

                                 if (!job.isProtocolBool())
                                {
                                    if (job.SwapBytes)
                                    {
                                        CommJob.SwapByteBuffer(ref writingData);
                                    }

                                    if (job.SwapWords)
                                    {
                                        CommJob.SwapWordBuffer(ref writingData);
                                    }
                                }

                                // Lacbus PC header
                                LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                                messageHeader.headerProtocolVersion = 1;
                                messageHeader.headerUnderlyingProtocol = underlyingProtocol;
                                messageHeader.headerMessageType = 0x43; // 'C' message
                                messageHeader.headerRtuNumber = lStation.LacbusPCRTUNumber;
                                ushort duLength = (ushort)(LacbusPcProtocol.LacbusRtuDUHeaderLength + LacbusPcProtocol.LacbusRtuBlockHeaderLength + 3 + itemSize);
                                bool addFramingByte = false;
                                if ((duLength % 2) > 0)
                                {
                                    addFramingByte = true;
                                    duLength++;
                                }
                                messageHeader.headerDataLength = duLength;
                                messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                                frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);

                                // Data Unit Header
                                LacbusRtuDataUnitHeader duHeader = new LacbusRtuDataUnitHeader();
                                duHeader.DUTotalLength = (ushort)(duLength / 2); // Number of words
                                duHeader.DURtuNumber = lStation.LacbusPCRTUNumber;
                                duHeader.DUType = 0x43; // Data Unit 'C'
                                duHeader.DUProtocolVersion = 0;
                                LacbusPcProtocol.PrepareLacbusRtuDuHeader(ref requestFrame, duHeader, ref frameSize);

                                // Data block 'b'
                                requestFrame[frameSize++] = 0x62;
                                // Body Length
                                ushort aux = (ushort)(duLength - LacbusPcProtocol.LacbusRtuDUHeaderLength - LacbusPcProtocol.LacbusRtuBlockHeaderLength);
                                if (addFramingByte == true)
                                {
                                    aux--;
                                }
                                requestFrame[frameSize++] = (byte)(aux >> 8);
                                requestFrame[frameSize++] = (byte)aux;
                                // Data
                                // Datum Number
                                requestFrame[frameSize++] = (byte)(job.LacbusPCDatumNumber >> 8);
                                requestFrame[frameSize++] = (byte)job.LacbusPCDatumNumber;
                                // Forcing (unforced)
                                requestFrame[frameSize++] = 0;
                                // Value
                                requestFrame[frameSize++] = writingData[1];
                                requestFrame[frameSize++] = writingData[0];
                                // Framing byte
                                if (addFramingByte == true)
                                {
                                    requestFrame[frameSize++] = 0;
                                }
                            }
                            break;
                    }
                    break;

                case DatumTypes.CountInput:
                    switch (underlyingProtocol)
                    {
                        case LacbusPcUnderlyingProtocols.LacbusRTU:
                            {
                                var listOnWriting = new List<Tag>();
                                lock (job.retLockList())
                                {
                                    listOnWriting.AddRange(job.TagsListOnWriting);
                                }
                                if (listOnWriting.Count == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 21");
                                    break;
                                }
                                byte[] auxArray = (byte[])jobData;
                                if ((auxArray == null) || auxArray.GetLength(0) == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 22");
                                    break;
                                }
                                uint tagDataType = (uint)listOnWriting[0].TagNode.DataType.Identifier;
                                int itemSize = 4;
                                float floatValue = 0.0F;
                                if (LacbusPcProtocol.ConvertByteArrayToFloatValue(auxArray, (BuiltInType)tagDataType, ref floatValue) == false)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 23");
                                    break;
                                }
                                byte[] writingData = BitConverter.GetBytes(floatValue);
                                if (writingData.Count() < itemSize)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 24");
                                    break;
                                }

                                // Lacbus PC header
                                LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                                messageHeader.headerProtocolVersion = 1;
                                messageHeader.headerUnderlyingProtocol = underlyingProtocol;
                                messageHeader.headerMessageType = 0x43; // 'C' message
                                messageHeader.headerRtuNumber = lStation.LacbusPCRTUNumber;
                                ushort duLength = (ushort)(LacbusPcProtocol.LacbusRtuDUHeaderLength + LacbusPcProtocol.LacbusRtuBlockHeaderLength + 3 + itemSize);
                                bool addFramingByte = false;
                                if ((duLength % 2) > 0)
                                {
                                    addFramingByte = true;
                                    duLength++;
                                }
                                messageHeader.headerDataLength = duLength;
                                messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                                frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);

                                // Data Unit Header
                                LacbusRtuDataUnitHeader duHeader = new LacbusRtuDataUnitHeader();
                                duHeader.DUTotalLength = (ushort)(duLength / 2); // Number of words
                                duHeader.DURtuNumber = lStation.LacbusPCRTUNumber;
                                duHeader.DUType = 0x43; // Data Unit 'C'
                                duHeader.DUProtocolVersion = 0;
                                LacbusPcProtocol.PrepareLacbusRtuDuHeader(ref requestFrame, duHeader, ref frameSize);

                                // Data block 'f'
                                requestFrame[frameSize++] = 0x66;
                                // Body Length
                                ushort aux = (ushort)(duLength - LacbusPcProtocol.LacbusRtuDUHeaderLength - LacbusPcProtocol.LacbusRtuBlockHeaderLength);
                                if (addFramingByte == true)
                                {
                                    aux--;
                                }
                                requestFrame[frameSize++] = (byte)(aux >> 8);
                                requestFrame[frameSize++] = (byte)aux;
                                // Data
                                // Datum Number & Datum Format
                                aux = (ushort)(job.LacbusPCDatumNumber << 4);
                                requestFrame[frameSize++] = (byte)(aux >> 8);
                                aux &= 0xf0;
                                requestFrame[frameSize++] = (byte)(aux + 0x06); // Input floating point 32bit
                                // Value
                                byte[] auxBuffer = new byte[itemSize];
                                int i = 0;
                                for (i = 0; i < itemSize; i++)
                                {
                                    auxBuffer[itemSize - 1 - i] = writingData[i];
                                }
                                for (i = 0; i < itemSize; i++)
                                {
                                    requestFrame[frameSize++] = auxBuffer[i];
                                }
                                // Lock action (set value no lock)
                                requestFrame[frameSize++] = 2;
                                // Framing byte
                                if (addFramingByte == true)
                                {
                                    requestFrame[frameSize++] = 0;
                                }
                            }
                            break;

                        case LacbusPcUnderlyingProtocols.SofbusPL:
                            {
                                var listOnWriting = new List<Tag>();
                                lock (job.retLockList())
                                {
                                    listOnWriting.AddRange(job.TagsListOnWriting);
                                }
                                if (listOnWriting.Count == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 25");
                                    break;
                                }
                                byte[] auxArray = (byte[])jobData;
                                if ((auxArray == null) || auxArray.GetLength(0) == 0)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 26");
                                    break;
                                }
                                uint tagDataType = (uint)listOnWriting[0].TagNode.DataType.Identifier;
                                int itemSize = 4;
                                Int32 int32Value = 0;
                                // If requested, apply the data conversion
                                if (job.ConversionCanBeApplied())
                                {
                                    double doubleValue = 0.0;
                                    if (LacbusPcProtocol.ConvertByteArrayToDoubleValue(auxArray, (BuiltInType)tagDataType, ref doubleValue) == false)
                                    {
                                        System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 27a");
                                        break;
                                    }
                                    int32Value = (Int32)job.ApplyDataInverseConversion(doubleValue);
                                }
                                else
                                {
                                    if (LacbusPcProtocol.ConvertByteArrayToInt32Value(auxArray, (BuiltInType)tagDataType, ref int32Value) == false)
                                    {
                                        System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 27");
                                        break;
                                    }
                                }
                                byte[] writingData = BitConverter.GetBytes(int32Value);
                                if (writingData.Count() < itemSize)
                                {
                                    System.Diagnostics.Debug.WriteLine("PrepareRequestFrame error 28");
                                    break;
                                }

                                // Lacbus PC header
                                LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                                messageHeader.headerProtocolVersion = 1;
                                messageHeader.headerUnderlyingProtocol = underlyingProtocol;
                                messageHeader.headerMessageType = 0x43; // 'C' message
                                messageHeader.headerRtuNumber = lStation.LacbusPCRTUNumber;
                                ushort duLength = (ushort)(LacbusPcProtocol.LacbusRtuDUHeaderLength + LacbusPcProtocol.LacbusRtuBlockHeaderLength + 2 + itemSize);
                                bool addFramingByte = false;
                                if ((duLength % 2) > 0)
                                {
                                    addFramingByte = true;
                                    duLength++;
                                }
                                messageHeader.headerDataLength = duLength;
                                messageHeader.headerPhoneNumber = lStation.LacbusPCPhoneNumber;
                                frameSize = LacbusPcProtocol.PrepareLacbusPCMessageHeader(ref requestFrame, messageHeader);

                                // Data Unit Header
                                LacbusRtuDataUnitHeader duHeader = new LacbusRtuDataUnitHeader();
                                duHeader.DUTotalLength = (ushort)(duLength / 2); // Number of words
                                duHeader.DURtuNumber = lStation.LacbusPCRTUNumber;
                                duHeader.DUType = 0x43; // Data Unit 'C'
                                duHeader.DUProtocolVersion = 0;
                                LacbusPcProtocol.PrepareLacbusRtuDuHeader(ref requestFrame, duHeader, ref frameSize);

                                // Data block 'c'
                                requestFrame[frameSize++] = 0x63;
                                // Body Length
                                ushort aux = (ushort)(duLength - LacbusPcProtocol.LacbusRtuDUHeaderLength - LacbusPcProtocol.LacbusRtuBlockHeaderLength);
                                if (addFramingByte == true)
                                {
                                    aux--;
                                }
                                requestFrame[frameSize++] = (byte)(aux >> 8);
                                requestFrame[frameSize++] = (byte)aux;
                                // Data
                                // Datum Number
                                requestFrame[frameSize++] = (byte)(job.LacbusPCDatumNumber >> 8);
                                requestFrame[frameSize++] = (byte)job.LacbusPCDatumNumber;
                                // Value
                                requestFrame[frameSize++] = writingData[3];
                                requestFrame[frameSize++] = writingData[2];
                                requestFrame[frameSize++] = writingData[1];
                                requestFrame[frameSize++] = writingData[0];
                                // Framing byte
                                if (addFramingByte == true)
                                {
                                    requestFrame[frameSize++] = 0;
                                }
                            }
                            break;
                    }
                    break;
            }

            return (frameSize);
        }

        bool UseSecondMediumForPollRTURequest(LacbusPCStation lStation)
        {
            bool returnValue = false;

            bool useSecondMediumValue = false;
            if (lStation.GetStateCommandVariableBit(ref useSecondMediumValue, (UInt16)LacbusPCStationVariableBits.StationUseSecondMedium) == true)
            {
                if(useSecondMediumValue == true)
                {
                    returnValue = true;
                }
            }

            return (returnValue);
        }

        bool SendPollRTURequest(LacbusPCCommJob lJob)
        {
            bool requestSent = false;
            LacbusPCStation lStation = (LacbusPCStation)lJob.Station;
            if (IsActiveCommunicationChannelOpened() || OpenActiveCommunicationChannel())
            {
                uint requestLength = 0;
                byte[] requestFrame = null;
                ExecuteJob(lJob);
                lock (lJob.retLockList())
                {
                    if (lJob.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                    {
                        if (lJob.TagsListToWrite.Count == 0)
                        {
                            lJob.TagsListToWrite.AddRange(lJob.TagsList);
                        }
                    }
                    if (lJob.TagsListToWrite.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("SendPollRTURequest(lJob) error 1");
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorPreparingFrameRequest;
                        return (false);
                    }
                }

                // Build the request message
                object writingData = null;
                lJob.GetJobData(ref writingData);
                lock (lJob.retLockList())
                {
                    if (lJob.TagsListOnWriting.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("SendPollRTURequest(lJob) error 2");
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorPreparingFrameRequest;
                        return (false);
                    }
                }
                bool useSecondMedium = false;
                if(lJob.LacbusPCDatumNumber != 0)
                {
                    useSecondMedium = true;
                }
                uint dim = GetPollRTURequestFrameLength(useSecondMedium);
                requestFrame = new byte[dim];
                requestLength = PreparePollRTURequestFrame(lStation, useSecondMedium, lJob.LacbusPCCommunicationDuration, ref requestFrame);
                if (requestLength == 0)
                {
                    LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorPreparingFrameRequest;
                    lock (lockThreadObject)
                    {
                        RemovePendingJob(lJob);
                        lJob.IsPending = false;
                    }

                    // Close the channel
                    CloseActiveCommunicationChannel();
                    System.Diagnostics.Debug.WriteLine("SendPollRTURequest(lJob) error 3");
                }

                else
                {
                    // Send the request
                    unsafe
                    {
                        int returnValue = chVortexDevManager.vortex_channel_send_msg(supervisorSendChannel, ref requestFrame, (int)requestLength, ref lastRequestMessageNumber);
                        if (returnValue == 0)
                        {
                            requestSent = true;

                            // Store the message number
                            lJob.requestMessageNumber = lastRequestMessageNumber;

                            // Update the last activity time on channel
                            lastActivityTimeOnSendChannel = DateTime.UtcNow;

                            lJob.LastExecutionTime = DateTime.UtcNow;
                            lJob.LacbusPCStatus = LacbusPCCommJobStatus.RequestPending;
                        }
                        else
                        {
                            lock (lockThreadObject)
                            {
                                RemovePendingJob(lJob);
                                lJob.IsPending = false;
                            }
                            LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorSendingFrameRequest;

                            // Close the channel
                            CloseActiveCommunicationChannel();
                            System.Diagnostics.Debug.WriteLine("SendPollRTURequest(lJob) error 4");
                        }
                    }
                }
            }
            else
            {
                if (chVortexDevManager != null)
                {
                    if (chVortexDevManager.ErrorDescription.Contains("501"))
                    {
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannelSyntax;
                    }
                    else if (chVortexDevManager.ErrorDescription.Contains("540"))
                    {
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannelAuthentication;
                    }
                    else
                    {
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannel;
                    }
                }
                else
                {
                    LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannel;
                }
#if DEBUG
                System.Diagnostics.Debug.WriteLine("SendPollRTURequest(lJob) error 5 - {0} - {1}, Station {2}", DateTime.Now, LastErrorCode, lStation.Name);
#endif
            }

            return (requestSent);
        }

        bool SendPollRTURequest(LacbusPCStation lStation)
        {
            bool requestSent = false;
            if (IsActiveCommunicationChannelOpened() || OpenActiveCommunicationChannel())
            {
                // Build the request message
                uint requestLength = 0;
                byte[] requestFrame = null;
                bool useSecondMedium = UseSecondMediumForPollRTURequest(lStation);
                uint dim = GetPollRTURequestFrameLength(useSecondMedium);
                requestFrame = new byte[dim];
                requestLength = PreparePollRTURequestFrame(lStation, useSecondMedium, lStation.LacbusPCCommunicationDuration, ref requestFrame);
                if (requestLength == 0)
                {
                    LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorPreparingFrameRequest;

                    // Close the channel
                    CloseActiveCommunicationChannel();
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - SendPollRTURequest error 1", DateTime.Now);
#endif
                }
                else
                {
                    // Send the request
                    unsafe
                    {
                        int returnValue = chVortexDevManager.vortex_channel_send_msg(supervisorSendChannel, ref requestFrame, (int)requestLength, ref lastRequestMessageNumber);
                        if (returnValue == 0)
                        {
                            requestSent = true;

                            // Store the message number
                            lStation.pollRTURequestMessageNumber = lastRequestMessageNumber;

                            // Update the last activity time on channel
                            lastActivityTimeOnSendChannel = DateTime.UtcNow;

                            lStation.pollRTURequestPending = true;
#if DEBUG
                            System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - SendPollRTURequest: request sent for station {1}", DateTime.Now, lStation.Name);
#endif
                        }
                        else
                        {
                            LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorSendingFrameRequest;

                            // Close the channel
                            CloseActiveCommunicationChannel();
#if DEBUG
                            System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - SendPollRTURequest error 2, Station", DateTime.Now, lStation.Name);
#endif
                        }
                    }
                }
            }
            else
            {
                if (chVortexDevManager != null)
                {
                    if (chVortexDevManager.ErrorDescription.Contains("501"))
                    {
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannelSyntax;
                    }
                    else if (chVortexDevManager.ErrorDescription.Contains("540"))
                    {
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannelAuthentication;
                    }
                    else
                    {
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannel;
                    }
                }
                else
                {
                    LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannel;
                }
#if DEBUG
                System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - SendPollRTURequest error 3 - {1}, Station {2}", DateTime.Now, LastErrorCode, lStation.Name);
#endif
            }

            return (requestSent);
        }

        bool SendRequest(CommJob job)
        {
            LacbusPCCommJob lJob = (LacbusPCCommJob)job;
            if(lJob == null)
            {
                System.Diagnostics.Debug.WriteLine("SendRequest error 1");
                return (false);
            }

            LacbusPCStation lStation = (LacbusPCStation)lJob.Station;

            bool requestSent = false;
            if (IsActiveCommunicationChannelOpened() || OpenActiveCommunicationChannel())
            {
                uint requestLength = 0;
                byte[] requestFrame = null;
                var lkJob = lJob.retLockList();
                lock (lkJob)
                {
                    ExecuteJob(job);
                    if (job.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                    {
                        if (job.TagsListToWrite.Count == 0)
                            job.TagsListToWrite.AddRange(job.TagsList);
                    }
                    if (job.TagsListToWrite.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("SendRequest error 2");
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorPreparingFrameRequest;
                        return (false);
                    }

                    // Build the request message
                    object writingData = null;
                    lJob.GetJobData(ref writingData);
                    if (job.TagsListOnWriting.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("SendRequest error 3");
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorPreparingFrameRequest;
                        return (false);
                    }

                    uint dim = GetRequestFrameLength(lJob, lStation.LacbusPCProtocolType);
                    if (dim > 0)
                    {
                        requestFrame = new byte[dim];
                        requestLength = PrepareRequestFrame(lJob, writingData, ref requestFrame, lStation.LacbusPCProtocolType);
                    }
                    else
                    {
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorPreparingFrameRequest;
                        System.Diagnostics.Debug.WriteLine("SendRequest error 3 bis");
                    }
                }
                if (requestLength == 0)
                {
                    LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorPreparingFrameRequest;
                    lock (lockThreadObject)
                    {
                        RemovePendingJob(job);
                        job.IsPending = false;
                    }

                    // Close the channel
                    CloseActiveCommunicationChannel();
                    System.Diagnostics.Debug.WriteLine("SendRequest error 4");
                }
                else
                {
                    // Send the request
                    unsafe
                    {
                        int returnValue = chVortexDevManager.vortex_channel_send_msg(supervisorSendChannel, ref requestFrame, (int)requestLength, ref lastRequestMessageNumber);
                        if (returnValue == 0)
                        {
                            requestSent = true;

                            // Store the message number
                            lJob.requestMessageNumber = lastRequestMessageNumber;

                            // Update the last activity time on channel
                            lastActivityTimeOnSendChannel = DateTime.UtcNow;

                            job.LastExecutionTime = DateTime.UtcNow;
                            lJob.LacbusPCStatus = LacbusPCCommJobStatus.RequestPending;
                        }
                        else
                        {
                            lock (lockThreadObject)
                            {
                                RemovePendingJob(job);
                                job.IsPending = false;
                            }
                            LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorSendingFrameRequest;

                            // Close the channel
                            CloseActiveCommunicationChannel();
                            System.Diagnostics.Debug.WriteLine("SendRequest error 5");
                        }
                    }
                }
            }
            else
            {
                if(chVortexDevManager != null)
                {
                    if(chVortexDevManager.ErrorDescription.Contains("501"))
                    {
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannelSyntax;
                    }
                    else if (chVortexDevManager.ErrorDescription.Contains("540"))
                    {
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannelAuthentication;
                    }
                    else
                    {
                        LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannel;
                    }
                }
                else
                {
                    LastErrorCode = (DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeErrorOpeningRequestChannel;
                }
                System.Diagnostics.Debug.WriteLine("SendRequest error 5");
            }

            return (requestSent);
        }

        public override bool DeviceOpen()
        {
            lock (lockStream)
            {
                // Check if the idle time after a connection failure has been elapsed
                double dtime = (DateTime.UtcNow - lastFailedConnectionTime).TotalMilliseconds;
                if ((dtime < LacbusPCConnectionTimeout) && (switchHost == false))
                {
                    if(performDeviceCloseConnection == true)
                    {
                        DeviceCloseConnection();
                    }
                    return (false);
                }

                if (performDeviceCloseConnection == true)
                {
                    DeviceCloseConnection();
                    return (false);
                }
                else
                {
                    performDeviceCloseConnection = true;
                }

                if((firstConnectionAttempt == false) && ((switchHost == true) || !String.IsNullOrWhiteSpace(_LacbusPCBackupHostName)))
                {
                    if (backupHostIsActive == false)
                    {
                        selectedHostName = LacbusPCBackupHostName;
                        backupHostIsActive = true;
                    }
                    else
                    {
                        selectedHostName = LacbusPCHostName;
                        backupHostIsActive = false;
                    }
                }

                firstConnectionAttempt = false;

                if (vortexDevManagerInitialized == false)
                {
                    if (chVortexDevManager == null)
                    {
                        lastFailedConnectionTime = DateTime.UtcNow;
                        SetChannelVariablesBits(false);
                        return (false);
                    }

                    // Initialize the BEEP client object
                    int returnValue = chVortexDevManager.vortex_init_ctx();
                    if (returnValue != 0)
                    {
                        System.Diagnostics.Debug.WriteLine("DeviceOpen error {0} in vortex_init_ctx", returnValue);
                        lastFailedConnectionTime = DateTime.UtcNow;
                        if (lastConnectionError == 0)
                        {
                            CommDriver.OnSystemEvent(null, Properties.Resources.ErrorInitializationVortexLibrary, EventSeverity.High);
                        }
                        
                        lastConnectionError = -1;
                        SetChannelVariablesBits(false);
                        return (false);
                    }
                    vortexDevManagerInitialized = true;
                }

                if (vortexConnectionInitialized == false)
                {
                    // Register the supported communication profile
                    int returnValue = 0;
                    IntPtr intptrRemoteFrameReceivedCallbackFunc = Marshal.GetFunctionPointerForDelegate(remoteFrameReceivedCallbackFunc);
                    gcHandleRemoteFrameReceivedCallbackFunc = GCHandle.Alloc(remoteFrameReceivedCallbackFunc);
                    unsafe
                    {
                        // Register the communication profile of the remote peer
                        returnValue = chVortexDevManager.vortex_profiles_register(remoteCommunicationProfile,
                                                                                  (IntPtr)null,
                                                                                  (IntPtr)null,
                                                                                  intptrRemoteFrameReceivedCallbackFunc);
                    }
                    if (returnValue != 0)
                    {
                        System.Diagnostics.Debug.WriteLine("DeviceOpen error {0} in vortex_profiles_register", returnValue);
                        lastFailedConnectionTime = DateTime.UtcNow;
                        if (lastConnectionError == 0)
                        {
                            CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.ErrorRegisterProfile, remoteCommunicationProfile), EventSeverity.High);
                        }

                        lastConnectionError = -2;
                        SetChannelVariablesBits(false);
                        return (false);
                    }

                    // Open a TCP/IP session
                    returnValue = chVortexDevManager.vortex_open_connection(selectedHostName, LacbusPCHostPort, LacbusPCConnectionTimeout);
                    if (returnValue != 0)
                    {
                        System.Diagnostics.Debug.WriteLine("DeviceOpen error {0} in vortex_open_connection", returnValue);
                        lastFailedConnectionTime = DateTime.UtcNow;
                        if (lastConnectionError == 0)
                        {
                            CommDriver.OnSystemEvent(null, Properties.Resources.ErrorGenericCommunicationFailure, EventSeverity.High);
                        }
                        lastConnectionError = -3;
                        SetChannelVariablesBits(false);
                        return (false);
                    }

                    // Check if the communication profile PC2FR is supported
                    unsafe
                    {
                        returnValue = chVortexDevManager.vortex_connection_is_profile_supported(communicationProfile);
                        if (returnValue != 0)
                        {
                            System.Diagnostics.Debug.WriteLine("DeviceOpen error: profile PC2FR is not supported");
                            if (lastConnectionError == 0)
                            {
                                CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.ErrorUnsupportedProfile, communicationProfile), EventSeverity.High);
                            }
                            lastConnectionError = returnValue;
                            chVortexDevManager.vortex_close_connection();
                            lastFailedConnectionTime = DateTime.UtcNow;
                            SetChannelVariablesBits(false);
                            return (false);
                        }
                    }

                    // Set the handle for remote disconnection
                    IntPtr intptrOnCloseConnectionCallbackFunc = Marshal.GetFunctionPointerForDelegate(onConnectionCloseCallbackFunc);
                    gcHandleReplyOnCloseConnectionCallbackFunc = GCHandle.Alloc(onConnectionCloseCallbackFunc);
                    returnValue = chVortexDevManager.vortex_connection_set_on_close(intptrOnCloseConnectionCallbackFunc);

                    vortexConnectionInitialized = true;
                }

                if (identificationChannelCreated == false)
                {
                    unsafe
                    {
                        string pcNumber = LacbusPCPCNumber.ToString();
                        int returnValue = chVortexDevManager.vortex_channel_new_full(identificationProfile, pcNumber, 1, ref identificationChannel);
                        if (returnValue != 0)
                        {
                            System.Diagnostics.Debug.WriteLine("DeviceOpen error {0} in vortex_channel_new_full error description = {1}", returnValue, chVortexDevManager.ErrorDescription);
                            if (lastConnectionError == 0)
                            {
                                switch (returnValue)
                                {
                                    case -2:
                                        CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.ErrorUnsupportedProfile, identificationProfile), EventSeverity.High);
                                        break;
                                    case -4:
                                        CommDriver.OnSystemEvent(null, String.Format(Properties.Resources.ErrorConnections, Name, chVortexDevManager.ErrorDescription), EventSeverity.High);
                                        break;
                                    default:
                                        CommDriver.OnSystemEvent(null, Properties.Resources.ErrorGenericCommunicationFailure, EventSeverity.High);
                                        break;
                                }
                                lastConnectionError = returnValue;
                            }
                            chVortexDevManager.vortex_close_connection();
                            if (gcHandleReplyOnCloseConnectionCallbackFunc.IsAllocated)
                            {
                                gcHandleReplyOnCloseConnectionCallbackFunc.Free();
                            }
                            lastFailedConnectionTime = DateTime.UtcNow;
                            SetChannelVariablesBits(false);
                            return (false);
                        }
                    }

                    // Set keep-alive
                    int retValue = chVortexDevManager.SetKeepAlive();
                    if (retValue != 0)
                    {
                        System.Diagnostics.Debug.WriteLine("DeviceOpen error {0} in chVortexDevManager.SetKeepAlive", retValue);
                    }

                    identificationChannelCreated = true;
                    lastConnectionError = 0;
                    SetChannelVariablesBits(true);
                    CommDriver.OnSystemEvent(null, Properties.Resources.ConnectionToFR1000Established, EventSeverity.Low);
                }

                SetConnectionBroken(false);
                return (true);
            }
        }

        void SetChannelVariablesBits(bool channelConnected)
        {
            //System.Diagnostics.Debug.WriteLine("SwitchServerDEBUG - SetChannelVariablesBits {0} - channelConnected: {1} - switchHost: {2}",
            //                                   DateTime.Now.ToString("HH:mm:ss.fff"), channelConnected, switchHost);
            SetStateCommandVariableBit(backupHostIsActive, (UInt16)ChannelVariableBits.ConnectedHost);
            SetStateCommandVariableBit(!channelConnected, (UInt16)ChannelVariableBits.ChannelUnconnected);
            if (switchHost == true)
            {
                switchHost = false;
                SetStateCommandVariableBit(false, (UInt16)ChannelVariableBits.SwitchServer);
            }
        }

        public override bool DeviceClose()
        {
            lock (lockStream)
            {
                if ((chVortexDevManager != null) && (vortexDevManagerInitialized == true))
                {
                    // Close the BEEP identification channel
                    if (identificationChannelCreated == true)
                    {
                        unsafe
                        {
                            if (chVortexDevManager.vortex_channel_close(identificationChannel) != 0)
                            {
                                return (false);
                            }

                            identificationChannelCreated = false;
                            identificationChannel = null;
                        }
                    }

                    // Close the TCP/IP session
                    if (vortexConnectionInitialized == true)
                    {
                        if (chVortexDevManager.vortex_close_connection() != 0)
                        {
                            return (false);
                        }

                        vortexConnectionInitialized = false;
                    }

                    // Terminates the Vortex library execution on the provided context
                    int returnValue = chVortexDevManager.vortex_exit_ctx();
                    if (returnValue != 0)
                    {
                        return (false);
                    }

                    vortexDevManagerInitialized = false;
                    if(gcHandleRemoteFrameReceivedCallbackFunc.IsAllocated)
                    {
                        gcHandleRemoteFrameReceivedCallbackFunc.Free();
                    }
                    if(gcHandleReplyOnCloseConnectionCallbackFunc.IsAllocated)
                    {
                        gcHandleReplyOnCloseConnectionCallbackFunc.Free();
                    }
                }

                return true;
            }
        }

        bool DeviceCloseConnection()
        {
            lock (lockStream)
            {
                performDeviceCloseConnection = false;
                if ((chVortexDevManager != null) && (vortexDevManagerInitialized == true))
                {
                    // Close the BEEP identification channel
                    if (identificationChannelCreated == true)
                    {
                        unsafe
                        {
                            if (chVortexDevManager.vortex_channel_close(identificationChannel) != 0)
                            {
                                return (false);
                            }

                            identificationChannelCreated = false;
                            identificationChannel = null;
                        }
                    }

                    // Close the TCP/IP session
                    if (vortexConnectionInitialized == true)
                    {
                        if (chVortexDevManager.vortex_close_connection() != 0)
                        {
                            return (false);
                        }

                        vortexConnectionInitialized = false;
                    }

                    if (gcHandleRemoteFrameReceivedCallbackFunc.IsAllocated)
                    {
                        gcHandleRemoteFrameReceivedCallbackFunc.Free();
                    }
                    if (gcHandleReplyOnCloseConnectionCallbackFunc.IsAllocated)
                    {
                        gcHandleReplyOnCloseConnectionCallbackFunc.Free();
                    }
                }

                return true;
            }
        }

        // Not used
        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        // Not used
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        // Not used
        public override uint GetBytesToRead() { return 1; }
        // Not used
        public override uint GetBytesToWrite() { return 1; }
        public void ProcessError(CommJob pendingjob, DriverErrorCodes errorCode)
        {
            LacbusPCCommJob LacbusPendingJob = (LacbusPCCommJob)pendingjob;
            RemovePendingJob(pendingjob);
            pendingjob.IsPending = false;
            pendingjob.LastExecutionTime = DateTime.UtcNow;
            ExecutedJobArgs eJob = new ExecutedJobArgs();
            eJob.Job = LacbusPendingJob;
            eJob.ErrorCode = errorCode;
            OnJobExecuted(eJob);
        }

        protected override void WorkingThread(object data)
        {
            int sleepCycle = WaitTime;
            if (sleepCycle == 0)
                sleepCycle = 1;
            ListJobPending.Clear();
            ListJobExecuted.Clear();
            ListPollRtuJobPending.Clear();

            // Fill the station map and the polling list
            foreach (LacbusPCStation stLac in CommDriver.GetChannelStations(this))
            {
                if (stLac != null)
                {
                    mapStations[stLac.LacbusPCRTUNumber] = stLac;
                    if(stLac.AutomaticPollingEnabled())
                    {
                        pollStationList.Add(stLac);
                    }
                }
            }

            // Set the TimeZoneInfo object for time conversion
            SetChannelTimeZoneInfo();

            NextScheduleTimeJobsList = DateTime.UtcNow;
            int loop = 0;
            while (true)
            {
                CommJob nextjob = null;
                if ((ListJobPending.Count == 0) || MultiPointProtocol)
                {
                    ScheduleListJob();
                    if (SynchroJob != null)
                        nextjob = SynchroJob;
                    else
                        nextjob = GetNextLacbusPCPendingJob();
                    if (nextjob != null)
                    {
                        LacbusPCCommJob lNextJob = (LacbusPCCommJob)nextjob;
                        if(lNextJob.LacbusPCDatumType != DatumTypes.RTUPollRequest)
                        {
                            ListJobPending.Add(nextjob);
                        }
                        else
                        {
                            if(ListPollRtuJobPending.Contains(lNextJob) == false)
                            {
                                ListPollRtuJobPending.Add(lNextJob);
                            }
                            nextjob = null;
                        }
                    }
                }

                if (nextjob != null)
                {
                    bool canSendRequest = false;
                    if (IsDeviceOpen() == true)
                    {
                        canSendRequest = true;
                    }
                    else if (DeviceOpen() == true)
                    {
                        canSendRequest = true;
                        lock (lockThreadObject)
                        {
                            ManageConnectionRestored();
                        }
                    }
                    if (canSendRequest == true)
                    {
                        if (SendRequest(nextjob) == false)
                        {
                            if (SynchroJob != null && SynchroJob == nextjob)
                            {
                                nextjob.ResetSynchro.WaitOne(Timeout);
                                SynchroJob = null;
                                nextjob.ResetSynchro.Reset();
                            }
                            System.Diagnostics.Debug.WriteLine("Error sending a request");

                            ProcessError(nextjob, LastErrorCode);
                        }
                    }
                }
                else
                {
                    if (!IsDeviceOpen())
                    {
                        if(DeviceOpen() == true)
                        {
                            lock (lockThreadObject)
                            {
                                ManageConnectionRestored();
                            }
                        }
                    }
                }

                // Manage RTU Poll requests
                PollRtu(ref nextjob);

                lock (lockThreadObject)
                {
                    if (isConnectionBroken == true)
                    {
                        ManageConnectionBroken(ListJobPending);
                    }
                    else
                    {
                        if (ListJobPending.Count == 0)
                        {
                            ManageRepliesFromRemote(null);
                        }
                        else
                        {
                            ManageRepliesFromRemote(ListJobPending[0]);
                        }

                        ManageMessagesFromRemote();

                        if (ListJobPending.Count > 0)
                        {
                            foreach (var job in ListJobPending)
                            {
                                LacbusPCCommJob lJob = (LacbusPCCommJob)job;
                                if (lJob.LacbusPCStatus == LacbusPCCommJobStatus.ReplyReceived)
                                {
                                    lJob.LacbusPCStatus = LacbusPCCommJobStatus.Idle;
                                    ListJobExecuted.Add(job);
                                }
                            }

                            foreach (var job in ListJobExecuted)
                            {
                                job.LastExecutionTime = DateTime.UtcNow;
                                if (SynchroJob != null && SynchroJob == job)
                                {
                                    job.ResetSynchro.WaitOne(Timeout);
                                    SynchroJob = null;
                                    job.ResetSynchro.Reset();
                                }
                                ListJobPending.Remove(job);
                            }
                            ListJobExecuted.Clear();
                        }

                        if (ListJobPending.Count > 0)
                        {
                            if (!MultiPointProtocol)
                            {
                                double dtime = (DateTime.UtcNow - ListJobPending[0].StartExecutionTime/*LastExecutionTime*/).TotalMilliseconds;
                                if (dtime > Timeout)
                                {
                                    if (IsActiveCommunicationChannelOpened() == true)
                                    {
                                        UnreferenceActiveCommunicationChannel();
                                    }

                                    if (SynchroJob != null && SynchroJob == ListJobPending[0])
                                    {
                                        ListJobPending[0].ResetSynchro.WaitOne(Timeout);
                                        SynchroJob = null;
                                        ListJobPending[0].ResetSynchro.Reset();
                                    }

                                    //Timeout error
                                    LastErrorCode = DriverErrorCodes.ErrorTimeOut;
                                    ProcessError(ListJobPending[0], LastErrorCode);
                                    ReceiveBuffer.Clear();
                                }
                            }
                        }
                    }
                }


                if (ListJobPending.Count == 0 && (pendingPollStationList.Count == 0) && !KeepOpened && IsDeviceOpen())
                {
                    if (IsActiveCommunicationChannelOpened() == true)
                    {
                        CloseActiveCommunicationChannel();
                    }
                    DeviceClose();
                }

                if (nextjob != null && StopWorkerThread.WaitOne(WaitTime, false))
                {
                    break;
                }
                else if (++loop > 4)
                {
                    loop = 0;
                    if (StopWorkerThread.WaitOne(sleepCycle, false))
                    {
                        break;
                    }
                }
                StopWorkerThread.WaitOne(0, false);
            }

            DeviceClose();
        }

        protected override void OnJobExecuted(ExecutedJobArgs e)
        {
            if(e.ErrorCode == DriverErrorCodes.ErrorNoError)
            {
                LacbusPCStation lStation = (LacbusPCStation)e.Job.Station;
                if((lStation != null) && (lStation.rtuIsConnected == false))
                {
                    lStation.rtuIsConnected = true;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - Channel {1} - Station {2} - OnJobExecuted has been called, rtuIsConnected set to = {3}", DateTime.Now, Name, lStation.Name, lStation.rtuIsConnected);
#endif
                }
            }
            base.OnJobExecuted(e);
        }

        #endregion

        #region Specific Methods

        const string NoTimeConversion = "(No Time Conversion)";

        bool UseUtcTimeForSourceTimestamp()
        {
            bool returnValue = true;
            if(_LacbusPCTimeZone == NoTimeConversion)
            {
                returnValue = false;
            }
            return (returnValue);
        }

        void SetChannelTimeZoneInfo()
        {
            if(String.IsNullOrWhiteSpace(_LacbusPCTimeZone) || (_LacbusPCTimeZone == NoTimeConversion))
            {
                channelTimeZoneInfo = TimeZoneInfo.Local;
            }
            else
            {
                try
                {
                    channelTimeZoneInfo = TimeZoneInfo.FindSystemTimeZoneById(_LacbusPCTimeZone);
                }
                catch (Exception e)
                {
                    channelTimeZoneInfo = TimeZoneInfo.Local;
                }
            }
        }

        void PollRtu(ref CommJob nextjob)
        {
            if ((nextjob == null) && (pendingPollStationList.Count == 0) &&
               ((ListPollRtuJobPending.Count > 0) || (lastPollStationTime == DateTime.MinValue) || ((DateTime.UtcNow - lastPollStationTime).TotalMilliseconds > ScheduleTimeJobsList)) &&
               (activePollStationMap.Count < _LacbusPCMaxPendingRTUPollReq))
            {
                // Give priority to the jobs that must send RTU Poll requests
                if((ListPollRtuJobPending.Count > 0) && (ListPollRtuJobPending[0] != null))
                {
                    if(!activePollStationMap.ContainsKey(ListPollRtuJobPending[0].Station.Name))
                    {
                        nextjob = ListPollRtuJobPending[0];
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - PollRtu calling SendPollRTURequest for Job", DateTime.Now.ToString("HH:mm:ss.fff"));
#endif
                        if (SendPollRTURequest(ListPollRtuJobPending[0]) == false)
                        {
#if DEBUG
                            System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - PollRtu - Error {1} in SendPollRTURequest for Job", DateTime.Now.ToString("HH:mm:ss.fff"), LastErrorCode);
#endif
                            if (SynchroJob != null && SynchroJob == nextjob)
                            {
                                nextjob.ResetSynchro.WaitOne(Timeout);
                                SynchroJob = null;
                                nextjob.ResetSynchro.Reset();
                            }
                            ProcessError(nextjob, LastErrorCode);
                        }
                        else
                        {
#if DEBUG
                            System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - PollRtu - SendPollRTURequest for Job OK", DateTime.Now.ToString("HH:mm:ss.fff"));
#endif
                            LacbusPCStation lStation = (LacbusPCStation)ListPollRtuJobPending[0].Station;
                            lStation.lastPollRTURequestTime = DateTime.UtcNow;
                            lStation.pollRTURequestPending = true;
                            pendingPollStationList.Add(lStation);
                            ListJobPending.Add(ListPollRtuJobPending[0]);
                        }
                        ListPollRtuJobPending.RemoveAt(0);
                    }
                    else
                    {
                        LacbusPCCommJob lJob = ListPollRtuJobPending[0];
                        lJob.LacbusPCStatus = LacbusPCCommJobStatus.Idle;
                        lJob.IsPending = false;
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        OnJobExecuted(eJob);
                        ListPollRtuJobPending.RemoveAt(0);
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - Poll RTU request from job not sent: communication already in progress for the station {1}", DateTime.Now.ToString("HH:mm:ss.fff"), lJob.Station.Name);
#endif
                    }
                }

                // Automatic polling of the RTUs
                else
                {
                    // Make a copy of the list of stations with automatic polling enabled and order it by lastActivePollStateTime
                    List<LacbusPCStation> candidateStationList = null;
                    candidateStationList = (from station in pollStationList/*.AsParallel()*/
                                            where !activePollStationMap.ContainsKey(station.Name) && station.MustSendPollRequest(Timeout)
                                            orderby station.lastActivePollStateTime ascending
                                            select station).ToList();
                    if ((candidateStationList != null) && (candidateStationList.Count > 0))
                    {
                        int activePollStationCount = activePollStationMap.Count;
                        int candidateStationIndex = 0;
                        for (candidateStationIndex = 0;
                             ((activePollStationCount + pendingPollStationList.Count) < _LacbusPCMaxPendingRTUPollReq) && (candidateStationIndex < candidateStationList.Count);
                             candidateStationIndex++)
                        {
                            LacbusPCStation lStation = candidateStationList[candidateStationIndex];
#if DEBUG
                            System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - PollRtu - calling SendPollRTURequest for Station {1}", DateTime.Now.ToString("HH:mm:ss.fff"), lStation.Name);
#endif
                            // Send the RTU Poll request
                            if (SendPollRTURequest(lStation) == true)
                            {
                                lStation.lastPollRTURequestTime = DateTime.UtcNow;
                                lStation.pollRTURequestPending = true;
                                pendingPollStationList.Add(lStation);
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - PollRtu - SendPollRTURequest for Station {1} OK", DateTime.Now.ToString("HH:mm:ss.fff"), lStation.Name);
#endif
                            }
                            else
                            {
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - PollRtu - Error {1} in SendPollRTURequest for Station {2}", DateTime.Now.ToString("HH:mm:ss.fff"), LastErrorCode, lStation.Name);
#endif
                                lStation.ManagePollSendError();
                            }
                        }
                    }

                    // Update the last polling time
                    lastPollStationTime = DateTime.UtcNow;
                }
            }

            // Check if the timeout is elapsed for the pending RTU Poll requests
            else if (pendingPollStationList.Count > 0)
            {
                DateTime currUTCTime = DateTime.UtcNow;
                List<LacbusPCStation> timedoutPollStationList = null;
                timedoutPollStationList = (from station in pendingPollStationList.AsParallel()
                                           where (station != null) && (station.CheckPollTimeout(currUTCTime, Timeout) == false)
                                           select station).ToList();
                if (timedoutPollStationList.Count > 0)
                {
                    List<LacbusPCStation> stillPendingStationList = null;
                    stillPendingStationList = (from station in pendingPollStationList.AsParallel()
                                               where (station != null) && (station.CheckPollTimeout(currUTCTime, Timeout) == true)
                                               select station).ToList();
                    foreach (LacbusPCStation timedoutStation in timedoutPollStationList)
                    {
                        // Remove the station from the list of stations for which the poll has been activated on the FR1000
                        // The station will be removed also from the pending station list
                        if (activePollStationMap.ContainsKey(timedoutStation.Name))
                        {
                            activePollStationMap.Remove(timedoutStation.Name);
                            timedoutStation.activePollState = false;
                            timedoutStation.lastActivePollStateTime = currUTCTime;
#if DEBUG
                            System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - PollRtu - Station {1} removed from activePollStationMap (case 1)",
                                                              currUTCTime.ToString("HH:mm:ss.fff"), timedoutStation.Name);
#endif
                        }
                    }

                    // Remove from pendingPollStationList the stations for which the answer to the RTU Poll request has not been received
                    pendingPollStationList.Clear();
                    if (stillPendingStationList.Count > 0)
                    {
                        pendingPollStationList.AddRange(stillPendingStationList);
                    }
                }
            }

            DateTime currentUTCTime = DateTime.UtcNow;
            if ((activePollStationMap.Count > 0) &&
               ((lastActivePollStationCheckTime == DateTime.MinValue) || ((currentUTCTime - lastActivePollStationCheckTime).TotalMilliseconds > ScheduleTimeJobsList)))
            {
                Dictionary<string, LacbusPCStation> copyMap = new Dictionary<string, LacbusPCStation>();
                foreach (var item in activePollStationMap)
                {
                    LacbusPCStation station = item.Value;
                    if ((currentUTCTime - station.lastDataReceivedTime).TotalMilliseconds < MaxTimeWithoutData)
                    {
                        copyMap[station.Name] = station;
                    }
                    else
                    {
                        station.activePollState = false;
                        station.lastActivePollStateTime = currentUTCTime;
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - PollRtu - Station {1} removed from activePollStationMap (case 2)",
                                                           currentUTCTime.ToString("HH:mm:ss.fff"), station.Name);
#endif
                    }
                }
                if (activePollStationMap.Count > copyMap.Count)
                {
                    activePollStationMap.Clear();
                    foreach (var item in copyMap)
                    {
                        LacbusPCStation station = item.Value;
                        activePollStationMap[station.Name] = station;
                    }
                }
                lastActivePollStationCheckTime = currentUTCTime;
            }
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets the next pending job. </summary>
        ///
        /// <returns>   The next pending job. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////

        protected LacbusPCCommJob GetNextLacbusPCPendingJob()
        {
            lock (lockScheduleFlag)
            {
                var queue = GetNextPendingQueue();
                if (queue != null)
                {
                    while (!queue.IsEmpty)
                    {
                        CommJob j = null;
                        if (queue.TryDequeue(out j) == true)
                        {
                            LacbusPCCommJob lj = (LacbusPCCommJob)j;
                            if (lj != null)
                            {
                                if (lj.MustSendARequest())
                                {
                                    return (lj);
                                }
                            }

                        }
                    }
                }
            }
            return null;
        }

        public void AddRepliesFromRemote(int messageNumber, byte[] frame)
        {
            lock (lockRepliesFromRemoteListObject)
            {
                mapReplies[messageNumber] = frame;
            }
        }

        public void AddMessageFromRemote(byte[] frame)
        {
            lock (lockReceivedFramesFromRemoteListObject)
            {
                ReceivedFramesFromRemoteList.Add(frame);
            }
        }

        public void AddToMapInputJobs(UInt64 searchKey, LacbusPCCommJob job)
        {
            lock (lockMapInputJobsObject)
            {
                if (!mapInputJobs.ContainsKey(searchKey))
                {
                    List<LacbusPCCommJob> ListJobs = new List<LacbusPCCommJob>();
                    ListJobs.Add(job);
                    mapInputJobs[searchKey] = ListJobs;
                }
                else if (!mapInputJobs[searchKey].Contains(job))
                {
                    mapInputJobs[searchKey].Add(job);
                }
            }
        }

        public void GetJobListForNewData(UInt64 searchKey, ref List<LacbusPCCommJob> jobList)
        {
            lock (lockMapInputJobsObject)
            {
                if (mapInputJobs.ContainsKey(searchKey))
                {
                    jobList = mapInputJobs[searchKey];
                }
            }
        }

        private bool IsOKReply(byte[] reply)
        {
            bool okReply = false;
            if ((reply.GetLength(0) > 3) && (reply[0] == 0x3c) && (reply[1] == 0x6f) && (reply[2] == 0x6b))
            {
                okReply = true;
            }
            return (okReply);
        }

        private bool IsErrorReply(byte[] reply, ref int errorCode)
        {
            bool errorReply = false;
            if ((reply.GetLength(0) > 6) && (reply[0] == 0x3c) && (reply[1] == 0x65) && (reply[2] == 0x72) && (reply[3] == 0x72) && (reply[4] == 0x6f) && (reply[5] == 0x72))
            {
                errorReply = true;
                errorCode = 0;
                Encoding enc8 = Encoding.UTF8;
                string replyString = enc8.GetString(reply);
                int first = replyString.IndexOf("'");
                if (first > 0)
                {
                    first++;
                    int last = replyString.LastIndexOf("'");
                    if (last > first)
                    {
                        string errorValue = replyString.Substring(first, last - first);
                        if (Int32.TryParse(errorValue, out errorCode) == false)
                        {
                            errorCode = 0;
                        }
                    }
                }
            }
            return (errorReply);
        }

        public void SetConnectionBroken(bool connectionIsBroken = true)
        {
            lock (lockThreadObject)
            {
                isConnectionBroken = connectionIsBroken;
                if(isConnectionBroken == true)
                {
                    CommDriver.OnSystemEvent(null, Properties.Resources.ConnectionToFrontEndBroken, EventSeverity.Low);
                    lastConnectionError = -10;
                }
            }
        }

        void ManageConnectionRestored()
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("Automatic Rtu Poll - {0} - Channel {1} - ManageConnectionRestored has been called", DateTime.Now, Name);
#endif
            foreach (LacbusPCStation lStation in CommDriver.GetChannelStations(this))
            {
                lStation.ManageConnectionRestored();
            }
        }

        void ManageConnectionBroken(List<CommJob> jobList)
        {
            try
            {
                isConnectionBroken = false;
                // Clear lists of received messages
                lock (lockRepliesFromRemoteListObject)
                {
                    mapReplies.Clear();
                }
                lock (lockReceivedFramesFromRemoteListObject)
                {
                    ReceivedFramesFromRemoteList.Clear();
                }

                // Set to inactive current jobs
                foreach (var job in jobList)
                {
                    LacbusPCCommJob lJob = (LacbusPCCommJob)job;
                    lJob.LacbusPCStatus = LacbusPCCommJobStatus.Idle;
                    if (SynchroJob != null && SynchroJob == lJob)
                    {
                        lJob.ResetSynchro.WaitOne(Timeout);
                        SynchroJob = null;
                        lJob.ResetSynchro.Reset();
                    }
                }

                // Set the error state of the jobs
                foreach (LacbusPCStation lStation in CommDriver.GetChannelStations(this))
                {
                    lStation.ManageConnectionBroken();
                }
 
                // Any station waiting for a reply to the poll RTU request? --> empty the list
                if (pendingPollStationList.Count > 0)
                {
                    pendingPollStationList.Clear();
                }
                // Any station with poll RTU request activated? --> empty the list
                if (activePollStationMap.Count > 0)
                {
                    DateTime currentUTCTime = DateTime.UtcNow;
                    Parallel.ForEach(activePollStationMap, item =>
                    {
                        item.Value.lastActivePollStateTime = currentUTCTime;
                        item.Value.activePollState = false;
                    });
                    activePollStationMap.Clear();
                    System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - MannageConnectionBroken cleared activePollStationMap",
                                                        DateTime.Now.ToString("HH:mm:ss.fff"));
                }

                // Set internal flags and free communication objects
                lock (lockStream)
                {
                    if ((chVortexDevManager != null) && (vortexDevManagerInitialized == true))
                    {
                        unsafe
                        {
                            if (supervisorSendChannelCreated == true)
                            {
                                supervisorSendChannelCreated = false;
                                supervisorSendChannel = null;
                            }
                            if (identificationChannelCreated == true)
                            {
                                identificationChannelCreated = false;
                                identificationChannel = null;
                            }
                        }
                        if (vortexConnectionInitialized == true)
                        {
                            vortexConnectionInitialized = false;
                        }

                        chVortexDevManager.ResetVortexConnection();
                    }

                    if(gcHandleReplyFrameReceivedCallbackFunc.IsAllocated)
                    {
                        gcHandleReplyFrameReceivedCallbackFunc.Free();
                    }
                    if(gcHandleRemoteFrameReceivedCallbackFunc.IsAllocated)
                    {
                        gcHandleRemoteFrameReceivedCallbackFunc.Free();
                    }
                    if(gcHandleReplyOnCloseConnectionCallbackFunc.IsAllocated)
                    {
                        gcHandleReplyOnCloseConnectionCallbackFunc.Free();
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        private void ManageRepliesFromRemote(CommJob pendingJob)
        {
            // Any message received?
            Dictionary<int, byte[]> mapRepliesCopy = null;
            lock (lockRepliesFromRemoteListObject)
            {
                if (mapReplies.Count > 0)
                {
                    mapRepliesCopy = new Dictionary<int, byte[]>(mapReplies);
                    mapReplies.Clear();

                    // Update the last activity timeout on the request channel
                    lastActivityTimeOnSendChannel = DateTime.UtcNow;
                }
            }
            if (mapRepliesCopy == null)
            {
                if (supervisorSendChannelCreated == true)
                {
                    // Check if the request channel must be closed
                    double dtime = (DateTime.UtcNow - lastActivityTimeOnSendChannel).TotalMilliseconds;
                    if (dtime > sendChannelMaxIdleTime)
                    {
                        if((pendingJob == null) && (pendingPollStationList.Count == 0))
                        {
                            CloseActiveCommunicationChannel();
                        }
                        else
                        {
                            UnreferenceActiveCommunicationChannel();
                        }
                    }
                }
                return;
            }

            System.Diagnostics.Debug.WriteLine("ManageRepliesFromRemote received {0} messages", mapRepliesCopy.Count);

            // Check if a job is waiting for a reply
            if (pendingJob != null)
            {
                System.Diagnostics.Debug.WriteLine("ManageRepliesFromRemote there is a pending job");

                LacbusPCCommJob lJob = (LacbusPCCommJob)pendingJob;
                if (mapRepliesCopy.ContainsKey(lJob.requestMessageNumber))
                {
                    System.Diagnostics.Debug.WriteLine("ManageRepliesFromRemote message number {0} found in mapRepliesCopy", lJob.requestMessageNumber);

                    byte[] replyFrame = mapRepliesCopy[lJob.requestMessageNumber];
                    if ((replyFrame != null) && (replyFrame.GetLength(0) > 0))
                    {
                        // Request accepted
                        if (IsOKReply(replyFrame) == true)
                        {
                            System.Diagnostics.Debug.WriteLine("ManageRepliesFromRemote received OK reply for message {0}", lJob.requestMessageNumber);

                            // remove the message from the temporary dictionary
                            mapRepliesCopy.Remove(lJob.requestMessageNumber);

                            // Check if the request sent by the the job is of type "Poll RTU"
                            if(lJob.LacbusPCDatumType == DatumTypes.RTUPollRequest)
                            {
                                LacbusPCStation lStation = (LacbusPCStation)lJob.Station;
                                if(pendingPollStationList.Contains(lStation))
                                {
                                    pendingPollStationList.Remove(lStation);
                                    lStation.lastPollRTURequestTime = DateTime.UtcNow;
                                    lStation.pollRTUCommunicationDuration = lJob.LacbusPCCommunicationDuration;
                                    if(lStation.pollRTUCommunicationDuration < 1)
                                    {
                                        lStation.pollRTUCommunicationDuration = 1;
                                    }
                                    lStation.pollRTURequestPending = false;
                                }
                                // Add the station to the list of stations activated for polling
                                if (!activePollStationMap.ContainsKey(lStation.Name))
                                {
                                    activePollStationMap.Add(lStation.Name, lStation);
                                    lStation.activePollState = true;
                                    lStation.lastActivePollStateTime = DateTime.UtcNow;
                                    lStation.lastDataReceivedTime = DateTime.UtcNow;
                                    System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - ManageRepliesFromRemote (case job) added to activePollStationMap station {1}",
                                                                        DateTime.Now.ToString("HH:mm:ss.fff"), lStation.Name);
                                }
                            }

                            lJob.LacbusPCStatus = LacbusPCCommJobStatus.ReplyReceived;
                            RemovePendingJob(pendingJob);
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.Job = lJob;
                            eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                            OnJobExecuted(eJob);
                        }
                        else
                        {
                            // Error
                            int errorCode = 0;
                            if (IsErrorReply(replyFrame, ref errorCode) == true)
                            {
                                System.Diagnostics.Debug.WriteLine("ManageRepliesFromRemote received error reply ({0}) for message {1}", errorCode, lJob.requestMessageNumber);

                                // remove the message from the temporary dictionary
                                mapRepliesCopy.Remove(lJob.requestMessageNumber);

                                RemovePendingJob(pendingJob);
                                ExecutedJobArgs eJob = new ExecutedJobArgs();
                                bool repeatTheRequest = false;

                                // Check if the request sent by the the job is of type "Poll RTU"
                                if (lJob.LacbusPCDatumType == DatumTypes.RTUPollRequest)
                                {
                                    LacbusPCStation lStation = (LacbusPCStation)lJob.Station;
                                    if (pendingPollStationList.Contains(lStation))
                                    {
                                        pendingPollStationList.Remove(lStation);
                                        lStation.lastPollRTURequestTime = DateTime.UtcNow;
                                        lStation.pollRTURequestPending = false;
                                    }

                                    // Remove the station from the list of stations for which the poll has been activated on the FR1000
                                    if (activePollStationMap.ContainsKey(lStation.Name))
                                    {
                                        activePollStationMap.Remove(lStation.Name);
                                        lStation.activePollState = false;
                                        lStation.lastActivePollStateTime = DateTime.UtcNow;
                                        System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - ManageRepliesFromRemote (case job) removed from activePollStationMap station {1}",
                                                                            DateTime.Now.ToString("HH:mm:ss.fff"), lStation.Name);
                                    }
                                }

                                switch (errorCode)
                                {
                                    case (int)LacbusPCErrorCodes.ErrorCodeUnableToProcessTheCommand:
                                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)errorCode;
                                        repeatTheRequest = true;
                                        break;
                                    case (int)LacbusPCErrorCodes.ErrorCodeSyntaxError:
                                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)errorCode;
                                        break;
                                    case (int)LacbusPCErrorCodes.ErrorCodeForbiddenCommand:
                                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)errorCode;
                                        break;
                                    case (int)LacbusPCErrorCodes.ErrorCodeBadRtuNumber:
                                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)errorCode;
                                        break;
                                    case (int)LacbusPCErrorCodes.ErrorCodeAlreadyAuthenticatedSCADA:
                                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)errorCode;
                                        break;
                                    case (int)LacbusPCErrorCodes.ErrorCodeDisabledScadaNumber:
                                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)errorCode;
                                        break;
                                    case (int)LacbusPCErrorCodes.ErrorCodeAuthenticationNeeded:
                                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)errorCode;
                                        break;
                                    case (int)LacbusPCErrorCodes.ErrorCodeUnsupportedProtocolVersion:
                                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)errorCode;
                                        break;
                                    default:
                                        eJob.ErrorCode = (DriverCodeBase.Enumerators.DriverErrorCodes)LacbusPCErrorCodes.ErrorCodeUnexpectedErrorCode;
                                        break;
                                }

                                // Do not repeat the request
                                if(repeatTheRequest == false)
                                {
                                    lock (pendingJob.retLockList())
                                    {
                                        pendingJob.TagsListOnWriting.Clear();
                                    }
                                }

                                eJob.Job = lJob;
                                OnJobExecuted(eJob);
                            }
                        }
                    }
                }
            }

            // Check if a station is waiting for a reply
            if (pendingPollStationList.Count > 0)
            {
                List<LacbusPCStation> stillPendingStationList = new List<LacbusPCStation>();
                foreach (LacbusPCStation pendingStation in pendingPollStationList)
                {
                    if (mapRepliesCopy.ContainsKey(pendingStation.pollRTURequestMessageNumber))
                    {
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - ManageRepliesFromRemote (pending RTU Poll request) message number {1} found in mapRepliesCopy", DateTime.Now.ToString("HH:mm:ss.fff"), pendingStation.pollRTURequestMessageNumber);
#endif

                        byte[] replyFrame = mapRepliesCopy[pendingStation.pollRTURequestMessageNumber];
                        if ((replyFrame != null) && (replyFrame.GetLength(0) > 0))
                        {
                            // Request accepted
                            if (IsOKReply(replyFrame) == true)
                            {
#if DEBUG
                                System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - ManageRepliesFromRemote (pending RTU Poll request) received OK reply for message {1}", DateTime.Now.ToString("HH:mm:ss.fff"), pendingStation.pollRTURequestMessageNumber);
#endif

                                // remove the message from the temporary dictionary
                                mapRepliesCopy.Remove(pendingStation.pollRTURequestMessageNumber);

                                // Notify the received reply to the station
                                pendingStation.ManagePollReply(0);

                                // Add the station to the list of stations activated for polling
                                if (!activePollStationMap.ContainsKey(pendingStation.Name))
                                {
                                    activePollStationMap.Add(pendingStation.Name, pendingStation);
                                    pendingStation.activePollState = true;
                                    pendingStation.lastActivePollStateTime = DateTime.UtcNow;
                                    pendingStation.lastDataReceivedTime = DateTime.UtcNow;
                                    System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - ManageRepliesFromRemote added to activePollStationMap station {1}",
                                                                        DateTime.Now.ToString("HH:mm:ss.fff"), pendingStation.Name);
                                }
                            }
                            else
                            {
                                // Error
                                int errorCode = 0;
                                if (IsErrorReply(replyFrame, ref errorCode) == true)
                                {
#if DEBUG
                                    System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - ManageRepliesFromRemote (pending RTU Poll request) received error reply ({1}) for message {2}", DateTime.Now.ToString("HH:mm:ss.fff"), errorCode, pendingStation.pollRTURequestMessageNumber);
#endif

                                    // remove the message from the temporary dictionary
                                    mapRepliesCopy.Remove(pendingStation.pollRTURequestMessageNumber);

                                    // Notify the received reply to the station
                                    pendingStation.ManagePollReply(errorCode);

                                    // Remove the station from the list of stations for which the poll has been activated on the FR1000
                                    if (activePollStationMap.ContainsKey(pendingStation.Name))
                                    {
                                        activePollStationMap.Remove(pendingStation.Name);
                                        pendingStation.activePollState = false;
                                        pendingStation.lastActivePollStateTime = DateTime.UtcNow;
                                        System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - ManageRepliesFromRemote removed from activePollStationMap station {1}",
                                                                            DateTime.Now.ToString("HH:mm:ss.fff"), pendingStation.Name);
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        // Do not remove the station from the pending station list
                        stillPendingStationList.Add(pendingStation);
                    }
                }
                if(pendingPollStationList.Count > stillPendingStationList.Count)
                {
                    pendingPollStationList.Clear();
                    if(stillPendingStationList.Count > 0)
                    {
                        pendingPollStationList.AddRange(stillPendingStationList);
                    }
                }
            }
        }

        private void ManageMessagesFromRemote()
        {
            // Any message received?
            List<byte[]> receivedMessages = new List<byte[]>();
            lock (lockReceivedFramesFromRemoteListObject)
            {
                if (ReceivedFramesFromRemoteList.Count > 0)
                {
                    // Make a local copy of the list of messages and clear it 
                    receivedMessages.AddRange(ReceivedFramesFromRemoteList);
                    // Clear the list of received messages
                    ReceivedFramesFromRemoteList.Clear();
                }
            }
            if (receivedMessages.Count > 0)
            {
                foreach (var msg in receivedMessages)
                {
                    // Parse the message
                    byte[] receivedMessage = msg;
                    int messageLength = receivedMessage.GetLength(0);
                    if (messageLength > 0)
                    {
                        System.Diagnostics.Debug.WriteLine("ManageMessagesFromRemote: parsing message of length {0} first byte = [{1}]", messageLength, receivedMessage[0]);
                    }
                    else
                    {
                        continue;
                    }

                    // Parse the message header
                    int errorCode = 0;
                    LacbusPCMessageHeader messageHeader = new LacbusPCMessageHeader();
                    if (!LacbusPcProtocol.ParseLacbusPCMessageHeader(receivedMessage, ref messageHeader, out errorCode))
                    {
                        // Incorrect message
                        System.Diagnostics.Debug.WriteLine("ManageMessagesFromRemote: error parsing header message");
                        switch(errorCode)
                        {
                            case (int)LacbusPCErrorCodes.ErrorCodeReceivedIncompleteMessage:
                                CommDriver.OnSystemEvent(null, Properties.Resources.ErrorIncompleteMessage, EventSeverity.Low);
                                break;
                            case (int)LacbusPCErrorCodes.ErrorCodeReceivedIncorrectMessageType:
                                CommDriver.OnSystemEvent(null, Properties.Resources.ErrorIncorrectMessageType, EventSeverity.Low);
                                break;
                            case (int)LacbusPCErrorCodes.ErrorCodeReceivedIncorrectUnderlyingProtocol:
                                CommDriver.OnSystemEvent(null, Properties.Resources.ErrorIncorrectUnderlyingProtocol, EventSeverity.Low);
                                break;
                            case (int)LacbusPCErrorCodes.ErrorCodeReceivedIncorrectProtocolVersion:
                                CommDriver.OnSystemEvent(null, Properties.Resources.ErrorIncorrectProtocolVersion, EventSeverity.Low);
                                break;
                            case (int)LacbusPCErrorCodes.ErrorCodeReceivedIncorrectLacbusPCHeaderLength:
                                CommDriver.OnSystemEvent(null, Properties.Resources.ErrorIncorrectLacbusPcHeaderLength, EventSeverity.Low);
                                break;
                            default:
                                CommDriver.OnSystemEvent(null, Properties.Resources.ErrorIncorrectLacbusPcHeader, EventSeverity.Low);
                                break;
                        }
                        continue;
                    }
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("ManageMessagesFromRemote: data of the header message ProtVer {0} UnderProt {1} MessType {2} RtuNum {3} PhoneNum {4} DataLen {5}",
                            messageHeader.headerProtocolVersion, messageHeader.headerUnderlyingProtocol, messageHeader.headerMessageType, messageHeader.headerRtuNumber,
                            messageHeader.headerPhoneNumber, messageHeader.headerDataLength);
                    }

                    // Check if there is any data contained in the message
                    if (messageHeader.headerDataLength == 0)
                    {
                        System.Diagnostics.Debug.WriteLine("ManageMessagesFromRemote: received message with no data");
                        continue;
                    }

                    // Check if the data part of the message has been correctly received
                    if (messageLength < (LacbusPcProtocol.LacbusPCMessageHeaderLength + messageHeader.headerDataLength))
                    {
                        System.Diagnostics.Debug.WriteLine("ManageMessagesFromRemote: received message of length {0} with wrong data length {1}", messageLength, messageHeader.headerDataLength);
                        continue;
                    }

                    // Check if the message is for a defined station
                    if (!mapStations.ContainsKey(messageHeader.headerRtuNumber))
                    {
                        if(messageHeader.headerMessageType != 0x49) // 'I' = FR Internal statuses
                        {
                            string warningMessage = String.Format(Properties.Resources.WarningReceivedMessageFromNonManagedStation, messageHeader.headerRtuNumber);
                            CommDriver.OnSystemEvent(null, warningMessage, EventSeverity.Low);
                            System.Diagnostics.Debug.WriteLine("ManageMessagesFromRemote: message for undefined station {0}", messageHeader.headerRtuNumber);
                            continue;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("ManageMessagesFromRemote: parsing I message (case no station defined)");
                            ParseIMessageData(receivedMessage, messageHeader);
                            continue;
                        }
                    }

                    // Check if the underlying protocol of the message matches the protocol set for the station
                    LacbusPCStation lStation = mapStations[messageHeader.headerRtuNumber];
                    if(lStation == null)
                    {
                        string warningMessage = String.Format(Properties.Resources.WarningReceivedMessageFromNonManagedStation, messageHeader.headerRtuNumber);
                        CommDriver.OnSystemEvent(null, warningMessage, EventSeverity.Low);
                        System.Diagnostics.Debug.WriteLine("ManageMessagesFromRemote: message for undefined station {0}", messageHeader.headerRtuNumber);
                        continue;
                    }
                    if(lStation.LacbusPCProtocolType != messageHeader.headerUnderlyingProtocol)
                    {
                        string warningMessage = String.Format(Properties.Resources.WarningReceivedMessageWithUnexpectedUnderlyingProtocol, messageHeader.headerRtuNumber, messageHeader.headerUnderlyingProtocol, lStation.LacbusPCProtocolType);
                        CommDriver.OnSystemEvent(null, warningMessage, EventSeverity.Low);
                        System.Diagnostics.Debug.WriteLine("ManageMessagesFromRemote: received message for station {0} with unexpected underlying protocol", messageHeader.headerRtuNumber);
                        continue;
                    }

                    // Parse the message data
                    switch (messageHeader.headerMessageType)
                    {
                        case 0x49: // 'I' = FR Internal statuses
                            System.Diagnostics.Debug.WriteLine("ManageMessagesFromRemote: parsing I message");
                            ParseIMessageData(receivedMessage, messageHeader);
                            break;

                        case 0x45: // 'E' = Spontaneous transmission from an RTU
                            lStation.lastDataReceivedTime = DateTime.UtcNow;
                            // Data received: remove the station from the list of stations for which the poll has been activated on the FR1000?
                            if (activePollStationMap.ContainsKey(lStation.Name) && (lStation.LacbusPCCommunicationDuration == 0))
                            {
                                activePollStationMap.Remove(lStation.Name);
                                lStation.activePollState = false;
                                lStation.lastActivePollStateTime = DateTime.UtcNow;
                                System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - ManageMessagesFromRemote removed from activePollStationMap station {1} case message E",
                                                                    DateTime.Now.ToString("HH:mm:ss.fff"), lStation.Name);
                            }

                            System.Diagnostics.Debug.WriteLine("ManageMessagesFromRemote: parsing E message");
                            ParseEMessageData(receivedMessage, messageHeader);
                            break;

                        case 0x55: // 'U' = Data Unit from RTU
                            lStation.lastDataReceivedTime = DateTime.UtcNow;
                            // Data received: remove the station from the list of stations for which the poll has been activated on the FR1000?
                            if (activePollStationMap.ContainsKey(lStation.Name))
                            {
                                lStation.lastPollRTURequestTime = DateTime.UtcNow;
                                if(lStation.LacbusPCCommunicationDuration == 0)
                                {
                                    activePollStationMap.Remove(lStation.Name);
                                    lStation.activePollState = false;
                                    lStation.lastActivePollStateTime = DateTime.UtcNow;
                                    System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - ManageMessagesFromRemote removed from activePollStationMap station {1} case message U",
                                                                        DateTime.Now.ToString("HH:mm:ss.fff"), lStation.Name);
                                }
                            }

                            System.Diagnostics.Debug.WriteLine("ManageMessagesFromRemote: parsing U message");
                            ParseUMessageData(receivedMessage, messageHeader);
                            break;

                        case 0x53: // 'S' = SMS from RTU
                            lStation.lastDataReceivedTime = DateTime.UtcNow;
                            // Data received: remove the station from the list of stations for which the poll has been activated on the FR1000?
                            if (activePollStationMap.ContainsKey(lStation.Name))
                            {
                                if (lStation.LacbusPCCommunicationDuration == 0) { 
                                    activePollStationMap.Remove(lStation.Name);
                                    lStation.activePollState = false;
                                    lStation.lastActivePollStateTime = DateTime.UtcNow;
                                    System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - ManageMessagesFromRemote removed from activePollStationMap station {1} case message S",
                                                                        DateTime.Now.ToString("HH:mm:ss.fff"), lStation.Name);
                                }
                            }

                            System.Diagnostics.Debug.WriteLine("ManageMessagesFromRemote: parsing S message");
                            ParseSMessageData(receivedMessage, messageHeader);
                            break;
                    }
                }
            }
        }

        private void ParseUMessageData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader)
        {
            // For the moment, this message type is supported for protocol LACBUS RTU && SOFBUS-PL
            if (messageHeader.headerUnderlyingProtocol == LacbusPcUnderlyingProtocols.LacbusRTU)
            {
                System.Diagnostics.Debug.WriteLine("ParseUMessageData: parsing U message, Protocol LACBUS RTU");
                ParseLacbusRtuUMessageData(receivedMessage, messageHeader);
            }
            else if (messageHeader.headerUnderlyingProtocol == LacbusPcUnderlyingProtocols.SofbusPL)
            {
                System.Diagnostics.Debug.WriteLine("ParseUMessageData: parsing U message, Protocol SOFBUS PL");
                ParseSofbusPlUMessageData(receivedMessage, messageHeader);
            }
        }

        private void ParseSMessageData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader)
        {
            if (messageHeader.headerUnderlyingProtocol == LacbusPcUnderlyingProtocols.LacbusSofbusSMS)
            {
                System.Diagnostics.Debug.WriteLine("ParseSMessageData: parsing S message, Protocol LACBUS/SOFBUS-SMS");
                ParseLacbusSofbusSmsSMessageData(receivedMessage, messageHeader);
            }
        }

        private void ParseSofbusPlUMessageData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader)
        {
            // Parse the header of the Data Unit
            int messageLength = receivedMessage.GetLength(0);
            int totalParsedBytes = LacbusPcProtocol.LacbusPCMessageHeaderLength;
            int dataParsedBytes = 0;
            LacbusRtuDataUnitHeader rtuHeader = new LacbusRtuDataUnitHeader();
            int parsedBytes = LacbusPcProtocol.ParseSofbusPlDUHeader(receivedMessage, totalParsedBytes, ref rtuHeader);
            if (parsedBytes == 0)
            {
                System.Diagnostics.Debug.WriteLine("ParseSofbusPlUMessageData parsing error 1");
                return;
            }
            totalParsedBytes += parsedBytes;
            dataParsedBytes += parsedBytes;

            System.Diagnostics.Debug.WriteLine("ParseSofbusPlUMessageData parsing DU {0}", rtuHeader.DUType);

            // Parse the data part of the Data Unit
            if ((totalParsedBytes < messageLength) && (dataParsedBytes < messageHeader.headerDataLength) && (dataParsedBytes < rtuHeader.DUTotalLength))
            {
                SofbusPlParseDuData(receivedMessage, messageHeader, rtuHeader, totalParsedBytes, dataParsedBytes);
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("ParseSofbusPlUMessageData parsing error 2");
            }
        }

        private void SofbusPlParseDuData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, int alreadyParsedBytes, int alreadyParsedDataBytes)
        {
            int messageLength = receivedMessage.GetLength(0);
            int totalParsedBytes = alreadyParsedBytes;
            int dataParsedBytes = alreadyParsedDataBytes;
            DateTime hTime = DateTime.UtcNow;

            // Parse the data blocks
            while ((totalParsedBytes < messageLength) &&
                  (dataParsedBytes < messageHeader.headerDataLength) &&
                  (dataParsedBytes < rtuHeader.DUTotalLength))
            {
                LacbusPCDataBlockHeader blockHeader = new LacbusPCDataBlockHeader();
                int parsedBytes = LacbusPcProtocol.ParseLacbusPCDataBlockHeader(receivedMessage, totalParsedBytes, ref blockHeader);
                // Invalid block?
                if (parsedBytes == 0)
                {
                    System.Diagnostics.Debug.WriteLine("SofbusPlParseDuData error invalid block header");
                    break;
                }
                // Invalid block length?
                if (blockHeader.blockBodyLength == 0)
                {
                    System.Diagnostics.Debug.WriteLine("SofbusPlParseDuData error body length = 0");
                    break;
                }

                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuData: parsing block {0}, body length = {1}", blockHeader.blockType, blockHeader.blockBodyLength);

                if (rtuHeader.DUType == 0x49) // 'I' DU 
                {
                    System.Diagnostics.Debug.WriteLine("SofbusPlParseDuData: parsing DU I");
                    // Only 'L', 'M', 'S' blocks are parsed
                    if ((blockHeader.blockType != 0x4c) && (blockHeader.blockType != 0x4d) && (blockHeader.blockType != 0x53))
                    {
                        System.Diagnostics.Debug.WriteLine("SofbusPlParseDuData: unsupported block {0} for DU I --> Skip it", blockHeader.blockType);
                        totalParsedBytes += parsedBytes + blockHeader.blockBodyLength;
                        dataParsedBytes += parsedBytes + blockHeader.blockBodyLength;
                        continue;
                    }

                    totalParsedBytes += parsedBytes;
                    dataParsedBytes += parsedBytes;

                    switch (blockHeader.blockType)
                    {
                        case 0x4c: // 'L' block
                            parsedBytes = SofbusPlParseDuIBlockL(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes);
                            break;

                        case 0x4d: // 'M' block
                            parsedBytes = SofbusPlParseDuIBlockM(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes);
                            break;

                        case 0x53: // 'S' block
                            parsedBytes = SofbusPlParseDuIBlockS(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes);
                            break;
                    }

                    if (parsedBytes == 0)
                    {
                        return;
                    }
                    totalParsedBytes += parsedBytes;
                    dataParsedBytes += parsedBytes;
                }
                else if (rtuHeader.DUType == 0x48) // 'H' DU 
                {
                    System.Diagnostics.Debug.WriteLine("SofbusPlParseDuData: parsing DU H");
                    // Only 'A', 'B', 'C' blocks are parsed
                    if ((blockHeader.blockType != 0x41) && (blockHeader.blockType != 0x42) && (blockHeader.blockType != 0x43))
                    {
                        System.Diagnostics.Debug.WriteLine("SofbusPlParseDuData: unsupported block {0} for DU H --> Skip it", blockHeader.blockType);
                        totalParsedBytes += parsedBytes + blockHeader.blockBodyLength;
                        dataParsedBytes += parsedBytes + blockHeader.blockBodyLength;
                        continue;
                    }

                    totalParsedBytes += parsedBytes;
                    dataParsedBytes += parsedBytes;

                    switch (blockHeader.blockType)
                    {
                        case 0x41: // 'A' block
                            parsedBytes = SofbusPlParseDuHBlockA(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes);
                            break;
                        case 0x42: // 'B' block
                            parsedBytes = SofbusPlParseDuHBlockB(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes);
                            break;
                        case 0x43: // 'C' block
                            parsedBytes = SofbusPlParseDuHBlockC(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes);
                            break;
                    }

                    if (parsedBytes == 0)
                    {
                        return;
                    }
                    totalParsedBytes += parsedBytes;
                    dataParsedBytes += parsedBytes;
                }
                else if (rtuHeader.DUType == 0x42) // 'B' DU
                {
                    System.Diagnostics.Debug.WriteLine("SofbusPlParseDuData: parsing DU B");
                    // Supported blocks are: 'Z', 'A', 'B', 'C', 'D', 'E', 'F', 'G', 'I', 'J'
                    switch (blockHeader.blockType)
                    {
                        case 0x5a: // 'Z' block
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            parsedBytes = SofbusPlParseDuBBlockZ(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes, ref hTime);
                            if (parsedBytes == 0)
                            {
                                return;
                            }
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            break;

                        case 0x41: // 'A' block
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            parsedBytes = SofbusPlParseDuBBlockA(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes, hTime);
                            if (parsedBytes == 0)
                            {
                                return;
                            }
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            break;

                        case 0x42: // 'B' block
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            parsedBytes = SofbusPlParseDuBBlockB(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes, hTime);
                            if (parsedBytes == 0)
                            {
                                return;
                            }
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            break;

                        case 0x43: // 'C' block
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            parsedBytes = SofbusPlParseDuBBlockC(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes, hTime);
                            if (parsedBytes == 0)
                            {
                                return;
                            }
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            break;

                        case 0x44: // 'D' block
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            parsedBytes = SofbusPlParseDuBBlockD(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes, hTime);
                            if (parsedBytes == 0)
                            {
                                return;
                            }
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            break;

                        case 0x45: // 'E' block
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            parsedBytes = SofbusPlParseDuBBlockE(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes, hTime);
                            if (parsedBytes == 0)
                            {
                                return;
                            }
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            break;

                        case 0x46: // 'F' block
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            parsedBytes = SofbusPlParseDuBBlockF(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes, hTime);
                            if (parsedBytes == 0)
                            {
                                return;
                            }
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            break;

                        case 0x47: // 'G' block
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            parsedBytes = SofbusPlParseDuBBlockG(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes, hTime);
                            if (parsedBytes == 0)
                            {
                                return;
                            }
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            break;

                        case 0x49: // 'I' block
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            parsedBytes = SofbusPlParseDuBBlockI(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes, hTime);
                            if (parsedBytes == 0)
                            {
                                return;
                            }
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            break;

                        case 0x4a: // 'J' block
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            parsedBytes = SofbusPlParseDuBBlockJ(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes, hTime);
                            if (parsedBytes == 0)
                            {
                                return;
                            }
                            totalParsedBytes += parsedBytes;
                            dataParsedBytes += parsedBytes;
                            break;

                        // Unsupported block --> Skip it
                        default:
                            System.Diagnostics.Debug.WriteLine("SofbusPlParseDuData: unsupported block {0} for DU B --> Skip it", blockHeader.blockType);
                            totalParsedBytes += parsedBytes + blockHeader.blockBodyLength;
                            dataParsedBytes += parsedBytes + blockHeader.blockBodyLength;
                            break;
                    }
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("SofbusPlParseDuData: unsupported DU {0}", rtuHeader.DUType);
                    return;
                }
            }
        }

        private DateTime ConvertTimeToUtc(DateTime timeToBeConverted)
        {
            DateTime convertedTime;
            try
            {
                if(UseUtcTimeForSourceTimestamp())
                {
                    convertedTime = TimeZoneInfo.ConvertTimeToUtc(timeToBeConverted, channelTimeZoneInfo);
                }
                else
                {
                    convertedTime = timeToBeConverted;
                }
            }
            catch (Exception e)
            {
                convertedTime = timeToBeConverted;
            }
            return (convertedTime);
        }

        private DateTime ConvertTimeFromUtc(DateTime timeToBeConverted)
        {
            DateTime convertedTime;
            try
            {
                convertedTime = TimeZoneInfo.ConvertTimeFromUtc(timeToBeConverted, channelTimeZoneInfo);
            }
            catch (Exception e)
            {
                convertedTime = timeToBeConverted;
            }
            return (convertedTime);
        }

        private int SofbusPlParseDuBBlockZ(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes, ref DateTime hTime)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);

            if(totalMessageLength < (alreadyParsedBytes + 6))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockZ: reached the end of the block");
                return (0);
            }

            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength) ||
                (blockHeader.blockBodyLength < 7)))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockZ - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                   totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            // Parse the date and time of the report
            int day = receivedMessage[alreadyParsedBytes];
            int month = receivedMessage[alreadyParsedBytes + 1];
            int year = receivedMessage[alreadyParsedBytes + 2];
            year += 2000;
            int hour = receivedMessage[alreadyParsedBytes + 3];
            int minute = receivedMessage[alreadyParsedBytes + 4];
            int second = receivedMessage[alreadyParsedBytes + 5];
            DateTime receivedTime = new DateTime(year, month, day, hour, minute, second);
            hTime = ConvertTimeToUtc(receivedTime);

            System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockZ: parsed time = {0} returned time = {1}",
                                               receivedTime, hTime);

            return (blockHeader.blockBodyLength);
        }

        private int SofbusPlParseDuBBlockD(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes, DateTime hTime)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockD - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                   totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockD: parsing {0}", DatumCategories.ReportCountIndex);

            int parsedBytes = 0;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfReportCounts(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, hTime, (byte)DatumCategories.ReportCountIndex);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseDuBBlockE(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes, DateTime hTime)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockE - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                   totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockE: parsing {0}", DatumCategories.ReportCountTimeBand1);

            int parsedBytes = 0;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfReportCounts(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, hTime, (byte)DatumCategories.ReportCountTimeBand1);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseDuBBlockF(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes, DateTime hTime)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockF - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                   totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockF: parsing {0}", DatumCategories.ReportCountTimeBand2);

            int parsedBytes = 0;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfReportCounts(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, hTime, (byte)DatumCategories.ReportCountTimeBand2);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseDuBBlockG(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes, DateTime hTime)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockG - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                   totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockG: parsing {0}", DatumCategories.ReportCountTimeBand3);

            int parsedBytes = 0;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfReportCounts(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, hTime, (byte)DatumCategories.ReportCountTimeBand3);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSequenceOfReportCounts(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, DateTime hTime, byte dataCategory)
        {
            // Check the message length
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength < (alreadyParsedBytes + 6))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfReportCounts: reached the end of the block");
                return (0);
            }

            // Get the datum number
            ushort datumNumber = receivedMessage[alreadyParsedBytes];
            datumNumber <<= 8;
            datumNumber += receivedMessage[alreadyParsedBytes + 1];

            // Get the value
            byte[] auxBuffer = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                auxBuffer[3 - i] = receivedMessage[alreadyParsedBytes + i + 2];
            }
            uint ciValue = BitConverter.ToUInt32(auxBuffer, 0);

            int parsedBytes = 6;

            System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfReportCounts: CI n. {0} value {1} category {2} timestamp {3}", datumNumber, ciValue, dataCategory, hTime);

            // Pass the received data to jobs
            DatumTypes newDataType = DatumTypes.CountInput;
            UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, datumNumber, (byte)newDataType, dataCategory);
            List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
            GetJobListForNewData(jobSearchKey, ref listJobs);
            if (listJobs.Count > 0)
            {
                // Process the received data
                foreach (var jobVal in listJobs)
                {
                    LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                    if (lJob == null)
                    {
                        continue;
                    }
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = lJob;
                    eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                    bool jobValueSet = false;
                    uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;

                    // If requested, apply the data conversion
                    if (lJob.ConversionCanBeApplied())
                    {
                        eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, lJob.ApplyDataConversion(ciValue), ref jobValueSet);
                    }
                    else
                    {
                        eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, ciValue, ref jobValueSet);
                    }

                    eJob.Timestamp = hTime;
                    if (jobValueSet == true)
                    {
                        OnJobExecuted(eJob);
                    }
                }
            }

            return (parsedBytes);
        }

        private int SofbusPlParseDuBBlockI(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes, DateTime hTime)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockI - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                   totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockI: parsing {0}", DatumCategories.ReportDIEventCount);

            int parsedBytes = 0;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfReportSignalling(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, hTime, (byte)DatumCategories.ReportDIEventCount);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseDuBBlockJ(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes, DateTime hTime)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockJ - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                  totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockJ: parsing {0}", DatumCategories.ReportDIActiveStateTimeCount);

            int parsedBytes = 0;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfReportSignalling(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, hTime, (byte)DatumCategories.ReportDIActiveStateTimeCount);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSequenceOfReportSignalling(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, DateTime hTime, byte dataCategory)
        {
            // Check the message length
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength < (alreadyParsedBytes + 6))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfReportSignalling: reached the end of the block");
                return (0);
            }

            // Get the datum number
            ushort datumNumber = receivedMessage[alreadyParsedBytes];
            datumNumber <<= 8;
            datumNumber += receivedMessage[alreadyParsedBytes + 1];

            // Get the value
            byte[] auxBuffer = new byte[4];
            for (int i = 0; i < 4; i++)
            {
                auxBuffer[3 - i] = receivedMessage[alreadyParsedBytes + i + 2];
            }
            uint diValue = BitConverter.ToUInt32(auxBuffer, 0);

            int parsedBytes = 6;

            System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfReportSignalling: DI n. {0} value {1} category {2} timestamp {3}", datumNumber, diValue, dataCategory, hTime);

            // Pass the received data to jobs
            DatumTypes newDataType = DatumTypes.DigitalInput;
            UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, datumNumber, (byte)newDataType, dataCategory);
            List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
            GetJobListForNewData(jobSearchKey, ref listJobs);
            if (listJobs.Count > 0)
            {
                // Process the received data
                foreach (var jobVal in listJobs)
                {
                    LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                    if (lJob == null)
                    {
                        continue;
                    }
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = lJob;
                    eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                    bool jobValueSet = false;
                    uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;

                    // If requested, apply the data conversion
                    if (lJob.ConversionCanBeApplied())
                    {
                        eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, lJob.ApplyDataConversion(diValue), ref jobValueSet);
                    }
                    else
                    {
                        eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, diValue, ref jobValueSet);
                    }

                    eJob.Timestamp = hTime;
                    if (jobValueSet == true)
                    {
                        OnJobExecuted(eJob);
                    }
                }
            }

            return (parsedBytes);
        }

        private int SofbusPlParseDuBBlockA(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes, DateTime hTime)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockA - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                   totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockA: parsing {0}", DatumCategories.ReportAverageMeasurement);

            int parsedBytes = 0;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfReportMeasurements(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, hTime, (byte)DatumCategories.ReportAverageMeasurement);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }


            return (parsedBytes);
        }

        private int SofbusPlParseDuBBlockB(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes, DateTime hTime)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockB - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                   totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockB: parsing {0}", DatumCategories.ReportMinimumMeasurement);

            int parsedBytes = 0;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfReportMeasurements(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, hTime, (byte)DatumCategories.ReportMinimumMeasurement);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }


            return (parsedBytes);
        }

        private int SofbusPlParseDuBBlockC(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes, DateTime hTime)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockC - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                   totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            System.Diagnostics.Debug.WriteLine("SofbusPlParseDuBBlockB: parsing {0}", DatumCategories.ReportMaximumMeasurement);

            int parsedBytes = 0;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfReportMeasurements(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, hTime, (byte)DatumCategories.ReportMaximumMeasurement);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }


            return (parsedBytes);
        }

        private int SofbusPlParseSequenceOfReportMeasurements(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, DateTime hTime, byte dataCategory)
        {
            // Check the message length
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength < (alreadyParsedBytes + 4))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfReportMeasurements: reached the end of the block");
                return (0);
            }

            // Get the datum number
            ushort datumNumber = receivedMessage[alreadyParsedBytes];
            datumNumber <<= 8;
            datumNumber += receivedMessage[alreadyParsedBytes + 1];

            // Get the value
            ushort datumValue = receivedMessage[alreadyParsedBytes + 2];
            datumValue <<= 8;
            datumValue += receivedMessage[alreadyParsedBytes + 3];

            System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfReportMeasurements: AI n. {0} value {1} category {2} timestamp {3}",
                                               datumNumber, datumValue, dataCategory, hTime);

            int parsedBytes = 4;

            // Pass the received data to jobs
            DatumTypes newDataType = DatumTypes.AnalogInput;
            UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, datumNumber, (byte)newDataType, dataCategory);
            List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
            GetJobListForNewData(jobSearchKey, ref listJobs);
            if (listJobs.Count > 0)
            {
                // Process the received data
                foreach (var jobVal in listJobs)
                {
                    LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                    if (lJob == null)
                    {
                        continue;
                    }
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = lJob;
                    eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                    bool jobValueSet = false;
                    uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;

                    // If requested, apply the data conversion
                    if (lJob.ConversionCanBeApplied())
                    {
                        eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, lJob.ApplyDataConversion(datumValue), ref jobValueSet);
                    }
                    else
                    {
                        eJob.Values = LacbusPcProtocol.ConvertUint16ValueToByteArray((BuiltInType)tagDataType, datumValue, ref jobValueSet);
                    }

                    eJob.Timestamp = hTime;
                    if (jobValueSet == true)
                    {
                        OnJobExecuted(eJob);
                    }
                }
            }

            return (parsedBytes);
        }

        private int SofbusPlParseDuHBlockB(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength < (alreadyParsedBytes + 6))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuHBlockB - Reached the end of the block");
                return (0);
            }
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)) ||
                (blockHeader.blockBodyLength < 6))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuHBlockB - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                   totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            // Parse the date and time of the historical data
            // Parse the date and time
            int day = receivedMessage[alreadyParsedBytes];
            int month = receivedMessage[alreadyParsedBytes + 1];
            int year = receivedMessage[alreadyParsedBytes + 2];
            year += 2000;
            int hour = receivedMessage[alreadyParsedBytes + 3];
            int minute = receivedMessage[alreadyParsedBytes + 4];
            int second = receivedMessage[alreadyParsedBytes + 5];
            DateTime receivedTime = new DateTime(year, month, day, hour, minute, second);
            receivedTime = ConvertTimeToUtc(receivedTime);

            int parsedBytes = 6;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfAlarmLogData(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, receivedTime);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSequenceOfAlarmLogData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, DateTime hTime)
        {
            // Check the message length
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength < (alreadyParsedBytes + 4))
            {
                return (0);
            }

            // Get the alarm type
            ushort alarmType = receivedMessage[alreadyParsedBytes];
            alarmType <<= 8;
            alarmType += receivedMessage[alreadyParsedBytes + 1];

            // Get the alarm characterization
            ushort alarmCharacterization = receivedMessage[alreadyParsedBytes + 2];
            alarmCharacterization <<= 8;
            alarmCharacterization += receivedMessage[alreadyParsedBytes + 3];

            int parsedBytes = 4;

            System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfAlarmLogData: {0}-{1}-{2} {3}:{4}:{5} received alarm {6}, value = {7}",
                                               hTime.Day, hTime.Month, hTime.Year, hTime.Hour, hTime.Minute, hTime.Second, alarmType, alarmCharacterization);

            // Pass received data to jobs
            UInt32 alarmInfo = alarmCharacterization;
            alarmInfo <<= 16;
            alarmInfo += alarmType;
            DatumTypes newDataType = DatumTypes.Alarm;
            DatumCategories newDataCategory = DatumCategories.Historical;
            UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, 0, (byte)newDataType, (byte)newDataCategory);
            List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
            GetJobListForNewData(jobSearchKey, ref listJobs);
            if (listJobs.Count > 0)
            {
                bool jobValueSet = false;
                byte[] receivedValueBuffer = LacbusPcProtocol.ConvertUint32ValueToByteArray(BuiltInType.UInt32, alarmInfo, ref jobValueSet);
                if (jobValueSet == true)
                {
                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        uint arrayDim  = lJob.TotalJobSize;
                        if(arrayDim > 0)
                        {
                            if(arrayDim > 4)
                            {
                                arrayDim = 4;
                            }
                            byte[] valueBuffer = new byte[arrayDim];
                            Array.Copy(receivedValueBuffer, valueBuffer, arrayDim);
                            eJob.Values = valueBuffer;
                            eJob.Timestamp = hTime;
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            // Logical inputs coming from alarms
            if ((alarmType == 1) || (alarmType == 2))
            {
                newDataType = DatumTypes.DigitalInput;
                jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, alarmCharacterization, (byte)newDataType, (byte)newDataCategory);
                //listJobs.Clear();
                List<LacbusPCCommJob> listJobs2 = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs2);
                if (listJobs2.Count > 0)
                {
                    bool logicalValue = false;
                    if (alarmType == 1)
                    {
                        logicalValue = true;
                    }

                    // Process the received data
                    foreach (var jobVal in listJobs2)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        eJob.Values = LacbusPcProtocol.ConvertBoolValueToByteArray((BuiltInType)tagDataType, logicalValue, ref jobValueSet);
                        eJob.Timestamp = hTime;
                        if (jobValueSet == true)
                        {
                            System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfAlarmLogData: set DI {0} to value = {1}",
                                                               alarmCharacterization, logicalValue);
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            return (parsedBytes);
        }

        private int SofbusPlParseDuHBlockC(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if(totalMessageLength < (alreadyParsedBytes + 6))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuHBlockC: reached the end of the block");
                return (0);
            }
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)) ||
                (blockHeader.blockBodyLength < 6))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuHBlockC - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                  totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            // Parse the date and time of the historical data
            // Parse the date and time
            int day = receivedMessage[alreadyParsedBytes];
            int month = receivedMessage[alreadyParsedBytes + 1];
            int year = receivedMessage[alreadyParsedBytes + 2];
            year += 2000;
            int hour = receivedMessage[alreadyParsedBytes + 3];
            int minute = receivedMessage[alreadyParsedBytes + 4];
            int second = receivedMessage[alreadyParsedBytes + 5];
            DateTime receivedTime = new DateTime(year, month, day, hour, minute, second);
            receivedTime = ConvertTimeToUtc(receivedTime);

            int parsedBytes = 6;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfDILogData(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, receivedTime);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSequenceOfDILogData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, DateTime hTime)
        {
            // Check the message length
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength < (alreadyParsedBytes + 4))
            {
                return (0);
            }

            // Get the value type
            ushort valueType = receivedMessage[alreadyParsedBytes];
            valueType <<= 8;
            valueType += receivedMessage[alreadyParsedBytes + 1];

            // Get the characterization (DI number)
            ushort datumNumber = receivedMessage[alreadyParsedBytes + 2];
            datumNumber <<= 8;
            datumNumber += receivedMessage[alreadyParsedBytes + 3];

            int parsedBytes = 4;

            System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfDILogData: {0}-{1}-{2} {3}:{4}:{5} received DI{6}, value = {7}",
                                               hTime.Day, hTime.Month, hTime.Year, hTime.Hour, hTime.Minute, hTime.Second, datumNumber, valueType);

            // Pass received data to jobs
            DatumTypes newDataType = DatumTypes.DigitalInput;
            DatumCategories newDataCategory = DatumCategories.Historical;
            UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, datumNumber, (byte)newDataType, (byte)newDataCategory);
            List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
            GetJobListForNewData(jobSearchKey, ref listJobs);
            if (listJobs.Count > 0)
            {
                bool logicalValue = false;
                if (valueType == 1)
                {
                    logicalValue = true;
                }

                // Process the received data
                foreach (var jobVal in listJobs)
                {
                    LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                    if (lJob == null)
                    {
                        continue;
                    }
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = lJob;
                    eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                    bool jobValueSet = false;
                    uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                    eJob.Values = LacbusPcProtocol.ConvertBoolValueToByteArray((BuiltInType)tagDataType, logicalValue, ref jobValueSet);
                    eJob.Timestamp = hTime;
                    if (jobValueSet == true)
                    {
                        OnJobExecuted(eJob);
                    }
                }
            }

            return (parsedBytes);
        }

        private int SofbusPlParseDuHBlockA(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength < (alreadyParsedBytes + 6))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuHBlockA - Reached the end of the block");
                return (0);
            }
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)) ||
                (blockHeader.blockBodyLength < 6))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuHBlockA - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                  totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            // Parse the date and time of the historical data
            // Parse the date and time
            int day = receivedMessage[alreadyParsedBytes];
            int month = receivedMessage[alreadyParsedBytes + 1];
            int year = receivedMessage[alreadyParsedBytes + 2];
            year += 2000;
            int hour = receivedMessage[alreadyParsedBytes + 3];
            int minute = receivedMessage[alreadyParsedBytes + 4];
            int second = receivedMessage[alreadyParsedBytes + 5];
            DateTime receivedTime = new DateTime(year, month, day, hour, minute, second);
            receivedTime = ConvertTimeToUtc(receivedTime);

            int parsedBytes = 6;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfAIorCIValues(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, receivedTime);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSequenceOfAIorCIValues(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, DateTime hTime)
        {
            // Get Datum number and type
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength < (alreadyParsedBytes + 2))
            {
                return (0);
            }
            byte datumType = (byte)(receivedMessage[alreadyParsedBytes] & 0xf0);
            datumType >>= 4;
            ushort datumNumber = (ushort)(receivedMessage[alreadyParsedBytes] & 0x0f);
            datumNumber <<= 8;
            datumNumber += receivedMessage[alreadyParsedBytes + 1];

            // Parse the datum value
            int parsedBytes = 0;
            int dataParsed = 0;
            if (datumType == 1) // AI: 2 bytes
            {
                if (totalMessageLength >= (alreadyParsedBytes + 4))
                {
                    dataParsed = SofbusPlParseSequenceOfAIValuesWithTimeStamp(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + 2, (int)datumNumber, hTime);
                }
            }
            else if (datumType == 2) // CI: 4 bytes
            {
                if (totalMessageLength >= (alreadyParsedBytes + 6))
                {
                    dataParsed = SofbusPlParseSequenceOfCIValuesWithTimeStamp(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + 2, (int)datumNumber, hTime);
                }
            }

            if (dataParsed > 0)
            {
                parsedBytes = 2 + dataParsed;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseDuIBlockL(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuIBlockL - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                   totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            int parsedBytes = 0;
            int nextCINumber = 1;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfCIValues(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, nextCINumber);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
                nextCINumber++;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSequenceOfCIValuesWithTimeStamp(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int nextCINumber, DateTime hTime)
        {
            int parsedBytes = 0;
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength >= (alreadyParsedBytes + 4))
            {
                byte[] auxBuffer = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    auxBuffer[3 - i] = receivedMessage[alreadyParsedBytes + i];
                }
                uint ciValue = BitConverter.ToUInt32(auxBuffer, 0);
                parsedBytes = 4;

                System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfCIValuesWithTimeStamp: {0}-{1}-{2} {3}:{4}:{5} received CI{6}, value = {7}",
                                                   hTime.Day, hTime.Month, hTime.Year, hTime.Hour, hTime.Minute, hTime.Second, nextCINumber, ciValue);

                // Pass received data to jobs
                DatumTypes newDataType = DatumTypes.CountInput;
                DatumCategories newDataCategory = DatumCategories.Historical;
                UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, (ushort)nextCINumber, (byte)newDataType, (byte)newDataCategory);
                List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs);
                if (listJobs.Count > 0)
                {
                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;

                        // If requested, apply the data conversion
                        if (lJob.ConversionCanBeApplied())
                        {
                            eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, lJob.ApplyDataConversion(ciValue), ref jobValueSet);
                        }
                        else
                        {
                            eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, ciValue, ref jobValueSet);
                        }

                        eJob.Timestamp = hTime;
                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSequenceOfCIValues(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int nextCINumber)
        {
            int parsedBytes = 0;
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength >= (alreadyParsedBytes + 4))
            {
                byte[] auxBuffer = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    auxBuffer[3 - i] = receivedMessage[alreadyParsedBytes + i];
                }
                uint ciValue = BitConverter.ToUInt32(auxBuffer, 0);
                parsedBytes = 4;
                System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfCIValues: CI{0} = {1}",
                                                   nextCINumber, ciValue);

                // Pass received data to jobs
                DatumTypes newDataType = DatumTypes.CountInput;
                DatumCategories newDataCategory = DatumCategories.Instantaneous;
                UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, (ushort)nextCINumber, (byte)newDataType, (byte)newDataCategory);
                List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs);
                if (listJobs.Count > 0)
                {
                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        
                        // If requested, apply the data conversion
                        if(lJob.ConversionCanBeApplied())
                        {
                            eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, lJob.ApplyDataConversion(ciValue), ref jobValueSet);
                        }
                        else
                        {
                            eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, ciValue, ref jobValueSet);
                        }

                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfCIValues: reached the end of the block");
            }

            return (parsedBytes);
        }

        private int SofbusPlParseDuIBlockM(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuIBlockM - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                   totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            int parsedBytes = 0;
            int nextAINumber = 1;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfAIValues(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, nextAINumber);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
                nextAINumber++;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSequenceOfAIValuesWithTimeStamp(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int nextAINumber, DateTime hTime)
        {
            int parsedBytes = 0;
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength >= (alreadyParsedBytes + 2))
            {
                byte[] auxBuffer = new byte[2];
                for (int i = 0; i < 2; i++)
                {
                    auxBuffer[1 - i] = receivedMessage[alreadyParsedBytes + i];
                }
                UInt16 aiValue = BitConverter.ToUInt16(auxBuffer, 0);
                parsedBytes = 2;

                System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfAIValuesWithTimeStamp: {0}-{1}-{2} {3}:{4}:{5} received AI{6}, value = {7}",
                                                   hTime.Day, hTime.Month, hTime.Year, hTime.Hour, hTime.Minute, hTime.Second, nextAINumber, aiValue);

                // Pass received data to jobs
                DatumTypes newDataType = DatumTypes.AnalogInput;
                DatumCategories newDataCategory = DatumCategories.Historical;
                UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, (ushort)nextAINumber, (byte)newDataType, (byte)newDataCategory);
                List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs);
                if (listJobs.Count > 0)
                {
                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;

                        // If requested, apply the data conversion
                        if (lJob.ConversionCanBeApplied())
                        {
                            eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, lJob.ApplyDataConversion(aiValue), ref jobValueSet);
                        }
                        else
                        {
                            eJob.Values = LacbusPcProtocol.ConvertUint16ValueToByteArray((BuiltInType)tagDataType, aiValue, ref jobValueSet);
                        }

                        eJob.Timestamp = hTime;
                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSequenceOfAIValues(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int nextAINumber)
        {
            int parsedBytes = 0;
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength >= (alreadyParsedBytes + 2))
            {
                byte[] auxBuffer = new byte[2];
                for (int i = 0; i < 2; i++)
                {
                    auxBuffer[1 - i] = receivedMessage[alreadyParsedBytes + i];
                }
                UInt16 aiValue = BitConverter.ToUInt16(auxBuffer, 0);
                parsedBytes = 2;
                System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfAIValues: AI{0} = {1}",
                                                   nextAINumber, aiValue);

                // Pass received data to jobs
                DatumTypes newDataType = DatumTypes.AnalogInput;
                DatumCategories newDataCategory = DatumCategories.Instantaneous;
                UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, (ushort)nextAINumber, (byte)newDataType, (byte)newDataCategory);
                List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs);
                if (listJobs.Count > 0)
                {
                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;

                        // If requested, apply the data conversion
                        if(lJob.ConversionCanBeApplied())
                        {
                            eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, lJob.ApplyDataConversion(aiValue), ref jobValueSet);
                        }
                        else
                        {
                            eJob.Values = LacbusPcProtocol.ConvertUint16ValueToByteArray((BuiltInType)tagDataType, aiValue, ref jobValueSet);
                        }

                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfAIValues: reached the end of the block");
            }


            return (parsedBytes);
        }

        private int SofbusPlParseDuIBlockS(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseDuIBlockS - Warning invalid length data: Total message length = {0}, parsed bytes = {1} parsed data bytes = {2} message data length = {3}, block body length = {4}",
                                                   totalMessageLength, alreadyParsedBytes, alreadyParsedDataBytes, messageHeader.headerDataLength, blockHeader.blockBodyLength);
                return (0);
            }

            int parsedBytes = 0;
            int nextDINumber = 1;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = SofbusPlParseSequenceOfDIValues(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, nextDINumber);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
                nextDINumber += 8;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSequenceOfDIValues(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int nextDINumber)
        {
            int parsedBytes = 0;
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength >= (alreadyParsedBytes + 1))
            {
                bool[] bitArray = new bool[8];
                int i = 0;
                byte bitMask = 0x80;
                for (i = 7, bitMask = 0x80; i >= 0; i--, bitMask >>= 1)
                {
                    if ((receivedMessage[alreadyParsedBytes] & bitMask) == 0)
                    {
                        bitArray[i] = false;
                    }
                    else
                    {
                        bitArray[i] = true;
                    }
                }
                System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfDIValues: DI{0} = {1}, DI{2} = {3}, DI{4} = {5}, DI{6} = {7}, DI{8} = {9}, DI{10} = {11}, DI{12} = {13}, DI{14} = {15}",
                                                   nextDINumber, bitArray[0], nextDINumber+1, bitArray[1], nextDINumber+2, bitArray[2], nextDINumber+3, bitArray[3], nextDINumber+4, bitArray[4],
                                                   nextDINumber + 5, bitArray[5], nextDINumber + 6, bitArray[6], nextDINumber + 7, bitArray[7]);
                parsedBytes = 1;

                // Pass received data to jobs
                DatumTypes newDataType = DatumTypes.DigitalInput;
                DatumCategories newDataCategory = DatumCategories.Instantaneous;
                for (i = 0; i < 8; i++)
                {
                    int nextDI = nextDINumber + i;
                    UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, (ushort)nextDI, (byte)newDataType, (byte)newDataCategory);
                    List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                    GetJobListForNewData(jobSearchKey, ref listJobs);
                    if (listJobs.Count > 0)
                    {
                        foreach (var jobVal in listJobs)
                        {
                            LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                            if (lJob == null)
                            {
                                continue;
                            }
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.Job = lJob;
                            eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                            bool jobValueSet = false;
                            uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                            //eJob.Values = BitConverter.GetBytes(bitArray[i]);
                            eJob.Values = LacbusPcProtocol.ConvertBoolValueToByteArray((BuiltInType)tagDataType, bitArray[i], ref jobValueSet);
                            if(jobValueSet == true)
                            {
                                OnJobExecuted(eJob);
                            }
                        }
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseSequenceOfDIValues: reached the end of the block");
            }

            return (parsedBytes);
        }

        private void ParseLacbusRtuUMessageData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader)
        {
            // Parse the header of the Data Unit
            int messageLength = receivedMessage.GetLength(0);
            int totalParsedBytes = LacbusPcProtocol.LacbusPCMessageHeaderLength;
            int dataParsedBytes = 0;
            LacbusRtuDataUnitHeader rtuHeader = new LacbusRtuDataUnitHeader();
            int parsedBytes = LacbusPcProtocol.ParseLacbusRtuDUHeader(receivedMessage, totalParsedBytes, ref rtuHeader);
            if (parsedBytes == 0)
            {
                System.Diagnostics.Debug.WriteLine("ParseLacbusRtuUMessageData: error in ParseLacbusRtuDUHeader");
                return;
            }
            totalParsedBytes += parsedBytes;
            dataParsedBytes += parsedBytes;

            // Parse the data part of the Data Unit
            if ((totalParsedBytes < messageLength) && (dataParsedBytes < messageHeader.headerDataLength) && (dataParsedBytes < rtuHeader.DUTotalLength))
            {
                System.Diagnostics.Debug.WriteLine("ParseLacbusRtuUMessageData: parsing DU data");
                LacbusRtuParseDuData(receivedMessage, messageHeader, rtuHeader, totalParsedBytes, dataParsedBytes);
            }
        }

        private void ParseLacbusSofbusSmsSMessageData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader)
        {
            // Parse the header of the SMS PDU
            int messageLength = receivedMessage.GetLength(0);
            int totalParsedBytes = LacbusPcProtocol.LacbusPCMessageHeaderLength;
            int dataParsedBytes = 0;
            LacbusSofbusSmsHeader smsHeader = new LacbusSofbusSmsHeader();
            smsHeader.errorCode = LacbusPcParsingErrorCode.ParsingErrorNoError;
            int parsedBytes = LacbusPcProtocol.ParseLacbusSofbusSmsHeader(receivedMessage, totalParsedBytes, ref smsHeader);
            if (parsedBytes == 0)
            {
                string warningMessage = String.Empty;
                switch (smsHeader.errorCode)
                {
                    case LacbusPcParsingErrorCode.ParsingErrorUnknownSMSVersion:
                        warningMessage = String.Format(Properties.Resources.WarningUnknownSMSProtocolVersion, messageHeader.headerRtuNumber, smsHeader.SHVersion);
                        break;
                    case LacbusPcParsingErrorCode.ParsingErrorMessageTooShort:
                    default:
                        warningMessage = String.Format(Properties.Resources.WarningBadlyFormattedMessage, messageHeader.headerRtuNumber);
                        break;
                }
                CommDriver.OnSystemEvent(null, warningMessage, EventSeverity.Low);
#if DEBUG
                System.Diagnostics.Debug.WriteLine("ParseLacbusSofbusSmsSMessageData: error in ParseLacbusSofbusSmsHeader {0}", warningMessage);
#endif
                return;
            }
            totalParsedBytes += parsedBytes;
            dataParsedBytes += parsedBytes;
#if DEBUG
            // Dump of the header of the SMS
            System.Diagnostics.Debug.WriteLine("ParseLacbusSofbusSmsSMessageData - {0} - SMS Header:", DateTime.Now);
            System.Diagnostics.Debug.WriteLine("Version              = {0}", smsHeader.SHVersion);
            System.Diagnostics.Debug.WriteLine("Size                 = {0}", smsHeader.SHSize);
            System.Diagnostics.Debug.WriteLine("Manufacturer         = {0}", smsHeader.SHManufacturer);
            System.Diagnostics.Debug.WriteLine("SMS Number           = {0}", smsHeader.SHSmsNumber);
            System.Diagnostics.Debug.WriteLine("Site Number          = {0}", smsHeader.SHSiteNumber);
            System.Diagnostics.Debug.WriteLine("Message Type         = {0}", smsHeader.SHMessageType);
            System.Diagnostics.Debug.WriteLine("Dev. Product Version = {0}", smsHeader.SHDeviceProductVersion);
            System.Diagnostics.Debug.WriteLine("Dev. Product Type    = {0}", smsHeader.SHDeviceProductType);
            System.Diagnostics.Debug.WriteLine("Dev. Rank            = {0}", smsHeader.SHDeviceRank);
            System.Diagnostics.Debug.WriteLine("Battery Type         = {0}", smsHeader.SHBatteryType);
#endif

            // Parse the data part of the SMS message
            if ((totalParsedBytes < messageLength))
            {
                System.Diagnostics.Debug.WriteLine("ParseLacbusSofbusSmsUMessageData: parsing DU data");
                LacbusSofbusSmsParseSmsData(receivedMessage, messageHeader, smsHeader, totalParsedBytes, dataParsedBytes);
            }
        }

        private void LacbusSofbusSmsParseSmsData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusSofbusSmsHeader smsHeader, int alreadyParsedBytes, int alreadyParsedDataBytes)
        {
            int messageLength = receivedMessage.GetLength(0);
            int totalParsedBytes = alreadyParsedBytes;
            int dataParsedBytes = alreadyParsedDataBytes;

            // Parse the data blocks
            while ((totalParsedBytes < messageLength) && (dataParsedBytes < messageHeader.headerDataLength))
            {
                int parsedBytes = 0;
                LacbusPcParsingErrorCode errorCode = LacbusPcParsingErrorCode.ParsingErrorNoError;
                // Select the parsing method
                switch (smsHeader.SHVersion)
                {
                    case 0:
                        parsedBytes = LacbusSofbusSmsV0ParseSmsData(receivedMessage, messageHeader, smsHeader, alreadyParsedBytes, ref errorCode);
                        break;
                    case 1:
                        parsedBytes = LacbusSofbusSmsV1ParseSmsData(receivedMessage, messageHeader, smsHeader, alreadyParsedBytes, ref errorCode);
                        break;
                    case 2:
                        parsedBytes = LacbusSofbusSmsV2ParseSmsData(receivedMessage, messageHeader, smsHeader, alreadyParsedBytes, ref errorCode);
                        break;
                }

                // Invalid data block?
                if (parsedBytes == 0)
                {
                    string warningMessage = String.Empty;
                    switch(errorCode)
                    {
                        case LacbusPcParsingErrorCode.ParsingErrorUnknownSMSType:
                            warningMessage = String.Format(Properties.Resources.WarningUnknownSMSType, messageHeader.headerRtuNumber, smsHeader.SHVersion, smsHeader.SHMessageType);
                            break;
                        case LacbusPcParsingErrorCode.ParsingErrorMessageTooShort:
                            warningMessage = String.Format(Properties.Resources.WarningBadlyFormattedMessage, messageHeader.headerRtuNumber);
                            break;
                        case LacbusPcParsingErrorCode.ParsingErrorUnknownDataFormat:
                            warningMessage = String.Format(Properties.Resources.WarningUnknownSMSDataFormat, messageHeader.headerRtuNumber);
                            break;
                        case LacbusPcParsingErrorCode.ParsingErrorDataFormatVersionMismatch:
                            warningMessage = String.Format(Properties.Resources.WarningUnsupportedSMSDataFormat, messageHeader.headerRtuNumber, smsHeader.SHVersion);
                            break;
                        case LacbusPcParsingErrorCode.ParsingErrorArchiveTypeVersionMismatch:
                            warningMessage = String.Format(Properties.Resources.WarningUnsupportedArchiveType, messageHeader.headerRtuNumber, smsHeader.SHVersion);
                            break;
                        case LacbusPcParsingErrorCode.ParsingErrorUnknownArchiveType:
                            warningMessage = String.Format(Properties.Resources.WarningUnsupportedArchiveType, messageHeader.headerRtuNumber);
                            break;
                        default:
                            warningMessage = String.Format(Properties.Resources.WarningBadMessageType, messageHeader.headerRtuNumber);
                            break;
                    }
                    CommDriver.OnSystemEvent(null, warningMessage, EventSeverity.Low);
                    System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseSmsData: error parsing message, messageLength = {0} totalParsedBytes = {1} dataParsedBytes = {2} messageHeader.headerDataLength = {3} protocol version = {4}",
                                                       messageLength, totalParsedBytes, dataParsedBytes, messageHeader.headerDataLength, smsHeader.SHVersion);
                    break;
                }

                totalParsedBytes += parsedBytes;
                dataParsedBytes += parsedBytes;
            }
        }

        private int LacbusSofbusSmsV0ParseSmsData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusSofbusSmsHeader smsHeader, int alreadyParsedBytes, ref LacbusPcParsingErrorCode errorCode)
        {
            // Select the message type
            switch (smsHeader.SHMessageType)
            {
                // Current Data Status
                case 0:
                    return (LacbusSofbusSmsParseCurrentDataStatus(receivedMessage, messageHeader, smsHeader, alreadyParsedBytes, ref errorCode));
                // Archiving
                case 1:
                    return (LacbusSofbusSmsParseArchiving(receivedMessage, messageHeader, smsHeader, alreadyParsedBytes, ref errorCode));
                // Unknown
                default:
                    errorCode = LacbusPcParsingErrorCode.ParsingErrorUnknownSMSType;
                    break;
            }
            return (0);
        }

        private int LacbusSofbusSmsV1ParseSmsData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusSofbusSmsHeader smsHeader, int alreadyParsedBytes, ref LacbusPcParsingErrorCode errorCode)
        {
            // Select the message type
            switch (smsHeader.SHMessageType)
            {
                // Current Data Status
                case 0:
                    return (LacbusSofbusSmsParseCurrentDataStatus(receivedMessage, messageHeader, smsHeader, alreadyParsedBytes, ref errorCode));
                // Archiving
                case 1:
                    return (LacbusSofbusSmsParseArchiving(receivedMessage, messageHeader, smsHeader, alreadyParsedBytes, ref errorCode));
                // Unknown
                default:
                    errorCode = LacbusPcParsingErrorCode.ParsingErrorUnknownSMSType;
                    break;
            }

            return (0);
        }

        private int LacbusSofbusSmsV2ParseSmsData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusSofbusSmsHeader smsHeader, int alreadyParsedBytes, ref LacbusPcParsingErrorCode errorCode)
        {
            // Select the message type
            switch (smsHeader.SHMessageType)
            {
                // Current Data Status
                case 0:
                    return (LacbusSofbusSmsParseCurrentDataStatus(receivedMessage, messageHeader, smsHeader, alreadyParsedBytes, ref errorCode));
                // Archiving
                case 1:
                    return (LacbusSofbusSmsParseArchiving(receivedMessage, messageHeader, smsHeader, alreadyParsedBytes, ref errorCode));
                // Report archiving
                case 2:
                    return (LacbusSofbusSmsParseReportArchiving(receivedMessage, messageHeader, smsHeader, alreadyParsedBytes, ref errorCode));
                // Unknown
                default:
                    errorCode = LacbusPcParsingErrorCode.ParsingErrorUnknownSMSType;
                    break;
            }

            return (0);
        }

        private int LacbusSofbusSmsParseCurrentDataStatus(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusSofbusSmsHeader smsHeader, int alreadyParsedBytes, ref LacbusPcParsingErrorCode errorCode)
        {
            // Check the number of bits to be parsed
            int numOfBytesToBeParsed = receivedMessage.GetLength(0) - alreadyParsedBytes;
            int numOfBitsToBeParsed = numOfBytesToBeParsed*8;
            if (numOfBitsToBeParsed < 48)
            {
                errorCode = LacbusPcParsingErrorCode.ParsingErrorMessageTooShort;
                return (0);
            }

            // Parse the timestamp
            DateTime receivedTime = LacbusPcProtocol.ParseSMSTimeStamp(receivedMessage, alreadyParsedBytes);
            receivedTime = ConvertTimeToUtc(receivedTime);

            int numOfParsedBits = 32;
            int initialByte = alreadyParsedBytes + 4;
            int numOfBitsInTheInitialByte = 8;
            while ((numOfBitsToBeParsed - numOfParsedBits) >= 14)
            {
                // Parse data status
                LacbusSMSCurrentDataStatus currentDataStatus = new LacbusSMSCurrentDataStatus();
                int parsedBits = LacbusPcProtocol.LacbusSofbusSmsParseCurrentDataStatus(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, ref currentDataStatus, ref errorCode);
                if (parsedBits == 0)
                {
                    return (0);
                }
                if ((currentDataStatus.datumFormat == 6) && (smsHeader.SHVersion < 2))
                {
                    errorCode = LacbusPcParsingErrorCode.ParsingErrorDataFormatVersionMismatch;
                    return (0);
                }
                numOfParsedBits += parsedBits;

                // Pass received data to jobs
                DatumTypes newDataType = DatumTypes.AnalogInput;
                DatumCategories newDataCategory = DatumCategories.Instantaneous;
                if (currentDataStatus.datumFormat == 0)
                {
                    newDataType = DatumTypes.DigitalInput;
                }
                UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, currentDataStatus.datumNum, (byte)newDataType, (byte)newDataCategory);
                List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs);
                if (listJobs.Count > 0)
                {
                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }

                        // Check the phone number
                        LacbusPCStation lStation = (LacbusPCStation)lJob.Station;
                        if(lStation == null)
                        {
                            continue;
                        }
                        if((messageHeader.headerPhoneNumber != lStation.LacbusPCPhoneNumber) && !lStation.LacbusPCPhoneNumber.Contains(messageHeader.headerPhoneNumber))
                        {
                            continue;
                        }

                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        switch(currentDataStatus.datumFormat)
                        {
                            case 0: // 1 bit
                                eJob.Values = LacbusPcProtocol.ConvertBoolValueToByteArray((BuiltInType)tagDataType, currentDataStatus.logicalValue, ref jobValueSet);
                                eJob.Timestamp = receivedTime;
                                break;
                            case 1: // 8 bit integer
                            case 2: // 10 bit integer
                            case 3: // 12 bit integer
                            case 4: // 16 bit integer
                            case 5: // 32 bit integer
                                if(((BuiltInType)tagDataType != BuiltInType.Float) && ((BuiltInType)tagDataType != BuiltInType.Double))
                                {
                                    // Integer tag --> Copy the raw value
                                    eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, currentDataStatus.intValue, ref jobValueSet);
                                }
                                else
                                {
                                    // Float or Double tag --> Copy the normalized value
                                    if ((BuiltInType)tagDataType == BuiltInType.Float)
                                    {
                                        eJob.Values = LacbusPcProtocol.ConvertFloatValueToByteArray((BuiltInType)tagDataType, currentDataStatus.floatValue, ref jobValueSet);
                                    }
                                    else
                                    {
                                        eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, currentDataStatus.doubleValue, ref jobValueSet);
                                    }
                                }
                                eJob.Timestamp = receivedTime;
                                break;
                            case 6: // 32 bit float (format defined from Version 2 onwards)
                                eJob.Values = LacbusPcProtocol.ConvertFloatValueToByteArray((BuiltInType)tagDataType, currentDataStatus.floatValue, ref jobValueSet);
                                eJob.Timestamp = receivedTime;
                                break;
                            default:
                                return (0);
                        }
                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                }

                // Pass received data to jobs - alternative data type: Count Input
                if (currentDataStatus.datumFormat != 0)
                {
                    DatumTypes newDataType2 = DatumTypes.CountInput;
                    UInt64 jobSearchKey2 = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, currentDataStatus.datumNum, (byte)newDataType2, (byte)newDataCategory);
                    List<LacbusPCCommJob> listJobs2 = new List<LacbusPCCommJob>();
                    GetJobListForNewData(jobSearchKey2, ref listJobs2);
                    if (listJobs2.Count > 0)
                    {
                        // Process the received data
                        foreach (var jobVal in listJobs2)
                        {
                            LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                            if (lJob == null)
                            {
                                continue;
                            }

                            // Check the phone number
                            LacbusPCStation lStation = (LacbusPCStation)lJob.Station;
                            if (lStation == null)
                            {
                                continue;
                            }
                            if ((messageHeader.headerPhoneNumber != lStation.LacbusPCPhoneNumber) && !lStation.LacbusPCPhoneNumber.Contains(messageHeader.headerPhoneNumber))
                            {
                                continue;
                            }

                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.Job = lJob;
                            eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                            bool jobValueSet = false;
                            uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                            switch (currentDataStatus.datumFormat)
                            {
                                case 0: // 1 bit
                                    eJob.Values = LacbusPcProtocol.ConvertBoolValueToByteArray((BuiltInType)tagDataType, currentDataStatus.logicalValue, ref jobValueSet);
                                    eJob.Timestamp = receivedTime;
                                    break;
                                case 1: // 8 bit integer
                                case 2: // 10 bit integer
                                case 3: // 12 bit integer
                                case 4: // 16 bit integer
                                case 5: // 32 bit integer
                                    if (((BuiltInType)tagDataType != BuiltInType.Float) && ((BuiltInType)tagDataType != BuiltInType.Double))
                                    {
                                        // Integer tag --> Copy the raw value
                                        eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, currentDataStatus.intValue, ref jobValueSet);
                                    }
                                    else
                                    {
                                        // Float or Double tag --> Copy the normalized value
                                        if ((BuiltInType)tagDataType == BuiltInType.Float)
                                        {
                                            eJob.Values = LacbusPcProtocol.ConvertFloatValueToByteArray((BuiltInType)tagDataType, currentDataStatus.floatValue, ref jobValueSet);
                                        }
                                        else
                                        {
                                            eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, currentDataStatus.doubleValue, ref jobValueSet);
                                        }
                                    }
                                    eJob.Timestamp = receivedTime;
                                    break;
                                case 6: // 32 bit float (format defined from Version 2 onwards)
                                    eJob.Values = LacbusPcProtocol.ConvertFloatValueToByteArray((BuiltInType)tagDataType, currentDataStatus.floatValue, ref jobValueSet);
                                    eJob.Timestamp = receivedTime;
                                    break;
                                default:
                                    return (0);
                            }
                            if (jobValueSet == true)
                            {
                                OnJobExecuted(eJob);
                            }
                        }
                    }
                }
            }

            return (alreadyParsedBytes + numOfBytesToBeParsed);
        }

        private int LacbusSofbusSmsParseArchiving(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusSofbusSmsHeader smsHeader, int alreadyParsedBytes, ref LacbusPcParsingErrorCode errorCode)
        {
            // Check the number of bits to be parsed
            int numOfBytesToBeParsed = receivedMessage.GetLength(0) - alreadyParsedBytes;
            if (numOfBytesToBeParsed < 8)
            {
                errorCode = LacbusPcParsingErrorCode.ParsingErrorMessageTooShort;
                return (0);
            }

            // Parse the archiving header
            LacbusSofbusSmsArchivalHeader archivalHeader = new LacbusSofbusSmsArchivalHeader();
            int parsedBytes = LacbusPcProtocol.ParseLacbusSofbusSmsArchivingHeader(receivedMessage, alreadyParsedBytes,
                                                                                   ref archivalHeader);
#if DEBUG
            // Dump of the header of the archiving SMS
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseArchiving - {0} - Archival Header:", DateTime.Now);
            System.Diagnostics.Debug.WriteLine("Initial value timestamp = {0}", archivalHeader.SAHInitialValueTimeStamp);
            System.Diagnostics.Debug.WriteLine("Data Number             = {0}", archivalHeader.SAHDataNumber);
            System.Diagnostics.Debug.WriteLine("Archival Period         = {0}", archivalHeader.SAHArchivalPeriod);
            System.Diagnostics.Debug.WriteLine("Number of Samples       = {0}", archivalHeader.SAHNumberOfSamples);
            System.Diagnostics.Debug.WriteLine("Archival Type           = {0}", archivalHeader.SAHArchivalType);
#endif

            // Parse the archival data
            switch (archivalHeader.SAHArchivalType)
            {
                // Incremental meter
                case 0:
                    if (LacbusSofbusSmsParseIncrementalMeter(receivedMessage, messageHeader, smsHeader, archivalHeader, alreadyParsedBytes + parsedBytes, ref errorCode) == 0)
                    {
                        return (0);
                    }
                    break;

                // Fixed (measurement)
                case 1:
                    if(LacbusSofbusSmsParseArchiveFixed(receivedMessage, messageHeader, smsHeader, archivalHeader, alreadyParsedBytes + parsedBytes, ref errorCode) == 0)
                    {
                        return (0);
                    }
                    break;

                // Incremental meter/flow
                case 2:
                    if(smsHeader.SHVersion < 2)
                    {
                        errorCode = LacbusPcParsingErrorCode.ParsingErrorArchiveTypeVersionMismatch;
                        return (0);
                    }
                    if (LacbusSofbusSmsParseIncrementalMeterFlow(receivedMessage, messageHeader, smsHeader, archivalHeader, alreadyParsedBytes + parsedBytes, ref errorCode) == 0)
                    {
                        return (0);
                    }
                    break;

                // Incremental flow
                case 3:
                    if (smsHeader.SHVersion < 2)
                    {
                        errorCode = LacbusPcParsingErrorCode.ParsingErrorArchiveTypeVersionMismatch;
                        return (0);
                    }
                    if (LacbusSofbusSmsParseIncrementalFlow(receivedMessage, messageHeader, smsHeader, archivalHeader, alreadyParsedBytes + parsedBytes, ref errorCode) == 0)
                    {
                        return (0);
                    }
                    break;

                default:
                    errorCode = LacbusPcParsingErrorCode.ParsingErrorUnknownArchiveType;
                    return (0);
            }


            return (alreadyParsedBytes + numOfBytesToBeParsed);
        }

        private int LacbusSofbusSmsParseReportArchiving(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusSofbusSmsHeader smsHeader, int alreadyParsedBytes, ref LacbusPcParsingErrorCode errorCode)
        {
            // Check the number of bytes to be parsed
            int numOfBytesToBeParsed = receivedMessage.GetLength(0) - alreadyParsedBytes;
            if (numOfBytesToBeParsed < 6)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseReportArchiving - Error message too short");
#endif
                errorCode = LacbusPcParsingErrorCode.ParsingErrorMessageTooShort;
                return (0);
            }

            // Parse the report archiving header
            int initialByte = alreadyParsedBytes;
            int numOfBitsInTheInitialByte = 8;
            LacbusSofbusSmsReportArchivalHeader reportArchivalHeader = new LacbusSofbusSmsReportArchivalHeader();
            int parsedBits = LacbusPcProtocol.ParseLacbusSofbusSmsReportArchivingHeader(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte,
                                                                                        ref reportArchivalHeader);
#if DEBUG
            // Dump of the header of the report archiving SMS
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseReportArchiving - {0} - Report Archival Header:", DateTime.Now);
            System.Diagnostics.Debug.WriteLine("Number of Report Blocks      = {0}", reportArchivalHeader.SRAHNumberOfReportBlocks);
            System.Diagnostics.Debug.WriteLine("Header Size                  = {0}", reportArchivalHeader.SRAHHeaderSize);
            System.Diagnostics.Debug.WriteLine("Timestamp of diagnostic data = {0}", reportArchivalHeader.SRAHTimeStamp);
            System.Diagnostics.Debug.WriteLine("Number of Diagnostic Data    = {0}", reportArchivalHeader.SRAHNumberOfDiagnosticData);
#endif

            // Parse the diagnostic data
            uint remainingBits = (uint)((receivedMessage.GetLength(0) - initialByte - 1) * 8 + numOfBitsInTheInitialByte);
            uint numberOfParsedDiagnosticData = 0;
            uint diagnosticDataValue = 0;
            DatumTypes newDataType = DatumTypes.AnalogInput;
            DatumCategories newDataCategory = DatumCategories.Historical;
            while ((remainingBits >= 17) && (numberOfParsedDiagnosticData < reportArchivalHeader.SRAHNumberOfDiagnosticData))
            {
                // Parse the data number
                uint diagnosticDataNumber = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 7);
                remainingBits -= 7;
                // Parse the format
                uint diagnosticDataFormat = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 2);
                remainingBits -= 2;
                // Parse the diagnostic value
                bool valueParsed = false;
                switch (diagnosticDataFormat)
                {
                    // 8 bit integer
                    case 0:
                        if (remainingBits < 8)
                        {
                            remainingBits = 0;
                        }
                        else
                        {
                            diagnosticDataValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 8);
                            remainingBits -= 8;
                            valueParsed = true;
                        }
                        break;

                    // 16 bit integer
                    case 1:
                        if (remainingBits < 16)
                        {
                            remainingBits = 0;
                        }
                        else
                        {
                            diagnosticDataValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 16);
                            remainingBits -= 16;
                            valueParsed = true;
                        }
                        break;
                }

                if (valueParsed == true)
                {
                    numberOfParsedDiagnosticData++;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseReportArchiving - Timestamp = {0}, Diagnostic Data Nr. = {1}, Diagnostic Value = {2}, Data Format = {3}",
                                                       reportArchivalHeader.SRAHTimeStamp, diagnosticDataNumber, diagnosticDataValue, diagnosticDataFormat);
#endif
                    // Pass data to jobs
                    UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)diagnosticDataNumber, (byte)newDataType, (byte)newDataCategory);
                    List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                    GetJobListForNewData(jobSearchKey, ref listJobs);
                    if (listJobs.Count > 0)
                    {
                        // Calculate the normalized Value
                        double normalizedValue = (double)diagnosticDataValue;
                        //switch (diagnosticDataFormat)
                        //{
                        //    case 0: // 8 bit integer
                        //        normalizedValue = (double)diagnosticDataValue / (double)0xFF * 100.0;
                        //        break;
                        //    case 1: // 16 bit integer
                        //        normalizedValue = (double)diagnosticDataValue / (double)0xFFFF * 100.0;
                        //        break;
                        //}

                        foreach (var jobVal in listJobs)
                        {
                            LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                            if (lJob == null)
                            {
                                continue;
                            }
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.Job = lJob;
                            eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                            bool jobValueSet = false;
                            uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                            if (((BuiltInType)tagDataType != BuiltInType.Double) && ((BuiltInType)tagDataType != BuiltInType.Float))
                            {
                                // Integer tag --> Copy the raw value
                                eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, diagnosticDataValue, ref jobValueSet);
                            }
                            else
                            {
                                // Float or Double tag --> Copy the normalized value
                                eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, normalizedValue, ref jobValueSet);
                            }
                            eJob.Timestamp = reportArchivalHeader.SRAHTimeStamp;
                            if (jobValueSet == true)
                            {
                                OnJobExecuted(eJob);
                            }
                        }
                    }

                    // Pass received data to jobs - alternative data type: Count Input
                    DatumTypes newDataType2 = DatumTypes.CountInput;
                    UInt64 jobSearchKey2 = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)diagnosticDataNumber, (byte)newDataType2, (byte)newDataCategory);
                    List<LacbusPCCommJob> listJobs2 = new List<LacbusPCCommJob>();
                    GetJobListForNewData(jobSearchKey2, ref listJobs2);
                    if (listJobs2.Count > 0)
                    {
                        double normalizedValue = (double)diagnosticDataValue;
                        foreach (var jobVal in listJobs2)
                        {
                            LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                            if (lJob == null)
                            {
                                continue;
                            }
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.Job = lJob;
                            eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                            bool jobValueSet = false;
                            uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                            if (((BuiltInType)tagDataType != BuiltInType.Double) && ((BuiltInType)tagDataType != BuiltInType.Float))
                            {
                                // Integer tag --> Copy the raw value
                                eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, diagnosticDataValue, ref jobValueSet);
                            }
                            else
                            {
                                // Float or Double tag --> Copy the normalized value
                                eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, normalizedValue, ref jobValueSet);
                            }
                            eJob.Timestamp = reportArchivalHeader.SRAHTimeStamp;
                            if (jobValueSet == true)
                            {
                                OnJobExecuted(eJob);
                            }
                        }
                    }
                }
                else
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseReportArchiving - Error parsing diagnostic data");
#endif
                    errorCode = LacbusPcParsingErrorCode.ParsingErrorUnknownDataFormat;
                    return (0);
                }

            }

            // Framing for diagnostic data
            initialByte = alreadyParsedBytes + reportArchivalHeader.SRAHHeaderSize;
            remainingBits -= (uint)numOfBitsInTheInitialByte;
            numOfBitsInTheInitialByte = 8;

            // Parse the report archival data blocks
            byte numberOfParsedDataBlocks = 0;
            while((numberOfParsedDataBlocks < reportArchivalHeader.SRAHNumberOfReportBlocks) && (remainingBits >= 73))
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseReportArchiving - Data Block Nr. = {0}",
                                                   numberOfParsedDataBlocks + 1);
#endif
                if(LacbusSofbusSmsParseReportDataBlock(receivedMessage, messageHeader, smsHeader, ref initialByte, ref numOfBitsInTheInitialByte, ref remainingBits) == 0)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseReportArchiving - Error parsing Data Block Nr. = {0}",
                                                       numberOfParsedDataBlocks + 1);
#endif
                    errorCode = LacbusPcParsingErrorCode.ParsingErrorUnknownDataFormat;
                    return (0);
                }
                numberOfParsedDataBlocks++;
            }

            return (alreadyParsedBytes + numOfBytesToBeParsed);
        }

        private int LacbusSofbusSmsParseReportDataBlock(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusSofbusSmsHeader smsHeader, ref int initialByte, ref int numOfBitsInTheInitialByte, ref uint numberOfRemainingBits)
        {
            // Parse the timestamp
            uint previousNumberOfRemainingBits = numberOfRemainingBits;
            DateTime dataBlockTimeStamp = LacbusPcProtocol.ParseSMSTimeStamp(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte);
            numberOfRemainingBits -= 27;
            dataBlockTimeStamp = ConvertTimeToUtc(dataBlockTimeStamp);

            // Parse the number of data (5 bits)
            uint numberOfData = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 5);
            numberOfRemainingBits -= 5;

#if DEBUG
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseReportDataBlock - Timestamp = {0}, Number of data = {1}",
                                               dataBlockTimeStamp, numberOfData);
#endif

            // Parse the data values
            DatumTypes newDataType = DatumTypes.AnalogInput;
            DatumCategories newDataCategory = DatumCategories.Historical;
            uint numberOfParsedData = 0;
            while((numberOfParsedData < numberOfData) && (numberOfRemainingBits >= 39))
            {
                // Parse the data number (7 bits)
                uint dataNumber = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 7);
                numberOfRemainingBits -= 7;

                // Parse the data format (2 bits)
                uint dataFormat = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 2);
                numberOfRemainingBits -= 2;
                bool valueParsed = false;
                bool parsedValueIsFloat = false;
                uint intDataValue = 0;
                float floatDataValue = 0.0F;
                switch (dataFormat)
                {
                    // 32 bit integer
                    case 0:
                        intDataValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 32);
                        numberOfRemainingBits -= 32;
                        valueParsed = true;
                        numberOfParsedData++;
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseReportDataBlock - Data Nr. = {0}, Value = {1}, Data Format = {2}",
                                                           dataNumber, intDataValue, dataFormat);
#endif
                        break;

                    // 32 bit float
                    case 1:
                        floatDataValue = LacbusPcProtocol.LacbusSofbusSmsParseFloatFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte);
                        numberOfRemainingBits -= 32;
                        valueParsed = true;
                        parsedValueIsFloat = true;
                        numberOfParsedData++;
#if DEBUG
                        System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseReportDataBlock - Data Nr. = {0}, Value = {1}, Data Format = {2}",
                                                           dataNumber, floatDataValue, dataFormat);
#endif
                        break;
                }

                if (valueParsed == false)
                {
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseReportDataBlock - Error parsing data format");
#endif
                    return (0);
                }

                // Pass data to the jobs
                UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)dataNumber, (byte)newDataType, (byte)newDataCategory);
                List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs);
                if (listJobs.Count > 0)
                {
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        if(parsedValueIsFloat == false)
                        {
                            eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, intDataValue, ref jobValueSet);
                        }
                        else
                        {
                            eJob.Values = LacbusPcProtocol.ConvertFloatValueToByteArray((BuiltInType)tagDataType, floatDataValue, ref jobValueSet);
                        }
                        eJob.Timestamp = dataBlockTimeStamp;
                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                }

                // Pass received data to jobs - alternative data type: Count Input
                DatumTypes newDataType2 = DatumTypes.CountInput;
                UInt64 jobSearchKey2 = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)dataNumber, (byte)newDataType2, (byte)newDataCategory);
                List<LacbusPCCommJob> listJobs2 = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey2, ref listJobs2);
                if (listJobs2.Count > 0)
                {
                    foreach (var jobVal in listJobs2)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        if (parsedValueIsFloat == false)
                        {
                            eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, intDataValue, ref jobValueSet);
                        }
                        else
                        {
                            eJob.Values = LacbusPcProtocol.ConvertFloatValueToByteArray((BuiltInType)tagDataType, floatDataValue, ref jobValueSet);
                        }
                        eJob.Timestamp = dataBlockTimeStamp;
                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                }

            }

            // Return the number of parsed bits
            return ((int)(previousNumberOfRemainingBits - numberOfRemainingBits));
        }

        private int LacbusSofbusSmsParseArchiveFixed(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusSofbusSmsHeader smsHeader, LacbusSofbusSmsArchivalHeader archivalHeader, int alreadyParsedBytes, ref LacbusPcParsingErrorCode errorCode)
        {
            int numOfBytesToBeParsed = receivedMessage.GetLength(0) - alreadyParsedBytes;
            if (numOfBytesToBeParsed < 2)
            {
                errorCode = LacbusPcParsingErrorCode.ParsingErrorMessageTooShort;
                return (0);
            }

            // Parse the sample size
            byte sampleFormat = receivedMessage[alreadyParsedBytes];
            int sampleSizeInBits = 0;
            switch(sampleFormat)
            {
                case 0:
                    sampleSizeInBits = 8;
                    break;

                case 1:
                    sampleSizeInBits = 10;
                    break;

                case 2:
                    sampleSizeInBits = 12;
                    break;

                case 3:
                    sampleSizeInBits = 16;
                    break;

                default:
                    errorCode = LacbusPcParsingErrorCode.ParsingErrorUnknownDataFormat;
                    return (0);
            }

            // Check if all the samples have been received
            int expectedArchivalDataLengthInBits = sampleSizeInBits * archivalHeader.SAHNumberOfSamples;
            int receivedDataLengthInBits = (numOfBytesToBeParsed - 1) * 8;
            if(receivedDataLengthInBits < expectedArchivalDataLengthInBits)
            {
                errorCode = LacbusPcParsingErrorCode.ParsingErrorMessageTooShort;
                return (0);
            }

            // Check if any job has been defined for the received data
            DatumTypes newDataType = DatumTypes.AnalogInput;
            DatumCategories newDataCategory = DatumCategories.Historical;
            UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)archivalHeader.SAHDataNumber, (byte)newDataType, (byte)newDataCategory);
            List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
            GetJobListForNewData(jobSearchKey, ref listJobs);
            // Alternative Data Type: CountInput
            DatumTypes newDataType2 = DatumTypes.CountInput;
            UInt64 jobSearchKey2 = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)archivalHeader.SAHDataNumber, (byte)newDataType2, (byte)newDataCategory);
            List<LacbusPCCommJob> listJobs2 = new List<LacbusPCCommJob>();
            GetJobListForNewData(jobSearchKey2, ref listJobs2);
            if ((listJobs.Count == 0) && (listJobs2.Count == 0))
            {
                return (numOfBytesToBeParsed);
            }

            // Parse the sample data
            TimeSpan deltaTime = new TimeSpan(0, archivalHeader.SAHArchivalPeriod, 0);
            DateTime sampleTime = archivalHeader.SAHInitialValueTimeStamp;
            sampleTime = ConvertTimeToUtc(sampleTime);

            int numOfParsedBits = 0;
            int initialByte = alreadyParsedBytes + 1;
            int numOfBitsInTheInitialByte = 8;
            while ((expectedArchivalDataLengthInBits - numOfParsedBits) >= sampleSizeInBits)
            {
                uint sampleValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, sampleSizeInBits);
                // Calculate the normalized Value
                double normalizedSampleValue = (double)sampleValue;
                //switch(sampleFormat)
                //{
                //    case 0: // 8 bit integer
                //        normalizedSampleValue = (double)sampleValue / (double)0xFF * 100.0;
                //        break;
                //    case 1: // 10 bit integer
                //        normalizedSampleValue = (double)sampleValue / (double)0x3FF * 100.0;
                //        break;
                //    case 2: // 12 bit integer
                //        normalizedSampleValue = (double)sampleValue / (double)0xFFF * 100.0;
                //        break;
                //    case 3: // 16 bit integer
                //        normalizedSampleValue = (double)sampleValue / (double)0xFFFF * 100.0;
                //        break;
                //}

#if DEBUG
                System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseArchiveFixed - Timestamp = {0}, Sample Value = {1}, Normalized Value = {2}", sampleTime, sampleValue, normalizedSampleValue);
#endif
                numOfParsedBits += sampleSizeInBits;

                // Pass received data to jobs
                foreach (var jobVal in listJobs)
                {
                    LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                    if (lJob == null)
                    {
                        continue;
                    }
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = lJob;
                    eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                    bool jobValueSet = false;
                    uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                    if(((BuiltInType)tagDataType != BuiltInType.Double) && ((BuiltInType)tagDataType != BuiltInType.Float))
                    {
                        // Integer tag --> Copy the raw value
                        eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, sampleValue, ref jobValueSet);
                    }
                    else
                    {
                        // Float or Double tag --> Copy the normalized value
                        eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, normalizedSampleValue, ref jobValueSet);
                    }
                    eJob.Timestamp = sampleTime;
                    if (jobValueSet == true)
                    {
                        OnJobExecuted(eJob);
                    }
                }

                // Pass received data to jobs - alternative data type: Count Input
                foreach (var jobVal in listJobs2)
                {
                    LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                    if (lJob == null)
                    {
                        continue;
                    }
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = lJob;
                    eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                    bool jobValueSet = false;
                    uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                    if (((BuiltInType)tagDataType != BuiltInType.Double) && ((BuiltInType)tagDataType != BuiltInType.Float))
                    {
                        // Integer tag --> Copy the raw value
                        eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, sampleValue, ref jobValueSet);
                    }
                    else
                    {
                        // Float or Double tag --> Copy the normalized value
                        eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, normalizedSampleValue, ref jobValueSet);
                    }
                    eJob.Timestamp = sampleTime;
                    if (jobValueSet == true)
                    {
                        OnJobExecuted(eJob);
                    }
                }

                // Increment the sample timestamp
                sampleTime += deltaTime;
            }

            return (numOfBytesToBeParsed);
        }

        private int LacbusSofbusSmsParseIncrementalMeter(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusSofbusSmsHeader smsHeader, LacbusSofbusSmsArchivalHeader archivalHeader, int alreadyParsedBytes, ref LacbusPcParsingErrorCode errorCode)
        {
            int numOfBytesToBeParsed = receivedMessage.GetLength(0) - alreadyParsedBytes;
            if (numOfBytesToBeParsed < 5)
            {
                errorCode = LacbusPcParsingErrorCode.ParsingErrorMessageTooShort;
                return (0);
            }

            // Check if any job has been defined for the received data
            DatumTypes newDataType = DatumTypes.AnalogInput;
            DatumCategories newDataCategory = DatumCategories.Historical;
            UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)archivalHeader.SAHDataNumber, (byte)newDataType, (byte)newDataCategory);
            List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
            GetJobListForNewData(jobSearchKey, ref listJobs);
            // Alternative Data Type: CountInput
            DatumTypes newDataType2 = DatumTypes.CountInput;
            UInt64 jobSearchKey2 = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)archivalHeader.SAHDataNumber, (byte)newDataType2, (byte)newDataCategory);
            List<LacbusPCCommJob> listJobs2 = new List<LacbusPCCommJob>();
            GetJobListForNewData(jobSearchKey2, ref listJobs2);
            if ((listJobs.Count == 0) && (listJobs2.Count == 0))
            {
                return (numOfBytesToBeParsed);
            }

            // Parse the initial value
            //int initialByte = alreadyParsedBytes + 1;
            int initialByte = alreadyParsedBytes;
            int numOfBitsInTheInitialByte = 8;
            uint sampleValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 32);
            // The increment size is always 0 for the initial value
            uint incrementSize = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 2);

            // Pass the initial value to jobs
            TimeSpan deltaTime = new TimeSpan(0, archivalHeader.SAHArchivalPeriod, 0);
            DateTime sampleTime = archivalHeader.SAHInitialValueTimeStamp;
            sampleTime = ConvertTimeToUtc(sampleTime);
            uint incrementValue = 0;
#if DEBUG
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseIncrementalMeter - Timestamp = {0}, Sample Value = {1}, Increment Format = {2}, Increment Value = {3}",
                                               sampleTime, sampleValue, incrementSize, incrementValue);
#endif
            foreach (var jobVal in listJobs)
            {
                LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                if (lJob == null)
                {
                    continue;
                }
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = lJob;
                eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                bool jobValueSet = false;
                uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, sampleValue, ref jobValueSet);
                eJob.Timestamp = sampleTime;
                if (jobValueSet == true)
                {
                    OnJobExecuted(eJob);
                }
            }

            // Pass the initial value to jobs - alternative data type: Count Input 
            foreach (var jobVal in listJobs2)
            {
                LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                if (lJob == null)
                {
                    continue;
                }
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = lJob;
                eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                bool jobValueSet = false;
                uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, sampleValue, ref jobValueSet);
                eJob.Timestamp = sampleTime;
                if (jobValueSet == true)
                {
                    OnJobExecuted(eJob);
                }
            }

            // Increment the sample timestamp
            sampleTime += deltaTime;

            // Parse the rest of data
            uint numberOfParsedSamples = 1;
            uint remainingBits = (uint)((receivedMessage.GetLength(0) - initialByte - 1) * 8 + numOfBitsInTheInitialByte);
            while ((remainingBits >= 2) && (numberOfParsedSamples < archivalHeader.SAHNumberOfSamples))
            {
                // Parse the increment size
                incrementSize = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 2);
                remainingBits -= 2;
                bool valueParsed = false;
                switch(incrementSize)
                {
                    // No increment
                    case 0:
                        valueParsed = true;
                        incrementValue = 0;
                    break;

                    // Increment format == 6 bits
                    case 1:
                        if(remainingBits < 6)
                        {
                            remainingBits = 0;
                        }
                        else
                        {
                            // Parse the increment value
                            incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 6);
                            remainingBits -= 6;
                            valueParsed = true;
                        }
                    break;

                    // Increment format == 14 bits
                    case 2:
                        if (remainingBits < 14)
                        {
                            remainingBits = 0;
                        }
                        else
                        {
                            // Parse the increment value
                            incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 14);
                            remainingBits -= 14;
                            valueParsed = true;
                        }
                    break;

                    // Increment format == 22 bits
                    case 3:
                        if (remainingBits < 22)
                        {
                            remainingBits = 0;
                        }
                        else
                        {
                            // Parse the increment value
                            incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 22);
                            remainingBits -= 22;
                            valueParsed = true;
                        }
                    break;
                }

                if (valueParsed == true)
                {
                    numberOfParsedSamples++;

                    // Calculate the sample value
                    sampleValue += incrementValue;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseIncrementalMeter - Timestamp = {0}, Sample Value = {1}, Increment Format = {2}, Increment Value = {3}",
                                                       sampleTime, sampleValue, incrementSize, incrementValue);
#endif
                    // Pass data to jobs
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, sampleValue, ref jobValueSet);
                        eJob.Timestamp = sampleTime;
                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }

                    // Pass data to jobs - alternative data type: Count Input
                    foreach (var jobVal in listJobs2)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, sampleValue, ref jobValueSet);
                        eJob.Timestamp = sampleTime;
                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }

                    // Increment the sample timestamp
                    sampleTime += deltaTime;
                }
                else
                {
                    // Stop the parsing loop
                    remainingBits = 0;
                }
            }

            return (numOfBytesToBeParsed);
        }

        private int LacbusSofbusSmsParseIncrementalMeterFlow(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusSofbusSmsHeader smsHeader, LacbusSofbusSmsArchivalHeader archivalHeader, int alreadyParsedBytes, ref LacbusPcParsingErrorCode errorCode)
        {
            int numOfBytesToBeParsed = receivedMessage.GetLength(0) - alreadyParsedBytes;
            if (numOfBytesToBeParsed < 10)
            {
                errorCode = LacbusPcParsingErrorCode.ParsingErrorMessageTooShort;
                return (0);
            }

            // Get the list of jobs defined for the received meter data
            DatumTypes newDataType = DatumTypes.AnalogInput;
            DatumCategories newDataCategory = DatumCategories.Historical;
            UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)archivalHeader.SAHDataNumber, (byte)newDataType, (byte)newDataCategory);
            List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
            GetJobListForNewData(jobSearchKey, ref listJobs);

            // Get the list of jobs defined for the received meter data - alternative data type: Count Input
            DatumTypes newDataType2 = DatumTypes.CountInput;
            UInt64 jobSearchKey2 = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)archivalHeader.SAHDataNumber, (byte)newDataType2, (byte)newDataCategory);
            List<LacbusPCCommJob> listJobs2 = new List<LacbusPCCommJob>();
            GetJobListForNewData(jobSearchKey2, ref listJobs2);

            // Parse the flow data number
            int initialByte = alreadyParsedBytes;
            int numOfBitsInTheInitialByte = 8;
            uint flowDataNumber = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 10);

            // Get the list of jobs defined for the received flow data
            UInt64 flowJobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)flowDataNumber, (byte)newDataType, (byte)newDataCategory);
            List<LacbusPCCommJob> flowListJobs = new List<LacbusPCCommJob>();
            GetJobListForNewData(flowJobSearchKey, ref flowListJobs);

            // Get the list of jobs defined for the received flow data - alternative data type: Count Input
            UInt64 flowJobSearchKey2 = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)flowDataNumber, (byte)newDataType2, (byte)newDataCategory);
            List<LacbusPCCommJob> flowListJobs2 = new List<LacbusPCCommJob>();
            GetJobListForNewData(flowJobSearchKey2, ref flowListJobs2);

            // Check if any job has been defined for the received flow data
            if ((listJobs.Count == 0) && (flowListJobs.Count == 0) && (listJobs2.Count == 0) && (flowListJobs2.Count == 0))
            {
                return (numOfBytesToBeParsed);
            }

#if DEBUG
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseIncrementalMeterFlow - Flow Info Number  = {0}", flowDataNumber);
#endif

            // Parse the flow calculation factor
            float flowCalculationFactor = LacbusPcProtocol.LacbusSofbusSmsParseFloatFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte);
            // double value used in calculations
            double doubleFlowCalculationValue = flowCalculationFactor; 

            // Parse the initial meter value
            uint sampleValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 32);
 
            // Parse the initial flow increment size
            uint incrementSize = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 2);

            // Parse the initial flow increment
            bool valueParsed = false;
            uint incrementValue = 0;
            uint remainingBits = (uint)((receivedMessage.GetLength(0) - initialByte - 1) * 8 + numOfBitsInTheInitialByte);
            switch (incrementSize)
            {
                // No increment
                case 0:
                    valueParsed = true;
                    incrementValue = 0;
                    break;

                // Increment format == 6 bits
                case 1:
                    // Parse the increment value
                    if (remainingBits < 6)
                    {
                        remainingBits = 0;
                    }
                    else
                    {
                        incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 6);
                        valueParsed = true;
                        remainingBits -= 6;
                    }
                    break;

                // Increment format == 14 bits
                case 2:
                    // Parse the increment value
                    if (remainingBits < 14)
                    {
                        remainingBits = 0;
                    }
                    else
                    {
                        incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 14);
                        valueParsed = true;
                        remainingBits -= 14;
                    }
                    break;

                // Increment format == 22 bits
                case 3:
                    if (remainingBits < 22)
                    {
                        remainingBits = 0;
                    }
                    else
                    {
                        // Parse the increment value
                        incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 22);
                        remainingBits -= 22;
                        valueParsed = true;
                    }
                    break;
            }
            if(valueParsed == false)
            {
                return (0);
            }

            // Calculate the initial value of the flow rate
            double flowRate = doubleFlowCalculationValue * incrementValue;
 
            // Pass the initial values of meter and flow to the jobs
            TimeSpan deltaTime = new TimeSpan(0, archivalHeader.SAHArchivalPeriod, 0);
            DateTime sampleTime = archivalHeader.SAHInitialValueTimeStamp;
            sampleTime = ConvertTimeToUtc(sampleTime);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseIncrementalMeterFlow - Timestamp = {0}, Sample Value = {1}, Increment Format = {2}, Increment Value = {3}, Flow Factor = {4}, Flow Rate = {5}",
                                               sampleTime, sampleValue, incrementSize, incrementValue, flowCalculationFactor, flowRate);
#endif
            // Meter Value
            foreach (var jobVal in listJobs)
            {
                LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                if (lJob == null)
                {
                    continue;
                }
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = lJob;
                eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                bool jobValueSet = false;
                uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, sampleValue, ref jobValueSet);
                eJob.Timestamp = sampleTime;
                if (jobValueSet == true)
                {
                    OnJobExecuted(eJob);
                }
            }

            // Meter Value - alternative data type: Count Input
            foreach (var jobVal in listJobs2)
            {
                LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                if (lJob == null)
                {
                    continue;
                }
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = lJob;
                eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                bool jobValueSet = false;
                uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, sampleValue, ref jobValueSet);
                eJob.Timestamp = sampleTime;
                if (jobValueSet == true)
                {
                    OnJobExecuted(eJob);
                }
            }

            // Flow Rate
            foreach (var jobVal in flowListJobs)
            {
                LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                if (lJob == null)
                {
                    continue;
                }
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = lJob;
                eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                bool jobValueSet = false;
                uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, flowRate, ref jobValueSet);
                eJob.Timestamp = sampleTime;
                if (jobValueSet == true)
                {
                    OnJobExecuted(eJob);
                }
            }

            // Flow Rate - alternative data type: Count Input
            foreach (var jobVal in flowListJobs2)
            {
                LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                if (lJob == null)
                {
                    continue;
                }
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = lJob;
                eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                bool jobValueSet = false;
                uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, flowRate, ref jobValueSet);
                eJob.Timestamp = sampleTime;
                if (jobValueSet == true)
                {
                    OnJobExecuted(eJob);
                }
            }

            // Increment the sample timestamp
            sampleTime += deltaTime;

            // Parse the rest of data
            uint numberOfParsedSamples = 1;
            remainingBits = (uint)((receivedMessage.GetLength(0) - initialByte - 1) * 8 + numOfBitsInTheInitialByte);
            while ((remainingBits >= 2) && (numberOfParsedSamples < archivalHeader.SAHNumberOfSamples))
            {
                // Parse the increment size
                incrementSize = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 2);
                remainingBits -= 2;
                valueParsed = false;
                switch (incrementSize)
                {
                    // No increment
                    case 0:
                        valueParsed = true;
                        incrementValue = 0;
                        break;

                    // Increment format == 6 bits
                    case 1:
                        if (remainingBits < 6)
                        {
                            remainingBits = 0;
                        }
                        else
                        {
                            // Parse the increment value
                            incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 6);
                            remainingBits -= 6;
                            valueParsed = true;
                        }
                        break;

                    // Increment format == 14 bits
                    case 2:
                        if (remainingBits < 14)
                        {
                            remainingBits = 0;
                        }
                        else
                        {
                            // Parse the increment value
                            incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 14);
                            remainingBits -= 14;
                            valueParsed = true;
                        }
                        break;

                    // Increment format == 22 bits
                    case 3:
                        if (remainingBits < 22)
                        {
                            remainingBits = 0;
                        }
                        else
                        {
                            // Parse the increment value
                            incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 22);
                            remainingBits -= 22;
                            valueParsed = true;
                        }
                        break;
                }

                if (valueParsed == true)
                {
                    numberOfParsedSamples++;

                    // Calculate the sample value
                    sampleValue += incrementValue;
                    // Calculate the flow rate
                    flowRate = doubleFlowCalculationValue * incrementValue;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseIncrementalMeterFlow - Timestamp = {0}, Sample Value = {1}, Increment Format = {2}, Increment Value = {3}, Flow Factor = {4}, Flow Rate = {5}",
                                                       sampleTime, sampleValue, incrementSize, incrementValue, flowCalculationFactor, flowRate);
#endif
                    // Pass meter data to jobs
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, sampleValue, ref jobValueSet);
                        eJob.Timestamp = sampleTime;
                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                    // Pass flow data to jobs
                    foreach (var jobVal in flowListJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, flowRate, ref jobValueSet);
                        eJob.Timestamp = sampleTime;
                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }

                    // Pass meter data to jobs - alternative data type: Count Input
                    foreach (var jobVal in listJobs2)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, sampleValue, ref jobValueSet);
                        eJob.Timestamp = sampleTime;
                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                    // Pass flow data to jobs - alternative data type: Count Input
                    foreach (var jobVal in flowListJobs2)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, flowRate, ref jobValueSet);
                        eJob.Timestamp = sampleTime;
                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }

                    // Increment the sample timestamp
                    sampleTime += deltaTime;
                }
                else
                {
                    // Stop the parsing loop
                    remainingBits = 0;
                }
            }

            return (numOfBytesToBeParsed);
        }

        private int LacbusSofbusSmsParseIncrementalFlow(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusSofbusSmsHeader smsHeader, LacbusSofbusSmsArchivalHeader archivalHeader, int alreadyParsedBytes, ref LacbusPcParsingErrorCode errorCode)
        {
            int numOfBytesToBeParsed = receivedMessage.GetLength(0) - alreadyParsedBytes;
            if (numOfBytesToBeParsed < 10)
            {
                errorCode = LacbusPcParsingErrorCode.ParsingErrorMessageTooShort;
                return (0);
            }

            //// Get the list of jobs defined for the received meter data
            DatumTypes newDataType = DatumTypes.AnalogInput;
            DatumCategories newDataCategory = DatumCategories.Historical;
            //UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)archivalHeader.SAHDataNumber, (byte)newDataType, (byte)newDataCategory);
            //List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
            //GetJobListForNewData(jobSearchKey, ref listJobs);

            // Parse the flow data number
            int initialByte = alreadyParsedBytes;
            int numOfBitsInTheInitialByte = 8;
            uint flowDataNumber = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 10);

            // Get the list of jobs defined for the received flow data
            UInt64 flowJobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)flowDataNumber, (byte)newDataType, (byte)newDataCategory);
            List<LacbusPCCommJob> flowListJobs = new List<LacbusPCCommJob>();
            GetJobListForNewData(flowJobSearchKey, ref flowListJobs);

            // Get the list of jobs defined for the received flow data - alternative data type: Count Input
            DatumTypes newDataType2 = DatumTypes.CountInput;
            UInt64 flowJobSearchKey2 = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)flowDataNumber, (byte)newDataType2, (byte)newDataCategory);
            List<LacbusPCCommJob> flowListJobs2 = new List<LacbusPCCommJob>();
            GetJobListForNewData(flowJobSearchKey2, ref flowListJobs2);

            // Check if any job has been defined for the received flow data
            if ((flowListJobs.Count == 0) && (flowListJobs2.Count == 0))
            {
                return (numOfBytesToBeParsed);
            }

#if DEBUG
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseIncrementalFlow - Flow Info Number  = {0}", flowDataNumber);
#endif

            // Parse the flow calculation factor
            float flowCalculationFactor = LacbusPcProtocol.LacbusSofbusSmsParseFloatFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte);
            // double value used in calculations
            double doubleFlowCalculationValue = flowCalculationFactor;

            // Parse the initial meter value
            uint sampleValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 32);

            // Parse the initial flow increment size
            uint incrementSize = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 2);

            // Parse the initial flow increment
            bool valueParsed = false;
            uint incrementValue = 0;
            uint remainingBits = (uint)((receivedMessage.GetLength(0) - initialByte - 1) * 8 + numOfBitsInTheInitialByte);
            switch (incrementSize)
            {
                // No increment
                case 0:
                    valueParsed = true;
                    incrementValue = 0;
                    break;

                // Increment format == 6 bits
                case 1:
                    // Parse the increment value
                    if (remainingBits < 6)
                    {
                        remainingBits = 0;
                    }
                    else
                    {
                        incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 6);
                        valueParsed = true;
                        remainingBits -= 6;
                    }
                    break;

                // Increment format == 14 bits
                case 2:
                    // Parse the increment value
                    if (remainingBits < 14)
                    {
                        remainingBits = 0;
                    }
                    else
                    {
                        incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 14);
                        valueParsed = true;
                        remainingBits -= 14;
                    }
                    break;

                // Increment format == 22 bits
                case 3:
                    if (remainingBits < 22)
                    {
                        remainingBits = 0;
                    }
                    else
                    {
                        // Parse the increment value
                        incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 22);
                        remainingBits -= 22;
                        valueParsed = true;
                    }
                    break;
            }
            if (valueParsed == false)
            {
                return (0);
            }

            // Calculate the initial value of the flow rate
            double flowRate = doubleFlowCalculationValue * incrementValue;

            //// Pass the initial value of flow to the jobs
            TimeSpan deltaTime = new TimeSpan(0, archivalHeader.SAHArchivalPeriod, 0);
            DateTime sampleTime = archivalHeader.SAHInitialValueTimeStamp;
            sampleTime = ConvertTimeToUtc(sampleTime);
#if DEBUG
            System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseIncrementalFlow - Timestamp = {0}, Sample Value = {1}, Increment Format = {2}, Increment Value = {3}, Flow Factor = {4}, Flow Rate = {5}",
                                               sampleTime, sampleValue, incrementSize, incrementValue, flowCalculationFactor, flowRate);
#endif
            //// Meter Value
            //foreach (var jobVal in listJobs)
            //{
            //    LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
            //    if (lJob == null)
            //    {
            //        continue;
            //    }
            //    ExecutedJobArgs eJob = new ExecutedJobArgs();
            //    eJob.Job = lJob;
            //    eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
            //    bool jobValueSet = false;
            //    uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
            //    eJob.Values = LacbusPcProtocol.ConvertUint32ValueToByteArray((BuiltInType)tagDataType, sampleValue, ref jobValueSet);
            //    eJob.Timestamp = sampleTime;
            //    if (jobValueSet == true)
            //    {
            //        OnJobExecuted(eJob);
            //    }
            //}

            // Flow Rate
            foreach (var jobVal in flowListJobs)
            {
                LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                if (lJob == null)
                {
                    continue;
                }
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = lJob;
                eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                bool jobValueSet = false;
                uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, flowRate, ref jobValueSet);
                eJob.Timestamp = sampleTime;
                if (jobValueSet == true)
                {
                    OnJobExecuted(eJob);
                }
            }

            // Flow Rate - alternative data type: Count Input
            foreach (var jobVal in flowListJobs2)
            {
                LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                if (lJob == null)
                {
                    continue;
                }
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.Job = lJob;
                eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                bool jobValueSet = false;
                uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, flowRate, ref jobValueSet);
                eJob.Timestamp = sampleTime;
                if (jobValueSet == true)
                {
                    OnJobExecuted(eJob);
                }
            }

            // Increment the sample timestamp
            sampleTime += deltaTime;

            // Parse the rest of data
            uint numberOfParsedSamples = 1;
            remainingBits = (uint)((receivedMessage.GetLength(0) - initialByte - 1) * 8 + numOfBitsInTheInitialByte);
            while ((remainingBits >= 2) && (numberOfParsedSamples < archivalHeader.SAHNumberOfSamples))
            {
                // Parse the increment size
                incrementSize = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 2);
                remainingBits -= 2;
                valueParsed = false;
                switch (incrementSize)
                {
                    // No increment
                    case 0:
                        valueParsed = true;
                        incrementValue = 0;
                        break;

                    // Increment format == 6 bits
                    case 1:
                        if (remainingBits < 6)
                        {
                            remainingBits = 0;
                        }
                        else
                        {
                            // Parse the increment value
                            incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 6);
                            remainingBits -= 6;
                            valueParsed = true;
                        }
                        break;

                    // Increment format == 14 bits
                    case 2:
                        if (remainingBits < 14)
                        {
                            remainingBits = 0;
                        }
                        else
                        {
                            // Parse the increment value
                            incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 14);
                            remainingBits -= 14;
                            valueParsed = true;
                        }
                        break;

                    // Increment format == 22 bits
                    case 3:
                        if (remainingBits < 22)
                        {
                            remainingBits = 0;
                        }
                        else
                        {
                            // Parse the increment value
                            incrementValue = LacbusPcProtocol.LacbusSofbusSmsParseIntFromBits(receivedMessage, ref initialByte, ref numOfBitsInTheInitialByte, 22);
                            remainingBits -= 22;
                            valueParsed = true;
                        }
                        break;
                }

                if (valueParsed == true)
                {
                    numberOfParsedSamples++;

                    // Calculate the flow rate
                    flowRate = doubleFlowCalculationValue * incrementValue;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine("LacbusSofbusSmsParseIncrementalFlow - Timestamp = {0}, Sample Value = {1}, Increment Format = {2}, Increment Value = {3}, Flow Factor = {4}, Flow Rate = {5}",
                                                       sampleTime, sampleValue, incrementSize, incrementValue, flowCalculationFactor, flowRate);
#endif
                    // Pass flow data to jobs
                    foreach (var jobVal in flowListJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, flowRate, ref jobValueSet);
                        eJob.Timestamp = sampleTime;
                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }

                    // Pass flow data to jobs - alternative data type: Count Input
                    foreach (var jobVal in flowListJobs2)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, flowRate, ref jobValueSet);
                        eJob.Timestamp = sampleTime;
                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }

                    // Increment the sample timestamp
                    sampleTime += deltaTime;
                }
                else
                {
                    // Stop the parsing loop
                    remainingBits = 0;
                }
            }

            return (numOfBytesToBeParsed);
        }

        private void LacbusRtuParseDuData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, int alreadyParsedBytes, int alreadyParsedDataBytes)
        {
            int messageLength = receivedMessage.GetLength(0);
            int totalParsedBytes = alreadyParsedBytes;
            int dataParsedBytes = alreadyParsedDataBytes;

            // Parse the data blocks
            while ((totalParsedBytes < messageLength) &&
                  (dataParsedBytes < messageHeader.headerDataLength) &&
                  (dataParsedBytes < rtuHeader.DUTotalLength))
            {
                LacbusPCDataBlockHeader blockHeader = new LacbusPCDataBlockHeader();
                int parsedBytes = LacbusPcProtocol.ParseLacbusPCDataBlockHeader(receivedMessage, totalParsedBytes, ref blockHeader);
                // Invalid block?
                if (parsedBytes == 0)
                {
                    System.Diagnostics.Debug.WriteLine("LacbusRtuParseDuData: error in LacbusPcProtocol.ParseLacbusPCDataBlockHeader messageLength = {0} totalParsedBytes = {1} dataParsedBytes = {2} messageHeader.headerDataLength = {3} rtuHeader.DUTotalLength = {4}",
                                                       messageLength, totalParsedBytes, dataParsedBytes, messageHeader.headerDataLength, rtuHeader.DUTotalLength);
                    break;
                }
                // Invalid block length?
                if (blockHeader.blockBodyLength == 0)
                {
                    System.Diagnostics.Debug.WriteLine("LacbusRtuParseDuData: error Invalid block length");
                    break;
                }
                // Only 'B', 'E', 'F' blocks are parsed
                if ((blockHeader.blockType != 0x42) && (blockHeader.blockType != 0x45) && (blockHeader.blockType != 0x46))
                {
                    totalParsedBytes += parsedBytes + blockHeader.blockBodyLength;
                    dataParsedBytes += parsedBytes + blockHeader.blockBodyLength;
                    System.Diagnostics.Debug.WriteLine("LacbusRtuParseDuData: unsupported block {0} --> Skip it", blockHeader.blockType);
                    continue;
                }

                totalParsedBytes += parsedBytes;
                dataParsedBytes += parsedBytes;
                parsedBytes = LacbusRtuParseDuBlock(receivedMessage, messageHeader, rtuHeader, blockHeader, totalParsedBytes, dataParsedBytes);
                if (parsedBytes == 0)
                {
                    System.Diagnostics.Debug.WriteLine("LacbusRtuParseDuData: error in LacbusRtuParseDuBlock messageLength = {0} totalParsedBytes = {1} dataParsedBytes = {2} messageHeader.headerDataLength = {3} rtuHeader.DUTotalLength = {4} blockHeader.blockBodyLength = {5}",
                                                      messageLength, totalParsedBytes, dataParsedBytes, messageHeader.headerDataLength, rtuHeader.DUTotalLength, blockHeader.blockBodyLength);
                    return;
                }
                totalParsedBytes += parsedBytes;
                dataParsedBytes += parsedBytes;
            }
        }

        private int LacbusRtuParseDuBlock(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, int alreadyParsedDataBytes)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            System.Diagnostics.Debug.WriteLine("LacbusRtuParseDuBlock - totalMessageLength = {0} alreadyParsedBytes = {1} blockHeader.blockBodyLength = {2} messageHeader.headerDataLength = {3} alreadyParsedDataBytes = {4}",
                                               totalMessageLength, alreadyParsedBytes, blockHeader.blockBodyLength, messageHeader.headerDataLength, alreadyParsedDataBytes);
            if ((totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength)) ||
                (messageHeader.headerDataLength < (alreadyParsedDataBytes + blockHeader.blockBodyLength)))
            {
                System.Diagnostics.Debug.WriteLine("LacbusRtuParseDuBlock Warning: incorrect message sizes");
                return (0);
            }

            switch (blockHeader.blockType)
            {
                case 0x42: // 'B' block
                    System.Diagnostics.Debug.WriteLine("LacbusRtuParseDuBlock: parsing B block");
                    return (LacbusRtuParseDuBBlock(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes));

                case 0x45: // 'E' block
                    System.Diagnostics.Debug.WriteLine("LacbusRtuParseDuBlock: parsing E block");
                    return (LacbusRtuParseDuEBlock(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes));

                case 0x46: // 'F' block
                    System.Diagnostics.Debug.WriteLine("LacbusRtuParseDuBlock: parsing F block");
                    return (LacbusRtuParseDuFBlock(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes));

                default:
                    return (0);
            }
        }

        private int LacbusRtuParseDuFBlock(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes)
        {
            // Parse the date and time of the historical data
            // Parse the date and time
            if (blockHeader.blockBodyLength < 6)
            {
                return (0);
            }

            int day = receivedMessage[alreadyParsedBytes];
            int month = receivedMessage[alreadyParsedBytes + 1];
            int year = receivedMessage[alreadyParsedBytes + 2];
            year += 2000;
            int hour = receivedMessage[alreadyParsedBytes + 3];
            int minute = receivedMessage[alreadyParsedBytes + 4];
            int second = receivedMessage[alreadyParsedBytes + 5];
            DateTime receivedTime = new DateTime(year, month, day, hour, minute, second);
            receivedTime = ConvertTimeToUtc(receivedTime);

            System.Diagnostics.Debug.WriteLine("LacbusRtuParseDuFBlock: Timestamp = {0}-{1}-{2} {3}:{4}:{5}",
                                               receivedTime.Day, receivedTime.Month, receivedTime.Year, receivedTime.Hour, receivedTime.Minute, receivedTime.Second);

            int parsedBytes = 6;

            while (parsedBytes < (blockHeader.blockBodyLength))
            {

                int dataParsed = ParseLacbusRtuSequenceOfAlarmLogs(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, receivedTime);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }
            return (parsedBytes);
        }

        private int ParseLacbusRtuSequenceOfAlarmLogs(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, DateTime hTime)
        {
            // Check the message length
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength < (alreadyParsedBytes + 4))
            {
                return (0);
            }

            // Get the alarm type
            ushort alarmType = receivedMessage[alreadyParsedBytes];
            alarmType <<= 8;
            alarmType += receivedMessage[alreadyParsedBytes + 1];

            // Get the characterization
            ushort alarmCharacterization = receivedMessage[alreadyParsedBytes + 2];
            alarmCharacterization <<= 8;
            alarmCharacterization += receivedMessage[alreadyParsedBytes + 3];

            int parsedBytes = 4;

            System.Diagnostics.Debug.WriteLine("ParseLacbusRtuSequenceOfAlarmLogs: {0}-{1}-{2} {3}:{4}:{5} received alarm {6}, value = {7}",
                                               hTime.Day, hTime.Month, hTime.Year, hTime.Hour, hTime.Minute, hTime.Second, alarmType, alarmCharacterization);

            // Pass the received data to jobs
            UInt32 alarmInfo = alarmCharacterization;
            alarmInfo <<= 16;
            alarmInfo += alarmType;
            DatumTypes newDataType = DatumTypes.Alarm;
            DatumCategories newDataCategory = DatumCategories.Historical;
            UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, 0, (byte)newDataType, (byte)newDataCategory);
            List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
            GetJobListForNewData(jobSearchKey, ref listJobs);
            if (listJobs.Count > 0)
            {
                bool jobValueSet = false;
                byte[] receivedValueBuffer = LacbusPcProtocol.ConvertUint32ValueToByteArray(BuiltInType.UInt32, alarmInfo, ref jobValueSet);
                if (jobValueSet == true)
                {

                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        uint arrayDim  = lJob.TotalJobSize;
                        if(arrayDim > 0)
                        {
                            if (arrayDim > 4)
                            {
                                arrayDim = 4;
                            }
                            byte[] valueBuffer = new byte[arrayDim];
                            Array.Copy(receivedValueBuffer, valueBuffer, arrayDim);
                            eJob.Values = valueBuffer;
                            eJob.Timestamp = hTime;
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            // Logical inputs coming from alarms
            if((alarmType == 1) || (alarmType == 2))
            {
                newDataType = DatumTypes.DigitalInput;
                jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, alarmCharacterization, (byte)newDataType, (byte)newDataCategory);
                //listJobs.Clear();
                List<LacbusPCCommJob> listJobs2 = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs2);
                if (listJobs2.Count > 0)
                {
                    bool logicalValue = false;
                    if(alarmType == 1)
                    {
                        logicalValue = true;
                    }
 
                    // Process the received data
                    foreach (var jobVal in listJobs2)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        eJob.Values = LacbusPcProtocol.ConvertBoolValueToByteArray((BuiltInType)tagDataType, logicalValue, ref jobValueSet);
                        eJob.Timestamp = hTime;
                        if (jobValueSet == true)
                        {
                            System.Diagnostics.Debug.WriteLine("ParseLacbusRtuSequenceOfAlarmLogs: set DI {0} to value = {1}",
                                                               alarmCharacterization, logicalValue);
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            return (parsedBytes);
        }

        private int LacbusRtuParseDuEBlock(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes)
        {
            // Parse the date and time of the historical data
            // Parse the date and time
            if (blockHeader.blockBodyLength < 6)
            {
                return (0);
            }

            int day = receivedMessage[alreadyParsedBytes];
            int month = receivedMessage[alreadyParsedBytes + 1];
            int year = receivedMessage[alreadyParsedBytes + 2];
            year += 2000;
            int hour = receivedMessage[alreadyParsedBytes + 3];
            int minute = receivedMessage[alreadyParsedBytes + 4];
            int second = receivedMessage[alreadyParsedBytes + 5];
            DateTime receivedTime = new DateTime(year, month, day, hour, minute, second);
            receivedTime = ConvertTimeToUtc(receivedTime);

            System.Diagnostics.Debug.WriteLine("LacbusRtuParseDuEBlock: Timestamp = {0}-{1}-{2} {3}:{4}:{5}",
                                               receivedTime.Day, receivedTime.Month, receivedTime.Year, receivedTime.Hour, receivedTime.Minute, receivedTime.Second);

            int parsedBytes = 6;

            while (parsedBytes < (blockHeader.blockBodyLength))
            {

                int dataParsed = ParseLacbusRtuSequenceOfDataNumFormatValueWithTime(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes, receivedTime);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }
            return (parsedBytes);
        }

        private int ParseLacbusRtuSequenceOfDataNumFormatValueWithTime(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes, DateTime hTime)
        {
            LacbusPCSequenceElementDatumNumFormatValueLock sequenceElement = new LacbusPCSequenceElementDatumNumFormatValueLock();
            int parsedBytes = LacbusPcProtocol.ParseLacbusRTUSequenceElementDatumNumFormatValue(receivedMessage, alreadyParsedBytes, ref sequenceElement);

            DatumTypes newDataType = DatumTypes.DigitalInput;
            DatumCategories newDataCategory = DatumCategories.Historical;
            if (parsedBytes > 0)
            {
                switch (sequenceElement.datumFormat)
                {
                    case 1:
                        System.Diagnostics.Debug.WriteLine("ParseLacbusRtuSequenceOfDataNumFormatValueWithTime: received  seq num {0} form {1} val {2} IsOut {3} Lock {4}",
                            sequenceElement.datumNum, sequenceElement.datumFormat, sequenceElement.logicalValue, sequenceElement.isOutput, sequenceElement.lockStatus);
                        if (sequenceElement.isOutput == false)
                        {
                            newDataType = DatumTypes.DigitalInput;
                        }
                        else
                        {
                            newDataType = DatumTypes.DigitalOutput;
                        }
                        break;
                    case 2:
                        System.Diagnostics.Debug.WriteLine("ParseLacbusRtuSequenceOfDataNumFormatValueWithTime: received  seq num {0} form {1} val {2} IsOut {3} Lock {4}",
                            sequenceElement.datumNum, sequenceElement.datumFormat, sequenceElement.doubleValue, sequenceElement.isOutput, sequenceElement.lockStatus);
                        if (sequenceElement.isOutput == false)
                        {
                            newDataType = DatumTypes.AnalogInput;
                        }
                        else
                        {
                            newDataType = DatumTypes.AnalogOutput;
                        }
                        break;
                    case 3:
                        System.Diagnostics.Debug.WriteLine("ParseLacbusRtuSequenceOfDataNumFormatValueWithTime: received  seq num {0} form {1} val {2} IsOut {3} Lock {4}",
                            sequenceElement.datumNum, sequenceElement.datumFormat, sequenceElement.floatValue, sequenceElement.isOutput, sequenceElement.lockStatus);
                        if (sequenceElement.isOutput == false)
                        {
                            newDataType = DatumTypes.AnalogInput;
                        }
                        else
                        {
                            newDataType = DatumTypes.AnalogOutput;
                        }
                        break;
                }

                // Pass received data to jobs
                UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, sequenceElement.datumNum, (byte)newDataType, (byte)newDataCategory);
                List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs);
                if (listJobs.Count > 0)
                {
                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        switch (sequenceElement.datumFormat)
                        {
                            case 1: // Boolean value
                                eJob.Values = LacbusPcProtocol.ConvertBoolValueToByteArray((BuiltInType)tagDataType, sequenceElement.logicalValue, ref jobValueSet);
                                eJob.Timestamp = hTime;
                                break;

                            case 2: // Double value
                                eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, sequenceElement.doubleValue, ref jobValueSet);
                                eJob.Timestamp = hTime;
                                break;

                            case 3: // Float value
                                eJob.Values = LacbusPcProtocol.ConvertFloatValueToByteArray((BuiltInType)tagDataType, sequenceElement.floatValue, ref jobValueSet);
                                eJob.Timestamp = hTime;
                                break;
                        }

                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            return (parsedBytes);
        }

        private int LacbusRtuParseDuBBlock(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes)
        {
            int parsedBytes = 0;
            while (parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = ParseSequenceOfDataNumFormatValueLock(receivedMessage, messageHeader, rtuHeader, blockHeader, alreadyParsedBytes + parsedBytes);
                // Invalid data sequence?
                if (dataParsed == 0)
                {
                    System.Diagnostics.Debug.WriteLine("LacbusRtuParseDuBBlock: error in ParseSequenceOfDataNumFormatValueLock");
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }
            return (parsedBytes);
        }

        private int ParseSequenceOfDataNumFormatValueLock(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusRtuDataUnitHeader rtuHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes)
        {
            LacbusPCSequenceElementDatumNumFormatValueLock sequenceElement = new LacbusPCSequenceElementDatumNumFormatValueLock();
            int parsedBytes = LacbusPcProtocol.ParseLacbusPCSequenceElementDatumNumFormatValueLock(receivedMessage, alreadyParsedBytes, ref sequenceElement);

            DatumTypes newDataType = DatumTypes.DigitalInput;
            DatumCategories newDataCategory = DatumCategories.Instantaneous;
            if (parsedBytes > 0)
            {
                System.Diagnostics.Debug.WriteLine("ParseSequenceOfDataNumFormatValueLock: parsing data of type {0}", sequenceElement.datumFormat);
                switch (sequenceElement.datumFormat)
                {
                    case 1:
                        System.Diagnostics.Debug.WriteLine("ParseSequenceOfDataNumFormatValueLock: received  seq num {0} form {1} val {2} IsOut {3} Lock {4}",
                            sequenceElement.datumNum, sequenceElement.datumFormat, sequenceElement.logicalValue, sequenceElement.isOutput, sequenceElement.lockStatus);
                        if (sequenceElement.isOutput == false)
                        {
                            newDataType = DatumTypes.DigitalInput;
                        }
                        else
                        {
                            newDataType = DatumTypes.DigitalOutput;
                        }
                        break;
                    case 2:
                        System.Diagnostics.Debug.WriteLine("ParseSequenceOfDataNumFormatValueLock: received  seq num {0} form {1} val {2} IsOut {3} Lock {4}",
                            sequenceElement.datumNum, sequenceElement.datumFormat, sequenceElement.doubleValue, sequenceElement.isOutput, sequenceElement.lockStatus);
                        if (sequenceElement.isOutput == false)
                        {
                            newDataType = DatumTypes.AnalogInput;
                        }
                        else
                        {
                            newDataType = DatumTypes.AnalogOutput;
                        }
                        break;
                    case 3:
                        System.Diagnostics.Debug.WriteLine("ParseSequenceOfDataNumFormatValueLock: received  seq num {0} form {1} val {2} IsOut {3} Lock {4}",
                            sequenceElement.datumNum, sequenceElement.datumFormat, sequenceElement.floatValue, sequenceElement.isOutput, sequenceElement.lockStatus);
                        if (sequenceElement.isOutput == false)
                        {
                            newDataType = DatumTypes.AnalogInput;
                        }
                        else
                        {
                            newDataType = DatumTypes.AnalogOutput;
                        }
                        break;
                }

                // Pass received data to jobs
                UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(rtuHeader.DURtuNumber, sequenceElement.datumNum, (byte)newDataType, (byte)newDataCategory);
                List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs);
                if (listJobs.Count > 0)
                {
                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        switch (sequenceElement.datumFormat)
                        {
                            case 1: // Boolean value
                                if(newDataType != DatumTypes.DigitalOutput)
                                {
                                    eJob.Values = LacbusPcProtocol.ConvertBoolValueToByteArray((BuiltInType)tagDataType, sequenceElement.logicalValue, ref jobValueSet);
                                }
                                else
                                {
                                    byte lockStatus = 0;
                                    if((sequenceElement.lockStatus == 2) || (sequenceElement.lockStatus == 3))
                                    {
                                        lockStatus = 1;
                                    }
                                    eJob.Values = LacbusPcProtocol.ConvertBoolValueWithLockStatusToByteArray((BuiltInType)tagDataType, sequenceElement.logicalValue, lockStatus, ref jobValueSet);
                                }
                                break;

                            case 2: // Double value
                                if (newDataType != DatumTypes.AnalogOutput)
                                {
                                    eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, sequenceElement.doubleValue, ref jobValueSet);
                                }
                                else
                                {
                                    byte lockStatus = 0;
                                    if ((sequenceElement.lockStatus == 2) || (sequenceElement.lockStatus == 3))
                                    {
                                        lockStatus = 1;
                                    }
                                    eJob.Values = LacbusPcProtocol.ConvertDoubleValueWithLockStatusToByteArray((BuiltInType)tagDataType, sequenceElement.doubleValue, lockStatus, ref jobValueSet);
                                }
                                break;

                            case 3: // Float value
                                if (newDataType != DatumTypes.AnalogOutput)
                                {
                                    eJob.Values = LacbusPcProtocol.ConvertFloatValueToByteArray((BuiltInType)tagDataType, sequenceElement.floatValue, ref jobValueSet);
                                }
                                else
                                {
                                    byte lockStatus = 0;
                                    if ((sequenceElement.lockStatus == 2) || (sequenceElement.lockStatus == 3))
                                    {
                                        lockStatus = 1;
                                    }
                                    eJob.Values = LacbusPcProtocol.ConvertFloatValueWithLockStatusToByteArray((BuiltInType)tagDataType, sequenceElement.floatValue, lockStatus, ref jobValueSet);
                                }
                                break;
                        }

                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            return (parsedBytes);
        }

        private void ParseEMessageData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader)
        {
            // Protocol LACBUS RTU
            if (messageHeader.headerUnderlyingProtocol == LacbusPcUnderlyingProtocols.LacbusRTU)
            {
                // Data are organized in a sequence of blocks
                int messageLength = receivedMessage.GetLength(0);
                int totalParsedBytes = LacbusPcProtocol.LacbusPCMessageHeaderLength;
                int dataParsedBytes = 0;
                while ((totalParsedBytes < messageLength) && (dataParsedBytes < messageHeader.headerDataLength))
                {
                    int parsedBytes = LacbusRtuParseSpontaneousDataBlock(receivedMessage, messageHeader, totalParsedBytes);
                    // Invalid block?
                    if (parsedBytes == 0)
                    {
                        break;
                    }

                    totalParsedBytes += parsedBytes;
                    dataParsedBytes += parsedBytes;
                }
            }

            // Protocol SOFBUS PL
            else if (messageHeader.headerUnderlyingProtocol == LacbusPcUnderlyingProtocols.SofbusPL)
            {
                // Data are organized in a sequence of blocks
                int messageLength = receivedMessage.GetLength(0);
                int totalParsedBytes = LacbusPcProtocol.LacbusPCMessageHeaderLength;
                int dataParsedBytes = 0;
                while ((totalParsedBytes < messageLength) && (dataParsedBytes < messageHeader.headerDataLength))
                {
                    int parsedBytes = SofbusPlParseSpontaneousDataBlock(receivedMessage, messageHeader, totalParsedBytes);
                    // Invalid block?
                    if (parsedBytes == 0)
                    {
                        break;
                    }

                    totalParsedBytes += parsedBytes;
                    dataParsedBytes += parsedBytes;
                }
            }
        }

        private int SofbusPlParseSpontaneousDataBlock(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, int alreadyParsedBytes)
        {
            // Check the block length
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength < (alreadyParsedBytes + 4))
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseSpontaneousDataBlock: message too short for the next block");
                return (0);
            }

            // Get the type of the data block
            byte blockType = receivedMessage[alreadyParsedBytes];

            // Get number of data
            byte numOfData = receivedMessage[alreadyParsedBytes + 1];

            // Get the base address
            ushort baseAddress = receivedMessage[alreadyParsedBytes + 2];
            baseAddress <<= 8;
            baseAddress += receivedMessage[alreadyParsedBytes + 3];

            // Calculate the number of data bytes
            int numOfDataBytes = 0;
            switch(blockType)
            {
                case 0x53: // 'S'
                case 0x43: // 'C'
                    numOfDataBytes = numOfData / 8;
                    if((numOfData % 8) > 0)
                    {
                        numOfDataBytes++;
                    }
                    break;

                case 0x4d: // 'M'
                case 0x52: // 'R'
                    numOfDataBytes = numOfData * 2;
                    break;

                case 0x4c: // 'L'
                    numOfDataBytes = numOfData * 4;
                    break;
            }

            if(numOfDataBytes == 0)
            {
                System.Diagnostics.Debug.WriteLine("SofbusPlParseSpontaneousDataBlock: invalid block type {0}", receivedMessage[alreadyParsedBytes]);
                return (0);
            }

            int datumNumber = (int)baseAddress;
            int parsedBytes = 4;
            int maxDatumNum = numOfData + baseAddress - 1;
            while ((totalMessageLength > (alreadyParsedBytes + parsedBytes)) && (parsedBytes < (numOfDataBytes + 4)) && (datumNumber <= maxDatumNum)) 
            {
                int parsedInfoBytes = SofbusPlParseSpontaneousDataInfo(receivedMessage, messageHeader, alreadyParsedBytes + parsedBytes, datumNumber, maxDatumNum, blockType);
                if (parsedInfoBytes == 0)
                {
                    parsedBytes = 0;
                    break;
                }
                if((blockType == 0x53) || (blockType == 0x43))
                {
                    datumNumber += 8;
                }
                else
                {
                    datumNumber++;
                }
                parsedBytes += parsedInfoBytes;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSpontaneousDataInfo(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, int alreadyParsedBytes, int datumNumber, int maxDatumNum, byte blockType)
        {
            int parsedBytes = 0;
            switch (blockType)
            {
                case 0x53: // 'S'
                    parsedBytes = SofbusPlParseSpontaneousDataInfoS(receivedMessage, messageHeader, alreadyParsedBytes, datumNumber, maxDatumNum);
                    break;
                case 0x43: // 'C'
                    parsedBytes = SofbusPlParseSpontaneousDataInfoC(receivedMessage, messageHeader, alreadyParsedBytes, datumNumber, maxDatumNum);
                    break;
                case 0x4d: // 'M'
                    parsedBytes = SofbusPlParseSpontaneousDataInfoM(receivedMessage, messageHeader, alreadyParsedBytes, datumNumber, maxDatumNum);
                    break;
                case 0x52: // 'R'
                    parsedBytes = SofbusPlParseSpontaneousDataInfoR(receivedMessage, messageHeader, alreadyParsedBytes, datumNumber, maxDatumNum);
                    break;
                case 0x4c: // 'L'
                    parsedBytes = SofbusPlParseSpontaneousDataInfoL(receivedMessage, messageHeader, alreadyParsedBytes, datumNumber, maxDatumNum);
                    break;
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSpontaneousDataInfoS(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, int alreadyParsedBytes, int datumNumber, int maxDatumNum)
        {
            int parsedBytes = 0;
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength >= (alreadyParsedBytes + 1))
            {
                bool[] bitArray = new bool[8];
                int i = 0;
                byte bitMask = 0x80;
                for (i = 7, bitMask = 0x80; i >= 0; i--, bitMask >>= 1)
                {
                    if ((receivedMessage[alreadyParsedBytes] & bitMask) == 0)
                    {
                        bitArray[i] = false;
                    }
                    else
                    {
                        bitArray[i] = true;
                    }
                }
                parsedBytes = 1;

                // Pass received data to jobs
                DatumTypes newDataType = DatumTypes.DigitalInput;
                DatumCategories newDataCategory = DatumCategories.Instantaneous;
                int nextDataNumber = datumNumber;
                for (i = 0, nextDataNumber = datumNumber; (i < 8) && (nextDataNumber <= maxDatumNum); i++, nextDataNumber++)
                {
                    UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)nextDataNumber, (byte)newDataType, (byte)newDataCategory);
                    List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                    GetJobListForNewData(jobSearchKey, ref listJobs);
                    if (listJobs.Count > 0)
                    {
                        foreach (var jobVal in listJobs)
                        {
                            LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                            if (lJob == null)
                            {
                                continue;
                            }
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.Job = lJob;
                            eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                            eJob.Values = BitConverter.GetBytes(bitArray[i]);
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSpontaneousDataInfoC(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, int alreadyParsedBytes, int datumNumber, int maxDatumNum)
        {
            int parsedBytes = 0;
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength >= (alreadyParsedBytes + 1))
            {
                bool[] bitArray = new bool[8];
                int i = 0;
                byte bitMask = 0x80;
                for (i = 7, bitMask = 0x80; i >= 0; i--, bitMask >>= 1)
                {
                    if ((receivedMessage[alreadyParsedBytes] & bitMask) == 0)
                    {
                        bitArray[i] = false;
                    }
                    else
                    {
                        bitArray[i] = true;
                    }
                }
                parsedBytes = 1;

                // Pass received data to jobs
                DatumTypes newDataType = DatumTypes.DigitalOutput;
                DatumCategories newDataCategory = DatumCategories.Instantaneous;
                int nextDataNumber = datumNumber;
                for (i = 0, nextDataNumber = datumNumber; (i < 8) && (nextDataNumber <= maxDatumNum); i++, nextDataNumber++)
                {
                    UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)nextDataNumber, (byte)newDataType, (byte)newDataCategory);
                    List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                    GetJobListForNewData(jobSearchKey, ref listJobs);
                    if (listJobs.Count > 0)
                    {
                        foreach (var jobVal in listJobs)
                        {
                            LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                            if (lJob == null)
                            {
                                continue;
                            }
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.Job = lJob;
                            eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                            eJob.Values = BitConverter.GetBytes(bitArray[i]);
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSpontaneousDataInfoM(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, int alreadyParsedBytes, int datumNumber, int maxDatumNum)
        {
            int parsedBytes = 0;
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength >= (alreadyParsedBytes + 2))
            {
                byte[] auxBuffer = new byte[2];
                for (int i = 0; i < 2; i++)
                {
                    auxBuffer[1 - i] = receivedMessage[alreadyParsedBytes + i];
                }
                short aiValue = BitConverter.ToInt16(auxBuffer, 0);
                parsedBytes = 2;

                // Pass received data to jobs
                DatumTypes newDataType = DatumTypes.AnalogInput;
                DatumCategories newDataCategory = DatumCategories.Instantaneous;
                UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)datumNumber, (byte)newDataType, (byte)newDataCategory);
                List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs);
                if (listJobs.Count > 0)
                {
                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        switch (tagDataType)
                        {
                            case (uint)BuiltInType.Byte:
                                {
                                    byte[] auxByteArray = new byte[1];
                                    auxByteArray[0] = (byte)aiValue;
                                    eJob.Values = auxByteArray;
                                }
                                break;
                            case (uint)BuiltInType.Int16:
                                {
                                    eJob.Values = BitConverter.GetBytes(aiValue);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.UInt16:
                                {
                                    ushort auxUshort = (ushort)aiValue;
                                    eJob.Values = BitConverter.GetBytes(auxUshort);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.Int32:
                                {
                                    int auxInt = (int)aiValue;
                                    eJob.Values = BitConverter.GetBytes(auxInt);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.UInt32:
                                {
                                    uint auxUInt = (uint)aiValue;
                                    eJob.Values = BitConverter.GetBytes(auxUInt);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.Int64:
                                {
                                    Int64 auxInt64 = (Int64)aiValue;
                                    eJob.Values = BitConverter.GetBytes(auxInt64);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.UInt64:
                                {
                                    UInt64 auxUInt64 = (UInt64)aiValue;
                                    eJob.Values = BitConverter.GetBytes(auxUInt64);
                                    jobValueSet = true;
                                }
                                break;
                        }

                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSpontaneousDataInfoR(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, int alreadyParsedBytes, int datumNumber, int maxDatumNum)
        {
            int parsedBytes = 0;
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength >= (alreadyParsedBytes + 2))
            {
                byte[] auxBuffer = new byte[2];
                for (int i = 0; i < 2; i++)
                {
                    auxBuffer[1 - i] = receivedMessage[alreadyParsedBytes + i];
                }
                short aiValue = BitConverter.ToInt16(auxBuffer, 0);
                parsedBytes = 2;

                // Pass received data to jobs
                DatumTypes newDataType = DatumTypes.AnalogOutput;
                DatumCategories newDataCategory = DatumCategories.Instantaneous;
                UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)datumNumber, (byte)newDataType, (byte)newDataCategory);
                List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs);
                if (listJobs.Count > 0)
                {
                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        switch (tagDataType)
                        {
                            case (uint)BuiltInType.Byte:
                                {
                                    byte[] auxByteArray = new byte[1];
                                    auxByteArray[0] = (byte)aiValue;
                                    eJob.Values = auxByteArray;
                                }
                                break;
                            case (uint)BuiltInType.Int16:
                                {
                                    eJob.Values = BitConverter.GetBytes(aiValue);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.UInt16:
                                {
                                    ushort auxUshort = (ushort)aiValue;
                                    eJob.Values = BitConverter.GetBytes(auxUshort);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.Int32:
                                {
                                    int auxInt = (int)aiValue;
                                    eJob.Values = BitConverter.GetBytes(auxInt);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.UInt32:
                                {
                                    uint auxUInt = (uint)aiValue;
                                    eJob.Values = BitConverter.GetBytes(auxUInt);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.Int64:
                                {
                                    Int64 auxInt64 = (Int64)aiValue;
                                    eJob.Values = BitConverter.GetBytes(auxInt64);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.UInt64:
                                {
                                    UInt64 auxUInt64 = (UInt64)aiValue;
                                    eJob.Values = BitConverter.GetBytes(auxUInt64);
                                    jobValueSet = true;
                                }
                                break;
                        }

                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            return (parsedBytes);
        }

        private int SofbusPlParseSpontaneousDataInfoL(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, int alreadyParsedBytes, int datumNumber, int maxDatumNum)
        {
            int parsedBytes = 0;
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength >= (alreadyParsedBytes + 4))
            {
                byte[] auxBuffer = new byte[4];
                for (int i = 0; i < 4; i++)
                {
                    auxBuffer[3 - i] = receivedMessage[alreadyParsedBytes + i];
                }
                uint ciValue = BitConverter.ToUInt32(auxBuffer, 0);
                parsedBytes = 4;

                // Pass received data to jobs
                DatumTypes newDataType = DatumTypes.CountInput;
                DatumCategories newDataCategory = DatumCategories.Instantaneous;
                UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)datumNumber, (byte)newDataType, (byte)newDataCategory);
                List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs);
                if (listJobs.Count > 0)
                {
                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if (lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        switch (tagDataType)
                        {
                            case (uint)BuiltInType.Byte:
                                {
                                    byte[] auxByteArray = new byte[1];
                                    auxByteArray[0] = (byte)ciValue;
                                    eJob.Values = auxByteArray;
                                }
                                break;
                            case (uint)BuiltInType.Int16:
                                {
                                    short auxShort = (short)ciValue;
                                    eJob.Values = BitConverter.GetBytes(auxShort);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.UInt16:
                                {
                                    ushort auxUshort = (ushort)ciValue;
                                    eJob.Values = BitConverter.GetBytes(auxUshort);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.Int32:
                                {
                                    int auxInt = (int)ciValue;
                                    eJob.Values = BitConverter.GetBytes(auxInt);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.UInt32:
                                {
                                    eJob.Values = BitConverter.GetBytes(ciValue);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.Int64:
                                {
                                    Int64 auxInt64 = (Int64)ciValue;
                                    eJob.Values = BitConverter.GetBytes(auxInt64);
                                    jobValueSet = true;
                                }
                                break;

                            case (uint)BuiltInType.UInt64:
                                {
                                    UInt64 auxUInt64 = (UInt64)ciValue;
                                    eJob.Values = BitConverter.GetBytes(auxUInt64);
                                    jobValueSet = true;
                                }
                                break;
                        }

                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                }
            }

            return (parsedBytes);
        }

        private int LacbusRtuParseSpontaneousDataBlock(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, int alreadyParsedBytes)
        {
            // Check the block length
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength < (alreadyParsedBytes + 4))
            {
                System.Diagnostics.Debug.WriteLine("LacbusRtuParseSpontaneousDataBlock: message too short for the next block");
                return (0);
            }

            // Check the begin of the data block: it must be 'A'
            if(receivedMessage[alreadyParsedBytes] != 0x41)
            {
                System.Diagnostics.Debug.WriteLine("LacbusRtuParseSpontaneousDataBlock: invalid block type {0}", receivedMessage[alreadyParsedBytes]);
                return (0);
            }

            // Get the number of consecutive data points
            int consecutiveInfoNumber = receivedMessage[alreadyParsedBytes + 1];

            // Get the number of the first datum
            ushort numberOfFirstDatum = receivedMessage[alreadyParsedBytes + 2];
            numberOfFirstDatum <<= 8;
            numberOfFirstDatum += receivedMessage[alreadyParsedBytes + 3];

            int datumNumber = numberOfFirstDatum;
            int parsedBytes = 4;
            for (int i=0; (i<consecutiveInfoNumber) && (totalMessageLength > (alreadyParsedBytes + parsedBytes)); i++, datumNumber++)
            {
                int parsedInfoBytes = LacbusRtuParseSpontaneousDataInfo(receivedMessage, messageHeader, alreadyParsedBytes + parsedBytes, datumNumber);
                if(parsedInfoBytes == 0)
                {
                    parsedBytes = 0;
                    break;
                }
                parsedBytes += parsedInfoBytes;
            }

            return (parsedBytes);
        }

        private int LacbusRtuParseSpontaneousDataInfo(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, int alreadyParsedBytes, int datumNumber)
        {
            // Check the data length
            int totalMessageLength = receivedMessage.GetLength(0);
            if (totalMessageLength < (alreadyParsedBytes + 1))
            {
                System.Diagnostics.Debug.WriteLine("LacbusRtuParseSpontaneousDataInfo: message too short for the next info");
                return (0);
            }

            // Get the data type
            byte dataType = receivedMessage[alreadyParsedBytes];

            // Check the data type
            int expectedDataLength = 0;
            switch(dataType)
            {
                case 2: // DI
                case 3: // DO
                    expectedDataLength = 1;
                    break;

                case 4: // AI 64 bit
                case 5: // AO 64 bit
                    expectedDataLength = 8;
                    break;

                case 6: // AI 32 bit
                case 7: // AO 32 bit
                    expectedDataLength = 4;
                    break;
            }
            if(expectedDataLength == 0)
            {
                System.Diagnostics.Debug.WriteLine("LacbusRtuParseSpontaneousDataInfo: invalid data type {0}", dataType);
                return (0);
            }

            // Check again the data length
            if (totalMessageLength < (alreadyParsedBytes + 1 + expectedDataLength))
            {
                System.Diagnostics.Debug.WriteLine("LacbusRtuParseSpontaneousDataInfo: message too short for the next info 2nd check");
                return (0);
            }

            // Parse data
            bool logicalValue = false;
            double doubleValue = 0.0;
            float floatValue = 0.0F;
            switch(dataType)
            {
                case 2: // DI
                case 3: // DO
                    if (receivedMessage[alreadyParsedBytes + 1] == 0)
                    {
                        logicalValue = false;
                    }
                    else
                    {
                        logicalValue = true;
                    }
                    break;

                case 4: // AI 64 bit
                case 5: // AO 64 bit
                    {
                        byte[] auxBuffer = new byte[8];
                        for (int i = 0, j = 8; i < 8; i++, j--)
                        {
                            auxBuffer[i] = receivedMessage[alreadyParsedBytes + j];
                        }
                        doubleValue = BitConverter.ToDouble(auxBuffer, 0);
                    }
                    break;

                case 6: // AI 32 bit
                case 7: // AO 32 bit
                    {
                        byte[] auxBuffer = new byte[4];
                        for (int i = 0, j = 4; i < 4; i++, j--)
                        {
                            auxBuffer[i] = receivedMessage[alreadyParsedBytes + j];
                        }
                        floatValue = BitConverter.ToSingle(auxBuffer, 0);
                    }
                    break;
            }

            // Pass received data to jobs
            DatumTypes newDataType = DatumTypes.DigitalInput;
            switch (dataType)
            {
                case 2: // DI
                    newDataType = DatumTypes.DigitalInput;
                    break;

                case 3: // DO
                    newDataType = DatumTypes.DigitalOutput;
                    break;

                case 4:
                    newDataType = DatumTypes.AnalogInput;
                    break;

                case 5:
                    newDataType = DatumTypes.AnalogOutput;
                    break;

                case 6:
                    newDataType = DatumTypes.AnalogInput;
                    break;

                case 7:
                    newDataType = DatumTypes.AnalogOutput;
                    break;
            }

            DatumCategories newDataCategory = DatumCategories.Instantaneous;
            UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, (ushort)datumNumber, (byte)newDataType, (byte)newDataCategory);
            List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
            GetJobListForNewData(jobSearchKey, ref listJobs);
            if (listJobs.Count > 0)
            {
                // Process the received data
                foreach (var jobVal in listJobs)
                {
                    LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                    if (lJob == null)
                    {
                        continue;
                    }
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.Job = lJob;
                    eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                    bool jobValueSet = false;
                    uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                    switch (dataType)
                    {
                        case 2: // DI
                        case 3: // DO
                            eJob.Values = LacbusPcProtocol.ConvertBoolValueToByteArray((BuiltInType)tagDataType, logicalValue, ref jobValueSet);
                            break;

                        case 4: // AI 64 bit
                        case 5: // AO 64 bit
                            eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, doubleValue, ref jobValueSet);
                            break;

                        case 6: // AI 32 bit
                        case 7: // AO 32 bit
                            eJob.Values = LacbusPcProtocol.ConvertFloatValueToByteArray((BuiltInType)tagDataType, floatValue, ref jobValueSet);
                            break;
                    }

                    if (jobValueSet == true)
                    {
                        OnJobExecuted(eJob);
                    }
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("LacbusRtuParseSpontaneousDataInfo: job not found for RTU {0} type {1} num {2}", messageHeader.headerRtuNumber, datumNumber, newDataType);
            }

            return (expectedDataLength + 1);
        }

        private void ParseIMessageData(byte[] receivedMessage, LacbusPCMessageHeader messageHeader)
        {
            // Check the underlying protocol coherence
            if(messageHeader.headerUnderlyingProtocol != LacbusPcUnderlyingProtocols.LacbusPC)
            {
                System.Diagnostics.Debug.WriteLine("ParseIMessageData: received I with invalid undelying protocol {0}", messageHeader.headerUnderlyingProtocol);
                return;
            }

            // Parse the 'B' blocks
            int messageLength = receivedMessage.GetLength(0);
            int totalParsedBytes = LacbusPcProtocol.LacbusPCMessageHeaderLength;
            int dataParsedBytes = 0;
            while((totalParsedBytes < messageLength) && (dataParsedBytes < messageHeader.headerDataLength))
            {
                LacbusPCDataBlockHeader blockHeader = new LacbusPCDataBlockHeader();
                int parsedBytes = LacbusPcProtocol.ParseLacbusPCDataBlockHeader(receivedMessage, totalParsedBytes, ref blockHeader);
                // Invalid block?
                if(parsedBytes == 0)
                {
                    break;
                }
                // Invalid block length?
                if (blockHeader.blockBodyLength == 0)
                {
                    break;
                }
                // Only 'B' blocks are parsed
                if(blockHeader.blockType != 0x42)
                {
                    totalParsedBytes += parsedBytes + blockHeader.blockBodyLength;
                    dataParsedBytes += parsedBytes + blockHeader.blockBodyLength;
                    continue;
                }

                totalParsedBytes += parsedBytes;
                dataParsedBytes += parsedBytes;

                parsedBytes = ParseDataBlock(receivedMessage, messageHeader, blockHeader, totalParsedBytes);
                // Invalid block body?
                if(parsedBytes == 0)
                {
                    break;
                }

                totalParsedBytes += parsedBytes;
                dataParsedBytes += parsedBytes;
            }
        }

        private int ParseDataBlock(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes)
        {
            // Check the block body length
            int totalMessageLength = receivedMessage.GetLength(0);
            if(totalMessageLength < (alreadyParsedBytes + blockHeader.blockBodyLength))
            {
                System.Diagnostics.Debug.WriteLine("ParseDataBlock - Warning invalid length data: Total message length = {0}, parsed bytes = {1}, block body length = {2}",
                                                   totalMessageLength, alreadyParsedBytes, blockHeader.blockBodyLength);
                return (0);
            }

            int parsedBytes = 0;
            switch(messageHeader.headerUnderlyingProtocol)
            {
                case LacbusPcUnderlyingProtocols.LacbusPC:
                    switch(messageHeader.headerMessageType)
                    {
                        case 0x49: // 'I' message
                            switch(blockHeader.blockType)
                            {
                                case 0x42: // 'B' block
                                    parsedBytes = ParseProt0MessIBlockB(receivedMessage, messageHeader, blockHeader, alreadyParsedBytes);
                                    break;
                            }
                            break;
                    }
                    break;
            }
            return (parsedBytes);
        }

        private int ParseProt0MessIBlockB(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes)
        {
            int parsedBytes = 0;
            while(parsedBytes < (blockHeader.blockBodyLength))
            {
                int dataParsed = ParseSequenceOfDataNumFormatValue(receivedMessage, messageHeader, blockHeader, alreadyParsedBytes + parsedBytes);
                // Invalid data sequence?
                if(dataParsed == 0)
                {
                    parsedBytes = 0;
                    break;
                }

                parsedBytes += dataParsed;
            }
            return (parsedBytes);
        }

        private int ParseSequenceOfDataNumFormatValue(byte[] receivedMessage, LacbusPCMessageHeader messageHeader, LacbusPCDataBlockHeader blockHeader, int alreadyParsedBytes)
        {
            LacbusPCSequenceElementDatumNumFormatValue sequenceElement = new LacbusPCSequenceElementDatumNumFormatValue();
            int parsedBytes = LacbusPcProtocol.ParseLacbusPCSequenceElementDatumNumFormatValue(receivedMessage, alreadyParsedBytes, ref sequenceElement);

            DatumTypes newDataType = DatumTypes.DigitalOutput;
            DatumCategories newDataCategory = DatumCategories.Instantaneous;
            if (parsedBytes > 0)
            {
                switch (sequenceElement.datumFormat)
                {
                    case 1:
                        System.Diagnostics.Debug.WriteLine("ParseSequenceOfDataNumFormatValue: received  seq num {0} form {1} val {2}",
                            sequenceElement.datumNum, sequenceElement.datumFormat, sequenceElement.logicalValue);
                        newDataType = DatumTypes.DigitalInput;
                        break;
                    case 2:
                        System.Diagnostics.Debug.WriteLine("ParseSequenceOfDataNumFormatValue: received  seq num {0} form {1} val {2}",
                            sequenceElement.datumNum, sequenceElement.datumFormat, sequenceElement.doubleValue);
                        newDataType = DatumTypes.AnalogInput;
                        break;
                    case 3:
                        System.Diagnostics.Debug.WriteLine("ParseSequenceOfDataNumFormatValue: received  seq num {0} form {1} val {2}",
                            sequenceElement.datumNum, sequenceElement.datumFormat, sequenceElement.floatValue);
                        newDataType = DatumTypes.AnalogInput;
                        break;
                }

                // Pass received data to jobs
                UInt64 jobSearchKey = LacbusPcProtocol.CalculateJobSearchKey(messageHeader.headerRtuNumber, sequenceElement.datumNum, (byte)newDataType, (byte)newDataCategory);
                List<LacbusPCCommJob> listJobs = new List<LacbusPCCommJob>();
                GetJobListForNewData(jobSearchKey, ref listJobs);
                if (listJobs.Count > 0)
                {
                    // Process the received data
                    foreach (var jobVal in listJobs)
                    {
                        LacbusPCCommJob lJob = (LacbusPCCommJob)jobVal;
                        if(lJob == null)
                        {
                            continue;
                        }
                        ExecutedJobArgs eJob = new ExecutedJobArgs();
                        eJob.Job = lJob;
                        eJob.ErrorCode = DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError;
                        bool jobValueSet = false;
                        uint tagDataType = (uint)lJob.TagsList[0].TagNode.DataType.Identifier;
                        switch (sequenceElement.datumFormat)
                        {
                            case 1: // Boolean value
                                eJob.Values = LacbusPcProtocol.ConvertBoolValueToByteArray((BuiltInType)tagDataType, sequenceElement.logicalValue, ref jobValueSet);
                                break;

                            case 2: // Double value
                                eJob.Values = LacbusPcProtocol.ConvertDoubleValueToByteArray((BuiltInType)tagDataType, sequenceElement.doubleValue, ref jobValueSet);
                                break;

                            case 3: // Float value
                                eJob.Values = LacbusPcProtocol.ConvertFloatValueToByteArray((BuiltInType)tagDataType, sequenceElement.floatValue, ref jobValueSet);
                                break;
                        }

                        if (jobValueSet == true)
                        {
                            OnJobExecuted(eJob);
                        }
                    }
                }

                // Pass received data to stations
                if((sequenceElement.datumNum > 4000) && ((sequenceElement.datumNum < 5001)))
                {
                    ushort rtuNumber = (ushort)(sequenceElement.datumNum - 4000);
                    // No communication with an RTU?
                    if(sequenceElement.logicalValue == false)
                    {
                        if (mapStations.ContainsKey(rtuNumber) == true)
                        {
                            // No communication: we can remove the station from the list of stations for which the poll has been activated on the FR1000
                            if (activePollStationMap.ContainsKey(mapStations[rtuNumber].Name))
                            {
                                activePollStationMap.Remove(mapStations[rtuNumber].Name);
                                mapStations[rtuNumber].activePollState = false;
                                mapStations[rtuNumber].lastActivePollStateTime = DateTime.UtcNow;
                                System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - ParseSequenceOfDataNumFormatValue removed from activePollStationMap station {1} case no communication",
                                                                    DateTime.Now.ToString("HH:mm:ss.fff"), mapStations[rtuNumber].Name);
                            }

                            mapStations[rtuNumber].ManageNoCommunicationNotification();
                        }
                    }
                }
                else if ((sequenceElement.datumNum > 1000) && ((sequenceElement.datumNum < 2001)))
                {
                    ushort rtuNumber = (ushort)(sequenceElement.datumNum - 1000);
                    // RTU fault?
                    if (sequenceElement.logicalValue == true)
                    {
                        if (mapStations.ContainsKey(rtuNumber) == true)
                        {
                            // Communication fault: we can remove the station from the list of stations for which the poll has been activated on the FR1000
                            if (activePollStationMap.ContainsKey(mapStations[rtuNumber].Name))
                            {
                                activePollStationMap.Remove(mapStations[rtuNumber].Name);
                                mapStations[rtuNumber].activePollState = false;
                                mapStations[rtuNumber].lastActivePollStateTime = DateTime.UtcNow;
                                System.Diagnostics.Debug.WriteLine("RTU Poll DBG - {0} - ParseSequenceOfDataNumFormatValue removed from activePollStationMap station {1} case fault",
                                                                    DateTime.Now.ToString("HH:mm:ss.fff"), mapStations[rtuNumber].Name);
                            }

                            mapStations[rtuNumber].ManageRTUFaultNotification();
                        }
                    }
                    else
                    {
                        if (mapStations.ContainsKey(rtuNumber) == true)
                        {
                            mapStations[rtuNumber].ManageRTUConnectionRestored();
                        }
                    }
                }
            }

            return (parsedBytes);
        }

        #endregion

        #region Properties

        private string _LacbusPCHostName;
        public string LacbusPCHostName
        {
            get
            {
                return _LacbusPCHostName;
            }
            set
            {
                LacbusPCHostName = value;
            }
        }

        private uint _LacbusPCHostPort;
        public uint LacbusPCHostPort
        {
            get
            {
                return _LacbusPCHostPort;
            }
            set
            {
                LacbusPCHostPort = value;
            }
        }

        private uint _LacbusPCConnectionTimeout;
        public uint LacbusPCConnectionTimeout
        {
            get
            {
                return _LacbusPCConnectionTimeout;
            }
            set
            {
                LacbusPCConnectionTimeout = value;
            }
        }

        private uint _LacbusPCCommunicationTimeout;
        public uint LacbusPCCommunicationTimeout
        {
            get
            {
                return _LacbusPCCommunicationTimeout;
            }
            set
            {
                LacbusPCCommunicationTimeout = value;
            }
        }

        private uint _LacbusPCMaxNumberOfAggregatedRequests;
        public uint LacbusPCMaxNumberOfAggregatedRequests
        {
            get
            {
                return _LacbusPCMaxNumberOfAggregatedRequests;
            }
            set
            {
                LacbusPCMaxNumberOfAggregatedRequests = value;
            }
        }

        private byte _LacbusPCPCNumber;
        public byte LacbusPCPCNumber
        {
            get { return _LacbusPCPCNumber; }
            set
            {
                _LacbusPCPCNumber = value;
            }
        }

        private string _LacbusPCBackupHostName;
        public string LacbusPCBackupHostName
        {
            get
            {
                return _LacbusPCBackupHostName;
            }
            set
            {
                _LacbusPCBackupHostName = value;
            }
        }

        private string _LacbusPCTimeZone;
        public string LacbusPCTimeZone
        {
            get { return _LacbusPCTimeZone; }
            set { _LacbusPCTimeZone = value; }
        }

        private byte _LacbusPCMaxPendingRTUPollReq;
        public byte LacbusPCMaxPendingRTUPollReq
        {
            get { return _LacbusPCMaxPendingRTUPollReq; }
            set
            {
                _LacbusPCMaxPendingRTUPollReq = value;
            }
        }

        #endregion

    }
}
