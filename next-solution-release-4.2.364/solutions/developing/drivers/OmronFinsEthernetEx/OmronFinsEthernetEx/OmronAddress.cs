using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using System.ComponentModel;
using System.Text.RegularExpressions;
using DriverBaseInterfaces;
using Opc.Ua;

namespace OmronFinsEthernet
{
    
    public class OmronAddress
    {
        #region Constructors

        /// <summary>
        /// Initializes the OmronAddress object with default values.
        /// </summary>
        public OmronAddress()
        {
            Reset();
        }

        /// <summary>
        /// Initializes the OmronAddress object parsing the assigned address.
        /// </summary>
        public OmronAddress(string inAddress)
        {
            Parse(inAddress);
        }

        /// <summary>
        /// Initializes the OmronAddress object parsing the assigned address and add offset.
        /// </summary>
        public OmronAddress(string inAddress, TagDefinition prevtagdefinition)
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
                            case DataFormats.Bit:
                                ushort bitIndex = (ushort)(_BitNumber + ArrayDimension);
                                _Address.USHORT += (ushort)(bitIndex / 16);
                                _BitNumber = (sbyte)(bitIndex % 16);
                                break;
                            case DataFormats.Word:
                                _Address.USHORT += ArrayDimension;
                                break;
                        }
                        break;
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        _Address.USHORT += ArrayDimension;
                        if(_DataFormat == DataFormats.Bit)
                            _BitNumber = 0;
                        break;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        _Address.USHORT += (ushort)(2 * ArrayDimension);
                        if(_DataFormat == DataFormats.Bit)
                            _BitNumber = 0;
                        break;
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        _Address.USHORT += (ushort)(4 * ArrayDimension);
                        if(_DataFormat == DataFormats.Bit)
                            _BitNumber = 0;
                        break;
                    case (uint)BuiltInType.String:
                        _Address.USHORT += (ushort)ArrayDimension;
                        break;
                }
            }
        }


        #endregion
        #region Methods

        void Reset()
        {
            _IsValid = false;
            _DataArea = DataAreas.Invalid;
            _DataFormat = DataFormats.Invalid;
            _Address = new ushortUnion(0);
            _BankNumber = -1;
            _BitNumber = -1;
            _StringLength = -1;
        }

        /// <summary>
        /// Set a new address.
        /// </summary>

        public void Set(string Address)
        {
            Parse(Address);
        }
  
        public string Get()
        {
            if(!_IsValid)
                return string.Empty;

            var dynamicstring = new StringBuilder();
            switch (_DataArea)
            {
                case DataAreas.IR:
                    dynamicstring.Append("IR");
                    break;
                case DataAreas.HR:
                   dynamicstring.Append("HR");
                    break;
                case DataAreas.AR:
                    dynamicstring.Append("AR");
                    break;
                case DataAreas.DM:
                    dynamicstring.Append("DM");
                    break;
                case DataAreas.T:
                    dynamicstring.Append("T");
                    break;
                case DataAreas.C:
                    dynamicstring.Append("C");
                    break;
                case DataAreas.EM:
                    dynamicstring.Append("EM");
                    break;
                case DataAreas.W:
                    dynamicstring.Append("W");
                    break;
                default:
                    return string.Empty;
            }
            if(_BankNumber>=0)
                dynamicstring.AppendFormat("{0:X}_", _BankNumber);
            dynamicstring.AppendFormat("{0}", _Address.USHORT);
            if(_DataFormat == DataFormats.Bit)
                dynamicstring.AppendFormat(".{0}", _BitNumber);
            else if (_DataFormat == DataFormats.String)
            {
                dynamicstring.AppendFormat(":{0}", _StringLength);
            }

            return dynamicstring.ToString();
        }

        public string GetFormattedaddress()
        {
            if (!_IsValid)
                return string.Empty;

            var dynamicstring = new StringBuilder();
            switch (_DataArea)
            {
                case DataAreas.IR:
                    dynamicstring.Append("IR");
                    break;
                case DataAreas.HR:
                    dynamicstring.Append("HR");
                    break;
                case DataAreas.AR:
                    dynamicstring.Append("AR");
                    break;
                case DataAreas.DM:
                    dynamicstring.Append("DM");
                    break;
                case DataAreas.T:
                    dynamicstring.Append("T");
                    break;
                case DataAreas.C:
                    dynamicstring.Append("C");
                    break;
                case DataAreas.EM:
                    dynamicstring.Append("EM");
                    break;
                case DataAreas.W:
                    dynamicstring.Append("W");
                    break;
                default:
                    return string.Empty;
            }
            if (_BankNumber >= 0)
                dynamicstring.AppendFormat("{0:X}", _BankNumber);
            dynamicstring.AppendFormat("{0}", _Address.USHORT.ToString().PadLeft(5,'0'));
            if (_DataFormat == DataFormats.Bit)
                dynamicstring.AppendFormat(".{0}", _BitNumber.ToString().PadLeft(3,'0'));

            return dynamicstring.ToString();
        }

        public bool isBit()
        {
            return (DataFormat == DataFormats.Bit);
        }

        /// <summary>
        /// Parse an address.
        /// </summary>
        void Parse(string Address)
        {
            Reset();
            DataAreas TempDataArea;
            DataFormats TempDataFormat = DataFormats.Word;
            int TempBankNumber = -1;
            int TempBitNumber = -1;
            int TempStringLength = -1;
            
            string TempAddress = String.Copy(Address).ToUpper().Trim();

            // Parse Data Area
            Regex TagNameParser = new Regex(@"^(?<DataArea>[A-Z]{1,3})");
            Match TagNameMatch = TagNameParser.Match(TempAddress);
            if (!TagNameMatch.Success)
                return;

            // check if is a Bank address
            if (TagNameMatch.Groups["DataArea"].Value.IndexOf("EM") == 0)
            {
                TempDataArea = DataAreas.EM;
                TempAddress = TempAddress.Substring(2, TempAddress.Length - 2);
            }
            else if (TagNameMatch.Groups["DataArea"].Value.IndexOf("E") == 0)
            {
                TempDataArea = DataAreas.EM;
                TempAddress = TempAddress.Substring(1, TempAddress.Length - 1);
            }
            else 
            {
                switch (TagNameMatch.Groups["DataArea"].Value)
                {
                    case "CIO":
                    case "IR":
                    case "I":
                        TempDataArea = DataAreas.IR;
                        break;
                    case "HR":
                    case "H":
                        TempDataArea = DataAreas.HR;
                        break;
                    case "AR":
                    case "A":
                        TempDataArea = DataAreas.AR;
                        break;
                    case "DM":
                    case "D":
                        TempDataArea = DataAreas.DM;
                        break;
                    case "TIM":
                    case "T":
                        TempDataArea = DataAreas.T;
                        break;
                    case "CNT":
                    case "C":
                        TempDataArea = DataAreas.C;
                        break;
                    case "WR":
                    case "W":
                        TempDataArea = DataAreas.W;
                        break;
                    default:
                        return;
                }

                TempAddress = TempAddress.Substring(TagNameMatch.Groups["DataArea"].Value.Length, TempAddress.Length - TagNameMatch.Groups["DataArea"].Value.Length);
            }


            // Parse Data Number/address
            TagNameParser = new Regex(@"^(?<BankNumber>(^|[0-9A-F]+)_)?(?<Address>\d{1,5})(?<BitNumber>\.\d{1,2})?(?<StringLength>\:\d{1,3})?$");
            TagNameMatch = TagNameParser.Match(TempAddress);
            if (!TagNameMatch.Success)
                return;

            uint TempValue = Convert.ToUInt32(TagNameMatch.Groups["Address"].Value);
            if (TempValue > 0xFFFF)
            {
                return;
            }

            if (TagNameMatch.Groups["BitNumber"].Value.Length > 1)
            {
                TempBitNumber = Convert.ToInt32(TagNameMatch.Groups["BitNumber"].Value.Remove(0, 1));
                if (TempBitNumber > 15)
                {
                    return;
                }
                
                switch (TempDataArea)
                {
                    case DataAreas.T:
                    case DataAreas.C:
                        return;
                    default:
                        TempDataFormat = DataFormats.Bit;
                        break;
                }
            }

            if (TagNameMatch.Groups["BankNumber"].Value.Length != 0)
            {
                if (TempDataArea != DataAreas.EM)
                    return;

                string address = TagNameMatch.Groups["BankNumber"].Value.Remove(TagNameMatch.Groups["BankNumber"].Value.Length - 1, 1);
                // emptry string = Current DataBlock
                if (string.IsNullOrEmpty(address))
                {
                    TempBankNumber = -1;
                }
                else
                { 
                    if (!Int32.TryParse(address, System.Globalization.NumberStyles.HexNumber, null, out Int32 dummy))
                        return;

                    if (dummy > OmronFinsEthernetProtocol.MAX_EM_NR)
                        return;

                    TempBankNumber = dummy;
                }
            }

            if (TagNameMatch.Groups["StringLength"].Value.Length > 1)
            {
                if (TempDataFormat == DataFormats.Bit)
                {
                    return;
                }
                switch (TempDataArea)
                {
                    case DataAreas.T:
                    case DataAreas.C:
                        return;
                    default:
                        try
                        {
                            TempStringLength = Convert.ToInt32(TagNameMatch.Groups["StringLength"].Value.Remove(0, 1));
                            TempDataFormat = DataFormats.String;
                        }
                        catch(Exception ex)
                        {
                            return;
                        }
                        break;
                }
            }

            _DataArea = TempDataArea;
            _DataFormat = TempDataFormat;
            _Address.USHORT = (ushort)TempValue;
            _BankNumber = TempBankNumber;
            _BitNumber = (sbyte)TempBitNumber;
            _IsValid = true;
            _StringLength = TempStringLength;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Data area
        /// </summary>
        private DataAreas _DataArea;
        public DataAreas DataArea
        {
            get
            {
                return _DataArea;
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

            set
            {
                _DataFormat = value;
            }
        }

        /// <summary>
        /// Start address
        /// </summary>
        private ushortUnion _Address;
        public ushortUnion Address
        {
            get
            {
                return _Address;
            }
            set
            {
                _Address = value;
            }
        }

        /// <summary>
        /// Bank Number
        /// </summary>
        private int _BankNumber;
        public int BankNumber
        {
            get
            {
                return _BankNumber;
            }
        }

        /// <summary>
        /// Bit Number
        /// </summary>
        private sbyte _BitNumber;
        public sbyte BitNumber
        {
            get
            {
                return _BitNumber;
            }

            set
            {
                _BitNumber = value;
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

        #endregion

    }
}
