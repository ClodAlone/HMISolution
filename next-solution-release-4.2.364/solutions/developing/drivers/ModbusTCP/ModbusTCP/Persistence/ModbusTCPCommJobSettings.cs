using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace ModbusTCP
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class ModbusTCPCommJobSettings : CommJobSettings
    {
                #region Constructors

        public ModbusTCPCommJobSettings(Session session, ModbusTCPCommJob job)
            : base(session, job)
        {
            _FunctionCode = job.FunctionCode;
            _StartAddress = job.StartAddress;
            _FileNumber = job.FileNumber;
            _SwapDWords = job.SwapDWords;
            _StringLength = job.StringLength;
            _BroadCast = job.BroadCast;
        }

        public ModbusTCPCommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected ModbusTCPCommJobSettings()
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
            _SwapDWords = false;
            _StringLength = 32;
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

        private bool _SwapDWords;
        public bool SwapDWords
        {
            get { return _SwapDWords; }
            set
            {
                SetPropertyValue("SwapDWords", ref _SwapDWords, value);
            }
        }
        private uint _StringLength;
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        /// <summary>   String Length of memory area Property. </summary>
        ///
        /// <value> The String Length. </value>
        ////////////////////////////////////////////////////////////////////////////////////////////////////
        public uint StringLength
        {
            get { return _StringLength; }
            set { SetPropertyValue("String Length", ref _StringLength, value); }
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
