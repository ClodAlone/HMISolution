////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GESRTP2Protocol.cs
//
// summary:	Implements the driver GESRTP2 protocol class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using System.Text;

namespace GESRTP2
{
    interface IPackable
    {
        byte[] Pack();
    }
    interface IExpandable
    {
        int Expand(ReqTypes reqType, byte[] DataArray, int Offset = 0);
    }

    /// <summary>   Error codes of GESRTP2. </summary>
    /// Note --> When defining a new error make sure it fits in 2 bytes : the 3rd and 4th bytes are used for the min/maj error code
    public enum GESRTP2ErrorCodes : int
    {
        ErrorTxWrite = 1000,
        ErrorRxRead,
        ErrorUnexpectedConnectionReply,
        ErrorUnexpectedPduType,
        ErrorUnexpectedInvokeId,
        ErrorUnexpectedSessionReply,
        ErrorNoRep,
        ErrorUnexpectedMailSeq,
        ErrorReceiveFewData,
        ErrorReceiveNack,
        ErrorUnexpectedReadReply,
        ErrorUnexpectedWriteReply,
        ErrorUnexpectedCapabilities,
        ErrorVarBadNotFound,
        ErrorSymbolicUnsupportedDataType,
        ErrorDirInvalid,
        ErrorArraySizeMismatch,
        ErrorDataTypeMisMatch,
    }

    /// <summary>   GESRTP2's target device memory areas. </summary>
    public enum AreaTypes : byte
    {
        /// <summary>   An enum constant representing the Register Words (%R) option. </summary>
        RegisterWords_R = 0x08,
        /// <summary>   An enum constant representing the Symbolic Address (internal use). </summary>
        Symbolic = 0x09,    // --> set this value to keep sorted the list
        /// <summary>   An enum constant representing the Analog Input Words (%AI) option. </summary>
        AnalogInputWords_AI = 0x0A,
        /// <summary>   An enum constant representing the Analog Output Words (%AQ option. </summary>
        AnalogOutputWords_AQ = 0x0C,
        /// <summary>   An enum constant representing the Discrete Input Bytes (%I) option. </summary>
        DiscreteInputBytes_I = 0x10,
        /// <summary>   An enum constant representing the Discrete Input Bits (%I) option. </summary>
        DiscreteInputBits_I = 0x46,
        /// <summary>   An enum constant representing the Discrete Output Bytes (%Q) option. </summary>
        DiscreteOutputBytes_Q = 0x12,
        /// <summary>   An enum constant representing the Discrete Output Bits (%Q) option. </summary>
        DiscreteOutputBits_Q = 0x48,
        /// <summary>   An enum constant representing the Discrete Temporary Bytes (%T) option. </summary>
        DiscreteTemporaryBytes_T = 0x14,
        /// <summary>   An enum constant representing the Discrete Temporary Bits (%T) option. </summary>
        DiscreteTemporaryBits_T = 0x4A,
        /// <summary>   An enum constant representing the Discrete Internal Bytes (%M) option. </summary>
        DiscreteInternalBytes_M = 0x16,
        /// <summary>   An enum constant representing the Discrete Internal Bits (%M) option. </summary>
        DiscreteInternalBits_M = 0x4C,
        /// <summary>   An enum constant representing the Discrete Bytes (%SA) option. </summary>
        DiscreteBytes_SA = 0x18,
        /// <summary>   An enum constant representing the Discrete Bits (%SA) option. </summary>
        DiscreteBits_SA = 0x4E,
        /// <summary>   An enum constant representing the Discrete Bytes (%SB) option. </summary>
        DiscreteBytes_SB = 0x1A,
        /// <summary>   An enum constant representing the Discrete Bits (%SB) option. </summary>
        DiscreteBits_SB = 0x50,
        /// <summary>   An enum constant representing the Discrete Bytes (%SC) option. </summary>
        DiscreteBytes_SC = 0x1C,
        /// <summary>   An enum constant representing the Discrete Bits (%SC) option. </summary>
        DiscreteBits_SC = 0x52,
        /// <summary>   An enum constant representing the Discrete Bytes (%S)(read only) option. </summary>
        DiscreteBytes_S_readOnly = 0x1E,
        /// <summary>   An enum constant representing the Discrete Bits (%S) (read only) option. </summary>
        DiscreteBits_S_readOnly = 0x54,
        /// <summary>   An enum constant representing the Genius Global Data Bytes (%G) option. </summary>
        GeniusGlobalDataBytes_G = 0x38,
        /// <summary>   An enum constant representing the Genius Global Data Bits (%G) option. </summary>
        GeniusGlobalDataBits_G = 0x56,
        /// <summary>   An enum constant representing the Word Memory (%W) option. </summary>
        WordMemory_W = 0xC4,
    }

    public enum PduTypes : byte
    {
        ConnectRequest,
        ConnectResponse,
        DataRequest,
        DataResponse,
        UnconfirmedRequest,
        ErrorRequest,
        DestinationsRequest,
        DestinationsResponse,
        SessionRequest,
    }
    public enum TrafficTypes : byte
    {
        InitialRequestTB = 0x80,
        StartResponse = 0x92,
        Continue = 0x93,
        CompletionAckTB = 0x94,
        InitialRequest = 0xC0,
        InvalidRequestNack = 0xD1,
        CompletionAck = 0xD4,
        AbortSequence = 0xD7,
    }
    public enum ReqTypes : byte
    {
        READ_SMEM = 0x04,
        WRITE_SMEM = 0x07,
        SESSION_CONTROL = 0x4f,
        READ_ADDR_VAR = 0x6b,
        WRITE_ADDR_VAR = 0x6f,
        READ_SYMBOL_LOOKUP = 0x6e,
        READ_DIR = 0x7e,
        ALL_TYPES = 0xFF
    }
    public enum Commands : byte
    {
        SRTP_NO_CMD,
        SRTP_ESTABLISH_SESSION_CMD,
        SRTP_TERMINATE_SESSION_CMD,
        SRTP_ALLOCATE_CONN_AREA_CMD,
        SRTP_DEALLOC_CONN_AREA_CMD,
        SRTP_ALLOCATE_BLOCK_TRANS_CMD,
        SRTP_DEALLOC_BLOCK_TRANS_CMD,
        SRTP_CONN_DATA_REQUEST_CMD,
        SRTP_BLOCK_TRANS_REQUEST_CMD
    }
    public enum MessageTypes : byte
    {
        SRTP_NO_MESSAGE,
        SRTP_MAIL_MESSAGE,
        SRTP_MAIL_MESSAGE_W_TEXT,
        SRTP_MAIL_MESSAGE_W_CONN,
        SRTP_MAIL_MESSAGE_W_BLOCK,
        SRTP_HP_MAIL_MESSAGE,
        SRTP_HP_MAIL_MESS_W_TEXT,
        SRTP_MSG_W_TEXT_AND_CONN,
        SRTP_MSG_W_TEXT_AND_BLOCK,
        SRTP_PACKED = 128,
        SRTP_TEXT,
        SRTP_TEXT_CONN,
        SRTP_TEXT_BLOCK,
        SRTP_ROUTER,
    }

    public enum SessionEnables
    {
        TERMINATE,
        ESTABLISH,
    }

    public enum Capabilities : uint
    {
        SRTP_SRP_SERVER = 0x0001,
        SRTP_DESTINATION_SERVER = 0x0002,
        SRTP_PRIV_CONNECTION_SERVER = 0x0004,
    }

    public enum SymbolicDataType : UInt32
    {
        Undefined = 255,
        Boolean = 0,   //VariableLength:1
        Byte = 13,  //VariableLength:1
        DInt = 1,   //VariableLength:4
        DWord = 18,  //VariableLength:4
        Int = 25,  //VariableLength:2
        LReal = 2,   //VariableLength:8
        Real = 27,  //VariableLength:4
        String = 24,  //VariableLength: nr bytes defined into PLC 
        UInt = 26,  // VariableLength:2
        Word = 14,  //VariableLength:2
    }    

    public class ReadWriteListLimitateSize
    {
        public uint ProtocolSize;
        public uint TotalRequestSize;
        public uint TotalResponseSize;
        public uint TotalNrJobs;

        public ReadWriteListLimitateSize()
        {
            TotalRequestSize = 0;
            TotalResponseSize = 0;
            ProtocolSize = 0;
            TotalNrJobs = 0;
        }

        public bool IsEmpty()
        {
            return (ProtocolSize == 0);
        }
    }

    public class GEF_STRING : IPackable, IExpandable
    {
        private const byte GEF_ASCII = 1;
        private const char NULL_TERMINATOR = '\0';

        string text = String.Empty;

        public string Text { get { return text; } set { text = value; } }

        public GEF_STRING(string tex)
        {
            text = tex;
        }

        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BitConverter.GetBytes(GEF_ASCII));
            retVal.AddRange(BitConverter.GetBytes((short)(text.Length + 1)));
            retVal.AddRange(ASCIIEncoding.ASCII.GetBytes(text));
            retVal.Add(Convert.ToByte(NULL_TERMINATOR));  // /0 null terminator

            return retVal.ToArray();
        }

        public int Expand(ReqTypes reqType, byte[] dataArray, int offset = 0)
        {
            int newOffset = offset;

            UInt16 textLen = BitConverter.ToUInt16(dataArray, newOffset);
            newOffset += 2;

            text = Encoding.ASCII.GetString(dataArray.Skip(newOffset).Take(textLen).ToArray()).TrimEnd(NULL_TERMINATOR);
            newOffset += textLen;

            return newOffset;
        }
    }

    public class Plc_xtnd_addr_rec : IPackable, IExpandable
    {
        private const byte SELECTIVE_FIP_SET = 1;
        private const byte VME_3PY_INT_ID = 2;
        private const byte XTND_SEG = 3;

        //byte _Seg_sel = 0; /* XTND_SEG = 25, 0x19 */
        //short _Xtnd_seg_sel = 0; /* OPP_MOD_REV_INFO (502, 0x01f6) */
        //byte _Property_id = 0; /* Refer to PACSystems Programmer-PLC PIDD */
        List<byte> _Data = new List<byte>(); /* Refer to PACSystems Programmer-PLC PIDD */

        //public byte Seg_sel { get { return _Seg_sel; } set { _Seg_sel = value; } }
        //public short Xtnd_seg_sel { get { return _Xtnd_seg_sel; } set { _Xtnd_seg_sel = value; } }
        //public byte Property_id { get { return _Property_id; } set { _Property_id = value; } }
        public byte[] Data { get { return _Data.ToArray(); } set { _Data.Clear(); _Data.AddRange(value); } }

        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();

            return retVal.ToArray();
        }

        public int Expand(ReqTypes reqType, byte[] dataArray, int offset = 0)
        {
            int newOffset = offset;
            _Data.AddRange(dataArray.Skip(newOffset).Take(8));
            newOffset += 8;

            return newOffset;
        }
    }

    public class Var_addr_list_rec : IPackable, IExpandable
    {
        /* Variable Address List */
        UInt32 _CoherencyCookie = 0;   /* must be 0, UNLESS read_addr_var is being usined with
                                    * SYMBOL_LOOKUP. In this case, the “CohenrencyCookie value
                                    * returned by SYMBOL_LOOKUP should be put here.
                                    */
        Plc_xtnd_addr_rec _Data_type = new Plc_xtnd_addr_rec();     /* see list below for
                                                    seg sels
                                                    * supported by PACSystems
                                                    */
        UInt32 _Read_length = 0;   /* number of bytes to read in units of bytes
                                * NOTE1: if bit offset field is non-zero, indicates the number of
                                bits read.
                                * NOTE2: For property pages read length may be 0, which indicates
                                read
                                * all information associated with this property page.            
                                 */

        public UInt32 CoherencyCookie { get { return _CoherencyCookie; } set { _CoherencyCookie = value; } }

        // 
        byte[] _InternalAddress = new byte[12];
        public byte[] InternalAddress { get { return _InternalAddress; } set { _InternalAddress = value; } }

        public Plc_xtnd_addr_rec Data_type { get { return _Data_type; } set { _Data_type = value; } }
        public UInt32 Read_length { get { return _Read_length; } set { _Read_length = value; } }

        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();

            retVal.AddRange(BitConverter.GetBytes(_CoherencyCookie));
            retVal.AddRange(_Data_type.Pack());
            retVal.AddRange(BitConverter.GetBytes(_Read_length));

            return retVal.ToArray();
        }

        public int Expand(ReqTypes reqType, byte[] DataArray, int Offset = 0)
        {
            int NewOffset = Offset;

            _CoherencyCookie = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;

            NewOffset = _Data_type.Expand(reqType, DataArray, NewOffset);

            _Read_length = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;

            return NewOffset;
        }
    }

    public class Control : IPackable, IExpandable
    {
        //Fields
        private Commands _Command = 0;
        private MessageTypes _MessageType = 0;
        private ushort _MbOffset = 0;
        private uint _Length = 0;

        //Properties
        public Commands Command { get { return _Command; } set { _Command = value; } }
        public MessageTypes MessageType { get { return _MessageType; } set { _MessageType = value; } }
        public ushort MbOffset { get { return _MbOffset; } set { _MbOffset = value; } }
        public uint Length { get { return _Length; } set { _Length = value; } }

        //Methods
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add((byte)_Command);
            retVal.Add((byte)_MessageType);
            retVal.AddRange(BitConverter.GetBytes(_MbOffset));
            retVal.AddRange(BitConverter.GetBytes(_Length));

            return retVal.ToArray();
        }

        public int Expand(ReqTypes reqType, byte[] DataArray, int Offset = 0)
        {
            int NewOffset = Offset;

            _Command = (Commands)DataArray[NewOffset];
            NewOffset += 1;
            _MessageType = (MessageTypes)DataArray[NewOffset];
            NewOffset += 1;
            _MbOffset = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;
            _Length = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;

            return NewOffset;
        }
    }

    public class SmemSelect : IPackable
    {
        //Fields
        private AreaTypes _SegSelector = 0;
        private UInt16 _SegOffset = 0;
        private UInt16 _Length = 0;

        //Properties
        public AreaTypes SegSelector { get { return _SegSelector; } set { _SegSelector = value; } }
        public UInt16 SegOffset { get { return _SegOffset; } set { _SegOffset = value; } }
        public UInt16 Length { get { return _Length; } set { _Length = value; } }

        //Methods
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add((byte)_SegSelector);
            retVal.AddRange(BitConverter.GetBytes(_SegOffset));
            retVal.AddRange(BitConverter.GetBytes(_Length));

            return retVal.ToArray();
        }
    }

    public class SmemSelectWrite : IPackable
    {
        //Fields
        private AreaTypes _SegSelector = 0;
        private UInt16 _SegOffset = 0;
        private UInt16 _Length = 0;
        byte[] _WriteData;

        //Properties
        public AreaTypes SegSelector { get { return _SegSelector; } set { _SegSelector = value; } }
        public UInt16 SegOffset { get { return _SegOffset; } set { _SegOffset = value; } }
        public UInt16 Length { get { return _Length; } set { _Length = value; } }
        public byte[] WriteData { get { return _WriteData; } set { _WriteData = value; } }

        //Methods
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add((byte)_SegSelector);
            retVal.AddRange(BitConverter.GetBytes(_SegOffset));
            retVal.AddRange(BitConverter.GetBytes(_Length));
            if (_WriteData != null)
                retVal.AddRange(_WriteData);

            return retVal.ToArray();
        }

    }

    public class PiggyBackedStatus : IExpandable
    {
        //Fields
        private byte _TargetNum = 0;
        private byte _PrivLev = 0;
        private ushort _SwpTime = 0;
        private ushort _Status = 0;

        //Properties
        public byte TargetNum { get { return _TargetNum; } }
        public byte PrivLev { get { return _PrivLev; } }
        public ushort SwpTime { get { return _SwpTime; } }
        public ushort Status { get { return _Status; } }

        //Methods

        public int Expand(ReqTypes reqType, byte[] DataArray, int Offset = 0)
        {
            int NewOffset = Offset;

            _TargetNum = DataArray[NewOffset];
            NewOffset += 1;
            _PrivLev = DataArray[NewOffset];
            NewOffset += 1;
            _SwpTime = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;
            _Status = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;

            return NewOffset;
        }
    }

    public class SymbolicDirRead : IPackable
    {
        public class DirReadData : IPackable
        {
            private UInt32 _RecordSize; /* Size of this record in bytes */
            public UInt32 RecordSize { get { return _RecordSize; } set { _RecordSize = value; } }

            private UInt16 _RecordType; /* directory, file See Table below for Record Types */
            public UInt16 RecordType { get { return _RecordType; } set { _RecordType = value; } }

            private UInt16 _Date_type; // FILE: Indicates the data_type for this file DIRECTORY: will be 0xFFFF
            public UInt16 Date_type { get { return _Date_type; } set { _Date_type = value; } }

            private UInt16 _Size;
            public UInt16 Size { get { return _Size; } set { _Size = value; } } /* size for Record type = FILE_TYPE: size (in bytes) of file indicated by name
	            * size for Record type = DIRECTORY_TYPE: size (in bytes) of all files in directory
	            * * ALL sizes indicate the number of bytes that are charged to the user and NOT the physical or the size specified in the FILE_WRITE's data	size.of the file.
	            */
            private UInt32 _Checksum; // checksum value for Record type = FILE_TYPE: 32 bit checksum of the file indicated by “name” checksum value for Record type = DIRECTORY_TYPE: checksum stored at directory level
            public UInt32 Checksum { get { return _Checksum; } set { _Checksum = value; } }

            private UInt32 _FileAttrMask;  /* Mask indicating file attributes currently set for this file */
            public UInt32 FileAttrMask { get { return _FileAttrMask; } set { _FileAttrMask = value; } }

            private byte[] _DateTimeStamp = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 }; /* 8 bytes indicating date time, Format of New Requests */
            public byte[] DateTimeStamp { get { return _DateTimeStamp; } set { _DateTimeStamp = value; } }

            private string _DirName = string.Empty;
            public string DirName { get { return _DirName; } set { _DirName = value; } }            

            public byte[] Pack()
            {
                return new byte[0];
            }

            public int Expand(ReqTypes reqType, byte[] DataArray, int Offset = 0)
            {
                int NewOffset = Offset;

                _RecordSize = BitConverter.ToUInt32(DataArray, NewOffset);
                NewOffset += 4;
                _RecordSize = BitConverter.ToUInt32(DataArray, NewOffset);
                NewOffset += 4;
                _RecordType = BitConverter.ToUInt16(DataArray, NewOffset);
                NewOffset += 2;
                _Date_type = BitConverter.ToUInt16(DataArray, NewOffset);
                NewOffset += 2;
                _Size = BitConverter.ToUInt16(DataArray, NewOffset);
                NewOffset += 2;
                _Checksum = BitConverter.ToUInt32(DataArray, NewOffset);
                NewOffset += 4;
                _FileAttrMask = BitConverter.ToUInt32(DataArray, NewOffset);
                NewOffset += 4;
                _DateTimeStamp = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
                NewOffset += 8;

                GEF_STRING gef_string = new GEF_STRING(string.Empty);
                NewOffset = gef_string.Expand(ReqTypes.ALL_TYPES, DataArray, NewOffset);
                _DirName = gef_string.Text;

                return NewOffset;
            }            
        }

        //Fields
        private UInt16 _Hrev = 2;
        private UInt16 _Req = (UInt16)ReqTypes.READ_DIR;
        private byte[] _Checksums = new byte[4] { 0, 0, 0, 0 };
        private byte[] _DataType = new byte[4] { 0xff, 0xff, 0xff, 0xff };
        private byte[] _FileAttributeMask = new byte[4] { 0, 0, 0, 0 };
        private byte[] _DateTimeStamp = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        private UInt32 _DataRequestLength = 0xa;
        private UInt32 _BufferLength = 0x44;
        private UInt32 _OffsetNameField = 0x30;
        private UInt32 _OffsetDataField = 0x3a;
        private byte[] _Spare = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        private UInt16 _Nrev = 1;
        private string _NameField;
        private UInt16 _DRev = 1;
        private UInt32 _LengthOfData = 4;
        private UInt32 _DirAttributes = 0;
        private List<DirReadData> _DirData = new List<DirReadData>();

        //Properties        
        public UInt32 DataRequestLength { get { return _DataRequestLength; } set { _DataRequestLength = value; } }
        public UInt32 BufferLength { get { return _BufferLength; } set { _BufferLength = value; } }
        public UInt32 OffsetDataField { get { return _OffsetDataField; } set { _OffsetDataField = value; } }
        public string NameField { get { return _NameField; } set { _NameField = value; } }
        public int NrDirData { get { return _DirData.Count; } }
        public List<DirReadData> DirData { get { return _DirData; } set { _DirData = value; } }


        public bool HasADirName()
        {
            return (NrDirData > 0 && !string.IsNullOrEmpty(_DirData[0].DirName));
        }

        public string GetDirName()
        {
            return (HasADirName() ? _DirData[0].DirName : string.Empty);
        }

        //Methods
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(new byte[] { 0, 0, 0, 0, 0} );
            retVal.AddRange(BitConverter.GetBytes(_Hrev));
            retVal.AddRange(BitConverter.GetBytes(_Req));
            retVal.AddRange(_Checksums);
            retVal.AddRange(_DataType);
            retVal.AddRange(_FileAttributeMask);
            retVal.AddRange(_DateTimeStamp);
            retVal.AddRange(BitConverter.GetBytes(_DataRequestLength));
            retVal.AddRange(BitConverter.GetBytes(_BufferLength));
            retVal.AddRange(BitConverter.GetBytes(_OffsetNameField));
            retVal.AddRange(BitConverter.GetBytes(_OffsetDataField));
            retVal.AddRange(_Spare);
            retVal.AddRange(BitConverter.GetBytes(_Nrev));
                        
            retVal.AddRange(BitConverter.GetBytes((UInt16)(_NameField.Length + 1)));
            retVal.AddRange(ASCIIEncoding.ASCII.GetBytes(_NameField));
            retVal.Add(0); // null terminator

            retVal.AddRange(BitConverter.GetBytes(_DRev));
            retVal.AddRange(BitConverter.GetBytes(_LengthOfData));
            retVal.AddRange(BitConverter.GetBytes(_DirAttributes));

            return retVal.ToArray();
        }

        public int Expand(ReqTypes reqType, byte[] DataArray, int Offset = 0)
        {
            int NewOffset = Offset;
                        
            //Buffer.BlockCopy(DataArray, NewOffset, _MbuIntern, 0, _MbuIntern.Length);
            //NewOffset += _MbuIntern.Length;

            _Hrev = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;
            _Req = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;
            Buffer.BlockCopy(DataArray, NewOffset, _Checksums, 0, _Checksums.Length);
            NewOffset += _Checksums.Length;
            Buffer.BlockCopy(DataArray, NewOffset, _DataType, 0, _DataType.Length);
            NewOffset += _DataType.Length;
            Buffer.BlockCopy(DataArray, NewOffset, _FileAttributeMask, 0, _FileAttributeMask.Length);
            NewOffset += _FileAttributeMask.Length;
            Buffer.BlockCopy(DataArray, NewOffset, _DateTimeStamp, 0, _DateTimeStamp.Length);
            NewOffset += _DateTimeStamp.Length;
            _LengthOfData = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            _BufferLength = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            _OffsetNameField = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            _OffsetDataField = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;

            NewOffset += 8; // alignment

            //NewOffset += 10; // Nrev + Length of Name + Name field        
            _DRev = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;
            _LengthOfData = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            UInt16 numOfRecords = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;

            for (UInt16 nrec = 0; nrec < numOfRecords; nrec++)
            {
                DirReadData dir = new DirReadData();
                NewOffset = dir.Expand(ReqTypes.ALL_TYPES, DataArray, NewOffset);
                _DirData.Add(dir);
            }

            return NewOffset;
        }
    }

    public class SymbolicLookUpRead : IPackable
    {
        public class SymbolLookupRec : IExpandable
        {
            Plc_xtnd_addr_rec _AddrRec = new Plc_xtnd_addr_rec();
            UInt32 _VariableLength = 0; /* data length, as recorded in symbol file */
            UInt32 _UnitType = 0; /* unit type, as recorded in symbol file */
            ushort _UnitDimension1 = 0; /* unit count, in dimension1, as recorded in symbol file */
            ushort _UnitDimension2 = 0; /* unit count, in dimension 2, as recorded in symbol file */
            byte[] _Spares = new byte[4]; /* spare bytes as recorded in symbol file */

            public Plc_xtnd_addr_rec AddrRec { get { return _AddrRec; } set { _AddrRec = value; } }
            public UInt32 VariableLength { get { return _VariableLength; } set { _VariableLength = value; } }
            public UInt32 UnitType { get { return _UnitType; } set { _UnitType = value; } }
            public ushort UnitDimension1 { get { return _UnitDimension1; } set { _UnitDimension1 = value; } }
            public ushort UnitDimension2 { get { return _UnitDimension2; } set { _UnitDimension2 = value; } }
            public byte[] Spares { get { return _Spares; } set { _Spares = value; } }

            public int Expand(ReqTypes reqType, byte[] DataArray, int Offset = 0)
            {
                int NewOffset = Offset;

                NewOffset = _AddrRec.Expand(reqType, DataArray, NewOffset);

                _VariableLength = BitConverter.ToUInt32(DataArray, NewOffset);
                NewOffset += 4;
                _UnitType = BitConverter.ToUInt32(DataArray, NewOffset);
                NewOffset += 4;
                _UnitDimension1 = BitConverter.ToUInt16(DataArray, NewOffset);
                NewOffset += 2;
                _UnitDimension2 = BitConverter.ToUInt16(DataArray, NewOffset);
                NewOffset += 2;
                Buffer.BlockCopy(DataArray, NewOffset, _Spares, 0, _Spares.Length);
                NewOffset += 4;

                return NewOffset;
            }

            public ushort GetTotalArrayDimension()
            {
                ushort total = 0;
                if (_UnitDimension1 > 0)
                    total = _UnitDimension1;
                if (_UnitDimension2 > 0)
                    total *= _UnitDimension2;
                return total;
            }
        }

        //Fields
        private UInt16 _Hrev = 2;
        private UInt16 _Req = (UInt16)ReqTypes.READ_SYMBOL_LOOKUP;
        private byte[] _Checksums = new byte[4] { 0, 0, 0, 0 };
        private byte[] _DataType = new byte[4] { 0xff, 0xff, 0xff, 0xff };
        private byte[] _FileAttributeMask = new byte[4] { 0, 0, 0, 0 };
        private byte[] _DateTimeStamp = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        private UInt32 _DataRequestLength = 4;
        private UInt32 _BufferLength = 0;
        private UInt32 _OffsetNameField = 0x30;
        private UInt32 _OffsetDataField = 0;
        private UInt16 _Nrev = 1;
        private string _NameField = string.Empty;
        private UInt16 _DRev = 1;
        private List<GEF_STRING> _RequestSymbols = new List<GEF_STRING>();
        private List<SymbolLookupRec> _ResponseSymbols = new List<SymbolLookupRec>();

        UInt32 _CoherencyCookie = 0;
        private ushort _NumSymbols;

        //Properties        
        public UInt32 DataRequestLength { get { return _DataRequestLength; } set { _DataRequestLength = value; } }
        public UInt32 BufferLength { get { return _BufferLength; } set { _BufferLength = value; } }
        public UInt32 OffsetDataField { get { return _OffsetDataField; } set { _OffsetDataField = value; } }
        public string NameField { get { return _NameField; } set { _NameField = value; } }
        public List<GEF_STRING> RequestSymbols { get { return _RequestSymbols; } set { _RequestSymbols = value; } }
        public ushort NumSymbols { get { return _NumSymbols; } set { _NumSymbols = value; } }
        public List<SymbolLookupRec> ResponseSymbols { get { return _ResponseSymbols; } set { _ResponseSymbols = value; } }

        //Methods
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(new byte[] { 0, 0, 0, 0, 0 });
            retVal.AddRange(BitConverter.GetBytes(_Hrev));
            retVal.AddRange(BitConverter.GetBytes(_Req));
            retVal.AddRange(_Checksums);
            retVal.AddRange(_DataType);
            retVal.AddRange(_FileAttributeMask);
            retVal.AddRange(_DateTimeStamp);
            retVal.AddRange(BitConverter.GetBytes(_DataRequestLength));
            retVal.AddRange(BitConverter.GetBytes(_BufferLength));
            retVal.AddRange(BitConverter.GetBytes(_OffsetNameField));
            _OffsetDataField = (uint)(_OffsetNameField + _NameField.Length + 1 + 4);
            retVal.AddRange(BitConverter.GetBytes(_OffsetDataField));
            retVal.AddRange(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            retVal.AddRange(BitConverter.GetBytes(_Nrev));

            // create an array bigger to contain null string terminator \0
            retVal.AddRange(BitConverter.GetBytes((UInt16)(_NameField.Length + 1)));
            retVal.AddRange(ASCIIEncoding.ASCII.GetBytes(_NameField));
            retVal.Add(0);
            retVal.AddRange(BitConverter.GetBytes(_DRev));

            retVal.AddRange(BitConverter.GetBytes((UInt16)_RequestSymbols.Count));
            foreach (GEF_STRING var in _RequestSymbols)
                retVal.AddRange(var.Pack());

            return retVal.ToArray();
        }

        public int Expand(ReqTypes reqType, byte[] DataArray, int Offset = 0)
        {
            int NewOffset = Offset;

            _Hrev = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;
            _Req = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;
            Buffer.BlockCopy(DataArray, NewOffset, _Checksums, 0, _Checksums.Length);
            NewOffset += _Checksums.Length;
            Buffer.BlockCopy(DataArray, NewOffset, _DataType, 0, _DataType.Length);
            NewOffset += _DataType.Length;
            Buffer.BlockCopy(DataArray, NewOffset, _FileAttributeMask, 0, _FileAttributeMask.Length);
            NewOffset += _FileAttributeMask.Length;
            Buffer.BlockCopy(DataArray, NewOffset, _DateTimeStamp, 0, _DateTimeStamp.Length);
            NewOffset += _DateTimeStamp.Length;
            _DataRequestLength = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            _BufferLength = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            _OffsetNameField = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            _OffsetDataField = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;

            NewOffset += 8; // aligment

            _DRev = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;
            _CoherencyCookie = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            _NumSymbols = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;

            _ResponseSymbols.Clear();
            for (int i = 0; i < _NumSymbols; i++)
            {
                SymbolLookupRec symbol = new SymbolLookupRec();
                NewOffset = symbol.Expand(reqType, DataArray, NewOffset);
                _ResponseSymbols.Add(symbol);
            }

            return NewOffset;
        }

        /// <summary>
        /// Get PLC variable internal address from symbolic name : this information is necessary to read/write plc variable value
        /// </summary>
        /// <param name="varIndex"></param>
        /// <param name="coherencyCookie"></param>
        /// <param name="internalAddress"></param>
        /// <param name="dataType"></param>
        /// <param name="variableLength"></param>
        /// <param name="arrayLength"></param>
        /// <returns></returns>
        public void GetSymbolicAddressInfo(int varIndex, out UInt32 coherencyCookie, out byte[] internalAddress, out UInt32 dataType, out UInt32 variableLength, out UInt32 arrayLength)
        {
            coherencyCookie = _CoherencyCookie;                    
            variableLength = _ResponseSymbols[varIndex].VariableLength;
            arrayLength = _ResponseSymbols[varIndex].GetTotalArrayDimension();            
            dataType = _ResponseSymbols[varIndex].UnitType;
            if ((SymbolicDataType)dataType == SymbolicDataType.Byte)
                variableLength = _ResponseSymbols[varIndex].VariableLength * 2;

            List<byte> intern = new List<byte>();
            intern.AddRange(_ResponseSymbols[varIndex].AddrRec.Data);
            intern.AddRange(BitConverter.GetBytes(variableLength));
            internalAddress = intern.ToArray();                
        }
    }

    public class SymbolicReadAddrVars : IPackable
    {       
        public class Var
        {
            UInt32 _CoherencyCookie = 0;
            byte[] _InternalAddress = new byte[12];

            public UInt32 CoherencyCookie { get { return _CoherencyCookie; } set { _CoherencyCookie = value; } }            
            public byte[] InternalAddress { get { return _InternalAddress; } set { _InternalAddress = value; } }
        }

        //Fields
        private UInt16 _Hrev = 2;
        private UInt16 _Req = (UInt16)ReqTypes.READ_ADDR_VAR;
        private byte[] _Checksums = new byte[4] { 0, 0, 0, 0 };
        private byte[] _DataType = new byte[4] { 0xff, 0xff, 0xff, 0xff };
        private byte[] _FileAttributeMask = new byte[4] { 0, 0, 0, 0 };
        private byte[] _DateTimeStamp = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        private UInt32 _DataRequestLength = 4;
        private UInt32 _BufferLength = 0;
        private UInt32 _OffsetNameField = 0;
        private UInt32 _OffsetDataField = 0x30;
        private byte[] _Spare = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };
        private UInt16 _Nrev = 1;
        private string _NameField = string.Empty;        
        private List<Var> _ReadVars = new List<Var>();

        //private ushort _NumSymbols;

        //Properties        
        public UInt32 DataRequestLength { get { return _DataRequestLength; } set { _DataRequestLength = value; } }
        public UInt32 BufferLength { get { return _BufferLength; } set { _BufferLength = value; } }
        public UInt32 OffsetDataField { get { return _OffsetDataField; } set { _OffsetDataField = value; } }
        public List<Var> ReadVars { get { return _ReadVars; } set { _ReadVars = value; } }
        //public ushort NumSymbols { get { return _NumSymbols; } set { _NumSymbols = value; } }
        
        //Methods
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(new byte[] { 0, 0, 0, 0, 0 });
            retVal.AddRange(BitConverter.GetBytes(_Hrev));
            retVal.AddRange(BitConverter.GetBytes(_Req));
            retVal.AddRange(_Checksums);
            retVal.AddRange(_DataType);
            retVal.AddRange(_FileAttributeMask);
            retVal.AddRange(_DateTimeStamp);
            
            _DataRequestLength = (UInt32)(2 + 4 + 4 + (16 * _ReadVars.Count));
            retVal.AddRange(BitConverter.GetBytes(_DataRequestLength));
            retVal.AddRange(BitConverter.GetBytes(_BufferLength));
            retVal.AddRange(BitConverter.GetBytes(_OffsetNameField));
            //_OffsetDataField = (uint)(_OffsetNameField + _NameField.Length + 1 + 4);
            retVal.AddRange(BitConverter.GetBytes(_OffsetDataField));
            retVal.AddRange(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });
            //retVal.AddRange(BitConverter.GetBytes(_Nrev));                      

            // c++ documentaion section
            retVal.AddRange(BitConverter.GetBytes(_Nrev));
            Int32 lengthOfData = 4 + (16 * _ReadVars.Count);
            retVal.AddRange(BitConverter.GetBytes(lengthOfData));
            retVal.AddRange(BitConverter.GetBytes((UInt32)_ReadVars.Count));
            try
            {
                foreach (Var var in _ReadVars)
                {
                    retVal.AddRange(BitConverter.GetBytes(var.CoherencyCookie));
                    retVal.AddRange(var.InternalAddress);
                }
            }
            catch (Exception ex) { }

            return retVal.ToArray();
        }

        public int Expand(ReqTypes reqType, byte[] DataArray, int Offset = 0)
        {
            int NewOffset = Offset;

            _Hrev = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;
            _Req = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;
            Buffer.BlockCopy(DataArray, NewOffset, _Checksums, 0, _Checksums.Length);
            NewOffset += _Checksums.Length;
            Buffer.BlockCopy(DataArray, NewOffset, _DataType, 0, _DataType.Length);
            NewOffset += _DataType.Length;
            Buffer.BlockCopy(DataArray, NewOffset, _FileAttributeMask, 0, _FileAttributeMask.Length);
            NewOffset += _FileAttributeMask.Length;
            Buffer.BlockCopy(DataArray, NewOffset, _DateTimeStamp, 0, _DateTimeStamp.Length);
            NewOffset += _DateTimeStamp.Length;
            _DataRequestLength = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            _BufferLength = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            _OffsetNameField = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            _OffsetDataField = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;

            NewOffset += 2; // aligment

            //uint nrev = BitConverter.ToUInt16(DataArray, NewOffset);
            //NewOffset += 2;
            //Int32 lengthOfData = BitConverter.ToInt32(DataArray, NewOffset);
            //NewOffset += 4;
            //UInt32 readVars = BitConverter.ToUInt16(DataArray, NewOffset);
            //NewOffset += 2;

            //for (int i = 0; i < readVars; i++)
            //{
            //    SymbolLookupRec symbol = new SymbolLookupRec();
            //    //NewOffset = symbol.Expand(reqType, DataArray, NewOffset);
            //}

            return NewOffset;
        }
    }

    public class SymbolicWriteAddrVars : IPackable
    {
        public class Var
        {
            UInt32 _CoherencyCookie = 0;
            byte[] _InternalAddress = new byte[12];
            byte[] _Data = new byte[0];

            public UInt32 CoherencyCookie { get { return _CoherencyCookie; } set { _CoherencyCookie = value; } }
            public byte[] InternalAddress { get { return _InternalAddress; } set { _InternalAddress = value; } }
            public byte[] Data { get { return _Data; } set { _Data = value; } }

            public ushort GetTotalWriteSize()
            {
                // CoherencyCookie + InternalAddress size
                return (ushort)(4 + 12 + _Data.Length);
            }
        }
        //Fields
        private UInt16 _Hrev = 2;
        private UInt16 _Req = (UInt16)ReqTypes.WRITE_ADDR_VAR;
        private byte[] _Checksums = new byte[4] { 0, 0, 0, 0 };
        private byte[] _DataType = new byte[4] { 0xff, 0xff, 0xff, 0xff };
        private byte[] _FileAttributeMask = new byte[4] { 0, 0, 0, 0 };
        private byte[] _DateTimeStamp = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        private UInt32 _DataRequestLength = 4;
        private UInt32 _BufferLength = 0;
        private UInt32 _OffsetNameField = 0;
        private UInt32 _OffsetDataField = 0x30;
        private byte[] _Spare = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 };        
        private string _NameField = string.Empty;
        private UInt16 _DRev = 1;        
        private List<Var> _WriteVars = new List<Var>();
                
        //Properties        
        public UInt32 DataRequestLength { get { return _DataRequestLength; } set { _DataRequestLength = value; } }
        public UInt32 BufferLength { get { return _BufferLength; } set { _BufferLength = value; } }
        public UInt32 OffsetDataField { get { return _OffsetDataField; } set { _OffsetDataField = value; } }

        public List<Var> WriteVars { get { return _WriteVars; } set { _WriteVars = value; } }
        

        //Methods
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(new byte[] { 0, 0, 0, 0, 0 });
            retVal.AddRange(BitConverter.GetBytes(_Hrev));
            retVal.AddRange(BitConverter.GetBytes(_Req));
            retVal.AddRange(_Checksums);
            retVal.AddRange(_DataType);
            retVal.AddRange(_FileAttributeMask);
            retVal.AddRange(_DateTimeStamp);

            _DataRequestLength = (UInt32)(2 + 4 + 4 + (16 * _WriteVars.Count));
            retVal.AddRange(BitConverter.GetBytes(_DataRequestLength));
            retVal.AddRange(BitConverter.GetBytes(_BufferLength));
            retVal.AddRange(BitConverter.GetBytes(_OffsetNameField));
            retVal.AddRange(BitConverter.GetBytes(_OffsetDataField));
            retVal.AddRange(new byte[] { 0, 0, 0, 0, 0, 0, 0, 0 });

            // c++ documentaion section
            retVal.AddRange(BitConverter.GetBytes(_DRev));
            Int32 lengthOfData = 4;
            foreach (Var var in _WriteVars)
                lengthOfData += var.GetTotalWriteSize();
            retVal.AddRange(BitConverter.GetBytes(lengthOfData));
            retVal.AddRange(BitConverter.GetBytes((UInt32)_WriteVars.Count));
            foreach (Var var in _WriteVars)
            {
                retVal.AddRange(BitConverter.GetBytes(var.CoherencyCookie));
                retVal.AddRange(var.InternalAddress);
            }

            foreach (Var var in _WriteVars)
                retVal.AddRange(var.Data);
            

            return retVal.ToArray();
        }
    }
    public class Mailbox : IPackable, IExpandable
    {
        //Fields
        private byte[] _MbuIntern = new byte[GESRTP2Protocol.MbuIntern_SIZE];
        private byte _Misc = 0;
        private byte _Seq = 0;
        private TrafficTypes _Typ = 0;
        private uint _Source = 0;
        private uint _Dest = 0;

        private byte[] _MbuIntern1 = new byte[GESRTP2Protocol.MbuIntern1_SIZE];
        private ushort _Buflen;
        private uint _Bufptr;

        private byte _Pkt;
        private byte _Tot;
        private ReqTypes _Req;

        private byte _Maj;
        private byte _Min;
        private byte[] _Data;
        private PiggyBackedStatus _PBStatus = new PiggyBackedStatus();
        private SmemSelect _SMSelect;
        private SmemSelectWrite _SMSelectWrite;
        private SymbolicDirRead _SymbolicDirread;
        private SymbolicLookUpRead _SymboliclookUpRead;
        private SymbolicReadAddrVars _SymbolicreadaddrVars;
        private SymbolicWriteAddrVars _Symbolicwriteaddrvars;

        //Properties
        public byte[] MbuIntern
        {
            get { return _MbuIntern; }
            set
            {
                _MbuIntern = new byte[GESRTP2Protocol.MbuIntern_SIZE];
                if (value.Length >= GESRTP2Protocol.MbuIntern_SIZE)
                {
                    Array.Copy(value, _MbuIntern, GESRTP2Protocol.MbuIntern_SIZE);
                }
                else
                {
                    Array.Copy(value, _MbuIntern, value.Length);
                }
            }
        }
        public byte Misc { get { return _Misc; } set { _Misc = value; } }
        public byte Seq { get { return _Seq; } set { _Seq = value; } }
        public TrafficTypes Typ
        {
            get { return _Typ; }
            set
            {
                _Typ = value;
                if (_Typ == TrafficTypes.InitialRequestTB)
                {
                    _Data = new byte[GESRTP2Protocol.RequestMailboxTbDataSize];
                }
                else
                {
                    _Data = new byte[GESRTP2Protocol.RequestMailboxDataSize];
                }
            }
        }
        public uint Source { get { return _Source; } set { _Source = value; } }
        public uint Dest { get { return _Dest; } set { _Dest = value; } }

        public byte[] MbuIntern1
        {
            set
            {
                _MbuIntern1 = new byte[GESRTP2Protocol.MbuIntern1_SIZE];
                if (value.Length >= GESRTP2Protocol.MbuIntern1_SIZE)
                {
                    Array.Copy(value, _MbuIntern1, GESRTP2Protocol.MbuIntern1_SIZE);
                }
                else
                {
                    Array.Copy(value, _MbuIntern1, value.Length);
                }
            }
        }
        public ushort Buflen { set { _Buflen = value; } }
        public uint Bufptr { set { _Bufptr = value; } }

        public byte Pkt { set { _Pkt = value; } }
        public byte Tot { set { _Tot = value; } }
        public ReqTypes Req { set { _Req = value; } }

        public byte Maj { set { _Maj = value; } get { return _Maj; } }
        public byte Min { set { _Min = value; } get { return _Min; } }    
        public byte[] Data {  get { return _Data; } }
        public PiggyBackedStatus PBStatus { get { return _PBStatus; } }
        public SmemSelect SMSelect { get { return _SMSelect; } }
        public SmemSelectWrite SMSelectWrite { get { return _SMSelectWrite; } }
        public SymbolicDirRead SymbolicDirread { get { return _SymbolicDirread; } }
        public SymbolicLookUpRead SymbolicLookUp { get { return _SymboliclookUpRead; } }
        public SymbolicReadAddrVars Symbolicreadaddrvars { get { return _SymbolicreadaddrVars; } }
        public SymbolicWriteAddrVars Symbolicwriteaddrvars { get { return _Symbolicwriteaddrvars; } }

        //Methods
        public void Init( byte seq, ReqTypes req, TrafficTypes typ, uint destination, UInt16 WriteLength = 0)
        {
            Misc = 0;
            Seq = seq;
            Typ = typ;
            Source = 0;
            Dest = destination;
            //Dest = GESRTP2Protocol.DEFAULT_DESTINATION;
            Pkt = 1;
            Tot = 1;
            Req = req;
            Buflen = WriteLength;
            Bufptr = 0;
        }

        public void InitService(ReqTypes req)
        {
            if (req == ReqTypes.READ_SMEM || req == ReqTypes.ALL_TYPES)
                _SMSelect = new SmemSelect();

            if (req == ReqTypes.WRITE_SMEM || req == ReqTypes.ALL_TYPES)
                _SMSelectWrite = new SmemSelectWrite();

            if (req == ReqTypes.READ_DIR || req == ReqTypes.ALL_TYPES)
                _SymbolicDirread = new SymbolicDirRead();

            if (req == ReqTypes.READ_SYMBOL_LOOKUP || req == ReqTypes.ALL_TYPES)
                _SymboliclookUpRead = new SymbolicLookUpRead();

            if (req == ReqTypes.READ_ADDR_VAR || req == ReqTypes.ALL_TYPES)
                _SymbolicreadaddrVars = new SymbolicReadAddrVars();

            if (req == ReqTypes.WRITE_ADDR_VAR || req == ReqTypes.ALL_TYPES)
                _Symbolicwriteaddrvars = new SymbolicWriteAddrVars();
        }

        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(_MbuIntern);
            retVal.Add(_Misc);
            retVal.Add(_Seq);
            retVal.Add((byte)_Typ);
            retVal.AddRange(BitConverter.GetBytes(_Source));
            retVal.AddRange(BitConverter.GetBytes(_Dest));
            if (_Typ == TrafficTypes.InitialRequestTB)
            {
                retVal.AddRange(_MbuIntern1);
                retVal.AddRange(BitConverter.GetBytes(_Buflen));
                retVal.AddRange(BitConverter.GetBytes(_Bufptr));
            }

            retVal.Add(_Pkt);
            retVal.Add(_Tot);
            retVal.Add((byte)_Req);

            switch (_Req)
            {
                case ReqTypes.READ_SMEM:
                    retVal.AddRange(_SMSelect.Pack());
                    break;
                case ReqTypes.WRITE_SMEM:
                    retVal.AddRange(_SMSelectWrite.Pack());
                    break;
                case ReqTypes.READ_DIR:
                    retVal.AddRange(_SymbolicDirread.Pack());
                    break;
                case ReqTypes.READ_SYMBOL_LOOKUP:
                    retVal.AddRange(_SymboliclookUpRead.Pack());
                    break;
                case ReqTypes.READ_ADDR_VAR:
                    retVal.AddRange(_SymbolicreadaddrVars.Pack());
                    break;
                case ReqTypes.WRITE_ADDR_VAR:
                    retVal.AddRange(_Symbolicwriteaddrvars.Pack());
                    break;
            }

            if ( GESRTP2Protocol.Mailbox_SIZE > retVal.Count())
            {
                _Data = new byte[GESRTP2Protocol.Mailbox_SIZE - retVal.Count()];
                retVal.AddRange(_Data);
            }
            
            return retVal.ToArray();
        }

        public int Expand(ReqTypes reqType, byte[] DataArray, int Offset = 0)
        {
            int NewOffset = Offset;

            Buffer.BlockCopy(DataArray, NewOffset, _MbuIntern, 0, _MbuIntern.Length);
            NewOffset += _MbuIntern.Length;

            _Misc = DataArray[NewOffset];
            NewOffset += 1;
            _Seq = DataArray[NewOffset];
            NewOffset += 1;
            _Typ = (TrafficTypes)DataArray[NewOffset];
            NewOffset += 1;
            _Source = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            _Dest = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            if (_Typ == TrafficTypes.InitialRequestTB)
            {
                Buffer.BlockCopy(DataArray, NewOffset, _MbuIntern1, 0, _MbuIntern1.Length);
                NewOffset += _MbuIntern1.Length;
                _Buflen = BitConverter.ToUInt16(DataArray, NewOffset);
                NewOffset += 2;
                _Bufptr = BitConverter.ToUInt32(DataArray, NewOffset);
                NewOffset += 4;
            }
            _Pkt = DataArray[NewOffset];
            NewOffset += 1;
            _Tot = DataArray[NewOffset];
            NewOffset += 1;
            if (_Typ != TrafficTypes.InitialRequestTB)
            {
                _Maj = DataArray[NewOffset];
                NewOffset += 1;
                _Min = DataArray[NewOffset];
                NewOffset += 1;
                _Data = new byte[GESRTP2Protocol.ImediateBufferSize];
                Buffer.BlockCopy(DataArray, NewOffset, _Data, 0, _Data.Length);
                NewOffset += _Data.Length;
            }

            NewOffset = _PBStatus.Expand(reqType, DataArray, NewOffset);
            
            return NewOffset;
        }
    }
 
    /// <summary>
    /// Encapsulated PDU
    /// </summary>
    public class EncapsPDU : IPackable, IExpandable
    {
        //Fields
        private PduTypes _PduType = 0;
        private byte _Version = 0;
        private UInt16 _InvokeId = 0;
        private uint _DataLength = 0;
        private uint _Capability = 0;
        private Control _Req = new Control();
        private Control _Rsp = new Control();
        private Mailbox _Mailbox = new Mailbox();
        private byte[] _EncapsData = null;

        //Properties
        public PduTypes PduType { get { return _PduType; } set { _PduType = value; } }
        public byte Version { get { return _Version; } set { _Version = value; } }
        public UInt16 InvokeId { get { return _InvokeId; } set { _InvokeId = value; } }
        public uint DataLength { get { return _DataLength; } set { _DataLength = value; } }
        public uint Capability { get { return _Capability; } }
        public Control Req { get { return _Req; } }
        public Control Rsp { get { return _Rsp; } }
        public Mailbox Mailbox { get { return _Mailbox; } }
        public byte[] EncapsData 
        {
            get { return _EncapsData; }
            set
            {
                _EncapsData = new byte[value.Length];
                Array.Copy(value, _EncapsData, value.Length);
            }
        }

        public EncapsPDU()
        {

        }

        public EncapsPDU(ReqTypes req)
        {
            Mailbox.InitService(req);
        }

        //Methods
        public void Init( PduTypes pduType, UInt16 invokeId,
                                    uint dataLength = 0)
        {
            PduType = pduType;
            Version = 0;
            InvokeId = invokeId;
            DataLength = dataLength;

            //req_control
            Req.Command = Commands.SRTP_NO_CMD;
            if (dataLength > 0)
            {
                Req.MessageType = MessageTypes.SRTP_HP_MAIL_MESS_W_TEXT;
            }
            else
            {
                Req.MessageType = MessageTypes.SRTP_MAIL_MESSAGE;
            }
            Req.MbOffset = 0;
            Req.Length = 0;

            //rsp_control
            Rsp.Command = Commands.SRTP_NO_CMD;
            Rsp.MessageType = MessageTypes.SRTP_MAIL_MESSAGE;
            Rsp.MbOffset = 0;
            Rsp.Length = 0;

        }

        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add((byte)_PduType);
            retVal.Add(_Version);
            retVal.AddRange(BitConverter.GetBytes(_InvokeId));
            retVal.AddRange(BitConverter.GetBytes(_DataLength));
            retVal.AddRange(_Req.Pack());
            retVal.AddRange(_Rsp.Pack());
            retVal.AddRange(_Mailbox.Pack());
            if (_EncapsData != null && _EncapsData.Length > 0)
                retVal.AddRange(_EncapsData);

            return retVal.ToArray();
        }

        public int Expand(ReqTypes reqType, byte[] DataArray, int Offset = 0)
        {
            if (DataArray.Length < Offset + GESRTP2Protocol.Pdu_SIZE)
                throw new IndexOutOfRangeException("Not enough data in the DataArray for the encapsulated packet");

            int NewOffset = Offset;

            _PduType = (PduTypes)DataArray[NewOffset];
            NewOffset += 1;
            _Version = DataArray[NewOffset];
            NewOffset += 1;
            _InvokeId = BitConverter.ToUInt16(DataArray, NewOffset);
            NewOffset += 2;
            _DataLength = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset += 4;
            _Capability = BitConverter.ToUInt32(DataArray, NewOffset);
            NewOffset = Req.Expand(reqType, DataArray, NewOffset);
            NewOffset = Rsp.Expand(reqType, DataArray, NewOffset);
            NewOffset = _Mailbox.Expand(reqType, DataArray, NewOffset);

            if (DataArray.Length > NewOffset)
            {
                int EncapsDataSize = DataArray.Length - NewOffset;

                EncapsData = new byte[EncapsDataSize];
                Buffer.BlockCopy(DataArray, 24, EncapsData, 0, EncapsDataSize);
                NewOffset += EncapsDataSize;
            }
            return NewOffset;
        }
    }

    public static class EncapsPDUFactory
    {
        public static EncapsPDU Connect()
        {
            EncapsPDU Pdu = new EncapsPDU();
            return Pdu;
        }

        private static uint GetRackSlotDestination(Station st)
        {
            return (uint)(((GESRTP2Station)st).Rack + (((GESRTP2Station)st).Slot << 4)) + GESRTP2Protocol.DEFAULT_DESTINATION_2;
        }

        public static EncapsPDU Session(SessionEnables SessionEnable, byte Seq, UInt16 InvokeId, uint destination)
        {
            EncapsPDU Pdu = new EncapsPDU(ReqTypes.SESSION_CONTROL);
            Pdu.Init(PduTypes.SessionRequest, InvokeId);
            Pdu.Mailbox.Init(Seq, ReqTypes.SESSION_CONTROL, TrafficTypes.InitialRequest, destination);
            Pdu.Mailbox.Data[0] = (byte)SessionEnable;
            return Pdu;
        }

        public static EncapsPDU DataAreaRead(GESRTP2CommJob Job, GESRTP2Channel channel)
        {            
            EncapsPDU Pdu = new EncapsPDU(ReqTypes.READ_SMEM);
            Pdu.Init(PduTypes.DataRequest, channel.mInvokeId);            
            Pdu.Mailbox.Init(channel.mSeq, ReqTypes.READ_SMEM, TrafficTypes.InitialRequest, GetRackSlotDestination(Job.Station));
            Pdu.Mailbox.SMSelect.SegSelector = Job.AreaType;
            Pdu.Mailbox.SMSelect.SegOffset = (UInt16)(Job.StartAddress - 1);
            Pdu.Mailbox.SMSelect.Length =
                    //number of PLC memory units (not nr of bytes) 
                    (ushort)(Job.TotalJobSize / Job.GetProtocolDataByteSize());

            return Pdu;
        }

        public static EncapsPDU SymbolicReadDir(GESRTP2CommJob Job, GESRTP2Channel channel)
        {
            EncapsPDU Pdu = new EncapsPDU(ReqTypes.READ_DIR);
            Pdu.Init(PduTypes.DataRequest, channel.mInvokeId, 0x44);            
            Pdu.Mailbox.Init(channel.mSeq, ReqTypes.READ_DIR, TrafficTypes.InitialRequestTB, GetRackSlotDestination(Job.Station), (UInt16)Pdu.DataLength);
            Pdu.Mailbox.SymbolicDirread.NameField = GESRTP2Protocol.LOOKUP_TABLE_START_DIR;

            return Pdu;
        }

        public static EncapsPDU SymbolicReadInfo(List<CommJob> list, GESRTP2Channel channel, uint dataLength)
        {
            EncapsPDU Pdu = new EncapsPDU(ReqTypes.READ_SYMBOL_LOOKUP);
            Pdu.Init(PduTypes.DataRequest, channel.mInvokeId, dataLength);
            Pdu.Req.MessageType = MessageTypes.SRTP_MAIL_MESSAGE_W_TEXT;
            Pdu.Rsp.MessageType = MessageTypes.SRTP_MAIL_MESSAGE_W_TEXT;
            Pdu.Mailbox.Init(channel.mSeq, ReqTypes.READ_SYMBOL_LOOKUP, TrafficTypes.InitialRequestTB, GetRackSlotDestination(list[0].Station));
            Pdu.Mailbox.Buflen = (ushort)dataLength;
            Pdu.Mailbox.SymbolicLookUp.NameField = String.Format("{0}{1}", ((GESRTP2Station)list[0].Station).Dir, ((GESRTP2Station)list[0].Station).LookUpTable);
            Pdu.Mailbox.SymbolicLookUp.BufferLength = dataLength;
            foreach (GESRTP2CommJob job in list)
                Pdu.Mailbox.SymbolicLookUp.RequestSymbols.Add(new GEF_STRING(job.SymbolicAddress));

            return Pdu;
        }

        public static EncapsPDU SymbolicReadVars(List<CommJob> list, GESRTP2Channel channel, uint dataLength)
        {
            EncapsPDU Pdu = new EncapsPDU(ReqTypes.READ_ADDR_VAR);
            Pdu.Init(PduTypes.DataRequest, channel.mInvokeId, dataLength);
            Pdu.Req.MessageType = MessageTypes.SRTP_MAIL_MESSAGE_W_TEXT;
            Pdu.Rsp.MessageType = MessageTypes.SRTP_MAIL_MESSAGE_W_TEXT;
            Pdu.Mailbox.Init(channel.mSeq, ReqTypes.READ_ADDR_VAR, TrafficTypes.InitialRequestTB, GetRackSlotDestination(list[0].Station));            
            Pdu.Mailbox.Buflen = (ushort)dataLength;            
            Pdu.Mailbox.Symbolicreadaddrvars.BufferLength = dataLength;

            foreach (GESRTP2CommJob job in list)
            {
                job.GetSymbolicAddressDeviceAddress(out UInt32 coherencyCookie, out byte[] deviceInternalAddress);

                SymbolicReadAddrVars.Var varRec = new SymbolicReadAddrVars.Var();
                varRec.CoherencyCookie = coherencyCookie;
                varRec.InternalAddress = deviceInternalAddress;
                
                Pdu.Mailbox.Symbolicreadaddrvars.ReadVars.Add(varRec);
            }

            return Pdu;
        }

        public static EncapsPDU SymbolicWriteVars(List<CommJob> list, GESRTP2Channel channel, uint dataLength)
        {
            EncapsPDU Pdu = new EncapsPDU(ReqTypes.WRITE_ADDR_VAR);
            Pdu.Init(PduTypes.DataRequest, channel.mInvokeId, dataLength);
            Pdu.Req.MessageType = MessageTypes.SRTP_MAIL_MESSAGE_W_TEXT;
            Pdu.Rsp.MessageType = MessageTypes.SRTP_MAIL_MESSAGE_W_TEXT;
            Pdu.Mailbox.Init(channel.mSeq, ReqTypes.WRITE_ADDR_VAR, TrafficTypes.InitialRequestTB, GetRackSlotDestination(list[0].Station));
            Pdu.Mailbox.Buflen = (ushort)dataLength;
            Pdu.Mailbox.Symbolicwriteaddrvars.BufferLength = dataLength;

            foreach (GESRTP2CommJob job in list)
            {
                job.GetSymbolicAddressDeviceAddress(out UInt32 coherencyCookie, out byte[] deviceInternalAddress);

                SymbolicWriteAddrVars.Var varRec = new SymbolicWriteAddrVars.Var();
                varRec.CoherencyCookie = coherencyCookie;
                varRec.InternalAddress = deviceInternalAddress;
                varRec.Data = job.writeData.ToArray();

                Pdu.Mailbox.Symbolicwriteaddrvars.WriteVars.Add(varRec);
            }

            return Pdu;
        }

        public static EncapsPDU DataAreaWrite(GESRTP2CommJob Job, GESRTP2Channel channel)
        {
            ushort writeElements = 0;
            EncapsPDU Pdu = new EncapsPDU(ReqTypes.WRITE_SMEM);
            //System.Diagnostics.Trace.TraceInformation(string.Format("GESRTP2 DBG - Write  t {0} writeData {1}",
            //                               DateTime.Now.ToString("HH:mm:ss.fff"), Job.writeData.Count));
                        
            byte[] WriteData;
            Pdu.Mailbox.SMSelectWrite.SegOffset = (UInt16)(Job.StartAddress + Job.writeOffset - 1);
            if (GESRTP2Protocol.DataType(Job.AreaType) != BuiltInType.Boolean)
            {
                WriteData = Job.writeData.GetRange(0, Job.writeData.Count()).ToArray();
                writeElements = (ushort)(WriteData.Length / Job.elementSize);
            }
            else
            {
                WriteData = DataAreaPrepareSendData(Job.writeData.ToArray(), Pdu.Mailbox.SMSelectWrite.SegOffset, ref writeElements);
            }

            Pdu.Init(PduTypes.DataRequest, channel.mInvokeId, (uint)WriteData.Length);
            Pdu.EncapsData = WriteData;            
            Pdu.Mailbox.Init(channel.mSeq, ReqTypes.WRITE_SMEM, TrafficTypes.InitialRequestTB, GetRackSlotDestination(Job.Station), (UInt16)Pdu.DataLength);
            Pdu.Mailbox.SMSelectWrite.SegSelector = Job.AreaType;                
            Pdu.Mailbox.SMSelectWrite.Length = writeElements;                
            Job.writeData.RemoveRange(0, (int)(writeElements * Job.elementSize));
            Job.writeOffset += writeElements;

            return Pdu;
        }

        static byte[] DataAreaPrepareSendData(byte[] inData, ushort Seg_Offset, ref ushort writeElements)
        {
            // Command Data
            //bits, we must pack them into one or more bytes
            int nByteOffs = Seg_Offset % 8;
            int nTotBits = inData.Length;
            int nTotalBytes = (inData.Length + nByteOffs + 7) / 8;

            byte[] outData = new byte[nTotalBytes];

            ushort nCount = 0;
            for (ushort i = 0; i < nTotalBytes; i++)
            {
                byte byPack = 0;
                for (ushort nBit = 0; nBit < 8; nBit++)
                {
                    if (nBit >= nByteOffs)
                    {
                        byPack >>= 1;
                        if (nCount < nTotBits)
                        {
                            byPack |= (byte)(inData[nCount++] << 7);
                        }
                    }
                }
                nByteOffs = 0;
                outData[i] = byPack;
            }
            writeElements = nCount;
            return outData;
        }
    }

    /// <summary>   Communication protocol of GESRTP2 driver. </summary>
    public static class GESRTP2Protocol
    {
        #region const

        public const int Pdu_SIZE = 56;
        public const int PduWriteBufferSize = 8;
        public const int MbuIntern_SIZE = 5;
        public const int MbuIntern1_SIZE = 2;
        public const int Mailbox_SIZE = 32;
        public const int RequestMailboxDataSize = 13;
        public const int RequestMailboxTbDataSize = 5;
        public const int ImediateBufferSize = 6;
        public const int PLCTYPE_PAC_MAX_PDU_SIZE = 0x800;  //2048 bytes
        public const int PLCTYPE_SERIES90_MAX_PDU_SIZE = 0x4000;    //16384 bytes
        public const int MAX_DATA_BYTES = 0x4000;    //16384 bytes
        public const int DEFAULT_DESTINATION = 0x00000e10;
        public const int DEFAULT_DESTINATION_2 = 0x00000e00;
        
        public const string TEST_COMM_DYNAMIC_SETTINGS = "GESRTP2.Station={0}|LinkType=1|AT=8|SA=1";

        public const string LOOKUP_TABLE_START_DIR = "/ram/";
        public const string LOOKUP_TABLE_NAME = "logic/PUBLISHED.PVT";

        public const int DATA_AREA_DEFAULT_STRING_SIZE = 32;
        public const int DATA_AREA_MAX_STRING_SIZE = 240;

        public const int SYMBOLIC_DEFAULT_STRING_SIZE = 1;

        public enum PAC_MAJOR_CODE_ERROR : byte
        {
            ILLEGAL_REQUEST_SRP_MAJOR_CODE = 1,
            SRP_INSUFFICIENT_PRIVILEGE = 2,
            PROTOCOL_INTERNAL_ERROR = 60,
            PROTOCOL_SEQUENCE_ERROR = 4,
            SRP_REQUEST_ERROR = 5,
            QUEUE_FULL = 7
        }

        public enum PAC_MINOR_CODE_SRP_REQUEST_ERROR : byte
        {
            SRP_REQ_ERROR_POINT_FORMAT_OBSOLETE = 111
        }

        public enum PlcTypes
        {
            PacSystem = 0,
            Series90_30
        }

        #endregion

        #region methods        
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return maximum memory size of GESRTP2CommJob objects. </summary>
        ///
        /// <returns>   The maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static uint GetMaxJobSize(GESRTP2Protocol.PlcTypes plcType)
        {
            switch (plcType)
            {
                case GESRTP2Protocol.PlcTypes.Series90_30:
                    return GESRTP2Protocol.PLCTYPE_SERIES90_MAX_PDU_SIZE;

                case GESRTP2Protocol.PlcTypes.PacSystem:
                    return GESRTP2Protocol.PLCTYPE_PAC_MAX_PDU_SIZE;
            }

            return GESRTP2Protocol.MAX_DATA_BYTES;
        }

        public static BuiltInType DataType(AreaTypes AreaType, BuiltInType symbolicAddressDataType = BuiltInType.Boolean)
        {
            switch (AreaType)
            {
                case AreaTypes.DiscreteBytes_SA:
                case AreaTypes.DiscreteBytes_SB:
                case AreaTypes.DiscreteBytes_SC:
                case AreaTypes.DiscreteInputBytes_I:
                case AreaTypes.DiscreteInternalBytes_M:
                case AreaTypes.DiscreteOutputBytes_Q:
                case AreaTypes.DiscreteTemporaryBytes_T:
                case AreaTypes.GeniusGlobalDataBytes_G:
                case AreaTypes.DiscreteBytes_S_readOnly:
                    return BuiltInType.Byte;
                case AreaTypes.AnalogInputWords_AI:
                case AreaTypes.AnalogOutputWords_AQ:
                case AreaTypes.RegisterWords_R:
                case AreaTypes.WordMemory_W:
                    return BuiltInType.UInt16;
                case AreaTypes.DiscreteBits_SA:
                case AreaTypes.DiscreteBits_SB:
                case AreaTypes.DiscreteBits_SC:
                case AreaTypes.DiscreteInputBits_I:
                case AreaTypes.DiscreteInternalBits_M:
                case AreaTypes.DiscreteOutputBits_Q:
                case AreaTypes.DiscreteTemporaryBits_T:
                case AreaTypes.GeniusGlobalDataBits_G:
                case AreaTypes.DiscreteBits_S_readOnly:
                    return BuiltInType.Boolean;
                case AreaTypes.Symbolic:
                    return symbolicAddressDataType;
                default:
                    return BuiltInType.String;
            }
        }

        public static int GetAreaDataSize(AreaTypes AreaType, BuiltInType symbolicAddressDataType = BuiltInType.Boolean)
        {
            switch (DataType(AreaType, symbolicAddressDataType))
            {                
                case BuiltInType.SByte:
                case BuiltInType.Byte:
                    return 1;
                case BuiltInType.Int16:
                case BuiltInType.UInt16:
                    return 2;
                case BuiltInType.Int32:
                case BuiltInType.UInt32:
                    return 4;
                case BuiltInType.Int64:
                case BuiltInType.UInt64:
                    return 4;
                case BuiltInType.Float:
                    return 4;
                case BuiltInType.Double:
                    return 8;
                case BuiltInType.String:
                    return 1;
                default:
                    return 1;
            }        
        }

        public static LinkType AreaLinkType(AreaTypes AreaType)
        {
            switch (AreaType)
            {
                case AreaTypes.DiscreteBytes_S_readOnly:
                case AreaTypes.DiscreteBits_S_readOnly:
                    return LinkType.Input;
                default:
                    return LinkType.InputOutput;
            }
        }

        public static bool InvalidAreaTypeLinkType(AreaTypes AreaType, LinkType LinkType)
        {
            return (AreaLinkType(AreaType) == LinkType.Input && LinkType != LinkType.Input);
        }

        public static bool InvalidAreaDataType(AreaTypes AreaType, UFUAModel.DataType Type)
        {
            return(DataType(AreaType) == BuiltInType.Boolean && Type != UFUAModel.DataType.Boolean);
        }

        public static bool InvalidAreaDataType(AreaTypes AreaType, NodeId Type)
        {
            return (DataType(AreaType) == BuiltInType.Boolean && (uint)Type.Identifier != (uint)BuiltInType.Boolean);
        }

        public static bool IsSymbolic(List<CommJob> list)
        {
            return (list.Count > 0 && ((GESRTP2CommJob)list[0]).AreaType == AreaTypes.Symbolic);
        }

        public static bool IsSymbolic(CommJob job)
        {
            return IsSymbolic(job.TagsList[0]);
        }

        public static bool IsSymbolic(Tag tag)
        {
            return (((GESRTP2DynTagSettings)tag.DynSettings).AreaType == AreaTypes.Symbolic);
        }        

        public static string GetNodeTree(NodeId NodeId, string Name)
        {
            string Out = "";

            //System.Text.RegularExpressions.Regex NameParser = new System.Text.RegularExpressions.Regex(@"^[\d]+:(?<Name>[\w]+)$");
            System.Text.RegularExpressions.Regex NameParser = new System.Text.RegularExpressions.Regex(@"[\d]+:(?<Name>[\w]+)$");
            System.Text.RegularExpressions.Match NameMatch = NameParser.Match(Name);
            if (NameMatch.Success)
            {
                Out = NameMatch.Groups["Name"].Value;
                string nameNodeId = NodeId.Identifier.ToString();
                System.Text.RegularExpressions.Regex NodeParser = new System.Text.RegularExpressions.Regex(@"^[^?]+[?](?<Node>[\w/]+)/[\w-]+$");
                System.Text.RegularExpressions.Match NodeMatch = NodeParser.Match(nameNodeId);
                if (NodeMatch.Success)
                {
                    Out = NodeMatch.Groups["Node"].Value + "." + Out;
                }
            }
            return Out;

        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Load in memory "buffer" the request frame for protocol's task "job". </summary>
        ///
        /// <param name="job">      . </param>
        /// <param name="buffer">   [in,out]. </param>
        ///
        /// <returns>   An uint. </returns>
        public static bool PrepareRequestDataArea(GESRTP2Channel channel, List<CommJob> list, out byte[] result, out int waitByte)
        {
            GESRTP2CommJob mJob = list[0] as GESRTP2CommJob;
            bool request = false;

            result = new byte[0];
            waitByte = 0;

            if ((DataType(mJob.AreaType) == BuiltInType.Boolean ? (((mJob.StartAddress - 1) % 8) + mJob.TotalJobSize + 7) / 8 : mJob.TotalJobSize) > mJob.GetMaxJobSize())
            {
            }
            else
            {
                if (mJob.ReadRequest())
                {
                    mJob.readData.Clear();
                    mJob.writeData.Clear();
                    mJob.onWrite = false;
                    result = EncapsPDUFactory.DataAreaRead(mJob, channel).Pack();
                    request = true;
                }
                else
                {
                    mJob.readData.Clear();
                    mJob.writeData.Clear();
                    mJob.onWrite = true;

                    //prepare a write request
                    object objectData = null;
                    mJob.GetJobData(ref objectData);
                    if (mJob.GetTagListOnWritingCount() != 0)
                    {
                        byte[] jobdata = (byte[])objectData;

                        //mJob.writeData.Clear();
                        mJob.writeData.AddRange(jobdata);
                        mJob.writeOffset = (ushort)(mJob.GetTagsListOnWritingByteOffset() / mJob.GetProtocolDataByteSize());

                        result = EncapsPDUFactory.DataAreaWrite(mJob, channel).Pack();
                        request = true;
                    }
                }
            }

            if (request)
                waitByte = (int)(GESRTP2Protocol.Pdu_SIZE + (GESRTP2Protocol.JobByteSize(mJob) > GESRTP2Protocol.ImediateBufferSize && !mJob.onWrite ? GESRTP2Protocol.JobByteSize(mJob) : 0));

            return request;
        }

        public static bool PrepareRequestSymbolicDir(GESRTP2Channel channel, GESRTP2CommJob job, out byte[] result, out int waitByte)
        {            
            result = EncapsPDUFactory.SymbolicReadDir(job, channel).Pack();
            if (result != new byte[0])            
                waitByte = (int)(GESRTP2Protocol.Pdu_SIZE);
            else
                waitByte = 0;

            return result != new byte[0];
        }

        public static List<CommJob> SymbolicGetJobsNeedInfo(List<CommJob> list)
        {
            return (from job in list
                    where !((GESRTP2CommJob)job).HasSymbolicAddressDevideInfo()
                    select job).ToList();
        }

        public static GESRTP2CommJob.GEState GetLowerJobState(List<CommJob> list)
        {
            var firstJob = (from job in list
                          orderby ((GESRTP2CommJob)job).GeState ascending
                          select job).First();

            return ((GESRTP2CommJob)firstJob).GeState;
        }

        public static bool PrepareRequestSymbolicInfo(GESRTP2Channel channel, List<CommJob> list, out byte[] result, out int waitByte)
        {
            result = new byte[0];
            waitByte = 0;

            List<CommJob> jobsNeedSymbolic = GESRTP2Protocol.SymbolicGetJobsNeedInfo(list);
            if (jobsNeedSymbolic != null && jobsNeedSymbolic.Count > 0)
            {
                // execute 1st time to get the message size
                result = EncapsPDUFactory.SymbolicReadInfo(jobsNeedSymbolic, channel, 0).Pack();
                if (result != new byte[0])
                {
                    result = EncapsPDUFactory.SymbolicReadInfo(jobsNeedSymbolic, channel, 0).Pack();
                    if (result != new byte[0])
                    {
                        uint dataLength = (uint)result.Length - GESRTP2Protocol.Pdu_SIZE;
                        // pass the message size as parameter
                        result = EncapsPDUFactory.SymbolicReadInfo(jobsNeedSymbolic, channel, dataLength).Pack();

                        waitByte = (int)(GESRTP2Protocol.Pdu_SIZE);
                    }
                }
            }

            return (result != new byte[0]);
        }

        public static bool PrepareRequestSymbolicReadWriteVars(GESRTP2Channel channel, List<CommJob> list, out byte[] result, out int waitByte)
        {
            bool request = false;

            waitByte = 0;

            if (list[0].ReadRequest())
            {
                foreach (GESRTP2CommJob mJob in list)
                {
                    mJob.readData.Clear();
                    mJob.writeData.Clear();
                    mJob.onWrite = false;
                }

                request = PrepareRequestSymbolicReadVars(channel, list, out result, out waitByte);
            }
            else
            {
                foreach (GESRTP2CommJob mJob in list)
                {
                    mJob.readData.Clear();
                    mJob.writeData.Clear();
                    mJob.onWrite = true;

                    //prepare a write request
                    object objectData = null;
                    mJob.GetJobData(ref objectData);
                    if (mJob.GetTagListOnWritingCount() != 0)
                        mJob.writeData.AddRange((byte[])objectData);                    
                }

                request = PrepareRequestSymbolicWriteVars(channel, list, out result, out waitByte);
            }

            return request;
        }

        public static bool PrepareRequestSymbolicReadVars(GESRTP2Channel channel, List<CommJob> list, out byte[] result, out int waitByte)
        {
            waitByte = 0;
            
            // execute 1st time to get the message size
            result = EncapsPDUFactory.SymbolicReadVars(list, channel, 0).Pack();
            if (result != new byte[0])
            {                
                uint dataLength = (uint)result.Length - GESRTP2Protocol.Pdu_SIZE;
                //// pass the message size as parameter
                result = EncapsPDUFactory.SymbolicReadVars(list, channel, dataLength).Pack();

                waitByte = (int)(GESRTP2Protocol.Pdu_SIZE);                
            }

            return (result != new byte[0]);
        }


        public static bool PrepareRequestSymbolicWriteVars(GESRTP2Channel channel, List<CommJob> list, out byte[] result, out int waitByte)
        {            
            waitByte = 0;

            // execute 1st time to get the message size
            result = EncapsPDUFactory.SymbolicWriteVars(list, channel, 0).Pack();
            if (result != new byte[0])
            {
                uint dataLength = (uint)result.Length - GESRTP2Protocol.Pdu_SIZE;
                //// pass the message size as parameter
                result = EncapsPDUFactory.SymbolicWriteVars(list, channel, dataLength).Pack();

                waitByte = (int)(GESRTP2Protocol.Pdu_SIZE);                
            }

            return (result != new byte[0]);
        }

        public static int JobByteSize(GESRTP2CommJob mJob)
        {
            return (int)(GESRTP2Protocol.DataType(mJob.AreaType) == BuiltInType.Boolean ? (((mJob.StartAddress - 1) % 8) + mJob.TotalJobSize + 7) / 8 : mJob.TotalJobSize);
        }

        public static bool PlcSupportSymbolic(PlcTypes plc)
        {
            return (plc == PlcTypes.PacSystem);
        }

        private static void GetPlcSymbolicInitialFrameSize(GESRTP2CommJob j, bool write, ref ReadWriteListLimitateSize frameSize)
        {
            frameSize.ProtocolSize = GESRTP2Protocol.GetMaxJobSize(((GESRTP2Station)j.Station).PlcType);

            frameSize.TotalRequestSize = 161; // put here the correct read size
            frameSize.TotalResponseSize = 56;
            
            if (write)
            {
                uint writeTotalResponseSize = 114;
                uint writeTotalRequestSize = 56;

                frameSize.TotalRequestSize = (frameSize.TotalRequestSize > writeTotalResponseSize ? frameSize.TotalRequestSize : writeTotalResponseSize);
                frameSize.TotalResponseSize = (frameSize.TotalResponseSize > writeTotalRequestSize ? frameSize.TotalResponseSize : writeTotalRequestSize);
            }
        }

        private static void GetPlcSymbolicJobRequestResponseSize(GESRTP2CommJob j, bool write, out uint requestSize, out uint responseSize)
        {            
            uint prepareRequestSymbolicInfoRequest = 0;
            uint prepareRequestSymbolicInfoResponse = 0;
            if (j.GeState < GESRTP2CommJob.GEState.SymbolicPolling)
            {
                prepareRequestSymbolicInfoRequest = (uint)(j.SymbolicAddress.Length + 5); // (VarLen + 5)
                prepareRequestSymbolicInfoResponse = (8 + 16); // 56 + Var + (8 + 16)
            }
            uint prepareRequestSymbolicReadVarsRequest = 16; // 114 + (VarInfo --> 16 bytes)
            uint prepareRequestSymbolicReadVarsResponse = (j.SymbolicAddressVariableLength == 0 ? j.GetSymbolicAddressVariableLengthFromTotalJobSize() : j.SymbolicAddressVariableLength);

            requestSize = (prepareRequestSymbolicInfoRequest > prepareRequestSymbolicReadVarsRequest ? prepareRequestSymbolicInfoRequest : prepareRequestSymbolicReadVarsRequest);
            responseSize = (prepareRequestSymbolicInfoResponse > prepareRequestSymbolicReadVarsResponse ? prepareRequestSymbolicInfoResponse : prepareRequestSymbolicReadVarsResponse);

            if (write)
            {
                uint requestSymbolicWriteVarsRequest = (8 + 16) + j.GetwriteDataLength();   // 56 + Var + (8 + 16 + Data to write)
                responseSize = 10;                
                requestSize = (requestSize > requestSymbolicWriteVarsRequest ? requestSize : requestSymbolicWriteVarsRequest);
                responseSize = (prepareRequestSymbolicInfoResponse > prepareRequestSymbolicReadVarsResponse ? prepareRequestSymbolicInfoResponse : prepareRequestSymbolicReadVarsResponse);
            }        
        }

        public static bool getSymbolicReadWriteListLimitate(GESRTP2CommJob j, bool write, ref ReadWriteListLimitateSize frameSize)
        {
            if (frameSize.IsEmpty())
                GetPlcSymbolicInitialFrameSize(j, write, ref frameSize);

            GetPlcSymbolicJobRequestResponseSize(j, write, out uint requestSize, out uint responseSize);

            // add job if request size fit protocol size or is the only job to schedule 
            bool add = ((frameSize.TotalRequestSize + requestSize <= frameSize.ProtocolSize) && (frameSize.TotalResponseSize + responseSize <= frameSize.ProtocolSize) || frameSize.TotalNrJobs == 0);
            if (add)
            {
                frameSize.TotalRequestSize += requestSize;
                frameSize.TotalResponseSize += responseSize;
                frameSize.TotalNrJobs++;
            }
            return add;
        }

        public static bool IsSymbolicDataCompatibleWithMoviconDataType(SymbolicDataType symbolicDataType, BuiltInType moviconDataType)
        {
            switch (symbolicDataType)
            {
                case SymbolicDataType.Boolean:
                    return (moviconDataType == BuiltInType.Boolean);
                case SymbolicDataType.Byte:
                    return (moviconDataType == BuiltInType.Byte || moviconDataType == BuiltInType.SByte);
                case SymbolicDataType.DInt:
                    return (moviconDataType == BuiltInType.Int32);
                case SymbolicDataType.DWord:
                    return (moviconDataType == BuiltInType.UInt32);
                case SymbolicDataType.Int:                
                    return (moviconDataType == BuiltInType.Int16);
                case SymbolicDataType.Word:
                case SymbolicDataType.UInt:
                    return (moviconDataType == BuiltInType.UInt16);
                case SymbolicDataType.LReal:
                    return (moviconDataType == BuiltInType.Double);
                case SymbolicDataType.Real:
                        return (moviconDataType == BuiltInType.Float);
                case SymbolicDataType.String:
                    return (moviconDataType == BuiltInType.String);
                case SymbolicDataType.Undefined:
                    return false;
                default:
                    return false;
            }    
        }
        #endregion

        #region methods override

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Validation of the response data type input. If correct copy data in job. </summary>
        ///
        /// <param name="receivebuffer">                    . </param>
        /// <param name="job">                              [in,out]. </param>
        /// <param name="items" type="ref List<object>">    [in,out]. </param>
        ///
        /// <returns>   true if it succeeds, false if it fails. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static bool ParseData(byte[] receivedbuffer, ref GESRTP2CommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);
            if (!areArguments && (receivedbuffer == null))
            {
                return (false);
            }
            else if (receivedbuffer == null)
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

            uint recivedData = (uint)receivedbuffer.Length;
            if (job.isProtocolBool() &&
                (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || job.ElementNumber > 0))
                recivedData *= 8;
            if (job.TotalJobSize < recivedData)
            {
                recivedData = job.TotalJobSize;
            }
            byte[] tempBuffer = new byte[recivedData];
            if (job.isProtocolBool() &&
                (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || job.ElementNumber > 0))
            {
                ushort bitIndex = (ushort)((job.StartAddress - 1) % 8);
                for (ushort Index = 0; Index < recivedData; Index++, bitIndex++)
                {
                    tempBuffer[Index] = (byte)((receivedbuffer[bitIndex / 8] >> (bitIndex % 8)) & 1);
                }
            }
            else
                Array.Copy(receivedbuffer, tempBuffer, recivedData);
            
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

        public static UFUAModel.DataType GetUFUAModelDataTypeFromBuiltType(BuiltInType type)
        {
            switch (type)
            {
                case BuiltInType.SByte:
                    return UFUAModel.DataType.SByte;
                case BuiltInType.Byte:
                    return UFUAModel.DataType.Byte;
                case BuiltInType.Int16:
                    return UFUAModel.DataType.Int16;
                case BuiltInType.UInt16:
                    return UFUAModel.DataType.UInt16;
                case BuiltInType.Int32:
                    return UFUAModel.DataType.Int32;
                case BuiltInType.UInt32:
                    return UFUAModel.DataType.UInt32;
                case BuiltInType.Int64:
                    return UFUAModel.DataType.Int64;
                case BuiltInType.UInt64:
                    return UFUAModel.DataType.UInt64;
                case BuiltInType.Float:
                    return UFUAModel.DataType.Float;
                case BuiltInType.Double:
                    return UFUAModel.DataType.Double;
                case BuiltInType.String:
                    return UFUAModel.DataType.String;
                default:
                    return UFUAModel.DataType.Boolean;
            }
        }

        public static BuiltInType GetBuiltTypeFromUFUAModelDataType(UFUAModel.DataType type)
        {
            switch (type)
            {
                case UFUAModel.DataType.SByte:
                    return BuiltInType.SByte;
                case UFUAModel.DataType.Byte:
                    return BuiltInType.Byte;
                case UFUAModel.DataType.Int16:
                    return BuiltInType.Int16;
                case UFUAModel.DataType.UInt16:
                    return BuiltInType.UInt16;
                case UFUAModel.DataType.Int32:
                    return BuiltInType.Int32;
                case UFUAModel.DataType.UInt32:
                    return BuiltInType.UInt32;
                case UFUAModel.DataType.Int64:
                    return BuiltInType.Int64;
                case UFUAModel.DataType.UInt64:
                    return BuiltInType.UInt64;
                case UFUAModel.DataType.Float:
                    return BuiltInType.Float;
                case UFUAModel.DataType.Double:
                    return BuiltInType.Double;
                case UFUAModel.DataType.String:
                    return BuiltInType.String;
                default:
                    return BuiltInType.Boolean;
            }
        }

        public static uint GetOpcUaDataTypeFromBuiltType(BuiltInType type)
        {
            switch (type)
            {
                case BuiltInType.SByte:
                    return Opc.Ua.DataTypes.SByte;
                case BuiltInType.Byte:
                    return Opc.Ua.DataTypes.Byte;
                case BuiltInType.Int16:
                    return Opc.Ua.DataTypes.Int16;
                case BuiltInType.UInt16:
                    return Opc.Ua.DataTypes.UInt16;
                case BuiltInType.Int32:
                    return Opc.Ua.DataTypes.Int32;
                case BuiltInType.UInt32:
                    return Opc.Ua.DataTypes.UInt32;
                case BuiltInType.Int64:
                    return Opc.Ua.DataTypes.Int64;
                case BuiltInType.UInt64:
                    return Opc.Ua.DataTypes.UInt64;
                case BuiltInType.Float:
                    return Opc.Ua.DataTypes.Float;
                case BuiltInType.Double:
                    return Opc.Ua.DataTypes.Double;
                case BuiltInType.String:
                    return Opc.Ua.DataTypes.String;
                default:
                    return Opc.Ua.DataTypes.Boolean;
            }
        }

        public static DriverErrorCodes MergeMinMajErrorCodeToMainErrorCode(DriverErrorCodes mainErrorCode, byte minError, byte majError)
        {
            byte[] errBytes = BitConverter.GetBytes((int)mainErrorCode);
            
            errBytes[2] = minError;
            errBytes[3] = majError;

            return (DriverErrorCodes)(BitConverter.ToInt32(errBytes, 0));
        }

        public static void GetMainMinMajErrorCodes(int sourceErrorCode, out int mainErrorCode, out byte minError, out byte majError)
        {
            if (sourceErrorCode == (int)DriverErrorCodes.ErrorNoError)
            {
                mainErrorCode = (int)DriverErrorCodes.ErrorNoError;
                minError = 0;
                majError = 0;
            }
            else
            {
                byte[] errBytes = BitConverter.GetBytes(sourceErrorCode);

                mainErrorCode = (int)(BitConverter.ToInt16(errBytes, 0));
                minError = errBytes[2];
                majError = errBytes[3];
            }
        }

        #endregion
    }
}