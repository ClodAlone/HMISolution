using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using System.Runtime.InteropServices;
using Opc.Ua;
using System.Text.RegularExpressions;
using DriverCodeBaseEx.Enumerators;

namespace EtherNetIP
{
    #region enums

    public enum EtherNetIpErrorCodes : int
    {
        ErrorWrongTransaction = 1000,
        ErrorRepTooShort,
        ErrorRepTooLong,
        ErrorRepTagSize,
        ErrorNoRep,
        ErrorRepDataType,
        ErrorWrongMRServiceReplyCode,
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
        ErrorEnbeddedStation,
        ErrorMRStatus = 10000,
    }

    public enum DataFormats
    {
        INVALID,
        BIT,
        WORD,
        DWORD,
        BYTE,
        LWORD
    }

    public enum PlcTypes
    { SLC500_MicroLogix, PLC5, ControlLogix_CompactLogix, Micro800_series }

    public enum PhisicalAddressesOptimizzations
    { False, True }

    public enum ReadStructuresModes
    { ReadFieldByField, ReadAsSingleBlock }

    public enum AddressTypes
    { DataFile, TagName }

    public enum TagFormats
    {
        BOOL,
        SINT,
        INT,
        DINT,
        REAL,
        TIMER,
        COUNTER,
        ARRAYOF32BITS,
        STRING,
        STRUCTURE,
        LINT,
        ULINT,
        LWORD
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
        LongInteger,
        String,
    }

    public enum SubElements
    { 
        Invalid = -1,        
        PRE = 1,
        ACC = 2,        
    }

    public enum CommandTypes
    {
        ReadCmd,
        WriteCmd,
        TestCmd,
        FwdOpen, 
        FwdClose,
        Invalid,
    }

    public enum TagTypes
    {
        Atomic,
        ArrayStandard,
        Structure,
        ArrayOfStructures,
    };
    
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
            if (index < (buffer.Length-1))
            {
                this.LOBYTE = buffer[index];
                this.HIBYTE = buffer[index + 1];
            } else {
                // case of incomplete message/frame
                this.LOBYTE = 0;
                this.HIBYTE = 0;
            }
        }
        public ushortUnion(List<byte> buffer, ushort index)
        {
            this.USHORT = 0;            
            if (index < (buffer.Count - 1))
            {
                this.LOBYTE = buffer[index];
                this.HIBYTE = buffer[index + 1];
            } else
            { // case of incomplete message/frame
                this.LOBYTE = 0;
                this.HIBYTE = 0;
            }
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

    public class EtherNetIpProtocol
    {
        #region constants

        public const ushort MAX_IOI_STRING_LENGTH = 200;
        public const ushort MIN_READ_REQUEST_LENGTH = 10;
        public const ushort MIN_WRITE_REQUEST_LENGTH = 13;
        public const ushort MIN_READ_REPLY_LENGTH = 9;
        public const ushort MIN_WRITE_REPLY_LENGTH = 6;
        public const ushort MAX_SEGMENT_DATA_SIZE = 224;
        public const ushort MAX_DATA_SIZE = 4096;
        public const ushort MAX_DATA_SIZE_DATAFILE_ADDRESS = 255;
        public const ushort LOGIX5550_CONNECTION_SIZE_HEADER = 54;
        public const ushort LOGIX5550_RESPONSE_SIZE_HEADER = 106;
        public const ushort LOGIX5550_HEADER_SIZE_OF_OPTIMIZED_RESPONSE = 52;
        public const ushort LOGIX5550_CONNECTION_SIZE = 504;
        public const ushort TCP_MAX_SEGMENT_SIZE = 4096;
        public const ushort STRING_MAX_LENGHT = 82; // real stringh length (used into dynamic setting after plc variable name : <xxx>)
        public const ushort MAX_STRING_SIZE = 84;   // memory size

        public const byte CIP_CLASS_INFO_LENGTH = 2;
        public const byte CIP_2BYTES_INSTANCE_INFO_LENGTH = 4;
        public const byte CIP_ARRAY_INDEX_SHORT_LENGTH = 2;
        public const byte CIP_ARRAY_INDEX_LONG_LENGTH = 4;

        public const byte ENCAPSULATION_HEADER_SIZE = 24;
        public const byte CIP_MULTIPLE_READ_HEADER_SIZE = 30;
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

        public static ushortUnion VENDOR_ID = new ushortUnion(326);
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

        public static string TEST_COMM_DYNAMIC_SETTINGS_ABSOLUTE = "EtherNetIP.Station={0}|LinkType=1|ABA=N7:0|ATYPE=0|TEFRM=2";

        public enum ReadWriteLimitate
        {
            Add,
            EvaluateOnly
        }

        public enum ProtocolParameterSet
        {
            None,
            DisableRedundantO_TNetwork
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
                return (TotalSize == 0 || ResponseSize == 0);
            }
        }

        #endregion

        #region static methods

        public delegate void PrepareHeader(
            EtherNetIPStation s,
            ref byte[] pdu, ref ushortUnion pduPointer);

        //public delegate ushort PrepareBody(
        //    ref List<EtherNetIPCommJob> List,
        //    ref EtherNetIPStation s,
        //    ref byte[] pdu, ref ushortUnion pduPointer);

        public static void PrepareEncapsulationHeader(ushortUnion Cmd, EtherNetIPChannel c, ref byte[] pdu, ref ushortUnion pduPointer)
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

        public static bool RegisterSession(ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c)
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

                if (c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer))
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
                        }
                        if (0 != lenData.USHORT)
                        {
                            c.DeviceRead(pdu, lenData.USHORT);
                        }

                    }
                }
                c.ReceiveClear();
            }

            return (c.SessionRegisterOk());
        }

        public static void UnRegisterSession(ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c)
        {
            c.InvalidateStationConnections();
            if (c.SessionRegisterOk())
            {
                PrepareEncapsulationHeader(ecUnRegisterSession, c, ref pdu, ref pduPointer);
                c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer);
                c.ReceiveClear();
                c.ClearSession();
            }
        }

        public static void CloseSession(ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c)
        {
            c.InvalidateStationConnections();
            if (c.SessionRegisterOk())
            {
                PrepareEncapsulationHeader(ecUnRegisterSession, c, ref pdu, ref pduPointer);
                // Send the request and don't wait for the reply
                c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer);
                c.ReceiveClear();
                c.ClearSession();
            }
        }

        public static bool Logix5550ForwardOpen(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c, ProtocolParameterSet pps = ProtocolParameterSet.None)
        {
            if (c.SessionRegisterOk())
            {
                if (!s.ConnectionIDSet)
                {
                    if (s.Logix5550NonBlockPhAdd == PhisicalAddressesOptimizzations.True && s.PlcType != PlcTypes.Micro800_series)
                    {
                        Logix5550GetCpuInformation(s, ref pdu, ref pduPointer, c);
                    }

                    // Set the initial values for the connection IDs
                    s.Logix5550SetInitConnIDs();

                    // Prepare the request message
                    EtherNetIPCommJob job = new EtherNetIPCommJob(s);
                    job.CommandType = CommandTypes.FwdOpen;
                    Logix5550PrepareUnconnectedRequest(job, c, s, ref pdu, ref pduPointer,pps);

                    c.ReceiveClear();

                    if (c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer))
                    {
                        //first we read the fixed-size header
                        if (c.DeviceRead(pdu, EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE)) //error timeout RX
                        {
                            // Then read the variable size data part
                            ushortUnion lenData = new ushortUnion(pdu, EtherNetIpProtocol.EDATA_LEN_OFFS);
                            if (lenData.USHORT >= 20)
                            {
                                byte[] pduData = new byte[lenData.USHORT];
                                if (c.DeviceRead(pduData, (uint)lenData.USHORT)) //error timeout RX
                                {
                                    pduData.CopyTo(pdu, EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE);
                                    // Analyze the reply header
                                    if (c.AnalyzeReplyHeader(ref pdu, ecSendRRData.USHORT, c.SessionHandle.UINT) ==
                                        (EtherNetIpErrorCodes)DriverErrorCodes.ErrorNoError)
                                    {
                                        if (pdu[MR_SVC_REPLY_CODE_OFFS] == 0xD4)
                                        {
                                            if (pdu[MR_SVC_REPLY_CODE_OFFS + 2] == 0)//Status is one byte N° 42
                                            {
                                                // Set the Originator > Target Connection ID
                                                s.OTNetConnID = new uintUnion(pdu, EtherNetIpProtocol.MR_SVC_REPLY_CODE_OFFS + 4);
                                                s.ConnectionIDSet = true;
                                            }
                                            else if (pdu[MR_SVC_REPLY_CODE_OFFS + 2] == 1)
                                            {
                                                ushortUnion ExtendedStatus = new ushortUnion(pdu, MR_SVC_REPLY_CODE_OFFS + 4);// The Extended Status 2 byte (N° 44 - 45)
                                                if (ExtendedStatus.USHORT == 0x125)//Error code 0x125 Invalid O->T redundant owner
                                                {
                                                    s.ConnectionIDSet = Logix5550ForwardOpen(s, ref pdu, ref pduPointer, c, ProtocolParameterSet.DisableRedundantO_TNetwork);
                                                    if (!s.ConnectionIDSet)
                                                    {
                                                        c.ClearSession();
                                                    }
                                                }
                                                else
                                                {
                                                    c.ClearSession();
                                                }
                                            }
                                            else if (pdu[MR_SVC_REPLY_CODE_OFFS + 2] == 0x64)//Error code 0x64 invalid section handle
                                            {
                                                c.ClearSession();
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

        public static bool Logix5550ForwardClose(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c)
        {
            // Connection is open?
            if (((s.PlcType != PlcTypes.ControlLogix_CompactLogix) && (s.PlcType != PlcTypes.Micro800_series)) || c.SessionHandle.UINT == 0 || !s.ConnectionIDSet)
            {
                return true;
            }

            // Prepare the request message
            EtherNetIPCommJob job = new EtherNetIPCommJob(s);
            job.CommandType = CommandTypes.FwdClose;
            Logix5550PrepareUnconnectedRequest(job, c, s, ref pdu, ref pduPointer);

            c.ReceiveClear();


            // Send the request
            if (c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer))
            {
                if (c.DeviceRead(pdu, EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE))
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

        public static void Logix5550GetCpuInformation(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c)
        {
            s.FirmwareVersion = 0;
            s.CpuModel = 0;

            // Prepare the request message
            Logix5550NonBlockPhAddPrepareGetCpuTypeRequest(c, s, ref pdu, ref pduPointer);
            c.ReceiveClear();

            // Send the request
            if (!c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer))
            {
                return;
            }

            // First read the fixed-size header
            if (c.DeviceRead(pdu, ENCAPSULATION_HEADER_SIZE))
            {
                // Read the variable size data part
                ushortUnion DataLen = new ushortUnion(pdu, EDATA_LEN_OFFS);
                if (DataLen.USHORT > 0)
                {
                    // Read the variable size data part
                    byte[] pduData = new byte[DataLen.USHORT];
                    if (!c.DeviceRead(pduData, (uint)DataLen.USHORT))
                    {
                        return;
                    }
                    pduData.CopyTo(pdu, ENCAPSULATION_HEADER_SIZE);
                }
            }


            // Pad character to be read?
            uint nDataLen = c.GetBytesToRead();
            if (nDataLen > 1)
            {
                return;
            }
            else if (nDataLen == 1)
            {
                //read an extra char (pad)
                byte[] cPad = new byte[1];
                if (!c.DeviceRead(cPad, 1))
                {
                    return;
                }
            }


            // Analyze the reply header
            if (c.AnalyzeReplyHeader(ref pdu, ecSendRRData.USHORT, c.SessionHandle.UINT) !=
                (EtherNetIpErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                return;
            }

            // Check the reply service code
            ushortUnion eDataLen = new ushortUnion(pdu, EDATA_LEN_OFFS);
            if (eDataLen.USHORT < 20)
            {
                return;
            }
            if (pdu[MR_SVC_REPLY_CODE_OFFS] != 0x81)
            {
                return;
            }

            // Check the status
            ushortUnion wStatus = new ushortUnion(pdu, MR_SVC_REPLY_CODE_OFFS + 2);
            if (wStatus.USHORT != 0)
            {
                return;
            }

            // Data parsing
            if (eDataLen.USHORT < 35)
            {
                return;
            }

            s.FirmwareVersion = pdu[MR_SVC_REPLY_CODE_OFFS + 10];
            // Get the product description
            byte nProductDescrLength = pdu[MR_SVC_REPLY_CODE_OFFS + 18];
            if (eDataLen.USHORT >= (35 + nProductDescrLength))
            {
                byte[] auxArray = new byte[nProductDescrLength];
                Array.Copy(pdu, MR_SVC_REPLY_CODE_OFFS + 19, auxArray, 0, nProductDescrLength);
                String szProductDescription = ASCIIEncoding.ASCII.GetString(auxArray);
                // Search for the CPU model
                int nIndex = szProductDescription.IndexOf("LOGIX");
                if (nIndex >= 0)
                {
                    String szCPUModel = szProductDescription.Substring(nIndex + 5, 4);
                    if (szCPUModel != String.Empty)
                    {
                        s.CpuModel = Convert.ToUInt16(szCPUModel);
                    }
                }
            }

        }

        public static bool Logix5000V21BuildInstancesMaps(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c)
        {
            // Get instance IDs of the global tags (scope == controller)
            ushort nErrorCode = 0;
            if (!Logix5000V21GetTagInstances(s, ref pdu, ref pduPointer, c, ref nErrorCode))
            {
                return (false);
            }

            // Set instances of the programs
            if (!Logix5000V21SetProgramInstances(s, ref pdu, ref pduPointer, c))
            {
                return (false);
            }

            // Get instance ID of the local tags of the programs (scope == program)
            nErrorCode = 0;
            if (s.m_listPrograms.Count != 0)
            {
                ushort nProgramInstance = 0;
                foreach (String szProgramName in s.m_listPrograms)
                {
                    if (s.m_mapProgramsInstances.TryGetValue(szProgramName, out nProgramInstance))
                    {
                        if (!Logix5000V21GetTagInstances(s, ref pdu, ref pduPointer, c, ref nErrorCode, szProgramName, nProgramInstance))
                        {
                            if (nErrorCode == 0)
                            {
                                return (false);
                            }
                            String szWarningMsg = string.Format(Properties.Resources.WARNRUNPROGRAMVARINST,
                                s.Name, nErrorCode, szProgramName);
                            c.getCommDriver().OnSystemEvent(ObjectIds.Server, szWarningMsg, Opc.Ua.EventSeverity.High);
                            continue;
                        }
                    }
                }
            }

            // Get template information
            if (!Logix5000V21BuildTemplateMaps(s, ref pdu, ref pduPointer, c))
            {
                return (false);
            }

            return (true);
        }

        public static bool Logix5000V21BuildTemplateMaps(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c)
        {
            // No templates?
            if (s.m_mapPlcTemplateInfo.Count() == 0)
            {
                // Nothing to do
                return (true);
            }

            // Initialize the map to be searched with the current map
            Dictionary<ushort, PlcTemplateInfo> mapTemplates = new Dictionary<ushort, PlcTemplateInfo>(s.m_mapPlcTemplateInfo);
            Dictionary<ushort, PlcTemplateInfo> mapTemplateFields = new Dictionary<ushort, PlcTemplateInfo>();

            bool bDone = false;
            while (!bDone)
            {
                // Get the information for the templates included in
                // mapTemplates; mapTemplateFields will contain the nested
                // templates not included in mapTemplates  
                if (!Logix5000V21GetTemplatesInfo(s, ref pdu, ref pduPointer, c,
                    ref mapTemplates, ref mapTemplateFields,
                                                 ref s.m_mapPlcTemplateInfo))
                {
                    Logix5000V21EmptyPlcTemplatesMap(ref mapTemplates);
                    Logix5000V21EmptyPlcTemplatesMap(ref mapTemplateFields);
                    return (false);
                }

                // If there are still templates not searched prepare the working maps
                // for the next iteration
                Logix5000V21EmptyPlcTemplatesMap(ref mapTemplates);
                if (mapTemplateFields.Count == 0)
                {
                    bDone = true;
                }
                else
                {
                    mapTemplates = new Dictionary<ushort, PlcTemplateInfo>(mapTemplateFields);
                    Logix5000V21EmptyPlcTemplatesMap(ref mapTemplateFields);
                }
            }

            // Done
            return (true);
        }

        public static void Logix5000V21EmptyPlcTemplatesMap(ref Dictionary<ushort, PlcTemplateInfo> mapPlcTemplateInfo)
        {
            foreach (KeyValuePair<ushort, PlcTemplateInfo> entry in mapPlcTemplateInfo)
                if (entry.Value.m_mapFieldInfo != null)
                    entry.Value.m_mapFieldInfo.Clear();
            mapPlcTemplateInfo.Clear();
        }

        public static bool Logix5000V21GetTemplatesInfo(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c,
                                                        ref Dictionary<ushort, PlcTemplateInfo> pSearchMap,
                                                        ref Dictionary<ushort, PlcTemplateInfo> pNewTemplatesMap,
                                                        ref Dictionary<ushort, PlcTemplateInfo> pDestMap)
        {
            // No templates?
            if (pSearchMap.Count == 0)
            {
                return (true);
            }

            List<EtherNetIPTemplateFieldInfo> pFieldInfoList = new List<EtherNetIPTemplateFieldInfo>();
            bool bIsString = false;
            uint nMemorySize = 0;
            ushort nErrorCode = 0;

            List<ushort> keyList = new List<ushort>();
            foreach (KeyValuePair<ushort, PlcTemplateInfo> entry in pSearchMap)
                keyList.Add(entry.Key);
            foreach (ushort Key in keyList)
            {
                bIsString = false;
                nErrorCode = 0;
                if (!Logix5000V21GetTemplateInfo(s, ref pdu, ref pduPointer, c,
                    Key, ref bIsString, ref nMemorySize,
                                                ref pFieldInfoList, ref nErrorCode))
                {
                    String szWarningMsg = string.Format(Properties.Resources.WARNRUNTEMPLATEINFO,
                        s.Name, nErrorCode, Key);
                    c.getCommDriver().OnSystemEvent(ObjectIds.Server, szWarningMsg, Opc.Ua.EventSeverity.High);
                    continue;
                }
                else
                {
                    Logix5000V21AddFieldsInfo(ref pDestMap, Key, nMemorySize,
                                              bIsString, ref pFieldInfoList,
                                              ref pNewTemplatesMap);
                }

                pFieldInfoList.Clear();
            }

            return (true);
        }

        public static void Logix5000V21AddFieldsInfo(
                                    ref Dictionary<ushort, PlcTemplateInfo> PlcTemplateMap,
                                    ushort TemplateInstance,
                                    uint MemorySize,
                                    bool StructVarIsString,
                                    ref List<EtherNetIPTemplateFieldInfo> FieldInfoList,
                                    ref Dictionary<ushort, PlcTemplateInfo> TemplatesFieldsMap)
        {

            UInt32 nFieldIndex = 0;
            PlcTemplateInfo pPlcTemplateInfo = new PlcTemplateInfo();
            pPlcTemplateInfo.m_bIsString = StructVarIsString;
            pPlcTemplateInfo.m_nMemorySize = MemorySize;
            pPlcTemplateInfo.m_mapFieldInfo = new Dictionary<string, PlcTemplateFieldInfo>();
            PlcTemplateMap[TemplateInstance] = pPlcTemplateInfo;
            foreach (EtherNetIPTemplateFieldInfo pFieldInfo in FieldInfoList)
            {
                if (pFieldInfo.m_szName == null || pFieldInfo.m_szName == "")
                {
                    continue;
                }
                // Add an item to the map of the fields of the template
                if (!PlcTemplateMap[TemplateInstance].m_mapFieldInfo.ContainsKey(
                                                                pFieldInfo.m_szName))
                {
                    PlcTemplateFieldInfo pPlcTemplateFieldInfo = new PlcTemplateFieldInfo();
                    Logix5000V21AddFieldInfo(
                                    ref pPlcTemplateFieldInfo,
                                    nFieldIndex++, pFieldInfo, ref TemplatesFieldsMap);
                    PlcTemplateMap[TemplateInstance].m_mapFieldInfo[pFieldInfo.m_szName] = pPlcTemplateFieldInfo;
                }
            }
        }

        public static void Logix5000V21AddFieldInfo(
                                    ref PlcTemplateFieldInfo pPlcTemplateFieldInfo,
                                    UInt32 nFieldIndex,
                                    EtherNetIPTemplateFieldInfo FieldInfo,
                                    ref Dictionary<ushort, PlcTemplateInfo> TemplatesFieldsMap)
        {
            pPlcTemplateFieldInfo.m_nType = FieldInfo.m_nType;
            // BOOL variable?
            if (FieldInfo.m_nType != 0xC1)
            {
                pPlcTemplateFieldInfo.m_nBitNumber = 0;
            }
            else
            {
                pPlcTemplateFieldInfo.m_nBitNumber = FieldInfo.m_nBitNumber;
            }
            pPlcTemplateFieldInfo.m_nIndex = nFieldIndex;
            pPlcTemplateFieldInfo.m_nOffset = FieldInfo.m_nOffset;
            pPlcTemplateFieldInfo.m_nDim0 = FieldInfo.m_nDim0;
            pPlcTemplateFieldInfo.m_szTemplateName = FieldInfo.m_szTemplateName;
            pPlcTemplateFieldInfo.m_szName = FieldInfo.m_szName;

            // Special case: structures
            if ((FieldInfo.m_nType & 0x8000) == 0x8000)
            {
                ushort nTemplateInstance = (ushort)(FieldInfo.m_nType & 0xFFF);
                pPlcTemplateFieldInfo.m_nTemplateInstance = nTemplateInstance;
                if (!TemplatesFieldsMap.ContainsKey(nTemplateInstance))
                {
                    TemplatesFieldsMap[nTemplateInstance] = new PlcTemplateInfo();
                }
            }
        }

        public static bool Logix5000V21GetTemplateInfo(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c,
                                    ushort nInstance,
                                    ref bool bIsString,
                                    ref uint nMemorySize,
                                    ref List<EtherNetIPTemplateFieldInfo> FieldInfoList,
                                    ref ushort nErrorCode)
        {
            bIsString = false;
            nMemorySize = 0;
            nErrorCode = 0;
            uint nTemplateDefSize = 0;

            ///////////////////////////////////////////////////////////////////////////
            // Get the template general information
            nErrorCode = 0;

            ushort nMemberCount = 0;
            if (!Logix5000V21GetTemplateGeneralInfo(s, ref pdu, ref pduPointer, c,
                                                   nInstance, ref nTemplateDefSize,
                                                   ref nMemorySize, ref nMemberCount,
                                                   ref nErrorCode))
            {
                return false;
            }

            ///////////////////////////////////////////////////////////////////////////
            // Get the template's members information
            s.m_TemplateBuffer = new byte[nTemplateDefSize];
            ushort nReadDataBytes = 0;
            nErrorCode = 0;
            if (!Logix5000V21GetTemplateFieldInfo(s, ref pdu, ref pduPointer, c,
                                                  new ushortUnion(nInstance), nTemplateDefSize,
                                                  ref nErrorCode, ref nReadDataBytes))
            {
                return false;
            }

            if (!Logix5000V21ParseTemplateFieldInfo(ref s.m_TemplateBuffer, nTemplateDefSize,
                                                   nMemberCount, ref bIsString,
                                                   ref FieldInfoList))
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }

            return (true);
        }

        public static bool Logix5000V21ParseTemplateFieldInfo(ref byte[] TemplateBuffer,
                                    uint nDataLen,
                                    ushort nMemberCount,
                                    ref bool bIsString,
                                    ref List<EtherNetIPTemplateFieldInfo> FieldInfoList)
        {
            bIsString = false;
            ushort nOffsetIndex = 0;
            ushort nSize = (ushort)(nMemberCount * 8);
            if (nDataLen < (nOffsetIndex + nSize))
            {
                return (false);
            }

            EtherNetIPTemplateFieldInfo FieldInfo = new EtherNetIPTemplateFieldInfo();

            // Parse field information
            ushort i = 0;
            for (i = 0; i < nSize; i += 8)
            {
                ushortUnion wAux = new ushortUnion(TemplateBuffer, (ushort)(nOffsetIndex + i));
                FieldInfo.m_nBitNumber = wAux.USHORT;
                FieldInfo.m_nDim0 = wAux.USHORT;
                wAux = new ushortUnion(TemplateBuffer, (ushort)(nOffsetIndex + i + 2));
                FieldInfo.m_nType = wAux.USHORT;
                uintUnion uAux = new uintUnion(TemplateBuffer, (ushort)(nOffsetIndex + i + 4));
                FieldInfo.m_nOffset = uAux.UINT;
                // Special case: strings
                if (FieldInfo.m_nType == 0x20C2)
                {
                    FieldInfo.m_nBitNumber = 0;
                }

                FieldInfoList.Add(FieldInfo);
            }

            nOffsetIndex += nSize;
            nSize = (ushort)(2 * (nMemberCount + 1));
            if (nDataLen < (nOffsetIndex + nSize))
            {
                return (false);
            }

            List<byte> auxList = new List<byte>();
            // Template name
            while ((nOffsetIndex < (nDataLen - 1)) &&
                (TemplateBuffer[nOffsetIndex] != 0))
            {
                auxList.Add(TemplateBuffer[nOffsetIndex++]);
            }
            nOffsetIndex++;
            String szTemplateName = ASCIIEncoding.ASCII.GetString(auxList.ToArray());
            auxList.Clear();
            if (nOffsetIndex >= nDataLen)
            {
                return (false);
            }

            EtherNetIPTemplateFieldInfo pFieldInfoTmp;
            int index = szTemplateName.IndexOf(';');
            string aux = index < 0 ? szTemplateName : szTemplateName.Substring(0, index);
            for (i = 0; i < FieldInfoList.Count; i++)
            {
                pFieldInfoTmp = FieldInfoList[i];
                pFieldInfoTmp.m_szTemplateName = aux;
                FieldInfoList[i] = pFieldInfoTmp;
            }

            // Special case: strings
            if (szTemplateName.IndexOf("ASCIISTRING") >= 0)
            {
                bIsString = true;
            }

            // Parse fields names
            List<EtherNetIPTemplateFieldInfo> tmpFieldInfoList = new List<EtherNetIPTemplateFieldInfo>();
            String szFieldName;
            foreach (EtherNetIPTemplateFieldInfo pFieldInfo in FieldInfoList)
            {
                FieldInfo = pFieldInfo;
                while ((nOffsetIndex <= (nDataLen - 1)) &&
                    (TemplateBuffer[nOffsetIndex] != 0))
                {
                    auxList.Add(TemplateBuffer[nOffsetIndex++]);
                }
                nOffsetIndex++;
                szFieldName = ASCIIEncoding.ASCII.GetString(auxList.ToArray());
                auxList.Clear();

                if (!string.IsNullOrEmpty(szFieldName))
                {
                    FieldInfo.m_szName = szFieldName;
                }
                tmpFieldInfoList.Add(FieldInfo);
            }

            FieldInfoList = tmpFieldInfoList;

            return (true);
        }


        public static bool Logix5000V21GetTemplateFieldInfo(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c,
                                                        ushortUnion nTemplateInstance,
                                                        uint nTemplateSize,
                                                        ref ushort nErrorCode,
                                                        ref ushort nReadDataBytes)
        {
            nErrorCode = 0;

            ///////////////////////////////////////////////////////////////////////////
            // Get the template's members information
            nReadDataBytes = 0;
            ushort nBytesToBeRead = 0;
            ushort nMaxDataToBeRead = MAX_DATA_SIZE - 44;
            while (nReadDataBytes < nTemplateSize)
            {
                // Prepare the request message 
                nBytesToBeRead = (ushort)(nTemplateSize - nReadDataBytes);
                // Avoid buffer overflow
                if (nBytesToBeRead > nMaxDataToBeRead)
                {
                    nBytesToBeRead = nMaxDataToBeRead;
                }
                Logix5000V21PrepareGetTemplateFieldsRequest(c, s, ref pdu, ref pduPointer,
                                                            nTemplateInstance,
                                                            new ushortUnion(nReadDataBytes),
                                                            new ushortUnion(nBytesToBeRead));

                // Send the request
                if (!c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer))
                {
                    c.DeviceClose();
                    return false;
                }

                // Read the reply
                if (!Logix5000V21ReadReply(s, ref pdu, ref pduPointer, c))
                {
                    return false;
                }


                // Analyze the reply header
                if (c.AnalyzeReplyHeader(ref pdu, ecSendRRData.USHORT, c.SessionHandle.UINT) !=
                    (EtherNetIpErrorCodes)DriverErrorCodes.ErrorNoError)
                {
                    return false;
                }

                // Check the reply service code
                ushortUnion eDataLen = new ushortUnion(pdu, EDATA_LEN_OFFS);
                if (eDataLen.USHORT < 20)
                {
                    Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                    return false;
                }
                if (pdu[MR_SVC_REPLY_CODE_OFFS] != 0xCC)
                {
                    Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                    return false;
                }

                // Check the reply status
                ushortUnion wStatus = new ushortUnion(pdu, MR_SVC_REPLY_CODE_OFFS + 2);
                if (wStatus.USHORT != 0 && wStatus.USHORT != 6)
                {
                    nErrorCode = wStatus.USHORT;
                    return false;
                }

                ushort nBytesReadInThisIteration = (ushort)(eDataLen.USHORT - 20);

                // Store the data
                Array.Copy(pdu, MR_SVC_REPLY_CODE_OFFS + 4, s.m_TemplateBuffer, nReadDataBytes, nBytesReadInThisIteration);

                nReadDataBytes += nBytesReadInThisIteration;
            }

            return (true);
        }

        public static ushort Logix5000V21PrepareGetTemplateFieldsRequest(EtherNetIPChannel c, EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer,
                                                            ushortUnion nInstance,
                                                            ushortUnion nReadOffset,
                                                            ushortUnion nReadSize)
        {
            s.LastTransOK = true;
            // Add the header
            // Add the header
            PrepareEncapsulationHeader(ecSendRRData, c, ref pdu, ref pduPointer);

            // Calculate the lengths of the encapsulated messages
            // Service, request path, attributes
            ushort nLength = 14;

            // Fill in packet data length
            ushortUnion wAux = new ushortUnion((ushort)(nLength + 30));
            pdu[EDATA_LEN_OFFS] = wAux.LOBYTE;
            pdu[EDATA_LEN_OFFS + 1] = wAux.HIBYTE;

            // Complete the request message
            Logix5000V21AddCpfUnconnectedHeader(ref pdu, ref pduPointer, nLength);

            // Message Request
            // Service = Read template
            pdu[pduPointer.USHORT++] = 0x4C;
            // Size of Request Path (words)
            pdu[pduPointer.USHORT++] = 0x03;
            // Request Path = 20, 6C (class = Template); 25, 00, xx, xx (Instance)
            pdu[pduPointer.USHORT++] = 0x20;
            pdu[pduPointer.USHORT++] = 0x6C;
            pdu[pduPointer.USHORT++] = 0x25;
            pdu[pduPointer.USHORT++] = 0x00;
            pdu[pduPointer.USHORT++] = nInstance.LOBYTE;
            pdu[pduPointer.USHORT++] = nInstance.HIBYTE;
            // Request specific data
            pdu[pduPointer.USHORT++] = nReadOffset.LOBYTE;
            pdu[pduPointer.USHORT++] = nReadOffset.HIBYTE;
            pdu[pduPointer.USHORT++] = 0x00;
            pdu[pduPointer.USHORT++] = 0x00;
            pdu[pduPointer.USHORT++] = nReadSize.LOBYTE;
            pdu[pduPointer.USHORT++] = nReadSize.HIBYTE;

            // Route Path Size
            pdu[pduPointer.USHORT++] = 0x01;

            // Reserved
            pdu[pduPointer.USHORT++] = 0x00;

            // Route Path: Port, Address
            pdu[pduPointer.USHORT++] = 0x01;
            pdu[pduPointer.USHORT++] = s.CPUSlot;

            return (pduPointer.USHORT);
        }

        public static bool Logix5000V21GetTemplateGeneralInfo(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c,
                                                        ushort nTemplateInstance,
                                                        ref uint nTemplateDefSize,
                                                        ref uint nMemSize,
                                                        ref ushort nMemberCount,
                                                        ref ushort nErrorCode)
        {
            nTemplateDefSize = 0;
            nMemSize = 0;
            nMemberCount = 0;
            nErrorCode = 0;

            // Prepare the request message
            Logix5000V21PrepareGetTemplateGeneralInfoRequest(c, s, ref pdu, ref pduPointer, nTemplateInstance);

            // Send the request
            if (!c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer))
            {
                c.DeviceClose();
                return false;
            }

            // Read the reply
            if (!Logix5000V21ReadReply(s, ref pdu, ref pduPointer, c))
            {
                return false;
            }

            // Analyze the reply header
            if (c.AnalyzeReplyHeader(ref pdu, ecSendRRData.USHORT, c.SessionHandle.UINT) !=
                (EtherNetIpErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                return false;
            }

            // Check the reply service code
            ushortUnion eDataLen = new ushortUnion(pdu, EDATA_LEN_OFFS);
            if (eDataLen.USHORT < 20)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }
            if (pdu[MR_SVC_REPLY_CODE_OFFS] != 0x83)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }


            // Check the status
            ushortUnion wStatus = new ushortUnion(pdu, MR_SVC_REPLY_CODE_OFFS + 2);
            if (wStatus.USHORT != 0)
            {
                nErrorCode = wStatus.USHORT;
                return false;
            }


            // Data parsing
            eDataLen.USHORT += ENCAPSULATION_HEADER_SIZE;
            if (!Logix5000V21ParseTemplateInfo(ref pdu, ref pduPointer, eDataLen.USHORT, nTemplateInstance,
                                      ref nTemplateDefSize, ref nMemSize, ref nMemberCount))
            {
                return (false);
            }

            return (true);
        }

        public static bool Logix5000V21ParseTemplateInfo(ref byte[] pdu, ref ushortUnion pduPointer,
                                                        ushort nDataLength,
                                                        ushort nTemplateInstance,
                                                        ref uint nTemplateDefSize,
                                                        ref uint nMemSize,
                                                        ref ushort nMemberCount)
        {
            // Check data length
            if (nDataLength != 74) {
                return (false);
            }


            // Template Size
            uintUnion nTemplateSize = new uintUnion(pdu, MR_SVC_REPLY_ADDSTSSZ_OFFS + 7);
            nTemplateSize.UINT = nTemplateSize.UINT * 4 - 20;

            // Size that the structure takes up in memory
            uintUnion nMemorySize = new uintUnion(pdu, MR_SVC_REPLY_ADDSTSSZ_OFFS + 15);


            // Number of members
            ushortUnion nMembersNumber = new ushortUnion(pdu, MR_SVC_REPLY_ADDSTSSZ_OFFS + 23);

            // Template handle
            ushortUnion nTemplateHandle = new ushortUnion(pdu, MR_SVC_REPLY_ADDSTSSZ_OFFS + 29);

            // Set the output parameters
            nTemplateDefSize = nTemplateSize.UINT;
            nMemSize = nMemorySize.UINT;
            nMemberCount = nMembersNumber.USHORT;

            return (true);
        }


        public static ushort Logix5000V21PrepareGetTemplateGeneralInfoRequest(EtherNetIPChannel c, EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer,
                                                        ushort nTemplateInstance)
        {
            s.LastTransOK = true;
            // Add the header
            PrepareEncapsulationHeader(ecSendRRData, c, ref pdu, ref pduPointer);

            // Calculate the lengths of the encapsulated messages
            //  Service, request path, attributes
            ushort nLength = 18;

            // Fill in packet data length
            ushortUnion wAux = new ushortUnion((ushort)(nLength + 30));
            pdu[EDATA_LEN_OFFS] = wAux.LOBYTE;
            pdu[EDATA_LEN_OFFS + 1] = wAux.HIBYTE;

            // Complete the request message
            Logix5000V21AddCpfUnconnectedHeader(ref pdu, ref pduPointer, nLength);

            // Message Request
            //  Service = 3 = Get Attribute List
            pdu[pduPointer.USHORT++] = 0x03;

            // Size of Request Path (words)
            pdu[pduPointer.USHORT++] = 0x03;

            // Request Path = 20, 6C (class Template Object); 25, 00, xx, xx
            //  (Template instance)
            pdu[pduPointer.USHORT++] = 0x20;
            pdu[pduPointer.USHORT++] = 0x6C;
            pdu[pduPointer.USHORT++] = 0x25;
            pdu[pduPointer.USHORT++] = 0x00;
            wAux.USHORT = nTemplateInstance;
            pdu[pduPointer.USHORT++] = wAux.LOBYTE;
            pdu[pduPointer.USHORT++] = wAux.HIBYTE;

            // Request specific data
            pdu[pduPointer.USHORT++] = 0x04; // Number of requested attributes
            pdu[pduPointer.USHORT++] = 0x00;
            pdu[pduPointer.USHORT++] = 0x04; // Attribute 4 = Template Object Definition
            pdu[pduPointer.USHORT++] = 0x00; //               Size
            pdu[pduPointer.USHORT++] = 0x05; // Attribute 5 = Template Structure Size
            pdu[pduPointer.USHORT++] = 0x00;
            pdu[pduPointer.USHORT++] = 0x02; // Attribute 2 = Member Count
            pdu[pduPointer.USHORT++] = 0x00;
            pdu[pduPointer.USHORT++] = 0x01; // Attribute 1 = Structure Handle
            pdu[pduPointer.USHORT++] = 0x00;

            // Route Path Size
            pdu[pduPointer.USHORT++] = 0x01;

            // Reserved
            pdu[pduPointer.USHORT++] = 0x00;

            // Route Path: Port, Address
            pdu[pduPointer.USHORT++] = 0x01;
            pdu[pduPointer.USHORT++] = s.CPUSlot;

            return pduPointer.USHORT;

        }


        public static bool Logix5000V21SetProgramInstances(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c)
        {
            if (s.m_listPrograms.Count == 0)
            {
                return (true);
            }

            // Get the list of true instances
            if (!Logix5000V21GetProgramTrueInstances(s, ref pdu, ref pduPointer, c))
            {
                return (false);
            }
            if (s.m_listProgramsTrueIstances.Count == 0)
            {
                return (true);
            }

            // Get programs' addresses
            if (!Logix5000V21GetProgramsAddresses(s, ref pdu, ref pduPointer, c))
            {
                return (false);
            }

            // Assign instances to programs
            if (!Logix5000V21SetProgramsTrueInstances(s, ref pdu, ref pduPointer, c))
            {
                return (false);
            }

            return (true);
        }

        public static bool Logix5000V21SetProgramsTrueInstances(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c)
        {
            if (s.m_mapProgramsInstances.Count == 0)
            {
                return (true);
            }

            ushort wStatus;
            ushort nTrueInstance = 0;
            uint nAddress = 0;
            foreach (string szProgramName in s.m_listPrograms)
            {
                wStatus = 0;
                if (!Logix5000V21GetProgramAddress2(s, ref pdu, ref pduPointer, c, s.m_mapProgramsInstances[szProgramName], ref nAddress, ref wStatus))
                {
                    if (wStatus == 0)
                    {
                        return (false);
                    }
                    String szWarningMsg = string.Format(Properties.Resources.WARNRUNPROGRAMTRUEINST,
                        s.Name, wStatus, szProgramName);
                    c.getCommDriver().OnSystemEvent(ObjectIds.Server, szWarningMsg, Opc.Ua.EventSeverity.High);
                    continue;
                }

                nTrueInstance = 0;
                if (s.m_mapProgramsAddresses.TryGetValue(nAddress, out nTrueInstance))
                {
                    s.m_mapProgramsInstances[szProgramName] = nTrueInstance;
                }
            }

            return (true);
        }


        public static bool Logix5000V21GetProgramAddress2(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c,
                                                       ushort nInstance,
                                                       ref uint nAddress,
                                                       ref ushort inStatus)
        {
            inStatus = 0;
            nAddress = 0;

            // Prepare the request message
            Logix5000V21PrepareGetProgramAddr2Request(c, s, ref pdu, ref pduPointer, new ushortUnion(nInstance));

            // Send the request
            if (!c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer))
            {
                c.DeviceClose();
                return false;
            }

            // Read the reply
            if (!Logix5000V21ReadReply(s, ref pdu, ref pduPointer, c))
            {
                return false;
            }

            // Analyze the reply header
            if (c.AnalyzeReplyHeader(ref pdu, ecSendUnitData.USHORT, c.SessionHandle.UINT) !=
                (EtherNetIpErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                return false;
            }

            // Check the reply service code
            ushortUnion eDataLen = new ushortUnion(pdu, EDATA_LEN_OFFS);
            if (eDataLen.USHORT < 22)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }
            if (pdu[LOGIX5550_REP_TNS_OFFS + 2] != 0x83)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }

            if (eDataLen.USHORT < 26)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }

            // Check the status
            ushortUnion wStatus = new ushortUnion(pdu, LOGIX5550_REP_GENSTS_OFFS);
            inStatus = wStatus.USHORT;
            if (inStatus != 0)
            {
                return false;
            }


            // Data parsing
            if (eDataLen.USHORT < 36)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }
            uintUnion uintAux = new uintUnion(pdu, LOGIX5550_REP_GENSTS_OFFS + 8);
            nAddress = uintAux.UINT;


            return (true);
        }

        public static ushort Logix5000V21PrepareGetProgramAddr2Request(EtherNetIPChannel c, EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer,
                                                              ushortUnion nInstance)
        {
            s.LastTransOK = true;
            // Add the header
            PrepareEncapsulationHeader(ecSendUnitData, c, ref pdu, ref pduPointer);

            // Calculate the lengths of the encapsulated messages
            // Sequence count, service, request path, attribute list
            ushort nLength = 14;

            // Fill in packet data length
            ushortUnion wAux = new ushortUnion((ushort)(nLength + 20));
            pdu[EDATA_LEN_OFFS] = wAux.LOBYTE;
            pdu[EDATA_LEN_OFFS + 1] = wAux.HIBYTE;

            // Add the CPF header (connected service)
            Logix5000V21AddCpfConnectedHeader(s, ref pdu, ref pduPointer, nLength);

            // Complete the request message

            // Sequence Count
            pdu[pduPointer.USHORT++] = s.GetTransaction().LOBYTE;
            pdu[pduPointer.USHORT++] = s.GetTransaction().HIBYTE;

            // Service Code = Get Attribute List (0x03)
            pdu[pduPointer.USHORT++] = 0x03;

            // Request Path Size (words)
            pdu[pduPointer.USHORT++] = 0x03;

            // 8 - Bit Logical Class Segment
            pdu[pduPointer.USHORT++] = 0x20;

            // Class = 0x6B
            pdu[pduPointer.USHORT++] = 0x6B;

            // 16 - Bit Logical Instance Segment 
            pdu[pduPointer.USHORT++] = 0x25;
            pdu[pduPointer.USHORT++] = 0x0;

            // Instance
            pdu[pduPointer.USHORT++] = nInstance.LOBYTE;
            pdu[pduPointer.USHORT++] = nInstance.HIBYTE;

            // Attribute count
            pdu[pduPointer.USHORT++] = 0x01;
            pdu[pduPointer.USHORT++] = 0x0;

            // Attribute list
            pdu[pduPointer.USHORT++] = 0x03;
            pdu[pduPointer.USHORT++] = 0x0;

            return pduPointer.USHORT;
        }

        public static bool Logix5000V21GetProgramsAddresses(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c)
        {
            if (s.m_listProgramsTrueIstances.Count == 0)
            {
                return (true);
            }

            // Added in version 10.1.0.6 (FOGBUGZ 6755)
            ushort nErrorCode = 0;

            uint nAddress = 0;
            foreach (ushort nInstance in s.m_listProgramsTrueIstances)
            {
                nErrorCode = 0;
                if (!Logix5000V21GetProgramAddress(s, ref pdu, ref pduPointer, c, new ushortUnion(nInstance), ref nAddress, ref nErrorCode))
                {
                    if (nErrorCode == 0)
                    {
                        return (false);
                    }
                    String szWarningMsg = string.Format(Properties.Resources.WARNRUNGETPROGRAMADD,
                            s.Name, nErrorCode, nInstance);
                    c.getCommDriver().OnSystemEvent(ObjectIds.Server, szWarningMsg, Opc.Ua.EventSeverity.High);
                    continue;
                }

                s.m_mapProgramsAddresses[nAddress] = nInstance;
            }

            return (true);
        }

        public static bool Logix5000V21GetProgramAddress(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c,
                                                                 ushortUnion nInstance, ref uint nAddress, ref ushort nErrorCode)
        {
            nErrorCode = 0;
            nAddress = 0;

            // Prepare the request message
            Logix5000V21PrepareGetProgramAddrRequest(c, s, ref pdu, ref pduPointer, nInstance);

            // Send the request
            if (!c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer))
            {
                c.DeviceClose();
                return false;
            }

            // Read the reply
            if (!Logix5000V21ReadReply(s, ref pdu, ref pduPointer, c))
            {
                return false;
            }

            // Analyze the reply header
            if (c.AnalyzeReplyHeader(ref pdu, ecSendUnitData.USHORT, c.SessionHandle.UINT) !=
                (EtherNetIpErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                return false;
            }

            // Check the reply service code
            ushortUnion eDataLen = new ushortUnion(pdu, EDATA_LEN_OFFS);
            if (eDataLen.USHORT < 22)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }
            if (pdu[LOGIX5550_REP_TNS_OFFS + 2] != 0x83)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }

            if (eDataLen.USHORT < 26)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }

            // Check the status
            ushortUnion wStatus = new ushortUnion(pdu, LOGIX5550_REP_GENSTS_OFFS);
            if (wStatus.USHORT != 0)
            {
                nErrorCode = wStatus.USHORT;
                return false;
            }

            // Data parsing
            if (eDataLen.USHORT < 36)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }
            uintUnion uintAux = new uintUnion(pdu, LOGIX5550_REP_GENSTS_OFFS + 8);
            nAddress = uintAux.UINT;

            return (true);
        }

        public static ushort Logix5000V21PrepareGetProgramAddrRequest(EtherNetIPChannel c, EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer,
                                                                ushortUnion nInstance)
        {
            s.LastTransOK = true;
            // Add the header
            PrepareEncapsulationHeader(ecSendUnitData, c, ref pdu, ref pduPointer);

            // Calculate the lengths of the encapsulated messages
            // Sequence count, service, request path, attribute list
            ushort nLength = 14;

            // Fill in packet data length
            ushortUnion wAux = new ushortUnion((ushort)(nLength + 20));
            pdu[EDATA_LEN_OFFS] = wAux.LOBYTE;
            pdu[EDATA_LEN_OFFS + 1] = wAux.HIBYTE;

            // Add the CPF header (connected service)
            Logix5000V21AddCpfConnectedHeader(s, ref pdu, ref pduPointer, nLength);

            // Complete the request message

            // Sequence Count
            pdu[pduPointer.USHORT++] = s.GetTransaction().LOBYTE;
            pdu[pduPointer.USHORT++] = s.GetTransaction().HIBYTE;

            // Service Code = Get Attribute List (0x03)
            pdu[pduPointer.USHORT++] = 0x03;

            // Request Path Size (words)
            pdu[pduPointer.USHORT++] = 0x03;

            // 8 - Bit Logical Class Segment
            pdu[pduPointer.USHORT++] = 0x20;

            // Class = 0x68
            pdu[pduPointer.USHORT++] = 0x68;

            // 16 - Bit Logical Instance Segment 
            pdu[pduPointer.USHORT++] = 0x25;
            pdu[pduPointer.USHORT++] = 0;

            // Instance
            pdu[pduPointer.USHORT++] = nInstance.LOBYTE;
            pdu[pduPointer.USHORT++] = nInstance.HIBYTE;

            // Attribute count
            pdu[pduPointer.USHORT++] = 0x1;
            pdu[pduPointer.USHORT++] = 0x0;

            // Attribute list
            pdu[pduPointer.USHORT++] = 0x10;
            pdu[pduPointer.USHORT++] = 0x0;

            return pduPointer.USHORT;
        }

        public static bool Logix5000V21GetProgramTrueInstances(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c)
        {
            // Prepare the request message
            Logix5000V21PrepareGetProgInstListRequest(c, s, ref pdu, ref pduPointer);

            // Send the request
            if (!c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer))
            {
                c.DeviceClose();
                return false;
            }

            // Read the reply
            if (!Logix5000V21ReadReply(s, ref pdu, ref pduPointer, c))
            {
                return false;
            }

            // Analyze the reply header
            if (c.AnalyzeReplyHeader(ref pdu, ecSendUnitData.USHORT, c.SessionHandle.UINT) !=
                (EtherNetIpErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                return false;
            }

            // Check the reply service code
            ushortUnion eDataLen = new ushortUnion(pdu, EDATA_LEN_OFFS);
            if (eDataLen.USHORT < 22)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }
            if (pdu[LOGIX5550_REP_TNS_OFFS + 2] != 0xCB)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }

            if (eDataLen.USHORT < 26)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }
            // Check the status
            ushortUnion wStatus = new ushortUnion(pdu, LOGIX5550_REP_GENSTS_OFFS);
            if (wStatus.USHORT != 0)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }

            // Data parsing
            if (eDataLen.USHORT < 30)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }
            eDataLen.USHORT += ENCAPSULATION_HEADER_SIZE;
            ushort nDataOffset = 0;
            ushortUnion nInstance;
            for (nDataOffset = LOGIX5550_REP_GENSTS_OFFS + 2;
                (nDataOffset + 4) <= eDataLen.USHORT; nDataOffset += 4)
            {
                nInstance = new ushortUnion(pdu, nDataOffset);
                s.m_listProgramsTrueIstances.Add(nInstance.USHORT);
            }

            return (true);
        }

        public static ushort Logix5000V21PrepareGetProgInstListRequest(EtherNetIPChannel c, EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            s.LastTransOK = true;
            // Add the header
            PrepareEncapsulationHeader(ecSendUnitData, c, ref pdu, ref pduPointer);

            // Calculate the lengths of the encapsulated messages
            //  Sequence count, service, request path
            ushort nLength = 10;

            // Fill in packet data length
            ushortUnion wAux = new ushortUnion((ushort)(nLength + 20));
            pdu[EDATA_LEN_OFFS] = wAux.LOBYTE;
            pdu[EDATA_LEN_OFFS + 1] = wAux.HIBYTE;

            // Add the CPF header (connected service)
            Logix5000V21AddCpfConnectedHeader(s, ref pdu, ref pduPointer, nLength);

            // Complete the request message

            // Sequence Count
            pdu[pduPointer.USHORT++] = s.GetTransaction().LOBYTE;
            pdu[pduPointer.USHORT++] = s.GetTransaction().HIBYTE;

            // Service Code 0x4B
            pdu[pduPointer.USHORT++] = 0x4B;

            // Request Path Size (words)
            pdu[pduPointer.USHORT++] = 0x03;

            // 8 - Bit Logical Class Segment
            pdu[pduPointer.USHORT++] = 0x20;

            // Class = 0x68 = Program
            pdu[pduPointer.USHORT++] = 0x68;

            // 16 - Bit Logical Instance Segment 
            pdu[pduPointer.USHORT++] = 0x25;
            pdu[pduPointer.USHORT++] = 0;

            // Instance = 0
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            return pduPointer.USHORT;
        }

        public static void Logix5000V21AddCpfConnectedHeader(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer,
                                                    ushort nEncapsulatedMsgLength)
        {
            //Handle = 0 for CIP encapsulated messages
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            //Timeout (seconds)
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            //Item Count
            pdu[pduPointer.USHORT++] = 2;
            pdu[pduPointer.USHORT++] = 0;

            //Address Item - Item Type = Connected Address Item
            pdu[pduPointer.USHORT++] = 0xA1;
            pdu[pduPointer.USHORT++] = 0;

            //Address Item - Item Data Length = 4
            pdu[pduPointer.USHORT++] = 0x04;
            pdu[pduPointer.USHORT++] = 0;

            //Address Item - Connection ID
            pdu[pduPointer.USHORT++] = s.OTNetConnID.LOUSHORT.LOBYTE;
            pdu[pduPointer.USHORT++] = s.OTNetConnID.LOUSHORT.HIBYTE;
            pdu[pduPointer.USHORT++] = s.OTNetConnID.HIUSHORT.LOBYTE;
            pdu[pduPointer.USHORT++] = s.OTNetConnID.HIUSHORT.HIBYTE;

            //Data Item - Item Type = Connected Data Item
            pdu[pduPointer.USHORT++] = 0xB1;
            pdu[pduPointer.USHORT++] = 0;

            //Data Item - Item Data Length = variable (filled up later)
            ushortUnion wAux = new ushortUnion(nEncapsulatedMsgLength);
            pdu[pduPointer.USHORT++] = wAux.LOBYTE;
            pdu[pduPointer.USHORT++] = wAux.HIBYTE;
        }

        public static bool Logix5000V21GetTagInstances(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c,
                                                ref ushort nErrorCode, string szProgram = "", ushort nProgramInstance = 0)
        {
            ushortUnion nRequestInstance = new ushortUnion(0);
            bool bTransferCompleted = false;
            bool bReturnValue = true;
            while (bReturnValue && !bTransferCompleted)
            {
                nErrorCode = 0;
                bReturnValue = Logix5000V21GetInstances(s, ref pdu, ref pduPointer, c,
                                                        ref nRequestInstance,
                                                        ref bTransferCompleted,
                                                        ref nErrorCode,
                                                        szProgram,
                                                        nProgramInstance);
                nRequestInstance.USHORT++;
            }

            return (bReturnValue);
        }

        public static bool Logix5000V21GetInstances(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c,
                           ref ushortUnion nRequestInstance, ref bool bTransferCompleted, ref ushort nErrorCode, string szProgram = "", ushort nProgramInstance = 0)
        {
            // Initializations
            bTransferCompleted = true;
            nErrorCode = 0;

            // Prepare the request message
            Logix5000V21PrepareGetInstancesRequest(c, s, ref pdu, ref pduPointer, nRequestInstance, szProgram);

            // Send the request
            if (!c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer))
            {
                c.DeviceClose();
                return false;
            }

            // Read the reply
            if (!Logix5000V21ReadReply(s, ref pdu, ref pduPointer, c))
            {
                return false;
            }

            // Analyze the reply header
            if (c.AnalyzeReplyHeader(ref pdu, ecSendRRData.USHORT, c.SessionHandle.UINT) !=
                (EtherNetIpErrorCodes)DriverErrorCodes.ErrorNoError)
            {
                return false;
            }

            // Check the reply service code
            ushortUnion nDataLen = new ushortUnion(pdu, EDATA_LEN_OFFS);

            if (nDataLen.USHORT < 20)
            {
                if (!c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer))
                {
                    c.DeviceClose();
                    return false;
                }
            }
            if (pdu[MR_SVC_REPLY_CODE_OFFS] != 0xD5)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }

            ushortUnion wStatus = new ushortUnion(pdu, MR_SVC_REPLY_CODE_OFFS + 2);

            if ((wStatus.USHORT != 0) && (wStatus.USHORT != 6))
            {
                if (nErrorCode != 0)
                {
                    nErrorCode = wStatus.USHORT;
                }
                else
                {
                    Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                }

                return false;
            }

            if (wStatus.USHORT == 6)
            {
                bTransferCompleted = false;
            }

            ///////////////////////////////////////////////////////////////////////////
            // Data parsing
            nDataLen = new ushortUnion(pdu, MR_SVC_REPLY_CODE_OFFS - 2);

            // Added in version 10.1.0.7 (FOGBUGZ 7948): case no data
            if (nDataLen.USHORT == 4)
            {
                // Only status, no variables, nor routines --> Done
                return (true);
            }

            if (nDataLen.USHORT < 22)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }

            nDataLen = new ushortUnion(pdu, EDATA_LEN_OFFS);
            nDataLen.USHORT += ENCAPSULATION_HEADER_SIZE;

            ushort nTotalProcessedBytes = MR_SVC_REPLY_CODE_OFFS + 4;
            ushort nProcessedBytes = 0;
            ushort nInstance = 0;
            while (nTotalProcessedBytes < nDataLen.USHORT)
            {
                nProcessedBytes = Logix5000V21ParseInstanceInfo(s, pdu, nDataLen.USHORT,
                                                                nTotalProcessedBytes,
                                                                szProgram,
                                                                nProgramInstance,
                                                                ref nInstance);
                if (nProcessedBytes == 0)
                {
                    break;
                }
                if (nInstance != 0)
                {
                    nRequestInstance.USHORT = nInstance;
                }
                nTotalProcessedBytes += nProcessedBytes;
            }

            return (true);
        }

        public static ushort Logix5000V21ParseInstanceInfo(EtherNetIPStation s, byte[] pdu,
                                                    ushort nDataLength,
                                                    ushort nAlreadyProcessedBytes,
                                                    string szProgram,
                                                    ushort nProgramInstance,
                                                    ref ushort nProcessedInstance)
        {
            // Object Instance
            nProcessedInstance = 0;
            if ((nAlreadyProcessedBytes + 2) > nDataLength)
            {
                return (0);
            }
            ushortUnion nInstance = new ushortUnion(pdu, nAlreadyProcessedBytes);
            nProcessedInstance = nInstance.USHORT;

            // Object Name
            if ((nAlreadyProcessedBytes + 6) > nDataLength)
            {
                return (0);
            }


            ushortUnion nNameLength = new ushortUnion(pdu, (ushort)(nAlreadyProcessedBytes + 4));

            if (nNameLength.USHORT == 0)
            {
                return (0);
            }
            // Object Type
            if ((nAlreadyProcessedBytes + nNameLength.USHORT + 8) > nDataLength)
            {
                return (0);
            }
            byte[] auxArray = new byte[nNameLength.USHORT];
            Array.Copy(pdu, nAlreadyProcessedBytes + 6, auxArray, 0, nNameLength.USHORT);
            String szName = ASCIIEncoding.ASCII.GetString(auxArray);

            ushort nProcessedBytes = (ushort)(6 + nNameLength.USHORT);


            // Element Type
            if ((nAlreadyProcessedBytes + nProcessedBytes + 2) > nDataLength)
            {
                return (0);
            }
            ushortUnion nType = new ushortUnion(pdu, (ushort)(nAlreadyProcessedBytes + nProcessedBytes));
            nProcessedBytes += 2;
            // Element size
            if ((nAlreadyProcessedBytes + nProcessedBytes + 2) > nDataLength)
            {
                return (0);
            }
            ushortUnion nElementSize = new ushortUnion(pdu, (ushort)(nAlreadyProcessedBytes + nProcessedBytes));
            nProcessedBytes += 2;

            // Array dimension 1
            if ((nAlreadyProcessedBytes + nProcessedBytes + 4) > nDataLength)
            {
                return (0);
            }
            uintUnion nDim0 = new uintUnion(pdu, (ushort)(nAlreadyProcessedBytes + nProcessedBytes));
            nProcessedBytes += 4;

            // Array dimension 2
            if ((nAlreadyProcessedBytes + nProcessedBytes + 4) > nDataLength)
            {
                return (0);
            }
            uintUnion nDim1 = new uintUnion(pdu, (ushort)(nAlreadyProcessedBytes + nProcessedBytes));
            nProcessedBytes += 4;

            // Array dimension 3
            if ((nAlreadyProcessedBytes + nProcessedBytes + 4) > nDataLength)
            {
                return (0);
            }
            uintUnion nDim2 = new uintUnion(pdu, (ushort)(nAlreadyProcessedBytes + nProcessedBytes));
            nProcessedBytes += 4;

            // Object type == Program?
            if (nType.USHORT == 0x1068)
            {
                if (szProgram == "")
                {
                    s.m_listPrograms.Add(szName);
                    s.m_mapProgramsInstances[szName] = nInstance.USHORT;
                }
            }

            if (Logix5000V21TagCanBeIgnored(szName, nType.USHORT))
            {
                //TRACE(_T("V21DBG - Ignored %s, %04x\n"), szName, (UINT)nType);
                return (nProcessedBytes);
            }

            // Build the complete name of the TAG
            String szTagCompleteName = szProgram == "" ? szName : szProgram + '.' + szName;

            // Check if the tag is a structure or an array of structures
            ushort nTemplateInstance = 0;
            TagTypes nTagClass = Logix5000V21TagType(nType.USHORT);
            if ((nTagClass == TagTypes.Structure) ||
               (nTagClass == TagTypes.ArrayOfStructures))
            {
                nTemplateInstance = (ushort)(nType.USHORT & 0xfff);
                // Add an item to the template map
                if (!s.m_mapPlcTemplateInfo.ContainsKey(nTemplateInstance))
                {
                    PlcTemplateInfo pPlcTemplateInfo = new PlcTemplateInfo();
                    pPlcTemplateInfo.m_mapFieldInfo = new Dictionary<string, PlcTemplateFieldInfo>();
                    s.m_mapPlcTemplateInfo[nTemplateInstance] = pPlcTemplateInfo;
                }

            }

            // Add an item to the map of the variables instances
            if (!s.m_mapPlcTagInstanceInfo.ContainsKey(szTagCompleteName))
            {
                PlcTagInstanceInfo newPlcTagInstanceInfo = new PlcTagInstanceInfo();
                newPlcTagInstanceInfo.m_nInstance = nInstance.USHORT;
                newPlcTagInstanceInfo.m_nType = nType.USHORT;
                newPlcTagInstanceInfo.m_nProgramInstance = nProgramInstance;
                newPlcTagInstanceInfo.m_nTemplateInstance = nTemplateInstance;
                newPlcTagInstanceInfo.m_nDim0 = nDim0.UINT;
                newPlcTagInstanceInfo.m_nDim1 = nDim1.UINT;
                newPlcTagInstanceInfo.m_nDim2 = nDim2.UINT;
                newPlcTagInstanceInfo.m_szTemplateName = szName;

                s.m_mapPlcTagInstanceInfo[szTagCompleteName] = newPlcTagInstanceInfo;
            }

            return (nProcessedBytes);
        }

        //public static void AddElemetsOfArrayList(string szName, ref List<string> pListOfName, uint nDim0, uint nDim1, uint nDim2)
        //{
        //    string szArrayIndex;
        //    if (nDim2 != 0)
        //    {
        //        for (uint indexL2 = 0; indexL2 < nDim2; indexL2++)
        //        {
        //            for (uint indexL1 = 0; indexL1 < nDim1; indexL1++)
        //            {
        //                for (uint indexL0 = 0; indexL0 < nDim0; indexL0++)
        //                {
        //                    szArrayIndex = string.Format("[{0}[{1}{2}]", indexL2, indexL1, indexL0);
        //                    pListOfName.Add(szName + szArrayIndex);
        //                }
        //            }
        //        }
        //    }
        //    if (nDim1 != 0)
        //    {
        //        for (uint indexL1 = 0; indexL1 < nDim1; indexL1++)
        //        {
        //            for (uint indexL0 = 0; indexL0 < nDim0; indexL0++)
        //            {
        //                szArrayIndex = string.Format("[{0}[{1}]", indexL1, indexL0);
        //                pListOfName.Add(szName + szArrayIndex);
        //            }
        //        }
        //    }
        //    else
        //    {
        //        for (uint indexL0 = 0; indexL0 < nDim0; indexL0++)
        //        {
        //            szArrayIndex = string.Format("[{0}]", indexL0);
        //            pListOfName.Add(szName + szArrayIndex);
        //        }
        //    }
        //}

        public static TagTypes Logix5000V21TagType(ushort nVarType)
        {
            // Structure?
            if ((nVarType & 0x8000) == 0x8000)
            {
                // Array of structure?
                if ((nVarType & 0x6000) != 0x0000)
                {
                    return (TagTypes.ArrayOfStructures);
                }

                // Structure
                else
                {
                    return (TagTypes.Structure);
                }
            }

            // Array with elements of atomic type?
            if ((nVarType & 0x6000) != 0x0000)
            {
                return (TagTypes.ArrayStandard);
            }

            // Atomic tag
            return (TagTypes.Atomic);
        }

        public static bool Logix5000V21TagCanBeIgnored(string szTagName,
                                                     ushort nTagType)
        {
            // Check the tag name
            if (szTagName == "")
            {
                return (true);
            }
            if ((szTagName.Length > 1))
            {
                if ((szTagName[0] == '_') && (szTagName[1] == '_'))
                {
                    return (true);
                }
            }
            if (szTagName.IndexOf(':') >= 0)
            {
                return (true);
            }

            // Check the tag type
            // Program
            if (nTagType == 0x1068)
            {
                return (true);
            }
            // Structure
            if ((nTagType & 0x8000) == 0x8000)
            {
                return (false);
            }
            // Array
            if (!((nTagType & 0x1000) == 0x1000) && ((nTagType & 0x6000) == 0x6000))
            {
                return (false);
            }
            // Atomic tag
            if (!((nTagType & 0x1000) == 0x1000))
            {
                return (false);
            }

            return (true);
        }

        public static bool Logix5000V21ReadReply(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c)
        {

            // First read the fixed-size header
            if (!c.DeviceRead(pdu, EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE))
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }
            else
            {
                ushortUnion DataLen = new ushortUnion(pdu, EDATA_LEN_OFFS);
                if (DataLen.USHORT > 0)
                {
                    // Read the variable size data part
                    byte[] pduData = new byte[DataLen.USHORT];
                    if (!c.DeviceRead(pduData, (uint)DataLen.USHORT))
                    {
                        Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                        return false;
                    }
                    pduData.CopyTo(pdu, EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE);
                }
            }

            // Pad character to be read?
            uint nDataLen = c.GetBytesToRead();
            if (nDataLen > 1)
            {
                Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                return false;
            }
            else if (nDataLen == 1)
            {
                //read an extra char (pad)
                byte[] cPad = new byte[1];
                if (!c.DeviceRead(cPad, 1))
                {
                    Logix5550ForwardClose(s, ref pdu, ref pduPointer, c);
                    return false;
                }
            }

            return true;
        }

        public static ushort Logix5000V21PrepareGetInstancesRequest(EtherNetIPChannel c, EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer,
                           ushortUnion nRequestInstance, string szProgram = "")
        {
            s.LastTransOK = true;
            PrepareEncapsulationHeader(ecSendRRData, c, ref pdu, ref pduPointer);

            // Complete the header filling up data length
            ushort nLength = (ushort)(szProgram == "" ? 18 : 20 + ((szProgram.Length + 1) / 2) * 2);
            ushortUnion wAux = new ushortUnion(nLength);
            pdu[EDATA_LEN_OFFS] = wAux.LOBYTE;
            pdu[EDATA_LEN_OFFS + 1] = wAux.HIBYTE;

            // Complete the request message
            Logix5000V21AddCpfUnconnectedHeader(ref pdu, ref pduPointer, nLength);

            // Message Request
            // Service = 0x55 Request
            pdu[pduPointer.USHORT++] = 0x55;
            // Request path
            if (szProgram == "")
            {
                // Size of Request Path (words)
                pdu[pduPointer.USHORT++] = 0x03;

            }
            else
            {
                // Size of Request Path (words)
                pdu[pduPointer.USHORT++] = (byte)((szProgram.Length + 1) / 2 + 4);

                // Request Path = Extended Symbol Segment, 20, 6B, 25, 00, xx, xx
                EtherNetIPCommJob.GetSymbol(szProgram, pdu, ref pduPointer);

            }
            // Request Path = 20, 6B (class = Symbol Object); 25, 00, xx, yy
            // (Object Instance ID = yyxx)
            pdu[pduPointer.USHORT++] = 0x20;
            pdu[pduPointer.USHORT++] = 0x6B;
            pdu[pduPointer.USHORT++] = 0x25;
            pdu[pduPointer.USHORT++] = 0x00;
            pdu[pduPointer.USHORT++] = nRequestInstance.LOBYTE;
            pdu[pduPointer.USHORT++] = nRequestInstance.HIBYTE;

            // List of requested attributes
            pdu[pduPointer.USHORT++] = 0x04; // Number of requested attributes (low byte)
            pdu[pduPointer.USHORT++] = 0x00; // Number of requested attributes (high byte)
            pdu[pduPointer.USHORT++] = 0x01; // Attribute 1 = Symbol Name
            pdu[pduPointer.USHORT++] = 0x00;
            pdu[pduPointer.USHORT++] = 0x02; // Attribute 2 = Symbol Type
            pdu[pduPointer.USHORT++] = 0x00;
            pdu[pduPointer.USHORT++] = 0x07; // Attribute 7 = Element Size
            pdu[pduPointer.USHORT++] = 0x00;
            pdu[pduPointer.USHORT++] = 0x08; // Attribute 8 = Array Sizes
            pdu[pduPointer.USHORT++] = 0x00;

            // Route Path Size
            pdu[pduPointer.USHORT++] = 0x01;

            // Reserved
            pdu[pduPointer.USHORT++] = 0x00;

            // Route Path: Port, Address
            pdu[pduPointer.USHORT++] = 0x01;
            pdu[pduPointer.USHORT++] = s.CPUSlot;

            return pduPointer.USHORT;
        }

        public static void Logix5000V21AddCpfUnconnectedHeader(ref byte[] pdu, ref ushortUnion pduPointer, ushort nEncapsulatedMsgLength)
        {
            // Handle = 0 for CIP encapsulated messages
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            // Timeout (seconds)
            pdu[pduPointer.USHORT++] = 0x0A;
            pdu[pduPointer.USHORT++] = 0;

            // Item Count
            pdu[pduPointer.USHORT++] = 2;
            pdu[pduPointer.USHORT++] = 0;

            // Address Item - Item Type = NULL Address Item
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            //Address Item - Item Data Length = 0
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            //Data Item - Item Type = Unconnected Data Item
            pdu[pduPointer.USHORT++] = 0xB2;
            pdu[pduPointer.USHORT++] = 0;
            //Data Item - Item Data Length
            ushortUnion wAux = new ushortUnion((ushort)(nEncapsulatedMsgLength + 14));
            pdu[pduPointer.USHORT++] = wAux.LOBYTE;
            pdu[pduPointer.USHORT++] = wAux.HIBYTE;

            // Service = 0x52 = Unconnected Send Request
            pdu[pduPointer.USHORT++] = 0x52;

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

            // Message request length
            wAux.USHORT = nEncapsulatedMsgLength;
            pdu[pduPointer.USHORT++] = wAux.LOBYTE;
            pdu[pduPointer.USHORT++] = wAux.HIBYTE;
        }

        public static void Logix5550NonBlockPhAddPrepareGetCpuTypeRequest(EtherNetIPChannel c, EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            s.LastTransOK = true;
            // Ethernet/IP Encapsulation Header
            PrepareEncapsulationHeader(ecSendRRData, c, ref pdu, ref pduPointer);

            // Common Industrial Protocol request
            ushortUnion nDataLen = new ushortUnion(Logix5550NonBlockPhAddPrepareGetCpuTypeCPF(s, ref pdu, ref pduPointer));

            // Fill in packet data length
            pdu[EDATA_LEN_OFFS] = nDataLen.LOBYTE;
            pdu[EDATA_LEN_OFFS + 1] = nDataLen.HIBYTE;

        }

        public static ushort Logix5550NonBlockPhAddPrepareGetCpuTypeCPF(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort nStartCh = pduPointer.USHORT;

            // Handle = 0 for CIP encapsulated messages
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            // Timeout (seconds)
            pdu[pduPointer.USHORT++] = 0x0A;
            pdu[pduPointer.USHORT++] = 0;

            // Common Packet Format (CPF)
            // Item Count
            pdu[pduPointer.USHORT++] = 2;
            pdu[pduPointer.USHORT++] = 0;

            // Common Packet Format (CPF)
            // Address Item - Item Type = NULL Address Item
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            // Common Packet Format (CPF)
            // Address Item - Item Data Length = 0
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            // Common Packet Format (CPF)
            // Data Item - Item Type = Unconnected Data Item
            pdu[pduPointer.USHORT++] = 0xB2;
            pdu[pduPointer.USHORT++] = 0;

            // Common Packet Format (CPF)
            // Data Item - Item Data Length = variable (filled up later)
            ushort nCPFDataLenOffs = pduPointer.USHORT;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            ushortUnion nPDULen = new ushortUnion(Logix5550NonBlockPhAddPrepareGetCpuTypeTPDU(s, ref pdu, ref pduPointer));

            //Fill up data length
            pdu[nCPFDataLenOffs] = nPDULen.LOBYTE;
            pdu[nCPFDataLenOffs + 1] = nPDULen.HIBYTE;

            return (ushort)(pduPointer.USHORT - nStartCh);
        }

        public static ushort Logix5550NonBlockPhAddPrepareGetCpuTypeTPDU(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort nStartCh = pduPointer.USHORT;

            // Service = 0x52 = Unconnected Send Request
            pdu[pduPointer.USHORT++] = 0x52;

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

            // Message request length (filled up later)
            ushort nMessageDataLenOffs = pduPointer.USHORT;
            pdu[pduPointer.USHORT++] = 0x00;
            pdu[pduPointer.USHORT++] = 0x00;

            // Message Request
            ushortUnion nMsgLen = new ushortUnion(Logix5550NonBlockPhAddPrepareGetCpuTypeMsgRequest(ref pdu, ref pduPointer));

            // Fill up message request length
            pdu[nMessageDataLenOffs] = nMsgLen.LOBYTE;
            pdu[nMessageDataLenOffs + 1] = nMsgLen.HIBYTE;

            // Route Path Size
            pdu[pduPointer.USHORT++] = 0x01;

            // Reserved
            pdu[pduPointer.USHORT++] = 0x00;

            // Route Path: Port, Address
            pdu[pduPointer.USHORT++] = 0x01;
            pdu[pduPointer.USHORT++] = s.CPUSlot;

            return (ushort)(pduPointer.USHORT - nStartCh);
        }

        public static ushort Logix5550NonBlockPhAddPrepareGetCpuTypeMsgRequest(ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort nStartCh = pduPointer.USHORT;

            // Service = 0x01 = Get Attribute All
            pdu[pduPointer.USHORT++] = 0x01;
            // Size of Request Path (words)
            pdu[pduPointer.USHORT++] = 0x02;
            // Request Path = 20, 01 (class = Identity Object); 24, 01 = Instance 1
            pdu[pduPointer.USHORT++] = 0x20;
            pdu[pduPointer.USHORT++] = 0x01;
            pdu[pduPointer.USHORT++] = 0x24;
            pdu[pduPointer.USHORT++] = 0x01;

            return (ushort)(pduPointer.USHORT - nStartCh);
        }



        public static ushort PrepareDataFileRequest(EtherNetIPCommJob job, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            EtherNetIPStation s = job.Station as EtherNetIPStation;
            if (s == null)
                return 0;

            if ((job.CommandType != CommandTypes.ReadCmd) && (job.CommandType != CommandTypes.WriteCmd))
                return 0;

            EtherNetIPChannel c = (EtherNetIPChannel)s.GetChannel();
            PrepareEncapsulationHeader(ecSendRRData, c, ref pdu, ref pduPointer);
            return (PrepareEncapsulationData(ecSendRRData, job, ref pdu, ref pduPointer));
        }

        public static ushort Logix5550PrepareRequest(ref List<CommJob> list, EtherNetIPChannel c, ref EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            // Prepare encapsulation header for PCCC read/write request
            PrepareEncapsulationHeader(ecSendUnitData, c, ref pdu, ref pduPointer);

            List<PrepareHeader> prepareHeaderList = new List<PrepareHeader>();
            PrepareHeader logix5550PrepareRequestHeader = Logix5550PrepareRequestHeader;

            prepareHeaderList.Add(logix5550PrepareRequestHeader);

            if (s.PlcType != PlcTypes.Micro800_series)
            {
                PrepareHeader logix5550TagsPrepareCpfReqHeader = Logix5550TagsPrepareCpfReqHeader;
                prepareHeaderList.Add(logix5550TagsPrepareCpfReqHeader);
            }
            // Complete the request message
            return PrepareEncapsulateRequest(ref prepareHeaderList, ref list, ref s, ref pdu, ref pduPointer);
        }

        public static ushort PrepareEncapsulationData(ushortUnion Cmd, EtherNetIPCommJob job, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            EtherNetIPStation s = job.Station as EtherNetIPStation;
            if (s == null)
                return 0;

            //at the moment we only support Unconnected Requests (SendRRData);
            if (Cmd.USHORT != EtherNetIpProtocol.ecSendRRData.USHORT)
                return 0;

            ushort StartCh = pduPointer.USHORT;

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
            ushortUnion PDULen = new ushortUnion(PrepareUnconnectedTPDU(s, job, ref pdu, ref pduPointer));

            if (PDULen.USHORT == 0)
            {
                return 0;
            }

            //Fill up data length
            pdu[CPFDataLenOffs] = PDULen.LOBYTE;
            pdu[CPFDataLenOffs + 1] = PDULen.HIBYTE;

            return ((ushort)(pduPointer.USHORT - StartCh));
        }

        public static ushort PrepareUnconnectedTPDU(EtherNetIPStation station, EtherNetIPCommJob job, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort StartCh = pduPointer.USHORT;

            /*At the moment we use Embedded PCCC Commands also for ControlLogix,
             * but in a future version the "native" CIP requests will be implemented.
             * The limitation is that we can only access ControlLogix "atomic" data types*/
            if (station.PlcType == PlcTypes.SLC500_MicroLogix ||
                station.PlcType == PlcTypes.PLC5 ||
                station.PlcType == PlcTypes.ControlLogix_CompactLogix)
            {
                //Message Router Service Request
                //Service Code = Execute PCCC service request (0x4B)
                pdu[pduPointer.USHORT++] = 0x4B;

                //Message Router Service Request
                //Size of Request Path = 2 (words)
                pdu[pduPointer.USHORT++] = 0x02;

                //Message Router Service Request
                //Request Path = 20,67 (class PCCC); 24,01 (instance 01)
                pdu[pduPointer.USHORT++] = 0x20;
                pdu[pduPointer.USHORT++] = 0x67;
                pdu[pduPointer.USHORT++] = 0x24;
                pdu[pduPointer.USHORT++] = 0x01;

                //Message Router Service Request Data
                //Execute_PCCC Requestor ID
                //length = vendor+s/n+other+1)
                pdu[pduPointer.USHORT++] = 0x07;

                //Message Router Service Request Data
                //Execute_PCCC Requestor ID
                //Vendor ID
                pdu[pduPointer.USHORT++] = VENDOR_ID.LOBYTE;
                pdu[pduPointer.USHORT++] = VENDOR_ID.HIBYTE;

                //Message Router Service Request Data
                //Execute_PCCC Requestor ID
                //CIP Serial #
                pdu[pduPointer.USHORT++] = CIP_SERIAL_NUMBER.LOUSHORT.LOBYTE;
                pdu[pduPointer.USHORT++] = CIP_SERIAL_NUMBER.LOUSHORT.HIBYTE;
                pdu[pduPointer.USHORT++] = CIP_SERIAL_NUMBER.HIUSHORT.LOBYTE;
                pdu[pduPointer.USHORT++] = CIP_SERIAL_NUMBER.HIUSHORT.HIBYTE;

                //Other is not present

                //===============================PCCC Command================================
                pdu[pduPointer.USHORT++] = 0x0F;       // CMD

                pdu[pduPointer.USHORT++] = 0;          // STS

                pdu[pduPointer.USHORT++] = station.GetTransaction().LOBYTE;       // TNS
                pdu[pduPointer.USHORT++] = station.GetTransaction().HIBYTE;       // TNS


                ushort offset = 0;
                //prepare a write request
                UInt16 nData = 0;
                byte[] jobdata = new byte[nData];

                job.CommandType = (job.ReadRequest() ? CommandTypes.ReadCmd : CommandTypes.WriteCmd);

                if (job.CommandType == CommandTypes.WriteCmd)
                {                    
                    object jobDataObject = null;
                    job.GetJobData(ref jobDataObject);
                    if (job.TagsListOnWriting.Count == 0 || jobDataObject == null)
                        return 0;

                    jobdata = (byte[])jobDataObject;
                    nData = (UInt16)jobdata.Count();
                    offset = (ushort)job.TagsListOnWriting[0].ByteOffset;
                }

                ushortUnion Temp = new ushortUnion(0);
                switch (station.PlcType)
                {
                    case PlcTypes.SLC500_MicroLogix:

                        byte jobSize = (byte)job.TotalJobSize;
                        if (job.CommandType == CommandTypes.ReadCmd)
                        {
                            pdu[pduPointer.USHORT++] = 0xA2;       // FNC = Protected Typed Logical Read with Three Address Fields
                        }
                        else if (job.CommandType == CommandTypes.WriteCmd)
                        {
                            //For arrays of LongInteger type booleans, 
                            //greater than 16 elements for writing are written as if they were a DWORD, then all 32 elements.
                            if ((job.DataFormat != DataFormats.BIT) || (job.TagsList[0].TagNode.ArrayDimension > 16))
                            {
                                pdu[pduPointer.USHORT++] = 0xAA;       // FNC = Protected Typed Logical Write with Three Address Fields
                            }
                            else
                            {
                                pdu[pduPointer.USHORT++] = 0xAB;       // FNC = Protected Typed Logical Bit Write Masked with Three Address Fields (undocumented function!)
                            }
                            jobSize = (byte)nData;
                        }

                        //For arrays of LongInteger type booleans, 
                        //greater than 16 elements for writing are written as if they were a DWORD, then all 32 elements.
                        if ((job.DataFormat != DataFormats.BIT) || (job.TagsList[0].TagNode.ArrayDimension > 16))
                        {
                            pdu[pduPointer.USHORT++] = jobSize;
                        }
                        else
                        {
                            //Byte Size
                            if ((job.CommandType == CommandTypes.ReadCmd) && (job.FileType == FileTypes.LongInteger))
                            {
                                pdu[pduPointer.USHORT++] = 4;
                            } 
                            else
                            {
                                pdu[pduPointer.USHORT++] = 2;
                            }
                            
                        }

                        // Insert File Number
                        writeUshort((ushort)job.FileNum, ref pdu, ref pduPointer);


                        pdu[pduPointer.USHORT++] = GetFileTypeCode(job.FileType);   // File type code

                        // Insert the Element Number
                        //recalculate in case we have an offset (happens only in write mode)
                        //the offset is divided by 4 bytes only in case of float file, by 2 in all other cases

                        if (!job.IsFileTypeIO())
                        {
                            //Element Number
                            if ((job.CommandType == CommandTypes.WriteCmd) &&
                                (job.DataFormat == DataFormats.BIT))
                            {
                                writeUshort((ushort)job.Element, ref pdu, ref pduPointer);
                            }
                            else
                            {
                                writeUshort((ushort)(job.Element + (offset / job.ElemSize)), ref pdu, ref pduPointer);
                            }
                        }
                        else
                        {
                            writeUshort((ushort)job.Slot, ref pdu, ref pduPointer);
                        }


                        if (job.IsFileTypeTC())
                        {
                            pdu[pduPointer.USHORT++] = (byte)job.SubElement;
                        }
                        else if (job.IsFileTypeIO())
                        {
                            uint WordNumber;
                            if ((job.CommandType == CommandTypes.WriteCmd) &&
                                (job.DataFormat == DataFormats.BIT))
                            {
                                WordNumber = job.Word + (ushort)((job.Bit + offset) >> 4);
                            }
                            else
                            {
                                WordNumber = job.Word + (ushort)(offset / job.ElemSize);
                            }

                            writeUshort((ushort)WordNumber, ref pdu, ref pduPointer);
                        }
                        else
                        {
                            //Subelement Number
                            if (job.CommandType == CommandTypes.ReadCmd)
                            {
                                pdu[pduPointer.USHORT++] = 0;
                            }
                            else if (!((job.DataFormat == DataFormats.BIT) && (job.Bit > 15)))
                            { 
                                 pdu[pduPointer.USHORT++] = 0;
                            }
                            else
                            {
                                pdu[pduPointer.USHORT++] = 1;
                            }
                        }

                        if (job.CommandType == CommandTypes.WriteCmd)
                        {
                            //prepare a write request
                            // Copy data to be written
                            //For arrays of LongInteger type booleans, 
                            //greater than 16 elements for writing are written as if they were a DWORD, then all 32 elements.
                            if ((job.DataFormat == DataFormats.BIT) && (job.TagsList[0].TagNode.ArrayDimension < 17))
                            {
                                byte BitToWrite = 0;
                                ushort AggregateWritingMask = 0;
                                ushort shift = 0;
                                ushort AggregateWriteValues = 0;
                                //Aggregates writing bit management
                                if (job.TagsListOnWriting.Count > 1)
                                {
                                    //Index for bitarray object scrolling
                                    ushort indexForBitarrayObjectScrolling = 0;

                                    //Conversion of the byte array (max 2 bytes because the write command manages only one word) 
                                    // in an array of bit1 example: 5 = True,False,True,False,False,False,False,False
                                    System.Collections.BitArray myBA1 = new System.Collections.BitArray(jobdata);
                                    myBA1.CopyTo(jobdata, 0);

                                    //Creation of the mask and the values to be written
                                    foreach (EtherNetIPTag tag in job.TagsListOnWriting )
                                    {
                                        shift = (ushort)((tag.EtherNetIPDynSettings.Bit ) % 16);
                                        AggregateWritingMask |= (ushort) (1 << shift);
                                        byte value =(byte) (myBA1.Get(indexForBitarrayObjectScrolling) ? 1 : 0);
                                        AggregateWriteValues |= (ushort) (value << shift);
                                        indexForBitarrayObjectScrolling++;
                                    }
                                } 
                                else
                                {
                                    BitToWrite = (byte)((job.Bit + offset) % 16);
                                }
                                //byte BitToWrite = (byte)((job.Bit + offset) % 16);
                                uint DataTypeBitSize = job.TagsList[0].TagNode.ArrayDimension == 0 ? 1 : job.TagsList[0].TagNode.ArrayDimension;
                                if (job.ElementNumber == 0)
                                    DataTypeBitSize = DataTypeBitSize * CommJob.GetDataTypeBitSize((uint)job.TagsList[0].TagNode.DataType.Identifier);
                                ushortUnion Mask = new ushortUnion(0);
                                ushortUnion Val = new ushortUnion(0);
                                if (DataTypeBitSize == 1)
                                {
                                    //Aggregates writing bit management
                                    if (job.TagsListOnWriting.Count > 1)
                                    {                                        
                                        Mask.USHORT = (ushort)(AggregateWritingMask);
                                        Val.USHORT = (ushort)(AggregateWriteValues);
                                    }
                                    else
                                    {
                                       Mask.USHORT = (ushort)(1 << BitToWrite);
                                       Val.USHORT = (ushort)((jobdata[0] & 1) << BitToWrite);
                                    }
                                }
                                else
                                {
                                    Val.LOBYTE = jobdata[0];
                                    if (jobdata.Count() > 1)
                                        Val.HIBYTE = jobdata[1];
                                    for (int i = 0; i < DataTypeBitSize; i++)
                                        Mask.USHORT = (ushort)(Mask.USHORT + (1 << i));

                                }
                                //Mask
                                pdu[pduPointer.USHORT++] = Mask.LOBYTE;
                                pdu[pduPointer.USHORT++] = Mask.HIBYTE;

                                //Value to write
                                pdu[pduPointer.USHORT++] = Val.LOBYTE;
                                pdu[pduPointer.USHORT++] = Val.HIBYTE;
                            }
                            else
                            {
                                for (uint i = 0; i < nData; ++i)
                                {
                                    pdu[pduPointer.USHORT++] = jobdata[i];
                                }
                            }
                        }
                        break;

                    case PlcTypes.ControlLogix_CompactLogix:
                    case PlcTypes.PLC5:
                        //make a scratch copy of the address
                        bool bBitWrite = false;

                        if (job.CommandType == CommandTypes.ReadCmd)
                        {
                            pdu[pduPointer.USHORT++] = 0x68;       // FNC = Typed read
                        }
                        else if (job.CommandType == CommandTypes.WriteCmd)
                        {
                            if (job.DataFormat != DataFormats.BIT)
                            {
                                pdu[pduPointer.USHORT++] = 0x67;       // FNC = Typed write
                            }
                            else
                            {
                                bBitWrite = true;
                                pdu[pduPointer.USHORT++] = 0x26;       // FNC = Read Modify Write
                            }
                        }

                        if (!bBitWrite)
                        {
                            pdu[pduPointer.USHORT++] = 0;		     // Packet offset low byte
                            pdu[pduPointer.USHORT++] = 0;		     // Packet offset high byte

                            Temp.USHORT = (ushort)(nData / job.ElemSize);
                            pdu[pduPointer.USHORT++] = Temp.LOBYTE;       // Total transaction low byte
                            pdu[pduPointer.USHORT++] = Temp.HIBYTE;		     // Total transaction high byte
                        }

                        // Start of PLC5 sys.address
                        pdu[pduPointer.USHORT++] = 0;
                        pdu[pduPointer.USHORT++] = (byte)'$';		 // (Logical ASCII Addressing)


                        String strAdd = job.ABString(offset);
                        if(strAdd == String.Empty)
                        {
                            return (0);
                        }

                        for (int i = 0; i < strAdd.Length && strAdd[i] != ('/'); i++)
                        {
                            pdu[pduPointer.USHORT++] = (byte)strAdd[i];
                        }

                        //End of address
                        pdu[pduPointer.USHORT++] = 0;

                        if (job.CommandType == CommandTypes.ReadCmd)
                        {
                            if (job.DataFormat != DataFormats.BIT)
                            {
                                Temp.USHORT = (ushort)(nData / job.ElemSize);
                            }
                            else
                            {
                                Temp.USHORT = (ushort)((job.Bit + nData - 1) / 16 + 1);
                            }
                            // Size
                            pdu[pduPointer.USHORT++] = Temp.LOBYTE;
                            pdu[pduPointer.USHORT++] = Temp.HIBYTE;
                        }
                        else
                        {
                            if (job.DataFormat != DataFormats.BIT)
                            {
                                //flag byte
                                //bits 4..7 = 1001 -> data type ID in next byte
                                //bits 0..3 = 1010 -> size encoded in next 2 bytes after type
                                pdu[pduPointer.USHORT++] = 0x9A;

                                //type ID = 9 (array)
                                pdu[pduPointer.USHORT++] = 0x09;

                                //size LSB (+1 for element type byte)
                                Temp.USHORT = (ushort)(nData + (job.DataFormat == DataFormats.WORD ? 1 : 2));
                                pdu[pduPointer.USHORT++] = Temp.LOBYTE;
                                //size MSB
                                pdu[pduPointer.USHORT++] = Temp.HIBYTE;

                                //element type byte
                                switch (job.DataFormat)
                                {
                                    case DataFormats.WORD:
                                        pdu[pduPointer.USHORT++] = 0x42;	//4=integer, 2 bytes per element
                                        break;
                                    case DataFormats.DWORD:
                                        pdu[pduPointer.USHORT++] = 0x94;	//9=type ID in next byte, 4 bytes per element
                                        pdu[pduPointer.USHORT++] = 0x08;	//Type ID = IEEE Floating point
                                        break;
                                }
                            }
                            else
                            {
                                //only one bit can be written at once
                                //Q_ASSERT(nData == 1);
                                ushortUnion wAndMask = new ushortUnion(0xFFFF);
                                ushortUnion wOrMask = new ushortUnion(0);
                                ushort wTemp = (ushort)(1 << (byte)((job.Bit + offset) % 16));
                                if (jobdata[0] != 0)
                                {
                                    wOrMask.USHORT |= wTemp;
                                }
                                else
                                {
                                    wTemp = (ushort)~wTemp;
                                    wAndMask.USHORT &= wTemp;
                                }
                                //AND Mask
                                pdu[pduPointer.USHORT++] = wAndMask.LOBYTE;
                                pdu[pduPointer.USHORT++] = wAndMask.HIBYTE;

                                //OR Mask
                                pdu[pduPointer.USHORT++] = wOrMask.LOBYTE;
                                pdu[pduPointer.USHORT++] = wOrMask.HIBYTE;
                            }
                            //and eventually....data to be written
                            if (!bBitWrite)
                            {
                                for (uint i = 0; i < nData; ++i)
                                {
                                    pdu[pduPointer.USHORT++] = jobdata[i];
                                }
                            }
                        }
                        break;
                }//switch

            }

            return ((ushort)(pduPointer.USHORT - StartCh));
        }

        public static ushort Logix5550PrepareUnconnectedRequest(EtherNetIPCommJob job, EtherNetIPChannel c, EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, ProtocolParameterSet pps = ProtocolParameterSet.None)
        {
            ushort StartCh = pduPointer.USHORT;
            PrepareEncapsulationHeader(ecSendRRData, c, ref pdu, ref pduPointer);

            // Complete the request message
            Logix5550PrepareUnconnectedCPF(job, s, ref pdu, ref pduPointer, pps);
            return ((ushort)(pduPointer.USHORT - StartCh));
        }

        public static ushort Logix5550PrepareUnconnectedCPF(EtherNetIPCommJob job, EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, ProtocolParameterSet pps = ProtocolParameterSet.None)
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
            ushortUnion PDULen = new ushortUnion(Logix5550PrepareUnconnectedTPDU(job, s, ref pdu, ref pduPointer,pps));

            if (PDULen.USHORT == 0)
            {
                return 0;
            }

            //Fill up data length
            pdu[CPFDataLenOffs] = PDULen.LOBYTE;
            pdu[CPFDataLenOffs + 1] = PDULen.HIBYTE;

            return ((ushort)(pduPointer.USHORT - StartCh));
        }

        private static ushort Logix5550GetNewConnectionID(int seed)
        {
            Random random = new Random(seed);
            return (ushort)random.Next(65535);
        }

        public static ushort Logix5550PrepareUnconnectedTPDU(EtherNetIPCommJob job, EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, ProtocolParameterSet pps = ProtocolParameterSet.None)
        {
            if (s.PlcType == PlcTypes.Micro800_series)
            {
                return Logix5550PrepareUnconnectedTPDUMicro800(job, s, ref pdu, ref pduPointer);
            }
            ushort StartCh = pduPointer.USHORT;

            if (job.CommandType == CommandTypes.FwdOpen)
            {
                //Service Code = Forward Open service request (0x54)
                pdu[pduPointer.USHORT++] = 0x54;
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

            if (job.CommandType == CommandTypes.FwdOpen)
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

            if (job.CommandType == CommandTypes.FwdOpen)
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
                pdu[pduPointer.USHORT++] = 0xF8;
                if (pps == ProtocolParameterSet.DisableRedundantO_TNetwork)
                    pdu[pduPointer.USHORT++] = 0x47;
                else
                    pdu[pduPointer.USHORT++] = 0xC7;

                // Target > Originator Requested Packet Interval
                pdu[pduPointer.USHORT++] = 0x80;
                pdu[pduPointer.USHORT++] = 0x84;
                pdu[pduPointer.USHORT++] = 0x1E;
                pdu[pduPointer.USHORT++] = 0;

                // Target > Originator Network Connection Parameters
                pdu[pduPointer.USHORT++] = 0xF8;
                if (pps == ProtocolParameterSet.DisableRedundantO_TNetwork)
                    pdu[pduPointer.USHORT++] = 0x47;
                else
                    pdu[pduPointer.USHORT++] = 0xC7;

                // Transport type / Trigger
                pdu[pduPointer.USHORT++] = 0xA3;
            }

            // Connection path size (words)
            pdu[pduPointer.USHORT++] = 0x03;

            if (job.CommandType == CommandTypes.FwdClose)
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

        public static ushort Logix5550PrepareUnconnectedTPDUMicro800(EtherNetIPCommJob job, EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort StartCh = pduPointer.USHORT;

            if (job.CommandType == CommandTypes.FwdOpen)
            {
                //Service Code = Forward Open service request (0x54)
                pdu[pduPointer.USHORT++] = 0x54;
            }
            else
            {
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

            if (job.CommandType == CommandTypes.FwdOpen)
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

            if (job.CommandType == CommandTypes.FwdOpen)
            {
                // Connection timeout multiplier
                //pdu[pduPointer.USHORT++] = 0x02;
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

                // Originator > Target Network Connection Parameters (ox47F8):
                // Connection Size = 504
                // Variable amount of data
                // High Priority
                // Connection Point to Point
                // Exclusive Owner
                pdu[pduPointer.USHORT++] = 0xF8;
                pdu[pduPointer.USHORT++] = 0x47;

                // Target > Originator Requested Packet Interval
                pdu[pduPointer.USHORT++] = 0x80;
                pdu[pduPointer.USHORT++] = 0x84;
                pdu[pduPointer.USHORT++] = 0x1E;
                pdu[pduPointer.USHORT++] = 0;

                pdu[pduPointer.USHORT++] = 0xF8;
                pdu[pduPointer.USHORT++] = 0x47;

                // Transport type / Trigger
                pdu[pduPointer.USHORT++] = 0xA3;
            }

            pdu[pduPointer.USHORT++] = 0x02;

            pdu[pduPointer.USHORT++] = 0x20;
            pdu[pduPointer.USHORT++] = 0x02;
            pdu[pduPointer.USHORT++] = 0x24;
            pdu[pduPointer.USHORT++] = 0x01;

            return ((ushort)(pduPointer.USHORT - StartCh));
        }


        public static void Logix5550PrepareRequestHeader(
            EtherNetIPStation s,
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
            EtherNetIPStation s,
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


        public static bool getWriteListLimitate(ref EtherNetIPCommJob j, ref ReadWriteListLimitateSize requestSize, ReadWriteLimitate mode)
        {
            if (requestSize.TotalSize == 0)
                requestSize.TotalSize = LOGIX5550_CONNECTION_SIZE_HEADER + 2;

            //foreach (EtherNetIPCommJob j in nextlist)
            //{
            //// check if ExceptionOutput job can be "closed" immediatly because data is not changed or has no values to be written
            //if (j.Type == LinkType.ExceptionOutput)
            //{
            //    // no data to write
            //    if (j.TagsListToWrite.Count == 0)
            //    {
            //        queueEmptyJobs.Add(j);
            //        continue;
            //    }

            //    // data to write is not changed
            //    if (!j.IsDataToWriteChanged(j.TagsList[0]))
            //    {                        
            //        queueEmptyJobs.Add(j);
            //        continue;
            //    }
            //}
            foreach (Tag tag in j.TagsList) {
                if (j.TagFormat == TagFormats.STRING)
                {
                    requestSize.TotalSize += j.GetTagReadBufferSize(tag, "DATA");
                    requestSize.TotalSize += j.GetTagReadBufferSize(tag, "LEN");
                }
                else
                    requestSize.TotalSize += j.GetTagReadBufferSize(tag);
                requestSize.TotalSize += 6;
                requestSize.TotalSize += (ushort)(j.adjSizeWrite(tag) - j.PartialArrayStart * j.ElemSize);
                if (requestSize.TotalSize > LOGIX5550_CONNECTION_SIZE)
                {
                    if (tag.Size / j.ElemSize <= 1)
                        return false;
                    int tmp = (j.adjSizeWrite(tag) + LOGIX5550_CONNECTION_SIZE + 1 - requestSize.TotalSize - j.ElemSize) / j.ElemSize;
                    tmp -= j.PartialArrayStart;
                    if (tmp > 0)
                    {
                        if (mode == ReadWriteLimitate.Add)
                            j.PartialArrayEnd = (ushort)(tmp + j.PartialArrayStart);
                        //nextlistLimitate.Add(j);
                    }
                    //System.Diagnostics.Trace.TraceInformation("--DEBUG-- EthernetIpProtocol  getWriteListLimitate writeSize:{0} tmp:{1} PartialArrayStart:{2}  :{3}", writeSize, tmp, j.PartialArrayStart, DateTime.UtcNow.ToString());
                    return (requestSize.TotalNrJobs == 0);
                }
            }
            //nextlistLimitate.Add(j);
            j.PartialArrayEnd = 0;
            // Allow writing just one tag per time
            //break;
            //}

            return true;
        }

        public static void GetTheWriteLimit(EtherNetIPCommJob j)
        {
            //ushort writeSize = LOGIX5550_CONNECTION_SIZE_HEADER + 2;
            //foreach (Tag tag in j.TagsList)
            //{
            //    if (j.TagFormat == TagFormats.STRING)
            //    {
            //        writeSize += j.GetTagReadBufferSize(tag, "DATA");
            //        writeSize += j.GetTagReadBufferSize(tag, "LEN");
            //    }
            //    else
            //        writeSize += j.GetTagReadBufferSize(tag);
            //    writeSize += 6;
            //    writeSize += (ushort)(j.adjSizeWrite(tag) - j.PartialArrayStart * j.ElemSize);
            //    if (writeSize > LOGIX5550_CONNECTION_SIZE)
            //    {
            //        if (tag.Size / j.ElemSize <= 1)
            //            return;
            //        //System.Diagnostics.Debug.WriteLine("@DEBUG GetTheWriteLimit j.adjSizeWrite(tag):{0} LOGIX5550_CONNECTION_SIZE:{1} writeSize:{2} j.ElemSize:{3}", j.adjSizeWrite(tag), LOGIX5550_CONNECTION_SIZE, writeSize, j.ElemSize);
            //        int tmpNumElemet = (j.adjSizeWrite(tag) + LOGIX5550_CONNECTION_SIZE + 1 - writeSize - j.ElemSize) / j.ElemSize;
            //        tmpNumElemet -= j.PartialArrayStart;
            //        if (tmpNumElemet > 0)
            //        {
            //            j.PartialArrayEnd = (ushort)(tmpNumElemet + j.PartialArrayStart);
            //        }
            //        //System.Diagnostics.Trace.TraceInformation("--DEBUG-- EthernetIpProtocol  getWriteListLimitate writeSize:{0} tmp:{1} PartialArrayStart:{2}  :{3}", writeSize, tmp, j.PartialArrayStart, DateTime.UtcNow.ToString());
            //        return;
            //    }
            //}
            //j.PartialArrayEnd = 0;       

            ReadWriteListLimitateSize requestSize = new ReadWriteListLimitateSize();
            // use ReadWriteLimitate.EvaluateOnly option because internal parameter for big array must be updated only during ExcecuteJob
            getWriteListLimitate(ref j, ref requestSize, ReadWriteLimitate.EvaluateOnly);
        }

        public static ushort Logix5550TagsPrepareMultiWriteReq(ref List<CommJob> listJobs, ref EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort StartCh = pduPointer.USHORT;

            // calculate nr elements to insert into frame (string type require element name + element size)
            s.lastProcessedTags.USHORT = (ushort)(listJobs.Count + listJobs.Count(j => ((EtherNetIPCommJob)j).TagFormat == TagFormats.STRING));

            ushortUnion OffsetIndex = new ushortUnion(pduPointer.USHORT);
            pduPointer.USHORT += (ushort)(s.lastProcessedTags.USHORT * 2);

            ushortUnion OffsetValue = new ushortUnion((ushort)(2 + pduPointer.USHORT - StartCh));

            //System.Diagnostics.Debug.WriteLine(string.Format("Logix5550TagsPrepareMultiWriteReqNew nr jobs {0}", listJobs.Count));
            List<EtherNetIPCommJob> wrList = new List<EtherNetIPCommJob>();
            foreach (EtherNetIPCommJob j in listJobs)
            {
                //System.Diagnostics.Debug.WriteLine(string.Format("Logix5550TagsPrepareMultiWriteReqNew {0}", j.TagsList[0].DynSettings.ToString()));

                Tag tag;
                //lock (j.retLockList())
                {
                    if (j.GetTagListOnWritingCount() == 0)
                        return 0;
                    tag = j.GetTagListOnWriting()[0];
                }

                // Set the service request offset
                pdu[OffsetIndex.USHORT++] = OffsetValue.LOBYTE;
                pdu[OffsetIndex.USHORT++] = OffsetValue.HIBYTE;

                // Set the service request
                pdu[pduPointer.USHORT++] = 0x4D;    // Service code = write
                OffsetValue.USHORT++;
                // Special case: strings
                if (j.TagFormat == TagFormats.STRING)
                {
                    // Add the field "DATA" to the IOI string (strings are
                    // predefined structures in Allen - Bradley PLCs).
                    OffsetValue.USHORT += j.GetAsciiTagname(s, ref pdu, ref pduPointer, tag, "DATA");
                }
                else
                {
                    GetTheWriteLimit(j);
                    OffsetValue.USHORT += j.GetAsciiTagname(s, ref pdu, ref pduPointer, tag);
                }

                OffsetValue.USHORT += j.GetTagFormat(ref pdu, ref pduPointer);

                // Number of elements to be written
                OffsetValue.USHORT += j.GetNumOfElements(ref pdu, ref pduPointer, tag, true);

                ushort startData = pduPointer.USHORT;

                ushort WriteLenght = j.GetTagWriteData(ref pdu, ref pduPointer, tag);
                // no data to write --> inputoutput/exception output with same value of driver
                if (WriteLenght == 0)
                    return 0;

                ushort endData = pduPointer.USHORT;

                // Add data
                OffsetValue.USHORT += WriteLenght;

                if (j.PartialArrayEnd == 0)
                {
                    //lock (j.retLockList())
                    {
                        j.TagsListToWrite.Remove(tag);
                        if (!j.TagsListOnWriting.Contains(tag))
                            j.TagsListOnWriting.Add(tag);
                    }
                }

                if (j.TagFormat == TagFormats.STRING)
                {
                    // Add the field "LEN" to the IOI string (strings are
                    // predefined structures in Allen - Bradley PLCs).

                    pdu[OffsetIndex.USHORT++] = OffsetValue.LOBYTE;
                    pdu[OffsetIndex.USHORT++] = OffsetValue.HIBYTE;

                    // Set the service request
                    pdu[pduPointer.USHORT++] = 0x4D;    // Service code = write                    
                    OffsetValue.USHORT++;

                    OffsetValue.USHORT += j.GetAsciiTagname(s, ref pdu, ref pduPointer, tag, "LEN");

                    OffsetValue.USHORT += j.GetTagFormat(ref pdu, ref pduPointer, TagFormats.DINT);

                    // force to 1 nr of elements to be written
                    OffsetValue.USHORT += j.GetNumOfElements(ref pdu, ref pduPointer, 1);

                    OffsetValue.USHORT += j.GetTagWriteDataStringLength(ref pdu, ref pduPointer, startData, endData);
                }
            }            

            // number of jobs
            return (s.lastProcessedTags.USHORT);
        }

        public static ushort Logix5550TagsPrepareMultiWriteReqMicro800(
            ref List<CommJob> List, ref EtherNetIPStation s,
            ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort StartCh = pduPointer.USHORT;
            ushort WriteLenght;

            EtherNetIPCommJob j = List[0] as EtherNetIPCommJob;
            Tag tag;

            ushort CPFDataLenOffs = pduPointer.USHORT;
            // Sequence Count
            pdu[pduPointer.USHORT++] = s.GetTransaction().LOBYTE;
            pdu[pduPointer.USHORT++] = s.GetTransaction().HIBYTE;

            //lock (j.retLockList())
            {
                if (j.GetTagListOnWritingCount() == 0)
                    return 0;
                tag = j.GetTagListOnWriting()[0];
            }

            s.lastProcessedTags.USHORT = 1;

            // Set the service request
            pdu[pduPointer.USHORT++] = 0x4D;    // Service code = write

            j.GetAsciiTagname(s, ref pdu, ref pduPointer, tag);

            j.GetTagFormatMicro800(ref pdu, ref pduPointer);

            // Number of elements to be written
            j.GetNumOfElementsMicro800(ref pdu, ref pduPointer, tag);


            // Add data
            if (j.TagFormat != TagFormats.STRING)
            {
                WriteLenght = j.GetTagWriteData(ref pdu, ref pduPointer, tag);
            }
            else
            {
                ushort stringLen = pduPointer.USHORT;
                pduPointer.USHORT++;
                ushort endString = pduPointer.USHORT;
                WriteLenght = j.GetTagWriteData(ref pdu, ref pduPointer, tag);
                while (endString < pduPointer.USHORT && pdu[endString] != 0)
                    endString++;
                pdu[stringLen] = (byte)(endString - stringLen - 1);
                pduPointer.USHORT = endString;
            }

            // no data to write --> inputoutput/exception output with same value of driver
            if (WriteLenght == 0)
                return 0;

            //lock (j.retLockList())
            {
                // Special case: data cannot be contained in one single frame
                if (tag.TagNode.ArrayDimension > 1)
                {
                    if (j.PartialArrayEnd == 0 || (j.PartialArrayEnd == tag.TagNode.ArrayDimension))
                    {
                        j.TagsListToWrite.Remove(tag);
                    }

                    if (!j.TagsListOnWriting.Contains(tag))
                        j.TagsListOnWriting.Add(tag);

                }
                else
                {
                    j.TagsListToWrite.Remove(tag);
                    if (!j.TagsListOnWriting.Contains(tag))
                        j.TagsListOnWriting.Add(tag);
                }
            }
            j.CommandType = CommandTypes.WriteCmd;

            return (ushort)(pduPointer.USHORT - CPFDataLenOffs);
        }

        public static bool getReadListLimitate(ref EtherNetIPCommJob j, ref ReadWriteListLimitateSize frameSize, ReadWriteLimitate mode)
        {
            if (frameSize.IsEmpty())
            {
                frameSize.TotalSize = (ushort)CIP_MULTIPLE_READ_HEADER_SIZE;
                frameSize.ResponseSize = (ushort)(j.BuildInfoMaps ? LOGIX5550_HEADER_SIZE_OF_OPTIMIZED_RESPONSE : LOGIX5550_RESPONSE_SIZE_HEADER);
            }

            //foreach (EtherNetIPCommJob j in nextlist)
            //{
            ushort TagIndex = 0;
            j.ReadTagEnd = j.ReadTagStart;
            foreach (Tag tag in j.TagsList)
            {
                if (TagIndex >= j.ReadTagStart)
                {
                    frameSize.TotalSize += j.GetTagReadBufferSize(tag);
                    frameSize.TotalSize += 5;
                    if (frameSize.TotalSize > LOGIX5550_CONNECTION_SIZE)
                    {
                        if (j.ReadTagStart != j.ReadTagEnd)
                        {
                            //nextlistLimitate.Add(j);
                        }

                        return (frameSize.TotalNrJobs == 0); //return;
                    }
                    frameSize.ResponseSize += 8;
                    frameSize.ResponseSize += (ushort)(j.adjSize(tag) - j.PartialArrayStart * j.ElemSize);
                    if (frameSize.ResponseSize > LOGIX5550_CONNECTION_SIZE)
                    {
                        if (j.ReadTagEnd != j.ReadTagStart)
                        {
                            //nextlistLimitate.Add(j);
                            return (frameSize.TotalNrJobs == 0); //return;
                        }
                        if(tag.Size / j.ElemSize <= 1)
                            return false;
                        int tmp =  (j.adjSize(tag) + LOGIX5550_CONNECTION_SIZE + 1 - frameSize.ResponseSize - j.ElemSize ) / j.ElemSize;
                        tmp -= j.PartialArrayStart;
                        if (tmp > 0)
                        {
                            if (mode == ReadWriteLimitate.Add)
                                j.PartialArrayEnd = (ushort)(tmp + j.PartialArrayStart);
                            //nextlistLimitate.Add(j);
                        }
                        return (frameSize.TotalNrJobs == 0);
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

        private static ushort numTagsListToRead(List<CommJob> List)
        {
            ushort i = 0;
            foreach (EtherNetIPCommJob j in List)
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

        public static ushort Logix5550TagsPrepareMultiReadReq(
            ref List<CommJob> List, ref EtherNetIPStation s,
            ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort StartCh = pduPointer.USHORT;

            s.lastProcessedTags.USHORT = numTagsListToRead(List);

            ushortUnion OffsetIndex = new ushortUnion(pduPointer.USHORT);

            pduPointer.USHORT += (ushort)(s.lastProcessedTags.USHORT * 2);

            ushortUnion OffsetValue = new ushortUnion((ushort)(2 + pduPointer.USHORT - StartCh));

            foreach (EtherNetIPCommJob j in List)
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

                        //Strings are considered as structures
                        OffsetValue.USHORT += j.GetAsciiTagname(s, ref pdu, ref pduPointer, tag);

                        // Number of elements to be read
                        OffsetValue.USHORT += j.GetNumOfElements(ref  pdu, ref  pduPointer, tag);
                    }
                    tagIndex++;
                }
            }


            // number of Tags
            return (s.lastProcessedTags.USHORT);
        }

        public static ushort Logix5550TagsPrepareMultiReadReqMicro800(
            ref List<CommJob> List, ref EtherNetIPStation s,
            ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushort StartCh = pduPointer.USHORT;

            s.lastProcessedTags.USHORT = 1;

            EtherNetIPCommJob j = List[0] as EtherNetIPCommJob;
            var tag = j.TagsList[0];

            ushort CPFDataLenOffs = pduPointer.USHORT;

            // Sequence Count
            pdu[pduPointer.USHORT++] = s.GetTransaction().LOBYTE;
            pdu[pduPointer.USHORT++] = s.GetTransaction().HIBYTE;


            // Service code = read
            pdu[pduPointer.USHORT++] = 0x4C;

            j.GetAsciiTagname(s, ref pdu, ref pduPointer, tag);

            // Number of elements to be read
            j.GetNumOfElementsMicro800(ref pdu, ref pduPointer, tag);

            j.CommandType = CommandTypes.ReadCmd;

            return (ushort)(pduPointer.USHORT - CPFDataLenOffs);
        }        

        public static ushort PrepareEncapsulateRequest(ref List<PrepareHeader> prepareHeaderList, ref List<CommJob> ListJob, ref EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
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

                DataLength.USHORT = PrepareEncapsulateRequest(ref prepareHeaderList, ref ListJob, ref s, ref pdu, ref pduPointer);

                //Fill up data length
                pdu[BodyStart] = DataLength.LOBYTE;
                pdu[BodyStart + 1] = DataLength.HIBYTE;
            }
            else
            {
                switch (((EtherNetIPCommJob)ListJob[0]).CommandType)
                {
                    case CommandTypes.ReadCmd:
                        if (s.PlcType == PlcTypes.Micro800_series)
                            return Logix5550TagsPrepareMultiReadReqMicro800(ref ListJob, ref s, ref pdu, ref pduPointer);
                        else
                            return Logix5550TagsPrepareMultiReadReq(ref ListJob, ref s, ref pdu, ref pduPointer);
                    case CommandTypes.WriteCmd:
                        if (s.PlcType == PlcTypes.Micro800_series)
                            return Logix5550TagsPrepareMultiWriteReqMicro800(ref ListJob, ref s, ref pdu, ref pduPointer);
                        else
                            return Logix5550TagsPrepareMultiWriteReq(ref ListJob, ref s, ref pdu, ref pduPointer);
                    default:
                        return 0;
                }
            }

            if (DataLength.USHORT == 0 || s.lastProcessedTags.USHORT == 0)
            {
                return 0;
            }

            return ((ushort)(pduPointer.USHORT - StartCh));
        }
        
        public static bool ParseData(byte[] receivedbuffer, ref EtherNetIPCommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            uint receivedSize = 0;
            byte[] tempBuffer = null;
                        
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

            if (job.AddressType != AddressTypes.DataFile)
            {
                if (job.TotalJobSize > receivedbuffer.Length)
                {
                    receivedSize = (uint)receivedbuffer.Length;
                }
                else
                {
                    receivedSize = job.TotalJobSize;
                }

                tempBuffer = new byte[receivedSize];
                if (!job.isProtocolBool() || (job.ElementNumber == 0 && (uint)job.TagsList[0].TagNode.DataType.Identifier != (uint)BuiltInType.Boolean))
                    Array.Copy(receivedbuffer, tempBuffer, receivedSize);
                else
                {
                    if (((EtherNetIPStation)job.Station).PlcType == PlcTypes.Micro800_series)
                    {
                        for (ushort bitIndex = 0; bitIndex < receivedSize; bitIndex++)
                        {
                            tempBuffer[bitIndex] = receivedbuffer[bitIndex];
                        }
                    }
                    else
                    {
                        for (ushort bitIndex = 0; bitIndex < receivedSize; bitIndex++)
                        {
                            tempBuffer[bitIndex] = (byte)((receivedbuffer[bitIndex / 8] >> (bitIndex % 8)) & 1);
                        }
                    }
                }
            }
            else /* the case address type DataFile */
            {
                tempBuffer = new byte[job.TotalJobSize];

                if (!job.isProtocolBool() || (job.ElementNumber == 0 && (uint)job.TagsList[0].TagNode.DataType.Identifier != (uint)BuiltInType.Boolean))
                {
                    if (job.TotalJobSize > receivedbuffer.Length)
                    {
                        receivedSize = (uint)receivedbuffer.Length;
                    }
                    else
                    {
                        receivedSize = job.TotalJobSize;
                    }
                    Array.Copy(receivedbuffer, tempBuffer, receivedSize);
                }
                else
                {
                    byte index = 0;
                    foreach (Tag tag in job.TagsList)
                    {
                        if (tag.TagNode.ArrayDimension != 0)
                        {
                            if (job.DataFormat == DataFormats.BIT)
                            {
                                for (ushort bitIndex = 0; (bitIndex / 8) < receivedbuffer.Length; bitIndex++)
                                {
                                    index = (byte)(tag.BitOffset + job.Bit + bitIndex);
                                    if ((index + 1) <= (tempBuffer.Length))
                                    {
                                        tempBuffer[bitIndex] = (byte)((receivedbuffer[index / 8] >> (index % 8)) & 1);
                                    }
                                }
                            }
                            else
                            {
                                for (ushort byteIndex = 0; index < receivedbuffer.Length; index = (byte)(tag.ByteOffset + (byteIndex++)))
                                {
                                    tempBuffer[byteIndex] = receivedbuffer[index];
                                }
                            }
                        }
                        else
                        {
                            for (ushort bitIndex = 0; (bitIndex < job.TagsList.Count) && ((index / 8) <= receivedbuffer.Length); bitIndex++)
                            {
                                index = (byte)(job.TagsList[bitIndex].ByteOffset + job.Bit);
                                tempBuffer[job.TagsList[bitIndex].ByteOffset] = (byte)((receivedbuffer[index / 8] >> (index % 8)) & 1);
                            }
                        }
                    }                    
                }
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

        static byte GetFileTypeCode(FileTypes FileType)
        {
            switch (FileType)
            {
                case FileTypes.Output: return 0x8B;	// output logical by slot
                case FileTypes.Input: return 0x8C;	// input logical by slot
                case FileTypes.Status: return 0x84;	// status
                case FileTypes.Binary: return 0x85;	// bit
                case FileTypes.Timer: return 0x86;	// timer
                case FileTypes.Counter: return 0x87;	// counter
                case FileTypes.Control: return 0x88;	// control
                case FileTypes.Integer: return 0x89;	// integer
                case FileTypes.Float: return 0x8A;	// floating point
                case FileTypes.LongInteger: return 0x91;	// Long word point
                case FileTypes.String: return 0x8D;	// string point
                default: return 0x89;	// integer
            }
        }

        private static void writeUshort(ushort data, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            if (data > 254)
            {
                pdu[pduPointer.USHORT++] = 0xFF;
                ushortUnion Temp = new ushortUnion(data);
                pdu[pduPointer.USHORT++] = Temp.LOBYTE;
                pdu[pduPointer.USHORT++] = Temp.HIBYTE;
            }
            else
            {
                pdu[pduPointer.USHORT++] = (byte)data;
            }
        }

        public static string GetNodeTree(NodeId NodeId,string Name)
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

        public static string GetDynTagTree(EtherNetIPDynTagSettings dynSetting)
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
                case DataFormats.LWORD:
                    return UFUAModel.DataType.UInt64;
                default:
                    return 0;
            }
        }

        public static bool IsOldPlcModel(PlcTypes plc)
        {
            return (plc == PlcTypes.SLC500_MicroLogix || plc == PlcTypes.PLC5);
        }

        private static ushortUnion Logix5000V21PrepareAttributesTPDU(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushortUnion pduPointerStart = pduPointer;

            // Sequence Count
            pdu[pduPointer.USHORT++] = s.GetTransaction().LOBYTE;
            pdu[pduPointer.USHORT++] = s.GetTransaction().HIBYTE;

            //Service Code = "Multiple" service request (0x0A)
            //m_WriteBuf[m_nWCh++] = 0x0A;

            //// Request Path Size (words)
            //m_WriteBuf[m_nWCh++] = 0x02;

            //// 8 - Bit Logical Class Segment
            //m_WriteBuf[m_nWCh++] = 0x20;

            //// Class = Message Router
            //m_WriteBuf[m_nWCh++] = 0x02;

            //// 8 - Bit Logical Instance Segment 
            //m_WriteBuf[m_nWCh++] = 0x24;

            //// Instance = 1
            //m_WriteBuf[m_nWCh++] = 0x01;

            ////Number of requests = 1
            //m_WriteBuf[m_nWCh++] = 1;
            //m_WriteBuf[m_nWCh++] = 0;

            //// Request offset 
            //m_WriteBuf[m_nWCh++] = 4;
            //m_WriteBuf[m_nWCh++] = 0;


            // Service Code = Get Attribute List (0x03)
            pdu[pduPointer.USHORT++] = 0x03;

            // Request Path Size (words)
            pdu[pduPointer.USHORT++] = 0x03;

            // 8 - Bit Logical Class Segment
            pdu[pduPointer.USHORT++] = 0x20;

            // Class = 0xAC
            pdu[pduPointer.USHORT++] = 0xAC;

            // 8 - Bit Logical Instance Segment 
            pdu[pduPointer.USHORT++] = 0x25;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 1;
            pdu[pduPointer.USHORT++] = 0;

            // Attribute count
            pdu[pduPointer.USHORT++] = 0x05;
            pdu[pduPointer.USHORT++] = 0;

            // Attribute list
            pdu[pduPointer.USHORT++] = 0x01;
            pdu[pduPointer.USHORT++] = 0;

            // Attribute list
            pdu[pduPointer.USHORT++] = 0x02;
            pdu[pduPointer.USHORT++] = 0;

            // Attribute list
            pdu[pduPointer.USHORT++] = 0x03;
            pdu[pduPointer.USHORT++] = 0;

            // Attribute list
            pdu[pduPointer.USHORT++] = 0x04;
            pdu[pduPointer.USHORT++] = 0;

            // Attribute list
            pdu[pduPointer.USHORT++] = 0x0a;
            pdu[pduPointer.USHORT++] = 0;

            return (new ushortUnion((ushort)(pduPointer.USHORT - pduPointerStart.USHORT)));
        }


        private static ushortUnion Logix5000V21PrepareAttributesCPF(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            ushortUnion pduPointerStart = pduPointer;

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

            //Common Packet Format (CPF)
            //Data Item - Item Data Length = variable (filled up later)
            ushortUnion nCPFDataLenOffs = pduPointer;
            pdu[pduPointer.USHORT++] = 0;
            pdu[pduPointer.USHORT++] = 0;

            //Common Packet Format (CPF)
            //Transport PDU
            ushortUnion nPDULen = Logix5000V21PrepareAttributesTPDU(s, ref pdu, ref pduPointer);

            //Fill up data length
            pdu[nCPFDataLenOffs.USHORT] = nPDULen.LOBYTE;// (nPDULen);
            pdu[nCPFDataLenOffs.USHORT + 1] = nPDULen.HIBYTE; // (nPDULen);

            return (new ushortUnion((ushort)(pduPointer.USHORT - pduPointerStart.USHORT)));
        }


        public static ushort Logix5000V21PrepareAttributesRequest(EtherNetIPChannel c, EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer)
        {
            PrepareEncapsulationHeader(ecSendUnitData, c, ref pdu, ref pduPointer);

            // Complete the request message
            ushortUnion DataLen = Logix5000V21PrepareAttributesCPF(s, ref pdu, ref pduPointer);

            // Fill in packet data length
            //WORD wAux = nDataLen;
            pdu[EDATA_LEN_OFFS] = DataLen.LOBYTE;// LOBYTE(wAux);
            pdu[EDATA_LEN_OFFS + 1] = DataLen.HIBYTE; // (wAux);

            return pduPointer.USHORT;
        }

        public static bool Logix5000V21ReadAttributes(EtherNetIPStation s, ref byte[] pdu, ref ushortUnion pduPointer, EtherNetIPChannel c, out ushort attribute1, out ushort attribute2, out uint attribute3, out uint attribute4, out uint attribute10)
        {
            bool Result = false;

            attribute1 = 0;
            attribute2 = 0;
            attribute3 = 0;
            attribute4 = 0;
            attribute10 = 0;

            // Prepare the request message
            Logix5000V21PrepareAttributesRequest(c, s, ref pdu, ref pduPointer);

            c.ReceiveClear();

            if (!c.EtherNetIpDeviceWrite(ref pdu, ref pduPointer))
                return Result;
            
            //first we read the fixed-size header
            if (c.DeviceRead(pdu, EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE)) { //error timeout RX
                // Then read the variable size data part
                ushortUnion lenData = new ushortUnion(pdu, EtherNetIpProtocol.EDATA_LEN_OFFS);
                if (lenData.USHORT >= 20)
                {
                    byte[] pduData = new byte[lenData.USHORT];
                    if (c.DeviceRead(pduData, (uint)lenData.USHORT)) //error timeout RX
                    {
                        pduData.CopyTo(pdu, EtherNetIpProtocol.ENCAPSULATION_HEADER_SIZE);
                        if (c.AnalyzeReplyHeader(ref pdu, ecSendUnitData.USHORT, c.SessionHandle.UINT) == (EtherNetIpErrorCodes)DriverErrorCodes.ErrorNoError)
                        {
                            // Check the reply service code
                            ushortUnion DataLen = new ushortUnion(pdu, EDATA_LEN_OFFS);
                            if (DataLen.USHORT < 22)
                                return Result;

                            if (pdu[LOGIX5550_REP_TNS_OFFS + 2] != 0x83)
                                return Result;

                            if (pdu[LOGIX5550_REP_TNS_OFFS + 4] != 0)
                                return Result;

                            //    wAttribute1 = 0;                        
                            if ((new ushortUnion(pdu, LOGIX5550_REP_TNS_OFFS + 6)).USHORT != 5)
                                return Result;

                            //Status Attribute 1                       
                            if (pdu[LOGIX5550_REP_TNS_OFFS + 10] != 0)
                                return Result;

                            //Value attribute 1
                            attribute1 = (new ushortUnion(pdu, LOGIX5550_REP_TNS_OFFS + 12)).USHORT;

                            ////Status Attribute 2                        
                            if (pdu[LOGIX5550_REP_TNS_OFFS + 16] != 0)
                                return Result;

                            //Value attribute 2
                            attribute2 = (new ushortUnion(pdu, LOGIX5550_REP_TNS_OFFS + 18).USHORT);

                            //Status Attribute 3                        
                            if (pdu[LOGIX5550_REP_TNS_OFFS + 22] != 0)
                                return Result;

                            //Value attribute 3
                            attribute3 = (new uintUnion(pdu, LOGIX5550_REP_TNS_OFFS + 24).UINT);

                            //Status Attribute 4                        
                            if (pdu[LOGIX5550_REP_TNS_OFFS + 30] != 0)
                                return Result;

                            //Value attribute 4
                            attribute4 = (new uintUnion(pdu, LOGIX5550_REP_TNS_OFFS + 32).UINT);

                            //Status Attribute 10
                            if (pdu[LOGIX5550_REP_TNS_OFFS + 38] != 0)
                                return Result;

                            //Value attribute 10
                            attribute10 = (new uintUnion(pdu, LOGIX5550_REP_TNS_OFFS + 40).UINT);

                            s.LastTransOK = true;

                            Result = true;
                        }
                    }
                }
            }
                        
            return Result;
        }

        #endregion

    }

}
