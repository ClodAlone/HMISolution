////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	GESRTP2Protocol.cs
//
// summary:	Implements the driver GESRTP2 protocol class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using Opc.Ua;
using DriverCodeBase.Enumerators;

namespace GESRTP2
{
    interface IPackable
    {
        byte[] Pack();
    }
    interface IExpandable
    {
        int Expand(byte[] DataArray, int Offset = 0);
    }

    /// <summary>   Error codes of GESRTP2. </summary>
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
    }

    /// <summary>   GESRTP2's target device memory areas. </summary>
    public enum AreaTypes : byte
    {
        /// <summary>   An enum constant representing the Register Words (%R) option. </summary>
        RegisterWords_R = 0x08,
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
        WRITE_ADDR_VAR = 0x6f
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
    public enum Capabiities : uint
    {
        SRTP_SRP_SERVER = 0x0001,
        SRTP_DESTINATION_SERVER = 0x0002,
        SRTP_PRIV_CONNECTION_SERVER = 0x0004,
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

        public int Expand(byte[] DataArray, int Offset = 0)
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
        byte[] _WriteData ;

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

        public int Expand(byte[] DataArray, int Offset = 0)
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
        private SmemSelect _SMSelect = new SmemSelect();
        private SmemSelectWrite _SMSelectWrite = new SmemSelectWrite();

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

        public byte Maj { set { _Maj = value; } }
        public byte Min { set { _Min = value; } }
        public byte[] Data {  get { return _Data; } }
        public PiggyBackedStatus PBStatus { get { return _PBStatus; } }
        public SmemSelect SMSelect { get { return _SMSelect; } }
        public SmemSelectWrite SMSelectWrite { get { return _SMSelectWrite; } }

        //Methods
        public void Init( byte seq, ReqTypes req, TrafficTypes typ, UInt16 WriteLength = 0)
        {
            Misc = 0;
            Seq = seq;
            Typ = typ;
            Source = 0;
            Dest = GESRTP2Protocol.DEFAULT_DESTINATION;
            Pkt = 1;
            Tot = 1;
            Req = req;
            Buflen = WriteLength;
            Bufptr = 0;
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

            if (_Req == ReqTypes.READ_SMEM)
                retVal.AddRange(_SMSelect.Pack());
            else if (_Req == ReqTypes.WRITE_SMEM)
                retVal.AddRange(_SMSelectWrite.Pack());

            if ( GESRTP2Protocol.Mailbox_SIZE > retVal.Count())
            {
                _Data = new byte[GESRTP2Protocol.Mailbox_SIZE - retVal.Count()];
                retVal.AddRange(_Data);
            }
            
            return retVal.ToArray();
        }

        public int Expand(byte[] DataArray, int Offset = 0)
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

            NewOffset = _PBStatus.Expand(DataArray, NewOffset);

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

        public int Expand(byte[] DataArray, int Offset = 0)
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
            NewOffset = Req.Expand(DataArray, NewOffset);
            NewOffset = Rsp.Expand(DataArray, NewOffset);
            NewOffset = _Mailbox.Expand(DataArray, NewOffset);

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

        public static EncapsPDU Session(SessionEnables SessionEnable, byte Seq, UInt16 InvokeId)
        {
            EncapsPDU Pdu = new EncapsPDU();
            Pdu.Init(PduTypes.SessionRequest, InvokeId);

            Pdu.Mailbox.Init(Seq, ReqTypes.SESSION_CONTROL, TrafficTypes.InitialRequest);
            Pdu.Mailbox.Data[0] = (byte)SessionEnable;
            return Pdu;
        }

        public static EncapsPDU Read(GESRTP2CommJob Job, GESRTP2Channel channel)
        {
            EncapsPDU Pdu = new EncapsPDU();
            Pdu.Init(PduTypes.DataRequest, channel.mInvokeId);
            Pdu.Mailbox.Init(channel.mSeq, ReqTypes.READ_SMEM, TrafficTypes.InitialRequest);
            Pdu.Mailbox.SMSelect.SegSelector = Job.AreaType;
            Pdu.Mailbox.SMSelect.SegOffset = (UInt16)(Job.StartAddress - 1);
            Pdu.Mailbox.SMSelect.Length =
                    //number of PLC memory units (not nr of bytes) 
                    (ushort)(Job.TotalJobSize / Job.GetProtocolDataByteSize());

            return Pdu;
        }

        public static EncapsPDU Write(GESRTP2CommJob Job, GESRTP2Channel channel)
        {
            ushort writeElements = 0;
            EncapsPDU Pdu = new EncapsPDU();
            System.Diagnostics.Trace.TraceInformation(string.Format("GESRTP2 DBG - Write  t {0} writeData {1}",
                                           DateTime.Now.ToString("HH:mm:ss.fff"), Job.writeData.Count));
            if (Job.writeData.Count() <= GESRTP2Protocol.PduWriteBufferSize ||
                (GESRTP2Protocol.DataType(Job.AreaType) == UFUAModel.DataType.Boolean && ( 
                (Job.StartAddress + Job.writeOffset -1) % 8 != 0 || Job.writeData.Count() <= GESRTP2Protocol.PduWriteBufferSize * 8)))
            {
                Pdu.Init(PduTypes.DataRequest, channel.mInvokeId);
                Pdu.Mailbox.Init(channel.mSeq, ReqTypes.WRITE_SMEM, TrafficTypes.InitialRequest);
                Pdu.Mailbox.SMSelectWrite.SegOffset = (UInt16)(Job.StartAddress + Job.writeOffset - 1);
                if (GESRTP2Protocol.DataType(Job.AreaType) != UFUAModel.DataType.Boolean)
                {
                    writeElements = (ushort)(Job.writeData.Count() / Job.elementSize);
                    Pdu.Mailbox.SMSelectWrite.WriteData = Job.writeData.ToArray();
                }
                else
                {
                    Pdu.Mailbox.SMSelectWrite.WriteData = PrepareSendData(Job.writeData.ToArray(), Pdu.Mailbox.SMSelectWrite.SegOffset, ref writeElements);
                    if(Pdu.Mailbox.SMSelectWrite.WriteData.Length > 1)
                    {
                        //ushort  tmpWriteElements = (ushort)( ((writeElements + 7) / 8) * 8 - (Job.StartAddress + Job.writeOffset - 1) % 8);
                        //writeElements = tmpWriteElements > writeElements ? writeElements : tmpWriteElements;
                        //byte[] tmp = new byte[(writeElements + 7) / 8];
                        //Array.Copy(Pdu.Mailbox.SMSelectWrite.WriteData, tmp, tmp.Length);                        
                        //Pdu.Mailbox.SMSelectWrite.WriteData = tmp;
                        Pdu.Mailbox.SMSelectWrite.WriteData = Pdu.Mailbox.SMSelectWrite.WriteData;
                    }
                }
                Pdu.Mailbox.SMSelectWrite.SegSelector = Job.AreaType;
                Pdu.Mailbox.SMSelectWrite.Length = writeElements;
                Job.writeData.RemoveRange(0, (int)(writeElements * Job.elementSize));
                Job.writeOffset += writeElements;
            }
            else
            {
                byte[] WriteData;
                if (GESRTP2Protocol.DataType(Job.AreaType) != UFUAModel.DataType.Boolean)
                {
                    WriteData = Job.writeData.GetRange(0, Job.writeData.Count()).ToArray();
                    writeElements = (ushort)(WriteData.Length / Job.elementSize);
                }
                else
                    WriteData = PrepareSendData(Job.writeData.ToArray(), Pdu.Mailbox.SMSelectWrite.SegOffset, ref writeElements);

                Pdu.Init(PduTypes.DataRequest, channel.mInvokeId, (uint)WriteData.Length);
                Pdu.EncapsData = WriteData;
                Pdu.Mailbox.Init(channel.mSeq, ReqTypes.WRITE_SMEM, TrafficTypes.InitialRequestTB, (UInt16)Pdu.DataLength);
                Pdu.Mailbox.SMSelectWrite.SegSelector = Job.AreaType;
                Pdu.Mailbox.SMSelectWrite.SegOffset = (UInt16)(Job.StartAddress + Job.writeOffset - 1);
                Pdu.Mailbox.SMSelectWrite.Length = writeElements;

                Job.writeOffset += writeElements;
                Job.writeData.RemoveRange(0, (int)(writeElements * Job.elementSize));
            }

            return Pdu;
        }

        static byte[] PrepareSendData(byte[] inData, ushort Seg_Offset, ref ushort writeElements)
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
        public const int MAX_DATA_BYTES = 0x4000;
        public const int DEFAULT_DESTINATION = 0x00000e10;
        public const int MAX_STRING_SIZE = 240;

        #endregion

        #region methods

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Return maximum memory size of GESRTP2CommJob objects. </summary>
        ///
        /// <returns>   The maximum job size. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static uint GetMaxJobSize()
        {
            return GESRTP2Protocol.MAX_DATA_BYTES;
        }
        public static UFUAModel.DataType DataType(AreaTypes AreaType)
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
                    return UFUAModel.DataType.Byte;
                case AreaTypes.AnalogInputWords_AI:
                case AreaTypes.AnalogOutputWords_AQ:
                case AreaTypes.RegisterWords_R:
                    return UFUAModel.DataType.UInt16;
                case AreaTypes.DiscreteBits_SA:
                case AreaTypes.DiscreteBits_SB:
                case AreaTypes.DiscreteBits_SC:
                case AreaTypes.DiscreteInputBits_I:
                case AreaTypes.DiscreteInternalBits_M:
                case AreaTypes.DiscreteOutputBits_Q:
                case AreaTypes.DiscreteTemporaryBits_T:
                case AreaTypes.GeniusGlobalDataBits_G:
                case AreaTypes.DiscreteBits_S_readOnly:
                    return UFUAModel.DataType.Boolean;
                default:
                    return UFUAModel.DataType.String;
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
            return(DataType(AreaType) == UFUAModel.DataType.Boolean && Type != UFUAModel.DataType.Boolean);
        }

        public static bool InvalidAreaDataType(AreaTypes AreaType, NodeId Type)
        {
            return (DataType(AreaType) == UFUAModel.DataType.Boolean && (uint)Type.Identifier != (uint)BuiltInType.Boolean);
        }

        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   Load in memory "buffer" the request frame for protocol's task "job". </summary>
        ///
        /// <param name="job">      . </param>
        /// <param name="buffer">   [in,out]. </param>
        ///
        /// <returns>   An uint. </returns>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public static byte[] PrepareRequest(GESRTP2Channel channel, GESRTP2CommJob job)
        {
            if (job.Station as GESRTP2Station == null)
                return new byte[0];
            if ((DataType(job.AreaType) == UFUAModel.DataType.Boolean ? (((job.StartAddress - 1) % 8) + job.TotalJobSize + 7) / 8 : job.TotalJobSize) > job.GetMaxJobSize())
                return new byte[0];

            if (job.ReadRequest())
            {
                job.onWrite = false;
                return EncapsPDUFactory.Read(job, channel).Pack();
            }
            else
            {
                lock (job.retLockList())
                {
                    if (job.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                    {
                        if (job.TagsListToWrite.Count == 0)
                            job.TagsListToWrite.AddRange(job.TagsList);
                    }
                    if (job.TagsListToWrite.Count == 0)
                        return new byte[0];

                    job.onWrite = true;

                    //prepare a write request
                    object objectData = null;
                    job.GetJobData(ref objectData);
                    if (job.TagsListOnWriting.Count == 0)
                        return new byte[0];

                    byte[] jobdata = (byte[])objectData;

                    job.writeData.Clear();
                    job.writeData.AddRange(jobdata);
                    job.writeOffset = (ushort)(job.TagsListOnWriting[0].ByteOffset / job.GetProtocolDataByteSize());
                }

                return EncapsPDUFactory.Write(job, channel).Pack();
              
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
            items.AddRange(changed);

            return true;
        }
        #endregion

    }
}
