using System;
using DriverBaseInterfaces;
using Opc.Ua;

namespace FanucCNC
{
    public class FanucFocasAddress
    {
        #region Constructors

        /// <summary>
        /// Initializes the FanucFocasAddress object with default values.
        /// </summary>
        public FanucFocasAddress()
        {
            Reset();
        }

        /// <summary>
        /// Initializes the FanucFocasAddress object parsing the assigned address.
        /// </summary>
        public FanucFocasAddress(string inAddress, BuiltInType mdt, uint arrayDimension)
        {
            Parse(inAddress, mdt, arrayDimension);
        }

        /// <summary>
        /// Initializes the OmronAddress object parsing the assigned address and add offset.
        /// </summary>
        public FanucFocasAddress(string inAddress, BuiltInType mdt, uint arrayDimension, TagDefinition prevtagdefinition)
        {
            Parse(inAddress, mdt, arrayDimension);
            if (!_IsValid)
                return;

            //if (prevtagdefinition.DataType.IdType == IdType.Numeric)
            //{
            //    ushort ArrayDimension = (ushort)prevtagdefinition.ArrayDimension;
            //    if (ArrayDimension == 0)
            //        ArrayDimension = 1;

            //    switch ((uint)prevtagdefinition.DataType.Identifier)
            //    {
            //        case (uint)BuiltInType.Boolean:
            //            switch (_DataFormat)
            //            {
            //                case DataFormats.Bit:
            //                    ushort bitIndex = (ushort)(_BitNumber + ArrayDimension);
            //                    _Address.USHORT += (ushort)(bitIndex / 16);
            //                    _BitNumber = (sbyte)(bitIndex % 16);
            //                    break;
            //                case DataFormats.Word:
            //                    _Address.USHORT += ArrayDimension;
            //                    break;
            //            }
            //            break;
            //        case (uint)BuiltInType.SByte:
            //        case (uint)BuiltInType.Byte:
            //        case (uint)BuiltInType.Int16:
            //        case (uint)BuiltInType.UInt16:
            //            _Address.USHORT += ArrayDimension;
            //            if(_DataFormat == DataFormats.Bit)
            //                _BitNumber = 0;
            //            break;
            //        case (uint)BuiltInType.Float:
            //        case (uint)BuiltInType.UInt32:
            //        case (uint)BuiltInType.Int32:
            //            _Address.USHORT += (ushort)(2 * ArrayDimension);
            //            if(_DataFormat == DataFormats.Bit)
            //                _BitNumber = 0;
            //            break;
            //        case (uint)BuiltInType.UInt64:
            //        case (uint)BuiltInType.Int64:
            //        case (uint)BuiltInType.Double:
            //            _Address.USHORT += (ushort)(4 * ArrayDimension);
            //            if(_DataFormat == DataFormats.Bit)
            //                _BitNumber = 0;
            //            break;
            //        case (uint)BuiltInType.String:
            //            _Address.USHORT += (ushort)ArrayDimension;
            //            break;
            //    }
            //}
        }


        #endregion

        #region Methods

        void Reset()
        {
            _IsValid = false;
            _Address = string.Empty;
            _FunctionCode = FanucCNCProtocol.PmdDataType.Unknown;
            _AddressTypeNumeric = FanucCNCProtocol.GetAddressTypeNumeric(_FunctionCode);
            _VarType = BuiltInType.Null;
            _AddressNumber = 0;
            _ReadDataType = FanucCNCProtocol.ReadDataType.Byte;
            //_NrBytesToRead = 0;
            _AddressBitNumber = -1;
            _StringLength = -1;
            _ParsingError = string.Empty;
        }

        /// <summary>
        /// Set a new address.
        /// </summary>

        //public void Set(string address)
        //{
        //    Parse(Address);
        //}
  
        //public string Get()
        //{
        //    if(!_IsValid)
        //        return string.Empty;

        //    var dynamicstring = new StringBuilder();
        //    switch (_DataArea)
        //    {
        //        case DataAreas.IR:
        //            dynamicstring.Append("IR");
        //            break;
        //        case DataAreas.HR:
        //           dynamicstring.Append("HR");
        //            break;
        //        case DataAreas.AR:
        //            dynamicstring.Append("AR");
        //            break;
        //        case DataAreas.DM:
        //            dynamicstring.Append("DM");
        //            break;
        //        case DataAreas.T:
        //            dynamicstring.Append("T");
        //            break;
        //        case DataAreas.C:
        //            dynamicstring.Append("C");
        //            break;
        //        case DataAreas.EM:
        //            dynamicstring.Append("EM");
        //            break;
        //        case DataAreas.W:
        //            dynamicstring.Append("W");
        //            break;
        //        default:
        //            return string.Empty;
        //    }
        //    if(_BankNumber>=0)
        //        dynamicstring.AppendFormat("{0:X}", _BankNumber);
        //    dynamicstring.AppendFormat("{0}", _Address.USHORT);
        //    if(_DataFormat == DataFormats.Bit)
        //        dynamicstring.AppendFormat(".{0}", _BitNumber);
        //    else if (_DataFormat == DataFormats.String)
        //    {
        //        dynamicstring.AppendFormat(":{0}", _StringLength);
        //    }

        //    return dynamicstring.ToString();
        //}

        //public string GetFormattedaddress()
        //{
        //    if (!_IsValid)
        //        return string.Empty;

        //    var dynamicstring = new StringBuilder();
        //    switch (_DataArea)
        //    {
        //        case DataAreas.IR:
        //            dynamicstring.Append("IR");
        //            break;
        //        case DataAreas.HR:
        //            dynamicstring.Append("HR");
        //            break;
        //        case DataAreas.AR:
        //            dynamicstring.Append("AR");
        //            break;
        //        case DataAreas.DM:
        //            dynamicstring.Append("DM");
        //            break;
        //        case DataAreas.T:
        //            dynamicstring.Append("T");
        //            break;
        //        case DataAreas.C:
        //            dynamicstring.Append("C");
        //            break;
        //        case DataAreas.EM:
        //            dynamicstring.Append("EM");
        //            break;
        //        case DataAreas.W:
        //            dynamicstring.Append("W");
        //            break;
        //        default:
        //            return string.Empty;
        //    }
        //    if (_BankNumber >= 0)
        //        dynamicstring.AppendFormat("{0:X}", _BankNumber);
        //    dynamicstring.AppendFormat("{0}", _Address.USHORT.ToString().PadLeft(5,'0'));
        //    if (_DataFormat == DataFormats.Bit)
        //        dynamicstring.AppendFormat(".{0}", _BitNumber.ToString().PadLeft(3,'0'));

        //    return dynamicstring.ToString();
        //}

        //public bool isBit()
        //{
        //    return (DataFormat == DataFormats.Bit);
        //}

        /// <summary>
        /// Parse an address.
        /// </summary>
        void Parse(string address, BuiltInType mdt, uint arrayDimension)
        {
            Reset();

            _Address = address;
            _VarType = mdt;

            if (String.IsNullOrWhiteSpace(address))
            {
                _ParsingError = Properties.Resources.ErrorAddressEmpty;
                return;
            }            

            //switch (area)
            //{
            //    case FanucCNCProtocol.DataArea.PMC_DATA:
                    _ParsingError = GetAddressType(ref address, ref _FunctionCode);
                    if (string.IsNullOrEmpty(_ParsingError))
                    {
                        _ParsingError = GetAddressNumbers(_FunctionCode, address, mdt, arrayDimension, ref _AddressNumber, ref _AddressBitNumber);
                        //if (!string.IsNullOrEmpty(_ParsingError))
                        //    _NrBytesToRead = GetNrBytesToRead(_FunctionCode, mdt);
                    }
            //        break;
            //    case FanucCNCProtocol.DataArea.CNC_DATA:
            //        _ParsingError = Properties.Resources.ErrorAddressDataTypeInvalid;
            //        break;                
            //    case FanucCNCProtocol.DataArea.FILE_MANAGEMENT_DATA:
            //        _ParsingError = Properties.Resources.ErrorAddressDataTypeInvalid;
            //        break;
            //}

            //// Parse Data Area and Start Address
            //string upperString = TempAddress.ToUpper();
            //TempAddress = upperString;

            //Regex TagNameParser = new Regex(@"^(?<DataArea>[A-Z]{1,3})(?<BankNumber>[0-9A-C]_)?(?<Address>\d{1,5})(?<BitNumber>\.\d{1,2})?(?<StringLength>\:\d{1,3})?$");
            //Match TagNameMatch = TagNameParser.Match(TempAddress);
            //if (!TagNameMatch.Success)
            //{
            //    return;
            //}

            //TempValue = Convert.ToUInt32(TagNameMatch.Groups["Address"].Value);

            //if (TempValue > 0xFFFF)
            //{
            //    return;
            //}

            //switch (TagNameMatch.Groups["DataArea"].Value)
            //{
            //    case "CIO":
            //    case "IR":
            //    case "I":
            //        TempDataArea = DataAreas.IR;
            //        break;
            //    case "HR":
            //    case "H":
            //        TempDataArea = DataAreas.HR;
            //        break;
            //    case "AR":
            //    case "A":
            //        TempDataArea = DataAreas.AR;
            //        break;
            //    case "DM":
            //    case "D":
            //        TempDataArea = DataAreas.DM;
            //        break;
            //    case "TIM":
            //    case "T":
            //        TempDataArea = DataAreas.T;
            //        break;
            //    case "CNT":
            //    case "C":
            //        TempDataArea = DataAreas.C;
            //        break;
            //    case "EM":
            //    case "E":
            //        TempDataArea = DataAreas.EM;
            //        break;
            //    case "WR":
            //    case "W":
            //        TempDataArea = DataAreas.W;
            //        break;
            //    default:
            //        return ;
            //}

            //if (TagNameMatch.Groups["BitNumber"].Value.Length > 1)
            //{
            //    TempBitNumber = Convert.ToInt32(TagNameMatch.Groups["BitNumber"].Value.Remove(0, 1));
            //    if (TempBitNumber > 15)
            //    {
            //        return;
            //    }

            //    switch (TempDataArea)
            //    {
            //        case DataAreas.T:
            //        case DataAreas.C:
            //            return;
            //        default:
            //            TempDataFormat = DataFormats.Bit;
            //            break;
            //    }
            //}

            //if (TagNameMatch.Groups["BankNumber"].Value.Length == 2)
            //{
            //    if (TempDataArea != DataAreas.EM)
            //    {
            //        return;
            //    }
            //    TempBankNumber = Convert.ToInt32(TagNameMatch.Groups["BankNumber"].Value.Remove(1, 1), 16);
            //}

            //if (TagNameMatch.Groups["StringLength"].Value.Length > 1)
            //{
            //    if (TempDataFormat == DataFormats.Bit)
            //    {
            //        return;
            //    }
            //    switch (TempDataArea)
            //    {
            //        case DataAreas.T:
            //        case DataAreas.C:
            //            return;
            //        default:
            //            try
            //            {
            //                TempStringLength = Convert.ToInt32(TagNameMatch.Groups["StringLength"].Value.Remove(0, 1));
            //                TempDataFormat = DataFormats.String;
            //            }
            //            catch(Exception ex)
            //            {
            //                return;
            //            }
            //            break;
            //    }
            //}

            //_DataArea = TempDataArea;
            //_DataFormat = TempDataFormat;
            //_Address.USHORT = (ushort)TempValue;
            //_BankNumber = TempBankNumber;
            //_BitNumber = (sbyte)TempBitNumber;
            //_IsValid = true;
            //_StringLength = TempStringLength;

            _IsValid = string.IsNullOrEmpty(_ParsingError);
        }

        private string GetAddressType(ref string address, ref FanucCNCProtocol.PmdDataType type)
        {
            string result = string.Empty;
            switch (address.Substring(0,1))
            {
                case "D":
                    type = FanucCNCProtocol.PmdDataType.PMC_D_DataTable;
                    break;
                default:
                    result = Properties.Resources.ErrorAddressDataTypeInvalid;
                    break;
            }
            address = address.Substring(1, address.Length - 1);
                    
            if (string.IsNullOrEmpty(result))
            {
                _AddressTypeNumeric = FanucCNCProtocol.GetAddressTypeNumeric(_FunctionCode);
            }

            return result;
        }

        private string GetAddressNumbers(FanucCNCProtocol.PmdDataType type, string address, BuiltInType mdt, uint arrayDimension, ref ushort adrNumber, ref sbyte adrBitNumber)
        {
            string result = Properties.Resources.ErrorAddressInvalid;            

            switch (type)
            {
                case FanucCNCProtocol.PmdDataType.PMC_D_DataTable:
                    {
                        var data = address.Split('.'); // bit sepeparator
                        switch (data.Length)
                        {
                            case 1:
                                if (!ushort.TryParse(data[0], out adrNumber))
                                    return result;

                                if (adrNumber < 1 || adrNumber > 10000)
                                    return result;

                                if (mdt == BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;
                                result = string.Empty;
                                break;

                            case 2:
                                if (!ushort.TryParse(data[0], out adrNumber) || !sbyte.TryParse(data[1], out adrBitNumber))
                                    return result;

                                if (adrNumber < 1 || adrNumber > 10000)
                                    return result;

                                if (adrBitNumber < 0 || adrBitNumber > 8)
                                    return result;

                                if (mdt != BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                                result = string.Empty;
                                break;
                        }
                    }
                    break;
                case FanucCNCProtocol.PmdDataType.PMC_T_ChangeableTimer:
                    if (!ushort.TryParse(address, out adrNumber))
                        return result;

                    if (adrNumber < 1 || adrNumber > 10000)
                        return result;

                    if (mdt == BuiltInType.Boolean)
                        return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                    result = string.Empty;
                    break;
                case FanucCNCProtocol.PmdDataType.PMC_R_InternalRelay:
                    if (!ushort.TryParse(address, out adrNumber))
                        return result;

                    if (adrNumber < 1 || adrNumber > 10000)
                        return result;

                    if (mdt == BuiltInType.Boolean)
                        return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                    result = string.Empty;
                    break;
                case FanucCNCProtocol.PmdDataType.PMC_A_MessageDemand:
                    {
                        var data = address.Split('.'); // bit sepeparator
                        switch (data.Length)
                        {
                            case 2:
                                if (!ushort.TryParse(data[0], out adrNumber) || !sbyte.TryParse(data[1], out adrBitNumber))
                                    return result;

                                if (adrNumber < 1 || adrNumber > 10000)
                                    return result;

                                if (adrBitNumber < 0 || adrBitNumber > 8)
                                    return result;

                                if (mdt != BuiltInType.Boolean)
                                    return Properties.Resources.ErrorAddressTagDataTypeIncorrect;

                                result = string.Empty;
                                break;
                        }
                    }
                    break;
            }

            return result;
        }

        //private uint GetNrBytesToRead(FanucCNCProtocol.AddressType type, BuiltInType mdt)
        //{
        //    uint result = 0;

        //    switch (type)
        //    {
        //        case FanucCNCProtocol.FunctionCode.PMC_D_DataTable:
        //        case FanucCNCProtocol.FunctionCode.PMC_T_ChangeableTimer:
        //        case FanucCNCProtocol.FunctionCode.PMC_R_InternalRelay:
        //        case FanucCNCProtocol.FunctionCode.PMC_A_MessageDemand:
        //            if (mdt == BuiltInType.Boolean)
        //            {
        //                if (ArrayNum)
        //                _NrBytesToRead = 
        //            } else
        //            {

        //            }
        //            break;
        //    }

        //    return result;
        //}

        public static bool IsValidAddress(string address, UFUAModel.DataType mdt, uint arrayDimension, out string error)
        {
            FanucFocasAddress adr = new FanucFocasAddress(address, FanucCNCProtocol.GetDataType(mdt), arrayDimension);

            error = adr.ParsingError;

            return adr.IsValid;
        }

        #endregion

        #region Properties
        private string _Address;
        public string Address
        {
            get { return _Address; }
        }
        
        private FanucCNC.FanucCNCProtocol.PmdDataType _FunctionCode;
        public FanucCNCProtocol.PmdDataType FunctionCode
        {
            get { return _FunctionCode; }
        }

        private short _AddressTypeNumeric;
        public short AddressTypeNumeric
        {
            get { return _AddressTypeNumeric; }
        }

        private FanucCNCProtocol.ReadDataType _ReadDataType;
        public short ReadDataType
        {
            get { return (short)_ReadDataType; }
        }

        private BuiltInType _VarType;
        public BuiltInType VarType
        {
            get { return _VarType; }
        }

        private ushort _AddressNumber;
        public ushort AddressNumber
        {
            get { return _AddressNumber; }
        }

        private sbyte _AddressBitNumber;
        public sbyte AddressBitNumber
        {
            get { return _AddressBitNumber; }
        }

        //private uint _NrBytesToRead;
        //public uint NrBytesToRead
        //{
        //    get { return _NrBytesToRead; }
        //}

        /// <summary>
        /// Property that indicates if an address is valid
        /// </summary>
        private bool _IsValid;
        public bool IsValid
        {
            get
            {
                return _IsValid;
            }
        }

        /// <summary>
        /// String length
        /// </summary>
        private int _StringLength;
        public int StringLength
        {
            get
            {
                return _StringLength;
            }
        }
        
        private string _ParsingError;
        public string ParsingError
        {
            get { return _ParsingError; }
        }
        #endregion
    }
}
