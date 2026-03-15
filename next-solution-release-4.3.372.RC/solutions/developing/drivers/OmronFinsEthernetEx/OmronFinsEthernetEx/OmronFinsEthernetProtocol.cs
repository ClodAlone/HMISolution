using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using System.Runtime.InteropServices;
using DriverCodeBaseEx.Enumerators;


namespace OmronFinsEthernet
{
    #region enums

    public enum DataAreas : sbyte
    {
        Invalid = -1,
        IR = 0,
        LR,
        HR,
        AR,
        DM,
        T,
        C,
        EM,
        W

    }

    public enum DataFormats : int
    {
        Invalid = -1,
        Bit,
        Word,
        String
    };

    public enum DataConversionTypes : int
    {
        None = 0,
        BCD16Bits,
        BCD32Bits
    };

    public enum CommandCodes : byte
    {
        Read,
        Write,
    };

    public enum OmronFinsEthernetErrorCodes : int
    {
        ErrorDriverError = 1000,
        ErrorSid,
        ErrorEndCode,
        ErrorIncompleteFrame,
        ErrorAnswer,

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
            this.LOBYTE = buffer[index ];
            this.HIBYTE = buffer[index + 1];
        }
        public ushortUnion(List<byte> buffer, ushort index)
        {
            this.USHORT = 0;
            this.LOBYTE = buffer[index ];
            this.HIBYTE = buffer[index + 1];
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
            this.LOBYTE = buffer[index];
            this.HIBYTE = buffer[index + 1];
        }
        public shortUnion(List<byte> buffer, ushort index)
        {
            this.SHORT = 0;
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
            this.LOUSHORT = new ushortUnion(buffer, index);
            this.HIUSHORT = new ushortUnion(buffer, (ushort)(index + 2));
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
            this.LOSHORT = new shortUnion(buffer, index);
            this.HISHORT = new shortUnion(buffer, (ushort)(index + 2));
        }
        public intUnion(List<byte> buffer, ushort index)
        {
            this.INT = 0;
            this.LOSHORT = new shortUnion(buffer, index);
            this.HISHORT = new shortUnion(buffer, (ushort)(index + 2));
        }

    }



    #endregion

    public class OmronFinsEthernetProtocol
    {
        #region constants
        public const uint BUFFER_SIZE = 1024;
        public const uint MAX_DATA_SIZE = BUFFER_SIZE - 26;
        public const uint MAX_DATA_SIZE_INPUT = BUFFER_SIZE - 22;
        public const int PROTOCOL_ERROR = 1500;
        public const byte ENCAPSULATION_HEADER_SIZE = 14;
        public const byte SID = 9;
        public const byte MRES = 12;
        public const byte SRES = 13;
        public const byte MAX_EM_NR = (0xFF - 0x20);    // data bank number max value allowed  

        public const string TEST_COMM_DYNAMIC = "OmronFinsEthernet.Station={0}|LinkType=1|Addr=DM0|Conv=0";
        #endregion

        #region methods
        public static uint GetMaxJobSize(LinkType Type)
        {
            if (Type == DriverCodeBaseEx.Enumerators.LinkType.Input)
                return  MAX_DATA_SIZE_INPUT;
            else
                return  MAX_DATA_SIZE;
        }
        public static UFUAModel.DataType DataType(OmronAddress addObj)
        {
            switch (addObj.DataFormat)
            {
                case DataFormats.Word:
                    return UFUAModel.DataType.UInt16;
                case DataFormats.Bit:
                    return UFUAModel.DataType.Boolean;
                case DataFormats.String:
                    return UFUAModel.DataType.String;
                default:
                    return 0;
            }
        }
        #endregion

        #region methods override
        public static uint GetFrameLength(OmronFinsEthernetCommJob job)
        {
            return BUFFER_SIZE;
        }
        
        public static ushort PrepareRequest(OmronFinsEthernetCommJob j, ref byte[] buffer,OmronFinsEthernetChannel c)
        {
            bool isWriteCommand = false;

            if (j == null || (j.Station as OmronFinsEthernetStation == null))
                return 0;

            OmronFinsEthernetStation s = j.Station as OmronFinsEthernetStation;
            ushort Count = 0;
            
            UInt16 DataSize = 0;
            byte[] jobdata = new byte[DataSize];
            ushort offset = 0;

            ushortUnion Elems;
            if (j.isProtocolBool() &&
                (j.TagsList[0].TagNode.DataType != Opc.Ua.DataTypes.Boolean && j.ElementNumber == 0))
                Elems = new ushortUnion((ushort)(j.TotalJobSize * 8));
            else
                Elems = new ushortUnion((ushort)j.TotalJobSize);

            if (!j.ReadRequest())
            {
              
                isWriteCommand = true;

                object objectData = null;
                j.GetJobData(ref objectData);
                jobdata = (byte[])objectData;
                if (jobdata.Count() == 0 || j.TagsListOnWriting.Count == 0)
                    return 0;
                offset = (ushort)j.TagsListOnWriting[0].ByteOffset;
                Elems.USHORT = DataSize = (ushort) jobdata.Count();
                
            }

            buffer[Count++] = 0x80; /*ICF*/
            buffer[Count++] = 0x00; /*RSV*/
            buffer[Count++] = 0x02; /*GCT*/
            buffer[Count++] = s.DestinationNetworkAddress;  /*DNA*/
            buffer[Count++] = s.DestinationNode;			/*DA1*/ /*Ethernet Unit FINS NODE NUMBER*/
            buffer[Count++] = s.DestinationUnit;			/*DA2 == 0 for CPU*/
            buffer[Count++] = c.SourceNetworkAddress;   	/*SNA*/
            buffer[Count++] = c.SourceNode;         		/*SA1*/ /*WS FINS NODE NUMBER*/
            buffer[Count++] = c.SourceUnit;         		/*SA2*/
            buffer[Count++] = s.SID; /*SID*/
            buffer[Count++] = 0x01; /*MRC*/
            if (!isWriteCommand)
                buffer[Count++] = 0x01; /*SRC*/
            else
                buffer[Count++] = 0x02; /*SRC*/

            buffer[Count++] = GetAreaCode(j.AddressObj);

            ushortUnion adjAddress = new ushortUnion(j.AddressObj.Address.USHORT);
            byte bitAddress;

            if (j.AddressObj.DataFormat == DataFormats.Bit)
            {
                ushort nBit = (ushort)(j.AddressObj.BitNumber + offset);
                adjAddress.USHORT += (ushort)(nBit / 16);
                bitAddress = (byte)(nBit % 16);
            }
            else
            {
                adjAddress.USHORT += (ushort)(offset/2);
                bitAddress = 0;
                Elems.USHORT = (ushort)((Elems.USHORT + 1) / 2);
            }
            
            buffer[Count++] = adjAddress.HIBYTE;
            buffer[Count++] = adjAddress.LOBYTE;
            buffer[Count++] = bitAddress;
            buffer[Count++] = Elems.HIBYTE;
            buffer[Count++] = Elems.LOBYTE;

            if (isWriteCommand)
            {
                if (j.DataConversionType == DataConversionTypes.None)
                {
                    if (j.AddressObj.DataFormat == DataFormats.Bit)
                    {
                        for (int i = 0; i < DataSize; i++)
                        {
                            buffer[Count++] = jobdata[i];
                        }
                    }
                    else
                    {
                        if (j.AddressObj.DataFormat != DataFormats.String)
                        {
                            for (ushort i = 0; i < DataSize; i += 2)
                            {
                                buffer[Count++] = jobdata[i + 1];
                                buffer[Count++] = jobdata[i];
                            }
                        }
                        else
                        {
                            for (ushort i = 0; i < DataSize; i++)
                            {
                                buffer[Count++] = jobdata[i];
                            }
                        }
                    }
                }
                else if (j.DataConversionType == DataConversionTypes.BCD16Bits)
                {
                    for (ushort i = 0; i < DataSize ; i+=2)
                    {
                        ushortUnion BCDWord = new ushortUnion(jobdata, i);
                        BCDWord.USHORT = WORDToBCD(BCDWord.USHORT);
                        buffer[Count++] = BCDWord.HIBYTE;
                        buffer[Count++] = BCDWord.LOBYTE;
                    }
                }
                else if (j.DataConversionType == DataConversionTypes.BCD32Bits)
                {
                    for (ushort i = 0; i < DataSize ; i+=4)
                    {
                        uintUnion BCDDWord = new uintUnion(jobdata, i);
                        BCDDWord.UINT = DWORDToBCD(BCDDWord.UINT);
                        buffer[Count++] = BCDDWord.LOUSHORT.HIBYTE;
                        buffer[Count++] = BCDDWord.LOUSHORT.LOBYTE;
                        buffer[Count++] = BCDDWord.HIUSHORT.HIBYTE;
                        buffer[Count++] = BCDDWord.HIUSHORT.LOBYTE;
                    }
                }
            }
            return Count;
        }


        public static ushort PrepareRequestTestInformation(OmronFinsEthernetCommJob j, ref byte[] buffer, OmronFinsEthernetChannel c)
        {
            bool isWriteCommand = false;

            if (j == null || (j.Station as OmronFinsEthernetStation == null))
                return 0;

            OmronFinsEthernetStation s = j.Station as OmronFinsEthernetStation;
            ushort Count = 0;

            UInt16 DataSize = 0;
            byte[] jobdata = new byte[DataSize];
            ushort offset = 0;

            ushortUnion Elems;
            if (j.isProtocolBool() &&
                (j.TagsList[0].TagNode.DataType != Opc.Ua.DataTypes.Boolean && j.ElementNumber == 0))
                Elems = new ushortUnion((ushort)(j.TotalJobSize * 8));
            else
                Elems = new ushortUnion((ushort)j.TotalJobSize);

            if (!j.ReadRequest())
            {

                isWriteCommand = true;

                object objectData = null;
                j.GetJobData(ref objectData);
                jobdata = (byte[])objectData;
                if (jobdata.Count() == 0 || j.TagsListOnWriting.Count == 0)
                    return 0;
                offset = (ushort)j.TagsListOnWriting[0].ByteOffset;
                Elems.USHORT = DataSize = (ushort)jobdata.Count();

            }

            buffer[Count++] = 0x80; /*ICF*/
            buffer[Count++] = 0x00; /*RSV*/
            buffer[Count++] = 0x07; /*GCT*/
            buffer[Count++] = s.DestinationNetworkAddress;  /*DNA*/
            buffer[Count++] = s.DestinationNode;			/*DA1*/ /*Ethernet Unit FINS NODE NUMBER*/
            buffer[Count++] = s.DestinationUnit;			/*DA2 == 0 for CPU*/
            buffer[Count++] = c.SourceNetworkAddress;   	/*SNA*/
            buffer[Count++] = c.SourceNode;         		/*SA1*/ /*WS FINS NODE NUMBER*/
            buffer[Count++] = c.SourceUnit;         		/*SA2*/
            buffer[Count++] = s.SID; /*SID*/
            //Command code
            buffer[Count++] = 0x05; /*MRC*/           
            buffer[Count++] = 0x01; /*SRC*/
            //Data
            buffer[Count++] = 0x00; /*SRC*/
            //else
            //    buffer[Count++] = 0x02; /*SRC*/

            //buffer[Count++] = GetAreaCode(j.AddressObj);

            //ushortUnion adjAddress = new ushortUnion(j.AddressObj.Address.USHORT);
            //byte bitAddress;

            //if (j.AddressObj.DataFormat == DataFormats.Bit)
            //{
            //    ushort nBit = (ushort)(j.AddressObj.BitNumber + offset);
            //    adjAddress.USHORT += (ushort)(nBit / 16);
            //    bitAddress = (byte)(nBit % 16);
            //}
            //else
            //{
            //    adjAddress.USHORT += (ushort)(offset / 2);
            //    bitAddress = 0;
            //    Elems.USHORT = (ushort)((Elems.USHORT + 1) / 2);
            //}

            //buffer[Count++] = adjAddress.HIBYTE;
            //buffer[Count++] = adjAddress.LOBYTE;
            //buffer[Count++] = bitAddress;
            //buffer[Count++] = Elems.HIBYTE;
            //buffer[Count++] = Elems.LOBYTE;

            //if (isWriteCommand)
            //{
            //    if (j.DataConversionType == DataConversionTypes.None)
            //    {
            //        if (j.AddressObj.DataFormat == DataFormats.Bit)
            //        {
            //            for (int i = 0; i < DataSize; i++)
            //            {
            //                buffer[Count++] = jobdata[i];
            //            }
            //        }
            //        else
            //        {
            //            if (j.AddressObj.DataFormat != DataFormats.String)
            //            {
            //                for (ushort i = 0; i < DataSize; i += 2)
            //                {
            //                    buffer[Count++] = jobdata[i + 1];
            //                    buffer[Count++] = jobdata[i];
            //                }
            //            }
            //            else
            //            {
            //                for (ushort i = 0; i < DataSize; i++)
            //                {
            //                    buffer[Count++] = jobdata[i];
            //                }
            //            }
            //        }
            //    }
            //    else if (j.DataConversionType == DataConversionTypes.BCD16Bits)
            //    {
            //        for (ushort i = 0; i < DataSize; i += 2)
            //        {
            //            ushortUnion BCDWord = new ushortUnion(jobdata, i);
            //            BCDWord.USHORT = WORDToBCD(BCDWord.USHORT);
            //            buffer[Count++] = BCDWord.HIBYTE;
            //            buffer[Count++] = BCDWord.LOBYTE;
            //        }
            //    }
            //    else if (j.DataConversionType == DataConversionTypes.BCD32Bits)
            //    {
            //        for (ushort i = 0; i < DataSize; i += 4)
            //        {
            //            uintUnion BCDDWord = new uintUnion(jobdata, i);
            //            BCDDWord.UINT = DWORDToBCD(BCDDWord.UINT);
            //            buffer[Count++] = BCDDWord.LOUSHORT.HIBYTE;
            //            buffer[Count++] = BCDDWord.LOUSHORT.LOBYTE;
            //            buffer[Count++] = BCDDWord.HIUSHORT.HIBYTE;
            //            buffer[Count++] = BCDDWord.HIUSHORT.LOBYTE;
            //        }
            //    }
            //}
            return Count;
        }

        public static byte GetAreaCode(OmronAddress AddressObj)
        {
            if (AddressObj.DataFormat == DataFormats.Bit)
            {
                switch (AddressObj.DataArea)
                {
                    case DataAreas.IR:
                        return 0x30;
                    case DataAreas.HR:
                        return 0x32;
                    case DataAreas.AR:
                        return 0x33;
                    case DataAreas.DM:
                        return 0x02;
                    case DataAreas.EM:
                        return (byte)(0x20 + AddressObj.BankNumber);
                    case DataAreas.W:
                        return 0x31;
                    default:
                        return 0;
                }
            }
            else
            {
                switch (AddressObj.DataArea)
                {
                    case DataAreas.IR:
                        return 0xb0;
                    case DataAreas.HR:
                        return 0xb2;
                    case DataAreas.AR:
                        return 0xb3;
                    case DataAreas.DM:
                        return 0x82;
                    case DataAreas.EM:
                        if (AddressObj.BankNumber>=0)
                            return (byte)(0xa0 + AddressObj.BankNumber);
                        else // current bank
                            return 0x98;
                    case DataAreas.W:
                        return 0xb1;
                    case DataAreas.T:
                    case DataAreas.C:
                        return 0x89;
                    default:
                        return 0;
                }
            }
        }

        public static ushort WORDToBCD(ushort inValue)
        {
            ushort value = inValue;
            ushort tmpValue = 0;
            ushort Factor = 0;
            if (value > 9999)
            {
                tmpValue = 0x9999;
            }
            else
            {
                while (value > 0)
                {
                    tmpValue += (ushort)((value % 10) << Factor);
                    value /= 10;
                    Factor += 4;
                }
            }
            return tmpValue;
        }

        public static ushort BCDToWORD(ushort inValue)
        {
            ushort value = inValue;
            ushort tmpValue = 0;
            ushort Factor = 1;
            while (value > 0)
            {
                tmpValue += (ushort)((value % 0x10) * Factor);
                value /= 0x10;
                Factor *= 10;
            }
            return tmpValue;
        }

        public static uint DWORDToBCD(uint inValue)
        {
            uint value = inValue;
            uint tmpValue = 0;
            ushort Factor = 0;
            if (value > 99999999)
            {
                tmpValue = 0x99999999;
            }
            else
            {
                while (value > 0)
                {
                    tmpValue += (uint)((value % 10) << Factor);
                    value /= 10;
                    Factor += 4;
                }
            }
            return tmpValue;
         }

        public static uint BCDToDWORD(uint inValue)
        {
            uint value = inValue;
            uint tmpValue = 0;
            uint Factor = 1;
            while (value > 0)
            {
                tmpValue += (uint)((value % 0x10) * Factor);
                value /= 0x10;
                Factor *= 10;
            }
            return tmpValue;
        }


        public static bool ParseData(byte[] receiveBuffer, ref OmronFinsEthernetCommJob j, ref List<object> items)
        {
            bool areArguments = (items.Count > 0);
            if (areArguments )
            {
                if (items.Count() == 1)
                    return false;
                if(OmronFinsEthernetCommJob.GetDataTypeSize((uint)j.Station.GetBuiltInType(items[0].GetType())) == 0)
                    return false;
            }
            if(((OmronFinsEthernetChannel)j.Station.GetChannel()).TestConnection)
            {
                return (true);
            }
            int DataSize = receiveBuffer.Length;

            if (DataSize > 0)
            {
                List<Tag> changed = new List<Tag>();
                byte[] jobdata = new byte[DataSize];
                ushort Count = 0;

                if (j.DataConversionType == DataConversionTypes.None)
                {
                    if (j.AddressObj.DataFormat == DataFormats.Bit)
                    {
                        if (j.TagsList[0].TagNode.DataType != Opc.Ua.DataTypes.Boolean && j.ElementNumber == 0)
                        {
                            jobdata = new byte[DataSize/8];
                            for (ushort i = 0; i < DataSize; i ++)
                            {
                                if (receiveBuffer[i] != 0)
                                    jobdata[i / 8] |= (byte)(1 << (i % 8));
                            }
                        }
                        else
                        {
                            receiveBuffer.CopyTo(jobdata, 0);
                        }
                    }
                    else
                    {
                        if (j.AddressObj.DataFormat != DataFormats.String)
                        {
                            for (ushort i = 0; i < DataSize; i += 2)
                            {
                                jobdata[Count++] = receiveBuffer[i + 1];
                                jobdata[Count++] = receiveBuffer[i];
                            }
                        }
                        else
                        {
                            for (ushort i = 0; i < DataSize; i++)
                            {
                                jobdata[Count++] = receiveBuffer[i];
                            }
                        }
                    }
                }
                else if (j.DataConversionType == DataConversionTypes.BCD16Bits)
                {
                    for (ushort i = 0; i < DataSize; i += 2)
                    {
                        ushortUnion BCDWord = new ushortUnion(0);
                        BCDWord.HIBYTE = receiveBuffer[i];
                        BCDWord.LOBYTE = receiveBuffer[i+1];
                        BCDWord.USHORT = BCDToWORD(BCDWord.USHORT);
                        jobdata[Count++] = BCDWord.LOBYTE;
                        jobdata[Count++] = BCDWord.HIBYTE;
                    }
                }
                else if (j.DataConversionType == DataConversionTypes.BCD32Bits)
                {
                    for (ushort i = 0; i < DataSize; i += 4)
                    {
                        uintUnion BCDDWord = new uintUnion(0);
                        BCDDWord.LOUSHORT.HIBYTE = receiveBuffer[i];
                        BCDDWord.LOUSHORT.LOBYTE = receiveBuffer[i + 1];
                        BCDDWord.HIUSHORT.HIBYTE = receiveBuffer[i + 2];
                        BCDDWord.HIUSHORT.LOBYTE = receiveBuffer[i + 3];
                        BCDDWord.UINT = BCDToDWORD(BCDDWord.UINT);
                        jobdata[Count++] = BCDDWord.LOUSHORT.LOBYTE;
                        jobdata[Count++] = BCDDWord.LOUSHORT.HIBYTE;
                        jobdata[Count++] = BCDDWord.HIUSHORT.LOBYTE;
                        jobdata[Count++] = BCDDWord.HIUSHORT.HIBYTE;
                    }
                }
                j.SetJobData(jobdata, ref changed);
                if (areArguments)
                {
                    if (j.TagsList.Count == items.Count - 1)
                    {
                        for (int k = 0; k < j.TagsList.Count; k++)
                        {
                            items[k + 1] = j.TagsList[k].Value.Value;
                        }
                    }
                }
                else
                    items.AddRange(changed);
                return true;
            }

            return false;
        }

        public static uint GetElementSize(DataFormats DataFormat)
        {
            switch (DataFormat)
            {
                case DataFormats.Bit:
                    return 1;
                case DataFormats.Word:
                    return 2;
            }
            return 0;
        }

        /// <summary>
        /// Create an identifier using ipaddress/updport to access to driver communication object when udp local port is set
        /// </summary>
        /// <param name="ipAddress"></param>
        /// <param name="updPort"></param>
        /// <returns></returns>
        public static string GetIpAddressUdpPortID(string ipAddress, int updPort)
        {
            return string.Format("{0}:{1}", ipAddress, updPort);
        }
        #endregion
    }
}
