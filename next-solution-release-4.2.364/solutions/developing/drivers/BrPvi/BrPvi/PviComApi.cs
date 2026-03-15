using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BrPvi
{
    public unsafe delegate void PviCallbackWithData64Bit(ulong wParam, IntPtr lParam, [In] void* pData, uint dataLen, [In] PviComStruct.T_RESPONSE_INFO* pInfo);
    public unsafe delegate void PviCallbackWithData32Bit(uint wParam, IntPtr lParam, [In] void* pData, uint dataLen, [In] PviComStruct.T_RESPONSE_INFO* pInfo);

    class PviComApi
    {
        #region API Calls
        [DllImport("PviCom64.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PviXInitialize")]
        public static extern unsafe int _PviXInitialize64Bit(ref uint phPVI, int Timeout, int RetryTime, [MarshalAs(UnmanagedType.LPStr)] string pInitParam, void* pRes);
        [DllImport("PviCom.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PviXInitialize")]
        public static extern unsafe int _PviXInitialize32Bit(ref uint phPVI, int Timeout, int RetryTime, [MarshalAs(UnmanagedType.LPStr)] string pInitParam, void* pRes);
        [DllImport("PviCom64.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PviXDeinitialize")]
        public static extern unsafe int _PviXDeinitialize64Bit(uint hPVI);
        [DllImport("PviCom.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PviXDeinitialize")]
        public static extern unsafe int _PviXDeinitialize32Bit(uint hPVI);
        [DllImport("PviCom64.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PviXSetGlobEventMsg")]
        public static extern unsafe int _PviXSetGlobEventMsg64Bit(uint hPVI, uint nGlobEvent, void* hEventMsg, uint EventMsgNo, IntPtr EventParam);
        [DllImport("PviCom.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PviXSetGlobEventMsg")]
        public static extern unsafe int _PviXSetGlobEventMsg32Bit(uint hPVI, uint nGlobEvent, void* hEventMsg, uint EventMsgNo, IntPtr EventParam);
        [DllImport("PviCom64.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PviXCreateRequest")]
        public static extern unsafe int _PviXCreateRequest64Bit(uint hPVI, string pObjectName, uint ObjectTyp, string pObjectDescriptor, void* hEventMsg, uint EventMsgNo, IntPtr EventParam, string pLinkDescriptor, void* hResMsg, uint ResMsgNo, IntPtr ResParam);
        [DllImport("PviCom.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PviXCreateRequest")]
        public static extern unsafe int _PviXCreateRequest32Bit(uint hPVI, string pObjectName, uint ObjectTyp, string pObjectDescriptor, void* hEventMsg, uint EventMsgNo, IntPtr EventParam, string pLinkDescriptor, void* hResMsg, uint ResMsgNo, IntPtr ResParam);
        [DllImport("PviCom64.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PviXWriteRequest")]
        public static extern unsafe int _PviXWriteRequest64Bit(uint hPVI, uint LinkID, uint nAccess, void* pData, uint DataLen, void* hResMsg, uint ResMsgNo, IntPtr ResParam);
        [DllImport("PviCom.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PviXWriteRequest")]
        public static extern unsafe int _PviXWriteRequest32Bit(uint hPVI, uint LinkID, uint nAccess, void* pData, uint DataLen, void* hResMsg, uint ResMsgNo, IntPtr ResParam);
        [DllImport("PviCom64.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PviXReadRequest")]
        public static extern unsafe int _PviXReadRequest64Bit(uint hPVI, uint LinkID, uint nAccess, void* hResMsg, uint ResMsgNo, IntPtr ResParam);
        [DllImport("PviCom.dll", CallingConvention = CallingConvention.Winapi, EntryPoint = "PviXReadRequest")]
        public static extern unsafe int _PviXReadRequest32Bit(uint hPVI, uint LinkID, uint nAccess, void* hResMsg, uint ResMsgNo, IntPtr ResParam);
        #endregion
    }
}
