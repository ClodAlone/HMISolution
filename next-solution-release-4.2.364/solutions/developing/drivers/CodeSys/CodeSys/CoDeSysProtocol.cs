using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using Opc.Ua;
using DriverCodeBase;
using DriverCodeBase.Enumerators;


namespace CoDeSys
{    
    public class CoDeSysProtocol
    {
        public const uint MAX_DATA_BYTES = 0x10000;
        public const uint MAX_AGGREGATED_JOBS = 50;

        public const string GENERIC_STRUCT= "*STRUCT*";

        public const int PLCHANDLERERRORSSHIFTOFFSET = 5000;
        public enum PLCHandlerErrors : int
        {
            RESULT_FAILED = -1,
            RESULT_OK = 0,
            RESULT_PLC_NOT_CONNECTED = 1,
            RESULT_PLC_LOGIN_FAILED = 2,
            RESULT_PLC_NO_CYCLIC_LIST_DEFINED = 3,
            RESULT_PLCHANDLER_INACTIVE = 4,
            RESULT_LOADING_SYMBOLS_FAILED = 5,
            RESULT_ITF_NOT_SUPPORTED = 6,
            RESULT_COMM_FATAL = 7,
            RESULT_NO_CONFIGURATION = 8,
            RESULT_INVALID_PARAMETER = 9,
            RESULT_ITF_FAILED = 10,
            RESULT_NOT_SUPPORTED = 11,
            RESULT_EXCEPTION = 12,
            RESULT_TIMEOUT = 13,
            RESULT_STILL_CONNECTED = 14,
            RESULT_RECONNECTTHREAD_STILL_ACTIVE = 15,
            RESULT_PLC_NOT_CONNECTED_SYMBOLS_LOADED = 16,
            RESULT_NO_UPDATE = 17,
            RESULT_OCX_CONVERSION_FAILED = 18,
            RESULT_TARGETID_MISMATCH = 19,
            RESULT_NO_OBJECT = 20,
            RESULT_COMPONENTS_NOT_LOADED = 21,
            RESULT_BUSY = 22,
            RESULT_DISABLED = 23,
            RESULT_PLC_FAILED = 24,
            RESULT_INVALID_SYMBOL = 25,
            RESULT_BUFFER_TOO_SMALL = 26,
            RESULT_NO_PROJECT = 27,
            RESULT_FILE_ERROR = 28,
            RESULT_RETAIN_MISMATCH = 29,
            RESULT_NO_ACCESS_RIGHTS = 30,
            RESULT_DUPLICATE_PLC_NAME = 31,
            RESULT_SIZE_MISMATCH = 32,
            RESULT_LIST_NO_WRITE_ACCESS = 33,
            RESULT_CONSISTENT_ACCESS_TIMEOUT = 34,
            RESULT_SYNC_CONSISTENT_ACCESS_DENIED = 35,
            RESULT_INVALID_ASCII_STRING = 36,
            RESULT_INVALID_STRING_LENGTH = 37,
            RESULT_OUTOFMEMORY = 38,
            RESULT_NO_FILE = 39,
            RESULT_APPLICATION_NOT_IN_STOP = 40,
            RESULT_APPLICATION_NOT_IN_RUN = 41,
            RESULT_OPERATIONMODE_NOT_IN_DEBUG = 42,
            RESULT_BACKUP_RESTORE_NOT_SUPPORTED = 43,
            RESULT_PLC_INCONSISTENT_STATE = 44,
            RESULT_PLC_INCOMPATIBLE = 45,
            RESULT_PLC_VERSION_INCOMPATIBLE = 46,
            RESULT_RETAIN_ERROR = 47,
            RESULT_APPLICATIONS_LOAD_ERROR = 48,
            RESULT_APPLICATIONS_START_ERROR = 49,
            RESULT_FILETRANSFER_ERROR = 50,
            RESULT_OPERATION_DENIED = 51,
            RESULT_FORCES_ACTIVE = 52,
            RESULT_META_VERSION_MISMATCH = 53,

            //defined for internal use of driver
            RESULT_UNMAPPED_STATION	= 999,
            RESULT_STATION_ID_INVALID = 998,
            RESULT_READVALUE_UNMAPPED_VAR = 997,
        }
               
        public enum PLCHandlerState
        {
            STATE_TERMINATE = -1,
            STATE_PLC_NOT_CONNECTED = 0,
            STATE_PLC_CONNECTED,
            STATE_NO_SYMBOLS,
            STATE_SYMBOLS_LOADED,
            STATE_RUNNING,
            STATE_DISCONNECT,
            STATE_NO_CONFIGURATION,
            STATE_PLC_NOT_CONNECTED_SYMBOLS_LOADED
        };

        /* CoDeSys Version */
        public enum PlcVersion 
        {
            //for now not supported
            //IDS_PLC_VER_23 = 507, 
            IDS_PLC_VER_30 = 508,
        }

        public const int DEFAULT_PORT = 1217;
        public const uint PLCHANDLER_USE_DEFAULT = 0;

        public enum Protocol {
            IDS_PROTOCOL_L2_ROUTE = 635,
            IDS_PROTOCOL_L2 = 636,
            IDS_PROTOCOL_L4 = 637,
        }
       
        public enum VarType : uint
        {
            VAR_TYPE_BOOL = 0,
            VAR_TYPE_BYTE = 3,
            VAR_TYPE_DATE = 15,
            //VAR_TYPE_DATE_AND_TIME = 17,
            VAR_TYPE_DINT = 7,
            //VAR_TYPE_DT = 17, 
            VAR_TYPE_DWORD = 9,
            VAR_TYPE_INT = 4,
            VAR_TYPE_LINT = 20,
            VAR_TYPE_LREAL = 11,
            //VAR_TYPE_LTIME = 22,
            VAR_TYPE_LWORD = 24,
            VAR_TYPE_REAL = 10,
            VAR_TYPE_SINT = 1,
            VAR_TYPE_STRING = 13,
            VAR_TYPE_TIME = 12,
            VAR_TYPE_TIME_OF_DAY = 16,
            //VAR_TYPE_TOD = 16,
            VAR_TYPE_UDINT = 8,
            VAR_TYPE_UINT = 5,
            VAR_TYPE_ULINT = 21,
            VAR_TYPE_USINT = 2,
            VAR_TYPE_WORD = 6,
            VAR_TYPE_WSTRING = 23,
            // used in or with real data type
            VAR_TYPE_ARRAY = 0x20000,
            VAR_TYPE_STRUCT,

            VAR_TYPE_E_UNKNOWN = 0x80040200
        }

        /// <summary>
        /// Retrive nr of byte associated to specific data type; for non numeric return 0 --> undefined
        /// </summary>
        /// <param name="varType"></param>
        /// <returns></returns>
        public static uint GetByteSizeOfVarType(VarType varType)
        {
            switch (varType)
            {
                case VarType.VAR_TYPE_BOOL:
                    return 1;
                case VarType.VAR_TYPE_BYTE:
                    return 1;
                case VarType.VAR_TYPE_DATE:
                    return 4;
                //case VarType.VAR_TYPE_DATE_AND_TIME:
                //    return 4;
                case VarType.VAR_TYPE_DINT:
                    return 4;
                //case VarType.VAR_TYPE_DT:
                //    return 4;
                case VarType.VAR_TYPE_DWORD:
                    return 4;
                case VarType.VAR_TYPE_INT:
                    return 2;
                case VarType.VAR_TYPE_LINT:
                    return 8;
                case VarType.VAR_TYPE_LREAL:
                    return 8;
                //case VarType.VAR_TYPE_LTIME:
                //    return 8;
                case VarType.VAR_TYPE_LWORD:
                    return 8;
                case VarType.VAR_TYPE_REAL:
                    return 4;
                case VarType.VAR_TYPE_SINT:
                    return 1;
                case VarType.VAR_TYPE_STRING:
                    return 256;
                case VarType.VAR_TYPE_TIME:
                    return 4;
                case VarType.VAR_TYPE_TIME_OF_DAY:
                    return 4;
                //case VarType.VAR_TYPE_TOD:
                //    return 4;
                case VarType.VAR_TYPE_UDINT:
                    return 4;
                case VarType.VAR_TYPE_UINT:
                    return 2;
                case VarType.VAR_TYPE_ULINT:
                    return 8;
                case VarType.VAR_TYPE_USINT:
                    return 1;
                case VarType.VAR_TYPE_WORD:
                    return 2;
                case VarType.VAR_TYPE_WSTRING:
                    return 162;
                //case CoDeSysProtocol.VarType.VAR_TYPE_ARRAY,
                //case CoDeSysProtocol.VarType.VAR_TYPE_STRUCT,
            }
            return 0;
        }

        //public static VarType GetDataTypeFromUAMode(string dataFormat)
        //{
        //    VarType Result = VarType.VAR_TYPE_E_UNKNOWN;
        //    int Value;
        //    if (int.TryParse(dataFormat, out Value))
        //        Result = GetDataTypeFromUAMode((UFUAModel.DataType)Value);

        //   return Result;
        //}
               

        public static UFUAModel.DataType GetDataType(VarType dataFormat)
        {
            switch (dataFormat)
            {
                case VarType.VAR_TYPE_BOOL:
                    return UFUAModel.DataType.Boolean;
                case VarType.VAR_TYPE_BYTE:
                    return UFUAModel.DataType.Byte;
                case VarType.VAR_TYPE_DATE:
                    return UFUAModel.DataType.UInt32;
                //case VarType.VAR_TYPE_DATE_AND_TIME:
                //    return 4;
                case VarType.VAR_TYPE_DINT:
                    return UFUAModel.DataType.Int32;
                //case VarType.VAR_TYPE_DT:
                //    return 4;
                case VarType.VAR_TYPE_DWORD:
                    return UFUAModel.DataType.UInt32;
                case VarType.VAR_TYPE_INT:
                    return UFUAModel.DataType.Int16;
                case VarType.VAR_TYPE_LINT:
                    return UFUAModel.DataType.Int64;
                case VarType.VAR_TYPE_LREAL:
                    return UFUAModel.DataType.Double;
                //case VarType.VAR_TYPE_LTIME:
                //    return 8;
                case VarType.VAR_TYPE_LWORD:
                    return UFUAModel.DataType.UInt64;
                case VarType.VAR_TYPE_REAL:
                    return UFUAModel.DataType.Float;
                case VarType.VAR_TYPE_SINT:
                    return UFUAModel.DataType.SByte;
                case VarType.VAR_TYPE_STRING:
                    return UFUAModel.DataType.String;
                case VarType.VAR_TYPE_TIME:
                    return UFUAModel.DataType.UInt32;
                case VarType.VAR_TYPE_TIME_OF_DAY:
                    return UFUAModel.DataType.UInt32;
                //case VarType.VAR_TYPE_TOD:
                //    return 4;
                case VarType.VAR_TYPE_UDINT:
                    return UFUAModel.DataType.UInt32;
                case VarType.VAR_TYPE_UINT:
                    return UFUAModel.DataType.UInt16;
                case VarType.VAR_TYPE_ULINT:
                    return UFUAModel.DataType.UInt64;
                case VarType.VAR_TYPE_USINT:
                    return UFUAModel.DataType.Byte;
                case VarType.VAR_TYPE_WORD:
                    return UFUAModel.DataType.UInt16;
                //case VarType.VAR_TYPE_WSTRING:
                //    return 162;
                default:
                    return 0;
            }
        }
               
        public static int GetVarTypeAndSize(ref string stringType, out uint varSize)
        {            
            VarType V = GetVarType(stringType);
            if (V== VarType.VAR_TYPE_E_UNKNOWN)
            {
                varSize = 0;
                return -1;
            } else
            {
                varSize = CoDeSysProtocol.GetByteSizeOfVarType(V);
                return (int)GetDataType(V);
            }
        }

        public static VarType GetVarType(string stringType)
        {
            VarType ResulType = VarType.VAR_TYPE_E_UNKNOWN;
            
            switch (stringType.ToUpper().Trim())
            {
                case "BOOL":
                    ResulType = VarType.VAR_TYPE_BOOL;
                    break;
                case "BYTE":
                    ResulType = VarType.VAR_TYPE_BYTE;
                    break;
                case "DINT":
                    ResulType = VarType.VAR_TYPE_DINT;
                    break;
                case "DWORD":
                    ResulType = VarType.VAR_TYPE_DWORD;
                    break;
                case "INT":
                    ResulType = VarType.VAR_TYPE_INT;
                    break;
                case "LINT":
                    ResulType = VarType.VAR_TYPE_LINT;
                    break;
                case "LREAL":
                    ResulType = VarType.VAR_TYPE_LREAL;
                    break;
                case "LWORD":
                    ResulType = VarType.VAR_TYPE_LWORD;
                    break;
                case "REAL":
                    ResulType = VarType.VAR_TYPE_REAL;
                    break;
                case "SINT":
                    ResulType = VarType.VAR_TYPE_SINT;
                    break;
                case "STRING":
                    ResulType = VarType.VAR_TYPE_STRING;
                    break;
                case "WSTRING":
                    ResulType = VarType.VAR_TYPE_STRING;
                    break;
                case "UDINT":
                    ResulType = VarType.VAR_TYPE_UDINT;
                    break;
                case "UINT":
                    ResulType = VarType.VAR_TYPE_UINT;
                    break;
                case "ULINT":
                    ResulType = VarType.VAR_TYPE_ULINT;
                    break;
                case "USINT":
                    ResulType = VarType.VAR_TYPE_USINT;
                    break;
                case "WORD":
                    ResulType = VarType.VAR_TYPE_WORD;
                    break;
                case "DATE":
                    ResulType = VarType.VAR_TYPE_DATE;
                    break;
                case "TIME":
                    ResulType = VarType.VAR_TYPE_TIME;
                    break;
                case "TOD": // PLC
                case "TIME_OF_DAY": // FILE
                    ResulType = VarType.VAR_TYPE_TIME_OF_DAY;
                    break;
                case GENERIC_STRUCT:
                    ResulType = VarType.VAR_TYPE_STRUCT;
                    break;
            }

            return ResulType;
        }

        #region String Function

        public static bool IsStringType(string varType)
        {
            return IsStringType(GetVarType(varType));
        }
        public static bool IsStringType(VarType varType)
        {
            return (varType == VarType.VAR_TYPE_STRING || varType == VarType.VAR_TYPE_WSTRING);
        }

        public static bool IsStandardDataType(string dataType)
        {
            return (CoDeSysProtocol.GetVarType(dataType) != CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN && CoDeSysProtocol.GetVarType(dataType) != CoDeSysProtocol.VarType.VAR_TYPE_STRUCT);
        }

        public static bool IsPlcStructureType(string varType)
        {
            return (CoDeSysProtocol.GetVarType(varType) == CoDeSysProtocol.VarType.VAR_TYPE_E_UNKNOWN || CoDeSysProtocol.GetVarType(varType) == CoDeSysProtocol.VarType.VAR_TYPE_STRUCT);
        }

        public static bool IsArrayOfStandardType(uint ulTypeId)
        {
            return ((ulTypeId & ((uint)CoDeSysProtocol.VarType.VAR_TYPE_ARRAY)) == (uint)CoDeSysProtocol.VarType.VAR_TYPE_ARRAY);
            //{
            //    ulTypeId ^= (uint)(CoDeSysProtocol.VarType.VAR_TYPE_ARRAY);
            //}
        }
        

        /// <summary>
        /// Convert array of byte into string (STRING, WSTRING); search the end of string ('\0' or '\0\0') and the converto string (ASCII, UFT8)
        /// </summary>
        /// <param name="data"></param>
        /// <param name="varType"></param>
        /// <returns></returns>
        public static byte[] StringConverterFromByteData(VarType varType, byte[] data)
        {
            int Index = 0;
            byte[] DataResult = null;
            byte[] Result = null;

            switch (varType)
            {
                case VarType.VAR_TYPE_STRING:
                    Index = Array.IndexOf(data, (byte)0);
                    if (Index >= 0)
                    {
                        DataResult = new byte[Index];
                        Array.Copy(data, DataResult, Index);
                    }

                    if (DataResult == null)
                        DataResult = data;

                    Result = Encoding.ASCII.GetBytes(Encoding.ASCII.GetString(DataResult));
                    break;
                
                case VarType.VAR_TYPE_WSTRING:
                    do
                    {
                        Index = Array.IndexOf(data, (byte)0, Index);
                        if (Index >= 0)
                        {
                            if (Index % 2 == 0)
                            {
                                if (Index + 1 < data.Length)
                                {
                                    if (data[Index + 1] == (byte)0)
                                    {
                                        DataResult = new byte[Index];
                                        Array.Copy(data, DataResult, Index);
                                        break;
                                    }
                                    Index++;
                                }
                            }
                            else
                            {
                                Index++;
                            }
                        }
                    } while (!(Index < 0));

                    if (DataResult == null)
                        DataResult = data;
                    Result = Encoding.UTF8.GetBytes(Encoding.Unicode.GetString(DataResult));
                    break;
            }

            return Result;
        }

        /// <summary>
        /// Convert array of byte into string (STRING, WSTRING); search the end of string ('\0' or '\0\0') and the converto string (ASCII, UFT8)
        /// varSize = 0 alla characters was accepted
        /// </summary>
        /// <returns></returns>
        public static byte[] StringConverterToData(VarType varType, uint varSize,byte[] data)
        {
            byte[] DataResult = null;
            uint NrBytes = 0;

            switch (varType)
            {
                case VarType.VAR_TYPE_STRING:
                    if (varSize != 0 && data.Length > varSize - 1)
                        NrBytes = varSize - 1;
                    else
                        NrBytes = (uint)data.Length;
                    // add space for char 0 (end of string)
                    DataResult = new byte[NrBytes + 1];
                    Array.Copy(data, DataResult, NrBytes);
                    break;

                case VarType.VAR_TYPE_WSTRING:
                    if (varSize != 0 && data.Length > varSize - 2)
                        NrBytes = varSize - 2;
                    else
                        NrBytes = (uint)data.Length;
                    // add space for doube (Unicode) char 0 (end of string)
                    DataResult = new byte[NrBytes + 2];
                    // string from movicon are UFT8
                    DataResult = Encoding.Unicode.GetBytes(Encoding.UTF8.GetString(data));
                    break;
            }

            return DataResult;
        }

        public static uint StringConverterCalculateSizeFromItem(VarType varType, uint ulSize)
        {            
            switch (varType)
            {
                case VarType.VAR_TYPE_STRING:
                    return ulSize;

                case VarType.VAR_TYPE_WSTRING:
                    return ulSize/2;

                default:
                    return 0;
            }
        }

        public static int ShiftError(PLCHandlerErrors error)
        {
            return (int)(error) + PLCHANDLERERRORSSHIFTOFFSET;
        }

        public static int UnShiftErrorCode(int error)
        {
            return UnShiftErrorCode((PLCHandlerErrors)error);
        }

        public static int UnShiftErrorCode(PLCHandlerErrors error)
        {
            // - 1 to manage FAILED error
            if ((int)error == (PLCHANDLERERRORSSHIFTOFFSET - 1))
                return -1;
            else if ((int)error > PLCHANDLERERRORSSHIFTOFFSET)
                return (int)(error) - PLCHANDLERERRORSSHIFTOFFSET;
            else
                return (int)(error);
        }

        #endregion


        /// <summary>
        /// Check if Element Number can be applied to device data type
        /// </summary>
        /// <param name="job"></param>
        /// <returns></returns>
        public static bool IsValidElementNumber(CoDeSysCommJob job)
        {
            if (job.ElementNumber > 0) {
                if (job.ProtocolDataSizeBig())
                {
                    if (job.ElementNumber > (job.GetProtocolDataBitSize() / CommJob.GetDataTypeBitSize((uint)job.TagsList[0].TagNode.DataType.Identifier) - 1))
                        return false;
                }
            }

            return true;
        }

        public static bool ParseData(byte[] receivebuffer, ref CoDeSysCommJob job, ref List<object> items)
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

            // check if element number is correct --> this operation can be perform only online base device data type is retrive by driver on fly
            if (!IsValidElementNumber(job)) { 
                changed = null;
                return false; 
            }
            else
            {
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
    }
}
