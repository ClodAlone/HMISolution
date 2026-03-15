using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using System.Text;

namespace ModbusTCPSlave
{
    interface IPackable
    {
        byte[] Pack();
    }
    interface IExpandable
    {
        void Expand(ref byte[] DataArray, ref int Offset);
    }

    public class ReceiveItem
    {
        public PDU Pdu;
        public bool IsValid;
        public ushort Transaction;
        public byte Station;
        public string IpAddress;

        public ReceiveItem()
        {
            Pdu = null;
            IsValid = false;
            Transaction = 0;
            Station = 0;
            IpAddress = "";
        }

        public ReceiveItem(PDU inPdu) : this()
        {
            Pdu = inPdu;
        }
    }


    public enum ModbusErrorCodes : int
    {
        ErrorCRCError = 1000,
        ErrorUnknownFunctionCode,
        ErrorReadError,
        ErrorReceiveFrameError,
        ErrorWrongSize,
        ErrorStationID,
        ErrorFunctionCode,
        ErrorProtocolIllegalFunction = ModbusSlaveProtocol.PROTOCOL_ERROR + 1,
        ErrorProtocolIllegalDataAddress = ModbusSlaveProtocol.PROTOCOL_ERROR + 2,
        ErrorProtocolIllegalDataValue = ModbusSlaveProtocol.PROTOCOL_ERROR + 3,
        ErrorProtocolSlaveDeviceFailure = ModbusSlaveProtocol.PROTOCOL_ERROR + 4,
        ErrorProtocolAcknowledge = ModbusSlaveProtocol.PROTOCOL_ERROR + 5,
        ErrorProtocolSlaveDeviceBusy = ModbusSlaveProtocol.PROTOCOL_ERROR + 6,
        ErrorProtocolMemoryParityError = ModbusSlaveProtocol.PROTOCOL_ERROR + 8,
        ErrorProtocolGatewayPathUnavailable = ModbusSlaveProtocol.PROTOCOL_ERROR + 10,
        ErrorProtocolGatewayTargetFailResponse = ModbusSlaveProtocol.PROTOCOL_ERROR + 11
    }

    public enum DataAreas : byte
    {
        Coils,
        DiscreteInputs,
        HoldingRegisters,
        InputRegisters,
    }

    public enum FunctionCodes : byte
    {
        ReadCoils = 0x01,
        ReadDiscreteInputs,
        ReadHoldingRegisters,
        ReadInputRegisters,
        WriteSingleCoil,
        WriteSingleRegister,
        WriteMultipleCoils = 0x0f,
        WriteMultipleRegisters,
        MaskWriteRegister = 0x16,
    }

    public static class BufferExpand
    {
        public static byte toByte(ref byte[] DataArray, ref int Offset)
        {
            checkSize(DataArray, Offset, 1);
            byte outval = DataArray[Offset];
            Offset += 1;
            return outval;
        }
        public static UInt16 toUInt16(ref byte[] DataArray, ref int Offset)
        {
            checkSize(DataArray, Offset, 2);
            UInt16 outval = Swap(BitConverter.ToUInt16(DataArray, Offset));
            Offset += 2;
            return outval;
        }

        public static UInt32 toUInt32(byte[] DataArray)
        {
            int Offset = 0;
            return toUInt32(ref DataArray, ref Offset);

        }

        public static UInt32 toUInt32(ref byte[] DataArray, ref int Offset)
        {
            checkSize(DataArray, Offset, 4);
            UInt32 outval = BitConverter.ToUInt16(DataArray, Offset);
            Offset += 4;
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
            return Encoding.UTF8.GetString(DataArray, 1, DataArray.Length - 1); ;
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
            } while (localValue != 0);

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
       
    public class MBAP : IPackable, IExpandable
    {
        public const ushort ModbusTcp = 0;
        public const byte Size = 6;
        public MBAP()
        {
            Init();
        }
        public MBAP(ref byte[] DataArray, ref int Offset)
        {
            Expand(ref DataArray, ref  Offset);
        }
        #region member

        private ushort _transaction;
        private ushort _protocol;
        private ushort _length;
        
        #endregion

        #region Properties
        public ushort transaction { get { return _transaction; } set { _transaction = value; } }
        public ushort protocol { get { return _protocol; } set { _protocol = value; } }
        public ushort pduLength { get { return (ushort)(_length); } set { _length = (ushort)(value); } }

        #endregion

        #region Methods
        public void Init()
        {
            _transaction = 0;
            _length = 1;
            _protocol = ModbusTcp;
        }
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.AddRange(BufferExpand.toArray(_transaction));
            retVal.AddRange(BufferExpand.toArray(_protocol));
            retVal.AddRange(BufferExpand.toArray(_length));

            return retVal.ToArray();
        }

        public void Expand(ref byte[] DataArray, ref int Offset)
        {
            _transaction = BufferExpand.toUInt16(ref DataArray, ref Offset);
            _protocol = BufferExpand.toUInt16(ref DataArray, ref Offset);
            _length = BufferExpand.toUInt16(ref DataArray, ref Offset);
        }
        #endregion
    }

    public class PDU : IPackable, IExpandable
    {
        public const byte SizeMin = 6;
        public const byte SizeMax = 254;
        public PDU()
        {
            Init();
        }
        public PDU(ref byte[] DataArray, ref int Offset)
        {
            Expand(ref DataArray, ref  Offset);
        }
        #region member

        private byte _unit;
        private FunctionCodes _functionCode;
        private ushort _startAddress;
        private ushort _quantity;
        private ushort _AndMask;
        private ushort _OrMask;
        private byte _byteCount;
        public byte[] _byteData;
        private byte _exception;
        public bool isValid;

        #endregion

        #region Properties
        public byte unit { get { return _unit; } set { _unit = value; } }
        public FunctionCodes functionCode { get { return _functionCode; } set { _functionCode = value; } }
        public ushort startAddress { get { return _startAddress; } set { _startAddress = value; } }
        public ushort quantity { get { return _quantity; } set { _quantity = value; } }
        public ushort AndMask { get { return _AndMask; } set { _AndMask = value; } }
        public ushort OrMask { get { return _OrMask; } set { _OrMask = value; } }
        public byte byteCount { get { return _byteCount; } set { _byteCount = value; } }
        public byte[] byteData { get { return _byteData; } set { _byteData = value; } }
        public byte exception { get { return _exception; } set { _exception = value; } }

        #endregion

        #region Methods
        public void Init()
        {
            _unit = 0;
            _functionCode = FunctionCodes.WriteSingleRegister;
            _startAddress = 0;
            _quantity = 1;
            _byteCount = 2;
            _exception = 0;
            _AndMask = 0xFFFF;
            _OrMask = 0;

        }
        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            retVal.Add(_unit);
            if (_exception == 0)
            {
                retVal.Add((byte)_functionCode);
                switch (_functionCode)
                {
                    case FunctionCodes.ReadCoils:
                    case FunctionCodes.ReadDiscreteInputs:
                    case FunctionCodes.ReadHoldingRegisters:
                    case FunctionCodes.ReadInputRegisters:
                        retVal.Add((byte)_byteData.Length);
                        retVal.AddRange(_byteData);
                        break;
                    case FunctionCodes.WriteSingleCoil:
                    case FunctionCodes.WriteSingleRegister:
                        retVal.AddRange(BufferExpand.toArray(_startAddress));
                        retVal.AddRange(_byteData);
                        break;
                    case FunctionCodes.WriteMultipleCoils:
                    case FunctionCodes.WriteMultipleRegisters:
                        retVal.AddRange(BufferExpand.toArray(_startAddress));
                        retVal.AddRange(BufferExpand.toArray(_quantity));
                        break;
                    case FunctionCodes.MaskWriteRegister:
                        retVal.AddRange(BufferExpand.toArray(_startAddress));
                        retVal.AddRange(BufferExpand.toArray(_AndMask));
                        retVal.AddRange(BufferExpand.toArray(_OrMask));
                        break;
                }
            }
            else
            {
                retVal.Add((byte)((byte)_functionCode | 0x80));
                retVal.Add(_exception);
            }

            return retVal.ToArray();
        }

        public void Expand(ref byte[] DataArray, ref int Offset)
        {
            _unit = BufferExpand.toByte(ref DataArray, ref Offset);
            _functionCode = (FunctionCodes)BufferExpand.toByte(ref DataArray, ref Offset);
            isValid = true;
            switch (_functionCode)
            {
                case FunctionCodes.ReadCoils:
                case FunctionCodes.ReadDiscreteInputs:
                case FunctionCodes.ReadHoldingRegisters:
                case FunctionCodes.ReadInputRegisters:
                    _startAddress = BufferExpand.toUInt16(ref DataArray, ref Offset);
                    _quantity = BufferExpand.toUInt16(ref DataArray, ref Offset);
                    break;
                case FunctionCodes.WriteSingleCoil:
                    _startAddress = BufferExpand.toUInt16(ref DataArray, ref Offset);
                    _quantity = 1;
                    _byteCount = 2;
                    _byteData = BufferExpand.toArray(ref DataArray, ref Offset, _byteCount);
                    break;
                case FunctionCodes.WriteSingleRegister:
                    _startAddress = BufferExpand.toUInt16(ref DataArray, ref Offset);
                    _quantity = 1;
                    _byteCount = 2;
                    _byteData = BufferExpand.toArray(ref DataArray,ref Offset,_byteCount);
                    break;
                case FunctionCodes.WriteMultipleCoils:
                case FunctionCodes.WriteMultipleRegisters:
                    _startAddress = BufferExpand.toUInt16(ref DataArray, ref Offset);
                    _quantity = BufferExpand.toUInt16(ref DataArray, ref Offset);
                    _byteCount = BufferExpand.toByte(ref DataArray, ref Offset);
                    _byteData = BufferExpand.toArray(ref DataArray,ref Offset,_byteCount);
                    break;
                case FunctionCodes.MaskWriteRegister:
                    _startAddress = BufferExpand.toUInt16(ref DataArray, ref Offset);
                    _quantity = 1;
                    _byteCount = 2;
                    _AndMask = BufferExpand.toUInt16(ref DataArray, ref Offset);
                    _OrMask = BufferExpand.toUInt16(ref DataArray, ref Offset);
                    break;
                default:
                    _exception = 1;
                    break;

            }

        }
        #endregion
    }

    public class ADU : IPackable, IExpandable
    {
        public ADU()
        {
            Init();
        }
        public ADU(ref byte[] DataArray, ref int Offset)
        {
            Expand(ref DataArray, ref  Offset);
        }
        //Fields
        private MBAP _Mbap;
        private PDU _Pdu;

        //Properties
        public MBAP Mbap { get { return _Mbap; } set { _Mbap = value; } }
        public PDU Pdu { get { return _Pdu; } set { _Pdu = value; } }


        //Methods
        public void Init()
        {
            _Mbap = new MBAP();
            _Pdu = new PDU();

        }

        public byte[] Pack()
        {
            List<byte> retVal = new List<byte>();
            byte[] pduBuffer = _Pdu.Pack();
            _Mbap.pduLength = (UInt16)pduBuffer.Length;
            retVal.AddRange(_Mbap.Pack());
            retVal.AddRange(pduBuffer);
            return retVal.ToArray();
        }

        public void Expand(ref byte[] DataArray, ref int Offset)
        {
            Init();
            _Mbap.Expand(ref DataArray, ref Offset);
            _Pdu.Expand(ref DataArray, ref Offset);
        }

    }

    public class ModbusSlaveProtocol
    {
        public const int INT_AnswerLen = 13;
        public const int PROTOCOL_ERROR = 1500;

        private const uint UINT_FileReq = 10;
        public const int UINT_RequestLen = 6;
        private const uint UINT_WriteRequestLen = 7;
        private const uint UINT_WriteFileReqLen = 10;

        public const int maxADUsize = 260;
        public const int MBAPHeaderSize = 6;

        #region methods
        public static bool validateFunctionCode(FunctionCodes functionCode)
        {
            switch (functionCode)
            {
                case FunctionCodes.ReadCoils:
                case FunctionCodes.ReadDiscreteInputs:
                case FunctionCodes.ReadHoldingRegisters:
                case FunctionCodes.ReadInputRegisters:
                case FunctionCodes.WriteSingleCoil:
                case FunctionCodes.WriteSingleRegister:
                case FunctionCodes.WriteMultipleCoils:
                case FunctionCodes.WriteMultipleRegisters:
                case FunctionCodes.MaskWriteRegister:
                    return true;
            }
            return false;
        }
        public static DataAreas DataAreaOfFunctionCode(FunctionCodes functionCode)
        {
            switch (functionCode)
            {
                case FunctionCodes.ReadDiscreteInputs:
                    return DataAreas.DiscreteInputs;
                case FunctionCodes.ReadInputRegisters:
                    return DataAreas.InputRegisters;
                case FunctionCodes.WriteSingleCoil:
                case FunctionCodes.WriteMultipleCoils:
                case FunctionCodes.ReadCoils:
                    return DataAreas.Coils;
                case FunctionCodes.WriteMultipleRegisters:
                case FunctionCodes.WriteSingleRegister:
                case FunctionCodes.MaskWriteRegister:
                case FunctionCodes.ReadHoldingRegisters:
                    return DataAreas.HoldingRegisters;
            }
            return unchecked((DataAreas)(-1));

        }
        public static ushort maxSizeOfDataArea(DataAreas dataArea)
        {
            switch (dataArea)
            {
                case DataAreas.Coils:
                case DataAreas.DiscreteInputs:
                    return 0x07D0;
                case DataAreas.InputRegisters:
                case DataAreas.HoldingRegisters:
                    return 0x07D;
            }
            return 0;

        }
        public static ushort maxSizeOfFunctionCode(FunctionCodes functionCode)
        {
            return maxSizeOfDataArea(DataAreaOfFunctionCode(functionCode));
        }
        #endregion

        #region methods override

        public static bool ParseData(byte[] receivedbuffer, ref ModbusTCPSlaveCommJob job, ref List<object> items)
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
        public static uint GetMaxJobSize(DataAreas DataArea)
        {

            if ((DataArea == DataAreas.DiscreteInputs) ||
               (DataArea == DataAreas.InputRegisters) )
            {
                return 250;
            }
            else
            {
                switch (DataArea)
                {
                    case DataAreas.Coils:
                        return 100;
                    case DataAreas.HoldingRegisters:
                        return 200;
                }
            }

            return 0;
        }

    }
}
