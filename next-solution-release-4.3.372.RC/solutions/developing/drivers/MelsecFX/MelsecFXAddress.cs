using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MelsecFX
{
    public enum MelsecFXDataArea : int
    {
        DataArea_Invalid = -1,
        DataArea_X = 0,
        DataArea_Y = 1,
        DataArea_S = 2,
        DataArea_M = 3,
        DataArea_M_Special = 4,
        DataArea_TN = 5,
        DataArea_C_16 = 6,
        DataArea_C_32 = 7,
        DataArea_D = 8,
        DataArea_D_Special = 9
    }

    public class MelsecFXAddress : Object
    {
        #region Constructors

        /// <summary>
        /// Initializes the MelsecFXAddress object with default values.
        /// </summary>
        public MelsecFXAddress()
        {
            ResetAddress();
        }

        /// <summary>
        /// Initializes the MelsecFXAddress object parsing the assigned address.
        /// </summary>
        public MelsecFXAddress(string FXAddress)
        {
            Parse(FXAddress);
        }

        #endregion

        #region Methods

        void ResetAddress()
        {
            _Address = String.Empty;
            _IsValid = false;
            _DataArea = MelsecFXDataArea.DataArea_Invalid;
            _StartAddress = 0;
        }

        /// <summary>
        /// Check if a string can be converted in an integer number
        /// </summary>
        bool IsNumeric(string str, bool OctalFormat)
        {
            if (str == String.Empty)
            {
                return false;
            }

            int i = 0;
            int StringLength = str.Length;
            char MinChar = '0';
            char MaxChar = '9';
            if (OctalFormat)
            {
                MaxChar = '7';
            }

            for (i = 0; i < StringLength; i++)
            {
                if ((str[i] < MinChar) || (str[i] > MaxChar))
                {
                    return false;
                }
            }

            return true;
        }

        int ConvertOctalStringToInt(string OctalString)
        {
            int dec = 0;
            int Product = 1;
            int digit = 0;

            for (int i = OctalString.Length - 1; i >= 0; i--, Product *= 8)
            {
                digit = OctalString[i] - 48;
                dec += (digit * Product);
            }

            return dec;
        }

        string ConvertIntToOctalString(int decValue)
        {
            int octValue = (decValue / 8) * 10 + decValue % 8;
            return octValue.ToString();
        }

        /// <summary>
        /// Parse an address.
        /// </summary>
        void Parse(string FXAddress)
        {
            ResetAddress();
            string TempAddress = String.Copy(FXAddress);

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
            MelsecFXDataArea TempDataArea = MelsecFXDataArea.DataArea_Invalid;
            int TempStartAddress = 0;
            string StartAddressString = String.Empty;

            if (TempAddress[0] == 'X')
            {
                StartAddressString = TempAddress.Substring(1);
                StartAddressString.Trim();
                if (IsNumeric(StartAddressString, true) == false)
                {
                    return;
                }
                TempStartAddress = ConvertOctalStringToInt(StartAddressString);
                TempDataArea = MelsecFXDataArea.DataArea_X;
            }

            else if (TempAddress[0] == 'Y')
            {
                StartAddressString = TempAddress.Substring(1);
                StartAddressString.Trim();
                if (IsNumeric(StartAddressString, true) == false)
                {
                    return;
                }
                TempStartAddress = ConvertOctalStringToInt(StartAddressString);
                TempDataArea = MelsecFXDataArea.DataArea_Y;
            }

            else if (TempAddress[0] == 'M')
            {
                StartAddressString = TempAddress.Substring(1);
                StartAddressString.Trim();
                if (IsNumeric(StartAddressString, false) == false)
                {
                    return;
                }
                if (!int.TryParse(StartAddressString, out TempStartAddress) ||
                    (TempStartAddress < 0))
                {
                    return;
                }
                if (TempStartAddress < 8000)
                {
                    TempDataArea = MelsecFXDataArea.DataArea_M;
                }
                else
                {
                    TempDataArea = MelsecFXDataArea.DataArea_M_Special;
                }
            }

            else if (TempAddress[0] == 'D')
            {
                StartAddressString = TempAddress.Substring(1);
                StartAddressString.Trim();
                if (IsNumeric(StartAddressString, false) == false)
                {
                    return;
                }
                if (!int.TryParse(StartAddressString, out TempStartAddress) ||
                    (TempStartAddress < 0))
                {
                    return;
                }
                if (TempStartAddress < 8000)
                {
                    TempDataArea = MelsecFXDataArea.DataArea_D;
                }
                else
                {
                    TempDataArea = MelsecFXDataArea.DataArea_D_Special;
                }
            }

            else if (TempAddress[0] == 'S')
            {
                StartAddressString = TempAddress.Substring(1);
                StartAddressString.Trim();
                if (IsNumeric(StartAddressString, false) == false)
                {
                    return;
                }
                if (!int.TryParse(StartAddressString, out TempStartAddress) ||
                    (TempStartAddress < 0))
                {
                    return;
                }
                TempDataArea = MelsecFXDataArea.DataArea_S;
            }

            else if (TempAddress[0] == 'C')
            {
                StartAddressString = TempAddress.Substring(1);
                StartAddressString.Trim();
                if (IsNumeric(StartAddressString, false) == false)
                {
                    if (TempAddress[1] != 'N')
                    {
                        return;
                    }

                    if (TempAddress.Length < 3)
                    {
                        return;
                    }

                    StartAddressString = TempAddress.Substring(2);
                    StartAddressString.Trim();
                }
                
                if (IsNumeric(StartAddressString, false) == false)
                {
                    return;
                }
                
                if (!int.TryParse(StartAddressString, out TempStartAddress) ||
                    (TempStartAddress < 0))
                {
                    return;
                }
                if (TempStartAddress < 200)
                {
                    TempDataArea = MelsecFXDataArea.DataArea_C_16;
                }
                else
                {
                    TempDataArea = MelsecFXDataArea.DataArea_C_32;
                }
            }

            else if (TempAddress[0] == 'T')
            {
                StartAddressString = TempAddress.Substring(1);
                StartAddressString.Trim();
                if (IsNumeric(StartAddressString, false) == false)
                {
                    if (TempAddress[1] != 'N')
                    {
                        return;
                    }

                    if (TempAddress.Length < 3)
                    {
                        return;
                    }

                    StartAddressString = TempAddress.Substring(2);
                    StartAddressString.Trim();
                }

                if (IsNumeric(StartAddressString, false) == false)
                {
                    return;
                }

                if (!int.TryParse(StartAddressString, out TempStartAddress) ||
                    (TempStartAddress < 0))
                {
                        return;
                }
                TempDataArea = MelsecFXDataArea.DataArea_TN;
            }

            else
            {
                return;
            }

            _IsValid = true;
            _Address = TempAddress;
            _DataArea = TempDataArea;
            _StartAddress = TempStartAddress;
        }

        /// <summary>
        /// Set a new address.
        /// </summary>

        public void Set(string FXAddress)
        {
            Parse(FXAddress);
        }

        public bool Set(int startAddress)
        {
            if ((startAddress < 0) || (_IsValid == false))
            {
                return false;
            }

            string area = String.Empty;
            string stadd = String.Empty;
            switch (_DataArea)
            {
                case MelsecFXDataArea.DataArea_X:
                case MelsecFXDataArea.DataArea_Y:
                    area = Address.Substring(0, 1);
                    stadd = ConvertIntToOctalString(startAddress);
                    break;

                case MelsecFXDataArea.DataArea_M:
                case MelsecFXDataArea.DataArea_S:
                case MelsecFXDataArea.DataArea_M_Special:
                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_D_Special:
                    area = Address.Substring(0, 1);
                    stadd = startAddress.ToString();
                    break;

                case MelsecFXDataArea.DataArea_C_16:
                case MelsecFXDataArea.DataArea_C_32:
                case MelsecFXDataArea.DataArea_TN:
                    stadd = Address.Substring(1, 1);
                    if (IsNumeric(area, false) == true) {
                        area = Address.Substring(0, 1);
                    }
                    else {
                        area = Address.Substring(0, 2);
                    }
                    stadd = startAddress.ToString();
                    break;

                default:
                    return false;
            }

            string newAddress = area + stadd;
            Parse(newAddress);

            return (_IsValid);
        }

        public Int32 GetBitStartAddress(MelsecFXPLCType plcType, uint offset)
        {
            if (!_IsValid)
            {
                return -1;
            }

            int bitStartAddress = -1;
            switch (plcType)
            {
                case MelsecFXPLCType.FX3U:
                case MelsecFXPLCType.FX2N:
                    switch (_DataArea)
                    {
                        case MelsecFXDataArea.DataArea_X:
                            bitStartAddress = 0x00240;
                            bitStartAddress += (Int32)(((_StartAddress + offset) >> 4) << 1);
                            bitStartAddress <<= 3;
                            bitStartAddress += (Int32)((_StartAddress + offset) % 16);
                            break;
                        case MelsecFXDataArea.DataArea_Y:
                            bitStartAddress = 0x00180;
                            bitStartAddress += (Int32)(((_StartAddress + offset) >> 4) << 1);
                            bitStartAddress <<= 3;
                            bitStartAddress += (Int32)((_StartAddress + offset) % 16);
                            break;
                        case MelsecFXDataArea.DataArea_S:
                            bitStartAddress = 0x00280;
                            bitStartAddress += (Int32)(((_StartAddress + offset) >> 4) << 1);
                            bitStartAddress <<= 3;
                            bitStartAddress += (Int32)((_StartAddress + offset) % 16);
                            break;
                        case MelsecFXDataArea.DataArea_M:
                            bitStartAddress = 0;
                            bitStartAddress += (Int32)(((_StartAddress + offset) >> 4) << 1);
                            bitStartAddress <<= 3;
                            bitStartAddress += (Int32)((_StartAddress + offset) % 16);
                            break;
                        case MelsecFXDataArea.DataArea_M_Special:
                            bitStartAddress = (Int32)(0x01E0 * 8 + _StartAddress + offset - 8000);
                            break;
                    }
                    break;

                case MelsecFXPLCType.FX:
                    switch (_DataArea)
                    {
                        case MelsecFXDataArea.DataArea_X:
                            bitStartAddress = (Int32)(0x0080*8 + _StartAddress + offset);
                            break;
                        case MelsecFXDataArea.DataArea_Y:
                            bitStartAddress = (Int32)(0x00A0*8 + _StartAddress + offset);
                            break;
                        case MelsecFXDataArea.DataArea_S:
                            bitStartAddress = (Int32)(_StartAddress + offset);
                            break;
                        case MelsecFXDataArea.DataArea_M:
                            bitStartAddress = (Int32)(0x0100*8 + _StartAddress + offset);
                            break;
                        case MelsecFXDataArea.DataArea_M_Special:
                            bitStartAddress = (Int32)(0x01E0*8 + _StartAddress + offset - 8000);
                            break;
                    }
                    break;
            }

            return bitStartAddress;
        }

        public Int32 GetByteStartAddress(MelsecFXPLCType plcType, uint offset)
        {
            if (!_IsValid)
            {
                return -1;
            }

            int byteStartAddress = -1;
            switch (plcType)
            {
                case MelsecFXPLCType.FX3U:
                case MelsecFXPLCType.FX2N:
                    switch (_DataArea)
                    {
                        case MelsecFXDataArea.DataArea_X:
                            byteStartAddress = (Int32) (0x00240 + (_StartAddress + offset) / 8);
                            break;
                        case MelsecFXDataArea.DataArea_Y:
                            byteStartAddress = (Int32)(0x00180 + (_StartAddress + offset) / 8);
                            break;
                        case MelsecFXDataArea.DataArea_S:
                            byteStartAddress = (Int32)(0x00280 + (_StartAddress + offset) / 8);
                            break;
                        case MelsecFXDataArea.DataArea_M:
                            byteStartAddress = (Int32)((_StartAddress + offset) / 8);
                            break;
                        case MelsecFXDataArea.DataArea_M_Special:
                            byteStartAddress = (Int32)(0x01E0 + (_StartAddress + offset - 8000) / 8);
                            break;
                        case MelsecFXDataArea.DataArea_TN:
                            byteStartAddress = (Int32)(0x01000 + (_StartAddress + offset) * 2);
                            break;
                        case MelsecFXDataArea.DataArea_C_16:
                            byteStartAddress = (Int32)(0x00A00 + (_StartAddress + offset) * 2);
                            break;
                        case MelsecFXDataArea.DataArea_D:
                            byteStartAddress = (Int32)(0x04000 + (_StartAddress + offset) * 2);
                            break;
                        case MelsecFXDataArea.DataArea_D_Special:
                            byteStartAddress = (Int32)(0x0E00 + (_StartAddress + offset - 8000) * 2);
                            break;
                        case MelsecFXDataArea.DataArea_C_32:
                            byteStartAddress = (Int32)(0x00C00 + (_StartAddress + offset - 200) * 4);
                            break;
                    }
                    break;

                case MelsecFXPLCType.FX:
                    switch (_DataArea)
                    {
                        case MelsecFXDataArea.DataArea_X:
                            byteStartAddress = (Int32)(0x0080 + (_StartAddress + offset) / 8);
                            break;
                        case MelsecFXDataArea.DataArea_Y:
                            byteStartAddress = (Int32)(0x00A0 + (_StartAddress + offset) / 8);
                            break;
                        case MelsecFXDataArea.DataArea_S:
                            byteStartAddress = (Int32)((_StartAddress + offset) / 8);
                            break;
                        case MelsecFXDataArea.DataArea_M:
                            byteStartAddress = (Int32)(0x0100 + (_StartAddress + offset) / 8);
                            break;
                        case MelsecFXDataArea.DataArea_M_Special:
                            byteStartAddress = (Int32)(0x01E0 + (_StartAddress + offset - 8000) / 8);
                            break;
                        case MelsecFXDataArea.DataArea_TN:
                            byteStartAddress = (Int32)(0x0800 + (_StartAddress + offset) * 2);
                            break;
                        case MelsecFXDataArea.DataArea_C_16:
                            byteStartAddress = (Int32)(0x0A00 + (_StartAddress + offset) * 2);
                            break;
                        case MelsecFXDataArea.DataArea_D:
                            byteStartAddress = (Int32)(0x1000 + (_StartAddress + offset) * 2);
                            break;
                        case MelsecFXDataArea.DataArea_D_Special:
                            byteStartAddress = (Int32)(0x0E00 + (_StartAddress + offset - 8000) * 2);
                            break;
                        case MelsecFXDataArea.DataArea_C_32:
                            byteStartAddress = (Int32)(0x0C00 + (_StartAddress + offset - 200) * 4);
                            break;
                    }
                    break;
            }

            return byteStartAddress;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Melsec FX address in string format
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
        /// Data area
        /// </summary>
        private MelsecFXDataArea _DataArea;
        public MelsecFXDataArea DataArea
        {
            get
            {
                return _DataArea;
            }
        }

        /// <summary>
        /// Start address
        /// </summary>
        private int _StartAddress;
        public int StartAddress
        {
            get
            {
                return _StartAddress;
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

        #endregion
    }
}
