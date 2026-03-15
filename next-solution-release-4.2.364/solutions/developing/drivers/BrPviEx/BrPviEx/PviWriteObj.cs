using System;
using System.Text;

namespace BrPvi
{
    public class PviWriteObj : Object
    {
        #region Constructors
        public PviWriteObj(BrPviPviObject pviObj)
        {
            PviObj = pviObj;
            DataLength = 0;
        }
        #endregion

        #region data fields
        public BrPviPviObject PviObj;
        object lockDataAccessObj = new object();
        #endregion

        #region Methods

        public bool SetData(byte[] dataBuffer, uint dataLen)
        {
            try
            {
                lock (lockDataAccessObj)
                {
                    if ((dataBuffer != null) && (dataLen > 0))
                    {
                        DataLength = dataLen;
                        _DataArray = new byte[dataLen];
                        dataBuffer.CopyTo(_DataArray, 0);
#if DEBUG
                        string debugMessage = String.Format("BR_DEBUG - PviWriteObj.SetData {0} - Copied {1} data bytes to be written for PviObj: {2}",
                                                            DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, PviObj.Name);
                        for (uint i = 0; i < DataLength; i++)
                        {
                            debugMessage += String.Format("[0x{0:X}]", _DataArray[i]);
                        }
                        System.Diagnostics.Debug.WriteLine(debugMessage);
#endif

                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviWriteObj.SetData {0} - Exception: {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
                return (false);
            }
            return (true);
        }

        public bool SetEvMaskData(string evMask)
        {
            return SetData(Encoding.ASCII.GetBytes(evMask), (uint)evMask.Length);
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
                lock (lockDataAccessObj)
                {
                    if (_DataArray == null)
                    {
                        _DataArray = new byte[0];
                    }
                    return _DataArray;
                }
            }
        }
        #endregion
    }
}
