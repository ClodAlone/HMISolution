using System;
using System.Collections.Generic;
using System.Linq;
using DriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace PhoenixContactPLCI
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PhoenixContactPLCICommJobSettings : CommJobSettings
    {
        #region Constructors

        public PhoenixContactPLCICommJobSettings(Session session, PhoenixContactPLCICommJob job)
            : base(session, job)
        {
            _Address = job.Address;
            //_ShortAddress = job.ShortAddress;
            _DataFormat = job.DataFormat;
            _StringLength= job.StringLength;
            
            _ParseOk = job.ParseOk;
                        
            //_TagName = job.TagName;
        }

        public PhoenixContactPLCICommJobSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        protected PhoenixContactPLCICommJobSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            _Address = String.Empty;
            _DataFormat = PhoenixContactPLCI.PhoenixContactPLCIProtocol.VarType.UNKNOWN;
            _StringLength = 0;

            _ParseOk = false;
        }


        #region Properties
                
        private string _Address;
        public string Address
        {
            get { return _Address; }
            set { SetPropertyValue("Address", ref _Address, value); }
        }

        private PhoenixContactPLCI.PhoenixContactPLCIProtocol.VarType _DataFormat;
        public PhoenixContactPLCI.PhoenixContactPLCIProtocol.VarType DataFormat
        {
            get { return _DataFormat; }
            set { SetPropertyValue("DataFormat", ref _DataFormat, value); }
        }

        private uint _StringLength;
        public uint StringLength
        {
            get { return _StringLength; }
            set { SetPropertyValue("StringLength", ref _StringLength, value); }
        }

        //private string _ShortAddress;
        //public string ShortAddress
        //{
        //    get { return _ShortAddress; }
        //    set { SetPropertyValue("ShortAddress", ref _ShortAddress, value); }
        //}

        private bool _ParseOk;
        public bool ParseOk
        {
            get { return _ParseOk; }
            set { _ParseOk = value; }
        }
           
        #endregion


        #region IDataErrorInfo Members
        #endregion

    }
}
