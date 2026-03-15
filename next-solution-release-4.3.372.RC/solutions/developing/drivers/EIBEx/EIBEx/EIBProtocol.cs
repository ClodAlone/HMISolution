namespace EIB
{
    public class EIBProtocol
    {
        public const int EIB_EXTRA_TIMEOUT = 200;
        public const int TIMEOUT_CHECK_CONNECTION = 5000;
        public const int REPETE_CHECK_CONNECTION = 6;

        public enum COMMPORT : byte
        {
            Com1,
            Com2,
            Com3,
            Com4,
            Com5,
            Com6,
            Com7,
            Com8,
            Com9,
            Com10
        }

        public enum ConnectorTypes : byte
        {
            KnxIpRouting,
            KnxIpTunneling,
            //USB = 1,
            //KnxIpTunneling = 2,
            //KnxIpRouting = 3,
            //KnxIpcEmiTl = 4,
            //Eiblib = 5
        }

        public enum EISDATAFORMAT : byte
        {
            EISDFBit,
            EISDFByte,
            EISDFWord,
            EISDFDWord,
            EISDFFloat,
            EISDFEIS3,
            EISDFEIS4,
            EISDFEIS5,
            EISDFEIS6,
            EISDFAccessPWD6Bytes,
            EISDFAccessPWD10Bytes,
            EISDFInt64
        }

        public const int EIB_WRITE_ERROR = 1000;
        public const int EIB_FALCON_ERROR = 2000;

        public enum EIB_ERROR_CODES : int
        {
            DeviceWriteErrorWriteError = EIB_WRITE_ERROR + 1,
            DeviceWriteErrorInvalidMessage = EIB_WRITE_ERROR + 2,
            DeviceWriteErrorDriverNotOpen = EIB_WRITE_ERROR + 3,
            DeviceWriteErrorQueueOverflow = EIB_WRITE_ERROR + 4,
            DeviceWriteErrorUnexpectedError = EIB_WRITE_ERROR + 255,
            DeviceFalconErrorConfirmationRead = EIB_FALCON_ERROR + 1,
            DeviceFalconErrorConfirmationWrite = EIB_FALCON_ERROR + 2,
            DeviceFalconErrorConnectionBroken = EIB_FALCON_ERROR + 3,
            DeviceFalconErrorTimeOut = EIB_FALCON_ERROR + 4,

            DeviceFalconError_ManageConnectionBroken = EIB_FALCON_ERROR + 9998,
            DeviceFalconError_ManageConnectionRestored = EIB_FALCON_ERROR + 9999,

        }
    }
}
