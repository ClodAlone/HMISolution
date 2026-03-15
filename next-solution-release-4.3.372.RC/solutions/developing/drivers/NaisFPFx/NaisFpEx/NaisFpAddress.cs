using System;
using System.Text;
using System.Text.RegularExpressions;
using DriverBaseInterfaces;
using Opc.Ua;

namespace NaisFp
{
    public class NaisFpAddress
    {
        #region Constructors

        /// <summary>
        /// Initializes the NaisFpAddress object with default values.
        /// </summary>
        public NaisFpAddress()
        {
            Reset();
        }

        /// <summary>
        /// Initializes the NaisFpAddress object parsing the assigned address.
        /// </summary>
        public NaisFpAddress(string inAddress)
        {
            Parse(inAddress);
        }

        /// <summary>
        /// Initializes the NaisFpAddress object parsing the assigned address and add offset.
        /// </summary>
        public NaisFpAddress(string inAddress, TagDefinition prevtagdefinition)
        {
            Parse(inAddress);
            if (!_IsValid)
                return ;

            if (prevtagdefinition.DataType.IdType == IdType.Numeric)
            {
                ushort ArrayDimension = (ushort)prevtagdefinition.ArrayDimension;
                if (ArrayDimension == 0)
                    ArrayDimension = 1;
                
                switch ((uint)prevtagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        switch (_DataFormat)
                        {
                            case DataFormats.BOOL:
                                ushort bitIndex = (ushort)(_BitNumber + ArrayDimension);
                                _StartAddress.USHORT += (ushort)(bitIndex / 16);
                                _BitNumber = (byte)(bitIndex % 16);
                                 break;
                            case DataFormats.WORD:
                                _StartAddress.USHORT += ArrayDimension;
                                break;
                            case DataFormats.DWORD:
                                _StartAddress.USHORT += (ushort)(2 * ArrayDimension);
                                break;
                        }
                        break;
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        _StartAddress.USHORT += ArrayDimension;
                        if (_DataFormat == DataFormats.BOOL)
                            _BitNumber = 0;
                        if (_DataFormat == DataFormats.DWORD)
                            _StartAddress.USHORT += ArrayDimension;
                        break;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        _StartAddress.USHORT += (ushort)(2 * ArrayDimension);
                        if (_DataFormat == DataFormats.BOOL)
                            _BitNumber = 0;
                        break;
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        _StartAddress.USHORT += (ushort)(4*ArrayDimension);
                        if (_DataFormat == DataFormats.BOOL)
                            _BitNumber = 0;
                        break;
                    case (uint)BuiltInType.String:
                        _StartAddress.USHORT += ArrayDimension;
                        break;
                }
            }
        }

        public bool SetElemArrayAddress(int MovType, int arrayIndex)
        {
            if ((arrayIndex < 0) || (_IsValid == false))
            {
                return false;
            }

            switch (MovType)
            {
                case (int)UFUAModel.DataType.Boolean:
                    switch (_DataFormat)
                    {
                        case DataFormats.BOOL:
                            ushort bitIndex = (ushort)(_BitNumber + arrayIndex);
                            _StartAddress.USHORT += (ushort)(bitIndex / 16);
                            _BitNumber = (byte)(bitIndex % 16);
                            break;
                        case DataFormats.WORD:
                            _StartAddress.USHORT += (ushort)arrayIndex;
                            break;
                        case DataFormats.DWORD:
                            _StartAddress.USHORT += (ushort)(2 * arrayIndex);
                            break;
                    }
                    break;
                case (int)UFUAModel.DataType.SByte:
                case (int)UFUAModel.DataType.Byte:
                case (int)UFUAModel.DataType.Int16:
                case (int)UFUAModel.DataType.UInt16:
                    _StartAddress.USHORT += (ushort)arrayIndex;
                    if (_DataFormat == DataFormats.BOOL)
                        _BitNumber = 0;
                    if (_DataFormat == DataFormats.DWORD)
                        _StartAddress.USHORT += (ushort)arrayIndex;
                    break;
                case (int)UFUAModel.DataType.Float:
                case (int)UFUAModel.DataType.UInt32:
                case (int)UFUAModel.DataType.Int32:
                    _StartAddress.USHORT += (ushort)(2 * arrayIndex);
                    if (_DataFormat == DataFormats.BOOL)
                        _BitNumber = 0;
                    break;
                case (int)UFUAModel.DataType.UInt64:
                case (int)UFUAModel.DataType.Int64:
                case (int)UFUAModel.DataType.Double:
                    _StartAddress.USHORT += (ushort)(4 * arrayIndex);
                    if (_DataFormat == DataFormats.BOOL)
                        _BitNumber = 0;
                    break;
                default:
                    return false;

            }

            return true;
            
        }

        #endregion

        #region Methods

        void Reset()
        {
            _IsValid = false;
            _MemoryArea = MemoryAreas.Invalid;
            _DataFormat = DataFormats.Invalid;
            _StartAddress = new ushortUnion(0);
            _BitNumber = 0;

        }

        /// <summary>
        /// Set a new address.
        /// </summary>

        public void Set(string Address)
        {
            Parse(Address);
        }

        public string GetNaisAddress(DataFormats inDataFormat, int offset = 0)
        {
            if (!_IsValid)
                return string.Empty;

            var dynamicstring = new StringBuilder();
            switch (_MemoryArea)
            {
                case MemoryAreas.DT:
                case MemoryAreas.FL:
                case MemoryAreas.LD:
                    switch (inDataFormat)
                    {
                        case DataFormats.BOOL:
                            break;
                        case DataFormats.WORD:
                        case DataFormats.DWORD:
                            GetWordLongAddress(ref dynamicstring, offset);
                            break;
                    }
                    break;
                case MemoryAreas.EV:
                case MemoryAreas.SV:
                    switch (inDataFormat)
                    {
                        case DataFormats.BOOL:
                            break;
                        case DataFormats.WORD:
                        case DataFormats.DWORD:
                            GetWordAddress(ref dynamicstring, offset);
                            break;
                    }
                    break;

                case MemoryAreas.L:
                case MemoryAreas.R:
                case MemoryAreas.X:
                case MemoryAreas.Y:
                    switch (inDataFormat)
                    {
                        case DataFormats.BOOL:
                            GetBitAddress(ref dynamicstring, offset);
                            break;
                        case DataFormats.WORD:
                        case DataFormats.DWORD:
                            GetWordAddress(ref dynamicstring, offset);
                            break;
                    }
                    break;
                default:
                    break;
            }

            return dynamicstring.ToString();
 
        }


        void GetBitAddress(ref StringBuilder dynamicstring,int offset = 0)
        {
            dynamicstring.Append(MemoryArea.ToString());
            int bitAddress = StartAddress.USHORT * 16 + offset + _BitNumber;
            dynamicstring.AppendFormat("{0:D3}", bitAddress / 16);
            dynamicstring.AppendFormat("{0:X1}", bitAddress % 16);
        }

        void GetWordLongAddress(ref StringBuilder dynamicstring, int offset = 0)
        {
            dynamicstring.AppendFormat("{0:D5}", _StartAddress.USHORT + offset);
        }
        void GetWordAddress(ref StringBuilder dynamicstring, int offset = 0)
        {
            dynamicstring.AppendFormat("{0:D4}", _StartAddress.USHORT + offset);
        }
        void GetAddressShort(ref StringBuilder dynamicstring)
        {
            dynamicstring.AppendFormat("{0}", _StartAddress.USHORT );
        }
        void GetBitAddressShort(ref StringBuilder dynamicstring)
        {
            dynamicstring.Append(MemoryArea.ToString());
            if (StartAddress.USHORT != 0)
                dynamicstring.AppendFormat("{0}", StartAddress.USHORT);
            dynamicstring.AppendFormat("{0:X1}", _BitNumber);
        }

        public string Get()
        {
            if(!_IsValid)
                return string.Empty;

            var dynamicstring = new StringBuilder();
            switch (_MemoryArea)
            {
                case MemoryAreas.DT:
                case MemoryAreas.FL:
                case MemoryAreas.LD:
                    switch (_DataFormat)
                    {
                        case DataFormats.BOOL:
                            break;
                        case DataFormats.WORD:
                            dynamicstring.Append(MemoryArea.ToString());
                            GetAddressShort(ref dynamicstring);
                            break;
                        case DataFormats.DWORD:
                            dynamicstring.Append("D");
                            dynamicstring.Append(MemoryArea.ToString());
                            GetAddressShort(ref dynamicstring);
                            break;
                    }
                    break;
                case MemoryAreas.EV:
                case MemoryAreas.SV:
                    switch (_DataFormat)
                    {
                        case DataFormats.BOOL:
                            break;
                        case DataFormats.WORD:
                            dynamicstring.Append(MemoryArea.ToString());
                            GetAddressShort(ref dynamicstring);
                            break;
                        case DataFormats.DWORD:
                            dynamicstring.Append("D");
                            dynamicstring.Append(MemoryArea.ToString());
                            GetAddressShort(ref dynamicstring);
                            break;
                    }
                    break;

                case MemoryAreas.L:
                case MemoryAreas.R:
                case MemoryAreas.X:
                case MemoryAreas.Y:
                    switch (_DataFormat)
                    {                            
                        case DataFormats.BOOL:
                            GetBitAddressShort(ref dynamicstring);
                            break;
                        case DataFormats.WORD:
                            dynamicstring.Append("W");
                            dynamicstring.Append(MemoryArea.ToString());
                            GetAddressShort(ref dynamicstring);
                            break;
                        case DataFormats.DWORD:
                            dynamicstring.Append("DW");
                            dynamicstring.Append(MemoryArea.ToString());
                            GetAddressShort(ref dynamicstring);
                            break;
                    }
                    break;
                default:
                    break;
            }

            return dynamicstring.ToString();
        }


        public bool isBit()
        {
            return (DataFormat == DataFormats.BOOL);
        }

        public byte Size()
        {
            switch (DataFormat)
            {
                case DataFormats.BOOL:
                    return (1);
                case DataFormats.WORD:
                    return(2);
                case DataFormats.DWORD:
                    return(4);
            }
            return (0);
        }

        /// <summary>
        /// Parse an address.
        /// </summary>
        public void Parse(string FpAddress)
        {
            Reset();
            MemoryAreas TempMemoryArea;
            DataFormats TempDataFormat = DataFormats.WORD;
            uint TempValue;
            int TempBitNumber = 0;
            string TempAddress = String.Copy(FpAddress);

            if (String.IsNullOrWhiteSpace(TempAddress))
            {
                return;
            }

            TempAddress.Trim();

            if (TempAddress.Length < 2)
            {
                return;
            }

            // Parse Data Area and Start Address
            string upperString = TempAddress.ToUpper();
            TempAddress = upperString;

            Regex TagNameParser = new Regex(@"^(?<MemoryArea>[A-Z]{1,3})(?<AddressH>\d{0,4})(?<AddressL>[0-9A-F])$");
            Match TagNameMatch = TagNameParser.Match(TempAddress);
            if (!TagNameMatch.Success)
            {
                return;
            }


            switch (TagNameMatch.Groups["MemoryArea"].Value)
            {
                case "X":
                    TempMemoryArea = MemoryAreas.X;
                    TempDataFormat = DataFormats.BOOL;
                    break;
                case "Y":
                    TempMemoryArea = MemoryAreas.Y;
                    TempDataFormat = DataFormats.BOOL;
                    break;
                case "R":
                    TempMemoryArea = MemoryAreas.R;
                    TempDataFormat = DataFormats.BOOL;
                    break;
                case "L":
                    TempMemoryArea = MemoryAreas.L;
                    TempDataFormat = DataFormats.BOOL;
                    break;
                case "WX":
                    TempMemoryArea = MemoryAreas.X;
                    TempDataFormat = DataFormats.WORD;
                    break;
                case "WY":
                    TempMemoryArea = MemoryAreas.Y;
                    TempDataFormat = DataFormats.WORD;
                    break;
                case "WR":
                    TempMemoryArea = MemoryAreas.R;
                    TempDataFormat = DataFormats.WORD;
                    break;
                case "WL":
                    TempMemoryArea = MemoryAreas.L;
                    TempDataFormat = DataFormats.WORD;
                    break;
                case "DWX":
                    TempMemoryArea = MemoryAreas.X;
                    TempDataFormat = DataFormats.DWORD;
                    break;
                case "DWY":
                    TempMemoryArea = MemoryAreas.Y;
                    TempDataFormat = DataFormats.DWORD;
                    break;
                case "DWR":
                    TempMemoryArea = MemoryAreas.R;
                    TempDataFormat = DataFormats.DWORD;
                    break;
                case "DWL":
                    TempMemoryArea = MemoryAreas.L;
                    TempDataFormat = DataFormats.DWORD;
                    break;

                case "LD":
                    TempMemoryArea = MemoryAreas.LD;
                    TempDataFormat = DataFormats.WORD;
                    break;
                case "DT":
                    TempMemoryArea = MemoryAreas.DT;
                    TempDataFormat = DataFormats.WORD;
                    break;
                case "EV":
                    TempMemoryArea = MemoryAreas.EV;
                    TempDataFormat = DataFormats.WORD;
                    break;
                case "FL":
                    TempMemoryArea = MemoryAreas.FL;
                    TempDataFormat = DataFormats.WORD;
                    break;
                case "SV":
                    TempMemoryArea = MemoryAreas.SV;
                    TempDataFormat = DataFormats.WORD;
                    break;

                case "DLD":
                    TempMemoryArea = MemoryAreas.LD;
                    TempDataFormat = DataFormats.DWORD;
                    break;
                case "DDT":
                    TempMemoryArea = MemoryAreas.DT;
                    TempDataFormat = DataFormats.DWORD;
                    break;
                case "DEV":
                    TempMemoryArea = MemoryAreas.EV;
                    TempDataFormat = DataFormats.DWORD;
                    break;
                case "DFL":
                    TempMemoryArea = MemoryAreas.FL;
                    TempDataFormat = DataFormats.DWORD;
                    break;
                case "DSV":
                    TempMemoryArea = MemoryAreas.SV;
                    TempDataFormat = DataFormats.DWORD;
                    break;
                
                default:
                    return ;
            }


            string AddressH = TagNameMatch.Groups["AddressH"].Value;
            string AddressL = TagNameMatch.Groups["AddressL"].Value;
            if (AddressL.Length != 1)
                return;
            if (AddressH.Length < 1)
                AddressH = "0";

            try
            {
                switch (TempDataFormat)
                {
                    case DataFormats.BOOL:
                        TempBitNumber = Convert.ToInt32(AddressL.ToString(), 16);
                        TempValue = Convert.ToUInt32(AddressH);
                        break;

                    case DataFormats.WORD:
                    case DataFormats.DWORD:
                        TempValue = Convert.ToUInt32(AddressH + AddressL);
                        break;
                    default:
                        return;
                }
            }
            catch
            {
                return;
            }


            _MemoryArea = TempMemoryArea;
            _DataFormat = TempDataFormat;
            _StartAddress.USHORT = (ushort)TempValue;
            _BitNumber = (byte)TempBitNumber;
            _IsValid = true;
        }

        #endregion

        #region Properties


        /// <summary>
        /// Memory Area
        /// </summary>
        private MemoryAreas _MemoryArea;
        public MemoryAreas MemoryArea
        {
            get
            {
                return _MemoryArea;
            }
        }

        /// <summary>
        /// Data Format
        /// </summary>
        private DataFormats _DataFormat;
        public DataFormats DataFormat
        {
            get
            {
                return _DataFormat;
            }
        }

        /// <summary>
        /// Start address
        /// </summary>
        private ushortUnion _StartAddress;
        public ushortUnion StartAddress
        {
            get
            {
                return _StartAddress;
            }
            set
            {
                _StartAddress = value;
            }
        }


        /// <summary>
        /// Bit Number
        /// </summary>
        private byte _BitNumber;
        public byte BitNumber
        {
            get
            {
                return _BitNumber;
            }
        }

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

        #endregion

    }
}
