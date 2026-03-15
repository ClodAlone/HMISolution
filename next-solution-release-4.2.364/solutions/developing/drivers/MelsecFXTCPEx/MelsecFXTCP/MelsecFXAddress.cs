using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Opc.Ua;

namespace MelsecFXTCP
{
    public enum MelsecFXDataArea : int
    {
        DataArea_Invalid = -1,
        DataArea_X = 0,
        DataArea_Y = 1,
        DataArea_M = 2,
        DataArea_L = 3,
        DataArea_S = 4,
        DataArea_B = 5,
        DataArea_F = 6,
        DataArea_TS = 7,
        DataArea_TC = 8,
        DataArea_CS = 9,
        DataArea_CC = 10,
        DataArea_TN = 11,
        DataArea_CN = 12,
        DataArea_D = 13,
        DataArea_W = 14,
        DataArea_R = 15,
        DataArea_C = 16,
        DataArea_D_Special = 17,
        DataArea_M_Special = 18,
        DataArea_CS_16 = 19,
        DataArea_CS_32 = 20,
        DataArea_CN_32 = 21,
        DataArea_CN_16 = 22
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
            _DataAreaCode = -1;
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
            int octValue = (decValue/8)*10 + decValue%8;
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
            int TempDataAreaCode = -1;
            string StartAddressString = String.Empty;
            if(TempAddress[0] == 'X')
            {
                StartAddressString = TempAddress.Substring(1);
                StartAddressString.Trim();
                if (IsNumeric(StartAddressString, true) == false)
                {
                    return;
                }
                TempStartAddress = ConvertOctalStringToInt(StartAddressString);
                TempDataArea = MelsecFXDataArea.DataArea_X;
                TempDataAreaCode = 0x5820;
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
                TempDataAreaCode = 0x5920;
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
                TempDataAreaCode = 0x4D20;
            }
            else if (TempAddress[0] == 'R')
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
                TempDataArea = MelsecFXDataArea.DataArea_R;
                TempDataAreaCode = 0x5220;
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
                TempDataAreaCode = 0x4420;
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
                TempDataAreaCode = 0x5320;
            }
            else if (TempAddress[0] == 'C')
            {
                if (TempAddress.Length < 3)
                {
                    return;
                }
                if (TempAddress[1] == 'S')
                {
                    StartAddressString = TempAddress.Substring(2);
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
                    if (TempStartAddress < 200)
                    {
                        TempDataArea = MelsecFXDataArea.DataArea_CS_16;
                    }
                    else
                    {
                        TempDataArea = MelsecFXDataArea.DataArea_CS_32;
                    }
                    TempDataAreaCode = 0x4353;
                }
                else if (TempAddress[1] == 'N')
                {
                    StartAddressString = TempAddress.Substring(2);
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
                    if (TempStartAddress < 200)
                    {
                        TempDataArea = MelsecFXDataArea.DataArea_CN_16;
                    }
                    else
                    {
                        TempDataArea = MelsecFXDataArea.DataArea_CN_32;
                    }
                    TempDataAreaCode = 0x434E;
                }
                else
                {
                    return;
                }
            }
            else if (TempAddress[0] == 'T')
            {
                if (TempAddress.Length < 3)
                {
                    return;
                }
                if (TempAddress[1] == 'S')
                {
                    StartAddressString = TempAddress.Substring(2);
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
                    TempDataArea = MelsecFXDataArea.DataArea_TS;
                    TempDataAreaCode = 0x5453;
                }
                else if (TempAddress[1] == 'N')
                {
                    StartAddressString = TempAddress.Substring(2);
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
                    TempDataArea = MelsecFXDataArea.DataArea_TN;
                    TempDataAreaCode = 0x544E;
                }
                else
                {
                    return;
                }
            }
            else
            {
                return;
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
                case MelsecFXDataArea.DataArea_L:
                case MelsecFXDataArea.DataArea_B:
                case MelsecFXDataArea.DataArea_F:
                case MelsecFXDataArea.DataArea_M_Special:
                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_W:
                case MelsecFXDataArea.DataArea_R:
                case MelsecFXDataArea.DataArea_C:
                case MelsecFXDataArea.DataArea_D_Special:
                    area = Address.Substring(0, 1);
                    stadd = startAddress.ToString();
                break;

                case MelsecFXDataArea.DataArea_TS:
                case MelsecFXDataArea.DataArea_TC:
                case MelsecFXDataArea.DataArea_CS:
                case MelsecFXDataArea.DataArea_CC:
                case MelsecFXDataArea.DataArea_CS_16:
                case MelsecFXDataArea.DataArea_CS_32:
                case MelsecFXDataArea.DataArea_TN:
                case MelsecFXDataArea.DataArea_CN:
                case MelsecFXDataArea.DataArea_CN_16:
                case MelsecFXDataArea.DataArea_CN_32:
                    area = Address.Substring(0, 2);
                    stadd = startAddress.ToString();
                break;

                default:
                return false;
            }

            string newAddress = area + stadd;
            Parse(newAddress);

            return(_IsValid);
        }

        public bool SetStructFieldAdd(uint fieldType, uint fieldArraySize)
        {
            switch(_DataArea)
            {
                case MelsecFXDataArea.DataArea_X://bit
                case MelsecFXDataArea.DataArea_Y://bit
                case MelsecFXDataArea.DataArea_L:
                case MelsecFXDataArea.DataArea_S://bit
                case MelsecFXDataArea.DataArea_B:
                case MelsecFXDataArea.DataArea_F:
                case MelsecFXDataArea.DataArea_M://bit
                case MelsecFXDataArea.DataArea_M_Special://bit
                case MelsecFXDataArea.DataArea_TS:
                case MelsecFXDataArea.DataArea_TC:
                case MelsecFXDataArea.DataArea_CS:
                case MelsecFXDataArea.DataArea_CC:
                    {
                        uint fieldSize = 0;
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

                        if (fieldArraySize > 0)
                        {
                            _StartAddress += (int)(fieldArraySize*fieldSize);
                        }
                        else
                        {
                            _StartAddress += (int)fieldSize;
                        }

                        if ((_DataArea == MelsecFXDataArea.DataArea_M))
                        {
                            if (_StartAddress >= 8000)
                            {
                                _DataArea = MelsecFXDataArea.DataArea_M_Special;
                            }
                        }
                    }
                break;

                case MelsecFXDataArea.DataArea_CS_16:
                case MelsecFXDataArea.DataArea_TN:
                case MelsecFXDataArea.DataArea_CN:
                case MelsecFXDataArea.DataArea_CN_16:
                case MelsecFXDataArea.DataArea_R:
                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_D_Special:
                case MelsecFXDataArea.DataArea_W:
                    {
                        uint fieldSize = 0;
                        switch (fieldType)
                        {
                            case (uint)BuiltInType.Boolean:
                                return false;
                            break;

                            case (uint)BuiltInType.Byte:
                            case (uint)BuiltInType.SByte:
                                fieldSize = 1;
                            break;

                            case (uint)BuiltInType.Int16:
                            case (uint)BuiltInType.UInt16:
                                fieldSize = 2;
                            break;

                            case (uint)BuiltInType.Float:
                            case (uint)BuiltInType.Int32:
                            case (uint)BuiltInType.UInt32:
                               fieldSize = 4;
                            break;

                            case (uint)BuiltInType.Int64:
                            case (uint)BuiltInType.UInt64:
                            case (uint)BuiltInType.Integer:
                            case (uint)BuiltInType.Double:
                                fieldSize = 8;
                            break;
                        }

                        if (fieldArraySize > 0)
                        {
                            fieldSize *= fieldArraySize;
                        }

                        if(fieldSize%2 > 0)
                        {
                            return false;
                        }
                        _StartAddress += (int)fieldSize/2;

                        if ((_DataArea == MelsecFXDataArea.DataArea_D))
                        {
                            if (_StartAddress >= 8000)
                            {
                                _DataArea = MelsecFXDataArea.DataArea_D_Special;
                            }
                        }
                        else if ((_DataArea == MelsecFXDataArea.DataArea_CN_16))
                        {
                            if (_StartAddress >= 200)
                            {
                                if (fieldSize % 4 > 0)
                                {
                                    return false;
                                }
                                _DataArea = MelsecFXDataArea.DataArea_CN_32;
                            }
                        }
                        else if ((_DataArea == MelsecFXDataArea.DataArea_CS_16))
                        {
                            if (_StartAddress >= 200)
                            {
                                if (fieldSize % 4 > 0)
                                {
                                    return false;
                                }
                                _DataArea = MelsecFXDataArea.DataArea_CS_32;
                            }
                        }
                    }
                break;

                case MelsecFXDataArea.DataArea_CN_32:
                case MelsecFXDataArea.DataArea_CS_32:
                    {
                        uint fieldSize = 0;
                        switch (fieldType)
                        {
                            case (uint)BuiltInType.Boolean:
                                return false;
                            break;

                            case (uint)BuiltInType.Byte:
                            case (uint)BuiltInType.SByte:
                                fieldSize = 1;
                            break;

                            case (uint)BuiltInType.Int16:
                            case (uint)BuiltInType.UInt16:
                                fieldSize = 2;
                            break;

                            case (uint)BuiltInType.Float:
                            case (uint)BuiltInType.Int32:
                            case (uint)BuiltInType.UInt32:
                               fieldSize = 4;
                            break;

                            case (uint)BuiltInType.Int64:
                            case (uint)BuiltInType.UInt64:
                            case (uint)BuiltInType.Integer:
                            case (uint)BuiltInType.Double:
                                fieldSize = 8;
                            break;
                        }

                        if (fieldArraySize > 0)
                        {
                            fieldSize *= fieldArraySize;
                        }

                        if(fieldSize%4 > 0)
                        {
                            return false;
                        }
                        _StartAddress += (int)fieldSize/4;
                    }
                break;
                default:
                    return false;
                break;
            }

            // Build the string of the new address
            string newSettings;
            switch (_DataArea)
            {
                case MelsecFXDataArea.DataArea_X:
                    newSettings = "X" + ConvertIntToOctalString(_StartAddress);
                break;
                case MelsecFXDataArea.DataArea_Y:
                    newSettings = "Y" + ConvertIntToOctalString(_StartAddress);
                break;
                case MelsecFXDataArea.DataArea_L:
                    newSettings = "L" + _StartAddress.ToString();
                break;
                case MelsecFXDataArea.DataArea_S:
                    newSettings = "S" + _StartAddress.ToString();
                break;
                case MelsecFXDataArea.DataArea_B:
                    newSettings = "B" + _StartAddress.ToString();
                break;
                case MelsecFXDataArea.DataArea_F:
                    newSettings = "F" + _StartAddress.ToString();
                break;
                case MelsecFXDataArea.DataArea_M:
                case MelsecFXDataArea.DataArea_M_Special:
                    newSettings = "M" + _StartAddress.ToString();
                break;
                case MelsecFXDataArea.DataArea_TS:
                    newSettings = "TS" + _StartAddress.ToString();
                break;
                case MelsecFXDataArea.DataArea_TC:
                newSettings = "TC" + _StartAddress.ToString();
                break;
                case MelsecFXDataArea.DataArea_CS:
                case MelsecFXDataArea.DataArea_CS_16:
                case MelsecFXDataArea.DataArea_CS_32:
                    newSettings = "CS" + _StartAddress.ToString();
                break;
                case MelsecFXDataArea.DataArea_CC:
                    newSettings = "CC" + _StartAddress.ToString();
                break;
                case MelsecFXDataArea.DataArea_TN:
                    newSettings = "TN" + _StartAddress.ToString();
                break;
                case MelsecFXDataArea.DataArea_CN:
                case MelsecFXDataArea.DataArea_CN_16:
                case MelsecFXDataArea.DataArea_CN_32:
                    newSettings = "CN" + _StartAddress.ToString();
                break;
                case MelsecFXDataArea.DataArea_R:
                    newSettings = "R" + _StartAddress.ToString();
                break;
                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_D_Special:
                    newSettings = "D" + _StartAddress.ToString();
                break;
                case MelsecFXDataArea.DataArea_W:
                    newSettings = "W" + _StartAddress.ToString();
                break;
                break;
                default:
                    return false;
                break;
            }

            _Address = newSettings;
            return true;
        }
                
        //Added version 2.2.30.0 (FOGBUGZ 15497)
        public string GetNewAddress( int iAddress)
        {
            string newSettings = String.Empty;
            switch (_DataArea)
            {
                case MelsecFXDataArea.DataArea_X:
                    newSettings = "X" +  ConvertIntToOctalString(iAddress);
                    break;
                case MelsecFXDataArea.DataArea_Y:
                    newSettings = "Y" + ConvertIntToOctalString(iAddress);
                    break;
                case MelsecFXDataArea.DataArea_L:
                    newSettings = "L" + iAddress.ToString();
                    break;
                case MelsecFXDataArea.DataArea_S:
                    newSettings = "S" + iAddress.ToString();
                    break;
                case MelsecFXDataArea.DataArea_B:
                    newSettings = "B" + iAddress.ToString();
                    break;
                case MelsecFXDataArea.DataArea_F:
                    newSettings = "F" + iAddress.ToString();
                    break;
                case MelsecFXDataArea.DataArea_M:
                case MelsecFXDataArea.DataArea_M_Special:
                    newSettings = "M" + iAddress.ToString();
                    break;
                case MelsecFXDataArea.DataArea_TS:
                    newSettings = "TS" + iAddress.ToString();
                    break;
                case MelsecFXDataArea.DataArea_TC:
                    newSettings = "TC" + iAddress.ToString();
                    break;
                case MelsecFXDataArea.DataArea_CS:
                case MelsecFXDataArea.DataArea_CS_16:
                case MelsecFXDataArea.DataArea_CS_32:
                    newSettings = "CS" + iAddress.ToString();
                    break;
                case MelsecFXDataArea.DataArea_CC:
                    newSettings = "CC" + iAddress.ToString();
                    break;
                case MelsecFXDataArea.DataArea_TN:
                    newSettings = "TN" + iAddress.ToString();
                    break;
                case MelsecFXDataArea.DataArea_CN:
                case MelsecFXDataArea.DataArea_CN_16:
                case MelsecFXDataArea.DataArea_CN_32:
                    newSettings = "CN" + _StartAddress.ToString();
                    break;
                case MelsecFXDataArea.DataArea_R:
                    newSettings = "R" + iAddress.ToString();
                    break;
                case MelsecFXDataArea.DataArea_D:
                case MelsecFXDataArea.DataArea_D_Special:
                    newSettings = "D" + iAddress.ToString();
                    break;
                case MelsecFXDataArea.DataArea_W:
                    newSettings = "W" + iAddress.ToString();
                    break;
                    break;
                default:
                    break;
            }
            return newSettings;
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
