using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;

namespace MelsecQEth
{
    public enum DataArea : int
    {
        Invalid = -1,
        X = 0,
        Y = 1,
        M = 2,
        L = 3,
        S = 4,
        B = 5,
        SB = 6,
        F = 7,
        SM = 8,
        TS = 9,
        TC = 10,
        CS = 11,
        CC = 12,
        TN = 13,
        CN = 14,
        D = 15,
        W = 16,
        SW = 17,
        R = 18,
        SD = 19,
        ZR = 20,
    }

    public class MelsecQAddress : Object
    {

        #region Constructors

        /// <summary>
        /// Initializes the MelsecQAddress object with default values.
        /// </summary>
        public MelsecQAddress()
        {
            ResetAddress();
        }

        /// <summary>
        /// Initializes the MelsecQAddress object parsing the assigned address.
        /// </summary>
        public MelsecQAddress(string QAddress)
        {
            Parse(QAddress);
        }

        #endregion

        #region Methods
 
        void ResetAddress()
        {
            _Address = String.Empty;
            _IsValid = false;
            _DataArea = DataArea.Invalid;
            _DataAreaCode = -1;
            _StartAddress = 0;
        }

        /// <summary>
        /// Check if a string can be converted in an integer number
        /// </summary>
        bool IsNumeric(string str, bool bHexFormat)
        {
            if (str == String.Empty)
            {
                return false;
            }

            bool bIsNumeric = true;
            int i;
            int nStringLength = str.Length;
            for (i = 0; i < nStringLength; i++)
            {
                if (!bHexFormat)
                {
                    if ((str[i] < '0' || str[i] > '9') && str[i] != ' ')
                    {
                        bIsNumeric = false;
                        break;
                    }
                }
                else
                {
                    if ((str[i] < '0') ||
                        ((str[i] > '9') && (str[i] < 'A')) ||
                        (str[i] > 'F') &&
                        (str[i] != ' '))
                    {
                        bIsNumeric = false;
                        break;
                    }
                }
            }
            return (bIsNumeric);
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

        string ConvertIntToHexString(int decValue)
        {
            int octValue = (decValue/8)*10 + decValue%8;
            return octValue.ToString();
        }
        private bool ConvertStringAddress(bool bHexFormat, ref int IntAddress, string StringAddress)
        {
            try
            {
                if (!bHexFormat)
                {
                    if (!IsNumeric(StringAddress, false))
                    {
                        return false;
                    }
                    IntAddress = int.Parse(StringAddress, System.Globalization.NumberStyles.Integer);
                }
                else
                {
                    if (!IsNumeric(StringAddress, true))
                    {
                        return false;
                    }
                    IntAddress = int.Parse(StringAddress, System.Globalization.NumberStyles.AllowHexSpecifier);
                }
            }
            catch
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Parse an address.
        /// </summary>
        void Parse(string QAddress, bool bHexFormat = true)
        {
            ResetAddress();
            string TempAddress = String.Copy(QAddress);

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
            DataArea TempDataArea = DataArea.Invalid;
            int TempStartAddress = 0;
            int TempDataAreaCode = -1;
            string StartAddressString = TempAddress.Substring(1);
            string StartAddressString1 = TempAddress.Substring(2);
            StartAddressString.Trim();

            switch (TempAddress[0])
            {
                case 'X':
                    if (ConvertStringAddress(bHexFormat, ref TempStartAddress, StartAddressString))
                    {
                        TempDataArea = DataArea.X;
                        TempDataAreaCode = 0x9C;
                    }
                    else
                        return;
                    break;
                case 'Y':
                    if (ConvertStringAddress(bHexFormat, ref TempStartAddress, StartAddressString))
                    {
                        TempDataArea = DataArea.Y;
                        TempDataAreaCode = 0x9D;
                    }
                    else
                        return;
                    break;
                case 'M':
                    if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString))
                    {
                        TempDataArea = DataArea.M;
                        TempDataAreaCode = 0x90;
                    }
                    else
                        return;
                    break;
                case 'L':
                    if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString))
                    {
                        TempDataArea = DataArea.L;
                        TempDataAreaCode = 0x92;
                    }
                    else
                        return;
                    break;
                case 'B':
                    if (ConvertStringAddress(bHexFormat, ref TempStartAddress, StartAddressString))
                    {
                        TempDataArea = DataArea.B;
                        TempDataAreaCode = 0xA0;
                    }
                    else
                        return;
                    break;
                case 'F':
                    if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString))
                    {
                        TempDataArea = DataArea.F;
                        TempDataAreaCode = 0x93;
                    }
                    else
                        return;
                    break;
                case 'R':
                    if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString))
                    {
                        TempDataArea = DataArea.R;
                        TempDataAreaCode = 0xAF;
                    }
                    else
                        return;
                    break;
                case 'D':
                    if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString))
                    {
                        TempDataArea = DataArea.D;
                        TempDataAreaCode = 0xA8;
                    }
                    else
                        return;
                    break;
                case 'W':
                    if (ConvertStringAddress(bHexFormat, ref TempStartAddress, StartAddressString))
                    {
                        TempDataArea = DataArea.W;
                        TempDataAreaCode = 0xB4;
                    }
                    else
                        return;
                    break;
                case 'S':
                    switch (TempAddress[1])
                    {
                        case 'B':
                            if (ConvertStringAddress(bHexFormat, ref TempStartAddress, StartAddressString1))
                            {
                                TempDataArea = DataArea.SB;
                                TempDataAreaCode = 0xA1;
                            }
                            else
                                return;
                            break;
                        case 'M':
                            if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString1))
                            {
                                TempDataArea = DataArea.SM;
                                TempDataAreaCode = 0x91;
                            }
                            else
                                return;
                            break;
                        case 'W':
                            if (ConvertStringAddress(bHexFormat, ref TempStartAddress, StartAddressString1))
                            {
                                TempDataArea = DataArea.SW;
                                TempDataAreaCode = 0xB5;
                            }
                            else
                                return;
                            break;
                        case 'D':
                            if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString1))
                            {
                                TempDataArea = DataArea.SD;
                                TempDataAreaCode = 0xA9;
                            }
                            else
                                return;
                            break;
                        default:
                            if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString))
                            {
                                TempDataArea = DataArea.S;
                                TempDataAreaCode = 0x98;
                            }
                            else
                                return;
                            break;
                    }
                    break;
                case 'C':
                    switch (TempAddress[1])
                    {
                        case 'S':
                            if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString1))
                            {
                                TempDataArea = DataArea.CS;
                                TempDataAreaCode = 0xC4;
                            }
                            else
                                return;
                            break;
                        case 'C':
                            if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString1))
                            {
                                TempDataArea = DataArea.CC;
                                TempDataAreaCode = 0xC3;
                            }
                            else
                                return;
                            break;
                        case 'N':
                            if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString1))
                            {
                                TempDataArea = DataArea.CN;
                                TempDataAreaCode = 0xC5;
                            }
                            else
                                return;
                            break;
                        default:
                            return ;
                    }
                    break;
                case 'T':
                    switch (TempAddress[1])
                    {
                        case 'S':
                            if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString1))
                            {
                                TempDataArea = DataArea.TS;
                                TempDataAreaCode = 0xC1;
                            }
                            else
                                return;
                            break;
                        case 'C':
                            if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString1))
                            {
                                TempDataArea = DataArea.TC;
                                TempDataAreaCode = 0xC0;
                            }
                            else
                                return;
                            break;
                        case 'N':
                            if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString1))
                            {
                                TempDataArea = DataArea.TN;
                                TempDataAreaCode = 0xC2;
                            }
                            else
                                return;
                            break;
                        default:
                            return ;
                    }
                    break;
                case 'Z':
                    switch (TempAddress[1])
                    {
                        case 'R':
                            if (ConvertStringAddress(false, ref TempStartAddress, StartAddressString1))
                            {
                                TempDataArea = DataArea.ZR;
                                TempDataAreaCode = 0xB0;
                            }
                            else
                                return;
                            break;
                        default:
                            return;
                    }
                    break;
                default:
                    return ;
            }

            _IsValid = true;
            _Address = TempAddress;
            _DataArea = TempDataArea;
            _StartAddress = TempStartAddress;
            _DataAreaCode = TempDataAreaCode;
        }

        /// <summary>
        /// Set a new address.
        /// </summary>

        public void Set(string QAddress)
        {
            Parse(QAddress);
        }

        public bool Set(int StartAddress)
        {
            if ((this.StartAddress < 0) || (_IsValid == false))
            {
                return false;
            }

            string area = String.Empty;
            string stadd = String.Empty;

            switch (_DataArea)
            {
                case DataArea.X:
                case DataArea.Y:
                case DataArea.B:
                case DataArea.W:
                    stadd = string.Format("{0:X}", this.StartAddress);
                    area = Address.Substring(0, 1);
                    break;

                case DataArea.M:
                case DataArea.L:
                case DataArea.S:
                case DataArea.F:
                case DataArea.D:
                case DataArea.R:
                    stadd = string.Format("{0}", this.StartAddress);
                    area = Address.Substring(0, 1);
                    break;

                case DataArea.SB:
                case DataArea.SW:
                    stadd = string.Format("{0:X}", this.StartAddress);
                    area = Address.Substring(0, 2);
                     break;

                case DataArea.SM:
                case DataArea.TS:
                case DataArea.TC:
                case DataArea.CS:
                case DataArea.CC:
                case DataArea.TN:
                case DataArea.CN:
                case DataArea.SD:
                case DataArea.ZR:
                    stadd = string.Format("{0}", this.StartAddress);
                    area = Address.Substring(0, 2);
                    break;
                default:
                    return false;
            }
            string newAddress = area + stadd;
            Parse(newAddress);

            return (_IsValid);
        }
        int GetDataAreaStringLength()
        {
            int nDataAreaStringLength = 0;
            switch (_DataArea)
            {
                case DataArea.X:
                case DataArea.Y:
                case DataArea.B:
                case DataArea.W:
                case DataArea.M:
                case DataArea.L:
                case DataArea.S:
                case DataArea.F:
                case DataArea.D:
                case DataArea.R:
                    nDataAreaStringLength = 1;
                    break;

                case DataArea.SB:
                case DataArea.SW:
                case DataArea.SM:
                case DataArea.TS:
                case DataArea.TC:
                case DataArea.CS:
                case DataArea.CC:
                case DataArea.TN:
                case DataArea.CN:
                case DataArea.SD:
                case DataArea.ZR:
                    nDataAreaStringLength = 2;
                    break;
            }

            return (nDataAreaStringLength);
        }


        public bool SetStructFieldAdd(uint fieldType, uint fieldArraySize)
        {
            // Valid address?
            if (!_IsValid)
            {
                return _IsValid;
            }

            // Get the length of the string of the area code and check it
            int nStartAddIndex = GetDataAreaStringLength();
            if (nStartAddIndex < 1)
                return false;
            if (nStartAddIndex >= _Address.Length)
                return false;
            if (fieldArraySize == 0)
            {
                fieldArraySize = 1;
            }
            int fieldSize = 0;
            switch (fieldType)
            {
                case (uint)BuiltInType.Boolean:
                    fieldSize = 1;
                    break;

                case (uint)BuiltInType.Byte:
                case (uint)BuiltInType.SByte:
                    fieldSize = 8;
                    break;

                case (uint)BuiltInType.Int16:
                case (uint)BuiltInType.UInt16:
                    fieldSize = 16;
                    break;

                case (uint)BuiltInType.Float:
                case (uint)BuiltInType.Int32:
                case (uint)BuiltInType.UInt32:
                    fieldSize = 32;
                    break;

                case (uint)BuiltInType.Int64:
                case (uint)BuiltInType.UInt64:
                case (uint)BuiltInType.Integer:
                case (uint)BuiltInType.Double:
                    fieldSize = 64;
                    break;
            }
            switch (_DataArea)
            {
                case DataArea.X:
                case DataArea.Y:
                case DataArea.B:
                case DataArea.M:
                case DataArea.L:
                case DataArea.S:
                case DataArea.F:
                case DataArea.SB:
                case DataArea.SM:
                case DataArea.TS:
                case DataArea.TC:
                case DataArea.CS:
                case DataArea.CC:
                    _StartAddress += (int)(fieldSize * fieldArraySize);
                    break;

                default:
                    _StartAddress += (int)((fieldSize * fieldArraySize + 15) / 16);
                    break;
            }

            // Build the string of the new address and set it
            string strNewAdd;
            switch (DataArea)
            {
                case DataArea.X:
                case DataArea.Y:
                case DataArea.B:
                case DataArea.SB:
                case DataArea.W:
                case DataArea.SW:
                    // Hexadecimal format
                    strNewAdd = string.Format("{0:X}", this.StartAddress);
                    break;

                default:
                    // Decimal format
                    strNewAdd = string.Format("{0}", this.StartAddress);
                    break;
            }
            _Address = _Address.Substring(0, GetDataAreaStringLength()) + strNewAdd;

            return true;

        }
        public string GetNewAddress(int iAddress)
        {
            // Build the string of the new address and set it
            string strNewAdd;
            switch (DataArea)
            {
                case DataArea.X:
                case DataArea.Y:
                case DataArea.B:
                case DataArea.SB:
                case DataArea.W:
                case DataArea.SW:
                    // Hexadecimal format
                    strNewAdd = string.Format("{0:X}", iAddress);
                    break;

                default:
                    // Decimal format
                    strNewAdd = string.Format("{0}", iAddress);
                    break;
            }
            return  _Address.Substring(0,GetDataAreaStringLength()) + strNewAdd;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Melsec Q address in string format
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
        private DataArea _DataArea;
        public DataArea DataArea
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
        /// Data area code
        /// </summary>
        private int _DataAreaCode;
        public int DataAreaCode
        {
            get
            {
                return _DataAreaCode;
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
