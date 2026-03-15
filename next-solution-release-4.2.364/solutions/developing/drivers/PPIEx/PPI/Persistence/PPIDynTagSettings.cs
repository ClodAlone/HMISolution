using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using DriverCodeBaseEx;
using DriverCodeBaseEx.Helpers;
using DriverCodeBaseEx.Enumerators;
using System.Text.RegularExpressions;
using DriverBaseInterfaces;
using Opc.Ua;
using UFUAModel;

namespace PPI
{
    public sealed class PPIDynTagSettings : DynTagSettings
    {
        #region Constructors

        public PPIDynTagSettings()
            : base()
        {
            German = false;
            Area = Step7Area.aP;
            Format = Step7Format.frmBit;
            Trans = Step7WordTrans.wtW;
            Offset = 0;
            Length = 1;
            Bit = 0;

            Valid = false;
        }

        bool Valid = false;
        #endregion
        
        #region Static Members

        private static readonly String StartAddressParameter = "SA";
        
        #endregion

        #region Override Functions

        public override void Parse(String dynamicSettings)
        {
            base.Parse(dynamicSettings);

            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            StartAddress = helper.GetPartByName(StartAddressParameter);

            bool ret = ParseAddress(StartAddress);
        }

        public override bool TryParse(String dynamicSettings)
        {
            if (!base.TryParse(dynamicSettings))
                return false;
            
            DynamicStringParser helper = new DynamicStringParser(dynamicSettings);
            StartAddress = helper.GetPartByName(StartAddressParameter);

            return ParseAddress(StartAddress);
        }

        public override string ToString()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            string saddr;
            saddr = ConvertToString_S7200();

            dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, saddr/*StartAddress*/);

            return dynamicstring.ToString();
        }

        public string ToStringSA()
        {
            var dynamicstring = new StringBuilder(base.ToString());
            dynamicstring.Append(DynamicStringParser.CharSep);
            
            dynamicstring.AppendFormat("{0}{1}{2}", StartAddressParameter, DynamicStringParser.CharAssign, StartAddress);

            return dynamicstring.ToString();
        }

        public override bool isTagByteSizeOk(uint ByteSize)
        {
            return (PPIProtocol.MAX_DATA_BYTES >= ByteSize);
        }
        public override UFUAModel.DataType getProtocolDataType()
        {
            ParseAddress(StartAddress);
            return (PPIProtocol.DataType(Format));
        }

        public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        {
            Step7Format newformat = Step7Format.frmInvalid;
            Step7Format prevformat = Step7Format.frmInvalid;

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
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        prevformat = Step7Format.frmDWord;
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
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        newformat = Step7Format.frmDWord;
                        break;
                }
            }
            if (newformat != Step7Format.frmInvalid)
                Format = newformat;
            if (thistagdefinition.DataType.IdType == IdType.Numeric && prevtagdefinition.DataType.IdType == IdType.Numeric)
            {
                if ((uint)thistagdefinition.DataType.Identifier == (uint)BuiltInType.String)
                    Offset += Length;
                else
                {
                    switch (prevformat)
                    {
                        case Step7Format.frmBit:
                            if ((uint)thistagdefinition.DataType.Identifier == (uint)BuiltInType.Boolean)
                            {
                                Bit++;
                                Offset += (Bit / 8);
                                Bit %= 8;
                            }
                            else
                            {
                                Offset++;
                                Bit = 0;
                            }
                            Length = 1;
                            break;
                        case Step7Format.frmByte:
                            Offset += Length;
                            Bit = 0;
                            //Format = Step7Format.frmBit;
                            Length = 1;
                            break;
                        case Step7Format.frmWord:
                            Offset += 2 * Length;
                            Bit = 0;
                            //Format = Step7Format.frmBit;
                            Length = 1;
                            break;
                        default:
                            Offset += 4 * Length;
                            Bit = 0;
                            //Format = Step7Format.frmBit;
                            Length = 1;
                            break;
                    }
                    switch ((uint)thistagdefinition.DataType.Identifier)
                    {
                        case (uint)BuiltInType.Boolean:
                        case (uint)BuiltInType.SByte:
                        case (uint)BuiltInType.Byte:
                            break;
                        case (uint)BuiltInType.Int16:
                        case (uint)BuiltInType.UInt16:
                            Offset = Offset + Offset % 2;
                            break;
                        case (uint)BuiltInType.Float:
                        case (uint)BuiltInType.UInt32:
                        case (uint)BuiltInType.Int32:
                        case (uint)BuiltInType.UInt64:
                        case (uint)BuiltInType.Int64:
                        case (uint)BuiltInType.Double:
                            Offset = ((Offset + 3) / 4) * 4;
                            break;
                        default:
                            return string.Empty;
                    }
                }
            }
            
            //if (newformat != Step7Format.frmInvalid)
            //    Format = newformat;
            return ToString();
        }

        //public override string GetNextDynSetting(TagDefinition prevtagdefinition, TagDefinition thistagdefinition)
        //{
        //    Step7Format newformat = Step7Format.frmInvalid;

        //    if (thistagdefinition.DataType.IdType == IdType.Numeric)
        //    {
        //        switch ((uint)thistagdefinition.DataType.Identifier)
        //        {
        //            case (uint)BuiltInType.Boolean:
        //                newformat = Step7Format.frmBit;
        //                break;
        //            case (uint)BuiltInType.SByte:
        //            case (uint)BuiltInType.Byte:
        //                newformat = Step7Format.frmByte;
        //                break;
        //            case (uint)BuiltInType.Int16:
        //            case (uint)BuiltInType.UInt16:
        //                newformat = Step7Format.frmWord;
        //                break;
        //            case (uint)BuiltInType.Float:
        //            case (uint)BuiltInType.UInt32:
        //            case (uint)BuiltInType.Int32:
        //            case (uint)BuiltInType.UInt64:
        //            case (uint)BuiltInType.Int64:
        //            case (uint)BuiltInType.Double:
        //                newformat = Step7Format.frmDWord;
        //                break;
        //        }
        //    }
        //    if (newformat != Step7Format.frmInvalid)
        //        Format = newformat;
        //    if (prevtagdefinition.DataType.IdType == IdType.Numeric)
        //    {
        //        switch ((uint)prevtagdefinition.DataType.Identifier)
        //        {
        //            case (uint)BuiltInType.Boolean:
        //                switch (Format)
        //                { 
        //                    case Step7Format.frmBit:
        //                        Bit++;
        //                        Offset += (Bit/8);
        //                        Bit %= 8;
        //                        Length = 1;
        //                        break;
        //                    case Step7Format.frmByte:
        //                        Offset += Length;
        //                        Bit = 0;
        //                        //Format = Step7Format.frmBit;
        //                        Length = 1;
        //                        break;
        //                    case Step7Format.frmWord:
        //                        Offset += 2*Length;
        //                        Bit = 0;
        //                        //Format = Step7Format.frmBit;
        //                        Length = 1;
        //                        break;
        //                    default:
        //                        Offset += 4*Length;
        //                        Bit = 0;
        //                        //Format = Step7Format.frmBit;
        //                        Length = 1;
        //                        break;
        //                }
        //                break;
        //            case (uint)BuiltInType.SByte:
        //            case (uint)BuiltInType.Byte:
        //                switch (Format)
        //                {
        //                    case Step7Format.frmBit:
        //                        Offset++;
        //                        Bit = 0;
        //                        //Format = Step7Format.frmByte;
        //                        Length = 1;
        //                        break;
        //                    case Step7Format.frmByte:
        //                        Offset += Length;
        //                        Bit = 0;
        //                        Length = 1;
        //                        break;
        //                    case Step7Format.frmWord:
        //                        Offset += 2 * Length;
        //                        Bit = 0;
        //                        //Format = Step7Format.frmByte;
        //                        Length = 1;
        //                        break;
        //                    default:
        //                        Offset += 4 * Length;
        //                        Bit = 0;
        //                        //Format = Step7Format.frmByte;
        //                        Length = 1;
        //                        break;
        //                }
        //                break;
        //            case (uint)BuiltInType.Int16:
        //            case (uint)BuiltInType.UInt16:
        //                switch (Format)
        //                {
        //                    case Step7Format.frmBit:
        //                        Offset ++;
        //                        if( Offset%2 > 0) {
        //                            Offset ++;
        //                        }
        //                        Bit = 0;
        //                        //Format = Step7Format.frmWord;
        //                        Length = 1;
        //                        break;
        //                    case Step7Format.frmByte:
        //                        Offset += Length;
        //                        if( Offset%2 > 0 ) {
        //                            Offset ++;
        //                        }
        //                        Bit = 0;
        //                        //Format = Step7Format.frmWord;
        //                        Length = 1;
        //                        break;
        //                    case Step7Format.frmWord:
        //                        Offset += 2*Length;
        //                        if( Offset%2 > 0 ) {
        //                            Offset ++;
        //                        }
        //                        Bit = 0;
        //                        Length = 1;
        //                        break;
        //                    default:
        //                        Offset += 4*Length;
        //                        if( Offset%2 > 0 ) {
        //                            Offset ++;
        //                        }
        //                        Bit = 0;
        //                        //Format = Step7Format.frmWord;
        //                        Length = 1;
        //                        break;
        //                }
        //                break;
        //            case (uint)BuiltInType.Float:
        //            case (uint)BuiltInType.UInt32:
        //            case (uint)BuiltInType.Int32:
        //            case (uint)BuiltInType.UInt64:
        //            case (uint)BuiltInType.Int64:
        //            case (uint)BuiltInType.Double: 
        //            switch (Format)
        //                {
        //                    case Step7Format.frmBit:
        //                        Offset ++;
        //                        if( Offset%2 > 0) {
        //                            Offset ++;
        //                        }
        //                        Bit = 0;
        //                        //Format = Step7Format.frmWord;
        //                        Length = 1;
        //                        break;
        //                    case Step7Format.frmByte:
        //                        Offset += Length;
        //                        if( Offset%2 > 0 ) {
        //                            Offset ++;
        //                        }
        //                        Bit = 0;
        //                        //Format = Step7Format.frmDWord;
        //                        Length = 1;
        //                        break;
        //                    case Step7Format.frmWord:
        //                        Offset += 2*Length;
        //                        if( Offset%2 > 0) {
        //                            Offset ++;
        //                        }
        //                        Bit = 0;
        //                        //Format = Step7Format.frmDWord;
        //                        Length = 1;
        //                        break;
        //                    default:
        //                        Offset += 4*Length;
        //                        if( Offset%2 > 0) {
        //                            Offset ++;
        //                        }
        //                        Bit = 0;
        //                        //Format = Step7Format.frmDWord;
        //                        Length = 1;
        //                        break;
        //                }
        //                break;
        //            case (uint)BuiltInType.String:
        //                Offset += Length;
        //                break;
        //            default:
        //                return string.Empty;
        //        }
        //    }
        //    //if (newformat != Step7Format.frmInvalid)
        //    //    Format = newformat;
        //    return ToString();
        //}

        #endregion

        public bool ParseAddress(string address)
        {
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
            if(t.Length>0)
                switch (t[0])
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
                case "AI":
                    return Step7Area.aI;
                case "A":
                    return Step7Area.aQ;
                case "Q":
                    return Step7Area.aQ;
                case "AQ":
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
                case "S":
                    return Step7Area.aS;
                case "V":
                    return Step7Area.aD;
                case "H":
                    return Step7Area.aH;
                case "T":
                    return Step7Area.aT;
                case "Z":
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
                case "V":
                    return Step7Area.aD;
                case "S":
                    return Step7Area.aS;
                case "H":
                    Format = Step7Format.frmDWord;
                    Trans = Step7WordTrans.wtT;
                    return Step7Area.aH;
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
            Bit = 0;

            if (ParsePlcAddress(Str))
            {
                Valid = true;
            }
            
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
                            //Post = string.Empty;
                            break;
                        case Step7WordTrans.wtC:
                            //Post = string.Format(",{0}", CounterChar);
                            break;
                        case Step7WordTrans.wtT:
                            //Post = ",T";
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
                        Dst = string.Format("{0}W{1}", Pre, Offset);
                    else
                        Dst = string.Format("{0}W{1}:{2}", Pre, Offset, Length);
                    break;
            }
            return Dst;
        }

        bool ParsePlcAddress(string Str)
        {
            return ParseS7200(Str);
        }


        bool ParseS7200(string Str)
        {

            string PureStr = string.Empty;
            string up = Str.ToUpper();
            TrimSpaces(ref PureStr, up);

            German = false;
            //bool Valid = false;

            if (PureStr.Length < 2)
                return false;

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
                case 'S':
                    Area = Step7Area.aS;
                    break;
                case 'H':
                    if (ParseByte('D', PureStr.Substring(1)))
                    {
                        Area = Step7Area.aH;
                        Format = Step7Format.frmDWord;
                        Trans = Step7WordTrans.wtT;
                        Valid = true;
                    }
                    return Valid;
                default:
                    return false;
            } // End of switch( PureStr[ 0 ] )

            if (ParseBit(PureStr.Substring(1)))
                Valid = true;
            else
            {
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
                        return false;
                }
            }

            return Valid;
        }

        bool ParseByte(char type, string Str)
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
                Trans = Step7WordTrans.wtW;
                break;
	            // default case added in version 10.0.0.14
              default:
	              return( false );
          }

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
            do
            {
                if (int.TryParse(temp, out len))
                {
                    found = true;
                    break;
                }
                temp = temp.Remove(temp.Length - 1);
            } while (temp.Length > 0);
            transform = substr.Substring(temp.Length);

            if( 0 == len )
              return false;
            Length = len;

	        // Added in version 10.0.0.14
	        Offset = offs;
            return true;
          case Step7Format.frmWord:
            temp = substr.Substring(1);
            found = false;
            do
            {
                if (int.TryParse(temp, out len))
                {
                    found = true;
                    break;
                }
                temp = temp.Remove(temp.Length - 1);
            } while (temp.Length > 0);
            transform = substr.Substring(1).Substring(temp.Length);;
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
          else if (point != -1)
          {
              int temp = 0;
              bool found = false;
              do
              {
                  if (int.TryParse(Str.Substring(temp, point - temp), out offs))
                  {
                      found = true;
                      break;
                  }
                  temp++;
              } while (temp < point);
              if (!found)
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
          case 'H':
            Trans = Step7WordTrans.wtT;
            return true;
          }
          return false;
        }

        #endregion

        public static bool StaticParseAddress(string Str, ref Step7Area area, ref int dbnumber,
            ref Step7Format format, ref Step7WordTrans trans, ref int offset, ref int bit, ref int length)
        {
            //if(!StaticTestAddress(Str, out format))
            //    return false;

            string PureStr = string.Empty;

            Str = Str.ToUpper();
            TrimSpaces(ref PureStr, Str);

            Regex addParser = new Regex(PPIProtocol.regMemArea);
            Match addMatch = addParser.Match(Str);
            if (addMatch.Success)
            {
                string sbit = addMatch.Groups["bit"].Value;
                string stipo = addMatch.Groups["tipo"].Value;
                string sarea = addMatch.Groups["area"].Value;
                string sarea1 = addMatch.Groups["area1"].Value;
                format = Step7Format.frmInvalid;

                if (sbit.Length > 0)
                {
                    switch (sarea)
                    {
                        case "AI":
                        case "AQ":
                        case "H":
                            return false;
                    }
                    sbit = sbit.Trim(new char[] { '.' });
                    bit = Convert.ToInt32(sbit);
                    if (bit > 7)
                        return false;
                    format = Step7Format.frmBit;
                }
                //Area address
                else if (stipo.Length > 0)
                {
                    switch (sarea)
                    {
                        case "T":
                        case "C":
                        case "Z":
                        case "H":
                        case "AI":
                        case "AQ":
                            return false;
                    }
                    
                    format = GetS7Type(stipo);
                }
                else
                {
                    switch (sarea)
                    {
                        case "T":
                        case "C":
                        case "Z":
                            if (format == Step7Format.frmInvalid)
                                format = Step7Format.frmWord;
                            break;
                        case "H":
                            format = Step7Format.frmDWord;
                            break;
                        case "AI":
                        case "AQ":
                            format = Step7Format.frmWord;
                            break;
                    }
                }

                if (format == Step7Format.frmInvalid)
                    return false;


                offset = Convert.ToInt32(addMatch.Groups["add"].Value);
                if (addMatch.Groups["len"].Value.Length > 0)
                    length = Convert.ToInt32(addMatch.Groups["len"].Value);

                area = GetS7StaticArea(sarea);
                return  true;
            }
            return false;
        }

        public static bool StaticTestAddress(string Str, out Step7Format Format, out Step7Area Area, out int length)
        {

            Format = Step7Format.frmInvalid;
            Area = Step7Area.aInvalid;
            length = -1;
            string PureStr = string.Empty;

            if (string.IsNullOrWhiteSpace(Str))
                return false;

            Str = Str.ToUpper();
            TrimSpaces(ref PureStr, Str);

            Regex addParser = new Regex(PPIProtocol.regMemArea);
            Match addMatch = addParser.Match(Str);
            if (addMatch.Success)
            {
                string sbit = addMatch.Groups["bit"].Value;
                string stipo = addMatch.Groups["tipo"].Value;
                string sarea = addMatch.Groups["area"].Value;
                string slen = addMatch.Groups["len"].Value;
                int bit = -1;
                int len = -1;
                Format = Step7Format.frmInvalid;

                if (sbit.Length > 0)
                {
                    switch (sarea)
                    {
                        case "AI":
                        case "AQ":
                        case "H":
                            return false;
                    }
                    sbit = sbit.Trim(new char[] { '.' });
                    bit = Convert.ToInt32(sbit);
                    if (bit > 7)
                        return false;
                    Format = Step7Format.frmBit;
                }
                else if (stipo.Length > 0)
                {
                    switch (sarea)
                    {
                        case "T":
                        case "C":
                        case "Z":
                        case "H":
                        case "AI":
                        case "AQ":
                            return false;
                    }

                    Format = GetS7Type(stipo);
                }
                else
                {
                    switch (sarea)
                    {
                        case "T":
                        case "C":
                        case "Z":
                            if (Format == Step7Format.frmInvalid)
                                Format = Step7Format.frmWord;
                            break;
                        case "H":
                            Format = Step7Format.frmDWord;
                            break;
                        case "AI":
                        case "AQ":
                            Format = Step7Format.frmWord;
                            break;
                    }
                }

                if (Format == Step7Format.frmInvalid)
                    return false;

                switch (sarea)
                {
                    case "AI":
                    case "AQ":
                        if( (Convert.ToInt32(addMatch.Groups["add"].Value) & 1) == 1)
                            return false;
                        break;
                }
                Area = GetS7StaticArea(sarea);
                
                if (slen.Length > 0)
                {
                    switch (sarea)
                    {
                        case "T":
                        case "C":
                        case "Z":
                        case "H":
                        case "AI":
                        case "AQ":
                            return false;
                    }
                    slen = slen.Trim(new char[] { ':' });
                    len = Convert.ToInt32(slen);
                    if (len > PPIProtocol.MAX_DATA_BYTES)
                        return false;
                    length = len;
                }
                return true;
            }
            return false;



            //Regex regMemArea = new Regex(PPIProtocol.regMemArea);

            //Format = Step7Format.frmBit;
            //Match addMatch = regMemArea.Match(Str);
            //if (addMatch.Success)
            //    area = GetS7StaticArea(addMatch.Groups["area"].Value);

            //if (area == Step7Area.aInvalid)
            //    return false;

            //if (addMatch.Groups["tipo"].Value.Length > 0)
            //    Format = GetS7Type(addMatch.Groups["tipo"].Value);
            //else if (area == Step7Area.aC)
            //    Format = Step7Format.frmWord;
            //else if (area == Step7Area.aT)
            //    Format = Step7Format.frmDWord;
            
            //if (Format == Step7Format.frmInvalid)
            //    return false;
            
            //if (Format == Step7Format.frmBit)
            //{
            //    int bit = -1;
            //    string sbit = addMatch.Groups["bit"].Value.Trim(new char[] { '.' });
            //    if (sbit.Length > 0)
            //        bit = Convert.ToInt32(sbit);
            //    if (bit > 7 || bit < 0)
            //        return false;
            //}
            //else
            //    if (addMatch.Groups["bit"].Value.Length > 0)
            //        return false;

            //return  true;
        }

        #region Properties
        //private int/*LinkType*/ _TagLinkType;
        //[Category("General")]
        //[Description("Link Type")]
        //public override int/*LinkType*/ TagLinkType
        //{
        //    get { return _TagLinkType; }
        //    set
        //    {
        //        _TagLinkType = value;
        //        OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
        //        OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
        //    }
        //}
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
        //        OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
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

        /// <summary>   Number of element to exchange. </summary>
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
                OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
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
 
        #endregion
     
        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            /*if (propertyName == "FunctionCode")
            {
            }*/
            if (propertyName == "TagLinkType")
            {
                Step7Format Format;
                Step7Area Area;
                int length;
                if (StaticTestAddress(StartAddress, out Format, out Area, out length))
                    if (InvalidFunctionCodeLinkType(Area))
                        return DriverCodeBaseEx.Properties.Resources.JobTypeInvalid;
            }
            
            if (propertyName == "StartAddress")
            {
                Step7Format Format;
                Step7Area Area;
                int length;
                
                if (!StaticTestAddress(StartAddress, out Format, out Area, out length))
                    return Properties.Resources.ErrorInvalidAddress;
                if ((uint)VarType != unchecked((uint)(-1)))
                {

                    if ((uint)VarType != unchecked((uint)(-1)))
                        return ProtocolDataSizeValidation(VarType);
                    if (InvalidFunctionCodeLinkType(Area))
                        return DriverCodeBaseEx.Properties.Resources.JobTypeInvalid;
                }
            }

            return null;
        }
        private bool InvalidFunctionCodeLinkType(Step7Area Area)
        {
            switch ((LinkType)TagLinkType)
            {
                case LinkType.ExceptionOutput:
                case LinkType.UnconditionalOutput:
                case LinkType.InputOutput:
                    if (Area == Step7Area.aAI ||
                        Area == Step7Area.aH||
                        Area == Step7Area.aI)
                        return true;
                    break;
            }
            return false;
        }

        #endregion

        #region INotifyPropertyChanged Members

        protected override void OnPropertyChanged(string propertyName)
        {
            OnPropertyChanged(new PropertyChangedEventArgs(propertyName));

            switch (propertyName)
            {
                case "TagLinkType":
                    OnPropertyChanged(new PropertyChangedEventArgs("StartAddress"));
                    //OnPropertyChanged(new PropertyChangedEventArgs("OutputAtStartup"));
                    break;
                case "VarType":
                    OnPropertyChanged(new PropertyChangedEventArgs("TagLinkType"));
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
