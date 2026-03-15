using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using System.Runtime.InteropServices;
using Opc.Ua;
using System.Text.RegularExpressions;
using DriverCodeBaseEx.Enumerators;

namespace OmronEthernetIP
{
    #region enums

    public enum OmronEthernetIPErrorCodes : int
    {
        ErrorWrongTransaction = 1000,
        ErrorRepTooShort,
        ErrorRepTooLong,
        ErrorRepTagSize,
        ErrorNoRep,
        ErrorRepDataType,
        ErrorWrongMRServiceReplyCode,
        ErrorMRStatus,
        ErrorWrongPCCCRespCode,
        ErrorRepSerNum,
        ErrorRepSerCode,
        ErrorRepStatus,
        ErrorStatusNotZero,
        ErrorEXTSTSCodeExt,
        ErrorUnknSTSEXTCode,
        ErrorUnknSTSCode,
        ErrorOutOfSync,
        ErrorEncapStatus1,
        ErrorEncapStatus2,
        ErrorEncapStatus3,
        ErrorEncapStatus64,
        ErrorEncapStatus65,
        ErrorEncapStatus69,
        ErrorUnknEncapStatus,
        ErrorInvalidReplyFragmentedProtocol,
        ErrorCipAddStatus = 0x00010000,
        ErrorCipAddStatus102 = 0x00010102,
        ErrorCipAddStatus104 = 0x00010104,
        ErrorCipAddStatus1103 = 0x00011103,
        ErrorCipAddStatus2103 = 0x00012103,
        ErrorCipAddStatus2104 = 0x00012104,
        ErrorCipAddStatus8001 = 0x00018001,
        ErrorCipAddStatus8007 = 0x00018007,
        ErrorCipAddStatus8009 = 0x00018009,
        ErrorCipAddStatus800F = 0x0001800F,
        ErrorCipAddStatus8010 = 0x00018010,
        ErrorCipAddStatus8011 = 0x00018011,
        ErrorCipAddStatus8017 = 0x00018017,
        ErrorCipAddStatus8018 = 0x00018018,
        ErrorCipAddStatus8021 = 0x00018021,
        ErrorCipAddStatus8022 = 0x00018022,
        ErrorCipAddStatus8023 = 0x00018023,
        ErrorCipAddStatus8024 = 0x00018024,
        ErrorCipAddStatus8025 = 0x00018025,
        ErrorCipAddStatus8027 = 0x00018027,
        ErrorCipAddStatus8028 = 0x00018028,
        ErrorCipAddStatus8029 = 0x00018029,
        ErrorCipAddStatus8031 = 0x00018031,
        ErrorCipStatus = 0x10000000,
        ErrorCipStatus02 = 0x10200000,
        ErrorCipStatus04 = 0x10400000,
        ErrorCipStatus05 = 0x10500000,
        ErrorCipStatus0C = 0x10C00000,
        ErrorCipStatus11 = 0x11100000,
        ErrorCipStatus13 = 0x11300000,
        ErrorCipStatus15 = 0x11500000,
        ErrorCipStatus1F = 0x11F00000,
        ErrorCipStatus20 = 0x12000000
    }

    public enum DataFormats
    {
        INVALID,
        BIT,
        BYTE,
        WORD,
        DWORD,
        SBYTE,
        SWORD,
        SDWORD,
        FLOAT,
        DOUBLE,
        LWORD,
        LINT,
        ULINT
    }

    public enum TagFormats
    {
        BOOL,
        SINT,
        USINT,
        INT,
        UINT,
        DINT,
        UDINT,
        REAL,
        LREAL,
        STRING,
        STRUCTURE,
        BYTE,
        WORD,
        DWORD,
        LWORD,
        LINT,
        ULINT

    }

    public enum FileTypes
    {
        Invalid = -1,
        Output = 0,
        Input,
        Status,
        Binary,
        Timer,
        Counter,
        Control,
        Integer,
        Float,
    }

    public enum SubElements
    { 
        Invalid = -1,
        ACC = 0,
        PRE,
    }

    public enum CommandTypes
    {
        ReadCmd,
        WriteCmd,
        TestCmd,
        FwdOpen, 
        FwdClose,
        ReadDataFormatCmd,
        LargeFwdOpen,
        Verified,
        Invalid,
    }

    public enum TagTypes
    {
        Atomic,
        ArrayStandard,
        Structure,
        ArrayOfStructures,
    };

    public enum PlcTypes
    { NJ, NX, Other_PLC }

    public enum FragmentType
    {
        First,
        Middle,
        Last
    }

    public enum FragmentedProtocolSupported
    {
        NotYetVerified = -1,
        UnSupported = 0,
        Supported = 1
    }
    #endregion

    #region dataTypes

    [StructLayout(LayoutKind.Explicit)]
    public struct ushortUnion
    {
        [FieldOffset(0)]
        public ushort USHORT;

        [FieldOffset(0)]
        public byte LOBYTE;
        [FieldOffset(1)]
        public byte HIBYTE;
   // Constructor:
        public ushortUnion(ushort USHORT)
        {
            this.LOBYTE = 0;
            this.HIBYTE = 0;
            this.USHORT = USHORT;
        }
        public ushortUnion(byte LOBYTE, byte HIBYTE)
        {
            this.USHORT = 0;
            this.LOBYTE = LOBYTE;
            this.HIBYTE = HIBYTE;
        }
        public ushortUnion(byte[] buffer, ushort index)
        {
            this.USHORT = 0;
            this.LOBYTE = buffer[index];
            this.HIBYTE = buffer[index + 1];
        }
        public ushortUnion(List<byte> buffer, ushort index)
        {
            this.USHORT = 0;
            this.LOBYTE = buffer[index];
            this.HIBYTE = buffer[index + 1];
        }

    }

    [StructLayout(LayoutKind.Explicit)]
    public struct uintUnion
    {
        [FieldOffset(0)]
        public uint UINT;

        [FieldOffset(0)]
        public ushortUnion LOUSHORT;
        [FieldOffset(2)]
        public ushortUnion HIUSHORT;

    // Constructor:
        public uintUnion(uint UINT)
        {
            this.LOUSHORT = new ushortUnion(0x0000);
            this.HIUSHORT = new ushortUnion(0x0000);
            this.UINT = UINT;
        }
        public uintUnion(byte LOBYTE_LW, byte HIBYTE_LW, byte LOBYTE_HW, byte HIBYTE_HW)
        {
            this.UINT = 0;
            this.LOUSHORT = new ushortUnion(LOBYTE_LW, HIBYTE_LW);
            this.HIUSHORT = new ushortUnion(LOBYTE_HW, HIBYTE_HW);
        }
        public uintUnion(byte[] buffer, ushort index)
        {
            this.UINT = 0;
            this.LOUSHORT = new ushortUnion(buffer, index);
            this.HIUSHORT = new ushortUnion(buffer, (ushort)(index + 2));
        }
        public uintUnion(List<byte> buffer, ushort index)
        {
            this.UINT = 0;
            this.LOUSHORT = new ushortUnion(buffer,index);
            this.HIUSHORT = new ushortUnion(buffer, (ushort)(index + 2));
        }

   }

    #endregion

    #region OmronEthernetIPFragmentInfo class
    public class OmronEthernetIPFragmentInfo
    {
        public OmronEthernetIPFragmentInfo()
        {
            _bufferToBeFragmented = null;
            _FType = FragmentType.First;
            _Handle = 0;
            _SequenceNumber = 0;
            _TotalNumberOfVariables = 0;
            _LengthOfTheBufferToBeFragmented = 0;
            _MaxLengthOfASingleFragment = 0;
        }

        public void SetBufferToBeFragmented(byte[] dataBuffer, ushort bufferLength)
        {
            if ((dataBuffer == null) || (bufferLength == 0) || (dataBuffer.Count() < (int)bufferLength))
            {
                return;
            }
            _LengthOfTheBufferToBeFragmented = (ushort)bufferLength;
            _bufferToBeFragmented = new byte[_LengthOfTheBufferToBeFragmented];
            Array.Copy(dataBuffer, 0, _bufferToBeFragmented, 0, _LengthOfTheBufferToBeFragmented);
        }

        public ushort GetNextFragmentBuffer(ref byte[] nextFragmentBuffer, ref bool isTheLastFragment)
        {
            nextFragmentBuffer = null;
            isTheLastFragment = false;
            if ((_bufferToBeFragmented == null) || (_LengthOfTheBufferToBeFragmented < 1) || (_MaxLengthOfASingleFragment < 1) || (_LengthOfTheBufferToBeFragmented < (_MaxLengthOfASingleFragment * _SequenceNumber)))
            {
                isTheLastFragment = true;
                return (0);
            }

            // Calculate the length of the next fragment and check if it is the last one
            int nextFragmentLength = _MaxLengthOfASingleFragment;
            int remainingFragmentLength = (ushort)(_LengthOfTheBufferToBeFragmented - (_MaxLengthOfASingleFragment * _SequenceNumber));
            if (remainingFragmentLength < 1)
            {
                isTheLastFragment = true;
                return (0);
            }
            if (remainingFragmentLength <= nextFragmentLength)
            {
                nextFragmentLength = remainingFragmentLength;
                isTheLastFragment = true;
            }

            // Copy the next fragment into the buffer
            nextFragmentBuffer = new byte[nextFragmentLength];
            Array.Copy(_bufferToBeFragmented, _MaxLengthOfASingleFragment * _SequenceNumber, nextFragmentBuffer, 0, nextFragmentLength);

            return ((ushort)nextFragmentLength);
        }

        // The sequence ID of the request is a combination of the fragment type and the sequence number
        public ushort GetRequestSequenceID()
        {
            ushort fragmentFlag = (ushort)_FType;
            ushort fragmentCounter = (ushort)(_SequenceNumber & OmronEthernetIPProtocol.FRAGMENT_MAX_SEQUENCE_NUMBER);
            ushort sequenceID = (ushort)(fragmentFlag << 14);
            sequenceID += fragmentCounter;
            return (sequenceID);
        }

        // Check the sequence ID of the PLC reply
        public bool CheckReplySequenceID(ushort replySequenceID)
        {
            ushort replyFragmentCounter = (ushort)(replySequenceID & OmronEthernetIPProtocol.FRAGMENT_MAX_SEQUENCE_NUMBER);
            if (replyFragmentCounter == _SequenceNumber)
            {
                return (true);
            }
            else
            {
                return (false);
            }
        }

        private byte[] _bufferToBeFragmented;

        private FragmentType _FType;
        public FragmentType FType
        {
            get { return _FType; }
            set { _FType = value; }
        }

        // Returned by the PLC in the reply to the first fragment
        private ushort _Handle;
        public ushort Handle
        {
            get { return _Handle; }
            set { _Handle = value; }
        }

        private ushort _SequenceNumber;
        public ushort SequenceNumber
        {
            get { return _SequenceNumber; }
            set { _SequenceNumber = value; }
        }

        private ushort _TotalNumberOfVariables;
        public ushort TotalNumberOfVariables
        {
            get { return _TotalNumberOfVariables; }
            set { _TotalNumberOfVariables = value; }
        }

        private ushort _LengthOfTheBufferToBeFragmented;
        public ushort LengthOfTheBufferToBeFragmented
        {
            get { return _LengthOfTheBufferToBeFragmented; }
            set { _LengthOfTheBufferToBeFragmented = value; }
        }

        private ushort _MaxLengthOfASingleFragment;
        public ushort MaxLengthOfASingleFragment
        {
            get { return _MaxLengthOfASingleFragment; }
            set { _MaxLengthOfASingleFragment = value; }
        }
    }
    #endregion

    public class OmronEthernetIPProtocol
    {
        #region constants

        //public const ushort MAX_IOI_STRING_LENGTH = 200;
        public const ushort MAX_IOI_STRING_LENGTH = 400;
        public const ushort MIN_READ_REQUEST_LENGTH = 10;
        public const ushort MIN_WRITE_REQUEST_LENGTH = 13;
        public const ushort MIN_READ_REPLY_LENGTH = 9;
        public const ushort MIN_WRITE_REPLY_LENGTH = 6;
        public const ushort MAX_SEGMENT_DATA_SIZE = 224;
        //public const ushort MAX_DATA_SIZE = 4096;
        public const ushort MAX_DATA_SIZE = 9216;
        public const ushort LOGIX5550_CONNECTION_SIZE_HEADER = 54;
        public const ushort LOGIX5550_RESPONSE_SIZE_HEADER = 106;
        //public const ushort LOGIX5550_CONNECTION_SIZE = 504;
        //public const ushort LOGIX5550_CONNECTION_SIZE = 1994;
        //public const ushort TCP_MAX_SEGMENT_SIZE = 4096;
        public const ushort TCP_MAX_SEGMENT_SIZE = 9216;
        public const ushort MAX_STRING_LENGTH = 256;

        public const ushort PLCTYPE_OTHER_MAXPDUSIZE = 502;
        public const ushort PLCTYPE_NJ_MAXPDUSIZE = 1994;
        public const ushort PLCTYPE_NX_MAXPDUSIZE = 8192;

        public const ushort CIP_SEQUENCE_COUNT_SIZE = 2;

        public const byte ENCAPSULATION_HEADER_SIZE = 24;
        public const byte EDATA_LEN_OFFS = 2;       //offset for Encaps.data length
        public const byte EDATA_SESS_HND_OFFS = 4;		//offset for Encaps.data Session Handle
        public const byte EDATA_STS_OFFS = 8;		//offset for Encaps.data Status
        public const byte MR_SVC_REPLY_CODE_OFFS = 40;		//message router service reply code offset
        public const byte MR_SVC_REPLY_GENSTS_OFFS = 42;		//message router service reply general status offset
        public const byte MR_SVC_REPLY_ADDSTSSZ_OFFS = 43;		//message router service reply additional status size offset
        public const byte PCCC_RESP_CODE_OFFS = 51;		//offset for PCCC response code
        public const byte PCCC_RESP_STS_OFFS = 52;		//offset for PCCC response STS
        public const byte PCCC_RESP_TNS_OFFS = 53;		//offset for PCCC response TNS
        public const byte PCCC_RESP_DATA_OFFS = 55;		//offset for PCCC response read values

        public const byte LOGIX5550_REP_TNS_OFFS = 44;      // Offset for TPDU reply TNS
        public const byte LOGIX5550_REP_GENSTS_OFFS = 48;   // Offset for TPDU reply General Status
        public const byte LOGIX5550_REP_GENSTSSZ_OFFS = 49; // Offset for TPDU reply Additional Status Size
        public const byte LOGIX5550_REP_SERNUM_OFFS = 50;   // Offset for TPDU reply Number of Services

        public const byte ALL_ATTRIBUTES_MIN_LENGTH = 26;
        public const byte REPLY_CIP_SERVICE_OFFSET = 22;
        public const ushort OMRON_VENDOR_ID = 0x2f; // OMRON Vendor ID
        public const ushort FRAGMENT_MAXPDUSIZE = 4096;

        public const ushort FRAGMENT_PDU_OVERHEAD = 10;
        public const ushort FRAGMENT_MAX_SEQUENCE_NUMBER = 0x3fff;
        public const ushort MULTIPLE_SERVICE_REQUEST_FIX_OVERHEAD = 8;
        public const ushort MULTIPLE_SERVICE_REPLY_FIX_OVERHEAD = 6;
        public const ushort FRAGMENT_REQUEST_FIX_OVERHEAD = 16;
        public const ushort FRAGMENT_REPLY_FIX_OVERHEAD = 6;

        public static ushortUnion VENDOR_ID = new ushortUnion(0x0000);
        public static uintUnion CIP_SERIAL_NUMBER = new uintUnion(0x00000000);

        //Encaspulation commands
        public static ushortUnion ecNOP = new ushortUnion(0x0000);
        public static ushortUnion ecListServices = new ushortUnion(0x0004);
        public static ushortUnion ecListIdentity = new ushortUnion(0x0063);
        public static ushortUnion ecListInterfaces = new ushortUnion(0x0064);
        public static ushortUnion ecRegisterSession = new ushortUnion(0x0065);
        public static ushortUnion ecUnRegisterSession = new ushortUnion(0x0066);
        public static ushortUnion ecSendRRData = new ushortUnion(0x006f);
        public static ushortUnion ecSendUnitData = new ushortUnion(0x0070);
        public static ushortUnion ecIndicateStatus = new ushortUnion(0x0072);
        public static ushortUnion ecCancel = new ushortUnion(0x0073);

        
        //Class 
        public const byte clTagNameService = 0x6A;
        public const byte clVariableObject = 0x6B;
        public const byte clNone87 = 0x87;
        public const byte clNoneC4 = 0xC4;
        public const byte clVariableTypeObject = 0x6C;

        //Service Attribute
        public const byte saReadsAnAttribute = 0x0E;
        public const byte saReadsMultiAttribute = 0x5F;
        public const byte saReadsAttributeAll = 0x01;

        //Attribute Segment 
        public const byte asReadsMAttribute68 = 0x68;


        public const string TEST_COMM_DYNAMIC_SETTINGS = "OmronEthernetIP.Station=Station{0}|LinkType=1|ABA=Test|TEFRM=3";

        public enum ReadWriteLimitate
        {
            Add,
            EvaluateOnly
        }        

        public class ReadWriteListLimitateSize
        {
            public ushort TotalSize;
            public ushort ResponseSize;
            public ushort TotalNrJobs;

            public ReadWriteListLimitateSize()
            {
                TotalSize = 0;
                ResponseSize = 0;
                TotalNrJobs = 0;
            }

            public bool IsEmpty()
            {
                return (TotalSize == 0 && ResponseSize == 0);
            }
        }

        #endregion

        #region static methods

        public delegate void PrepareHeader(
            OmronEthernetIPStation s,
            ref byte[] pdu, ref ushortUnion pduPointer);

        public delegate ushort PrepareBody(
            ref List<CommJob> List,
            ref OmronEthernetIPStation s,
            ref byte[] pdu, ref ushortUnion pduPointer);

        public static void PrepareEncapsulationHeader(ushortUnion Cmd, OmronEthernetIPChannel c, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            pduPointer.USHORT = 0;

            //Command
            pdu[pduPointer.USHORT++] = Cmd.LOBYTE;
            pdu[pduPointer.USHORT++] = Cmd.HIBYTE;

            //Length (filled in by caller function)
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            //Session handle
            pdu[pduPointer.USHORT++] = c.SessionHandle.LOUSHORT.LOBYTE;
            pdu[pduPointer.USHORT++] = c.SessionHandle.LOUSHORT.HIBYTE;
            pdu[pduPointer.USHORT++] = c.SessionHandle.HIUSHORT.LOBYTE;
            pdu[pduPointer.USHORT++] = c.SessionHandle.HIUSHORT.HIBYTE;

            //Status
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            //Sender Context
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            //Options
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;


        }
        public static bool RegisterSession(ref byte[] pdu, ref ushortUnion pduPointer, OmronEthernetIPChannel c)
        {
            if (!c.SessionRegisterOk())
            {
                PrepareEncapsulationHeader(ecRegisterSession, c, ref pdu, ref pduPointer);

                //Encapsulation data for RegisterSession are just 2 UINTs, so we do it here
                pdu[pduPointer.USHORT++] = 1;		//Protocol Version lobyte, const value
                pdu[pduPointer.USHORT++] = 0;		//Protocol Version hibyte, const value
                pdu[pduPointer.USHORT++] = 0;		//Options flags lobyte, const value
                pdu[pduPointer.USHORT++] = 0;		//Options flags hibyte, const value

                c.ReceiveClear();

                if (c.OmronEthernetIPDeviceWrite(ref pdu, ref pduPointer))
                {
                    //first we read the fixed-size header
                    if (c.DeviceRead(pdu, ENCAPSULATION_HEADER_SIZE))
                    {
                        ushortUnion lenData = new ushortUnion(pdu, EDATA_LEN_OFFS);
                        ushortUnion ecCommand = new ushortUnion(pdu, 0);
                        if (ecRegisterSession.USHORT == ecCommand.USHORT)
                        {
                            uintUnion Sts = new uintUnion(pdu, EDATA_STS_OFFS);
                            if (Sts.UINT == 0)
                            {
                                c.SessionHandle = new uintUnion(pdu, EDATA_SESS_HND_OFFS);
                            }
#if DEBUG
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("DEBUG RegisterSession - Non-zero status in reply");
                            }
#endif
                        }
#if DEBUG
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("DEBUG RegisterSession - Unexpected command in reply");
                        }
#endif
                        if (0 != lenData.USHORT)
                        {
                            c.DeviceRead(pdu, lenData.USHORT) ;
                        }

                    }
#if DEBUG
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("DEBUG RegisterSession - Error in DeviceRead");
                    }
#endif
                }
#if DEBUG
                else
                {
                    System.Diagnostics.Debug.WriteLine("DEBUG RegisterSession - Error in OmronEthernetIPDeviceWrite");
                }
#endif
                c.ReceiveClear();
            }

            return (c.SessionRegisterOk());
        }

        public static void UnRegisterSession(ref byte[] pdu, ref ushortUnion pduPointer, OmronEthernetIPChannel c)
        {
            if (c.SessionRegisterOk())
            {
                PrepareEncapsulationHeader(ecUnRegisterSession, c, ref pdu, ref pduPointer);
                c.OmronEthernetIPDeviceWrite(ref pdu, ref pduPointer);
                c.ClearSession();
                System.Threading.Thread.Sleep(500);
            }

            c.ReceiveClear();
            c.DeviceClose();
        }

        public static void CloseSession(ref byte[] pdu, ref ushortUnion pduPointer, OmronEthernetIPChannel c)
        {
            c.InvalidateStationConnections();
            if (c.SessionRegisterOk())
            {
                PrepareEncapsulationHeader(ecUnRegisterSession, c, ref pdu, ref pduPointer);
                // Send the request and don't wait for the reply
                c.OmronEthernetIPDeviceWrite(ref pdu, ref pduPointer);
                c.ReceiveClear();
                c.ClearSession();
            }
        }

        public static bool GetListServices(ref byte[] pdu, ref ushortUnion pduPointer, OmronEthernetIPChannel c)
        {
            bool result = false;

            PrepareEncapsulationHeader(ecListServices, c, ref pdu, ref pduPointer);

            c.ReceiveClear();

            if (c.OmronEthernetIPDeviceWrite(ref pdu, ref pduPointer))
            {
                //first we read the fixed-size header
                if (c.DeviceRead(pdu, ENCAPSULATION_HEADER_SIZE))
                {
                    ushortUnion lenData = new ushortUnion(pdu, EDATA_LEN_OFFS);
                    ushortUnion ecCommand = new ushortUnion(pdu, 0);
                    if (ecListServices.USHORT == ecCommand.USHORT)
                    {
                        uintUnion Sts = new uintUnion(pdu, EDATA_STS_OFFS);
                        if (Sts.UINT == 0)
                        {
                            result = true;
                        }
#if DEBUG                        
                        else
                            System.Diagnostics.Debug.WriteLine("DEBUG RegisterSession - Non-zero status in reply");
#endif                            
                    }
#if DEBUG
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("DEBUG RegisterSession - Unexpected command in reply");
                    }
#endif
                    if (0 != lenData.USHORT)
                    {
                        c.DeviceRead(pdu, lenData.USHORT);
                    }

                }
#if DEBUG
                else
                {
                    System.Diagnostics.Debug.WriteLine("DEBUG RegisterSession - Error in DeviceRead");
                }
#endif
            }
#if DEBUG
            else
            {
                System.Diagnostics.Debug.WriteLine("DEBUG RegisterSession - Error in OmronEthernetIPDeviceWrite");
            }
#endif
            c.ReceiveClear();


            return (result);

        }

        public static bool Logix5550LargeForwardOpen(OmronEthernetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, OmronEthernetIPChannel c, ref bool bRetryTheConnection)
        {
            bRetryTheConnection = false;
            if (c.SessionRegisterOk())
            {
                if (!s.ConnectionIDSet)
                {
                    // Set the initial values for the connection IDs
                    s.Logix5550SetInitConnIDs();

                    // Prepare the request message
                    Logix5550PrepareUnconnectedRequest(CommandTypes.LargeFwdOpen, c, s, ref pdu, ref pduPointer);

                    c.ReceiveClear();

                    if (c.OmronEthernetIPDeviceWrite(ref pdu, ref pduPointer))
                    {
                        //first we read the fixed-size header
                        if (c.DeviceRead(pdu, OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE)) //error timeout RX
                        {
                            // Then read the variable size data part
                            ushortUnion lenData = new ushortUnion(pdu, OmronEthernetIPProtocol.EDATA_LEN_OFFS);
                            if (lenData.USHORT >= 20)
                            {
                                byte[] pduData = new byte[lenData.USHORT];
                                if (c.DeviceRead(pduData, (uint)lenData.USHORT)) //error timeout RX
                                {
                                    if ((lenData.USHORT + OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE) > (OmronEthernetIPProtocol.TCP_MAX_SEGMENT_SIZE))
                                    {
                                        c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                                                        string.Format(Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                                        Properties.Resources.ErrorBufferIsShort,
                                                                        Opc.Ua.EventSeverity.High);
                                        return (s.ConnectionIDSet);
                                    }
                                    pduData.CopyTo(pdu, OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE);
                                    // Analyze the reply header
                                    OmronEthernetIPErrorCodes ErrorAnalyzeReplyHeader = c.AnalyzeReplyHeader(ref pdu, ecSendRRData.USHORT, c.SessionHandle.UINT);
                                    if (ErrorAnalyzeReplyHeader == (OmronEthernetIPErrorCodes)DriverErrorCodes.ErrorNoError)
                                    {
                                        if (pdu[MR_SVC_REPLY_CODE_OFFS] == 0xDB)
                                        {
                                            ushortUnion Status = new ushortUnion(pdu, MR_SVC_REPLY_CODE_OFFS + 1);
                                            if (Status.USHORT == 0)
                                            {
                                                // Set the Originator > Target Connection ID
                                                s.OTNetConnID = new uintUnion(pdu, OmronEthernetIPProtocol.MR_SVC_REPLY_CODE_OFFS + 4);
                                                s.ConnectionIDSet = true;
                                            }
                                            else
                                            {
                                                bRetryTheConnection = AnalyzeReplyError(ref pdu, c, s);
                                            }
                                        }
                                        else
                                        {
                                            c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                                                            string.Format(Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                                            Properties.Resources.ErrorUnexpectedReply,
                                                                            Opc.Ua.EventSeverity.High);
                                        }
                                    }
                                    else
                                    {
                                        c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                                                        string.Format(Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                                        string.Format(Properties.Resources.ErrorAnalyzeReplyHeader, ErrorAnalyzeReplyHeader),
                                                                        Opc.Ua.EventSeverity.High);
                                    }
                                }
                                else
                                {
                                    c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                            string.Format(Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                          Properties.Resources.ErrorEXTSTS23,
                                            Opc.Ua.EventSeverity.High);
                                }
                            }
                        }
                        else
                        {
                            c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                        string.Format(Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                      Properties.Resources.ErrorEXTSTS23,
                                        Opc.Ua.EventSeverity.High);
                        }
                    }
                    else
                    {
                        c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                                        string.Format(Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                        Properties.Resources.ErrorDeviceWriteKo,
                                        Opc.Ua.EventSeverity.High);
                    }
                    c.ReceiveClear();
                }
            }
            else
            {
                c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                                string.Format(Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                Properties.Resources.ErrorSessionRegisterKo,
                                       Opc.Ua.EventSeverity.High);
                s.ConnectionIDSet = false;
            }
            return (s.ConnectionIDSet);
        }

        public static bool  AnalyzeReplyError(ref byte[] pdu, OmronEthernetIPChannel c, OmronEthernetIPStation s)
        {
            bool bRetryTheConnection = false;
            //Connection failure
            if (pdu[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS] == 0x01)
            {
                //Invalid connection size
                if ( pdu[44] == 0x09 && pdu[45] == 0x01)
                {
                    bRetryTheConnection = true;
                }
                else
                {
                    c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                                        string.Format(Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                        string.Format(Properties.Resources.ErrorForwardLOpen_CodeExtended, 
                                                                  pdu[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS], pdu[45], pdu[44]),
                                                    Opc.Ua.EventSeverity.High);
                }
            }
            // Service not supported
            else if (pdu[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS] == 0x08)
            {
                bRetryTheConnection = true;
            }
            else
            {
                c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                                string.Format(Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                string.Format(Properties.Resources.ErrorForwardOpen_Code, pdu[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS]),
                                                Opc.Ua.EventSeverity.High);
            }
            return (bRetryTheConnection);
        }

        public static bool Logix5550ForwardOpen(OmronEthernetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, OmronEthernetIPChannel c)
        {
            if (c.SessionRegisterOk())
            {
                if (!s.ConnectionIDSet)
                {                  
                    // Set the initial values for the connection IDs
                    s.Logix5550SetInitConnIDs();

                    // Prepare the request message
                    Logix5550PrepareUnconnectedRequest(CommandTypes.FwdOpen, c, s, ref pdu, ref pduPointer);

                    c.ReceiveClear();

                    if (c.OmronEthernetIPDeviceWrite(ref pdu, ref pduPointer))
                    {
                        //first we read the fixed-size header
                        if (c.DeviceRead(pdu, OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE)) //error timeout RX
                        {
                            // Then read the variable size data part
                            ushortUnion lenData = new ushortUnion(pdu, OmronEthernetIPProtocol.EDATA_LEN_OFFS);
                            if (lenData.USHORT >= 20)
                            {
                                byte[] pduData = new byte[lenData.USHORT];
                                if (c.DeviceRead(pduData, (uint)lenData.USHORT)) //error timeout RX
                                {
                                    pduData.CopyTo(pdu, OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE);
                                    // Analyze the reply header
                                    if (c.AnalyzeReplyHeader(ref pdu, ecSendRRData.USHORT, c.SessionHandle.UINT) ==
                                        (OmronEthernetIPErrorCodes)DriverErrorCodes.ErrorNoError)
                                    {
                                        if (pdu[MR_SVC_REPLY_CODE_OFFS] == 0xD4)
                                        {
                                            ushortUnion Status = new ushortUnion(pdu, MR_SVC_REPLY_CODE_OFFS + 1);
                                            if (Status.USHORT == 0)
                                            {
                                                // Set the Originator > Target Connection ID
                                                s.OTNetConnID = new uintUnion(pdu, OmronEthernetIPProtocol.MR_SVC_REPLY_CODE_OFFS + 4);
                                                s.ConnectionIDSet = true;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    c.ReceiveClear();
                }
            }
            else
            {
                s.ConnectionIDSet = false;
            }
            return (s.ConnectionIDSet);
        }

        public static bool Logix5550ForwardClose(OmronEthernetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, OmronEthernetIPChannel c)
        {
            // Connection is open?
            //if (s.SessionHandle.UINT == 0 || !s.ConnectionIDSet)
            //{
            //        return true;
            //}

            // Prepare the request message
            Logix5550PrepareUnconnectedRequest(CommandTypes.FwdClose, c, s, ref pdu, ref pduPointer);

            c.ReceiveClear();

            // Send the request
            if (c.OmronEthernetIPDeviceWrite(ref pdu, ref pduPointer))
            {
                if (c.DeviceRead(pdu, OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE))
                {
                    // Read the variable size data part
                    ushortUnion DataLen = new ushortUnion(pdu, EDATA_LEN_OFFS);
                    if (DataLen.USHORT > 0)
                    {
                        // Read the variable size data part
                        byte[] pduData = new byte[DataLen.USHORT];
                        if (c.DeviceRead(pduData, (uint)DataLen.USHORT))
                        {
                            // Do not check the reply
                            s.ConnectionIDSet = false;
                        }
                    }
                }
            }
            c.ReceiveClear();
            if (s.ConnectionIDSet)
            {
                s.ConnectionIDSet = false;
            }

            return !s.ConnectionIDSet;
        }

        private static bool CheckEncapsulationHeader(ref byte[] pdu, ushort expectedReplyCommand)
        {
            ushortUnion lenData = new ushortUnion(pdu, EDATA_LEN_OFFS);
            ushortUnion ecCommand = new ushortUnion(pdu, 0);
            uintUnion status = new uintUnion(pdu, EDATA_STS_OFFS);
            if (ecCommand.USHORT != expectedReplyCommand)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckEncapsulationHeader - Wrong reply command: {1}",
                                                       currentTime, ecCommand.USHORT));
                }
#endif
                return (false);
            }

            // Check the status
            if (status.UINT != 0)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckEncapsulationHeader - Status: {1}",
                                                       currentTime, status.UINT));
                }
#endif
                return (false);
            }

            // Check the length
            if (lenData.USHORT == 0)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckEncapsulationHeader - Reply without data",
                                                       currentTime));
                }
#endif
                return (false);
            }

            return (true);
        }

        public static bool GetDeviceIdentity(ref byte[] pdu, ref ushortUnion pduIndex, OmronEthernetIPChannel c, OmronEthernetIPStation s, ref ushort vendorID, ref string productName)
        {
            // Prepare the request message
            PrepareGetDeviceIdentityRequest(ref pdu, ref pduIndex, c, s);

            // Empty the channel receive buffer
            c.ReceiveClear();

            // Send the request
            if (!c.OmronEthernetIPDeviceWrite(ref pdu, ref pduIndex))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetDeviceIdentity - OmronEthernetIPDeviceWrite failed",
                                                       currentTime));
                }
#endif
                return (false);
            }

            // Read the fixed-size header
            if (!c.DeviceRead(pdu, ENCAPSULATION_HEADER_SIZE))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetDeviceIdentity - DeviceRead Encapsulation Header failed",
                                                       currentTime));
                }
#endif
                c.ReceiveClear();
                return (false);
            }

            // Validate the reply header
            if (!CheckEncapsulationHeader(ref pdu, ecSendUnitData.USHORT))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetDeviceIdentity - Error in CheckEncapsulationHeader",
                                                       currentTime));
                }
#endif
                c.ReceiveClear();
                return (false);
            }

            // Read the remaining part of the reply
            ushortUnion lenData = new ushortUnion(pdu, EDATA_LEN_OFFS);
            if (!c.DeviceRead(pdu, lenData.USHORT))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetDeviceIdentity - DeviceRead CIP reply failed",
                                                       currentTime));
                }
#endif
                c.ReceiveClear();
                return (false);
            }

            // Parse the identity attibutes
            if (!ParseDeviceIdentityAttributes(ref pdu, lenData.USHORT, ref vendorID, ref productName, s))
            {
                c.ReceiveClear();
                return (false);
            }

            c.ReceiveClear();
            return (true);
        }
        private static void PrepareGetDeviceIdentityRequest(ref byte[] pdu, ref ushortUnion pduIndex, OmronEthernetIPChannel c, OmronEthernetIPStation s)
        {

            // Ethernet/IP header
            PrepareEncapsulationHeader(ecSendUnitData, c, ref pdu, ref pduIndex);

            // Interface handle: CIP
            pdu[pduIndex.USHORT++] = 0;
            pdu[pduIndex.USHORT++] = 0;
            pdu[pduIndex.USHORT++] = 0;
            pdu[pduIndex.USHORT++] = 0;

            // Timeout
            pdu[pduIndex.USHORT++] = 0;
            pdu[pduIndex.USHORT++] = 0;

            // Item Count
            pdu[pduIndex.USHORT++] = 2;
            pdu[pduIndex.USHORT++] = 0;

            // Connected Address Item
            pdu[pduIndex.USHORT++] = 0xa1;
            pdu[pduIndex.USHORT++] = 0;

            // Length: 4
            pdu[pduIndex.USHORT++] = 4;
            pdu[pduIndex.USHORT++] = 0;

            // Address Item == Connection ID
            pdu[pduIndex.USHORT++] = s.OTNetConnID.LOUSHORT.LOBYTE;
            pdu[pduIndex.USHORT++] = s.OTNetConnID.LOUSHORT.HIBYTE;
            pdu[pduIndex.USHORT++] = s.OTNetConnID.HIUSHORT.LOBYTE;
            pdu[pduIndex.USHORT++] = s.OTNetConnID.HIUSHORT.HIBYTE;

            // Connected Data Item
            pdu[pduIndex.USHORT++] = 0xb1;
            pdu[pduIndex.USHORT++] = 0;

            // Length: 8
            pdu[pduIndex.USHORT++] = 8;
            pdu[pduIndex.USHORT++] = 0;

            // Sequence count
            pdu[pduIndex.USHORT++] = s.GetTransaction().LOBYTE;
            pdu[pduIndex.USHORT++] = s.GetTransaction().HIBYTE;

            // Service: Get Attributes All
            pdu[pduIndex.USHORT++] = 1;

            // Path Size: 2 words
            pdu[pduIndex.USHORT++] = 2;

            // Request Path: Identity, Instance 1
            pdu[pduIndex.USHORT++] = 0x20; // Class Segment
            pdu[pduIndex.USHORT++] = 1; // Identity
            pdu[pduIndex.USHORT++] = 0x24; // Instance Segment
            pdu[pduIndex.USHORT++] = 1; // Instance ID
        }

        private static bool ParseDeviceIdentityAttributes(ref byte[] pdu, ushort pduLength, ref ushort vendorID, ref string productName, OmronEthernetIPStation s)
        {
            // Initialize the output arguments
            vendorID = 0;
            productName = String.Empty;

            // Check the length
            if (pduLength < ALL_ATTRIBUTES_MIN_LENGTH)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ParseDeviceIdentityAttributes - message too short",
                                                       currentTime));
                }
#endif
                return (false);
            }

            // Check the sequence count
            ushortUnion receivedCipSequenceCount = new ushortUnion(pdu, REPLY_CIP_SERVICE_OFFSET - 2);
            if (!s.CheckTransaction(receivedCipSequenceCount.USHORT))
            {
                s.LastTransOK = false;
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ParseDeviceIdentityAttributes - Wrong CIP sequence count:{1}",
                                                       currentTime, receivedCipSequenceCount.USHORT));
                }
#endif
                return (false);
            }
            s.LastTransOK = true;

            // Check the service ID
            if (pdu[REPLY_CIP_SERVICE_OFFSET] != 0x81)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ParseDeviceIdentityAttributes - Wrong reply service ID",
                                                       currentTime));
                }
#endif
                return (false);
            }

            // Check the status
            if (pdu[REPLY_CIP_SERVICE_OFFSET + 2] != 0)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ParseDeviceIdentityAttributes - General status different from success: {1}",
                                                       currentTime, pdu[REPLY_CIP_SERVICE_OFFSET + 2]));
                }
#endif
                return (false);
            }

            // Second check of the length
            if (pduLength < (ALL_ATTRIBUTES_MIN_LENGTH + 16))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ParseDeviceIdentityAttributes - message too short (2nd check)",
                                                       currentTime));
                }
#endif
                return (false);
            }

            // Parse the Vendor ID
            ushortUnion vID = new ushortUnion(pdu, REPLY_CIP_SERVICE_OFFSET + 4);
            vendorID = vID.USHORT;

            // Parse the Product Name
            byte nameLength = pdu[ALL_ATTRIBUTES_MIN_LENGTH + 14];
            if ((nameLength > 0) && (pduLength >= (ALL_ATTRIBUTES_MIN_LENGTH + 15 + nameLength)))
            {
                UTF8Encoding enc = new UTF8Encoding();
                byte[] buffer = new byte[nameLength];
                try
                {
                    Array.Copy(pdu, ALL_ATTRIBUTES_MIN_LENGTH + 15, buffer, 0, nameLength);
                    productName = enc.GetString(buffer);
                }
                catch (Exception ex)
                {
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ParseDeviceIdentityAttributes - Exception trying to parse the product name: {1}",
                                                           currentTime, ex.Message));
                    }
#endif
                    productName = String.Empty;
                    return (false);
                }
            }

            return (true);
        }

        public static bool RequestOnTheDevive(ref byte[] pdu, ref ushortUnion pduIndex, OmronEthernetIPChannel c, OmronEthernetIPStation s, ref OmronEthernetIPChannel.RequestOfDevice request)
        {
            // Prepare the request message
            PrepareRequest(ref pdu, ref pduIndex, c, s, ref  request);

            // Empty the channel receive buffer
            c.ReceiveClear();
            c.InitializeNewDataEvent();

            // Send the request
            if (!c.OmronEthernetIPDeviceWrite(ref pdu, ref pduIndex))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetDeviceIdentity - OmronEthernetIPDeviceWrite failed",
                                                       currentTime));
                }
#endif
                request.Error = String.Format(OmronEthernetIP.Properties.Resources.ErrorImportSendRequest, request.Request.ToString());
                return (false);
            }
            

            //Read the fixed-size header
            if (!c.BeginDeviceRead(ENCAPSULATION_HEADER_SIZE))
            {
                request.Error = String.Format(OmronEthernetIP.Properties.Resources.ErrorImportBeginDeviceRead, request.Request.ToString());
                c.ReceiveClear();
                return false;
            }
            DriverErrorCodes conn = DriverErrorCodes.ErrorNoError;
            c.WaitNewDataEvent(ref conn);
            c.ResetNewDataEvent();
            if (conn != DriverErrorCodes.ErrorNoError)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetDeviceIdentity - DeviceRead Encapsulation Header failed",
                                                       currentTime));
                }
#endif
                request.Error = String.Format(OmronEthernetIP.Properties.Resources.ErrorImportTimeOutAnswerHeader, request.Request.ToString());
                c.ReceiveClear();
                return (false);
            }            
            byte[] d = c.GetReceiveBuffer();
            Array.Copy(d, 0, pdu, 0, d.Length);
           
            // Validate the reply header
            if (!CheckEncapsulationHeader(ref pdu, ecSendUnitData.USHORT))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetDeviceIdentity - Error in CheckEncapsulationHeader",
                                                       currentTime));
                }
#endif
                request.Error = String.Format(OmronEthernetIP.Properties.Resources.ErrorImportCheckHeader, request.Request.ToString());
                c.ReceiveClear();
                return (false);
            }

            // Read the remaining part of the reply
            ushortUnion lenData = new ushortUnion(pdu, EDATA_LEN_OFFS);

            if (!c.BeginDeviceRead(lenData.USHORT))
            {
                request.Error = String.Format(OmronEthernetIP.Properties.Resources.ErrorImportBeginDeviceReadDate, request.Request.ToString());
                c.ReceiveClear();
                return false;
            }

            conn = DriverErrorCodes.ErrorNoError;
            c.WaitNewDataEvent(ref conn);
            c.ResetNewDataEvent();
            if (conn != DriverErrorCodes.ErrorNoError)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetDeviceIdentity - DeviceRead CIP reply failed",
                                                       currentTime));
                }
#endif
                request.Error = String.Format( OmronEthernetIP.Properties.Resources.ErrorImportTimeOutAnswerDate, request.Request.ToString());
                c.ReceiveClear();
                return (false);
            }

            d = c.GetReceiveBuffer();
            //if the number of bytes is not what we expect, it execute the reading of the buffer until completion
            while (lenData.USHORT > d.Length)
            {
                ushort remainingNumByte = (ushort)(lenData.USHORT - d.Length);
                byte[] remainingByte = new byte[(remainingNumByte)];
                if (!c.DeviceRead(remainingByte, remainingNumByte))
                {
                    request.Error = String.Format(OmronEthernetIP.Properties.Resources.ErrorImportTimeOutAnswerDate, request.Request.ToString());
                    c.ReceiveClear();
                    return (false);
                }
                d = (byte[])d.Concat(remainingByte).ToArray();                
            }  
            Array.Copy(d, 0, pdu, 0, d.Length);
            
            // Parse the identity attibutes
            if (!ParseTheDeviceResponse(ref pdu, lenData.USHORT, s, ref request))
            {
                c.ReceiveClear();
                return (false);
            }

            return (true);
        }

        private static void PrepareRequest(ref byte[] pdu, ref ushortUnion pduIndex, OmronEthernetIPChannel c, OmronEthernetIPStation s, ref OmronEthernetIPChannel.RequestOfDevice request)
        {

            // Ethernet/IP header
            PrepareEncapsulationHeader(ecSendUnitData, c, ref pdu, ref pduIndex);
            byte bClass = 0;
            byte bServiceAttribute = 0;
            byte bAttributeSegment = 0; 
            byte bPathSizeInWords = 0;
            byte bPathSegmetAttribute = 0x00;
            switch(request.Request)
            {
                case OmronEthernetIPChannel.CommandRequestOfDevice.GetTheTypeOfDevice:
                    {
                        bClass = clNoneC4;
                        bServiceAttribute = saReadsAnAttribute;
                        bAttributeSegment = 0x66;
                        bPathSizeInWords = 0x03;
                        bPathSegmetAttribute = 0x30;
                    }
                    break;
                case OmronEthernetIPChannel.CommandRequestOfDevice.GetNumberOfItem:
                    {
                        bClass = clTagNameService;
                        bServiceAttribute = saReadsAnAttribute;
                        bAttributeSegment = 0x68;
                        bPathSizeInWords = 0x03;
                        bPathSegmetAttribute = 0x30;
                    }
                    break;
                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAllItems:
                    {
                        bClass = clTagNameService;
                        bServiceAttribute = saReadsMultiAttribute;
                        bAttributeSegment = 0x00;
                        bPathSizeInWords = 0x02;
                        bPathSegmetAttribute = 0x01;
                    }
                    break;
                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAttributeSingle:
                    {
                        bClass = clVariableObject;
                        bServiceAttribute = saReadsAnAttribute;
                        bAttributeSegment = 0x01;
                        bPathSizeInWords = 0x03;
                        bPathSegmetAttribute = 0x30;
                    }
                    break;
                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAttributesAll:
                    {
                        bClass = clVariableObject;
                        bServiceAttribute = saReadsAttributeAll;
                        bAttributeSegment = 0x00;
                        bPathSizeInWords = 0x03;
                        bPathSegmetAttribute = 0x01;
                    }
                    break;
                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAttributeSingleVariableTypeObject:
                    {
                        bClass = clVariableTypeObject;
                        bServiceAttribute = saReadsAnAttribute;
                        bAttributeSegment = 0x01;
                        bPathSizeInWords = 0x03;
                        bPathSegmetAttribute = 0x30;
                    }
                    break;
                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAttributesAllVariableTypeObject:
                    {
                        bClass = clVariableTypeObject;
                        bServiceAttribute = saReadsAttributeAll;
                        bAttributeSegment = 0x00;
                        bPathSizeInWords = 0x03;
                        bPathSegmetAttribute = 0x01;
                    }
                    break;

            }
            
            // Interface handle: CIP
            pdu[pduIndex.USHORT++] = 0;
            pdu[pduIndex.USHORT++] = 0;
            pdu[pduIndex.USHORT++] = 0;
            pdu[pduIndex.USHORT++] = 0;

            // Timeout
            pdu[pduIndex.USHORT++] = 0;
            pdu[pduIndex.USHORT++] = 0;

            // Item Count
            pdu[pduIndex.USHORT++] = 2;
            pdu[pduIndex.USHORT++] = 0;

            // Connected Address Item          
            pdu[pduIndex.USHORT++] = 0xa1;
            pdu[pduIndex.USHORT++] = 0;

           
            // Length: 4
            pdu[pduIndex.USHORT++] = 4;
            pdu[pduIndex.USHORT++] = 0;

            // Address Item == Connection ID
            pdu[pduIndex.USHORT++] = s.OTNetConnID.LOUSHORT.LOBYTE;
            pdu[pduIndex.USHORT++] = s.OTNetConnID.LOUSHORT.HIBYTE;
            pdu[pduIndex.USHORT++] = s.OTNetConnID.HIUSHORT.LOBYTE;
            pdu[pduIndex.USHORT++] = s.OTNetConnID.HIUSHORT.HIBYTE;

            // Connected Data Item
            pdu[pduIndex.USHORT++] = 0xb1;
            pdu[pduIndex.USHORT++] = 0;

            // Length: 8            
            pduIndex.USHORT++;
            pduIndex.USHORT++;
            uint positionLength = pduIndex.USHORT;
            //pdu[pduIndex.USHORT++] = lLength;
            //pdu[pduIndex.USHORT++] = 0;

            // Sequence count
            pdu[pduIndex.USHORT++] = s.GetTransaction().LOBYTE;
            pdu[pduIndex.USHORT++] = s.GetTransaction().HIBYTE;

            // Service: Get Attributes All
            pdu[pduIndex.USHORT++] = bServiceAttribute;


            switch(request.Request)
            {
                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAttributesAll:
                    {
                        if (request.ListItemsImportDevice[request.NumVariableObject].InstanceID < 255)
                        {
                            // Path Size: 2 words
                            pdu[pduIndex.USHORT++] = 0x02;

                            // Request Path: Identity, Instance 1
                            pdu[pduIndex.USHORT++] = 0x20; // Class Segment
                            pdu[pduIndex.USHORT++] = bClass; // Identity

                            //Istance segment
                            pdu[pduIndex.USHORT++] = 0x24;//.... ..00 = Logical Segment Format: 8-bit Logical Segment (0)                                               $

                            pdu[pduIndex.USHORT++] = (byte)request.ListItemsImportDevice[request.NumVariableObject].InstanceID;
                        }
                        else if (request.ListItemsImportDevice[request.NumVariableObject].InstanceID < 65536)
                        {
                            // Path Size: 2 words
                            pdu[pduIndex.USHORT++] = 0x03;

                            // Request Path: Identity, Instance 1
                            pdu[pduIndex.USHORT++] = 0x20; // Class Segment
                            pdu[pduIndex.USHORT++] = bClass; // Identity

                            //Istance segment
                            pdu[pduIndex.USHORT++] = 0x25;//.... ..01 = Logical Segment Format: 16-bit Logical Segment (1)

                            pdu[pduIndex.USHORT++] = 0x00;/*padding*/

                            ushortUnion istance = new ushortUnion((ushort)request.ListItemsImportDevice[request.NumVariableObject].InstanceID);
                            pdu[pduIndex.USHORT++] = istance.LOBYTE;
                            pdu[pduIndex.USHORT++] = istance.HIBYTE;
                        }
                        else 
                        {
                            // Path Size: 2 words
                            pdu[pduIndex.USHORT++] = 0x04;

                            // Request Path: Identity, Instance 1
                            pdu[pduIndex.USHORT++] = 0x20; // Class Segment
                            pdu[pduIndex.USHORT++] = bClass; // Identity

                            //Istance segment
                            pdu[pduIndex.USHORT++] = 0x26;//.... ..10 = Logical Segment Format: 32-bit Logical Segment (1)

                            pdu[pduIndex.USHORT++] = 0x00;/*padding*/

                            uintUnion istance = new uintUnion((uint)request.ListItemsImportDevice[request.NumVariableObject].InstanceID);
                            pdu[pduIndex.USHORT++] = istance.LOUSHORT.LOBYTE;
                            pdu[pduIndex.USHORT++] = istance.LOUSHORT.HIBYTE;
                            pdu[pduIndex.USHORT++] = istance.HIUSHORT.LOBYTE;
                            pdu[pduIndex.USHORT++] = istance.HIUSHORT.HIBYTE;
                        }
 
                    }
                    break;

                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAllItems:
                    {
                        // Path Size: 2 words
                        pdu[pduIndex.USHORT++] = bPathSizeInWords;

                        // Request Path: Identity, Instance 1
                        pdu[pduIndex.USHORT++] = 0x20; // Class Segment
                        pdu[pduIndex.USHORT++] = bClass; // Identity

                        pdu[pduIndex.USHORT++] = 0x24; // Instance Segment
                        pdu[pduIndex.USHORT++] = 0x00; // Instance ID


                        //request istance number
                        uintUnion requestItance = new uintUnion(1);
                        if (request.ListItemsImportDevice.Count() == 0)
                        {
                            //Start Instance
                            pdu[pduIndex.USHORT++] = requestItance.LOUSHORT.LOBYTE; 
                            pdu[pduIndex.USHORT++] = requestItance.LOUSHORT.HIBYTE; 
                            pdu[pduIndex.USHORT++] = requestItance.HIUSHORT.LOBYTE;
                            pdu[pduIndex.USHORT++] = requestItance.HIUSHORT.HIBYTE;

                        }
                        else
                        {
                            //Start Instance
                            var last = request.ListItemsImportDevice.Values.Last();
                            requestItance.UINT = last.InstanceID + 1;
                            pdu[pduIndex.USHORT++] = requestItance.LOUSHORT.LOBYTE;
                            pdu[pduIndex.USHORT++] = requestItance.LOUSHORT.HIBYTE;
                            pdu[pduIndex.USHORT++] = requestItance.HIUSHORT.LOBYTE;
                            pdu[pduIndex.USHORT++] = requestItance.HIUSHORT.HIBYTE;

                        }

                        //Num of Instance
                        uintUnion numItemd = new uintUnion(request.NumberOfItemsToRequest);
                        pdu[pduIndex.USHORT++] = numItemd.LOUSHORT.LOBYTE;
                        pdu[pduIndex.USHORT++] = numItemd.LOUSHORT.HIBYTE;
                        pdu[pduIndex.USHORT++] = numItemd.HIUSHORT.LOBYTE;
                        pdu[pduIndex.USHORT++] = numItemd.HIUSHORT.HIBYTE;

                        //Kind of Variabl
                        //0x01 System Variable
                        //0x02 User Defined Variable
                        pdu[pduIndex.USHORT++] = 0x02;

                        pdu[pduIndex.USHORT++] = 0x00;
                    }
                    break;
                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAttributesAllVariableTypeObject:
                    {
                        if (request.ListNestedInstanceID[(int)request.CycleNestedInstanceID] < 255)
                        {
                            // Path Size: 2 words
                            pdu[pduIndex.USHORT++] = 0x02;

                            // Request Path: Identity, Instance 1
                            pdu[pduIndex.USHORT++] = 0x20; // Class Segment
                            pdu[pduIndex.USHORT++] = bClass; // Identity

                            //Istance segment
                            pdu[pduIndex.USHORT++] = 0x24;//.... ..00 = Logical Segment Format: 8-bit Logical Segment (0)                                               $

                            pdu[pduIndex.USHORT++] = (byte)request.ListNestedInstanceID[(int)request.CycleNestedInstanceID];
                        }
                        else if (request.ListNestedInstanceID[(int)request.CycleNestedInstanceID] < 65536)
                        {
                            // Path Size: 2 words
                            pdu[pduIndex.USHORT++] = 0x03;

                            // Request Path: Identity, Instance 1
                            pdu[pduIndex.USHORT++] = 0x20; // Class Segment
                            pdu[pduIndex.USHORT++] = bClass; // Identity

                            //Istance segment
                            pdu[pduIndex.USHORT++] = 0x25;//.... ..01 = Logical Segment Format: 16-bit Logical Segment (1)

                            pdu[pduIndex.USHORT++] = 0x00;/*padding*/

                            ushortUnion istance = new ushortUnion((ushort)request.ListNestedInstanceID[(int)request.CycleNestedInstanceID]);
                            pdu[pduIndex.USHORT++] = istance.LOBYTE;
                            pdu[pduIndex.USHORT++] = istance.HIBYTE;
                        }
                        else
                        {
                            // Path Size: 2 words
                            pdu[pduIndex.USHORT++] = 0x04;

                            // Request Path: Identity, Instance 1
                            pdu[pduIndex.USHORT++] = 0x20; // Class Segment
                            pdu[pduIndex.USHORT++] = bClass; // Identity

                            //Istance segment
                            pdu[pduIndex.USHORT++] = 0x26;//.... ..10 = Logical Segment Format: 32-bit Logical Segment (1)

                            pdu[pduIndex.USHORT++] = 0x00;/*padding*/

                            uintUnion istance = new uintUnion((uint)request.ListNestedInstanceID[(int)request.CycleNestedInstanceID]);
                            pdu[pduIndex.USHORT++] = istance.LOUSHORT.LOBYTE;
                            pdu[pduIndex.USHORT++] = istance.LOUSHORT.HIBYTE;
                            pdu[pduIndex.USHORT++] = istance.HIUSHORT.LOBYTE;
                            pdu[pduIndex.USHORT++] = istance.HIUSHORT.HIBYTE;
                        }
                    }
                    break;

                default:
                    {
                        // Path Size: 2 words
                        pdu[pduIndex.USHORT++] = bPathSizeInWords;

                        // Request Path: Identity, Instance 1
                        pdu[pduIndex.USHORT++] = 0x20; // Class Segment
                        pdu[pduIndex.USHORT++] = bClass; // Identity

                        pdu[pduIndex.USHORT++] = 0x24; // Instance Segment
                        pdu[pduIndex.USHORT++] = 0x00; // Instance ID

                        pdu[pduIndex.USHORT++] = bPathSegmetAttribute; // Instance Segment
                        pdu[pduIndex.USHORT++] = bAttributeSegment; // Instance ID
                    }
                    break;
            }

            // Length: 8
            ushortUnion Length = new ushortUnion((ushort)(pduIndex.USHORT - positionLength));
            pdu[positionLength - 2] = Length.LOBYTE;
            pdu[positionLength - 1] = Length.HIBYTE;

        }

        private static bool ParseTheDeviceResponse(ref byte[] pdu, ushort pduLength,  OmronEthernetIPStation s, ref OmronEthernetIPChannel.RequestOfDevice request)
        {

            // Check the length
            if (pduLength < ALL_ATTRIBUTES_MIN_LENGTH)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ParseDeviceIdentityAttributes - message too short",
                                                       currentTime));
                }
#endif
                request.Error = String.Format(OmronEthernetIP.Properties.Resources.ErrorImportOAnswerDateShort, request.Request.ToString());
                return (false);
            }

            // Check the sequence count
            ushortUnion receivedCipSequenceCount = new ushortUnion(pdu, REPLY_CIP_SERVICE_OFFSET - 2);
            if (!s.CheckTransaction(receivedCipSequenceCount.USHORT))
            {
                s.LastTransOK = false;
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ParseDeviceIdentityAttributes - Wrong CIP sequence count:{1}",
                                                       currentTime, receivedCipSequenceCount.USHORT));
                }
#endif
                request.Error = String.Format(OmronEthernetIP.Properties.Resources.ErrorImportOAnswerSequenceCount, request.Request.ToString());
                return (false);
            }
            s.LastTransOK = true;

            byte bCheckSetviceId ;
            switch(request.Request)
            {
                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAllItems:
                        bCheckSetviceId = saReadsMultiAttribute;
                    break;
                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAttributesAll:
                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAttributesAllVariableTypeObject:
                    bCheckSetviceId = saReadsAttributeAll;
                    break;
                default:
                    bCheckSetviceId = saReadsAnAttribute;
                    break;
            }            

            // Check the service ID
            if (pdu[REPLY_CIP_SERVICE_OFFSET] != (0x80 | bCheckSetviceId))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ParseDeviceIdentityAttributes - Wrong reply service ID",
                                                       currentTime));
                }
#endif
                request.Error = String.Format(OmronEthernetIP.Properties.Resources.ErrorImportOAnswerServiceID, request.Request.ToString());
                return (false);
            }

            // Check the status
            if (pdu[REPLY_CIP_SERVICE_OFFSET + 2] != 0)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ParseDeviceIdentityAttributes - General status different from success: {1}",
                                                       currentTime, pdu[REPLY_CIP_SERVICE_OFFSET + 2]));
                }
#endif
                request.Error = EncodingErrorResponseCode(pdu[REPLY_CIP_SERVICE_OFFSET + 2]);
                return (false);
            }

            //Parser request analyzer.
            switch (request.Request)
            {
                case OmronEthernetIPChannel.CommandRequestOfDevice.GetNumberOfItem:
                    {
                        // Tag Name Server number of Items into the device
                        uintUnion uintUnionLocal = new uintUnion(pdu, ALL_ATTRIBUTES_MIN_LENGTH);
                        request.NumberOfItemsToRequest = uintUnionLocal.UINT;
                        request.NumberOfItems = uintUnionLocal.UINT;
                    }
                    return (true);

                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAttributeSingle:
                    {
                        uintUnion UgetAttribiteSingle = new uintUnion(pdu, ALL_ATTRIBUTES_MIN_LENGTH);
                        if(UgetAttribiteSingle.UINT > 255)
                            return (true);
                        else
                            return (false);
                    }

                case OmronEthernetIPChannel.CommandRequestOfDevice.GetTheTypeOfDevice:
                    {
                        ushortUnion ushortUnionLocal = new ushortUnion(pdu, REPLY_CIP_SERVICE_OFFSET + 3);
                        request.DeviceType = System.Text.Encoding.UTF8.GetString(pdu, REPLY_CIP_SERVICE_OFFSET + 6, ushortUnionLocal.USHORT);
                    }
                    return(true);

                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAttributesAll:
                    {
                        return ParseVariableObject(ref pdu, (REPLY_CIP_SERVICE_OFFSET + 4), pduLength, ref request);
                    }

                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAttributeSingleVariableTypeObject:
                    {
                        uintUnion UgetAttribiteSingle = new uintUnion(pdu, ALL_ATTRIBUTES_MIN_LENGTH);
                        if (UgetAttribiteSingle.UINT > 255)
                            return (true);
                        else
                        {
                            request.Error = String.Format(OmronEthernetIP.Properties.Resources.ErrorImportOAnswerDateShort, request.Request.ToString());
                            return (false);
                        }
                    }

                case OmronEthernetIPChannel.CommandRequestOfDevice.GetAttributesAllVariableTypeObject:
                    {
                        return ParseVariableTypeObject(ref pdu, (REPLY_CIP_SERVICE_OFFSET + 4), pduLength, ref request);
                    }

            }

            // Second check of the length
            if (pduLength < (ALL_ATTRIBUTES_MIN_LENGTH + 16))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - ParseDeviceIdentityAttributes - message too short (2nd check)",
                                                       currentTime));
                }
#endif
                return (false);
            }

            if (request.Request == OmronEthernetIPChannel.CommandRequestOfDevice.GetAllItems)
            {
                // Number of items contained in the response
                ushortUnion NumofItemsSend = new ushortUnion(pdu, REPLY_CIP_SERVICE_OFFSET + 4);

                //Instance Remaining is a bool 0 no 1 yes
                byte bInstanceRemaining = pdu[REPLY_CIP_SERVICE_OFFSET + 6];

                if (bInstanceRemaining > 0)
                {
                    //Number of items to request, when the value is equal to zero, the request for items ends.
                    request.NumberOfItemsToRequest = request.NumberOfItemsToRequest - NumofItemsSend.USHORT;
                }
                else
                {
                    request.NumberOfItemsToRequest = 0;
                }

                return ParseListItems(ref pdu, (REPLY_CIP_SERVICE_OFFSET + 8), pduLength, NumofItemsSend.USHORT, ref request);
            }

            return (true);
        }

        public static bool ParseListItems(ref byte[] pdu, ushort pduIndex, ushort pduLength, ushort NumOfItemsSend , ref OmronEthernetIPChannel.RequestOfDevice request)
        {
            bool res = false;
            NumOfItemsSend =(ushort) (((ushort)(request.ListItemsImportDevice.Count())) + NumOfItemsSend);
            while (true)
            {
                //Instance ID (4 byte)
                uintUnion InstanceID = new uintUnion(pdu, pduIndex);
                pduIndex += 4;

                // Data Length Item(2 Byte)
                ushortUnion LengthTitem = new ushortUnion(pdu, pduIndex);
                pduIndex += 2;
                ushort endLengthItem = (ushort)(pduIndex + LengthTitem.USHORT);

                //Class ID 2 byte
                ushortUnion ClassId = new ushortUnion(pdu, pduIndex);
                pduIndex += 2;

                //Instance ID (4 byte)
                uintUnion InstanceID_2 = new uintUnion(pdu, pduIndex);
                pduIndex += 4;

                //Tag Name Length SHORT (1 Byte)
                byte TagNameLength = pdu[pduIndex];
                pduIndex += 1;                

                //tag name
                string tagName = System.Text.Encoding.UTF8.GetString(pdu, pduIndex, (TagNameLength ));

                //padding (1 Byte)
                ushort padding = (ushort)((pduIndex + TagNameLength) % 2);

                if ((endLengthItem == (pduIndex + TagNameLength + padding) &&
                    (InstanceID.UINT == InstanceID_2.UINT) && 
                    ((pduIndex + TagNameLength + padding) <= pduLength)))
                {
                    OmronEthernetIPChannel.ItemImportDevice ItemImportDevice = new OmronEthernetIPChannel.ItemImportDevice();
                    ItemImportDevice.Name = tagName;
                    ItemImportDevice.InstanceID = InstanceID.UINT;
                    ItemImportDevice.ClassID = ClassId.USHORT;
                    request.ListItemsImportDevice.Add(InstanceID.UINT,ItemImportDevice);
                    if(request.ListItemsImportDevice.Count == NumOfItemsSend)
                    {
                        res = true;
                        break;
                    }
                    pduIndex = (ushort)(pduIndex + TagNameLength + padding );
                }
                else
                {
                    break;
                }                
            }
            return (res);
        }
        public static bool ParseVariableObject(ref byte[] pdu, ushort pduIndex, ushort pduLength, ref OmronEthernetIPChannel.RequestOfDevice request)
        {
            //bool res = false;

            //Size (4 byte)
            //BYTE,SINT,USINT 1
            //BOOL,INT, UINT, WORD 2
            //DINT, UDINT, REAL,DWORD 4
            //LINT, ULINT,LREAL,LWORD TIME, DATE, TIME_OF_DAY, DATE_AND_TIME 8
            //STRING Size of String including NULL Max 1986Byte
            //STRUCT Size of Struct
            //ARRAY Size of Arra
            uintUnion size = new uintUnion(pdu, pduIndex);
            request.ListItemsImportDevice[request.NumVariableObject].Size = size.UINT;

            pduIndex += 4;
            
            //Data Type
            byte datatype = pdu[pduIndex];
            request.ListItemsImportDevice[request.NumVariableObject].Type = EncodingDataType(pdu[pduIndex], size.UINT);
            pduIndex += 1;
            
            //Data Type of Array
            byte datatypeofarray = pdu[pduIndex];
            request.ListItemsImportDevice[request.NumVariableObject].Datatypeofarray = EncodingDataType(pdu[pduIndex], size.UINT);
            pduIndex += 1;

            //Array Dimension
            byte arraydimension = pdu[pduIndex];
            pduIndex += 1;
            request.ListItemsImportDevice[request.NumVariableObject].ArrayDimension = arraydimension;

            //padding
            if ((pduIndex % 2) == 1)
            {
                pduIndex += 1;
            }

            //Num of Elements
            if (arraydimension != 0)
            {
                for (int index = 0; index < arraydimension; index++)
                {
                    uintUnion arraydim = new uintUnion(pdu, pduIndex);
                    pduIndex += 4;
                    request.ListItemsImportDevice[request.NumVariableObject].ListOfIndexArray.Add(arraydim.UINT);
                    request.ListItemsImportDevice[request.NumVariableObject].ListOfIndexArray.Add(0);
                }
            }
            //Reserved Byte 
            pduIndex += 8;

            //Bit No
            //A bit specified position (0-15) at the time of the BOOL Data Type.
            //If it is not type Bool, this Attribute returns 0
            byte BitNo = pdu[pduIndex];
            pduIndex += 4;

            //Variable Type Instance ID
            uintUnion VariableTypeInstanceID = new uintUnion(pdu, pduIndex);
            pduIndex += 4;
            request.ListItemsImportDevice[request.NumVariableObject].NestedInstanceID = VariableTypeInstanceID.UINT;
            //Num of Elements
            if (arraydimension != 0)
            {
                for (int index = 0; index < arraydimension; index++)
                {
                    uintUnion arraydim = new uintUnion(pdu, pduIndex);
                    pduIndex += 4;
                    request.ListItemsImportDevice[request.NumVariableObject].ListOfIndexArray[(index * 2 + 1)] =  arraydim.UINT;
                }
            }
            return (true);
        }

        public static bool ParseVariableTypeObject(ref byte[] pdu, ushort pduIndex, ushort pduLength, ref OmronEthernetIPChannel.RequestOfDevice request)
        {
            //bool res = false;
            OmronEthernetIPChannel.ItemImportDevice item = new OmronEthernetIPChannel.ItemImportDevice();
            //Size (4 byte)
            //BYTE,SINT,USINT 1
            //BOOL,INT, UINT, WORD 2
            //DINT, UDINT, REAL,DWORD 4
            //LINT, ULINT,LREAL,LWORD TIME, DATE, TIME_OF_DAY, DATE_AND_TIME 8
            //STRING Size of String including NULL Max 1986Byte
            //STRUCT Size of Struct
            //ARRAY Size of Arra
            uintUnion size = new uintUnion(pdu, pduIndex);
            item.Size = size.UINT;
            pduIndex += 4;

            pduIndex++;//Reserved

            //Data Type
            byte datatype = pdu[pduIndex];
            item.Type = EncodingDataType(pdu[pduIndex], size.UINT);
            pduIndex += 1;

            //Data Type of Array
            byte datatypeofarray = pdu[pduIndex];
            item.Datatypeofarray = EncodingDataType(pdu[pduIndex], size.UINT);
            pduIndex += 1;

            //Array Dimension
            byte arraydimension = pdu[pduIndex];
            pduIndex += 1;
            item.ArrayDimension = arraydimension;

            //Num of Elements
            if (arraydimension != 0)
            {
                for (int index = 0; index < arraydimension; index++)
                {
                    uintUnion arraydim = new uintUnion(pdu, pduIndex);
                    item.ListOfIndexArray.Add(arraydim.UINT);
                    item.ListOfIndexArray.Add(0);
                    pduIndex += 4;
                }
            }

            //Number of Member
            ushortUnion numberofmembers = new ushortUnion(pdu, pduIndex);
            pduIndex += 2;
            item.NumberofMembers = numberofmembers.USHORT;

            //Reserved Byte 
            pduIndex += 4;

            //CRC Code
            ushortUnion CRCcode = new ushortUnion(pdu, pduIndex);
            pduIndex += 2;

            //Variable Type Name Length
            byte Length = pdu[pduIndex];
            pduIndex += 1;

            //tag name
            string tagName = System.Text.Encoding.UTF8.GetString(pdu, pduIndex, (Length));
            pduIndex += Length;
            //(padding)
            if ((pduIndex % 2) != 0)
            {
                pduIndex++;
            }
            item.Name = tagName;

            // Next Instance ID(32bit)
            uintUnion NextVariableInstanceID = new uintUnion(pdu, pduIndex);
            pduIndex += 4;
            if (NextVariableInstanceID.UINT != 0)
            {
                item.NextInstanceID = NextVariableInstanceID.UINT;
                request.ListNestedInstanceID.Add(NextVariableInstanceID.UINT);
            }
            item.InstanceID = request.ListNestedInstanceID[(int)request.CycleNestedInstanceID];

            // Nesting Variable Type Instance ID(32bit)
            uintUnion NestingVariableInstanceID = new uintUnion(pdu, pduIndex);
            pduIndex += 4;
            if (NestingVariableInstanceID.UINT != 0)
            {
                item.NestedInstanceID = NestingVariableInstanceID.UINT;
                request.ListNestedInstanceID.Add(NestingVariableInstanceID.UINT);
            }

            if (arraydimension != 0)
            {
                for (int index = 0; index < arraydimension; index++)
                {
                    uintUnion arraydim = new uintUnion(pdu, pduIndex);
                    item.ListOfIndexArray[(index * 2 +1)] = arraydim.UINT;
                    pduIndex += 4;
                }
            }
            if(!request.DictionaryNestedInstanceID.ContainsKey(request.ListNestedInstanceID[(int)request.CycleNestedInstanceID]))
                request.DictionaryNestedInstanceID.Add(request.ListNestedInstanceID[(int)request.CycleNestedInstanceID], item);
            return (true);
        }

        public static bool CheckDeviceIdentity(ushort vendorID, string productName)
        {
            // Check the Vendor ID and the Product Name of the device
            if ((vendorID != OmronEthernetIPProtocol.OMRON_VENDOR_ID) || (!productName.Contains("NJ")))
            {
                return (false);
            }

            return (true);
        }

        public static string EncodingErrorResponseCode(byte errorCode)
        {
            string msgErro = string.Empty;
            switch(errorCode)
            {
                case 0x04:/*The request path specification is not correct. (Not0)*/
                    msgErro = OmronEthernetIP.Properties.Resources.ErrorCipStatus04;
                    break;

                case 0x05:/*Instance ID specified is invalid*/
                    msgErro = OmronEthernetIP.Properties.Resources.ErrorCipStatus05;
                    break;

                case 0x0C:/*A download is in progress.*/
                    msgErro = OmronEthernetIP.Properties.Resources.ErrorCipStatus0C;
                    break;

                case 0x11:/*A responce exceeds the maximum response length*/
                    msgErro = OmronEthernetIP.Properties.Resources.ErrorCipStatus11;
                    break;

                case 0x13:/*The data length was too short for the specified service*/
                    msgErro = OmronEthernetIP.Properties.Resources.ErrorCipStatus13;
                    break;

                case 0x15:/*The data length was too long for the specified service*/
                    msgErro = OmronEthernetIP.Properties.Resources.ErrorCipStatus15;
                    break;
            }
            return msgErro;
        }

        public static string EncodingDataType(byte datatype, uint size)
        {
            switch(datatype)
            {
                case 0xC1: return ("BOOL");
                case 0xC2: return ("SINT");
                case 0xC3: return ("INT");
                case 0xC4: return ("DINT");
                case 0xC5: return ("LINT");
                case 0xC6: return ("USINT");
                case 0xC7: return ("UINT");
                case 0xC8: return ("UDINT");
                case 0xC9: return ("ULINT");
                case 0xCA: return ("REAL");
                case 0xCB: return ("LREAL");
                case 0xD0: return (string.Format("STRING({0})", size));
                case 0xD1: return ("BYTE");
                case 0xD2: return ("WORD");
                case 0xD3: return ("DWORD");
                case 0xDB: return ("TIME");
                case 0xD4: return ("LWORD");
                case 0xA0: return ("STRUCT");
                case 0xA2: return ("STRUCT");
                case 0xA3: return ("ARRAY");
                case 0x04: return ("UINT BCD");
                case 0x05: return ("UDINT BCD");
                case 0x06: return ("ULINT BCD");
                case 0x07: return ("ENUM");
                case 0x08: return ("DATE_NSEC");
                case 0x09: return ("TIME_NSEC");
                case 0x0A: return ("DATE_AND_TIME_NSEC");
                case 0x0B: return ("TIME_OF_DAY_NSEC");
                case 0x0C: return ("Union");
            }
            return ("BOOL");
        }

        public static bool GetVariableMonitoringObjectAttributes(ref byte[] pdu, ref ushortUnion pduIndex, OmronEthernetIPChannel c, OmronEthernetIPStation s)
        {
            // Prepare the request message
            PrepareGetVariableMonitoringObjectAttributes(ref pdu, ref pduIndex, c, s);

            // Empty the channel receive buffer
            c.ReceiveClear();

            // Send the request
            if (!c.OmronEthernetIPDeviceWrite(ref pdu, ref pduIndex))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetVariableMonitoringObjectAttributes - OmronEthernetIPDeviceWrite failed",
                                                       currentTime));
                }
#endif
                return (false);
            }

            // Read the fixed-size header
            if (!c.DeviceRead(pdu, ENCAPSULATION_HEADER_SIZE))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetVariableMonitoringObjectAttributes - DeviceRead Encapsulation Header failed",
                                                       currentTime));
                }
#endif
                c.ReceiveClear();
                return (false);
            }

            // Validate the reply header
            if(!CheckEncapsulationHeader(ref pdu, ecSendUnitData.USHORT))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - Error in CheckEncapsulationHeader",
                                                       currentTime));
                }
#endif
                c.ReceiveClear();
                return (false);
            }

            // Read the remaining part of the reply
            ushortUnion lenData = new ushortUnion(pdu, EDATA_LEN_OFFS);
            if (!c.DeviceRead(pdu, lenData.USHORT))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetVariableMonitoringObjectAttributes - DeviceRead CIP reply failed",
                                                       currentTime));
                }
#endif
                c.ReceiveClear();
                return (false);
            }

            // Check the reply
            if (!CheckVariableMonitoringObjectAttributes(ref pdu, lenData.USHORT, s))
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - GetVariableMonitoringObjectAttributes - CheckVariableMonitoringObjectAttributes failed",
                                                       currentTime));
                }
#endif
                c.ReceiveClear();
                return (false);
            }

            c.ReceiveClear();
            return (true);
        }

        private static void PrepareGetVariableMonitoringObjectAttributes(ref byte[] pdu, ref ushortUnion pduIndex, OmronEthernetIPChannel c, OmronEthernetIPStation s)
        {

            // Ethernet/IP header
            PrepareEncapsulationHeader(ecSendUnitData, c, ref pdu, ref pduIndex);

            // Interface handle: CIP
            pdu[pduIndex.USHORT++] = 0;
            pdu[pduIndex.USHORT++] = 0;
            pdu[pduIndex.USHORT++] = 0;
            pdu[pduIndex.USHORT++] = 0;

            // Timeout
            pdu[pduIndex.USHORT++] = 0;
            pdu[pduIndex.USHORT++] = 0;

            // Item Count
            pdu[pduIndex.USHORT++] = 2;
            pdu[pduIndex.USHORT++] = 0;

            // Connected Address Item
            pdu[pduIndex.USHORT++] = 0xa1;
            pdu[pduIndex.USHORT++] = 0;

            // Length: 4
            pdu[pduIndex.USHORT++] = 4;
            pdu[pduIndex.USHORT++] = 0;

            // Address Item == Connection ID
            pdu[pduIndex.USHORT++] = s.OTNetConnID.LOUSHORT.LOBYTE;
            pdu[pduIndex.USHORT++] = s.OTNetConnID.LOUSHORT.HIBYTE;
            pdu[pduIndex.USHORT++] = s.OTNetConnID.HIUSHORT.LOBYTE;
            pdu[pduIndex.USHORT++] = s.OTNetConnID.HIUSHORT.HIBYTE;

            // Connected Data Item
            pdu[pduIndex.USHORT++] = 0xb1;
            pdu[pduIndex.USHORT++] = 0;

            // Length: 8
            pdu[pduIndex.USHORT++] = 8;
            pdu[pduIndex.USHORT++] = 0;

            // Sequence count
            pdu[pduIndex.USHORT++] = s.GetTransaction().LOBYTE;
            pdu[pduIndex.USHORT++] = s.GetTransaction().HIBYTE;

            // Service: Get Attributes All
            pdu[pduIndex.USHORT++] = 1;

            // Path Size: 2 words
            pdu[pduIndex.USHORT++] = 2;

            // Request Path: Variable Monitoringy, Instance 0
            pdu[pduIndex.USHORT++] = 0x20; // Class Segment
            pdu[pduIndex.USHORT++] = 0x6d; // Variable Monitoring
            pdu[pduIndex.USHORT++] = 0x24; // Instance Segment
            pdu[pduIndex.USHORT++] = 0; // Instance ID
        }

        private static bool CheckVariableMonitoringObjectAttributes(ref byte[] pdu, ushort pduLength, OmronEthernetIPStation s)
        {
            // Check the PDU length (the length of the message without the Ethernet/IP header)
            if (pduLength < ALL_ATTRIBUTES_MIN_LENGTH)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckVariableMonitoringObjectAttributes - message too short",
                                                       currentTime));
                }
#endif
                return (false);
            }

            // Check the sequence count
            ushortUnion receivedCipSequenceCount = new ushortUnion(pdu, REPLY_CIP_SERVICE_OFFSET - 2);
            if(!s.CheckTransaction(receivedCipSequenceCount.USHORT))
            {
                s.LastTransOK = false;
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckVariableMonitoringObjectAttributes - Wrong CIP sequence count:{1}",
                                                       currentTime, receivedCipSequenceCount.USHORT));
                }
#endif
                return (false);
            }
            s.LastTransOK = true;

            // Check the service ID
            if (pdu[REPLY_CIP_SERVICE_OFFSET] != 0x81)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckVariableMonitoringObjectAttributes - Wrong reply service ID",
                                                       currentTime));
                }
#endif
                return (false);
            }

            // Check the status
            if (pdu[REPLY_CIP_SERVICE_OFFSET + 2] != 0)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - CheckVariableMonitoringObjectAttributes - General status different from success: {1}",
                                                       currentTime, pdu[REPLY_CIP_SERVICE_OFFSET + 2]));
                }
#endif
                return (false);
            }

            return (true);
        }

        public static ushort Logix5550PrepareRequest(ref List<CommJob> list, OmronEthernetIPChannel c, ref OmronEthernetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            bool isReading = true;
            PrepareBody prepareBody;
            if ((((OmronEthernetIPCommJob)list[0]).CommandType == CommandTypes.ReadCmd) || (((OmronEthernetIPCommJob)list[0]).CommandType == CommandTypes.ReadDataFormatCmd))
            {
                prepareBody = Logix5550TagsPrepareMultiReadReq;
            }
            else if (((OmronEthernetIPCommJob)list[0]).CommandType == CommandTypes.WriteCmd)
            {
                isReading = false;
                prepareBody = Logix5550TagsPrepareMultiWriteReq;
            }
            else
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(String.Format("{0} - Failure 1 in Logix5550PrepareRequest job: {1}", DateTime.UtcNow.ToString(), ((OmronEthernetIPCommJob)list[0]).ABAddress));
#endif
                return 0;
            }

            PrepareEncapsulationHeader(ecSendUnitData, c, ref pdu, ref pduPointer);

            List<PrepareHeader> prepareHeaderList = new List<PrepareHeader>();
            PrepareHeader logix5550PrepareRequestHeader = Logix5550PrepareRequestHeader;
            PrepareHeader logix5550TagsPrepareCpfReqHeader = Logix5550TagsPrepareCpfReqHeader;

            prepareHeaderList.Add(logix5550PrepareRequestHeader);
            prepareHeaderList.Add(logix5550TagsPrepareCpfReqHeader);

            // Complete the request message
            ushort returnValue = PrepareEncapsulateRequest(ref prepareHeaderList, prepareBody,
                                             ref list, ref s,
                                             ref pdu, ref pduPointer);
#if DEBUG
            if (returnValue == 0)
            {
                if (isReading)
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("{0} - Failure in PrepareEncapsulateRequest (Read Request)", DateTime.UtcNow.ToString()));
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine(String.Format("{0} - Failure in PrepareEncapsulateRequest (Write Request)", DateTime.UtcNow.ToString()));
                }
            }
#endif
            return (returnValue);
        }

        public static ushort Logix5550PrepareUnconnectedRequest(CommandTypes command, OmronEthernetIPChannel c, OmronEthernetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort StartCh = pduPointer.USHORT;
            PrepareEncapsulationHeader(ecSendRRData, c, ref pdu, ref pduPointer);

            // Complete the request message
            Logix5550PrepareUnconnectedCPF(command, s, ref pdu, ref pduPointer);
            return ((ushort)(pduPointer.USHORT - StartCh));
        }
        public static ushort Logix5550PrepareUnconnectedCPF(CommandTypes command, OmronEthernetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort StartCh = pduPointer.USHORT;

            //Handle = 0 for CIP encapsulated messages
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
             
             
            //Timeout (seconds)
            pdu[pduPointer.USHORT++] = 0x0A;
            pdu[pduPointer.USHORT++] = 0;
             
            //Common Packet Format (CPF)
            //Item Count
            pdu[pduPointer.USHORT++] = 2;
            pdu[pduPointer.USHORT++] = 0;
             
            //Common Packet Format (CPF)
            //Address Item - Item Type = NULL Address Item
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            
            //Common Packet Format (CPF)
            //Address Item - Item Data Length = 0
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            
            //Common Packet Format (CPF)
            //Data Item - Item Type = Unconnected Data Item
            pdu[pduPointer.USHORT++] = 0xB2;
            pdu[pduPointer.USHORT++] = 0;
            
            //Common Packet Format (CPF)
            //Data Item - Item Data Length = variable (filled up later)
            ushort CPFDataLenOffs = pduPointer.USHORT;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            //Common Packet Format (CPF)
            //Transport PDU
            ushortUnion PDULen = new ushortUnion(Logix5550PrepareUnconnectedTPDU(command, s, ref pdu, ref pduPointer));

            if (PDULen.USHORT == 0)
            {
                return 0;
            }

            //Fill up data length
            pdu[CPFDataLenOffs] = PDULen.LOBYTE;
            pdu[CPFDataLenOffs + 1] = PDULen.HIBYTE;
             
            return ((ushort)(pduPointer.USHORT - StartCh));
        }

        private static ushort Logix5550GetNewConnectionID(int seed )
        {
            Random random = new Random(seed);
            return (ushort)random.Next(65535);
        }

        public static ushort Logix5550PrepareUnconnectedTPDU(CommandTypes command, OmronEthernetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort StartCh = pduPointer.USHORT;
            if (command == CommandTypes.FwdOpen)
            {
                //Service Code = Forward Open service request (0x54)
                pdu[pduPointer.USHORT++] = 0x54;
            }
            else if(command == CommandTypes.LargeFwdOpen)
            {
                //Service Code = Large Forward Open service request (0x5b)
                pdu[pduPointer.USHORT++] = 0x5b;
            }
            else {
                //Service Code = Forward Close service request (0x4E)
                pdu[pduPointer.USHORT++] = 0x4E;
            }
            
            //Size of Request Path = 2 (words)
            pdu[pduPointer.USHORT++] = 0x02;
            
            //Request Path = 20,06 (class CM); 24,01 (instance 01)
            pdu[pduPointer.USHORT++] = 0x20;
            pdu[pduPointer.USHORT++] = 0x06;
            pdu[pduPointer.USHORT++] = 0x24;
            pdu[pduPointer.USHORT++] = 0x01;
            
            // Connection priority / Tick time
            pdu[pduPointer.USHORT++] = 0x06;

            // Timeout ticks
            pdu[pduPointer.USHORT++] = 0x9A;
            
            if ((command == CommandTypes.FwdOpen) || (command == CommandTypes.LargeFwdOpen))
            {
                // Originator > Target Network Connection ID
                pdu[pduPointer.USHORT++] = s.OTNetConnID.LOUSHORT.LOBYTE;
                pdu[pduPointer.USHORT++] = s.OTNetConnID.LOUSHORT.HIBYTE;
                pdu[pduPointer.USHORT++] = s.OTNetConnID.HIUSHORT.LOBYTE;
                pdu[pduPointer.USHORT++] = s.OTNetConnID.HIUSHORT.HIBYTE;
                
                // Target > Originator Network Connection ID
                pdu[pduPointer.USHORT++] = s.TONetConnID.LOUSHORT.LOBYTE;
                pdu[pduPointer.USHORT++] = s.TONetConnID.LOUSHORT.HIBYTE;
                pdu[pduPointer.USHORT++] = s.TONetConnID.HIUSHORT.LOBYTE;
                pdu[pduPointer.USHORT++] = s.TONetConnID.HIUSHORT.HIBYTE;
                // Connection serial number
                s.ConnectionSerialNumber.USHORT = Logix5550GetNewConnectionID((int)s.OTNetConnID.UINT);
            }
            pdu[pduPointer.USHORT++] = s.ConnectionSerialNumber.LOBYTE;
            pdu[pduPointer.USHORT++] = s.ConnectionSerialNumber.HIBYTE;

            // Originator Vendor ID
            pdu[pduPointer.USHORT++] = VENDOR_ID.LOBYTE;
            pdu[pduPointer.USHORT++] = VENDOR_ID.HIBYTE;

            // Originator Serial #
            pdu[pduPointer.USHORT++] = CIP_SERIAL_NUMBER.LOUSHORT.LOBYTE;
            pdu[pduPointer.USHORT++] = CIP_SERIAL_NUMBER.LOUSHORT.HIBYTE;
            pdu[pduPointer.USHORT++] = CIP_SERIAL_NUMBER.HIUSHORT.LOBYTE;
            pdu[pduPointer.USHORT++] = CIP_SERIAL_NUMBER.HIUSHORT.HIBYTE;

            if (command == CommandTypes.FwdOpen)
            {
                // Connection timeout multiplier
                pdu[pduPointer.USHORT++] = 0x04;

                // Reserved
                pdu[pduPointer.USHORT++] = 0;
                pdu[pduPointer.USHORT++] = 0;
                pdu[pduPointer.USHORT++] = 0;
                
                // Originator > Target Requested Packet Interval
                pdu[pduPointer.USHORT++] = 0x80;
                pdu[pduPointer.USHORT++] = 0x84;
                pdu[pduPointer.USHORT++] = 0x1E;
                pdu[pduPointer.USHORT++] = 0;

                // Originator > Target Network Connection Parameters (ox43F8):
                // Connection Size = 504
                // Variable amount of data
                // Low Priority
                // Connection Point to Point
                // Exclusive Owner
                //pdu[pduPointer.USHORT++] = 0xF8;
                //pdu[pduPointer.USHORT++] = 0xC7;
                uint connectionSize = s.MaxPduSize;
                pdu[pduPointer.USHORT++] = (byte)connectionSize;
                byte connectionParametersHighByte = (byte)(0xC6 + ((connectionSize & 0x100) >> 8)); 
                pdu[pduPointer.USHORT++] = connectionParametersHighByte;
                
                // Target > Originator Requested Packet Interval
                pdu[pduPointer.USHORT++] = 0x80;
                pdu[pduPointer.USHORT++] = 0x84;
                pdu[pduPointer.USHORT++] = 0x1E;
                pdu[pduPointer.USHORT++] = 0;

                // Target > Originator Network Connection Parameters
                //pdu[pduPointer.USHORT++] = 0xF8;
                //pdu[pduPointer.USHORT++] = 0xC7;
                pdu[pduPointer.USHORT++] = (byte)connectionSize;
                pdu[pduPointer.USHORT++] = connectionParametersHighByte;

                // Transport type / Trigger
                pdu[pduPointer.USHORT++] = 0xA3;
            }

            else if (command == CommandTypes.LargeFwdOpen)
            {
                // Connection timeout multiplier
                pdu[pduPointer.USHORT++] = 0x04;

                // Reserved
                pdu[pduPointer.USHORT++] = 0;
                pdu[pduPointer.USHORT++] = 0;
                pdu[pduPointer.USHORT++] = 0;

                // Originator > Target Requested Packet Interval
                pdu[pduPointer.USHORT++] = 0x80;
                pdu[pduPointer.USHORT++] = 0x84;
                pdu[pduPointer.USHORT++] = 0x1E;
                pdu[pduPointer.USHORT++] = 0;

                // Originator > Target Network Connection Parameters (0x420007cc):
                // Exclusive Owner (0...)
                // Connection Point to Point (.10.)
                // Low Priority (.... 00..)
                // Variable amount of data (.... ..1.)
                // Connection Size = 1996 (.... .... .... .... 0000 0111 1100 1100)
                //pdu[pduPointer.USHORT++] = 0xCC;
                //pdu[pduPointer.USHORT++] = 0x07;
                uint connectionSize = s.MaxPduSize;
                pdu[pduPointer.USHORT++] = (byte)connectionSize;
                byte connectionParametersHighByte = (byte)((connectionSize & 0xFF00) >> 8);
                pdu[pduPointer.USHORT++] = connectionParametersHighByte;
                pdu[pduPointer.USHORT++] = 0;
                pdu[pduPointer.USHORT++] = 0x42;

                // Target > Originator Requested Packet Interval
                pdu[pduPointer.USHORT++] = 0x80;
                pdu[pduPointer.USHORT++] = 0x84;
                pdu[pduPointer.USHORT++] = 0x1E;
                pdu[pduPointer.USHORT++] = 0;

                // Target > Originator Network Connection Parameters (0x420007cc):
                //pdu[pduPointer.USHORT++] = 0xCC;
                //pdu[pduPointer.USHORT++] = 0x07;
                pdu[pduPointer.USHORT++] = (byte)connectionSize;
                pdu[pduPointer.USHORT++] = connectionParametersHighByte;
                pdu[pduPointer.USHORT++] = 0;
                pdu[pduPointer.USHORT++] = 0x42;

                // Transport type / Trigger
                pdu[pduPointer.USHORT++] = 0xA3;
            }

            // Connection path size (words)
            pdu[pduPointer.USHORT++] = 0x03;

            if (command == CommandTypes.FwdClose)
            {
                // Reserved
                pdu[pduPointer.USHORT++] = 0;
            }
            
            // Connection Path
            pdu[pduPointer.USHORT++] = 0x01;
            //pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = s.CPUSlot;
            pdu[pduPointer.USHORT++] = 0x20;
            pdu[pduPointer.USHORT++] = 0x02;
            pdu[pduPointer.USHORT++] = 0x24;
            pdu[pduPointer.USHORT++] = 0x01;
            
            return ((ushort)(pduPointer.USHORT - StartCh));
        }

        public static void Logix5550PrepareRequestHeader(
            OmronEthernetIPStation s,
            ref byte[] pdu, ref ushortUnion pduPointer)
        {
            //Handle = 0 for CIP encapsulated messages
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            //Timeout (seconds)
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            //Common Packet Format (CPF)
            //Item Count
            pdu[pduPointer.USHORT++] = 2;
            pdu[pduPointer.USHORT++] = 0;

            //Common Packet Format (CPF)
            //Address Item - Item Type = Connected Address Item
            pdu[pduPointer.USHORT++] = 0xA1;
            pdu[pduPointer.USHORT++] = 0;

            //Common Packet Format (CPF)
            //Address Item - Item Data Length = 4
            pdu[pduPointer.USHORT++] = 0x04;
            pdu[pduPointer.USHORT++] = 0;

            //Common Packet Format (CPF)
            //Address Item - Connection ID
            pdu[pduPointer.USHORT++] = s.OTNetConnID.LOUSHORT.LOBYTE;
            pdu[pduPointer.USHORT++] = s.OTNetConnID.LOUSHORT.HIBYTE;
            pdu[pduPointer.USHORT++] = s.OTNetConnID.HIUSHORT.LOBYTE;
            pdu[pduPointer.USHORT++] = s.OTNetConnID.HIUSHORT.HIBYTE;

            //Common Packet Format (CPF)
            //Data Item - Item Type = Connected Data Item
            pdu[pduPointer.USHORT++] = 0xB1;
            pdu[pduPointer.USHORT++] = 0;


        }

        public static void Logix5550TagsPrepareCpfReqHeader(
            OmronEthernetIPStation s,
            ref byte[] pdu, ref ushortUnion pduPointer)
        {
            // Sequence Count
            pdu[pduPointer.USHORT++] = s.GetTransaction().LOBYTE;
            pdu[pduPointer.USHORT++] = s.GetTransaction().HIBYTE;

            //Service Code = "Multiple" service request (0x0A)
            pdu[pduPointer.USHORT++] = 0x0A;

            // Request Path Size (words)
            pdu[pduPointer.USHORT++] = 0x02;

            // 8 - Bit Logical Class Segment
            pdu[pduPointer.USHORT++] = 0x20;

            // Class = Message Router
            pdu[pduPointer.USHORT++] = 0x02;

            // 8 - Bit Logical Instance Segment 
            pdu[pduPointer.USHORT++] = 0x24;

            // Instance = 1
            pdu[pduPointer.USHORT++] = 0x01;
        }

        public static bool getWriteListLimitate(ref OmronEthernetIPCommJob j, ref ReadWriteListLimitateSize frameSize, ReadWriteLimitate mode)
        {
            //if (frameSize.IsEmpty())
            //    frameSize.TotalSize = LOGIX5550_CONNECTION_SIZE_HEADER;
            if (frameSize.IsEmpty())
                frameSize.TotalSize = MULTIPLE_SERVICE_REQUEST_FIX_OVERHEAD + CIP_SEQUENCE_COUNT_SIZE;

            //ushort lastWriteSize = 0;            
            uint pduMaxSize = ((OmronEthernetIPStation)j.Station).MaxPduSize;

            // Be sure to avoid problems in writing big arrays: when the index is > 255, the segment type 0x29 is used instead of 0x28 to specify the array index;
            // the segment type 0x29 requires two more bytes
            pduMaxSize -= 2;
            
                foreach (Tag tag in j.TagsList)
                {
                //lastWriteSize = writeSize;
                    frameSize.TotalSize += j.GetTagReadBufferSize(tag);
                    frameSize.TotalSize += 8;
                    if ((uint)tag.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                    {
                        // In case of boolean variables, the request contains two data bytes: Status (TRUE/FALSE) and Forced set/reset Information (Forced/Not Forced)
                        frameSize.TotalSize += (ushort)((j.adjSizeWrite(tag) - j.PartialArrayStart * j.ElemSize) * 2);
                    }
                    else if ((uint)tag.TagNode.DataType.Identifier == (uint)BuiltInType.String)
                    {
                        // In case of string variables, the request contains also 2 bytes for the length of the string
                        frameSize.TotalSize += (ushort)(j.adjSizeWrite(tag) - j.PartialArrayStart * j.ElemSize + 2);
                    }
                    else
                    {
                        frameSize.TotalSize += (ushort)(j.adjSizeWrite(tag) - j.PartialArrayStart * j.ElemSize);
                    }
                    if (frameSize.TotalSize > pduMaxSize)
                    {
                        if (tag.Size / j.ElemSize <= 1)
                        {
                            //System.Diagnostics.Trace.TraceInformation("{0} Write DBG - getWriteListLimitate 1 - writeSize = {1}",
                            //                                         DateTime.UtcNow.ToString(), lastWriteSize);
                            return false;
                        }
                        int tmp = (j.adjSizeWrite(tag) + (int)pduMaxSize + 1 - frameSize.TotalSize - j.ElemSize) / j.ElemSize;
                        if (mode == ReadWriteLimitate.Add)
                            tmp -= j.PartialArrayStart;                        
                        if (tmp > 0)
                        {
                            if (mode == ReadWriteLimitate.Add)
                                j.PartialArrayEnd = (ushort)(tmp + j.PartialArrayStart);
                            //nextlistLimitate.Add(j);
                        }

                        //System.Diagnostics.Trace.TraceInformation("{0} Write DBG - getWriteListLimitate 2 - writeSize = {1}",
                        //                                         DateTime.UtcNow.ToString(), lastWriteSize);
                        return (frameSize.TotalNrJobs == 0);
                    }
                }
                //nextlistLimitate.Add(j);
                j.PartialArrayEnd = 0;

                return true;
                // Allow writing just one tag per time
            //    break;
            //}

            //System.Diagnostics.Trace.TraceInformation("{0} Write DBG - getWriteListLimitate 3 - writeSize = {1}",
            //                                         DateTime.UtcNow.ToString(), writeSize);
        }


        //public static void getWriteListLimitate(List<OmronEthernetIPCommJob> nextlist, ref List<CommJob> nextlistLimitate)
        //{
        //    ushort writeSize = LOGIX5550_CONNECTION_SIZE_HEADER;
        //    //ushort lastWriteSize = 0;
        //    uint pduMaxSize = 0;
        //    if(nextlist.Count > 0)
        //    {
        //        OmronEthernetIPStation jobStation = (OmronEthernetIPStation)nextlist[0].Station;
        //        pduMaxSize = jobStation.MaxPduSize;
        //    }
        //    foreach (OmronEthernetIPCommJob j in nextlist)
        //    {
        //        foreach (Tag tag in j.TagsList)
        //        {
        //            //lastWriteSize = writeSize;
        //            writeSize += j.GetTagReadBufferSize(tag);
        //            if(j.TagNameRequestBuffer != null)
        //            {
        //                writeSize--;
        //            }
        //            writeSize +=8;
        //            writeSize += (ushort)(j.adjSizeWrite(tag) - j.PartialArrayStart * j.ElemSize);
        //            if (writeSize > pduMaxSize)
        //            {
        //                if (tag.Size / j.ElemSize <= 1)
        //                {
        //                    //System.Diagnostics.Trace.TraceInformation("{0} Write DBG - getWriteListLimitate 1 - writeSize = {1}",
        //                    //                                         DateTime.UtcNow.ToString(), lastWriteSize);
        //                    return;
        //                }
        //                int tmp = (j.adjSizeWrite(tag) + (int)pduMaxSize + 1 - writeSize - j.ElemSize) / j.ElemSize;
        //                if (tmp > 0)
        //                {
        //                    j.PartialArrayEnd = (ushort)(tmp + j.PartialArrayStart);
        //                    nextlistLimitate.Add(j);
        //                }

        //                //System.Diagnostics.Trace.TraceInformation("{0} Write DBG - getWriteListLimitate 2 - writeSize = {1}",
        //                //                                         DateTime.UtcNow.ToString(), lastWriteSize);
        //                return;
        //            }
        //        }
        //        nextlistLimitate.Add(j);
        //        j.PartialArrayEnd = 0;
        //        // Allow writing just one tag per time
        //        break;
        //    }

        //    //System.Diagnostics.Trace.TraceInformation("{0} Write DBG - getWriteListLimitate 3 - writeSize = {1}",
        //    //                                         DateTime.UtcNow.ToString(), writeSize);
        //}

        public static ushort Logix5550TagsPrepareMultiWriteReq(
            ref List<CommJob> List, ref OmronEthernetIPStation s,
            ref byte[] pdu, ref ushortUnion pduPointer)
        {
            //System.Diagnostics.Trace.TraceInformation("{0} Write DBG - Logix5550TagsPrepareMultiWriteReq - Job in List: {1}",
            //                                          DateTime.UtcNow.ToString(), List.Count);

            ushort StartCh = pduPointer.USHORT;
            s.lastProcessedTags.USHORT = (ushort)List.Count;
            ushortUnion OffsetIndex = new ushortUnion(pduPointer.USHORT);
            pduPointer.USHORT += (ushort)(s.lastProcessedTags.USHORT * 2);
            ushortUnion OffsetValue = new ushortUnion((ushort)(2 + pduPointer.USHORT - StartCh));

            foreach (OmronEthernetIPCommJob j in List)
            {
                Tag tag = j.GetFirstTagToWrite();
                if (tag == null)
                    return 0;

                // Set the service request offset
                pdu[OffsetIndex.USHORT++] = OffsetValue.LOBYTE;
                pdu[OffsetIndex.USHORT++] = OffsetValue.HIBYTE;

                // Set the service request
                pdu[pduPointer.USHORT++] = 0x4D;    // Service code = write
                OffsetValue.USHORT++;
                OffsetValue.USHORT += j.GetAsciiTagname(s, ref pdu, ref pduPointer, tag);

                OffsetValue.USHORT += j.GetTagFormat(ref pdu, ref pduPointer);

                // Number of elements to be written
                OffsetValue.USHORT += j.GetWriteNumOfElements(ref pdu, ref pduPointer, tag);

                ushort startData = pduPointer.USHORT;
                // Add data
                ushort WriteLenght = j.GetTagWriteData(ref pdu, ref pduPointer, tag);
                // no data to write --> inputoutput/exception output with same value of driver

                if (((WriteLenght == 0) && (j.TagFormat != TagFormats.STRING)) || 
                    ((WriteLenght == 0xFFFF) && (j.TagFormat == TagFormats.STRING)))
                { 
                    return 0;
                }

                // Add data
                OffsetValue.USHORT += WriteLenght;

                ushort endData = pduPointer.USHORT;

                //if (j.PartialArrayEnd == 0)
                //{
                //    j.MoveFromToWriteToOnWriting(tag);
                //}
            }

            // number of jobs
            return (s.lastProcessedTags.USHORT);
        }

        public static bool getReadListLimitate(ref OmronEthernetIPCommJob j, ref ReadWriteListLimitateSize frameSize, ReadWriteLimitate mode)
        {
            OmronEthernetIPStation s = j.Station as OmronEthernetIPStation;
            OmronEthernetIPChannel ch = (OmronEthernetIPChannel)s.GetChannel();
            if (ch.FragmentProtocolCanBeApplied(j))
            {
                return (getReadListLimitateForFragmentProtocol(ref j, ref frameSize, mode));
            }

            if (frameSize.IsEmpty())
            {
                //frameSize.TotalSize = (ushort)(LOGIX5550_CONNECTION_SIZE_HEADER);
                //frameSize.ResponseSize = (ushort)(LOGIX5550_CONNECTION_SIZE_HEADER - 2);
                frameSize.TotalSize = MULTIPLE_SERVICE_REQUEST_FIX_OVERHEAD + CIP_SEQUENCE_COUNT_SIZE;
                frameSize.ResponseSize = MULTIPLE_SERVICE_REPLY_FIX_OVERHEAD + CIP_SEQUENCE_COUNT_SIZE;
            }

            uint pduMaxSize = ((OmronEthernetIPStation)j.Station).MaxPduSize;

            //foreach (OmronEthernetIPCommJob j in nextlist)
            //{
                ushort TagIndex = 0;
                j.ReadTagEnd = j.ReadTagStart;
                foreach (Tag tag in j.TagsList)
                {
                    if (TagIndex >= j.ReadTagStart)
                    {
                        frameSize.TotalSize += j.GetTagReadBufferSize(tag);
                        frameSize.TotalSize += 6;
                        //if (readSize > LOGIX5550_CONNECTION_SIZE)
                        if (frameSize.TotalSize > pduMaxSize)
                        {
                            if (j.ReadTagStart != j.ReadTagEnd)
                            {
                                //nextlistLimitate.Add(j);
                            }
                            return (frameSize.TotalNrJobs == 0); //return;
                        }
                        frameSize.ResponseSize += 8;
                        if((uint)tag.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                        {
                            // In case of boolean variables, the response contains two data bytes: Status (TRUE/FALSE) and Forced set/reset Information (Forced/Not Forced)
                            frameSize.ResponseSize += (ushort)((j.adjSize(tag) - j.PartialArrayStart * j.ElemSize) * 2);
                        }
                        else if ((uint)tag.TagNode.DataType.Identifier == (uint)BuiltInType.String)
                        {
                            // In case of string variables, the response contains also 2 bytes for the length of the string
                            frameSize.ResponseSize += (ushort)(j.adjSize(tag) - j.PartialArrayStart * j.ElemSize + 2);
                        }
                        else
                        {
                            frameSize.ResponseSize += (ushort)(j.adjSize(tag) - j.PartialArrayStart * j.ElemSize);
                        }
                        //if (responseSize > LOGIX5550_CONNECTION_SIZE)
                        if (frameSize.ResponseSize > pduMaxSize)
                        {
                            if (j.ReadTagEnd != j.ReadTagStart)
                            {
                                //nextlistLimitate.Add(j);
                                return (frameSize.TotalNrJobs == 0); //return;
                            }
                            if (tag.Size / j.ElemSize <= 1)
                                return false;
                            //int tmp = (j.adjSize(tag) + LOGIX5550_CONNECTION_SIZE + 1 - responseSize - j.ElemSize) / j.ElemSize;
                            int tmp = (j.adjSize(tag) + (int)pduMaxSize + 1 - frameSize.ResponseSize - j.ElemSize) / j.ElemSize;
                            if (mode == ReadWriteLimitate.Add)
                                tmp -= j.PartialArrayStart;
                            if (tmp > 0)
                            {                                
                                if (mode == ReadWriteLimitate.Add)
                                    j.PartialArrayEnd = (ushort)(tmp + j.PartialArrayStart);
                            }
                            return (frameSize.TotalNrJobs == 0);  //return;
                        }
                        if (mode == ReadWriteLimitate.Add)
                            j.ReadTagEnd++;
                    }
                    TagIndex++;
                }
                //nextlistLimitate.Add(j);
                j.ReadTagEnd = 0;
                j.PartialArrayEnd = 0;
            //}
            return true;
        }

        public static bool getReadListLimitateForFragmentProtocol(ref OmronEthernetIPCommJob j, ref ReadWriteListLimitateSize frameSize, ReadWriteLimitate mode)
        {
            if (frameSize.IsEmpty())
            {
                frameSize.TotalSize = FRAGMENT_REQUEST_FIX_OVERHEAD + CIP_SEQUENCE_COUNT_SIZE;
                frameSize.ResponseSize = FRAGMENT_REPLY_FIX_OVERHEAD + CIP_SEQUENCE_COUNT_SIZE;
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - getReadListLimitateForFragmentProtocol - step 1 - frameSize.TotalSize: {1} - frameSize.ResponseSize: {2} - frameSize.TotalNrJobs: {3}",
                                                       currentTime, frameSize.TotalSize, frameSize.ResponseSize, frameSize.TotalNrJobs));
                }
#endif
            }

            //uint replyPduMaxSize = ((OmronEthernetIPStation)j.Station).MaxPduSize + ENCAPSULATION_HEADER_SIZE + REPLY_CIP_SERVICE_OFFSET;
            uint replyPduMaxSize = ((OmronEthernetIPStation)j.Station).MaxPduSize;

            uint requestPduMaxSize = FRAGMENT_MAXPDUSIZE;

            ushort TagIndex = 0;
            j.ReadTagEnd = j.ReadTagStart;
            foreach (Tag tag in j.TagsList)
            {
                if (TagIndex >= j.ReadTagStart)
                {
                    frameSize.TotalSize += j.GetTagFragmentedReadBufferSize(tag);
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - getReadListLimitateForFragmentProtocol - step 2 - frameSize.TotalSize: {1} - frameSize.ResponseSize: {2} - frameSize.TotalNrJobs: {3}",
                                                           currentTime, frameSize.TotalSize, frameSize.ResponseSize, frameSize.TotalNrJobs));
                    }
#endif

                    if (frameSize.TotalSize > requestPduMaxSize)
                    {
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - getReadListLimitateForFragmentProtocol - frameSize.TotalSize > requestPduMaxSize - frameSize.TotalSize: {1} - frameSize.ResponseSize: {2} - frameSize.TotalNrJobs: {3}",
                                                               currentTime, frameSize.TotalSize, frameSize.ResponseSize, frameSize.TotalNrJobs));
                        }
#endif
                        return (frameSize.TotalNrJobs == 0); //return;
                    }

                    frameSize.ResponseSize += 4;
#if DEBUG
                    {
                        string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                        System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - getReadListLimitateForFragmentProtocol - step 3 - frameSize.TotalSize: {1} - frameSize.ResponseSize: {2} - frameSize.TotalNrJobs: {3}",
                                                           currentTime, frameSize.TotalSize, frameSize.ResponseSize, frameSize.TotalNrJobs));
                    }
#endif

                    if ((uint)tag.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                    {
                        // In case of boolean variables, the response contains two data bytes: Status (TRUE/FALSE) and Forced set/reset Information (Forced/Not Forced)
                        frameSize.ResponseSize += (ushort)((j.adjSize(tag) - j.PartialArrayStart * j.ElemSize) * 2);
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - getReadListLimitateForFragmentProtocol - step 4 - frameSize.TotalSize: {1} - frameSize.ResponseSize: {2} - frameSize.TotalNrJobs: {3}",
                                                               currentTime, frameSize.TotalSize, frameSize.ResponseSize, frameSize.TotalNrJobs));
                        }
#endif
                    }
                    else if ((uint)tag.TagNode.DataType.Identifier == (uint)BuiltInType.String)
                    {
                        // In case of string variables, the response contains also 2 bytes for the length of the string
                        frameSize.ResponseSize += (ushort)(j.adjSize(tag) - j.PartialArrayStart * j.ElemSize + 2);
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - getReadListLimitateForFragmentProtocol - step 5 - frameSize.TotalSize: {1} - frameSize.ResponseSize: {2} - frameSize.TotalNrJobs: {3}",
                                                               currentTime, frameSize.TotalSize, frameSize.ResponseSize, frameSize.TotalNrJobs));
                        }
#endif
                    }
                    else
                    {
                        frameSize.ResponseSize += (ushort)(j.adjSize(tag) - j.PartialArrayStart * j.ElemSize);
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - getReadListLimitateForFragmentProtocol - step 6 - frameSize.TotalSize: {1} - frameSize.ResponseSize: {2} - frameSize.TotalNrJobs: {3}",
                                                               currentTime, frameSize.TotalSize, frameSize.ResponseSize, frameSize.TotalNrJobs));
                        }
#endif
                    }

                    if (frameSize.ResponseSize > replyPduMaxSize)
                    {
                        if (j.ReadTagEnd != j.ReadTagStart)
                        {
                            return (frameSize.TotalNrJobs == 0); //return;
                        }
                        if (tag.Size / j.ElemSize <= 1)
                            return false;
                        int tmp = (j.adjSize(tag) + (int)replyPduMaxSize + 1 - frameSize.ResponseSize - j.ElemSize) / j.ElemSize;
                        if (mode == ReadWriteLimitate.Add)
                            tmp -= j.PartialArrayStart;
                        if (tmp > 0)
                        {
                            if (mode == ReadWriteLimitate.Add)
                                j.PartialArrayEnd = (ushort)(tmp + j.PartialArrayStart);
                        }
#if DEBUG
                        {
                            string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                            System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - getReadListLimitateForFragmentProtocol - frameSize.ResponseSize > replyPduMaxSize - frameSize.TotalSize: {1} - frameSize.ResponseSize: {2} - frameSize.TotalNrJobs: {3}",
                                                               currentTime, frameSize.TotalSize, frameSize.ResponseSize, frameSize.TotalNrJobs));
                        }
#endif
                        return (frameSize.TotalNrJobs == 0);  //return;
                    }
                    if (mode == ReadWriteLimitate.Add)
                        j.ReadTagEnd++;
                }
                TagIndex++;
            }

            j.ReadTagEnd = 0;
            j.PartialArrayEnd = 0;
#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - getReadListLimitateForFragmentProtocol - return true - frameSize.TotalSize: {1} - frameSize.ResponseSize: {2} - frameSize.TotalNrJobs: {3}",
                                                   currentTime, frameSize.TotalSize, frameSize.ResponseSize, frameSize.TotalNrJobs));
            }
#endif
            return true;
        }

        public static ushort GetCompleteVariableListForFragmentRequest(ref List<CommJob> list, ref byte[] dataBuffer, ref ushort totalTagNumber)
        {
            totalTagNumber = 0;
            ushort byteCount = 0;
            ushortUnion bufferIndex = new ushortUnion(0);
            foreach (OmronEthernetIPCommJob j in list)
            {
                foreach (var tag in j.TagsList)
                {
                    byteCount += j.GetFragmentedAsciiTagname(ref dataBuffer, ref bufferIndex, tag);
                    totalTagNumber++;
                }
            }

            return (bufferIndex.USHORT);
        }

        public static ushort PrepareConnectedFragmentedReadRequest(OmronEthernetIPChannel c, OmronEthernetIPStation s, ref byte[] pdu, ref ushortUnion pduIndex, ref OmronEthernetIPFragmentInfo fragmentInfo)
        {
            ushort startIndex = pduIndex.USHORT;

            // Encapsulation header
            PrepareEncapsulationHeader(ecSendUnitData, c, ref pdu, ref pduIndex);

            // Request header
            Logix5550PrepareRequestHeader(s, ref pdu, ref pduIndex);

            // The request length must be filled later 
            ushort requestLengthIndex = pduIndex.USHORT;
            pdu[pduIndex.USHORT++] = 0;
            pdu[pduIndex.USHORT++] = 0;

            // Get the next fragment data request to be sent
            byte[] nextFragmentBuffer = null;
            bool isTheLastFragment = false;
            ushort nextFragmentBufferLength = fragmentInfo.GetNextFragmentBuffer(ref nextFragmentBuffer, ref isTheLastFragment);
            if (nextFragmentBufferLength == 0)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - PrepareFragmentedReadRequest - Error in GetNextFragmentBuffer",
                                                       currentTime));
                }
#endif
                return (0);
            }

#if DEBUG
            {
                string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - PrepareFragmentedReadRequest - nextFragmentBufferLength: {1} - fragmentInfo.MaxLengthOfASingleFragment: {2}",
                                                   currentTime, nextFragmentBufferLength, fragmentInfo.MaxLengthOfASingleFragment));
            }
#endif

            // Sequence Count
            pdu[pduIndex.USHORT++] = s.GetTransaction().LOBYTE;
            pdu[pduIndex.USHORT++] = s.GetTransaction().HIBYTE;

            // Add the fragment of the the variable name list to the request message
            ushort encapsulatedFragmentRequestLength = PrepareEncapsulatedFragmentRequest(ref pdu, ref pduIndex, ref nextFragmentBuffer, nextFragmentBufferLength, isTheLastFragment, ref fragmentInfo);
#if DEBUG
            if (encapsulatedFragmentRequestLength == 0)
            {
#if DEBUG
                {
                    string currentTime = DateTime.Now.ToString("HH:mm:ss.fff");
                    System.Diagnostics.Debug.WriteLine(String.Format("OMRON EthernetIP DBG - {0} - PrepareFragmentedReadRequest - Error in PrepareEncapsulatedFragmentRequest",
                                                       currentTime));
                }
#endif
                return (0);
            }
#endif
            // Fill the length field of the message
            ushort requestLength = (ushort)(encapsulatedFragmentRequestLength + 2);
            pdu[requestLengthIndex] = (byte)(requestLength & 0xff);
            pdu[requestLengthIndex + 1] = (byte)(requestLength >> 8);

            return ((ushort)(pduIndex.USHORT - startIndex));
        }

        public static ushort PrepareEncapsulatedFragmentRequest(ref byte[] pdu, ref ushortUnion pduIndex, ref byte[] fragmentBuffer, ushort fragmentBufferLength, bool isTheLastFragment, ref OmronEthernetIPFragmentInfo fragmentInfo)
        {
            ushort StartCh = pduIndex.USHORT;

            PrepareGetVariablesMultipleHeader(ref pdu, ref pduIndex);


            // Handle Number
            pdu[pduIndex.USHORT++] = (byte)(fragmentInfo.Handle & 0xff);
            pdu[pduIndex.USHORT++] = (byte)(fragmentInfo.Handle >> 8);

            if (isTheLastFragment)
            {
                // Special case: only one fragment
                if (fragmentInfo.FType == FragmentType.First)
                {
                    fragmentInfo.SequenceNumber = OmronEthernetIPProtocol.FRAGMENT_MAX_SEQUENCE_NUMBER;
                }
                else
                {
                    fragmentInfo.FType = FragmentType.Last;
                }
            }

            // Sequential ID
            ushort sequentialID = fragmentInfo.GetRequestSequenceID();
            pdu[pduIndex.USHORT++] = (byte)(sequentialID & 0xff);
            pdu[pduIndex.USHORT++] = (byte)(sequentialID >> 8);

            // Additional fields present only in the first fragment request
            if (fragmentInfo.FType == FragmentType.First)
            {
                // Access Watch Timer
                pdu[pduIndex.USHORT++] = 0;
                pdu[pduIndex.USHORT++] = 0;

                // CRC
                pdu[pduIndex.USHORT++] = 0;
                pdu[pduIndex.USHORT++] = 0;

                // Number of variable names
                pdu[pduIndex.USHORT++] = (byte)(fragmentInfo.TotalNumberOfVariables & 0xff);
                pdu[pduIndex.USHORT++] = (byte)(fragmentInfo.TotalNumberOfVariables >> 8);
            }

            // Add the list of the variables names (may be a part of it)
            Array.Copy(fragmentBuffer, 0, pdu, pduIndex.USHORT, fragmentBufferLength);
            pduIndex.USHORT += fragmentBufferLength;

            // Eventually, set the type of the fragment to "the last one"
            if (isTheLastFragment && (fragmentInfo.FType != FragmentType.Last))
            {
                fragmentInfo.FType = FragmentType.Last;
            }

            return ((ushort)(pduIndex.USHORT - StartCh));
        }

        private static void PrepareGetVariablesMultipleHeader(ref byte[] pdu, ref ushortUnion pduIndex)
        {
            // Service: Get Variables Multiple
            pdu[pduIndex.USHORT++] = 0x50;

            // Path Size: 2 words
            pdu[pduIndex.USHORT++] = 2;

            // Request Path: Variable Monitoring Object, Instance 0
            pdu[pduIndex.USHORT++] = 0x20; // Class Segment
            pdu[pduIndex.USHORT++] = 0x6d; // Variable Monitoring
            pdu[pduIndex.USHORT++] = 0x24; // Instance Segment
            pdu[pduIndex.USHORT++] = 0; // Instance ID
        }

        /// <summary>
        /// Calculate frame's data size depending of plc type
        /// </summary>
        /// <param name="plcType"></param>
        /// <returns></returns>
        public static ushort getReadFrameDataSize(PlcTypes plcType)
        {
            return (ushort)(OmronEthernetIPProtocol.GetMaxPduSize(plcType) - (OmronEthernetIPProtocol.LOGIX5550_RESPONSE_SIZE_HEADER + 8));
        }

        private static ushort numTagsListToRead(List<CommJob> List)
        {
            ushort i = 0;
            foreach (OmronEthernetIPCommJob j in List)
            {
                ushort tagIndex = 0;
                foreach (Tag tag in j.TagsList)
                {
                    if((j.ReadTagEnd == 0 && j.ReadTagStart == 0) || (tagIndex < j.ReadTagEnd && tagIndex >= j.ReadTagStart) || (j.ReadTagEnd == 0 && j.ReadTagStart > 0 && tagIndex >= j.ReadTagStart))
                    {
                        i++;
                    }
                    tagIndex++;
                }
            }
            return i;
        }

        public static ushort Logix5550TagsPrepareMultiReadReq(ref List<CommJob> List, ref OmronEthernetIPStation s,
                                                              ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort StartCh = pduPointer.USHORT;

            s.lastProcessedTags.USHORT = numTagsListToRead(List);

            ushortUnion OffsetIndex = new ushortUnion(pduPointer.USHORT);

            pduPointer.USHORT += (ushort)(s.lastProcessedTags.USHORT * 2);

            ushortUnion OffsetValue = new ushortUnion((ushort)(2 + pduPointer.USHORT - StartCh));

            foreach (OmronEthernetIPCommJob j in List)
            {
                ushort tagIndex = 0;
                foreach (var tag in j.TagsList)
                {
                    if ((j.ReadTagEnd == 0 && j.ReadTagStart == 0) || (tagIndex < j.ReadTagEnd && tagIndex >= j.ReadTagStart) || (j.ReadTagEnd == 0 && j.ReadTagStart > 0 && tagIndex >= j.ReadTagStart))
                    {
                        // Set the service request offset
                        pdu[OffsetIndex.USHORT++] = OffsetValue.LOBYTE;
                        pdu[OffsetIndex.USHORT++] = OffsetValue.HIBYTE;

                        // Service code = read
                        pdu[pduPointer.USHORT++] = 0x4C;
                        OffsetValue.USHORT++;
                        OffsetValue.USHORT += j.GetAsciiTagname(s,ref  pdu, ref  pduPointer, tag);

                        // Number of elements to be read
                        OffsetValue.USHORT += j.GetNumOfElements(ref  pdu, ref  pduPointer, tag);
                    }
                    tagIndex++;
                }
            }


            // number of Tags
            return (s.lastProcessedTags.USHORT);
        }

        public static ushort PrepareEncapsulateRequest(
            ref List<PrepareHeader> prepareHeaderList, PrepareBody prepareBody,
            ref List<CommJob> ListJob,ref OmronEthernetIPStation s,
            ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort StartCh = pduPointer.USHORT;
            ushortUnion DataLength = new ushortUnion(0);

            if (prepareHeaderList.Count > 0)
            {
                PrepareHeader prepareHeader = prepareHeaderList[0];
                prepareHeader(s, ref pdu, ref pduPointer);

                //Data Item - Item Data Length = variable (filled up later)
                ushort BodyStart = pduPointer.USHORT;
                pduPointer.USHORT += 2;

                prepareHeaderList.RemoveAt(0);

                DataLength.USHORT = PrepareEncapsulateRequest(ref prepareHeaderList, prepareBody, ref ListJob,ref s, ref pdu, ref pduPointer);

                //Fill up data length
                pdu[BodyStart] = DataLength.LOBYTE;
                pdu[BodyStart + 1] = DataLength.HIBYTE;
            }
            else
            {
                return prepareBody(ref ListJob, ref s, ref pdu, ref pduPointer);
            }

            if (DataLength.USHORT == 0 || s.lastProcessedTags.USHORT == 0)
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(String.Format("{0} - Failure 1 in PrepareEncapsulateRequest, DataLength.USHORT = {1}, s.lastProcessedTags.USHORT = {2}", DateTime.UtcNow.ToString(), DataLength.USHORT, s.lastProcessedTags.USHORT));
#endif
                return 0;
            }

            return ((ushort)(pduPointer.USHORT - StartCh));
        }

        public static bool ParseData(byte[] receivedbuffer, ref OmronEthernetIPCommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            uint recivedSize;

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
                bt != BuiltInType.UInteger)
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

            if (job.TotalJobSize > receivedbuffer.Length)
            {
                recivedSize = (uint)receivedbuffer.Length;
            }
            else
            {
                recivedSize = job.TotalJobSize;
            }

            byte[] tempBuffer = new byte[recivedSize];

            if((job.DataFormat == DataFormats.BIT) &&
                (job.TagsList[0].TagNode.ArrayDimension > 0) &&
                (!job.ABAddress[job.ABAddress.Length - 1].Equals(']')))
            {
                for (ushort bitCnt = 0; bitCnt < recivedSize; bitCnt++)
                {
                    tempBuffer[bitCnt] = (byte)((receivedbuffer[bitCnt / 8] >> (bitCnt % 8)) & 1);
                }
            }
            else
            {
                Array.Copy(receivedbuffer, tempBuffer, recivedSize);
            }

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

        public static string GetNodeTree(NodeId NodeId,string Name)
        {
            string Out = "";
            //Case 18633 Problem on structures splited.(@"^[\d]+:(?<Name>[\w]+)$")
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

        public static string GetDynTagTree(OmronEthernetIPDynTagSettings dynSetting)
        {
            string Out = "";

            string nameNodeId = dynSetting.ABAddress;
            Regex NodeNameParser = new Regex(@"^[^?]+[?](?<NodeName>[\w/]+)$");
            Match NodeNameMatch = NodeNameParser.Match(nameNodeId);
            if (NodeNameMatch.Success)
            {
                Out = NodeNameMatch.Groups["NodeName"].Value;
            }
            return Out;

        }

        public static string getOffTree(ref string Tree)
        {
            string Root = "";

            Regex NodeNameParser = new Regex(@"^(?<Root>[\w]+)(?<Tree>/[\w/]+)?$");
            Match NodeNameMatch = NodeNameParser.Match(Tree);
            if (NodeNameMatch.Success)
            {
                Root = NodeNameMatch.Groups["Root"].Value;
                Tree = NodeNameMatch.Groups["Tree"].Value;
                if (!string.IsNullOrEmpty(Tree))
                    Tree = NodeNameMatch.Groups["Tree"].Value.Remove(0, 1);

            }
            return Root;

        }

        public static UFUAModel.DataType DataType(DataFormats DataFormat)
        {
            switch (DataFormat)
            {
                case DataFormats.BIT:
                    return UFUAModel.DataType.Boolean;
                case DataFormats.BYTE:
                    return UFUAModel.DataType.Byte;
                case DataFormats.WORD:
                    return UFUAModel.DataType.UInt16;
                case DataFormats.DWORD:
                    return UFUAModel.DataType.UInt32;
                case DataFormats.SBYTE:
                    return UFUAModel.DataType.SByte;
                case DataFormats.SWORD:
                    return UFUAModel.DataType.Int16;
                case DataFormats.SDWORD:
                    return UFUAModel.DataType.Int32;
                case DataFormats.FLOAT:
                    return UFUAModel.DataType.Float;
                case DataFormats.DOUBLE:
                    return UFUAModel.DataType.Double;
                case DataFormats.ULINT:
                case DataFormats.LWORD:
                    return UFUAModel.DataType.UInt64;
                case DataFormats.LINT:
                    return UFUAModel.DataType.Int64;
                default:
                    return 0;
            }
        }

        public static uint GetMaxPduSize(PlcTypes plcType)
        {
            uint MaxPduSize = OmronEthernetIPProtocol.MAX_DATA_SIZE;
            switch (plcType)
            {
                case PlcTypes.NJ:
                    MaxPduSize = PLCTYPE_NJ_MAXPDUSIZE;
                    break;
                case PlcTypes.NX:
                    MaxPduSize = PLCTYPE_NX_MAXPDUSIZE;
                    break;
                default:
                    MaxPduSize = PLCTYPE_OTHER_MAXPDUSIZE;
                    break;
            }
            return MaxPduSize;
        }

        #endregion

    }

}
