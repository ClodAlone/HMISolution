using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using System.Runtime.InteropServices;
using Opc.Ua;

namespace SaiaDataMode
{
    #region enums
    public enum ChannelTypes : int
    {
        Serial,
        Socket,
    };

    public enum AreaTypes : int
    {
        Inputs,
        Outputs,
        Flags,
        Registers,
        Timers,
        Counters,
        DataBlock,
    };

    public enum DataConversionTypes : int
    {
        None,
        FFP,
    };

    public enum SaiaDataModetErrorCodes : int
    {
        ErrorInvalidLenght = 1000,
        ErrorIncompleteReply,
        ErrorCrc,
        ErrorWrongSequence,
        ErrorWriteResponseAttribute,
        ErrorReadResponseAttribute,
        ErrorReadResponseNack,
        ErrorReadResponseLength,
    }

    public enum ReplyAttributes : int
    {
        Response = 1,
        ACK_NAK,
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
    public struct ulongUnion
    {
        [FieldOffset(0)]
        public ulong ULONG;

        [FieldOffset(0)]
        public uintUnion LOUINT;
        [FieldOffset(4)]
        public uintUnion HIUINT;

        // Constructor:
        public ulongUnion(ulong ULONG)
        {
            this.LOUINT = new uintUnion(0x0000);
            this.HIUINT = new uintUnion(0x0000);
            this.ULONG = ULONG;
        }
        public ulongUnion(byte[] buffer, ushort index)
        {
            this.ULONG = 0;
            this.HIUINT = new uintUnion(buffer, index);
            this.LOUINT = new uintUnion(buffer, (ushort)(index + 4));
        }
        public ulongUnion(List<byte> buffer, ushort index)
        {
            this.ULONG = 0;
            this.HIUINT = new uintUnion(buffer, index);
            this.LOUINT = new uintUnion(buffer, (ushort)(index + 4));
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
    public struct longUnion
    {
        [FieldOffset(0)]
        public long LONG;

        [FieldOffset(0)]
        public intUnion LOINT;
        [FieldOffset(4)]
        public intUnion HIINT;

        // Constructor:
        public longUnion(long LONG)
        {
            this.LOINT = new intUnion(0x0000);
            this.HIINT = new intUnion(0x0000);
            this.LONG = LONG;
        }
        public longUnion(byte[] buffer, ushort index)
        {
            this.LONG = 0;
            this.HIINT = new intUnion(buffer, index);
            this.LOINT = new intUnion(buffer, (ushort)(index + 4));
        }
        public longUnion(List<byte> buffer, ushort index)
        {
            this.LONG = 0;
            this.HIINT = new intUnion(buffer, index);
            this.LOINT = new intUnion(buffer, (ushort)(index + 4));
        }

    }

    [StructLayout(LayoutKind.Explicit)]
    public struct floatUnion
    {
        [FieldOffset(0)]
        public float FLOAT;
        [FieldOffset(0)]
        public intUnion INTUNION;
        [FieldOffset(0)]
        public uintUnion UINTUNION;

        // Constructor:
        public floatUnion(float FLOAT)
        {
            this.INTUNION = new intUnion(0x0000);
            this.UINTUNION = new uintUnion(0x0000);
            this.FLOAT = FLOAT;
        }
        public floatUnion(int INT)
        {
            this.FLOAT = 0;
            this.UINTUNION = new uintUnion(0x0000);
            this.INTUNION = new intUnion(INT);
        }
        public floatUnion(uint UINT)
        {
            this.FLOAT = 0;
            this.INTUNION = new intUnion(0x0000);
            this.UINTUNION = new uintUnion(UINT);
        }
        public floatUnion(byte[] buffer, ushort index)
        {
            this.FLOAT = 0;
            this.INTUNION = new intUnion(0x0000);
            this.UINTUNION = new uintUnion(buffer, index);
        }
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct doubleUnion
    {
        [FieldOffset(0)]
        public double DOUBLE;
        [FieldOffset(0)]
        public longUnion LONGUNION;
        [FieldOffset(0)]
        public ulongUnion ULONGUNION;

        // Constructor:
        public doubleUnion(double DOUBLE)
        {
            this.ULONGUNION = new ulongUnion(0x0000);
            this.LONGUNION = new longUnion(0x0000);
            this.DOUBLE = DOUBLE;
        }
        public doubleUnion(ulong ULONG)
        {
            this.DOUBLE = 0;
            this.LONGUNION = new longUnion(0);
            this.ULONGUNION = new ulongUnion(ULONG);
        }
        public doubleUnion(long LONG)
        {
            this.DOUBLE = 0;
            this.ULONGUNION = new ulongUnion(0);
            this.LONGUNION = new longUnion(LONG);
        }
        public doubleUnion(byte[] buffer, ushort index)
        {
            this.DOUBLE = 0;
            this.LONGUNION = new longUnion(0);
            this.ULONGUNION = new ulongUnion(buffer, index);
        }
    }

    #endregion

    public class SaiaDataModeProtocol
    {
        #region static const
        public const int PROTOCOL_ERROR = 1500;
        public const uint MAX_DATA_SIZE = 128;
        public const uint PROTOCOL_OVER = 20;

        public const byte VersionOffs = 4;
        public const byte ProtocolTypeOffs = 5;
        public const byte SequenceOffs = 6;
        public const byte DestinationOffs = 9;
        public const byte DataOffs = 9;

        public const byte ReplyOverhead = 11;
        public const byte WriteReplyLen = 2;

        public const ushort MaxMessageLength = 1024;

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

        public static bool ParseData(byte[] receivedbuffer, ref SaiaDataModeCommJob job, ref List<Object> items)
        {
            List<Tag> changed = new List<Tag>();
            uint recivedSize = (uint)receivedbuffer.Length;

            if (job.isProtocolBool() &&
                (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean || job.ElementNumber > 0))
                recivedSize *= 8;

            byte[] tempBuffer = new byte[recivedSize];
            if (job.isProtocolBool())
            {
                if (job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Boolean ||
                    job.ElementNumber > 0)
                {
                    for (ushort bitIndex = 0; bitIndex < recivedSize; bitIndex++)
                    {
                        tempBuffer[bitIndex] = (byte)((receivedbuffer[bitIndex / 8] >> (bitIndex % 8)) & 1);
                    }
                }
                else 
                {
                    Array.Copy(receivedbuffer, 0, tempBuffer, 0, recivedSize);
                }
            }
            else
            {
                ushort Count = 0;
                for (ushort i = 0; i < recivedSize; i += 4)
                {
                    if (job.AreaType == AreaTypes.Registers && 
                        job.DataConversionType == DataConversionTypes.FFP &&
                        job.TagsList[0].TagNode.DataType == Opc.Ua.DataTypes.Float)
                    {
                        uintUnion value = new uintUnion(receivedbuffer, (ushort)i);
                        floatUnion Result = new floatUnion(job.ConvertFFPToIEEE(value.UINT));
                        tempBuffer[Count++] = Result.UINTUNION.LOUSHORT.LOBYTE;
                        tempBuffer[Count++] = Result.UINTUNION.LOUSHORT.HIBYTE;
                        tempBuffer[Count++] = Result.UINTUNION.HIUSHORT.LOBYTE;
                        tempBuffer[Count++] = Result.UINTUNION.HIUSHORT.HIBYTE;
                    }
                    else
                    {
                        tempBuffer[Count++] = receivedbuffer[i + 3];
                        tempBuffer[Count++] = receivedbuffer[i + 2];
                        tempBuffer[Count++] = receivedbuffer[i + 1];
                        tempBuffer[Count++] = receivedbuffer[i];
                    }
                }
            }
            job.SetJobData(tempBuffer, ref changed);
            items.AddRange(changed);

            return true;
        }
        #endregion

        public static uint GetMaxJobSize(AreaTypes AreaType)
        {
            if (AreaType < AreaTypes.Registers)
                return SaiaDataModeProtocol.MAX_DATA_SIZE / 8;
            else
                return SaiaDataModeProtocol.MAX_DATA_SIZE;
        }


    }
}
