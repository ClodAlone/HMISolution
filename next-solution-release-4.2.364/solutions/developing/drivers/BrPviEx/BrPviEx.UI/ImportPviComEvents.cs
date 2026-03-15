using System;
using System.Runtime.InteropServices;

namespace BrPvi.UI
{
    public class ImportPviComEvents
    {
        #region Constructors
        public ImportPviComEvents(BrPviPlcImportParser importer)//ImportTagsEditorTree importer)
        {
            correspondingImporter = importer;
        }
        #endregion

        #region Data members
        //ImportTagsEditorTree correspondingImporter;
        BrPviPlcImportParser correspondingImporter;
        #endregion

        #region Methods
        public unsafe void PviConnectCallback64Bit(ulong wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviConnectCallback64Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviConnectCallback64Bit {0} - dataLen: {1} - pInfo.LinkID: {2} - pInfo.nMode: {3} - pInfo.nType: {4} - pInfo.ErrCode: {5} - pInfo.Status: {6}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviEventInfo globalEventInfo = new PviEventInfo(*pInfo);
                    correspondingImporter.AddGlobalEvent(globalEventInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviConnectCallback64Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviDisconnectCallback64Bit(ulong wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviDisconnectCallback64Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviDisconnectCallback64Bit {0} - dataLen: {1} - pInfo.LinkID: {2} - pInfo.nMode: {3} - pInfo.nType: {4} - pInfo.ErrCode: {5} - pInfo.Status: {6}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviEventInfo globalEventInfo = new PviEventInfo(*pInfo);
                    correspondingImporter.AddGlobalEvent(globalEventInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviDisconnectCallback64Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviArrangeCallback64Bit(ulong wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviArrangeCallback64Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviArrangeCallback64Bit {0} - dataLen: {1} - pInfo.LinkID: {2} - pInfo.nMode: {3} - pInfo.nType: {4} - pInfo.ErrCode: {5} - pInfo.Status: {6}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviEventInfo globalEventInfo = new PviEventInfo(*pInfo);
                    correspondingImporter.AddGlobalEvent(globalEventInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviArrangeCallback64Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviDataCallback64Bit(ulong wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            //System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviDataCallback64Bit has been called {0}",
            //                                   DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    PviEventInfo eventInfo = new PviEventInfo(*pInfo);
                    GCHandle gcPviObj = GCHandle.FromIntPtr(lParam);
                    BrPviPviObject pviObj = (BrPviPviObject)gcPviObj.Target;
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviDataCallback64Bit {0} - PviObj: {1} - handle: {2} dataLen: {3} - pInfo.LinkID: {4} - pInfo.nMode: {5} - pInfo.nType: {6} - pInfo.ErrCode: {7} - pInfo.Status: {8}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, lParam, dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviObjEvent pviEvent = new PviObjEvent(eventInfo, pviObj);
                    if (eventInfo.nMode == (uint)PviObjectModes.POBJ_MODE_READ)
                    {
                        if ((eventInfo.ErrCode == 0) && (pData != null))
                        {
                            pviObj.Status = PviObjectStatus.DataReceived;
                            pviEvent.SetDataInfo(pData, dataLen);
                        }
                        pviObj.LastError = eventInfo.ErrCode;
                        pviObj.SetDataEvent(true);
                    }
                    if (correspondingImporter != null)
                    {
                        correspondingImporter.AddPviEvent(pviEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviDataCallback64Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviConnectCallback32Bit(uint wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviConnectCallback32Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviConnectCallback32Bit {0} - dataLen: {1} - pInfo.LinkID: {2} - pInfo.nMode: {3} - pInfo.nType: {4} - pInfo.ErrCode: {5} - pInfo.Status: {6}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviEventInfo globalEventInfo = new PviEventInfo(*pInfo);
                    correspondingImporter.AddGlobalEvent(globalEventInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviConnectCallback32Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviDisconnectCallback32Bit(uint wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviDisconnectCallback32Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviDisconnectCallback32Bit {0} - dataLen: {1} - pInfo.LinkID: {2} - pInfo.nMode: {3} - pInfo.nType: {4} - pInfo.ErrCode: {5} - pInfo.Status: {6}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviEventInfo globalEventInfo = new PviEventInfo(*pInfo);
                    correspondingImporter.AddGlobalEvent(globalEventInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviDisconnectCallback32Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviArrangeCallback32Bit(uint wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviArrangeCallback32Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviArrangeCallback32Bit {0} - dataLen: {1} - pInfo.LinkID: {2} - pInfo.nMode: {3} - pInfo.nType: {4} - pInfo.ErrCode: {5} - pInfo.Status: {6}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviEventInfo globalEventInfo = new PviEventInfo(*pInfo);
                    correspondingImporter.AddGlobalEvent(globalEventInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviArrangeCallback32Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviDataCallback32Bit(uint wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            //System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviDataCallback32Bit has been called {0}",
            //                                   DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    PviEventInfo eventInfo = new PviEventInfo(*pInfo);
                    GCHandle gcPviObj = GCHandle.FromIntPtr(lParam);
                    BrPviPviObject pviObj = (BrPviPviObject)gcPviObj.Target;
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviDataCallback32Bit {0} - PviObj: {1} - handle: {2} dataLen: {3} - pInfo.LinkID: {4} - pInfo.nMode: {5} - pInfo.nType: {6} - pInfo.ErrCode: {7} - pInfo.Status: {8}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, lParam, dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviObjEvent pviEvent = new PviObjEvent(eventInfo, pviObj);
                    if (eventInfo.nMode == (uint)PviObjectModes.POBJ_MODE_READ)
                    {
                        if ((eventInfo.ErrCode == 0) && (pData != null))
                        {
                            pviObj.Status = PviObjectStatus.DataReceived;
                            pviEvent.SetDataInfo(pData, dataLen);
                        }
                        pviObj.LastError = eventInfo.ErrCode;
                        pviObj.SetDataEvent(true);
                    }
                    if (correspondingImporter != null)
                    {
                        correspondingImporter.AddPviEvent(pviEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - ImportPviComEvents.PviDataCallback32Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }
        #endregion
    }
}
