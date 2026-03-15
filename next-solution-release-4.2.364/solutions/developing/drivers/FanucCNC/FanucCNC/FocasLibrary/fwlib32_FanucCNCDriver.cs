/*-------------------------------------------------------------------*/
/* FWLIB32.cs library customizazione                                 */
/*-------------------------------------------------------------------*/

using System.Runtime.InteropServices;

namespace FanucCNC.Focas_Library
{
    public partial class Focas1
    {
        public enum DownloadData : short
        {
            NCProgram = 0,
            ToolOffsetData = 1,
            Parameter = 2,
            PitchErrorCompensationData = 3,
            CustomMacroVariables = 4,
            WorkZeroOffsetData = 5,
            OperationHistoryData = 7,
            RotaryTableDynamicFixtureOffset = 18
        }

        public const int IODBPMCXX_CDATA_SIZE = 4000;

        /* pmc_rdpmcrng:read PMC data(area specified) */
        /* pmc_wrpmcrng:write PMC data(area specified) */
        [StructLayout(LayoutKind.Explicit)]
        public class IODBPMCXX
        {
            [FieldOffset(0)]
            public short type_a;    /* PMC address type */
            [FieldOffset(2)]
            public short type_d;    /* PMC data type */
            [FieldOffset(4)]
            public short datano_s;  /* start PMC address */
            [FieldOffset(6)]
            public short datano_e;  /* end PMC address */
            [FieldOffset(8),
            MarshalAs(UnmanagedType.ByValArray, SizeConst = IODBPMCXX_CDATA_SIZE)]
            public byte[] cdata;       /* PMC data */
        } /* In case that the number of data is 5 */

        [DllImport("FWLIB32.dll", EntryPoint = "pmc_rdpmcrng")]
        public static extern short pmc_rdpmcrng(ushort FlibHndl,
            short a, short b, ushort c, ushort d, ushort e, [Out, MarshalAs(UnmanagedType.LPStruct)] IODBPMCXX f);


        /* write PMC data(area specified) */
        [DllImport("FWLIB32.dll", EntryPoint = "pmc_wrpmcrng")]
        public static extern short pmc_wrpmcrng(ushort FlibHndl, ushort a, [In, MarshalAs(UnmanagedType.LPStruct)] IODBPMCXX b);

        public static string GetFocasErrorDescription(short errCode)
        {
            return GetFocasErrorDescription((focas_ret)errCode);
        }

        public static string GetFocasErrorDescription(focas_ret errCode)
        {
            string result = string.Empty;

            switch (errCode)
            {
                case focas_ret.EW_PROTOCOL:
                    result = "protocol error";
                    break;
                case focas_ret.EW_SOCKET:
                    result = "windows socket error";
                    break;
                case focas_ret.EW_NODLL:
                    result = "dll not exist error";
                    break;
                case focas_ret.EW_BUS:
                    result = "bus error";
                    break;
                case focas_ret.EW_SYSTEM2:
                    result = "system error";
                    break;
                case focas_ret.EW_HSSB:
                    result = "Hssb communication error";
                    break;
                case focas_ret.EW_HANDLE:
                    result = "Windows library handle error";
                    break;
                case focas_ret.EW_VERSION:
                    result = "CNC/PMC version missmatch";
                    break;
                case focas_ret.EW_UNEXP:
                    result = "abnormal error";
                    break;
                case focas_ret.EW_SYSTEM:
                    result = "system error";
                    break;
                case focas_ret.EW_PARITY:
                    result = "shared RAM parity error";
                    break;
                case focas_ret.EW_MMCSYS:
                    result = "emm386 or mmcsys install error";
                    break;
                case focas_ret.EW_RESET:
                    result = "reset or stop occured error";
                    break;
                case focas_ret.EW_BUSY:
                    result = "busy error";
                    break;
                case focas_ret.EW_OK:
                    result = "no problem";
                    break;
                case focas_ret.EW_FUNC:
                //case focas_ret.EW_NOPMC:
                    result = "command prepare error or pmc not exist";
                    break;
                case focas_ret.EW_LENGTH:
                    result = "data block length error";
                    break;
                case focas_ret.EW_NUMBER:
                //case focas_ret.EW_RANGE:
                    result = "data number error or address range error";
                    break;
                case focas_ret.EW_ATTRIB:
                //case focas_ret.EW_TYPE:
                    result = "data attribute error or data type error";
                    break;
                case focas_ret.EW_DATA:
                    result = "data error";
                    break;
                case focas_ret.EW_NOOPT:
                    result = "no option error";
                    break;
                case focas_ret.EW_PROT:
                    result = "write protect error";
                    break;
                case focas_ret.EW_OVRFLOW:
                    result = "memory overflow error";
                    break;
                case focas_ret.EW_PARAM:
                    result = "cnc parameter not correct error";
                    break;
                case focas_ret.EW_BUFFER:
                    result = "buffer error";
                    break;
                case focas_ret.EW_PATH:
                    result = "path error";
                    break;
                case focas_ret.EW_MODE:
                    result = "cnc mode error";
                    break;
                case focas_ret.EW_REJECT:
                    result = "execution rejected error";
                    break;
                case focas_ret.EW_DTSRVR:
                    result = "data server error";
                    break;
                case focas_ret.EW_ALARM:
                    result = "alarm has been occurred";
                    break;
                case focas_ret.EW_STOP:
                    result = "CNC is not running";
                    break;
                case focas_ret.EW_PASSWD:
                    result = "Protection data error";
                    break;
                //case focas_ret.DNC_NORMAL:
                //    result = "Normal completed";
                //    break;
                case focas_ret.DNC_CANCEL:
                    result = "DNC operation was canceled by CNC";
                    break;
                case focas_ret.DNC_OPENERR:
                    result = "File open error";
                    break;
                case focas_ret.DNC_NOFILE:
                    result = "File not found";
                    break;
                case focas_ret.DNC_READERR:
                    result = "Read error";
                    break;
            }
            return result;
        }    

    } // End for Focas1 class
}