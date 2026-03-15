using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SerialDriverCodeBase;
using DevExpress.Xpo;
using System.ComponentModel;

namespace RMS621
{
    [MapInheritance(MapInheritanceType.ParentTable)]
    public class RMS621ChannelSettings : SerialChannelSettings
    {
        #region Constructors

        public RMS621ChannelSettings(Session session)
            : base(session)
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        private RMS621ChannelSettings()
        {
            // This constructor is used when an object is loaded from a persistent storage.
            // Do not place any code here.
        }

        #endregion

        public void DefaultSettings()
        {
            base.DefaultSettings();
            CommPortName = "Com1";
            CommPortBaudRate = 9600;
            CommPortDataBits = 8;
            CommPortParity = (int)System.IO.Ports.Parity.None;
            CommPortStopBits = (int)System.IO.Ports.StopBits.One;
            CommPortHandshake = (int)System.IO.Ports.Handshake.None;
            CommPortRtsEnable = false;
            CommPortDtrEnable = false;
            CommPortReadTimeout = 5000;
            CommPortWriteTimeout = 5000;
        }

        public void CopyProperties(RMS621ChannelSettings ch)
        {
            base.CopyProperties(ch);
        }

        #region IDataErrorInfo Members

        protected override String PerformValidation(String propertyName)
        {
            string sBase = base.PerformValidation(propertyName);
            if (sBase != null)
            {
                return sBase;
            }

            return null;
        }

        #endregion
    }
}
