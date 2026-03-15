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
using UFUAModel;

namespace MpiPcAdapter
{
    public sealed class MpiPcAdapterDynTagSettings : DynTagSettings
    {
        #region Constructors

        public MpiPcAdapterDynTagSettings()
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
            saddr = ConvertToString_S7300();

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
            return (MpiPcAdapterProtocol.MAX_JOB_DATA_BYTES >= ByteSize);
        }
        public override UFUAModel.DataType getProtocolDataType()
        {
            ParseAddress(StartAddress);
            return (MpiPcAdapterProtocol.DataType(Format));
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
                switch ((uint)thistagdefinition.DataType.Identifier)
                {
                    case (uint)BuiltInType.Boolean:
                        switch (prevformat)
                        {
                            case Step7Format.frmBit:
                                Bit++;
                                Offset += (Bit / 8);
                                Bit %= 8;
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
                        break;
                    case (uint)BuiltInType.SByte:
                    case (uint)BuiltInType.Byte:
                        switch (prevformat)
                        {
                            case Step7Format.frmBit:
                                Offset++;
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
                        switch (prevformat)
                        {
                            case Step7Format.frmBit:
                                Offset++;
                                if (Offset % 2 > 0)
                                {
                                    Offset++;
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
                        break;
                    case (uint)BuiltInType.Float:
                    case (uint)BuiltInType.UInt32:
                    case (uint)BuiltInType.Int32:
                    case (uint)BuiltInType.UInt64:
                    case (uint)BuiltInType.Int64:
                    case (uint)BuiltInType.Double:
                        switch (prevformat)
                        {
                            case Step7Format.frmBit:
                                Offset++;
                                if (Offset % 2 > 0)
                                {
                                    Offset++;
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
                        break;
                    case (uint)BuiltInType.String:
                        Offset += Length;
                        break;
                    default:
                        return string.Empty;
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

        static Step7Format GetS7Type(string t, ref DataType dataType)
        {
            if (t.Length > 0)
                switch (t[0])
                {
                    case 'B':
                        dataType = DataType.Byte;
                        return Step7Format.frmByte;
                    case 'W':
                        dataType = DataType.UInt16;
                        return Step7Format.frmWord;
                    case 'D':
                        dataType = DataType.UInt32;
                        return Step7Format.frmDWord;
                    case 'X':
                        dataType = DataType.Boolean;
                        return Step7Format.frmBit;
                }
            
            dataType = DataType.Boolean;
            return Step7Format.frmInvalid;
        }

        static Step7Area  GetS7StaticArea(string s, ref bool german, ref Step7WordTrans trans, ref int dbNumber)
        {
            german = false;
            trans = Step7WordTrans.wtW;
            switch (s)
            {
                case "E":
                    german = true;
                    return Step7Area.aI;
                case "I":
                    return Step7Area.aI;
                case "A":
                    german = true;
                    return Step7Area.aQ;
                case "Q":
                    return Step7Area.aQ;
                case "PE":
                    german = true;
                    return Step7Area.aPE;
                case "PA":
                    german = true;
                    return Step7Area.aPA;
                case "P":
                    return Step7Area.aP;
                case "M":
                    german = true;
                    return Step7Area.aM;
                case "F":
                    return Step7Area.aM;
                case "V":
                    dbNumber = 1;
                    return Step7Area.aD;
                case "S":
                    return Step7Area.aS;
                case "H":
                    return Step7Area.aH;
                case "D":
                    return Step7Area.aD;
                case "T":
                    trans = Step7WordTrans.wtT;
                    return Step7Area.aT;
                case "Z":
                    trans = Step7WordTrans.wtC;
                    return Step7Area.aC;
                case "C":
                    german = true;
                    trans = Step7WordTrans.wtC;
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
                    Format = Step7Format.frmDWord;
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

            if (ParsePlcAddress(Str))
            {
                Valid = true;
            }
            
            return Valid;
        }

        #region parse S7 address
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
            return ParseS7300(Str);
        }

 
        bool ParseS7300( string Str )
        {
            string PureStr = string.Empty;
            string up = Str.ToUpper();
            TrimSpaces(ref PureStr, up);

            if( PureStr.Length < 2 )
            return false;

            Step7Area area = Step7Area.aInvalid;
            int dbnumber = -1;
            Step7Format format = Step7Format.frmInvalid;
            Step7WordTrans trans = Step7WordTrans.wtW;
            int offset = 0;
            int bit = 0;
            bool german = false;
            int length = 0;
            Valid = StaticParseAddress(Str, ref area, ref dbnumber, ref format, ref trans, ref offset, ref bit, ref length, ref german);
            if(Valid)
            {
                Area = area;
                Format = format;
                Trans = trans;
                Offset = offset;
                Length = length;
                DbNumber = dbnumber;
                German = german;
                Bit = bit;

            }

            return Valid;
        }
 
        #endregion
        public static bool StaticParseAddress(string Str, ref Step7Area area, ref int dbnumber, ref Step7Format format,
            ref Step7WordTrans trans, ref int offset, ref int bit, ref int length, ref bool german)
        {
            DataType dataType;
            return StaticParseAddress(Str, ref area, ref dbnumber, ref format,ref trans, ref offset, ref bit, ref length, ref german, out dataType);
        }

        public static bool StaticParseAddress(string Str, ref Step7Area area, ref int dbnumber, ref Step7Format format,
            ref Step7WordTrans trans, ref int offset, ref int bit, ref int length, ref bool german, out DataType dataType)
        {
            dataType = DataType.UInt64;
            length = 0;
            if (Str == null)
                return false;

            string PureStr = string.Empty;

            Str = Str.ToUpper();
            TrimSpaces(ref PureStr, Str);

            bool Valid = true;

            Regex dbParser = new Regex(MpiPcAdapterProtocol.regDbArea);

            Match dbMatch = dbParser.Match(Str);
            if (dbMatch.Success)
            {
                //DB type address
                area = Step7Area.aD;
                dbnumber = Convert.ToInt32(dbMatch.Groups["num"].Value);
                offset = Convert.ToInt32(dbMatch.Groups["add"].Value);
                format = GetS7Type(dbMatch.Groups["tipo"].Value, ref dataType);

                if (dbMatch.Groups["conv"].Value.Length > 0)
                {
                    if (dataType != DataType.UInt16 )
                        Valid = false;
                    else if (dbMatch.Groups["conv"].Value == ",T")
                    {
                        dataType = DataType.UInt32;
                        trans = Step7WordTrans.wtT;
                    }
                    else if (dbMatch.Groups["conv"].Value == ",C")
                    {
                        trans = Step7WordTrans.wtC;
                    }
                    else
                        Valid = false;
                }

                string sbit = dbMatch.Groups["bit"].Value;
                sbit = sbit.Trim(new char[] { '.' });
                if (sbit.Length > 0)
                {
                    bit = Convert.ToInt32(sbit);
                    dataType = DataType.Boolean;
                }

                if (dbMatch.Groups["len"].Value.Length > 1)
                {
                    length = Convert.ToInt32(dbMatch.Groups["len"].Value.Remove(0, 1));
                    // invalidate string's size too big
                    if (length >= MpiPcAdapterProtocol.MAX_SINGLE_TASK_BYTE)
                        Valid = false;
                }
            }
            else
            {
                Regex addParser = new Regex(MpiPcAdapterProtocol.regMemArea);
                Match addMatch = addParser.Match(Str);
                if (addMatch.Success)
                {
                    //Area address
                    trans = Step7WordTrans.wtW;
                    area = GetS7StaticArea(addMatch.Groups["area"].Value, ref german, ref trans, ref dbnumber);

                    offset = Convert.ToInt32(addMatch.Groups["add"].Value);
                    if (addMatch.Groups["len"].Value.Length > 1)
                        length = Convert.ToInt32(addMatch.Groups["len"].Value.Remove(0, 1));

                    if (addMatch.Groups["tipo"].Value.Length > 0)
                        format = GetS7Type(addMatch.Groups["tipo"].Value, ref dataType);
                    else if (area == Step7Area.aT)
                    {
                        format = Step7Format.frmWord;
                        dataType = DataType.UInt32;
                    }
                    else if (area == Step7Area.aC ||
                              area == Step7Area.aTIEC ||
                              area == Step7Area.aCIEC)
                    {
                        format = Step7Format.frmWord;
                        dataType = DataType.UInt16;
                    }
                    else
                    {
                        format = Step7Format.frmBit;
                    }

                    string sbit = addMatch.Groups["bit"].Value;
                    sbit = sbit.Trim(new char[] { '.' });
                    if (sbit.Length > 0)
                    {
                        bit = Convert.ToInt32(sbit);
                        dataType = DataType.Boolean;
                    }

                    if (addMatch.Groups["conv"].Value.Length > 0)
                    {
                        if (area == Step7Area.aT ||
                            area == Step7Area.aC ||
                            area == Step7Area.aTIEC ||
                            area == Step7Area.aCIEC)
                            Valid = false;
                        if (dataType != DataType.UInt16)
                            Valid = false;
                        else if (addMatch.Groups["conv"].Value == ",T")
                        {
                            dataType = DataType.UInt32;
                            trans = Step7WordTrans.wtT;
                        }
                        else if (addMatch.Groups["conv"].Value == ",C")
                        {
                            trans = Step7WordTrans.wtC;
                        }
                        else
                            Valid = false;
                    }

                }

                if (format == Step7Format.frmInvalid)
                    Valid = false;

                if (area == Step7Area.aInvalid)
                    Valid = false;

                if (format == Step7Format.frmBit ^ dataType == DataType.Boolean)
                    Valid = false;

                if (dataType == DataType.Boolean & bit > 15)
                    Valid = false;

            }
            return Valid;
        }

        public static bool StaticTestAddress(string Str, ref int length, out DataType dataType)
        {
            Step7Area area = Step7Area.aInvalid;
            int dbnumber = -1;
            Step7Format format = Step7Format.frmInvalid;
            Step7WordTrans trans = Step7WordTrans.wtC;
            int offset = 0;
            int bit = 0;
            bool german = false;
            return StaticParseAddress(Str, ref area, ref dbnumber, ref format, ref trans, ref offset, ref bit, ref length, ref german, out dataType);
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
            
            if (propertyName == "StartAddress")
            {
                DataType dataType;
                int lenght = 0;
                if (!StaticTestAddress(StartAddress,ref lenght, out dataType))
                    return Properties.Resources.ErrorInvalidAddress;
                if ((uint)VarType != unchecked((uint)(-1)))
                    return ProtocolDataSizeValidation(VarType);
                if (InvalidStringArray())
                    return UFUAModel.Properties.Resources.ArraysOfStringsAreInvalid;
            }
            if (propertyName == "ArrayDimension")
            {
                if (ParseAddress(StartAddress))
                {
                    if (InvalidStringArray())
                        return UFUAModel.Properties.Resources.ArraysOfStringsAreInvalid;
                }
            }

            return null;
        }

        private bool InvalidStringArray()
        {
            return (VarType == UFUAModel.DataType.String && ArrayDimension != 0);
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
