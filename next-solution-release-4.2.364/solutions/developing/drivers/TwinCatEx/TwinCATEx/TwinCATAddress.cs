using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TwinCAT.Ads;
using DriverBaseInterfaces;
using Opc.Ua;

namespace TwinCAT
{
    public enum TwinCATDataArea : byte
    {
        DataArea_I = 0,
        DataArea_Q = 1,
        DataArea_M = 2,
        DataArea_Invalid = 255
    }

    public enum TwinCATDataFormat : byte
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
        DataFormat_LINT = 11,
        DataFormat_ULINT = 12,
        DataFormat_Invalid = 255
    }

    public enum TwinCATOffsets : uint
    {
        Offset_IB = 0x1F400,
        Offset_IX = 0xFA000,
        Offset_QB = 0x3E800,
        Offset_QX = 0x1F4000
    }

    public class TwinCATAddress : Object
    {
        #region Constructors

        /// <summary>
        /// Initializes the TwinCATAddress object with default values.
        /// </summary>
        public TwinCATAddress()
        {
            _TwinCATVersion = (byte)TwinCATVersions.Version2x;
            ResetAddress();
        }

        /// <summary>
        /// Initializes the TwinCATAddress object parsing the assigned address.
        /// </summary>
        public TwinCATAddress(string tcAddress, byte tcVersion = (byte)TwinCATVersions.Version2x)
        {
            TwinCATVersion = tcVersion;
            Parse(tcAddress);
        }

        #endregion

        #region Methods

        void ResetAddress()
        {
            _Address = String.Empty;
            _IsValid = false;
            _DataArea = TwinCATDataArea.DataArea_Invalid;
            _DataFormat = TwinCATDataFormat.DataFormat_Invalid;
            _IsNumeric = true;
            _ElementNumber = 0;
            _ByteElementNumber = 0;
            _BitNumber = 0;
            _IndexGroup = 0;
            _IndexOffset = 0;
            _StartDynByteElementNumber = 0;
        }

        public void SetTwinCATVersion(byte tcVersion)
        {
            if(((tcVersion != (byte)TwinCATVersions.Version2x) &&
                (tcVersion != (byte)TwinCATVersions.Version3x)) ||
                (tcVersion == TwinCATVersion))
            {
                return;
            }
            TwinCATVersion = tcVersion;
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

            TwinCATDataArea TempDataArea;
            if (TempAddress[1] == 'I')
            {
                TempDataArea = TwinCATDataArea.DataArea_I;
            }
            else if (TempAddress[1] == 'Q')
            {
                TempDataArea = TwinCATDataArea.DataArea_Q;
            }
            else
            {
                TempDataArea = TwinCATDataArea.DataArea_M;
            }

            // Parse Data Format
            TwinCATDataFormat TempDataFormat;
            int SearchIndex = 2;
            if (TempAddress[2] == 'B')
            {
                TempDataFormat = TwinCATDataFormat.DataFormat_Byte;
                SearchIndex++;
            }
            else if (TempAddress[2] == 'W')
            {
                TempDataFormat = TwinCATDataFormat.DataFormat_Word;
                SearchIndex++;
            }
            else if (TempAddress[2] == 'D')
            {
                TempDataFormat = TwinCATDataFormat.DataFormat_DWord;
                SearchIndex++;
            }
            else if (TempAddress[2] == 'X')
            {
                TempDataFormat = TwinCATDataFormat.DataFormat_Bit;
                SearchIndex++;
            }
            else
            {
                TempDataFormat = TwinCATDataFormat.DataFormat_Bit;
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
            if (TempDataFormat == TwinCATDataFormat.DataFormat_Bit)
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
            if(TwinCATVersion == (byte)TwinCATVersions.Version3x)
            {
                switch (TempDataFormat)
                {
                    case TwinCATDataFormat.DataFormat_Word:
                        TempByteElementNumber *= 2;
                        break;

                    case TwinCATDataFormat.DataFormat_DWord:
                        TempByteElementNumber *= 4;
                        break;
                }
            }

            // Parse bit offset
            int TempBitNumber = 0;
            if (TempDataFormat == TwinCATDataFormat.DataFormat_Bit)
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
            if (TwinCATVersion == (byte)TwinCATVersions.Version3x)
            {
                switch (TempDataArea)
                {
                    case TwinCATDataArea.DataArea_I:
                        if (TempDataFormat == TwinCATDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup =
                                           (int)AdsReservedIndexGroups.IOImageRWIX;
                            TempIndexOffset = (int)TwinCATOffsets.Offset_IX +
                                              TempByteElementNumber * 8 + TempBitNumber;
                        }
                        else
                        {
                            TempIndexGroup =
                                           (int)AdsReservedIndexGroups.IOImageRWIB;
                            TempIndexOffset = (int)TwinCATOffsets.Offset_IB +
                                              TempByteElementNumber;
                        }
                        break;

                    case TwinCATDataArea.DataArea_M:
                        if (TempDataFormat == TwinCATDataFormat.DataFormat_Bit)
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

                    case TwinCATDataArea.DataArea_Q:
                        if (TempDataFormat == TwinCATDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup =
                                           (int)AdsReservedIndexGroups.IOImageRWOX;
                            TempIndexOffset = (int)TwinCATOffsets.Offset_QX +
                                              TempByteElementNumber * 8 + TempBitNumber;
                        }
                        else
                        {
                            TempIndexGroup =
                                           (int)AdsReservedIndexGroups.IOImageRWOB;
                            TempIndexOffset = (int)TwinCATOffsets.Offset_QB + TempByteElementNumber;
                        }
                        break;

                    default:
                        return;
                }
            }
            else // TwinCATVersions.Version2x
            {
                switch (TempDataArea)
                {
                    case TwinCATDataArea.DataArea_I:
                        if (TempDataFormat == TwinCATDataFormat.DataFormat_Bit)
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

                    case TwinCATDataArea.DataArea_M:
                        if (TempDataFormat == TwinCATDataFormat.DataFormat_Bit)
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

                    case TwinCATDataArea.DataArea_Q:
                        if (TempDataFormat == TwinCATDataFormat.DataFormat_Bit)
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

        public void Set(string tcAddress, byte tcVersion = (byte)TwinCATVersions.Version2x)
        {
            TwinCATVersion = tcVersion;
            Parse(tcAddress);
        }

        private void SetIndexGroupAndOffset()
        {
            int TempIndexGroup = 0;
            int TempIndexOffset = 0;
            if (TwinCATVersion == (byte)TwinCATVersions.Version3x)
            {
                switch (DataArea)
                {
                    case TwinCATDataArea.DataArea_I:
                        if (DataFormat == TwinCATDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.IOImageRWIX;
                            TempIndexOffset = (int)TwinCATOffsets.Offset_IX +
                                              ByteElementNumber * 8 + BitNumber;
                        }
                        else
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.IOImageRWIB;
                            TempIndexOffset = (int)TwinCATOffsets.Offset_IB + ByteElementNumber;
                        }
                        break;

                    case TwinCATDataArea.DataArea_M:
                        if (DataFormat == TwinCATDataFormat.DataFormat_Bit)
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

                    case TwinCATDataArea.DataArea_Q:
                        if (DataFormat == TwinCATDataFormat.DataFormat_Bit)
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.IOImageRWOX;
                            TempIndexOffset = (int)TwinCATOffsets.Offset_QX +
                                              ByteElementNumber * 8 + BitNumber;
                        }
                        else
                        {
                            TempIndexGroup = (int)AdsReservedIndexGroups.IOImageRWOB;
                            TempIndexOffset = (int)TwinCATOffsets.Offset_QB + ByteElementNumber;
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
                    case TwinCATDataArea.DataArea_I:
                        if (DataFormat == TwinCATDataFormat.DataFormat_Bit)
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

                    case TwinCATDataArea.DataArea_M:
                        if (DataFormat == TwinCATDataFormat.DataFormat_Bit)
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

                    case TwinCATDataArea.DataArea_Q:
                        if (DataFormat == TwinCATDataFormat.DataFormat_Bit)
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
                case TwinCATDataFormat.DataFormat_Bit:
                    NewAddressString += "X";
                break;

                case TwinCATDataFormat.DataFormat_Byte:
                    NewAddressString += "B";
                break;

                case TwinCATDataFormat.DataFormat_Word:
                    NewAddressString += "W";
                break;

                default:
                    NewAddressString += "D";
                break;
            }

            NewAddressString += ElementNumber.ToString();

            if (_DataFormat == TwinCATDataFormat.DataFormat_Bit)
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

            if (TwinCATVersion == (byte)TwinCATVersions.Version3x)
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
                _DataFormat = TwinCATDataFormat.DataFormat_Byte;
            }
            else
            {
                switch ((uint)thistagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        switch(DataFormat)
                        {
                            case TwinCATDataFormat.DataFormat_Bit:
                                _BitNumber += (prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1);
                                _ElementNumber += _BitNumber / 8;
                                _BitNumber &= 8;
                                break;

                            case TwinCATDataFormat.DataFormat_Byte:
                                _BitNumber = 0;
                                _ElementNumber += (prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1);
                                DataFormat = TwinCATDataFormat.DataFormat_Bit;
                                break;

                            case TwinCATDataFormat.DataFormat_Word:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 2;
                                DataFormat = TwinCATDataFormat.DataFormat_Bit;
                                break;

                            default:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 4;
                                DataFormat = TwinCATDataFormat.DataFormat_Bit;
                                break;
                        }
                        break;

                    case (uint)BuiltInType.Byte:
                    case (uint)BuiltInType.SByte:
                        switch(DataFormat)
                        {
                            case TwinCATDataFormat.DataFormat_Bit:
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
                                DataFormat = TwinCATDataFormat.DataFormat_Byte;
                                break;

                            case TwinCATDataFormat.DataFormat_Byte:
                                _BitNumber = 0;
                                _ElementNumber += (prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1);
                                break;

                            case TwinCATDataFormat.DataFormat_Word:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 2;
                                DataFormat = TwinCATDataFormat.DataFormat_Byte;
                                break;

                            default:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 4;
                                DataFormat = TwinCATDataFormat.DataFormat_Byte;
                                break;
                        }
                        break;

                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        switch(DataFormat)
                        {
                            case TwinCATDataFormat.DataFormat_Bit:
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
                                DataFormat = TwinCATDataFormat.DataFormat_Word;
                                break;

                            case TwinCATDataFormat.DataFormat_Byte:
                                _BitNumber = 0;
                                _ElementNumber += (prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1);
                                if(_ElementNumber%2 > 0)
                                {
                                    _ElementNumber++;
                                }
                                DataFormat = TwinCATDataFormat.DataFormat_Word;
                                break;

                            case TwinCATDataFormat.DataFormat_Word:
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
                                DataFormat = TwinCATDataFormat.DataFormat_Word;
                                break;
                        }
                        break;

                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        switch(DataFormat)
                        {
                            case TwinCATDataFormat.DataFormat_Bit:
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
                                DataFormat = TwinCATDataFormat.DataFormat_DWord;
                                break;

                            case TwinCATDataFormat.DataFormat_Byte:
                                _BitNumber = 0;
                                _ElementNumber += (prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1);
                                if(_ElementNumber%2 > 0)
                                {
                                    _ElementNumber++;
                                }
                                DataFormat = TwinCATDataFormat.DataFormat_DWord;
                                break;

                            case TwinCATDataFormat.DataFormat_Word:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 2;
                                if(_ElementNumber%2 > 0)
                                {
                                    _ElementNumber++;
                                }
                                DataFormat = TwinCATDataFormat.DataFormat_DWord;
                                break;

                            default:
                                _BitNumber = 0;
                                _ElementNumber += ((prevtagdefinition.ArrayDimension > 0 ? (int)prevtagdefinition.ArrayDimension : 1)) * 4;
                                if(_ElementNumber%2 > 0)
                                {
                                    _ElementNumber++;
                                }
                                DataFormat = TwinCATDataFormat.DataFormat_DWord;
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
            _DataFormat = TwinCATDataFormat.DataFormat_Byte;
            RebuildAddressString();
            SetIndexGroupAndOffset();
        }
        #endregion

        #region Properties

        /// <summary>
        /// TwinCAT address in string format
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
                if (value)
                    _InvalidErrorCode = AdsErrorCode.InternalError;
                else
                    _InvalidErrorCode = AdsErrorCode.NoError;
                _IsValid = value;
            }
        }

        /// <summary>
        /// Property that indicates ErrorCode associated to Invalid state
        /// </summary>
        private AdsErrorCode _InvalidErrorCode;
        public AdsErrorCode InvalidErrorCode
        {
            get
            {
                return _InvalidErrorCode;
            }

            set
            {
                _InvalidErrorCode = value;
            }
        }

        /// <summary>
        /// Data area
        /// </summary>
        private TwinCATDataArea _DataArea;
        public TwinCATDataArea DataArea
        {
            get
            {
                return _DataArea;
            }
        }

        /// <summary>
        /// Data format
        /// </summary>
        private TwinCATDataFormat _DataFormat;
        public TwinCATDataFormat DataFormat
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

        private byte _TwinCATVersion;
        public byte TwinCATVersion
        {
            get { return _TwinCATVersion; }
            set
            {
                _TwinCATVersion = value;
            }
        }

        #endregion
    }
}
