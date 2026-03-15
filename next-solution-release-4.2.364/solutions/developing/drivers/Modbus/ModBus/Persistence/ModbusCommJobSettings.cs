using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;
namespace ModBus
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ModbusCommJobSettings : CommJobSettings
    {
        #region Constructors

        public ModbusCommJobSettings(Session session, ModbusCommJob job)
            : base(session, job)
        {
            _FunctionCode = job.FunctionCode;
            _StartAddress = job.StartAddress;
            _FileNumber = job.FileNumber;
            _BroadCast = job.BroadCast;
        }
        
        public ModbusCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected ModbusCommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _FunctionCode = FunctionCodes.MultipleRegisters;
            _StartAddress = 0;
            _FileNumber = 0;
            _BroadCast = false;
        }

        #region Properties

        private FunctionCodes _FunctionCode;
        public FunctionCodes FunctionCode
        {
            get
            {
                return _FunctionCode;
            }
            set
            {
                SetPropertyValue("FunctionCode", ref _FunctionCode, value);
            }
        }
        private UInt16 _StartAddress;
        public UInt16 StartAddress
        {
            get
            {
                return _StartAddress;
            }
            set
            {
                SetPropertyValue("StartAddress", ref _StartAddress, value);
            }
        }

        private uint _FileNumber;
        public uint FileNumber
        {
            get
            {
                return _FileNumber;
            }
            set
            {
                SetPropertyValue("FileNumber", ref _FileNumber, value);
            }
        }

        private bool _BroadCast;
        public bool BroadCast
        {
            get
            {
                return _BroadCast;
            }
            set
            {
                SetPropertyValue("BroadCast", ref _BroadCast, value);
            }
        }
        #endregion


        #region IDataErrorInfo Members

        #endregion
    }
}
