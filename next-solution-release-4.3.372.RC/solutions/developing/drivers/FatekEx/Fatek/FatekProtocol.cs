using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using DriverCodeBaseEx.Enumerators;
using System.Text.RegularExpressions;
using System.Text;

namespace Fatek
{
    public class FatekProtocol
    {
        #region const
        public const int FATEK_ERRORCODE_OFFSET = 1000;

        private const byte FATEK_ERRORNOERROR = 48; // '0' char in ascii table

        public enum FatekErrorCodes : int
        {
            //plc internal error
            ErrorNoError = FATEK_ERRORCODE_OFFSET,
            ErrorIllegalValue = FATEK_ERRORCODE_OFFSET + 2,
            ErrorIllegalFormat = FATEK_ERRORCODE_OFFSET + 4,
            ErrorCanNotRunLadderChecksumErrorWhenRunPLC = FATEK_ERRORCODE_OFFSET + 5,
            ErrorCanNotRunPLCIDLadderIDwhenRunPLC = FATEK_ERRORCODE_OFFSET + 6,
            ErrorCanNotRunSnytaxCheckErrorWhenRunPLC = FATEK_ERRORCODE_OFFSET + 7,
            ErrorCanNotRunFunctionNotSupported = FATEK_ERRORCODE_OFFSET + 9,
            ErrorIllegalAddress = FATEK_ERRORCODE_OFFSET + 0xA,
            //plc internal error

            // driver error
            ErrorReceiveFrameError = FATEK_ERRORCODE_OFFSET + 100,
            ErrorStationIdInvalid = FATEK_ERRORCODE_OFFSET + 101,
            ErrorCRCInvalid = FATEK_ERRORCODE_OFFSET + 102,
            ErrorGeneric = FATEK_ERRORCODE_OFFSET + 103,
            // driver error
        }

        public enum DataArea : int
        {
            Invalid = -1,
            X,
            Y,
            M,
            S,
            T,
            C,
            TMR,
            CTR,
            HR,
            DR,
            FR,
        }

        private const byte STX = 2;
        private const byte ETX = 3;

        public const int PROTOCOL_DEFAULT_TCP_PORT = 500;
        public const int PROTOCOL_FRAME_AREAD_ADDRESS_SIZE = 7;
        public const int PROTOCOL_FRAME_MAX_SIZE = PROTOCOL_ANSWER_MIN_SIZE + PROTOCOL_FRAME_AREAD_ADDRESS_SIZE + (255  * 4); // + valure address + data area
        public const int PROTOCOL_ANSWER_HEADER_SIZE = 6; // STX (1byte) + Station ID (2 bytes) + Command ID (2 bytes) + Error code (1 byte)
        public const int PROTOCOL_ANSWER_FOOTER_SIZE = 3; // CRC (2 byte) + STX (1 byte)
        public const int PROTOCOL_ANSWER_MIN_SIZE = PROTOCOL_ANSWER_HEADER_SIZE + PROTOCOL_ANSWER_FOOTER_SIZE;

        private const string PROTOCOL_READ_DISCRETE_COMMAND = "44";
        private const string PROTOCOL_WRITE_DISCRETE_COMMAND = "45";
        private const string PROTOCOL_READ_REGISTER_COMMAND = "46";
        private const string PROTOCOL_WRITE_REGISTER_COMMAND = "47";

        private const byte PROTOCOL_DESCRETE_VALUE_OFF = 48;  // '0'
        private const byte PROTOCOL_DESCRETE_VALUE_ON = 49;   // '1'

        // costant used internally to create an invalid Dynamic link address during Structure splitting --> is the only to set quality BasErrorConfig for an invalid structur's member
        public const string STRUCT_MEMBER_INVALID = "***STRUCT_MEMBER_INVALID***";

        public const string TEST_COMM_DYNAMIC_SETTINGS = "Fatek.Station={0}|LinkType=1|SA=R0";
        #endregion

        #region methods
        public static bool SplitStartAddress(string startAddress, out DataArea area, out ushort areaAddress, out string errorCode)
        {
            errorCode = string.Empty;
            area = DataArea.Invalid;
            areaAddress = 0;

            if (string.IsNullOrEmpty(startAddress))
            {
                errorCode = Properties.Resources.ErrorStartAddressEmpty;
                return false;
            }

            // split adress into 2 part : <data area> <area address>
            var r = new Regex(@"^(\D*)(\d*)", RegexOptions.IgnoreCase);

            var match = r.Match(startAddress);
            if (match.Groups.Count !=3)
            {
                errorCode = Properties.Resources.ErrorStartAddressInvalid;
                return false;
            }

            // validate data area
            if (!GetDataArea(match.Groups[1].ToString(), out area)) {
                errorCode = Properties.Resources.ErrorStartAddressDataAreaInvalid;
                return false;
            }

            // validate address/offset
            if (!GetDataAreaAddress(match.Groups[2].ToString(), area, out areaAddress))
            {
                area = DataArea.Invalid;
                errorCode = Properties.Resources.ErrorStartAddressInvalid;
                return false;
            }

            return true;
        }

        public static bool IsBitDataArea(DataArea area)
        {
            return (area == DataArea.Y || area == DataArea.X || area == DataArea.M || area == DataArea.S || area == DataArea.T || area == DataArea.C);
        }

        private static bool GetDataArea(string symbol, out DataArea area)
        {
            switch (symbol)
            {
                case "X":
                    area = DataArea.X;
                    return true;
                case "Y":
                    area= DataArea.Y;
                    return true;
                case "M":
                    area = DataArea.M;
                    return true;
                case "S":
                    area = DataArea.S;
                    return true;
                case "T":
                    area = DataArea.T;
                    return true;
                case "C":
                    area = DataArea.C;
                    return true;
                case "RT":
                    area = DataArea.TMR;
                    return true;
                case "RC":
                    area = DataArea.CTR;
                    return true;
                case "R":
                    area = DataArea.HR;
                    return true;
                case "D":
                    area = DataArea.DR;
                    return true;
                case "F":
                    area = DataArea.FR;
                    return true;
                default:
                    area = DataArea.Invalid;
                    return false;
            }
        }

        private static bool GetDataAreaAddress(string text, DataArea area, out ushort areaAddress)
        {
            areaAddress = 0;

            if (!ushort.TryParse(text, out areaAddress))
                return false;

            switch (area)
            {
                case DataArea.X:
                    return (areaAddress >= Properties.Settings.Default.AreaXMinRange && areaAddress <= Properties.Settings.Default.AreaXMaxRange);
                case DataArea.Y:
                    return (areaAddress >= Properties.Settings.Default.AreaYMinRange && areaAddress <= Properties.Settings.Default.AreaYMaxRange);
                case DataArea.M:
                    return (areaAddress >= Properties.Settings.Default.AreaMMinRange && areaAddress <= Properties.Settings.Default.AreaMMaxRange);
                case DataArea.S:
                    return (areaAddress >= Properties.Settings.Default.AreaSMinRange && areaAddress <= Properties.Settings.Default.AreaSMaxRange);
                case DataArea.T:
                    return (areaAddress >= Properties.Settings.Default.AreaTMinRange && areaAddress <= Properties.Settings.Default.AreaTMaxRange);
                case DataArea.C:
                    return (areaAddress >= Properties.Settings.Default.AreaCMinRange && areaAddress <= Properties.Settings.Default.AreaCMaxRange);
                case DataArea.TMR:
                    return (areaAddress >= Properties.Settings.Default.AreaTMRMinRange && areaAddress <= Properties.Settings.Default.AreaTMRMaxRange);
                case DataArea.CTR:
                    return (areaAddress >= Properties.Settings.Default.AreaCTRMinRange && areaAddress <= Properties.Settings.Default.AreaCTRMaxRange);
                case DataArea.HR:
                    return (areaAddress >= Properties.Settings.Default.AreaHRMinRange && areaAddress <= Properties.Settings.Default.AreaHRMaxRange);
                case DataArea.DR:
                    return (areaAddress >= Properties.Settings.Default.AreaDRMinRange && areaAddress <= Properties.Settings.Default.AreaDRMaxRange);
                case DataArea.FR:
                    return (areaAddress >= Properties.Settings.Default.AreaFRMinRange && areaAddress <= Properties.Settings.Default.AreaFRMaxRange);
                default:
                    return false;
            }
        }

        public static string GetStartAddressFormatted(FatekProtocol.DataArea area, ushort areaAddress)//, bool exchangeDiscreteAsRegister = false)
        {            
            ushort addressLenght = 0;
            string symbol = string.Empty;

            switch (area)
            {
                case DataArea.X:
                    symbol = "X";
                    addressLenght = 5;
                    break;
                case DataArea.Y:
                    symbol = "Y";
                    addressLenght = 5;
                    break;
                case DataArea.M:
                    symbol = "M";
                    addressLenght = 5;
                    break;
                case DataArea.S:
                    symbol = "S";
                    addressLenght = 5;
                    break;
                case DataArea.T:
                    symbol = "T";
                    addressLenght = 5;
                    break;
                case DataArea.C:
                    symbol = "C";
                    addressLenght = 5;
                    break;
                case DataArea.TMR:
                    symbol = "RT";                    
                    addressLenght = 6;
                    break;
                case DataArea.CTR:
                    symbol = "RC";
                    addressLenght = 6;
                    break;
                case DataArea.HR:
                    symbol = "R";
                    addressLenght = 6;
                    break;
                case DataArea.DR:
                    symbol = "D";
                    addressLenght = 6;
                    break;
                case DataArea.FR:
                    symbol = "F";                    
                    addressLenght = 6;
                    break;
                default:
                    symbol = string.Empty;
                    addressLenght = 0;
                    break;
            }

            //if (exchangeDiscreteAsRegister)
            //{
            //    symbol = "W" + symbol;
            //    addressLenght++;
            //}

            return string.Format("{0}{1}", symbol, areaAddress.ToString(new string('0', addressLenght - symbol.Length)));
        }

        private static string FormatValueToWriteIntoDiscrete(FatekCommJob job, byte data)
        {
            if (data == 0)
                return Encoding.ASCII.GetString(new byte[] { FatekProtocol.PROTOCOL_DESCRETE_VALUE_OFF });
            else
                return Encoding.ASCII.GetString(new byte[] { FatekProtocol.PROTOCOL_DESCRETE_VALUE_ON });
        }

        private static string FormatValueToWriteIntoRegister(Tag tag, byte[] data, uint startPosition, int ElementNumber, ref uint tagItem)
        {
            string w1 = string.Empty;
            string w2 = string.Empty;
            switch (tag.TagNode.DataType.Identifier)
            {
                case Opc.Ua.DataTypes.Boolean:
                case Opc.Ua.DataTypes.Byte:
                case Opc.Ua.DataTypes.SByte:
                    //return string.Format("{0:X4}", data[startPosition]);
                case Opc.Ua.DataTypes.UInt16:
                case Opc.Ua.DataTypes.Int16:
                    {
                        tagItem = 1;
                        return string.Format("{0:X4}", BitConverter.ToInt16(data.Skip((int)startPosition).Take(2).ToArray(), 0));
                    }
                case Opc.Ua.DataTypes.Int32:
                case Opc.Ua.DataTypes.UInt32:
                    tagItem = 2;
                    w1 = string.Format("{0:X4}", BitConverter.ToInt16(data.Skip((int)startPosition).Take(2).ToArray(), 0));
                    w2 = string.Format("{0:X4}", BitConverter.ToInt16(data.Skip((int)startPosition + 2).Take(2).ToArray(), 0));
                    return string.Format("{0}{1}", w1, w2);
                case Opc.Ua.DataTypes.Float:
                    switch (ElementNumber)
                    {
                        case 1:
                            tagItem = 2;
                            w1 = string.Format("{0:X4}", BitConverter.ToInt16(data.Skip((int)startPosition).Take(2).ToArray(), 0));
                            return string.Format("{0}", w1);
                        //case 2:
                        //    {
                        //        tagItem = 2;
                        //        w1 = string.Format("{0:X4}", BitConverter.ToInt16(data.Skip((int)startPosition).Take(2).ToArray(), 0));
                        //        w2 = string.Format("{0:X4}", BitConverter.ToInt16(data.Skip((int)startPosition + 2).Take(2).ToArray(), 0));
                        //        return string.Format("{0}{1}", w1, w2);
                        //    }
                        default:
                            tagItem = 2;
                            w1 = string.Format("{0:X4}", BitConverter.ToInt16(data.Skip((int)startPosition).Take(2).ToArray(), 0));
                            w2 = string.Format("{0:X4}", BitConverter.ToInt16(data.Skip((int)startPosition + 2).Take(2).ToArray(), 0));
                            return string.Format("{0}{1}", w1, w2);
                    }
                default:
                    return string.Empty;
            }
        }

        private static bool GetDataToReadFormatted(FatekCommJob job, out string cmd, out uint itemCount, out string startAddress)
        {
            itemCount = 0;
            startAddress = string.Empty;

            cmd = GetReadWriteCommand(job);

            if (FatekProtocol.IsBitDataArea(job.Area))
            {
                if ((uint)job.TagsList[0].TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.Boolean)
                {
                    startAddress = GetStartAddressFormatted(job.Area, job.AreaAddress);
                    itemCount = (UInt16)job.TotalJobSize;
                }
                else
                {
                    startAddress = GetStartAddressFormatted(job.Area, job.AreaAddress);
                    itemCount = (UInt16)(job.TotalJobSize * 8);
                }
            } 
            else 
            { 
                if (job.TotalJobSize <= 1)
                    itemCount = 1;
                else
                    itemCount = (UInt16)(job.TotalJobSize / 2);
                startAddress = GetStartAddressFormatted(job.Area, job.AreaAddress);
            }            

            return true;
        }

        private static bool GetDataToWriteFormatted(FatekCommJob job, out uint itemCount, out string startAddress, out string dataToWrite)
        {
            dataToWrite = string.Empty;
            itemCount = 0;
            startAddress = string.Empty;
            ushort areaAddress = 0;
            uint startPosition = 0;

            object objectData = null;
            job.GetJobData(ref objectData);
            if (job.TagsListOnWriting.Count == 0)
                return false;

            byte[] jobdata = (byte[])objectData;
            uint PpotocolByteSize = job.GetProtocolDataByteSize();
            // Calculate the number of bytes to be read
            if (FatekProtocol.IsBitDataArea(job.Area))
            {
                for (startPosition = 0; startPosition < jobdata.Length; startPosition++)
                {
                    dataToWrite += FormatValueToWriteIntoDiscrete(job, jobdata[startPosition]);
                    itemCount++;
                }
                areaAddress = (ushort)(job.AreaAddress + job.TagsListOnWriting[0].ByteOffset);
            }
            else
            {
                if (job.TagsList[0].TagNode.ArrayDimension == 0)
                {
                    foreach (var tag in job.TagsListOnWriting)
                    {
                        uint tagItem = 0;
                        dataToWrite += FormatValueToWriteIntoRegister(tag, jobdata, startPosition, job.ElementNumber, ref tagItem);
                        startPosition += (tag.Size < 2 ? 2 : tag.Size);
                        itemCount += tagItem;
                    }                    
                }
                else
                {
                    Tag tag = job.TagsListOnWriting[0];
                    uint size = tag.Size / tag.TagNode.ArrayDimension;
                    for (int i=0; i < tag.TagNode.ArrayDimension; i++)
                    {
                        uint tagItem = 0;
                        dataToWrite += FormatValueToWriteIntoRegister(tag, jobdata, startPosition, job.ElementNumber, ref tagItem);
                        startPosition += (size < 2 ? 2 : size);
                        itemCount += tagItem;
                    }
                }

                areaAddress = (ushort)(job.AreaAddress + (job.TagsListOnWriting[0].ByteOffset /2));                    
            }

            startAddress = GetStartAddressFormatted(job.Area, areaAddress); // job.AreaAddress

            return true;        
        }

        private static byte[] GetDataPartfromAnswer(List<byte> reciveBuffer)
        {
            // STX (1byte) + Station ID (2 bytes) + Command ID (2 bytes) + Error code (1 byte) + <Data part ....> + CRC (2 byte) + STX (1 byte)
            return reciveBuffer.GetRange(PROTOCOL_ANSWER_HEADER_SIZE, (reciveBuffer.Count - (PROTOCOL_ANSWER_HEADER_SIZE + PROTOCOL_ANSWER_FOOTER_SIZE))).ToArray();
        }

        private static byte[] GetReadDataFromAnswer(FatekCommJob job, byte[] data)
        {
            byte[] dataBuffer = null;

            if (FatekProtocol.IsBitDataArea(job.Area))
            {                
                if ((uint)job.TagsList[0].TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.Boolean)
                {
                    dataBuffer = new byte[data.Length];
                    for (int i = 0; i < data.Length; i++)
                    {
                        switch (data[i])
                        {
                            case PROTOCOL_DESCRETE_VALUE_OFF:
                                dataBuffer[i] = 0;
                                break;
                            case PROTOCOL_DESCRETE_VALUE_ON:
                                dataBuffer[i] = 1;
                                break;
                        }
                    }
                }
                else
                {
                    uint byteArraySize = (uint)Math.DivRem((int)data.Length, 8, out int bitRest);
                    if (bitRest > 0)
                        byteArraySize++;
                    // convert array of bit into array of byte
                    dataBuffer = new byte[byteArraySize];                    

                    int byteNr = 0;
                    byte bitNr = 0;
                    for (int i = 0; i < data.Length; i++)
                    {
                        if (data[i] == PROTOCOL_DESCRETE_VALUE_ON)
                            dataBuffer[byteNr] |= (byte)(1 << bitNr);

                        if (bitNr == 7)
                        {
                            bitNr = 0;
                            byteNr++;
                        }
                        else
                        {
                            bitNr++;
                        }
                    }
                }
            }
            else
            {
                uint dataStartPosition = 0;
                uint dataBufferDestPosition = 0;
                dataBuffer = new byte[data.Length / 2];
                while (dataStartPosition < data.Length)
                {
                    Array.Copy(Hex2intByeArray(data, dataStartPosition), 0, dataBuffer, dataBufferDestPosition, 2);
                    dataStartPosition += 4;
                    dataBufferDestPosition += 2;
                }
            }

            return dataBuffer;
        }

        private static string GetReadWriteCommand(FatekCommJob job)
        {
            if (FatekProtocol.IsBitDataArea(job.Area))
            {
                if (job.CommandType == FatekCommJob.CommandTypes.WriteCmd)
                        return PROTOCOL_WRITE_DISCRETE_COMMAND;                    
                    else
                        return PROTOCOL_READ_DISCRETE_COMMAND;
            }
            else
            {
                if (job.CommandType == FatekCommJob.CommandTypes.WriteCmd)
                    return PROTOCOL_WRITE_REGISTER_COMMAND;
                else
                    return PROTOCOL_READ_REGISTER_COMMAND;             
            }
        }

        private static byte ComputeLRC(List<byte> buf, int Len)
        {
            byte bTempValue = 0;
            int i = 0;
            while (Len-- > 0)
                bTempValue += buf[i++];

            //bTempValue = (byte)(0xFF - bTempValue);
            //bTempValue += 0x01;
            return bTempValue;
        }

        private static string ComputeLRCTo2DigitString(List<byte> buf, int Len)
        {
            byte crc = ComputeLRC(buf, Len);

            return string.Format("{0:X2}", crc);
        }

        private static DriverErrorCodes GetPlcErrorCode(byte errorCode)
        {
            DriverErrorCodes resultError = (DriverErrorCodes)FatekErrorCodes.ErrorGeneric;

            if (errorCode == FatekProtocol.FATEK_ERRORNOERROR)
            {
                resultError = DriverErrorCodes.ErrorNoError;
            }
            else
            {                
                // if a number (ascii format) ?
                if (errorCode >= 48 && errorCode <= 57)
                    resultError = (DriverErrorCodes)(((int)errorCode - 48) + FATEK_ERRORCODE_OFFSET);
            }

            return resultError;
        }


        public static DriverErrorCodes ParseAnswer(FatekCommJob job, List<byte> reciveBuffer, out byte[] dataBuffer)
        {
            DriverErrorCodes errorCode = DriverErrorCodes.ErrorNoError;
            dataBuffer = null;

            #region frame integrity check
            if (reciveBuffer.Count < FatekProtocol.PROTOCOL_ANSWER_MIN_SIZE)
                return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)FatekProtocol.FatekErrorCodes.ErrorReceiveFrameError;

            // initial/final telegram char
            if (reciveBuffer[0] != FatekProtocol.STX || reciveBuffer[reciveBuffer.Count - 1] != FatekProtocol.ETX)
                return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)FatekProtocol.FatekErrorCodes.ErrorReceiveFrameError;

            // station ID
            if (Encoding.ASCII.GetString(reciveBuffer.GetRange(1,2).ToArray()) != ((FatekStation)job.Station).StationID.ToString("00"))
                return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)FatekProtocol.FatekErrorCodes.ErrorStationIdInvalid;

            //// command ID
            //if (Encoding.ASCII.GetString(reciveBuffer.GetRange(3, 2).ToArray()) != FatekProtocol.GetReadWriteCommand(job))
            //    return (DriverCodeBase.Enumerators.DriverErrorCodes)FatekProtocol.FatekErrorCodes.ErrorStationIdInvalid;

            // error code
            errorCode = GetPlcErrorCode(reciveBuffer[5]);
            if (errorCode != DriverErrorCodes.ErrorNoError)
                return errorCode;

            string crcX = ComputeLRCTo2DigitString(reciveBuffer, reciveBuffer.Count - 3);
            if (Encoding.ASCII.GetString(reciveBuffer.GetRange(reciveBuffer.Count - 3, 2).ToArray()) != crcX)
                return (DriverCodeBaseEx.Enumerators.DriverErrorCodes)FatekProtocol.FatekErrorCodes.ErrorCRCInvalid;
            #endregion

            #region frame data parsing
            if (job.CommandType == FatekCommJob.CommandTypes.ReadCmd)            
                dataBuffer = GetReadDataFromAnswer(job, GetDataPartfromAnswer(reciveBuffer));
            #endregion

            return errorCode;
        }

        static byte[] Hex2intByeArray(byte[] hexString, uint startPosition = 0)
        {
            int val = 0;

            if (hexString.Length >= (startPosition + 4))
            {
                for (uint i = 0; i < 4; i++)
                {
                    short ch = hexString[startPosition + i];
                    if (ch <= 57)
                        val += (ch - 48) * (1 << (4 * (4 - 1 - (int)i)));
                    else
                        val += (ch - 55) * (1 << (4 * (4 - 1 - (int)i)));
                }
            }
            return BitConverter.GetBytes((short)val);
        }

        public static uint PrepareReadRequest(FatekCommJob job, ref List<byte> buffer)
        {
            FatekStation st = (FatekStation)job.Station;

            //start of telegram
            buffer.Add(STX);

            // station id
            buffer.AddRange(Encoding.ASCII.GetBytes(string.Format("{0:X2}", st.StationID)));
            
            if (!GetDataToReadFormatted(job, out string cmd,out uint itemCount, out string startAddress))
                return 0;

            buffer.AddRange(Encoding.ASCII.GetBytes(cmd));

            // nr of register to read
            buffer.AddRange(Encoding.ASCII.GetBytes(string.Format("{0:X2}", itemCount)));

            // start address
            buffer.AddRange(Encoding.ASCII.GetBytes(startAddress));

            string crcX = ComputeLRCTo2DigitString(buffer, buffer.Count);
            buffer.AddRange(Encoding.ASCII.GetBytes(crcX));
            //end of telegram
            buffer.Add(ETX);

            return (uint)buffer.Count;
        }

        public static uint PrepareWriteRequest(FatekCommJob job, ref List<byte> buffer)
        {
            FatekStation st = (FatekStation)job.Station;

            //start of telegram
            buffer.Add(STX);

            // station id
            buffer.AddRange(Encoding.ASCII.GetBytes(string.Format("{0:X2}", st.StationID)));

            // command id --> read            
            buffer.AddRange(Encoding.ASCII.GetBytes(GetReadWriteCommand(job)));

            if (!GetDataToWriteFormatted(job, out uint itemCount, out string startAddress, out string dataToWrite))
                return 0;

            // nr of register to read
            buffer.AddRange(Encoding.ASCII.GetBytes(string.Format("{0:X2}", itemCount)));

            // start address
            buffer.AddRange(Encoding.ASCII.GetBytes(startAddress));

            // data to write
            buffer.AddRange(Encoding.ASCII.GetBytes(dataToWrite));

            string crcX = ComputeLRCTo2DigitString(buffer, buffer.Count);
            buffer.AddRange(Encoding.ASCII.GetBytes(crcX));
            //end of telegram
            buffer.Add(ETX);

            return (uint)buffer.Count;
        }

        public static uint PrepareRequest(FatekCommJob job, ref List<byte> buffer)
        {
            // read command
            job.CommandType = (job.ReadRequest() ? FatekCommJob.CommandTypes.ReadCmd : FatekCommJob.CommandTypes.WriteCmd);
            switch (job.CommandType)
            {
                case FatekCommJob.CommandTypes.ReadCmd:
                    return PrepareReadRequest(job, ref buffer);
                case FatekCommJob.CommandTypes.WriteCmd:
                    return PrepareWriteRequest(job, ref buffer);
                //case FatekCommJob.CommandTypes.Invalid:
                //    return 0;
                default:
                    return 0;
            }
        }

        private static uint ConvertOpcDataTypeToBuiltInDataType(UFUAModel.DataType varType)
        {
            switch (varType)
            {
                case UFUAModel.DataType.Boolean:
                    return (uint)BuiltInType.Boolean;
                case UFUAModel.DataType.SByte:
                    return (uint)BuiltInType.SByte;
                case UFUAModel.DataType.Byte:
                    return (uint)BuiltInType.Byte;
                case UFUAModel.DataType.Int16:
                    return (uint)BuiltInType.Int16;
                case UFUAModel.DataType.UInt16:
                    return (uint)BuiltInType.UInt16;
                case UFUAModel.DataType.Int32:
                    return (uint)BuiltInType.Int32;
                case UFUAModel.DataType.UInt32:
                    return (uint)BuiltInType.UInt32;
                case UFUAModel.DataType.Int64:
                    return (uint)BuiltInType.Int64;
                case UFUAModel.DataType.UInt64:
                    return (uint)BuiltInType.UInt64;
                case UFUAModel.DataType.Float:
                    return (uint)BuiltInType.Float;
                case UFUAModel.DataType.Double:
                    return (uint)BuiltInType.Double;
                case UFUAModel.DataType.String:
                    return (uint)BuiltInType.String;
                default:                    
                    return (uint)BuiltInType.Boolean;
            }
        }


        // used for dynamic setting validation into UI
        public static bool IsTypeAdmitted(DataArea area, UFUAModel.DataType varType) {
            // for struct data type
            if ((uint)varType == unchecked((uint)-1))
                return IsTypeAdmittedForStruct(area);
            else
                return IsTypeAdmitted(area, ConvertOpcDataTypeToBuiltInDataType(varType));
        }

        // used for dynamic setting validation at runtime
        public static bool IsTypeAdmitted(DataArea area, NodeId type)
        {
            if (type.IdType == IdType.Numeric)
                return IsTypeAdmitted(area, (uint)type.Identifier);
            else
                return false;
        }

        // used for dynamic setting validation at runtime
        private static bool IsTypeAdmitted(DataArea area, uint type)
        {
            // for struct data type
            if ((uint)type == unchecked((uint)-1))
                return IsTypeAdmittedForStruct(area);

            bool admitted = IsTypeAdmitted(type);
            if (admitted && ((uint)type == (uint)BuiltInType.Float))
                admitted = (area == DataArea.HR || area == DataArea.DR || area == DataArea.FR);

            return admitted;
        }

        private static bool IsTypeAdmittedForStruct(DataArea area) 
        {
            return true;// (area == DataArea.HR || area == DataArea.DR || area == DataArea.FR);
        }

        private static bool IsTypeAdmitted(uint nType)
        {
            if (nType == (uint)BuiltInType.Boolean ||
                nType == (uint)BuiltInType.Byte ||
                nType == (uint)BuiltInType.SByte ||
                nType == (uint)BuiltInType.Int16 ||
                nType == (uint)BuiltInType.UInt16 ||
                //nType == (uint)BuiltInType.Double ||
                nType == (uint)BuiltInType.Float ||
                nType == (uint)BuiltInType.Int32 ||
                nType == (uint)BuiltInType.UInt32
            //nType == (uint)BuiltInType.Int64 ||
            //nType == (uint)BuiltInType.UInt64 ||
            //nType == (uint)BuiltInType.String ||                
            )
                return true;
            else
                return false;
        }

        public static bool ParseData(byte[] receivebuffer, ref FatekCommJob job, ref List<object> items)
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
                if (!IsTypeAdmitted((uint)bt))
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

            job.SetJobData(receivebuffer, ref changed);

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

        public static uint GetMaxJobSize(DataArea area)
        {
            if (FatekProtocol.IsBitDataArea(area))
                return (248 / 8); // nr of elements                
            else
                return (128 * 2); // nr of byte                
        }
        #endregion        
    }
}