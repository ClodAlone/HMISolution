using Accon.AGLink;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AGLinkSharedData;

namespace Simotion
{

    public class PlcConnection
    {
        private enum State
        {
            eNotOpen = 0,
            eDevOpened,
            eDialedUp,
            eInitAdapter,
            eConnected,
            eNotInit,
        }
        
        public Int32 devNr;
        private State conState = State.eNotInit;
        public Int32 connNr = -1;
        private Int32 timeout = 1;
        private string MLFBNr;
        private string HostName;
        private ushort HostPort = 0;
        CommunicationDriver CommDriver;
        protected object lockStream;
        private int nErrorLibraryCode = AGL4.AGL40_SUCCESS;        
        
        public bool SetParasTcpIP()
        {
            AGL4.S7tcpip para = new AGL4.S7tcpip();

            para.Conn[0].Address = HostName;
            para.Conn[0].ConnType = AGL4.CONN_PG;
            para.Conn[0].PLCClass = AGL4.PLC_Class.ePLC_300_400;
            para.Conn[0].TimeOut = timeout;
            para.Conn[0].PlcNr = 1;
            para.Conn[0].PortNr = HostPort;
            para.Conn[0].OwnAddress = 0;
            para.Conn[0].OwnPortNr = 0;

            int max = AGL4.GetMaxDevices();
            if (devNr > max)
            {
                CommDriver.OnSystemEvent(ObjectIds.Server, string.Format(Properties.Resources.ErrorMaxDeviceNrReached, max), Opc.Ua.EventSeverity.High);
                return false;
            }

            if ((nErrorLibraryCode = AGL4.SetDevType(devNr, AGL4.TYPE_S7_TCPIP)) != AGL4.AGL40_SUCCESS)
            {
                return false;
            }
            if ((nErrorLibraryCode = AGL4.SetParas(devNr, AGL4.TYPE_S7_TCPIP, (object)para)) != AGL4.AGL40_SUCCESS)
            {
                return false;
            }
            return true;
        }
        public PlcConnection()
        {
            devNr = GetDevNr();
            conState = State.eNotInit;
            lockStream = new object();
            MLFBNr = "---";
            HostName = "";
        }

        public bool Init(string HostName, ushort HostPort, Int32 timeout, CommunicationDriver CommDriver)
        {
            this.HostName = HostName;
            this.HostPort = HostPort;
            this.timeout = timeout;
            this.CommDriver = CommDriver;

            return true;
        }

        public bool Connect(List<Station> stations, bool bLoadFile = true)
        {                      
            Int32 RetVal = 0;

            lock (lockStream)
            {
                if (conState == State.eNotInit)
                {
                    if (stations == null || stations.Count==0)
                        return false;

                    conState = State.eNotOpen;
                }                                
                                
                if (conState == State.eNotOpen)
                {
                    if (!SetParasTcpIP())
                    {
                        return false;
                    }

                    RetVal = AGL4.OpenDevice(devNr);
                    if (RetVal == AGL4.AGL40_SUCCESS)
                    {
                        conState = State.eDevOpened;
                    }
                    else
                    {
                        nErrorLibraryCode = RetVal;
                    }
                }

                if (conState == State.eDevOpened)
                {
                    RetVal = AGL4.DialUp(devNr, timeout);
                    if (RetVal == AGL4.AGL40_SUCCESS)
                    {
                        conState = State.eDialedUp;
                    }
                    else
                    {
                        nErrorLibraryCode = RetVal;
                    }
                }

                if (conState == State.eDialedUp)
                {
                    RetVal = AGL4.InitAdapter(devNr, timeout);
                    if (RetVal == AGL4.AGL40_SUCCESS)
                    {
                        conState = State.eInitAdapter;
                    }
                    else
                    {
                        nErrorLibraryCode = RetVal;
                    }
                }

                if (conState == State.eInitAdapter)
                {
                    RetVal = AGL4.PLCConnect(devNr, 1, out connNr, timeout);
                    if (RetVal == AGL4.AGL40_SUCCESS)
                    {
                        conState = State.eConnected;
                    }
                    else
                    {
                        nErrorLibraryCode = RetVal;
                    }
                }
                                
                //if (conState == State.eConnected && bLoadFile)
                //{
                //    #region Load symbolic file (.sti)                    
                //    foreach (SimotionStation station in stations)
                //    {
                //        //The file has already been load, it is useless to delete everything to re-read the same file,
                //        //also because if you perform the procedure an exception is generated in the AGLink40 library. 
                //        if (station.HandleFile == IntPtr.Zero)
                //        {
                //            // identify symbolic file (and data source PLC/Project); try also to convert from old driver version                            
                //            if (station.SymbolFileImportSource == SimImportParser.ImportSourceManagement.Project)
                //            {
                //                // Finally try to load symbolic file
                //                if (!station.LoadFileSymbol())
                //                    station.SymbolFileImportSource = SimImportParser.ImportSourceManagement.None;
                //            }
                //        }
                //    }
                //    #endregion
                //}
            }

            if (IsConnected())
            {
                nErrorLibraryCode = AGL4.ReadMLFBNr(connNr, out MLFBNr, timeout);
                CommDriver.OnSystemEvent(null, Properties.Resources.ConnectionEstablished, EventSeverity.High);
                return true;
            }
            else
            {
                Disconnect();
                return false;
            }
        }

        public bool Disconnect()
        {
            lock (lockStream)
            {
                if (conState == State.eNotInit)
                    return false;
                Int32 _RetVal = 0;
                if (conState == State.eConnected)
                {
                    _RetVal = AGL4.PLCDisconnect(connNr, timeout);
                    if ((_RetVal != AGL4.AGL40_CONNECTION_CLOSED) &&
                         (_RetVal != AGL4.AGL40_NOT_CONNECTED) &&
                         (_RetVal != AGL4.AGL40_SUCCESS))
                    {
                        nErrorLibraryCode = _RetVal;
                    }
                    conState = State.eInitAdapter;
                }
                if (conState == State.eInitAdapter)
                {
                    _RetVal = AGL4.ExitAdapter(devNr, timeout);
                    if (_RetVal != AGL4.AGL40_SUCCESS)
                    {
                        nErrorLibraryCode = _RetVal;
                    }
                    conState = State.eDialedUp;
                }
                if (conState == State.eDialedUp)
                {
                    _RetVal = AGL4.HangUp(devNr, timeout);
                    if (_RetVal != AGL4.AGL40_SUCCESS)
                    {
                        nErrorLibraryCode = _RetVal;
                    }
                    conState = State.eDevOpened;
                }
                if (conState == State.eDevOpened)
                {
                    _RetVal = AGL4.CloseDevice(devNr);
                if (_RetVal != AGL4.AGL40_SUCCESS)
                {
                    nErrorLibraryCode = _RetVal;
                }
                    conState = State.eNotOpen;
                }

                return _RetVal == AGL4.AGL40_SUCCESS;
        }
    }

        public String GetMLFBNr()
        {
            return MLFBNr;
        }

        public bool IsConnected()
        {
            lock (lockStream)
            {
                return conState == State.eConnected;
            }
        }
        public bool IsInit()
        {
            return conState != State.eNotInit;
        }        
             
        private Int32 GetDevNr()
        {
            return AGLinkShared.GetNewDevNr();
        }        
    }
    /// <summary>
    /// End class PlcConnection
    /// </summary>

    public class SimotionChannel : ChannelList
    {
        #region Constructors

        /// <summary>
        /// Initializes the SimotionChannel object.
        /// </summary>
        public SimotionChannel(CommunicationDriver commdriver, SimotionChannelSettings settings)
            : base(commdriver, settings)
        {
            _TcpChannelHostName = settings.TcpChannelSettingsHostName;
            _TcpChannelHostPort = settings.TcpChannelSettingsHostPort;
            plcConnection = new PlcConnection();            
        }

        #endregion

        #region Members
        public PlcConnection plcConnection;
        bool lastConnectionError = false;        
        #endregion

        #region Abstracts Methods

        public override bool IsDeviceOpen()
        {
            bool returnValue = plcConnection.IsConnected();
            SetStateCommandVariableBit(!returnValue, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return returnValue;
        }

        public override bool DeviceOpen()
        {
            InitPLCConnect();
            if (!plcConnection.IsConnected())
            {                
                plcConnection.Connect(CommDriver.GetChannelStations(this));
            }
            if (!IsDeviceOpen() && ! lastConnectionError )
                CommDriver.OnSystemEvent(ObjectIds.Server, Properties.Resources.ErrorConnection, Opc.Ua.EventSeverity.High);
            lastConnectionError = !IsDeviceOpen();
            return !lastConnectionError;
        }

        public override bool DeviceClose()
        {
            DisConnection();
            SetStateCommandVariableBit(true, (UInt16)ChannelVariableBits.ChannelUnconnected);
            return plcConnection.Disconnect();
        }

        public override bool DeviceRead(byte[] Buffer, uint Count) { return true; }
        public override bool DeviceWrite(byte[] Buffer, uint Count) { return true; }
        public override uint GetBytesToRead() { return 1; }
        public override uint GetBytesToWrite() { return 1; }
        #endregion

        #region Override Methods

        protected void SetJobsInError(List<CommJob> list, DriverErrorCodes error, bool generalError = false)
        {
            for (int i = 0; i<list.Count; i++)
            {
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                eJob.ErrorCode = error;                
                eJob.GeneralError = generalError;
                eJob.Job = list[i];
                OnJobExecuted(eJob);
            }
            return;
        }

        public override bool IsScheduledJobsListFull(List<CommJob> jobList)
        {
            return (jobList.Count == SimotionProtocol.MAX_AGGREGATED_JOBS);
        }

        public override void SplitInExecutionLists(List<CommJob> jobList, ref List<List<CommJob>> exList)
        {
            int jobIndex = 0;
            while (jobIndex < jobList.Count)
            {
                bool write = false;
                bool first = true;
                string station = string.Empty;
                //append a new list to the end of exList
                List<CommJob> list = new List<CommJob>();

                while (jobIndex < jobList.Count)
                {
                    SimotionCommJob j = jobList.ElementAt(jobIndex) as SimotionCommJob;
                    if (j != null)
                    {
                        if (first)
                        {
                            write = (j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;//j.TagsListToWrite.Count > 0 || j.Type == LinkType.UnconditionalOutput;
                            station = j.Station.Name;
                            first = false;
                        }
                        bool jwrite = (j.GetTagListOnWritingCount() > 0 && (j.Type == LinkType.ExceptionOutput || j.Type == LinkType.InputOutput)) || j.Type == LinkType.UnconditionalOutput;
                        if (write)
                        {
                            if (j.Station != null && j.Station.Name == station && jwrite)
                            {
                                if (list.Count >= SimotionProtocol.MAX_AGGREGATED_JOBS)
                                    break;

                                list.Add(j);
                                jobList.RemoveAt(jobIndex);
                                jobIndex--;
                            }
                        }
                        else
                        {
                            if (j.Station != null && j.Station.Name == station && !jwrite && j.Type != LinkType.ExceptionOutput)
                            {
                                if (list.Count >= SimotionProtocol.MAX_AGGREGATED_JOBS)
                                    break;

                                list.Add(j);
                                jobList.RemoveAt(jobIndex);
                                jobIndex--;
                            }
                        }
                    }
                    jobIndex++;
                }

                if (list.Count > 0)
                {
                    exList.Add(list);
                    jobIndex = 0;
                }
            } 
        }
        #endregion

        #region Methods     
        public override bool ExecuteJobList(ref DriverErrorCodes conn, List<CommJob> exjoblist)
        {
            if (conn != DriverErrorCodes.ErrorNoError)
            {
                SetJobsInError(exjoblist, conn, (conn == DriverErrorCodes.ErrorTimeOut));
                return false;
            }

            ExchangeData(exjoblist);

            return false;
        }

        public override bool Startup()
        {
            if (!bChannelStarted)
            {
                if (!HasAllStationsSymbolicFile())
                    return false;
            }

            return base.Startup();
        }

        private bool HasAllStationsSymbolicFile()
        {
            List<Station> stations = CommDriver.GetChannelStations(this);

            return (stations.Count(s => ((SimotionStation)s).ImportSource == SimotionImportParser.SimImportParser.ImportSourceManagement.UnAvailableOrInvalid) != stations.Count);
        }

        protected void ExchangeData(List<CommJob> list)
        {            
            Parallel.ForEach(list, j =>
            {
                if (j.Type == DriverCodeBaseEx.Enumerators.LinkType.UnconditionalOutput)
                    j.FillWholeTagsListOnWriting();
            });


            if (list[0].Type == DriverCodeBaseEx.Enumerators.LinkType.Input ||
               (list[0].Type == DriverCodeBaseEx.Enumerators.LinkType.InputOutput &&
               list[0].GetTagListOnWritingCount() == 0))
            {
                ReadData(list);
            }
            else
            {
                WriteData(list);
            }
        }

        protected void ReadData(List<CommJob> list)
        {
            List<AGL4.SymbolicRW> rwsTmp = new List<AGL4.SymbolicRW>();                        
            List<AGLinkReadWriteElement> Elements = new List<AGLinkReadWriteElement>();

            SimotionStation s = list[0].Station as SimotionStation;
            if (s == null)
                return;

            foreach (SimotionCommJob j in list) 
            {
                if (j.AccessHandle == SimotionCommJob.AccessHandleState.Undefined)
                {
                    string szError = string.Empty;
                    string address = GetAddresTypePLC(s, j.StartAddress);
                    if (j.LoadAccessHandleAndSize(((SimotionStation)j.Station).HandleFile, s.Name, address, ref szError))
                        j.AccessHandle = SimotionCommJob.AccessHandleState.Found;
                    else
                        j.AccessHandle = SimotionCommJob.AccessHandleState.NotFound;
                }

                if (j.AccessHandle == SimotionCommJob.AccessHandleState.NotFound)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorFromDllAGLink;
                    base.ExecuteJob(j);
                    eJob.Job = j;
                    OnJobExecuted(eJob);
                }
                else
                {
                    rwsTmp.Add(j.m_stSymbolicRW);
                    // to read from PLC keep rwsTmp[] array index, job and default errorcode (for future use)
                    Elements.Add(new AGLinkReadWriteElement((rwsTmp.Count - 1), j, DriverErrorCodes.ErrorNoError));
                }
                
                if (Elements.Count >= SimotionProtocol.MAX_AGGREGATED_JOBS)
                    break;
            }

            // no elements to exchange --> exit 
            if (Elements.Count(a=>a.HasRWSSymbolToExchange()) == 0)
                return;

            AGL4.SymbolicRW[] rws = rwsTmp.ToArray();
            ;            
            //Read 
            Int32 nRet = AGL4.Simotion_ReadMixEx(plcConnection.connNr, rws, Timeout);
            if (nRet < AGL4.AGL40_SUCCESS)
            {                
                string szAux = String.Format("{0} {1}", s.Name, String.Format(Properties.Resources.ErrorFromDllAGLink, nRet));
                CommDriver.OnSystemEvent(ObjectIds.Server, szAux, Opc.Ua.EventSeverity.High);
                if ((nRet == AGL4.AGL40_TIMEOUT)||
                    (nRet == AGL4.AGL40_CONNECTION_CLOSED) ||
                    (nRet == AGL4.AGL40_NOT_CONNECTED))
                {
                    LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorTimeOut;
                }
                else
                {
                    LastErrorCode =  (DriverErrorCodes)S7ErrorCodes.ErrorFromDllAGLink;
                }

                DisConnection();
            }

            foreach (AGLinkReadWriteElement Element in Elements)
            {                
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                SimotionCommJob j = Element.Job;

                AGL4.SymbolicRW PLCData = new AGL4.SymbolicRW();
                if (Element.HasRWSSymbolToExchange())
                    PLCData = rws[Element.SymbolicRWSIndex];

                int ErrorCode = (int)Element.ErrorCode;
                // no initial errore code
                if (ErrorCode == (int)DriverErrorCodes.ErrorNoError)
                {
                    // general error code (connection broken, ecc)
                    if (nRet < AGL4.AGL40_SUCCESS)
                    {
                        ErrorCode = (int)LastErrorCode;
                    }
                    else
                    {
                        // error of single element
                        if (PLCData.SError != 0)
                            ErrorCode = (int)PLCData.SError;
                        else if (PLCData.Result != 0)
                            ErrorCode = (int)PLCData.Result;
                        // generic error
                        else if (!Element.HasRWSSymbolToExchange())
                            ErrorCode = (int)S7ErrorCodes.ErrorFromDllAGLink;
                    }
                }

                if (ErrorCode == (int)DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError)
                {
                    eJob.ErrorCode = DriverErrorCodes.ErrorNoError;

                    //if ((j.S7DataFormat != S7DataFormats.String) && (j.S7DataFormat != S7DataFormats.S7_DTL))
                    //{
                    //    eJob.Values = PLCData.Buffer;
                    //    //System.Diagnostics.Debug.WriteLine("Value 0 {0:X}{1:X}", j.m_stSymbolicRW.Buffer[0], j.m_stSymbolicRW.Buffer[1]);
                    //}

                    ////Condition only Array of String
                    //else if ((j.TagsList[0].TagNode.ArrayDimension != 0) &&
                    //    (j.S7DataFormat == S7DataFormats.String) &&
                    //    (PLCData.BufferLen >= (j.TagsList[0].TagNode.ArrayDimension * j.StringLength)))
                    //{

                    //    uint tmpOffsetSource = 0;
                    //    uint tmpOffsetDestination = 0;
                    //    byte[] tmpBuffer = null;
                    //    for (int nuberofCyclin = 0; nuberofCyclin < j.TagsList[0].TagNode.ArrayDimension; nuberofCyclin++)
                    //    {
                    //        if (nuberofCyclin == 0)
                    //        {
                    //            tmpBuffer = new byte[j.StringLength];
                    //            tmpOffsetSource = 1;
                    //            tmpOffsetDestination = 0;
                    //        }
                    //        else
                    //        {
                    //            int nDimensionArray = (int)j.StringLength * nuberofCyclin;
                    //            Array.Resize(ref tmpBuffer, tmpBuffer.Length + (int)j.StringLength);
                    //            tmpOffsetSource += (j.StringLength + 1);
                    //            tmpOffsetDestination += j.StringLength;
                    //        }
                    //        Array.Copy(PLCData.Buffer, tmpOffsetSource, tmpBuffer, tmpOffsetDestination, j.StringLength);
                    //    }
                    //    eJob.Values = tmpBuffer;
                    //}
                    ////Condition only S7_DTL
                    //else if (j.S7DataFormat == S7DataFormats.S7_DTL)
                    //{
                    //    byte[] tmpBuffer = new byte[PLCData.BufferLen];
                    //    tmpBuffer[0] = PLCData.Buffer[1];
                    //    tmpBuffer[1] = PLCData.Buffer[0];
                    //    tmpBuffer[2] = PLCData.Buffer[2];
                    //    tmpBuffer[3] = PLCData.Buffer[3];
                    //    tmpBuffer[4] = PLCData.Buffer[4];
                    //    tmpBuffer[5] = PLCData.Buffer[5];
                    //    tmpBuffer[6] = PLCData.Buffer[6];
                    //    tmpBuffer[7] = PLCData.Buffer[7];
                    //    tmpBuffer[8] = PLCData.Buffer[11];
                    //    tmpBuffer[9] = PLCData.Buffer[10];
                    //    tmpBuffer[10] = PLCData.Buffer[9];
                    //    tmpBuffer[11] = PLCData.Buffer[8];

                    //    eJob.Values = tmpBuffer;
                    //}
                    //else
                    //{
                    //    uint numChar = (uint)PLCData.Buffer[0];
                    //    byte[] stringAnswer = new byte[numChar];
                    //    Array.Copy(PLCData.Buffer, 1, stringAnswer, 0, numChar);
                    //    eJob.Values = stringAnswer;
                    //}                  

                    switch (j.S7DataFormat)
                    {
                        //case S7DataFormats.S7_DTL:
                        //    {
                        //        byte[] tmpBuffer = new byte[PLCData.BufferLen];
                        //        tmpBuffer[0] = PLCData.Buffer[1];
                        //        tmpBuffer[1] = PLCData.Buffer[0];
                        //        tmpBuffer[2] = PLCData.Buffer[2];
                        //        tmpBuffer[3] = PLCData.Buffer[3];
                        //        tmpBuffer[4] = PLCData.Buffer[4];
                        //        tmpBuffer[5] = PLCData.Buffer[5];
                        //        tmpBuffer[6] = PLCData.Buffer[6];
                        //        tmpBuffer[7] = PLCData.Buffer[7];
                        //        tmpBuffer[8] = PLCData.Buffer[11];
                        //        tmpBuffer[9] = PLCData.Buffer[10];
                        //        tmpBuffer[10] = PLCData.Buffer[9];
                        //        tmpBuffer[11] = PLCData.Buffer[8];
                        //        eJob.Values = tmpBuffer;
                        //    }
                        //    break;

                        case S7DataFormats.String:
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                uint numChar = (uint)PLCData.Buffer[0];
                                byte[] stringAnswer = new byte[numChar];
                                Array.Copy(PLCData.Buffer, 1, stringAnswer, 0, numChar);
                                eJob.Values = stringAnswer;
                            }
                            else
                            {
                                if(PLCData.BufferLen >= j.TotalJobSize)
                                {
                                    uint numChar = 0;
                                    uint tmpOffsetSource = 1;
                                    uint arrayElementSize = j.StringLength * tmpOffsetSource;
                                    uint arrayElementSizeDevice = (uint)(PLCData.BufferLen / j.TagsList[0].TagNode.ArrayDimension) - tmpOffsetSource;

                                    // Check if the buffer of read data contains the expected data
                                    if(arrayElementSizeDevice < arrayElementSize)
                                    {
                                        eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData;
                                    }
                                    else
                                    {
                                        // movicon buffer
                                        byte[] tmpBuffer = new byte[j.TotalJobSize * tmpOffsetSource];

                                        for (int nuberofCyclin = 0; nuberofCyclin < j.TagsList[0].TagNode.ArrayDimension; nuberofCyclin++)
                                        {
                                            // string size (nr of chars)
                                            numChar = (uint)PLCData.Buffer[((arrayElementSizeDevice + tmpOffsetSource) * nuberofCyclin)];
                                            if (numChar > j.StringLength)
                                                numChar = j.StringLength;

                                            // real string size depend on j.S7DataFormat
                                            Array.Copy(PLCData.Buffer, ((arrayElementSizeDevice * nuberofCyclin) + ((nuberofCyclin + 1) * tmpOffsetSource)), tmpBuffer, (arrayElementSize * nuberofCyclin), (numChar * tmpOffsetSource));
                                        }
                                        eJob.Values = tmpBuffer;
                                    }
                                }
                                else
                                {
                                    eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData;
                                }
                            }
                        break;

                        //case S7DataFormats.WString:
                        //    if (j.TagsList[0].TagNode.ArrayDimension == 0)
                        //    {
                        //        uint numChar = (uint)PLCData.Buffer[0] * 2;
                        //        byte[] stringAnswer = new byte[numChar];
                        //        Array.Copy(PLCData.Buffer, 2, stringAnswer, 0, numChar);
                        //        eJob.Values = stringAnswer;
                        //    }
                        //    else
                        //    {
                        //        if (PLCData.BufferLen >= j.TotalJobSize)
                        //        {
                        //            uint numChar = 0;
                        //            uint tmpOffsetSource = 2;
                        //            uint arrayElementSize = j.StringLength * tmpOffsetSource;
                        //            uint arrayElementSizeDevice = (uint)(PLCData.BufferLen / j.TagsList[0].TagNode.ArrayDimension) - tmpOffsetSource;

                        //            // Check if the buffer of read data contains the expected data
                        //            if (arrayElementSizeDevice < arrayElementSize)
                        //            {
                        //                eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData;
                        //            }
                        //            else
                        //            {
                        //                // movicon buffer
                        //                byte[] tmpBuffer = new byte[j.TotalJobSize * tmpOffsetSource];

                        //                for (int nuberofCyclin = 0; nuberofCyclin < j.TagsList[0].TagNode.ArrayDimension; nuberofCyclin++)
                        //                {
                        //                    // string size (nr of chars)
                        //                    numChar = (uint)PLCData.Buffer[((arrayElementSizeDevice + tmpOffsetSource) * nuberofCyclin)];
                        //                    if (numChar > j.StringLength)
                        //                        numChar = j.StringLength;

                        //                    // real string size depend on j.S7DataFormat
                        //                    Array.Copy(PLCData.Buffer, ((arrayElementSizeDevice * nuberofCyclin) + ((nuberofCyclin + 1) * tmpOffsetSource)), tmpBuffer, (arrayElementSize * nuberofCyclin), (numChar * tmpOffsetSource));
                        //                }
                        //                eJob.Values = tmpBuffer;
                        //            }
                        //        }
                        //        else
                        //        {
                        //            eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData;
                        //        }
                        //    }
                        //    break;

                        case S7DataFormats.S5Time:
                            {
                                byte[] tmpBuffer = new byte[j.TagsList[0].Size];

                                if (SimotionProtocol.ConvertFromS5Time(j.Trans, PLCData.Buffer, ref tmpBuffer))                                     
                                    eJob.Values = tmpBuffer;
                                else
                                    eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTagDataTypeTooSmal;

                            }
                            break;

                        default:
                            eJob.Values = PLCData.Buffer;
                            break;
                    }
                }
                else
                {
                    eJob.ErrorCode = (DriverErrorCodes)ErrorCode;
                }
                base.ExecuteJob(j);
                eJob.Job = j;
                OnJobExecuted(eJob);
            }
        }

        protected void WriteData(List<CommJob> list)
        {
            List<AGL4.SymbolicRW> rwsTmp = new List<AGL4.SymbolicRW>();
            List<AGLinkReadWriteElement> Elements = new List<AGLinkReadWriteElement>();

            SimotionStation s = list[0].Station as SimotionStation;
            if (s == null)
                return;

            foreach (SimotionCommJob j in list)
            {
                if (j.AccessHandle == SimotionCommJob.AccessHandleState.Undefined)
                {
                    string szError = string.Empty;
                    string address = GetAddresTypePLC(s, j.StartAddress);
                    if (j.LoadAccessHandleAndSize(((SimotionStation)j.Station).HandleFile, s.Name, address, ref szError))
                        j.AccessHandle = SimotionCommJob.AccessHandleState.Found;
                    else
                        j.AccessHandle = SimotionCommJob.AccessHandleState.NotFound;
                }

                base.ExecuteJob(j);
                object objectData = null;
                j.GetJobData(ref objectData);
                if (j.TagsListOnWriting.Count == 0)
                    continue;

                if (j.AccessHandle == SimotionCommJob.AccessHandleState.NotFound)
                {
                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorFromDllAGLink;
                    eJob.Job = j;
                    OnJobExecuted(eJob);

                    continue;
                }
                
                #region Prepare data to send to PLC
                byte[] jobdata = new byte[0];
                int lengthBuff = 0;
                // for all data type except WSTRING
                if(objectData is byte[])
                {
                    jobdata = (byte[])objectData;
                    lengthBuff = jobdata.Length;
                    if (jobdata.Length > j.m_stSymbolicRWrite.BufferLen)
                    {
                        lengthBuff = j.m_stSymbolicRWrite.BufferLen;
                    }
                }

                switch (j.S7DataFormat)
                {
                    //case S7DataFormats.S7_DTL:
                    //    for (int nCyclic = 0; nCyclic < j.TagsList.Count; nCyclic++)
                    //    {
                    //        if (nCyclic > 7)
                    //        {
                    //            break;
                    //        }
                    //        byte[] jobdataTmp = new byte[j.TagsList[nCyclic].Size];
                    //        j.TagsList[nCyclic].GetTagBuffer(ref jobdataTmp);
                    //        switch (nCyclic)
                    //        {
                    //            case 0:
                    //                j.m_stSymbolicRWrite.Buffer[0] = jobdataTmp[1];
                    //                j.m_stSymbolicRWrite.Buffer[1] = jobdataTmp[0];
                    //                break;
                    //            case 1:
                    //            case 2:
                    //            case 3:
                    //            case 4:
                    //            case 5:
                    //            case 6:
                    //                j.m_stSymbolicRWrite.Buffer[nCyclic + 1] = jobdataTmp[0];
                    //                break;
                    //            case 7:
                    //                j.m_stSymbolicRWrite.Buffer[8] = jobdataTmp[3];
                    //                j.m_stSymbolicRWrite.Buffer[9] = jobdataTmp[2];
                    //                j.m_stSymbolicRWrite.Buffer[10] = jobdataTmp[1];
                    //                j.m_stSymbolicRWrite.Buffer[11] = jobdataTmp[0];
                    //                break;
                    //            default:
                    //                break;
                    //        }
                    //    }
                    //    break;

                    case S7DataFormats.String:
                        {
                            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                            {
                                // reset write buffer before to fill with new data
                                Array.Clear(j.m_stSymbolicRWrite.Buffer, 0, j.m_stSymbolicRWrite.Buffer.Length);
                                uint numChar = 0;
                                while ((numChar < jobdata.Length) && (jobdata[numChar] != 0))
                                    numChar++;

                                if (numChar > j.m_stSymbolicRWrite.BufferLen - 1)
                                    numChar = (uint)(j.m_stSymbolicRWrite.BufferLen - 1);
                                else if (numChar > j.StringLength)
                                    numChar = j.StringLength;

                                j.m_stSymbolicRWrite.Buffer[0] = (byte)numChar;
                                Array.Copy(jobdata, 0, j.m_stSymbolicRWrite.Buffer, 1, numChar);
                            }
                            else
                            {
                                Array.Clear(j.m_stSymbolicRWrite.Buffer, 0, j.m_stSymbolicRWrite.Buffer.Length);
                                int tmpOffsetSource = 1;
                                int arrayElementSize = (int)(j.StringLength * tmpOffsetSource);
                                int arrayElementSizeDevice = (int)(j.m_stSymbolicRWrite.BufferLen / j.TagsList[0].TagNode.ArrayDimension) - tmpOffsetSource;
                                // Check if the buffer of the job can contain the data to be written
                                if(arrayElementSizeDevice < arrayElementSize)
                                {
                                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                                    eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooData;
                                    eJob.Job = j;
                                    OnJobExecuted(eJob);
                                    continue;
                                }

                                for (int nuberofCyclin = 0; nuberofCyclin < j.TagsList[0].TagNode.ArrayDimension; nuberofCyclin++)
                                {
                                    uint numChar = 0;
                                    for (int i = 0; i < arrayElementSize; i++)
                                    {
                                        if (jobdata[(arrayElementSize * nuberofCyclin) + i] != 0)
                                            numChar++;
                                        else
                                            break;
                                    }

                                    if (numChar > arrayElementSizeDevice)
                                        numChar = (uint)(arrayElementSizeDevice);
                                    else if (numChar > j.StringLength)
                                        numChar = j.StringLength;
                                    j.m_stSymbolicRWrite.Buffer[((arrayElementSize + tmpOffsetSource) * nuberofCyclin)] = (byte)numChar;
                                    Array.Copy(jobdata, (arrayElementSize * nuberofCyclin), j.m_stSymbolicRWrite.Buffer, ((arrayElementSize * nuberofCyclin) + ((nuberofCyclin + 1) * tmpOffsetSource)), numChar);
                                }
                            }
                            break;
                        }
                    //case S7DataFormats.WString:
                    //    {
                    //        // invalid data from Movicon
                    //        if (objectData == null)
                    //        {
                    //            ExecutedJobArgs eJob = new ExecutedJobArgs();
                    //            eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooFewData;
                    //            eJob.Job = j;
                    //            OnJobExecuted(eJob);

                    //            continue;
                    //        }
                    //        else
                    //        {
                    //            if (j.TagsList[0].TagNode.ArrayDimension == 0)
                    //            {
                    //                // reset write buffer before to fill with new data
                    //                Array.Clear(j.m_stSymbolicRWrite.Buffer, 0, j.m_stSymbolicRWrite.Buffer.Length);

                    //                string moviconString = (string)objectData;

                    //                uint numChar = 0;
                    //                // check that string length don't exceed plc string size
                    //                if ((moviconString.Length * 2) > j.m_stSymbolicRWrite.BufferLen - 2)
                    //                    numChar = (uint)((j.m_stSymbolicRWrite.BufferLen - 2) / 2);
                    //                else if (moviconString.Length > j.StringLength)
                    //                    numChar = j.StringLength;
                    //                else
                    //                    numChar = (uint)moviconString.Length;

                    //                // set string length
                    //                j.m_stSymbolicRWrite.Buffer[0] = (byte)numChar;
                    //                // Unicode, 1 char --> 2 bytes
                    //                Array.Copy(System.Text.Encoding.Unicode.GetBytes(moviconString), 0, j.m_stSymbolicRWrite.Buffer, 2, numChar * 2);
                    //            }
                    //            else
                    //            {
                    //                Array.Clear(j.m_stSymbolicRWrite.Buffer, 0, j.m_stSymbolicRWrite.Buffer.Length);

                    //                int tmpOffsetSource = 2;
                    //                int arrayElementSize = (int)(j.StringLength * tmpOffsetSource);
                    //                int arrayElementSizeDevice = (int)(j.m_stSymbolicRWrite.BufferLen / j.TagsList[0].TagNode.ArrayDimension) - tmpOffsetSource;
                    //                // Check if the buffer of the job can contain the data to be written
                    //                if (arrayElementSizeDevice < arrayElementSize)
                    //                {
                    //                    ExecutedJobArgs eJob = new ExecutedJobArgs();
                    //                    eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTooData;
                    //                    eJob.Job = j;
                    //                    OnJobExecuted(eJob);
                    //                    continue;
                    //                }

                    //                uint arrayDimension = j.TagsList[0].TagNode.ArrayDimension;

                    //                Array moviconStringArr = objectData as Array;
                    //                for (int nuberofCyclin = 0; nuberofCyclin < j.TagsList[0].TagNode.ArrayDimension; nuberofCyclin++)
                    //                {
                    //                    string moviconString = moviconStringArr.GetValue(nuberofCyclin).ToString().Trim('\0');

                    //                    uint numChar = 0;
                    //                    if ((moviconString.Length * 2) > arrayElementSizeDevice)
                    //                        numChar = (uint)(arrayElementSizeDevice / 2);
                    //                    else if (numChar > j.StringLength)
                    //                        numChar = j.StringLength;
                    //                    else
                    //                        numChar = (uint)moviconString.Length;

                    //                    j.m_stSymbolicRWrite.Buffer[((arrayElementSize + tmpOffsetSource) * nuberofCyclin)] = (byte)numChar;

                    //                    Array.Copy(System.Text.Encoding.Unicode.GetBytes(moviconString), 0, j.m_stSymbolicRWrite.Buffer, ((arrayElementSize * nuberofCyclin) + ((nuberofCyclin + 1) * tmpOffsetSource)), numChar * 2);
                    //                }
                    //            }
                    //        }
                    //        break;
                    //    }
                    case S7DataFormats.S5Time:
                        if (!SimotionProtocol.ConvertToS5Time(j.Trans, jobdata, ref j.m_stSymbolicRWrite.Buffer))
                        {
                            ExecutedJobArgs eJob = new ExecutedJobArgs();
                            eJob.ErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorTagDataTypeTooSmal;
                            eJob.Job = j;
                            OnJobExecuted(eJob);

                            continue;
                        }
                        break;

                    default:
                        Array.Copy(jobdata, 0, j.m_stSymbolicRWrite.Buffer, 0, lengthBuff);
                        //System.Diagnostics.Debug.WriteLine("Value 0 {0:X}{1:X}", j.m_stSymbolicRW.Buffer[0], j.m_stSymbolicRW.Buffer[1]);
                        break;
                }
                #endregion

                rwsTmp.Add(j.m_stSymbolicRWrite);
                // to read from PLC keep rwsTmp[] array index, job and default errorcode (for feature use)
                Elements.Add(new AGLinkReadWriteElement((rwsTmp.Count - 1), j, DriverErrorCodes.ErrorNoError));

                if (Elements.Count >= SimotionProtocol.MAX_AGGREGATED_JOBS)
                    break;
            }

            // no elements to exchange --> exit 
            if (Elements.Count(a => a.HasRWSSymbolToExchange()) == 0)
                return;

            AGL4.SymbolicRW[] rws = rwsTmp.ToArray();
          
            //Write 
            Int32 nRet = AGL4.Simotion_WriteMixEx(plcConnection.connNr, rws, Timeout);
            if (nRet < AGL4.AGL40_SUCCESS)
            {                
                string szAux = String.Format("{0} {1}", s.Name, String.Format(Properties.Resources.ErrorFromDllAGLink, nRet));
                CommDriver.OnSystemEvent(ObjectIds.Server, szAux, Opc.Ua.EventSeverity.High);
                if ((nRet == AGL4.AGL40_TIMEOUT) ||
                    (nRet == AGL4.AGL40_CONNECTION_CLOSED) ||
                    (nRet == AGL4.AGL40_NOT_CONNECTED))
                {
                    LastErrorCode = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorTimeOut;
                }
                else
                {
                    LastErrorCode = (DriverErrorCodes)S7ErrorCodes.ErrorFromDllAGLink;
                }

                DisConnection();
            }

            foreach (AGLinkReadWriteElement Element in Elements) {
                ExecutedJobArgs eJob = new ExecutedJobArgs();
                SimotionCommJob j = Element.Job;

                AGL4.SymbolicRW PLCData = new AGL4.SymbolicRW();
                if (Element.HasRWSSymbolToExchange())
                    PLCData = rws[Element.SymbolicRWSIndex];

                int ErrorCode = (int)Element.ErrorCode;
                // no initial errore code
                if (ErrorCode == (int)DriverErrorCodes.ErrorNoError)
                {
                    // general error code (connection broken, ecc)
                    if (nRet < AGL4.AGL40_SUCCESS)
                    {
                        ErrorCode = (int)LastErrorCode;
                    }
                    else
                    {
                        // error of single element
                        if (PLCData.SError != 0)
                            ErrorCode = (int)PLCData.SError;
                        else if (PLCData.Result != 0)
                            ErrorCode = (int)PLCData.Result;
                        // generic error
                        else if (!Element.HasRWSSymbolToExchange())
                            ErrorCode = (int)S7ErrorCodes.ErrorFromDllAGLink;
                    }
                }

                eJob.ErrorCode = (DriverErrorCodes)ErrorCode; 
                eJob.Job = j;
                OnJobExecuted(eJob);
            }
        }

        public string GetAddresTypePLC(SimotionStation station, string originaAddress)
        {
            string address;            
            int result = 0;
            int error_pos = 0;
            IntPtr accessHandle = IntPtr.Zero;

            // test dynamic setting address
            address = originaAddress;
            result = AGL4.Simotion_GetNodeByPath(station.HandleFile, address, ref accessHandle, ref error_pos);
            if (result == AGL4.AGL40_SUCCESS)
                return address;
            else
                return originaAddress;
        }

        public bool InitPLCConnect()
        {            
            if (plcConnection.IsInit())
                return true;

            return plcConnection.Init(TcpChannelHostName, (ushort)TcpChannelHostPort, Timeout, CommDriver);
        }

        public bool DisConnection()
        {
            if (!plcConnection.IsInit())
                return true;
            
            return plcConnection.Disconnect();
        }
        
        #endregion        

        #region Properties
        /// <summary>   Host name. </summary>
        private string _TcpChannelHostName;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the name of the TCP channel host. </summary>
        ///
        /// <value> The name of the TCP channel host. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public string TcpChannelHostName
        {
            get { return _TcpChannelHostName; }
            set
            {
                _TcpChannelHostName = value;
            }
        }

        /// <summary>   Host port. </summary>
        private int _TcpChannelHostPort;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Gets or sets the TCP channel host port. </summary>
        ///
        /// <value> The TCP channel host port. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public int TcpChannelHostPort
        {
            get
            {
                return _TcpChannelHostPort;
            }
            set
            {
                _TcpChannelHostPort = value;
            }
        }
        #endregion
    }
}
