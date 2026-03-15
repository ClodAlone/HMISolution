using System;
using System.Collections.Generic;
using System.Linq;
using SerialDriverCodeBaseEx;
using DevExpress.Xpo;
using System.ComponentModel;

namespace PPI
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class PPIChannelSettings : SerialChannelSettings
    {
        #region Constructors

        public PPIChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private PPIChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }
        
        
        #endregion

        #region methods

        public void DefaultSettings()
        {
            base.DefaultSettings();

            CommPortName = "Com1";
            CommPortBaudRate = 19200;
            CommPortDataBits = 8;
            CommPortParity = (int)System.IO.Ports.Parity.Even;
            CommPortStopBits = (int)System.IO.Ports.StopBits.One;
            CommPortHandshake = (int)System.IO.Ports.Handshake.None;
            CommPortRtsEnable = false;
            CommPortDtrEnable = false;
            CommPortReadTimeout = 5000;
            CommPortWriteTimeout = 5000;

            _DriverAddress = 1;
            _UseSignal = false;
        }

        public void CopyProperties(PPIChannelSettings ch)
        {
            base.CopyProperties(ch);
            _DriverAddress = ch.DriverAddress;
            _UseSignal = ch.UseSignal;
        }

        #endregion

        #region Properties

        private byte _DriverAddress;
        public byte DriverAddress
        {
            get { return _DriverAddress; }
            set { SetPropertyValue("DriverAddress", ref _DriverAddress, value); }
        }

        private bool _UseSignal;
        public bool UseSignal
        {
            get { return _UseSignal; }
            set { SetPropertyValue("UseSignal", ref _UseSignal, value); }
        }

        #endregion


        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
                return sBase;

            if (propertyName == "DriverAddress")
            {
                if (_DriverAddress > PPIProtocol.PPI_MAX_ID || _DriverAddress < PPIProtocol.PPI_MIN_ID)
                    return Properties.Resources.ErrorInvalidPPIAddress;
            }

            return null;
        }

        #endregion
    }
}
