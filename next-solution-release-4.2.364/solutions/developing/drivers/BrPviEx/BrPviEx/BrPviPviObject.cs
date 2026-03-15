using DriverCodeBaseEx.Enumerators;
using System;
using System.Runtime.InteropServices;
using System.Threading;

namespace BrPvi
{
    public enum PviObjectTypes : uint
    {
        POBJ_PVI,
        POBJ_LINE,
        POBJ_DEVICE,
        POBJ_STATION,
        POBJ_CPU,
        POBJ_MODULE,
        POBJ_TASK,
        POBJ_PVAR
    }

    public enum PviObjectStatus: uint
    {
        MustBeCreated,
        CreationPending,
        NotCreated,
        Ready,
        DataReceived,
        MustBeDeleted,
        NotDeleted,
        Deleted,
        DataInfoReceived,
        UnMappedOnPlc
    }

    // Request/response/event mode
    public enum PviObjectModes : uint
    {
        POBJ_MODE_NULL,             // undefiened
        POBJ_MODE_EVENT,            // event
        POBJ_MODE_READ,             // read
        POBJ_MODE_WRITE,            // write
        POBJ_MODE_CREATE,           // create object
        POBJ_MODE_DELETE,           // delete object
        POBJ_MODE_LINK,             // link object
        POBJ_MODE_CHGLINK,          // change link object
        POBJ_MODE_UNLINK            // unlink object
    }

    public class BrPviPviObject : Object , ICloneable
    {
        #region Constructors

        public BrPviPviObject(string stationName)
        {
            DataOrErrorReceived = new ManualResetEvent(false);
            _StationName = stationName;
            Reset();
        }

        #endregion

        #region Data Members
        Object lockDataAccessObj = new object();
        public ManualResetEvent DataOrErrorReceived;
        #endregion

        #region Methods

        void Reset()
        {
            _Name = String.Empty;
            _Type = PviObjectTypes.POBJ_PVI;
            _ConnDescr = String.Empty;
            _LinkDescr = String.Empty;
            _LinkID = 0;
            _Status = PviObjectStatus.MustBeCreated;
            _LastError = 0;
            _DataLength = 0;
            _EvMask = string.Empty;
            SetDataEvent(false);
        }

        public bool IsStationObject()
        {
            return (Type != PviObjectTypes.POBJ_TASK && Type != PviObjectTypes.POBJ_PVAR);
        }

        public bool IsVarObject()
        {
            return (Type == PviObjectTypes.POBJ_TASK || Type == PviObjectTypes.POBJ_PVAR);
        }

        public bool IsVarObjectOnly()
        {
            return Type == PviObjectTypes.POBJ_PVAR;
        }

        public bool IsCpuMessage()
        {
            return Type == PviObjectTypes.POBJ_CPU;
        }

        public unsafe void SetData(void* pData, uint dataLen)
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
                        Status = PviObjectStatus.DataReceived;
                        //System.Diagnostics.Debug.WriteLine("BR_DEBUG - BrPviPviObject.SetData {0} - Copied {1} data bytes for PviObj: {2}",
                        //                                   DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, Name);

#if DEBUG
                        string debugMessage = String.Format("BR_DEBUG - BrPviPviObject.SetData {0} - Copied {1} data bytes for PviObj: {2} - Data Bytes: ",
                                                           DateTime.Now.ToString("HH:mm:ss.fff"), dataLen, Name);
                        for (uint i = 0; i < DataLength; i++)
                        {
                            debugMessage += String.Format("[0x{0:X}]", DataArray[i]);
                        }
                        System.Diagnostics.Debug.WriteLine(debugMessage);
#endif
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviObjEvent.SetData {0} - Exception: {1}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), ex.Message);
            }
        }

        public void ResetData()
        {
            lock (lockDataAccessObj)
            {
                _DataLength = 0;
                _DataArray = new byte[0];
            }
        }

        public void UpdateValue(byte[] dataBuffer, uint dataLen)
        {
            try
            {
                lock (lockDataAccessObj)
                {
                    DataLength = dataLen;
                    _DataArray = new byte[DataLength];
                    dataBuffer.CopyTo(_DataArray, 0);
                    Status = PviObjectStatus.DataReceived;
                    System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviObjEvent.UpdateValue {0} - Obj: {1} - Copied {2} bytes in the data buffer",
                                                       DateTime.Now.ToString("HH:mm:ss.fff"), Name, dataLen);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("BR_DEBUG - PviObjEvent.UpdateValue {0} - Obj: {1} - Exception: {2}",
                                                   DateTime.Now.ToString("HH:mm:ss.fff"), Name, ex.Message);
            }
        }

        public void SetDataEvent(bool set = true)
        {
            lock(lockDataAccessObj)
            {
                if(set)
                {
                    DataOrErrorReceived.Set();
                }
                else
                {
                    DataOrErrorReceived.Reset();
                }
            }
        }

        public bool IsLastErrorFatal()
        {
            return (LastError == (uint)DriverErrorCodes.ErrorTimeOut || LastError == BrPviProcol.PVI_EVENT_NO_CONNECTION_AVAILABLE_TO_THE_PLC_ERROR || (LastError >= BrPviProcol.PVI_EVENT_SYSTEM_ERROR_MIN_RANGE && LastError <= BrPviProcol.PVI_EVENT_SYSTEM_ERROR_MAX_RANGE));
        }

        public bool IsNewDataEvent()
        {
            return (Status == PviObjectStatus.DataReceived || (_DataArray != null && _DataArray.Length > 0));
        }
        #endregion

        #region Properties

        private string _Name;
        public string Name
        {
            get
            {
                return (_Name);
            }

            set
            {
                _Name = value;
            }
        }

        private PviObjectTypes _Type;
        public PviObjectTypes Type
        {
            get
            {
                return (_Type);
            }

            set
            {
                _Type = value;
            }
        }

        private string _ConnDescr;
        public string ConnDescr
        {
            get
            {
                return (_ConnDescr);
            }

            set
            {
                _ConnDescr = value;
            }
        }

        private string _LinkDescr;
        public string LinkDescr
        {
            get
            {
                return (_LinkDescr);
            }

            set
            {
                _LinkDescr = value;
            }
        }

        private uint _LinkID;
        public uint LinkID
        {
            get
            {
                lock (lockDataAccessObj)
                {
                    return (_LinkID);
                }
            }

            set
            {
                lock (lockDataAccessObj)
                {
                    _LinkID = value;
                }
            }
        }

        private PviObjectStatus _Status;
        public PviObjectStatus Status
        {
            get
            {
                lock(lockDataAccessObj)
                {
                    return (_Status);
                }
            }

            set
            {
                lock (lockDataAccessObj)
                {
                    _Status = value;
                }
            }
        }

        private uint _LastError;
        public uint LastError
        {
            get
            {
                lock (lockDataAccessObj)
                {
                    return (_LastError);
                }
            }

            set
            {
                lock (lockDataAccessObj)
                {
                    _LastError = value;
                }
            }
        }

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

        /// <summary>
        /// Events mask of PVIMonitor's element used to activate/deactivate it
        /// </summary>
        private string _EvMask;
        public string EvMask
        {
            get
            {
                return (_EvMask);
            }

            set
            {
                _EvMask = value;
            }
        }

        private string _StationName;
        public string StationName
        {
            get
            {
                return (_StationName);
            }

            set
            {
                _StationName = value;
            }
        }
        #endregion

        public object Clone()
        {
            return MemberwiseClone();
        }
    }
}
