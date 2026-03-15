using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace BrPvi
{
    public class PviComEvents : Object
    {
        #region Constructors
        public PviComEvents()
        {
        }

        public PviComEvents(BrPviChannel channel)
        {
            correspondingChannel = channel;
        }
        #endregion

        #region Data members
        BrPviChannel correspondingChannel;
        #endregion

        #region Methods
        public unsafe void PviConnectCallback64Bit(ulong wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviConnectCallback64Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviConnectCallback64Bit {0} - dataLen: {1} - pInfo.LinkID: {2} - pInfo.nMode: {3} - pInfo.nType: {4} - pInfo.ErrCode: {5} - pInfo.Status: {6}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviEventInfo globalEventInfo = new PviEventInfo(*pInfo);
                    GCHandle gcChannel = GCHandle.FromIntPtr(lParam);
                    BrPviChannel channel = (BrPviChannel)gcChannel.Target;
                    channel.AddGlobalEvent(globalEventInfo);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviConnectCallback64Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviDisconnectCallback64Bit(ulong wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviDisconnectCallback64Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviDisconnectCallback64Bit {0} - dataLen: {1} - pInfo.LinkID: {2} - pInfo.nMode: {3} - pInfo.nType: {4} - pInfo.ErrCode: {5} - pInfo.Status: {6}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviEventInfo globalEventInfo = new PviEventInfo(*pInfo);
                    GCHandle gcChannel = GCHandle.FromIntPtr(lParam);
                    BrPviChannel channel = (BrPviChannel)gcChannel.Target;
                    channel.AddGlobalEvent(globalEventInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviDisconnectCallback64Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviArrangeCallback64Bit(ulong wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviArrangeCallback64Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviArrangeCallback64Bit {0} - dataLen: {1} - pInfo.LinkID: {2} - pInfo.nMode: {3} - pInfo.nType: {4} - pInfo.ErrCode: {5} - pInfo.Status: {6}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviEventInfo globalEventInfo = new PviEventInfo(*pInfo);
                    GCHandle gcChannel = GCHandle.FromIntPtr(lParam);
                    BrPviChannel channel = (BrPviChannel)gcChannel.Target;
                    channel.AddGlobalEvent(globalEventInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviArrangeCallback64Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviDataCallback64Bit(ulong wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviDataCallback64Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    PviEventInfo eventInfo = new PviEventInfo(*pInfo);
                    GCHandle gcPviObj = GCHandle.FromIntPtr(lParam);
                    BrPviPviObject pviObj = (BrPviPviObject)gcPviObj.Target;
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviDataCallback64Bit {0} - PviObj: {1} - dataLen: {2} - pInfo.LinkID: {3} - pInfo.nMode: {4} - pInfo.nType: {5} - pInfo.ErrCode: {6} - pInfo.Status: {7}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviObjEvent pviEvent = new PviObjEvent(eventInfo, pviObj);
                    if((eventInfo.nMode == (uint)PviObjectModes.POBJ_MODE_EVENT) && (eventInfo.ErrCode == 0) && (pData != null))
                    {
                        pviEvent.PviObj.Status = PviObjectStatus.DataReceived;
                        if(dataLen > 0)
                        {
                            pviEvent.SetData(pData, dataLen);
                        }
                    }
                    else if((eventInfo.nMode == (uint)PviObjectModes.POBJ_MODE_READ) && (eventInfo.ErrCode == 0) && (pData != null))
                    {
                        pviEvent.PviObj.Status = PviObjectStatus.DataInfoReceived;
                        if (dataLen > 0)
                        {
                            pviEvent.SetDataInfo(pData, dataLen);
                        }
                    }
                    if(correspondingChannel != null)
                    {
                        correspondingChannel.AddPviEvent(pviEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviDataCallback64Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviConnectCallback32Bit(uint wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviConnectCallback32Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviConnectCallback32Bit {0} - dataLen: {1} - pInfo.LinkID: {2} - pInfo.nMode: {3} - pInfo.nType: {4} - pInfo.ErrCode: {5} - pInfo.Status: {6}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviEventInfo globalEventInfo = new PviEventInfo(*pInfo);
                    GCHandle gcChannel = GCHandle.FromIntPtr(lParam);
                    BrPviChannel channel = (BrPviChannel)gcChannel.Target;
                    channel.AddGlobalEvent(globalEventInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviConnectCallback32Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviDisconnectCallback32Bit(uint wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviDisconnectCallback32Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviDisconnectCallback32Bit {0} - dataLen: {1} - pInfo.LinkID: {2} - pInfo.nMode: {3} - pInfo.nType: {4} - pInfo.ErrCode: {5} - pInfo.Status: {6}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviEventInfo globalEventInfo = new PviEventInfo(*pInfo);
                    GCHandle gcChannel = GCHandle.FromIntPtr(lParam);
                    BrPviChannel channel = (BrPviChannel)gcChannel.Target;
                    channel.AddGlobalEvent(globalEventInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviDisconnectCallback32Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviArrangeCallback32Bit(uint wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviArrangeCallback32Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviArrangeCallback32Bit {0} - dataLen: {1} - pInfo.LinkID: {2} - pInfo.nMode: {3} - pInfo.nType: {4} - pInfo.ErrCode: {5} - pInfo.Status: {6}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviEventInfo globalEventInfo = new PviEventInfo(*pInfo);
                    GCHandle gcChannel = GCHandle.FromIntPtr(lParam);
                    BrPviChannel channel = (BrPviChannel)gcChannel.Target;
                    channel.AddGlobalEvent(globalEventInfo);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviArrangeCallback32Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        public unsafe void PviDataCallback32Bit(uint wParam, IntPtr lParam, void* pData, uint dataLen, PviComStruct.T_RESPONSE_INFO* pInfo)
        {
            System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviDataCallback32Bit has been called {0}",
                                               DateTime.Now.ToString("HH:mm:ss.fff"));
            try
            {
                unsafe
                {
                    PviEventInfo eventInfo = new PviEventInfo(*pInfo);
                    GCHandle gcPviObj = GCHandle.FromIntPtr(lParam);
                    BrPviPviObject pviObj = (BrPviPviObject)gcPviObj.Target;
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviDataCallback32Bit {0} - PviObj: {1} - dataLen: {2} - pInfo.LinkID: {3} - pInfo.nMode: {4} - pInfo.nType: {5} - pInfo.ErrCode: {6} - pInfo.Status: {7}",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), pviObj.Name, dataLen, (*pInfo).LinkID, (*pInfo).nMode, (*pInfo).nType, (*pInfo).ErrCode, (*pInfo).Status);
                    PviObjEvent pviEvent = new PviObjEvent(eventInfo, pviObj);
                    if ((eventInfo.nMode == (uint)PviObjectModes.POBJ_MODE_EVENT) && (eventInfo.ErrCode == 0) && (pData != null))
                    {
                        pviEvent.PviObj.Status = PviObjectStatus.DataReceived;
                        if (dataLen > 0)
                        {
                            pviEvent.SetData(pData, dataLen);
                        }
                    }
                    else if ((eventInfo.nMode == (uint)PviObjectModes.POBJ_MODE_READ) && (eventInfo.ErrCode == 0) && (pData != null))
                    {
                        pviEvent.PviObj.Status = PviObjectStatus.DataInfoReceived;
                        if (dataLen > 0)
                        {
                            pviEvent.SetDataInfo(pData, dataLen);
                        }
                    }
                    if (correspondingChannel != null)
                    {
                        correspondingChannel.AddPviEvent(pviEvent);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviComEvents.PviDataCallback32Bit {0} - Exception {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return;
            }
        }

        #endregion
    }
}
