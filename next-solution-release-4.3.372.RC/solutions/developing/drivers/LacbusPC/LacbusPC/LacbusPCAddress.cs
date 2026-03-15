using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;
using DriverBaseInterfaces;
using Opc.Ua;

namespace LacbusPC
{
    public enum LacbusPCDataArea : byte
    {
        DataArea_I = 0,
        DataArea_Q = 1,
        DataArea_M = 2,
        DataArea_Invalid = 255
    }

    public enum LacbusPCDataFormat : byte
    {
        DataFormat_Bit = 0,
        DataFormat_Byte = 1,
        DataFormat_Word = 2,
        DataFormat_DWord = 3,
        DataFormat_SINT = 4,
        DataFormat_INT = 5,
        DataFormat_DINT = 6,
        DataFormat_REAL = 7,
        DataFormat_LREAL = 8,
        DataFormat_ARRAY = 9,
        DataFormat_STRING = 10,
        DataFormat_Invalid = 255
    }

    public enum LacbusPCOffsets : uint
    {
        Offset_IB = 0x1F400,
        Offset_IX = 0xFA000,
        Offset_QB = 0x3E800,
        Offset_QX = 0x1F4000
    }

    public class LacbusPCAddress : Object
    {
        #region Constructors

        /// <summary>
        /// Initializes the LacbusPCAddress object with default values.
        /// </summary>
        public LacbusPCAddress()
        {
            _LacbusPCVersion = (byte)LacbusPCVersions.Version2x;
            ResetAddress();
        }

        /// <summary>
        /// Initializes the LacbusPCAddress object parsing the assigned address.
        /// </summary>
        public LacbusPCAddress(string tcAddress, byte tcVersion = (byte)LacbusPCVersions.Version2x)
        {
            LacbusPCVersion = tcVersion;
            Parse(tcAddress);
        }

        #endregion

        #region Methods

        void ResetAddress()
        {
            _Address = String.Empty;
            _IsValid = false;
            _DataArea = LacbusPCDataArea.DataArea_Invalid;
            _DataFormat = LacbusPCDataFormat.DataFormat_Invalid;
            _IsNumeric = true;
            _ElementNumber = 0;
            _ByteElementNumber = 0;
            _BitNumber = 0;
            _IndexGroup = 0;
            _IndexOffset = 0;
            _StartDynByteElementNumber = 0;
        }

        public void SetLacbusPCVersion(byte tcVersion)
        {
            if(((tcVersion != (byte)LacbusPCVersions.Version2x) &&
                (tcVersion != (byte)LacbusPCVersions.Version3x)) ||
                (tcVersion == LacbusPCVersion))
            {
                return;
            }
            LacbusPCVersion = tcVersion;
            Parse(Address);
        }

        /// <summary>
        /// Parse a numeric address.
        /// </summary>
        void Parse(string tcAddress)
        {
            string TempAddress = String.Copy(tcAddress);
            ResetAddress();

            if (String.IsNullOrWhiteSpace(TempAddress))
            {
                return;
            }

            TempAddress.Trim();

            if (TempAddress[0] != '%')
            {
                _IsNumeric = false;
                _Address = String.Copy(TempAddress);
                _IsValid = true;
                return;
            }

            if (TempAddress.Length < 3)
            {
                return;
            }

            // Parse Data Area
            string upperString = TempAddress.ToUpper();
            TempAddress = upperString;
            if ((TempAddress[1] != 'I') && (TempAddress[1] != 'Q') &&
                (TempAddress[1] != 'M'))
            {
                return;
            }

            LacbusPCDataArea TempDataArea;
            if (TempAddress[1] == 'I')
            {
                TempDataArea = LacbusPCDataArea.DataArea_I;
            }
            else if (TempAddress[1] == 'Q')
            {
                TempDataArea = LacbusPCDataArea.DataArea_Q;
            }
            else
            {
                TempDataArea = LacbusPCDataArea.DataArea_M;
            }

            // Parse Data Format
            LacbusPCDataFormat TempDataFormat;
            int SearchIndex = 2;
            if (TempAddress[2] == 'B')
            {
                TempDataFormat = LacbusPCDataFormat.DataFormat_Byte;
                SearchIndex++;
            }
            else if (TempAddress[2] == 'W')
            {
                TempDataFormat = LacbusPCDataFormat.DataFormat_Word;
                SearchIndex++;
            }
            else if (TempAddress[2] == 'D')
            {
                TempDataFormat = LacbusPCDataFormat.DataFormat_DWord;
                SearchIndex++;
            }
            else if (TempAddress[2] == 'X')
            {
                TempDataFormat = LacbusPCDataFormat.DataFormat_Bit;
                SearchIndex++;
            }
            else
            {
                TempDataFormat = LacbusPCDataFormat.DataFormat_Bit;
            }

            // Skip white spaces
            while ((SearchIndex < TempAddress.Length) &&
                   (TempAddress[SearchIndex] == ' '))
            {
                SearchIndex++;
            }

            if (SearchIndex >= TempAddress.Length)
            {
                return;
            }

            // Look for the character '.'
            int DotIndex = TempAddress.IndexOf('.', SearchIndex);
            if (TempDataFormat == LacbusPCDataFormat.DataFormat_Bit)
            {
                if (DotIndex <= SearchIndex)
                {
                    return;
                }
            }
            else
            {
                if (DotIndex != -1)
                {
                    return;
                }
                DotIndex = TempAddress.Length;
            }

            // Parse byte offset
            string ByteOffsetString = TempAddress.Substring(
                                                       SearchIndex,
                                                       DotIndex - SearchIndex);
            int TempElementNumber = 0;
            if (!int.TryParse(ByteOffsetString, out TempElementNumber) ||
                (TempElementNumber < 0))
            {
                return;
            }
            int TempByteElementNumber = TempElementNumber;
            if(LacbusPCVersion == (byte)LacbusPCVersions.Version3x)
            {
                switch (TempDataFormat)
                {
                    case LacbusPCDataFormat.DataFormat_Word:
                        TempByteElementNumber *= 2;
                        break;

                    case LacbusPCDataFormat.DataFormat_DWord:
                        TempByteElementNumber *= 4;
                        break;
                }
            }

            // Parse bit offset
            int TempBitNumber = 0;
            if (TempDataFormat == LacbusPCDataFormat.DataFormat_Bit)
            {
                if ((DotIndex + 1) >= TempAddress.Length)
                {
                    return;
                }
                string BitOffsetString = TempAddress.Substring(
                                                DotIndex + 1,
                                                TempAddress.Length - DotIndex -1);
                if (!int.TryParse(BitOffsetString, out TempBitNumber) ||
                    (TempBitNumber < 0))
                {
                    return;
                }
            }

            // Index group and index offset (absolute address)
            int TempIndexGroup = 0;
            int TempIndexOffset = 0;
            if (LacbusPCVersion == (byte)LacbusPCVersions.Version3x)
            {
                switch (TempDataArea)
                {
                    case LacbusPCDataArea.DataArea_I:
                        if (TempDataFormat == LacbusPCDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup =
                                           (int)AdsReservedIndexGroups.IOImageRWIX;
                            TempIndexOffset = (int)LacbusPCOffsets.Offset_IX +
                                              TempByteElementNumber * 8 + TempBitNumber;
                        }
                        else
                        {
                            TempIndexGroup =
                                           (int)AdsReservedIndexGroups.IOImageRWIB;
                            TempIndexOffset = (int)LacbusPCOffsets.Offset_IB +
                                              TempByteElementNumber;
                        }
                        break;

                    case LacbusPCDataArea.DataArea_M:
                        if (TempDataFormat == LacbusPCDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup =
                                           (int)AdsReservedIndexGroups.PlcRWMB + 1;
                            TempIndexOffset =
                                            TempByteElementNumber * 8 + TempBitNumber;
                        }
                        else
                        {
                            TempIndexGroup =
                                           (int)AdsReservedIndexGroups.PlcRWMB;
                            TempIndexOffset = TempByteElementNumber;
                        }
                        break;

                    case LacbusPCDataArea.DataArea_Q:
                        if (TempDataFormat == LacbusPCDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup =
                                           (int)AdsReservedIndexGroups.IOImageRWOX;
                            TempIndexOffset = (int)LacbusPCOffsets.Offset_QX +
                                              TempByteElementNumber * 8 + TempBitNumber;
                        }
                        else
                        {
                            TempIndexGroup =
                                           (int)AdsReservedIndexGroups.IOImageRWOB;
                            TempIndexOffset = (int)LacbusPCOffsets.Offset_QB + TempByteElementNumber;
                        }
                        break;

                    default:
                        return;
                }
            }
            else // LacbusPCVersions.Version2x
            {
                switch (TempDataArea)
                {
                    case LacbusPCDataArea.DataArea_I:
                        if (TempDataFormat == LacbusPCDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup =
                                       (int)AdsReservedIndexGroups.IOImageRWIX;
                            TempIndexOffset =
                                       TempElementNumber * 8 + TempBitNumber;
                        }
                        else
                        {
                            TempIndexGroup =
                                       (int)AdsReservedIndexGroups.IOImageRWIB;
                            TempIndexOffset = TempElementNumber;
                        }
                        break;

                    case LacbusPCDataArea.DataArea_M:
                        if (TempDataFormat == LacbusPCDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup =
                                       (int)AdsReservedIndexGroups.PlcRWMB + 1;
                            TempIndexOffset =
                                       TempElementNumber * 8 + TempBitNumber;
                        }
                        else
                        {
                            TempIndexGroup =
                                           (int)AdsReservedIndexGroups.PlcRWMB;
                            TempIndexOffset = TempElementNumber;
                        }
                        break;

                    case LacbusPCDataArea.DataArea_Q:
                        if (TempDataFormat == LacbusPCDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup =
                                       (int)AdsReservedIndexGroups.IOImageRWOX;
                            TempIndexOffset = 
                                       TempElementNumber * 8 + TempBitNumber;
                        }
                        else
                        {
                            TempIndexGroup =
                                       (int)AdsReservedIndexGroups.IOImageRWOB;
                            TempIndexOffset = TempElementNumber;
                        }
                        break;

                    default:
                        return;
                }
            }

            // Valid address --> set it
            _Address = TempAddress;
            _IsValid = true;
            _DataArea = TempDataArea;
            _DataFormat = TempDataFormat;
            _IsNumeric = true;
            _ElementNumber = TempElementNumber;
            _ByteElementNumber = TempByteElementNumber;
            _BitNumber = TempBitNumber;
            _IndexGroup = TempIndexGroup;
            _IndexOffset = TempIndexOffset;
        }

        /// <summary>
        /// Set a new numeric address.
        /// </summary>

        public void Set(string tcAddress, byte tcVersion = (byte)LacbusPCVersions.Version2x)
        {
            LacbusPCVersion = tcVersion;
            Parse(tcAddress);
        }

        private void SetIndexGroupAndOffset()
        {
            int TempIndexGroup = 0;
            int TempIndexOffset = 0;
            if (LacbusPCVersion == (byte)LacbusPCVersions.Version3x)
            {
                switch (DataArea)
                {
                    case LacbusPCDataArea.DataArea_I:
                        if (DataFormat == LacbusPCDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.IOImageRWIX;
                            TempIndexOffset = (int)LacbusPCOffsets.Offset_IX +
                                              ByteElementNumber * 8 + BitNumber;
                        }
                        else
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.IOImageRWIB;
                            TempIndexOffset = (int)LacbusPCOffsets.Offset_IB + ByteElementNumber;
                        }
                        break;

                    case LacbusPCDataArea.DataArea_M:
                        if (DataFormat == LacbusPCDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.PlcRWMB + 1;
                            TempIndexOffset = ByteElementNumber * 8 + BitNumber;
                        }
                        else
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.PlcRWMB;
                            TempIndexOffset = ByteElementNumber;
                        }
                        break;

                    case LacbusPCDataArea.DataArea_Q:
                        if (DataFormat == LacbusPCDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.IOImageRWOX;
                            TempIndexOffset = (int)LacbusPCOffsets.Offset_QX +
                                              ByteElementNumber * 8 + BitNumber;
                        }
                        else
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.IOImageRWOB;
                            TempIndexOffset = (int)LacbusPCOffsets.Offset_QB + ByteElementNumber;
                        }
                        break;

                    default:
                        return;
                }
            }
            else
            {
                switch (DataArea)
                {
                    case LacbusPCDataArea.DataArea_I:
                        if (DataFormat == LacbusPCDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.IOImageRWIX;
                            TempIndexOffset = ElementNumber * 8 + BitNumber;
                        }
                        else
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.IOImageRWIB;
                            TempIndexOffset = ElementNumber;
                        }
                        break;

                    case LacbusPCDataArea.DataArea_M:
                        if (DataFormat == LacbusPCDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.PlcRWMB + 1;
                            TempIndexOffset = ElementNumber * 8 + BitNumber;
                        }
                        else
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.PlcRWMB;
                            TempIndexOffset = ElementNumber;
                        }
                        break;

                    case LacbusPCDataArea.DataArea_Q:
                        if (DataFormat == LacbusPCDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.IOImageRWOX;
                            TempIndexOffset = ElementNumber * 8 + BitNumber;
                        }
                        else
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.IOImageRWOB;
                            TempIndexOffset = ElementNumber;
                        }
                        break;

                    default:
                        return;
                }
            }

            _IndexGroup = TempIndexGroup;
            _IndexOffset = TempIndexOffset;
        }

        private void RebuildAddressString()
        {
            string TempAddress = String.Copy(Address);
            TempAddress.Trim();
            if (TempAddress.Length < 3)
            {
                return;
            }

            string NewAddressString = String.Empty;
            NewAddressString = TempAddress.Substring(0, 2);

            switch (_DataFormat)
            {
                case LacbusPCDataFormat.DataFormat_Bit:
                    NewAddressString += "X";
                break;

                case LacbusPCDataFormat.DataFormat_Byte:
                    NewAddressString += "B";
                break;

                case LacbusPCDataFormat.DataFormat_Word:
                    NewAddressString += "W";
                break;

                default:
                    NewAddressString += "D";
                break;
            }

            NewAddressString += ElementNumber.ToString();

            if (_DataFormat == LacbusPCDataFormat.DataFormat_Bit)
            {
                NewAddressString += ".";
                NewAddressString += BitNumber.ToString();
            }

            _Address = NewAddressString;
        }

        public void GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            if (!_IsValid)
            {
                return;
            }
            if (_Address.Length < 3)
            {
                return;
            }
            if (!_IsNumeric)
            {
                return;
            }
            if (thistagdefinition.DataType.IdType != IdType.Numeric || prevtagdefinition.DataType.IdType != IdType.Numeric)
            {
                return;
            }

            if (LacbusPCVersion == (byte)LacbusPCVersions.Version3x)
            {
                switch ((uint)prevtagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                    case (uint)BuiltInType.Byte:
                    case (uint)BuiltInType.SByte:
                        _ByteElementNumber += (prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1);
                        break;

                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        _ByteElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 2;
                        break;

                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        _ByteElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 4;
                        break;
                }

                switch ((uint)thistagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        _ByteElementNumber += (_ByteElementNumber - _StartDynByteElementNumber) % 2;
                        break;

                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        if (((_ByteElementNumber - _StartDynByteElementNumber) % 4) > 0)
                        {
                            _ByteElementNumber += 4 - ((_ByteElementNumber - _StartDynByteElementNumber) % 4);
                        }
                        break;
                }

                _ElementNumber = _ByteElementNumber;
                _BitNumber = 0;
                _DataFormat = LacbusPCDataFormat.DataFormat_Byte;
            }
            else
            {
                switch ((uint)thistagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        switch(DataFormat)
                        {
                            case LacbusPCDataFormat.DataFormat_Bit:
                                _BitNumber += (prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1);
                                _ElementNumber += _BitNumber / 8;
                                _BitNumber &= 8;
                                break;

                            case LacbusPCDataFormat.DataFormat_Byte:
                                _BitNumber = 0;
                                _ElementNumber += (prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1);
                                DataFormat = LacbusPCDataFormat.DataFormat_Bit;
                                break;

                            case LacbusPCDataFormat.DataFormat_Word:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 2;
                                DataFormat = LacbusPCDataFormat.DataFormat_Bit;
                                break;

                            default:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 4;
                                DataFormat = LacbusPCDataFormat.DataFormat_Bit;
                                break;
                        }
                        break;

                    case (uint)BuiltInType.Byte:
                    case (uint)BuiltInType.SByte:
                        switch(DataFormat)
                        {
                            case LacbusPCDataFormat.DataFormat_Bit:
                                _BitNumber = 0;
                                if (prevtagdefinition.ArrayDimension > 0)
                                {
                                    _ElementNumber += (int)prevtagdefinition.ArrayDimension / 8;
                                    if((prevtagdefinition.ArrayDimension%8) > 0)
                                    {
                                        _ElementNumber++;
                                    }
                                }
                                else
                                {
                                    _ElementNumber++;
                                }
                                DataFormat = LacbusPCDataFormat.DataFormat_Byte;
                                break;

                            case LacbusPCDataFormat.DataFormat_Byte:
                                _BitNumber = 0;
                                _ElementNumber += (prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1);
                                break;

                            case LacbusPCDataFormat.DataFormat_Word:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 2;
                                DataFormat = LacbusPCDataFormat.DataFormat_Byte;
                                break;

                            default:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 4;
                                DataFormat = LacbusPCDataFormat.DataFormat_Byte;
                                break;
                        }
                        break;

                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        switch(DataFormat)
                        {
                            case LacbusPCDataFormat.DataFormat_Bit:
                                _BitNumber = 0;
                                if (prevtagdefinition.ArrayDimension > 0)
                                {
                                    _ElementNumber += (int)prevtagdefinition.ArrayDimension / 8;
                                    if((prevtagdefinition.ArrayDimension%8) > 0)
                                    {
                                        _ElementNumber++;
                                    }
                                }
                                else
                                {
                                    _ElementNumber++;
                                }
                                if(_ElementNumber%2 > 0)
                                {
                                    _ElementNumber++;
                                }
                                DataFormat = LacbusPCDataFormat.DataFormat_Word;
                                break;

                            case LacbusPCDataFormat.DataFormat_Byte:
                                _BitNumber = 0;
                                _ElementNumber += (prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1);
                                if(_ElementNumber%2 > 0)
                                {
                                    _ElementNumber++;
                                }
                                DataFormat = LacbusPCDataFormat.DataFormat_Word;
                                break;

                            case LacbusPCDataFormat.DataFormat_Word:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 2;
                                if(_ElementNumber%2 > 0)
                                {
                                    _ElementNumber++;
                                }
                                break;

                            default:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 4;
                                if(_ElementNumber%2 > 0)
                                {
                                    _ElementNumber++;
                                }
                                DataFormat = LacbusPCDataFormat.DataFormat_Word;
                                break;
                        }
                        break;

                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        switch(DataFormat)
                        {
                            case LacbusPCDataFormat.DataFormat_Bit:
                                _BitNumber = 0;
                                if (prevtagdefinition.ArrayDimension > 0)
                                {
                                    _ElementNumber += (int)prevtagdefinition.ArrayDimension / 8;
                                    if((prevtagdefinition.ArrayDimension%8) > 0)
                                    {
                                        _ElementNumber++;
                                    }
                                }
                                else
                                {
                                    _ElementNumber++;
                                }
                                if(_ElementNumber%2 > 0)
                                {
                                    _ElementNumber++;
                                }
                                DataFormat = LacbusPCDataFormat.DataFormat_DWord;
                                break;

                            case LacbusPCDataFormat.DataFormat_Byte:
                                _BitNumber = 0;
                                _ElementNumber += (prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1);
                                if(_ElementNumber%2 > 0)
                                {
                                    _ElementNumber++;
                                }
                                DataFormat = LacbusPCDataFormat.DataFormat_DWord;
                                break;

                            case LacbusPCDataFormat.DataFormat_Word:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 2;
                                if(_ElementNumber%2 > 0)
                                {
                                    _ElementNumber++;
                                }
                                DataFormat = LacbusPCDataFormat.DataFormat_DWord;
                                break;

                            default:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 4;
                                if(_ElementNumber%2 > 0)
                                {
                                    _ElementNumber++;
                                }
                                DataFormat = LacbusPCDataFormat.DataFormat_DWord;
                                break;
                        }
                        break;

                    default:
                        return;
                }
                _ByteElementNumber = _ElementNumber;
            }

            RebuildAddressString();
            SetIndexGroupAndOffset();
        }

        public void SetFirstDynSettings()
        {
            _BitNumber = 0;
            _ElementNumber = _ByteElementNumber;
            _StartDynByteElementNumber = _ByteElementNumber;
            _DataFormat = LacbusPCDataFormat.DataFormat_Byte;
            RebuildAddressString();
            SetIndexGroupAndOffset();
        }
        #endregion

        #region Properties

        /// <summary>
        /// LacbusPC address in string format
        /// </summary>
        private string _Address;
        public string Address
        {
            get
            {
                return _Address;
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

            set
            {
                _IsValid = value;
            }
        }

        /// <summary>
        /// Data area
        /// </summary>
        private LacbusPCDataArea _DataArea;
        public LacbusPCDataArea DataArea
        {
            get
            {
                return _DataArea;
            }
        }

        /// <summary>
        /// Data format
        /// </summary>
        private LacbusPCDataFormat _DataFormat;
        public LacbusPCDataFormat DataFormat
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
        /// Property that indicates if an address is numeric (e.g. %M100)
        /// </summary>
        private bool _IsNumeric;
        public bool IsNumeric
        {
            get
            {
                return _IsNumeric;
            }
        }

        /// <summary>
        /// Offset of the address
        /// </summary>
        private int _ElementNumber;
        public int ElementNumber
        {
            get
            {
                return _ElementNumber;
            }
        }

        /// <summary>
        /// Offset in bytes of the address
        /// </summary>
        private int _ByteElementNumber;
        public int ByteElementNumber
        {
            get
            {
                return _ByteElementNumber;
            }
        }

        /// <summary>
        /// Bit offset of the address
        /// </summary>
        private int _BitNumber;
        public int BitNumber
        {
            get
            {
                return _BitNumber;
            }
        }

        /// <summary>
        /// Index Group
        /// </summary>
        private int _IndexGroup;
        public int IndexGroup
        {
            get
            {
                return _IndexGroup;
            }

            set
            {
                _IndexGroup = value;
            }
        }

        /// <summary>
        /// Index Offset
        /// </summary>
        private int _IndexOffset;
        public int IndexOffset
        {
            get
            {
                return _IndexOffset;
            }

            set
            {
                _IndexOffset = value;
            }
        }

        /// <summary>
        /// First offset in bytes when splitting structures
        /// </summary>
        private int _StartDynByteElementNumber;
        public int StartDynByteElementNumber
        {
            get
            {
                return _StartDynByteElementNumber;
            }
        }

        private byte _LacbusPCVersion;
        public byte LacbusPCVersion
        {
            get { return _LacbusPCVersion; }
            set
            {
                _LacbusPCVersion = value;
            }
        }

        #endregion
    }
}
