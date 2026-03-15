using System;
using System.Runtime.InteropServices;
using System.Collections.Generic;
using System.Linq;
using log4net;
using System.Threading;
using System.IO;

namespace CoDeSys
{
    public class CoDeSysPLCHandlerWrapper : IDisposable
    {
        #region Api calls to CoDeSys wrapper library
        #region Object instace functions
        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWIsWrapperInstalled", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe bool CSWIsWrapperInstalled();

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWIsCoDeSysInstalled", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe bool CSWIsCoDeSysInstalled();

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWIsPlcHandlerInitialized", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool CSWIsPlcHandlerInitialized(int stationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWDeletePLCHandler", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWDeletePLCHandler(int stationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWInit", CallingConvention = CallingConvention.Cdecl)]
        private static extern void CSWInit();

        public const int GET_NEW_STATION_ID = 0;
        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWCreateStation", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWCreateStation(ref int stationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWNumActiveStations", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWNumActiveStations();

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWReleaseStation", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWReleaseStation(int stationID);
        #endregion

        #region Various
        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetLastError", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWGetLastError(int stationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetState", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWGetState(int stationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWDisconnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWDisconnect(int stationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetVarListFromPLC", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWGetVarListFromPLC(int stationID, ref IntPtr ptrSymbols, out uint ulNumOfSymbols);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetItem", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWGetItem(int stationID, string pszSymbol, ref IntPtr ptrSymbols);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWConnect", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWConnect(int nStationID, ulong nTypeConnection, string GatewayAddress, string PLCAddress, ulong port, string username, string passwordPLC, string GatewayPassword, ulong timeOut, ulong ulNumTries, string logFile);
        #endregion

        #region Sync read functions
        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWSyncReadVarsFromPlc", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWSyncReadVarsFromPlc(int nStationID, string pszSymbols, uint ulNumOfSymbols);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWSyncReadVarsRelease", CallingConvention = CallingConvention.Cdecl)]

        private static extern unsafe int CSWSyncReadVarsRelease(int nStationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetSyncReadedVarFromPlc", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWGetSyncReadedVarFromPlc(int nStationID, uint ulSymbolNr, uint nSymbolSize, IntPtr ptrData, out uint ulTimeStamp, out byte bQuality);
        #endregion

        #region Cycling Read functions
        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWCycDefineVarList", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWCycDefineVarList(int nStationID, string pszSymbols, /*[In]*/ uint ulNumOfSymbols, uint ulUpdateRate);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWCycReadVars", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWCycReadVars(int nStationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWCycReadVarsRelease", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe void CSWCycReadVarsRelease(int nStationID);

        //[DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWCycEnterVarAccess", CallingConvention = CallingConvention.Cdecl)]
        //private static extern unsafe int CSWCycEnterVarAccess(int nStationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWCycDeleteVarList", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWCycDeleteVarList(int stationID);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWSetCycVarAccessKeepAlive", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWSetCycVarAccessKeepAlive(int stationID, int bKeepalive);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetCycReadedVar", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWGetCycReadedVar(int nStationID, uint ulSymbolNr, uint nSymbolSize, IntPtr ptrData, out uint ulTimeStamp, out byte bQuality);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWGetCysReadCallBackInfo", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWGetCysReadCallBackInfo(int nStationID, out bool bCallBackUpdated, out uint nCallBackNotifyCounter);

        //[DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWConnectionErrorOccured", CallingConvention = CallingConvention.Cdecl)]
        //private static extern unsafe bool CSWConnectionErrorOccured(int nStationID);
        #endregion

        #region Write functions
        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWSyncWriteInitValues", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWSyncWriteInitValues(int nStationID, uint ulNumVarValues);

        [DllImport("CoDeSysWrapper.dll", EntryPoint = "CSWSyncWriteDeleteValues", CallingConvention = CallingConvention.Cdecl)]
        private static extern unsafe int CSWSyncWriteDeleteValues(int nStationID);        

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
                Array.Copy(data, Data, data.Length);
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

            public void SetInError()
            {
                VarIndex = -1;
            }
        }

        public class VarValue
        {
            public const byte QUALITY_GOOD = 1;
            public const byte QUALITY_BAD = 0;

            public bool UnMappedVar { set; get; }
            public byte Quality { set; get; }
            public uint TimeStamp { set; get; }
            public byte[] Data;

            public VarValue(uint dataSize)
            {
                UnMappedVar = false;
                Quality = QUALITY_BAD;
                TimeStamp = 0;
                if (dataSize <= 0)
                    Data = null;
                else
                    Data = new byte[dataSize];
            }

            public bool HasData()
            {
                return (Data != null && Quality == QUALITY_GOOD && !UnMappedVar);
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

        #region Constructors
        public CoDeSysPLCHandlerWrapper()
        {
            _StationId = STATION_ID_INVALID;
        }
        #endregion

        #region Properties

        private uint[] _CycReadVarSize;

        private int _StationId;
        public int StationId
        {
            get { return _StationId; }
            set { _StationId = value; }
        }
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
                Result = CSWIsCoDeSysInstalled();
            }
            catch (Exception ex) { }

            return Result;
        }

        /// <summary>
        /// Init internal object inside library; each client must call it 
        /// </summary>
        public void Init()
        {
            CSWInit();
        }

        /// <summary>
        /// Create CoDeSys object instance
        /// </summary>        
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors Create()
        {
            CoDeSysProtocol.PLCHandlerErrors result;

            if (_StationId != STATION_ID_INVALID)
                Release();

            result = CreateStation();
            
            return result;
        }

        /// <summary>
        /// Retrive unique station ID (one for each client)
        /// </summary>        
        /// <returns></returns>
        private CoDeSysProtocol.PLCHandlerErrors CreateStation(int newStationID = GET_NEW_STATION_ID)
        {            
            CoDeSysProtocol.PLCHandlerErrors result = (CoDeSysProtocol.PLCHandlerErrors)CSWCreateStation(ref newStationID);
            if (result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                _StationId = newStationID;
                // force anyway cycle list reading parameter
                CSWSetCycVarAccessKeepAlive(_StationId, 0);
            }
            else
            {
                _StationId = STATION_ID_INVALID;
            }

            return result;
        }

        /// <summary>
        /// Release internal resource of CoDeSysWrapper api associated to ID
        /// </summary>
        public void Release()
        {
            if (_StationId != STATION_ID_INVALID)
            {
                CSWReleaseStation(_StationId);

                _StationId = STATION_ID_INVALID;                
            }
        }

        public CoDeSysProtocol.PLCHandlerErrors CheckConnectionAndRunningState(string ip, ulong nTypeConnection, string address, ulong port, string username, string passwordPLC, string gatewayPassword, int timeout, int ulNumTries, CoDeSysChannel ch = null)
        {
            CoDeSysProtocol.PLCHandlerErrors ret = CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;

            if (IsPlcHandlerInitialized())
            {
                CoDeSysProtocol.PLCHandlerState state = GetState();
                switch (state)
                {
                    case CoDeSysProtocol.PLCHandlerState.STATE_RUNNING:
                        return CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;

                    case CoDeSysProtocol.PLCHandlerState.STATE_PLC_CONNECTED:
                    case CoDeSysProtocol.PLCHandlerState.STATE_SYMBOLS_LOADED:
                        //if (stopWorkerThread != null)
                        //    stopWorkerThread.WaitOne((int)timeout);

                        //if (GetState() == CoDeSysProtocol.PLCHandlerState.STATE_RUNNING)
                        //    return CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;
                        //else
                        //    return CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_NOT_CONN
                        //    return CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_NOT_CONNECTED;
                        return CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_NOT_CONNECTED;

                    // wait library automatic reconnection
                    case CoDeSysProtocol.PLCHandlerState.STATE_PLC_CONNECT_ERROR:
                    case CoDeSysProtocol.PLCHandlerState.STATE_PLC_NOT_CONNECTED:
                    case CoDeSysProtocol.PLCHandlerState.STATE_DISCONNECT:
                        //if (stopWorkerThread != null)
                        //    stopWorkerThread.WaitOne((int)timeout);
                        //return CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_NOT_CONNECTED;
                        return CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_NOT_CONNECTED;

                    default:
                        //case STATE_PLC_NOT_CONNECTED_SYMBOLS_LOADED:  
                        //case STATE_NO_CONFIGURATION:
                        //case STATE_NO_SYMBOLS:
                        //case STATE_DISCONNECT:
                        //case STATE_PLC_NOT_CONNECTED:
                        ret = (CoDeSysProtocol.PLCHandlerErrors)CSWDisconnect(_StationId);

                        if (ch != null)
                            ch.ResetAllJobsCoDeSysParameters();

                        if (ret == CoDeSysProtocol.PLCHandlerErrors.RESULT_COMM_FATAL)
                            ret = CreateStation(_StationId);

                        // try to riestablish connection using CSWConnect
                        break;
                }
            }

            string logFile = String.Empty;
            if (ch != null)
                logFile = ch.GetLogFileForWrapper();
                
            ret = (CoDeSysProtocol.PLCHandlerErrors)CSWConnect(_StationId, nTypeConnection, ip, address, port, username, passwordPLC, gatewayPassword, (ulong)timeout, (ulong)ulNumTries, logFile);
                        
            return ret;
        }

        /// <summary>
        /// Get Last state of CoDeSys PLCHandler (contain also connection state)
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerState GetState()
        {
            return (CoDeSysProtocol.PLCHandlerState)CSWGetState(_StationId);
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

            vars = null;

            CoDeSysProtocol.PLCHandlerErrors Result = (CoDeSysProtocol.PLCHandlerErrors)CSWGetVarListFromPLC(_StationId, ref ptrData, out uint NrSymbols);
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

            Result = (CoDeSysProtocol.PLCHandlerErrors)CSWGetItem(_StationId, varName, ref ptrData);
            if (Result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                unsafe
                {
                    try
                    {
                        tagInfo = (PlcSymbolDesc)Marshal.PtrToStructure(ptrData, typeof(PlcSymbolDesc));
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
                uint ulTimeStamp = 0;
                byte bQuality = 0;

                // alloc a block of (unmanaged) memory to exchange data with c++ CoDeSysWrapper
                //ptrData = Marshal.AllocHGlobal((int)varSize);
                ptrData = Marshal.AllocCoTaskMem((int)varSize);                

                //System.Diagnostics.Debug.WriteLine(string.Format("GetReadedValue {0}", _StationId));
                // depending of request mode
                if (mode == PlcValueReadingMode.Sync)
                    Result = (CoDeSysProtocol.PLCHandlerErrors)CSWGetSyncReadedVarFromPlc(_StationId, varID, varSize, ptrData, out ulTimeStamp, out bQuality);
                else
                    Result = (CoDeSysProtocol.PLCHandlerErrors)CSWGetCycReadedVar(_StationId, varID, varSize, ptrData, out ulTimeStamp, out bQuality);

                if (Result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                {
                    try
                    {
                        // if good data, copy from unmanged memory (allocated in C#) to struct byte[]
                        if (bQuality == 1)
                        {
                            Marshal.Copy(ptrData, value.Data, 0, (int)varSize);
                            value.Quality = bQuality;
                            value.TimeStamp = ulTimeStamp;
                        }
                    }
                    catch
                    {
                        bQuality = 0;
                    }
                }

                if (Result != CoDeSysProtocol.PLCHandlerErrors.RESULT_OK || bQuality == VarValue.QUALITY_BAD)
                {
                    value.UnMappedVar = (Result != CoDeSysProtocol.PLCHandlerErrors.RESULT_READVALUE_UNMAPPED_VAR);
                    value.Quality = VarValue.QUALITY_BAD;
                    value.TimeStamp = 0;
                    value.ResetData();
                }

                //release allocated memory
                //Marshal.FreeHGlobal(ptrData);
                Marshal.FreeCoTaskMem(ptrData);
            } else {
                value.Quality = VarValue.QUALITY_BAD;
                value.TimeStamp = 0;
                value.ResetData();
            }

            return CoDeSysProtocol.PLCHandlerErrors.RESULT_OK;
        }

        /// <summary>
        /// Execute a Sync read from CoDeSys of listed tags
        /// </summary>
        /// <param name="varList"></param>
        /// <param name="varValues"></param>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors SyncReadVarsFromPlc(List<VarParam> varList, out List<VarValue> varValues)
        {            
            GetReadVarsParameters(varList, out string varListToString, out uint[] SyncReadVarSize);

            uint nrVar = (uint)varList.Count;
            varValues = new List<VarValue>();

            // Execute Sync read to CodeSysWrapper of listed tags (tags list are send as a single string where tag's name are separated by special character)
            CoDeSysProtocol.PLCHandlerErrors Result = (CoDeSysProtocol.PLCHandlerErrors)CSWSyncReadVarsFromPlc(_StationId, varListToString, nrVar);
            if (Result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                for (uint varID = 0; varID < nrVar; varID++)
                {
                    Result = GetReadedValue(PlcValueReadingMode.Sync, varID, SyncReadVarSize[varID], out VarValue value);
                    if (Result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                        varValues.Add(value);
                    else
                        varValues.Add(new VarValue(0) { Quality = VarValue.QUALITY_BAD });
                }

                //once used, release list of tags in codesys
                Result = (CoDeSysProtocol.PLCHandlerErrors)CSWSyncReadVarsRelease(_StationId);
            }
            else
            {
                for (uint varID = 0; varID < nrVar; varID++)
                    varValues.Add(new VarValue(0) { Quality = VarValue.QUALITY_BAD });
            }            

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

        public void ResetCycDefineVarListPointer()
        {
            _CycReadVarSize = null;
        }

        /// <summary>
        /// Delete list of tags used by cycling polling define previously by CycDefineVarList
        /// </summary>
        /// <param name="tags"></param>
        /// <param name="ulUpdateRate"></param>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors CycDeleteVarList()
        {
            CoDeSysProtocol.PLCHandlerErrors Result;
            
            _CycReadVarSize = null;

            Result = (CoDeSysProtocol.PLCHandlerErrors)CSWCycDeleteVarList(_StationId);             
            
            return Result;
        }

        /// <summary>
        /// Execute a CyclingRead of previously define tags (by CycDefineVarList) from CoDeSys; if varList != null, only a subset of it
        /// </summary>
        /// <param name="varValues"></param>
        /// <param name="startVarID"></param>
        /// <param name="NrVars"></param>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors CycReadVars(List<VarParam> varList, out List<VarValue> varValues)
        {
            CoDeSysProtocol.PLCHandlerErrors result = CoDeSysProtocol.PLCHandlerErrors.RESULT_FAILED;            

            varValues = new List<VarValue>();

            // if list of item present
            if (IsCycDefineVarListDefine())
                result = (CoDeSysProtocol.PLCHandlerErrors)CSWCycReadVars(_StationId);
            else
                result = CoDeSysProtocol.PLCHandlerErrors.RESULT_PLC_NO_CYCLIC_LIST_DEFINED;

            //System.Diagnostics.Debug.WriteLine(string.Format("CSWCycReadVarEnd {0}", _StationId));
            if (result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                foreach (VarParam Var in varList)
                {
                    //System.Diagnostics.Debug.WriteLine(string.Format("GetReadedValueEnd {0}", _StationId));
                    result = GetReadedValue(PlcValueReadingMode.Cycling, (uint)Var.VarIndex, Var.VarSize, out VarValue value);
                    //System.Diagnostics.Debug.WriteLine(string.Format("GetReadedValueEnd {0}", _StationId));
                    if (result == CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
                        varValues.Add(value);
                    else
                        varValues.Add(new VarValue(0) { Quality = VarValue.QUALITY_BAD });
                }
            }
            else
            {
                foreach (VarParam Var in varList)
                    varValues.Add(new VarValue(0) { Quality = VarValue.QUALITY_BAD });
            }

            //once used, release list of tags in codesys            
            CSWCycReadVarsRelease(_StationId);

            return result;
        }

        ///// <summary>
        ///// Execute a lock of vars into CoDeSys; requested by CoDeSys before execute CycReadVars
        ///// </summary>
        ///// <param name="plcValues"></param>
        ///// <returns></returns>
        //public CoDeSysProtocol.PLCHandlerErrors CycEnterVarAccess(CallingFunctions CalledFrom)
        //{
        //    CoDeSysProtocol.PLCHandlerErrors Result;

        //    try
        //    {
        //        //System.Diagnostics.Debug.WriteLine(string.Format("CycEnterVarAccess {0}", _StationId));
        //        Result = (CoDeSysProtocol.PLCHandlerErrors)CSWCycEnterVarAccess(_StationId);
        //        //System.Diagnostics.Debug.WriteLine(string.Format("CycEnterVarAccessEnd {0}", _StationId));
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Debug(String.Format(Properties.Resources.ExceptionCatched, "CycEnterVarAccess", CalledFrom.ToString()), ex);
        //        Result = CoDeSysProtocol.PLCHandlerErrors.RESULT_FAILED;
        //    }

        //    return Result;
        //}

        ///// <summary>
        ///// Release Execute a lock of vars into CoDeSys; requested by CoDeSys before execute CycReadVars
        ///// </summary>
        ///// <param name="plcValues"></param>
        ///// <returns></returns>
        //public void CycLeaveVarAccess(CallingFunctions CalledFrom)
        //{
        //    try
        //    {
        //        //System.Diagnostics.Debug.WriteLine(string.Format("CycLeaveVarAccess {0}", _StationId));
        //        CSWCycLeaveVarAccess(_StationId);
        //        //System.Diagnostics.Debug.WriteLine(string.Format("CycLeaveVarAccessEnd {0}", _StationId));
        //    }
        //    catch (Exception ex)
        //    {
        //        log.Debug(String.Format(Properties.Resources.ExceptionCatched, "CycLeaveVarAccess", CalledFrom.ToString()), ex);
        //    }
        //}

        public CoDeSysProtocol.PLCHandlerErrors GetCysReadCallBackInfo(out bool bCallBackUpdated, out uint nCallBackNotifyCounter)
        {
            bCallBackUpdated = false;
            nCallBackNotifyCounter = 0;
            return (CoDeSysProtocol.PLCHandlerErrors)CSWGetCysReadCallBackInfo(_StationId, out bCallBackUpdated, out nCallBackNotifyCounter);
        }

        //public bool CSWConnectionErrorOccured()
        //{
        //    return CSWConnectionErrorOccured(_StationId);
        //}

        /// <summary>
        /// Execute a Sync write operations of a list of tags
        /// </summary>
        /// <param name="plcValues"></param>
        /// <returns></returns>
        public CoDeSysProtocol.PLCHandlerErrors SyncWriteVarValues(List<VarWrite> varValues)
        {
            uint NrVar = (uint)varValues.Count;

            CoDeSysProtocol.PLCHandlerErrors Result = ((CoDeSysProtocol.PLCHandlerErrors)CSWSyncWriteInitValues(_StationId, NrVar));
            if (Result== CoDeSysProtocol.PLCHandlerErrors.RESULT_OK)
            {
                for (uint VarID = 0; VarID < NrVar; VarID++)
                {
                    VarWrite Var = varValues[(int)VarID];
                    
                    IntPtr ptrData = new IntPtr();

                    // alloc a block of (unmanaged) memory to exchange data with c++ CoDeSysWrapper
                    //ptrData = Marshal.AllocHGlobal(Var.VarSize);
                    ptrData = Marshal.AllocCoTaskMem(Var.VarSize);

                    Marshal.Copy(Var.Data, 0, ptrData, Var.VarSize);

                    CSWSyncWriteAddValue(_StationId, VarID, Var.VarName, ptrData, Var.VarSize);

                    //release allocated memory
                    //Marshal.FreeHGlobal(ptrData);
                    Marshal.FreeCoTaskMem(ptrData);
                }
                Result = (CoDeSysProtocol.PLCHandlerErrors)CSWSyncWriteVarsToPlc(_StationId);

                CSWSyncWriteDeleteValues(_StationId);
            }

            return Result;
        }

        public void DeletePLCHandler()
        {
            CSWDeletePLCHandler(_StationId);
        }

        public bool IsPlcHandlerInitialized()
        {
            return CSWIsPlcHandlerInitialized(_StationId);
        }

        public bool IsStationCreated()
        {
            return (_StationId != STATION_ID_INVALID);
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
            Release();
        }
        #endregion
    }
}
