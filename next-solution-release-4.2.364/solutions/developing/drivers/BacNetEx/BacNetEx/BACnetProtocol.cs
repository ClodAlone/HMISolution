////////////////////////////////////////////////////////////////////////////////////////////////////
// file:	BACnetProtocol.cs
//
// summary:	Implements the driver BACnet protocol class
////////////////////////////////////////////////////////////////////////////////////////////////////

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using Opc.Ua;
using UFUAModel.Extensions;

namespace BACnet
{
    /// <summary>   Error codes of BACnet. </summary>
    public enum BACnetErrorCodes : int
    {
        ErrorTxWrite = 1000,
        ErrorUnexpectedInvokeId,
        ErrorProtocolError,
        ErrorUnknownProperty,
        ErrorWriteAccessDenied,
        ErrorValueOutOfRange,
        ErrorInvalidDataType,
        ErrorCovSubscriptionFailed,
        ErrorBACnetTimeOut,
        ErrorWhoHasFailed,
        ErrorWhoHisFailed,
        ErrorRegisterToBBMD,
        ErrorAllStationsOnError,
        ErrorInvalidStation,
        ErrorUnreachableStation,
        ErrorUnknownObject,
    }

    public enum TimeSyncs
    { None,System_Time, UTC_Time  }

    public enum DataLogModes
    { None, On_new_data, On_data_change }
    public enum PriorityLevels
    {
        _1_Manual_Life_Safety = 1,
        _2_Automatic_Life_Safety,
        _3_Available,
        _4_Available,
        _5_Critical_Equipment_Control,
        _6_Minimum_On_Off,
        _7_Available,
        _8_Manual_Operator,
        _9_Available,
        _10_Available,
        _11_Available,
        _12_Available,
        _13_Available,
        _14_Available,
        _15_Available,
        _16_Available,
    }

    interface IPackable
    {
        byte[] Pack();
    }
    interface IExpandable
    {
        void Expand(ref byte[] DataArray, ref int Offset);
    }

    public class ReceiveItem : ICloneable
    {
        public BACnetIPFrame Frame;
        public IPEndPoint RemoteEndPoint;
        public DateTime TimeStamp;
        public bool isValid;
        public bool isUnMappedDevice; //source device in not a station of channel --> invalid message      

        public ReceiveItem(BACnetIPFrame inFrame, IPEndPoint inRemoteEndPoint, DateTime inTimeStamp)
        {
            Frame = inFrame;
            RemoteEndPoint = inRemoteEndPoint;
            TimeStamp = inTimeStamp;
            isValid = false;
            isUnMappedDevice = false;
        }

        public bool ComingFromEndPoint(IPEndPoint ep)
        {
            if (RemoteEndPoint != null && ep!= null && RemoteEndPoint.Address.ToString() == ep.Address.ToString())
                return true;
            else
                return false;
        }

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
    public static class BufferExpand
    {
        public static byte toByte(ref byte[] DataArray, ref int Offset)
        {
            checkSize(DataArray,Offset,1);
            byte outval = DataArray[Offset];
            Offset += 1;
            return outval;
        }
        public static UInt16 toUInt16(ref byte[] DataArray, ref int Offset)
        {
            checkSize(DataArray, Offset,2);
            UInt16 outval = BitConverter.ToUInt16(DataArray, Offset);
            Offset += 2;
            return Swap(outval);
        }
        
        public static UInt32 toUInt32(byte[] DataArray)
        {
            int Offset = 0;
            return toUInt32(ref DataArray, ref Offset);

        }

        public static UInt32 toUInt32(ref byte[] DataArray, ref int Offset)
        {
            checkSize(DataArray, Offset, 4);
            UInt32 outval = 0;
            for (UInt16 i = 0; i < 4; i++)
                outval = (outval << 8) + DataArray[i];
            return outval;
        }
        public static float toFloat(byte[] DataArray)
        {
            checkSize(DataArray, 0, 4);
            float outval = BitConverter.ToSingle(swapArray(DataArray), 0);
            return outval;
        }
        public static double toDouble(byte[] DataArray)
        {
            checkSize(DataArray, 0, 8);
            double outval = BitConverter.ToDouble(swapArray(DataArray), 0);
            return outval;
        }
        public static string toString(byte[] DataArray)
        {
            return Encoding.UTF8.GetString(DataArray, 1, DataArray.Length-1); ;
        }
        public static Date toDate(byte[] DataArray)
        {
            checkSize(DataArray, 0, 4);
            Date outval = new Date(DataArray);
            return outval;
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
        public static byte[] toBitString(byte[] DataArray)
        {
            byte[] outBuffer = new byte[2];
            byte usedBit = 8;
            while (usedBit > 0 && (DataArray[0] & (1 << (usedBit - 1))) == 0)
                usedBit--;
            outBuffer[0] = (byte)(8 - usedBit);
            outBuffer[1] = (byte)(DataArray[0] << outBuffer[0]);
            return outBuffer;
        }
        public static byte[] fromBitString(byte[] DataArray)
        {
            checkSize(DataArray, 0, 2);
            byte[] rt = new byte[1];
            rt [0]=(byte)(DataArray[1] >> DataArray[0]);
            return(rt);
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
            }while (localValue != 0);

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
    /// <summary>
    /// BACnetIPFrame
    /// </summary>
    public class BACnetIPFrameHeader : IPackable, IExpandable
    {
        public static UInt16 Size = 4;
        public BACnetIPFrameHeader()
        {
            Init();
        }
        public BACnetIPFrameHeader(bool bBMD)
        {
            Init(bBMD);
        }
        private byte _BVLCtype;
        private BACnetEnums.BVLC_Functions _BVLCfunction;
        private BACnetEnums.BVLC_Result _BVLCresult;
        private UInt16 _FrameSize;
        private IPEndPoint _OriginalEndPoint;
        private bool _IsBBMDMessage;

        //Properties
        public byte BVLCtype { get { return _BVLCtype; } set { _BVLCtype = value; } }
        public BACnetEnums.BVLC_Functions BVLCfunction { get { return _BVLCfunction; } set { _BVLCfunction = value; } }
        public BACnetEnums.BVLC_Result BVLCresult { get { return _BVLCresult; } set { _BVLCresult = value; } }
        public UInt16 FrameSize { get { return _FrameSize; } set { _FrameSize = value; } }
        public IPEndPoint OriginalEndPoint { get { return _OriginalEndPoint; }set { _OriginalEndPoint = value; } }
        public bool IsBBMDMessage { get { return _IsBBMDMessage; } set { _IsBBMDMessage = value; } }
        //Methods
        public void Init()
        {
            _BVLCtype = BACnetEnums.BVLC_TYPE_BIP;
            _BVLCfunction = BACnetEnums.BVLC_Functions.UnicastNPDU;
            _BVLCresult = BACnetEnums.BVLC_Result.Successful;
            _FrameSize = 0;
            _OriginalEndPoint = null;
            _IsBBMDMessage = false;
        }
        public void Init(bool bBMD)
        {
            _BVLCtype = BACnetEnums.BVLC_TYPE_BIP;
            if (bBMD)
                _BVLCfunction = BACnetEnums.BVLC_Functions.DistBcastToNet;
            else
                _BVLCfunction = BACnetEnums.BVLC_Functions.UnicastNPDU;
            _BVLCresult = BACnetEnums.BVLC_Result.Successful;
            _FrameSize = 0;
            _OriginalEndPoint = null;
            _IsBBMDMessage = false;
        }
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_BVLCtype);
            retVal.Add((byte)_BVLCfunction);
            retVal.AddRange(BufferExpand.toArray((UInt16)(_FrameSize + Size)));
            return retVal.ToArray();
        }
        public void Expand(ref byte[] DataArray, ref int Offset)
        {
            Init();
            _BVLCtype = BufferExpand.toByte(ref DataArray, ref Offset);
            _BVLCfunction = (BACnetEnums.BVLC_Functions)BufferExpand.toByte(ref DataArray, ref Offset);
            _FrameSize = (UInt16)(BufferExpand.toUInt16(ref DataArray, ref Offset) - Size);
            switch (_BVLCfunction) {
                case BACnetEnums.BVLC_Functions.BVLC_Result:
                    _BVLCresult = (BACnetEnums.BVLC_Result)BufferExpand.toUInt16(ref DataArray, ref Offset);
                    break;

                case BACnetEnums.BVLC_Functions.ForwardedNPDU:
                    long ip = ((long)DataArray[Offset + 3] << 24) + ((long)DataArray[Offset + 2] << 16) + ((long)DataArray[Offset + 1] << 8) + (long)DataArray[Offset];
                    int port = (DataArray[Offset + 4] << 8) + DataArray[Offset + 5];    // 0xbac0 maybe
                    _OriginalEndPoint = new IPEndPoint(ip, port);
                    Offset += 6;
                    break;
            }
        }        
    }
    /// <summary>
    /// BACnetIPFrame
    /// </summary>
    public class BACnetIPFrame : IPackable, IExpandable
    {
        public BACnetIPFrame()
        {
            Init();
            _Npdu = new NPDU();
        }

        public BACnetIPFrame(bool bBMD)
        {
            Init(bBMD);
            _Npdu = new NPDU();
        }
        public BACnetIPFrame(BACnetEnums.UnconfirmedService UNCONFIRMED_SERVICE, bool bBMD)
        {
            Init(bBMD);
            _Npdu = new NPDU(UNCONFIRMED_SERVICE);
        }
        public BACnetIPFrame(BACnetEnums.ConfirmedService CONFIRMED_SERVICE)
        {
            Init();
            _Npdu = new NPDU(CONFIRMED_SERVICE);
        }
        public BACnetIPFrame(BACnetEnums.BACnet_BVLC_FUNCTION BVLC_FUNCTION)
        {
            Init(true);
            switch (BVLC_FUNCTION) {
                case BACnetEnums.BACnet_BVLC_FUNCTION.BVLC_REGISTER_FOREIGN_DEVICE:
                    Header.BVLCfunction = BACnetEnums.BVLC_Functions.RegisterDevice;
                    break;
                case BACnetEnums.BACnet_BVLC_FUNCTION.BVLC_READ_FOREIGN_DEVICE_TABLE:
                    Header.BVLCfunction = BACnetEnums.BVLC_Functions.ReadDeviceTable;
                    break;
                case BACnetEnums.BACnet_BVLC_FUNCTION.BVLC_READ_BROADCAST_DIST_TABLE:
                    Header.BVLCfunction = BACnetEnums.BVLC_Functions.ReBcastDistTable;
                    break;
            }
            
            _Npdu = new NPDU(BVLC_FUNCTION);
        }
        //Fields
        private BACnetIPFrameHeader _Header;
        private UInt16 _BBMDLifetime;
        private List<byte> _BVLLmessage;
        private NPDU _Npdu;

        //Properties
        public BACnetIPFrameHeader Header { get { return _Header; } set { _Header = value; } }
        public UInt16 BBMDLifetime { get { return _BBMDLifetime; } set { _BBMDLifetime = value; } }
        public List<byte> BVLLmessage { get { return _BVLLmessage; } set { _BVLLmessage = value; } }
        public NPDU Npdu { get { return _Npdu; } set { _Npdu = value; } }


        //Methods
        public void Init()
        {
            _Header = new BACnetIPFrameHeader();
            _BBMDLifetime = 60;
            _BVLLmessage = new List<byte>();

        }
        public void Init(bool bBMD)
        {
            _Header = new BACnetIPFrameHeader(bBMD);
            _BBMDLifetime = 60;
            _BVLLmessage = new List<byte>();

        }

        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            switch (_Header.BVLCfunction)
            {
                case BACnetEnums.BVLC_Functions.WrBcastDistTable:
                    _BVLLmessage.Add(0x53);
                    _BVLLmessage.Add(0x67);
                    _BVLLmessage.Add(0x1f);
                    _BVLLmessage.Add(0x1a);
                    _BVLLmessage.Add(0xba);
                    _BVLLmessage.Add(0xc0);
                    _BVLLmessage.Add(0xff);
                    _BVLLmessage.Add(0xff);
                    _BVLLmessage.Add(0xff);
                    _BVLLmessage.Add(0xff);
                    _Header.FrameSize = (UInt16)_BVLLmessage.Count();
                    retVal.AddRange(_Header.Pack());
                    retVal.AddRange(_BVLLmessage);
                    break;
                case BACnetEnums.BVLC_Functions.ReadDeviceTable:
                case BACnetEnums.BVLC_Functions.ReBcastDistTable:
                    _Header.FrameSize = 0;
                    retVal.AddRange(_Header.Pack());
                    break;
                
                case BACnetEnums.BVLC_Functions.RegisterDevice:
                    _Header.FrameSize = 2;
                    retVal.AddRange(_Header.Pack());
                    //retVal.AddRange(BufferExpand.toArray((UInt16)((_BBMDLifetime > 1092) ? 1092 : (_BBMDLifetime))));
                    retVal.AddRange(BufferExpand.toArray(_BBMDLifetime));
                    break;
                case BACnetEnums.BVLC_Functions.UnicastNPDU:
                default:
                    byte[] pduBuffer = _Npdu.Pack();
                    _Header.FrameSize = (UInt16)pduBuffer.Length;
                    retVal.AddRange(_Header.Pack());
                    retVal.AddRange(pduBuffer);
                    break;
            }
            return retVal.ToArray();
        }

        public void Expand(ref byte[] DataArray, ref int Offset)
        {
            Init();
            _Npdu = new NPDU();
            _Header.Expand(ref DataArray, ref Offset);
            switch (_Header.BVLCfunction)
            {
                case BACnetEnums.BVLC_Functions.WrBcastDistTable:
                case BACnetEnums.BVLC_Functions.ReadDeviceTable:
                case BACnetEnums.BVLC_Functions.ReBcastDistTable:
                    _BVLLmessage = new List<byte>(BufferExpand.toArray(ref DataArray, ref Offset, _Header.FrameSize));
                    _Header.IsBBMDMessage = true;
                    break;
                case BACnetEnums.BVLC_Functions.RegisterDevice:
                    _BBMDLifetime = BufferExpand.toUInt16(ref DataArray, ref Offset);
                    _Header.IsBBMDMessage = true;
                    break;
                case BACnetEnums.BVLC_Functions.ReadDeviceTableACK:
                case BACnetEnums.BVLC_Functions.ReBcastDistTableACK:
                case BACnetEnums.BVLC_Functions.BVLC_Result:
                    _Header.IsBBMDMessage = true;
                    break;
                case BACnetEnums.BVLC_Functions.ForwardedNPDU:
                case BACnetEnums.BVLC_Functions.BcastNPDU:                    
                case BACnetEnums.BVLC_Functions.UnicastNPDU:
                case BACnetEnums.BVLC_Functions.DistBcastToNet:
                default:
                    _Npdu.Expand(ref DataArray, ref Offset);
                    break;
            }
        }

    }
    /// <summary>
    /// NPDU
    /// </summary>
    public class NPDU : IPackable, IExpandable
    {
        public NPDU(BACnetEnums.ConfirmedService ConfirmedServiceChoice,bool DestinationSpecifierPresent = false)
        {
            Init();
            DataExpectingReply = true;
            Apdu.ConfirmedServiceChoice = ConfirmedServiceChoice;
        }
        public NPDU(BACnetEnums.UnconfirmedService UnconfirmedServiceChoice, bool DestinationSpecifierPresent = false)
        {
            Init();
            DataExpectingReply = false;
            Apdu.UnconfirmedServiceChoice = UnconfirmedServiceChoice;
        }
        public NPDU(BACnetEnums.BACnet_BVLC_FUNCTION BvlcFunctionChoice, bool DestinationSpecifierPresent = false)
        {
            Init();
            DataExpectingReply = false;
            //Apdu.UnconfirmedServiceChoice = BvlcFunctionChoice;
        }
        public NPDU(bool DestinationSpecifierPresent = false)
        {
            Init();
            DataExpectingReply = false;
            Apdu.UnconfirmedServiceChoice = BACnetEnums.UnconfirmedService.WHO_IS;
        }
        //Fields
        private byte _Version;
        private byte _Control;
        private APDU _Apdu;
        private UInt16 _DNET;
        private byte[] _DADR;
        private UInt16 _SNET;
        private byte[] _SADR;
        private byte _HopCount;
        private byte _MessageType;
        private UInt16 _VendorID;

        //Properties
        public byte Version { get { return _Version; } set { _Version = value; } }
        public BACnetEnums.NetworkPriority NetworkPriority { set { _Control = (byte)((((byte)value) & 0x03) + (_Control & (0x03 ^ 0xFF))); } }
        public bool DataExpectingReply
        {
            get 
            {
                return (_Control & (1 << 2)) == (1 << 2); 
            } 
            set 
            { 
                _Control = (byte)((Convert.ToByte(value) << 2) + (_Control & ((1 << 2) ^ 0xFF))); 
            } 
        }
        public bool SourceSpecifierPresent
        {
            get 
            {
                return (_Control & (1 << 3)) == (1 << 3); 
            } 
            set 
            { 
                _Control = (byte)((Convert.ToByte(value) << 3) + (_Control & ((1 << 3) ^ 0xFF))); 
            } 
        }
        public bool DestinationSpecifierPresent 
        {
            get 
            {
                return (_Control & (1 << 5)) == (1 << 5); 
            } 
            set 
            { 
                _Control = (byte)((Convert.ToByte(value) << 5) + (_Control & ((1 << 5) ^ 0xFF))); 
            } 
        }
        public bool MessageTypePresent
        {
            get
            {
                return (_Control & (1 << 7)) == (1 << 7);
            }
            set
            {
                _Control = (byte)((Convert.ToByte(value) << 7) + (_Control & ((1 << 7) ^ 0xFF)));
            }
        }
        public UInt16 DNET { get { return _DNET; } set { _DNET = value; } }
        public byte DLEN { get { return (byte)_DADR.Length; } set { _DADR = new byte[value]; } }
        public byte[] DADR { get { return _DADR; } set { _DADR = value; } }
        public UInt32 DAddress 
        { 
            get 
            {
                return BufferExpand.getUInt32(DADR);  
            }
            set { _DADR = BufferExpand.getArray(value); } 
        }
        public UInt16 SNET { get { return _SNET; } set { _SNET = value; } }
        public byte SLEN { get { return (byte)_SADR.Length; } set { _SADR = new byte[value]; } }
        public byte[] SADR { get { return _SADR; } set { _SADR = value; } }
        public UInt32 SAddress
        {
            get
            {
                return BufferExpand.getUInt32(SADR);
            }
            set { _SADR = BufferExpand.getArray(value); }
        }
        public byte MessageType { get { return _MessageType; } set { _MessageType = value; } }
        public UInt16 VendorID { get { return _VendorID; } set { _VendorID = value; } }
        public APDU Apdu { get { return _Apdu; } }

        //Methods
        public void Init(bool inDestinationSpecifierPresent = false)
        {
            _Version = BACnetEnums.PROTOCOL_VERSION;
            _Control = 0;
            SourceSpecifierPresent = false;
            DestinationSpecifierPresent = inDestinationSpecifierPresent;
            NetworkPriority = BACnetEnums.NetworkPriority.NORMAL;
            MessageTypePresent = false;
            _HopCount = 0xff;
            _Apdu = new APDU();

        }

        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            bool AddApduData = true;

            retVal.Add(_Version);
            retVal.Add(_Control);
            if (DestinationSpecifierPresent)
            {
                retVal.AddRange(BufferExpand.toArray((UInt16)(_DNET)));
                retVal.Add((byte)_DADR.Length);
                if ((byte)_DADR.Length != 0)
                    retVal.AddRange(_DADR);
            }
            if (SourceSpecifierPresent)
            {
                retVal.AddRange(BufferExpand.toArray((UInt16)(_SNET)));
                retVal.Add((byte)_SADR.Length);
                if ((byte)_SADR.Length != 0)
                    retVal.AddRange(_SADR);
            }

            if (DestinationSpecifierPresent)
                retVal.Add(_HopCount);

            if (MessageTypePresent)
            {
                retVal.Add(_MessageType);
                retVal.AddRange(BufferExpand.toArray((UInt16)(_VendorID)));
                if (_MessageType == (byte)BACnetEnums.BACnet_NETWORK_MESSAGE_TYPE.NETWORK_MESSAGE_I_AM_ROUTER_TO_NETWORK)
                    AddApduData = false;                
            }

            if (AddApduData)
                retVal.AddRange(_Apdu.Pack());

            return retVal.ToArray();
        }

        public void Expand(ref byte[] DataArray, ref int Offset)
        {
            Init();
            _Version = BufferExpand.toByte(ref DataArray, ref Offset);
            _Control = BufferExpand.toByte(ref DataArray, ref Offset);
            if (DestinationSpecifierPresent)
            {
                _DNET = BufferExpand.toUInt16(ref DataArray, ref Offset);
                DLEN = BufferExpand.toByte(ref DataArray, ref Offset);
                if (DLEN != 0)
                    _DADR = BufferExpand.toArray(ref DataArray, ref Offset, DLEN);
            }            
            if (SourceSpecifierPresent)
            {
                _SNET = BufferExpand.toUInt16(ref DataArray, ref Offset);
                SLEN = BufferExpand.toByte(ref DataArray, ref Offset);
                if (SLEN != 0)
                    _SADR = BufferExpand.toArray(ref DataArray, ref Offset, SLEN);
            }
            if (DestinationSpecifierPresent)
                _HopCount = BufferExpand.toByte(ref DataArray, ref Offset);
            
            if (MessageTypePresent)
            {
                _MessageType = BufferExpand.toByte(ref DataArray, ref Offset);
                _VendorID = BufferExpand.toUInt16(ref DataArray, ref Offset);
            }

            //BcastNPDU
            if (DataArray.Length>Offset)
                _Apdu.Expand(ref DataArray, ref Offset);
        }

    }
    /// <summary>
    /// APDU
    /// </summary>
    public class APDU : IPackable, IExpandable
    {
        //ASHRAEE STANDARD 20.1.2.11
        public APDU()
        {
            Init();
        }
       //Fields
        private byte _bytePduType = 0;
        private byte _byteSeg = 0;
        private byte _InvokeId = 0;
        private byte _SequenceNumber = 0;
        private byte _WindowSize = 0;
        private byte _ServiceChoice = 0;
        private byte _Reason = 0;
        private ServiceTagList _ServiceRequest;

        //Properties
        public BACnetEnums.BACnetPDU PduType 
        { 
            set { _bytePduType = (byte)((((byte)value) & 0x70) + (_bytePduType & (0x70 ^ 0xFF))); }
            get { return (BACnetEnums.BACnetPDU)(_bytePduType & 0x70); } 
        }
        public bool SEG { 
            set { _bytePduType = (byte)((Convert.ToByte(value) << 3) + (_bytePduType & ((1 << 3) ^ 0xFF))); }
            get { return (Convert.ToBoolean(_bytePduType & (1 << 3))); }
        }
        public bool MOR { set { _bytePduType = (byte)((Convert.ToByte(value) << 2) + (_bytePduType & ((1 << 2) ^ 0xFF))); } }
        public bool SA { set { _bytePduType = (byte)((Convert.ToByte(value) << 1) + (_bytePduType & ((1 << 1) ^ 0xFF))); } }
        public bool NAK { set { _bytePduType = (byte)((Convert.ToByte(value) << 1) + (_bytePduType & ((1 << 1) ^ 0xFF))); } }
        public bool SRV { set { _bytePduType = (byte)(Convert.ToByte(value) + (_bytePduType & (1 ^ 0xFF))); } }

        public byte MaxSegs
        {
            set { _byteSeg = (byte)(((value & 0x07) << 4) + (_byteSeg & (0x70 ^ 0xFF))); }
            get { return (byte)((_bytePduType & 0x70) >> 4); }
        }
        public byte MaxResp
        {
            set { _byteSeg = (byte)((value & 0x0F) + (_byteSeg & (0x0F ^ 0xFF))); }
            get { return (byte)(_bytePduType & 0x0F); }
        }
        public byte InvokeId { get { return _InvokeId; } set { _InvokeId = value; } }
        public byte SequenceNumber { get { return _SequenceNumber; } set { _SequenceNumber = value; } }
        public byte WindowSize { get { return _WindowSize; } set { _WindowSize = value; } }
        public byte RejectReason { get { return _Reason; } set { _Reason = value; } }
        public byte AbortReason { get { return _Reason; } set { _Reason = value; } }
        public BACnetEnums.ConfirmedService ConfirmedServiceChoice 
        { 
            get { return (BACnetEnums.ConfirmedService)_ServiceChoice; } 
            set { 
                _ServiceChoice = (byte)value;
                PduType = BACnetEnums.BACnetPDU.CONFIRMED_REQUEST;
            } 
        }
        public BACnetEnums.UnconfirmedService UnconfirmedServiceChoice 
        { 
            get { return (BACnetEnums.UnconfirmedService)_ServiceChoice; }
            set
            {
                _ServiceChoice = (byte)value;
                PduType = BACnetEnums.BACnetPDU.UNCONFIRMED_REQUEST;
            }
        }
        public BACnetEnums.BACnet_BVLC_FUNCTION BvlcFunctionServiceChoice
        {
            get { return (BACnetEnums.BACnet_BVLC_FUNCTION)_ServiceChoice; }
            set
            {
                _ServiceChoice = (byte)value;
                PduType = BACnetEnums.BACnetPDU.CONFIRMED_REQUEST;
            }
        }


        public ServiceTagList ServiceRequest { get { return _ServiceRequest; } }


        //Methods
        public void Init()
        {

            _bytePduType = 0;
            PduType = BACnetEnums.BACnetPDU.UNCONFIRMED_REQUEST;
            SEG = false;
            MOR = false;
            SA = false;
            NAK = false;
            SRV = false;

            _byteSeg = 0;
            MaxSegs = 0;
            MaxResp = 5;

            _InvokeId = 0;
            _SequenceNumber = 0;
            _WindowSize = 0;
            _ServiceChoice = 0;
            _Reason = 0;
            _ServiceRequest = new ServiceTagList();

        }

        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_bytePduType);
            switch (PduType)
            {
                case BACnetEnums.BACnetPDU.CONFIRMED_REQUEST:
                    retVal.Add(_byteSeg);
                    retVal.Add(_InvokeId);
                    if(SEG)
                    {
                        retVal.Add(_SequenceNumber);
                        retVal.Add(_WindowSize);
                    }
                    retVal.Add(_ServiceChoice);
                    retVal.AddRange(_ServiceRequest.Pack());
                    break;
                case BACnetEnums.BACnetPDU.UNCONFIRMED_REQUEST:
                    retVal.Add(_ServiceChoice);
                    retVal.AddRange(_ServiceRequest.Pack());
                    break;
                case BACnetEnums.BACnetPDU.SIMPLE_ACK:
                    retVal.Add(_InvokeId);
                    retVal.Add(_ServiceChoice);
                    break;
                case BACnetEnums.BACnetPDU.COMPLEX_ACK:
                    retVal.Add(_InvokeId);
                    retVal.Add(_ServiceChoice);
                    retVal.AddRange(_ServiceRequest.Pack());
                    break;
                case BACnetEnums.BACnetPDU.SEGMENT_ACK:
                    retVal.Add(_InvokeId);
                    retVal.Add(_SequenceNumber);
                    retVal.Add(_WindowSize);
                    retVal.Add(_ServiceChoice);
                    retVal.AddRange(_ServiceRequest.Pack());
                    break;
                case BACnetEnums.BACnetPDU.ERROR:
                    retVal.Add(_InvokeId);
                    retVal.Add(_ServiceChoice);
                    break;
                case BACnetEnums.BACnetPDU.REJECT:
                case BACnetEnums.BACnetPDU.ABORT:
                    retVal.Add(_InvokeId);
                    retVal.Add(_Reason);
                    break;
            }


            return retVal.ToArray();
        }

        public void Expand(ref byte[] DataArray, ref int Offset)
        {
            Init();
            _bytePduType = BufferExpand.toByte(ref DataArray, ref Offset);
            switch (PduType)
            {
                case BACnetEnums.BACnetPDU.CONFIRMED_REQUEST:
                    _byteSeg = BufferExpand.toByte(ref DataArray, ref Offset);
                    _InvokeId = BufferExpand.toByte(ref DataArray, ref Offset);
                    if (SEG)
                    {
                        _SequenceNumber = BufferExpand.toByte(ref DataArray, ref Offset);
                        _WindowSize = BufferExpand.toByte(ref DataArray, ref Offset);
                    }
                    _ServiceChoice = BufferExpand.toByte(ref DataArray, ref Offset);
                    _ServiceRequest.Expand(ref DataArray, ref Offset);
                    break;
                case BACnetEnums.BACnetPDU.UNCONFIRMED_REQUEST:
                    _ServiceChoice = BufferExpand.toByte(ref DataArray, ref Offset);
                    _ServiceRequest.Expand(ref DataArray, ref Offset);
                    break;
                case BACnetEnums.BACnetPDU.SIMPLE_ACK:
                    _InvokeId = BufferExpand.toByte(ref DataArray, ref Offset);
                    _ServiceChoice = BufferExpand.toByte(ref DataArray, ref Offset);
                    break;
                case BACnetEnums.BACnetPDU.COMPLEX_ACK:
                    _InvokeId = BufferExpand.toByte(ref DataArray, ref Offset);
                    _ServiceChoice = BufferExpand.toByte(ref DataArray, ref Offset);
                    _ServiceRequest.Expand(ref DataArray, ref Offset);
                    break;
                case BACnetEnums.BACnetPDU.SEGMENT_ACK:
                    _InvokeId = BufferExpand.toByte(ref DataArray, ref Offset);
                    _SequenceNumber = BufferExpand.toByte(ref DataArray, ref Offset);
                    _WindowSize = BufferExpand.toByte(ref DataArray, ref Offset);
                    _ServiceChoice = BufferExpand.toByte(ref DataArray, ref Offset);
                    _ServiceRequest.Expand(ref DataArray, ref Offset);
                    break;
                case BACnetEnums.BACnetPDU.ERROR:
                    _InvokeId = BufferExpand.toByte(ref DataArray, ref Offset);
                    _ServiceChoice = BufferExpand.toByte(ref DataArray, ref Offset);
                    _ServiceRequest.Expand(ref DataArray, ref Offset);
                    break;
                case BACnetEnums.BACnetPDU.REJECT:
                case BACnetEnums.BACnetPDU.ABORT:
                    _InvokeId = BufferExpand.toByte(ref DataArray, ref Offset);
                    _Reason = BufferExpand.toByte(ref DataArray, ref Offset);
                    break;
            }

            int NewOffset = Offset;

        }

    }
    
    /// <summary>
    /// Service Request
    /// </summary>
    public class ServiceTagList : IPackable, IExpandable
    {

        public ServiceTagList()
        {
            _context = -1;
            _List = new List<ServiceTag>();
        }

        private List<ServiceTag> _List;
        public List<ServiceTag> List
        {
            get { return _List; }
            set { _List = value; }
        }

        private sbyte _context;
        public sbyte context
        {
            get { return _context; }
            set { _context = value; }
        }

        public void Add(ServiceTag Tag)
        {
            _List.Add(Tag);
        }

        public void AddRange(ServiceTagList TagList)
        {
            _List.AddRange(TagList.List);
        }
        
        public int Count()
        {
            return _List.Count();
        }

        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            foreach (ServiceTag Tag in _List)
                retVal.AddRange(Tag.Pack());
            return retVal.ToArray();
        }

        public void Expand(ref byte[] DataArray, ref int Offset)
        {
            while(DataArray.Length > Offset)
            {
                ServiceTag Tag = new ServiceTag();
                Tag.Expand(ref DataArray, ref Offset);
                if (!Tag.isClosing)
                    _List.Add(Tag);
                else if (_context == Tag.ContextTag)
                    break;
            }
        }
    }

    public class BACnetObjectIdentifier : ICloneable
    {
        private BACnetEnums.ObjectTypes _ObjectType;
        private UInt32 _instance;
        public BACnetObjectIdentifier(BACnetEnums.ObjectTypes objectType, UInt32 instance)
        {
            _instance = instance & BACnetEnums.MAX_INSTANCE;
            _ObjectType = objectType;
        }
        public BACnetObjectIdentifier(UInt32 value)
        {
            _instance = value & BACnetEnums.MAX_INSTANCE;
            _ObjectType = (BACnetEnums.ObjectTypes)(value >> BACnetEnums.INSTANCE_BITS);
        }
        public BACnetObjectIdentifier()
        {
            _instance = 0;
            _ObjectType = BACnetEnums.ObjectTypes.OBJECT_NULL;
        }
        public UInt32 valueUint
        {
            get { return _instance + ((UInt32)_ObjectType << BACnetEnums.INSTANCE_BITS); }
            set 
            {
                _instance = value & BACnetEnums.MAX_INSTANCE;
                _ObjectType = (BACnetEnums.ObjectTypes)(value >> BACnetEnums.INSTANCE_BITS);
            }
        }        
        public byte[] valueArray
        {
            get
            {
                if (_ObjectType == BACnetEnums.ObjectTypes.OBJECT_NULL)
                    return null;
                return BufferExpand.toArray(valueUint);
            }
            set
            {
                if(value == null)
                {
                    _instance = 0;
                    _ObjectType = BACnetEnums.ObjectTypes.OBJECT_NULL;
                }
                else
                {
                    valueUint = BufferExpand.toUInt32(valueArray);
                }
            }
        }
        public BACnetEnums.ObjectTypes ObjectType
        {
            get { return _ObjectType; }
        }
        public UInt32 instance
        {
            get { return _instance; }
        }
        
        public object Clone()
        {
            return MemberwiseClone();
        }
    }

    public struct Enumerated
    {
        private byte[] _value;
        public Enumerated(byte[] DataArray)
        {
            _value = DataArray;
        }

        public byte[] DataArray
        {
            get {return _value; }
        }
        public uint Uint
        {
            get { return BufferExpand.getUInt32(_value); }
        }
    }

    public struct BitString
    {
        private byte[] _value;
        public BitString(byte[] DataArray)
        {
            if(DataArray == null)
            {
                _value = null;
                return;
            }
            BufferExpand.checkSize(DataArray, 0, 2);
            _value = new byte[2];
            Array.Copy(DataArray, _value, 2);
        }

        public byte[] value
        {
            get 
            {
                if (_value == null)
                {
                    return  null;
                }
                return BufferExpand.fromBitString(_value);
            }
        }

    }

    public struct Date
    {
        private byte[] _value;
        public Date(DateTime date)
        {        
            _value = new byte[4];
            _value[0] = (byte)(date.Year - 1900);
            _value[1] = (byte)(date.Month);
            _value[2] = (byte)(date.Day);
            _value[3] = (byte)(date.DayOfWeek);
        }
        public Date(byte[] DataArray)
        {
            if(DataArray == null)
            {
                _value = null;
            }
            else
            {
                BufferExpand.checkSize(DataArray, 0, 4);
                _value = new byte[4];
                Array.Copy(DataArray, _value, 4);
            }
        }

        public byte[] value
        {
            get { return _value; }
        }
    }

    public struct Time
    {
        private byte[] _value;
        public Time(DateTime time)
        {
            _value = new byte[4];
            _value[0] = (byte)time.Hour;
            _value[1] = (byte)time.Minute;
            _value[2] = (byte)time.Second;
            _value[3] = (byte)(time.Millisecond / 10);
        }
        public Time(byte[] DataArray)
        {
            if (DataArray == null)
            {
                _value = null;
            }
            else
            {
                BufferExpand.checkSize(DataArray, 0, 4);
                _value = new byte[4];
                Array.Copy(DataArray, _value, 4);
            }
        }

        public byte[] value
        {
            get { return _value; }
        }
    }

    public class IAmAnswer
    {
        public BACnetObjectIdentifier DeviceIdentifier { set; get; }
        public UInt32 MaxAPDULength { set; get; }
        public uint SegmentationSupported { set; get; }
        public UInt16 VendorIdentifier { set; get; }
        public bool DestinationSpecifierPresent { set; get; }
        public UInt16 DNET { set; get; }
        public byte DLEN { set; get; }
        public byte[] DADR { set; get; }

        public IAmAnswer()
        {
            DeviceIdentifier = new BACnetObjectIdentifier();
            MaxAPDULength = 0;
            SegmentationSupported = 0;
            VendorIdentifier = 0;
            DestinationSpecifierPresent = false;
            DNET = 0;
            DLEN = 0;
            DADR = new byte[0];
        }
    }

    /// <summary>
    /// Service Tag
    /// </summary>
    public class ServiceTag : IPackable, IExpandable
        {
            #region Constructors

            public ServiceTag(sbyte contextTag = -1)
            {
                DefaultSettings();
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(bool inBool, sbyte contextTag = -1)
            {
                DefaultSettings();
                Boolean = inBool;
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(UInt32 inUInt, sbyte contextTag = -1)
            {
                DefaultSettings();
                UInt = inUInt;
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(Int32 inInt, sbyte contextTag = -1)
            {
                DefaultSettings();
                Int = inInt;
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(float inFloat, sbyte contextTag = -1)
            {
                DefaultSettings();
                Float = inFloat;
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(double inDouble, sbyte contextTag = -1)
            {
                DefaultSettings();
                Double = inDouble;
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(byte[] inOctetString, sbyte contextTag = -1)
            {
                DefaultSettings();
                OctetString = inOctetString;
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(string inString, sbyte contextTag = -1)
            {
                DefaultSettings();
                CharacterString = inString;
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(Enumerated inEnumerated, sbyte contextTag = -1)
            {
                DefaultSettings();
                Enumerated = new Enumerated(inEnumerated.DataArray);
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(BitString inBitString, sbyte contextTag = -1)
            {
                DefaultSettings();
                BitString = new BitString(inBitString.value);
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(Date inDate, sbyte contextTag = -1)
            {
                DefaultSettings();
                Date = new Date(inDate.value);
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(Time inTime, sbyte contextTag = -1)
            {
                DefaultSettings();
                Time = new Time(inTime.value);
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(BACnetObjectIdentifier inObjectID, sbyte contextTag = -1)
            {
                DefaultSettings();
                ObjectID = new BACnetObjectIdentifier(inObjectID.valueUint);
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(ServiceTagList inTagList, sbyte contextTag = -1)
            {
                DefaultSettings();
                TagList = inTagList;
                if (contextTag >= 0)
                    ContextTag = (byte)contextTag;
            }
            public ServiceTag(BACnetCommJob job, byte contextTag,out bool noDataToWrite, out string error)
            {
                noDataToWrite = true;

                DefaultSettings();
                TagList = new ServiceTagList();
                ContextTag = contextTag;
                if (job.IsRelinquishForced())
                {
                    ServiceTag nullTag = new ServiceTag();
                    nullTag.Values = null;
                    TagList.Add(new ServiceTag());
                    error = null;
                    return;
                }

                ServiceTag jobTag = new ServiceTag();
                object jobData = null;
                job.GetJobData(ref jobData);
                if (jobData == null || job.TagsListOnWriting.Count == 0)
                {                
                    if (jobData == null) { 
                        error = string.Format("jobData == null on ServiceTag");
                        return;
                    }
                    if (job.TagsListOnWriting.Count == 0)
                    {
                        error = string.Format("job.TagsListOnWriting.Count == 0 on ServiceTag");
                        return;
                    }
                    error = string.Empty;
                    return;
                }
                List<byte> jobDataList = new List<byte>((byte[])jobData);
                try
                {
                    switch (job.PropertyIdentifier)
                    {
                        case BACnetEnums.PropertyIdentifier.DATE_LIST:
                            while (jobDataList.Count() >= BACnetEnums.CalendarEntrySize)
                            {
                                bool rt = false;
                                jobTag.ContextTag = (byte)(jobDataList[0] - 1);
                                switch ((BACnetEnums.CalendarEntryTags)jobTag.ContextTag)
                                {
                                    case BACnetEnums.CalendarEntryTags.Date:
                                        jobTag.Values = jobDataList.GetRange(1, 4).ToArray();
                                        break;
                                    case BACnetEnums.CalendarEntryTags.DateRange:
                                        jobTag.TagList = new ServiceTagList();
                                        jobTag.TagList.Add(new ServiceTag(new Date(BufferExpand.swapArray(jobDataList.GetRange(1, 4).ToArray()))));
                                        jobTag.TagList.Add(new ServiceTag(new Date(BufferExpand.swapArray(jobDataList.GetRange(5, 4).ToArray()))));
                                        break;
                                    case BACnetEnums.CalendarEntryTags.WeekNDay:
                                        jobTag.Values = jobDataList.GetRange(1, 3).ToArray();
                                        break;
                                    default:
                                        rt = true;
                                        break;
                                }
                                if (rt)
                                    break;
                                else
                                {
                                    TagList.Add(jobTag);
                                    jobDataList.RemoveRange(0, BACnetEnums.CalendarEntrySize);
                                    jobTag = new ServiceTag();
                                }
                            }
                            break;
                        case BACnetEnums.PropertyIdentifier.EFFECTIVE_PERIOD:
                            TagList.Add(new ServiceTag(new Date(BufferExpand.swapArray(jobDataList.GetRange(0, 4).ToArray()))));
                            TagList.Add(new ServiceTag(new Date(BufferExpand.swapArray(jobDataList.GetRange(4, 4).ToArray()))));
                            break;
                        case BACnetEnums.PropertyIdentifier.WEEKLY_SCHEDULE:
                            jobTag.ContextTag = 0;
                            jobTag.TagList = new ServiceTagList();
                            AddEvents(jobTag, jobDataList);
                            TagList.Add(jobTag);
                            break;
                        case BACnetEnums.PropertyIdentifier.LIST_OF_OBJECT_PROPERTY_REFERENCES:
                            while (jobDataList.Count() >= BACnetEnums.PropertyIdentifierSize)
                            {
                                UInt32 auxVal = BitConverter.ToUInt16(jobDataList.GetRange(4, 2).ToArray(), 0);
                                if ((BACnetEnums.PropertyIdentifier)auxVal != BACnetEnums.PropertyIdentifier.ACKED_TRANSITIONS)
                                {
                                    TagList.Add(new ServiceTag(new BACnetObjectIdentifier(BitConverter.ToUInt32(jobDataList.GetRange(0, 4).ToArray(), 0)),
                                        (sbyte)BACnetEnums.ObjectPropertyReferencesTags.objectIdentifier));
                                    TagList.Add(new ServiceTag((UInt16)auxVal,
                                        (sbyte)BACnetEnums.ObjectPropertyReferencesTags.propertyIdentifier));
                                    auxVal = BitConverter.ToUInt16(jobDataList.GetRange(6, 2).ToArray(), 0);
                                    if (auxVal != BACnetEnums.DEFAULT_ARRAY_ID)
                                    {
                                        TagList.Add(new ServiceTag((UInt16)auxVal,
                                            (sbyte)BACnetEnums.ObjectPropertyReferencesTags.propertyArrayIndex));
                                    }
                                    auxVal = BitConverter.ToUInt32(jobDataList.GetRange(8, 4).ToArray(), 0);
                                    if (auxVal != BACnetEnums.DEFAULT_DEVICE_ID)
                                    {
                                        TagList.Add(new ServiceTag(new BACnetObjectIdentifier(auxVal),
                                            (sbyte)BACnetEnums.ObjectPropertyReferencesTags.deviceIdentifier));
                                    }
                                }
                                jobDataList.RemoveRange(0, BACnetEnums.PropertyIdentifierSize);
                            }
                            break;
                        case BACnetEnums.PropertyIdentifier.EXCEPTION_SCHEDULE:
                            if (jobDataList[0] == 0)
                            {
                                jobTag.ContextTag = 0;
                                jobTag.TagList = new ServiceTagList();
                                ServiceTag aux = new ServiceTag();
                                aux.ContextTag = (byte)(jobDataList[1]);
                                switch ((BACnetEnums.CalendarEntryTags)aux.ContextTag)
                                {
                                    case BACnetEnums.CalendarEntryTags.Date:
                                        aux.Values = jobDataList.GetRange(2, 4).ToArray();
                                        break;
                                    case BACnetEnums.CalendarEntryTags.DateRange:
                                        aux.TagList = new ServiceTagList();
                                        aux.TagList.Add(new ServiceTag(new Date(BufferExpand.swapArray(jobDataList.GetRange(2, 4).ToArray()))));
                                        aux.TagList.Add(new ServiceTag(new Date(BufferExpand.swapArray(jobDataList.GetRange(6, 4).ToArray()))));
                                        break;
                                    case BACnetEnums.CalendarEntryTags.WeekNDay:
                                        aux.Values = jobDataList.GetRange(2, 3).ToArray();
                                        break;
                                    default:
                                        break;
                                }
                                jobTag.TagList.Add(aux);
                            }
                            else
                            {
                                jobTag.TagList.Add(new ServiceTag(new BACnetObjectIdentifier(BitConverter.ToUInt32(jobDataList.GetRange(2, 4).ToArray(), 0)), 1));
                            }

                            TagList.Add(jobTag);
                            jobDataList.RemoveRange(0, 10);

                            UInt32 eventPriority = jobDataList[0];
                            jobDataList.RemoveRange(0, 1);

                            jobTag = new ServiceTag();
                            jobTag.ContextTag = 2;
                            jobTag.TagList = new ServiceTagList();
                            AddEvents(jobTag, jobDataList);
                            if (jobTag.TagList.Count() > 0)
                            {
                                TagList.Add(jobTag);
                                jobTag = new ServiceTag(eventPriority, 3);
                                TagList.Add(jobTag);
                            }
                            else
                                TagList = new ServiceTagList();

                            break;
                        default:
                            jobTag.Type = BACnetEnums.ObjectPropertyTypeDictionary[job.BACnetObjectType.ToString()][job.PropertyIdentifier];
                            jobTag.Values = (byte[])jobData;
                            TagList.Add(jobTag);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    error = string.Format("Unexpected error on ServiceTag {0} data={1}", ex.Message, (jobData == null ? "null" : (jobData == null ? "null" : string.Join(",", ((byte[])jobData).Select(b => b.ToString("X2"))))));
                    _TagList.List = null;
                    _TagList = null;
                }

                error = string.Empty;

                noDataToWrite = false;
            }

            private static void AddEvents(ServiceTag jobTag, List<byte> jobDataList)
            {
                while (jobDataList.Count() >= BACnetEnums.TimeValueSize)
                {
                    ServiceTag aux = new ServiceTag();
                    try
                    {
                        double value = BitConverter.ToDouble(jobDataList.GetRange(BACnetEnums.TimeValueValue, sizeof(Double)).ToArray(), 0);
                        switch ((BACnetEnums.APPLICATION_TAG)jobDataList[4])
                        {
                            case BACnetEnums.APPLICATION_TAG.BOOLEAN:
                                aux.Boolean = (value != 0.0);
                                break;
                            case BACnetEnums.APPLICATION_TAG.UNSIGNED_INT:
                                aux.UInt = (UInt32)value;
                                break;
                            case BACnetEnums.APPLICATION_TAG.SIGNED_INT:
                                aux.Int = (Int32)value;
                                break;
                            case BACnetEnums.APPLICATION_TAG.REAL:
                                aux.Float = (float)value;
                                break;
                            case BACnetEnums.APPLICATION_TAG.DOUBLE:
                                aux.Double = value;
                                break;
                            case BACnetEnums.APPLICATION_TAG.ENUMERATED:
                                aux.UInt = (UInt32)value;
                                aux.Type = BACnetEnums.APPLICATION_TAG.ENUMERATED;
                                break;
                    }
                    }
                    catch
                    {
                    }
                    jobTag.TagList.Add(new ServiceTag(new Time(jobDataList.GetRange(0, BACnetEnums.TimeSize).ToArray())));
                    jobTag.TagList.Add(aux);
                    jobDataList.RemoveRange(0, BACnetEnums.TimeValueSize);
                }
            }

            #endregion

            #region Member
            private byte[] _EncodedData;
            #endregion

            private bool _Class;
            public bool isContextTag
            {
                set { _Class = value; }
                get { return _Class; }
            }
            private byte _ContextTag;
            public byte ContextTag
            {
                set
                {
                    _Class = true;
                    _ContextTag = value;
                }
                get { return _ContextTag; }
            }
            private bool _isClosing;
            public bool isClosing
            {
                set
                {
                    _isClosing = value;
                    _isTagList = false;
                }
                get { return _isClosing; }
            }
            private bool _isTagList;
            public bool isTagList
            {
                set
                {
                    _isClosing = false;
                    _isTagList = value;
                }
                get { return _isTagList; }
            }
            private BACnetEnums.APPLICATION_TAG _Type;
            public BACnetEnums.APPLICATION_TAG Type
            {
                set { _Type = value; }
                get { return _Type; }
            }
            public UInt32 Size
            {
                get
                {
                    if (_EncodedData != null)
                        return (UInt32)_EncodedData.Length;
                    else return 0;
                }
            }
            public bool Boolean
            {
                get
                {
                    if (_EncodedData != null)
                        return (bool)(_EncodedData[0] == 0 ? false : true);
                    else return false;
                }
                set
                {
                    _EncodedData = new byte[1];
                    _EncodedData[0] = (byte)(value == false ? 0 : 1);
                    _Type = BACnetEnums.APPLICATION_TAG.BOOLEAN;
                }
            }
            public UInt32 UInt
            {
                get
                {
                    if (_EncodedData != null)
                        return BufferExpand.getUInt32(_EncodedData);
                    else return 0;
                }
                set
                {
                    _EncodedData = BufferExpand.getArray(value);
                    _Type = BACnetEnums.APPLICATION_TAG.UNSIGNED_INT;
                }
            }
            public Int32 Int
            {
                get
                {
                    if (_EncodedData != null)
                        return (Int32)BufferExpand.getUInt32(_EncodedData);
                    else return 0;
                }
                set
                {
                    _EncodedData = BufferExpand.getArray((UInt32)value);
                    _Type = BACnetEnums.APPLICATION_TAG.SIGNED_INT;

                }
            }
            public float Float
            {
                get
                {
                    if (_EncodedData != null)
                        return BufferExpand.toFloat(_EncodedData);
                    else return 0;
                }
                set
                {
                    _EncodedData = BufferExpand.toArray(value);
                    _Type = BACnetEnums.APPLICATION_TAG.REAL;
                }
            }
            public double Double
            {
                get
                {
                    if (_EncodedData != null)
                        return BufferExpand.toDouble(_EncodedData);
                    else return 0;
                }
                set
                {
                    _EncodedData = BufferExpand.toArray(value);
                    _Type = BACnetEnums.APPLICATION_TAG.DOUBLE;
                }
            }
            public byte[] OctetString
            {
                get
                {
                    if (_EncodedData == null)
                        return null;
                    if (_Type == BACnetEnums.APPLICATION_TAG.CHARACTER_STRING)
                        return Encoding.UTF8.GetBytes(CharacterString);
                    return _EncodedData;
                }
                set
                {
                    _EncodedData = value;
                    _Type = BACnetEnums.APPLICATION_TAG.OCTET_STRING;
                }
            }
            public string CharacterString
            {
                get
                {
                    if (_EncodedData == null)
                        return null;
                    return BufferExpand.toString(_EncodedData);
                }
                set
                {
                    _EncodedData = BufferExpand.toArray(value);
                    _Type = BACnetEnums.APPLICATION_TAG.CHARACTER_STRING;
                }
            }
            public BitString BitString
            {
                get
                {
                    return new BitString(_EncodedData);
                }
                set
                {
                    _EncodedData = BufferExpand.toBitString(value.value);
                    _Type = BACnetEnums.APPLICATION_TAG.BIT_STRING;
                }
            }
            public Enumerated Enumerated
            {
                get
                {
                    return new Enumerated(_EncodedData);
                }
                set
                {
                    _EncodedData = value.DataArray ;
                    _Type = BACnetEnums.APPLICATION_TAG.ENUMERATED;
                }
            }
            public Date Date
            {
                get { return new Date(_EncodedData); }
                set
                {
                    _EncodedData = value.value;
                    _Type = BACnetEnums.APPLICATION_TAG.DATE;
                }
            }
            public Time Time
            {
                get { return new Time(_EncodedData); }
                set
                {
                    _EncodedData = value.value;
                    _Type = BACnetEnums.APPLICATION_TAG.TIME;
                }
            }
            public BACnetObjectIdentifier ObjectID
            {
                get
                {
                    if (_EncodedData == null)
                        return new BACnetObjectIdentifier();
                    return new BACnetObjectIdentifier(BufferExpand.toUInt32(_EncodedData));
                }
                set
                {
                    _EncodedData = BufferExpand.toArray(value.valueUint);
                    _Type = BACnetEnums.APPLICATION_TAG.OBJECT_ID;
                }
            }
            public byte[] Values
            {
                get
                {
                    if (_EncodedData == null)
                        return null;
                    switch (_Type)
                    {
                        case BACnetEnums.APPLICATION_TAG.CHARACTER_STRING:
                            return Encoding.UTF8.GetBytes(CharacterString);
                        case BACnetEnums.APPLICATION_TAG.OCTET_STRING:
                        case BACnetEnums.APPLICATION_TAG.BOOLEAN:
                        case BACnetEnums.APPLICATION_TAG.OBJECT_ID:
                        case BACnetEnums.APPLICATION_TAG.TIME:
                            return _EncodedData;
                        case BACnetEnums.APPLICATION_TAG.BIT_STRING:
                            return BufferExpand.fromBitString(_EncodedData);
                        default:
                            return BufferExpand.swapArray(_EncodedData);
                    }
                }
                set
                {
                    if (value == null)
                    {
                        _EncodedData = null;
                        return;
                    }
                    switch (_Type)
                    {
                        case BACnetEnums.APPLICATION_TAG.CHARACTER_STRING:
                            CharacterString = Encoding.UTF8.GetString(value);
                            break;
                        case BACnetEnums.APPLICATION_TAG.OCTET_STRING:
                        case BACnetEnums.APPLICATION_TAG.BOOLEAN:
                        case BACnetEnums.APPLICATION_TAG.OBJECT_ID:
                            _EncodedData = value;
                            break;
                        case BACnetEnums.APPLICATION_TAG.BIT_STRING:
                            _EncodedData = BufferExpand.toBitString(value);
                            break;
                        default:
                            _EncodedData = BufferExpand.swapArray(value);
                            break;
                    }
                }
            }
            public Double doubleValue
            {
                get
                {
                    try
                    {
                        switch (_Type)
                        {
                            case BACnetEnums.APPLICATION_TAG.BOOLEAN:
                                return (double)_EncodedData[0];
                            case BACnetEnums.APPLICATION_TAG.ENUMERATED:
                            case BACnetEnums.APPLICATION_TAG.UNSIGNED_INT:
                                return (double)BufferExpand.getUInt32(_EncodedData);
                            case BACnetEnums.APPLICATION_TAG.SIGNED_INT:
                                return (double)((Int32)BufferExpand.getUInt32(_EncodedData));
                            case BACnetEnums.APPLICATION_TAG.REAL:
                                return System.Convert.ToDouble(BufferExpand.toFloat(_EncodedData).ToString());
                            case BACnetEnums.APPLICATION_TAG.DOUBLE:
                                return BufferExpand.toDouble(_EncodedData);
                            default:
                                return 0;
                        }
                    }
                    catch (NotImplementedException notImp)
                    {
                        return 0;
                    }
                }
                set
                {
                    switch (_Type)
                    {
                        default:
                            _EncodedData = new byte[0];
                            break;
                    }
                }
            }

            private ServiceTagList _TagList;
            public ServiceTagList TagList
            {
                get
                {
                    return _TagList;
                }
                set
                {
                    isTagList = true; 
                    _TagList = value;
                }
            }

            public byte[] Pack()
            {
                List<byte> retVal = new List<byte>();
                if (_isTagList)
                {
                    retVal.Add((byte)((((byte)_ContextTag << 4) & 0xF0) + 0x0E));
                    retVal.AddRange(TagList.Pack());
                    retVal.Add((byte)((((byte)_ContextTag << 4) & 0xF0) + 0x0F));
                }
                else if(_EncodedData == null)
                {
                    retVal.Add(0);
                }
                else if (_Type == BACnetEnums.APPLICATION_TAG.BOOLEAN)
                {
                    if (!_Class || _ContextTag > 0x0f)
                    {
                        retVal.Add((byte)(0x10 + _EncodedData[0]));
                    }
                    else
                    {
                        retVal.Add((byte)((((byte)_ContextTag << 4) & 0xF0) + 9));
                        retVal.Add(_EncodedData[0]);
                    }
                }
                else
                {
                    byte Tag;
                    if (!_Class)
                        Tag = (byte)((byte)_Type << 4);
                    else
                    {
                        if (_ContextTag <= 0xf)
                            Tag = (byte)((_ContextTag << 4) + 8);
                        else
                            return new byte[0];
                    }
                    if (Size < 0x05)
                    {
                        Tag = (byte)((((byte)Size) & 0x07) + (Tag & (0x07 ^ 0xFF)));
                        retVal.Add(Tag);
                    }
                    else if (Size < 0xfE)
                    {
                        Tag = (byte)(0x05 + (Tag & (0x07 ^ 0xFF)));
                        retVal.Add(Tag);
                        retVal.Add((byte)Size);
                    }
                    else if (Size < 0x10000)
                    {
                        Tag = (byte)(0x05 + (Tag & (0x07 ^ 0xFF)));
                        retVal.Add(Tag);
                        retVal.Add(0xFE);
                        retVal.Add((byte)(Size >> 8));
                        retVal.Add((byte)(Size & 0xFF));
                    }
                    else
                    {
                        Tag = (byte)(0x05 + (Tag & (0x07 ^ 0xFF)));
                        retVal.Add(Tag);
                        retVal.Add(0xFF);
                        retVal.Add((byte)(Size >> 24));
                        retVal.Add((byte)((Size >> 16) & 0xFF));
                        retVal.Add((byte)((Size >> 8) & 0xFF));
                        retVal.Add((byte)(Size & 0xFF));
                    }

                    retVal.AddRange(_EncodedData);
                }

                return retVal.ToArray();
            }

            public void Expand(ref byte[] DataArray, ref int Offset)
            {
                byte Tag = BufferExpand.toByte(ref DataArray, ref Offset);
                _Class = Convert.ToBoolean(Tag & (1 << 3));
                if (_Class)
                    _ContextTag = (byte)((Tag & 0xF0) >> 4);
                else
                    _Type = (BACnetEnums.APPLICATION_TAG)((Tag & 0xF0) >> 4);
                if (_Type == BACnetEnums.APPLICATION_TAG.BOOLEAN)
                {
                    _EncodedData = new byte[1];
                    _EncodedData[0] = (byte)(Tag & 0x01);
                }
                else
                {
                    UInt32 _Size;
                    if ((Tag & 0x07) < 6)
                    {
                        if ((Tag & 0x07) < 5)
                            _Size = (UInt32)(Tag & 0x07);
                        else
                        {
                            Tag = BufferExpand.toByte(ref DataArray, ref Offset);
                            if (Tag < 0xFE)
                                _Size = Tag;
                            else if (Tag == 0xFE)
                                _Size = BufferExpand.toUInt16(ref DataArray, ref Offset);
                            else
                                _Size = BufferExpand.toUInt32(ref DataArray, ref Offset);
                        }
                        if (_Size != 0)
                            _EncodedData = BufferExpand.toArray(ref DataArray, ref Offset, _Size);
                    }
                    else if ((Tag & 0x07) == 7)
                    {
                        isClosing = true;
                    }
                    else if ((Tag & 0x07) == 6)
                    {
                        ServiceTagList tmpTagList = new ServiceTagList();
                        tmpTagList.context = (sbyte)_ContextTag; 
                        tmpTagList.Expand(ref DataArray, ref Offset);
                        TagList = tmpTagList;
                    }

                }

                int NewOffset = Offset;

            }

            public void DefaultSettings()
            {
                _Class = false;
                _Type = BACnetEnums.APPLICATION_TAG.NULL;
                _EncodedData = null;
            }


        }

    public static class IPFrameFactory
    {
        public enum WriteFormatResult
        {
            UNDEFINED,
            OK,
            ERR_PROPERTY_IDENTIFIER_MISSING,
            ERR_NO_DATA_TO_WRITE,
            ERR_NO_DATA_TO_WRITE_GENERIC,
        }

        public static BACnetIPFrame WhoIs(BACnetStation Station)
        {
            BACnetIPFrame BACnetIPFrame = new BACnet.BACnetIPFrame(BACnetEnums.UnconfirmedService.WHO_IS, Station.BBMDRegister);
            BACnetIPFrame.Npdu.DestinationSpecifierPresent = true;
            BACnetIPFrame.Npdu.DNET = 0xffff;
            BACnetIPFrame.Npdu.DLEN = 0;
            return BACnetIPFrame;
        }
        public static BACnetIPFrame WhoHas(string ObjectName, BACnetStation Station)//, BACnetObjectIdentifier deviceInstance = null)
        {
            BACnetIPFrame BACnetIPFrame = new BACnet.BACnetIPFrame(BACnetEnums.UnconfirmedService.WHO_HAS, Station.BBMDRegister);

            //if (deviceInstance != null) {
            //    //BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(deviceInstance.instance-1, 0));
            //    //BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(deviceInstance.instance+1, 1));
            //    BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(0, 0));
            //    BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(4194302, 1));
            //}

            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(ObjectName, 3));
            if (Station.DestinationSpecifierPresent)
            {
                BACnetIPFrame.Npdu.DestinationSpecifierPresent = Station.DestinationSpecifierPresent;
                BACnetIPFrame.Npdu.DNET = Station.DNET;
                BACnetIPFrame.Npdu.DLEN = Station.DLEN;
                BACnetIPFrame.Npdu.DADR = Station.DADR;
            }
            return BACnetIPFrame;
        }
        public static BACnetIPFrame TimeSynch(BACnetStation Station)
        {
            BACnetIPFrame BACnetIPFrame = new BACnetIPFrame(BACnetEnums.UnconfirmedService.TIME_SYNCHRONIZATION, Station.BBMDRegister);
            DateTime Now = DateTime.Now;
            if (Station.DestinationSpecifierPresent)
            {
                BACnetIPFrame.Npdu.DestinationSpecifierPresent = Station.DestinationSpecifierPresent;
                BACnetIPFrame.Npdu.DNET = Station.DNET;
                BACnetIPFrame.Npdu.DLEN = Station.DLEN;
                BACnetIPFrame.Npdu.DADR = Station.DADR;
            }
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(new Date(Now)));
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(new Time(Now)));
            return BACnetIPFrame;
        }
        public static BACnetIPFrame UtcTimeSynch(BACnetStation Station)
        {
            BACnetIPFrame BACnetIPFrame = new BACnetIPFrame(BACnetEnums.UnconfirmedService.UTC_TIME_SYNCHRONIZATION, Station.BBMDRegister);
            DateTime UtcNow = DateTime.UtcNow;
            if (Station.DestinationSpecifierPresent)
            {
                BACnetIPFrame.Npdu.DestinationSpecifierPresent = Station.DestinationSpecifierPresent;
                BACnetIPFrame.Npdu.DNET = Station.DNET;
                BACnetIPFrame.Npdu.DLEN = Station.DLEN;
                BACnetIPFrame.Npdu.DADR = Station.DADR;
            }
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(new Date(UtcNow)));
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(new Time(UtcNow)));
            return BACnetIPFrame;
        }
        public static BACnetIPFrame confirmedSubscribeCov(byte InvokeID, UInt32 SubscriberIdentifier, BACnetObjectIdentifier MonitoredObjectID, BACnetStation Station)
        {
            BACnetIPFrame BACnetIPFrame = new BACnetIPFrame(BACnetEnums.ConfirmedService.SUBSCRIBE_COV);
            if (Station.DestinationSpecifierPresent)
            {
                BACnetIPFrame.Npdu.DestinationSpecifierPresent = Station.DestinationSpecifierPresent;
                BACnetIPFrame.Npdu.DNET = Station.DNET;
                BACnetIPFrame.Npdu.DLEN = Station.DLEN;
                BACnetIPFrame.Npdu.DADR = Station.DADR;
            }
            BACnetIPFrame.Npdu.Apdu.InvokeId = InvokeID;
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(SubscriberIdentifier, 0));
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(MonitoredObjectID, 1));
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(false, 2));
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag((UInt32)(Station.COVInterval == 0 ? 0 : Station.COVInterval + 1 ), 3));
            return BACnetIPFrame;
        }
        public static BACnetIPFrame confirmedReadProperty(BACnetCommJob job)
        {
            BACnetStation Station = job.Station as BACnetStation;
            BACnetIPFrame BACnetIPFrame = new BACnetIPFrame(BACnetEnums.ConfirmedService.READ_PROPERTY);            
            if (Station.DestinationSpecifierPresent)
            {
                BACnetIPFrame.Npdu.DestinationSpecifierPresent = Station.DestinationSpecifierPresent;
                BACnetIPFrame.Npdu.DNET = Station.DNET;
                BACnetIPFrame.Npdu.DLEN = Station.DLEN;
                BACnetIPFrame.Npdu.DADR = Station.DADR;
            }
            BACnetIPFrame.Npdu.Apdu.InvokeId = job.InvokeID;
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(job.ObjectID, 0));
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag((uint)job.PropertyIdentifier, 1));
            switch (job.PropertyIdentifier)
            {
                case BACnetEnums.PropertyIdentifier.STATE_TEXT:                
                case BACnetEnums.PropertyIdentifier.WEEKLY_SCHEDULE:
                case BACnetEnums.PropertyIdentifier.EXCEPTION_SCHEDULE:
                    BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag((uint)job.ArrayIndex , 2));
                    break;
                case BACnetEnums.PropertyIdentifier.PRIORITY_ARRAY:
                    if (job.TagsList[0].TagNode.ArrayDimension != 0)
                        BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag((uint)job.ArrayIndex, (sbyte)job.TagsList[0].TagNode.ArrayDimension));
                    else
                        BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag((uint)job.ArrayIndex, 2));
                    break;
                default:
                    break;
            }

            return BACnetIPFrame;
        }

        public static WriteFormatResult confirmedWriteProperty(out BACnetIPFrame BACnetIPFrame, ref BACnetCommJob job, out string error)
        {
            error = string.Empty;

            BACnetStation Station = job.Station as BACnetStation;
            BACnetIPFrame = new BACnetIPFrame(BACnetEnums.ConfirmedService.WRITE_PROPERTY);
            if (!BACnetEnums.ObjectPropertyWritableDictionary[job.ObjectID.ObjectType.ToString()][job.PropertyIdentifier])
                return WriteFormatResult.ERR_PROPERTY_IDENTIFIER_MISSING;       
            
            if (Station.DestinationSpecifierPresent)
            {
                BACnetIPFrame.Npdu.DestinationSpecifierPresent = Station.DestinationSpecifierPresent;
                BACnetIPFrame.Npdu.DNET = Station.DNET;
                BACnetIPFrame.Npdu.DLEN = Station.DLEN;
                BACnetIPFrame.Npdu.DADR = Station.DADR;
            }
            BACnetIPFrame.Npdu.Apdu.InvokeId = job.InvokeID;
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag(job.ObjectID, 0));
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag((uint)job.PropertyIdentifier, 1));
            switch (job.PropertyIdentifier)
            {
                case BACnetEnums.PropertyIdentifier.STATE_TEXT:
                case BACnetEnums.PropertyIdentifier.PRIORITY_ARRAY:
                case BACnetEnums.PropertyIdentifier.WEEKLY_SCHEDULE:
                case BACnetEnums.PropertyIdentifier.EXCEPTION_SCHEDULE:
                    BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag((uint)job.ArrayIndex, 2));
                    break;
                default:
                    break;
            }

            bool noDataToWrite;
            ServiceTag outValue = new ServiceTag(job, 3,out noDataToWrite,out error);
            //no value to write or no value changed
            if(outValue.TagList == null || outValue.TagList.Count() ==0)
            {
                if (noDataToWrite)
                    return WriteFormatResult.ERR_NO_DATA_TO_WRITE;
                else
                    return WriteFormatResult.ERR_NO_DATA_TO_WRITE_GENERIC;
            }
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(outValue);
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(new ServiceTag((UInt32)job.PriorityLevel,4));

            return WriteFormatResult.OK;
        }

        public static BACnetIPFrame RegisterAsForeignDevice(BACnetChannel channel, ushort bBMDLifetime)
        {
            BACnetIPFrame BACnetIPFrame = new BACnet.BACnetIPFrame(BACnetEnums.BACnet_BVLC_FUNCTION.BVLC_REGISTER_FOREIGN_DEVICE);
            BACnetIPFrame.BBMDLifetime = bBMDLifetime;
            BACnetIPFrame.Npdu.DestinationSpecifierPresent = true;
            BACnetIPFrame.Npdu.DNET = 0xffff;
            BACnetIPFrame.Npdu.DLEN = 0;
            return BACnetIPFrame;
        }

        public static BACnetIPFrame ReadForeignDeviceTable(BACnetChannel channel)
        {
            BACnetIPFrame BACnetIPFrame = new BACnet.BACnetIPFrame(BACnetEnums.BACnet_BVLC_FUNCTION.BVLC_READ_FOREIGN_DEVICE_TABLE);
            BACnetIPFrame.Npdu.DestinationSpecifierPresent = true;
            BACnetIPFrame.Npdu.DNET = 0xffff;
            BACnetIPFrame.Npdu.DLEN = 0;
            return BACnetIPFrame;
        }

        public static BACnetIPFrame ReadBBMDDeviceTable(BACnetChannel channel)
        {
            BACnetIPFrame BACnetIPFrame = new BACnet.BACnetIPFrame(BACnetEnums.BACnet_BVLC_FUNCTION.BVLC_READ_BROADCAST_DIST_TABLE);
            BACnetIPFrame.Npdu.DestinationSpecifierPresent = true;
            BACnetIPFrame.Npdu.DNET = 0xffff;
            BACnetIPFrame.Npdu.DLEN = 0;
            return BACnetIPFrame;
        }

        public static BACnetIPFrame iAmARouterToNetwork()
        {
            BACnetIPFrame BACnetIPFrame = new BACnetIPFrame(BACnetEnums.UnconfirmedService.I_AM, false);
            BACnetIPFrame.Header.BVLCfunction = BACnetEnums.BVLC_Functions.BcastNPDU;

            ServiceTag jobTag = new ServiceTag();

            BACnetIPFrame.Npdu.MessageTypePresent = true;
            BACnetIPFrame.Npdu.SourceSpecifierPresent = false;
            BACnetIPFrame.Npdu.DestinationSpecifierPresent = true;
            BACnetIPFrame.Npdu.DNET = 0xffff;
            BACnetIPFrame.Npdu.DLEN = 0;
            // reuse this field for DNET scope in this command (instead of modify protocol class)
            BACnetIPFrame.Npdu.VendorID = 20;

            BACnetIPFrame.Npdu.MessageType = (byte)BACnetEnums.BACnet_NETWORK_MESSAGE_TYPE.NETWORK_MESSAGE_I_AM_ROUTER_TO_NETWORK; 

            return BACnetIPFrame;
        }

        public static BACnetIPFrame iAm(int deviceId,UInt16 vendorID)
        {
            BACnetIPFrame BACnetIPFrame = new BACnetIPFrame(BACnetEnums.UnconfirmedService.I_AM, false);
            ServiceTag jobTag = new ServiceTag();

            //BACnetIPFrame.Npdu.SourceSpecifierPresent = true;            
            //BACnetIPFrame.Npdu.SADR = new byte[] { 0x0, 0x0, 0x0, 0x0, 0x0, 0x0 };
            //BACnetIPFrame.Npdu.SNET = 0; // 20;
            //BACnetIPFrame.Npdu.DestinationSpecifierPresent = false;
                        
            BACnetIPFrame.Npdu.SourceSpecifierPresent = false;
            BACnetIPFrame.Npdu.DestinationSpecifierPresent = true;
            BACnetIPFrame.Npdu.DNET = 0xffff;
            BACnetIPFrame.Npdu.DLEN = 0;
            // reuse this field for DNET scope in this command (instead of modify protocol class)
            BACnetIPFrame.Npdu.VendorID = 0;

            jobTag = new ServiceTag(new BACnetObjectIdentifier(BACnetEnums.ObjectTypes.DEVICE, (UInt32)deviceId));
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(jobTag);

            jobTag = new ServiceTag();
            jobTag.Type = BACnetEnums.ObjectPropertyTypeDictionary[BACnetEnums.ObjectTypes.DEVICE.ToString()][BACnetEnums.PropertyIdentifier.MAX_APDU_LENGTH_ACCEPTED];
            jobTag.Values = BitConverter.GetBytes(BACnetEnums.MAX_APDU_LENGTH);
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(jobTag);

            jobTag = new ServiceTag();
            jobTag.Type = BACnetEnums.ObjectPropertyTypeDictionary[BACnetEnums.ObjectTypes.DEVICE.ToString()][BACnetEnums.PropertyIdentifier.SEGMENTATION_SUPPORTED];
            jobTag.Values = BitConverter.GetBytes((byte)BACnetEnums.BACnet_SEGMENTATION.SEGMENTATION_NONE);
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(jobTag);

            jobTag = new ServiceTag();
            jobTag.Type = BACnetEnums.ObjectPropertyTypeDictionary[BACnetEnums.ObjectTypes.DEVICE.ToString()][BACnetEnums.PropertyIdentifier.VENDOR_IDENTIFIER];
            jobTag.Values = BitConverter.GetBytes(vendorID);
            BACnetIPFrame.Npdu.Apdu.ServiceRequest.Add(jobTag);

            return BACnetIPFrame;
        }
    }

    public static class IPFrameValidator
    {
        public static bool isIAm(BACnetIPFrame frame)
        {
            if (frame.Npdu.Apdu.UnconfirmedServiceChoice != BACnetEnums.UnconfirmedService.I_AM)
                return false;
            if (frame.Npdu.Apdu.ServiceRequest.List.Count() != 4)
                return false;

            if (frame.Npdu.Apdu.ServiceRequest.List[0].Type != BACnetEnums.APPLICATION_TAG.OBJECT_ID)
                return false;
            if (frame.Npdu.Apdu.ServiceRequest.List[1].Type != BACnetEnums.APPLICATION_TAG.UNSIGNED_INT)
                return false;
            if (frame.Npdu.Apdu.ServiceRequest.List[2].Type != BACnetEnums.APPLICATION_TAG.ENUMERATED)
                return false;
            if (frame.Npdu.Apdu.ServiceRequest.List[3].Type != BACnetEnums.APPLICATION_TAG.UNSIGNED_INT)
                return false;

            return true;
        }

        public static bool IAm(BACnetIPFrame frame, out IAmAnswer answer)
        {
            answer = new IAmAnswer();

            if (!isIAm(frame))
                return false;

            answer.DeviceIdentifier = frame.Npdu.Apdu.ServiceRequest.List[0].ObjectID;
            answer.MaxAPDULength = frame.Npdu.Apdu.ServiceRequest.List[1].UInt;
            answer.SegmentationSupported = frame.Npdu.Apdu.ServiceRequest.List[2].Enumerated.Uint;
            answer.VendorIdentifier = (UInt16)frame.Npdu.Apdu.ServiceRequest.List[3].UInt;
            if(frame.Npdu.SourceSpecifierPresent)
            {
                answer.DestinationSpecifierPresent = true;
                answer.DNET = frame.Npdu.SNET;
                answer.DLEN = frame.Npdu.SLEN;
                answer.DADR = frame.Npdu.SADR;
            }
            else
                answer.DestinationSpecifierPresent = false;


            return true;
        }

        private static bool isIHave(BACnetIPFrame frame)
        {
            if (frame.Npdu.Apdu.UnconfirmedServiceChoice != BACnetEnums.UnconfirmedService.I_HAVE)
                return false;
            if (frame.Npdu.Apdu.ServiceRequest.List.Count() != 3)
                return false;
            if (frame.Npdu.Apdu.ServiceRequest.List[0].Type != BACnetEnums.APPLICATION_TAG.OBJECT_ID)
                return false;
            if (frame.Npdu.Apdu.ServiceRequest.List[1].Type != BACnetEnums.APPLICATION_TAG.OBJECT_ID)
                return false;
            if (frame.Npdu.Apdu.ServiceRequest.List[2].Type != BACnetEnums.APPLICATION_TAG.CHARACTER_STRING)
                return false;
            return true;
        }

        private static bool isIHave(BACnetIPFrame frame, BACnetObjectIdentifier DeviceID, string ObjectName)
        {
            if (!isIHave(frame))
                return false;

            if (DeviceID.valueUint != frame.Npdu.Apdu.ServiceRequest.List[0].ObjectID.valueUint)
                return false;
            if (ObjectName != frame.Npdu.Apdu.ServiceRequest.List[2].CharacterString)
                return false;
            return true;
        }

        public static bool IHave(BACnetIPFrame frame, BACnetObjectIdentifier DeviceID, string ObjectName, out BACnetObjectIdentifier outObjectID)
        {
            outObjectID = new BACnetObjectIdentifier();

            if (!isIHave(frame, DeviceID, ObjectName))
                return false;

            outObjectID = frame.Npdu.Apdu.ServiceRequest.List[1].ObjectID;

            return true;
        }

        public static bool isCovSubscriptionOk(BACnetIPFrame frame, byte InvokeID)
        {
            if (frame.Npdu.Apdu.PduType != BACnetEnums.BACnetPDU.SIMPLE_ACK)
                return false;
            if (frame.Npdu.Apdu.InvokeId != InvokeID)
                return false;
            if (frame.Npdu.Apdu.ConfirmedServiceChoice != BACnetEnums.ConfirmedService.SUBSCRIBE_COV)
                return false;

            return true;
        }
        public static bool isCovSubscriptionFailed(BACnetIPFrame frame, byte InvokeID)
        {
            if (frame.Npdu.Apdu.PduType != BACnetEnums.BACnetPDU.ERROR)
                return false;
            if (frame.Npdu.Apdu.InvokeId != InvokeID)
                return false;
            if (frame.Npdu.Apdu.ConfirmedServiceChoice != BACnetEnums.ConfirmedService.SUBSCRIBE_COV)
                return false;

            return true;
        }
        public static bool isUnconfirmedCovNotification(BACnetIPFrame frame)
        {
            if (frame.Npdu.Apdu.UnconfirmedServiceChoice != BACnetEnums.UnconfirmedService.COV_NOTIFICATION)
                return false;
            if (frame.Npdu.Apdu.ServiceRequest.List.Count() != 5)
                return false;
            if (!frame.Npdu.Apdu.ServiceRequest.List[4].isTagList)
                return false;
            if (frame.Npdu.Apdu.ServiceRequest.List[4].TagList.Count() != 4)
                return false;

            return true;
        }
        public static bool UnconfirmedCovNotification(BACnetIPFrame frame, out UInt32 outSubscriberID, out BACnetObjectIdentifier outDeviceID,
            out BACnetObjectIdentifier outObjectID, out UInt32 outTimeRemaining, out ServiceTagList TagList)
        {

            outSubscriberID = 0;
            outDeviceID = null;
            outObjectID = null;
            outTimeRemaining = 0;
            TagList = null;

            if (!isUnconfirmedCovNotification(frame))
                return false;

            outSubscriberID = frame.Npdu.Apdu.ServiceRequest.List[0].UInt;
            outDeviceID = frame.Npdu.Apdu.ServiceRequest.List[1].ObjectID;
            outObjectID = frame.Npdu.Apdu.ServiceRequest.List[2].ObjectID;
            outTimeRemaining = frame.Npdu.Apdu.ServiceRequest.List[3].UInt;
            TagList = frame.Npdu.Apdu.ServiceRequest.List[4].TagList;

            return true;
        }
        public static bool isErrorFrame(BACnetIPFrame frame)
        {
            return (frame.Npdu.Apdu.PduType == BACnetEnums.BACnetPDU.ERROR);
        }
        public static bool isRejectFrame(BACnetIPFrame frame)
        {
            return (frame.Npdu.Apdu.PduType == BACnetEnums.BACnetPDU.REJECT);
        }

        public static BACnetErrorCodes GetError(BACnetIPFrame frame)
        {
            if (!isErrorFrame(frame))
                return (BACnetErrorCodes)DriverErrorCodes.ErrorNoError;
            if (frame.Npdu.Apdu.ServiceRequest.List.Count() < 2)
                return BACnetErrorCodes.ErrorProtocolError;
            switch((BACnetEnums.ERROR_CODE)frame.Npdu.Apdu.ServiceRequest.List[1].Enumerated.Uint)
            {
                case BACnetEnums.ERROR_CODE.UNKNOWN_PROPERTY:
                    return BACnetErrorCodes.ErrorUnknownProperty;
                case BACnetEnums.ERROR_CODE.WRITE_ACCESS_DENIED:
                    return BACnetErrorCodes.ErrorWriteAccessDenied;
                case BACnetEnums.ERROR_CODE.VALUE_OUT_OF_RANGE:
                    return BACnetErrorCodes.ErrorValueOutOfRange;
                case BACnetEnums.ERROR_CODE.INVALID_DATA_TYPE:
                    return BACnetErrorCodes.ErrorInvalidDataType;
                case BACnetEnums.ERROR_CODE.UNKNOWN_OBJECT:
                    return BACnetErrorCodes.ErrorUnknownObject;
                default:
                    return BACnetErrorCodes.ErrorProtocolError;
            }
        }
        public static bool isForThisReadJob(BACnetIPFrame frame, byte InvokeID)
        {
            if (frame.Npdu.Apdu.InvokeId != InvokeID)
                return false;
            if (frame.Npdu.Apdu.ConfirmedServiceChoice != BACnetEnums.ConfirmedService.READ_PROPERTY)
                return false;

            return true;
        }
        public static bool isConfirmedReadProperty(BACnetIPFrame frame, byte InvokeID, BACnetObjectIdentifier ObjectID,
            BACnetEnums.PropertyIdentifier property, ushort ArrayIndex)
        {
            if (!isForThisReadJob(frame, InvokeID))
                return false;
            if (frame.Npdu.Apdu.ServiceRequest.List.Count() == 4)
            {
                if (ArrayIndex != frame.Npdu.Apdu.ServiceRequest.List[2].UInt)
                    return false;
            }
            else if (frame.Npdu.Apdu.ServiceRequest.List.Count() != 3)
                return false;
            if (ObjectID.valueUint != frame.Npdu.Apdu.ServiceRequest.List[0].ObjectID.valueUint)
                return false;
            if ((uint)property != frame.Npdu.Apdu.ServiceRequest.List[1].UInt)
                return false;

            return true;
        }

        public static bool ConfirmedReadProperty(BACnetIPFrame frame, byte InvokeID, BACnetObjectIdentifier ObjectID,
            BACnetEnums.PropertyIdentifier property, ushort ArrayIndex, BACnetCommJob job, out byte[] answer, out DriverErrorCodes error)
        {
            answer = null;
            error = DriverErrorCodes.ErrorNoError;

            if (!isConfirmedReadProperty(frame, InvokeID, ObjectID, property, ArrayIndex))
            {
                error = (DriverErrorCodes)BACnetErrorCodes.ErrorUnknownObject;
                return false;
            }

            int indxTagValue = frame.Npdu.Apdu.ServiceRequest.List.Count() - 1;
            List<byte> retVal = new List<byte>();
            bool objectPropertyReferencesPend = false;

            UInt16 PropID = BACnetEnums.MAX_BACnet_PROPERTY_ID;
            UInt16 ArrayIdx = BACnetEnums.DEFAULT_ARRAY_ID;
            UInt32 DeviceID = BACnetEnums.DEFAULT_DEVICE_ID;

            UInt32 calendarReference = 0;
            List<byte> periodCalendarEntry = new List<byte>();
            List<byte> listOfeventPriorityTimeValues = new List<byte>();

            foreach (ServiceTag Tag in frame.Npdu.Apdu.ServiceRequest.List[indxTagValue].TagList.List)
            {
                switch (property)
                {
                    case BACnetEnums.PropertyIdentifier.DATE_LIST:
                        if (!Tag.isContextTag)
                            break;
                        retVal.Add(((byte)(Tag.ContextTag + 1)));
                        switch ((BACnetEnums.CalendarEntryTags)Tag.ContextTag)
                        {
                            case BACnetEnums.CalendarEntryTags.Date:
                                AddRange(retVal, Tag.Values);
                                retVal.AddRange(new byte[4]);
                                break;
                            case BACnetEnums.CalendarEntryTags.DateRange:
                                AddRange(retVal, Tag.TagList.List[0].Values);
                                AddRange(retVal, Tag.TagList.List[1].Values);
                                break;
                            case BACnetEnums.CalendarEntryTags.WeekNDay:
                                AddRange(retVal, Tag.Values);
                                retVal.AddRange(new byte[5]);
                                break;

                        }
                        break;
                    case BACnetEnums.PropertyIdentifier.WEEKLY_SCHEDULE:
                        bool isTime = true;
                        foreach (ServiceTag weeklyTag in Tag.TagList.List)
                        {
                            if (isTime)
                                AddRange(retVal, weeklyTag.Values);
                            else
                            {
                                if (((byte)weeklyTag.Type > 5) && ((byte)weeklyTag.Type != 9))
                                    retVal.Add(0);
                                else
                                    retVal.Add((byte)weeklyTag.Type);
                                AddRange(retVal, BitConverter.GetBytes(weeklyTag.doubleValue));
                            }
                            isTime ^= true;
                        }
                        if (!isTime)
                            retVal.RemoveRange(retVal.Count() - 4, 4);
                        break;
                    case BACnetEnums.PropertyIdentifier.LIST_OF_OBJECT_PROPERTY_REFERENCES:
                        if (!Tag.isContextTag)
                            break;
                        switch ((BACnetEnums.ObjectPropertyReferencesTags)Tag.ContextTag)
                        {
                            case BACnetEnums.ObjectPropertyReferencesTags.objectIdentifier:
                                if (objectPropertyReferencesPend)
                                {
                                    if (PropID != BACnetEnums.MAX_BACnet_PROPERTY_ID &&
                                        (BACnetEnums.PropertyIdentifier)PropID != BACnetEnums.PropertyIdentifier.ACKED_TRANSITIONS)
                                    {
                                        retVal.AddRange(BitConverter.GetBytes(PropID));
                                        retVal.AddRange(BitConverter.GetBytes(ArrayIdx));
                                        retVal.AddRange(BitConverter.GetBytes(DeviceID));
                                    }
                                    else
                                        retVal.RemoveRange(retVal.Count() - 4, 4);


                                    PropID = BACnetEnums.MAX_BACnet_PROPERTY_ID;
                                    ArrayIdx = BACnetEnums.DEFAULT_ARRAY_ID;
                                    DeviceID = BACnetEnums.DEFAULT_DEVICE_ID;
                                }
                                AddRange(retVal, BitConverter.GetBytes(Tag.ObjectID.valueUint));
                                objectPropertyReferencesPend = true;
                                break;
                            case BACnetEnums.ObjectPropertyReferencesTags.propertyIdentifier:
                                PropID = (UInt16)Tag.UInt;
                                break;
                            case BACnetEnums.ObjectPropertyReferencesTags.propertyArrayIndex:
                                ArrayIdx = (UInt16)Tag.UInt;
                                break;
                            case BACnetEnums.ObjectPropertyReferencesTags.deviceIdentifier:
                                DeviceID = Tag.UInt;
                                break;
                        }
                        break;
                    case BACnetEnums.PropertyIdentifier.PRESENT_VALUE:
                        if (ObjectID.ObjectType == BACnetEnums.ObjectTypes.SCHEDULE)
                        {
                            AddRange(retVal, BitConverter.GetBytes(Tag.doubleValue));
                        }
                        else
                            AddRange(retVal, Tag.Values);
                        break;
                    case BACnetEnums.PropertyIdentifier.EXCEPTION_SCHEDULE:
                        if (!Tag.isContextTag)
                            break;
                        switch ((BACnetEnums.ExceptionScheduleTags)Tag.ContextTag)
                        {
                            case BACnetEnums.ExceptionScheduleTags.eventPriority:
                                if (periodCalendarEntry.Count() != 0)
                                {
                                    retVal.Add(0);
                                    retVal.AddRange(periodCalendarEntry);
                                }
                                else
                                {
                                    retVal.Add(1);
                                    retVal.AddRange(BitConverter.GetBytes(calendarReference));
                                    retVal.AddRange(new byte[5]);
                                }
                                retVal.Add((byte)Tag.UInt);
                                retVal.AddRange(listOfeventPriorityTimeValues);
                                break;
                            case BACnetEnums.ExceptionScheduleTags.listOfTimeValues:
                                bool isTimexception = true;
                                foreach (ServiceTag timeValueTag in Tag.TagList.List)
                                {
                                    if (isTimexception)
                                        AddRange(listOfeventPriorityTimeValues, timeValueTag.Values);
                                    else
                                    {
                                        if ((byte)timeValueTag.Type > 5)
                                            listOfeventPriorityTimeValues.Add(0);
                                        else
                                            listOfeventPriorityTimeValues.Add((byte)timeValueTag.Type);
                                        AddRange(listOfeventPriorityTimeValues, BitConverter.GetBytes(timeValueTag.doubleValue));
                                    }
                                    isTimexception ^= true;
                                }
                                if (!isTimexception)
                                    listOfeventPriorityTimeValues.RemoveRange(listOfeventPriorityTimeValues.Count() - 4, 4);
                                break;
                            case BACnetEnums.ExceptionScheduleTags.periodCalendarEntry:
                                if (!Tag.isTagList)
                                    break;
                                periodCalendarEntry.Add(((byte)(Tag.TagList.List[0].ContextTag)));
                                switch ((BACnetEnums.CalendarEntryTags)Tag.TagList.List[0].ContextTag)
                                {
                                    case BACnetEnums.CalendarEntryTags.Date:
                                        AddRange(periodCalendarEntry, Tag.TagList.List[0].Values);
                                        periodCalendarEntry.AddRange(new byte[4]);
                                        break;
                                    case BACnetEnums.CalendarEntryTags.DateRange:
                                        AddRange(periodCalendarEntry, Tag.TagList.List[0].TagList.List[0].Values);
                                        AddRange(periodCalendarEntry, Tag.TagList.List[0].TagList.List[1].Values);
                                        break;
                                    case BACnetEnums.CalendarEntryTags.WeekNDay:
                                        AddRange(periodCalendarEntry, Tag.TagList.List[0].Values);
                                        periodCalendarEntry.AddRange(new byte[5]);
                                        break;

                                }
                                break;
                            case BACnetEnums.ExceptionScheduleTags.periodCalendarReference:
                                calendarReference = Tag.UInt;
                                break;

                        }
                        break;
                    case BACnetEnums.PropertyIdentifier.FAULT_VALUES:
                    case BACnetEnums.PropertyIdentifier.ALARM_VALUES:
                        retVal.AddRange(BitConverter.GetBytes(Tag.UInt));
                        break;
                    case BACnetEnums.PropertyIdentifier.OBJECT_IDENTIFIER:
                        // 4 bytes data (represent object type + instance number) are in wrong order to be represent with UINT32 values --> reverse it
                        AddRange(retVal, Tag.Values.Reverse().ToArray());
                        break;
                    case BACnetEnums.PropertyIdentifier.PRIORITY_ARRAY:
                        if (Tag.Values != null)
                        {
                            //retVal.AddRange(Tag.Values);
                            byte[] tagValue = BACnetProtocol.ConvertPriorityArrayValueIntoTagFormat(Tag, job.TagsList[0]);
                            if (tagValue != null)
                            {                                
                                retVal.AddRange(tagValue);
                            }
                            else
                            {
                                error = (DriverErrorCodes)BACnetErrorCodes.ErrorValueOutOfRange;
                                return false;
                            }                            
                        }
                        else
                        {
                            byte[] nullValue = BACnetProtocol.GetPriorityArrayNullValue(job.TagsList[0]);
                            if (nullValue != null)
                            {
                                //instead of null value return "user" friendly value
                                retVal.AddRange(nullValue);
                            }
                            else
                            {
                                error = (DriverErrorCodes)BACnetErrorCodes.ErrorValueOutOfRange;
                                return false;
                            }
                        }
                        break;
                    default:
                        AddRange(retVal, Tag.Values);
                        break;
                }
            }
            if (property == BACnetEnums.PropertyIdentifier.LIST_OF_OBJECT_PROPERTY_REFERENCES &&
                objectPropertyReferencesPend)
            {
                if (PropID != BACnetEnums.MAX_BACnet_PROPERTY_ID &&
                    (BACnetEnums.PropertyIdentifier)PropID != BACnetEnums.PropertyIdentifier.ACKED_TRANSITIONS)
                {
                    retVal.AddRange(BitConverter.GetBytes(PropID));
                    retVal.AddRange(BitConverter.GetBytes(ArrayIdx));
                    retVal.AddRange(BitConverter.GetBytes(DeviceID));
                }
                else
                    retVal.RemoveRange(retVal.Count() - 4, 4);
            }

            if (retVal.Count != 0)
                answer = retVal.ToArray();
            return true;
        }
        private static void AddRange(List<byte> retVal, byte[] Values)
        {
            if (Values != null)
                retVal.AddRange(Values);

        }
        public static bool isForThisWriteJob(BACnetIPFrame frame, byte InvokeID)
        {
            if (frame.Npdu.Apdu.InvokeId != InvokeID)
                return false;
            if (frame.Npdu.Apdu.ConfirmedServiceChoice != BACnetEnums.ConfirmedService.WRITE_PROPERTY)
                return false;

            return true;
        }

        public static bool isconfirmedForeignDeviceRegistration(ReceiveItem receiveItem)
        {
            if (receiveItem.isValid && receiveItem.Frame.Header.BVLCfunction == BACnetEnums.BVLC_Functions.BVLC_Result)
            {
                if (receiveItem.Frame.Header.BVLCresult == BACnetEnums.BVLC_Result.Successful)
                {
                    receiveItem.isValid = false;
                    return true;
                }
            }

            return false;
        }

        public static bool isconfirmedReadBDTTableDeviceRegistration(ReceiveItem receiveItem)
        {
            if (receiveItem.isValid && receiveItem.Frame.Header.BVLCfunction == BACnetEnums.BVLC_Functions.ReBcastDistTableACK)
            {
                if (receiveItem.Frame.Header.BVLCresult == BACnetEnums.BVLC_Result.Successful)
                {
                    receiveItem.isValid = false;
                    return true;
                }
            }

            return false;
        }

        public static bool isConfirmedReadDFTTableDeviceRegistration(ReceiveItem receiveItem)
        {
            if (receiveItem.isValid && receiveItem.Frame.Header.BVLCfunction == BACnetEnums.BVLC_Functions.ReadDeviceTableACK)
            {
                if (receiveItem.Frame.Header.BVLCresult == BACnetEnums.BVLC_Result.Successful)
                {
                    receiveItem.isValid = false;
                    return true;
                }
            }

            return false;
        }

        public static bool isUnConfirmedWhoIsRequest(ReceiveItem receiveItem)
        {
            return (receiveItem.isValid && receiveItem.Frame.Npdu.Apdu.UnconfirmedServiceChoice == BACnetEnums.UnconfirmedService.WHO_IS);
        }

        public static bool isBBMDMessage(ReceiveItem receiveItem)
        {
            return (receiveItem.isValid && receiveItem.Frame.Header.IsBBMDMessage);
        }
    }

    /// <summary>   Communication protocol of BACnet driver. </summary>
    public class BACnetProtocol
    {
        #region const
        // Cov watch polling time (every XXX second, if any kind of message 
        //public const int COV_WATCHDOG_POLLING_TIME = 10;    // sec

        public const uint MAX_INSTANCE_NUMBER = 4194303;

        #endregion

        #region methods

        public static UFUAModel.DataType DataType(BACnetEnums.ObjectTypes BACnetObjectType, BACnetEnums.PropertyIdentifier PropertyIdentifier, ushort ArrayIndex)
        {
            if (ArrayIndex == 0)
                return UFUAModel.DataType.UInt32;
            else
                return BACnetEnums.ApplicationTagDataType[BACnetEnums.ObjectPropertyTypeDictionary[BACnetObjectType.ToString()][PropertyIdentifier]];
        }

        public static LinkType AreaLinkType(string ObjectType, BACnetEnums.PropertyIdentifier Property)
        {
            if (BACnetEnums.ObjectPropertyWritableDictionary[ObjectType][Property])
                return LinkType.InputOutput;
            else
                return LinkType.Input;
        }

        //public static void BACnetParseData(ReceivedPacket packet, Dictionary<string, BACnetStation> allowedIp, out ReceiveItem receiveItem)
        //{
        //    BACnetParseData(packet.RemoteEndPoint, packet.Data, packet.TimeStamp, allowedIp, out receiveItem);
        //}

        public static void BACnetParseData(object remoteEndPoint , byte[] packet, DateTime timeStamp, Dictionary<string, BACnetStation> allowedIp, out ReceiveItem receiveItem)
        {
            receiveItem = new ReceiveItem(new BACnetIPFrame(), new IPEndPoint(0, 0), new DateTime());

            //ReceivedPacket packet;
            //lock (lockThreadObject)
            //{
            //    ////if (ReceivePacketQueue.Count() > 0)
            //    //if (GetBytesToRead() > 0)
            //    //{
            //    //if (PacketBufferingEnabled)
            //    //{
            //        packet = GetReceivePacketQueue();

            //        //CloseAllDeviceClose();                    
            //    //}
            //    else
            //    {
            //        packet = new ReceivedPacket();
            //        //lock (lockThreadObject)
            //        //{
            //        //    packet.RemoteEndPoint = 
            //        //    packet.Data = new byte[ReceiveBuffer.Count];
            //        //    Array.Copy(ReceiveBuffer.ToArray(), packet.Data, ReceiveBuffer.Count);
            //        //    ReceiveBuffer.Clear();
            //        //}
            //    }
            //    //}
            //    //else
            //    //{
            //    //    return;
            //    //}
            //}

            if (packet == null || packet.Count() < BACnetIPFrameHeader.Size)
                return;

            receiveItem.TimeStamp = timeStamp;
            receiveItem.RemoteEndPoint = remoteEndPoint as IPEndPoint;

            int Offset = 0;
            BACnetIPFrameHeader Header = new BACnetIPFrameHeader();
            try
            {
                Header.Expand(ref packet, ref Offset);
            }
            catch (NotImplementedException notImp)
            {
                //listDebug.Add(string.Format("{0} ReceiveBuffer.Clear() 1",
                //    DateTime.Now.ToString("HH:mm:ss.fff")
                //    ));
                return;
            }
            if (Header.BVLCtype != BACnetEnums.BVLC_TYPE_BIP)
            {
                //listDebug.Add(string.Format("{0} ReceiveBuffer.Clear() 2",
                //    DateTime.Now.ToString("HH:mm:ss.fff")
                //    ));
                return;
            }
            if (Header.FrameSize > 0)
            {
                if (packet.Length < BACnetIPFrameHeader.Size + Header.FrameSize)
                    return;
                byte[] bufferTmp = new byte[BACnetIPFrameHeader.Size + Header.FrameSize];
                Array.Copy(packet, bufferTmp, BACnetIPFrameHeader.Size + Header.FrameSize);
                packet = bufferTmp;
                Offset = 0;
                try
                {
                    receiveItem.Frame.Expand(ref packet, ref Offset);
                    if (receiveItem.Frame.Header.BVLCfunction == BACnetEnums.BVLC_Functions.ForwardedNPDU)
                    {
                        // a BVLC_FORWARDED_NPDU frame by a BBMD, change the remote_address to the original one (stored in the BVLC header) 
                        // we don't care about the BBMD address
                        receiveItem.RemoteEndPoint = receiveItem.Frame.Header.OriginalEndPoint;
                    }
                }
                catch (NotImplementedException notImp)
                {
                    //listDebug.Add(string.Format("{0} ReceiveBuffer.Clear() 3",
                    //    DateTime.Now.ToString("HH:mm:ss.fff")
                    //    ));
                    return;
                }
            }

            receiveItem.isValid = true;
            //check if message is direct to one of channel's stations; broadcast (.255) always allowed
            if (receiveItem.RemoteEndPoint != null)
            {
                IPAddress ad = receiveItem.RemoteEndPoint.Address;
                byte LastAddressByte = receiveItem.RemoteEndPoint.Address.GetAddressBytes()[3];
                if (LastAddressByte != 255 && !allowedIp.ContainsKey(ad.ToString()))
                {
                    receiveItem.isUnMappedDevice = true;
#if DEBUG
                    System.Diagnostics.Debug.WriteLine(string.Format("-------------------------BACnet {0},Message with IP Address {1} don't match stations on channel {2}", DateTime.Now, ad, ""));
#endif
                }

                if (allowedIp.ContainsKey(ad.ToString()))
                {
                    BACnetStation st = allowedIp[ad.ToString()] as BACnetStation;
                    if (st != null)
                    {
                        ////discard message when station is disabled
                        //if (st.IsStateCommandVariableBit((UInt16)StationVariableBits.StationActiveCommand, true))
                        //{
                        //    receiveItem = null;
                        //    return;
                        //}

                        st.LastMessageFromDevice = DateTime.UtcNow;
                    }
                }
            }
#if DEBUG
            System.Diagnostics.Debug.WriteLine(string.Format("-------------------------BACnet {0},Func:{1},Type:0x{2},Value:{3}", DateTime.Now, receiveItem.Frame.Header.BVLCfunction.ToString(), receiveItem.Frame.Header.BVLCtype.ToString("X"), string.Join(",", packet.Select(b => b.ToString("X2")))));
#endif
        }

        public static int ParseDeviceInstanceString(string text)
        {
            int instance = -1;

            if (!string.IsNullOrEmpty(text))
            {
                try
                {
                    UInt32 numVal = Convert.ToUInt32(text);
                    if (numVal < BACnetEnums.MAX_INSTANCE)
                        instance = (int)numVal;
                }
                catch (Exception ex)
                {
                    return instance;
                }
            }

            return instance;
        }

        public static UInt32 GetNewCovSubscriberID(ref Dictionary<uint, List<BACnetCommJob>> map)
        {
            UInt32 subscriberID = GetNewCovSubscriberID();
            while (subscriberID == 0 || map.ContainsKey(subscriberID))
            {
                subscriberID = GetNewCovSubscriberID();
            }

            return subscriberID;
        }

        public static UInt32 GetNewCovSubscriberID()
        {
            Random rndGen = new Random((int)(DateTime.UtcNow.Ticks % int.MaxValue));
            UInt32 SubscriberID = (UInt32)rndGen.Next(int.MinValue, int.MaxValue);
            return SubscriberID;
        }

        public static IPAddress GetBroadcastAddress(IPAddress address)
        {
            IPAddress subnetMask = new IPAddress(ReturnSubnetmask(address));

            byte[] ipAdressBytes = address.GetAddressBytes();
            byte[] subnetMaskBytes = subnetMask.GetAddressBytes();

            if (ipAdressBytes.Length != subnetMaskBytes.Length)
                throw new ArgumentException("Lengths of IP address and subnet mask do not match.");

            byte[] broadcastAddress = new byte[ipAdressBytes.Length];
            for (int i = 0; i < broadcastAddress.Length; i++)
            {
                broadcastAddress[i] = (byte)(ipAdressBytes[i] | (subnetMaskBytes[i] ^ 255));
            }
            return new IPAddress(broadcastAddress);
        }

        static public long ReturnSubnetmask(IPAddress ipaddress)
        {
            return 0xFFFFFF;
        }

        static public uint ReturnFirtsOctet(IPAddress iPAddress)
        {
            byte[] byteIP = iPAddress.GetAddressBytes();
            uint ipInUint = (uint)byteIP[0];
            return ipInUint;
        }

        /// <summary>
        /// Get replacement value for null value from .config file
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public static byte[] GetPriorityArrayNullValue(Tag tag)
        {
            byte[] result = null;

            try
            {
                switch ((uint)tag.TagNode.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        result = BitConverter.GetBytes(Convert.ToBoolean(Properties.Settings.Default.PriorityArrayNullValue));
                        break;
                    case (uint)BuiltInType.Byte:
                    case (uint)BuiltInType.SByte:
                        byte b = Convert.ToByte(Properties.Settings.Default.PriorityArrayNullValue);
                        result = new byte[1] { b };
                        break;
                    case (uint)BuiltInType.Int16:
                        result = BitConverter.GetBytes(Convert.ToInt16(Properties.Settings.Default.PriorityArrayNullValue));
                        break;
                    case (uint)BuiltInType.UInt16:
                        result = BitConverter.GetBytes(Convert.ToUInt16(Properties.Settings.Default.PriorityArrayNullValue));
                        break;
                    case (uint)BuiltInType.Int32:
                        result = BitConverter.GetBytes(Convert.ToInt32(Properties.Settings.Default.PriorityArrayNullValue));
                        break;
                    case (uint)BuiltInType.UInt32:
                        result = BitConverter.GetBytes(Convert.ToUInt32(Properties.Settings.Default.PriorityArrayNullValue));
                        break;
                    case (uint)BuiltInType.Int64:
                        result = BitConverter.GetBytes(Convert.ToInt64(Properties.Settings.Default.PriorityArrayNullValue));
                        break;
                    case (uint)BuiltInType.UInt64:
                        result = BitConverter.GetBytes(Convert.ToUInt64(Properties.Settings.Default.PriorityArrayNullValue));
                        break;
                    case (uint)BuiltInType.Float:
                        result = result = BitConverter.GetBytes(Convert.ToSingle(Properties.Settings.Default.PriorityArrayNullValue));
                        break;
                    case (uint)BuiltInType.Double:
                        result = result = BitConverter.GetBytes(Convert.ToDouble(Properties.Settings.Default.PriorityArrayNullValue));
                        break;
                    case (uint)BuiltInType.String:
                        result = Encoding.UTF8.GetBytes(Properties.Settings.Default.PriorityArrayNullValue.ToString());
                        break;
                }
            }
            catch (Exception ex)
            {
                // do nothing --> null value result will be managed outsitde method
            }

            return result;
        }

        public static byte[] ConvertPriorityArrayValueIntoTagFormat(ServiceTag priorityArrayElement, Tag tag)
        {
            byte[] result = null;

            try
            {
                switch ((uint)tag.TagNode.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        switch (priorityArrayElement.Type)
                        {
                            case BACnetEnums.APPLICATION_TAG.REAL:
                                result = new byte[1] { (byte)(priorityArrayElement.doubleValue == 0 ? 0 : 1) };
                                break;
                            case BACnetEnums.APPLICATION_TAG.ENUMERATED:
                            case BACnetEnums.APPLICATION_TAG.UNSIGNED_INT:
                                result = new byte[1] { (byte)(priorityArrayElement.UInt == 0 ? 0 : 1) };
                                break;
                        }
                        break;
                    case (uint)BuiltInType.Byte:
                    case (uint)BuiltInType.SByte:                    
                        switch (priorityArrayElement.Type)
                        {
                            case BACnetEnums.APPLICATION_TAG.REAL:
                                result = new byte[1] { (byte)priorityArrayElement.doubleValue };
                                break;
                            case BACnetEnums.APPLICATION_TAG.ENUMERATED:
                            case BACnetEnums.APPLICATION_TAG.UNSIGNED_INT:
                                result = new byte[1] { (byte)priorityArrayElement.UInt };
                                break;
                        }
                        break;
                    case (uint)BuiltInType.Int16:
                        switch (priorityArrayElement.Type)
                        {
                            case BACnetEnums.APPLICATION_TAG.REAL:
                                result = BitConverter.GetBytes((Int16)priorityArrayElement.doubleValue);
                                break;
                            case BACnetEnums.APPLICATION_TAG.ENUMERATED:
                            case BACnetEnums.APPLICATION_TAG.UNSIGNED_INT:
                                result = BitConverter.GetBytes((Int16)priorityArrayElement.UInt);
                                break;
                        }
                        break;
                    case (uint)BuiltInType.UInt16:
                        switch (priorityArrayElement.Type)
                        {
                            case BACnetEnums.APPLICATION_TAG.REAL:
                                result = BitConverter.GetBytes((UInt16)priorityArrayElement.doubleValue);
                                break;
                            case BACnetEnums.APPLICATION_TAG.ENUMERATED:
                            case BACnetEnums.APPLICATION_TAG.UNSIGNED_INT:
                                result = BitConverter.GetBytes((UInt16)priorityArrayElement.UInt);
                                break;
                        }
                        break;
                    case (uint)BuiltInType.Int32:
                        switch (priorityArrayElement.Type)
                        {
                            case BACnetEnums.APPLICATION_TAG.REAL:
                                result = BitConverter.GetBytes((Int32)priorityArrayElement.doubleValue);
                                break;
                            case BACnetEnums.APPLICATION_TAG.ENUMERATED:
                            case BACnetEnums.APPLICATION_TAG.UNSIGNED_INT:
                                result = BitConverter.GetBytes((Int32)(priorityArrayElement.UInt));
                                break;
                        }
                        break;
                    case (uint)BuiltInType.UInt32:
                        switch (priorityArrayElement.Type)
                        {
                            case BACnetEnums.APPLICATION_TAG.REAL:
                                result = BitConverter.GetBytes((UInt32)priorityArrayElement.doubleValue);
                                break;
                            case BACnetEnums.APPLICATION_TAG.ENUMERATED:
                            case BACnetEnums.APPLICATION_TAG.UNSIGNED_INT:
                                result = BitConverter.GetBytes(priorityArrayElement.UInt);
                                break;
                        }
                        break;
                    case (uint)BuiltInType.Int64:
                        switch (priorityArrayElement.Type)
                        {
                            case BACnetEnums.APPLICATION_TAG.REAL:
                                result = BitConverter.GetBytes((Int64)priorityArrayElement.doubleValue);
                                break;
                            case BACnetEnums.APPLICATION_TAG.ENUMERATED:
                            case BACnetEnums.APPLICATION_TAG.UNSIGNED_INT:
                                result = BitConverter.GetBytes((Int64)priorityArrayElement.UInt);
                                break;
                        }
                        break;
                    case (uint)BuiltInType.UInt64:
                        switch (priorityArrayElement.Type)
                        {
                            case BACnetEnums.APPLICATION_TAG.REAL:
                                result = BitConverter.GetBytes((UInt64)priorityArrayElement.doubleValue);
                                break;
                            case BACnetEnums.APPLICATION_TAG.ENUMERATED:
                            case BACnetEnums.APPLICATION_TAG.UNSIGNED_INT:
                                result = BitConverter.GetBytes((UInt64)priorityArrayElement.UInt);
                                break;
                        }
                        break;                        
                    case (uint)BuiltInType.Float:
                        switch (priorityArrayElement.Type)
                        {
                            case BACnetEnums.APPLICATION_TAG.REAL:
                                result = BitConverter.GetBytes((Single)priorityArrayElement.doubleValue);
                                break;
                            case BACnetEnums.APPLICATION_TAG.ENUMERATED:
                            case BACnetEnums.APPLICATION_TAG.UNSIGNED_INT:
                                result = BitConverter.GetBytes((Single)priorityArrayElement.UInt);
                                break;
                        }
                        break;
                    case (uint)BuiltInType.Double:
                        switch (priorityArrayElement.Type)
                        {
                            case BACnetEnums.APPLICATION_TAG.REAL:
                                result = BitConverter.GetBytes(priorityArrayElement.doubleValue);
                                break;
                            case BACnetEnums.APPLICATION_TAG.ENUMERATED:
                            case BACnetEnums.APPLICATION_TAG.UNSIGNED_INT:
                                result = BitConverter.GetBytes((Double)priorityArrayElement.UInt);
                                break;
                        }
                        break;
                        break;
                }
            }
            catch (Exception ex)
            {
                // do nithing
            }

            return result;
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
        public static bool ParseData(byte[] receivedbuffer, ref BACnetCommJob job, ref List<object> items)
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

            byte[] tempBuffer = new byte[job.TotalJobSize];

            if (job.isProtocolBool() &&
                (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || job.ElementNumber > 0))
            {
                ushort bitIndex = 0;
                for (ushort Index = 0; Index < recivedData; Index++, bitIndex++)
                {
                    tempBuffer[Index] = (byte)((receivedbuffer[bitIndex / 8] >> (bitIndex % 8)) & 1);
                }
            }
            else if (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.String)
                tempBuffer = receivedbuffer;
            else
                Array.Copy(receivedbuffer, tempBuffer, recivedData);

            job.SetJobData(tempBuffer, ref changed);
            items.AddRange(changed);

            return true;
        }
        #endregion
    }
 }
