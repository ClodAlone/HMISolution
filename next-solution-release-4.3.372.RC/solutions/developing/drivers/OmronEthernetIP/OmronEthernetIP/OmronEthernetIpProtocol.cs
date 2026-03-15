using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using System.Runtime.InteropServices;
using Opc.Ua;
using System.Text.RegularExpressions;
using DriverCodeBase.Enumerators;

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
        ErrorPathSegmentError,
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

        #endregion

        #region static methods

        public delegate void PrepareHeader(
            OmronEthernetIPStation s,
            ref byte[] pdu, ref ushortUnion pduPointer);

        public delegate ushort PrepareBody(
            ref List<OmronEthernetIPCommJob> List,
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

            c.InvalidateStationConnections();            
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
                                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                                        OmronEthernetIP.Properties.Resources.ErrorBufferIsShort,
                                                                        Opc.Ua.EventSeverity.High);
                                        return (s.ConnectionIDSet);
                                    }
                                    pduData.CopyTo(pdu, OmronEthernetIPProtocol.ENCAPSULATION_HEADER_SIZE);
                                    // Analyze the reply header
                                    OmronEthernetIPErrorCodes ErrorAnalyzeReplyHeader = c.AnalyzeReplyHeader(ref pdu, ecSendRRData.USHORT, c.SessionHandle.UINT);
                                    if (ErrorAnalyzeReplyHeader == (OmronEthernetIPErrorCodes)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
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
                                                                            string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                                            OmronEthernetIP.Properties.Resources.ErrorUnexpectedReply,
                                                                            Opc.Ua.EventSeverity.High);
                                        }
                                    }
                                    else
                                    {
                                        c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorAnalyzeReplyHeader, ErrorAnalyzeReplyHeader),
                                                                        Opc.Ua.EventSeverity.High);
                                    }
                                }
                                else
                                {
                                    c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                            string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                          OmronEthernetIP.Properties.Resources.ErrorEXTSTS23,
                                            Opc.Ua.EventSeverity.High);
                                }
                            }
                        }
                        else
                        {
                            c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                      OmronEthernetIP.Properties.Resources.ErrorEXTSTS23,
                                        Opc.Ua.EventSeverity.High);
                        }
                    }
                    else
                    {
                        c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                        OmronEthernetIP.Properties.Resources.ErrorDeviceWriteKo,
                                        Opc.Ua.EventSeverity.High);
                    }
                    c.ReceiveClear();
                }
            }
            else
            {
                c.getCommDriver().OnSystemEvent(ObjectIds.Server,
                                                string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                OmronEthernetIP.Properties.Resources.ErrorSessionRegisterKo,
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
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                        string.Format(OmronEthernetIP.Properties.Resources.ErrorForwardLOpen_CodeExtended, 
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
                                                string.Format(OmronEthernetIP.Properties.Resources.ErrorConnectionFailed, s.Name) +
                                                string.Format(OmronEthernetIP.Properties.Resources.ErrorForwardOpen_Code, pdu[OmronEthernetIPProtocol.MR_SVC_REPLY_GENSTS_OFFS]),
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
                                        (OmronEthernetIPErrorCodes)DriverCodeBase.Enumerators.DriverErrorCodes.ErrorNoError)
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
            //// Connection is open?
            //if (c.SessionHandle.UINT == 0 || !s.ConnectionIDSet)
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
                s.ClearConnection();
            }

            return !s.ConnectionIDSet;
        }

        public static ushort Logix5550PrepareRequest(ref List<OmronEthernetIPCommJob> list, OmronEthernetIPChannel c, ref OmronEthernetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            bool isReading = true;
            PrepareBody prepareBody;
            if ((list[0].CommandType == CommandTypes.ReadCmd) || (list[0].CommandType == CommandTypes.ReadDataFormatCmd))
            {
                prepareBody = Logix5550TagsPrepareMultiReadReq;
            }
            else if (list[0].CommandType == CommandTypes.WriteCmd)
            {
                isReading = false;
                prepareBody = Logix5550TagsPrepareMultiWriteReq;
            }
            else
            {
#if DEBUG
                System.Diagnostics.Debug.WriteLine(String.Format("{0} - Failure 1 in Logix5550PrepareRequest job: {1}", DateTime.UtcNow.ToString(), list[0].ABAddress));
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

        public static void getWriteListLimitate(List<OmronEthernetIPCommJob> nextlist, ref List<OmronEthernetIPCommJob> nextlistLimitate)
        {
            ushort writeSize = LOGIX5550_CONNECTION_SIZE_HEADER;
            //ushort lastWriteSize = 0;
            uint pduMaxSize = 0;
            if(nextlist.Count > 0)
            {
                OmronEthernetIPStation jobStation = (OmronEthernetIPStation)nextlist[0].Station;
                pduMaxSize = jobStation.MaxPduSize;
            }
            foreach (OmronEthernetIPCommJob j in nextlist)
            {
                foreach (Tag tag in j.TagsList)
                {
                    //lastWriteSize = writeSize;
                    writeSize += j.GetTagReadBufferSize(tag);
                    writeSize +=8;
                    if ((uint)tag.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                    {
                        // In case of boolean variables, the request contains two data bytes: Status (TRUE/FALSE) and Forced set/reset Information (Forced/Not Forced)
                        writeSize += (ushort)((j.adjSizeWrite(tag) - j.PartialArrayStart * j.ElemSize) * 2);
                    }
                    else if ((uint)tag.TagNode.DataType.Identifier == (uint)BuiltInType.String)
                    {
                        // In case of string variables, the request contains also 2 bytes for the length of the string
                        writeSize += (ushort)(j.adjSizeWrite(tag) - j.PartialArrayStart * j.ElemSize + 2);
                    }
                    else
                    {
                        writeSize += (ushort)(j.adjSizeWrite(tag) - j.PartialArrayStart * j.ElemSize);
                    }
                    if (writeSize > pduMaxSize)
                    {
                        if (tag.Size / j.ElemSize <= 1)
                        {
                            return;
                        }
                        int tmp = (j.adjSizeWrite(tag) + (int)pduMaxSize + 1 - writeSize - j.ElemSize) / j.ElemSize;
                        tmp -= j.PartialArrayStart;
                        if (tmp > 0)
                        {
                            j.PartialArrayEnd = (ushort)(tmp + j.PartialArrayStart);
                            nextlistLimitate.Add(j);
                        }

                        return;
                    }
                }
                nextlistLimitate.Add(j);
                j.PartialArrayEnd = 0;
                // Allow writing just one tag per time
                break;
            }

        }

        public static ushort Logix5550TagsPrepareMultiWriteReq(
            ref List<OmronEthernetIPCommJob> List, ref OmronEthernetIPStation s,
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
                Tag tag;
                lock (j.retLockList())
                {
                    if (j.Type == DriverCodeBase.Enumerators.LinkType.UnconditionalOutput)
                    {
                        if (j.TagsListToWrite.Count == 0)
                        {
                            j.TagsListToWrite.AddRange(j.TagsList);
                        }
                    }

                    if (j.TagsListToWrite.Count == 0)
                    {
                        //System.Diagnostics.Trace.TraceInformation("{0} Write DBG - Logix5550TagsPrepareMultiWriteReq - Error 1",
                        //                                          DateTime.UtcNow.ToString());
                        return 0;
                    }
                    tag = j.TagsListToWrite[0];
                }

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

                if (j.PartialArrayEnd == 0)
                {
                    lock (j.retLockList())
                    {
                        j.TagsListToWrite.Remove(tag);
                        if (!j.TagsListOnWriting.Contains(tag))
                            j.TagsListOnWriting.Add(tag);
                    }
                }
            }

            // number of jobs
            return (s.lastProcessedTags.USHORT);
        }

        public static void getReadListLimitate(List<OmronEthernetIPCommJob> nextlist, ref List<OmronEthernetIPCommJob> nextlistLimitate)
        {
            ushort readSize = (ushort)(LOGIX5550_CONNECTION_SIZE_HEADER);
            ushort responseSize = (ushort)(LOGIX5550_CONNECTION_SIZE_HEADER - 2);
            uint pduMaxSize = 0;
            if (nextlist.Count > 0)
            {
                OmronEthernetIPStation jobStation = (OmronEthernetIPStation)nextlist[0].Station;
                pduMaxSize = jobStation.MaxPduSize;
            }

            foreach (OmronEthernetIPCommJob j in nextlist)
            {
                ushort TagIndex = 0;
                j.ReadTagEnd = j.ReadTagStart;
                foreach (Tag tag in j.TagsList)
                {
                    if (TagIndex >= j.ReadTagStart)
                    {
                        readSize += j.GetTagReadBufferSize(tag);
                        readSize += 6;
                        //if (readSize > LOGIX5550_CONNECTION_SIZE)
                        if (readSize > pduMaxSize)
                        {
                            if (j.ReadTagStart != j.ReadTagEnd)
                                nextlistLimitate.Add(j);
                            return;
                        }
                        responseSize += 8;
                        if((uint)tag.TagNode.DataType.Identifier == (uint)BuiltInType.Boolean)
                        {
                            // In case of boolean variables, the response contains two data bytes: Status (TRUE/FALSE) and Forced set/reset Information (Forced/Not Forced)
                            responseSize += (ushort)((j.adjSize(tag) - j.PartialArrayStart * j.ElemSize) * 2);
                        }
                        else if ((uint)tag.TagNode.DataType.Identifier == (uint)BuiltInType.String)
                        {
                            // In case of string variables, the response contains also 2 bytes for the length of the string
                            responseSize += (ushort)(j.adjSize(tag) - j.PartialArrayStart * j.ElemSize + 2);
                        }
                        else
                        {
                            responseSize += (ushort)(j.adjSize(tag) - j.PartialArrayStart * j.ElemSize);
                        }

                        //if (responseSize > LOGIX5550_CONNECTION_SIZE)
                        if (responseSize > pduMaxSize)
                        {
                            if (j.ReadTagEnd != j.ReadTagStart)
                            {
                                nextlistLimitate.Add(j);
                                return;
                            }
                            if(tag.Size / j.ElemSize <= 1)
                            {
                                return;
                            }
                            //int tmp = (j.adjSize(tag) + LOGIX5550_CONNECTION_SIZE + 1 - responseSize - j.ElemSize) / j.ElemSize;
                            int tmp = (j.adjSize(tag) + (int)pduMaxSize + 1 - responseSize - j.ElemSize) / j.ElemSize;
                            tmp -= j.PartialArrayStart;
                            if (tmp > 0  )
                            {
                                j.PartialArrayEnd = (ushort)(tmp + j.PartialArrayStart);
                                nextlistLimitate.Add(j);
                            }
                            return;
                        }
                        j.ReadTagEnd++;
                    }
                    TagIndex++;
                }
                nextlistLimitate.Add(j);
                j.ReadTagEnd = 0;
                j.PartialArrayEnd = 0;
            }
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

        private static ushort numTagsListToRead(List<OmronEthernetIPCommJob> List)
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

        public static ushort Logix5550TagsPrepareMultiReadReq(ref List<OmronEthernetIPCommJob> List, ref OmronEthernetIPStation s,
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
            ref List<OmronEthernetIPCommJob> ListJob,ref OmronEthernetIPStation s,
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
            //List<Tag> changed = new List<Tag>();
            //uint recivedSize;
            //if (job.TotalJobSize > receivedbuffer.Length)
            //{
            //    recivedSize = (uint)receivedbuffer.Length;
            //}
            //else
            //{
            //    recivedSize = job.TotalJobSize;
            //}

            //byte[] tempBuffer = new byte[recivedSize];
            //Array.Copy(receivedbuffer, tempBuffer, recivedSize);

            //job.SetJobData(tempBuffer, ref changed);
            //items.AddRange(changed);

            //return true;

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
                (!job.ABAddress.Contains(']')))
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
            //Array.Copy(receivedbuffer, tempBuffer, recivedSize);

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
