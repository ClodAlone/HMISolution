using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBaseEx;
using Opc.Ua;
using Accon.AGLink;
using Accon.Symbolik;
using DriverCodeBaseEx.Enumerators;
using DevExpress.Xpo.DB;
using DevExpress.Xpo;
using System.IO;
using System.Reflection;

namespace S7TIASymbolic
{
    /// <summary>
    /// Object used to manage data/job to exchange with PLC using AGLink function
    /// </summary>
    public class AGLinkReadWriteElement
    {
        private const int NO_ELEMENT = -1;

        public int SymbolicRWSIndex { set; get; }
        public S7TIACommJob Job { set; get; }
        public DriverErrorCodes ErrorCode { set; get; }

        public AGLinkReadWriteElement()
        {
            SymbolicRWSIndex = NO_ELEMENT;
            Job = null;
            ErrorCode = DriverErrorCodes.ErrorNoError;
        }

        public bool HasRWSSymbolToExchange() {
            return (SymbolicRWSIndex != NO_ELEMENT);
        }

        public AGLinkReadWriteElement(int symbolicRWSIndex, S7TIACommJob job, DriverErrorCodes errorCode) {
            SymbolicRWSIndex = symbolicRWSIndex;
            Job = job;
            ErrorCode = errorCode;
        }
    }

    public enum S7ErrorCodes : int
    {
        ErrorWrongHeader = 1000,
        ErrorTooFewData,
        ErrorTagDataTypeTooSmal,
        ErrorTooData,
        ErrorConnectionToDevice = 1400,
        ErrorStatusNotZero = 1500,
        ErrorFromDevice = 2000,
        ErrorFromDllAGLink = 3000,
        ErrorAGLinkTiaFileNotPresent = 4000
    }

    public enum S7DataFormats
    {
        Bool = AGL4.SystemType.S7_Bool,
        Byte = AGL4.SystemType.S7_Byte,
        Char = AGL4.SystemType.S7_Char,
        USInt = AGL4.SystemType.S7_USInt,
        SInt = AGL4.SystemType.S7_SInt,
        Int = AGL4.SystemType.S7_Int,
        UInt = AGL4.SystemType.S7_UInt,
        Date = AGL4.SystemType.S7_Date,
        Word = AGL4.SystemType.S7_Word,
        S5Time = AGL4.SystemType.S7_S5Time,
        DInt = AGL4.SystemType.S7_DInt,
        UDInt = AGL4.SystemType.S7_UDInt,
        DWord = AGL4.SystemType.S7_DWord,
        Real = AGL4.SystemType.S7_Real,
        Time = AGL4.SystemType.S7_Time,
        Time_Of_Day = AGL4.SystemType.S7_Time_Of_Day,
        LReal = AGL4.SystemType.S7_LReal,
        String = AGL4.SystemType.S7_String,
        Struct = AGL4.SystemType.S7_Struct,
        LWord = AGL4.SystemType.S7_LWord,
        LInt = AGL4.SystemType.S7_LInt,
        S7_DTL = AGL4.SystemType.S7_DTL,
        ULInt = AGL4.SystemType.S7_ULInt,
        WString = AGL4.SystemType.S7_WString,
    }

    // S7Time format management
    public enum Step7WordTrans {
        wtW,
        wtC,
        wtT
    }

    public class S7TIAProtocol
    {

        public const uint MAX_DATA_BYTES = 0x10000;
        public const uint MAX_AGGREGATED_JOBS = 50;
        public const uint MAX_COMMENT_LENGTH = 256;

        public const string S5_TIME_SEPARATOR_CHAR = ",";
        
        public const uint STRING_DEFAULT_SIZE = 256;
        public const string SYMBOLIC_FILE_EXT = ".tia";

        #region methods override

        public static bool ParseData(byte[] receivebuffer, ref S7TIACommJob job, ref List<object> items)
        {
            List<Tag> changed = new List<Tag>();
            bool areArguments = (items.Count > 0);
            if (!areArguments && (receivebuffer == null))
            {
                return (false);
            }
            else if (receivebuffer == null)
            {
                BuiltInType bt = job.Station.GetBuiltInType(items[0].GetType());
                if (bt != BuiltInType.Byte && bt != BuiltInType.Double &&
                    bt != BuiltInType.Float && bt != BuiltInType.Int16 &&
                    bt != BuiltInType.Int32 && bt != BuiltInType.Int64 &&
                    bt != BuiltInType.Integer && bt != BuiltInType.Number &&
                    bt != BuiltInType.SByte && bt != BuiltInType.UInt16 &&
                    bt != BuiltInType.UInt32 && bt != BuiltInType.UInt64 &&
                    bt != BuiltInType.UInteger)
                {
                    return false;
                }

                items[0] = DriverErrorCodes.ErrorNoError;
                return (true);
            }

            if (areArguments)
            {
                items[0] = DriverErrorCodes.ErrorNoError;
            }

            byte[] tempBuffer = new byte[receivebuffer.Length];

            Array.Copy(receivebuffer, tempBuffer, receivebuffer.Length);

            job.SetJobData(tempBuffer, ref changed);

            if (areArguments)
            {
                if (job.TagsList.Count == items.Count - 1)
                {
                    for (int k = 0; k < job.TagsList.Count; k++)
                    {
                        items[k + 1] = job.TagsList[k].Value.Value;
                    }
                }
            }
            else
            {
                items.AddRange(changed);
            }

            return true;
        }

        public static string GetErrorString(int err)
        {
            String str;
            switch (err)
            {
                case 0x01:
                    str = Properties.Resources.IDS_HWFAULT;
                    break;
                case 0x03:
                    str = Properties.Resources.IDS_ILLEGALOBJACCESS;
                    break;
                case 0x05:
                    str = Properties.Resources.IDS_INVALIDADDRESS;
                    break;
                case 0x06:
                    str = Properties.Resources.IDS_DATATYPENOTSUPP;
                    break;
                case 0x0A:
                    str = Properties.Resources.IDS_OBJNOTEXIST;
                    break;
                default:
                    str = Properties.Resources.IDS_UNKNOWNRESP;
                    break;
            }
            return str;
        }

        public static UFUAModel.DataType DataType(S7DataFormats Format)
        {
            switch (Format)
            {
                case S7DataFormats.Bool:
                    return UFUAModel.DataType.Boolean;
                case S7DataFormats.Byte:
                case S7DataFormats.Char:
                case S7DataFormats.USInt:
                    return UFUAModel.DataType.Byte;
                case S7DataFormats.Date:
                case S7DataFormats.UInt:
                case S7DataFormats.Word:
                    return UFUAModel.DataType.UInt16;
                case S7DataFormats.DInt:
                case S7DataFormats.DWord:
                    return UFUAModel.DataType.Int32;
                case S7DataFormats.Int:
                    return UFUAModel.DataType.Int16;
                case S7DataFormats.ULInt:
                    return UFUAModel.DataType.UInt64;
                case S7DataFormats.LInt:
                    return UFUAModel.DataType.Int64;
                case S7DataFormats.LReal:
                    return UFUAModel.DataType.Double;
                case S7DataFormats.LWord:
                    return UFUAModel.DataType.UInt64;
                case S7DataFormats.Real:
                    return UFUAModel.DataType.Float;
                case S7DataFormats.SInt:
                    return UFUAModel.DataType.SByte;
                case S7DataFormats.String:
                    return UFUAModel.DataType.String;
                case S7DataFormats.WString:
                    return UFUAModel.DataType.String;
                case S7DataFormats.Time:
                case S7DataFormats.Time_Of_Day:
                case S7DataFormats.S5Time:
                case S7DataFormats.UDInt:
                    return UFUAModel.DataType.UInt32;
                default:
                    return 0;
            }
        }

        public static string GetNodeTree(NodeId NodeId, string Name)
        {
            string Out = "";

            //System.Text.RegularExpressions.Regex NameParser = new System.Text.RegularExpressions.Regex(@"^[\d]+:(?<Name>[\w]+)$");
            System.Text.RegularExpressions.Regex NameParser = new System.Text.RegularExpressions.Regex(@"[\d]+:(?<Name>[\w]+)$");
            System.Text.RegularExpressions.Match NameMatch = NameParser.Match(Name);
            if (NameMatch.Success)
            {
                Out = NameMatch.Groups["Name"].Value;
                string nameNodeId = NodeId.Identifier.ToString();
                System.Text.RegularExpressions.Regex NodeParser = new System.Text.RegularExpressions.Regex(@"^[^?]+[?](?<Node>[\w/]+)/[\w-]+$");
                System.Text.RegularExpressions.Match NodeMatch = NodeParser.Match(nameNodeId);
                if (NodeMatch.Success)
                {
                    Out = NodeMatch.Groups["Node"].Value + "." + Out;
                }
            }
            return Out;

        }

        #endregion

        private static UInt16 Bcd2Bin(UInt16 Val)
        {
            int a, b, c;

            a = (Val >> 8) & 0x0f;
            b = (Val >> 4) & 0x0f;
            c = (Val >> 0) & 0x0f;

            if ((a > 9) || (b > 9) || (c > 9))
                return 0;
            return Convert.ToUInt16((a * 100) + (b * 10) + c);
        }

        private static UInt16 Bin2Bcd(int Val)
        {
            UInt16 RetVal = 0;
            int a, b;

            Val %= 1000;

            a = Val / 100;
            Val %= 100;
            b = Val / 10;
            Val %= 10;
            RetVal = (UInt16)(a << 8);
            RetVal |= (UInt16)(b << 4);
            RetVal |= (UInt16)Val;
            return RetVal;
        }

        public static bool ConvertFromS5Time(Step7WordTrans trans, byte[] receivebuffer, ref byte[] tempBuffer)
        {
            int TimeFactor;
            int i = 0, j = 0;
            UInt32 DWordVal = 0;
            int Len = 0;

            Len = 1;

            // target buffer size too small
            if (receivebuffer.Length > tempBuffer.Length)
                return false;

            do
            {
                UInt16 WordVal = (UInt16)((receivebuffer[j++] << 8) | receivebuffer[j++]);
                switch (trans)
                {
                    case Step7WordTrans.wtW:
                        tempBuffer[i++] = (byte)(WordVal >> 0);
                        tempBuffer[i++] = (byte)(WordVal >> 8);
                        break;
                    case Step7WordTrans.wtC:
                        WordVal = Bcd2Bin((UInt16)(WordVal & 0xfff));
                        tempBuffer[i++] = (byte)(WordVal >> 0);
                        tempBuffer[i++] = (byte)(WordVal >> 8);
                        break;
                    case Step7WordTrans.wtT:
                        TimeFactor = (WordVal >> 12) & 3;
                        WordVal = Bcd2Bin((UInt16)(WordVal & 0xfff));
                        switch (TimeFactor)
                        {
                            case 0:
                                DWordVal = (UInt32)(WordVal * 10);
                                break;
                            case 1:
                                DWordVal = (UInt32)(WordVal * 100);
                                break;
                            case 2:
                                DWordVal = (UInt32)(WordVal * 1000);
                                break;
                            case 3:
                                DWordVal = (UInt32)(WordVal * 10000);
                                break;
                        }
                        tempBuffer[i++] = (byte)(DWordVal >> 0);
                        tempBuffer[i++] = (byte)(DWordVal >> 8);
                        tempBuffer[i++] = (byte)(DWordVal >> 16);
                        tempBuffer[i++] = (byte)(DWordVal >> 24);
                        break;
                }
            } while (--Len > 0);

            return true;
        }


        public static bool ConvertToS5Time(Step7WordTrans trans, byte[] jobdata, ref byte[] tempBuffer)
        {            
            UInt16 WordVal;
            UInt32 DWordVal;
            UInt32 DWtemp;
            UInt16 TimeFactor;
            
            uint nLength = 2;
            uint idx = 0;
            uint Len = nLength / 2;

            byte[] pDst = new byte[jobdata.Length];
                
            Array.Copy(jobdata, 0, pDst, 0 ,jobdata.Length);

            do
            {
                switch (trans)
                {
                    case Step7WordTrans.wtW:
                        S7TIACommJob.SwapByteBuffer(ref pDst, (int)idx/*4*/, (int)nLength);
                        Len = 1;
                        break;
                    case Step7WordTrans.wtC:
                        WordVal = pDst[idx + 1];
                        WordVal <<= 8;
                        WordVal |= pDst[idx + 0];
                        if (WordVal > 999)
                            WordVal = 999;
                        WordVal = Bin2Bcd(WordVal);
                        pDst[idx + 1] = (byte)(WordVal >> 0);
                        pDst[idx + 0] = (byte)(WordVal >> 8);
                        idx += 2;
                        break;
                    case Step7WordTrans.wtT:
                        DWordVal = (UInt32)(pDst[idx + 3] << 24);
                        DWtemp = (UInt32)(pDst[idx + 2] << 16);
                        DWordVal |= DWtemp;
                        DWtemp = (UInt32)(pDst[idx + 1] << 8);
                        DWordVal |= DWtemp;
                        DWordVal |= pDst[idx + 0];

                        if (DWordVal < 10000)
                        {
                            TimeFactor = 0;
                            WordVal = (UInt16)(DWordVal / 10);
                        }
                        else
                        {
                            if (DWordVal < 100000)
                            {
                                TimeFactor = 1;
                                WordVal = (UInt16)(DWordVal / 100);
                            }
                            else
                            {
                                if (DWordVal < 1000000)
                                {
                                    TimeFactor = 2;
                                    WordVal = (UInt16)(DWordVal / 1000);
                                }
                                else
                                {
                                    TimeFactor = 3;
                                    WordVal = (UInt16)(DWordVal / 10000);
                                    if (WordVal > 999)
                                        WordVal = 999;
                                }
                            }
                        }
                        UInt16 u = Bin2Bcd((int)WordVal);
                        WordVal = TimeFactor;
                        WordVal <<= 12;
                        WordVal |= u;

                        pDst[idx + 1] = (byte)(WordVal >> 0);
                        pDst[idx + 0] = (byte)(WordVal >> 8);
                        idx += 4;
                        break;
                }

            } while (--Len > 0);        
            
            ////after the checks here the "jobdata" buffer arrives with the data size as UIN32, 
            ////if it is a data type S5Time, the buffer must be compacted to Word
            //if (trans == Step7WordTrans.wtT)
            //{
            //byte[] tmpjobdata = new byte[nLength];
            for (uint index = 0, indexSor = 0; index < nLength; index += 2, indexSor += 4)
            {
                tempBuffer[index] = pDst[indexSor];
                tempBuffer[index + 1] = pDst[indexSor + 1];
            }
               
            //tempBuffer = tmpjobdata;
            //}

            return true;
        }
        #region Symbolic File Management Common methods
        public static string GetFileImportName(string projectFile)
        {
            if (!String.IsNullOrEmpty(projectFile))
                return Path.GetFileName(projectFile);
            else
                return projectFile;
        }

        public static bool IsImportedFile(string projectFile, string importedPath = null)
        {
            if (!String.IsNullOrEmpty(projectFile))
            {
                string path = Path.GetDirectoryName(projectFile);
                if (String.IsNullOrEmpty(importedPath))
                    return (string.IsNullOrEmpty(path));
                else
                    return (path.ToLower() == importedPath.ToLower());
            }
            else
            {
                return false;
            }
        }

        public static string GetOldFileName(string stationName)
        {
            return string.Format("{0}_{1}_Plc{2}", stationName, S7TIAProtocol.GetDriverName(), S7TIAProtocol.SYMBOLIC_FILE_EXT);
        }

        public static string GetDriverName()
        {
            return Assembly.GetExecutingAssembly().GetName().Name.Replace(".UI", "");
        }

        public static string GetDriverPath(string targerConn)
        {
            string path = null;
            string connect = CommunicationDriver.GetConnectionString(targerConn, "Drivers", GetDriverName(), null);
            using (IDataLayer dl = CommunicationDriver.GetSpecificDataLayer(connect, out string filebase, out InMemoryDataStore InMemory, out bool targetIsFile))
            {
                if (targetIsFile)
                    path = Path.GetDirectoryName(filebase);
            }

            return path;
        }
        #endregion
    }
}
