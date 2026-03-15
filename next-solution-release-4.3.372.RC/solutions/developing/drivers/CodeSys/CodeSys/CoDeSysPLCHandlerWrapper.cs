using System;
using System.Runtime.InteropServices;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using System.Linq;
using log4net;
using System.Threading;

namespace CoDeSys
{
    public class CoDeSysPLCHandlerWrapper : IDisposable
    {
        #region Api calls to CoDeSys wrapper library
        #region Object instace functions
        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWIsWrapperInstalled", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CSWIsWrapperInstalled();

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWIsCoDeSysInstalled", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CSWIsCoDeSysInstalled();        

        //[DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWInit", CallingConvention = CallingConvention.Cdecl)]
        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWInit", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CSWInit();

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWCreateStation", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWCreateStation(out int stationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWNumActiveStations", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWNumActiveStations();        

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWReleaseStation", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWReleaseStation(int stationID);
        #endregion

        #region Various
        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetLastError", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CSWGetLastError(int stationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetState", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CSWGetState(int stationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWConnectViaGateway3", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CSWConnectViaGateway3(int stationID, string ip, string address, int symbols, uint timeout);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWDisconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CSWDisconnect(int stationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetVarListFromPLC", CallingConvention = CallingConvention.Cdecl)]
        private static extern int CSWGetVarListFromPLC(int stationID, ref IntPtr ptrSymbols, out uint ulNumOfSymbols);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetItem", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int CSWGetItem(int stationID, string pszSymbol, ref IntPtr ptrSymbols);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWConnect", CallingConvention = CallingConvention.StdCall)]
        private static extern unsafe int CSWConnect(int nStationID, ulong nTypeConnection, string GatewayAddress, string PLCAddress, ulong port, string username, string passwordPLC, string GatewayPassword);
        #endregion

        #region Sync read functions
        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWSyncReadVarsFromPlc", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWSyncReadVarsFromPlc(int nStationID, string pszSymbols, uint ulNumOfSymbols);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWSyncReadVarsFromPlcReleaseValues", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWSyncReadVarsFromPlcReleaseValues(int nStationID);
        
        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWReleaseSyncReadedVars", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void CSWReleaseSyncReadedVars(int nStationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetSyncReadedVarFromPlc", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWGetSyncReadedVarFromPlc(int nStationID, uint ulSymbolNr, uint nSymbolSize, IntPtr ptrData, out uint ulTimeStamp, out byte bQuality);
        #endregion

        #region Cycling Read functions
        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWCycDefineVarList", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWCycDefineVarList(int nStationID, string pszSymbols, /*[In]*/ uint ulNumOfSymbols, uint ulUpdateRate);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWCycReadVars", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWCycReadVars(int nStationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWCycLeaveVarAccess", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void CSWCycLeaveVarAccess(int nStationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWCycEnterVarAccess", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWCycEnterVarAccess(int nStationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWCycDeleteVarList", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWCycDeleteVarList(int stationID, int bKeepalive);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetCycReadedVar", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWGetCycReadedVar(int nStationID, uint ulSymbolNr, uint nSymbolSize, IntPtr ptrData, out uint ulTimeStamp, out byte bQuality);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetCysReadCallBackInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWGetCysReadCallBackInfo(int nStationID, out bool bCallBackUpdated, out uint nCallBackNotifyCounter);
        #endregion

        #region Write functions
        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWSyncWriteInitValues", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWSyncWriteInitValues(int nStationID, uint ulNumVarValues);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWSyncWriteAddValue", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWSyncWriteAddValue(int nStationID, uint ulSymbolNr, string pszSymbol, IntPtr pValue, int iValueSize);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWSyncWriteVarsToPlc", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWSyncWriteVarsToPlc(int nStationID);
        #endregion

        [StructLayout(LayoutKind.Sequential)]
        public struct PlcSymbolDesc
        {
            [MarshalAs(UnmanagedType.LPStr)]
            public string pszName;
            public uint ulTypeId;   //public ulong ulOffset;
            [MarshalAs(UnmanagedType.LPStr)]
            public string pszType;
            public ushort usRefId;
            public uint ulOffset;  //public ulong ulOffset;
            public uint ulSize;            
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 2, ArraySubType = UnmanagedType.LPStr)]
            public char[] szAccess;
            public sbyte bySwapSize;
        }

        #endregion

        private enum PlcValueReadingMode
        {
            Cycling,
            Sync
        }              

        public class VarParam
        {
            public string VarName { set; get; }
            public uint VarSize { set; get; }
            public int VarIndex { set; get; }
            public int ValueIndex { set; get; }

            public VarParam()
            {
                VarName = string.Empty;
                VarSize = 0;
                VarIndex = 0;
            }

            public VarParam(string varName, CoDeSysProtocol.VarType varType)
            {
                VarName = varName;                
                VarSize = CoDeSysProtocol.GetByteSizeOfVarType(varType);
            }

            public VarParam(string varName, CoDeSysProtocol.VarType varType, uint arrayDimension)
            {
                VarName = varName;
                VarSize = CoDeSysProtocol.GetByteSizeOfVarType(varType) * arrayDimension;
            }

            public VarParam(string varName, uint size)
            {
                VarName = varName;
                VarSize = size;
            }

            public VarParam(string varName, CoDeSysProtocol.VarType varType, uint size, int varIndex, int valueIndex) : this(varName, size)
            {
                VarIndex = varIndex;
                ValueIndex = valueIndex;
            }

            public static int SizeOf(VarParam var)
            {
                if (var == null)
                    return 0;
                else 
                    // + VarSize  + VarIndex  + ValueIndex size in byte
                    return (var.VarName.Length + 6);
            }
        }

        public class VarWrite
        {
            public string VarName { set; get; }

            public int VarIndex { set; get; }
            
            public byte[] Data { set; get; }

            public int VarSize { get { return (Data == null ? 0 : Data.Length); } } 

            public VarWrite()
            {
                VarName = string.Empty;
                VarIndex = 0;
                Data = null;
            }
                        
            public VarWrite(string varName, byte[] data)
            {
                VarName = varName;
                Data = new byte[data.Length];
                Array.Copy(data, Data,data.Length);
            }

            public VarWrite(string varName, byte[] data, int varIndex) : this(varName, data)
            {
                VarIndex = varIndex;                
            }

            public bool HasData()
            {
                return (Data != null);
            }

            public static int SizeOf(VarWrite var)
            {
                if (var == null)
                    return 0;
                else
                    // + VarIndex + 
                    return (var.VarName.Length + 2 + var.VarSize);
            }
        }

        public class VarValue
        {
            public bool UnMappedVar { set; get; }
            public byte Quality { set; get; }
            public uint TimeStamp { set; get; }
            public byte[] Data;

            public VarValue(uint dataSize)
            {
                UnMappedVar = false;
                Quality = 0;
                TimeStamp = 0;
                if (dataSize <= 0)
                    Data = null;
                else
                    Data = new byte[dataSize];
            }

            public bool HasData()
            {
                return (Data != null && Quality == 1 && !UnMappedVar);
            }

            public void ResetData()
            {
                Data = null;
            }

            public static int SizeOf(VarValue var)
            {
                if (var == null)
                    return 0;
                else
                    // UnMappedVar + Quality + TimeStamp + byte size
                    return ((var.Data != null ? var.Data.Length : 0) + 4);
            }
        }

        private const int STATION_ID_INVALID = 0;

        public enum ConnectionState
        {
            Unknown,
            Connected,
            Disconnected
        }

        #region Constructors
        public CoDeSysPLCHandlerWrapper()
        {
            _Disposed = false;
            _StationId = STATION_ID_INVALID;
            _ConnectState = ConnectionState.Unknown;
            _PingHostBeforeConnectTimeOut = 0;
            //_LastState = CoDeSysProtocol.PLCHandlerState.STATE_DISCONNECT;
        }
        #endregion

        #region Properties

        private bool _Disposed;
        private object _LockObject = 1;        
        private uint[] _CycReadVarSize;
        private int _PingHostBeforeConnectTimeOut;

        private int _StationId;
        public int StationId
        {
            get { return _StationId; }
            set { _StationId = value; }
        }

        private ConnectionState _ConnectState;
        public ConnectionState ConnectState
        {
            get { return _ConnectState; }
            set { _ConnectState = value; }
        }

        //CoDeSysProtocol.PLCHandlerState _LastState;
        //public CoDeSysProtocol.PLCHandlerState LastState
        //{
        //    get { return _LastState; }
        //    set { _LastState = value; }
        //}
        #endregion

        #region Specific Methods

        static readonly ILog log = log4net.LogManager.GetLogger(Properties.Resources.AreaFileLog);

        /// <summary>
        /// Check il CoDeSysWrapper.dll is present/installed into driver's directory 
        /// </summary>
        public bool IsWrapperInstalled()
        {
            bool Result = false;

            try
            {
                // this function do not do anything. Used only to check is .dll is present
                //System.Diagnostics.Debug.WriteLine("IsWrapperInstalled");
                Result = CSWIsWrapperInstalled();
                //System.Diagnostics.Debug.WriteLine("IsWrapperInstalledEnd");

            } catch (Exception ex) { }

            return Result;
        }

        /// <summary>
        /// Check il CoDeSys is installed into PC
        /// </summary>
        public bool IsCoDeSysInstalled()
        {
            bool Result = false;


            if (!IsWrapperInstalled())
                return false;

            try
            {
                //System.Diagnostics.Debug.WriteLine("IsCoDeSysInstalled");
                Result = CSWIsCoDeSysInstalled();
                //System.Diagnostics.Debug.WriteLine("IsCoDeSysInstalledEnd");
            }
            catch (Exception ex) { }

            return Result;
        }

        /// <summary>
        /// Init internal object inside library; each client must call it 
        /// </summary>
        public void Init()
        {
            lock (_LockObject)
            {
                //System.Diagnostics.Debug.WriteLine("CSWInit");
                CSWInit();
                //System.Diagnostics.Debug.WriteLine("CSWInitEnd");
            }
        }

        /// <summary>
        /// Create CoDeSys object instance
        /// </summary>        
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors Create()
        {
            CoDeSysProtocol.PLCHandlerErrors Result;

            unsafe
            {
                if (_StationId != STATION_ID_INVALID)
                    Release();

                Result = CreateStation();
            }
            return Result;
        }

        /// <summary>
        /// Retrive unique station ID (one for each client)
        /// </summary>        
        /// <returns></returns>
        private CoDeSysProtocol.PLCHandlerErrors CreateStation()
        {
            CoDeSysProtocol.PLCHandlerErrors Result = CoDeSysProtocol.PLCHandlerErrors.RESULT_FAILED;
            //long c = CSWNumActiveStations();
            unsafe
            {
                //Result = CoDeSysProtocol.PLCHandlerErrors.RESULT_FAILED;
                //_StationId = STATION_ID_INVALID;

                lock (_LockObject)
                {
                    int NewStationID;
                    //System.Diagnostics.Debug.WriteLine("CSWCreateStation");
                    Result = (CoDeSysProtocol.PLCHandlerErrors)CSWCreateStation(out NewStationID);
                    //System.Diagnostics.Debug.WriteLine(string.Format("CSWCreateStationEnd{0}", NewStationID));
                    if (Result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                        _StationId = NewStationID;
                    else
                        _StationId = STATION_ID_INVALID;
                }

                int NrActiveAclient = CSWNumActiveStations();
            }

            return Result;
        }

        /// <summary>
        /// Release internal resource of CoDeSysWrapper api associated to ID
        /// </summary>
        public void Release()
        {
            if (_StationId != STATION_ID_INVALID)
            {
                lock (_LockObject)
                {
                    Disconnect();

                    //System.Diagnostics.Debug.WriteLine(string.Format("CSWReleaseStation{0}", _StationId));
                    CSWReleaseStation(_StationId);
                    //System.Diagnostics.Debug.WriteLine(string.Format("CSWReleaseStationEnd{0}", _StationId));

                    _StationId = STATION_ID_INVALID;
                }
            }
        }

        
        /// <summary>
        /// Establish connecion to device;
        /// Address is CoDeSys parameter that identify device (more device can be under the same ip address)
        /// </summary>
        public CoDeSysProtocol.PLCHandlerErrors Connect(string ip, string address, ManualResetEvent stopWorkerThread) {

            bool TryToConnect = true;

            if (_PingHostBeforeConnectTimeOut > 0)
                TryToConnect = PingHost(ip, _PingHostBeforeConnectTimeOut, stopWorkerThread);

            if (TryToConnect)
                return (CoDeSysProtocol.PLCHandlerErrors)ConnectViaGateway3(ip, address, 1, CoDeSysProtocol.PLCHANDLER_USE_DEFAULT);
            else
                return CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_NOT_CONNECTED;
        }

        public CoDeSysProtocol.PLCHandlerErrors ConnectAndRunning(string ip, string address, ManualResetEvent stopWorkerThread)
        {
            CoDeSysProtocol.PLCHandlerErrors ret = Connect(ip, address, stopWorkerThread);
            if (ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            { 
                if (GetState() == CoDeSysProtocol.PLCHandlerState.STATE_RUNNING)
                {
                    ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;
                }
                else
                {
                    ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_FAILED;
                }                
            }

            return ret;
        }

        public bool AllConnectionsAndRunning(string ip, ulong nTypeConnection, string address, ulong port, string username, string passwordPLC, string GatewayPassword,ManualResetEvent stopWorkerThread)
        {
            if (AllConnections(ip, nTypeConnection, address, port, username, passwordPLC, GatewayPassword, stopWorkerThread) == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                return (GetState() == CoDeSysProtocol.PLCHandlerState.STATE_RUNNING);
            }
            return false;
        }

        public CoDeSysProtocol.PLCHandlerErrors CheckConnectionAndRunningState(string ip, ulong nTypeConnection, string address, ulong port, string username, string passwordPLC, string GatewayPassword, ManualResetEvent stopWorkerThread)
        {
            CoDeSysProtocol.PLCHandlerErrors ret = AllConnections(ip, nTypeConnection, address, port, username, passwordPLC, GatewayPassword, stopWorkerThread);
            if (ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                if (GetState() == CoDeSysProtocol.PLCHandlerState.STATE_RUNNING)
                {
                    ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;
                }
                else
                {
                    ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_FAILED;
                }
            }

            return (ret);
        }

        public CoDeSysProtocol.PLCHandlerErrors AllConnections(string ip, ulong nTypeConnection, string address, ulong port, string username, string passwordPLC, string GatewayPassword, ManualResetEvent stopWorkerThread)
        {

            bool TryToConnect = true;

            if (_PingHostBeforeConnectTimeOut > 0)
                TryToConnect = PingHost(ip, _PingHostBeforeConnectTimeOut, stopWorkerThread);

            if (TryToConnect)
                return ((CoDeSysProtocol.PLCHandlerErrors)Connections(ip, nTypeConnection, address, port , username, passwordPLC, GatewayPassword));
            else
                return CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_NOT_CONNECTED;
        }

        private CoDeSysProtocol.PLCHandlerErrors ConnectViaGateway3(string ip, string address, int symbols, uint timeout)
        {
            //System.Diagnostics.Debug.WriteLine(string.Format("ConnectViaGateway3 {0} {1} {2}", _StationId, ip, address));
            CoDeSysProtocol.PLCHandlerErrors State = (CoDeSysProtocol.PLCHandlerErrors)CSWConnectViaGateway3(_StationId, ip, address, symbols, timeout);
            //System.Diagnostics.Debug.WriteLine(string.Format("ConnectViaGateway3End {0}", _StationId));
            if ((State == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)/*||
                (State == CoDeSysProtocol.PLCHandlerErrors.RESULT_RECONNECTTHREAD_STILL_ACTIVE)*/)
                _ConnectState = ConnectionState.Connected;
            return State;
        }

        private CoDeSysProtocol.PLCHandlerErrors Connections(string GatewayAddress,  ulong nTypeConnection, string PLCAddress, ulong port, string username, string passwordPLC, string GatewayPassword)
        { 
            System.Diagnostics.Debug.WriteLine(string.Format("Connect {0} {1} {2}", _StationId, GatewayAddress, PLCAddress));
            CoDeSysProtocol.PLCHandlerErrors State = (CoDeSysProtocol.PLCHandlerErrors)CSWConnect(_StationId, nTypeConnection, GatewayAddress, PLCAddress, port, username, passwordPLC, GatewayPassword); 
            System.Diagnostics.Debug.WriteLine(string.Format("ConnectEnd {0}", _StationId));
            if (State == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                _ConnectState = ConnectionState.Connected;
            return State;
        }

        /// <summary>
        /// Disconnect to device
        /// </summary>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerState Disconnect()
        {
            if (IsCycDefineVarListDefine())
            {
                CycLeaveVarAccess(CallingFunctions.CoDeSysPLCHandlerWrapper_Disconnect);
                CycDeleteVarList(CallingFunctions.CoDeSysPLCHandlerWrapper_Disconnect);
            }

            _ConnectState = ConnectionState.Disconnected;

            return (CoDeSysProtocol.PLCHandlerState)CSWDisconnect(_StationId);
        }
        
        /// <summary>
        /// Get Last state of CoDeSys PLCHandler (contain also connection state)
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerState GetState()
        {
            //_LastState = (CoDeSysProtocol.PLCHandlerState)CSWGetState(_StationId);
            //System.Diagnostics.Debug.WriteLine(string.Format("GetState {0}", _StationId));
            CoDeSysProtocol.PLCHandlerState State = (CoDeSysProtocol.PLCHandlerState)CSWGetState(_StationId);
            //System.Diagnostics.Debug.WriteLine(string.Format("GetStateEnd {0}", _StationId));

            return State;
        }

        /// <summary>
        /// Get last error generated by Codesys library
        /// </summary>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors GetLastError()
        {
            //System.Diagnostics.Debug.WriteLine(string.Format("CSWGetLastError {0}", _StationId));
            return (CoDeSysProtocol.PLCHandlerErrors)CSWGetLastError(_StationId);
        }

        /// <summary>
        /// Request last error description 
        /// </summary>
        /// <returns></returns>
        public string GetLastErrorDescription()
        {
            int error = CSWGetLastError(_StationId);

            return GetLastErrorDescription(error);
        }

        /// <summary>
        /// Get error description of defined error; if not mapped, return error code
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        public string GetLastErrorDescription(int error)
        {
            CoDeSysProtocol.PLCHandlerErrors CodifiedError;
            if (Enum.TryParse(error.ToString(), out CodifiedError))
                return CodifiedError.ToString();
            else
                return error.ToString();
        }

        /// <summary>
        /// Get error description of defined error
        /// </summary>
        public string GetLastErrorDescription(CoDeSysProtocol.PLCHandlerErrors error)
        {
            return error.ToString();
        }

        /// <summary>
        /// To allow faster connection, try to ping it before connect
        /// </summary>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public void EnableToPingHostBeforeConnection(int timeout)
        {
            _PingHostBeforeConnectTimeOut = timeout;           
        }

        /// <summary>
        /// Ping defined IP address
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="timeout"></param>
        /// <returns></returns>
        public bool PingHost(string ip, int timeout, ManualResetEvent stopWorkerThread)
        {
            try
            {
                PingReply reply = new Ping().Send(ip, timeout);

                return (reply.Status == IPStatus.Success);
            }
            catch (Exception ex)
            {
                if (stopWorkerThread != null)
                    stopWorkerThread.WaitOne(timeout);
                return false;
            }
        }


        /// <summary>
        /// Calculate the address of C pointer basing of element index and data size
        /// </summary>
        /// <param name="src"></param>
        /// <param name="offset"></param>
        /// <returns></returns>
        private IntPtr IntPtrOffset(IntPtr src, int offset)
        {
            unsafe { return new IntPtr((byte*)src + offset); }
        }

        /// <summary>
        /// Retrive the list of tag mapped into device
        /// </summary>
        /// <param name="symbols"></param>
        public CoDeSysProtocol.PLCHandlerErrors GetVarListFromPLC(out List<PlcSymbolDesc> vars)
        {
            IntPtr ptrData = IntPtr.Zero;            
            uint NrSymbols;

            vars = null;

            //System.Diagnostics.Debug.WriteLine(string.Format("CSWGetVarListFromPLC {0}", _StationId));
            CoDeSysProtocol.PLCHandlerErrors Result = (CoDeSysProtocol.PLCHandlerErrors)CSWGetVarListFromPLC(_StationId, ref ptrData, out NrSymbols);
            //System.Diagnostics.Debug.WriteLine(string.Format("CSWGetVarListFromPLCEnd {0}", _StationId));
            if (Result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                if (NrSymbols > 0 && ptrData != IntPtr.Zero)
                {
                    vars = new List<PlcSymbolDesc>();

                    int offset = 0;
                    for (uint i = 0; i < NrSymbols; i++)
                    {
                        unsafe
                        {
                            try
                            {
                                IntPtr currentStruct = IntPtrOffset(ptrData, offset);
                                PlcSymbolDesc element = (PlcSymbolDesc)Marshal.PtrToStructure(currentStruct, typeof(PlcSymbolDesc));
                                vars.Add(element);
                                offset += Marshal.SizeOf(typeof(PlcSymbolDesc));
                            }
                            catch (Exception ex)
                            {
                                Result = CoDeSysProtocol.PLCHandlerErrors.RESULT_FAILED;
                            }
                        }
                    }
                }
            }

            return Result;
        }

        /// <summary>
        /// Get information from device about specific tag
        /// </summary>
        /// <param name="pszSymbol"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors GetItem(string varName, out PlcSymbolDesc tagInfo)
        {
            IntPtr ptrData = IntPtr.Zero;
            CoDeSysProtocol.PLCHandlerErrors Result = CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;

            tagInfo = new PlcSymbolDesc();

            //System.Diagnostics.Debug.WriteLine(string.Format("CSWGetItem {0}", _StationId));
            Result = (CoDeSysProtocol.PLCHandlerErrors)CSWGetItem(_StationId, varName, ref ptrData);
            //System.Diagnostics.Debug.WriteLine(string.Format("CSWGetItemEnd {0}", _StationId));
            if (Result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                unsafe
                {
                    try
                    {
                        //System.Diagnostics.Debug.WriteLine(string.Format("CSWGetItem {0}", _StationId));
                        tagInfo = (PlcSymbolDesc)Marshal.PtrToStructure(ptrData, typeof(PlcSymbolDesc));
                        //System.Diagnostics.Debug.WriteLine(string.Format("CSWGetItemEnd {0}", _StationId));
                    } catch (Exception ex)
                    {
                        Result = CoDeSysProtocol.PLCHandlerErrors.RESULT_FAILED;
                    }
                }
            }

            return Result;
        }

        /// <summary>
        /// Convert array/list of tag into one string separated by special character
        /// </summary>        
        /// <returns></returns>
        private void GetReadVarsParameters(List<VarParam> tags, out string tagListToSingleString, out uint[] readVarSize)
        {
            tagListToSingleString = string.Join("#", tags.Select(x => x.VarName).ToArray());

            readVarSize = tags.Select(x => x.VarSize).ToArray();
        }

        /// <summary>
        /// CoDeSys c++ library share result of requested values (via Sync o Cycle) into a block of mory that cannot be shared with C#:
        /// for this reason a set of special funcion was created to access to a singol value by index
        /// </summary>
        /// <param name="mode"></param>
        /// <param name="symbolID"></param>
        /// <param name="symbol"></param>
        /// <returns></returns>
        private CoDeSysProtocol.PLCHandlerErrors GetReadedValue(PlcValueReadingMode mode, uint varID, uint varSize, out VarValue value)
        {
            CoDeSysProtocol.PLCHandlerErrors Result = CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;

            value = new VarValue(varSize);

            if (varSize > 0)
            {
                IntPtr ptrData = new IntPtr();
                uint ulTimeStamp=0;
                byte bQuality=0;

                // alloc a block of (unmanaged) memory to exchange data with c++ CoDeSysWrapper
                ptrData = Marshal.AllocHGlobal((int)varSize);

                //System.Diagnostics.Debug.WriteLine(string.Format("GetReadedValue {0}", _StationId));
                // depending of request mode
                if (mode == PlcValueReadingMode.Sync)
                    Result = (CoDeSysProtocol.PLCHandlerErrors)CSWGetSyncReadedVarFromPlc(_StationId, varID, varSize, ptrData, out ulTimeStamp, out bQuality);
                else
                    Result = (CoDeSysProtocol.PLCHandlerErrors)CSWGetCycReadedVar(_StationId, varID, varSize, ptrData, out ulTimeStamp, out bQuality);
                //System.Diagnostics.Debug.WriteLine(string.Format("GetReadedValueEnd {0}", _StationId));

                if (Result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                {
                    try
                    {
                        // if good data, copy from unmanged memory (allocated in C#) to struct byte[]
                        if (bQuality == 1)
                        {
                            //System.Diagnostics.Debug.WriteLine(string.Format("GetReadedValue {0}", _StationId));
                            Marshal.Copy(ptrData, value.Data, 0, (int)varSize);
                            //System.Diagnostics.Debug.WriteLine(string.Format("GetReadedValueEnd {0}", _StationId));
                            value.Quality = bQuality;
                            value.TimeStamp = ulTimeStamp;
                        }
                    }
                    catch
                    {
                        bQuality = 0;
                    }
                }

                if (Result != CoDeSysProtocol.PLCHandlerErrors.RESULT_OK || bQuality==0)
                {
                    value.UnMappedVar = (Result != CoDeSysProtocol.PLCHandlerErrors.RESULT_READVALUE_UNMAPPED_VAR);
                    value.Quality = 0;
                    value.TimeStamp = 0;
                    value.ResetData();
                }

                //release allocated memory
                Marshal.FreeHGlobal(ptrData);
            } else {
                value.Quality = 0;
                value.TimeStamp = 0;
                value.ResetData();
            }

            return CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;
        }

        /// <summary>
        /// Execute a Sync read from CoDeSys of listed tags
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="plcValues"></param>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors SyncReadVarsFromPlc(List<VarParam> vars, out List<VarValue> varValues)
        {
            string VarList;
            uint[] _SyncReadVarSize;
            GetReadVarsParameters(vars, out VarList, out _SyncReadVarSize);
            uint NrVar = (uint)vars.Count;
            VarValue value;

            varValues = new List<VarValue>();

            //System.Diagnostics.Debug.WriteLine(string.Format("CSWSyncReadVarsFromPlc {0}", _StationId));
            // Execute Sync read to CodeSysWrapper of listed tags (tags list are send as a single string where tag's name are separated by special character)
            CoDeSysProtocol.PLCHandlerErrors Result = (CoDeSysProtocol.PLCHandlerErrors)CSWSyncReadVarsFromPlc(_StationId, VarList, NrVar);
            //System.Diagnostics.Debug.WriteLine(string.Format("CSWSyncReadVarsFromPlcEnd {0}", _StationId));
            if (Result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                for (uint VarID = 0; VarID < NrVar; VarID++) {
                    Result = GetReadedValue(PlcValueReadingMode.Sync, VarID, _SyncReadVarSize[VarID], out value);
                    if (Result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)                                            
                        varValues.Add(value);                    
                    else
                        break;                    
                }
                // once used, release it
                CSWSyncReadVarsFromPlcReleaseValues(_StationId);

                //once used, release list of tags in codesys
                CSWReleaseSyncReadedVars(_StationId);            
            }

            if (Result != CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                varValues = null;

            return Result;
        }

        /// <summary>
        /// Define a list of tags that CoDeSys automatically cycle every ulUpdateRate (ms); results can be accessed by CycReadVars or by callback
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="ulUpdateRate"></param>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors CycDefineVarList(List<VarParam> vars, uint ulUpdateRate)
        {
            CoDeSysProtocol.PLCHandlerErrors Result;
            string VarList;

            GetReadVarsParameters(vars, out VarList, out _CycReadVarSize);

            //System.Diagnostics.Debug.WriteLine(string.Format("CSWCycDefineVarList {0}", _StationId));
            Result = (CoDeSysProtocol.PLCHandlerErrors)CSWCycDefineVarList(_StationId, VarList, (uint)vars.Count, ulUpdateRate);
            //System.Diagnostics.Debug.WriteLine(string.Format("CSWCycDefineVarListEnd {0}", _StationId));

            if (Result != CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                _CycReadVarSize = null;

            return Result;
        }

        public bool IsCycDefineVarListDefine()
        {
            return (_CycReadVarSize != null);
        }

        /// <summary>
        /// Delete list of tags used by cycling polling define previously by CycDefineVarList
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="ulUpdateRate"></param>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors CycDeleteVarList(CallingFunctions CalledFrom)
        {
            CoDeSysProtocol.PLCHandlerErrors Result;
            _CycReadVarSize = null;

            try
            {
                //System.Diagnostics.Debug.WriteLine(string.Format("CycDeleteVarList {0}", _StationId));
                Result = (CoDeSysProtocol.PLCHandlerErrors)CSWCycDeleteVarList(_StationId, 0);
                //System.Diagnostics.Debug.WriteLine(string.Format("CycDeleteVarListEnd {0}", _StationId));
            }
            catch (Exception ex)
            {
                log.Debug(String.Format(Properties.Resources.ExceptionCatched, "CycDeleteVarList", CalledFrom.ToString()), ex);
                Result = CoDeSysProtocol.PLCHandlerErrors.RESULT_FAILED;
            }

            return Result;
        }

        public CoDeSysProtocol.PLCHandlerErrors CycReadVars(out List<VarValue> varValues)
        {
            CoDeSysProtocol.PLCHandlerErrors Result;

            varValues = null;

            //System.Diagnostics.Debug.WriteLine(string.Format("CycReadVars {0}", _StationId));
            Result = CycReadVars(null, out varValues);
            //System.Diagnostics.Debug.WriteLine(string.Format("CycReadVarsEnd {0}", _StationId));

            return Result;
        }

        /// <summary>
        /// Execute a CyclingRead of previously define tags (by CycDefineVarList) from CoDeSys; if varList != null, only a subset of it
        /// </summary>
        /// <param name="varValues"></param>
        /// <param name="startVarID"></param>
        /// <param name="NrVars"></param>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors CycReadVars(List<VarParam> varList,  out List<VarValue> varValues)
        {
            CoDeSysProtocol.PLCHandlerErrors Result = CoDeSysProtocol.PLCHandlerErrors.RESULT_FAILED;
            VarValue value;
            uint NrVar;
            

            varValues = new List<VarValue>();

            //System.Diagnostics.Debug.WriteLine(string.Format("CSWCycReadVar {0}", _StationId));
            Result = (CoDeSysProtocol.PLCHandlerErrors)CSWCycReadVars(_StationId);
            //System.Diagnostics.Debug.WriteLine(string.Format("CSWCycReadVarEnd {0}", _StationId));
            if (Result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                if (varList == null) {
                    NrVar = (uint)_CycReadVarSize.Length;                    
                    for (uint VarID = 0; VarID < NrVar; VarID++)
                    {
                        //System.Diagnostics.Debug.WriteLine(string.Format("GetReadedValue {0}", _StationId));
                        Result = GetReadedValue(PlcValueReadingMode.Cycling, VarID, _CycReadVarSize[VarID], out value);
                        //System.Diagnostics.Debug.WriteLine(string.Format("GetReadedValueEnd {0}", _StationId));
                        if (Result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                            varValues.Add(value);
                    }

                } else {
                    
                    foreach (VarParam Var in varList)
                    {
                        //System.Diagnostics.Debug.WriteLine(string.Format("GetReadedValueEnd {0}", _StationId));
                        Result = GetReadedValue(PlcValueReadingMode.Cycling, (uint)Var.VarIndex, Var.VarSize, out value);
                        //System.Diagnostics.Debug.WriteLine(string.Format("GetReadedValueEnd {0}", _StationId));
                        if (Result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                            varValues.Add(value);
                    }
                }
            }

            if (Result != CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                varValues = null;

            return Result;
        }

        /// <summary>
        /// Execute a lock of vars into CoDeSys; requested by CoDeSys before execute CycReadVars
        /// </summary>
        /// <param name="plcValues"></param>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors CycEnterVarAccess(CallingFunctions CalledFrom)
        {
            CoDeSysProtocol.PLCHandlerErrors Result;

            try
            {
                //System.Diagnostics.Debug.WriteLine(string.Format("CycEnterVarAccess {0}", _StationId));
                Result = (CoDeSysProtocol.PLCHandlerErrors)CSWCycEnterVarAccess(_StationId);
                //System.Diagnostics.Debug.WriteLine(string.Format("CycEnterVarAccessEnd {0}", _StationId));
            }
            catch (Exception ex)
            {
                log.Debug(String.Format(Properties.Resources.ExceptionCatched, "CycEnterVarAccess", CalledFrom.ToString()), ex);
                Result = CoDeSysProtocol.PLCHandlerErrors.RESULT_FAILED;
            }

            return Result;
        }

        /// <summary>
        /// Release Execute a lock of vars into CoDeSys; requested by CoDeSys before execute CycReadVars
        /// </summary>
        /// <param name="plcValues"></param>
        /// <returns></returns>
        public void CycLeaveVarAccess(CallingFunctions CalledFrom)
        {
            try
            {
                //System.Diagnostics.Debug.WriteLine(string.Format("CycLeaveVarAccess {0}", _StationId));
                CSWCycLeaveVarAccess(_StationId);
                //System.Diagnostics.Debug.WriteLine(string.Format("CycLeaveVarAccessEnd {0}", _StationId));
            }
            catch(Exception ex)
            {
                log.Debug(String.Format(Properties.Resources.ExceptionCatched, "CycLeaveVarAccess", CalledFrom.ToString()), ex);
            }
        }


        public CoDeSysProtocol.PLCHandlerErrors GetCysReadCallBackInfo(out bool bCallBackUpdated, out uint nCallBackNotifyCounter)
        {
            bCallBackUpdated = false;
            nCallBackNotifyCounter = 0;
            return (CoDeSysProtocol.PLCHandlerErrors)CSWGetCysReadCallBackInfo(_StationId, out bCallBackUpdated, out nCallBackNotifyCounter);
        }

        /// <summary>
        /// Execute a Sync write operations of a list of tags
        /// </summary>
        /// <param name="plcValues"></param>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors SyncWriteVarValues(List<VarWrite> varValues)
        {
            uint NrVar = (uint)varValues.Count;

            CoDeSysProtocol.PLCHandlerErrors Result = ((CoDeSysProtocol.PLCHandlerErrors)CSWSyncWriteInitValues(_StationId, NrVar));
            if (Result== CoDeSysProtocol.PLCHandlerErrors.RESULT_OK) {

                for (uint VarID = 0; VarID < NrVar; VarID++)
                {
                    VarWrite Var = varValues[(int)VarID];
                    
                    IntPtr ptrData = new IntPtr();

                    // alloc a block of (unmanaged) memory to exchange data with c++ CoDeSysWrapper
                    ptrData = Marshal.AllocHGlobal(Var.VarSize);
                    
                    Marshal.Copy(Var.Data, 0, ptrData, Var.VarSize);

                    CSWSyncWriteAddValue(_StationId, VarID, Var.VarName, ptrData, Var.VarSize);

                    //release allocated memory
                    Marshal.FreeHGlobal(ptrData);
                }
                Result = (CoDeSysProtocol.PLCHandlerErrors)CSWSyncWriteVarsToPlc(_StationId);
            }

            return Result;
        }

        //public void TestBufferRead()
        //{
        //    int nSize = 5;
        //    IntPtr ptrData = new IntPtr();

        //    // here's where you need to know how big the array is !
        //    ptrData = Marshal.AllocHGlobal(nSize);

        //    //Marshal.Copy(ptrData, abData, 0, nSize);

        //    int Result = PHTestBufferRead(ref ptrData, nSize);

        //    byte byte0 = Marshal.ReadByte(ptrData, 0);
        //    byte byte1 = Marshal.ReadByte(ptrData, 1);
        //    byte byte2 = Marshal.ReadByte(ptrData, 2);
        //    byte byte3 = Marshal.ReadByte(ptrData, 3);
        //    byte byte4 = Marshal.ReadByte(ptrData, 4);

        //    // we copy the memory into .NET memory
        //    byte[] abData = new byte[nSize];

        //    Marshal.Copy(ptrData, abData, 0, nSize);

        //    Marshal.FreeHGlobal(ptrData);
        //}

        #endregion

        #region IDisposable Interface
        public void Dispose()
        {
            lock (_LockObject)
            {
                if (_Disposed)
                    return;

                _Disposed = true;
            }
        }
        #endregion
    }
}
