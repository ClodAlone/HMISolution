using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using System.Runtime.InteropServices;
using Opc.Ua;

namespace NaisFp
{
    #region enums

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

    public enum NaisFptErrorCodes : int
    {
        ErrorInvalidLenght = 1000,
        ErrorIncompleteReply,
        ErrorCrc,
        ErrorWrongSequence,
        ErrorWriteResponseAttribute,
        ErrorReadResponseAttribute,
        ErrorReadResponseNack,
        ErrorReadResponseLength,
        ErrorNotExpectedMessage,
        ErrorBCC,
        ErrorBadRxChars,
        ErrorStationUnmismatch,
        ErrorFormat,
        ErrorWrongCommand,
        ErrorProc,
        ErrorReturnUnknownCode,
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

    public struct ComBuffer
    {
        public byte[] buffer;
        public ushort size;
        public ushort pointer;

        public ComBuffer(ushort size)
        {
            this.size = size;
            this.buffer = new byte[size];
            this.pointer = 0;
        }

        public bool insert(byte value)
        {
            if (size > pointer)
            {
                buffer[pointer++] = value;
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool insert(char value)
        {
            return insert((byte)(value & 0xff));
        }

        public bool compare(ushort index ,char value)
        {
            return buffer[index] == (byte)(value & 0xff);
        }

        public bool insert(string value)
        {
            ushort len = (ushort)value.Length;
            if (size >= pointer + len)
            {
                byte[] asciiBytes = Encoding.ASCII.GetBytes(value);
                foreach (byte b in asciiBytes)
                {
                    if(!insert(b))
                        return false;
                } 
                return true;
            }
            else
            {
                return false;
            }
        }
 
        public bool insert(ComBuffer inTxBuffer)
        {
            if (size >= pointer + inTxBuffer.pointer)
            {
                int i = 0;
                while (i < inTxBuffer.pointer)
                {
                    if(!insert(inTxBuffer.buffer[i++]))
                        return false;
                }
                return true;
            }
            else
            {
                return false;
            }
        }

        public void clear()
        {            
            // clear buffer contents
            for (int i =0; i < pointer; i++)
                buffer[i] = 0;
            pointer = 0;            
        }
    }

    #endregion

    public class NaisFpProtocol
    {
        #region static const

        public const byte ReplyOverhead = 9;
        public const byte OverheadFP2 = 12;
        public const byte SupervisorIDFP2 = 10;
        public const byte StartHeader = 0;
        public const byte StationId = 1;
        public const byte EndHeader = 3;
        public const byte ErrCode = 4;
        public const byte InitReadData = 6;

        public const ushort MAX_DATA_BYTES = 49;
        public const ushort MAX_DATA_BYTES_LONG_MSG	= 1014;
        public const ushort MAX_FRAME_LENGTH_LONG_MSG = 2048;
        public const string TEST_COMM_DYNAMIC_SETTINGS = "NaisFp.Station={0}|LinkType=1|Addr=DT0";

        #endregion

        #region methods
        public static uint GetMaxJobSize(FrameFormats FrameFormat)
        {
            uint ReturnValue = MAX_DATA_BYTES;
            if (FrameFormat == FrameFormats.Long)
            {
                ReturnValue = MAX_DATA_BYTES_LONG_MSG;
            }
            return (ReturnValue);
        }
        public static UFUAModel.DataType DataType(NaisFpAddress addObj)
        {
            switch (addObj.DataFormat)
            {
                case DataFormats.DWORD:
                    return UFUAModel.DataType.UInt32;
                case DataFormats.WORD:
                    return UFUAModel.DataType.UInt16;
                case DataFormats.BOOL:
                    return UFUAModel.DataType.Boolean;
                default:
                    return 0;
            }
        }
        #endregion

        #region methods override

        public static bool CheckCRC(byte[] buffer)
        {
            ushortUnion nCRC = CalculateCRC(buffer, (uint)buffer.Count() - 2);
            ushortUnion readCRC = new ushortUnion(buffer, (ushort)(buffer.Count() - 2));
            return (readCRC.USHORT == nCRC.USHORT);

        }

        public static ushortUnion CalculateCRC(byte[] buffer, uint Count)
        {
            ushortUnion nCRC = new ushortUnion(0);
            for (uint i = 0; i < Count ; i++)
            {
                byte tmp = nCRC.LOBYTE;
                nCRC.LOBYTE = nCRC.HIBYTE;
                nCRC.HIBYTE = tmp;
                nCRC.LOBYTE ^= buffer[i];
                nCRC.LOBYTE ^= (byte)(nCRC.LOBYTE >> 4);
                nCRC.HIBYTE ^= (byte)(nCRC.LOBYTE << 4);
                nCRC.USHORT ^= (ushort)((nCRC.LOBYTE << 4) << 1);
            }
            return nCRC;
        }
        public static bool ParseData(byte[] receiveBuffer, ref NaisFpCommJob job, ref List<Object> items)
        {
            bool areArguments = (items.Count > 0);
            if (!areArguments && (receiveBuffer == null))
            {
                return (false);
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
                    return false;
                }

                items[0] = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
                return (true);
            }

            if (areArguments)
            {
                items[0] = DriverCodeBaseEx.Enumerators.DriverErrorCodes.ErrorNoError;
            }


            uint DataSize = (uint)receiveBuffer.Length;
            if (DataSize > 0)
            {
                List<Tag> changed = new List<Tag>();
                uint recivedSize;
                byte[] tempBuffer;

                if (job.AddressObj.DataFormat == DataFormats.BOOL)
                {
                    if (job.TagsList[0].TagNode.DataType != Opc.Ua.DataTypes.Boolean && job.ElementNumber == 0)
                    {
                        tempBuffer = receiveBuffer;
                    }
                    else
                    {
                        recivedSize = DataSize * 8;
                        tempBuffer = new byte[recivedSize];
                        for (ushort bitCnt = 0; bitCnt < recivedSize; bitCnt++)
                        {
                            tempBuffer[bitCnt] = (byte)((receiveBuffer[bitCnt / 8] >> (bitCnt % 8)) & 1);
                        }
                    }
                }
                else
                {
                    tempBuffer = receiveBuffer;
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

            return false;
        }
        #endregion


    }
}
