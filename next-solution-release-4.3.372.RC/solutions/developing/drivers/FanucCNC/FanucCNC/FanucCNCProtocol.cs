using System;
using System.Collections.Generic;
using Opc.Ua;
using DriverCodeBase;
using DriverCodeBase.Enumerators;
using FanucCNC.Focas_Library;
using System.Reflection;
using System.Runtime.InteropServices;
using System.IO;

namespace FanucCNC
{
    public class FanucCNCProtocol
    {
        #region Api import
        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetDllDirectory(string lpPathName);
        #endregion

        public const uint MAX_DATA_BYTES = 50000; //256; //512
        public const int DEFAULT_PORT = 8193;
        public const int CNC_PROGRAM_CHAR_START_END = 37; //"%"
        public const int CNC_PROGRAM_END_OF_LINE = 10; //"%"

        /// <summary>
        /// Conditional variable used bit
        /// </summary>
        public enum ConditionalVariableState : int
        {
            ACTIVATION_REQUEST = 0,
            ERROR_DURING_WRITE = 1,
        }

        // define a string size assigned automatically by driver when define specific parameter
        public const int MAX_STRING_SIZE_PARAMETER = 250;

        public enum MachineSeries : int
        {
            Serie0iB = 0,
            Serie30iB = 1,
            Serie31iB = 2,
            Serie35iB = 3,            
        }

        public enum FunctionCategory : int
        {
            Pmc = 0,
            Cnc = 1,
            FileManagement = 2,
        }
        public enum FunctionCode : int
        {
            //Menu_Func_Separator = -1,
            Unknown = 0,
            Func_cnc_actf = 1,
            Func_cnc_absolute = 2,
            Func_cnc_rdaxisdata = 3,
            Func_cnc_rdtofsr_cnc_wrtofsr = 4,   // impostare il tipo di stazione da dispositivo
            Func_cnc_rdparam_cnc_wrparam = 5,
            Func_cnc_statinfo = 6,
            Func_pmc_rdpmcrng_pmc_wrpmcrng = 7,
            Func_cnc_rdsvmeter = 8,

            Func_cnc_rdzofs_cnc_wrzofs = 9,
            Func_cnc_rdspeed = 10,
            Func_cnc_rdalmmsg2 = 11,
            Func_cnc_rdopmsg3 = 12,
            Func_cnc_pdf_rdactpt = 13,            

            //Func_cnc_rdpdf_alldir = 14,
            //Func_cnc_pdf_add = 15,
            Func_cnc_pdf_slctmain = 16,
            Func_cnc_pdf_del = 17,

            Func_cnc_getpath_cnc_setpath = 18,

            //Func_cnc_dwnstart4,
            //Func_cnc_download4,
            //Func_cnc_dwnend4,
            //Func_cnc_upstart4,
            //Func_cnc_upload4,
            //Func_cnc_upend4,

            Func_Custom_UploadProgramFromCnc = 19,
            Func_Custom_DownloadProgramToCnc = 20,

            Func_cnc_pdf_rdmain = 21,
        }
        
        public enum ErrorCodes : int
        {
            ErrorGenericCommunicationError = 1001,
            ErrorConnectionBroken,

            #region Files Management Error
            ErrorFileNameInvalid,
            ErrorFileNotAvailable,
            ErrorFileBodyEmpty,
            ErrorReadingActiveProgramFromCNC,
            ErrorActiveProgramOnCNCEmpty,
            ErrorDuringDownloadingProgram,
            ErrorDuringProgramActivation,

            ErrorDuringUploadingProgramFromCNC,
            ErrorDuringSavingDowloadedProgramFromCNC,
            ErrorConvertingInOutData,

            ErrorFunctionNotSupported,
            ErrorBadWaitingForInitialData,
            ErrorChangingCNCPath,
            ErrorInvalidCNCPath,
            #endregion

            //ErrorGenericPlciException = 1001,
            //ErrorFatalPlciException,
            //ErrorRecoverablePlciException,
            //ErrorUnMappedTag,

            //ErrorInvalidDataFormat,
            //ErrorInvalidArraySize,
            //ErrorGetFromPlcBadBinFilePath,
            //ErrorGetFromPlcCannotWriteBinFile
        }

        public static UFUAModel.DataType GetDataType(BuiltInType type)
        {
            switch (type)
            {
                case BuiltInType.Boolean:
                    return UFUAModel.DataType.Boolean;
                case BuiltInType.Byte:
                    return UFUAModel.DataType.Byte;
                case BuiltInType.SByte:
                    return UFUAModel.DataType.SByte;
                case BuiltInType.Int16:
                    return UFUAModel.DataType.Int16;
                case BuiltInType.UInt16:
                    return UFUAModel.DataType.UInt16;
                case BuiltInType.Int32:
                    return UFUAModel.DataType.Int32;
                case BuiltInType.UInt32:
                    return UFUAModel.DataType.UInt32;
                case BuiltInType.Int64:
                    return UFUAModel.DataType.Int64;
                case BuiltInType.UInt64:
                    return UFUAModel.DataType.UInt64;
                case BuiltInType.Float:
                    return UFUAModel.DataType.Float;
                case BuiltInType.Double:
                    return UFUAModel.DataType.Double;
                case BuiltInType.String:
                    return UFUAModel.DataType.String;
                default:
                    return UFUAModel.DataType.Boolean;
            }
        }

        public static BuiltInType GetDataType(UFUAModel.DataType type)
        {
            switch (type)
            {
                case UFUAModel.DataType.Boolean:
                    return BuiltInType.Boolean;
                case UFUAModel.DataType.Byte:
                    return BuiltInType.Byte;
                case UFUAModel.DataType.SByte:
                    return BuiltInType.SByte;
                case UFUAModel.DataType.Int16:
                    return BuiltInType.Int16;
                case UFUAModel.DataType.UInt16:
                    return BuiltInType.UInt16;
                case UFUAModel.DataType.Int32:
                    return BuiltInType.Int32;
                case UFUAModel.DataType.UInt32:
                    return BuiltInType.UInt32;
                case UFUAModel.DataType.Int64:
                    return BuiltInType.Int64;
                case UFUAModel.DataType.UInt64:
                    return BuiltInType.UInt64;
                case UFUAModel.DataType.String:
                    return BuiltInType.String;
                default:
                    return BuiltInType.Null;
            }
        }

        public static int GetDataTypeSize(UFUAModel.DataType type)
        {
            switch (type)
            {
                case UFUAModel.DataType.Boolean:
                    return 1;
                case UFUAModel.DataType.Byte:                    
                case UFUAModel.DataType.SByte:
                    return 1;
                case UFUAModel.DataType.Int16:                    
                case UFUAModel.DataType.UInt16:
                    return 2;
                case UFUAModel.DataType.Int32:                    
                case UFUAModel.DataType.UInt32:
                case UFUAModel.DataType.Float:
                    return 4;
                case UFUAModel.DataType.Int64:                    
                case UFUAModel.DataType.UInt64:
                case UFUAModel.DataType.Double:
                    return 8;
                case UFUAModel.DataType.String:
                    return 0;
                default:
                    return 0;
            }
        }

        public static bool ParseData(byte[] receivebuffer, ref FanucCNCCommJob job, ref List<object> items)
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

        /// <summary>
        /// Add new search path for DLLImport function: if search path is null, add DLL's driver path
        /// </summary>
        /// <param name="searchPath"></param>
        public static void AddDllFocasLibrarySearchPath(string searchPath = null)
        {
            if (string.IsNullOrEmpty(searchPath)) {
                // get DLL's driver (assembly) path
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                searchPath = Path.GetDirectoryName(Uri.UnescapeDataString(uri.Path));
            }

            try
            {
                SetDllDirectory(searchPath);
            } catch (Exception ex) { }
        }
        
        /// <summary>
        /// Using a simple library function test if Focas library were properly installad
        /// </summary>
        /// <returns></returns>
        public static bool CheckFocasLibrary()
        {
            bool result = false;

            try {                

                int res_handle = Focas1.cnc_allclibhndl3("127.0.0.1", (ushort)DEFAULT_PORT, 1, out ushort focasHandle);
                if (res_handle != (int)Focas1.focas_ret.EW_NODLL)
                    result = true;

                if (focasHandle != 0)
                    Focas1.cnc_freelibhndl(focasHandle);

            } catch { }

            return result;
        }

        public static bool IsUploadDownloadFunctionCode(FunctionCode fc)
        {
            return (fc == FunctionCode.Func_Custom_UploadProgramFromCnc || fc == FunctionCode.Func_Custom_DownloadProgramToCnc || fc == FunctionCode.Func_cnc_pdf_slctmain || fc == FunctionCode.Func_cnc_pdf_del);
        }
    }
}
