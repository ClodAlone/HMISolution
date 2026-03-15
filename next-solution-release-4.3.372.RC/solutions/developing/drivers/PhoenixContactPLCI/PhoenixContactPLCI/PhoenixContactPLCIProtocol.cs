using System;
using System.Collections.Generic;
using Opc.Ua;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using System.Text;
using System.Linq;

namespace PhoenixContactPLCI
{
    public class PhoenixContactPLCIProtocol
    {
        public const int MAX_REQUEST_MESSAGE_SIZE = 934;
        public const int MAX_SUBSCRIPTIONS_REQUEST_MESSAGE_SIZE = 958;
        public const uint MAX_DATA_BYTES = 0x10000;        
        public const int DEFAULT_PORT = 41100;
        public const string PLC_IMPORT_SOURCE_PATH = @"Eclr:/Domains/ProConOS/ImageFile/Meta.bin";

        public enum ConnectionState
        {
            Unknown,            // initial connection state (1st loop)
            Connected,          // connected to plc, but plc is not is running state --> data cannot be exchange in this state
            ConnectedAndRunning,// connected to plc and plc state is running --> data now can be exchanged 
            Disconnected        // disconnected to plc
        }

        public enum ErrorCodes : int
        {
            ErrorGenericPlciException = 1001,
            ErrorFatalPlciException,
            ErrorRecoverablePlciException,
            ErrorUnMappedTag,
            ErrorConnectionBroken,
            ErrorInvalidDataFormat,
            ErrorInvalidArraySize,
            ErrorGetFromPlcBadBinFilePath,
            ErrorGetFromPlcCannotWriteBinFile
        }
               
        public enum VarType : uint
        {            
            BOOL = 0,
            SINT = 1,
            INT = 2,
            DINT = 3,
            USINT = 4,
            UINT = 5,
            UDINT = 6,
            REAL = 7,
            LREAL = 8,
            TIME = 9,
            BYTE = 10,
            WORD = 11,
            DWORD = 12,
            STRING = 13,
            STRUCT = 14,
            UNKNOWN = 0x80040200
        }

        public class VarRead
        {
            public string Address { set; get; }

            public int ReadValueIndex { set; get; }

            public PhoenixContactPLCICommJob Job { set; get; }

            public DriverErrorCodes ErrorCode { set; get; }

            public VarRead()
            {
                Address = string.Empty;
                ReadValueIndex = 0;
                Job = null;
                ErrorCode = DriverErrorCodes.ErrorNoError;
            }

            public VarRead(PhoenixContactPLCICommJob job, int readValueIndex)
            {
                Job = job;
                Address = Job.Address;
                ReadValueIndex = readValueIndex;
                ErrorCode = DriverErrorCodes.ErrorNoError;
            }
        }

        public class VarWrite
        {
            public string Address { set; get; }

            public PhoenixContactPLCICommJob Job { set; get; }

            public object Data { set; get; }

            public DriverErrorCodes ErrorCode { set; get; }

            public VarWrite()
            {
                Address = string.Empty;
                Data = null;
                Job = null;
                ErrorCode = DriverErrorCodes.ErrorNoError;
            }

            public VarWrite(PhoenixContactPLCICommJob job, object data)
            {
                Job = job;
                Address = Job.Address;
                Data = data;
                ErrorCode = DriverErrorCodes.ErrorNoError;
            }
        }

        /// <summary>
        /// Convert data readed from PLC (.Net Framework object) into array of byte
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="readedValue"></param>
        /// <param name="result"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool GetByteFromReadedValue(VarType varType, object readedValue, out byte[] result, out ErrorCodes error)
        {
            error = (ErrorCodes)(DriverErrorCodes.ErrorNoError);

            result = null;
            try
            {
                switch (varType)
                {
                    case VarType.BOOL:
                        result = BitConverter.GetBytes((bool)(readedValue));
                        break;
                    case VarType.SINT:
                        result = BitConverter.GetBytes((sbyte)(readedValue));
                        break;
                    case VarType.INT:
                        result = BitConverter.GetBytes((Int16)(readedValue));
                        break;
                    case VarType.DINT:
                        result = BitConverter.GetBytes((Int32)(readedValue));
                        break;
                    case VarType.USINT:
                        result = BitConverter.GetBytes((byte)(readedValue));
                        break;
                    case VarType.UINT:
                        result = BitConverter.GetBytes((UInt16)(readedValue));
                        break;
                    case VarType.UDINT:
                        result = BitConverter.GetBytes((UInt32)(readedValue));
                        break;
                    case VarType.REAL:
                        result = BitConverter.GetBytes((float)(readedValue));
                        break;
                    case VarType.LREAL:
                        result = BitConverter.GetBytes((double)(readedValue));
                        break;
                    case VarType.TIME:
                        result = BitConverter.GetBytes((UInt32)(readedValue));
                        break;
                    case VarType.BYTE:
                        result = BitConverter.GetBytes((byte)(readedValue));
                        break;
                    case VarType.WORD:
                        result = BitConverter.GetBytes((UInt16)(readedValue));
                        break;
                    case VarType.DWORD:
                        result = BitConverter.GetBytes((UInt32)(readedValue));
                        break;
                    case VarType.STRING:
                        result = ASCIIEncoding.ASCII.GetBytes(readedValue.ToString());
                        break;
                    default:
                        error = ErrorCodes.ErrorInvalidDataFormat;
                        break;
                }
            } catch (Exception ex) {
                error = ErrorCodes.ErrorInvalidDataFormat;
            }
            
            return (error == (ErrorCodes)DriverErrorCodes.ErrorNoError);
        }

        /// <summary>
        /// Convert data (array of elements) readed from PLC (.Net Framework object) into array of byte; is array size mismatch with tag definition return error
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="arrayDimension"></param>
        /// <param name="readedValue"></param>
        /// <param name="result"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public static bool GetByteFromReadedArrayOfValues(VarType varType, uint arrayDimension, uint stringLength, object readedValue, CommJob j, out byte[] result, out ErrorCodes error)
        {
            uint readedArrayDimension = 0;
            error = (ErrorCodes)(DriverErrorCodes.ErrorNoError);

            result = null;

            try
            {
                switch (varType)
                {
                    case VarType.BOOL:
                        {
                            List<bool> arr = (List<bool>)readedValue;
                            readedArrayDimension = (uint)arr.Count;
                            result = arr.SelectMany(BitConverter.GetBytes).ToArray();
                        }
                        break;
                    case VarType.SINT:
                        {
                            List<sbyte> arr = (List<sbyte>)readedValue;
                            readedArrayDimension = (uint)arr.Count;
                            result = arr.Select(v => (byte)(v)).ToArray();
                        }
                        break;
                    case VarType.INT:
                        {
                            List<Int16> arr = (List<Int16>)readedValue;
                            readedArrayDimension = (uint)arr.Count;
                            result = arr.SelectMany(BitConverter.GetBytes).ToArray();
                        }
                        break;
                    case VarType.DINT:
                        {
                            List<Int32> arr = (List<Int32>)readedValue;
                            readedArrayDimension = (uint)arr.Count;
                            result = arr.SelectMany(BitConverter.GetBytes).ToArray();
                        }
                        break;
                    case VarType.USINT:
                        {
                            List<byte> arr = (List<byte>)readedValue;
                            readedArrayDimension = (uint)arr.Count;
                            result = arr.ToArray();
                        }
                        break;
                    case VarType.UINT:                        
                        {
                            List<UInt16> arr = (List<UInt16>)readedValue;
                            readedArrayDimension = (uint)arr.Count;
                            result = arr.SelectMany(BitConverter.GetBytes).ToArray();
                        }
                        break;
                    case VarType.UDINT:
                        {
                            List<UInt32> arr = (List<UInt32>)readedValue;
                            readedArrayDimension = (uint)arr.Count;
                            result = arr.SelectMany(BitConverter.GetBytes).ToArray();
                        }
                        break;
                    case VarType.REAL:
                        {
                            List<float> arr = (List<float>)readedValue;
                            readedArrayDimension = (uint)arr.Count;
                            result = arr.SelectMany(BitConverter.GetBytes).ToArray();
                        }
                        break;
                    case VarType.LREAL:
                        {
                            List<double> arr = (List<double>)readedValue;
                            readedArrayDimension = (uint)arr.Count;
                            result = arr.SelectMany(BitConverter.GetBytes).ToArray();
                        }
                        break;
                    case VarType.TIME:
                        {
                            List<UInt32> arr = (List<UInt32>)readedValue;
                            readedArrayDimension = (uint)arr.Count;
                            result = arr.SelectMany(BitConverter.GetBytes).ToArray();
                        }
                        break;
                    case VarType.BYTE:
                        {
                            List<byte> arr = (List<byte>)readedValue;
                            readedArrayDimension = (uint)arr.Count;
                            result = arr.ToArray();
                        }
                        break;
                    case VarType.WORD:
                        {
                            List<UInt16> arr = (List<UInt16>)readedValue;
                            readedArrayDimension = (uint)arr.Count;
                            result = arr.SelectMany(BitConverter.GetBytes).ToArray();
                        }
                        break;
                    case VarType.DWORD:
                        {
                            List<UInt32> arr = (List<UInt32>)readedValue;
                            readedArrayDimension = (uint)arr.Count;
                            result = arr.SelectMany(BitConverter.GetBytes).ToArray();
                        }
                        break;
                    case VarType.STRING:
                        {
                            List<string> lst = (List<string>)readedValue;
                            readedArrayDimension = (uint)lst.Count;
                            int nBufferLenght = 0;
                            int longeststring = 0;
                            //Scroll down the list to get the length of the longest string.
                            for (int i = 0; i < readedArrayDimension; i++)
                            {
                                nBufferLenght += lst[i].Length;
                                if(longeststring < lst[i].Length)
                                {
                                    longeststring = lst[i].Length;
                                }
                            }
                            //Size job update
                            j.TagsList[0].Size = (uint)(readedArrayDimension * longeststring);
                            byte[] arrResult = new byte[j.TagsList[0].Size];
                            for (int i = 0; i < arrayDimension; i++)
                            { 
                                Array.Copy(ASCIIEncoding.ASCII.GetBytes(lst[i]), 0, arrResult, i * longeststring, lst[i].Length);
                            }
                            result = arrResult;
                        }
                        break;
                    default:
                        error = ErrorCodes.ErrorInvalidDataFormat;
                        break;
                }
            }
            catch (Exception ex) {
                error = ErrorCodes.ErrorInvalidDataFormat;
            }

            if (error == (ErrorCodes)DriverErrorCodes.ErrorNoError && result != null) {
                if (readedArrayDimension != arrayDimension)
                    error = ErrorCodes.ErrorInvalidArraySize;
            }

            return (error == (ErrorCodes)DriverErrorCodes.ErrorNoError);
        }

        /// <summary>
        /// Convert data to write (from Movicon) into PLC data (.Net Framework object)
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="writeValue"></param>
        /// <returns></returns>
        public static object GetObjectFromWriteValue(VarType varType, byte[] writeValue)
        {
            object result = null;

            try
            {
                switch (varType)
                {
                    case VarType.BOOL:
                        result = (object)BitConverter.ToBoolean(writeValue, 0);
                        break;
                    case VarType.SINT:
                        result = (object)((sbyte)writeValue[0]);
                        break;
                    case VarType.INT:
                        result = (object)BitConverter.ToInt16(writeValue, 0);
                        break;
                    case VarType.DINT:
                        result = (object)BitConverter.ToInt32(writeValue, 0);
                        break;
                    case VarType.USINT:
                        result = (object)((byte)writeValue[0]);
                        break;
                    case VarType.UINT:
                        result = (object)BitConverter.ToUInt16(writeValue, 0);
                        break;
                    case VarType.UDINT:
                        result = (object)BitConverter.ToUInt32(writeValue, 0);
                        break;
                    case VarType.REAL:
                        result = (object)BitConverter.ToSingle(writeValue, 0);
                        break;
                    case VarType.LREAL:
                        result = (object)BitConverter.ToDouble(writeValue, 0);
                        break;
                    case VarType.TIME:
                        result = (object)BitConverter.ToUInt32(writeValue, 0);
                        break;
                    case VarType.BYTE:
                        result = (object)((byte)writeValue[0]);
                        break;
                    case VarType.WORD:
                        result = (object)BitConverter.ToUInt16(writeValue, 0);
                        break;
                    case VarType.DWORD:
                        result = (object)BitConverter.ToUInt32(writeValue, 0);
                        break;
                    case VarType.STRING:
                        // converting from array of byte, unused array's element remain 0 --> remove from end
                        result = ASCIIEncoding.ASCII.GetString(writeValue).TrimEnd('\0');
                        break;                    
                }
            }
            catch (Exception ex) { }

            return result;
        }

        /// <summary>
        /// Convert array data to write (from Movicon) into PLC data (.Net Framework object)
        /// </summary>
        /// <param name="varType"></param>
        /// <param name="writeValue"></param>
        /// <param name="arrayDimension"></param>
        /// <param name="stringLength"></param>
        /// <returns></returns>
        public static object GetObjectFromWriteArratOfValues(VarType varType, byte[] writeValue, uint arrayDimension, uint stringLength)
        {
            object result = null;

            try
            {
                switch (varType)
                {
                    case VarType.BOOL:
                        {
                            var a = new bool[writeValue.Length / sizeof(bool)];
                            Buffer.BlockCopy(writeValue, 0, a, 0, writeValue.Length);
                            result = a.ToList();
                        }
                        break;
                    case VarType.SINT:
                        {
                            var a = new sbyte[writeValue.Length / sizeof(sbyte)];
                            Buffer.BlockCopy(writeValue, 0, a, 0, writeValue.Length);
                            result = a.ToList();
                        }
                        break;
                    case VarType.INT:
                        {
                            var a = new Int16[writeValue.Length / sizeof(Int16)];
                            Buffer.BlockCopy(writeValue, 0, a, 0, writeValue.Length);
                            result = a.ToList();
                        }
                        break;
                    case VarType.DINT:
                        {
                            var a = new Int32[writeValue.Length / sizeof(Int32)];
                            Buffer.BlockCopy(writeValue, 0, a, 0, writeValue.Length);
                            result = a.ToList();
                        }
                        break;
                    case VarType.USINT:
                        {
                            var a = new byte[writeValue.Length / sizeof(byte)];
                            Buffer.BlockCopy(writeValue, 0, a, 0, writeValue.Length);
                            result = a.ToList();
                        }
                        break;
                    case VarType.UINT:
                        {
                            var a = new UInt16[writeValue.Length / sizeof(UInt16)];
                            Buffer.BlockCopy(writeValue, 0, a, 0, writeValue.Length);
                            result = a.ToList();
                        }
                        break;
                    case VarType.UDINT:
                        {
                            var a = new UInt32[writeValue.Length / sizeof(UInt32)];
                            Buffer.BlockCopy(writeValue, 0, a, 0, writeValue.Length);
                            result = a.ToList();
                        }
                        break;
                    case VarType.REAL:
                        {                            
                            var a = new float[writeValue.Length / sizeof(float)];
                            Buffer.BlockCopy(writeValue, 0, a, 0, writeValue.Length);
                            result = a.ToList();
                        }                    
                        break;
                    case VarType.LREAL:
                        {
                            var a = new double[writeValue.Length / sizeof(double)];
                            Buffer.BlockCopy(writeValue, 0, a, 0, writeValue.Length);
                            result = a.ToList();
                        }
                        break;
                    case VarType.TIME:
                        {
                            var a = new UInt32[writeValue.Length / sizeof(UInt32)];
                            Buffer.BlockCopy(writeValue, 0, a, 0, writeValue.Length);
                            result = a.ToList();
                        }
                        break;
                    case VarType.BYTE:
                        {
                            var a = new byte[writeValue.Length / sizeof(byte)];
                            Buffer.BlockCopy(writeValue, 0, a, 0, writeValue.Length);
                            result = a.ToList();
                        }
                        break;
                    case VarType.WORD:
                        {
                            var a = new UInt16[writeValue.Length / sizeof(UInt16)];
                            Buffer.BlockCopy(writeValue, 0, a, 0, writeValue.Length);
                            result = a.ToList();
                        }
                        break;
                    case VarType.DWORD:
                        {
                            var a = new UInt32[writeValue.Length / sizeof(UInt32)];
                            Buffer.BlockCopy(writeValue, 0, a, 0, writeValue.Length);
                            result = a.ToList();
                        }
                        break;
                    case VarType.STRING:
                        {
                            // array of string is an array of bytes --> ArrayDimension * String Length Size
                            List<string> a = new List<string>();
                            for (int i=0; i< arrayDimension; i++)
                            {
                                string data = ASCIIEncoding.ASCII.GetString(writeValue.Skip((int)(i * stringLength)).Take((int)stringLength).ToArray()).Trim('\0');
                                a.Add(data);
                            }
                            result = a;
                        }
                        break;
                }
            }
            catch (Exception ex) { }

            return result;
        }


        /// <summary>
        /// Retrive nr of byte associated to specific data type; for non numeric return 0 --> undefined
        /// </summary>
        /// <param name="varType"></param>
        /// <returns></returns>
        //public static uint GetByteSizeOfVarType(VarType varType)
        //{
        //    switch (varType)
        //    {
        //        case VarType.BOOL:
        //            return 1;
        //        case VarType.SINT:
        //            return 1;
        //        case VarType.INT:
        //            return 2;
        //        case VarType.DINT:
        //            return 4;
        //        case VarType.USINT:
        //            return 1;
        //        case VarType.UINT:
        //            return 2;
        //        case VarType.UDINT:
        //            return 4;
        //        case VarType.REAL:
        //            return 4;
        //        case VarType.LREAL:
        //            return 8;
        //        case VarType.TIME:
        //            return 4;
        //        case VarType.BYTE:
        //            return 1;
        //        case VarType.WORD:
        //            return 2;
        //        case VarType.DWORD:
        //            return 4;
        //        case VarType.STRING:
        //            return 0;            
        //    }
        //    return 0;
        //}               

        public static UFUAModel.DataType GetDataType(VarType dataFormat)
        {
            switch (dataFormat)
            {
                case VarType.BOOL:
                    return UFUAModel.DataType.Boolean;
                case VarType.BYTE:
                    return UFUAModel.DataType.Byte;
                case VarType.TIME:
                    return UFUAModel.DataType.UInt32;
                case VarType.DINT:
                    return UFUAModel.DataType.Int32;
                case VarType.UDINT:
                case VarType.DWORD:
                    return UFUAModel.DataType.UInt32;
                case VarType.INT:
                    return UFUAModel.DataType.Int16;
                case VarType.LREAL:
                    return UFUAModel.DataType.Double;
                case VarType.REAL:
                    return UFUAModel.DataType.Float;
                case VarType.SINT:
                    return UFUAModel.DataType.SByte;
                case VarType.STRING:
                    return UFUAModel.DataType.String;
                case VarType.UINT:
                    return UFUAModel.DataType.UInt16;
                case VarType.USINT:
                    return UFUAModel.DataType.Byte;
                case VarType.WORD:
                    return UFUAModel.DataType.Int16;
                default:
                    return 0;
            }
        }

        public static VarType GetDataFormatFromMoviconDataType(uint typ)
        {
            switch ((BuiltInType)typ)
            {
                case BuiltInType.Boolean:
                    return VarType.BOOL;
                case BuiltInType.Byte:
                    return VarType.BYTE;
                //case BuiltInType.UInt32:
                //    return VarType.TIME;
                case BuiltInType.Int32:
                    return VarType.DINT;
                case BuiltInType.UInt32:
                //case VarType.DWORD:
                    return VarType.UDINT;
                case BuiltInType.Int16:
                    return VarType.INT;
                case BuiltInType.Double:
                    return VarType.LREAL;
                case BuiltInType.Float:
                    return VarType.REAL;
                case BuiltInType.SByte:
                    return VarType.SINT;
                case BuiltInType.String:
                    return VarType.STRING;
                case BuiltInType.UInt16:
                    return VarType.UINT;
                //case VarType.USINT:
                //    return UFUAModel.DataType.Byte;
                //case VarType.WORD:
                //    return UFUAModel.DataType.Int16;
                default:
                    return 0;
            }
        }

        public static VarType GetVarType(string stringType)
        {
            VarType ResulType = VarType.UNKNOWN;

            switch (stringType.ToUpper().Trim())
            {
                case "BOOL":
                case "BOOLEAN":
                    ResulType = VarType.BOOL;
                    break;
                case "SINT":
                case "SBYTE":
                    ResulType = VarType.SINT;
                    break;
                case "INT":
                case "INT16":
                    ResulType = VarType.INT;
                    break;
                case "DINT":
                case "INT32":
                    ResulType = VarType.DINT;
                    break;
                case "USINT":
                    ResulType = VarType.USINT;
                    break;
                case "UINT":
                case "UINT16":
                    ResulType = VarType.UINT;
                    break;
                case "UDINT":
                case "UINT32":
                    ResulType = VarType.UDINT;
                    break;
                case "REAL":
                case "SINGLE":
                    ResulType = VarType.REAL;
                    break;
                case "LREAL":
                case "DOUBLE":
                    ResulType = VarType.LREAL;
                    break;
                case "TIME":
                    ResulType = VarType.TIME;
                    break;
                case "BYTE":
                    ResulType = VarType.BYTE;
                    break;
                case "WORD":
                    ResulType = VarType.WORD;
                    break;
                case "DWORD":
                    ResulType = VarType.DWORD;
                    break;
                case "STRING":
                case "IECSTRING80":
                    ResulType = VarType.STRING;
                    break;
                case "STRUCT":
                    ResulType = VarType.STRUCT;
                    break;                  
            }

            return ResulType;
        }

        public static string GetVarTypeByNumber(uint Type)
        {
            string ResulType = string.Empty;

            switch (Type)
            {
                case 2:
                    ResulType = "Boolean";
                    break;

                case 4:
                    ResulType = "SByte";
                    break;

                case 5:
                    ResulType = "Byte";
                    break;

                case 6:
                    ResulType = "Int16";
                    break;

                case 3:
                case 7:
                    ResulType = "UInt16";
                    break;

                case 8:
                    ResulType = "Int32";
                    break;

                case 9:
                    ResulType = "UInt32";
                    break;

                case 12:
                    ResulType = "Single";
                    break;

                case 13:
                    ResulType = "Double";
                    break;
                
                case 14:
                    ResulType = "String";
                    break;

                case 17:
                    ResulType = "Struct";
                    break;
            }

            return ResulType;
        }

        #region String Function
        public static bool IsPlcStructureType(string varType)
        {
            return (PhoenixContactPLCIProtocol.GetVarType(varType) == PhoenixContactPLCIProtocol.VarType.UNKNOWN || PhoenixContactPLCIProtocol.GetVarType(varType) == PhoenixContactPLCIProtocol.VarType.STRUCT);
        }

        #endregion        

        public static bool CheckIfTagCanBeInsertTheReadList(String szAddress, int nTotalJobSize, ref int nCurrentMessageSize, ref int nCurrentMessageByteSize)
        {
            bool bReturn = false;
            int nFindFirstPoint = 0;
            int nStringSize = 0;
            nFindFirstPoint = szAddress.IndexOf(".") + 1;
            nStringSize = szAddress.Length - nFindFirstPoint;

            //into the protocol 12 byte are usefully for identifier the tag: two byte before name and two after name, and 8 date type, but 
            // if name contain an odd number of bytes, the two byte after name becomes one byte.
            int nIdentifierTag = 12;
            if (nStringSize % 2 != 0)
            {
                nIdentifierTag = 11;
            }

            if ((nCurrentMessageSize + nStringSize + nIdentifierTag) < PhoenixContactPLCIProtocol.MAX_REQUEST_MESSAGE_SIZE)
            {
                nCurrentMessageSize += (nStringSize + nIdentifierTag);
                bReturn = true;
            }
            else
            {
                return false;
            }

            //now checked the size in the protocol insert 4 byte before the value 
            if ((nCurrentMessageByteSize + nTotalJobSize + 4) <= PhoenixContactPLCIProtocol.MAX_SUBSCRIPTIONS_REQUEST_MESSAGE_SIZE)
            {
                nCurrentMessageByteSize += (nTotalJobSize + 4);
                bReturn = true;
            }
            else
            {
                bReturn = false;
            }

            return (bReturn);
        }

        public static bool IfTagSizeTooBigForSubScription(uint nTotalJobSize)
        {
            return (nTotalJobSize >= MAX_SUBSCRIPTIONS_REQUEST_MESSAGE_SIZE);
        }

        public static bool CheckIfTagCanBeInsertTheSubScriptionList(uint nTotalJobSize, ref uint nCurrentMessageByteSize)
        {
            bool bReturn = false;
            //into he protocol 8 are: two byte before name and two after name end 8 byte type date 
            if ((nCurrentMessageByteSize + nTotalJobSize + 4) <= MAX_SUBSCRIPTIONS_REQUEST_MESSAGE_SIZE)
            {
                nCurrentMessageByteSize += (nTotalJobSize + 4);
                bReturn = true;
            }
            return bReturn;
        }


        public static bool ParseData(byte[] receivebuffer, ref PhoenixContactPLCICommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);
            if (!areArguments && (receivebuffer == null))
            {
                return (false);
            }
            else if (receivebuffer == null)
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

            byte[] tempBuffer = new byte[receivebuffer.Length];

            Array.Copy(receivebuffer, tempBuffer, receivebuffer.Length);

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

        public static string GetNodeTree(NodeId NodeId, string Name)
        {
            string Out = "";

            //System.Text.RegularExpressions.Regex NameParser = new System.Text.RegularExpressions.Regex(@"^[\d]+:(?<Name>[\w]+)$");
            System.Text.RegularExpressions.Regex NameParser = new System.Text.RegularExpressions.Regex(@"[\d]+:(?<Name>[\w]+)$");
            System.Text.RegularExpressions.Match NameMatch = NameParser.Match(Name);
            if (NameMatch.Success)
            {
                Out = NameMatch.Groups["Name"].Value;
                string nameNodeId = NodeId.Identifier.ToString();
                System.Text.RegularExpressions.Regex NodeParser = new System.Text.RegularExpressions.Regex(@"^[^?]+[?](?<Node>[\w/]+)/[\w-]+$");
                System.Text.RegularExpressions.Match NodeMatch = NodeParser.Match(nameNodeId);
                if (NodeMatch.Success)
                {
                    Out = NodeMatch.Groups["Node"].Value + "." + Out;
                }
            }
            return Out;

        }

        public static bool IsValidStringSize(uint size)
        {
            return (size <= 0 || 4096 > size);
        }
    }
}
