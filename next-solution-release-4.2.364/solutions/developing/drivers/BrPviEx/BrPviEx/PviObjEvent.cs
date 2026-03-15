using System;
using System.Runtime.InteropServices;

namespace BrPvi
{
    public class PviObjEvent : Object , ICloneable
    {
        #region Constructors
        public PviObjEvent(PviEventInfo info, BrPviPviObject pviObj)
        {
            EventInfo = info;
            PviObj = pviObj;
            DataLength = 0;
        }
        #endregion

        #region data fields
        public PviEventInfo EventInfo;
        public BrPviPviObject PviObj;
        object lockDataAccessObj = new object();
        #endregion

        #region Methods
        public unsafe void SetData(void* pData, uint dataLen)
        {
            //            try
            //            {
            //                lock(lockDataAccessObj)
            //                {
            //                    if ((pData != null) && (dataLen > 0))
            //                    {
            //                        DataLength = dataLen;
            //                        _DataArray = new byte[DataLength];
            //                        byte* dataBuffer = (byte*)pData;
            //                        Marshal.Copy((IntPtr)dataBuffer, _DataArray, 0, (int)DataLength);
            //                        //System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviObjEvent.SetData {0} - Copied {1} data bytes for PviObj: {2}",
            //                        //                                   DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, PviObj.Name);

            //#if DEBUG
            //                        string debugMessage = String.Format("BR_DEBUG - PviObjEvent.SetData {0} - Copied {1} data bytes for PviObj: {2} - Data Bytes: ",
            //                                                           DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, PviObj.Name);
            //                        for (uint i = 0; i < DataLength; i++)
            //                        {
            //                            debugMessage += String.Format("[0x{0:X}]", DataArray[i]);
            //                        }
            //                        System.Diagnostics.Debug.WriteLine(debugMessage);
            //#endif
            //                    }
            //                }
            //            }
            //            catch(Exception ex)
            //            {
            //                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviObjEvent.SetData {0} - Exception: {1}",
            //                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
            //            }
            PviObj.SetData(pData, dataLen);
        }

        public unsafe void SetDataInfo(void* pData, uint dataLen)
        {
            try
            {
                lock (lockDataAccessObj)
                {
                    if ((pData != null) && (dataLen > 0))
                    {
                        DataLength = dataLen;
                        _DataArray = new byte[DataLength];
                        byte* dataBuffer = (byte*)pData;
                        Marshal.Copy((IntPtr)dataBuffer, _DataArray, 0, (int)DataLength);
#if DEBUG
                        //string debugMessage = String.Format("BR_DEBUG - PviObjEvent.SetDataInfo {0} - Copied {1} data bytes for PviObj: {2} - Data Bytes: ",
                        //                                   DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, PviObj.Name);
                        //for (uint i = 0; i < DataLength; i++)
                        //{
                        //    debugMessage += String.Format("[0x{0:X}]", DataArray[i]);
                        //}
                        string debugMessage = String.Format("BR_DEBUG - PviObjEvent.SetDataInfo {0} - Copied {1} data bytes for PviObj: {2}",
                                                           DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, PviObj.Name);
                        System.Diagnostics.Debug.WriteLine(debugMessage);
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviObjEvent.SetDataInfo {0} - Exception: {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
            }
        }
        #endregion

        #region Properties
        private uint _DataLength;
        public uint DataLength
        {
            get
            {
                return _DataLength;
            }
            set
            {
                _DataLength = value;
            }
        }

        private byte[] _DataArray;
        public byte[] DataArray
        {
            get
            {
                lock(lockDataAccessObj)
                {
                    if(_DataArray == null)
                    {
                        _DataArray = new byte[0];
                    }
                    return _DataArray;
                }
            }
        }
        #endregion

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
