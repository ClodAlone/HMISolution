using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using System.Text;
using Opc.Ua;
using DriverCodeBase.Enumerators;
using System.Text.RegularExpressions;

namespace DICom
{
    public class DIComProtocol
    {
        public const string ALL_CLIENTS_ALLOWED = "";
        public const int MAX_VAR_NAME_SIZE = PDU.CONFIG_VAR_NAME_LENGHT;
        public const int DEFAULT_TCP_PORT = 65100;
        public const int PROTOCOL_ERROR = 1500;
        public const int STRING_MAX_SIZE = 1024;
        public const uint DEFAULT_DATA_COLLECTING_TIMEOUT = 1500;
        public const int RECEIVE_BUFFER_SECURITY_SIZE = 5000;

        public enum ExpandResult
        {
            None,
            Ok,
            MessageParsingError,
            MessageIncomplete,
            EmptyVarDefinition
        }

        public enum DiCommVarType : int
        {            
            TYPE_BOOL = 0, // boolean [True = 0x01, False = 0x00]
            TYPE_BYTE = 2, //unsigned short integer [0..255]
            TYPE_WORD = 3, //unsigned integer [0..65535]
            TYPE_DWORD = 4, //unsigned double integer [0..232]
            TYPE_LWORD = 5, //unsigned long integer [0..264]
            TYPE_SINT = 6, //signed short integer [-128..127]
            TYPE_INT = 7, //signed integer [-32768..32767]
            TYPE_DINT = 8, //signed double integer [-231..(231-1)]
            TYPE_LINT = 9, //signed long integer [-263..(263-1)]
            TYPE_USINT = 10, //unsigned short integer [0..255]
            TYPE_UINT = 11, //unsigned integer [0..65535]
            TYPE_UDINT = 12, //unsigned double integer [0..232]
            TYPE_ULINT = 13, //unsigned long integer [0..264]
            TYPE_REAL = 14, //single precision floating point number as defined in IEEE754, 32 bits
            TYPE_LREAL = 15, // double precision floating point number as defined in IEEE754, 64 bits
            TYPE_STRING = 16, //one-dimensional array of ASCII characters (1 byte) terminated by a null character ‘\0’
            TYPE_WSTRING = 17, //one-dimensional array of UNICODE characters (2 bytes) terminated by a null character ‘\0\0’
            TYPE_ENUM = 25, // enumerated type --> unsupported
            TYPE_ARRAY = 26, //array--> unsupported
            TYPE_USERDEF = 28 //28 --> unsupported
        }

        public enum LoadType : byte
        {
            NONE = 0, 
            CONFIG = 1, // Variables configuration(size, type, tag, ...). 
            DATA = 2, //Process & control data. Load size depends on the configuration.
            KEEP_ALIVE = 3
        }

        public enum ReceiveStates
        {
            Init,
            WaitHeader,
            WaitData,
        }

        public enum FunctionMode
        {
            Standard,   // driver's publish changed paraneters when data arrived
            PublishMessageDataWithSameDateTime,    // driver's publish all changed parameters (inside message) when data arrived with the same SourceTimeStamp
            ForcePublishCollectedMessagesDataWithSameDateTime,    // driver's collect message until all expected (defined by config messages) arrived, and publish all parameters (changed or not) with the same SourceTimeStamp
            ConfigMode    // driver's collect config record for external application
        }

        // Client command state variable custom bit
        public enum ClientCommandState : ushort
        {
            Undefined = 0,
            Connected = 1,
            Disconnected = 2
        }

        public class ReceiveItem : ICloneable
        {
            public DIComProtocol.PDU Pdu { set; get; }            
            public bool IsValid { set; get; }            
            public HEADER Header { set; get; }
            public string IpAddress { set; get; }

            public DIComProtocol.ReceiveStates ReceiveState { get; set; }

            public int ExpectedClientMessageSize { get; set; } = 0;

            public ReceiveItem()
            {
                Init(string.Empty);
            }

            public void Init(string clientIpAddress)
            {
                Pdu = new DIComProtocol.PDU();
                IsValid = false;
                Header = null;                
                IpAddress = clientIpAddress;
                ReceiveState = ReceiveStates.Init;
                //do not reset header this value
                //ExpectedClientMessageSize = 0;
            }

            public void CleanUpRawData()
            {
                if (Header != null)
                    Header.RawData.Clear();

                if (Pdu != null)
                    Pdu.RawData.Clear();
            }

            public List<byte> GetRawData()
            {
                List<byte> l = new List<byte>();
                    
                if (Header != null)
                    l.AddRange(Header.RawData);

                if (Pdu != null)
                    l.AddRange(Pdu.RawData);

                return l;
            }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        public enum DIComErrorCodes : int
        {
            ErrorReadError = PROTOCOL_ERROR,
            ErrorReceiveFrameError = PROTOCOL_ERROR + 1,
            ErrorWrongSize = PROTOCOL_ERROR + 2,
            ErrorProtocolIllegalDataAddress = DIComProtocol.PROTOCOL_ERROR + 3,
            ErrorProtocolIllegalDataValue = DIComProtocol.PROTOCOL_ERROR + 4,
            ErrorProtocolSlaveDeviceFailure = DIComProtocol.PROTOCOL_ERROR + 5,
            ErrorProtocolAcknowledge = DIComProtocol.PROTOCOL_ERROR + 6,
            ErrorProtocolSlaveDeviceBusy = DIComProtocol.PROTOCOL_ERROR + 7,
            ErrorVarNotCollected = DIComProtocol.PROTOCOL_ERROR + 8,
            ErrorChannelFailedToStart = DIComProtocol.PROTOCOL_ERROR + 9
        }

        public static class BufferExpand
        {
            public static byte toByte(ref byte[] DataArray, ref int Offset)
            {
                checkSize(DataArray, Offset, 1);
                byte outval = DataArray[Offset];
                Offset += 1;
                return outval;
            }
            public static UInt16 toUInt16(ref byte[] DataArray, ref int Offset)
            {
                checkSize(DataArray, Offset, 2);
                UInt16 outval = BitConverter.ToUInt16(DataArray, Offset);
                Offset += 2;
                return outval;
            }
            public static UInt32 toUInt32(ref byte[] DataArray)
            {
                int Offset = 0;
                return toUInt32(ref DataArray, ref Offset);
            }
            public static UInt32 toUInt32(ref byte[] DataArray, ref int Offset)
            {
                checkSize(DataArray, Offset, 4);
                UInt32 outval = BitConverter.ToUInt16(DataArray, Offset);
                Offset += 4;
                return outval;
            }
            public static float toFloat(ref byte[] DataArray)
            {
                checkSize(DataArray, 0, 4);
                float outval = BitConverter.ToSingle(swapArray(DataArray), 0);
                return outval;
            }
            public static double toDouble(ref byte[] DataArray)
            {
                checkSize(DataArray, 0, 8);
                double outval = BitConverter.ToDouble(swapArray(DataArray), 0);
                return outval;
            }
            public static string toString(ref byte[] DataArray, ref int offset, int lenght)
            {
                string result = Encoding.UTF8.GetString(DataArray, offset, lenght).Trim('\0');
                offset += lenght;
                return result;
            }
            public static byte[] toArray(ref byte[] DataArray, ref int Offset, UInt32 lenght)
            {
                checkSize(DataArray, Offset, lenght);
                byte[] outBuffer = new byte[lenght];
                if (lenght != 0)
                    for (UInt16 i = 0; i < lenght; i++)
                        outBuffer[i] = DataArray[Offset++];
                return outBuffer;
            }
            public static byte[] toArray(UInt16 inValue)
            {
                return BitConverter.GetBytes(Swap(inValue));
            }
            public static byte[] toArray(UInt32 inValue)
            {
                return swapArray(BitConverter.GetBytes(inValue));
            }
            public static byte[] toArray(float value)
            {
                return swapArray(BitConverter.GetBytes(value));
            }
            public static byte[] toArray(double value)
            {
                return swapArray(BitConverter.GetBytes(value));
            }
            public static byte[] toArray(string value)
            {
                List<byte> retVal = new List<byte>();
                retVal.Add(0);
                retVal.AddRange(Encoding.UTF8.GetBytes(value));
                return retVal.ToArray();
            }
            public static UInt32 getUInt32(byte[] inBuffer)
            {
                UInt32 outval = 0;
                if (inBuffer.Length != 0)
                    for (UInt16 i = 0; i < inBuffer.Length; i++)
                        outval = (outval << 8) + inBuffer[i];
                return outval;
            }
            public static byte[] getArray(UInt32 value)
            {
                UInt32 localValue = value;
                List<byte> retVal = new List<byte>();
                do
                {
                    retVal.Add((byte)(localValue & 0xff));
                    localValue >>= 8;
                } while (localValue != 0);

                return swapArray(retVal.ToArray());
            }
            public static UInt16 Swap(UInt16 inValue)
            {
                return (UInt16)((inValue >> 8) + (inValue << 8));
            }
            public static byte[] swapArray(byte[] inBuffer)
            {
                byte[] outBuffer = new byte[inBuffer.Length];
                if (inBuffer.Length != 0)
                    for (UInt16 i = 0; i < inBuffer.Length; i++)
                        outBuffer[i] = inBuffer[inBuffer.Length - 1 - i];
                return outBuffer;
            }
            public static void checkSize(byte[] DataArray, int Offset, UInt32 lenght)
            {
                if (Offset + lenght > DataArray.Count())
                    throw new NotImplementedException();
            }
        }

        public class DiComVar : ICloneable
        {            
            int _RecordID;
            public int RecordID
            {
                get { return _RecordID; }
                set { _RecordID = value; }
            }

            int _VarIndex;
            public int VarIndex
            {
                get { return _VarIndex; }
                set { _VarIndex = value; }
            }

            DiCommVarType _VarType;
            public DiCommVarType VarType
            {
                get { return _VarType; }
                set { _VarType = value; }
            }

            private ushort _VarSize;
            public ushort VarSize
            {
                get { return _VarSize; }
                set { _VarSize = value; }
            }

            private byte[] _VarValue;
            public byte[] VarValue
            {
                get { return _VarValue; }
                set { _VarValue = value; }
            }

            private string _VarName;
            public string VarName
            {
                get { return _VarName; }
                set { _VarName = value; }
            }

            private byte[] _Reserve;
            public byte[] Reserve
            {
                get { return _Reserve; }
                set { _Reserve = value; }
            }

            private string _Note;
            public string Note
            {
                get { return _Note; }
                set { _Note = value; }
            }

            private DriverErrorCodes _Error;
            public DriverErrorCodes Error
            {
                get { return _Error; }
                set { _Error = value; }
            }

            public string GetVarValue()
            {
                string result = null;

                try {
                    switch (_VarType)
                    {
                        case DiCommVarType.TYPE_BOOL:
                            result = "";
                            break;
                        case DiCommVarType.TYPE_BYTE:
                            result = _VarValue[0].ToString();
                            break;
                        case DiCommVarType.TYPE_WORD:
                            result = BitConverter.ToUInt16(_VarValue, 0).ToString();
                            break;
                        case DiCommVarType.TYPE_DWORD:
                            result = BitConverter.ToUInt32(_VarValue, 0).ToString();
                            break;
                        case DiCommVarType.TYPE_LWORD:
                            result = BitConverter.ToUInt64(_VarValue, 0).ToString();
                            break;
                        case DiCommVarType.TYPE_SINT:
                            result = _VarValue[0].ToString();
                            break;
                        case DiCommVarType.TYPE_INT:
                            result = BitConverter.ToInt16(_VarValue, 0).ToString();
                            break;
                        case DiCommVarType.TYPE_DINT:
                            result = BitConverter.ToInt32(_VarValue, 0).ToString();
                            break;
                        case DiCommVarType.TYPE_LINT:
                            result = BitConverter.ToUInt64(_VarValue, 0).ToString();
                            break;
                        case DiCommVarType.TYPE_USINT:
                            result = _VarValue[0].ToString();
                            break;
                        case DiCommVarType.TYPE_UINT:
                            result = BitConverter.ToUInt16(_VarValue, 0).ToString();
                            break;
                        case DiCommVarType.TYPE_UDINT:
                            result = BitConverter.ToInt32(_VarValue, 0).ToString();
                            break;
                        case DiCommVarType.TYPE_ULINT:
                            result = BitConverter.ToUInt64(_VarValue, 0).ToString();
                            break;
                        case DiCommVarType.TYPE_REAL:
                            result = BitConverter.ToSingle(_VarValue, 0).ToString();
                            break;
                        case DiCommVarType.TYPE_LREAL:
                            result = BitConverter.ToDouble(_VarValue, 0).ToString();
                            break;
                        case DiCommVarType.TYPE_STRING:
                            result = Encoding.UTF8.GetString(_VarValue, 0, _VarValue.Count()).Trim('\0');
                            break;
                        case DiCommVarType.TYPE_WSTRING:
                            result = Encoding.UTF32.GetString(_VarValue, 0, _VarValue.Count()).Trim('\0');
                            break;
                        case DiCommVarType.TYPE_ENUM:
                            result = "Usupported TYPE_ENUM data type";
                            break;
                        case DiCommVarType.TYPE_ARRAY:
                            result = "Usupported TYPE_ARRAY data type";
                            break;
                        case DiCommVarType.TYPE_USERDEF:
                            result = "Usupported TYPE_USERDEF data type";
                            break;
                    }
                } 
                catch (Exception ex)
                {
                   result = "Invalid var value";
                }

                return result;
            }

            public bool IsVarTypeString()
            {
                return (_VarType == DiCommVarType.TYPE_STRING || _VarType == DiCommVarType.TYPE_WSTRING);                
            }

            public DiComVar()
            {
                _RecordID = 0;
                _VarIndex = 0;
                _VarType = DiCommVarType.TYPE_BOOL;
                _VarSize = 0;
                _VarName = string.Empty;
                _Reserve = null;
                _VarValue = null;
                _Note = string.Empty;
                _Error = DriverErrorCodes.ErrorNoError;
            }

            public DiComVar(DIComClientConfigVar var) : this()
            {
                _RecordID = var.RecordID;
                _VarIndex = var.VarIndex;
                _VarType = var.VarType;
                _VarSize = var.VarSize;
                _VarName = var.VarName;
                _Note = var.Note;
            }

            public object Clone()
            {
                return MemberwiseClone();
            }
        }

        public class HEADER
        {
            private const byte BUC_PREAMBLE_CHAR = 255;
            public const int BUC_PREAMBLE_SIZE = 12;
            private const int BUC_HEADER_SIZE = 8;
            private const byte RESERVE_SIZE = 4;
            public const byte SIZE = (BUC_PREAMBLE_SIZE + BUC_HEADER_SIZE);
            private const byte KEEP_ALIVE_SIZE = (BUC_PREAMBLE_SIZE + BUC_HEADER_SIZE);

            private byte[] HEADER_BUC_PREAMBLE = Enumerable.Repeat(BUC_PREAMBLE_CHAR, BUC_PREAMBLE_SIZE).ToArray();

            public HEADER()
            {
                Init();
            }

            #region Properties
            private List<byte> _RawData;
            public List<byte> RawData
            {
                get { return _RawData; }
                set { _RawData = value; }
            }

            private LoadType _LoadType;
            public LoadType LoadType
            {
                get { return _LoadType; }
                set { _LoadType = value; }
            }

            private ushort _LoadSize;
            public ushort LoadSize 
            { 
                get { return _LoadSize; } 
                set { _LoadSize = value; }
            }

            public ushort TotalLoadSize
            {
                get { 
                    switch (_LoadType)
                    {
                        case LoadType.CONFIG:
                        case LoadType.DATA:
                            return (ushort)(SIZE + _LoadSize);
                        case LoadType.KEEP_ALIVE:                                                    
                            return (ushort)(BUC_PREAMBLE_SIZE + _LoadSize);
                        default:
                            return (ushort)(SIZE + _LoadSize);

                    }                    
                }
            }

            public byte _RecordID;
            public byte RecordID
            { 
                get { return _RecordID; } 
                set { _RecordID = value; }
            }
            
            private byte[] _Reserve;
            private byte[] Reserve { 
                get { return _Reserve; }
                set { _Reserve = value; }
            }

            private ExpandResult _ExpandResult;
            public ExpandResult ExpandResult
            {
                get { return _ExpandResult; }
                set { _ExpandResult = value; }
            }

            private int _HeaderStartOffset;
            public int HeaderStartOffset
            {
                get { return _HeaderStartOffset; }
                set { _HeaderStartOffset = value; }
            }
            #endregion

            #region Methods
            private void Init()
            {
                _RawData = new List<byte>();
                _LoadType = LoadType.NONE;
                _LoadSize = 0;
                _RecordID = 0;
                _Reserve = new byte[RESERVE_SIZE];
                _ExpandResult = ExpandResult.None;
                _HeaderStartOffset = 0;
            }

            public byte[] Pack()
            {
                byte[] msg = new byte[0];
                if (_LoadType == LoadType.KEEP_ALIVE)
                {
                    Array.Resize(ref msg, KEEP_ALIVE_SIZE);
                    Array.Copy(HEADER_BUC_PREAMBLE, msg, BUC_PREAMBLE_SIZE);
                    msg[BUC_PREAMBLE_SIZE] = (byte)LoadType.KEEP_ALIVE;
                } 
                return msg;
            }

            private int GetHeaderStartPosition()
            {
                int position = -1;
                for (int i = 0; i < _RawData.Count - BUC_PREAMBLE_SIZE; i++)
                {
                    if (_RawData.GetRange(i, BUC_PREAMBLE_SIZE).SequenceEqual(HEADER_BUC_PREAMBLE))
                    {
                        position = i;
                        break;
                    }
                }

                return position;
            }

            public void Expand(ref byte[] dataArray)
            {            
                _RawData.Clear();
                _RawData.AddRange(dataArray);
                                
                _ExpandResult = ExpandResult.MessageIncomplete;
                _HeaderStartOffset = GetHeaderStartPosition();
                if (_HeaderStartOffset < 0)
                    return;

                if (dataArray.Length < (_HeaderStartOffset + DIComProtocol.HEADER.SIZE))
                    return;

                _ExpandResult = ExpandResult.MessageParsingError;

                int offset = _HeaderStartOffset + BUC_PREAMBLE_SIZE;

                // type of message
                _LoadType = (LoadType)dataArray[offset];
                offset++;
                
                if (!Enum.IsDefined(typeof(LoadType), _LoadType))
                    return;

                if (_LoadType == LoadType.KEEP_ALIVE)
                {
                    // short keep alive generated by simulator
                    if (dataArray.Length == (DIComProtocol.HEADER.BUC_PREAMBLE_SIZE + 1))
                    {
                        _LoadSize = (ushort)1;
                        _ExpandResult = ExpandResult.Ok;
                        return;
                    }
                    
                    if (dataArray.Length > (DIComProtocol.HEADER.BUC_PREAMBLE_SIZE + 1))
                    {
                        // new message ?
                        if (dataArray[(DIComProtocol.HEADER.BUC_PREAMBLE_SIZE + 2)-1] == BUC_PREAMBLE_CHAR)
                        {
                            _LoadSize = (ushort)1;
                            _ExpandResult = ExpandResult.Ok;
                            return;
                        }

                        _LoadSize = DIComProtocol.HEADER.BUC_HEADER_SIZE;
                        _ExpandResult = ExpandResult.Ok;
                        return;
                    }
                }                

                // offset will be increment inside toUInt16 method
                _LoadSize = BufferExpand.toUInt16(ref dataArray, ref offset);
                if (_LoadSize == 0)
                    return;

                _RecordID = dataArray[offset];
                offset++;

                _Reserve = new byte[RESERVE_SIZE];
                Array.Copy(dataArray, offset, _Reserve, 0, RESERVE_SIZE);
                offset += RESERVE_SIZE;

                _ExpandResult = ExpandResult.Ok;
            }

            #endregion
        }

        public class PDU
        {
            private const int CONFIG_VAR_SIZE = 24;
            public const int CONFIG_VAR_NAME_LENGHT = 10;
            private const int CONFIG_VAR_RESERVED = 10;

            public PDU()
            {
                Init();
            }
            #region Properties
            private List<byte> _RawData;
            public List<byte> RawData
            {
                get { return _RawData; }
                set { _RawData = value; }
            }
            List<DIComProtocol.DiComVar> _Vars;
            public List<DIComProtocol.DiComVar> Vars
            {
                get { return _Vars; }
                set { _Vars = value; }
            }

            private ExpandResult _ExpandResult;
            public ExpandResult ExpandResult
            {
                get { return _ExpandResult; }
                set { _ExpandResult = value; }
            }
            #endregion

            #region Methods
            public void Init()
            {
                _RawData = new List<byte>();
                _ExpandResult = ExpandResult.None;
                _Vars = null;
            }

            private bool IsEmptyConfigVar(ref byte[] dataArray, ref int offset)
            {
                // Check if 24 consecutive bytes in the buffer (starting from offset)
                // are equal to 0
                byte[] varDefinition = new byte[CONFIG_VAR_SIZE];
                Array.Copy(dataArray, offset, varDefinition, 0, CONFIG_VAR_SIZE);
                List<byte> nullBytes = (from byteVal in varDefinition where byteVal != 0 select byteVal).ToList();
                return (nullBytes.Count == 0);
            }


            private bool ExpandConfigVar(ref byte[] dataArray, ref int offset, out DiComVar newVar)
            {
                _RawData.Clear();
                _RawData.AddRange(dataArray);

                newVar = new DiComVar();

                _ExpandResult = ExpandResult.MessageIncomplete;
                if ((offset + CONFIG_VAR_SIZE) > dataArray.Length)
                    return false;

                // Check if the next variable definition is empty
                if(IsEmptyConfigVar(ref dataArray, ref offset))
                {
                    _ExpandResult = ExpandResult.EmptyVarDefinition;
                    return (false);
                }

                _ExpandResult = ExpandResult.MessageParsingError;
                newVar.VarType = (DiCommVarType)dataArray[offset];
                offset++;

                if (!Enum.IsDefined(typeof(DiCommVarType), newVar.VarType))
                    return false;

                newVar.VarSize = (ushort)BufferExpand.toUInt16(ref dataArray, ref offset);

                newVar.VarName = BufferExpand.toString(ref dataArray, ref offset, CONFIG_VAR_NAME_LENGHT);
                if (string.IsNullOrWhiteSpace(newVar.VarName))
                    return false;

                newVar.Reserve = BufferExpand.toArray(ref dataArray, ref offset, CONFIG_VAR_RESERVED);

                // move to next
                offset++;

                _ExpandResult = ExpandResult.Ok;

                return true;
            }

            public void ExpandConfig(HEADER header, ref byte[] dataArray)
            {
                _RawData.Clear();
                _RawData.AddRange(dataArray);

                int offset = 0;
                _ExpandResult = ExpandResult.MessageIncomplete;
                if (dataArray.Length < header.LoadSize)
                    return;

                _Vars = new List<DiComVar>();

                _ExpandResult = ExpandResult.Ok;
                while (_ExpandResult == ExpandResult.Ok && (offset < dataArray.Length && offset<header.LoadSize))
                {
                    if (ExpandConfigVar(ref dataArray, ref offset, out DiComVar newVar))                    
                    {
                        newVar.RecordID = header.RecordID;
                        newVar.VarIndex = _Vars.Count();
                        _Vars.Add(newVar);
                    }
                    else if(_ExpandResult == ExpandResult.EmptyVarDefinition)
                    {
                        // Empty variable definition --> Force parsing result to OK and terminate loop
                        _ExpandResult = ExpandResult.Ok;
                        offset = header.LoadSize;
                    }
                }
            }

            private byte[] SwapBytes(DiCommVarType type, byte[] data)
            {
                switch (type)
                {
                    case DiCommVarType.TYPE_BOOL:
                    case DiCommVarType.TYPE_BYTE:
                    case DiCommVarType.TYPE_USINT:
                    case DiCommVarType.TYPE_SINT:
                        //1 byte
                        return data;

                    case DiCommVarType.TYPE_WORD:
                    case DiCommVarType.TYPE_INT:
                    case DiCommVarType.TYPE_UINT:
                        //2 byte
                        CommJob.SwapByteBuffer(ref data);                                                    
                        return data;

                    case DiCommVarType.TYPE_DWORD:
                    case DiCommVarType.TYPE_DINT:
                    case DiCommVarType.TYPE_UDINT:
                    case DiCommVarType.TYPE_REAL:
                        CommJob.SwapWordBuffer(ref data);
                        //4 byte
                        return data;

                    case DiCommVarType.TYPE_ULINT:
                    case DiCommVarType.TYPE_LINT:
                    case DiCommVarType.TYPE_LREAL:
                    case DiCommVarType.TYPE_LWORD:
                        //8 byte
                        DIComCommJob.SwapDWordBuffer(ref data);
                        return data;
                    case DiCommVarType.TYPE_STRING:
                    case DiCommVarType.TYPE_WSTRING:
                    case DiCommVarType.TYPE_ENUM:
                    case DiCommVarType.TYPE_ARRAY:
                    case DiCommVarType.TYPE_USERDEF:
                        return data;
                    default:
                        return data;
                }
            }

            private bool ExpandDataVar(DIComProtocol.DiComVar var, ref byte[] dataArray, ref int offset, out DiComVar newVar)
            {
                newVar = null;

                _ExpandResult = ExpandResult.MessageIncomplete;
                if ((offset + var.VarSize) > dataArray.Length)
                    return false;

                newVar = (DIComProtocol.DiComVar)var.Clone();

                byte[] data = dataArray.Skip(offset).Take(var.VarSize).ToArray();
                switch (newVar.VarType)
                {
                    case DiCommVarType.TYPE_STRING:
                        // remove '\0' characters
                        string s = Encoding.UTF8.GetString(data).Trim('\0');
                        newVar.VarValue = Encoding.UTF8.GetBytes(s);
                        break;
                    case DiCommVarType.TYPE_WSTRING:
                        // remove '\0' characters
                        string ws = Encoding.UTF32.GetString(data).Trim('\0');
                        newVar.VarValue = Encoding.UTF32.GetBytes(ws);
                        break;
                    default:
                        newVar.VarValue = data;
                        break;
                }                

                offset += var.VarSize;

                return true;
            }

            public void ExpandData(HEADER header, Dictionary<int, List<DIComProtocol.DiComVar>> recordMaps, ref byte[] dataArray)
            {
                //_RawData.Clear();
                _RawData.AddRange(dataArray);

                int offset = 0;
                _ExpandResult = ExpandResult.MessageIncomplete;
                if (dataArray.Length < header.LoadSize)
                    return;

                _Vars = new List<DiComVar>();

                // invalid record id (not recived before)
                if (!recordMaps.ContainsKey(header.RecordID))
                {
                    ExpandResult = ExpandResult.MessageParsingError;
                    return;
                }
                
                foreach (DIComProtocol.DiComVar var in recordMaps[header.RecordID])
                {
                    if (ExpandDataVar(var, ref dataArray, ref offset, out DiComVar newVar))
                    {
                        newVar.VarIndex = _Vars.Count();
                        _Vars.Add(newVar);
                    }
                    else
                    {                        
                        break;
                    }
                }

                ExpandResult = ExpandResult.Ok;
            }
            #endregion
        }


        public class WizardDataDevice
        {
            public class Variable
            {
                public string DynamicLink { get; set; }
                public string TagName { get; set; }
                public int VarSize { get; set; }
                public int TagType { get; set; }

                public Variable()
                {
                    DynamicLink = string.Empty;
                    TagName = string.Empty;
                    VarSize = 0;
                    TagType = (int)BuiltInType.Boolean;
                }
            }

            public string StationName { get; set; }
            public string DynamicLink { get; set; }
            public List<Variable> Variables { get; set; }
            public WizardDataDevice()
            {
                StationName = string.Empty;
                DynamicLink = string.Empty;
                Variables = new List<Variable>();
            }
        }

        #region methods

        public static int GetMoviconTypeId(DIComProtocol.DiCommVarType type)
        {
            switch (type)
            {
                case DIComProtocol.DiCommVarType.TYPE_BOOL:
                    return (int)UFUAModel.DataType.Boolean;                    
                case DIComProtocol.DiCommVarType.TYPE_BYTE:
                case DIComProtocol.DiCommVarType.TYPE_USINT:
                    return (int)UFUAModel.DataType.Byte;
                case DIComProtocol.DiCommVarType.TYPE_INT:
                    return (int)UFUAModel.DataType.Int16;
                case DIComProtocol.DiCommVarType.TYPE_UINT:
                case DIComProtocol.DiCommVarType.TYPE_WORD:
                    return (int)UFUAModel.DataType.UInt16;
                case DIComProtocol.DiCommVarType.TYPE_DINT:
                    return (int)UFUAModel.DataType.Int32;
                case DIComProtocol.DiCommVarType.TYPE_UDINT:
                case DIComProtocol.DiCommVarType.TYPE_DWORD:
                    return (int)UFUAModel.DataType.UInt32;
                case DIComProtocol.DiCommVarType.TYPE_LINT:
                    return (int)UFUAModel.DataType.Int64;
                case DIComProtocol.DiCommVarType.TYPE_ULINT:
                case DIComProtocol.DiCommVarType.TYPE_LWORD:
                    return (int)UFUAModel.DataType.UInt64;
                case DIComProtocol.DiCommVarType.TYPE_REAL:
                    return (int)UFUAModel.DataType.Float;
                case DIComProtocol.DiCommVarType.TYPE_LREAL:
                    return (int)UFUAModel.DataType.Double;
                case DIComProtocol.DiCommVarType.TYPE_STRING:
                case DIComProtocol.DiCommVarType.TYPE_WSTRING:
                    return (int)UFUAModel.DataType.String;
            }

            return -1;
        }
        #endregion

        #region methods override

        public static bool ParseData(byte[] receivebuffer, ref DIComCommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);
            if (!areArguments && (receivebuffer == null))
            {
                return (false);
            }
            else if (receivebuffer == null)
            {
                BuiltInType bt = job.Station.GetBuiltInType(items[0].GetType());
                if (bt != BuiltInType.Byte && bt != BuiltInType.Double &&
                    bt != BuiltInType.Float && bt != BuiltInType.Int16 &&
                    bt != BuiltInType.Int32 && bt != BuiltInType.Int64 &&
                    bt != BuiltInType.Integer && bt != BuiltInType.Number &&
                    bt != BuiltInType.SByte && bt != BuiltInType.UInt16 &&
                    bt != BuiltInType.UInt32 && bt != BuiltInType.UInt64 &&
                    bt != BuiltInType.UInteger && bt != BuiltInType.String)
                {
                    return false;
                }

                items[0] = DriverErrorCodes.ErrorNoError;
                return (true);
            }

            if (areArguments)
            {
                items[0] = DriverErrorCodes.ErrorNoError;
            }

            byte[] tempBuffer = new byte[receivebuffer.Length];

            Array.Copy(receivebuffer, tempBuffer, receivebuffer.Length);

            job.SetJobData(tempBuffer, ref changed);

            if (areArguments)
            {
                if (job.TagsList.Count == items.Count - 1)
                {
                    for (int k = 0; k < job.TagsList.Count; k++)
                    {
                        items[k + 1] = job.TagsList[k].Value.Value;
                    }
                }
            }
            else
            {
                items.AddRange(changed);
            }

            return true;
        }

        public static string GetNodeTree(NodeId NodeId, string Name)
        {
            string Out = "";

            Regex NameParser = new Regex(@"[\d]+:(?<Name>[\w]+)$");
            Match NameMatch = NameParser.Match(Name);
            if (NameMatch.Success)
            {
                Out = NameMatch.Groups["Name"].Value;
                string nameNodeId = NodeId.Identifier.ToString();
                Regex NodeParser = new Regex(@"^[^?]+[?](?<Node>[\w/]+)/[\w-]+$");
                Match NodeMatch = NodeParser.Match(nameNodeId);
                if (NodeMatch.Success)
                {
                    Out = NodeMatch.Groups["Node"].Value + "." + Out;
                }
            }
            return Out;

        }

        public static string CorrectVarNameToMoviconVariableName(string varName)
        {
            string result = varName.ToLower();

            if (result.IndexOf(".") > 0)
                result = result.Replace(".", "__");

            if (result.IndexOf("&") > 0)
                result = result.Replace("&", "_and_");

            if (result.IndexOf(" ") > 0)
                result = result.Replace(" ", "_");
                        
            if (result.IndexOf("+") > 0)
                result = result.Replace("+", "plus");

            if (result.IndexOf("-") > 0)
                result = result.Replace("-", "minus");

            return result;
        }

        public static string CorrectMoviconVariableNameToVarName(string moviconVariableName)
        {
            string result = moviconVariableName.ToLower();

            // don't change replacing sequence to avoid multiplce replace
            if (result.IndexOf("__") > 0)
                result = result.Replace("__", ".");

            if (result.IndexOf("_and_") > 0)
                result = result.Replace("_and_", "&");

            if (result.IndexOf("_") > 0)
                result = result.Replace("_", " ");            

            if (result.IndexOf("plus") > 0)
                result = result.Replace("plus", "+");

            if (result.IndexOf("minus") > 0)
                result = result.Replace("minus", "-");

            return result;
        }
        #endregion       
    }
}
