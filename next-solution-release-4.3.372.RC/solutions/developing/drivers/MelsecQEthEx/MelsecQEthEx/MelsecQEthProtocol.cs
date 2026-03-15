using System;
using DriverCodeBaseEx;
using System.Collections.Generic;
using Opc.Ua;

namespace MelsecQEth
{    
    public class MelsecQEthProtocol
    {
        public enum MelsecQErrorCodes : int
        {
            ErrorCodeUnknownSubHeader = 1000,
            ErrorCodeMismatchSubheader = 1001,
            ErrorCodeInvalidCommand = 1002,
            ErrorCodeIncorrectDeviceDesignation = 1003,
            ErrorCodeExceedingAddOrNumOfPoints = 1004,
            ErrorCodeWrongHeadDeviceNum = 1005,
            ErrorCodePCNumberError = 1006,
            ErrorCodeModeError = 1007,
            ErrorCodeRemoteError = 1008,
            ErrorCodeAbnormalCode = 1009,
            ErrorCodeMonitoringTimerExceeded = 1010,
            ErrorCodeGenericError = 1011,
            ErrorCodeIncompleteFrame = 1012,
            ErrorWrongReply = 1013,
            ErrorFromDevice = 2000,
            ErrorOnlineChangeDisabled = ErrorFromDevice + 0x55,

            ErrorLabelDoesNotExist = 0x40C0,
            ErrorLabelsDoesNotExist = 0x4030,
            ErrorLabelSizeMismatch = 0x40CB,
            ErrorLabelArraySizeMismatch = 0x40C1,
            ErrorLabelTooManyRequest = 0x40C4,
            ErrorLabelDataCannotBeAccessWithLabel = 0x40CE
        }

        public class ReadWriteListLimitateSize
        {
            public uint ProtocolSize;
            public uint TotalRequestSize;
            public uint TotalResponseSize;
            public uint TotalNrJobs;
            public uint MaxNrJobs;

            public ReadWriteListLimitateSize()
            {
                TotalRequestSize = 0;
                TotalResponseSize = 0;
                ProtocolSize = 0;
                TotalNrJobs = 0;
                MaxNrJobs = uint.MaxValue;
            }

            public bool IsEmpty()
            {
                return (ProtocolSize == 0);
            }
        }
        
        public const string TEST_COMM_DYNAMIC = "MelsecQEth.Station={0}|LinkType=1|Addr=D0|Cpu=0";

        public const int STRING_TERMINATOR_NULL_CHARS = 2;

        public const int MAXBYTE_SIZE_QnA = 960;
        public const int MAXBYTE_SIZE_R_L_Q = 1920;
        public const int MAXBYTE_SIZE = 1920;
        public enum PlcTypes
        { CPU_MODEL_QnA, CPU_MODEL_iQR, CPU_MODEL_L_Q }

        public enum AddressTypes : byte
        { 
            DataArea = 0,
            Label
        }

        public enum DataTypeID : byte
        {
            Bit_Timer_Counter_LongTimer_LongCounter = 1,
            Word_RetentiveTimer = 2,
            DoubleWord_LongRetentiveTimer = 3,
            Word = 4,
            DoubleWord = 5,
            Fload = 6,
            Double = 7,
            Time = 8,
            String = 9,
            WString = 10
        }

        public enum StringSettingsAutoDetectStates
        {
            NotRequested = 0,
            Requested,
            RequestedDone
        }

        public enum RuntimeAggregationLimits
        {
            Read,
            Write,
            NotExchanged
        }

        public static uint GetMaxJobSize(MelsecQEthProtocol.PlcTypes plcType)
        {
            switch (plcType)
            {
                case MelsecQEthProtocol.PlcTypes.CPU_MODEL_L_Q:
                    return MelsecQEthProtocol.MAXBYTE_SIZE_R_L_Q;

                case MelsecQEthProtocol.PlcTypes.CPU_MODEL_iQR:
                    return MelsecQEthProtocol.MAXBYTE_SIZE_R_L_Q;

                case MelsecQEthProtocol.PlcTypes.CPU_MODEL_QnA:
                    return MelsecQEthProtocol.MAXBYTE_SIZE_QnA;
            }

            return (uint)MelsecQEthProtocol.MAXBYTE_SIZE;
        }

        public static bool PlcSupportLabelAddress(PlcTypes plc)
        {
            return (plc == PlcTypes.CPU_MODEL_iQR);
        }

        public static bool IsLabelAddress(List<CommJob> list)
        {
            return (list.Count > 0 && ((MelsecQEthCommJob)list[0]).AddressType == AddressTypes.Label);
        }

        public static bool IsLabelAddress(CommJob job)
        {
            return IsLabelAddress(job.TagsList[0]);
        }

        public static bool IsLabelAddress(Tag tag)
        {
            return (((MelsecQEthDynTagSettings)tag.DynSettings).AddressType == AddressTypes.Label);
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

        public static byte[] GetModuleTargetBuffer(CpuTargets cpuTarget)
        {
            byte[] buffer = new byte[3];

            switch (cpuTarget)
            {
                case CpuTargets.Plc1:
                    buffer[0] = 0xE0; // Dest. mod. I/O N. (l. byte).
                    break;
                case CpuTargets.Plc2:
                    buffer[0] = 0xE1; // Dest. mod. I/O N. (l. byte).
                    break;
                case CpuTargets.Plc3:
                    buffer[0] = 0xE2; // Dest. mod. I/O N. (l. byte).
                    break;
                case CpuTargets.Plc4:
                    buffer[0] = 0xE3; // Dest. mod. I/O N. (l. byte).
                    break;

                default:
                case CpuTargets.ControlPLC:
                    buffer[0] = 0xFF; // Dest. mod. I/O N. (l. byte).
                    break;

            }
            buffer[1] = 0x03; // Dest. mod. I/O N. (h. byte).
            buffer[2] = 0; // Dest. module station number.

            return buffer;
        }


        public static uint GetStringJobSize(MelsecQEthCommJob job)
        {
            return GetStringJobSize(job.AddressType, job.UnicodeString, job.StringLength);
        }
        /// <summary>
        /// Get string job size (in byte)
        /// </summary>
        /// <param name="unicode"></param>
        /// <param name="stringLength"></param>
        /// <returns></returns>
        public static uint GetStringJobSize(AddressTypes addressType, bool unicode, uint stringLength)
        {
            if (addressType == AddressTypes.DataArea)
            {
                if (unicode)
                    return stringLength * 2;
                else
                    return stringLength;
            }
            else
            {
                if (unicode)
                    return stringLength * 2 + STRING_TERMINATOR_NULL_CHARS;                    
                else
                    return stringLength + STRING_TERMINATOR_NULL_CHARS;
            }
        }

        public static byte GetArrayLabelUnitSpecification(Tag tag)
        {
            return ((uint)tag.TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.Boolean ? (byte)0 : (byte)1);
        }

        public static bool IsStringJob(CommJob job)
        {
            return (job.TagsList[0].TagNode.DataType.IdType == Opc.Ua.IdType.Numeric && (uint)job.TagsList[0].TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.String);
        }

        public static bool IsStringJob(Tag defTag)
        {
            return (defTag.TagNode.DataType.IdType == Opc.Ua.IdType.Numeric && (uint)defTag.TagNode.DataType.Identifier == (uint)Opc.Ua.DataTypes.String);
        }
    }
}
