using System;

namespace BrPvi
{
    public class BrPviProcol
    {
        public enum InterfaceTypes
        {
            Ethernet,
            Serial
        }

        public enum FlowControlTypes
        {
            None,
            RtsOff,
            RS232Mode,
            RS422Mode
        }

        public enum ParityTypes
        {
            None,
            Odd,
            Even,
            Mark,
            Space
        }

        public enum BrPviErrorCodes : int
        {
            ErrorCodeCreateFailure = 500,
            ErrorCodeEventError = 501,
            ErrorCodePrepareWriteRequest = 502,
            ErrorCodeConnectionBroken = 503,
            ErrorCodeIdentificationError = 504,

            ErrorCreatePviObjectsFailed = 99999
        }

        public enum BrPviVersions : byte
        {
            Version2x,
            Version3x,
            Version4x
        }

        public enum BrPviConnectionStatus
        {
            NotInitialized,
            Disconnected,
            Connected,
            Arranged
        }

        public enum DeviceStateCheckMode
        {
            None,
            Sync,
            Async,
        }


        public const uint POBJ_EVENT_ERROR = 3;

        public const uint PVI_EVENT_INVALID_OBJECT_INFORMATION_ERROR = 4806;
        public const uint PVI_EVENT_IDENTIFICATION_ERROR = 4813;
        public const uint PVI_EVENT_NO_CONNECTION_AVAILABLE_TO_THE_PLC_ERROR = 4808;
        public const uint PVI_EVENT_SYSTEM_ERROR_MIN_RANGE = 12000;
        public const uint PVI_EVENT_SYSTEM_ERROR_MAX_RANGE = 12499;

        public const string TEST_COMM_DYNAMIC_SETTINGS = "BrPvi.Station={0}|LinkType=1||PviVar=Dummy|PviTask=|RR=500|AL=0|STRTY=0";

        /// <summary>
        /// Test if DLL's was installed into PC calling a function
        /// </summary>
        /// <returns></returns>
        public static bool IsPviMonitorInstalled()
        {
            bool result = true;
            unsafe
            {
                try
                {                    
                    if (System.Environment.Is64BitProcess)
                    {
                        uint linkID = 0;
                        int returnValue = PviComApi._PviXInitialize64Bit(ref linkID, 5000, 1, null, null);
                        if (returnValue == 0)
                            returnValue = PviComApi._PviXDeinitialize64Bit(linkID);
                    }
                    else
                    {
                        uint linkID = 0;
                        int returnValue = PviComApi._PviXInitialize32Bit(ref linkID, 5000, 1, null, null);
                        if (returnValue == 0)
                            returnValue = PviComApi._PviXDeinitialize32Bit(linkID);
                    }
                } catch (Exception ex)
                {
                    result = false;
                }
            }
            return result;
        }
    }
}

