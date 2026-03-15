using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DriverCodeBase;
using DriverCodeBase.Helpers;
using System.Text.RegularExpressions;
using DriverBaseInterfaces;
using Opc.Ua;
using DevExpress.Xpo;

namespace S7TCP
{
    public sealed class S7TCPDynTagSettings : DynTagSettings
    {
        #region Constructors

        public S7TCPDynTagSettings()
            : base()
        {
            German = false;
            Area = Step7Area.aP;
            Format = Step7Format.frmBit;
            Trans = Step7WordTrans.wtW;
            Offset = 0;
            Length = 1;
            DbNumber = 0;
            Bit = 0;
            S7_200 = false;
            LenStringEnable = false;
            SwapDWords = false;

            Valid = false;
            _StructStringFieldLengths = string.Empty;
            _IsMemberOfStruct = false;
        }

        bool Valid = false;
        #endregion
        
        #region Static Members

        private static readonly String StartAddressParameter = "SA";
        private static readonly String LenStringEnableParameter = "LenStrEn";
        private static readonly String SwapDWordsParameter = "SwapDWords";
        private static readonly String StructStringFieldLengthsParameter = "SSFL";
        private static readonly String IsMemberOfStructParameter = "IMOS";

        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            StartAddress = helper.GetPartByName(StartAddressParameter);
            LenStringEnable = helper.GetPartByName(LenStringEnableParameter, false);
            SwapDWords = helper.GetPartByName(SwapDWordsParameter, false);
            StructStringFieldLengths = helper.GetPartByName(StructStringFieldLengthsParameter);

            bool ret = ParseAddress(StartAddress);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;
            
            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            StartAddress = helper.GetPartByName(StartAddressParameter);
            LenStringEnable = helper.GetPartByName(LenStringEnableParameter, false);
            SwapDWords = helper.GetPartByName(SwapDWordsParameter, false);

            // optional parameter
            if (String.IsNullOrEmpty(helper.GetPartByName(StructStringFieldLengthsParameter)))
            {
                StructStringFieldLengths = String.Empty;
            }
            else
            {
                StructStringFieldLengths = helper.GetPartByName(StructStringFieldLengthsParameter);
            }

            _IsMemberOfStruct = helper.GetPartByName(IsMemberOfStructParameter, false);

            return ParseAddress(StartAddress);
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);

            if (SwapDWords)
            {
                dynamicstring.AppendFormat("{0}{1}{2}", SwapDWordsParameter, DynamicStringParser.CharAssign, SwapDWords);
                dynamicstring.Append(DynamicStringParser.CharSep);
            } 

            string saddr;
            if (!S7_200)
                saddr = ConvertToString_S7300();
            else
                saddr = ConvertToString_S7200();

            dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, saddr/*StartAddress*/);
            if(LenStringEnable)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", LenStringEnableParameter, DynamicStringParser.CharAssign, LenStringEnable);
            }

            // optional parameter
            if (!String.IsNullOrWhiteSpace(StructStringFieldLengths))
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", StructStringFieldLengthsParameter, DynamicStringParser.CharAssign, StructStringFieldLengths);
            }

            return dynamicstring.ToString();
        }

        public string ToStringSA()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);

            if (SwapDWords)
            {                
                dynamicstring.AppendFormat("{0}{1}{2}", SwapDWordsParameter, DynamicStringParser.CharAssign, SwapDWords);
                dynamicstring.Append(DynamicStringParser.CharSep);
            }

            dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, StartAddress);

            if (LenStringEnable)
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", LenStringEnableParameter, DynamicStringParser.CharAssign, LenStringEnable);
            }

            // optional parameter
            if (!String.IsNullOrWhiteSpace(StructStringFieldLengths))
            {
                dynamicstring.Append(DynamicStringParser.CharSep);
                dynamicstring.AppendFormat("{0}{1}{2}", StructStringFieldLengthsParameter, DynamicStringParser.CharAssign, StructStringFieldLengths);
            }

            return dynamicstring.ToString();
        }

        //public override bool isTagByteSizeOk(uint ByteSize)
        //{
        //    if ((ArrayDimension != 0) && 
        //        (VarType > UFUAModel.DataType.Boolean) && 
        //        (VarType < UFUAModel.DataType.String))
        //    {
        //        return true;
        //    }
        //    else
        //        return (S7Protocol.MAX_DATA_BYTES >= ByteSize);
        //}
        public override UFUAModel.DataType getProtocolDataType()
        {
            ParseAddress(StartAddress);
            return (S7Protocol.DataType(Format));
        }

        public override string GetFirstDynSetting(Tag tag, TagDefinition thistagdefinition)
        {
            if (!TryParse(tag.TagNode.DynamicSettings))
            {
                return AddMemberOfStructInfoToDynamicSettings(tag.TagNode.DynamicSettings);
            }
            else
            {
                if ((thistagdefinition.DataType.IdType == IdType.Numeric) && ((uint)thistagdefinition.DataType.Identifier == (uint)BuiltInType.String))
                {
                    LenStringEnable = true;
                    return AddMemberOfStructInfoToDynamicSettings(ToString());
                }
                else
                {
                    return AddMemberOfStructInfoToDynamicSettings(tag.TagNode.DynamicSettings);
                }
            }            
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            Step7Format newformat = Step7Format.frmInvalid;
            Step7Format prevformat = Step7Format.frmInvalid;
            
            int longLength = 1;
            if (prevtagdefinition.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)prevtagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        prevformat = Step7Format.frmBit;
                        break;
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        prevformat = Step7Format.frmByte;
                        break;
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        prevformat = Step7Format.frmWord;
                        break;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        prevformat = Step7Format.frmDWord;
                        break;
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        prevformat = Step7Format.frmByte;
                        longLength = 8;
                        break;
                    case (uint)BuiltInType.String:
                        prevformat = Step7Format.frmByte;
                        break;
                }
            }

            if (thistagdefinition.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)thistagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        newformat = Step7Format.frmBit;
                        break;
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        newformat = Step7Format.frmByte;
                        break;
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        newformat = Step7Format.frmWord;
                        break;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                        newformat = Step7Format.frmDWord;
                        break;
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        newformat = Step7Format.frmByte;
                        break;
                    case (uint)BuiltInType.String:
                        newformat = Step7Format.frmByte;
                        break;
                }
            }

            if (newformat != Step7Format.frmInvalid)
                Format = newformat;
            else if (prevtagdefinition.ArrayDimension > 0)
                Length = (int)prevtagdefinition.ArrayDimension;

            // correct length for 8byte data type (unsuppored by S7TCP protocol)
            Length *= longLength;

            if (thistagdefinition.DataType.IdType == IdType.Numeric && prevtagdefinition.DataType.IdType == IdType.Numeric)
            {
                switch ((uint)thistagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        LenStringEnable = false;
                        switch (prevformat)
                        {
                            case Step7Format.frmBit:
                                if (thistagdefinition.ArrayDimension == 0)
                                {
                                    if (prevtagdefinition.ArrayDimension == 0)
                                    {
                                        Bit++;
                                        Offset += (Bit / 8);
                                        Bit %= 8;
                                    }
                                    else
                                    {
                                        Bit = 0;
                                        Offset = Offset + ((int)(prevtagdefinition.ArrayDimension / 8));
                                        if ((prevtagdefinition.ArrayDimension % 8) != 0)
                                        {
                                            Offset++;
                                        }
                                        Offset += (Offset % 2);
                                    }
                                }
                                else
                                { 
                                    if (prevtagdefinition.ArrayDimension == 0)
                                    {
                                        Bit = 0;
                                        Offset++;
                                        Offset += (Offset % 2);
                                    } 
                                    else
                                    {
                                        Bit = 0;
                                        Offset = Offset + ((int)(prevtagdefinition.ArrayDimension / 8));
                                        if ((prevtagdefinition.ArrayDimension % 8) != 0)
                                        {
                                            Offset++;
                                        }
                                        Offset += (Offset % 2);
                                    }
                                }
                                Length = 1;
                                break;
                            case Step7Format.frmByte:
                                Offset += Length;
                                Offset += (Offset % 2);
                                Bit = 0;
                                //Format = Step7Format.frmBit;
                                Length = 1;
                                break;
                            case Step7Format.frmWord:
                                Offset += 2 * Length;
                                Offset += (Offset % 2);
                                Bit = 0;
                                //Format = Step7Format.frmBit;
                                Length = 1;
                                break;
                            default:
                                Offset += 4 * Length;
                                Offset += (Offset % 2);
                                Bit = 0;
                                //Format = Step7Format.frmBit;
                                Length = 1;
                                break;
                        }                        
                        break;
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        LenStringEnable = false;
                        switch (prevformat)
                        {
                            case Step7Format.frmBit:
                                if (prevtagdefinition.ArrayDimension == 0)
                                {
                                    Offset++;
                                }
                                else
                                {
                                    Offset = Offset + ((int)(prevtagdefinition.ArrayDimension / 8));
                                    if ((prevtagdefinition.ArrayDimension % 8) != 0)
                                    {
                                        Offset++;
                                    }
                                    Offset += (Offset % 2);
                                }
                                Bit = 0;
                                //Format = Step7Format.frmByte;
                                Length = 1;
                                break;
                            case Step7Format.frmByte:
                                Offset += Length;
                                Bit = 0;
                                Length = 1;
                                break;
                            case Step7Format.frmWord:
                                Offset += 2 * Length;
                                Bit = 0;
                                //Format = Step7Format.frmByte;
                                Length = 1;
                                break;
                            default:
                                Offset += 4 * Length;
                                Bit = 0;
                                //Format = Step7Format.frmByte;
                                Length = 1;
                                break;
                        }

                        break;
                    case (uint)BuiltInType.Int16:
                    case (uint)BuiltInType.UInt16:
                        LenStringEnable = false;
                        switch (prevformat)
                        {
                            case Step7Format.frmBit:
                                if (prevtagdefinition.ArrayDimension == 0)
                                {
                                    Offset++;
                                    if (Offset % 2 > 0)
                                    {
                                        Offset++;
                                    }
                                    Length = 1;
                                }
                                else
                                {
                                    Offset = Offset + ((int)(prevtagdefinition.ArrayDimension / 8));
                                    if ((prevtagdefinition.ArrayDimension % 8) != 0)
                                    {
                                        Offset++;
                                    }
                                    Offset += (Offset % 2);
                                }                                
                                Bit = 0;
                                //Format = Step7Format.frmWord;
                                Length = 1;
                                break;
                            case Step7Format.frmByte:
                                Offset += Length;
                                if (Offset % 2 > 0)
                                {
                                    Offset++;
                                }
                                Bit = 0;
                                //Format = Step7Format.frmWord;
                                Length = 1;
                                break;
                            case Step7Format.frmWord:
                                Offset += 2 * Length;
                                if (Offset % 2 > 0)
                                {
                                    Offset++;
                                }
                                Bit = 0;
                                Length = 1;
                                break;
                            default:
                                Offset += 4 * Length;
                                if (Offset % 2 > 0)
                                {
                                    Offset++;
                                }
                                Bit = 0;
                                //Format = Step7Format.frmWord;
                                Length = 1;
                                break;
                        }
                        //if ((prevtagdefinition.ArrayDimension != 0) &&
                        //    ((uint)prevtagdefinition.DataType.Identifier == (uint)BuiltInType.Boolean))
                        //{
                        //    Offset += (int)(prevtagdefinition.ArrayDimension / 8);
                        //    Offset += (Offset % 2);
                        //}
                        break;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        LenStringEnable = false;
                        switch (prevformat)
                        {
                            case Step7Format.frmBit:
                                //Offset++;
                                //if (Offset % 2 > 0)
                                //{
                                //    Offset++;
                                //}
                                if (prevtagdefinition.ArrayDimension == 0)
                                {
                                    Offset++;
                                    if (Offset % 2 > 0)
                                    {
                                        Offset++;
                                    }
                                    Length = 1;
                                }
                                else
                                {
                                    Offset = Offset + ((int)(prevtagdefinition.ArrayDimension / 8));
                                    if ((prevtagdefinition.ArrayDimension % 8) != 0)
                                    {
                                        Offset++;
                                    }
                                    Offset += (Offset % 2);
                                }
                                Bit = 0;
                                //Format = Step7Format.frmWord;
                                Length = 1;
                                break;
                            case Step7Format.frmByte:
                                Offset += Length;
                                if (Offset % 2 > 0)
                                {
                                    Offset++;
                                }
                                Bit = 0;
                                //Format = Step7Format.frmDWord;
                                Length = 1;
                                break;
                            case Step7Format.frmWord:
                                Offset += 2 * Length;
                                if (Offset % 2 > 0)
                                {
                                    Offset++;
                                }
                                Bit = 0;
                                //Format = Step7Format.frmDWord;
                                Length = 1;
                                break;
                            default:
                                Offset += 4 * Length;
                                if (Offset % 2 > 0)
                                {
                                    Offset++;
                                }
                                Bit = 0;
                                //Format = Step7Format.frmDWord;
                                Length = 1;
                                break;
                        }
                        //if ((prevtagdefinition.ArrayDimension != 0) &&
                        //    ((uint)prevtagdefinition.DataType.Identifier == (uint)BuiltInType.Boolean))
                        //{
                        //    Offset += (int)(prevtagdefinition.ArrayDimension / 8);
                        //    Offset += (Offset % 2);
                        //}
                        break;
                    case (uint)BuiltInType.String:
                        {
                            switch (prevformat)
                            {
                                case Step7Format.frmBit:
                                    //Offset++;
                                    if (prevtagdefinition.ArrayDimension == 0)
                                    {
                                        Offset++;
                                        Offset += (Offset % 2);
                                    }
                                    else
                                    {
                                        Offset = Offset + ((int)(prevtagdefinition.ArrayDimension / 8));
                                        if ((prevtagdefinition.ArrayDimension % 8) != 0)
                                        {
                                            Offset++;
                                        }
                                        Offset += (Offset % 2);
                                    }
                                    Bit = 0;
                                    break;
                                case Step7Format.frmByte:
                                    Offset += Length;
                                    if (Offset % 2 > 0)
                                    {
                                        Offset++;
                                    }
                                    Bit = 0;
                                    break;
                                case Step7Format.frmWord:
                                    Offset += 2 * Length;
                                    if (Offset % 2 > 0)
                                    {
                                        Offset++;
                                    }
                                    Bit = 0;
                                    break;
                                default:
                                    Offset += 4 * Length;
                                    if (Offset % 2 > 0)
                                    {
                                        Offset++;
                                    }
                                    Bit = 0;
                                    break;
                            }
                            LenStringEnable = true;
                            // Skip the two bytes of the string header
                            Offset += 2;
                            // Set the string Length
                            if (DeviceSize > 0)
                            {
                                Length = DeviceSize;
                                DeviceSize = 0;
                            }
                            else if (String.IsNullOrWhiteSpace(StructStringFieldLengths))
                            {
                                // Default length
                                Length = (int)S7Protocol.DEFAULTSTRINGLENGTH_TIAPORTAL;//254;
                            }
                            else
                            {
                                // Check if the length has been specified for this element of the structure
                                bool stringLengthFound = false;
                                string stringLengths = StructStringFieldLengths.ToString();

                                // Check if a global length has been set
                                if ((stringLengths.IndexOf(":") == 0) && (stringLengths.Length > 1))
                                {
                                    string lengthInfo = stringLengths.Substring(1);
                                    UInt32 stringLength = 0;
                                    if (UInt32.TryParse(lengthInfo, out stringLength))
                                    {
                                        Length = (int)stringLength;
                                        stringLengthFound = true;
                                    }
                                }

                                // Check if the length has been specified for this element of the structure
                                else
                                {
                                    // Build the search key
                                    int indexSizeString = thistagdefinition.Name.IndexOf(':');
                                    string searchKey = String.Empty;
                                    if (indexSizeString < 0)
                                    {
                                        searchKey = thistagdefinition.Name + ":";
                                    }
                                    else
                                    {
                                        searchKey = thistagdefinition.Name.Substring(indexSizeString + 1) + ":";
                                    }
                                    indexSizeString = stringLengths.IndexOf(searchKey);
                                    if (indexSizeString >= 0)
                                    {
                                        string auxString = stringLengths.Substring(indexSizeString);
                                        if (!String.IsNullOrWhiteSpace(auxString))
                                        {
                                            int colonIndex = auxString.IndexOf(":");
                                            if (colonIndex > 0)
                                            {
                                                int numericalStringLength = auxString.Length - colonIndex - 1;
                                                int semicolonIndex = auxString.IndexOf(";");
                                                if ((semicolonIndex > 0) && (semicolonIndex > colonIndex))
                                                {
                                                    numericalStringLength = semicolonIndex - colonIndex - 1;
                                                }
                                                if (numericalStringLength > 0)
                                                {
                                                    string lengthInfo = auxString.Substring(colonIndex + 1, numericalStringLength);
                                                    Int64 stringLength = 0;
                                                    if (Int64.TryParse(lengthInfo, out stringLength))
                                                    {
                                                        if (stringLength > 0)
                                                        {
                                                            Length = (int)stringLength;
                                                            stringLengthFound = true;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                                if (!stringLengthFound)
                                {
                                    Length = (int)S7Protocol.DEFAULTSTRINGLENGTH_TIAPORTAL; // 254;
                                }
                            }
                        }
                        //if ((prevtagdefinition.ArrayDimension != 0) &&
                        //    ((uint)prevtagdefinition.DataType.Identifier == (uint)BuiltInType.Boolean))
                        //{
                        //    Offset += (int)(prevtagdefinition.ArrayDimension / 8);
                        //    Offset += (Offset % 2);
                        //}
                        break;
                    default:
                        return string.Empty;
                }

            }

            //if (newformat != Step7Format.frmInvalid)
            //    Format = newformat;
            return AddMemberOfStructInfoToDynamicSettings(ToString());
        }

        /// <summary>
        /// Add on fly to dynamic link information about struct membership
        /// </summary>
        /// <param name="dynamicstring"></param>
        /// <returns></returns>
        private string AddMemberOfStructInfoToDynamicSettings(string dynamicstring)
        {
            dynamicstring += DynamicStringParser.CharSep;
            dynamicstring += string.Format("{0}{1}{2}", IsMemberOfStructParameter, DynamicStringParser.CharAssign, true);

            return dynamicstring;
        }

        #endregion

        public bool ParseAddress(string address)
        {
            if ((address == null) || (address == String.Empty))
            {
                return false;
            }

            bool ret = false;

            ret = ParseDataPLC(address);
            
            return ret;
        }
        static void TrimSpaces( ref string Dst, string Src )
        {
          int len = Src.Length;
          int i = 0;

          Dst = string.Empty;

          if( len == 0 )
            return;

          do {
            if( Src[ i ] != ' ' )
              Dst += Src[ i ];
          } while( ++i < len );
        }
        static Step7Format GetS7Type(string t)
        { 
            switch(t[0])
            {
                case 'B':
                    return Step7Format.frmByte;
                    break;
                case 'W':
                    return Step7Format.frmWord;
                    break;
                case 'D':
                    return Step7Format.frmDWord;
                    break;
                case 'X':
                    return Step7Format.frmBit;
                    break;
            }
            return Step7Format.frmInvalid;
        }

        static Step7Area GetS7StaticArea(string s)
        {
            switch (s)
            {
                case "E":
                    return Step7Area.aI;
                case "I":
                    return Step7Area.aI;
                case "A":
                    return Step7Area.aQ;
                case "Q":
                    return Step7Area.aQ;
                case "PE":
                    return Step7Area.aPE;
                case "PA":
                    return Step7Area.aPA;
                case "P":
                    return Step7Area.aP;
                case "M":
                    return Step7Area.aM;
                case "F":
                    return Step7Area.aM;
                    break;
                case "V":
                    return Step7Area.aD;
                case "S":
                    return Step7Area.aS;
                case "H":
                    return Step7Area.aH;
                case "D":
                    return Step7Area.aD;
                case "T":
                    return Step7Area.aT;
                case "Z":
                    return Step7Area.aC;
                case "C":
                    return Step7Area.aC;
            }
            return Step7Area.aInvalid;
        }

        Step7Area GetS7Area(string s)
        {
            switch (s)
            { 
                case "E":
                    German = true;
                    return Step7Area.aI;
                case "I":
                    return Step7Area.aI;
                case "A":
                    German = true;
                    return Step7Area.aQ;
                case "Q":
                    return Step7Area.aQ;
                case "PE":
                    German = true;
                    return Step7Area.aPE;
                case "PA":
                    German = true;
                    return Step7Area.aPA;
                case "P":
                    return Step7Area.aP;
                case "M":
                    German = true;
                    return Step7Area.aM;
                case "F":
                    return Step7Area.aM;
                    break;
                case "V":
                    DbNumber = 1;
                    return Step7Area.aD;
                case "S":
                    return Step7Area.aS;
                case "H":
                    return Step7Area.aH;
                case "D":
                    return Step7Area.aD;
                case "T":
                    Format = Step7Format.frmWord;
                    Trans = Step7WordTrans.wtT;
                    return Step7Area.aT;
                case "Z":
                    German = true;
                    Format = Step7Format.frmWord;
                    Trans = Step7WordTrans.wtC;
                    return Step7Area.aC;
                case "C":
                    Format = Step7Format.frmWord;
                    Trans = Step7WordTrans.wtC;
                    return Step7Area.aC;
            }
            return Step7Area.aInvalid;
        }
        bool ParseDataPLC(string Str)
        {
            string PureStr = string.Empty;

            Str = Str.ToUpper();
            TrimSpaces(ref PureStr, Str);

            German = false;
            //bool Valid = false;
            
            Area = Step7Area.aP;
            Format = Step7Format.frmBit;
            Trans = Step7WordTrans.wtW;
            Offset = 0;
            Length = 1;
            DbNumber = 0;
            Bit = 0;
            S7_200 = false;

            if (ParsePlcAddress(Str))
            {
                Valid = true;
            }
            else
            {
                S7_200 = true;
                Valid = ParsePlcAddress(Str);
            }
            
            /*
            Regex dbParser = new Regex(@"^DB(?<num>\d+).DB(?<tipo>\w)(?<add>\d+)(?<bit>.\d+)?(?<len>:\d+})?(?<conv>,\w)?");

            Match dbMatch = dbParser.Match(Str);
            if (dbMatch.Success)
            {
                //DB type address
                Area = Step7Area.aD;
                DbNumber = Convert.ToInt32(dbMatch.Groups["num"].Value);
                Format = GetS7Type(dbMatch.Groups["tipo"].Value);
                if(dbMatch.Groups["conv"].Value == "T")
                    Trans = Step7WordTrans.wtT;
                else if(dbMatch.Groups["conv"].Value == "C")
                    Trans = Step7WordTrans.wtC;
                Offset = Convert.ToInt32(dbMatch.Groups["add"].Value);
                if(dbMatch.Groups["bit"].Value.Length > 0)
                    Bit = Convert.ToInt32(dbMatch.Groups["bit"].Value.Replace('.', ' '));
                if (dbMatch.Groups["len"].Value.Length > 0)
                    Length = Convert.ToInt32(dbMatch.Groups["len"].Value);
                Valid = true;
                S7_200 = false;
            }
            else
            {
                Regex addParser = new Regex(@"^(?<area>[EIAQPEMFTZC]+)(?<tipo>[BWDX]{1})?(?<add>\d+)(?<bit>.\d+)?(?<len>:\d+})?(?<conv>,[TC]{1})?");
                Match addMatch = addParser.Match(Str);
                if (addMatch.Success)
                { 
                    //Area address
                    Format = GetS7Type(addMatch.Groups["tipo"].Value);
                    if(addMatch.Groups["conv"].Value == "T")
                        Trans = Step7WordTrans.wtT;
                    else if(addMatch.Groups["conv"].Value == "C")
                        Trans = Step7WordTrans.wtC;
                    Offset = Convert.ToInt32(addMatch.Groups["add"].Value);
                    if(addMatch.Groups["bit"].Value.Length > 0)
                        Bit = Convert.ToInt32(addMatch.Groups["bit"].Value.Replace('.', ' '));
                    if (addMatch.Groups["len"].Value.Length > 0)
                        Length = Convert.ToInt32(addMatch.Groups["len"].Value);

                    string s = addMatch.Groups["area"].Value;
                    Area = GetS7Area(s);
                    if (s == "AI" || s == "AQ" || s == "V" || s == "S" || s == "H")
                        S7_200 = true;
                    else if (s == "P" || s == "PE" || s == "PA")
                        S7_200 = false;
                    Valid = true;
                }
            }*/
            
            return Valid;
        }

        #region parse S7 address
        string ConvertToString_S7200()
        {
            char InputChar = 'I';
            char OutputChar = 'Q';
            char CounterChar = 'C';

            string Dst = string.Empty;
            string Pre = string.Empty;
            string Post = string.Empty;

            if (!Valid)
                return string.Empty;

            if (German)
            {
                InputChar = 'E';
                OutputChar = 'A';
                CounterChar = 'Z';
            }
            switch (Area)
            {
                case Step7Area.aI:
                    Pre = string.Format("{0}", InputChar);
                    break;
                case Step7Area.aQ:
                    Pre = string.Format("{0}", OutputChar);
                    break;
                case Step7Area.aM:
                    Pre = "M";
                    break;
                case Step7Area.aD:
                    Pre = "V";
                    break;
                case Step7Area.aS:
                    Pre = "S";
                    break;
                case Step7Area.aH:
                    Pre = "H";
                    break;
                case Step7Area.aAI:
                    Dst = string.Format("AI{0}", Offset);
                    return Dst;
                case Step7Area.aAQ:
                    Dst = string.Format("AQ{0}", Offset);
                    return Dst;
                case Step7Area.aTIEC:
                    if (1 == Length)
                        Dst = string.Format("T{0}", Offset);
                    else
                        Dst = string.Format("T{0}:{1}", Offset, Length);
                    return Dst;
                case Step7Area.aCIEC:
                    if (1 == Length)
                        Dst = string.Format("{0}{1}", CounterChar, Offset);
                    else
                        Dst = string.Format("{0}{1}:{2}", CounterChar, Offset, Length);
                    return Dst;
            }

            switch( Format ) {
              case Step7Format.frmBit:
                if( 1 == Length )
                  Dst = string.Format( "{0}{1}.{2}", Pre, Offset, Bit );
	            else {
	              Dst = string.Format( "{0}{1}.{2}:{3}", Pre, Offset, Bit, Length );
	            }
                break;
              case Step7Format.frmByte:
                if( 1 == Length )
                  Dst = string.Format( "{0}B{1}", Pre, Offset );
                else
                  Dst = string.Format( "{0}B{1}:{2}", Pre, Offset, Length );
                break;
              case Step7Format.frmDWord:
                if( 1 == Length )
                  Dst = string.Format( "{0}D{1}", Pre, Offset );
                else
                  Dst = string.Format( "{0}D{1}:{2}", Pre, Offset, Length );
                break;
              case Step7Format.frmWord:
                switch( Trans ) {
                case Step7WordTrans.wtW:
                  Post = string.Empty;
                  break;
                case Step7WordTrans.wtC:
                  Post = string.Format( ",{0}", CounterChar );
                  break;
                case Step7WordTrans.wtT:
                  Post = ",T";
                  break;
                /*case wtT001:
                  Post = CString( _T( ",T" ) ) + strFormatTimer1;
                  break;
                case wtT01:
                  Post = CString( _T( ",T" ) ) + strFormatTimer2;
                  break;
                case wtT1:
                  Post = CString( _T( ",T" ) ) + strFormatTimer3;
                  break;
                case wtT10:
                  Post = CString( _T( ",T" ) ) + strFormatTimer4;
                  break;*/
                }
    
                if( 1 == Length )
                  Dst = string.Format( "{0}W{1}{2}", Pre, Offset, Post );
                else
                  Dst = string.Format( "{0}W{1}:{2}{3}", Pre, Offset, Length, Post );
                break;
              }
            return Dst;
        }
        string ConvertToString_S7300()
        {
            char InputChar = 'I';
            char OutputChar = 'Q';
            char CounterChar = 'C';
            
            string Dst = string.Empty;
            string Pre = string.Empty;
            string Post = string.Empty;

            if (!Valid)
                return Dst;

            if (German)
            {
                InputChar = 'E';
                OutputChar = 'A';
                CounterChar = 'Z';
            }

            switch (Area)
            {
                case Step7Area.aP:
                    Pre = "P";
                    break;
                case Step7Area.aPE:
                    Pre = string.Format("P{0}", InputChar);
                    break;
                case Step7Area.aPA:
                    Pre = string.Format("P{0}", OutputChar);
                    break;
                case Step7Area.aI:
                    Pre = string.Format("{0}", InputChar);
                    break;
                case Step7Area.aQ:
                    Pre = string.Format("{0}", OutputChar);
                    break;
                case Step7Area.aM:
                    Pre = "M";
                    break;
                case Step7Area.aD:
                    if (Step7Format.frmBit == Format)
                        Pre = string.Format("DB{0}.DBX", DbNumber);
                    else
                        Pre = string.Format("DB{0}.DB", DbNumber);
                    break;
                case Step7Area.aT:
                    if (1 == Length)
                        Dst = string.Format("T{0}", Offset);
                    else
                        Dst = string.Format("T{0}:{1}", Offset, Length);
                    return Dst;
                case Step7Area.aC:
                    if (1 == Length)
                        Dst = string.Format("{0}{1}", CounterChar, Offset);
                    else
                        Dst = string.Format("{0}{1}:{2}", CounterChar, Offset, Length);
                    return Dst;
            }

            switch (Format)
            {
                case Step7Format.frmBit:
                    if (1 == Length)
                        Dst = string.Format("{0}{1}.{2}", Pre, Offset, Bit);
                    else
                    {
                        Dst = string.Format("{0}{1}.{2}:{3}", Pre, Offset, Bit, Length);
                    }
                    break;
                case Step7Format.frmByte:
                    if (1 == Length)
                        Dst = string.Format("{0}B{1}", Pre, Offset);
                    else
                        Dst = string.Format("{0}B{1}:{2}", Pre, Offset, Length);
                    break;
                case Step7Format.frmDWord:
                    if (1 == Length)
                        Dst = string.Format("{0}D{1}", Pre, Offset);
                    else
                        Dst = string.Format("{0}D{1}:{2}", Pre, Offset, Length);
                    break;
                case Step7Format.frmWord:
                    switch (Trans)
                    {
                        case Step7WordTrans.wtW:
                            Post = string.Empty;
                            break;
                        case Step7WordTrans.wtC:
                            Post = string.Format(",{0}", CounterChar);
                            break;
                        case Step7WordTrans.wtT:
                            Post = ",T";
                            break;
                        /*case wtT001:
                          Post = CString( _T( ",T" ) ) + strFormatTimer1;
                          break;
                        case wtT01:
                          Post = CString( _T( ",T" ) ) + strFormatTimer2;
                          break;
                        case wtT1:
                          Post = CString( _T( ",T" ) ) + strFormatTimer3;
                          break;
                        case wtT10:
                          Post = CString( _T( ",T" ) ) + strFormatTimer4;
                          break;*/
                    }

                    if (1 == Length)
                        Dst = string.Format("{0}W{1}{2}", Pre, Offset, Post);
                    else
                        Dst = string.Format("{0}W{1}:{2}{3}", Pre, Offset, Length, Post);
                    break;
            }
            return Dst;
        }
        bool ParsePlcAddress(string Str)
        {

            if (String.IsNullOrEmpty(Str))
            {
                return (false);
            }

            if (!S7_200)
                return ParseS7300(Str);
            else
                return ParseS7200(Str);
        }

        bool ParseS7200(string Str)
        {
            if (String.IsNullOrEmpty(Str))
            {
                return (false);
            }

            string PureStr = string.Empty;
            string up = Str.ToUpper();
            TrimSpaces(ref PureStr, up);

            German = false;
            //bool Valid = false;
            Valid = false;

            if (PureStr.Length < 2)
                return false;

            string szAdd;
            switch (PureStr[0])
            {
                case 'E':
                case 'I':
                    if (PureStr[0] == 'E')
                        German = true;
                    Area = Step7Area.aI;
                    break;
                case 'A':
                    if (PureStr[1] == 'I')
                    {
                        if (PureStr.Length < 3)
                            return false;
                        if (ParseByte('W', PureStr.Substring(2)))
                        {
                            Area = Step7Area.aAI;
                            Valid = true;
                        }
                        return (Valid);
                    }
                    else if (PureStr[1] == 'Q')
                    {
                        if (PureStr.Length < 3)
                            return false;
                        if (ParseByte('W', PureStr.Substring(2)))
                        {
                            Area = Step7Area.aAQ;
                            Valid = true;
                        }
                        return (Valid);
                    }
                    else
                    {
                        German = true;
                        Area = Step7Area.aQ;
                    }
                    break;
                case 'Q':
                    Area = Step7Area.aQ;
                    break;
                case 'M':
                    Area = Step7Area.aM;
                    break;
                case 'V':
                    Area = Step7Area.aD;
                    DbNumber = 1;
                    break;
                /*case _T( 'T' ):
                    if( ParseTC( PureStr.Mid( 1 ) ) ) {
                        m_Area = aTIEC;
                        m_Valid = true;
                    }
                    return m_Valid;
                break;*/
                case 'T':
                    if (ParseTC(PureStr.Substring(1)))
                    {
                        Area = Step7Area.aTIEC;
                        Format = Step7Format.frmWord;
                        Trans = Step7WordTrans.wtT;
                        Valid = true;
                    }
                    return Valid;
                    break;
                case 'Z':
                case 'C':
                    if (PureStr[0] == 'Z')
                        German = true;
                    /*if( ParseTC( PureStr.Mid( 1 ) ) ) {
                        m_Area = aCIEC;
                        m_Valid = true;
                    }
                    return m_Valid;*/
                    if (ParseTC(PureStr.Substring(1)))
                    {
                        Area = Step7Area.aCIEC;
                        Format = Step7Format.frmWord;
                        Trans = Step7WordTrans.wtC;
                        Valid = true;
                    }
                    return Valid;
                    break;
                case 'S':
                    Area = Step7Area.aS;
                    break;
                case 'H':
                    if (ParseByte('D', PureStr.Substring(1)))
                    {
                        Area = Step7Area.aH;
                        Valid = true;
                    }
                    return Valid;
                    break;
                default:
                    return false;
                    break;
            } // End of switch( PureStr[ 0 ] )

            switch (PureStr[1])
            {
                case 'B':
                case 'W':
                case 'D':
                    if (PureStr.Length < 3)
                        return false;
                    if (ParseByte(PureStr[1], PureStr.Substring(2)))
                        Valid = true;
                    break;
                default:
                    if (ParseBit(PureStr.Substring(1)))
                        Valid = true;
                    break;
            }

            return Valid;
        }

        bool ParseS7300( string Str )
        {
            if(String.IsNullOrEmpty(Str))
            {
              return (false);
            }

            string PureStr = string.Empty;
            string up = Str.ToUpper();
            TrimSpaces(ref PureStr, up);

            German = false;
            //bool Valid = false;
            Valid = false;

            Area = Step7Area.aP;
            Format = Step7Format.frmBit;
            Trans = Step7WordTrans.wtW;
            Offset   = 0;
            Length   = 1;
            DbNumber = 0;
            Bit      = 0;
            S7_200 = false;

            if( PureStr.Length < 2 )
            return false;

            // Added in version 10.1.0.2 (FOGBUGZ 1961)
            int nPIndex = 1;
            int nPType = 0;

            switch( PureStr[ 0 ] ) {
                case 'E':
                case 'I':
                    if (PureStr[0] == 'E')
                        German = true;
                    Area = Step7Area.aI;
                    break;
                case 'A':
                case 'Q':
                    if (PureStr[0] == 'A')
                        German = true;
                    Area = Step7Area.aQ;
                    break;
                case 'M':
                    Area = Step7Area.aM;
                    break;
                case 'P':
                    if( (PureStr[1] == 'E') || (PureStr[1] == 'I') ) {
	                    nPIndex = 2;
	                    nPType = 1;
	                    if( PureStr[1] == 'E' ) {
		                    German = true;
	                    }
                    }
                    else if((PureStr[1] == 'A') || (PureStr[1] == 'Q')) {
	                    nPIndex = 2;
	                    nPType = 2;
	                    if( PureStr[1] == 'A' ) {
		                    German = true;
	                    }
                    }
                    if( nPIndex == 2 ) {
	                    if( PureStr.Length < 3 ) {
		                    return( false );
	                    }
                    }

                    switch( nPType ) {
	                    case 1:
                            Area = Step7Area.aPE;
	                        break;
	                    case 2:
                            Area = Step7Area.aPA;
	                        break;
	                    default:
                            Area = Step7Area.aP;
	                        break;
                    }
                    break;

                case 'D':
                    if( ParseDB( PureStr ) ) {
                        Area = Step7Area.aD;
                        Valid = true;
                    }
                    return Valid;
                case 'T':
                    if( ParseTC( PureStr.Substring( 1 ) ) ) {
                        Area = Step7Area.aT;
                        Format = Step7Format.frmWord;
                        Trans = Step7WordTrans.wtT;
                        Valid = true;
                    }
                    return Valid;

                case 'Z':
                case 'C':
                    if (PureStr[0] == 'Z')
                        German = true;
                    if( ParseTC( PureStr.Substring( 1 ) ) ) {
                        Area = Step7Area.aC;
                        Format = Step7Format.frmWord;
                        Trans = Step7WordTrans.wtC;
                        Valid = true;
                    }
                    return Valid;
            }

            // Modified in version 10.1.0.2 (FOGBUGZ 1961)
            //switch( PureStr[ 1 ] ) {
            switch( PureStr[ nPIndex ] ) {
                case 'B':
                case 'W':
                case 'D':
                if( PureStr.Length < (nPIndex + 2) ) {
	                Valid = false;
                }
                else if( ParseByte( PureStr[ nPIndex ], PureStr.Substring( nPIndex + 1 ) ) )
	                Valid = true;
                break;
                default:
                if( PureStr.Length < (nPIndex + 1) ) {
	                Valid = false;
                }
                else if( ParseBit( PureStr.Substring( nPIndex ) ) )
                    Valid = true;
                else
                    Valid = false;

                break;
            }
            return Valid;
        }
        bool ParseByte( char type, string Str )
        {
          int offs, len;
          string substr;
          string transform;

          switch( type ) {
              case 'B':
                Format = Step7Format.frmByte;
                break;
              case 'W':
                Format = Step7Format.frmWord;
                Trans = Step7WordTrans.wtW;
                break;
              case 'D':
                Format = Step7Format.frmDWord;
                break;
	            // default case added in version 10.0.0.14
              default:
	              return( false );
          }
        
        if(String.IsNullOrEmpty(Str) )
        {
            return (false);
        }
         int result = 0;
         bool found = false;
         string temp = Str;
         do
         {
             if (int.TryParse(temp, out offs))
             {
                 found = true;
                 break;
             }

            temp = temp.Remove(temp.Length - 1);
                  
         } while (temp.Length > 0);

         substr = Str.Substring(temp.Length);
         if (found && substr.Length == 0)
         {
             Offset = offs;
             Length = 1;
             return true;
         }
         else if (!found)
             return false;
          /*
          switch( sscanf( Str, "%d%s", offs, substr ) ) {
          case 1:
            Offset = offs;
            m_Length = 1;
            return true;
          case 2:
            break;
          default:
            return false;
          }*/

          switch( Format ) {
          case Step7Format.frmByte:
          case Step7Format.frmDWord:
            temp = substr.Substring(1);
            found = false;
                   
            len = 0;
            while (temp.Length > 0)
            {
                if (int.TryParse(temp, out len))
                {
                    found = true;
                    break;
                }
                temp = temp.Remove(temp.Length - 1);

            } 

            transform = substr.Substring(temp.Length);

            if( 0 == len )
              return false;

            // invalidate string's size too big
            if (len > S7Protocol.DEFAULTSTRINGLENGTH_TIAPORTAL)
                return false;

            Length = len;

	        // Added in version 10.0.0.14
	        Offset = offs;
            return true;
          case Step7Format.frmWord:
            temp = substr.Substring(1);
            found = false;

            len = 0;
            while (temp.Length > 0)
            {
                if (int.TryParse(temp, out len))
                {
                    found = true;
                    break;
                }
                temp = temp.Remove(temp.Length - 1);
            }
            transform = substr.Substring(1).Substring(temp.Length);

            if ((0 == len) && (substr.Length == 0))
                return false;

            if (!found)
            {
                if (substr[0] != ',')
                    return false;

                Offset = offs;
                Length = 1;

                return ParseConversion(transform);

            }
            else if (found && transform.Length == 0)
            {
                // invalidate string's size too big
                if (len >= S7Protocol.DEFAULTSTRINGLENGTH_TIAPORTAL)
                    return false;

                Trans = Step7WordTrans.wtW;
                Offset = offs;
                
                Length = len;
                return true;
            }
            else if (found && transform.Length > 0)
            {
                if (',' != transform[0])
                    return false;
                Offset = offs;
                Length = len;

                return ParseConversion(transform.Substring(1));
            }
            break;
          }

          return false;
        }

        bool ParseTC( string Str )
        {
            string substr;
            string substr2;
            int offs, len;

            bool found = false;
            if(String.IsNullOrEmpty(Str))
            {
                return (false);
            }
            string temp = Str;
            do
            {
                if (int.TryParse(temp, out offs))
                {
                    found = true;
                    break;
                }
                temp = temp.Remove(temp.Length - 1);
            } while (temp.Length > 0);
            substr = Str.Substring(temp.Length); 
            if (found && substr.Length == 0)
            {
                Offset = offs;
                Length = 1;
                return true;
            }
            else if (found && substr.Length > 0)
            {
                found = false;
                temp = substr;
                do
                {
                    if (int.TryParse(temp, out len))
                    {
                        found = true;
                        break;
                    }
                    temp = temp.Remove(temp.Length - 1);
                } while (temp.Length > 0);
                substr2 = substr.Substring(temp.Length); 
                if (!found || substr2.Length > 0)
                    return false;
                Offset = offs;
                Length = len;
                return true;
            }
            return false;
        }

        bool ParseBit( string Str )
        {
          int offs, bitn, len;

            if(String.IsNullOrEmpty(Str))
            {
                return (false);
            }

          int point = Str.IndexOf('.');
          int colon = Str.IndexOf(':');
          if (point != -1 && colon != -1 && point < colon)
          { 
              //3
              if (!int.TryParse(Str.Substring(0, point + 1), out offs))
                  return false;
              if (!int.TryParse(Str.Substring(point + 1, colon-point + 1), out bitn))
                  return false;
              if (!int.TryParse(Str.Substring(colon + 1), out len))
                  return false;
              if (bitn > 7)
                  return false;

              if (len == 0)
                  return false;
              Format = Step7Format.frmBit;
              Offset = offs;
              Bit = bitn;
              Length = len;
              return true;
          }
          else if (point != -1 && colon == -1)
          { 
              //2
              if (!int.TryParse(Str.Substring(0, point), out offs))
                  return false;
              if (!int.TryParse(Str.Substring(point + 1), out bitn))
                  return false;

              Format = Step7Format.frmBit;
              Offset = offs;
              Bit = bitn;
              Length = 1;
              return true;
          }
          return false;
        }

        bool ParseDB( string Str )
        {
          string substr;
          int Db;

          if(String.IsNullOrEmpty(Str))
          {
              return (false);
          }

          int nidx = Str.IndexOf("DB");
          if (nidx == -1)
              return false;
          string temp = Str.Substring(nidx+2);
          nidx = temp.IndexOf('.');
          if (nidx == -1)
              return false;
          if (!int.TryParse(temp.Substring(0, nidx), out Db))
              return false;
          substr = temp.Substring(nidx + 1);
          if (substr.Length == 0)
              return false;
          if (substr.IndexOf("DB") != 0)
              return false;
          substr = substr.Substring(2);

          if (String.IsNullOrEmpty(substr))
          {
              return (false);
          }

            DbNumber = Db;
          if( 'X' == substr[ 0 ] )
            return ParseBit( substr.Substring(1));
          return ParseByte( substr[ 0 ], substr.Substring(1));
        }

        bool ParseConversion( string Str )
        {
          if( 1 != Str.Length)
            return false;

          switch( Str[ 0 ] ) {
          case 'Z':
          case 'C':
            if(Str[0] == 'Z')
                German = true;
            Trans = Step7WordTrans.wtC;
            return true;
          case 'T':
            Trans = Step7WordTrans.wtT;
            return true;
          }
          return false;
        }

        #endregion

        public static bool StaticParseAddress(string Str, ref Step7Area area, ref int dbnumber,
            ref Step7Format format, ref Step7WordTrans trans, ref int offset, ref int bit, ref int length, ref bool s7_200)
        {
            string PureStr = string.Empty;

            Str = Str.ToUpper();
            TrimSpaces(ref PureStr, Str);

            bool Valid = false;

            
            Regex dbParser = new Regex(@"^DB(?<num>\d+).DB(?<tipo>\w)(?<add>\d+)(?<bit>.\d+)?(?<len>:\d+})?(?<conv>,\w)?");

            Match dbMatch = dbParser.Match(Str);
            if (dbMatch.Success)
            {
                //DB type address
                area = Step7Area.aD;
                dbnumber = Convert.ToInt32(dbMatch.Groups["num"].Value);
                format = GetS7Type(dbMatch.Groups["tipo"].Value);
                if (dbMatch.Groups["conv"].Value == "T")
                    trans = Step7WordTrans.wtT;
                else if (dbMatch.Groups["conv"].Value == "C")
                    trans = Step7WordTrans.wtC;
                offset = Convert.ToInt32(dbMatch.Groups["add"].Value);
                if (dbMatch.Groups["bit"].Value.Length > 0)
                    bit = Convert.ToInt32(dbMatch.Groups["bit"].Value);
                if (dbMatch.Groups["len"].Value.Length > 0)
                    length = Convert.ToInt32(dbMatch.Groups["len"].Value);
                Valid = true;
                s7_200 = false;
            }
            else
            {
                Regex addParser = new Regex(@"^(?<area>[EIAQPEMFTZC]+)(?<tipo>[BWDX]{1})?(?<add>\d+)(?<bit>.\d+)?(?<len>:\d+})?(?<conv>,[TC]{1})?");
                Match addMatch = addParser.Match(Str);
                if (addMatch.Success)
                {
                    //Area address
                    if(addMatch.Groups["tipo"].Value.Length > 0)
                        format = GetS7Type(addMatch.Groups["tipo"].Value);
                    if (addMatch.Groups["conv"].Value == "T")
                        trans = Step7WordTrans.wtT;
                    else if (addMatch.Groups["conv"].Value == "C")
                        trans = Step7WordTrans.wtC;
                    offset = Convert.ToInt32(addMatch.Groups["add"].Value);
                    string sbit = addMatch.Groups["bit"].Value;
                    sbit = sbit.Trim(new char[]{'.'});
                    if (sbit.Length > 0)
                        bit = Convert.ToInt32(sbit);
                    if (addMatch.Groups["len"].Value.Length > 0)
                        length = Convert.ToInt32(addMatch.Groups["len"].Value);

                    string s = addMatch.Groups["area"].Value;
                    area = GetS7StaticArea(s);
                    if (s == "AI" || s == "AQ" || s == "V" || s == "S" || s == "H")
                        s7_200 = true;
                    else if (s == "P" || s == "PE" || s == "PA")
                        s7_200 = false;
                    Valid = true;
                }
            }
            return Valid;
        }

        bool ParseData200(string Str)
        {
            string PureStr = string.Empty;

            Str.ToUpper();
            TrimSpaces(ref PureStr, Str);

            German = false;
            //bool Valid = false;
            
            return Valid;
        }

        #region Properties
        //private UFUAModel.DataType _VarType;
        //[Category("General")]
        //[Description("Variable Type")]
        //public override UFUAModel.DataType VarType
        //{
        //    get
        //    {
        //        return _VarType;
        //    }
        //    set
        //    {
        //        _VarType = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //    }
        //}
        //private uint _ArrayDimension;
        //[Category("General")]
        //[Description("Array Dimension")]
        //public override uint ArrayDimension
        //{
        //    get
        //    {
        //        return _ArrayDimension;
        //    }
        //    set
        //    {
        //        _ArrayDimension = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
        //    }
        //}

        ///// <summary>   Number of element to exchange. </summary>
        //private int _ElementNumber;
        //[Category("General")]
        //[Description("Element Number")]
        //public override int ElementNumber
        //{
        //    get { return _ElementNumber; }
        //    set
        //    {
        //        _ElementNumber = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
        //    }
        //}
        private string _StartAddress;
        [Category("Device Data")]
        [Description("Address")]
        public string StartAddress
        {
            get { return _StartAddress; }
            set
            {
                _StartAddress = value;
                OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
            }
        }
        private Step7Area _Area;
        [Category("Device Data")]
        [Description("Data Area")]
        public Step7Area Area
        {
            get { return _Area; }
            set { _Area = value; }
        }
        private Step7Format _Format;
        [Category("Device Data")]
        [Description("Data Format")]
        public Step7Format Format
        {
            get { return _Format; }
            set { _Format = value; }
        }
        private Step7WordTrans _Trans;
        [Category("Device Data")]
        [Description("Data Transformation")]
        public Step7WordTrans Trans
        {
            get { return _Trans; }
            set { _Trans = value; }
        }
        private int _Offset;
        [Category("Device Data")]
        [Description("Data Offset")]
        public int Offset
        {
            get { return _Offset; }
            set { _Offset = value; }
        }
        private int _Length;
        [Category("Device Data")]
        [Description("Data Length")]
        public int Length
        {
            get { return _Length; }
            set { _Length = value; }
        }
        private int _DbNumber;
        [Category("Device Data")]
        [Description("Data Block")]
        public int DbNumber
        {
            get { return _DbNumber; }
            set { _DbNumber = value; }
        }
        private int _Bit;
        [Category("Device Data")]
        [Description("Bit number")]
        public int Bit
        {
            get { return _Bit; }
            set { _Bit = value; }
        }
        private bool _German;
        [Category("Device Data")]
        [Description("German syntax")]
        public bool German
        {
            get { return _German; }
            set { _German = value; }
        }
        private bool _S7_200;
        [Category("Device Data")]
        [Description("S720 Addressing")]
        public bool S7_200
        {
            get { return _S7_200; }
            set { _S7_200 = value; }
        }
        private bool _LenStringEnable;
        [Category("Device Data")]
        [Description("Len String Enable")]
        public bool LenStringEnable
        {
            get { return _LenStringEnable; }
            set { _LenStringEnable = value; }
        }

        private bool _SwapDWords;
        [Category("Device Data")]
        [Description("Swap DWords")]
        public bool SwapDWords
        {
            get { return _SwapDWords; }
            set
            {
                _SwapDWords = value;
            }
        }

        private string _StructStringFieldLengths;
        [Category("Device Data")]
        [Description("Max. Lengths of the Structure Fields of String Type")]
        [Size(SizeAttribute.Unlimited)]
        public string StructStringFieldLengths
        {
            get { return _StructStringFieldLengths; }
            set
            {
                _StructStringFieldLengths = value;
                OnPropertyChanged("StructStringFieldLengths");
            }
        }

        private bool _IsMemberOfStruct;
        [Category("Device Data")]
        [Description("Is Member Of Struct")]        
        public bool IsMemberOfStruct
        {
            get { return _IsMemberOfStruct; }            
        }
        #endregion
        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "StartAddress")
            {
                if (!ParseAddress(StartAddress))
                {
                    return UFUAModel.Properties.Resources.InvalidDynamcSettings;
                }
                //if (InvalidStringSize())
                //    return Properties.Resources.StrngSizeOutOfRange;
                if (InvalidStringArray())
                    return UFUAModel.Properties.Resources.ArraysOfStringsAreInvalid;
                if ((uint)VarType != unchecked((uint)(-1)))
                {
                    if ((Trans == Step7WordTrans.wtT) && (VarType != UFUAModel.DataType.UInt32))
                    {
                        return Properties.Resources.InvalidVariableTypeForDataFormatS5TIME;
                    }
                    if (isProtocolBool() && VarType != UFUAModel.DataType.Boolean)
                        return UFUAModel.Properties.Resources.DataTypeIncompatible;
                    return ProtocolDataSizeValidation(VarType);
                }
            }
            if (propertyName == "ArrayDimension")
            {
                if (ParseAddress(StartAddress))
                {
                    if (InvalidStringArray())
                        return UFUAModel.Properties.Resources.ArraysOfStringsAreInvalid;
                }
            }

            if (propertyName == "SwapDWords")
            {
                if (SwapDWords == true)
                {
                    if ((VarType == UFUAModel.DataType.Boolean) ||
                        ((VarType == UFUAModel.DataType.Byte) && (ArrayDimension < 8)) ||
                        ((VarType == UFUAModel.DataType.SByte) && (ArrayDimension < 8)) ||
                        ((VarType == UFUAModel.DataType.Int16) && (ArrayDimension < 4)) ||
                        ((VarType == UFUAModel.DataType.UInt16) && (ArrayDimension < 4)) ||
                        ((VarType == UFUAModel.DataType.Int32) && (ArrayDimension < 2)) ||
                        ((VarType == UFUAModel.DataType.UInt32) && (ArrayDimension < 2)) ||
                        ((VarType == UFUAModel.DataType.Float) && (ArrayDimension < 2)) ||
                        ((VarType == UFUAModel.DataType.Float) && (ArrayDimension < 2)))
                    {
                        return Properties.Resources.SwapDWordsNotAdmitted;
                    }
                }
            }

            return null;
        }
        private bool InvalidStringArray()
        {
            return (VarType == UFUAModel.DataType.String && ArrayDimension != 0 );
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "VarType":
                    OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;

                case "ArrayDimension":
                    OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ElementNumber"));
                    break;

                case "ElementNumber":
                    OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("VarType"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("ArrayDimension"));
                    break;
            }
        }
        #endregion
    }
}
