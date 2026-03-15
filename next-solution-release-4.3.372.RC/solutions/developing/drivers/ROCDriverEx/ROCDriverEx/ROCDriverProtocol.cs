using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Enumerators;
using System.Runtime.InteropServices;
using Opc.Ua;
using System.Drawing;
using DevExpress.Office.Utils;
using DevExpress.XtraPrinting.Native;

namespace ROCDriver
{
    #region enums

    public enum DataTypes
    {
        BIN,
        AC,
        INT8,
        INT16,
        INT32,
        UINT8,
        UINT16,
        UINT32,
        FLOAT,
        DBL,
        TLP,
        TIME
    }

    public enum FrameFormats : int
    {
        Normal,
        Long,
    }

    public enum ChannelTypes : int
    {
        Serial,
        Socket,
    }

    public enum MemoryAreas : sbyte
    {
        Invalid = -1,
        X = 1, 
        Y,
        R,
        L,
        DT,
        FL,
        LD,
        SV,
        EV,
    }
    
    public enum DataFormats : int
    {
        Invalid = -1,
        BOOL, 
        WORD,
        DWORD, 
        //INT, 
        //DINT, 
        //REAL, 

    }

    public enum ROCDriverOpcodes
    {
        RequestParameters = 180,
        WriteParameters = 181,
        ErrorIndicator = 255
    }

    public enum ROCDriverOffsets : byte
    {
        DestinationUnit,
        DestinationGroup,
        SourceUnit,
        SourceGroup,
        Opcode,
        DataLength,
        NumberOfParameters
    }

    public enum ROCDriverErrorCodes : int
    {
        ErrorIncompleteReply = 1000,
        ErrorInvalidHeader,
        ErrorUnexpectedOpcode,
        ErrorMissingData,
        ErrorCrc,
        ErrorMalformattedErrorMessage,
        ErrorUnexpectedNumberOfParameters,
        ErrorParseFailure,
        ErrorTLPMismatch,
        ErrorParsingParameterData,
        ErrorMalformattedReply,
        ErrorInConnectionCheck,
        ErrorReadReplyLength,
        ErrorErrorCode = 1200,
        ErrorErrorCode1,
        ErrorErrorCode2,
        ErrorErrorCode3,
        ErrorErrorCode4,
        ErrorErrorCode5,
        ErrorErrorCode6,
        ErrorErrorCode12 = 1212,
        ErrorErrorCode13,
        ErrorErrorCode14,
        ErrorErrorCode15,
        ErrorErrorCode16,
        ErrorErrorCode17,
        ErrorErrorCode18,
        ErrorErrorCode19,
        ErrorErrorCode20,
        ErrorErrorCode21,
        ErrorErrorCode22,
        ErrorErrorCode24 = 1224,
        ErrorErrorCode25,
        ErrorErrorCode29 = 1229,
        ErrorErrorCode30,
        ErrorErrorCode31,
        ErrorErrorCode32,
        ErrorErrorCode33,
        ErrorErrorCode34,
        ErrorErrorCode50 = 1250,
        ErrorErrorCode51,
        ErrorErrorCode52,
        ErrorErrorCode61 = 1261,
        ErrorErrorCode62,
        ErrorErrorCode63,
        ErrorErrorCode77 = 1277,
    }

    public enum ReplyAttributes : int
    {
        Response = 1,
        ACK_NAK,
    }
   
    #endregion

 
    #region structDataTypes

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
            this.HIBYTE = buffer[index];
            this.LOBYTE = buffer[index + 1];
        }
        public ushortUnion(List<byte> buffer, ushort index)
        {
            this.USHORT = 0;
            this.HIBYTE = buffer[index];
            this.LOBYTE = buffer[index + 1];
        }

    }

    [StructLayout(LayoutKind.Explicit)]
    public struct shortUnion
    {
        [FieldOffset(0)]
        public short SHORT;

        [FieldOffset(0)]
        public byte LOBYTE;
        [FieldOffset(1)]
        public byte HIBYTE;
        // Constructor:
        public shortUnion(short SHORT)
        {
            this.LOBYTE = 0;
            this.HIBYTE = 0;
            this.SHORT = SHORT;
        }
        public shortUnion(byte LOBYTE, byte HIBYTE)
        {
            this.SHORT = 0;
            this.LOBYTE = LOBYTE;
            this.HIBYTE = HIBYTE;
        }
        public shortUnion(byte[] buffer, ushort index)
        {
            this.SHORT = 0;
            this.HIBYTE = buffer[index];
            this.LOBYTE = buffer[index + 1];
        }
        public shortUnion(List<byte> buffer, ushort index)
        {
            this.SHORT = 0;
            this.HIBYTE = buffer[index];
            this.LOBYTE = buffer[index + 1];
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
            this.HIUSHORT = new ushortUnion(buffer, index);
            this.LOUSHORT = new ushortUnion(buffer, (ushort)(index + 2));
        }
        public uintUnion(List<byte> buffer, ushort index)
        {
            this.UINT = 0;
            this.HIUSHORT = new ushortUnion(buffer, index);
            this.LOUSHORT = new ushortUnion(buffer, (ushort)(index + 2));
        }

    }

    [StructLayout(LayoutKind.Explicit)]
    public struct intUnion
    {
        [FieldOffset(0)]
        public int INT;

        [FieldOffset(0)]
        public shortUnion LOSHORT;
        [FieldOffset(2)]
        public shortUnion HISHORT;

        // Constructor:
        public intUnion(int INT)
        {
            this.LOSHORT = new shortUnion(0x0000);
            this.HISHORT = new shortUnion(0x0000);
            this.INT = INT;
        }
        public intUnion(byte LOBYTE_LW, byte HIBYTE_LW, byte LOBYTE_HW, byte HIBYTE_HW)
        {
            this.INT = 0;
            this.LOSHORT = new shortUnion(LOBYTE_LW, HIBYTE_LW);
            this.HISHORT = new shortUnion(LOBYTE_HW, HIBYTE_HW);
        }
        public intUnion(byte[] buffer, ushort index)
        {
            this.INT = 0;
            this.HISHORT = new shortUnion(buffer, index);
            this.LOSHORT = new shortUnion(buffer, (ushort)(index + 2));
        }
        public intUnion(List<byte> buffer, ushort index)
        {
            this.INT = 0;
            this.HISHORT = new shortUnion(buffer, index);
            this.LOSHORT = new shortUnion(buffer, (ushort)(index + 2));
        }

    }

    [StructLayout(LayoutKind.Explicit)]
    public struct floatUnion
    {
        [FieldOffset(0)]
        public float FLOAT;
        [FieldOffset(0)]
        public uintUnion UINTUNION;

        // Constructor:
        public floatUnion(float FLOAT)
        {
            this.UINTUNION = new uintUnion(0x0000);
            this.FLOAT = FLOAT;
        }
        public floatUnion(uint UINT)
        {
            this.FLOAT = 0;
            this.UINTUNION = new uintUnion(UINT);
        }
        public floatUnion(byte[] buffer, ushort index)
        {
            this.FLOAT = 0;
            this.UINTUNION = new uintUnion(buffer, index);
        }
    }

    #endregion

    public class ROCDriverProtocol
    {
        #region static const

        public const byte ReadRequestOverhead = 9;
        public const byte ReadReplyOverhead = 9;
        public const byte WriteRequestOverhead = 9;
        public const byte WriteReplyOverhead = 8;
        public const byte TLPAddressLength = 3;
        public const byte HeaderLength = 6;
        public const byte DataLengthOffset = 5;
        public const byte CRCLength = 2;
        public const ushort MAX_FRAME_LENGTH = 240;
        public const uint StringDefaultLength = 10;
        public const string TEST_COMM_DYNAMIC_SETTINGS = "ROCDriver.Station={0}|LinkType=1|PT=0|LN=0|PRM=0|DT=5";

        #endregion

        #region methods

        #region Frame dimensions
        public static uint GetMaxJobSize()
        {
            uint ReturnValue = MAX_FRAME_LENGTH - ReadRequestOverhead - TLPAddressLength;
            return (ReturnValue);
        }

        public static bool GetJobWriteRequestResponseSize(ref ROCDriverCommJob j, ref uint requestFrameSize, ref uint responseFrameSize)
        {
            // Inizialization of output parameters 
            requestFrameSize = 0;
            responseFrameSize = 0;

            if(j.TagsList.Count < 1)
            {
                return false;
            }

            // The response frame is calculated for the longer case: error message, with two bytes of error code
            responseFrameSize += 2;

            // 3 bytes for TLP data in the request frame 
            requestFrameSize += TLPAddressLength;

            // Add the size of the data to the length of the write request
            switch (j.DataType)
            {
                case DataTypes.BIN:
                case DataTypes.INT8:
                case DataTypes.UINT8:
                    requestFrameSize += 1;
                    break;
                case DataTypes.AC:
                    if (j.StringLength > 0)
                    {
                        requestFrameSize += j.StringLength;
                    }
                    else
                    {
                        requestFrameSize += 1;
                    }
                    break;
                case DataTypes.INT16:
                case DataTypes.UINT16:
                    requestFrameSize += 2;
                    break;
                case DataTypes.INT32:
                case DataTypes.UINT32:
                case DataTypes.FLOAT:
                case DataTypes.TIME:
                    requestFrameSize += 4;
                    break;
                case DataTypes.DBL:
                    requestFrameSize += 8;
                    break;
                case DataTypes.TLP:
                    requestFrameSize += 3;
                    break;
            }

            return true;
        }

        public static bool GetJobReadRequestResponseSize(ref ROCDriverCommJob j, ref uint requestFrameSize, ref uint responseFrameSize)
        {
            // Inizialization of output parameters 
            requestFrameSize = 0;
            responseFrameSize = 0;

            if (j.TagsList.Count < 1)
            {
                return false;
            }

            // 3 bytes for TLP data in the request frame and in the response one 
            requestFrameSize += TLPAddressLength;
            responseFrameSize += TLPAddressLength;

            // Add the size of the data to the length of the reponse frame
            switch(j.DataType)
            {
                case DataTypes.BIN:
                case DataTypes.INT8:
                case DataTypes.UINT8:
                    responseFrameSize += 1;
                    break;
                case DataTypes.AC:
                    if(j.StringLength > 0)
                    {
                        responseFrameSize += j.StringLength;
                    }
                    else
                    {
                        responseFrameSize += 1;
                    }
                    break;
                case DataTypes.INT16:
                case DataTypes.UINT16:
                    responseFrameSize += 2;
                    break;
                case DataTypes.INT32:
                case DataTypes.UINT32:
                case DataTypes.FLOAT:
                case DataTypes.TIME:
                    responseFrameSize += 4;
                    break;
                case DataTypes.DBL:
                    responseFrameSize += 8;
                    break;
                case DataTypes.TLP:
                    responseFrameSize += 3;
                    break;
            }

            return true;
        }

        private static uint GetReadRequestTotalSize(List<CommJob> list)
        {
            if(list.Count < 1)
            {
                return 0;
            }

            uint totalSize = ReadRequestOverhead;
            totalSize += (uint)(3*list.Count); // 3 bytes of the TLP address for each job

            return(totalSize);
        }

        private static uint GetWriteRequestTotalSize(List<CommJob> list)
        {
            if (list.Count < 1)
            {
                return 0;
            }

            uint totalSize = WriteRequestOverhead;
            totalSize += (uint)(TLPAddressLength * list.Count); // 3 bytes of the TLP address for each job

            // Data bytes
            foreach (ROCDriverCommJob j in list)
            {
                uint jobWriteDataSize = j.GetWriteDataLength();
                if (jobWriteDataSize < 1)
                {
                    return (0);
                }
                totalSize += jobWriteDataSize;
            }

            return (totalSize);
        }

        public static uint GetReadResponseTotalSize(List<CommJob> list)
        {
            if (list.Count < 1)
            {
                return 0;
            }

            uint totalSize = ReadReplyOverhead;

            foreach(CommJob j in list)
            {
                ROCDriverCommJob rocJob = j as ROCDriverCommJob;
                uint jobRequestFrameSize = 0;
                uint jobResponseFrameSize = 0;
                if(!GetJobReadRequestResponseSize(ref rocJob, ref jobRequestFrameSize, ref jobResponseFrameSize))
                {
                    return(0);
                }
                totalSize += jobResponseFrameSize;
            }

            return (totalSize);
        }

        #endregion

        #region Prepare Frame

        private static uint PrepareHeader(List<CommJob> list, ROCDriverChannel channel, ref byte[] pdu, ref byte opcode)
        {
            if(list.Count < 1)
            {
                return (0);
            }
            if (pdu.Length < HeaderLength)
            {
                return (0);
            }
            ROCDriverStation station = (ROCDriverStation)list[0].Station;
            if(station == null)
            {
                return (0);
            }

            int bufferIndex = 0;
            // Destination Unit
            pdu[bufferIndex++] = station.StationID;
            // Destination Group
            pdu[bufferIndex++] = station.StationGroup;
            // Source Unit
            pdu[bufferIndex++] = channel.HostAddress;
            // Source Group
            pdu[bufferIndex++] = channel.HostGroup;
            // Opcode
            ROCDriverCommJob j = (ROCDriverCommJob)list[0];
            if (j.IsWriteRequest() && !j.IsReadRWReady())
            {
                opcode = (byte)ROCDriverOpcodes.WriteParameters;
            }
            else
            {
                opcode = (byte)ROCDriverOpcodes.RequestParameters;

            }
            pdu[bufferIndex++] = opcode;
            // Data Length (to be set later)
            pdu[bufferIndex++] = 0;

            return ((uint)bufferIndex);
        }

        private static uint PrepareReadRequest(List<CommJob> list, ref byte[] pdu)
        {
            // Check the buffer length
            uint requestTotalSize = GetReadRequestTotalSize(list);
            if ((requestTotalSize == 0) || (pdu.Length < requestTotalSize))
            {
                return (0);
            }

            uint requestIndexInitialOffset = HeaderLength;
            uint bufferIndex = 0;

            // Number of parameters to be read
            pdu[requestIndexInitialOffset + bufferIndex++] = (byte)list.Count;

            // TLPs (addresses) of the parameters
            foreach(ROCDriverCommJob j in list)
            {
                pdu[requestIndexInitialOffset + bufferIndex++] = j.PointType;
                pdu[requestIndexInitialOffset + bufferIndex++] = j.LogicalNumber;
                pdu[requestIndexInitialOffset + bufferIndex++] = j.Parameter;
            }

            return (bufferIndex);
        }

        private static uint PrepareWriteRequest(List<CommJob> list, ref byte[] pdu)
        {
            // Check the buffer length
            uint requestTotalSize = GetWriteRequestTotalSize(list);
            if ((requestTotalSize == 0) || (pdu.Length < requestTotalSize))
            {
                return (0);
            }

            uint requestIndexInitialOffset = HeaderLength;
            uint bufferIndex = 0;

            // Number of parameters to be written
            pdu[requestIndexInitialOffset + bufferIndex++] = (byte)list.Count;

            foreach (ROCDriverCommJob j in list)
            {
                // TLP (address) of the parameter
                pdu[requestIndexInitialOffset + bufferIndex++] = j.PointType;
                pdu[requestIndexInitialOffset + bufferIndex++] = j.LogicalNumber;
                pdu[requestIndexInitialOffset + bufferIndex++] = j.Parameter;
                // Data to be written
                byte[] dataBuffer = null;
                if(!j.GetWriteData(ref dataBuffer))
                {
                    return (0);
                }
                if(dataBuffer.Length < 1)
                {
                    return (0);
                }
                for(int i=0; i<dataBuffer.Length; i++)
                {
                    pdu[requestIndexInitialOffset + bufferIndex++] = dataBuffer[i];
                }
            }

            return (bufferIndex);
        }

        public static uint PrepareFrame(List<CommJob> list, ROCDriverChannel channel, ref byte[] pdu)
        {
            if(list.Count == 0)
            {
                return 0;
            }

            // Prepare the header of the request:
            //    destination unit (1 byte)
            //    destination group (1 byte)
            //    source unit (1 byte)
            //    source group (1 byte)
            //    opcode (1 byte): 180 = Read Multiple Parameters, 181 = Write Multiple Parameters
            //    data length (1 byte): initialized to 0
            byte opcode = 0;
            if(PrepareHeader(list, channel, ref pdu, ref opcode) == 0)
            {
                return 0;
            }
            uint dataLength = 0;
            if(opcode == (byte)ROCDriverOpcodes.RequestParameters)
            {
                dataLength = PrepareReadRequest(list, ref pdu);
            }
            else
            {
                dataLength = PrepareWriteRequest(list, ref pdu);
            }
            if(dataLength == 0)
            {
                return (0);
            }
            else
            {
                // Set the data length
                pdu[DataLengthOffset] = (byte)dataLength;
                // CRC
                ushortUnion nCRC = CalculateCRC(pdu, dataLength + HeaderLength);
                pdu[dataLength + HeaderLength] = nCRC.LOBYTE;
                pdu[dataLength + HeaderLength + 1] = nCRC.HIBYTE;
                if(opcode == (byte)ROCDriverOpcodes.RequestParameters)
                {
                    return (dataLength + ReadRequestOverhead - 1);
                }
                else
                {
                    return (dataLength + WriteRequestOverhead - 1);
                }
            }
        }

        public static uint PrepareTestRequest(ROCDriverStation station, ROCDriverChannel channel, ref byte[] pdu)
        {
            if (pdu.Length < (HeaderLength + TLPAddressLength + 1 + CRCLength))
            {
                return (0);
            }

            // Prepare the header of the request:
            //    destination unit (1 byte)
            //    destination group (1 byte)
            //    source unit (1 byte)
            //    source group (1 byte)
            //    opcode (1 byte): 180 = Read Multiple Parameters
            //    data length (1 byte): initialized to 0
            int bufferIndex = 0;
            // Destination Unit
            pdu[bufferIndex++] = station.StationID;
            // Destination Group
            pdu[bufferIndex++] = station.StationGroup;
            // Source Unit
            pdu[bufferIndex++] = channel.HostAddress;
            // Source Group
            pdu[bufferIndex++] = channel.HostGroup;
            // Opcode
            pdu[bufferIndex++] = (byte)ROCDriverOpcodes.RequestParameters;
            // Data Length
            pdu[bufferIndex++] = TLPAddressLength + 1;

            // Data part of the request
            // Number of parameters to be read
            pdu[bufferIndex++] = 1;
            // TLPs (address) of the parameter
            pdu[bufferIndex++] = station.PingPointType;
            pdu[bufferIndex++] = station.PingLogicalNumber;
            pdu[bufferIndex++] = station.PingParameter;
            // CRC
            ushortUnion nCRC = CalculateCRC(pdu, (uint)bufferIndex);
            pdu[bufferIndex++] = nCRC.LOBYTE;
            pdu[bufferIndex++] = nCRC.HIBYTE;

            return ((uint)bufferIndex);
        }

        public static BuiltInType GetProtocolBuiltInType(DataTypes dataType)
        {
            BuiltInType builtInType = BuiltInType.Null;
            switch(dataType)
            {
                case DataTypes.BIN:
                case DataTypes.UINT8:
                    builtInType = BuiltInType.Byte;
                    break;
                case DataTypes.INT16:
                    builtInType = BuiltInType.Int16;
                    break;
                case DataTypes.UINT16:
                    builtInType = BuiltInType.UInt16;
                    break;
                case DataTypes.INT32:
                    builtInType = BuiltInType.Int32;
                    break;
                case DataTypes.UINT32:
                case DataTypes.TLP:
                    builtInType = BuiltInType.UInt32;
                    break;
                case DataTypes.FLOAT:
                    builtInType = BuiltInType.Float;
                    break;
                case DataTypes.DBL:
                    builtInType = BuiltInType.Double;
                    break;
                case DataTypes.AC:
                case DataTypes.TIME:
                    builtInType = BuiltInType.String;
                    break;
            }

            return(builtInType);
        }

        // The method returns the number of data bytes for a parameter, according to its data format
        public static uint GetJobDataSize(ROCDriverCommJob j)
        {
            uint byteSize = 0;

            switch (j.DataType)
            {
                case DataTypes.BIN:
                case DataTypes.INT8:
                case DataTypes.UINT8:
                    byteSize = 1;
                    break;
                case DataTypes.AC:
                    if(j.StringLength > 0)
                    {
                        byteSize = j.StringLength;
                    }
                    else
                    {
                        byteSize = 1;
                    }
                    break;
                case DataTypes.INT16:
                case DataTypes.UINT16:
                    byteSize = 2;
                    break;
                case DataTypes.INT32:
                case DataTypes.UINT32:
                case DataTypes.FLOAT:
                case DataTypes.TIME:
                    byteSize = 4;
                    break;
                case DataTypes.TLP:
                    byteSize = 3;
                    break;
                case DataTypes.DBL:
                    byteSize = 8;
                    break;
            }

            return(byteSize);
        }
        #endregion

        #endregion

        #region CRC calculation
        static UInt16[] CRC16Table = {0, 49345, 49537, 320, 49921, 960, 640, 49729, 50689, 1728,
                                         1920, 51009, 1280, 50625, 50305, 1088, 52225, 3264, 3456,
                                         52545, 3840, 53185, 52865, 3648, 2560, 51905, 52097, 2880,
                                         51457, 2496, 2176, 51265, 55297, 6336, 6528, 55617, 6912,
                                         56257, 55937, 6720, 7680, 57025, 57217, 8000, 56577, 7616,
                                         7296, 56385, 5120, 54465, 54657, 5440, 55041, 6080, 5760,
                                         54849, 53761, 4800, 4992, 54081, 4352, 53697, 53377, 4160,
                                         61441, 12480, 12672, 61761, 13056, 62401, 62081, 12864,
                                         13824, 63169, 63361, 14144, 62721, 13760, 13440, 62529,
                                         15360, 64705, 64897, 15680, 65281, 16320, 16000, 65089,
                                         64001, 15040, 15232, 64321, 14592, 63937, 63617, 14400,
                                         10240, 59585, 59777, 10560, 60161, 11200, 10880, 59969,
                                         60929, 11968, 12160, 61249, 11520, 60865, 60545, 11328,
                                         58369, 9408, 9600, 58689, 9984, 59329, 59009, 9792, 8704,
                                         58049, 58241, 9024, 57601, 8640, 8320, 57409, 40961, 24768,
                                         24960, 41281, 25344, 41921, 41601, 25152, 26112, 42689,
                                         42881, 26432, 42241, 26048, 25728, 42049, 27648, 44225,
                                         44417, 27968, 44801, 28608, 28288, 44609, 43521, 27328,
                                         27520, 43841, 26880, 43457, 43137, 26688, 30720, 47297,
                                         47489, 31040, 47873, 31680, 31360, 47681, 48641, 32448,
                                         32640, 48961, 32000, 48577, 48257, 31808, 46081, 29888,
                                         30080, 46401, 30464, 47041, 46721, 30272, 29184, 45761,
                                         45953, 29504, 45313, 29120, 28800, 45121, 20480, 37057,
                                         37249, 20800, 37633, 21440, 21120, 37441, 38401, 22208,
                                         22400, 38721, 21760, 38337, 38017, 21568, 39937, 23744,
                                         23936, 40257, 24320, 40897, 40577, 24128, 23040, 39617,
                                         39809, 23360, 39169, 22976, 22656, 38977, 34817, 18624,
                                         18816, 35137, 19200, 35777, 35457, 19008, 19968, 36545,
                                         36737, 20288, 36097, 19904, 19584, 35905, 17408, 33985,
                                         34177, 17728, 34561, 18368, 18048, 34369, 33281, 17088,
                                         17280, 33601, 16640, 33217, 32897, 16448};

        public static bool CheckCRC(byte[] buffer)
        {
            int pduLength = buffer.Length;
            if( pduLength <= 0 )
            {
                return false;
            }
            return (CheckCRC(buffer, (uint)pduLength));
        }

        public static bool CheckCRC(byte[] buffer, uint pduLength)
        {
            ushortUnion nCRC = CalculateCRC(buffer, pduLength - 2);
            ushortUnion readCRC = new ushortUnion(buffer, (ushort)(pduLength - 2));
            return ((readCRC.HIBYTE == nCRC.LOBYTE) && (readCRC.LOBYTE == nCRC.HIBYTE));
        }

        public static ushortUnion CalculateCRC(byte[] buffer, uint Count)
        {
            UInt16 ival, temp;
            //ival = 0xFFFF;
            ival = 0;
            int i = 0;
            while ((Count--) > 0)
            {
                byte b1 = (byte)(ival >> 8);
                temp = (UInt16)(b1 & 0xFF);
                ival = (UInt16)(temp ^ CRC16Table[((byte)ival ^ buffer[i++]) & 0xFF]);
            }

            ushortUnion nCRC = new ushortUnion(ival);
            return nCRC;
        }

        #endregion

        #region Checks

        public static byte[] ErrorCodes =
        {
            1, 2, 3, 4, 5, 6,
            12, 13, 14, 15, 16, 17, 18, 19,
            20, 21, 22, 23, 24, 25, 29,
            30, 31, 32, 33, 34,
            50, 51, 52,
            61, 62, 63,
            77
        };

        public static int CheckMessageHeader(byte[] pduHeader, ROCDriverChannel channel, ROCDriverStation station, byte expectedOpcode, ref byte remainingMessageLength)
        {
            remainingMessageLength = 0;
            // Check the Destination Unit, Destination Group, Source Unit, Source Group 
            if ((pduHeader[(int)ROCDriverOffsets.DestinationUnit] != channel.HostAddress) || (pduHeader[(int)ROCDriverOffsets.DestinationGroup] != channel.HostGroup) ||
                (pduHeader[(int)ROCDriverOffsets.SourceUnit] != station.StationID) || (pduHeader[(int)ROCDriverOffsets.SourceGroup] != station.StationGroup))
            {
                return ((int)ROCDriverErrorCodes.ErrorInvalidHeader);
            }

            // Check the Opcode
            if ((pduHeader[4] != expectedOpcode) && (pduHeader[(int)ROCDriverOffsets.Opcode] != (byte)ROCDriverOpcodes.ErrorIndicator))
            {
                return ((int)ROCDriverErrorCodes.ErrorUnexpectedOpcode);
            }

            // Get the byte size of the following data part of the message
            remainingMessageLength = pduHeader[(int)ROCDriverOffsets.DataLength];

            return ((int)DriverErrorCodes.ErrorNoError);
        }

        public static int CheckReplyToPingMessage(byte[] pdu, ROCDriverChannel channel, ROCDriverStation station)
        {
            // Check the Destination Unit, Destination Group, Source Unit, Source Group 
            if ((pdu[(int)ROCDriverOffsets.DestinationUnit] != channel.HostAddress) || (pdu[(int)ROCDriverOffsets.DestinationGroup] != channel.HostGroup) ||
                (pdu[(int)ROCDriverOffsets.SourceUnit] != station.StationID) || (pdu[(int)ROCDriverOffsets.SourceGroup] != station.StationGroup))
            {
                return ((int)ROCDriverErrorCodes.ErrorMalformattedReply);
            }

            // Check the Opcode
            if ((pdu[4] != (byte)ROCDriverOpcodes.RequestParameters) && (pdu[(int)ROCDriverOffsets.Opcode] != (byte)ROCDriverOpcodes.ErrorIndicator))
            {
                return ((int)ROCDriverErrorCodes.ErrorMalformattedReply);
            }

            // Check the CRC
            if (!ROCDriverProtocol.CheckCRC(pdu))
            {
                return ((int)ROCDriverErrorCodes.ErrorMalformattedReply);
            }

            return ((int)DriverErrorCodes.ErrorNoError);
        }

        public static bool IsErrorMessage(byte[] pdu)
        {
            // Error codes are sent in a message with Opcode 255
            if (pdu[(int)ROCDriverOffsets.Opcode] != (byte)ROCDriverOpcodes.ErrorIndicator)
            {
                return (false);
            }

            return (true);
        }

        public static int GetErrorCode(byte errorCode)
        {
            if(ErrorCodes.Contains(errorCode))
            {
                return((int)(ROCDriverErrorCodes.ErrorErrorCode + errorCode));
            }
            else
            {
                return ((int)ROCDriverErrorCodes.ErrorErrorCode);
            }
        }

        #endregion

        #region methods override
        private static byte[] ConvertTLPtoUInt32(byte[] tlpBuffer)
        {
            byte[] uintBuffer = new byte[4];
            if(tlpBuffer.Length >= 3)
            {
                uintUnion uintValue = new uintUnion();
                uintValue.LOUSHORT.LOBYTE = tlpBuffer[0];
                uintValue.LOUSHORT.HIBYTE = tlpBuffer[1];
                uintValue.HIUSHORT.LOBYTE = tlpBuffer[2];
                uint value = uintValue.UINT;
                uintBuffer = BitConverter.GetBytes(value);
            }

            return (uintBuffer);
        }

        private static byte[] ConvertTIMEtoString(byte[] timeBuffer)
        {
            string timeString = String.Empty;
            UInt32 epochTime = 0;
            DateTime messageTimestamp = DateTime.MinValue;
            byte[] timeStringBuffer = new byte[0];
            try
            {
                epochTime = BitConverter.ToUInt32(timeBuffer, 0);
                messageTimestamp = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddSeconds(epochTime);
                timeString = messageTimestamp.ToString("yyyy-MM-ddTHH:mm:ss");
                timeStringBuffer = Encoding.ASCII.GetBytes(timeString);
            }
            catch (Exception e)
            {
                timeStringBuffer = new byte[0];
            }

            return (timeStringBuffer);
        }

        public static int ParseData(byte[] receiveBuffer, ref ROCDriverCommJob job, ref List<Object> items)
        {
            bool areArguments = (items.Count > 0);
            // Check that the data buffer is not null
            if (!areArguments && (receiveBuffer == null))
            {
                return ((int)DriverErrorCodes.ErrorParsingAnswer);
            }
            else if (receiveBuffer == null)
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
                    items[0] = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorParsingAnswer;
                    return ((int)DriverErrorCodes.ErrorParsingAnswer);
                }

                items[0] = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                return ((int)DriverErrorCodes.ErrorNoError);
            }

            // Check the length of the received buffer
            if (receiveBuffer.Length < (ROCDriverProtocol.TLPAddressLength + 1))
            {
                if (areArguments)
                {
                    items[0] = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorParsingAnswer;
                }
                return ((int)DriverErrorCodes.ErrorParsingAnswer);
            }

            // Check the TLP address of the data point: it must match the one assigned to the job
            if ((receiveBuffer[0] != job.PointType) || (receiveBuffer[1] != job.LogicalNumber) || (receiveBuffer[2] != job.Parameter))
            {
                if (areArguments)
                {
                    items[0] = ROCDriverErrorCodes.ErrorTLPMismatch;
                }
                return ((int)ROCDriverErrorCodes.ErrorTLPMismatch);
            }

            if (areArguments)
            {
                items[0] = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
            }

            // Set the number of data bytes that can be copied from the received reply 
            uint receivedSize = 0;
            if (job.TotalJobSize > (receiveBuffer.Length - TLPAddressLength))
            {
                receivedSize = (uint)(receiveBuffer.Length - TLPAddressLength);
            }
            else
            {
                receivedSize = job.TotalJobSize;
            }

            // Copy the received data
            byte[] tempBuffer = new byte[receivedSize];
            List<Tag> changed = new List<Tag>();
            Array.Copy(receiveBuffer, TLPAddressLength, tempBuffer, 0, receivedSize);

            // Special case: for TIME format a conversion to string is required
            if(job.DataType == DataTypes.TIME)
            {
                byte[] timeBuffer = new byte[tempBuffer.Length];
                tempBuffer.CopyTo(timeBuffer, 0);
                tempBuffer = ConvertTIMEtoString(timeBuffer);
            }
            // Special case: for TLP format a conversion to UInt32 is required
            else if (job.DataType == DataTypes.TLP)
            {
                byte[] tlpBuffer = new byte[tempBuffer.Length];
                tempBuffer.CopyTo(tlpBuffer, 0);
                tempBuffer = ConvertTLPtoUInt32(tlpBuffer);
            }

            // Set the value of the read variable and check if it is changed
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

            return ((int)DriverErrorCodes.ErrorNoError);
        }
        #endregion


    }
}
