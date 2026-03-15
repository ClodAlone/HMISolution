using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BrPvi
{
    public class PviComManager
    {
        #region Constructors

        public PviComManager()
        {
            _ErrorDescription = "";
        }
        #endregion

        #region Data Members
        // Handle for the communication instance
        uint linkID = 0;
        // Global event types
        public const uint POBJ_EVENT_PVI_CONNECT = 240;
        public const uint POBJ_EVENT_PVI_DISCONN = 241;
        public const uint POBJ_EVENT_PVI_ARRANGE = 242;

        // Access types
        public const uint POBJ_ACC_EVMASK = 5;
        public const uint POBJ_ACC_DATA = 11;
        public const uint POBJ_ACC_TYPE_EXTERN = 14;
        public const uint POBJ_ACC_LIST_TASK = 35;
        public const uint POBJ_ACC_LIST_PVAR = 36;

        // Special user message for using callback functions (with data)
        const uint SET_PVICALLBACK_DATA = 0xfffffffe;
        #endregion

        #region Methods
        public bool IsInitialized()
        {            
            return (linkID != 0);
        }

        public int PviXInitialize(int Timeout, int RetryTime, string ipAddress, int portNumber)
        {
            int returnValue = 0;
            linkID = 0;
            unsafe
            {
                if (System.Environment.Is64BitProcess)
                {
                    if (String.IsNullOrWhiteSpace(ipAddress))
                    {
                        returnValue = PviComApi._PviXInitialize64Bit(ref linkID, Timeout, RetryTime, null, null);
                        System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXInitialize {0} - Result of PviXInitialize {1} for local server - Comm. Instance: {2}",
                                                            DateTime.Now.ToString("HH:mm:ss.fff"), returnValue, linkID);
                    }
                    else
                    {
                        string pInitParam = String.Format("IP={0} PN={1}", ipAddress, portNumber);
                        returnValue = PviComApi._PviXInitialize64Bit(ref linkID, Timeout, RetryTime, pInitParam, null);
                        System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXInitialize {0} - Result of PviXInitialize {1} for remote server {2} - Comm. Instance: {3}",
                                                            DateTime.Now.ToString("HH:mm:ss.fff"), returnValue, pInitParam, linkID);
                    }
                }
                else
                {
                    if (String.IsNullOrWhiteSpace(ipAddress))
                    {
                        returnValue = PviComApi._PviXInitialize32Bit(ref linkID, Timeout, RetryTime, null, null);
                        System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXInitialize {0} - Result of PviXInitialize {1} for local server - Comm. Instance: {2}",
                                                            DateTime.Now.ToString("HH:mm:ss.fff"), returnValue, linkID);
                    }
                    else
                    {
                        string pInitParam = String.Format("IP={0} PN={1}", ipAddress, portNumber);
                        returnValue = PviComApi._PviXInitialize32Bit(ref linkID, Timeout, RetryTime, pInitParam, null);
                        System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXInitialize {0} - Result of PviXInitialize {1} for remote server {2} - Comm. Instance: {3}",
                                                            DateTime.Now.ToString("HH:mm:ss.fff"), returnValue, pInitParam, linkID);
                    }

                }
            }
            return (returnValue);
        }

        public int PviXDeinitialize()
        {
            int returnValue = 0;
            unsafe
            {
                if (System.Environment.Is64BitProcess)
                {
                    returnValue = PviComApi._PviXDeinitialize64Bit(linkID);
                }
                else
                {
                    returnValue = PviComApi._PviXDeinitialize32Bit(linkID);
                }
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXDeinitialize {0} - Result of PviXDeinitialize {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), returnValue);
                linkID = 0;
            }
            return (returnValue);
        }

        public int SetPviGlobalEvents(IntPtr ptrConnectCallback, IntPtr ptrDisconnectCallback, IntPtr ptrArrangeCallback, IntPtr eventParam)
        {
            if (linkID == 0)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.SetPviGlobalEvents {0} - Invalid linkID",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (-1);
            }

            int returnValue = 0;

            unsafe
            {
                if (System.Environment.Is64BitProcess)
                {
                    returnValue = PviComApi._PviXSetGlobEventMsg64Bit(linkID, POBJ_EVENT_PVI_CONNECT, ptrConnectCallback.ToPointer(), SET_PVICALLBACK_DATA, eventParam);
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.SetPviGlobalEvents {0} - Result of PviXSetGlobEventMsg for POBJ_EVENT_PVI_CONNECT {1}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), returnValue);
                    if (returnValue == 0)
                    {
                        returnValue = PviComApi._PviXSetGlobEventMsg64Bit(linkID, POBJ_EVENT_PVI_DISCONN, ptrDisconnectCallback.ToPointer(), SET_PVICALLBACK_DATA, eventParam);
                        System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.SetPviGlobalEvents {0} - Result of PviXSetGlobEventMsg for POBJ_EVENT_PVI_DISCONN {1}",
                                                           DateTime.Now.ToString("HH:mm:ss.fff"), returnValue);
                    }
                    if (returnValue == 0)
                    {
                        returnValue = PviComApi._PviXSetGlobEventMsg64Bit(linkID, POBJ_EVENT_PVI_ARRANGE, ptrArrangeCallback.ToPointer(), SET_PVICALLBACK_DATA, eventParam);
                        System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.SetPviGlobalEvents {0} - Result of PviXSetGlobEventMsg for POBJ_EVENT_PVI_ARRANGE {1}",
                                                           DateTime.Now.ToString("HH:mm:ss.fff"), returnValue);
                    }
                }
                else
                {
                    returnValue = PviComApi._PviXSetGlobEventMsg32Bit(linkID, POBJ_EVENT_PVI_CONNECT, ptrConnectCallback.ToPointer(), SET_PVICALLBACK_DATA, eventParam);
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.SetPviGlobalEvents {0} - Result of PviXSetGlobEventMsg for POBJ_EVENT_PVI_CONNECT {1}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), returnValue);
                    if (returnValue == 0)
                    {
                        returnValue = PviComApi._PviXSetGlobEventMsg32Bit(linkID, POBJ_EVENT_PVI_DISCONN, ptrDisconnectCallback.ToPointer(), SET_PVICALLBACK_DATA, eventParam);
                        System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.SetPviGlobalEvents {0} - Result of PviXSetGlobEventMsg for POBJ_EVENT_PVI_DISCONN {1}",
                                                           DateTime.Now.ToString("HH:mm:ss.fff"), returnValue);
                    }
                    if (returnValue == 0)
                    {
                        returnValue = PviComApi._PviXSetGlobEventMsg32Bit(linkID, POBJ_EVENT_PVI_ARRANGE, ptrArrangeCallback.ToPointer(), SET_PVICALLBACK_DATA, eventParam);
                        System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.SetPviGlobalEvents {0} - Result of PviXSetGlobEventMsg for POBJ_EVENT_PVI_ARRANGE {1}",
                                                           DateTime.Now.ToString("HH:mm:ss.fff"), returnValue);
                    }
                }
            }

            return (returnValue);
        }

        public int PviXCreateRequest(IntPtr callbackFunc, BrPviPviObject pviObj, IntPtr lparamPviObj)
        {
            if (linkID == 0)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXCreateRequest {0} - Invalid linkID",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (-1);
            }

            int returnValue = 0;
            unsafe
            {
                if (System.Environment.Is64BitProcess)
                {
                    returnValue = PviComApi._PviXCreateRequest64Bit(linkID, pviObj.Name, (uint)pviObj.Type, pviObj.ConnDescr, callbackFunc.ToPointer(), SET_PVICALLBACK_DATA, lparamPviObj, pviObj.LinkDescr, callbackFunc.ToPointer(), SET_PVICALLBACK_DATA, lparamPviObj);
                }
                else
                {
                    returnValue = PviComApi._PviXCreateRequest32Bit(linkID, pviObj.Name, (uint)pviObj.Type, pviObj.ConnDescr, callbackFunc.ToPointer(), SET_PVICALLBACK_DATA, lparamPviObj, pviObj.LinkDescr, callbackFunc.ToPointer(), SET_PVICALLBACK_DATA, lparamPviObj);
                }
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXCreateRequest {0} - Result of PviXCreateRequest {1} for PVI Obj {2}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), returnValue, pviObj.Name);
            }

            return (returnValue);
        }
                
        public int PviXWriteRequest(IntPtr callbackFunc, PviWriteObj pviObjToBeWritten, IntPtr lparamPviObj, uint nAccess)
        {
            if (linkID == 0)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXWriteRequest {0} - Invalid linkID",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (-1);
            }
            if (pviObjToBeWritten.PviObj.LinkID == 0)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXWriteRequest {0} - Invalid LinkID for the PVI object {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), pviObjToBeWritten.PviObj.Name);
                return (-2);
            }

            if(pviObjToBeWritten.DataLength == 0)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXWriteRequest {0} - Invalid length of data to be written for the PVI object {1}",
                                                    DateTime.Now.ToString("HH:mm:ss.fff"), pviObjToBeWritten.PviObj.Name);
                return (-2);
            }

#if DEBUG
            string debugMessage = String.Format("BR_DEBUG - PviComManager.PviXWriteRequest {0} - writing PVI object {1} - Data bytes: ",
                                                DateTime.Now.ToString("HH:mm:ss.fff"), pviObjToBeWritten.PviObj.Name);
            for(uint i=0; i< pviObjToBeWritten.DataLength; i++)
            {
                debugMessage += String.Format("[0x{0:X}]", pviObjToBeWritten.DataArray[i]);
            }
            System.Diagnostics.Debug.WriteLine(debugMessage);
#endif

            int returnValue = 0;
            // Allocate a pointer (unmanaged memory) for the data buffer
            IntPtr dataPnt = Marshal.AllocHGlobal((int)pviObjToBeWritten.DataLength);
            unsafe
            {
                // Copy the array to unmanaged memory
                Marshal.Copy(pviObjToBeWritten.DataArray, 0, dataPnt, (int)pviObjToBeWritten.DataLength);

                if (System.Environment.Is64BitProcess)
                {
                    returnValue = PviComApi._PviXWriteRequest64Bit(linkID, pviObjToBeWritten.PviObj.LinkID, nAccess, (void*)dataPnt, pviObjToBeWritten.DataLength, callbackFunc.ToPointer(), SET_PVICALLBACK_DATA, lparamPviObj);
                }
                else
                {
                    returnValue = PviComApi._PviXWriteRequest32Bit(linkID, pviObjToBeWritten.PviObj.LinkID, nAccess, (void*)dataPnt, pviObjToBeWritten.DataLength, callbackFunc.ToPointer(), SET_PVICALLBACK_DATA, lparamPviObj);
                }
            }

            return (returnValue);
        }
       
        public int PviXReadRequest(IntPtr callbackFunc, BrPviPviObject pviObj, uint accessType, IntPtr lparamPviObj)
        {
            if (linkID == 0)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXReadRequest {0} - Invalid linkID",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (-1);
            }
            if (pviObj.LinkID == 0)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXReadRequest {0} - Invalid LinkID for the PVI object {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name);
                return (-2);
            }

            int returnValue = 0;
            unsafe
            {
                if (System.Environment.Is64BitProcess)
                {
                    returnValue = PviComApi._PviXReadRequest64Bit(linkID, pviObj.LinkID, accessType, callbackFunc.ToPointer(), SET_PVICALLBACK_DATA, lparamPviObj);
                }
                else
                {
                    returnValue = PviComApi._PviXReadRequest32Bit(linkID, pviObj.LinkID, accessType, callbackFunc.ToPointer(), SET_PVICALLBACK_DATA, lparamPviObj);
                }
            }

            System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - PviComManager.PviXReadRequest {0} - Sent read request for PVI object {1} - Access Type: {2} - Return Value: {3}",
                                                DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, accessType, returnValue));

            return (returnValue);
        }

        public int PviXDeleteRequest(IntPtr callbackFunc, BrPviPviObject pviObj, IntPtr lparamPviObj)
        {
            if (linkID == 0)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXDeleteRequest {0} - Invalid linkID",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"));
                return (-1);
            }
            if (pviObj.LinkID == 0)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComManager.PviXDeleteRequest {0} - Invalid LinkID for the PVI object {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name);
                return (-2);
            }

            int returnValue = 0;
            unsafe
            {
                if (System.Environment.Is64BitProcess)
                {
                    returnValue = PviComApi._PviXDeleteRequest64Bit(linkID, pviObj.Name, callbackFunc.ToPointer(), SET_PVICALLBACK_DATA, lparamPviObj);
                }
                else
                {
                    returnValue = PviComApi._PviXDeleteRequest32Bit(linkID, pviObj.Name, callbackFunc.ToPointer(), SET_PVICALLBACK_DATA, lparamPviObj);
                }
            }

            System.Diagnostics.Debug.WriteLine(String.Format("BR_DEBUG - PviComManager.PviXDeleteRequest {0} - Sent read request for PVI object {1}-{2} - Return Value: {3}",
                                                DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.LinkID, pviObj.Name, returnValue));

            return (returnValue);
        }

        #endregion

        #region Properties

        /// <summary>
        /// This property stores the last error description.
        /// </summary>
        private string _ErrorDescription;
        public string ErrorDescription
        {
            get
            {
                return _ErrorDescription;
            }
        }
        #endregion
    }
}
